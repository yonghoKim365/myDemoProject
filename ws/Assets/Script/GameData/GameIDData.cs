using System;
using UnityEngine;
using System.Collections.Generic;

public class GameIDData
{
	public const int MAX_TRANSCEND_LEVEL = 10;
	public const int MAX_P_LEVEL = 99;

	public bool isNew = false;

	// 강화레벨 붙은 아이디.
	public string id = null;

	// 강화레벨, exp 모두 빠진 아이디.
	public string resourceId = null;

	// 서버에서 받은 아이디.
	public string serverId = null;

	public int exp = 0;

	public Xint reinforceLevel = -1;

	public int[] transcendLevel = new int[]{0,0,0,0};

	public TranscendData transcendData
	{
		get
		{
			TranscendData td = null;

			GameManager.info.transcendData.TryGetValue(baseId, out td);

			return td;
		}
	}

	public bool canReforge()
	{
		return (totalPLevel < 99 && rare == RareType.SS);
	}

	// 제련 레벨.
	public int totalPLevel = 0;

	public bool hasPLevel = false;


	public int level = 1;


	public enum Type
	{
		Skill, Unit, Equip, None
	}

	public static GameIDData dummyUnit = new GameIDData();
	public static GameIDData dummySkill = new GameIDData();


	const string N = "_N_";
	const string R = "_R_";
	const string S = "_S_";
	const string L = "_L_";

	const string N_END = "_N";
	const string R_END = "_R";
	const string S_END = "_S";
	const string L_END = "_L";


	public Type type;

	public void parse(string checkId)
	{
		if(checkId == null) return;
		parse (checkId, getItemTypeById(checkId));
	}

	public void parse(string checkId, Type t)
	{
		if(checkId == null) return;
		type = t;

		transcendLevel[0] = 0; transcendLevel[1] = 0; transcendLevel[2] = 0; transcendLevel[3] = 0;
		hasPLevel = false;

		if(checkId.EndsWith(N_END))
		{
			checkId = checkId.Replace(N_END,"_10");
		}
		else if(checkId.EndsWith(R_END))
		{
			checkId = checkId.Replace(R_END,"_10");
		}
		else if(checkId.EndsWith(S_END))
		{
			checkId = checkId.Replace(S_END,"_10");
		}
		else if(checkId.EndsWith(L_END))
		{
			checkId = checkId.Replace(L_END,"_10");
		}
		else if(checkId.Contains(N))
		{
			checkId = checkId.Replace(N,"_10_");
		}
		else if(checkId.Contains(R))
		{
			checkId = checkId.Replace(R,"_10_");
		}
		else if(checkId.Contains(S))
		{
			checkId = checkId.Replace(S,"_10_");
		}
		else if(checkId.Contains(L))
		{
			checkId = checkId.Replace(L,"_10_");
		}

		if(type == Type.Unit)
		{
			if(checkId.Length <= 4)
			{
				checkId += "00001_0";
			}
		}


		serverId = checkId;

		string[] attr = serverId.Split('_');

		int len = attr.Length;
		int rl = 1;


		switch(type)
		{
		case Type.Equip:

			//LEO_HD2_1 기본형. LEO_HD2_1_1 강화레벨 LEO_HD2_1_15_0 exp 추가.

			resourceId = attr[0]+"_"+attr[1]+"_"+attr[2];

			if(len > 4) // exp 있음.
			{
				int.TryParse(attr[4],out exp);

				int.TryParse(attr[3],out rl);
				reinforceLevel = rl;

				id = resourceId + "_" + attr[3];
			}
			else if(len == 4)
			{
				int.TryParse(attr[3],out rl);
				reinforceLevel = rl;

				id = resourceId + "_" + attr[3];
				serverId = id + "_0";
				exp = 0;
			}
			else // exp 붙여야함.
			{
				reinforceLevel = 1;
				id = resourceId + "_0";
				serverId = id + "_0";
				exp = 0;
			}

			break;
		case Type.Skill:

			//SK_A016 , SK_A016_0, SK_A016_15_1
			//attr[0] = SK  attr[1] = A016 attr[2] = inforcelevel, attr[3] = exp

			resourceId = attr[0]+"_"+attr[1];
			id = resourceId + "_" + attr[2];
			int.TryParse(attr[2],out rl);
			reinforceLevel = rl;

			if(len > 3)
			{
				int.TryParse(attr[3],out exp);
			}
			else
			{
				serverId = id + "_0";
				exp = 0;
			}

			break;
		case Type.Unit:


			resourceId = attr[0];
			id = attr[0];

			int.TryParse(id.Substring(id.Length-5,3), out rl);
			reinforceLevel = rl;

			if(len > 1)
			{
				int.TryParse(attr[1],out exp);
			}
			else
			{
				serverId = serverId + "_0";
				exp = 0;
			}

			break;
		}

		level = reinforceLevel;

		int transcendLevellInfoIndex = attr.Length-1;

		totalPLevel = 0;

		if(attr[transcendLevellInfoIndex].Length == 8)
		{
			hasPLevel = true;

			for(int i = 0; i < 4; ++i)
			{
				int.TryParse( attr[transcendLevellInfoIndex].Substring(i*2,2), out transcendLevel[i]);
				totalPLevel += transcendLevel[i];
			}
		}
		else
		{
			hasPLevel = false;
		}

		_baseIdWithoutRare = null;
	}




