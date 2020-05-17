using System;
using System.Collections.Generic;
using UnityEngine;

sealed public partial class UnitSkillData : BaseSkillData
{
	
	bool _tempB;	
	
	public UnitSkillData ()
	{
	}

	const string PASSIVE_SKILL_OFF = "PS_E_OFF";

	public bool usePassiveSkillEffect = true;

	public string resourceId = null;

	public float activeSkillCooltime = -1.0f;
	public float activeSkillDuration = 0.0f;
	public string activeSkillEffect = null;

	public Condition conAct = Condition.NONE;
	
	public int[] conHpMinMax;
	
	public int exeChance;
	
	
	public float conCooltime;
	public int conEnemy;
	
	public int conEnemyInDistance;
	
	public int conMaxCount;
	
	public enum Condition
	{
		Dead, Attack, Damage, AfterAttack, NONE
	}

	public bool hasSkillAni = false;

	public string activeSkillCamId = null;

	
	sealed public override void setData(List<object> l, Dictionary<string, int> k)
	{
		base.setData(l,k);

		skillDataType = SkillDataType.Unit;

		Util.parseObject(l[k["A_SKILL_CT"]],out activeSkillCooltime, true, -1.0f);
		Util.parseObject(l[k["A_SKILL_DURATION"]],out activeSkillDuration, true, -1.0f);
		activeSkillEffect = l[k["A_SKILL_EFFECT"]].ToString();

		if(string.IsNullOrEmpty( activeSkillEffect ) == false )
		{

			if( activeSkillEffect == PASSIVE_SKILL_OFF )
			{
				activeSkillEffect = string.Empty;
				usePassiveSkillEffect = false;
			}
		}

		isPassiveSkill = (activeSkillCooltime < 0);

#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && activeSkillCooltime > -1)
		{
			activeSkillCooltime = 0.1f;
		}
#endif

		resourceId = l[k["RESOURCE"]].ToString();
		if(string.IsNullOrEmpty(resourceId)) resourceId = null;

		tempCheckers.Clear();

		bool ignoreSkillAni = false;

		switch((string)l[k["CON_ACT"]])
		{
		case "DEAD":
			conAct = Condition.Dead;
			tempCheckers.Add(checkCharacterStateDead);
			break;
		case "ATTACK":
			conAct = Condition.Attack;
			tempCheckers.Add(checkCharacterStateAttack);
			break;
		case "DAMAGE":
			conAct = Condition.Damage;
			tempCheckers.Add(checkCharacterStateDamage);
			break;			
		case "ATTACKED":
			conAct = Condition.AfterAttack;
			tempCheckers.Add(checkCharacterStateAfterAttack);
			break;		
		case "NOANI":
			ignoreSkillAni = true;
			break;
		}

		parser(l[k["CON_HPMIN"]], out conHpMinMax, checkHpMinMax);

		if(l[k["CON_COOLTIME"]] is string)
		{
			_tstr = (string)l[k["CON_COOLTIME"]];
			if(string.IsNullOrEmpty(_tstr))
			{
				hasCoolTime = false;
			}
			else if(_tstr.Contains(","))
			{
				hasCoolTime = true;
				string[] tsr = _tstr.Split(',');
				Util.parseObject(tsr[1], out coolTimeStartDelay, true, 0.0f);
				parser(tsr[0], out conCooltime, checkCooltime);
				coolTime = conCooltime;
			}
		}
		else
		{
			hasCoolTime = true;
			parser(l[k["CON_COOLTIME"]], out conCooltime, checkCooltime);	
			coolTime = conCooltime;
			
		}



		parser(l[k["CON_ENEMY"]], out conEnemy, checkAttackerNum);
		
		parser(l[k["CON_ENEMY_DIST"]], out conEnemyInDistance, checkEnermyInDistance);
		
		parser(l[k["CON_MAX_COUNT"]], out conMaxCount, checkMaxCount);
		
		parser(l[k["EXE_CHANCE"]], out exeChance, checkExeChance);
		
		
		if(exeType > 0)
		{
			if(conAct == Condition.NONE)
			{
				if(ignoreSkillAni == false) hasSkillAni = true;
				
				if(skillType == Skill.Type.ATTACK)
				{
					tempCheckers.Add(checkReplaceNormalAttackWithSkillAttack);
				}
			}
			else if(conAct == Condition.AfterAttack)
			{
				hasSkillAni = true;
			}
		}

		if(activeSkillCooltime > -1)
		{
			tempCheckers.Add(checkActiveSkill);
		}

		checkers = tempCheckers.ToArray();
		tempCheckers.Clear();
		_checkerNum = checkers.Length;


		activeSkillCamId = (string)l[k["CAMID"]];

		if(string.IsNullOrEmpty(activeSkillCamId))
		{
			activeSkillCamId = null;
		}

	}

