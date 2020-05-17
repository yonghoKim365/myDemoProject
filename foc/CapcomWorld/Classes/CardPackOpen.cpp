//
//  CardPackOpen.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 12. 13..
//
//

#include "CardPackOpen.h"
#include "MainScene.h"
#include "Tutorial.h"

CardPackOpen::CardPackOpen(CCSize layerSize, CCArray* _cardpack, bool _IsTutorial) : cardPack(NULL), cardDetailViewLayer(NULL)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    
    setDisableWithRunningScene();
 
    cardPack = _cardpack;
    showCount = 0;
    IsTutorial = _IsTutorial;
    IsTouchCardPack = false;
    
    InitUI();
}

CardPackOpen::~CardPackOpen()
{
    this->removeAllChildrenWithCleanup(true);
}

void CardPackOpen::InitUI()
{
    CCSprite* blackBG = CCSprite::create("ui/shop/bg_black.png");
    blackBG->setAnchorPoint(ccp(0.0f, 0.0f));
    blackBG->setPosition((accp(0.0f, 0.0f)));
    blackBG->setOpacity(177);
    blackBG->setTag(BLACK_BG);
    this->addChild(blackBG, 100);
    
    CCSize size = GameConst::WIN_SIZE;
    
    if(false == IsTutorial)
    {
        AReceivedCard* r_card = (AReceivedCard*)cardPack->objectAtIndex(showCount);
        CardInfo *card = new CardInfo();
        card->setId(r_card->card_id);
        card->setSrl(r_card->card_srl);
        card->setLevel(r_card->card_lev);
        card->setExp(r_card->card_exp);
        card->setAttack(r_card->card_attack);
        card->setDefence(r_card->card_defense);
        card->setSkillEffect(r_card->card_skill_effect);
        

        CardInfo* cardBP = CardDictionary::sharedCardDictionary()->getInfo(r_card->card_id);
        card->setBp(cardBP->getBp());
        card->setAttribute(cardBP->getAttribute());
        card->setRare(cardBP->getRare());
    
        cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
        cardDetailViewLayer->setScale(0.64f);
        this->addChild(cardDetailViewLayer, 100);

        CC_SAFE_DELETE(card);
    }
    else
    {
        CardInfo *card = (CardInfo*)cardPack->objectAtIndex(showCount);
        cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
        cardDetailViewLayer->setScale(0.64f);
        this->addChild(cardDetailViewLayer,100);

    }

    CCSprite* cardpack = CCSprite::create("ui/card_tab/cardpack/cardpack_01_s.png");
    cardpack->setAnchorPoint(ccp(0.5f, 0.5f));
    cardpack->setPosition((accp(308.0f, 490.0f)));
    cardpack->setScale(2.0f);
    cardpack->setTag(CARDPACK);
    this->addChild(cardpack, 102);

    CCSpriteFrame *aa01 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_00.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa02 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_01.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa03 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_02.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa04 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_03.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa05 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_04.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa06 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_05.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa07 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_06.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa08 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_07.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa09 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_08.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa10 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_09.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa11 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_10.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa12 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_11.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa13 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_12.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa14 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_13.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa15 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_14.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa16 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_15.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa17 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_16.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa18 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_17.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa19 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_18.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa20 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_19.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa21 = CCSpriteFrame::create("ui/card_tab/cardpack/cutline_01_20.png", CCRectMake(0,0,75/SCREEN_ZOOM_RATE,400/SCREEN_ZOOM_RATE));

    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa01);
    aniFrame->addObject(aa02);
    aniFrame->addObject(aa03);
    aniFrame->addObject(aa04);
    aniFrame->addObject(aa05);
    aniFrame->addObject(aa06);
    aniFrame->addObject(aa07);
    aniFrame->addObject(aa08);
    aniFrame->addObject(aa09);
    aniFrame->addObject(aa10);
    aniFrame->addObject(aa11);
    aniFrame->addObject(aa12);
    aniFrame->addObject(aa13);
    aniFrame->addObject(aa14);
    aniFrame->addObject(aa15);
    aniFrame->addObject(aa16);
    aniFrame->addObject(aa17);
    aniFrame->addObject(aa18);
    aniFrame->addObject(aa19);
    aniFrame->addObject(aa20);
    aniFrame->addObject(aa21);
    aniFrame->autorelease();
    
    regAni(aniFrame, this, ccp(0.5f, 0.5f), accp(465.0f, 480.0f), CUT_LINE, 10000, 0.07f, 2.0f);
    
    CCSpriteFrame *bb01 = CCSpriteFrame::create("ui/card_tab/cardpack/touch_01.png", CCRectMake(0,0,175/SCREEN_ZOOM_RATE,62/SCREEN_ZOOM_RATE));
    CCSpriteFrame *bb02 = CCSpriteFrame::create("ui/card_tab/cardpack/touch_02.png", CCRectMake(0,0,175/SCREEN_ZOOM_RATE,62/SCREEN_ZOOM_RATE));

    CCArray *aniFrame2 = new CCArray();
    aniFrame2->addObject(bb01);
    aniFrame2->addObject(bb02);
    aniFrame2->autorelease();

    regAni(aniFrame2, this, ccp(0.5f, 0.5f), accp(408.0f, 884.0f), ANI_TOUCH_ICON, 10000, 0.5f);
}

