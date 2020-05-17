using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {

	public string mainCharacterId = "";
	public string subCharacterId = "";

	public Player[] players = new Player[2];
	public Player[] pvpPlayers = new Player[2];


	public bool canTagPlayer = false;
	public bool canTagPVP = false;


	public bool waitingForPlayerChange = false;
	public bool waitingForPVPChange = false;

	public Xfloat playerChangeDelay = 0.0f;
	public Xfloat pvpChangeDelay = 0.0f;

	public int selectPlayerIndex  = 0;
	public int selectPVPIndex = 0;

	public int unSelectPlayerIndex  = 1;
	public int unSelectPVPIndex = 1;


	public bool clickMyPlayerChange = false;
	public bool clickPVPPlayerChange = false;


	void Awake()
	{
		players = new Player[2];
		pvpPlayers = new Player[2];
	}

	void Start()
	{
		init ();
	}



	public void initPlayerUniqueId()
	{
		if(players[0] != null) players[0].stat.uniqueId = 0;
		if(players[1] != null) players[1].stat.uniqueId = 1;

		if(pvpPlayers != null)
		{
			if(pvpPlayers[0] != null) pvpPlayers[0].stat.uniqueId = 2;
			if(pvpPlayers[1] != null) pvpPlayers[1].stat.uniqueId = 3;
		}
	}


	public void init()
	{
		// 변경 대기중인 상황인지.
		waitingForPlayerChange = false;
		waitingForPVPChange = false;

		// 변경시 이펙트 딜레이를 줄것인지.
		playerChangeDelay.Set(0.0f);
		pvpChangeDelay.Set(0.0f);

		selectPlayerIndex = 0;
		unSelectPlayerIndex = 1;

		selectPVPIndex = 0;
		unSelectPVPIndex = 1;

		// 변경 버튼을 눌렀는지.
		clickMyPlayerChange = false;
		clickPVPPlayerChange = false;

		// 주인공쪽 컨트롤러 황설화.
		GameManager.me.uiManager.uiPlay.playerControlSlots[0].gameObject.SetActive(true);
		GameManager.me.uiManager.uiPlay.playerControlSlots[1].gameObject.SetActive(false);

		for(int i = 0; i < 2; ++i)
		{
			GameManager.me.uiManager.uiPlay.playerTagSlot[i].setVisible(false);
			GameManager.me.uiManager.uiPlay.playerTagSlot[i].transform.parent.gameObject.SetActive(true);

		}

		for(int i = 0; i < 2; ++i)
		{
			GameManager.me.uiManager.uiPlay.pvpTagSlot[i].setVisible(false);
			GameManager.me.uiManager.uiPlay.pvpTagSlot[i].transform.parent.gameObject.SetActive(true);
		}
	}



	public void visibleTagSlots()
	{
		GameManager.me.uiManager.uiPlay.playerTagSlot[0].setVisible(false);

		if(DebugManager.instance.useTagMatchMode && canTagPlayer)
		{
			GameManager.me.uiManager.uiPlay.playerTagSlot[1].setVisible(true);
		}
		else
		{
			GameManager.me.uiManager.uiPlay.playerTagSlot[1].setVisible(false);
		}


		GameManager.me.uiManager.uiPlay.pvpTagSlot[0].setVisible(false);
		
		if(DebugManager.instance.useTagMatchMode && canTagPVP)
		{
			GameManager.me.uiManager.uiPlay.pvpTagSlot[1].setVisible(true);
		}
		else
		{
			GameManager.me.uiManager.uiPlay.pvpTagSlot[1].setVisible(false);
		}
	}



	public void initPlayer(Vector3 startPos)
	{
		GamePlayerData tagGpd = null;
		bool useTagMatch = false;

		if(GameManager.me.recordMode == GameManager.RecordMode.replay)
		{
			tagGpd = GameDataManager.replayAttacker2Data;
		}
		else
		{
			tagGpd = GameDataManager.instance.getSubGamePlayerData();
		}

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			tagGpd = DebugManager.instance.playerData2;
		}
#endif

		useTagMatch = (tagGpd != null);


		if(players[0] != null && players[0] != GameManager.me.player)
		{
			try
			{
				GameManager.me.characterManager.cleanMonster(players[0]);
			}
			catch
			{
				
			}
		}


		players[0] = GameManager.me.player;

#if UNITY_EDITOR
		GameManager.me.player.pet.container.name = "player main";
#endif

		GameManager.me.uiManager.uiPlay.playerTagSlot[0].playerSlotIndex = 0;
		GameManager.me.uiManager.uiPlay.playerTagSlot[1].playerSlotIndex = 1;

		selectPlayerIndex = 0;
		unSelectPlayerIndex = 1;

		players[0].updateAllGauge();

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong)
		{
			useTagMatch = false;
		}

		if(DebugManager.instance.useTagMatchMode == false)
		{
			useTagMatch = false;
		}

