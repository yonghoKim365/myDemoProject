using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreefightPanel : UIBasePanel
{

    public UIEventTrigger BtnChange;  //장비교체
    public UIEventTrigger BtnShop;    //상점
    public UIEventTrigger BtnTip; //팁
    public UIEventTrigger BtnStart;   //시작

    public UILabel NameAndLvLabel;
    public UILabel AttackLabel;
    public UILabel OpenInfoLabel;   //오픈시간  

    public Transform PlayCharRoot;// 플레이 중인 캐릭터 보여줄 곳

    public GameObject Tip;  //팁

    public UIEventTrigger LevelBattle;   //레벨난투장
    public UIEventTrigger FreeBattle;   //자유난투장

    private NetData._UserInfo CharInven;

    private uint MyLevelStageIdx;   //나의레벨에맞는 스테이지 인덱스
    private uint FreeLevelStageIdx; //자유난투장 스테이지 인덱스
    private uint SelectedStageIdx;   //선택한 난투장스테이지번호

    public override void Init()
    {
        SceneManager.instance.sw.Reset();
        SceneManager.instance.sw.Start();
        SceneManager.instance.showStopWatchTimer("Freefight panel, Init() start");

        base.Init();
        Tip.gameObject.SetActive(false);
        CharInven = NetData.instance.GetUserInfo();

        EventDelegate.Set(BtnShop.onClick, delegate ()
        {
            base.Hide();
            UIMgr.OpenShopPanel(this);
        });

        EventDelegate.Set(BtnTip.onClick, delegate ()
        {
            Tip.gameObject.SetActive(true);
        });
        EventDelegate.Set(Tip.transform.FindChild("Close").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            Tip.gameObject.SetActive(false);
        });

        EventDelegate.Set(BtnChange.onClick, delegate ()
        {
            base.Hide();
            UIMgr.OpenEquipPanel(this);
        });

        EventDelegate.Set(LevelBattle.onClick, delegate ()
        {
            OnClickBattleStage(false);
        });
        EventDelegate.Set(FreeBattle.onClick, delegate ()
        {
            OnClickBattleStage(true);
        });

        EventDelegate.Set(BtnStart.onClick, delegate ()
        {
            Debug.Log("Select StageIndex : " + SelectedStageIdx);
            //UIMgr.instance.AddPopup(141, 1233 , 117);

			NetworkClient.instance.SendPMsgMessRoomEnterC(SelectedStageIdx);
        });


        OpenInfoLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1221), 10, 20);//일단임시,,시간보내주면 바꾸기

        SetBattleInfo();   

        NetworkClient.instance.SendPMsgMessQueryC();//조회

		SceneManager.instance.showStopWatchTimer ("Freefight panel, Init() finish");
		SceneManager.instance.sw.Stop ();
    }

    public override void LateInit()
    {
        base.LateInit();


        string nickName = NetData.instance.Nickname;
        string _lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
        NameAndLvLabel.text = string.Format("{0} {1}", _lvStr, nickName);
        AttackLabel.text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), CharInven._TotalAttack);


        //장착중인 파트너 생성
        NetData._CostumeData equipCostumeData = CharInven.GetEquipCostume();
        uint weaponId = 0, clothId = 0, headId = 0;
        if (CharInven.isHideCostum)
        {
            NetData._ItemData head = CharInven.GetEquipParts(ePartType.HELMET);
            NetData._ItemData cloth = CharInven.GetEquipParts(ePartType.CLOTH);
            NetData._ItemData weapon = CharInven.GetEquipParts(ePartType.WEAPON);

            if (head != null)
                headId = head._equipitemDataIndex;

            if (cloth != null)
                clothId = cloth._equipitemDataIndex;

            if (weapon != null)
                weaponId = weapon._equipitemDataIndex;
        }

        UIHelper.CreatePcUIModel("FreefightPanel", PlayCharRoot, CharInven.GetCharIdx(), headId, equipCostumeData._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 3, CharInven.isHideCostum, false);

        if (SceneManager.instance.CurTutorial == TutorialType.FREEFIGHT)
            OnSubTutorial();
    }

    /// <summary>
    /// 레벨에 따른 난투장 셋팅
    /// </summary>
    void SetBattleInfo()
    {
        uint lv = NetData.instance.GetUserInfo()._Level;
        List<DungeonTable.FreefightTableInfo> list = _LowDataMgr.instance.GetLowDataFreeFightList();

        DungeonTable.FreefightTableInfo myLevelStage = null;//나의렙..
        for (int i = 0; i < list.Count; i++)
        {
            if (NetData.instance.UserLevel >= list[i].MinenterLv && list[i].MaxenterLv >= NetData.instance.UserLevel)
            {
                myLevelStage = list[i];
                break;
            }
        }

        LevelBattle.transform.FindChild("Title").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1223), myLevelStage.MinenterLv, myLevelStage.MaxenterLv);
        LevelBattle.transform.FindChild("Explain").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1224), myLevelStage.MinenterLv, myLevelStage.MaxenterLv);
        LevelBattle.transform.FindChild("Mon/icon/icon").GetComponent<UISprite>().spriteName =_LowDataMgr.instance.GetMonsterInfo(uint.Parse(myLevelStage.RegenCBossIdx.list[0])).PortraitId;

        MyLevelStageIdx = myLevelStage.StageIndex;

        //자유
        FreeLevelStageIdx =  list[list.Count - 1].StageIndex;
        FreeBattle.transform.FindChild("Mon/icon/icon").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetMonsterInfo(uint.Parse(list[list.Count - 1].RegenCBossIdx.list[0])).PortraitId;

        //첨에는 일단 일반으로.. 그뒤는 마지막선택했던곳으로 (이건후에)
        OnClickBattleStage(false);


    }
    /// <summary>
    /// 난투장선택
    /// </summary>
    /// <param name="isFree"> 자유? </param>
    void OnClickBattleStage(bool isFree)
    {
        SelectedStageIdx = isFree ? FreeLevelStageIdx : MyLevelStageIdx;
        LevelBattle.transform.FindChild("cover").gameObject.SetActive(!isFree);
        FreeBattle.transform.FindChild("cover").gameObject.SetActive(isFree);

    }



    public override void Hide()
    {
        if (Tip.activeSelf)
        {
            Tip.SetActive(false);
            return;
        }


        base.Hide();
        UIMgr.OpenTown();
    }

}
