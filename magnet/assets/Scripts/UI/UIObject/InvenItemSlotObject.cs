using UnityEngine;
using System;

public class InvenItemSlotObject : MonoBehaviour {

    public UIEventTrigger BtnTrigger;
    
    public UISprite ItemIcon;//아이템 아이콘
    public UISprite GradeIcon;//아이템 등급 아이콘
    public UISprite BackGround;
    public UISprite DefaultBg;  //
    public UILabel Stack;//아이템의 개수
    public UILabel EnchantLv;//강화횟수

    public GameObject EquipMark;    //장착중
    public GameObject Unable;   //장착불가
    public GameObject Alram;    //신규알림
    public GameObject UseInvenRoot; //장착불가와신규알림은 인벤에서만쓰일수있도록...
    private Action<ulong> CallBack;
    private Action<ulong, byte> CallBackTwo;
    public ulong Key;
    public byte Key2;

    private AtlasMgr _AtlasMgr;

    void Awake()
    {
        _AtlasMgr = AtlasMgr.instance;

    }

    void Start()
    {
        EventDelegate.Set(BtnTrigger.onClick, delegate() {
            if (CallBack != null)
                CallBack(Key);
            else if (CallBackTwo != null)
                CallBackTwo(Key, Key2);
        });

        if(transform.parent != null)
        {
            GameObject parentGo = transform.parent.gameObject;
            gameObject.layer = parentGo.layer;
            transform.SetChildLayer(parentGo.layer);
        }
    }

    /// <summary> 개수만 재갱신함. </summary>
    public void RefreshAmount(ushort amount)
    {
        if (amount <= 1)
            Stack.gameObject.SetActive(false);
        else
        {
            Stack.gameObject.SetActive(true);
			Stack.text = SceneManager.instance.NumberToString(amount);//amount.ToString(); // ToString("#,##");
        }
    }

    /// <summary> 인벤토리의 아이템 정의 </summary>
    /// <param name="itemData">셋팅하고자 하는 아이템데이터</param>
    /// <param name="startDepth">시작 뎁스 값</param>
    public void SetInvenItemSlot(NetData._ItemData itemData, Action<ulong, byte> callBack, int startDepth=8)
    {
        EnchantLv.gameObject.SetActive(itemData.IsEquipItem());
        if (itemData.IsEquipItem())
        {
            SetEquipItem(itemData.GetEquipLowData(), itemData.Count);
            EnchantLv.text = itemData._enchant <= 0 ? "" : string.Format("+{0}", itemData._enchant);

        }
        else
            SetUseItem(itemData.GetUseLowData(), itemData.Count);

        Key = itemData._itemIndex;
        Key2 = itemData._itemType;
        CallBackTwo = callBack;

        SetDepth(startDepth);
    }

    /// <summary> 장착중인 아이템 슬롯 셋팅 </summary>
    public void SetMountItemSlot(NetData._ItemData itemData, Action<ulong> callBack, uint key)
    {
        EnchantLv.gameObject.SetActive(itemData.IsEquipItem());
        if (itemData == null)//장착중인 아이템이 없다 다 꺼놓는다.
        {
            EmptySlot();
        }
        else//장착중인 아이템이 있다 아이템 셋팅한다 
        {
            SetEquipItem(itemData.GetEquipLowData(), 0);
            EnchantLv.text = string.Format("+{0}", itemData._enchant);
            Key = key;
            CallBack = callBack;
        }
    }

