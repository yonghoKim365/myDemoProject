//
//  UserStatLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 13..
//
//

#include "UserStatLayer.h"
#include "DojoLayerDojo.h"
#include "CCHttpRequest.h"
#include "ARequestSender.h"
#include "MyUtil.h"
#include "MainScene.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif

UserStatLayer *UserStatLayer::instance = NULL;

extern "C" {
    void updateGoldAndRefreshStat(int gold, int itemID)
    {
        // -- 서버로 아이템 id 날려줘서 보관함에 들어가도록.
        ResponseBuyInfo *buyInfo = ARequestSender::getInstance()->requestBuyItem(itemID);
        
        if(buyInfo)
        {
            PlayerInfo* pinfo = PlayerInfo::getInstance();
            pinfo->myCash = buyInfo->user_stat_gold;
            pinfo->myCoin = buyInfo->user_stat_coin;
            pinfo->setRevengePoint(buyInfo->user_stat_revenge);
            pinfo->setFame(buyInfo->user_stat_fame);
            
            
            
            
            //pinfo->setQuestPoint(buyInfo->user_stat_q_pnt);
            pinfo->setStamina(buyInfo->user_stat_q_pnt);
            //pinfo->setBattlePoint(buyInfo->user_stat_d_pnt);
            pinfo->setBattlePoint(buyInfo->user_stat_a_pnt);
                        
//            pinfo->setDefensePoint(buyInfo->user_stat_d_pnt);
//            pinfo->setAttackPoint(buyInfo->user_stat_a_pnt);
            pinfo->setUpgradePoint(buyInfo->user_stat_u_pnt);
            pinfo->setXp(buyInfo->user_stat_exp);
            pinfo->setLevel(buyInfo->user_stat_lev);
            
            UserStatLayer *info = UserStatLayer::getInstance();
            info->refreshUI();
        }
        
        MainScene::getInstance()->openPurchasePopup(itemID);
    }
};

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#ifdef __cplusplus
extern "C"{
#endif
    
    void Java_com_capcom_FOC_billing_ActivityShop_nativeUpdateGoldAndRefreshStat(JNIEnv* env, jobject thisObj,jint gold,jint itemID){
        
        CCLog(" UpdateGoldAndRefreshStat, gold:%d itemId:%d", gold, itemID);
        
        // -- 서버로 아이템 id 날려줘서 보관함에 들어가도록.
        ResponseBuyInfo *buyInfo = ARequestSender::getInstance()->requestBuyItem(itemID);
        
        if(buyInfo)
        {
            PlayerInfo* pinfo = PlayerInfo::getInstance();
            pinfo->myCash = buyInfo->user_stat_gold;
            pinfo->myCoin = buyInfo->user_stat_coin;
            pinfo->setRevengePoint(buyInfo->user_stat_revenge);
            pinfo->setFame(buyInfo->user_stat_fame);
            
            
            pinfo->setStamina(buyInfo->user_stat_q_pnt);
            pinfo->setBattlePoint(buyInfo->user_stat_a_pnt);
            pinfo->setUpgradePoint(buyInfo->user_stat_u_pnt);
            pinfo->setXp(buyInfo->user_stat_exp);
            pinfo->setLevel(buyInfo->user_stat_lev);
            
            UserStatLayer *info = UserStatLayer::getInstance();
            info->refreshUI();
        }
        
        MainScene::getInstance()->openPurchasePopup(itemID);
        
    }
    
#ifdef __cplusplus
}
#endif

#endif

