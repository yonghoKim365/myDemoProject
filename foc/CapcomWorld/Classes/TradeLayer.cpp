//
//  TradeLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 19..
//
//

#include "TradeLayer.h"
#include "PlayerInfo.h"
#include "CustomCCTableViewCell.h"


TradeLayer::TradeLayer(CCSize layerSize)
{
    nSelectedTab = 0; // 0 == 검색, 1 == 등록, 2 == 트레이드상태
    nSelectedSortOption = 0;
    cardRegisterStep = 0;
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    nSelectedTab = 0;
    InitLayer(nSelectedTab);
    
}


TradeLayer::~TradeLayer()
{
    
    
}

void TradeLayer::InitLayer(int _selectedTab)
{
    //CCSize size = this->getContentSize();// CCDirector::sharedDirector()->getWinSize();
    
    //InitRegisterLayer(cardForReg);
    ///*
    if (cardRegisterStep == 0)
    {
        //InitTab(_selectedTab);
        InitSubLayer(nSelectedTab);
    }
    else{
        InitRegisterLayer(cardForReg);
    }
    //*/
    
}

void TradeLayer::InitTab(int _selectedTab){
    
    CCSize size = this->getContentSize();// CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* pSpr1;
    CCSprite* pSpr2;
    CCSprite* pSpr3;
    
    CCLabelTTF* pLabel1;
    CCLabelTTF* pLabel2;
    CCLabelTTF* pLabel3;
    
    pLabel1 = CCLabelTTF::create("검색"   , "Thonburi", 13);
    pLabel2 = CCLabelTTF::create("등록" , "Thonburi", 13);
    pLabel3 = CCLabelTTF::create("트레이드상태" , "Thonburi", 13);
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel3->setColor(subBtn_color_normal);
    
    if (_selectedTab==0){
        pSpr1 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_a2.png");
        pLabel1->setColor(cocos2d::ccc3(0,132,255));
    }
    else pSpr1 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_a1.png");
    
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr1->setPosition( accp(10, size.height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - tab_btn_h));
    pSpr1->setTag(10);
    this->addChild(pSpr1, 0);
    
    if (_selectedTab==1){
        pSpr2 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_b2.png");
        pLabel2->setColor(cocos2d::ccc3(0,132,255));
    }
    else pSpr2 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_b1.png");
    
    pSpr2->setAnchorPoint(ccp(0,0));
    pSpr2->setPosition( accp(220, size.height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - tab_btn_h));
    pSpr2->setTag(11);
    this->addChild(pSpr2, 0);
    
    if (_selectedTab==2){
        pSpr3 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_c2.png");
        pLabel3->setColor(cocos2d::ccc3(0,132,255));
    }
    else pSpr3 = CCSprite::create("ui/card_tab/cards_trade_tab_btn_c1.png");
    
    pSpr3->setAnchorPoint(ccp(0,0));
    pSpr3->setPosition( accp(430, size.height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - tab_btn_h));
    pSpr3->setTag(12);
    this->addChild(pSpr3, 0);
    
    
    //pLabel1->setTag(0);
    //pLabel2->setTag(1);
    //pLabel3->setTag(2);
    
    // position the label on the center of the screen
    int btn_y = this->getContentSize().height*2 - MAIN_LAYER_TOP_MARGIN - tab_btn_h + text_offy;
    registerLabel( this,ccp(0.5,0), accp(114, btn_y),pLabel1, 130);
    registerLabel( this,ccp(0.5,0), accp(321, btn_y),pLabel2, 130);
    registerLabel( this,ccp(0.5,0), accp(525, btn_y),pLabel3, 130);
}

