using UnityEngine;
using System.Collections.Generic;

public class UserInfoPopup : UIBasePanel {

    public long UserRoleId;
    public string UserName;
    public int VipLv;
    public int UserLv;
    public int CharIdx;

    public GameObject CharInfoGo;
    public GameObject InfoGo;
    public GameObject[] FriendOnOff;//On, Off
    //public GameObject[] GuildOnOff;//On, Off

    public Transform EquipPartsTf;

    public UILabel CurCostumStatus;
    public UILabel UserNameLbl;

    private bool IsCharInfoView;

    public override void Init()
    {
        base.Init();

        EventDelegate.Set(transform.FindChild("Info/BtnClose").GetComponent<UIButton>().onClick, Close);
        EventDelegate.Set(transform.FindChild("Info").GetComponent<UIEventTrigger>().onClick, Close);
        EventDelegate.Set(transform.FindChild("Info/BtnInfo").GetComponent<UIEventTrigger>().onClick, ()=> {//유저 정보
            NetworkClient.instance.SendPMsgQueryRoleInfoC(UserRoleId);
        });
        
        EventDelegate.Set(transform.FindChild("Info/BtnGuild").GetComponent<UIEventTrigger>().onClick, ()=> {//길드 초대

        });
        EventDelegate.Set(transform.FindChild("Info/BtnWhisper").GetComponent<UIEventTrigger>().onClick, ()=> {//귓속말
            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
            if (chat != null)
            {
                NetData.ChatData data; ;
                data.Msg = null;
                data.GuildName = null;
                data.UserName = UserName;
                data.UserUID = UserRoleId;
                data.GuildId = 0;
                data.ClassId = CharIdx;
                data.VipLv = VipLv;
                data.Lv = UserLv;
                data.WhisperUID = UserRoleId;

                (chat as ChatPopup).StartWhisper(data);
            }

            Close();
        });

        //if(NetData.instance.GetUserInfo()._GuildId <= 0 )
        //{
        //    transform.FindChild("Info/BtnGuild").collider.enabled = false;
        //    transform.FindChild("Info/BtnGuild").GetComponent<UISprite>().color = Color.gray;
        //    transform.FindChild("Info/BtnGuild/txt").GetComponent<UILabel>().color = Color.gray;
        //}

        EventDelegate.Set(CharInfoGo.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, delegate () {
            if (IsCharInfoView)
                Close();
            else
            {
                CharInfoGo.SetActive(false);
                InfoGo.SetActive(true);
            }
        });

        //GuildOnOff[0].SetActive(false);
        //GuildOnOff[1].SetActive(true);
        FriendOnOff[0].SetActive(false);
        FriendOnOff[1].SetActive(true);
        
        FriendOnOff[0].transform.FindChild("txt").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(210);

        CharInfoGo.SetActive(false);
        InfoGo.SetActive(false);
    }

