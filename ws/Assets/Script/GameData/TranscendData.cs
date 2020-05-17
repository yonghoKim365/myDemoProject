using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class TranscendData : BaseData
{

	public string id;

	public int[] maxLevel = new int[4];

	public string[] attr = new string[4];

	public int[] attrIndex = new int[4];

	public string[] description = new string[4];

	private int[] _applyValue = new int[4];

	public enum ApplyType
	{
		Rate, Value
	}
	
	private ApplyType[] _applyType = new ApplyType[4];

	sealed public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = l[k["ID"]].ToString();

		Util.parseObject(l[k["ATTR_MAX_LEVEL_1"]], out maxLevel[0], 0);
		Util.parseObject(l[k["ATTR_MAX_LEVEL_2"]], out maxLevel[1], 0);
		Util.parseObject(l[k["ATTR_MAX_LEVEL_3"]], out maxLevel[2], 0);
		Util.parseObject(l[k["ATTR_MAX_LEVEL_4"]], out maxLevel[3], 0);

		attr[0] = l[k["ATTR_DS_1"]].ToString();
		attr[1] = l[k["ATTR_DS_2"]].ToString();
		attr[2] = l[k["ATTR_DS_3"]].ToString();
		attr[3] = l[k["ATTR_DS_4"]].ToString();

		for(int i = 0; i < 4; ++i)
		{
			if(string.IsNullOrEmpty(attr[i]))
			{
				attrIndex[i] = -1000;
				continue;
			}
			attrIndex[i] = WSATTR.getAttrIndexByAttrName(attr[i]);
		}

		parseValue(l[k["ATTR_INC_VALUE_1"]].ToString(),0);
		parseValue(l[k["ATTR_INC_VALUE_2"]].ToString(),1);
		parseValue(l[k["ATTR_INC_VALUE_3"]].ToString(),2);
		parseValue(l[k["ATTR_INC_VALUE_4"]].ToString(),3);

		description[0] = l[k["ATTR_NAME_1"]].ToString();
		description[1] = l[k["ATTR_NAME_2"]].ToString();
		description[2] = l[k["ATTR_NAME_3"]].ToString();
		description[3] = l[k["ATTR_NAME_4"]].ToString();
	}

	private void parseValue(string logic, int index)
	{
		string[] l = logic.Split('_');

		if(l[1] == "R")
		{
			_applyType[index] = ApplyType.Rate;
		}
		else
		{
			_applyType[index] = ApplyType.Value;
		}

		int.TryParse(l[0], out _applyValue[index]);
	}


	/*
	// 해당 속성에 초월 속성을 적용할 수 있는지.
	public bool canApply(int inputAttrIndex)
	{
		for(int i = 0; i < length; ++i)
		{
			if(getAttrIndex(i) == inputAttrIndex) return true;
		}

		return false;
	}
	*/

	public int getAttrIndex(int selectIndex)
	{
		return attrIndex[selectIndex];
	}


