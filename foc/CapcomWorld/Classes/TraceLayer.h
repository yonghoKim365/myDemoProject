//
//  TraceLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 13. 1. 30..
//
//

#ifndef __CapcomWorld__TraceLayer__
#define __CapcomWorld__TraceLayer__

#include <iostream>

#include "cocos2d.h"
//#include "cocos-ext.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "QuestFullScreen.h"
#include "QuestLayer.h"
#include "FileManager.h"
#include "CCHttpRequest.h"
//#include "PopUp.h"
#include "BattleFullScreen.h"
#include "NpcInfo.h"
#include "TraceResultLayer.h"
#include "LevelUpAction.h"
#include "CardDetailViewLayer.h"



using namespace cocos2d;
using namespace std;
using namespace CocosDenshion;

const float XP_GAUGE_LENGTH     = 513.0f;
const float XP_GAUGE_Y_POS      = 79.0f;
const float XP_SPLASH_Y_POS     = XP_GAUGE_Y_POS + 17.5f;
const int   STAMINA_GAUGE_X_POS = 15;
const float STAMINA_GAUGE_Y_POS = 30.0f;
const int   BATTLE_GAUGE_X_POS  = STAMINA_GAUGE_X_POS + 263;


const float XP_GAUGE_LENGTH2     = 513.0f;
const float HIT_GAUGE_LENGTH     = 496.0f;//XP_GAUGE_LENGTH2 - 38.0f;
const float HIT_GAUGE_BACKGROUND_X_POS = 10.0f;
const float HIT_GAUGE_BACKGROUND_Y_POS = 840;//780.0f;

// STAMINI_X_POS + 273

enum chaseStep
{
    CHASE_TAB = 0,
    CHASE_GO,
    CHASE_NORMAL_BATTLE,
    CHASE_RIVAL,
    CHASE_HIDDEN_RIVAL,
    CHASE_BOSS,
};



class EventObject : public cocos2d::CCObject
{
public:
    int eventCode;
    int coin;
    //int stamina;
    int sp,bp;
    int exp;
    CardInfo* card;
    
};

class TraceUILayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    TraceUILayer();//int y);
    ~TraceUILayer();
    
    CCSprite* xp[10];
    CCSprite* maxXp[10];
    void refreshXp(int yy);
    void refreshMaxXp(int yy);
    void removeXp();
    void removeMaxXp();
    
    CCSprite* xpGauge[2];
    void loadXpGauge();
    void refreshXpGauge(int yy);
    
    
    
    CCSprite* stamina[10];
    CCSprite* maxStamina[10];
    void refreshStamina(int yy);
    void refreshMaxStamina(int yy);
    void removeStamina();
    void removeMaxStamina();
    
    CCSprite* staminaGauge[3];
    void loadStaminaGauge();
    void refreshStaminaGauge(int yy);
    
    CCSprite* battlePoints[10];
    CCSprite* maxBattlePoints[10];
    void refreshBattlePoints(int yy);
    void refreshMaxBattlePoints(int yy);
    void removeBattlePoints();
    void removeMaxBattlePoints();
    
    CCSprite* battlePointsGauge[3];
    void loadBattlePointsGauge();
    void refreshBattlePointsGauge(int yy);
    
    CCSprite* level[5];
    void refreshLevel(int yy);
    void removeLevel();
    
    CCSprite* sprStageProgressRatio[5];
    void refreshStageProgressRatio(int xx, int yy);
    void removeStageProgressRatio(int progress);
    
    void Refresh();
    
private:
    CocosDenshion::SimpleAudioEngine* soundBG;
    bool bFinishedBattle;
};

class TraceNormalEnemyLayer : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    TraceNormalEnemyLayer(NpcInfo* npcInfo);
    ~TraceNormalEnemyLayer();
    
    NpcInfo* npcInfo;
    
    static TraceNormalEnemyLayer* getInstance();
    
    void InitUI();//void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
//    int npc_code;
//    const char* npc_imagePath;
//    const char* npc_desc;
    
    void InitNormalBattle1(int money);
    void InitNormalBattle2(int timeLimit);
    void battleTouch(CCPoint touchPos);
    
    CCArray* frameToHit;
    
    void normalBattleBtnCallback(CCObject* pSender);
    
    void battleTimeCounter();
    int normalBattleStep;
    
    float battleRemainTime;
    int requestedMoney;
    /*
    float battleLimitTime;
    float touchUp;
    */
    
    CCSprite* hitGauge[3];
    void loadHitGauge();
    void refreshHitGauge();
    CCSprite* pSprGaugeSparkle;
    
    void actionHitShake(const char* background, CCPoint location);
    void removeShake();
    void removeHit();
    
    
    
    void actionNormalKO();
    void callBackNormalKO();
    void callBackRemoveKo();
    
    void actionNormalTimeout();
    void callBackNormalTimeout();
    void callBackRemoveTimeOut();
    
    void cbFullGauge();
    
    CCSprite* spriteToHit;
    