UserStatLayer::UserStatLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    this->setContentSize(size);
    
    instance = this;
    
    CCSprite* pSprTopUIBg = CCSprite::create("ui/home/ui_userinfo_bg.png");//HelloWorld.png");
    pSprTopUIBg->setAnchorPoint(ccp(0.5f,1));
    pSprTopUIBg->setPosition( ccp(size.width/2, size.height) );
    this->addChild(pSprTopUIBg, 0);
    
    //CCSprite* pSprFace = CCSprite::create("ui/battle_tab/photo_02.png");
    //regSprite( this, ccp(0,1), accp(11, size.height*2-8), pSprFace, 1200);
    //pSprFace->setTag(909);

    //CheckLayerSize(this);
    
    pLevelUpIcon = NULL;
    
    for (int scan = 0;scan < 3;scan++)
    {


        
        
        
//        attackPoints[scan] = NULL;
//        maxAttackPoints[scan] = NULL;
//        defensePoints[scan] = NULL;
//        maxDefensePoints[scan] = NULL;
//        level[scan] = NULL;
//        attackPointsGauge[scan] = NULL;
//        defensePointsGauge[scan] = NULL;
//        friendCount[scan] = NULL;
        
        stamina[scan] = NULL;
        maxStamina[scan] = NULL;
        battlePoints[scan] = NULL;
        maxBattlePoints[scan] = NULL;
        level[scan] = NULL;
        staminaGauge[scan] = NULL;
        battlePointsGauge[scan] = NULL;
        friendCount[scan] = NULL;
        
        
        
        
        
    }
    
    for (int scan = 0;scan < 10;scan++)
        cash[scan] = NULL;
    
    for (int scan = 0;scan < 10;scan++)
        pSprCoin[scan] = NULL;

    
    
    
    
//    refreshQuestPoints();
//    refreshMaxQuestPoints();
    
//    refreshAttackPoints();
//    refreshMaxAttackPoints();
    
//    refreshDefensePoints();
//    refreshMaxDefensePoints();
    
    refreshStamina();
    refreshMaxStamina();
    
    refreshBattlePoints();
    refreshMaxBattlePoints();
    
    
    
    
    
    refreshCash();
    
    refreshCoin();
    
    refreshLevel();
    
    refreshFriendCount();
    
    
    
    
    
//    loadQuestPointsGauge();
//    refreshQuestPointsGauge();
    
//    loadAttackPointsGauge();
//    refreshAttackPointsGauge();
    
