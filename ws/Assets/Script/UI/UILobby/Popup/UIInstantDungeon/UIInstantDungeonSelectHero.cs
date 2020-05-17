using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class UIInstantDungeonSelectHero : UIPopupBase 
{
	public UILabel lbRoundStartPrice;
	public UISprite spRoundStartPriceType;

	public UIInstantDungeonSelectHeroSlot[] slots = new UIInstantDungeonSelectHeroSlot[3];

	public UIButton btnOk;

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnOk.gameObject).onClick = onClickOk;

		slots[0].characterName = Character.LEO;
		slots[1].characterName = Character.KILEY;
		slots[2].characterName = Character.CHLOE;

		slots[0].iconName = "img_characimg4";
		slots[1].iconName = "img_characimg2";
		slots[2].iconName = "img_characimg3";

		UIEventListener.Get(slots[0].btn.gameObject).onClick = onClickLeo;
		UIEventListener.Get(slots[1].btn.gameObject).onClick = onClickKiley;
		UIEventListener.Get(slots[2].btn.gameObject).onClick = onClickChloe;
	}

	void onClickLeo(GameObject go)
	{
		_currentSelectHero = Character.LEO;
		refresh();
	}

	void onClickKiley(GameObject go)
	{
		if(GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY) == false) return;
		_currentSelectHero = Character.KILEY;
		refresh();
	}

	void onClickChloe(GameObject go)
	{
		if(GameDataManager.instance.serverHeroData.ContainsKey(Character.CHLOE) == false) return;
		_currentSelectHero = Character.CHLOE;
		refresh();
	}

	void refresh()
	{
		for(int i = 0; i < 3; ++i)
		{
			slots[i].isSelect = (slots[i].characterName == _currentSelectHero);
		}
	}


	protected override void initUI ()
	{

	}

	public override void hide (bool isInit = false)
	{
		base.hide (isInit);

		if(isInit == false)
		{

		}
	}

	private string _currentSelectHero = Character.LEO;

	public GameObject goEnterNoPriceType,goEnterPriceType;

	public override void show ()
	{
		base.show ();
		lbRoundStartPrice.text = GameManager.me.uiManager.popupInstantDungeon.lbRoundStartPrice.text;
		spRoundStartPriceType.spriteName = GameManager.me.uiManager.popupInstantDungeon.spRoundStartPriceType.spriteName;


		goEnterNoPriceType.SetActive(GameManager.me.uiManager.popupInstantDungeon.goEnterNoPriceType.activeSelf);
		goEnterPriceType.SetActive(GameManager.me.uiManager.popupInstantDungeon.goEnterPriceType.activeSelf);
		
		
		for(int i = 0; i < 3; ++i)
		{
			slots[i].isEnable = GameDataManager.instance.serverHeroData.ContainsKey(slots[i].characterName);

			if(slots[i].characterName == GameDataManager.instance.selectHeroId)
			{
				slots[i].spMainSubIcon.enabled = true;
				slots[i].spMainSubIcon.spriteName = "img_mark_cha_main";
			}
			else if(GameDataManager.instance.selectSubHeroId != null && slots[i].characterName == GameDataManager.instance.selectSubHeroId)
			{
				slots[i].spMainSubIcon.enabled = true;
				slots[i].spMainSubIcon.spriteName = "img_mark_cha_sub";
			}
			else
			{
				slots[i].spMainSubIcon.enabled = false;
			}

		}

		refresh();

	}

	void onClickOk(GameObject go)
	{
		hide (false);
		GameManager.me.uiManager.popupInstantDungeon.selectHeroData = GameDataManager.instance.heroes[_currentSelectHero];
		EpiServer.instance.sendPlaySigong(GameManager.me.uiManager.popupInstantDungeon.selectSigongData.id);
	}

}



