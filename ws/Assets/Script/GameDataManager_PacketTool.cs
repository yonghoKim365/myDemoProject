
using System.Collections.Generic;
using UnityEngine;

sealed public partial class GameDataManager : MonoBehaviour 
{
	private List<string> _tempStringList = new List<string>();

	void setChargingTime(int leftSec)
	{
		if(leftSec == 0) // 에너지 맥스임.
		{
			
		}
		else
		{
			
		}
	}
	
	void setSelectedUnit(string heroId, int slotIndex, string unitId)
	{
		if(string.IsNullOrEmpty(unitId) == false)
		{
			playerUnitSlots[heroId][slotIndex].isOpen.Set(true);
			playerUnitSlots[heroId][slotIndex].setData(unitId);	
		}
		else
		{
			playerUnitSlots[heroId][slotIndex].isOpen.Set(false);
		}
	}
	
	void setSelectedSkill(string heroId, int slotIndex, string skillId)
	{
		if(string.IsNullOrEmpty(skillId) == false)
		{
			playerSkillSlots[heroId][slotIndex].isOpen.Set(true);
			playerSkillSlots[heroId][slotIndex].setData(skillId);	
		}
		else
		{
			playerSkillSlots[heroId][slotIndex].isOpen.Set(false);
		}
	}

	// 이전 해당 아이템들의 총 갯수를 기록.
	private Dictionary<string, int> _invenPrevItemChecker = new Dictionary<string, int>();
	// 이전 해당 아이템들의 new 갯수를 기록.
	private Dictionary<string, int> _invenNewItemChecker = new Dictionary<string, int>();

	void settingNewInvenItem(List<GameIDData> targetList, GameIDData.Type invenType)
	{
		bool hasNewItem = false;

		foreach(GameIDData gd in targetList)
		{
			// 이전 목록에 있고 그 녀석이 new 였다면.
			if(_invenPrevItemChecker.ContainsKey(gd.serverId) && _invenPrevItemChecker[gd.serverId] > 0)
			{
				if(_invenNewItemChecker[gd.serverId] > 0)
				{
					gd.isNew = true;
					hasNewItem = true;
				}

				--_invenPrevItemChecker[gd.serverId];
				--_invenNewItemChecker[gd.serverId];
			}
			// 이전 목록에 없으면 무조건 new.
			else
			{
				gd.isNew = true;
				hasNewItem = true;
			}
		}

		_invenPrevItemChecker.Clear();
		_invenNewItemChecker.Clear();

		switch(invenType)
		{
		case GameIDData.Type.Equip:
			UILobby.setHasNewEquipRune(hasNewItem);
			break;
		case GameIDData.Type.Skill:
			UILobby.setHasNewSkillRune(hasNewItem);
			break;
		case GameIDData.Type.Unit:
			UILobby.setHasNewUnitRune(hasNewItem);
			break;
		}


	}

	void saveNewInvenItem(List<GameIDData> sourceList)
	{
		_invenPrevItemChecker.Clear();
		_invenNewItemChecker.Clear();

		foreach(GameIDData gd in sourceList)
		{
			// 총 갯수를 기록.
			if(_invenPrevItemChecker.ContainsKey(gd.serverId))
			{
				++_invenPrevItemChecker[gd.serverId];
			}
			else
			{
				_invenPrevItemChecker.Add(gd.serverId, 1);
			}

			// new 였던 애들을 기록.
			if(_invenNewItemChecker.ContainsKey(gd.serverId))
			{
				if(gd.isNew) ++_invenPrevItemChecker[gd.serverId];
			}
			else
			{
				if(gd.isNew) _invenNewItemChecker.Add(gd.serverId, 1);
				else _invenNewItemChecker.Add(gd.serverId, 0);
			}
		}
	}


	void parsePartsInven(string[] equipments, bool checkNewItem = false)
	{
		if(checkNewItem && equipments != null) saveNewInvenItem(partsInventoryList);

		partsInventory.Clear();
		partsInventoryList.Clear();
		
		if(equipments == null) return;
		
		int len = equipments.Length;
		for(int i = 0; i < len; ++i)
		{
			GameIDData ei = new GameIDData();
			ei.parse(equipments[i], GameIDData.Type.Equip);
			
			if(partsInventory.ContainsKey(ei.serverId) == false) partsInventory[ei.serverId] = ei;
			partsInventoryList.Add(ei);
		}

		if(checkNewItem) settingNewInvenItem(partsInventoryList, GameIDData.Type.Equip);
	}


	void parseSkillInven(string[] p, bool checkNewItem = false)
	{
		if(checkNewItem && p != null) saveNewInvenItem(skillInventoryList);

		skillInventory.Clear();
		skillInventoryList.Clear();
		if(p == null) return;
		foreach(string key in p)
		{
//			if(string.IsNullOrEmpty(key)) continue;
			GameIDData ei = new GameIDData();
			ei.parse(key, GameIDData.Type.Skill);
			if(skillInventory.ContainsKey(ei.serverId) == false) skillInventory[ei.serverId] = ei;
			skillInventoryList.Add(ei);
		}

		if(checkNewItem) settingNewInvenItem(skillInventoryList, GameIDData.Type.Skill);
	}
	