//    loadDefensePointsGauge();
//    refreshDefensePointsGauge();
    
    loadStaminaGauge();
    refreshStaminaGauge();
    
    loadBattlePointsGauge();
    refreshBattlePointsGauge();

    
    
    
    
    loadXpGauge();
    refreshXpGauge();

    CCMenuItemImage *pSprTopUIBtnCoin = CCMenuItemImage::create("ui/home/ui_userinfo_point_btn1.png","ui/home/ui_userinfo_point_btn2.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnCoin->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnCoin->setPosition( accp( 404,914));//size.width/5 * 4,0));
    pSprTopUIBtnCoin->setTag(0);
    
    CCMenuItemImage *pSprTopUIBtnGold = CCMenuItemImage::create("ui/home/ui_userinfo_point_btn1.png","ui/home/ui_userinfo_point_btn2.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnGold->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnGold->setPosition( accp( 594,914));//size.width/5 * 4,0));
    pSprTopUIBtnGold->setTag(1);
    
    CCMenuItemImage *pSprTopUIBtnQuest = CCMenuItemImage::create("ui/home/bp_empty.png","ui/home/bp_empty.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnQuest->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnQuest->setOpacity(0);
    pSprTopUIBtnQuest->setPosition( accp(110, 836));//size.width/5 * 4,0));
    pSprTopUIBtnQuest->setTag(2);
    
    CCMenuItemImage *pSprTopUIBtnAttack = CCMenuItemImage::create("ui/home/bp_empty.png","ui/home/bp_empty.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnAttack->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnAttack->setOpacity(0);
    pSprTopUIBtnAttack->setPosition( accp(290, 836));//size.width/5 * 4,0));
    pSprTopUIBtnAttack->setTag(3);
    
    CCMenuItemImage *pSprTopUIBtnDefense = CCMenuItemImage::create("ui/home/bp_empty.png","ui/home/bp_empty.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnDefense->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnDefense->setOpacity(0);
    pSprTopUIBtnDefense->setPosition( accp(470, 836));//size.width/5 * 4,0));
    pSprTopUIBtnDefense->setTag(4);
    
    CCMenuItemImage *pSprTopUIBtnFriends = CCMenuItemImage::create("ui/home/friend_btn.png","ui/home/friend_btn.png",this,menu_selector(UserStatLayer::topUICallback));
    pSprTopUIBtnFriends->setAnchorPoint( ccp(0,0));
    pSprTopUIBtnFriends->setOpacity(0);
    pSprTopUIBtnFriends->setPosition( accp(100, 895));//size.width/5 * 4,0));
    pSprTopUIBtnFriends->setTag(5);

    CCMenu* pMenu = CCMenu::create(pSprTopUIBtnCoin, pSprTopUIBtnGold, pSprTopUIBtnQuest, pSprTopUIBtnAttack, pSprTopUIBtnDefense, pSprTopUIBtnFriends, NULL);
    pMenu->setPosition(CCPointZero);
    pMenu->setTag(99);
    this->addChild(pMenu, 1000);

    
    UserStatLayer::getInstance()->updateUserProfileImage();
    
    
}

UserStatLayer::~UserStatLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void UserStatLayer::topUICallback(CCObject* pSender)
{
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    soundButton1();
    
    switch(tag)
    {
        case 0: // 샵 아이템 레이어
        {
            if (TraceHistoryLayer::getInstance() != NULL){
                TraceHistoryLayer::getInstance()->closeLayer();
            }
            if (TraceDetailLayer::getInstance()){
                MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(),true);
                TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
            }
            
            MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_SHOP);
            /*
            MainScene* main = (MainScene*)MainScene::getInstance();
            main->releaseSubLayers();
            main->initShopLayer();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(4);
            item1->selected();
            
            main->curLayerTag = 4;
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(4);
             */
        }
            break;
            
        case 1: // 샵 골드 레이어
        {
            if (TraceHistoryLayer::getInstance() != NULL){
                TraceHistoryLayer::getInstance()->closeLayer();
            }
            if (TraceDetailLayer::getInstance()){
                MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(),true);
                TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
            }
            
            MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_SHOP);
            /*
            MainScene* main = (MainScene*)MainScene::getInstance();
            main->releaseSubLayers();
            main->initShopLayer();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(4);
            item1->selected();
            
            main->curLayerTag = 4;
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(4);
             */

            ShopLayer* shoplayer = ShopLayer::getInstance();
            shoplayer->InitBtn(1);
            shoplayer->ReleaseSubLayer();
            shoplayer->InitGoldlayer(0);
        }
            break;
            
        case 2:
        {
            // -- quest
            //popupQuest("Quest 포인트는 퀘스트를 진행할 때 소비되며, \n 퀘스트 마다 소비되는 포인트가 각각 다릅니다. \nQuest 포인트가 부족한 경우 \n 더 이상 퀘스트를 할 수 없습니다.\n 3분당 1씩 충전이 되며, \n 그린 젬을 사용하여 충전을 할 수 있습니다. \n 그린 젬은 게임을 진행하면서 획득할 수 있으며, \n 상점에서도 구매가 가능합니다.");
            popupQuest("퀘스트에서 추적 활동을 실행하는 데에 \n소모되는 포인트입니다. \n‘3분당 1’씩 회복하며 ‘그린 젬’을 사용하면\n즉시 완전 회복할 수 있습니다.\n");
        }
            break;
            
        case 3:
        {
            
            // -- attack
            //popupQuest("상대 이용자에게 공격을 할 때 \n 사용하는 포인트를 Atk 포인트라고 합니다. \n 각각의 카드마다 Atk 포인트가 다르며, \n Atk 포인트가 부족한 경우 더 이상 \n 배틀을 할 수 없습니다. \n 3분당 1씩 충전이 되며, \n 블루 젬을 사용하여 충전을 할 수 있습니다. \n 블루 젬은 게임을 진행하면서 획득할 수 있으며, \n 상점에서도 구매가 가능합니다.");
            
        }
            break;
            
        case 4:
        {
            // -- defense
            //popupQuest("다른 이용자들의 공격을 방어할 때 \n 사용하는 포인트를 Def 포인트라고 합니다. \n Def 포인트가 부족한 경우 다른 이용자가 \n 공격해 온다면 자동으로 해당 포인트에 맞는 \n 팀을 구성해 방어팀을 구성하게 됩니다. \n 3분당 1씩 충전이 되며, \n 블루 젬을 사용하여 충전을 할 수 있습니다. \n 블루 젬은 게임을 진행하면서 획득할 수 있으며, \n상점에서도 구매가 가능합니다.");
            popupQuest("플레이어간 배틀, 라이벌 배틀과 같은\n카드 배틀에 소모되는 포인트입니다. \n ‘1분당 1’씩 회복하며 ‘블루 젬’을 사용하면\n즉시 완전 회복할 수 있습니다.");
        }
            break;
            
        case 5:
        {
            if (TraceHistoryLayer::getInstance() != NULL){
                TraceHistoryLayer::getInstance()->closeLayer();
            }
            if (TraceDetailLayer::getInstance()){
                MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(),true);
                TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
            }
            
            // -- 친구초대
            ARequestSender::getInstance()->requestFriendsToGameServer();
        }
            break;
    }
}

