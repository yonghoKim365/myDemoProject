//
//  ShopLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 19..
//
//

#include "ShopLayer.h"
#include "ShopItem.h"
#include "ShopGold.h"
#include "ShopTreasure.h"
#include "ShopRoulette.h"

ShopLayer* ShopLayer::instance = NULL;

ShopLayer* ShopLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

ShopLayer::ShopLayer(CCSize layerSize) : shopItemLayer(NULL), shopGoldLayer(NULL), shopTreasureLayer(NULL), shopRouletteLayer(NULL)
{
    instance = this;
    
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    
    LayerEndPos = 0.0f;
    bTouchMove = false;
    
    curTab = ITEM_TAB;
    
    //ResponseBuyInfo *buyInfo = ARequestSender::getInstance()->requestBuyItem(10005);
    
    InitUI();
}

ShopLayer::~ShopLayer()
{
    CC_SAFE_DELETE(arryItem);
       
    this->removeAllChildrenWithCleanup(true);
}

void ShopLayer::InitUI()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    float x = 10.0f;
    
    CCSprite* ItemBtn = CCSprite::create("ui/shop/shop_tab_a1.png");
    ItemBtn->setTag(ITEM_TAB);
    ItemBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    ItemBtn->setPosition(accp(x, (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 152));
    this->addChild(ItemBtn, 100);
    
    CCLabelTTF* ItemLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("shop_item_btn"), "HelveticaNeue-Bold", 13);
    ItemLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp((x + 155.0f)/2.0f, (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 140), ItemLabel, 160);

    x+=155.0f;
    CCSprite* GoldBtn = CCSprite::create("ui/shop/shop_tab_b1.png");
    GoldBtn->setTag(GOLD_TAB);
    GoldBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    GoldBtn->setPosition(accp(x, (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 152));
    this->addChild(GoldBtn, 100);
    
    CCLabelTTF* GoldLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("shop_gold_btn"), "HelveticaNeue-Bold", 13);
    GoldLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(x + (155.0f/2.0f), (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 140), GoldLabel, 160);
    
    x+=155.0f;
    CCSprite* TreasureBtn = CCSprite::create("ui/shop/shop_tab_b1.png");
    TreasureBtn->setTag(TREASURE_TAB);
    TreasureBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    TreasureBtn->setPosition(accp(x, (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 152));
    this->addChild(TreasureBtn, 100);
    
    CCLabelTTF* TreasureLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("shop_honor_btn"), "HelveticaNeue-Bold", 13);
    TreasureLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(x + (155.0f/2.0f), (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 140), TreasureLabel, 160);
    
    x+=155.0f;
    CCSprite* RouletteBtn = CCSprite::create("ui/shop/shop_tab_c1.png");
    RouletteBtn->setTag(ROULETTE_TAB);
    RouletteBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RouletteBtn->setPosition(accp(x, (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 152));
    this->addChild(RouletteBtn, 100);
    
    CCLabelTTF* RouletteLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("shop_roulette_btn"), "HelveticaNeue-Bold", 13);
    RouletteLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(x + (155.0f/2.0f), (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 140), RouletteLabel, 160);
    
    ChangeSpr(this, 0, "ui/shop/shop_tab_a2.png", 60);
    ReleaseSubLayer();
    InitItemLayer(0);
}

void ShopLayer::InitBtn(int curTab)
{
    ChangeSpr(this, ITEM_TAB, "ui/shop/shop_tab_a1.png", 60);
    ChangeSpr(this, GOLD_TAB, "ui/shop/shop_tab_b1.png", 60);
    ChangeSpr(this, TREASURE_TAB, "ui/shop/shop_tab_b1.png", 60);
    ChangeSpr(this, ROULETTE_TAB, "ui/shop/shop_tab_c1.png", 60);
    
    if(ITEM_TAB == curTab)      ChangeSpr(this, ITEM_TAB, "ui/shop/shop_tab_a2.png", 60);
    if(GOLD_TAB == curTab)      ChangeSpr(this, GOLD_TAB, "ui/shop/shop_tab_b2.png", 60);
    if(TREASURE_TAB == curTab)  ChangeSpr(this, TREASURE_TAB, "ui/shop/shop_tab_b2.png", 60);
    if(ROULETTE_TAB == curTab)  ChangeSpr(this, ROULETTE_TAB, "ui/shop/shop_tab_c2.png", 60);
}

void ShopLayer::ReleaseSubLayer()
{
    if(shopItemLayer)
    {
        this->removeChild(shopItemLayer, true);
        shopItemLayer = NULL;
    }
    
    if(shopGoldLayer)
    {
        this->removeChild(shopGoldLayer, true);
        shopGoldLayer = NULL;
    }
    
    if(shopTreasureLayer)
    {
        this->removeChild(shopTreasureLayer, true);
        shopTreasureLayer = NULL;
    }
    
    if(shopRouletteLayer)
    {
        this->removeChild(shopRouletteLayer, true);
        shopRouletteLayer = NULL;
    }
}

void ShopLayer::InitItemList()
{
    int itemList[8] = { 10008, 10004, 10005, 10006, 10007, 10001, 10002, 10003 };
    arryItem = new CCArray;
    
    for(int i=0; i<8; ++i)
    {
        Item_Data* itemData =  FileManager::sharedFileManager()->GetItemInfo(itemList[i]);
        arryItem->addObject(itemData);
    }
}

