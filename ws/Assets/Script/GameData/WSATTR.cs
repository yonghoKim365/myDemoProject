using System;
using System.Collections.Generic;
using UnityEngine;

public class WSATTR 
{
	public const int HPMAX_I = 0;
	public const int MPMAX_I = 1;
	public const int MP_RECOVERY_I = 2;
	public const int DEF_MAGIC_I = 3;
	public const int SKILL_SP_DISCOUNT_I = 4;
	public const int SKILL_ATK_UP_I = 5;
	public const int SKILL_UP_I = 6;
	public const int SKILL_TIME_UP_I = 7;
	public const int SPMAX_I = 8;
	public const int SP_RECOVERY_I = 9;
	public const int DEF_PHYSIC_I = 10;
	public const int SUMMON_SP_PER_I = 11;
	public const int UNIT_HP_UP_I = 12;
	public const int UNIT_DEF_UP_I = 13;
	public const int ATK_PHYSIC_I = 14;
	public const int ATK_MAGIC_I = 15;
	public const int ATK_RANGE_I = 16;
	public const int ATK_SPEED_I = 17;
	public const int ATTR1_I = 18;
	public const int ATTR2_I = 19;
	public const int ATTR3_I = 20;
	public const int ATTR4_I = 21;
	public const int ATTR5_I = 22;
	public const int ATTR6_I = 23;
	public const int ATTR7_I = 24;
	public const int SPEED_I = 25;
	public const int COOLTIME_I = 26;
	public const int MP_I = 27;
	public const int E_ATTR1_I = 28;
	public const int E_ATTR2_I = 29;
	public const int E_ATTR3_I = 30;
	public const int E_ATTR4_I = 31;
	public const int E_ATTR5_I = 32;
	public const int E_ATTR6_I = 33;
	public const int E_ATTR7_I = 34;
	public const int T_ATTR1_I = 35;
	public const int T_ATTR2_I = 36;
	public const int SUCCESS_CHANCE_1_I = 37;
	public const int E1_ATTR1_I = 38;
	public const int E1_ATTR2_I = 39;
	public const int SUCCESS_CHANCE_2_I = 40;
	public const int E2_ATTR1_I = 41;
	public const int E2_ATTR2_I = 42;
	public const int SUCCESS_CHANCE_3_I = 43;
	public const int E3_ATTR1_I = 44;
	public const int E3_ATTR2_I = 45;
	public const int SUCCESS_CHANCE_4_I = 46;
	public const int E4_ATTR1_I = 47;
	public const int E4_ATTR2_I = 48;
	public const int ATK_ATTR1_I = 49;
	public const int ATK_ATTR2_I = 50;
	public const int ATK_ATTR3_I = 51;
	public const int ATK_ATTR4_I = 52;
	public const int ATK_ATTR5_I = 53;
	public const int ATK_ATTR6_I = 54;
	public const int ATK_ATTR7_I = 55;
	public const int MOVE_SPEED_I = 56;
	public const int HP_I = 57;




	public const string HPMAX = "HPMAX";
	public const string MPMAX = "MPMAX";
	public const string MP_RECOVERY = "MP_RECOVERY";
	public const string DEF_MAGIC = "DEF_MAGIC";
	public const string SKILL_SP_DISCOUNT = "SKILL_SP_DISCOUNT";
	public const string SKILL_ATK_UP = "1-2_SKILL_ATK_UP";
	public const string SKILL_UP = "3_SKILL_UP";
	public const string SKILL_TIME_UP = "4-9_SKILL_TIME_UP";
	public const string SPMAX = "SPMAX";
	public const string SP_RECOVERY = "SP_RECOVERY";
	public const string DEF_PHYSIC = "DEF_PHYSIC";
	public const string SUMMON_SP_PER = "SUMMON_SP_PER";
	public const string UNIT_HP_UP = "UNIT_HP_UP";
	public const string UNIT_DEF_UP = "UNIT_DEF_UP";
	public const string ATK_PHYSIC = "ATK_PHYSIC";
	public const string ATK_MAGIC = "ATK_MAGIC";
	public const string ATK_RANGE = "ATK_RANGE";
	public const string ATK_SPEED = "ATK_SPEED";
	public const string ATTR1 = "ATTR1";
	public const string ATTR2 = "ATTR2";
	public const string ATTR3 = "ATTR3";
	public const string ATTR4 = "ATTR4";
	public const string ATTR5 = "ATTR5";
	public const string ATTR6 = "ATTR6";
	public const string ATTR7 = "ATTR7";
	public const string SPEED = "SPEED";
	public const string COOLTIME = "COOLTIME";
	public const string MP = "MP";
	public const string E_ATTR1 = "E_ATTR1";
	public const string E_ATTR2 = "E_ATTR2";
	public const string E_ATTR3 = "E_ATTR3";
	public const string E_ATTR4 = "E_ATTR4";
	public const string E_ATTR5 = "E_ATTR5";
	public const string E_ATTR6 = "E_ATTR6";
	public const string E_ATTR7 = "E_ATTR7";
	public const string T_ATTR1 = "T_ATTR1";
	public const string T_ATTR2 = "T_ATTR2";
	public const string SUCCESS_CHANCE_1 = "SUCCESS_CHANCE_1";
	public const string E1_ATTR1 = "E1_ATTR1";
	public const string E1_ATTR2 = "E1_ATTR2";
	public const string SUCCESS_CHANCE_2 = "SUCCESS_CHANCE_2";
	public const string E2_ATTR1 = "E2_ATTR1";
	public const string E2_ATTR2 = "E2_ATTR2";
	public const string SUCCESS_CHANCE_3 = "SUCCESS_CHANCE_3";
	public const string E3_ATTR1 = "E3_ATTR1";
	public const string E3_ATTR2 = "E3_ATTR2";
	public const string SUCCESS_CHANCE_4 = "SUCCESS_CHANCE_4";
	public const string E4_ATTR1 = "E4_ATTR1";
	public const string E4_ATTR2 = "E4_ATTR2";
	public const string ATK_ATTR1 = "ATK_ATTR1";
	public const string ATK_ATTR2 = "ATK_ATTR2";
	public const string ATK_ATTR3 = "ATK_ATTR3";
	public const string ATK_ATTR4 = "ATK_ATTR4";
	public const string ATK_ATTR5 = "ATK_ATTR5";
	public const string ATK_ATTR6 = "ATK_ATTR6";
	public const string ATK_ATTR7 = "ATK_ATTR7";
	public const string MOVE_SPEED = "MOVE_SPEED";
	public const string HP = "HP";



