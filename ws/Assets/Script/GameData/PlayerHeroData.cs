using System;
using System.Collections.Generic;

sealed public class PlayerHeroData : BaseHeroData
{
	public const int LEVEL_MAX = 1;

	public string id = "";

	public int itemRange = 200;

	public string type;
	
	public int expMin;
	public int expMax;
	
	public int levelMax;
	
	public PlayerHeroData ()
	{
	}
	
	sealed public override void setData(List<object> list, Dictionary<string, int> k)
	{
		id = (string)list[k["ID"]];
		base.setData(list, k);
	}
	
	sealed public override void setDataToCharacter (Monster cha)
	{
		base.setDataToCharacter (cha);
		cha.stat.atkPhysic = 0;
		cha.stat.atkMagic = 0;		

	}
	
}



public class Character
{
	public const string LEO = "LEO";
	public const string KILEY = "KILEY";
	public const string LUKE = "LUKE";
	public const string CHLOE = "CHLOE";
	public const string SCARLETT = "SCARLETT";

	public const string DRAGON = "DRAGON";
	public const string FENRIR = "FENRIR";
	public const string PEGASUS = "PEGASUS";
	public const string GRIFFON = "GRIFFON";

	public const string LEO_IMG = "img_photo_empty01";
	public const string CHLOE_IMG = "img_photo_empty02";
	public const string KILEY_IMG = "img_photo_empty03";

	public const string EMPTY_IMG = "img_photo_empty";

	public enum ID
	{
		LEO, KILEY, LUKE, CHLOE, SCARLETT, DRAGON, FENRIR, PEGASUS, GRIFFON
	}

	public static ID getCharacterId(string heroId)
	{
		switch(heroId)
		{
		case LEO: return ID.LEO; break;
		case KILEY: return ID.KILEY; break;
		case LUKE: return ID.LUKE; break;
		case CHLOE: return ID.CHLOE; break;
		case SCARLETT: return ID.SCARLETT; break;		
		}
		
		return ID.LEO;
	}


	public static string getCharacterId(ID heroId)
	{
		switch(heroId)
		{
		case ID.LEO: return LEO; break;
		case ID.KILEY: return KILEY; break;
		case ID.LUKE: return LUKE; break;
		case ID.CHLOE: return CHLOE; break;
		case ID.SCARLETT: return SCARLETT; break;		
		}
		
		return LEO;
	}



	public static string getCharacterImage(ID id)
	{
		switch(id)
		{
		case ID.LEO: return LEO_IMG;
		case ID.KILEY: return KILEY_IMG;
		//case ID.LUKE: return LUKE; break;
		case ID.CHLOE: return CHLOE_IMG;
//		case ID.SCARLETT: return SCARLETT_IMG; break;		
		}

		return string.Empty;
	}

	public static string getCharacterTagIconImage(ID id)
	{
		switch(id)
		{
		case ID.LEO: return "img_hero_01";
		case ID.KILEY: return "img_hero_02";
		case ID.CHLOE: return "img_hero_03";
		case ID.LUKE: return "img_hero_04";
		}
		
		return string.Empty;
	}



	public static string getName(string heroId)
	{
		return Util.getUIText(heroId);
	}


	public static readonly string[] KILEY_ATK = new string[5]{"pc_kiley_atk1","pc_kiley_atk2","pc_kiley_atk3","pc_kiley_atk4","pc_kiley_atk5"};
	public static readonly string[] KILEY_DMG = new string[5]{"pc_kiley_dmg1","pc_kiley_dmg2","pc_kiley_dmg3","pc_kiley_dmg4","pc_kiley_dmg5"};
	public static readonly string[] KILEY_DIE = new string[3]{"pc_kiley_die1","pc_kiley_die2","pc_kiley_die3"};
	public static readonly string[] KILEY_GRN = new string[3]{"pc_kiley_grn1","pc_kiley_grn2","pc_kiley_grn3"};

	public static readonly string[] LEO_ATK = new string[5]{"pc_leo_atk1","pc_leo_atk2","pc_leo_atk3","pc_leo_atk4","pc_leo_atk5"};
	public static readonly string[] LEO_DMG = new string[5]{"pc_leo_dmg1","pc_leo_dmg2","pc_leo_dmg3","pc_leo_dmg4","pc_leo_dmg5"};
	public static readonly string[] LEO_DIE = new string[3]{"pc_leo_die1","pc_leo_die2","pc_leo_die3"};
	public static readonly string[] LEO_GRN = new string[3]{"pc_leo_grn1","pc_leo_grn2","pc_leo_grn3"};

	public static readonly string[] CHLOE_ATK = new string[5]{"pc_chloe_atk1","pc_chloe_atk2","pc_chloe_atk3","pc_chloe_atk4","pc_chloe_atk5"};
	public static readonly string[] CHLOE_DMG = new string[5]{"pc_chloe_dmg1","pc_chloe_dmg2","pc_chloe_dmg3","pc_chloe_dmg4","pc_chloe_dmg5"};
	public static readonly string[] CHLOE_DIE = new string[3]{"pc_chloe_die1","pc_chloe_die2","pc_chloe_die3"};
	public static readonly string[] CHLOE_GRN = new string[3]{"pc_chloe_grn1","pc_chloe_grn2","pc_chloe_grn3"};


}