void UserStatLayer::HideMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition( ccp(0,-10000));
    }
}

void UserStatLayer::ShowMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition( CCPointZero);
    }    
}

void UserStatLayer::setEnableMenu(bool flag)
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setEnabled(flag);
    }
}

void UserStatLayer::refreshUI()
{

    
    
    
    
//    refreshQuestPoints();
//    refreshMaxQuestPoints();
    
//    refreshAttackPoints();
//    refreshMaxAttackPoints();
    
//    refreshDefensePoints();
//    refreshMaxDefensePoints();
        
    refreshStamina();
    refreshMaxStamina();
    
    refreshBattlePoints();
    refreshMaxBattlePoints();
    
    
    
    
    
    refreshCash();
    
    refreshCoin();
    
    refreshLevel();
    
    refreshFriendCount();
    
    
    
    
    
//    refreshQuestPointsGauge();
    
//    refreshAttackPointsGauge();
    
//    refreshDefensePointsGauge();
    
    refreshStaminaGauge();
    
    refreshBattlePointsGauge();


    
    
    
    refreshXpGauge();
    
}


void UserStatLayer::loadXpGauge()
{
    xpGauge = CCSprite::create("ui/home/gage_exp.png");
    xpGauge->setAnchorPoint(ccp(0, 0));
    this->addChild(xpGauge, 2000);
}

void UserStatLayer::refreshXpGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxXp = playerInfo->getExpCap();
    float ratio = (float)playerInfo->xp / (float)playerInfo->getMaxXp();
    
    
    
    
    
    xpGauge->setPosition(accp(108, 895));
    //xpGauge->setPosition(accp(109, 896));
    
    
    
    
    
    xpGauge->setScaleX(ratio * 529);
}
/*
void UserStatLayer::loadQuestPointsGauge()
{
    char buffer[32];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_quest0%d.png", scan + 1);
        questPointsGauge[scan] = CCSprite::create(buffer);
        questPointsGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(questPointsGauge[scan], 2000);
    }
}

void UserStatLayer::refreshQuestPointsGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->questPoints / (float)playerInfo->maxQuestPoints;
    for (int scan = 0;scan < 3;scan++)
        questPointsGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = 116;
    questPointsGauge[0]->setPosition(accp(x, 849));
    x += 5;
    questPointsGauge[1]->setPosition(accp(x, 849));
    questPointsGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    questPointsGauge[2]->setPosition(accp(x, 849));
}

void UserStatLayer::loadAttackPointsGauge()
{
    char buffer[32];
    for (int scan = 0;scan < 3;scan++)
    {

        sprintf(buffer, "ui/home/gage_atk0%d.png", scan + 1);
        attackPointsGauge[scan] = CCSprite::create(buffer);
        attackPointsGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(attackPointsGauge[scan], 2000);
    }
}

void UserStatLayer::refreshAttackPointsGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->attackBP / (float)playerInfo->maxAttackPoints;
    for (int scan = 0;scan < 3;scan++)
        attackPointsGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = 301;
    attackPointsGauge[0]->setPosition(accp(x, 849));
    x += 5;
    attackPointsGauge[1]->setPosition(accp(x, 849));
    attackPointsGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    attackPointsGauge[2]->setPosition(accp(x, 849));
}

void UserStatLayer::loadDefensePointsGauge()
{
    char buffer[32];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_def0%d.png", scan + 1);
        defensePointsGauge[scan] = CCSprite::create(buffer);
        defensePointsGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(defensePointsGauge[scan], 2000);
    }
}

void UserStatLayer::refreshDefensePointsGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->defenceBP / (float)playerInfo->maxDefensePoints;
    for (int scan = 0;scan < 3;scan++)
        defensePointsGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = 488;
    defensePointsGauge[0]->setPosition(accp(x, 849));
    x += 5;
    defensePointsGauge[1]->setPosition(accp(x, 849));
    defensePointsGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    defensePointsGauge[2]->setPosition(accp(x, 849));
}
*/