    /// <summary> 아이템의 테이블 아이디로 셋팅을 함.</summary>
    public void SetLowDataItemSlot(uint lowDataIdx, uint amount, Action<ulong> callBack = null)
    {
        EnchantLv.gameObject.SetActive(false);
        if (lowDataIdx < 100)
        {
            //string iconName = null;
            switch ((Sw.UNITE_TYPE)lowDataIdx)
            {
                case Sw.UNITE_TYPE.UNITE_TYPE_COIN:
                    lowDataIdx = 599000;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_GEM:
                    lowDataIdx = 599001;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_CONTRIBUTION:
                    lowDataIdx = 599004;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_HONOR:
                    lowDataIdx = 599005;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_ROYAL_BADGE:
                    lowDataIdx = 599002;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_LION_KING_BADGE:
                    lowDataIdx = 599006;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_FAME:
                    lowDataIdx = 599003;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_POWER:    
                    lowDataIdx = 599104;
                    break;
                case Sw.UNITE_TYPE.UNITE_TYPE_ROLE_EXP:
                    lowDataIdx = 599105;
                    break;
                //case Sw.UNITE_TYPE.UNITE_TYPE_TITLE:
                    
                //    break;

                default:
                    Debug.Log("unDefined type error " + (Sw.UNITE_TYPE)lowDataIdx);
                    EmptySlot();
                    return;
            }

            //if( string.IsNullOrEmpty(iconName ) )
            //{
            //    Debug.LogError(string.Format("not setting image name error {0}", (Sw.UNITE_TYPE)lowDataIdx ) );
            //}

            //ItemIcon.atlas = _AtlasMgr.GetLoadAtlas(LoadAtlasType.UseItem);
            //SetDefault(iconName, "", amount, "Bod_IconBg");
        }

        Item.EquipmentInfo equip = _LowDataMgr.instance.GetLowDataEquipItemInfo(lowDataIdx);
        if (equip != null)
            SetEquipItem(equip, amount);
        else
        {
            Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(lowDataIdx);
            if (useLowData == null)//파트너?
                SetPartnerItem(lowDataIdx, amount);
            else if (useLowData != null)
                SetUseItem(useLowData, amount);
            else
            {
                EmptySlot();
            }
        }

        if (callBack != null && 100 < lowDataIdx)
        {
            Key = lowDataIdx;
            CallBack = callBack;
        }
    }

    /// <summary> Background를 제외한 것들 꺼놓기 </summary>
    public void EmptySlot()
    {
        ItemIcon.gameObject.SetActive(false);
        GradeIcon.gameObject.SetActive(false);
        Stack.gameObject.SetActive(false);
        BackGround.atlas = _AtlasMgr.GetLoadAtlas(PoolAtlasType.Bod);
        BackGround.spriteName = "Bod_IconBg";
        DefaultBg.gameObject.SetActive(true);
        EnchantLv.gameObject.SetActive(false);
        EquipMark.SetActive(false);

        Key = 0;
        Key2 = 0;
        CallBack = null;
        CallBackTwo = null;
    }
    
    /// <summary> 장비 아이템 </summary>
    void SetEquipItem(Item.EquipmentInfo lowData, uint amount)
    {
        if (lowData == null)
            return;

        ItemIcon.atlas = _AtlasMgr.GetEquipAtlasForClassId(lowData.Class);
        BackGround.atlas = _AtlasMgr.GetLoadAtlas(LoadAtlasType.Item);

        //if (ItemIcon.atlas != EquipAtlas)
        //    ItemIcon.atlas = EquipAtlas;

        //if (BackGround.atlas != EquipAtlas)
        //    BackGround.atlas = EquipAtlas;

        DefaultBg.gameObject.SetActive(false);

        string icon = _LowDataMgr.instance.GetLowDataIcon(lowData.Icon), gradeIcon = GetGradeIcon(lowData.Grade);
        string bg = string.Format("Icon_bg_0{0}", lowData.Grade);
        SetDefault(icon, gradeIcon, amount, bg);
    }

