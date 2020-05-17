//
//  PopUp.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 20..
//
//

#include "PopUp.h"
#include "ShopItem.h"
#include "ShopTreasure.h"
#include "StageLayer.h"
#include "GiftListLayer.h"
#include "FileManager.h"
#include "ShopRoulette.h"
#include "ItemLayer.h"
#include "QuestEnemy.h"

BasePopUP::BasePopUP()
{
    this->setTouchEnabled(true);
    
    playEffect("audio/popup_01.mp3");
}

BasePopUP::~BasePopUP()
{
    this->removeAllChildrenWithCleanup(true);
}

void BasePopUP::InitUI(void* data)
{
    
}

void BasePopUP::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void BasePopUP::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void BasePopUP::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

ItemPopUp::ItemPopUp()
{
    
}

ItemPopUp::~ItemPopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void ItemPopUp::InitUI(void* data)
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);

    if(NULL != data)
        PossiblePurchase(data);
    else
        ImpossiblePurchase();
}

void ItemPopUp::PossiblePurchase(void* data)
{
    Item_Data* itemData = (Item_Data*)data;
    itemID = itemData->itemID;
    
    string itemName = "\"" + itemData->itemName + "\"";
    CCLabelTTF* itemLabel = CCLabelTTF::create(itemName.c_str(), "Thonburi", 13);
    itemLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), itemLabel, 160);
    
    CCLabelTTF* buyLabel = CCLabelTTF::create("아이템 구입하시겠습니까?", "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), buyLabel, 160);
    
    CCLabelTTF* remainLabel = CCLabelTTF::create("구매 후 잔여 골드", "Thonburi", 13);
    remainLabel->setColor(COLOR_ORANGE);
    registerLabel(this, ccp(0.0f, 0.0f), accp(150.0f, 390.0f), remainLabel, 160);
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    const int myGold = pInfo->getCash();
    
    char buff[15];
    sprintf(buff, "%d", myGold - atoi(itemData->itemPrice.c_str()));
    
    CCLabelTTF* remainGoldLabel = CCLabelTTF::create(buff, "Thonburi", 13);
    remainGoldLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.0f, 0.0f), accp(340.0f, 390.0f), remainGoldLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(PURCHASE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("buy_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CANCEL_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void ItemPopUp::ImpossiblePurchase()
{
    CCLabelTTF* Label = CCLabelTTF::create("골드가 부족합니다", "Thonburi", 13);
    Label->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), Label, 160);
    
    CCLabelTTF* Labe2 = CCLabelTTF::create("충전하시겠습니까?", "Thonburi", 13);
    Labe2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 420.0f), Labe2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("charge_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CANCEL_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void ItemPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void ItemPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, PURCHASE_BTN, localPoint))
    {
        soundButton1();
               
        ResponseBuyInfo *buyInfo = ARequestSender::getInstance()->requestBuyItem(itemID);
        
        if(buyInfo)
        {
            PlayerInfo* pinfo = PlayerInfo::getInstance();
            pinfo->myCash = buyInfo->user_stat_gold;
            pinfo->myCoin = buyInfo->user_stat_coin;
            pinfo->setRevengePoint(buyInfo->user_stat_revenge);
            pinfo->setFame(buyInfo->user_stat_fame);
            pinfo->setStamina(buyInfo->user_stat_q_pnt);
            //pinfo->setDefensePoint(buyInfo->user_stat_d_pnt);
            pinfo->setBattlePoint(buyInfo->user_stat_a_pnt);
            pinfo->setUpgradePoint(buyInfo->user_stat_u_pnt);
            pinfo->setXp(buyInfo->user_stat_exp);
            pinfo->setLevel(buyInfo->user_stat_lev);
            
            UserStatLayer *info = UserStatLayer::getInstance();
            info->refreshUI();
        }
        else
            popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
        
        ShopItem* parent = (ShopItem*)this->getParent();
        parent->RemovePopUp();
    }
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        ShopLayer* shoplayer = ShopLayer::getInstance();
        shoplayer->InitBtn(1);
        shoplayer->ReleaseSubLayer();
        shoplayer->InitGoldlayer(0);
    }
    
    if(GetSpriteTouchCheckByTag(this, CANCEL_BTN, localPoint))
    {
        soundButton1();
        ShopItem* parent = (ShopItem*)this->getParent();
        parent->RemovePopUp();
    }
}

void ItemPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

GoldPopUp::GoldPopUp()
{
    
}

GoldPopUp::~GoldPopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void GoldPopUp::InitUI(void* data)
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    itemData = (Item_Data*)data;
    
    string itemName = "\"" + itemData->itemName + "\"";
    CCLabelTTF* itemLabel = CCLabelTTF::create(itemName.c_str(), "Thonburi", 13);
    itemLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), itemLabel, 160);
    
    CCLabelTTF* buyLabel = CCLabelTTF::create("골드를 충전하시겠습니까?", "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), buyLabel, 160);
    
    CCLabelTTF* remainLabel = CCLabelTTF::create("충전 후 골드", "Thonburi", 13);
    remainLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.0f), accp(150.0f, 390.0f), remainLabel, 160);
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    const int myGold = pInfo->getCash();
    
    char buff[15];
    sprintf(buff, "%d", myGold + itemData->itemAmount);
    
    CCLabelTTF* remainGoldLabel = CCLabelTTF::create(buff, "Thonburi", 13);
    remainGoldLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.0f, 0.0f), accp(340.0f, 390.0f), remainGoldLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("charge_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    
}

void GoldPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

extern "C" {
    void IAPurchase(int type);
};

void GoldPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        ShopItem* parent = (ShopItem*)this->getParent();
        parent->RemovePopUp();

        // -- 선택된 아이템
        CCLOG("선택된 아이템 ID : %d",itemData->itemID);
        switch (itemData->itemID)
        {
            case 10009:
                IAPurchase(0);
                break;
            case 10010:
                IAPurchase(1);
                break;
            case 10011:
                IAPurchase(2);
                break;
            case 10012:
                IAPurchase(3);
                break;
        }
    }
    
    if(GetSpriteTouchCheckByTag(this, RIGHT_BTN, localPoint))
    {
        soundButton1();
        
        ShopItem* parent = (ShopItem*)this->getParent();
        parent->RemovePopUp();
    }
}

void GoldPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

TreasurePopUp::TreasurePopUp()
{
    
}

TreasurePopUp::~TreasurePopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void TreasurePopUp::InitUI(void* data)
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    if(NULL == data)
    {
        LowHonor();
    }
    else if((strcmp((const char *) data, "maxcard") == 0))
    {
        MaxCard();
    }
    else
    {
        ResponseTbInfo* rewardData = (ResponseTbInfo*)data;
        
        if(0 != rewardData->card_id)
        {
            Card(rewardData->card_id);
        }
        
        if(0 != rewardData->coin)
        {
            Coin(rewardData->coin);
        }
    }
}

