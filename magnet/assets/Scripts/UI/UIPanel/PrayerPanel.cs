using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrayerPanel : UIBasePanel
{
    public UIButton BtnClose;
    public Transform ItemRoot;

    public UIAtlas EquipAtlas;
    public UIAtlas UseAtlas;
    public UIAtlas Face;

    private List<uint> ids;
    private List<uint> type;

    public override void Init()
    {
        base.Init();
        ids = null;
        EventDelegate.Set(BtnClose.onClick, delegate () {
            Close();
            UIMgr.OpenGuildPanel();
        });
    }

    public override void LateInit()
    {
        base.LateInit();

        // 아이템id
        ids = (List<uint>)parameters[0];
        //타입
        type = (List<uint>)parameters[1];

        SetItem();
    }

    void SetItem()
    {
        for (int i = 0; i < ids.Count; i++)
        {
            Transform slot = ItemRoot.GetChild(i);

            UISprite img = slot.GetComponent<UISprite>();

            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(ids[i]);
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                if (img.atlas != EquipAtlas)
                    img.atlas = EquipAtlas;

                img.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
            }
            else//소모아이템이 드랍아이템 대표로 등록되어 있음
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(ids[i]);
                if (uLowData == null)
                {
                    //Debug.LogError("item id error" + ids[i]);
                    //continue;
                    //따로체크 
                    if (img.atlas != UseAtlas)
                        img.atlas = UseAtlas;

                    if (type[i] == 1)//골드 
                    {
                        img.spriteName = "Icon_10000";
                    }
                    if (type[i] == 2)//원보 
                    {
                        img.spriteName = "Icon_10001";
                    }
                    if (type[i] == 3)//공헌
                    {
                        img.spriteName = "Icon_10006";
                    }
                    if (type[i] == 4)//명ㅇㅖ
                    {
                        img.spriteName = "Icon_10007";
                    }
                    if (type[i] == 5)//휘장
                    {
                        img.spriteName = "Icon_10002";
                    }
                    if (type[i] == 6)//사자휘장
                    {
                        img.spriteName = "Icon_10008";
                    }
                    if (type[i] == 7)//성망
                    {
                        img.spriteName = "Icon_10005";
                    }
                    if (type[i] == 8)//체력
                    {
                        img.spriteName = "Icon_10009";
                    }
                    if (type[i] == 10)  //파트너
                    {
                        if (_LowDataMgr.instance.IsGetRewardType(10, ids[i]))
                        {
                            if (img.atlas != Face)
                                img.atlas = Face;

                            img.spriteName = _LowDataMgr.instance.GetPartnerInfo(ids[i]).PortraitId;
                        }
                    }

                }
                else
                {
                    if (img.atlas != UseAtlas)
                        img.atlas = UseAtlas;

                    img.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);

                }


            }


            // 아이템 상세팝업
            UIEventTrigger triItem = slot.GetComponent<UIEventTrigger>();
            int idx = i;
            EventDelegate.Set(triItem.onClick, delegate () { OnclicItemPopup(ids[idx]); });

        }

    }
    /// <summary> 아이템 상세창 </summary>
    void OnclicItemPopup(uint id)
    {
        if (id == 0)
            return;

        UIMgr.OpenDetailPopup(this, id);
        //Vector2 position = pos.transform.position;
        //UIMgr.OpenClickPopup(id, position);
    }
}