private:
    const char* textAdjust(const char *input);
    static TraceNormalEnemyLayer* instance;
    
    bool bNormalEnenyKo;
};

class TraceHiddenRivalLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    /*
    static TraceHiddenRivalLayer* getInstance()
    {
        if (!instance)
            return NULL;
        return instance;
    }
    bool init()
    {
        if (!CCLayer::init())
            return false;
        instance = this;
        return true;
    }*/
    
    TraceHiddenRivalLayer(NpcInfo* npcInfo, int _rid, int _remainHp, int _MaxHp, int _rivalLevel, int _limitTime, int _questID=0);
    ~TraceHiddenRivalLayer();
    
    NpcInfo* npcInfo;
    
    static TraceHiddenRivalLayer* getInstance();
    
    void InitUI();//void* data);
    void timeCounter();
    
    void actionHiddenRival1();
    void actionHiddenRival2();
    void callBackHiddenRival();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void BtnCallback(CCObject* pSender);
    
    CCSprite* spriteToThunder;
    CCArray* frameToThunder;
    
    CCSprite* hiddenRivalHp[10];
    CCSprite* hiddenRivalMaxHp[10];
    void refreshHiddenRivalHp(int yy);
    void refreshHiddenRivalMaxHp(int yy);
    void removeHiddenRivalHp();
    void removeHiddenRivalMaxHp();
    CCSprite* hitGauge[3];  // HP 게이지
    void loadHitGauge();
    void refreshHitGauge();
    
    CCArray* frameToFire;
    CCSprite* spriteToHighFire;
    CCSprite* spriteToLowFire;
    CCArray* frameToSpotlight;
    CCSprite* spriteToSpotlight;
    
    //TraceHiddenRivalLayer() { }
    //~TraceHiddenRivalLayer()
    //{
    //    delete frameToThunder;
    //}
    
    void removeActionResouces();
    AReceivedRival* receivedRivalInfo;

    
private:
    static TraceHiddenRivalLayer* instance;
    const char* textAdjust(const char *input);
    
    int     rid;
    float   remainHp;
    float   maxHp;
    int     rivalLevel;
    int     limitTime;
    int     questID;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
};

class TraceRivalLayer : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    TraceRivalLayer(NpcInfo* npcInfo, int _rid, int _remainHp, int _MaxHp, int _rivalLevel, int _limitTime, int _questID=0);
    ~TraceRivalLayer();
    
    NpcInfo* npcInfo;
    
    static TraceRivalLayer* getInstance();
    
    
    void InitUI();//void* data);
    void timeCounter();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void actionRival1();
    void actionRival2();
    void callBackRival();
    
    void BtnCallback(CCObject* pSender);
    
    CCSprite* rivalHp[10];
    CCSprite* rivalMaxHp[10];
    void refreshRivalHp(int yy);
    void refreshRivalMaxHp(int yy);
    void removeRivalHp();
    void removeRivalMaxHp();
    CCSprite* hitGauge[3];  // HP 게이지
    void loadHitGauge();
    void refreshHitGauge();
    
    CCArray* frameToFire;
    CCSprite* spriteToHighFire;
    CCSprite* spriteToLowFire;
    CCArray* frameToSpotlight;
    CCSprite* spriteToSpotlight;
    
    void removeActionResouces();
    

    AReceivedRival* receivedRivalInfo;
    
    
    
private:
    static TraceRivalLayer* instance;
    const char* textAdjust(const char *input);
    int     rid;
    float   remainHp;
    float   maxHp;
    int     rivalLevel;
    int     limitTime;
    int     questID;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
};

class TraceBossLayer : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    TraceBossLayer(NpcInfo* npcInfo, int _questID, int _remainHp, int _maxHp, int _bossLv);
    ~TraceBossLayer();
    
    NpcInfo* npcInfo;
    
    static TraceBossLayer* getInstance();
    
    void InitUI();//void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
            
    void actionBoss();
    void callBackBoss();
    
    void BtnCallback(CCObject* pSender);
    
    CCSprite* bossHp[10];
    CCSprite* bossMaxHp[10];
    void refreshBossHp(int yy);
    void refreshBossMaxHp(int yy);
    void removeBossHp();
    void removeBossMaxHp();
    CCSprite* hitGauge[3];  // HP 게이지
    void loadHitGauge();
    void refreshHitGauge();
    
    void removeActionResouces();
    
    float curHp, maxHp;
    int   bossLevel;

