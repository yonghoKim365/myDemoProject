//
//  TeamEditLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 16..
//
//

#include "TeamEditLayer.h"
#include "CardInfo.h"
#include "CardDeckLayer.h"
#include "PlayerInfo.h"
#include "ARequestSender.h"
#include "Tutorial.h"

TeamEditLayer* TeamEditLayer::instance = NULL;

TeamEditLayer* TeamEditLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

TeamEditLayer::TeamEditLayer(int _nTeam, int _nId, CardInfo* _teamCards[], TeamEditBtnBackDelegate *_delegate, int _callFrom) // TeamEditLayer::TeamEditLayer(int _nTeam, CCArray* _teamCards[])
{
    instance = this;
    
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        //bRet = true;
        
    } while (0);
    
    nCallFrom = _callFrom;
    
    bEmptyDeckSelected = false;
    bCardSelected = false;
    bInitDone = false;
    nTeam = _nTeam;
    nId = _nId;
    delegate = _delegate;
    
    for (int i=0;i<5;i++){
        originTeam[i] = _teamCards[i];
        cardInDeck[i] = _teamCards[i];
        
        originCardSrl[i] = 0;
        if (cardInDeck[i] != NULL){
            originCardSrl[i] = cardInDeck[i]->getSrl();
        }
    }
    
    cardMaker = new ACardMaker();
    
    InitLayer();
    
    this->setTouchEnabled(true);
    
    bInitDone = true;
    //TestB();
    
}

TeamEditLayer::~TeamEditLayer(){
    cardMaker->autorelease();
    cardMaker = NULL;
}

