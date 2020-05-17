//
//  ResponseBasic.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#ifndef CapcomWorld_ResponseBasic_h
#define CapcomWorld_ResponseBasic_h

#include "cocos2d.h"

using namespace cocos2d;

class ResponseBasic : public cocos2d::CCObject
{
    
public:
    const char* res;
    const char* msg;
};


class ResponseUpgradeInfo : public ResponseBasic
{
public:
    int attackPoint;
    int defensePoint;
    int questPoint;
    int remainPoint;    // 남은 upgrade point
};


class BattleUserInfo : public CCObject
{
public:
    long long userId;
    const char* nickName;
    int level;
    int numOfAppFriends;
    int ranking;
    const char* profileImgUrl;
    int leaderCard;
    int attack;
    int defense;
};


class ResponseOpponent : public ResponseBasic
{
    
public:
    CCArray *battleUserList;
};


class ResponseBattleInfo : public ResponseBasic
{
public:
    int battleResult; // 0,1,2 = win, lose, draw
    int reward_coin;
    int reward_fame;
    int used_battle_point;
    int opponent_card[5];
    
    const char* enemy_nick;
    int enemy_level;
    int enemy_appFriends;
    //int enemy_defense_pnt;
    int enemy_battle_pnt;
    int enemy_victory_cnt;
    //int enemy_draw_cnt;
    
    int attackerPoint;
    int attackerSkillPoint;
    int defenderPoint;
    int defenderSkillPoint;
    
    // for rival battle
    /*
    float rival_hp;
    float rival_hp_max;
    int rival_attack_point;
    int user_attack_point;
    */
    
    CCArray *skills; // 삭제예정 - rounds 에 포함되어있음
    
    int first_attack; // 1 == user, 2 == rival
    
    int user_max_hp;
    int user_remain_hp;    // 배틀이 끝나고 남아있는 hp, user와 rival중 한명은 0이다.
    
    int rival_max_hp;
    int rival_remain_hp;
    //int rival_origin_hp; // 배틀 시작전의 hp, 배틀 종료후에는 rival_remain_hp가 됨.
    
    int user_attack_tot;
    int user_friends_tot;
    int user_ext_tot;
    
    int rival_attack_tot;
    int rival_friends_tot;
    int rival_ext_tot;
    
    CCArray *rounds;
};

class OpponentSkillInfo : public CCObject
{
public:
    int cardID;
    int side; // 0 == attack, 1 == defense
    int skillID;
    int slot;
};

class ResponseNoticeInfo : public ResponseBasic
{
public:
    CCArray *notices;
    
};

class NoticeInfo : public CCObject
{
public:
    int type; // 0 == battle, 1 == trade
    int noticeID;
    int result; // 0 == win, 1 == lose, 2 == draw
    const char* nick;
    int date; // unix time stamp
    
};

class ResponseDetailNoticeInfo : public ResponseBasic
{
public:
    int battleResult; // 0,1,2 = win, lose, draw
    int reward_coin;
    int reward_fame;
    int opponent_card[5];
// opponent stat
    const char* enemy_nick;
    int enemy_level;
    int enemy_appFriends;
    int enemy_defense_pnt;
    int enemy_battle_pnt;
    int enemy_victory_cnt;
    int enemy_draw_cnt;
    
};

class ResponseQuestListInfo : public ResponseBasic
{
public:
    CCArray *questList; // QuestInfo Array
};

class AQuestInfo : public CCArray
{
public:
    int questID;
    int beginTime;
    int endTime;
    //int completeCnt;
    int progress;
    int progressMax;
    int enemy;
    int clear; // 1 == clear
    //int card1;
    //int card2;
    //int card3;
};
/*
<coin>xx</coin>
<exp>xx</exp>
<card>xx</card>
<items>x,x</items> <!-- 100% 수행하면 보상 두개. 하나로 묶음 -->
 */

class ResponseQuestUpdateResultInfo : public ResponseBasic
{
public:
    int progress;
    int progressMax;
    int coin;
    int exp;
    int bp;
    int sp;
    //int stamina;
    
