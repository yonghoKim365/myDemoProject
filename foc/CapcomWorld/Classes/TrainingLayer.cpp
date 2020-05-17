//
//  TrainingLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 19..
//
//

#include "TrainingLayer.h"
#include "PlayerInfo.h"
#include "ARequestSender.h"
#include "UserStatLayer.h"
#include "CardDictionary.h"
#include "PopUp.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

TrainingLayer* TrainingLayer::instance = NULL;
TrainingLayer* TrainingLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

TrainingLayer::TrainingLayer(CCSize layerSize) : trainingInfo(NULL), topCha(NULL), bottomCha(NULL)
{
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
    } while (0);
    
    instance = this;
    
    this->setContentSize(layerSize);
    
    bTouchBegan = false;
    fusionStep = 0;
    fusionCard1 = NULL;
    fusionCard2 = NULL;
    cardMaker = new ACardMaker();
    //bTouchMove = false;
    
    for(int i=0; i<5; ++i)
    {
        effectHit[i] = NULL;
    }
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0,0,128,128));
    
    aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);

    InitLayer(fusionStep);
}


TrainingLayer::~TrainingLayer()
{
    this->removeAllChildrenWithCleanup(true);
    cardMaker = NULL;
}

float QuestProgressRatio;
float increaseQuestProgressRatio;
CCSprite *pGreenGauge;


