//
//  QuestResult.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 11..
//
//

#include "QuestResult.h"
#include "MainScene.h"
#include "ChapterLayer.h"
#include "PopUp.h"
/*
QuestResultLayer::QuestResultLayer(CCSize layersize) : pRewardLayer(NULL), questResult(NULL)
{
    this->setContentSize(layersize);
    YPos = 0;
}

QuestResultLayer::~QuestResultLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void QuestResultLayer::InitUI(ResponseQuestUpdateResultInfo* _questResult)
{
    questResult = _questResult;
    
    CCSize size = this->getContentSize();
    
    YPos = (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_MARGIN - 339 - 115;
    
    const int questID = StageLayer::getInstance()->getQuestID();
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
    string pathBG = CCFileUtils::sharedFileUtils()->getDocumentPath();
    pathBG = pathBG + questInfo->stageBG_L;
    
    CCSprite* pQuestBG = CCSprite::create(pathBG.c_str(), CCRectMake(0,accp(112),accp(500),accp(132)));
    pQuestBG->setAnchorPoint(ccp(0, 0));
    pQuestBG->setScale(1.22f);
    pQuestBG->setPosition(accp(5, YPos + 132));
    this->addChild(pQuestBG, 0);

    CCSprite* pResultBG = CCSprite::create("ui/quest/result_bg.png");
    pResultBG->setAnchorPoint(ccp(0, 0));
    pResultBG->setPosition(accp(0, YPos));
    this->addChild(pResultBG, 0);
    
    CCLabelTTF* pStageLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("questresult_name"), "HelveticaNeue-Bold", 12);
    pStageLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(16, YPos + 300), pStageLabel, 10);
    
    PlayerInfo* pi = PlayerInfo::getInstance();
    CardInfo *card = pi->GetCardInDeck(0, 0, 2);
    
    CardListInfo* cardInfo = NULL;
    
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        cardInfo = FileManager::sharedFileManager()->GetCardInfo(PlayerInfo::getInstance()->TutorialLeaderCardID);
    }
    else
    {
        cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());
    }
    //string path = "ui/cha/";
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
    path += cardInfo->GetSmallBattleImg();

    CCSprite* pChar = CCSprite::create(path.c_str());
    pChar->setAnchorPoint(ccp(0, 0));
    pChar->setScale(0.63f);
    pChar->setPosition(accp(5, YPos + 131));
    this->addChild(pChar, 0);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("퀘스트 진행도", "HelveticaNeue", 12);
    pLabel1->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(20, YPos + 88), pLabel1, 10);

    CCLabelTTF* pLabel2 = CCLabelTTF::create("경험치", "HelveticaNeue", 12);
    pLabel2->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(20, YPos + 53), pLabel2, 10);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("퀘스트 포인트", "HelveticaNeue", 12);
    pLabel3->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(20, YPos + 18), pLabel3, 10);
    
    // --  퀘스트 진행도
    
    ResponseQuestListInfo* questServerlist = ARequestSender::getInstance()->requestQuestList();
    if (atoi(questServerlist->res)!=0){
        popupNetworkError(questServerlist->res, questServerlist->msg, "requestQuestList");
        return;
    }
    
    AQuestInfo* info = NULL;
    
    for(int i=0; i<questServerlist->questList->count(); ++i)
    {
        info = (AQuestInfo*)questServerlist->questList->objectAtIndex(i);
        
        if(StageLayer::getInstance()->getQuestID() == info->questID) break;
    }

    if(questResult)
    {
    // -- 퀘스트 진행도
        QuestProgressRatio = (float)(_questResult->progress-1) / (float)_questResult->progressMax;
        if(QuestProgressRatio < 0.0f)
            QuestProgressRatio = 0.0f;
          
        //if(info->completeCnt > 0 || QuestProgressRatio >= 1.0f)
        if(info->clear == 1 || QuestProgressRatio >= 1.0f)
            QuestProgressRatio = 1.0f;

        increaseQuestProgressRatio = 1.0f / _questResult->progressMax;
        if(increaseQuestProgressRatio + QuestProgressRatio > 1.0f)
            increaseQuestProgressRatio = 0.0f;
        
        pGreenGauge = CCSprite::create("ui/quest/result_gage_a.png");
        pGreenGauge->setAnchorPoint(ccp(0, 0));
        pGreenGauge->setScaleX(QuestProgressRatio);
        pGreenGauge->setPosition(accp(186, YPos + 93));
        this->addChild(pGreenGauge, 0);
        
        // -- 유저 경험치
//        UesrExpRatio = (float)(pi->getXp()) / (float)(pi->getExpCap());
//        
//        if(UesrExpRatio <= 0.0f)
//            UesrExpRatio = 0.0f;
//        
//        increaseUesrExpRatio = float(questResult->exp) / (float)pi->getExpCap();
//        
//        if(increaseUesrExpRatio >= 1.0f)
//            increaseUesrExpRatio = 1.0f;
//        
//        CCLOG("현재 경험치 : %d / 패킷 유저 경험치 : %d / 맥스 경험치 : %d",pi->getXp(), _questResult->exp, pi->getExpCap());

        pRedGauge = CCSprite::create("ui/quest/result_gage_b.png");
        pRedGauge->setAnchorPoint(ccp(0, 0));
        pRedGauge->setScaleX(UesrExpRatio);
        pRedGauge->setPosition(accp(186, YPos + 58));
        this->addChild(pRedGauge, 0);
        
        // -- 퀘스트 포인트
        QuestPointRatio = (float)(pi->getQuestPoint()) / (float)(pi->getMaxQuestPoint());
        
        decreaseQuestPointRatio = (float)(pi->getQuestPoint()-_questResult->user_questPoint) / (float)(pi->getMaxQuestPoint());

        //_questResult->user_questPoint
        pYellowGauge = CCSprite::create("ui/quest/result_gage_c.png");
        pYellowGauge->setAnchorPoint(ccp(0, 0));
        pYellowGauge->setScaleX(QuestPointRatio);
        pYellowGauge->setPosition(accp(186, YPos + 23));
        this->addChild(pYellowGauge, 0);
    }
    else
    {
        QuestProgressRatio = 0.0f;
        increaseQuestProgressRatio = 0.0f;
        
        pGreenGauge = CCSprite::create("ui/quest/result_gage_a.png");
        pGreenGauge->setAnchorPoint(ccp(0, 0));
        pGreenGauge->setScaleX(QuestProgressRatio);
        pGreenGauge->setPosition(accp(186, YPos + 93));
        this->addChild(pGreenGauge, 0);
        
        // -- 유저 경험치
        UesrExpRatio = 0.0f;
        increaseUesrExpRatio = 0.0f;
        
        //CCLOG("현재 경험치 : %d / 패킷 유저 경험치 : %d / 맥스 경험치 : %d",pi->getXp(), _questResult->exp, pi->getExpCap());
        
        pRedGauge = CCSprite::create("ui/quest/result_gage_b.png");
        pRedGauge->setAnchorPoint(ccp(0, 0));
        pRedGauge->setScaleX(UesrExpRatio);
        pRedGauge->setPosition(accp(186, YPos + 58));
        this->addChild(pRedGauge, 0);
        
        // -- 퀘스트 포인트
        QuestPointRatio = 0.0f;
        
        decreaseQuestPointRatio = 0.0f;
        
        //_questResult->user_questPoint
        pYellowGauge = CCSprite::create("ui/quest/result_gage_c.png");
        pYellowGauge->setAnchorPoint(ccp(0, 0));
        pYellowGauge->setScaleX(QuestPointRatio);
        pYellowGauge->setPosition(accp(186, YPos + 23));
        this->addChild(pYellowGauge, 0);
    }
}

void QuestResultLayer::RunAction()
{
    //CCEaseOut *fadeOut = CCEaseOut;
    
    //CCFiniteTimeAction* layerMove = CCMoveTo::actionWithDuration(0.2f, accp(10, -75));

    CCEaseOut *layerMove = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(10, -75)), 2.0f);
    //CCEaseOut* layerMove = CCEaseOut::actionWithDuration(0.2f, accp(10, -75));
    
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    
    CCCallFunc *clearAction = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::ClearAction));
    
    CCCallFunc *greenScale = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::increaseGreenGauge));
    
    CCCallFunc *redScale = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::increaseRedGauge));
    
    CCCallFunc *yellowScale = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::decreaseYellowGauge));
    
    CCCallFunc *rewardInit = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::InitRewardLayer));

    this->runAction(CCSequence::actions(layerMove, delay1, clearAction, delay1, greenScale, delay1, redScale, delay1, yellowScale, rewardInit, NULL));
}

void QuestResultLayer::ClearAction()
{
    CCSprite* pClear = CCSprite::create("ui/quest/result_clear.png");
    pClear->setAnchorPoint(ccp(0.5, 0.5));
    pClear->setScale(5.0f);
    pClear->setPosition(accp(260 + 81, YPos + 152 + 55));
    this->addChild(pClear, 0);

    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    pClear->runAction(actionScale1);

    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.05f);
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestResultLayer::GradeAction));
    this->runAction(CCSequence::actions(delay1, callBack, NULL));
    
    playEffect("audio/clear_01.mp3");
}

void QuestResultLayer::GradeAction()
{
    QuestInfo* questInfo = ChapterLayer::getInstance()->curChapter;
    
    const int comboCount = QuestFullScreen::getInstance()->ultraComboCount;
    
    CCSprite* pGrade = NULL;
    
    if(comboCount >= questInfo->q_ranks)
        pGrade = CCSprite::create("ui/quest/result_grade_s.png");
    else if(comboCount >= questInfo->q_ranka)
        pGrade = CCSprite::create("ui/quest/result_grade_a.png");
    else if(comboCount >= questInfo->q_rankb)
        pGrade = CCSprite::create("ui/quest/result_grade_b.png");
    else if(comboCount >= questInfo->q_rankc)
        pGrade = CCSprite::create("ui/quest/result_grade_c.png");
    else
        pGrade = CCSprite::create("ui/quest/result_grade_d.png");
    
    pGrade->setAnchorPoint(ccp(0.5, 0.5));
    pGrade->setScale(3.0f);
    pGrade->setPosition(accp(260 + 285, YPos + 152 + 95));
    this->addChild(pGrade, 0);
    
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    pGrade->runAction(actionScale2);
}

void QuestResultLayer::increaseGreenGauge()
{
    //increaseQuestProgressRatio;
    CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, QuestProgressRatio + increaseQuestProgressRatio, 1.0f);
    pGreenGauge->runAction(Scale);
}

void QuestResultLayer::increaseRedGauge()
{
    CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, UesrExpRatio + increaseUesrExpRatio, 1.0f);
    pRedGauge->runAction(Scale);
}

void QuestResultLayer::decreaseYellowGauge()
{
    CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, QuestPointRatio - decreaseQuestPointRatio, 1.0f);
    
    //CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, 1.0f);
    pYellowGauge->runAction(Scale);

}

void QuestResultLayer::InitRewardLayer()
{
    CCSize size = this->getContentSize();
    
    pRewardLayer = new QuestRewardLayer(size);
    pRewardLayer->setAnchorPoint(ccp(0, 0));
    pRewardLayer->setPosition(accp(640, 0));
    pRewardLayer->InitUI(questResult);
    this->addChild(pRewardLayer);
    
    pRewardLayer->RunAction();
}
 */
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

