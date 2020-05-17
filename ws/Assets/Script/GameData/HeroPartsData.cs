using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroPartsData
{

	public const string HPMAX = "HPMAX";
	public const string SPMAX = "SPMAX";
	public const string SP_RECOVERY = "SP_RECOVERY";
	public const string MPMAX = "MPMAX";
	public const string MP_RECOVERY = "MP_RECOVERY";
	public const string ATK_PHYSIC = "ATK_PHYSIC";
	public const string ATK_MAGIC = "ATK_MAGIC";
	public const string DEF_PHYSIC = "DEF_PHYSIC";
	public const string DEF_MAGIC = "DEF_MAGIC";
	public const string SPEED = "SPEED";
	public const string SUMMON_SP_PER = "SUMMON_SP_PER";
	public const string UNIT_HP_UP = "UNIT_HP_UP";
	public const string UNIT_DEF_UP = "UNIT_DEF_UP";
	public const string SKILL_SP_DISCOUNT = "SKILL_SP_DISCOUNT";
	public const string SKILL_ATK_UP = "1-2_SKILL_ATK_UP";
	public const string SKILL_UP = "3_SKILL_UP";
	public const string SKILL_TIME_UP = "4-9_SKILL_TIME_UP";
	public const string ATK_RANGE = "ATK_RANGE";
	public const string ATK_SPEED = "ATK_SPEED";


	public HeroPartsData ()
	{
	}

	public string id;

	private BaseHeroPartsDataInfo _baseData;

	public string baseId { get { return _baseData.baseId; } }

	public int baseLevel { get { return _baseData.baseLevel; } }

	public Xint grade { get { return _baseData.grade; } }

	public string name { get { return GameManager.info.heroBasePartsData[_baseData.baseId].name; } }
	public string character { get { return GameManager.info.heroBasePartsData[_baseData.baseId].character; } }

	public string baseIdWithoutRare 
	{ 
		get 
		{ 
			if(GameManager.info.heroBasePartsData.ContainsKey(_baseData.baseId))
			{
				return GameManager.info.heroBasePartsData[_baseData.baseId].baseIdWithoutRare; 
			}
			else
			{
				return string.Empty;
			}
		} 
	}

	public string resource 
	{ 
		get 
		{ 
			if(type == HeroParts.VEHICLE)
			{
				if(GameManager.info.heroBasePartsData[_baseData.baseId].resource.Contains("griffon"))
				{
					return "griffon";
				}
				else if(GameManager.info.heroBasePartsData[_baseData.baseId].resource.Contains("pegasus"))
				{
					return "pegasus";
				}
				else if(GameManager.info.heroBasePartsData[_baseData.baseId].resource.Contains("fenrir"))
				{
					return "fenrir";
				}
				else if(GameManager.info.heroBasePartsData[_baseData.baseId].resource.Contains("dragon"))
				{
					return "dragon";
				}
			}

			return GameManager.info.heroBasePartsData[_baseData.baseId].resource; 
		} 
	}

	public string vehicleResource
	{
		get
		{
			return GameManager.info.heroBasePartsData[_baseData.baseId].resource; 
		}
	}

	public string texture
	{
		get
		{
			return GameManager.info.heroBasePartsData[_baseData.baseId].texture; 
		}
	}


	public string type { get { return GameManager.info.heroBasePartsData[_baseData.baseId].type; } }
	public string icon { get { return GameManager.info.heroBasePartsData[_baseData.baseId].icon; } }

	public float[] previewSettingValue { get { return GameManager.info.heroBasePartsData[_baseData.baseId].previewSettingValue; } }

	public GameValueType.Type[] valueTypeDic { get { return _baseData.valueTypeDic; } }


	public int rare
	{
		get
		{
			return grade-1;
		}
	}


	public Xfloat[] getArrayValueByIndex(int index)
	{
		switch(index)
		{
		case BaseHeroPartsDataInfo.INDEX_HP_MAX: return hpMax;
		case BaseHeroPartsDataInfo.INDEX_SP_MAX: return spMax;
		case BaseHeroPartsDataInfo.INDEX_SP_RECOVERY: return spRecovery;
		case BaseHeroPartsDataInfo.INDEX_MP_MAX: return mpMax;
		case BaseHeroPartsDataInfo.INDEX_MP_RECOVERY: return mpRecovery;
		case BaseHeroPartsDataInfo.INDEX_ATK_PHYSIC: return atkPhysic;
		case BaseHeroPartsDataInfo.INDEX_ATK_MAGIC: return atkMagic;
		case BaseHeroPartsDataInfo.INDEX_DEF_PHYSIC: return defPhysic;
		case BaseHeroPartsDataInfo.INDEX_DEF_MAGIC: return defMagic;
		case BaseHeroPartsDataInfo.INDEX_SPEED: return speed;
			
		case BaseHeroPartsDataInfo.INDEX_SUMMON_SP_PERCENT: return summonSpPercent;
		case BaseHeroPartsDataInfo.INDEX_UNIT_HP_UP: return unitHpUp;
		case BaseHeroPartsDataInfo.INDEX_UNIT_DEF_UP: return unitDefUp;
			
			
		case BaseHeroPartsDataInfo.INDEX_SKILL_SP_DISCOUNT: return skillSpDiscount;
		case BaseHeroPartsDataInfo.INDEX_SKILL_ATK_UP: return skillAtkUp;
		case BaseHeroPartsDataInfo.INDEX_SKILL_UP: return skillUp;
		case BaseHeroPartsDataInfo.INDEX_SKILL_TIME_UP: return skillTimeUp;
		}

		return null;
	}





	public Xfloat[] hpMax { get { return _baseData.hpMax; } } 
	public Xfloat[] spMax { get { return _baseData.spMax; } } 
	public Xfloat[] spRecovery { get { return _baseData.spRecovery; } } 
	public Xfloat[] mpMax { get { return _baseData.mpMax; } } 
	public Xfloat[] mpRecovery { get { return _baseData.mpRecovery; } } 
	public Xfloat[] atkPhysic { get { return _baseData.atkPhysic; } } 
	public Xfloat[] atkMagic { get { return _baseData.atkMagic; } } 
	public Xfloat[] defPhysic { get { return _baseData.defPhysic; } } 
	public Xfloat[] defMagic { get { return _baseData.defMagic; } } 
	public Xfloat[] speed { get { return _baseData.speed; } } 
	public Xfloat[] summonSpPercent { get { return _baseData.summonSpPercent; } } 
	public Xfloat[] unitHpUp { get { return _baseData.unitHpUp; } } 
	public Xfloat[] unitDefUp { get { return _baseData.unitDefUp; } } 
	public Xfloat[] skillSpDiscount { get { return _baseData.skillSpDiscount; } } 
	

	public Xfloat[] skillAtkUp { get { return _baseData.skillAtkUp; } } 
	public Xfloat[] skillUp { get { return _baseData.skillUp; } } 
	public Xfloat[] skillTimeUp { get { return _baseData.skillTimeUp; } } 
	public Xfloat[] atkSpeed { get { return _baseData.atkSpeed; } } 
	public float atkRange { get { return _baseData.atkRange; } } 
	
	public AttackData attackType { get { return _baseData.attackType; } }

	public string descriptionSuperRare { get { return _baseData.descriptionSuperRare; } } 
	public string descriptionLegend { get { return _baseData.descriptionLegend; } } 

	public void setData(List<object> list, Dictionary<string, int> k, BaseHeroPartsDataInfo baseData)
	{
		_baseData = baseData;
	}


	public float nowSummonSpPercent = 0.0f;
	public float nowUnitHpUp = 0.0f;
	public float nowUnitDefUp = 0.0f;
	public float nowSkillSpDiscount = 0.0f;
	public float nowSkillAtkUp = 0.0f;
	public float nowSkillUp = 0.0f;
	public float nowSkillTimeUp = 0.0f;

	
	public void setDataToCharacter(Monster cha, GameIDData infoData)
	{
		int reinforcement = infoData.reinforceLevel;
		int rare = infoData.rare;

		if(infoData.totalPLevel > 0)
		{
			cha.maxHp += Mathf.CeilToInt( infoData.getTranscendValueByATTR( GameValueType.getPartsApplyValue(hpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_HP_MAX],reinforcement),WSATTR.HPMAX_I) - 0.4f);
			cha.hp += Mathf.CeilToInt( infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(hpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_HP_MAX],reinforcement),WSATTR.HPMAX_I) - 0.4f);
			
			cha.maxSp += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(spMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_MAX],reinforcement),WSATTR.SPMAX_I);
			
			cha.sp += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(spMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_MAX],reinforcement),WSATTR.SPMAX_I);
			
			cha.stat.spRecovery += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(spRecovery,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_RECOVERY],reinforcement),WSATTR.SP_RECOVERY_I); 
			
			cha.maxMp += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(mpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_MAX],reinforcement),WSATTR.MPMAX_I); 
			
			cha.stat.mpRecovery += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(mpRecovery,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_RECOVERY],reinforcement),WSATTR.MP_RECOVERY_I); 
			
			cha.mp += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(mpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_MAX],reinforcement),WSATTR.MPMAX_I); 
			
			cha.stat.atkPhysic += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(atkPhysic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_PHYSIC],reinforcement),WSATTR.ATK_PHYSIC_I); 
			
			cha.stat.atkMagic += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(atkMagic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_MAGIC],reinforcement),WSATTR.ATK_MAGIC_I); 
			
			cha.stat.defPhysic += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(defPhysic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_DEF_PHYSIC],reinforcement),WSATTR.DEF_PHYSIC_I); 
			
			cha.stat.defMagic += infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(defMagic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_DEF_MAGIC],reinforcement),WSATTR.DEF_MAGIC_I); 

			IFloat tempS = GameValueType.getPartsApplyValue(speed,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SPEED],reinforcement);
			IFloat tSpeed = infoData.getTranscendValueByATTR(tempS,WSATTR.SPEED_I);

			if(tempS > 0)
			{
				cha.stat.originalAtkSpeedRate = cha.stat.originalAtkSpeedRate *  cha.stat.changeAnimationSpeed(tSpeed, tempS);
				cha.stat.speed += tSpeed;
			}

			/*
			nowSummonSpPercent = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(summonSpPercent,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SUMMON_SP_PERCENT],reinforcement) * 0.01f, WSATTR.SUMMON_SP_PER_I);
			cha.stat.summonSpPercent += nowSummonSpPercent;
			
			nowUnitHpUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(unitHpUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_HP_UP],reinforcement) * 0.01f, WSATTR.UNIT_HP_UP_I);
			cha.stat.unitHpUp += nowUnitHpUp;
			
			nowUnitDefUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(unitDefUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_DEF_UP],reinforcement) * 0.01f, WSATTR.UNIT_DEF_UP_I);
			cha.stat.unitDefUp += nowUnitDefUp;
			
			nowSkillSpDiscount = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillSpDiscount,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_SP_DISCOUNT],reinforcement) * 0.01f, WSATTR.SKILL_SP_DISCOUNT_I);
			cha.stat.skillMpDiscount += nowSkillSpDiscount;
			
			nowSkillAtkUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillAtkUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_ATK_UP],reinforcement) * 0.01f, WSATTR.SKILL_ATK_UP_I);
			cha.stat.skillAtkUp += nowSkillAtkUp;
			
			nowSkillUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_UP],reinforcement) * 0.01f, WSATTR.SKILL_UP_I);
			cha.stat.skillUp += nowSkillUp;
			
			nowSkillTimeUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillTimeUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_TIME_UP],reinforcement) * 0.01f, WSATTR.SKILL_TIME_UP_I);
			cha.stat.skillTimeUp += nowSkillTimeUp;
			*/


			nowSummonSpPercent = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(summonSpPercent,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SUMMON_SP_PERCENT],reinforcement), WSATTR.SUMMON_SP_PER_I) * 0.01f;
			cha.stat.summonSpPercent += nowSummonSpPercent;
			
			nowUnitHpUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(unitHpUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_HP_UP],reinforcement), WSATTR.UNIT_HP_UP_I) * 0.01f;
			cha.stat.unitHpUp += nowUnitHpUp;
			
			nowUnitDefUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(unitDefUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_DEF_UP],reinforcement), WSATTR.UNIT_DEF_UP_I) * 0.01f;
			cha.stat.unitDefUp += nowUnitDefUp;
			
			nowSkillSpDiscount = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillSpDiscount,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_SP_DISCOUNT],reinforcement), WSATTR.SKILL_SP_DISCOUNT_I) * 0.01f;
			cha.stat.skillMpDiscount += nowSkillSpDiscount;
			
			nowSkillAtkUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillAtkUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_ATK_UP],reinforcement), WSATTR.SKILL_ATK_UP_I) * 0.01f;
			cha.stat.skillAtkUp += nowSkillAtkUp;
			
			nowSkillUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_UP],reinforcement), WSATTR.SKILL_UP_I) * 0.01f;
			cha.stat.skillUp += nowSkillUp;
			
			nowSkillTimeUp = infoData.getTranscendValueByATTR(GameValueType.getPartsApplyValue(skillTimeUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_TIME_UP],reinforcement), WSATTR.SKILL_TIME_UP_I) * 0.01f;
			cha.stat.skillTimeUp += nowSkillTimeUp;




			IFloat tempAtkSpeed = GameValueType.getPartsApplyValue(atkSpeed,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_SPEED],reinforcement);

			if(tempAtkSpeed > 0)
			{
				IFloat tAtkSpeed = infoData.getTranscendValueByATTR(tempAtkSpeed, WSATTR.ATK_SPEED_I);
				cha.stat.originalAtkSpeedRate = cha.stat.originalAtkSpeedRate * cha.stat.changeAnimationSpeed(tAtkSpeed, tempAtkSpeed);
				
				cha.stat.atkSpeed += tAtkSpeed;
				cha.stat.atkSpeed = MathUtil.Round(cha.stat.atkSpeed * 100.0f) * 0.01f;
			}

			cha.stat.atkRange = infoData.getTranscendValueByATTR(atkRange, WSATTR.ATK_RANGE_I);




		}
		else
		{
			cha.maxHp += Mathf.CeilToInt( GameValueType.getPartsApplyValue(hpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_HP_MAX],reinforcement) - 0.4f);
			cha.hp += Mathf.CeilToInt(GameValueType.getPartsApplyValue(hpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_HP_MAX],reinforcement) - 0.4f);
			
			cha.maxSp += GameValueType.getPartsApplyValue(spMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_MAX],reinforcement);
			
			cha.sp += GameValueType.getPartsApplyValue(spMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_MAX],reinforcement);
			
			cha.stat.spRecovery += GameValueType.getPartsApplyValue(spRecovery,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SP_RECOVERY],reinforcement); 
			
			cha.maxMp += GameValueType.getPartsApplyValue(mpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_MAX],reinforcement); 
			
			cha.stat.mpRecovery += GameValueType.getPartsApplyValue(mpRecovery,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_RECOVERY],reinforcement); 
			
			cha.mp += GameValueType.getPartsApplyValue(mpMax,valueTypeDic[BaseHeroPartsDataInfo.INDEX_MP_MAX],reinforcement); 
			
			cha.stat.atkPhysic += GameValueType.getPartsApplyValue(atkPhysic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_PHYSIC],reinforcement); 
			
			cha.stat.atkMagic += GameValueType.getPartsApplyValue(atkMagic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_MAGIC],reinforcement); 
			
			cha.stat.defPhysic += GameValueType.getPartsApplyValue(defPhysic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_DEF_PHYSIC],reinforcement); 
			
			cha.stat.defMagic += GameValueType.getPartsApplyValue(defMagic,valueTypeDic[BaseHeroPartsDataInfo.INDEX_DEF_MAGIC],reinforcement); 
			
			cha.stat.speed += GameValueType.getPartsApplyValue(speed,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SPEED],reinforcement); 
			
			nowSummonSpPercent = GameValueType.getPartsApplyValue(summonSpPercent,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SUMMON_SP_PERCENT],reinforcement) * 0.01f;
			cha.stat.summonSpPercent += nowSummonSpPercent;
			
			nowUnitHpUp = GameValueType.getPartsApplyValue(unitHpUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_HP_UP],reinforcement) * 0.01f;
			cha.stat.unitHpUp += nowUnitHpUp;
			
			nowUnitDefUp = GameValueType.getPartsApplyValue(unitDefUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_UNIT_DEF_UP],reinforcement) * 0.01f;
			cha.stat.unitDefUp += nowUnitDefUp;
			
			nowSkillSpDiscount = GameValueType.getPartsApplyValue(skillSpDiscount,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_SP_DISCOUNT],reinforcement) * 0.01f;
			cha.stat.skillMpDiscount += nowSkillSpDiscount;
			
			nowSkillAtkUp = GameValueType.getPartsApplyValue(skillAtkUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_ATK_UP],reinforcement) * 0.01f;
			cha.stat.skillAtkUp += nowSkillAtkUp;
			
			nowSkillUp = GameValueType.getPartsApplyValue(skillUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_UP],reinforcement) * 0.01f;
			cha.stat.skillUp += nowSkillUp;
			
			nowSkillTimeUp = GameValueType.getPartsApplyValue(skillTimeUp,valueTypeDic[BaseHeroPartsDataInfo.INDEX_SKILL_TIME_UP],reinforcement) * 0.01f;
			cha.stat.skillTimeUp += nowSkillTimeUp;
			
			cha.stat.atkSpeed += GameValueType.getPartsApplyValue(atkSpeed,valueTypeDic[BaseHeroPartsDataInfo.INDEX_ATK_SPEED],reinforcement);
			cha.stat.atkSpeed = MathUtil.Round(cha.stat.atkSpeed * 100.0f) * 0.01f;
			
			cha.stat.atkRange = atkRange;
		}
	}		



	public string getIcon()
	{
		return GameManager.info.heroBasePartsData[_baseData.baseId].icon;
	}




	public static int sortValueByPartsType(string type)
	{
		switch(type)
		{
		case HeroParts.HEAD:
			return 0;
			break;
		case HeroParts.BODY:
			return 1;
			break;
		case HeroParts.WEAPON:
			return 2;
			break;
		case HeroParts.VEHICLE:
			return 3;
			break;
		}
		return 0;
	}

	public static int sortValueByPartsCharacter(string type)
	{
		if(type == UIHero.nowHero) return 5;

		switch(type)
		{
		case Character.LEO:
			return 4;
			break;
		case Character.KILEY:
			return 3;
			break;
		case Character.CHLOE:
			return 2;
			break;
		case Character.LUKE:
			return 1;
			break;
		}
		return 0;
	}


	public bool setPreviewPosition(Monster sampleHero)
	{
		if(sampleHero == null) return false;
		if(previewSettingValue == null || previewSettingValue.Length != 7) return false;

		Vector3 _v; Quaternion _q = new Quaternion();
		
		_v = sampleHero.cTransform.localScale;
		_v.x = previewSettingValue[0]; 
		_v.y = previewSettingValue[0]; 
		_v.z = previewSettingValue[0];
		sampleHero.cTransform.localScale = _v;
		
		_v.x = previewSettingValue[1];
		_v.y = previewSettingValue[2];
		_v.z = previewSettingValue[3];
		
		sampleHero.cTransform.localPosition = _v;
		
		_v.x = previewSettingValue[4];
		_v.y = previewSettingValue[5];
		_v.z = previewSettingValue[6];
		
		_q.eulerAngles = _v;
		
		sampleHero.cTransform.localRotation = _q;

		return true;
	}



