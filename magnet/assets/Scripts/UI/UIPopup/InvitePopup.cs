using UnityEngine;
using System.Collections.Generic;

public class InvitePopup : UIBasePanel {

    public GameObject SlotPrefab;
    public UIScrollView ScrollView;
    public UIGrid Grid;

    public GameObject Empty;
    public List<GameObject> SlotList = new List<GameObject>();
    //private ObjectPaging Paging;
    private List<NetData.FriendBaseInfo> FriendList = new List<NetData.FriendBaseInfo>();
    private List<NetData.GuildMemberInfo> GuildMemberList = new List<NetData.GuildMemberInfo>();

    private GAME_MODE CurGameMode;

    private byte CurTabType;//0 길드, 1친구
    private bool IsSendFriend;

    public override void Init()
    {
        base.Init();
        EventDelegate.Set(transform.FindChild("Btn_AllSend").GetComponent<UIButton>().onClick, OnAllInvite);

        //Paging = ObjectPaging.CreatePagingPanel(ScrollView, ScrollView.transform.GetChild(0).gameObject, SlotPrefab, 1, 4, 0, 0, OnCallbackSlot);

        for (int i = 0; i < 5; i++)
        {
            GameObject go = null;
            go = Instantiate(SlotPrefab) as GameObject;
            go.transform.parent = Grid.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            SlotList.Add(go);
        }

        Destroy(SlotPrefab);

        transform.FindChild("TabBtnList").GetComponent<UITabGroup>().Initialize(OnCallbackTabGroup);
        EventDelegate.Set(transform.FindChild("BtnClose").GetComponent<UIEventTrigger>().onClick, Close);
    }

    public override void LateInit()
    {
        base.LateInit();

        CurGameMode = (GAME_MODE)parameters[0];
    }

    public override void Close()
    {
        base.Close();
        UIMgr.OpenReadyPopup(GAME_MODE.NONE, null, 0, 0);
    }

    void OnCallbackTabGroup(int arr)
    {
        if( arr == 0)//길드
        {
            uint guildId = NetData.instance.GetUserInfo()._GuildId;
            if (0 < guildId)
                NetworkClient.instance.SendPMsgGuildMemberListC(guildId);
            else
            {
                for(int i=0; i < SlotList.Count; i++)
                {
                    SlotList[i].SetActive(false);
                    Empty.SetActive(true);
                }
            }
        }
        else if(arr == 1)//친구
        {
            NetworkClient.instance.SendPMsgFriendQueryListC();
        }

        CurTabType = (byte)arr;
    }
    