QuestRewardLayer::QuestRewardLayer(CCSize layerSize) : actionLevelUP(NULL), questResult(NULL)
{
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    
    bLevelUp = false;
    bUpgradePoint = false;
    
    //InitUI(questResult);
}

QuestRewardLayer::~QuestRewardLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void QuestRewardLayer::InitUI(ResponseQuestUpdateResultInfo* _questResult)
{
    questResult = _questResult;
    
    CCSize size = this->getContentSize();
    
    YPos = (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_MARGIN - 339 - 110;
    
    CCSprite* pRewardBG = CCSprite::create("ui/quest/result_reward_bg.png");
    pRewardBG->setAnchorPoint(ccp(0, 0));
    pRewardBG->setPosition(accp(0, YPos - 324 - 15));
    this->addChild(pRewardBG, 100);

    CCLabelTTF* pLabel1 = CCLabelTTF::create("획득한 코인", "HelveticaNeue", 12);
    pLabel1->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(235, YPos - 324 - 15 + 242 - 4), pLabel1, 10);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("획득한 카드", "HelveticaNeue", 12);
    pLabel2->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0.5, 0), ccp(size.width/2, accp(YPos - 324 - 15 + 242 - 48)), pLabel2, 10);

    // -- 퀘스트 선택, 스테이지 선택 버튼
    pQuestSelectBtn = CCSprite::create("ui/quest/result_btn1.png");
    pQuestSelectBtn->setAnchorPoint(ccp(0, 0));
    pQuestSelectBtn->setPosition(accp(109, YPos - 324 - 15 - 10 - 43));
    pQuestSelectBtn->setTag(1);
    this->addChild(pQuestSelectBtn, 100);

    CCLabelTTF* pLabel5 = CCLabelTTF::create("퀘스트 선택", "HelveticaNeue-Bold", 12);
    pLabel5->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(154, YPos - 324 - 15 - 10 - 38), pLabel5, 110);
    
    pStageSelectBtn = CCSprite::create("ui/quest/result_btn1.png");
    pStageSelectBtn->setAnchorPoint(ccp(0, 0));
    pStageSelectBtn->setPosition(accp(325, YPos - 324 - 15 - 10 - 43));
    pStageSelectBtn->setTag(2);
    this->addChild(pStageSelectBtn, 100);
    
    if(_questResult)
    {
        continueToPlay = (questResult->progress >= questResult->progressMax) ? false : true;

        CCLabelTTF* pLabel4 = CCLabelTTF::create(LocalizationManager::getInstance()->get("questreward_name"), "HelveticaNeue-Bold", 12);
        pLabel4->setColor(COLOR_WHITE);
        registerLabel( this,ccp(0, 0), accp(16, YPos - 324 - 15 + 285), pLabel4, 110);

        char buff[10];
        sprintf(buff, "%d", _questResult->coin);
        CCLabelTTF* pLabel3 = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
        pLabel3->setColor(COLOR_WHITE);
        registerLabel( this,ccp(0, 0), accp(370, YPos - 324 - 15 + 242 - 4), pLabel3, 10);

        cardMaker = new ACardMaker();
        float XPos = (10+116+5)*SCREEN_ZOOM_RATE + 5;
        
        if (_questResult->cards->count() > 0){
            QuestRewardCardInfo *card = (QuestRewardCardInfo*)_questResult->cards->objectAtIndex(0);
            
            const int cardID = card->card_id;
            if(0 != cardID)
            {
                CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(cardID);
                cardMaker->MakeCardThumb(this, cardInfo, ccp(XPos, 162), 178, 1000, 500);
            }
        }
                
        CCLabelTTF* pLabel6 = CCLabelTTF::create((continueToPlay) ? "계속하기" : "스테이지 선택", "HelveticaNeue-Bold", 12);
        pLabel6->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0, 0), accp(((continueToPlay) ? 385 : 355), YPos - 324 - 15 - 10 - 38), pLabel6, 110);
    }
    else
    {
        continueToPlay = true;
        
        CCLabelTTF* pLabel6 = CCLabelTTF::create((continueToPlay) ? "계속하기" : "스테이지 선택", "Thonburi", 13);
        pLabel6->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0, 0), accp(((continueToPlay) ? 385 : 355), YPos - 324 - 15 - 10 - 38), pLabel6, 110);

    }
}

