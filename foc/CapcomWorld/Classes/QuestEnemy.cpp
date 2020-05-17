//
//  QuestEnemy.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 9..
//
//

#include "QuestEnemy.h"
#include "QuestFullScreen.h"
#include "ShakeAction.h"
#include "StageLayer.h"
#include "PopUp.h"

EnemyLayer *EnemyLayer::instance = NULL;
bool EnemyLayer::ultraComboStarted = false;
bool EnemyLayer::normalComboStarted = false;


EnemyLayer::EnemyLayer(CCSize layerSize) : UltratouchAni(NULL), questResult(NULL)
{
    this->setContentSize(layerSize);
    
    AttackStartTime = 0;
    UpdateStartTime = 0;
    
    TouchCnt = 0;
    LiveTouchMark = 0;
    UltraComboCount = 0;
    
    bTouch          = false;
    bOneExcute      = false;
    bRunAction      = false;
    TouchMarkCreate = true;
    IsUltraMode = false;
    IsUltraModeAction = false;
    
    const int questID = StageLayer::getInstance()->getQuestID();
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
    
    ultraComboStarted = (PlayerInfo::getInstance()->getTutorialProgress() == 12) ? true : false;
    normalComboStarted = (PlayerInfo::getInstance()->getTutorialProgress() == 12) ? true : false;
    /*
    nomalComboCnt       = questInfo->nomalComboCnt;
    ultraRepeat         = questInfo->ultraRepeat;
    ultraComboTimeOut   = questInfo->ultraComboTimeOut;
    comboScaleUPSpeed   = questInfo->comboScaleUPSpeed;
    comboCreateDelay    = questInfo->comboCreateDelay;
    comboNumScreen      = questInfo->comboNumScreen;
    */
    for(int k=0; k<5; ++k)
    {
        pEnemy[k] = NULL;
    }
    
    for(int i=0; i<MAX_TOUCH_MARK; ++i)
    {
        arryTagTable[i] = i;
        bCreated[i] = false;
    }
    
    this->schedule(schedule_selector(EnemyLayer::shakeUpdate), 1.0f/60.0f);
    
    InitUI();
    /*
    soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
    
    if (PlayerInfo::getInstance()->getBgmOption()){
    if(1 == questInfo->level)
        soundBG->playBackgroundMusic("audio/bgm_boss_01.mp3", true);
    else
        soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
    }
    */
    playEffect("audio/ready_01.mp3");
}

EnemyLayer::~EnemyLayer()
{
    CC_SAFE_DELETE(aniFrame);
    CC_SAFE_DELETE(aniFrameTouch);
    
    this->removeAllChildrenWithCleanup(true);
}

