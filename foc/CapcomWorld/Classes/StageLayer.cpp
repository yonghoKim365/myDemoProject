//
//  StageLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

#include "StageLayer.h"
#include "PopUp.h"
//#include "StageSubLayer.h"
#include "TraceLayer.h"


#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

StageLayer* StageLayer::instance = NULL;

StageLayer* StageLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

StageLayer::StageLayer()// : pQuestFullScreen(NULL) //, pQuestLayer(NULL) //, cardMaker(NULL)
{
//    soundMainBG();
    
    instance = this;
    
    UnlockStartYPos = 0.0f;
    
    this->setTouchEnabled(true);
    //this->setClipsToBounds(true);
    bTouchMove = false;
}

StageLayer::~StageLayer()
{
    //CC_SAFE_DELETE(cardMaker);
    
    this->removeAllChildrenWithCleanup(true);
}

void StageLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
}

void StageLayer::SetStageData(CCArray *stageList, CCArray* UnlockStage, ResponseQuestListInfo* stageServerList)
{
    StageList = stageList;
    UnlockStageList = UnlockStage;
    questListFromServer = stageServerList;
}

int StageLayer::GetCountOfStage() const
{
    return StageList->count();
}

void StageLayer::InitUI()
{
    const int STAGE_BACK_BTN_SPACE  = 50;
    const int STAGE_BACK_BTN_MARGIN = 10;
    
    CCSize size = this->getContentSize();
    
//    StartYPos = size.height - MAIN_LAYER_TOP_MARGIN - STAGE_HEIGHT + 24;
    
    UnlockStartYPos = size.height - MAIN_LAYER_TOP_MARGIN - STAGE_HEIGHT + 24 - STAGE_BACK_BTN_SPACE - STAGE_BACK_BTN_MARGIN;
    
    CCMenuItemImage *pSprBtnAtTop = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(StageLayer::BackBtnCallback));
    pSprBtnAtTop->setAnchorPoint( ccp(0,0));
    pSprBtnAtTop->setPosition( ccp(0,0));//size.width/5 * 0,0));
    pSprBtnAtTop->setTag(0);
    
    CCMenu* pMenu = CCMenu::create(pSprBtnAtTop, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( accp(10, size.height-STAGE_BACK_BTN_SPACE));
    pMenu->setTag(199);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기"   , "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0.5), accp(320, size.height-25), pLabel1, 100);
    
/*
 * ??∏¥ ?§Ì??¥Ï??§Ï? Î≥¥Ï?Ïß????
 
    // -- ???¬•?????? ???????¬ß??????? ?¬•?????? ???????¬ß??.
    const int StageCount = StageList->count();
    
    for(int i=0; i<StageCount; ++i)
    {
        QuestInfo* tempStage = (QuestInfo*)StageList->objectAtIndex(i);
        MakeLockStageCell(tempStage, i);
        //MakeStageCell(tempStage, i);
    }
 */
    if (atoi(questListFromServer->res) != 0){
        popupNetworkError(questListFromServer->res, questListFromServer->msg, "requestQuestList");
        return;
    }
    
    const int UnlockStageCount = UnlockStageList->count();
    
    for(int k=UnlockStageCount-1; k>-1; --k)
    {
        QuestInfo* tempStage = (QuestInfo*)StageList->objectAtIndex(k);
        MakeStageCell(tempStage, k);
    }

    if (UnlockStageCount > 3)
    {
        CCMenuItemImage *pSprBtnAtBottom = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(StageLayer::BackBtnCallback));
        pSprBtnAtBottom->setAnchorPoint( ccp(0,0));
        pSprBtnAtBottom->setPosition( ccp(0,0));//size.width/5 * 0,0));
        pSprBtnAtBottom->setTag(0);
        
        CCMenu* pMenu = CCMenu::create(pSprBtnAtBottom, NULL);
        
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( accp(10,23) );
        pMenu->setTag(199);
        
        this->addChild(pMenu, 100);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기"   , "HelveticaNeue-Bold", 12);
        pLabel1->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0.5,0.5), accp(320,48), pLabel1, 100);
    }
}

