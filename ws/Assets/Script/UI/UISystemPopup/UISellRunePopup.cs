using UnityEngine;
using System.Collections;

public class UISellRunePopup : UISystemPopupBase {

	public UILabel lbPrice;

	public UIButton btnMulti;

	protected override void awakeInit ()
	{
		base.awakeInit ();
		UIEventListener.Get(btnMulti.gameObject).onClick = onClickMulti;
	}

	GameIDData _data = new GameIDData();

	public override void show (PopupData pd, string msg)
	{
		base.show (pd, msg);

		_data.parse( (string)pd.data[0] );

		lbPrice.text = Util.GetCommaScore( _data.getSellPrice() );
	}

	public void onClickMulti(GameObject go)
	{
		if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.HERO)
		{
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();
			hide();
			GameManager.me.uiManager.uiMenu.uiHero.startSellMode( _data );
		}
		else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SUMMON)
		{
			GameManager.me.uiManager.popupSummonDetail.hide();
			hide();
			GameManager.me.uiManager.uiMenu.uiSummon.startSellMode( _data );
		}
		else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SKILL)
		{
			GameManager.me.uiManager.popupSkillPreview.isEnabled = false;
			hide();
			GameManager.me.uiManager.uiMenu.uiSkill.startSellMode( _data );
		}
	}

}
