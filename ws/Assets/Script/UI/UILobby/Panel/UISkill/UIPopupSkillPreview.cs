using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class UIPopupSkillPreview : MonoBehaviour {

	public UIPopupcardDetailReforgePanel transcendInfo;

	public UIPanelRefresher[] panelRefresher;

	public UICardFrame cardFrame;
	public GameObject descriptionContainer;

	public UIIconTooltip iconToolTip;

	public UIButton btnClose,btnPutOn,btnBreak,btnPutOff,btnOk,btnInstantBuy;


	public GameObject goCannotWear;
	public UILabel lbWearCharacter;
	public Camera skillPreviewCamera;
	public GameObject previewMapFile;

	public UISprite spBackground, spIconTargetingType, spIconSkillType;

	public UISprite spSkipModeBackground;

	public UIButton btnIconSkillType, btnTargetingType;

	private StringBuilder _sb = new StringBuilder();
	private StringBuilder _sb2 = new StringBuilder();

	public UISprite spMiddleDescriptionContainer, spSkillDescriptionBg;
	public UILabel lbRuneCharging, lbChargingTime, lbSkillMp, lbSkillCootime, lbSkillDescription;

	public UIPopupSkillPreviewDescriptionSlot[] skillDescriptionSlot = new UIPopupSkillPreviewDescriptionSlot[5];

	public UIScrollView scrollView;

	public GameObject mainPanel;

	public Transform movablePanel;

	public static bool canPlaySkill = true;
	public UILabel lbInstantBuyPrice;

	public bool isFromInventorySlot = false;

	// 45번 튜토리얼 끝난 후 화면에 UI이펙트를 보여주기위해...
	public bool needToShowTutorialEndCircleEffect = false;

	public UIButton btnCompose, btnEvolution;
	public UISprite spComposeNotice;

	public RuneInfoPopup.Type popupType;

	public SimpleUIEventListener btnShootReceiver;

	bool _isEnabled = false;
	
	bool _isOpenSkillBook = false;
	
	Vector3 _v;


	bool _isInitUI = false;

	void Awake ()
	{
		UIEventListener.Get(btnClose.gameObject).onClick = onClose;
		UIEventListener.Get(btnOk.gameObject).onClick = onClose;

		UIEventListener.Get(btnPutOn.gameObject).onClick = onPutOn;
		UIEventListener.Get(btnBreak.gameObject).onClick = onBreak;
		UIEventListener.Get(btnPutOff.gameObject).onClick = onPutOff;

		UIEventListener.Get(btnIconSkillType.gameObject).onClick = onIcon1;
		UIEventListener.Get(btnTargetingType.gameObject).onClick = onIcon2;


		UIEventListener.Get(btnReinfoce.gameObject).onClick = onClickReinforce;
		UIEventListener.Get(btnCompose.gameObject).onClick = onClickCompose;

		UIEventListener.Get(btnEvolution.gameObject).onClick = onClickEvolution;
		UIEventListener.Get(btnReforge.gameObject).onClick = onClickReforge;

		UIEventListener.Get(btnInstantBuy.gameObject).onClick = onClickInstantBuy;

		btnShootReceiver.downCallback = onShoot;

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


	void onClose(GameObject go)
	{
		isEnabled = false;
	}

	void onPutOn(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		isEnabled = false;
		GameManager.me.uiManager.uiMenu.uiSkill.startPutOn(skillInfoData);
	}

	void onPutOff(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		isEnabled = false;
		GameManager.me.uiManager.uiMenu.uiSkill.startPutOff(skillInfoData);
	}

	void onBreak(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		UISystemPopup.open(UISystemPopup.PopupType.Sell, 
		                   Util.getUIText("MSG_SELL_SKILLRUNE",RareType.WORD[skillInfoData.rare],skillInfoData.skillData.name, skillInfoData.getSellPrice().ToString()),
		                   onConfirmBreak, null, skillInfoData.serverId);
	}

	void onConfirmBreak()
	{
		isEnabled = false;
		SoundData.play("uirn_runemelt");
		EpiServer.instance.sendSellSkillRune(skillInfoData.serverId);
	}



	bool _shootRightNow = false;

	void onShoot()
	{
		_shootRightNow = true;
	}





	public bool isEnabled
	{
		set
		{
			previewMapFile.SetActive(value);

			GameManager.me.stageManager.playTime = 0.0f;
			skillPreviewCamera.gameObject.SetActive(value);
			gameObject.SetActive(value);
			_isEnabled = value;
			isOpen = value;

			if(value == false)
			{
				spSkipModeBackground.gameObject.SetActive(false);

				GameManager.me.inGameGUIGameCamera.enabled = true;
				GameManager.me.inGameGUIPreviewCamera.enabled = false;

				GameManager.me.inGameGUICamera = GameManager.me.inGameGUIGameCamera;

				GameManager.me.player.chargingLevel = 0;
				GameManager.me.player.isEnabled = false;
				GameManager.me.player.container.gameObject.SetActive(value);
				GameManager.me.clearStage();

				movablePanel.parent = mainPanel.transform;
				RuneStudioMain.instance.cam512.gameObject.SetActive(false);

//				challengeItemData = null;

				if(_isOpenSkillBook)
				{
					//GameManager.me.uiManager.popupRuneBook.gameObject.SetActive(true);

				}

				GameManager.me.uiManager.uiMenuCamera.camera.enabled = true;

				if(popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve  || popupType == RuneInfoPopup.Type.Transcend)
				{
					if(RuneStudioMain.instance.type == RuneStudioMain.Type.Reinforce
					   || RuneStudioMain.instance.type == RuneStudioMain.Type.Compose
					   || RuneStudioMain.instance.type == RuneStudioMain.Type.Evolve
					   || RuneStudioMain.instance.type == RuneStudioMain.Type.Transcend)
					{
						RuneStudioMain.instance.endProcess();
					}
				}

				popupType = RuneInfoPopup.Type.Normal;

				iconToolTip.hide();

				--GameManager.me.uiManager.popupReforege.step;

				if(needToShowTutorialEndCircleEffect)
				{
					TutorialManager.uiTutorial.tutorialEndCircleEffect.show();
				}

				needToShowTutorialEndCircleEffect = false;
			}
			else
			{
				GameManager.me.player.isEnabled = true;
			}
		}
		get
		{
			return _isEnabled;
		}
	}


	public void setOtherUserDescription(Player p)
	{
		float useMp = skillInfoData.skillData.mp;
		useMp -= (useMp * p.skillSpDiscount(skillInfoData.skillData));
		useMp = skillInfoData.getTranscendValueByATTR(useMp, WSATTR.MP_I);

		lbSkillMp.text = string.Format("{0:0}",Mathf.CeilToInt(useMp / p.maxMp * 100.0f));  

	}

	void showSkillInfo(HeroSkillData d, int rare)
	{
		if(d == null) return;

		cardFrame.lbName.text = d.name;

		if(_compareData != null && popupType != RuneInfoPopup.Type.Book)
		{
			cardFrame.setLevel( skillInfoData.level , (skillInfoData.level - _compareData.level) );
		}
		else
		{
			cardFrame.setLevel( skillInfoData.level );
		}


		int labelOffset = 0;

		/*
		if(popupType == RuneInfoPopup.Type.Book)
		{
			setReinforceStat(out labelOffset, false,0);
		}
		else
		{

			if(skillInfoData.totalPLevel > 0)
			{
				setReinforceStat(out labelOffset, true,skillInfoData.transcendData.giveExpValue);
			}
			else
			{
				setReinforceStat(out labelOffset, false,0);
			}
		}
		*/

		_sb.Length = 0;
		_sb2.Length = 0;

		float chargingTime = d.getChargingTime(skillInfoData.reinforceLevel);

		lbChargingTime.text = getStatStringValue(chargingTime);

		if(_compareData != null)
		{
			float diffChargingTime = chargingTime - _compareData.skillData.getChargingTime(_compareData.reinforceLevel);
			if((int)diffChargingTime > 0)
			{
				lbChargingTime.text += "[ffe400] (↑"+getStatStringValue(diffChargingTime)+")[-]";
			}
			else if(diffChargingTime < 0)
			{
				lbChargingTime.text += "[be1010] (↓"+getStatStringValue(diffChargingTime)+")[-]";
			}
		}

		// 충전 시간은 초월 없음.

		//* 충전시간 : ( 0~0.3 : 매우빠름 / 0.4~0.7 : 빠름 / 0.8~1 : 보통 / 1.1~1.4 : 느림 / 1.5~ : 매우빠름).
//		if(chargingTime <= 0.3f) lbRuneCharging.text = "매우빠름";
//		else if(chargingTime <= 0.7f) lbRuneCharging.text = "빠름";
//		else if(chargingTime <= 1.0f) lbRuneCharging.text = "보통";
//		else if(chargingTime <= 1.4f) lbRuneCharging.text = "느림";
//		else lbRuneCharging.text = "매우느림"; 



		float useMp = d.mp;
		useMp -= (useMp * GameManager.me.player.skillSpDiscount(d));
		float useMpRate = Mathf.CeilToInt(useMp / GameManager.me.player.maxMp * 100.0f);
		lbSkillMp.text = string.Format("{0:0}",useMpRate);   //getStatStringValue( Mathf.RoundToInt(useMp / GameManager.me.player.maxMp * 100.0f));


		// 비교할 mprate
		if(_compareData != null && compareSkillData != null)
		{
			float cUseMp = compareSkillData.mp;
			cUseMp -= (cUseMp * GameManager.me.player.skillSpDiscount(compareSkillData));
			float useCMpRate = Mathf.CeilToInt(cUseMp / GameManager.me.player.maxMp * 100.0f);

			float diffMpRate = useMpRate - useCMpRate;
			if((int)(diffMpRate*100) > 0)
			{
				lbSkillMp.text += "[ffe400] (↑"+getStatStringValuePoint(diffMpRate)+")[-]";
			}
			else if((diffMpRate*100) < 0)
			{
				lbSkillMp.text += "[be1010] (↓"+getStatStringValuePoint(diffMpRate)+")[-]";
			}
		}

		float coolTime = d.coolTime;
		lbSkillCootime.text =  getStatStringValuePoint(coolTime);


		// 비교할 skill 쿨타임.
		if(_compareData != null && compareSkillData != null)
		{
			float diffCoolTime = coolTime - compareSkillData.coolTime;
			if((int)(diffCoolTime * 100) > 0)
			{
				lbSkillCootime.text += "[ffe400] (↑"+getStatStringValuePoint(diffCoolTime)+")[-]";
			}
			else if(diffCoolTime * 100 < 0)
			{
				lbSkillCootime.text += "[be1010] (↓"+getStatStringValuePoint(diffCoolTime)+")[-]";
			}
		}




		lbSkillDescription.text = skillInfoData.skillData.description;

		_v = spMiddleDescriptionContainer.cachedTransform.localPosition;
		_v.y = lbSkillDescription.cachedTransform.localPosition.y - lbSkillDescription.printedSize.y - 8.0f; 
		spMiddleDescriptionContainer.cachedTransform.localPosition = _v;

		spSkillDescriptionBg.height = (int)(lbSkillDescription.printedSize.y) + 30 + 82 + labelOffset;

		transcendInfo.setData(skillInfoData);

		int len = d.skillEffects.Length;

		int settingIndex = 0;

		int compareReinforceLevel = -1;
		
		if(_compareData != null) compareReinforceLevel = _compareData.reinforceLevel;



		if(transcendInfo.gameObject.activeSelf)
		{
			_v = transcendInfo.transform.localPosition;

			_v.y = spMiddleDescriptionContainer.cachedTransform.localPosition.y - 122.0f + 18f;
			skillDescriptionSlot[settingIndex].transform.localPosition = _v;

			transcendInfo.transform.localPosition = _v;
		}



		for(int i = 0; i < len || i < 5; ++i)
		{
			if(i < len)
			{
				skillDescriptionSlot[i].gameObject.SetActive(true);

//				lbSkillEffect[i].enabled = true;
//				spDescriptionBg[i].enabled = true;

				//확률 + 지속시간 + 효과이름 + 효과양
				
				// 확률
				int chance = d.getSuccessChance(i,skillInfoData.reinforceLevel,rare);

				if(chance <= 0) 
				{
					continue;
				}

				if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[d.skillEffects[i].type].targetText) && 
				   string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[d.skillEffects[i].type].effectText))
				{
					continue;
				}

				int cIndex = 0;
				if(chance < 100)
				{
					if(chance >= 71) cIndex = 1;
					else if(chance >= 40) cIndex = 2;
					else if(chance >= 0) cIndex = 3;
				}

				int checkChanceIndex = cIndex;

				if(compareReinforceLevel > 0)
				{
					chance = compareSkillData.getSuccessChance(i,_compareData.reinforceLevel,_compareData.rare);

					checkChanceIndex = 0;
					if(chance < 100)
					{
						if(chance >= 71) checkChanceIndex = 1;
						else if(chance >= 40) checkChanceIndex = 2;
						else if(chance >= 0) checkChanceIndex = 3;
					}
				}

				if(cIndex > 0)
				{
					if(cIndex != checkChanceIndex)
					{
						_sb.Append("[ffe400]");
					}

					if(cIndex == 1)
					{
						_sb.Append(Util.getUIText("HIGH"));
					}
					else if(cIndex == 2)
					{
						_sb.Append(Util.getUIText("DEFAULT"));
					}
					else if(cIndex == 3)
					{
						_sb.Append(Util.getUIText("LOW"));
					}

					if(cIndex != checkChanceIndex)
					{
						_sb.Append("[-]");
					}

					_sb.Append("  "+Util.getUIText("BY_CHANCE")+" ");
				}


				switch(d.exeData.type)
				{
				case 10:
					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, d.exeData.attr[1], compareReinforceLevel));
					break;
				case 11:
					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, d.exeData.attr[2], compareReinforceLevel));
					break;
				case 12:
					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, d.exeData.attr[1], compareReinforceLevel));
					break;
				case 16:
					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, d.exeData.attr[1], compareReinforceLevel));
					break;
				case 17:
					// TYPE 17. 타임리밋	  데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격
					float tf = (float)d.exeData.attr[6];
					int ti  = (int)(tf * 0.01f); // delay
					int totalCount = (int)(tf - (ti * 100));

					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, d.exeData.attr[1], compareReinforceLevel, totalCount));
					break;
				default:
					_sb.Append(d.skillEffects[i].getEffectDurationString(skillInfoData.reinforceLevel, -1, compareReinforceLevel));
					break;
				}


				_sb.Append("[-]");

				skillDescriptionSlot[settingIndex].lbText.text = _sb.ToString();
				_sb.Length = 0;

				skillDescriptionSlot[settingIndex].spBg.height = Mathf.CeilToInt(skillDescriptionSlot[settingIndex].lbText.printedSize.y + 8);

				if(settingIndex > 0)
				{
					_v = skillDescriptionSlot[settingIndex-1].transform.localPosition;
					_v.y -= skillDescriptionSlot[settingIndex-1].lbText.printedSize.y + 18;
					skillDescriptionSlot[settingIndex].transform.localPosition = _v;
				}
				else
				{
					_v = skillDescriptionSlot[settingIndex].transform.localPosition;
					_v.x = -196;

					if(transcendInfo.gameObject.activeSelf)
					{
						_v.y = transcendInfo.gameObject.transform.localPosition.y -( 230 - 128 );
					}
					else
					{
						_v.y = spMiddleDescriptionContainer.cachedTransform.localPosition.y - 122.0f + 18f;
					}

					skillDescriptionSlot[settingIndex].transform.localPosition = _v;
				}

				skillDescriptionSlot[settingIndex].lbText.enabled = true;
				skillDescriptionSlot[settingIndex].spBg.enabled = true;

				++settingIndex;
			}
			else
			{
				skillDescriptionSlot[i].gameObject.SetActive(false);
			}
		}


		for(int i = settingIndex; i < len || i < 5; ++i)
		{
			skillDescriptionSlot[i].lbText.enabled = false;
			skillDescriptionSlot[i].spBg.enabled = false;
		}



		switch(d.targeting)
		{
		case 0:
			spIconTargetingType.spriteName = "icn_skill_target_all";
			strIconDescription[1] = Util.getUIText("TTYPE_ALL");
			break;
		case 1:
			spIconTargetingType.spriteName = "icn_skill_target_front";
			strIconDescription[1] = Util.getUIText("TTYPE_FRONT");
			break;
		case 2:
			spIconTargetingType.spriteName = "icn_skill_target_appoint";

			if(d.targetAttr[0] == TargetingData.AUTOMATIC_CLOSE_TARGET_1)
			{
				strIconDescription[1] = Util.getUIText("TTYPE_APPOINT");
			}
			else
			{
				strIconDescription[1] = Util.getUIText("TTYPE_LOWHP");
			}
			break;
		case 3:
			spIconTargetingType.spriteName = "icn_skill_target_frontline";
			strIconDescription[1] = Util.getUIText("TTYPE_FRONTLINE");
			break;
		}


		switch(d.skillType)
		{
		case Skill.Type.ATTACK:
			spIconSkillType.spriteName = "icn_skill_character_attack";
			strIconDescription[0] = Util.getUIText("STYPE_ATK");
			break;
		case Skill.Type.BUFF:
			spIconSkillType.spriteName = "icn_skill_character_buff";
			strIconDescription[0] = Util.getUIText("STYPE_BUFF");
			break;
		case Skill.Type.DEBUFF:
			spIconSkillType.spriteName = "icn_skill_character_debuff";
			strIconDescription[0] = Util.getUIText("STYPE_DEBUFF");
			break;
		case Skill.Type.HEAL:
			spIconSkillType.spriteName = "icn_skill_character_heal";
			strIconDescription[0] = Util.getUIText("STYPE_HEAL");
			break;
		}


		for(int i = 0; i < 2; ++i)
		{
			lbIconDescription[i].text = strIconDescription[i];
		}

