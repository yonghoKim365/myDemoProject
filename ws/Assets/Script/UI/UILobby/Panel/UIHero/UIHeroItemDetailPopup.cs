using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIHeroItemDetailPopup : UIPopupBase 
{
	public UIPanelRefresher[] panelRefresher;

	public RuneInfoPopup.Type popupType;

	Quaternion _q;

	public UICharacterRotate characterRotate;

	public SimpleRotater simpleRotater;

	public UIPopupcardDetailReforgePanel transcendInfo;


	[HideInInspector] public Player sampleHero;

	public UILabel lbItemDescriptionName, lbItemDescriptionStat, lbItemDescriptionMaxStat, lbItemDescriptionStat2, lbSuperRareLegendDescription, lbInstantBuyPrice;

	public UISprite spDescriptionBg, spDescriptionBg2;

	public Transform sampleContainer;

	public UIScrollView scrollView;

	public GameObject mainPanel;
	public Transform movablePanel;

	public UIButton btnSell, btnWear, btnInteractive, btnInstantBuy;



	public UICardFrame cardFrame;
	public GameObject descriptionContainer;

	public GameObject goRareLegendPartsDescriptionContainer;

	public UISprite spBackground;

	public UISprite spSkipModeBackground;

	public UITabButton[] tabSGradeInfo;

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnSell.gameObject).onClick = onClickBtnSell;
		UIEventListener.Get(btnWear.gameObject).onClick = onClickBtnWear;
		UIEventListener.Get(btnInteractive.gameObject).onPress = onPressInteractive;


		UIEventListener.Get(btnReinfoce.gameObject).onClick = onClickReinforce;
		UIEventListener.Get(btnCompose.gameObject).onClick = onClickCompose;
		UIEventListener.Get(btnEvolution.gameObject).onClick = onClickEvolution;
		UIEventListener.Get(btnReforge.gameObject).onClick = onClickReforge;

		UIEventListener.Get(tabSGradeInfo[0].gameObject).onClick = onClickSGrade1;
		UIEventListener.Get(tabSGradeInfo[1].gameObject).onClick = onClickSGrade2;
		UIEventListener.Get(tabSGradeInfo[2].gameObject).onClick = onClickSGrade3;

	}


	public void hideAllButton()
	{
		btnSell.gameObject.SetActive(false);
		btnWear.gameObject.SetActive(false);
		btnInstantBuy.gameObject.SetActive(false);
		btnReinfoce.gameObject.SetActive(false);
		btnCompose.gameObject.SetActive(false);
		btnEvolution.gameObject.SetActive(false);
		btnReforge.gameObject.SetActive (false);
	}



	void onClickSGrade1(GameObject go)
	{
		if(tabSGradeInfo[0].isEnabled) return;
		setSGradeInfo(0);
		
		data.parse( data.getEquipBookId("1") );
		
		setStat(true, data);

		resizeDescriptionBg();
	}
	
	void onClickSGrade2(GameObject go)
	{
		if(tabSGradeInfo[1].isEnabled) return;
		setSGradeInfo(1);
		
		data.parse( data.getEquipBookId("2") );
		
		setStat(true, data);

		resizeDescriptionBg();
	}
	
	void onClickSGrade3(GameObject go)
	{
		if(tabSGradeInfo[2].isEnabled) return;
		setSGradeInfo(2);
		
		data.parse( data.getEquipBookId("3") );
		
		setStat(true, data);

		resizeDescriptionBg();
	}
	
	
	void setSGradeInfo(int index)
	{
		for(int i = 0; i < 3; ++i)
		{
			tabSGradeInfo[i].isEnabled = (i == index);
		}
	}



	public void onClickBtnSell(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		UISystemPopup.open(UISystemPopup.PopupType.Sell, 
		                   Util.getUIText("MSG_SELL_EQUIP",RareType.WORD[data.rare],data.partsData.name,data.getSellPrice().ToString()), 
		                   onConfirmSell, null, data.serverId);
	}

	void onConfirmSell()
	{
		hide ();
		EpiServer.instance.sendSellEquip(data.serverId);
	}



	public void onClickBtnWear(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;
		switch(data.partsData.type)
		{
		case HeroParts.HEAD:
			SoundData.play("uihe_attach_hat");
			break;
		case HeroParts.BODY:
			SoundData.play("uihe_attach_amr");
			break;
		case HeroParts.WEAPON:
			SoundData.play("uihe_attach_wpn");
			break;
		case HeroParts.VEHICLE:
			SoundData.play("uihe_attach_pet");
			break;
		}

		hide ();
		EpiServer.instance.sendChangeEquip(data.serverId);
	}

	public GameIDData data = new GameIDData();


	private GamePlayerData _gpd = new GamePlayerData(Character.LEO);



	private string _rareDescription = "";
	private string _legendDescription = "";

	void showFrame()
	{
		cardFrame.showFrame(data.rare);
		cardFrame.setTranscendLevel(data);
	}



	public bool isFromInventorySlot = false;
	List<GameObject> _activeButtons = new List<GameObject>();

	private GameIDData _compareData = null;


	public void show(GameIDData d, RuneInfoPopup.Type type, bool isInventorySlot, bool isFirstSequenceForReinforce = true, GamePlayerData gpd = null, GameIDData compareData = null)
	{
		

		spBackground.gameObject.SetActive(true);

		spSkipModeBackground.gameObject.SetActive(false);

		popupType = type;

		simpleRotater.enabled = false;

		isFromInventorySlot = isInventorySlot;

		SoundData.play("uihe_itempopup");

		movablePanel.localPosition = new Vector3(-3000,0,0);


		if(type == RuneInfoPopup.Type.Reinforce || type == RuneInfoPopup.Type.Make || type == RuneInfoPopup.Type.Compose || type == RuneInfoPopup.Type.Evolve || type == RuneInfoPopup.Type.Transcend)
		{
			spBackground.color = new Color(0,0,0,1f/255f);
		}
		else
		{
			spBackground.color = new Color(0,0,0,100f/255f);
		}


		RuneStudioMain.instance.cam512.gameObject.SetActive(true);

		data.parse( d.serverId);

		_compareData = compareData;

		showFrame();

		setCharacter(gpd);

		descriptionContainer.SetActive(popupType != RuneInfoPopup.Type.Make);
		cardFrame.showDescriptionPanel(popupType != RuneInfoPopup.Type.Make, true);




		cardFrame.lbName.text = data.partsData.name;

		if(_compareData != null && popupType != RuneInfoPopup.Type.Book)
		{
			cardFrame.setLevel( data.level , (data.level - _compareData.level) );
		}
		else
		{
			cardFrame.setLevel( data.level );
		}


		gameObject.SetActive(true);

		btnClose.gameObject.SetActive(type != RuneInfoPopup.Type.Reinforce || isFirstSequenceForReinforce == false);

		setButtons(isFirstSequenceForReinforce);

		visibleComposeNoticeMark();


		tabSGradeInfo[0].transform.parent.gameObject.SetActive(type == RuneInfoPopup.Type.Book && (data.rare >= RareType.S));
		
		setSGradeInfo(0);

		setStat(isFirstSequenceForReinforce, data);

		if(sampleHero != null)
		{
			sampleHero.shadow.gameObject.SetActive(false);
			if(sampleHero.pet != null) sampleHero.pet.shadow.gameObject.SetActive(false);
		}

		reinforceInfoBar.setReinforceData(data);

		showProcess2();
	}



	void showProcess2()
	{
		resizeDescriptionBg();

		scrollView.ResetPosition();

		if(popupType == RuneInfoPopup.Type.Make)
		{
			movablePanel.parent = RuneStudioMain.instance.moveablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Reinforce)
		{
			movablePanel.parent = RuneStudioMain.instance.reinforceMoveablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Compose)
		{
			movablePanel.parent = RuneStudioMain.instance.composeMovablePanelParent;
		}
		else if(popupType == RuneInfoPopup.Type.Evolve)
		{
			movablePanel.parent = RuneStudioMain.instance.evolveMovablePanelParent[4];
		}
		else if(popupType == RuneInfoPopup.Type.Transcend)
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


	void resizeDescriptionBg()
	{
		spDescriptionBg.height = Mathf.CeilToInt(lbItemDescriptionName.printedSize.y + 74 + _bgHeightOffset);
		
		if(goRareLegendPartsDescriptionContainer.activeSelf)
		{
			if(transcendInfo.gameObject.activeSelf == false)
			{
				_v = spDescriptionBg.cachedTransform.localPosition;
				_v.x = 0.0f;
				_v.y -= 159;
				_v.y -= spDescriptionBg.height + 10.0f;
				goRareLegendPartsDescriptionContainer.transform.localPosition = _v;

			}
			else
			{
				_v = transcendInfo.transform.localPosition;
				_v.y = spDescriptionBg.cachedTransform.localPosition.y - spDescriptionBg.height - 20.0f;
				transcendInfo.transform.localPosition = _v;

				_v = goRareLegendPartsDescriptionContainer.transform.localPosition;
				_v.y = transcendInfo.transform.localPosition.y - 100 - (267-113);
				goRareLegendPartsDescriptionContainer.transform.localPosition = _v;
			}
			spDescriptionBg2.height = Mathf.CeilToInt(lbSuperRareLegendDescription.printedSize.y + 20);
		}
		else
		{
			_v = transcendInfo.transform.localPosition;
			_v.y = spDescriptionBg.cachedTransform.localPosition.y - spDescriptionBg.height + 102 - 126;
			transcendInfo.transform.localPosition = _v;
		}
	}






	void setButtons(bool isFirstSequenceForReinforce)
	{
		_activeButtons.Clear();

		if(popupType == RuneInfoPopup.Type.Reinforce && isFirstSequenceForReinforce)
		{
			btnWear.gameObject.SetActive(false);
			btnSell.gameObject.SetActive(false);
			btnReinfoce.gameObject.SetActive(false);
			btnInstantBuy.gameObject.SetActive(false);
			btnCompose.gameObject.SetActive(false);
			btnEvolution.gameObject.SetActive(false);
			btnReforge.gameObject.SetActive (false);

		}
		else if(popupType == RuneInfoPopup.Type.Book || popupType == RuneInfoPopup.Type.PreviewOnly || popupType == RuneInfoPopup.Type.Make)
		{
			btnWear.gameObject.SetActive(false);
			btnSell.gameObject.SetActive(false);
			btnReinfoce.gameObject.SetActive(false);
			btnCompose.gameObject.SetActive(false);
			btnInstantBuy.gameObject.SetActive(false);
			btnEvolution.gameObject.SetActive(false);
			btnReforge.gameObject.SetActive (false);
		}
		else //Normal, Reinforce
		{
			btnInstantBuy.gameObject.SetActive(false);

			btnWear.gameObject.SetActive(true);
			btnSell.gameObject.SetActive(true);

			// 탭에서 온 녀석.
			if(isFromInventorySlot == false)
			{
				btnWear.isEnabled = false;
				btnSell.isEnabled = false;
			}
			// 일반으로 온 녀석.
			else
			{
				btnWear.isEnabled = (data.partsData.character == GameManager.me.uiManager.uiMenu.uiHero.tabPlayer.currentTab);//GameDataManager.instance.selectHeroId);
				btnSell.isEnabled = true;
			}

			btnReinfoce.gameObject.SetActive(true);

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
		
		_v = btnWear.transform.localPosition;



		if(btnWear.gameObject.activeSelf) _activeButtons.Add(btnWear.gameObject);
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











	void setCharacter(GamePlayerData gpd = null)
	{
		if(sampleHero != null)
		{
			GameManager.me.characterManager.cleanMonster(sampleHero);
			sampleHero = null;
		}
		
		
		sampleHero = (Player)GameManager.me.characterManager.getMonster(true,true,data.partsData.character,false);


		if(gpd != null)
		{
			gpd.copyTo(_gpd);
		}
		else if(GameDataManager.instance.heroes.ContainsKey(data.partsData.character))
		{
			GameDataManager.instance.heroes[data.partsData.character].copyTo(_gpd);
		}
		else
		{
			GameDataManager.instance.defaultHeroData[data.partsData.character].copyTo(_gpd);
		}

		
		if(data.partsData.type == HeroParts.WEAPON)
		{
			_gpd.partsWeapon = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 20.0f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-70,-575);
			_q.eulerAngles = new Vector3(0,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
		}
		else if(data.partsData.type == HeroParts.HEAD)
		{
			_gpd.partsHead = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 16.0f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-27,-576);
			_q.eulerAngles = new Vector3(3,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
			
		}
		else if(data.partsData.type == HeroParts.BODY)
		{
			_gpd.partsBody = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 21.8f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-181.8379f,-115.0146f,-476.4911f);
			_q.eulerAngles = new Vector3(-9.700012f,2.8f,1.5f);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 100.0f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
			//-175  -15  572
		}
		else if(data.partsData.type == HeroParts.VEHICLE)
		{
			_gpd.partsVehicle = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 25.8f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-20,-573);
			_q.eulerAngles = new Vector3(6.31f,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
		}
		
		
		sampleHero.init(_gpd,true,false);
		
		sampleHero.container.SetActive(true);
		
		if(data.partsData.type == HeroParts.WEAPON)
		{
			int len = sampleHero.smrs.Length;
			for(int i = 0; i < len; ++i)
			{
				sampleHero.smrs[i].enabled = (sampleHero.smrs[i].name == data.partsData.resource || sampleHero.smrs[i].name == (data.partsData.resource + "_arrow"));
			}
		}
		else if(data.partsData.type == HeroParts.VEHICLE)
		{
			sampleHero.pet = (Pet)GameManager.me.characterManager.getMonster(true,true,data.partsData.resource.ToUpper(),false);
			sampleHero.pet.init(sampleHero);
			sampleHero.setVisible(false,false);
			sampleHero.pet.isEnabled = true;
			sampleHero.ani.Stop();
		}
		else if(data.partsData.type == HeroParts.BODY || data.partsData.type == HeroParts.HEAD)
		{
			int len = sampleHero.smrs.Length;
			for(int i = 0; i < len; ++i)
			{
				sampleHero.smrs[i].enabled = (sampleHero.smrs[i].name.Contains("weapon") == false);
			}
		}
		
		
		_v.x = 0; _v.y =0; _v.z = 0;
		
		sampleHero.setParent( sampleContainer );
		sampleHero.cTransform.localPosition = _v;
		
		_q.eulerAngles = _v;
		sampleContainer.transform.localRotation = _q;
		
		_q.eulerAngles = _v;
		sampleHero.cTransform.localRotation = _q;
		
		sampleHero.tf.localPosition = _v;
		
		_q.eulerAngles = _v;
		sampleHero.tf.localRotation = _q;
		
		
//		if(data.partsData.type == HeroParts.WEAPON)
//		{
//			sampleHero.animation.Play("weapon");
//		}
//		else
//		{
//			sampleHero.animation.Play("idle");
//		}


		if(data.partsData.type == HeroParts.WEAPON)
		{
			sampleHero.animation.Play("weapon");

			if(data.partsData.setPreviewPosition(sampleHero) == false)
			{
				if(data.partsData.character == "LEO")
				{
					_v = sampleHero.cTransform.localPosition;
					_v.y = -13f;
					sampleHero.cTransform.localPosition = _v;
				}
				
				sampleHero.cTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
			}
		}
		else if(data.partsData.type == HeroParts.HEAD)
		{
			_v = sampleHero.cTransform.localPosition;
			_v.x = 0.0f;
			_v.y = -50f;
			_v.z = 0.0f;
			sampleHero.cTransform.localPosition = _v;
			
			sampleHero.animation.Play("idle");
			sampleHero.cTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}
		else
		{
			sampleHero.animation.Play("idle");
			sampleHero.cTransform.localScale = Vector3.one;
		}




		
		_v = sampleHero.cTransform.position;
		sampleHero.shadow.transform.position = _v;



		#if UNITY_EDITOR
		
		if(DebugManager.instance.useDebug)
		{
			UIHeroPreviewDataSetter s = sampleHero.cTransform.gameObject.GetComponent<UIHeroPreviewDataSetter>();
			if(s == null)
			{
				sampleHero.cTransform.gameObject.AddComponent<UIHeroPreviewDataSetter>();
			}
		}
		#endif


	}




	void setStatHeader(int type)
	{
		switch(type)
		{
		case 0:

#if UNITY_EDITOR
			if(DebugManager.instance.useDebug)
			{
				sb1.Append("id: "+ data.partsData.baseId + "\n");
			}
#endif
			sb1.Append("[f0b938]- " + Util.getUIText("HERO") + " -[-]\n"); sb2.Append("\n"); sb3.Append("\n"); sb4.Append("\n");
			break;
		case 1:
			sb1.Append("[f0b938]- " + Util.getUIText("UNIT_RUNE") + " -[-]\n"); sb2.Append("\n"); sb3.Append("\n"); sb4.Append("\n");
			break;
		case 2:
			sb1.Append("[f0b938]- " + Util.getUIText("SKILL_RUNE") + " -[-]\n"); sb2.Append("\n"); sb3.Append("\n"); sb4.Append("\n");
			break;
		}
	}

	bool[] _statHeader = new bool[3];

	private float _bgHeightOffset = 0;

	void setStat(bool isFirstSequenceForReinforce, GameIDData descriptionData)
	{
		sb1.Length = 0;
		sb2.Length = 0;
		sb3.Length = 0;
		sb4.Length = 0;

		_statHeader[0] = false;
		_statHeader[1] = false;
		_statHeader[2] = false;


		HeroPartsData selectItem = descriptionData.partsData;
		GameIDData comparisonItem = null;

		if(isFromInventorySlot == true 
		   && popupType != RuneInfoPopup.Type.Book 
		   && popupType != RuneInfoPopup.Type.PreviewOnly 
		   && (popupType == RuneInfoPopup.Type.Normal || ( isFirstSequenceForReinforce == false )) )
		{
			if(GameDataManager.instance.selectHeroId == descriptionData.partsData.character)
			{
				switch(descriptionData.partsData.type)
				{
				case HeroParts.HEAD:
					comparisonItem = GameDataManager.selectedPlayerData.partsHead.itemInfo;
					break;
				case HeroParts.BODY:
					comparisonItem = GameDataManager.selectedPlayerData.partsBody.itemInfo;
					break;
				case HeroParts.WEAPON:
					comparisonItem = GameDataManager.selectedPlayerData.partsWeapon.itemInfo;
					break;
				case HeroParts.VEHICLE:
					comparisonItem = GameDataManager.selectedPlayerData.partsVehicle.itemInfo;
					break;
				}
			}
			else
			{
				if(descriptionData.partsData.character != null && GameDataManager.instance.heroes.ContainsKey(  descriptionData.partsData.character ))
				{

					switch(descriptionData.partsData.type)
					{
					case HeroParts.HEAD:
						comparisonItem = GameDataManager.instance.heroes[descriptionData.partsData.character].partsHead.itemInfo;
						break;
					case HeroParts.BODY:
						comparisonItem = GameDataManager.instance.heroes[descriptionData.partsData.character].partsBody.itemInfo;
						break;
					case HeroParts.WEAPON:
						comparisonItem = GameDataManager.instance.heroes[descriptionData.partsData.character].partsWeapon.itemInfo;
						break;
					case HeroParts.VEHICLE:
						comparisonItem = GameDataManager.instance.heroes[descriptionData.partsData.character].partsVehicle.itemInfo;
						break;
					}
				}
			}
		}


		setStatLine(WSATTR.HPMAX_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_HP_MAX, Util.getUIText("MAXHP"),"{0:0}");
		setStatLine(WSATTR.SPMAX_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_SP_MAX, Util.getUIText("MAXSP"),"{0:0}");
		setStatLine(WSATTR.SP_RECOVERY_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_SP_RECOVERY, Util.getUIText("SPRECOVERY"),"{0:0}");
		setStatLine(WSATTR.MPMAX_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_MP_MAX, Util.getUIText("MAXMP"),"{0:0}");
		setStatLine(WSATTR.MP_RECOVERY_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_MP_RECOVERY, Util.getUIText("MPRECOVERY"),"{0:0.0}",false,"",10);
		setStatLine(WSATTR.ATK_PHYSIC_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_ATK_PHYSIC, Util.getUIText("ATKPHYSIC"),"{0:0}");
		setStatLine(WSATTR.ATK_MAGIC_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_ATK_MAGIC, Util.getUIText("ATKMAGIC"),"{0:0}");
		setStatLine(WSATTR.DEF_PHYSIC_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_DEF_PHYSIC, Util.getUIText("DEFPHYSIC"),"{0:0}");
		setStatLine(WSATTR.DEF_MAGIC_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_DEF_MAGIC, Util.getUIText("DEFMAGIC"),"{0:0}");
		setStatLine(WSATTR.SPEED_I,descriptionData,0,selectItem,comparisonItem,BaseHeroPartsDataInfo.INDEX_SPEED, Util.getUIText("MOVESPEED"),"{0:0}");
		/*
		if(_statHeader[0])
		{
			sb1.Append("\n");sb2.Append("\n");sb3.Append("\n");
		}
		
		setStatLine(1,selectItem,null,BaseHeroPartsDataInfo.INDEX_SUMMON_SP_PERCENT, "소환SP 감소","{0:0}",false,"%");
		setStatLine(1,selectItem,null,BaseHeroPartsDataInfo.INDEX_UNIT_HP_UP, "생명력 증가","{0:0}",false,"%");
		setStatLine(1,selectItem,null,BaseHeroPartsDataInfo.INDEX_UNIT_DEF_UP, "방어력 증가","{0:0}",false,"%");
		
		if(_statHeader[1])
		{
			sb1.Append("\n");sb2.Append("\n");sb3.Append("\n");
		}
		
		setStatLine(2,selectItem,null,BaseHeroPartsDataInfo.INDEX_SKILL_SP_DISCOUNT, "소모MP 감소","{0:0}",false,"%");
		setStatLine(2,selectItem,null,BaseHeroPartsDataInfo.INDEX_SKILL_ATK_UP, "공격력 증가","{0:0}",false,"%");
		setStatLine(2,selectItem,null,BaseHeroPartsDataInfo.INDEX_SKILL_UP, "회복량 증가","{0:0}",false,"%");
		setStatLine(2,selectItem,null,BaseHeroPartsDataInfo.INDEX_SKILL_TIME_UP, "지속시간 증가","{0:0}",false,"%");
		*/

//		if(_statHeader[2]) sb1.Append("\n");

		string txt = sb1.ToString().Trim();

		while(txt.Length > 2 && txt.EndsWith("\n"))
		{
			txt = txt.Substring(0,txt.Length-2);
		}

		lbItemDescriptionName.text = txt;
		lbItemDescriptionStat.text = sb2.ToString();

		if(_compareData != null)
		{
			lbItemDescriptionMaxStat.text = sb4.ToString();
			lbItemDescriptionStat2.text = sb3.ToString();
		}
		else
		{
			lbItemDescriptionMaxStat.text = sb3.ToString();
			lbItemDescriptionStat2.text = "";
		}

		_rareDescription = descriptionData.partsData.descriptionSuperRare;
		_legendDescription = descriptionData.partsData.descriptionLegend;

		sb1.Length = 0;
		sb2.Length = 0;
		sb3.Length = 0;
		sb4.Length = 0;

		if(string.IsNullOrEmpty(_rareDescription ) == false)
		{
			sb1.Append(_rareDescription);
		}

		if(string.IsNullOrEmpty(_legendDescription ) == false)
		{
			if(sb1.Length > 0) sb1.Append("\n\n");
			sb1.Append(_legendDescription);
		}
		  
		if(sb1.Length > 0)
		{
			goRareLegendPartsDescriptionContainer.SetActive( true ) ;
			lbSuperRareLegendDescription.text = sb1.ToString();
			sb1.Length = 0;
		}
		else
		{
			goRareLegendPartsDescriptionContainer.SetActive( false ) ;
		}


		transcendInfo.setData(descriptionData);

	}

	StringBuilder sb1 = new StringBuilder();
	StringBuilder sb2 = new StringBuilder();
	StringBuilder sb3 = new StringBuilder();
	StringBuilder sb4 = new StringBuilder();

	// cInfo : 현재 주인공이 차고 있는 템. 인벤에 있는 템을 미리보기했을때 현재 착용하고 있는 템과의 비교를 해주기 위한 데이터다.

	void setStatLine(int transcendATTR, GameIDData descriptionData, int headerIndex, HeroPartsData hpd, GameIDData cInfo, int valueTypeIndex, string name, string format, bool useRound = true, string lastWord = "", int usePointNumLimit = -1)
	{
		GameValueType.Type valueType = hpd.valueTypeDic[valueTypeIndex];
		Xfloat[] value = hpd.getArrayValueByIndex(valueTypeIndex);

		int len = value.Length;
		if(len > 0 && value[0] > 0)
		{
			if(_statHeader[headerIndex] == false)
			{
				_statHeader[headerIndex] = true;
//				setStatHeader(headerIndex);
			}

			// 스탯 이름.
			sb1.Append("[fcd4a3]");
			sb1.Append(name);
			sb1.Append("[-]\n");

			// 현재 장비 스탯 정보.
			float defaultValue = GameValueType.getPartsApplyValue(value,valueType,descriptionData.reinforceLevel);

			if(descriptionData.totalPLevel > 0)
			{
				defaultValue = descriptionData.getTranscendValueByATTR(defaultValue, transcendATTR);
			}

			if(useRound) defaultValue = Mathf.CeilToInt(defaultValue - 0.4f);
			if(usePointNumLimit > -1 && defaultValue >= usePointNumLimit) format = "{0:0}";

			sb2.Append("[ffffff]");
			sb2.Append(string.Format(format,defaultValue));
			sb2.Append(lastWord+"[-]");

			// 강화 했을때 수치 변화를 알려주기 위한 녀석이다.
			if(_compareData != null)
			{
				Xfloat[] rValue = _compareData.partsData.getArrayValueByIndex(valueTypeIndex);
				float rDefaultValue = GameValueType.getPartsApplyValue(rValue,valueType,_compareData.reinforceLevel);

				if(_compareData.totalPLevel > 0)
				{
					rDefaultValue = _compareData.getTranscendValueByATTR(rDefaultValue, transcendATTR);
				}

				if(useRound) rDefaultValue = Mathf.CeilToInt(rDefaultValue - 0.4f);
				rDefaultValue = defaultValue - rDefaultValue;
				
				if(rDefaultValue > 0)
				{
					sb2.Append("[ffe400] (↑");
					sb2.Append(string.Format(format,rDefaultValue));
					sb2.Append(lastWord + ")");
				}
				else if(rDefaultValue < 0)
				{
					sb2.Append("[be1010] (↓");
					sb2.Append(string.Format(format,rDefaultValue));
					sb2.Append(lastWord + ")");
				}
			}

			// 비교할 스탯 정보.
			if(cInfo != null)
			{
				Xfloat[] cValue = cInfo.partsData.getArrayValueByIndex(valueTypeIndex);
				float cDefaultValue = GameValueType.getPartsApplyValue(cValue,valueType,cInfo.reinforceLevel);

				if(cInfo.totalPLevel > 0)
				{
					cDefaultValue = cInfo.getTranscendValueByATTR(cDefaultValue, transcendATTR);
				}

				if(useRound) cDefaultValue = Mathf.CeilToInt(cDefaultValue - 0.4f);
				cDefaultValue = defaultValue - cDefaultValue;

				if(cDefaultValue > 0)
				{
					sb3.Append("[74f344]+");
					sb3.Append(string.Format(format,cDefaultValue));
					sb3.Append(lastWord);
				}
				else if(cDefaultValue < 0)
				{
					sb3.Append("[f1290f]");
					sb3.Append(string.Format(format,cDefaultValue));
					sb3.Append(lastWord);
				}
			}

			sb2.Append("\n");
			sb3.Append("\n");
			sb4.Append("\n");

			if(name.Contains("\n"))
			{
				sb2.Append("\n");
				sb3.Append("\n");
				sb4.Append("\n");
			}

		}
	}




	public override void hide(bool isInit = false)
	{
		spSkipModeBackground.gameObject.SetActive(false);

		gameObject.SetActive(false);

		if(isInit == false)
		{
			if(popupType == RuneInfoPopup.Type.Reinforce || popupType == RuneInfoPopup.Type.Compose || popupType == RuneInfoPopup.Type.Evolve || popupType == RuneInfoPopup.Type.Transcend)
			{
				RuneStudioMain.instance.endProcess();
			}
		}

		movablePanel.parent = mainPanel.transform;
		
		RuneStudioMain.instance.cam512.gameObject.SetActive(false);

		if(sampleHero != null)
		{
			GameManager.me.characterManager.cleanMonster(sampleHero);
			sampleHero = null;
		}

		_compareData = null;

		--GameManager.me.uiManager.popupReforege.step;
	}

	void onPressInteractive(GameObject go, bool isPress)
	{
		if(sampleHero == null) return;
		
		if(isPress)
		{
			characterRotate.state = UICharacterRotate.STATE_PRESS;
		}
		else
		{
			characterRotate.state = UICharacterRotate.STATE_NORMAL;
		}
	}


	public UIPopupCardDetailReinforcePanel reinforceInfoBar;
	public UIButton btnReinfoce;
	public UIButton btnReforge;
	
	
	void onClickReinforce(GameObject go)
	{
		if(btnClose.gameObject.activeSelf == false) return;

		if(data.reinforceLevel >= GameIDData.MAX_LEVEL)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REINFORCE_NOMORE"));
			return;
		}

		hide ();
		GameManager.me.uiManager.uiMenu.uiHero.startReinforceMode( GameIDData.getClone( data )  , isFromInventorySlot);
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



	public UISprite spComposeNotice;
	public UIButton btnCompose, btnEvolution;


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
		
		
		GameManager.me.uiManager.popupEvolution.show(Rune.Type.Equipment, data.serverId, (isFromInventorySlot)?WSDefine.NO:WSDefine.YES);
		
	}




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
		else if(data.reinforceLevel < GameIDData.MAX_LEVEL)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("COMPOSE_LEVELLIMIT"));
			return;
		}
		else
		{
			int composeSourceNumber = 0;
			
			for(int i = 0; i < GameDataManager.instance.partsInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.partsInventoryList[i].rare == data.rare &&
				   GameDataManager.instance.partsInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					++composeSourceNumber;
					if(composeSourceNumber > 2) break;
				}
			}
			
			if(isFromInventorySlot) --composeSourceNumber;
			
			if(composeSourceNumber <= 0)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NO_COMPOSE_EQUIP",RareType.CHARACTER[data.rare]));
				return;
			}
		}


		hide ();

		GameManager.me.uiManager.uiMenu.uiHero.startComposeMode(GameIDData.getClone(  data ), isFromInventorySlot);

	}	



	void visibleComposeNoticeMark()
	{
		bool visible = true;

		if(data.rare >= RareType.S)
		{
			visible = false;
		}
		else if(data.reinforceLevel < GameIDData.MAX_LEVEL)
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
			
			for(int i = 0; i < GameDataManager.instance.partsInventoryList.Count; ++i)
			{
				if(GameDataManager.instance.partsInventoryList[i].rare == data.rare &&
				   GameDataManager.instance.partsInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					++composeSourceNumber;
					if(composeSourceNumber > 2) break;
				}
			}
			
			if(isFromInventorySlot) --composeSourceNumber;
			
			if(composeSourceNumber <= 0)
			{
				visible = false;
			}
		}



		spComposeNotice.enabled = visible;
	}
}



public class RuneInfoPopup
{
	public enum Type
	{
		Normal, Reinforce, Compose, Evolve,
		
		PreviewOnly, Book, Make,

		Transcend
	}
}