void TradeLayer::InitSubLayer(int _selectedTab)
{
    CCSize size = this->getContentSize();// CCDirector::sharedDirector()->getWinSize();
    /*
    std::string _path_normal[] = {
        "ui/card_tab/card_sort_attribute_btn01_1.png",
        "ui/card_tab/card_sort_attribute_btn02_1.png",
        "ui/card_tab/card_sort_attribute_btn02_1.png",
        "ui/card_tab/card_sort_attribute_btn03_1.png",
        "ui/card_tab/cards_trade_tab_sortbar1.png",
        "ui/card_tab/cards_trade_search_bar.png",
        "ui/card_tab/cards_trade_search_btn1.png"
    };
    
    std::string _path_selected[] = {
        "ui/card_tab/card_sort_attribute_btn01_2.png",
        "ui/card_tab/card_sort_attribute_btn02_2.png",
        "ui/card_tab/card_sort_attribute_btn02_2.png",
        "ui/card_tab/card_sort_attribute_btn03_2.png",
        "ui/card_tab/cards_trade_tab_sortbar1.png",
        "ui/card_tab/cards_trade_search_bar.png",
        "ui/card_tab/cards_trade_search_btn1.png"
    };
    
    //_path_normal[nSelectedSortOption] = _path_selected[nSelectedSortOption];
    
    float y = size.height*2 - MAIN_LAYER_TOP_MARGIN - tab_btn_h - _space1_h - arrange_btn_h;
    //float anc[] = { 0, 0,   0, 0,   0, 0,   0, 0,  0,0, 0,0, 0,0};
    //float pos[] = {10, y, 167, y, 323, y, 479, y, 10,y-arrange_btn_h, 365,y-arrange_btn_h, 585,y-arrange_btn_h};
    //float z[]   = { 0,0,0,0,0,0,0};
    
    //regSprites(this, 3, _path_normal, anc, pos, z, 14);
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create(_path_normal[0].c_str(), _path_selected[0].c_str(),this,menu_selector(TradeLayer::SortMenuCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp(10,0));//size.width/5 * 0,0));
    pSprBtn1->setTag(20);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create(_path_normal[1].c_str(), _path_selected[1].c_str(),this,menu_selector(TradeLayer::SortMenuCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp( 167,0));//size.width/5 * 0,0));
    pSprBtn2->setTag(21);
    
    CCMenuItemImage *pSprBtn3 = CCMenuItemImage::create(_path_normal[2].c_str(), _path_selected[2].c_str(),this,menu_selector(TradeLayer::SortMenuCallback));
    pSprBtn3->setAnchorPoint( ccp(0,0));
    pSprBtn3->setPosition( accp( 323,0));//size.width/5 * 0,0));
    pSprBtn3->setTag(22);
    
    CCMenuItemImage *pSprBtn4 = CCMenuItemImage::create(_path_normal[3].c_str(), _path_selected[3].c_str(),this,menu_selector(TradeLayer::SortMenuCallback));
    pSprBtn4->setAnchorPoint( ccp(0,0));
    pSprBtn4->setPosition( accp( 479,0));//size.width/5 * 0,0));
    pSprBtn4->setTag(23);
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1, pSprBtn2, pSprBtn3, pSprBtn4, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( accp(0, y));
    pMenu->setTag(99);
    pSprBtn1->selected();
    this->addChild(pMenu, 10);
    
    CCSprite *pSpr5 = CCSprite::create(_path_normal[4].c_str());
    pSpr5->setTag(30);
    regSprite(this, ccp(0,0), accp(10, y-arrange_btn_h), pSpr5, 0);
    CCSprite *pSpr6 = CCSprite::create(_path_normal[5].c_str());
    pSpr6->setTag(31);
    regSprite(this, ccp(0,0), accp(365, y-arrange_btn_h), pSpr6, 0);
    CCSprite *pSpr7 = CCSprite::create(_path_normal[6].c_str());
    pSpr7->setTag(32);
    regSprite(this, ccp(0,0), accp(585, y-arrange_btn_h), pSpr7, 0);

    CCLabelTTF* pLabel1 = CCLabelTTF::create("All"   , "Thonburi", 13);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("Smash" , "Thonburi", 13);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("Guard" , "Thonburi", 13);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("Throw" , "Thonburi", 13);
    CCLabelTTF* pLabel5 = CCLabelTTF::create("공격력 높은 순서로 보기" , "Thonburi", 13);
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel3->setColor(subBtn_color_normal);
    pLabel4->setColor(subBtn_color_normal);
    pLabel5->setColor(subBtn_color_normal);
    
    pLabel1->setTag(201);
    pLabel2->setTag(211);
    pLabel3->setTag(221);
    pLabel4->setTag(231);
    pLabel5->setTag(301);
    
    // position the label on the center of the screen
    int btn_y = this->getContentSize().height*2 - MAIN_LAYER_TOP_MARGIN - tab_btn_h - _space1_h - arrange_btn_h + text_offy;
    registerLabel( this,ccp(0.5,0), accp( 78+10, btn_y),pLabel1, 130);
    registerLabel( this,ccp(0.5,0), accp(233+10, btn_y),pLabel2, 130);
    registerLabel( this,ccp(0.5,0), accp(386+10, btn_y),pLabel3, 130);
    registerLabel( this,ccp(0.5,0), accp(539+10, btn_y),pLabel4, 130);
    registerLabel( this,ccp(  0,0), accp( 21+10, btn_y-arrange_btn_h),pLabel5, 130);
    */
    // init card list
    PlayerInfo *pi = PlayerInfo::getInstance();
    //float list_h = size.height*2 - MAIN_LAYER_TOP_MARGIN - tab_btn_h - _space1_h - arrange_btn_h - arrange_btn_h - _space1_h;
    float list_h = size.height*2;// - MAIN_LAYER_TOP_MARGIN;// - tab_btn_h - _space1_h - arrange_btn_h - arrange_btn_h - _space1_h;
    
    int call_from = CALL_CARDLIST_FROM_TRADE_1;
    if (_selectedTab == 1)call_from = CALL_CARDLIST_FROM_TRADE_2;
    
    cardListLayer = new CardListLayer(CCRectMake(0,0,this->getContentSize().width-10, accp(list_h)), pi->myCards, call_from, this, true, -1);
    cardListLayer->setAnchorPoint(ccp(0,0));
    cardListLayer->setPosition(accp(10,0));
    this->addChild(cardListLayer,10);
    
}

