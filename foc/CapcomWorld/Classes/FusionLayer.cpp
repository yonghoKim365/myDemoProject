//
//  FusionLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 18..
//
//

#include "FusionLayer.h"
#include "PlayerInfo.h"
#include "ARequestSender.h"
#include "UserStatLayer.h"
#include "PopUp.h"

FusionLayer* FusionLayer::instance = NULL;

FusionLayer* FusionLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

FusionLayer::FusionLayer(CCSize layerSize) : fusionInfo(NULL)
{
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
    } while (0);
    
    instance = this;
    bTouchBegan = false;
    //bTouchMove = false;
    
    this->setContentSize(layerSize);
    
    fusionStep = 0;
    fusionCard1 = NULL;
    fusionCard2 = NULL;
    cardMaker = new ACardMaker();
    InitLayer(fusionStep);
    popupCnt = 0;
}


FusionLayer::~FusionLayer()
{
    this->removeAllChildrenWithCleanup(true);
    cardMaker = NULL;
    
}


void FusionLayer::InitLayer(int _step)
{
    this->removeAllChildrenWithCleanup(true);
    
    if (_step == 0){
        int base_y = 146;//73;//
        popupCnt = 0;
    
        CCSprite* pSpr3 = CCSprite::create("ui/card_tab/cards_fusion_slash.png");
        regSprite(this, ccp(0,0), accp(0, 334-base_y), pSpr3, 5);
    
        CCSprite* pSpr4 = CCSprite::create("ui/card_tab/cards_fusion_cardslot1.png");
        pSpr4->setTag(10);
        regSprite(this, ccp(0,0), accp(29,481-base_y), pSpr4, 5);
    
        CCSprite* pSpr5 = CCSprite::create("ui/card_tab/cards_fusion_cardslot2.png");
        pSpr5->setTag(11);
        regSprite(this, ccp(0,0), accp(450,157-base_y), pSpr5, 5);
        //CheckLayerSize(this);
    
        CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/cards_fusion_btn1.png","ui/card_tab/cards_fusion_btn2.png",this,menu_selector(FusionLayer::MenuCallback));
        pSprBtn1->setAnchorPoint( ccp(0,0));
        pSprBtn1->setPosition( accp(0,0));
        pSprBtn1->setTag(0);
    
        CCMenu* pMenu = CCMenu::create(pSprBtn1,NULL);
    
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( accp(232, 400-base_y));
        pMenu->setTag(99);
        this->addChild(pMenu, 3000);
        
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        
        if (fusionCard1 != NULL){
            
            cardMaker->MakeCardThumb(this, fusionCard1, ccp(29,481-base_y+17), 304, 10, 50);
            
            CCSprite* pSprite = CCSprite::create("ui/card_tab/cards_fusion_bg_a2.png");
            pSprite->setAnchorPoint(accp(0,0));
            pSprite->setPosition( accp(0,350-base_y) );
            this->addChild(pSprite, 0);
        }
        else{
            CCSprite* pSprite = CCSprite::create("ui/card_tab/cards_fusion_bg_a1.png");
            pSprite->setAnchorPoint(accp(0,0));
            pSprite->setPosition( accp(0,350-base_y) );
            this->addChild(pSprite, 0);
        }
        if (fusionCard2 != NULL){
            
            cardMaker->MakeCardThumb(this, fusionCard2, ccp(450,157-base_y+16), 242, 10, 51);
            
            CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_fusion_bg_b2.png");
            pSpr2->setAnchorPoint(ccp(0,0));
            pSpr2->setPosition( accp(0,-64) );
            this->addChild(pSpr2, 0);
        }
        else{
            CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_fusion_bg_b1.png");
            pSpr2->setAnchorPoint(ccp(0,0));
            pSpr2->setPosition( accp(0,-64) );
            this->addChild(pSpr2, 0);
        }
    }
    else if (_step == 1){
        
        // init card list
        this->setTouchEnabled(false);
        PlayerInfo *pi = PlayerInfo::getInstance();
        
        /*
        CCLog("card list------");
        for(int i=0;i<pi->myCards->count();i++){
            CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);
            CCLog("card->id:%d, srl:%d", card->getId(), card->getSrl());
        }
        */
        
        
        CCArray *cardlist;
        if (fusionCard1 == NULL && fusionCard2 == NULL){
            
            if (tutorialProgress == 5){
                cardlist = new CCArray();
                for(int i=0;i<pi->myCards->count();i++){
                    CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);
                    if (card->getId() == 30011){
                        cardlist->addObject(card);
                    }
                }
            }
            else{
                //cardlist = pi->myCards;
                
                cardlist = new CCArray();
                
                // 동종의 카드가 2장이상 있는 카드만 보여준다.
                // fusion level이 4인 카드도 스킵
                
                for(int i=0;i<pi->myCards->count();i++){
                    CardInfo* card1 = (CardInfo*)pi->myCards->objectAtIndex(i);
                    
                    if (card1->getFusionCount() == 0)continue;
                    if (card1->GetFusionLevel() == 4)continue;
                    
                    //int myCardID = card1->getId()/10;
                    int sameKindCnt = 0;
                    for(int j=0;j<pi->myCards->count();j++){
                        CardInfo* card2 = (CardInfo*)pi->myCards->objectAtIndex(j);
                        
                        if (card1->getId()/10 == card2->getId()/10 && card1->getSrl() != card2->getSrl()){
                            sameKindCnt++;
                        }
                    }
                    if (sameKindCnt>0){
                        cardlist->addObject(card1);
                    }
                }
                
                if (cardlist->count()==0){
                    popupOk("합성 가능한 카드가 없습니다");
                }
            }
        }
        else{
            
            if (selectedFusionDeck == 0 && fusionCard2 != NULL){
                selectedCard = fusionCard2;
            }
            else if (selectedFusionDeck == 1 && fusionCard1 != NULL){
                selectedCard = fusionCard1;
            }
            
            //CCLog("selectedCard->id:%d getSrl():%d", selectedCard->getId(), selectedCard->getSrl());
            
            cardlist = new CCArray();
            for(int i=0;i<pi->myCards->count();i++){
                CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);
                
                // 같은 종류의 카드만 넣되 내 자신은 제외한 리스트를 보여준다.
                if (card->getId()/10 != selectedCard->getId()/10)continue;
                if (selectedCard->getSrl() == card->getSrl())continue;
                if (card->GetFusionLevel()==4)continue;
                
                if (selectedFusionDeck == 0){
                    if (selectedCard->GetFusionLevel() > card->GetFusionLevel())continue;
                }
                else if (selectedFusionDeck == 1){
                    if (selectedCard->GetFusionLevel() < card->GetFusionLevel())continue;
                }
                
                cardlist->addObject(card);
                
                /*
                if (card->getId()/10 == selectedCard->getId()/10 && selectedCard->getSrl() != card->getSrl() && selectedCard->GetFusionLevel() >= card->GetFusionLevel()){
                    CCLog("card->id:%d, srl:%d", card->getId(), card->getSrl());
                    cardlist->addObject(card);
                }
                 */
            }
        }
        cardListLayer = new CardListLayer( CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height), cardlist, CALL_CARDLIST_FROM_FUSION, this, true, -1);
        cardListLayer->setAnchorPoint(ccp(0,0));
        cardListLayer->setPosition(accp(10,0));
        this->addChild(cardListLayer,10);
    }
    else if (_step == 2){
        
        ToTopZPriority(this);
        
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        //cardDetailViewLayer = new CardDetailViewLayer( CCRectMake(0,0,size.width, size.height,  fusionCard1);
        cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width,size.height),fusionCard1, this, DIRECTION_NEWCARD);
        this->addChild(cardDetailViewLayer,10);
        
        CCSprite* pSpr = CCSprite::create("ui/card_tab/cards_text_bg.png");
        pSpr->setAnchorPoint(ccp(0,0));
        pSpr->setPosition(accp(62,160));
        this->addChild(pSpr, 20);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("합성으로 인해 카드가 강해졌습니다", "HelveticaNeue-Bold", 12);
        pLabel1->setColor(subBtn_color_normal);
        
        CCLabelTTF* pLabel2 = CCLabelTTF::create("+공격력 ", "HelveticaNeue-Bold", 12);
        pLabel2->setColor(COLOR_ORANGE);
    
        char buf1[10];
        sprintf(buf1, "%d", fusionCard1->getAttack() - originAttack);
        
        CCLabelTTF* pLabel4 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
        pLabel4->setColor(COLOR_WHITE);

        CCLabelTTF* pLabel3 = CCLabelTTF::create("+체력 ", "HelveticaNeue-Bold", 12);
        pLabel3->setColor(COLOR_ORANGE);
        
        char buf2[10];
        sprintf(buf2, "%d", fusionCard1->getDefence() - originDefense);

        CCLabelTTF* pLabel5 = CCLabelTTF::create(buf2, "HelveticaNeue-Bold", 12);
        pLabel5->setColor(COLOR_WHITE);

        registerLabel(this, ccp(0,0), accp(83,260), pLabel1, 30);
        
        registerLabel(this, ccp(0,0), accp(83,210), pLabel2, 30);
        registerLabel(this, ccp(0,0), accp(170,210), pLabel4, 30);
        
        registerLabel(this, ccp(0,0), accp(83,180), pLabel3, 30);
        registerLabel(this, ccp(0,0), accp(170,180), pLabel5, 30);
        
        //this->setTouchEnabled(true);
    }
    bTouchBegan = false;
    
    
}