void TrainingLayer::InitLayer(int _step)
{
    this->removeAllChildrenWithCleanup(true);
    
    if (_step == 0){
        int base_y = 146;//73;//
        popupCnt = 0;
        
        CCSprite* pSpr3 = CCSprite::create("ui/card_tab/cards_training_slash.png");
        regSprite(this, ccp(0,0), accp(0, 334-base_y), pSpr3, 5);
        
        CCSprite* pSpr4 = CCSprite::create("ui/card_tab/cards_training_cardslot1.png");
        pSpr4->setTag(10);
        regSprite(this, ccp(0,0), accp(29,481-base_y), pSpr4, 5);
        
        CCSprite* pSpr5 = CCSprite::create("ui/card_tab/cards_training_cardslot2.png");
        pSpr5->setTag(11);
        regSprite(this, ccp(0,0), accp(450,157-base_y), pSpr5, 5);
        //CheckLayerSize(this);
        
        CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/cards_training_btn1.png","ui/card_tab/cards_training_btn2.png",this,menu_selector(TrainingLayer::MenuCallback));
        pSprBtn1->setAnchorPoint( ccp(0,0));
        pSprBtn1->setPosition( accp(0,0));
        pSprBtn1->setTag(0);
        
        CCMenu* pMenu = CCMenu::create(pSprBtn1,NULL);
        pMenu->setTag(99);
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( accp(232, 400-base_y));
        this->addChild(pMenu, 3000);
        
        if (fusionCard1 != NULL){
            
            cardMaker->MakeCardThumb(this, fusionCard1, ccp(29,481-base_y+17), 304, 10, 50);
            
            /*
            CCSprite* pSpr = CCSprite::create(GetCharImgPath(fusionCard1->getId(), 0));
            pSpr->setTag(50);
            
            CCSize aa = pSpr->getTexture()->getContentSizeInPixels();
            float cardScale = (float)290 / aa.height;
            pSpr->setScale(cardScale);
            regSprite(this, ccp(0,0), accp(29+6,481-base_y+20), pSpr, 10);
            */
            CCSprite* pSprite = CCSprite::create("ui/card_tab/cards_training_bg_a2.png");
            pSprite->setAnchorPoint(accp(0,0));
            pSprite->setPosition( accp(0,350-base_y-20) );
            this->addChild(pSprite, 0);
        }
        else{
            CCSprite* pSprite = CCSprite::create("ui/card_tab/cards_training_bg_a1.png");
            pSprite->setAnchorPoint(accp(0,0));
            pSprite->setPosition( accp(0,350-base_y-20) );
            this->addChild(pSprite, 0);
            
        }
        if (fusionCard2 != NULL){
            cardMaker->MakeCardThumb(this, fusionCard2, ccp(450,157-base_y+16), 242, 10, 51);
            /*
            CCSprite* pSpr = CCSprite::create(GetCharImgPath(fusionCard2->getId(), 0));
            
            CCSize aa = pSpr->getTexture()->getContentSizeInPixels();
            float cardScale = (float)230 / aa.height;
            pSpr->setScale(cardScale);
            
            regSprite(this, ccp(0,0), accp(450+6,157-base_y+20), pSpr, 10);
            pSpr->setTag(51);
            */
            CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_training_bg_b2.png");
            pSpr2->setAnchorPoint(ccp(0,0));
            pSpr2->setPosition( accp(0,-64-20) );
            this->addChild(pSpr2, 0);
        }
        else{
            CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_training_bg_b1.png");
            pSpr2->setAnchorPoint(ccp(0,0));
            pSpr2->setPosition( accp(0,-64-20) );
            this->addChild(pSpr2, 0);
        }
        
    }
    else if (_step == 1){
        // init card list
        this->setTouchEnabled(false);
        PlayerInfo *pi = PlayerInfo::getInstance();
        
        //// make available card list
        CCArray *cardlist;
        if (fusionCard1 == NULL && fusionCard2 == NULL){
            //cardlist = pi->myCards;
            
            cardlist = new CCArray();
            if (selectedFusionDeck == 0){
                
                for(int i=0;i<pi->myCards->count();i++){
                    CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);
                    
                    if (card->bTraingingMaterial)continue;
                    if (card->getLevel() == CardInfo::MAX_LEVEL)continue;
                    //if (fusionCard2 != NULL && fusionCard2->getLevel() > card->getLevel())continue;

                    cardlist->addObject(card);
                }
            }
            else if (selectedFusionDeck == 1){
                
                for(int i=0;i<pi->myCards->count();i++){
                    CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);

                    if (card->getLevel() ==  CardInfo::MAX_LEVEL)continue;
                    
                    //if (fusionCard1 != NULL && fusionCard1->getLevel() < card->getLevel())continue;
                    
                    cardlist->addObject(card);
                    
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
            
            CCLog("selectedCard->id:%d getSrl():%d", selectedCard->getId(), selectedCard->getSrl());
            
            cardlist = new CCArray();
            for(int i=0;i<pi->myCards->count();i++){
                CardInfo* card = (CardInfo*)pi->myCards->objectAtIndex(i);
                // 내 자신은 제외한 리스트를 보여준다.
                if (selectedFusionDeck == 0){
                    //if (selectedCard->getSrl() != card->getSrl() && card->getLevel()< CardInfo::MAX_LEVEL && card->bTraingingMaterial == false){
                    if (selectedCard->getSrl() == card->getSrl())continue;
                    if (card->getLevel() == CardInfo::MAX_LEVEL)continue;
                    if (card->bTraingingMaterial)continue;
                    
                    cardlist->addObject(card);
                }
                else if (selectedFusionDeck == 1){
                    // 단련용 카드만 들어가야함
                    if (selectedCard->getSrl() != card->getSrl()){
                        //CCLog("card->id:%d, srl:%d", card->getId(), card->getSrl());
                        cardlist->addObject(card);
                    }
                }
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
        cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width,size.height),fusionCard1, this);
        this->addChild(cardDetailViewLayer,10);
        
        CCSprite* pSpr = CCSprite::create("ui/card_tab/cards_text_bg.png");
        pSpr->setAnchorPoint(ccp(0,0));
        pSpr->setPosition(accp(62,160));
        this->addChild(pSpr, 20);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create(" 단련으로 인해 카드가 강해졌습니다", "HelveticaNeue-Bold", 12);
        pLabel1->setColor(COLOR_WHITE);
        
        std::string text_att ="+공격력 ";
        std::string text_def ="+체력 ";
        
        CCLabelTTF* pLabel2 = CCLabelTTF::create(text_att.c_str(), "HelveticaNeue", 12);
        pLabel2->setColor(COLOR_ORANGE);
        
        char buf1[10];
        sprintf(buf1, "%d", fusionCard1->getAttack() - originAttack);
        
        CCLabelTTF* pLabel4 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
        pLabel4->setColor(COLOR_WHITE);

        CCLabelTTF* pLabel3 = CCLabelTTF::create(text_def.c_str(), "HelveticaNeue", 12);
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
        /*
        CCSprite* pSpr2 = CCSprite::create("ui/card_tab/cards_training_lvup.png");
        pSpr2->setAnchorPoint(ccp(0,0));
        pSpr2->setPosition(accp(152,532));
        this->addChild(pSpr2, 20);
        */
        
        //int aaa = fusionCard1->getExp() + 50;
        
        progressStartVal = originExp;
        progressTargetVal = fusionCard1->getExp();
        ProgressMaxVal = originExpCap;
        progressNextTargetVal = 0;
        
        if (fusionCard1->getLevel() > originLevel){
            progressTargetVal = originExpCap;
            progressNextTargetVal = fusionCard1->getExp();//progressTargetVal - ProgressMaxVal;
        }
        
        InitGauge(progressStartVal, progressTargetVal, ProgressMaxVal);
        
        // trainig effect 동안 block한 touch를 다시 enable 시킨다.
        this->setTouchEnabled(true);
        
        trainingCard1IDforLevelUp = fusionCard1->getId();
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.4f);
        CCCallFunc* callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::RunAction));
        this->runAction(CCSequence::actions(delay1, callBack, NULL));
    }
    
    bTouchBegan = false;
}

