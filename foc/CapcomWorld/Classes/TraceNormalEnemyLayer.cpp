//
//  TraceNormalEnemyLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 13. 2. 7..
//
//


//#include "TraceNormalEnemyLayer.h"
#include "TraceLayer.h"
#include "TraceResultLayer.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

#include "ShakeAction.h"

TraceNormalEnemyLayer* TraceNormalEnemyLayer::instance = NULL;




TraceNormalEnemyLayer* TraceNormalEnemyLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

TraceNormalEnemyLayer::TraceNormalEnemyLayer(NpcInfo* npcInfo)
{
    instance = this;
    this->npcInfo = npcInfo;
    
    for(int i=0;i<3;i++){
        hitGauge[i] = NULL;
    }
    
    this->setTouchEnabled(true);
    
    bNormalEnenyKo = false;
    
}

TraceNormalEnemyLayer::~TraceNormalEnemyLayer()
{
    
}

void TraceNormalEnemyLayer::InitUI(){
    
}

void TraceNormalEnemyLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (normalBattleStep == 10){
        
        //: Ï¢??Î•?Í∞??????? ?∞Ï?Î•?Ï∂???©Î???
        CCTouch *touch = (CCTouch*)(touches->anyObject());
        CCPoint location = touch->getLocationInView();
        
        //: UI Ï¢??Î•?GLÏ¢??Î°?Î≥?≤Ω?©Î???
        location = CCDirector::sharedDirector()->convertToGL(location);
        
        CCPoint localPoint = this->convertToNodeSpace(location);
        
        battleTouch(location);
    }
}

void TraceNormalEnemyLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
/*
    if (normalBattleStep == 10){
        
        //: Ï¢??Î•?Í∞??????? ?∞Ï?Î•?Ï∂???©Î???
        CCTouch *touch = (CCTouch*)(touches->anyObject());
        CCPoint location = touch->getLocationInView();
        
        //: UI Ï¢??Î•?GLÏ¢??Î°?Î≥?≤Ω?©Î???
        location = CCDirector::sharedDirector()->convertToGL(location);
        
        CCPoint localPoint = this->convertToNodeSpace(location);
        
        battleTouch(location);
    }
 */
}

//questResult->enemy_code
//npcInfo->npcImagePath

