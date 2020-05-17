//
//  AttackDeckCell.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 15..
//
//

#include "AttackDeckCell.h"
#include "DeckListLayer.h"
#include "Tutorial.h"

AttackDeckCell* AttackDeckCell::instance = NULL;

AttackDeckCell* AttackDeckCell::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}


//AttackDeckCell::AttackDeckCell(CCArray *cards, int _team, int _nId, const char *label){
AttackDeckCell::AttackDeckCell(CardInfo* _cards[], int _team, int _nId, const char *label){
    
    instance = this;
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    for(int i=0;i<5;i++){
        cards[i] = _cards[i];
    }

    _label = label;
    whichTeam = _team;
    nId = _nId;
    
    cardMaker = new ACardMaker();
    InitLayer();
}


AttackDeckCell::~AttackDeckCell(){
    this->removeAllChildrenWithCleanup(true);
}

void AttackDeckCell::InitLayer()
{

    CCSprite* pSprite;
    if (whichTeam == 0){
        pSprite = CCSprite::create("ui/card_tab/team/cards_team_bg_atk.png");
    }
    else if (whichTeam == 1){
        pSprite = CCSprite::create("ui/card_tab/team/cards_team_bg_def.png");
    }
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite);

    
    int totAp = 0;
    int totDp = 0;
    int totBp = 0;
    numOfCardsInDeck = 0;
    CCLog("AttackDeckCell::InitLayer, team:%d id:%d",whichTeam,nId);
    for(int i=0;i<5;i++){

        CardInfo *card = cards[i];
        
        if (card == NULL)continue;
        
        numOfCardsInDeck++;
        
        CCLog(" card[%d], id:%d name:%s, srl:%d",i, card->getId(), card->getName(), card->getSrl());
        
        totAp += card->getAttack();
        totDp += card->getDefence();
        totBp += card->getBp();
        
        cardMaker->MakeCardThumb(this, card,ccp(12 + (121*i),95), 170, 0, 10+i);
    }
    
    this->setContentSize(CCSize(this->getContentSize().width, accp(pSprite->getTexture()->getContentSizeInPixels().height) ));
    
//    int a = this->getContentSize().height;
//    int b = this->getContentSize().width;
    
    /*
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_team_btn_a.png","ui/card_tab/team/cards_team_btn_a.png",this,menu_selector(AttackDeckCell::MenuCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp(0,0));
    pSprBtn1->setTag(0);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/card_tab/team/cards_team_btn_b.png","ui/card_tab/team/cards_team_btn_b.png",this,menu_selector(AttackDeckCell::MenuCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp(207,0));
    pSprBtn2->setTag(1);
    
    CCMenuItemImage *pSprBtn3 = CCMenuItemImage::create("ui/card_tab/team/cards_team_btn_c.png","ui/card_tab/team/cards_team_btn_c.png",this,menu_selector(AttackDeckCell::MenuCallback));
    pSprBtn3->setAnchorPoint( ccp(0,0));
    pSprBtn3->setPosition( accp(413,0));
    pSprBtn3->setTag(2);
     */
    
    CCSprite *pSprBtn1 = CCSprite::create("ui/card_tab/team/cards_team_btn_a.png");
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp(0,4));
    pSprBtn1->setTag(0);
    pSprBtn1->setScaleX(1.49f);
    this->addChild(pSprBtn1,10);
    /*
    CCSprite *pSprBtn2 = CCSprite::create("ui/card_tab/team/cards_team_btn_b.png");
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp(207,4));
    pSprBtn2->setTag(1);
    this->addChild(pSprBtn2,10);
    */
    CCSprite *pSprBtn3 = CCSprite ::create("ui/card_tab/team/cards_team_btn_c.png");
    //pSprBtn3->setAnchorPoint( ccp(0,0));
    //pSprBtn3->setPosition( accp(413,4));
    pSprBtn3->setAnchorPoint( ccp(1,0));//0,0));
    pSprBtn3->setPosition( accp(619,4));//413,4));
    pSprBtn3->setScaleX(1.49f);
    pSprBtn3->setTag(2);
    this->addChild(pSprBtn3,10);
    
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("팀 설정", "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_GRAY);
    registerLabel( this, ccp(0.5,0), accp(103+51,10), pLabel1, 100);
    /*
    CCLabelTTF* pLabel2 = CCLabelTTF::create("팀 복사", "HelveticaNeue-Bold", 12);
    if (whichTeam == 0){
        if (numOfCardsInDeck > 0){
            pLabel2->setColor(COLOR_GRAY);
        }
        else{
            pLabel2->setColor(COLOR_DARK_GRAY);
        }
    }
    else if (whichTeam == 1){
        pLabel2->setColor(COLOR_DARK_GRAY);
    }
    registerLabel( this, ccp(0.5,0), accp(309,10), pLabel2, 100);
    */
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("덱 비우기", "HelveticaNeue-Bold", 12);
    if (numOfCardsInDeck > 0){
        pLabel3->setColor(COLOR_GRAY);
    }
    else{
        pLabel3->setColor(COLOR_DARK_GRAY);
    }
    registerLabel( this, ccp(0.5,0), accp(516-51,10), pLabel3, 100);
    
    
    char buf1[10];
    sprintf(buf1, "%d", totAp);
    char buf2[10];
    sprintf(buf2, "%d", totDp);
    char buf3[10];
    sprintf(buf3, "%d", totBp);
    
    CCLabelTTF* pLabel31 = CCLabelTTF::create("공격력", "HelveticaNeue-Bold", 12);
    pLabel31->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(137,69), pLabel31, 100);
    
    CCLabelTTF* pLabel33 = CCLabelTTF::create("체력", "HelveticaNeue-Bold", 12);
    pLabel33->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(294,69), pLabel33, 100);
    
    CCLabelTTF* pLabel35 = CCLabelTTF::create("배틀포인트", "HelveticaNeue-Bold", 12);
    pLabel35->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(1,0.5), accp(496,69), pLabel35, 100);
    
    CCLabelTTF* pLabel32 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
    pLabel32->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(147,69), pLabel32, 100);
    
    CCLabelTTF* pLabel34 = CCLabelTTF::create(buf2, "HelveticaNeue-Bold", 12);
    pLabel34->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(304,69), pLabel34, 100);
    
    CCLabelTTF* pLabel36 = CCLabelTTF::create(buf3, "HelveticaNeue-Bold", 12);
    pLabel36->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5), accp(506,69), pLabel36, 100);
    
    
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
    if (numOfCardsInDeck > 0){
        pLabel4->setColor(COLOR_WHITE);
    }
    else{
        pLabel4->setColor(COLOR_DARK_GRAY);
    }
    registerLabel( this, ccp(0.5,0), accp(320,56), pLabel4, 100);
    */
    
    
    std::string team_name = "팀 ";
    //char buf[5];
    //sprintf(buf,"%d", nId+1);
    //team_name.append(buf);
    CCLabelTTF* pLabel7 = CCLabelTTF::create(team_name.c_str(), "HelveticaNeue-Bold", 12);
    pLabel7->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0.5,0), accp(56,284), pLabel7, 100);
    
    CCLabelTTF* pLabel8 = CCLabelTTF::create(_label.c_str(), "Thonburi", 13);
    pLabel8->setColor(subBtn_color_normal);
    registerLabel( this, ccp(0,0), accp(130,286), pLabel8, 100);
    

    
    //return bRet;
}



