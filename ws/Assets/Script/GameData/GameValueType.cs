using System;
using UnityEngine;

public class GameValueType
{
	public enum Type
	{
		Single, Fixed//, Section
	}


//	- AP : 물리공격력 + 마법공격력.
//	- DP : (물리방어력 + 마법방어력) / 2.
//	- HP : 생명력.
//	- LV : 강화 레벨.

	public enum ApplyStatValue
	{
		None, AP, DP, HP, LV
	}

	public GameValueType ()
	{
	}



	public static float getApplyValue(Xfloat[] attr, GameValueType.Type valueType, int applyReinforceLevel)
	{
		float v = 0;
		
		switch(valueType)
		{
		case GameValueType.Type.Fixed:
			
			// 원래 적용 레벨 수치는 0보다 크다. -값으로 된 것은 현 차징시간이 최소차징시간보다 적을때의 예외상황을 위해.
			// 만들어둔것이다.
			if(applyReinforceLevel < 0)
			{
				v = attr[0] * -(applyReinforceLevel / 0.01f);//* 0.01f);
			}
			else
			{
				v = attr[0] + (attr[1] - attr[0]) / 19f * (applyReinforceLevel - 1);
			}
			break;
		default:
			v = attr[0];
			break;
		}

		return MathUtil.Round(v * 100.0f) / 100.0f;
	}




	public static IFloat getApplyValue(Xfloat[] attr, GameValueType.Type valueType, int applyReinforceLevel, GameValueType.ApplyStatValue statValue, MonsterStat shooterStat)
	{
		float v = 0;

		switch(valueType)
		{
		case GameValueType.Type.Fixed:

			// 원래 적용 레벨 수치는 0보다 크다. -값으로 된 것은 현 차징시간이 최소차징시간보다 적을때의 예외상황을 위해.
			// 만들어둔것이다.
			if(applyReinforceLevel < 0)
			{
				v = attr[0] * -(applyReinforceLevel / 100.0f); //* 0.01f);
			}
			else
			{
				v = attr[0] + (attr[1] - attr[0]) / 19f * (applyReinforceLevel - 1);
			}
			break;
		default:
			v = attr[0];
			break;
		}




		//    → 적용되는 값 = (이 스킬을 사용하는 캐릭터의) XX 스탯 값 * N / 100
		switch(statValue)
		{
		case ApplyStatValue.AP: //   - AP : 물리공격력 + 마법공격력.

			v = (shooterStat.atkPhysic + shooterStat.atkMagic) * v * 0.01f;

			break;

		case ApplyStatValue.DP: //   - DP : (물리방어력 + 마법방어력) / 2.
			v = (shooterStat.defMagic + shooterStat.defPhysic) * 0.5f * v * 0.01f;
			break;

		case ApplyStatValue.HP: //   - HP : 생명력.
			v = (float)(shooterStat.maxHp) * v * 0.01f;
			break;
		case ApplyStatValue.LV: //   - LV : 강화 레벨.
			v = (float)(shooterStat.reinforceLevel) * v * 0.01f;
			break;

		default:

			break;
		}

		return MathUtil.Round(v * 100.0f) / 100.0f;

	}


	public static IFloat getPartsApplyValue(Xfloat[] attr, GameValueType.Type valueType, int applyReinforceLevel)
	{
		switch(valueType)
		{
		case GameValueType.Type.Fixed:

			float v = attr[0] + (attr[1] - attr[0]) / 19f * (applyReinforceLevel - 1);
			return MathUtil.Round(v * 100.0f) / 100.0f;

		default:
			return attr[0];
		}
	}

	

}

