using System;
using UnityEngine;

public class CutSceneCharacterData
{
	public Monster cha;
	public bool canClone = false;
	
	public string id;
	public bool isPlayerMon;
	public Monster.TYPE type;

	public string replaceTexture = null;
	
	public Monster clone()
	{
		Monster mon = null;

		if(type == Monster.TYPE.NPC && GameManager.info.npcData[id].type == NPCData.Type.PLAYER)
		{
			NPCData data = GameManager.info.npcData[id];
			GamePlayerData gpd = new GamePlayerData(data.resource);
			gpd = DebugManager.instance.setPlayerData(gpd,false, data.resource,  data.head, data.body, data.weapon, data.vehicle);
			gpd.faceTexture = data.faceTexture;
#if UNITY_EDITOR
			UnityEngine.Debug.Log( data.resource );
#endif

			Player p = (Player)GameManager.me.characterManager.getMonster(true,false,data.resource,false);
			p.init(gpd,false,false);

			if(gpd.partsVehicle != null && gpd.partsVehicle.parts != null)
			{
				p.pet = (Pet)GameManager.me.characterManager.getMonster(true,false,gpd.partsVehicle.parts.resource.ToUpper(),false);
				p.pet.init(p);
			}
			else
			{
				p.pet = null;
			}

			p.isEnabled = true;


			p.cutSceneInit();
			p.isCutSceneOnlyCharacter = true;

			if(p.pet != null)
			{
				p.pet.cutSceneInit();
				p.pet.isCutSceneOnlyCharacter = true;
			}

			p.setParent( GameManager.me.mapManager.mapStage );

			mon = (Monster)p;
		}
		else if(type == Monster.TYPE.PLAYER)
		{
			GamePlayerData gpd;

			if(GameDataManager.instance.heroes.ContainsKey(id))
			{
				gpd = GameDataManager.instance.heroes[id];
			}
			else
			{
				gpd = new GamePlayerData(id);

				switch(id)
				{
				case Character.LEO:
					gpd = DebugManager.instance.setPlayerData(gpd, false, id,  					
					                                          GameManager.info.setupData.defaultLeo2[0],
					                                          GameManager.info.setupData.defaultLeo2[1],
					                                          GameManager.info.setupData.defaultLeo2[2],
					                                          GameManager.info.setupData.defaultLeo2[3]);
					break;
				case Character.KILEY:



					gpd = DebugManager.instance.setPlayerData(gpd, false, id,  
															  GameManager.info.setupData.defaultKiley2[0],
					                                          GameManager.info.setupData.defaultKiley2[1],
					                                          GameManager.info.setupData.defaultKiley2[2],
					                                          GameManager.info.setupData.defaultKiley2[3]);

					break;
				case Character.CHLOE:
					gpd = DebugManager.instance.setPlayerData(gpd, false, id,  
					                                          GameManager.info.setupData.defaultChloe2[0],
					                                          GameManager.info.setupData.defaultChloe2[1],
					                                          GameManager.info.setupData.defaultChloe2[2],
					                                          GameManager.info.setupData.defaultChloe2[3],
					                                          null,null,"","pc_chloe_face01");
					break;
				}
			}

			Player p = (Player)GameManager.me.characterManager.getMonster(true,false,id,false);
			p.init(gpd,false,false);

			if(gpd.partsVehicle != null && gpd.partsVehicle.parts != null)
			{
				p.pet = (Pet)GameManager.me.characterManager.getMonster(true,false,gpd.partsVehicle.parts.resource.ToUpper(),false);
				p.pet.init(p);
			}
			else
			{
				p.pet = null;
			}

			p.isEnabled = true;

			p.cutSceneInit();
			p.isCutSceneOnlyCharacter = true;
			
			p.pet.cutSceneInit();
			p.pet.isCutSceneOnlyCharacter = true;

			p.setParent( GameManager.me.mapManager.mapStage );

			mon = (Monster)p;
		}

		if(mon == null)
		{
			mon = GameManager.me.cutSceneManager.addMonsterToStage(isPlayerMon, type, id);
		}

		if(mon != null && replaceTexture != null)
		{
			foreach(SkinnedMeshRenderer smr in mon.smrs)
			{
				if(smr.name.Contains("_line") == false)
				{
					CharacterUtil.setTexture(smr, replaceTexture);
				}
			}
		}

		return mon;
	}
	
}

