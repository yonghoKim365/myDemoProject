using UnityEngine;
using System.Collections.Generic;

public class CategoryPanel : UIBasePanel {

    public Transform Scroll;

    public UITabGroup TabGroup;

    private UIBasePanel ReOpenPanel;

    public override void Init()
    {
        base.Init();

        TabGroup.Initialize(OnClickTab);

        int tutoSupportArr = 0;
        for(int i=0; i < TabGroup.TabList.Count; i++)
        {
            if (TabGroup.TabList[i].GetComponent<TutorialSupport>() == null)
                continue;

            tutoSupportArr = i;
            break;
        }

        List<Item.CategoryListInfo> list = _LowDataMgr.instance.GetLowDataCategoryList();
        UIHelper.CreateSlotItem(true, list.Count, Scroll.GetChild(0), Scroll, (tf, arr) => {//타이틀, 아이템리스트 생성
            tf.FindChild("Title/txt").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(list[arr].CategoryName);
            tf.FindChild("Title/desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(list[arr].CategoryDesc);
            if (arr < TabGroup.TabList.Count)
                TabGroup.TabList[arr].GetComponent<UITabbase>().ChangeLabel(_LowDataMgr.instance.GetStringCommon(list[arr].CategoryName));

            tf.name = string.Format("{0}", arr);
            Transform itemListTf = tf.FindChild("ItemList");
            List<string> itemIdList = list[arr].ItemList.list;
            UIHelper.CreateSlotItem(true, itemIdList.Count, itemListTf.GetChild(0), itemListTf, (itemTf, itemArr) => {//아이템들 생성
                uint itemid = uint.Parse(itemIdList[itemArr]);
                SetItemSlot(itemid, itemTf);
                if(arr == tutoSupportArr && itemArr == 0)
                {
                    TutorialSupport support = itemTf.gameObject.AddComponent<TutorialSupport>();
                    support.TutoType = TutorialType.CATEGORY;
                    support.SortId = 3;
                    support.IsScroll = true;
                }
            });

            itemListTf.GetComponent<UIGrid>().repositionNow = true;

            tf.FindChild("Title").GetComponent<UIWidget>().height = 190 + (110 * Mathf.RoundToInt((list[arr].ItemList.Count * 0.1f) - 0.5f));

            if (0 < arr)
            {
                //int line = 0;
                //if (1 <= list[arr - 1].ItemList.Count * 0.1f)
                //    line = list[arr - 1].ItemList.Count *0.1f;
                tf.localPosition = -new Vector3(0, -Scroll.GetChild(arr - 1).localPosition.y + Scroll.GetChild(arr-1).FindChild("Title").GetComponent<UIWidget>().height, 0);
                //tf.localPosition = new Vector3(0, -((190 * arr) + (110 * line) ));
            }
            else
                tf.localPosition = Vector3.zero;
        });
    }

    public override void LateInit()
    {
        base.LateInit();

        if(0 < parameters.Length)
            ReOpenPanel = (UIBasePanel)parameters[0];

        if( mStarted)
        {
            List<Item.CategoryListInfo> list = _LowDataMgr.instance.GetLowDataCategoryList();
            for(int i=0; i < Scroll.childCount; i++)
            {
                Transform itemListTf = Scroll.GetChild(i).FindChild("ItemList");
                List<string> itemIdList = list[i].ItemList.list;
                for(int j=0; j < itemIdList.Count; j++)
                {
                    uint itemid = uint.Parse(itemIdList[j]);
                    SetItemSlot(itemid, itemListTf.GetChild(j) );
                }
                
            }

            TabGroup.CoercionTab(0);
        }

        OnSubTutorial();
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    void SetItemSlot(uint itemid, Transform itemTf)
    {
        Item.ItemInfo itemInfo = _LowDataMgr.instance.GetUseItem(itemid);

        UISprite iconSp = itemTf.FindChild("icon").GetComponent<UISprite>();
        if (itemInfo.Type == (byte)AssetType.CostumeShard || itemInfo.Type == (byte)AssetType.PartnerShard)
            iconSp.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);

        iconSp.spriteName = _LowDataMgr.instance.GetLowDataIcon(itemInfo.Icon);
        itemTf.FindChild("grade").GetComponent<UISprite>().spriteName = string.Format("Icon_0{0}", itemInfo.Grade);
        NetData._ItemData item = NetData.instance.GetUserInfo().GetItemForItemID(itemid, (byte)eItemType.USE);
        if (item != null)
        {
            itemTf.FindChild("new").gameObject.SetActive(item.IsNewItem);
            itemTf.FindChild("disable").gameObject.SetActive(false);
            itemTf.FindChild("stack").GetComponent<UILabel>().text = item.Count.ToString();

            item.IsNewItem = false;//바로 꺼준다.Hide에서 처리해도 되지만 여기서 처리해보도록한다
        }
        else
        {
            itemTf.FindChild("new").gameObject.SetActive(false);
            itemTf.FindChild("disable").gameObject.SetActive(true);
            itemTf.FindChild("stack").GetComponent<UILabel>().text = "0";
        }

        EventDelegate.Set(itemTf.GetComponent<UIEventTrigger>().onClick, () => {
            if(SceneManager.instance.CurTutorial == TutorialType.CATEGORY)
            {
                if(itemTf.GetComponent<TutorialSupport>() == null)//스킵
                {
                    UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
                    if (basePanel != null)
                    {
                        SceneManager.instance.CurTutorial = SceneManager.instance.CurTutorial;//다음으로 넘김.
                        basePanel.Close();
                    }
                }
            }

            UIMgr.OpenDetailPopup(this, itemInfo.Id, 7);
        });
    }

    void OnClickTab(int arr)
    {
        Vector3 movePos = Scroll.GetChild(arr).localPosition;
        movePos.y = -(movePos.y + 58);
        SpringPanel spring = Scroll.GetComponent<SpringPanel>();
        if (spring != null)
        {
            spring.target = movePos;
            spring.enabled = true;
        }
        else
            SpringPanel.Begin(Scroll.gameObject, movePos, 8);
    }

    public override void Hide()
    {
        base.Hide();

		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        //List<NetData._ItemData> itemList = NetData.instance.GetUserInfo().GetTypeItemList(eItemType.USE);
        //for (int i = 0; i < itemList.Count; i++)
        //{
        //    if (!itemList[i].IsNewItem)
        //        continue;

        //    itemList[i].IsNewItem = false;//나갈때 다 꺼져
        //}

        SceneManager.instance.SetAlram(AlramIconType.CATEGORY, false);

        if (ReOpenPanel != null)
            ReOpenPanel.Show(ReOpenPanel.GetParams());
        else
        {
            Debug.LogWarning("ReOpenPanel is Null");
            UIMgr.OpenTown();
        }
    }
}
