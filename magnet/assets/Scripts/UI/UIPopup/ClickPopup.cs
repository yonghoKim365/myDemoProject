using UnityEngine;
using System.Collections;

public class ClickPopup : UIBasePanel {

    //public UILabel NameLbl;//이름
    //public UILabel AbilityLbl;//고정 능력치
    //public UILabel RndAbilityLbl;//랜덤 능력치
    //public UILabel DescLbl;//설명

    public Transform EquipItemTf;
    public Transform UseItemTf;

    public GameObject InvenItemPrefab;//ItemRoot 밑에 만들 아이템 프리팹
    
    private InvenItemSlotObject InvenSlot;
    //public string[] GradeNameColor = { "[FFFFFF]", "[5BAB37]", "[237EB3]", "[7842A8]", "[A2712C]", "[B19E24]", "[9E3234]" };

    public override void Init()
    {
        base.Init();

        GameObject invenGo = Instantiate(InvenItemPrefab) as GameObject;
        invenGo.transform.parent = transform;
        invenGo.transform.localPosition = Vector3.zero;
        invenGo.transform.localScale = Vector3.one;

        InvenSlot = invenGo.GetComponent<InvenItemSlotObject>();
    }

    /// <summary> 아이템 기본 정보를 보여준다. </summary>
    public override void LateInit()
    {
        base.LateInit();
        if (parameters.Length <= 0)
            return;

        uint lowDataId = (uint)parameters[0];
        Vector3 newPos = (Vector3)parameters[1];
        newPos.z = 0;
        newPos.y += 0.15f;
        //newPos.x += 0.35f;

        Debug.Log(string.Format("click item lowDataID {0}", lowDataId));

        
        EquipItemTf.position = newPos;
        UseItemTf.position = newPos;

        InvenSlot.SetLowDataItemSlot(lowDataId, 0);

        Item.EquipmentInfo equipLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(lowDataId);
        if (equipLowData != null)//장비 아이템이다.
        {
            EquipItemTf.gameObject.SetActive(true);
            UseItemTf.gameObject.SetActive(false);
            InvenSlot.transform.parent = EquipItemTf.FindChild("ItemRoot");
            
            Item.ItemValueInfo valueLowData = _LowDataMgr.instance.GetLowDataItemValueInfo(equipLowData.BasicOptionIndex);
            UILabel nameLbl = EquipItemTf.FindChild("name").GetComponent<UILabel>();
            UILabel descLbl = EquipItemTf.FindChild("desc").GetComponent<UILabel>();
            UILabel abilityLbl = EquipItemTf.FindChild("ability_value").GetComponent<UILabel>();
            UILabel rndLbl = EquipItemTf.FindChild("rnd_ability").GetComponent<UILabel>();
            
            nameLbl.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(equipLowData.Grade), _LowDataMgr.instance.GetStringItem(equipLowData.NameId));
            descLbl.text = _LowDataMgr.instance.GetStringItem(equipLowData.DescriptionId);
            abilityLbl.text = string.Format("{0} : {1}", uiMgr.GetAbilityLocName((AbilityType)valueLowData.OptionId)
                , uiMgr.GetAbilityStrValue((AbilityType)valueLowData.OptionId, valueLowData.BasicValue*0.1f));
            rndLbl.text = string.Format(_LowDataMgr.instance.GetStringCommon(179)
                , 0, 3);

            //생성되고 잘리는경우 위치값조정필요

            if (EquipItemTf.localPosition.x > 454)
                EquipItemTf.SetLocalX(454f);
            else if (EquipItemTf.localPosition.x < -447)
                EquipItemTf.SetLocalX(-447f);
          
        }
        else//소비 아이템이다.
        {
            EquipItemTf.gameObject.SetActive(false);
            UseItemTf.gameObject.SetActive(true);
            InvenSlot.transform.parent = UseItemTf.FindChild("ItemRoot");

            UILabel nameLbl = UseItemTf.FindChild("name").GetComponent<UILabel>();
            UILabel descLbl = UseItemTf.FindChild("desc").GetComponent<UILabel>();

            Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(lowDataId);
            nameLbl.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(useLowData.Grade), _LowDataMgr.instance.GetStringItem(useLowData.NameId));

            descLbl.text = _LowDataMgr.instance.GetStringItem(useLowData.DescriptionId);

            //생성되고 잘리는경우 위치값조정필요
            if (UseItemTf.localPosition.x > 385)
                UseItemTf.SetLocalX(385);
            else if (UseItemTf.localPosition.x < -507)
                UseItemTf.SetLocalX(-507);

        }

        InvenSlot.transform.localPosition = Vector3.zero;
        InvenSlot.transform.localScale = Vector3.one;
    }

    public override void Hide()
    {
        base.Hide();
    }

}