void TreasurePopUp::Card(int card)
{
    CCLabelTTF* Label1 = CCLabelTTF::create("축하합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 570.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("획득한 카드는", "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 540.0f), Label2, 160);
    
    CCLabelTTF* Label10 = CCLabelTTF::create("\"내 카드""\"에서 확인할 수 있습니다.", "Thonburi", 13);
    Label10->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label10, 160);
    
    ACardMaker* cardMaker = new ACardMaker();
    CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(card);
    cardMaker->MakeCardThumb(this, cardInfo, accp(520.0f, 620.0f), 178, 1000, 500);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(MYCARD_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("내 카드", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void TreasurePopUp::Coin(int coin)
{
    CCLabelTTF* Label1 = CCLabelTTF::create("축하합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 540.0f), Label1, 160);
    
    char buff[15];
    sprintf(buff, "%d", coin);
    
    string coinAmount = buff;
    
    string coinDesc = "\"" + coinAmount + "코인\"" + " 획득하셨습니다.";
    CCLabelTTF* Label2 = CCLabelTTF::create(coinDesc.c_str(), "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label2, 160);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* coinSpr = CCSprite::create("ui/shop/item_coin.png");
    coinSpr->setAnchorPoint(ccp(0.5f, 0.5f));
    coinSpr->setPosition(ccp(size.width/2, accp(440.0f)));
    this->addChild(coinSpr, 10);
    
    CCSprite* CloseBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    CloseBtn->setTag(CLOSE_BTN);
    CloseBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    CloseBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(CloseBtn, 10);
    
    CCLabelTTF* CloseLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    CloseLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), CloseLabel, 160);
}

void TreasurePopUp::LowHonor()
{
    CCLabelTTF* Label1 = CCLabelTTF::create("명성 포인트가 부족합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("배틀을 통해 명성 포인트를 획득할", "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), Label2, 160);
    
    CCLabelTTF* Label10 = CCLabelTTF::create("수 있습니다.", "Thonburi", 13);
    Label10->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 420.0f), Label10, 160);
    
    CCSprite* BattleBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    BattleBtn->setTag(BATTLE_BTN);
    BattleBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    BattleBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(BattleBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("배틀", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);

    CCSprite* CloseBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    CloseBtn->setTag(CLOSE_BTN);
    CloseBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    CloseBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(CloseBtn, 10);
    
    CCLabelTTF* CloseLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    CloseLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), CloseLabel, 160);
}

void TreasurePopUp::MaxCard()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);

    CCLabelTTF* Label2 = CCLabelTTF::create("카드 저장공간이 부족합니다. \n ""\"카드->내 카드""\" 에서 \n 저장공간을 확보하세요", "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(MYCARD_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("내 카드", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void TreasurePopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void TreasurePopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, BATTLE_BTN, localPoint))
    {
        MainScene::getInstance()->setEnableMainMenu(true);
        UserStatLayer::getInstance()->setEnableMenu(true);
        ShopLayer::getInstance()->setTouchEnabled(true);
        
        soundButton1();
        
        MainScene* main = MainScene::getInstance();
        main->curLayerTag = 2;
        CCMenu* pMenu = (CCMenu*)main->getChildByTag(99);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(i);
            item->unselected();
        }
        
        CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(2);
        item->selected();
        
        ARequestSender::getInstance()->requestOpponent();
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(2);
    }

    if(GetSpriteTouchCheckByTag(this, MYCARD_BTN, localPoint))
    {
        MainScene::getInstance()->setEnableMainMenu(true);
        UserStatLayer::getInstance()->setEnableMenu(true);
        ShopLayer::getInstance()->setTouchEnabled(true);
        
        soundButton1();
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_CARD);
        /*
        MainScene* main = MainScene::getInstance();
        main->curLayerTag = 1;
        CCMenu* pMenu = (CCMenu*)main->getChildByTag(99);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(i);
            item->unselected();
        }

        CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(1);
        item->selected();
     
        main->releaseSubLayers();
        main->initCardLayer();
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(1);
         */
    }

    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        MainScene::getInstance()->setEnableMainMenu(true);
        UserStatLayer::getInstance()->setEnableMenu(true);
        ShopLayer::getInstance()->setTouchEnabled(true);
        
        soundButton1();
        
        ShopTreasure* parent = (ShopTreasure*)this->getParent();
        parent->RemovePopUp();
    }

}

void TreasurePopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

RoulettePopUP::RoulettePopUP()
{
    
}

RoulettePopUP::~RoulettePopUP()
{
    
}

void RoulettePopUP::InitUI(void *data)
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);

    if(NULL == data)
    {
        MaxCard();
    }
    else
    {
        ResponseRoulette* rouletteData = (ResponseRoulette*)data;
        
        if(0 != rouletteData->item_id)
        {
            Item(rouletteData->item_id);
        }
        
        if(0 != rouletteData->card_id)
        {
            Card(rouletteData);
        }
        
        if(0 != rouletteData->coin)
        {
            Coin(rouletteData->coin);
        }
        
        CC_SAFE_DELETE(rouletteData); 
    }
}

void RoulettePopUP::Coin(int coin)
{
    PlayerInfo::getInstance()->addCoin(coin);
    UserStatLayer::getInstance()->refreshUI();
    
    CCLabelTTF* Label1 = CCLabelTTF::create("축하합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 540.0f), Label1, 160);
    
    char buff[15];
    sprintf(buff, "%d", coin);
    
    string coinAmount = buff;
    
    string coinDesc = "\"" + coinAmount + "코인\"" + " 획득하셨습니다.";
    CCLabelTTF* Label2 = CCLabelTTF::create(coinDesc.c_str(), "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label2, 160);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* coinSpr = CCSprite::create("ui/shop/item_coin.png");
    coinSpr->setAnchorPoint(ccp(0.5f, 0.5f));
    coinSpr->setPosition(ccp(size.width/2, accp(440.0f)));
    this->addChild(coinSpr, 10);
    
    CCSprite* CloseBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    CloseBtn->setTag(CLOSE_BTN);
    CloseBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    CloseBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(CloseBtn, 10);
    
    CCLabelTTF* CloseLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    CloseLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), CloseLabel, 160);    
}