	public bool canUseOneShotUnitSkill = false;

	bool checkActiveSkill(Monster mon, SkillSlot ss)
	{
		if(mon.isPlayerSide && mon.monsterUISlotIndex > -1)
		{
			if(activeSkillDuration > 0)
			{
				return UIPlay.getUnitSlot(mon.monsterUISlotIndex).checkActiveSkillDuration();
			}
			else
			{
				return UIPlay.getUnitSlot(mon.monsterUISlotIndex).unitSlot.canUseOneShotUnitSkill;
			}
		}
		else if(mon.aiUnitSlot != null)
		{
			if(activeSkillDuration > 0)
			{
				return (mon.aiUnitSlot.checkActiveSkillDuration());
			}
			else
			{
				return (mon.aiUnitSlot.canUseOneShotUnitSkill);
			}			
		}

		return false;
	}

	
	bool checkReplaceNormalAttackWithSkillAttack(Monster mon, SkillSlot ss)
	{
		return (mon.action.state == MonsterUnitAction.STATE_ACTION && mon.action.canUsePassiveSkill);
	}
	
	
	// 죽을때 발동...
	bool checkCharacterStateDead(Monster mon, SkillSlot ss)
	{
		return (mon.state == Monster.DEAD); //mon.hp <= 0 || 
	}	
	
	
	bool checkCharacterStateAttack(Monster mon, SkillSlot ss)
	{

		return (mon.receiveDamageMonstersByMe.Count > 0 && mon.action.canUsePassiveSkill);//(mon.action.isDamageFrame == false && mon.state.Contains(Monster.SHOOT_HEADER));
	}	

	bool checkCharacterStateAfterAttack(Monster mon, SkillSlot ss)
	{
		return (mon.action.isAfterAttackFrame && mon.action.canUsePassiveSkill);
	}

	// 데미지를 받을 때 발동하는 녀석은 죽을때는 발동하지 않는다.
	bool checkCharacterStateDamage(Monster mon, SkillSlot ss)
	{
		return (mon.action.isDamageFrame && mon.action.canUsePassiveSkill);
	}	
	
	bool checkHpMinMax(Monster mon, SkillSlot ss)
	{
		return ( ( (mon.hpPer) * 100.0f > conHpMinMax[0])  &&  ( (mon.hpPer)  * 100.0f <= conHpMinMax[1]));
	}		
	
	
	bool checkCooltime(Monster mon, SkillSlot ss)
	{
		return ss.checkCooltime();
	}
	
	bool checkAttackerNum(Monster mon, SkillSlot ss)
	{
		return mon.attackers.Count >= conEnemy;
	}
	
	bool checkEnermyInDistance(Monster mon, SkillSlot ss)
	{
		if(mon.isPlayerSide)
		{
			return (mon.cTransformPosition.x + conEnemyInDistance >= GameManager.me.characterManager.monsterLeftLine);
		}
		else
		{
			return (mon.cTransformPosition.x - conEnemyInDistance <= GameManager.me.characterManager.playerMonsterRightLine);
		}
	}
	
	bool checkMaxCount(Monster mon, SkillSlot ss)
	{
		return (ss.exeCount < conMaxCount);
	}
	
	
	bool checkExeChance(Monster mon, SkillSlot ss)
	{
		return (exeChance > GameManager.inGameRandom.Range(0,100));//GameManager.getRandomNum());	
	}
	
	
	