void FusionLayer::MenuCallback(cocos2d::CCObject *pSender)
{
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    switch(tag){
        case 0:
            if (fusionCard1 == NULL || fusionCard2 == NULL)
                break;
            
            soundButton2();
        
            // coin check
            //int needed = (int)ceil(((double)fusionCard1->getPrice() * 0.6 + (double)fusionCard2->getPrice() * 0.6));
            int needed = fusionCard2->getPrice(); // 합성비용 변경. 2013.2.16.김용호
            if (PlayerInfo::getInstance()->myCoin < needed)
            {
                Purchase2Popup *popup = new Purchase2Popup;
                popup->InitUI((void*)LocalizationManager::getInstance()->get("fusion_popup"));
                this->addChild(popup, 5000);
                return;
            }
            else{
                if (popupCnt == 0){
                    popupCnt++;
                    FusionPopup *popup = new FusionPopup;
                    
                    std::string text = "합성에는 xx코인이 필요합니다. \n 카드를 합성하시겠습니까?\n \n합성 재료로 사용된 카드는\n합성후 사라집니다";
                    
                    //훈련에는 xx골드가 필요합니다. \n 카드를 훈련시키시겠습니까?"
                    popup->InitUI((void*)text.c_str(),needed);//(void*)LocalizationManager::getInstance()->get("fusion_popup"));
                    this->addChild(popup, 5000);
                    
                    this->setTouchEnabled(false);
                }
            }
            
            break;
    }
}


void FusionLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchBeganLocation = location;
    
    bTouchBegan = true;
}

void FusionLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    if (bTouchBegan==false)return;
    
    bTouchBegan = false;
    
    if (fusionStep !=0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (fabs(location.x - touchBeganLocation.x) < 30 && fabs(location.y - touchBeganLocation.y) < 30){
        
    
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    
    //if(false == bTouchMove)
    //{
        for(int i=0;i<2;i++){
            CCSprite *pic = (CCSprite*)this->getChildByTag(10+i);
            if (pic != NULL){
                CCRect *pic1rect = new CCRect(pic->boundingBox());
                if (pic1rect->containsPoint(localPoint)){
                    //this->removeChild(pic, true);
                    CCLog("pic button press,%d",i);
                    
                    selectedFusionDeck = i;
                    if (i==0)fusionCard1 = NULL;
                    else if (i==1)fusionCard2 = NULL;
                    
                    
                    if (fusionCard1 == NULL && fusionCard2 == NULL){
                        CCArray *cardlist = new CCArray();
                        
                        // 동종의 카드가 2장이상 있는 카드만 보여준다.
                        // fusion level이 4인 카드도 스킵
                        
                        for(int i=0;i<PlayerInfo::getInstance()->myCards->count();i++){
                            CardInfo* card1 = (CardInfo*)PlayerInfo::getInstance()->myCards->objectAtIndex(i);
                            
                            if (card1->getFusionCount() == 0)continue;
                            if (card1->bTraingingMaterial)continue;
                            
                            //int myCardID = card1->getId()/10;
                            int sameKindCnt = 0;
                            for(int j=0;j<PlayerInfo::getInstance()->myCards->count();j++){
                                CardInfo* card2 = (CardInfo*)PlayerInfo::getInstance()->myCards->objectAtIndex(j);
                                if (card1->getId()/10 == card2->getId()/10 && card1->getSrl() != card2->getSrl()){
                                    sameKindCnt++;
                                }
                            }
                            if (sameKindCnt>0){
                                cardlist->addObject(card1);
                            }
                        }
                        
                        if (cardlist->count()==0){
                            popupOk("합성 가능한 카드가 없습니다");
                        }
                        else{
                            fusionStep = 1;
                            InitLayer(fusionStep);
                        }
                    }
                    else{
                        fusionStep = 1;
                        InitLayer(fusionStep);
                    }
                }
            }
        }
    //}
    }
    
    //bTouchMove = false;
}

void FusionLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //bTouchMove = true;
}


void FusionLayer::ButtonA(CardInfo* card)
{
    
//    if (card->getFusionCount()==0){
//        const char* text = (const char*)LocalizationManager::getInstance()->get("fusion_max_popup");
//        popupOkCancel(card, text, this);
//    }
    if (selectedFusionDeck == 1 && PlayerInfo::getInstance()->isBelongInTeam(card)){
        const char* text = "팀에 설정되어 있는 카드 입니다.\n 합성후에는 팀에서 카드가 삭제되므로 \n 팀 설정을 다시 해주시기 바랍니다.\n 합성 하시겠습니까?";
        popupOkCancel(card, text, this);
    }
    else{
        AddCardToFusionDeck(card, selectedFusionDeck);
        
        this->removeChild(cardListLayer, true);
        
        this->setTouchEnabled(true);
    }
        
}

void FusionLayer::CardListBackBtnPressed()
{
    this->removeChild(cardListLayer, true);
    this->setTouchEnabled(true);
    fusionStep = 0;
    InitLayer(fusionStep);
}

void FusionLayer::ButtonOK(CardInfo* card){
    
    MainScene::getInstance()->removePopup();
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
    
    AddCardToFusionDeck(card, selectedFusionDeck);
    
}

void FusionLayer::ButtonCancel()
{
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
}

void FusionLayer::AddCardToFusionDeck(CardInfo *card, int deck)
{
    //int base_y = 146;//73;//
    if (deck==0){
        fusionCard1 = card;
        originAttack = fusionCard1->getAttack();
        originDefense = fusionCard1->getDefence();
    }
    else if (deck==1){
        fusionCard2 = card;
    }
    selectedCard = card;
    fusionStep = 0;
    InitLayer(fusionStep);
    
}

void FusionLayer::setFusionData(ResponseFusionInfo* _data)
{
    fusionInfo = _data;
}

void FusionLayer::DoCardFusion(CardInfo *card1, CardInfo *card2)
{
    if(fusionInfo)
    {
        if (atoi(fusionInfo->res)==0)
        {
            /*
            if (tutorialProgress < TUTORIAL_DONE && tutorialProgress == 5)
            {
                FusionLayer* flayer = FusionLayer::getInstance();
                flayer->setTouchEnabled(false);
                CCMenu* pMenu3 = (CCMenu*)flayer->getChildByTag(99);
                pMenu3->setEnabled(false);
                
                tutorialProgress = 6;
                
                PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
                
                TutorialPopUp *basePopUp = new TutorialPopUp;
                basePopUp->InitUI(&tutorialProgress);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                MainScene::getInstance()->addChild(basePopUp, 9000);
            }
             */
            CardInfo *dicInfo = CardDictionary::sharedCardDictionary()->getInfo(fusionInfo->card_id);
            if (dicInfo)
            {
                fusionCard1->setFusionCount(dicInfo->getFusionCount());
                fusionCard1->setBp(dicInfo->getBp());
            }
            
            fusionCard1->setSrl(fusionInfo->card_srl);
            fusionCard1->setId(fusionInfo->card_id);
            fusionCard1->setLevel(fusionInfo->card_level);
            fusionCard1->setExp(fusionInfo->card_exp);
            fusionCard1->setAttack(fusionInfo->attack);
            fusionCard1->setDefence(fusionInfo->defense);
            //fusionCard1->updateRare();
            
            PlayerInfo::getInstance()->myCards->removeObject(fusionCard2);
            PlayerInfo::getInstance()->addCoin(-fusionInfo->cost);
            UserStatLayer::getInstance()->refreshCoin();
                    
            fusionStep = 2;
            InitLayer(fusionStep);
            
        }
        else{
            popupNetworkError(fusionInfo->res,fusionInfo->msg, NULL);
            fusionStep = 0;
            InitLayer(fusionStep);
            
        }
    }
    else
        popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
}

