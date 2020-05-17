//
//  BattleDuelLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#include "BattleDuelLayer.h"
#include "BattlePlayerCell.h"
#include "MainScene.h"
#include "ARequestSender.h"
#include "PopUp.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

BattleDuelLayer* BattleDuelLayer::instance = NULL;

BattleDuelLayer::BattleDuelLayer(CCSize layerSize) : battleLayer(NULL)
{
    this->setContentSize(layerSize);
    this->setAnchorPoint(ccp(0,0));
    
    instance = this;
    
    nBattleStep = 0;
    selectedTeam = 0;
    
    CCLog("BattleDuelLayer() selectedTeam :%d",selectedTeam);
    
    //clipRect = CCRectMake(0,0, layerSize.width, layerSize.height);
    cardMaker = new ACardMaker();
    
    
    InitUI();
    nBattleStepOld = -1;
    nBattleStep = 0;
    
    //ARequestSender::getInstance()->requestOpponent();
    InitLayer(0);
    //initThread(); //
    
    //CheckLayerSize(this);
    
    
//    CheckLayerSize(this);
    
    this->setTouchEnabled(true);
//    this->setClipsToBounds(true);
}


BattleDuelLayer::~BattleDuelLayer()
{
    
    this->removeAllChildrenWithCleanup(true);
    
}

void BattleDuelLayer::InitUI()
{
    this->removeAllChildrenWithCleanup(true);
    
    CCSprite* pSprite = CCSprite::create("ui/battle_tab/battle_duel_bg.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setScale(1.3f);
    pSprite->setPosition( ccp(0,-80.0f) );
    this->addChild(pSprite, 0);
    
    CCSize size = this->getContentSize();
    
}


void BattleDuelLayer::InitLayer(int _step, int _selectedTeam)
{
    selectedTeam = _selectedTeam;
    InitLayer(_step);
}

void BattleDuelLayer::InitLayer(int _step)
{
    if (nBattleStepOld == _step)return;
    
    CCLog(" selectedTeam :%d",selectedTeam);
    
    nBattleStepOld = _step;
    nBattleStep = _step;
    
    //delete battleListLayer;
    if (battleListLayer != NULL){
        this->removeChild(battleListLayer,true);
    }
    
    if (battlePrebattleLayer != NULL){
        this->removeChild(battlePrebattleLayer,true);
    }
    
    if (cardListLayer != NULL){
        this->removeChild(cardListLayer,true);
        this->removeChild(teamEditLayer,true);
        cardListLayer = NULL;
        teamEditLayer = NULL;
    }
    
    if(NULL != battleLayer)
    {
        this->setTouchEnabled(true);
        
        RestoreZProirity(this);

        this->removeChild(battleLayer, true);
        battleLayer = NULL;
    }
    
    nBattleStep = _step;
    if (_step == 0)InitLayer0();
    else if (_step == 1)InitLayer1(selectedTeam);
    else if (_step == 2)InitBattleLayer(selectedTeam);
    else if (_step == 3)InitLayer3();
    else if (_step == 10){
        InitTeamEditLayer();
    }
    
    
}

void BattleDuelLayer::InitLayer0()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    //battleListLayer = new BattleListLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)) );
    battleListLayer = new BattleListLayer(CCSize(size.width, this->getContentSize().height));
    battleListLayer->setAnchorPoint(ccp(0,0));
    this->addChild(battleListLayer,100);
}

void BattleDuelLayer::InitLayer1(int _team)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    battlePrebattleLayer = new BattlePrebattleLayer(CCSize(size.width, this->getContentSize().height), selectedUser, _team);
    battlePrebattleLayer->setAnchorPoint(ccp(0,0));
    this->addChild(battlePrebattleLayer,100);
    
}