void TradeLayer::InitRegisterLayer(CardInfo *card)
{
    CCSize size = this->getContentSize();// CCDirector::sharedDirector()->getWinSize();
    /*
    std::string _path_normal[] = {
        "ui/card_tab/cards_trade_regist_card_top.png",
        "ui/card_tab/cards_trade_regist_resource_top.png",
        "ui/card_tab/card_sort_attribute_btn02_1.png",
        "ui/card_tab/card_sort_attribute_btn03_1.png",
        "ui/card_tab/cards_trade_tab_sortbar1.png",
        "ui/card_tab/cards_trade_search_bar.png",
        "ui/card_tab/cards_trade_search_btn1.png"
    };
    
    float y = size.height*2 - MAIN_LAYER_TOP_MARGIN;// - tab_btn_h - _space1_h - arrange_btn_h;
    float anc[] = { 0, 0,   0, 0,   0, 0,   0, 0,  0,0, 0,0, 0,0};
    float pos[] = {10, y, 167, y, 323, y, 479, y, 10,y-arrange_btn_h, 365,y-arrange_btn_h, 585,y-arrange_btn_h};
    float z[]   = { 0,0,0,0,0,0,0};
    
    regSprites(this, 1, _path_normal, anc, pos, z, 50);
    */
//    CCLabelTTF* pLabel1;
//    pLabel1->setColor(subBtn_color_normal);
    
    int yy = 20;
    CCSprite* pSpr1 = CCSprite::create("ui/card_tab/cards_trade_regist_btna1.png");
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr1->setPosition( accp(109, yy));
    pSpr1->setTag(90);
    this->addChild(pSpr1, 0);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("등록"   , "Thonburi", 13);
    pLabel1->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(213, yy + 5),pLabel1, 10);
    
    CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_trade_regist_btnb1.png");
    pSpr2->setAnchorPoint(ccp(0,0));
    pSpr2->setPosition( accp(325, yy));
    pSpr2->setTag(91);
    this->addChild(pSpr2, 0); // h 42
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("뒤로가기"   , "Thonburi", 13);
    pLabel2->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(431, yy + 5),pLabel2, 10);
    
    for(int i=0;i<3;i++){
        int yy = 20+42+15+(71*i)+(5*i);
        CCSprite* pSpr3 = CCSprite::create("ui/card_tab/cards_trade_regist_resource_input_bg.png");
        pSpr3->setAnchorPoint(ccp(0,0));
        pSpr3->setPosition( accp(10, yy));
        this->addChild(pSpr3, 0); //
        
        CCSprite* pSpr31 = CCSprite::create("ui/card_tab/cards_trade_regist_resource_inputbtn1.png");
        pSpr31->setAnchorPoint(ccp(0,0));
        pSpr31->setPosition( accp(530, yy + 10));
        this->addChild(pSpr31, 0);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("입력"   , "Thonburi", 13);
        pLabel1->setColor(subBtn_color_normal);
        registerLabel( this,ccp(0.5,0), accp(575, yy + 20),pLabel1, 10);
    }
    
    
    yy = 20+42+15+71+5+71+5+71+10;
    CCSprite* pSpr6 = CCSprite::create("ui/card_tab/cards_trade_regist_resource_top.png");
    pSpr6->setAnchorPoint(ccp(0,0));
    pSpr6->setPosition( accp(10, yy));
    this->addChild(pSpr6, 0); //
    
    CustomCCTableViewCell *cell = new CustomCCTableViewCell();
    cell->MakeCell(card, CALL_CARDLIST_FROM_TRADE_REG,-1);
    cell->setAnchorPoint(ccp(0,0));
    cell->setPosition(accp(10, yy + 55));
    this->addChild(cell, 10);
    
    
    CCSprite* pSpr7 = CCSprite::create("ui/card_tab/cards_trade_regist_card_top.png");
    pSpr7->setAnchorPoint(ccp(0,0));
    pSpr7->setPosition( accp(10, size.height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - 55));
    this->addChild(pSpr7, 0);
    
}

