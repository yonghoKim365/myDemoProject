using System;
using UnityEngine;

sealed public partial class SkillEffectData
{
	public SkillEffectData clone(HeroSkillData inputSkillData)
	{
		SkillEffectData sd = new SkillEffectData();

		sd.startDelay = this.startDelay;

		sd.attr = new Xfloat[this.attr.Length][];

		for(int i = 0; i < sd.attr.Length; ++i)
		{
			if(this.attr[i] == null)
			{
				sd.attr[i] = null;
			}
			else
			{
				sd.attr[i] = new Xfloat[this.attr[i].Length];
				Array.Copy(this.attr[i], sd.attr[i], this.attr[i].Length);
			}
		}

		sd.valueType = new GameValueType.Type[this.valueType.Length];
		Array.Copy(this.valueType, sd.valueType, this.valueType.Length);

		sd.statValueType = new GameValueType.ApplyStatValue[this.statValueType.Length];
		Array.Copy(this.statValueType, sd.statValueType, this.statValueType.Length);

		sd.type = this.type;

		sd.levelCorrection = this.levelCorrection;
		sd.isDurationType = this.isDurationType;

		sd.isFixedType = this.isFixedType;

		sd.preSkillDamageCalc = null;
		sd.setApplySkillType(sd.type, sd);

		sd.skillData = inputSkillData;

		return sd;
	}


	public Xfloat startDelay = -1;

	public Xfloat[][] attr;

	public GameValueType.Type[] valueType;
	public GameValueType.ApplyStatValue[] statValueType;


	public int type;
	
	public bool levelCorrection = false;
	public bool isDurationType = false;
	

	public bool isFixedType = false;

	public BaseSkillData skillData;
	
	
	public SkillEffectData ()
	{
	}


	public static SkillEffectData getSkillEffectData(int type,  params object[] args)
	{
		SkillEffectData data = new SkillEffectData();
		data.type = type;
		
		int len = 0;
		
		if(type > 0)
		{
			len = GameManager.info.skillEffectSetupData[type].len;	
		}


		data.attr = new Xfloat[len][];
		data.valueType = new GameValueType.Type[len];
		data.statValueType = new GameValueType.ApplyStatValue[len];

		for(int i = 0; i < len; ++i)
		{
#if UNITY_EDITOR
			try
			{
				object ck = args[i];
			}
			catch
			{
				Debug.Log("err..");
			}
#endif

			string argStr = args[i].ToString();
//			if(argStr.Contains("/"))
//			{
			//				data.attr[i] = Util.stringToFloatArray(argStr,'/');
//				data.valueType[i] = GameValueType.Type.Section;
//			}
			//else
			if(argStr.Contains(","))
			{
				data.valueType[i] = GameValueType.Type.Fixed;

				if(argStr.Contains("_"))
				{
					string[] ts0 = argStr.Split(',');

					data.attr[i] = new Xfloat[2];

					for(int j = 0; j < 2; ++j)
					{
						string[] ts1 = ts0[j].Split('_');


						Util.tryFloatParse(ts1[0], out data.attr[i][j]);

						switch(ts1[1])
						{
						case "AP":
							data.statValueType[i] = GameValueType.ApplyStatValue.AP;
							break;
						case "DP":
							data.statValueType[i] = GameValueType.ApplyStatValue.DP;
							break;
						case "HP":
							data.statValueType[i] = GameValueType.ApplyStatValue.HP;
							break;
						case "LV":
							data.statValueType[i] = GameValueType.ApplyStatValue.LV;
							break;
						default:
							data.statValueType[i] = GameValueType.ApplyStatValue.None;
							break;
						}
					}
				}
				else
				{
					data.attr[i] = Util.stringToXFloatArray(argStr,',');
					data.statValueType[i] = GameValueType.ApplyStatValue.None;
				}
			}
			else
			{
				data.valueType[i] = GameValueType.Type.Single;

				if(argStr.Contains("_"))
				{
					string[] ts2 = argStr.Split('_');
					data.attr[i] = new Xfloat[1]{0};
					Util.tryFloatParse(ts2[0], out data.attr[i][0]);

					switch(ts2[1])
					{
					case "AP":
						data.statValueType[i] = GameValueType.ApplyStatValue.AP;
						break;
					case "DP":
						data.statValueType[i] = GameValueType.ApplyStatValue.DP;
						break;
					case "HP":
						data.statValueType[i] = GameValueType.ApplyStatValue.HP;
						break;
					case "LV":
						data.statValueType[i] = GameValueType.ApplyStatValue.LV;
						break;
					default:
						data.statValueType[i] = GameValueType.ApplyStatValue.None;
						break;
					}
				}
				else
				{
					data.attr[i] = Util.stringToXFloatArray(argStr,'/');
					data.statValueType[i] = GameValueType.ApplyStatValue.None;
				}
			}
		}	

#if UNITY_EDITOR
//		Debug.Log(type);
#endif
		data.levelCorrection = GameManager.info.skillEffectSetupData[type].useLevelCorrection;

		data.preSkillDamageCalc = null;

		data.isDurationType = false;

		data = data.setApplySkillType(type, data);

		return data;
	}
	


