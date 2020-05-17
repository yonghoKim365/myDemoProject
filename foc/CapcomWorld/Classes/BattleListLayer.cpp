//
//  BattleListLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 19..
//
//

#include "BattleListLayer.h"
#include "BattlePlayerCell.h"
#include "MainScene.h"
#include "BattleDuelLayer.h"
#include "ARequestSender.h"

BattleListLayer::BattleListLayer(CCSize layerSize)
{
    clipRect = CCRectMake(0,0, layerSize.width, layerSize.height);
    this->setContentSize(layerSize);
    this->setAnchorPoint(ccp(0,0));
    
    InitLayer();
    
    //CheckLayerSize(this);
    this->setTouchEnabled(true);
    //this->setClipsToBounds(true);
}


BattleListLayer::~BattleListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void BattleListLayer::InitLayer()
{
    PlayerInfo *pi = PlayerInfo::getInstance();
    
    int num_of_cell = pi->battleList->count();
    printf("num %d\n", num_of_cell);
    
    int refresh_btn_height = 32;
    int bottom_space = 30;
    
    int content_h = 20 + 49 + (num_of_cell * 166) + 10 + refresh_btn_height + bottom_space;
    content_h = accp(content_h);
    this->setContentSize(CCSize(this->getContentSize().width, content_h));
    this->setPositionY(0 - this->getContentSize().height  + clipRect.size.height);// + accp(CARDS_LAYER_BTN_HEIGHT));
    
    yStart = this->getPositionY();
    
    int yy = this->getContentSize().height * SCREEN_ZOOM_RATE - 20 - 49;
    
    
    CCSprite* pSpr1 = CCSprite::create("ui/battle_tab/battle_duel_list_honor.png");
    pSpr1->setAnchorPoint(accp(0,0));
    pSpr1->setPosition( accp(229,yy) );
    this->addChild(pSpr1, 0);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("honor_point"), "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_WHITE);
    pLabel1->setTag(21);
    registerLabel( this,ccp(0,0), accp(283,yy+8), pLabel1,10);
    
    RefreshNumber(PlayerInfo::getInstance()->getFame(), 600, yy+15, this);
    
    if (num_of_cell > 0){
        yy = yy + 10;
        
        for(int i=0;i<num_of_cell;i++){
            yy = yy - 156;
            BattlePlayerCell *cell = new BattlePlayerCell((UserInfo*)pi->battleList->objectAtIndex(i), this);
            cell->setAnchorPoint(accp(0,0));
            cell->setPosition(accp(10,yy));
            this->addChild(cell, 0);
            yy = yy - 10;
        }
    }
    else {
        yy-=10;
    }
    yy-=10;
    
    yy-=refresh_btn_height;
    
    CCSprite* pSprBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pSprBtn->setAnchorPoint(ccp(0,0));
    pSprBtn->setPosition( accp(218,yy-12) );
    pSprBtn->setTag(10);
    this->addChild(pSprBtn, 0);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create(LocalizationManager::getInstance()->get("seach_btn"), "HelveticaNeue-Bold", 12);
    pLabel2->setColor(COLOR_YELLOW);
    pLabel2->setTag(21);
    registerLabel( this,ccp(0.5,0), accp(320, yy-6), pLabel2,10);
    
}


void BattleListLayer::ButtonBattle(UserInfo *_user)
{
    /*
    CCLog("Button battle, user name:%s",_user->myNickname.c_str());
    this->removeAllChildrenWithCleanup(true);
    InitLayer(1);
     */
    BattleDuelLayer::getInstance()->ButtonBattle(_user);
}


void BattleListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
    
    //CCLog("ccTouchesBegan,y=%f", this->getPositionY());
    
}

void BattleListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    

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
        else if (endPos < yStart)
            endPos = yStart;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    
    if (GetSpriteTouchCheckByTag(this, 10, localPoint))
    {
        soundButton1();
        
        ARequestSender::getInstance()->requestOpponent();
    }
    
    if (clipRect.size.height > this->getContentSize().height)return;
    
    float y = this->getPositionY();
    //    float yy = this->getPositionY() + this->getContentSize().height;
    
    //CCLog("DeckListLayer layer y:%f, y+h:%f",y,yy);
    
    if (y < yStart){
        //if (y < yStart){
            //CCActionInterval *action = CCActionInterval::initWithDuration(0.3);
            //CCActionEase *move = CCActionEase::initWithAction(cocos2d::CCActionInterval *pAction)
            
            CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0,this->yStart)), 3);
            CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        //}
    }
    //else if (y > 0){
    else if (y > 0){ //accp(CARDS_LAYER_BTN_HEIGHT)){
        
        CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0, accp(0))), 3);
        CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
    
}


void BattleListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    if (clipRect.size.height > this->getContentSize().height)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    //CCLog("touch began");
    if (bTouchPressed){
        
        if (touchStartPoint.y != location.y){
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
            //CCLog("deckListLayer.y:%f",this->getPositionY());
        }
    }
    float distance = fabs(startPosition.y - location.y);
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
    
}

void BattleListLayer::scrollingEnd()
{
    this->stopAllActions();
	//this->setIsScrolling(false);
}

void BattleListLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
	//if (clipsToBounds)
    //{
        CCRect scissorRect = CCRect(0.0f, 0.0f, winSize.width-accp(10), winSize.height);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    //}
    
    CCNode::visit();
    
    //if (clipsToBounds){
        glDisable(GL_SCISSOR_TEST);
    //}
}

////////////////////////////////////////
#pragma mark clipping
