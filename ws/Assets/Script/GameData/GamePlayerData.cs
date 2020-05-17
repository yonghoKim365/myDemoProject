using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayerData
{
	string _id;
	public string id
	{
		get
		{
			return _id;
		}
		set
		{
			if(value != null)
			{
				characterId = Character.getCharacterId(value);
			}
			_id = value;
		}
	}

	public Character.ID characterId = Character.ID.LEO;
	
	public GamePlayerData(string characterId)
	{
		id = characterId;
	}
	
	public void copyTo(GamePlayerData target, bool includeRunes = false)
	{
		target.id = id;
		target.level = level;
		target.heroData = heroData;
		target.partsHead = partsHead;
		target.partsBody = partsBody;
		target.partsWeapon = partsWeapon;
		target.partsVehicle = partsVehicle;
		target.faceTexture = faceTexture;
		target.ai = ai;

		if(includeRunes)
		{
			if(_units == null)
			{
				target.units = null;
			}
			else
			{
				string[] tempUnits = new string[_units.Length];

				for(int i = 0; i < _units.Length; ++i)
				{
					tempUnits[i] = _units[i];
				}
				target.units = tempUnits;
			}

			if(_skills == null)
			{
				target.skills = null;
			}
			else
			{
				string[] tempSkills = new string[_skills.Length];
				
				for(int i = 0; i < _skills.Length; ++i)
				{
					tempSkills[i] = _skills[i];
				}
				target.skills = tempSkills;
			}
		}
	}



	public GamePlayerData clone(bool includeRunes = true)
	{
		GamePlayerData target = new GamePlayerData(id);

		target.id = id;
		target.level = level;
		target.heroData = heroData;
		target.partsHead = partsHead;
		target.partsBody = partsBody;
		target.partsWeapon = partsWeapon;
		target.partsVehicle = partsVehicle;
		target.faceTexture = faceTexture;
		target.ai = ai;
		
		if(includeRunes)
		{
			if(_units == null)
			{
				target.units = null;
			}
			else
			{
				string[] tempUnits = new string[_units.Length];
				
				for(int i = 0; i < _units.Length; ++i)
				{
					tempUnits[i] = _units[i];
				}
				target.units = tempUnits;
			}
			
			if(_skills == null)
			{
				target.skills = null;
			}
			else
			{
				string[] tempSkills = new string[_skills.Length];
				
				for(int i = 0; i < _skills.Length; ++i)
				{
					tempSkills[i] = _skills[i];
				}
				target.skills = tempSkills;
			}
		}

		return target;
	}



	public PlayerHeroData heroData;
	
	public string faceTexture = null;
	
	public HeroPartsItem partsHead = null;
	public HeroPartsItem partsBody = null;
	public HeroPartsItem partsWeapon = null;
	public HeroPartsItem partsVehicle = null;
	
	public string[] unitResourceId = null;
	string[] _units = null;
	public string[] units
	{
		get
		{
			return _units;
		}
		set
		{
			_units = value;
			
			if(units == null) unitResourceId = null;
			else unitResourceId = new string[units.Length];
			
			if(units != null && units.Length > 0)
			{
				for(int i = 0; i < units.Length; ++i)
				{
					if( string.IsNullOrEmpty(units[i]) == false)
					{
						if(units[i].Contains("_"))
						{
							unitResourceId[i] = Util.getUnitIdWithoutEXPAndTranscendLevel(units[i]);
						}
						else
						{
							unitResourceId[i] = units[i];
						}
					}
				}
			}
		}
	}
	
	string[] _skills;
	public string[] skills
	{
		get
		{
			//			Debug.LogError("get skills");
			return _skills;
		}
		set
		{
//			if(value != null && value.Length > 0)
//			{
//				Debug.LogError("========================" + id +  " skills value : " + value[0]);
//			}
//			else
//			{
//				Debug.LogError("========================" + id +  " skills value : null " );
//			}

			_skills = value;
		}
	}
	
	
	public string ai;
	private Xint _level = new Xint(1);
	
	public int level
	{
		get
		{
			return _level.Get();
		}
		set
		{
			value = 1;

#if UNITY_EDITOR
			//Debug.LogError("id : " + id + " set Hero Level! : " + value + "    ");
#endif

			heroData = GameManager.info.playerHeroData[id][value-1];
			_level.Set(value);
		}
	}


	public string serialize()
	{
		List<string> l = new List<string>();
		l.Add(id);
		l.Add(level.ToString());
		l.Add(ai);
		for(int i = 0; i<5;++i)
		{
			if(units != null && units.Length > i && units[i] != null)
			{
				l.Add(units[i]);
			}
			else
			{
				l.Add("");
			}
		}
		
		for(int i = 0; i<3;++i)
		{
			if(skills != null && skills.Length > i && skills[i] != null)
			{
				l.Add(skills[i]);
			}
			else 
			{
				l.Add("");
			}
		}
		
		l.Add(partsHead.partsId);
		l.Add(partsBody.partsId);
		l.Add(partsWeapon.partsId);
		l.Add(partsVehicle.partsId);
		
		l.Add(GameManager.me.uiManager.popupChampionshipResult.enemyId);
		
		l.Add(GameManager.me.uiManager.popupChampionshipResult.matchNumber.ToString());
		
		l.Add(UIPlay.playerLeagueGrade.ToString());
		l.Add(UIPlay.pvpleagueGrade.ToString());
		
		return string.Join(",",l.ToArray());
	}
	
	private static List<string> _tempStringList = new List<string>();
	public static GamePlayerData deserialize(string[] d, bool isFirstPlayer)
	{
		int index = 5;
		if(isFirstPlayer == false) index = 25;

		GamePlayerData gpd = new GamePlayerData(d[index]);
		gpd.level = Convert.ToInt16(d[index+1]);
		gpd.ai = d[index+2];
		
		_tempStringList.Clear();
		for(int i = index+3; i < index+8; ++i)
		{
			if(string.IsNullOrEmpty(d[i]) == false) _tempStringList.Add(d[i]);
			else _tempStringList.Add("");
		}
		
		gpd.units = _tempStringList.ToArray();
		
		_tempStringList.Clear();
		for(int i = index+8; i < index+11; ++i)
		{
			if(string.IsNullOrEmpty(d[i]) == false) _tempStringList.Add(d[i]);
			else _tempStringList.Add("");
		}
		
		gpd.skills = _tempStringList.ToArray();
		
		gpd.partsHead = new HeroPartsItem(gpd.id,d[index+11]);
		gpd.partsBody = new HeroPartsItem(gpd.id,d[index+12]);
		gpd.partsWeapon = new HeroPartsItem(gpd.id,d[index+13]);
		gpd.partsVehicle = new HeroPartsItem(gpd.id,d[index+14]);
		
		GameManager.me.uiManager.popupChampionshipResult.enemyId = d[index+15];
		GameManager.me.uiManager.popupChampionshipResult.matchNumber = Convert.ToInt32( d[index+16] );
		
		UIPlay.playerLeagueGrade = Convert.ToInt32( d[index+17] );
		UIPlay.pvpleagueGrade = Convert.ToInt32( d[index+18] );
		
		return gpd;
	}
	
	
	
	
	public void setAllDataToPlayer(Player player)
	{
		if(heroData == null) return;
		
		heroData.setDataToCharacter(player);
		
		if(partsHead != null) partsHead.parts.setDataToCharacter(player,partsHead.itemInfo);
		if(partsBody != null) partsBody.parts.setDataToCharacter(player,partsBody.itemInfo);
		if(partsVehicle != null) partsVehicle.parts.setDataToCharacter(player,partsVehicle.itemInfo);
		
		// 공격 무기를 제일 늦게 하는 이유는 무기에서 공격 거리를 세팅하기 때문이다.
		if(partsWeapon != null) partsWeapon.parts.setDataToCharacter(player,partsWeapon.itemInfo);
		
		player.stat.atkPhysic += (player.stat.atkPhysic) * heroData.atkPhysic * 0.01f;
		player.stat.atkMagic += (player.stat.atkMagic) * heroData.atkMagic * 0.01f;
		
		player.mp = GameManager.info.setupData.defaultMp;
		
		player.stat.baseLevel = level;
		
		if(BattleSimulator.nowSimulation == false)
		{
			//switch(GameManager.me.stageManager

			if(GameManager.me.isStartGame)
			{
#if UNITY_EDITOR
				if(DebugManager.instance.useDebug)
				{
					if(player.isPlayerSide) ai = DebugManager.instance.ai;
					else ai = DebugManager.instance.pvpAi;
				}
				else
#endif
				{
					switch(GameManager.me.stageManager.nowPlayingGameType)
					{
					case GameType.Mode.Championship:
					case GameType.Mode.Friendly:
						ai = GameDataManager.getPVPAI(player.isPlayerSide);
						break;

					case GameType.Mode.Epic:
					case GameType.Mode.Hell:
						ai = "AI_AUTOPLAY";
						break;
					case GameType.Mode.Sigong:

						if(player.isPlayerSide)
						{
							if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PVP)
							{
								ai = GameDataManager.getPVPAI(true);
							}
							else
							{
								ai = "AI_AUTOPLAY";
							}
						}
						else
						{
							ai = "AI_PVP6";
						}

						break;
					}
				}
			}
		}
		
		player.setAI(ai);

//		* 유저 아군 (or 적) 히어로 : 히어로 장착파츠의 레어등급으로 레벨 구함, 										
//		▷ 장비 레벨 = D급 : 5레벨 / C급 : 15레벨 / B급 : 35레벨 / A급 : 60레벨 / S급 : 90레벨										
//		▷ 유저히어로의 대상유닛레벨 = (모자 레벨 + 강화레벨(1~20) + 의상 레벨 + 강화레벨 + 무기 레벨 + 강화레벨 + 타는펫 레벨 + 강화레벨) / 4										
				
		int equipRareLevel = 0;

		if(partsHead != null)
		{
			equipRareLevel += getEquipRareLevel(partsHead.itemInfo.rare) + partsHead.itemInfo.reinforceLevel;
		}

		if(partsBody != null)
		{
			equipRareLevel += getEquipRareLevel(partsBody.itemInfo.rare) + partsBody.itemInfo.reinforceLevel;
		}

		if(partsVehicle != null)
		{
			equipRareLevel += getEquipRareLevel(partsVehicle.itemInfo.rare) + partsVehicle.itemInfo.reinforceLevel;
		}

		if(partsWeapon != null)
		{
			equipRareLevel += getEquipRareLevel(partsWeapon.itemInfo.rare) + partsWeapon.itemInfo.reinforceLevel;
		}

		player.stat.skillTargetLevel = equipRareLevel / 4;
	}

	int getEquipRareLevel(int r)
	{
		switch(r)
		{
		case RareType.D: return 5;
		case RareType.C: return 15;
		case RareType.B: return 35;
		case RareType.A: return 60;
		case RareType.S: return 90;
		case RareType.SS: return 120;
		}

		return 5;
	}
	
	
	public void setHead(string headId)
	{
		partsHead = new HeroPartsItem(id, headId);
	}
	
	public void setBody(string bodyId)
	{
		partsBody = new HeroPartsItem(id, bodyId);
	}
	
	public void setWeapon(string weaponId)
	{
		partsWeapon = new HeroPartsItem(id, weaponId);
	}
	
	public void setVehicle(string vehicleId)
	{
		if(string.IsNullOrEmpty(vehicleId)) partsVehicle = null;
		else partsVehicle = new HeroPartsItem(id,vehicleId);
	}
	
	
	public void refreshCharacterEquip(Player player)
	{
		foreach(KeyValuePair<string, GameObject> kv in player.heads)
		{
			player.heads[kv.Key].SetActive( partsHead.parts.resource.Equals(kv.Key));
		}
		
		foreach(KeyValuePair<string, GameObject> kv in player.bodies)
		{
			player.bodies[kv.Key].SetActive( partsBody.parts.resource.Equals(kv.Key));
		}
		
		foreach(KeyValuePair<string, GameObject> kv in player.weapons)
		{
			player.weapons[kv.Key].SetActive( partsWeapon.parts.resource.Equals(kv.Key));
		}
	}
}
