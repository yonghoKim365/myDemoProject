using System;
using UnityEngine;

sealed public partial class SkillEffectData
{

	const string PLUS_PREFIX = "[ffe400] (↑";
	const string MINUS_PREFIX = "[be1010] (↓";

	public string getEffectDurationString(int reinforceLevel, int attackDuration, int compareReinforceLevel = -1, float valueMulti = 1, float timeOffset = 500f)
	{
		bool hasCompareValue = (compareReinforceLevel > 0);

		float value = 0;

		float compareValue = 0;

		if(type != 28 && type != 29)
		{
			value = getApplyValue(0,reinforceLevel);
			if(hasCompareValue) compareValue = getApplyValue(0,compareReinforceLevel);
		}

		if(attackDuration > 0)
		{
			if( (type >= 1 && type <= 9) || type == 33)
			{
				value = value * (float)attackDuration / timeOffset;
				if(hasCompareValue) compareValue = compareValue * (float)attackDuration / timeOffset;
			}
		}

		string str = "";
		string tempString = "";

		// ~~ 동안. [ 비교대상 ]
		if(isDurationType)
		{
			if( string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[type].timeText ) == false)
			{
				float dValue = 0;

				if(type == 30)
				{
					dValue = getApplyValue(0,reinforceLevel);
					dValue *= 0.001f;
				}
				else
				{
					dValue = getApplyValue(1,reinforceLevel);
					dValue *= 0.001f;
				}

				if(hasCompareValue)
				{
					float d2Value = 0;
					if(type == 30)
					{
						d2Value = getApplyValue(0,compareReinforceLevel);
						d2Value *= 0.001f;
					}
					else
					{
						d2Value = getApplyValue(1,compareReinforceLevel);
						d2Value *= 0.001f;
					}

					tempString = UIPopupSkillPreview.getStatStringValue(dValue - d2Value);

					if(dValue >= d2Value)
					{
						str = GameManager.info.skillEffectSetupData[type].getTimeText( UIPopupSkillPreview.getStatStringValue(dValue) + checkUpDownString(PLUS_PREFIX , UIPopupSkillPreview.getStatStringValue(dValue - d2Value) , ")")+ "[-]");
					}
					else
					{
						str = GameManager.info.skillEffectSetupData[type].getTimeText( UIPopupSkillPreview.getStatStringValue(dValue) + checkUpDownString(MINUS_PREFIX , UIPopupSkillPreview.getStatStringValue(dValue - d2Value) , ")")+ "[-]");
					}
				}
				else
				{
					str = GameManager.info.skillEffectSetupData[type].getTimeText( UIPopupSkillPreview.getStatStringValue(dValue) );
				}

			}
		}

		if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[type].targetText) == false)
		{
			str += GameManager.info.skillEffectSetupData[type].targetText + " ";
		}

		if(type == 25)
		{
			value *= 0.01f;
			compareValue *= 0.01f;
		}

		value *= valueMulti;
		compareValue *= valueMulti;


		// 지속시간 + 효과이름 + 효과양 

		bool isUpDownValue = ( string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[type].upText ) == false);

		// ~~ 한다. [비교대상]
		if(isUpDownValue)
		{
			if(value > 0)
			{
				if(hasCompareValue)
				{
					if(value >= compareValue)
					{
						str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(value) + checkUpDownString( PLUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")") +"[-]", GameManager.info.skillEffectSetupData[type].upText);
					}
					else
					{
						str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(value) + checkUpDownString( MINUS_PREFIX, UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")") +"[-]", GameManager.info.skillEffectSetupData[type].upText);
					}
				}
				else // 비교값이 없을때.
				{
					str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(value) , GameManager.info.skillEffectSetupData[type].upText);
				}
			}
			else
			{
				if(hasCompareValue)
				{
					if(value >= compareValue)
					{
						str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(-value) + checkUpDownString( PLUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+"[-]", GameManager.info.skillEffectSetupData[type].downText);
					}
					else
					{
						str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(-value) + checkUpDownString( MINUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+"[-]", GameManager.info.skillEffectSetupData[type].downText);
					}
				}
				else // 비교값이 없을때.
				{
					str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(-value) , GameManager.info.skillEffectSetupData[type].downText);
				}



			}
		}
		else
		{
			switch(type)
			{
			case 12:
			case 13:
			case 24:
			case 31:

				if(hasCompareValue)
				{
					if(value >= compareValue)
					{
						str +=  GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value))  + checkUpDownString(PLUS_PREFIX ,  UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+ "[-]");
					}
					else
					{
						str +=  GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value))  + checkUpDownString(MINUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+ "[-]");
					}
				}
				else
				{
					str +=  GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value)) );
				}

				break;
			default:





				if(hasCompareValue)
				{
					if(value >= compareValue)
					{
						str +=  GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue((value))  + checkUpDownString( PLUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+ "[-]");
					}
					else
					{
						str +=  GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue((value))  + checkUpDownString( MINUS_PREFIX , UIPopupSkillPreview.getStatStringValue(Mathf.Abs(value - compareValue)) , ")")+ "[-]");
					}
				}
				else
				{
					str += GameManager.info.skillEffectSetupData[type].getEffectText( UIPopupSkillPreview.getStatStringValue(value) );
				}



				break;
			}
		}

		return str;
	}


	string checkUpDownString(string prefix, string str, string suffix)
	{
		if(str != "0" && str != "0.0")
		{
			return prefix + str + suffix;
		}
		else
		{
			return string.Empty;
		}
	}


}


