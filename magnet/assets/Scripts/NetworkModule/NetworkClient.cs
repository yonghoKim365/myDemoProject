using Core.Net;
using UnityEngine;
using System.Net;
using Network = Core.Net.Network;
using Protocol;
using System.Text;
using Sw;
using System.Collections.Generic;

public partial class NetworkClient : Immortal<NetworkClient>
{
    protected override string ImmortalHierarchyPath
    {
        get
        {
            return "Network";
        }
    }

    private Network mNetworkAuth;
    private Network mNetworkGame;

    System.Action<ConnectState> authConnectCallBack;
    System.Action<ConnectState> gameConnectCallBack;

    private int mapID;
    private int mapserverID;
    private int _regenX;
    private int _regenY;
    private int _serverIP;
    private int _serverPort;

    private bool _firstLogin;
    private ulong _guideID;

    public int GetSelectedServer()
    {
        return _serverIP;
    }

    public int GetMapID()
    {
        return mapID;
    }

    public int GetSelectedPort()
    {
        return _serverPort;
    }

    //로딩완료가 안되었을시 처리해줄 데이터
    //마을용
    public List<PMsgMapRoleInfoS> townUnitLoadingList = new List<PMsgMapRoleInfoS>();
    //멀티용
    public List<PMsgBattleMapRoleInfoS> beforeUnitLoadingList = new List<PMsgBattleMapRoleInfoS>();
    public List<PMsgBattleNpcInfoS> beforeNpcLoadingList = new List<PMsgBattleNpcInfoS>();

    public void GetRegenPos(out int regenX, out int regenY)
    {
        regenX = _regenX;
        regenY = _regenY;
    }

    public bool GetAuthConnection()
    {
        return mNetworkAuth.Connected;
    }

    public bool GetGameConnection()
    {
        return mNetworkGame.Connected;
    }

    public void ConnectServer(string ServerID, int port, System.Action<ConnectState> authCallBack)
    {
        authConnectCallBack = authCallBack;
        mNetworkAuth.Connect(ServerID, port);
    }

    public bool InitializeNetworkClient()
    {
        MsgTable.RegistAllMsg();

        mNetworkAuth = new Network(MsgTable.Pool, "Auth_Network");
        mNetworkAuth.EventConnect += AuthEventConnectHandler;
        mNetworkAuth.AddMsgListener<PMsgLoginS>(PMsgLoginSHandler);
        mNetworkAuth.AddMsgListener<PMsgServerListS>(PMsgServerListSHandler);
        mNetworkAuth.AddMsgListener<PMsgAccountCertifyS>(PMsgAccountCertifySHandler);

        //TypeSDK 관련
        mNetworkAuth.AddMsgListener<PMsgGoogleCertifyS>(PMsgGoogleCertifySHandler);
        mNetworkAuth.AddMsgListener<PMsgFacebookCertifyS>(PMsgFacebookCertifySHandler);

        mNetworkGame = new Network(MsgTable.Pool, "Game_Network");
        mNetworkGame.EventConnect += GameEventConnectHandler;
        mNetworkGame.AddMsgListener<PMsgGameLoginS>(PMsgGameLoginSHandler);
        mNetworkGame.AddMsgListener<PMsgUserInfoS>(PMsgUserInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGameDisconnectS>(PMsgGameDisconnectSHandler);

        //TypeSDK 관련
        mNetworkGame.AddMsgListener<PMsgUserBindGoogleS>(PMsgUserBindGoogleSHandler);
        mNetworkGame.AddMsgListener<PMsgUserBindFacebookS>(PMsgUserBindFacebookSHandler);
        mNetworkGame.AddMsgListener<PMsgUserBindQueryFbGoogleS>(PMsgUserBindQueryFbGoogleSHandler);

        mNetworkGame.AddMsgListener<PMsgTalkCS>(PMsgTalkCSHandler);
        mNetworkGame.AddMsgListener<PMsgTalkRecvS>(PMsgTalkRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgTalkBlackListS>(PMsgTalkBlackListSHandler);
        mNetworkGame.AddMsgListener<PMsgTalkDelBlackCS>(PMsgTalkDelBlackCSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleListS>(PMsgRoleListSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleCreateNewS>(PMsgRoleCreateNewSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleDeleteS>(PMsgRoleDeleteSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleSelectS>(PMsgRoleSelectSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleInfoS>(PMsgRoleInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgMapEnterMapS>(PMsgMapEnterMapSHander);
        mNetworkGame.AddMsgListener<PMsgMapLeaveMapS>(PMsgMapLeaveMapSHander);
        mNetworkGame.AddMsgListener<PMsgNpcInfoS>(PMsgNpcInfoSHander);
        mNetworkGame.AddMsgListener<PMsgMapRoleInfoS>(PMsgMapRoleInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgMapMoveCS>(PMsgMapMoveCSHanler);
        mNetworkGame.AddMsgListener<PMsgMapMoveRecvS>(PMsgMapMoveRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentQueryS>(PMsgEquipmentQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgItemQueryS>(PMsgItemQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentDelS>(PMsgEquipmentDelSHandler);
        //mNetworkGame.AddMsgListener<PMsgEquipmentUserS>(PMsgEquipmentUserSHanler);
        mNetworkGame.AddMsgListener<PMsgItemDelS>(PMsgItemDelSHandler);
        mNetworkGame.AddMsgListener<PMsgCostumeQueryS>(PMsgCostumeQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentEvolveS>(PMsgEquipmentEvolveSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleAttributeS>(PMsgRoleAttributeSHander);
        mNetworkGame.AddMsgListener<PMsgEquipmentEnchantS>(PMsgEquipmentEnchantSHandler);
        //mNetworkGame.AddMsgListener<PMsgEquipmentBreakS>(PMsgEquipmentBreakSHandler);
        mNetworkGame.AddMsgListener<PMsgItemFusionS>(PMsgItemFusionSHandler);
        mNetworkGame.AddMsgListener<PMsgCostumeFusionS>(PMsgCostumeFusionSHandler);
        mNetworkGame.AddMsgListener<PMsgCostumeEvolveS>(PMsgCostumeEvolveSHandler);
        mNetworkGame.AddMsgListener<PMsgCostumeUserS>(PMsgCostumeUserSHandler);
        mNetworkGame.AddMsgListener<PMsgCostumeTokenS>(PMsgCostumeTokenSHandler);
        //mNetworkGame.AddMsgListener<PMsgEquipmentSellS>(PMsgEquipmentSellSHandler);
        mNetworkGame.AddMsgListener<PMsgItemSellS>(PMsgItemSellSHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentEnchantTurboS>(PMsgEquipmentEnchantTurboSHandler);

        mNetworkGame.AddMsgListener<PMsgDailyTimeS>(PMsgDailyTimeSHandler);
        mNetworkGame.AddMsgListener<PMsgPowerTimeS>(PMsgPowerTimeSHandler);

        mNetworkGame.AddMsgListener<PMsgStageQueryS>(PMsgStageQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgStageStartS>(PMsgStageStartSHandler);

        mNetworkGame.AddMsgListener<PMsgStageCompleteS>(PMsgStageCompleteSHandler);

        //mNetworkGame.AddMsgListener<PMsgStageFlopS>(PMsgStageFlopSHandler);

        //코스튬 보이기 안보이기
        mNetworkGame.AddMsgListener<PMsgCostumeShowFlagS>(PMsgCostumeShowFlagSHandler);

        //소탕
        mNetworkGame.AddMsgListener<PMsgStageSweepS>(PMsgStageSweepSHandler);
        mNetworkGame.AddMsgListener<PMsgStageSweepResultS>(PMsgStageSweepResultSHendler);

        //코스튬 레벨업
        mNetworkGame.AddMsgListener<PMsgCostumeSkillUpgradeS>(PMsgCostumeSkillUpgradeSHandler);

        mNetworkGame.AddMsgListener<PMsgReturnMainMapS>(PMsgReturnMainMapSHandler);

        //파트너
        mNetworkGame.AddMsgListener<PMsgHeroQueryS>(PMsgHeroQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgHeroFusionS>(PMsgHeroFusionSHandler);
        mNetworkGame.AddMsgListener<PMsgHeroSkillUpgradeS>(PMsgHeroSkillUpgradeSHandler);
        mNetworkGame.AddMsgListener<PMsgHeroEvolveS>(PMsgHeroEvolveSHandler);

        mNetworkGame.AddMsgListener<PMsgPingCS>(PMsgPingCSHandler);

        // 메일
        mNetworkGame.AddMsgListener<PMsgEmailQueryListS>(PMsgEmailQueryListSHandler);
        mNetworkGame.AddMsgListener<PMsgEmailReadDetailS>(PMsgEmailReadDetailSHandler);
        mNetworkGame.AddMsgListener<PMsgEmailFeatchS>(PMsgEmailFeatchSHandler);
        mNetworkGame.AddMsgListener<PMsgEmailOnKeyFeatchS>(PMsgEmailOnKeyFeatchSHandler);
        mNetworkGame.AddMsgListener<PMsgEmailDelS>(PMsgEmailDelSHandler);
        mNetworkGame.AddMsgListener<PMsgEmailOnKeyDelS>(PMsgEmailOnKeyDelSHandler);

        //친구
        mNetworkGame.AddMsgListener<PMsgFriendQueryListS>(PMsgFriendQueryListSHandler);
        //mNetworkGame.AddMsgListener<PMsgFriendFullInfoS>(PMsgFriendFullInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendSearchS>(PMsgFriendSearchSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendRecommendListS>(PMsgFriendRecommendListSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendAddS>(PMsgFriendAddSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendNotifyAddS>(PMsgFriendNotifyAddSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendCancleAddS>(PMsgFriendCancleAddSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendNotifyCancleAddS>(PMsgFriendNotifyCancleAddSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendRequestFriendListS>(PMsgFriendRequestFriendListSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendSelfRequestFriendListS>(PMsgFriendSelfRequestFriendListSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendApplicantS>(PMsgFriendApplicantSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendNotifyApplicantS>(PMsgFriendNotifyApplicantSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendDelFriendS>(PMsgFriendDelFriendSHandler);
        //mNetworkGame.AddMsgListener<PMsgFriendRequestInvalidS>(PMsgFriendRequestInvalidSHandler);
        //mNetworkGame.AddMsgListener<PMsgFriendSelfRequestInvalidS>(PMsgFriendSelfRequestInvalidSHandler);
        mNetworkGame.AddMsgListener<PMsgFriendGivePowerS>(PMsgFriendGivePowerSHandler);


        //경험치&골드던전
        mNetworkGame.AddMsgListener<PMsgExpBattleQueryS>(PMsgExpBattleQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgExpBattleStartS>(PMsgExpBattleStartSHandler);
        mNetworkGame.AddMsgListener<PMsgExpBattleCompleteS>(PMsgExpBattleCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgCoinBattleQueryS>(PMsgCoinBattleQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgCoinBattleStartS>(PMsgCoinBattleStartSHandler);
        mNetworkGame.AddMsgListener<PMsgCoinBattleCompleteS>(PMsgCoinBattleCompleteSHandler);

        //스테미너 충전 치트키
        mNetworkGame.AddMsgListener<PMsgBuyPowerS>(PMsgBuyPowerSHandler);

        //마계의탑
        mNetworkGame.AddMsgListener<PMsgTowerBattleQueryS>(PMsgTowerBattleQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgTowerBattleStartS>(PMsgTowerBattleStartSHandler);
        mNetworkGame.AddMsgListener<PMsgTowerRankQueryS>(PMsgTowerRankQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgTowerBattleCompleteS>(PMsgTowerBattleCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgTowerUseTimeQueryS>(PMsgTowerUseTimeQuerySHandler);

        //레이드
        mNetworkGame.AddMsgListener<PMsgBossBattleQueryS>(PMsgBossBattleQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgBossBattleStartS>(PMsgBossBattleStartSHandler);
        mNetworkGame.AddMsgListener<PMsgBossBattleCompleteS>(PMsgBossBattleCompleteSHandler);

        //상점
        mNetworkGame.AddMsgListener<PMsgShopInfoQueryS>(PMsgShopInfoQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgShopByItemS>(PMsgShopByItemSHandler);
        mNetworkGame.AddMsgListener<PMsgShopRefreshS>(PMsgShopRefreshSHandler);

        //vip
        mNetworkGame.AddMsgListener<PMsgVipQueryInfoS>(PMsgVipQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgVipFetchSweepTicketS>(PMsgVipFetchSweepTicketSHandler);
        mNetworkGame.AddMsgListener<PMsgVipRepairSignInS>(PMsgVipRepairSignInSHandler);

        //퀘스트
        mNetworkGame.AddMsgListener<PMsgTaskQueryInfoS>(PMsgTaskQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgTaskReceiveTaskS>(PMsgTaskReceiveTaskSHandler);
        mNetworkGame.AddMsgListener<PMsgTaskCompleteS>(PMsgTaskCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgTaskFetchBonusS>(PMsgTaskFetchBonusSHandler);
        mNetworkGame.AddMsgListener<PMsgDailyTaskQueryInfoS>(PMsgDailyTaskQueryInfoSHandler);

        //랭킹
        mNetworkGame.AddMsgListener<PMsgRankQueryS>(PMsgRankQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgRankGuildQueryS>(PMsgRankGuildQuerySHandler);

        //뽑기
        mNetworkGame.AddMsgListener<PMsgLotteryQueryInfoS>(PMsgLotteryQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxCommonFreeS>(PMsgLotteryBoxCommonFreeSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxCommonS>(PMsgLotteryBoxCommonSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxCommonManytimesS>(PMsgLotteryBoxCommonManytimesSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxSeniorFreeS>(PMsgLotteryBoxSeniorFreeSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxSeniorS>(PMsgLotteryBoxSeniorSHandler);
        mNetworkGame.AddMsgListener<PMsgLotteryBoxSeniorManytimesS>(PMsgLotteryBoxSeniorManytimesSHandler);

        //난투장
        mNetworkGame.AddMsgListener<PMsgMessQueryS>(PMsgMessQuerySHandler);
        //mNetworkGame.AddMsgListener<PMsgMessRoomQueryS>(PMsgMessRoomQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgMessRoomEnterS>(PMsgMessRoomEnterSHandler);
        mNetworkGame.AddMsgListener<PMsgMessRoomLeaveS>(PMsgMessRoomLeaveSHandler);
		mNetworkGame.AddMsgListener<PMsgMessDropS>(PMsgMessDropSHandler);
		mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossBeginS>(PMsgMessSynNotifyBossBeginSHandler);
		mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossEndS>(PMsgMessSynNotifyBossEndSHandler);
		mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossBeginOrEndS>(PMsgMessSynNotifyBossBeginOrEndSHandler);

		//mNetworkGame.AddMsgListener<PMsgMessChangeRoomS>(PMsgMessChangeRoomSHandler);
		mNetworkGame.AddMsgListener<PMsgMessBossInfoS>(PMsgMessBossInfoSHandler);

        mNetworkGame.AddMsgListener<PMsgBattleMapEnterMapS>(PMsgBattleMapEnterMapSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapLeaveMapS>(PMsgBattleMapLeaveMapSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapRoleInfoS>(PMsgBattleMapRoleInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapMoveCS>(PMsgBattleMapMoveCSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapMoveRecvS>(PMsgBattleMapMoveRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleNpcInfoS>(PMsgBattleNpcInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleAttackS>(PMsgRoleAttackSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleAttackPrepareS>(PMsgBattleAttackPrepareSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleAttackPrepareRecvS>(PMsgBattleAttackPrepareRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapKickS>(PMsgBattleMapKickSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleAttackRecvS>(PMsgRoleAttackRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleDieRecvS>(PMsgRoleDieRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleReliveRecvS>(PMsgRoleReliveRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleMapFlyS>(PMsgBattleMapFlySHandler);
        
        mNetworkGame.AddMsgListener<PMsgRoleReliveS>(PMsgRoleReliveSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleSuperArmorRecoveryS>(PMsgRoleSuperArmorRecoverySHandler);

        //난투장 신규 발사체
        mNetworkGame.AddMsgListener<PMsgBattleProjectTileInfoS>(PMsgBattleProjectTileInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleAddProjectTileS>(PMsgBattleAddProjectTileSHandler);
        mNetworkGame.AddMsgListener<PMsgBattleDelProjectTileS>(PMsgBattleDelProjectTileSHandler);

        //멀티플레이 버프
        mNetworkGame.AddMsgListener<PMsgBuffAttackRecvS>(PMsgBuffAttackRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgAddBuffS>(PMsgAddBuffSHandler);

        //길드
        mNetworkGame.AddMsgListener<PMsgGuildCreateNewS>(PMsgGuildCreateNewSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQueryBaseInfoS>(PMsgGuildQueryBaseInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQueryDetailedInfoS>(PMsgGuildQueryDetailedInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildMemberListS>(PMsgGuildMemberListSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildRecommendListS>(PMsgGuildRecommendListSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildChangeIconS>(PMsgGuildChangeIconSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildChangeNameDeclarationAnnouncementS>(PMsgGuildChangeNameDeclarationAnnouncementSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildSearchGuildS>(PMsgGuildSearchGuildSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildApplyGuildS>(PMsgGuildApplyGuildSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQueryApplyListS>(PMsgGuildQueryApplyListSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQueryGuildListS>(PMsgGuildQueryGuildListSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildExamineApplicantS>(PMsgGuildExamineApplicantSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildSynExamineApplicantS>(PMsgGuildSynExamineApplicantSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildUpgradeLevelS>(PMsgGuildUpgradeLevelSHandler);
        mNetworkGame.AddMsgListener<PMsgSetGuildJoinsetS>(PMsgSetGuildJoinsetSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildGuildPrayS>(PMsgGuildGuildPraySHandler);
        mNetworkGame.AddMsgListener<PMsgGuildGuildDonateS>(PMsgGuildGuildDonateSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildAppointPositionS>(PMsgGuildAppointPositionSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildKitkMemberS>(PMsgGuildKitkMemberSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildSynKitkMemberS>(PMsgGuildSynKitkMemberSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildAppointGuildLeaderS>(PMsgGuildAppointGuildLeaderSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildSecedeGuildS>(PMsgGuildSecedeGuildSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildAttributeS>(PMsgGuildAttributeSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQueryGuildStatusS>(PMsgGuildQueryGuildStatusSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildQuerySelfInfoS>(PMsgGuildQuerySelfInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildNameQueryS>(PMsgGuildNameQuerySHandler);
        //mNetworkGame.AddMsgListener<PMsgGuildFullGoddsInfoOnBodyByApplicantS>(PMsgGuildFullGoddsInfoOnBodyByApplicantSHandler);
        //mNetworkGame.AddMsgListener<PMsgGuildFullGoddsInfoOnBodyS>(PMsgGuildFullGoddsInfoOnBodySHandler);
        mNetworkGame.AddMsgListener<PMsgGuildIDQueryS>(PMsgGuildIDQuerySHandler);

        //스테이지 별 보상
        mNetworkGame.AddMsgListener<PMsgStageChapterQueryS>(PMsgStageChapterQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgStageChapterRewardS>(PMsgStageChapterRewardSHandler);

        //칭호
        mNetworkGame.AddMsgListener<PMsgTitleQueryInfoS>(PMsgTitleQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgUseTitleS>(PMsgUseTitleSHandler);
        mNetworkGame.AddMsgListener<PMsgSynNewTitleS>(PMsgSynNewTitleSHandler);
        //출첵
        mNetworkGame.AddMsgListener<PMsgSignInQueryInfoS>(PMsgSignInQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgSignInS>(PMsgSignInSHandler);
        mNetworkGame.AddMsgListener<PMsgFillInSignInS>(PMsgFillInSignInSHandler);

        //업적
        mNetworkGame.AddMsgListener<PMsgAchieveQueryInfoS>(PMsgAchieveQueryInfoSHandler);
        //mNetworkGame.AddMsgListener<PMsgAchieveEquipTotalQueryInfoS>(PMsgAchieveEquipTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveFightTotalQueryInfoS>(PMsgAchieveFightTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveFriendTotalQueryInfoS>(PMsgAchieveFriendTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveMoneyTotalQueryInfoS>(PMsgAchieveMoneyTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchievePlayTotalQueryInfoS>(PMsgAchievePlayTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveRoleTotalQueryInfoS>(PMsgAchieveRoleTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveVipTotalQueryInfoS>(PMsgAchieveVipTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveFetchAwardS>(PMsgAchieveFetchAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveFetchPointsAwardS>(PMsgAchieveFetchPointsAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveSynPointsTotalValueS>(PMsgAchieveSynPointsTotalValueSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveSynAchieveStatisValueS>(PMsgAchieveSynAchieveStatisValueSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveSynAchieveCompleteS>(PMsgAchieveSynAchieveCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveOneKeyFetchAchieveAwardS>(PMsgAchieveOneKeyFetchAchieveAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveSynFightDataTotalValueS>(PMsgAchieveSynFightDataTotalValueSHandler);


        //콜로세움
        mNetworkGame.AddMsgListener<PMsgColosseumQueryS>(PMsgColosseumQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumCreateRoomS>(PMsgColosseumCreateRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumInviteS>(PMsgColosseumInviteSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumInviteRecvS>(PMsgColosseumInviteRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumRoomInfoS>(PMsgColosseumRoomInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumEnterRoomRecvS>(PMsgColosseumEnterRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumEnterRoomS>(PMsgColosseumEnterRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumLeaveRoomS>(PMsgColosseumLeaveRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumLeaveRoomRecvS>(PMsgColosseumLeaveRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumKickRoomS>(PMsgColosseumKickRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumKickRoomRecvS>(PMsgColosseumKickRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumStartS>(PMsgColosseumStartSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumCompleteS>(PMsgColosseumCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumSweepS>(PMsgColosseumSweepSHandler);
        mNetworkGame.AddMsgListener<PMsgColosseumSweepResultS>(PMsgColosseumSweepResultS);

        //7일연속출석보상
        mNetworkGame.AddMsgListener<PMsgWelfareXDayLoginQueryInfoS>(PMsgWelfareXDayLoginQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareFetchXDayRewardS>(PMsgWelfareFetchXDayRewardSHandler);
        //복귀패키지
        mNetworkGame.AddMsgListener<PMsgWelfareReturnQueryInfoS>(PMsgWelfareReturnFetchRewardCHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareReturnFetchRewardS>(PMsgWelfareReturnFetchRewardSHandler);
        //신썹보상
        mNetworkGame.AddMsgListener<PMsgWelfareOpenSvrQueryInfoS>(PMsgWelfareOpenSvrQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareOpenSvrFetchRewardS>(PMsgWelfareOpenSvrFetchRewardSHandler);
        // 로그인유지보상 
        mNetworkGame.AddMsgListener<PMsgWelfareOnlineQueryInfoS>(PMsgWelfareOnlineQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareOnlineFetchRewardS>(PMsgWelfareOnlineFetchRewardSHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareOnlineSynCanFetchS>(PMsgWelfareOnlineSynCanFetchSHandler);
        //랩업패키지보상 
        mNetworkGame.AddMsgListener<PMsgWelfareRoleUpgradeQueryInfoS>(PMsgWelfareRoleUpgradeQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgWelfareFetchRoleUpgradeRewardS>(PMsgWelfareFetchRoleUpgradeRewardSHandler);

        //멀티 보스레이드
        mNetworkGame.AddMsgListener<PMsgMultiBossQueryS>(PMsgMultiBossQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossCreateRoomS>(PMsgMultiBossCreateRoomSHandler);

        mNetworkGame.AddMsgListener<PMsgMultiBossInviteS>(PMsgMultiBossInviteSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossInviteRecvS>(PMsgMultiBossInviteRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossRoomInfoS>(PMsgMultiBossRoomInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossEnterRoomS>(PMsgMultiBossEnterRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossEnterRoomRecvS>(PMsgMultiBossEnterRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossLeaveRoomS>(PMsgMultiBossLeaveRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossLeaveRoomRecvS>(PMsgMultiBossLeaveRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossKickRoomS>(PMsgMultiBossKickRoomSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossKickRoomRecvS>(PMsgMultiBossKickRoomRecvSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossStartS>(PMsgMultiBossStartSHandler);
        mNetworkGame.AddMsgListener<PMsgMultiBossCompleteS>(PMsgMultiBossCompleteSHandler);


        //누적충전보상
        mNetworkGame.AddMsgListener<PMsgRechargeTotalQueryInfoS>(PMsgRechargeTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeTotalFetchRewardS>(PMsgRechargeTotalFetchRewardSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeTotalSynCanFetchS>(PMsgRechargeTotalSynCanFetchSHandler);
        //일간충전 보상
        mNetworkGame.AddMsgListener<PMsgRechargeDailyQueryInfoS>(PMsgRechargeDailyQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeDailyFetchRewardS>(PMsgRechargeDailyFetchRewardSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeDailySynCanFetchS>(PMsgRechargeDailySynCanFetchSHandler);
        //누적소비보상
        mNetworkGame.AddMsgListener<PMsgRechargeConsumerQueryInfoS>(PMsgRechargeConsumerQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeConsumerFetchRewardS>(PMsgRechargeConsumerFetchRewardSHandler);
        mNetworkGame.AddMsgListener<PMsgRechargeConsumerSynCanFetchS>(PMsgRechargeConsumerSynCanFetchSHandler);

        //길드퀘스트
        mNetworkGame.AddMsgListener<PMsgGuildTaskQueryInfoS>(PMsgGuildTaskQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildTaskFetchBonusS>(PMsgGuildTaskFetchBonusSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildTaskSynTaskCompleteS>(PMsgGuildTaskSynTaskCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgFetchRewardGuildUserTaskQueryInfoS>(PMsgFetchRewardGuildUserTaskQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildUserTaskQueryInfoS>(PMsgGuildUserTaskQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildUserTaskFetchBonusS>(PMsgGuildUserTaskFetchBonusSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildUserTaskAllocatingTaskS>(PMsgGuildUserTaskAllocatingTaskSHandler);
        mNetworkGame.AddMsgListener<PMsgGuildUserTaskSynCompleteS>(PMsgGuildUserTaskSynCompleteSHandler);

        //유저 정보 확인
        mNetworkGame.AddMsgListener<PMsgQueryRoleInfoS>(PMsgQueryRoleInfoSHandler);

        //유저 실명인증
        mNetworkGame.AddMsgListener<PMsgUserCertifyInfoS>(PMsgUserCertifyInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgUserCertifySetS>(PMsgUserCertifySetSHandler);

        ////활동량
        //mNetworkGame.AddMsgListener<PMsgActivePointsQueryInfoS>(PMsgActivePointsQueryInfoSHandler);
        //mNetworkGame.AddMsgListener<PMsgActivePointsFetchRewardS>(PMsgActivePointsFetchRewardSHandler);
        //mNetworkGame.AddMsgListener<PMsgActivePointsTotalCountQueryInfoS>(PMsgActivePointsTotalCountQueryInfoSHandler);

        //차관
        mNetworkGame.AddMsgListener<PMsgArenaInfoS>(PMsgArenaInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaFightListS>(PMsgArenaFightListSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaFightResultNoticeS>(PMsgArenaFightResultNoticeSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaResetTimesS>(PMsgArenaResetTimesSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaRankInfoS>(PMsgArenaRankInfoSHandler);
        //mNetworkGame.AddMsgListener<PMsgArenaGetRankAwardS>(PMsgArenaGetRankAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaRankListS>(PMsgArenaRankListSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaMatchListS>(PMsgArenaMatchListSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaMatchInfoS>(PMsgArenaMatchInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaFightStartS>(PMsgArenaFightStartSHandler);
        //mNetworkGame.AddMsgListener<PMsgArenaFightLeaveS>(PMsgArenaFightLeaveSHandler);
        mNetworkGame.AddMsgListener<PMsgArenaFightCompleteS>(PMsgArenaFightCompleteSHandler);


		// Bulletine, notice
		mNetworkGame.AddMsgListener<PMsgBulletinSynNewInfoS>(PMsgBulletineCompleteSHandler);

        //자랑하기
        mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinEvolveHeroS>(PMsgSynDynamicBulletinEvolveHeroSHandler);
        mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinNewHeroS>(PMsgSynDynamicBulletinNewHeroSHandler);
        mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinEvolveEquipmentS>(PMsgSynDynamicBulletinEvolveEquipmentSHandler);
        mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinEnchantEquipmentS>(PMsgSynDynamicBulletinEnchantEquipmentSHandler);
        mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinNewEquipmentS>(PMsgSynDynamicBulletinNewEquipmentSHandler);
        //mNetworkGame.AddMsgListener<PMsgSynDynamicBulletinEnchantHeroS>(PMsgSynDynamicBulletinEnchantHeroSHandler);
        //mNetworkGame.AddMsgListener<PMsgDynamicBulletinSynInfoS>(PMsgDynamicBulletinSynInfoSHandler);

        //신규아이템?
        mNetworkGame.AddMsgListener<PMsgSynNewItemS>(PMsgPMsgSynNewItemSHandler);

        //셋트장비
        mNetworkGame.AddMsgListener<PMsgEquipmentSetQueryS>(PMsgEquipmentSetQuerySHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentSetChangeS>(PMsgEquipmentSetChangeSHandler);
        mNetworkGame.AddMsgListener<PMsgEquipmentSetSelectS>(PMsgEquipmentSetSelectSHandler);

        //일일업적
        mNetworkGame.AddMsgListener<PMsgAchieveDailyQueryInfoS>(PMsgAchieveDailyQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyFightTotalQueryInfoS>(PMsgAchieveDailyFightTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyFriendTotalQueryInfoS>(PMsgAchieveDailyFriendTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyMoneyTotalQueryInfoS>(PMsgAchieveDailyMoneyTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyPlayTotalQueryInfoS>(PMsgAchieveDailyPlayTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyRoleTotalQueryInfoS>(PMsgAchieveDailyRoleTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyVipTotalQueryInfoS>(PMsgAchieveDailyVipTotalQueryInfoSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyFetchAwardS>(PMsgAchieveDailyFetchAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyFetchPointsAwardS>(PMsgAchieveDailyFetchPointsAwardSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailySynPointsTotalValueS>(PMsgAchieveDailySynPointsTotalValueSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailySynAchieveStatisValueS>(PMsgAchieveDailySynAchieveStatisValueSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailySynFightDataTotalValueS>(PMsgAchieveDailySynFightDataTotalValueSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailySynAchieveCompleteS>(PMsgAchieveDailySynAchieveCompleteSHandler);
        mNetworkGame.AddMsgListener<PMsgAchieveDailyOneKeyFetchAchieveAwardS>(PMsgAchieveDailyOneKeyFetchAchieveAwardSHandler);

        //신분
        mNetworkGame.AddMsgListener<PMsgRoleIdentifyListS>(PMsgRoleIdentifyListSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleIdentifyUnlockS>(PMsgRoleIdentifyUnlockSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleIdentifyUpgradeS>(PMsgRoleIdentifyUpgradeSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleIdentifyUseS>(PMsgRoleIdentifyUseSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleIdentifyUnlockedListS>(PMsgRoleIdentifyUnlockedListSHandler);
        //스킬
        mNetworkGame.AddMsgListener<PMsgRoleActiveSkillListS>(PMsgRoleActiveSkillListSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleActiveSkillUpgradeS>(PMsgRoleActiveSkillUpgradeSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleActiveSkillUseS>(PMsgRoleActiveSkillUseSHandler);
        mNetworkGame.AddMsgListener<PMsgRolePassiveSkillUpgradeTurboS>(PMsgRolePassiveSkillUpgradeTurboSHandler);
        //패시브
        mNetworkGame.AddMsgListener<PMsgRolePassiveSkillListS>(PMsgRolePassiveSkillListSHandler);
        mNetworkGame.AddMsgListener<PMsgRolePassiveSkillUpgradeS>(PMsgRolePassiveSkillUpgradeSHandler);
        mNetworkGame.AddMsgListener<PMsgRolePassiveSkillUseS>(PMsgRolePassiveSkillUseSHandler);
        mNetworkGame.AddMsgListener<PMsgRoleActiveSkillUpgradeTurboS>(PMsgRoleActiveSkillUpgradeTurboSHandler);

        return true;
    }

    public void ConnectGameServer(string ServerID, int port, System.Action<ConnectState> gameCallBack)
    {
        gameConnectCallBack = gameCallBack;
        _firstLogin = true;
        mNetworkGame.Connect(ServerID, port);
    }

    void OnDestroy()
    {
        mNetworkAuth.Dispose();
        mNetworkGame.Dispose();
    }

    public void DisconnectAuthServer()
    {
        mNetworkAuth.Close();
    }

    public void DisconnectGameServer()
    {
        mNetworkGame.Close();
    }

    private void AuthEventConnectHandler(Network nw, ConnectState state)
    {
        Debug.Log("A Auth Connect state:" + state + " " + mNetworkAuth.Disposed + " " + mNetworkGame.Disposed);
        switch (state)
        {
            case ConnectState.Success:
                break;

            case ConnectState.Error:
                break;

            case ConnectState.Close:
                break;
        }

        authConnectCallBack(state);
    }

    private void GameEventConnectHandler(Network nw, ConnectState state)
    {
        Debug.Log("A Game Connect state:" + state);
        switch (state)
        {
            case ConnectState.Success:
                break;

            case ConnectState.Error:
                break;

            case ConnectState.Close:
                break;
        }

        gameConnectCallBack(state);
    }

    #region Request

    //LoginServer
    public bool SendPMsgLoginC(long ullUserId, int SignedKey, int unServerId)
    {
        if (!GetAuthConnection())
            return false;

        var sendMsg = new PMsgLoginC();
        sendMsg.UllUserId = ullUserId;
        sendMsg.UnSignedKey = SignedKey;
        sendMsg.UnServerId = unServerId;
        mNetworkAuth.SendMsg(MSG_DEFINE._MSG_LOGIN_C, sendMsg);

        return true;
    }

    public bool SendPMsgServerListC()
    {
        if (!GetAuthConnection())
            return false;

        var sendMsg = new PMsgServerListC();
        mNetworkAuth.SendMsg(MSG_DEFINE._MSG_SERVER_LIST_C, sendMsg);

        return true;
    }

    public bool SendPMsgGoogleCertifyC(string szGoogleAccount, string szToken)
    {
        if (!GetAuthConnection())
            return false;

        var sendMsg = new PMsgGoogleCertifyC();
        sendMsg.SzGoogleAccount = szGoogleAccount;
        sendMsg.SzToken = szToken;
        mNetworkAuth.SendMsg(MSG_DEFINE._MSG_GOOGLE_CERTIFY_C, sendMsg);

        return true;
    }

    public bool SendPMsgFacebookCertifyC(string szFacebookAccount, string szToken)
    {
        if (!GetAuthConnection())
            return false;

        var sendMsg = new PMsgFacebookCertifyC();
        sendMsg.SzFacebookAccount = szFacebookAccount;
        sendMsg.SzToken = szToken;
        mNetworkAuth.SendMsg(MSG_DEFINE._MSG_FACEBOOK_CERTIFY_C, sendMsg);

        return true;
    }

    // GameServer
    public bool SendPMsgGameLoginC(long ullUserId, int unDevice, int unVersion, int unCode, int unLanguage, int unRelogin)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgGameLoginC();
        sendMsg.UllUserId = ullUserId;
        sendMsg.UnDevice = unDevice;
        sendMsg.UnVersion = unVersion;
        sendMsg.UnCode = unCode;
        sendMsg.UnLanguage = unLanguage;
        sendMsg.UnRelogin = unRelogin;
        sendMsg.UnReserved1 = 0;
        sendMsg.UnReserved2 = 0;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_GAME_USER_LOGIN_C, sendMsg);

        return true;
    }

    public bool SendPMsgTalkCS(int unChannel, long ullDestID, string szDestName, string szMsg, int unResult)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTalkCS();

        sendMsg.UnChannel = unChannel;
        sendMsg.UllDestID = ullDestID;
        sendMsg.SzDestName = szDestName;
        sendMsg.SzMsg = szMsg;
        sendMsg.UnResult = unResult;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TALK_CS, sendMsg);

        return true;
    }

    public bool SendPMsgTalkBlackListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTalkBlackListC();

        sendMsg.UnReserved1 = 0;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TALK_BLACK_LIST_C, sendMsg);

        return true;
    }

    public bool SendPMsgTalkAddBlackCS(long ullDestID, int unResult)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTalkAddBlackCS();

        sendMsg.UllDestID = ullDestID;
        sendMsg.UnResult = unResult;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TALK_ADD_BLACK_CS, sendMsg);

        return true;
    }

    public bool SendPMsgTalkDelBlackCS(long ullDestID, int unResult)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTalkDelBlackCS();

        sendMsg.UllDestID = ullDestID;
        sendMsg.UnResult = unResult;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TALK_DEL_BLACK_CS, sendMsg);
        return true;
    }

    public bool SendPMsgPingCS(long ull64Timer)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgPingCS();

        sendMsg.Ull64Timer = ull64Timer;
		Debug.Log ("sendMsg:" + sendMsg);
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_PING, sendMsg);
        return true;
    }

    public bool SendPMsgRoleCreateNewC(string szName, int unType, int unPos)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRoleCreateNewC();

        sendMsg.SzName = szName;
        sendMsg.UnType = unType;
        sendMsg.UnPos = unPos;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_CREATE_NEW_C, sendMsg);

        return true;
    }

    public bool SendPMsgRoleDeleteC(long ullRoleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRoleDeleteC();

        sendMsg.UllRoleId = ullRoleId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_DELETE_C, sendMsg);

        return true;
    }

    public bool SendPMsgRoleSelectC(long ullRoleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRoleSelectC();

        sendMsg.UllRoleId = ullRoleId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_SELECT_C, sendMsg);

        return true;
    }

    public bool SendPMsgMapMoveCS(int posX, int posY, float realX, float realY)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMapMoveCS();

        sendMsg.UnMapId = mapID;

        var mapPos = new MapPos();
        mapPos.UnPosX = posX;
        mapPos.UnPosY = posY;
        mapPos.FData1 = realX;
        mapPos.FData2 = realY;

        sendMsg.CMapPos.Add(mapPos);
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MAP_MOVE_CS, sendMsg);

        return true;
    }

    public bool SendPMsgMapMoveCS(List<PathVertex> movePath, float realX, float realY)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMapMoveCS();

        sendMsg.UnMapId = mapID;

        //시작점은 빼자
        for (int i = 1; i < movePath.Count; i++)
        {
            var mapPos = new MapPos();
            mapPos.UnPosX = (int)movePath[i].myTilePos.x;
            mapPos.UnPosY = (int)movePath[i].myTilePos.y;

            mapPos.FData1 = realX;
            mapPos.FData2 = realY;

            sendMsg.CMapPos.Add(mapPos);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MAP_MOVE_CS, sendMsg);

        return true;
    }

    public bool SendPMsgMapMoveCSArray(MapPos[] pos)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMapMoveCS();

        sendMsg.UnMapId = mapID;

        for (int i = 0; i < pos.Length; i++)
        {
            //var mapPos = MapPos();
            //mapPos.UnPosX = x[i];
            //mapPos.UnPosY = y[i];

            sendMsg.CMapPos.Add(pos[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MAP_MOVE_CS, sendMsg);

        return true;
    }

    public bool SendPMsgEquipmentQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgEquipmentQueryC");
        var sendMsg = new PMsgEquipmentQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_QUERY_C, sendMsg);

        return true;
    }

    public bool SendPMsgItemQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgItemQueryC");
        var sendMsg = new PMsgItemQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ITEM_QUERY_C, sendMsg);

        return true;
    }

    public bool SendPMsgEquipmentUserC(int ID, int Type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEquipmentUserC();
        sendMsg.UnId = ID;
        sendMsg.UnType = Type;
        sendMsg.UnRoleFlag = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_USE_C, sendMsg);

        return true;
    }

    public bool SendPMsgCostumeQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgCostumeQueryC");
        var sendMsg = new PMsgCostumeQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_QUERY_C, sendMsg);

        return true;
    }

    /// <summary> 아이템 승급 </summary>
    public bool SendPMsgEquipmentEvolveC(int ID, int itemTypeID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEquipmentEvolveC();
        sendMsg.UnId = ID;
        sendMsg.UnType = itemTypeID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_EVOLVE_C, sendMsg);

        return true;
    }

    /// <summary> 아이템 강화 </summary>
    public bool SendPMsgEquipmentEnchantC(ulong itemUID, uint equipLowId)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("EquipEnchant");
        var sendMsg = new PMsgEquipmentEnchantC();
        sendMsg.UnId = (int)itemUID;
        sendMsg.UnType = (int)equipLowId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_C, sendMsg);

        return true;
    }
    /*
    /// <summary> 아이템 분해(장비) </summary>
    public bool SendPMsgEquipmentBreakC(List<int> IDs, int itemTypeID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEquipmentBreakC();
        int count = IDs.Count;
        for (int i = 0; i < count; i++)
        {
            sendMsg.UnId.Add(IDs[i]);
        }
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_BREAK_C, sendMsg);

        return true;
    }
    */
    /// <summary> 아이템 합성 </summary>
    public bool SendPMsgItemFusionC(int Type, int Num)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgItemFusionC();
        sendMsg.UnType = Type;
        sendMsg.UnNum = Num;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ITEM_FUSION_C, sendMsg);

        return true;
    }

    /// <summary> 코스튬 획득 </summary>
    public bool SendPMsgCostumeFusionC(int Type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCostumeFusionC();
        sendMsg.UnType = Type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_FUSION_C, sendMsg);

        return true;
    }

    /// <summary> 코스튬 승급 </summary>
    public bool SendPMsgCostumeEvolveC(int Id, int Type)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("CostumeEvolve");
        var sendMsg = new PMsgCostumeEvolveC();
        sendMsg.UnId = Id;
        sendMsg.UnType = Type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_EVOLVE_C, sendMsg);

        return true;
    }

    /// <summary> 코스튬 장착 </summary>
    public bool SendPMsgCostumeUserC(int Id, int Type)
    {
        if (!GetGameConnection())
            return false;

        NetData._CostumeData costume = NetData.instance.GetUserInfo().GetEquipCostume();
        if (costume == null)
            return false;

        if ((int)costume._costumeIndex == Id)//다르면 장착
            return false;

        var sendMsg = new PMsgCostumeUserC();
        sendMsg.UnId = Id;
        sendMsg.UnType = Type;
        sendMsg.UnRoleFlag = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_USE_C, sendMsg);

        return true;
    }

    /// <summary> 코스튬 보석 삽입 </summary>
    public bool SendPMsgCostumeTokenC(int Id, int Type, int TokenID, int Position)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCostumeTokenC();
        sendMsg.UnId = Id;
        sendMsg.UnType = Type;
        sendMsg.UnTokenItemId = TokenID;
        sendMsg.UnPosition = Position;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_TOKEN_C, sendMsg);

        return true;
    }
    /*
    /// <summary> 장비 아이템 판매 </summary>
    public bool SendPMsgEquipmentSellC(List<ulong> SellItemList)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEquipmentSellC();

        for (int i = 0; i < SellItemList.Count; i++)
        {
            var arg = new EquipmentSellInfo();
            arg.UnId = (int)SellItemList[i];
            sendMsg.CInfo.Add(arg);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_SELL_C, sendMsg);

        return true;
    }
    */
    /// <summary> 소비 아이템 판매 </summary>
    public bool SendPMsgItemSellC(uint itemLowID, int num)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgItemSellC();

        var arg = new ItemSellInfo();
        arg.UnType = (int)itemLowID;
        arg.UnNum = num;
        sendMsg.CInfo.Add(arg);

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ITEM_SELL_C, sendMsg);

        return true;
    }

    /// <summary> 싱글 스테이지 정보 받음 </summary>
    public bool SendPMsgStageQueryC(int StageType)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgStageQueryC");
        var sendMsg = new PMsgStageQueryC();
        sendMsg.UnStageType = StageType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_QUERY_C, sendMsg);

        return true;
    }

    /// <summary> 싱글스테이지 시작 </summary>
    public bool SendPMsgStageStartC(int unStageId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgStageStartC();
        sendMsg.UnStageId = unStageId;

        ulong[] partnerIDList = NetData.instance.GetUserInfo().GetEquipPartnersIdx();
        for (int i = 0; i < partnerIDList.Length; i++)
        {
            if (0 < partnerIDList[i])
                sendMsg.UnHeroId.Add((int)partnerIDList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_START_C, sendMsg);

        return true;
    }

    public bool SendPMsgStageCompleteC(int unStageId, int unClear, int[] unStarList)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgStageCompleteC();
        sendMsg.UnStageId = unStageId;
        sendMsg.UnVictory = unClear;
        for (int i = 0; i < unStarList.Length; i++)
        {
            sendMsg.UnStar.Add(unStarList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_COMPLETE_C, sendMsg);
        SceneManager.instance.ShowNetProcess("ClearSingle");
        return true;
    }
    /*
    /// <summary> 싱글게임 보상카드 오픈. </summary>
    public bool SendPMsgStageFlopC(uint stageId, bool isCash, int cardArr)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgStageFlopC();
        sendMsg.UnStageId = (int)stageId;//던전 아이디
        sendMsg.UnType = isCash ? 2 : 1;//1 이면 공짜 2면 캐쉬
        sendMsg.UnIdx = cardArr;//오픈한 카드의 배열 1~4값
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_FLOP_C, sendMsg);

        return true;
    }
    */
    /// <summary> 던전 소탕 </summary>
    public bool SendPMsgStageSweepC(uint stageId, int count, bool autoFlop)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgStageSweepC();
        sendMsg.UnStageId = (int)stageId;//던전 아이디
        sendMsg.UnTimes = count;//횟수
        //sendMsg.UnAutoFlop = autoFlop ? 1 : 0;//카드 한장 더?

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_SWEEP_C, sendMsg);

        return true;
    }

    /// <summary> 코스튬 스킬레벨업 </summary>
    public bool SendPMsgCostumeSkillUpgradeC(int unId, int unSkillIdx)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCostumeSkillUpgradeC();
        sendMsg.UnId = unId;
        sendMsg.UnSkillIdx = unSkillIdx;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_SKILL_UPGRADE_C, sendMsg);

        return true;
    }

    /// <summary> 모든 메일 정보 요청 조회 </summary>
    public bool PMsgEmailQueryListC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.MailCount = 0;

        var sendMsg = new PMsgEmailQueryListC();
        // sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_QUERY_LIST_C, sendMsg);

        return true;
    }

    /// <summary> 우편 상세 정보 조회(가져오기) </summary>
    public bool PMsgEmailReadDetailC(uint mailId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEmailReadDetailC();
        sendMsg.UnId = mailId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_READ_DETAIL_C, sendMsg);

        return true;
    }

    /// <summary> 첨부 아이템 수령 요청 </summary>
    public bool PMsgEmailFeatchC(uint mailId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEmailFeatchC();
        sendMsg.UnId = mailId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_FETCH_C, sendMsg);

        return true;
    }

    /// <summary> 일괄 수령 요청 </summary>
    public bool PMsgEmailOneKeyFeatchC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEmailOneKeyFeatchC();
        //sendMsg.UnReserved = reservedId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_ONEKEY_FEATCH_C, sendMsg);

        return true;
    }

    /// <summary> 단기 메일 삭제 요청 </summary>
    public bool PMsgEmailDelC(uint mailId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEmailDelC();
        sendMsg.UnId = mailId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_DEL_C, sendMsg);

        return true;
    }

    /// <summary> 일괄삭제 요청 </summary>
    public bool PMsgEmailOnKeyDelC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgEmailOnKeyDelC();
        // sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EMAIL_ONEKEY_DEL_C, sendMsg);

        return true;
    }


    public bool SendPMsgReturnMainMapC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgReturnMainMapC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RETURN_MAIN_MAP_C, sendMsg);

        return true;
    }

    public bool SendPMsgHeroQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgHeroQueryC");
        var sendMsg = new PMsgHeroQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_HERO_QUERY_C, sendMsg);

        return true;
    }

    public bool SendPMsgHeroFusionC(int unHeroType)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgHeroFusionC();
        sendMsg.UnHeroType = unHeroType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_HERO_FUSION_C, sendMsg);

        return true;
    }

    /// <summary> 파트너 성장(경험치) </summary>
    public bool SendPMsgHeroUseExpItemC(ulong uid, uint expLowDataId, int expAmount)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgHeroUseExpItemC();
        sendMsg.UnHeroId = (int)uid;
        sendMsg.UnItemId = (int)expLowDataId;
        sendMsg.UnItemNum = expAmount;

        SceneManager.instance.ShowNetProcess("PMsgHeroUseExpItemC");
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_HERO_USE_EXP_ITEM_C, sendMsg);

        return true;
    }

    /// <summary> 파트너 진화 별증가 </summary>
    public bool SendPMsgHeroEvolveC(ulong uid)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgHeroEvolveC();
        sendMsg.UnHeroId = (int)uid;
        SceneManager.instance.ShowNetProcess("pmsgHeroEvolveC");
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_HERO_EVOLVE_C, sendMsg);
        return true;
    }
   
    /// <summary> 파트너 액티브 스킬 레벨업 </summary>
    public bool SendPMsgHeroSkillUpgradeC(ulong parId, int skillArr)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgHeroSkillUpgradeC();
        sendMsg.UnHeroId = (int)parId;
        sendMsg.UnSkillIdx = skillArr;

        SceneManager.instance.ShowNetProcess("PMsgHeroSkillUpgradeC");
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_HERO_SKILL_UPGRADE_C, sendMsg);
        return true;
    }

    /// <summary> 친구리스트 요청 조회 </summary>
    public bool SendPMsgFriendQueryListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendQueryListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_QUERY_LIST_C, sendMsg);
        return true;
    }
    
    /// <summary> 친구 검색 요청 </summary>
    public bool SendPMsgFriendSearchC(string CharName)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendSearchC();
        sendMsg.SzName = CharName;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_SEARCH_C, sendMsg);
        return true;
    }

    /// <summary> 추천친구 리스트 요청 </summary>
    public bool SendPMsgFriendRecommendListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendRecommendListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_RECOMMEND_LISTC, sendMsg);
        return true;
    }

    /// <summary> 신청 추가 친구 </summary>
    public bool SendPMsgFriendAddC(ulong CharId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendAddC();
        sendMsg.UllRoleId = CharId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_ADD_C, sendMsg);
        return true;
    }

    /// <summary> 친구추가 리스트에서 타인에 대한 친구추가 신청 취소 </summary>
    public bool SendPMsgFriendCancleAddC(ulong CharId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendCancleAddC();
        sendMsg.UllRoleId = CharId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_CANCLE_ADD_C, sendMsg);
        return true;
    }

    /// <summary> 신청자 리스트 조회 </summary>
    public bool SendPMsgFriendRequestFriendListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendRequestFriendListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_REQUEST_FRIEND_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 본인이 추가한 친구 리스트 조회 </summary>
    public bool SendPMsgFriendSelfRequestFriendListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendSelfRequestFriendListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_SELF_REQUEST_FRIEND_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 신청자 요청 처리 </summary>
    public bool SendPMsgFriendApplicantC(ulong CharId, int Agree)    //Agree 동의혹은 거절, 1:동의 2:거절
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendApplicantC();
        sendMsg.UllRoleId = CharId;
        sendMsg.UnAccede = Agree;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_APPLICANT_C, sendMsg);
        return true;
    }

    /// <summary> 친구 삭제 요청 </summary>
    public bool SendPMsgFriendDelFriendC(ulong CharId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendDelFriendC();
        sendMsg.UllRoleId = CharId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_DEL_FRIEND_C, sendMsg);
        return true;
    }


    /// <summary> 친구에게 체력선물 요청 </summary>
    public bool SendPMsgFriendGivePowerC(ulong CharId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFriendGivePowerC();
        sendMsg.UllRoleId = CharId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_FRIEND_GIVE_POWER_C, sendMsg);
        return true;
    }

    public bool SendPMsgMapEnterMapReadyC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMapEnterMapReadyC();
        sendMsg.UnMapId = mapID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MAP_ENTER_MAP_READY_C, sendMsg);
        return true;
    }


    //경험치 던전 남은횟수 요청
    public bool SendPMsgExpBattleQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgExpBattleQueryC");
        var sendMsg = new PMsgExpBattleQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EXP_BATTLE_QUERY_C, sendMsg);
        return true;
    }

    //경험치 던전 시작요청
    public bool SendPMsgExpBattleStartC(uint unExpBattleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgExpBattleStartC();
        sendMsg.UnExpBattleId = (int)unExpBattleId;

        ulong[] partnerIDList = NetData.instance.GetUserInfo().GetEquipPartnersIdx();
        for (int i = 0; i < partnerIDList.Length; i++)
        {
            if (0 < partnerIDList[i])
                sendMsg.UnHeroId.Add((int)partnerIDList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EXP_BATTLE_START_C, sendMsg);
        return true;
    }

    //경험치 던전 결과전달
    public bool SendPMsgExpBattleCompleteC(uint unExpBattleId, int unVictory, int unExpTotal)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgExpBattleCompleteC();
        sendMsg.UnExpBattleId = (int)unExpBattleId;
        sendMsg.UnVictory = unVictory;
        sendMsg.UnExpTotal = unExpTotal;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EXP_BATTLE_COMPLETE_C, sendMsg);
        return true;
    }

    //골드 던전 시작요청
    public bool SendPMsgCoinBattleStartC(uint lowDataId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCoinBattleStartC();
        sendMsg.UnExpBattleId = (int)lowDataId;

        ulong[] partnerIDList = NetData.instance.GetUserInfo().GetEquipPartnersIdx();
        for (int i = 0; i < partnerIDList.Length; i++)
        {
            if (0 < partnerIDList[i])
                sendMsg.UnHeroId.Add((int)partnerIDList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COIN_BATTLE_START_C, sendMsg);
        return true;
    }

    //골드 던전 종료
    public bool SendPMsgCoinBattleCompleteC(uint lowDataId, int unVictory, int unGoldTotal)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCoinBattleCompleteC();
        sendMsg.UnExpBattleId = (int)lowDataId;
        sendMsg.UnVictory = unVictory;
        sendMsg.UnCoinTotal = unGoldTotal;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COIN_BATTLE_COMPLETE_C, sendMsg);
        return true;
    }

    //골드 던전 남은횟수 정보 조회
    public bool SendPMsgCoinBattleQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgCoinBattleQueryC");
        var sendMsg = new PMsgCoinBattleQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COIN_BATTLE_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 체력 구매(치트키로 사용중) </summary>
    public bool SendPMsgBuyPowerC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBuyPowerC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BUY_POWER_C, sendMsg);
        return true;
    }

    /// <summary> 마계의탑 정보 조회 </summary>
    public bool SendPMsgTowerBattleQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgTowerBattleQueryC");
        var sendMsg = new PMsgTowerBattleQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TOWER_BATTLE_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 마계의탑 게임시작 </summary>
    public bool SendPMsgTowerBattleStartC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTowerBattleStartC();
        sendMsg.UnTowerBattleId = (int)dungeonId;

        ulong[] partnerIDList = NetData.instance.GetUserInfo().GetEquipPartnersIdx();
        for (int i = 0; i < partnerIDList.Length; i++)
        {
            if (0 < partnerIDList[i])
                sendMsg.UnHeroId.Add((int)partnerIDList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TOWER_BATTLE_START_C, sendMsg);
        return true;
    }

    /// <summary> 마계의탑 클리어 </summary>
    public bool SendPMsgTowerBattleCompleteC(uint dungeonId, uint dieNum, uint killNum, float clearTime, bool isSuccess)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgTowerBattleCompleteC");

        var sendMsg = new PMsgTowerBattleCompleteC();
        sendMsg.UnTowerBattleId = (int)dungeonId;
        sendMsg.UnDieNum = dieNum;
        sendMsg.UnKillNum = killNum;
        sendMsg.UnVictory = isSuccess ? 1 : 0;
        sendMsg.UnTime = (uint)clearTime;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TOWER_BATTLE_COMPLETE_C, sendMsg);
        return true;
    }

    /// <summary> 마계의탑 랭킹 조회 </summary>
    public bool SendPMsgTowerRankQueryC(uint level)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgTowerRankQueryC");
        var sendMsg = new PMsgTowerRankQueryC();
        sendMsg.UnLevel = (int)level;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TOWER_RANK_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 마계의탑 전투소모 시간 최고 기록 조회 </summary>
    public bool SendPMsgTowerUseTimeQueryC(uint towerLv)
    {
        SceneManager.instance.ShowNetProcess("PMsgTowerUseTimeQueryC");
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTowerUseTimeQueryC();
        sendMsg.UnLevel = (int)towerLv;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TOWER_USE_TIME_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 레이드 조회 </summary>
    public bool SendPMsgBossBattleQueryC(int bossType)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgBossBattleQueryC");
        var sendMsg = new PMsgBossBattleQueryC();
        sendMsg.UnBattleType = bossType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BOSS_BATTLE_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 레이드 시작 </summary>
    public bool SendPMsgBossBattleStartC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBossBattleStartC();
        sendMsg.UnBossBattleId = (int)dungeonId;

        ulong[] partnerIDList = NetData.instance.GetUserInfo().GetEquipPartnersIdx();
        for (int i = 0; i < partnerIDList.Length; i++)
        {
            if (0 < partnerIDList[i])
                sendMsg.UnHeroId.Add((int)partnerIDList[i]);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BOSS_BATTLE_START_C, sendMsg);
        return true;
    }

    /// <summary> 레이드 종료 </summary>
    public bool SendPMsgBossBattleCompleteC(uint dungeonId, bool isSuccess, uint getExp)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBossBattleCompleteC();
        sendMsg.UnBossBattleId = (int)dungeonId;
        sendMsg.UnVictory = isSuccess ? 1 : 0;
        sendMsg.UnExpTotal = (int)getExp;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BOSS_BATTLE_COMPLETE_C, sendMsg);
        return true;
    }

    /// <summary> 상점 정보 조회 </summary>
    public bool SendPMsgShopInfoQueryC(uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgShopInfoQueryC();
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SHOP_INFO_QUERY_C, sendMsg);
        return true;
    }
    /// <summary> 상점 판매 물품 구매 요청 </summary>
    public bool SendPMsgShopByItemC(ulong Index, uint ShopType, uint GoodsType, uint Account, uint TableId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgShopByItemC();
        sendMsg.UllIndex = Index;
        sendMsg.UnShopType = ShopType;
        sendMsg.UnGoods = GoodsType;
        sendMsg.UnNum = Account;
        sendMsg.UnDbIndex = TableId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SHOP_BY_ITEM_C, sendMsg);
        return true;
    }
    /// <summary> 상점 판매 물품 리스트 리셋 요청 </summary>
    public bool SendPMsgShopRefreshC(uint Type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgShopRefreshC();
        sendMsg.UnShopType = Type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SHOP_Refresh_C, sendMsg);
        return true;
    }

    /// <summary> VIP정보 조회 </summary>
    public bool SendPMsgVipQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgVipQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_VIP_QUERY_INFO_C, sendMsg);
        return true;
    }
    /// <summary> 매일 소탕권 수동 수령 요청 </summary>
    public bool SendPMsgVipFetchSweepTicketC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgVipFetchSweepTicketC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_VIP_FETCH_SWEEP_TICKET_C, sendMsg);
        return true;
    }
    /// <summary> 출석 메꾸기 요청 </summary>
    public bool SendPMsgVipRepairSignInC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgVipRepairSignInC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_VIP_REPAIR_SIGN_IN_C, sendMsg);
        return true;
    }

    public bool SendPMsgTaskQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgTaskQueryInfoC");
        var sendMsg = new PMsgTaskQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TASK_QUERY_INFO_C, sendMsg);
        return true;
    }

    public bool SendPMsgTaskReceiveTaskC(uint QuestID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTaskReceiveTaskC();
        sendMsg.UnTaskId = QuestID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TASK_RECEIVE_TASK_C, sendMsg);
        return true;
    }

    public bool SendPMsgTaskCompleteC(uint QuestID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTaskCompleteC();
        sendMsg.UnTaskId = QuestID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TASK_COMPLETE_C, sendMsg);
        return true;
    }

    public bool SendPMsgTaskFetchBonusC(uint QuestID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgTaskFetchBonusC();
        sendMsg.UnTaskId = QuestID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TASK_FETCH_BONUS_C, sendMsg);
        return true;
    }

    public bool SendPMsgDailyTaskQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgDailyTaskQueryInfoC");
        var sendMsg = new PMsgDailyTaskQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_DAILY_TASK_QUERY_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 랭킹 </summary>
    public bool SendPMsgRankQueryC(int type)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRankQueryC");

        var sendMsg = new PMsgRankQueryC();
        sendMsg.UnRankType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RANK_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 길드랭킹 </summary>
    public bool SendPMsgRankGuildQueryC(int type)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRankGuildQueryC");

        var sendMsg = new PMsgRankGuildQueryC();
        sendMsg.UnRankType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RANK_GUILD_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 상자뽑기 정보 조회 </summary>
    public bool SendPMsgLotteryQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryQueryInfoC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_QUERYINFO_C, sendMsg);
        return true;
    }

    /// <summary> 무료 보통 뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxCommonFreeC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxCommonFreeC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_COMMONFREE_C, sendMsg);
        return true;
    }

    /// <summary> 유료 보통뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxCommonC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxCommonC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_COMMON_C, sendMsg);
        return true;
    }

    /// <summary> 보통 10연뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxCommonManytimesC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxCommonManytimesC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_COMMON_MANYTIMES_C, sendMsg);
        return true;
    }

    /// <summary> 무료 고급뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxSeniorFreeC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxSeniorFreeC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_SENIORFREE_C, sendMsg);
        return true;
    }

    /// <summary> 유료 고급뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxSeniorC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxSeniorC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_SENIOR_C, sendMsg);
        return true;
    }

    /// <summary> 10연 고급뽑기 요청 </summary>
    public bool SendPMsgLotteryBoxSeniorManytimesC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLotteryBoxSeniorManytimesC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOTTERY_BOX_SENIOR_MANYTIMES_C, sendMsg);
        return true;
    }

    /// <summary> 챕터 보상 조회 </summary>
    public bool SendPMsgStageChapterQueryC(int stageType)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgStageChapterQueryC");
        var sendMsg = new PMsgStageChapterQueryC();
        sendMsg.UnStageType = stageType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_CHAPTER_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 챕터 보상 획득 </summary>
    public bool SendPMsgStageChapterRewardC(int chapterId, int boxId ,int stageType)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgStageChapterRewardC();
        sendMsg.UnChapterId = chapterId;
        sendMsg.UnBoxId = boxId;
        sendMsg.UnStageType= stageType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_STAGE_CHAPTER_REWARD_C, sendMsg);
        return true;
    }

    /// <summary> 코스튬 숨기기 </summary>
    public bool SendPMsgCostumeShowFlagC(bool isHide)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgCostumeShowFlagC();
        sendMsg.UnShowFlag = (int)(isHide ? COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE : COSTUME_FLAG_TYPE.COSTUME_FLAG_SHOW);

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COSTUME_SHOW_FLAG_C, sendMsg);
        return true;
    }

    /// <summary> 칭호 정보 요청 </summary>
    public bool SendPMsgTitleQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgTitleQueryInfoC");
        var sendMsg = new PMsgTitleQueryInfoC();
        sendMsg.UnReserved = 1;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TITLE_QUERY_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 칭호 변경 </summary>
    public bool SendPMsgUseTitleC(uint lowDataId, bool isEquip)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgUseTitleC();
        sendMsg.UnTitleId = lowDataId;
        sendMsg.UnAdorn = (uint)(isEquip ? 1 : 2);//1:사용, 2:미사용

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_TITLE_USE_TITLE_C, sendMsg);
        return true;
    }

    /// <summary> 서버로 요청 </summary>
    public bool SendPMsgLoginOverC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgLoginOverC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_LOGIN_OVER_C, sendMsg);
        return true;
    }

    /// <summary> 튜토리얼 단계저장  </summary>
    public bool SendPMsgRoleGuideC(uint idx)
    {
        if (!GetGameConnection())
            return false;
        
        _guideID = idx;
        
        var sendMsg = new PMsgRoleGuideC();
        sendMsg.UllGuide = idx;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_GUIDE_C, sendMsg);
        return true;
    }


    /// <summary> 당월 출석정보 요청  </summary>
    public bool SendPMsgSignInQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgSignInQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SIGNIN_QUERY_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 출석체크 요청 </summary>
    public bool SendPMsgSignInC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgSignInC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SIGNIN_SIGNIN_C, sendMsg);
        return true;
    }

    /// <summary> 출석채우기 요청 </summary>
    public bool SendPMsgFillInSignInC(uint month)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgFillInSignInC();
        sendMsg.UnYYYYMMDD = month;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_SIGNIN_FILL_IN_SIGNIN_C, sendMsg);
        return true;
    }

    ///<summary> 업적 정보조회 요청 </summary>
    public bool SendPMsgAchieveQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_QUERY_INFO_C, sendMsg);
        return true;
    }

    /////<summary> 장비관련 업적데이터 통계정보 조화 요청 </summary>
    //public bool SendPMsgAchieveEquipTotalQueryInfoC()
    //{
    //    if (!GetGameConnection())
    //        return false;

    //    var sendMsg = new PMsgAchieveEquipTotalQueryInfoC();
    //    mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_EQUIP_TOTAL_QUERY_INFO_C, sendMsg);
    //    return true;
    //}

    ///<summary> 전투관련 업적데이터 통계정보 조회 요청 </summary>
    public bool SendPMsgAchieveFightTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveFightTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_FIGHT_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> 소셜관련 업적데이터 통계정보 조회요청 </summary>
    public bool SendPMsgAchieveFriendTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveFriendTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_FRIEND_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> 경제관련 업적데이터 통계정보 조회요청 </summary>
    public bool SendPMsgAchieveMoneyTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveMoneyTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_MONEY_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> 컨텐츠관련 업적데이터 통계쩡보 조회요청 </summary>
    public bool SendPMsgAchievePlayTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchievePlayTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_PLAY_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> 캐릭터관련 업적데이터 통계정보 조회 요청응답 </summary>
    public bool SendPMsgAchieveRoleTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveRoleTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_ROLE_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> vip관련 업적데이터  </summary>
    public bool SendPMsgAchieveVipTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveVipTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_VIP_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary> 업적보상 획득요청 </summary>
    public bool SendPMsgAchieveFetchAwardC(uint type, uint subType)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveFetchAwardC();
        sendMsg.UnType = type;
        sendMsg.UnSubType = subType;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_FETCH_AWARD_C, sendMsg);
        return true;
    }

    ///<summary> 업적 포인트 보상 획득요청  </summary>
    public bool SendPMsgAchieveFetchPointAwardC(uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveFetchPointAwardC();
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_FETCH_POINTS_AWARD_C, sendMsg);
        return true;
    }

    ///<summary>전투관련 업적데이터 동기화 </summary>
    public bool SendPMsgAchieveSynFightDataTotalValueC(uint combo, uint damage, uint skillCount, uint monsterKill, uint bossKill, uint Pk, uint die, uint revive, uint heroSkillCnt)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveSynFightDataTotalValueC();
        sendMsg.UnMaxCaromCount = combo;
        sendMsg.UnMaxDamage = damage;
        sendMsg.UnUseSkillTotal = skillCount;
        sendMsg.UnKillMonsterTotal = monsterKill;
        sendMsg.UnKillBossTotal = bossKill;
        sendMsg.UnKillOtherRoleTotal = Pk;
        sendMsg.UnOneselfDieTotal = die;
        sendMsg.UnOnselfReviveTotal = revive;
        sendMsg.UnHeroUseSkillTotal = heroSkillCnt;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_SYN_FIGHTDATA_TOTAL_VALUE_C, sendMsg);
        return true;
    }


    /// <summary> 콜로세움 던전 정보 </summary>
    public bool SendPMsgColosseumQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgColosseumQueryC");
        var sendMsg = new PMsgColosseumQueryC();
        sendMsg.UnReserved = 1;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 던전 생성 </summary>
    public bool SendPMsgColosseumCreateRoomC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumCreateRoomC();
        sendMsg.UnColosseumId = (int)dungeonId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_CREATE_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 던전 파티 초대 </summary>
    public bool SendPMsgColosseumInviteC(int inviteType, List<long> userList)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumInviteC();
        sendMsg.UnGlobalFlag = inviteType;//0 == 친구or길드, 1 == 마을
        if (userList != null)
        {
            for (int i = 0; i < userList.Count; i++)
            {
                sendMsg.UllRoleId.Add(userList[i]);
            }
        }

        //sendMsg.Ull
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_INVITE_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 던전 방 조회(초대 받을 시 사용) </summary>
    public bool SendPMsgColosseumRoomInfoC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumRoomInfoC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_ROOM_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 던전 방 이탈 </summary>
    public bool SendPMsgColosseumLeaveRoomC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumLeaveRoomC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_LEAVE_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 던전 방 진입 </summary>
    public bool SendPMsgColosseumEnterRoomC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumEnterRoomC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_ENTER_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 유저 추방 </summary>
    public bool SendPMsgColosseumKickRoomC(ulong userRoleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumKickRoomC();
        sendMsg.UllRoleId = (long)userRoleId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_KICK_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 게임 시작 </summary>
    public bool SendPMsgColosseumStartC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumStartC();
        sendMsg.UnColosseumId = (int)dungeonId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_START_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 게임 종료 </summary>
    public bool SendPMsgColosseumCompleteC(uint dungeonId, bool isSuccess, int getExp)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumCompleteC();
        sendMsg.UnColosseumId = (int)dungeonId;
        sendMsg.UnVictory = isSuccess ? 1 : 0;
        sendMsg.UnExpTotal = getExp;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_COMPLETE_C, sendMsg);
        return true;
    }

    /// <summary> 콜로세움 게임 소탕 </summary>
    public bool SendPMsgColosseumSweepC(uint dungeonId, int count)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgColosseumSweepC();
        sendMsg.UnColosseumId = (int)dungeonId;
        sendMsg.UnTimes = count;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_COLOSSEUM_SWEEP_C, sendMsg);
        return true;
    }

    ///<summary> 업적보상 일괄획득  </summary>
    public bool SendPMsgAchieveOneKeyFetchAchieveAwardC(uint type)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgAchieveOneKeyFetchAchieveAwardC();
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_ONEKEY_FETCH_ACHIEVE_AWARD_C, sendMsg);
        return true;
    }
    ///<summary> 로그인유지보상 정보조회  </summary>
    public bool SendPMsgWelfareOnlineQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareOnlineQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFAREONLINE_QUERY_INFO_C, sendMsg);
        return true;
    }
    ///<summary> 로그인유지보상 획득  </summary>
    public bool SendPMsgWelfareOnlineFetchRewardC(uint fetchLv)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareOnlineFetchRewardC();
        sendMsg.UnFetchLevel = fetchLv;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFAREONLINE_FETCH_REWARD_C, sendMsg);
        return true;
    }

    ///<summary> 신썹오픈보상정보조회  </summary>
    public bool SendPMsgWelfareOpenSvrQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareOpenSvrQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_OPEN_SVR_QUERY_INFO_C, sendMsg);
        return true;
    }
    ///<summary> 신썹오픈보상 획득 </summary>
    public bool SendPMsgWelfareOpenSvrFetchRewardC(uint day)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareOpenSvrFetchRewardC();
        sendMsg.UnDay = day;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_OPEN_SVR_FETCH_REWARD_C, sendMsg);
        return true;
    }
    ///<summary> 복귀보상정보조회  </summary>
    public bool SendPMsgWelfareReturnQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareReturnQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_RETURN_QUERY_INFO_C, sendMsg);
        return true;
    }
    ///<summary> 복귀보상 획득 </summary>
    public bool SendPMsgWelfareReturnFetchRewardC(uint day)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareReturnFetchRewardC();
        sendMsg.UnDay = day;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_RETURN_FETCH_REWARD_C, sendMsg);
        return true;
    }

    ///<summary> 7일연속 로그인 보상정보조회  </summary>
    public bool SendPMsgWelfareXDayLoginQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareXDayLoginQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_XDAY_LOGIN_QUERY_INFO_C, sendMsg);
        return true;
    }
    ///<summary> 7일연속로그인 보상 획득 </summary>
    public bool SendPMsgWelfareFetchXDayRewardC(uint day)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareFetchXDayRewardC();
        sendMsg.UnDay = day;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_FETCH_XDAY_REWARD_C, sendMsg);
        return true;
    }

    ///<summary> 랩업패키지 보상정보조회 </summary>
    public bool SendPMsgWelfareRoleUpgradeQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareRoleUpgradeQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_ROLE_UPGRADE_QUERY_INFO_C, sendMsg);
        return true;
    }

    ///<summary>랩업패키지 보상 획득</summary>
    public bool SendPMsgWelfareFetchRoleUpgradeRewardC(uint idx)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgWelfareFetchRoleUpgradeRewardC();
        sendMsg.UnRewardIndex = idx;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_WELFARE_FETCH_ROLE_UPGRADE_REWARD_C, sendMsg);
        return true;
    }

    ///<summary> 멀티보스 레이드 정보 조회 </summary>
    public bool SendPMsgMultiBossQueryC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgMultiBossQueryC");
        var sendMsg = new PMsgMultiBossQueryC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_QUERY_C, sendMsg);
        return true;
    }

    ///<summary> 멀티보스 레이드 방생성 </summary>
    public bool SendPMsgMultiBossCreateRoomC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossCreateRoomC();
        sendMsg.UnBossBattleId = (int)dungeonId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_CREATE_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 파티 초대 </summary>
    public bool SendPMsgMultiBossInviteC(int inviteType, List<long> userList)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossInviteC();
        sendMsg.UnGlobalFlag = inviteType;//0 == 친구or길드, 1 == 마을
        if (userList != null)
        {
            for (int i = 0; i < userList.Count; i++)
            {
                sendMsg.UllRoleId.Add(userList[i]);
            }
        }

        //sendMsg.Ull
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_INVITE_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 방 조회(초대 받을 시 사용) </summary>
    public bool SendPMsgMultiBossRoomInfoC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossRoomInfoC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_ROOM_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 방 이탈 </summary>
    public bool SendPMsgMultiBossLeaveRoomC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossLeaveRoomC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_LEAVE_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 방 진입 </summary>
    public bool SendPMsgMultiBossEnterRoomC(long roomId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossEnterRoomC();
        sendMsg.UllRoomId = roomId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_ENTER_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 유저 추방 </summary>
    public bool SendPMsgMultiBossKickRoomC(ulong userRoleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossKickRoomC();
        sendMsg.UllRoleId = (long)userRoleId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_KICK_ROOM_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 게임 시작 </summary>
    public bool SendPMsgMultiBossStartC(uint dungeonId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossStartC();
        sendMsg.UnBossBattleId = (int)dungeonId;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_START_C, sendMsg);
        return true;
    }

    /// <summary> 멀티보스 레이드 종료 </summary>
    public bool SendPMsgMultiBossCompleteC(uint dungeonId, bool isSuccess, uint getExp)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMultiBossCompleteC();
        sendMsg.UnBossBattleId = (int)dungeonId;
        sendMsg.UnVictory = isSuccess ? 1 : 0;
        sendMsg.UnExpTotal = (int)getExp;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MULTI_BOSS_COMPLETE_C, sendMsg);
        return true;
    }


    /// <summary> 누적충전 정보조회 </summary>
    public bool SendPMsgRechargeTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    /// <summary> 누적충전 정보보상획득</summary>
    public bool SendPMsgRechargeTotalFetchRewardC(uint lv)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeTotalFetchRewardC();
        sendMsg.UnFetchLevel = lv;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_TOTAL_FETCH_REWARD_C, sendMsg);
        return true;
    }
    /// <summary> 일간충전 정보조회 </summary>
    public bool SendPMsgRechargeDailyQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeDailyQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_DAILY_QUERY_INFO_C, sendMsg);
        return true;
    }
    /// <summary> 일간충전 정보보상획득</summary>
    public bool SendPMsgRechargeDailyFetchRewardC(uint lv)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeDailyFetchRewardC();
        sendMsg.UnFetchLevel = lv;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_DAILY_FETCH_REWARD_C, sendMsg);
        return true;
    }
    /// <summary> 누적소비 정보조회 </summary>
    public bool SendPMsgRechargeConsumerQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeConsumerQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_CONSUMER_QUERY_INFO_C, sendMsg);
        return true;
    }
    /// <summary> 누적소비 정보보상획득</summary>
    public bool SendPMsgRechargeConsumerFetchRewardC(uint lv)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRechargeConsumerFetchRewardC();
        sendMsg.UnFetchLevel = lv;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_RECHARGE_CONSUMER_FETCH_REWARD_C, sendMsg);
        return true;
    }

    public bool SendPMsgAccountCertifyC(uint unPlatformType, string szMachineKey, uint unApplId, uint unChannelId, string szChannelAccountId, string szChannelAccountName, string szChannelToken, string szChannelData)
    {
        if (!GetAuthConnection())
            return false;

        var sendMsg = new PMsgAccountCertifyC();
        sendMsg.UnPlatformType = (int)unPlatformType;
        sendMsg.SzMachineKey = szMachineKey;
        sendMsg.UnApplId = (int)unApplId;
        sendMsg.UnChannelId = (int)unChannelId;
        sendMsg.SzChannelAccountId = szChannelAccountId;
        sendMsg.SzChannelAccountName = szChannelAccountName;
        sendMsg.SzChannelToken = szChannelToken;
        sendMsg.SzChannelData = szChannelData;

        Debug.Log("SendPMsgAccountCertifyC:" + szMachineKey);

        mNetworkAuth.SendMsg(MSG_DEFINE._MSG_ACCOUNT_CERTIFY_C, sendMsg);
        return true;
    }

    /// <summary> 유저정보 가져오기 </summary>
    public bool SendPMsgQueryRoleInfoC(long roleId)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgQueryRoleInfoC();
        sendMsg.UllRoleId = roleId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_QUERY_ROLE_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 인증 정보 받아오기 </summary>
    public bool SendPMsgUserCertifyInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgUserCertifyInfoC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_CERTIFY_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 인증 받기 </summary>
    public bool SendPMsgUserCertifySetC(string name, string number)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgUserCertifySetC();
        sendMsg.SzName = name;
        sendMsg.SzIdCard = number;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_CERTIFY_SET_C, sendMsg);
        return true;
    }

    /// <summary> 활동량 정보 조회 </summary>
    /*public bool SendPMsgActivePointsQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgActivePointsQueryInfoC");
        var sendMsg = new PMsgActivePointsQueryInfoC();
        sendMsg.NReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACTIVE_POINTS_QUERY_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 활동량 보상받기 </summary>
    public bool SendPMsgActivePointsFetchRewardC(uint id)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgActivePointsFetchRewardC();
        sendMsg.UnLevel = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACTIVE_POINTS_FETCHREWARD_C, sendMsg);
        return true;
    }

    /// <summary> 활동량 클리어 한 정보들 </summary>
    public bool SendPMsgActivePointsTotalCountQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgActivePointsTotalCountQueryInfoC();
        sendMsg.NReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACTIVE_POINTS_TOTALCOUNT_QUERY_INFO_C, sendMsg);
        return true;
    }*/
    
    /// <summary> 차관 정보 요청 </summary>
    public bool SendPMsgArenaInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgArenaInfoC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 차관 전황 정보 요청 </summary>
    public bool SendPMsgArenaFightListC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgArenaFightListC");
        var sendMsg = new PMsgArenaFightListC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_FIGHT_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 차관 리셋 횟수 요청 </summary>
    public bool SendPMsgArenaResetTimesC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgArenaResetTimesC");
        var sendMsg = new PMsgArenaResetTimesC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_RESET_TIMES_C, sendMsg);
        return true;
    }

    /// <summary> 차관 본인 랭킹 조회 </summary>
    public bool SendPMsgArenaRankInfoC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgArenaRankInfoC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_RANK_INFO_C, sendMsg);
        return true;
    }

    /// <summary> 차관 TOP100 랭킹 조회 </summary>
    public bool SendPMsgArenaRankListC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgArenaRankListC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_RANK_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 차관 상대 리셋 요청 </summary>
    public bool SendPMsgArenaMatchListC(int matchRank)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgArenaMatchListC");

        var sendMsg = new PMsgArenaMatchListC();
        sendMsg.UnMatchRank = matchRank;// 전투전(최초) 순위 初始名次
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_MATCH_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 차관 전투 요청 </summary>
    public bool SendPMsgArenaFightStartC(int[] partnerIds, long matchTargetRoleId, int matchTargetRank)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgArenaFightStartC();
        for (int i = 0; i < partnerIds.Length; i++)
        {
            if (partnerIds[i] <= 0)
                continue;

            sendMsg.UnHeroId.Add(partnerIds[i] );
        }

        SceneManager.instance.ShowNetProcess("PMsgArenaFightStartC");
        sendMsg.UllMatchRoleId = matchTargetRoleId;
        sendMsg.UnMatchRank = matchTargetRank;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_FIGHT_START_C, sendMsg);
        return true;
    }
    /*
    /// <summary> 차관 전투 나가기 요청 </summary>
    public bool SendPMsgArenaFightLeaveC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgArenaFightLeaveC();
        sendMsg.UnReserved = 1;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_FIGHT_LEAVE_C, sendMsg);
        return true;
    }
    */
    /// <summary> 차관 전투 완료 </summary>
    public bool SendPMsgArenaFightCompleteC(bool isVictory)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgArenaFightCompleteC");
        var sendMsg = new PMsgArenaFightCompleteC();
        sendMsg.UnVictory = (int)(isVictory ? FIGHT_RESULT.FIGHT_RESULT_WIN : FIGHT_RESULT.FIGHT_RESULT_FAIL);
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ARENA_FIGHT_COMPLETE_C, sendMsg);
        return true;
    }

    /// <summary> 장비 셋트 리스트 조회 </summary>
    public bool SendPMsgEquipmentSetQueryC()
    {
        if (!GetGameConnection())
            return false;
        
        var sendMsg = new PMsgEquipmentSetQueryC();
        sendMsg.UnReserved = 1;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_SET_QUERY_C, sendMsg);
        return true;
    }

    /// <summary> 셋트 장비 교체 </summary>
    public bool SendPMsgEquipmentSetChangeC(uint id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgEquipmentSetChangeC");
        var sendMsg = new PMsgEquipmentSetChangeC();
        sendMsg.UnSetId = (int)id;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_SET_CHANGE_C, sendMsg);
        return true;
    }

    /// <summary> 셋트 장비 구매 </summary>
    public bool SendPMsgEquipmentSetSelectC(uint id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgEquipmentSetSelectC");
        var sendMsg = new PMsgEquipmentSetSelectC();
        sendMsg.UnSetId = (int)id;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_SET_SELECT_C, sendMsg);
        return true;
    }
    
    /// <summary> 연속 강화 </summary>
    public bool SendPMsgEquipmentEnchantTurboC(ulong itemIdx, uint dataId, int count)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgEquipmentEnchantTurboC");
        var sendMsg = new PMsgEquipmentEnchantTurboC();
        sendMsg.UnId = (int)itemIdx;
        sendMsg.UnType = (int)dataId;
        sendMsg.UnTimes = count;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_TURBO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyQueryInfoC");

        var sendMsg = new PMsgAchieveDailyQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyFightTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyFightTotalQueryInfoC");
        var sendMsg = new PMsgAchieveDailyFightTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_FIGHT_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyFriendTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyFriendTotalQueryInfoC");
        var sendMsg = new PMsgAchieveDailyFriendTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_FRIEND_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyMoneyTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyMoneyTotalQueryInfoC");
        var sendMsg = new PMsgAchieveDailyMoneyTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_MONEY_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyPlayTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyPlayTotalQueryInfoC");

        var sendMsg = new PMsgAchieveDailyPlayTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_PLAY_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyRoleTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyRoleTotalQueryInfoC");

        var sendMsg = new PMsgAchieveDailyRoleTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_ROLE_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyVipTotalQueryInfoC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgAchieveDailyVipTotalQueryInfoC");

        var sendMsg = new PMsgAchieveDailyVipTotalQueryInfoC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_VIP_TOTAL_QUERY_INFO_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyFetchAwardC(uint type)
    {
        if (!GetGameConnection())
            return false;
        var sendMsg = new PMsgAchieveDailyFetchAwardC();
        sendMsg.UnType = type;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_FETCH_AWARD_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyFetchPointAwardC(uint level)
    {
        if (!GetGameConnection())
            return false;
        var sendMsg = new PMsgAchieveDailyFetchPointAwardC();
        sendMsg.UnLevel = level;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_FETCH_POINTS_AWARD_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailySynFightDataTotalValueC()
    {
        if (!GetGameConnection())
            return false;
        var sendMsg = new PMsgAchieveDailySynFightDataTotalValueC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_SYN_FIGHTDATA_TOTAL_VALUE_C, sendMsg);
        return true;
    }
    public bool SendPMsgAchieveDailyOneKeyFetchAchieveAwardC()
    {
        if (!GetGameConnection())
            return false;
        var sendMsg = new PMsgAchieveDailyOneKeyFetchAchieveAwardC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ACHIEVE_DAILY_ONEKEY_FETCH_ACHIEVE_AWARD_C, sendMsg);
        return true;
    }

    /// <summary> 신분 리스트 요청 </summary>
    public bool SendPMsgRoleIdentifyListC()
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgRoleIdentifyListC");
        var sendMsg = new PMsgRoleIdentifyListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_IDENTIFY_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 신분 추가 요청 </summary>
    public bool SendPMsgRoleIdentifyUnlockC(uint statusId)
    {
        if (!GetGameConnection())
            return false;
        SceneManager.instance.ShowNetProcess("PMsgRoleIdentifyUnlockC");
        var sendMsg = new PMsgRoleIdentifyUnlockC();
        sendMsg.UnType = (int)statusId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCK_C, sendMsg);
        return true;
    }

    /// <summary> 신분 레벨업 요청 </summary>
    public bool SendPMsgRoleIdentifyUpgradeC(int id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRoleIdentifyUpgradeC");
        var sendMsg = new PMsgRoleIdentifyUpgradeC();
        sendMsg.UnId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_IDENTIFY_UPGRADE_C, sendMsg);
        return true;
    }

    /// <summary> 신분 장착 요청 </summary>
    public bool SendPMsgRoleIdentifyUseC(int id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRoleIdentifyUseC");
        var sendMsg = new PMsgRoleIdentifyUseC();
        sendMsg.UnId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_IDENTIFY_USE_C, sendMsg);
        return true;
    }

    /// <summary> 보유중인 스킬리스트 요청 </summary>
    public bool SendPMsgRoleActiveSkillListC()
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRoleActiveSkillListC");
        var sendMsg = new PMsgRoleActiveSkillListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 스킬 레벨업 요청 </summary>
    public bool SendPMsgRoleActiveSkillUpgradeC(uint lowDataId, int number)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRoleActiveSkillUpgradeC");
        var sendMsg = new PMsgRoleActiveSkillUpgradeC();
        sendMsg.UnType = (int)lowDataId;
        sendMsg.UnIdx = number;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_C, sendMsg);
        return true;
    }

    /// <summary> 스킬장착 요청 </summary>
    public bool SendPMsgRoleActiveSkillUseC(uint lowDataId)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRoleActiveSkillUseC");
        var sendMsg = new PMsgRoleActiveSkillUseC();
        sendMsg.UnType = (int)lowDataId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_USE_C, sendMsg);
        return true;
    }
    
    /// <summary> 보유중인 패시브 리스트 요청 </summary>
    public bool SendPMsgRolePassiveSkillListC()
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillListC");
        var sendMsg = new PMsgRolePassiveSkillListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_LIST_C, sendMsg);
        return true;
    }

    /// <summary> 패시브 레벨업 요청 </summary>
    public bool SendPMsgRolePassiveSkillUpgradeC(uint lowDataId)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUpgradeC");
        var sendMsg = new PMsgRolePassiveSkillUpgradeC();
        sendMsg.UnSkillId = (int)lowDataId;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_C, sendMsg);
        return true;
    }

    /// <summary> 패시브 장착 요청 </summary>
    public bool SendPMsgRolePassiveSkillUseC(int id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUseC");
        var sendMsg = new PMsgRolePassiveSkillUseC();
        sendMsg.UnSkillId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_USE_C, sendMsg);
        return true;
    }

    /// <summary> 패시브 일괄 강화 요청 </summary>
    public bool SendPMsgRolePassiveSkillUpgradeTurboC(int id)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUpgradeTurboC");
        var sendMsg = new PMsgRolePassiveSkillUpgradeTurboC();
        sendMsg.UnSkillId = id;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_TURBO_C, sendMsg);
        return true;
    }

    /// <summary> 스킬 일괄강화  요청 </summary>
    public bool SendPMsgRoleActiveSkillUpgradeTurboC(int id, int number)
    {
        if (!GetGameConnection())
            return false;

        SceneManager.instance.ShowNetProcess("PMsgRoleActiveSkillUpgradeTurboC");
        var sendMsg = new PMsgRoleActiveSkillUpgradeTurboC();
        sendMsg.UnType = id;
        sendMsg.UnIdx = number;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_TURBO_C, sendMsg);
        return true;
    }

    /// <summary> 신분 미해금 정보 호출 </summary>
    public bool SendPMsgRoleIdentifyUnlockedListC()
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRoleIdentifyUnlockedListC");
        var sendMsg = new PMsgRoleIdentifyUnlockedListC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCKED_LIST_C, sendMsg);
        return true; 
    }
    //서버에 구글바인드 요청
    public bool SendPMsgUserBindGoogleC(string szGoogleAccount)
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUseC");
        var sendMsg = new PMsgUserBindGoogleC();
        sendMsg.SzGoogleAccount  = szGoogleAccount;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_BIND_GOOGLE_C, sendMsg);
        return true;
    }

    public bool SendPMsgUserBindQueryFbGoogleC()
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUseC");
        var sendMsg = new PMsgUserBindQueryFbGoogleC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_BIND_QUERY_FB_GOOGLE_C, sendMsg);
        return true;
    }

    //서버에 페이스북 바인드 요청
    public bool SendPMsgUserBindFacebookC(string szFacebookAccount)
    {
        if (!GetGameConnection())
            return false;

        //SceneManager.instance.ShowNetProcess("PMsgRolePassiveSkillUseC");
        var sendMsg = new PMsgUserBindFacebookC();
        sendMsg.SzFacebookAccount = szFacebookAccount;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_USER_BIND_FACEBOOK_C, sendMsg);
        return true;
    }

    #endregion

    #region Response

    /// <summary> 서버와의? 로그인 성공. 게임로그인으로 넘어가기 전 단계 </summary>
    private void PMsgLoginSHandler(PMsgLoginS pmsgLogin)
    {
        uint ErrorCode = (uint)pmsgLogin.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgLogin.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            SceneManager.instance.EndNetProcess("Login");
            return;
        }

        //byte[] ipbyte = System.BitConverter.GetBytes(pmsgLogin.UnIP);
        //Debug.Log(pmsgLogin);
        //IPAddress ipAddress = new IPAddress(System.BitConverter.ToUInt32(ipbyte, 0));


        _serverIP = pmsgLogin.UnIP;
        _serverPort = pmsgLogin.UnPort;

        DisconnectAuthServer();

        NetData.instance.UnCode = pmsgLogin.UnCode;
        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
        {
            return;
        }

        ///게임 로그인을 호출한다.
        (panel as LoginPanel).SendGameLogin();
    }

    /// <summary> 서버리스트 받아온다. </summary>
    private void PMsgServerListSHandler(PMsgServerListS pmsgServerList)
    {
        /*
        for(int i=0;i< pmsgServerList.CServerInfoCount;i++)

        
        {
            byte[] ipbyte = System.BitConverter.GetBytes(pmsgServerList.CServerInfoList[i].UnIp);
            IPAddress ipAddress = new IPAddress(System.BitConverter.ToUInt32(ipbyte, 0));

            Debug.Log("Server List ID:" + pmsgServerList.CServerInfoList[i].UnServerID + " IP:" + ipAddress + " Port:" + pmsgServerList.CServerInfoList[i].UnPort);
        }
        */
        /*
        Debug.Log("PMsgServerListSHandler : " + pmsgServerList);
        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
            return;

        (panel as LoginPanel).OnServerList(pmsgServerList.CServerInfoList);
        */
    }

    private void PMsgAccountCertifySHandler(PMsgAccountCertifyS pmsgAccountCertifyS)
    {
        uint ErrorCode = (uint)pmsgAccountCertifyS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAccountCertifyS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
            return;

        Debug.Log("PMsgAccountCertifySHandler:" + pmsgAccountCertifyS.UllUserId + ",SignedKey" + pmsgAccountCertifyS.UnSignedKey);

        NetData.instance.UUID = pmsgAccountCertifyS.UllUserId;
        NetData.instance._userid = pmsgAccountCertifyS.SzMachineKey;
        (panel as LoginPanel).OnAccountCertifyComplet(pmsgAccountCertifyS.UllUserId, pmsgAccountCertifyS.UnSignedKey, eLoginType.GUEST);
    }

    private void PMsgGoogleCertifySHandler(PMsgGoogleCertifyS pmsgGoogleCertifySH)
    {
        uint ErrorCode = (uint)pmsgGoogleCertifySH.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgGoogleCertifySH.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
            return;

        Debug.Log("PMsgGoogleCertifySHandler:" + pmsgGoogleCertifySH.UllUserId + ",SignedKey" + pmsgGoogleCertifySH.UnSignedKey);

        NetData.instance.UUID = pmsgGoogleCertifySH.UllUserId;
        NetData.instance._userid = pmsgGoogleCertifySH.SzGoogleAccount;
        (panel as LoginPanel).OnAccountCertifyComplet(pmsgGoogleCertifySH.UllUserId, pmsgGoogleCertifySH.UnSignedKey, eLoginType.GOOGLE);
    }

    private void PMsgFacebookCertifySHandler(PMsgFacebookCertifyS pmsgFacebookCertifyS)
    {
        uint ErrorCode = (uint)pmsgFacebookCertifyS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgFacebookCertifyS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
            return;

        Debug.Log("PMsgFacebookCertifyS:" + pmsgFacebookCertifyS.UllUserId + ",SignedKey" + pmsgFacebookCertifyS.UnSignedKey);

        NetData.instance.UUID = pmsgFacebookCertifyS.UllUserId;
        NetData.instance._userid = pmsgFacebookCertifyS.SzFacebookAccount;
        (panel as LoginPanel).OnAccountCertifyComplet(pmsgFacebookCertifyS.UllUserId, pmsgFacebookCertifyS.UnSignedKey, eLoginType.FACEBOOK);
    }

    /// <summary> 게임 로그인 실패시. 호출. </summary>
    private void PMsgGameLoginSHandler(PMsgGameLoginS pmsgGameLogin)
    {
        SceneManager.instance.EndNetProcess("Login");
        uint ErrorCode = (uint)pmsgGameLogin.UnErrorCode;
        if (pmsgGameLogin.UnErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            Debug.Log(pmsgGameLogin.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            UIMgr.instance.AddErrorPopup((int)ErrorCode);

            UIBasePanel loginPanel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
            //if (loginPanel != null)
            //    (loginPanel as LoginPanel).IsSendLogin = false;
        }
        else
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            
        }
    }

    private void PMsgUserInfoSHandler(PMsgUserInfoS pmsgPMsgUserInfo)
    {
        
    }

    private void PMsgUserBindGoogleSHandler(PMsgUserBindGoogleS pmsgUserBindGoogleS)
    {
        Debug.Log(pmsgUserBindGoogleS);

    }

    private void PMsgUserBindFacebookSHandler(PMsgUserBindFacebookS pmsgUserBindFacebookS)
    {
        Debug.Log("PMsgUserBindFacebookSHandler:" + pmsgUserBindFacebookS);

        uint ErrorCode = (uint)pmsgUserBindFacebookS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgUserBindFacebookS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel optionPanel =  UIMgr.GetUIBasePanel("UIPanel/OptionPanel");

        if (optionPanel != null)
            (optionPanel as OptionPanel).OnFacebookBind(pmsgUserBindFacebookS.SzFacebookAccount);
    }

    private void PMsgUserBindQueryFbGoogleSHandler(PMsgUserBindQueryFbGoogleS pmsgUserBindQueryFbGoogleS)
    {
        Debug.Log("PMsgUserBindQueryFbGoogleSHandler:" + pmsgUserBindQueryFbGoogleS);

        uint ErrorCode = (uint)pmsgUserBindQueryFbGoogleS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgUserBindQueryFbGoogleS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel optionPanel = UIMgr.GetUIBasePanel("UIPanel/OptionPanel");

        if (optionPanel != null)
            (optionPanel as OptionPanel).OnCheckBindQuery(pmsgUserBindQueryFbGoogleS);
    }

    private void PMsgGameDisconnectSHandler(PMsgGameDisconnectS pmsgGameDisconnectS)
    {
        UIMgr.instance.AddPopup(141, 109, 117, 0, 0 ,() =>//8
        {
            if (SceneManager.instance.CurrState() != _STATE.LOGIN)
            {
                SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
                {
                    UIMgr.ClearUI(true);
                    UITextList.ClearTextList();
                    //UIMgr.GetTownBasePanel().Close();
                    //UIMgr.instance.Clear(UIMgr.UIType.System);

                    //NetworkClient.instance.DisconnectGameServer();//연결 종료
                    NetData.instance.InitUserData();
                    NetData.instance.ClearCharIdc();

                    SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
                });
            }
            else
            {
                NetData.instance.InitUserData();
                NetData.instance.ClearCharIdc();
                
                UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/SelectHeroPanel");
                if (heroPanel != null)
                    (heroPanel as SelectHeroPanel).OnCloseCompulsion();
            }

        }, null, null);
    }

    private void PMsgTalkCSHandler(PMsgTalkCS pmsgTalkCS)
    {
        uint ErrorCode = (uint)pmsgTalkCS.UnResult;

        UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup", false);//SceneManager.instance.ChatPopup(false);
        if (chat == null)
            return;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == (int)Sw.ErrorCode.ER_MsgTalkCS_CD_Error)
            {
                (chat as ChatPopup).OnReciveChat(_LowDataMgr.instance.GetStringCommon(438), ChatType.System);
            }
            else if(ErrorCode == (int)Sw.ErrorCode.ER_MsgTalkCS_DestID_Error )//대상을 찾을 수 없음
            {
                //UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/FriendPanel");
                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
                if (friendPanel != null)
                {
                    (friendPanel as SocialPanel).OnReciveChat("[FF0000]" + _LowDataMgr.instance.GetStringCommon(908) + "[-]", false);
                }
                else
                {
                    (chat as ChatPopup).OnReciveChat("[FF0000]" + _LowDataMgr.instance.GetStringCommon(908) + "[-]", ChatType.Whisper);
                }
            }
            else
            {
                (chat as ChatPopup).OnReciveChat("[FF0000]ChatError: " + GetErrorString(ErrorCode) + "[-]", ChatType.System);
            }
        }
        else
        {
            if (pmsgTalkCS.UnChannel == (int)TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE ||
                pmsgTalkCS.UnChannel == (int)TALK_CHANNEL_TYPE.TALK_CHANNEL_MAP )//내가 보낸것이 일루옴
            {
                NetData._UserInfo user = NetData.instance.GetUserInfo();
                NetData.ChatData data;
                data.UserName = user._charName;
                data.GuildName = null;
                data.Msg = pmsgTalkCS.SzMsg;

                data.ClassId = (int)user._userCharIndex;
                data.Lv = (int)user._Level;
                data.VipLv = (int)user._VipLevel;
                data.GuildId = user._GuildId;
                data.UserUID = (long)user._charUUID;
                data.WhisperUID = pmsgTalkCS.UllDestID;//발신자의 아이디.

                (chat as ChatPopup).OnReciveChat(data, (ChatType)pmsgTalkCS.UnChannel);
            }
            else if (pmsgTalkCS.UnChannel == (int)TALK_CHANNEL_TYPE.TALK_CHANNEL_FRIEND)//내가 보낸것이 일루옴
            {
                StringBuilder str = new StringBuilder();
                str.Append(NetData.instance.Nickname);
                str.Append(": ");
                str.Append(pmsgTalkCS.SzMsg);

                string msg = str.ToString();

                List<string> list = null;
                if (SocialPanel.TalkHistory.TryGetValue(pmsgTalkCS.SzDestName, out list))
                    list.Add(msg);
                else
                {
                    list = new List<string>();
                    list.Add(msg);
                    SocialPanel.TalkHistory.Add(pmsgTalkCS.SzDestName, list);
                }

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
                if (friendPanel != null)
                    (friendPanel as SocialPanel).OnReciveChat(msg, true);
            }
            else if (pmsgTalkCS.UnChannel == (int)TALK_CHANNEL_TYPE.TALK_CHANNEL_SYSTEM)//내가 보낸것이 일루옴
            {
                string msg = pmsgTalkCS.SzMsg;
                (chat as ChatPopup).OnReciveChat(msg, ChatType.System);
            }
        }
    }

    private void PMsgTalkRecvSHandler(PMsgTalkRecvS pmsgTalkRecvS)
    {
        if (pmsgTalkRecvS.UnChannel == (int)TALK_CHANNEL_TYPE.TALK_CHANNEL_FRIEND)
        {
            char[] WhisperArr = SceneManager.instance.optionData.BlockWhisper.ToCharArray(); //귓말
            //친구채팅거부
            if (WhisperArr[1] == '2')
                return;

            StringBuilder str = new StringBuilder();
            str.Append(pmsgTalkRecvS.SzSourName);
            str.Append(": ");
            str.Append(pmsgTalkRecvS.SzMsg);
            string msg = str.ToString();

            List<string> list = null;
            if (SocialPanel.TalkHistory.TryGetValue(pmsgTalkRecvS.SzSourName, out list))
                list.Add(msg);
            else
            {
                list = new List<string>();
                list.Add(msg);
                SocialPanel.TalkHistory.Add(pmsgTalkRecvS.SzSourName, list);
            }

            UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
            if (friendPanel != null)
                (friendPanel as SocialPanel).OnReciveChat(msg, false);

            //return;
        }
        else
        {
            ChatType type = (ChatType)pmsgTalkRecvS.UnChannel;
            NetData.ChatData data;
            data.UserName = pmsgTalkRecvS.SzSourName;
            data.GuildName = pmsgTalkRecvS.SzGuildName;
            data.Msg = pmsgTalkRecvS.SzMsg;

            data.ClassId = pmsgTalkRecvS.UnRoleType;
            data.Lv = pmsgTalkRecvS.UnLevel;
            data.VipLv = pmsgTalkRecvS.UnVipLev;
            data.GuildId = pmsgTalkRecvS.UllGuildID;
            data.UserUID = data.Msg.Contains("[url=Item/") ? 0 : pmsgTalkRecvS.UllSourID;

            if (type == ChatType.Whisper)
                data.WhisperUID = pmsgTalkRecvS.UllSourID;
            else
                data.WhisperUID = 0;

            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup", false);
            if (chat != null)
                (chat as ChatPopup).OnReciveChat(data, type);
        }

    }

    private void PMsgTalkBlackListSHandler(PMsgTalkBlackListS pmsgTalkBlackListS)
    {
        
    }

    private void PMsgTalkAddBlackCS(PMsgTalkAddBlackCS pmsgTalkAddBlackCS)
    {
        
    }

    private void PMsgTalkDelBlackCSHandler(PMsgTalkDelBlackCS pmsgTalkDelBlackCS)
    {
        
    }

    /// <summary> 게임 로그인 성공. 서버에서 캐릭터 리스트를 받아온다. </summary>
    private void PMsgRoleListSHandler(PMsgRoleListS pmsgRoleListS)
    {
        for (int i = 0; i < pmsgRoleListS.CRoleInfo.Count; i++)
        {
            ulong c_usn = (ulong)pmsgRoleListS.CRoleInfo[i].UllRoleId;
            uint character_id = (uint)pmsgRoleListS.CRoleInfo[i].UnType;
            string nickname = pmsgRoleListS.CRoleInfo[i].SzName;
            short slot_no = (short)pmsgRoleListS.CRoleInfo[i].UnPos;
            ushort level = (ushort)pmsgRoleListS.CRoleInfo[i].UnLevel;
            uint skillSetId = 0;

            uint head = 0, cloth = 0, weapon = 0;
            for(int j=0; j < pmsgRoleListS.CRoleInfo[i].UnEquipmentId.Count; j++)
            {
                Item.EquipmentInfo info = _LowDataMgr.instance.GetLowDataEquipItemInfo( (uint)pmsgRoleListS.CRoleInfo[i].UnEquipmentId[j] );
                if (info == null)
                    continue;

                if (info.UseParts == (byte)ePartType.CLOTH)
                    cloth = info.Id;
                else if (info.UseParts == (byte)ePartType.WEAPON)
                    weapon = info.Id;
                else if (info.UseParts == (byte)ePartType.HELMET)
                    head = info.Id;
            }

            uint costumeId = (uint)pmsgRoleListS.CRoleInfo[i].UnCostumeId;
            if (costumeId == 0)
            {
                if (pmsgRoleListS.CRoleInfo[i].UnType == 11000)//권사
                    costumeId = 100;
                else if (pmsgRoleListS.CRoleInfo[i].UnType == 12000)//포졸
                    costumeId = 110;
                else if (pmsgRoleListS.CRoleInfo[i].UnType == 13000)//의사
                    costumeId = 120;
            }

            NetData.CharacterInfo charInfo = new NetData.CharacterInfo(
                c_usn, character_id, nickname, slot_no, level, cloth, head, weapon
                , costumeId, skillSetId
                , pmsgRoleListS.CRoleInfo[i].UnCostumeShowFlag == (int)Sw.COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE 
            );

            //아직 자세한 코스튬 정보까진 받아오지 않으므로 임시 설정
            NetData.CostumeInfo costumeInfo = new NetData.CostumeInfo();
            costumeInfo.costume_idx = costumeId;
            costumeInfo.c_usn = c_usn;

            NetData.instance.AddCharacter(charInfo, false);
            NetData.instance.AddCostume(costumeInfo, false);
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/LoginPanel");
        if (panel == null)
            return;

        (panel as LoginPanel).OnSuccessLogin();
    }

    private void PMsgRoleCreateNewSHandler(PMsgRoleCreateNewS pmsgRoleCreateNewS)
    {
        uint ErrorCode = (uint)pmsgRoleCreateNewS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgRoleCreateNewS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgRoleCreateNewS);

        ulong c_usn = (ulong)pmsgRoleCreateNewS.CRoleInfo.UllRoleId;
        uint character_id = (uint)pmsgRoleCreateNewS.CRoleInfo.UnType;
        string nickname = pmsgRoleCreateNewS.CRoleInfo.SzName;
        short slot_no = (short)pmsgRoleCreateNewS.CRoleInfo.UnPos;
        ushort level = (ushort)pmsgRoleCreateNewS.CRoleInfo.UnLevel;
        uint skillSetId = 0;

        uint head = 0, cloth = 0, weapon = 0;
        for (int j = 0; j < pmsgRoleCreateNewS.CRoleInfo.UnEquipmentId.Count; j++)
        {
            Item.EquipmentInfo info = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)pmsgRoleCreateNewS.CRoleInfo.UnEquipmentId[j]);
            if (info == null)
                continue;

            if (info.UseParts == (byte)ePartType.CLOTH)
                cloth = info.Id;
            else if (info.UseParts == (byte)ePartType.WEAPON)
                weapon = info.Id;
            else if (info.UseParts == (byte)ePartType.HELMET)
                head = info.Id;
        }

        uint costumeId = (uint)pmsgRoleCreateNewS.CRoleInfo.UnCostumeId;
        if (costumeId == 0)
        {
            if (pmsgRoleCreateNewS.CRoleInfo.UnType == 11000)//권사
                costumeId = 100;
            else if (pmsgRoleCreateNewS.CRoleInfo.UnType == 12000)//포졸
                costumeId = 110;
            else if (pmsgRoleCreateNewS.CRoleInfo.UnType == 13000)//의사
                costumeId = 120;
        }

        NetData.CharacterInfo charInfo = new NetData.CharacterInfo(
            c_usn, character_id, nickname, slot_no, level, cloth, head, weapon
            , costumeId, skillSetId
            , pmsgRoleCreateNewS.CRoleInfo.UnCostumeShowFlag == (int)Sw.COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE
        );

        //아직 자세한 코스튬 정보까진 받아오지 않으므로 임시 설정
        NetData.CostumeInfo costumeInfo = new NetData.CostumeInfo();
        costumeInfo.costume_idx = costumeId;
        costumeInfo.c_usn = c_usn;

        NetData.instance.AddCharacter(charInfo, false);
        NetData.instance.AddCostume(costumeInfo, false);

        //SendPMsgCostumeUserC((int)costumeInfo.costume_id, 1);

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/SelectHeroPanel");
        if (panel == null)
            return;

        (panel as SelectHeroPanel).OnCharCreateAndGameStart(costumeInfo.c_usn);
    }

    private void PMsgRoleDeleteSHandler(PMsgRoleDeleteS pmsgRoleDeleteS)
    {
        uint ErrorCode = (uint)pmsgRoleDeleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgRoleDeleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        
        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/SelectHeroPanel");
        if (panel == null)
            return;

        (panel as SelectHeroPanel).OnAnswerDeleteChar();
    }

    private void PMsgRoleSelectSHandler(PMsgRoleSelectS pmsgRoleSelectS)
    {
        uint ErrorCode = (uint)pmsgRoleSelectS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgRoleSelectS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //최초 로그인시체크해야할것들
        SendPMsgWelfareReturnQueryInfoC(); //복귀유저인가?
        SendPMsgLotteryQueryInfoC(); //무료뽑기시간인가?
        PMsgEmailQueryListC();//받은메일이잇는가?
        //NetworkClient.instance.PMsgEmailQueryListC();//받은메일이잇는가?
        SendPMsgFriendRequestFriendListC();//받은친구요청이 있는가
        SendPMsgStageChapterQueryC(1);//일반별보상
        SendPMsgStageChapterQueryC(2);//하드별보상

        SendPMsgAchieveQueryInfoC(); //업적정보조회

        //혜택쪽싸그리...
        SendPMsgSignInQueryInfoC();// 출석했니   
        SendPMsgWelfareXDayLoginQueryInfoC();//7일연속
        SendPMsgWelfareRoleUpgradeQueryInfoC();//렙업
    }

    private void PMsgRoleInfoSHandler(PMsgRoleInfoS pmsgRoleInfoS)
    {

        Debug.Log(pmsgRoleInfoS);

		/*
        //유저의 정보를 채워줘야한다.
        ulong c_idx = (ulong)pmsgRoleInfoS.UllRoleId;//캐릭터 고유 아이디
        ulong c_exp = (ulong)pmsgRoleInfoS.UllExp;//경험치

        uint c_level_ti = (uint)pmsgRoleInfoS.UnLevel; //레벨
        uint vlpLevel = (uint)pmsgRoleInfoS.UnVipLevel; //vip렙

        ulong cash = (ulong)pmsgRoleInfoS.UnGem;
        //ulong c_total_cash = 0;//이제까지 사용한 캐쉬 양.(VIP용)
        ulong gold = (ulong)pmsgRoleInfoS.UllCoins;//돈
        ulong energy = (ulong)pmsgRoleInfoS.UnPower;
        //string c_use_last_stamp = jsonCharInfo["last_use_energy_time"].str;//마지막으로 사용한 에너지 시간
        ulong fame = (ulong)pmsgRoleInfoS.UllFame;
        ulong contribution = (ulong)pmsgRoleInfoS.UllContribution;
        ulong badge = (ulong)pmsgRoleInfoS.UllRoyalBadge;
        ulong honor = (ulong)pmsgRoleInfoS.UllHonor;
        ulong heart = 0;
        ulong lion = (ulong)pmsgRoleInfoS.UllLionKingBadge;

        ulong c_attend = 0;//출석 횟수
        string c_attend_last = "";//마지막 출석일

        string c_nick_vc = pmsgRoleInfoS.SzName;//유저 캐릭터의 닉
        uint c_unit_table_id = (uint)pmsgRoleInfoS.UnType;//캐릭터 로우 데이터 아이디
        uint guildId = (uint)pmsgRoleInfoS.UnGuildId;
        bool isHideCostume = pmsgRoleInfoS.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE;
*/
        

		
        //이에 맞는 아이템 정의 필요
        //NetData.instance.GetUserInfo().SetPlayCharInfo(c_idx, c_level_ti, vlpLevel, c_exp, c_attend, c_attend_last, c_nick_vc, c_unit_table_id, (uint)pmsgRoleInfoS.UnAttack, guildId, contribution, isHideCostume);
		NetData.instance.GetUserInfo ().SetPlayCharInfo (pmsgRoleInfoS);
		NetData.instance.GetUserInfo ().SetPlayCharAsset (pmsgRoleInfoS);

        AtlasMgr.instance.Load();

        //코스튬까지만
		if (pmsgRoleInfoS.UnType == 11000)
        {
            ushort[] skillLevel = new ushort[5];
            skillLevel[0] = 1;
            skillLevel[1] = 1;
            skillLevel[2] = 1;
            skillLevel[3] = 1;
            skillLevel[4] = 1;

            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(100) == null)
                NetData.instance.GetUserInfo().CreateCostume(1, (uint)100, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(101) == null)
                NetData.instance.GetUserInfo().CreateCostume(2, (uint)101, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(102) == null)
                NetData.instance.GetUserInfo().CreateCostume(3, (uint)102, (ushort)0, 0, skillLevel, false, false);
            //if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(103) == null)
            //    NetData.instance.GetUserInfo().CreateCostume(4, (uint)103, (ushort)0, 0, skillLevel, false, false);
        }
		else if (pmsgRoleInfoS.UnType == 12000)
        {
            ushort[] skillLevel = new ushort[5];
            skillLevel[0] = 1;
            skillLevel[1] = 1;
            skillLevel[2] = 1;
            skillLevel[3] = 1;
            skillLevel[4] = 1;

            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(110) == null)
                NetData.instance.GetUserInfo().CreateCostume(1, (uint)110, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(111) == null)
                NetData.instance.GetUserInfo().CreateCostume(2, (uint)111, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(112) == null)
                NetData.instance.GetUserInfo().CreateCostume(3, (uint)112, (ushort)0, 0, skillLevel, false, false);
        }
		else if (pmsgRoleInfoS.UnType == 13000)
        {
            ushort[] skillLevel = new ushort[5];
            skillLevel[0] = 1;
            skillLevel[1] = 1;
            skillLevel[2] = 1;
            skillLevel[3] = 1;
            skillLevel[4] = 1;

            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(120) == null)
                NetData.instance.GetUserInfo().CreateCostume(1, (uint)120, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(121) == null)
                NetData.instance.GetUserInfo().CreateCostume(2, (uint)121, (ushort)0, 0, skillLevel, false, false);
            if (NetData.instance.GetUserInfo().GetCostumeDataForLowDataID(122) == null)
                NetData.instance.GetUserInfo().CreateCostume(3, (uint)122, (ushort)0, 0, skillLevel, false, false);
        }

        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        byte partnerType = 0;
        //uint defaultId = 0;
        List<Partner.PartnerDataInfo> dataList = _LowDataMgr.instance.GetPartnerDataToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            //if (userInfo.GetPartnerForDataID(dataList[i].Id) != null)
            if(userInfo.GetPartnerForType(dataList[i].Type) )//해당 클래스 타입이 이미있는지 검사
                continue;

            if (dataList[i].Type != partnerType)
            {
                partnerType = dataList[i].Type;
                //defaultId = dataList[i].Id;
                userInfo.CreatePartner((ulong)partnerType + 10000, (ushort)dataList[i].Id, 1, 0, 0, 0, 0, 0, 0, false);//원본 추후에 서버에서 재작업완료되면 이거 살려야함
            }
        }

        _guideID = (ulong)pmsgRoleInfoS.UllGuide;

        SceneManager.instance.CheckTutorial();
        SendPMsgCostumeQueryC();
    }

    private void PMsgMapEnterMapSHander(PMsgMapEnterMapS pmsgMapEnterMapS)
    {
        //맵입장시(마을등) 서버에서 보내줌
        Debug.Log(pmsgMapEnterMapS);

        mapID = pmsgMapEnterMapS.UnMapId;

        _regenX = pmsgMapEnterMapS.UnPosX;
        _regenY = pmsgMapEnterMapS.UnPosY;

        SceneManager.instance.TownAction();

        // 난투장 나가고 여기들어옴 이경우도생각좀 난투장패널을 열ㅇㅓ줘야하는가

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/SelectHeroPanel");
        if (panel == null)
            return;

        //(panel as SelectHeroPanel).Close();
        (panel as SelectHeroPanel).OnClosePanel();
    }

    private void PMsgMapLeaveMapSHander(PMsgMapLeaveMapS pmsgMapLeaveMapS)
    {
        if (TownState.TownActive)
        {
            SceneManager.instance.GetState<TownState>().TownUserLeave(pmsgMapLeaveMapS);
        }
    }

    private void PMsgNpcInfoSHander(PMsgNpcInfoS pmsgNpcInfoS)
    {   //NPC의 정보에 대한 데이터들
    }

    private void PMsgMapRoleInfoSHandler(PMsgMapRoleInfoS pmsgMapRoleInfoS)
    {
        //새로운 유저가 이맵에 들어왔다.

        if (TownState.TownActive)
        {
            SceneManager.instance.GetState<TownState>().TownUserEnterAsync(pmsgMapRoleInfoS);
        }
        else
        {
            townUnitLoadingList.Add(pmsgMapRoleInfoS);
        }
    }

    private void PMsgMapMoveCSHanler(PMsgMapMoveCS pmsgMapMoveCS)
    {
        //내이동이 정상적으로 되었을시
        uint ErrorCode = (uint)pmsgMapMoveCS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            //UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgMapMoveCS.GetType().ToString() + "-" + GetErrorString(ErrorCode) + "-" + pmsgMapMoveCS);
            return;
        }
    }

    private void PMsgMapMoveRecvSHandler(PMsgMapMoveRecvS pmsgMapMoveRecvS)
    {
        //다른 유저의 이동을 브로드캐스팅 받을시
        if (TownState.TownActive)
        {
            TownUnit tu = SceneManager.instance.GetState<TownState>().GetTownUnit(pmsgMapMoveRecvS.UllRoleId);

            if (tu != null)
            {
                if(pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].FData1 != 0 && pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].FData2 != 0)
                {
                    //Vector3 pos = NaviTileInfo.instance.GetTilePos(pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].UnPosX, pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].UnPosY);

                    Vector3 pos = new Vector3(pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].FData1, 0f, pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].FData2);

                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(pos, out navHit, 20f, 9))
                    {
                        pos = navHit.position;
                    }

                    if (Vector3.Distance(pos, tu.transform.position) > 5.0)
                    {
                        tu.navAgent.Warp(pos);
                        return;
                    }
                    float moveSpeed = 1f;
                    float dist = Vector3.Distance(pos, tu.transform.position);
                    if (dist > 3)
                    {
                        moveSpeed = 1f * (dist / 3);
                    }
                    tu.MovePosition(pos, moveSpeed);
                }
                else
                {
                    Vector3 pos = NaviTileInfo.instance.GetTilePos(pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].UnPosX, pmsgMapMoveRecvS.CMapPos[pmsgMapMoveRecvS.CMapPos.Count - 1].UnPosY);

                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(pos, out navHit, 5f, 9))
                    {
                        pos = navHit.position;
                    }

                    if (Vector3.Distance(pos, tu.transform.position) > 5.0)
                    {
                        tu.navAgent.Warp(pos);
                        return;
                    }
                    float moveSpeed = 1f;
                    float dist = Vector3.Distance(pos, tu.transform.position);
                    if (dist > 3)
                    {
                        moveSpeed = 1f * (dist / 3);
                    }
                    tu.MovePosition(pos, moveSpeed);
                }
            }
        }

    }


	
	/// <summary> 장비아이템 정보 갱신 및 추가 </summary>
	private void PMsgEquipmentQuerySHandler(PMsgEquipmentQueryS pmsgEquipmentQueryS)
	{
		SceneManager.instance.EndNetProcess("PMsgEquipmentQueryC");
        Debug.Log("pmsgEquipmentQueryS " + pmsgEquipmentQueryS);
		for (int i = 0; i < pmsgEquipmentQueryS.UnEquipmentCount; i++)
        {
            EquipmentInfo equip = pmsgEquipmentQueryS.CEquipmentInfo[i];

            if (equip.UnPosition == 0)//미 장착 아이템
            {
                //장착중이었던 아이템인가 확인
                if (NetData.instance.GetUserInfo().isEquipItem((ulong)equip.UnId))
                {
                    if (!NetData.instance.GetUserInfo().isHaveItem((ulong)equip.UnId))
                    {
                        //아예없는것인가 체크 - 새로생성
                        NetData._ItemData itemdata = NetData.instance.GetUserInfo().CreateEquipItem((ulong)equip.UnId, (uint)equip.UnType, (ushort)equip.UnEnchantTime, 0, 0, (uint)equip.UnAttack);
                        if (itemdata == null)
                            continue;

                        NetData.instance.GetUserInfo().ItemEquip((ulong)equip.UnId);
                        NetData.instance.GetUserInfo().RefreshTotalAttackPoint();

						PMsgEquipmentQuerySHandlerSub_ParseAbility(itemdata.StatList, equip);
                        //itemdata.EvolveAndEnchantApply();
                    }
                    else
                    {
                        //있기는있으면 장착 해제
                        NetData.instance.GetUserInfo().ItemUnEquip((ulong)equip.UnId);
                        NetData.instance.GetUserInfo().RefreshTotalAttackPoint();
                    }
                }
                else
                {
                    //장착중은 아니였던 아이템 - 아이템이 있나 확인
                    NetData._ItemData existItem = NetData.instance.GetUserInfo().GetItemDataForIndexAndType((ulong)equip.UnId, (byte)eItemType.EQUIP);
                    if (existItem != null)
                    {
                        //정보갱신
                        existItem._equipitemDataIndex = (uint)equip.UnType;
                        existItem._enchant = (ushort)equip.UnEnchantTime;
                        //existItem._Grade = (ushort)equip.UnEvolveTime;
                        //existItem._MinorGrade = (ushort)equip.UnEvolveStar;
                        existItem._Attack = (uint)equip.UnAttack;

                        existItem.StatList.Clear();

						PMsgEquipmentQuerySHandlerSub_ParseAbility(existItem.StatList, equip);
                        //existItem.EvolveAndEnchantApply();
                    }
                    else
                    {
                        NetData._ItemData itemdata = NetData.instance.GetUserInfo().CreateEquipItem((ulong)equip.UnId, (uint)equip.UnType, (ushort)equip.UnEnchantTime, 0, 0, (uint)equip.UnAttack);
                        if (itemdata == null)
                            continue;

						PMsgEquipmentQuerySHandlerSub_ParseAbility(itemdata.StatList, equip);
                        //itemdata.EvolveAndEnchantApply();
                        
                        bool isLoadingPanel = SceneManager.instance.IsShowLoadingPanel();
                        if (!isLoadingPanel && SceneManager.instance.CurrState() != _STATE.LOGIN)//아이템 추가된곳
                        {
                            //SceneManager.instance.AddRecommendData(itemdata);
                            string msg = string.Format(_LowDataMgr.instance.GetStringCommon(832), itemdata.GetLocName());
                            UIMgr.AddLogChat(msg);

                        }
                    }
                }
            }


            if (equip.UnPosition == 1)//장착 아이템
            {
                //장착중이었던 아이템인가 확인
                if (!NetData.instance.GetUserInfo().isEquipItem((ulong)equip.UnId))
                {
                    //!미장착중이었던 아이템

                    if (!NetData.instance.GetUserInfo().isHaveItem((ulong)equip.UnId))
                    {
                        //아예없는것인가 체크 - 새로생성
                        NetData._ItemData itemdata = NetData.instance.GetUserInfo().CreateEquipItem((ulong)equip.UnId, (uint)equip.UnType, (ushort)equip.UnEnchantTime, 0,0, (uint)equip.UnAttack);
                        if (itemdata == null)
                            continue;

                        NetData.instance.GetUserInfo().ItemEquip((ulong)equip.UnId);

						PMsgEquipmentQuerySHandlerSub_ParseAbility(itemdata.StatList, equip);
                        //itemdata.EvolveAndEnchantApply();

                    }
                    else//있기는있으면 장착
                        NetData.instance.GetUserInfo().ItemEquip((ulong)equip.UnId);

                    NetData.instance.GetUserInfo().RefreshTotalAttackPoint();
                }
                else
                {
                    NetData._ItemData existItem = NetData.instance.GetUserInfo().GetEquipPartsForItemID((ulong)equip.UnId);
                    if (existItem != null)
                    {
                        //정보갱신
                        existItem._equipitemDataIndex = (uint)equip.UnType;
                        existItem._enchant = (ushort)equip.UnEnchantTime;
                        //existItem._Grade = (ushort)equip.UnEvolveTime;
                        //existItem._MinorGrade = (ushort)equip.UnEvolveStar;
                        existItem._Attack = (uint)equip.UnAttack;

                        existItem.StatList.Clear();

						PMsgEquipmentQuerySHandlerSub_ParseAbility(existItem.StatList, equip);
                        //existItem.EvolveAndEnchantApply();


                    }
                    else
                    {
                        NetData._ItemData itemdata = NetData.instance.GetUserInfo().CreateEquipItem((ulong)equip.UnId, (uint)equip.UnType, (ushort)equip.UnEnchantTime, 0, 0, (uint)equip.UnAttack);
                        NetData.instance.GetUserInfo().ItemEquip((ulong)equip.UnId);
                        NetData.instance.GetUserInfo().RefreshTotalAttackPoint();

						PMsgEquipmentQuerySHandlerSub_ParseAbility(itemdata.StatList, equip);
                        //itemdata.EvolveAndEnchantApply();

                    }
                }
            }
        }
    }

	private void PMsgEquipmentQuerySHandlerSub_ParseAbility(List<NetData.ItemAbilityData> _StatList, EquipmentInfo equip){

        if (0 < equip.UnBasicValue.Count )
		{
			Item.EquipmentInfo defaultAbility = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equip.UnType);
			for(int i=0; i < equip.UnBasicValue.Count; i++)
            {
                if(0 < equip.UnBasicOption[i])
                    PMsgEquipmentQuerySHandlerSub_AddAbility(_StatList, (uint)equip.UnBasicOption[i], equip.UnBasicValue[i]);
            }

			//if (defaultAbility != null)
			//{
			//	PMsgEquipmentQuerySHandlerSub_AddAbility(_StatList, defaultAbility.BasicOptionIndex, equip.UnBasicValue);
			//}
		}
		
		//if (equip.UnRandomOption1 != 0)
		//{
		//	PMsgEquipmentQuerySHandlerSub_AddAbility(_StatList, (uint)equip.UnRandomOption1, equip.UnRandomValue1);
		//}
		
		//if (equip.UnRandomOption2 != 0)
		//{
		//	PMsgEquipmentQuerySHandlerSub_AddAbility(_StatList, (uint)equip.UnRandomOption2, equip.UnRandomValue2);
		//}
		
	}
	
	private void PMsgEquipmentQuerySHandlerSub_AddAbility(List<NetData.ItemAbilityData> _StatList, uint optionIndex, int UnBasicValue){
		
		Item.ItemValueInfo itemValue = _LowDataMgr.instance.GetLowDataItemValueInfo(optionIndex);
		
		if (itemValue != null)
		{
			NetData.ItemAbilityData abilityData = new NetData.ItemAbilityData();
			abilityData.Ability = (AbilityType)itemValue.OptionId;
			abilityData.Value = UnBasicValue;//(int)equip.UnBasicValue;
			
			_StatList.Add(abilityData);
		}
	}




    /// <summary> 아이템 개수 증가or 하락 응답 </summary>
    private void PMsgItemQuerySHandler(PMsgItemQueryS pmsgItemQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgItemQueryC");
        for (int i = 0; i < pmsgItemQueryS.UnItemCount; i++)
        {
            ItemInfo item = pmsgItemQueryS.CItemInfo[i];

            NetData._ItemData existitem = NetData.instance.GetUserInfo().GetItemDataForIndexAndType((ulong)item.UnId, (byte)eItemType.USE);

            if (existitem == null)//PMsgPMsgSynNewItemSHandler 추가로 인해 최초 장비리스트를 받을때를 제외하고는 신규장비는 저기로온다.
            {
                existitem = NetData.instance.GetUserInfo().CreateUseItem((ulong)item.UnId, (uint)item.UnType, (ushort)item.UnNum, false);

                if (!SceneManager.instance.IsShowLoadingPanel())
                {
                    string msg = string.Format(_LowDataMgr.instance.GetStringCommon(832), existitem.GetLocName());
                    UIMgr.AddLogChat(msg);

                    //신규 소비아이템
                    existitem.IsNewItem = true;
                    //SceneManager.instance.AddNewItemId(existitem._itemIndex);

                    Item.ItemInfo lowData = existitem.GetUseLowData();
                    bool isShard = (lowData.Type == (byte)AssetType.CostumeShard || lowData.Type == (byte)AssetType.PartnerShard);
                    SceneManager.instance.SetNoticePanel(NoticeType.GetItem, lowData.Icon, null, isShard);
                }

            }
            else
            {
                //갯수 업데이트
                existitem.Count = (ushort)item.UnNum;
            }
        }
    }

    /// <summary> 소비, 장비 삭제 시 응답. </summary>
    private void PMsgEquipmentDelSHandler(PMsgEquipmentDelS pmsgEquipmentDelS)
    {
        NetData.instance.GetUserInfo().RemoveEquipTypeItem((ulong)pmsgEquipmentDelS.UnId);
    }
    /*
    /// <summary> 장비 장착or해제 응답. </summary>
    private void PMsgEquipmentUserSHanler(PMsgEquipmentUserS pmsgEquipmentUserS)
    {
        uint ErrorCode = (uint)pmsgEquipmentUserS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEquipmentUserS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        ulong itemId = (ulong)pmsgEquipmentUserS.UnId;
        UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        if (pmsgEquipmentUserS.UnType == 1)
        {
            //장착
            //NetData.instance.GetUserInfo().ItemEquip(itemId);
            if (heroPanel != null)
                (heroPanel as HeroPanel).OnEquipItem(true, itemId);
        }
        else if (pmsgEquipmentUserS.UnType == 2)
        {
            //해제
            if (heroPanel != null)
                (heroPanel as HeroPanel).OnEquipItem(false, itemId);
        
            //NetData.instance.GetUserInfo().ItemUnEquip(itemId);
        }

        SceneManager.instance.SetNoticePanel(NoticeType.PowerUp);
    }
    */
    private void PMsgItemDelSHandler(PMsgItemDelS pmsgItemDelS)
    {
        NetData.instance.GetUserInfo().RemoveUseTypeItem((ulong)pmsgItemDelS.UnId);
    }

    private void PMsgCostumeQuerySHandler(PMsgCostumeQueryS pmsgCostumeQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgCostumeQueryC");
        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        for (int i = 0; i < pmsgCostumeQueryS.CCostumeInfo.Count; i++)
        {
            NetData._CostumeData costume = userInfo.GetCostumeDataForLowDataID((uint)pmsgCostumeQueryS.CCostumeInfo[i].UnType);
            if (costume == null)
            {
                ushort[] skillLevel = new ushort[5];
                /*
                skillLevel[0] = 1;
                skillLevel[1] = 1;
                skillLevel[2] = 1;
                skillLevel[3] = 1;
                skillLevel[4] = 1;
                */

                skillLevel[0] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[0];
                skillLevel[1] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[1];
                skillLevel[2] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[2];
                skillLevel[3] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[3];
                skillLevel[4] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[4];


                userInfo.CreateCostume(costume._costumeIndex,
                                                                (uint)pmsgCostumeQueryS.CCostumeInfo[i].UnType,
                                                                (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnEvolveTime,
                                                                (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnEvolveStar,
                                                                skillLevel,
                                                                false,
                                                                true);
            }
            else
            {
                costume._costumeIndex = (ulong)pmsgCostumeQueryS.CCostumeInfo[i].UnId;
                costume._isOwn = true;
                costume._MinorGrade = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnEvolveStar;
                costume._Grade = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnEvolveTime;

                ushort[] skillLevel = new ushort[5];
                skillLevel[0] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[0];
                skillLevel[1] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[1];
                skillLevel[2] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[2];
                skillLevel[3] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[3];
                skillLevel[4] = (ushort)pmsgCostumeQueryS.CCostumeInfo[i].UnSkillLevel[4];

                costume._skillLevel = skillLevel;

                //소켓 셋팅
                //costume._equipJewelList.Clear();
                IList<int> tokenList = pmsgCostumeQueryS.CCostumeInfo[i].UnToken;
                int tokenCount = tokenList.Count;
                for (int j = 0; j < tokenCount; j++)
                {
                    uint lowDataId = (uint)tokenList[j];
                    if (lowDataId <= 0)
                        continue;

                    costume._EquipJewelLowId[j] = lowDataId;
                }

                if (pmsgCostumeQueryS.CCostumeInfo[i].UnPosition == 0)
                {
                    costume.unEquipCostume();
                }
                else if (pmsgCostumeQueryS.CCostumeInfo[i].UnPosition == 1)
                {
                    costume.EquipCostume();

                    TownState state = SceneManager.instance.GetState<TownState>();

                    if (state != null && state.MyHero != null)
                    {
                        state.MyHero.SetChangeSkin(true);
                        state.MyHero.ChangeSkin();
                    }
                }
            }
        }
        
        if (_firstLogin)
        {
            if (_guideID == 0)
            {
                SendPMsgCostumeShowFlagC(true);//강제로 감춰놓는다.
                SceneManager.instance.TutorialAction();
            }
            else
            {
                SendPMsgLoginOverC();
                _firstLogin = false;
            }
        }
    }

    /// <summary> 장비 승급 응답 </summary>
    private void PMsgEquipmentEvolveSHandler(PMsgEquipmentEvolveS pmsgEquipmentEvolveS)
    {
        if (pmsgEquipmentEvolveS.UnErrorCode != (int)Sw.ErrorCode.ER_success)//에러
        {
            //if (pmsgEquipmentEvolveS.UnErrorCode == (int)Sw.ErrorCode.ER_EquipmentEvolveS_Material_Num_Error)
            //{
            //    UIMgr.instance.AddErrorPopup((int)ErrorCode);(-5999, null, null, null);//재료 부족
            //}
            //if (pmsgEquipmentEvolveS.UnErrorCode == (int)Sw.ErrorCode.ER_EquipmentEvolveS_Max_Level)
            //{
            //    UIMgr.instance.AddErrorPopup((int)ErrorCode);(-5996, null, null, null);//최대 레벨
            //}
            //if (pmsgEquipmentEvolveS.UnErrorCode == (int)Sw.ErrorCode.ER_EquipmentEvolveS_Coin_Num_Error)
            //{
            //    UIMgr.instance.AddErrorPopup((int)ErrorCode);(-1002, null, null, null);//재화 부족
            //}

            UIMgr.instance.AddErrorPopup((int)pmsgEquipmentEvolveS.UnErrorCode);
            return;
        }

        //성공시는 이펙트 출력
        NetData._ItemData itemData = NetData.instance.GetUserInfo().GetEquipItemForIdx((ulong)pmsgEquipmentEvolveS.UnId);
        if (itemData == null)
            itemData = NetData.instance.GetUserInfo().GetItemDataForIndexAndType((ulong)pmsgEquipmentEvolveS.UnId, (byte)eItemType.EQUIP);

        UIBasePanel equipPanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (equipPanel != null)
            equipPanel.NetworkData(MSG_DEFINE._MSG_EQUIPMENT_EVOLVE_C, itemData);
        //UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        //if (heroPanel != null)
        //{
        //    (heroPanel as HeroPanel).OnReceiveEvolveResult(itemData);
        //}
    }

    /// <summary> 재화 및 캐릭터 관련 업데이트 </summary>
    private void PMsgRoleAttributeSHander(PMsgRoleAttributeS pmsgRoleAttributeS)
    {

        //일부 당장쓰이는거몇개만 해둠
        Debug.Log(string.Format("Add Type = {0}, Value = {1}", (ROLE_ATTRIBUTE_TYPE)pmsgRoleAttributeS.UnAttribType, (ulong)pmsgRoleAttributeS.UllValue));
        if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_LEVEL) {
			uint level = NetData.instance.UserLevel;
			NetData.instance.GetUserInfo ()._Level = (uint)pmsgRoleAttributeS.UllValue;
			if (0 < level && 0 < (NetData.instance.UserLevel - level)) {
				if (TownState.TownActive) {//레벨업함.
                    UIBasePanel townPanel = UIMgr.GetUIBasePanel ("UIPanel/TownPanel");
					if (townPanel != null ) {//&& townPanel.gameObject.activeSelf
                        (townPanel as TownPanel).LevelUp();
                    }

                    SceneManager.instance.SetNoticePanel(NoticeType.LevelUp);
                }
                
                string msg = string.Format (_LowDataMgr.instance.GetStringCommon (831), NetData.instance.UserLevel);
				UIMgr.AddLogChat (msg);
			}

		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_EXP) {
			NetData.instance.GetUserInfo ()._exp = (ulong)pmsgRoleAttributeS.UllValue;
			if (TownState.TownActive) {
				UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
				if (townPanel != null)
					(townPanel as TownPanel).RefreshUserInfo (0);
			}
		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_COIN) {
			ulong prevGold = NetData.instance.GetAsset (AssetType.Gold);
			NetData.instance.SetAsset (AssetType.Gold, (ulong)pmsgRoleAttributeS.UllValue);
			long getGgld = pmsgRoleAttributeS.UllValue - (long)prevGold;

			if (TownState.TownActive) {
				UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
				if (townPanel != null)
					(townPanel as TownPanel).RefreshUserInfo (3/*,(long)prevGold*/);
			}

			if (0 < getGgld) {
				string msg = string.Format (_LowDataMgr.instance.GetStringCommon (836), getGgld, pmsgRoleAttributeS.UllValue);
				UIMgr.AddLogChat (msg);
			}
		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_GEM) {
			ulong prev = NetData.instance.GetAsset (AssetType.Cash);
			NetData.instance.SetAsset (AssetType.Cash, (ulong)pmsgRoleAttributeS.UllValue);
			if (TownState.TownActive) {
				UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
				if (townPanel != null)
					(townPanel as TownPanel).RefreshUserInfo (5);
			}

			long get = pmsgRoleAttributeS.UllValue - (long)prev;
			if (0 < get) {
				string msg = string.Format (_LowDataMgr.instance.GetStringCommon (837), get, pmsgRoleAttributeS.UllValue);
				UIMgr.AddLogChat (msg);
			}
		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_POWER) {
			NetData.instance.SetAsset (AssetType.Energy, (ulong)pmsgRoleAttributeS.UllValue);
			if (TownState.TownActive) {
				UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
				if (townPanel != null)
					(townPanel as TownPanel).RefreshUserInfo (4);
			}
		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_ATTACK) {
            //uint totalAtt =  NetData.instance.GetUserInfo ().GetEquipAttack (false);
           //UIMgr.instance.PrevAttack = (int)NetData.instance.GetUserInfo().RefreshTotalAttackPoint(); //(int)NetData.instance.GetUserInfo ()._TotalAttack;

            NetData.instance.GetUserInfo ()._TotalAttack = (uint)pmsgRoleAttributeS.UllValue; // base attack + item attack + costume attack


//			if (TownState.TownActive && totalAtt != 0) {//최초일 경우 0이겠지
//				UIBasePanel basePanel = UIMgr.instance.GetFirstUI ();
//				if (basePanel != null) {
//					//Vector3 worldPos = Vector3.zero;
//					//worldPos.y = basePanel.transform.position.y;
//					Vector3 worldPos = Vector3.zero;
//					UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
//					if (townPanel != null && townPanel.gameObject.activeSelf)
//						worldPos = townPanel.transform.FindChild ("lv_up_root").position;
//					else
//						worldPos.y = UIMgr.instance.GetFirstUI ().transform.position.y;
//
//                    //UIMgr.OpenStatEffPopup(/*(int)totalAtt*/ 0.5f, _LowDataMgr.instance.GetStringCommon(47), worldPos);
//                }
//            }
            
			NetData.instance.GetUserInfo ().RefreshTotalAttackPoint (true);

		} else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_BASE_ATTACK) {


			// 레벨업시 
			// ROLE_ATTRIBUTE_TYPE_ATTACK
			// ROLE_ATTRIBUTE_TYPE_BASE_ATTACK
			// 순서로 들어온다
			uint totalAtt = (uint)NetData.instance.GetUserInfo ()._TotalAttack;
			//UIMgr.instance.PrevAttack = (int)NetData.instance.GetUserInfo().RefreshTotalAttackPoint(); //(int)NetData.instance.GetUserInfo ()._TotalAttack;

            NetData.instance.GetUserInfo ()._BaseAttack = (uint)pmsgRoleAttributeS.UllValue;
			//NetData.instance.GetUserInfo ().RefreshTotalAttackPoint (false);

			if (totalAtt != 0) {//최초일 경우 0이겠지//TownState.TownActive && 
				//UIBasePanel basePanel = UIMgr.instance.GetFirstUI ();
				//if (basePanel != null) {
                    UIMgr.instance.PrevAttack = (int)totalAtt;
     //               Vector3 worldPos = Vector3.zero;
					//UIBasePanel townPanel = UIMgr.GetTownBasePanel ();
					//if (townPanel != null && townPanel.gameObject.activeSelf)
					//	worldPos = townPanel.transform.FindChild ("lv_up_root").position;
					//else
					//	worldPos.y = UIMgr.instance.GetFirstUI ().transform.position.y;
				//}
			}

			NetData.instance.GetUserInfo ().RefreshTotalAttackPoint (true);
            if(TownState.TownActive)
                SceneManager.instance.SetNoticePanel(NoticeType.PowerUp);
        }
        else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_ROYAL_BADGE)//휘장
        {
            ulong prev = NetData.instance.GetAsset(AssetType.Badge);
            NetData.instance.SetAsset(AssetType.Badge, (ulong)pmsgRoleAttributeS.UllValue);

            if (prev <= 0 && NetData.instance.GetUserInfo().GetItemForItemID(599002, (byte)eItemType.USE) == null)//가라 휘장
                NetData.instance.GetUserInfo().CreateUseItem((ulong)599002, (uint)599002, (ushort)0, false);

            long get = pmsgRoleAttributeS.UllValue - (long)prev;
            if (0 < get)
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(839), get, pmsgRoleAttributeS.UllValue);
                UIMgr.AddLogChat(msg);
            }
        }
        else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_FAME)//성망
        {
            ulong prev = NetData.instance.GetAsset(AssetType.FAME);
            NetData.instance.SetAsset(AssetType.FAME, (ulong)pmsgRoleAttributeS.UllValue);

            if (prev <= 0 && NetData.instance.GetUserInfo().GetItemForItemID(599003, (byte)eItemType.USE) == null)//가라 성망
            {
                NetData.instance.GetUserInfo().CreateUseItem((ulong)599003, (uint)599003, (ushort)0, false);
            }

            long get = pmsgRoleAttributeS.UllValue - (long)prev;
            if (0 < get)
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(838), get, pmsgRoleAttributeS.UllValue);
                UIMgr.AddLogChat(msg);
            }
        }
        else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_HONOR)//명예
        {
            ulong prev = NetData.instance.GetAsset(AssetType.Honor);
            NetData.instance.SetAsset(AssetType.Honor, (ulong)pmsgRoleAttributeS.UllValue);

            long get = pmsgRoleAttributeS.UllValue - (long)prev;
            if (0 < get)
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(841), get, pmsgRoleAttributeS.UllValue);
                UIMgr.AddLogChat(msg);
            }
        }
        else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_CONTRIBUTION)//공헌
        {
            ulong prev = NetData.instance.GetAsset(AssetType.Contribute);
            NetData.instance.SetAsset(AssetType.Contribute, (ulong)pmsgRoleAttributeS.UllValue);

            long get = pmsgRoleAttributeS.UllValue - (long)prev;
            if (0 < get)
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(840), get, pmsgRoleAttributeS.UllValue);
                UIMgr.AddLogChat(msg);
            }
        }
        else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_LION_KING_BADGE)
        {
            NetData.instance.SetAsset(AssetType.LionBadge, (ulong)pmsgRoleAttributeS.UllValue);
        }
        //else if (pmsgRoleAttributeS.UnAttribType == (int)ROLE_ATTRIBUTE_TYPE.ROLE_ATTRIBUTE_TYPE_POWER)//이거 위에 동일한거 있는데 주석처리?
        //{
        //    NetData.instance.SetAsset(AssetType.Energy, (ulong)pmsgRoleAttributeS.UllValue);
        //}
    }

    /// <summary> 아이템 강화 </summary>
    private void PMsgEquipmentEnchantSHandler(PMsgEquipmentEnchantS pmsgEquipmentEnchantS)
    {
        SceneManager.instance.EndNetProcess("EquipEnchant");
        //UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");

        if (pmsgEquipmentEnchantS.UnErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)pmsgEquipmentEnchantS.UnErrorCode);

            Debug.Log("Enchant ErrorCode " + (ErrorCode)pmsgEquipmentEnchantS.UnErrorCode);
            //if (heroPanel != null)
            //    (heroPanel as HeroPanel).OnEndAutoEnchant();

            return;
        }

        //강화 완료 처리
        NetData._ItemData itemData = NetData.instance.GetUserInfo().GetEquipItemForIdx((ulong)pmsgEquipmentEnchantS.UnId);
        if (itemData == null)//보유 리스트에서 다시 찾는다.
            itemData = NetData.instance.GetUserInfo().GetItemDataForIndexAndType((ulong)pmsgEquipmentEnchantS.UnId, (byte)eItemType.EQUIP);

        UIBasePanel equipPanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (equipPanel != null)
            equipPanel.NetworkData(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_C, itemData);
        //if (heroPanel != null)
        //    (heroPanel as HeroPanel).OnReceiveEnchant(itemData);

    }
    /*
    /// <summary> 아이템 분해 </summary>
    private void PMsgEquipmentBreakSHandler(PMsgEquipmentBreakS pmsgEquipmentBreakS)
    {
        uint ErrorCode = (uint)pmsgEquipmentBreakS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEquipmentBreakS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        if (heroPanel != null)
            (heroPanel as HeroPanel).OnReceiveDisassemble(pmsgEquipmentBreakS.UnId[0]);
    }
    */
    /// <summary> 아이템 합성 </summary>
    private void PMsgItemFusionSHandler(PMsgItemFusionS pmsgItemFusionS)
    {
        uint ErrorCode = (uint)pmsgItemFusionS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgItemFusionS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        //if (heroPanel != null)
        //    (heroPanel as HeroPanel).OnReceiveFusion();
    }

    /// <summary> 코스튬 획득 </summary>
    private void PMsgCostumeFusionSHandler(PMsgCostumeFusionS pmsgCostumeFusionS)
    {
        uint ErrorCode = (uint)pmsgCostumeFusionS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCostumeFusionS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData._CostumeData costumeData = NetData.instance.GetUserInfo().GetCostumeDataForLowDataID((uint)pmsgCostumeFusionS.UnType);
        costumeData._MinorGrade = 0;
        costumeData._Grade = 0;
        //costumeData._skillLevel = 0;
        //costumeData._isEquip = true;
        costumeData._isOwn = true;

        UIBasePanel costumePanel = UIMgr.GetUIBasePanel("UIPanel/CostumePanel");
        if (costumePanel != null)
        {
            (costumePanel as CostumePanel).OnReceiveGetCostume(costumeData);
        }
    }

    /// <summary> 코스튬 승급 </summary>
    private void PMsgCostumeEvolveSHandler(PMsgCostumeEvolveS pmsgCostumeEvolveS)
    {
        SceneManager.instance.EndNetProcess("CostumeEvolve");
        uint ErrorCode = (uint)pmsgCostumeEvolveS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 

            Debug.Log(pmsgCostumeEvolveS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData._CostumeData costumeData = NetData.instance.GetUserInfo().GetCostumeDataForLowDataID((uint)pmsgCostumeEvolveS.UnType);
        UIBasePanel costumePanel = UIMgr.GetUIBasePanel("UIPanel/CostumePanel");
        if (costumePanel != null)
        {
            (costumePanel as CostumePanel).OnReceiveEvolve(costumeData);
        }
    }

    /// <summary> 코스튬 장착 응답. </summary>
    private void PMsgCostumeUserSHandler(PMsgCostumeUserS pmsgCostumeUserS)
    {
        uint ErrorCode = (uint)pmsgCostumeUserS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCostumeUserS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(string.Format("PMsgCostumeUserS - {0}, IsMount : {1}", pmsgCostumeUserS, pmsgCostumeUserS.UnRoleFlag == 1 ? "true" : "false"));

        NetData.instance.GetUserInfo().EquipCostume((ulong)pmsgCostumeUserS.UnId);//알아서 장착시키고 기존꺼는 해제시킴.
        if (pmsgCostumeUserS.UnRoleFlag == 1)//장착
        {
            NetData._CostumeData costumeData = NetData.instance.GetUserInfo().GetCostumeForIndex((ulong)pmsgCostumeUserS.UnId);
            UIBasePanel costumePanel = UIMgr.GetUIBasePanel("UIPanel/CostumePanel");
            if (costumePanel != null)//코스튬에서 장착을 시도함.
            {
                (costumePanel as CostumePanel).OnReceiveMountCostume(costumeData);
            }
            else//ChapterPopup에서 장착 시도함
            {
                UIBasePanel popup = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
                if (popup != null)
                    (popup as ReadyPopup).OnPMsgCostume(costumeData);
                //UIMgr.instance.OnPMsgChapterPopupCostume(costumeData);
            }
        }
        else if (pmsgCostumeUserS.UnRoleFlag == 2)//해제
        {
            //뭐해줄까
        }
    }

    /// <summary> 코스튬 보석 삽입 </summary>
    private void PMsgCostumeTokenSHandler(PMsgCostumeTokenS pmsgCostumeTokenS)
    {
        uint ErrorCode = (uint)pmsgCostumeTokenS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCostumeTokenS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel costumePanel = UIMgr.GetUIBasePanel("UIPanel/CostumePanel");
        if (costumePanel != null)//코스튬 보석 삽입 응답 처리
        {
            NetData._CostumeData costumeData = NetData.instance.GetUserInfo().GetCostumeForIndex((ulong)pmsgCostumeTokenS.UnId);
            (costumePanel as CostumePanel).OnReceiveJewel(costumeData);
        }
    }
    /*
    /// <summary> 장비 아이템 판매 응답 </summary>
    private void PMsgEquipmentSellSHandler(PMsgEquipmentSellS pmsgEquipmentSellS)
    {
        uint ErrorCode = (uint)pmsgEquipmentSellS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEquipmentSellS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        if (heroPanel != null)
        {
            (heroPanel as HeroPanel).OnReceiveSelectSell(pmsgEquipmentSellS.CInfo[0].UnId);
        }
    }
    */
    /// <summary> 소비 아이템 판매 응답 </summary>
    private void PMsgItemSellSHandler(PMsgItemSellS pmsgItemSellSHandler)
    {
        uint ErrorCode = (uint)pmsgItemSellSHandler.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgItemSellSHandler.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //pmsgItemSellSHandler.CInfoList[0].UnId
        //UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/InventoryPanel");
        //if (heroPanel != null)
        //    (heroPanel as HeroPanel).OnReceiveSellUseItem((uint)pmsgItemSellSHandler.CInfo[0].UnType);
    }

    private void PMsgDailyTimeSHandler(PMsgDailyTimeS pmsgDailyTimeS)
    {

    }

    private void PMsgPowerTimeSHandler(PMsgPowerTimeS pmsgPowerTimeS)
    {
        int powerTime = pmsgPowerTimeS.UnPowerTime;
        SceneManager.instance.RegenPowerTime = System.DateTime.Now.AddSeconds(powerTime);
        UIMgr.instance.SetRegenPower();
    }

    /// <summary> 싱글 스테이지 정보 받아오기 </summary>
    private void PMsgStageQuerySHandler(PMsgStageQueryS pmsgStageQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgStageQueryC");
        int count = pmsgStageQueryS.UnCount;
        for (int i = 0; i < count; i++)
        {
            StageInfo stageInfo = pmsgStageQueryS.CInfo[i];

            if (0 == stageInfo.UnComplete)//미 클리어
                continue;

            byte[] stageClear = new byte[3];

            stageClear[0] = (byte)stageInfo.UnStar[0];
            stageClear[1] = (byte)stageInfo.UnStar[1];
            stageClear[2] = (byte)stageInfo.UnStar[2];
            NetData.instance.GetUserInfo().ClearSingleStageDic.Add((uint)stageInfo.UnStageId
                , new NetData.ClearSingleStageData((uint)stageInfo.UnStageId, stageInfo.UnDailyComplete, stageInfo.UnDailyReset, stageClear[0], stageClear[1], stageClear[2]));
        }

        if (0 < count)
        {
            SingleGameState.lastSelectStageId = (uint)pmsgStageQueryS.CInfo[count - 1].UnStageId;
        }
        
    }

    /// <summary> 싱글 스테이지 시작 </summary>
    private void PMsgStageStartSHandler(PMsgStageStartS pmsgStageStartS)
    {
        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
        uint ErrorCode = (uint)pmsgStageStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (Panel != null)
                (Panel as ChapterPanel).OnErrorGameStart();

            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgStageStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }


        if(CheatPopup.IsStayProtocol)
        {
            CheatPopup.IsStayProtocol = false;
            CheatPopup.CheatStageState = CheatPopup.ClearStageState.Complete;
            return;
        }

        Debug.Log("싱글스테이지시작  " + pmsgStageStartS);
        NetData.RewardData rewardData = new NetData.RewardData();

        //획득 예정인 것들 저장한다.
        int bossDropCount = pmsgStageStartS.CBossDrop.Count;
        for (int i = 0; i < bossDropCount; i++)
        {
            StageDrop bossDrop = pmsgStageStartS.CBossDrop[i];
            rewardData.GetExp += (uint)bossDrop.UnExp;
            rewardData.GetCoin += bossDrop.UnCoin;

            int dropCount = bossDrop.CInfo.Count;
            for (int j = 0; j < dropCount; j++)
            {
                rewardData.GetList.Add(new NetData.DropItemData((uint)bossDrop.CInfo[j].UnItemId, (ushort)bossDrop.CInfo[j].UnItemNum, bossDrop.CInfo[j].UnType));
            }
        }

        //int cardListCount = pmsgStageStartS.CFlopInfo.Count;
        //for (int i = 0; i < cardListCount; i++)
        //{
        //    rewardData.CardList.Add(new NetData.DropItemData((uint)pmsgStageStartS.CFlopInfo[i].UnItemId, (uint)pmsgStageStartS.CFlopInfo[i].UnItemNum, pmsgStageStartS.CFlopInfo[i].UnType));
        //}

        //유저 정보 저장해놓는다.
        rewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref rewardData.SaveExp, ref rewardData.SaveMaxExp);

        rewardData.StageId = (uint)pmsgStageStartS.UnStageId;
        NetData.instance._RewardData = rewardData;

        if (Panel != null)
            (Panel as ChapterPanel).OnStageStart();
    }

    /// <summary> 싱글 스테이지 끝 </summary>
    private void PMsgStageCompleteSHandler(PMsgStageCompleteS pmsgStageCompleteS)
    {
        SceneManager.instance.EndNetProcess("ClearSingle");
        uint ErrorCode = (uint)pmsgStageCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgStageCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (CheatPopup.IsStayProtocol)
        {
            CheatPopup.IsStayProtocol = false;
            CheatPopup.CheatStageState = CheatPopup.ClearStageState.InGame;
            return;
        }
        
        if (pmsgStageCompleteS.UnVictory == 1)//성공
        {
			if (SceneManager.instance.testData.bSingleSceneTestStart){

				if (SceneManager.instance.testData.nTestStageId == pmsgStageCompleteS.UnStageId){
					SceneManager.instance.testData.nextStageId = SceneManager.instance.testData.nTestStageId + 1;
					if (SceneManager.instance.testData.nextStageId % 100 == 11){
						int t = SceneManager.instance.testData.nextStageId / 100;
						SceneManager.instance.testData.nextStageId = (t + 1) * 100 + 1; // 201,301,401..
					}
				}
			}

            byte[] stageClear = new byte[] { (byte)pmsgStageCompleteS.UnStar[0], (byte)pmsgStageCompleteS.UnStar[1], (byte)pmsgStageCompleteS.UnStar[2] };

            NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
            NetData.ClearSingleStageData clearSingleData = null;//이미 클리어 했던 스테이지라면 체크함
            if (userInfo.ClearSingleStageDic.TryGetValue((uint)pmsgStageCompleteS.UnStageId, out clearSingleData))
            {
                byte totalStar = (byte)(stageClear[0] + stageClear[1] + stageClear[2]);
                byte curTotalStar = (byte)(clearSingleData.Clear_0 + clearSingleData.Clear_1 + clearSingleData.Clear_2);
                if (curTotalStar < totalStar)
                {
                    clearSingleData.Clear_0 = stageClear[0];
                    clearSingleData.Clear_1 = stageClear[1];
                    clearSingleData.Clear_2 = stageClear[2];
                }

                ++clearSingleData.DailyClearCount;
            }
            else//새롭게 클리어한 스테이지. 정보 갱신한다.
            {
                // 모든모드를 최초클리어햇니
                //if (pmsgStageCompleteS.UnStageId >= 10000)
                    SingleGameState.IsFirstReward = true;

                clearSingleData = new NetData.ClearSingleStageData((uint)pmsgStageCompleteS.UnStageId, 1, 0, stageClear[0], stageClear[1], stageClear[2]);
                userInfo.ClearSingleStageDic.Add((uint)pmsgStageCompleteS.UnStageId, clearSingleData);

                DungeonTable.StageInfo stageInfo = _LowDataMgr.instance.GetStageInfo(clearSingleData.StageId);
                uint stageId = 0;
                if (stageInfo.type == 1)
                    stageId = (uint)(stageInfo.ChapId * 100);
                else
                    stageId = 10000 + (uint)(stageInfo.ChapId * 100);

                if (10 <= stageInfo.StageId - stageId)
                {
                    string msg = string.Format(_LowDataMgr.instance.GetStringCommon(842), _LowDataMgr.instance.GetStringStageData(stageInfo.ChapName));
                    UIMgr.AddLogChat(msg);
                }
            }
            
            if ((G_GameInfo.GameInfo as SingleGameInfo).UiState == 2)//클라이언트가 먼저 실행됬다.
            {
                G_GameInfo.GameInfo.OpenResultPanel(true);
            }
            else
            {
                (G_GameInfo.GameInfo as SingleGameInfo).UiState = 1;
                Quest.QuestInfo info = QuestManager.instance.CheckSubQuest(QuestSubType.SINGLEGAMECLEAR, (uint)pmsgStageCompleteS.UnStageId);
                if (info != null)//&& 
                {
                    if(0 < info.QuestTalkSceneID)
                        (G_GameInfo.GameInfo as SingleGameInfo).QuestTalkId = info.ID;

                    (G_GameInfo.GameInfo as SingleGameInfo).IsQuestClear = true;
                }
            }
        }
        else//실패
        {
            NetData.instance.ClearRewardData();
            G_GameInfo.GameInfo.OpenResultPanel(false);
        }
    }
    /*
    /// <summary> 싱글게임 카드 보상 </summary>
    private void PMsgStageFlopSHandler(PMsgStageFlopS pmsgStageFlopS)
    {
        UIBasePanel rewardPanel = UIMgr.GetUIBasePanel("UIPanel/SelectRewardPanel");
        uint ErrorCode = (uint)pmsgStageFlopS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgStageFlopS.GetType().ToString() + "-" + GetErrorString(ErrorCode));

            if (rewardPanel != null)
            {
                (rewardPanel as SelectRewardPanel).OnErrorResetReward();
            }

            return;
        }

        if (CheatPopup.IsStayProtocol)
        {
            CheatPopup.IsStayProtocol = false;
            CheatPopup.CheatStageState = CheatPopup.ClearStageState.InGame;
            return;
        }

        if (rewardPanel != null)
        {
            uint lowDataId = (uint)pmsgStageFlopS.CFlopInfo[0].UnItemId;
            if (lowDataId < 100)
                lowDataId = (uint)pmsgStageFlopS.CFlopInfo[0].UnType;

            (rewardPanel as SelectRewardPanel).PMsgCardOpen(lowDataId, pmsgStageFlopS.UnType, (ushort)pmsgStageFlopS.CFlopInfo[0].UnItemNum);
        }
    }
    */
    /// <summary> 던전 소탕 응답 </summary>
    public void PMsgStageSweepSHandler(PMsgStageSweepS pmsgStageSweepS)
    {
        uint ErrorCode = (uint)pmsgStageSweepS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == (int)Sw.ErrorCode.ER_StageSweepS_Vip_Level_Error)
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
            else
                UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgStageSweepS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint stageId = (uint)pmsgStageSweepS.UnStageId;
        int sweepCount = pmsgStageSweepS.UnTimes;

        NetData.ClearSingleStageData clearSingleData = null;//이미 클리어 했던 스테이지라면 체크함
        if (NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(stageId, out clearSingleData))
        {
            ++clearSingleData.DailyClearCount;
            UIBasePanel chapterPanel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
            if (chapterPanel != null)
            {
                (chapterPanel as ChapterPanel).OnPMsgStageSweep(clearSingleData.DailyClearCount);
            }
        }

        Quest.QuestInfo info = QuestManager.instance.CheckSubQuest(QuestSubType.SINGLEGAMECLEAR, stageId);
        if (info != null && 0 < info.QuestTalkSceneID)
        {
            UIMgr.OpenMissionPanel(info.ID);
        }
    }

    /// <summary> 던전 소탕 드랍 보상. </summary>
    public void PMsgStageSweepResultSHendler(PMsgStageSweepResultS pmsgStageSweepResultS)
    {
        Debug.Log("Sweep Log " + pmsgStageSweepResultS);
        UIBasePanel chapterPanel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
        if (chapterPanel != null)
        {
            uint stageId = (uint)pmsgStageSweepResultS.UnStageId;
            int sweepCount = pmsgStageSweepResultS.UnTimes;
            int getCoin = 0, getExp = 0;

            List<NetData.DropItemData> getList = new List<NetData.DropItemData>();
            int bossDrop = pmsgStageSweepResultS.CBossDrop.Count;
            for (int i = 0; i < bossDrop; i++)
            {
                getCoin += pmsgStageSweepResultS.CBossDrop[i].UnCoin;//획득 골드
                getExp += pmsgStageSweepResultS.CBossDrop[i].UnExp;//획득 경험치

                IList<Sw.DropItem> bossItemList = pmsgStageSweepResultS.CBossDrop[i].CInfo;
                int dropCount = bossItemList.Count;
                for (int j = 0; j < dropCount; j++)//획득 아이템
                {
                    getList.Add(new NetData.DropItemData((uint)bossItemList[j].UnItemId, (ushort)bossItemList[j].UnItemNum, bossItemList[j].UnType));
                }
            }

            List<NetData.DropItemData> getCardList = new List<NetData.DropItemData>();
            //int cardCount = pmsgStageSweepResultS.CFlopInfo.Count;
            //for (int i = 0; i < cardCount; i++)//카드 뒤집기 보상.
            //{
            //    getCardList.Add(new NetData.DropItemData((uint)pmsgStageSweepResultS.CFlopInfo[i].UnItemId
            //        , (uint)pmsgStageSweepResultS.CFlopInfo[i].UnItemNum, pmsgStageSweepResultS.CFlopInfo[i].UnType));
            //}

            NetData.SweepSlotData slotData = new NetData.SweepSlotData(getCoin, getExp, sweepCount, getList, getCardList);
            (chapterPanel as ChapterPanel).OnPMsgSweepReward(slotData);
        }
    }

    /// <summary> 코스튬 스킬 레벨업 응답 </summary>
    private void PMsgCostumeSkillUpgradeSHandler(PMsgCostumeSkillUpgradeS pmsgCostumeSkillUpgradeS)
    {
        UIBasePanel costumePanel = UIMgr.GetUIBasePanel("UIPanel/CostumePanel");
        uint ErrorCode = (uint)pmsgCostumeSkillUpgradeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            //if (ErrorCode == (int)Sw.ErrorCode.ER_CostumeSkillUpgradeS_Role_Level_Error)
            //{
            //    UIMgr.instance.AddErrorPopup((int)ErrorCode);(-3998, null, null, null);//캐릭터 레벨 부족
            //}
            //else if (ErrorCode == (int)Sw.ErrorCode.ER_CostumeSkillUpgradeS_Coin_Num_Error)
            //{
            //    UIMgr.instance.AddErrorPopup((int)ErrorCode);(-1002, null, null, null);//재화 부족
            //}

            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCostumeSkillUpgradeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData._CostumeData costume = NetData.instance.GetUserInfo().GetCostumeForIndex((ulong)pmsgCostumeSkillUpgradeS.UnId);
        if (costume == null)
        {
            costume = NetData.instance.GetUserInfo().GetEquipCostume();
            Debug.LogError("not found costumeData! is set equipCostume");
        }

        if (costumePanel != null)
            (costumePanel as CostumePanel).OnPMsgCostumeSkillUp(costume, pmsgCostumeSkillUpgradeS.UnSkillIdx);

    }

    private void PMsgReturnMainMapSHandler(PMsgReturnMainMapS pmsgReturnMainMapS)
    {
        uint ErrorCode = (uint)pmsgReturnMainMapS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgReturnMainMapS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }
    /// <summary> 모든 메일 정보 요청 조회의 응답 </summary>
    private void PMsgEmailQueryListSHandler(PMsgEmailQueryListS pmsgEmailQueryListS)
    {
        uint mailCount = (uint)pmsgEmailQueryListS.UnMailCount; //우편수량
        SceneManager scenMgr = SceneManager.instance;
        scenMgr.EndNetProcess("GetMailList");
        scenMgr.CheckMailCount((int)mailCount, mailCount < 10 && scenMgr.CheckNetProcess("GetMailList") );

        Debug.Log(pmsgEmailQueryListS);
        
        List<NetData.EmailBaseInfo> emailInfoList = new List<NetData.EmailBaseInfo>();

        bool isAlram = false;
        for (int i = 0; i < mailCount; i++)
        {
            EmailBaseInfo email = pmsgEmailQueryListS.CEmailInfo[i];

            uint mailId = email.UnId;
            //long userId = email.UllUserId;
            long senderId = email.UllSendUserId;
            int mailType = email.UsMailType;
            int isReceive = email.UcReadflag;
            ulong readTime = email.UllReadtime;
            ulong sendTime = email.UllSendtime;
            ulong overTime = email.UllOvertime;

            NetData.EmailBaseInfo em = new NetData.EmailBaseInfo(mailId/*, userId*/, senderId, mailType, isReceive, readTime, sendTime, overTime);

            emailInfoList.Add(em);

            if (!isAlram && isReceive == 0)//안읽음
                isAlram = true;
        }

        scenMgr.SetAlram(AlramIconType._SOCIAL, isAlram);

        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (Panel != null)
            (Panel as SocialPanel).GetMailList(emailInfoList);
    }

    /// <summary> 우편 상세 정보 조회(가져오기) 응답 </summary>
    private void PMsgEmailReadDetailSHandler(PMsgEmailReadDetailS pmsgEmailReadDetailS)
    {
        SceneManager.instance.EndNetProcess("ReadMail");

        uint ErrorCode = (uint)pmsgEmailReadDetailS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEmailReadDetailS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgEmailReadDetailS);

        uint MailId = pmsgEmailReadDetailS.UnId;
        ulong ReadTime = pmsgEmailReadDetailS.UllReadtime;
        //string Content = pmsgEmailReadDetailS.SzContent.ToString();
        //uint Cash = pmsgEmailReadDetailS.UnGen;
        //uint Gold = pmsgEmailReadDetailS.UnCoin;
        //uint Fame = pmsgEmailReadDetailS.UnFame;
        //uint Honor = pmsgEmailReadDetailS.UnHonor;
        //uint Contribution = pmsgEmailReadDetailS.UnContribution;
        //uint RoyalBadge = pmsgEmailReadDetailS.UnRoyalBadge;
        //uint LionBadge = pmsgEmailReadDetailS.UnLionKingBadge;
        int Count = pmsgEmailReadDetailS.UnCount;

        List<NetData.EmailAttachmentInfo> emAtt = new List<NetData.EmailAttachmentInfo>();
        for (int i = 0; i < pmsgEmailReadDetailS.CAttachments.Count; i++)
        {
            EmailAttachmentInfo email = pmsgEmailReadDetailS.CAttachments[i];
            NetData.EmailAttachmentInfo att = new NetData.EmailAttachmentInfo(email.UnType, email.UnGoodsId, email.UnNum);
            emAtt.Add(att);
        }

        NetData.EamilDetails emDetail = new NetData.EamilDetails(MailId, ReadTime,/* Content, Cash, Gold, Fame, Honor, Contribution, RoyalBadge, LionBadge*/ Count, emAtt);

        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (Panel != null)
            (Panel as SocialPanel).GetDetailMailData(emDetail);

    }

    /// <summary> 첨부 아이템 수령 요청 응답 </summary>
    private void PMsgEmailFeatchSHandler(PMsgEmailFeatchS pmsgEmailFeatchS)
    {
        SceneManager.instance.EndNetProcess("GetMail");
        uint ErrorCode = (uint)pmsgEmailFeatchS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_EmailFeatchC_Success_Error)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEmailFeatchS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgEmailFeatchS);

        // 잠시 
        List<uint> info = new List<uint>();
        info.Add(pmsgEmailFeatchS.UnId);


        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (Panel != null)
            (Panel as SocialPanel).GetMailItem(info);

    }

    /// <summary> 일괄 수령 요청 응답 </summary>
    private void PMsgEmailOnKeyFeatchSHandler(PMsgEmailOnKeyFeatchS pmsgEmailOnKeyFeatchS)
    {
        SceneManager.instance.EndNetProcess("GetAllMail");

        uint ErrorCode = (uint)pmsgEmailOnKeyFeatchS.UnErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEmailOnKeyFeatchS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgEmailOnKeyFeatchS);
        uint mailErrorCode = 0;
        uint Error = 0;
        List<uint> info = new List<uint>();
        List<NetData.EmailAttachmentInfo> itemInfo = new List<NetData.EmailAttachmentInfo>();
        for (int i = 0; i < pmsgEmailOnKeyFeatchS.UnMailCount; i++)
        {
            EmailOneKeyFeatchInfo email = pmsgEmailOnKeyFeatchS.COnKeyFeatchInfo[i];
            mailErrorCode = email.UnErrorCode;
            if (mailErrorCode != (int)Sw.ErrorCode.ER_EmailFeatchC_Success_Error)
            {
                // 장비가 있는경우 일괄수령불가 
                Debug.Log(pmsgEmailOnKeyFeatchS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
                Error = mailErrorCode;
            }
            else
            {
                info.Add(email.UnId);
                for (int j = 0; j < email.CAttachments.Count; j++)
                {
                    EmailAttachmentInfo eInfo = email.CAttachments[j];
                    NetData.EmailAttachmentInfo em = new NetData.EmailAttachmentInfo(eInfo.UnType, eInfo.UnGoodsId, eInfo.UnNum);
                    itemInfo.Add(em);
                }
            }
        }


        if (info.Count >= 1) // 받은수량이 한개라도 있다면?
        {
            UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
            if (Panel != null)
                (Panel as SocialPanel).GetAllMailItem(info, itemInfo);
        }
        else
        {
            if (Error != 0) //if (mailErrorCode != (int)Sw.ErrorCode.ER_success)
                UIMgr.instance.AddErrorPopup((int)Error);
        }
   
    }

    /// <summary>  단기 메일 삭제 요청에 대한 응답 </summary>
    private void PMsgEmailDelSHandler(PMsgEmailDelS pmsgEmailDelS)
    {
        SceneManager.instance.EndNetProcess("DelMail");
        SceneManager.instance.CheckMailCount(-1);//단일 메일이므로 1개만 삭제

        uint ErrorCode = (uint)pmsgEmailDelS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEmailDelS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        List<int> getMail = new List<int>();
        int mailId = (int)pmsgEmailDelS.UnId;

        getMail.Add(mailId);
        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (Panel != null)
            (Panel as SocialPanel).ActionDeleteMailData(getMail);
    }

    /// <summary>  일괄삭제 요청에 대한 응답 </summary>
    private void PMsgEmailOnKeyDelSHandler(PMsgEmailOnKeyDelS pmsgEmailOnKeyDelS)
    {
        SceneManager.instance.EndNetProcess("DelAllMail");
        SceneManager.instance.CheckMailCount(-pmsgEmailOnKeyDelS.UnMailCount);

        uint ErrorCode = (uint)pmsgEmailOnKeyDelS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgEmailOnKeyDelS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        List<int> getMail = new List<int>();
        List<NetData.EmailDelInfo> emDel = new List<NetData.EmailDelInfo>();
        for (int i = 0; i < pmsgEmailOnKeyDelS.UnMailCount; i++)
        {
            EmailDelInfo email = pmsgEmailOnKeyDelS.CEmailDelInfo[i];
            NetData.EmailDelInfo em = new NetData.EmailDelInfo(email.UnId);
            emDel.Add(em);
            getMail.Add((int)email.UnId);
        }

        UIBasePanel Panel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (Panel != null)
            (Panel as SocialPanel).ActionDeleteMailData(getMail);

    }

    private void PMsgHeroQuerySHandler(PMsgHeroQueryS pmsgHeroQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgHeroQueryC");
        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();

        for (int i = 0; i < pmsgHeroQueryS.CInfo.Count; i++)
        {
            NetData._PartnerData partner = userInfo.GetPartnerForDataID((uint)pmsgHeroQueryS.CInfo[i].UnType);

            ulong partnerIdx = (ulong)pmsgHeroQueryS.CInfo[i].UnHeroId;
            uint level = (uint)pmsgHeroQueryS.CInfo[i].UnLevel;
            ulong exp = (ulong)pmsgHeroQueryS.CInfo[i].UllExp;
            ushort enchant = 0;// (ushort)pmsgHeroQueryS.CInfo[i].UnEnchantTime;
    

            if (partner != null)
            {
                partner.SetSpawnPartnerData(partnerIdx, level, exp, enchant, 0, 0, (uint)pmsgHeroQueryS.CInfo[i].UnAttack);
            }
            else//없는 것은 진화된 유닛임.
            {
                Partner.PartnerDataInfo lowData = _LowDataMgr.instance.GetPartnerInfo((uint)pmsgHeroQueryS.CInfo[i].UnType);
                if (lowData != null)
                {
                    List<uint> list = _LowDataMgr.instance.GetLowDataPartnerIdForType(lowData.Type);
                    int idCount = list.Count;
                    for (int j = 0; j < idCount; j++)//해당 그룹 중에 이미 존재하는 파트너가 있다면 삭제
                    {
                        userInfo.RemovePartnerForDataID(list[j]);
                    }

                    partner = userInfo.CreatePartner(partnerIdx, (ushort)pmsgHeroQueryS.CInfo[i].UnType, level, 0, 0, enchant, exp, 0, (uint)pmsgHeroQueryS.CInfo[i].UnAttack, true);
                }
            }

            if (partner != null)
            {
                //0번은평타
                partner.EquipBuffSkillData(0, new NetData._PartnerActiveSkillData((uint)pmsgHeroQueryS.CInfo[i].UnBaseSkillId, 1));

                for (int j = 0; j < pmsgHeroQueryS.CInfo[i].CSkillInfo.Count; j++)
                {
                    uint skillId = (uint)pmsgHeroQueryS.CInfo[i].CSkillInfo[j].UnSkillId;
                    if (0 < skillId)
                        partner.EquipBuffSkillData((ushort)(j + 1), new NetData._PartnerActiveSkillData(skillId, (byte)pmsgHeroQueryS.CInfo[i].CSkillInfo[j].UnSkillLevel));
                }

            }
        }

        NetData.instance.GetUserInfo().SortPartnerList();

        if(TownState.TownActive)
        {
            //파트너관련체크
            bool partner = false;
            List<NetData._PartnerData> dataList = NetData.instance.GetUserInfo().GetPartnerList();
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i]._isOwn)
                    continue;

                if (dataList[i].NowShard >= dataList[i]._needShard)
                {
                    partner = true;
                    break;
                }
            }

            SceneManager.instance.SetAlram(AlramIconType.PARTNER, partner);
        }
        //UIBasePanel panel = UIMgr.instance.FindInShowing("UIPanel/TownPanel");
        //if (panel != null && panel.gameObject.activeSelf)
        //    (panel as TownPanel).CheckAlramMark();

    }

    private void PMsgHeroFusionSHandler(PMsgHeroFusionS pmsgHeroFusionS)
    {
        uint ErrorCode = (uint)pmsgHeroFusionS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgHeroFusionS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/PartnerPanel");
        if (panel == null)
            return;

        uint lowDataId = (uint)pmsgHeroFusionS.UnHeroType;

        (panel as PartnerPanel).OnPartnerFusion(lowDataId);

    }
    //안쓰는기능, 일단 프르토콜은 존재함
    ///// <summary> 파트너 경험치 성장 응답 </summary> 
    //private void PMsgHeroUseExpItemSHandler(PMsgHeroUseExpItemS pmsgHeroUseExpItemS)
    //{
    //    SceneManager.instance.EndNetProcess("PMsgHeroUseExpItemC");
    //    uint ErrorCode = (uint)pmsgHeroUseExpItemS.UnErrorCode;
    //    if (ErrorCode != (int)Sw.ErrorCode.ER_success)
    //    {
    //        UIMgr.instance.AddErrorPopup((int)ErrorCode); 
    //        Debug.Log(pmsgHeroUseExpItemS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
    //        return;
    //    }

    //    UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/PartnerPanel");
    //    if (partnerPanel != null)
    //    {
    //        (partnerPanel as PartnerPanel).OnPMsgHeroUseExp(pmsgHeroUseExpItemS.UnHeroId);
    //    }
    //}

    /// <summary> 파트너 진화 별 상승 응답 </summary> 
    private void PMsgHeroEvolveSHandler(PMsgHeroEvolveS pmsgHeroEvolveS)
    {
        SceneManager.instance.EndNetProcess("pmsgHeroEvolveC");
        uint ErrorCode = (uint)pmsgHeroEvolveS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgHeroEvolveS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgHeroEvolveS);
        UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/PartnerPanel");
        if (partnerPanel != null)
        {
            (partnerPanel as PartnerPanel).OnPMsgHeroPromoSHandler((uint)pmsgHeroEvolveS.UnHeroId);
        }
    }

    
    /// <summary> 파트너액티브스킬 레벌업 응답 </summary>
    private void PMsgHeroSkillUpgradeSHandler(PMsgHeroSkillUpgradeS pmsgHeroSkillUpgradeS)
    {
        SceneManager.instance.EndNetProcess("PMsgHeroSkillUpgradeC");

        uint ErrorCode = (uint)pmsgHeroSkillUpgradeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgHeroSkillUpgradeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgHeroSkillUpgradeS);
        UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/PartnerPanel");
        if (partnerPanel != null)
            (partnerPanel as PartnerPanel).OnPMsgHeroSkillUpgradeSHandler();
    }

    /// <summary> 친구리스트 요청 조회 응답 </summary> 
    private void PMsgFriendQueryListSHandler(PMsgFriendQueryListS pmsgFriendQueryListS)
    {
        SceneManager.instance.EndNetProcess("GetFriendList");

        Debug.Log(pmsgFriendQueryListS);

        uint ErrorCode = (uint)pmsgFriendQueryListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendQueryListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        List<NetData.FriendBaseInfo> FriendList = new List<NetData.FriendBaseInfo>();
        for (int i = 0; i < pmsgFriendQueryListS.CFriendInfo.Count; i++)
        {
            FriendBaseInfo friendInfo = pmsgFriendQueryListS.CFriendInfo[i];
            NetData.FriendBaseInfo Info = new NetData.FriendBaseInfo(
                friendInfo.UllRoleId,
                friendInfo.SzName,
                friendInfo.UnLevel,
                friendInfo.UnRoleType,
                friendInfo.UllLoginTime,
                friendInfo.UllSendPowerTime,
                friendInfo.UnAttack,
                (int)eGuildMemberOnLineStatus.eGuildMemberOnLineStatus_On_Line == friendInfo.UnOnLineStatus
            );
            FriendList.Add(Info);
        }

        UIBasePanel invitePopup = UIMgr.GetUIBasePanel("UIPopup/InvitePopup");
        if (invitePopup != null)
        {
            (invitePopup as InvitePopup).OnFriendList(FriendList);
        }
        UIBasePanel guildPanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (guildPanel != null)
        {
            (guildPanel as GuildPanel).CheckMyFrinedList(FriendList);
        }

        UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
        {
            ulong chatUserRoleId = 0;
            UIBasePanel userInfoPopup = UIMgr.GetUIBasePanel("UIPopup/UserInfoPopup");
            if(userInfoPopup != null )//1:1 친구 대화로 온것.
                chatUserRoleId = (ulong)(userInfoPopup as UserInfoPopup).UserRoleId;

            (friendPanel as SocialPanel).GetFriendList(FriendList, chatUserRoleId);
        }
        else
        {
            UIBasePanel userInfoPopup = UIMgr.GetUIBasePanel("UIPopup/UserInfoPopup");
            if (userInfoPopup != null)
            {
                userInfoPopup.NetworkData(MSG_DEFINE._MSG_FRIEND_QUERY_LIST_C, FriendList);
            }
        }
    }

    /// <summary> 친구 자세한 정보 조회 응답 </summary> 
    //private void PMsgFriendFullInfoSHandler(PMsgFriendFullInfoS pmsgFriendFullInfoS)
    //{
    //    SceneManager.instance.EndNetProcess("FriendDetailInfo");

    //    uint ErrorCode = (uint)pmsgFriendFullInfoS.UnErrorCode;
    //    if (ErrorCode != (int)Sw.ErrorCode.ER_success)
    //    {
    //        UIMgr.instance.AddErrorPopup((int)ErrorCode); 
    //        Debug.Log(pmsgFriendFullInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
    //        return;
    //    }

    //    ulong Id = pmsgFriendFullInfoS.UllRoleId;
    //    uint equipCnt = pmsgFriendFullInfoS.UnEquipCount;
    //    uint costumeCnt = pmsgFriendFullInfoS.UnCostumeCount;

    //    List<NetData._EquipmentInfo> equipList = new List<NetData._EquipmentInfo>();
    //    List<NetData._ItemData> itemdataList = new List<NetData._ItemData>();

    //    for (int i = 0; i < pmsgFriendFullInfoS.CEquipmentInfo.Count; i++)
    //    {
    //        PMsgFriendFullInfoS.Types.EquipmentInfo equip = pmsgFriendFullInfoS.CEquipmentInfo[i];
    //        NetData._EquipmentInfo Info = new NetData._EquipmentInfo(equip.UnId, equip.UnType, equip.UnPosition, equip.UnEnchantTime, equip.UnEvolveStar, equip.UnEvolveTime, equip.UnRandomOption1, equip.UnRandomValue1, equip.UnRandomOption2, equip.UnRandomValue2, equip.UnBasicValue);
    //        equipList.Add(Info);

    //        NetData._ItemData itemdata = NetData.instance.GetFriendFullInfo().CreateEquipItem((ulong)equip.UnId, (uint)equip.UnType, (ushort)equip.UnEnchantTime, (ushort)equip.UnEvolveTime, (ushort)equip.UnEvolveStar, 0);
    //        NetData.instance.GetFriendFullInfo().ItemEquip((ulong)equip.UnId);
    //        Item.EquipmentInfo defaultAbility = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equip.UnType);

    //        if (defaultAbility != null)
    //        {
    //            Item.ItemValueInfo itemValue = _LowDataMgr.instance.GetLowDataItemValueInfo(defaultAbility.BasicOptionIndex);

    //            if (itemValue != null)
    //            {
    //                NetData.ItemAbilityData abilityData = new NetData.ItemAbilityData();
    //                abilityData.Ability = (AbilityType)itemValue.OptionId;
    //                abilityData.Value = (int)equip.UnBasicValue;

    //                itemdata.StatList.Add(abilityData);
    //            }
    //        }

    //        if (equip.UnRandomOption1 != 0)
    //        {
    //            Item.ItemValueInfo itemValue = _LowDataMgr.instance.GetLowDataItemValueInfo((uint)equip.UnRandomOption1);
    //            NetData.ItemAbilityData abilityData = new NetData.ItemAbilityData();
    //            abilityData.Ability = (AbilityType)itemValue.OptionId;
    //            abilityData.Value = (int)equip.UnRandomValue1;

    //            itemdata.StatList.Add(abilityData);
    //        }

    //        if (equip.UnRandomOption2 != 0)
    //        {
    //            Item.ItemValueInfo itemValue = _LowDataMgr.instance.GetLowDataItemValueInfo((uint)equip.UnRandomOption2);
    //            NetData.ItemAbilityData abilityData = new NetData.ItemAbilityData();
    //            abilityData.Ability = (AbilityType)itemValue.OptionId;
    //            abilityData.Value = (int)equip.UnRandomValue2;

    //            itemdata.StatList.Add(abilityData);
    //        }

    //        itemdata.EvolveAndEnchantApply();
    //        //}

    //        itemdataList.Add(itemdata);

    //    }

    //    List<NetData._CostumeInfo> costumeList = new List<NetData._CostumeInfo>();
    //    for (int i = 0; i < pmsgFriendFullInfoS.CCostumeInfo.Count; i++)
    //    {
    //        PMsgFriendFullInfoS.Types.CostumeInfo costume = pmsgFriendFullInfoS.CCostumeInfo[i];
    //        NetData._CostumeInfo Info = new NetData._CostumeInfo(costume.UnId, costume.UnType, costume.UnPosition, costume.UnEvolveStar, costume.UnEvolveTime);
    //        List<int> tokenList = new List<int>();
    //        List<int> skillList = new List<int>();
    //        for (int j = 0; j < costume.UnToken.Count; j++)
    //        {
    //            tokenList.Add(costume.UnToken[j]);
    //        }
    //        for (int k = 0; k < costume.UnSkillLevel.Count; k++)
    //        {
    //            skillList.Add(costume.UnSkillLevel[k]);
    //        }
    //        Info.unToken = tokenList;
    //        Info.unSkillLevel = skillList;

    //        costumeList.Add(Info);
    //    }

    //    NetData.FriendFullInfo friendFullInfo = new NetData.FriendFullInfo();
    //    friendFullInfo.RoldId = Id;
    //    friendFullInfo.EqupipCount = equipCnt;
    //    friendFullInfo.CostumeCount = costumeCnt;
    //    friendFullInfo.cEquipmentInfo = equipList;
    //    friendFullInfo.cCostumeInfo = costumeList;




    //    UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
    //    if (friendPanel != null)
    //        (friendPanel as SocialPanel).FriendFullInfo(friendFullInfo, itemdataList);

    //}

    /// <summary> 친구 검색 요청 응답 </summary> 
    private void PMsgFriendSearchSHandler(PMsgFriendSearchS pmsgFriendSearchS)
    {
        SceneManager.instance.EndNetProcess("SearchFriend");

        uint ErrorCode = (uint)pmsgFriendSearchS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendSearchS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

       List<NetData.FriendBaseInfo> SearchInfo = new List<NetData.FriendBaseInfo>();
        for (int i = 0; i < pmsgFriendSearchS.CFriendInfo.Count; i++)
        {
            FriendBaseInfo friendInfo = pmsgFriendSearchS.CFriendInfo[i];
            NetData.FriendBaseInfo Info = new NetData.FriendBaseInfo(
                friendInfo.UllRoleId,
                friendInfo.SzName,
                friendInfo.UnLevel,
                friendInfo.UnRoleType,
                friendInfo.UllLoginTime,
                friendInfo.UllSendPowerTime,
                friendInfo.UnAttack,
                (int)eGuildMemberOnLineStatus.eGuildMemberOnLineStatus_On_Line == friendInfo.UnOnLineStatus
            );
            SearchInfo.Add(Info);
        }

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).SetSearchFriendPopup(SearchInfo);

    }

    /// <summary> 추천친구 리스트 요청 응답 </summary> 
    private void PMsgFriendRecommendListSHandler(PMsgFriendRecommendListS pmsgFriendRecommendListS)
    {
        SceneManager.instance.EndNetProcess("GetRecFriendList");
        uint ErrorCode = (uint)pmsgFriendRecommendListS.UnErrorCode;

        // 친구꽉찼을때 오는팝업은 무시하고 ui상에표시하는걸로..
        if(ErrorCode == (int)Sw.ErrorCode.ER_Friend_RecommendList_Friend_CountMax_Error)
        {
            UIBasePanel friend = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
            if (friend != null)
                (friend as SocialPanel).EmptyRecommend();

            Debug.Log(pmsgFriendRecommendListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }


        if (ErrorCode == 1151)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            return;
        }
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendRecommendListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        List<NetData.FriendBaseInfo> RecommendList = new List<NetData.FriendBaseInfo>();
        for (int i = 0; i < pmsgFriendRecommendListS.CFriendInfo.Count; i++)
        {
            FriendBaseInfo friendInfo = pmsgFriendRecommendListS.CFriendInfo[i];
            NetData.FriendBaseInfo Info = new NetData.FriendBaseInfo(
                friendInfo.UllRoleId,
                friendInfo.SzName,
                friendInfo.UnLevel,
                friendInfo.UnRoleType,
                friendInfo.UllLoginTime,
                friendInfo.UllSendPowerTime,
                friendInfo.UnAttack,
                (int)eGuildMemberOnLineStatus.eGuildMemberOnLineStatus_On_Line == friendInfo.UnOnLineStatus
            );

            RecommendList.Add(Info);
        }


        UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).GetRecommendList(RecommendList);

    }

    /// <summary> 신청 추가 친구 응답 </summary> 
    private void PMsgFriendAddSHandler(PMsgFriendAddS pmsgFriendAddS)
    {
        SceneManager.instance.EndNetProcess("AddFriend");

        uint ErrorCode = (uint)pmsgFriendAddS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendAddS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel guildPanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (guildPanel != null)
            (guildPanel as GuildPanel).SuccessFriendAdd();

        UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel == null)
            return;

        NetData.FriendRequestBaseInfo Info = new NetData.FriendRequestBaseInfo(pmsgFriendAddS.UllRoleId, pmsgFriendAddS.SzName, pmsgFriendAddS.NLevel, pmsgFriendAddS.UnRoleType, pmsgFriendAddS.UllApplyTime);
        (friendPanel as SocialPanel).AddFriendView(Info);

    }

    /// <summary> 신청자의 신청 정보 피신청자에게 통지 </summary> 
    private void PMsgFriendNotifyAddSHandler(PMsgFriendNotifyAddS pmsgFriendNotifyAddS)
    {
       
        NetData.FriendRequestBaseInfo Info = new NetData.FriendRequestBaseInfo(pmsgFriendNotifyAddS.UllRoleId, pmsgFriendNotifyAddS.SzName, pmsgFriendNotifyAddS.NLevel, pmsgFriendNotifyAddS.UnRoleType, pmsgFriendNotifyAddS.UllApplyTime);
        
                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).NotifyAddFriend(Info);

        //받은요청있으면 알림
        char[] inviteArr = SceneManager.instance.optionData.BlockInvite.ToCharArray(); //친구초대 거부일시 리스트안보여주는방향..
        if (inviteArr[1] != '2')
        {
            SceneManager.instance.SetAlram(AlramIconType.SOCIAL, true);
        }
    }

    /// <summary> 친구추가 리스트에서 타인에 대한 친구추가 신청 취소응답 </summary> 
    private void PMsgFriendCancleAddSHandler(PMsgFriendCancleAddS pmsgFriendCancleAddS)
    {
        SceneManager.instance.EndNetProcess("CancleFriend");


        ulong Id = pmsgFriendCancleAddS.UllRoleId;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).FriendCancle(Id);


    }

    /// <summary> 신청추가 친구 취소 통지 </summary> 
    private void PMsgFriendNotifyCancleAddSHandler(PMsgFriendNotifyCancleAddS pmsgFriendNotifyCancleAddS)
    {
        ulong Id = pmsgFriendNotifyCancleAddS.UllRoleId;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).NotifyCancleFriend(Id);


    }

    /// <summary> 신청자 리스트 조회 응답 </summary> 
    private void PMsgFriendRequestFriendListSHandler(PMsgFriendRequestFriendListS pmsgFriendRequestFriendListS)
    {
        SceneManager.instance.EndNetProcess("GetRcvFriendList");
        Debug.Log(pmsgFriendRequestFriendListS);
        uint ErrorCode = (uint)pmsgFriendRequestFriendListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendRequestFriendListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        List<NetData.FriendRequestBaseInfo> RecieveList = new List<NetData.FriendRequestBaseInfo>();

        char[] inviteArr = SceneManager.instance.optionData.BlockInvite.ToCharArray(); //친구초대 거부일시 리스트안보여주는방향..

        if(inviteArr[1] != '2')
        {
            for (int i = 0; i < pmsgFriendRequestFriendListS.CRequestInfo.Count; i++)
            {
                FriendRequestBaseInfo friendInfo = pmsgFriendRequestFriendListS.CRequestInfo[i];
                NetData.FriendRequestBaseInfo Info = new NetData.FriendRequestBaseInfo(friendInfo.UllRoleId, friendInfo.SzName, friendInfo.NLevel, friendInfo.UnRoleType, friendInfo.UllRequestTime);
                RecieveList.Add(Info);
            }
        }


        if (RecieveList.Count > 0)
            SceneManager.instance.SetAlram(AlramIconType.SOCIAL, true);


        UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).GetRecieveList(RecieveList);
    }

    /// <summary> 본인이 추가한 친구 리스트 조회 응답 </summary> 
    private void PMsgFriendSelfRequestFriendListSHandler(PMsgFriendSelfRequestFriendListS pmsgFriendSelfRequestFriendListS)
    {
        SceneManager.instance.EndNetProcess("GetReqFriendList");
        uint ErrorCode = (uint)pmsgFriendSelfRequestFriendListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgFriendSelfRequestFriendListS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgFriendSelfRequestFriendListS);
        List<NetData.FriendRequestBaseInfo> sendList = new List<NetData.FriendRequestBaseInfo>();
        for (int i = 0; i < pmsgFriendSelfRequestFriendListS.CRequestInfo.Count; i++)
        {
            FriendRequestBaseInfo friendInfo = pmsgFriendSelfRequestFriendListS.CRequestInfo[i];
            NetData.FriendRequestBaseInfo Info = new NetData.FriendRequestBaseInfo(friendInfo.UllRoleId, friendInfo.SzName, friendInfo.NLevel, friendInfo.UnRoleType, friendInfo.UllRequestTime);
            sendList.Add(Info);
        }

        UIBasePanel guildPanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (guildPanel != null)
            (guildPanel as GuildPanel).CheckSendFriendList(sendList);

        UIBasePanel userPop = UIMgr.GetUIBasePanel("UIPopup/UserInfoPopup");
        if (userPop != null)
            userPop.NetworkData(MSG_DEFINE._MSG_FRIEND_SELF_REQUEST_FRIEND_LIST_C, sendList);

        UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).GetSendList(sendList);
    }

    /// <summary> 신청자 요청 처리 응답 </summary> 
    private void PMsgFriendApplicantSHandler(PMsgFriendApplicantS pmsgFriendApplicantS)
    {
        SceneManager.instance.EndNetProcess("ApplyFriend");

        uint ErrorCode = (uint)pmsgFriendApplicantS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendApplicantS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        // 1-> 동의 2 ->거절
        ulong Id = pmsgFriendApplicantS.UllRoleId;
        uint AgreeOrReject = pmsgFriendApplicantS.UnAccede;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).FriendApplicant(Id, AgreeOrReject);

    }

    /// <summary> 신청자에게 처리결과 통지 </summary> 
    private void PMsgFriendNotifyApplicantSHandler(PMsgFriendNotifyApplicantS pmsgFriendNotifyApplicantS)
    {
        ulong Id = pmsgFriendNotifyApplicantS.UllRoleId;
        uint flag = pmsgFriendNotifyApplicantS.UnAccede;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).NotifyResultFriend(Id, flag);

    }

    /// <summary> 친구 삭제 요청 응답 </summary> 
    private void PMsgFriendDelFriendSHandler(PMsgFriendDelFriendS pmsgFriendDelFriendS)
    {
        SceneManager.instance.EndNetProcess("DelFriend");

        uint ErrorCode = (uint)pmsgFriendDelFriendS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendDelFriendS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        ulong Id = pmsgFriendDelFriendS.UllRoleId;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).CancleFriend(Id);

    }
    /*
    /// <summary> 신청 친구 리스트에서 유효기간이 지난 신청에대한 삭제처리 요청 응답 </summary> 
    private void PMsgFriendRequestInvalidSHandler(PMsgFriendRequestInvalidS pmsgFriendRequestInvalidS)
    {
        uint ErrorCode = (uint)pmsgFriendRequestInvalidS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendRequestInvalidS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        ulong id = pmsgFriendRequestInvalidS.UllRoleId;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).InvalidCancleRecieveFriend(id);

    }

    /// <summary> 친구 추가 리스트에서 유효기간이 지난 신청에대한 삭제처리 요청 응답 </summary> 
    private void PMsgFriendSelfRequestInvalidSHandler(PMsgFriendSelfRequestInvalidS pmsgFriendSelfRequestInvalidS)
    {
        uint ErrorCode = (uint)pmsgFriendSelfRequestInvalidS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendSelfRequestInvalidS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }


        ulong id = pmsgFriendSelfRequestInvalidS.UllRoleId;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).InvalidCancleSendFriend(id);
    }
    */

    /// <summary> 친구에게 체력선물 요청 응답 </summary> 
    private void PMsgFriendGivePowerSHandler(PMsgFriendGivePowerS pmsgFriendGivePowerS)
    {
        SceneManager.instance.EndNetProcess("GiveHeart");

        uint ErrorCode = (uint)pmsgFriendGivePowerS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFriendGivePowerS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        ulong id = pmsgFriendGivePowerS.UllRoleId;
        ulong time = pmsgFriendGivePowerS.UllSendPowerTime;

                UIBasePanel friendPanel = UIMgr.GetUIBasePanel("UIPanel/SocialPanel");
        if (friendPanel != null)
            (friendPanel as SocialPanel).GiveHeart(id, time);

    }



    /// <summary> 경험치 던전 클리어 횟수 응답 </summary>
    private void PMsgExpBattleQuerySHandler(PMsgExpBattleQueryS pmsgExpBattleQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgExpBattleQueryC");
        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.ExpBattleCount, pmsgExpBattleQueryS.UnDailyComplete, 0);
        UIBasePanel specialPanel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (specialPanel != null)
            (specialPanel as DungeonPanel).OnOpenQuery();
    }

    /// <summary> 골드 던전 클리어 횟수 응답 </summary>
    private void PMsgCoinBattleQuerySHandler(PMsgCoinBattleQueryS pmsgCoinBattleQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgCoinBattleQueryC");
        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.GoldBattleCount, pmsgCoinBattleQueryS.UnDailyComplete, 0);
        UIBasePanel specialPanel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (specialPanel != null)
            (specialPanel as DungeonPanel).OnOpenQuery();
    }

    /// <summary> 경험치 던전 시작 응답 </summary>
    private void PMsgExpBattleStartSHandler(PMsgExpBattleStartS pmsgExpBattleStartS)
    {
        uint ErrorCode = (uint)pmsgExpBattleStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgExpBattleStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData.RewardData rewardData = new NetData.RewardData();

        //유저 정보 저장해놓는다.
        rewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref rewardData.SaveExp, ref rewardData.SaveMaxExp);

        rewardData.StageId = (uint)pmsgExpBattleStartS.UnExpBattleId;
        NetData.instance._RewardData = rewardData;

        Debug.Log(pmsgExpBattleStartS);
        //각 몹들의 경험치가 여기에 들어옴
        //UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        //if (partnerPanel != null)
        //{
        //    (partnerPanel as DungeonPanel).OnExpGameStart(pmsgExpBattleStartS);
        //}
    }

    /// <summary> 경험치던전 종료 응답 </summary>
    private void PMsgExpBattleCompleteSHandler(PMsgExpBattleCompleteS pmsgExpBattleCompleteS)
    {
        uint ErrorCode = (uint)pmsgExpBattleCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgExpBattleCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgExpBattleCompleteS.UnVictory == 1)//성공
        {
            int expTotal = pmsgExpBattleCompleteS.UnExpTotal;
            NetData.instance._RewardData.GetExp = (uint)expTotal;
            //UIMgr.Open("UIPanel/ResultRewardStarPanel", true);
        }
        else//실패
        {
            NetData.instance.ClearRewardData();
            //UIMgr.Open("UIPanel/ResultRewardStarPanel", false);
        }

        G_GameInfo.GameInfo.OpenResultPanel(pmsgExpBattleCompleteS.UnVictory == 1);
    }

    /// <summary> 골드던전 시작 응답 </summary>
    private void PMsgCoinBattleStartSHandler(PMsgCoinBattleStartS pmsgCoinBattleStartS)
    {
        uint ErrorCode = (uint)pmsgCoinBattleStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCoinBattleStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //Debug.Log("골드 던전 " + pmsgCoinBattleStartS);
        //각 몹들의 경험치가 여기에 들어옴
        //UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        //if (partnerPanel != null)
        //{
        //    (partnerPanel as DungeonPanel).OnGoldGameStart(pmsgCoinBattleStartS);
        //}
    }

    /// <summary> 골드던전 종료 응답 </summary>
    private void PMsgCoinBattleCompleteSHandler(PMsgCoinBattleCompleteS pmsgCoinBattleCompleteS)
    {
        uint ErrorCode = (uint)pmsgCoinBattleCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgCoinBattleCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgCoinBattleCompleteS.UnVictory == 1)//성공
        {
            NetData.instance._RewardData.GetCoin = pmsgCoinBattleCompleteS.UnCoinTotal;
            //UIMgr.Open("UIPanel/ResultRewardStarPanel", true);
        }
        else//실패
        {
            NetData.instance.ClearRewardData();
            //UIMgr.Open("UIPanel/ResultRewardStarPanel", false);
        }

        G_GameInfo.GameInfo.OpenResultPanel(pmsgCoinBattleCompleteS.UnVictory == 1);
    }

    /// <summary> 체력 구매(치트키로 사용중) 응답 </summary>
    private void PMsgBuyPowerSHandler(PMsgBuyPowerS pmsgBuyPowerS)
    {
        uint ErrorCode = (uint)pmsgBuyPowerS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgBuyPowerS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

    }

    /// <summary> 마계의탑 정보 조회 응답 </summary>
    private void PMsgTowerBattleQuerySHandler(PMsgTowerBattleQueryS pmsgTowerBattleQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgTowerBattleQueryC");
        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.TowerCount, pmsgTowerBattleQueryS.UnDailyComplete, 0);
        NetData.instance.GetUserInfo().TowerFloor = pmsgTowerBattleQueryS.UnMaxLevel;
        //UIBasePanel towerPanel = UIMgr.GetUIBasePanel("UIPanel/TowerPanel");
        //if (towerPanel != null)
        //{
        //    (towerPanel as TowerPanel).OnTowerInit(pmsgTowerBattleQueryS.UnMaxLevel);
        //}
    }

    /// <summary> 마계의탑 시작 응답 </summary>
    private void PMsgTowerBattleStartSHandler(PMsgTowerBattleStartS pmsgTowerBattleStartS)
    {
        uint ErrorCode = (uint)pmsgTowerBattleStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTowerBattleStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData.instance.ClearRewardData();
        NetData.instance._RewardData.GetCoin = pmsgTowerBattleStartS.UnDropCoin;
        NetData.instance._RewardData.GetExp = (uint)pmsgTowerBattleStartS.UnDropExp;
        NetData.instance._RewardData.GetAsset = pmsgTowerBattleStartS.UnDropRoyalBadge;

        NetData.instance._RewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref NetData.instance._RewardData.SaveExp
            , ref NetData.instance._RewardData.SaveMaxExp);

        int count = pmsgTowerBattleStartS.CDrop.Count;
        for (int i = 0; i < count; i++)
        {
            TowerDropItem itemData = pmsgTowerBattleStartS.CDrop[i];
            NetData.instance._RewardData.GetList.Add(new NetData.DropItemData((uint)itemData.UnItemId, (uint)itemData.UnItemNum, itemData.UnType));
        }

        UIBasePanel towerPanel = UIMgr.GetUIBasePanel("UIPanel/TowerPanel");
        if (towerPanel != null)
            (towerPanel as TowerPanel).OnPMsgTowerBattleStartSHandler(pmsgTowerBattleStartS.UnTowerBattleId);
    }

    /// <summary> 마계의탑 랭킹 조회 응답 </summary>
    private void PMsgTowerRankQuerySHandler(PMsgTowerRankQueryS pmsgTowerRankQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgTowerRankQueryC");
        
        if (TownState.TownActive)
        {
            List<NetData.TowerRankData> rankList = new List<NetData.TowerRankData>();
            int count = pmsgTowerRankQueryS.CRank.Count;
            for (int i = 0; i < count; i++)
            {
                TowerRank rankData = pmsgTowerRankQueryS.CRank[i];
                NetData.TowerRankData data;
                data.RankNumber = (byte)rankData.UnRank;
                data.RoleId = (ulong)rankData.UnRoleId;
                data.CharLowData = (uint)rankData.UnRoleType;
                data.Name = rankData.SzName;
                data.Seconds = (uint)rankData.UnData;

                rankList.Add(data);
            }

            UIBasePanel towerPanel = UIMgr.GetUIBasePanel("UIPanel/TowerPanel");
            if (towerPanel != null)
                (towerPanel as TowerPanel).OnPMsgTowerRankQuerySHandler(rankList);
        }
        else
        {
            string msg = "";
            long userRoleId = (long)NetData.instance.GetUserInfo()._charUUID;
            int count = pmsgTowerRankQueryS.CRank.Count;
            for (int i = 0; i < count; i++)
            {
                TowerRank rankData = pmsgTowerRankQueryS.CRank[i];
                if (rankData.UnRoleId != userRoleId)
                    continue;

                //(uint)rankData.UnData
                //순위권 내
                msg = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(161), (byte)rankData.UnRank);
                break;
            }

            if (string.IsNullOrEmpty(msg))//순위권 밖
                msg = _LowDataMgr.instance.GetStringCommon(193);

            UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/ResultRewardStarPanel");
            if (basePanel != null)
                basePanel.NetworkData(MSG_DEFINE._MSG_TOWER_RANK_QUERY_S, msg);
        }
    }

    /// <summary> 해당 층 클리어 최단 시간 결과값 </summary>
    private void PMsgTowerUseTimeQuerySHandler(PMsgTowerUseTimeQueryS pmsgTowerUseTimeQueryS)
    {
        Debug.Log(pmsgTowerUseTimeQueryS);
        SceneManager.instance.EndNetProcess("PMsgTowerUseTimeQueryC");
        int towerLv = pmsgTowerUseTimeQueryS.UnLevel;
        int time = pmsgTowerUseTimeQueryS.UnUseTime;
        
        UIBasePanel resultPanel = UIMgr.GetUIBasePanel("UIPanel/ResultRewardStarPanel");
        if (resultPanel != null)
            resultPanel.NetworkData(MSG_DEFINE._MSG_TOWER_USE_TIME_QUERY_S, towerLv, time);
    }

    /// <summary> 마계의탑 클리어 응답 </summary>
    private void PMsgTowerBattleCompleteSHandler(PMsgTowerBattleCompleteS pmsgTowerBattleCompleteS)
    {
        SceneManager.instance.EndNetProcess("PMsgTowerBattleCompleteC");
        uint ErrorCode = (uint)pmsgTowerBattleCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTowerBattleCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        bool isVictory = pmsgTowerBattleCompleteS.UnVictory == 0 ? false : true;
        if (!isVictory)
        {
            NetData.instance.ClearRewardData();
        }

        SendPMsgTowerBattleQueryC();//마탑 층수 정보 갱신
        G_GameInfo.GameInfo.OpenResultPanel(isVictory);
    }

    /// <summary> 레이드 조회 응답 </summary>
    private void PMsgBossBattleQuerySHandler(PMsgBossBattleQueryS pmsgBossBattleQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgBossBattleQueryC");
        int battleType = pmsgBossBattleQueryS.UnBattleType;//Group ID
        int completeCount = pmsgBossBattleQueryS.UnDailyComplete;

        EtcID key = EtcID.BossRaid3Count;
        if (battleType == 1)
            key = EtcID.BossRaid1Count;
        else if (battleType == 2)
            key = EtcID.BossRaid2Count;

        NetData.instance.GetUserInfo().SetCompleteCount(key, completeCount, 0);

        //UIBasePanel specialPanel = UIMgr.GetUIBasePanel("UIPanel/SpecialPanel");
        //if (specialPanel != null)
        //    (specialPanel as SpecialPanel).OnOpenQuery();
        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (panel != null)
            (panel as DungeonPanel).OnOpenQuery();
    }

    /// <summary> 레이드 시작 응답 </summary>
    private void PMsgBossBattleStartSHandler(PMsgBossBattleStartS pmsgBossBattleStartS)
    {
        uint ErrorCode = (uint)pmsgBossBattleStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgBossBattleStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //pmsgBossBattleStartS.UnHeroIdList//이런게 있음 데려가는 파트너임
        NetData.instance.ClearRewardData();
        NetData.instance._RewardData.GetCoin = pmsgBossBattleStartS.UnDropCoin;
        NetData.instance._RewardData.GetExp = (uint)pmsgBossBattleStartS.UnDropExp;

        NetData.instance._RewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref NetData.instance._RewardData.SaveExp
            , ref NetData.instance._RewardData.SaveMaxExp);

        int count = pmsgBossBattleStartS.CDrop.Count;
        for (int i = 0; i < count; i++)
        {
            BossDropItem itemData = pmsgBossBattleStartS.CDrop[i];
            NetData.instance._RewardData.GetList.Add(new NetData.DropItemData((uint)itemData.UnItemId, (uint)itemData.UnItemNum, itemData.UnType));
        }

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (panel != null)
            (panel as DungeonPanel).OnRaidGameStart(pmsgBossBattleStartS.UnBossBattleId);
    }

    /// <summary> 레이드 클리어 응답 </summary>
    private void PMsgBossBattleCompleteSHandler(PMsgBossBattleCompleteS pmsgBossBattleCompleteS)
    {
        uint ErrorCode = (uint)pmsgBossBattleCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgBossBattleCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        bool isVictory = pmsgBossBattleCompleteS.UnVictory == 0 ? false : true;
        if (!isVictory)
        {
            NetData.instance.ClearRewardData();
        }

        //UIMgr.Open("UIPanel/ResultRewardStarPanel", isVictory);
        G_GameInfo.GameInfo.OpenResultPanel(isVictory);
    }


    /// <summary> 상점 정보 응답 </summary>
    private void PMsgShopInfoQuerySHandler(PMsgShopInfoQueryS pmsgShopInfoQueryS)
    {

        SceneManager.instance.EndNetProcess("GetShopInfo");
        Debug.Log(pmsgShopInfoQueryS);
        UIBasePanel shopPanel = UIMgr.GetUIBasePanel("UIPanel/ShopPanel");
        uint ErrorCode = (uint)pmsgShopInfoQueryS.NErrorCode;

        if (ErrorCode == 1190)  //삼정설정이 없는것 -> 걍 빈화면보이게해줘
        {
            (shopPanel as ShopPanel).ViewInfo(pmsgShopInfoQueryS.UnType, null);
            return;
        }
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgShopInfoQueryS.GetType().ToString() + "-" + GetErrorString(ErrorCode));

            return;
        }


        //물품정보 
        List<NetData.ShopItemInfo> Info = new List<NetData.ShopItemInfo>();
        for (int i = 0; i < pmsgShopInfoQueryS.CItemInfo.Count; i++)
        {
            ShopItemInfo info = pmsgShopInfoQueryS.CItemInfo[i];
            NetData.ShopItemInfo shop = new NetData.ShopItemInfo(info.UllIndex, info.UnGoods, info.UnNum, info.UnDbIndex);
            Info.Add(shop);
        }

        NetData.ShopItemInfoData shopInfo = new NetData.ShopItemInfoData(pmsgShopInfoQueryS.UnType, pmsgShopInfoQueryS.UllRefreshTimer, pmsgShopInfoQueryS.UllManualRefreshTimer,
                                               pmsgShopInfoQueryS.UnManualRefreshCount, pmsgShopInfoQueryS.UnItemCount, Info);

        if (shopPanel != null)
        {
            (shopPanel as ShopPanel).ViewInfo(pmsgShopInfoQueryS.UnType, shopInfo);
            return;
        }


    }
    /// <summary>판매 물품 구매 요청응답</summary>
    private void PMsgShopByItemSHandler(PMsgShopByItemS pmsgShopByItemS)
    {

        SceneManager.instance.EndNetProcess("PurchaseShopItme");

        uint ErrorCode = (uint)pmsgShopByItemS.NErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgShopByItemS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }



        UIBasePanel shopPanel = UIMgr.GetUIBasePanel("UIPanel/ShopPanel");
        if (shopPanel != null)
            (shopPanel as ShopPanel).ShopView(pmsgShopByItemS.UnShopType);



    }
    /// <summary>판매 물품 리스트 리셋 요청 응답</summary>
    private void PMsgShopRefreshSHandler(PMsgShopRefreshS pmsgShopRefreshS)
    {
        SceneManager.instance.EndNetProcess("RefreshShop");
        uint ErrorCode = (uint)pmsgShopRefreshS.NErrorCode;
      
        UIBasePanel shopPanel = UIMgr.GetUIBasePanel("UIPanel/ShopPanel");
        if (ErrorCode == 1200)  //삼정설정이 없는것 -> 걍 빈화면보이게해줘
        {
            (shopPanel as ShopPanel).ViewInfo(pmsgShopRefreshS.UnShopType, null);
            return;
        }
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgShopRefreshS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //UIBasePanel shopPanel = UIMgr.GetUIBasePanel("UIPanel/ShopPanel");
        if (shopPanel != null)
            (shopPanel as ShopPanel).ShopView(pmsgShopRefreshS.UnShopType);

    }

    /// <summary> VIP정보 조회 응답 </summary>
    private void PMsgVipQueryInfoSHandler(PMsgVipQueryInfoS pmsgVipQueryInfoS)
    {
        //uint ErrorCode = (uint)pmsgVipQueryInfoS.UnErrorCode;

        //if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        //{
        //    UIMgr.instance.AddErrorPopup((int)ErrorCode); 
        //    Debug.Log(pmsgVipQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
        //    return;
        //}

        uint lv = pmsgVipQueryInfoS.UnVipLevel; //vip등급
        uint exp = pmsgVipQueryInfoS.UnVipExp;  //vip경험치
        uint ticketTime = pmsgVipQueryInfoS.UnSweepTicketTime;  //가장최근 소탕권 수령시간 yyyymmdd
        uint RepairCnt = pmsgVipQueryInfoS.UnRepairSignInCount; //당우러 출석 메꾼 횟수 yyyymmdd

        UIBasePanel VopPopup = UIMgr.GetUIBasePanel("UIPanel/VipPanel");
        if (VopPopup != null)
            (VopPopup as VipPopup).VipPopUpSetting(lv, exp, ticketTime, RepairCnt);

    }

    /// <summary> 매일 소탕권 수동 수령 요청 응답 </summary>
    private void PMsgVipFetchSweepTicketSHandler(PMsgVipFetchSweepTicketS pmsgVipFetchSweepTicketS)
    {
        uint ErrorCode = (uint)pmsgVipFetchSweepTicketS.UnErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgVipFetchSweepTicketS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint cnt = pmsgVipFetchSweepTicketS.UnCount;    //획득 소탕권 수량

        UIBasePanel VopPopup = UIMgr.GetUIBasePanel("UIPanel/VipPanel");
        //  if (VopPopup != null)
        // (VopPopup as VipPopup).VipPopUpSetting(lv, exp, ticketTime, RepairCnt);

    }
    /// <summary> 출석 메꾸기 요청 응답 </summary>
    private void PMsgVipRepairSignInSHandler(PMsgVipRepairSignInS pmsgVipRepairSignInS)
    {
        uint ErrorCode = (uint)pmsgVipRepairSignInS.UnErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgVipRepairSignInS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }

    /// <summary> 랭킹 </summary>
    private void PMsgRankQuerySHandler(PMsgRankQueryS pmsgRankQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgRankQueryC");
        Debug.Log("pmsgRankQueryS " + pmsgRankQueryS);

        int rankType = pmsgRankQueryS.UnRankType;
        int myRank = pmsgRankQueryS.UnMyRank;
        long myValue = pmsgRankQueryS.UllMyData;
        //int Cnt = pmsgRankQueryS.UnCount;

        List<NetData.RankInfo> Info = new List<NetData.RankInfo>();
        for (int i = 0; i < pmsgRankQueryS.CInfo.Count; i++)
        {
            RankInfo info = pmsgRankQueryS.CInfo[i];
            NetData.RankInfo Rank = new NetData.RankInfo(info.UnRank, (ulong)info.UllRoleId, info.UnRoleType, info.SzName, info.UnLevel, info.UllData, info.UnVipLevel);
            Info.Add(Rank);
        }

        UIBasePanel RankPanel = UIMgr.GetUIBasePanel("UIPanel/RankingPanel");
        if (RankPanel != null)
            (RankPanel as RankPanel).GetRankList(rankType, myRank, myValue, Info);

        //길드
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (basePanel != null)
            (basePanel as GuildJoinPanel).OnPMsgRankList(pmsgRankQueryS.UnCount, Info);
    }
    /// <summary> 길드랭킹 </summary>
    private void PMsgRankGuildQuerySHandler(PMsgRankGuildQueryS pmsgRankGuildQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgRankGuildQueryC");

        int rankType = pmsgRankGuildQueryS.UnRankType;
        int myRank = pmsgRankGuildQueryS.UnMyRank;
        long myValue = pmsgRankGuildQueryS.UllMyData;
        int cnt = pmsgRankGuildQueryS.UnCount;
        Debug.Log("pmsgRankGuildQueryS " + pmsgRankGuildQueryS);
        List<NetData.RankInfo> Info = new List<NetData.RankInfo>();
        for (int i = 0; i < pmsgRankGuildQueryS.CInfo.Count; i++)
        {
            RankInfo info = pmsgRankGuildQueryS.CInfo[i];
            NetData.RankInfo Rank = new NetData.RankInfo(info.UnRank, (ulong)info.UllRoleId, info.UnRoleType, info.SzName, info.UnLevel, info.UllData, info.UnVipLevel);
            Info.Add(Rank);
        }

        UIBasePanel rankPanel = UIMgr.GetUIBasePanel("UIPanel/RankingPanel");
        if (rankPanel != null && !rankPanel.IsHidePanel)
            (rankPanel as RankPanel).GetRankList(rankType, myRank, myValue, Info);
        
        //길드
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GuildPanel");
        if (basePanel != null && !basePanel.IsHidePanel)
            (basePanel as GuildPanel).OnPMsgRankList(myRank, cnt, Info);

        //길드
        UIBasePanel guildJoinPanel = UIMgr.GetUIBasePanel("UIPanel/GuildJoinPanel");
        if (guildJoinPanel != null && !guildJoinPanel.IsHidePanel)
            (guildJoinPanel as GuildJoinPanel).OnPMsgRankList(cnt, Info);
    }
    

    private void PMsgTaskQueryInfoSHandler(PMsgTaskQueryInfoS pmsgTaskQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgTaskQueryInfoC");
        uint ErrorCode = (uint)pmsgTaskQueryInfoS.NErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTaskQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgTaskQueryInfoS.UnTaskCount == 0)
        {
            //퀘스트 목록이 한개도없다 처음퀘스트를 받아야한다
            SendPMsgTaskReceiveTaskC(1);
        }
        else
        {
            //퀘스트 목록이 있다 
            QuestManager.instance.SetQuestInfo(pmsgTaskQueryInfoS);
        }

        UIBasePanel panel = UIMgr.GetTownBasePanel();
        if (panel != null)
            (panel as TownPanel).MissionListSetting();

        QuestManager.instance.CheckMissingQuest();
     
    }

    private void PMsgTaskReceiveTaskSHandler(PMsgTaskReceiveTaskS pmsgTaskReceiveTaskS)
    {
        uint ErrorCode = (uint)pmsgTaskReceiveTaskS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success && ErrorCode != (int)Sw.ErrorCode.ER_TASK_RECEIVE_TASK_REPEAT)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTaskReceiveTaskS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }

    private void PMsgTaskCompleteSHandler(PMsgTaskCompleteS pmsgTaskCompleteS)
    {
        uint ErrorCode = (uint)pmsgTaskCompleteS.NErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTaskCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //완료가 되었으니 보상 받기
        if(Debug.isDebugBuild && CheatPopup.IsStayProtocol)
        {
            CheatPopup.IsStayProtocol = false;
        }

        ///일단 보류
        //SceneManager.instance.SetNoticePanel(NoticeType.Quest, pmsgTaskCompleteS.UnTaskId);

        //SendPMsgTaskFetchBonusC(pmsgTaskCompleteS.UnTaskId);
    }

    private void PMsgTaskFetchBonusSHandler(PMsgTaskFetchBonusS pmsgTaskFetchBonusS)
    {
        //Quest.QuestInfo lowData = _LowDataMgr.instance.GetLowDataQuestData(pmsgTaskFetchBonusS.UnTaskId);
        SceneManager.instance.SetNoticePanel(NoticeType.GetMailItem, pmsgTaskFetchBonusS.UnTaskId);
     
        QuestManager.instance.QuestCompleteAfterProcess(pmsgTaskFetchBonusS.UnTaskId);

        //퀘스트 완료 보너스를 받았으니 퀘스트를 검색하여 새로운 퀘스트를 받자
        QuestManager.instance.SearchNextQuest(pmsgTaskFetchBonusS.UnTaskId);
    }

    private void PMsgDailyTaskQueryInfoSHandler(PMsgDailyTaskQueryInfoS pmsgDailyTaskQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgDailyTaskQueryInfoC");
        uint ErrorCode = (uint)pmsgDailyTaskQueryInfoS.NErrorCode;

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgDailyTaskQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgDailyTaskQueryInfoS.UnTaskCount == 0)
        {
            //일일퀘스트 

        }
        else
        {

        }
    }

    /// <summary> 상자뽑기 정보 조회 응답 </summary>
    private void PMsgLotteryQueryInfoSHandler(PMsgLotteryQueryInfoS pmsgLotteryQueryInfoS)
    {
        uint norFreeTime = pmsgLotteryQueryInfoS.UnCommonFreeTime;
        uint seniorFreeTime = pmsgLotteryQueryInfoS.UnSeniorFreeTime;

        SceneManager.instance.SetGachaFreeTime(norFreeTime, seniorFreeTime);

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        //UIBasePanel shopPanel = UIMgr.GetUIBasePanel("UIPanel/ShopPanel");
        if (basePanel != null)
        {
            (basePanel as GachaPanel).OnTimeUpdate(norFreeTime, seniorFreeTime);
        }
        //else
        //{
        //    if(shopPanel!=null)
        //    {
        //        UIMgr.OpenGachaPanel(norFreeTime, seniorFreeTime);
        //        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        //        if (townPanel != null)
        //            townPanel.Hide();
        //    }

        //}
    }

    /// <summary> 무료 보통 뽑기 요청 응답 </summary>
    private void PMsgLotteryBoxCommonFreeSHandler(PMsgLotteryBoxCommonFreeS pmsgLotteryBoxCommonFreeS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxCommonFreeS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxCommonFreeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgLotteryBoxCommonFreeS.GoodsInfo.Count <= 0)//혹시 모를 예외처리
            return;

        //무조건 1개임. 그 이상 표현할 UI가 없음
        LotteryGoodsInfo info = pmsgLotteryBoxCommonFreeS.GoodsInfo[0];
        NetData.DropItemData itemData = new NetData.DropItemData(info.UnGoods, (uint)info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnOnce(true, false, itemData);
    }

    /// <summary> 유료 보통 뽑기 요청 응답 </summary>
    private void PMsgLotteryBoxCommonSHandler(PMsgLotteryBoxCommonS pmsgLotteryBoxCommonS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxCommonS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxCommonS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgLotteryBoxCommonS.GoodsInfo.Count <= 0)//혹시 모를 예외처리
            return;

        //무조건 1개임. 그 이상 표현할 UI가 없음
        LotteryGoodsInfo info = pmsgLotteryBoxCommonS.GoodsInfo[0];
        NetData.DropItemData itemData = new NetData.DropItemData(info.UnGoods, (uint)info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnOnce(false, false, itemData);
    }

    /// <summary> 보통 10연뽑 요청 응답 </summary>
    private void PMsgLotteryBoxCommonManytimesSHandler(PMsgLotteryBoxCommonManytimesS pmsgLotteryBoxCommonManytimesS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxCommonManytimesS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxCommonManytimesS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        int count = pmsgLotteryBoxCommonManytimesS.GoodsInfo.Count;
        if (count <= 0)//혹시 모를 예외처리
            return;

        NetData.DropItemData[] itemDataArr = new NetData.DropItemData[count];
        for (int i = 0; i < count; i++)
        {
            LotteryGoodsInfo info = pmsgLotteryBoxCommonManytimesS.GoodsInfo[i];
            itemDataArr[i] = new NetData.DropItemData(info.UnGoods, (uint)info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnTen(false, itemDataArr);
    }

    /// <summary> 무료 고급 뽑기 요청 응답 </summary>
    private void PMsgLotteryBoxSeniorFreeSHandler(PMsgLotteryBoxSeniorFreeS pmsgLotteryBoxSeniorFreeS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxSeniorFreeS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxSeniorFreeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgLotteryBoxSeniorFreeS.GoodsInfo.Count <= 0)//혹시 모를 예외처리
            return;

        //무조건 1개임. 그 이상 표현할 UI가 없음
        LotteryGoodsInfo info = pmsgLotteryBoxSeniorFreeS.GoodsInfo[0];
        NetData.DropItemData itemData = new NetData.DropItemData(info.UnGoods, (uint)info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnOnce(true, true, itemData);
    }

    /// <summary> 유료 고급뽑기 요청 응답 </summary>
    private void PMsgLotteryBoxSeniorSHandler(PMsgLotteryBoxSeniorS pmsgLotteryBoxSeniorS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxSeniorS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxSeniorS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (pmsgLotteryBoxSeniorS.GoodsInfo.Count <= 0)//혹시 모를 예외처리
            return;

        //무조건 1개임. 그 이상 표현할 UI가 없음
        LotteryGoodsInfo info = pmsgLotteryBoxSeniorS.GoodsInfo[0];
        NetData.DropItemData itemData = new NetData.DropItemData(info.UnGoods, (uint)info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnOnce(false, true, itemData);
    }

    /// <summary> 고급 10연뽑 요청 응답 </summary>
    private void PMsgLotteryBoxSeniorManytimesSHandler(PMsgLotteryBoxSeniorManytimesS pmsgLotteryBoxSeniorManytimesS)
    {
        uint ErrorCode = (uint)pmsgLotteryBoxSeniorManytimesS.NErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgLotteryBoxSeniorManytimesS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        int count = pmsgLotteryBoxSeniorManytimesS.GoodsInfo.Count;
        if (count <= 0)//혹시 모를 예외처리
            return;

        NetData.DropItemData[] itemDataArr = new NetData.DropItemData[count];
        for (int i = 0; i < count; i++)
        {
            LotteryGoodsInfo info = pmsgLotteryBoxSeniorManytimesS.GoodsInfo[i];
            itemDataArr[i] = new NetData.DropItemData(info.UnGoods, info.UnNum, (int)info.UnType);//데이터가 비슷한 형태이므로 이거를 사용.
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
        if (basePanel != null)
            (basePanel as GachaPanel).OnTen(true, itemDataArr);
    }

    /// <summary> 챕터 보상 조회 응답 </summary>
    private void PMsgStageChapterQuerySHandler(PMsgStageChapterQueryS pmsgStageChapterQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgStageChapterQueryC");
        int count = pmsgStageChapterQueryS.CInfo.Count;
        List<NetData.StageStarRewardData> list = new List<NetData.StageStarRewardData>();
        for (int i = 0; i < count; i++)
        {
            PMsgStageChapterQueryS.Types.ChapterInfo info = pmsgStageChapterQueryS.CInfo[i];
            list.Add(new NetData.StageStarRewardData(info.UnBoxId + 1, info.UnStar, info.UnChapterId, pmsgStageChapterQueryS.UnStageType));
        }
        Debug.Log("pmsgStageChapterQueryS" + pmsgStageChapterQueryS);

        //일반/하드일때 두번들어옴
        if(pmsgStageChapterQueryS.UnStageType ==1)
        {
            NetData.instance.GetUserInfo().StageStarReward = list;
        }
        else if(pmsgStageChapterQueryS.UnStageType ==2)
        {
            NetData.instance.GetUserInfo().HardStageStarReward = list;
        }

    }

    /// <summary> 챕터 보상 획득 응답 </summary>
    private void PMsgStageChapterRewardSHandler(PMsgStageChapterRewardS pmsgStageChapterRewardS)
    {
        uint ErrorCode = (uint)pmsgStageChapterRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgStageChapterRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        SceneManager.instance.SetNoticePanel(NoticeType.Message, 889, null);

        if (pmsgStageChapterRewardS.UnStageType == 1)
        {
            for (int i = 0; i < NetData.instance.GetUserInfo().StageStarReward.Count; i++)
            {
                if (NetData.instance.GetUserInfo().StageStarReward[i].ChapterID != pmsgStageChapterRewardS.UnChapterId)
                    continue;

                if (NetData.instance.GetUserInfo().StageStarReward[i].StageType != pmsgStageChapterRewardS.UnStageType)
                    continue;

                NetData.instance.GetUserInfo().StageStarReward[i] = new NetData.StageStarRewardData(pmsgStageChapterRewardS.UnBoxId + 1, NetData.instance.GetUserInfo().StageStarReward[i].Value, pmsgStageChapterRewardS.UnChapterId, pmsgStageChapterRewardS.UnStageType);
                break;
            }
        }
        else if(pmsgStageChapterRewardS.UnStageType == 2)
        {
            for (int i = 0; i < NetData.instance.GetUserInfo().HardStageStarReward.Count; i++)
            {
                if (NetData.instance.GetUserInfo().HardStageStarReward[i].ChapterID != pmsgStageChapterRewardS.UnChapterId)
                    continue;

                if (NetData.instance.GetUserInfo().HardStageStarReward[i].StageType != pmsgStageChapterRewardS.UnStageType)
                    continue;

                NetData.instance.GetUserInfo().HardStageStarReward[i] = new NetData.StageStarRewardData(pmsgStageChapterRewardS.UnBoxId + 1, NetData.instance.GetUserInfo().StageStarReward[i].Value, pmsgStageChapterRewardS.UnChapterId, pmsgStageChapterRewardS.UnStageType);
                break;
            }
        }



        UIBasePanel chapter = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
        if (chapter != null)
            (chapter as ChapterPanel).OnPMsgStageChapterRewardSHandler(pmsgStageChapterRewardS.UnChapterId, pmsgStageChapterRewardS.UnBoxId + 1);
    }

    /// <summary> 코스튬 보기 안보기 응답 </summary>
    private void PMsgCostumeShowFlagSHandler(PMsgCostumeShowFlagS pmsgCostumeShowFlagS)
    {
        bool isHide = pmsgCostumeShowFlagS.UnShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE;

        UIBasePanel equipPanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        
        if (equipPanel != null && equipPanel.gameObject.activeSelf)
        {
            equipPanel.NetworkData(MSG_DEFINE._MSG_COSTUME_SHOW_FLAG_C, isHide);
        }
        
        NetData.instance.GetUserInfo().isHideCostum = isHide;
    }

    /// <summary> 칭호 정보요청 응답 </summary>
    private void PMsgTitleQueryInfoSHandler(PMsgTitleQueryInfoS pmsgTitleQueryInfoS)
    {
       

        SceneManager.instance.EndNetProcess("PMsgTitleQueryInfoC");
        uint ErrorCode = (uint)pmsgTitleQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgTitleQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint lowDataIdL = 0;
        uint lowDataIdR = 0;
        int count = pmsgTitleQueryInfoS.CTitleInfo.Count;
        List<uint> list = new List<uint>();
        for (int i = 0; i < count; i++)
        {
            if (pmsgTitleQueryInfoS.CTitleInfo[i].UnInUse == 1)//1사용중
            {
                Title.TitleInfo titleInfo = _LowDataMgr.instance.GetLowDataTitle(pmsgTitleQueryInfoS.CTitleInfo[i].UnTitleId);
                if (titleInfo.Type == 1)
                    lowDataIdL = titleInfo.Id;
                else if (titleInfo.Type == 2)
                    lowDataIdR = titleInfo.Id;
            }

            list.Add(pmsgTitleQueryInfoS.CTitleInfo[i].UnTitleId);
        }

        NetData.instance.GetUserInfo().SetTitleLowDataId(lowDataIdL, lowDataIdR);

        UIBasePanel titlePanel = UIMgr.GetUIBasePanel("UIPopup/SelectTitlePopup");
        if (titlePanel != null)
        {
            (titlePanel as SelectTitlePopup).OnPMsgReciveGetList(list);
        }

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        if (townPanel != null && townPanel.gameObject.activeSelf)
        {
            (townPanel as TownPanel).ChangeUnitTitle(NetData.instance.GetUserInfo()._charUUID, lowDataIdL, lowDataIdR);
        }
    }

    /// <summary> 칭호 변경요청 응답 </summary>
    private void PMsgUseTitleSHandler(PMsgUseTitleS pmsgUseTitleS)
    {

       

        uint ErrorCode = (uint)pmsgUseTitleS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgUseTitleS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        
        uint newTitleId = 0;
        uint lowDataIdL = NetData.instance.GetUserInfo()._LeftTitle;
        uint lowDataIdR = NetData.instance.GetUserInfo()._RightTitle;
        Title.TitleInfo titleInfo = _LowDataMgr.instance.GetLowDataTitle(pmsgUseTitleS.UnTitleId);
        if (titleInfo.Type == 1)
        {
            newTitleId = pmsgUseTitleS.UnAdorn == 2 ? 1 : titleInfo.Id;
            lowDataIdL = pmsgUseTitleS.UnAdorn == 2 ? 0 : titleInfo.Id;
        }
        else if (titleInfo.Type == 2)
        {
            newTitleId = pmsgUseTitleS.UnAdorn == 2 ? 2 : titleInfo.Id;
            lowDataIdR = pmsgUseTitleS.UnAdorn == 2 ? 0 : titleInfo.Id;
        }

        NetData.instance.GetUserInfo().SetTitleLowDataId(lowDataIdL, lowDataIdR);

        UIBasePanel titlePanel = UIMgr.GetUIBasePanel("UIPopup/SelectTitlePopup");
        if (titlePanel != null)
        {
            (titlePanel as SelectTitlePopup).OnPMsgChangeTitle(newTitleId);
        }

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        if (townPanel != null)
        {
            (townPanel as TownPanel).ChangeUnitTitle(NetData.instance.GetUserInfo()._charUUID, lowDataIdL, lowDataIdR);
        }
    }
    
    /// <summary> 칭호 획득 알림 </summary>
    private void PMsgSynNewTitleSHandler(PMsgSynNewTitleS pmsgSynNewTitleS)
    {
       


        Title.TitleInfo info = _LowDataMgr.instance.GetLowDataTitle(pmsgSynNewTitleS.UnTitleId);

        string noti = string.Format(_LowDataMgr.instance.GetStringCommon(956), _LowDataMgr.instance.GetLowDataTitleName(info.TitleName) );
        SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, noti);
        UIMgr.AddLogChat(noti);
    }

    /// <summary> 당월 출석정보조회 응답 </summary>
    private void PMsgSignInQueryInfoSHandler(PMsgSignInQueryInfoS pmsgSignInQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("SignInInfo");

        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");

        uint ErrorCode = (uint)pmsgSignInQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if(benefitPanel!=null)
            {
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
                Debug.Log(pmsgSignInQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            }
            return;
        }

        Debug.Log(pmsgSignInQueryInfoS);

        uint signInfo = pmsgSignInQueryInfoS.UnSingnInInfo;
        uint time = pmsgSignInQueryInfoS.UnSigninTime;
        uint fillSign = pmsgSignInQueryInfoS.UnFillInSign;
        
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowDailySignIn(signInfo, time, fillSign);
        }
        else
        {
            bool isAlreadyAttend = false;
            if (0 < time)
            {
                System.DateTime dateTime = System.DateTime.ParseExact(time.ToString(), "yyyyMMdd", null);
                if (dateTime.Year == System.DateTime.Now.Year && dateTime.Month == System.DateTime.Now.Month
                    && dateTime.Day == System.DateTime.Now.Day)//오늘 이미 할당 받음.
                    isAlreadyAttend = true;
            }
            SceneManager.instance.SetAlram(AlramIconType.BENEFIT, !isAlreadyAttend );
            SceneManager.instance.SetBenefitAlram(0, !isAlreadyAttend);

            //UIBasePanel townPanel = UIMgr.GetTownBasePanel();
            //if(townPanel != null && townPanel.gameObject.activeSelf)
            //{
            //    (townPanel as TownPanel).CheckAttendTutorial(isAlreadyAttend);
            //}

        }

    }


    /// <summary> 출석체크 응답 </summary>
    private void PMsgSignInSHandler(PMsgSignInS pmsgSignInS)
    {
        SceneManager.instance.EndNetProcess("SignIn");

        uint ErrorCode = (uint)pmsgSignInS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgSignInS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint signInfo = pmsgSignInS.UnSingnInInfo;

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (benefitPanel != null)
        {
            NetworkClient.instance.SendPMsgSignInQueryInfoC();   //출첵 

            //(benefitPanel as BenefitPanel).ShowDailySignIn(signInfo, 999, 999);
        }

        if (UIMgr.instance.IsActiveTutorial)
        {
            UIBasePanel tutoPanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
            if (tutoPanel == null || !(tutoPanel as TutorialPopup).IsNetworkDelay)
                return;

            //(tutoPanel as TutorialPopup).OnEndNetwork("Attendance", new object[] {
            //    signInfo
            //});
        }
    }

    /// <summary> 출석채우기 응답 </summary>
    private void PMsgFillInSignInSHandler(PMsgFillInSignInS pmsgFillInSignInS)
    {
        SceneManager.instance.EndNetProcess("FillSignIn");

        uint ErrorCode = (uint)pmsgFillInSignInS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgFillInSignInS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

    }


    /// <summary> 업적정보조회 요청 응답  </summary>
    private void PMsgAchieveQueryInfoSHandler(PMsgAchieveQueryInfoS pmsgAchieveQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("GetAchieveList");
        uint ErrorCode = (uint)pmsgAchieveQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveQueryInfoS);

        List<NetData.AchieveLevelInfo> list = new List<NetData.AchieveLevelInfo>(); //업적단계정보

        uint lvCount = pmsgAchieveQueryInfoS.UnLevelCount;  //업적단계정보수량
        for (int i = 0; i < pmsgAchieveQueryInfoS.CAchieveLevelInfo.Count; i++)
        {
            AchieveLevelInfo Info = pmsgAchieveQueryInfoS.CAchieveLevelInfo[i];
            NetData.AchieveLevelInfo info = new NetData.AchieveLevelInfo(Info.UnType, Info.UnSubType, Info.UnLevel, Info.UnComplete, Info.UnFetch);
            list.Add(info);
        }

        SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, false);
        for (int i = 0; i < list.Count; i++)
        {
            // 달성되있는데 보상안받은경우가 있다면 알림표시
            if (list[i].Complete == 1 && list[i].Fetch == 0)
            {
                SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, true);
                continue;
            }
        }


        uint roleP = pmsgAchieveQueryInfoS.UnRolePoints;    //캐릭터타입업적
        uint fightP = pmsgAchieveQueryInfoS.UnFightPoints;  //전투
       // uint equipP = pmsgAchieveQueryInfoS.UnEquipPoints;  //장비
        uint moneyP = pmsgAchieveQueryInfoS.UnMoneyPoints;  //경제
        uint playP = pmsgAchieveQueryInfoS.UnPlayPoints; //컨텐츠
        uint friendP = pmsgAchieveQueryInfoS.UnFriendPoints;//소셜
        uint vipP = pmsgAchieveQueryInfoS.UnVipPoints;


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).SetAchieveInfo(lvCount, list, roleP, fightP,  moneyP, playP, friendP, vipP);
        }


    }


    ///// <summary> 장비관련 업적데이터 통계정보조회  </summary>
    //private void PMsgAchieveEquipTotalQueryInfoSHandler(PMsgAchieveEquipTotalQueryInfoS pmsgAchieveEquipTotalQueryInfoS)
    //{
    //    SceneManager.instance.EndNetProcess("EquipAchieve");

    //    uint ErrorCode = (uint)pmsgAchieveEquipTotalQueryInfoS.UnErrorCode;
    //    if (ErrorCode != (int)Sw.ErrorCode.ER_success)
    //    {
    //        UIMgr.instance.AddErrorPopup((int)ErrorCode); 
    //        Debug.Log(pmsgAchieveEquipTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
    //        return;
    //    }

    //    List<ulong> data = new List<ulong>();
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnCommonEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnSeniorEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnPreciousEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnHeroEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnAncientEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnFameEquipTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnMaterialTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnEquipUpgradeTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnEquipAnnealTotal);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnEquipmentBreak);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnSeniorEquipAll);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnPreciousEquipAll);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnHeroEquipAll);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnAncientEquipAll);
    //    data.Add(pmsgAchieveEquipTotalQueryInfoS.UnFameEquipAll);


    //    UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
    //    if (achievePanel != null)
    //    {
    //        (achievePanel as AchievePanel)._AchieveDataTest(3, data);
    //    }

    //}

    /// <summary> 전투관련 업적데이터 통계정보  </summary>
    private void PMsgAchieveFightTotalQueryInfoSHandler(PMsgAchieveFightTotalQueryInfoS pmsgAchieveFightTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("BattleAchieve");

        uint ErrorCode = (uint)pmsgAchieveFightTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveFightTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveFightTotalQueryInfoS);

        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();
        dataDic.Add(1, pmsgAchieveFightTotalQueryInfoS.UnMaxCaromCount);
        dataDic.Add(2, pmsgAchieveFightTotalQueryInfoS.UnMaxDamage);
        dataDic.Add(3, pmsgAchieveFightTotalQueryInfoS.UnUseSkillTotal);
        dataDic.Add(4, pmsgAchieveFightTotalQueryInfoS.UnHeroUseSkillTotal);
        dataDic.Add(11, pmsgAchieveFightTotalQueryInfoS.UnKillMonsterTotal);
        dataDic.Add(12, pmsgAchieveFightTotalQueryInfoS.UnKillBossTotal);
        dataDic.Add(13, pmsgAchieveFightTotalQueryInfoS.UnKillOtherRoleTotal);
        dataDic.Add(21, pmsgAchieveFightTotalQueryInfoS.UnOneselfDieTotal);
        dataDic.Add(22, pmsgAchieveFightTotalQueryInfoS.UnOnselfReviveTotal);



        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(2, dataDic);
        }
    }

    /// <summary> 소셜관련 업적데이터통계정보 </summary>
    private void PMsgAchieveFriendTotalQueryInfoSHandler(PMsgAchieveFriendTotalQueryInfoS pmsgAchieveFriendTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("SocialAchieve");

        uint ErrorCode = (uint)pmsgAchieveFriendTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveFriendTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveFriendTotalQueryInfoS);

        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();

        dataDic.Add(1, pmsgAchieveFriendTotalQueryInfoS.UnSendPowerFriend);
        dataDic.Add(2, pmsgAchieveFriendTotalQueryInfoS.UnRecvPowerFriend);

        //테이블 미적용
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendTeamPvp);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnRecvPowerFriend);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendTeamFiveFlower);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendSpendGuildPray);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendSpendCionGuildDonate);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendSpendGemGuildDonate);
        //dataDic.Add(99, pmsgAchieveFriendTotalQueryInfoS.UnFriendGuildTaskCount);



        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(6, dataDic);
        }
    }
    /// <summary> 경제 관련 업적데이터 통계정보  </summary>
    private void PMsgAchieveMoneyTotalQueryInfoSHandler(PMsgAchieveMoneyTotalQueryInfoS pmsgAchieveMoneyTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("EconomyAchieve");

        uint ErrorCode = (uint)pmsgAchieveMoneyTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveMoneyTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveMoneyTotalQueryInfoS);
        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();

        dataDic.Add(1, pmsgAchieveMoneyTotalQueryInfoS.UnCoinTotal);
        dataDic.Add(2, pmsgAchieveMoneyTotalQueryInfoS.UnSpendCoinTotal);
        dataDic.Add(3, pmsgAchieveMoneyTotalQueryInfoS.UnGemTotal);
        dataDic.Add(4, pmsgAchieveMoneyTotalQueryInfoS.UnSpendGemTotal);

        dataDic.Add(11, pmsgAchieveMoneyTotalQueryInfoS.UnBuyGoodsTotal);
        dataDic.Add(12, pmsgAchieveMoneyTotalQueryInfoS.UnSellGoodsTotal);
        dataDic.Add(13, pmsgAchieveMoneyTotalQueryInfoS.UnComposeGoodsCount);
        dataDic.Add(14, pmsgAchieveMoneyTotalQueryInfoS.UnMaterialGoodsCount);
        dataDic.Add(15, pmsgAchieveMoneyTotalQueryInfoS.UnHeroChipCount);

        dataDic.Add(21, pmsgAchieveMoneyTotalQueryInfoS.UnFameTotal);
        dataDic.Add(22, pmsgAchieveMoneyTotalQueryInfoS.UnSpendFameTotal);
        dataDic.Add(23, pmsgAchieveMoneyTotalQueryInfoS.UnRoyalBadgeCount);
        dataDic.Add(24, pmsgAchieveMoneyTotalQueryInfoS.UnSpendRoyalBadge);
        dataDic.Add(25, pmsgAchieveMoneyTotalQueryInfoS.UnGuildContribution);
        dataDic.Add(26, pmsgAchieveMoneyTotalQueryInfoS.UnSpendGuildContribution);

        //아직테이블없음
        //dataDic.Add(99, pmsgAchieveMoneyTotalQueryInfoS.UnLionKing);
        //dataDic.Add(99, pmsgAchieveMoneyTotalQueryInfoS.UnSpendLionKing);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(4, dataDic);
        }
    }
    /// <summary> 컨텐츠 관련업적 데이터 통계정보 </summary>
    private void PMsgAchievePlayTotalQueryInfoSHandler(PMsgAchievePlayTotalQueryInfoS pmsgAchievePlayTotalQueryInfoS)
    {

        SceneManager.instance.EndNetProcess("ContentsAchieve");

        uint ErrorCode = (uint)pmsgAchievePlayTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchievePlayTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchievePlayTotalQueryInfoS);

        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();

        dataDic.Add(1, pmsgAchievePlayTotalQueryInfoS.UnAdventureCount);
        //dataDic.Add(2, pmsgAchievePlayTotalQueryInfoS.UnPvpRankCount);
        dataDic.Add(3, pmsgAchievePlayTotalQueryInfoS.UnTowerCount);

        //테이블없음
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnCoinStageCount);
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnExpStageCount);
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnFiveFlowerCount);

        dataDic.Add(11, pmsgAchievePlayTotalQueryInfoS.UnThreeTalentCount);
        dataDic.Add(12, pmsgAchievePlayTotalQueryInfoS.UnThreeTalentSimple);
        dataDic.Add(13, pmsgAchievePlayTotalQueryInfoS.UnThreeTalentCommon);
        dataDic.Add(14, pmsgAchievePlayTotalQueryInfoS.UnThreeTalentDifficulty);
        dataDic.Add(15, pmsgAchievePlayTotalQueryInfoS.UnThreeTalentArduous);

        //테이블없음
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnCompleteMultyBoss);
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnTeamPvpCount);

        dataDic.Add(21, pmsgAchievePlayTotalQueryInfoS.UnSinglePvp);
        dataDic.Add(22, pmsgAchievePlayTotalQueryInfoS.UnKillMessBossCount);

        //테이블업승
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnEighteenBronzeCount);
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnGuildManorCount);
        //dataDic.Add(99, pmsgAchievePlayTotalQueryInfoS.UnTheLionConference);


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(5, dataDic);
        }

    }

    /// <summary> 캐릭터 관련 업적 데이터 통계정보  </summary>
    private void PMsgAchieveRoleTotalQueryInfoSHandler(PMsgAchieveRoleTotalQueryInfoS pmsgAchieveRoleTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("CharAchieve");

        uint ErrorCode = (uint)pmsgAchieveRoleTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveRoleTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

                Debug.Log(pmsgAchieveRoleTotalQueryInfoS);

        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();

        dataDic.Add(1, pmsgAchieveRoleTotalQueryInfoS.UnRoleLevel);
        dataDic.Add(2, pmsgAchieveRoleTotalQueryInfoS.UnRankCount);
        dataDic.Add(3, pmsgAchieveRoleTotalQueryInfoS.UnFightCount);

        dataDic.Add(11, pmsgAchieveRoleTotalQueryInfoS.UnEquipSetCount);
        dataDic.Add(12, pmsgAchieveRoleTotalQueryInfoS.UnEquipUpgradeTotal);
        dataDic.Add(13, pmsgAchieveRoleTotalQueryInfoS.UnEquipAnnealTotal);

        dataDic.Add(21, pmsgAchieveRoleTotalQueryInfoS.UnSeniorEquipAll);
        dataDic.Add(22, pmsgAchieveRoleTotalQueryInfoS.UnPreciousEquipAll);
        dataDic.Add(23, pmsgAchieveRoleTotalQueryInfoS.UnHeroEquipAll);
        dataDic.Add(24, pmsgAchieveRoleTotalQueryInfoS.UnAncientEquipAll);
        dataDic.Add(25, pmsgAchieveRoleTotalQueryInfoS.UnFameEquipAll);

        dataDic.Add(31, pmsgAchieveRoleTotalQueryInfoS.UnIdentityCount);
        dataDic.Add(32, pmsgAchieveRoleTotalQueryInfoS.UnSkillEquipSetCount);
        dataDic.Add(33, pmsgAchieveRoleTotalQueryInfoS.UnSkillUpgradeCount);

        dataDic.Add(41, pmsgAchieveRoleTotalQueryInfoS.UnHeroCount);
        dataDic.Add(42, pmsgAchieveRoleTotalQueryInfoS.UnHeroRiseInRankCount);
        dataDic.Add(43, pmsgAchieveRoleTotalQueryInfoS.UnHeroSkillUpgradeCount);

        //없는것들
        //dataDic.Add(99, pmsgAchieveRoleTotalQueryInfoS.UnHeroAwakenCount);
        //dataDic.Add(99, pmsgAchieveRoleTotalQueryInfoS.UnCostumeAnnealCount);
        //dataDic.Add(99, pmsgAchieveRoleTotalQueryInfoS.UnEquipCostumeCount);
        //dataDic.Add(99, pmsgAchieveRoleTotalQueryInfoS.UnComposeCostumeCount);


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(1, dataDic);
        }

    }

    /// <summary> vip시스템 관련 업적데이터 통계정보  </summary>
    private void PMsgAchieveVipTotalQueryInfoSHandler(PMsgAchieveVipTotalQueryInfoS pmsgAchieveVipTotalQueryInfoS)
    {

        SceneManager.instance.EndNetProcess("VipAchieve");

        uint ErrorCode = (uint)pmsgAchieveVipTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveVipTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveVipTotalQueryInfoS);
        Dictionary<uint, ulong> dataDic = new Dictionary<uint, ulong>();

        dataDic.Add(1, pmsgAchieveVipTotalQueryInfoS.UnRechargeCoin);
        dataDic.Add(2, pmsgAchieveVipTotalQueryInfoS.UnRechargeGem);
        dataDic.Add(3, pmsgAchieveVipTotalQueryInfoS.UnCommonLottery);

        dataDic.Add(11, pmsgAchieveVipTotalQueryInfoS.UnCommonLottery);
        dataDic.Add(12, pmsgAchieveVipTotalQueryInfoS.UnSeniorLottery);
        dataDic.Add(13, pmsgAchieveVipTotalQueryInfoS.UnVipLevel);


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel)._AchieveDataTest(7, dataDic);
        }
    }
    /// <summary> 업적보상 획득요청  </summary>
    private void PMsgAchieveFetchAwardSHandler(PMsgAchieveFetchAwardS pmsgAchieveFetchAwardS)
    {
        SceneManager.instance.EndNetProcess("FetchAchieve");

        uint ErrorCode = (uint)pmsgAchieveFetchAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveFetchAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData.AchieveLevelInfo info = new NetData.AchieveLevelInfo(pmsgAchieveFetchAwardS.UnType, pmsgAchieveFetchAwardS.UnSubType, pmsgAchieveFetchAwardS.UnLevel, pmsgAchieveFetchAwardS.UnComplete, pmsgAchieveFetchAwardS.UnFetch);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetAfterAchieveMent(info);
        }

    }
    /// <summary> 업적 포인트 보상 획득 요청  </summary>
    private void PMsgAchieveFetchPointsAwardSHandler(PMsgAchieveFetchPointsAwardS pmsgAchieveFetchPointsAwardS)
    {
        SceneManager.instance.EndNetProcess("GetAchievePoint");

        uint ErrorCode = (uint)pmsgAchieveFetchPointsAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode); 
            Debug.Log(pmsgAchieveFetchPointsAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }

    /// <summary> 업적 포인트 누적치 정보 동기화   </summary>
    private void PMsgAchieveSynPointsTotalValueSHandler(PMsgAchieveSynPointsTotalValueS pmsgAchieveSynPointsTotalValueS)
    {
     // Debug.Log(pmsgAchieveSynPointsTotalValueS);

        //required uint32 unType = 1;     // 업적 대분류 成就大类型
        //required uint32 unPoints = 2;	// 포인트 누적치 积分


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetAfterAchieveMentPoint(pmsgAchieveSynPointsTotalValueS.UnType, pmsgAchieveSynPointsTotalValueS.UnPoints);
        }
    }

    /// <summary> 업적 누적치 정보 동기화   </summary>
    private void PMsgAchieveSynAchieveStatisValueSHandler(PMsgAchieveSynAchieveStatisValueS pnsgAchieveSynAchieveStatisValueS)
    {
    //  Debug.Log(pnsgAchieveSynAchieveStatisValueS);
        //required uint32 unType = 1;     // 업적 대분류 成就大类型
        //required uint32 unSubType = 2;  // 업적 소분류 成就小类型
        //required uint64 unValue = 3;	// 누적치 累积值
    }

    /// <summary> 업적   </summary>
    private void PMsgAchieveSynAchieveCompleteSHandler(PMsgAchieveSynAchieveCompleteS pmsgAchieveSynAchieveCompleteS)
    {
        SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, true);

        //현재 업적UI켜져있으면 return
        UIBasePanel achive = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achive != null && achive.isActiveAndEnabled)
        {
            return;
        }

        Debug.Log(pmsgAchieveSynAchieveCompleteS);
        //업적 실시간으로 완료됬을때 호출됨
        string data = string.Format("{0},{1},{2},{3}", 
            pmsgAchieveSynAchieveCompleteS.UnType,
            pmsgAchieveSynAchieveCompleteS.UnSubType,
            pmsgAchieveSynAchieveCompleteS.UnLevel,
            1);

        
        SceneManager.instance.SetNoticePanel(NoticeType.Achiev, 0, data);
        //else
        //{
        //    TempCoroutine.instance.FrameDelay(5f, ()=>{
        //        SceneManager.instance.SetNoticePanel(NoticeType.Achiev, 0, data);
        //    });
        //}

        //마을제외하고 인게임이나 난투장이라면 미리 예약

    }

    private void PMsgAchieveSynFightDataTotalValueSHandler(PMsgAchieveSynFightDataTotalValueS pmsgAchieveSynFightDataTotalValueS)
    {
        uint ErrorCode = (uint)pmsgAchieveSynFightDataTotalValueS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveSynFightDataTotalValueS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

    }

    /// <summary> 콜로세움 던전 정보 응답 </summary>
    private void PMsgColosseumQuerySHandler(PMsgColosseumQueryS pmsgColosseumQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgColosseumQueryC");
        int dailyComplete = pmsgColosseumQueryS.UnDailyComplete;//당일 클리어 횟수
        int dailyMaxComplete = pmsgColosseumQueryS.UnMaxDailyComplete;//당일 클리어 횟수 상한

        int lastClearId = 0;
        if (0 < pmsgColosseumQueryS.CInfo.Count)
            lastClearId = pmsgColosseumQueryS.CInfo[pmsgColosseumQueryS.CInfo.Count - 1].UnBattleId;

        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.ColosseumCount, dailyComplete, dailyMaxComplete);
        NetData.instance.GetUserInfo()._ColosseumData = new NetData.ColosseumData((uint)lastClearId);

        //UIMgr.OpenColosseumPanel((uint)lastClearId, dailyComplete, dailyMaxComplete);
        //UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        //if (townPanel != null)
        //    townPanel.Hide();
    }

    /// <summary> 콜로세움 방 생성 응답 </summary>
    private void PMsgColosseumCreateRoomSHandler(PMsgColosseumCreateRoomS pmsgColosseumCreateRoomS)
    {
        uint ErrorCode = (uint)pmsgColosseumCreateRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if( (int)Sw.ErrorCode.ER_ColosseumCreateRoomS_Attack_Error == ErrorCode)//공격력 부족
            {
                DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo((uint)pmsgColosseumCreateRoomS.UnColosseumId);
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(729), coloInfo.FightingPower);
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
            }
            else
                UIMgr.instance.AddErrorPopup((int)ErrorCode); 

            Debug.Log(pmsgColosseumCreateRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint dungeonId = (uint)pmsgColosseumCreateRoomS.UnColosseumId;
        long roomId = pmsgColosseumCreateRoomS.UllRoomId;

        NetData.RoomData roomData = new NetData.RoomData();
        roomData.RoomId = roomId;
        roomData.DungeonId = dungeonId;
        roomData.IsLeader = true;
        roomData.Owner = null;
        roomData.UserList = new List<NetData.RoomUserInfo>();

        NetData.instance.GameRoomData = roomData;

        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/ColosseumPanel");
        if (panel != null)
        {
            (panel as ColosseumPanel).OnCreateRoom(dungeonId, roomId);
        }
    }

    /// <summary> 콜로세움 초대 응답 </summary>
    private void PMsgColosseumInviteSHandler(PMsgColosseumInviteS pmsgColosseumInviteS)
    {
        uint ErrorCode = (uint)pmsgColosseumInviteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumInviteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //List <long> roleList = new List<long>();
        //int inviteType = pmsgColosseumInviteS.UnGlobalFlag;//0 친구or길드, 1 마을
        //for(int i=0; i < pmsgColosseumInviteS.UllRoleIdCount; i++ )
        //{
        //    roleList.Add(pmsgColosseumInviteS.UllRoleIdList[i] );
        //}
        
    }

    /// <summary> 콜로세움 초대 받은거 응답 </summary>
    private void PMsgColosseumInviteRecvSHandler(PMsgColosseumInviteRecvS pmsgColosseumInviteRecvS)
    {
        if( !TownState.TownActive)//마을 아니면 무시
            return;

        //파티초대거부인지 체크
        char[] inviteArr = SceneManager.instance.optionData.BlockInvite.ToCharArray();
        if (inviteArr[2] == '2')
            return;


        DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo((uint)pmsgColosseumInviteRecvS.UnColosseumId);
        //입장 조건 검사
        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        if (userInfo._Level < coloInfo.LimitLevel)//레벨 제한 걸림
        {
            return;
        }
        else if (userInfo._TotalAttack < coloInfo.FightingPower)//권장 전투력 미달
        {
            return;
        }
        else if (userInfo._ColosseumData.StageId < coloInfo.StageId)//0이라는 것은 한번도 실행을 안한것 렙제만 벗어나면 가능한것
        {
            return;
        }
        else if (0 < NetData.instance.GameRoomData.RoomId)//이미 방에 들어가 있음
        {
            return;
        }
        else if (UIMgr.instance.IsActiveTutorial)//튜토리얼중
        {
            return;
        }
        else// if (userInfo._ColosseumData.MaxCompleteCount <= userInfo._ColosseumData.CompleteCount)//금일 입장 못함
        {
            int now = 0, max = 0;
            NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ColosseumCount, ref now, ref max);
            if (max <= now)
                return;
        }

        //{0} 님이 {1}{2} [{3}][-]에 초대 하셨습니다.
        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(730)
            , pmsgColosseumInviteRecvS.SzName, "[55FF55]", _LowDataMgr.instance.GetStringCommon(720), _LowDataMgr.instance.GetStringStageData(coloInfo.String));

        if (pmsgColosseumInviteRecvS.UnGlobalFlag == 1)//0 친구or길드, 1 마을
        {
            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup", false);
            if (chat == null)
                return;

            msg = string.Format("{0} [url=colosseum,{1}]{2}[/url]", msg, pmsgColosseumInviteRecvS.UllRoomId, _LowDataMgr.instance.GetStringCommon(747));//[파티 참여] URL삽입
            (chat as ChatPopup).OnReciveChat(msg, ChatType.World);
        }
        else
        {
            if (!TownState.TownActive || UIMgr.GetUIBasePanel("UIPopup/ReadyPopup") != null)//초대를 받을 수 없는 상태이다
                return;

            msg = string.Format("{0} {1}", msg, _LowDataMgr.instance.GetStringCommon(747));//[파티 참여]
            UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(727), msg, _LowDataMgr.instance.GetStringCommon(158), _LowDataMgr.instance.GetStringCommon(272), null
                , () => {//ok
                    SendPMsgColosseumEnterRoomC(pmsgColosseumInviteRecvS.UllRoomId);
                }
                , () => {//cancel

                }
            );
        }
    }

    /// <summary> 콜로세움 방조회 정보 응답 </summary>
    private void PMsgColosseumRoomInfoSHandler(PMsgColosseumRoomInfoS pmsgColosseumRoomInfoS)
    {
        NetData.RoomData roomData = new NetData.RoomData();
        if (pmsgColosseumRoomInfoS.CInfo.Count <= 0)//뭔가 잘못되었다.
            return;

        ColosseumRoomInfo roomInfo = pmsgColosseumRoomInfoS.CInfo[0];
        long roomId = roomInfo.UllRoomId;
        int coloId = roomInfo.UnColosseumId;
        long ownerId = roomInfo.UllOwnerId;
        int maxRoomNum = roomInfo.UnMaxRole;

        if (maxRoomNum < roomInfo.CInfo.Count)//방이 꽉 찻다
        {
            UIMgr.instance.AddPopup(141, 124, 117);//-101
            return;
        }

        roomData.RoomId = roomId;
        roomData.DungeonId = (uint)coloId;
        roomData.IsLeader = false;
        roomData.UserList = new List<NetData.RoomUserInfo>();
        for (int j = 0; j < roomInfo.CInfo.Count; j++)
        {
            ColosseumRoomRoleInfo userInfo = roomInfo.CInfo[j];
            int charType = userInfo.UnType;
            int lv = userInfo.Unlevel;
            int battlePower = userInfo.UnAttack;
            long charRoleId = userInfo.UllRoleId;
            string name = userInfo.SzName;

            if (ownerId == charRoleId)
            {
                roomData.Owner = new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, 0);
            }
            else
            {
                int slot = 0;
                if (roomInfo.CInfo.Count == 3)//내가 제일 늦게 들어옴
                {
                    if((ulong)charRoleId == NetData.instance.GetUserInfo().GetCharUUID() )
                        slot = 1;
                }
                
                roomData.UserList.Add(new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, slot));
            }
        }

        NetData.instance.GameRoomData = roomData;
        uint dungeonId = roomData.DungeonId;//현재 초대받은 던전의 아이디
        UIBasePanel basePanel = UIMgr.instance.GetCurPanel();
        if (basePanel is ColosseumPanel)//현재 콜로세움이었다면
        {
            (basePanel as ColosseumPanel).OnReadyPopup(dungeonId);
        }
        else//그렇지 않았다면
        {
            if (basePanel is TownPanel)
                basePanel.Hide();
            else
            {
                //if (basePanel.ReturnType == PrevReturnType.Hide)
                //    basePanel.Hide();
                //else
                //{
                //    basePanel.Close();
                //    if (basePanel != null)
                //        basePanel.Close();
                //    if (basePanel != null)
                //        basePanel.Close();
                //}
                UIMgr.instance.OnlyDefaultUI();
                UIMgr.GetTownBasePanel().Hide();
            }

            UIMgr.OpenColosseumPanel(dungeonId);
        }

    }

    /// <summary> 콜로세움 방 진입 응답 </summary>
    private void PMsgColosseumEnterRoomSHandler(PMsgColosseumEnterRoomS pmsgColosseumEnterRoomS)
    {
        uint ErrorCode = (uint)pmsgColosseumEnterRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumEnterRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }

    /// <summary> 콜로세움 브로드케스팅 방 진입 응답 </summary>
    private void PMsgColosseumEnterRoomRecvSHandler(PMsgColosseumEnterRoomRecvS pmsgColosseumEnterRoomRecvS)
    {
        if (pmsgColosseumEnterRoomRecvS.CInfo.Count <= 0)
            return;

        NetData.RoomUserInfo inUserInfo;
        //for (int j = 0; j < pmsgColosseumEnterRoomRecvS.CInfoCount; j++)
        {
            ColosseumRoomRoleInfo userInfo = pmsgColosseumEnterRoomRecvS.CInfo[0];
            int charType = userInfo.UnType;
            int lv = userInfo.Unlevel;
            int battlePower = userInfo.UnAttack;
            long charRoleId = userInfo.UllRoleId;
            string name = userInfo.SzName;
            
            inUserInfo = new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, NetData.instance.GameRoomData.UserList.Count);
            NetData.instance.AddGameRoomUser(inUserInfo);
        }

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            (readyPop as ReadyPopup).OnInUser(inUserInfo);
        }
    }

    /// <summary> 콜로세움 방 이탈 응답 </summary>
    private void PMsgColosseumLeaveRoomSHandler(PMsgColosseumLeaveRoomS pmsgColosseumLeaveRoomS)
    {
        uint ErrorCode = (uint)pmsgColosseumLeaveRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumLeaveRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        } 

        NetData.instance.ClearGameRoomData();
        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            (readyPop as ReadyPopup).OnNetworkClose();
        }
    }

    /// <summary> 콜로세움 방 이탈 브로드케이트 응답 </summary>
    private void PMsgColosseumLeaveRoomRecvSHandler(PMsgColosseumLeaveRoomRecvS pmsgColosseumLeaveRoomRecvS)
    {
        ulong roleId = (ulong)pmsgColosseumLeaveRoomRecvS.UllRoleId;
        int arr = NetData.instance.GetGameRoomUserArr(roleId);
        NetData.instance.GameRoomRemoveUser(roleId);

        //if (NetData.instance.GameRoomData.Owner == null)//무시
        //    return;

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            if ( !NetData.instance.GameRoomData.IsLeader && roleId == NetData.instance.GameRoomData.Owner.Id)//방 폭파. 방장이 나갔음.
            {
                SendPMsgColosseumLeaveRoomC(pmsgColosseumLeaveRoomRecvS.UllRoomId);
            }
            else if (readyPop != null)
            {
                (readyPop as ReadyPopup).OnOutUser(arr);
            }
        }

    }

    /// <summary> 콜로세움 유저 추방 응답 </summary>
    private void PMsgColosseumKickRoomSHandler(PMsgColosseumKickRoomS pmsgColosseumKickRoomS)
    {
        /*//어차피 브로드캐스팅으로 방장도 받는다 일단 무시.
        ulong roleId = (ulong)pmsgColosseumKickRoomS.UllRoleId;
        int arr = NetData.instance.GetGameRoomUserArr(roleId);
        NetData.instance.GameRoomRemoveUser(roleId);

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            if(0 <= arr)
                (readyPop as ReadyPopup).OnOutUser(arr);
        }
        */
    }

    /// <summary> 콜로세움 유저 추방 브로드캐스팅 응답 </summary>
    private void PMsgColosseumKickRoomRecvSHandler(PMsgColosseumKickRoomRecvS pmsgColosseumKickRoomRecvS)
    {
        ulong roleId = (ulong)pmsgColosseumKickRoomRecvS.UllRoleId;

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop == null)
        {
            NetData.instance.ClearGameRoomData();
            return;
        }

        if (roleId == NetData.instance.GetUserInfo().GetCharUUID())//나임
        {
            NetData.instance.ClearGameRoomData();
            (readyPop as ReadyPopup).OnNetworkClose();
        }
        else
        {
            int arr = NetData.instance.GetGameRoomUserArr(roleId);
            if (0 <= arr)
                (readyPop as ReadyPopup).OnOutUser(arr);

            NetData.instance.GameRoomRemoveUser(roleId);
        }

    }

    /// <summary> 콜로세움 게임 시작 </summary>
    private void PMsgColosseumStartSHandler(PMsgColosseumStartS pmsgColosseumStartS)
    {
        uint ErrorCode = (uint)pmsgColosseumStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData.instance.ClearRewardData();
        NetData.instance._RewardData.GetCoin = pmsgColosseumStartS.UnDropCoin;
        NetData.instance._RewardData.GetExp = (uint)pmsgColosseumStartS.UnDropExp;

        NetData.instance._RewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref NetData.instance._RewardData.SaveExp
            , ref NetData.instance._RewardData.SaveMaxExp);

        long roomId = pmsgColosseumStartS.UllRoomId;
        uint dungeonId = (uint)pmsgColosseumStartS.UnColosseumId;

        List<NetData.DropItemData> list = new List<NetData.DropItemData>();
        for (int i = 0; i < pmsgColosseumStartS.CDrop.Count; i++)
        {
            ColosseumDropItem dropInfo = pmsgColosseumStartS.CDrop[i];
            list.Add(new NetData.DropItemData((uint)dropInfo.UnItemId, (uint)dropInfo.UnItemNum, dropInfo.UnType));
        }

        NetData.instance._RewardData.GetList = list;

        UIBasePanel coloPanel = UIMgr.GetUIBasePanel("UIPanel/ColosseumPanel");
        if (coloPanel != null)
            (coloPanel as ColosseumPanel).OnStartGame(dungeonId, roomId);
    }

    /// <summary> 콜로세움 게임 종료 </summary>
    private void PMsgColosseumCompleteSHandler(PMsgColosseumCompleteS pmsgColosseumCompleteS)
    {
        uint ErrorCode = (uint)pmsgColosseumCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (TownState.TownActive)
            return;

        NetData.instance.ClearGameRoomData();
        bool isVictory = pmsgColosseumCompleteS.UnVictory == 0 ? false : true;
        if (!isVictory)
        {
            NetData.instance.ClearRewardData();
        }

        G_GameInfo.GameInfo.EndGame(isVictory);
    }

    /// <summary> 콜로세움 게임 소탕 </summary>
    private void PMsgColosseumSweepSHandler(PMsgColosseumSweepS pmsgColosseumSweepS)
    {
        uint ErrorCode = (uint)pmsgColosseumSweepS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgColosseumSweepS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/ColosseumPanel");
        if(basePanel != null)
        {
            (basePanel as ColosseumPanel).OnSweep();
        }
    }

    /// <summary> 콜로세움 게임 소탕 결과 </summary>
    private void PMsgColosseumSweepResultS(PMsgColosseumSweepResultS pmsgColosseumSweepResultS)
    {
        Debug.Log("PMsgColosseumSweepResultS : " + pmsgColosseumSweepResultS);
        uint dungeonId = (uint)pmsgColosseumSweepResultS.UnColosseumId;
        int clearCount = pmsgColosseumSweepResultS.UnTimes;
        int dropCoin = pmsgColosseumSweepResultS.UnDropCoin;
        int dropExp = pmsgColosseumSweepResultS.UnDropExp;

        List<NetData.EmailAttachmentInfo> getMailList = new List<NetData.EmailAttachmentInfo>();
        for (int i = 0; i < pmsgColosseumSweepResultS.CDrop.Count; i++)
        {
            ColosseumDropItem dropItem = pmsgColosseumSweepResultS.CDrop[i];
            getMailList.Add(new NetData.EmailAttachmentInfo((uint)dropItem.UnType, (uint)dropItem.UnItemId/*(uint)(dropItem.UnItemId == 0 ? dropItem.UnType : dropItem.UnItemId)*/, (ushort)dropItem.UnItemNum) );
        }

        if (0 < dropCoin)
            getMailList.Add(new NetData.EmailAttachmentInfo((int)AssetType.Gold, 0, (uint)dropCoin));

        if (0 < dropExp)
            getMailList.Add(new NetData.EmailAttachmentInfo((int)AssetType.Exp, 0, (uint)dropExp));

        if (0 < getMailList.Count)
        {
            SceneManager.instance.SetNoticePanel(NoticeType.GetMailItem, 0, null, getMailList);
        }
    }

    /// <summary> 7일연속로그인 보상 정보조회 </summary>
    private void PMsgWelfareXDayLoginQueryInfoSHandler(PMsgWelfareXDayLoginQueryInfoS pmsgWelfareXDayLoginQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("WeekWelfareInfo");


        uint ErrorCode = (uint)pmsgWelfareXDayLoginQueryInfoS.UnErrorCode;
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == (int)Sw.ErrorCode.ER_WelfareXDayLoginReward_Welfare_End || ErrorCode == (int)Sw.ErrorCode.ER_WelfareXDayLoginReward_Wccumulate_Days_Is_Full)
            {
                if (benefitPanel != null)
                {
                    (benefitPanel as BenefitPanel).EndPeriodTime((int)ErrorCode);
                    Debug.Log(pmsgWelfareXDayLoginQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
                    UIMgr.instance.AddErrorPopup((int)ErrorCode);

                    return;
                }
            }
            //Debug.Log(pmsgWelfareXDayLoginQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).Show7Days(pmsgWelfareXDayLoginQueryInfoS.UnFetchInfo, pmsgWelfareXDayLoginQueryInfoS.UnDay);
        }

    }
    /// <summary> 7일연속로그인 보상획득 </summary>
    private void PMsgWelfareFetchXDayRewardSHandler(PMsgWelfareFetchXDayRewardS pmsgWelfareFetchXDayRewardS)
    {
        SceneManager.instance.EndNetProcess("GetWeekWelfare");

        uint ErrorCode = (uint)pmsgWelfareFetchXDayRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareFetchXDayRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

     // Debug.Log(pmsgWelfareFetchXDayRewardS);

        NetworkClient.instance.SendPMsgWelfareXDayLoginQueryInfoC();

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        //if (benefitPanel != null)
        //{
        //    (benefitPanel as BenefitPanel).Show7Days(pmsgWelfareFetchXDayRewardS.UnFetchInfo);
        //}

    }
    /// <summary> 복귀패키지 정보조회 </summary>
    private void PMsgWelfareReturnFetchRewardCHandler(PMsgWelfareReturnQueryInfoS pmsgWelfareReturnQueryInfoS)
    {
        uint ErrorCode = (uint)pmsgWelfareReturnQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            // UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareReturnQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

    //    Debug.Log(pmsgWelfareReturnQueryInfoS);

        System.DateTime today = System.DateTime.Now;
        string day = "";
        day += today.Year.ToString().Substring(2, 2);
        day += string.Format("{0:00}", today.Month);
        day += today.Day;


        for (int i = 0; i < pmsgWelfareReturnQueryInfoS.UnInfoCount; i++)
        {
            FetchTimeInfo info = pmsgWelfareReturnQueryInfoS.CFetchTimeInfo[i];
            // 오늘날짜에 보상있으면 받자
            if (uint.Parse(day) == info.UnYYMMDD)
            {
                NetworkClient.instance.SendPMsgWelfareReturnFetchRewardC(info.UnDay);
            }

        }



    }
    /// <summary> 복귀패키지 보상획득 </summary>
    private void PMsgWelfareReturnFetchRewardSHandler(PMsgWelfareReturnFetchRewardS pmsgWelfareReturnFetchRewardS)
    {
        uint ErrorCode = (uint)pmsgWelfareReturnFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            // UIMgr.instance.AddPopup((int)Sw.ErrorCode, null, null, null);
            Debug.Log(pmsgWelfareReturnFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }




        // 복귀팝업
    //    UIMgr.instance.ComebackDay = (int)pmsgWelfareReturnFetchRewardS.UnFetchInfo;
            //UIMgr.OpenCombackPopup(1);

    }
    /// <summary> 신썹오픈 정보조회 </summary>
    private void PMsgWelfareOpenSvrQueryInfoSHandler(PMsgWelfareOpenSvrQueryInfoS pmsgWelfareOpenSvrQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("newSvrWelfareInfo");

        uint ErrorCode = (uint)pmsgWelfareOpenSvrQueryInfoS.UnErrorCode;
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (benefitPanel != null)
            {
                (benefitPanel as BenefitPanel).EndPeriodTime((int)ErrorCode);
                return;
            }
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareOpenSvrQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

    //    Debug.Log(pmsgWelfareOpenSvrQueryInfoS);


        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowNewServer(pmsgWelfareOpenSvrQueryInfoS.UnFetchInfo, pmsgWelfareOpenSvrQueryInfoS.UnDay);

        }


    }
    /// <summary> 신썹오픈 보상획득 </summary>
    private void PMsgWelfareOpenSvrFetchRewardSHandler(PMsgWelfareOpenSvrFetchRewardS pmsgWelfareOpenSvrFetchRewardS)
    {
        SceneManager.instance.EndNetProcess("GetNewSvrWelfare");

        uint ErrorCode = (uint)pmsgWelfareOpenSvrFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareOpenSvrFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetworkClient.instance.SendPMsgWelfareOpenSvrQueryInfoC();


        //Debug.Log(pmsgWelfareOpenSvrFetchRewardS);
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        //if (benefitPanel != null)
        //{
        //    (benefitPanel as BenefitPanel)._ShowNewServer(pmsgWelfareOpenSvrFetchRewardS.UnFetchInfo);
        //}
    }
    /// <summary> 로그인유지보상 정보조회 </summary>
    private void PMsgWelfareOnlineQueryInfoSHandler(PMsgWelfareOnlineQueryInfoS pmsgWelfareOnlineQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("AccessWelfareInfo");

        uint ErrorCode = (uint)pmsgWelfareOnlineQueryInfoS.UnErrorCode;
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");

        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (ErrorCode == (int)Sw.ErrorCode.ER_WelfareOnline_FetchLevel_Is_Max)
            {
                //이때는 모든보상수령완료상태로 체크해줌
                if (benefitPanel != null)
                {
                    (benefitPanel as BenefitPanel).ShowAccessTime((uint)_LowDataMgr.instance.GetLowDataWalfare(1).Count + 1, 0);
                }
                return;
            }

            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareOnlineQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgWelfareOnlineQueryInfoS);

        uint fetchLv = pmsgWelfareOnlineQueryInfoS.UnOnlineRewardFetchLevel;    //로그인유지보상상태 마지막수령보상단계
        uint countDown = pmsgWelfareOnlineQueryInfoS.UnCountdown;   //다음보상까지 카운트다운(초)

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowAccessTime(fetchLv, countDown);
        }


    }
    /// <summary>  로그인유지 보상획득 </summary>
    private void PMsgWelfareOnlineFetchRewardSHandler(PMsgWelfareOnlineFetchRewardS pmsgWelfareOnlineFetchRewardS)
    {
        SceneManager.instance.EndNetProcess("GetAccessWelfare");

        uint ErrorCode = (uint)pmsgWelfareOnlineFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareOnlineFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //  Debug.Log(pmsgWelfareOnlineFetchRewardS);

        SceneManager.instance.SetAlram(AlramIconType.BENEFIT, false);
        SceneManager.instance.SetBenefitAlram(1, false);

        //전부받앗으면 0으로 오기때문에 예외처리를해줘야함
        uint fetchLv = pmsgWelfareOnlineFetchRewardS.UnFetchLevel == 0 ? (uint)_LowDataMgr.instance.GetLowDataWalfare(1).Count + 1 : pmsgWelfareOnlineFetchRewardS.UnFetchLevel;

        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowAccessTime(fetchLv, pmsgWelfareOnlineFetchRewardS.UnCountdown);
        }
    }
    /// <summary>  클라에 획득가능한 보상여부 존재 여부 알림 (몇분치)</summary>
    private void PMsgWelfareOnlineSynCanFetchSHandler(PMsgWelfareOnlineSynCanFetchS pmsgWelfareOnlineSynCanFetchS)
    {
        SceneManager.instance.SetAlram(AlramIconType.BENEFIT, true);
        if(pmsgWelfareOnlineSynCanFetchS.UnFetchLevel <= _LowDataMgr.instance.GetLowDataWalfare(1).Count)
        {
            SceneManager.instance.SetAlram(AlramIconType.BENEFIT, true);
            SceneManager.instance.SetBenefitAlram(1, true);
        }
    }

    /// <summary> 업적 일괄수령  </summary>
    private void PMsgAchieveOneKeyFetchAchieveAwardSHandler(PMsgAchieveOneKeyFetchAchieveAwardS pmsgAchieveOneKeyFetchAchieveAwardS)
    {
        SceneManager.instance.EndNetProcess("FetchAllAchieve");

        uint ErrorCode = (uint)pmsgAchieveOneKeyFetchAchieveAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveOneKeyFetchAchieveAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

   //   Debug.Log(pmsgAchieveOneKeyFetchAchieveAwardS);

        //if(pmsgAchieveOneKeyFetchAchieveAwardS.CReachInfoCount > 0)
        //{

        //    UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        //    if (achievePanel != null)
        //    {
        //        (achievePanel as AchievePanel).AfterGetAllAchieve(pmsgAchieveOneKeyFetchAchieveAwardS.CReachInfoList[0].UnType);
        //    }
        //}

        if (pmsgAchieveOneKeyFetchAchieveAwardS.CReachInfo.Count == 0)
            return;

        List<NetData.AchieveLevelInfo> infoList = new List<NetData.AchieveLevelInfo>();
        for (int i = 0; i < pmsgAchieveOneKeyFetchAchieveAwardS.CReachInfo.Count; i++)
        {
            AchieveReachInfo Info = pmsgAchieveOneKeyFetchAchieveAwardS.CReachInfo[i];
            NetData.AchieveLevelInfo info = new NetData.AchieveLevelInfo(Info.UnType, Info.UnSubType, Info.UnLevel, Info.UnComplete,Info.UnFetch);
            infoList.Add(info);
        }

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).AfterGetAllAchieve(infoList);
        }
    }
    /*
     * S>(PMsgWelfareRoleUpgradeQueryInfoSHandler);
     *     rdS>(PMsgWelfareFetchRoleUpgradeRewardSSHandler);

     */



    /// <summary> 랩업패키지 정보조회 </summary>
    private void PMsgWelfareRoleUpgradeQueryInfoSHandler(PMsgWelfareRoleUpgradeQueryInfoS pmsgWelfareRoleUpgradeQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("LvWelfareInfo");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");

        uint ErrorCode = (uint)pmsgWelfareRoleUpgradeQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (benefitPanel != null)
            {
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
                Debug.Log(pmsgWelfareRoleUpgradeQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            }
            return;
        }
        uint info = pmsgWelfareRoleUpgradeQueryInfoS.UnFetchInfo;
        List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(2); //레벨업보상 


        string dayCnt = System.Convert.ToString(info, 2);
        int check = 0;
        for (int i = 0; i < dayCnt.Length; i++)
        {
            if (dayCnt.Substring(i, 1) == "1")
                check++;
        }

        if (check < list.Count && list[check].RewardCondition <= NetData.instance.GetUserInfo()._Level)
            SceneManager.instance.SetBenefitAlram(4, true);
        else
            SceneManager.instance.SetBenefitAlram(4, false);





        // Debug.Log(pmsgWelfareRoleUpgradeQueryInfoS);

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowLevelUp(pmsgWelfareRoleUpgradeQueryInfoS.UnFetchInfo);
        }
    }
    /// <summary> 랩업패키지보상 </summary>
    private void PMsgWelfareFetchRoleUpgradeRewardSHandler(PMsgWelfareFetchRoleUpgradeRewardS pmsgWelfareFetchRoleUpgradeRewardS)
    {
        SceneManager.instance.EndNetProcess("GetLvWelfare");

        uint ErrorCode = (uint)pmsgWelfareFetchRoleUpgradeRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgWelfareFetchRoleUpgradeRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint info = pmsgWelfareFetchRoleUpgradeRewardS.UnFetchInfo;
        List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(2); //레벨업보상 



        string dayCnt = System.Convert.ToString(info, 2);
        int check = 0;
        for (int i = 0; i < dayCnt.Length; i++)
        {
            if (dayCnt.Substring(i, 1) == "1")
                check++;
        }

        if (check < list.Count &&  list[(int)check].RewardCondition <= NetData.instance.GetUserInfo()._Level)
            SceneManager.instance.SetBenefitAlram(4, true);
        else
            SceneManager.instance.SetBenefitAlram(4, false);

        //    Debug.Log(pmsgWelfareFetchRoleUpgradeRewardS);
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowLevelUp(pmsgWelfareFetchRoleUpgradeRewardS.UnFetchInfo);
        }
    }


    /// <summary> 멀티보스 레이드 정보 조회 </summary>
    private void PMsgMultiBossQuerySHandler(PMsgMultiBossQueryS pmsgMultiBossQueryS)
    {
        SceneManager.instance.EndNetProcess("PMsgMultiBossQueryC");
        int dailyComplete = pmsgMultiBossQueryS.UnDailyComplete;
        int maxComplete = pmsgMultiBossQueryS.UnMaxDailyComplete;

        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.MultyBossRaid1Count, dailyComplete, maxComplete);
    }

    /// <summary> 멀티보스 레이드 방생성 </summary>
    private void PMsgMultiBossCreateRoomSHandler(PMsgMultiBossCreateRoomS pmsgMultiBossCreateRoomS)
    {
        uint ErrorCode = (uint)pmsgMultiBossCreateRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossCreateRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        uint dungeonId = (uint)pmsgMultiBossCreateRoomS.UnBossBattleId;
        long roomId = pmsgMultiBossCreateRoomS.UllRoomId;

        NetData.RoomData roomData = new NetData.RoomData();
        roomData.RoomId = roomId;
        roomData.DungeonId = dungeonId;
        roomData.IsLeader = true;
        roomData.Owner = null;
        roomData.UserList = new List<NetData.RoomUserInfo>();

        NetData.instance.GameRoomData = roomData;

        //UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/SpecialPanel");
        //if (panel != null)
        //{
        //    (panel as SpecialPanel).OnCreateMultyRaid(dungeonId, roomId);
        //}
        UIBasePanel panel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (panel != null)
        {
            (panel as DungeonPanel).OnMultyRaid(dungeonId, false);
        }
    }

    /// <summary> 멀티보스레이드 초대 응답 </summary>
    private void PMsgMultiBossInviteSHandler(PMsgMultiBossInviteS pmsgMultiBossInviteS)
    {
        uint ErrorCode = (uint)pmsgMultiBossInviteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossInviteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //List <long> roleList = new List<long>();
        //int inviteType = pmsgColosseumInviteS.UnGlobalFlag;//0 친구or길드, 1 마을
        //for(int i=0; i < pmsgColosseumInviteS.UllRoleIdCount; i++ )
        //{
        //    roleList.Add(pmsgColosseumInviteS.UllRoleIdList[i] );
        //}
        
    }

    /// <summary> 멀티보스레이드 초대 받은거 응답 </summary>
    private void PMsgMultiBossInviteRecvSHandler(PMsgMultiBossInviteRecvS pmsgMultiBossInviteRecvS)
    {
        if (!TownState.TownActive)//마을 아니면 무시
            return;

        //파티초대거부인지 체크
        char[] inviteArr = SceneManager.instance.optionData.BlockInvite.ToCharArray();
        if (inviteArr[2] == '2')
            return;

        DungeonTable.MultyBossRaidInfo info = _LowDataMgr.instance.GetLowDataMultyBossInfo((uint)pmsgMultiBossInviteRecvS.UnBossBattleId);
        //입장 조건 검사
        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        if (userInfo._Level < info.EnterLevel)//레벨 제한 걸림
        {
            return;
        }
        else if (userInfo._TotalAttack < info.FightingPower)//권장 전투력 미달
        {
            return;
        }
        else if (0 < NetData.instance.GameRoomData.RoomId)//이미 방에 들어가 있음
        {
            return;
        }
        else if (UIMgr.instance.IsActiveTutorial)//튜토리얼중
        {
            return;
        }
        else //if (userInfo.MultyRaidDailyMaxCount <= userInfo.MultyRaidDailyCount)//금일 입장 못함
        {
            int now = 0, max = 0;
            userInfo.GetCompleteCount(EtcID.MultyBossRaid1Count, ref now, ref max);
            if (max <= now)
                return;
        }

        //{0} 님이 {1} [{2}]에 초대 하셨습니다.
        uint key = 0;
        string colorStr = null;
        if (info.level == 1)
        {
            key = 167;
            colorStr = "[1EC91E]";
        }
        else if (info.level == 2)
        {
            key = 168;
            colorStr = "[FFAA00]";
        }
        else if (info.level == 3)
        {
            key = 169;
            colorStr = "[D73333]";
        }

        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(730)
            , pmsgMultiBossInviteRecvS.SzName, colorStr, _LowDataMgr.instance.GetStringStageData(info.stageString), _LowDataMgr.instance.GetStringCommon(key));

        if (pmsgMultiBossInviteRecvS.UnGlobalFlag == 1)//0 친구or길드, 1 마을
        {
            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup", false);
            if (chat == null)
                return;

            msg = string.Format("{0} [url=multyRaid,{1}]{2}[/url]", msg, pmsgMultiBossInviteRecvS.UllRoomId, _LowDataMgr.instance.GetStringCommon(747));//[파티 참여] URL삽입
            (chat as ChatPopup).OnReciveChat(msg, ChatType.World);
        }
        else
        {
            if (!TownState.TownActive || UIMgr.GetUIBasePanel("UIPopup/ReadyPopup") != null)//초대를 받을 수 없는 상태이다
                return;

            msg = string.Format("{0} {1}", msg, _LowDataMgr.instance.GetStringCommon(747));//[파티 참여]
            UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(727), msg, _LowDataMgr.instance.GetStringCommon(158), _LowDataMgr.instance.GetStringCommon(272), null
                , () => {//ok
                    SendPMsgMultiBossEnterRoomC(pmsgMultiBossInviteRecvS.UllRoomId);
                }
                , () => {//cancel

                }
            );
        }
    }

    /// <summary> 멀티보스레이드 방조회 정보 응답 </summary>
    private void PMsgMultiBossRoomInfoSHandler(PMsgMultiBossRoomInfoS pmsgMultiBossRoomInfoS)
    {
        NetData.RoomData roomData = new NetData.RoomData();
        if (pmsgMultiBossRoomInfoS.CInfo.Count <= 0)//뭔가 잘못되었다.
            return;

        MultiBossRoomInfo roomInfo = pmsgMultiBossRoomInfoS.CInfo[0];
        long roomId = roomInfo.UllRoomId;
        int coloId = roomInfo.UnBossBattleId;
        long ownerId = roomInfo.UllOwnerId;
        int maxRoomNum = roomInfo.UnMaxRole;

        Debug.Log(pmsgMultiBossRoomInfoS);
        if (maxRoomNum < roomInfo.CInfo.Count)//방이 꽉 찻다
        {
            UIMgr.instance.AddPopup(141, 124, 117);//-101
            return;
        }

        roomData.RoomId = roomId;
        roomData.DungeonId = (uint)coloId;
        roomData.IsLeader = false;

        roomData.UserList = new List<NetData.RoomUserInfo>();
        for (int j = 0; j < roomInfo.CInfo.Count; j++)
        {
            MultiBossRoomRoleInfo userInfo = roomInfo.CInfo[j];
            int charType = userInfo.UnType;
            int lv = userInfo.Unlevel;
            int battlePower = userInfo.UnAttack;
            long charRoleId = userInfo.UllRoleId;
            string name = userInfo.SzName;

            if (ownerId == charRoleId)//방장
            {
                roomData.Owner = new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, 0);
            }
            else//파티원
            {
                int slot = 0;
                if (roomInfo.CInfo.Count == 3)//내가 제일 늦게 들어옴
                {
                    if ((ulong)charRoleId == NetData.instance.GetUserInfo().GetCharUUID())
                        slot = 1;
                }

                roomData.UserList.Add(new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, slot));
            }   
        }

        NetData.instance.GameRoomData = roomData;
        uint dungeonId = roomData.DungeonId;//현재 초대받은 던전의 아이디
        UIBasePanel basePanel = UIMgr.instance.GetCurPanel();
        if (basePanel is DungeonPanel)//현재 콜로세움이었다면
        {
            (basePanel as DungeonPanel).OnMultyRaid(dungeonId, true);
        }
        else//그렇지 않았다면
        {
            if (basePanel is TownPanel)
                basePanel.Hide();
            else
            {
                UIMgr.instance.OnlyDefaultUI();
                UIMgr.GetTownBasePanel().Hide();
            }

            UIMgr.OpenDungeonPanel(true, dungeonId);
            //UIMgr.OpenSpecial(true, dungeonId);
        }

    }

    /// <summary> 멀티보스레이드 방 진입 응답 </summary>
    private void PMsgMultiBossEnterRoomSHandler(PMsgMultiBossEnterRoomS pmsgMultiBossEnterRoomS)
    {
        uint ErrorCode = (uint)pmsgMultiBossEnterRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossEnterRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
    }

    /// <summary> 멀티보스레이드 브로드케스팅 방 진입 응답 </summary>
    private void PMsgMultiBossEnterRoomRecvSHandler(PMsgMultiBossEnterRoomRecvS pmsgMultiBossEnterRoomRecvS)
    {
        if (pmsgMultiBossEnterRoomRecvS.CInfo.Count <= 0)
            return;

        NetData.RoomUserInfo inUserInfo;
        //for (int j = 0; j < pmsgColosseumEnterRoomRecvS.CInfoCount; j++)
        {
            MultiBossRoomRoleInfo userInfo = pmsgMultiBossEnterRoomRecvS.CInfo[0];
            int charType = userInfo.UnType;
            int lv = userInfo.Unlevel;
            int battlePower = userInfo.UnAttack;
            long charRoleId = userInfo.UllRoleId;
            string name = userInfo.SzName;

            inUserInfo = new NetData.RoomUserInfo((ulong)charRoleId, name, (uint)charType, (uint)lv, (uint)battlePower, NetData.instance.GameRoomData.UserList.Count);
            NetData.instance.AddGameRoomUser(inUserInfo);
        }

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            (readyPop as ReadyPopup).OnInUser(inUserInfo);
        }
    }

    /// <summary> 멀티보스레이드 방 이탈 응답 </summary>
    private void PMsgMultiBossLeaveRoomSHandler(PMsgMultiBossLeaveRoomS pmsgMultiBossLeaveRoomS)
    {
        uint ErrorCode = (uint)pmsgMultiBossLeaveRoomS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            //UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossLeaveRoomS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            //return;
        }

        NetData.instance.ClearGameRoomData();
        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            (readyPop as ReadyPopup).OnNetworkClose();
        }
    }

    /// <summary> 멀티보스레이드 방 이탈 브로드케이트 응답 </summary>
    private void PMsgMultiBossLeaveRoomRecvSHandler(PMsgMultiBossLeaveRoomRecvS pmsgMultiBossLeaveRoomRecvS)
    {
        ulong roleId = (ulong)pmsgMultiBossLeaveRoomRecvS.UllRoleId;
        int arr = NetData.instance.GetGameRoomUserArr(roleId);
        NetData.instance.GameRoomRemoveUser(roleId);

        //if (NetData.instance.GameRoomData.Owner == null)
        //    return;

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            if ( !NetData.instance.GameRoomData.IsLeader && roleId == NetData.instance.GameRoomData.Owner.Id)//방 폭파. 방장이 나갔음.
            {
                SendPMsgMultiBossLeaveRoomC(pmsgMultiBossLeaveRoomRecvS.UllRoomId);
            }
            else if (readyPop != null)
            {
                (readyPop as ReadyPopup).OnOutUser(arr);
            }
        }

    }

    /// <summary> 멀티보스레이드 유저 추방 응답 </summary>
    private void PMsgMultiBossKickRoomSHandler(PMsgMultiBossKickRoomS pmsgMultiBossKickRoomS)
    {
        /*//어차피 브로드캐스팅으로 방장도 받는다 일단 무시.
        ulong roleId = (ulong)pmsgColosseumKickRoomS.UllRoleId;
        int arr = NetData.instance.GetGameRoomUserArr(roleId);
        NetData.instance.GameRoomRemoveUser(roleId);

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop != null)
        {
            if(0 <= arr)
                (readyPop as ReadyPopup).OnOutUser(arr);
        }
        */
    }

    /// <summary> 멀티보스레이드 유저 추방 브로드캐스팅 응답 </summary>
    private void PMsgMultiBossKickRoomRecvSHandler(PMsgMultiBossKickRoomRecvS pmsgMultiBossKickRoomRecvS)
    {

        ulong roleId = (ulong)pmsgMultiBossKickRoomRecvS.UllRoleId;

        UIBasePanel readyPop = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPop == null)
        {
            NetData.instance.ClearGameRoomData();
            return;
        }

        if (roleId == NetData.instance.GetUserInfo().GetCharUUID())//나임
        {
            NetData.instance.ClearGameRoomData();
            (readyPop as ReadyPopup).OnNetworkClose();
        }
        else
        {
            int arr = NetData.instance.GetGameRoomUserArr(roleId);
            if (0 <= arr)
                (readyPop as ReadyPopup).OnOutUser(arr);

            NetData.instance.GameRoomRemoveUser(roleId);
        }

    }

    /// <summary> 멀티보스레이드 방 게임 시작 응답 </summary>
    private void PMsgMultiBossStartSHandler(PMsgMultiBossStartS pmsgMultiBossStartS)
    {
        uint ErrorCode = (uint)pmsgMultiBossStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossStartS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        NetData.instance.ClearRewardData();
        NetData.instance._RewardData.GetCoin = pmsgMultiBossStartS.UnDropCoin;
        NetData.instance._RewardData.GetExp = (uint)pmsgMultiBossStartS.UnDropExp;

        NetData.instance._RewardData.SaveLevel = NetData.instance.GetUserInfo()._Level;
        NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref NetData.instance._RewardData.SaveExp
            , ref NetData.instance._RewardData.SaveMaxExp);

        long roomId = pmsgMultiBossStartS.UllRoomId;
        int dungeonId = pmsgMultiBossStartS.UnBossBattleId;

        List<NetData.DropItemData> list = new List<NetData.DropItemData>();
        for (int i = 0; i < pmsgMultiBossStartS.CDrop.Count; i++)
        {
            MultiBossDropItem dropInfo = pmsgMultiBossStartS.CDrop[i];
            list.Add(new NetData.DropItemData((uint)dropInfo.UnItemId, (uint)dropInfo.UnItemNum, dropInfo.UnType));
        }

        NetData.instance._RewardData.GetList = list;

        UIBasePanel partnerPanel = UIMgr.GetUIBasePanel("UIPanel/DungeonPanel");
        if (partnerPanel != null)
        {
            (partnerPanel as DungeonPanel).OnMultyRaidGameStart();
        }
    }

    /// <summary> 멀티보스레이드 방 게임 종료 응답 </summary>
    private void PMsgMultiBossCompleteSHandler(PMsgMultiBossCompleteS pmsgMultiBossCompleteS)
    {
        uint ErrorCode = (uint)pmsgMultiBossCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgMultiBossCompleteS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        if (TownState.TownActive)
            return;

        NetData.instance.ClearGameRoomData();
        bool isVictory = pmsgMultiBossCompleteS.UnVictory == 0 ? false : true;
        if (!isVictory)
        {
            NetData.instance.ClearRewardData();
        }

        G_GameInfo.GameInfo.EndGame(isVictory);
    }


    /// <summary> 누적충전보상조회 </summary>
    private void PMsgRechargeTotalQueryInfoSHandler(PMsgRechargeTotalQueryInfoS pmsgRechargeTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("RechargeTotal");
        uint ErrorCode = (uint)pmsgRechargeTotalQueryInfoS.UnErrorCode;

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if(ErrorCode == (int)Sw.ErrorCode.ER_RechargeTotaQueryInfo_Welfare_End) // 이벤트기간이 아닙니다.
            {
                if (benefitPanel != null)
                {
                    (benefitPanel as BenefitPanel).ShowRechargeTotal(pmsgRechargeTotalQueryInfoS.UllMoneyTotal, pmsgRechargeTotalQueryInfoS.UnFetchInfo, pmsgRechargeTotalQueryInfoS.UnCanFetchInfo,true);
                }
            }
            else
            {
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
                Debug.Log(pmsgRechargeTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            }
         
            return;
        }

        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowRechargeTotal(pmsgRechargeTotalQueryInfoS.UllMoneyTotal, pmsgRechargeTotalQueryInfoS.UnFetchInfo, pmsgRechargeTotalQueryInfoS.UnCanFetchInfo);
        }
    }
    /// <summary>누적충전 보상획득 </summary>
    private void PMsgRechargeTotalFetchRewardSHandler(PMsgRechargeTotalFetchRewardS pmsgRechargeTotalFetchRewardS)
    {
        SceneManager.instance.EndNetProcess("GetRechargeTotal");

        uint ErrorCode = (uint)pmsgRechargeTotalFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRechargeTotalFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

   //   Debug.Log(pmsgRechargeTotalFetchRewardS);
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        //if (benefitPanel != null)
        //{
        //    (benefitPanel as BenefitPanel).ShowLevelUp(pmsgWelfareFetchRoleUpgradeRewardS.UnFetchInfo);
        //}
    }
    /// <summary> 누적충전 획득가능알림 </summary>
    private void PMsgRechargeTotalSynCanFetchSHandler(PMsgRechargeTotalSynCanFetchS pmsgRechargeTotalSynCanFetchS)
    {
       SceneManager.instance.SetAlram(AlramIconType.BENEFIT, true);
    }


    /// <summary> 일간충전보상조회 </summary>
    private void PMsgRechargeDailyQueryInfoSHandler(PMsgRechargeDailyQueryInfoS pmsgRechargeDailyQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("DailyTotal");
        uint ErrorCode = (uint)pmsgRechargeDailyQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRechargeDailyQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //   Debug.Log(pmsgRechargeDailyQueryInfoS);

        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowRechargeDaily(pmsgRechargeDailyQueryInfoS.UllMoneyTotal, pmsgRechargeDailyQueryInfoS.UnFetchInfo, pmsgRechargeDailyQueryInfoS.UnCanFetchInfo);
        }
    }
    /// <summary>일간충전 보상획득 </summary>
    private void PMsgRechargeDailyFetchRewardSHandler(PMsgRechargeDailyFetchRewardS pmsgRechargeDailyFetchRewardS)
    {
        SceneManager.instance.EndNetProcess("GetDailyTotal");

        uint ErrorCode = (uint)pmsgRechargeDailyFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRechargeDailyFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

//      Debug.Log(pmsgRechargeDailyFetchRewardS);
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        //if (benefitPanel != null)
        //{
        //    (benefitPanel as BenefitPanel).ShowLevelUp(pmsgWelfareFetchRoleUpgradeRewardS.UnFetchInfo);
        //}
    }
    /// <summary> 일간충전 획득가능알림 </summary>
    private void PMsgRechargeDailySynCanFetchSHandler(PMsgRechargeDailySynCanFetchS pmsgRechargeDailySynCanFetchS)
    {
        SceneManager.instance.SetAlram(AlramIconType.BENEFIT, true);
    }

    /// <summary> 누적소비보상조회 </summary>
    private void PMsgRechargeConsumerQueryInfoSHandler(PMsgRechargeConsumerQueryInfoS pmsgRechargeConsumerQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("ConsumerTotal");
        uint ErrorCode = (uint)pmsgRechargeConsumerQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRechargeConsumerQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //Debug.Log(pmsgRechargeConsumerQueryInfoS);


        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel");
        if (benefitPanel != null)
        {
            (benefitPanel as BenefitPanel).ShowRechargeConsumer(pmsgRechargeConsumerQueryInfoS.UllMoneyTotal, pmsgRechargeConsumerQueryInfoS.UnFetchInfo, pmsgRechargeConsumerQueryInfoS.UnCanFetchInfo);
        }
    }
    /// <summary>누적소비  보상획득 </summary>
    private void PMsgRechargeConsumerFetchRewardSHandler(PMsgRechargeConsumerFetchRewardS pmsgRechargeConsumerFetchRewardS)
    {
        SceneManager.instance.EndNetProcess("GetConsumerTotal");

        uint ErrorCode = (uint)pmsgRechargeConsumerFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRechargeConsumerFetchRewardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

 //     Debug.Log(pmsgRechargeConsumerFetchRewardS);
        //UIBasePanel benefitPanel = UIMgr.GetUIBasePanel("UIPanel/Benefitpanel2");
        //if (benefitPanel != null)
        //{
        //    (benefitPanel as BenefitPanel).ShowLevelUp(pmsgWelfareFetchRoleUpgradeRewardS.UnFetchInfo);
        //}
    }
    /// <summary>누적소비  획득가능알림 </summary>
    private void PMsgRechargeConsumerSynCanFetchSHandler(PMsgRechargeConsumerSynCanFetchS pmsgRechargeConsumerSynCanFetchS)
    {

       SceneManager.instance.SetAlram(AlramIconType.BENEFIT, true);

    }

    /// <summary> 정보 확인 </summary>
    private void PMsgQueryRoleInfoSHandler(PMsgQueryRoleInfoS pmsgQueryRoleInfoS)
    {
        uint ErrorCode = (uint)pmsgQueryRoleInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgQueryRoleInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgQueryRoleInfoSHandler " + pmsgQueryRoleInfoS);
        
        UIBasePanel readyPanel = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
        if (readyPanel != null)//준비창 유저 장착 장보
        {
            uint[] equipItem = new uint[(int)ePartType.PART_MAX];
            int count = (int)pmsgQueryRoleInfoS.UnEquipCount;
            for (int i = 0; i < count; i++)
            {
                PMsgQueryRoleInfoS.Types.EquipmentInfo info = pmsgQueryRoleInfoS.CEquipmentInfo[i];
                int equipId = info.UnType;
                if (equipId <= 0)
                    continue;

                Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equipId);
                if (equipInfo == null)
                {
                    Debug.LogError(string.Format("friend equip item not found lowdataID={0} error", equipId));
                    continue;
                }

                equipItem[equipInfo.UseParts] = (uint)equipId;
            }

            uint costumeId = (uint)pmsgQueryRoleInfoS.CCostumeInfo.UnType;
            uint skillSetId = 0;
            if (skillSetId <= 0)
            {
                switch (pmsgQueryRoleInfoS.UnType)
                {
                    case 11000:
                        skillSetId = 100;
                        break;
                    case 12000:
                        skillSetId = 200;
                        break;
                    case 13000:
                        skillSetId = 300;
                        break;
                }
            }

            (readyPanel as ReadyPopup).OnInUser((ulong)pmsgQueryRoleInfoS.UllRoleId, (uint)pmsgQueryRoleInfoS.UnType
                , equipItem[(int)ePartType.HELMET], costumeId, equipItem[(int)ePartType.CLOTH], equipItem[(int)ePartType.WEAPON]
                , skillSetId, pmsgQueryRoleInfoS.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE);
        }
        else//대상 상세정보 보기.
        {
            NetData._ItemData[] list = new NetData._ItemData[(int)ePartType.RING];
            int count = (int)pmsgQueryRoleInfoS.UnEquipCount;
            for (int i = 0; i < count; i++)
            {
                PMsgQueryRoleInfoS.Types.EquipmentInfo info = pmsgQueryRoleInfoS.CEquipmentInfo[i];
                if (info.UnType <= 0)
                    continue;

                Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)info.UnType);
                if (equipInfo == null)
                {
                    Debug.LogError(string.Format("friend equip item not found lowdataID={0} error", info.UnType));
                    continue;
                }
                
                NetData._ItemData equipItem = new NetData._ItemData((ulong)info.UnId, (uint)info.UnType, (ushort)info.UnEnchantTime, 0, 0, (uint)info.UnAttack);//(ushort)info.UnEvolveTime, (ushort)info.UnEvolveStar
                
                for (int j = 0; j < info.UnBasicValue.Count; j++)
                {
                    if(0 < info.UnBasicOption[j])
                        PMsgEquipmentQuerySHandlerSub_AddAbility(equipItem.StatList, (uint)info.UnBasicOption[j], info.UnBasicValue[j]);
                }

                list[(int)equipItem.EquipPartType - 1] = equipItem;
            }

            //List<ushort> skillList = new List<ushort>();
            //for(int i=0; i < pmsgQueryRoleInfoS.CCostumeInfo.UnSkillLevel.Count; i++)
            //{
            //    skillList.Add((ushort)pmsgQueryRoleInfoS.CCostumeInfo.UnSkillLevel[i] );
            //}

            NetData._CostumeData costume = new NetData._CostumeData(
                (ulong)pmsgQueryRoleInfoS.CCostumeInfo.UnId
                , (uint)pmsgQueryRoleInfoS.CCostumeInfo.UnType
                , (ushort)pmsgQueryRoleInfoS.CCostumeInfo.UnEvolveTime
                , (ushort)pmsgQueryRoleInfoS.CCostumeInfo.UnEvolveStar
                , null//skillList.ToArray()
                , true
                , true);

            uint[] tokenList = new uint[pmsgQueryRoleInfoS.CCostumeInfo.UnToken.Count];
            for(int i=0; i < pmsgQueryRoleInfoS.CCostumeInfo.UnToken.Count; i++)
            {
                tokenList[i] = (uint)pmsgQueryRoleInfoS.CCostumeInfo.UnToken[i];
            }

            uint skillSetId = 0;
            if (skillSetId <= 0)
            {
                switch (pmsgQueryRoleInfoS.UnType)
                {
                    case 11000:
                        skillSetId = 100;
                        break;
                    case 12000:
                        skillSetId = 200;
                        break;
                    case 13000:
                        skillSetId = 300;
                        break;
                }
            }

            costume._EquipJewelLowId = tokenList;

            UIBasePanel userPop = UIMgr.GetUIBasePanel("UIPopup/UserInfoPopup");
            if(userPop != null)
            {
                (userPop as UserInfoPopup).UserCharInfo((uint)pmsgQueryRoleInfoS.UnType, skillSetId
                , list, costume, pmsgQueryRoleInfoS.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE);

            }
        }
    }
    
    /// <summary> 인증정보 확인 </summary>
    private void PMsgUserCertifyInfoSHandler(PMsgUserCertifyInfoS pmsgUserCertifyInfoS)
    {
        int state = pmsgUserCertifyInfoS.UnCertifyStatus;
        string name = pmsgUserCertifyInfoS.SzName;
        string number = pmsgUserCertifyInfoS.SzIdCard;

        UIBasePanel certifyPanel = UIMgr.GetUIBasePanel("UIPopup/NameCertifyPopup");
        if (certifyPanel != null)
        {
            (certifyPanel as NameCertifyPopup).OnCertify(state, name, number);
        }
    }

    /// <summary> 정보 확인 </summary>
    private void PMsgUserCertifySetSHandler(PMsgUserCertifySetS pmsgUserCertifySetS)
    {
        UIBasePanel certifyPanel = UIMgr.GetUIBasePanel("UIPopup/NameCertifyPopup");
        uint ErrorCode = (uint)pmsgUserCertifySetS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            if (certifyPanel != null)
            {
                (certifyPanel as NameCertifyPopup).OnError();
            }

            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgUserCertifySetS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        string name = pmsgUserCertifySetS.SzName;
        string number = pmsgUserCertifySetS.SzIdCard;
        if (certifyPanel != null)
        {
            (certifyPanel as NameCertifyPopup).OnSendCertify(name, number);
        }
    }
    /*
    /// <summary> 활동량 확인 </summary>
    private void PMsgActivePointsQueryInfoSHandler(PMsgActivePointsQueryInfoS pmsgActivePointsQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgActivePointsQueryInfoC");
        uint ErrorCode = (uint)pmsgActivePointsQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgActivePointsQueryInfoS.GetType().ToString(), GetErrorString(ErrorCode), pmsgActivePointsQueryInfoS));
            return;
        }

        uint rewardInfo = pmsgActivePointsQueryInfoS.UnRewardFetchInfo;
        uint resetTime = pmsgActivePointsQueryInfoS.UnResetLastTaday;
        uint activePoint = pmsgActivePointsQueryInfoS.UnActivePoints;

        SceneManager.instance.SetAlram(AlramIconType.ACTIVITY, false);
        if (rewardInfo == 0)
        {
            //보상받은게없다면 현재 포인트가 1단계보상 포인트조건보다 크거나같을경우 알림표시가 떠야함
            if (activePoint >= _LowDataMgr.instance.GetLowDataActiveRewardList()[0].PointValue)
            {
                SceneManager.instance.SetAlram(AlramIconType.ACTIVITY, true);
            }
        }
        else
        {
            char[] rewardAlram = System.Convert.ToString(rewardInfo, 2).ToCharArray();
            //마지막 보상을 받기전이고 
            if (rewardAlram.Length < _LowDataMgr.instance.GetLowDataActiveRewardList().Count)
            {
                //다음 보상 활동포인트조건을 만족한경우
                if (activePoint >= _LowDataMgr.instance.GetLowDataActiveRewardList()[rewardAlram.Length].PointValue)
                    SceneManager.instance.SetAlram(AlramIconType.ACTIVITY, true);
            }
        }

        if (TownState.TownActive)
            SceneManager.instance.GetState<TownState>()._ActivityInfo = new TownState.ActivityInfo(activePoint, resetTime, rewardInfo);
    }

    /// <summary> 활동량 보상받기 </summary>
    private void PMsgActivePointsFetchRewardSHandler(PMsgActivePointsFetchRewardS pmsgActivePointsFetchRewardS)
    {
        uint ErrorCode = (uint)pmsgActivePointsFetchRewardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgActivePointsFetchRewardS.GetType().ToString(), GetErrorString(ErrorCode), pmsgActivePointsFetchRewardS));
            return;
        }

        if (!TownState.TownActive)
            return;

        uint rewardInfo = pmsgActivePointsFetchRewardS.UnRewardFetchInfo;

        SceneManager.instance.GetState<TownState>()._ActivityInfo.RewardInfo = rewardInfo;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/ActivityPanel");
        if (basePanel != null)
        {
            (basePanel as ActivityPanel).OnPMsgReward(rewardInfo);
        }
    }
    
    /// <summary> 활동량 보상받기 </summary>
    private void PMsgActivePointsTotalCountQueryInfoSHandler(PMsgActivePointsTotalCountQueryInfoS pmsgActivePointsTotalCountQueryInfoS)
    {
        if (!TownState.TownActive)
            return;
        
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/ActivityPanel");
        if (basePanel != null)
        {
            (basePanel as ActivityPanel).OnInitActivity(pmsgActivePointsTotalCountQueryInfoS);
        }
    }
*/
    /// <summary> 차관 정보 응답 </summary>
    private void PMsgArenaInfoSHandler(PMsgArenaInfoS pmsgArenaInfoS)
    {
        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        Debug.Log("pmsgArenaInfoS : " + pmsgArenaInfoS);
        if (arenaPanel != null)
            arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_INFO_S, pmsgArenaInfoS.UnDailyTime, pmsgArenaInfoS.UnDailyMaxTime, pmsgArenaInfoS.UnDailyReset,pmsgArenaInfoS.UnTopRank);
    }

    /// <summary> 차관 전황 정보 요청 응답 </summary>
    private void PMsgArenaFightListSHandler(PMsgArenaFightListS pmsgArenaFightListS)
    {
        SceneManager.instance.EndNetProcess("PMsgArenaFightListC");

        Debug.Log(string.Format("<color=green>{0}</color>", pmsgArenaFightListS));
        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (arenaPanel == null)
            return;

        List<NetData.ArenaFightInfo> list = new List<NetData.ArenaFightInfo>();
        int count = pmsgArenaFightListS.UnCount;
        for(int i=0; i < count; i++)
        {
            ArenaFightInfo info = pmsgArenaFightListS.CInfo[i];
            list.Add( new NetData.ArenaFightInfo(info.UllRoleId, info.UllId, info.UllTimestamp, info.SzName, info.UnType, info.UnLevel, info.UnAttack, info.UnRankOrig, info.UnFightResult, info.UnRankDest, info.UnCamp) );
        }

        arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_FIGHT_LIST_S, list);
    }

    /// <summary> 차관 새로운 전황 정보 통지 </summary>
    private void PMsgArenaFightResultNoticeSHandler(PMsgArenaFightResultNoticeS pmsgArenaFightResultNoticeS)
    {

    }

    /// <summary> 차관 리셋 횟수 요청 응답 </summary>
    private void PMsgArenaResetTimesSHandler(PMsgArenaResetTimesS pmsgArenaResetTimesS)
    {
        SceneManager.instance.EndNetProcess("PMsgArenaResetTimesC");
        uint ErrorCode = (uint)pmsgArenaResetTimesS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaResetTimesS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaResetTimesS));
            return;
        }
        
        Debug.LogError(pmsgArenaResetTimesS);
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ARENA_RESET_TIMES_S, pmsgArenaResetTimesS.UnDailyTime, pmsgArenaResetTimesS.UnDailyMaxTime, pmsgArenaResetTimesS.UnDailyReset);
    }

    /// <summary> 차관 본인 랭킹 조회 응답 </summary>
    private void PMsgArenaRankInfoSHandler(PMsgArenaRankInfoS pmsgArenaRankInfoS)
    {
        Debug.Log("pmsgArenaRankInfoS: " + pmsgArenaRankInfoS);
        int rank = NetData.instance.GetUserInfo().ArenaRanking;
        NetData.instance.GetUserInfo().ArenaRanking = pmsgArenaRankInfoS.UnRank;
        //if (rank <= 0)
        //{
            UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
            if (arenaPanel == null)
                return;

            arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_RANK_INFO_S, pmsgArenaRankInfoS.UnRank);
        //}
    }

    ///// <summary> 차관 랭킹 보상 획득 응답 </summary>
    //private void PMsgArenaGetRankAwardSHandler(PMsgArenaGetRankAwardS pmsgArenaGetRankAwardS)
    //{
    //    uint ErrorCode = (uint)pmsgArenaGetRankAwardS.UnErrorCode;
    //    if (ErrorCode != (int)Sw.ErrorCode.ER_success)
    //    {
    //        UIMgr.instance.AddErrorPopup((int)ErrorCode);
    //        Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaGetRankAwardS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaGetRankAwardS));
    //        return;
    //    }

    //    if (!TownState.IsMapLoad || NetData.instance.GetUserInfo().ArenaRanking == 0)
    //        return;
        
    //    string msg = string.Format(string.Format(_LowDataMgr.instance.GetStringCommon(1010), NetData.instance.GetUserInfo().ArenaRanking, pmsgArenaGetRankAwardS.UnRankPoint) );
    //    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
    //    UIMgr.AddLogChat(msg);
    //}

    /// <summary> 차관 TOP100 랭킹 정보 응답 </summary>
    private void PMsgArenaRankListSHandler(PMsgArenaRankListS pmsgArenaRankListS)
    {
        List<NetData.ArenaUserInfo> list = new List<NetData.ArenaUserInfo>();
        int myRank = pmsgArenaRankListS.UnMyRank;
        int count = pmsgArenaRankListS.UnCount;
        if (count <= 0)
            return;
        Debug.Log(pmsgArenaRankListS);
        for(int i=0; i < count; i++)
        {
            list.Add(new NetData.ArenaUserInfo(pmsgArenaRankListS.CInfo[i].UllRoleId
                , pmsgArenaRankListS.CInfo[i].SzName
                , pmsgArenaRankListS.CInfo[i].UnType
                , pmsgArenaRankListS.CInfo[i].UnLevel
                , 0
                , pmsgArenaRankListS.CInfo[i].UnAttack
                , pmsgArenaRankListS.CInfo[i].UnRank
                , 0));
        }

        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (arenaPanel == null)
            return;

        arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_RANK_LIST_S, list);
    }

    /// <summary> 차관 상대 리셋 요청 응답 </summary>
    private void PMsgArenaMatchListSHandler(PMsgArenaMatchListS pmsgArenaMatchListS)
    {
        SceneManager.instance.EndNetProcess("PMsgArenaMatchListC");
        uint ErrorCode = (uint)pmsgArenaMatchListS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaMatchListS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaMatchListS));
            return;
        }

        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (arenaPanel == null)
            return;

        Debug.Log(string.Format("<color=green>{0}</color>", pmsgArenaMatchListS));

        List<NetData.ArenaUserInfo> list = new List<NetData.ArenaUserInfo>();
        for(int i=0; i < pmsgArenaMatchListS.UnCount; i++)
        {
            MatchListInfo info = pmsgArenaMatchListS.CInfo[i];
            if ((ulong)info.UllRoleId == NetData.instance.GetUserInfo()._charUUID)//내꺼 
                continue;

            list.Add(new NetData.ArenaUserInfo((info.UllRoleId), info.SzName, info.UnType, info.UnLevel, (int)info.UnVipLevel, info.UnAttack, info.UnRank, (ulong)info.UllGuildId) );
        }
        
        arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_MATCH_LIST_S, list);
    }

    /// <summary> 차관 상대 상세 정보 </summary>
    private void PMsgArenaMatchInfoSHandler(PMsgArenaMatchInfoS pmsgArenaMatchInfoS)
    {
        Debug.Log(string.Format("<color=green>{0}</color>", pmsgArenaMatchInfoS));
        UIBasePanel arenaPanel = UIMgr.GetUIBasePanel("UIPanel/ArenaPanel");
        if (arenaPanel != null) {

            //내 데이터 생성
            NetData.instance.MakePlayerSyncData(true);

            //적군 셋팅
            List<NetData._PartnerData> parList = new List<NetData._PartnerData>();//파트너 셋팅
            for(int i=0; i < pmsgArenaMatchInfoS.CInfo.CHeroInfo.Count; i++)
            {
                NetData._PartnerData data = new NetData._PartnerData((ulong)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnHeroId, (ushort)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnType
                    , (uint)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnLevel, i //0, 0(ushort)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnEvolveStar, (ushort)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnEnchantTime
                    , (ulong)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UllExp,(uint)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnAttack, true);

                data.ActiveSkillList = new Dictionary<ushort, NetData._PartnerActiveSkillData>();

                //0번평타
                data.EquipBuffSkillData(0, new NetData._PartnerActiveSkillData((uint)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].UnBaseSkillId, 1));
                for (int j = 0; j < pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].CSkillInfo.Count; j++)
                {
                    ushort skillId = (ushort)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].CSkillInfo[j].UnSkillId;
                    if (skillId <= 0)
                        continue;
                   
                    data.EquipBuffSkillData((ushort)(j+1)
                        , new NetData._PartnerActiveSkillData(skillId, (byte)pmsgArenaMatchInfoS.CInfo.CHeroInfo[i].CSkillInfo[j].UnSkillLevel));

                }

                parList.Add(data);
            }

            List<int> equipList = new List<int>();//추후에 장비로 바꿔어야함.
            for (int i = 0; i < pmsgArenaMatchInfoS.CInfo.UnEquipmentId.Count; i++)
                equipList.Add(pmsgArenaMatchInfoS.CInfo.UnEquipmentId[i]);

            uint costumeId = (uint)pmsgArenaMatchInfoS.CInfo.UnCostumeId;
            if(costumeId <= 0)
            {
                Newbie.NewbieInfo newbieInfo = _LowDataMgr.instance.GetNewbieCharacterData((uint)pmsgArenaMatchInfoS.CInfo.UnType);
                if (newbieInfo != null)
                    costumeId = newbieInfo.CostumIdx;
            }

            NetData.instance.MakeEnemySyncData((ulong)pmsgArenaMatchInfoS.CInfo.UllRoleId
                , pmsgArenaMatchInfoS.CInfo.SzName
                , (uint)pmsgArenaMatchInfoS.CInfo.UnType
                , (uint)pmsgArenaMatchInfoS.CInfo.UnLevel
                , pmsgArenaMatchInfoS.CInfo.UnVipLevel
                , pmsgArenaMatchInfoS.CInfo.UnAttack
                , costumeId
                , pmsgArenaMatchInfoS.CInfo.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE ? true : false
                , equipList
                , parList
                , pmsgArenaMatchInfoS.CInfo.UnTitlePrefix
                , pmsgArenaMatchInfoS.CInfo.UnTitleSuffix
                , 0);
            
            arenaPanel.NetworkData(MSG_DEFINE._MSG_ARENA_MATCH_INFO_S);
        }
    }

    /// <summary> 차관 전투 요청 응답 </summary>
    private void PMsgArenaFightStartSHandler(PMsgArenaFightStartS pmsgArenaFightStartS)
    {
        SceneManager.instance.EndNetProcess("PMsgArenaFightStartC");
        uint ErrorCode = (uint)pmsgArenaFightStartS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaFightStartS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaFightStartS));
            return;
        }
    }
    /*
    /// <summary> 차관 전투 나가기 요청 응답 </summary>
    private void PMsgArenaFightLeaveSHandler(PMsgArenaFightLeaveS pmsgArenaFightLeaveS)
    {
        uint ErrorCode = (uint)pmsgArenaFightLeaveS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaFightLeaveS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaFightLeaveS));
            return;
        }
    }
    */

    /// <summary> 차관 전투 완료 응답 </summary>
    private void PMsgArenaFightCompleteSHandler(PMsgArenaFightCompleteS pmsgArenaFightCompleteS)
    {
        SceneManager.instance.EndNetProcess("PMsgArenaFightCompleteC");
        uint ErrorCode = (uint)pmsgArenaFightCompleteS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgArenaFightCompleteS.GetType().ToString(), GetErrorString(ErrorCode), pmsgArenaFightCompleteS));
            return;
        }
        Debug.Log(pmsgArenaFightCompleteS);

        uint getpoint = (uint)pmsgArenaFightCompleteS.UnPoint;
        bool isWin = pmsgArenaFightCompleteS.UnVictory == (int)FIGHT_RESULT.FIGHT_RESULT_WIN ? true : false;
        uint rangUpgradeGem = (uint)pmsgArenaFightCompleteS.UnRankUpgradeGem;
        //SendPMsgArenaRankInfoC();//내 차관 랭킹 다시 확인.

        //UIMgr.Open("UIPanel/ResultRewardStarPanel", isWin, getpoint);
        G_GameInfo.GameInfo.OpenResultPanel(isWin, getpoint, rangUpgradeGem);
    }

    /*[캐릭터 리스트 화면에서 출력되는 공지]
     [캐릭터가 게임에 로그인 될때 출력되는 공지]*/
    private void PMsgBulletineCompleteSHandler(PMsgBulletinSynNewInfoS pmsgBulletineSyncNewInfoS)
	{
        //SceneManager.instance.EndNetProcess("PMsgBulletinSynNewInfoS");
        //		uint ErrorCode = (uint)pmsgBulletineSyncNewInfoS.e.UnErrorCode;
        //		if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        //		{
        //			UIMgr.instance.AddErrorPopup((int)ErrorCode);
        //			Debug.Log(string.Format("{0}-{1} = {2}", pmsgBulletineSyncNewInfoS.GetType().ToString(), GetErrorString(ErrorCode), pmsgBulletineSyncNewInfoS));
        //			return;
        //		}

        Debug.Log(pmsgBulletineSyncNewInfoS);
        if (pmsgBulletineSyncNewInfoS.UnType == (int) eBulletinType.BulletinType_Role_Playing)
        {
            string msg = string.Format("[{0}]{1}", pmsgBulletineSyncNewInfoS.SzTitle, pmsgBulletineSyncNewInfoS.SzBulletin);
            SceneManager.instance.SetNoticePanel(NoticeType.System, 0, msg);
        }
        else if(pmsgBulletineSyncNewInfoS.UnType == (int)eBulletinType.BulletinType_Role_List)
        {

        }

		//string szNotice = pmsgBulletineSyncNewInfoS.SzBulletin;

		//Debug.Log (" notice :" + szNotice);
		//SendPMsgArenaRankInfoC();//내 차관 랭킹 다시 확인.
		
		//UIMgr.Open("UIPanel/ResultRewardStarPanel", isWin, getpoint);

	}
    
    /// <summary> 신규 장비 자랑하기 </summary>
    private void PMsgSynDynamicBulletinNewEquipmentSHandler(PMsgSynDynamicBulletinNewEquipmentS pmsgSynDynamicBulletinNewEquipmentS)
    {
        Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinNewEquipmentS);
        string dataStr = string.Format("{0},{1},{2},{3}"
            , pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnType
            , pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicValue
            , pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnAttack
            , pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnEnchantTime);

        string options = null;
        for(int i=0; i < pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicOption.Count; i++)
        {
            if(i == 0 )
                options = string.Format("{0},{1}", pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicValue[i]);
            else
                options += string.Format("|{0},{1}", pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinNewEquipmentS.CEquipmentInfo.UnBasicValue[i]);
        }

        if (!string.IsNullOrEmpty(options))
            dataStr += string.Format("/{0}", options);
        
        SceneManager.instance.SetBullewtin(
            (int)eDynamicBulletinType.DynamicBulletinType_Equipment, 
            pmsgSynDynamicBulletinNewEquipmentS.UnItemType, 
            pmsgSynDynamicBulletinNewEquipmentS.UnLevel, 
            0,
            (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Null, 
            pmsgSynDynamicBulletinNewEquipmentS.StrName,
            dataStr
        );
    }

    /// <summary> 강화 장비 자랑하기 </summary>
    private void PMsgSynDynamicBulletinEnchantEquipmentSHandler(PMsgSynDynamicBulletinEnchantEquipmentS pmsgSynDynamicBulletinEnchantEquipmentS)
    {
        Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinEnchantEquipmentS);
        string dataStr = string.Format("{0},{1},{2}"
            , pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnType
            , pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnAttack
            , pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnEnchantTime);

        string options = null;
        for (int i = 0; i < pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnBasicOption.Count; i++)
        {
            if (i == 0)
                options = string.Format("{0},{1}", pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnBasicValue[i]);
            else
                options += string.Format("|{0},{1}", pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinEnchantEquipmentS.CEquipmentInfo.UnBasicValue[i]);
        }

        if (!string.IsNullOrEmpty(options))
            dataStr += string.Format("/{0}", options);

        SceneManager.instance.SetBullewtin(
            (int)eDynamicBulletinType.DynamicBulletinType_Equipment,
            pmsgSynDynamicBulletinEnchantEquipmentS.UnItemType,
            pmsgSynDynamicBulletinEnchantEquipmentS.UnLevel,
            pmsgSynDynamicBulletinEnchantEquipmentS.UnCount,
            (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Enchant,
            pmsgSynDynamicBulletinEnchantEquipmentS.StrName,
            dataStr
        );
    }

    /// <summary> 승급 장비 자랑하기 </summary>
    private void PMsgSynDynamicBulletinEvolveEquipmentSHandler(PMsgSynDynamicBulletinEvolveEquipmentS pmsgSynDynamicBulletinEvolveEquipmentS)
    {
        Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinEvolveEquipmentS);
        string dataStr = string.Format("{0},{1},{2}"
            , pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnType
            , pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnAttack
            , pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnEnchantTime);

        string options = null;
        for (int i = 0; i < pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnBasicOption.Count; i++)
        {
            if (i == 0)
                options = string.Format("{0},{1}", pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnBasicValue[i]);
            else
                options += string.Format("|{0},{1}", pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnBasicOption[i], pmsgSynDynamicBulletinEvolveEquipmentS.CEquipmentInfo.UnBasicValue[i]);
        }

        if (!string.IsNullOrEmpty(options))
            dataStr += string.Format("/{0}", options);

        SceneManager.instance.SetBullewtin(
            (int)eDynamicBulletinType.DynamicBulletinType_Equipment,
            pmsgSynDynamicBulletinEvolveEquipmentS.UnItemType,
            pmsgSynDynamicBulletinEvolveEquipmentS.UnLevel,
            pmsgSynDynamicBulletinEvolveEquipmentS.UnCount,
            (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Evolve,
            pmsgSynDynamicBulletinEvolveEquipmentS.StrName,
            dataStr
        );
    }

    /// <summary> 신규 파트너 자랑하기 </summary>
    private void PMsgSynDynamicBulletinNewHeroSHandler(PMsgSynDynamicBulletinNewHeroS pmsgSynDynamicBulletinNewHeroS)
    {
        Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinNewHeroS);
        //string dataStr = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnType
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnBasicValue
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnAttack
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnEnchantTime
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnEvolveTime
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnEvolveStar
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnRandomOption1
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnRandomValue1
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnRandomOption2
        //    , pmsgSynDynamicBulletinNewHeroS.CEquipmentInfo.UnRandomValue2);

        SceneManager.instance.SetBullewtin(
            (int)eDynamicBulletinType.DynamicBulletinType_Hero,
            pmsgSynDynamicBulletinNewHeroS.UnItemType,
            pmsgSynDynamicBulletinNewHeroS.UnLevel,
            0,
            (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Null,
            pmsgSynDynamicBulletinNewHeroS.StrName,
            null
        );
    }

    /// <summary> 강화 파트너 자랑하기 </summary>
    //private void PMsgSynDynamicBulletinEnchantHeroSHandler(PMsgSynDynamicBulletinEnchantHeroS pmsgSynDynamicBulletinEnchantHeroS)
    //{
    //    Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinEnchantHeroS);

    //    SceneManager.instance.SetBullewtin(
    //        (int)eDynamicBulletinType.DynamicBulletinType_Hero,
    //        pmsgSynDynamicBulletinEnchantHeroS.UnItemType,
    //        pmsgSynDynamicBulletinEnchantHeroS.UnLevel,
    //        pmsgSynDynamicBulletinEnchantHeroS.UnCount,
    //        (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Enchant,
    //        pmsgSynDynamicBulletinEnchantHeroS.StrName,
    //        null
    //    );
    //}

    /// <summary> 승급 파트너 자랑하기 </summary>
    private void PMsgSynDynamicBulletinEvolveHeroSHandler(PMsgSynDynamicBulletinEvolveHeroS pmsgSynDynamicBulletinEvolveHeroS)
    {
        Debug.Log("자랑하기 : " + pmsgSynDynamicBulletinEvolveHeroS);

        SceneManager.instance.SetBullewtin(
            (int)eDynamicBulletinType.DynamicBulletinType_Hero,
            pmsgSynDynamicBulletinEvolveHeroS.UnItemType,
            pmsgSynDynamicBulletinEvolveHeroS.UnLevel,
            pmsgSynDynamicBulletinEvolveHeroS.UnCount,
            (int)eDynamicBulletinOperateType.DynamicBulletinOperateType_Evolve,
            pmsgSynDynamicBulletinEvolveHeroS.StrName,
            null
        );
    }

    /// <summary> 신규 소비아이템 </summary>
    private void PMsgPMsgSynNewItemSHandler(PMsgSynNewItemS pmsgSynNewItemS)
    {
        Debug.Log(string.Format("<color=yellow>{0}</color>", pmsgSynNewItemS) );
        
        NetData._ItemData newItem = NetData.instance.GetUserInfo().CreateUseItem((ulong)pmsgSynNewItemS.UnId, (uint)pmsgSynNewItemS.UnType, (ushort)pmsgSynNewItemS.UnNum, true);
        SceneManager.instance.SetAlram(AlramIconType.CATEGORY, true);
        if (!SceneManager.instance.IsShowLoadingPanel())
        {
            string msg = string.Format(_LowDataMgr.instance.GetStringCommon(832), newItem.GetLocName());
            UIMgr.AddLogChat(msg);
            
            Item.ItemInfo lowData = newItem.GetUseLowData();
            bool isShard = (lowData.Type == (byte)AssetType.CostumeShard || lowData.Type == (byte)AssetType.PartnerShard);
            SceneManager.instance.SetNoticePanel(NoticeType.GetItem, lowData.Icon, null, isShard);
        }

        //}
    }

    /// <summary>셋트아이템 보유중인거 </summary>
    private void PMsgEquipmentSetQuerySHandler(PMsgEquipmentSetQueryS pmsgEquipmentSetQueryS)
    {
        Debug.Log("EquipmentSetQuery " + pmsgEquipmentSetQueryS);
        uint mountItem = (uint)pmsgEquipmentSetQueryS.UnCurrentSetId;
        for (int i=0; i < pmsgEquipmentSetQueryS.UnSetId.Count; i++)
        {
            if (pmsgEquipmentSetQueryS.UnSetId[i] <= 0)
                continue;

            NetData.instance.GetUserInfo().AddSetItemData(new NetData.SetItemData((uint)pmsgEquipmentSetQueryS.UnSetId[i], mountItem.Equals((uint)pmsgEquipmentSetQueryS.UnSetId[i])) );
        }
    }

    /// <summary> 셋트아이템 장착 </summary>
    private void PMsgEquipmentSetChangeSHandler(PMsgEquipmentSetChangeS pmsgEquipmentSetChangeS)
    {
        SceneManager.instance.EndNetProcess("PMsgEquipmentSetChangeC");
        uint ErrorCode = (uint)pmsgEquipmentSetChangeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgEquipmentSetChangeS.GetType().ToString(), GetErrorString(ErrorCode), pmsgEquipmentSetChangeS));
            return;
        }
        
        uint prevMountId = NetData.instance.GetUserInfo().GetMountSetItem().LowDataId;
        NetData.instance.GetUserInfo().ChangeMountSetItem((uint)pmsgEquipmentSetChangeS.UnSetId);
        
        UIBasePanel equiPnael = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (equiPnael != null)
            equiPnael.NetworkData(MSG_DEFINE._MSG_EQUIPMENT_SET_CHANGE_C, (uint)pmsgEquipmentSetChangeS.UnSetId, prevMountId);
    }

    /// <summary> 셋트아이템 추가 </summary>
    private void PMsgEquipmentSetSelectSHandler(PMsgEquipmentSetSelectS pmsgEquipmentSetSelectS)
    {
        SceneManager.instance.EndNetProcess("PMsgEquipmentSetSelectC");
        uint ErrorCode = (uint)pmsgEquipmentSetSelectS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgEquipmentSetSelectS.GetType().ToString(), GetErrorString(ErrorCode), pmsgEquipmentSetSelectS));
            return;
        }
        
        Debug.Log("pmsgEquipmentSetSelectS " + pmsgEquipmentSetSelectS);

        NetData.SetItemData setData = new NetData.SetItemData((uint)pmsgEquipmentSetSelectS.UnSetId, false);
        NetData.instance.GetUserInfo().AddSetItemData(setData);

        UIBasePanel equiPnael = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (equiPnael != null)
            equiPnael.NetworkData(MSG_DEFINE._MSG_EQUIPMENT_SET_SELECT_C, setData);
    }

    /// <summary> 연속강화 응답 </summary>
    private void PMsgEquipmentEnchantTurboSHandler(PMsgEquipmentEnchantTurboS pmsgEquipmentEnchantTurboS)
    {
        SceneManager.instance.EndNetProcess("PMsgEquipmentEnchantTurboC");
        uint ErrorCode = (uint)pmsgEquipmentEnchantTurboS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(string.Format("{0}-{1} = {2}", pmsgEquipmentEnchantTurboS.GetType().ToString(), GetErrorString(ErrorCode), pmsgEquipmentEnchantTurboS));
            return;
        }
        
        NetData._ItemData itemData = NetData.instance.GetUserInfo().GetEquipItemForIdx((ulong)pmsgEquipmentEnchantTurboS.UnId);
        if (itemData == null)//보유 리스트에서 다시 찾는다.
            itemData = NetData.instance.GetUserInfo().GetItemDataForIndexAndType((ulong)pmsgEquipmentEnchantTurboS.UnId, (byte)eItemType.EQUIP);

        UIBasePanel equipPanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (equipPanel != null)
            equipPanel.NetworkData(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_TURBO_C, itemData);
    }

    ///<summary>일일 업적 단계 정보 조회</summary> 
    private void PMsgAchieveDailyQueryInfoSHandler(PMsgAchieveDailyQueryInfoS pmsgAchieveDailyQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyQueryInfoS);

        List<NetData.AchieveLevelInfo> list = new List<NetData.AchieveLevelInfo>(); //업적단계정보

        uint lvCount = pmsgAchieveDailyQueryInfoS.UnLevelCount;  //업적단계정보수량
        for (int i = 0; i < pmsgAchieveDailyQueryInfoS.CAchieveDailyLevelInfo.Count; i++)
        {
            NetData.AchieveLevelInfo info = new NetData.AchieveLevelInfo(pmsgAchieveDailyQueryInfoS.CAchieveDailyLevelInfo[i].UnType, 0,
                pmsgAchieveDailyQueryInfoS.CAchieveDailyLevelInfo[i].UnLevel, pmsgAchieveDailyQueryInfoS.CAchieveDailyLevelInfo[i].UnComplete, pmsgAchieveDailyQueryInfoS.CAchieveDailyLevelInfo[i].UnFetch);
            list.Add(info);
        }

        //SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, false);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    // 달성되있는데 보상안받은경우가 있다면 알림표시
        //    if (list[i].Complete == 1 && list[i].Fetch == 0)
        //    {
        //        SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, true);
        //        continue;
        //    }
        //}


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).SetDailyAchieveInfo(lvCount, list, pmsgAchieveDailyQueryInfoS.UnPoints);
        }



    }
    /// <summary>
    /// 전투관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyFightTotalQueryInfoS"></param>
    private void PMsgAchieveDailyFightTotalQueryInfoSHandler(PMsgAchieveDailyFightTotalQueryInfoS pmsgAchieveDailyFightTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyFightTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyFightTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyFightTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyFightTotalQueryInfoS);

        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();
        dataDic.Add(114, pmsgAchieveDailyFightTotalQueryInfoS.UnKillOtherRoleTotal);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(1, dataDic);
        }

    }
    /// <summary>
    /// 소셜관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyFriendTotalQueryInfoS"></param>
    private void PMsgAchieveDailyFriendTotalQueryInfoSHandler(PMsgAchieveDailyFriendTotalQueryInfoS pmsgAchieveDailyFriendTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyFriendTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyFriendTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyFriendTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyFriendTotalQueryInfoS);
        
        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();
        dataDic.Add(601, pmsgAchieveDailyFriendTotalQueryInfoS.UnSendPowerFriend);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(2, dataDic);
        }
    }
    /// <summary>
    /// 경제관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyMoneyTotalQueryInfoS"></param>
    private void PMsgAchieveDailyMoneyTotalQueryInfoSHandler(PMsgAchieveDailyMoneyTotalQueryInfoS pmsgAchieveDailyMoneyTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyMoneyTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyMoneyTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyMoneyTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyMoneyTotalQueryInfoS);

        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();
        dataDic.Add(402, (uint)pmsgAchieveDailyMoneyTotalQueryInfoS.UnSpendCoinTotal);
        dataDic.Add(404, (uint)pmsgAchieveDailyMoneyTotalQueryInfoS.UnSpendGemTotal);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(3, dataDic);
        }
    }
    /// <summary>
    /// 컨텐츠관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyPlayTotalQueryInfoS"></param>
    private void PMsgAchieveDailyPlayTotalQueryInfoSHandler(PMsgAchieveDailyPlayTotalQueryInfoS pmsgAchieveDailyPlayTotalQueryInfoS)
    {

        SceneManager.instance.EndNetProcess("PMsgAchieveDailyPlayTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyPlayTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log(pmsgAchieveDailyPlayTotalQueryInfoS);

        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();
        dataDic.Add(501, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnAdventureCount);//모험클리어
        dataDic.Add(504, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnCoinStageCount);//골드던전클리어
        dataDic.Add(505, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnExpStageCount);//경험치던전클리어
        dataDic.Add(503, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnTowerCount);//마탑
        dataDic.Add(521, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnSinglePvp);//차관
        dataDic.Add(517, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnTeamPvpCount);//콜로
        dataDic.Add(506, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnFiveFlowerCount);//오화
        dataDic.Add(516, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnCompleteMultyBoss);//멀티보스
        //dataDic.Add(502, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnPvpRankCount);//어려움모드
        //dataDic.Add(113, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnKillMessBossCount);//난투장몬스터
        dataDic.Add(511, (uint)pmsgAchieveDailyPlayTotalQueryInfoS.UnThreeTalentCount);//보스레이드

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(4, dataDic);
        }
    }

    /// <summary>
    /// 캐릭터관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyRoleTotalQueryInfoS"></param>
    private void PMsgAchieveDailyRoleTotalQueryInfoSHandler(PMsgAchieveDailyRoleTotalQueryInfoS pmsgAchieveDailyRoleTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyRoleTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyRoleTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyRoleTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyRoleTotalQueryInfoS);

        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();
        dataDic.Add(115, (uint)pmsgAchieveDailyRoleTotalQueryInfoS.UnComposeLoginCount);//로그인

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(5, dataDic);
        }
    }
    /// <summary>
    /// vip관련 일일업적
    /// </summary>
    /// <param name="pmsgAchieveDailyVipTotalQueryInfoS"></param>
    private void PMsgAchieveDailyVipTotalQueryInfoSHandler(PMsgAchieveDailyVipTotalQueryInfoS pmsgAchieveDailyVipTotalQueryInfoS)
    {
        SceneManager.instance.EndNetProcess("PMsgAchieveDailyVipTotalQueryInfoC");
        uint ErrorCode = (uint)pmsgAchieveDailyVipTotalQueryInfoS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyVipTotalQueryInfoS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyVipTotalQueryInfoS);

        Dictionary<uint, uint> dataDic = new Dictionary<uint, uint>();

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).GetDailyAchieveInfo(6, dataDic);
        }

    }
    private void PMsgAchieveDailyFetchAwardSHandler(PMsgAchieveDailyFetchAwardS pmsgAchieveDailyFetchAwardS)
    {
        uint ErrorCode = (uint)pmsgAchieveDailyFetchAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyFetchAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyFetchAwardS);

        List<NetData.AchieveLevelInfo> infoList = new List<NetData.AchieveLevelInfo>();

        NetData.AchieveLevelInfo info  = new NetData.AchieveLevelInfo(pmsgAchieveDailyFetchAwardS.UnType, 0, pmsgAchieveDailyFetchAwardS.UnLevel, pmsgAchieveDailyFetchAwardS.UnComplete, pmsgAchieveDailyFetchAwardS.UnFetch);
        infoList.Add(info);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).AfterGetDailyAchieve(infoList);
        }


    }
    /// <summary>
    /// 일일업적 포인트보상획득 요청
    /// </summary>
    /// <param name="pmsgAchieveDailyFetchPointsAwardS"></param>
    private void PMsgAchieveDailyFetchPointsAwardSHandler(PMsgAchieveDailyFetchPointsAwardS pmsgAchieveDailyFetchPointsAwardS)
    {
        uint ErrorCode = (uint)pmsgAchieveDailyFetchPointsAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyFetchPointsAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        Debug.Log(pmsgAchieveDailyFetchPointsAwardS);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).SetDailyPoint(true,pmsgAchieveDailyFetchPointsAwardS.UnLevel);
        }
    }
    /// <summary>
    /// 일일업적 포인트 누적치 동기화
    /// </summary>
    /// <param name="pmsgAchieveDailySynPointsTotalValueS"></param>
    private void PMsgAchieveDailySynPointsTotalValueSHandler(PMsgAchieveDailySynPointsTotalValueS pmsgAchieveDailySynPointsTotalValueS)
    {
        Debug.Log(pmsgAchieveDailySynPointsTotalValueS);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).SetDailyPoint(false,pmsgAchieveDailySynPointsTotalValueS.UnPoints);
        }

    }
    /// <summary>
    /// 일일업적치 동기화
    /// </summary>
    /// <param name="pmsgAchieveDailySynAchieveStatisValueS"></param>
    private void PMsgAchieveDailySynAchieveStatisValueSHandler(PMsgAchieveDailySynAchieveStatisValueS pmsgAchieveDailySynAchieveStatisValueS)
    {
        Debug.Log(pmsgAchieveDailySynAchieveStatisValueS);

        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).SyncDailyAchievValue(pmsgAchieveDailySynAchieveStatisValueS.UnType, (uint)pmsgAchieveDailySynAchieveStatisValueS.UllValue);
        }
    }
    private void PMsgAchieveDailySynFightDataTotalValueSHandler(PMsgAchieveDailySynFightDataTotalValueS pmsgAchieveDailySynFightDataTotalValueS)
    {
        uint ErrorCode = (uint)pmsgAchieveDailySynFightDataTotalValueS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailySynFightDataTotalValueS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
        }
        
    }
    private void PMsgAchieveDailySynAchieveCompleteSHandler(PMsgAchieveDailySynAchieveCompleteS pmsgAchieveDailySynAchieveCompleteS)
    {
        Debug.Log(string.Format("일일업적달성메시지 !{0}" , pmsgAchieveDailySynAchieveCompleteS));

        //테이블에 있는거일떄만 
        List<Achievement.DailyInfo> daylist = _LowDataMgr.instance.GetLowDataDaiylAchievementInfoList(pmsgAchieveDailySynAchieveCompleteS.UnType);

        if (daylist.Count > 0)
        {
            SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, true);

            //업적 실시간으로 완료됬을때 호출됨
            string data = string.Format("{0},{1},{2},{3}",
                pmsgAchieveDailySynAchieveCompleteS.UnType,
                0,
                pmsgAchieveDailySynAchieveCompleteS.UnLevel,
                0);


            SceneManager.instance.SetNoticePanel(NoticeType.Achiev, 0, data);

        }

    }
    private void PMsgAchieveDailyOneKeyFetchAchieveAwardSHandler(PMsgAchieveDailyOneKeyFetchAchieveAwardS pmsgAchieveDailyOneKeyFetchAchieveAwardS)
    {
        uint ErrorCode = (uint)pmsgAchieveDailyOneKeyFetchAchieveAwardS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgAchieveDailyOneKeyFetchAchieveAwardS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
        }

        Debug.Log(pmsgAchieveDailyOneKeyFetchAchieveAwardS);

        List<NetData.AchieveLevelInfo> infoList = new List<NetData.AchieveLevelInfo>();

        for (int i = 0; i < pmsgAchieveDailyOneKeyFetchAchieveAwardS.CReachInfo.Count; i++)
        {
            NetData.AchieveLevelInfo info = new NetData.AchieveLevelInfo(pmsgAchieveDailyOneKeyFetchAchieveAwardS.CReachInfo[i].UnType, 0,
                pmsgAchieveDailyOneKeyFetchAchieveAwardS.CReachInfo[i].UnLevel, pmsgAchieveDailyOneKeyFetchAchieveAwardS.CReachInfo[i].UnComplete,
                pmsgAchieveDailyOneKeyFetchAchieveAwardS.CReachInfo[i].UnFetch);
            infoList.Add(info);
        }


        UIBasePanel achievePanel = UIMgr.GetUIBasePanel("UIPanel/AchievePanel");
        if (achievePanel != null)
        {
            (achievePanel as AchievePanel).AfterGetDailyAchieve(infoList);
        }
    }

    /// <summary> 신분 리스트 요청 응답 </summary>
    private void PMsgRoleIdentifyListSHandler(PMsgRoleIdentifyListS pmsgRoleIdentifyListS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleIdentifyListC");
        Debug.Log("Status Info = " + pmsgRoleIdentifyListS);

        List<NetData.StatusData> dataList = new List<NetData.StatusData>();
        int count = pmsgRoleIdentifyListS.UnCount;
        for(int i=0; i < count; i++)
        {
            RoleIdentifyInfo info = pmsgRoleIdentifyListS.CIdentifyInfo[i];
            dataList.Add( new NetData.StatusData(true, info.UnPosition == 1, info.UnId, info.UnLevel, info.UnPoint, (uint)info.UnType) );
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_IDENTIFY_LIST_S, dataList);
    }

    /// <summary> 신분 추가 요청 응답 </summary>
    private void PMsgRoleIdentifyUnlockSHandler(PMsgRoleIdentifyUnlockS pmsgRoleIdentifyUnlockS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleIdentifyUnlockC");
        Debug.Log("PMsgRoleIdentifyUnlockS " + pmsgRoleIdentifyUnlockS);
        uint ErrorCode = (uint)pmsgRoleIdentifyUnlockS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleIdentifyUnlockS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCK_S, pmsgRoleIdentifyUnlockS.UnType);
    }

    /// <summary> 신분 렙업 요청 응답 </summary>
    private void PMsgRoleIdentifyUpgradeSHandler(PMsgRoleIdentifyUpgradeS pmsgRoleIdentifyUpgradeS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleIdentifyUpgradeC");
        uint ErrorCode = (uint)pmsgRoleIdentifyUpgradeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleIdentifyUpgradeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_IDENTIFY_UPGRADE_S, pmsgRoleIdentifyUpgradeS.UnId);
    }

    /// <summary> 신분 장착 요청 응답 </summary>
    private void PMsgRoleIdentifyUseSHandler(PMsgRoleIdentifyUseS pmsgRoleIdentifyUseS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleIdentifyUseC");
        uint ErrorCode = (uint)pmsgRoleIdentifyUseS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleIdentifyUseS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_IDENTIFY_USE_S, pmsgRoleIdentifyUseS.UnId);
    }

    /// <summary> 스킬리스트 요청 응답 </summary>
    private void PMsgRoleActiveSkillListSHandler(PMsgRoleActiveSkillListS pmsgRoleActiveSkillListS)
    {
        //SceneManager.instance.EndNetProcess("PMsgRoleActiveSkillListC");
        Debug.Log("pmsgRoleActiveSkillListS " + pmsgRoleActiveSkillListS);
        List<NetData.SkillSetData> list = NetData.instance.GetUserInfo()._SKillSetList;
        for (int i=0; i < pmsgRoleActiveSkillListS.UnCount; i++)
        {
            SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet((uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnType);

            NetData.SkillSetData data = new NetData.SkillSetData(pmsgRoleActiveSkillListS.CSkillInfo[i].UnPosition == 1,
                new uint[] {
                    (uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnLevel[0],
                    (uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnLevel[1],
                    (uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnLevel[2],
                    (uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnLevel[3]
                }
                , new uint[] {
                    setInfo.skill1,
                    setInfo.skill2,
                    setInfo.skill3,
                    setInfo.skill4
                }
                , setInfo.Id
                , (uint)pmsgRoleActiveSkillListS.CSkillInfo[i].UnIdentifyType);

            bool isAdd = true;
            for(int j=0; j < list.Count; j++)
            {
                if(list[j].SkillSetId.Equals(data.SkillSetId) ) {
                    list[j] = data;
                    isAdd = false;
                    break;
                }
            }
            if(isAdd)
                list.Add(data);
        }

        //NetData.instance.GetUserInfo()._SKillSetList.AddRange(list);
    }

    /// <summary> 스킬 렙업 요청 응답 </summary>
    private void PMsgRoleActiveSkillUpgradeSHandler(PMsgRoleActiveSkillUpgradeS pmsgRoleActiveSkillUpgradeS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleActiveSkillUpgradeC");
        uint ErrorCode = (uint)pmsgRoleActiveSkillUpgradeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleActiveSkillUpgradeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //NetData.SkillSetData skillSetData = NetData.instance.GetUserInfo().GetSkillSetData((uint)pmsgRoleActiveSkillUpgradeS.UnType);
        //skillSetData.SkillLevel[pmsgRoleActiveSkillUpgradeS.UnIdx - 1] += 1;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_S, pmsgRoleActiveSkillUpgradeS.UnType, pmsgRoleActiveSkillUpgradeS.UnIdx);
    }

    /// <summary> 스킬 장착 요청 응답 </summary>
    private void PMsgRoleActiveSkillUseSHandler(PMsgRoleActiveSkillUseS pmsgRoleActiveSkillUseS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleActiveSkillUseC");
        Debug.Log("pmsgRoleActiveSkillUseS " + pmsgRoleActiveSkillUseS);
        uint ErrorCode = (uint)pmsgRoleActiveSkillUseS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleActiveSkillUseS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }
        
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_USE_S, (uint)pmsgRoleActiveSkillUseS.UnType);
    }

    /// <summary> 스킬 일괄 강화 </summary>
    private void PMsgRoleActiveSkillUpgradeTurboSHandler(PMsgRoleActiveSkillUpgradeTurboS pmsgRoleActiveSkillUpgradeTurboS)
    {
        SceneManager.instance.EndNetProcess("PMsgRoleActiveSkillUpgradeTurboC");
        Debug.Log("pmsgRoleActiveSkillUpgradeTurboS " + pmsgRoleActiveSkillUpgradeTurboS);
        uint ErrorCode = (uint)pmsgRoleActiveSkillUpgradeTurboS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleActiveSkillUpgradeTurboS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //NetData.SkillSetData skillSetData = NetData.instance.GetUserInfo().GetSkillSetData((uint)pmsgRoleActiveSkillUpgradeTurboS.UnType);
        //skillSetData.SkillLevel[pmsgRoleActiveSkillUpgradeTurboS.UnIdx - 1] = (uint)pmsgRoleActiveSkillUpgradeTurboS.UnTimes;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_TURBO_S, pmsgRoleActiveSkillUpgradeTurboS.UnType, pmsgRoleActiveSkillUpgradeTurboS.UnIdx);
    }
    
    /// <summary> 패시브리스트 요청 응답 </summary>
    private void PMsgRolePassiveSkillListSHandler(PMsgRolePassiveSkillListS pmsgRolePassiveSkillListS)
    {
        //SceneManager.instance.EndNetProcess("pmsgRolePassiveSkillListC");
        Debug.Log(pmsgRolePassiveSkillListS);
        List<NetData.PassiveData> list = NetData.instance.GetUserInfo()._PassiveList;
        for (int i = 0; i < pmsgRolePassiveSkillListS.UnCount; i++)
        {
            NetData.PassiveData data = new NetData.PassiveData(pmsgRolePassiveSkillListS.CSkillInfo[i].UnPosition == 1,
                pmsgRolePassiveSkillListS.CSkillInfo[i].UnSkillId,
                (uint)pmsgRolePassiveSkillListS.CSkillInfo[i].UnLevel,
                (uint)pmsgRolePassiveSkillListS.CSkillInfo[i].UnIdentifyType);

            bool isAdd = true;
            for (int j=0; j < list.Count; j++)
            {
                if(list[j].PassiveId.Equals(data.PassiveId) )
                {
                    list[j] = data;
                    isAdd = false;
                    break;
                }
            }

            if(isAdd)
                list.Add(data);
        }

        //NetData.instance.GetUserInfo()._PassiveList.AddRange(list);
    }

    /// <summary> 패시브 렙업 요청 응답 </summary>
    private void PMsgRolePassiveSkillUpgradeSHandler(PMsgRolePassiveSkillUpgradeS pmsgRolePassiveSkillUpgradeS)
    {
        Debug.Log("pmsgRolePassiveSkillUpgradeS " + pmsgRolePassiveSkillUpgradeS);
        SceneManager.instance.EndNetProcess("PMsgRolePassiveSkillUpgradeC");
        uint ErrorCode = (uint)pmsgRolePassiveSkillUpgradeS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRolePassiveSkillUpgradeS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        //NetData.PassiveData passiveData = NetData.instance.GetUserInfo().GetPassiveData(pmsgRolePassiveSkillUpgradeS.UnSkillId);
        //passiveData.Level += 1;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_S, pmsgRolePassiveSkillUpgradeS.UnSkillId);
    }
    
    /// <summary> 패시브 장착 요청 응답 </summary>
    private void PMsgRolePassiveSkillUseSHandler(PMsgRolePassiveSkillUseS pmsgRolePassiveSkillUseS)
    {
        SceneManager.instance.EndNetProcess("PMsgRolePassiveSkillUseC");
        uint ErrorCode = (uint)pmsgRolePassiveSkillUseS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRolePassiveSkillUseS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_USE_S, pmsgRolePassiveSkillUseS.UnSkillId);
    }

    /// <summary> 신분 미해금 요청 응답 </summary>
    private void PMsgRoleIdentifyUnlockedListSHandler(PMsgRoleIdentifyUnlockedListS proto)
    {
        //SceneManager.instance.EndNetProcess("PMsgRoleIdentifyUnlockedListC");
        Debug.Log("PMsgRoleIdentifyUnlockedListS " + proto);
        List<NetData.StatusData> dataList = new List<NetData.StatusData>();
        for (int i=0; i < proto.UnCount; i++)
        {
            dataList.Add(new NetData.StatusData(false, false, proto.CIdentifyInfo[i].UnType, 1, proto.CIdentifyInfo[i].UnPoint, (uint)proto.CIdentifyInfo[i].UnType) );
        }

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCKED_LIST_C, dataList);
    }
    
    /// <summary> 패시브 일괄강화 </summary>
    private void PMsgRolePassiveSkillUpgradeTurboSHandler(PMsgRolePassiveSkillUpgradeTurboS pmsgRolePassiveSkillUpgradeTurboS)
    {
        SceneManager.instance.EndNetProcess("PMsgRolePassiveSkillUpgradeTurboC");
        Debug.Log("pmsgRolePassiveSkillUpgradeTurboS " + pmsgRolePassiveSkillUpgradeTurboS);

        //NetData.PassiveData passiveData = NetData.instance.GetUserInfo().GetPassiveData(pmsgRolePassiveSkillUpgradeTurboS.UnSkillId);
        //passiveData.Level = (uint)pmsgRolePassiveSkillUpgradeTurboS.UnTimes;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        if (basePanel != null)
            basePanel.NetworkData(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_TURBO_S, pmsgRolePassiveSkillUpgradeTurboS.UnSkillId);
    }
    #endregion

    void Update()
    {
        mNetworkAuth.Update();
        mNetworkGame.Update();
    }

    public string GetErrorString(uint ErrorCode)
    {
        return ((ErrorCode)ErrorCode).ToString();
    }

}