void TeamEditLayer::InitLayer()
{
    
    if (nTeam==0){ // attack team
        CCSprite* pSpr = CCSprite::create("ui/card_tab/team/cards_team_slot_select_bg_atk.png");
        regSprite(this, ccp(0,0), ccp(0,0), pSpr, 100);
        
    }
    else if (nTeam == 1){
        CCSprite* pSpr = CCSprite::create("ui/card_tab/team/cards_team_slot_select_bg_def.png");
        regSprite(this, ccp(0,0), ccp(0,0), pSpr, 100);
    }
    
    
    int totAp = 0;
    int totDp = 0;
    int totBp = 0;
    for (int i=0;i<5;i++){
        
        
        if (cardInDeck[i] != NULL){

            /*
            const char* path = GetCharImgPath(cardInDeck[i]->getId(), 0);
            CCSprite* pSpr = CCSprite::create(path);
            
            CCSize aa = pSpr->getTexture()->getContentSizeInPixels();
            float cardScale = (float)180 / aa.height;
            pSpr->setScale(cardScale);
            pSpr->setTag(10+i);
            regSprite(this, ccp(0,0), accp(20+i*120,50), pSpr, 100);
            */
            
            totAp += cardInDeck[i]->getAttack();
            totDp += cardInDeck[i]->getDefence();
            totBp += cardInDeck[i]->getBp();
            cardMaker->MakeCardThumb(this, cardInDeck[i], ccp(22+i*121, 130), 170, 100, 10+i);
        }
        else{
            /*
            CCSprite *pSpr = CCSprite::create("ui/card_tab/team/cards_team_slot_select_icon_1.png");
            pSpr->setTag(10+i);
            regSprite(this, ccp(0,0), accp(55+(i*121),110), pSpr, 100);
             */
        }
    }
    
    ///////////
    /*
    std::string text = "공격력 ";
    char buf1[10];
    sprintf(buf1, "%d", totAp);
    char buf2[10];
    sprintf(buf2, "%d", totDp);
    char buf3[10];
    sprintf(buf3, "%d", totBp);
    
    text.append(buf1).append(" 방어력 ").append(buf2).append(" 배틀포인트 ").append(buf3);
    CCLabelTTF* pLabel4 = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pLabel4->setColor(subBtn_color_normal);
    registerLabel( this, ccp(0.5,0), accp(320,85), pLabel4, 100);
    */
    char buf1[10];
    sprintf(buf1, "%d", totAp);
    char buf2[10];
    sprintf(buf2, "%d", totDp);
    char buf3[10];
    sprintf(buf3, "%d", totBp);
    
    CCLabelTTF* pLabel31 = CCLabelTTF::create("공격력", "HelveticaNeue-Bold", 14);
    pLabel31->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(147,100), pLabel31, 100);
    
    CCLabelTTF* pLabel33 = CCLabelTTF::create("체력", "HelveticaNeue-Bold", 14);
    pLabel33->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(304,100), pLabel33, 100);
    
    CCLabelTTF* pLabel35 = CCLabelTTF::create("배틀포인트", "HelveticaNeue-Bold", 14);
    pLabel35->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(506,100), pLabel35, 100);
    
    CCLabelTTF* pLabel32 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 14);
    pLabel32->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(157,100), pLabel32, 100);
    
    CCLabelTTF* pLabel34 = CCLabelTTF::create(buf2, "HelveticaNeue-Bold", 14);
    pLabel34->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(314,100), pLabel34, 100);
    
    CCLabelTTF* pLabel36 = CCLabelTTF::create(buf3, "HelveticaNeue-Bold", 14);
    pLabel36->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(516,100), pLabel36, 100);
    ////////////////////////
    
    std::string path1 = "ui/card_tab/team/cards_team_slot_confirm_a1.png";
    std::string path2 = "ui/card_tab/team/cards_team_slot_confirm_a2.png";
    
    std::string btn_cancel1 = "ui/card_tab/team/cards_team_slot_cancel_a1.png";
    std::string btn_cancel2 = "ui/card_tab/team/cards_team_slot_cancel_a2.png";
    
    if (nTeam == 1){
        path1 = "ui/card_tab/team/cards_team_slot_confirm_b1.png";
        path2 = "ui/card_tab/team/cards_team_slot_confirm_b2.png";
    }
    
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create(path1.c_str(),path2.c_str(),this,menu_selector(TeamEditLayer::MenuCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( ccp(0,0));
    pSprBtn1->setTag(91);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create(btn_cancel1.c_str(),btn_cancel2.c_str(),this,menu_selector(TeamEditLayer::MenuCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp(305,0));
    pSprBtn2->setTag(92);
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1,pSprBtn2, NULL);
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( accp(20, 10));
    pMenu->setTag(99);
    this->addChild(pMenu, 100);
    
    CCLOG("tutorialProgress : %d", tutorialProgress);
    if(tutorialProgress >= TUTORIAL_TEAM_SETTING_PREVIEW_3
       && tutorialProgress < TUTORIAL_TEAM_SETTING_PREVIEW_7)
        pMenu->setEnabled(false);
    
    CCLabelTTF* pLabel5 = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    pLabel5->setColor(COLOR_YELLOW);
    registerLabel( this, ccp(0.5,0), accp(20+148,10+15), pLabel5, 100);
    
    CCLabelTTF* pLabel6 = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    pLabel6->setColor(cocos2d::ccc3(220,0,0));
    registerLabel( this, ccp(0.5,0), accp(325+148,10+15), pLabel6, 100);
    
    //pSprBackBtn = CCSprite::create("ui/card_tab/team/cards_team_slot_back_icon.png");
    //regSprite(this, ccp(0,0), accp(20,238), pSprBackBtn, 100);
    
}

void TeamEditLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    /*
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    CCSprite *pic1 = (CCSprite*)(this->getChildByTag(10));
    
    CCRect *rect = new CCRect(pSprBackBtn->boundingBox());
    if (rect->containsPoint(localPoint)){
        CCLog("back button press");
    }
    
    CCRect *pic1rect = new CCRect(pic1->boundingBox());
    if (pic1rect->containsPoint(localPoint)){
        CCLog("pic 1 button press");
    }
     */
    
}

void TeamEditLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //CCLog(" TeamEditLayer::ccTouchesEnded ");
    
    if (bInitDone==false)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    /*
    CCRect *rect = new CCRect(pSprBackBtn->boundingBox());
    if (rect->containsPoint(localPoint)){
        CCLog("back button press");
        
        buttonConfirm();
        //((CardDeckLayer*)(this->getParent()))->ButtonBack();//->ButtonBack();
        return;
    }
     */
    
    if (bCardSelected){
        
        for(int i=0;i<5;i++){
            CCSprite *pic = (CCSprite*)this->getChildByTag(10+i);
            if (pic != NULL){
                CCRect *pic1rect = new CCRect(pic->boundingBox());
                if (pic1rect->containsPoint(localPoint)){
                    
                    // 선택된 카드와 중복되는 카드가 deck안에 있으면 deck에 있는 카드를 삭제한다. 
                    int v = isDuplicateCardInDeck(selectedCard);
                    if (v >= 0){
                        cardInDeck[v] = NULL;
                        PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, v, NULL);
                        this->removeAllChildrenWithCleanup(false);
                        InitLayer();
                    }

                    cardInDeck[i] = selectedCard;
                    
                    PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, i, selectedCard);
                    
                    // layer refresh
                    this->removeAllChildrenWithCleanup(false);
                    InitLayer();
                    bCardSelected = false;
                    
                    // 장착중 라벨 갱신을 위해 카드 리스트 갱신
                    if (nCallFrom == 0){ // call from team edit
                        float list_y = CardDeckLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                        CardDeckLayer::getInstance()->refreshCardList();
                    }
                    else if (nCallFrom == 1){ // call from battle
                        float list_y = BattleDuelLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                        BattleDuelLayer::getInstance()->refreshCardList();
                    }
                    
                }
            }
            else{
                // 카드 이미지 없이 빈 slot 일 경우에는 10+i 에 해당하는 이미지가 없어 터치 detect가 안된다.
                // 이 경우 화살표 이미지(20+i 태그를 가진 애니 아이콘)로 터치 인식하게 하여 카드 추가하게 한다.
                pic = (CCSprite*)this->getChildByTag(20+i);
                if (pic != NULL){
                    CCRect *pic1rect = new CCRect(pic->boundingBox());
                    if (pic1rect->containsPoint(localPoint))
                    {
                        if(tutorialProgress == TUTORIAL_TEAM_SETTING_PREVIEW_4)
                        {
                            MainScene::getInstance()->removeChildByTag(98765, true);

                            const bool TutorialMode = true;
                            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                            int temp = TUTORIAL_TEAM_SETTING_PREVIEW_5;
                            basePopUp->InitUI(&temp);
                            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                            basePopUp->setPosition(accp(0.0f, 0.0f));
                            basePopUp->setTag(98765);
                            MainScene::getInstance()->addChild(basePopUp, 9000);
                            
                            this->setTouchEnabled(false);
                            
                            CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
                            pMenu->setEnabled(false);
                        }
                        
                        // 선택된 카드와 중복되는 카드가 deck안에 있으면 deck에 있는 카드를 삭제한다. 
                        int v = isDuplicateCardInDeck(selectedCard);
                        if (v >= 0){
                            cardInDeck[v] = NULL;
                            PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, v, NULL);
                            this->removeAllChildrenWithCleanup(false);
                            InitLayer();
                        }
                        
                        cardInDeck[i] = selectedCard;
                        
                        PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, i, selectedCard);
                        
                        this->removeAllChildrenWithCleanup(false);
                        InitLayer();
                        bCardSelected = false;
                                                
                        if (nCallFrom == 0){ // call from team edit
                            float list_y = CardDeckLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                            CardDeckLayer::getInstance()->refreshCardList();//list_y, nId);
                        }
                        else if (nCallFrom == 1){ // call from battle
                            float list_y = BattleDuelLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                            BattleDuelLayer::getInstance()->refreshCardList();//list_y, nId);
                        }
                        
                        
                        
                        if(tutorialProgress == TUTORIAL_TEAM_SETTING_PREVIEW_6
                           && 0 != cardInDeck[0] && 0 != cardInDeck[1] && 0 != cardInDeck[2]
                           && 0 != cardInDeck[3] && 0 != cardInDeck[4])
                        {
                            MainScene::getInstance()->removeChildByTag(98765, true);

                            const bool TutorialMode = true;
                            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                            int temp = TUTORIAL_TEAM_SETTING_PREVIEW_7;
                            basePopUp->InitUI(&temp);
                            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                            basePopUp->setPosition(accp(0.0f, 0.0f));
                            basePopUp->setTag(98765);
                            MainScene::getInstance()->addChild(basePopUp, 9000);
                            
                            this->setTouchEnabled(false);
                        }

                        if(tutorialProgress == TUTORIAL_TEAM_SETTING_PREVIEW_6)
                        {
                            ACardTableView::getInstance()->DrawTutorialIcon();
                        }

                        
                    }
                }
            }
            
            
        }
    }
    else{
        for(int i=0;i<5;i++){
            CCSprite *pic = (CCSprite*)this->getChildByTag(10+i);
            if (pic != NULL){
                CCRect *pic1rect = new CCRect(pic->boundingBox());
                if (pic1rect->containsPoint(localPoint)){
                    
                    cardInDeck[i] = NULL;
                    
                    PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, i, NULL);
                    this->removeAllChildrenWithCleanup(false);
                    InitLayer();
                    
                    if (nCallFrom == 0){ // call from team edit
                        float list_y = CardDeckLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                        CCLog(" list_y :%f", list_y);
                        CardDeckLayer::getInstance()->refreshCardList();//list_y, nId);
                    }
                    else if (nCallFrom == 1){ // call from battle
                        float list_y = BattleDuelLayer::getInstance()->cardListLayer->_tableView->getPositionY();
                        CCLog(" list_y :%f", list_y);
                        BattleDuelLayer::getInstance()->refreshCardList();//list_y, nId);
                    }
                }
            }
        }
        
    }
    /*
    for(int i=0;i<5;i++){
        CCSprite *pSprBtn = (CCSprite*)(this->getChildByTag(20+i));
        if (pSprBtn != NULL){
            CCRect *pic1rect = new CCRect(pSprBtn->boundingBox());
            if (pic1rect->containsPoint(localPoint)){
                selectedDeck = i;
                bEmptyDeckSelected = true;
            }
        }
    }
     */
}



