//
//  CollectListLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 29..
//
//

#include "CollectListLayer.h"

#include "MainScene.h"
#include "DojoLayerCard.h"
#include "UserStatLayer.h"
#include "math.h"



#define CollectCard_W (104) // (131) //(146)
#define CollectCard_H (176) // (198) //(220)
#define CollectCard_W_Space (20) //(32) //(12) // 카드 좌우 여백
#define CollectCard_H_Space (6) //(10) // 카드 위아래 여백

 
#define NUM_OF_H (5)
#define NUM_OF_V (3) //(8)


CollectListLayer* CollectListLayer::instance = NULL;
CollectListLayer:: CollectListLayer(CCRect mRect, CCArray *cardList)
{
    StartYPos = 0;
    
    nPage = 0;
    
    instance = this;
    
    layerRect = mRect;
    
    originCollectList = cardList;
    collectList = cardList;
    
    
    for(int i=0;i<originCollectList->count();i++){
        AColloctionCard* card = (AColloctionCard*)originCollectList->objectAtIndex(i);
        //CCLog("card->getId():%d", card->cardId);
        CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(card->cardId);
        card->series = ci->series;
    }
    
    
    //this->setContentSize(layerSize);
    
    cardMaker = new ACardMaker();
    
    listLayerRect = CCRectMake(0,0 , layerRect.size.width, layerRect.size.height-38);
    
    InitLayer(0, 0);
        
    this->setTouchEnabled(true);
}

CollectListLayer::~CollectListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void CollectListLayer::InitLayer(int _depth, int _series){

    this->removeAllChildrenWithCleanup(true);
    
    layerDepth = _depth;
    if (_depth == 0){
        InitUI1();
    }
    else if (_depth == 1){
        //nPage = 0;
        InitUI2(_series);
    }
}

void CollectListLayer::InitUI1()
{
    setContentSize(layerRect.size);
    
    this->setPositionY(0);
    
    CCSize size = this->getContentSize();
    
    int yy = size.height - accp(MAIN_LAYER_TOP_MARGIN);
    
    yy -= accp(112); // banner bg height;
    
    for (int i=0;i<4;i++){
        cocos2d::CCSprite* pSprBanner1 = CCSprite::create("ui/card_collect/card_collect_bg_a1.png");
        pSprBanner1->setAnchorPoint(ccp(0,0));
        pSprBanner1->setPosition(ccp(accp(10), yy));
        pSprBanner1->setTag(50+i*10);
        this->addChild(pSprBanner1);
        /*
        cocos2d::CCSprite* pSprLogo = CCSprite::create("ui/card_collect/card_collect_logo01.png");
        pSprLogo->setAnchorPoint(ccp(0,0));
        pSprLogo->setPosition(ccp(accp(10+24), yy+accp(19)));
        pSprLogo->setTag(50+i*10 + 1);
        this->addChild(pSprLogo);
        */
        string char_path = "ui/card_collect/card_collect_cha0";
        char p[1];
        sprintf(p, "%d", i+1);
        char_path.append(p).append(".png");
        cocos2d::CCSprite* pSprChar = CCSprite::create(char_path.c_str());
        pSprChar->setAnchorPoint(ccp(0,0));
        
        int xx = 392;
        switch(i){
            case 1: xx = 383; break;
            case 2: xx = 499; break;
            case 3: xx = 474; break;
        }
        pSprChar->setPosition(ccp(accp(10+xx), yy+accp(11)));
        pSprChar->setTag(50+i*10 + 2);
        this->addChild(pSprChar);
        
        string bannerLabel = "스트리트파이터";
        switch(i){
            case 1: bannerLabel = "파이널 파이트"; break;
            case 2: bannerLabel = "뱀파이어 헌터"; break;
            case 3: bannerLabel = "사립저스티스 학원"; break;
        }
        
        //CCLog("bannerLabel :%s", bannerLabel.c_str());
        CCLabelTTF* pLabel6 = CCLabelTTF::create(bannerLabel.c_str() , "HelveticaNeue-Bold", 16);
        pLabel6->setAnchorPoint(ccp(0,0.5));
        pLabel6->setPosition(ccp(accp(10+226),yy+accp(59)));
        pLabel6->setColor(COLOR_WHITE);
        this->addChild(pLabel6);
        
        yy -= accp(112); // banner bg height;
        yy -= accp(10);
    }
}