void EnemyLayer::InitUI()
{
    const int questID = StageLayer::getInstance()->getQuestID();
    
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
    string pathBG = CCFileUtils::sharedFileUtils()->getDocumentPath();
    pathBG = pathBG + questInfo->stageBG_L;

    CCLog("pathBG :%s", pathBG.c_str());
    
    CCSprite* pBG = CCSprite::create(pathBG.c_str());
    pBG->setAnchorPoint(ccp(0,0));
    pBG->setScale(1.4f);
    pBG->setPosition(accp(0, 368));
    this->addChild(pBG, 50);
    
    CCSprite* pBlueBg = CCSprite::create("ui/quest/quest_cha_bg.png");
    pBlueBg->setAnchorPoint(ccp(0,0));
    pBlueBg->setPosition( ccp(0, 35) );
    this->addChild(pBlueBg, 300);
    
    PlayerInfo* pi = PlayerInfo::getInstance();
    CardListInfo* cardInfo = NULL;
    // -- 튜토리얼 중이면 가지고 있는 카드중 랜덤으로 리더 선택
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        const int cardID = PlayerInfo::getInstance()->TutorialLeaderCardID;
        cardInfo = FileManager::sharedFileManager()->GetCardInfo(cardID);
    }
    else
    {
        CardInfo* card = pi->GetCardInDeck(0, 0, 2);
        
        if (card == NULL){
            CCLog(" team setting error, need leader card");
        }
        
        cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());
    }
    
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path += cardInfo->GetSmallBattleImg();
    
    MyChar = CCSprite::create(path.c_str());
    MyChar->setAnchorPoint(ccp(0,0));
    MyChar->setPosition(accp(-512, 115));
    MyChar->setFlipX(true);
    this->addChild(MyChar, 400);

    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    //float sscale = size.width / 320.f;
    /*
    if(0 != questInfo->questEnemy1[0])
    {
        YPos[0] = (size.height*SCREEN_ZOOM_RATE) -70.0f - (512.0f*1.2f) - 90.0f;

        XPos[0] = 220.0f;
        
        //string path = "ui/cha/";
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=questInfo->questEnemy1;
        pEnemy[0] = CCSprite::create(path.c_str());
        pEnemy[0]->setAnchorPoint(ccp(0,0));
        pEnemy[0]->setScale(1.2f);
        //pEnemy[0]->setPosition(accp(-614, YPos[0]));
        pEnemy[0]->setPosition(accp(640, YPos[0]));
        pEnemy[0]->setTag(200);
        this->addChild(pEnemy[0], 100);
    }
    
    if(0 != questInfo->questEnemy2[0])
    {
        YPos[1] = (size.height*SCREEN_ZOOM_RATE) -25.0f - (512.0f*0.95f) - 90.0f;
        XPos[1] = 172.0f;
        //string path = "ui/cha/";
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=questInfo->questEnemy2;
        pEnemy[1] = CCSprite::create(path.c_str());
        pEnemy[1]->setAnchorPoint(ccp(0,0));
        pEnemy[1]->setScale(0.95f);
        //pEnemy[1]->setPosition(accp(-486, YPos[1]));
        pEnemy[1]->setPosition(accp(640, YPos[1]));
        pEnemy[1]->setTag(201);
        this->addChild(pEnemy[1], 90);
    }
    
    
    if(0 != questInfo->questEnemy3[0])
    {
        
        XPos[2] = -130.0f;
        YPos[2] = (size.height*SCREEN_ZOOM_RATE) -60.0f - (512.0f*1.55f) - 90.0f;
        
        //string path = "ui/cha/";
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=questInfo->questEnemy3;
        pEnemy[2] = CCSprite::create(path.c_str());
        pEnemy[2]->setAnchorPoint(ccp(0,0));
        pEnemy[2]->setScale(1.55f);
        //pEnemy[2]->setPosition(accp(-793, YPos[2]));
        pEnemy[2]->setPosition(accp(640, YPos[2]));
        pEnemy[2]->setTag(202);
        this->addChild(pEnemy[2], 100);
    }

    if(0 != questInfo->questEnemy4[0])
    {
        XPos[3] = -140.0f;
        YPos[3] = (size.height*SCREEN_ZOOM_RATE) -80.0f - (512.0f*1.08f) - 90.0f;
        //string path = "ui/cha/";
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=questInfo->questEnemy4;
        pEnemy[3] = CCSprite::create(path.c_str());
        pEnemy[3]->setAnchorPoint(ccp(0,0));
        pEnemy[3]->setScale(1.08f);
        //pEnemy[3]->setPosition(accp(-553, YPos[3]));
        pEnemy[3]->setPosition(accp(640, YPos[3]));
        pEnemy[3]->setTag(203);
        this->addChild(pEnemy[3], 90);
    }

    if(0 != questInfo->questEnemy5[0])
    {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
        XPos[4] = -205.0f;
        YPos[4] = (size.height*2) -35.0f - (512.0f*0.95f) - 90.0f;
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
        XPos[4] = -205.0f;        
        YPos[4] = (size.height*SCREEN_ZOOM_RATE) -35.0f - (512.0f*0.95f) - 90.0f;
#endif
        //string path = "ui/cha/";
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=questInfo->questEnemy5;
        pEnemy[4] = CCSprite::create(path.c_str());
        pEnemy[4]->setAnchorPoint(ccp(0,0));
        pEnemy[4]->setScale(0.95f);
        //pEnemy[4]->setPosition(accp(-486, YPos[4]));
        pEnemy[4]->setPosition(accp(640, YPos[4]));
        pEnemy[4]->setTag(204);
        this->addChild(pEnemy[4], 80);
    }
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0,0,128,128));
    
    aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);
   
    CCSpriteFrame *aa00 = CCSpriteFrame::create("ui/quest/eff_c01.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa11 = CCSpriteFrame::create("ui/quest/eff_c02.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa22 = CCSpriteFrame::create("ui/quest/eff_c03.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa33 = CCSpriteFrame::create("ui/quest/eff_c04.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa44 = CCSpriteFrame::create("ui/quest/eff_c05.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa55 = CCSpriteFrame::create("ui/quest/eff_c06.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa66 = CCSpriteFrame::create("ui/quest/eff_c07.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa77 = CCSpriteFrame::create("ui/quest/eff_c08.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa88 = CCSpriteFrame::create("ui/quest/eff_c09.png", CCRectMake(0,0,220,220));
    CCSpriteFrame *aa99 = CCSpriteFrame::create("ui/quest/eff_c10.png", CCRectMake(0,0,220,220));
    
    aniFrameTouch = new CCArray();
    aniFrameTouch->addObject(aa00);
    aniFrameTouch->addObject(aa11);
    aniFrameTouch->addObject(aa22);
    aniFrameTouch->addObject(aa33);
    aniFrameTouch->addObject(aa44);
    aniFrameTouch->addObject(aa55);
    aniFrameTouch->addObject(aa66);
    aniFrameTouch->addObject(aa77);
    aniFrameTouch->addObject(aa88);
    aniFrameTouch->addObject(aa99);
    
 //   aniFrameTouch;
 //   CCAnimation *animationTouch;
    // -- 터치마크 좌표 세팅
    float x = 64.0f;
    float y = 80.0f + 368.0f;
    
    for(int i=0; i<MAX_TOUCH_MARK; ++i)
    {
        TouchMarkPos[i].x = x;
        TouchMarkPos[i].y = y;
        
        x+=128.0f;
        
        if(4 == i%5)
        {
            x = 64.0f;
            y += 80.0f;
        }        
    }
     */
}