void RoulettePopUP::Card(ResponseRoulette* data)
{
    CardInfo *card = new CardInfo();
    card->setId(data->card_id);
    card->setExp(data->card_exp);
    card->setLevel(data->card_level);
    card->setSkillEffect(data->card_skill_effect);
    card->setDefence(data->card_defense);
    card->setAttack(data->card_attack);
    card->setSrl(data->card_srl);
    PlayerInfo::getInstance()->addCard(data->card_id, card);

    CCLabelTTF* Label1 = CCLabelTTF::create("축하합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 570.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("획득한 카드는", "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 540.0f), Label2, 160);
    
    CCLabelTTF* Label10 = CCLabelTTF::create("확인할 수 있습니다.", "Thonburi", 13);
    Label10->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label10, 160);
    
    ACardMaker* cardMaker = new ACardMaker();
    CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(data->card_id);
    cardMaker->MakeCardThumb(this, cardInfo, accp(520.0f, 620.0f), 178, 1000, 500);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void RoulettePopUP::Item(int itemId)
{
    CCLabelTTF* Label1 = CCLabelTTF::create("축하합니다", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 570.0f), Label1, 160);
    
    Item_Data* data = FileManager::sharedFileManager()->GetItemInfo(itemId);
    
    string itemDesc = "\"" + data->itemName + "\"" + " 획득하셨습니다.";
    CCLabelTTF* Label2 = CCLabelTTF::create(itemDesc.c_str(), "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label2, 160);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    string path = "ui/shop/";
    path+=data->itemImgPath;
    CCSprite* coinSpr = CCSprite::create(path.c_str());
    coinSpr->setAnchorPoint(ccp(0.5f, 0.5f));
    coinSpr->setPosition(ccp(size.width/2, accp(440.0f)));
    this->addChild(coinSpr, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void RoulettePopUP::MaxCard()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("카드 저장공간이 부족합니다. \n ""\"카드->내 카드""\" 에서 \n 저장공간을 확보하세요", "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(MYCARD_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("내 카드", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void RoulettePopUP::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void RoulettePopUP::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        soundButton1();
        
        ShopRoulette* parent = (ShopRoulette*)this->getParent();
        parent->RemovePopUp();
    }
    
    if(GetSpriteTouchCheckByTag(this, MYCARD_BTN, localPoint))
    {
        soundButton1();
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_CARD);
        /*
        MainScene* main = MainScene::getInstance();
        main->curLayerTag = 1;
        CCMenu* pMenu = (CCMenu*)main->getChildByTag(99);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(i);
            item->unselected();
        }
        
        CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(1);
        item->selected();
        
        main->releaseSubLayers();
        main->initCardLayer();
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(1);
         */
    }

}

void RoulettePopUP::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

QuestPopUp::QuestPopUp()
{
    
}

QuestPopUp::~QuestPopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void QuestPopUp::InitUI(void *data)
{
    if(NULL == data)
        TeamEdit();
    else if (strcmp((const char *) data, "charge") == 0)
        Charge();
    else if (strcmp((const char *) data, "replay") == 0)
        Replay();
    else if (strcmp((const char *) data, "noReward") == 0)
        NoReward();
}

void QuestPopUp::TeamEdit()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("공격 팀에 리더 카드가 설정되어", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 540.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("있지 않아 수행할 수 없습니다.", "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("리더를 설정하시겠습니까?", "Thonburi", 13);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), Label3, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(TEAM_EDIT_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("설정", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void QuestPopUp::Charge()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("스테미나가 부족합니다", "Thonburi", 13);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("그린 젬을 사용하여 포인트를 회복하세요.", "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("charge_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void QuestPopUp::Replay()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("클리어한 스테이지입니다.\n다시하시겠습니까?", "Thonburi", 13);
    Label1->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 440.0f), Label1, 160);
    
/*    CCLabelTTF* Label2 = CCLabelTTF::create("다시하시겠습니까?", "Thonburi", 13);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 510.0f), Label2, 160);*/
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(REPLAY_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("다시하기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void QuestPopUp::NoReward()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_l.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("카드 저장공간이 부족하여 \n퀘스트를 수행할 수 없습니다. \n ""\"카드->내 카드""\" 에서 \n 저장공간을 확보하세요", "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(MYCARD_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("내 카드", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void QuestPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void QuestPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        
        ARequestSender::getInstance()->requestItemListAsync();
        
        //dojo->ReleaseLayer();
        //dojo->InitItemLayer(0);
        
    }
    
    if(GetSpriteTouchCheckByTag(this, TEAM_EDIT_BTN, localPoint))
    {
        soundButton1();
        
        MainScene* main = MainScene::getInstance();
        main->curLayerTag = 1;
        CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(1);
        item1->selected();
        
        main->releaseSubLayers();
        main->initTemaEditLayer();
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(1);
    }
    
    if(GetSpriteTouchCheckByTag(this, MYCARD_BTN, localPoint))
    {
        soundButton1();
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_CARD);
        /*
        MainScene* main = MainScene::getInstance();
        main->curLayerTag = 1;
        CCMenu* pMenu = (CCMenu*)main->getChildByTag(99);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(i);
            item->unselected();
        }
        
        CCMenuItemImage *item = (CCMenuItemImage *)pMenu->getChildByTag(1);
        item->selected();
        
        main->releaseSubLayers();
        main->initCardLayer();
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(1);
         */
    }

    if(GetSpriteTouchCheckByTag(this, REPLAY_BTN, localPoint))
    {
        soundButton1();
        StageLayer *stageLayer = StageLayer::getInstance();
        if (stageLayer)
            stageLayer->ReleaseAndInitQuestFullScreen();
    }
    
    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        soundButton1();
        
        StageLayer* parent = (StageLayer*)this->getParent();
        parent->removePopUp();
        
    }

}

void QuestPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

ItemUsePopUp::ItemUsePopUp() : cardOpen(NULL)
{
    itemID = 0;
}