void CardPackOpen::CardPackOpenAction()
{
    playEffect("audio/card_touch.mp3");
    
    this->removeChildByTag(CARDPACK, true);
    this->removeChildByTag(CUT_LINE, true);
    this->removeChildByTag(ANI_TOUCH_ICON, true);
    
    CCSprite* cardLeft = CCSprite::create("ui/card_tab/cardpack/cardpack_02_s.png");
    cardLeft->setAnchorPoint(ccp(0.5f, 0.5f));
    cardLeft->setPosition((accp(308.0f, 490.0f)));
    cardLeft->setScale(2.0f);
    cardLeft->setTag(CARDPACK_LEFT);
    this->addChild(cardLeft, 102);
    
    CCDelayTime *delay0 = CCDelayTime::actionWithDuration(2.0f);
    CCFiniteTimeAction* actionMove2 = CCMoveTo::actionWithDuration(0.5f, accp(-370.0f, 490.0f));
    cardLeft->runAction(CCSequence::actions(delay0, actionMove2, NULL));

    CCCallFunc* call_Cut1 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::CardPackCut_01));
    this->runAction(call_Cut1);

    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(1.33f);
    CCCallFunc* call_Cut2 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::CardPackCut_02));
    this->runAction(CCSequence::actions(delay1, call_Cut2, NULL));
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.67f);
    CCCallFunc* call_RemoveCut2 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::RemoveCut02));
    this->runAction(CCSequence::actions(delay2, call_RemoveCut2, NULL));
    
    CCDelayTime *delay3 = CCDelayTime::actionWithDuration(2.0f);
    CCCallFunc* call_Fade1 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::FadeWhite01));
    this->runAction(CCSequence::actions(delay3, call_Fade1, NULL));
    
    CCDelayTime *delay4 = CCDelayTime::actionWithDuration(3.33f);
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.0f, 0.85f);
    CCDelayTime *delay5 = CCDelayTime::actionWithDuration(0.14f);
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.14f, 0.80f);
    cardDetailViewLayer->runAction(CCSequence::actions(delay4, actionScale1, delay5, actionScale2, NULL));
    
    CCDelayTime *delay6 = CCDelayTime::actionWithDuration(2.03f);
    CCCallFunc* call_Fade2 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::FadeWhite02));
    this->runAction(CCSequence::actions(delay6, call_Fade2, NULL));
    
    CCDelayTime *delay7 = CCDelayTime::actionWithDuration(2.17f);
    CCCallFunc* call_Fade3 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::FadeWhite03));
    this->runAction(CCSequence::actions(delay7, call_Fade3, NULL));
    
    CCDelayTime *delay8 = CCDelayTime::actionWithDuration(3.83f);
    CCCallFunc* call_Btn = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::CreateBtn));
    this->runAction(CCSequence::actions(delay8, call_Btn, NULL));

    ++showCount;
}