void UserStatLayer::loadStaminaGauge()
{
    char buffer[64];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_stamina0%d.png", scan + 1);
        staminaGauge[scan] = CCSprite::create(buffer);
        staminaGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(staminaGauge[scan], 2000);
    }
}

void UserStatLayer::refreshStaminaGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->getStamina() / (float)playerInfo->getMaxStamina();   // stamina 로 수정해야 함
    for (int scan = 0;scan < 3;scan++){
        if (staminaGauge[scan] != NULL){
            staminaGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
        }
    }
    int x = 126;
    staminaGauge[0]->setPosition(accp(x, 850));
    x += 5;
    staminaGauge[1]->setPosition(accp(x, 850));
    staminaGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    staminaGauge[2]->setPosition(accp(x, 850));
}

void UserStatLayer::loadBattlePointsGauge()
{
    char buffer[64];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_bp0%d.png", scan + 1);
        battlePointsGauge[scan] = CCSprite::create(buffer);
        battlePointsGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(battlePointsGauge[scan], 2000);
    }
}

void UserStatLayer::refreshBattlePointsGauge()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->getBattlePoint() / (float)playerInfo->getMaxBattlePoint();// maxDefensePoints;   // stamina 로 수정해야 함
    for (int scan = 0;scan < 3;scan++)
        battlePointsGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = 399;
    battlePointsGauge[0]->setPosition(accp(x, 850));
    x += 5;
    battlePointsGauge[1]->setPosition(accp(x, 850));
    battlePointsGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    battlePointsGauge[2]->setPosition(accp(x, 850));
}





/*
void UserStatLayer::removeQuestPoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (questPoints[scan] != NULL)
        {
            this->removeChild(questPoints[scan], true);
        }
        questPoints[scan] = NULL;
    }
}

void UserStatLayer::refreshQuestPoints()
{
    PlayerInfo *pi = PlayerInfo::getInstance();
    removeQuestPoints();
    int x = 226;
    float scale = 1.0f;
    char buffer[3];
    sprintf(buffer, "%d", pi->questPoints);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        questPoints[scan] = createNumber(number, accp(x, 870), scale);
        instance->addChild(questPoints[scan], 2000);
        CCSize size = questPoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        questPoints[scan]->setPosition(accp(x, 870));
    }
}

void UserStatLayer::refreshMaxQuestPoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxQuestPoints = 136;
    removeMaxQuestPoints();
    bool skip = true;
    int x = 232;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        int number = (playerInfo->maxQuestPoints % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxQuestPoints[scan] = createNumber(number, accp(x, 870), scale);
        this->addChild(maxQuestPoints[scan], 2000);
        CCSize size = maxQuestPoints[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void UserStatLayer::removeMaxQuestPoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxQuestPoints[scan] != NULL)
            this->removeChild(maxQuestPoints[scan], true);
        maxQuestPoints[scan] = NULL;
    }
}

void UserStatLayer::refreshAttackPoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->attackBP = 802;
    removeAttackPoints();
    int x = 411;
    float scale = 1.0f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->attackBP);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        attackPoints[scan] = createNumber(number, accp(x, 870), scale);
        instance->addChild(attackPoints[scan], 2000);
        CCSize size = attackPoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        attackPoints[scan]->setPosition(accp(x, 870));
    }
}

void UserStatLayer::refreshMaxAttackPoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxAttackPoints = 802;
    removeMaxAttackPoints();
    bool skip = true;
    int x = 417;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        int number = (playerInfo->maxAttackPoints % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxAttackPoints[scan] = createNumber(number, accp(x, 870), scale);
        this->addChild(maxAttackPoints[scan], 2000);
        CCSize size = maxAttackPoints[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void UserStatLayer::removeAttackPoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (attackPoints[scan] != NULL)
            this->removeChild(attackPoints[scan], true);
        attackPoints[scan] = NULL;
    }
}

void UserStatLayer::removeMaxAttackPoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxAttackPoints[scan] != NULL)
            this->removeChild(maxAttackPoints[scan], true);
        maxAttackPoints[scan] = NULL;
    }
}

void UserStatLayer::refreshDefensePoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->defenceBP = 35;
    removeDefensePoints();
    int x = 589;
    float scale = 1.0f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->defenceBP);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        defensePoints[scan] = createNumber(number, accp(x, 870), scale);
        this->addChild(defensePoints[scan], 2000);
        CCSize size = defensePoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        defensePoints[scan]->setPosition(accp(x, 870));
    }
}

void UserStatLayer::refreshMaxDefensePoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxDefensePoints = 117;
    removeMaxDefensePoints();
    bool skip = true;
    int x = 595;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        int number = (playerInfo->maxDefensePoints % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxDefensePoints[scan] = createNumber(number, accp(x, 870), scale);
        this->addChild(maxDefensePoints[scan], 2000);
        CCSize size = maxDefensePoints[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void UserStatLayer::removeDefensePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (defensePoints[scan] != NULL)
            this->removeChild(defensePoints[scan], true);
        defensePoints[scan] = NULL;
    }
}

void UserStatLayer::removeMaxDefensePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxDefensePoints[scan] != NULL)
            this->removeChild(maxDefensePoints[scan], true);
        maxDefensePoints[scan] = NULL;
    }
}
*/