ItemUsePopUp::~ItemUsePopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void ItemUsePopUp::InitUI(void* data)
{
    ItemInfo* itemData = (ItemInfo*)data;
    
    itemID = itemData->itemID;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    if(10008 == itemID)
    {
        CCLabelTTF* buyLabel = CCLabelTTF::create("카드 패키지를 개봉하시겠습니까? \n 개봉된 카드는 \'카드 -> 내 카드\'에서 \n확인 하실 수 있습니다.", "Thonburi", 13);
        buyLabel->setColor(COLOR_WHITE);
        registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, 450.0f), buyLabel, 160);
    }
    else
    {
        CCLabelTTF* buyLabel = CCLabelTTF::create("아이템을 사용하시겠습니까? \n 사용한 아이템은 환불이 되지 않습니다.", "Thonburi", 13);
        buyLabel->setColor(COLOR_WHITE);
        registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, 450.0f), buyLabel, 160);
    }
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(USE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("사용", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}
void ItemUsePopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void ItemUsePopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, USE_BTN, localPoint))
    {
        soundButton1();
        
        ResponseUseInfo *useInfo = ARequestSender::getInstance()->requestUseItem(itemID);
        
        if(NULL == useInfo)
        {
            ItemListLayer* parent = (ItemListLayer*)this->getParent();
            parent->RemovePopUp();

            popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
            return;
        }
        
        if (atoi(useInfo->res) != 0)
        {
            ItemListLayer* parent = (ItemListLayer*)this->getParent();
            parent->RemovePopUp();

            popupNetworkError(useInfo->res, useInfo->msg, "requestUseItem");
            return;
        }

        if(useInfo->cards)
        {
            ItemListLayer* parent = (ItemListLayer*)this->getParent();
            parent->RemovePopUp();
            
            cardOpen = new CardPackOpen(this->getContentSize(), useInfo->cards);
            cardOpen->setAnchorPoint(ccp(0.0f, 0.0f));
            cardOpen->setPosition(accp(0.0f, 0.0f));
            cardOpen->setTag(94556);
            MainScene::getInstance()->addChild(cardOpen, 9000);
        
            for (int i=0; i<useInfo->cards->count();i++){
                AReceivedCard* r_card = (AReceivedCard*)useInfo->cards->objectAtIndex(i);
                
                CardInfo *card = new CardInfo();
                card->setId(r_card->card_id);
                
                card->setSrl(r_card->card_srl);
                card->setLevel(r_card->card_lev);
                card->setExp(r_card->card_exp);
                card->setAttack(r_card->card_attack);
                card->setDefence(r_card->card_defense);
                card->setSkillEffect(r_card->card_skill_effect);
                //card->sets r_card->card_skill_lev);
                
                PlayerInfo::getInstance()->addCard(r_card->card_id, card);
            }
        }
        else if(10006 == itemID || 10007 == itemID )
        {
            ARequestSender::getInstance()->requestItemListAsync();
            // -- 레드잼 팩, 그린잼 팩
        }
        else
        {
            PlayerInfo* pinfo = PlayerInfo::getInstance();
            pinfo->myCoin = useInfo->user_stat_coin;
            pinfo->setCash(useInfo->user_stat_gold);
            pinfo->setFame(useInfo->user_stat_fame);
            pinfo->setStamina(useInfo->user_stat_q_pnt);
            //pinfo->setDefensePoint(useInfo->user_stat_d_pnt);
            pinfo->setBattlePoint(useInfo->user_stat_a_pnt);
            pinfo->setUpgradePoint(useInfo->user_stat_u_pnt);
            pinfo->setXp(useInfo->user_stat_exp);
            pinfo->setLevel(useInfo->user_stat_lev);
            
            UserStatLayer *info = UserStatLayer::getInstance();
            info->refreshUI();
            
            ARequestSender::getInstance()->requestItemListAsync();
        }
    
        //DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
        //dojo->ReleaseLayer();
        //dojo->InitItemLayer(0);
        
        //ARequestSender::getInstance()->requestItemListAsync();
    }
    
    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        soundButton1();
        
        ItemListLayer* parent = (ItemListLayer*)this->getParent();
        parent->RemovePopUp();

    }
}

void ItemUsePopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

GiftPopUp::GiftPopUp()
{
    srlID = 0;
}

GiftPopUp::~GiftPopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void GiftPopUp::InitUI(void* data)
{
    //Item_Data* itemData = (Item_Data*)data;
    //itemID = itemData->itemID;
    
    CCSize winSize = GameConst::WIN_SIZE;// CCDirector::sharedDirector()->getWinSize();

    gift = (GiftInfo*)data;
    srlID = gift->giftSrl;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);

    
    CCLabelTTF* buyLabel = CCLabelTTF::create("아이템을 받으시겠습니까?\n선물로 받은 아이템은 환불이 \n되지 않습니다.", "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(winSize.width/2, accp(390.0f)), buyLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(RECEIVE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("받기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
}

void GiftPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void GiftPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, RECEIVE_BTN, localPoint))
    {
        soundButton1();

        ResponseItemInfo* itemInfo = ARequestSender::getInstance()->requestItemList();
        
        if(NULL == itemInfo)
        {
            popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
            return;
        }
        
        if (atoi((const char*)itemInfo->res) == 0)
        {
            PlayerInfo::getInstance()->MyItemCount = 0;
            
            PlayerInfo::getInstance()->myItemList = new CCArray();
            
            for(int i=0;i<itemInfo->itemList->count();i++)
            {
                ItemInfo* item = (ItemInfo*)itemInfo->itemList->objectAtIndex(i);
                CCLog(" item id:%d count:%d", item->itemID, item->count);
                PlayerInfo::getInstance()->MyItemCount = PlayerInfo::getInstance()->MyItemCount + item->count;
                PlayerInfo::getInstance()->myItemList->addObject(item);
            }
        }
        else{
            popupNetworkError(itemInfo->res, itemInfo->msg,"requestItemList");
            return;
        }

        const int giftID = gift->giftID;
        
        // -- 코인, 골드는 받는 즉시 use 처리
        if(10009 == giftID || 10010 == giftID || 10011 == giftID || 10012 == giftID)
        {
            // --받기
            ARequestSender::getInstance()->requestGiftReceive(gift->giftSrl);
            
            // -- 사용
            ResponseUseInfo *useInfo = ARequestSender::getInstance()->requestUseItem(gift->giftID);
            
            if(NULL == useInfo)
            {
                popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
                return;
            }
            
            if (atoi(useInfo->res) != 0){
                popupNetworkError(useInfo->res, useInfo->msg, "requestUseItem");
                return;
            }
        
            PlayerInfo* pinfo = PlayerInfo::getInstance();
            
            pinfo->myCoin = useInfo->user_stat_coin;
            pinfo->setCash(useInfo->user_stat_gold);
            pinfo->setFame(useInfo->user_stat_fame);
            pinfo->setStamina(useInfo->user_stat_q_pnt);
            //pinfo->setDefensePoint(useInfo->user_stat_d_pnt);
            pinfo->setBattlePoint(useInfo->user_stat_a_pnt);
            pinfo->setUpgradePoint(useInfo->user_stat_u_pnt);
            pinfo->setXp(useInfo->user_stat_exp);
            pinfo->setLevel(useInfo->user_stat_lev);
            
            UserStatLayer *info = UserStatLayer::getInstance();
            info->refreshUI();
            
            // -- 보관함 레이어 초기화
            ARequestSender::getInstance()->requestGiftListAsync();
        }
        else if(10013 == giftID)
        {
            // -- 메달은 보관함 이동 x
            // --받기
            ARequestSender::getInstance()->requestGiftReceive(gift->giftSrl);
            
            // -- 보관함 레이어 초기화
            ARequestSender::getInstance()->requestGiftListAsync();
        }
        else
        {
            const int myItemcnt = PlayerInfo::getInstance()->MyItemCount;
            
            if(myItemcnt < 999)
            {
                ARequestSender::getInstance()->requestGiftReceive(srlID);
                
                // -- 보관함 레이어 초기화
                ARequestSender::getInstance()->requestGiftListAsync();
            }
            else
            {
                GiftListLayer* parent = (GiftListLayer*)this->getParent();
                parent->removePopUp();
                
                popupOk("저장할 공간이 없어 이동할 수 없습니다.\n 아이템을 사용하여 공간을 확보하세요");
            }
        }
    }
    
    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        soundButton1();
        
        GiftListLayer* parent = (GiftListLayer*)this->getParent();
        parent->removePopUp();
        
    }
}

void GiftPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

/////////////////////////////////////////////////////////////////////////////////
SocialPopUp::SocialPopUp() : friendInfo(NULL), kakaoInfo(NULL), xb(NULL)
{
    
}

SocialPopUp::~SocialPopUp()
{
    this->removeAllChildrenWithCleanup(true);
    
    CC_SAFE_DELETE(xb);
}

void SocialPopUp::InitUI(void* data)
{
    friendInfo = (UserMedalInfo*)data;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 210.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* sendLabel = CCLabelTTF::create("메달을 보내시겠습니까?\n친구에게 메달을 보내면\n 자신도 메달을 받습니다.", "HelveticaNeue-Bold", 12);
    sendLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, 430.0f), sendLabel, 160);
       
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(SEND_MADAL_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 215.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("보내기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f-10), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 215.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f-10), RightLabel, 160);
}

void SocialPopUp::InitUI2(AKakaoUser *user)
{
    kakaoInfo = user;
    xb = new XBridge();
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 210.0f));
    this->addChild(popupBG);
    
    std::string msg = user->nickname;
    msg.append("님께\n 카카오톡으로 초대 메시지를\n보내시겠어요?");
    CCLabelTTF* sendLabel = CCLabelTTF::create(msg.c_str(), "HelveticaNeue-Bold", 12);
    sendLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, 430.0f), sendLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(INVITE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 215.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("보내기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f-10), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(CLOSE_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 215.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f-10), RightLabel, 160);
}

void SocialPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void SocialPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, SEND_MADAL_BTN, localPoint))
    {
        soundButton1();
        
        SocialListLayer* parent = (SocialListLayer*)this->getParent();
        parent->RemovePopUp();
        
        
        if (ARequestSender::getInstance()->requestSendMedal(friendInfo->user->userID))
        {
            CCLog("send medal ok");
            
            ChangeSpr(SocialLayer::getInstance()->pSocialListlayer, friendInfo->tag, 0, "ui/shop/spin_m_btn_lock2.png", 60);
            
            CC_SAFE_DELETE(friendInfo);
        
            //SocialLayer::getInstance()->pSocialListlayer;
            //popupOk("메달을 보냈습니다");
        }
        else
        {
            CC_SAFE_DELETE(friendInfo);
            
            popupNetworkError("error", "", "requestSendMedal");
        }
        
    }
    
    if(GetSpriteTouchCheckByTag(this, INVITE_BTN, localPoint))
    {
        soundButton1();
        
        SocialListLayer* parent = (SocialListLayer*)this->getParent();
        parent->RemovePopUp();
        
        xb->sendKakaoMsg("캡콤의 캐릭터가 총출동!!! 전설의 파이터 세계에서 같이 싸우자 친구야!!!", kakaoInfo->userID);
        // for test
        //MainScene::getInstance()->reservePopup(5);
        
        //PlayerInfo::getInstance()->recordMedalSentTime(kakaoInfo->userID);
    }

    if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        soundButton1();
        
        SocialListLayer* parent = (SocialListLayer*)this->getParent();
        parent->RemovePopUp();
        
    }
}

void SocialPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}


/////////////////////////////////////////////////////////////////////////////////
TutorialPopUp::TutorialPopUp()
{
    
}

TutorialPopUp::~TutorialPopUp()
{
    this->removeAllChildrenWithCleanup(true);
}

void TutorialPopUp::InitUI(void *data)
{
    page = *((int*) data);
    switch (*((int*) data))
    {
        case 0:
            displayPage0();
            break;
        case 1:
            displayPage1();
            break;
        case 2:
            displayPage2();
            break;
        case 3:
            displayPage3();
            break;
        case 4:
            displayPage4();
            break;
        case 5:
            displayPage5();
            break;
        case 6:
            displayPage6();
            break;
        case 7:
            displayPage7();
            break;
        case 8:
            displayPage8();
            break;
        case 9:
            displayPage9();
            break;
        case 10:
            displayPage10();
            break;
        case 11:
            displayPage11();
            break;
    }
    
    setTouchEnabled(true);
}

