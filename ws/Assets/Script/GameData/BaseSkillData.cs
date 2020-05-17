using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseSkillData
{
	public enum SkillDataType
	{
		Hero, Unit
	}

	public bool hasShotEffect = false;
	public string colorEffectId = null;

	public SkillDataType skillDataType = SkillDataType.Hero;

	public bool isPassiveSkill = false;

	public bool isChangeSideSkill = false;

	protected IVector3 _v;
	protected IVector3 _v2;
	
	// 타겟팅 타입 2번일때 6~11번 공격타입은 최대 타겟 거리를 갖는다.
	protected int _targetDistanceLimit = 999999;
	
	public string id;
	public string baseId;
	public string name;
	public int baseLevel;


	public bool hasCoolTime = false;
	public float coolTimeStartDelay = 0.0f;
	
	public Xfloat coolTime = 0.0f;
	
	public int exeType;
	
	public AttackData exeData;
	
	public int targeting;
	public Xint[] targetAttr;
	
	
	public bool isTargetingForward = false;
	
	
	public Skill.Type skillType;
	
	public SkillEffectData[] skillEffects;
	public int totalEffectNum = 0;
	
	public string description;	
	
	
	public Skill.TargetType targetType;
	
	public bool checkMissChance = false;


	public const string E_SHOT_BLUE01 = "E_SHOT_BLUE01"; 
	public const string E_SHOT_YELLOW01 = "E_SHOT_YELLOW01"; 
	public const string E_SHOT_VIOLET01 = "E_SHOT_VIOLET01"; 
	public const string E_SHOT_GREEN01 = "E_SHOT_GREEN01"; 
	public const string E_SHOT_RED01 = "E_SHOT_RED01"; 

	public TranscendData transcendData = null;

	public BaseSkillData ()
	{
	}


	public void shootShotEffect(Vector3 pos, Transform parent)
	{
		if(hasShotEffect) GameManager.info.effectData[colorEffectId].getEffect(-1000,pos, null, parent);
	}



	public delegate bool CheckTargetingType(Monster target);

	public bool canUseThisSkillOnThisType(Monster target)
	{
		for(int i = 0; i < totalEffectNum; ++i)
		{
			if(SkillEffectData.checkApplyTargetType(this, target, skillEffects[i].type) == false)
			{
				continue;
			}

			return true;
		}
		return false;
	}

	
	int calcValue = 0;
	IFloat _resetBulletTime = -1.0f;
	bool _needResetBulletTime = false;


	public bool applySkillEffect(Monster target, int skillLevel, Bullet bullet = null, int applyReinforceLevel = 0, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		_needResetBulletTime = false;

		if(target == null) return false;

		if(bullet != null) // 총알이 붙은 스킬이면... 총알검사 딜레이를 검사한다. 검사해서 통과했으면 세부 사항은 다 검사.(그래서 실제 이펙트 검사시에는 강제로 체크가능하도록 체크타임을 리셋한다.
		{
			if(target.canCheckThisBullet(bullet) == false)
			{
				return false;
			}
			else
			{
				_needResetBulletTime = true;
				_resetBulletTime = target.getBulletRecordTime(bullet);
			}
		}
		
		bool success = false;

		if(UIPopupSkillPreview.isOpen)
		{
			for(int i = 0; i < totalEffectNum; ++i)
			{
				if(_needResetBulletTime) target.resetBulletRecordTime(bullet);

				skillEffects[i].applySkillEffect(this, skillLevel, target, applyReinforceLevel, bullet, attacker, nowApplyDamagePer);
			}

			success = true;
		}
		else
		{
			for(int i = 0; i < totalEffectNum; ++i)
			{
				if(target.isEnabled == false) break;
				
				if(isSuccess(i,applyReinforceLevel))
				{
					if(checkMissChance && target.canAvoid())
					{
						
					}
					else
					{
						if(_needResetBulletTime) target.resetBulletRecordTime(bullet);
						if(skillEffects[i].applySkillEffect(this, skillLevel, target, applyReinforceLevel, bullet, attacker, nowApplyDamagePer))
						{
							success = true;	
						}
					}
				}
			}

		}

		if(_needResetBulletTime)
		{
			target.resetBulletRecordTime(bullet, _resetBulletTime);
		}
		
		if(success == false)
		{
			_v = target.cTransformPosition;

			if(target.isPlayer)
			{
				if(skillType != Skill.Type.BUFF)
				{
					if(bullet == null || target.stat.uniqueId != bullet.attackerInfo.uniqueId)
					{
						if(attacker == null || target.stat.uniqueId != attacker.attackerUniqueId)
						{
							target.showMissEffect(bullet, attacker);
						}
					}
				}
			}
			else
			{
				target.showMissEffect(bullet, attacker);
			}
		}
		else
		{
			if(isPassiveSkill)
			{
				GameManager.info.effectData[EffectData.E_P_HIT].getParticleEffectByCharacterSize(target.stat.uniqueId, target, null, target.tf);
			}
		}
		
		return success;
	}




	public IFloat preDamageCalc(Monster target, int heroSkillLevel, int applyReinforceLevel, Monster attacker = null, float damagePer = 1.0f, float minimumDamagePer =  1.0f)
	{
		IFloat totalPreDamage = 0.0f;
		for(int i = 0; i < totalEffectNum; ++i)
		{
			if(checkMissChance && target.canAvoid()){}
			else
			{
				// 성공 확률 보정값을 곱해준다.
				//_successChance[index][rareType];
				if(skillEffects[i].preSkillDamageCalc != null) 
				{
					totalPreDamage += (skillEffects[i].preSkillDamageCalc(this, heroSkillLevel, target, applyReinforceLevel, attacker, damagePer, minimumDamagePer));// * ((float)_successChance[i][rareType] * 0.01f);
				}
			}
		}

		return totalPreDamage;
	}



	
	
	public bool checkLevelCorrection(int heroSkillLevel, int targetCharacterLevel, Bullet bullet = null, Monster attacker = null)
	{
		//	[구버전]	* 레벨 보정 : 대상유닛레벨 - 스킬레벨  = N 일때, [100 - (N-A)*B] 확률로 효과 적용							
		//		 MIN:0 / MAX:100 , A = 10 , B = 3		
		/*
		int skillPlayerLevel = 0;
		
		if(skillDataType == SkillDataType.Unit)
		{
			if(attacker != null) skillPlayerLevel = attacker.stat.level;
			else if(bullet != null) skillPlayerLevel = bullet.attackerInfo.stat.level;
		}
		else
		{
			skillPlayerLevel = heroSkillLevel;
		}
		
		calcValue = (100 -  ( (targetCharacterLevel - skillPlayerLevel) - 10) * 3);
		*/


		// [신버전] * 레벨 보정 : 대상유닛레벨 - 스킬레벨 = N 일때, [100 - (N - A) * B] 확률로 효과 적용
		//		 MIN:0 / MAX:100 , A = 20 , B = 3		

		int skillPlayerLevel = 0;


//		<대상유닛레벨>
//			* 유저 아군 (or 적) 유닛 & 몬스터 유닛 : 해당 유닛의, 베이스레벨 + 강화레벨
//			* 몬스터 히어로 : 데이타시트에 설정된 레벨
//			* 유저 아군 (or 적) 히어로 : 히어로 장착파츠의 레어등급으로 레벨 구함, 
//		▷ 장비 레벨 = D급 : 5레벨 / C급 : 15레벨 / B급 : 35레벨 / A급 : 60레벨 / S급 : 90레벨
//		▷ 유저히어로의 대상유닛레벨 = (모자 레벨 + 강화레벨(1~20) + 의상 레벨 + 강화레벨 + 무기 레벨 + 강화레벨 + 타는펫 레벨 + 강화레벨) / 4



//		<스킬레벨>
//		* 히어로스킬 : 해당 스킬룬의, 베이스레벨 + 강화레벨
//		* 유닛 액티브/패시브 스킬 : 시전 유닛의, 베이스레벨 + 강화레벨

		if(skillDataType == SkillDataType.Unit)
		{
			if(attacker != null) skillPlayerLevel = attacker.stat.baseLevel + attacker.stat.reinforceLevel;
			else if(bullet != null) skillPlayerLevel = bullet.attackerInfo.stat.baseLevel + bullet.attackerInfo.stat.reinforceLevel;
		}
		else
		{
			skillPlayerLevel = heroSkillLevel;
		}

//		레벨보정 공식 (대상유닛레벨 - 스킬레벨 = N 일때, [100 - (N - A) * B] 확률로 효과 적용) 에서 A값 변경, 20 -> 0으로 

		calcValue = (100 -  ( (targetCharacterLevel - skillPlayerLevel) - 0) * 3);


		
		if(calcValue <= 0) return false;
		else if(calcValue >= 100) return true;
		
		return (GameManager.inGameRandom.Range(0,100)) < calcValue;
	}
	
	
	protected string _temp;	
	private static List<int[]> _ti = new List<int[]>();
	private static List<GameValueType.Type> _tiV = new List<GameValueType.Type>();
	void parseSuccessChance(object obj)
	{
		_temp = (obj).ToString().Trim();
		if(_temp.Length > 0)
		{
			/*
			if(_temp.Contains("/"))
			{
				_ti.Add( Util.stringToIntArray( _temp, '/') );	
				_tiV.Add( GameValueType.Type.Section );
			}
			else 
			*/

			if(_temp.Contains(","))
			{
				_ti.Add( Util.stringToIntArray( _temp, ',') );	
				_tiV.Add( GameValueType.Type.Fixed );
			}
			else
			{
				_ti.Add( Util.stringToIntArray( _temp, '/') );	
				_tiV.Add( GameValueType.Type.Single );
			}


		}
	}	
	
	public int[][] successChance;	
	public GameValueType.Type[] successValueType;
	

	public bool isSuccess(int index, int applyReinforceLevel)
	{

//#if UNITY_EDITOR
//		Debug.LogError("수정 필요");
//		return true;
//#endif

#if UNITY_EDITOR
		try
		{
			GameValueType.Type t = successValueType[index];
		}
		catch
		{
			Debug.LogError(id);
		}
#endif

		switch(successValueType[index])
		{
		case GameValueType.Type.Fixed:

			float v = 0;
			
			// 원래 적용 레벨 수치는 0보다 크다. -값으로 된 것은 현 차징시간이 최소차징시간보다 적을때의 예외상황을 위해.
			// 만들어둔것이다.
			if(applyReinforceLevel < 0)
			{
				v = successChance[index][0] * -(applyReinforceLevel / 100.0f); //* 0.01f);
			}
			else
			{
				v = successChance[index][0] + (successChance[index][1] - successChance[index][0]) / 19f * (applyReinforceLevel - 1);
			}
			
			return MathUtil.RoundToInt(v) > (GameManager.inGameRandom.Range(0,100));

			/*
		case GameValueType.Type.Section:

			int maxValue = successChance[index].Length - 1;

			int a = Mathf.FloorToInt((float)applyReinforceLevel / 5.0f);
			if(a > maxValue) a = maxValue;
			int b = a + 1;
			if(b > maxValue) b = maxValue;
			float c = applyReinforceLevel / 5.0f - a;
			return (Mathf.Round( (successChance[index][a] + c * ( successChance[index][b] - successChance[index][a] ))  * 100.0f) * 0.01f) > (GameManager.inGameRandom.Range(0,100));;
*/

		default:
			return successChance[index][0] > (GameManager.inGameRandom.Range(0,100));
			break;
		}
	}



	public int getSuccessChance(int index, int applyReinforceLevel, int applyRareLevel)
	{
		switch(successValueType[index])
		{
		case GameValueType.Type.Fixed:

			float v = 0;
			
			// 원래 적용 레벨 수치는 0보다 크다. -값으로 된 것은 현 차징시간이 최소차징시간보다 적을때의 예외상황을 위해.
			// 만들어둔것이다.
			if(applyReinforceLevel < 0)
			{
				v = successChance[index][0] * -(applyReinforceLevel / 100.0f);//* 0.01f);
			}
			else
			{
				v = successChance[index][0] + (successChance[index][1] - successChance[index][0]) / 19f * (applyReinforceLevel - 1);
			}
			
			return MathUtil.RoundToInt(v);

		default:
			return successChance[index][0];
			break;
		}
	}


	
	public virtual void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		name = (string)l[k["NAME"]];
		Util.parseObject(l[k["LEVEL"]], out baseLevel, true, 0);

		switch((string)l[k["SKILLTYPE"]])
		{
		case "ATTACK":
			skillType = Skill.Type.ATTACK;	
			targetType = Skill.TargetType.ENEMY;
			checkMissChance = true;
			break;
		case "DEBUFF":
			skillType = Skill.Type.DEBUFF;			
			targetType = Skill.TargetType.ENEMY;
			checkMissChance = true;
			break;	
		case "BUFF":
			skillType = Skill.Type.BUFF;		
			targetType = Skill.TargetType.ME;
			checkMissChance = false;
			break;				
		case "HEAL":
			skillType = Skill.Type.HEAL;	
			targetType = Skill.TargetType.ME;
			checkMissChance = false;
			break;							
		}			
		
		_ti.Clear();
		_tiV.Clear();
		parseSuccessChance(l[k["SUCCESS_CHANCE_1"]]);
		parseSuccessChance(l[k["SUCCESS_CHANCE_2"]]);
		parseSuccessChance(l[k["SUCCESS_CHANCE_3"]]);
		parseSuccessChance(l[k["SUCCESS_CHANCE_4"]]);
		parseSuccessChance(l[k["SUCCESS_CHANCE_5"]]);
		
		int len = _ti.Count;
		
		successChance = new int[len][];
		successValueType = new GameValueType.Type[len];
		
		for(int i = 0; i < len; ++i)
		{
			successChance[i] = _ti[i];
			successValueType[i] = _tiV[i];
		}
		
		_ti.Clear();
		_tiV.Clear();

		_temp = null;
		

		Util.parseObject(l[k["EXE_TYPE"]],out exeType,true, 0);
		
		exeData = AttackData.getAttackData(exeType, l[k["E_ATTR1"]],l[k["E_ATTR2"]],l[k["E_ATTR3"]],l[k["E_ATTR4"]],l[k["E_ATTR5"]],l[k["E_ATTR6"]],l[k["E_ATTR7"]]);
		
		Util.parseObject(l[k["TARGETING_TYPE"]],out targeting, true, 0);
		
		targetAttr = null;
		
		setTargetingChecker(l,k);
		
		skillEffects = new SkillEffectData[hasSkillEffect(l[k["EFFECT_1"]],l[k["EFFECT_2"]],l[k["EFFECT_3"]],l[k["EFFECT_4"]],l[k["EFFECT_5"]])];
		
		totalEffectNum = skillEffects.Length;

#if UNITY_EDITOR

		if(successChance.Length < totalEffectNum)
		{
			Debug.LogError(id + "  successChance error ");
		}

		if(successValueType.Length < totalEffectNum)
		{
			Debug.LogError(id + "  successValueType error ");
		}
#endif


		for(int i = 0; i < totalEffectNum; ++i)
		{
			int startDelay = -1;

			switch(i)
			{
				case 0:
				skillEffects[i] = SkillEffectData.getSkillEffectData(Util.objectToInt(l[k["EFFECT_1"]]), l[k["E1_ATTR1"]], l[k["E1_ATTR2"]]);

				if(k.ContainsKey("E1_DEALY"))
				{
					Util.parseObject(l[k["E1_DEALY"]], out startDelay, -1);
					if(startDelay > 0)
					{
						skillEffects[i].startDelay = ((float)startDelay * 0.001f);
					}
				}

				break;
				case 1:
				skillEffects[i] = SkillEffectData.getSkillEffectData(Util.objectToInt(l[k["EFFECT_2"]]), l[k["E2_ATTR1"]], l[k["E2_ATTR2"]]);

				if(k.ContainsKey("E2_DEALY"))
				{
					Util.parseObject(l[k["E2_DEALY"]], out startDelay, -1);
					if(startDelay > 0)
					{
						skillEffects[i].startDelay = ((float)startDelay * 0.001f);
					}
				}

				break;
				case 2:
				skillEffects[i] = SkillEffectData.getSkillEffectData(Util.objectToInt(l[k["EFFECT_3"]]), l[k["E3_ATTR1"]], l[k["E3_ATTR2"]]);

				if(k.ContainsKey("E3_DEALY"))
				{
					Util.parseObject(l[k["E3_DEALY"]], out startDelay, -1);
					if(startDelay > 0)
					{
						skillEffects[i].startDelay = ((float)startDelay * 0.001f);
					}
				}


				break;
				case 3:
				skillEffects[i] = SkillEffectData.getSkillEffectData(Util.objectToInt(l[k["EFFECT_4"]]), l[k["E4_ATTR1"]], l[k["E4_ATTR2"]]);

				if(k.ContainsKey("E4_DEALY"))
				{
					Util.parseObject(l[k["E4_DEALY"]], out startDelay, -1);
					if(startDelay > 0)
					{
						skillEffects[i].startDelay = ((float)startDelay * 0.001f);
					}
				}


				break;
				case 4:
				skillEffects[i] = SkillEffectData.getSkillEffectData(Util.objectToInt(l[k["EFFECT_5"]]), l[k["E5_ATTR1"]], l[k["E5_ATTR2"]]);

				if(k.ContainsKey("E5_DEALY"))
				{
					Util.parseObject(l[k["E5_DEALY"]], out startDelay, -1);
					if(startDelay > 0)
					{
						skillEffects[i].startDelay = ((float)startDelay * 0.001f);
					}
				}

				break;
			}
			
			skillEffects[i].skillData = this;

			if(skillEffects[i].type == 29)
			{
				isChangeSideSkill = true;
			}
		}


		description = ((string)l[k["DESCRIPTION"]]).Replace("\\n","\n");;
		
	}	
	
	int hasSkillEffect(params object[] objs)
	{
		int count = 0;
		for(int i = 0; i < 5; ++i)
		{
			if(objs[i] is string && objs[i].ToString().Contains(",")) ++count;
			else if(objs[i] is int || objs[i] is long) ++count;
		}
		return count;
	}
	
	
	
	

	public void playSkillSound()
	{
		if(skillDataType == SkillDataType.Hero && GameManager.info.skillIconData.ContainsKey(GameManager.info.heroBaseSkillData[baseId].resource))
		{
			if(string.IsNullOrEmpty(GameManager.info.skillIconData[ GameManager.info.heroBaseSkillData[baseId].resource ].soundId)) return;

#if UNITY_EDITOR
//			if(BattleSimulator.nowSimulation == false)
//			{
//				Debug.Log(GameManager.info.skillIconData[ GameManager.info.heroBaseSkillData[baseId].resource ].soundId);
//			}
#endif

			SoundData.play(GameManager.info.skillIconData[ GameManager.info.heroBaseSkillData[baseId].resource ].soundId);
		}
	}


	
	
	
	
	
	
