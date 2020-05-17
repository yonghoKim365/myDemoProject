//
//  ShopRoulette.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 21..
//
//

#include "ShopRoulette.h"

ShopRoulette* ShopRoulette::instance = NULL;

ShopRoulette* ShopRoulette::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

ShopRoulette::ShopRoulette(CCSize layerSize) : rouletteMachine1(NULL), rouletteMachine2(NULL), rouletteMachine3(NULL)
{
    instance = this;
    
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    
    bTouchMove= false;
    
    medalCount = 0;
}

ShopRoulette::~ShopRoulette()
{
    this->removeAllChildrenWithCleanup(true);
}

void ShopRoulette::setMedalCount(int _count)
{
    medalCount = _count;
}

void ShopRoulette::InitUI()
{
    CCLabelTTF* medalLabel = CCLabelTTF::create("메달", "HelveticaNeue-Bold", 12);
    medalLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(320.0f, 635.0f), medalLabel, 160);
    
    char buff[10];
    sprintf(buff, "%d", medalCount);
    
    CCLabelTTF* medalcountLabel = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
    medalcountLabel->setColor(COLOR_YELLOW);
    medalcountLabel->setTag(777);
    registerLabel(this, ccp(0.0f, 0.0f), accp(354.0f, 635.0f), medalcountLabel, 160);

    CCSprite* RouletteBg = CCSprite::create("ui/shop/spin_bg.png");
    RouletteBg->setAnchorPoint(ccp(0.0f, 0.0f));
    RouletteBg->setPosition(accp(10.0f, 70.0f));
    RouletteBg->setTag(1000);
    this->addChild(RouletteBg);
    
    CCSprite* SendMedal = CCSprite::create("ui/shop/spin_btn_a1.png");
    SendMedal->setAnchorPoint(ccp(0.0f, 0.0f));
    SendMedal->setPosition(accp(10.0f, 32.0f));
    SendMedal->setTag(2000);
    this->addChild(SendMedal);
    
    CCLabelTTF* sendLabel = CCLabelTTF::create("메달 보내기", "HelveticaNeue-Bold", 12);
    sendLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(9.0f + 236.0f/2.0f, 46.0f), sendLabel, 160);
    
    CCSprite* ReceiveMedal = CCSprite::create("ui/shop/spin_btn_b1.png");
    ReceiveMedal->setAnchorPoint(ccp(0.0f, 0.0f));
    ReceiveMedal->setPosition(accp(10.0f + 236.0f + 149.0f, 32.0f));
    ReceiveMedal->setTag(2001);
    this->addChild(ReceiveMedal);
    
    CCLabelTTF* ReceiveLabel = CCLabelTTF::create("메달 받기", "HelveticaNeue-Bold", 12);
    ReceiveLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(9.0f + 236.0f + 149.0f + 236.0f/2.0f, 46.0f), ReceiveLabel, 160);
    
    CCSprite* SpinBtn = CCSprite::create("ui/shop/spin_btn1.png");
    SpinBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    SpinBtn->setPosition(accp(24.0f, 83.0f));
    SpinBtn->setTag(2002);
    this->addChild(SpinBtn, 100);
}

void ShopRoulette::InitRouletteMachine()
{
    rouletteMachine1 = new RouletteMachine(this->getContentSize());
    rouletteMachine1->setAnchorPoint(ccp(0.0f, 0.0f));
    rouletteMachine1->setPosition(accp(0.0f, 0.0f));
    this->addChild(rouletteMachine1);
    
    rouletteMachine2 = new RouletteMachine(this->getContentSize());
    rouletteMachine2->setAnchorPoint(ccp(0.0f, 0.0f));
    rouletteMachine2->setPosition(accp(200.0f, 0.0f));
    this->addChild(rouletteMachine2);
    
    rouletteMachine3 = new RouletteMachine(this->getContentSize());
    rouletteMachine3->setAnchorPoint(ccp(0.0f, 0.0f));
    rouletteMachine3->setPosition(accp(400.0f, 0.0f));
    this->addChild(rouletteMachine3);
}

