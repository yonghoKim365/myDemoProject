using UnityEngine;
using System.Collections.Generic;

public class LoadingTipPanel : UIBasePanel
{

    public GameObject BlackSprite, InfoPanel;
    public GameObject[] LoadingBgs;//0 일반, 1 1:1
    public GameObject[] LoadingChar;//0 권, 1 명, 2 의

    public UISprite[] MyPartners;   //나의 파트너
    public UISprite[] EnemyPartners;    //적으파트너 

    public Transform Indicator;
    public UISlider LoadingBar;
    public UITexture[] Texture;
    public UILabel Desc;
    public System.Action LoadingCallBack;
    public UILabel Percent;
    public object[] SubParams;

    public GameObject BarEff;//바이펙트

    private float startPos = -580;
    private GAME_MODE CurGameMode;

    public override void Init()
    {
        base.Init();
        transform.FindChild("NormalLoading/Version").GetComponent<UILabel>().text
                    = string.Format(_LowDataMgr.instance.GetStringCommon(1011), BatteryLevel.GetCFBundleVersion());

        Percent.text = string.Format("0 / 100");

        //Fx_UI_chagwan_glow_G//간체
        //Fx_UI_chagwan_glow_B//번체
        UIHelper.CreateEffectInGame(LoadingBgs[1].transform.FindChild("Title"), "Fx_UI_chagwan_glow" + SystemDefine.LocalEff);
    }

    //< 꺼질때에는 사용한 애셋을 메모리해제시킨다.
    //string SelectTexureName = "";
    void OnDisable()
    {
        //추후에 로딩이미지 어셋번들로 사용할때는 이부분 활용

        //AssetbundleLoader.RemoveTextureAsset(SelectTexureName);
        //SelectTexureName = "";
    }