void QuestRewardLayer::InitLevelUplayer()
{
    actionLevelUP = new LevelUpAction(this->getContentSize(), questResult->user_uPnt);
    actionLevelUP->setAnchorPoint(ccp(0.0f, 0.0f));
    actionLevelUP->setTag(9999);
    actionLevelUP->setPosition(accp(0.0f, 0.0f));
    MainScene::getInstance()->addChild(actionLevelUP, 13000);
    

}

void QuestRewardLayer::InitUPointLayer()
{
    MainScene* main = MainScene::getInstance();
    main->initLevelUpLayer();
}

void QuestRewardLayer::RunAction()
{
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    
    CCEaseOut *layerMove = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(0, 0)), 2.0f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestRewardLayer::InitLevelUplayer));
    
    CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestRewardLayer::tutorial));
    
    CCCallFunc *callBack2 = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestRewardLayer::enableMainMenu));
    
    CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestRewardLayer::InitUPointLayer));

    PlayerInfo* pInfo = PlayerInfo::getInstance();
    
    if(questResult)
    {
        if (questResult->user_uPnt != 0)
            bUpgradePoint = true;

        if(bUpgradePoint && PlayerInfo::getInstance()->getTutorialProgress() >= TUTORIAL_DONE)
        {
            if(pInfo->getLevel() != questResult->user_lev)
                this->runAction(CCSequence::actions(delay1, layerMove, delay1, callBack, callBack2, NULL)); // -- 레벨업
            else
                this->runAction(CCSequence::actions(delay1, layerMove, delay1, callBack3, callBack2, NULL)); // -- 스탯 업그레이드
        }
        else
            this->runAction(CCSequence::actions(delay1, layerMove, delay1, callBack1, callBack2, NULL));

        pInfo->setXp(questResult->user_exp);
        pInfo->setLevel(questResult->user_lev);
        pInfo->setUpgradePoint(questResult->user_uPnt);
        pInfo->setCoin(questResult->user_coin);
        pInfo->setRevengePoint(questResult->user_revenge);
        pInfo->setStamina(questResult->user_questPoint);
        pInfo->setBattlePoint(questResult->user_attackPoint);
        //pInfo->setDefensePoint(questResult->user_defensePoint);
        
        UserStatLayer *info = UserStatLayer::getInstance();
        info->refreshUI();
    }
    else
    {
        this->runAction(CCSequence::actions(delay1, layerMove, delay1, callBack1, callBack2, NULL));
    }
    
    //this->runAction(layerMove);
}

