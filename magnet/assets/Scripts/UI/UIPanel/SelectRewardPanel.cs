using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 스테이지 종료 후 보상선택 패널 여기는 싱글스테이지에서 승리했을 경우에만 들어온다.
/// </summary>
public class SelectRewardPanel : UIBasePanel {

    public GameObject OriginalRewardGo;//보상오브젝트 원본 프리팹
    public Transform[] RewardRootTrs;//보상오브젝트가 위치할 부모들
    public GameObject HaveCashGroup;//보유하고있는 캐쉬그룹

    public UILabel HaveCashLabel;//보유캐쉬량
    //public UILabel TitleLabel;//타이틀 라벨
    //public UILabel ContinueLabel;//계속하기 버튼의 라벨

    public UIButton ContinueBtn;//계속하기 버튼

    private bool IsSelect;

    private RewardCardObject[] RewardCards;

    private int GoldCardIdx = -1;
    private int CashCardIdx = -1;
    //private bool isSelect = false;//무료보상선택했는지 여부
    bool IsCallReward;
    

    public override void Init()
    {
	    base.Init();
        IsCallReward = false;

        EventDelegate.Set(ContinueBtn.onClick, Close);
     }

    public override void LateInit()
    {
        base.LateInit();
        
        ulong ownCash = NetData.instance.GetAsset(AssetType.Cash);
        //HaveCashLabel.text = ownCash == 0 ? "0" : ownCash.ToString(); // ToString("#,##");
		HaveCashLabel.text = ownCash.ToString ();
        
        StartCoroutine( "SelectRewardProcess" );

        //UIBasePanel panel = UIMgr.instance.FindInShowing("UIPopup/ChangePopup");
        //if (panel != null)
        //{
        //    (panel as ChangePopup).CheckStateActivefalse();
        //}
    }

    /// <summary> 보상 선택 프로세스 </summary>
    IEnumerator SelectRewardProcess()
    {
        //보상 오브젝트 4개 생성
        List<NetData.DropItemData> cardList = NetData.instance.GetRewardCardItem();
        int count = cardList.Count;
        RewardCards = new RewardCardObject[count];

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(OriginalRewardGo) as GameObject;
            go.transform.parent = RewardRootTrs[i];
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            RewardCards[i] = go.GetComponent<RewardCardObject>();
            RewardCards[i].SetRewardCard( ClickCardObject, cardList[i].LowDataId, cardList[i].Amount, i+1);//SingleClearInfo.reward_list[i]);
        }

        SoundManager.instance.PlaySfxSound(eUISfx.UI_reward_preview, false);
        yield return new WaitForSeconds(2f);//연출 시작전에 2초간 뭐가 있는지 보여줌

        SoundManager.instance.PlaySfxSound(eUISfx.UI_reward_hide, false);

        //뒤집는다.//연출 시작
        for (int i=0; i < RewardCards.Length; i++)
        {
            RewardCards[i].StartCardAction();
        }
        
		if (SceneManager.instance.testData.bSingleSceneTestStart) {
			yield return new WaitForSeconds(4f);

			ClickCardObject(Random.Range(1,4));

			yield return new WaitForSeconds(2f);

			Close ();

		}
		if (SceneManager.instance.testData.bQuestTestStart) {
			yield return new WaitForSeconds(4f);
			
			ClickCardObject(Random.Range(1,4));
			
			yield return new WaitForSeconds(2f);
			
			Close ();
			
		}
        yield return null;
    }

    /// <summary> 닫기 버튼 여기서는 계속하기 버튼임. </summary>
    public override void Close()
    {
        if (GoldCardIdx == -1 || IsCallReward)//보상을 받지 않이함. 무시한다.
        {
            if(GoldCardIdx == -1)
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 915);

            return;
        }

        ContinueBtn.enabled = false;
        IsCallReward = true;

        if (UIMgr.GetUIBasePanel("UIPanel/ResultRewardStarPanel") != null)
            return;

        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
            UIMgr.Open("UIPanel/ResultRewardStarPanel", true);

        base.Close();
    }

	void ClickCardObject(int cardArr)// cardArr is 1~4
    {
        if (IsSelect)
            return;

        if (0 <= GoldCardIdx)
        {
            if (GoldCardIdx != cardArr-1 && CashCardIdx < 0)
            {
                ulong ownCash = NetData.instance.GetAsset(AssetType.Cash);
                ulong cash = (ulong)_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.PassCard);
                if (ownCash < cash)
                {
                    UIMgr.instance.AddErrorPopup((int)Sw.ErrorCode.ER_StageFlopS_Gem_Error);
                    return;
                }

                CashCardIdx = cardArr - 1;
            }
            else
                return;
        }
        else
            GoldCardIdx = cardArr - 1;

        IsSelect = true;
        //NetworkClient.instance.SendPMsgStageFlopC(NetData.instance._RewardData.StageId, 0 <= CashCardIdx, cardArr);
    }

    /// <summary> 카드 오픈 보상. </summary>
    public void PMsgCardOpen(uint itemLowDataId, int isCash, ushort amount)
    {
        if (isCash == 2)
        {
            for (int i = 0; i < RewardCards.Length; i++)
            {
                RewardCards[i].ActiveCollider(false);
                RewardCards[i].CostGroup.SetActive(false);
            }
            
            RewardCards[CashCardIdx].OpeningEvent(itemLowDataId, amount);
            NetData.instance.AddGetRewardItem(itemLowDataId, amount);

            ulong ownCash = NetData.instance.GetAsset(AssetType.Cash);
            //HaveCashLabel.text = ownCash == 0 ? "0" : ownCash.ToString(); // ToString("#,##");
			HaveCashLabel.text = ownCash.ToString();
        }
        else// if(CashCardIdx < 0)
        {
            RewardCards[GoldCardIdx].OpeningEvent( itemLowDataId, amount);
            NetData.instance.AddGetRewardItem(itemLowDataId, amount);

            for (int i = 0; i < RewardCards.Length; i++)
            {
                if (i != (GoldCardIdx) )//선택 않된 녀석만 오픈
                    RewardCards[i].CostShowEvent();
            }
        }

        IsSelect = false;
        SoundManager.instance.PlaySfxSound(eUISfx.UI_reward_choice, false);
    }

    /// <summary> 카드 보상받는데 에러가 낫다 초기화 해준다. </summary>
    public void OnErrorResetReward()
    {
        if(0 <= CashCardIdx)
        {
            CashCardIdx = -1;
        }
        else if(0 <= GoldCardIdx)
        {
            GoldCardIdx = -1;
        }

        IsSelect = false;
    }
}