/*
void StageLayer::MakeLockStageCell(QuestInfo* pStage, int tag)
{
    // -- ?¬ß???¬•?? ??????§Œ?    CCSprite* pStageBG = CCSprite::create("ui/quest/quest_stage_bg_lock.png");
    pStageBG->setAnchorPoint(ccp(0,0));
    pStageBG->setPosition( accp(10, StartYPos) );
    this->addChild(pStageBG, 0);
    
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(pStage->questID);
    string pathBG = "ui/main_bg/";
    pathBG = pathBG + questInfo->stageBG_s;
    
    // -- ?¬ß???¬•?? ????¬•?????    CCSprite* pStageImg = CCSprite::create(pathBG.c_str());
    pStageImg->setAnchorPoint(ccp(0,0));
    pStageImg->setPosition( accp(26, StartYPos + 215) );
    this->addChild(pStageImg, 0);
    
    QuestInfo* tempStage = (QuestInfo*)StageList->objectAtIndex(tag);
    char buffer[32];
    sprintf(buffer, "%d_name", tempStage->questID);
    // -- ?¬ß???¬•?? ?????    std::string text = LocalizationManager::getInstance()->get(buffer);
    text = text + " ";
    CCLabelTTF* pStageTitle = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pStageTitle->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(215, StartYPos + 334), pStageTitle, 10);
    
    // ?¬ß???¬•?? ?¬¥??
    char buff[3];
    sprintf(buff, "%d", tag+1);
    
    string strStage = "STAGE";
    strStage = strStage + " " + buff;
    
    CCLabelTTF* pStageNumber = CCLabelTTF::create(strStage.c_str(), "HelveticaNeue-Bold", 12);
    pStageNumber->setColor(COLOR_GRAY);
    registerLabel( this,ccp(0, 0), accp(40, StartYPos + 335), pStageNumber, 10);

    // -- ?¬ß???¬•?? ?¬ß??
    sprintf(buffer, "%d_msg", tempStage->questID);
    text = LocalizationManager::getInstance()->get(buffer);
    CCLabelTTF* pStageDesc = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 11);
    pStageDesc->setColor(COLOR_GRAY);
    pStageDesc->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(235, (text.find("\n") < text.length()) ? StartYPos + 254 : StartYPos + 283), pStageDesc, 10);
    
    // -- QBP
    CCLabelTTF* pQBP = CCLabelTTF::create(LocalizationManager::getInstance()->get("questpoint_msg"), "HelveticaNeue", 11);
    pQBP->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(235, StartYPos + 223), pQBP, 10);
    
    char buff1[5];
    sprintf(buff1, "%d", pStage->questBP);
    
    CCLabelTTF* pQBP1 = CCLabelTTF::create(buff1, "HelveticaNeue-Bold", 11);
    pQBP1->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(418, StartYPos + 223), pQBP1 , 10);
    
    // -- ?????????
    CCSprite* pQuestBtn = CCSprite::create("ui/quest/quest_btn_lock.png");
    pQuestBtn->setAnchorPoint(ccp(0,0));
    pQuestBtn->setPosition( accp(520, StartYPos + 227));
    this->addChild(pQuestBtn, 15);
    
    CCLabelTTF* pQuestLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("questlock_btn"), "HelveticaNeue-Bold", 12);
    pQuestLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(542, StartYPos + 250), pQuestLabel , 16);
    
    StartYPos = StartYPos - STAGE_HEIGHT - 10;
}
 */

void StageLayer::BackBtnCallback(CCObject* pSender)
{
    soundButton1();
    
    StageLayer::getInstance()->removeAllChildrenWithCleanup(true);
    DojoLayerQuest::getInstance()->InitUI();
}

