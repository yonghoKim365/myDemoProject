using UnityEngine;
using System.Collections;

public class UIChampionshipLastWeekResultListSlotPanel : UIListGridItemPanelBase {

	public UIButton btn, btnInfo;

	public PhotoDownLoader face;

	public UILabel lbName, lbWinPoint, lbRanking;

	public UISprite spIcon, spHighRankerIcon, spHighRankBg, spDefaultBg, spEmptyFace;

	public UILabel lbAtkWin, lbAtkLose, lbDefWin, lbDefLose;

	// Use this for initialization
	protected override void initAwake ()
	{
		//UIEventListener.Get(btn.gameObject).onClick = onSelect;
		//UIEventListener.Get(btnInfo.gameObject).onClick = onClickInfo;
	}



	void onClickInfo(GameObject go)
	{
		if(PandoraManager.instance.localUser.userID != data.userId)
		{
			EpiServer.instance.sendEnemyData(data);
		}
		else
		{
			GameManager.me.uiManager.popupFriendDetail.myData();
		}
	}


	public override void setPhotoLoad()
	{
		if(data == null) return;

		if(data.showPhoto == WSDefine.TRUE)
		{
			face.down(data.imageUrl);
		}
	}	

	P_Champion data;


	public const string ICON_DOWN = "icn_mark_skull";
	public const string ICON_BRONZ = "icn_levelmedal_bronz";
	public const string ICON_GOLD = "icn_levelmedal_gold";
	public const string ICON_LEGEND = "icn_levelmedal_legend";
	public const string ICON_MASTER = "icn_levelmedal_master";
	public const string ICON_PLATINUM = "icn_levelmedal_platinum";
	public const string ICON_SILVER = "icn_levelmedal_silver";

	static Vector3 _downgradeIconPosition = new Vector3(-352,224.74f,220);
	static Vector3 _medalIconPosition = new Vector3(-341,226,220);

	static Color _myColor = new Color(254f/255f,240f/255f,193f/255f);
	static Color _enemyColor = new Color(157.0f/255.0f, 216f/255f, 36f/255f);

	public override void setData(object obj)
	{
		data = (P_Champion)obj;

		UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, face);

		lbName.text = Util.GetShortID(data.nickname,10);

		lbWinPoint.text = data.score + "";

		lbRanking.text = data.rank.ToString();

		Vector3 _v;

		if(PandoraManager.instance.localUser.userID != data.userId)
		{
			lbName.color = _enemyColor;
		}
		else
		{
			lbName.color = _myColor;;
		}


		switch(data.rank)
		{
		case 1:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_gold";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			spDefaultBg.enabled = false;
			break;
		case 2:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_silver";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			spDefaultBg.enabled = false;
			break;
		case 3:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_bronz";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			spDefaultBg.enabled = false;
			break;
		default:
			spHighRankerIcon.enabled = false;
			spHighRankBg.enabled = false;
			lbRanking.text = Util.getUIText("WORD_RANK", data.rank.ToString());
			spDefaultBg.enabled = true;
			break;
		}



		switch(GameDataManager.instance.lastChampionshipData.league)
		{
		case WSDefine.LEAGUE_LEGEND:
			if(data.rank >= 8)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_PLATINUM:
			
			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_LEGEND; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_MASTER:
			
			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_PLATINUM; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_GOLD:
			
			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_MASTER; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
				
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_SILVER:
			
			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_GOLD; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_BRONZE:
			if(data.rank <= 7 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_SILVER; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		}

		lbAtkWin.text = "" + data.attackWin;
		lbAtkLose.text = "" + data.attackLose; 
		lbDefWin.text = "" + data.defenceWin; 
		lbDefLose.text = "" + data.defenceLose; 
	}




}