    /// <summary> 소모 아이템 </summary>
    void SetUseItem(Item.ItemInfo lowData, uint amount)
    {
        if (lowData == null)
            return;

        ItemIcon.atlas = _AtlasMgr.GetLoadAtlas(LoadAtlasType.UseItem);

        // 소모아이템중 보석만 배경이들어가고 나머지는배경이없이..
        string bg;
        if(lowData.Type == (int)AssetType.Jewel)
        {
            BackGround.atlas = _AtlasMgr.GetLoadAtlas(LoadAtlasType.Item);
            bg = string.Format("Icon_bg_0{0}", lowData.Grade);
            DefaultBg.gameObject.SetActive(false);

        }
        if (lowData.Type == (int)AssetType.PartnerShard || lowData.Type == (int)AssetType.CostumeShard)
        {
            ItemIcon.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
            BackGround.atlas = _AtlasMgr.GetLoadAtlas(PoolAtlasType.Bod);
            bg = "Bod_IconBg";
            DefaultBg.gameObject.SetActive(true);
        }
        else
        {
            BackGround.atlas = _AtlasMgr.GetLoadAtlas(PoolAtlasType.Bod);
            bg = "Bod_IconBg";
            DefaultBg.gameObject.SetActive(true);
        }

        string icon = _LowDataMgr.instance.GetLowDataIcon(lowData.Icon), gradeIcon = GetGradeIcon(lowData.Grade);
        SetDefault(icon, gradeIcon, amount, bg);
    }

    /// <summary> 파트너 아이템 </summary>
    void SetPartnerItem(uint lowData, uint amount)
    {
        if (lowData == 0)
            return;

        //if (ItemIcon.atlas != Face)
        ItemIcon.atlas = _AtlasMgr.GetLoadAtlas(LoadAtlasType.Face);

        //파트너는배경없이..
        //if (BackGround.atlas != Bod)
        BackGround.atlas = _AtlasMgr.GetLoadAtlas(PoolAtlasType.Bod);
        DefaultBg.gameObject.SetActive(true);


        Partner.PartnerDataInfo parInfo = _LowDataMgr.instance.GetPartnerInfo(lowData);
        if (parInfo == null)
            return;

        string icon = parInfo.PortraitId, gradeIcon = "Icon_01";
        SetDefault(icon, gradeIcon, amount, "Bod_IconBg");
    }

    /// <summary> 기본 공통사항 정의 </summary>
    void SetDefault(string icon, string gradeIcon, uint amount, string bg)
    {
        ItemIcon.gameObject.SetActive(true);
        GradeIcon.gameObject.SetActive(true);

        ItemIcon.spriteName = icon;
        GradeIcon.spriteName = gradeIcon;
        BackGround.spriteName = bg;
        
        if (amount <= 1)
            Stack.gameObject.SetActive(false);
        else
        {
            Stack.gameObject.SetActive(true);
			Stack.text = SceneManager.instance.NumberToString(amount);// amount.ToString(); // ToString("#,##");
        }
    }

    /// <summary> 아이템 테두리 아이콘 빼오기. 일단은 이렇게? </summary>
    string GetGradeIcon(int grade)
    {
        string iconName = string.Format("Icon_0{0}", grade);
        
        return iconName;
    }

    /// <summary> 받아온 뎁스 셋팅 </summary>
    public void SetDepth(int startDepth)
    {
        BackGround.depth = startDepth;
        DefaultBg.depth = startDepth + 1;
        ItemIcon.depth = startDepth+2;
        EnchantLv.depth = startDepth + 3;
        GradeIcon.depth = startDepth+2;
        EquipMark.transform.GetComponent<UISprite>().depth = startDepth + 3;
        Stack.depth = startDepth+4;

        GetComponent<UIWidget>().depth += startDepth+5;
    }
    
    //void OnPress(bool isPress)
    void OnMouseDown()
    {
        //if (!isPress || CallBack == null || Key == 0)
            //return;

        CallBack(Key);
    }

    /// <summary> 상황에 따라 아이템 배경이미지 변경해준다. </summary>
    public void SetBackGround(string name)
    {
        BackGround.spriteName = name;
    }
    
}