void StageLayer::MakeStageCell(QuestInfo* pStage, int tag)
{
    AQuestInfo* info = getAQuestInfo(pStage->questID);
    
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(pStage->questID);
    
    //CCLog(" UnlockStartYPos :%f", UnlockStartYPos);
    
    // -- ?¬ß???¬•?? ??????§Œ?
    CCSprite* pStageBG = NULL;
    if(1 == questInfo->level)
        pStageBG = CCSprite::create("ui/quest/quest_stage_bg_boss.png");
    else
        pStageBG = CCSprite::create("ui/quest/quest_stage_bg.png");
    
    pStageBG->setAnchorPoint(ccp(0,0));
    pStageBG->setPosition( accp(10, UnlockStartYPos) );
    this->addChild(pStageBG, 100);
    

    ////////////////////////////////////////////////
    string pathBG = "ui/main_bg/";
    pathBG = pathBG + questInfo->stageBG_s;
    
    //
    CCSprite* pStageImg = CCSprite::create(pathBG.c_str());
    pStageImg->setAnchorPoint(ccp(0,0));
    pStageImg->setPosition( accp(26, UnlockStartYPos + 215 - 180) );
    this->addChild(pStageImg, 100);
    
    
    if (info != NULL && info->progress >= 100 && info->clear == 1){
        CCSprite* pStageClear = CCSprite::create("ui/quest/quest_clear.png");
        pStageClear->setAnchorPoint(ccp(0,0));
        pStageClear->setPosition( accp(26, UnlockStartYPos + 215 - 180) );
        this->addChild(pStageClear, 100);
    }
     
    /////////////////////////////////////////////////
  
    QuestInfo* tempStage = (QuestInfo*)StageList->objectAtIndex(tag);
    char buffer[64];
    sprintf(buffer, "%d_name", tempStage->questID);
    // -- ?§Ì??¥Ï? ?????
    std::string text = LocalizationManager::getInstance()->get(buffer);
    text = text + " ";
    CCLabelTTF* pStageTitle = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pStageTitle->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(215, UnlockStartYPos + 334-180), pStageTitle, 110);
    
    // ?¬ß???¬•?? ?¬¥??
    char buff[3];
    sprintf(buff, "%d", tag+1);
    
    string strStage = "STAGE";
    strStage = strStage + " " + buff;
    
    CCLabelTTF* pStageNumber = CCLabelTTF::create(strStage.c_str(), "HelveticaNeue-Bold", 12);
    pStageNumber->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(40, UnlockStartYPos + 335-180), pStageNumber, 110);

/*
    sprintf(buffer, "%d_msg", tempStage->questID);
    //CCLog("-----------buffer:%s", buffer);
    text = LocalizationManager::getInstance()->get(buffer);
    //CCLog("-----------text $s", text.c_str());
    CCLabelTTF* pStageDesc = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 11);
    pStageDesc->setColor(COLOR_GRAY);
    pStageDesc->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(235, (text.find("\n") < text.length()) ? UnlockStartYPos + 254 : UnlockStartYPos + 283-180), pStageDesc, 110);
*/    
    // -- QBP
    /*
    CCLabelTTF* pQBP = CCLabelTTF::create(LocalizationManager::getInstance()->get("questpoint_msg"), "HelveticaNeue", 11);
    pQBP->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(235, UnlockStartYPos + 223-180), pQBP, 110);

    char buff1[5];
    sprintf(buff1, "%d", pStage->questBP);
    CCLabelTTF* pQBP1 = CCLabelTTF::create(buff1, "HelveticaNeue-Bold", 11);
    pQBP1->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(418, UnlockStartYPos + 223-180), pQBP1 , 110);
    */
    // -- ?????????
    CCSprite* pQuestBtn = CCSprite::create("ui/quest/quest_btn_a1.png");
    pQuestBtn->setAnchorPoint(ccp(0,0));
    pQuestBtn->setPosition( accp(520, UnlockStartYPos + 227-180));
    pQuestBtn->setTag(tag);
    this->addChild(pQuestBtn, 111);
    
    CCLabelTTF* pQuestLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("stage_btn"), "HelveticaNeue-Bold", 12);
    pQuestLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(542, UnlockStartYPos + 250-180), pQuestLabel , 112);
    
    float QuestProgress = 0.0f;
    
    if(NULL == info)
        QuestProgress = 0.0f;
    else if(info->clear == 1)
        QuestProgress = 1.0f;
    else
        QuestProgress = (float)info->progress / (float)100;
    
    if(QuestProgress > 1.0f)
        QuestProgress = 1.0f;
    
    CCSprite* pGauge = CCSprite::create("ui/quest/gage_quest.png");
    pGauge->setAnchorPoint(ccp(0,0));
    
    const float ratio = QuestProgress;
    pGauge->setScaleX(ratio);

    pGauge->setPosition( accp(28, UnlockStartYPos + 196-180));

    this->addChild(pGauge, 100);

    UnlockStartYPos = UnlockStartYPos - STAGE_HEIGHT - 10;
}

