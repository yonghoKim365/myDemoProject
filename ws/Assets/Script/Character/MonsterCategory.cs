using System;

sealed public class MonsterCategory
{
	public MonsterCategory ()
	{
	}

	public const string HEROMONSTER = "HERO";
	public const string PLAYER = "PLAYER";
	public const string NPC = "NPC";
	public const string PET = "PET";
	public const string EFFECT = "EFF";
	public const string OBJECT = "OBJ";
	public const string UNIT = "UNIT";
	public const string CHASER = "CHASER";
	public const string PROTECT = "PROTECT";
	public const string CHALLENGE_RUN_BOSS = "CRB";
	public const string CHALLENGE_SURVIVAL_BOSS = "CSB";
	public const string CHALLENGE_HUNT_BOSS = "CHB";
	public const string B_TEST_BOSS = "BB";
	public const string MONSTER_DEAD_ZONE = "MDZ";


	public enum Category
	{
		HEROMONSTER,
		PLAYER,
		NPC,
		PET,
		EFFECT,
		OBJECT,
		UNIT,
		CHASER,
		PROTECT,
		CHALLENGE_RUN_BOSS,
		CHALLENGE_SURVIVAL_BOSS,
		CHALLENGE_HUNT_BOSS,
		B_TEST_BOSS,
		MONSTER_DEAD_ZONE
	}

	public static Category getCategory(string str)
	{
		if(str != null)
		{
			switch(str)
			{
			case HEROMONSTER:		return Category.HEROMONSTER; break; 
			case PLAYER:			return Category.PLAYER; break; 
			case NPC:			return Category.NPC; break; 
			case PET:			return Category.PET; break; 
			case EFFECT:			return Category.EFFECT; break; 
			case OBJECT:			return Category.OBJECT; break; 
			case UNIT:			return Category.UNIT; break; 
			case CHASER:			return Category.CHASER; break; 
			case PROTECT:			return Category.PROTECT; break; 
			case CHALLENGE_RUN_BOSS:	return Category.CHALLENGE_RUN_BOSS; break; 
			case CHALLENGE_SURVIVAL_BOSS:   return Category.CHALLENGE_SURVIVAL_BOSS; break; 
			case CHALLENGE_HUNT_BOSS:	return Category.CHALLENGE_HUNT_BOSS; break; 
			case B_TEST_BOSS:		return Category.B_TEST_BOSS; break; 
			case MONSTER_DEAD_ZONE:		return Category.MONSTER_DEAD_ZONE; break; 
			}

		}

		return Category.UNIT;
	}

	
}

