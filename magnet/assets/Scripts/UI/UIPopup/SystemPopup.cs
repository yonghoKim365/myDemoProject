using UnityEngine;
using System.Collections.Generic;
using System;

public enum PopupType
{
    None = 0, GameQuit, InGamePause, Message, Revive,
}
public class SystemPopup : UIBasePanel {

    public UILabel SubMsg;
    public UILabel MainMsg;
    public UISprite Seconds;

    public UILabel[] BtnLbls;
    public UIEventTrigger[] Btns;
    public UISprite[] BtnSps;

    public Transform BtnGroupTf;
    
    public SceneManager.PopupData CurPopupData;
    
    //private float SaveTimeScale;
    private float TimeSecond = 10;//난투장 부활시 사용되는 카운트 다운. 당장은 가라.

    //private Vector3 BtnGroupOriginPos;
    private DateTime StartTime;//절대값으로 계산하기 위해 사용.

    private string[] OnDataFuction=new string[3];

    public override void Init()
    {
        base.Init();

        EventDelegate.Set(Btns[0].onClick, OnClick_1);
        EventDelegate.Set(Btns[1].onClick, OnClick_2);
        EventDelegate.Set(Btns[2].onClick, OnClick_3);
    }

    public override void LateInit()
    {
        base.LateInit();

        CurPopupData = (SceneManager.PopupData)parameters[0];

        if (CurPopupData.LowDataID != 0)
        {
            string msg = _LowDataMgr.instance.GetLowDataErrorString( (uint)CurPopupData.LowDataID);
            if (string.IsNullOrEmpty(msg) )
            {
                CurPopupData.TitleMsg = _LowDataMgr.instance.GetStringCommon(118);
                CurPopupData.MainMsg = string.Format("enum={0}, number={1}", (Sw.ErrorCode)CurPopupData.LowDataID, CurPopupData.LowDataID);
                CurPopupData.InputBtn_0 = _LowDataMgr.instance.GetStringCommon(117);
            }
            else
            {
                string[] errorSoundStr = new string[] {
                    "Level",
                    "Material_Num",
                    "Coin_Num",
                    "Item",
                    "Gem",
                    "BuyCoinS",
                    "ItemSellS",
                    "SweepS_",
                    "RequireStageId",
                    "Power",
                    "DailyTime",
                    "Shard_Num",
                    "Currency",
                    "Goods_Count",
                    "Attack",
                };

                string errorCode = ((Sw.ErrorCode)CurPopupData.LowDataID).ToString();
                for(int i=0; i < errorSoundStr.Length; i++)
                {
                    if ( !errorCode.Contains(errorSoundStr[i]))
                        continue;

                    SoundManager.instance.PlaySfxSound(eUISfx.UI_reset_warn_negation, false);
                    break;
                }

                if (CurPopupData.LowDataID == (int)Sw.ErrorCode.ER_StageCompleteS_Stage_Check_Error)
                    OnDataFuction[0] = "logout";
                else if (CurPopupData.LowDataID == (int)Sw.ErrorCode.ER_Friend_Add_Friend_Again_Cd)
                    msg = string.Format(msg, _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.AddFriendagain));

                CurPopupData.MainMsg = msg;
            }
        }