void StageLayer::initPopUp(void *data)
{
    this->setTouchEnabled(false);

    basePopUp = new QuestPopUp();
    basePopUp->InitUI(data);
    //basePopUp->setTag(999);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    basePopUp->setPosition(accp(0.0f, -this->getPositionY() * SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 3100);
}

void StageLayer::removePopUp()
{
    this->setTouchEnabled(true);
    
    if(basePopUp)
    {
        //this->removeChildByTag(999, true);
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }
}

void StageLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI ?¬¢?????GL?¬¢???¬∞?????§Œ?¬©????
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    const int StageCount = StageList->count();
    
    for(int i=0; i<StageCount; ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            soundButton1();
            
            ChangeSpr(this, i, "ui/quest/quest_btn_a2.png", 100);
        }
    }
    
    touchStartPoint = location;
    bTouchPressed = true;

    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void StageLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI ?¬¢?????GL?¬¢???¬∞?????§Œ?¬©????
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float y = this->getPositionY();
    
#if (1)
    if (moving == true)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos > 0)
            endPos = 0;
        else if (endPos < LayerStartPos)
            endPos = LayerStartPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    
    const int StageCount = StageList->count();

    for(int i=0; i<StageCount; ++i)
    {
        ChangeSpr(this, i, "ui/quest/quest_btn_a1.png", 100);

        if(false == bTouchMove)
        {
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                                
                QuestInfo* questInfo =  (QuestInfo*)StageList->objectAtIndex(i);
                QuestID = questInfo->questID;
            
                AQuestInfo* info = getAQuestInfo(questInfo->questID);
                qProgress = 0;
                if (info != NULL) {
                    qProgress = info->progress;
                }
                
                InitTraceLayer(questInfo->questID, qProgress);//info->progress);
                /////////////////////////////////////////////////
                /*
                soundButton2();
                
                bool possibleQuest = true;
                //bool stageClear = true;
                
                soundButton1();
                
                QuestInfo* questInfo =  (QuestInfo*)StageList->objectAtIndex(i);
                
                CCLOG("??????¬ß???¬•?? : %d", questInfo->questID);

                // -- ??????¬®??????§¬•√?
                PlayerInfo* pInfo = PlayerInfo::getInstance();
                
                if(pInfo->getQuestPoint() < questInfo->questBP)
                {
                    CCLog("??????¬®???????¬∞¬±");
                    
                    void* data = (void*)"charge";
                    
                    initPopUp(data);
                    
                    possibleQuest = false;
                }
                
                // -- ?¬∂¬®?? ???¬∂????¬ß?? ??§¬•√?
                CardInfo *card = pInfo->GetCardInDeck(0, 0, 2);
                
                if(card == NULL && true == possibleQuest && 12 == PlayerInfo::getInstance()->getTutorialProgress())
                {
                    CCLog("?¬∂¬®???¬∫?¬∂???¬ß?? ???");
                    
                    initPopUp(NULL);
                    
                    possibleQuest = false;
                }

                if(pInfo->myCards->count() >= 60 && true == possibleQuest)
                {
                    //popupOk(LocalizationManager::getInstance()->get("no_reward_card"));
                    //possibleQuest = false;
                    
                    void* data = (void*)"noReward";
                    QuestID = questInfo->questID;
                    initPopUp(data);
                    
                    possibleQuest = false;
                }

                // -- ?¬®?¬∂¬®?¬•?? ?¬ß???¬•?????? ?¬®?? ??§¬•√?
                AQuestInfo* info = getAQuestInfo(questInfo->questID);
                if(info && info->completeCnt > 0 && true == possibleQuest)
                {
                    void* data = (void*)"replay";
                    QuestID = questInfo->questID;
                    initPopUp(data);
                    
                    possibleQuest = false;
                }
                
                if(possibleQuest)
                {
                    this->setTouchEnabled(false);
                    
                    this->removeAllChildrenWithCleanup(true);
                    
                    QuestID = questInfo->questID;
                    
                    InitQuestFullScreen();
                    
                }
                 */
                 //////////////////////////////////////////
            }
        }
    }
    
    bTouchMove = false;
}

void StageLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("?¬ß???¬•?? ??????¬¢?? %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
    //bTouchMove = true;
/*
    float distance = fabs(startPosition.y - location.y);
    
    if (distance > 5.0f)
        bTouchMove = true;

    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
    if (distance < 15.0f && timeDelta > 50.0f)
    {
        moving = false;
        startPosition = location;
        startTime = currentTime;
    }
    else if (distance > 5.0f)
        moving = true;
    if (moving)
    {
        distance = fabs(lastPosition.y - location.y);
        timeDelta = (currentTime.tv_usec - lastTime.tv_usec) / 1000.0f + (currentTime.tv_sec - lastTime.tv_sec) * 1000.0f;
        if (distance < 15.0f && timeDelta > 50.0f)
        {
            moving = false;
            startPosition = location;
            startTime = currentTime;
        }
    }
    
    lastPosition = location;
    lastTime = currentTime;
 */
}

void StageLayer::scrollingEnd()
{
    this->stopAllActions();
}

void StageLayer::InitQuestFullScreen()
{
    //this->setClipsToBounds(false);
    this->removeAllChildrenWithCleanup(true);
    this->setPosition(accp(0, 0));
    
    QuestLoad();
}

void StageLayer::QuestLoad()
{
    MainScene::getInstance()->HideMainMenu();
    MainScene::getInstance()->setEnableMainMenu(false);
    UserStatLayer::getInstance()->HideMenu();
    addLoadingAni();
    
    bool gameStart= true;
    //*/
    // -- ????????¬∂????¬•??????¬∞??
    
    
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(QuestID);
    
    FileManager* fmanager = FileManager::sharedFileManager();
    std::string basePathL = FOC_IMAGE_SERV_URL;
    basePathL.append("images/cha/cha_l/");
    
    std::string basePathS  = FOC_IMAGE_SERV_URL;
    basePathS.append("images/cha/cha_s/");
    
    std::string basePathBG  = FOC_IMAGE_SERV_URL;
    basePathBG.append("images/bg/");

    std::vector<std::string> downloads;
    /*
    if(0 != questInfo->questEnemy1[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy1.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathL + questInfo->questEnemy1;
            downloads.push_back(downPath);
        }
    }

    if(0 != questInfo->questEnemy2[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy2.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathL + questInfo->questEnemy2;
            downloads.push_back(downPath);
        }
    }
    */
    if(0 != questInfo->questEnemy3[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy3.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathL + questInfo->questEnemy3;
            downloads.push_back(downPath);
        }
    }
    /*
    if(0 != questInfo->questEnemy4[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy4.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathL + questInfo->questEnemy4;
            downloads.push_back(downPath);
        }
    }
    
    if(0 != questInfo->questEnemy5[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy5.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathL + questInfo->questEnemy5;
            downloads.push_back(downPath);
        }
    }
    */
    if(0 != questInfo->stageBG_L.c_str())
    {
        if(!fmanager->IsFileExist(questInfo->stageBG_L.c_str()))
        {
            gameStart = false;
            
            string downPath = basePathBG + questInfo->stageBG_L;
            downloads.push_back(downPath);
        }
    }

    PlayerInfo* pi = PlayerInfo::getInstance();
    CardInfo* card = NULL;
    // -- ????¬∂¬®?? ?¬ß???¬©??????????? ??¬•???¬ß?????¬∫?? ?¬∂¬®?? ???
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        const int myCardCount = pi->myCards->count();
        
        const int index = rand()%myCardCount;
        
        card = (CardInfo*)pi->myCards->objectAtIndex(index);
    }
    else
    {
        card = pi->GetCardInDeck(0, 0, 2);
        
        if (card == NULL){
            CCLog(" team setting error, need leader card");
        }
    }
    
    if(card)
    {
        PlayerInfo::getInstance()->TutorialLeaderCardID = card->getId();
        
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());

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
        requestor->addDownloadTask(downloads, this, callfuncND_selector(StageLayer::onHttpRequestCompleted));
    }
    
    //*///
    if(true == gameStart)
    {
        QuestStartAction();
    }
}