//		strIconDescription[2] = "";
//		_v = btnIconSkillType.gameObject.transform.localPosition;
//		_v.x = -89.0f;
//		btnIconSkillType.gameObject.transform.localPosition = _v;
//		
//		_v = btnTargetingType.gameObject.transform.localPosition;
//		_v.x = 72;
//		btnTargetingType.gameObject.transform.localPosition = _v;


	}


	public  static string getStatStringValue(float inputValue)
	{
		if(Mathf.Abs(inputValue) >= 10)
		{
			return string.Format("{0:0}",inputValue);

		}
		else
		{
			return string.Format("{0:0.0}",inputValue);
		}
	}

	public  static string getStatStringValuePoint(float inputValue)
	{
		float diff = 0;

		if(inputValue > 0)
		{
			diff = inputValue - (int)inputValue;
		}
		else if(inputValue < 0)
		{
			diff = inputValue + (int)inputValue;
		}

		if( Mathf.Abs(diff * 100) > 0)
		{
			inputValue = (Mathf.Round(inputValue * 100))*0.01f;
			return string.Format("{0:0.0}",inputValue);
		}
		else
		{
			return string.Format("{0:0}",inputValue);
		}
	}



	public GameIDData skillInfoData = new GameIDData();
	public HeroSkillData skillData = new HeroSkillData();
	public HeroSkillData compareSkillData = new HeroSkillData();


	public void showMakeResult(GameIDData newSkillInfo)
	{
		canPlaySkill = false;
		RuneStudioMain.instance.spSkillIcon.cachedGameObject.transform.parent.gameObject.SetActive(true);
		show (newSkillInfo, RuneInfoPopup.Type.Make, false);
	}