    public override void NetworkData(params object[] proto)
    {
        base.NetworkData(proto);
        switch((Sw.MSG_DEFINE) proto[0])
        {
            case Sw.MSG_DEFINE._MSG_FRIEND_QUERY_LIST_C:
                bool isFriend = false;
                List<NetData.FriendBaseInfo> friendList = (List<NetData.FriendBaseInfo>)proto[1];
                for(int i=0; i < friendList.Count; i++)
                {
                    if (!friendList[i].ullRoleId.Equals((ulong)UserRoleId))
                        continue;

                    isFriend = true;
                    break;
                }

                FriendOnOff[0].SetActive(true);
                FriendOnOff[1].SetActive(false);

                if (!isFriend)//친구일 경우 1:1대화 기능 활성화
                {
                    EventDelegate.Set(transform.FindChild("Info/BtnFriend").GetComponent<UIEventTrigger>().onClick, () => {//친구 등록
                        NetworkClient.instance.SendPMsgFriendAddC((ulong)UserRoleId);
                        FriendOnOff[0].SetActive(false);
                        FriendOnOff[1].SetActive(true);
                    });

                    break;
                }

                //이름 변경 '1:1 대화'
                FriendOnOff[0].transform.FindChild("txt").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1140);

                EventDelegate.Set(transform.FindChild("Info/BtnFriend").GetComponent<UIEventTrigger>().onClick, () => {//친구 1:1 대화
                    //Close();
                    Hide();//일단 하이드 시켜놓는다.
                    UIMgr.GetTownBasePanel().Hide();
                    UIMgr.OpenSocialPanel(5);
                });
                break;
            case Sw.MSG_DEFINE._MSG_FRIEND_SELF_REQUEST_FRIEND_LIST_C://내가 신청한 친구 리스트
                bool isRequestUser = false;
                List<NetData.FriendRequestBaseInfo> sendList = (List<NetData.FriendRequestBaseInfo>)proto[1];
                for(int i=0; i < sendList.Count; i++)
                {
                    if (!sendList[i].ullRoleId.Equals( (ulong)UserRoleId) )
                        continue;

                    isRequestUser = true;
                    break;
                }
                
                if( !isRequestUser)//신청리스트에도 없다면 내 친구리스트에 존재하는지 확인.
                    NetworkClient.instance.SendPMsgFriendQueryListC();
                else
                {
                    FriendOnOff[0].SetActive(false);
                    FriendOnOff[1].SetActive(true);
                }

                break;
        }
    }

    public override void LateInit()
    {
        base.LateInit();
        if (parameters.Length == 0)
            return;

        UserRoleId = (long)parameters[0];
        UserName = (string)parameters[1];
        CharIdx = (int)parameters[2];
        VipLv = (int)parameters[3];
        UserLv = (int)parameters[4];
        IsCharInfoView = (bool)parameters[5];
        if (IsCharInfoView)
        {
            NetworkClient.instance.SendPMsgQueryRoleInfoC(UserRoleId);
        }
        else
        {
            InfoGo.SetActive(true);

            transform.FindChild("Info/Lv").GetComponent<UILabel>().text = string.Format("{0}", UserLv);//_LowDataMgr.instance.GetStringCommon(453), <-레벨
            transform.FindChild("Info/Txt_Name").GetComponent<UILabel>().text = string.Format("{0}", string.Format(UserName));
            transform.FindChild("Info/Class/port").GetComponent<UISprite>().spriteName = UIHelper.GetClassPortIcon((uint)CharIdx, 2);

            if (0 < VipLv)
            {
                transform.FindChild("Info/VipInfo").gameObject.SetActive(true);
                transform.FindChild("Info/VipInfo/vip").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), VipLv);
            }
            else
                transform.FindChild("Info/VipInfo").gameObject.SetActive(false);

            NetworkClient.instance.SendPMsgFriendSelfRequestFriendListC();//내가 신청한 친구 리스트에 존재하는 확인절차.
        }

        if(TownState.TownActive)//혹시 모르니
            SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
    }

    public override void Close()
    {
        base.Close();

    }

    public void UserCharInfo(uint charIdx, uint skillSetId, NetData._ItemData[] equipList, NetData._CostumeData costume, bool hideCostume)
    {   
        CurCostumStatus.text = costume.GetLocName();

        string lvAndName = string.Format(_LowDataMgr.instance.GetStringCommon(453), UserLv);
        UserNameLbl.text = string.Format("{0} {1}", lvAndName, UserName);

        uint helmet = 0, cloth = 0, weapon = 0;
        for(int i=0; i < EquipPartsTf.childCount; i++)
        {
            Transform tf = EquipPartsTf.GetChild(i);
            if (tf == null)
                continue;

            UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
            UISprite bg = tf.GetComponent<UISprite>();
            UISprite grade = tf.FindChild("grade").GetComponent<UISprite>();
            UISprite icon = tf.FindChild("icon").GetComponent<UISprite>();
            UILabel enchantLv = tf.FindChild("Num").GetComponent<UILabel>();

            if (equipList[i] == null)
            {
                icon.enabled = false;
                grade.enabled = false;
                bg.spriteName = UIHelper.GetDefaultEquipIcon((ePartType)i+1);
                enchantLv.gameObject.SetActive(false);

                continue;
            }

            NetData._ItemData equipData = equipList[i];

            if (equipData.EquipPartType == ePartType.HELMET)
                helmet = equipData._equipitemDataIndex;
            else if (equipData.EquipPartType == ePartType.WEAPON)
                weapon = equipData._equipitemDataIndex;
            else if (equipData.EquipPartType == ePartType.CLOTH)
                cloth = equipData._equipitemDataIndex;
            
            icon.enabled = true;
            grade.enabled = true;

            UIAtlas atlas = null;
            atlas = AtlasMgr.instance.GetEquipAtlasForClassId(equipData.GetEquipLowData().Class);

            icon.atlas = atlas;
            icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(equipData.GetEquipLowData().Icon);
            grade.spriteName = string.Format("Icon_0{0}", equipData.GetEquipLowData().Grade);
            bg.spriteName = string.Format("Icon_bg_0{0}", equipData.GetEquipLowData().Grade);
            enchantLv.text = equipData._enchant <= 0 ? "" : string.Format("+{0}", equipData._enchant);
            enchantLv.gameObject.SetActive(true);

            EventDelegate.Set(uiTri.onClick, delegate () {
                if (equipData == null)
                    return;
                UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
                if (basePanel == null)
                    return;

                UIMgr.OpenDetailPopup(this, equipData, GetComponent<UIPanel>().depth + 3 );
            });
        }

        //코스튬 화면
        UIHelper.CreatePcUIModel("CharacterPanel", RotationTargetList[0].transform, charIdx, helmet, costume._costmeDataIndex
            , cloth, weapon, skillSetId, 3, hideCostume, false);
        
        CharInfoGo.SetActive(true);
        InfoGo.SetActive(false);
    }

}