//	public static int sortForUpgrade(HeroPartsData x, HeroPartsData y)
//	{
//		int i = x.rare.CompareTo(y.rare);
//		if (i == 0) i = y.inforceLevel.CompareTo(x.inforceLevel); 
//		return i;
//	}


	public static int getRareByBaseId(string inputId)
	{
		switch(inputId.Substring(inputId.Length-2,1))
		{
		case "1":
			return RareType.D;
		case "2":
			return RareType.C;
		case "3":
			return RareType.B;
		case "4":
			return RareType.A;
		case "5":
			return RareType.S;
		case "6":
			return RareType.SS;
		}
		
		return RareType.D;
		
	}


	public static string convertBaseIdToRareId(string inputId)
	{
		return inputId.Substring(0,inputId.LastIndexOf("_"))+inputId.Substring(inputId.Length-2,1)+"_"+inputId.Substring(inputId.Length-1)+"1_20";
	}

}



public class BaseHeroPartsDataInfo
{
	public string baseId;

	public GameValueType.Type[] valueTypeDic = new GameValueType.Type[18];

	public Xint grade = 0;

	public const int INDEX_HP_MAX = 0;
	public const int INDEX_SP_MAX = 1;
	public const int INDEX_SP_RECOVERY = 2;
	public const int INDEX_MP_MAX = 3;
	public const int INDEX_MP_RECOVERY = 4;
	public const int INDEX_ATK_PHYSIC = 5;
	public const int INDEX_ATK_MAGIC = 6;
	public const int INDEX_DEF_PHYSIC = 7;
	public const int INDEX_DEF_MAGIC = 8;
	public const int INDEX_SPEED = 9;
	public const int INDEX_SUMMON_SP_PERCENT = 10;
	public const int INDEX_UNIT_HP_UP = 11;
	public const int INDEX_UNIT_DEF_UP = 12;
	public const int INDEX_SKILL_SP_DISCOUNT = 13;
	public const int INDEX_SKILL_ATK_UP = 14;
	public const int INDEX_SKILL_UP = 15;
	public const int INDEX_SKILL_TIME_UP = 16;
	public const int INDEX_ATK_SPEED = 17;