void TrainingLayer::MenuCallback(cocos2d::CCObject *pSender)
{
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    switch(tag){
        case 0:
            if (fusionCard1 == NULL || fusionCard2 == NULL)
                break;
            
            soundButton2();
            
            // training possible check
            if (fusionCard1->getLevel() == CardInfo::MAX_LEVEL)
            {
                AlertPopup *popup = new AlertPopup;
                popup->InitUI((void*)LocalizationManager::getInstance()->get("training_max_popup"));
                this->addChild(popup, 5000);
                return;
            }

            // coin check
            //int needed = (int)ceil(((double)fusionCard1->getPrice() * 0.6 + (double)fusionCard2->getPrice() * 0.6));
            int needed = fusionCard2->getPrice();
            if (PlayerInfo::getInstance()->myCoin < needed)
            {
                Purchase2Popup *popup = new Purchase2Popup;
                popup->InitUI((void*)LocalizationManager::getInstance()->get("Training_popup"));
                this->addChild(popup, 5000);
                return;
            }
            else{
                if (popupCnt == 0){
                    popupCnt++;
                    TrainingPopup *popup = new TrainingPopup;
                    
                    std::string text = "단련에는 xx골드가 필요합니다. \n 카드를 단련시키시겠습니까? \n \n단련 재료로 사용된 카드는 \n 단련후 사라집니다.";
                    
                    popup->InitUI((void*)text.c_str(),needed);//(void*)LocalizationManager::getInstance()->get("fusion_popup"));
                    this->addChild(popup, 5000);
                    
                    // trainig effect 동안 touch disable
                    this->setTouchEnabled(false);
                }
            }

            //DoCardTraining(fusionCard1, fusionCard2);
            break;
    }
}



void TrainingLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchBeganLocation = location;
    
    bTouchBegan = true;
    
}

void TrainingLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    if (bTouchBegan==false)return;
    
    bTouchBegan = true;
    
    CCLog("touch ended, fusionStep:%d",fusionStep);
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (fabs(location.x - touchBeganLocation.x) < 30 && fabs(location.y - touchBeganLocation.y) < 30){
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (fusionStep !=0)return;
    
    CCLog(" fusionStep :%d", fusionStep);
    
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
                    
                    fusionStep = 1;
                    InitLayer(fusionStep);
                }
            }
        }
    //}
    }
}

void TrainingLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //bTouchMove = true;
}


void TrainingLayer::ButtonA(CardInfo* card)
{
    if (PlayerInfo::getInstance()->myCards->count()>2){
        if (selectedFusionDeck ==1 && PlayerInfo::getInstance()->isBelongInTeam(card)){
            const char* text = "팀에 설정되어 있는 카드 입니다.\n 단련후에는 팀에서 카드가 삭제되므로 \n 팀 설정을 다시 해주시기 바랍니다.\n 단련 하시겠습니까?";
            popupOkCancel(card, text, this);
        }
        else{
            
            AddCardToFusionDeck(card, selectedFusionDeck);
            
            this->removeChild(cardListLayer, true);
            
            this->setTouchEnabled(true);
        }
    }
    else{
        popupOk("최소 2장의 카드는 소지해야 합니다.");
    }
    
}

void TrainingLayer::CardListBackBtnPressed()
{
    fusionStep = 0;
    InitLayer(fusionStep);
    this->removeChild(cardListLayer, true);
    this->setTouchEnabled(true);
}

void TrainingLayer::ButtonOK(CardInfo* card){
    
    MainScene::getInstance()->removePopup();
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
    
    AddCardToFusionDeck(card, selectedFusionDeck);
    
}