void TraceNormalEnemyLayer::InitNormalBattle1(int money)
{
    this->setTouchEnabled(false);
    
    bNormalEnenyKo = false;
    
    normalBattleStep = 0;
    
    requestedMoney = money;
    
    CCSize size = this->getContentSize();
  
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path.append(npcInfo->npcImagePath);
    
    CCSprite* pSprEnemy = CCSprite::create(path.c_str());// npcInfo->npcImagePath);
    CCLog("Enemy Image Address : %s", path.c_str());
    if (pSprEnemy)
    {
        pSprEnemy->setAnchorPoint(ccp(0.5f, 0.0f));
        pSprEnemy->setPosition(ccp(size.width/2.0f-accp(50), 0.0f+accp(85)));
        pSprEnemy->setScale(1.5f);
        pSprEnemy->setTag(200);
        
        this->addChild(pSprEnemy, 1);//-5);
    }
    
    /////////////////////////////////
    //
    CCSpriteFrame* frame0 = CCSpriteFrame::create("ui/quest/eff_c01.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame1 = CCSpriteFrame::create("ui/quest/eff_c02.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame2 = CCSpriteFrame::create("ui/quest/eff_c03.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame3 = CCSpriteFrame::create("ui/quest/eff_c04.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame4 = CCSpriteFrame::create("ui/quest/eff_c05.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame5 = CCSpriteFrame::create("ui/quest/eff_c06.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame6 = CCSpriteFrame::create("ui/quest/eff_c07.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame7 = CCSpriteFrame::create("ui/quest/eff_c08.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame8 = CCSpriteFrame::create("ui/quest/eff_c09.png", CCRectMake(0, 0, 220, 220));
    CCSpriteFrame* frame9 = CCSpriteFrame::create("ui/quest/eff_c10.png", CCRectMake(0, 0, 220, 220));
    
    frameToHit = new CCArray();
    
    frameToHit->addObject(frame0);
    frameToHit->addObject(frame1);
    frameToHit->addObject(frame2);
    frameToHit->addObject(frame3);
    frameToHit->addObject(frame4);
    frameToHit->addObject(frame5);
    frameToHit->addObject(frame6);
    frameToHit->addObject(frame7);
    frameToHit->addObject(frame8);
    frameToHit->addObject(frame9);
    
    
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/quest/trace/trace_btn_blue_a1.png", "ui/quest/trace/trace_btn_blue_a2.png", this, menu_selector(TraceNormalEnemyLayer::normalBattleBtnCallback));
    pSprBtn1->setAnchorPoint( ccp(1,0));
    pSprBtn1->setPosition( ccp(size.width/2-10,0));    // size.width/5 * 0,0));
    pSprBtn1->setTag(40);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/quest/trace/trace_btn_red_a1.png", "ui/quest/trace/trace_btn_red_a2.png", this, menu_selector(TraceNormalEnemyLayer::normalBattleBtnCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( ccp(size.width/2+10,0));    // size.width/5 * 0,0));
    pSprBtn2->setTag(41);
    
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1,pSprBtn2, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( ccp(0, accp(406)));
    pMenu->setTag(400);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("수락", "HelveticaNeue-Bold", 18);
    pLabel1->setColor(COLOR_WHITE);
    pLabel1->setTag(401);
    registerLabel( this,ccp(0.5,0.5), ccp(size.width/2-accp(80), accp(465)), pLabel1, 100);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("결투", "HelveticaNeue-Bold", 19);
    pLabel2->setColor(COLOR_WHITE);
    pLabel2->setTag(402);
    registerLabel( this,ccp(0.5,0.5), ccp(size.width/2+accp(80), accp(465)), pLabel2, 100);
    
    CCSprite* pSprTalkBox = CCSprite::create("ui/quest/trace/trace_talkbox.png");
    pSprTalkBox->setAnchorPoint(ccp(0,0));
    pSprTalkBox->setPosition(accp(20,120));
    pSprTalkBox->setTag(403);
    this->addChild(pSprTalkBox,2);
    
    std::string adjustedText = textAdjust(npcInfo->npcDesc);
    CCLabelTTF* pLabel3 = CCLabelTTF::create(adjustedText.c_str(), "HelveticaNeue-Bold", 15);
    pLabel3->setColor(COLOR_WHITE);
    pLabel3->setTag(404);
    registerLabel( this,ccp(0.5,0.5), ccp(size.width/2, accp(120+60)), pLabel3, 100);
    
    
    
    
    
    // npc name
    string name = npcInfo->npcName;
    CCLabelTTF* pLblRivalInfoAtTalkBox = CCLabelTTF::create(name.append(" ").c_str(), "HelveticaNeue-BoldItalic", 14);
    pLblRivalInfoAtTalkBox->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(90.0f, 245.0f), pLblRivalInfoAtTalkBox, 105);
}

const char* TraceNormalEnemyLayer::textAdjust(const char *input)
{
    std::string text = input;
    do {
        int pos = text.find("\\n");
        if (pos >= text.length())
            return text.c_str();
        text = text.replace(pos, 2, "\n");
    } while (1);
    return text.c_str();
}

void TraceNormalEnemyLayer::InitNormalBattle2(int timeLimit)
{
    CCSize size = this->getContentSize();
    
    normalBattleStep = 10;
    
    battleRemainTime = timeLimit;//npcInfo->battleLimitTime;
    
    this->schedule(schedule_selector(TraceNormalEnemyLayer::battleTimeCounter), 1.0f);
/*
    CCSprite* pSprHitBackground = CCSprite::create("ui/quest/trace/trace_gage_bg.png");
    pSprHitBackground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprHitBackground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS, HIT_GAUGE_BACKGROUND_Y_POS));
    pSprHitBackground->setTag(90);
    this->addChild(pSprHitBackground, 10);
    
    CCSprite* pSprTimeBakcground = CCSprite::create("ui/quest/trace/trace_gage_boss_time.png");
    pSprTimeBakcground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprTimeBakcground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+160.0f, HIT_GAUGE_BACKGROUND_Y_POS-20.0f));
    pSprTimeBakcground->setTag(91);
    this->addChild(pSprTimeBakcground, 9);
    
    
    string strRemainTime = "00:00:";
    char buf[3];
    sprintf(buf,"%d",timeLimit);
    if (timeLimit<10)strRemainTime.append("0").append(buf);
    else strRemainTime.append(buf);
        
    CCLabelTTF* pLabelTime = CCLabelTTF::create(strRemainTime.c_str(), "HelveticaNeue-Bold", 12);
    pLabelTime->setColor(COLOR_WHITE);
    pLabelTime->setTag(193);
    registerLabel( this,accp(0,0.5), ccp(accp(370),size.height-accp(145)), pLabelTime, 100);
*/    
    //////////////////////
    //
    loadHitGauge();
    refreshHitGauge();
    
    //        second = 10;
    //        secDividedBy10 = 0;
    //        secDividedBy100 = 0;
    
    //        this->schedule(schedule_selector(TraceNormalEnemyLayer::cbTimeOut), 1/100);
    //float currentGauge = HIT_GAUGE_LENGTH;//hitGauge[1]->getScaleX();
    float currentGauge = hitGauge[1]->getScaleX();
    float gageRate = currentGauge / HIT_GAUGE_LENGTH;
    float targetSec = gageRate * npcInfo->normalBattleLimitTime * 5;
    //float decVal_per_sec = currentGauge / currentNpc->battleLimitTime;
    
    //CCLog("currentGauge : %f", currentGauge);
    //CCLog("gageRate %f", gageRate);
    //CCLog(" targetSec %f", targetSec);
    
    
    //CCActionInterval* barScale = CCScaleTo::create(10.0f, 0.0f, 1.0f);
    CCActionInterval* barScale = CCScaleTo::create(targetSec, 0.0f, 1.0f);
    CCActionInterval* edgeMove = CCMoveTo::create(targetSec, accp(HIT_GAUGE_BACKGROUND_X_POS+18, HIT_GAUGE_BACKGROUND_Y_POS+6));
    
    hitGauge[1]->runAction(barScale);
    //hitGauge[2]->runAction(edgeMove);
    
    //trace_gage_rival_vs.png
}

void TraceNormalEnemyLayer::battleTimeCounter()
{
    battleRemainTime -= 1.0f;
    
    if (battleRemainTime <= 0){
        this->unschedule(schedule_selector(TraceNormalEnemyLayer::battleTimeCounter));
        battleRemainTime = 0;
        CCLog("time out");
        normalBattleStep = 20;
        
        hitGauge[1]->stopAllActions();
        hitGauge[2]->stopAllActions();
        pSprGaugeSparkle->stopAllActions();
        
        TraceLayer::getInstance()->normalBattleResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 1, PlayerInfo::getInstance()->traceTeam, false);
        actionNormalTimeout();
    }
    
    
    //CCLog("battleRemainTime :%f", battleRemainTime);
    
    this->removeChildByTag(193, true);
    
    int nRemainSec = battleRemainTime;
    string strRemainTime = "00:00:";
    char buf[3];
    sprintf(buf,"%d",nRemainSec);
    if (nRemainSec<10)strRemainTime.append("0").append(buf);
    else strRemainTime.append(buf);
    
    CCLabelTTF* pLabelTime = CCLabelTTF::create(strRemainTime.c_str(), "HelveticaNeue-Bold", 12);
    pLabelTime->setColor(COLOR_WHITE);
    pLabelTime->setTag(193);
    CCSize size = this->getContentSize();
    registerLabel( this,accp(0,0.5), ccp(accp(370),size.height-accp(145)), pLabelTime, 100);
}

void TraceNormalEnemyLayer::battleTouch(CCPoint touchPos)
{
    if (bNormalEnenyKo)return;
    
    //actionHitShake("ui/main_bg/bg_1.png", touchPos);
    actionHitShake(NULL, touchPos);

    
    //////////////////////////
    //
    hitGauge[1]->stopAllActions();
    hitGauge[2]->stopAllActions();
    pSprGaugeSparkle->stopAllActions();
    pSprGaugeSparkle->setScaleX(0.1f);
    pSprGaugeSparkle->setScaleY(0.3f);
    
    float currentGauge = hitGauge[1]->getScaleX();
    float newGauge = currentGauge + npcInfo->touchUp;
    
    if (newGauge >= HIT_GAUGE_LENGTH)
    {
        //CCLog("battleTouch, newGauge >= HIT_GAUGE_LENGTH");
        bNormalEnenyKo = true;
        
        newGauge = HIT_GAUGE_LENGTH;
        hitGauge[1]->setScaleX(HIT_GAUGE_LENGTH);
        hitGauge[2]->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+7 + newGauge, HIT_GAUGE_BACKGROUND_Y_POS+7));
        pSprGaugeSparkle->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+7 + newGauge, HIT_GAUGE_BACKGROUND_Y_POS+7 + 15));
        CCActionInterval* extension = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.5f, pSprGaugeSparkle->getScaleY()*1.5f);
        CCActionInterval* scaledown = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.2f, pSprGaugeSparkle->getScaleY()*1.2f);
        pSprGaugeSparkle->runAction(CCRepeatForever::create((CCActionInterval* )CCSequence::create(extension, scaledown, NULL)));
        
        CCActionInterval* fullGaugeDelay = CCDelayTime::create(0.15f);
        CCCallFunc* fullGaugeCallback = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::cbFullGauge));
        hitGauge[1]->runAction(CCSequence::create(fullGaugeDelay, fullGaugeCallback, NULL));
    }
    else
    {
        hitGauge[1]->setScaleX(newGauge);
        hitGauge[2]->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+7 + newGauge, HIT_GAUGE_BACKGROUND_Y_POS+7));
        pSprGaugeSparkle->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+7 + newGauge, HIT_GAUGE_BACKGROUND_Y_POS+7 + 15));
        CCActionInterval* extension = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.5f, pSprGaugeSparkle->getScaleY()*1.5f);
        CCActionInterval* scaledown = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.2f, pSprGaugeSparkle->getScaleY()*1.2f);
        pSprGaugeSparkle->runAction(CCRepeatForever::create((CCActionInterval* )CCSequence::create(extension, scaledown, NULL)));
        
        float gageRate = currentGauge / HIT_GAUGE_LENGTH;
        if (currentGauge < 1.0f)
            gageRate = 1.0f;

        float targetSec = gageRate * npcInfo->normalBattleLimitTime * 5.0f;
        CCActionInterval* barScale = CCScaleTo::create(targetSec, 0.0f, 1.0f);
        CCActionInterval* edgeMove = CCMoveTo::create(targetSec, accp(HIT_GAUGE_BACKGROUND_X_POS+7, HIT_GAUGE_BACKGROUND_Y_POS+7));
        CCActionInterval* edgeMoveCopy = CCMoveTo::create(targetSec, accp(HIT_GAUGE_BACKGROUND_X_POS+7, HIT_GAUGE_BACKGROUND_Y_POS+7 + 15));
        hitGauge[1]->runAction(barScale);
        hitGauge[2]->runAction(edgeMove);
        pSprGaugeSparkle->runAction(edgeMoveCopy);
    }
    
