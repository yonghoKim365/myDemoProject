//
//  ItemListlayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 6..
//
//

#include "ItemListlayer.h"
#include "MainScene.h"
#include "PopUp.h"

ItemListLayer::ItemListLayer(CCSize layerSize)
{
    this->setClipsToBounds(true);
    
    bTouchMove = false;
    
    //this->setContentSize(layerSize);
}

ItemListLayer::~ItemListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void ItemListLayer::SetItemData(CCArray* _item)
{
    myItem = _item;
    
    
/*/
    
    ItemInfo* tempItem1 = new ItemInfo;
    tempItem1->itemID = 10004;
    tempItem1->count = 1;
    

    myItem = new CCArray;
    
    for(int i=0; i<4; ++i)
    {
        myItem->addObject(tempItem1);
    }
  //*/
}

int ItemListLayer::GetCountOfItem() const
{
    return myItem->count();
}


void ItemListLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
}

void ItemListLayer::InitUI()
{
    StartYPos = this->getContentSize().height - 45;
 
    const int itemcount = myItem->count();

    if(itemcount <= 0)
        EmptyItem();
    else
    {
        for(int i=0; i<itemcount; ++i)
        {
            ItemInfo* tempItem = (ItemInfo*)myItem->objectAtIndex(i);
            if (!tempItem || tempItem->itemID < 10000 || tempItem->itemID > 20000)
                continue;
            MakeItemCell(tempItem, i);
        }
    }
}

void ItemListLayer::EmptyItem()
{
    CCSprite* pSprTextBG = CCSprite::create("ui/home/text_bg.png");
    pSprTextBG->setAnchorPoint(ccp(0,0));
    pSprTextBG->setPosition(accp(9, this->getContentSize().height - 250));
    this->addChild(pSprTextBG, 60);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCLabelTTF* pLabel1 = CCLabelTTF::create("소유한 아이템이 없습니다.", "Thonburi", 14);
    pLabel1->setColor(COLOR_YELLOW);
    //registerLabel( this,ccp(0, 0), accp(180, this->getContentSize().height - 170), pLabel1, 60);
    registerLabel( this,ccp(0.5, 0), ccp(size.width/2, accp(this->getContentSize().height - 170)), pLabel1, 60);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("상점에서 아이템을 구입 할 수 있습니다.", "Thonburi", 14);
    pLabel2->setColor(COLOR_WHITE);
    //registerLabel( this,ccp(0, 0), accp(110, this->getContentSize().height - 210), pLabel2, 60);
    registerLabel( this,ccp(0.5, 0), ccp(size.width/2, accp(this->getContentSize().height - 210)), pLabel2, 60);
    
    CCSprite* pSprShopBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pSprShopBtn->setAnchorPoint(ccp(0.5,0));
    pSprShopBtn->setTag(9999);
    //pSprShopBtn->setPosition(accp(this->getContentSize().width - 103, this->getContentSize().height - 300));
    pSprShopBtn->setPosition(ccp(this->getContentSize().width/2, accp(this->getContentSize().height - 300)));
    this->addChild(pSprShopBtn, 60);
    
    CCLabelTTF* pLabelShop = CCLabelTTF::create("상점", "Thonburi", 14);
    pLabelShop->setColor(COLOR_YELLOW);
    //registerLabel( this,ccp(0, 0), accp(this->getContentSize().width - 26, this->getContentSize().height - 295), pLabelShop, 60);
    registerLabel( this,ccp(0.5, 0), ccp(this->getContentSize().width/2, accp(this->getContentSize().height - 295)), pLabelShop, 60);

}

