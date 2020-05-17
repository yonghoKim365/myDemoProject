using UnityEngine;
using System.Collections;

public class UIPopupFriendInviteReward : MonoBehaviour {
	
	public UISprite[] spInviteClear;
	public UILabel[] lbInviteReward;
	public UISprite[] spInviteDouble;
	public UILabel lbInviteNum;

	bool _checkRewardDouble = false;
	bool _showRewardDouble = true;

	public void refreshInviteNum()
	{
		lbInviteNum.text = GameDataManager.instance.inviteCount.ToString();
		
		spInviteClear[0].enabled = GameDataManager.instance.inviteCount >= 10;
		spInviteClear[1].enabled = GameDataManager.instance.inviteCount >= 20;
		spInviteClear[2].enabled = GameDataManager.instance.inviteCount >= 30;
		spInviteClear[3].enabled = GameDataManager.instance.inviteCount >= 40;

		if (spInviteClear.Length > 4){
			spInviteClear[4].enabled = GameDataManager.instance.inviteCount >= 50;
			spInviteClear[5].enabled = GameDataManager.instance.inviteCount >= 60;
			spInviteClear[6].enabled = GameDataManager.instance.inviteCount >= 70;
		}
		
		string inviteColor = "bf9086";
		string defaultColor = "854c1b";
		
		lbInviteReward[0].text = "["+((GameDataManager.instance.inviteCount >= 10)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD10")+"[-]";
		lbInviteReward[1].text = "["+((GameDataManager.instance.inviteCount >= 20)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD20")+"[-]";
		lbInviteReward[2].text = "["+((GameDataManager.instance.inviteCount >= 30)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD30")+"[-]";
		lbInviteReward[3].text = "["+((GameDataManager.instance.inviteCount >= 40)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD40")+"[-]";
		if (lbInviteReward.Length > 4){
			lbInviteReward[4].text = "["+((GameDataManager.instance.inviteCount >= 50)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD50")+"[-]";
			lbInviteReward[5].text = "["+((GameDataManager.instance.inviteCount >= 60)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD60")+"[-]";
			lbInviteReward[6].text = "["+((GameDataManager.instance.inviteCount >= 70)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD70")+"[-]";
		}
		
		if(_checkRewardDouble == false)
		{
			_checkRewardDouble = true;
			_showRewardDouble = Util.getUIText("INVITE_DOUBLE").Equals("Y");
			
			spInviteDouble[0].enabled = _showRewardDouble;
			spInviteDouble[1].enabled = _showRewardDouble;
			spInviteDouble[2].enabled = _showRewardDouble;
			spInviteDouble[3].enabled = _showRewardDouble;
			if (spInviteDouble.Length > 4){
				spInviteDouble[4].enabled = _showRewardDouble;
				spInviteDouble[5].enabled = _showRewardDouble;
				spInviteDouble[6].enabled = _showRewardDouble;
			}
		}
		
		if(_showRewardDouble)
		{
			spInviteDouble[0].alpha = ((GameDataManager.instance.inviteCount >= 10)?0.65f:1.0f);
			spInviteDouble[1].alpha = ((GameDataManager.instance.inviteCount >= 20)?0.65f:1.0f);
			spInviteDouble[2].alpha = ((GameDataManager.instance.inviteCount >= 30)?0.65f:1.0f);
			spInviteDouble[3].alpha = ((GameDataManager.instance.inviteCount >= 40)?0.65f:1.0f);
			if (spInviteDouble.Length > 4){
				spInviteDouble[4].alpha = ((GameDataManager.instance.inviteCount >= 50)?0.65f:1.0f);
				spInviteDouble[5].alpha = ((GameDataManager.instance.inviteCount >= 60)?0.65f:1.0f);
				spInviteDouble[6].alpha = ((GameDataManager.instance.inviteCount >= 70)?0.65f:1.0f);
			}
		}
	}
}
