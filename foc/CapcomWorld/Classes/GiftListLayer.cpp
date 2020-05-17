//
//  GiftListLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 6..
//
//

#include "GiftListLayer.h"
#include "PopUp.h"

GiftListLayer::GiftListLayer(CCSize layerSize)
{
    //this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    this->setClipsToBounds(true);
    
    bTouchMove = false;
}

GiftListLayer::~GiftListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void GiftListLayer::SetGiftData(CCArray* _gift)
{
    myGift = _gift;
    /*
    Gift* tempGift1 = new Gift;
    tempGift1->giftImgPath = "ui/shop/item_coin.png";
    tempGift1->giftName = "Coin";
    tempGift1->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift1->giftCount = 1;
    tempGift1->giftSenderName = "APD1";
    
    Gift* tempGift2 = new Gift;
    tempGift2->giftImgPath = "ui/shop/item_coin.png";
    tempGift2->giftName = "Red Gem";
    tempGift2->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift2->giftCount = 4;
    tempGift2->giftSenderName = "APD2";

    Gift* tempGift3 = new Gift;
    tempGift3->giftImgPath = "ui/shop/item_coin.png";
    tempGift3->giftName = "Red Gem";
    tempGift3->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift3->giftCount = 7;
    tempGift3->giftSenderName = "APD3";

    Gift* tempGift4 = new Gift;
    tempGift4->giftImgPath = "ui/shop/item_coin.png";
    tempGift4->giftName = "Red Gem";
    tempGift4->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift4->giftCount = 2;
    tempGift4->giftSenderName = "APD4";

    Gift* tempGift5 = new Gift;
    tempGift5->giftImgPath = "ui/shop/item_coin.png";
    tempGift5->giftName = "Red Gem";
    tempGift5->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift5->giftCount = 3;
    tempGift5->giftSenderName = "APD5";

    Gift* tempGift6 = new Gift;
    tempGift6->giftImgPath = "ui/shop/item_coin.png";
    tempGift6->giftName = "Red Gem";
    tempGift6->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift6->giftCount = 4;
    tempGift6->giftSenderName = "APD6";
    
    Gift* tempGift7 = new Gift;
    tempGift7->giftImgPath = "ui/shop/item_coin.png";
    tempGift7->giftName = "Red Gem";
    tempGift7->giftDescription = "소모된 포인트(공격+방어)를";
    tempGift7->giftCount = 4;
    tempGift7->giftSenderName = "APD6";

    myGift->addObject(tempGift1);
    myGift->addObject(tempGift2);
    myGift->addObject(tempGift3);
    myGift->addObject(tempGift4);
    myGift->addObject(tempGift5);
    myGift->addObject(tempGift6);
    myGift->addObject(tempGift7);
     */
}

int GiftListLayer::GetCountOfGift() const
{
    return myGift->count();
}

void GiftListLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
}

void GiftListLayer::InitUI()
{
    StartYPos = this->getContentSize().height - 70;
    
    const int count = myGift->count();
    
    for(int i=0; i<count; ++i)
    {
        GiftInfo* tempGift = (GiftInfo*)myGift->objectAtIndex(i);
        
        MakeGiftCell(tempGift, i);
    }   
}

void GiftListLayer::MakeGiftCell(GiftInfo* pGift, int tag)
{
    CCSprite* pSprItemBG = CCSprite::create("ui/shop/item_bg.png");
    pSprItemBG->setAnchorPoint(ccp(0,0));
    pSprItemBG->setPosition(accp(9, StartYPos));
    this->addChild(pSprItemBG, 60);
    
    Item_Data* giftData =  FileManager::sharedFileManager()->GetItemInfo(pGift->giftID);
    
    string imgPath = "ui/shop/";
    imgPath += giftData->itemImgPath;

    CCSprite* pSprGift = CCSprite::create(imgPath.c_str());
    pSprGift->setAnchorPoint(ccp(0,0));
    pSprGift->setPosition(accp(25, StartYPos + 40));
    this->addChild(pSprGift, 60);

    CCLabelTTF* pGiftName = CCLabelTTF::create(giftData->itemName.c_str(), "Thonburi", 14);
    pGiftName->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 125), pGiftName, 60);
    
    CCLabelTTF* pGiftDescription1 = CCLabelTTF::create(giftData->itemDesc.c_str(), "Thonburi", 12);
    pGiftDescription1->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 95), pGiftDescription1, 60);