// -- 결과화면
void BattleDuelLayer::InitLayer3()
{
    resultBG_On();
    
    MainScene::getInstance()->ShowMainMenu();
    DojoLayerBattle::getInstance()->ShowSubMenu();
    UserStatLayer::getInstance()->ShowMenu();
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    int yy = this->getContentSize().height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN;
    
    yy -= 260;
    CCSprite* pSpr1 = CCSprite::create("ui/battle_tab/battle_duel_result_info_bg1.png");
    pSpr1->setAnchorPoint(accp(0,0));
    pSpr1->setPosition( accp(10,yy) );
    this->addChild(pSpr1, 10);
    
    CardInfo *card = pInfo->GetCardInDeck(0, selectedTeam, 2);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());
        
        if(cardInfo)
        {
            string path = "ui/cha/";
//            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path += cardInfo->GetSmallBattleImg();
            
            CCSprite* pChar = CCSprite::create(path.c_str());//,160))
            pChar->setScale(0.63f);
            pChar->setAnchorPoint(accp(0,0));
            pChar->setPosition( accp(10, yy+94) );
            addChild(pChar,5);
        }
    }
    
    DojoLayerDojo* dojo = (DojoLayerDojo*)DojoLayerDojo::getInstance();
    Bg_List* bglist = (Bg_List*)dojo->BgDictionary->objectForKey(pInfo->getBackground());

    CCSprite* pSprBg2 = NULL;
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        pSprBg2 = CCSprite::create("ui/main_bg/bg_1.png", CCRectMake(0,accp(112),accp(500),accp(131)));
    }
    else
    {
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=bglist->L_ImgPath;
        
        pSprBg2 = CCSprite::create(path.c_str(),CCRectMake(0,accp(112),accp(500),accp(131)));//,160));
    }

    pSprBg2->setScale(1.22f);
    pSprBg2->setAnchorPoint(accp(0,0));
    pSprBg2->setPosition( accp(15,yy+95) ); // -174
    addChild(pSprBg2,0);
    
    CCSprite* pSpr11 = NULL;
    if(0 == pInfo->battleResponseInfo->battleResult)
    {
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_win.png");
        playEffect("audio/win_01.mp3");
    }
    else if(1 == pInfo->battleResponseInfo->battleResult)
    {
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_lose.png");
        playEffect("audio/lose_01.mp3");
    }
    else if(2 == pInfo->battleResponseInfo->battleResult)
    {
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_draw.png");
        playEffect("audio/draw_01.mp3");
    }

    pSpr11->setAnchorPoint(accp(0,0));
    pSpr11->setPosition( accp(237,yy+114) );
    this->addChild(pSpr11, 6);
    
    CCLabelTTF* pLabel10 = CCLabelTTF::create("획득코인"  , "HelveticaNeue", 12);
    pLabel10->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(53, yy + 45), pLabel10, 10);
    
    int coin_income = pInfo->battleResponseInfo->reward_coin;
    
    pInfo->addCoin(coin_income);
    char buf0[10];
    sprintf(buf0, "%d", coin_income);
    CCLayer *_parentLayer = NULL;
    CCLayer *_childLayer = this;
    bool bLoop = true;
    while(bLoop){
        _parentLayer = (CCLayer*)_childLayer->getParent();
        if (_parentLayer==NULL){
            _parentLayer = _childLayer;
            bLoop=false;
        }
        else _childLayer = _parentLayer;
    }
    if (_parentLayer != NULL){
        //MainScene *mainScene = (MainScene*)_parentLayer;
        
        UserStatLayer *info = UserStatLayer::getInstance();
        info->refreshCoin();
    }
    
    
    CCLabelTTF* pLabel11 = CCLabelTTF::create(buf0  , "HelveticaNeue-Bold", 12);
    pLabel11->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(151, yy + 45), pLabel11, 10);
    
    CCLabelTTF* pLabel12 = CCLabelTTF::create("뺏어온 명성"  , "HelveticaNeue", 12);
    pLabel12->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(290, yy + 45), pLabel12, 10);
    
    char buf3[10];
    sprintf(buf3, "%d", pInfo->battleResponseInfo->reward_fame);
    CCLabelTTF* pLabel13 = CCLabelTTF::create(buf3, "HelveticaNeue-Bold", 12);
    pLabel13->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(422, yy + 45), pLabel13, 10);
    
    CCLabelTTF* pLabel14 = CCLabelTTF::create("잔여배틀포인트"  , "HelveticaNeue", 12);
    pLabel14->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(58, yy + 12), pLabel14, 10);

    char buf1[10];
    int attckPoint = pInfo->getBattlePoint() - pInfo->battleResponseInfo->used_battle_point;
    if(attckPoint <= 0) attckPoint= 0;
    
    pInfo->setFame(pInfo->getFame() + pInfo->battleResponseInfo->reward_fame);
    
    pInfo->setBattlePoint(attckPoint);
    UserStatLayer *info = UserStatLayer::getInstance();
    info->refreshAttackPoints();

    sprintf(buf1, "%d", attckPoint);
    CCLabelTTF* pLabel15 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
    pLabel15->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(222, yy + 12), pLabel15, 10);
    
    /*
    CCLabelTTF* pLabel16 = CCLabelTTF::create("잔여방어포인트"  , "HelveticaNeue", 12);
    pLabel16->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(290, yy + 12), pLabel16, 10);

    char buf2[10];
    sprintf(buf2, "%d", pInfo->getDefensePoint());
    CCLabelTTF* pLabel17 = CCLabelTTF::create(buf2  , "HelveticaNeue-Bold", 12);
    pLabel17->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(454, yy + 12), pLabel17, 10);
    */
    //////// space
    yy -= 10;
    
    /////////
    yy -= 348;
    CCSprite* pSpr3 = CCSprite::create("ui/battle_tab/battle_duel_result_info_bg2.png");
    pSpr3->setAnchorPoint(accp(0,0));
    pSpr3->setPosition( accp(10,yy) );
    this->addChild(pSpr3, 0);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("배틀상대정보"  , "HelveticaNeue-Bold", 12);
    pLabel3->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(20, yy + 310), pLabel3,10);
    
    CCLabelTTF* pLabel31 = CCLabelTTF::create(pInfo->battleResponseInfo->enemy_nick  , "HelveticaNeue-Bold", 12);
    CCLabelTTF* pLabel32 = CCLabelTTF::create("Lv."    , "HelveticaNeue-Bold", 12);
    
    char buf38[10];
    sprintf(buf38, "%d", pInfo->battleResponseInfo->enemy_level);
    CCLabelTTF* pLabel38 = CCLabelTTF::create(buf38    , "HelveticaNeue-Bold", 12);
    pLabel38->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(154, yy + 236), pLabel38, 10);

    CCLabelTTF* pLabel33 = CCLabelTTF::create("친구"      , "HelveticaNeue", 12);
    
    char buf34[10];
    sprintf(buf34, "%d", pInfo->battleResponseInfo->enemy_appFriends);
    CCLabelTTF* pLabel34 = CCLabelTTF::create(buf34       , "HelveticaNeue-Bold", 12);
    
