//
//  TraceBossLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 13. 2. 8..
//
//

#include "TraceLayer.h"
#include "ShakeAction.h"
#include "BattleFullScreen.h"

/*
const float XP_GAUGE_LENGTH2     = 513.0f;
const float HIT_GAUGE_LENGTH    = XP_GAUGE_LENGTH2 - 38.0f;
const float HIT_GAUGE_BACKGROUND_X_POS = 10.0f;
const float HIT_GAUGE_BACKGROUND_Y_POS = 840;//780.0f;
*/

TraceBossLayer* TraceBossLayer::instance = NULL;

TraceBossLayer* TraceBossLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

TraceBossLayer::TraceBossLayer(NpcInfo* npcInfo, int _questID, int _remainHp, int _maxHp, int _bossLv)
{
    this->npcInfo = npcInfo;
    this->setTouchEnabled(true);
    this->questID = _questID;
    //_remainHp
    BOSS_HP_GAUGE_LENGTH = 475.0f;
    instance = this;
    curHp = _remainHp;
    maxHp = _maxHp;
    bossLevel = _bossLv;
}

TraceBossLayer::~TraceBossLayer()
{
    removeAllChildrenWithCleanup(true);
    
}

void TraceBossLayer::InitUI()
{
    
    
    CCSize size = this->getContentSize();

    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path.append(npcInfo->npcImagePath);
    
    CCSprite* pSprEnemy = CCSprite::create(path.c_str());// npcInfo->npcImagePath);
    if (pSprEnemy)
    {
        pSprEnemy->setAnchorPoint(ccp(0.5f, 0.0f));
        pSprEnemy->setPosition(ccp(size.width/2.0f-accp(50), 0.0f+accp(85)));
        pSprEnemy->setScale(1.5f);
        pSprEnemy->setTag(200);
        
        this->addChild(pSprEnemy, 1);//-5);
    }
        
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/quest/trace/trace_btn_blue_a1.png", "ui/quest/trace/trace_btn_blue_a2.png", this, menu_selector(TraceBossLayer::BtnCallback));
    pSprBtn1->setAnchorPoint( ccp(1,0));
    pSprBtn1->setPosition( ccp(size.width/2-10,0));    // size.width/5 * 0,0));
    pSprBtn1->setTag(40);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/quest/trace/trace_btn_red_a1.png", "ui/quest/trace/trace_btn_red_a2.png", this, menu_selector(TraceBossLayer::BtnCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( ccp(size.width/2+10,0));    // size.width/5 * 0,0));
    pSprBtn2->setTag(41);
    
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1,pSprBtn2, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( ccp(0, accp(406)));
    pMenu->setTag(400);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("이전", "HelveticaNeue-Bold", 18);
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
    
    std::string npcDesc = textAdjust(npcInfo->npcDesc);
    CCLabelTTF* pLabel3 = CCLabelTTF::create(npcDesc.c_str(), "HelveticaNeue-Bold", 15);
    //CCLabelTTF* pLabel3 = CCLabelTTF::create(npcInfo->npcDesc, "HelveticaNeue-Bold", 15);
    pLabel3->setColor(COLOR_WHITE);
    pLabel3->setTag(404);
    registerLabel( this,ccp(0.5,0.5), ccp(size.width/2, accp(120+60)), pLabel3, 100);
    
    CCSprite* pSprHitBackground = CCSprite::create("ui/quest/trace/trace_gage_bg.png");
    pSprHitBackground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprHitBackground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS, HIT_GAUGE_BACKGROUND_Y_POS));
    pSprHitBackground->setTag(90);
    this->addChild(pSprHitBackground, 10);
    
    CCSprite* pSprVs = CCSprite::create("ui/quest/trace/trace_gage_rival_vs.png");
    pSprVs->setAnchorPoint(ccp(1.0f, 0.5f));
    pSprVs->setPosition(ccp(size.width - accp(7.5f), accp(HIT_GAUGE_BACKGROUND_Y_POS)));
    this->addChild(pSprVs,10);
    
    // npc name
    string name = npcInfo->npcName;
    CCLabelTTF* pLblRivalInfoAtTalkBox = CCLabelTTF::create(name.append(" ").c_str(), "HelveticaNeue-BoldItalic", 14);
    pLblRivalInfoAtTalkBox->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(90.0f, 245.0f), pLblRivalInfoAtTalkBox, 105);
    
    /*
    CCSprite* pSprTimeBakcground = CCSprite::create("ui/quest/trace/trace_gage_boss_time.png");
    pSprTimeBakcground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprTimeBakcground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS+160.0f, HIT_GAUGE_BACKGROUND_Y_POS-20.0f));
    pSprTimeBakcground->setTag(91);
    this->addChild(pSprTimeBakcground, 9);
    */
    
    //////////////////////
    //
    
    int yy = this->getContentSize().height - accp(160.0f);
    
    refreshBossHp(yy);
    refreshBossMaxHp(yy);
    loadHitGauge();
    refreshHitGauge();
    
    // 보스 게이지 슬래시
    CCSprite* pSprHpGaugeSlash = CCSprite::create("ui/quest/trace/trace_level_gage_slash.png");
    pSprHpGaugeSlash->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprHpGaugeSlash->setPosition(ccp(accp(BOSS_HP_GAUGE_LENGTH / 2.0f + 27.0f), yy + accp(58.5f)));
    pSprHpGaugeSlash->setScale(1.7f);
    this->addChild(pSprHpGaugeSlash, 15);
}