private:
    int BOSS_HP_GAUGE_LENGTH;// = 475.0f;
    static TraceBossLayer* instance;
    const char* textAdjust(const char *input);
    int questID;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
};

class TraceLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    TraceLayer(int _questID, int _progress);//, const char* stageLabel);
    TraceLayer(int _rival_battle_id, NpcInfo* _npc, int rivalKind, AReceivedRival* rivalInfo); // rivalKind, 1 == rival, 2 == hidden
    ~TraceLayer();
    
    static TraceLayer* getInstance();
    
    
    void InitChaseUI();
    void InitUI1();
    void InitUI2();
    void InitNormalBattle1();
    void InitNormalBattle2();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void refreshStageCompleteRate();
    void actionZoomInOut(const char* background);
    void actionTab();
    void actionGo();
    void cbActionGo();
    void cbActionSurprised();
    void actionSurprised(const char* background);
    void cbActionEntry();
    
    void actionExpUp(int nCallBack, int val);
    void callBackExpUp(CCSprite* sprite);
    
    void actionGetCoin(int nCallBack , int val);
    void callBackCoin0();
    //void callBackCoin1();
    //void callBackCoin2();
    
    void actionGetCard(int nCallBack);
    void actionShowCard();
    void callBackShowCard();
    
    void callBackRewardCard();
    void actionDrumCrash();
    void callBackDrumCrash1(CCSprite* sprite);
    void callBackDrumCrash2();
    
    void actionGetStamina(int val);
    void callBackStamina(CCSprite* sprite);
    void actionEnemy(int _enemyType);
    
    void callBackNormalEnemy();
    void goRival();
    void goRivalFromHistory(int rid, AReceivedRival *ri);
    void goRivalAfterLose();
    void goHiddenRival();
    void goHiddenRivalFromHistory(int rid, AReceivedRival *ri);
    void goBoss();
    void callBackRivalBattle();
    void goBossAfterLose();
    
    void actionMoneySendReward();
    void callBackMoneySendReward();
    
    void actionRivalBattleResult1();
    void actionRivalBattleResult2();
    
    void cbTimeOut();
    
    int questID;
    int BossID, BossHP;
    //int rivalBattleID;
    const char* stageLabel;
    
    //ResponseQuestUpdateResultInfo* normalMoneyResult;
    ResponseQuestUpdateResultInfo* normalBattleResult;
    
    //int normalBattleStep;
    int popupCnt;
    //void battleTouch(CCPoint touchPos);
    
    int actionCallFrom;
        
    void setChaseStep(int n);
    
    int questProgress;
    
    TraceUILayer* traceUILayer;
    
    void setUserStat(ResponseRivalBattle* result);
    void setUserStat(ResponseQuestUpdateResultInfo* result);
    void setRivalBattleResult(ResponseRivalBattle* result);
    void setBossBattleResult(ResponseQuestUpdateResultInfo *result);
    void setTraceResult(ResponseQuestUpdateResultInfo* result);
    
    ResponseQuestUpdateResultInfo* receivedTraceResultInfo;
    ResponseRivalBattle* responseRivalBattleInfo;
    
    CCArray *rewardEventQueue;
    
    void RewardEventManager();
    
    bool bOpenLevelUpAfterTrace;
    bool bOpenLevelUpAfterBattle;
    void InitLevelUplayer(int uPoint);
    void CloseLevelUpLayer();
    LevelUpAction *actionLevelUP;
    
    void exitLayer();
    
    //ResponseBattleInfo *responseBattleinfo;
    
    const char* textAdjust(const char *input);
    
    // for battle scene
    ResponseBattleInfo *battleInfoForBattleScene;
    void goBattleLayerFromRival(ResponseRivalBattle* rivalBattleResult, NpcInfo* npcInfo, int _nTeam);
    void goBattleLayerFromQuest(ResponseQuestUpdateResultInfo* bossBattleResult, NpcInfo* npcInfo, int _nTeam);
    BattleFullScreen* battleLayer;
    void callBackFromBattle(int nFrom);
    //void callBackFromBoss();
    
    bool bCallFromHistory;  // TraceHistory -> TraceLayer
    bool bRemoveDetail;     // 
    
    TraceNormalEnemyLayer* normalEnemyLayer;

    TraceResultLayer* pTraceResultLayer;
    void startFight();
    void startKo();
    
    int rivalLevel;
    
    
    
    
    
    void onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data);
    int gQuestID;
    int gRid_for_rivalLayer;
    NpcInfo* gCurrentNpc;
    int gRivalKind;
    AReceivedRival* gRivalInfo;
    