void FusionLayer::ArrowAction()
{
    CCSprite* arrow1= CCSprite::create("ui/card_tab/cards_fusion_eff.png");
    arrow1->setAnchorPoint(accp(0.0f, 0.0f));
    arrow1->setPosition( accp(30.0f, 500.0f) );
    arrow1->setTag(779);
    MainScene::getInstance()->addChild(arrow1, 11050);
    
    //CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.66f);
    CCFiniteTimeAction* move1 = CCMoveTo::actionWithDuration(0.4f, accp(258.0f, 400.0f));
    CCFiniteTimeAction* scale1 = CCScaleTo::actionWithDuration(0.4f, 0.5f);
    arrow1->runAction(CCSpawn::actions(move1, scale1, NULL));
    
    CCSprite* arrow2= CCSprite::create("ui/card_tab/cards_fusion_eff.png");
    arrow2->setAnchorPoint(accp(0.0f, 0.0f));
    arrow2->setTag(780);
    arrow2->setPosition( accp(360.0f, 174.0f) );
    MainScene::getInstance()->addChild(arrow2, 11050);
    
    //CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.66f);
    CCFiniteTimeAction* move2 = CCMoveTo::actionWithDuration(0.4f, accp(258.0f, 400.0f));
    CCFiniteTimeAction* scale2 = CCScaleTo::actionWithDuration(0.4f, 0.5f);
    arrow2->runAction(CCSpawn::actions(move2, scale2, NULL));
}

void FusionLayer::removeArrow()
{
    MainScene::getInstance()->removeChildByTag(779, true);
    MainScene::getInstance()->removeChildByTag(780, true);
}

void FusionLayer::CenterButtonAction()
{
    CCSprite* center = CCSprite::create("ui/card_tab/cards_fusion_btn_eff.png");
    center->setScale(1.0f);
    center->setAnchorPoint(ccp(0.5f,0.5f));
    center->setOpacity(0);
    center->setTag(778);
    center->setPosition( accp(1.0f + (638.0f/2.0f), 148.0f + (645.0f/2.0f))); // -- 638, 645 image size
    MainScene::getInstance()->addChild(center, 11050);
    
    CCFadeIn* in3 = CCFadeIn::actionWithDuration(0.3f);
    CCFiniteTimeAction* scale = CCScaleTo::actionWithDuration(1.5f, 4.0f);

    center->runAction(CCSequence::actions(in3, scale, NULL));
}