	public string itemName
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return partsData.name;
			case Type.Skill:
				return skillData.name;
			case Type.Unit:
				return unitData.name;
			}
			
			return string.Empty;
		}
	}



	public string colorName
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return UICardFrame.getRareColorName(rare, partsData.name);
			case Type.Skill:
				return UICardFrame.getRareColorName(rare, skillData.name);
			case Type.Unit:
				return UICardFrame.getRareColorName(rare, unitData.name);
			}
			return string.Empty;
		}
	}



	
	public void showInfoPopup(bool isMyInfo = true, bool isJackPotMode = true)
	{
		switch(type)
		{
		case Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show(this, RuneInfoPopup.Type.PreviewOnly, true);

			break;
		case Type.Skill:
			GameManager.me.uiManager.popupSkillPreview.show(this, RuneInfoPopup.Type.PreviewOnly, true);
			GameManager.me.uiManager.popupSkillPreview.isJackpotMode = isJackPotMode;
			if(isMyInfo == false) GameManager.me.uiManager.popupSkillPreview.lbSkillMp.text = "-";
			break;

		case Type.Unit:
			GameManager.me.uiManager.popupSummonDetail.show(this, RuneInfoPopup.Type.PreviewOnly, true);
			if(isMyInfo == false) GameManager.me.uiManager.popupSummonDetail.lbUseSp.text = "-";
			break;
		}		
	}





	public int rare
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return partsData.grade-1;
			case Type.Skill:
				return skillData.grade-1;
			case Type.Unit:
				return unitData.grade-1;
			}

			return 0;
		}
	}

	public const int MAX_LEVEL = 20;

	public int maxLevel = 20;
