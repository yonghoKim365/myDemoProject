//
//  EventListLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 31..
//
//

#include "EventListLayer.h"
#include "EventDetailLayer.h"
#include "PlayerInfo.h"
#include "EventInfo.h"

EventListLayer::EventListLayer(CCSize layerSize) : pEventDetailLayer(NULL)
{
    NumOfEvent = 0;
    yStart = 0;
    EndYPos = 0;
    bTouchMove = false;
    this->setClipsToBounds(true);
}

EventListLayer::~EventListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void EventListLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
    
    CC_SAFE_DELETE(pEventDetailLayer);
}

void EventListLayer::InitLogData()
{
    Event = PlayerInfo::getInstance()->eventList;
    /*
    tempEvent* myEvent1 = new tempEvent;
    myEvent1->ImgPath = "ui/home/ui_home_event_test01.png";
    myEvent1->EventMsg = "Come back 'Blanca Card Pack";
    
    tempEvent* myEvent2 = new tempEvent;
    myEvent2->ImgPath = "ui/home/ui_home_event_test02.png";
    myEvent2->EventMsg = "Skill Card Collection";
    
    tempEvent* myEvent3 = new tempEvent;
    myEvent3->ImgPath = "ui/home/ui_home_event_test03.png";
    myEvent3->EventMsg = "Deliver One's Present";
    
    tempEvent* myEvent4 = new tempEvent;
    myEvent4->ImgPath = "ui/home/ui_home_event_test03.png";
    myEvent4->EventMsg = "Deliver One's Present";
    
    tempEvent* myEvent5 = new tempEvent;
    myEvent5->ImgPath = "ui/home/ui_home_event_test03.png";
    myEvent5->EventMsg = "Deliver One's Present";
    
    tempEvent* myEvent6 = new tempEvent;
    myEvent6->ImgPath = "ui/home/ui_home_event_test02.png";
    myEvent6->EventMsg = "Deliver One's Present";
    
    Event->addObject(myEvent1);
    Event->addObject(myEvent2);
    Event->addObject(myEvent3);
    Event->addObject(myEvent4);
    Event->addObject(myEvent5);
    Event->addObject(myEvent6);
     */
}

void EventListLayer::InitUI()
{
    StartYpos = this->getContentSize().height -35.0f;
    
    if(Event)
    {
        NumOfEvent = Event->count();
        
        for(int i=0; i<NumOfEvent; i++)
        {
            EventInfo* info = (EventInfo*)Event->objectAtIndex(i);
            MakeEventCell(info->eventBanner, i);
        }
    }
}

int EventListLayer::GetNumOfLog() const
{
    if(Event)
        return Event->count();
    else
        return 0;
}

void EventListLayer::MakeEventCell(const string& EventImg, int tag)
{
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
    path+=EventImg;
    CCSprite* pSprEvent = CCSprite::create(path.c_str());
    pSprEvent->setAnchorPoint(ccp(0, 0));
    pSprEvent->setPosition(accp(25, StartYpos));
    pSprEvent->setTag(tag);
    this->addChild(pSprEvent, 60);
    
    StartYpos-=40;
    /*
    CCLabelTTF* pLabel1 = CCLabelTTF::create(EventMsg.c_str(), "Thonburi", 13);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(140, StartYpos), pLabel1, 60);
    */
    StartYpos-=10;
    
    CCSprite* pLine = CCSprite::create("ui/home/ui_home_bg_line.png");
    pLine->setAnchorPoint(ccp(0, 0));
    pLine->setPosition(accp(15, StartYpos));
    this->addChild(pLine, 60);
    
    StartYpos-=130;
}

void EventListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;    
}

void EventListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float y = this->getPositionY();
  
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EventListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EventListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EventListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
       
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(EventListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
   
    for(int i=0; i<NumOfEvent; ++i)
    {
        if(false == bTouchMove)
        {
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                this->setTouchEnabled(false);
                this->setPosition(accp(0,0));
                this->removeAllChildrenWithCleanup(true);
                
                EventInfo* temp = (EventInfo*)Event->objectAtIndex(i);
                
                CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();            
                pEventDetailLayer = new EventDetaillayer(size, temp);
                pEventDetailLayer->setAnchorPoint(ccp(0, 0));
                pEventDetailLayer->setPosition(accp(0, 0));
                this->addChild(pEventDetailLayer, 100);
            }
        }
    }
    
    bTouchMove = false;
}

void EventListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("이벤트 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
    bTouchMove = true;
}

void EventListLayer::scrollingEnd()
{
    this->stopAllActions();
}

void EventListLayer::visit()
{
    /*
	if (clipsToBounds)
    {
        CCRect scissorRect = CCRect(0, 101, this->getContentSize().width, 300);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (clipsToBounds)
        glDisable(GL_SCISSOR_TEST);
     */
    CCSize winSize = CCDirector::sharedDirector()->getWinSize();
    int clip_y = accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT + 30);
    int clip_h = winSize.height - accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN) - accp(50)-accp(34);
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, winSize.width, clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}