//bool bCellTouchBegan = false;

void AttackDeckCell::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
}


void AttackDeckCell::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    if (DeckListLayer::getInstance()->moving == true)
        return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    //if (location.y < touchStartPos.y || location.y > touchEndPos.y)return;
    if (location.y < touchStartPos.y || location.y > touchEndPos.y){
        CCLog("out of touch range in AttackDeckCell");
        return;
    }
    
    if(tutorialProgress == TUTORIAL_QUEST_1)
        return;
    
    if(tutorialProgress == TUTORIAL_TEAM_SETTING_PREVIEW_2)
    {
        if(GetSpriteTouchCheckByTag(this, 0, localPoint) && 0 == nId) // team 1
        {
            MainScene::getInstance()->removeChildByTag(98765, true);
            
            const bool TutorialMode = true;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            int temp = TUTORIAL_TEAM_SETTING_PREVIEW_3;
            basePopUp->InitUI(&temp);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }
        else
            return;
    }

    if (GetSpriteTouchCheckByTag(this, 0, localPoint)){ // EDIT
        if (delegate != NULL)
        {
            soundButton1();
            delegate->ButtonEdit(this, nId);
        }
    }
    else if (GetSpriteTouchCheckByTag(this, 1, localPoint)){ // COPY
        if (whichTeam == 0){
            if (numOfCardsInDeck > 0){
                if (delegate != NULL){
                    soundButton1();
                    delegate->ButtonCopy(this);
                }
            }
        }
    }
    else if (GetSpriteTouchCheckByTag(this, 2, localPoint)){ // REMOVE
        if (numOfCardsInDeck > 0){
            if (delegate != NULL){
                soundButton1();
                delegate->ButtonRemove(this);
                for(int i=0;i<5;i++){
                    cards[i] = NULL;
                }
                this->removeAllChildrenWithCleanup(false);
                InitLayer();        
            }
        }
    }
}

void AttackDeckCell::SetTouchArea(cocos2d::CCPoint *cFrom, cocos2d::CCPoint *cEnd){
    touchStartPos.y = cFrom->y;
    touchEndPos.y = cEnd->y;
}


void AttackDeckCell::MenuCallback(CCObject *pSender){
    /*
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    switch(tag){
        case 0:
            delegate->ButtonEdit(this);
            // edit
            break;
        case 1:
            // copy
            break;
        case 2:
            // remove
            break;
        case 3:
            break;
        case 4:
            break;
    }
     */
}