void TrainingLayer::ButtonCancel()
{
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
}

void TrainingLayer::AddCardToFusionDeck(CardInfo *card, int deck)
{
    if (deck==0){
        fusionCard1 = card;
        originAttack = fusionCard1->getAttack();
        originDefense = fusionCard1->getDefence();
        originExp = fusionCard1->getExp();
        originLevel = fusionCard1->getLevel();
        
        CardInfo *cardFromDic = CardDictionary::sharedCardDictionary()->getInfo(fusionCard1->getId());
        cardFromDic->setLevel(fusionCard1->getLevel());
        originExpCap = cardFromDic->getExpCap();
    
        
        selectedCard = card;
        CCLog(" card attack:%d defense:%d expCap:%d", originAttack, originDefense, originExpCap);
    }
    else if (deck==1){
        fusionCard2 = card;
    }
    selectedCard = card;
    fusionStep = 0;
    InitLayer(fusionStep);
    
}

void TrainingLayer::setTrainingData(ResponseTrainingInfo* _data)
{
    trainingInfo = _data;
}

void TrainingLayer::DoCardTraining()
{
    if(trainingInfo)
    {
        if (atoi(trainingInfo->res)==0){
            
            fusionCard1->setSrl(trainingInfo->card_srl);
            fusionCard1->setId(trainingInfo->card_id);
            fusionCard1->setLevel(trainingInfo->card_level);
            fusionCard1->setExp(trainingInfo->card_exp);
            fusionCard1->setAttack(trainingInfo->attack);
            fusionCard1->setDefence(trainingInfo->defense);
            fusionCard1->setSkillEffect(trainingInfo->skill_effect);
            //fusionCard1->updateRare();
            
            PlayerInfo::getInstance()->myCards->removeObject(fusionCard2);
            PlayerInfo::getInstance()->addCoin(-trainingInfo->cost);
            UserStatLayer::getInstance()->refreshCoin();
            
            //fusionStep = 2;
            //InitLayer(fusionStep);
        }
        else{
            popupNetworkError(trainingInfo->res,trainingInfo->msg, "training");
            fusionStep = 0;
            InitLayer(fusionStep);
        }
    }
    else
    {
        popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
        return;
    }
    
    CCSize size = GameConst::WIN_SIZE; //CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* effectBtn = CCSprite::create("ui/card_tab/cards_training_btn_eff.png");
    effectBtn->setTag(900);
    effectBtn->setScale(2.0f);
    effectBtn->setOpacity(0);
    effectBtn->setAnchorPoint(ccp(0.5f, 0.5f));
    effectBtn->setPosition(accp(size.width*SCREEN_ZOOM_RATE/2.0f, (size.height*SCREEN_ZOOM_RATE/2.0f)-170.0f));
    this->addChild(effectBtn, 6000);
  
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.3f);
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.1f);
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.3f);
    
    CCCallFunc* callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionSlide));

    effectBtn->runAction(CCSequence::actions(fadein, delay, fadeout, callBack, NULL));
}