void UserStatLayer::refreshStamina()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->attackBP = 802;
    removeStamina();
    int x = 360;
    float scale = 1.3f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->getStamina());    
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        stamina[scan] = createNumber(number, accp(x, 865), scale);
        instance->addChild(stamina[scan], 2000);
        CCSize size = stamina[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        stamina[scan]->setPosition(accp(x, 865));
    }
}

void UserStatLayer::refreshAttackPoints()
{
    refreshStamina();
}

void UserStatLayer::refreshMaxStamina()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxAttackPoints = 802;
    removeMaxStamina();
    bool skip = true;
    int x = 315;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        int number = (playerInfo->getMaxStamina() % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));      // maxStamina 로 수정해야 함
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxStamina[scan] = createNumber(number, accp(x, 848), scale);
        this->addChild(maxStamina[scan], 2000);
        CCSize size = maxStamina[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void UserStatLayer::removeStamina()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (stamina[scan] != NULL)
            this->removeChild(stamina[scan], true);
        stamina[scan] = NULL;
    }
}

void UserStatLayer::removeMaxStamina()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxStamina[scan] != NULL)
            this->removeChild(maxStamina[scan], true);
        maxStamina[scan] = NULL;
    }
}

void UserStatLayer::refreshBattlePoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->defenceBP = 35;
    removeBattlePoints();
    int x = 630;
    float scale = 1.3f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->getBattlePoint());//  defenceBP);   // battlePoint 로 수정해야 함
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        battlePoints[scan] = createNumber(number, accp(x, 865), scale);
        this->addChild(battlePoints[scan], 2000);
        CCSize size = battlePoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        battlePoints[scan]->setPosition(accp(x, 865));
    }
}

void UserStatLayer::refreshMaxBattlePoints()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->maxDefensePoints = 117;
    removeMaxBattlePoints();
    bool skip = true;
    int x = 585;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        //int number = (playerInfo->maxDefensePoints % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));     // maxBattlePoint 로 수정해야 함
        int number = (playerInfo->getMaxBattlePoint() % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));     // maxBattlePoint 로 수정해야 함
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxBattlePoints[scan] = createNumber(number, accp(x, 848), scale);
        this->addChild(maxBattlePoints[scan], 2000);
        CCSize size = maxBattlePoints[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void UserStatLayer::removeBattlePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (battlePoints[scan] != NULL)
            this->removeChild(battlePoints[scan], true);
        battlePoints[scan] = NULL;
    }
}

void UserStatLayer::removeMaxBattlePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxBattlePoints[scan] != NULL)
            this->removeChild(maxBattlePoints[scan], true);
        maxBattlePoints[scan] = NULL;
    }
}





void format_commas(int n, char *out)
{
    int c;
    char buf[20];
    char *p;
    
    sprintf(buf, "%d", n);
    c = 2 - strlen(buf) % 3;
    for (p = buf; *p != 0; p++) {
        *out++ = *p;
        if (c == 1) {
            *out++ = ',';
        }
        c = (c + 1) % 3;
    }
    *--out = 0;
}