void ItemListLayer::MakeItemCell(ItemInfo* pItem, int tag)
{
    CCSprite* pSprItemBG = CCSprite::create("ui/shop/item_bgs.png");
    pSprItemBG->setAnchorPoint(ccp(0,0));
    pSprItemBG->setPosition(accp(9, StartYPos));
    this->addChild(pSprItemBG, 60);
    
    Item_Data* itemData =  FileManager::sharedFileManager()->GetItemInfo(pItem->itemID);
    
    string imgPath = "ui/shop/";
    imgPath += itemData->itemImgPath;
    
    CCSprite* pSprItem = CCSprite::create(imgPath.c_str());
    pSprItem->setAnchorPoint(ccp(0,0));
    pSprItem->setPosition(accp(25, StartYPos + 15));
    this->addChild(pSprItem, 60);
    
    char nameText[32];
    std::string text;
    if (itemData->itemID != 10013)
    {
        sprintf(nameText, "%d_name", itemData->itemID);
        text = LocalizationManager::getInstance()->get(nameText);
    }
    else
        text = "메달";
    CCLabelTTF* pItemName = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pItemName->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 100), pItemName, 60);
    
    if (itemData->itemID != 10013)
    {
        sprintf(nameText, "%d_msg", itemData->itemID);
        text = LocalizationManager::getInstance()->get(nameText);
    }
    else
        text = "메달";
    CCLabelTTF* pItemDescription1 = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 12);
    pItemDescription1->setColor(subBtn_color_normal);
    pItemDescription1->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(159, (text.find("\n") < text.length()) ? StartYPos + 44 : StartYPos + 70), pItemDescription1, 60);
    /*
    CCLabelTTF* pItemDescription2 = CCLabelTTF::create(pItem->itemDescription.c_str(), "Thonburi", 12);
    pItemDescription2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 40), pItemDescription2, 60);
    */
    CCLabelTTF* pItemCount = CCLabelTTF::create("보유", "HelveticaNeue", 12);
    pItemCount->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 10), pItemCount, 60);
    
    char buff[10];
    sprintf(buff, "%d", pItem->count);
    
    CCLabelTTF* pItemCount1 = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
    pItemCount1->setColor(COLOR_WHITE);
    pItemCount1->setTag(1000 + tag);
    registerLabel( this,ccp(0, 0), accp(210, StartYPos + 10), pItemCount1, 60);

    CCSprite* pUse = CCSprite::create("ui/shop/list_btn1.png");
    pUse->setAnchorPoint(ccp(0, 0));
    pUse->setPosition(accp(530, StartYPos+30));
    pUse->setTag(tag);
    this->addChild(pUse, 60);

    CCLabelTTF* pUselavel = CCLabelTTF::create("사용", "HelveticaNeue-Bold", 12);
    pUselavel->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(555, StartYPos + 55), pUselavel, 70);

    StartYPos-= 155;
}

