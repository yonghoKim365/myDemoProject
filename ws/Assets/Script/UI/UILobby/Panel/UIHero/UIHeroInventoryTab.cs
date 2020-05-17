using UnityEngine;
using System.Collections;

public class UIHeroInventoryTab : MonoBehaviour 
{

	public bool isMyInven = true;

	public UISprite spRareBg, spIcon, spRareBorder, spSelectBorder;

	public UILabel lbCount, lbTranscendLevel;

	public UIButton btn;
	
	public GameIDData data;

	public int defaultDepth = -1000;

	void Awake()
	{
		UIEventListener.Get(btn.gameObject).onClick = onSelectTab;
	}

	public void setData(GameIDData d)
	{
		isChecked = false;
		spSelectBorder.enabled = false;
		data = d;

		spRareBorder.spriteName = RareType.getRareLineSprite(d.rare);
		spRareBg.spriteName = RareType.getRareBgSprite(d.rare);

		Icon.setEquipIcon(data.partsData.getIcon(), spIcon, defaultDepth);

		lbCount.text = "l"+d.level;

		Util.setTranscendLevel(lbTranscendLevel, d.totalPLevel);

	}

	public bool select
	{
		set
		{
			spSelectBorder.enabled = value;
		}
	}

	
	bool _isChecked = false;
	
	public bool isChecked
	{
		get
		{
			return _isChecked;
		}
		set
		{
			_isChecked = value;
			btn.enabled = !value;
		}
	}

	
	public void onSelectTab(GameObject go)
	{
		if(isMyInven)
		{
			GameManager.me.uiManager.uiMenu.uiHero.onClickTabSlot(this, data);
		}
		else
		{
			GameManager.me.uiManager.uiVisitingLobby.onClickEquip(data);
		}
	}
	
	
	
}