	public static int getAttrIndexByAttrName(string inputCode)
	{


		switch(inputCode)
		{
		case HPMAX:
				return HPMAX_I;
		case MPMAX:
			return MPMAX_I;
		case MP_RECOVERY:
			return MP_RECOVERY_I;
		case DEF_MAGIC:
			return DEF_MAGIC_I;
		case SKILL_SP_DISCOUNT:
			return SKILL_SP_DISCOUNT_I;
		case SKILL_ATK_UP:
			return SKILL_ATK_UP_I;
		case SKILL_UP:
			return SKILL_UP_I;
		case SKILL_TIME_UP:
			return SKILL_TIME_UP_I;
		case SPMAX:
			return SPMAX_I;
		case SP_RECOVERY:
			return SP_RECOVERY_I;
		case DEF_PHYSIC:
			return DEF_PHYSIC_I;
		case SUMMON_SP_PER:
			return SUMMON_SP_PER_I;
		case UNIT_HP_UP:
			return UNIT_HP_UP_I;
		case UNIT_DEF_UP:
			return UNIT_DEF_UP_I;
		case ATK_PHYSIC:
			return ATK_PHYSIC_I;
		case ATK_MAGIC:
			return ATK_MAGIC_I;
		case ATK_RANGE:
			return ATK_RANGE_I;
		case ATK_SPEED:
			return ATK_SPEED_I;
		case ATTR1:
			return ATTR1_I;
		case ATTR2:
			return ATTR2_I;
		case ATTR3:
			return ATTR3_I;
		case ATTR4:
			return ATTR4_I;
		case ATTR5:
			return ATTR5_I;
		case ATTR6:
			return ATTR6_I;
		case ATTR7:
			return ATTR7_I;
		case SPEED:
			return SPEED_I;
		case COOLTIME:
			return COOLTIME_I;
		case MP:
			return MP_I;
		case E_ATTR1:
			return E_ATTR1_I;
		case E_ATTR2:
			return E_ATTR2_I;
		case E_ATTR3:
			return E_ATTR3_I;
		case E_ATTR4:
			return E_ATTR4_I;
		case E_ATTR5:
			return E_ATTR5_I;
		case E_ATTR6:
			return E_ATTR6_I;
		case E_ATTR7:
			return E_ATTR7_I;
		case T_ATTR1:
			return T_ATTR1_I;
		case T_ATTR2:
			return T_ATTR2_I;
		case SUCCESS_CHANCE_1:
			return SUCCESS_CHANCE_1_I;
		case E1_ATTR1:
			return E1_ATTR1_I;
		case E1_ATTR2:
			return E1_ATTR2_I;
		case SUCCESS_CHANCE_2:
			return SUCCESS_CHANCE_2_I;
		case E2_ATTR1:
			return E2_ATTR1_I;
		case E2_ATTR2:
			return E2_ATTR2_I;
		case SUCCESS_CHANCE_3:
			return SUCCESS_CHANCE_3_I;
		case E3_ATTR1:
			return E3_ATTR1_I;
		case E3_ATTR2:
			return E3_ATTR2_I;
		case SUCCESS_CHANCE_4:
			return SUCCESS_CHANCE_4_I;
		case E4_ATTR1:
			return E4_ATTR1_I;
		case E4_ATTR2:
			return E4_ATTR2_I;
		case ATK_ATTR1:
			return ATK_ATTR1_I;
		case ATK_ATTR2:
			return ATK_ATTR2_I;
		case ATK_ATTR3:
			return ATK_ATTR3_I;
		case ATK_ATTR4:
			return ATK_ATTR4_I;
		case ATK_ATTR5:
			return ATK_ATTR5_I;
		case ATK_ATTR6:
			return ATK_ATTR6_I;
		case ATK_ATTR7:
			return ATK_ATTR7_I;
		case MOVE_SPEED:
			return MOVE_SPEED_I;
		case HP:
			return HP_I;
		default:
			return -1;
		}

	}

}