/*
    //hitGauge[2]->setPosition(accp(28 + newGauge, HIT_GAUGE_BACKGROUND_Y_POS+6));
    
    /////////////////////////
    //
    
    float gageRate = currentGauge / HIT_GAUGE_LENGTH;
    if (currentGauge < 1.0f)
        gageRate = 1.0f;
    
    float targetSec = gageRate * npcInfo->normalBattleLimitTime * 5;
    
    CCActionInterval* barScale = CCScaleTo::create(targetSec, 0.0f, 1.0f);
    
    hitGauge[1]->runAction(barScale);
    
    if (newGauge == HIT_GAUGE_LENGTH){
        
        hitGauge[1]->stopAllActions();
        //hitGauge[2]->stopAllActions();
        normalBattleStep = 30;
        
        TraceLayer::getInstance()->normalBattleResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 1, PlayerInfo::getInstance()->traceTeam, true);
        
        actionNormalKO();
    }
 */
}

void TraceNormalEnemyLayer::cbFullGauge()
{
    hitGauge[1]->stopAllActions();
    normalBattleStep = 30;
    TraceLayer::getInstance()->normalBattleResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 1, PlayerInfo::getInstance()->traceTeam, true);
    
    actionNormalKO();
}

void TraceNormalEnemyLayer::normalBattleBtnCallback(CCObject* pSender)
{
    CCSize size = this->getContentSize();
    
    if (TraceLayer::getInstance()->popupCnt>0)return;
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 40: // accept
        {
            //NpcInfo *npcInfo = getNpc(questResult->enemy_code);
                        
            if (PlayerInfo::getInstance()->getCoint() < requestedMoney){
                // ??Î∂?°±, ???//
                // ??Ï∂©Ï? ???- ????ºÎ? ?¥Î? yes/no
                this->setTouchEnabled(false);
                NoMoneyPopup *popup = new NoMoneyPopup();
                popup->InitUI();
                this->addChild(popup, 5000);
            }
            else{
                // ????™® ??? yes/no
                // yes - server command
                // no - popup close
                this->setTouchEnabled(false);
                MoneySpendNotiPopup *popup = new MoneySpendNotiPopup(requestedMoney);
                this->addChild(popup, 5000);
            }
        }
            break;
        case 41: // battle
        {
            // Í≤∞Ì? ???
            this->removeChildByTag(400, true);
            this->removeChildByTag(401, true);
            this->removeChildByTag(402, true);
            this->removeChildByTag(403, true);
            this->removeChildByTag(404, true);
            
            
            CCSprite* pSprHitBackground = CCSprite::create("ui/quest/trace/trace_gage_bg.png");
            pSprHitBackground->setAnchorPoint(ccp(0.0f, 0.5f));
            pSprHitBackground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS, HIT_GAUGE_BACKGROUND_Y_POS));
            pSprHitBackground->setTag(90);
            this->addChild(pSprHitBackground, 10);
            
            CCSprite* pSprTimeBakcground = CCSprite::create("ui/quest/trace/trace_gage_boss_time.png");
            pSprTimeBakcground->setAnchorPoint(ccp(0.0f, 0.5f));
            pSprTimeBakcground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+160.0f, HIT_GAUGE_BACKGROUND_Y_POS-20.0f));
            pSprTimeBakcground->setTag(91);
            this->addChild(pSprTimeBakcground, 9);
            
            CCSprite* pSprVs = CCSprite::create("ui/quest/trace/trace_gage_rival_vs.png");
            pSprVs->setAnchorPoint(ccp(1.0f, 0.5f));
            pSprVs->setPosition(ccp(size.width - accp(7.5f), accp(HIT_GAUGE_BACKGROUND_Y_POS)));
            pSprVs->setTag(92);
            this->addChild(pSprVs, 10);
            
            
            string strRemainTime = "00:00:";
            char buf[3];
            sprintf(buf,"%d",npcInfo->normalBattleLimitTime);
            if (npcInfo->normalBattleLimitTime<10)strRemainTime.append("0").append(buf);
            else strRemainTime.append(buf);
            
            CCLabelTTF* pLabelTime = CCLabelTTF::create(strRemainTime.c_str(), "HelveticaNeue-Bold", 12);
            pLabelTime->setColor(COLOR_WHITE);
            pLabelTime->setTag(193);
            registerLabel(this,accp(0,0.5), ccp(accp(370),size.height-accp(145)), pLabelTime, 100);
            
            
            this->setTouchEnabled(false);
            TraceLayer::getInstance()->startFight();
        }
            break;
    }
}

