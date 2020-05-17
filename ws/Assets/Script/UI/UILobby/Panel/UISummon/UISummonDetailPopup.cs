using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class UISummonDetailPopup : MonoBehaviour 
{

	public UIPopupcardDetailReforgePanel transcendInfo;

	public GameObject descriptionContainer;

	public UIPanelRefresher[] panelRefresher;

	public UICardFrame cardFrame;


	Vector3 _v;

	public UIIconTooltip iconToolTip;

	public UICharacterRotate characterRotate;

	public SimpleRotater rotater;


	[HideInInspector] public Monster sample;
	public Transform sampleContainer;





	public UISprite spBackground;
	public UISprite spSkipModeBackground;

	public UISprite spIconPhysicOrMagic, spIconRange, spIconTargetingType;

	public UIButton btnPhysicOrMagic, btnRange, btnTargetingType;

	public UIButton btnClose,btnPutOn,btnBreak,btnPutOff,btnInteractive,btnInstantBuy;

	public GameObject goCannotWear;
	public UILabel lbWearCharacter;





	public UITabButton[] tabPassiveInfo;

	// 테스트용.
	public UIButton btnPlayAttack, btnPlayDeadAni, btnPlaySkillAni;

	public UIButton btnCompose, btnEvolution;
	public UIButton btnReinfoce;
	public UIButton btnReforge;



	public UIScrollView scrollView;

	public GameObject mainPanel;



	public Transform movablePanel;

	public UILabel lbInstantBuyPrice;


	public UILabel lbStatAtk, lbStatDef, lbStatHP, lbStatSp;

	public UISummonDetailSkillDescriptionSlot[] skillEffectSlot = new UISummonDetailSkillDescriptionSlot[6];


	public UILabel lbUseSp;

	public bool isFromInventorySlot = false;

	public UIPopupCardDetailReinforcePanel reinforceInfoBar;
	
	int _rare = 0;

	GameIDData data = new GameIDData();

	public RuneInfoPopup.Type popupType;

	void Awake()
	{
		UIEventListener.Get(btnClose.gameObject).onClick = onClickClose;
		UIEventListener.Get(btnPutOn.gameObject).onClick = onClickPutOn;
		UIEventListener.Get(btnPutOff.gameObject).onClick = onClickPutOff;
		UIEventListener.Get(btnBreak.gameObject).onClick = onClickBreak;
		UIEventListener.Get(btnInteractive.gameObject).onPress = onPressInteractive;

		UIEventListener.Get(btnInstantBuy.gameObject).onClick = onClickInstantBuy;

		UIEventListener.Get(btnPhysicOrMagic.gameObject).onClick = onIcon1;
		UIEventListener.Get(btnTargetingType.gameObject).onClick = onIcon2;
		UIEventListener.Get(btnRange.gameObject).onClick = onIcon3;

		UIEventListener.Get(btnReinfoce.gameObject).onClick = onClickReinforce;
		UIEventListener.Get(btnCompose.gameObject).onClick = onClickCompose;

		UIEventListener.Get(btnEvolution.gameObject).onClick = onClickEvolution;

		UIEventListener.Get(btnReforge.gameObject).onClick = onClickReforge;


		UIEventListener.Get(btnPlayAttack.gameObject).onClick = onClickPlayAttackAni;
		UIEventListener.Get(btnPlayDeadAni.gameObject).onClick = onClickPlayDeadAni;
		UIEventListener.Get(btnPlaySkillAni.gameObject).onClick = onClickPlaySkillAni;


		UIEventListener.Get(tabPassiveInfo[0].gameObject).onClick = onClickPassive1;
		UIEventListener.Get(tabPassiveInfo[1].gameObject).onClick = onClickPassive2;
		UIEventListener.Get(tabPassiveInfo[2].gameObject).onClick = onClickPassive3;


		scrollView.ResetPosition();
	}


	void onClickPassive1(GameObject go)
	{
		if(tabPassiveInfo[0].isEnabled) return;
		setPassiveInfo(0);

		GameIDData.dummyUnit.parse(data.baseId + "02001", GameIDData.Type.Unit);

		setDescription(true, GameIDData.dummyUnit);
	}

	void onClickPassive2(GameObject go)
	{
		if(tabPassiveInfo[1].isEnabled) return;
		setPassiveInfo(1);

		GameIDData.dummyUnit.parse(data.baseId + "02002", GameIDData.Type.Unit);

		setDescription(true, GameIDData.dummyUnit);
	}

	void onClickPassive3(GameObject go)
	{
		if(tabPassiveInfo[2].isEnabled) return;
		setPassiveInfo(2);

		GameIDData.dummyUnit.parse(data.baseId + "02003", GameIDData.Type.Unit);

		setDescription(true, GameIDData.dummyUnit);
	}


	void setPassiveInfo(int index)
	{
		for(int i = 0; i < 3; ++i)
		{
			tabPassiveInfo[i].isEnabled = (i == index);
		}
	}



	void onClickPlayAttackAni(GameObject go)
	{
		if(sample != null)
		{
			sample.playAni(Monster.ATK);
			sample.renderAniRightNow();
		}
	}


	void onClickPlaySkillAni(GameObject go)
	{
		if(sample != null)
		{
			int atkNum = getActiveUnitSkillAttackType();
			sample.playAni(Monster.ATK_IDS[atkNum]);
			sample.renderAniRightNow();
		}
	}


	void onClickPlayDeadAni(GameObject go)
	{
		if(sample != null)
		{
			sample.ani.Play(Monster.DEAD);
			SoundData.playDieSound(  sample.resourceId );
		}
	}



	void onClickInstantBuy(GameObject go)
	{
//		UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYITEMRUBY", challengeItemData.price.ToString()), onConfirmInstantBuy, null, challengeItemData.price);
	}
	
	
	void onConfirmInstantBuy()
	{
//		if(GameDataManager.priceCheck(ShopItem.Type.ruby, challengeItemData.price))
//		{
//			EpiServer.instance.sendBuyChallengeItem(challengeItemData.id);
//		}
	}



	public string[] strIconDescription = new string[3];

	public UILabel[] lbIconDescription = new UILabel[3];


	void onIcon1(GameObject go)
	{
		iconToolTip.start(strIconDescription[0],go.transform.position.x,go.transform.position.y + 150.0f);
	}

	void onIcon2(GameObject go)
	{
		iconToolTip.start(strIconDescription[1],go.transform.position.x,go.transform.position.y + 150.0f);
	}

	void onIcon3(GameObject go)
	{
		iconToolTip.start(strIconDescription[2],go.transform.position.x,go.transform.position.y + 150.0f);
	}


	public void onClickClose(GameObject go)
	{
		SoundData.play("uicm_close_mid");

		if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SUMMON)
		{
			GameManager.me.uiManager.uiMenu.uiSummon.gameObject.SetActive(true);
		}

		hide ();


		if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SUMMON)
		{
			if(GameManager.me.uiManager.uiMenu.uiSummon.gameObject.activeSelf)
			{
				GameManager.me.uiManager.uiMenu.uiSummon.checkTutorial();
			}
		}
	}
	
	public void onClickPutOn(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		hide ();
		GameManager.me.uiManager.uiMenu.uiSummon.startPutOn(data);
	}
	
	public void onClickPutOff(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		hide ();
		GameManager.me.uiManager.uiMenu.uiSummon.startPutOff( data  );
	}
	
	public void onClickBreak(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		UISystemPopup.open(UISystemPopup.PopupType.Sell, 
		                   Util.getUIText("MSG_SELL_UNITRUNE",RareType.WORD[data.rare],data.unitData.name, data.getSellPrice().ToString()),
		                   onConfirmBreak, null, data.serverId);
	}


	void onConfirmBreak()
	{
		hide();
		SoundData.play("uirn_runemelt");
		EpiServer.instance.sendSellUnitRune(data.serverId);
	}





	public GameIDData dummy = new GameIDData();

	public void show(GameIDData d, RuneInfoPopup.Type type, bool isInventorySlot, bool isFirstSequenceForReinforce = true, GameIDData compareData = null)
	{
		show(d,type,d.rare,d.level, isInventorySlot, isFirstSequenceForReinforce, compareData);
		if(isFirstSequenceForReinforce == false) btnClose.gameObject.SetActive(true);
	}

	// 뽑기는 당연히 인벤토리 슬롯이 될 수가 없다.
	public void showMakeResult(GameIDData d)
	{
		show(d, RuneInfoPopup.Type.Make, d.rare, d.level, false);
	}

	List<GameObject> _activeButtons = new List<GameObject>();


	private bool _isOpenRuneBook = false;
	private GameIDData _compareData = null;

	public void show(GameIDData d, RuneInfoPopup.Type type, int rare, int level, bool isInventorySlot = true, bool isFirstSequenceForReinforce = true, GameIDData compareData = null)
	{
		spSkipModeBackground.gameObject.SetActive(false);
		spBackground.gameObject.SetActive(true);

//		btnPlayAttack.gameObject.SetActive((type == Type.Book));
//		btnPlayDeadAni.gameObject.SetActive((type == Type.Book));
//		btnPlaySkillAni.gameObject.SetActive(false);

		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false)
			#endif
		{
			GamePlayerData nowPlayerData;

			if(GameManager.me.uiManager.uiMenu.uiSummon.gameObject.activeSelf && GameDataManager.instance.serverHeroData.Count > 1 && GameDataManager.instance.heroes.ContainsKey(GameManager.me.uiManager.uiMenu.uiSummon.tabPlayer.currentTab))
			{
				nowPlayerData = GameDataManager.instance.heroes[GameManager.me.uiManager.uiMenu.uiSummon.tabPlayer.currentTab];
			}
			else
			{
				nowPlayerData = GameDataManager.selectedPlayerData;
			}

			GameManager.me.changeMainPlayer(nowPlayerData,nowPlayerData.id,nowPlayerData.partsVehicle.parts.resource.ToUpper());
		}


		if(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			if(GameManager.me.effectManager.didStartLoadEffect == false)
			{
				GameManager.me.effectManager.startLoadEffects(true);
			}
		}
		
		
		_isOpenRuneBook = GameManager.me.uiManager.popupRuneBook.gameObject.activeSelf;

		if(_isOpenRuneBook)
		{
			GameManager.me.uiManager.uiMenuCamera.camera.enabled = false;
		}


		data.parse(d.serverId);
		_compareData = compareData;
		_rare = rare;

		reinforceInfoBar.setReinforceData(data);

		isFromInventorySlot = isInventorySlot;

		popupType = type;

		if(sample != null)
		{
			GameManager.me.characterManager.cleanMonster(sample);
			sample = null;
		}


		descriptionContainer.SetActive(popupType != RuneInfoPopup.Type.Make);
		cardFrame.showDescriptionPanel(popupType != RuneInfoPopup.Type.Make, false);


		RuneStudioMain.instance.cam512.gameObject.SetActive(true);

		characterRotate.canRotate = true;

		rotater.enabled = (type == RuneInfoPopup.Type.Make);

		SoundData.play("uicm_popup_mid");

		movablePanel.localPosition = new Vector3(-3000,0,0);

		spBackground.color = new Color(0,0,0,100.0f/255.0f);

		gameObject.SetActive(true);

		characterRotate.state = UICharacterRotate.STATE_NORMAL;

		btnClose.gameObject.SetActive(type != RuneInfoPopup.Type.Reinforce);

		_activeButtons.Clear();

		if(type == RuneInfoPopup.Type.Reinforce && isFirstSequenceForReinforce)
		{
			btnInstantBuy.gameObject.SetActive(false);
			
			btnBreak.gameObject.SetActive(false);
			btnPutOn.gameObject.SetActive(false);
			btnPutOff.gameObject.SetActive(false);
			btnReinfoce.gameObject.SetActive(false);
			btnCompose.gameObject.SetActive(false);
			btnEvolution.gameObject.SetActive(false);
			btnReforge.gameObject.SetActive(false);
			goCannotWear.SetActive(false);
		}
		else if(type == RuneInfoPopup.Type.Book || type == RuneInfoPopup.Type.PreviewOnly || type == RuneInfoPopup.Type.Make)
		{
//			challengeItemData = null;

			btnInstantBuy.gameObject.SetActive(false);
			btnBreak.gameObject.SetActive(false);
			btnPutOn.gameObject.SetActive(false);
			btnPutOff.gameObject.SetActive(false);
			btnReinfoce.gameObject.SetActive(false);
			btnCompose.gameObject.SetActive(false);
			btnEvolution.gameObject.SetActive(false);
			btnReforge.gameObject.SetActive(false);
			goCannotWear.SetActive(false);
		}
		else //Normal, Reinforce
		{
//			challengeItemData = null;

			btnInstantBuy.gameObject.SetActive(false);

			btnBreak.gameObject.SetActive(true);

			btnReinfoce.gameObject.SetActive(true);

			// 탭에서 온 녀석.
			if(isFromInventorySlot == false)
			{
				btnBreak.isEnabled = false;
				btnPutOn.gameObject.SetActive(false);
				btnPutOff.gameObject.SetActive(true);

				goCannotWear.SetActive(false);
			}
			// 일반으로 온 녀석.
			else
			{
				btnBreak.isEnabled = true;
				btnPutOff.gameObject.SetActive(false);

				if(GameManager.me.uiManager.uiMenu.uiSummon.checkCanPutOn(data.unitData))
				{
					btnPutOn.gameObject.SetActive(true);
					goCannotWear.SetActive(false);
				}
				else
				{
					goCannotWear.SetActive(true);
					btnPutOn.gameObject.SetActive(false);
					
					lbWearCharacter.text = Util.getUIText("STR_USE_THIS", Util.getUIText( GameManager.me.uiManager.uiMenu.uiSummon.lastCheckPutOnCharacter ) );
				}

			}

			btnReinfoce.gameObject.SetActive(true);


//			* 제련 원본 룬 : 제련 레벨 99 미만의 SS등급 소환룬, 스킬룬, 히어로장비   (A이하 : 합성 버튼 / S : 진화 버튼 / SS : 제련 버튼)														
//				* 제련 재료 룬 : (등급부분을 제외하고) 원본 룬과 동일한 베이스 아이디를 가진 SS등급 or S등급														
//					예> UN613 의 제련 재료 : UN613, UN513 // SK_6110 : SK6110, SK5110 // LEO_BD6_22 : LEO_BD6_22, LEO_BD5_22														
//					* 제련 시, 100,000 골드 소모														

			if (data.canReforge())
			{
//				btnReinfoce.isEnabled = false;
				btnReforge.gameObject.SetActive(true);
			}
			else
			{
//				btnReinfoce.isEnabled = true;
				btnReforge.gameObject.SetActive(false);
			}

			btnEvolution.gameObject.SetActive(data.rare == RareType.S);
			btnCompose.gameObject.SetActive(data.rare < RareType.S);

		}


		_v.y = -253f;
		_v.z = -56f;

		if(goCannotWear.activeSelf) _activeButtons.Add(goCannotWear);
		if(btnPutOn.gameObject.activeSelf) _activeButtons.Add(btnPutOn.gameObject);
		if(btnPutOff.gameObject.activeSelf) _activeButtons.Add(btnPutOff.gameObject);
		if(btnReinfoce.gameObject.activeSelf) _activeButtons.Add(btnReinfoce.gameObject);
		if(btnReforge.gameObject.activeSelf) _activeButtons.Add(btnReforge.gameObject);
		if(btnCompose.gameObject.activeSelf) _activeButtons.Add(btnCompose.gameObject);
		if(btnEvolution.gameObject.activeSelf) _activeButtons.Add(btnEvolution.gameObject);


		for(int i = _activeButtons.Count - 1; i >=0; --i)
		{
			_v.x = 323 - i * (323-177);
			_activeButtons[i].transform.localPosition = _v;
		}

		_activeButtons.Clear();

		cardFrame.showFrame(_rare);
		cardFrame.setTranscendLevel(data);


		if(data.unitData.atkPhysic > data.unitData.atkMagic)
		{
			spIconPhysicOrMagic.spriteName = "icn_animal_character_physicsattack";
			strIconDescription[0] = Util.getUIText("STYPE_ATKPHYSIC");
		}
		else
		{
			spIconPhysicOrMagic.spriteName = "icn_animal_character_magicattack";
			strIconDescription[0] = Util.getUIText("STYPE_ATKMAGIC");
		}

		if(data.unitData.attackType.type == 1 || data.unitData.attackType.type == 3) 
		{
			spIconTargetingType.spriteName = "icn_animal_character_oneattack";
			strIconDescription[1] = Util.getUIText("STYPE_ONEATK");
		}
		else
		{
			spIconTargetingType.spriteName = "icn_animal_character_multiattack";
			strIconDescription[1] = Util.getUIText("STYPE_MULTIATK");
		}
		
		if(data.unitData.attackType.type == 1 || data.unitData.attackType.type == 2)
		{
			spIconRange.spriteName = "icn_animal_character_shortdistance";
			strIconDescription[2] = Util.getUIText("STYPE_SHORTATK");
		}
		else 
		{
			if(data.unitData.attackType.type == 14)
			{
				spIconRange.spriteName = "icn_skill_character_buster";
				strIconDescription[2] = Util.getUIText("STYPE_BUSTATK");
			}
			else
			{
				spIconRange.spriteName = "icn_animal_character_longdistance";
				strIconDescription[2] = Util.getUIText("STYPE_LONGATK");
			}
		}

		for(int i = 0; i < 3; ++i)
		{
			lbIconDescription[i].text = strIconDescription[i];
		}


		cardFrame.lbName.text = data.unitData.name;

		if(_compareData != null && popupType != RuneInfoPopup.Type.Book)
		{
			cardFrame.setLevel( level , (level - _compareData.level) );
		}
		else
		{
			cardFrame.setLevel( level );
		}

		sb.Length = 0;
		sb2.Length = 0;


		tabPassiveInfo[0].transform.parent.gameObject.SetActive(type == RuneInfoPopup.Type.Book && data.rare >= RareType.S);

		setPassiveInfo(0);


		setDescription(popupType == RuneInfoPopup.Type.Book, data);

		_q = sampleContainer.localRotation;
		//_v.x = 0; _v.y = 21; _v.z = 0;
		_q.eulerAngles = _v;
		sampleContainer.localRotation = _q;

		if(sample == null) StartCoroutine( getMonster(sample, data.unitData, sampleContainer, _rare) );

		showProcess2();


		visibleComposeNoticeMark();