//    CCLabelTTF* pLabel35 = CCLabelTTF::create("총 방어 포인트"  , "HelveticaNeue", 12);
//    char buf39[10];
//    sprintf(buf39, "%d", pInfo->battleResponseInfo->enemy_defense_pnt);
//    CCLabelTTF* pLabel39 = CCLabelTTF::create(buf39  , "HelveticaNeue-Bold", 12);
//    registerLabel( this,ccp(0,0), accp(468, yy + 236), pLabel39, 10);
    
    CCLabelTTF* pLabel36 = CCLabelTTF::create("전적"      , "HelveticaNeue", 12);
    
    char buf37[10];
    sprintf(buf37, "%d", pInfo->battleResponseInfo->enemy_victory_cnt);
    string enemyVictorCnt = buf37;
    string enemyVictory = enemyVictorCnt + "승";
    
    char buf40[10];
    float aaa = float(pInfo->battleResponseInfo->enemy_victory_cnt) / float(pInfo->battleResponseInfo->enemy_battle_pnt);
    int aaa1 = (int)(aaa * 100);
    sprintf(buf40, "%d", aaa1);
    string enemyVictoryPnt = buf40;
    
    enemyVictory = enemyVictory + "(" + enemyVictoryPnt + "%)";
    
    CCLabelTTF* pLabel37 = CCLabelTTF::create(enemyVictory.c_str()    , "HelveticaNeue-Bold", 12);
    
    pLabel31->setColor(COLOR_YELLOW);
    pLabel32->setColor(COLOR_WHITE);
    pLabel33->setColor(COLOR_ORANGE);
    pLabel34->setColor(COLOR_WHITE);
    //pLabel35->setColor(COLOR_ORANGE);
    pLabel36->setColor(COLOR_ORANGE);
    pLabel37->setColor(COLOR_WHITE);
    //pLabel39->setColor(COLOR_WHITE);

    registerLabel( this,ccp(0,0), accp(120, yy + 266), pLabel31, 10);
    registerLabel( this,ccp(0,0), accp(120, yy + 236), pLabel32, 10);
    registerLabel( this,ccp(0,0), accp(200, yy + 236), pLabel33, 10);
    registerLabel( this,ccp(0,0), accp(249, yy + 236), pLabel34, 10);
    //registerLabel( this,ccp(0,0), accp(313, yy + 236), pLabel35, 10);
    registerLabel( this,ccp(0,0), accp(120, yy + 206), pLabel36, 10);
    registerLabel( this,ccp(0,0), accp(174, yy + 206), pLabel37, 10);
    
    float x = 24;
    
    for (int i=0;i<5;i++)
    {
        CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[i]);

        if (cardInfo != NULL)
        {
            cardMaker->MakeCardThumb(this, cardInfo, ccp(x, yy+12), 170, 100, 10+i);
        }
        
        x+=120;
    }
    
    
    UserStatLayer::getInstance()->refreshUI();
    
    yy-=52;
    CCSprite* pSprBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pSprBtn->setAnchorPoint(ccp(0,0));
    pSprBtn->setPosition( accp(218,yy) );
    pSprBtn->setTag(30);
    this->addChild(pSprBtn, 0);
    
    CCLabelTTF* pLabelBtn = CCLabelTTF::create("배틀 리스트"  , "HelveticaNeue-Bold", 12);
    pLabelBtn->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0), accp(320, yy + 6), pLabelBtn, 10);
    
    CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
    std::vector<std::string> downloads;
    
    CCLog(" selectedUser->profileImageUrl :%s", selectedUser->profileImageUrl.c_str());
    if (selectedUser->profileImageUrl.length() > 0)
    {
        downloads.push_back(selectedUser->profileImageUrl);
        requestor->addDownloadTask(downloads, this, callfuncND_selector(BattleDuelLayer::registerUserProfileImg));
    }
    /*
    printf("tp %d\n", tutorialProgress);
    if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 10)
    {
        this->setTouchEnabled(false);
        
        tutorialProgress = 11;
        TutorialPopUp *basePopUp = new TutorialPopUp;
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(basePopUp, 9000);
    }
     */
}

