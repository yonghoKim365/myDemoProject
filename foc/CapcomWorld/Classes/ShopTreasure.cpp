//
//   ShopTreasure.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 20..
//
//

#include "ShopTreasure.h"

static int famePoints[4] = { 20, 50, 100, 200 };

ShopTreasure::ShopTreasure(CCSize layerSize) : basePopUp(NULL)
{
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    
    bTouchMove = false;
    
    InitUI();
}

ShopTreasure::~ShopTreasure()
{
    CC_SAFE_DELETE(reward);
        
    this->removeAllChildrenWithCleanup(true);
}

void ShopTreasure::InitUI()
{
    StartYPos = this->getContentSize().height*SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN - 170 - MAIN_LAYER_TOP_MARGIN - 28;
    
    CCSprite* HonorBG = CCSprite::create("ui/shop/shop_honorpoint_bg.png");
    HonorBG->setAnchorPoint(ccp(0.0f, 0.0f));
    HonorBG->setPosition(accp(10.0f, StartYPos + 110.0f));
    HonorBG->setTag(1000);
    this->addChild(HonorBG);
    
    CCLabelTTF* MyHonor = CCLabelTTF::create("명성 포인트", "HelveticaNeue-Bold", 12);
    MyHonor->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(178, StartYPos + 118.0f), MyHonor, 60);
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    const int HonorPoint = pInfo->getFame();
    
    char buff[10];
    sprintf(buff, "%d", HonorPoint);
    
    CCLabelTTF* HonorPointLebel = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
    HonorPointLebel->setTag(999);
    HonorPointLebel->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(300, StartYPos + 118.0f), HonorPointLebel, 60);

    StartYPos-=45.0f;
    
    makeCell("ui/shop/item_box_a.png", LocalizationManager::getInstance()->get("501_name"), "낮은", "20P", 0);
    makeCell("ui/shop/item_box_b.png", LocalizationManager::getInstance()->get("502_name"), "보통", "50P", 1);
    makeCell("ui/shop/item_box_c.png", LocalizationManager::getInstance()->get("503_name"), "높은", "100P", 2);
    makeCell("ui/shop/item_box_d.png", LocalizationManager::getInstance()->get("504_name"), "높은", "200P", 3);
}

void ShopTreasure::makeCell(const string& _SprPath, const string& _BoxName, const string& _Desc, const string& _HonorPoint, int _tag)
{
    CCSprite* pSprItemBG = CCSprite::create("ui/shop/item_bgs.png");
    pSprItemBG->setAnchorPoint(ccp(0,0));
    pSprItemBG->setPosition(accp(10, StartYPos));
    this->addChild(pSprItemBG, 60);
    
    CCSprite* BoxSpr = CCSprite::create(_SprPath.c_str());
    BoxSpr->setAnchorPoint(ccp(0,0));
    BoxSpr->setPosition(accp(25, StartYPos + 15));
    this->addChild(BoxSpr, 60);
    
    CCLabelTTF* BoxName = CCLabelTTF::create(_BoxName.c_str(), "HelveticaNeue-Bold", 12);
    BoxName->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 100), BoxName, 60);
    
    string Description = _Desc + " 확률로 카드를 획득\n할 수 있습니다.";
    CCLabelTTF* BoxDescription1 = CCLabelTTF::create(Description.c_str(), "HelveticaNeue", 12);
    BoxDescription1->setColor(COLOR_GRAY);
    BoxDescription1->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 40), BoxDescription1, 60);
    
    //CCLabelTTF* BoxDescription2 = CCLabelTTF::create("할 수 있습니다.", "HelveticaNeue-Bold", 12);
    //BoxDescription2->setColor(COLOR_GRAY);
    //registerLabel( this,ccp(0, 0), accp(159, StartYPos + 40), BoxDescription2, 60);
    
    CCLabelTTF* Honor = CCLabelTTF::create("필요 명성", "HelveticaNeue", 12);
    Honor->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(159, StartYPos + 10), Honor, 60);
    
    CCLabelTTF* Honor2 = CCLabelTTF::create(_HonorPoint.c_str(), "HelveticaNeue-Bold", 12);
    Honor2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(255, StartYPos + 10), Honor2, 60);
    
    CCSprite* OpenBtn = CCSprite::create("ui/shop/list_btn1.png");
    OpenBtn->setTag(_tag);
    OpenBtn->setAnchorPoint(ccp(0, 0));
    OpenBtn->setPosition(accp(530, StartYPos+30));
    this->addChild(OpenBtn, 60);
    
    CCLabelTTF* OpenLabel = CCLabelTTF::create("열기", "HelveticaNeue-Bold", 12);
    OpenLabel->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(555, StartYPos + 55), OpenLabel, 70);
    
    StartYPos-= 155;

}