//  deck안에 중복되는 srl값을 가지는 카드가 있는지 확인한다. 
int TeamEditLayer::isDuplicateCardInDeck(CardInfo* _card){
    for(int i=0;i<5;i++){
        if (cardInDeck[i] != NULL && _card != NULL){
            if (cardInDeck[i]->getSrl() == _card->getSrl())return i;
        }
    }
    return -1;
}

void TeamEditLayer::CardSelected(CardInfo *_card)
{
    if (bCardSelected){
        this->removeAllChildrenWithCleanup(false);
        InitLayer();
    }
    bCardSelected = true;
    selectedCard = _card;
    
    CCLog(" TeamEditLayer::CardSelected, cardid:%d, srl:%d",_card->getId(), _card->getSrl());
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/card_tab/team/cards_team_slot_select_icon_1.png", CCRectMake(0,0,accp(116),accp(175)));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/card_tab/team/cards_team_slot_select_icon_2.png", CCRectMake(0,0,accp(116),accp(175)));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/card_tab/team/cards_team_slot_select_icon_3.png", CCRectMake(0,0,accp(116),accp(175)));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/card_tab/team/cards_team_slot_select_icon_4.png", CCRectMake(0,0,accp(116),accp(175)));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->autorelease();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    
    for(int i=0;i<5;i++){
        // 해당 deck에 <추가아이콘버튼>을 넣는다.
        
        regAni(aniFrame, this, ccp(0.5,0.5), accp(76+(i*121),210), 20+i, 100);
    }
}