	string _tstr;
	private void parser(object obj, out int[] arr, Checker callback)
	{
		_tstr = (string)obj;
		if( string.IsNullOrEmpty(_tstr))
		{
			arr = null;
			return;
		}
		string[] ta = _tstr.Split(',');
		arr = new int[ta.Length];
		int.TryParse(ta[0], out arr[0]);
		int.TryParse(ta[1], out arr[1]);
		tempCheckers.Add(callback);
	}
	
	private void parser(object obj, out int intVar, Checker callback)
	{
		if(obj is int)
		{
			intVar = (int)obj;
			tempCheckers.Add(callback);
		}
		else if(obj is  long)
		{
			intVar = (int)(long)obj;
			tempCheckers.Add(callback);
		}
		else intVar = -1;
	}		
	
	
	private void parser(object obj, out float intVar, Checker callback)
	{
		if(obj is float)
		{
			intVar = (float)obj;
			tempCheckers.Add(callback);
		}
		else if(obj is double)
		{
			intVar = (float)(double)obj;
			tempCheckers.Add(callback);
		}
		else if(obj is long)
		{
			intVar = (float)(long)obj;
			tempCheckers.Add(callback);			
		}
		else if(obj is int)
		{
			intVar = (float)(int)obj;
			tempCheckers.Add(callback);			
		}
		
		else intVar = 0.0f;
	}		
	
	

	delegate bool Checker(Monster mon, SkillSlot ss);
	
	List<Checker> tempCheckers = new List<Checker>();
	Checker[] checkers;
	int _checkerNum = 0;	
	
	public bool check(Monster mon,  SkillSlot ss)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log(mon.resourceId + " unitskilldata check : " + _checkerNum);
		}
		#endif

		for(int i = 0; i < _checkerNum; ++i)
		{
			if(checkers[i](mon, ss) == false)
			{
				return false;
			}
		}
		return true;
	}	
	
	
	
	
	
	
	// =========== 타겟팅 ============ //
	