void EnemyLayer::StopAction5()
{
    //pEnemy[4]->stopActionByTag(5);
    this->setTouchEnabled(true);
    //this->unschedule(schedule_selector(EnemyLayer::update));
}

void EnemyLayer::SlideEnemy()
{
    if(MyChar)
    {
        CCDelayTime *delay = CCDelayTime::actionWithDuration(0.3f);
        CCEaseOut *layerMove = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(128, 115)), 2.0f);
        MyChar->runAction(CCSequence::actions(delay, layerMove, NULL));
    }

    if(pEnemy[4])
    {
        CCEaseOut *layerMove4 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.165f, accp(XPos[4], YPos[4])), 2.0f);
        pEnemy[4]->runAction(layerMove4);
    }
    
    if(pEnemy[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.033f);
        CCEaseOut *layerMove3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.231f, accp(XPos[3], YPos[3])), 2.0f);
        pEnemy[3]->runAction(CCSequence::actions(delay3, layerMove3, NULL));
    }
    
    if(pEnemy[2])
    {
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.165f);
        CCEaseOut *layerMove2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.198f, accp(XPos[2], YPos[2])), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::StopAction5));
        pEnemy[2]->runAction(CCSequence::actions(delay2, layerMove2, callBack, NULL));
    }
    
    if(pEnemy[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.330f);
        CCEaseOut *layerMove1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.099f, accp(XPos[1], YPos[1])), 2.0f);
        pEnemy[1]->runAction(CCSequence::actions(delay1, layerMove1, NULL));
    }
    
    if(pEnemy[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.396f);
        CCEaseOut *layerMove0 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.066f, accp(XPos[0], YPos[0])), 2.0f);
        pEnemy[0]->runAction(CCSequence::actions(delay0, layerMove0, NULL));
    }
}