void TraceNormalEnemyLayer::loadHitGauge()
{
    char buffer[64];
    for (int scan=0; scan<3; scan++)
    {
        if (scan > 1)
            sprintf(buffer, "ui/quest/trace/trace_gage_yellow2.png");
        else
            sprintf(buffer, "ui/quest/trace/trace_gage_red%d.png", scan+1);
        
        hitGauge[scan] = CCSprite::create(buffer);
        hitGauge[scan]->setAnchorPoint(ccp(0.0f, 0.0f));

        this->addChild(hitGauge[scan], 10);
    }
}

void TraceNormalEnemyLayer::refreshHitGauge()
{
    float ratio = 0.05f;
    //float ratio = 1.0f;//0.0f;
    for (int scan=0; scan<3; scan++)
        hitGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    
    int x = HIT_GAUGE_BACKGROUND_X_POS + 7;
    hitGauge[1]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    hitGauge[1]->setScaleX(0.0f);//ratio * HIT_GAUGE_LENGTH);
    /*
    int x = HIT_GAUGE_BACKGROUND_X_POS+5;
    hitGauge[0]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    x += 13;
    hitGauge[1]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    hitGauge[1]->setScaleX(ratio * HIT_GAUGE_LENGTH);
    x += ratio * HIT_GAUGE_LENGTH;
    hitGauge[2]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
     */
//    hitGauge[2]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+7));
    hitGauge[2]->setScaleX(5.0f);
    hitGauge[2]->setBlendFunc((ccBlendFunc){GL_SRC_ALPHA, GL_SRC_ALPHA});

    
    
    
    
    
    
    
    
    
    
    pSprGaugeSparkle = CCSprite::create("ui/quest/trace/message_exp.png");
    pSprGaugeSparkle->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGaugeSparkle->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+7 + 15));//accp(x, HIT_GAUGE_BACKGROUND_Y_POS+7));
    pSprGaugeSparkle->setScaleX(0.1f);
    pSprGaugeSparkle->setScaleY(0.3f);
    pSprGaugeSparkle->setRotation(90.0f);
    pSprGaugeSparkle->setOpacity(180.0f);
    pSprGaugeSparkle->setTag(93);
    
    CCActionInterval* extension = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.5f, pSprGaugeSparkle->getScaleY()*1.5f);
    CCActionInterval* scaledown = CCScaleTo::create(0.3f, pSprGaugeSparkle->getScaleX()*1.2f, pSprGaugeSparkle->getScaleY()*1.2f);
    pSprGaugeSparkle->runAction(CCRepeatForever::create((CCActionInterval* )CCSequence::create(extension, scaledown, NULL)));
    
    this->addChild(pSprGaugeSparkle, 10);
}