void StageLayer::ReleaseAndInitQuestFullScreen()
{
    this->setTouchEnabled(false);
    
    this->removeAllChildrenWithCleanup(true);
    InitQuestFullScreen();
}

void StageLayer::QuestStart()
{
    /*
    removeLoadingAni();
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    pQuestLayer = new QuestLayer(size);
    
    pQuestLayer->setAnchorPoint(ccp(0, 0));
    
    pQuestLayer->setPosition(accp(0, 0));
    
    this->addChild(pQuestLayer, 100);
    
    pQuestLayer->InitUI();
     */
}

void StageLayer::QuestStartAction()
{
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(1.5f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(StageLayer::QuestStart));
    
    this->runAction(CCSequence::actions(delay1, callBack, NULL));
}

void StageLayer::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("quest image download complete");
        
        QuestStartAction();
    }
    
}

void StageLayer::onHttpRequestCompletedForTrace(cocos2d::CCObject* pSender, void* data)
{
    HttpResponsePacket* response = (HttpResponsePacket* )data;
    if (response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLog("trace image download complete");
        removePageLoading();
        
        TraceLayer *traceLayer = new TraceLayer(QuestID, qProgress);
        traceLayer->setAnchorPoint(ccp(0,0));
        traceLayer->setPosition(accp(0,-MAIN_LAYER_BTN_HEIGHT));
        this->addChild(traceLayer, 10000);
    }
}

AQuestInfo *StageLayer::getAQuestInfo(int questID)
{
    AQuestInfo* info = NULL;
    
    for(int i=0; i<questListFromServer->questList->count(); ++i)
    {
        info = (AQuestInfo*)questListFromServer->questList->objectAtIndex(i);
        
        //CCLog("info->questID :%d", info->questID);
        
        if(questID == info->questID)
            break;
    }
    
    if(info)
    {
        if(questID != info->questID)
            info = NULL;
    }
    
    return info;
}

QuestInfo *StageLayer::getQuestInfo(int questID)
{


    for (int scan = 0;scan < StageList->count();scan++)
    {
        QuestInfo* questInfo =  (QuestInfo*)StageList->objectAtIndex(scan);
        if (questInfo->questID == questID)
            return questInfo;
    }
    return NULL;
}