void BattleDuelLayer::InitTeamEditLayer()
{
    MainScene::getInstance()->HideMainMenu();
    UserStatLayer::getInstance()->HideMenu();
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    
    CCLog("InitTeamEditLayer(), selectedTeam  : %d", selectedTeam);
    
    // init card list
    cardListLayer = new CardListLayer( CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height-accp(120+90+30)), pi->myCards, CALL_CARDLIST_FROM_DECK, this, true, selectedTeam);
    
    cardListLayer->setAnchorPoint(ccp(0,0));
    cardListLayer->setPosition(accp(10,90+90+70));
    this->addChild(cardListLayer,10);
    
    // init team edit layer
    teamEditLayer = new TeamEditLayer(0, BattlePrebattleLayer::getInstance()->selectedTeam, pi->getCardDeck(BattlePrebattleLayer::getInstance()->selectedTeam), this, 1);
    teamEditLayer->setPosition(ccp(0,0-accp(MAIN_LAYER_BTN_HEIGHT)));
    this->addChild(teamEditLayer,20);
    
}

void BattleDuelLayer::InitBattleLayer(int selectTeam)
{
    MainScene::getInstance()->HideMainMenu();
    DojoLayerBattle::getInstance()->HideSubMenu();
    UserStatLayer::getInstance()->HideMenu();
    
    ToTopZPriority(this);
    this->removeAllChildrenWithCleanup(true);
    this->setTouchEnabled(false);
    
    BattleLoad();
}