void EnemyLayer::shakeUpdate(CCTime dt)
{
    /*
    if(false == bOneExcute && true == bTouch)
    {
        pAttackEffect = CCSprite::create("ui/battle/eff_defense.png");
        pAttackEffect->setAnchorPoint(ccp(0 ,0));
        pAttackEffect->setPosition((accp(0, 368.0f)));
        this->addChild(pAttackEffect, 100);

        bOneExcute = true;
        bTouch = false;
    }
*/
    if(TouchCnt >= nomalComboCnt && false == IsUltraModeAction && 12 > PlayerInfo::getInstance()->getTutorialProgress() && ultraComboStarted == false)
    {
        if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 2)
        {
            EnemyLayer::getInstance()->normalComboStarted = false;
            
            tutorialProgress = 3;
            TutorialPopUp *basePopUp = new TutorialPopUp;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            MainScene::getInstance()->addChild(basePopUp, 21000);
        }        
    }
    
    if(TouchCnt >= nomalComboCnt && false == IsUltraModeAction && ultraComboStarted == true)
    {
        TouchCnt = 0;
        
        this->setTouchEnabled(false);
        // 여기로 들어오면 안됨
        //QuestFullScreen* ptr = QuestFullScreen::getInstance();
        //ptr->RunUltra();
        
        IsUltraMode = true;
        IsUltraModeAction = true;
        
        CCDelayTime *delay = CCDelayTime::actionWithDuration(1.5f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::regUltraUpdate));
        this->runAction(CCSequence::actions(delay, callBack, NULL));
        
        --ultraRepeat;
        
        if(ultraRepeat <= 0)
        {
            CCDelayTime *delay1 = CCDelayTime::actionWithDuration(ultraComboTimeOut);
            CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::RunKO));
            this->runAction(CCSequence::actions(delay1, callBack1, NULL));
        }
        else
        {
            CCDelayTime *delay1 = CCDelayTime::actionWithDuration(ultraComboTimeOut);
            CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::RunNormalCombo));
            this->runAction(CCSequence::actions(delay1, callBack1, NULL));
        }
    }
 
    if(true == bOneExcute)
    {
        this->moveSprite(pAttackEffect, 0.0f, 368.0f, 0.415f, this, callfunc_selector(EnemyLayer::RemoveAttackEffect));
    }
}

void EnemyLayer::regUltraUpdate()
{
    this->schedule(schedule_selector(EnemyLayer::UltraUpdate), 1.0f/60.0f);
}

void EnemyLayer::unregUltraUpdate()
{
    this->unschedule(schedule_selector(EnemyLayer::UltraUpdate));
}