void ShopTreasure::InitPopUp(void *data)
{
    if(NULL == basePopUp)
    {
        basePopUp = new TreasurePopUp();
        basePopUp->InitUI(data);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        
        ShopLayer* parent = (ShopLayer*)this->getParent();
        
        basePopUp->setPosition(accp(0.0f, -parent->getPositionY() * SCREEN_ZOOM_RATE));
        
        this->addChild(basePopUp, 1000);
    }
}

void ShopTreasure::RemovePopUp()
{
    this->setTouchEnabled(true);
    
    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }
}

void ShopTreasure::requestTreasure(CCNode* sender, void* _boxInfo)
{
    //int* boxID = (int*)_boxInfo;

    ResponseTbInfo *responseInfo = (ResponseTbInfo*)_boxInfo;// ARequestSender::getInstance()->requestTb((*boxID));
    
    if(responseInfo)
    {
        if (atoi(responseInfo->res) == 0){
            PlayerInfo* pi = PlayerInfo::getInstance();
            pi->setRevengePoint(responseInfo->user_stat_revenge);
            pi->setFame(responseInfo->user_stat_fame);
            pi->setStamina(responseInfo->user_stat_q_pnt);
            //pi->setDefensePoint(responseInfo->user_stat_d_pnt);
            pi->setBattlePoint(responseInfo->user_stat_a_pnt);
            pi->setUpgradePoint(responseInfo->user_stat_u_pnt);
            pi->setCash(responseInfo->user_stat_gold);
            pi->setCoin(responseInfo->user_stat_coin);
            pi->setXp(responseInfo->user_stat_exp);
            pi->setLevel(responseInfo->user_stat_lev);
            UserStatLayer::getInstance()->refreshUI();
            
            if (responseInfo->card_id != 0){
                CardInfo *card = new CardInfo();
                card->setId(responseInfo->card_id);
                card->setExp(responseInfo->card_exp);
                card->setLevel(responseInfo->card_lev);
                card->setSkillEffect(responseInfo->card_skill_effect);
                card->setDefence(responseInfo->card_defense);
                card->setAttack(responseInfo->card_attack);
                card->setSrl(responseInfo->card_srl);
                
                PlayerInfo::getInstance()->addCard(responseInfo->card_id, card);
            }
            
            CCLabelTTF* label = (CCLabelTTF*)this->getChildByTag(999);
            float x = label->getPositionX();
            float y = label->getPositionY();
            
            removeChildByTag(999, true);
            
            char buff[10];
            sprintf(buff, "%d", responseInfo->user_stat_fame);
            
            CCLabelTTF* HonorPointLebel = CCLabelTTF::create(buff, "Thonburi", 12);
            HonorPointLebel->setTag(999);
            HonorPointLebel->setColor(COLOR_WHITE);
        registerLabel( this,ccp(0, 0), accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE), HonorPointLebel, 60);
        }
        else{
            popupNetworkError(responseInfo->res, responseInfo->msg, "requestSellCard");
        }
        
        InitPopUp(responseInfo);
    }
    else
    {
        this->setTouchEnabled(true);
        popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요.");
    }
}

void ShopTreasure::eff1()
{
    CCSprite *eff1 = CCSprite::create("ui/shop/item_box_open_eff01.png");
    eff1->setAnchorPoint(ccp(0.5f, 0.0f));
    eff1->setPosition((accp(320.f, 220.0f)));
    eff1->setOpacity(0);
    eff1->setTag(802);
    MainScene::getInstance()->addChild(eff1, 1200);
    
    CCFadeIn* fadein1 = CCFadeIn::actionWithDuration(1.0f);
    eff1->runAction(fadein1);

}
void ShopTreasure::eff2()
{
    CCSprite *eff2 = CCSprite::create("ui/shop/item_box_open_eff02.png");
    eff2->setAnchorPoint(ccp(0.5f, 0.0f));
    eff2->setPosition((accp(320.f, 220.0f)));
    eff2->setOpacity(0);
    eff2->setTag(803);
    MainScene::getInstance()->addChild(eff2, 1300);
    
    CCFadeIn* fadein2 = CCFadeIn::actionWithDuration(1.0f);
    eff2->runAction(fadein2);
}

