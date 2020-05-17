using Core.Net;
using UnityEngine;
using System.Net;
using Network = Core.Net.Network;
using Protocol;
using System.Text;
using Sw;
using System.Collections.Generic;

public partial class NetworkClient
{
    #region Request

    /// <summary>
    /// 길드 설립 요청
    /// </summary>
    /// <param name="icon">아이콘</param>
    /// <param name="name">길드이름</param>
    /// <param name="declaration">길드선언</param>
    /// <param name="autoAdd"> 1=심사가입 2=자유가입 </param>
    /// <param name="createType"> 1=원보설립 2=골드설립 </param>
    /// <returns></returns>
    public bool SendPMsgGuildCreateNewC(uint icon, string name, string declaration, uint autoAdd, uint createType)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildCreateNewC();
        sendMsg.UnIcon = icon;
        sendMsg.SzName = name;
        sendMsg.SzDeclaration = declaration;
        sendMsg.UnAutoAdd = autoAdd;
        sendMsg.UnCreateType = createType;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_CREATE_NEW_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 기본 정보 요청
    /// </summary>
    /// <param name="id"> 길드id </param>
    /// <returns></returns>
    public bool SendPMsgGuildQueryBaseInfoC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildQueryBaseInfoC();
        sendMsg.UnGuildId = id;