//	public void showInstantBuy(P_ChallengeItem cItem, GameIDData newSkillInfo)
//	{
//		challengeItemData = cItem;
//		show (newSkillInfo, Type.InstantBuy, false);
//		btnInstantBuy.gameObject.SetActive(true);
//		lbInstantBuyPrice.text = cItem.price.ToString();
//	}

	List<GameObject> _activeButtons = new List<GameObject>();
	private GameIDData _compareData = null;

	public bool isJackpotMode = false;

	// isFirstSequence는 강화때 처음에는 프리뷰 화면이 재생되지 않게하기 위해 만든 변수다...
	public void show(GameIDData infoData, RuneInfoPopup.Type type, bool isInventorySlot, bool isFirstSequence = true, GamePlayerData gpd = null, GameIDData compareData = null)
	{
		needToShowTutorialEndCircleEffect = false;

		isJackpotMode = false;

		spBackground.gameObject.SetActive(true);

		spSkipModeBackground.gameObject.SetActive(false);

		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false)
			#endif
		{
			if(gpd == null)
			{
				GamePlayerData nowPlayerData = GameDataManager.selectedPlayerData;
				GameManager.me.changeMainPlayer(nowPlayerData,nowPlayerData.id,nowPlayerData.partsVehicle.parts.resource.ToUpper(), true);
			}
			else
			{
				GameManager.me.changeMainPlayer(gpd,gpd.id,gpd.partsVehicle.parts.resource.ToUpper(), true);
			}
		}

		gameObject.SetActive(false);

		skillInfoData.parse(infoData.serverId);
		_compareData = compareData;

		skillData = skillInfoData.skillData.clone(skillData);
		if(skillInfoData.totalPLevel > 0) skillInfoData.transcendData.apply(skillData, skillInfoData.transcendLevel);
		skillData.exeData.init(AttackData.AttackerType.Hero, AttackData.AttackType.Skill, skillData, skillInfoData.transcendData, skillInfoData.transcendLevel);

		if(_compareData != null)
		{
			compareSkillData = _compareData.skillData.clone(compareSkillData);
			if(_compareData.totalPLevel > 0) _compareData.transcendData.apply(compareSkillData, _compareData.transcendLevel);
		}


		isFromInventorySlot = isInventorySlot;

		// 부하를 줄이기위해 쓸데없는 렌더링은 끈다.
		_isOpenSkillBook = GameManager.me.uiManager.popupRuneBook.gameObject.activeSelf;
		//GameManager.me.uiManager.popupRuneBook.gameObject.SetActive(false);
		if(_isOpenSkillBook) GameManager.me.uiManager.uiMenuCamera.camera.enabled = false;

		popupType = type;

		movablePanel.localScale = Vector3.one;

		SoundData.play("uicm_popup_mid");
		
		movablePanel.localPosition = new Vector3(-3000,0,0);
		
		spBackground.color = new Color(0,0,0,100.0f/255.0f);

		showSkillInfo(skillData, skillInfoData.rare);

		if(popupType == RuneInfoPopup.Type.Make || popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend) setNowSkillIcon(); // 커버.

		gameObject.SetActive (true);

		btnClose.gameObject.SetActive(type != RuneInfoPopup.Type.Reinforce || isFirstSequence == false);

		cardFrame.showFrame(skillInfoData.rare);
		cardFrame.setTranscendLevel(skillInfoData);

		setButtons(isFirstSequence);

		visibleComposeNoticeMark();

		descriptionContainer.SetActive(popupType != RuneInfoPopup.Type.Make);
		cardFrame.showDescriptionPanel(popupType != RuneInfoPopup.Type.Make, (popupType == RuneInfoPopup.Type.Make || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend));

		showProcess2();

		if(popupType != RuneInfoPopup.Type.Make)
		{
			if(popupType == RuneInfoPopup.Type.Reinforce)
			{
				if(isFirstSequence == false) initPlaySkillScene();
			}
			else
			{
				initPlaySkillScene();
			}
		}

		isEnabled = true;

		if(popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend)
		{
			RuneStudioMain.instance.cam256.gameObject.SetActive(true);
			
			cardFrame.goMakeSkillIconCover.SetActive(true);
			cardFrame.goMakeSkillIconCover.animation.Stop();
			cardFrame.goMakeSkillIconCover.transform.localScale = new Vector3(0.93454f, 1.666676f, 0.9402466f);
		}
		else if(popupType != RuneInfoPopup.Type.Make)
		{
			StartCoroutine(playSkill());
		}

		reinforceInfoBar.setReinforceData(skillInfoData);
	}


	void setButtons(bool isFirstSequence)
	{
		_activeButtons.Clear();

		if(popupType == RuneInfoPopup.Type.Reinforce && isFirstSequence)
		{
			btnInstantBuy.gameObject.SetActive(false);
			
			btnBreak.gameObject.SetActive(false);
			btnPutOn.gameObject.SetActive(false);
			btnPutOff.gameObject.SetActive(false);
			btnReinfoce.gameObject.SetActive(false);
			btnEvolution.gameObject.SetActive(false);
			btnCompose.gameObject.SetActive(false);
			btnReforge.gameObject.SetActive(false);
			goCannotWear.SetActive(false);

		}
		else if(popupType == RuneInfoPopup.Type.Book || popupType == RuneInfoPopup.Type.PreviewOnly || popupType == RuneInfoPopup.Type.Make)
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
		else 
		{
			btnInstantBuy.gameObject.SetActive(false);

			btnBreak.gameObject.SetActive(true);

			// 탭에서 온 녀석.
			if(isFromInventorySlot == false)
			{
				btnBreak.isEnabled = false;
				btnPutOn.gameObject.SetActive(false);
				btnPutOff.gameObject.SetActive(true);
				btnReinfoce.gameObject.SetActive(true);

				goCannotWear.SetActive(false);

			}
			// 일반으로 온 녀석.
			else
			{
				btnBreak.isEnabled = true;

				btnPutOff.gameObject.SetActive(false);
				btnReinfoce.gameObject.SetActive(true);


				if(GameManager.me.uiManager.uiMenu.uiSkill.checkCanPutOn(skillInfoData.baseId))
				{
					btnPutOn.gameObject.SetActive(true);
					goCannotWear.SetActive(false);
				}
				else
				{
					goCannotWear.SetActive(true);
					btnPutOn.gameObject.SetActive(false);

					lbWearCharacter.text = Util.getUIText("STR_USE_THIS", Util.getUIText( GameManager.me.uiManager.uiMenu.uiSkill.lastCheckPutOnCharacter ) );
				}
			}


			btnReinfoce.gameObject.SetActive(true);

			if (skillInfoData.canReforge())
			{
//				btnReinfoce.isEnabled = false;
				btnReforge.gameObject.SetActive(true);
			}
			else
			{
//				btnReinfoce.isEnabled = true;
				btnReforge.gameObject.SetActive(false);
			}


			btnEvolution.gameObject.SetActive(skillInfoData.rare == RareType.S);
			
			btnCompose.gameObject.SetActive(skillInfoData.rare < RareType.S);

		}
		
		
		
		_v.y = -251f;
		_v.z = 295f;
		

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

	}



	public void reinforcePlayScene()
	{
		cardFrame.goMakeSkillIconCover.animation.Play();
		initPlaySkillScene();
		canPlaySkill = true;
		StartCoroutine(playSkill());
	}


	void initPlaySkillScene()
	{
		GameManager.me.uiManager.uiPlay.resetCamera();
		CharacterAttachedUI.gameViewCamera = skillPreviewCamera;
		
		GameManager.me.inGameGUIGameCamera.enabled = false;
		GameManager.me.inGameGUIPreviewCamera.enabled = true;
		
		GameManager.me.inGameGUICamera = GameManager.me.inGameGUIPreviewCamera;
		
		_shootRightNow = false;

		GameManager.me.player.container.gameObject.SetActive(true);
		GameManager.me.player.cTransform.gameObject.SetActive(true);
		GameManager.me.player.pet.cTransform.gameObject.SetActive(true);
		
		GameManager.me.stageManager.setDemoRound();
		
		GameManager.me.player.resetPosition(false);
		GameManager.me.player.hp = 100;
		GameManager.me.player.chargingLevel = 0;

		if(GameManager.me.player.hpBar != null) GameManager.me.player.hpBar.visible = false;
		
		if(skillData.exeData.type == 12) GameManager.me.player.setPositionCtransform(new Vector3(500,0,0));
		
		GameManager.me.player.lookDirection(1000);

		GameManager.me.characterManager.inGameGUIContinaer.gameObject.SetActive(true);

	}



	public void setNowSkillIcon()
	{
		setSkillIcon(skillInfoData.getSkillIcon(), skillInfoData.rare);
	}

	public void setSkillIcon(string iconResource, int iconRare)
	{
		Icon.setSkillIcon(iconResource, RuneStudioMain.instance.spSkillIcon);


		RuneStudioMain.instance.spSkillIcon.gameObject.SetActive(true);
		RuneStudioMain.instance.spSkillIconFrame.gameObject.SetActive(true);

		switch(iconRare)
		{
		case RareType.D:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_d";
			break;
		case RareType.C:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_c";
			break;
		case RareType.B:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_b";
			break;
		case RareType.A:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_a";
			break;
		case RareType.S:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_s";
			break;
		case RareType.SS:
			RuneStudioMain.instance.spSkillIconFrame.spriteName = "img_cardskill_ss";
			break;
		}

		if(iconRare > RareType.SS && iconRare < 50)
		{
			iconRare = RareType.SS;
		}

		for(int i = 0; i < 6; ++i)
		{
			RuneStudioMain.instance.skillIconRareBg[i].SetActive(i == iconRare);
		}
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






	Monster addMonster(bool isPlayerMonster, string unitId, Vector3 pos)
	{
		Monster mon = GameManager.me.mapManager.addMonsterToStage(null, null, isPlayerMonster,null,unitId,pos);
		mon.hp = 999999;

		GameManager.me.characterManager.setHpBar(mon.hpBar);
		mon.hpBar = null;
		mon.setIdleAndFreeze();
		mon.isFreeze.Set( false );
		return mon;
	}


	void setTarget(Monster mon)
	{
		GameManager.me.player.setSkillTarget(mon);
		GameManager.me.player.targetPosition = mon.cTransformPosition;
		GameManager.me.player.targetHeight = mon.getHitObject().height;
		GameManager.me.player.targetUniqueId = mon.stat.uniqueId;
	}


	IEnumerator playSkill()
	{

		HeroSkillData hsd = skillData;

		bool isRangeType = (!((hsd.exeData.type == 0 && hsd.targeting == 2) || hsd.exeData.type == 3));

		bool isChangeSideSkill = false;

		if(hsd.skillEffects != null)
		{
			int len = hsd.skillEffects.Length;

			for(int i = 0; i < len ; ++i)
			{
				if(hsd.isChangeSideSkill)
				{
					isChangeSideSkill = true;
					break;
				}
			}
		}



		_v = GameManager.me.player.cTransformPosition;

		// 적에게 쏜다.
		if(hsd.targetType == Skill.TargetType.ENEMY)
		{
			// center
			_v.x = 840.0f;
			_v.z = 0.0f;
			
			if(isRangeType)
			{
				setTarget(addMonster(false,"EU"+UnityEngine.Random.Range(1,3),_v));
				
				//left
				_v.x = 800.0f;
				_v.z = -90.0f;
				
				addMonster(false,"EU"+UnityEngine.Random.Range(1,3),_v);
				
				//right
				_v.x = 750.0f;
				_v.z = 90.0f;
				
				Monster mon = addMonster(false,"EU"+UnityEngine.Random.Range(1,3),_v);
				if(hsd.exeData.type == 15) setTarget(mon);
			}
			else
			{
				setTarget(addMonster(false,"EU"+UnityEngine.Random.Range(1,3),_v));
			}
		}
		// 우리편에게 쏜다.
		else
		{
			
			// center
			_v.x = 840.0f;
			_v.z = 0.0f;
			
			Monster mon;
			
			if(isRangeType)
			{
				if(GameDataManager.instance.unitSlots[0].isOpen.Get() == false)
				{
					GameDataManager.instance.addLoadModelData( GameManager.info.monsterData[GameManager.info.unitData["UN1"].resource]);
					GameDataManager.instance.startModelLoad();
					while(GameDataManager.instance.isCompleteLoadModel == false){ yield return null; };
					mon = addMonster(true,"UN1",_v);
				}
				else mon = addMonster(true,GameDataManager.instance.unitSlots[0].unitData.id,_v);
				
				setTarget(mon);
				//left
				_v.x = 800.0f;
				_v.z = -90.0f;
				
				if(GameDataManager.instance.unitSlots[1].isOpen.Get() == false)
				{
					GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData["UN2"].resource]);
					GameDataManager.instance.startModelLoad();
					while(GameDataManager.instance.isCompleteLoadModel == false){ yield return null; };
					mon = addMonster(true,"UN2",_v);
				}
				else addMonster(true,GameDataManager.instance.unitSlots[1].unitData.id,_v);
				
				//right
				_v.x = 750.0f;
				_v.z = 90.0f;
				
				
				if(GameDataManager.instance.unitSlots[2].isOpen.Get() == false)
				{
					GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData["UN3"].resource]);
					GameDataManager.instance.startModelLoad();
					while(GameDataManager.instance.isCompleteLoadModel == false){ yield return null; };
					mon = addMonster(true,"UN3",_v);
				}
				else mon = addMonster(true,GameDataManager.instance.unitSlots[2].unitData.id,_v);
				
				if(hsd.exeData.type == 15) setTarget(mon);
			}
			else
			{
				if(GameDataManager.instance.unitSlots[0].isOpen.Get() == false)
				{
					GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData["UN1"].resource]);
					GameDataManager.instance.startModelLoad();
					while(GameDataManager.instance.isCompleteLoadModel == false){ yield return null; };
					mon = addMonster(true,"UN1",_v);
				}
				else mon = addMonster(true,GameDataManager.instance.unitSlots[0].unitData.id,_v);
				
				setTarget(mon);
			}
		}


		while(canPlaySkill == false && (popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend) )
		{
			yield return null;
		}

		if(popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend)
		{
			yield return Util.ws1;
		}


		GameManager.me.effectManager.addDefaultEffect();
		GameManager.me.effectManager.loadEffectFromHeroSkillData(hsd);
		GameManager.me.effectManager.startLoadEffects(true);
		
		while(GameManager.me.effectManager.isCompleteLoadEffect == false){ yield return null; };



		while(true)
		{
			Monster target = GameManager.me.player.skillTarget;

//			Debug.Log("1");

			yield return Util.ws05;

//			Debug.Log("2");
			switch(hsd.targeting)
			{
			case 0:
				break;
			case 1:
				GameManager.me.player.targetingDecal.init(TargetingDecal.DecalType.Circle, hsd.exeData.targetRange * 0.005f);
				GameManager.me.player.targetingDecal.setPosition(GameManager.me.player.targetPosition);
				break;
			case 2:
				GameManager.me.player.targetingDecal.init(TargetingDecal.DecalType.Circle, GameManager.me.player.skillTarget.summonEffectSize * 0.5f );
				GameManager.me.player.targetingDecal.setPosition(GameManager.me.player.targetPosition);
				break;
			default:
				GameManager.me.player.targetingDecal.init(TargetingDecal.DecalType.Arrow);
				GameManager.me.player.targetingDecal.setPosition(GameManager.me.player.cTransformPosition);
				break;
			}

//			Debug.Log("3");
			_shootRightNow = false;

			yield return Util.ws05;

			GameManager.me.player.maxSp = 99999;
			GameManager.me.player.maxMp = 99999;
			GameManager.me.player.sp = 99999;
			GameManager.me.player.mp = 99999;


			GameManager.me.player.chargingGauge.visible = true;
			GameManager.me.player.startCharging(hsd, hsd.getChargingTime(skillInfoData.reinforceLevel), skillInfoData);
			_chargingStartTime = GameManager.me.stageManager.playTime;


			while(true)
			{
//				Debug.Log("5");
				yield return null;
//				Debug.Log("6");
				GameManager.me.player.updateChargingEffect();
				if(_shootRightNow) break;
				if(checkCharging(GameManager.me.player))
				{
					break;
				}
			}

//			Debug.Log("7");
			yield return Util.ws1;

//			Debug.Log("8");
			GameManager.me.player.nowBulletPatternId = hsd.resource;
			hsd.exeData.playSkill(GameManager.me.player, skillInfoData.reinforceLevel + hsd.baseLevel, GameManager.me.player.applyReinforceLevel);
			
			GameManager.me.player.chargingLevel = 0;
			GameManager.me.player.targetingDecal.hide();
			GameManager.me.player.chargingGauge.visible = false;
//			Debug.Log("9");
			yield return new WaitForSeconds(2.0f);

//			Debug.Log("10");
			while(GameManager.me.player.state != Monster.NORMAL || GameManager.me.bulletManager.playerBulletList.Count != 0)
			{
//				Debug.Log("11");
				yield return Util.ws05;
			}


			if(isChangeSideSkill)
			{
				if(target.isPlayerSide == false)
				{
					yield return new WaitForSeconds(0.2f);
				}

				target.lookDirection(1000);

				yield return new WaitForSeconds(4.0f);
				target.skillPreviewDead();
				yield return Util.ws1;
				setTarget(addMonster(false,"EU"+UnityEngine.Random.Range(1,3),_v));
			}
			else
			{
				setTarget(target);
			}
		}


		yield return null;
	}

	float _chargingStartTime = 0.0f;
	private bool checkCharging(Player player)
	{
		return player.onCharging();
	}


	public static Xbool isOpen = false;

	void OnEnable()
	{
		if(GameManager.me != null)
		{
			GameManager.me.isPaused = false;
//			isOpen = true;
			CharacterAttachedUI.gameViewCamera = skillPreviewCamera;
		}
	}

	void OnDisable()
	{
		SoundData.play("uicm_close_mid");

//		isOpen = false;
		//GameManager.me.isPlaying = false;
		if(GameManager.me != null && 
		   GameManager.me.player != null &&
		   GameManager.me.player.chargingGauge != null)
		{
			GameManager.me.player.chargingGauge.visible = false;
		}

	}






	public UIPopupCardDetailReinforcePanel reinforceInfoBar;
	public UIButton btnReinfoce;
	public UIButton btnReforge;

	void onClickReinforce(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		if(skillInfoData.reinforceLevel >= GameIDData.MAX_LEVEL)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REINFORCE_NOMORE"));
			return;
		}



		isEnabled = false;
		GameManager.me.uiManager.uiMenu.uiSkill.startReinforceMode(GameIDData.getClone(skillInfoData), isFromInventorySlot);
	}


	void onClickReforge(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		
		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}
		
		isEnabled = false;
		
		GameManager.me.uiManager.popupReforege.show(skillInfoData.serverId, !isFromInventorySlot);
	}



	void onClickEvolution(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		
		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}
		
		if(skillInfoData.rare >= RareType.SS)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("EVOLVE_NOMORE"));
			return;
		}
		else if(skillInfoData.rare != RareType.S || skillInfoData.reinforceLevel < 20)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("EVOLVE_LEVELLIMIT"));
			return;
		}
		
		
		GameManager.me.uiManager.popupEvolution.show(Rune.Type.Skill, skillInfoData.serverId, (isFromInventorySlot)?WSDefine.NO:WSDefine.YES);

	}


	void onClickCompose(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		if(popupType == RuneInfoPopup.Type.Reinforce && RuneStudioMain.instance.step != RuneStudioMain.Step.Finish)
		{
			Debug.Log("wait...");
			return;
		}

		if(skillInfoData.rare >= RareType.S)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("COMPOSE_NOMORE"));
			return;
		}
		else if(skillInfoData.reinforceLevel < 20)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("COMPOSE_LEVELLIMIT"));
			return;
		}
		else
		{
			int composeSourceNumber = 0;
			
			for(int i = 0; i < GameDataManager.instance.skillInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.skillInventoryList[i].rare == skillInfoData.rare
				   && GameDataManager.instance.skillInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
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

				//합성 재료가 없습니다.\n합성 재료로 O급 20레벨 소환룬(스킬룬)이 필요합니다.

				if(GameDataManager.instance.playerSkillSlots != null)
				{
					foreach(KeyValuePair<string, PlayerSkillSlot[]> kv in GameDataManager.instance.playerSkillSlots)
					{
						if( GameDataManager.instance.serverHeroData.ContainsKey( kv.Key ) )
						{
							foreach(PlayerSkillSlot ps in kv.Value)
							{
								if(ps.isOpen == false || ps.infoData == null) continue;
								
								if(ps.infoData.rare == skillInfoData.rare &&
								   ps.infoData.reinforceLevel == GameIDData.MAX_LEVEL)
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




				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NO_COMPOSE_SKILL",RareType.CHARACTER[skillInfoData.rare]) );
				return;
			}
		}


		isEnabled = false;
		GameManager.me.uiManager.uiMenu.uiSkill.startComposeMode(GameIDData.getClone( skillInfoData), isFromInventorySlot);
	}






	void visibleComposeNoticeMark()
	{
		bool visible = true;
		
		if(skillInfoData.rare >= RareType.S)
		{
			visible = false;
		}
		else if(skillInfoData.reinforceLevel < 20)
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
			
			for(int i = 0; i < GameDataManager.instance.skillInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.skillInventoryList[i].rare == skillInfoData.rare
				   && GameDataManager.instance.skillInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
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
				
				//합성 재료가 없습니다.\n합성 재료로 O급 20레벨 소환룬(스킬룬)이 필요합니다.
				if(GameDataManager.instance.skills != null)
				{
					foreach(PlayerSkillSlot ps in GameDataManager.instance.skills)
					{
						if(ps.isOpen == false || ps.infoData == null) continue;
						
						if(ps.infoData.rare == skillInfoData.rare &&
						   ps.infoData.reinforceLevel == GameIDData.MAX_LEVEL)
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
				
				
				visible = false;
			}
		}
		
		spComposeNotice.enabled = visible;
	}




















}