/*
0 : 본인
1 : 자신을 공격한 근접 (공격타입1,2) 유닛
2 : 자신이 공격한 상대 (적 유닛 or 히어로)
   - 범위공격의 경우, 그 중 1마리 랜덤선택
3 : 자신과 가장 가까운 적
4 : 자신과 가장 가까운 아군 유닛
5 : 전방 N거리(속성1) 지점
6 : HP가 가장 낮은 아군 유닛
7 : 정면 방향		
*/	
	
	public override void setTargetingChecker (List<object> l, Dictionary<string, int> k)
	{
		
		switch(targeting)
		{
		case 0:
			unitMonsterTargetingChecker = unitTargeting0;
			break;
		case 1:
			unitMonsterTargetingChecker = unitTargeting1;
			break;
		case 2:
			unitMonsterTargetingChecker = unitTargeting2;
			break;
		case 3:
			unitMonsterTargetingChecker = unitTargeting3;
			break;
		case 4:
			unitMonsterTargetingChecker = unitTargeting4;
			break;
		case 5:
			targetAttr = new Xint[2];
			Util.parseObject(l[k["T_ATTR1"]], out targetAttr[0], true, 0);
			Util.parseObject(l[k["T_ATTR2"]], out targetAttr[1], true, 0);
			unitMonsterTargetingChecker = unitTargeting5;
			break;
		case 6:
			unitMonsterTargetingChecker = unitTargeting6;
			break;
		case 7:
			unitMonsterTargetingChecker = unitTargeting7;
			break;
		}
	}
	
	public bool unitTargeting0(Monster attacker)
	{
		// 0 : 본인
		attacker.setSkillTarget( attacker );
		attacker.targetPosition = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = attacker.stat.uniqueId;

		return true;
	}
	
	public bool unitTargeting1(Monster attacker)
	{
		//1 : 자신을 공격한 근접 (공격타입1,2) 유닛
		if(attacker.attacker != null && attacker.attacker.stat.uniqueId == attacker.attackerUniqueId && attacker.attacker.hp > 0 && attacker.attacker.stat.monsterType == Monster.TYPE.UNIT)
		{
			attacker.setSkillTarget( attacker.attacker );
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}
		
		return false;
	}
	

	static List<int> _targetIndexSorter = new List<int>();
	public bool unitTargeting2(Monster attacker)
	{
		_targetIndexSorter.Clear();

		for(int i = attacker.receiveDamageMonstersByMe.Count - 1; i >= 0; --i)
		{
			if(attacker.receiveDamageMonstersByMe[i].isSameMonster()) _targetIndexSorter.Add(i);
		}

		if(_targetIndexSorter.Count > 0)
		{
			attacker.setSkillTarget( attacker.receiveDamageMonstersByMe[_targetIndexSorter[GameManager.inGameRandom.Range(0,_targetIndexSorter.Count)]] );
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			attacker.receiveDamageMonstersByMe.Clear();
			_targetIndexSorter.Clear();
			return true;
		}
		attacker.receiveDamageMonstersByMe.Clear();
		_targetIndexSorter.Clear();

		//2 : 자신이 공격한 상대 (적 유닛 or 히어로)
		//   - 범위공격의 경우, 그 중 1마리 랜덤선택	
		return false;
	}
	
	public bool unitTargeting3(Monster attacker)
	{
		//3 : 자신과 가장 가까운 적
		attacker.setSkillTarget( GameManager.me.characterManager.getCloseEnemyTarget(attacker.isPlayerSide, attacker) );
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}	
		
		return false;
	}
	
	public bool unitTargeting4(Monster attacker)
	{
		//4 : 자신과 가장 가까운 아군 유닛
		attacker.setSkillTarget( GameManager.me.characterManager.getCloseTeamTarget(attacker.isPlayerSide, attacker, monsterTypeChecker) );
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}	
		
		return false;
	}
	
	public bool unitTargeting5(Monster attacker)
	{
		//5 : 전방 N거리(속성1) 지점
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;

		if(attacker.isPlayerSide)
		{
			_v.x += targetAttr[0].Get(); 
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= targetAttr[0].Get();
			attacker.targetPosition = _v;
		}
		
		return true;
	}	
	
	public bool unitTargeting6(Monster attacker)
	{
		//6 : HP가 가장 낮은 아군 유닛
		attacker.setSkillTarget(GameManager.me.characterManager.getLowestHPTeamTarget(attacker.isPlayerSide, attacker, monsterTypeChecker));
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}
		
		return false;
	}

	
	public bool unitTargeting7(Monster attacker)
	{
		//7 : 정면 방향
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += 1000.0f;
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= 1000.0f;
			attacker.targetPosition = _v;
		}
		
		return true;
	}	





	public bool monsterTypeChecker(Monster target)
	{
		return (target.stat.monsterType == Monster.TYPE.UNIT);
	}


	public void doActiveSkill(Monster mon, bool isAiMode = false)
	{
		if(mon.isEnabled == false || mon.hp <= 0) return;

		if(isAiMode == false && unitMonsterTargetingChecker(mon) == false)
		{
			return;
		}

		mon.clearOnAttackAnimationMethod();
		mon.onCompleteAttackAni(true);
		mon.renderAniRightNow();

		doActiveSkillRightNow(mon);



	}


	public void doActiveSkillRightNow(Monster mon)
	{
//		#if UNITY_EDITOR
//		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
//		{
//			GameManager.me.cutSceneManager.startUnitSkillCamScene(UnitSkillCamMaker.instance.nowId, mon.cTransform.position, false);
//			return;
//		}
//		#endif
		
		if(mon.isPlayerSide)
		{
			ParticleEffect.SKILL_EFFECT_SHOOTER_ID = mon.stat.uniqueId;
			GameManager.me.cutSceneManager.startUnitSkillCamScene(UIPlayUnitSlot.nowReadySkillCamId, mon.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill);
		}


		if(hasSkillAni)
		{
			mon.action.canUsePassiveSkill = true;
			mon.action.setAttackDelay(2.0f);
			mon.action.nowUnitSkillData = this;
			mon.action.freezeTime = 0.2f;
			mon.action.state = MonsterUnitAction.STATE_WAIT;
			mon.state = Monster.ATK_IDS[exeType];
			mon.renderAniRightNow();
		}
		else
		{
			mon.action.setAttackDelay(2.0f);
			mon.nowBulletPatternId = (resourceId == null)?id:resourceId;
			exeData.monsterShoot(mon, 1);
		}
	}



	public string getBulletPatternId()
	{
		return (resourceId == null)?id:resourceId;
	}


}