void TradeLayer::MenuCallback(cocos2d::CCObject *pSender)
{
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    switch(tag){
        case 0:
            break;
    }
}

void TradeLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
}

void TradeLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=0;i<3;i++){
        CCSprite *pic = (CCSprite*)this->getChildByTag(10+i);
        if (pic != NULL){
            CCRect *pic1rect = new CCRect(pic->boundingBox());
            if (pic1rect->containsPoint(localPoint)){
                //this->removeChild(pic, true);
                CCLog("pic button press,%d",i);
                nSelectedTab = i;
                nSelectedSortOption=0;
                if (nSelectedTab==1)cardRegisterStep=0;
                
                this->removeAllChildrenWithCleanup(true);
                InitTab(nSelectedTab);
                InitSubLayer(nSelectedTab);
            }
        }
    }
    
    for(int i=0;i<3;i++){
        CCSprite *pic = (CCSprite*)this->getChildByTag(30+i);
        if (pic != NULL){
            CCRect *pic1rect = new CCRect(pic->boundingBox());
            if (pic1rect->containsPoint(localPoint)){
                //this->removeChild(pic, true);
                nSelectedSortOption=i;
                
                switch(i){
                    case 0:
                        //CCLog(" sort drop down");
                        //cardListLayer->SortByAttack();
                        break;
                    case 1:
                        // search text
                        CCLog(" search text field");
                        break;
                    case 2:
                        // search buton
                        CCLog(" search button");
                        break;
                }
            }
        }
    }
    
    CCSprite *btn1 = (CCSprite*)this->getChildByTag(90);
    if (btn1 != NULL){
        CCRect *pic1rect = new CCRect(btn1->boundingBox());
        if (pic1rect->containsPoint(localPoint)){
            CCLog(" button Register");
            this->removeAllChildrenWithCleanup(true);
            nSelectedTab = 1;
            cardRegisterStep = 0;
            InitTab(nSelectedTab);
            InitSubLayer(nSelectedTab);
        }
    }
    
    CCSprite *btn2= (CCSprite*)this->getChildByTag(91);
    if (btn2 != NULL){
        CCRect *pic1rect = new CCRect(btn2->boundingBox());
        if (pic1rect->containsPoint(localPoint)){
            CCLog(" button back");
            this->removeAllChildrenWithCleanup(true);
            nSelectedTab = 1;
            cardRegisterStep = 0;
            InitTab(nSelectedTab);
            InitSubLayer(nSelectedTab);
        }
    }
    
    
    
}

void TradeLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
}

/*
void TradeLayer::SortMenuCallback(CCObject *pSender)
{
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    if (tag >= 20 && tag <= 23)
    {
        CCMenu *menu = (CCMenu*)this->getChildByTag(99);
        if (menu)
        {
            for (int scan = 0;scan < 4;scan++)
            {
                CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(20+scan);
                if (!item)
                    continue;
                item->unselected();
            }
        }
    }
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 20:
            cardListLayer->resetList();
            item->selected();
            break;
        case 21:
            cardListLayer->FilteringCardList(ATRB_SMASH);
            item->selected();
            break;
        case 22:
            cardListLayer->FilteringCardList(ATRB_GUARD);
            item->selected();
            break;
        case 23:
            cardListLayer->FilteringCardList(ATRB_THROW);
            item->selected();
            break;
    }
}
*/

void TradeLayer::ButtonA(CardInfo *card){
    CCLog("Trade!!");
    if (nSelectedTab == 1 && cardRegisterStep == 0){
        this->removeAllChildrenWithCleanup(true);
        cardForReg = card;
        cardRegisterStep = 1;
        InitLayer(nSelectedTab);
    }
}

void TradeLayer::CardImagePressed(CardInfo* card)
{
    //CCLog("MyCardLayer::Card Pic pressed");
    this->setTouchEnabled(false);
    cardListLayer->setCellTouch(false);
    
    ToTopZPriority(this);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    _cardDetailView = new CardDetailViewLayer(CCSize(size.width,size.height),card, this);
    this->addChild(_cardDetailView,1000);
    this->setTouchEnabled(false);
    
    CCLayer *parent = (CCLayer*)this->getParent();
    CCLayer *grandParent = (CCLayer*)parent->getParent();
    parent->setTouchEnabled(false);
    grandParent->setTouchEnabled(false);
    
}

void TradeLayer::CloseDetailView(){
    
    this->removeChild(_cardDetailView, true);
    this->setTouchEnabled(true);
    cardListLayer->setCellTouch(true);
    
    RestoreZProirity(this);
    
    CCLayer *parent = (CCLayer*)this->getParent();
    CCLayer *grandParent = (CCLayer*)parent->getParent();
    parent->setTouchEnabled(true);
    grandParent->setTouchEnabled(true);
    
}