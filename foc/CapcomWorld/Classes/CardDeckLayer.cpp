//
//  CardDeckLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 11..
//
//

#include "CardDeckLayer.h"
#include "ARequestSender.h"
#include "MainScene.h"
#include "ARequestSender.h"
#include "PopUp.h"
#include "TeamEditLayer.h"

using namespace cocos2d;

CardDeckLayer* CardDeckLayer::instance = NULL;

CardDeckLayer::CardDeckLayer(DeckInfo *di, CCSize layerSize)
{
    this->setContentSize(layerSize);
    
    instance = this;
    _deckInfo = di;
    selected_team = 0;
    uiDepth = 0;
    
    this->setTouchEnabled(true);
    
    /*
    const char* aaa[] = {
        "Team 1",
        "Team 2",
        "Team 3",
        "Team 4"};
    
    for (int i=0;i<4;i++){
        TeamLabel[i] = aaa[i];
    }
    */
    cardList = new CCArray();
    for(int i=0;i<PlayerInfo::getInstance()->myCards->count();i++){
        CardInfo* card = (CardInfo*)PlayerInfo::getInstance()->myCards->objectAtIndex(i);
        
        if (card->bTraingingMaterial)continue;
        cardList->addObject(card);
    }
    
    //initThread();
    
    ARequestSender::getInstance()->requestTeamlist();
    InitLayer(uiDepth, 0);
    
    
    /*
    //this->setDelegate(NULL);
	//this->setDataSource(NULL);
	this->setClipsToBounds(true);

	this->setShowsHorizontalScrollIndicator(false);//true);
	this->setShowsVerticalScrollIndicator(false);//true);
	this->setEditable(true);
	this->setLockHorizontal(true);
	//this->setScrollingToIndexPath(NULL);
	this->setScrollDelegate(this);
	this->setIsScheduled(false);
     */
}

CardDeckLayer::~CardDeckLayer(){

    CCDirector::sharedDirector()->getTouchDispatcher()->removeDelegate(this);
	this->removeAllChildrenWithCleanup(true);
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
//    delete xb;
//    xb = NULL;
#endif
    
}