void BattleDuelLayer::BattleLoad()
{
    addLoadingAni();
    
    bool gameStart = true;
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    FileManager* fmanager = FileManager::sharedFileManager();
    
    //FOC_IMAGE_SERV_URL
    std::string basePathL = FOC_IMAGE_SERV_URL;
    basePathL.append("images/cha/cha_l/");
    
    std::string basePathS  = FOC_IMAGE_SERV_URL;
    basePathS.append("images/cha/cha_s/");
    
    std::vector<std::string> downloads;
    
    // -- 적 이미지
    for(int i=0; i<5; ++i)
    {
        if(0 != pInfo->battleResponseInfo->opponent_card[i])
        {
            CardListInfo* card = fmanager->GetCardInfo(pInfo->battleResponseInfo->opponent_card[i]);
            
            if(card)
            {
                if(!fmanager->IsFileExist(card->GetLargeBattleImg()))
                {
                    gameStart = false;
                    
                    string downPath = basePathL + card->GetLargeBattleImg();
                    downloads.push_back(downPath);
                }
            }
        }
    }
    
    // -- 아군 이미지
    for(int k=0; k<5; ++k)
    {
        CardInfo *card = pInfo->GetCardInDeck(0, selectedTeam, k);
        
        if(card)
        {
            CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
            
            if(cardInfo)
            {
                if(!fmanager->IsFileExist(cardInfo->GetLargeBattleImg()))
                {
                    gameStart = false;
                    
                    string downPath = basePathL + cardInfo->GetLargeBattleImg();
                    downloads.push_back(downPath);
                }
            }
        }
    }
    
    // -- 결과 화면에 나올 리더 스몰 이미지
    CardInfo *card = pInfo->GetCardInDeck(0, selectedTeam, 2);
    
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());
        
        if(!fmanager->IsFileExist(cardInfo->GetSmallBattleImg()))
        {
            gameStart = false;
            
            string downPath = basePathS + cardInfo->GetSmallBattleImg();
            downloads.push_back(downPath);
        }
    }
    
    if(0 != pInfo->battleResponseInfo->opponent_card[2])
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[2]);
        
        //CCLog("cardInfo->GetSmallBattleImg():%s",cardInfo->GetSmallBattleImg());
        
        if(!fmanager->IsFileExist(cardInfo->GetSmallBattleImg()))
        {
            gameStart = false;
            
            string downPath = basePathS + cardInfo->GetSmallBattleImg();
            downloads.push_back(downPath);
        }
    }
    
    if(false == gameStart)
    {
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        requestor->addDownloadTask(downloads, this, callfuncND_selector(BattleDuelLayer::onHttpRequestCompleted));
    }
    
    if(true == gameStart)
    {
        
        BattleStartAction();
    }
}

void BattleDuelLayer::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("battle image download complete");
        
        BattleStartAction();
    }
}

void BattleDuelLayer::BattleStartAction()
{
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(1.5f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleDuelLayer::BattleStart));
    
    this->runAction(CCSequence::actions(delay1, callBack, NULL));    
}

void BattleDuelLayer::BattleStart()
{
    removeLoadingAni();

    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    /*
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    battleLayer = new BattleFullScreen(size, selectedTeam);
    battleLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    battleLayer->setPosition(accp(0.0f, 0.0f));
    battleLayer->SetBattleResult(pInfo->battleResponseInfo);
    battleLayer->InitUI();
    battleLayer->InitGame();
    this->addChild(battleLayer, 100);
    */
    
    pInfo->battleResponseInfo->attackerSkillPoint = pInfo->battleResponseInfo->user_ext_tot;
    pInfo->battleResponseInfo->defenderSkillPoint = pInfo->battleResponseInfo->rival_ext_tot;
    /*
    int rivalHp             = pInfo->battleResponseInfo->rival_max_hp;
    int rivalHpMax          = pInfo->battleResponseInfo->rival_max_hp;
    int rivalAttackPoint    = pInfo->battleResponseInfo->rival_attack_tot;
    int myHp                = pInfo->battleResponseInfo->user_max_hp;
    int myHpMax             = pInfo->battleResponseInfo->user_max_hp;
    int userAttackPoint     = pInfo->battleResponseInfo->user_attack_tot;
      */  
    CCSize size = GameConst::WIN_SIZE;
    battleLayer = new BattleFullScreen(size, PlayerInfo::getInstance()->traceTeam, 0);
    battleLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    battleLayer->setPosition(accp(0.0f, 0.0f));
    battleLayer->SetBattleResult(pInfo->battleResponseInfo);
    battleLayer->InitUI();
    battleLayer->InitGame();
    this->addChild(battleLayer, 100);
    
}

void BattleDuelLayer::ButtonBattle(UserInfo *_user)
{
    //CCLog("Button battle, user name:%s",_user->myNickname.c_str());
    this->removeAllChildrenWithCleanup(true);
    selectedUser = _user;
    InitLayer(1);
}

void BattleDuelLayer::ButtonBack()
{
    MainScene::getInstance()->ShowMainMenu();
    UserStatLayer::getInstance()->ShowMenu();
    InitLayer(1);
}

