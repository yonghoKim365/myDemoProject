using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangePopup : UIBasePanel
{
    public UILabel Name;
    public UILabel Attack;

    public UISprite Bg;
    public UISprite Grade;
    public UISprite Img;
    public UIButton BtnClose;
    public UIButton BtnChange;

    //private List<NetData._ItemData> CompareData;  //자동장착이 되야할지 비교해야할 아이템데이터 
    private List<NetData._ItemData> RecomendList = new List<NetData._ItemData>();

    //private NetData._ItemData CurData;  //현재 창에 나와있는 데이터
    // 파츠별로...저장해줌
    //private Dictionary<ePartType, NetData._ItemData> EquipPartsDic = new Dictionary<ePartType, NetData._ItemData>();


    public override void Init()
    {
        base.Init();

        EventDelegate.Set(BtnChange.onClick, OnClickChanged);
        EventDelegate.Set(BtnClose.onClick, Close);
    }

    public override void LateInit()
    {
        base.LateInit();
        if (parameters == null || parameters.Length == 0 || parameters[0] == null)
        {
            Close();
            return;
        }
        
        RecomendList = (List<NetData._ItemData>)parameters[0];
        if(RecomendList.Count <= 0 )
            Close();
        else
            ShowItem(RecomendList[0] );
    }

    void ShowItem(NetData._ItemData recomend)
    {
        Name.text = recomend.GetLocName();
        Item.EquipmentInfo equipLowData = recomend.GetEquipLowData();
        Img.spriteName = _LowDataMgr.instance.GetLowDataIcon(equipLowData.Icon);
        Bg.spriteName = string.Format("Icon_bg_0{0}", equipLowData.Grade);
        Grade.spriteName = string.Format("Icon_0{0}", equipLowData.Grade);
        Attack.text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), recomend._Attack.ToString());

        //UIEventTrigger etri = transform.FindChild("Bg").GetComponent<UIEventTrigger>();
        //EventDelegate.Set(etri.onClick, delegate () { OnclicItemPopup(Img.transform, recomend._equipitemDataIndex); });

		if (SceneManager.instance.testData.bSingleSceneTestStart || SceneManager.instance.testData.bQuestTestStart) {
			TempCoroutine.instance.FrameDelay(0.5f, ()=>{
				OnClickChanged();
			});
		}
    }
    
    /// <summary> 해당 파츠 장착. </summary>
    void OnClickChanged()
    {
        if (RecomendList.Count <= 0 )
        {
            Close();
            //Hide();
            return;
        }

        NetworkClient.instance.SendPMsgEquipmentUserC((int)RecomendList[0]._itemIndex, 1);
        RecomendList.RemoveAt(0);
        if (0 < RecomendList.Count)
            ShowItem(RecomendList[0]);
        else
            Close();
    }
    
    public override void Close()
    {
        if (1 < RecomendList.Count)
        {
            RecomendList.RemoveAt(0);//닫기를 눌렀다는 것은 해당 번째의 아이템을 무시 했다는거.
            ShowItem(RecomendList[0]);
        }
        else
        {
            RecomendList.Clear();
            base.Close();
        }
    }


}