void TutorialPopUp::displayPage0()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [1/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("FOC 세계로 오신것을 환영합니다.", "Thonburi", 15);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 825.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("FOC의 최강자가 되기 위해서는\n세계 각지에 있는 동료들을 모아야 합니다.", "Thonburi", 15);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 735.0f), Label3, 160);

    CCLabelTTF* Label4 = CCLabelTTF::create("마침, 당신의 동료가 되기 위해 사람들이 모였군요!", "Thonburi", 15);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 695.0f), Label4, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_01_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 354.0f));
    this->addChild(image01);

    CCLabelTTF* Label5 = CCLabelTTF::create("(축하합니다. 당신의 동료는 \'카드 > 내 카드\'에서\n확인 가능합니다.)", "Thonburi", 15);
    Label5->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 270.0f), Label5, 160);

    CCLabelTTF* Label6 = CCLabelTTF::create("하지만, 이걸로는 부족합니다.\n동료가 많을수록 더욱 든든하겠죠?", "Thonburi", 15);
    Label6->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 190.0f), Label6, 160);
    
    CCLabelTTF* Label7 = CCLabelTTF::create("퀘스트를 통해 많은 동료를 모을 수 있습니다.", "Thonburi", 15);
    Label7->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 155.0f), Label7, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(NEXT_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("다음단계", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage1()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [2/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("악당에게 붙잡혀 있는 인질이 있습니다.\n악당을 물리치세요. 그럼 당신의 동료가 될 것입니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 790.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("먼저, 챕터를 선택하세요. \"시작\" 버튼을 터치 합니다.", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 735.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_02_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 470.0f));
    this->addChild(image01);

    CCLabelTTF* Label4 = CCLabelTTF::create("다음에는 스테이지의 \"시작\" 버튼을 터치 합니다.", "Thonburi", 14);
    Label4->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 440.0f), Label4, 160);
    
    CCSprite* image02 = CCSprite::create("ui/tutorial/tutorial_contents_02_2.png");
    image02->setAnchorPoint(ccp(0.5f, 0.0f));
    image02->setPosition(accp(640.0f /2, 250.0f));
    this->addChild(image02);

    CCLabelTTF* Label5 = CCLabelTTF::create("그럼, 악당들과의 전투가 벌어집니다.\n그럼 전투를 시작해볼까요?", "Thonburi", 14);
    Label5->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 170.0f), Label5, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(QUEST_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("퀘스트 하러가기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage2()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [3/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("당신 앞에 악당이 서 있습니다. 너무 겁먹지 마세요.\n당신은 충분히 이길 수 있습니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 790.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("적이 보이는 화면을 터치 합니다.\n화면 어디를 터치해도 적을 공격합니다.", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 705.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_03_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 270.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("공격을 하면 적의 체력이 소모됩니다.\n체력이 모두 소모되면 적이 쓰러집니다.\n자! 공격해봅시다.", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 170.0f), Label4, 160);
    
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(CLOSE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("적 공격해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::displayPage3()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [4/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("아주 잘하셨습니다. 멋진 연속 공격으로 적이 기절\n해버렸군요. 적을 \"KO\"시킬 수 있는 절호의 기회\n입니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 760.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("\"보너스 콤보 타임\"을 통해 일격을 날리세요.", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 705.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_04_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 290.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("화면상에 나타나는 붉은 공격 마크를 터치하세요!\n사라지기 전에 빠르게 터치해야 합니다.", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 210.0f), Label4, 160);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("시간안에 많은 공격 마크를 터치하여\n콤보를 쌓고 보너스 포인트를 획득하세요!", "Thonburi", 14);
    Label5->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 130.0f), Label5, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(CLOSE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("일격 날려보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage4()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [5/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("와우! 적을 물리치셨군요. 붙잡혀 있던 인질이 당신과\n함께 하게 되었습니다. 이렇게 퀘스트를 통해 더욱더\n많은 동료를 얻을 수 있습니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 760.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("또한 자신이 소유하고 있는 동료를 \"합성\"을 통해 강화\n시킬 수 있습니다.", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 675.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_05_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 300.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("(합성시 공격력과 체력이 상승합니다.)", "Thonburi", 14);
    Label4->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 220.0f), Label4, 160);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("즉, 강력한 자신만의 군대를 만들 수 있는 것이지요!\n그렇다면 망설일 필요가 없지요! \"합성\"을 해봅시다!", "Thonburi", 14);
    Label5->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 130.0f), Label5, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(FUSION_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("합성해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage5()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [6/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("\"합성\" 방법은 매우 간단해요!", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 820.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("우선 합성 후 강화시킬 동료를 선택하고,\n합성에 쓰일 동료를 선택 한 후", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 745.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_06_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 470.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("\"FUSION\" 버튼을 터치하면 끝!\n합성에 쓰인 카드는 강화 후 소멸됩니다.", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 400.0f), Label4, 160);
    
    CCSprite* image02 = CCSprite::create("ui/tutorial/tutorial_contents_06_2.png");
    image02->setAnchorPoint(ccp(0.5f, 0.0f));
    image02->setPosition(accp(640.0f /2, 120.0f));
    this->addChild(image02);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(CLOSE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("합성해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage6()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [7/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("보셨죠? 당신의 동료가 더욱 강해졌습니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 820.0f), Label2, 160);
    
    CCLabelTTF* Label22 = CCLabelTTF::create("이렇게 동료를 모으고 강화시키세요!\nFOC 세계의 최강자로 군림하세요!", "Thonburi", 14);
    Label22->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 740.0f), Label22, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("물론 당신과 동일한 목표를 가진 이들과 경쟁해야\n하지만요... 아! 자신있다구요? 역시 당신답군요!", "Thonburi", 14);
    Label3->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 660.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_07_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 440.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("좋습니다. 다른 이와 한번 겨뤄봅시다.\n우선, 다른 이와 대전을 하기 전에 준비를 해야 합니다.", "Thonburi", 14);
    Label4->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 360.0f), Label4, 160);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("바로 대전에서 사용할 \"팀\"을 구성하는 거지요.", "Thonburi", 14);
    Label5->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 300.0f), Label5, 160);
    
    CCLabelTTF* Label6 = CCLabelTTF::create("준비 되셨나요?", "Thonburi", 14);
    Label6->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 270.0f), Label6, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(NEXT_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("다음단계", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::displayPage7()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [8/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("팀 구성은 매우 간단합니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 820.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("팀은 자신의 동료로 구성합니다.\n최소 1명~5명의 인원(카드)으로 구성할 수 있습니다.", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 745.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_08_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 410.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("\"팀설정\" 버튼을 터치하여 \n팀으로 구성할 카드를 고르기만 하면 됩니다.\n우선 \"팀설정\" 버튼을 눌러볼까요?", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 305.0f), Label4, 160);
    
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(TEAM_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("팀 설정해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}

void TutorialPopUp::displayPage8()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [9/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("OK!!! \"팀설정\" 버튼을 터치하면 든든한 당신의\n동료들이 짠!하고 나타납니다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 790.0f), Label2, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_09_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 290.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("첫번째, 설정할 카드를 위 그림처럼 터치!\n두번째, 설정할 위치를 위 그림처럼 터치!\n", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 215.0f), Label3, 160);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("가운데에 설정한 \"리더카드\"는\n 홈 화면에 노출됩니다\n 이제 팀 설정 마무리하고 대전으로 넘어갑시다.", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 95.0f), Label4, 160);
    
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(NEXT_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("다음단계", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::displayPage9()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [10/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("대전에서 팀 설정은 큰 영향을 미칩니다.\n 팀구성에 앞서 간단한 팁을 알려드릴께요!", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 790.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("첫번째, 대전은 자신의 팀 공격력 VS 상대방의\n 팀 체력 결전!", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 705.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_10_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 500.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("두번째, 리더 카드는 스킬 발동 확률이 높다!", "Thonburi", 14);
    Label4->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 460.0f), Label4, 160);
    
    CCSprite* image02 = CCSprite::create("ui/tutorial/tutorial_contents_10_2.png");
    image02->setAnchorPoint(ccp(0.5f, 0.0f));
    image02->setPosition(accp(640.0f /2, 250.0f));
    this->addChild(image02);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("위 2가지만 기억하면\n강호의 세계에서 강자가 될 수 있습니다.", "Thonburi", 14);
    Label5->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 170.0f), Label5, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(CLOSE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("팀 설정 해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::displayPage10()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [11/12]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("이젠 모든 준비가 다 끝난 것 같으니\n 상대방에게 도전해봅시다.", "Thonburi", 14);
    Label2->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 790.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("도전 방법은 간단합니다. 대전하고 싶은 상대방을\n선택하고 \"Battle Start\" 버튼을 터치하세요!", "Thonburi", 14);
    Label3->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 705.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_11_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 320.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("그럼 상대방과 불꽃튀기는 승부가 시작됩니다.\n아! 중요한 부분을 빼먹었네요!", "Thonburi", 14);
    Label4->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 240.0f), Label4, 160);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("대전은 자동으로 이루어집니다.", "Thonburi", 14);
    Label5->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 190.0f), Label5, 160);
    
    CCLabelTTF* Label6 = CCLabelTTF::create("즉, 당신을 경기의 감독이라고 표현하면 될까요?", "Thonburi", 14);
    Label6->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 160.0f), Label6, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(BATTLE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("대전해보기", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::displayPage11()
{
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_bg01.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Label1 = CCLabelTTF::create("게임 시작하기 [END]", "Thonburi", 20);
    Label1->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 885.0f), Label1, 160);
    
    CCLabelTTF* Label2 = CCLabelTTF::create("대전에서 승리하면 당신의 강함을 증명하는 것 뿐만\n아니라 상대방이 가진 돈과 명성 포인트를 빼앗을\n수 있어요. 비록 상대방은 화가 나겠지만요. 하하!", "Thonburi", 14);
    Label2->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 760.0f), Label2, 160);
    
    CCLabelTTF* Label3 = CCLabelTTF::create("(명성 포인트는 \"상점 > 명성샵\"에서 사용하여 카드를\n획득할 수 있습니다.)", "Thonburi", 14);
    Label3->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 665.0f), Label3, 160);
    
    CCSprite* image01 = CCSprite::create("ui/tutorial/tutorial_contents_12_1.png");
    image01->setAnchorPoint(ccp(0.5f, 0.0f));
    image01->setPosition(accp(640.0f /2, 370.0f));
    this->addChild(image01);
    
    CCLabelTTF* Label4 = CCLabelTTF::create("FOC 세계를 여행할 모든 준비가 끝난것 같군요!\n당신의 건투를 기원할께요.", "Thonburi", 14);
    Label4->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 290.0f), Label4, 160);
    
    CCLabelTTF* Label5 = CCLabelTTF::create("게임에 대한 좀 더 자세한 정보는 홈페이지에서\n확인할 수 있습니다.\n(도장 > 설정 > 고객문의에서 확인하세요)", "Thonburi", 14);
    Label5->setColor(COLOR_TUTORIAL);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 160.0f), Label5, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    LeftBtn->setTag(DONE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp((640.0f - 306.0f)/2, 30.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create("도장으로 이동", "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(640.0f / 2, 40.0f), LeftLabel, 160);
}


void TutorialPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void TutorialPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    printf("tutorial touch\n");
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, NEXT_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);

        if (tutorialProgress == 0 || tutorialProgress == 6)
        {
            PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
            tutorialProgress++;
        }
        page++;
        InitUI(&page);
    }
    else if(GetSpriteTouchCheckByTag(this, QUEST_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
        PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
        page++;

        
        ARequestSender::getInstance()->requestChapterList();
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_QUEST);
        /*
        MainScene* main = MainScene::getInstance();
        
        CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(3);
        item1->selected();
        
        main->curLayerTag = 3;
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(3);
         */

    }
    else if(GetSpriteTouchCheckByTag(this, FUSION_BTN, localPoint))
    {
        resultBG_Off();
        
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        
        PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
        page++;
        
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_CARD);
        /*
        MainScene* main = MainScene::getInstance();
        main->releaseSubLayers();
        main->initCardLayer();
        
        CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(1);
        item1->selected();
        
        main->curLayerTag = 1;
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(1);
         */
        /////////////////////////////////////
        
        DojoLayerCard* card = DojoLayerCard::getInstance();
        
        card->releaseSubLayer();
        card->InitSubLayer(CARD_TAB_SUB_2);
        card->setTouchEnabled(false);
      
        card->SetNormalSubBtns();
        card->SetSelectedSubBtn(13);

        CCMenu* pMenu2 = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
                
        for (int i=11 ;i<15; ++i)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu2->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item2 = (CCMenuItemImage *)pMenu2->getChildByTag(13);
        item2->selected();
        
        card->curLayerTag = 14;

        pMenu2->setEnabled(false);

        FusionLayer* flayer = FusionLayer::getInstance();
        flayer->setTouchEnabled(false);
        CCMenu* pMenu3 = (CCMenu*)flayer->getChildByTag(99);
        pMenu3->setEnabled(false);
        
        tutorialProgress++;
        InitUI(&page);
    }
    else if(GetSpriteTouchCheckByTag(this, TEAM_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        
        page++;
        
        MainScene* main = MainScene::getInstance();
        main->releaseSubLayers();
        main->initCardLayer();
        DojoLayerCard::getInstance()->releaseSubLayer();
        DojoLayerCard::getInstance()->InitSubLayer(CARD_TAB_SUB_1);
        DojoLayerCard::getInstance()->SetNormalSubBtns();
        DojoLayerCard::getInstance()->SetSelectedSubBtn(12);
    }
    else if(GetSpriteTouchCheckByTag(this, BATTLE_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
        PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
        page++;
        
        ARequestSender::getInstance()->requestOpponent();
        
        MainScene* main = MainScene::getInstance();
        
        CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(2);
        item1->selected();
        
        main->curLayerTag = 2;
        
        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(2);

    }
    else if(GetSpriteTouchCheckByTag(this, DONE_BTN, localPoint))
    {
        resultBG_Off();
        
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
        MainScene* main = MainScene::getInstance();
        main->initDojoLayer();
        CCMenu *menu = (CCMenu*)main->getChildByTag(99);
        menu->setEnabled(true);
        
        for (int i=0;i<5;i++)
        {
            CCMenuItemImage *item1 = (CCMenuItemImage*)menu->getChildByTag(i);
            item1->unselected();
        }
        
        CCMenuItemImage *item1 = (CCMenuItemImage*)menu->getChildByTag(0);
        item1->selected();

        main->SetNormalSubBtns();
        main->SetSelectedSubBtn(0);
        
        UserStatLayer::getInstance()->setEnableMenu(true);
        
        tutorialProgress = TUTORIAL_DONE;
        PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
        PlayerInfo::getInstance()->setTutorialCompleted(true);
        
        soundMainBG();
    }
    else if(GetSpriteTouchCheckByTag(this, CLOSE_BTN, localPoint))
    {
        // -- 퀘스트 적 공격해보기
        if(2 == tutorialProgress)
        {
            PlayerInfo::getInstance()->setTutorialProgress(1);
            EnemyLayer::getInstance()->normalComboStarted = true;
        }
        if(5 == tutorialProgress)
        {
            FusionLayer* flayer = FusionLayer::getInstance();
            flayer->setTouchEnabled(true);
            CCMenu* pMenu3 = (CCMenu*)flayer->getChildByTag(99);
            pMenu3->setEnabled(true);
        }
        if(8 == tutorialProgress)
        {
            TeamEditLayer* teamEdit = (TeamEditLayer*)TeamEditLayer::getInstance();
            teamEdit->setTouchEnabled(true);
        }
        if(9 == tutorialProgress)
        {
            TeamEditLayer* teamEdit = (TeamEditLayer*)TeamEditLayer::getInstance();
            CCMenu* pMenu = (CCMenu*)teamEdit->getChildByTag(99);
            pMenu->setEnabled(true);
        }
        if(3 == tutorialProgress)
        {
            PlayerInfo::getInstance()->setTutorialProgress(1);
            EnemyLayer::getInstance()->normalComboStarted = true;
            EnemyLayer::startUltraCombo();
        }

        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
    
}

void TutorialPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

Purchase2Popup::Purchase2Popup()
{
    
}

Purchase2Popup::~Purchase2Popup()
{
    this->removeAllChildrenWithCleanup(true);
}

void Purchase2Popup::InitUI(void* data)
{
    /*
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
 
    CCLabelTTF* buyLabel = CCLabelTTF::create((const char *)data, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, 450.0f), buyLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("buy_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    */
    ////////////
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 220-100;
    popupBG->setPosition(accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    CCLabelTTF* buyLabel = CCLabelTTF::create((const char *)data, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
        
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //LeftBtn->setPosition(accp(93.0f, 225.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("buy_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //RightBtn->setPosition(accp(342.0f, 225.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
    
}

void Purchase2Popup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void Purchase2Popup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
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
    
    if(GetSpriteTouchCheckByTag(this, RIGHT_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
}

void Purchase2Popup::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

AlertPopup::AlertPopup()
{
    
}

AlertPopup::~AlertPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void AlertPopup::InitUI(void* data)
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);

    CCLabelTTF* buyLabel = CCLabelTTF::create((const char *)data, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), buyLabel, 160);
    /*
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("buy_btn"), "Thonburi", 13);
    LeftLabel->setColor(cocos2d::ccc3(254,223, 51));
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    */
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(cocos2d::ccc3(254,223, 51));
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    
}

void AlertPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void AlertPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
    
    if(GetSpriteTouchCheckByTag(this, RIGHT_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
}

void AlertPopup::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

extern "C" {
    void openInviteMsgSuccessPopup(const char *dicId, long long kakaoID)
    {
        CCLog(" openInviteMsgSuccessPopup, kakaoID :%lld", kakaoID);
        const char *text = LocalizationManager::getInstance()->get(dicId);
        if (text == NULL || strlen(text) == 0)
            return;
        
        AlertPopup *popup = new AlertPopup;
        popup->InitUI((void *)text);
        popup->setAnchorPoint(ccp(0.0f, 0.0f));
        popup->setPosition(ccp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(popup, 5000);
        PlayerInfo::getInstance()->recordMedalSentTime(kakaoID);
        SocialInviteLayer::getInstance()->pSocialListlayer->setDisableInviteBtn(kakaoID);
    }
    
    void openInviteMsgFailPopup(int error_code)
    {
        const char *text = "메시지 발송에 실패하였습니다.";
        //const char *text = LocalizationManager::getInstance()->get(dicId);
        if (text == NULL || strlen(text) == 0)
            return;
        
        AlertPopup *popup = new AlertPopup;
        popup->InitUI((void *)text);
        popup->setAnchorPoint(ccp(0.0f, 0.0f));
        popup->setPosition(ccp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(popup, 5000);
    }
    
    
};
FusionPopup::FusionPopup()
{
    
}

FusionPopup::~FusionPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void FusionPopup::InitUI(void* data, int cost)
{
    /*
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    char buf[10];
    sprintf(buf, "%d", cost);
    
    std::string aa = "";
    aa.append((const char*)data);
    int pos = aa.find("xx");
    aa.replace(pos,2, buf);
    CCLog("aa=%s", aa.c_str());
    
    //CCLabelTTF* buyLabel = CCLabelTTF::create((const char *)data, "Thonburi", 13);
    CCLabelTTF* buyLabel = CCLabelTTF::create(aa.c_str(), "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f-50), buyLabel, 160);
    
    //CCLabelTTF* costLabel = CCLabelTTF::create("가격", "Thonburi", 13);
    //costLabel->setColor(COLOR_WHITE);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f-50), costLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    */
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 220-100;
    popupBG->setPosition(accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    char buf[10];
    sprintf(buf, "%d", cost);
    
    std::string aa = "";
    aa.append((const char*)data);
    int pos = aa.find("xx");
    aa.replace(pos,2, buf);
    CCLog("aa=%s", aa.c_str());
    
    CCLabelTTF* buyLabel = CCLabelTTF::create(aa.c_str(), "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f-50), buyLabel, 160);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //LeftBtn->setPosition(accp(93.0f, 225.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //RightBtn->setPosition(accp(342.0f, 225.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
}



void FusionPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void FusionPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);

        const int card1Srl = FusionLayer::getInstance()->fusionCard1->getSrl();
        const int card2Srl = FusionLayer::getInstance()->fusionCard2->getSrl();
        ARequestSender::getInstance()->requestFusion(card1Srl, card2Srl);
    }
    
    if(GetSpriteTouchCheckByTag(this, RIGHT_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        FusionLayer::getInstance()->popupCnt = 0;
        FusionLayer::getInstance()->setTouchEnabled(true);
    }
}

void FusionPopup::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}


TrainingPopup::TrainingPopup()
{
    
}

TrainingPopup::~TrainingPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void TrainingPopup::InitUI(void* data, int cost)
{
    //CheckLayerSize(this);
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 220-100;
    popupBG->setPosition(accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    char buf[10];
    sprintf(buf, "%d", cost);
    
    std::string aa = "";
    aa.append((const char*)data);
    int pos = aa.find("xx");
    aa.replace(pos,2, buf);
    CCLog("aa=%s", aa.c_str());
    
    CCLabelTTF* buyLabel = CCLabelTTF::create(aa.c_str(), "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f-50), buyLabel, 160);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
        
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(CHARGE_BTN);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //LeftBtn->setPosition(accp(93.0f, 225.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(RIGHT_BTN);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    //RightBtn->setPosition(accp(342.0f, 225.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    //registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
    
}

void TrainingPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void TrainingPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CHARGE_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
        //const int card1Srl = TrainingLayer::getInstance()->fusionCard1->getSrl();
        //const int card2Srl = TrainingLayer::getInstance()->fusionCard2->getSrl();
        //ARequestSender::getInstance()->requestTraining(card1Srl, card2Srl);
        
        TrainingLayer::getInstance()->loadImage();
    }
    
    if(GetSpriteTouchCheckByTag(this, RIGHT_BTN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        
        TrainingLayer::getInstance()->popupCnt = 0;
        
        TrainingLayer::getInstance()->setTouchEnabled(true);
    }
}

void TrainingPopup::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}