	public int baseLevel = 1;

	public Xfloat[] hpMax; //0
	public Xfloat[] spMax; //1
	public Xfloat[] spRecovery; //2
	public Xfloat[] mpMax; //3
	public Xfloat[] mpRecovery; //4
	public Xfloat[] atkPhysic; //5
	public Xfloat[] atkMagic; //6
	public Xfloat[] defPhysic; //7
	public Xfloat[] defMagic; //8
	public Xfloat[] speed; //9
	public Xfloat[] summonSpPercent; //10
	public Xfloat[] unitHpUp; //11
	public Xfloat[] unitDefUp; //12
	public Xfloat[] skillSpDiscount; //13

	public Xfloat[] skillAtkUp; //14
	public Xfloat[] skillUp; //15
	public Xfloat[] skillTimeUp;	//16
	
	public Xfloat[] atkSpeed; //17

	public float atkRange = 0.0f;
	
	public float nowSummonSpPercent = 0.0f;
	public float nowUnitHpUp = 0.0f;
	public float nowUnitDefUp = 0.0f;
	public float nowSkillSpDiscount = 0.0f;
	public float nowSkillAtkUp = 0.0f;
	public float nowSkillUp = 0.0f;
	public float nowSkillTimeUp = 0.0f;
	

	public string descriptionSuperRare = "";
	public string descriptionLegend = "";
	