    CCArray *cards;
    
    int slotIndex;
    int item1,item2;
    int user_exp;
    int user_exp_max;
    int user_lev;
    int user_uPnt;
    int user_coin;
    int user_revenge;
    int user_questPoint;
    int user_attackPoint;
    //int user_defensePoint;
    
    
    int completeCnt;
    
    int enemy_type; // 0 == none, 1== normal, 2== rival, 3==hrival, 4 == boss
    int enemy_code;
    int rid;
    int enemy_hp;         // remain hp
    int enemy_hp_max;     // remain hp max
    int enemy_level;
    int battle_limit_time;
    //int boss_hp_max;
    int boss_attack_point;
    int my_hp;
    int my_hp_max;
    int my_attack_point;
    
    //int need_money;
    
    int action; // 0 == trace, 1 == battle, 2 == avoid
    int enemy;  // 1 == normal,2 == boss,  none
    int questBattleResult;    // 1 == win, 2 == 0
    
    int user_max_hp;
    int user_remain_hp;    // 배틀이 끝나고 남아있는 hp, user와 rival중 한명은 0이다.
    
    int rival_max_hp;
    int rival_hp_before_battle; // 배틀 시작전의 hp, 배틀 종료후에는 rival_hp_after_battle가 됨
    int rival_hp_after_battle;
    
    int user_attack_tot;
    int user_friends_tot;
    int user_ext_tot;
    
    int rival_attack_tot;
    int rival_friends_tot;
    int rival_ext_tot;
    
    CCArray *rounds;
};

class ResponseRivalBattle : public ResponseBasic
{
public:
    //const char* battleResult;
    int battleResult; // 0 == lose, 1 == win, 2 == na
    int reward_coin;
    int reward_exp;
    CCArray *reward_cards;
    int used_battle_point;
    
    int user_max_hp;
    int user_remain_hp;    // 배틀이 끝나고 남아있는 hp, user와 rival중 한명은 0이다.
    
    int rival_origin_hp; // 배틀 시작전의 hp, 배틀 종료후에는 rival_remain_hp가 됨.
    int rival_remain_hp;
    int rival_max_hp;
    int rival_level;
    int battle_limit_time;
    
    int user_attack_tot;
    int user_friends_tot;
    int user_ext_tot;
    
    int rival_attack_tot;
    int rival_friends_tot;
    int rival_ext_tot;
    
    CCArray *rounds;

    int user_lev;
    int user_exp;
    int user_coin;
    int user_gold;
    int user_uPnt;
    int user_attackPoint;
    int user_defensePoint;
    int user_questPoint;
    int user_revenge;
    //    int user_fame;
};
class ABattleRound : public CCObject
{
public:
    int round_id;
    int attacker_hp;
    int attacker_friend;
    int attacker_ext;
    int attacker_heal;
    int attacker_point;
    
    int defender_hp;
    int defender_friend;
    int defender_ext;
    int defender_heal;
    int defender_point;
    
    CCArray *skills;
};

class QuestRewardCardInfo : public cocos2d::CCObject
{
public:
    int card_srl;
    int card_id;
    int card_level;
    int card_exp;
    int card_attack;
    int card_defense;
    int card_skillEffect;
    int card_skillLev;
};

class ResponseRefreshInfo : public ResponseBasic
{
public:
    int user_fame;
    int user_questPoint;
    int user_defensePoint;
    int user_attackPoint;
    int user_uPoint;
    int user_revenge;
    int user_level;
    int coin;
    int gold;
    int exp, max_exp;
    int notice;
    int battleCnt;
    int victoryCnt;
    int drawCnt;
    int user_ranking;
 
    
    
};

class ResponseCollectionInfo : public ResponseBasic
{
public:
    CCArray *cardlist; // AColloctionCard
};

class AColloctionCard : public CCObject
{
public:
    bool bOwn;
    int  cardId;
    int  series;
};

class ResponseFusionInfo : public ResponseBasic
{
public:
    int card_srl;
    int card_id;
    int card_level;
    int card_exp;
    int attack;
    int defense;
    int cost;
    
};