void CardPackOpen::CardPackCut_01()
{
    CCSpriteFrame *aa00 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_00.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa01 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_01.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa02 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_02.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa03 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_03.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa04 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_04.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa05 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_05.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa06 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_06.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa07 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard_07.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa00);
    aniFrame->addObject(aa01);
    aniFrame->addObject(aa02);
    aniFrame->addObject(aa03);
    aniFrame->addObject(aa04);
    aniFrame->addObject(aa05);
    aniFrame->addObject(aa06);
    aniFrame->addObject(aa07);
    aniFrame->autorelease();
    
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrame->objectAtIndex(0);
    
    CCSprite* pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(ccp(0.5f, 0.5f));
	pSprAni->setPosition(accp(510.0f, 490.0f));
    pSprAni->setScale(2.0f);
    pSprAni->setTag(CUT_CARD_1);
	this->addChild(pSprAni,10000);
    
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrame->count();i++){
        animation->addSpriteFrame((CCSpriteFrame*)aniFrame->objectAtIndex(i));
    }
    animation->setLoops(1);
    animation->setDelayPerUnit(0.08f);
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
    pSprAni->runAction( animate );
}

void CardPackOpen::CardPackCut_02()
{
    this->removeChildByTag(CUT_CARD_1, true);
    
    CCSpriteFrame *aa00 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard2_00.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa01 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard2_01.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa02 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard2_02.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa03 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard2_03.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa04 = CCSpriteFrame::create("ui/card_tab/cardpack/cutcard2_04.png", CCRectMake(0,0,130/SCREEN_ZOOM_RATE,390/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa00);
    aniFrame->addObject(aa01);
    aniFrame->addObject(aa02);
    aniFrame->addObject(aa03);
    aniFrame->addObject(aa04);
    aniFrame->autorelease();
    
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrame->objectAtIndex(0);
    
    CCSprite* pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(ccp(0.5f, 0.5f));
	pSprAni->setPosition(accp(510.0f, 490.0f));
    pSprAni->setScale(2.0f);
    pSprAni->setTag(CUT_CARD_2);
	this->addChild(pSprAni,10000);
    
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrame->count();i++){
        animation->addSpriteFrame((CCSpriteFrame*)aniFrame->objectAtIndex(i));
    }
    animation->setLoops(1);
    animation->setDelayPerUnit(0.06f);
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
	pSprAni->runAction( animate );
}

void CardPackOpen::RemoveCut02()
{
    this->removeChildByTag(CUT_CARD_2, true);
}

void CardPackOpen::FadeWhite01()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* whiteBG = CCSprite::create("ui/card_tab/cardpack/card_bgcard_w_s.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setOpacity(0);
    whiteBG->setTag(FADE_01);
    this->addChild(whiteBG, 200);
    
    CCFadeTo* fadein = CCFadeTo::actionWithDuration(0.13f, 255);
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.67f);
    CCFadeTo* fadeout = CCFadeTo::actionWithDuration(0.4f, 0);
    
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::RemoveFade01));
    
    whiteBG->runAction(CCSequence::actions(fadein, delay, fadeout, callWhite, NULL));
}

void CardPackOpen::FadeWhite02()
{
    playEffect("audio/card_show.mp3");
    
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* whiteBG = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_start_01_171.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setOpacity(0);
    whiteBG->setTag(FADE_02);
    this->addChild(whiteBG, 200);
    
    CCFadeTo* fadein = CCFadeTo::actionWithDuration(0.14f, 255);
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.66f);
    CCFadeTo* fadeout = CCFadeTo::actionWithDuration(0.34f, 0);
    
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::RemoveFade02));
    
    whiteBG->runAction(CCSequence::actions(fadein, delay, fadeout, callWhite, NULL));

}

void CardPackOpen::FadeWhite03()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* whiteBG = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_bg_white.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setOpacity(170);
    whiteBG->setTag(FADE_03);
    this->addChild(whiteBG, 200);
    
    CCFadeTo* fadeout = CCFadeTo::actionWithDuration(0.66f, 0);
    
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::RemoveFade03));
    
    whiteBG->runAction(CCSequence::actions(fadeout, callWhite, NULL));
}

void CardPackOpen::FadeWhite04()
{
    playEffect("audio/card_show.mp3");
    
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* whiteBG = CCSprite::create("ui/card_tab/cardpack/card_white.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setScale(0.0f);
    whiteBG->setOpacity(255);
    whiteBG->setTag(FADE_04);
    this->addChild(whiteBG, 200);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 1.6f);
    whiteBG->runAction(CCSequence::actions(actionScale1, NULL));
    
    CCFadeTo* fadeout = CCFadeTo::actionWithDuration(0.33f, 0);
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::RemoveFade04));
    whiteBG->runAction(CCSequence::actions(fadeout, callWhite, NULL));
}


