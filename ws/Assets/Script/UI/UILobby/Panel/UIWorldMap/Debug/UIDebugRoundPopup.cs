using UnityEngine;
using System.Collections;

public class UIDebugRoundPopup : MonoBehaviour {

	public UIButton btnNormal, btnRecommand, btnSelect, btnClose;

	public UILabel lbId;


	void Awake()
	{
		UIEventListener.Get(btnClose.gameObject).onClick = onClickClose;
		UIEventListener.Get(btnNormal.gameObject).onClick = onClickNormal;
		UIEventListener.Get(btnRecommand.gameObject).onClick = onClickRecommand;
		UIEventListener.Get(btnSelect.gameObject).onClick = onClickSelect;
	}

	public void onClickClose(GameObject go)
	{
		gameObject.SetActive(false);
	}


	string _id;

	public UILabel lbTestModeInfo;

	public static bool setMyInfo = true;

	bool _isSigong = false;

	public void show(string id, bool isSigong)
	{
		_id = id;
		lbId.text = id;
		setMyInfo = true;
		_isSigong = isSigong;
	}

	GamePlayerData setTestModeData;

	void onClickNormal(GameObject go)
	{
		string checkId = _id;

		if(_isSigong)
		{
			checkId = GameManager.info.testSigong[_id].roundId;
			if(checkId.StartsWith("PVP"))
			{
				checkId = "PVP";
			}
		}

		if(GameManager.info.roundData.ContainsKey(checkId))
		{
			if(checkId.ToLower().Contains("pvp"))
			{
				if(setMyInfo)
				{
					DebugManager.useTestRound = true;
					setMyInfo = false;
					lbId.text = checkId + ":  PVP 설정" ;
				}
				else
				{
					DebugManager.useTestRound = true;
					GameManager.me.stageManager.setNowRound(GameManager.info.roundData[checkId],GameType.Mode.Championship);
					DebugManager.instance.setDebugPVPData();
					GameManager.me.uiManager.showLoading();
					setMyInfo = true;
					GameManager.me.startGame();
				}
			}
			else
			{
				DebugManager.useTestRound = true;
				GameManager.me.stageManager.setNowRound(GameManager.info.roundData[checkId],GameType.Mode.Epic);
				GameManager.me.uiManager.showLoading();
				
				if(checkId == "INTRO")
				{
					StartCoroutine( GameManager.me.startOpening() );
				}
				else
				{
					GameManager.me.startGame();
				}
			}
		}
	}

	void onClickRecommand(GameObject go)
	{

		string checkId = _id;
		
		if(_isSigong)
		{
			checkId = GameManager.info.testSigong[_id].roundId;
			if(checkId.StartsWith("PVP"))
			{
				checkId = "PVP";
			}
		}


		if(GameManager.info.roundData.ContainsKey(checkId))
		{
			if(GameManager.info.testModeData.ContainsKey(checkId))
			{
				bool isPVPMode = true;
				DebugManager.useTestRound = true;


				if(checkId.ToLower().Contains("pvp") == false)
				{
					GameManager.me.stageManager.setNowRound(GameManager.info.roundData[checkId], GameType.Mode.Epic);
					isPVPMode = false;
					GameManager.me.uiManager.showLoading();
				}
				else
				{
					GameManager.me.stageManager.setNowRound(GameManager.info.roundData[checkId], GameType.Mode.Championship);
				}

				if(checkId == "INTRO")
				{
					StartCoroutine( GameManager.me.startOpening() );
				}
				else
				{
					TestModeData tmd = GameManager.info.testModeData[checkId];
					
					GamePlayerData testModeData = new GamePlayerData(tmd.hero);
					
					string[] u = new string[5];
					u[0] = tmd.u1;
					u[1] = tmd.u2;
					u[2] = tmd.u3;
					u[3] = tmd.u4;
					u[4] = tmd.u5;
					
					string[] s = new string[3];
					s[0] = tmd.s1;
					s[1] = tmd.s2;
					s[2] = tmd.s3;

					if(isPVPMode)
					{
						if(setMyInfo)
						{
							setMyInfo = false;
							lbId.text = _id + "  : PVP 설정";
							DebugManager.instance.setPlayerData(testModeData,true,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.pvpAi);
							setTestModeData = testModeData;
						}
						else
						{
							GameManager.me.uiManager.showLoading();
							DebugManager.instance.pvpPlayerData = new GamePlayerData(tmd.hero);
							DebugManager.instance.setPlayerData(DebugManager.instance.pvpPlayerData,true,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.ai);
							GameManager.me.startGame(1,setTestModeData);
						}
					}
					else
					{
						GameManager.me.uiManager.showLoading();
						DebugManager.instance.setPlayerData(testModeData,true,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.pvpAi);
						GameManager.me.startGame(1,testModeData);
					}

				}
			}
			else
			{
				UISystemPopup.open( UISystemPopup.PopupType.Default, "추천덱이 없습니다.");
			}
		}		
	}

	void onClickSelect(GameObject go)
	{

		string checkId = _id;
		
		if(_isSigong)
		{
			checkId = GameManager.info.testSigong[_id].roundId;
			if(checkId.StartsWith("PVP"))
			{
				checkId = "PVP";
			}
		}

		if(GameManager.info.roundData.ContainsKey(checkId))
		{
			UIDebugTestModeListSlotPanel.selectDebugRoundId = checkId;
			
			GameManager.me.uiManager.uiMenu.uiWorldMap.debugTestModeList.gameObject.SetActive(true);
			GameManager.me.uiManager.uiMenu.uiWorldMap.debugTestModeList.draw();
			
		}
	}


}
