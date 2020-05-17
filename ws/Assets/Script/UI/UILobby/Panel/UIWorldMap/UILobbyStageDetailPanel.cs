using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILobbyStageDetailPanel  : UIPopupBase 
{

	Xint _selectedAct = 1;
	Xint _selectedStage = 1;
	Xint _selectedRound = 1;

	public UIWorldMapRoundMonsterInfo roundMonsterInfoSlot;
	public UIChallengeItemSlot roundRewardInfoSlot;

	public GameObject goRewardContainer;
	public GameObject goMonsterContainer;

	public UIButton btnStartRound, btnCutSceneOnOff;

	public UISprite spGameMode, spCutSceneOnOff;

	public UIWorldMapRoundPanel[] roundPanel;

	public UILabel lbRoundStartPrice, lbEnergy, lbEnergyTime, lbRoundInfo, lbStageTitle, lbDescription;

	public UISprite sbRoundType, spRoundStartPriceType, spMonsterInfoWord;


	public UIScrollView contentContainer;
	public UIPanel contentPanel;


	public UISprite spRewardBg, spMonsterBg;


	public static bool mustPlayCutScene = false;

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnStartRound.gameObject).onClick = onStartRound;
		UIEventListener.Get(btnClose.gameObject).onClick = onClose;

		UIEventListener.Get(btnCutSceneOnOff.gameObject).onClick = onClickCutSceneOnOff;

		GameDataManager.chargeTimeDispatcher -= updateNextEnergy;
		GameDataManager.chargeTimeDispatcher += updateNextEnergy;

		GameDataManager.energyDispatcher -= updatePriceForStartRound;
		GameDataManager.energyDispatcher += updatePriceForStartRound;
	}

	
	void onClose(GameObject go)
	{
		SoundData.play(closeSound);
		GameManager.me.clearMemory();
		hide();
		GameManager.me.uiManager.uiMenu.uiWorldMap.checkTutorialStart(false);
	}

	void updateNextEnergy(string timeStr)
	{
		if(gameObject.activeInHierarchy == false) return;
		lbEnergyTime.text = timeStr;
	}

	public void onStartRound(GameObject go)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			GameManager.me.startGame(0.5f);
		}
		else
#endif
		{
			if(GameDataManager.instance.energy > 0)
			{
				EpiServer.instance.sendPlayEpic(_selectedAct, _selectedStage, _selectedRound);

				if(GameDataManager.instance.roundClearStatusCheck(_selectedAct, _selectedStage, _selectedRound) == false)
				{
					PlayerPrefs.SetInt("CS"+_selectedAct+_selectedStage+_selectedRound,WSDefine.YES);
				}
			}
			else
			{
				UISystemPopup.open( UISystemPopup.PopupType.YesNo, Util.getUIText("GO_TO_ENERGYSHOP"), goToEnergyShop);
			}

		}
	}

	void goToEnergyShop()
	{
		GameManager.me.uiManager.popupShop.showEnergyShop();
	}



	void onClickCutSceneOnOff (GameObject go)
	{
		GameDataManager.instance.canCutScenePlay = !GameDataManager.instance.canCutScenePlay;
		spCutSceneOnOff.enabled = GameDataManager.instance.canCutScenePlay;
	}





	void updatePriceForStartRound()
	{
		if(GameDataManager.instance.energy <= 0)
		{
//			spRoundStartPriceType.spriteName = "img_icn_cash";
//			lbRoundStartPrice.text = GameDataManager.instance.roundInfo.playNeedRuby + "";
//			lbRoundStartPrice.text = "[b82b00]1[-]";
		}
		else
		{
//			spRoundStartPriceType.spriteName = "img_icn_energe";
//			lbRoundStartPrice.text = "[fff1c9]1[-]";
		}

		lbRoundStartPrice.text = "[fff1c9]1[-]";
		lbEnergy.text = GameDataManager.instance.energy.ToString();
	}





	public string openSound = "uicm_popup_big";
	public string closeSound = "uicm_close_big";

	public bool canCheckTutorial48_50 = false;

	bool _canUseCutSceneSkip = false;

	public void open(int act, int stage, int round, bool playAni = false, bool refreshOnly = false)
	{
		mustPlayCutScene = false;

		if(
			TutorialManager.isTutorialOpen("T44") ||
			TutorialManager.isTutorialOpen("T45") ||
			TutorialManager.isTutorialOpen("T46") ||
			TutorialManager.isTutorialOpen("T13") ||
			TutorialManager.isTutorialOpen("T15") ||
			TutorialManager.isTutorialOpen("T46") ||
			TutorialManager.isTutorialOpen("T51") || 
			TutorialManager.isTutorialOpen("T24") )
		{
			return;
		}

		if(refreshOnly == false)
		{
			if(playAni)
			{
				SoundData.play(openSound);
				popupPanel.transform.localPosition = new Vector3(0,1000,0);
				
			}
			else
			{
				popupPanel.transform.localPosition = new Vector3(0,0,0);
			}
		}

		contentContainer.transform.localPosition = Vector3.zero;
		contentPanel.clipOffset = new Vector2(0,0);

		iconTooltip.hide();

		clearSlots();

		int i;
		int len;

		_selectedAct = act;
		_selectedStage = stage;
		_selectedRound = round;

		ActData ad = GameManager.info.actData[act];
		StageData sd = GameManager.info.stageData[ad.stages[stage-1]];

		RoundData rd;

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			rd = GameManager.info.roundData[DebugManager.instance.debugRoundId];
		}
		else