const char* TraceBossLayer::textAdjust(const char *input)
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
/*
<?xml version="1.0" encoding="utf-8"?><response><res>0</res>
<message></message>
<quests>
<quest id="20011" begin="1360774663" end="0" progress="100" clear="0" max_progress="999" enemy="0"></quest>
</quests>
</response>
*/

void TraceBossLayer::refreshBossHp(int yy)
{
    removeBossHp();
    int x = BOSS_HP_GAUGE_LENGTH / 2.0f + 15.0f;
    float scale = 1.0f;
    char buffer[10];
    sprintf(buffer, "%d", (int)curHp);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan=0; scan<length; scan++)
    {
        int number = (value % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
        bossHp[scan] = createNumber(number, ccp(accp(x), yy + accp(52.0f)), scale);
        this->addChild(bossHp[scan], 15);
        CCSize size = bossHp[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 1;
        bossHp[scan]->setPosition(ccp(accp(x), yy + accp(52.0f)));
    }
}

void TraceBossLayer::refreshBossMaxHp(int yy)
{
    removeBossMaxHp();
    bool skip = true;
    int x = BOSS_HP_GAUGE_LENGTH / 2.0f + 15.0f + 27.0f;
    float scale = 1.0f;
    for (int scan=9; scan>-1; scan--)
    {
        int number = ((int)maxHp % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        bossMaxHp[scan] = createNumber(number, ccp(accp(x), yy + accp(52.0f)), scale);
        this->addChild(bossMaxHp[scan], 15);
        CCSize size = bossMaxHp[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceBossLayer::removeBossHp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (bossHp[scan] != NULL)
            this->removeChild(bossHp[scan], true);
        bossHp[scan] = NULL;
    }
}

void TraceBossLayer::removeBossMaxHp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (bossMaxHp[scan] != NULL)
            this->removeChild(bossMaxHp[scan], true);
        bossMaxHp[scan] = NULL;
    }
}

void TraceBossLayer::loadHitGauge()
{
    char buffer[64];
    for (int scan=0; scan<3; scan++)
    {
        sprintf(buffer, "ui/quest/trace/trace_gage_red%d.png", scan+1);
        hitGauge[scan] = CCSprite::create(buffer);
        hitGauge[scan]->setAnchorPoint(ccp(0.0f, 0.0f));
        this->addChild(hitGauge[scan], 10);
    }
}

void TraceBossLayer::refreshHitGauge()
{
    //float ratio = npcInfo-> 1;
    float ratio = (float)curHp / (float)maxHp;
    //float ratio = 1.0f;//0.0f;
    CCLog("ratio : %f", ratio);
    for (int scan=0; scan<2; scan++)
        hitGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    
    int x = HIT_GAUGE_BACKGROUND_X_POS + 7;
    hitGauge[1]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    hitGauge[1]->setScaleX(ratio * HIT_GAUGE_LENGTH);
    /*
    int x = HIT_GAUGE_BACKGROUND_X_POS+5;
    hitGauge[0]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    x += 13;
    hitGauge[1]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
    hitGauge[1]->setScaleX(ratio * HIT_GAUGE_LENGTH);
    x += ratio * HIT_GAUGE_LENGTH;
    hitGauge[2]->setPosition(accp(x, HIT_GAUGE_BACKGROUND_Y_POS+6));
     */
}

void TraceBossLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void TraceBossLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
        //: 좌표를 가져올 임의 터치를 추출합니다.
        CCTouch *touch = (CCTouch*)(touches->anyObject());
        CCPoint location = touch->getLocationInView();
        
        //: UI 좌표를 GL좌표로 변경합니다
        location = CCDirector::sharedDirector()->convertToGL(location);
        
        CCPoint localPoint = this->convertToNodeSpace(location);
    
        //battleTouch(location);
}