void CardDeckLayer::InitLayer(int _uiDepth, int _teamID)
{
    this->removeAllChildrenWithCleanup(true);
    
    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");//HelloWorld.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite, 0);
    
    CCSize size = this->getContentSize();
    
    if (_uiDepth == 0){
    
        /*
         // 공격팀 , 방어팀 탭 삭제
        CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_team_tap_atk1.png","ui/card_tab/team/cards_team_tap_atk2.png",this,menu_selector(CardDeckLayer::MenuCallback));
        pSprBtn1->setAnchorPoint( ccp(0,0));
        pSprBtn1->setPosition( accp(10,0));
        pSprBtn1->setTag(0);
        if (selected_team==0)pSprBtn1->selected();
    
        CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/card_tab/team/cards_team_tap_def1.png","ui/card_tab/team/cards_team_tap_def2.png",this,menu_selector(CardDeckLayer::MenuCallback));
        pSprBtn2->setAnchorPoint( ccp(0,0));
        pSprBtn2->setPosition( accp(320,0));
        pSprBtn2->setTag(1);
        if (selected_team==1)pSprBtn2->selected();
    
        CCMenu* pMenu = CCMenu::create(pSprBtn1, pSprBtn2,NULL);
    
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( accp(0, (size.height*SCREEN_ZOOM_RATE)-MAIN_LAYER_TOP_MARGIN-CARD_DECK_TOP_UI_SPACE_2));
        
        pMenu->setTag(99);
        this->addChild(pMenu, 100);
        
        CCLabelTTF* pLabel3 = CCLabelTTF::create(LocalizationManager::getInstance()->get("atkteamtab_btn"), "HelveticaNeue-Bold", 13);
        pLabel3->setColor(COLOR_RED);
        pLabel3->setTag(50);
        registerLabel( this, ccp(0.5,0.5), accp(10+160,(size.height*SCREEN_ZOOM_RATE)-MAIN_LAYER_TOP_MARGIN-CARD_DECK_TOP_UI_SPACE_2+27), pLabel3, 100);
        
        // 160 and 27 is half of image's width and height.
        CCLabelTTF* pLabel4 = CCLabelTTF::create(LocalizationManager::getInstance()->get("defteamtab_btn"), "HelveticaNeue-Bold", 13);
        pLabel4->setColor(COLOR_GRAY);
        pLabel4->setTag(51);
        registerLabel( this, ccp(0.5,0.5), accp(320+160,(size.height*SCREEN_ZOOM_RATE)-MAIN_LAYER_TOP_MARGIN-CARD_DECK_TOP_UI_SPACE_2+27), pLabel4, 100);
        */
        
        InitDeckLayer(selected_team);
        
        std::string text = "팀 설정에서는 퀘스트 진행 중 만나게 되는 \n다양한 라이벌 및 보스들과 싸우게 되는 카드들을 설정합니다.\n또한 다른 이용자들과 서로 강함을 뽐내는 배틀에서도 사용됩니다.\n \n공격력과 체력 그리고 배틀포인트는 \n설정된 카드들의 합으로 구성됩니다.";
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 11);
        //"배틀과 퀘스트를 위한 팀을 구성합니다.",
        pLabel1->setColor(COLOR_WHITE);
        int yy = size.height - accp(CARD_DECK_TOP_UI_SPACE_3) - accp(CARD_DECK_SELL_H)- accp(170);
        
        registerLabel( this, ccp(0.5,0.5), ccp(size.width/2,yy), pLabel1, 100);
        
        
    }
    else if (uiDepth == 1){
        
        MainScene::getInstance()->HideMainMenu();
        DojoLayerCard::getInstance()->HideMenu();
        UserStatLayer::getInstance()->HideMenu();
        
        // init card list
        cardListLayer = new CardListLayer( CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height - accp(120+90-30)), this->cardList, CALL_CARDLIST_FROM_DECK, this,false, _teamID);
        
        cardListLayer->setAnchorPoint(accp(0,0));
        cardListLayer->setPosition(accp(10,90+90)); //y좌표 대충 맞춤.아래의 team edit layer위로만 오도록 맞춤
        this->addChild(cardListLayer,10);
        
        // init team edit layer
        teamEditLayer = new TeamEditLayer(selectedCell->whichTeam, selectedCell->nId, selectedCell->cards, this, 0);
        teamEditLayer->setPosition(ccp(0,0-accp(CARDS_LAYER_BTN_HEIGHT+MAIN_LAYER_BTN_HEIGHT)));
        this->addChild(teamEditLayer,20);
        
    }
    
}

void CardDeckLayer::refreshCardList()//float layer_y, int teamID)
{
    cardListLayer->_tableView->RefreshPage();    
}

void CardDeckLayer::InitDeckLayer(int whichTeam)
{
    CCSize size = this->getContentSize();
    //int content_h = accp((size.height*2)-(TOP_UI_SPACE_1+TOP_UI_SPACE_2+TOP_UI_SPACE_3));
    int content_h = accp((size.height*SCREEN_ZOOM_RATE)-(MAIN_LAYER_TOP_MARGIN+CARD_DECK_TOP_UI_SPACE_2+CARD_DECK_TOP_UI_SPACE_3));
    
    if (whichTeam==0){
        _deckListLayer = new DeckListLayer(0, CCSize(this->getContentSize().width, accp(CARD_DECK_SELL_H * NUM_OF_ATTACK_DECK + CARD_DECK_SELL_SPACE * (NUM_OF_ATTACK_DECK-1))), this);
        
        int offy = this->getContentSize().height - accp(MAIN_LAYER_TOP_MARGIN+CARD_DECK_TOP_UI_SPACE_2+CARD_DECK_TOP_UI_SPACE_3) - _deckListLayer->getContentSize().height;
        
        _deckListLayer->setPosition(ccp(5, offy));
        _deckListLayer->setAnchorPoint(ccp(0,0));
        _deckListLayer->yStart = offy;
        _deckListLayer->contentClippingH = content_h;
        
        //_deckListLayer->setDelegate(this);
        this->addChild(_deckListLayer);
    }
    else if (whichTeam==1){
        //_deckListLayer = new DeckListLayer(1, CCSize(this->getContentSize().width, accp(CARD_DECK_SELL_H * NUM_OF_DEFENCE_DECK + CARD_DECK_SELL_SPACE * (NUM_OF_DEFENCE_DECK-1))),this);

        _deckListLayer = new DeckListLayer(1, CCSize(this->getContentSize().width, accp(CARD_DECK_SELL_H)),this);
        
        int offy = this->getContentSize().height - accp(MAIN_LAYER_TOP_MARGIN+CARD_DECK_TOP_UI_SPACE_2+CARD_DECK_TOP_UI_SPACE_3) - _deckListLayer->getContentSize().height;
        
        //offy -= accp(CARD_DECK_SELL_SPACE);
        
        _deckListLayer->setPosition(ccp(5, offy));
        _deckListLayer->setAnchorPoint(ccp(0,0));
        _deckListLayer->yStart = offy;
        _deckListLayer->contentClippingH = content_h;
        //_deckListLayer->setDelegate(this);
        this->addChild(_deckListLayer);
    }
}