//	1레벨 당 상승되는 수치의 값 
//		["N_V" or "N_R" 형식] 
//		(예: '3_V' → 레벨x3 더하기 // '3_R' → 레벨x3 % 증가)

	private IFloat applyRateValue(IFloat originalValue, int pLevel, int valueIndex)
	{
		if(_applyType[valueIndex] == ApplyType.Rate)
		{
			originalValue = originalValue*((IFloat)(100+_applyValue[valueIndex]*pLevel)/100.0f);//*0.01f);

		}
		else
		{
			originalValue = originalValue + _applyValue[valueIndex] * pLevel;
		}

		return originalValue;
	}

	private int applyRateValue(int originalValue, int pLevel, int valueIndex)
	{
		if(_applyType[valueIndex] == ApplyType.Rate)
		{
			originalValue = (((IFloat)(100+(_applyValue[valueIndex] * pLevel))/100.0f)*originalValue).AsInt();
			//originalValue = (((IFloat)(100+(_applyValue[valueIndex] * pLevel))*0.01f)*originalValue).AsInt();

		}
		else
		{
			originalValue = originalValue + _applyValue[valueIndex] * pLevel;
		}
		
		return originalValue;
	}





	//unit : ATK_ATTR1,ATK_ATTR2,ATK_ATTR3,ATK_ATTR4,ATK_ATTR5,ATK_ATTR6,ATK_ATTR7,MOVE_SPEED,ATK_RANGE,ATK_SPEED,ATK_PHYSIC,ATK_MAGIC,DEF_PHYSIC,DEF_MAGIC,HP

	//skill : COOLTIME,MP,  
	//E_ATTR1,E_ATTR2,E_ATTR3,E_ATTR4,E_ATTR5,E_ATTR6,E_ATTR7,
	//T_ATTR1,T_ATTR2,
	//SUCCESS_CHANCE_1,E1_ATTR1,E1_ATTR2,SUCCESS_CHANCE_2,E2_ATTR1,E2_ATTR2,SUCCESS_CHANCE_3,E3_ATTR1,E3_ATTR2,SUCCESS_CHANCE_4,E4_ATTR1,E4_ATTR2

	//HD: HPMAX,MPMAX,MP_RECOVERY,DEF_MAGIC,SKILL_SP_DISCOUNT,1-2_SKILL_ATK_UP,3_SKILL_UP,4-9_SKILL_TIME_UP
	//BD: HPMAX,SPMAX,SP_RECOVERY,DEF_PHYSIC,SUMMON_SP_PER,UNIT_HP_UP,UNIT_DEF_UP
	//WP: MPMAX,MP_RECOVERY,ATK_PHYSIC,ATK_MAGIC,SKILL_SP_DISCOUNT,1-2_SKILL_ATK_UP,3_SKILL_UP,4-9_SKILL_TIME_UP,ATK_RANGE,ATK_SPEED,ATTR1,ATTR2,ATTR3,ATTR4,ATTR5,ATTR6,ATTR7
	//RD: SPMAX,SP_RECOVERY,SPEED,SUMMON_SP_PER,UNIT_HP_UP,UNIT_DEF_UP

	public void apply(HeroSkillData hd, int[] tLevel)
	{
		if(tLevel == null) return;

		hd.exeData.init(AttackData.AttackerType.Hero, AttackData.AttackType.Skill, hd, this, tLevel, true);

		for(int i = 0; i < 4; ++i)
		{
			if(tLevel[i] == 0) continue;

			int attrIndex = getAttrIndex(i);

			switch(attrIndex)
			{
			case WSATTR.COOLTIME_I:
				hd.coolTime = applyRateValue(hd.coolTime, tLevel[i], i);
				break;
			case WSATTR.MP_I:
				hd.mp = applyRateValue(hd.mp.Get (), tLevel[i], i);
				break;
			case  WSATTR.E_ATTR1_I : 
				hd.exeData.attrOriginal[0] = applyRateValue(hd.exeData.attrOriginal[0].Get(), tLevel[i], i);
				hd.exeData.attr[0] = hd.exeData.attrOriginal[0];
				break;
			case  WSATTR.E_ATTR2_I : 
				hd.exeData.attrOriginal[1] = applyRateValue(hd.exeData.attrOriginal[1].Get(), tLevel[i], i);
				hd.exeData.attr[1] = hd.exeData.attrOriginal[1];
				break;
			case  WSATTR.E_ATTR3_I : 
				hd.exeData.attrOriginal[2] = applyRateValue(hd.exeData.attrOriginal[2].Get(), tLevel[i], i);
				hd.exeData.attr[2] = hd.exeData.attrOriginal[2];
				break;
			case  WSATTR.E_ATTR4_I : 
				hd.exeData.attrOriginal[3] = applyRateValue(hd.exeData.attrOriginal[3].Get(), tLevel[i], i);
				hd.exeData.attr[3] = hd.exeData.attrOriginal[3];
				break;
			case  WSATTR.E_ATTR5_I : 
				hd.exeData.attrOriginal[4] = applyRateValue(hd.exeData.attrOriginal[4].Get(), tLevel[i], i);
				hd.exeData.attr[4] = hd.exeData.attrOriginal[4];
				break;
			case  WSATTR.E_ATTR6_I : 
				hd.exeData.attrOriginal[5] = applyRateValue(hd.exeData.attrOriginal[5].Get(), tLevel[i], i);
				hd.exeData.attr[5] = hd.exeData.attrOriginal[5];
				break;
			case  WSATTR.E_ATTR7_I : 
				hd.exeData.attrOriginal[6] = applyRateValue(hd.exeData.attrOriginal[6].Get(), tLevel[i], i);
				hd.exeData.attr[6] = hd.exeData.attrOriginal[6];
				break;
				
			case  WSATTR.T_ATTR1_I : 
				hd.targetAttr[0] = applyRateValue(hd.targetAttr[0].Get(), tLevel[i], i);
				break;
				
			case  WSATTR.T_ATTR2_I : 
				hd.targetAttr[1] = applyRateValue(hd.targetAttr[1].Get(), tLevel[i], i);
				break;
				
			case  WSATTR.SUCCESS_CHANCE_1_I : 
				hd.successChance[0] = applyRateValueToArray(hd.successChance[0], tLevel[i], i);
				break;
				
			case  WSATTR.E1_ATTR1_I : 
				hd.skillEffects[0].attr[0] = applyRateValueToArray(hd.skillEffects[0].attr[0], tLevel[i], i);
				break;
				
			case  WSATTR.E1_ATTR2_I : 
				hd.skillEffects[0].attr[1] = applyRateValueToArray(hd.skillEffects[0].attr[1], tLevel[i], i);
				break;
				
			case  WSATTR.SUCCESS_CHANCE_2_I : 
				hd.successChance[1] = applyRateValueToArray( hd.successChance[1], tLevel[i], i);
				break;
				
			case  WSATTR.E2_ATTR1_I : 
				hd.skillEffects[1].attr[0] = applyRateValueToArray(hd.skillEffects[1].attr[0], tLevel[i], i);
				break;
				
			case  WSATTR.E2_ATTR2_I : 
				hd.skillEffects[1].attr[1] = applyRateValueToArray(hd.skillEffects[1].attr[1], tLevel[i], i);
				break;
				
				
			case  WSATTR.SUCCESS_CHANCE_3_I : 
				hd.successChance[2] = applyRateValueToArray(hd.successChance[2], tLevel[i], i);
				break;
				
			case  WSATTR.E3_ATTR1_I : 
				hd.skillEffects[2].attr[0] = applyRateValueToArray(hd.skillEffects[2].attr[0], tLevel[i], i);
				break;
				
			case  WSATTR.E3_ATTR2_I : 
				hd.skillEffects[2].attr[1] = applyRateValueToArray(hd.skillEffects[2].attr[1], tLevel[i], i);
				break;
				
			case  WSATTR.SUCCESS_CHANCE_4_I : 
				hd.successChance[3] = applyRateValueToArray(hd.successChance[3], tLevel[i], i);
				break;
				
			case  WSATTR.E4_ATTR1_I : 
				hd.skillEffects[3].attr[0] = applyRateValueToArray(hd.skillEffects[3].attr[0], tLevel[i], i);
				break;
				
			case  WSATTR.E4_ATTR2_I : 
				hd.skillEffects[3].attr[1] = applyRateValueToArray(hd.skillEffects[3].attr[1], tLevel[i], i);
				break;
				
			}


		}
	}

	public int getValueByATTR(int[] transcendLevel, int inputValue, int inputIndex)
	{
		if(transcendLevel == null) return inputValue;

		for(int i = 0; i < 4; ++i)
		{
			if(transcendLevel[i] > 0)
			{
				if(attrIndex[i] == inputIndex)
				{
					return applyRateValue(inputValue, transcendLevel[i], i);
				}
			}
		}

		return inputValue;
	}


	public IFloat getValueByATTR(int[] transcendLevel, IFloat inputValue, int inputIndex)
	{
		if(transcendLevel == null) return inputValue;
		
		for(int i = 0; i < 4; ++i)
		{
			if(transcendLevel[i] > 0)
			{
				if(attrIndex[i] == inputIndex)
				{
					return applyRateValue(inputValue, transcendLevel[i], i);
				}
			}
		}
		
		return inputValue;
	}














	Xint[] applyRateValueToArray(Xint[] arr, int transLevel, int targetIndex)
	{
		for(int i = arr.Length - 1; i >= 0; --i)
		{
			arr[i] = applyRateValue(arr[i].Get(), transLevel, targetIndex);
		}

		return arr;
	}

	int[] applyRateValueToArray(int[] arr, int transLevel, int targetIndex)
	{
		for(int i = arr.Length - 1; i >= 0; --i)
		{
			arr[i] = applyRateValue(arr[i], transLevel, targetIndex);
		}
		
		return arr;
	}

	Xfloat[] applyRateValueToArray(Xfloat[] arr, int transLevel, int targetIndex)
	{
		for(int i = arr.Length - 1; i >= 0; --i)
		{
			arr[i] = applyRateValue(arr[i], transLevel, targetIndex);
		}
		
		return arr;
	}




	/*
	public void setDescriptionLine(UIPopupTranscendDescriptionLine[] lines, int inputTranscendLevel)
	{

		for(int i = 0; i < lines.Length; ++i)
		{
			if(length > i && setValue(getLogic(i), inputTranscendLevel))
			{
				lines[i].gameObject.SetActive(true);
				lines[i].lbName.text = getAttrDesc(i);

				if(_applyValue < 0)
				{
					lines[i].lbValue.text = "[c51313]"+_applyValue+((_applyType == ApplyType.Rate)?"%[-]":"[-]");
					lines[i].lbUpDown.text = "d";
				}
				else
				{
					lines[i].lbValue.text = "[ffd200]"+_applyValue+((_applyType == ApplyType.Rate)?"%[-]":"[-]");
					lines[i].lbUpDown.text = "u";
				}
			}
			else
			{
				lines[i].gameObject.SetActive(false);
			}

		}
	}
	*/



	//	1레벨 당 상승되는 수치의 값 
	//		["N_V" or "N_R" 형식] 
	//		(예: '3_V' → 레벨x3 더하기 // '3_R' → 레벨x3 % 증가)
	
	public string getApplyRateValueString(int pLevel, int valueIndex)
	{
		int resultValue = (pLevel * _applyValue[valueIndex]);
		string prefix = "";

		if(resultValue > 0)
		{
			prefix = "+";
		}

		if(_applyType[valueIndex] == ApplyType.Rate)
		{
			return prefix + resultValue + "%";
		}
		else
		{
			return prefix + resultValue + "";
		}
	}

	public int getApplyRateValue(int pLevel, int valueIndex)
	{
		return (pLevel * _applyValue[valueIndex]);
	}

	public string getApplyRateTypeString(int valueIndex)
	{
		if(_applyType[valueIndex] == ApplyType.Rate)
		{
			return "%";
		}
		else
		{
			return "";
		}
	}

}