void FusionLayer::DoCardFusionAction()
{
    
    int base_y = 146;
    
    CCSprite* white1= CCSprite::create("ui/card_tab/cards__eff_white.png");
    white1->setAnchorPoint(accp(0,0));
    white1->setScale(0.33f);
    white1->setTag(775);
    white1->setOpacity(0);
    white1->setPosition( accp(29,481-base_y+17) );
    this->addChild(white1, 11050);
    
    CCFadeIn* in1 = CCFadeIn::actionWithDuration(0.66f);
    CCFadeOut* out1 = CCFadeOut::actionWithDuration(0.66f);
    white1->runAction(CCSequence::actions(in1, out1, NULL));
    
    CCSprite* white2= CCSprite::create("ui/card_tab/cards__eff_white.png");
    white2->setAnchorPoint(accp(0,0));
    white2->setScale(0.264f);
    white2->setOpacity(0);
    white1->setTag(776);
    white2->setPosition( accp(450,157-base_y+16) );
    this->addChild(white2, 11050);
    
    CCFadeIn* in2 = CCFadeIn::actionWithDuration(0.66f);
    CCFadeOut* out2 = CCFadeOut::actionWithDuration(0.66f);
    white2->runAction(CCSequence::actions(in2, out2, NULL));

    CCDelayTime *delay10 = CCDelayTime::actionWithDuration(0.66f);
    CCDelayTime *delay11 = CCDelayTime::actionWithDuration(0.5f);
    CCCallFunc* callBack10 = CCCallFunc::actionWithTarget(this, callfunc_selector(FusionLayer::ArrowAction));
    CCCallFunc* callBack11 = CCCallFunc::actionWithTarget(this, callfunc_selector(FusionLayer::removeArrow));
    CCCallFunc* callBack12 = CCCallFunc::actionWithTarget(this, callfunc_selector(FusionLayer::CenterButtonAction));
    this->runAction(CCSequence::actions(delay10, callBack10, delay11, callBack11, callBack12, NULL));

    
    CCSize size = CCDirector::sharedDirector()->getWinSizeInPixels();
    
    CCSprite* card= CCSprite::create("ui/card_tab/card_spin01.png");
    card->setAnchorPoint(ccp(0.5f,0.5f));
    card->setScale(0.0f);
    card->setTag(777);
    card->setPosition( accp(size.width/2, size.height/2) );
    MainScene::getInstance()->addChild(card, 11150);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.1f, 0.5f, 0.5f);
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 0.5f);
    
    CCFiniteTimeAction* actionScale3 = CCScaleTo::actionWithDuration(0.1f, 1.0f, 1.0f);
    CCFiniteTimeAction* actionScale4 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 1.0f);
    
    CCFiniteTimeAction* actionScale5 = CCScaleTo::actionWithDuration(0.1f, 1.5f, 1.5f);
    CCFiniteTimeAction* actionScale6 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 1.5f);
    
    CCFiniteTimeAction* actionScale7 = CCScaleTo::actionWithDuration(0.1f, 2.0f, 2.0f);
    CCFiniteTimeAction* actionScale8 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 2.0f);
    
    CCFiniteTimeAction* actionScale9 = CCScaleTo::actionWithDuration(0.1f, 2.5f, 2.5f);
    CCFiniteTimeAction* actionScale10 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 2.5f);
    
    CCFiniteTimeAction* actionScale11 = CCScaleTo::actionWithDuration(0.1f, 3.0f, 3.0f);
    CCFiniteTimeAction* actionScale12 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 3.0f);
    
    CCFiniteTimeAction* actionScale13 = CCScaleTo::actionWithDuration(0.1f, 3.5f, 3.5f);
    CCFiniteTimeAction* actionScale14 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 3.5f);
    
    CCFiniteTimeAction* actionScale15 = CCScaleTo::actionWithDuration(0.1f, 4.0f, 4.0f);
    CCFiniteTimeAction* actionScale16 = CCScaleTo::actionWithDuration(0.1f, 0.1f, 4.0f);
    
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.66f);
    card->runAction(CCSequence::actions(delay, delay, actionScale1, actionScale2, actionScale3, actionScale4, actionScale5, actionScale6, actionScale7, actionScale8,
                                        actionScale9, actionScale10, actionScale11, actionScale12, actionScale13, actionScale14, actionScale15, actionScale16, NULL));
    
    
    CCSprite* whiteBG = CCSprite::create("ui/shop/bg_white.png");
    whiteBG->setAnchorPoint(ccp(0.0f, 0.0f));
    whiteBG->setPosition((accp(0.f, 0.0f)));
    whiteBG->setOpacity(0);
    whiteBG->setTag(804);
    MainScene::getInstance()->addChild(whiteBG, 12060);
        
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.5f);
    CCCallFunc* callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(FusionLayer::requestFusion));
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.5f);
    whiteBG->runAction(CCSequence::actions(delay, delay, delay, fadein, callBack4, fadeout, NULL));

}

void FusionLayer::requestFusion()
{
    this->removeChildByTag(775, true);
    this->removeChildByTag(776, true);
    MainScene::getInstance()->removeChildByTag(777, true);
    MainScene::getInstance()->removeChildByTag(778, true);
    MainScene::getInstance()->removeChildByTag(804, true);
    
    //this->setTouchEnabled(true);
    
    DoCardFusion(fusionCard1, fusionCard2);
}

void FusionLayer::CloseDetailView()
{
    //if (tutorialProgress >= TUTORIAL_TOTAL-1)
    //{
        MainScene::getInstance()->setEnableMainMenu(true);
        UserStatLayer::getInstance()->setEnableMenu(true);
        DojoLayerCard::getInstance()->setEnableMainMenu(true);
        
        CCMenu* Menu = (CCMenu*)this->getChildByTag(99);
        
        if(Menu)
            Menu->setEnabled(true);
        
        this->setTouchEnabled(true);
    //}
    
    this->setTouchEnabled(true);
    this->stopAllActions();
    this->removeChild(cardDetailViewLayer, true);
    
    fusionStep = 0;
    fusionCard1 = NULL;
    fusionCard2 = NULL;
    InitLayer(fusionStep);
    
    RestoreZProirity(this);
}