void TraceBossLayer::BtnCallback(CCObject* pSender)
{    
    if (TraceLayer::getInstance()->popupCnt>0)return;
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 40: // prev
        {
            ///////////////////
            //
            // 추적 배경 음악 재생
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
                soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
            }
            
            //this->removeChildByTag(200, true);
            this->removeAllChildrenWithCleanup(true);
            TraceLayer::getInstance()->setChaseStep(CHASE_TAB);
        }
        break;
        case 41: // battle
        {
            
            if (PlayerInfo::getInstance()->getBattlePoint() >= PlayerInfo::getInstance()->getTeamBattlePoint(0, PlayerInfo::getInstance()->traceTeam)){
                
                this->removeAllChildrenWithCleanup(true);
                
                ResponseQuestUpdateResultInfo *battleResult = ARequestSender::getInstance()->requestUpdateQuestResult(questID, 3, PlayerInfo::getInstance()->traceTeam, true);
                
                if (atoi((const char*)battleResult->res) == 0){
                    TraceLayer::getInstance()->setBossBattleResult(battleResult);
                }
                else{
                    popupNetworkError(battleResult->res, "boss battle error", "boss battle error");
                }
                
                this->removeAllChildrenWithCleanup(true);
            }
            else{
                // popup
                // 배틀포인트가 모자름
                this->setTouchEnabled(false);
                NoBattlePointPopup* popup = new NoBattlePointPopup();
                popup->InitUI();
                this->addChild(popup, 5000);
            }
        }
        break;
    }
}

void TraceBossLayer::callBackBoss()            // 보스
{
    removeActionResouces();
    InitUI();
}

void TraceBossLayer::removeActionResouces()
{
    for(int i=25;i<=29;i++){
        this->removeChildByTag(i, true);
    }
}