	public SkillEffectData setApplySkillType(int inputType, SkillEffectData data)
	{
		switch(inputType)
		{
		case 1: data.applySkillEffect = data.type1; 
			data.preSkillDamageCalc = data.physicAtkDamagePreCalc;
			break;
		case 2: data.applySkillEffect = data.type2; 
			data.preSkillDamageCalc = data.physicAtkDamagePreCalc;
			break;
		case 3: data.applySkillEffect = data.type3; 
			data.preSkillDamageCalc = data.physicAtkDamagePreCalc;
			break;
		case 4: data.applySkillEffect = data.type4; 
			data.preSkillDamageCalc = data.physicAtkDamagePreCalc;
			break;
		case 5: data.applySkillEffect = data.type5; 
			data.preSkillDamageCalc = data.magicAtkDamagePreCalc;
			break;
		case 6: data.applySkillEffect = data.type6; 
			data.preSkillDamageCalc = data.magicAtkDamagePreCalc;
			break;
		case 7: data.applySkillEffect = data.type7; 
			data.preSkillDamageCalc = data.magicAtkDamagePreCalc;
			break;
		case 8: data.applySkillEffect = data.type8; break;
		case 9: data.applySkillEffect = data.type9; break;
		case 10: data.applySkillEffect = data.type10; data.isDurationType = true;break;
		case 11: data.applySkillEffect = data.type11; data.isDurationType = true;break;
		case 12: data.applySkillEffect = data.type12; data.isDurationType = true;break;
		case 13: data.applySkillEffect = data.type13; data.isDurationType = true;break;
		case 14: data.applySkillEffect = data.type14; data.isDurationType = true;break;
		case 15: data.applySkillEffect = data.type15; data.isDurationType = true;break;
		case 16: data.applySkillEffect = data.type16; data.isDurationType = true;break;
		case 17: data.applySkillEffect = data.type17; data.isDurationType = true;break;
		case 18: data.applySkillEffect = data.type18; data.isDurationType = true;break;
		case 19: data.applySkillEffect = data.type19; data.isDurationType = true;break;
		case 20: data.applySkillEffect = data.type20; data.isDurationType = true;break;
		case 21: data.applySkillEffect = data.type21; data.isDurationType = true;break;
		case 22: data.applySkillEffect = data.type22; data.isDurationType = true;break;
		case 23: data.applySkillEffect = data.type23; data.isDurationType = true;break;
		case 24: data.applySkillEffect = data.type24; data.isDurationType = true;break;
		case 25: data.applySkillEffect = data.type25; break;
			
		case 28: data.applySkillEffect = data.type28; break;
		case 29: data.applySkillEffect = data.type29; data.isDurationType = false;break;
		case 30: data.applySkillEffect = data.type30; data.isDurationType = true;break;
		case 31: data.applySkillEffect = data.type31; data.isDurationType = true;break;
		case 32: data.applySkillEffect = data.type32; break;
		case 33: data.applySkillEffect = data.type33; 
			data.preSkillDamageCalc = data.magicAtkDamagePreCalc;
			break;			
		case 34: data.applySkillEffect = data.type34; data.isDurationType = true;break;
		case 35: data.applySkillEffect = data.type35; data.isDurationType = true;break;
		case 36: data.applySkillEffect = data.type36; data.isDurationType = true;break;
		}

		return data;

	}




//===  Skill Effect 적용 부분 ============================================///
	
	IFloat _tempF;
	IFloat _tempF2;
	
	public delegate bool ApplySkillEffect(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f);
	public ApplySkillEffect applySkillEffect;

	bool type1(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 1) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		if(bullet == null)
		{
			_tempF = getApplyValue(0,applyReinforceLevel,attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type1");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, _tempF, 0, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[1].effDown, GameManager.info.skillEffectSetupData[1].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkPhysic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkMagic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[1].effDown, GameManager.info.skillEffectSetupData[1].soundDown);					
		}
		
		return true;
	}
	

