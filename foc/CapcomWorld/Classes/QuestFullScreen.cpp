//
//  QuestFullScreen.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

/*
#include "QuestFullScreen.h"
#include "QuestLayer.h"
#include "StageLayer.h"
#include "PopUp.h"

QuestFullScreen* QuestFullScreen::instance = NULL;

QuestFullScreen* QuestFullScreen::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

QuestFullScreen::QuestFullScreen(CCSize layerSize)
{
    this->setContentSize(layerSize);
    
    instance = this;
    
    OneTimeInit = false;
    
    rewardCardID = 0;
    
    ultraComboCount = 0;
    
    InitUI();
}

QuestFullScreen::~QuestFullScreen()
{
    this->removeAllChildrenWithCleanup(true);
}

void QuestFullScreen::InitUI()
{
    CCSprite* pBg = CCSprite::create("ui/home/ui_BG.png");
    pBg->setAnchorPoint(ccp(0,0));
    pBg->setPosition(ccp(0, 0) );
    this->addChild(pBg, 0);
    
    CCSprite* pBgUp = CCSprite::create("ui/quest/quest_bg_up.png");
    pBgUp->setAnchorPoint(ccp(0,0));
    pBgUp->setPosition(accp(0, 814) );
    this->addChild(pBgUp, 200);
    
    CCSprite* pBgBottom = CCSprite::create("ui/quest/quest_bg_down.png");
    pBgBottom->setAnchorPoint(ccp(0,0));
    pBgBottom->setPosition(accp(0, 0) );
    this->addChild(pBgBottom, 200);
    
    CCSprite* pGauge = CCSprite::create("ui/quest/quest_gage_bg.png");
    pGauge->setAnchorPoint(ccp(0,0));
    pGauge->setPosition(accp(0, 290) );
    this->addChild(pGauge, 250);

    YellowGauge = CCSprite::create("ui/quest/quest_gage_a.png");
    YellowGauge->setAnchorPoint(ccp(0,0));
    YellowGauge->setPosition(accp(10, 357) );
    GaugeXPos = 10.0f;
    this->addChild(YellowGauge, 300);
    
    RedGauge = CCSprite::create("ui/quest/quest_gage_b.png");
    RedGauge->setAnchorPoint(ccp(0,0));
    RedGauge->setPosition(accp(10, 357) );
    this->addChild(RedGauge, 270);
    
    CCSprite* pGauge1 = CCSprite::create("ui/quest/quest_gage_bg_o.png");
    pGauge1->setAnchorPoint(ccp(0,0));
    pGauge1->setPosition(accp(0, 290) );
    this->addChild(pGauge1, 300);

    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    pEnemyLayer = new EnemyLayer(CCSize(size.width, size.height));
    pEnemyLayer->setAnchorPoint(ccp(0, 0));
    pEnemyLayer->setPosition(accp(0, 0));
    this->addChild(pEnemyLayer, 100);
    pEnemyLayer->SlideEnemy();

    if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 1)
    {
        tutorialProgress = 2;
        TutorialPopUp *basePopUp = new TutorialPopUp;
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        this->addChild(basePopUp, 9000);
    }
}

void QuestFullScreen::decreaseGauge()
{

//    const int questID = StageLayer::getInstance()->getQuestID();
//    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
//    
//    float decPosX = 622.0f / (float)((questInfo->nomalComboCnt + 1) * questInfo->ultraRepeat);
//    
//    GaugeXPos = GaugeXPos - decPosX;
//    
//    CCFiniteTimeAction* actionMove = CCMoveTo::actionWithDuration(0.2f, accp(GaugeXPos, 357.0f));
//    
//    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestFullScreen::decreaseRedGauge));
//    
//    YellowGauge->runAction(CCSequence::actions(actionMove, callBack, NULL));

}

void QuestFullScreen::decreaseRedGauge()
{
    CCFiniteTimeAction* actionMove = CCMoveTo::actionWithDuration(0.4f, accp(GaugeXPos, 357.0f));
    
    //CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestFullScreen::decreaseRedGauge));
    
    RedGauge->runAction(actionMove);
}

void QuestFullScreen::ShowRewardCard()
{
    this->removeChild(pEnemyLayer, true);
    this->removeAllChildrenWithCleanup(true);
    
    const int questID = StageLayer::getInstance()->getQuestID();
    //questResult = ARequestSender::getInstance()->requestUpdateQuestResult(questID, ultraComboCount);
    questResult = ARequestSender::getInstance()->requestUpdateQuestResult(questID,0,0,true);
    
    if(NULL != questResult)
    {
        if (atoi(questResult->res) != 0){
            popupNetworkError(questResult->res, questResult->msg, "requestUpdateQuestResult");
            return;
        }
        
        PlayerInfo* pInfo = PlayerInfo::getInstance();
        pInfo->setXp(questResult->user_exp - questResult->exp);
    }
    
}

void QuestFullScreen::RunKO(int ultraCombo)
{
    ultraComboCount = ultraCombo;
    
    playEffect("audio/ko_01.mp3");
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    //float x = size.width;
    
    CCSprite* Ko = CCSprite::create("ui/quest/quest_ko.png");
    Ko->setAnchorPoint(ccp(0.5, 0.5));
    Ko->setScale(3.0f);
    Ko->setPosition(ccp(size.width/2, accp(600)));
    this->addChild(Ko, 400);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.08f, 1.0f);

    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);

    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.05f, 8.0f, 0.0f);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.0f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestFullScreen::ShowRewardCard));
    
    Ko->runAction(CCSequence::actions(actionScale1, delay1, actionScale2, delay2, callBack, NULL));
}

void QuestFullScreen::RunUltra()
{
    playEffect("audio/ultracombo_01.mp3");
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    //float screenRate = size.width / 320.0f; //
    
    CCSprite* pUltra = CCSprite::create("ui/quest/quest_combo_notice.png"); // 350 * 146
    int image_w = 350;
    
    
    pUltra->setAnchorPoint(ccp(0.5f,0));
    pUltra->setPosition(ccp(-(image_w/2), accp(480)));
    this->addChild(pUltra, 450);
    
    CCEaseOut *layerMove1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.1f, ccp(size.width/2, accp(480))), 2.0f);
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.8f);
    CCEaseOut *layerMove2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.1f, ccp(size.width+image_w/2, accp(480))), 2.0f);
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(QuestFullScreen::EnemyLayerTouchTrue));
    pUltra->runAction(CCSequence::actions(layerMove1, delay1, layerMove2, callBack, NULL));

}

void QuestFullScreen::EnemyLayerTouchTrue()
{
    pEnemyLayer->setTouchEnabled(true);
}

void QuestFullScreen::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if(!OneTimeInit)
    {
        CCLOG("결과화면");
        // 이쪽으로 들어오면 안됨
    
        //QuestLayer* ptr = QuestLayer::getInstance();
        //ptr->Result(questResult);

        OneTimeInit = true;
    }
}

void QuestFullScreen::CloseDetailView()
{
    //RestoreZProirity(this);
    
    this->removeChild(_cardDetailView, true);
    
    if(!OneTimeInit)
    {
        CCLOG("결과화면");
        // 이쪽으로 들어오면 안됨 
        
        //QuestLayer* ptr = QuestLayer::getInstance();
        //ptr->Result(questResult);
        
        OneTimeInit = true;
    }
    //MainScene::getInstance()->removeChild(cardDetailViewLayer,true);
    
    //this->setTouchEnabled(true);
    
}
*/
////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////

