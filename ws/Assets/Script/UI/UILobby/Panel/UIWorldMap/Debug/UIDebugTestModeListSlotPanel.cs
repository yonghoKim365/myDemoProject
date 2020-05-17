using UnityEngine;
using System.Collections;
using System;

public class UIDebugTestModeListSlotPanel : UIListGridItemPanelBase 
{
	public static string selectDebugRoundId = "";

	public UIButton btnStart;
	public UILabel lbTitle;

	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnStart.gameObject).onClick = onClickRound;
	}

	static GamePlayerData setTestModeData;
	void onClickRound(GameObject go)
	{
		if(WindRunnerMain.instance != null || CutSceneManager.nowOpenCutScene) return;

		if(GameManager.info.roundData.ContainsKey(selectDebugRoundId))
		{
			bool isPVPMode = true;
			DebugManager.useTestRound = true;

			if(selectDebugRoundId.ToLower().Contains("pvp") == false)
			{
				GameManager.me.stageManager.setNowRound(GameManager.info.roundData[selectDebugRoundId], GameType.Mode.Epic);
				isPVPMode = false;
				GameManager.me.uiManager.showLoading();
			}
			else
			{
				GameManager.me.stageManager.setNowRound(GameManager.info.roundData[selectDebugRoundId], GameType.Mode.Championship);
			}

			if(selectDebugRoundId == "INTRO")
			{
				StartCoroutine( GameManager.me.startOpening() );
			}
			else
			{
				TestModeData tmd = GameManager.info.testModeData[_id];

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

				if(isPVPMode == false)
				{
					GameManager.me.uiManager.showLoading();
					DebugManager.instance.setPlayerData(testModeData,true,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.ai);
					GameManager.me.startGame(1,testModeData);
				}
				else
				{
					if(UIDebugRoundPopup.setMyInfo)
					{
						UIDebugRoundPopup.setMyInfo = false;
						DebugManager.instance.setPlayerData(testModeData,true,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.ai);
						GameManager.me.uiManager.uiMenu.uiWorldMap.debugRoundList.askPopup.lbId.text = "PVP 설정";
						GameManager.me.uiManager.uiMenu.uiWorldMap.debugTestModeList.gameObject.SetActive(false);
						setTestModeData = testModeData;
					}
					else
					{
						DebugManager.instance.pvpPlayerData = new GamePlayerData(tmd.hero);
						DebugManager.instance.setPlayerData(DebugManager.instance.pvpPlayerData,false,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s,DebugManager.instance.pvpAi);
						GameManager.me.uiManager.showLoading();
						GameManager.me.startGame(1,setTestModeData);
					}
				}
			}
		}
	}

	public override void setPhotoLoad()
	{

	}	

	string _id;
	public override void setData(object obj)
	{
		_id = (string)obj;
		lbTitle.text = _id;
	}

}