private:
    
    //ResponseQuestUpdateResultInfo* tempResultInfo;
    
    static TraceLayer* instance;
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    void BackBtnCallback(CCObject* pSender);
    
    
    void normalBattleBtnCallback(CCObject* pSender);
    void rivalBattleBtnCallback(CCObject* pSender);
    void hiddenRivalBattleBtnCallback(CCObject* pSender);
    void bossBattleBtnCallback(CCObject* pSender);
    void teamChangeCallback(CCObject* pSender);
    
    int stageCompleteRate;
    
    int touchBeganTag;
    int hitCounter;
    
    CCArray* frameToHit;
    CCSprite* spriteToTab;
    CCArray* frameToTab;
    CCSprite* spriteToGo;
    CCArray* frameToGo;
    CCSprite* spriteFindingEnemyEntry;
    CCArray* frameFindingEnemyEntry;
    CCArray* frameFindingEnemyMain;
    CCArray* frameFindingEnemyExit;
    CCSprite* spriteToAlert;
    CCArray* frameToAlert;
    
    CCArray* frameToSpotlight;
    CCSprite* spriteToSpotlight;
    
    //    int second;
    //    int secDividedBy10;
    //    int secDividedBy100;
    
    int nChaseStep;
    
    int  getChaseStep();
    
    void traceUI(int ccp_y);
    void refreshTraceUI();
    void initTeamUI();//int ccp_y);
    void refreshTeamUI(int ccp_y);
    void removeTeamUI();
    
    void popupStaminaBuy();
    
    int questSp;
    
    CCArray *rewardCards;
    NpcInfo* currentNpc;
    
    void releaseTab();
    void releaseGo();
    
    
    TraceBossLayer* bossLayer;
    TraceHiddenRivalLayer* hiddenRivalLayer;
    TraceRivalLayer* rivalLayer;
    
    void backToHistoryDetail();
    void saveTraceTeam();
    
    int rid_for_rivalLayer;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
    
    //// reward card open effect
    enum CARDPACK_TAG
    {
        BLACK_BG = 800,
        CARDPACK,
        CUT_LINE,
        ANI_TOUCH_ICON,
        CARDPACK_LEFT,
        CUT_CARD_1,
        CUT_CARD_2,
        FADE_01,
        FADE_02,
        FADE_03,
        FADE_04,
        
        CARD_CLOSE_BTN,
        CARD_CLOSE_BTN_LABEL,
        
    };
    bool bRewardCardAction;
    int  showCardCnt;
    void FadeWhite04();
    void RemoveFade04();
    //CCArray* rewardCards;
    void showRewardCard();
    CardDetailViewLayer* cardDetailViewLayer;
    void CreateBtn();
    void startCardAction();
    void closeCardAction();
    
    int prevUserLevelBeforeRivalBattle;
    
};

class MoneySpendNotiPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    MoneySpendNotiPopup(int coin);
    ~MoneySpendNotiPopup();
    
    void InitUI(int coin);//void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
protected:
    
};

class NoMoneyPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    NoMoneyPopup();
    ~NoMoneyPopup();
    
    void InitUI();//void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
protected:
    
};

class NoStaminaPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    NoStaminaPopup();
    ~NoStaminaPopup();
    
    void InitUI();//void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    CocosDenshion::SimpleAudioEngine* soundBG;
    
protected:
    
};

class NoBattlePointPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    NoBattlePointPopup();
    ~NoBattlePointPopup();
    
    void InitUI();//void* data);
    
    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    CocosDenshion::SimpleAudioEngine* soundBG;
};

class StageClearPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    StageClearPopup();
    ~StageClearPopup();
    
    void InitUI();//void* data);
    
    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
protected:
    CocosDenshion::SimpleAudioEngine* soundBG;
    
};

class TeamSettingWarningPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    TeamSettingWarningPopup();
    ~TeamSettingWarningPopup();
    
    void InitUI();//void* data);
    
    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
};

class TraceFailPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    TraceFailPopup();
    ~TraceFailPopup();
    
    void initUI();
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
protected:
};

#endif /* defined(__CapcomWorld__TraceLayer__) */
