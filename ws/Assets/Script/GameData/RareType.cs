using System;

sealed public class RareType
{
	
	public const int D=0;
	public const int C=1;
	public const int B=2;
	public const int A=3;
	public const int S=4;
	public const int SS=5;

	public const int MAX_RARE = RareType.SS;

	public const int RUNE_BOOK_SLOT_NUM = 6; //// d,c,b,a,s,ss

	public static readonly string[] CHARACTER = new string[6]{"D","C","B","A","S","SS"};

	public static readonly string[] SERVER_CHARACTER = new string[6]{"D","C","B","A","S","X"};


	public static readonly string[] SHORT_WORD_UPPER = new string[6]{"_D","_C","_B","_A","_S","_SS"};

	public static readonly string[] WORD = new string[6]{"D","C","B","A","S","SS"};


	public RareType ()
	{
	}


	public static string getRareLineSprite(int rare)
	{
		if(rare > RareType.SS) rare = RareType.SS;

		switch(rare)
		{
		case RareType.D:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_D;
			break;
		case RareType.C:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_C;
			break;
		case RareType.B:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_B;
			break;
		case RareType.A:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_A;
			break;
		case RareType.S:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_S;
			break;
		case RareType.SS:
			return UIHeroInventorySlot.SLOT_LINE_GRADE_SS;
			break;
		}

		return "";
	}

	public static string getRareBgSprite(int rare)
	{
		if(rare > RareType.SS) rare = RareType.SS;

		switch(rare)
		{
		case RareType.D:
			return UIHeroInventorySlot.SLOT_BG_GRADE_D;
			break;
		case RareType.C:
			return UIHeroInventorySlot.SLOT_BG_GRADE_C;
			break;
		case RareType.B:
			return UIHeroInventorySlot.SLOT_BG_GRADE_B;
			break;
		case RareType.A:
			return UIHeroInventorySlot.SLOT_BG_GRADE_A;
			break;
		case RareType.S:
			return UIHeroInventorySlot.SLOT_BG_GRADE_S;
			break;
		case RareType.SS:
			return UIHeroInventorySlot.SLOT_BG_GRADE_SS;
			break;
		}

		return "";
	}

}