#endif
		{
			rd = GameManager.info.roundData[sd.rounds[round-1]];
		}

		GameManager.me.stageManager.setNowRound(rd, GameType.Mode.Epic, act, stage, round);



		string str = "";

		lbEnergy.text = GameDataManager.instance.energy.ToString();
		lbEnergyTime.text = string.Empty;


		Util.stringBuilder.Length = 0;

		lbRoundInfo.text = "";//rd.description;
		lbRoundInfo.gameObject.SetActive(true);

		setRoundInfoText(rd);

		switch(rd.mode)
		{
		case RoundData.MODE.KILLEMALL:

			if(rd.settingTime > -1) Util.stringBuilder.Append("등장하는 모든 적 처치");
			else Util.stringBuilder.Append( "등장하는 모든 적 처치 ");
			if(rd.settingTime > -1)
			{
				Util.stringBuilder.Append( "(제한시간 "+ rd.settingTime +"초)" );
			}

			break;
		case RoundData.MODE.SURVIVAL:
			Util.stringBuilder.Append( rd.settingTime + "초 동안 살아남기");
			break;
		case RoundData.MODE.PROTECT:

			Util.stringBuilder.Append(  rd.settingTime + "초 동안 ");

			i = 0;
			foreach(StageMonsterData data in rd.protectObject)
			{
				if(i > 0) Util.stringBuilder.Append( "," );
				Util.stringBuilder.Append( GameManager.info.npcData[data.id].name );
				++i;
			}

			Util.stringBuilder.Append( " 보호" );


			//GameManager.info.npcData[rd.protectObject[
			break;
		case RoundData.MODE.SNIPING:
			string hId = rd.heroMonsters[rd.targetIndex].id;
			Util.stringBuilder.Append( "결투!  Vs " + GameManager.info.heroMonsterData[hId].name);
			break;
		case RoundData.MODE.KILLCOUNT:

			len = rd.killMonsterIds.Length;
			for(i = 0; i < len; ++i)
			{
				if(i > 0) Util.stringBuilder.Append( "," );
				Util.stringBuilder.Append( GameManager.info.unitData[rd.killMonsterIds[i]].name + " " + rd.killMonsterNum[i] + "마리");
			}

			Util.stringBuilder.Append( " 처치");
			break;
		case RoundData.MODE.KILLCOUNT2:
			Util.stringBuilder.Append( "몬스터 " +rd.killMonsterNum[0]+ "마리 처치");
			break;
		case RoundData.MODE.ARRIVE:

			Util.stringBuilder.Append( Mathf.CeilToInt((float)rd.targetPos * 0.01f) + "m 앞으로 이동하기");

			if(rd.chaser != null)
			{
				Util.stringBuilder.Append( " (추격몬스터 : " + GameManager.info.npcData[rd.chaser.id].name + ")");
			}

			if(rd.settingTime > -1)
			{
				Util.stringBuilder.Append( " (제한시간 " +  rd.settingTime + "초)");
			}


			break;
		case RoundData.MODE.DESTROY:

			//roundInfo = "[목표오브젝트] [0]개 파괴 (추격몬스터 : [추격몬스터이름]) (제한시간 000초)";

			Dictionary<string, int> objNum = new Dictionary<string, int>();

			foreach(StageMonsterData data in rd.destroyObject)
			{
				if(objNum.ContainsKey(data.id) == false)
				{
					objNum[data.id] = 1;
				}
				else
				{
					++objNum[data.id];	
				}
			}

			len = 0;


			List<StageDetailInfoObjectName> dt = new List<StageDetailInfoObjectName>();

			foreach(KeyValuePair<string, int> kv in objNum)
			{
				if(dt.Count == 0)
				{
					dt.Add(new StageDetailInfoObjectName());
					dt[0].name = GameManager.info.npcData[kv.Key].name;
					dt[0].num = kv.Value;
				}
				else
				{
					bool hasKey = false;
					foreach(StageDetailInfoObjectName tempDt in dt)
					{
						if(hasKey == false)
						{
							if(tempDt.name == GameManager.info.npcData[kv.Key].name)
							{
								tempDt.num += kv.Value;
								hasKey = true;
							}
						}
					}

					if(hasKey == false)
					{
						dt.Add(new StageDetailInfoObjectName());
						dt[dt.Count - 1].name = GameManager.info.npcData[kv.Key].name;
						dt[dt.Count - 1].num = kv.Value;
					}
				}
			}

			foreach(StageDetailInfoObjectName kv in dt)
			{
				if(len > 0) Util.stringBuilder.Append( ",");
				Util.stringBuilder.Append( kv.name + " " + kv.num + "개 ");
				++len;
			}

			dt.Clear();

			Util.stringBuilder.Append( "파괴 ");

			if(rd.chaser != null)
			{
				Util.stringBuilder.Append( " (추격몬스터 : " + GameManager.info.npcData[rd.chaser.id].name + ")");
			}

			if(rd.settingTime > -1)
			{
				Util.stringBuilder.Append( "(제한시간 " + rd.settingTime + "초)");
			}
			break;			
		case RoundData.MODE.GETITEM:

			len = 0;
			foreach(KeyValuePair<string, Xint> kv in rd.getItemData.needCount)
			{
				if(len > 0) Util.stringBuilder.Append( ",");
				Util.stringBuilder.Append( GameManager.info.mapObjectData[kv.Key].name + " "+ kv.Value + "개");
				++len;
			}

			Util.stringBuilder.Append( " 획득하기 ");

			if(rd.settingTime > -1)
			{
				Util.stringBuilder.Append( "(제한시간 "+rd.settingTime+"초)");
			}

			break;
		}

		lbRoundInfo.text = Util.stringBuilder.ToString();

		lbDescription.text = rd.description;

		lbStageTitle.text = "[ ACT"+act+" ] "+ sd.title;

		updatePriceForStartRound();

		sbRoundType.spriteName = getRoundTypeIcon(rd);

		setRoundInfoText(rd);


		GameManager.me.uiManager.uiPlay.lbEpicRoundInfo.text = lbRoundInfo.text;
		GameManager.me.uiManager.uiPlay.spEpicRoundIcon.spriteName = sbRoundType.spriteName;
		GameManager.me.uiManager.uiPlay.spEpicRoundText.spriteName = "img_text_round" + (round);

		int clearIndex = 0;

		if(act < GameDataManager.instance.maxAct)
		{
			clearIndex = 5;
		}
		else if(stage < GameDataManager.instance.maxStage)
		{
			clearIndex = 5;
		}
		else
		{
			clearIndex = GameDataManager.instance.maxRound - 1;
		}


		for(i = 0; i < 5; ++i)
		{
			if(stage <= 2)
			{
				roundPanel[i].gameObject.SetActive((i < 3));
			}
			else if(stage <= 4)
			{
				roundPanel[i].gameObject.SetActive((i < 4));
			}
			else
			{
				roundPanel[i].gameObject.SetActive(true);
			}

			if(i <= clearIndex)
			{
				if(i + 1 == round)
				{
					bool isClear = clearIndex == 5;
					if(GameDataManager.instance.maxAct == act && GameDataManager.instance.maxStage == stage && GameDataManager.instance.maxRound > i + 1)
					{
						isClear = true;
					}

					roundPanel[i].setData(UIWorldMapRoundPanel.RoundStatus.SELECT,i+1, isClear);
				}
				else if( i + 1 > clearIndex)
				{
					roundPanel[i].setData(UIWorldMapRoundPanel.RoundStatus.SELECT2,i+1, clearIndex == 5);
				}
				else
				{
					roundPanel[i].setData(UIWorldMapRoundPanel.RoundStatus.CLEAR,i+1);
				}
			}
			else
			{
				roundPanel[i].setData(UIWorldMapRoundPanel.RoundStatus.LOCK,i+1);
			}
		}




		///액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