void StageLayer::InitTraceLayer(int questID, int questProgress)
{
    MainScene::getInstance()->HideMainMenu();
    MainScene::getInstance()->removeUserStatLayer();
    
    this->stopAllActions();
    this->setTouchEnabled(false);
    this->removeAllChildrenWithCleanup(true);
    
    this->lastLayerY = this->getPositionY();
    this->setPosition(accp(0,0));
    
    char buffer[64];
    sprintf(buffer, "%d_name", questID);
    std::string text = LocalizationManager::getInstance()->get(buffer);
    
    /////////////////////////////////////
    //
    // 추적 레이어 초기화 및 배경 이미지 다운로드

    addPageLoading();
    bool startTrace = true;
    
    FileManager* fManager = FileManager::sharedFileManager();
    std::vector<std::string> downloads;

    // 퀘스트 배경
    Quest_Data* questInfo = fManager->GetQuestInfo(questID);
    std::string bgImgPath = FOC_IMAGE_SERV_URL;
    bgImgPath.append("images/bg/");
    
    if (!fManager->IsFileExist(questInfo->stageBG_L.c_str()))
    {
        startTrace = false;
        string downPath = bgImgPath + questInfo->stageBG_L;
        downloads.push_back(downPath);
    }
    
    // 적 캐릭터
    CCArray* npcList = MainScene::getInstance()->getQuestNpc(questID);
    std::string enemyImgPath = FOC_IMAGE_SERV_URL;
    enemyImgPath.append("images/cha/cha_l/");
    
    for (int i=0; i<npcList->count(); i++)
    {
        CCInteger* nCode = (CCInteger* )npcList->objectAtIndex(i);
        NpcInfo* npc = MainScene::getInstance()->getNpc(nCode->getValue());
        CCLog("Npc Image List for this Stage : %s", npc->npcImagePath);
        if (!fManager->IsFileExist(npc->npcImagePath))
        {
            startTrace = false;
            string downPath = enemyImgPath + npc->npcImagePath;
            downloads.push_back(downPath);
            CCLog("Download Npc Image for this Stage : %s", downPath.c_str());
        }        
    }
    
    if (PlayerInfo::getInstance()->GetCardInDeck(0, 0, 2) == NULL){
        removePageLoading();
        TraceLayer* traceLayer = new TraceLayer(questID, questProgress);
        traceLayer->setAnchorPoint(ccp(0.0f, 0.0f));
        traceLayer->setPosition(accp(0.0f, -MAIN_LAYER_BTN_HEIGHT));
        this->addChild(traceLayer, 10000);
        return;
    }
    
    // 리더 카드 캐릭터
    CardListInfo* charInfo = fManager->GetCardInfo(PlayerInfo::getInstance()->GetCardInDeck(0, 0, 2)->cardId);
    std::string charImgPath = FOC_IMAGE_SERV_URL;
    charImgPath.append("images/cha/cha_s/");

    if (!fManager->IsFileExist(charInfo->GetSmallBattleImg()))
    {
        startTrace = false;
        string downPath = charImgPath + charInfo->GetSmallBattleImg();
        downloads.push_back(downPath);
    }
    
    // 이미지 유무를 체크하고 없으면 다운로드
    if (false == startTrace)
    {
        CCHttpRequest* requestActor = CCHttpRequest::sharedHttpRequest();
        requestActor->addDownloadTask(downloads, this, callfuncND_selector(StageLayer::onHttpRequestCompletedForTrace));
    }
    else
    {
        CCLog("downloading trace images is completed!");
        removePageLoading();
        
        TraceLayer* traceLayer = new TraceLayer(questID, questProgress);
        traceLayer->setAnchorPoint(ccp(0.0f, 0.0f));
        traceLayer->setPosition(accp(0.0f, -MAIN_LAYER_BTN_HEIGHT));
        this->addChild(traceLayer, 10000);
    }
/*
    // 다운로드 받지 않고 로컬로 테스트

    //TraceLayer *traceLayer = new TraceLayer(questID, questProgress);//, text.c_str());
    TraceLayer *traceLayer = new TraceLayer(questID, questProgress);
    traceLayer->setAnchorPoint(ccp(0,0));
    traceLayer->setPosition(accp(0,-MAIN_LAYER_BTN_HEIGHT));
    this->addChild(traceLayer, 10000);
*/

/*
    TraceTestLayer *stageSubLayer = new TraceTestLayer(questID, questProgress);
    stageSubLayer->setAnchorPoint(ccp(0,0));
    stageSubLayer->setPosition(accp(0,-MAIN_LAYER_BTN_HEIGHT));
    this->addChild(stageSubLayer, 10000);
*/
}