	int atkType;
	object attr1;
	object attr2;
	object attr3;
	object attr4;
	object attr5;
	object attr6;
	object attr7;
	
	AttackData _attackType = null;
	public AttackData attackType
	{
		get
		{
			if(_attackType == null)
			{
				_attackType = AttackData.getAttackData(atkType, attr1, attr2, attr3, attr4, attr5, attr6, attr7);
				attr1 = null;
				attr2 = null;
				attr3 = null;
				attr4 = null;
				attr5 = null;
				attr6 = null;
				attr7 = null;
			}
			
			return _attackType;
		}
	}

	const string NORMAL = "NORMAL";
	const string RARE = "RARE";
	const string SUPERRARE = "SUPERRARE";
	const string LEGEND = "LEGEND";


	public void setData(List<object> list, Dictionary<string, int> k)
	{
		baseId = (string)list[k["BASE"]];

		Util.parseObject(list[k["BASELV"]], out baseLevel, true, 0);

		int g = 1;
		Util.parseObject(list[k["GRADE"]], out g, true, 1);
		grade = g;

		parseStat(list[k[HeroPartsData.HPMAX]],out hpMax, INDEX_HP_MAX);
		parseStat(list[k[HeroPartsData.SPMAX]],out spMax, INDEX_SP_MAX);
		parseStat(list[k[HeroPartsData.SP_RECOVERY]],out spRecovery, INDEX_SP_RECOVERY);
		parseStat(list[k[HeroPartsData.MPMAX]],out mpMax, INDEX_MP_MAX);
		parseStat(list[k[HeroPartsData.MP_RECOVERY]],out mpRecovery, INDEX_MP_RECOVERY);
		parseStat(list[k[HeroPartsData.ATK_PHYSIC]],out atkPhysic, INDEX_ATK_PHYSIC);
		parseStat(list[k[HeroPartsData.ATK_MAGIC]],out atkMagic, INDEX_ATK_MAGIC);
		parseStat(list[k[HeroPartsData.DEF_PHYSIC]],out defPhysic, INDEX_DEF_PHYSIC);
		parseStat(list[k[HeroPartsData.DEF_MAGIC]],out defMagic, INDEX_DEF_MAGIC);
		parseStat(list[k[HeroPartsData.SPEED]],out speed, INDEX_SPEED);
		parseStat(list[k[HeroPartsData.SUMMON_SP_PER]],out summonSpPercent, INDEX_SUMMON_SP_PERCENT);
		parseStat(list[k[HeroPartsData.UNIT_HP_UP]],out unitHpUp, INDEX_UNIT_HP_UP);
		parseStat(list[k[HeroPartsData.UNIT_DEF_UP]],out unitDefUp, INDEX_UNIT_DEF_UP);
		parseStat(list[k[HeroPartsData.SKILL_SP_DISCOUNT]],out skillSpDiscount, INDEX_SKILL_SP_DISCOUNT);
		parseStat(list[k[HeroPartsData.SKILL_ATK_UP]],out skillAtkUp, INDEX_SKILL_ATK_UP);
		parseStat(list[k[HeroPartsData.SKILL_UP]],out skillUp, INDEX_SKILL_UP);
		parseStat(list[k[HeroPartsData.SKILL_TIME_UP]],out skillTimeUp, INDEX_SKILL_TIME_UP);
		
		Util.parseObject(list[k["ATK_RANGE"]],out atkRange, true, 0.0f);

		parseStat(list[k["ATK_SPEED"]],out atkSpeed, INDEX_ATK_SPEED);
		for(int i = atkSpeed.Length - 1; i >= 0; --i)
		{
			atkSpeed[i] *= 0.001f;
		}

		atkType = 0;
		Util.parseObject(list[k["ATK_TYPE"]], out atkType, true, 1);
		
		attr1 = list[k["ATTR1"]];
		attr2 = list[k["ATTR2"]];
		attr3 = list[k["ATTR3"]];
		attr4 = list[k["ATTR4"]];
		attr5 = list[k["ATTR5"]];
		attr6 = list[k["ATTR6"]];
		attr7 = list[k["ATTR7"]];


		descriptionSuperRare = list[k["DESC_S"]].ToString().Replace("\\n","\n");
		descriptionLegend = list[k["DESC_L"]].ToString().Replace("\\n","\n");
	}


	
	
	void parseStat(object rawData, out Xfloat[] val, int typeDicIndex)
	{
		if(rawData is String)
		{
			if(((string)rawData).Contains(","))
			{
				val = Util.stringToXFloatArray((string)rawData,',');
				valueTypeDic[typeDicIndex] = GameValueType.Type.Fixed;
			}
			else
			{
				val = new Xfloat[1];
				Util.parseObject(rawData, out val[0], true, 0);
				valueTypeDic[typeDicIndex] = GameValueType.Type.Single;

			}

		}
		else
		{
			val = new Xfloat[1];
			Util.parseObject(rawData, out val[0], true, 0);
			valueTypeDic[typeDicIndex] = GameValueType.Type.Single;
		}
	}









}






public class HeroParts
{
	public const string VEHICLE = "RD"; //3
	public const string HEAD = "HD"; //0
	public const string WEAPON = "WP"; //2
	public const string BODY = "BD"; //1	

}