	bool type2(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 2) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type2");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, _tempF, 0, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[2].effDown, GameManager.info.skillEffectSetupData[2].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkPhysic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkMagic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[2].effDown, GameManager.info.skillEffectSetupData[2].soundDown);					
		}	
		
		return true;
	}
	
	bool type3(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{

		if(checkApplyTargetType(skillData, skillTarget, 3) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type3");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, _tempF, 0, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[3].effDown, GameManager.info.skillEffectSetupData[3].soundDown );
		}
		else
		{
			bullet.attackerInfo.stat.atkPhysic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkMagic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[3].effDown, GameManager.info.skillEffectSetupData[3].soundDown);					
		}	
		
		return true;
	}	

	
	bool type4(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{

		if(checkApplyTargetType(skillData, skillTarget, 4) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type4");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, _tempF, 0, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[4].effDown, GameManager.info.skillEffectSetupData[4].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkPhysic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkMagic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[4].effDown, GameManager.info.skillEffectSetupData[4].soundDown);					
		}	
		
		return true;
	}		
	
	bool type5(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{

		if(checkApplyTargetType(skillData, skillTarget, 5) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type5");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, 0, _tempF, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[5].effDown, GameManager.info.skillEffectSetupData[5].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkMagic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkPhysic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[5].effDown, GameManager.info.skillEffectSetupData[5].soundDown);					
		}		
		
		return true;
	}		
	
	bool type6(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 6) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type6");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, 0, _tempF, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[6].effDown, GameManager.info.skillEffectSetupData[6].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkMagic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkPhysic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[6].effDown, GameManager.info.skillEffectSetupData[6].soundDown);					
		}		
		
		return true;
	}		
	
	bool type7(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 7) == false)
		{
			return false;
		}


		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type7");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, 0, _tempF, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[7].effDown, GameManager.info.skillEffectSetupData[7].soundDown);
		}
		else
		{
			bullet.attackerInfo.stat.atkMagic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkPhysic.Set( 0 );
			return skillTarget.damage(bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[7].effDown, GameManager.info.skillEffectSetupData[7].soundDown);					
		}		
		
		return true;
	}		
	
	bool type8(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 8) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		float hpValue;
		float calcValue = 0;

		if(bullet != null)
		{
			calcValue = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat);
			hpValue = (calcValue * (1.0f+bullet.attackerInfo.stat.getSkillUp(skillData)) - 0.4f);
			skillTarget.hp += hpValue;
			skillTarget.hpEffect(hpValue);
		}
		else 
		{
			calcValue = getApplyValue(0, applyReinforceLevel, attacker.stat);
			hpValue = (calcValue * (1.0f+attacker.stat.getSkillUp(skillData)) - 0.4f);
			skillTarget.hp += hpValue;
			skillTarget.hpEffect(hpValue);
		}


		if(calcValue > 0)
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[8].effUp].getParticleEffectByCharacterSize(getAttackerMonsterUniqueId(bullet,attacker),skillTarget);	
			skillTarget.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[8].upIcon);
			SoundData.playHitSound(8,true);
		}
		else
		{
			skillTarget.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[8].downIcon);
			SoundData.playHitSound(8,false);
		}



		// 힐 이펙트 
		
		return true;
	}		
	
	
	bool type9(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 9) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}
		
		//Debug.//LogError("-(int) ((float)skillTarget.hp * (getRareValue(0,rareType))*0.01f : " + (-(int) ((float)skillTarget.hp * (getRareValue(0,rareType))*0.01f)));

		float calcValue = 0;

		if(bullet != null)
		{
			calcValue = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat);
		}
		else
		{
			calcValue = getApplyValue(0, applyReinforceLevel, attacker.stat);
		}

		if(calcValue > 0)
		{
			if(string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[9].effUp ) == false)
			{
				GameManager.info.effectData[GameManager.info.skillEffectSetupData[9].effUp].getParticleEffectByCharacterSize(getAttackerMonsterUniqueId(bullet,attacker),skillTarget);	
			}

			float skillHpValue = ((skillTarget.maxHp - skillTarget.hp) * (calcValue)*0.01f);	
			skillTarget.hp += skillHpValue;
			skillTarget.hpEffect(skillHpValue);

			skillTarget.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[9].upIcon);

			SoundData.playHitSound(9,true);
			//Debug.Log(skillTarget.hp);
		}
		else
		{

			skillTarget.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[9].downIcon);
			SoundData.playHitSound(9,false);

			if(string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[9].effDown ) == false)
			{
				if(bullet != null)
				{
					return skillTarget.damage(bullet.attackerInfo.stat.monsterType,attacker,bullet.attackerInfo.stat.uniqueId,true,  - (skillTarget.hp * (calcValue)*0.01f) , true, GameManager.info.skillEffectSetupData[9].effDown) ;
				}
				else
				{
					return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true,  - (skillTarget.hp * (calcValue)*0.01f) , true, GameManager.info.skillEffectSetupData[9].effDown) ;
				}

			}
			else
			{
				if(bullet != null)
				{
					return skillTarget.damage(bullet.attackerInfo.stat.monsterType,attacker,bullet.attackerInfo.stat.uniqueId,true,  - (skillTarget.hp * (calcValue)*0.01f) , false) ;	
				}
				else
				{
					return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true,  - (skillTarget.hp * (calcValue)*0.01f) , false) ;	
				}
			}
		}
		
		return true;
	}		
	
	bool type10(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 10) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type11(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 11) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF, 500.0f);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF, 500.0f);
		}

		return true;
	}		
	
	bool type12(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 12) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}		

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			_tempF2 = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f+ bullet.attackerInfo.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			_tempF2 = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
		}

		skillTarget.characterEffect.addEffect(bullet,attacker,this, _tempF2, _tempF, 500.0f);
		
		return true;
	}		
	
	bool type13(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 13) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			_tempF2 = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f+ bullet.attackerInfo.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			_tempF2 = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
		}

		skillTarget.characterEffect.addEffect(bullet,attacker,this, _tempF2, _tempF, 500.0f);
		
		return true;
	}		
	
	bool type14(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 14) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type15(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 15) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else if(attacker != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type16(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 16) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type17(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 17) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type18(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 18) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else if(attacker != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type19(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{

		if(checkApplyTargetType(skillData, skillTarget, 19) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel,bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel,attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type20(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 20) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type21(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 21) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type22(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 22) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel,bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel,bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel,attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type23(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 23) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{
			return false;
		}			

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel,bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type24(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 24) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{
			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type25(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 25) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		EffectManager.showEfectBySkillEffectType(25, skillTarget, bullet);
		// 넉백 속도.

		if(attr[1][0] > 0)
		{
			if(bullet != null)
			{
				skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), attr[1][0]);//getRareValue(1,rareType));
			}
			else
			{
				skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), attr[1][0]);//getRareValue(1,rareType));
			}

		}
		else
		{
			if(bullet != null)
			{
				skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat)*5.0f);//getRareValue(1,rareType));
			}
			else
			{
				skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), getApplyValue(0, applyReinforceLevel, attacker.stat)*5.0f);//getRareValue(1,rareType));
			}
		}

		return true;
	}		

	

	
	bool type28(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 28) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	
		
		if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[28].effUp) == false)
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[28].effUp].getParticleEffectByCharacterSize(getAttackerMonsterUniqueId(bullet,attacker),skillTarget);
		}

		SoundData.playHitSound(28,true);
		skillTarget.characterEffect.removeEffectBySkillType(Skill.Type.DEBUFF, 29); // 29번은 아군으로 변환시킨거고 데미지가 자동으로 깍여나가는 상황이니까 여기서는 제외한다.
		
		return true;
	}		
	
	bool type29(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 29) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}

		float calcValue = 0;
		
		if(bullet != null)
		{
			calcValue = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat);
		}
		else
		{
			calcValue = getApplyValue(0, applyReinforceLevel, attacker.stat);
		}


		if(skillTarget.stat.monsterType == Monster.TYPE.UNIT)
		{
			GameManager.me.characterManager.changeSideMonsters.Push(skillTarget);	

			skillTarget.characterEffect.addEffect(bullet,attacker,this, calcValue, 1000.0f, 1000.0f );
		}
		
		return true;
	}		
	
	bool type30(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 30) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
		}
		else
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
		}

		skillTarget.characterEffect.addEffect(bullet,attacker,this, 0, _tempF );
		
		return true;
	}		
	
	
	bool type31(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 31) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else 
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		
	
	bool type32(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 32) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet != null)
		{
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat));
		}
		else
		{
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), getApplyValue(1, applyReinforceLevel, attacker.stat));
		}

		return true;
	}		

	
	// 독 공격.
	bool type33(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 33) == false)
		{
			return false;
		}
		

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{

			return false;
		}	

		if(bullet == null)
		{
			_tempF = getApplyValue(0, applyReinforceLevel, attacker.stat) * (1.0f+attacker.stat.getSkillAtkUp(skillData)) * nowApplyDamagePer;
			//Log.log("type33");
			return skillTarget.damage(attacker.stat.monsterType,attacker,attacker.stat.uniqueId,true, 1, attacker.tf, 0, _tempF, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[33].effDown, GameManager.info.skillEffectSetupData[33].soundDown );
		}
		else
		{
			bullet.attackerInfo.stat.atkMagic.Set( getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.originalStat) * (1.0f+bullet.attackerInfo.originalStat.getSkillAtkUp(skillData)) * nowApplyDamagePer );
			bullet.attackerInfo.stat.atkPhysic.Set( 0 );
			return skillTarget.damage( bullet, skillTarget, bullet.damagePer, bullet.minimumDamagePer, true, GameManager.info.skillEffectSetupData[33].effDown, GameManager.info.skillEffectSetupData[33].soundDown  );					
		}	
		
		return true;
	}	






	bool type34(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 34) == false)
		{
			return false;
		}

		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{
			
			return false;
		}	
		
		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF, 500.0f);
		}
		else
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF, 500.0f);
		}
		
		return true;
	}		




	bool type35(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 35) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{
			
			return false;
		}	

		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else if(attacker != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}

		return true;
	}		



	bool type36(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Bullet bullet = null, Monster attacker = null, float nowApplyDamagePer = 1.0f)
	{
		if(checkApplyTargetType(skillData, skillTarget, 36) == false)
		{
			return false;
		}
		
		if(UIPopupSkillPreview.isOpen == false && levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel, bullet, attacker) == false)
		{
			
			return false;
		}	
		
		if(bullet != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, bullet.attackerInfo.stat) * (1.0f + bullet.attackerInfo.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, bullet.attackerInfo.stat), _tempF);
		}
		else if(attacker != null)
		{
			_tempF = getApplyValue(1, applyReinforceLevel, attacker.stat) * (1.0f + attacker.stat.getSkillTimeUp(skillData));
			skillTarget.characterEffect.addEffect(bullet,attacker,this, getApplyValue(0, applyReinforceLevel, attacker.stat), _tempF);
		}
		
		return true;
	}	





	public static bool checkApplyTargetType(BaseSkillData skillData, Monster skillTarget, int checkTypeIndex)
	{
		if(skillTarget.stat.monsterType == Monster.TYPE.NPC && GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToNPC == false)
		{
			return false;
		}
		else if(skillTarget.stat.monsterType == Monster.TYPE.HERO)
		{
			if(skillTarget.isPlayer)
			{
				if(GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToHERO == false)
				{
					return false;
				}
			}
			else
			{
				if(skillData.skillType == Skill.Type.BUFF)
				{
					if(GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToMonsterHeroBuff == false)
					{
						return false;
					}
				}
				else if(skillData.skillType == Skill.Type.DEBUFF)
				{
					if(GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToMonsterHeroDebuff == false)
					{
						return false;
					}
				}
				else if(skillData.skillType == Skill.Type.ATTACK)
				{
					if(GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToMonsterHeroAttack == false)
					{
						return false;
					}
				}
				else if(skillData.skillType == Skill.Type.HEAL)
				{
					if(GameManager.info.skillEffectSetupData[checkTypeIndex].canApplyToMonsterHeroHeal == false)
					{
						return false;
					}
				}
			}
		}

		return true;
	}






	Monster.TYPE getAttackerMonsterType(Bullet bullet, Monster attacker)
	{
		if(bullet != null) return bullet.attackerInfo.stat.monsterType;
		else if(attacker != null) return attacker.stat.monsterType;
		return Monster.TYPE.NONE;
	}

	int getAttackerMonsterUniqueId(Bullet bullet, Monster attacker)
	{
		if(bullet != null) return bullet.attackerInfo.stat.uniqueId;
		else if(attacker != null) return attacker.stat.uniqueId;
		return -1000;
	}



	IFloat getApplyValue(int index, int applyReinforceLevel)
	{
		return GameValueType.getApplyValue(attr[index], valueType[index], applyReinforceLevel);
	}


	IFloat getApplyValue(int index, int applyReinforceLevel, MonsterStat shooterStat)
	{
		return GameValueType.getApplyValue(attr[index], valueType[index], applyReinforceLevel, statValueType[index], shooterStat);
	}





}