///==================================================================================================== ///	
// 타겟팅 관련!!!!!
// 타겟팅 관련!!!!!
// 타겟팅 관련!!!!!	
///==================================================================================================== ///	
	
	public virtual void setTargetingChecker(List<object> l, Dictionary<string, int> k)
	{

		switch(targeting)
		{
		case TargetingData.FIXED_1:
			targetAttr = new Xint[2];	
			Util.parseObject(l[k["T_ATTR1"]], out targetAttr[0], true, 0);
			Util.parseObject(l[k["T_ATTR2"]], out targetAttr[1], true, 0);
			break;			
		case TargetingData.AUTOMATIC_2:
			targetAttr = new Xint[2];
			Util.parseObject(l[k["T_ATTR1"]], out targetAttr[0], true, 0);
			Util.parseObject(l[k["T_ATTR2"]], out targetAttr[1], true, 0);
			break;
		}
		
		setTargetingChecker2();
	}


	public void setTargetingChecker2()
	{
		isTargetingForward = (targeting == TargetingData.FORWARD_LINEAR_3);
		
		switch(exeType)
		{
		case 3: 
		case 4:
		case 5:				
			if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2WithOutDistCheck; 
				playerTargetingChecker =  canPlayerHeroTargetingType2WithOutDistCheck;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			else if(targeting == TargetingData.FORWARD_LINEAR_3)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType3; 					
				playerTargetingChecker =  canPlayerHeroTargetingType3;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType3PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType3PVP2;
			}
			break;
		case 6:	
			_targetDistanceLimit = exeData.attr[4];
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			break;			 
		case 7:	
			_targetDistanceLimit = exeData.attr[5];
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			break;			 
		case 8: 
			_targetDistanceLimit = exeData.attr[3];		 
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			break;
		case 9: 
			_targetDistanceLimit = exeData.attr[4];
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			else if(targeting == TargetingData.FORWARD_LINEAR_3)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType3; 					
				playerTargetingChecker =  canPlayerHeroTargetingType3;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType3PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType3PVP2;
			}
			
			break;
		case 10: 
			_targetDistanceLimit = exeData.attr[5];
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			break;
		case 11: 
			_targetDistanceLimit = exeData.attr[6];
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
				playerTargetingChecker =  canPlayerHeroTargetingType2;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}
			break;
		case 15: 
			_targetDistanceLimit = exeData.attr[1]; // 전체 연결거리를 검사...
			heroMonsterTargetingChecker = canMonsterHeroTargetingType2; 
			playerTargetingChecker =  canPlayerHeroTargetingType2;
			playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
			playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			break;			
		case 17: 
			heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
			playerTargetingChecker =  canPlayerHeroTargetingType1;
			playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
			playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			break;			
		default:
			
			if(targeting == TargetingData.FIXED_1)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType1; 
				playerTargetingChecker =  canPlayerHeroTargetingType1;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType1PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType1PVP2;
			}
			else if(targeting == TargetingData.AUTOMATIC_2)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType2WithOutDistCheck; 
				playerTargetingChecker =  canPlayerHeroTargetingType2WithOutDistCheck;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType2PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType2PVP2;
			}	
			else if(targeting == TargetingData.FORWARD_LINEAR_3)
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingType3; 					
				playerTargetingChecker =  canPlayerHeroTargetingType3;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTargetingType3PVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTargetingType3PVP2;
			}
			else
			{
				heroMonsterTargetingChecker = canMonsterHeroTargetingTypeReturnTrue;
				playerTargetingChecker =  canPlayerHeroTypeReturnTrue;
				playerHeroTargetingPVPModeChecker = canPlayerHeroTypeReturnTruePVP;
				playerHeroTargetingPVPModeMoveForwardChecker = canPlayerHeroTypeReturnTruePVP2;
			}
			break;
		}		

	}


	
	
	
	
	
	
	
	
	
	
