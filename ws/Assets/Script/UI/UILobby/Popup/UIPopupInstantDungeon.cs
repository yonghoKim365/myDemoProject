using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class UIPopupInstantDungeon  : UIPopupBase 
{
	public const string EASY = "EASY";
	public const string NORMAL = "NORMAL";
	public const string HARD = "HARD";
	public const string EVENT = "EVENT";
		
	public UIButton tabEasy,tabNormal,tabHard, tabEvent;

	public GameObject[] goTabOn;
	public GameObject[] goTabOff;

	public UISprite spTabEasyNew, spTabNormalNew, spTabHardNew, spTabEventNew;

	public P_Sigong selectSigongData = null;

	public UIWorldMapRoundMonsterInfo roundMonsterInfoSlot;
	public UIChallengeItemSlot roundRewardInfoSlot;

	public GameObject goRewardContainer;
	public GameObject goMonsterContainer;

	public GameObject goEnterPriceType;
	public GameObject goEnterNoPriceType;


	public UIButton btnStartRound;

	public UIWorldMapRoundPanel[] roundPanel;

	public UILabel lbRoundStartPrice, lbDescription, lbNoMoreDungeon;

	public UISprite spRoundStartPriceType, spMonsterInfoWord;


	public UIScrollView contentContainer;//monsterScrollView, rewardScrollView


	public UIPanel contentPanel;

	public UISprite spRewardBg, spMonsterBg;

	public UIInstantDungeonList list;


	public enum Type
	{
		Easy, Normal, Hard, Event, None
	}
	
	public static Type type = Type.Easy;
	
	public static Type prevType = Type.None;



	const int REWARD_SLOT_NUM_PER_LINE = 3;
	const int MONSTER_SLOT_NUM_PER_LINE = 4;

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnStartRound.gameObject).onClick = onStartRound;
		UIEventListener.Get(btnClose.gameObject).onClick = onClose;

		UIEventListener.Get(tabEasy.gameObject).onClick = onClickTabEasy;
		UIEventListener.Get(tabNormal.gameObject).onClick = onClickTabNormal;
		UIEventListener.Get(tabHard.gameObject).onClick = onClickTabHard;
		UIEventListener.Get(tabEvent.gameObject).onClick = onClickTabEvent;

		GameDataManager.energyDispatcher -= updatePrice;
		GameDataManager.energyDispatcher += updatePrice;

		GameDataManager.rubyDispatcher -= updatePrice;
		GameDataManager.rubyDispatcher += updatePrice;

		GameDataManager.goldDispatcher -= updatePrice;
		GameDataManager.goldDispatcher += updatePrice;

	}



	void onClickTabEasy(GameObject go)
	{
		if(type == Type.Easy) return;
		prevType = type;
		type = Type.Easy;
		refresh();
	}
	
	void onClickTabNormal(GameObject go)
	{
		if(type == Type.Normal) return;
		prevType = type;
		type = Type.Normal;
		refresh();
	}


	void onClickTabHard(GameObject go)
	{
		if(type == Type.Hard) return;
		prevType = type;
		type = Type.Hard;
		refresh();
	}



	void onClickTabEvent(GameObject go)
	{
		if(type == Type.Event) return;
		prevType = type;
		type = Type.Event;
		refresh();
	}




	void onClose(GameObject go)
	{
		SoundData.play(closeSound);
		GameManager.me.clearMemory();
		hide();
		GameManager.me.uiManager.uiMenu.uiWorldMap.checkTutorialStart(false);
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

			if(selectSigongData.priceType == WSDefine.ENERGY)
			{
				if(GameDataManager.instance.energy < selectSigongData.price)
				{
					UISystemPopup.open( UISystemPopup.PopupType.YesNo, Util.getUIText("GO_TO_ENERGYSHOP"), GameManager.me.uiManager.popupShop.showEnergyShop);
					return;
				}
			}
			else if(selectSigongData.priceType == WSDefine.GOLD)
			{
				if(GameDataManager.instance.gold < selectSigongData.price)
				{
					UISystemPopup.open( UISystemPopup.PopupType.YesNo, Util.getUIText("GOTO_GOLD_SHOP"), GameManager.me.uiManager.popupShop.showGoldShop);
					return;
				}
			}
			else if(selectSigongData.priceType == WSDefine.RUBY)
			{
				if(GameDataManager.instance.ruby < selectSigongData.price)
				{
					UISystemPopup.open( UISystemPopup.PopupType.YesNo, Util.getUIText("GOTO_RUBY_SHOP"), GameManager.me.uiManager.popupShop.showRubyShop);
					return;
				}
			}

			if(selectSigongData.forcedDeck != null && selectSigongData.forcedDeck == "HERO_SELECT")
			{
				popupSelectHero.show();
			}
			else
			{
				EpiServer.instance.sendPlaySigong(selectSigongData.id);
			}
		}
	}

	public UIInstantDungeonSelectHero popupSelectHero;


	public GamePlayerData selectHeroData;

	public void onCompleteReceivePlaySigong()
	{
		P_Sigong p = GameManager.me.uiManager.popupInstantDungeon.selectSigongData;

		if(p.roundId.StartsWith("PVP"))
		{
			DebugManager.instance.pvpPlayerData = TestModeData.getTestModePlayerData(selectSigongData.roundId.Substring(4), false, "AI_PVP6");
			DebugManager.instance.pvpPlayerData2 = null;

			UIPlay.pvpleagueGrade = 6;

			UIPlay.pvpImageUrl = DebugManager.instance.pvpPlayerData.id;
			UIPlay.playerImageUrl = PandoraManager.instance.localUser.image_url;

			GameManager.me.stageManager.setNowRound( GameManager.info.roundData["PVP"] , GameType.Mode.Sigong);
		}
		else
		{
			GameManager.me.stageManager.setNowRound( GameManager.info.roundData[GameManager.me.uiManager.popupInstantDungeon.selectSigongData.roundId] , GameType.Mode.Sigong);
		}

		GameManager.me.stageManager.sigongData = p;
		
		hide();

		GameManager.me.uiManager.showLoading();

		GamePlayerData gpd = null;

		if(string.IsNullOrEmpty(p.forcedDeck) == false)
		{
			if(p.forcedDeck == "HERO_SELECT")
			{
				gpd = selectHeroData;
			}
			else
			{
				gpd = TestModeData.getTestModePlayerData(p.forcedDeck,true,DebugManager.instance.ai, p.handicap);
			}
		}

		if(gpd == null)
		{
			if(string.IsNullOrEmpty( p.handicap ) == false)
			{
				string heroId = GameDataManager.instance.selectHeroId;
				
				gpd = new GamePlayerData(heroId);
				
				P_Hero heroData = GameDataManager.instance.serverHeroData[GameDataManager.instance.selectHeroId];
				
				string[] u = new string[5];
				string[] s = new string[3];

				if(p.handicap == WSDefine.HANDICAP_TYPE_UNIT || p.handicap == WSDefine.HANDICAP_TYPE_BOTH)
				{
					u[0] = string.Empty;
					u[1] = string.Empty;
					u[2] = string.Empty;
					u[3] = string.Empty;
					u[4] = string.Empty;
				}
				else
				{
					u = GameDataManager.instance.getSelectUnitRunes(null);
				}

				if(p.handicap == WSDefine.HANDICAP_TYPE_SKILL || p.handicap == WSDefine.HANDICAP_TYPE_BOTH)
				{
					s[0] = string.Empty;
					s[1] = string.Empty;
					s[2] = string.Empty;
				}
				else
				{
					s = GameDataManager.instance.getSelectSkillRunes(null);
				}

				DebugManager.instance.setPlayerData(gpd, true, 
				                                    heroData.name,
				                                    heroData.selEqts[HeroParts.HEAD],
				                                    heroData.selEqts[HeroParts.BODY],
				                                    heroData.selEqts[HeroParts.WEAPON],
				                                    heroData.selEqts[HeroParts.VEHICLE],
				                                    u,
				                                    s);

			}
		}

		GameManager.me.startGame(0.5f, gpd);
	}





	void updatePrice()
	{
		if(selectSigongData != null && gameObject != null && gameObject.activeSelf)
		{
			updatePriceForStartDungeon(selectSigongData, false);
		}
	}

	void updatePriceForStartDungeon(P_Sigong data, bool checkStatus = true)
	{
		if(  string.IsNullOrEmpty( data.priceType ) )
		{
			goEnterNoPriceType.SetActive(true);
			goEnterPriceType.SetActive(false);
		}
		else
		{
			goEnterNoPriceType.SetActive(false);
			goEnterPriceType.SetActive(true);

			bool enough = false;
			
			if(data.priceType == WSDefine.ENERGY)
			{
				spRoundStartPriceType.spriteName = WSDefine.ICON_ENERGY;
				
				if(GameDataManager.instance.energy < data.price)
				{
					enough = false;
				}
				else
				{
					enough = true;
				}
			}
			else if(data.priceType == WSDefine.GOLD)
			{
				spRoundStartPriceType.spriteName = WSDefine.ICON_GOLD;
				
				if(GameDataManager.instance.gold < data.price)
				{
					enough = false;
				}
				else
				{
					enough = true;
				}
			}
			else if(data.priceType == WSDefine.RUBY)
			{
				spRoundStartPriceType.spriteName = WSDefine.ICON_RUBY;
				
				if(GameDataManager.instance.ruby < data.price)
				{
					enough = false;
				}
				else
				{
					enough = true;
				}
			}
			
			if(enough)
			{
				lbRoundStartPrice.text = "[fff1c9]"+data.price+"[-]";
			}
			else
			{
				lbRoundStartPrice.text = "[b82b00]"+data.price+"[-]";
			}
		}

		if(checkStatus == false) return;

		if(string.IsNullOrEmpty(data.enterDepId) == false && GameDataManager.instance.sigongList[data.enterDepId].clearCount <= 0)
		{
			btnStartRound.isEnabled = false;
		}
		else
		{

			checkCanEnterDungeon(data);

			StartCoroutine(updateCurrentDungeonState(data));
		}
	}


	IEnumerator updateCurrentDungeonState(P_Sigong data)
	{
		while(data != null && selectSigongData != null && selectSigongData.id == data.id )
		{
			if(checkCanEnterDungeon(data))
			{
				yield return Util.ws1;
			}
			else
			{
				yield break;
			}
		}
	}

	bool checkCanEnterDungeon(P_Sigong data)
	{
		TimeSpan ts = (DateTime.Now - GameDataManager.instance.sigongTimer[data.id]);
		
		int leftTime = 0;

		bool canEnter = true;

		bool nowOpen = true;

		if(data.enterWaitingTime > 0)
		{
			leftTime = data.enterWaitingTime - (int)ts.TotalSeconds;
			
			if(leftTime >= 0) // 입장 시간이 남았으면...
			{
				canEnter = false;
			}
		}
		else if(data.enterCoolTime > 0)
		{
			leftTime = data.enterCoolTime - (int)ts.TotalSeconds;
			
			if(leftTime >= 0) // 쿨타임이 남았으면...
			{
				canEnter = false;
			}
		}

		if(data.closingTime > 0) // 닫힌 던전이면.
		{
			leftTime = data.closingTime - (int)ts.TotalSeconds;
			
			if(leftTime <= 0)
			{
				canEnter = false;
				nowOpen = false;
			}
		}

		btnStartRound.isEnabled = canEnter;

		return nowOpen;
	}

	public string openSound = "uicm_popup_big";
	public string closeSound = "uicm_close_big";

	private bool _playAni = false;

	private bool _selectLastPlayedIndex = false;

	private int _lastSelectedDungeonTotalCount = 0;


	public void open(bool playAni = false, bool refreshOnly = false, bool selectLastPlayedIndex = false)
	{
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

		if( string.IsNullOrEmpty( lbNoMoreDungeon.text ) )
		{
			lbNoMoreDungeon.text = Util.getUIText("NOMORE_SIGONG");
		}

		_playAni = playAni;

		gameObject.SetActive(true);

		popupSelectHero.hide();

		if(refreshOnly == false)
		{
			if(_playAni)
			{
				SoundData.play(openSound);
				popupPanel.transform.localPosition = new Vector3(0,1000,0);
				
			}
			else
			{
				popupPanel.transform.localPosition = new Vector3(0,0,0);
			}
		}

		if(selectLastPlayedIndex == false)
		{
			list.clear();
		}

		_selectLastPlayedIndex = selectLastPlayedIndex;

		EpiServer.instance.sendPrepareSigong();
	}


	public void onCompleteReceiveSigongData()
	{
		if(_playAni) ani.Play();

		StartCoroutine(refreshCT());
	}


	public void requestSeverData()
	{
		_playAni = false;
		EpiServer.instance.sendPrepareSigong();
	}


	IEnumerator refreshCT()
	{
		while(ani.isPlaying)
		{
			yield return Util.ws01;
		}

		yield return Util.ws01;

		refresh(false);
	}





	void refresh(bool fromTab = true)
	{
		contentContainer.transform.localPosition = Vector3.zero;
		contentPanel.clipOffset = new Vector2(0,0);
		
		iconTooltip.hide();
		
		clearSlots();
		
		int i;
		int len;

		int getCurrentCount = getCurrentDungeonCount();


		Debug.LogError(fromTab + "  _selectIndex: " + _selectIndex + "   _lastSelectedDungeonTotalCount : " + _lastSelectedDungeonTotalCount
		               + "  getCurrentCount : " + getCurrentCount );

		if(fromTab == false && _selectIndex > 0 && _lastSelectedDungeonTotalCount == getCurrentCount && _selectIndex < getCurrentCount)
		{
			if(getCurrentCount > 3)
			{
				list.draw(false);
			}
			else
			{
				list.draw();
			}
		}
		else
		{
			list.draw();
			_selectIndex = 0;
		}

		checkNew();

		if(setDungeonByIndex(_selectIndex) == false)
		{
			lbNoMoreDungeon.enabled = true;
			contentPanel.enabled = false;
			btnStartRound.gameObject.SetActive(false);
		}

		_lastSelectedDungeonTotalCount = getCurrentDungeonCount();

		refreshTab();
	}


	bool setDungeonByIndex(int index)
	{
		if(GameDataManager.instance.sigongList != null)
		{
			foreach(KeyValuePair<string, P_Sigong> kv in GameDataManager.instance.sigongList)
			{
				if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Easy)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EASY) 
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Normal)
				{
					if(kv.Value.category != UIPopupInstantDungeon.NORMAL)
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Hard)
				{
					if(kv.Value.category != UIPopupInstantDungeon.HARD)
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Event)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EVENT)
					{
						continue;
					}
				}

				if(index > 0)
				{
					--index;
					continue;
				}

				btnStartRound.gameObject.SetActive(true);
				contentPanel.enabled = true;
				lbNoMoreDungeon.enabled = false;
				setDungeon(kv.Value, _selectIndex);
				return true;
			}
		}

		return false;

	}



	int getCurrentDungeonCount()
	{
		int count = 0;

		if(GameDataManager.instance.sigongList != null)
		{
			foreach(KeyValuePair<string, P_Sigong> kv in GameDataManager.instance.sigongList)
			{
				if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Easy)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EASY) 
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Normal)
				{
					if(kv.Value.category != UIPopupInstantDungeon.NORMAL)
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Hard)
				{
					if(kv.Value.category != UIPopupInstantDungeon.HARD)
					{
						continue;
					}
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Event)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EVENT)
					{
						continue;
					}
				}

				++count;
			}
		}

		return count;
	}





	void refreshTab()
	{
		switch(type)
		{
		case Type.Easy:
			setTabOnOff(0);
			break;
		case Type.Normal:
			setTabOnOff(1);
			break;

		case Type.Hard:
			setTabOnOff(2);
			break;
		case Type.Event:
			setTabOnOff(3);
			break;
		}
	}

	void setTabOnOff(int onIndex)
	{
		for(int i = 0; i < 4; ++i)
		{
			goTabOn[i].SetActive(i == onIndex);
			goTabOff[i].SetActive(i != onIndex);
		}
	}









	void drawRewardList(Dictionary<string, int> rewards)
	{
		if(rewards == null) return;

		foreach(KeyValuePair<string, int> kv in rewards)
		{
			addRewardSlot(kv.Key, kv.Value);
		}
	}




	void addUnitMonsterSlot(string unitId)
	{
		if( string.IsNullOrEmpty( unitId ) == false && GameManager.info.unitData.ContainsKey( unitId ))
		{
			addMonsterSlot(GameManager.info.unitData[unitId].resource,3,GameManager.info.unitData[unitId].slotName, GameManager.info.unitData[unitId].level);
		}
	}

	void drawMonsterSlot(RoundData rd, string pvpDeck = null)
	{
		if(pvpDeck != null)
		{
			TestModeData tmd = GameManager.info.testModeData[pvpDeck];

			if(tmd.hero == Character.LEO)
			{
				addMonsterSlot( Character.LEO_IMG, 1, Util.getUIText(tmd.hero), 1, UIWorldMapRoundMonsterInfo.Type.Player);
			}
			else if(tmd.hero == Character.KILEY)
			{
				addMonsterSlot( Character.KILEY_IMG, 1,Util.getUIText(tmd.hero), 1, UIWorldMapRoundMonsterInfo.Type.Player);
			}
			else if(tmd.hero == Character.CHLOE)
			{
				addMonsterSlot( Character.CHLOE_IMG, 1,Util.getUIText(tmd.hero), 1, UIWorldMapRoundMonsterInfo.Type.Player);
			}

			addUnitMonsterSlot(tmd.u1);
			addUnitMonsterSlot(tmd.u2);
			addUnitMonsterSlot(tmd.u3);
			addUnitMonsterSlot(tmd.u4);
			addUnitMonsterSlot(tmd.u5);

			return;
		}

//		if(DebugManager.instance.useDebug == false)
		{
			// 몬스터 히어로.
			if(rd.heroMonsters != null)
			{
				bool drawHiddenBoss = false;

				foreach(StageMonsterData d in rd.heroMonsters)
				{
					if(d.attr != null)
					{
						if(drawHiddenBoss == false)
						{
							if(d.attr == "H")
							{
								drawHiddenBoss = true;
								addMonsterSlot("hidden",2,"?", 99, UIWorldMapRoundMonsterInfo.Type.Hidden);
							}
						}

						continue;
					}
					
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



	IEnumerator refreshContentView()
	{
		clearSlots();

		RoundData rd;

		bool isPvpMode = false;

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			rd = GameManager.info.roundData[DebugManager.instance.debugRoundId];
		}
		else
#endif
		{

			if(selectSigongData.roundId.StartsWith("PVP"))
			{
				rd = GameManager.info.roundData["PVP"];
				isPvpMode = true;

				Debug.LogError(selectSigongData.roundId.Substring(4));
			}
			else
			{
				if(GameManager.info.roundData.TryGetValue(selectSigongData.roundId, out rd) == false)
				{
					yield break;
				}
			}
		}

		string description = "";

		if(string.IsNullOrEmpty(selectSigongData.desc) == false)
		{
			description = selectSigongData.desc;
		}
		else
		{
			description = rd.description;
		}

		lbDescription.text = description.Replace("BLINK]","]");

		goRewardContainer.gameObject.SetActive(true);

		_v = goRewardContainer.transform.position;
		
		_v.y = lbDescription.transform.position.y - (lbDescription.printedSize.y + 50);

		goRewardContainer.transform.position = _v;

		drawRewardList(selectSigongData.rewards);

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
			spRewardBg.height = Mathf.RoundToInt( Mathf.FloorToInt((_rewardSlots.Count-1) / REWARD_SLOT_NUM_PER_LINE) * 131 + 194); 
		}

		goMonsterContainer.transform.position = _v;

		if(isPvpMode)
		{
			drawMonsterSlot(rd, selectSigongData.roundId.Substring(4));
		}
		else
		{
			drawMonsterSlot(rd);
		}


		if(_monsterSlots.Count > 0)
		{
			spMonsterBg.height = Mathf.RoundToInt( Mathf.FloorToInt((_monsterSlots.Count-1) / MONSTER_SLOT_NUM_PER_LINE) * 149 + 219);
		}

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


//		rewardScrollView.ResetPosition();
//		monsterScrollView.ResetPosition();
		contentContainer.ResetPosition();


		if(description.Contains("BLINK]"))
		{
			++_effPlayIndex;
			StartCoroutine(descriptionEffect(_effPlayIndex,description));
		}
	}

	private int _effPlayIndex = 0;

	IEnumerator descriptionEffect(int effPlayIndex, string source)
	{
		string originalText = source.Replace("BLINK]","]");
		string changeText = source.Replace("BLINK]","00]");

		int i = 0;
		while(effPlayIndex == _effPlayIndex)
		{
			lbDescription.text = originalText;
			yield return Util.ws05;

			if(i >= 3)
			{
				i = 0;
				yield return Util.ws05;
			}

			lbDescription.text = changeText;
			yield return Util.ws03;

			++i;

		}
	}


	List<UIChallengeItemSlot> _rewardSlots = new List<UIChallengeItemSlot>();
	Stack<UIChallengeItemSlot> _rewardSlotPool = new Stack<UIChallengeItemSlot>();

	void addRewardSlot(string rewardId, int chance)
	{
		UIChallengeItemSlot s;

		if(_rewardSlotPool.Count > 0 ) s = _rewardSlotPool.Pop();
		else
		{
			s = Instantiate(roundRewardInfoSlot) as UIChallengeItemSlot;
		}
		
		_rewardSlots.Add(s);

		s.gameObject.SetActive(true);

		s.setSigongData(rewardId, chance);

		s.type = UIChallengeItemSlot.Type.StageRewardPreviewItem;
		s.useButton = true;

		s.transform.parent = roundRewardInfoSlot.transform.parent;

		_v.x = 61  + ((_rewardSlots.Count -1) % REWARD_SLOT_NUM_PER_LINE) * 140;
		_v.y = -95 - ((_rewardSlots.Count-1) / REWARD_SLOT_NUM_PER_LINE) * 131 - 10;

		_v.z = 0;

		s.transform.localPosition = _v;
	}







	List<UIWorldMapRoundMonsterInfo> _monsterSlots = new List<UIWorldMapRoundMonsterInfo>();
	Stack<UIWorldMapRoundMonsterInfo> _monsterSlotPool = new Stack<UIWorldMapRoundMonsterInfo>();

	void addMonsterSlot(string iconId, int rare, string name, int level, UIWorldMapRoundMonsterInfo.Type iconType = UIWorldMapRoundMonsterInfo.Type.Normal)
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

		s.setData(iconId, name, rare, level, iconType);
		s.transform.parent = roundMonsterInfoSlot.transform.parent;

		_v.x = 6  + ((_monsterSlots.Count -1) % MONSTER_SLOT_NUM_PER_LINE) * 106;
//		_v.y = 0;

//		_v.x = 6  + ((_monsterSlots.Count -1) % 5) * 142;
		_v.y = -44 - ((_monsterSlots.Count-1) / MONSTER_SLOT_NUM_PER_LINE) * 149 - 10;
		_v.z = 0;

		s.setPosition(_v);
	}

	private int _selectIndex = -1;

	public void setDungeon(P_Sigong data, int selectIndex = 0)
	{
#if UNITY_EDITOR
		Debug.LogError("setDungeon : " + selectIndex);
#endif

		_selectIndex = selectIndex;

		if(_selectIndex < 0) _selectIndex = 0;

		selectSigongData = data;

		GameManager.me.uiManager.popupInstantDungeon.list.listGrid.refreshPanel();

		StartCoroutine(refreshContentView());

		updatePriceForStartDungeon(data);
	}



	public void hide()
	{
		clearSlots();

		selectSigongData = null;

		gameObject.SetActive(false);

		saveLocalData();
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
//		iconTooltip.start(selectItemSlot.infoData.getTooltipDescription(), selectItemSlot.transform.position.x, selectItemSlot.transform.position.y + 150.0f);
	}










	public static Dictionary<string, bool> checkNewDic = new Dictionary<string, bool>();

	private static StringBuilder _sb = new StringBuilder();
	public static void saveLocalData()
	{
		_sb.Length = 0;
		
		foreach(KeyValuePair<string, bool> kv in checkNewDic)
		{
			if(kv.Value)
			{
				_sb.Append(kv.Key);
				_sb.Append(",");
			}
		}
		
		PlayerPrefs.SetString("SIGONG", _sb.ToString());
		
		_sb.Length = 0;
	}
	
	public static void loadLocalData()
	{
		checkNewDic.Clear();
		
		string str = PlayerPrefs.GetString("SIGONG");
		if(str != null)
		{
			string[] s = str.Split(',');
			
			foreach(string mKey in s)
			{
				if(string.IsNullOrEmpty(mKey) == false)
				{
					checkNewDic.Add(mKey, true);
				}
			}
		}
	}



	public static int easyNewNum = 0;
	public static int normalNewNum = 0;
	public static int hardNewNum = 0;
	public static int eventNewNum = 0;

	void checkNew()
	{
		easyNewNum = 0;
		normalNewNum = 0;
		hardNewNum = 0;
		eventNewNum = 0;

		if(GameDataManager.instance.sigongList != null)
		{
			foreach(KeyValuePair<string, P_Sigong> kv in GameDataManager.instance.sigongList)
			{
				switch(kv.Value.category)
				{
				case EASY:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++easyNewNum;
					break;
				case NORMAL:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++normalNewNum;
					break;
				case HARD:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++hardNewNum;
					break;

				case EVENT:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++eventNewNum;
					break;
				}
			}
		}

		spTabEasyNew.enabled = (easyNewNum > 0);
		spTabNormalNew.enabled = (normalNewNum > 0);
		spTabHardNew.enabled = (hardNewNum > 0);
		spTabEventNew.enabled = (eventNewNum > 0);
	}



}