    void OnCallbackSlot(int arr, GameObject go)
    {
        if(CurTabType == 0)//길드
        {
            if (GuildMemberList.Count <= arr)
            {
                go.SetActive(false);
                return;
            }
            
            go.SetActive(true);
            Transform tf = go.transform;
            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(GuildMemberList[arr].Type);
            tf.FindChild("lv").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(453), GuildMemberList[arr].Lv);
            tf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(155), GuildMemberList[arr].Name );
            tf.FindChild("battle").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), GuildMemberList[arr].Power);
            tf.FindChild("face").GetComponent<UISprite>().spriteName = charLowData.PortraitId;
                        
            if (0 <= NetData.instance.GetGameRoomUserArr(GuildMemberList[arr].Id) )//이미 초대되어 있음
            {
                tf.FindChild("BtnInvite").GetComponent<UISprite>().color = Color.gray;
                tf.FindChild("BtnInvite").collider.enabled = false;
                tf.FindChild("BtnInvite/label").GetComponent<UILabel>().color = Color.gray;
            }
            else
            {
                tf.FindChild("BtnInvite").GetComponent<UISprite>().color = Color.white;
                tf.FindChild("BtnInvite").collider.enabled = true;
                tf.FindChild("BtnInvite/label").GetComponent<UILabel>().color = Color.white;

                List<long> list = new List<long>();
                list.Add((long)GuildMemberList[arr].Id);
                EventDelegate.Set(tf.FindChild("BtnInvite").GetComponent<UIButton>().onClick, delegate () {
                    if (CurGameMode == GAME_MODE.COLOSSEUM)
                        NetworkClient.instance.SendPMsgColosseumInviteC(0, list);
                    else
                        NetworkClient.instance.SendPMsgMultiBossInviteC(0, list);

                    IsSendFriend = false;
                });
            }
            
        }
        else if(CurTabType == 1)//친구
        {
            if(FriendList.Count <= arr)
            {
                go.SetActive(false);
                return;
            }

            go.SetActive(true);
            Transform tf = go.transform;
            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(FriendList[arr].nLookId);
            tf.FindChild("lv").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(453), FriendList[arr].nLevel);
            tf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(155), FriendList[arr].szName);
            tf.FindChild("battle").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), FriendList[arr].BattlePower);
            tf.FindChild("face").GetComponent<UISprite>().spriteName = charLowData.PortraitId;

            if (0 <= NetData.instance.GetGameRoomUserArr(FriendList[arr].ullRoleId))//이미 초대되어 있음
            {
                tf.FindChild("BtnInvite").GetComponent<UISprite>().color = Color.gray;
                tf.FindChild("BtnInvite").collider.enabled = false;
                tf.FindChild("BtnInvite/label").GetComponent<UILabel>().color = Color.gray;
            }
            else
            {
                tf.FindChild("BtnInvite").GetComponent<UISprite>().color = Color.white;
                tf.FindChild("BtnInvite").collider.enabled = true;
                tf.FindChild("BtnInvite/label").GetComponent<UILabel>().color = Color.white;

                List<long> list = new List<long>();
                list.Add((long)FriendList[arr].ullRoleId);
                EventDelegate.Set( tf.FindChild("BtnInvite").GetComponent<UIButton>().onClick, delegate() {
                    if (CurGameMode == GAME_MODE.COLOSSEUM)
                        NetworkClient.instance.SendPMsgColosseumInviteC(0, list);
                    else
                        NetworkClient.instance.SendPMsgMultiBossInviteC(0, list);

                    IsSendFriend = true;
                } );
            }
        }
    }

    /// <summary> 현재 탭의 사람들에게 보냄 </summary>
    void OnAllInvite()
    {
        List<long> list = new List<long>();
        if (CurTabType == 0)//길드
        {
            for (int i = 0; i < GuildMemberList.Count; i++)
            {
                if (0 <= NetData.instance.GetGameRoomUserArr(GuildMemberList[i].Id))//이미 초대되어 있음
                    continue;

                list.Add((long)GuildMemberList[i].Id);
                Debug.Log(string.Format("Invite Guild User Name = {0}, User RoleId = {1}", GuildMemberList[i].Name, GuildMemberList[i].Id));
            }
        }
        else if (CurTabType == 1)//친구
        {
            for(int i=0; i < FriendList.Count; i++)
            {
                if (0 <= NetData.instance.GetGameRoomUserArr(FriendList[i].ullRoleId) )//이미 초대되어 있음
                    continue;

                list.Add((long)FriendList[i].ullRoleId);
                Debug.Log(string.Format("Invite Friend User Name = {0}, User RoleId = {1}", FriendList[i].szName, FriendList[i].ullRoleId));
            }
        }

        if (0 < list.Count)
        {
            if (CurGameMode == GAME_MODE.COLOSSEUM)
                NetworkClient.instance.SendPMsgColosseumInviteC(0, list);
            else
                NetworkClient.instance.SendPMsgMultiBossInviteC(0, list);
        }
    }

    public void OnFriendList(List<NetData.FriendBaseInfo> friendList)
    {
        DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo(NetData.instance.GameRoomData.DungeonId);
        //ulong myRoleId = NetData.instance.GetUserInfo().GetCharUUID();
        for (int i = 0; i < friendList.Count;)//예외처리
        {
            if ( !friendList[i].IsLogin)//접속 종료한 유저 제외시킨다.
            {
                friendList.RemoveAt(i);
            }
            else
            if (friendList[i].nLevel < coloInfo.LimitLevel)//레벨 제한 걸림
            {
                friendList.RemoveAt(i);
            }
            else if (friendList[i].BattlePower < coloInfo.FightingPower)//권장 전투력 미달
            {
                friendList.RemoveAt(i);
            }
            else
                ++i;
        }

        FriendList.Clear();
        FriendList = friendList;

        int count = SlotList.Count < friendList.Count ? friendList.Count : SlotList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject go = null;
            if (SlotList.Count <= i)
            {
                go = Instantiate(SlotList[0]) as GameObject;
                go.transform.parent = Grid.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                SlotList.Add(go);
            }
            else
                go = SlotList[i];

            OnCallbackSlot(i, go);
        }

        ScrollView.enabled = friendList.Count < 4 ? false : true;
        ScrollView.ResetPosition();
        Grid.repositionNow = true;

        Empty.SetActive(friendList.Count == 0);
    }

    public void OnGuildMemberList(List<NetData.GuildMemberInfo> memberList)
    {
        DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo(NetData.instance.GameRoomData.DungeonId );
        ulong myRoleId = NetData.instance.GetUserInfo().GetCharUUID();
        for(int i=0; i < memberList.Count;)//예외처리
        {
            if (memberList[i].Id == myRoleId)//나 자신은 목록에서 제외.
            {
                memberList.RemoveAt(i);
            }
            else if (memberList[i].LoginTime < memberList[i].LogountTime)//접속 종료한 유저 제외시킨다.
            {
                memberList.RemoveAt(i);
            }
            else if(memberList[i].Lv < coloInfo.LimitLevel)//레벨 제한 걸림
            {
                memberList.RemoveAt(i);
            }
            //else if(memberList[i].Power < coloInfo.FightingPower)//권장 전투력 미달
            //{
            //    memberList.RemoveAt(i);
            //}
            else
                ++i;
        }

        GuildMemberList.Clear();
        GuildMemberList = memberList;

        int count = SlotList.Count < memberList.Count ? memberList.Count : SlotList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject go = null;
            if (SlotList.Count <= i)
            {
                go = Instantiate(SlotList[0]) as GameObject;
                go.transform.parent = Grid.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                SlotList.Add(go);
            }
            else
                go = SlotList[i];

            OnCallbackSlot(i, go);
        }

        ScrollView.enabled = memberList.Count < 4 ? false : true;
        ScrollView.ResetPosition();
        Grid.repositionNow = true;

        Empty.SetActive(memberList.Count == 0);


        //Paging.RefreshSlot(GuildMemberList.Count );
    }

    //int GuildMemberSort(NetData.GuildMemberInfo a, NetData.GuildMemberInfo b )
    //{
    //    if (a.LoginTime < a.LogountTime && b.LogountTime <= b.LoginTime)
    //    {
    //        return 1;
    //    }
    //    else if (a.LogountTime <= a.LoginTime && b.LoginTime < b.LogountTime)
    //        return -1;

    //    return 0;
    //}
}
