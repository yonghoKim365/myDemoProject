using Core.Net;
using Sw;

namespace Protocol
{
    public class MsgTable
    {
        public static readonly MsgPool Pool = new MsgPool();

        public static void RegistAllMsg()
        {
            Pool.RegistS2CMsg<PMsgLoginS>(MSG_DEFINE._MSG_LOGIN_S);
            Pool.RegistS2CMsg<PMsgAccountCertifyS>(MSG_DEFINE._MSG_ACCOUNT_CERTIFY_S);
            Pool.RegistS2CMsg<PMsgGameLoginS>(MSG_DEFINE._MSG_GAME_USER_LOGIN_S);

            //TypeSDK관련 메세지들
            Pool.RegistS2CMsg<PMsgGoogleCertifyS>(MSG_DEFINE._MSG_GOOGLE_CERTIFY_S);
            Pool.RegistS2CMsg<PMsgFacebookCertifyS>(MSG_DEFINE._MSG_FACEBOOK_CERTIFY_S);

            Pool.RegistS2CMsg<PMsgUserBindGoogleS>(MSG_DEFINE._MSG_USER_BIND_GOOGLE_S);
            Pool.RegistS2CMsg<PMsgUserBindFacebookS>(MSG_DEFINE._MSG_USER_BIND_FACEBOOK_S);

            Pool.RegistS2CMsg<PMsgUserBindQueryFbGoogleS>(MSG_DEFINE._MSG_USER_BIND_QUERY_FB_GOOGLE_S);

            Pool.RegistS2CMsg<PMsgGameDisconnectS>(MSG_DEFINE._MSG_GAME_DISCONNECT_S);
            Pool.RegistS2CMsg<PMsgUserInfoS>(MSG_DEFINE._MSG_USER_USERINFO_S);
            Pool.RegistS2CMsg<PMsgTalkCS>(MSG_DEFINE._MSG_TALK_CS);
            Pool.RegistS2CMsg<PMsgTalkRecvS>(MSG_DEFINE._MSG_TALK_RECV_S);
            Pool.RegistS2CMsg<PMsgTalkBlackListS>(MSG_DEFINE._MSG_TALK_BLACK_LIST_S);
            Pool.RegistS2CMsg<PMsgTalkAddBlackCS>(MSG_DEFINE._MSG_TALK_ADD_BLACK_CS);
            Pool.RegistS2CMsg<PMsgTalkDelBlackCS>(MSG_DEFINE._MSG_TALK_DEL_BLACK_CS);
            Pool.RegistS2CMsg<PMsgPingCS>(MSG_DEFINE._MSG_USER_PING);
            Pool.RegistS2CMsg<PMsgServerListS>(MSG_DEFINE._MSG_SERVER_LIST_S);
            Pool.RegistS2CMsg<PMsgRoleListS>(MSG_DEFINE._MSG_ROLE_LIST_S);
            Pool.RegistS2CMsg<PMsgRoleCreateNewS>(MSG_DEFINE._MSG_ROLE_CREATE_NEW_S);
            Pool.RegistS2CMsg<PMsgRoleDeleteS>(MSG_DEFINE._MSG_ROLE_DELETE_S);
            Pool.RegistS2CMsg<PMsgRoleSelectS>(MSG_DEFINE._MSG_ROLE_SELECT_S);
            Pool.RegistS2CMsg<PMsgRoleInfoS>(MSG_DEFINE._MSG_ROLE_INFO_S);
            Pool.RegistS2CMsg<PMsgMapEnterMapS>(MSG_DEFINE._MSG_MAP_ENTER_MAP_S);
            Pool.RegistS2CMsg<PMsgMapLeaveMapS>(MSG_DEFINE._MSG_MAP_LEAVE_MAP_S);
            Pool.RegistS2CMsg<PMsgMapMoveCS>(MSG_DEFINE._MSG_MAP_MOVE_CS);
            Pool.RegistS2CMsg<PMsgMapMoveRecvS>(MSG_DEFINE._MSG_MAP_MOVE_REVC_S);
            Pool.RegistS2CMsg<PMsgMapKickS>(MSG_DEFINE._MSG_MAP_KICK_S);
            Pool.RegistS2CMsg<PMsgNpcInfoS>(MSG_DEFINE._MSG_NPC_INFO_S);
            Pool.RegistS2CMsg<PMsgMapRoleInfoS>(MSG_DEFINE._MSG_MAP_ROLE_INFO_S);
            Pool.RegistS2CMsg<PMsgEquipmentQueryS>(MSG_DEFINE._MSG_EQUIPMENT_QUERY_S);
            Pool.RegistS2CMsg<PMsgItemQueryS>(MSG_DEFINE._MSG_ITEM_QUERY_S);
            Pool.RegistS2CMsg<PMsgEquipmentDelS>(MSG_DEFINE._MSG_EQUIPMENT_DEL_S);
            Pool.RegistS2CMsg<PMsgEquipmentUserS>(MSG_DEFINE._MSG_EQUIPMENT_USE_S);
            Pool.RegistS2CMsg<PMsgItemDelS>(MSG_DEFINE._MSG_ITEM_DEL_S);
            Pool.RegistS2CMsg<PMsgCostumeQueryS>(MSG_DEFINE._MSG_COSTUME_QUERY_S);
            Pool.RegistS2CMsg<PMsgEquipmentEvolveS>(MSG_DEFINE._MSG_EQUIPMENT_EVOLVE_S);
            Pool.RegistS2CMsg<PMsgRoleAttributeS>(MSG_DEFINE._MSG_ROLE_ATTRIBUTE_S);
            Pool.RegistS2CMsg<PMsgEquipmentEnchantS>(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_S);
            Pool.RegistS2CMsg<PMsgEquipmentBreakS>(MSG_DEFINE._MSG_EQUIPMENT_BREAK_S);
            Pool.RegistS2CMsg<PMsgItemFusionS>(MSG_DEFINE._MSG_ITEM_FUSION_S);
            Pool.RegistS2CMsg<PMsgCostumeFusionS>(MSG_DEFINE._MSG_COSTUME_FUSION_S);
            Pool.RegistS2CMsg<PMsgCostumeEvolveS>(MSG_DEFINE._MSG_COSTUME_EVOLVE_S);
            Pool.RegistS2CMsg<PMsgCostumeUserS>(MSG_DEFINE._MSG_COSTUME_USE_S);
            Pool.RegistS2CMsg<PMsgCostumeTokenS>(MSG_DEFINE._MSG_COSTUME_TOKEN_S);
            Pool.RegistS2CMsg<PMsgCostumeSkillUpgradeS>(MSG_DEFINE._MSG_COSTUME_SKILL_UPGRADE_S);
            Pool.RegistS2CMsg<PMsgEquipmentSellS>(MSG_DEFINE._MSG_EQUIPMENT_SELL_S);
            Pool.RegistS2CMsg<PMsgItemSellS>(MSG_DEFINE._MSG_ITEM_SELL_S);
            Pool.RegistS2CMsg<PMsgDailyTimeS>(MSG_DEFINE._MSG_DAILY_TIME_S);
            Pool.RegistS2CMsg<PMsgPowerTimeS>(MSG_DEFINE._MSG_POWER_TIME_S);
            Pool.RegistS2CMsg<PMsgStageQueryS>(MSG_DEFINE._MSG_STAGE_QUERY_S);
            Pool.RegistS2CMsg<PMsgStageStartS>(MSG_DEFINE._MSG_STAGE_START_S);
            Pool.RegistS2CMsg<PMsgStageCompleteS>(MSG_DEFINE._MSG_STAGE_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgSynNewItemS>(MSG_DEFINE._MSG_ITEM_SYN_NEW_ITEM_S);
            

            //카드 뒤집기
            //Pool.RegistS2CMsg<PMsgStageFlopS>(MSG_DEFINE._MSG_STAGE_FLOP_S);

            //던전 소탕
            Pool.RegistS2CMsg<PMsgStageSweepS>(MSG_DEFINE._MSG_STAGE_SWEEP_S);
            Pool.RegistS2CMsg<PMsgStageSweepResultS>(MSG_DEFINE._MSG_STAGE_SWEEP_RESULT_S);

            // 메일
            Pool.RegistS2CMsg<PMsgEmailQueryListS>(MSG_DEFINE._MSG_EMAIL_QUERY_LIST_S);
            Pool.RegistS2CMsg<PMsgEmailReadDetailS>(MSG_DEFINE._MSG_EMAIL_READ_DETAIL_S);
            Pool.RegistS2CMsg<PMsgEmailFeatchS>(MSG_DEFINE._MSG_EMAIL_FETCH_S);
            Pool.RegistS2CMsg<PMsgEmailOnKeyFeatchS>(MSG_DEFINE._MSG_EMAIL_ONEKEY_FEATCH_S);
            Pool.RegistS2CMsg<PMsgEmailDelS>(MSG_DEFINE._MSG_EMAIL_DEL_S);
            Pool.RegistS2CMsg<PMsgEmailOnKeyDelS>(MSG_DEFINE._MSG_EMAIL_ONEKEY_DEL_S);
            Pool.RegistS2CMsg<PMsgReturnMainMapS>(MSG_DEFINE._MSG_RETURN_MAIN_MAP_S);

            //파트너
            Pool.RegistS2CMsg<PMsgHeroQueryS>(MSG_DEFINE._MSG_HERO_QUERY_S);
            Pool.RegistS2CMsg<PMsgHeroFusionS>(MSG_DEFINE._MSG_HERO_FUSION_S);
            Pool.RegistS2CMsg<PMsgHeroUseExpItemS>(MSG_DEFINE._MSG_HERO_USE_EXP_ITEM_S);
            Pool.RegistS2CMsg<PMsgHeroEvolveS>(MSG_DEFINE._MSG_HERO_EVOLVE_S);
            //Pool.RegistS2CMsg<PMsgHeroEnchantS>(MSG_DEFINE._MSG_HERO_ENCHANT_S);
            Pool.RegistS2CMsg<PMsgHeroSkillUpgradeS>(MSG_DEFINE._MSG_HERO_SKILL_UPGRADE_S);
            //Pool.RegistS2CMsg<PMsgHeroSkillBuyS>(MSG_DEFINE._MSG_HERO_SKILL_BUY_S);
            //Pool.RegistS2CMsg<PMsgHeroSkillRefreshS>(MSG_DEFINE._MSG_HERO_SKILL_REFRESH_S);
            //Pool.RegistS2CMsg<PMsgHeroQualityUpgradeS>(MSG_DEFINE._MSG_HERO_QUALITY_UPGRADE_S);

            // 친구
            Pool.RegistS2CMsg<PMsgFriendQueryListS>(MSG_DEFINE._MSG_FRIEND_QUERY_LIST_S);
            //Pool.RegistS2CMsg<PMsgFriendFullInfoS>(MSG_DEFINE._MSG_FRIEND_FULL_INFO_S);
            Pool.RegistS2CMsg<PMsgFriendSearchS>(MSG_DEFINE._MSG_FRIEND_SEARCH_S);
            Pool.RegistS2CMsg<PMsgFriendRecommendListS>(MSG_DEFINE._MSG_FRIEND_RECOMMEND_LIST_S);
            Pool.RegistS2CMsg<PMsgFriendAddS>(MSG_DEFINE._MSG_FRIEND_ADD_S);
            Pool.RegistS2CMsg<PMsgFriendNotifyAddS>(MSG_DEFINE._MSG_FRIEND_NOTIFY_ADD_S);
            Pool.RegistS2CMsg<PMsgFriendCancleAddS>(MSG_DEFINE._MSG_FRIEND_CANCLE_ADD_S);
            Pool.RegistS2CMsg<PMsgFriendNotifyCancleAddS>(MSG_DEFINE._MSG_FRIEND_NOTIFY_CANCLE_ADD_S);
            Pool.RegistS2CMsg<PMsgFriendRequestFriendListS>(MSG_DEFINE._MSG_FRIEND_REQUEST_FRIEND_LIST_S);
            Pool.RegistS2CMsg<PMsgFriendSelfRequestFriendListS>(MSG_DEFINE._MSG_FRIEND_SELF_REQUEST_FRIEND_LIST_S);
            Pool.RegistS2CMsg<PMsgFriendApplicantS>(MSG_DEFINE._MSG_FRIEND_APPLICANT_S);
            Pool.RegistS2CMsg<PMsgFriendNotifyApplicantS>(MSG_DEFINE._MSG_FRIEND_NOTIFY_APPLICANT_S);
            Pool.RegistS2CMsg<PMsgFriendDelFriendS>(MSG_DEFINE._MSG_FRIEND_DEL_FRIEND_S);
            Pool.RegistS2CMsg<PMsgFriendGivePowerS>(MSG_DEFINE._MSG_FRIEND_GIVE_POWER_S);

            //경험치&골드던전
            Pool.RegistS2CMsg<PMsgExpBattleQueryS>(MSG_DEFINE._MSG_EXP_BATTLE_QUERY_S);
            Pool.RegistS2CMsg<PMsgExpBattleStartS>(MSG_DEFINE._MSG_EXP_BATTLE_START_S);
            Pool.RegistS2CMsg<PMsgExpBattleCompleteS>(MSG_DEFINE._MSG_EXP_BATTLE_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgCoinBattleQueryS>(MSG_DEFINE._MSG_COIN_BATTLE_QUERY_S);
            Pool.RegistS2CMsg<PMsgCoinBattleStartS>(MSG_DEFINE._MSG_COIN_BATTLE_START_S);
            Pool.RegistS2CMsg<PMsgCoinBattleCompleteS>(MSG_DEFINE._MSG_COIN_BATTLE_COMPLETE_S);

            //치트용으로 놓은 스테미너 충전
            Pool.RegistS2CMsg<PMsgBuyPowerS>(MSG_DEFINE._MSG_BUY_POWER_S);

            //마계의탑
            Pool.RegistS2CMsg<PMsgTowerBattleQueryS>(MSG_DEFINE._MSG_TOWER_BATTLE_QUERY_S);
            Pool.RegistS2CMsg<PMsgTowerBattleStartS>(MSG_DEFINE._MSG_TOWER_BATTLE_START_S);
            Pool.RegistS2CMsg<PMsgTowerBattleCompleteS>(MSG_DEFINE._MSG_TOWER_BATTLE_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgTowerRankQueryS>(MSG_DEFINE._MSG_TOWER_RANK_QUERY_S);
            Pool.RegistS2CMsg<PMsgTowerUseTimeQueryS>(MSG_DEFINE._MSG_TOWER_USE_TIME_QUERY_S);

            //보스레이드
            Pool.RegistS2CMsg<PMsgBossBattleQueryS>(MSG_DEFINE._MSG_BOSS_BATTLE_QUERY_S);
            Pool.RegistS2CMsg<PMsgBossBattleStartS>(MSG_DEFINE._MSG_BOSS_BATTLE_START_S);
            Pool.RegistS2CMsg<PMsgBossBattleCompleteS>(MSG_DEFINE._MSG_BOSS_BATTLE_COMPLETE_S);

            //상점
            Pool.RegistS2CMsg<PMsgShopInfoQueryS>(MSG_DEFINE._MSG_SHOP_INFO_QUERY_S);
            Pool.RegistS2CMsg<PMsgShopByItemS>(MSG_DEFINE._MSG_SHOP_BY_ITEM_S);
            Pool.RegistS2CMsg<PMsgShopRefreshS>(MSG_DEFINE._MSG_SHOP_Refresh_S);

            //vip
            Pool.RegistS2CMsg<PMsgVipQueryInfoS>(MSG_DEFINE._MSG_VIP_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgVipFetchSweepTicketS>(MSG_DEFINE._MSG_VIP_FETCH_SWEEP_TICKET_S);
            Pool.RegistS2CMsg<PMsgVipRepairSignInS>(MSG_DEFINE._MSG_VIP_REPAIR_SIGN_IN_S);

            //퀘스트
            Pool.RegistS2CMsg<PMsgTaskQueryInfoS>(MSG_DEFINE._MSG_TASK_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgTaskReceiveTaskS>(MSG_DEFINE._MSG_TASK_RECEIVE_TASK_S);
            Pool.RegistS2CMsg<PMsgTaskCompleteS>(MSG_DEFINE._MSG_TASK_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgTaskFetchBonusS>(MSG_DEFINE._MSG_TASK_FETCH_BONUS_S);
            Pool.RegistS2CMsg<PMsgDailyTaskQueryInfoS>(MSG_DEFINE._MSG_DAILY_TASK_QUERY_INFO_S);

            //랭킹
            Pool.RegistS2CMsg<PMsgRankQueryS>(MSG_DEFINE._MSG_RANK_QUERY_S);
            Pool.RegistS2CMsg<PMsgRankGuildQueryS>(MSG_DEFINE._MSG_RANK_GUILD_QUERY_S);

            //뽑기
            Pool.RegistS2CMsg<PMsgLotteryQueryInfoS>(MSG_DEFINE._MSG_LOTTERY_BOX_QUERYINFO_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxCommonFreeS>(MSG_DEFINE._MSG_LOTTERY_BOX_COMMONFREE_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxCommonS>(MSG_DEFINE._MSG_LOTTERY_BOX_COMMON_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxCommonManytimesS>(MSG_DEFINE._MSG_LOTTERY_BOX_COMMON_MANYTIMES_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxSeniorFreeS>(MSG_DEFINE._MSG_LOTTERY_BOX_SENIORFREE_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxSeniorS>(MSG_DEFINE._MSG_LOTTERY_BOX_SENIOR_S);
            Pool.RegistS2CMsg<PMsgLotteryBoxSeniorManytimesS>(MSG_DEFINE._MSG_LOTTERY_BOX_SENIOR_MANYTIMES_S);

            //난투장
            Pool.RegistS2CMsg<PMsgMessQueryS>(MSG_DEFINE._MSG_MESS_QUERY_S);
            //Pool.RegistS2CMsg<PMsgMessRoomQueryS>(MSG_DEFINE._MSG_MESS_ROOM_QUERY_S);
            Pool.RegistS2CMsg<PMsgMessRoomEnterS>(MSG_DEFINE._MSG_MESS_ROOM_ENTER_S);
            Pool.RegistS2CMsg<PMsgMessRoomLeaveS>(MSG_DEFINE._MSG_MESS_ROOM_LEAVE_S);
            Pool.RegistS2CMsg<PMsgBattleMapEnterMapS>(MSG_DEFINE._MSG_BATTLE_MAP_ENTER_MAP_S);
            Pool.RegistS2CMsg<PMsgBattleMapLeaveMapS>(MSG_DEFINE._MSG_BATTLE_MAP_LEAVE_MAP_S);
            Pool.RegistS2CMsg<PMsgBattleMapRoleInfoS>(MSG_DEFINE._MSG_BATTLE_MAP_ROLE_INFO_S);
            Pool.RegistS2CMsg<PMsgBattleMapMoveCS>(MSG_DEFINE._MSG_BATTLE_MAP_MOVE_CS);
            Pool.RegistS2CMsg<PMsgBattleMapMoveRecvS>(MSG_DEFINE._MSG_BATTLE_MAP_MOVE_REVC_S);
            Pool.RegistS2CMsg<PMsgBattleNpcInfoS>(MSG_DEFINE._MSG_BATTLE_NPC_INFO_S);
            Pool.RegistS2CMsg<PMsgRoleAttackS>(MSG_DEFINE._MSG_ROLE_ATTACK_S);
			Pool.RegistS2CMsg<PMsgMessSynNotifyBossBeginS>(MSG_DEFINE._MSG_MESS_SYN_NOTIFY_BOSS_BEGIN_S);
			Pool.RegistS2CMsg<PMsgMessSynNotifyBossEndS>(MSG_DEFINE._MSG_MESS_SYN_NOTIFY_BOSS_END_S);
			Pool.RegistS2CMsg<PMsgMessSynNotifyBossBeginOrEndS>(MSG_DEFINE._MSG_MESS_SYN_NOTIFY_BOSS_BEGIN_OR_END_S);

            Pool.RegistS2CMsg<PMsgBattleAttackPrepareS>(MSG_DEFINE._MSG_BATTLE_ATTACK_PREPARE_S);
            Pool.RegistS2CMsg<PMsgBattleAttackPrepareRecvS>(MSG_DEFINE._MSG_BATTLE_ATTACK_PREPARE_REVC_S);
            Pool.RegistS2CMsg<PMsgBattleMapKickS>(MSG_DEFINE._MSG_BATTLE_MAP_KICK_S);
            Pool.RegistS2CMsg<PMsgRoleAttackRecvS>(MSG_DEFINE._MSG_ROLE_ATTACK_REVC_S);
            Pool.RegistS2CMsg<PMsgRoleDieRecvS>(MSG_DEFINE._MSG_ROLE_DIE_RECV_S);
            Pool.RegistS2CMsg<PMsgRoleReliveRecvS>(MSG_DEFINE._MSG_ROLE_RELIVE_RECV_S);
            Pool.RegistS2CMsg<PMsgBattleMapFlyS>(MSG_DEFINE._MSG_BATTLE_MAP_FLY_S);
            Pool.RegistS2CMsg<PMsgMessDropS>(MSG_DEFINE._MSG_MESS_DROP_S);
            //Pool.RegistS2CMsg<PMsgMessChangeRoomS>(MSG_DEFINE._MSG_MESS_CHANGE_ROOM_S);
            Pool.RegistS2CMsg<PMsgMessBossInfoS>(MSG_DEFINE._MSG_MESS_BOSS_INFO_S);
            Pool.RegistS2CMsg<PMsgRoleReliveS>(MSG_DEFINE._MSG_ROLE_RELIVE_S);
            Pool.RegistS2CMsg<PMsgRoleSuperArmorRecoveryS>(MSG_DEFINE._MSG_ROLE_SUPER_ARMOR_RECOVERY_S);

            //난투장 발사체
            Pool.RegistS2CMsg<PMsgBattleProjectTileInfoS>(MSG_DEFINE._MSG_BATTLE_PROJECTTILE_INFO_S);
            Pool.RegistS2CMsg<PMsgBattleAddProjectTileS>(MSG_DEFINE._MSG_BATTLE_ADD_PROJECTTILE_S);
            Pool.RegistS2CMsg<PMsgBattleDelProjectTileS>(MSG_DEFINE._MSG_BATTLE_DEL_PROJECTTILE_S);

            //멀티플레이 버프
            Pool.RegistS2CMsg<PMsgAddBuffS>(MSG_DEFINE._MSG_BATTLE_ADD_BUFF_S);
            Pool.RegistS2CMsg<PMsgBuffAttackRecvS>(MSG_DEFINE._MSG_BUFF_ATTACK_REVC_S);

            //길드
            Pool.RegistS2CMsg<PMsgGuildCreateNewS>(MSG_DEFINE._MSG_GUILD_CREATE_NEW_S);
            Pool.RegistS2CMsg<PMsgGuildQueryBaseInfoS>(MSG_DEFINE._MSG_GUILD_QUERY_BASE_INFO_S);
            Pool.RegistS2CMsg<PMsgGuildQueryDetailedInfoS>(MSG_DEFINE._MSG_GUILD_QUERY_DETAILED_INFO_S);
            Pool.RegistS2CMsg<PMsgGuildMemberListS>(MSG_DEFINE._MSG_GUILD_MEMBER_LIST_S);
            Pool.RegistS2CMsg<PMsgGuildRecommendListS>(MSG_DEFINE._MSG_GUILD_RECOMMEND_LIST_S);
            Pool.RegistS2CMsg<PMsgGuildChangeIconS>(MSG_DEFINE._MSG_GUILD_CHANGE_ICON_S);
            Pool.RegistS2CMsg<PMsgGuildChangeNameDeclarationAnnouncementS>(MSG_DEFINE._MSG_GUILD_CHANGE_NAME_DECLARATION_ANNOUNCEMENT_S);
            Pool.RegistS2CMsg<PMsgGuildSearchGuildS>(MSG_DEFINE._MSG_GUILD_SEARCH_GUILD_S);
            Pool.RegistS2CMsg<PMsgGuildApplyGuildS>(MSG_DEFINE._MSG_GUILD_APPLY_GUILD_S);
            Pool.RegistS2CMsg<PMsgGuildQueryApplyListS>(MSG_DEFINE._MSG_GUILD_QUERY_APPLYLIST_S);
            Pool.RegistS2CMsg<PMsgGuildQueryGuildListS>(MSG_DEFINE._MSG_GUILD_QUERY_GUILDLIST_S);
            Pool.RegistS2CMsg<PMsgGuildExamineApplicantS>(MSG_DEFINE._MSG_GUILD_EXAMINE_APPLICANT_S);
            Pool.RegistS2CMsg<PMsgGuildSynExamineApplicantS>(MSG_DEFINE._MSG_GUILD_SYN_EXAMINE_APPLICANT_S);
            Pool.RegistS2CMsg<PMsgGuildUpgradeLevelS>(MSG_DEFINE._MSG_GUILD_UPGRADE_LEVEL_S);
            Pool.RegistS2CMsg<PMsgSetGuildJoinsetS>(MSG_DEFINE._MSG_GUILD_SET_JOINSET_GUILD_S);
            Pool.RegistS2CMsg<PMsgGuildGuildPrayS>(MSG_DEFINE._MSG_GUILD_GUILDPRAY_S);
            Pool.RegistS2CMsg<PMsgGuildGuildDonateS>(MSG_DEFINE._MSG_GUILD_GUILDDONATE_S);
            Pool.RegistS2CMsg<PMsgGuildAppointPositionS>(MSG_DEFINE._MSG_GUILD_APPOINT_POSITION_S);
            Pool.RegistS2CMsg<PMsgGuildKitkMemberS>(MSG_DEFINE._MSG_GUILD_KITK_MEMBER_S);
            Pool.RegistS2CMsg<PMsgGuildSynKitkMemberS>(MSG_DEFINE._MSG_GUILD_SYN_KITK_MEMBER_S);
            Pool.RegistS2CMsg<PMsgGuildAppointGuildLeaderS>(MSG_DEFINE._MSG_GUILD_APPOINT_GUILD_LEADER_S);
            Pool.RegistS2CMsg<PMsgGuildSecedeGuildS>(MSG_DEFINE._MSG_GUILD_SECEDE_GUILD_S);
            //Pool.RegistS2CMsg<PMsgGuildFullGoddsInfoOnBodyS>(MSG_DEFINE._MSG_GUILD_FULL_GODDSINFO_ON_BODY_S);
            Pool.RegistS2CMsg<PMsgGuildAttributeS>(MSG_DEFINE._MSG_GUILD_ATTRIBUTE_UPDATE_S);
            Pool.RegistS2CMsg<PMsgGuildQueryGuildStatusS>(MSG_DEFINE._MSG_GUILD_QUERY_GUILD_STATUS_S);
            Pool.RegistS2CMsg<PMsgGuildQuerySelfInfoS>(MSG_DEFINE._MSG_GUILD_QUERY_SELF_INFO_S);
            //Pool.RegistS2CMsg<PMsgGuildFullGoddsInfoOnBodyByApplicantS>(MSG_DEFINE._MSG_GUILD_FULL_GODDSINFO_ON_BODY_BY_APPLICANT_S);
            Pool.RegistS2CMsg<PMsgGuildNameQueryS>(MSG_DEFINE._MSG_GUILD_NAME_QUERY_S);
            Pool.RegistS2CMsg<PMsgGuildIDQueryS>(MSG_DEFINE._MSG_GUILD_ID_QUERY_S); 

            //스테이지 별 보상
            Pool.RegistS2CMsg<PMsgStageChapterQueryS>(MSG_DEFINE._MSG_STAGE_CHAPTER_QUERY_S);
            Pool.RegistS2CMsg<PMsgStageChapterRewardS>(MSG_DEFINE._MSG_STAGE_CHAPTER_REWARD_S);

            //코스튬 보기, 감추기
            Pool.RegistS2CMsg<PMsgCostumeShowFlagS>(MSG_DEFINE._MSG_COSTUME_SHOW_FLAG_S);

            //칭호
            Pool.RegistS2CMsg<PMsgTitleQueryInfoS>(MSG_DEFINE._MSG_TITLE_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgUseTitleS>(MSG_DEFINE._MSG_TITLE_USE_TITLE_S);
            Pool.RegistS2CMsg<PMsgSynNewTitleS>(MSG_DEFINE._MSG_TITLE_SYN_NEW_TITLE_S);

            //출첵
            Pool.RegistS2CMsg<PMsgSignInQueryInfoS>(MSG_DEFINE._MSG_SIGNIN_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgSignInS>(MSG_DEFINE._MSG_SIGNIN_SIGNIN_S);
            Pool.RegistS2CMsg<PMsgFillInSignInS>(MSG_DEFINE._MSG_SIGNIN_FILL_IN_SIGNIN_S);

            //업적
            Pool.RegistS2CMsg<PMsgAchieveQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_QUERY_INFO_S);
            //Pool.RegistS2CMsg<PMsgAchieveEquipTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_EQUIP_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveFightTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_FIGHT_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveFriendTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_FRIEND_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveMoneyTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_MONEY_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchievePlayTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_PLAY_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveRoleTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_ROLE_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveVipTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_VIP_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveFetchAwardS>(MSG_DEFINE._MSG_ACHIEVE_FETCH_AWARD_S);
            Pool.RegistS2CMsg<PMsgAchieveFetchPointsAwardS>(MSG_DEFINE._MSG_ACHIEVE_FETCH_POINTS_AWARD_S);
            Pool.RegistS2CMsg<PMsgAchieveSynPointsTotalValueS>(MSG_DEFINE._MSG_ACHIEVE_SYN_POINTS_TOTAL_VALUE_S);
            Pool.RegistS2CMsg<PMsgAchieveSynAchieveStatisValueS>(MSG_DEFINE._MSG_ACHIEVE_SYN_ACHIEVE_STATIS_VALUE_S);
            Pool.RegistS2CMsg<PMsgAchieveSynAchieveCompleteS>(MSG_DEFINE._MSG_ACHIEVE_SYN_ACHIEVE_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgAchieveOneKeyFetchAchieveAwardS>(MSG_DEFINE._MSG_ACHIEVE_ONEKEY_FETCH_ACHIEVE_AWARD_S);
            Pool.RegistS2CMsg<PMsgAchieveSynFightDataTotalValueS>(MSG_DEFINE._MSG_ACHIEVE_SYN_FIGHTDATA_TOTAL_VALUE_S);

            //콜로세움
            Pool.RegistS2CMsg<PMsgColosseumQueryS>(MSG_DEFINE._MSG_COLOSSEUM_QUERY_S);
            Pool.RegistS2CMsg<PMsgColosseumCreateRoomS>(MSG_DEFINE._MSG_COLOSSEUM_CREATE_ROOM_S);
            Pool.RegistS2CMsg<PMsgColosseumInviteS>(MSG_DEFINE._MSG_COLOSSEUM_INVITE_S);
            Pool.RegistS2CMsg<PMsgColosseumInviteRecvS>(MSG_DEFINE._MSG_COLOSSEUM_INVITE_RECV_S);
            Pool.RegistS2CMsg<PMsgColosseumRoomListS>(MSG_DEFINE._MSG_COLOSSEUM_ROOM_INFO_S);
            Pool.RegistS2CMsg<PMsgColosseumRoomInfoS>(MSG_DEFINE._MSG_COLOSSEUM_ROOM_INFO_S);
            Pool.RegistS2CMsg<PMsgColosseumEnterRoomS>(MSG_DEFINE._MSG_COLOSSEUM_ENTER_ROOM_S);
            Pool.RegistS2CMsg<PMsgColosseumEnterRoomRecvS>(MSG_DEFINE._MSG_COLOSSEUM_ENTER_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgColosseumLeaveRoomS>(MSG_DEFINE._MSG_COLOSSEUM_LEAVE_ROOM_S);
            Pool.RegistS2CMsg<PMsgColosseumLeaveRoomRecvS>(MSG_DEFINE._MSG_COLOSSEUM_LEAVE_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgColosseumKickRoomS>(MSG_DEFINE._MSG_COLOSSEUM_KICK_ROOM_S);
            Pool.RegistS2CMsg<PMsgColosseumKickRoomRecvS>(MSG_DEFINE._MSG_COLOSSEUM_KICK_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgColosseumStartS>(MSG_DEFINE._MSG_COLOSSEUM_START_S);
            Pool.RegistS2CMsg<PMsgColosseumCompleteS>(MSG_DEFINE._MSG_COLOSSEUM_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgColosseumSweepS>(MSG_DEFINE._MSG_COLOSSEUM_SWEEP_S);
            Pool.RegistS2CMsg<PMsgColosseumSweepResultS>(MSG_DEFINE._MSG_COLOSSEUM_SWEEP_RESULT_S);

            // 로그인유지보상
            Pool.RegistS2CMsg<PMsgWelfareOnlineQueryInfoS>(MSG_DEFINE._MSG_WELFAREONLINE_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgWelfareOnlineFetchRewardS>(MSG_DEFINE._MSG_WELFAREONLINE_FETCH_REWARD_S);
            Pool.RegistS2CMsg<PMsgWelfareOnlineSynCanFetchS>(MSG_DEFINE._MSG_WELFAREONLINE_SYN_CAN_FETCH_S);
            //7일연속보상
            Pool.RegistS2CMsg<PMsgWelfareXDayLoginQueryInfoS>(MSG_DEFINE._MSG_WELFARE_XDAY_LOGIN_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgWelfareFetchXDayRewardS>(MSG_DEFINE._MSG_WELFARE_FETCH_XDAY_REWARD_S);
            //복귀보상
            Pool.RegistS2CMsg<PMsgWelfareReturnQueryInfoS>(MSG_DEFINE._MSG_WELFARE_RETURN_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgWelfareReturnFetchRewardS>(MSG_DEFINE._MSG_WELFARE_RETURN_FETCH_REWARD_S);
            //신썹보상
            Pool.RegistS2CMsg<PMsgWelfareOpenSvrQueryInfoS>(MSG_DEFINE._MSG_WELFARE_OPEN_SVR_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgWelfareOpenSvrFetchRewardS>(MSG_DEFINE._MSG_WELFARE_OPEN_SVR_FETCH_REWARD_S);
            //랩업 패키지보상 
            Pool.RegistS2CMsg<PMsgWelfareRoleUpgradeQueryInfoS>(MSG_DEFINE._MSG_WELFARE_ROLE_UPGRADE_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgWelfareFetchRoleUpgradeRewardS>(MSG_DEFINE._MSG_WELFARE_FETCH_ROLE_UPGRADE_REWARD_S);
            //누적충전 보상
            Pool.RegistS2CMsg<PMsgRechargeTotalQueryInfoS>(MSG_DEFINE._MSG_RECHARGE_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgRechargeTotalFetchRewardS>(MSG_DEFINE._MSG_RECHARGE_TOTAL_FETCH_REWARD_S);
            Pool.RegistS2CMsg<PMsgRechargeTotalSynCanFetchS>(MSG_DEFINE._MSG_RECHARGE_TOTAL_SYN_CAN_FETCH_S);
            //일간충전
            Pool.RegistS2CMsg<PMsgRechargeDailyQueryInfoS>(MSG_DEFINE._MSG_RECHARGE_DAILY_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgRechargeDailyFetchRewardS>(MSG_DEFINE._MSG_RECHARGE_DAILY_FETCH_REWARD_S);
            Pool.RegistS2CMsg<PMsgRechargeDailySynCanFetchS>(MSG_DEFINE._MSG_RECHARGE_DAILY_SYN_CAN_FETCH_S);
            //누적소비 보상
            Pool.RegistS2CMsg<PMsgRechargeConsumerQueryInfoS>(MSG_DEFINE._MSG_RECHARGE_CONSUMER_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgRechargeConsumerFetchRewardS>(MSG_DEFINE._MSG_RECHARGE_CONSUMER_FETCH_REWARD_S);
            Pool.RegistS2CMsg<PMsgRechargeConsumerSynCanFetchS>(MSG_DEFINE._MSG_RECHARGE_CONSUMER_SYN_CAN_FETCH_S);

            //멀티보스레이드
            Pool.RegistS2CMsg<PMsgMultiBossQueryS>(MSG_DEFINE._MSG_MULTI_BOSS_QUERY_S);
            Pool.RegistS2CMsg<PMsgMultiBossCreateRoomS>(MSG_DEFINE._MSG_MULTI_BOSS_CREATE_ROOM_S);
            Pool.RegistS2CMsg<PMsgMultiBossInviteS>(MSG_DEFINE._MSG_MULTI_BOSS_INVITE_S);
            Pool.RegistS2CMsg<PMsgMultiBossInviteRecvS>(MSG_DEFINE._MSG_MULTI_BOSS_INVITE_RECV_S);
            Pool.RegistS2CMsg<PMsgMultiBossRoomListS>(MSG_DEFINE._MSG_MULTI_BOSS_ROOM_LIST_S);
            Pool.RegistS2CMsg<PMsgMultiBossRoomInfoS>(MSG_DEFINE._MSG_MULTI_BOSS_ROOM_INFO_S);
            Pool.RegistS2CMsg<PMsgMultiBossEnterRoomS>(MSG_DEFINE._MSG_MULTI_BOSS_ENTER_ROOM_S);
            Pool.RegistS2CMsg<PMsgMultiBossEnterRoomRecvS>(MSG_DEFINE._MSG_MULTI_BOSS_ENTER_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgMultiBossLeaveRoomS>(MSG_DEFINE._MSG_MULTI_BOSS_LEAVE_ROOM_S);
            Pool.RegistS2CMsg<PMsgMultiBossLeaveRoomRecvS>(MSG_DEFINE._MSG_MULTI_BOSS_LEAVE_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgMultiBossKickRoomS>(MSG_DEFINE._MSG_MULTI_BOSS_KICK_ROOM_S);
            Pool.RegistS2CMsg<PMsgMultiBossKickRoomRecvS>(MSG_DEFINE._MSG_MULTI_BOSS_KICK_ROOM_RECV_S);
            Pool.RegistS2CMsg<PMsgMultiBossStartS>(MSG_DEFINE._MSG_MULTI_BOSS_START_S);
            Pool.RegistS2CMsg<PMsgMultiBossCompleteS>(MSG_DEFINE._MSG_MULTI_BOSS_COMPLETE_S);

            //길드퀘스트
            Pool.RegistS2CMsg<PMsgGuildTaskQueryInfoS>(MSG_DEFINE._MSG_GUILD_TASK_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgGuildTaskFetchBonusS>(MSG_DEFINE._MSG_GUILD_TASK_FETCH_BONUS_S);
            Pool.RegistS2CMsg<PMsgGuildTaskSynTaskCompleteS>(MSG_DEFINE._MSG_GUILD_TASK_SYN_TASK_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgFetchRewardGuildUserTaskQueryInfoS>(MSG_DEFINE._MSG_FETCHREWARD_GUILD_TASK_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgGuildUserTaskQueryInfoS>(MSG_DEFINE._MSG_GUILD_USER_TASK_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgGuildUserTaskFetchBonusS>(MSG_DEFINE._MSG_GUILD_USER_TASK_FETCH_BONUS_S);
            Pool.RegistS2CMsg<PMsgGuildUserTaskAllocatingTaskS>(MSG_DEFINE._MSG_GUILD_USER_TASK_ALLOCATINGTASK_S);
            Pool.RegistS2CMsg<PMsgGuildUserTaskSynCompleteS>(MSG_DEFINE._MSG_GUILD_USER_TASK_SYN_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgQueryRoleInfoS>(MSG_DEFINE._MSG_QUERY_ROLE_INFO_S);
            //Pool.RegistS2CMsg<PMsgMessChangeRoomS>(MSG_DEFINE._MSG_MESS_CHANGE_ROOM_S);

            //실명인증
            Pool.RegistS2CMsg<PMsgUserCertifyInfoS>(MSG_DEFINE._MSG_USER_CERTIFY_INFO_S);
            Pool.RegistS2CMsg<PMsgUserCertifySetS>(MSG_DEFINE._MSG_USER_CERTIFY_SET_S);

            ////활동량
            //Pool.RegistS2CMsg<PMsgActivePointsQueryInfoS>(MSG_DEFINE._MSG_ACTIVE_POINTS_QUERY_INFO_S);
            //Pool.RegistS2CMsg<PMsgActivePointsFetchRewardS>(MSG_DEFINE._MSG_ACTIVE_POINTS_FETCHREWARD_S);
            //Pool.RegistS2CMsg<PMsgActivePointsTotalCountQueryInfoS>(MSG_DEFINE._MSG_ACTIVE_POINTS_TOTALCOUNT_QUERY_INFO_S);

            //1:1 차관
            Pool.RegistS2CMsg<PMsgArenaInfoS>(MSG_DEFINE._MSG_ARENA_INFO_S);
            Pool.RegistS2CMsg<PMsgArenaFightListS>(MSG_DEFINE._MSG_ARENA_FIGHT_LIST_S);
            Pool.RegistS2CMsg<PMsgArenaFightResultNoticeS>(MSG_DEFINE._MSG_ARENA_FIGHT_RESULT_NOTICE_S);
            Pool.RegistS2CMsg<PMsgArenaResetTimesS>(MSG_DEFINE._MSG_ARENA_RESET_TIMES_S);
            Pool.RegistS2CMsg<PMsgArenaRankInfoS>(MSG_DEFINE._MSG_ARENA_RANK_INFO_S);
            Pool.RegistS2CMsg<PMsgArenaRankListS>(MSG_DEFINE._MSG_ARENA_RANK_LIST_S);
            Pool.RegistS2CMsg<PMsgArenaMatchListS>(MSG_DEFINE._MSG_ARENA_MATCH_LIST_S);
            Pool.RegistS2CMsg<PMsgArenaMatchInfoS>(MSG_DEFINE._MSG_ARENA_MATCH_INFO_S);
            Pool.RegistS2CMsg<PMsgArenaFightStartS>(MSG_DEFINE._MSG_ARENA_FIGHT_START_S);
            Pool.RegistS2CMsg<PMsgArenaFightLeaveS>(MSG_DEFINE._MSG_ARENA_FIGHT_LEAVE_S);
            Pool.RegistS2CMsg<PMsgArenaFightCompleteS>(MSG_DEFINE._MSG_ARENA_FIGHT_COMPLETE_S);


            // bulletin, notice
            Pool.RegistS2CMsg<PMsgBulletinSynNewInfoS>(MSG_DEFINE._MSG_NEW_SYN_BULLETIN_S);

            //자랑하기
            Pool.RegistS2CMsg<PMsgSynDynamicBulletinNewEquipmentS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_NEW_EQUIPMENT_S);
            Pool.RegistS2CMsg<PMsgSynDynamicBulletinEnchantEquipmentS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_ENCHANT_EQUIPMENT_S);
            Pool.RegistS2CMsg<PMsgSynDynamicBulletinEvolveEquipmentS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_EVOLVE_EQUIPMENT_S);
            Pool.RegistS2CMsg<PMsgSynDynamicBulletinNewHeroS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_NEW_HERO_S);
            //Pool.RegistS2CMsg<PMsgSynDynamicBulletinEnchantHeroS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_ENCHANT_HERO_S);
            Pool.RegistS2CMsg<PMsgSynDynamicBulletinEvolveHeroS>(MSG_DEFINE._MSG_SYN_DYNAMIC_BULLETIN_EVOLVE_HERO_S);

            //신규 장비
            Pool.RegistS2CMsg<PMsgEquipmentSetQueryS>(MSG_DEFINE._MSG_EQUIPMENT_SET_QUERY_S);
            Pool.RegistS2CMsg<PMsgEquipmentSetChangeS>(MSG_DEFINE._MSG_EQUIPMENT_SET_CHANGE_S);
            Pool.RegistS2CMsg<PMsgEquipmentSetSelectS>(MSG_DEFINE._MSG_EQUIPMENT_SET_SELECT_S);
            Pool.RegistS2CMsg<PMsgEquipmentEnchantTurboS>(MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_TURBO_S);

            //일일업적
            Pool.RegistS2CMsg<PMsgAchieveDailyQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyFightTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_FIGHT_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyFriendTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_FRIEND_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyMoneyTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_MONEY_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyPlayTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_PLAY_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyRoleTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_ROLE_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyVipTotalQueryInfoS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_VIP_TOTAL_QUERY_INFO_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyFetchAwardS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_FETCH_AWARD_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyFetchPointsAwardS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_FETCH_POINTS_AWARD_S);
            Pool.RegistS2CMsg<PMsgAchieveDailySynPointsTotalValueS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_SYN_POINTS_TOTAL_VALUE_S);
            Pool.RegistS2CMsg<PMsgAchieveDailySynAchieveStatisValueS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_SYN_ACHIEVE_STATIS_VALUE_S);
            Pool.RegistS2CMsg<PMsgAchieveDailySynFightDataTotalValueS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_SYN_FIGHTDATA_TOTAL_VALUE_S);
            Pool.RegistS2CMsg<PMsgAchieveDailySynAchieveCompleteS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_SYN_ACHIEVE_COMPLETE_S);
            Pool.RegistS2CMsg<PMsgAchieveDailyOneKeyFetchAchieveAwardS>(MSG_DEFINE._MSG_ACHIEVE_DAILY_ONEKEY_FETCH_ACHIEVE_AWARD_S);

            //신분
            Pool.RegistS2CMsg<PMsgRoleIdentifyListS>(MSG_DEFINE._MSG_ROLE_IDENTIFY_LIST_S);
            Pool.RegistS2CMsg<PMsgRoleIdentifyUnlockS>(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCK_S);
            Pool.RegistS2CMsg<PMsgRoleIdentifyUpgradeS>(MSG_DEFINE._MSG_ROLE_IDENTIFY_UPGRADE_S);
            Pool.RegistS2CMsg<PMsgRoleIdentifyUseS>(MSG_DEFINE._MSG_ROLE_IDENTIFY_USE_S);
            Pool.RegistS2CMsg<PMsgRoleIdentifyUnlockedListS>(MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCKED_LIST_S);

            //스킬
            Pool.RegistS2CMsg<PMsgRoleActiveSkillListS>(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_LIST_S);
            Pool.RegistS2CMsg<PMsgRoleActiveSkillUpgradeS>(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_S);
            Pool.RegistS2CMsg<PMsgRoleActiveSkillUseS>(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_USE_S);
            Pool.RegistS2CMsg<PMsgRoleActiveSkillUpgradeTurboS>(MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_TURBO_S);
            //패시브
            Pool.RegistS2CMsg<PMsgRolePassiveSkillListS>(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_LIST_S);
            Pool.RegistS2CMsg<PMsgRolePassiveSkillUpgradeS>(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_S);
            Pool.RegistS2CMsg<PMsgRolePassiveSkillUseS>(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_USE_S);
            Pool.RegistS2CMsg<PMsgRolePassiveSkillUpgradeTurboS>(MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_TURBO_S);

        }
    }
}