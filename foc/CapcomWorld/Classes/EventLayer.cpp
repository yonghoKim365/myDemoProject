//
//  EventLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 31..
//
//

#include "EventLayer.h"
#include "DojoLayerDojo.h"

EventLayer::EventLayer(CCSize layerSize) : pSprEvent(NULL), pEventlistLayer(NULL)
{
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);

    InitUI();
}

EventLayer::~EventLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void EventLayer::InitUI()
{
    pSprEvent = CCSprite::create("ui/home/ui_home_bg_event.png");
    pSprEvent->setAnchorPoint(ccp(0, 0));
    pSprEvent->setPosition(accp(10, 779 - MAIN_LAYER_TOP_MARGIN - 667));
    this->addChild(pSprEvent, 60);
    
    CCSprite* pClose = CCSprite::create("ui/home/ui_home_bg_event_close1.png");
    pClose->setAnchorPoint(ccp(0, 0));
    pClose->setTag(0);
    pClose->setPosition(accp(547.0f, 779 - MAIN_LAYER_TOP_MARGIN - 58));
    this->addChild(pClose, 60);
    
    CCLabelTTF* title = CCLabelTTF::create("이벤트", "HelveticaNeue-Bold", 13);
    title->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.5f, 0.0f), accp(320.0f, 710.0f), title, 60);

    pEventlistLayer = new EventListLayer(CCSize(this->getContentSize().width, 603));
    pEventlistLayer->InitLogData();
    const float LayerHeight = EVENT_CELL_HEIGHT * pEventlistLayer->GetNumOfLog();
    pEventlistLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
    pEventlistLayer->InitUI();
    pEventlistLayer->LayerStartPos = (603 - LayerHeight)/SCREEN_ZOOM_RATE;

    pEventlistLayer->setAnchorPoint(ccp(0, 0));
    pEventlistLayer->setPosition(accp(0, 603 - LayerHeight));
    pEventlistLayer->setTouchEnabled(true);
    this->addChild(pEventlistLayer, 60);
}

void EventLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
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

void EventLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void EventLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 0, localPoint))
    {
        ChangeSpr(this, 0, "ui/home/ui_home_bg_event_close1.png", 100);
        
        soundButton1();
        
        DojoLayerDojo::getInstance()->releaseEvent();
        DojoLayerDojo::getInstance()->setTouchEnabled(true);
    }
}

void EventLayer::PosReInit()
{
    //pEventlistLayer->setPosition(accp(0, 0));
}