        SceneManager.instance.ShowNetProcess("PMsgGuildQueryBaseInfoC");
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_BASE_INFO_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 상세정보 요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <returns></returns>
    public bool SendPMsgGuildQueryDetailedInfoC(uint id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgGuildQueryDetailedInfoC");

        var sendMsg = new PMsgGuildQueryDetailedInfoC();
        sendMsg.UnGuildId = id;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_DETAILED_INFO_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 구성원 리스트요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <returns></returns>
    public bool SendPMsgGuildMemberListC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildMemberListC();
        sendMsg.UnGuildId = id;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_MEMBER_LIST_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 추천길드 리스트 요청
    /// </summary>
    /// <returns></returns>
    public bool SendPMsgGuildRecommendListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildRecommendListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_RECOMMEND_LIST_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 아이콘변경요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="icon">새길드아이콘</param>
    /// <returns></returns>
    public bool SendPMsgGuildChangeIconC(uint id, uint icon)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildChangeIconC();
        sendMsg.UnGuildId = id;
        sendMsg.UnNewIcon = icon;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_CHANGE_ICON_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 이름, 선언, 공고변경요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.길드이름 2.길드선언 3.길드공고</param>
    /// <param name="str">새로운내용</param>
    /// <returns></returns>
    public bool SendPMsgGuildChangeNameDeclarationAnnouncementC(uint id, uint type, string str)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildChangeNameDeclarationAnnouncementC();
        sendMsg.UnGuildId = id;
        sendMsg.UnType = type;
        sendMsg.SzNewValue = str;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_CHANGE_NAME_DECLARATION_ANNOUNCEMENT_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드검색요청
    /// </summary>
    /// <param name="name">길드이름</param>
    /// <returns></returns>
    public bool SendPMsgGuildSearchGuildC(string name)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildSearchGuildC();
        sendMsg.SzGuildName = name;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_SEARCH_GUILD_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 가입요청/취소 
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.가입 2.가입취소</param>
    /// <returns></returns>
    public bool SendPMsgGuildApplyGuildC(uint id, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildApplyGuildC();
        sendMsg.UnGuildId = id;
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_APPLY_GUILD_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 길드가입 신청자리스트 요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <returns></returns>
    public bool SendPMsgGuildQueryApplyListC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildQueryApplyListC();
        sendMsg.UnGuildId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_APPLYLIST_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 내가신청한 길드리스트 요청
    /// </summary>
    /// <returns></returns>
    public bool SendPMsgGuildQueryGuildListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildQueryGuildListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_GUILDLIST_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 길드 가입신청 심사
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="roleId">캐릭id</param>
    /// <param name="accede">1.동의 2.거절</param>
    /// <returns></returns>
    public bool SendPMsgGuildExamineApplicantC(uint id, ulong roleId, uint accede)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildExamineApplicantC();
        sendMsg.UnGuildId = id;
        sendMsg.UllRoleId = roleId;
        sendMsg.Unaccede = accede;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_EXAMINE_APPLICANT_C, sendMsg);
        return true;

    }
    /// <summary>
    /// 길드 로비,축원,상점렙업요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.길드로비 2.축원 3.상점</param>
    /// <returns></returns>
    public bool SendPMsgGuildUpgradeLevelC(uint id, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildUpgradeLevelC();
        sendMsg.UnGuildId = id;
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_UPGRADE_LEVEL_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 신규길원 가입방법 설정요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.아무나 2.심사가입</param>
    /// <returns></returns>
    public bool SendPMsgSetGuildJoinsetC(uint id, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgSetGuildJoinsetC();
        sendMsg.UnGuildId = id;
        sendMsg.UnJoinSet = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_SET_JOINSET_GUILD_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 축원요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.일반축원 2.고급축원</param>
    /// <returns></returns>
    public bool SendPMsgGuildGuildPrayC(uint id, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildGuildPrayC();
        sendMsg.UnGuildId = id;
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_GUILDPRAY_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 길드 기부요청
    /// </summary>
    /// <param name="id">길드id</param>
    /// <param name="type">1.저랩기부 2.중랩기부 3.고랩기부</param>
    /// <returns></returns>
    public bool SendPMsgGuildGuildDonateC(uint id, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildGuildDonateC();
        sendMsg.UnGuildId = id;
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_GUILDDONATE_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 직위 위임요청
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <param name="type">2.부길마 3.정예 4.길원</param>
    /// <returns></returns>
    public bool SendPMsgGuildAppointPositionC(uint id, ulong roleId, uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildAppointPositionC();
        sendMsg.UnGuildId = id;
        sendMsg.UllRoleId = roleId;
        sendMsg.UnPositionType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_APPOINT_POSITION_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길원 추방
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public bool SendPMsgGuildKitkMemberC(uint id, ulong roleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildKitkMemberC();
        sendMsg.UnGuildId = id;
        sendMsg.UllRoleId = roleId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_KITK_MEMBER_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길마 위임요청
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public bool SendPMsgGuildAppointGuildLeaderC(uint id, ulong roleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildAppointGuildLeaderC();
        sendMsg.UnGuildId = id;
        sendMsg.UllRoleId = roleId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_APPOINT_GUILD_LEADER_C, sendMsg);
        return true;
    }
    /// <summary>
    /// 길드 탈퇴/해산신청
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool SendPMsgGuildSecedeGuildC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildSecedeGuildC();
        sendMsg.UnGuildId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_SECEDE_GUILD_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드 실시간 정보요청
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool SendPMsgGuildQueryGuildStatusC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildQueryGuildStatusC();
        sendMsg.UnGuildId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_GUILD_STATUS_C, sendMsg);
        return true;
    }

    /// <summary>
    /// 길드에서 자신의정보요청
    /// </summary>
    public bool SendPMsgGuildQuerySelfInfoC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildQuerySelfInfoC();
        sendMsg.UnGuildId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_QUERY_SELF_INFO_C, sendMsg);
        return true;
    }

    public bool SendPMsgGuildTaskQueryInfoC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildTaskQueryInfoC();
        sendMsg.UnGuildId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_TASK_QUERY_INFO_C, sendMsg);
        return true;
    }

    public bool SendPMsgGuildTaskFetchBonusC(uint id, uint taskId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildTaskFetchBonusC();
        sendMsg.UnGuildId = id;
        sendMsg.UnTaskId = taskId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_TASK_FETCH_BONUS_C, sendMsg);
        return true;
    }

    public bool SendPMsgFetchRewardGuildUserTaskQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFetchRewardGuildUserTaskQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FETCHREWARD_GUILD_TASK_QUERY_INFO_C, sendMsg);
        return true;
    }

    public bool SendPMsgGuildUserTaskQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildUserTaskQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_USER_TASK_QUERY_INFO_C, sendMsg);
        return true;
    }

    public bool SendPMsgGuildUserTaskFetchBonusC(uint taskId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildUserTaskFetchBonusC();
        sendMsg.UnTaskId = taskId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_USER_TASK_FETCH_BONUS_C, sendMsg);
        return true;
    }
    public bool SendPMsgGuildUserTaskAllocatingTaskC(uint taskId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGuildUserTaskAllocatingTaskC();
        sendMsg.UnTaskId = taskId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_USER_TASK_ALLOCATINGTASK_C, sendMsg);
        return true;
    }

    /// <summary> 보낸 길드 아이디에 맞게 길드 리스트를 넘겨준다 </summary>
    public bool SendPMsgGuildNameQueryC(List<ulong> guildList)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgGuildNameQueryC");
        var sendMsg = new PMsgGuildNameQueryC();
        for (int i = 0; i < guildList.Count; i++)
        {
            sendMsg.UllGuildId.Add(guildList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_NAME_QUERY_C, sendMsg);
        return true;
    }

    public bool SendPMsgGuildSetRoleLevelForJoinGuildC(uint id, uint level)
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgGuildSetRoleLevelForJoinGuildC");
        var sendMsg = new PMsgGuildSetRoleLevelForJoinGuildC();
        sendMsg.UnGuildId = id;
        sendMsg.UnLevel = level;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_SET_ROLE_LEVEL_FOR_JOIN_GUILD_C, sendMsg);
        return true;
    }


    /// <summary> 캐릭터 아이디로 길드정보 찾아옴 </summary>
    public bool SendPMsgGuildIDQueryC(List<ulong> roleIdList)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgGuildIDQueryC");
        var sendMsg = new PMsgGuildIDQueryC();
        for (int i = 0; i < roleIdList.Count; i++)
        {
            sendMsg.UllRoleId.Add(roleIdList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GUILD_ID_QUERY_C, sendMsg);
        return true;
    }


    #endregion

    #region Response

    //길드 설립요청 
    private void PMsgGuildCreateNewSHandler(PMsgGuildCreateNewS pmsgGuildCreateNewS)
    {
        uint ErrorCode = (uint)pmsgGuildCreateNewS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == 1262)//이름중복
            {
                UIMgr.instance.AddPopup(141, 626, 117);
            }
            else if (ErrorCode == 1259)//길드이름 문자규칙오류
            {
                UIMgr.instance.AddPopup(141, 625, 117);

            }
            else if (ErrorCode == 1258) //이름빈칸
            {
                UIMgr.instance.AddPopup(141, 625, 117);

            }
            //else if(ErrorCode == )//선언,공지도추가해주기
            //{

            //}
            else
            {
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
            }

            Debug.Log(pmsgGuildCreateNewS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //Debug.Log("PMsgGuildCreateNewS - " + pmsgGuildCreateNewS);
        NetData.instance.GetUserInfo()._GuildId = pmsgGuildCreateNewS.UnGuildId;
        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null)
            (joinPanel as GuildJoinPanel).OnPMsgCreateGuild();

        //UIMgr.OpenGuildPanel();
    }

    //길드 기본정보요청 
    private void PMsgGuildQueryBaseInfoSHandler(PMsgGuildQueryBaseInfoS pmsgGuildQueryBaseInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgGuildQueryBaseInfoC");
        uint ErrorCode = (uint)pmsgGuildQueryBaseInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildQueryBaseInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildQueryBaseInfoS - " + pmsgGuildQueryBaseInfoS);

        List<NetData.GuildSimpleInfo> SimpleInfoList = new List<NetData.GuildSimpleInfo>();
        for (int i = 0; i < pmsgGuildQueryBaseInfoS.CSimpleInfo.Count; i++)
        {
            Sw.GuildSimpleInfo info = pmsgGuildQueryBaseInfoS.CSimpleInfo[i];
            NetData.GuildSimpleInfo SimpleInfo;
            SimpleInfo.Icon = info.UnIcon;
            SimpleInfo.Id = info.UnGuildId;
            SimpleInfo.Name = info.SzName;
            SimpleInfo.LeaderName = info.SzGuildLeaderName;
            //SimpleInfo.Declaration = info.SzDeclaration;
            SimpleInfo.Count = info.UnMemberCount;
            SimpleInfo.guildLv = info.UnGuildLevel;
            SimpleInfo.JoinSet = info.UnJoinSet;
            SimpleInfo.CreateTime = info.UllCreateTime;
            SimpleInfo.Attack = info.UnGuildAttack;
            SimpleInfo.JoinLevel = info.UnJoinRoleLevel;

            SimpleInfoList.Add(SimpleInfo);
        }

        UIBasePanel rankPanel = UIMgr.GetUIBasePanel("UIPanel/RankingPanel");
        if (rankPanel != null && !rankPanel.IsHidePanel)
            (rankPanel as RankPanel).OnMyGuildInfo(SimpleInfoList[0]);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null && !basePanel.IsHidePanel)
            (basePanel as GuildPanel).SetGuildInfo(SimpleInfoList);

        UIBasePanel guildpnel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (guildpnel != null && !basePanel.IsHidePanel)
            (guildpnel as GuildJoinPanel).OnGuildDetailInfo(SimpleInfoList[0]);
    }

    //길드 상세정보
    private void PMsgGuildQueryDetailedInfoSHandler(PMsgGuildQueryDetailedInfoS pmsgGuildQueryDetailedInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgGuildQueryDetailedInfoC");
        uint ErrorCode = (uint)pmsgGuildQueryDetailedInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildQueryDetailedInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgGuildQueryDetailedInfoS);

        if (pmsgGuildQueryDetailedInfoS.CDetailedInfo.Count <= 0)
            return;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null && !basePanel.IsHidePanel)
        {
            GuildDetailedInfo info = pmsgGuildQueryDetailedInfoS.CDetailedInfo[0];
            NetData.GuildDetailedInfo guildInfo = new NetData.GuildDetailedInfo
               (info.UnGuildLevel, info.UnBlessLevel, info.UnShopLevel, info.SzDeclaration, info.SzAnnouncement, info.UnBankroll);
            (basePanel as GuildPanel).SetGuildDetailInfo(guildInfo);

            return;
        }

        UIBasePanel rankPanel = UIMgr.GetUIBasePanel("UIPanel/RankingPanel");
        if(rankPanel != null && !rankPanel.IsHidePanel)
        {
            GuildDetailedInfo info = pmsgGuildQueryDetailedInfoS.CDetailedInfo[0];
            (rankPanel as RankPanel).OnGuildDetailInfoPopup(info.UnGuildLevel, info.UnBlessLevel, info.UnShopLevel, info.SzDeclaration, info.SzAnnouncement, info.UnBankroll);
        }
        
        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null && !(joinPanel as GuildJoinPanel).IsCreateGuild && !joinPanel.IsHidePanel)//길드 가입에서 상세정보를 보기 했을 경우.
        {
            GuildDetailedInfo info = pmsgGuildQueryDetailedInfoS.CDetailedInfo[0];
            (joinPanel as GuildJoinPanel).OnPMsgGuildDetailInfo(info.UnGuildLevel, info.UnBlessLevel, info.UnShopLevel, info.SzDeclaration, info.SzAnnouncement, info.UnBankroll);
        }
        else
        {
            //상점이 열려있을때만... 
            UIBasePanel panel = UIMgr.instance.FindInShowing("UIPanel/ShopPanel");
            if (panel != null && !panel.IsHidePanel)
            {
                //상점패널에서 길드상점레벨이 필요하므로 두개를 나눠서...
                //uint lv = 0;
                //for (int i = 0; i < pmsgGuildQueryDetailedInfoS.CDetailedInfo.Count; i++)//어차피 0번째 배열로 가져올거면서 왜 포문을 도는건지?
                //{
                //uint lv = pmsgGuildQueryDetailedInfoS.CDetailedInfo[0].UnGuildLevel;
                (panel as ShopPanel).GuildShopView(pmsgGuildQueryDetailedInfoS.CDetailedInfo[0].UnGuildLevel);
            }
        }
    }

    //길드 구성원 리스트
    private void PMsgGuildMemberListSHandler(PMsgGuildMemberListS pmsgGuildMemberListS)
    {
        uint ErrorCode = (uint)pmsgGuildMemberListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildMemberListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildMemberListS - " + pmsgGuildMemberListS);

        List<NetData.GuildMemberInfo> memberList = new List<NetData.GuildMemberInfo>();
        for (int i = 0; i < pmsgGuildMemberListS.UnMemberCount; i++)
        {
            Sw.GuildMemberInfo Ginfo = pmsgGuildMemberListS.CMemberInfo[i];
            NetData.GuildMemberInfo info = new NetData.GuildMemberInfo(
                Ginfo.UllRoleId,
                Ginfo.SzName,
                Ginfo.UnRoleType,
                Ginfo.UnRoleLevel,
                Ginfo.UnPosition,
                Ginfo.UllContribution,
                Ginfo.UllLoginTime,
                Ginfo.UllLogOutTime,
                Ginfo.UnRolePower,
                Ginfo.UnOnLineStatus);

            memberList.Add(info);
        }

        UIBasePanel invitePop = UIMgr.GetUIBasePanel("UIPopup/InvitePopup");
        if (invitePop != null)
        {
            (invitePop as InvitePopup).OnGuildMemberList(memberList);
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).SetGuildMemberList(memberList);

    }

    //추천길드 리스트
    private void PMsgGuildRecommendListSHandler(PMsgGuildRecommendListS pmsgGuildRecommendListS)
    {
        uint ErrorCode = (uint)pmsgGuildRecommendListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildRecommendListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildRecommendListS - " + pmsgGuildRecommendListS);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (basePanel == null)
            return;

        List<NetData.GuildSimpleInfo> recommendList = new List<NetData.GuildSimpleInfo>();
        int count = pmsgGuildRecommendListS.CSimpleInfo.Count;
        for (int i = 0; i < count; i++)
        {
            Sw.GuildSimpleInfo info = pmsgGuildRecommendListS.CSimpleInfo[i];
            recommendList.Add(new NetData.GuildSimpleInfo(info.UnIcon, info.UnGuildId, info.SzName, info.SzGuildLeaderName, info.UnMemberCount, info.UnGuildLevel, info.UnJoinSet, info.UllCreateTime, info.UnGuildAttack, info.UnJoinRoleLevel));
        }

        (basePanel as GuildJoinPanel).OnPMsgRecommendList(recommendList);
    }

    //길드 아이콘 변경 PMsgGuildChangeIconS
    private void PMsgGuildChangeIconSHandler(PMsgGuildChangeIconS pmsgGuildChangeIconS)
    {
        uint ErrorCode = (uint)pmsgGuildChangeIconS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildChangeIconS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildChangeIconS - " + pmsgGuildChangeIconS);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).RefreshIcon(pmsgGuildChangeIconS.UnNewIcon);

    }

    //길드이름, 선언, 공고변경
    private void PMsgGuildChangeNameDeclarationAnnouncementSHandler(PMsgGuildChangeNameDeclarationAnnouncementS pmsgGuildChangeNameDeclarationAnnouncementS)
    {
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");

        uint ErrorCode = (uint)pmsgGuildChangeNameDeclarationAnnouncementS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (basePanel != null)
                (basePanel as GuildPanel).SetGuildNameDeclareAnoount(false, ErrorCode, pmsgGuildChangeNameDeclarationAnnouncementS.SzNewValue);

            // UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildChangeNameDeclarationAnnouncementS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildChangeNameDeclarationAnnouncementS - " + pmsgGuildChangeNameDeclarationAnnouncementS);
        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null)//길드 생성시에는 공지사항을 입력하는 것이 없다. 생성 후 여기서 호출해준다.
        {
            joinPanel.Close();//닫고
            UIMgr.OpenGuildPanel();//길드 페이지로 넘긴다.
        }


        if (basePanel != null)
            (basePanel as GuildPanel).SetGuildNameDeclareAnoount(true, ErrorCode, pmsgGuildChangeNameDeclarationAnnouncementS.SzNewValue);


    }

    //길드검색 
    private void PMsgGuildSearchGuildSHandler(PMsgGuildSearchGuildS pmsgGuildSearchGuildS)
    {
        uint ErrorCode = (uint)pmsgGuildSearchGuildS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == (int)Sw.ErrorCode.ER_Guild_SearchGuild_Name_Is_Not_Exist)
            {
                UIBasePanel joinPanel2 = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
                if (joinPanel2 != null)
                    (joinPanel2 as GuildJoinPanel).OnPMsgSearchGuild(null);

                UIBasePanel guild = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
                if (guild != null)
                    (guild as GuildPanel).OnPMsgSearchGuild(null);

            }
            else
                UIMgr.instance.AddErrorPopup((int)ErrorCode);

            Debug.Log(pmsgGuildSearchGuildS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildSearchGuildS - " + pmsgGuildSearchGuildS);
        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null)
        {
            List<NetData.GuildSimpleInfo> infoList = new List<NetData.GuildSimpleInfo>();
            int count = pmsgGuildSearchGuildS.CSimpleInfo.Count;
            for (int i = 0; i < count; i++)
            {
                GuildSimpleInfo info = pmsgGuildSearchGuildS.CSimpleInfo[i];
                infoList.Add(new NetData.GuildSimpleInfo(info.UnIcon, info.UnGuildId, info.SzName, info.SzGuildLeaderName, info.UnMemberCount, info.UnGuildLevel, info.UnJoinSet, info.UllCreateTime, info.UnGuildAttack, info.UnJoinRoleLevel));
            }

            (joinPanel as GuildJoinPanel).OnPMsgSearchGuild(infoList);
        }

        UIBasePanel guildPanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (guildPanel != null)
        {
            List<NetData.GuildSimpleInfo> infoList = new List<NetData.GuildSimpleInfo>();
            int count = pmsgGuildSearchGuildS.CSimpleInfo.Count;
            for (int i = 0; i < count; i++)
            {
                GuildSimpleInfo info = pmsgGuildSearchGuildS.CSimpleInfo[i];
                infoList.Add(new NetData.GuildSimpleInfo(info.UnIcon, info.UnGuildId, info.SzName, info.SzGuildLeaderName, info.UnMemberCount, info.UnGuildLevel, info.UnJoinSet, info.UllCreateTime, info.UnGuildAttack, info.UnJoinRoleLevel));
            }

            (guildPanel as GuildPanel).OnPMsgSearchGuild(infoList);

        }
    }

    //길드가입요청/요청취소 
    private void PMsgGuildApplyGuildSHandler(PMsgGuildApplyGuildS pmsgGuildApplyGuildS)
    {
        uint ErrorCode = (uint)pmsgGuildApplyGuildS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == 1291)   //다른길드소속
            {
                UIMgr.instance.AddPopup(141, 610, 117);
            }
            else if (ErrorCode == 1294) //레벨부족. ,길드설정레벨이들어가므로 다시해줘야함..일단 임시로 20
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(945), 20);
                UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(141), msg, _LowDataMgr.instance.GetStringCommon(117));
            }
            else if (ErrorCode == 1293)//쿨타임중
            {
                UIMgr.instance.AddPopup(141, 629, 117);
            }
            else
            {
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
            }

            Debug.Log(pmsgGuildApplyGuildS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log("PMsgGuildApplyGuildS - " + pmsgGuildApplyGuildS);
        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null)
        {
            //1가입, 2 취소
            (joinPanel as GuildJoinPanel).OnPMsgApply(pmsgGuildApplyGuildS.UnGuildId, pmsgGuildApplyGuildS.UnType == 1);
        }
    }

    //길드가입 신청자리스트 
    private void PMsgGuildQueryApplyListSHandler(PMsgGuildQueryApplyListS pmsgGuildQueryApplyListS)
    {
        uint ErrorCode = (uint)pmsgGuildQueryApplyListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            Debug.Log(pmsgGuildQueryApplyListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));

            if (ErrorCode == (int)Sw.ErrorCode.ER_Guild_HaveNot_Permission)
            {
                if (UIMgr.GetTownBasePanel() != null)
                    return;
            }

            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            return;
        }

        Debug.Log("PMsgGuildQueryApplyListS - " + pmsgGuildQueryApplyListS);

        List<NetData.ApplyRoleInfo> applyList = new List<NetData.ApplyRoleInfo>();
        for (int i = 0; i < pmsgGuildQueryApplyListS.UnApplyCount; i++)
        {
            Sw.ApplyRoleInfo Ginfo = pmsgGuildQueryApplyListS.CRoleInfo[i];
            NetData.ApplyRoleInfo info = new NetData.ApplyRoleInfo(
                Ginfo.UllRoleId,
                Ginfo.SzName,
                Ginfo.UnRoleType,
                Ginfo.UnRoleLevel,
                Ginfo.UnRolePower
               );

            applyList.Add(info);
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).SetAddminView(0, applyList); //관리탭 처음은 무조건 전체보기로 


        bool active = applyList.Count > 0 ? true : false;
        SceneManager.instance.SetAlram(AlramIconType.GUILD, active);

        UIBasePanel town = UIMgr.GetTownBasePanel();
        if (town != null)
            (town as TownPanel).AlramMark[(int)AlramIconType.GUILD].SetActive(active);






    }

    //내가신청한 길드리스트 
    private void PMsgGuildQueryGuildListSHandler(PMsgGuildQueryGuildListS pmsgGuildQueryGuildListS)
    {
        uint ErrorCode = (uint)pmsgGuildQueryGuildListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildQueryGuildListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildQueryGuildListS - " + pmsgGuildQueryGuildListS);

        UIBasePanel joinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (joinPanel != null)
        {
            List<uint> joinList = new List<uint>();
            int count = pmsgGuildQueryGuildListS.CGuildInfo.Count;
            for (int i = 0; i < count; i++)
            {
                GuildSimpleInfo info = pmsgGuildQueryGuildListS.CGuildInfo[i];
                joinList.Add(info.UnGuildId);
            }

            (joinPanel as GuildJoinPanel).OnPMsgJoinList(joinList);
        }

    }


    //길드 가입신청 심사  
    private void PMsgGuildExamineApplicantSHandler(PMsgGuildExamineApplicantS pmsgGuildExamineApplicantS)
    {
        uint ErrorCode = (uint)pmsgGuildExamineApplicantS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildExamineApplicantS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildExamineApplicantS - " + pmsgGuildExamineApplicantS);
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).MemberCountChange(pmsgGuildExamineApplicantS.Unaccede);

    }

    //길드 가입신청의 심사결과 클라에 동기화 
    private void PMsgGuildSynExamineApplicantSHandler(PMsgGuildSynExamineApplicantS pmsgGuildSynExamineApplicantS)
    {
        uint ErrorCode = (uint)pmsgGuildSynExamineApplicantS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildSynExamineApplicantS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log("PMsgGuildSynExamineApplicantS - " + pmsgGuildSynExamineApplicantS);


    }

    // 길드로비, 축원, 상점렙업
    private void PMsgGuildUpgradeLevelSHandler(PMsgGuildUpgradeLevelS pmsgGuildUpgradeLevelS)
    {
        uint ErrorCode = (uint)pmsgGuildUpgradeLevelS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildUpgradeLevelS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }


        uint type = pmsgGuildUpgradeLevelS.UnType;
        uint typeLv = pmsgGuildUpgradeLevelS.UnGuildLevel;

        Debug.Log("PMsgGuildUpgradeLevelS - " + pmsgGuildUpgradeLevelS);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).GoLobby(type, typeLv);

    }

    //길드 신규길원 가입방법 설정 
    private void PMsgSetGuildJoinsetSHandler(PMsgSetGuildJoinsetS pmsgSetGuildJoinsetS)
    {
        uint ErrorCode = (uint)pmsgSetGuildJoinsetS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgSetGuildJoinsetS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgSetGuildJoinsetS - " + pmsgSetGuildJoinsetS);
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
        {
            UIMgr.instance.AddPopup(141, 1137, 117);
        }
    }
    //길드 축원
    private void PMsgGuildGuildPraySHandler(PMsgGuildGuildPrayS pmsgGuildGuildPrayS)
    {
        uint ErrorCode = (uint)pmsgGuildGuildPrayS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildGuildPrayS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //   Debug.Log("PMsgGuildGuildPrayS - " + pmsgGuildGuildPrayS);



    }

    //길드기부 
    private void PMsgGuildGuildDonateSHandler(PMsgGuildGuildDonateS pmsgGuildGuildDonateS)
    {
        uint ErrorCode = (uint)pmsgGuildGuildDonateS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildGuildDonateS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //     Debug.Log("PMsgGuildGuildDonateS - " + pmsgGuildGuildDonateS);


    }

    //직위 위임 
    private void PMsgGuildAppointPositionSHandler(PMsgGuildAppointPositionS pmsgGuildAppointPositionS)
    {
        bool sucess = (uint)pmsgGuildAppointPositionS.UnErrorCode == (uint)Sw.ErrorCode.ER_success;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).PositionChange(sucess, (uint)pmsgGuildAppointPositionS.UnErrorCode, pmsgGuildAppointPositionS.UnPositionType);

        //uint ErrorCode = (uint)pmsgGuildAppointPositionS.UnErrorCode;
        //if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        //{
        //    UIMgr.instance.AddErrorPopup((int)ErrorCode);
        //    Debug.Log(pmsgGuildAppointPositionS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
        //    return;
        //}

        Debug.Log("PMsgGuildAppointPositionS - " + pmsgGuildAppointPositionS);



    }

    //길원추방 
    private void PMsgGuildKitkMemberSHandler(PMsgGuildKitkMemberS pmsgGuildKitkMemberS)
    {
        uint ErrorCode = (uint)pmsgGuildKitkMemberS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildKitkMemberS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log("PMsgGuildKitkMemberS - " + pmsgGuildKitkMemberS);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).PositionChange(true, (uint)pmsgGuildKitkMemberS.UnErrorCode, 5);



    }

    //추방당한 길원통지 
    private void PMsgGuildSynKitkMemberSHandler(PMsgGuildSynKitkMemberS pmsgGuildSynKitkMemberS)
    {
        Debug.Log("PMsgGuildSynKitkMemberS - " + pmsgGuildSynKitkMemberS);

        NetData.instance.GetUserInfo()._GuildId = 0;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)//길드창을 보고있을시
            (basePanel as GuildPanel).OutMemberNotice();
        else//길드창을 보고있으지 않다면 채팅창에 뿌려놓는다
        {
            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
            if (chat != null)
            {
                (chat as ChatPopup).OnReciveChat(_LowDataMgr.instance.GetStringCommon(674), ChatType.World);
            }
        }

    }

    //길마 위임 
    private void PMsgGuildAppointGuildLeaderSHandler(PMsgGuildAppointGuildLeaderS pmsgGuildAppointGuildLeaderS)
    {
        uint ErrorCode = (uint)pmsgGuildAppointGuildLeaderS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildAppointGuildLeaderS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).PositionChange(true, ErrorCode, 1);

        Debug.Log("PMsgGuildAppointGuildLeaderS - " + pmsgGuildAppointGuildLeaderS);
    }

    //길드탈퇴/해산  
    private void PMsgGuildSecedeGuildSHandler(PMsgGuildSecedeGuildS pmsgGuildSecedeGuildS)
    {
        uint ErrorCode = (uint)pmsgGuildSecedeGuildS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildSecedeGuildS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildSecedeGuildS - " + pmsgGuildSecedeGuildS);
        NetData.instance.GetUserInfo()._GuildId = 0;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).Close();

    }

   
    //클라에 길드속성변경 통지 PMsgGuildAttributeS
    private void PMsgGuildAttributeSHandler(PMsgGuildAttributeS pmsgGuildAttributeS)
    {

        Debug.Log("PMsgGuildAttributeS - " + pmsgGuildAttributeS);

        uint type = pmsgGuildAttributeS.UnAttribType;
        ulong value = pmsgGuildAttributeS.UllValue;
        string strValue = pmsgGuildAttributeS.StrValue;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).AttributeUpdate(type, value, strValue);
    }

    //길드 실시간응답
    private void PMsgGuildQueryGuildStatusSHandler(PMsgGuildQueryGuildStatusS pmsgGuildQueryGuildStatusS)
    {
        uint ErrorCode = (uint)pmsgGuildQueryGuildStatusS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildQueryGuildStatusS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildQueryGuildStatusS - " + pmsgGuildQueryGuildStatusS);

        List<NetData.GuildStatusInfo> statusList = new List<NetData.GuildStatusInfo>();
        for (int i = 0; i < pmsgGuildQueryGuildStatusS.CStatusInfo.Count; i++)
        {
            Sw.GuildStatusInfo info = pmsgGuildQueryGuildStatusS.CStatusInfo[i];
            NetData.GuildStatusInfo guildInfo = new NetData.GuildStatusInfo
            (
               info.UnGuildId,
               info.UnType,
               info.SzRoleName1,
               info.SzRoleName2,
               info.UnParam1,
               info.StrParam2,
               info.UllTime

            //

            );
            statusList.Add(guildInfo);
        }



        //길드알림
        for (int i = 0; i < pmsgGuildQueryGuildStatusS.CStatusInfo.Count; i++)
        {
            Sw.GuildStatusInfo info = pmsgGuildQueryGuildStatusS.CStatusInfo[i];
            string msg = "";
            uint msgType = info.UnType;
            switch (msgType)
            {
                case 1: //길드가입
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(845), info.SzRoleName1);
                    //msg += info.SzRoleName1;
                    //msg += "길드에 가입했습니다";
                    break;
                case 2: //길드탈퇴
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(846), info.SzRoleName1);
                    //msg += info.SzRoleName1;
                    //msg += "길드를 탈퇴했습니다";
                    break;
                case 3: //직위에 피임명 
                    Guild.GuildPositionInfo position = _LowDataMgr.instance.GetLowdataGuildPositionInfo(info.UnParam1);
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(847), info.SzRoleName1, _LowDataMgr.instance.GetStringCommon(position.name));
                    //msg += info.SzRoleName1;
                    //msg += _LowDataMgr.instance.GetStringCommon(position.name);
                    //msg += "임명되었습니다";
                    break;
                case 4: //길마되기
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(848), info.SzRoleName1);

                    //msg += info.SzRoleName1;
                    //msg += "새로운 길마가 되었습니다";
                    break;
                case 5:
                    uint[] donateTableIdx = { 10000, 11000, 12000 };
                    Guild.DonateInfo donateInfo = _LowDataMgr.instance.GetLowdataGuildDonateInfo(donateTableIdx[info.UnParam1 - 1]);
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(849), info.SzRoleName1, donateInfo.DonateValue,
                        donateInfo.DonateType == 1 ? _LowDataMgr.instance.GetStringCommon(4) : _LowDataMgr.instance.GetStringCommon(3));

                    //msg += info.SzRoleName1;
                    //uint[] donateTableIdx = { 10000, 11000, 12000 };
                    //Guild.DonateInfo donateInfo = _LowDataMgr.instance.GetLowdataGuildDonateInfo(donateTableIdx[info.UnParam1 - 1]);
                    //msg += donateInfo.DonateValue;
                    //msg += donateInfo.DonateType == 1 ? "골드" : "원보";
                    //msg += "기부했습니다";
                    break;
                case 6:
                    msg += string.Format(_LowDataMgr.instance.GetStringCommon(850), info.SzRoleName1);

                    //msg += info.SzRoleName1;
                    //msg += "축원하였습니다";
                    break;
                case 7:
                    msg += _LowDataMgr.instance.GetStringCommon(851);
                    break;
                case 8:
                    msg += _LowDataMgr.instance.GetStringCommon(852);
                    break;

            }

            // 채팅에 쁘리라
            if (msg != "")
            {
                //ChatPopup chat = SceneManager.instance.ChatPopup(false);
                UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
                if (chat == null)
                    return;
                (chat as ChatPopup).OnReciveChat(msg, ChatType.Guild);
            }

        }

        if (statusList[0].Type == 1)//실시간 가입 응답
        {
            NetData.instance.GetUserInfo()._GuildId = statusList[0].Id;
            UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
            if (basePanel != null)
            {
                (basePanel as GuildJoinPanel).OnPMsgJoinGuild();
            }
        }

        //일단 축원만
        if (statusList[0].Type == 6)
        {

            string[] str = statusList[0].Param2.Split(';');

            List<uint> id = new List<uint>();
            List<uint> type = new List<uint>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == "")
                    continue;
                string[] index = str[i].Split(' ');
                id.Add(uint.Parse(index[1]));
                type.Add(uint.Parse(index[0]));
            }

            if (statusList[0].Name == NetData.instance.GetUserInfo()._charName)
            {
                UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
                if (basePanel != null)
                    (basePanel as GuildPanel).BlessResult(statusList[0].Name, id, type);
            }

        }









    }

    //길드 캐릭터 자신의 정보획득
    private void PMsgGuildQuerySelfInfoSHandler(PMsgGuildQuerySelfInfoS pmsgGuildQuerySelfInfoS)
    {
        uint ErrorCode = (uint)pmsgGuildQuerySelfInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildQuerySelfInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgGuildQuerySelfInfoS - " + pmsgGuildQuerySelfInfoS);

        NetData.GuildSelfInfo myInfo = null;
        myInfo = new NetData.GuildSelfInfo(
            pmsgGuildQuerySelfInfoS.UnProfession,
            pmsgGuildQuerySelfInfoS.UnPosition,
            pmsgGuildQuerySelfInfoS.UllAddGuildTime,
            pmsgGuildQuerySelfInfoS.UnContributionTotal,
            pmsgGuildQuerySelfInfoS.UnContributionSpendTotal,
            pmsgGuildQuerySelfInfoS.UnDonateCoinTotal,
            pmsgGuildQuerySelfInfoS.UnDonateGemTotal);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).SetGuildMyInfo(myInfo);
    }

   
    // 길드 퀘스트 정보 조회 응답
    private void PMsgGuildTaskQueryInfoSHandler(PMsgGuildTaskQueryInfoS pmsgGuildTaskQueryInfoS)
    {
        uint ErrorCode = (uint)pmsgGuildTaskQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildTaskQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //Debug.Log("PMsgGuildTaskQueryInfoS - " + pmsgGuildTaskQueryInfoS);

        List<NetData.GuildTaskInfo> taskList = new List<NetData.GuildTaskInfo>();
        for (int i = 0; i < pmsgGuildTaskQueryInfoS.UnTaskCount; i++)
        {
            Sw.GuildTaskInfo task = pmsgGuildTaskQueryInfoS.CTaskInfos[i];

            NetData.GuildTaskInfo info = new NetData.GuildTaskInfo(task.UnTaskId, task.UnGuildTaskType, task.UnCondValueTotal, task.UnComplete);
            taskList.Add(info);
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).GetQuestList(taskList);

    }
    //길드 퀘스트 보상획득 요청응답
    private void PMsgGuildTaskFetchBonusSHandler(PMsgGuildTaskFetchBonusS pmsgGuildTaskFetchBonusS)
    {
        uint ErrorCode = (uint)pmsgGuildTaskFetchBonusS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildTaskFetchBonusS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        // Debug.Log("PMsgGuildTaskFetchBonusS - " + pmsgGuildTaskFetchBonusS);
    }
    //길드퀘스트 클리어 통보
    private void PMsgGuildTaskSynTaskCompleteSHandler(PMsgGuildTaskSynTaskCompleteS pmsgGuildTaskSynTaskCompleteS)
    {

        //  Debug.Log("PMsgGuildTaskSynTaskCompleteS - " + pmsgGuildTaskSynTaskCompleteS);
    }
    //개인이 이밈 보상을 획득한 길드퀘스트 정보조회
    private void PMsgFetchRewardGuildUserTaskQueryInfoSHandler(PMsgFetchRewardGuildUserTaskQueryInfoS pmsgFetchRewardGuildUserTaskQueryInfoS)
    {
        uint ErrorCode = (uint)pmsgFetchRewardGuildUserTaskQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgFetchRewardGuildUserTaskQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //    Debug.Log("PMsgFetchRewardGuildUserTaskQueryInfoS - " + pmsgFetchRewardGuildUserTaskQueryInfoS);
    }
    //길드 개인퀘스트 정보조회응답
    private void PMsgGuildUserTaskQueryInfoSHandler(PMsgGuildUserTaskQueryInfoS pmsgGuildUserTaskQueryInfoS)
    {
        uint ErrorCode = (uint)pmsgGuildUserTaskQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildUserTaskQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //   Debug.Log("PMsgGuildUserTaskQueryInfoS - " + pmsgGuildUserTaskQueryInfoS);

        List<NetData.GuildUserTaskInfo> taskList = new List<NetData.GuildUserTaskInfo>();
        for (int i = 0; i < pmsgGuildUserTaskQueryInfoS.UnTaskCount; i++)
        {
            Sw.GuildUserTaskInfo task = pmsgGuildUserTaskQueryInfoS.CTaskInfos[i];

            NetData.GuildUserTaskInfo info = new NetData.GuildUserTaskInfo(task.UnTaskId, task.UnGuildTaskType, task.UnCondValueTotal, task.UnComplete, task.UnCondFetchReward);
            taskList.Add(info);
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).GetUserQuestList(taskList);

    }
    //길드 개인퀘스트 보상획득요청응답
    private void PMsgGuildUserTaskFetchBonusSHandler(PMsgGuildUserTaskFetchBonusS pmsgGuildUserTaskFetchBonusS)
    {
        uint ErrorCode = (uint)pmsgGuildUserTaskFetchBonusS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildUserTaskFetchBonusS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        // Debug.Log("PMsgGuildUserTaskFetchBonusS - " + pmsgGuildUserTaskFetchBonusS);
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).CompleteQuest(pmsgGuildUserTaskFetchBonusS.UnTaskId, 1);
    }
    //길드 개인퀘스트 분배요청 응답
    private void PMsgGuildUserTaskAllocatingTaskSHandler(PMsgGuildUserTaskAllocatingTaskS pmsgGuildUserTaskAllocatingTaskS)
    {
        uint ErrorCode = (uint)pmsgGuildUserTaskAllocatingTaskS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildUserTaskAllocatingTaskS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        // Debug.Log("PMsgGuildUserTaskAllocatingTaskS - " + pmsgGuildUserTaskAllocatingTaskS);


        NetData.GuildUserTaskInfo info = new NetData.GuildUserTaskInfo(pmsgGuildUserTaskAllocatingTaskS.UnTaskId, pmsgGuildUserTaskAllocatingTaskS.UnGuildTaskType, pmsgGuildUserTaskAllocatingTaskS.UnCondValueTotal, pmsgGuildUserTaskAllocatingTaskS.UnComplete, 0);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).ResetGuildUserQuest(info);

    }
    //길드 개인퀘스트 클리어 통지
    private void PMsgGuildUserTaskSynCompleteSHandler(PMsgGuildUserTaskSynCompleteS pmsgGuildUserTaskSynCompleteS)
    {
        //  SceneManager.instance.SetAlram(AlramIconType.GUILD] = 1;

    }

    /// <summary> 길드 리스트 이름 </summary>
    private void PMsgGuildNameQuerySHandler(PMsgGuildNameQueryS pmsgGuildNameQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgGuildNameQueryC");
        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (arenaPanel != null)
        {
            arenaPanel.NetworkData(MSG_DEFINE._MSG_GUILD_NAME_QUERY_S, pmsgGuildNameQueryS);
        }
    }

    //길드가입레벨 변경
    private void PMsgGuildSetRoleLevelForJoinGuildSHandler(PMsgGuildSetRoleLevelForJoinGuildS pmsgGuildSetRoleLevelForJoinGuildS)
    {
        SceneManager.instance.EndNetProcess("PMsgGuildSetRoleLevelForJoinGuildC");
        Debug.Log("PMsgGuildSetRoleLevelForJoinGuildS" + pmsgGuildSetRoleLevelForJoinGuildS);
        uint ErrorCode = (uint)pmsgGuildSetRoleLevelForJoinGuildS.UnErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            //  UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGuildSetRoleLevelForJoinGuildS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }


        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null)
            (basePanel as GuildPanel).SetGuildJoinLevel(pmsgGuildSetRoleLevelForJoinGuildS.UnLevel);
    }

    /// <summary> 캐릭터 아이디로 길드정보 찾아옴 </summary>
    private void PMsgGuildIDQuerySHandler(PMsgGuildIDQueryS pmsgGuildIDQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgGuildIDQueryC");
        //Debug.Log("pmsgGuildIDQueryS" + pmsgGuildIDQueryS);

        UIBasePanel rankPanel = UIMgr.GetUIBasePanel("UIPanel/RankingPanel");
        if (rankPanel != null)
            (rankPanel as RankPanel).OnRankerGuildInfoList(pmsgGuildIDQueryS);
    }

    #endregion

}