void TraceNormalEnemyLayer::actionHitShake(const char* background, CCPoint location)
{
    playEffect("audio/hit_01.mp3");
    CCSize size = this->getContentSize();
        
//    CCSprite* pSprBackground = CCSprite::create(background);
//    pSprBackground->setAnchorPoint(ccp(0.5f, 0.5f));
//    pSprBackground->setPosition(ccp(size.width/2.0f, size.height/2.0f));
//    pSprBackground->setTag(-7);
//    pSprBackground->setScale(2.5f);    // ??????¬ß???¬¨??????250% ???
    
    CCLayerColor* alertLayer = CCLayerColor::create(ccc4(255.0f, 0.0f, 0.0f, 30.0f), size.width, size.height);
    alertLayer->setAnchorPoint(ccp(0.5f, 0.5f));
    alertLayer->setPosition(ccp(0.0f, 0.0f));
    alertLayer->setTag(87);
    this->addChild(alertLayer, 6);

    
    CCActionInterval* shake =  CCShake::create(0.3f, 28.5f, size.width - accp(100.0f), accp(150.0f));//size.width-28.5f, 28.5f);
    if (size.width == 640){
        shake =  CCShake::create(0.3f, 28.5f, size.width/2-accp(50), accp(85));//size.width-28.5f, 28.5f);
    }
    
    this->getChildByTag(200)->runAction(CCSequence::create(shake, NULL));
    
//    this->addChild(pSprBackground, -5);
    
    // ??¬¨?????????¬¨¬©????
    CCSpriteFrame* frame = (CCSpriteFrame* )frameToHit->objectAtIndex(0);
    spriteToHit = CCSprite::createWithSpriteFrame(frame);
    spriteToHit->setAnchorPoint(ccp(0.5f, 0.5f));
    

    spriteToHit->setPosition(accp(location.x*SCREEN_ZOOM_RATE+210.0f, location.y*SCREEN_ZOOM_RATE-280.0f));
    if (size.width == 640){
        spriteToHit->setPosition(accp(location.x*SCREEN_ZOOM_RATE, location.y*SCREEN_ZOOM_RATE));
    }
    
    spriteToHit->setScale(2.0f);
    spriteToHit->setTag(88);
    
    //    CCLog("x : %f, y : %f", location.x, location.y);
    
    CCAnimation* animationToHit = CCAnimation::create();
    for (int i=0; i<frameToHit->count(); i++)
    {
        animationToHit->addSpriteFrame((CCSpriteFrame* )frameToHit->objectAtIndex(i));
    }
    animationToHit->setLoops(1);
    animationToHit->setDelayPerUnit(0.03f);
    
    CCAnimate* animate = CCAnimate::create(animationToHit);
    CCCallFunc* cbRemoveHit = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::removeHit));
    
    spriteToHit->runAction(CCSequence::create(animate, cbRemoveHit, NULL));
    
    this->addChild(spriteToHit, 6);//-3);
}
/*
void TraceNormalEnemyLayer::removeShake()
{
    this->removeChildByTag(-7, true);
}
*/
void TraceNormalEnemyLayer::removeHit()
{
    this->removeChildByTag(87, true);
    this->removeChildByTag(88, true);
}