void CollectListLayer::InitUI2(int _series)
{
 
    setContentSize(CCSize(layerRect.size.width, listLayerRect.size.height));
    
    this->setPositionY(0 - this->getContentSize().height  + layerRect.size.height);
    //StartYPos = this->getPositionY();
    
    CCSize size = this->getContentSize();
    
    int yy = size.height - accp(MAIN_LAYER_TOP_MARGIN);
    
    //////////////////////////////////// back button
    yy -= accp(CARDLIST_PREV_BTN_UPPER_SPACE);
    yy -= accp(CARDLIST_PREV_BTN_H);
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(CollectListLayer::BackBtnCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( ccp(0,0));//size.width/5 * 0,0));
    pSprBtn1->setTag(0);
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( ccp(accp(10), yy));
    pMenu->setTag(199);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기 "   , "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0.5), ccp( getContentSize().width/2, yy + accp(24)), pLabel1, 130);
    
    //////////////////////////////////// back button
    
    yy -= accp(CARDLIST_PREV_BTN_UPPER_SPACE);
    
    
    // filtering
    collectList = new CCArray();
    for(int i=0;i<originCollectList->count();i++){
        AColloctionCard* card = (AColloctionCard*)originCollectList->objectAtIndex(i);
        //CCLog(" card id:%d, card->series :%d", card->getId(), card->series);
        if (card->series == _series)collectList->addObject(card);
    }
    
    pCollectScrollLayer = new CollectScrollLayer(CCRectMake(0,0,this->getContentSize().width,yy), collectList);
    this->setAnchorPoint(ccp(0,0));
    this->addChild(pCollectScrollLayer, 40);
    
    
    
}

void CollectListLayer::BackBtnCallback(CCObject *pSender)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    soundButton1();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if (tag == 0){
        InitLayer(0,0);
    }
}

void CollectListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (layerDepth == 0){
        if (GetSpriteTouchCheckByTag(this, 50, localPoint)){
            pressedTag = 50;
        }
        else if (GetSpriteTouchCheckByTag(this, 60, localPoint)){
            //CCLog(" 60");
            pressedTag = 60;
        }
        else if (GetSpriteTouchCheckByTag(this, 70, localPoint)){
            //CCLog(" 70");
            pressedTag = 70;
        }
        else if (GetSpriteTouchCheckByTag(this, 80, localPoint)){
            //CCLog(" 80");
            pressedTag = 80;
        }
    }
    
    touchStartPoint = location;
    bTouchPressed = true;    

    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void CollectListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (layerDepth == 0){
        if (GetSpriteTouchCheckByTag(this, 50, localPoint)){
            if (pressedTag == 50){
                selectedSeries = 0;
                InitLayer(1,1);
            }
        }
        else if (GetSpriteTouchCheckByTag(this, 60, localPoint)){
            if (pressedTag == 60){
                selectedSeries = 1;
                InitLayer(1,2);
            }
        }
        else if (GetSpriteTouchCheckByTag(this, 70, localPoint)){
            if (pressedTag == 70){
                selectedSeries = 2;
                InitLayer(1,3);
            }
        }
        else if (GetSpriteTouchCheckByTag(this, 80, localPoint)){
            if (pressedTag == 80){
                selectedSeries = 3;
                InitLayer(1,4);
            }
        }
        pressedTag = 0;
        return;
    }
    else if (layerDepth == 1){
        /*
        if (GetSpriteTouchCheckByTag(this, 11, localPoint)){
            CCLog("Page prev");
            if (nPage>0){
                nPage--;
                this->removeAllChildrenWithCleanup(false);
                InitUI2(nPage); // InitUI(nPage);
            }
        }
        else if (GetSpriteTouchCheckByTag(this, 12, localPoint)){
            CCLog("Page next");
            if (nPage < nMaxPage-1){
                nPage++;
                this->removeAllChildrenWithCleanup(false);
                InitUI2(nPage); // InitUI(nPage);
            }
        }
         */
    }
    /*
    //if (layerRect.size.height > this->getContentSize().height)return;
    if (listLayerRect.size.height > this->getContentSize().height)return;
    
    
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
        if (endPos > accp(CARDS_LAYER_BTN_HEIGHT))
            endPos = accp(CARDS_LAYER_BTN_HEIGHT);
        else if (endPos < StartYPos)
            endPos = StartYPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    
    
    
    float y = this->getPositionY();
    
    if (touchStartPoint.y < location.y){
        // scroll up
        CCLog(" scroll up");
    }
    else if (touchStartPoint.y > location.y){
        // scroll down
        CCLog(" scroll down");
    }
    
    CCLog(" y :%f StartYPos :%d", y, StartYPos);

    if (y < StartYPos){
        
            CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0,this->StartYPos)), 3);
            CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
    else if (y > accp(CARDS_LAYER_BTN_HEIGHT)){
        
        CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0,accp(CARDS_LAYER_BTN_HEIGHT))), 3);
        CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
     */
}

