//
//  ItemLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#include "ItemLayer.h"
#include "ItemInfo.h"
#include "GiftInfo.h"
#include "PlayerInfo.h"

ItemLayer::ItemLayer(CCSize layerSize, int tab) : pGiftLayer(NULL), pItemListlayer(NULL)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    
    bTouchMove = false;
    
    CurrentTag = -1;
    
    InitUI();
    
    if(0 == tab)
        InitMyItemlayer();
    else if(1 == tab)
        InitGiftLayer();
}

ItemLayer::~ItemLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void ItemLayer::InitUI()
{
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0,0));
    pSprBG->setPosition( accp(0,0) );
    this->addChild(pSprBG);
    
    CCSprite* pSprMyItem = CCSprite::create("ui/home/home_item_tab_a1.png");
    pSprMyItem->setAnchorPoint(ccp(0, 0));
    pSprMyItem->setPosition( accp(10, 695) );
    pSprMyItem->setTag(1);
    this->addChild(pSprMyItem, 100);
   
    CurrentTag = 3;
    
    CCLabelTTF* pLabelMyItem = CCLabelTTF::create(LocalizationManager::getInstance()->get("keeptab_btn"), "HelveticaNeue-BoldT", 13);
    pLabelMyItem->setColor(COLOR_GRAY);
    registerLabel(this, ccp(0.5f, 0.0f), accp(10.0f + (310.0f/2.0f), 705), pLabelMyItem, 110);
    
    CCSprite* pSprGift = CCSprite::create("ui/home/home_item_tab_b1.png");
    pSprGift->setAnchorPoint(ccp(0, 0));
    pSprGift->setPosition( accp(320, 695) );
    pSprGift->setTag(2);
    this->addChild(pSprGift, 100);
    
    CCLabelTTF* pLabelGift = CCLabelTTF::create(LocalizationManager::getInstance()->get("gifttab_btn"), "HelveticaNeue-Bold", 13);
    pLabelGift->setColor(COLOR_GRAY);
    registerLabel(this, ccp(0.5f, 0.0f), accp(10.0f + 310.0f + (310.0f/2.0f), 705), pLabelGift, 110);
}

void ItemLayer::InitGiftLayer()
{
    CurrentTag = 4;
    
    ChangeSpr(this, 2, "ui/home/home_item_tab_b2.png", 100);
    
    ResponseGiftInfo* giftInfo = ARequestSender::getInstance()->requestGiftList();
    
    if(giftInfo)
    {
        if (atoi((const char*)giftInfo->res) == 0){
            PlayerInfo::getInstance()->myGiftList = new CCArray();
            
            for(int i=0;i<giftInfo->giftList->count();i++)
            {
                GiftInfo* gift = (GiftInfo*)giftInfo->giftList->objectAtIndex(i);
                CCLog("gift id:%d count:%d", gift->giftID, gift->count);

                PlayerInfo::getInstance()->myGiftList->addObject(gift);
            }
        }
        else{
            popupNetworkError(giftInfo->res, giftInfo->msg,"requestGiftList");
            return;
        }

        pGiftLayer = new GiftListLayer(CCSize(this->getContentSize().width, this->getContentSize().height));
        
        pGiftLayer->SetGiftData(PlayerInfo::getInstance()->myGiftList);
        
        const float LayerHeight = GIFT_HEIGHT * pGiftLayer->GetCountOfGift() + 10 * pGiftLayer->GetCountOfGift() + 10;
        
        pGiftLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
        
        pGiftLayer->InitUI();
        
        pGiftLayer->LayerStartPos = (584 - LayerHeight)/SCREEN_ZOOM_RATE;
        
        pGiftLayer->setAnchorPoint(ccp(0, 0));
        
        pGiftLayer->setPosition(accp(0, 584 - LayerHeight));
        
        pGiftLayer->setTouchEnabled(true);
        
        this->addChild(pGiftLayer, 60);
    }
    else
        popupOk("보관함 목록을 불러오는데 실패했습니다. \n잠시후에 다시 시도해주세요");
}