void EnemyLayer::UltraUpdate(CCTime dt)
{
    if(true == IsUltraMode)
    {
        if(LiveTouchMark < comboNumScreen)
        {
            if(true == TouchMarkCreate)
            {
                // -- 동시에 나올 수 있는 터치마크 최대 개수
                int stageMaxTouch = comboNumScreen - LiveTouchMark;
                
                // -- 생성되어야 할 터치마크
                int curTouch = rand() % stageMaxTouch;
                
                // -- 최소 한개는 생성
                if(0 == curTouch)   curTouch = 1;
                
                for(int i =0; i<curTouch; ++i)
                {
                    int randIdx = getUncreatedTouchIdx();
                    
                    if(randIdx >= 0)
                    {
                        //CCLog("생성 콤보 태그:  %d", randIdx);
                        
                        CCSprite* pTouchMark = CCSprite::create("ui/quest/combo_mark_05.png");
                        pTouchMark->setTag(randIdx);
                        pTouchMark->setAnchorPoint(ccp(0.5, 0.5));
                        pTouchMark->setScale(0.0f);
                        pTouchMark->setPosition(accp(TouchMarkPos[randIdx].x, TouchMarkPos[randIdx].y));
                        this->addChild(pTouchMark, 550);
                        
                        int *tag = new int(randIdx);
                        //CCLog("콤보 태그:  %d", randIdx);
                        CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(comboScaleUPSpeed, 1.5f);
                        //CCDelayTime *delay1 = CCDelayTime::actionWithDuration(4.0f);
                        CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(EnemyLayer::removeTouchSpr),(void*)tag);
                        pTouchMark->runAction(CCSequence::actions(actionScale1, callBack, NULL));
                        
                        ++LiveTouchMark;
                        
                        bCreated[randIdx] = true;
                    }
                }
                
                TouchMarkCreate = false;
                
                CCDelayTime *CreateDelay = CCDelayTime::actionWithDuration(comboCreateDelay);
                CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::SetTouchCreateTrue));
                this->runAction(CCSequence::actions(CreateDelay, callBack, NULL));
            }
        }
    }
}

int EnemyLayer::getUncreatedTouchIdx()
{
    int randIdx = -1;
    
    srand(time(NULL));
  
    bool loop = false;
    
    for(int k=0; k<MAX_TOUCH_MARK; ++k)
    {
        if(false == bCreated[k])
        {
            loop = true;
            break;
        }
    }
    
    if(loop)
    {
        while(1)
        {
            randIdx = rand() % (MAX_TOUCH_MARK);
            
            if(false == bCreated[randIdx] )
            {
                break;
            }
        }
        
        //CCLog("루프:  %d", randIdx);
    }
    
    return randIdx;
}

void EnemyLayer::SetTouchCreateTrue()
{
    TouchMarkCreate = true;
}

void EnemyLayer::removeTouchSpr(CCNode* sender, void* _tag)
{
    int* tag = (int*)_tag;
    
    //CCLog("삭제 인덱스:  %d", *tag);r

    bCreated[*tag] = false;
    --LiveTouchMark;
    
    this->removeChildByTag(*tag, true);
    
    CC_SAFE_DELETE(tag);
}

void EnemyLayer::RunNormalCombo()
{
    
    // 여기로 들어오면 안됨
    //QuestFullScreen* ptr = QuestFullScreen::getInstance();
    //ptr->decreaseGauge();
    
    LiveTouchMark = 0;
    bRunAction = false;
    IsUltraMode = false;
    IsUltraModeAction = false;
    
    for(int i=0; i<25; ++i)
    {
        this->removeChildByTag(i, true);
    }

    unregUltraUpdate();
}

void EnemyLayer::RunKO()
{
    soundBG->stopBackgroundMusic();

    for(int i=0; i<25; ++i)
    {
        this->removeChildByTag(i, true);
    }
    
    this->unschedule(schedule_selector(EnemyLayer::UltraUpdate));
    
    this->setTouchEnabled(false);
    
    // 여기로 들어오면 안됨
    //QuestFullScreen* ptr = QuestFullScreen::getInstance();
    //ptr->decreaseGauge();
    //ptr->RunKO(UltraComboCount);
}

void EnemyLayer::RunShake()
{
    if(pEnemy[2])
    {
        pEnemy[2]->runAction(CCShake::create(0.2f, 15.8f, XPos[2], YPos[2]));
    }
    
    if(pEnemy[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
        pEnemy[1]->runAction(CCSequence::actions(delay1, CCShake::create(0.2f, 11.5f, XPos[1], YPos[1]), NULL));
        this->runAction(CCSequence::actions(delay1, NULL));
    }
    
    if(pEnemy[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.25f);
        pEnemy[3]->runAction(CCSequence::actions(delay3, CCShake::create(0.2f, 11.5f, XPos[3], YPos[3]), NULL));
        this->runAction(CCSequence::actions(delay3, NULL));
    }
    
    if(pEnemy[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.3f);
        pEnemy[0]->runAction(CCSequence::actions(delay0, CCShake::create(0.2f, 11.5f, XPos[0], YPos[0]), NULL));
        this->runAction(CCSequence::actions(delay0, NULL));
    }
    
    if(pEnemy[4])
    {
        CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.2f);
        pEnemy[4]->runAction(CCSequence::actions(delay4, CCShake::create(0.2f, 11.5f, XPos[4], YPos[4]), NULL));
        this->runAction(CCSequence::actions(delay4, NULL));
    }
}