void TeamEditLayer::AddCardToDeck(CardInfo *_card){
    /*
    if (bEmptyDeckSelected){
        bEmptyDeckSelected = false;
        
        cardInDeck[selectedDeck] = _card;
        
        // remove arrow icon
        this->removeChildByTag(20+selectedDeck, true);
        
        // add card image to deck
        const char* path = GetCharImgPath(_card->getId(), 0);
        CCSprite* pSpr = CCSprite::create(path);
        CCSize aa = pSpr->getTexture()->getContentSizeInPixels();
        float cardScale = (float)180 / aa.height;
        pSpr->setScale(cardScale);
        pSpr->setTag(10+selectedDeck);
        regSprite(this, ccp(0,0), accp(20+selectedDeck*120,50), pSpr, 100);
    }
     */
}

void TeamEditLayer::buttonConfirm()
{
    if (PlayerInfo::getInstance()->getTeamBattlePoint(0,0) > PlayerInfo::getInstance()->getMaxBattlePoint()){
        //cardInDeck[i] = prevCard;
        //PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, i, prevCard);
        
        std::string text1 = "팀의 배틀포인트가 \n유저의 최대 배틀포인트보다\n 크면 배틀을 할 수 없습니다.\n \n나의 최대 배틀포인트 : ";
        char buf1[5];
        sprintf(buf1,"%d", PlayerInfo::getInstance()->getMaxBattlePoint());
        char buf2[5];
        sprintf(buf2,"%d", PlayerInfo::getInstance()->getTeamBattlePoint(0,0));
        text1.append(buf1).append("\n팀의 배틀포인트 : ").append(buf2);
        
        popupOk(text1.c_str());
        return;
    }
    
    int cardSrls[5];
    for(int i=0;i<5;i++)
    {
        if (cardInDeck[i] == NULL)cardSrls[i]=0;
        else cardSrls[i] = cardInDeck[i]->getSrl();
    }
    
    if(NULL != cardInDeck[2])
    {
        // -- 리더 대표이미지 다운로드
        FileManager::sharedFileManager()->downloadLeaderImg(cardInDeck[2]->cardId);
    }
    
    int unchangeCnt = 0;
    for(int i=0;i<5;i++){
        if (originCardSrl[i] == cardSrls[i])unchangeCnt++;
    }
    if (unchangeCnt != 5){
        ARequestSender::getInstance()->requestEditTeam(nTeam, nId, cardSrls);
    }
    delegate->ButtonBack();
    
}

void TeamEditLayer::buttonCancel()
{
    for (int i=0;i<5;i++){
        PlayerInfo::getInstance()->SetCardToDeck(nTeam, nId, i, originTeam[i]);
    }
    delegate->ButtonBack();
}


void TeamEditLayer::MenuCallback(CCObject *pSender){
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if(tutorialProgress < TUTORIAL_TOTAL-1)
    {
        if(tag == 92)
            return;
        else
            removeClickIcon(MainScene::getInstance());
    }
    
    soundButton1();
    
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 91: // confirm
            buttonConfirm();
            
            if(TUTORIAL_TEAM_SETTING_PREVIEW_7 == tutorialProgress)
            {
                MainScene::getInstance()->removeChildByTag(98765, true);

                const bool TutorialMode = true;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                int temp = TUTORIAL_TEAM_SETTING_PREVIEW_8;
                basePopUp->InitUI(&temp);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
                
                AttackDeckCell::getInstance()->setTouchEnabled(false);
            }
            
            break;
        case 92: // cancel
            buttonCancel();
            break;
    }
    
}



