//		btnPlaySkillAni.gameObject.SetActive((type == Type.Book) && getActiveUnitSkillAttackType() > -1);

	}



	int getActiveUnitSkillAttackType()
	{
		int slen = 0;

		if(data == null) return -1;


		if(data.unitData.skill != null) slen = data.unitData.skill.Length;
		
		for(int i = 0; i < slen ; ++i)
		{
			if(GameManager.info.unitSkillData[data.unitData.skill[i]].activeSkillCooltime > -1)
			{
				return GameManager.info.unitSkillData[data.unitData.skill[i]].exeType;
			}
		}

		return -1;
	}




	void showProcess2()
	{
		scrollView.ResetPosition();

		if(popupType == RuneInfoPopup.Type.Make )
		{
			movablePanel.parent = RuneStudioMain.instance.moveablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Reinforce )
		{
			movablePanel.parent = RuneStudioMain.instance.reinforceMoveablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Compose )
		{
			movablePanel.parent = RuneStudioMain.instance.composeMovablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Evolve)
		{
			movablePanel.parent = RuneStudioMain.instance.evolveMovablePanelParent[4];
		}
		else if(popupType == RuneInfoPopup.Type.Transcend )
		{
			movablePanel.parent = RuneStudioMain.instance.transcendMovablePanelParent;
		}
		else
		{
			movablePanel.parent = mainPanel.transform;
		}

		movablePanel.localScale = Vector3.one;
		movablePanel.localPosition = Vector3.zero;
	}


	List<string> _unitSkillDescriptions = new List<string>();


	public void setOtherUserDescription(Player p)
	{
		UnitData baseUnit = GameManager.info.unitData[ data.unitData.baseUnitId];
		
		float useSp = 0; 
		
		useSp = data.unitData.sp;
		useSp -= (useSp * p.summonSpPercent(data.unitData));
		lbUseSp.text = Mathf.RoundToInt(useSp / p.maxSp * 100.0f) + ""; 
	}


	void setDescription(bool isBookType, GameIDData showData)
	{
		UnitData baseUnit = GameManager.info.unitData[showData.unitData.baseUnitId];

		float useSp = 0; 

		if(isBookType)
		{
			useSp = baseUnit.sp;
			useSp -= (useSp * GameManager.me.player.summonSpPercent(baseUnit));
			lbUseSp.text = Mathf.RoundToInt(useSp / GameManager.me.player.maxSp * 100.0f) + ""; 
		}
		else
		{
			useSp = showData.unitData.sp;
			useSp -= (useSp * GameManager.me.player.summonSpPercent(showData.unitData));

			lbUseSp.text = Mathf.RoundToInt(useSp / GameManager.me.player.maxSp * 100.0f) + ""; 
		}

		sb2.Length = 0;

//		sb2.Append(string.Format("{0:0}",showData.unitData.sp));
//		lbStatSp.text = sb2.ToString();
		sb2.Length = 0;


		//======== 공격력 ========== //

		int atk = 0;

		float ap = showData.unitData.atkPhysic;
		float am = showData.unitData.atkMagic;
		float asd = showData.unitData.atkSpeed;
		int aa = 0;

		if(isBookType == false)
		{
			ap = showData.getTranscendValueByATTR( ap, WSATTR.ATK_PHYSIC_I);
			am = showData.getTranscendValueByATTR( am, WSATTR.ATK_MAGIC_I);
			asd = showData.getTranscendValueByATTR( asd, WSATTR.ATK_SPEED_I);
		}


		switch(showData.unitData.attackType.type)
		{
		case 10:
			aa = showData.unitData.attackType.attr[1];
			if(isBookType == false) aa = showData.getTranscendValueByATTR( aa, WSATTR.ATK_ATTR2_I);
			atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
			break;
		case 11:
			aa = showData.unitData.attackType.attr[2];
			if(isBookType == false) aa = showData.getTranscendValueByATTR( aa, WSATTR.ATK_ATTR3_I);
			atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
			break;
		case 12:
			aa = showData.unitData.attackType.attr[1];
			if(isBookType == false) aa = showData.getTranscendValueByATTR( aa, WSATTR.ATK_ATTR2_I);
			atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
			break;
		default:
			atk = Mathf.RoundToInt((ap + am) / asd);
			break;
		}

		sb2.Append(string.Format("{0:0}",atk));


		if(_compareData != null && isBookType == false)
		{
			int nowAtk = atk;

			ap = _compareData.unitData.atkPhysic;
			am = _compareData.unitData.atkMagic;
			asd = _compareData.unitData.atkSpeed;

			if(isBookType == false)
			{
				ap = _compareData.getTranscendValueByATTR(ap, WSATTR.ATK_PHYSIC_I);
				am = _compareData.getTranscendValueByATTR(am, WSATTR.ATK_MAGIC_I);
				asd = _compareData.getTranscendValueByATTR(asd, WSATTR.ATK_SPEED_I);
			}

			switch(_compareData.unitData.attackType.type)
			{
			case 10:
				aa = _compareData.unitData.attackType.attr[1];
				if(isBookType == false) aa = _compareData.getTranscendValueByATTR(aa, WSATTR.ATK_ATTR2_I);
				atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
				break;
			case 11:
				aa = _compareData.unitData.attackType.attr[2];
				atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
				if(isBookType == false) aa = _compareData.getTranscendValueByATTR(aa, WSATTR.ATK_ATTR3_I);
				break;
			case 12:
				aa = _compareData.unitData.attackType.attr[1];
				if(isBookType == false) aa = _compareData.getTranscendValueByATTR(aa, WSATTR.ATK_ATTR2_I);
				atk = Mathf.RoundToInt((ap + am) / asd * ((float)aa) / 500f);
				break;
			default:
				atk = Mathf.RoundToInt((ap + am) / asd);
				break;
			}

			displayDiffStat(nowAtk, atk, sb2);
		}

		lbStatAtk.text = sb2.ToString();
		sb2.Length = 0;


		//========== DEF =============//

		float def = ((showData.unitData.defMagic > showData.unitData.defPhysic)?showData.unitData.defMagic:showData.unitData.defPhysic);

		if(isBookType == false)
		{
			if(showData.unitData.defMagic > showData.unitData.defPhysic)
			{
				def = showData.getTranscendValueByATTR(def, WSATTR.DEF_MAGIC_I);
			}
			else
			{
				def = showData.getTranscendValueByATTR(def, WSATTR.DEF_PHYSIC_I);
			}
		}

		sb2.Append(string.Format("{0:0}",def));

		if(_compareData != null && isBookType == false)
		{
			float nowDef = def;

			def = ((_compareData.unitData.defMagic > _compareData.unitData.defPhysic)?_compareData.unitData.defMagic:_compareData.unitData.defPhysic);

			if(isBookType == false)
			{
				if(_compareData.unitData.defMagic > _compareData.unitData.defPhysic)
				{
					def = _compareData.getTranscendValueByATTR(def, WSATTR.DEF_MAGIC_I);
				}
				else
				{
					def = _compareData.getTranscendValueByATTR(def, WSATTR.DEF_PHYSIC_I);
				}
			}

			displayDiffStat(nowDef , def, sb2);
		}


		lbStatDef.text = sb2.ToString();
		sb2.Length = 0;



		//========== HP =============//
		float uhp = showData.unitData.hp;
		if(isBookType == false) uhp = showData.getTranscendValueByATTR( uhp, WSATTR.HP_I);

		sb2.Append(string.Format("{0:0}",uhp));//{0:0.0} //{0:0}

		// 비교 대상 있을시.
		if(_compareData != null && isBookType == false)
		{
			float cUhp = _compareData.unitData.hp;

			if(isBookType == false) cUhp = _compareData.getTranscendValueByATTR(cUhp, WSATTR.HP_I);

			displayDiffStat(uhp , cUhp, sb2);
		}

		lbStatHP.text = sb2.ToString();
		sb2.Length = 0;





		int slen = 0;
		if(showData.unitData.skill != null) slen = showData.unitData.skill.Length;

		_unitSkillDescriptions.Clear();
		
		for(int i = 0; i < slen; ++i)
		{
			if(GameManager.info.unitSkillData.ContainsKey(showData.unitData.skill[i]) &&  string.IsNullOrEmpty( GameManager.info.unitSkillData[ showData.unitData.skill[i] ] .description ) == false)
			{
				_unitSkillDescriptions.Add( GameManager.info.unitSkillData[ showData.unitData.skill[i] ].description );
			}
		}
		
		slen = _unitSkillDescriptions.Count;

		transcendInfo.setData(showData);

		for(int i = 0; i < slen || i < 6; ++i)
		{
			if(i < slen)
			{
				skillEffectSlot[i].gameObject.SetActive(true);

				sb.Append(_unitSkillDescriptions[i] + "[-]");

				skillEffectSlot[i].lbText.text = sb.ToString();

				sb.Length = 0;

				skillEffectSlot[i].spBg.height = Mathf.CeilToInt(skillEffectSlot[i].lbText.printedSize.y + 12);
				
				if(i > 0)
				{
					_v = skillEffectSlot[i-1].transform.localPosition;
					_v.y -= skillEffectSlot[i-1].lbText.printedSize.y + 18;
					skillEffectSlot[i].transform.localPosition = _v;
				}
				else if( i == 0 )
				{
					_v = skillEffectSlot[i].transform.localPosition;

					if(transcendInfo.gameObject.activeSelf)
					{
						_v.y = -269;
					}
					else
					{
						_v.y = -167;
					}

					skillEffectSlot[i].transform.localPosition = _v;
				}
			}
			else
			{
				skillEffectSlot[i].gameObject.SetActive(false);
			}
		}

		_unitSkillDescriptions.Clear();
		sb.Length = 0;
		sb2.Length = 0;
	}


	void displayDiffStat(float original, float compare, StringBuilder inputSb)
	{
		int diffDef = (int)(original - compare);
		
		if(diffDef > 0)
		{
			inputSb.Append("[ffe400] (↑" + string.Format("{0:0}",diffDef) + ")[-]");
		}
		else if(diffDef < 0)
		{
			inputSb.Append("[be1010] (↓" + string.Format("{0:0}",diffDef) + ")[-]");
		}
	}

	void displayDiffStat(int original, int compare, StringBuilder inputSb)
	{
		int diffDef = original - compare;
		
		if(diffDef > 0)
		{
			inputSb.Append("[ffe400] (↑" + string.Format("{0:0}",diffDef) + ")[-]");
		}
		else if(diffDef < 0)
		{
			inputSb.Append("[be1010] (↓" + string.Format("{0:0}",diffDef) + ")[-]");
		}
	}



	static StringBuilder sb = new StringBuilder();	
	static StringBuilder sb2 = new StringBuilder();	

	Quaternion _q;



	public void resize(float shotScale)
	{
		_v = Vector3.one * sample.monsterData.scale * (110.0f / (shotScale * (GameManager.info.modelData[ sample.monsterData.resource ].scale * 0.01f) ));
		sample.cTransform.localScale = _v;
	}

	IEnumerator getMonster(Monster mon, UnitData md, Transform parent, int rare, bool isPlayAni = true)
	{
		while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

		GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[md.resource]);
		GameDataManager.instance.startModelLoad();

		while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

		mon = GameManager.me.characterManager.getMonster(false,true,md.resource,false);

		mon.init(null, null, md.id,true,Monster.TYPE.UNIT);

		CharacterUtil.setRare(rare, mon);

		mon.cTransform.localScale *= 110.0f / mon.uiSize2; //90.0f / sample.getHitObject().height;

		mon.container.SetActive(true);

		mon.setParent( parent );

		_v = mon.cTransform.localPosition;
		_v.x = 0.0f;
		_v.y = GameManager.info.modelData[mon.resourceId].shotYPos;
		_v.z = 0.0f;
		mon.cTransform.localPosition = _v;
		
		_q = parent.transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		parent.transform.localRotation = _q;
		
		_q = mon.cTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		mon.cTransform.localRotation = _q;
		
		
		_q = mon.tf.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		mon.tf.localRotation = _q;

		_v = mon.cTransform.position;
		mon.shadow.tf.position = _v;


		if(isPlayAni && mon.ani.GetClip(Monster.NORMAL_LOBBY) != null)
		{
			mon.ani.Play(Monster.NORMAL_LOBBY);
			mon.ani.CrossFadeQueued(Monster.NORMAL_LOBBY);
		}
		else if(isPlayAni && mon.ani.GetClip(Monster.NORMAL) != null)
		{
			mon.ani.Play(Monster.NORMAL);
		}
		else
		{
			if( mon.ani.GetClip(Monster.ATK) == null)
			{
				mon.ani[Monster.NORMAL].time = mon.ani[Monster.NORMAL].length / 2.0f;
				mon.ani[Monster.NORMAL].speed = 0.0f;
				mon.ani.Play(Monster.NORMAL);
			}
			else
			{
				mon.ani[Monster.ATK].time = GameManager.info.modelData[mon.resourceId].poseTime;
				mon.ani[Monster.ATK].speed = 0.0f;
				mon.ani.Play(Monster.ATK);
			}
		}

		if(isPlayAni)
		{
			sample = mon;

//			if(PerformanceManager.isLowPc == false)
			{
				GameManager.me.effectManager.checkUnitBodyEffect(mon, false);
			}
		}
	}

	public void showSkipModeResult()
	{
		movablePanel.parent = mainPanel.transform;
		movablePanel.localScale = Vector3.one;
		movablePanel.localPosition = Vector3.zero;
	}

	public void hide()
	{
		spSkipModeBackground.gameObject.SetActive(false);

		iconToolTip.hide();
		//popupCharacterCameraContainer.SetActive(false);
		gameObject.SetActive(false);

		movablePanel.parent = mainPanel.transform;

		RuneStudioMain.instance.cam512.gameObject.SetActive(false);

		if(sample != null)
		{
			GameManager.me.effectManager.removeBodyEffect(sample);

			GameManager.me.characterManager.cleanMonster(sample);
			sample = null;
		}

		if(popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend)
		{
			RuneStudioMain.instance.endProcess();
		}

		popupType = RuneInfoPopup.Type.Normal;

		GameManager.me.uiManager.uiMenuCamera.camera.enabled = true;

		if(_isOpenRuneBook)
		{
			//GameManager.me.uiManager.popupRuneBook.gameObject.SetActive(true);
			_isOpenRuneBook = false;
		}

		--GameManager.me.uiManager.popupReforege.step;
	}


	float checkTime = 0.0f;
	
	void onPressInteractive(GameObject go, bool isPress)
	{
		if(sample == null) return;
		
		if(isPress)
		{
			rotater.enabled = false;
			characterRotate.state = UICharacterRotate.STATE_PRESS;
			checkTime = Time.realtimeSinceStartup;
		}
		else
		{
			characterRotate.state = UICharacterRotate.STATE_NORMAL;
			if(Time.realtimeSinceStartup - checkTime < 0.4f)
			{
//				sample.playAni(Monster.ATK);
//				sample.renderAniRightNow();

				if(sample.ani.GetClip(Monster.ATK) != null && sample.ani.IsPlaying(Monster.ATK) == false )
				{

					sample.ani.CrossFade(Monster.ATK);

					int atkNum = 0;

					if(Monster.ATK_INDEX.TryGetValue(Monster.ATK, out atkNum))
					{
						SoundData.playAttackSound(sample.monsterData.resource,  atkNum);
					}

					if(sample.ani.GetClip(Monster.NORMAL_LOBBY) != null)
					{
						sample.ani.CrossFadeQueued(Monster.NORMAL_LOBBY);
					}
					else if(sample.ani.GetClip(Monster.NORMAL) != null)
					{
						sample.ani.CrossFadeQueued(Monster.NORMAL);
					}
				}
			}
			rotater.enabled = (popupType == RuneInfoPopup.Type.Make);
		}
	}










	void onClickReinforce(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		if(data.reinforceLevel >= GameIDData.MAX_LEVEL)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REINFORCE_NOMORE"));
			return;
		}


		hide();

		GameManager.me.uiManager.uiMenu.uiSummon.startReinforceMode(GameIDData.getClone( data ) , isFromInventorySlot);
	}


	void onClickReforge(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		
		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}

		hide();

		GameManager.me.uiManager.popupReforege.show(data.serverId, !isFromInventorySlot);
	}


	void onClickEvolution(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		
		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}
		
		if(data.rare >= RareType.SS)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("EVOLVE_NOMORE"));
			return;
		}
		else if(data.rare != RareType.S || data.reinforceLevel < 20)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("EVOLVE_LEVELLIMIT"));
			return;
		}


		GameManager.me.uiManager.popupEvolution.show(Rune.Type.Unit, data.serverId, (isFromInventorySlot)?WSDefine.NO:WSDefine.YES);

	}




	public UISprite spComposeNotice;

	void onClickCompose(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}


		if(data.rare >= RareType.S)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("COMPOSE_NOMORE"));
			return;
		}
		else if(data.reinforceLevel < 20)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("COMPOSE_LEVELLIMIT"));
			return;
		}
		else
		{
			int composeSourceNumber = 0;
			
			for(int i = 0; i < GameDataManager.instance.unitInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.unitInventoryList[i].rare == data.rare &&
				   GameDataManager.instance.unitInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					++composeSourceNumber;
					if(composeSourceNumber > 2) break;
				}
			}
			
			if(isFromInventorySlot) --composeSourceNumber;
			
			if(composeSourceNumber <= 0)
			{

				int needNum = 1;
				if(isFromInventorySlot == false) needNum = 2;

				if(GameDataManager.instance.playerUnitSlots != null)
				{
					foreach(KeyValuePair<string, PlayerUnitSlot[]> kv in GameDataManager.instance.playerUnitSlots)
					{
						if(GameDataManager.instance.serverHeroData.ContainsKey(kv.Key))
						{
							foreach(PlayerUnitSlot ps in kv.Value)
							{
								if(ps.isOpen == false || ps.unitInfo == null) continue;
								
								if(ps.unitInfo.rare == data.rare &&
								   ps.unitInfo.reinforceLevel == GameIDData.MAX_LEVEL)
								{
									--needNum;
									
									if(needNum <= 0)
									{
										UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("CANT_COMPOSE_SELECTED"));
										
										return;
									}
								}
							}
						}
					}
				}

				//합성 재료가 없습니다.\n합성 재료로 O급 20레벨 소환룬(스킬룬)이 필요합니다.
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NO_COMPOSE_SUMMON",RareType.CHARACTER[data.rare]));
				return;
			}
		}





		hide();
		
		GameManager.me.uiManager.uiMenu.uiSummon.startComposeMode( GameIDData.getClone( data ), isFromInventorySlot);
	}













	float currentTime = 0.0f;
	float _updateLoopLeftTime = 0.0f; 
	void LateUpdate()
	{
		float newTime = currentTime + Time.smoothDeltaTime;
		float frameTime = newTime - currentTime; 
		currentTime = newTime;
		_updateLoopLeftTime += frameTime; 
		
		while ( _updateLoopLeftTime >= GameManager.LOOP_INTERVAL)      
		{       
			if(sample != null)
			{
				sample.updateAnimationMethod();
			}
			_updateLoopLeftTime -= GameManager.globalDeltaTime;
		} 
		
//		if(sample != null) sample.renderAni();
	}










	void visibleComposeNoticeMark()
	{
		bool visible = true;
		
		if(data.rare >= RareType.S)
		{
			visible = false;
		}
		else if(data.reinforceLevel < 20)
		{
			visible = false;
		}
		else if(btnCompose.gameObject.activeSelf == false)
		{
			visible = false;
		}
		else
		{
			int composeSourceNumber = 0;
			
			for(int i = 0; i < GameDataManager.instance.unitInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.unitInventoryList[i].rare == data.rare &&
				   GameDataManager.instance.unitInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					++composeSourceNumber;
					if(composeSourceNumber > 2) break;
				}
			}
			
			if(isFromInventorySlot) --composeSourceNumber;
			
			if(composeSourceNumber <= 0)
			{
				
				int needNum = 1;
				if(isFromInventorySlot == false) needNum = 2;
				
				if(GameDataManager.instance.unitSlots != null)
				{
					foreach(PlayerUnitSlot ps in GameDataManager.instance.unitSlots)
					{
						if(ps.isOpen == false || ps.unitInfo == null) continue;
						
						if(ps.unitInfo.rare == data.rare &&
						   ps.unitInfo.reinforceLevel == GameIDData.MAX_LEVEL)
						{
							--needNum;
							
							if(needNum <= 0)
							{
								spComposeNotice.enabled = true;
								
								return;
							}
						}
					}
				}
				
				//합성 재료가 없습니다.\n합성 재료로 O급 20레벨 소환룬(스킬룬)이 필요합니다.
				visible = false;
			}
		}
		
		
		spComposeNotice.enabled = visible;
	}


}
