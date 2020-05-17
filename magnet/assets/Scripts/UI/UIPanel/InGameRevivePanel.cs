using UnityEngine;
using System.Collections;

public class InGameRevivePanel : UIBasePanel {

    public UIButton GiveupBtn, ReviveBtn;

    public UIButton FreeGemReviveBtn;   //난투장 유료부활.

    public UILabel FreefightReviveGemLabel;
    public UISprite CountDownSeconds;

    enum labelT
    {
	   main, left, right,
    }
    public UILabel[] Labels;

    public GameObject FreefightRevivalPopup;

    private System.DateTime TimeSecond;//난투장 부활시 사용되는 카운트 다운

    System.Action Callback;
    public override void Init()
    {
	    base.Init();
        TimeSecond = System.DateTime.Now.AddSeconds(_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.FreeFightwatingtime));

        transform.FindChild("BtnGroup").gameObject.SetActive(false);
        transform.FindChild("Center").gameObject.SetActive(false);
        FreefightRevivalPopup.SetActive(false);

        G_GameInfo.GameInfo.HudPanel.Hide();

        EventDelegate.Set(GiveupBtn.onClick, OnClickGiveup);
	    EventDelegate.Set(ReviveBtn.onClick, OnClickRevive);

        EventDelegate.Set(FreeGemReviveBtn.onClick, delegate () {

            if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                //돈부족하면 알림
                if (NetData.instance.GetAsset(AssetType.Cash) < _LowDataMgr.instance.GetEtcTableValue<ulong>(EtcID.FreeFightreviveCoin))
                {
                    UIMgr.instance.AddErrorPopup(2752);//원보부족
                    return;
                }

                OnClickRevive();
            }
        });


        CameraManager.instance.SetGrayScale (true);
    }

    public override void LateInit()
    {
	    base.LateInit();
        Callback = (System.Action)parameters[0];

        if((bool)parameters[1]==true)
        {
            //난투장일때임..
            transform.FindChild("BtnGroup").gameObject.SetActive(false);
            transform.FindChild("Center").gameObject.SetActive(false);
            FreefightRevivalPopup.SetActive(true);
            FreefightReviveGemLabel.text = string.Format("{0} {1}", _LowDataMgr.instance.GetEtcTableValue<string>(EtcID.FreeFightreviveCoin) ,_LowDataMgr.instance.GetStringCommon(3));
                
        }
        else
        {
            transform.FindChild("BtnGroup").gameObject.SetActive(true);
            transform.FindChild("Center").gameObject.SetActive(true);
            FreefightRevivalPopup.SetActive(false);
        }
    }

    void Update()
    {
        if (!FreefightRevivalPopup.activeSelf)
            return;
       
        System.TimeSpan seconds = TimeSecond - System.DateTime.Now;

        if (0 <= seconds.TotalSeconds)
        {
            CountDownSeconds.spriteName = ((int)seconds.TotalSeconds).ToString("0");
        }

    }

    public override PrevReturnType Prev()
    {
	   OnClickGiveup();
	   return base.Prev();
    }

    void OnClickGiveup()
    {
		CameraManager.instance.SetGrayScale (false);
        //게임을 끝낸다. -> 결과화면 보여주기?
        G_GameInfo._GameInfo.ForcedGameEnd();
        Close();
    }

    void OnClickRevive()
    {
		CameraManager.instance.SetGrayScale (false);
	   //재화 개수를 확인한다.
	   bool CheckCash = true;
	   if (CheckCash)
	   {
		  //재화 소비해 부활할건지 팝업띄워준다. '예'면 재화소비후 부활 '아니오'면 그냥 팝업닫기.
		  ReviveFunc();
	   }
	   else
	   {
		  //재화 충전 팝업 띄워주기.
	   }
    }
    
    void ReviveFunc()
    {
        /*
        if (SceneManager.instance.IsRTNetwork)
        {
            if (G_GameInfo.GameInfo is FreeFightGameInfo)
                (G_GameInfo.GameInfo as FreeFightGameInfo).RevivePlayer();
        }
        else
        {
            if (G_GameInfo.GameInfo is SingleGameInfo)
                (G_GameInfo.GameInfo as SingleGameInfo).RevivePlayer();
        }

	   //< 부활했을시 처리
	   EventListner.instance.TriggerEvent("Revive");

	   if (null != parentPanel && null != G_GameInfo.GameInfo)// && reviveParentPanel)
		  parentPanel.Show();
       */

        if (SceneManager.instance.IsRTNetwork)
        {
            if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                //여기서 Close()안해주고 부활완료된곳에서 Close처리해줌
                NetworkClient.instance.SendPMsgRoleReliveC();

                if (Callback != null)
                    Callback();

                Callback = null;

                return;
                
            }
        }
       
        if (Callback != null)
            Callback();

        Callback = null;
        Close();
    }


}