void CardDeckLayer::MenuCallback(CCObject *pSender){
    
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    soundButton1();
    
    if (tag >= 0 && tag <= 1)
    {
        CCMenu *menu = (CCMenu*)this->getChildByTag(99);
        if (menu)
        {
            for (int scan = 0;scan < 2;scan++)
            {
                CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(scan);
                if (!item)
                    continue;
                item->unselected();
            }
        }
    }
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 0:
            {
                item->selected();
                CCLabelTTF* pLabel1 = (CCLabelTTF*)this->getChildByTag(50);
                pLabel1->setColor(COLOR_RED);
                CCLabelTTF* pLabel2 = (CCLabelTTF*)this->getChildByTag(51);
                pLabel2->setColor(COLOR_GRAY);
                pLabel1 = pLabel2 = NULL;
            }
            break;
        case 1:
            {
                item->selected();
                CCLabelTTF* pLabel1 = (CCLabelTTF*)this->getChildByTag(50);
                pLabel1->setColor(COLOR_GRAY);
                CCLabelTTF* pLabel2 = (CCLabelTTF*)this->getChildByTag(51);
                pLabel2->setColor(COLOR_BLUE);
                pLabel1 = pLabel2 = NULL;
            }
            break;
    }
    
    if (tag == selected_team)return;
    
    switch(tag){
        case 0:
        {
            selected_team = 0;
        }
            break;
        case 1:
        {
            selected_team = 1;
        }
            break;
    }
    
    this->removeChild(_deckListLayer, true);
    InitDeckLayer(selected_team);
    
    
}

void CardDeckLayer::ButtonEdit(CCObject *cell, int teamID){
    CCLog("CardDeckLayer->ButtonEdit");
    
    selectedCell = (AttackDeckCell*) cell;
    uiDepth = 1;
    InitLayer(uiDepth, teamID);
/*
    if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 7)
    {
        TeamEditLayer* teamEdit = (TeamEditLayer*)TeamEditLayer::getInstance();
        teamEdit->setTouchEnabled(false);
        CCMenu* pMenu = (CCMenu*)teamEdit->getChildByTag(99);
        pMenu->setEnabled(false);

        tutorialProgress = 8;
        TutorialPopUp *basePopUp = new TutorialPopUp;
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(basePopUp, 9000);
    }
 */
}

/*
void CardDeckLayer::ButtonCopy(CCObject *cell)
{
    CCLog("CardDeckLayer->ButtonCopy");

    selectedCell = (AttackDeckCell*) cell;
//#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    PlayerInfo::getInstance()->xb->DropDownView(TeamLabel, 4, selectedCell->nId, 20);
//#endif
}

void CardDeckLayer::copyTeam(int _to)
{
    if (selectedCell->nId == _to)return;
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    pi->EmptyDeck(0, _to);
    for(int i=0;i<5;i++){
        if (selectedCell->cards[i] != NULL){
            pi->SetCardToDeck(0, _to, i, selectedCell->cards[i]);
        }
    }
    
    int cardSrls[5];
    for(int i=0;i<5;i++){
        CardInfo *card = pi->GetCardInDeck(0, _to, i);
        if (card == NULL)cardSrls[i]=0;
        else cardSrls[i] = card->getSrl();
    }
    
    
    int n = 0;
    for (int i=0;i<5;i++){
        n += cardSrls[i];
    }
    
    if (n > 0){ // check deck empty
        if (ARequestSender::getInstance()->requestEditTeam(0, _to, cardSrls)){
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
            CardDeckLayer::getInstance()->schedule(schedule_selector(CardDeckLayer::refreshLayer),0.5);
#else
            this->removeChild(_deckListLayer, true);
            InitDeckLayer(selected_team);
#endif
        }
    }
}
 */   
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
void CardDeckLayer::refreshLayer()
{
    this->removeChild(_deckListLayer, true);
    InitDeckLayer(selected_team);
    
    CardDeckLayer::getInstance()->unschedule(schedule_selector(CardDeckLayer::refreshLayer));
}
#endif

