//
//  TraceResultLayer.cpp
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 19..
//
//

#include "TraceResultLayer.h"
#include "TraceLayer.h"

void TraceResultLayer::startFight()
{
    CCSize size = this->getContentSize();
    
    /////////
    //
    // CLICK
    
    // Click Background Image
    CCSprite* pSprClickBG = CCSprite::create("ui/quest/trace/click_bg.png");
    pSprClickBG->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprClickBG->setPosition(ccp(size.width/2.0f, size.height/2.0f - accp(180.0f)));
    pSprClickBG->setTag(5);
    this->addChild(pSprClickBG, 5);
    
    // Click Title
    CCLabelTTF* pLblClickTitle = CCLabelTTF::create("화면을 연타하세요!!", "HelveticaNeue-BoldItalic", 15);
    pLblClickTitle->setAnchorPoint(ccp(0.5f, 0.5f));
    pLblClickTitle->setPosition(ccp(size.width/2.0f, size.height/2.0f - accp(180.0f)));
    pLblClickTitle->setColor(COLOR_RED);
    pLblClickTitle->setTag(6);
    this->addChild(pLblClickTitle, 6);
    
    // Click Animation
    CCSpriteFrame *click1 = CCSpriteFrame::create("ui/tutorial/tutorial_preview_click01.png", CCRectMake(0,0,100/SCREEN_ZOOM_RATE,100/SCREEN_ZOOM_RATE));
    CCSpriteFrame *click2 = CCSpriteFrame::create("ui/tutorial/tutorial_preview_click02.png", CCRectMake(0,0,100/SCREEN_ZOOM_RATE,100/SCREEN_ZOOM_RATE));
    
    aniFrames = new CCArray();
    aniFrames->addObject(click1);
    aniFrames->addObject(click2);
    aniFrames->autorelease();
    
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrames->objectAtIndex(0);
    CCSprite* pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(ccp(0.5f, 0.0f));
	pSprAni->setPosition(ccp(size.width/2.0f + accp(120.0f), size.height/2.0f - accp(180.0f) - accp(30.0f)));
    pSprAni->setRotation(90.0f);
    pSprAni->setTag(7);
	this->addChild(pSprAni, 10000);
    
    CCAnimation* animation = CCAnimation::create();
    
    for(int i=0;i<aniFrames->count();i++){
        animation->addSpriteFrame((CCSpriteFrame*)aniFrames->objectAtIndex(i));
    }
    animation->setLoops(-1);
    animation->setDelayPerUnit(0.3f);
    
    CCAnimate* animate = CCAnimate::create(animation);
    CCRepeatForever* repeat = CCRepeatForever::create(animate);
	pSprAni->runAction(repeat);
    
    ////////////
    //
    // GET READY
    
    CCSprite* pSprGetReadyBG = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_ready_02.png");
    pSprGetReadyBG->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGetReadyBG->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprGetReadyBG->setOpacity(255.0f);
    pSprGetReadyBG->setScale(0.5f);
    pSprGetReadyBG->setTag(10);
    this->addChild(pSprGetReadyBG, 7);
    
    CCCallFunc* cbPlayReady = CCCallFunc::create(this, callfunc_selector(MyUtil::soundReady));
    CCActionInterval* readyBgFadeOut = CCFadeTo::create(12.0f / 30.0f, 0.0f);
    CCActionInterval* readyBgScaleUp = CCScaleTo::create(4.0f / 30.0f, 1.0f);
    pSprGetReadyBG->runAction(CCSpawn::create(cbPlayReady, readyBgFadeOut, readyBgScaleUp, NULL));
    
    
    CCSprite* pSprGetReady = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_ready_01.png");
    pSprGetReady->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGetReady->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprGetReady->setScale(0.5f);
    pSprGetReady->setTag(11);
    this->addChild(pSprGetReady, 6);
    
    CCActionInterval* readyScaleUp = CCScaleTo::create(4.0 / 30.0f, 1.0f);
    CCActionInterval* readyDelay = CCDelayTime::create((40.0f - 4.0f) / 30.0f);
    CCActionInterval* readyFadeOut = CCFadeTo::create((77.0f - 40.0f) / 30.0f, 0.0f);
    pSprGetReady->runAction(CCSequence::create(readyScaleUp, readyDelay, readyFadeOut, NULL));
    
    
    CCSprite* pSprGetReadyRing = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_ready_03.png");
    pSprGetReadyRing->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGetReadyRing->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprGetReadyRing->setScale(0.5f);
    pSprGetReadyRing->setTag(12);
    this->addChild(pSprGetReadyRing, 5);
    
    CCActionInterval* ringScaleUp = CCScaleTo::create(4.0f / 30.0f, 1.0f);
    CCActionInterval* ringDelay = CCDelayTime::create((40.0f - 4.0f) / 30.0f);
    CCActionInterval* ringFadeOut = CCFadeTo::create((77.0f - 40.0f) / 30.0f, 0.0f);
    pSprGetReadyRing->runAction(CCSequence::create(ringScaleUp, ringDelay, ringFadeOut, NULL));
    
    
    CCSprite* pSprFightFog = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_fight_03.png");
    pSprFightFog->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprFightFog->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprFightFog->setOpacity(0.0f);
    pSprFightFog->setTag(13);
    this->addChild(pSprFightFog, 8);
    
    CCActionInterval* fogWait = CCDelayTime::create(43.0f / 30.0f);
    CCActionInterval* fogFadeIn = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* fogScaleUp = CCScaleTo::create((45.0f - 43.0f)*10.0f / 30.0f, 5.0f);
    CCActionInterval* fogFadeOut = CCFadeTo::create((45.0f - 43.0f)*10.0f / 30.0f, 0.0f);
    pSprFightFog->runAction(CCSequence::create(fogWait, fogFadeIn, CCSpawn::create(fogScaleUp, fogFadeOut, NULL), NULL));
    
    
    CCSprite* pSprFightBG = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_fight_02.png");
    pSprFightBG->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprFightBG->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprFightBG->setOpacity(0.0f);
    pSprFightBG->setTag(14);
    this->addChild(pSprFightBG, 7);
    
    CCActionInterval* fightBgWait = CCDelayTime::create(45.0f / 30.0f);
    CCActionInterval* fightBgFadeIn = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* fightBgFadeOut = CCFadeTo::create((57.0f - 45.0f) / 30.0f, 0.0f);
    pSprFightBG->runAction(CCSequence::create(fightBgWait, fightBgFadeIn, fightBgFadeOut, NULL));
    
    
    CCSprite* pSprFight = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_fight_01.png");
    pSprFight->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprFight->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    pSprFight->setOpacity(0.0f);
    pSprFight->setScale(6.0f);
    pSprFight->setTag(15);
    this->addChild(pSprFight, 6);
    
    CCActionInterval* fightWait = CCDelayTime::create(40.0f / 30.0f);
    CCActionInterval* fightFadeIn = CCFadeTo::create(0.0f, 255.0f);
    CCCallFunc* cbPlayStart = CCCallFunc::create(this, callfunc_selector(MyUtil::soundGo));
    CCActionInterval* fightScaleDown = CCScaleTo::create((43.0f - 40.0f) / 30.0f, 0.95f);
    CCActionInterval* fightScaleUp = CCScaleTo::create((45.0f - 43.0f) / 30.0f, 1.0f);
    CCActionInterval* fightDelay = CCDelayTime::create((74.0f - 45.0f) / 30.0f);
    CCActionInterval* fightStretch = CCScaleBy::create((77.0f - 74.0f) / 30.0f, 10.0f, 0.0f);
    CCCallFunc* callBackStartFight = CCCallFunc::create(this, callfunc_selector(TraceResultLayer::cbStartFight));
    pSprFight->runAction(CCSequence::create(fightWait, fightFadeIn, CCSpawn::create(fightScaleDown, cbPlayStart, NULL), fightScaleUp, fightDelay, fightStretch, callBackStartFight, NULL));
    
    
    ////////////
    //
    // SPOTLIGHT
    
    CCSpriteFrame* spotlightFrame0 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame1 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame2 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame3 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame4 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame5 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame6 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0, 0, accp(128), accp(128)));
    
    frameToSpotlight = new CCArray();
    frameToSpotlight->addObject(spotlightFrame0);
    frameToSpotlight->addObject(spotlightFrame1);
    frameToSpotlight->addObject(spotlightFrame2);
    frameToSpotlight->addObject(spotlightFrame3);
    frameToSpotlight->addObject(spotlightFrame4);
    frameToSpotlight->addObject(spotlightFrame5);
    frameToSpotlight->addObject(spotlightFrame6);
    
    CCSpriteFrame* spotlightFrame = (CCSpriteFrame* )frameToSpotlight->objectAtIndex(0);
    spriteToSpotlight = CCSprite::createWithSpriteFrame(spotlightFrame);
    spriteToSpotlight->setAnchorPoint(ccp(0.5f, 0.5f));
    spriteToSpotlight->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(60.0f)));
    spriteToSpotlight->setScale(4.0f);
    spriteToSpotlight->setOpacity(0.0f);
    spriteToSpotlight->setTag(16);
    
    CCAnimation* animationToSpotlight = CCAnimation::create();
    for (int i=0; i<frameToSpotlight->count(); i++)
    {
        animationToSpotlight->addSpriteFrame((CCSpriteFrame* )frameToSpotlight->objectAtIndex(i));
    }
    animationToSpotlight->setLoops(1);
    animationToSpotlight->setDelayPerUnit((50.0f - 43.0f) / 30.0f / 5.0f);
    
    CCAnimate* animateToSpotlight = CCAnimate::create(animationToSpotlight);
    CCActionInterval* spotlightDelay = CCDelayTime::create(43.0f / 30.0f);
    CCActionInterval* spotlightShow = CCFadeTo::create(0.0f, 150.0f);
    CCActionInterval* spotlightHide = CCFadeTo::create(0.0f, 0.0f);
    spriteToSpotlight->runAction(CCSequence::create(spotlightDelay, spotlightShow, animateToSpotlight, spotlightHide, NULL));
    
    this->addChild(spriteToSpotlight, 5);
}