//		GameDataManager.instance.playData["HERO"] = players[0].playerData.id;
		mainCharacterId = players[0].playerData.id;

		if(useTagMatch == false )
		{
			enableTagSlot(true,-1, false, true);
			GameManager.me.uiManager.uiPlay.playerTagSlot[0].setVisible(false);
			GameManager.me.uiManager.uiPlay.playerTagSlot[1].setVisible(false);
			canTagPlayer = false;
			return;
		}
		else
		{
			canTagPlayer = true;
		}

//		if(DebugManager.instance.useDebug)
//		{
//			players[unSelectPlayerIndex] = GameManager.me.mapManager.addPlayerToStage(true,  DebugManager.instance.playerData2  , startPos, false, 1);
//		}
//		else

		if(players[unSelectPVPIndex] != null)
		{
			try
			{
				GameManager.me.characterManager.cleanMonster(players[unSelectPVPIndex]);
			}
			catch
			{
				
			}
		}

		players[unSelectPlayerIndex] = GameManager.me.mapManager.addPlayerToStage(true,  tagGpd, startPos, false, 1);
	
		if(players[unSelectPlayerIndex] != null && players[unSelectPlayerIndex].playerData != null)
		{
//			GameDataManager.instance.playSubData["HERO"] = players[unSelectPlayerIndex].playerData.id;
			subCharacterId = players[unSelectPlayerIndex].playerData.id;
		}

		#if UNITY_EDITOR
		players[unSelectPlayerIndex].pet.container.name = "player unselect " + unSelectPlayerIndex;
#endif

		initUnselectPlayer(players[unSelectPlayerIndex]);
		enableTagSlot(true,1, true, true);

		players[unSelectPlayerIndex].updateAllGauge();

		GameManager.me.player.initChargingGauge();
	}


	public void initPVPPlayer(Vector3 startPos)
	{
		bool useTagMatch = (DebugManager.instance.pvpPlayerData2 != null);

		if(pvpPlayers[0] != null && pvpPlayers[0] != GameManager.me.pvpPlayer)
		{
			try
			{
				GameManager.me.characterManager.cleanMonster(pvpPlayers[0]);
			}
			catch
			{

			}
		}

		pvpPlayers[0] = GameManager.me.pvpPlayer;

#if UNITY_EDITOR
		pvpPlayers[0].pet.container.name = "pvp player main";
#endif

		GameManager.me.uiManager.uiPlay.pvpTagSlot[0].playerSlotIndex = 0;
		GameManager.me.uiManager.uiPlay.pvpTagSlot[1].playerSlotIndex = 1;

		selectPVPIndex = 0;
		unSelectPVPIndex = 1;

		pvpPlayers[0].updateAllGauge();

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong)
		{
			useTagMatch = false;
		}

		if(DebugManager.instance.useTagMatchMode == false)
		{
			useTagMatch = false;
		}

		if(useTagMatch == false)
		{
			enableTagSlot(false,-1, false, true);
			GameManager.me.uiManager.uiPlay.pvpTagSlot[0].gameObject.SetActive(false);
			GameManager.me.uiManager.uiPlay.pvpTagSlot[1].gameObject.SetActive(false);
			canTagPVP = false;
			return;
		}

		canTagPVP = true;

		if(pvpPlayers[unSelectPVPIndex] != null)
		{
			try
			{
				GameManager.me.characterManager.cleanMonster(pvpPlayers[unSelectPVPIndex]);
			}
			catch
			{

			}
		}

		pvpPlayers[unSelectPVPIndex] = GameManager.me.mapManager.addPlayerToStage(false,DebugManager.instance.pvpPlayerData2, startPos, false, 1);

#if UNITY_EDITOR
		pvpPlayers[unSelectPlayerIndex].pet.container.name = "pvpPlayers unselect " + unSelectPlayerIndex;