    public override void LateInit()
    {
        base.LateInit();

        prevLoadingTime = 0f;
        Indicator.transform.SetLocalX(-572);
        //loadPercent = 0f;
        LoadingBar.value = 0f;
        BarEff.transform.SetLocalScaleX(0); //일단영


        SceneManager.instance.AddIndicator();
        //SceneManager.eLoadingTipType TipType = (SceneManager.eLoadingTipType)parameters[0];
        CurGameMode = (GAME_MODE)parameters[0];

        TempCoroutine.instance.FrameDelay(0f, () =>
        {
            SceneManager.instance.RemoveIndicator();
            if (LoadingCallBack != null)
                LoadingCallBack();
            LoadingCallBack = null;

        });

        if (CurGameMode == GAME_MODE.ARENA)
        {
            LoadingBgs[0].SetActive(false);
            LoadingBgs[1].SetActive(true);

            Percent = LoadingBgs[1].transform.FindChild("loading").GetComponent<UILabel>();
            Percent.text = string.Format("0%");

            Transform lTf = LoadingBgs[1].transform.FindChild("Lhero");
            Transform rTf = LoadingBgs[1].transform.FindChild("Rhero");
            Transform title = LoadingBgs[1].transform.FindChild("Title");

            TweenScale titleScale = title.GetComponent<TweenScale>();
            TweenPosition lPos = lTf.GetComponent<TweenPosition>();
            TweenPosition rPos = rTf.GetComponent<TweenPosition>();
            titleScale.ResetToBeginning();
            lPos.ResetToBeginning();
            rPos.ResetToBeginning();

            titleScale.PlayForward();
            lPos.PlayForward();
            rPos.PlayForward();
            
            List<PlayerUnitData> list = NetData.instance._playerSyncData.playerSyncDatas;
            List<PlayerUnitData> partList = NetData.instance._playerSyncData.partnerSyncDatas;

            List<PlayerUnitData> myPartList = new List<PlayerUnitData>();
            List<PlayerUnitData> enemyPartList = new List<PlayerUnitData>();

            for(int i=0;i<partList.Count; i++)
            {
                if (partList[i]._TeamID == (int)eTeamType.Team1)
                    myPartList.Add(partList[i]);
                else
                    enemyPartList.Add(partList[i]);
            }


            for (int i = 0; i < list.Count; i++)
            {

                if (list[i]._TeamID == (int)eTeamType.Team1)//나(Left)
                {
                    lTf.FindChild("Txt_power").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), (int)SubParams[0]);
                    lTf.FindChild("Txt_levelnum").GetComponent<UILabel>().text = string.Format("{0}", list[i]._Level);
                    lTf.FindChild("Txt_name").GetComponent<UILabel>().text = string.Format("{0}", list[i]._Name);
                    lTf.FindChild("Txt_GuildName").GetComponent<UILabel>().text = (string)SubParams[2];
                    lTf.FindChild("port").GetComponent<UISprite>().spriteName = UIHelper.GetClassPortIcon(list[i]._charIdx, 2);
                    lTf.FindChild("Txt_rank").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(522), ArenaGameState.MyRank);

                    //파트너잇는지 ?
                    for (int j=0;j < MyPartners.Length; j++)
                    {
                        if(j >= myPartList.Count)
                        {
                            MyPartners[j].gameObject.SetActive(false);
                            continue;
                        }
                        MyPartners[j].spriteName = _LowDataMgr.instance.GetPartnerInfo(myPartList[j]._partnerID).PortraitId;
                        MyPartners[j].gameObject.SetActive(true);
                    }



                }
                else//적(Right)
                {
                    rTf.FindChild("Txt_power").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), (int)SubParams[1]);
                    rTf.FindChild("Txt_levelnum").GetComponent<UILabel>().text = string.Format("{0}", list[i]._Level);
                    rTf.FindChild("Txt_name").GetComponent<UILabel>().text = string.Format("{0}", list[i]._Name);
                    rTf.FindChild("Txt_GuildName").GetComponent<UILabel>().text = (string)SubParams[3];
                    rTf.FindChild("port").GetComponent<UISprite>().spriteName = UIHelper.GetClassPortIcon(list[i]._charIdx, 2);
                    rTf.FindChild("Txt_rank").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(522), ArenaGameState.TargetRank);

                    //파트너잇는지 ?
                    for (int j = 0; j < EnemyPartners.Length; j++)
                    {
                        if (j >= enemyPartList.Count)
                        {
                            EnemyPartners[j].gameObject.SetActive(false);
                            continue;
                        }
                        EnemyPartners[j].spriteName = _LowDataMgr.instance.GetPartnerInfo(enemyPartList[j]._partnerID).PortraitId;
                        EnemyPartners[j].gameObject.SetActive(true);
                    }

                }

               
            }

            ArenaLoadingRrev(lTf, rTf, title);
            List<Loading.LoadingInfo> infoList = _LowDataMgr.instance.GetLowDataLoading((byte)CurGameMode);

            int imgArr = Random.Range(0, infoList.Count);
            Loading.LoadingInfo info = infoList[imgArr];
            Texture tx = Resources.Load("UI/LoadingImg/" + info.Loadimg) as Texture;
            Texture[1].mainTexture = tx;
        }
        else
        {
            Percent = LoadingBgs[0].transform.FindChild("Loading/LoadLabel").GetComponent<UILabel>();
            Percent.text = string.Format("0 / 100");
            uint charIdx = NetData.instance.GetUserInfo()._userCharIndex;
            LoadingChar[0].SetActive(charIdx == 11000);
            LoadingChar[1].SetActive(charIdx == 12000);
            LoadingChar[2].SetActive(charIdx == 13000);

            LoadingBgs[0].SetActive(true);
            LoadingBgs[1].SetActive(false);

            byte modeType = (byte)CurGameMode;

            List<Loading.LoadingInfo> infoList = _LowDataMgr.instance.GetLowDataLoading(modeType);
            if (infoList == null || infoList.Count == 0)
            {
                if (modeType == 0)
                {
                    int rndKey = Random.Range(533, 536);//1을 더 추가함
                    Desc.text = _LowDataMgr.instance.GetStringCommon((uint)rndKey);
                    return;
                }
                infoList = _LowDataMgr.instance.GetLowDataLoading(0);
            }

            int imgArr = Random.Range(0, infoList.Count);
            int textArr = Random.Range(0, infoList.Count);

            Loading.LoadingInfo info = infoList[imgArr];
            Texture tx = Resources.Load("UI/LoadingImg/" + info.Loadimg) as Texture;
            Texture[0].mainTexture = tx;

            Desc.text = _LowDataMgr.instance.GetStringCommon(infoList[textArr].Loadtext);
        }

        //  StartCoroutine(waitingLoading());
    }

    //퍼센트올라가기전에 연출
    public float prevLoadingTime = 0f;
    public void ArenaLoadingRrev(Transform lTf, Transform rTf, Transform title)
    {
        //잠시
        TweenPosition Ltp = lTf.GetComponent<TweenPosition>();
        if (Ltp != null)
        {
            Ltp.ResetToBeginning();
            Ltp.PlayForward();
        }
        TweenPosition Rtp = rTf.GetComponent<TweenPosition>();
        if (Rtp != null)
        {
            Rtp.ResetToBeginning();
            Rtp.PlayForward();
        }

        prevLoadingTime += Ltp.duration;
        TweenScale ts = title.GetComponent<TweenScale>();
        prevLoadingTime += ts.duration;

        TempCoroutine.instance.FrameDelay(0.1f, () =>
        {
            if (ts != null)
            {
                ts.ResetToBeginning();
                ts.PlayForward();
            }
        });


    }
    public void changeLoadingBar(float value)
    {
        LoadingBar.value = value;
        Indicator.localPosition = new Vector3(value * LoadingBar.foregroundWidget.localSize.x + startPos, Indicator.localPosition.y, Indicator.localPosition.z);

        if (CurGameMode == GAME_MODE.ARENA)
            Percent.text = string.Format("{0}%", (value * 100f).ToString("N0"));
        else
            Percent.text = string.Format("{0} / 100", (value * 100f).ToString("N0"));

        //// 바 이펙트 스케일처리
        //if (LoadingBar.value < 0.08)
        //    BarEff.transform.localScale = new Vector3((LoadingBar.value / 0.01f) * 0.13f, BarEff.transform.localScale.y, BarEff.transform.localScale.z);
        // 바 이펙트 스케일처리
        if (LoadingBar.value < 0.23)
            BarEff.transform.localScale = new Vector3((LoadingBar.value / 0.01f) * 0.045f, BarEff.transform.localScale.y, BarEff.transform.localScale.z);

        else
            BarEff.transform.localScale = new Vector3(1f, BarEff.transform.localScale.y, BarEff.transform.localScale.z);


    }

    public override void Hide()
    {
        base.Hide();
        SubParams = null;
    }
    
    public override void HideEvent()
    {
        base.HideEvent();
        SceneManager.instance.HideRoot();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        GUI.Label(new Rect(700, 0, 300, 40), "로딩팁패널상태\n Show : " + gameObject.activeSelf);
        //LongtuIdStr = GUI.TextField(new Rect(700, 40, 150, 20), LongtuIdStr);
    }
#endif
}