void TraceNormalEnemyLayer::actionNormalKO(){
    
    this->unschedule(schedule_selector(TraceNormalEnemyLayer::battleTimeCounter));
    
    CCSize size = this->getContentSize();
/*
    std::string text_sign1 = "YOU WIN";
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 20);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_ORANGE);
    pLblSp->setScale(0.0f);
*/
    CCSprite* pSprYouWin = CCSprite::create("ui/battle/battle_duel_result_win.png");
    pSprYouWin->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprYouWin->setPosition(ccp(size.width/2.0f, size.height/2.0f));// + accp(300.0f)));
    pSprYouWin->setOpacity(0.0f);
    
    CCCallFunc* cbStartKo = CCCallFunc::create(this, callfunc_selector(TraceLayer::startKo));
    CCActionInterval* startKoDelay = CCDelayTime::create(98.0f / 30.0f);
    CCActionInterval* spShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* spDelay = CCDelayTime::create(3.0f);
    CCActionInterval* spHide = CCFadeTo::create(0.0f, 0.0f);
    CCCallFunc* cbNormalKoResult = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::callBackNormalKO));
    CCCallFunc* cbRemoveNormalKo = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::callBackRemoveKo));
    pSprYouWin->runAction(CCSequence::create(CCSpawn::create(cbStartKo, startKoDelay, NULL), spShow, spDelay, spHide, cbNormalKoResult, cbRemoveNormalKo, NULL));