void TrainingLayer::actionSlide()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();

    // -- image width = 640
    CCSprite* topBG = CCSprite::create("ui/battle/skill_cha_bg.png");
    topBG->setTag(901);
    topBG->setScale(0.8f);
    topBG->setAnchorPoint(ccp(0.0f, 0.0f));
    topBG->setPosition(accp(-640.0f*0.8f, 400.0f));
    this->addChild(topBG, 2000);

    
    CCEaseOut *moveTopBG = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.15f, accp(0.0f, 400.0f)), 10.0f);
    topBG->runAction(CCSequence::actions(moveTopBG, NULL));
    
    // -- image width = 512 / height = 256
    CardListInfo* cardInfo1 = FileManager::sharedFileManager()->GetCardInfo(fusionCard1->getId());
    string path1 = CCFileUtils::sharedFileUtils()->getDocumentPath();
    path1+=cardInfo1->smallBattleImg;
    
    topCha = CCSprite::create(path1.c_str());
    topCha->setTag(902);
    topCha->setAnchorPoint(ccp(0.5f, 0.5f));
    topCha->setPosition(accp(-512.0f, 404.0f + 256.0f / 2.0f));
    this->addChild(topCha, 2000);

    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
    CCEaseOut *moveTopCha = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.15f, accp(0.0f + 512.0f/2.0f, 404.0f + 256.0f / 2.0f)), 10.0f);
    CCCallFunc* callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionScale));
    topCha->runAction(CCSequence::actions(delay1, moveTopCha, callBack, NULL));
    
    // -- image width = 640
    CCSprite* bottomBG = CCSprite::create("ui/battle/skill_cha_bg.png");
    bottomBG->setTag(903);
    bottomBG->setScale(0.8f);
    bottomBG->setFlipX(true);
    bottomBG->setAnchorPoint(ccp(1.0f, 0.0f));
    bottomBG->setPosition(accp(size.width*SCREEN_ZOOM_RATE + 640.0f, 30.0f));
    this->addChild(bottomBG, 5000);

    CCEaseOut *moveBottomBG = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.15f, accp(size.width*SCREEN_ZOOM_RATE, 30.0f)), 10.0f);
    bottomBG->runAction(CCSequence::actions(moveBottomBG, NULL));

    // -- image width = 512
    CardListInfo* cardInfo2 = FileManager::sharedFileManager()->GetCardInfo(fusionCard2->getId());
    string path2 = CCFileUtils::sharedFileUtils()->getDocumentPath();
    path2+=cardInfo2->smallBattleImg;

    bottomCha = CCSprite::create(path2.c_str());
    bottomCha->setTag(904);
    bottomCha->setFlipX(true);
    bottomCha->setAnchorPoint(ccp(1.0f, 0.0f));
    bottomCha->setPosition(accp(size.width*SCREEN_ZOOM_RATE + 512.0f, 34.0f));
    this->addChild(bottomCha, 5000);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.1f);
    CCEaseOut *moveBottomCha = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.15f, accp(size.width*SCREEN_ZOOM_RATE, 34.0f)), 10.0f);
    bottomCha->runAction(CCSequence::actions(delay2, moveBottomCha, NULL));
}

void TrainingLayer::actionScale()
{
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
    CCFiniteTimeAction* Scale1 = CCScaleTo::actionWithDuration(0.07f, 1.3f);
    CCFiniteTimeAction* Scale2 = CCScaleTo::actionWithDuration(0.07f, 1.0f);
    CCCallFunc* callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionShake));
    topCha->runAction(CCSequence::actions(delay, Scale1, Scale2, callBack, NULL));
}

void TrainingLayer::actionShake()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCFiniteTimeAction* shake   = CCShake::create(1.0f, 8.0f, size.width*SCREEN_ZOOM_RATE, 34.0f);

    bottomCha->runAction(CCSequence::actions(shake, NULL));
    
    CCCallFunc* HitLT = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionHitLT));
    CCCallFunc* HitLB = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionHitLB));
    CCCallFunc* HitM = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionHitM));
    CCCallFunc* HitRT = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionHitRT));
    CCCallFunc* HitRB = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionHitRB));
    
    CCCallFunc* FadeIn = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::actionFadein));

    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
    
    this->runAction(CCSequence::actions(HitM,
                                        delay1, HitLT,
                                        delay1, HitRB,
                                        delay1, HitRT,
                                        delay1, HitLB,
                                        delay1, HitM,
                                        delay1, HitLT,
                                        delay1, HitRB,
                                        delay1, HitRT,
                                        delay1, HitLB,
                                        delay1, FadeIn,
                                        NULL));
}

void TrainingLayer::actionFadein()
{
    CCSprite* whiteBG = CCSprite::create("ui/shop/bg_white.png");
    whiteBG->setAnchorPoint(ccp(0.0f, 0.0f));
    whiteBG->setPosition((accp(0.f, 0.0f)));
    whiteBG->setOpacity(0);
    whiteBG->setTag(905);
    MainScene::getInstance()->addChild(whiteBG, 12060);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.5f);
    CCCallFunc* remove = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::removeActions));
    whiteBG->runAction(CCSequence::actions(fadein, remove, NULL));
    
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    CCCallFunc* detailview = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::Detailview));
    this->runAction(CCSequence::actions(delay1, detailview, NULL));
}

void TrainingLayer::Detailview()
{
    fusionStep = 2;
    InitLayer(fusionStep);
}

void TrainingLayer::removeActions()
{
    this->removeChildByTag(900, true);
    this->removeChildByTag(901, true);
    this->removeChildByTag(902, true);
    this->removeChildByTag(903, true);
    this->removeChildByTag(904, true);
    MainScene::getInstance()->removeChildByTag(905, true);
}