void ItemListLayer::UseItem(int tag)
{
    ItemInfo* tempItem = (ItemInfo*)myItem->objectAtIndex(tag);
   
    PlayerInfo* pinfo = PlayerInfo::getInstance();
    
    bool itemUse = true;
    // -- bluegem
    if(10004 == tempItem->itemID)
    {
        //
        //if(pinfo->getAttackPoint() == pinfo->getMaxAttackPoint() && pinfo->getDefensePoint() == pinfo->getMaxDefencePoint())
        if(pinfo->getBattlePoint() == pinfo->getMaxBattlePoint())
        {
            popupOk("소모된 배틀포인트가 없을 경우  \n 사용할 수 없습니다.");
            itemUse = false;
        }
    }
    // -- 그린잼
    if(10005 == tempItem->itemID)
    {
        if(pinfo->getStamina() == pinfo->getMaxStamina() )
        {
            popupOk("소모된 스테미나가 없을 경우 \n 사용할 수 없습니다.");
            itemUse = false;
        }
    }
    // -- 카드팩
    if(10008 == tempItem->itemID)
    {
        if(pinfo->myCards->count() > NUM_OF_MAX_CARD-5)
        {
            popupOk("계정 슬롯이 부족하기 때문에\n사용할 수 없습니다.");
            itemUse = false;
        }
    }
    
    // -- 메달은 룰렛으로 이동
    if(10013 == tempItem->itemID)
    {
        itemUse = false;
        
        ARequestSender::getInstance()->requestMedalCount();
    }
    
    if(itemUse)
    {
        InitPopUp(tempItem);
        /*
        // -- 레드잼, 그린잼, 카드팩은 팝업으로 사용여부를 물어본다.
        if(10004 == tempItem->itemID || 10005 == tempItem->itemID || 10008 == tempItem->itemID)
            InitPopUp(tempItem);
        else
        {
            ResponseUseInfo *useInfo = ARequestSender::getInstance()->requestUseItem(tempItem->itemID);
            
            if(NULL == useInfo)
            {
                popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
                return;
            }
            
            if (atoi(useInfo->res) != 0){
                popupNetworkError(useInfo->res, useInfo->msg, "requestUseItem");
                return;
            }
            
            if(10006 == tempItem->itemID || 10007 == tempItem->itemID )
            {
                // -- 레드잼 팩, 그린잼 팩 userstat 항목이 0으로 오기 때문에 구분해줌
            }
            else
            {
                pinfo->myCoin = useInfo->user_stat_coin;
                pinfo->setCash(useInfo->user_stat_gold);
                pinfo->setFame(useInfo->user_stat_fame);
                pinfo->setQuestPoint(useInfo->user_stat_q_pnt);
                pinfo->setDefensePoint(useInfo->user_stat_d_pnt);
                pinfo->setAttackPoint(useInfo->user_stat_a_pnt);
                pinfo->setUpgradePoint(useInfo->user_stat_u_pnt);
                pinfo->setXp(useInfo->user_stat_exp);
                pinfo->setLevel(useInfo->user_stat_lev);
                
                UserStatLayer *info = UserStatLayer::getInstance();
                info->refreshUI();
            }

            ARequestSender::getInstance()->requestItemListAsync();
            
            DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
            dojo->ReleaseLayer();
            dojo->InitItemLayer(0);
         
        
        }
         */
    }
}

void ItemListLayer::InitPopUp(void *data)
{
    this->setTouchEnabled(false);
    
    basePopUp = new ItemUsePopUp();
    basePopUp->InitUI(data);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    basePopUp->setPosition(accp(0.0f, -this->getPositionY() * SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 1000);
}

void ItemListLayer::RemovePopUp()
{
    this->setTouchEnabled(true);
    
    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }

}

void ItemListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    for(int i=0; i<myItem->count(); ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            ChangeSpr(this, i, "ui/shop/list_btn2.png", 60);
        }
    }

    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void ItemListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: Ï¢??Î•?Í∞??????? ?∞Ï?Î•?Ï∂???©Î???
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
        else if (endPos < LayerStartPos)
            endPos = LayerStartPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ItemListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    float y = this->getPositionY();
    
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ItemListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ItemListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ItemListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }

        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ItemListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    const int itemCount = myItem->count();
    
    for(int i=0; i<itemCount; ++i)
    {
        ChangeSpr(this, i, "ui/shop/list_btn1.png", 60);
        
        if(false == bTouchMove)
        {
            ItemInfo* tempItem = (ItemInfo*)myItem->objectAtIndex(i);
            if (!tempItem || tempItem->itemID < 10000 || tempItem->itemID > 20000)
                continue;
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                soundButton1();
                UseItem(i);
            }
        }
    }
    
    if (GetSpriteTouchCheckByTag(this, 9999, localPoint))
    {
        soundButton1();
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_SHOP);
        /*
        MainScene* main = (MainScene*)MainScene::getInstance();
        
        main->releaseSubLayers();
        main->initShopLayer();
        
        CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(4);
        item1->selected();
        
        main->curLayerTag = 4;
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(4);
         */
    }
    
    
    bTouchMove = false;
}

void ItemListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    const int itemcount = myItem->count();
    
    if(itemcount > 0)
    {
        if (touchStartPoint.y != location.y)
        {
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
            
            //CCLog("????????????Ï¢?? %f, %f",this->getPosition().x, this->getPosition().y);
        }
    }
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;

    float distance = fabs(startPosition.y - location.y);
        
    if (distance > 5.0f)
        bTouchMove = true;

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

void ItemListLayer::scrollingEnd()
{
    this->stopAllActions();
}

void ItemListLayer::visit()
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