void CollectListLayer::scrollingEnd()
{
    this->stopAllActions();
}

void CollectListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
    /*
    if (layerDepth == 0)return;
    //if (layerRect.size.height > this->getContentSize().height)return;
    if (listLayerRect.size.height > this->getContentSize().height)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (bTouchPressed)
    {
        if (touchStartPoint.y != location.y)
        {
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
        }
        
        CCPoint movingVector;
        movingVector.x = location.x - touchMovePoint.x;
        movingVector.y = location.y - touchMovePoint.y;
        
        //CCLog("이동거리 %f", movingVector.y);
        
        touchMovePoint = location;
    }

    float distance = fabs(startPosition.y - location.y);
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    //printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
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

//void CollectListLayer::SetStartYPos(int YPos)
//{
//    StartYPos = YPos;
//}

void CollectListLayer::PageNaviMenuCallBack(CCObject *pSender)
{
    /*
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    if (tag >= 0 && tag <= 3)
    {
        CCMenu *menu = (CCMenu*)this->getChildByTag(99);
        if (menu)
        {
            for (int scan = 0;scan < 4;scan++)
            {
                CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(scan);
                if (!item)
                    continue;
                //item->unselected();
            }
        }
    }
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    
    switch(tag){
        case 11:
            if (nPage>0){
                nPage--;
                this->removeAllChildrenWithCleanup(false);
                InitUI2(nPage); // InitUI(nPage);
            }
            break;
        case 12:
            if (nPage < nMaxPage-1){
                nPage++;
                this->removeAllChildrenWithCleanup(false);
                InitUI2(nPage); // InitUI(nPage);
            }
            break;
    }
     */
}
/*
void CollectListLayer::setThumbTouch(bool b)
{
    for(int i=0;i<num_of_cell_per_page;i++)
    {
        CardThumbLayer *thumbLayer = (CardThumbLayer*)this->getChildByTag(900+i);
        
        if (thumbLayer != NULL){
            thumbLayer->setTouchEnabled(b);
        }
    }
}

void CollectListLayer::beforeOpenDetailView()
{
    this->setTouchEnabled(false);
    this->setThumbTouch(false);
    
}
void CollectListLayer::afterCloseDetailView()
{
    this->schedule(schedule_selector(CollectListLayer::setEnableTouchAfterDelay),0.2f);
}

void CollectListLayer::setEnableTouchAfterDelay()
{
    this->setTouchEnabled(true);
    this->setThumbTouch(true);
    
    this->unschedule(schedule_selector(CollectListLayer::setEnableTouchAfterDelay));
}
*/
/////////////////////////////////////////////////////////////////////////////////////////////////


