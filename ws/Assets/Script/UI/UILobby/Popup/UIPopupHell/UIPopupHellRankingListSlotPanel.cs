using UnityEngine;
using System.Collections;

public class UIPopupHellRankingListSlotPanel : UIListGridItemPanelBase {

	public UILabel lbName, lbScore, lbRanking;

	public UISprite spRankingIcon, spBackground, spEmptyFace;

	public PhotoDownLoader face;

	public UIButton btnInfo;

	protected override void initAwake ()
	{
		UIEventListener.Get(btnInfo.gameObject).onClick = onClickInfo;
	}


	public override void setPhotoLoad()
	{
		UIManager.setPlayerPhoto(_photoDownloadUrl, spEmptyFace, face, UIManager.PhotoDownLoadType.DownLoad);
	}	

	public P_UserRank userData;
	public P_FriendRank friendData;

	bool _isMyPanel = false;

	string _photoDownloadUrl = null;

	public override void setData(object obj)
	{
		_isMyPanel = false;

		if(GameManager.me.uiManager.popupHell.list.isFriendType == false)
		{
			userData = (P_UserRank) obj;


			lbScore.text = Util.GetCommaScore( userData.score );
			lbRanking.text = userData.rank.ToString();

			if(userData.userId == PandoraManager.instance.localUser.userID)
			{
				spBackground.spriteName = "img_border_listbox1";
				_isMyPanel = true;
				lbName.text = "[ffe090]"+Util.GetShortID( userData.nickname , 12)+"[-]";
			}
			else
			{
				spBackground.spriteName = "img_border_listbox";
				lbName.text = "[8ef63f]"+Util.GetShortID( userData.nickname , 12)+"[-]";
			}

			spRankingIcon.enabled = (userData.rank <= 3);

			UIManager.setPlayerPhoto(userData.imageUrl, spEmptyFace, face, UIManager.PhotoDownLoadType.Init);
			_photoDownloadUrl = userData.imageUrl;


//			Debug.LogError( userData.nickname + ":" + userData.userId);

		}
		else
		{
			friendData = (P_FriendRank) obj;
			if(friendData.userId == PandoraManager.instance.localUser.userID)
			{
				lbName.text = "[ffe090]"+Util.GetShortID( GameDataManager.instance.name , 12)+"[-]";
				_photoDownloadUrl = PandoraManager.instance.localUser.image_url;

			}
			else if( epi.GAME_DATA.appFriendDic.ContainsKey( friendData.userId ))
			{
				lbName.text = "[8ef63f]"+Util.GetShortID( epi.GAME_DATA.appFriendDic[friendData.userId].f_Nick , 12)+"[-]";
				_photoDownloadUrl = epi.GAME_DATA.appFriendDic[friendData.userId].image_url;
			}
			else if(GameDataManager.instance.friendDatas.ContainsKey(friendData.userId ))
			{
				lbName.text = "[8ef63f]"+Util.GetShortID( GameDataManager.instance.friendDatas[friendData.userId].nickname, 12)+"[-]";

				if(epi.GAME_DATA.friendDic.ContainsKey(friendData.userId))
				{
					_photoDownloadUrl = epi.GAME_DATA.friendDic[friendData.userId].image_url;
				}
				else
				{
					_photoDownloadUrl = null;
				}
			}
			else
			{
				_photoDownloadUrl = null;
				lbName.text = "친구 아님";
			}

			UIManager.setPlayerPhoto(_photoDownloadUrl, spEmptyFace, face, UIManager.PhotoDownLoadType.Init);

			if(friendData.userId == PandoraManager.instance.localUser.userID)
			{
				spBackground.spriteName = "img_border_listbox1";
				_isMyPanel = true;
			}
			else
			{
				spBackground.spriteName = "img_border_listbox";
			}


			lbRanking.text = friendData.rank.ToString();
			lbScore.text = Util.GetCommaScore( friendData.score );

			spRankingIcon.enabled = (friendData.rank <= 3);

		}
	}

	public static string checkingHellUserName = "";

	public void onClickInfo(GameObject go)
	{
		if(_isMyPanel)
		{
			GameManager.me.uiManager.popupFriendDetail.myData();
		}
		else if(GameManager.me.uiManager.popupHell.list.isFriendType == false)
		{
			checkingHellUserName = lbName.text;
			EpiServer.instance.sendGetHellUserInfo( userData );
		}
		else
		{
			EpiServer.instance.sendGetFriendDetail( friendData.userId);
		}
	}


}
