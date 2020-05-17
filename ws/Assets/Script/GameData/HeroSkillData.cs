using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class HeroSkillData : BaseSkillData
{

	public string baseIdWithoutRare;

	#if UNITY_EDITOR
	
	public static List<string> error = new List<string>();
	
	#endif


	public const int CHARING_NORMAL = 0;
	public const int CHARING_LV1 = 1;
	public const int CHARING_LV2 = 2;
	public const int CHARING_LV3 = 3;



//	쿨타임	스킬타입	소모MP
//	COOLTIME	SKILL_TYPE	MP

	public bool isChargingTimeSingle = false;

	public Xfloat minChargingTime = 0f;
	public Xfloat maxChargingTime = 0f;

	public IFloat getChargingTime(int inforceLv)
	{
		//  - N 강화레벨의 풀차징타임 = 최소차징시간 + (최대차징시간 - 최소차징시간) / 19 * (N - 1).
		if(isChargingTimeSingle)
		{
			return MathUtil.RoundToInt( minChargingTime * 100.0f ) * 0.01f;
		}
		else
		{
			return MathUtil.RoundToInt(( minChargingTime + (maxChargingTime - minChargingTime) / 19f * (inforceLv -1 )) * 100.0f ) * 0.01f;
		}
	}


	public Xfloat mp = 0;

	public bool isMonsterSkill = false;

	public bool isBook = false;

	public bool isBase = false;

	public enum LinkResourceType
	{
		Default, HeroSkill, UnitSkill
	}

	public LinkResourceType linkResourceType = LinkResourceType.Default;

	public string linkResource;

	public string resource
	{
		get
		{
			if(isBase)
			{
				switch(linkResourceType)
				{
				case HeroSkillData.LinkResourceType.HeroSkill:
					return linkResource;
					break;
				case HeroSkillData.LinkResourceType.UnitSkill:
					return GameManager.info.unitSkillData[linkResource].getBulletPatternId();
					break;
				default: 
					return id;
				}
			}
			else
			{
				switch(GameManager.info.heroBaseSkillData[baseId].linkResourceType)
				{
				case HeroSkillData.LinkResourceType.HeroSkill:
					return GameManager.info.heroBaseSkillData[baseId].linkResource;
					break;
				case HeroSkillData.LinkResourceType.UnitSkill:
					return GameManager.info.unitSkillData[GameManager.info.heroBaseSkillData[baseId].linkResource].getBulletPatternId();
					break;
				default :
					return baseId;
				}
			}
		}
	}


	public HeroSkillData ()
	{
	}


	public bool isBaseId = false;


	public Xint grade = 0;

	public int rare
	{
		get
		{
			return grade.Get() - 1;
		}
	}

	public static string convertBaseIdToRareId(string inputId)
	{
		return inputId + "_20_0_0";
	}


	sealed public override void setData (List<object> l, Dictionary<string, int> k)
	{
		base.setData (l, k);

		int g = 1;
		Util.parseObject(l[k["GRADE"]], out g, true, 1);
		grade = g;


		/*
		if((l[k["CHARGINGTIME"]].ToString()).Contains("/"))
		{
			float[] ct = Util.stringToFloatArray( (string)l[k["CHARGINGTIME"]], '/');
			maxChargingTime = ct[0];
		}
		else
		{
			Util.parseObject(l[k["CHARGINGTIME"]], out maxChargingTime, true, 1.0f);
		}
		*/

		float[] ct = Util.stringToFloatArray( (l[k["CHARGINGTIME"]].ToString()) , ',');

		minChargingTime = ct[0];
		if(ct.Length > 1)
		{
			maxChargingTime = ct[1];
			isChargingTimeSingle = false;
		}
		else
		{
			isChargingTimeSingle = true;
		}

		float tempCoolTime = 0;
		Util.parseObject(l[k["COOLTIME"]], out tempCoolTime, true, 0);
		coolTime = tempCoolTime;
		hasCoolTime = true;

		float tmp = 0;
		Util.parseObject(l[k["MP"]], out tmp, true, 0);
		mp = tmp;


		isMonsterSkill = (l[k["MP"]].ToString() == "Y");

		switch( (l[k["ATTACHED_EFF"]]).ToString() )
		{
		case "B":
			hasShotEffect = true;
			colorEffectId = E_SHOT_BLUE01;
			break;
		case "Y":
			hasShotEffect = true;
			colorEffectId = E_SHOT_YELLOW01;
			break;
		case "V":
			hasShotEffect = true;
			colorEffectId = E_SHOT_VIOLET01;
			break;
		case "G":
			hasShotEffect = true;
			colorEffectId = E_SHOT_GREEN01;
			break;
		case "R":
			hasShotEffect = true;
			colorEffectId = E_SHOT_RED01;
			break;
		}

		isBook = (l[k["BOOK"]].ToString() == "Y");

		if(isBase)
		{
			string r = ((l[k["RESOURCE"]]).ToString()).ToString();

			if(string.IsNullOrEmpty(r) == false)
			{
				if(r.Contains(":"))
				{
					linkResourceType = LinkResourceType.UnitSkill;
					linkResource = r.Substring(2);
				}
				else
				{
					linkResourceType = LinkResourceType.HeroSkill;
					linkResource = r;
				}
			}
			else
			{
				linkResourceType = LinkResourceType.Default;
			}


#if UNITY_EDITOR
			//Debug.Log(id);
#endif

			baseIdWithoutRare = id.Substring(0,id.IndexOf("_")) + id.Substring(id.IndexOf("_")+2);

		}


#if UNITY_EDITOR

		bool hasWrongTargeting = false;

		switch(exeType)
		{
		case 0:
			break;
		case 1:
			break;
		case 2:

			break;
		case 3:
		case 4:
		case 5:
			if(targeting != 2 && targeting != 3)
			{
				hasWrongTargeting = true;
			}
			break;
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
			if(targeting != 1 && targeting != 2)
			{
				hasWrongTargeting = true;
			}
			break;
		case 12:
			if(targeting != 3)
			{
				hasWrongTargeting = true;
			}
			break;

		case 15:
			if(targeting != 2)
			{
				hasWrongTargeting = true;
			}
			break;
		case 16:
			if(targeting != 0)
			{
				hasWrongTargeting = true;
			}
			break;
		case 17:
			if(targeting != 1)
			{
				hasWrongTargeting = true;
			}
			break;
		case 18:
			if(targeting != 2 && targeting != 3)
			{
				hasWrongTargeting = true;
			}
			break;

		}

		if(hasWrongTargeting)
		{
			Debug.LogError(id + " targeting: " + + targeting + "    atkType : " + exeType);
			error.Add(id + " targeting: " + + targeting + "    atkType : " + exeType);
		}
#endif
	}





	public HeroSkillData clone(HeroSkillData inputData = null)
	{
		HeroSkillData hd;

		if(inputData == null)
		{
			hd = new HeroSkillData();
		}
		else
		{
			hd = inputData;
		}

		hd.grade = this.grade;
		hd.minChargingTime = this.minChargingTime;
		hd.maxChargingTime = this.maxChargingTime;
		hd.isChargingTimeSingle = this.isChargingTimeSingle;
		hd.coolTime = this.coolTime;
		hd.hasCoolTime = this.hasCoolTime;
		hd.mp = this.mp; 

		hd.isMonsterSkill = this.isMonsterSkill;
		hd.skillDataType = this.skillDataType;

		hd.hasShotEffect = this.hasShotEffect;
		hd.colorEffectId = this.colorEffectId;

		hd.isPassiveSkill = this.isPassiveSkill;

		hd.isBook = this.isBook;

		hd.linkResourceType = this.linkResourceType;
		hd.linkResource = this.linkResource;
			 
		hd.baseId = this.baseId;
		hd.isBase = this.isBase;

		hd.id = this.id;
		hd.name = this.name;

		hd.baseLevel = this.baseLevel;

		hd.skillType = this.skillType;

		hd.targetType = this.targetType;

		hd.checkMissChance = this.checkMissChance;

		int len = this.successChance.Length;

		hd.successChance = new int[len][];

		for(int i = 0; i < len; ++i)
		{
			if(this.successChance[i] == null)
			{
				hd.successChance[i] = null;
			}
			else
			{
				hd.successChance[i] = new int[this.successChance[i].Length];
				for(int j = 0; j < hd.successChance[i].Length; ++j)
				{
					hd.successChance[i][j] = this.successChance[i][j];
				}
			}
		}


		len = this.successValueType.Length;
		hd.successValueType = new GameValueType.Type[len];

		for(int i = 0; i < len; ++i)
		{
			hd.successValueType[i] = this.successValueType[i];
		}

		hd.exeType = this.exeType;

		hd.exeData = this.exeData.clone();

		hd.targeting = this.targeting;

		hd.coolTimeStartDelay = this.coolTimeStartDelay;

		hd.isTargetingForward = this.isTargetingForward;

		hd.transcendData = this.transcendData;

		hd.description = "";

		if(this.targetAttr != null)
		{
			hd.targetAttr = new Xint[this.targetAttr.Length];
			Array.Copy(this.targetAttr, hd.targetAttr, this.targetAttr.Length);
		}
		else
		{
			hd.targetAttr = null;
		}

		switch(targeting)
		{
		case TargetingData.FIXED_1:
		case TargetingData.AUTOMATIC_2:

			hd.targetAttr = new Xint[2];	
			hd.targetAttr[0] = this.targetAttr[0];
			hd.targetAttr[1] = this.targetAttr[1];

			break;
		default:
			hd.targetAttr = null;
			break;
		}
		
		hd.setTargetingChecker2();

		len = this.skillEffects.Length;

		hd.skillEffects = new SkillEffectData[len];

		for(int i = 0; i < len; ++i)
		{
			hd.skillEffects[i] = this.skillEffects[i].clone(hd);
		}

		hd.totalEffectNum = this.totalEffectNum;

		hd.isChangeSideSkill = this.isChangeSideSkill;

		hd.baseIdWithoutRare = this.baseIdWithoutRare;

		return hd;
	}



}