void EnemyLayer::RunHitnShake()
{
    if(pEnemy[2])
    {
        pEnemy[2]->runAction(CCShake::create(0.2f, 15.8f, XPos[2], YPos[2]));
        
        AniPlay(pHitEffect[2], aniFrame, this, ccp(0, 0), accp(120, -350 + 368.0f * 2/SCREEN_ZOOM_RATE), 3.0f, 102, 500, callfunc_selector(EnemyLayer::RemoveHitEffect2));
        /*
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
        AniPlay(pHitEffect[2], aniFrame, this, ccp(0, 0), accp(120, -350 + 368.0f * 2/SCREEN_ZOOM_RATE), 3.0f, 102, 500, callfunc_selector(EnemyLayer::RemoveHitEffect2));
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
        AniPlay(pHitEffect[2], aniFrame, this, ccp(0, 0), accp(120, -350 + 368.0f * 2.f), 3.0f, 102, 500, callfunc_selector(EnemyLayer::RemoveHitEffect2));
#endif
*/
    }
    
    
    
    if(pEnemy[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
        pEnemy[1]->runAction(CCSequence::actions(delay1, CCShake::create(0.2f, 11.5f, XPos[1], YPos[1]), NULL));
        
        CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::HitPlay0));
        this->runAction(CCSequence::actions(delay1, callBack1, NULL));
    }
    
    if(pEnemy[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.25f);
        pEnemy[3]->runAction(CCSequence::actions(delay3, CCShake::create(0.2f, 11.5f, XPos[3], YPos[3]), NULL));
        
        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::HitPlay3));
        this->runAction(CCSequence::actions(delay3, callBack3, NULL));
    }

    if(pEnemy[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.3f);
        pEnemy[0]->runAction(CCSequence::actions(delay0, CCShake::create(0.2f, 11.5f, XPos[0], YPos[0]), NULL));
        
        CCCallFunc *callBack0 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::HitPlay1));
        this->runAction(CCSequence::actions(delay0, callBack0, NULL));
    }
    
    if(pEnemy[4])
    {
        CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.2f);
        pEnemy[4]->runAction(CCSequence::actions(delay4, CCShake::create(0.2f, 11.5f, XPos[4], YPos[4]), NULL));
        
        CCCallFunc *callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(EnemyLayer::HitPlay4));
        this->runAction(CCSequence::actions(delay4, callBack4, NULL));
    }
   
    // 여기로 들어오면 안됨
    //QuestFullScreen* ptr = QuestFullScreen::getInstance();
    //ptr->decreaseGauge();
}


void EnemyLayer::HitPlay0()
{
    AniPlay(pHitEffect[0], aniFrame, this, ccp(0, 0), accp(370, (-290 + 368.0f * 2/SCREEN_ZOOM_RATE)), 3.0f, 100, 500, callfunc_selector(EnemyLayer::RemoveHitEffect0));
}

void EnemyLayer::HitPlay1()
{
    AniPlay(pHitEffect[1], aniFrame, this, ccp(0, 0), accp(270, (-270 + 368.0f * 2/SCREEN_ZOOM_RATE)), 3.0f, 101, 500, callfunc_selector(EnemyLayer::RemoveHitEffect1));
}