void ShopTreasure::whiteBG()
{
    CCSprite* whiteBG = CCSprite::create("ui/shop/bg_white.png");
    whiteBG->setAnchorPoint(ccp(0.0f, 0.0f));
    whiteBG->setPosition((accp(0.f, 0.0f)));
    whiteBG->setOpacity(0);
    whiteBG->setTag(804);
    MainScene::getInstance()->addChild(whiteBG, 1500);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.5f);
    CCCallFunc* callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopTreasure::releaseEff));
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.5f);
    whiteBG->runAction(CCSequence::actions(fadein, callBack4, fadeout, NULL));
    
    
}

void ShopTreasure::releaseBG()
{
    MainScene::getInstance()->removeChildByTag(800, true);
    MainScene::getInstance()->removeChildByTag(804, true);
}

void ShopTreasure::releaseEff()
{
    MainScene::getInstance()->removeChildByTag(801, true);
    MainScene::getInstance()->removeChildByTag(802, true);
    MainScene::getInstance()->removeChildByTag(803, true);
}

void ShopTreasure::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    
    for(int i=0; i<4; ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            ChangeSpr(this, i, "ui/shop/list_btn2.png", 60);
        }
    }
}

void ShopTreasure::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=0; i<4; ++i)
    {
        ChangeSpr(this, i, "ui/shop/list_btn1.png", 60);
        
        if(false == bTouchMove)
        {
            if(GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                soundButton1();
                
                if(PlayerInfo::getInstance()->getFame() < famePoints[i])
                {
                    InitPopUp(NULL);
                }
                else if(NUM_OF_MAX_CARD <= PlayerInfo::getInstance()->myCards->count())
                {
                    void* data = (void*)"maxcard";
                    InitPopUp(data);
                }
                else
                {
                    MainScene::getInstance()->setEnableMainMenu(false);
                    UserStatLayer::getInstance()->setEnableMenu(false);
                    ShopLayer::getInstance()->setTouchEnabled(false);
                    
                    ResponseTbInfo *responseInfo = ARequestSender::getInstance()->requestTb(501+i);

                    if(responseInfo)
                    {
                        CCSprite* blackBG = CCSprite::create("ui/shop/bg_black.png");
                        blackBG->setAnchorPoint(ccp(0.0f, 0.0f));
                        blackBG->setPosition((accp(0.f, 0.0f)));
                        blackBG->setOpacity(177);
                        blackBG->setTag(800);
                        MainScene::getInstance()->addChild(blackBG, 1000);
                        
                        CCSprite* Box = NULL;
                        if(0==i)
                            Box = CCSprite::create("ui/shop/item_box_open_a.png");
                        else if (1==i)
                            Box = CCSprite::create("ui/shop/item_box_open_b.png");
                        else if (2==i)
                            Box = CCSprite::create("ui/shop/item_box_open_c.png");
                        else if (3==i)
                            Box = CCSprite::create("ui/shop/item_box_open_d.png");

                        Box->setAnchorPoint(ccp(0.5f, 0.0f));
                        Box->setPosition((accp(320.f, 220.0f)));
                        Box->setTag(801);
                        MainScene::getInstance()->addChild(Box, 1100);
                        
                        CCCallFunc* callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopTreasure::eff1));
                        CCCallFunc* callBack2 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopTreasure::eff2));
                        CCCallFunc* callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopTreasure::whiteBG));
                        CCCallFunc* callBack6 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopTreasure::releaseBG));
                        
                        CCDelayTime* delay1 = CCDelayTime::actionWithDuration(0.5f);
                        CCDelayTime* delay2 = CCDelayTime::actionWithDuration(1.0f);
                        
                        CCCallFuncND *callBack5 = CCCallFuncND::actionWithTarget(this, callfuncND_selector(ShopTreasure::requestTreasure), (void*)responseInfo);
                        
                        this->runAction(CCSequence::actions(callBack1, delay1, callBack2, delay2, callBack3, delay2, callBack6, callBack5, NULL));
                        
                        this->setTouchEnabled(false);
                    }
                    else
                        popupOk("서버와의 연결이 원활하지 않습니다. \n 잠시후에 다시 시도해주세요.");
                }
            }
        }
    }
    
    bTouchMove = false;
}

void ShopTreasure::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (touchStartPoint.y != location.y)
    {
        touchStartPoint.y = location.y;
    }
    
    float distance = fabs(touchStartPoint.y - location.y);
    
    if (distance > 5.0f)
        bTouchMove = true;
}