/*
    CCLabelTTF* pGiftDescription2 = CCLabelTTF::create(pGift->giftDescription.c_str(), "Thonburi", 12);
    pGiftDescription2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 65), pGiftDescription2, 60);
  */  
    CCLabelTTF* pGiftCount = CCLabelTTF::create("아이템 수", "Thonburi", 12);
    pGiftCount->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 35), pGiftCount, 60);
    
    char buff[10];
    sprintf(buff, "%d", pGift->count);
    
    CCLabelTTF* pGiftCount1 = CCLabelTTF::create(buff, "Thonburi", 12);
    pGiftCount1->setColor(COLOR_WHITE);
    pGiftCount1->setTag(1000 + tag);
    registerLabel( this,ccp(0, 0), accp(270, StartYPos + 35), pGiftCount1, 60);
    
    CCLabelTTF* pGiftSender = CCLabelTTF::create("선물한 사람", "Thonburi", 12);
    pGiftSender->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 5), pGiftSender, 60);
    
    CCLabelTTF* pGiftSender1 = CCLabelTTF::create(pGift->nick, "Thonburi", 12);
    pGiftSender1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(280, StartYPos + 5), pGiftSender1, 60);

    CCSprite* pSprReceive = CCSprite::create("ui/card_tab/card_list_btn.png");
    pSprReceive->setAnchorPoint(ccp(0, 0));
    pSprReceive->setPosition(accp(530, StartYPos+43));
    pSprReceive->setTag(tag);
    this->addChild(pSprReceive, 60);
    
    CCLabelTTF* pLabelReceive = CCLabelTTF::create("받기", "Thonburi", 12);
    pLabelReceive->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(555, StartYPos + 68), pLabelReceive, 60);
    
    StartYPos -= 180;
}

void GiftListLayer::ReceiveGift(int Index)
{
    CCNode* pNode = this->getChildByTag(1000 + Index);
    
    CCLabelTTF* pLabel = (CCLabelTTF*)pNode;
    
    float x = pLabel->getPositionX();
    float y = pLabel->getPositionY();
    
    this->removeChild(pLabel, true);
    
    GiftInfo* tempItem = (GiftInfo*)myGift->objectAtIndex(Index);
    --tempItem->count;
    
    char buff[10];
    sprintf(buff, "%d", tempItem->count);
    
    pLabel = CCLabelTTF::create(buff, "Thonburi", 12);
    pLabel->setTag(1000 + Index);
    registerLabel( this,ccp(0, 0), accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE), pLabel, 60);
    
    CCLog("ReceiveGift, 30");

    if(tempItem->count <= 0)
    {
        myGift->removeObjectAtIndex(Index);
        
        this->removeAllChildrenWithCleanup(true);
        
        const float LayerHeight = GIFT_HEIGHT * this->GetCountOfGift() + 5 * this->GetCountOfGift();
        
        this->LayerStartPos = (584 - LayerHeight)/SCREEN_ZOOM_RATE;
        
        this->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
        
        this->setPosition(accp(0, 584 - LayerHeight));
        
        InitUI();    
    }
}

void GiftListLayer::removePopUp()
{
    this->setTouchEnabled(true);
    
    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }
}

void GiftListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    const int giftCount = myGift->count();
    
    //if(false ==  bTouchMove)
    //{
    for(int i=0; i<giftCount; ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            nLastTag = i;
        }
    }
    //}
}

void GiftListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
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
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(GiftListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(GiftListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(GiftListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(GiftListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    const int giftCount = myGift->count();
    
    //if(false ==  bTouchMove)
    //{
        for(int i=0; i<giftCount; ++i)
        {
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                if (nLastTag == i){
                soundButton1();
                
                this->setTouchEnabled(false);

                GiftInfo* tempItem = (GiftInfo*)myGift->objectAtIndex(i);
                
                basePopUp = new GiftPopUp();
                basePopUp->InitUI(tempItem);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                
                basePopUp->setPosition(accp(0.0f, -this->getPositionY() * SCREEN_ZOOM_RATE));
                
                this->addChild(basePopUp, 1000);    

                //ReceiveGift(i);
                }
                
                
                nLastTag = -1;
            }
        }
    //}
    
    bTouchMove = false;
}

void GiftListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("내 선물 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
    bTouchMove = true;
}

void GiftListLayer::scrollingEnd()
{
    this->stopAllActions();
}

void GiftListLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT);
    int clip_h = winSize.height - accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN + CARD_DECK_TOP_UI_SPACE_2+CARD_DECK_TOP_UI_SPACE_3);
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, this->getContentSize().width, clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}