void EnemyLayer::HitPlay2()
{
    AniPlay(pHitEffect[2], aniFrame, this, ccp(0, 0), accp(120, (-350 + 368.0f * 2/SCREEN_ZOOM_RATE)), 3.0f, 102, 500, callfunc_selector(EnemyLayer::RemoveHitEffect2));
}

void EnemyLayer::HitPlay3()
{
    AniPlay(pHitEffect[3], aniFrame, this, ccp(0, 0), accp(-30, -320 + 368.0f * 2/SCREEN_ZOOM_RATE), 3.0f, 103, 500, callfunc_selector(EnemyLayer::RemoveHitEffect3));
}

void EnemyLayer::HitPlay4()
{
    AniPlay(pHitEffect[4], aniFrame, this, ccp(0, 0), accp(-120, (-250 + 368.0f * 2/SCREEN_ZOOM_RATE)), 3.0f, 104, 500, callfunc_selector(EnemyLayer::RemoveHitEffect4));
}

/*
void EnemyLayer::HitPlay0()
{
    AniPlay(pHitEffect[0], aniFrame, this, ccp(0, 0), accp(370, -290 + 368.0f), 3.0f, 100, 500, callfunc_selector(EnemyLayer::RemoveHitEffect0));
}

void EnemyLayer::HitPlay1()
{
    AniPlay(pHitEffect[1], aniFrame, this, ccp(0, 0), accp(270, -270 + 368.0f), 3.0f, 101, 500, callfunc_selector(EnemyLayer::RemoveHitEffect1));
}

void EnemyLayer::HitPlay2()
{
    AniPlay(pHitEffect[2], aniFrame, this, ccp(0, 0), accp(120, -350 + 368.0f), 3.0f, 102, 500, callfunc_selector(EnemyLayer::RemoveHitEffect2));
}

void EnemyLayer::HitPlay3()
{
    AniPlay(pHitEffect[3], aniFrame, this, ccp(0, 0), accp(-30, -320 + 368.0f), 3.0f, 103, 500, callfunc_selector(EnemyLayer::RemoveHitEffect3));
}

void EnemyLayer::HitPlay4()
{
    AniPlay(pHitEffect[4], aniFrame, this, ccp(0, 0), accp(-120, -250 + 368.0f), 3.0f, 104, 500, callfunc_selector(EnemyLayer::RemoveHitEffect4));
}
*/


void EnemyLayer::RemoveHitEffect0()
{
    this->removeChildByTag(100, true);
    bRunAction = false;
}

void EnemyLayer::RemoveHitEffect1()
{
    this->removeChildByTag(101, true);
    bRunAction = false;
}

void EnemyLayer::RemoveHitEffect2()
{
    this->removeChildByTag(102, true);
    bRunAction = false;
}

void EnemyLayer::RemoveHitEffect3()
{
    this->removeChildByTag(103, true);
    bRunAction = false;
}

void EnemyLayer::RemoveHitEffect4()
{
    this->removeChildByTag(104, true);
    bRunAction = false;
}

void EnemyLayer::RenderComboCount()
{
    CCSprite* pComboBG = CCSprite::create("ui/quest/quest_combo_count_bg.png");
    pComboBG->setAnchorPoint(ccp(0.0f, 0.0f));
    pComboBG->setPosition(accp(0.0f, 194.0f));
    this->addChild(pComboBG, 600);
    
    CCSprite* pCombo = CCSprite::create("ui/quest/quest_combo_count.png");
    pCombo->setAnchorPoint(ccp(0.0f, 0.0f));
    pCombo->setPosition(accp(135.0f, 206.0f));
    this->addChild(pCombo, 600);
    
    DrawNumber(UltraComboCount, accp(235.0f, 205.0f));
}