void BattleDuelLayer::ButtonA(CardInfo* _card){
    //teamEditLayer->AddCardToDeck(_card);
    teamEditLayer->CardSelected(_card);
}

void BattleDuelLayer::CardImagePressed(CardInfo* card)
{
    teamEditLayer->CardSelected(card);
}

void BattleDuelLayer::CardListBackBtnPressed()
{
    ButtonBack();
}

void BattleDuelLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
//    touchStartPoint = location;
//    bTouchPressed = true;
    
    
}

void BattleDuelLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
//    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (nBattleStep == 0){
    }
    else if (nBattleStep == 1){
    }
    else if (nBattleStep == 3){
        if (GetSpriteTouchCheckByTag(this, 30, localPoint)){
            
            //////////////////
            //
            // 결과 배경 음악 정지
            
            resultBG_Off();
            
            ///////////////////
            //
            // 메인 배경 음악 재생
            
            soundMainBG();
            
            soundButton1();
            
            CCLog(" button - back to duel battle list");
            
            //ARequestSender::getInstance()->requestOpponent();
            
            this->removeAllChildrenWithCleanup(true);
            nBattleStep = 0;
            InitLayer(0);
                        
        }
    }
    
    //CCLog(" sublayer y =%f, height=%f",y,a);
}


void BattleDuelLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
}

void BattleDuelLayer::registerUserProfileImg(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        if (response->succeed)
        {
            std::vector<std::string>::iterator iter;
            for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter)
            {
                std::string str = *iter;
                //CCLog("registerUserProfileImg, %s", str.c_str());
                
                size_t found;
                found=str.find_last_of("/\\");
                
                std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + str.substr(found+1);

                CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
                CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
                
                if (pSprFace != NULL)
                {
                    CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
                    float cardScale = (float)80 / aa.height;
                    pSprFace->setScale(cardScale);
                    //regSprite( this, ccp(0.0f, 0.0f), accp(25.0f, 250.0f), pSprFace, 100);
                    regSprite( this, ccp(0.0f, 0.0f), accp(25.0f, 250.0f + 88), pSprFace, 100);
                }
                
            }
        }
        else
        {
            // ERROR
        }
    }
}

void *BattleDuelLayer::threadAction(void *threadid)
{
    
    ARequestSender::getInstance()->requestOpponent();
    
    BattleDuelLayer::getInstance()->schedule(schedule_selector(BattleDuelLayer::threadCallBack));
    BattleDuelLayer::getInstance()->unschedule(schedule_selector(BattleDuelLayer::threadTimeoutCallback));
    pthread_exit(NULL);
    
}

void BattleDuelLayer::initThread()
{
    
    addLoadingAni(this);
    int t = 0;
    pthread_create(&threads, NULL, BattleDuelLayer::threadAction, (void *)t);
    this->schedule(schedule_selector(BattleDuelLayer::threadTimeoutCallback),5);
}

void BattleDuelLayer::threadCallBack()
{
    //CCLog("threadCallBack !!!!!!!!!!");
    InitLayer(0);
    removeLoadingAni(this);
    this->unschedule(schedule_selector(BattleDuelLayer::threadCallBack));
}

void BattleDuelLayer::threadTimeoutCallback()
{
    CCLog("thread time out");
    this->unschedule(schedule_selector(BattleDuelLayer::threadTimeoutCallback));
    //pthread_exit(NULL);
    pthread_kill(threads, 0);
    popupNetworkError("Timeout","", "Sorry, \n please try again.");
}


void BattleDuelLayer::refreshCardList() //float layer_y, int teamID)
{
    cardListLayer->_tableView->RefreshPage();
    /*
    this->removeChild(cardListLayer,true);
    
    cardListLayer = new CardListLayer( CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height - accp(120+90+30)), PlayerInfo::getInstance()->myCards, CALL_CARDLIST_FROM_DECK, this,true, teamID);
    
    cardListLayer->setAnchorPoint(ccp(0,0));
    cardListLayer->setPosition(accp(10,90+90+70)); //y좌표 대충 맞춤.아래의 team edit layer위로만 오도록 맞춤
    cardListLayer->_tableView->setPositionY(layer_y);
    this->addChild(cardListLayer,10);
    */
    
}