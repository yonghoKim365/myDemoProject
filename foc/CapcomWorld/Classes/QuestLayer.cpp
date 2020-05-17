//
//  QuestLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 12..
//
//
/*
#include "QuestLayer.h"
#include "MainScene.h"

QuestLayer* QuestLayer::instance = NULL;

QuestLayer* QuestLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}


QuestLayer::QuestLayer(CCSize layerSize) : pQuestFullScreen(NULL)
{
    this->setContentSize(layerSize);
    
    instance = this;
}

QuestLayer::~QuestLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void QuestLayer::InitUI()
{
    ToTopZPriority(this);

    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
     
    pQuestFullScreen = new QuestFullScreen(size);
     
    pQuestFullScreen->setAnchorPoint(ccp(0, 0));
     
    pQuestFullScreen->setPosition(accp(0, 0));
     
    this->addChild(pQuestFullScreen, 100);
}


void QuestLayer::Result(ResponseQuestUpdateResultInfo* _questResult)
{
    
    this->removeAllChildrenWithCleanup(true);
    this->removeChild(pQuestFullScreen, true);
    
    MainScene::getInstance()->ShowMainMenu();
    UserStatLayer::getInstance()->ShowMenu();
    RestoreZProirity(this);
        
    pResultLayer = new QuestResultLayer(this->getContentSize());
    pResultLayer->setAnchorPoint(ccp(0, 0));
    pResultLayer->setPosition(accp(640 ,-75));
    pResultLayer->InitUI(_questResult);
    this->addChild(pResultLayer);

    resultBG_On();
    
    pResultLayer->RunAction();
     
}

*/
