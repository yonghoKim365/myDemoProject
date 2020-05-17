//
//  ShopItem.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 19..
//
//

#include "ShopItem.h"

ShopItem::ShopItem(CCSize layerSize) : basePopUp(NULL)
{
    curPage = 0;
    
    bTouchMove = false;
    
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
}

ShopItem::~ShopItem()
{
    this->removeAllChildrenWithCleanup(true);
}

void ShopItem::SetItemData(CCArray* arryItem)
{
    MyItem = arryItem;
}

void ShopItem::InitUI(int _curPage)
{
    curPage = _curPage;
    StartYPos = this->getContentSize().height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - 170 - MAIN_LAYER_TOP_MARGIN - 42;
    
    const int itemCount = MyItem->count();
    
    maxpage = (itemCount / ITEM_PER_PAGE) + 1;
    
    int startI = curPage * ITEM_PER_PAGE;
    int endI = startI + ITEM_PER_PAGE;
    
    if(endI > itemCount) endI = itemCount;
    
    for(int i=startI; i<endI; ++i)
    {
        Item_Data* tempItemp = (Item_Data*)MyItem->objectAtIndex(i);
        
        makeCell(tempItemp, i);
    }
    
    if(itemCount>ITEM_PER_PAGE)
        pageBtn(curPage, itemCount);
}

void ShopItem::makeCell(Item_Data* _item, int tag)
{
    CCSprite* pSprItemBG = CCSprite::create("ui/shop/item_bg.png");
    pSprItemBG->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprItemBG->setPosition(accp(10.0f, StartYPos));
    this->addChild(pSprItemBG, 60);
    
    string path = "ui/shop/";
    path+=_item->itemImgPath;
    CCSprite* pSprItem = CCSprite::create(path.c_str());
    pSprItem->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprItem->setPosition(accp(24.0f, StartYPos + 38.0f));
    this->addChild(pSprItem, 60);

    char nameText[64];
    sprintf(nameText, "%d_name", _item->itemID);
    CCLabelTTF* ItemName = CCLabelTTF::create(LocalizationManager::getInstance()->get(nameText), "HelveticaNeue-Bold", 12);
    ItemName->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0, 0), accp(159.0f, StartYPos + 120.0f), ItemName, 160);
    
    sprintf(nameText, "%d_msg", _item->itemID);
    std::string text = LocalizationManager::getInstance()->get(nameText);
    CCLabelTTF* ItemDesc1 = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 12);
    ItemDesc1->setColor(COLOR_GRAY);
    registerLabel(this, ccp(0, 0), accp(159.0f, (text.find("\n") < text.length()) ? StartYPos + 58.0f : StartYPos + 90.0f), ItemDesc1, 160);
    
    //CCLabelTTF* ItemDesc2 = CCLabelTTF::create(_item->ItemDesc2.c_str(), "Thonburi", 13);
    //ItemDesc2->setColor(COLOR_WHITE);
    //registerLabel(this, ccp(0, 0), accp(159.0f, StartYPos + 70.0f), ItemDesc2, 160);
    
/*    CCLabelTTF* ItemAmount = CCLabelTTF::create("수량", "Thonburi", 13);
    ItemAmount->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0, 0), accp(159.0f, StartYPos + 40.0f), ItemAmount, 160);*/

    CCSprite* pSprCash = CCSprite::create("ui/shop/icon_gold.png");
    pSprCash->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprCash->setPosition(accp(159.0f, StartYPos + 25.0f));
    this->addChild(pSprCash, 60);
    
    //char buff[10];
    //sprintf(buff, "%d", _item->itemPrice);

    CCLabelTTF* Gold1 = CCLabelTTF::create(_item->itemPrice.c_str(), "HelveticaNeue", 12);
    Gold1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0, 0), accp(195.0f, StartYPos + 23.0f), Gold1, 160);
    
/*    CCLabelTTF* Gold2 = CCLabelTTF::create("골드", "Thonburi", 13);
    Gold2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0, 0), accp(240.0f, StartYPos + 40.0f), Gold2, 160);*/

    CCSprite* purchaseBtn = CCSprite::create("ui/shop/list_btn1.png");
    purchaseBtn->setAnchorPoint(ccp(0, 0));
    purchaseBtn->setTag(tag);
    purchaseBtn->setPosition(accp(530, StartYPos+43));
    this->addChild(purchaseBtn, 60);
    
    CCLabelTTF* pLabelReceive = CCLabelTTF::create(LocalizationManager::getInstance()->get("buy_btn"), "HelveticaNeue-Bold", 12);
    pLabelReceive->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(555, StartYPos + 68), pLabelReceive, 70);
    
    StartYPos -= 181.0f;
}