void TraceBossLayer::actionBoss()
{
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    CCSize size = this->getContentSize();
    
    //////////////
    //
    // 보스 등장 액션
    
    //    NpcInfo* npcInfo = getNpc(questResult->enemy_code);
    //    string path = "ui/cha/";
    //    path.append(npcInfo->npcImagePath);
    //npcInfo->npcImagePath
    //npcInfo->touchUp;
    //npcInfo->gaugeDrop;
    
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path.append(npcInfo->npcImagePath);
    
    CCSprite* pSprEnemy = CCSprite::create(path.c_str());
    pSprEnemy->setAnchorPoint(ccp(0.5f, 0.0f));
    pSprEnemy->setPosition(ccp(size.width/2.0f-accp(50), 0.0f+accp(85)));
    pSprEnemy->setScale(1.5f);
    pSprEnemy->setTag(25);
    
    this->addChild(pSprEnemy, -10);
    
    ///////////////////////
    //
    // 보스 등장 시 인트로 액션
    
    // 보스 배경 액션
    
    CCSprite* pSprBossBg = CCSprite::create("ui/quest/trace/quest_rival/quest_boss_Bg.png");
    pSprBossBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprBossBg->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprBossBg->setOpacity(0.0f);
    pSprBossBg->setTag(26);
    
    CCActionInterval* bossBgDelay = CCDelayTime::create(7.0f / 30.0f);
    CCActionInterval* bossBgFadeIn = CCFadeTo::create((27.0f - 7.0f) / 30.0f, 255.0f);
    pSprBossBg->runAction(CCSequence::create(bossBgDelay, bossBgFadeIn, NULL));
    
    CCActionInterval* bossBgScaleUp = CCScaleTo::create((42.0f - 27.0f) / 90.0f, 1.12f);
    CCActionInterval* bossBgScaleDown = CCScaleTo::create((42.0f - 27.0f) / 90.0f, 1.1f);
    pSprBossBg->runAction(CCRepeatForever::create((CCActionInterval* )CCSequence::create(bossBgScaleUp, bossBgScaleDown, NULL)));
    
    this->addChild(pSprBossBg, -10);
    
    // 보스 타이틀 액션
    
    CCSprite* pSprBossTitle = CCSprite::create("ui/quest/trace/quest_rival/foc_quest_boss_1.png");
    pSprBossTitle->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprBossTitle->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprBossTitle->setOpacity(0.0f);
    pSprBossTitle->setRotation(-45.0f);
    pSprBossTitle->setScale(10.0f);
    pSprBossTitle->setTag(27);
    
    CCActionInterval* bossTitleFadeIn = CCFadeTo::create(7.0f / 30.0f, 255.0f);
    CCActionInterval* bossTitleScaleDown = CCScaleTo::create(7.0f / 30.0f, 1.0f);
    CCActionInterval* bossTitleRotate1 = CCRotateTo::create(3.0f / 30.0f, -34.5f);
    CCActionInterval* bossTitleRotate2 = CCRotateTo::create((7.0f - 3.0f) / 30.0f, 0.0f);
    CCActionInterval* bossTitleDelay = CCDelayTime::create((66.0f - 7.0f) / 30.0f + 1.5f);
    CCActionInterval* bossTitleStretch = CCScaleBy::create((72.0f - 66.0f) / 30.0f, 25.0f, 0.3f);
    CCActionInterval* bossTitleFadeOut = CCFadeTo::create((72.0f - 66.0f) / 30.0f, 0.0f);
    
    CCCallFunc* cbRival2 = NULL;
    cbRival2 = CCCallFunc::create(this, callfunc_selector(TraceBossLayer::callBackBoss));
    
    pSprBossTitle->runAction(CCSequence::create(CCSpawn::create(bossTitleFadeIn, bossTitleScaleDown, CCSequence::create(bossTitleRotate1, bossTitleRotate2, NULL), NULL), bossTitleDelay, CCSpawn::create(bossTitleStretch, bossTitleFadeOut, NULL), cbRival2, NULL));
    
    this->addChild(pSprBossTitle, -9);
    
    // 보스 타이틀 효과
    CCSprite* pSprTitleEffect = CCSprite::create("ui/quest/trace/quest_rival/foc_quest_boss_2.png");
    pSprTitleEffect->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTitleEffect->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprTitleEffect->setOpacity(0.0f);
    pSprTitleEffect->setTag(28);
    
    CCActionInterval* titleEffectDelay = CCDelayTime::create(7.0f / 30.0f);
    CCActionInterval* titleEffectFadeIn = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* titleEffectScaleUp = CCScaleBy::create((17.0f - 7.0f) / 30.0f, 3.0f);
    CCActionInterval* titleEffectFadeOut = CCFadeTo::create((17.0f - 7.0f) / 30.0f, 0.0f);
    pSprTitleEffect->runAction(CCSequence::create(titleEffectDelay, titleEffectFadeIn, CCSpawn::create(titleEffectScaleUp, titleEffectFadeOut, NULL), NULL));
    
    this->addChild(pSprTitleEffect, -9);
    
    // 섬광 효과
    CCSprite* pSprSpotlight = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_bglight_03.png");
    pSprSpotlight->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprSpotlight->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprSpotlight->setOpacity(0.0f);
    pSprSpotlight->setTag(29);
    
    CCActionInterval* spotlightDelay1 = CCDelayTime::create(8.0f / 30.0f);
    CCActionInterval* spotlightFadeIn = CCFadeTo::create((12.0f - 8.0f) / 30.0f, 255.0f);
    CCActionInterval* spotlightFadeOut = CCFadeTo::create((40.0f - 20.0f) / 30.0f, 0.0f);
    pSprSpotlight->runAction(CCSequence::create(spotlightDelay1, spotlightFadeIn, spotlightFadeOut, NULL));
    
    this->addChild(pSprSpotlight, -9);
    
}
