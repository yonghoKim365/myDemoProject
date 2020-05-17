using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class GameDataManager : MonoBehaviour {

	//==== 모델링 데이터 ====//
	
	public bool isCompleteLoadModel = true;
	public Queue<string> _modelLoader = new Queue<string>();
	
	
	// 게임 시작전에 프리로드할 녀석들...
	public void loadDefaultModelData()
	{
		isCompleteLoadModel = false;

//		addLoadModelData("OBJ_KILEY",null);

		//addLoadModelData("npc_kileymaster01".ToUpper(),null);

//		addLoadModelData("FX_EVENT_PROP_01",null);
//		addLoadModelData("FX_EVENT_STONE_02",null);
//		addLoadModelData("FX_EVENT_VILLAGE_BOX_01",null);
//		addLoadModelData("FX_OBJ_SKULLBARRICADE01_FIRE",null);

//		addLoadModelData("EVENT_DARKFENCE02",null);

		addLoadModelData("PC_LEO",null,null);
		addLoadModelData("PC_KILEY",null,null);
		addLoadModelData("PC_CHLOE",null,null);
		addLoadModelData("PC_SCARLETT",null,null);

		addLoadModelData("PET_FENRIR",null,null);
		addLoadModelData("PET_DRAGON",null,null);
		addLoadModelData("PET_GRIFFON",null,null);
		addLoadModelData("PET_PEGASUS",null,null);

//		addLoadMapData("mapshader");

		bool loadDefaultModel = true;
#if UNITY_EDITOR

		if(CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker)
		{
			loadDefaultModel = false;
		}
#endif

		if(loadDefaultModel)
		{
//			for(int i = 1; i <= 3; ++i)
//			{
//				addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData["UN"+i].resource].resource, null);
//			}
			
			for(int i = 1; i <= 2; ++i)
			{
				addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData["EU"+i].resource]);
			}
		}



		loadUnitSlotModelData();
		startModelLoad(true);
		startMapLoad(true);


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
#endif

		int checkActIndex = GameDataManager.instance.maxActWithCheckingMaxAct - 1;
		if(checkActIndex >= 0)
		{
			GameDataManager.instance.loadLobbyMap(UILobby.LOBBY_MAP_NAMES[checkActIndex]);
		}
	}
	
	
	// 펫 슬롯에 있는 애들을 프리 로드한다.
	public void loadUnitSlotModelData(bool startLoad = false)
	{
		for(i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
		{
			if(unitSlots[i].isOpen)
			{
				addLoadModelData(GameManager.info.monsterData[unitSlots[i].unitData.resource]);
			}
		}


		foreach(KeyValuePair<string, PlayerUnitSlot[]> kv in playerUnitSlots)
		{
			for(i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
			{
				if(kv.Value[i].isOpen)
				{
					addLoadModelData(GameManager.info.monsterData[kv.Value[i].unitData.resource]);
				}
			}
		}


		if(startLoad) startModelLoad();
	}

	public void addLoadModelData(MonsterData md)
	{
		addLoadModelData(md.resource, null, md);
	}
	
	public void addLoadModelData(string str, MonsterData createPreloadMonster, MonsterData effectMonsterData)
	{
		if(createPreloadMonster != null)
		{
			GameManager.me.characterManager.addPreloadingData(createPreloadMonster.id);
		}

		str = str.ToLower();

		GameManager.me.prevLoadingMonsterResource.Add(str);

//		Debug.Log(str);

		if((GameManager.me.characterManager.monsterResource.ContainsKey(str) == false || GameManager.me.characterManager.monsterResource[str] == null) && _modelLoader.Contains(str) == false)
		{
			GameDataManager.progress = 0;
			isCompleteLoadModel = false;
			_modelLoader.Enqueue(str);
		}

		if(effectMonsterData != null)
		{
			loadMonsterEffectModelData(effectMonsterData);
		}
	}

	void loadMonsterEffectModelData(MonsterData monsterData)
	{
		if(monsterData.deleteMotionType == ChracterDeleteMotionType.EFFECT)
		{
			EffectData deleteMotionEffect = GameManager.info.effectData[monsterData.deleteMotionValue];
			
			if(deleteMotionEffect.type == EffectData.ResourceType.CHARACTER)
			{
				addLoadModelData( GameManager.info.monsterData[deleteMotionEffect.resource].resource, null, null);
			}
		}			
	}

	
	string getModelName(string name)
	{
		return name + "," + name + "," + name + "_line" + "," + name + "_linethick" ;
	}

	public static float progress = 1;
	private int _totalModelNum = 0;
	int _leftNum = 0;

	public void startModelLoad(bool veryFirst = false)
	{
		_leftNum =  _modelLoader.Count;

#if UNITY_EDITOR
//		Debug.Log("startModelLoad : " + _leftNum);
#endif
//		GameManager.me.setGuiLog(" startModelLoad : " + _leftNum);

		if(veryFirst) _totalModelNum = _leftNum;

		if(_leftNum> 0)
		{
			progress = 1.0f - (float)_leftNum / (float)_totalModelNum;

			string name = _modelLoader.Dequeue();

//			GameManager.me.setGuiLog(" name : " + name);

			switch(name)
			{
			case "pc_leo": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PLAYER, name, onCompleteLoadMonsterModel, GameManager.me.transform);break;				
			case "pet_fenrir": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PET,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;		

			case "pc_kiley": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PLAYER,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;				
			case "pet_dragon": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PET,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;		

			case "pc_chloe": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PLAYER,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;				
			case "pet_griffon": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PET,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;		

			case "pc_scarlett": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PLAYER,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;				
			case "pet_pegasus": GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.PET,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;		

			case "mob_skeletonking01e":GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.EFFECT,name, onCompleteLoadMonsterModel, GameManager.me.transform);break;;
			
//			case "npc_ibram01":GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.MONSTER,"npc_ibram01,npc_ibram01,npc_ibram01_mouth,npc_ibram01_face,npc_ibram01_eye,npc_ibram01_body", onCompleteLoadMonsterModel, GameManager.me.transform);break;;								
//			case "npc_crow01":GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.MONSTER,"npc_crow01,npc_crow01,obj_darkcrystal01", onCompleteLoadMonsterModel, GameManager.me.transform);break;;								

			default:
				GameManager.me.characterManager.loadBaseCharacter(Monster.ResourceType.MONSTER, name, onCompleteLoadMonsterModel, GameManager.me.transform);break;;								
				break;
			}
		}
		else
		{
			progress = 1;
			isCompleteLoadModel = true;
//			Debug.Log("====== COMPLETE LOAD MODEL!! ========");
//			GameManager.me.setGuiLog("====== COMPLETE LOAD MODEL!! ========");

			//GameManager.me.clearMemory(); 버벅임 때문에 일단 안씀....
		}
	}





	void onCompleteLoadMonsterModel(GameObject go, Monster cha)
	{
		go.SetActive(false);
		startModelLoad();
	}	
}