void TraceResultLayer::cbStartFight()
{
    this->removeChildByTag(5, true);
    this->removeChildByTag(6, true);
    this->removeChildByTag(7, true);
    
    for (int i=10; i<17; i++)
    {
        this->removeChildByTag(i, true);
    }
    
    TraceNormalEnemyLayer::getInstance()->setTouchEnabled(true);
    TraceNormalEnemyLayer::getInstance()->InitNormalBattle2(TraceNormalEnemyLayer::getInstance()->npcInfo->normalBattleLimitTime);
    
//    removeFromParentAndCleanup(true);
//    this->startKo();
}

void TraceResultLayer::startKo()
{
    CCSize size = this->getContentSize();
    
    
    CCSpriteFrame* fadeOutFrame0 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_170.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame1 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_171.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame2 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_172.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame3 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_173.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame4 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_174.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame5 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_175.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame6 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_176.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame7 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_177.png", CCRectMake(0, 0, accp(640), accp(800)));
    CCSpriteFrame* fadeOutFrame8 = CCSpriteFrame::create("ui/quest/trace/quest_ko/quest_ko_start_01_178.png", CCRectMake(0, 0, accp(640), accp(800)));
    
    frameToFadeOut = new CCArray();
    frameToFadeOut->addObject(fadeOutFrame0);
    frameToFadeOut->addObject(fadeOutFrame1);
    frameToFadeOut->addObject(fadeOutFrame2);
    frameToFadeOut->addObject(fadeOutFrame3);
    frameToFadeOut->addObject(fadeOutFrame4);
    frameToFadeOut->addObject(fadeOutFrame5);
    frameToFadeOut->addObject(fadeOutFrame6);
    frameToFadeOut->addObject(fadeOutFrame7);
    frameToFadeOut->addObject(fadeOutFrame8);
    
    CCSpriteFrame* fadeOutFrame = (CCSpriteFrame* )frameToFadeOut->objectAtIndex(0);
    spriteToFadeOut = CCSprite::createWithSpriteFrame(fadeOutFrame);
    spriteToFadeOut->setAnchorPoint(ccp(0.5f, 0.5f));
    spriteToFadeOut->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    spriteToFadeOut->setTag(10);
    
    CCAnimation* animationToFadeOut = CCAnimation::create();
    for (int i=0; i<frameToFadeOut->count(); i++)
    {
        animationToFadeOut->addSpriteFrame((CCSpriteFrame* )frameToFadeOut->objectAtIndex(i));
    }
    animationToFadeOut->setLoops(1);
    animationToFadeOut->setDelayPerUnit((9.0f - 0.0f) / 30.0f / 5.0f);
    
    CCAnimate* animateToFadeOut = CCAnimate::create(animationToFadeOut);
    CCActionInterval* fadeOutHide = CCFadeTo::create(0.0f, 0.0f);
    spriteToFadeOut->runAction(CCSequence::create(animateToFadeOut, fadeOutHide, NULL));
    
    this->addChild(spriteToFadeOut, 5);
    
    
    CCSprite* pSprWhiteCurtain = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_bg_white.png");
    pSprWhiteCurtain->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprWhiteCurtain->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprWhiteCurtain->setOpacity(0.0f);
    pSprWhiteCurtain->setTag(11);
    
    CCActionInterval* whiteCurtainDelay1 = CCDelayTime::create((4.0f - 0.0f) / 30.0f);
    CCActionInterval* whiteCurtainFadeIn = CCFadeTo::create((10.0f - 4.0f) / 30.0f, 255.0f);
    CCActionInterval* whiteCurtainDelay2 = CCDelayTime::create((82.0f - 10.0f) / 30.0f);
    CCActionInterval* whiteCurtainFadeOut = CCFadeTo::create((98.0f - 82.0f) / 30.0f, 0.0f);
    CCCallFunc* callBackRemoveKo = CCCallFunc::create(this, callfunc_selector(TraceResultLayer::cbRemoveKo));
    pSprWhiteCurtain->runAction(CCSequence::create(whiteCurtainDelay1, whiteCurtainFadeIn, whiteCurtainDelay2, whiteCurtainFadeOut, callBackRemoveKo, NULL));
    
    this->addChild(pSprWhiteCurtain, 6);
    
    
    CCSprite* pSprKo = CCSprite::create("ui/quest/trace/quest_ko/quest_ko.png");
    pSprKo->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprKo->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprKo->setOpacity(0.0f);
    pSprKo->setScale(5.0f);
    pSprKo->setTag(12);
    
    CCActionInterval* koDelay1 = CCDelayTime::create((26.0f - 0.0f) / 30.0f);
    CCActionInterval* koShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* koScaleDown = CCScaleTo::create((29.0f - 26.0f) / 30.0f, 0.95f * 0.75f);
    CCCallFunc* cbPlayGo = CCCallFunc::create(this, callfunc_selector(MyUtil::soundGo));
    CCCallFunc* cbPlayKo = CCCallFunc::create(this, callfunc_selector(MyUtil::soundKo));
    CCActionInterval* koScaleUp = CCScaleTo::create((31.0f - 29.0f) / 30.0f, 1.0f * 0.75f);
    CCActionInterval* koDelay2 = CCDelayTime::create((81.0f - 31.0f) / 30.0f);
    CCActionInterval* koStretch = CCScaleTo::create((84.0f - 81.0f) / 30.0f, 10.0f, 0.0f);
    pSprKo->runAction(CCSequence::create(koDelay1, koShow, koScaleDown, CCSpawn::create(cbPlayGo, cbPlayKo, koScaleUp, NULL), koDelay2, koStretch, NULL));
    
    this->addChild(pSprKo, 9);
    
    
    CCSprite* pSprKoBG = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_bg_01.png");
    pSprKoBG->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprKoBG->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprKoBG->setOpacity(0.0f);
    pSprKoBG->setTag(13);
    
    CCActionInterval* KoBgDelay1 = CCDelayTime::create((28.0f - 0.0f) / 30.0f);
    CCActionInterval* KoBgFadeIn = CCFadeTo::create((62.0f - 28.0f) / 30.0f, 255.0f);
    CCActionInterval* KoBgDelay2 = CCDelayTime::create((71.0f - 62.0f) / 30.0f);
    CCActionInterval* KoBgFadeOut = CCFadeTo::create((86.0f - 71.0f) / 30.0f, 0.0f);
    pSprKoBG->runAction(CCSequence::create(KoBgDelay1, KoBgFadeIn, KoBgDelay2, KoBgFadeOut, NULL));
    
    this->addChild(pSprKoBG, 7);
    
    
    CCSpriteFrame* spotlightFrame0 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame1 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame2 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame3 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame4 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame5 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0, 0, accp(128), accp(128)));
    CCSpriteFrame* spotlightFrame6 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0, 0, accp(128), accp(128)));
    
    frameToSpotlight = new CCArray();
    frameToSpotlight->addObject(spotlightFrame0);
    frameToSpotlight->addObject(spotlightFrame1);
    frameToSpotlight->addObject(spotlightFrame2);
    frameToSpotlight->addObject(spotlightFrame3);
    frameToSpotlight->addObject(spotlightFrame4);
    frameToSpotlight->addObject(spotlightFrame5);
    frameToSpotlight->addObject(spotlightFrame6);
    
    CCSpriteFrame* spotlightFrame = (CCSpriteFrame* )frameToSpotlight->objectAtIndex(0);
    spriteToSpotlight = CCSprite::createWithSpriteFrame(spotlightFrame);
    spriteToSpotlight->setAnchorPoint(ccp(0.5f, 0.5f));
    spriteToSpotlight->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    spriteToSpotlight->setScale(4.0f);
    spriteToSpotlight->setOpacity(0.0f);
    spriteToSpotlight->setTag(14);
    
    CCAnimation* animationToSpotlight = CCAnimation::create();
    for (int i=0; i<frameToSpotlight->count(); i++)
    {
        animationToSpotlight->addSpriteFrame((CCSpriteFrame* )frameToSpotlight->objectAtIndex(i));
    }
    animationToSpotlight->setLoops(1);
    animationToSpotlight->setDelayPerUnit((36.0f - 29.0f) / 30.0f / 5.0f);
    
    CCAnimate* animateToSpotlight = CCAnimate::create(animationToSpotlight);
    CCActionInterval* spotlightDelay = CCDelayTime::create((29.0f - 0.0f) / 30.0f);
    CCActionInterval* spotlightShow = CCFadeTo::create(0.0f, 150.0f);
    CCActionInterval* spotlightHide = CCFadeTo::create(0.0f, 0.0f);
    spriteToSpotlight->runAction(CCSequence::create(spotlightDelay, spotlightShow, animateToSpotlight, spotlightHide, NULL));
    
    this->addChild(spriteToSpotlight, 8);
    
    
    CCSprite* pSprKoFog = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_02.png");
    pSprKoFog->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprKoFog->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprKoFog->setOpacity(0.0f);
    pSprKoFog->setScale(1.0f * 0.75f);
    pSprKoFog->setTag(15);
    
    CCActionInterval* koFogDelay = CCDelayTime::create((29.0f - 0.0f) / 30.0f);
    CCActionInterval* koFogShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* koFogScaleUp = CCScaleTo::create((59.0f - 29.0f) / 15.0f, 5.0f);
    CCActionInterval* koFogFadeOut = CCFadeTo::create((59.0f - 29.0f) / 15.0f, 0.0f);
    pSprKoFog->runAction(CCSequence::create(koFogDelay, koFogShow, CCSpawn::create(koFogScaleUp, koFogFadeOut, NULL), koFogFadeOut, NULL));
    
    this->addChild(pSprKoFog, 10);
    
    
    CCSprite* pSprWhiteKo = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_01.png");
    pSprWhiteKo->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprWhiteKo->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprWhiteKo->setOpacity(0.0f);
    pSprWhiteKo->setScale(1.0f * 0.75f);
    pSprWhiteKo->setTag(16);
    
    CCActionInterval* whiteKoDelay = CCDelayTime::create((31.0f - 0.0f) / 30.0f);
    CCActionInterval* whiteKoShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* whiteKoFadeOut = CCFadeTo::create((62.0f - 31.0f) / 30.0f, 0.0f);
    pSprWhiteKo->runAction(CCSequence::create(whiteKoDelay, whiteKoShow, whiteKoFadeOut, NULL));
    
    this->addChild(pSprWhiteKo, 10);
}

void TraceResultLayer::cbRemoveKo()
{
    for (int i=10; i<17; i++)
    {
        this->removeChildByTag(i, true);
    }
    
    //    TraceNormalEnemyLayer::getInstance()->setTouchEnabled(true);
    //    TraceNormalEnemyLayer::getInstance()->InitNormalBattle2(TraceNormalEnemyLayer::getInstance()->npcInfo->normalBattleLimitTime);
}