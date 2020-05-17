using UnityEngine;
using System.Collections;

public class UIPopupDefenceResultSlotPanel : UIListGridItemPanelBase {

	public UILabel lbRank, lbName, lbMatchNum, lbScore;

	public UISprite spResult;

	public UIButton btnReplay;

	public PhotoDownLoader photo;

	public UISprite spEmptyFace;


	public UISprite spDefaultBg, spHighRankerBg, spHighRankerIcon;


	protected override void initAwake ()
	{
		UIEventListener.Get(btnReplay.gameObject).onClick = onClickReplay;

	}

	private string _attackerId;
	private string _roundId;


	public override void setPhotoLoad()
	{
		if(data == null) return;
		photo.down(data.imageUrl);
	}	


	P_DefenceRecord data = null;
	public override void setData(object obj)
	{
		data = (P_DefenceRecord)obj;

		lbName.text = data.nickname;

		lbScore.text = Util.GetCommaScore(data.score);

		if(data.rank <= 3)
		{
			spHighRankerBg.enabled = true;
			spHighRankerIcon.enabled = true;
			spDefaultBg.enabled = false;

			switch(data.rank)
			{
			case 1: spHighRankerIcon.spriteName = "img_rank_gold"; break;
			case 2: spHighRankerIcon.spriteName = "img_rank_silver"; break;
			case 3: spHighRankerIcon.spriteName = "img_rank_bronz"; break;
			}

			lbRank.text = data.rank.ToString();
		}
		else
		{
			spHighRankerBg.enabled = false;
			spHighRankerIcon.enabled = false;
			spDefaultBg.enabled = true;

			lbRank.text = Util.getUIText("WORD_RANK", data.rank.ToString());
		}


		switch(data.round)
		{
		case "R0": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","1"); break;
		case "R1": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","2"); break;
		case "R2": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","3"); break;
		case "R3": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","4"); break;
		case "R4": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","5"); break;
		case "R5": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","6"); break;
		case "R6": lbMatchNum.text = Util.getUIText("MATCH_NUMBER","7"); break;
		}

		if(data.didDefend == WSDefine.YES)
		{
			spResult.spriteName = "ibtn_mark_winidle";
		}
		else
		{
			spResult.spriteName = "ibtn_mark_loseidle";
		}

		UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, photo);

	}

	void onClickReplay(GameObject go)
	{
		EpiServer.instance.sendGetReplay(false, data.attackerId, data.round);
	}
}