void QuestRewardLayer::enableMainMenu()
{
    if (PlayerInfo::getInstance()->getTutorialProgress() < TUTORIAL_DONE)
        return;
    
    MainScene::getInstance()->setEnableMainMenu(true);
}

void QuestRewardLayer::tutorial()
{
    if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 3)
    {
        this->setTouchEnabled(false);
        
        tutorialProgress = 4;
        TutorialPopUp *basePopUp = new TutorialPopUp;
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(basePopUp, 9000);
    }
}

void QuestRewardLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;    
}

void QuestRewardLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    // -- 퀘스트 선택 버튼
    if(GetSpriteTouchCheckByTag(this, 1, localPoint))
    {
        soundButton1();
        
        resultBG_Off();
        
        MainScene* main = MainScene::getInstance();
        main->releaseSubLayers();
        main->initQuestLayer();
    }
    
    // -- 스테이지 선택 버튼
    if(GetSpriteTouchCheckByTag(this, 2, localPoint))
    {
        soundButton1();
        
        resultBG_Off();
        
        if(continueToPlay)
        {
            StageLayer *stageLayer = StageLayer::getInstance();
            if (stageLayer)
            {
                // -- 퀘스트 포인트 체크
                PlayerInfo* pInfo = PlayerInfo::getInstance();
                QuestInfo *questInfo = stageLayer->getQuestInfo(stageLayer->getQuestID());
                if (questInfo == NULL || pInfo == NULL)
                    return;
                
                if(pInfo->myCards->count() >= 60)
                {
                    void* data = (void*)"noReward";
                    stageLayer->initPopUp(data);
                }
                else if(pInfo->getStamina() < questInfo->questBP)
                {
                    void* data = (void*)"charge";
                    stageLayer->initPopUp(data);
                }
                else
                {
                    this->removeAllChildrenWithCleanup(true);
                    stageLayer->InitQuestFullScreen();
                }
            }
        }
        else
        {
            ARequestSender::getInstance()->requestStageList();
            
            //this->removeAllChildrenWithCleanup(true);
            //ChapterLayer* layer = ChapterLayer::getInstance();
            //layer->InitStagelayer(layer->curChapter);
        }
    }
}

void QuestRewardLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}
