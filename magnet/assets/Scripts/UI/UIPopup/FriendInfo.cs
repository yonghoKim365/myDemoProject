using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  친구의 장착중인 아이템정보창 
/// </summary>
public class FriendInfo : UIBasePanel
{
    public UIButton BtnClose;

    private NetData._UserInfo CharInven;    //한 캐릭터가 들고있는 인벤토리 정보


    public override void Init()
    {
        base.Init();
            
        EventDelegate.Set(BtnClose.onClick, OnClickClose);
    }

   
    void OnClickClose()
    {
        gameObject.SetActive(false);
    }

}