void ItemLayer::InitMyItemlayer()
{
    CurrentTag = 3;
    
    ChangeSpr(this, 1, "ui/home/home_item_tab_a2.png", 100);
    
    pItemListlayer = new ItemListLayer(CCSize(this->getContentSize().width, this->getContentSize().height));
    
    pItemListlayer->SetItemData(PlayerInfo::getInstance()->myItemList);
    
    const float LayerHeight = (ITEM_HEIGHT * pItemListlayer->myItem->count()) + (10 * pItemListlayer->myItem->count()) + 10;
    
    pItemListlayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
    
    pItemListlayer->InitUI();
    
    pItemListlayer->LayerStartPos = (584 - LayerHeight)/SCREEN_ZOOM_RATE;
    
    pItemListlayer->setAnchorPoint(ccp(0, 0));
    
    pItemListlayer->setPosition(accp(0, 584 - LayerHeight));
    
    //pItemListlayer->setPosition(accp(0, 0));
    
    pItemListlayer->setTouchEnabled(true);
    
    this->addChild(pItemListlayer, 60);

    /*
    if(itemInfo)
    {
        if (atoi((const char*)itemInfo->res) == 0){
            PlayerInfo::getInstance()->myItemList = new CCArray();
            
            for(int i=0;i<itemInfo->itemList->count();i++)
            {
                ItemInfo* item = (ItemInfo*)itemInfo->itemList->objectAtIndex(i);
                CCLog(" item id:%d count:%d", item->itemID, item->count);
                PlayerInfo::getInstance()->myItemList->addObject(item);
            }
        }
        else{
            popupNetworkError(itemInfo->res, itemInfo->msg,"requestItemList");
            return;
        }
        
        
        //Item_Data* itemData =  FileManager::sharedFileManager()->GetItemInfo(10001);
        
        pItemListlayer = new ItemListLayer(CCSize(this->getContentSize().width, this->getContentSize().height));
        
        pItemListlayer->SetItemData(PlayerInfo::getInstance()->myItemList);
        
        const float LayerHeight = (ITEM_HEIGHT * pItemListlayer->myItem->count()) + (10 * pItemListlayer->myItem->count()) + 10;
        
        pItemListlayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
        
        pItemListlayer->InitUI();
        
        pItemListlayer->LayerStartPos = (584 - LayerHeight)/SCREEN_ZOOM_RATE;
        
        pItemListlayer->setAnchorPoint(ccp(0, 0));
        
        pItemListlayer->setPosition(accp(0, 584 - LayerHeight));
        
        //pItemListlayer->setPosition(accp(0, 0));
        
        pItemListlayer->setTouchEnabled(true);

        this->addChild(pItemListlayer, 60);
    }
    else
        popupOk("아이템 리스트를 불러오는데 실패했습니다. \n잠시후에 다시 시도해주세요");
     */
}

void ItemLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void ItemLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    //if(false == bTouchMove)
    //{
        // -- 소유한 아이템
        if (GetSpriteTouchCheckByTag(this, 1, localPoint))
        {
            if(3 != CurrentTag)
            {
                soundButton1();
                //CCLog("소유한 아이템");
                
                ChangeSpr(this, 2, "ui/home/home_item_tab_b1.png", 100);
                ChangeSpr(this, 1, "ui/home/home_item_tab_a2.png", 100);
                
                this->removeChild(pItemListlayer, true);
                this->removeChild(pGiftLayer, true);
                
                ARequestSender::getInstance()->requestItemListAsync();
                //InitMyItemlayer();
                
                CurrentTag = 3;
            }
        }
        
        // -- 선물함
        if (GetSpriteTouchCheckByTag(this, 2, localPoint))
        {
            if(4 != CurrentTag)
            {
                soundButton1();
                //CCLog("선물함");
                
                ChangeSpr(this, 1, "ui/home/home_item_tab_a1.png", 100);
                ChangeSpr(this, 2, "ui/home/home_item_tab_b2.png", 100);
                
                this->removeChild(pItemListlayer, true);
                this->removeChild(pGiftLayer, true);
                
                ARequestSender::getInstance()->requestGiftListAsync();
                //InitGiftLayer();
                
                CurrentTag= 4;
            }
        }
    //}
    
    bTouchMove = false;
}

void ItemLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    bTouchMove = true;
}