void ShopLayer::InitItemLayer(int curPage)
{
    curTab = ITEM_TAB;
    
    InitItemList();
    
    int count = arryItem->count();
    
    int remainItem = count - (curPage * ITEM_PER_PAGE);
    
    if(remainItem > ITEM_PER_PAGE)
        LayerEndPos = ( (ITEM_PER_PAGE * 170.0f) + (ITEM_PER_PAGE * 10.0f) + 100 ) - 668;
    else if(count > ITEM_PER_PAGE)
        LayerEndPos = ( (remainItem * 170.0f) + (remainItem * 10.0f) + 100 ) - 668;
    else
        LayerEndPos = ((remainItem * 170.0f) + (remainItem * 10.0f)) - 668;
    
    LayerEndPos = LayerEndPos/2;
    //CCLOG("엔드좌표 %f", LayerEndPos);
    
    shopItemLayer = new ShopItem(this->getContentSize());
    shopItemLayer->SetItemData(arryItem);
    shopItemLayer->InitUI(curPage);
    shopItemLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    shopItemLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(shopItemLayer);
    shopItemLayer->setPositionY(0.0f);
}

void ShopLayer::InitGoldList()
{
    arryGold = new CCArray;
    
    for(int i=10009; i<10013; ++i)
    {
        Item_Data* itemData =  FileManager::sharedFileManager()->GetItemInfo(i);
        arryGold->addObject(itemData);
    }
}

void ShopLayer::InitGoldlayer(int curPage)
{
    curTab = GOLD_TAB;

    InitGoldList();
    
    int count = arryGold->count();
    
    int remainGold = count - (curPage * ITEM_PER_PAGE);
    
    if(remainGold > ITEM_PER_PAGE)
        LayerEndPos = ( (ITEM_PER_PAGE * 170.0f) + (ITEM_PER_PAGE * 10.0f) + 100 ) - 668;
    else if(count > ITEM_PER_PAGE)
        LayerEndPos = ( (remainGold * 170.0f) + (remainGold * 10.0f) + 100 ) - 668;
    else
        LayerEndPos = ((remainGold * 170.0f) + (remainGold * 10.0f)) - 668;
    
    LayerEndPos = LayerEndPos/2;
    //CCLOG("엔드좌표 %f", LayerEndPos);
    
    shopGoldLayer = new ShopGold(this->getContentSize());
    shopGoldLayer->SetGoldData(arryGold);
    shopGoldLayer->InitUI(curPage);
    shopGoldLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    shopGoldLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(shopGoldLayer);
    shopGoldLayer->setPositionY(0.0f);
}

void ShopLayer::InitTreasureLayer()
{
    curTab = TREASURE_TAB;

    shopTreasureLayer = new ShopTreasure(this->getContentSize());
    shopTreasureLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    shopTreasureLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(shopTreasureLayer);
}

void ShopLayer::InitRouletteLayer(int _medalCount)
{
    curTab = ROULETTE_TAB;
    
    shopRouletteLayer = new ShopRoulette(this->getContentSize());
    shopRouletteLayer->setMedalCount(_medalCount);
    shopRouletteLayer->InitUI();
    shopRouletteLayer->InitRouletteMachine();
    shopRouletteLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    shopRouletteLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(shopRouletteLayer);
}

void ShopLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void ShopLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //touchMovePoint = location;
    
    if (touchStartPoint.y != location.y)
    {
        if(curTab != ROULETTE_TAB && curTab != TREASURE_TAB)
        {
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
        }
        
        CCLog("상점 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
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

void ShopLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float y = this->getPositionY();
    
#if (1)
    if (moving == true && curTab != ROULETTE_TAB && curTab != TREASURE_TAB)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos < 0)
            endPos = 0;
        else if (endPos > LayerEndPos)
            endPos = LayerEndPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif

    if(LayerEndPos < 0)
    {
        if(y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, accp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if(y < 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, accp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    if(LayerEndPos > 0)
    {
        if(y > LayerEndPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, accp(0, LayerEndPos*2)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if(y < 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, accp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(false == bTouchMove)
    {
        if(GetSpriteTouchCheckByTag(this, ITEM_TAB, localPoint))
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->stopAllEffects();
            
            soundButton1();
            
            InitBtn(ITEM_TAB);
           
            ReleaseSubLayer();
            InitItemLayer(0);
        }
        
        if(GetSpriteTouchCheckByTag(this, GOLD_TAB, localPoint))
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->stopAllEffects();
            
            soundButton1();
            
            InitBtn(GOLD_TAB);
            
            ReleaseSubLayer();
            InitGoldlayer(0);
        }
        
        if(GetSpriteTouchCheckByTag(this, TREASURE_TAB, localPoint))
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->stopAllEffects();
            
            soundButton1();
            
            InitBtn(TREASURE_TAB);
            
            ReleaseSubLayer();
            InitTreasureLayer();
        }
        
        if(GetSpriteTouchCheckByTag(this, ROULETTE_TAB, localPoint))
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->stopAllEffects();
            
            soundButton1();
            
            ARequestSender::getInstance()->requestMedalCount();
        }
    }
    
    bTouchMove = false;
}


void ShopLayer::scrollingEnd()
{
    this->stopAllActions();
}