void ShopRoulette::CalRoulette(int& OutMatch, string& OutSymbol, int* OutSlot)
{
    srand((unsigned) time(NULL));
    int num[3];
    num[0] = rand()%100;
    num[1] = rand()%100;
    num[2] = rand()%100;
    
    for(int i=0; i<3; ++i)
    {
        if(0 <= num[i] && 5>num[i])
        {
            // -- 캡콤 카드
            OutSlot[i] = ROULETTE_ICON_CAPCOM;
        }
        else if(5 <= num[i] && 15>num[i])
        {
            // -- 카드 낱개
            OutSlot[i] = ROULETTE_ICON_CARD;
        }
        else if(15 <= num[i] && 30>num[i])
        {
            // -- 레드잼
            OutSlot[i] = ROULETTE_ICON_REDGEM;
        }
        else if(30 <= num[i] && 50>num[i])
        {
            // -- 그린잼
            OutSlot[i] = ROULETTE_ICON_GREENGEM;
        }
        else if(50 <= num[i] && 77>num[i])
        {
            // -- 코인주머니
            OutSlot[i] = ROULETTE_ICON_COIN;
        }
        else
        {
            // -- ko
            OutSlot[i] = ROULETTE_ICON_KO;
        }
    }
    
    CCLOG("%d %d %d", OutSlot[0], OutSlot[1], OutSlot[2]);
    
    int tempSlot[3];
    tempSlot[0] = OutSlot[0];
    tempSlot[1] = OutSlot[1];
    tempSlot[2] = OutSlot[2];
    int key = -1;
    OutMatch = 1;
    
    for(int i=0; i<3; ++i)
    {
        for(int j=0; j<3; ++j)
        {
            if(i == j) continue;
            
            if(tempSlot[i] == tempSlot[j])
            {
                key = tempSlot[i];
                tempSlot[i] = -10;
                ++OutMatch;
            }
        }
    }
    
    CCLOG("key %d", key);
    CCLOG("match %d", OutMatch);
    
    switch (key) {
        case ROULETTE_ICON_CAPCOM:  OutSymbol = "capcom";  break;
        case ROULETTE_ICON_CARD:    OutSymbol = "wing";    break;
        case ROULETTE_ICON_REDGEM:  OutSymbol = "redgem";  break;
        case ROULETTE_ICON_GREENGEM:OutSymbol = "greengem";break;
        case ROULETTE_ICON_COIN:    OutSymbol = "coin";    break;
        case ROULETTE_ICON_KO:      OutSymbol = "ko";      break;
        default:                    OutSymbol = "ko";      break;
    }
    
    CCLOG("%s", OutSymbol.c_str());
}

void ShopRoulette::RunRoulette(ResponseRoulette* _rouletteData)
{
    if(NULL == _rouletteData)
    {
        this->setTouchEnabled(true);
        popupOk("서버와의 연결이 원활하지 않습니다. \n잠시후에 다시 시도해주세요");
        return;
    }
    
    // -- 룰렛 사용 후 메달 갯수 갱신
    this->removeChildByTag(777, true);
    
    --medalCount;
    
    char buff[10];
    sprintf(buff, "%d", medalCount);
    
    CCLabelTTF* medalcountLabel = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
    medalcountLabel->setColor(COLOR_YELLOW);
    medalcountLabel->setTag(777);
    registerLabel(this, ccp(0.0f, 0.0f), accp(354.0f, 635.0f), medalcountLabel, 160);
    
    rouletteMachine1->Repeat(slot[0], 0);
    rouletteMachine2->Repeat(slot[1], 3);
    rouletteMachine3->Repeat(slot[2], 6);
    
    CCDelayTime *delay = CCDelayTime::actionWithDuration(4.0f);
    
    if(0 == _rouletteData->card_id && 0 == _rouletteData->item_id && 0 == _rouletteData->coin)
    {
        // -- 꽝이면 팝업 안띄움
        CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(ShopRoulette::SetTouchEnable));
        this->runAction(CCSequence::actions(delay, callBack1, NULL));
    }
    else
    {
        CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(ShopRoulette::InitPopUp), (void*)_rouletteData);
        this->runAction(CCSequence::actions(delay, callBack, NULL));
    }
}

void ShopRoulette::StartRoulette()
{
    this->setTouchEnabled(false);
    
    int match;
    string symbol;
    
    CalRoulette(match, symbol, slot);
    
    ARequestSender::getInstance()->requestRoulette(symbol, match);
}

void ShopRoulette::InitPopUp(CCNode* sender, void *data)
{
    basePopUp = new RoulettePopUP();
    basePopUp->InitUI(data);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    ShopLayer* parent = (ShopLayer*)this->getParent();
    
    basePopUp->setPosition(accp(0.0f, -parent->getPositionY() * SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 1000);    
}

void ShopRoulette::RemovePopUp()
{
    //this->setTouchEnabled(true);
    
    SetTouchEnable();

    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }
}