//		if(canCheckTutorial48_50 && TutorialManager.instance.check("T48") )
//		{
//			
//		}
		///액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
//		else if(canCheckTutorial48_50 && TutorialManager.instance.check("T49") )
//		{
//			
//		}
		///액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
//		else if(canCheckTutorial48_50 && TutorialManager.instance.check("T50") )
//		{
//			
//		}
//		else 
		if(TutorialManager.instance.check("T44"))
		{

		}



		spMonsterInfoWord.gameObject.SetActive(false);

		gameObject.SetActive(true);


		// 이미 깬 스테이지거나 한번 본 녀석이면...
		if(GameDataManager.instance.roundClearStatusCheck(act, stage, round) || PlayerPrefs.GetInt("CS"+act+stage+round,WSDefine.NO) == WSDefine.YES)
		{
			btnCutSceneOnOff.isEnabled = true;
			spCutSceneOnOff.enabled = GameDataManager.instance.canCutScenePlay;
			_canUseCutSceneSkip = true;
		}
		else
		{
			btnCutSceneOnOff.isEnabled = false;
			spCutSceneOnOff.enabled = true;
			mustPlayCutScene = true;
			_canUseCutSceneSkip = false;
		}

		//_rewardSlots

		goRewardContainer.gameObject.SetActive(true);

		_v = goRewardContainer.transform.position;
		
		_v.y = lbDescription.transform.position.y - (lbDescription.printedSize.y + 50);

		goRewardContainer.transform.position = _v;


		drawRewardList(rd);

		if(_rewardSlots.Count <= 0) goRewardContainer.gameObject.SetActive(false);

		_v = goMonsterContainer.transform.position;

		_v.y = goRewardContainer.transform.position.y;

		if(_rewardSlots.Count > 0)
		{
			_v.y = _rewardSlots[_rewardSlots.Count - 1].transform.position.y - 100.0f;
		}
		else
		{
			_v.y = lbDescription.transform.position.y - (lbDescription.printedSize.y + 50);
		}

		if(_rewardSlots.Count > 0)
		{
			spRewardBg.height = Mathf.RoundToInt( Mathf.FloorToInt((_rewardSlots.Count-1) / 5) * 131 + 194); 
		}

		goMonsterContainer.transform.position = _v;

		drawMonsterSlot(rd);

		if(_monsterSlots.Count > 0)
		{
			spMonsterBg.height = Mathf.RoundToInt( Mathf.FloorToInt((_monsterSlots.Count-1) / 5) * 149 + 219);
		}


		StartCoroutine(repositionContent(playAni));

		++_playIndex;

		StartCoroutine(playRoundInfoTextAnimation(lbRoundInfo.text, _playIndex));

		canCheckTutorial48_50 = false;
	}





	void drawRewardList(RoundData rd)
	{
		if(rd.rewards == null) return;

		foreach(string str in rd.rewards)
		{
			addRewardSlot(str);
		}
	}






	void drawMonsterSlot(RoundData rd)
	{
//		if(DebugManager.instance.useDebug == false)
		{
			// 몬스터 히어로.
			if(rd.heroMonsters != null)
			{
				foreach(StageMonsterData d in rd.heroMonsters)
				{
					if(d.attr != null) continue;
					
					if(GameManager.info.heroMonsterData[d.id].isMiddleBoss)
					{
						addMonsterSlot(GameManager.info.heroMonsterData[d.id].resource,2,GameManager.info.heroMonsterData[d.id].slotName, GameManager.info.heroMonsterData[d.id].level);
					}
					else
					{
						addMonsterSlot(GameManager.info.heroMonsterData[d.id].resource,1,GameManager.info.heroMonsterData[d.id].slotName, GameManager.info.heroMonsterData[d.id].level);
					}
				}
			}
			
			// 기본배치
			if(rd.unitMonsters != null)
			{
				foreach(StageMonsterData d in rd.unitMonsters)
				{
					addMonsterSlot(GameManager.info.unitData[d.id].resource,3,GameManager.info.unitData[d.id].slotName, GameManager.info.unitData[d.id].level);
				}
			}
			
			// 소환유닛.
			if(rd.units != null)
			{
				foreach(string u in rd.units)
				{
					addMonsterSlot(GameManager.info.unitData[u].resource,3,GameManager.info.unitData[u].slotName, GameManager.info.unitData[u].level);
				}
			}
		}
	}



	public void checkTutorialAfterRetryPopup()
	{

	}



	IEnumerator repositionContent(bool playAni)
	{
		yield return null;

		_v.x = 0; _v.y = 1000; _v.z = 0;
		spMonsterInfoWord.cachedTransform.localPosition = _v;
		yield return null;

		foreach(UIWorldMapRoundMonsterInfo s in _monsterSlots)
		{
			s.resetPosition();
		}
		
		_v.x = 0; _v.y = 0; _v.z = 0;
		spMonsterInfoWord.cachedTransform.localPosition = _v;
		spMonsterInfoWord.gameObject.SetActive(true);
		
		contentContainer.ResetPosition();

		yield return Util.ws01;

		if(playAni) ani.Play();
	}


	float _roundInfoLbPosX = 0.0f;
	float _roundInfoWidth = 0.0f;

	public Transform tfRoundInfoPanelLong,tfRoundInfoPanelShort,tfRoundInfoPanelMiddle;
	public int[] roundInfoTextWidth = new int[3]{512,376,410};
	public float[] roundInfoTextPosX = new float[3]{-85f,32f,20f};

	void setRoundInfoText(RoundData rd)
	{
		if(rd.mode == RoundData.MODE.PROTECT || rd.mode == RoundData.MODE.KILLEMALL)
		{
			_roundInfoLbPosX = roundInfoTextPosX[0];
			_roundInfoWidth = roundInfoTextWidth[0];
			tfRoundInfoPanelLong.gameObject.SetActive(true);
			tfRoundInfoPanelShort.gameObject.SetActive(false);
			tfRoundInfoPanelMiddle.gameObject.SetActive(false);
			lbRoundInfo.transform.parent = tfRoundInfoPanelLong;

			GameManager.me.uiManager.uiPlay.lbEpicRoundInfo.transform.localPosition = new Vector3(-343.7f, 280.5f,0);
		}
		else if(rd.mode == RoundData.MODE.ARRIVE)
		{
			_roundInfoLbPosX = roundInfoTextPosX[1];
			_roundInfoWidth = roundInfoTextWidth[1];
			tfRoundInfoPanelLong.gameObject.SetActive(false);
			tfRoundInfoPanelShort.gameObject.SetActive(false);
			tfRoundInfoPanelMiddle.gameObject.SetActive(true);
			lbRoundInfo.transform.parent = tfRoundInfoPanelMiddle;
			
			GameManager.me.uiManager.uiPlay.lbEpicRoundInfo.transform.localPosition = new Vector3(-284.7f, 280.5f,0);
		}
		else
		{
			_roundInfoLbPosX = roundInfoTextPosX[2];
			_roundInfoWidth = roundInfoTextWidth[2];
			tfRoundInfoPanelLong.gameObject.SetActive(false);
			tfRoundInfoPanelShort.gameObject.SetActive(true);
			tfRoundInfoPanelMiddle.gameObject.SetActive(false);
			lbRoundInfo.transform.parent = tfRoundInfoPanelShort;

			GameManager.me.uiManager.uiPlay.lbEpicRoundInfo.transform.localPosition = new Vector3(-300, 280.5f,0);
		}
		
		_v.x = _roundInfoLbPosX; _v.y = 0; _v.z = 0;
		lbRoundInfo.transform.localPosition = _v;
	}

	int _playIndex = 0;

	IEnumerator playRoundInfoTextAnimation(string info, int playIndex)
	{
		yield return null;

		if(_canUseCutSceneSkip)
		{
			btnCutSceneOnOff.isEnabled = true;
			spCutSceneOnOff.enabled = GameDataManager.instance.canCutScenePlay;
		}
		else
		{
			btnCutSceneOnOff.isEnabled = false;
			spCutSceneOnOff.enabled = true;
			mustPlayCutScene = true;
		}


		_v.x = _roundInfoLbPosX; _v.y = 0; _v.z = 0;
		lbRoundInfo.transform.localPosition = _v;

		float printSize = lbRoundInfo.printedSize.x;

		yield return Util.ws1;

		if(printSize > _roundInfoWidth)
		{
			while(_playIndex == playIndex)
			{
				_v.x = lbRoundInfo.transform.localPosition.x;
				_v.x -= 4.0f; 
				_v.y = 0;
				_v.z = 0;

				if(_v.x < -(printSize - _roundInfoLbPosX - _roundInfoWidth))
				{
					yield return Util.ws1;
					if(_playIndex == playIndex) _v.x = _roundInfoLbPosX;
					if(_playIndex == playIndex)lbRoundInfo.gameObject.SetActive(false);
					yield return Util.ws02;
					if(_playIndex == playIndex)lbRoundInfo.gameObject.SetActive(true);
					if(_playIndex == playIndex)lbRoundInfo.transform.localPosition = _v;
					yield return Util.ws1;
				}
				else
				{
					yield return Util.ws005;
					if(_playIndex == playIndex)lbRoundInfo.transform.localPosition = _v;
				}
			}
		}
	}



	List<UIChallengeItemSlot> _rewardSlots = new List<UIChallengeItemSlot>();
	Stack<UIChallengeItemSlot> _rewardSlotPool = new Stack<UIChallengeItemSlot>();

	void addRewardSlot(string rewardId)
	{
		UIChallengeItemSlot s;

		if(_rewardSlotPool.Count > 0 ) s = _rewardSlotPool.Pop();
		else
		{
			s = Instantiate(roundRewardInfoSlot) as UIChallengeItemSlot;
		}
		
		_rewardSlots.Add(s);

		s.gameObject.SetActive(true);

		s.setData(rewardId);

		s.type = UIChallengeItemSlot.Type.StageRewardPreviewItem;
		s.useButton = true;

		s.transform.parent = goRewardContainer.transform;
		
		_v.x = 61  + ((_rewardSlots.Count -1) % 5) * 142;
		_v.y = -95 - ((_rewardSlots.Count-1) / 5) * 131 - 10;
		_v.z = 0;

		s.transform.localPosition = _v;
	}







	List<UIWorldMapRoundMonsterInfo> _monsterSlots = new List<UIWorldMapRoundMonsterInfo>();
	Stack<UIWorldMapRoundMonsterInfo> _monsterSlotPool = new Stack<UIWorldMapRoundMonsterInfo>();

	void addMonsterSlot(string iconId, int rare, string name, int level)
	{
		foreach(UIWorldMapRoundMonsterInfo info in _monsterSlots)
		{
			if(info.name == name && info.checkId == iconId && info.level == level) return;
		}

		UIWorldMapRoundMonsterInfo s;

		if(_monsterSlotPool.Count > 0 ) s = _monsterSlotPool.Pop();
		else
		{
			s = Instantiate(roundMonsterInfoSlot) as UIWorldMapRoundMonsterInfo;
			s.cachedTransform = s.transform;
		}

		_monsterSlots.Add(s);

		s.setData(iconId, name, rare, level);
		s.transform.parent = goMonsterContainer.transform;

		_v.x = 6  + ((_monsterSlots.Count -1) % 6) * 112;
		_v.y = -44 - ((_monsterSlots.Count-1) / 6) * 149 - 10;
		_v.z = 0;

		s.setPosition(_v);
	}


	public void hide()
	{
		clearSlots();

		gameObject.SetActive(false);
	}


	void clearSlots()
	{
		foreach(UIWorldMapRoundMonsterInfo s in _monsterSlots)
		{
			_monsterSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_monsterSlots.Clear();
		
		
		foreach(UIChallengeItemSlot s in _rewardSlots)
		{
			_rewardSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_rewardSlots.Clear();
	}


	string getRoundTypeIcon(RoundData rd)
	{
		switch(rd.mode)
		{
		case RoundData.MODE.KILLEMALL: return "img_epicmode_kill"; // 섬멸 
		case RoundData.MODE.SURVIVAL: return "img_epicmode_survival"; // 서바이벌
		case RoundData.MODE.PROTECT: return "img_epicmode_protect"; // 보허
		case RoundData.MODE.SNIPING: return "img_epicmode_sniping"; //보스대전
		case RoundData.MODE.KILLCOUNT: return "img_epicmode_killcount"; //몬스터 사냥
		case RoundData.MODE.KILLCOUNT2: return "img_epicmode_killcount";
		case RoundData.MODE.ARRIVE: return "img_epicmode_arrive";
		case RoundData.MODE.DESTROY: return "img_epicmode_destroy";
		case RoundData.MODE.GETITEM: return "img_epicmode_getitem";
			break;
		}

		return "";
	}

	public UIIconTooltip iconTooltip;

	public void onClickRewardItem(UIChallengeItemSlot selectItemSlot)
	{
		if(gameObject.activeSelf)
		{
			iconTooltip.start(selectItemSlot.infoData.getTooltipDescription(), selectItemSlot.transform.position.x, selectItemSlot.transform.position.y + 150.0f);
		}
	}



	void Update()
	{
		if(Input.GetMouseButtonUp(1))
		{

		}
	}
}


public class StageDetailInfoObjectName
{
	public string name;
	public int num;
}