void CardDeckLayer::ButtonRemove(CCObject *cell)
{
    selectedCell = (AttackDeckCell*) cell;
   
    // check deck is empty
    int n = 0;
    for (int i=0;i<5;i++){
        if (selectedCell->cards[i] != NULL){
            n +=selectedCell->cards[i]->getSrl();
        }
    }
    
    if (n != 0){
    
        PlayerInfo *pi = PlayerInfo::getInstance();
        
        int blankCardSrls[5];
        for(int i=0;i<5;i++){blankCardSrls[i]=0;}
        
        if (ARequestSender::getInstance()->requestEditTeam(selectedCell->whichTeam, selectedCell->nId, blankCardSrls)){
            pi->EmptyDeck(selectedCell->whichTeam, selectedCell->nId);
        }
    }
    
}


void CardDeckLayer::ButtonBack(){
    uiDepth = 0;
    //RestoreZProirity(this);
    
    MainScene::getInstance()->ShowMainMenu();
    DojoLayerCard::getInstance()->ShowMenu();
    UserStatLayer::getInstance()->ShowMenu();
    
    InitLayer(uiDepth,0);
/*
    if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 8)
    {
        AttackDeckCell* cell = AttackDeckCell::getInstance();
        cell->setTouchEnabled(false);
        
        tutorialProgress = 10;
        TutorialPopUp *basePopUp = new TutorialPopUp;
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(basePopUp, 9000);
    }
 */
}

void CardDeckLayer::ButtonA(CardInfo* _card){
    //teamEditLayer->AddCardToDeck(_card);
    
    teamEditLayer->CardSelected(_card);
}

void CardDeckLayer::CardImagePressed(CardInfo* card)
{
    teamEditLayer->CardSelected(card);
}

void CardDeckLayer::CardListBackBtnPressed()
{
    teamEditLayer->buttonCancel();
}

void *CardDeckLayer::threadAction(void *threadid)
{
    ARequestSender::getInstance()->requestTeamlist();
    
    CardDeckLayer::getInstance()->schedule(schedule_selector(CardDeckLayer::threadCallBack));
    CardDeckLayer::getInstance()->unschedule(schedule_selector(CardDeckLayer::threadTimeoutCallback));
    pthread_exit(NULL);
        
}

void CardDeckLayer::initThread()
{
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/card/card_load_01.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/card/card_load_02.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/card/card_load_03.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->autorelease();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    
    regAni(aniFrame, this, ccp(0.5f, 0.5f), ccp(this->getContentSize().width/2, this->getContentSize().height/2), 22, 0);
    
    int t = 0;
    pthread_create(&threads, NULL, CardDeckLayer::threadAction, (void *)t);
    this->schedule(schedule_selector(CardDeckLayer::threadTimeoutCallback),5);
}

void CardDeckLayer::threadCallBack()
{
    //CCLog("threadCallBack !!!!!!!!!!");
    InitLayer(0,0);
    this->removeChildByTag(22, true);
    this->unschedule(schedule_selector(CardDeckLayer::threadCallBack));
}

void CardDeckLayer::threadTimeoutCallback()
{
    CCLog("thread time out");
    this->unschedule(schedule_selector(CardDeckLayer::threadTimeoutCallback));
    //pthread_exit(NULL);
    pthread_kill(threads, 0);
    popupNetworkError("Timeout","", "Sorry, \n please try again.");
}