void ShopRoulette::SetTouchEnable()
{
    ChangeSpr(this, 2002, "ui/shop/spin_btn1.png", 100);
    
    this->setTouchEnabled(true);
}

void ShopRoulette::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(false == bTouchMove)
    {
        // -- 메달 보내기
        if(GetSpriteTouchCheckByTag(this, 2000, localPoint))
        {
            soundButton1();
            
            ChangeSpr(this, 2000, "ui/shop/spin_btn_a2.png", 100);
        }
        
        // -- 메달 받기
        if(GetSpriteTouchCheckByTag(this, 2001, localPoint))
        {
            soundButton1();
            
            ChangeSpr(this, 2001, "ui/shop/spin_btn_b2.png", 100);
        }
        
        // -- 룰렛
        if(GetSpriteTouchCheckByTag(this, 2002, localPoint))
        {
            soundButton2();
            
            ChangeSpr(this, 2002, "ui/shop/spin_btn2.png", 100);
            
            PlayerInfo* pInfo = PlayerInfo::getInstance();
            
            if(pInfo->myCards->count() >= NUM_OF_MAX_CARD)
            {
                InitPopUp(NULL, NULL);
            }
            else
            {
                if(medalCount > 0)
                {
                    StartRoulette();
                }
                else
                {
                    popupOk("메달이 부족합니다");
                }
            }
            
        }

    }

}

void ShopRoulette::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);    
    CCPoint localPoint = this->convertToNodeSpace(location);

    ChangeSpr(this, 2002, "ui/shop/spin_btn1.png", 100);
    
    if(false == bTouchMove)
    {
        // -- 메달 보내기
        if(GetSpriteTouchCheckByTag(this, 2000, localPoint))
        {
            //soundButton1();
            
            ChangeSpr(this, 2000, "ui/shop/spin_btn_a1.png", 100);
            
            ARequestSender::getInstance()->requestFriendsToGameServer();
        }

        // -- 메달 받기
        if(GetSpriteTouchCheckByTag(this, 2001, localPoint))
        {
            ARequestSender::getInstance()->requestGiftListAsync();
        }

    }
    
    bTouchMove = false;
}

void ShopRoulette::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    bTouchMove = true;
}

RouletteMachine::RouletteMachine(CCSize layerSize)
{
    //this->setClipsToBounds(true);
    this->setContentSize(layerSize);
    
    InitUI();
}

RouletteMachine::~RouletteMachine()
{
    CocosDenshion::SimpleAudioEngine::sharedEngine()->stopEffect(soundID);
    
    this->removeAllChildrenWithCleanup(true);
    
    CC_SAFE_DELETE(aniFrame1);
    CC_SAFE_DELETE(aniFrame2);
    CC_SAFE_DELETE(aniFrame3);
}

void RouletteMachine::InitUI()
{
    IconName[ROULETTE_ICON_REDGEM]      = "ui/shop/spin_icon_red.png";
    IconName[ROULETTE_ICON_GREENGEM]    = "ui/shop/spin_icon_green.png";
    IconName[ROULETTE_ICON_CAPCOM]      = "ui/shop/spin_icon_capcom.png";
    IconName[ROULETTE_ICON_CARD]        = "ui/shop/spin_icon_card.png";
    IconName[ROULETTE_ICON_COIN]        = "ui/shop/spin_icon_coin.png";
    IconName[ROULETTE_ICON_KO]          = "ui/shop/spin_icon_ko.png";
    
    float y = 250.0f;
    
    Ypos1[0] = y;
 
    slot[0] = CCSprite::create(IconName[ROULETTE_ICON_GREENGEM].c_str());
    slot[0]->setAnchorPoint(ccp(0.0f, 0.0f));
    slot[0]->setPosition(accp(33.0f, y));
    slot[0]->setTag(0);
    this->addChild(slot[0], 100);
    
    y+= 108.0f;
    
    Ypos1[1] = y;
    
    slot[1] = CCSprite::create(IconName[ROULETTE_ICON_CAPCOM].c_str());
    slot[1]->setAnchorPoint(ccp(0.0f, 0.0f));
    slot[1]->setPosition(accp(33.0f, y));
    slot[1]->setTag(1);
    this->addChild(slot[1], 100);
    
    y+= 108.0f;
    
    Ypos1[2] = y;
    
    slot[2] = CCSprite::create(IconName[ROULETTE_ICON_CARD].c_str());
    slot[2]->setAnchorPoint(ccp(0.0f, 0.0f));
    slot[2]->setPosition(accp(33.0f, y));
    slot[2]->setTag(2);
    this->addChild(slot[2], 100);
   
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/shop/spin_icon_capcom_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/shop/spin_icon_card_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/shop/spin_icon_coin_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/shop/spin_icon_green_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/shop/spin_icon_ko_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/shop/spin_icon_red_s.png", CCRectMake(0,0,187/SCREEN_ZOOM_RATE,102/SCREEN_ZOOM_RATE));
    
    aniFrame1 = new CCArray();
    aniFrame1->addObject(aa1);
    aniFrame1->addObject(aa2);
    aniFrame1->addObject(aa3);
    aniFrame1->addObject(aa4);
    aniFrame1->addObject(aa5);
    aniFrame1->addObject(aa6);
    
    aniFrame2 = new CCArray();
    aniFrame2->addObject(aa2);
    aniFrame2->addObject(aa3);
    aniFrame2->addObject(aa4);
    aniFrame2->addObject(aa5);
    aniFrame2->addObject(aa6);
    aniFrame2->addObject(aa1);
    
    aniFrame3 = new CCArray();
    aniFrame3->addObject(aa3);
    aniFrame3->addObject(aa4);
    aniFrame3->addObject(aa5);
    aniFrame3->addObject(aa6);
    aniFrame3->addObject(aa1);
    aniFrame3->addObject(aa2);
}