void UserStatLayer::refreshCash()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->myCash = 17510;
    removeCash();
    int x = 589;
    float scale = 1.0f;
    char buffer[10];
    format_commas(playerInfo->myCash, buffer);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        if (buffer[scan] == ',')
            cash[scan] = createComma(accp(x, 917), scale);
        else
        {
            int number = buffer[scan] - '0';
            cash[scan] = createNumber(number, accp(x, 920), scale);
        }
        this->addChild(cash[scan], 2000);
        CCSize size = cash[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        if (buffer[scan] == ',')
            cash[scan]->setPosition(accp(x, 917));
        else
            cash[scan]->setPosition(accp(x, 920));
    }
}

void UserStatLayer::removeCash()
{
    for (int scan = 0;scan < 10;scan++)
    {
        if (cash[scan] != NULL)
            this->removeChild(cash[scan], true);
        cash[scan] = NULL;
    }
}

void UserStatLayer::refreshCoin()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeCoin();
    int x = 400;
    float scale = 1.0f;
    char buffer[10];
    format_commas(playerInfo->myCoin, buffer);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        if (buffer[scan] == ','){
            pSprCoin[scan] = createComma(accp(x, 917), scale);
            //CCLog("refreshCoin, scan:%d",scan);
            pSprCoin[scan]->setTag(3000+scan);
        }
        else
        {
            int number = buffer[scan] - '0';
            pSprCoin[scan] = createNumber(number, accp(x, 920), scale);
            //CCLog("refreshCoin, scan:%d",scan);
            pSprCoin[scan]->setTag(3000+scan);
        }
        this->addChild(pSprCoin[scan], 2000);
        CCSize size = pSprCoin[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        if (buffer[scan] == ',')
            pSprCoin[scan]->setPosition(accp(x, 917));
        else
            pSprCoin[scan]->setPosition(accp(x, 920));
        
        //CCLog("refreshCoin, scan:%d, tag:%d",scan, coin[scan]->getTag());
    }
}

void UserStatLayer::removeCoin()
{
    for (int scan = 0;scan < 10;scan++)
    {
        if (pSprCoin[scan] != NULL){
            this->removeChild(pSprCoin[scan], true);
        }
        //if (this->getChildByTag(3000+scan) == NULL)
        //    continue;
        //this->removeChildByTag(3000+scan,true);
    }
    /*
    for (int scan = 0;scan < 10;scan++)
    {
        if (pSprCoin[scan] != NULL){
            CC_SAFE_DELETE(pSprCoin[scan]);
            pSprCoin[scan] = NULL;
        }
    }
     */
    
    /*
     for (int scan = 0;scan < 10;scan++)
     {
     if (coin[scan] != NULL){
     //this->removeChild(coin[scan], true);
     //CCLog(" coin[scan]->getTag():%d",coin[scan]->getTag());
     this->removeChildByTag(3000+scan,false);
     }
     coin[scan] = NULL;
     }
     */
}

void UserStatLayer::refreshLevel()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //playerInfo->myLevel = 9;
    removeLevel();
    int x = 84;
    float scale = 0.9f;
    char buffer[10];
    sprintf(buffer, "%d", playerInfo->myLevel);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        level[scan] = createNumber(number, accp(x, 845), scale);
        this->addChild(level[scan], 2000);
        CCSize size = level[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        level[scan]->setPosition(accp(x, 845));
    }
}

void UserStatLayer::removeLevel()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (level[scan] != NULL)
            this->removeChild(level[scan], true);
        level[scan] = NULL;
    }
}

void UserStatLayer::refreshFriendCount()
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeFriendCount();
    int x = 198;
    float scale = 0.9f;
    char buffer[10];
    sprintf(buffer, "%d", playerInfo->numOfKakaoAppFriends);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        friendCount[scan] = createNumber(number, accp(x, 920), scale);
        this->addChild(friendCount[scan], 2000);
        CCSize size = friendCount[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        friendCount[scan]->setPosition(accp(x, 920));
    }
}

void UserStatLayer::removeFriendCount()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (friendCount[scan] != NULL){
            this->removeChild(friendCount[scan], true);
            friendCount[scan] = NULL;
        }
    }
}

