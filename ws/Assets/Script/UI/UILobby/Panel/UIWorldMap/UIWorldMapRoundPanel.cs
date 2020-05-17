using UnityEngine;
using System.Collections;

public class UIWorldMapRoundPanel : MonoBehaviour 
{
	public UISprite spRoundBg, spRoundText, spArrow, spClear;

	public UIButton btnClick;

	void Awake()
	{
		UIEventListener.Get(btnClick.gameObject).onClick = onClickRound;
	}

	public enum RoundStatus
	{
		CLEAR, SELECT, SELECT2, LOCK
	}

	void onClickRound(GameObject go)
	{
		if(_status == RoundStatus.LOCK) return;

		if(_nowRound == GameManager.me.stageManager.playRound) return;

		GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.open(GameManager.me.stageManager.playAct,
		                                                            GameManager.me.stageManager.playStage,
		                                                            _nowRound, false, true);
	}

	private RoundStatus _status;
	private int _nowRound;

	public void setData(RoundStatus status, int nowRoundNum = -1, bool isClearStage = false)
	{
		_status = status;
		_nowRound = nowRoundNum;

		if(nowRoundNum == 1) spArrow.enabled = false;
		else spArrow.enabled = true;

		switch(status)
		{

		case RoundStatus.CLEAR:
			spRoundBg.spriteName = "img_epic_round_bg";
			spRoundText.enabled = true;
			spRoundText.spriteName = "img_text_round"+nowRoundNum;
			spClear.enabled = true;
//			spArrow.color = new Color(1,1,1);
			break;

		case RoundStatus.SELECT:
			spRoundBg.spriteName = "img_epic_round_select";
			spRoundText.enabled = true;
			spRoundText.spriteName = "img_text_round"+nowRoundNum;
			spClear.enabled = isClearStage;
//			spArrow.color = new Color(1,1,1);
			break;

		case RoundStatus.SELECT2:
			spRoundBg.spriteName = "img_epic_round_bg";
			spRoundText.enabled = true;
			spRoundText.spriteName = "img_text_round"+nowRoundNum;
			spClear.enabled = false;
			//			spArrow.color = new Color(1,1,1);
			break;

		case RoundStatus.LOCK:
			spRoundBg.spriteName = "img_epic_round_lock";
			spRoundText.enabled = false;
			spClear.enabled = false;
//			spArrow.color = new Color(0.7f,0.7f,0.7f);
			break;
		}
	}

}
