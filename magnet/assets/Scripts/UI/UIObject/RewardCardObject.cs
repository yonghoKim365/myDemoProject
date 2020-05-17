using UnityEngine;
using System.Collections;

public class RewardCardObject : MonoBehaviour {
    
    public GameObject RewardGroup, CostGroup;
    public GameObject ItemSlotPrefab;
    //public GameObject DragonImgGo;//끄고 키는 용도.
    public GameObject CardOpenEffGo;//카드 오픈 이펙트
    public UISprite Img;

    public UILabel CostLabel;
    
    private UIPlayTween PlayTween;
    
    private System.Action<int> ClickCallback;// 부모패널에서 받아온 콜백
    private InvenItemSlotObject ItemSlotObj;

    private int CardArray;

    /// <summary> 보상 세팅 </summary>
    public void SetRewardCard(System.Action<int> _clickCallback, uint lowDataId, uint amount, int arr)
    {
        GameObject slot = Instantiate(ItemSlotPrefab) as GameObject;
        slot.transform.parent = RewardGroup.transform;
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        ItemSlotObj = slot.GetComponent<InvenItemSlotObject>();

        CardArray = arr;
        ClickCallback = _clickCallback;

        UIEventListener.Get(gameObject).onClick = CardClickEvent;
        
        CostGroup.SetActive(false);
        ActiveCollider(false);
        PlayTween = GetComponentInParent<UIPlayTween>();

        Img.spriteName = "reward_pendant_back";
        CardOpenEffGo.SetActive(false);

        ItemSlotObj.SetLowDataItemSlot(lowDataId, amount);
        
        CostLabel.text = string.Format("{0}", _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.PassCard) );
    }
    
    /// <summary> 해당 카드가 눌렸을때 들어오는 함수 처리는 부모 패널에서 하도록한다. </summary>
    void CardClickEvent(GameObject go)
    {
        ClickCallback(CardArray);
    }

    /// <summary>
    /// 부모 패널에서 실질적인 선택이 이뤄지면 들어오는 함수
    /// </summary>
    public void OpeningEvent(uint lowDataId, ushort amount)
    {
        ActiveCollider(false);
        CostGroup.SetActive(false);
        CardOpenEffGo.SetActive(true);

        TweenRotation rot = GetComponentInParent<TweenRotation>();//시작은 로테이션
        rot = TweenRotation.Begin(transform.parent.gameObject, rot.duration, Quaternion.Euler(-rot.from));
        rot.onFinished.Clear();

        TempCoroutine.instance.FrameDelay(rot.delay + (rot.duration * 0.5f), () => {
            Img.spriteName = "reward_pendant_back";
            ItemSlotObj.SetLowDataItemSlot(lowDataId, amount);

            RewardGroup.SetActive(true);
            CostGroup.SetActive(false);
        });

        //EventDelegate.Set(rot.onFinished, delegate () {
            
        //    ItemSlotObj.SetLowDataItemSlot(lowDataId, amount);

        //    RewardGroup.SetActive(true);
        //    CostGroup.SetActive(false);

        //});

        rot.ResetToBeginning();
        rot.Play(true);
    }
    
    public void CostShowEvent()
    {
	   CostGroup.SetActive(true);
    }
    
    /// <summary> 카드 이펙트 실행 함수 </summary>
    public void StartCardAction()
    {
        ActiveCollider(false);
        RewardGroup.SetActive(false);

        TweenRotation rot = GetComponentInParent<TweenRotation>();//시작은 로테이션
        rot.ResetToBeginning();
        rot.PlayForward();

        TempCoroutine.instance.FrameDelay(rot.delay + (rot.duration * 0.5f), () => {
            Img.spriteName = "reward_pendant_01";
        });

        EventDelegate.Set(rot.onFinished, GroupOneTweenPos);
    }
    
    void GroupOneTweenPos()//로테이션 다음에 가운데로 움직임
    {
        PlayTween.tweenGroup = 1;
        PlayTween.Play(true);

        EventDelegate.Set(PlayTween.onFinished, GroupTwoTweenPos);
    }

    void GroupTwoTweenPos()//다음에 원위치
    {
        PlayTween.tweenGroup = 1;
        PlayTween.Play(false);

        EventDelegate.Set(PlayTween.onFinished, EndCardAction );
    }
    
    void EndCardAction()
    {
        ActiveCollider(true);
    }

    public void ActiveCollider(bool active)
    {
        gameObject.collider.enabled = active;
    }
}