CCSprite *UserStatLayer::createNumber(int number, cocos2d::CCPoint pos, float scale) {
    assert(number >= 0 && number < 10);
    char buffer[64];
    sprintf(buffer, "ui/home/number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

CCSprite *UserStatLayer::createComma(cocos2d::CCPoint pos, float scale) {
    
    CCSprite *sprite = CCSprite::create("ui/home/number_comma.png");
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

void UserStatLayer::updateUserProfileImage(){
    
    PlayerInfo *pi =PlayerInfo::getInstance();
    
    //PlayerInfo::getInstance()->SetUserProfileUrl("http://th-p0.talk.kakao.co.kr/th/talkp/wkaXAOKWOJ/DbreQnUKeRKi4rBx07v9YK/oqx55_110x110_c.jpg");
    
    std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(PlayerInfo::getInstance()->GetUserProfileUrl());
    
    CCLog("updateUserProfileImage: %s", PlayerInfo::getInstance()->GetUserProfileUrl());
    CCLog("updateUserProfileImage, filename=%s",filename.c_str());
    
    if (filename.size() > 0){
        if (FileManager::sharedFileManager()->IsProfileFileExist(filename.c_str())){
            //CCLog("updateUserProfileImage, image exist. do register");
            registerUserProfileImg(filename);
        }
        else{
            //CCLog("updateUserProfileImage, image NOT exist. do download");
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            std::vector<std::string> downloads;
            downloads.push_back(pi->GetUserProfileUrl());
            requestor->addDownloadTask(downloads, this, callfuncND_selector(UserStatLayer::profileImgDownloaded));
        }
    }
}


void UserStatLayer::profileImgDownloaded(cocos2d::CCObject *pSender, void *data){
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        //CCLog(" profileImgDownloaded, OK");
        
        this->removeChildByTag(909,true);
        
        std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(PlayerInfo::getInstance()->GetUserProfileUrl());
        
        registerUserProfileImg(filename);
    }
}

void UserStatLayer::registerUserProfileImg(std::string filename){
    
    //CCLog("registerUserProfileImg, filename:%s", filename.c_str());
    
    std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + filename;
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
    
    if (pSprFace != NULL){
        CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
        float imgScale = (float)78 / aa.height;
        
        pSprFace->setScale(imgScale);
        
        regSprite( this, ccp(0,1), accp(16, size.height*SCREEN_ZOOM_RATE-14), pSprFace, 1200);
        
    }

    //DojoLayerDojo::getInstance()->RefreshUsername();
    UserStatLayer::getInstance()->refreshFriendCount();
    
}


void UserStatLayer::AddLevelUpIcon()
{
    RemoveLevelUpIcon();
    
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    if (pMenu->getPositionY()!=0)return;
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    pLevelUpIcon = CCMenuItemImage::create("ui/quest/levelup_statbtn1.png", "ui/quest/levelup_statbtn2.png", this, menu_selector(UserStatLayer::LevelUpCallback));
    pLevelUpIcon->setAnchorPoint(ccp(0,0));
    pLevelUpIcon->setPosition(accp(66, (size.height*SCREEN_ZOOM_RATE) - 94));
    pLevelUpIcon->setTag(248);
    
    pLevelUpMenu = CCMenu::create(pLevelUpIcon, NULL);
    pLevelUpMenu->setPosition( CCPointZero );
    pLevelUpMenu->setTag(249);
    this->addChild(pLevelUpMenu, 1500);
}

void UserStatLayer::RemoveLevelUpIcon()
{
    if(NULL != pLevelUpIcon)
    {
        this->removeChild(pLevelUpIcon, true);
        pLevelUpIcon = NULL;
    }
    
    if(NULL != pLevelUpMenu)
    {
        this->removeChild(pLevelUpMenu, true);
        pLevelUpMenu = NULL;
    }
}


void UserStatLayer::LevelUpCallback(CCObject* pSender)
{
    
    if (TraceHistoryLayer::getInstance() != 0){
        TraceHistoryLayer::getInstance()->closeLayer();
    }
    if (TraceDetailLayer::getInstance() != 0){
        MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(), true);
    }
    
    CCMenu *menu = (CCMenu*)this->getChildByTag(249);
    CCMenuItemImage *item = (CCMenuItemImage*)menu->getChildByTag(248);
    item->unselected();
    
    MainScene::getInstance()->initLevelUpLayer();
}