void RouletteMachine::Repeat(int iconIndex, int addRepeat)
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()){
    soundID = CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/spin_in_01.mp3", true);
    }

    this->removeChildByTag(999, true);
    
    this->removeChild(slot[0], true);
    RoulettePlay(slot[0], aniFrame1, accp(33.0f, Ypos1[0]), 0, 500, callfuncND_selector(RouletteMachine::removeSpr), 12 + addRepeat, iconIndex-1);
    
    this->removeChild(slot[1], true);
    RoulettePlay(slot[1], aniFrame2, accp(33.0f, Ypos1[1]), 1, 500, callfuncND_selector(RouletteMachine::removeSpr), 13 + addRepeat, iconIndex);
    
    this->removeChild(slot[2], true);
    RoulettePlay(slot[2], aniFrame3, accp(33.0f, Ypos1[2]), 2, 500, callfuncND_selector(RouletteMachine::removeSpr), 14 + addRepeat, iconIndex+1);
}

void RouletteMachine::RoulettePlay(CCSprite* pSprAni, CCArray *aniFrames, CCPoint pos, int _tag, int _z, SEL_CallFuncND selector, int repeat, int selectedIcon)
{
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrames->objectAtIndex(0);
    
    pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(ccp(0.0f, 0.0f));
	pSprAni->setPosition(pos);
    //pSprAni->setScale(scale);
    pSprAni->setTag(_tag);
	this->addChild(pSprAni,_z);
    
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrames->count();i++)
    {
        animation->addSpriteFrame((CCSpriteFrame*)aniFrames->objectAtIndex(i));
    }
    animation->setLoops(repeat);
    animation->setDelayPerUnit(0.03f);
    
    Roulette *data = new Roulette;
    data->removetag = _tag;
    
    if(selectedIcon < 0) selectedIcon = 5;
    if(selectedIcon > 5) selectedIcon = 0;
    
    data->selectedIcon = selectedIcon;
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, selector, (void*)data);
	pSprAni->runAction(CCSequence::actions(animate, callBack, NULL));
}

void RouletteMachine::removeSpr(CCNode* sender, void* _data)
{
    Roulette* tag = (Roulette*)_data;
    this->removeChildByTag(tag->removetag, true);
    
    slot[tag->removetag] = CCSprite::create(IconName[tag->selectedIcon].c_str());
    slot[tag->removetag]->setAnchorPoint(ccp(0.0f, 0.0f));
    slot[tag->removetag]->setPosition(accp(33.0f, Ypos1[tag->removetag]));
    slot[tag->removetag]->setTag(tag->removetag);
    this->addChild(slot[tag->removetag], 100);
    
    // -- 가운데 슬롯 빨간 테두리
    if(1 == tag->removetag)
    {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->stopEffect(soundID);

        playEffect("audio/spin_out_01.mp3");
        
        CCSprite* pEdge = CCSprite::create("ui/shop/spin_icon_bgeff.png");
        pEdge->setAnchorPoint(ccp(0.0f, 0.0f));
        pEdge->setPosition(accp(27.0f, Ypos1[1] - 6.0f));
        pEdge->setTag(999);
        this->addChild(pEdge, 110);
    }
    
    CC_SAFE_DELETE(tag);
}