CollectScrollLayer* CollectScrollLayer::instance = NULL;
CollectScrollLayer:: CollectScrollLayer(CCRect mRect, CCArray *cardList)
{
    StartXPos = 0;
    
    nPage = 0;
    
    instance = this;
    
    layerRect = mRect;
    
    collectList = cardList;
    
    setContentSize(CCSize(layerRect.size.width, layerRect.size.height));
    
    InitUI();
    
    //clippingRect = CCRectMake(0,0 , layerRect.size.width, layerRect.size.height-38);
    
    this->setTouchEnabled(true);
}

CollectScrollLayer::~CollectScrollLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void CollectScrollLayer::InitUI()
{
    //CheckLayerSize(this);
    
    int MAX_CARD_PER_PAGE = NUM_OF_H * NUM_OF_V;         // 한 페이지당 cell 갯수
    
    // collectList 안에서 중복된 카드들 정리.
    bool bLoop = true;
    int cnt = 0;
    if (collectList->count()>1){
        while(bLoop){
            
            //CCLog("cnt = %d",cnt);
            
            CCInteger *v = (CCInteger*)collectList->objectAtIndex(cnt);
            int card_id = v->getValue();
            
            //CCLog("card_id :%d", card_id);
            
            int sameCnt = 0;
            for(int i=0;i<collectList->count();i++){
                
                //CCLog("for (), i= %d", i);
                
                CCInteger *v2 = (CCInteger*)collectList->objectAtIndex(i);
                int card_id2 = v2->getValue();
                
                if (card_id == card_id2)sameCnt++;
                
                if (sameCnt>1){
                    //CCLog("remove card id:%d, index=%d", card_id2, i);
                    collectList->removeObject(v2);
                    i = 0;
                    sameCnt = 0;
                }
            }
            
            cnt++;
            if (cnt >= collectList->count())bLoop = false;
        }
    }
    
    /*
     for (int i=0;i<collectList->count();i++)
     {
     
     CCInteger *v = (CCInteger*)collectList->objectAtIndex(i);
     
     int card_id = v->getValue();
     
     CCLog("card_id :%d", card_id);
     
     }
     */
    /*
    CCInteger *lastCardId = (CCInteger*)collectList->objectAtIndex(collectList->count()-1);
    for(int i=0;i<30;i++){
        collectList->addObject(lastCardId);
    }
     */
    
    
    total_cell = collectList->count();
    int total_page = total_cell / MAX_CARD_PER_PAGE;
    if (total_cell % MAX_CARD_PER_PAGE != 0)total_page++;
    
    nMaxPage = total_page;
    
    int layerWidth = accp(10) + (nMaxPage * (accp(CollectCard_W + CollectCard_W_Space) * NUM_OF_H));
    
    setContentSize(CCSize(layerWidth, this->getContentSize().height));
    
    int yy = this->getContentSize().height;
    int xx = accp(10);
    int lineCnt = 0;
    int pageCnt = 0;
    for(int i=0;i<collectList->count();i++)
    {
        if (i%NUM_OF_H==0){
            lineCnt++;
            if (lineCnt > NUM_OF_V){
                pageCnt++;
                lineCnt = 1;
                yy = this->getContentSize().height;
            }
            xx = accp(10) + (pageCnt * (accp(CollectCard_W + CollectCard_W_Space) * NUM_OF_H));
            yy -= accp(CollectCard_H);
            if (lineCnt>1)yy -= accp(CollectCard_H_Space);
            
        }
                    
        CCInteger *cardId = (CCInteger*)collectList->objectAtIndex(i);
        
        CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(cardId->getValue());
        
        ci->setLevel(0);
        
        CardThumbLayer *thumbLayer = new CardThumbLayer(ci, ccp(xx*SCREEN_ZOOM_RATE, yy*SCREEN_ZOOM_RATE), CollectCard_H);
        
        thumbLayer->setDelegate(this);
        
        this->addChild(thumbLayer,1000);
        
        thumbLayer->setTag(900+i);

        xx += accp(CollectCard_W + CollectCard_W_Space);
    }
    
}

void CollectScrollLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    
    bTouchPressed = true;
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);

}

void CollectScrollLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    //if (clippingRect.size.height > this->getContentSize().height)return;
    
    
    if (nMaxPage == 1)return;
    
    if (touchStartPoint.x < location.x){
        // scroll 
        CCLog(" scroll right");
    }
    else if (touchStartPoint.x > location.x){
        // scroll down
        CCLog(" scroll left");
    }
    
    
    float x = this->getPositionX();
    
    CCLog(" x :%f StartYPos :%d", x, StartXPos);
    
    CCLog("getContentSize().width:%f", getContentSize().width);
    CCLog("GameConst::WIN_SIZE.width:%f", GameConst::WIN_SIZE.width);
    
    
    int x_limit = GameConst::WIN_SIZE.width - getContentSize().width - accp(10);
    
    if (x > StartXPos){
        
        CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(this->StartXPos, 0)), 3);
        CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectScrollLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
    else if (x <  x_limit){
        
        CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(x_limit,0)), 3);
        CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectScrollLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
    
#if (1)
    if (moving == true)
    {
        float distance = startPosition.x - location.x;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionX() - velocity * 3500.f;
        if (endPos < x_limit)
            endPos = x_limit;
        else if (endPos > StartXPos)
            endPos = StartXPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(endPos, 0)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(CollectScrollLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    
}



void CollectScrollLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    
    if (nMaxPage == 1)return;
    
    if (bTouchPressed)
    {
        if (touchStartPoint.x != location.x)
        {
            this->setPositionX(this->getPosition().x + (location.x-touchStartPoint.x));
            touchStartPoint.x = location.x;
        }
        
        
        CCPoint movingVector;
        movingVector.x = location.x - touchMovePoint.x;
        movingVector.y = location.y - touchMovePoint.y;
        
        //CCLog("이동거리 %f", movingVector.y);
        
        touchMovePoint = location;
    }
    
    float distance = fabs(startPosition.x - location.x);
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    //printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
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
        distance = fabs(lastPosition.x - location.x);
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

void CollectScrollLayer::scrollingEnd()
{
    this->stopAllActions();
}

void CollectScrollLayer::setThumbTouch(bool b)
{
    //for(int i=0;i<num_of_cell_per_page;i++)
    for(int i=0;i<collectList->count();i++)
    {
        CardThumbLayer *thumbLayer = (CardThumbLayer*)this->getChildByTag(900+i);
        
        if (thumbLayer != NULL){
            thumbLayer->setTouchEnabled(b);
        }
    }
}

void CollectScrollLayer::beforeOpenDetailView()
{
    this->setTouchEnabled(false);
    this->setThumbTouch(false);
    
}
void CollectScrollLayer::afterCloseDetailView()
{
    this->schedule(schedule_selector(CollectScrollLayer::setEnableTouchAfterDelay),0.2f);
}

void CollectScrollLayer::setEnableTouchAfterDelay()
{
    this->setTouchEnabled(true);
    this->setThumbTouch(true);
    
    this->unschedule(schedule_selector(CollectScrollLayer::setEnableTouchAfterDelay));
}

/////////////////////////////////////////////////////////////////////////////////////////////////

CardThumbLayer::CardThumbLayer(CardInfo *card, CCPoint pos, int thumbnail_h)
{
    _card = card;
    
    cardMaker = new ACardMaker();
    
    cardMaker->MakeCardThumb(this, _card, pos, thumbnail_h, 100, 10);
    
    this->setTouchEnabled(true);
    
    CardDetailViewMakeCnt = 0;
    
}

CardThumbLayer::~CardThumbLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void CardThumbLayer::Init()
{
    
}

void CardThumbLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    touchBeganPoint = location;
}

void CardThumbLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    if (MainScene::getInstance()->popupCnt >> 0)return;
    //if (location.x != touchBeganPoint.x || location.y!= touchBeganPoint.y)return;
    
    //CCLog("CardThumbLayer::ccTouchesEnded, card id:%d", this->_card->getId());
    //CCLog("CardThumbLayer::ccTouchesEnded, x:%f y:%f", this->getPositionX(), this->getPositionY());
    
    if ( fabs(touchBeganPoint.x - location.x) < 10 && fabs(touchBeganPoint.y - location.y) < 10){
        location = CCDirector::sharedDirector()->convertToGL(location);
        
        CCPoint localPoint = this->convertToNodeSpace(location);
        
        if (GetSpriteTouchCheckByTag(this, 10, localPoint)){
            CCLog(" card thumb layer touch");
            OpenDetailView(_card);
        }
    }
    
    
    
}


void CardThumbLayer::OpenDetailView(CardInfo* card)//, CCObject *sender)
{
    
    CCLog("CardThumbLayer::OpenDetailView, CardDetailViewMakeCnt:%d",CardDetailViewMakeCnt);
    
    if (CardDetailViewMakeCnt>0){
        return;
    }
    
    CardDetailViewMakeCnt++;

    this->setTouchEnabled(false);
        
    MainScene::getInstance()->HideMainMenu();
    DojoLayerDojo::getInstance()->HideMenu();
    UserStatLayer::getInstance()->HideMenu();
    this->setTouchEnabled(false);
    
    if (delegate != NULL){
        delegate->beforeOpenDetailView();
    }
    
    //if (CollectListLayer::getInstance() != NULL){
    //    CollectListLayer::getInstance()->setTouchEnabled(false);
    //    CollectListLayer::getInstance()->setThumbTouch(false);
    //}
    
    
    CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
    pSpr0->setAnchorPoint(ccp(0,0));
    pSpr0->setPosition( ccp(0,0) );
    pSpr0->setTag(88);
    MainScene::getInstance()->addChild(pSpr0, 9999);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    _cardDetailView = new CardDetailViewLayer(CCSize(size.width,size.height),card, this);
    _cardDetailView->setPosition(ccp(0,0));
    MainScene::getInstance()->addChild(_cardDetailView,10000,1010);
    this->setTouchEnabled(false);
    
}

void CardThumbLayer::CloseDetailView(){
    
    CCLog(" CardThumbLayer::CloseDetailView");
    
    CardDetailViewMakeCnt--;
    
    MainScene::getInstance()->removeChildByTag(1010, true);
    MainScene::getInstance()->removeChildByTag(88, true);
    _cardDetailView->autorelease();
    MainScene::getInstance()->ShowMainMenu();
    DojoLayerDojo::getInstance()->ShowMenu();
    UserStatLayer::getInstance()->ShowMenu();
    //this->setTouchEnabled(true);
    
    if (delegate != NULL){
        delegate->afterCloseDetailView();
    }
    this->schedule(schedule_selector(CardThumbLayer::enableTouch),0.2f);
    
}

void CardThumbLayer::enableTouch()
{
    //if (CollectListLayer::getInstance() != NULL){
    //    CollectListLayer::getInstance()->setTouchEnabled(true);
    //    CollectListLayer::getInstance()->setThumbTouch(true);
    //}
    this->setTouchEnabled(true);
    this->unschedule(schedule_selector(CardThumbLayer::enableTouch));
}

void CardThumbLayer::setDelegate(ThumbLayerDelegate *d)
{
    delegate = d;
}