//	{
//		get
//		{
//			switch(type)
//			{
//			case Type.Equip:
//				return partsData.baseLevel + 25;
//			case Type.Skill:
//				return skillData.baseLevel + 25;
//			case Type.Unit:
//				return unitData.baseLevel + 25;
//			}
//
//			return level;
//		}
//	}


	public int baseLevel
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return partsData.baseLevel;
			case Type.Skill:
				return skillData.baseLevel;
			case Type.Unit:
				return unitData.baseLevel;
			}
			return level;
		}
	}


	public string baseId
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return partsData.baseId;
				break;
			case Type.Skill:
				return skillData.baseId;
			case Type.Unit:
				return unitData.baseUnitId;
			}
			
			return null;
		}
	}

	/*
	public TranscendData transcendData
	{
		get
		{
			TranscendData td = null;

			if(GameManager.info != null)
			{
				switch(type)
				{
				case Type.Equip:
					//if(GameManager.info.equipTranscendData != null) GameManager.info.equipTranscendData.TryGetValue(baseId, out td);
					break;
				case Type.Skill:
					//if(GameManager.info.skillTranscendData != null) GameManager.info.skillTranscendData.TryGetValue(baseId, out td);
					break;
				case Type.Unit:
					//if(GameManager.info.unitTranscendData != null) GameManager.info.unitTranscendData.TryGetValue(baseId, out td);
					break;
				}
			}

			return td;
		}
	}
*/


	/*
	public static int convertReinforceLevelToPlusLevel(int inputRare, int inputReinforceLevel)
	{
		int returnValue = 0;
		
		switch(inputRare)
		{
		case RareType.D:
			returnValue = inputReinforceLevel;
			break;
		case RareType.C:
			returnValue = inputReinforceLevel - 5;
			break;
		case RareType.B:
			returnValue =  inputReinforceLevel - 10;
			break;
		case RareType.A:
		//case RareType.S:
			returnValue = inputReinforceLevel - 15;
			if(returnValue > 5) returnValue = 5;
			break;
		}

		return returnValue;
	}
	*/

	/*
	public static int calcReinforcePlusLevel(int inputRare, int inputReinforceLevel)
	{
		int returnValue = 0;
		
		switch(inputRare)
		{
		case RareType.D:
			returnValue = inputReinforceLevel;
			break;
		case RareType.C:
			returnValue = inputReinforceLevel - 5;
			break;
		case RareType.B:
			returnValue =  inputReinforceLevel - 10;
			break;
		case RareType.A:
		//case RareType.S:
			returnValue = inputReinforceLevel - 15;
			if(returnValue > 5) returnValue = 5;
			break;
		}
		
		return returnValue;

	}
	*/

	/*
	// 표시 강화 레벨.
	public int getReinforcePlusLevel
	{
		get
		{
			int returnValue = 0;

			switch(rare)
			{
			case RareType.D:
				returnValue = reinforceLevel;
				break;
			case RareType.C:
				returnValue = reinforceLevel - 5;
				break;
			case RareType.B:
				returnValue =  reinforceLevel - 10;
				break;
			case RareType.A:
			//case RareType.S:
				returnValue = reinforceLevel - 15;
				if(returnValue > 5) returnValue = 5;
				break;
			}

			return returnValue;
		}
	}
	*/

	public int grade
	{
		get
		{
			switch(type)
			{
			case Type.Equip:
				return partsData.grade;
				break;
			case Type.Skill:
				return skillData.grade;
			case Type.Unit:
				return unitData.grade;
			}
			
			return 0;

		}
	}


	public string getTooltipDescription()
	{
		switch(type)
		{
		case Type.Equip:
			return "["+RareType.WORD[rare]+"등급] " + "Lv." + level + " " + partsData.name;

		case Type.Skill:
			return "["+RareType.WORD[rare]+"등급] " + "Lv." + level + " " + skillData.name;

		case Type.Unit:
			return "["+RareType.WORD[rare]+"등급] " + "Lv." + level + " " + unitData.name;
		}

		return string.Empty;
	}


	const string IMG_GRADE_01 = "common_grade_01";
	const string IMG_GRADE_02 = "common_grade_02";
	const string IMG_GRADE_03 = "common_grade_03";
	const string IMG_GRADE_04 = "common_grade_04";

	public string getGradeSprite()
	{
		switch(grade)
		{
		case 1:
			return IMG_GRADE_01;
			break;
		case 2:
			return IMG_GRADE_02;
			break;
		case 3:
			return IMG_GRADE_03;
			break;
		case 4:
			return IMG_GRADE_04;
			break;
		}

		return "";
	}


	public float getReinforceProgressPercent()
	{
//		Debug.LogError("Check");

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return 0;
#endif

		switch(type)
		{
		case Type.Equip:
			return (float)exp /(float)GameDataManager.instance.equipReinforceData[partsData.character + RareType.SERVER_CHARACTER[rare] + reinforceLevel].nx;
		case Type.Skill:
			return (float)exp /(float)GameDataManager.instance.skillReinforceData[RareType.SERVER_CHARACTER[rare] + reinforceLevel].nx;
		case Type.Unit:
			return (float)exp /(float)GameDataManager.instance.unitReinforceData[RareType.SERVER_CHARACTER[rare] + reinforceLevel].nx;
		}

		return 0;
	}


	public int getSellPrice()
	{
		switch(type)
		{
		case Type.Equip:
			return GameDataManager.instance.equipReinforceData[partsData.character + RareType.SERVER_CHARACTER[rare] + reinforceLevel].sp;
		case Type.Skill:
			return GameDataManager.instance.skillReinforceData[RareType.SERVER_CHARACTER[rare] + reinforceLevel].sp;
		case Type.Unit:
			return GameDataManager.instance.unitReinforceData[RareType.SERVER_CHARACTER[rare] + reinforceLevel].sp;
		}
		
		return 0;
	}



	public P_Reinforce getReinforceData(int inputRareLevel = -1)
	{
		string rareChar = RareType.SERVER_CHARACTER[rare];

		if(inputRareLevel < 0)
		{
			inputRareLevel = reinforceLevel;
		}

		switch(type)
		{
		case Type.Equip:
			return GameDataManager.instance.equipReinforceData[partsData.character + rareChar + inputRareLevel];
			break;
		case Type.Skill:
			return GameDataManager.instance.skillReinforceData[rareChar + inputRareLevel];
		case Type.Unit:
			return GameDataManager.instance.unitReinforceData[rareChar + inputRareLevel ];
		}

		return null;
	}

	


	public static int getInforceLevelFromUnitId(string inputUnitId)
	{
		int result = 0;
		int.TryParse(inputUnitId.Substring(inputUnitId.Length-5,3), out result);
		return result;
	}






	public string getHeroPartsIcon()
	{
		//Debug.LogError(serverId + "  " + resourceId);

		return GameManager.info.heroPartsDic[resourceId].getIcon();
	}


	public HeroPartsData partsData 
	{ 
		get 
		{ 
//			Debug.Log(resourceId);
			return GameManager.info.heroPartsDic[resourceId]; 
		} 
	}

	public string getSkillIcon()
	{
#if UNITY_EDITOR
		try
		{
			return GameManager.info.skillIconData[GameManager.info.heroSkillData[resourceId].resource].icon;
		}
		catch
		{
			Debug.LogError(GameManager.info.heroSkillData[resourceId].resource);

			return string.Empty;
		}

#else
		return GameManager.info.skillIconData[GameManager.info.heroSkillData[resourceId].resource].icon;
#endif

	}

	public HeroSkillData skillData 
	{ 
		get 
		{ 
#if UNITY_EDITOR

			try
			{
				HeroSkillData test = GameManager.info.heroSkillData[resourceId];
			}
			catch
			{
				Debug.Log(serverId + "  " +  resourceId);
			}
#endif
			return GameManager.info.heroSkillData[resourceId]; 
		} 
	}

	public string getUnitIcon()
	{
		return GameManager.info.monsterData[GameManager.info.unitData[resourceId].resource].resource;
	}

	public void setUnitIcon(UISprite targetSprite, int defaultDepth)
	{
#if UNITY_EDITOR

		try
		{
			MonsterData.setUnitIcon( GameManager.info.monsterData[GameManager.info.unitData[resourceId].resource], targetSprite, defaultDepth );
		}
		catch
		{
			Debug.Log(resourceId);
		}

#else
		MonsterData.setUnitIcon( GameManager.info.monsterData[GameManager.info.unitData[resourceId].resource], targetSprite, defaultDepth );
#endif

	
	}


	public UnitData unitData 
	{ 
		get 
		{ 
//			Debug.Log(resourceId);

			return GameManager.info.unitData[resourceId]; 
		} 
	}

	//=========================== 장비 정렬 ==========================//


	public static int sortForHeroPartsList(GameIDData x, GameIDData y)
	{
		//		1> 선택된 히어로의 아이템 (선택된히어로 > 레오 > 카일리 > 클로이 > 루크)			
		//		2> 장착부위 (모자>의상>무기>타는펫)			
		int i = HeroPartsData.sortValueByPartsCharacter(x.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(y.partsData.character));
		
		if (i == 0) i = HeroPartsData.sortValueByPartsType(x.partsData.type).CompareTo(HeroPartsData.sortValueByPartsType(y.partsData.type));
		
		if (i == 0) i = y.rare.CompareTo(x.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)	
		
		if (i == 0) i = y.reinforceLevel.CompareTo(x.reinforceLevel); //		4> 강화레벨		

		return i;
	}



	//히어로		장착 위치		레어도	
	public static int sortEquipDataByHeroFromHigh(GameIDData x, GameIDData y)
	{
		int i = HeroPartsData.sortValueByPartsCharacter(y.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(x.partsData.character));
		if (i == 0) i = HeroPartsData.sortValueByPartsType(y.partsData.type).CompareTo(HeroPartsData.sortValueByPartsType(x.partsData.type));
		if (i == 0) i = y.rare.CompareTo(x.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)

		return i;
	}
	
	public static int sortEquipDataByHeroFromLow(GameIDData x, GameIDData y)
	{
		int i = HeroPartsData.sortValueByPartsCharacter(x.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(y.partsData.character));
		if (i == 0) i = HeroPartsData.sortValueByPartsType(x.partsData.type).CompareTo(HeroPartsData.sortValueByPartsType(y.partsData.type));
		if (i == 0) i = x.rare.CompareTo(y.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)

		return i;
	}


	//레어도		레벨		히어로	

	public static int sortEquipDataByRareFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.rare.CompareTo(x.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)
		if (i == 0) i = y.level.CompareTo(x.level);
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(y.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(x.partsData.character));

		return i;
	}
	
	public static int sortEquipDataByRareFromLow(GameIDData x, GameIDData y)
	{
		int i = x.rare.CompareTo(y.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)
		if (i == 0) i = x.level.CompareTo(y.level);
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(x.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(y.partsData.character));

		return i;
	}


	//레벨		레어도		히어로	
	public static int sortEquipDataByLevelFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.level.CompareTo(x.level);
		if (i == 0) i = y.rare.CompareTo(x.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(y.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(x.partsData.character));

		return i;

	}
	
	public static int sortEquipDataByLevelFromLow(GameIDData x, GameIDData y)
	{
		int i = x.level.CompareTo(y.level);
		if (i == 0) i = x.rare.CompareTo(y.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)\
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(x.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(y.partsData.character));

		return i;
	}



	//장착위치		히어로		레어도	
	public static int sortEquipDataByTypeFromHigh(GameIDData x, GameIDData y)
	{
		int i = HeroPartsData.sortValueByPartsType(y.partsData.type).CompareTo(HeroPartsData.sortValueByPartsType(x.partsData.type));
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(y.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(x.partsData.character));
		if (i == 0) i = y.rare.CompareTo(x.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)\
		return i;
	}
	
	public static int sortEquipDataByTypeFromLow(GameIDData x, GameIDData y)
	{
		int i = HeroPartsData.sortValueByPartsType(x.partsData.type).CompareTo(HeroPartsData.sortValueByPartsType(y.partsData.type));
		if (i == 0) i = HeroPartsData.sortValueByPartsCharacter(x.partsData.character).CompareTo(HeroPartsData.sortValueByPartsCharacter(y.partsData.character));
		if (i == 0) i = x.rare.CompareTo(y.rare); //		3> 레어도 (레전드>슈퍼레어>레어>노말)\
		return i;
	}



	//=========================== 유닛 정렬 ==========================//


	//레어도	이름	레벨
	public static int sortUnitDataByRareFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		if (i == 0) i = y.unitData.name.CompareTo(x.unitData.name);
		return i;
	}
	
	public static int sortUnitDataByRareFromLow(GameIDData x, GameIDData y)
	{
		int i = x.rare.CompareTo(y.rare);	
		if (i == 0) i = x.level.CompareTo(y.level);
		if (i == 0) i = x.unitData.name.CompareTo(y.unitData.name);
		return i;
	}
	
	//레벨	레어도	이름
	
	public static int sortUnitDataByLevelFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.level.CompareTo(x.level);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.unitData.name.CompareTo(x.unitData.name);
		return i;
	}
	
	public static int sortUnitDataByLevelFromLow(GameIDData x, GameIDData y)
	{
		int i = x.level.CompareTo(y.level);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.unitData.name.CompareTo(y.unitData.name);
		return i;

	}
	
	//이름	레어도	레벨
	
	public static int sortUnitDataByNameFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.unitData.name.CompareTo(x.unitData.name);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		return i;
	}
	
	public static int sortUnitDataByNameFromLow(GameIDData x, GameIDData y)
	{
		int i = x.unitData.name.CompareTo(y.unitData.name);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;

	}
	
	
	//소모SP	레어도	레벨
	
	public static int sortUnitDataByUseSpFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.unitData.sp.CompareTo(x.unitData.sp);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		return i;
	}
	
	public static int sortUnitDataByUseSpFromLow(GameIDData x, GameIDData y)
	{
		int i = x.unitData.sp.CompareTo(y.unitData.sp);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;

	}
	
	//공격력	레어도	레벨
	
	public static int sortUnitDataByAtkFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.unitData.atk.CompareTo(x.unitData.atk);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		return i;
	}
	
	public static int sortUnitDataByAtkFromLow(GameIDData x, GameIDData y)
	{
		int i = x.unitData.atk.CompareTo(y.unitData.atk);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
	}
	
	//방어력	레어도	레벨
	
	public static int sortUnitDataByDefFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.unitData.def.CompareTo(x.unitData.def);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		return i;
	}
	
	public static int sortUnitDataByDefFromLow(GameIDData x, GameIDData y)
	{
		int i = x.unitData.def.CompareTo(y.unitData.def);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
	}
	
	//생명력	레어도	레벨
	public static int sortUnitDataByHpFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.unitData.hp.CompareTo(x.unitData.hp);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		return i;
	}

	public static int sortUnitDataByHpFromLow(GameIDData x, GameIDData y)
	{
		int i = x.unitData.hp.CompareTo(y.unitData.hp);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
	}