void ShopItem::pageBtn(int _curPage, int itemCount)
{
    CCSprite* PrevBtn = CCSprite::create("ui/card_tab/page_arrow_p1.png");
    PrevBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    PrevBtn->setPosition(accp(10.0f, StartYPos + 110.0f));
    PrevBtn->setTag(1000);
    this->addChild(PrevBtn);
    
    CCSprite* NextBtn = CCSprite::create("ui/card_tab/page_arrow_n1.png");
    NextBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    NextBtn->setPosition(accp(396.0f, StartYPos + 110.0f));
    NextBtn->setTag(1001);
    this->addChild(NextBtn);
    
    char buff[10];
    sprintf(buff, "%d", _curPage + 1);
    
    CCLabelTTF* curPageLabel = CCLabelTTF::create(buff, "Thonburi", 13);
    curPageLabel->setColor(COLOR_WHITE);
    curPageLabel->setTag(999);
    registerLabel(this, ccp(0, 0), accp(290.0f, StartYPos + 110.0f), curPageLabel, 160);
    
    CCLabelTTF* slashLabel = CCLabelTTF::create("/", "Thonburi", 13);
    slashLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0, 0), accp(320.0f, StartYPos + 110.0f), slashLabel, 160);
    
    char buff1[10];
    sprintf(buff1, "%d", maxpage);
    
    CCLabelTTF* maxPageLabel = CCLabelTTF::create(buff1, "Thonburi", 13);
    maxPageLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0, 0), accp(350.0f, StartYPos + 110.0f), maxPageLabel, 160);
}

void ShopItem::InitPopUp(void *data)
{
    Item_Data* itemData = (Item_Data*)data;
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    const int myGold = pInfo->getCash();
    
    const int remaingGold = myGold - atoi(itemData->itemPrice.c_str());
    
    if(remaingGold<0) data = NULL;
        
    basePopUp = new ItemPopUp();
    basePopUp->InitUI(data);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    ShopLayer* parent = (ShopLayer*)this->getParent();
    
    basePopUp->setPosition(accp(0.0f, -parent->getPositionY() * SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 1000);    
}

void ShopItem::RemovePopUp()
{
    this->setTouchEnabled(true);
    
    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }

}

void ShopItem::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;

    for(int i=0; i<MyItem->count(); ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            ChangeSpr(this, i, "ui/shop/list_btn2.png", 60);
        }
    }
}

void ShopItem::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    // -- Page Prev
    if(GetSpriteTouchCheckByTag(this, 1000, localPoint))
    {
        if(curPage != 0)
        {
            this->removeAllChildrenWithCleanup(true);
            
            --curPage;
            if(curPage < 0) curPage = 0;
            
            ShopLayer* parent = (ShopLayer*)this->getParent();
            parent->InitItemLayer(curPage);
            parent->setPositionY(0.0f);
        }
    }
    
    // -- Page Next
    if(GetSpriteTouchCheckByTag(this, 1001, localPoint))
    {
        if(curPage+1 != maxpage)
        {
            this->removeAllChildrenWithCleanup(true);
            ++curPage;
            if(curPage >= maxpage) curPage = maxpage;
            
            ShopLayer* parent = (ShopLayer*)this->getParent();
            parent->InitItemLayer(curPage);
            parent->setPositionY(0.0f);
        }
    }

    for(int i=0; i<MyItem->count(); ++i)
    {
        ChangeSpr(this, i, "ui/shop/list_btn1.png", 60);
        
        if(false == bTouchMove)
        {
            if(GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                soundButton1();
                
                this->setTouchEnabled(false);
                
                tempShopItem* temp = (tempShopItem*)MyItem->objectAtIndex(i);
                InitPopUp(temp);
                
                //CCLOG("%d 눌림",i);
            }
        }
    }
    
    bTouchMove = false;
}


void ShopItem::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //bTouchMove = true;
}