void CardPackOpen::RemoveFade01()
{
    this->removeChildByTag(FADE_01, true);
}

void CardPackOpen::RemoveFade02()
{
    this->removeChildByTag(FADE_02, true);
}

void CardPackOpen::RemoveFade03()
{
    this->removeChildByTag(FADE_03, true);
}

void CardPackOpen::RemoveFade04()
{
    this->removeChildByTag(FADE_04, true);
}


void CardPackOpen::CreateBtn()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* btn = CCSprite::create("ui/shop/popup_btn_c1.png");
    btn->setAnchorPoint(ccp(0.5f, 0.5f));
    btn->setPosition((ccp(size.width/2, accp(60.0f))));
    btn->setTag(BTN);
    this->addChild(btn, 200);
    
    CCLabelTTF* confirm = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue", 13);
    confirm->setColor(COLOR_YELLOW);
    confirm->setTag(LABEL);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, accp(60.0f)), confirm, 201);
}

void CardPackOpen::ShowCard()
{
    if(cardPack)
    {
        CCSize size = GameConst::WIN_SIZE;
        
        if(false == IsTutorial)
        {
            AReceivedCard* r_card = (AReceivedCard*)cardPack->objectAtIndex(showCount);
            
            CardInfo *card = new CardInfo();
            card->setId(r_card->card_id);
            card->setSrl(r_card->card_srl);
            card->setLevel(r_card->card_lev);
            card->setExp(r_card->card_exp);
            card->setAttack(r_card->card_attack);
            card->setDefence(r_card->card_defense);
            card->setSkillEffect(r_card->card_skill_effect);
            
            CardInfo* cardBP = CardDictionary::sharedCardDictionary()->getInfo(r_card->card_id);
            card->setBp(cardBP->getBp());
            card->setAttribute(cardBP->getAttribute());
            card->setRare(cardBP->getRare());

            cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
            cardDetailViewLayer->setScale(0.0f);
            this->addChild(cardDetailViewLayer, 100);
            
            CC_SAFE_DELETE(card);

            CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 0.80f);
            cardDetailViewLayer->runAction(CCSequence::actions(actionScale1, NULL));
            
            CCCallFunc* call_Fade4 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::FadeWhite04));
            this->runAction(call_Fade4);
            
            CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
            CCCallFunc* call_btn = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::CreateBtn));
            this->runAction(CCSequence::actions(delay, call_btn, NULL));
            
        }
        else
        {
            CardInfo *card = (CardInfo*)cardPack->objectAtIndex(showCount);
            cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
            cardDetailViewLayer->setScale(0.0f);
            this->addChild(cardDetailViewLayer,100);
            
            CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 0.80f);
            cardDetailViewLayer->runAction(CCSequence::actions(actionScale1, NULL));
            
            CCCallFunc* call_Fade4 = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::FadeWhite04));
            this->runAction(call_Fade4);
            
            CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
            CCCallFunc* call_btn = CCCallFunc::actionWithTarget(this, callfunc_selector(CardPackOpen::CreateBtn));
            this->runAction(CCSequence::actions(delay, call_btn, NULL));
        }
        
        ++showCount;
    }

}

void CardPackOpen::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void CardPackOpen::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(false == IsTouchCardPack)
    {
        CCLOG("cardpack open action execute");
        
        IsTouchCardPack = true;
        
        CardPackOpenAction();
    }
    
    if(GetSpriteTouchCheckByTag(this, BTN, localPoint))
    {
        //CCLOG("confirm");
        
        soundButton1();
        
        this->removeChild(cardDetailViewLayer, true);
        this->removeChildByTag(BTN, true);
        this->removeChildByTag(LABEL, true);
        
        if(showCount >= cardPack->count())
        {
            restoreTouchDisable();
            MainScene::getInstance()->removeChildByTag(94556, true);
            
            if(false == IsTutorial)
            {
                ARequestSender::getInstance()->requestItemListAsync();
            }
            else
            {
                MainScene::getInstance()->removeChildByTag(98765, true);
                
                const bool TutorialMode = true;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                int State = TUTORIAL_GET_CARD_COMPLETE;
                basePopUp->InitUI(&State);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
            }
        }
        else
            ShowCard();
    }
}

void CardPackOpen::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}
