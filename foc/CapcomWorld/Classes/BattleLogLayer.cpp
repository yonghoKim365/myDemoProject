//
//  BattleLogLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#include "BattleLogLayer.h"
#include "ARequestSender.h"
#include "DojoLayerDojo.h"

BattleLogLayer* BattleLogLayer::instance = NULL;

BattleLogLayer* BattleLogLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

BattleLogLayer::BattleLogLayer(CCSize layerSize) : pBattleLogListLayer(NULL), noticeInfo(NULL)
{
    instance = this;
    
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    InitUI();
}

BattleLogLayer::~BattleLogLayer()
{
    this->removeAllChildrenWithCleanup(true);

    CC_SAFE_DELETE(noticeInfo);
}

void BattleLogLayer::InitUI()
{
    CCSprite* pSprEvent = CCSprite::create("ui/home/ui_home_bg_event.png");
    pSprEvent->setAnchorPoint(ccp(0, 0));
    pSprEvent->setPosition(accp(10, 779 - MAIN_LAYER_TOP_MARGIN - 667));
    this->addChild(pSprEvent, 60);
    
    CCSprite* pClose = CCSprite::create("ui/home/ui_home_bg_event_close1.png");
    pClose->setAnchorPoint(ccp(0, 0));
    pClose->setTag(0);
    pClose->setPosition(accp(547.0f, 779 - MAIN_LAYER_TOP_MARGIN - 58));
    this->addChild(pClose, 60);
    
    CCLabelTTF* title = CCLabelTTF::create("최근방어", "HelveticaNeue-Bold", 13);
    title->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.5f, 0.0f), accp(320.0f, 710.0f), title, 60);
}

void BattleLogLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 0, localPoint))
    {
        ChangeSpr(this, 0, "ui/home/ui_home_bg_event_close2.png", 100);
    }
}

void BattleLogLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);

    if(GetSpriteTouchCheckByTag(this, 0, localPoint))
    {
        ChangeSpr(this, 0, "ui/home/ui_home_bg_event_close1.png", 100);

        soundButton1();
        
        DojoLayerDojo::getInstance()->releaseBattleLog();
        DojoLayerDojo::getInstance()->setTouchEnabled(true);
    }
}