//    pLblSp->runAction(CCSequence::create(spExtension, spDelay, CCSpawn::create(spDrop, spFadeOut, NULL), cb, NULL));
    
    this->addChild(pSprYouWin, 5);
//    registerLabel(this, ccp(0.5f, 0.5f), accp(size.width, size.height-20-20), pLblSp, 5);
}

void TraceNormalEnemyLayer::callBackNormalKO()
{
    TraceLayer::getInstance()->setTraceResult(TraceLayer::getInstance()->normalBattleResult);
}

void TraceNormalEnemyLayer::callBackRemoveKo()
{
    this->removeAllChildrenWithCleanup(true);    
}

void TraceNormalEnemyLayer::actionNormalTimeout(){
    
    CCSize size = this->getContentSize();
    
    this->setTouchEnabled(false);
/*
    std::string text_sign1 = "TIME OUT";
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 20);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_ORANGE);
    pLblSp->setScale(0.2f);
 */
    CCLayerColor* curtain = CCLayerColor::create(ccc4(0.0f, 0.0f, 0.0f, 0.0f), size.width, size.height);
    curtain->setAnchorPoint(ccp(0.5f, 0.5f));
    curtain->setPosition(ccp(0.0f, 0.0f));

    CCActionInterval* curtainFadeIn = CCFadeTo::create(0.5f, 70.0f);
    curtain->runAction(curtainFadeIn);
    
    this->addChild(curtain, 4);
    
    CCSprite* pSprTimeOut = CCSprite::create("ui/quest/trace/quest_ko/quest_ilban_you_time_out.png");
    pSprTimeOut->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTimeOut->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprTimeOut->setScaleX(0.0f);
    
    CCActionInterval* spExtension = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay = CCDelayTime::create(3.0f);
    CCActionInterval* spFoldTitle = CCScaleTo::create(0.3f, 0.0f, 1.0f);
    CCCallFunc* cbNormalTimeOutResult = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::callBackNormalTimeout));
//    CCCallFunc* cbRemoveNormalTimeOut = CCCallFunc::create(this, callfunc_selector(TraceNormalEnemyLayer::callBackRemoveTimeOut));
    
    pSprTimeOut->runAction(CCSequence::create(spExtension, spDelay, spFoldTitle, cbNormalTimeOutResult, NULL));
    
    this->addChild(pSprTimeOut, 5);
    
//    registerLabel(this, ccp(0.5f, 0.5f), accp(size.width, size.height-20-20), pLblSp, 5);
}

void TraceNormalEnemyLayer::callBackNormalTimeout()
{
    this->setTouchEnabled(false);
    TraceFailPopup* popup = new TraceFailPopup();
    popup->initUI();
    this->addChild(popup, 5000);
}

void TraceNormalEnemyLayer::callBackRemoveTimeOut()
{
    this->removeAllChildrenWithCleanup(true);
    TraceLayer::getInstance()->setChaseStep(CHASE_TAB);
}
