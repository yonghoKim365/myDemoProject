using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComebackPopup : UIBasePanel
{
    public Transform SlotRoot;  //아이템정보슬롯 부모

    public UIAtlas EquipAtlas;
    public UIAtlas ItemAtlas;
    public UIAtlas Face;

    public override void Init()
    {
        base.Init();

        //일단 아이템 셋팅
        List<Welfare.EventCheckInfo> list = new List<Welfare.EventCheckInfo>();
        list = _LowDataMgr.instance.GetLowDataEventCheck(3);

        for (int i = 0; i < SlotRoot.childCount; i++)
        {
            GameObject go = SlotRoot.GetChild(i).gameObject;
            UILabel itemName = go.transform.FindChild("Txt_gold").GetComponent<UILabel>(); //수량
            UISprite itemImg = go.transform.FindChild("get").FindChild("Icon").GetComponent<UISprite>();//아이템 이미지

            List<GatchaReward.FixedRewardInfo> fixList = _LowDataMgr.instance.GetFixedRewardItemGroupList(list[i].RewardId);

            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(fixList[0].ItemId);
            bool isEquip = false;
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                if (itemImg.atlas != EquipAtlas)
                    itemImg.atlas = EquipAtlas;

                itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
                itemName.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(eLowData.NameId), fixList[0].ItemCount);
                isEquip = true;
            }
            else//소모아이템이 드랍아이템 대표로 등록되어 있음
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(fixList[0].ItemId);
                if (uLowData == null)
                {
                    //Debug.LogError("item id error" + list[0].ItemId);
                    //따로체크 
                    if (itemImg.atlas != ItemAtlas)
                        itemImg.atlas = ItemAtlas;
                    isEquip = false;

                    if (list[0].Type == 1)//골드 
                    {
                        itemImg.spriteName = "Icon_10000";
                        itemName.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(4), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 2)//원보 
                    {
                        itemImg.spriteName = "Icon_10001";
                        itemName.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(3), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 10)  //파트너
                    {
                        if (_LowDataMgr.instance.IsGetRewardType(10, fixList[0].ItemId))
                        {
                            if (itemImg.atlas != Face)
                                itemImg.atlas = Face;

                            itemImg.spriteName = _LowDataMgr.instance.GetPartnerInfo(fixList[0].ItemId).PortraitId;
                            itemName.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringUnit(_LowDataMgr.instance.GetPartnerInfo(fixList[0].ItemId).NameId), fixList[0].ItemCount);
                            isEquip = false;
                        }
                    }
                    //continue;
                }
                else
                {
                    if (itemImg.atlas != ItemAtlas)
                        itemImg.atlas = ItemAtlas;

                    itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);
                    itemName.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(uLowData.NameId), fixList[0].ItemCount);

                    isEquip = false;
                }

            }
        }
    }
    public override void LateInit()
    {
        base.LateInit();
        // 여기서 현재 출석현황표시 
        int info = (int)parameters[0];

        /*
         info 
         1 = 1 (1일차보상수령)
         2 = 1 0  --> 0 1 ( 1일차X 2일차O)
         3 = 1 1  --> 1 1 ( 1일차O 2일차O)
         4 = 100  --> 0 0 1 (1일차x 2일차 3일차O)
         5 = 101  --> 1 0 1 (1일차O 2일차x 3일차O)
         */
        SoundManager.instance.PlaySfxSound(eUISfx.UI_achive_attendence_alarm, false);//업적 달성 알림음 / 혜택 달성음

        string dayCnt = System.Convert.ToString(info, 2);//2진수로옴
        List<int> checkCount = new List<int>();
        // 반대로 
        for (int i = dayCnt.Length - 1; i >= 0; i--)
        {
            checkCount.Add(int.Parse(dayCnt.Substring(i, 1)));
        }

        for (int i = 0; i < SlotRoot.childCount; i++)
        {
            GameObject go = SlotRoot.GetChild(i).gameObject;
            UILabel NextDay = go.transform.FindChild("get").FindChild("Txt_Day").GetComponent<UILabel>(); //획득전 나올 선명한날짜
            GameObject Stemp = go.transform.FindChild("get").FindChild("Stemp").gameObject; // 완료 도장

            if (checkCount.Count <= i)
            {
                //아직 미획득상태
                NextDay.gameObject.SetActive(true);
                Stemp.SetActive(false);
            }
            else
            {
                if (checkCount[i] == 0)
                {
                    //획득x
                    NextDay.gameObject.SetActive(true);
                    Stemp.SetActive(false);
                }
                else
                {
                    //획득
                    NextDay.gameObject.SetActive(false);
                    Stemp.SetActive(true);
                }
            }

        }

        StartCoroutine("CloseCoroutines");

    }

    IEnumerator CloseCoroutines()
    {
        float delayTime = 0f;

        while (true)
        {
            delayTime += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);

            if (delayTime > 0.5f)
                break;
        }
        Close();
    }

    public override void Close()
    {
        StopAllCoroutines();
        UIMgr.instance.ComebackDay = 0;

        base.Close();
    }
}