void TrainingLayer::actionHitLT()
{
    CCSize size = GameConst::WIN_SIZE;
    playEffect("audio/hit_01.mp3");
    AniPlay(effectHit[3], aniFrame, this, ccp(0.0f, 0.0f), accp(184.0f, size.height - 800.0f), 2.0f, 600, 6500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void TrainingLayer::actionHitLB()
{
    CCSize size = GameConst::WIN_SIZE;
    playEffect("audio/hit_01.mp3");
    AniPlay(effectHit[4], aniFrame, this, ccp(0.0f, 0.0f), accp(200.0f, size.height - 650.0f), 2.0f, 600, 6500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void TrainingLayer::actionHitM()
{
    CCSize size = GameConst::WIN_SIZE;
    playEffect("audio/hit_01.mp3");
    AniPlay(effectHit[2], aniFrame, this, ccp(0.0f, 0.0f), accp(310.0f, size.height - 740.0f), 2.0f, 600, 6500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void TrainingLayer::actionHitRT()
{
    CCSize size = GameConst::WIN_SIZE;
    playEffect("audio/hit_01.mp3");
    AniPlay(effectHit[1], aniFrame, this, ccp(0.0f, 0.0f), accp(404.0f, size.height - 620.0f), 2.0f, 600, 6500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void TrainingLayer::actionHitRB()
{
    CCSize size = GameConst::WIN_SIZE;
    playEffect("audio/hit_01.mp3");
    AniPlay(effectHit[0], aniFrame, this, ccp(0.0f, 0.0f), accp(440.0f, size.height - 780.0f), 2.0f, 600, 6500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void TrainingLayer::loadImage()
{
    //
    addPageLoading();
    
    bool startTraining = true;
    
    //FOC_IMAGE_SERV_URL
    std::string basePathS  = FOC_IMAGE_SERV_URL;
    basePathS.append("images/cha/cha_s/");
    
    std::vector<std::string> downloads;

    FileManager* fmanager = FileManager::sharedFileManager();
    
    CardListInfo* cardInfo1 = FileManager::sharedFileManager()->GetCardInfo(fusionCard1->getId());
    
    if(!fmanager->IsFileExist(cardInfo1->GetSmallBattleImg()))
    {
        startTraining = false;
        
        string downPath = basePathS + cardInfo1->GetSmallBattleImg();
        downloads.push_back(downPath);
    }
    
    CardListInfo* cardInfo2 = FileManager::sharedFileManager()->GetCardInfo(fusionCard2->getId());
    
    if(!fmanager->IsFileExist(cardInfo2->GetSmallBattleImg()))
    {
        startTraining = false;
        
        string downPath = basePathS + cardInfo2->GetSmallBattleImg();
        downloads.push_back(downPath);
    }
    
    if(false == startTraining)
    {
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        requestor->addDownloadTask(downloads, this, callfuncND_selector(TrainingLayer::onHttpRequestCompleted));
    }
    
    if(true == startTraining)
    {
        removePageLoading();
        ARequestSender::getInstance()->requestTraining(fusionCard1->getSrl(), fusionCard2->getSrl());
    }

}

void TrainingLayer::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("training image download complete");
        removePageLoading();
        ARequestSender::getInstance()->requestTraining(fusionCard1->getSrl(), fusionCard2->getSrl());
    }
}

void TrainingLayer::CloseDetailView()
{
    this->stopAllActions();
    
    MainScene::getInstance()->setEnableMainMenu(true);
    UserStatLayer::getInstance()->setEnableMenu(true);
    DojoLayerCard::getInstance()->setEnableMainMenu(true);
    
    CCMenu* Menu = (CCMenu*)this->getChildByTag(99);
    
    if(Menu)
        Menu->setEnabled(true);
    
    this->setTouchEnabled(true);

    this->removeChild(cardDetailViewLayer, true);
    
    fusionStep = 0;
    fusionCard1 = NULL;
    fusionCard2 = NULL;
    InitLayer(fusionStep);
    
    RestoreZProirity(this);
    
    
}

void TrainingLayer::InitGauge(int _from, int _to, int _max)
{
    
    if (progressTargetVal >= _max){
        progressTargetVal = _max;
    }
    
//    int progress = _from;
//    int progressMax = _max;
    
    QuestProgressRatio = (float)(_from-1.0f) / (float)_max;
    if(QuestProgressRatio <= 0.0f)
        QuestProgressRatio = 0.0f;
    
    
    ProgressTargetRatio = (float)(_to-1.0f) / (float)_max;
    if(ProgressTargetRatio <= 0.0f)
        ProgressTargetRatio = 0.0f;
        
    increaseQuestProgressRatio = 1.0f / _max;
    if(increaseQuestProgressRatio + QuestProgressRatio >= 1.0f)
        increaseQuestProgressRatio = 0.0f;
    
    CCLog(" QuestProgressRatio :%f", QuestProgressRatio);
    CCLog(" increaseQuestProgressRatio:%f", increaseQuestProgressRatio);
    
    this->removeChildByTag(101, true);
    this->removeChildByTag(102, true);
    this->removeChildByTag(103, true);
    
    CCSprite *pSprBG = CCSprite::create("ui/card_tab/cards_training_gage_bg.png");
    pSprBG->setAnchorPoint(ccp(0, 0));
    pSprBG->setPosition(accp(78,464));
    pSprBG->setTag(101);
    this->addChild(pSprBG,1000);
    
    pGreenGauge = NULL;
    pGreenGauge = CCSprite::create("ui/card_tab/cards_training_gage.png");
    pGreenGauge->setAnchorPoint(ccp(0, 0));
    pGreenGauge->setScaleX(QuestProgressRatio);
    pGreenGauge->setPosition(accp(78+63, 464+20));
    pGreenGauge->setTag(102);
    this->addChild(pGreenGauge,1000);
    
    CCSprite *pSprBG2 = CCSprite::create("ui/card_tab/cards_training_gage_bg_u.png");
    pSprBG2->setAnchorPoint(ccp(0, 0));
    pSprBG2->setPosition(accp(78,464));
    pSprBG2->setTag(103);
    this->addChild(pSprBG2,1001);
    
}

void TrainingLayer::increaseGreenGauge()
{
    //increaseQuestProgressRatio;
    //CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, QuestProgressRatio + increaseQuestProgressRatio, 1.0f);
    CCFiniteTimeAction* Scale = CCScaleTo::actionWithDuration(0.5f, ProgressTargetRatio, 1.0f);
    pGreenGauge->runAction(Scale);
}

void TrainingLayer::showLevelUp()
{
    CCSprite *pSprlvup = CCSprite::create("ui/card_tab/cards_training_lvup.png");
    pSprlvup->setAnchorPoint(ccp(0, 0));
    pSprlvup->setPosition(accp(122,500));
    this->addChild(pSprlvup,1000);
}

void TrainingLayer::InitNextGauge()
{
    
    //CardInfo *cardFromDic = CardDictionary::sharedCardDictionary()->getInfo(fusionCard1->getId());
    CardInfo *cardFromDic = CardDictionary::sharedCardDictionary()->getInfo(trainingCard1IDforLevelUp);
    cardFromDic->setLevel(fusionCard1->getLevel());
    originExpCap = cardFromDic->getExpCap();
    
    ProgressMaxVal = originExpCap;
    progressTargetVal = progressNextTargetVal;
    progressNextTargetVal = 0;
    if (progressTargetVal >= ProgressMaxVal){
        progressNextTargetVal = progressTargetVal - ProgressMaxVal;
    }
    InitGauge(0, progressTargetVal, ProgressMaxVal);
    RunAction();
}

void TrainingLayer::RunAction(){
    
    CCCallFunc *greenScale = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::increaseGreenGauge));
    
    if (progressTargetVal == ProgressMaxVal){
        
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
        
        CCCallFunc *callBackLevelUp = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::showLevelUp));
        
        if (progressNextTargetVal > 0){
            CCCallFunc *callBackNextGauge = CCCallFunc::actionWithTarget(this, callfunc_selector(TrainingLayer::InitNextGauge));
            
            this->runAction(CCSequence::actions(greenScale, delay1, callBackLevelUp, callBackNextGauge, NULL));
        }
        else{
            // level up
            this->runAction(CCSequence::actions(greenScale, delay1, callBackLevelUp, NULL));
            
        }
    }
    else{
        this->runAction(CCSequence::actions(greenScale, NULL));
        
    }
//    if (PlayerInfo::getInstance()->getUpgradePoint()!=0)
//        bLevelUp = true;
//    
//    if(bLevelUp)
//        this->runAction(CCSequence::actions(delay1, layerMove, delay1, callBack, NULL));
//    else
//        this->runAction(CCSequence::actions(delay1, layerMove, NULL));
    
    
    
}