void EnemyLayer::DrawNumber(int _value, CCPoint _pos)
{
    //if(_value < 0 || _value > maxUpgradePoint) return;
    
    ReleaseNumber();
    
    float x = _pos.x*SCREEN_ZOOM_RATE;
    char buffer[3];
    float scale = 2.0f;
    sprintf(buffer, "%d", _value);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        ComboCount[scan] = createNumber(number, accp(x, _pos.y*SCREEN_ZOOM_RATE), scale);
        this->addChild(ComboCount[scan], 2000);
        CCSize size = ComboCount[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale + 2;
        ComboCount[scan]->setPosition(accp(x/2, _pos.y*SCREEN_ZOOM_RATE));
        
        CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.1f, 1.0f);
        ComboCount[scan]->runAction(actionScale2);
    }
}

void EnemyLayer::ReleaseNumber()
{
    if(ComboCount[0])
    {
        this->removeChild(ComboCount[0], true);
        ComboCount[0] = NULL;
    }
    
    if(ComboCount[1])
    {
        this->removeChild(ComboCount[1], true);
        ComboCount[1] = NULL;
    }
    
    if(ComboCount[2])
    {
        this->removeChild(ComboCount[2], true);
        ComboCount[2] = NULL;
    }
}

CCSprite* EnemyLayer::createNumber(int number, CCPoint pos, float scale)
{
    assert(number >= 0 && number < 10);
    char buffer[64];
    sprintf(buffer, "ui/card/card_number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0.0f, 0.0f));
    sprite->setPosition(pos);
    return sprite;
}

void EnemyLayer::RemoveAttackEffect()
{
    this->removeChild(pAttackEffect, true);
    bOneExcute = false;
}

void EnemyLayer::UltraTouchAni(float x, float y)
{
    AniPlay(UltratouchAni, aniFrameTouch, this, ccp(0.5f, 0.5f), accp(x, y), 1.0f, 777, 500, callfunc_selector(EnemyLayer::removeUltraTouchAni));
}

void EnemyLayer::removeUltraTouchAni()
{
    this->removeChildByTag(777, true);
}

void EnemyLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    getUncreatedTouchIdx();
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);

    bTouch = true;
    
    if(false == bRunAction && false == IsUltraMode && normalComboStarted == true)
    {
        if(false == bOneExcute && true == bTouch)
        {
            pAttackEffect = CCSprite::create("ui/battle/eff_defense.png");
            pAttackEffect->setAnchorPoint(ccp(0 ,0));
            pAttackEffect->setPosition((accp(0, 368.0f)));
            this->addChild(pAttackEffect, 100);
            
            bOneExcute = true;
            bTouch = false;
        }

        playEffect("audio/hit_01.mp3");
        
        ++TouchCnt;
        bRunAction = true;
        RunHitnShake(); // 노말 콤보 모드는 히트이펙트, 캐릭터 흔들기 동시에
    }
    
    for(int i=0; i<25; ++i)
    {
        if(GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            if(false == bRunAction && true == IsUltraMode)
            {
                /*
                if(false == bOneExcute && true == bTouch)
                {
                    pAttackEffect = CCSprite::create("ui/battle/eff_defense.png");
                    pAttackEffect->setAnchorPoint(ccp(0 ,0));
                    pAttackEffect->setPosition((accp(0, 368.0f)));
                    this->addChild(pAttackEffect, 100);
                    
                    bOneExcute = true;
                    bTouch = false;
                }
                */
                
                playEffect("audio/hit_01.mp3");
                
                ++UltraComboCount;
                
                RenderComboCount();
                
                RunShake();
                
                CCSprite* comboMark = (CCSprite*)this->getChildByTag(i);
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
                UltraTouchAni(comboMark->getPositionX()*SCREEN_ZOOM_RATE + 100.0f, comboMark->getPositionY()*SCREEN_ZOOM_RATE - 100.0f);
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
                UltraTouchAni(comboMark->getPositionX()*SCREEN_ZOOM_RATE,          comboMark->getPositionY()*SCREEN_ZOOM_RATE);
#endif
                
            }

            this->removeChildByTag(i, true);
            
            --LiveTouchMark;
            bCreated[i] = false;
        }
    }
}

void EnemyLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);    
}

void EnemyLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}