//========================== 스킬 정렬 ========================//
	// 레어도		레벨 이름
	public static int sortSkillDataByRareFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);
		if (i == 0) i = y.skillData.name.CompareTo(x.skillData.name);
		return i;
		
	}
	
	public static int sortSkillDataByRareFromLow(GameIDData x, GameIDData y)
	{
		int i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		if (i == 0) i = x.skillData.name.CompareTo(y.skillData.name);
		return i;
	}
	
	//레벨	레어도	이름
	
	public static int sortSkillDataByLevelFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.level.CompareTo(x.level);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.skillData.name.CompareTo(x.skillData.name);
		return i;
	}
	
	public static int sortSkillDataByLevelFromLow(GameIDData x, GameIDData y)
	{
		int i = x.level.CompareTo(y.level);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.skillData.name.CompareTo(y.skillData.name);

		return i;
	}
	
	
	
	//이름	레어도	레벨
	
	public static int sortSkillDataByNameFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.skillData.name.CompareTo(x.skillData.name);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);

		return i;
	}
	
	public static int sortSkillDataByNameFromLow(GameIDData x, GameIDData y)
	{
		int i = x.skillData.name.CompareTo(y.skillData.name);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
	}
	
	
	//소모MP	레어도	레벨
	
	public static int sortSkillDataByUseMpFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.skillData.mp.CompareTo(x.skillData.mp);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);

		return i;
		
	}
	
	public static int sortSkillDataByUseMpFromLow(GameIDData x, GameIDData y)
	{
		int i = x.skillData.mp.CompareTo(y.skillData.mp);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
		
	}
	
	
	//스킬타입	레어도	레벨
	public static int sortSkillDataBySkillTypeFromHigh(GameIDData x, GameIDData y)
	{
		int i = y.skillData.skillType.CompareTo(x.skillData.skillType);
		if (i == 0) i = y.rare.CompareTo(x.rare);
		if (i == 0) i = y.level.CompareTo(x.level);

		return i;
	}
	
	public static int sortSkillDataBySkillTypeFromLow(GameIDData x, GameIDData y)
	{
		int i = x.skillData.skillType.CompareTo(y.skillData.skillType);
		if (i == 0) i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.level.CompareTo(y.level);
		return i;
	}



	//========================== 초월재료 정렬 ========================//
	//레어등급 낮은 순 > 제련레벨 낮은 순 > 강화레벨 낮은순
	public static int sortReforgeMaterial(GameIDData x, GameIDData y)
	{
		int i = x.rare.CompareTo(y.rare);
		if (i == 0) i = x.totalPLevel.CompareTo(y.totalPLevel);
		if (i == 0) i = x.reinforceLevel.Get().CompareTo(y.reinforceLevel.Get());
		return i;
	}









	public void reinforceCalc(out int resultLevel, out float resultPer, out int price, GameIDData org, List<GameIDData> sources)
	{
		P_Reinforce orgData = getReinforceData();

		if(org.level >= MAX_LEVEL)
		{
			Debug.LogError("강화 오류!");
		}

		// 재료 아이템에서 추가 경험치를 얻는다.
		int getExp = 0;


		foreach (GameIDData src in sources) // LEO_BD1_1_0_0
		{

// 구버전.
//			for ( $level = 1; $level <= $srcLevel; $level++ )
//			{
//				$srcKey = $srcParts[0].$srcRare.$srcLevel;
//				$getExp += WSDBGameData::$GM_EQUIPMENT_REINFORCE[$srcKey]['GIVE_EXP'];
//			}

//			for ( int i = 1 ; i <= src.level; ++i )
//			{
//				getExp += UnityEngine.Mathf.CeilToInt( src.getReinforceData(i).gx );
//			}

// 신버전.          
//			$getExp += WSDBGameData::$GM_EQUIPMENT_REINFORCE[$srcKey]['GIVE_EXP'];

			if(org.rare == RareType.SS && src.rare < RareType.SS)
			{
				getExp += UnityEngine.Mathf.CeilToInt( src.getReinforceData( src.level ).gx / 2);
			}
			else
			{
				getExp += UnityEngine.Mathf.CeilToInt( src.getReinforceData( src.level ).gx );
			}
		}

		// 강화 비용 정산
		price = orgData.rp * sources.Count;

		addExpRune(org, getExp, out resultLevel, out resultPer);
	}




	public void addExpRune(GameIDData orgData, int plusExp, out int resultLevel, out float resultPer)
	{

		if ( MAX_LEVEL <= orgData.level )
		{
			Debug.LogError("오류");
		}

		int orgLevel = orgData.level;
		int orgExp = orgData.exp;

		do
		{
			P_Reinforce data = orgData.getReinforceData(orgLevel);

			if ( plusExp >= ( data.nx - orgExp ) )
			{
				orgLevel += 1;
				plusExp -= ( data.nx - orgExp );
				orgExp = 0;
			}
			else
			{
				orgExp += plusExp;
				plusExp = 0;
			}
			
		} while ( 0 < plusExp && 20 > orgLevel );


		resultLevel = orgLevel;

		resultPer = (float)orgExp / (float)getReinforceData(resultLevel).nx;
		if(resultPer > 1) resultPer = 1;

	}


	public GameIDData clone()
	{
		GameIDData gd = new GameIDData();
		gd.parse(serverId, type);
		return gd;
	}


	public static GameIDData getClone(GameIDData gd)
	{
		if(gd != null) return gd.clone();

		return  null;
	}

	public static Type getItemTypeById(string id )
	{
		switch (id.Substring(0,2))
		{
		case "EN":
			return Type.None;
		case "GO":
			return Type.None;
		case "RU":
			return Type.None;
		case "EX":
			return Type.None;

		case "LE":
		case "KI":
		case "CH":
			return Type.Equip;

		case "SK":
			return  Type.Skill;

		case "UN":
			return Type.Unit;
		}

		return Type.None;
	}


	const string DEFAULT_BOOK_VARIATION = "1";
	public string getEquipBookId(string variation = DEFAULT_BOOK_VARIATION)
	{
		Util.stringBuilder.Length = 0;

		Util.stringBuilder.Append( partsData.baseId.Substring(0,partsData.baseId.LastIndexOf("_")) );
		Util.stringBuilder.Append( partsData.baseId.Substring(partsData.baseId.Length-2,1) );
		Util.stringBuilder.Append( "_" );
		Util.stringBuilder.Append( partsData.baseId.Substring(partsData.baseId.Length-1) );
		Util.stringBuilder.Append( variation );
		Util.stringBuilder.Append( "_20" );

		return Util.stringBuilder.ToString();
	}




	public int getTranscendValueByATTR(int inputValue, int inputAttr)
	{
		TranscendData td = transcendData;

		if(td == null) return inputValue;

		return td.getValueByATTR(transcendLevel, inputValue, inputAttr);
	}
	
	
	
	
	public IFloat getTranscendValueByATTR(IFloat inputValue, int inputAttr)
	{
		TranscendData td = transcendData;
		
		if(td == null) return inputValue;
		
		return td.getValueByATTR(transcendLevel, inputValue, inputAttr);
	}


	private string _baseIdWithoutRare = null;
	public string baseIdWithoutRare
	{
		get
		{
			if(_baseIdWithoutRare == null)
			{
				//	* 제련 재료 룬 : (등급부분을 제외하고) 원본 룬과 동일한 베이스 아이디를 가진 SS등급 or S등급												
				//	예> UN613 의 제련 재료 : UN613, UN513 // SK_6110 : SK6110, SK5110 // LEO_BD6_22 : LEO_BD6_22, LEO_BD5_22												
				switch(type)
				{
				case Type.Unit:
					_baseIdWithoutRare = baseId.Substring(3);
					return _baseIdWithoutRare;

				case Type.Skill:
					_baseIdWithoutRare =  baseId.Substring(0,baseId.IndexOf("_")) + baseId.Substring(baseId.IndexOf("_")+2);//  .Replace("_","");  //;baseId.Substring( baseId.IndexOf("_")+ 1 );
					return _baseIdWithoutRare;

				case Type.Equip:

					_baseIdWithoutRare = baseId.Substring( 0, baseId.IndexOf("_") + 3 ) + baseId.Substring( baseId.LastIndexOf("_") + 2)  ;
					return _baseIdWithoutRare;
				}

				return string.Empty;
			}
			else
			{
				return _baseIdWithoutRare;
			}
		}
	}







	public int getValueByATTR(int[] transcendLevel, int inputValue, int inputAttr)
	{
		if(transcendLevel == null || totalPLevel <= 0 || transcendData == null) return inputValue;
		return transcendData.getValueByATTR(transcendLevel, inputValue, inputAttr);
	}
	
	
	public IFloat getValueByATTR(int[] transcendLevel, IFloat inputValue, int inputAttr)
	{
		if(transcendLevel == null || totalPLevel <= 0 || transcendData == null) return inputValue;
		return transcendData.getValueByATTR(transcendLevel, inputValue, inputAttr);
	}

}