        SetStringPopup();
    }

    /// <summary> Table을 통해서 팝업을 셋팅함. </summary>
    void SetStringPopup()
    {
        SubMsg.text = CurPopupData.TitleMsg;
        MainMsg.text = CurPopupData.MainMsg;

        int btnCount = 0;
        Vector3 pos = Btns[0].transform.parent.localPosition;
        if (!string.IsNullOrEmpty(CurPopupData.InputBtn_0))
            ++btnCount;

        if (!string.IsNullOrEmpty(CurPopupData.InputBtn_1))
            ++btnCount;

        if (!string.IsNullOrEmpty(CurPopupData.InputBtn_2))
            ++btnCount;

        Color[] colr = new Color[3];
        if (btnCount == 1)
        {
            pos.x = 0;
            colr[0] = new Color32(135, 242, 184, 255);
        }
        else if (btnCount == 2)
        {
            pos.x = 80;
            colr[0] = new Color32(135, 242, 184, 255); 
            colr[1] = Color.white;
        }
        else
        {
            pos.x = 0;
            colr[0] = new Color32(135, 242, 184, 255); 
            colr[1] = Color.white;
            colr[2] = Color.white;
        }
        
        string[] inputBtns = new string[] { CurPopupData.InputBtn_0, CurPopupData.InputBtn_1, CurPopupData.InputBtn_2 };
        Btns[0].transform.parent.localPosition = pos;
        for (int i = 0; i < 3; i++)
        {
            if (string.IsNullOrEmpty(inputBtns[i]))
                Btns[i].gameObject.SetActive(false);
            else
            {
                Btns[i].gameObject.SetActive(true);
                BtnLbls[i].text = inputBtns[i];
                BtnSps[i].color = colr[i];
            }
        }
    }

    void OnTableFuction(string key)
    {
        if(key == "logout")
        {
            SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () => { 
                UITextList.ClearTextList();
                UIMgr.ClearUI(true);

                NetworkClient.instance.DisconnectGameServer();//연결 종료
                NetData.instance.InitUserData();
                NetData.instance.ClearCharIdc();

                SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
            });
        }

    }

    //Hide함수 기존의 뒤로가기 키로 갔을때를 대비하여 구동방식대로 작성.
    public override void Hide()
    {
        if (Btns[2].gameObject.activeSelf)//취소
        {
            if (!string.IsNullOrEmpty(OnDataFuction[2]))
            {
                OnTableFuction(OnDataFuction[2]);
                OnDataFuction[2] = null;
            }

            if (CurPopupData.OnCallBack_3 != null)
                CurPopupData.OnCallBack_3();
        }
        else if (Btns[1].gameObject.activeSelf)//하나뿐
        {
            if (!string.IsNullOrEmpty(OnDataFuction[1]))
            {
                OnTableFuction(OnDataFuction[1]);
                OnDataFuction[1] = null;
            }

            if (CurPopupData.OnCallBack_2 != null)
                CurPopupData.OnCallBack_2();
        }
        else if (Btns[0].gameObject.activeSelf)
        {
            if (!string.IsNullOrEmpty(OnDataFuction[0]))
            {
                OnTableFuction(OnDataFuction[0]);
                OnDataFuction[0] = null;
            }

            if (CurPopupData.OnCallBack_1 != null)
                CurPopupData.OnCallBack_1();
        }

        OnEnd();
    }

    /// <summary>
    /// 1번 버튼 클릭 2개or 3개 일 경우
    /// </summary>
    void OnClick_1()
    {
        if (Btns[0].gameObject.activeSelf)
        {
            if (!string.IsNullOrEmpty(OnDataFuction[0]))
            {
                OnTableFuction(OnDataFuction[0]);
                OnDataFuction[0] = null;
            }

            if (CurPopupData.OnCallBack_1 != null)
                CurPopupData.OnCallBack_1();
        }

        OnEnd();// Hide();
    }
    /// <summary>
    /// 2번 버튼 여기는 1번 버튼이 없을 경우에도 실행 가능. 혼자 or 3개일 경우
    /// </summary>
    void OnClick_2()
    {
        if (Btns[1].gameObject.activeSelf)
        {
            if (!string.IsNullOrEmpty(OnDataFuction[1]))
            {
                OnTableFuction(OnDataFuction[1]);
                OnDataFuction[1] = null;
            }

            if (CurPopupData.OnCallBack_2 != null)
                CurPopupData.OnCallBack_2();
        }

        OnEnd();//Hide();
    }

    /// <summary>
    /// 3번 버튼 여기는 1번 버튼이 있을 경우에만 적용됨. 2개 or 3개
    /// </summary>
    void OnClick_3()
    {
        if (Btns[2].gameObject.activeSelf)
        {
            if (!string.IsNullOrEmpty(OnDataFuction[2]))
            {
                OnTableFuction(OnDataFuction[2]);
                OnDataFuction[2] = null;
            }

            if (CurPopupData.OnCallBack_3 != null)
                CurPopupData.OnCallBack_3();
        }

        OnEnd();//Hide();
    }

    //이곳에서 base.Hide를 실행. 실질적인 상속받은 Hide
    public void OnEnd()
    {
        base.Hide();
        SceneManager.instance.CurPopupId = -1;

        if (0 < TimeSecond)
        {
            TimeSecond = -1;
            Seconds.gameObject.SetActive(false);
        }

        //if (IsTimeScaleZero)
        //{
        //    Time.timeScale = SaveTimeScale;
        //    SaveTimeScale = 1;
        //}
    }

    /// <summary> 해당 시간안에 버튼을 클릭해야 할 경우. 외부에서 계산된 시간을 넣어주면 됨.</summary>
    void SetSeconds(float time)
    {
        Seconds.gameObject.SetActive(true);
        TimeSecond = time;
        StartTime = DateTime.Now;
    }
    
    void Update()
    {
        if (!gameObject.activeSelf)
            return;
        else if (TimeSecond < 0)
            return;

        float seconds = TimeSecond - (DateTime.Now - StartTime).Seconds;
        if (0 <= seconds)
            Seconds.spriteName = seconds.ToString("0");
        else
        {
            TimeSecond = -1;
            Seconds.gameObject.SetActive(false);
        }
    }

}