class ResponseTrainingInfo : public ResponseBasic
{
public:
    int card_srl;
    int card_id;
    int card_level;
    int card_exp;
    int attack;
    int defense;
    int skill_effect;
    int cost;
    
};

class ResponseItemInfo : public ResponseBasic
{
public:
    CCArray *itemList;
};

class ResponseGiftInfo : public ResponseBasic
{
public:
    CCArray *giftList;
};

class ResponseBuyInfo : public ResponseBasic
{
public:
    int cost_gold;
    int cost_coin;
    int item_count;
    int itemID;
    int user_stat_gold;
    int user_stat_coin;
    int user_stat_revenge;
    int user_stat_fame;
    int user_stat_q_pnt;
    //int user_stat_d_pnt;
    int user_stat_a_pnt;
    int user_stat_u_pnt;
    int user_stat_exp;
    int user_stat_lev;
};

class ResponseUseInfo : public ResponseBasic
{
public:
    int user_stat_fame;
    int user_stat_q_pnt;
    //int user_stat_d_pnt;
    int user_stat_a_pnt;
    int user_stat_u_pnt;
    int user_stat_coin;
    int user_stat_gold;
    int user_stat_exp;
    int user_stat_lev;
    int item_id;
    int item_count;
    
    CCArray *cards;
    
};

class AReceivedCard : public CCObject
{
public:
    int card_srl;
    int card_id;
    int card_lev;
    int card_exp;
    int card_attack;
    int card_defense;
    int card_skill_effect;
    int card_skill_lev;
};

class ResponseSellInfo : public ResponseBasic
{
public:
    int user_stat_revenge;
    int user_stat_fame;
    int user_stat_q_pnt;
    //int user_stat_d_pnt;
    int user_stat_a_pnt;
    int user_stat_u_pnt;
    int user_stat_gold;
    int user_stat_coin;
    int user_stat_exp;
    int user_stat_lev;
    int coin;
};

class ResponseTbInfo : public ResponseBasic
{
public:
    int user_stat_revenge;
    int user_stat_fame;
    int user_stat_q_pnt;
    //int user_stat_d_pnt;
    int user_stat_a_pnt;
    int user_stat_u_pnt;
    int user_stat_gold;
    int user_stat_coin;
    int user_stat_exp;
    int user_stat_lev;
    
    int card_exp;
    int card_lev;
    int card_skill_effect;
    int card_defense;
    int card_attack;
    int card_srl;
    int card_id;
    
    int coin;
    
};

class ResponseTutorial : public ResponseBasic
{
public:
    int tutorialProgress;
};

class ResponseMedal : public ResponseBasic
{
public:
    int medalCount;
    
};

class ResponseRoulette : public ResponseBasic
{
public:
    int coin;
    int card_skill_effect;
    int card_defense;
    int card_attack;
    int card_exp;
    int card_level;
    int card_id;
    int card_srl;
    int item_id;
    int item_count;
};

class ResponseEvent : public ResponseBasic
{
    
public:
    CCArray *eventList;
};

class ResponseSMexclude : public ResponseBasic
{
public:
    std::vector<long long> *userlist;
};

class ResponseTrace : public ResponseBasic
{
public:
    int traceResult;
    int coin;
};



class AReceivedRival : public CCObject
{
public:
    int type;   // 1 == rival, 2 == hidden
    int max_hp;
    int cur_hp;
    int npc_lv;
    int npc_id;
    int quest_id;
    int rid;    // rival id
    int birth;  //rival event birth time stamp
    int limit;  //rival event limit time stamp
    bool bRewardReceived;
    long long ownerUserID;
    CCArray* colleagues;
};

class AReceivedColleague : public CCObject
{
public:
    //const char* name;
    //const char* imgUrl;
    long long userid;
    int damages;
    int leaderCard_id;
    int user_lv;
};


class ResponseRivalList : public ResponseBasic
{
public:
    CCArray *rivals;
};

class ResponseRivalReward : public ResponseBasic
{
public:
    CCArray* rewardCards; // QuestRewardCardInfo
};



#endif
