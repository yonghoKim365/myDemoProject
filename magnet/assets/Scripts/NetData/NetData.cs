using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class NetData : Immortal<NetData>
{
    public static double GameTimeSec { get { return TimeMgr.instance.RealTime; } }
    /*
    public ulong SnsID { get; set; }

    /// <summary> 계정 고유ID </summary>
    public ulong AccountUUID { get { return userInfo.user_idx; } }
    
    /// <summary> 현재 선택된 메인 캐릭터 </summary>
    public ulong MainCharUUID { get { return 0; } }
    
    public AccountInfo userInfo = new AccountInfo();
    */
    /// <summary> 계정 고유ID </summary>
    public ulong AccountUUID {
        get {
            if(_userInfo == null)
            {
                Debug.LogError("is UserInfo NULL Error!");
                return 0;
            }
            return _userInfo.GetAccountUUID();
        }
    }

    /// <summary> 유엔코드 </summary>
    public int UnCode
    {
        set;
        get;
    }

    /// <summary> 고유 계정 아이디 </summary>
    public long UUID
    {
        get;
        set;
    }


    private Dictionary<ulong, CharacterInfo> characterDic = new Dictionary<ulong, CharacterInfo>();
    private Dictionary<ulong, CostumeInfo> costumeDic = new Dictionary<ulong, CostumeInfo>();
    public RewardData _RewardData = new RewardData();//획득 정보.
    public RoomData GameRoomData;//콜로세움, 멀티보스레이드(난투장이랑은 별도임)

    //ip
    public string _Ip;//ping 검사할 ip

    //현재로그인된 계정의 Type
    public eLoginType _LoginType;
    //현재로그인된 계정의 userid
    public string _userid;
    #region 계정관련 데이터들

    public eLoginType GetLoginType()
    {
        return _LoginType;
    }    

    public string GetUserID()
    {
        return _userid;
    }

    #endregion

    #region :: Character 관련 ::

    public int CharacterCount { get { return characterDic.Count; } }

    //public void AddCharacter(CharacterInfo info, bool overwrite = true)
    public void AddCharacter(CharacterInfo info, bool overwrite = true)
    {
        if (characterDic.ContainsKey(info.c_usn))
        {
            if (overwrite)
                characterDic[info.c_usn].Set(info);
            else
                characterDic[info.c_usn] = info;
        }
        else
            characterDic.Add(info.c_usn, info);
    }

    public Dictionary<ulong, CharacterInfo> GetCharacters()
    {
        return characterDic;
    }

    public CharacterInfo GetCharacter(ulong charIdx)
    {
        if (characterDic.ContainsKey(charIdx))
            return characterDic[charIdx];

        Debug.LogWarning("Not found CharacterInfo : " + charIdx);
        return null;
    }

    /// <summary> 캐릭터 삭제 함수. </summary>
    public bool DeleteCharacter(ulong charIdx)
    {
        if (!characterDic.ContainsKey(charIdx))
            return false;

        characterDic.Remove(charIdx);
        return true;
    }

    /// <summary> 캐릭터, 코스튬 리스트 초기화. </summary>
    public void ClearCharIdc()
    {
        if (characterDic != null)
            characterDic.Clear();

        if (costumeDic != null)
            costumeDic.Clear();
    }

    #endregion

    #region :: Costume 관련 ::

    public void AddCostume(CostumeInfo info, bool overwrite = true)
    {
        if (costumeDic.ContainsKey(info.c_usn))
        {
            if (overwrite)
                costumeDic[info.c_usn].Set(info);
            else
                costumeDic[info.c_usn] = info;
        }
        else
            costumeDic.Add(info.c_usn, info);
    }

    public Dictionary<ulong, CostumeInfo> GetCostumes()
    {
        return costumeDic;
    }

    public CostumeInfo GetCostume(ulong costumeIdx)
    {
        if (costumeDic.ContainsKey(costumeIdx))
            return costumeDic[costumeIdx];

        Debug.LogWarning("Not found CostumeInfo : " + costumeIdx);
        return null;
    }
    
    // 일단은 이렇게
    public CostumeInfo GetCostumeByChar(ulong charIdx)
    {
        Dictionary<ulong, CostumeInfo>.ValueCollection values = costumeDic.Values;

        for (int i = 0; i < values.Count; i++)
        {
            CostumeInfo info = values.ElementAt(i);
            if (info.c_usn == charIdx)
                return info;
        }

        return null;
    }

    #endregion

    #region 게임종료 

    #endregion

    #region 콜로세움, 멀티보스레이드 방 정보

    public bool GameRoomRemoveUser(ulong userRoleId)
    {
        for (int i = 0; i < GameRoomData.UserList.Count; i++)
        {
            if (GameRoomData.UserList[i].Id != userRoleId)
                continue;

            GameRoomData.UserList.RemoveAt(i);
            return true;
        }

        return false;
    }

    public void AddGameRoomUser(RoomUserInfo userInfo)
    {
        if (GameRoomData.UserList == null)
            GameRoomData.UserList = new List<RoomUserInfo>();
        
        if (GameRoomData.UserList.Contains(userInfo))
        {
            Debug.LogError("Already Added userInfo " + userInfo.Name);
            return;
        }

        GameRoomData.UserList.Add(userInfo);
    }

    public void ClearGameRoomData()
    {
        if (GameRoomData.UserList != null)
            GameRoomData.UserList.Clear();

        GameRoomData.DungeonId = 0;
        GameRoomData.Owner = null;
        GameRoomData.RoomId = 0;
        GameRoomData.IsLeader = false;
    }

    public int GetGameRoomUserArr(ulong userRoleId)
    {
        int arr = -1;
        for(int i=0; i < GameRoomData.UserList.Count; i++)
        {
            if (GameRoomData.UserList[i].Id != userRoleId)
                continue;

            arr = GameRoomData.UserList[i].Slot;
            break;
        }

        return arr;
        ////방장이 조회 할 경우 0, 1임 파티원이 조회하면 0 ~2임(0은 방장)
        //if (GameRoomData.OwnerId == GetUserInfo().GetCharUUID())
        //    return arr;
        //else
        //    return arr-1;
    }

    public ulong GetGameRoomUserRoleId(int arr)
    {
        //if (GameRoomData.UserList.Count <= arr)//방장이 조회 할 경우 0, 1임 파티원이 조회하면 0 ~2임(0은 방장)
        //    return 0;

        for(int i=0; i < GameRoomData.UserList.Count; i++)
        {
            if (GameRoomData.UserList[i].Slot == arr)
                return GameRoomData.UserList[i].Id;
        }

        return 0;
    }

    #endregion

    #region 게임 보상 정보

    public void AddGetRewardItem(uint lowDataId, ushort amount)
    {
        _RewardData.GetList.Add(new DropItemData(lowDataId, amount, (int)lowDataId));
    }
    
    public List<DropItemData> GetRewardCardItem()
    {
        return _RewardData.CardList;
    }

    public uint GetRewardStageId()
    {
        return _RewardData.StageId;
    }

    public void ClearRewardData()
    {
        if (_RewardData == null)
            return;

        _RewardData.GetList.Clear();
        _RewardData.CardList.Clear();
        _RewardData.GetExp = 0;
        _RewardData.GetCoin = 0;
        _RewardData.SaveLevel = 0;
        _RewardData.SaveExp = 0;
        _RewardData.SaveMaxExp = 0;
        _RewardData.StageId = 0;
        _RewardData.GetAsset = 0;
    }
    
    #endregion

    /// <summary> 유저 로그아웃? </summary>
    public void InitUserData()
    {
        _userInfo.ClearData();
        SceneManager.instance.UserLogout();
    }
}