#endif

		initUnselectPlayer(pvpPlayers[unSelectPVPIndex]);
		enableTagSlot(false,1, true, true);

		pvpPlayers[unSelectPVPIndex].updateAllGauge();

		GameManager.me.pvpPlayer.initChargingGauge();
	}


	void initUnselectPlayer(Player p)
	{
		p.setVisible(false);
		p.miniMapPointer.visible = false;
		p.miniMapPointer.canInitVisibleAtStartTime = false;

#if UNITY_EDITOR
		p.miniMapPointer.name = "FUCKYOU " + p.isPlayerSide;
#endif

		p.sp = 0;
		p.mp = 0;
	}



	void enableTagSlot(bool isPlayerSide, int index, bool updateStat, bool isVeryFirstInit)
	{
		if(DebugManager.instance.useTagMatchMode == false) index = -1;

		if(isPlayerSide)
		{
			for(int i = 0; i < 2; ++i)
			{
				GameManager.me.uiManager.uiPlay.playerTagSlot[i].init(isPlayerSide, index, i == index, isVeryFirstInit);
			}

			if(updateStat && index >= 0)
			{
				GameManager.me.uiManager.uiPlay.playerTagSlot[index].updateAllGauge(players[index]);
				GameManager.me.uiManager.uiPlay.playerTagSlot[index].spPlayerIcon.spriteName = Character.getCharacterTagIconImage(players[index].playerData.characterId);
			}
		}
		else
		{
			for(int i = 0; i < 2; ++i)
			{
				GameManager.me.uiManager.uiPlay.pvpTagSlot[i].init(isPlayerSide, index, i == index, isVeryFirstInit);
			}

			if(updateStat && index >= 0)
			{
				GameManager.me.uiManager.uiPlay.pvpTagSlot[index].updateAllGauge( pvpPlayers[index] );
				GameManager.me.uiManager.uiPlay.pvpTagSlot[index].spPlayerIcon.spriteName = Character.getCharacterTagIconImage(pvpPlayers[index].playerData.characterId);
			}
		}
	}


	public bool hasAlivePlayer(bool isPlayerSide)
	{
		if(DebugManager.instance.useTagMatchMode == false)
		{
			return false;
		}
		
		if(isPlayerSide)
		{
			if(canTagPlayer == false || unSelectPlayerIndex < 0) return false;

			return (players[unSelectPlayerIndex] != null && players[unSelectPlayerIndex].hp > 0);
		}
		else
		{
			if(canTagPVP == false || unSelectPVPIndex < 0) return false;

			return (pvpPlayers[unSelectPVPIndex] != null && pvpPlayers[unSelectPVPIndex].hp > 0);
		}
	}


	public Player getWatingPlayer(bool isPlayerSide)
	{
		if(isPlayerSide)
		{
			return players[unSelectPlayerIndex];
		}
		else
		{
			return pvpPlayers[unSelectPVPIndex];
		}
	}



	public UIPlayTagSlot getWatingTagSlot(bool isPlayerSide)
	{
		if(isPlayerSide)
		{
			return GameManager.me.uiManager.uiPlay.playerTagSlot[unSelectPlayerIndex];
		}
		else
		{
			return GameManager.me.uiManager.uiPlay.pvpTagSlot[unSelectPVPIndex];
		}
	}




	public bool startRelay(bool isPlayerSide, int tagIndex)
	{
		if(DebugManager.instance.useTagMatchMode == false)
		{
			return false;
		}

		bool result = false;

		if(isPlayerSide)
		{
			if(players[unSelectPlayerIndex].hp > 0)
			{
				result = true;
				GameManager.me.battleManager.clickMyPlayerChange = true;
				GameManager.me.uiManager.uiPlay.playerTagSlot[unSelectPlayerIndex].state = UIPlayTagSlot.State.Ready;
			}

			GameManager.me.uiManager.uiPlay.playerTagSlot[selectPlayerIndex].setDead();
		}
		else
		{
			if(pvpPlayers[unSelectPVPIndex].hp > 0)
			{
				result = true;
				GameManager.me.battleManager.clickPVPPlayerChange = true;
				GameManager.me.uiManager.uiPlay.pvpTagSlot[unSelectPVPIndex].state = UIPlayTagSlot.State.Ready;
			}

			GameManager.me.uiManager.uiPlay.pvpTagSlot[selectPVPIndex].setDead();
		}


		if(GameManager.me.recordMode == GameManager.RecordMode.record)
		{
			if(GameManager.me.playMode == GameManager.PlayMode.replay)
			{
				GameManager.setTimeScale = GameManager.me.uiManager.uiPlay.replaySpeed;
			}
			else if(GameManager.me.uiManager.uiPlay.btnFastPlay.gameObject.activeInHierarchy && GameManager.me.isFastPlay)
			{
				GameManager.setTimeScale = UIPlay.FAST_PLAY_SPEED;
			}
			else
			{
				GameManager.setTimeScale = 1.0f;
			}
		}


		return result;
	}



	private IEnumerator hidePlayerChangeControllPanelEffect()
	{
		GameManager.me.uiManager.uiPlay.playerChangeControlPanelEffect.alpha = 0;

		float alpha = 0;

		while(alpha < 0.9f)
		{
			alpha += 0.1f;
			GameManager.me.uiManager.uiPlay.playerChangeControlPanelEffect.alpha = alpha;
			yield return new WaitForSeconds(0.02f);
		}

		while(alpha > 0)
		{
			alpha -= 0.1f;
			GameManager.me.uiManager.uiPlay.playerChangeControlPanelEffect.alpha = alpha;
			yield return new WaitForSeconds(0.02f);
		}

		GameManager.me.uiManager.uiPlay.playerChangeControlPanelEffect.gameObject.SetActive(false);
	}


	public void startChangePlayer(bool isPlayerSide)
	{
		if(DebugManager.instance.useTagMatchMode == false) return;

		CharacterManager cm = GameManager.me.characterManager;
		List<Monster> myMonList;
		List<Monster> enemyMonList;

		if(isPlayerSide)
		{
			if(waitingForPlayerChange)
			{
				return;
			}

			waitingForPlayerChange = true;
			GameManager.me.player.setChange(false);

			myMonList = cm.playerMonster;
			enemyMonList = cm.monsters;

			GameManager.me.uiManager.uiPlay.playerChangeControlPanelEffect.gameObject.SetActive(true);

			StartCoroutine(hidePlayerChangeControllPanelEffect());

			//			playerChangeDelay.Set(0.5f);

		}
		else
		{
			if(waitingForPVPChange) return;

			waitingForPVPChange = true;
			GameManager.me.pvpPlayer.setChange(false);

			myMonList = cm.monsters;
			enemyMonList = cm.playerMonster;

			//			pvpChangeDelay.Set(0.5f);
		}


		foreach(Monster mon in myMonList)
		{
			if(mon.isPlayer == false && mon.isEnabled)
			{
				mon.dead();
			}
		}


		// 교체시 아군 주인공의 위치 기준으로만 넉백이 되는 문제 수정.
		//if(VersionData.checkCodeVersion(10))
		{
			if(isPlayerSide)
			{
				foreach(Monster mon in enemyMonList)
				{
					if( mon.isEnabled)
					{
						if( Xfloat.lessThan(  VectorUtil.Distance(GameManager.me.player.cTransformPosition, mon.cTransformPosition) , GameManager.info.setupData.tagKnuckBackValue[0].Get() ))
						{
							mon.characterEffect.addKnockBack(GameManager.info.setupData.tagKnuckBackValue[1]);

							if(mon.isPlayer == false) mon.characterEffect.addStun(GameManager.info.setupData.tagStunTime);
						}
					}
				}
			}
			else
			{
				foreach(Monster mon in enemyMonList)
				{
					if( mon.isEnabled)
					{
						if( Xfloat.lessThan(  VectorUtil.Distance(GameManager.me.pvpPlayer.cTransformPosition, mon.cTransformPosition) , GameManager.info.setupData.tagKnuckBackValue[0].Get() ))
						{
							mon.characterEffect.addKnockBack(GameManager.info.setupData.tagKnuckBackValue[1]);

							if(mon.isPlayer == false) mon.characterEffect.addStun(GameManager.info.setupData.tagStunTime);
						}
					}
				}
			}
		}
		/*
		else
		{
			foreach(Monster mon in enemyMonList)
			{
				if( mon.isEnabled)
				{
					if( Xfloat.lessThan(  VectorUtil.Distance(GameManager.me.player.cTransformPosition, mon.cTransformPosition) , GameManager.info.setupData.tagKnuckBackValue[0].Get() ))
					{
						mon.characterEffect.addKnockBack(GameManager.info.setupData.tagKnuckBackValue[1]);
					}
				}
			}
		}
		*/

	}


	void changeSelectIndex(bool isPlayerSide)
	{
		if(isPlayerSide)
		{
			if(selectPlayerIndex == 0)
			{
				selectPlayerIndex = 1;
				unSelectPlayerIndex = 0;
			}
			else
			{
				selectPlayerIndex = 0;
				unSelectPlayerIndex = 1;
			}
		}
		else
		{
			if(selectPVPIndex == 0)
			{
				selectPVPIndex = 1;
				unSelectPVPIndex = 0;
			}
			else
			{
				selectPVPIndex = 0;
				unSelectPVPIndex = 1;
			}
		}

	}


	void changePlayer(bool isPlayerSide)
	{
		if(DebugManager.instance.useTagMatchMode == false) return;

		CharacterManager cm = GameManager.me.characterManager;
		List<Monster> monsters;
		Player p;

		Vector3 newPos;

		changeSelectIndex(isPlayerSide);

		if(isPlayerSide)
		{
			players[unSelectPlayerIndex].setVisible(false);
			players[unSelectPlayerIndex].miniMapPointer.visible = false;

			GameManager.me.player = players[selectPlayerIndex];
			GameManager.me.player.setPositionCtransform(players[unSelectPlayerIndex].cTransformPosition);

			enableTagSlot(true,unSelectPlayerIndex, true, false);

			monsters = GameManager.me.characterManager.playerMonster;

			p = GameManager.me.player;

			// 슬롯도 변경.
			GameManager.me.uiManager.uiPlay.playerControlSlots[selectPlayerIndex].gameObject.SetActive(true);
			GameManager.me.uiManager.uiPlay.playerControlSlots[unSelectPlayerIndex].gameObject.SetActive(false);

			// 카메라 타켓도 바뀐 애로 변경.
			GameManager.me.uiManager.uiPlay.cameraTarget = GameManager.me.player.cTransform;

		}
		else
		{
			pvpPlayers[unSelectPVPIndex].setVisible(false);
			pvpPlayers[unSelectPVPIndex].miniMapPointer.visible = false;

			GameManager.me.pvpPlayer = pvpPlayers[selectPVPIndex];
			GameManager.me.pvpPlayer.setPositionCtransform(pvpPlayers[unSelectPVPIndex].cTransformPosition);

			enableTagSlot(false,unSelectPVPIndex, true, false);

			monsters = GameManager.me.characterManager.monsters;

			p = GameManager.me.pvpPlayer;

			BaseSkillData.enemyHero = GameManager.me.pvpPlayer;
		}

		p.setChange(true);
		p.updateAllGauge();


		// 기존 플레이어는 지우고.
		for(int i = monsters.Count - 1; i >= 0; --i)
		{
			if(monsters[i].isPlayer)
			{
				monsters.RemoveAt(i);
			}
		}

		// 새로 활성화된 플레이어를 삽입.
		monsters.Add( p );
	}



	public bool checkMyPlayerDead()
	{
		if(DebugManager.instance.useTagMatchMode)
		{
			if(players[0] != null && players[0].hp > 0)
			{
				return false;
			}
			else if(canTagPlayer && players[1] != null && players[1].hp > 0)
			{
				return false;
			}
		}

		return true;
	}



	public bool checkWinWhenTimeOver()
	{
		IFloat pHp = players[0].hp;
		IFloat eHp = pvpPlayers[0].hp;

		if(DebugManager.instance.useTagMatchMode)
		{
			if(canTagPlayer)
			{
				pHp += players[1].hp;
			}

			if(canTagPVP)
			{
				eHp += pvpPlayers[1].hp;
			}
		}

		return (pHp > eHp);
	}



	public bool checkPVPPlayerDead()
	{
		if(DebugManager.instance.useTagMatchMode)
		{
			if(pvpPlayers[0] != null && pvpPlayers[0].hp > 0)
			{
				return false;
			}
			else if(canTagPVP && pvpPlayers[1] != null && pvpPlayers[1].hp > 0)
			{
				return false;
			}
		}
		
		return true;		
	}


	public Dictionary<string, int> getPlayerResultState()
	{
		Dictionary<string, int> result = new Dictionary<string, int>();

		if(players[0].hp > 0)
		{
			result.Add( Character.getCharacterId(players[0].playerData.characterId), WSDefine.YES);
		}
		else
		{
			result.Add( Character.getCharacterId(players[0].playerData.characterId), WSDefine.NO);
		}

		if(canTagPlayer)
		{
			if(players[1].hp > 0)
			{
				result.Add( Character.getCharacterId(players[1].playerData.characterId), WSDefine.YES);
			}
			else
			{
				result.Add( Character.getCharacterId(players[1].playerData.characterId), WSDefine.NO);
			}
		}

		return result;
	}

	public Dictionary<string, int> getPVPPlayerResultState()
	{
		Dictionary<string, int> result = new Dictionary<string, int>();

		if(pvpPlayers[0].hp > 0)
		{
			result.Add( Character.getCharacterId(pvpPlayers[0].playerData.characterId), WSDefine.YES);
		}
		else
		{
			result.Add( Character.getCharacterId(pvpPlayers[0].playerData.characterId), WSDefine.NO);
		}
		
		if(canTagPVP)
		{
			if(pvpPlayers[1].hp > 0)
			{
				result.Add( Character.getCharacterId(pvpPlayers[1].playerData.characterId), WSDefine.YES);
			}
			else
			{
				result.Add( Character.getCharacterId(pvpPlayers[1].playerData.characterId), WSDefine.NO);
			}
		}

		return result;
	}





	public void update()
	{
		if(DebugManager.instance.useTagMatchMode == false) return;

		if(canTagPlayer)
		{
			if(waitingForPlayerChange)
			{
				//			if(playerChangeDelay.Get() > 0)
				//			{
				//				playerChangeDelay.Set( playerChangeDelay.Get() - GameManager.globalDeltaTime);
				//			}
				//			else
				{
					changePlayer(true);
					waitingForPlayerChange = false;
				}
			}
			else
			{
				if(players[unSelectPlayerIndex] != null && players[unSelectPlayerIndex].hp > 0)
				{
					players[unSelectPlayerIndex].recoveryWaitPlayer();

					GameManager.me.uiManager.uiPlay.playerTagSlot[unSelectPlayerIndex].update();
				}
			}
			
//			for(int i = 0; i < 2; ++i)
//			{
//
//			}


			
			if(clickMyPlayerChange)
			{
				startChangePlayer(true);
				clickMyPlayerChange = false;
			}
		}



		if(canTagPVP)
		{
			if(waitingForPVPChange)
			{
				//			if(pvpChangeDelay.Get() > 0)
				//			{
				//				pvpChangeDelay.Set( pvpChangeDelay.Get() - GameManager.globalDeltaTime);
				//			}
				//			else
				{
					changePlayer(false);
					waitingForPVPChange = false;
				}
			}
			else
			{
				if(pvpPlayers[unSelectPVPIndex] != null && pvpPlayers[unSelectPVPIndex].hp > 0)
				{
					pvpPlayers[unSelectPVPIndex].recoveryWaitPlayer();

					GameManager.me.uiManager.uiPlay.pvpTagSlot[unSelectPVPIndex].update();
				}
			}
			
			
			
//			for(int i = 0; i < 2; ++i)
//			{
//
//			}
			
			
			if(clickPVPPlayerChange)
			{
				startChangePlayer(false);
				clickPVPPlayerChange = false;
			}

		}

	}



	public bool removePlayerFromSlots(Player p, bool isPlayerSide, bool clearEffect = true)
	{
		if(p == null) return false;

		Player[] playerArr;

		if(isPlayerSide)
		{
			playerArr = players;
		}
		else
		{
			playerArr = pvpPlayers;
		}


		if(playerArr == null) return false;
		
		for(int i = 0; i < playerArr.Length ; ++i)
		{
			if(playerArr[i] == null) continue;
			
			if(playerArr[i] == p)
			{
				try
				{
					playerArr[i].clearPlayerEffect();
				}
				catch
				{

				}
			}

			return true;
		}

		return false;
	}


	public void clearStage()
	{
		for(int i = 0; i < 2; ++i)
		{
			if(players[i] != null)
			{
				if(GameManager.me.player != players[i])
				{
					GameManager.me.characterManager.cleanMonster(players[i]);
				}
				else
				{

				}
			}

			players[i] = null;
		}


		for(int i = 0; i < 2; ++i)
		{
			if(pvpPlayers[i] != null)
			{
				if(GameManager.me.pvpPlayer != pvpPlayers[i])
				{
					GameManager.me.characterManager.cleanMonster(pvpPlayers[i]);
				}
				else
				{
					
				}
			}
			
			pvpPlayers[i] = null;
		}
	}

	
	void OnDestroy()
	{
		players = null;
		pvpPlayers = null;
	}



}
