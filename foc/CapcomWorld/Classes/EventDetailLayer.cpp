//
//  EventDetailLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#include "EventDetailLayer.h"

EventDetaillayer::EventDetaillayer(CCSize layerSize, EventInfo* event)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    
    myEvent = event;
    
    InitUI();
}

EventDetaillayer::~EventDetaillayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void EventDetaillayer::InitUI()
{
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();

    path+=myEvent->eventDetail;
    CCSprite* pEvent = CCSprite::create(path.c_str());
    pEvent->setAnchorPoint(ccp(0, 0));
    pEvent->setPosition(accp(25, 779 - MAIN_LAYER_TOP_MARGIN - 120 - 68));
    this->addChild(pEvent, 200);

    std::string text = myEvent->eventDescription;
    do {
        int pos = text.find("\\n");
        if (pos >= text.length())
            break;
        text = text.replace(pos, 2, "\n");
    } while (1);
    
    CCLabelTTF* eventDescription = CCLabelTTF::create(text.c_str(), "Arial-ItalicMT", 13);
    eventDescription->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(30.0f, this->getContentSize().height*SCREEN_ZOOM_RATE - 520.0f), eventDescription, 112);
    
    CCSprite* pBackBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pBackBtn->setAnchorPoint(ccp(0, 0));
    pBackBtn->setPosition(accp(25, 200));
    pBackBtn->setTag(7);
    this->addChild(pBackBtn, 200);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("Back", "Thonburi", 13);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(100, 206), pLabel1, 210);
}

//void EventDetaillayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
//{
//
//}

void EventDetaillayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 7, localPoint))
    {
        this->removeAllChildrenWithCleanup(true);
        this->setTouchEnabled(false);
        
        pEventlistLayer = new EventListLayer(CCSize(this->getContentSize().width, 598));
        pEventlistLayer->InitLogData();
        const float LayerHeight = EVENT_CELL_HEIGHT * pEventlistLayer->GetNumOfLog();
        pEventlistLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
        pEventlistLayer->InitUI();
        pEventlistLayer->LayerStartPos = (598 - LayerHeight)/SCREEN_ZOOM_RATE;
        
        pEventlistLayer->setAnchorPoint(ccp(0, 0));
        pEventlistLayer->setPosition(accp(0, 598 - LayerHeight));
        pEventlistLayer->setTouchEnabled(true);
        
        CCNode* pNode = this->getParent();
        CCLayer* pLayer = (CCLayer*)pNode;
        pLayer->addChild(pEventlistLayer, 60);
    }
}

//void EventDetaillayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
//{
//
//}