	void parseUnitInven(string[] p, bool checkNewItem = false)
	{
		if(checkNewItem && p != null) saveNewInvenItem(unitInventoryList);

		unitInventory.Clear();
		unitInventoryList.Clear();
		if(p == null) return;
		foreach(string key in p)
		{
//			if(string.IsNullOrEmpty(key)) continue;
			GameIDData ei = new GameIDData();
			ei.parse(key, GameIDData.Type.Unit);

			if(unitInventory.ContainsKey(ei.serverId) == false)
			{
				unitInventory[ei.serverId] = ei;
			}

			unitInventoryList.Add(ei);
		}

		if(checkNewItem) settingNewInvenItem(unitInventoryList, GameIDData.Type.Unit);
	}

	public Dictionary <string, P_Hero> serverHeroData;

	void parseHeroInven(Dictionary <string, P_Hero> p)
	{
		foreach(KeyValuePair<string, P_Hero> kv in p)
		{
			if(heroes.ContainsKey(kv.Key) == false)
			{
				heroes.Add(kv.Key, new GamePlayerData(kv.Key));

				if(kv.Key == Character.CHLOE)
				{
					heroes[kv.Key].faceTexture = "pc_chloe_face01";
				}
			}
			
			heroes[kv.Key].level = level;
			//kv.Value.name;
			
			heroes[kv.Key].setHead(kv.Value.selEqts[HeroParts.HEAD]);
			heroes[kv.Key].setBody(kv.Value.selEqts[HeroParts.BODY]);
			heroes[kv.Key].setWeapon(kv.Value.selEqts[HeroParts.WEAPON]);
			heroes[kv.Key].setVehicle(kv.Value.selEqts[HeroParts.VEHICLE]);

			//parseSelectUnits(kv.Key,kv.Value.selUnitRunes);
			//parseSelectSkills(kv.Key,kv.Value.selSkillRunes);

		}

		serverHeroData = p;
	}

	void parseSelectHero(P_Hero p)
	{
		if(p == null) return;
		if(heroes.ContainsKey(p.name) == false)
		{
			heroes.Add(p.name, new GamePlayerData(p.name));
		}
		
		heroes[p.name].level = level;

		heroes[p.name].setHead(p.selEqts[HeroParts.HEAD]);
		heroes[p.name].setBody(p.selEqts[HeroParts.BODY]);
		heroes[p.name].setWeapon(p.selEqts[HeroParts.WEAPON]);
		heroes[p.name].setVehicle(p.selEqts[HeroParts.VEHICLE]);

		serverHeroData[p.name] = p;

	}




	void parseSelectUnits(string heroId, Dictionary<string, string> units )
	{
		if( string.IsNullOrEmpty(heroId) ||  units == null) return;

		if(selectUnitRunes.ContainsKey(heroId) == false) selectUnitRunes.Add(heroId, null);

		selectUnitRunes[heroId] = units;

		_tempStringList.Clear();

		foreach(KeyValuePair<string, string> kv in units)
		{
			switch(kv.Key)
			{
			case UnitSlot.U1: setSelectedUnit(heroId,0,kv.Value); break;
			case UnitSlot.U2: setSelectedUnit(heroId,1,kv.Value); break;
			case UnitSlot.U3: setSelectedUnit(heroId,2,kv.Value); break;
			case UnitSlot.U4: setSelectedUnit(heroId,3,kv.Value); break;
			case UnitSlot.U5: setSelectedUnit(heroId,4,kv.Value); break;
			}

			_tempStringList.Add(kv.Value);
		}

		if(heroes.ContainsKey(heroId))
		{
			heroes[heroId].units = _tempStringList.ToArray();
		}

		_tempStringList.Clear();
	}

	
	void parseSelectSkills(string heroId, Dictionary<string, string> skills)
	{
		if(string.IsNullOrEmpty(heroId) || skills == null) return;

		if(selectSkillRunes.ContainsKey(heroId) == false) selectSkillRunes.Add(heroId, null);

		selectSkillRunes[heroId] = skills;

		_tempStringList.Clear();

		foreach(KeyValuePair<string, string> kv in skills)
		{
			switch(kv.Key)
			{
			case SkillSlot.S1: setSelectedSkill(heroId,0,kv.Value); break;
			case SkillSlot.S2: setSelectedSkill(heroId,1,kv.Value); break;
			case SkillSlot.S3: setSelectedSkill(heroId,2,kv.Value); break;
			}

			_tempStringList.Add(kv.Value);
		}

		if(heroes.ContainsKey(heroId))
		{
			heroes[heroId].skills = _tempStringList.ToArray();
		}
		
		_tempStringList.Clear();
	}





	int getUnitSlotNumBySlotId(string slotId)
	{
		switch(slotId)
		{
		case UnitSlot.U1: return 0; break;
		case UnitSlot.U2: return 1; break;
		case UnitSlot.U3: return 2; break;
		case UnitSlot.U4: return 3; break;
		case UnitSlot.U5: return 4; break;
		}
		return 0;
	}


	
	int getSkillSlotNumBySlotId(string slotId)
	{
		switch(slotId)
		{
		case SkillSlot.S1: return 0; break;
		case SkillSlot.S2: return 1; break;
		case SkillSlot.S3: return 2; break;
		}
		return 0;
	}
}