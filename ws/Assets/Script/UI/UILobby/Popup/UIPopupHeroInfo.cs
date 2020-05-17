using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class UIPopupHeroInfo : UIPopupBase 
{
	public UISprite spMain, spSub;

	public UIHeroInventorySlot[] partsSlots = new UIHeroInventorySlot[4];
	public UISummonInvenSlot[] summonSlots = new UISummonInvenSlot[5];
	public UISkillInvenSlot[] skillSlots = new UISkillInvenSlot[3];

	public Transform characterContainer;

	public UICharacterRotate characterRotate;
	
	public UIButton[] btnShowCharacterInfo;
	public UIButton btnInteractive;
	public UIButton btnLeo, btnKiley, btnChloe, btnLuke, btnUse, btnUseSub, btnSelectCancel;
	public GameObject goLeoOn, goKileyOn, goChloeOn;

	public UILabel lbSelectHero, lbHeroDetailStat, lbHeroExpOnly, lbInstalledRune;
	public UILabel lbInfoName, lbInfoConetnt;

	public UISprite spBtnUse, spBtnSubUse, spCurrentCharacterIllust;

	public GameObject panelInfo2;

	public UISprite spChloeLock, spKileyLock;

	public UISprite spBtnMainBg, spBtnSubBg, spBtnCancelBg;

	public GameObject goAct3Lock, goAct5Lock;


	protected override void awakeInit ()
	{
		UIEventListener.Get(btnLeo.gameObject).onClick = onClickLeo;
		UIEventListener.Get(btnKiley.gameObject).onClick = onClickKiley;
		UIEventListener.Get(btnChloe.gameObject).onClick = onClickChloe;

		UIEventListener.Get(btnShowCharacterInfo[0].gameObject).onClick = onClickShowCharacterInfo;
		UIEventListener.Get(btnShowCharacterInfo[1].gameObject).onClick = onClickShowCharacterInfo;

		UIEventListener.Get(btnInteractive.gameObject).onPress = onPressInteractive;
		UIEventListener.Get(btnInteractive.gameObject).onClick = onClickHero;

		UIEventListener.Get(btnUse.gameObject).onClick = onClickUseHero;
		UIEventListener.Get(btnUseSub.gameObject).onClick = onClickUseHeroAsSub;

		UIEventListener.Get(btnSelectCancel.gameObject).onClick = onClickCancelSub;

		UIButtonOffset tempBo = btnUse.GetComponent<UIButtonOffset>();
		if(tempBo != null) tempBo.enabled = false;
		tempBo = btnUseSub.GetComponent<UIButtonOffset>();
		if(tempBo != null) tempBo.enabled = false;

	}


	protected override void initUI ()
	{
		lbSelectHero.text = Util.getUIText("STR_HERO_SELECT");
		lbHeroDetailStat.text = Util.getUIText("STR_HERO_DETAIL_STAT");
		lbHeroExpOnly.text = Util.getUIText("STR_EXP_HERO_ONLY");
		lbInstalledRune.text = Util.getUIText("STR_INSTALLED_RUNE");
	}



	public override void hide (bool isInit)
	{
		base.hide (isInit);

		if(isInit == false && hero != null)
		{
			GameManager.me.characterManager.cleanMonster(hero);
		}
	}


	public void refreshTab()
	{
		spChloeLock.enabled = (!GameDataManager.instance.hasCharacter(Character.CHLOE));
		spKileyLock.enabled = (!GameDataManager.instance.hasCharacter(Character.KILEY));

		btnChloe.isEnabled = (GameDataManager.instance.maxAct >= 5 );

		btnKiley.isEnabled = (GameDataManager.instance.maxAct >= 3 );

		switch(nowHero)
		{
		case Character.LEO:
			
			btnLeo.gameObject.SetActive(false);
			goLeoOn.gameObject.SetActive(true);
			
			btnKiley.gameObject.SetActive(true);
			goKileyOn.gameObject.SetActive(false);
			
			btnChloe.gameObject.SetActive(true);
			goChloeOn.gameObject.SetActive(false);
			
			_v = btnKiley.transform.localPosition;
			_v.x = 425;
			btnKiley.transform.localPosition = _v;
			
			spCurrentCharacterIllust.spriteName = "img_preview_cha_01";
			
			break;
		case Character.KILEY:
			
			btnLeo.gameObject.SetActive(true);
			goLeoOn.gameObject.SetActive(false);
			
			btnKiley.gameObject.SetActive(false);
			goKileyOn.gameObject.SetActive(true);		
			
			btnChloe.gameObject.SetActive(true);
			goChloeOn.gameObject.SetActive(false);
			
			spCurrentCharacterIllust.spriteName = "img_preview_cha_02";
			
			break;
		case Character.CHLOE:
			
			btnLeo.gameObject.SetActive(true);
			goLeoOn.gameObject.SetActive(false);
			
			btnKiley.gameObject.SetActive(true);
			goKileyOn.gameObject.SetActive(false);		
			
			_v = btnKiley.transform.localPosition;
			_v.x = 354;
			btnKiley.transform.localPosition = _v;
			
			
			btnChloe.gameObject.SetActive(false);
			goChloeOn.gameObject.SetActive(true);		
			
			spCurrentCharacterIllust.spriteName = "img_preview_cha_03";
			
			break;
		}


		_v = spMain.transform.localPosition;

		switch(GameDataManager.instance.selectHeroId)
		{
		case Character.LEO:

			if(goLeoOn.gameObject.activeSelf)
			{
				_v.x = goLeoOn.transform.localPosition.x - 62;
			}
			else
			{
				_v.x = btnLeo.transform.localPosition.x - 59;
			}

			break;
		case Character.KILEY:

			if(goKileyOn.gameObject.activeSelf)
			{
				_v.x = goKileyOn.transform.localPosition.x - 62;
			}
			else
			{
				_v.x = btnKiley.transform.localPosition.x - 59;
			}

			break;
		case Character.CHLOE:

			if(goChloeOn.gameObject.activeSelf)
			{
				_v.x = goChloeOn.transform.localPosition.x - 62;
			}
			else
			{
				_v.x = btnChloe.transform.localPosition.x - 59;
			}

			break;
		}

		spMain.transform.localPosition = _v;


		if(GameDataManager.instance.selectSubHeroId == null)
		{
			spSub.enabled = false;
		}
		else
		{
			switch(GameDataManager.instance.selectSubHeroId)
			{
			case Character.LEO:
				
				if(goLeoOn.gameObject.activeSelf)
				{
					_v.x = goLeoOn.transform.localPosition.x - 62;
				}
				else
				{
					_v.x = btnLeo.transform.localPosition.x - 59;
				}
				
				break;
			case Character.KILEY:
				
				if(goKileyOn.gameObject.activeSelf)
				{
					_v.x = goKileyOn.transform.localPosition.x - 62;
				}
				else
				{
					_v.x = btnKiley.transform.localPosition.x - 59;
				}
				
				break;
			case Character.CHLOE:
				
				if(goChloeOn.gameObject.activeSelf)
				{
					_v.x = goChloeOn.transform.localPosition.x - 62;
				}
				else
				{
					_v.x = btnChloe.transform.localPosition.x - 59;
				}
				
				break;
			}

			spSub.enabled = true;
			spSub.transform.localPosition = _v;
		}


	}

	void onClickUseHero(GameObject go)
	{
		SoundData.play("uihe_changehero"); 

		if(GameDataManager.instance.selectHeroId == nowHero) return;

		if(GameDataManager.instance.selectSubHeroId != null)
		{
			switch(nowHero)
			{
			case Character.LEO:

				if(GameDataManager.instance.serverHeroData.ContainsKey(Character.CHLOE))
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.CHLOE);
				}
				else if(GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY))
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.KILEY);
				}
				else
				{
					EpiServer.instance.sendChangeHero(nowHero, null);
				}

				break;
			case Character.KILEY:

				if(GameDataManager.instance.serverHeroData.ContainsKey(Character.CHLOE))
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.CHLOE);
				}
				else
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.LEO);
				}

				break;
			case Character.CHLOE:

				if(GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY))
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.KILEY);
				}
				else
				{
					EpiServer.instance.sendChangeHero(nowHero, Character.LEO);
				}

				break;
			}
		}
		else
		{
			EpiServer.instance.sendChangeHero(nowHero, GameDataManager.instance.selectHeroId);
		}
	}



	void onClickUseHeroAsSub(GameObject go)
	{
		SoundData.play("uihe_changehero"); 
		
		if(GameDataManager.instance.selectSubHeroId == nowHero) return;

		if(GameDataManager.instance.serverHeroData.Count == 1)
		{
			return;
		}

		EpiServer.instance.sendChangeHero(GameDataManager.instance.selectHeroId, nowHero);
	}



	public void onClickCancelSub(GameObject go)
	{
		SoundData.play("uihe_changehero"); 
		
		if(GameDataManager.instance.serverHeroData.Count == 1)
		{
			return;
		}
		
		EpiServer.instance.sendChangeHero(GameDataManager.instance.selectHeroId, string.Empty);
	}




	public void onCompleteChangeHero()
	{
		refreshUseButton();
		refreshTab();

		UIMenu m = GameManager.me.uiManager.uiMenu;

		if(m.currentPanel == UIMenu.HERO)
		{
			m.uiHero.tabPlayer.setSelect();
		}
		else if(m.currentPanel == UIMenu.SUMMON)
		{
			m.uiSummon.tabPlayer.setSelect();
		}
		else if(m.currentPanel == UIMenu.SKILL)
		{
			m.uiSkill.tabPlayer.setSelect();
		}
	}

	void onClickLeo(GameObject go) 
	{  
		setCharacter(Character.LEO);
		//EpiServer.instance.sendChangeHero(Character.LEO, true) ; 
	}

	void onClickKiley(GameObject go)
	{ 
		
		if(GameDataManager.instance.heroes.ContainsKey(Character.KILEY))
		{
			SoundData.play("uihe_changehero"); 
			setCharacter(Character.KILEY);
			//EpiServer.instance.sendChangeHero(Character.KILEY, true) ; 
		}
		else
		{
			
			UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUY_HERO", Util.getUIText("KILEY")), onBuyKiley, null, GameDataManager.instance.heroPrices[Character.KILEY].ToString());
		}
	}


	void onClickChloe(GameObject go) 
	{ 
		
		if(GameDataManager.instance.heroes.ContainsKey(Character.CHLOE))
		{
			SoundData.play("uihe_changehero"); 
			setCharacter(Character.CHLOE);
			//EpiServer.instance.sendChangeHero(Character.CHLOE, true) ; 
		}
		else
		{
			UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUY_HERO", Util.getUIText("CHLOE")), onBuyChloe, null, GameDataManager.instance.heroPrices[Character.CHLOE].ToString());
		}
	}














	public void init(string startCharacter)
	{
		show ();

		goAct3Lock.gameObject.SetActive( GameDataManager.instance.maxAct < 3);
		goAct5Lock.gameObject.SetActive( GameDataManager.instance.maxAct < 5);


		characterRotate.state = UICharacterRotate.STATE_NORMAL;
		
		nowHero = "";

		setCharacter(startCharacter);

		setCharacterInfoVisible(false);

		if(string.IsNullOrEmpty(nowHero) || (nowHero == Character.KILEY && GameDataManager.instance.maxAct < 3) || (nowHero == Character.CHLOE && GameDataManager.instance.maxAct < 5))
		{
			Debug.LogError("HAS ERRORRRRRRRRRRR~~~~~~");

			//onSelectLeo(null);
		}
	}


	private bool _statPanelVisible = false;
	public void setCharacterInfoVisible(bool isShow, bool tween = false)
	{
		_statPanelVisible = isShow;
		if(isShow)
		{
			if(tween) TweenPosition.Begin(panelInfo2,0.2f,new Vector3(-463.0f,-74.0f,-568f)).method = UITweener.Method.EaseIn;
			else panelInfo2.transform.localPosition = new Vector3(-463.0f,-74.0f,-568f);			
		}
		else
		{
			if(tween) TweenPosition.Begin(panelInfo2,0.2f,new Vector3(-1008,-74,-568f)).method = UITweener.Method.EaseIn;
			else panelInfo2.transform.localPosition = new Vector3(-1008,-74,-568f);			
		}
	}


	public UIScrollView partsContentView;
	void onClickShowCharacterInfo(GameObject go)
	{
		partsContentView.ResetPosition();
		if(_statPanelVisible)
		{
			setCharacterInfoVisible(false, true);
		}
		else
		{
			setCharacterInfoVisible(true, true);
		}
	}



	private void setCharacterData()
	{
		
		sb1.Length = 0;
		sb2.Length = 0;
		
		if(hero == null) return;
		
		hero.changeGamePlayerData(GameDataManager.instance.heroes[nowHero]);
		
		sb1.Append("[f0b938]- "+Util.getUIText("HERO")+" -[-]\n[e0c3a1]");
		sb2.Append("\n");
		
		setStatLine(hero.maxHp,"MAXHP","{0:0}");
		setStatLine(hero.maxSp,"MAXSP","{0:0}");
		setStatLine(hero.stat.spRecovery,"SPRECOVERY","{0:0.0}");
		setStatLine(hero.maxMp,"MAXMP","{0:0}");
		setStatLine(hero.stat.mpRecovery,"MPRECOVERY","{0:0.0}");
		
		if(hero.stat.atkPhysic > hero.stat.atkMagic) setStatLine(hero.stat.atkPhysic,"ATKPHYSIC","{0:0.0}");
		else setStatLine(hero.stat.atkMagic,"ATKMAGIC","{0:0.0}");
		
		setStatLine(hero.stat.defPhysic,"DEFPHYSIC","{0:0.0}");
		setStatLine(hero.stat.defMagic,"DEFMAGIC","{0:0.0}");
		setStatLine(hero.stat.speed,"MOVESPEED","{0:0}");
		
		sb1.Append("[-]");
		sb1.Append("[f0b938]- " + Util.getUIText("UNIT_RUNE") + " -[-]\n[e0c3a1]");
		sb2.Append("\n");
		
		
		setStatLine(hero.stat.summonSpPercent*100,"SUMMON_SP_PERCENT","{0:0}","%\n");
		setStatLine(hero.stat.unitHpUp*100,"UNIT_HP_UP","{0:0}","%\n");
		setStatLine(hero.stat.unitDefUp*100,"UNIT_DEF_UP","{0:0}","%\n");
		
		sb1.Append("[-]");
		sb1.Append("[f0b938]- "+Util.getUIText("SKILL_RUNE")+" -[-]\n[e0c3a1]");
		sb2.Append("\n");
		
		setStatLine(hero.stat.skillMpDiscount*100,"SKILL_MP_DISCOUNT","{0:0}","%\n");
		setStatLine(hero.stat.skillAtkUp*100,"SKILL_ATK_UP","{0:0}","%\n");
		setStatLine(hero.stat.skillUp*100,"SKILL_UP","{0:0}","%\n");
		setStatLine(hero.stat.skillTimeUp*100,"SKILL_TIME_UP","{0:0}","%\n");
		
		sb1.Append("[-]");
		
		lbInfoName.text = sb1.ToString();
		lbInfoConetnt.text = sb2.ToString();
	}
	
	StringBuilder sb1 = new StringBuilder();
	StringBuilder sb2 = new StringBuilder();
	void setStatLine(float value, string nameId, string format, string lastWord = "\n")
	{
		sb1.Append(Util.getUIText(nameId));
		sb1.Append("\n");
		sb2.Append(string.Format(format,value));
		sb2.Append(lastWord);
	}


	void onClickHero(GameObject go)
	{
		if(hero != null)
		{
			hero.ani.Play(Monster.SHOOT);
			hero.pet.ani.Play(Monster.SHOOT);
			
			hero.ani.CrossFadeQueued(Monster.NORMAL);
			hero.pet.ani.CrossFadeQueued(Monster.NORMAL);
			
			SoundData.playPlayerAttackSound(GameManager.info.soundData[hero.resourceId], hero.playerData.characterId, 100);
		}
	}
	
	
	void onPressInteractive(GameObject go, bool isPress)
	{
		
		if(hero == null) return;
		
		if(isPress)
		{
			characterRotate.state = UICharacterRotate.STATE_PRESS;
		}
		else
		{
			characterRotate.state = UICharacterRotate.STATE_NORMAL;
		}
	}


	Player hero = null;
	
	private void refreshHeroModel()
	{
		
		if(hero != null)
		{
			GameManager.me.characterManager.cleanMonster(hero);
		}
		
		hero = (Player)GameManager.me.characterManager.getMonster(true,true,nowHero,false);
		
		hero.init(GameDataManager.instance.heroes[nowHero],true,false);
		hero.pet = (Pet)GameManager.me.characterManager.getMonster(false,false,GameDataManager.instance.heroes[nowHero].partsVehicle.parts.resource.ToUpper(),false);
		hero.pet.init(hero);
		hero.setParent(characterContainer);
		hero.cTransform.localPosition = new Vector3(0,0,0);	
		
		setCharacterData();
	}
	
	
	public static string _nowHero = null;
	public static string nowHero
	{
		get
		{
			#if UNITY_EDITOR
			if(DebugManager.instance.useDebug)
			{
				return DebugManager.instance.defaultHero;
			}
			#endif
			
			if(_nowHero == null) return Character.LEO;
			return _nowHero;
		}
		set
		{
			
			_nowHero = value;
		}
	}


	void setCharacter(string heroId)
	{
		if(nowHero != heroId)
		{
			nowHero = heroId;

			refreshTab();
			refreshUseButton();
			refreshSlots();
			refreshHeroModel();
		}
	}


	private void refreshUseButton()
	{
		// 히어로를 레오 1종만 보유한 경우. 
		if(GameDataManager.instance.serverHeroData.Count == 1 || (GameDataManager.instance.selectSubHeroId == null && nowHero == GameDataManager.instance.selectHeroId))
		{
			btnUse.isEnabled = false;
			spBtnUse.enabled = true;
			spBtnMainBg.spriteName = "ibtn_cha_select_main_on";

			_v.x = 278; _v.y = -410; _v.z = 0;
			btnUse.transform.localPosition = _v;

			btnUseSub.gameObject.SetActive(false);
			btnSelectCancel.gameObject.SetActive(false);
		}
		else
		{
			btnUseSub.gameObject.SetActive(true);
			btnSelectCancel.gameObject.SetActive(false);

			// 서브 히어로 탭일때 선택 취소 버튼 노출.
			if(GameDataManager.instance.selectSubHeroId != null && nowHero == GameDataManager.instance.selectSubHeroId)
			{
				btnSelectCancel.gameObject.SetActive(true);

				btnUse.isEnabled = false;
				btnUseSub.isEnabled = false;

				spBtnUse.enabled = false;
				spBtnSubUse.enabled = true;
				
				spBtnMainBg.spriteName = "ibtn_cha_select_main_off";
				spBtnSubBg.spriteName = "ibtn_cha_select_sub_on";

			}
			if(GameDataManager.instance.selectHeroId != null && nowHero == GameDataManager.instance.selectHeroId)
			{
				btnSelectCancel.gameObject.SetActive(false);
				
				btnUse.isEnabled = false;
				btnUseSub.isEnabled = false;
				
				spBtnUse.enabled = true;
				spBtnSubUse.enabled = false;
				
				spBtnMainBg.spriteName = "ibtn_cha_select_main_on";
				spBtnSubBg.spriteName = "ibtn_cha_select_sub_off";
			}
			// 메인 /서브 히어로로 선택되지 않은 히어로 탭일때 양버튼 모두 활성.
			else if(nowHero != GameDataManager.instance.selectHeroId && nowHero != GameDataManager.instance.selectSubHeroId)
			{
				btnUse.isEnabled = true;
				btnUseSub.isEnabled = true;
				
				spBtnUse.enabled = false;
				spBtnSubUse.enabled = false;
				
				spBtnMainBg.spriteName = "ibtn_cha_select_main_on";
				spBtnSubBg.spriteName = "ibtn_cha_select_sub_on";
			}


			if(btnSelectCancel.gameObject.activeSelf)
			{
				_v.x = 125; _v.y = -410; _v.z = 0;
				btnUse.transform.localPosition = _v;
				
				_v.x = 325; _v.y = -410; _v.z = 0;
				btnUseSub.transform.localPosition = _v;
			}
			else
			{
				_v.x = 170; _v.y = -410; _v.z = 0;
				btnUse.transform.localPosition = _v;
				
				_v.x = 386; _v.y = -410; _v.z = 0;
				btnUseSub.transform.localPosition = _v;
			}

		}
	}


	private void refreshSlots()
	{
		if(string.IsNullOrEmpty(nowHero) || GameDataManager.instance.serverHeroData.ContainsKey(nowHero) == false)
		{
			Debug.LogError("Error!");
			return;
		}

		P_Hero heroData = GameDataManager.instance.serverHeroData[nowHero];

		partsSlots[0].setStringData(heroData.selEqts[HeroParts.HEAD], false);
		partsSlots[1].setStringData(heroData.selEqts[HeroParts.BODY], false);
		partsSlots[2].setStringData(heroData.selEqts[HeroParts.WEAPON], false);
		partsSlots[3].setStringData(heroData.selEqts[HeroParts.VEHICLE], false);

		foreach(UIHeroInventorySlot ps in partsSlots)
		{
			ps.slotType = BaseSlot.InventorySlotType.HeroInfoSlot;
		}

		int i = 0;

		foreach(string u in GameDataManager.instance.getSelectUnitRunes(nowHero))
		{
			if(string.IsNullOrEmpty(u))
			{
				summonSlots[i].setData( null );
			}
			else
			{
				GameIDData gd = new GameIDData();
				gd.parse(u, GameIDData.Type.Unit);
				summonSlots[i].setData( gd );
				summonSlots[i].slotType = BaseSlot.InventorySlotType.HeroInfoSlot;
			}
			
			++i;
		}
		
		i = 0;
		foreach(string s in GameDataManager.instance.getSelectSkillRunes(nowHero))
		{
			if(string.IsNullOrEmpty(s))
			{
				skillSlots[i].setData( null );
			}
			else
			{
				GameIDData gd = new GameIDData();
				gd.parse(s, GameIDData.Type.Skill);
				skillSlots[i].setData( gd );
				skillSlots[i].slotType = BaseSlot.InventorySlotType.HeroInfoSlot;
			}
			++i;
		}
	}



	
	
	void onBuyKiley()
	{
		if(GameDataManager.instance.ruby < GameDataManager.instance.heroPrices[Character.KILEY])
		{
			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
		}
		else
		{
			EpiServer.instance.sendBuyHero(Character.KILEY, GameIDData.Type.None);
		}
	}
	
	
	void onBuyChloe()
	{
		if(GameDataManager.instance.ruby < GameDataManager.instance.heroPrices[Character.CHLOE])
		{
			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
		}
		else
		{
			EpiServer.instance.sendBuyHero(Character.CHLOE, GameIDData.Type.None);
		}
	}
	
	
	
	void onSelectLuke(GameObject go) 
	{  
		SoundData.play("uihe_changehero"); 
		//setCharacter(LUKE); 
	}
}