///// ============================================================== //////
///// 타게팅 검사.	
///// ============================================================== //////	
	
	Monster _target;
	public delegate bool CheckTargeting(Monster attacker, int targetIndex = -1);
	public CheckTargeting heroMonsterTargetingChecker;
	
	
	public delegate bool CheckPlayerHeroTargeting(Monster p, bool checkTargetOnly, TargetingDecal decal);
	public CheckPlayerHeroTargeting playerTargetingChecker;


	public delegate bool CheckPlayerHeroTargetingPVPMode(Player target, Vector3 targetPosition);
	public CheckPlayerHeroTargetingPVPMode playerHeroTargetingPVPModeChecker;
	public CheckPlayerHeroTargetingPVPMode playerHeroTargetingPVPModeMoveForwardChecker;

	
	public delegate bool CheckUnitMonsterTargeting(Monster attacker);
	public CheckUnitMonsterTargeting unitMonsterTargetingChecker;
	
	
	// 무조건 OK 시키기위함.
	bool ctReturnTrue(Monster attacker) { return true; }
	
	// 타게팅 1번 고정 거리일 경우 데칼 좌우측 영역을 미리 계산해둔다...
	IFloat _fixedLineLeft = 0.0f;
	IFloat _fixedLineRight = 0.0f;
	
	
	
	//============= 히어로 몬스터 타게팅 검사 로직  =======================

	public bool canPlayerHeroTypeReturnTrue(Monster player, bool checkTargetOnly, TargetingDecal decal)
	{
		return true;
	}	

	public bool canPlayerHeroTypeReturnTrue()
	{
		return true;
	}	


	public bool canMonsterHeroTargetingTypeReturnTrue(Monster attacker, int targetIndex = -1)
	{
		attacker.skillTargetChecker = ctReturnTrue;
		return true;
	}
	
	// 2번 자동선택 타겟팅 중 6~11번은 최대 타겟 거리를 계산해야한다.
	// 6~11번을 제외한 녀석들은 자동타겟이면 무조건 되는거다.
	bool ctAutoByTargetDistance(Monster attacker)
	{
		return (VectorUtil.DistanceXZ(attacker.skillTarget.cTransformPosition, attacker.cTransformPosition) <= _targetDistanceLimit);
	}	

	// 고정영역이면 현재 위치에서 스킬을 쓸 수 있는지만 검사하면 됨.
	bool ctFixed(Monster attacker)
	{
		return ( attacker.skillTarget.cTransformPosition.x > attacker.cTransformPosition.x + _fixedLineLeft && attacker.skillTarget.cTransformPosition.x < attacker.cTransformPosition.x + _fixedLineRight);
	}
	
	
	bool _returnMoveValue;
	bool ctMoveFixed(Monster attacker)
	{
		_returnMoveValue = true;
		
		// 시전자가 타겟보다 우측에 있으면 좌측으로 이동시킨다.
		if(attacker.cTransformPosition.x > attacker.skillTarget.cTransformPosition.x)
		{
			if(attacker.lineLeft - attacker.stat.speed * GameManager.globalDeltaTime > GameManager.me.characterManager.playerMonsterRightLine)
			{
				_v = attacker.cTransformPosition;
				_v.x -= attacker.stat.speed * GameManager.globalDeltaTime;
				attacker.setPosition(_v);

				attacker.setPlayAniRightNow(Monster.WALK);
				_returnMoveValue = true;
			}
			else
			{
				attacker.setPlayAniRightNow(Monster.NORMAL);

				_returnMoveValue = false;
			}
		}
		else // 시전자가 타겟보다 좌측에 있으면 우측으로 이동시킨다. 
		{
			_v = attacker.cTransformPosition;
			_v.x += attacker.stat.speed * GameManager.globalDeltaTime;	
			//Log.log("#632");
			attacker.state = Monster.WALK;
			attacker.setPosition(_v);
			_returnMoveValue = true;
		}
		
		// 단순히 앞쪽을 바라보게 하기위함이다.
		_v.x -= 20.0f; 
		_v2 = attacker.cTransformPosition;
		attacker.tf.rotation = Util.getFixedQuaternionSlerp(attacker.tf.rotation, Util.getLookRotationQuaternion(_v - _v2), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);	

		return _returnMoveValue;
	}
	
	bool ctMoveAuto(Monster attacker)
	{
		_returnMoveValue = false;
		// 타겟이 타격 반경보다 왼쪽에 있을때. => 내가 좌측으로 이동한다.
		if( attacker.skillTarget.cTransformPosition.x < attacker.cTransformPosition.x + _fixedLineLeft)
		{
			if(attacker.lineLeft - attacker.stat.speed * GameManager.globalDeltaTime > GameManager.me.characterManager.playerMonsterRightLine)
			{
				_v = attacker.cTransformPosition;
				_v.x -= attacker.stat.speed * GameManager.globalDeltaTime;
				attacker.setPosition(_v);
				attacker.setPlayAniRightNow(Monster.WALK);
				_returnMoveValue = true;
			}
			else
			{
				attacker.setPlayAniRightNow(Monster.NORMAL);
				_returnMoveValue = false;
			}
		}
		// 타겟이 타격 반경보다 오른쪽에 있을때. => 내가 우측으로 이동한다. 단 우측 끝까지밖에 못간다.
		else if(attacker.skillTarget.cTransformPosition.x < attacker.cTransformPosition.x + _fixedLineRight)
		{
			if(attacker.lineRight + attacker.stat.speed * GameManager.globalDeltaTime >= StageManager.mapEndPosX)
			{
				attacker.setPlayAniRightNow(Monster.NORMAL);
				_returnMoveValue = false;
			}
			else
			{
				_v = attacker.cTransformPosition;
				_v.x += attacker.stat.speed * GameManager.globalDeltaTime;				
				attacker.setPlayAniRightNow(Monster.WALK);
				attacker.setPosition(_v);
				_returnMoveValue = true;
			}
		}
		
		// 단순히 앞쪽을 바라보게 하기위함이다.
		_v.x -= 20.0f; 


		_v2 = attacker.cTransformPosition;
		attacker.tf.rotation = Util.getFixedQuaternionSlerp(attacker.tf.rotation, Util.getLookRotationQuaternion(_v - _v2), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
		
		return _returnMoveValue;
	}	
	
	
	public bool canMonsterHeroTargetingType1(Monster attacker, int targetIndex = -1)
	{
		// 타겟팅 1,2번 
		_target = null;
		
		//<1 : 히어로 전방 A거리(cm), B지름> 
		//  - 바닥에 뭔가가 떨어지거나 생성되는 스킬에 적용
		//  - 타게팅(차징)중 히어로 전방 A거리에 B지름 형태의 (원형 마법진모양의) 타게팅 데칼을 표시함
		//  - 설정된 지름의 크기에 따라서 원형태의 마법진 이미지의 크기를 변경해서 표시함
		//  - B지름이 400을 넘어가면 타원으로 (일단;) 늘려서 표시
		_fixedLineLeft = -(float)targetAttr[0] - ((float)targetAttr[1]) * 0.5f;
		_fixedLineRight = -(float)targetAttr[0] + ((float)targetAttr[1]) * 0.5f;
		
		if(targetType == Skill.TargetType.ENEMY) 
		{
			// 플레이어 몬스터에서 고른다. (적의 적군)
			_target = GameManager.me.characterManager.getCloseEnemyTargetByIndexForHeroMonster(attacker.isPlayerSide, targetIndex);
		}
		else 
		{
			// 적 몬스터에서 고른다. (적의 아군)
			_target = GameManager.me.characterManager.getCloseTeamTarget(attacker.isPlayerSide, attacker);
		}
		
		if(_target == null) return false;

		_v = attacker.cTransformPosition;
		
		if( Xfloat.greaterThan(  _target.cTransformPosition.x , GameManager.me.characterManager.targetZonePlayerLine + _fixedLineLeft )
		   &&  Xfloat.lessThan( _target.cTransformPosition.x , StageManager.mapEndPosX.Get() + _fixedLineRight ) )
		{
			attacker.setSkillTarget( _target );
			attacker.skillTargetChecker = ctFixed;
			attacker.skillMove = ctMoveFixed;
			_target = null;
			return true;				
		}
		
		_target = null;
		return false;
	}
	
	// 리미트 거리가 있는 것.
	public bool canMonsterHeroTargetingType2(Monster attacker, int targetIndex = -1)
	{
		//데미지 범위. exeData.attr[1];
		_target = TargetingData.getAutomaticTarget(attacker, targetType, targetAttr, canUseThisSkillOnThisType);
		if(_target == null) return false;

		// 타겟이 같은 편일때는 그냥 움직여서 쏘면 되니까 상관 없음.
		// 근데 타겟이 다른 편일때는 전방 거리까지 움직여서 쏠 수 있는지 확인을 해야할 것.
		if(_target.isPlayerSide) // 적히어로몬스터가 우리편에게 쏘는거면...
		{
			if(GameManager.me.characterManager.targetZonePlayerLine - _target.cTransformPosition.x > _targetDistanceLimit)
			{
				_target = null;
				return false;
			}
		}
		
		attacker.setSkillTarget( _target );
		attacker.skillTargetChecker = ctAutoByTargetDistance;
		attacker.skillMove = ctMoveAuto;
		_target = null;				
		return true;
	}		
	
	// 리미트 거리가 없는 것.
	public bool canMonsterHeroTargetingType2WithOutDistCheck(Monster attacker, int targetIndex = -1)
	{
		_target = TargetingData.getAutomaticTarget(attacker, targetType, targetAttr, canUseThisSkillOnThisType);
		if(_target == null) return false;
		attacker.setSkillTarget( _target );
		attacker.skillTargetChecker = ctReturnTrue;
		_target = null;				
		return true;
	}		
	
	public bool canMonsterHeroTargetingType3(Monster attacker, int targetIndex = -1)
	{		
		attacker.skillTargetChecker = ctReturnTrue;
		return true;		
	}		
	
	
	
//============= 플레이어 타게팅 검사 로직  =======================
	
	public bool canPlayerHeroTargetingType1(Monster p, bool checkTargetOnly, TargetingDecal decal)
	{
		// 플레이어 1번의 경우.....
		_v = p.cTransformPosition;
		_v.x += ((p.isPlayerSide)?targetAttr[0].Get():-targetAttr[0].Get());

		if(checkTargetOnly == false)
		{
			p.targetPosition = _v;	
		}

		_v.y = 1.0f;
		decal.setPosition(_v);	

		return true;
	}
	
	
	// 리미트 거리가 있는 것.
	public bool canPlayerHeroTargetingType2(Monster p, bool checkTargetOnly, TargetingDecal decal)
	{
		//데미지 범위. exeData.attr[1];
		_target = TargetingData.getAutomaticTarget(p, targetType, targetAttr, canUseThisSkillOnThisType);

		if(_target == null) return false;
		//단순히 현재 상태에서 검사!
		if(VectorUtil.DistanceXZ( _target.cTransformPosition, p.cTransformPosition) < _targetDistanceLimit)
		{
			if(checkTargetOnly == false)
			{
				p.setSkillTarget( _target );
				p.targetHeight = _target.hitObject.height;
				p.targetPosition = _target.cTransformPosition;		
				p.targetUniqueId = _target.stat.uniqueId;
			}				

			_v = _target.cTransformPosition;
			_v.y = 1.0f;

			decal.setPosition(_v);

			_target = null;
			return true;
		}
		
		_target = null;
		return false;
	}		
	
	// 리미트 거리가 없는 것.
	public bool canPlayerHeroTargetingType2WithOutDistCheck(Monster p, bool checkTargetOnly, TargetingDecal decal)
	{
		_target = TargetingData.getAutomaticTarget(p, targetType, targetAttr, canUseThisSkillOnThisType);
		if(_target == null) return false;


		if(checkTargetOnly == false)
		{
			p.setSkillTarget( _target );
			p.targetHeight = _target.hitObject.height;
			p.targetPosition = _target.cTransformPosition;		
			p.targetUniqueId = _target.stat.uniqueId;

		}

		//if(p.isPlayer) 
		{
			_v = _target.cTransformPosition;
			_v.y = 1.0f;

			decal.setPosition(_v);
		}

		_target = null;				
		return true;
	}		
	
	public bool canPlayerHeroTargetingType3(Monster p, bool checkTargetOnly, TargetingDecal decal)
	{	
		_v = p.cTransformPosition;
		_v.x += (p.fowardDirectionValue);

		if(p.hasShootingPos)
		{
			_v.y = p.shootingPos[1];
			_v.y *= p.monsterData.scale;
		}
		else 
		{
			_v.y += p.hitObject.height * 0.7f;
		}

		if(checkTargetOnly == false)
		{
			p.targetHeight = p.hitObject.height;
			p.targetPosition = _v;
			p.targetUniqueId = -1;
			p.setSkillTarget(  null );
		}

		if(decal != null)
		{
			_v = p.cTransformPosition;
			_v.y = 1.0f;

			decal.setPosition(_v);
		}

		return true;
	}	



	
}







