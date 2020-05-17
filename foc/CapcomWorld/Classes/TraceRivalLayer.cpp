//
//  TraceRivalLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 13. 2. 12..
//
//

#include "TraceLayer.h"
#include "ShakeAction.h"
#include "BattleFullScreen.h"
#include "MainScene.h"

TraceRivalLayer* TraceRivalLayer::instance = NULL;

TraceRivalLayer* TraceRivalLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

TraceRivalLayer::TraceRivalLayer(NpcInfo* npcInfo, int _rid, int _remainHp, int _MaxHp, int _rivalLevel, int _limitTime, int _questID)
{
    this->npcInfo = npcInfo;
    this->setTouchEnabled(true);
    this->rid = _rid;
    instance = this;
    remainHp = _remainHp;
    maxHp = _MaxHp;
    rivalLevel = _rivalLevel;
    limitTime = _limitTime;
    CCLog(" TraceRivalLayer(), remainHp :%f",remainHp);
    
    questID = _questID;
}

TraceRivalLayer::~TraceRivalLayer()
{
    removeAllChildrenWithCleanup(true);
    
}

void TraceRivalLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void TraceRivalLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    //battleTouch(location);
}

void TraceRivalLayer::BtnCallback(CCObject* pSender)
{
    soundButton1();
    
    if (TraceLayer::getInstance()->popupCnt>0)return;
    if (this->isTouchEnabled()==false)return;
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 40: // prev
        {
            //this->removeChildByTag(200, true);
            this->removeAllChildrenWithCleanup(true);
            
            // 라이벌 이력에서 접근하여 다시 돌아가는 경우
            if (TraceLayer::getInstance()->bCallFromHistory) {
                
                TraceLayer::getInstance()->exitLayer();
            }
            // 추적 상태로 접근하여 다시 추적을 하는 경우
            else {
                
                ///////////////////
                //
                // 라이벌 배경 음악 정지
                
                if (PlayerInfo::getInstance()->getBgmOption()){
                    soundBG->stopBackgroundMusic();
                }
                
                ///////////////////
                //
                // 추적 배경 음악 재생
                
                if (PlayerInfo::getInstance()->getBgmOption()){
                    soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
                    soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
                }
                
                TraceLayer::getInstance()->setChaseStep(CHASE_TAB);
            }
        }
            break;
        case 41: // battle
        {
                        
            if (PlayerInfo::getInstance()->getBattlePoint() >= PlayerInfo::getInstance()->getTeamBattlePoint(0, PlayerInfo::getInstance()->traceTeam)){
                
//                TraceLayer::getInstance()->responseRivalBattleInfo = ARequestSender::getInstance()->requestRBattle(rid,PlayerInfo::getInstance()->traceTeam);
                
//                int r = atoi(TraceLayer::getInstance()->responseRivalBattleInfo->res);
//                if (r == 0){
                int curTime = time(NULL);
                if (0 < limitTime - curTime){
                    
                    TraceLayer::getInstance()->responseRivalBattleInfo = ARequestSender::getInstance()->requestRBattle(rid, PlayerInfo::getInstance()->traceTeam);
                    int result = atoi(TraceLayer::getInstance()->responseRivalBattleInfo->res);
                    if ( result == 0){
                        TraceLayer::getInstance()->setRivalBattleResult(TraceLayer::getInstance()->responseRivalBattleInfo);
                        this->removeAllChildrenWithCleanup(true);
                    }
                    else if ( result == 2024){
                        popupOk("종료된 결투입니다.");
                        // 이미 종료된 배틀 (친구가 이미 킬)
                        // 이미 종료된 배틀입니다.
                        // 결투 버튼 삭제
                        // 뒤로 돌아가기
                    }
                    else{
                        // 네트워크 에러
                        popupNetworkError(TraceLayer::getInstance()->responseRivalBattleInfo->res, TraceLayer::getInstance()->responseRivalBattleInfo->res, "requestRBattle error");
                    }
                }
//                else if (r == 2024){
                else if (0 > limitTime - curTime) {
                    // 종료된 전투
                    popupOk("종료된 결투입니다.");
                }
                else{
                    popupNetworkError(TraceLayer::getInstance()->responseRivalBattleInfo->res, TraceLayer::getInstance()->responseRivalBattleInfo->res, "requestRBattle error");
                }
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

void TraceRivalLayer::callBackRival()
{
    removeActionResouces();
    InitUI();
}

void TraceRivalLayer::removeActionResouces()
{
    for(int i=10;i<=20;i++){
        this->removeChildByTag(i, true);
    }
}


void TraceRivalLayer::InitUI()
{
    CCSize size = this->getContentSize();
    
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path.append(npcInfo->npcImagePath);
    
    CCSprite* pSprEnemy = CCSprite::create(path.c_str());// npcInfo->npcImagePath);
    if (pSprEnemy)
    {
        pSprEnemy->setAnchorPoint(ccp(0.5f, 0.0f));
        pSprEnemy->setPosition(ccp(size.width/2.0f-accp(25), 0.0f+accp(85)));
        pSprEnemy->setScale(1.5f);
        pSprEnemy->setTag(12);
        
        this->addChild(pSprEnemy, 1);//-5);
    }
        
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/quest/trace/trace_btn_blue_a1.png", "ui/quest/trace/trace_btn_blue_a2.png", this, menu_selector(TraceRivalLayer::BtnCallback));
    pSprBtn1->setAnchorPoint( ccp(1,0));
    pSprBtn1->setPosition( ccp(size.width/2-10,0));    // size.width/5 * 0,0));
    pSprBtn1->setTag(40);
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/quest/trace/trace_btn_red_a1.png", "ui/quest/trace/trace_btn_red_a2.png", this, menu_selector(TraceRivalLayer::BtnCallback));
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
    pSprTalkBox->setPosition(accp(20,120));//+90));
    pSprTalkBox->setTag(403);
    this->addChild(pSprTalkBox,2);
    
    std::string npcDesc = textAdjust(npcInfo->npcDesc);
    CCLabelTTF* pLabel3 = CCLabelTTF::create(npcDesc.c_str(), "HelveticaNeue-Bold", 15);
    pLabel3->setColor(COLOR_WHITE);
    pLabel3->setTag(404);
    registerLabel( this,ccp(0.5,0.5), ccp(size.width/2, accp(120+60/*+90*/)), pLabel3, 100);
    
    /////////////////////////////////
    CCSprite* pSprHitBackground = CCSprite::create("ui/quest/trace/trace_gage_bg.png");
    pSprHitBackground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprHitBackground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS, HIT_GAUGE_BACKGROUND_Y_POS));
    pSprHitBackground->setTag(90);
    this->addChild(pSprHitBackground, 10);
    
    CCSprite* pSprVs = CCSprite::create("ui/quest/trace/trace_gage_rival_vs.png");
    pSprVs->setAnchorPoint(ccp(1,0.5));
    pSprVs->setPosition(ccp(size.width-accp(10.0f), accp(HIT_GAUGE_BACKGROUND_Y_POS)));
    this->addChild(pSprVs,10);
    
    CCSprite* pSprTimeBakcground = CCSprite::create("ui/quest/trace/trace_gage_rival_time.png");
    pSprTimeBakcground->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprTimeBakcground->setPosition(accp(HIT_GAUGE_BACKGROUND_X_POS, HIT_GAUGE_BACKGROUND_Y_POS-20.0f));
    pSprTimeBakcground->setTag(91);
    this->addChild(pSprTimeBakcground, 9);
    
    
    
    // time
    
    time_t curTime = time(NULL);
    
    
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    
    int lim_sec  = localtime((time_t* )&(limitTime))->tm_sec;
    int lim_min  = localtime((time_t* )&(limitTime))->tm_min;
    int lim_hour = localtime((time_t* )&(limitTime))->tm_hour;
    
    // 24:00가 00:00으로 표현되기 때문에 발생하는 버그에 대한 수정 코드
    if (lim_hour < cur_hour)    lim_hour += 24;
    
    int cur_total_sec = cur_sec + cur_min*60 + cur_hour * 3600;
    int lit_total_sec = lim_sec + lim_min*60 + lim_hour * 3600;
    
    int time_offset = lit_total_sec - cur_total_sec;
    int time_offset_h = time_offset/3600;
    time_offset = time_offset%3600;
    int time_offset_m = time_offset/60;
    time_offset = time_offset%60;
    int time_offset_s =  time_offset;
    
    this->schedule(schedule_selector(TraceRivalLayer::timeCounter), 1.0f);
    
    string time = "";
    char buf[5];
    
    // hour
    sprintf(buf, "%d", time_offset_h);
    if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
    // min
    sprintf(buf, "%d", time_offset_m);
    if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");;
    
    // sec
    sprintf(buf, "%d", time_offset_s);
    if (time_offset_s<10)  time.append("0").append(buf);
    else time.append(buf);
    
    CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
    pLblTime->setColor(COLOR_GRAY);
    pLblTime->setTag(701);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f-accp(80.0f), accp(HIT_GAUGE_BACKGROUND_Y_POS-19)), pLblTime, 110);

    

    
    
     // npc name
    string npcRivalInfo = "";
    char levelBuf[5];
    sprintf(levelBuf, "%d", rivalLevel);
    CCLog("========================================");
    CCLog("npc level - serve val : %d", rivalLevel);
    CCLog("npc level - buf value : %s", levelBuf);
    CCLog("========================================");
    npcRivalInfo.append("Lv").append(levelBuf).append(". ").append(npcInfo->npcName).append(" ");
    CCLabelTTF* pLblRivalInfo = CCLabelTTF::create(npcRivalInfo.c_str(), "HelveticaNeue-BoldItalic", 13.5f);
    pLblRivalInfo->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(1.0f, 0.5f), ccp(size.width-accp(130.0f), accp(HIT_GAUGE_BACKGROUND_Y_POS-18)), pLblRivalInfo, 105);

    string name = npcInfo->npcName;
    CCLabelTTF* pLblRivalInfoAtTalkBox = CCLabelTTF::create(name.append(" ").c_str(), "HelveticaNeue-BoldItalic", 14);
    pLblRivalInfoAtTalkBox->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(90.0f, 245.0f), pLblRivalInfoAtTalkBox, 105);
    
    

    
    
    //////////////////////
    //
    
    int yy = this->getContentSize().height - accp(160.0f);
    
    refreshRivalHp(yy);
    refreshRivalMaxHp(yy);
    loadHitGauge();
    refreshHitGauge();
    
    // 라이벌 게이지 슬래시
    CCSprite* pSprHpGaugeSlash = CCSprite::create("ui/quest/trace/trace_level_gage_slash.png");
    pSprHpGaugeSlash->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprHpGaugeSlash->setPosition(ccp(accp(RIVAL_HP_GAUGE_LENGTH / 2.0f + 27.0f), yy + accp(58.5f)));
    pSprHpGaugeSlash->setScale(1.7f);
    this->addChild(pSprHpGaugeSlash, 15);
}

void TraceRivalLayer::timeCounter()
{
    removeChildByTag(701, true);
    
    CCSize size = this->getContentSize();
    time_t curTime = time(NULL);
    
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    
    int lim_sec  = localtime((time_t* )&(limitTime))->tm_sec;
    int lim_min  = localtime((time_t* )&(limitTime))->tm_min;
    int lim_hour = localtime((time_t* )&(limitTime))->tm_hour;
    
    // 24:00가 00:00으로 표현되기 때문에 발생하는 버그에 대한 수정 코드
    if (lim_hour < cur_hour)    lim_hour += 24;
    
    int cur_total_sec = cur_sec + cur_min*60 + cur_hour * 3600;
    int lit_total_sec = lim_sec + lim_min*60 + lim_hour * 3600;
    
    int time_offset = lit_total_sec - cur_total_sec;
    int time_offset_h = time_offset/3600;
    time_offset = time_offset%3600;
    int time_offset_m = time_offset/60;
    time_offset = time_offset%60;
    int time_offset_s =  time_offset;
    
    string time = "";
    char buf[5];
    
    // hour
    sprintf(buf, "%d", time_offset_h);
    if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
    // min
    sprintf(buf, "%d", time_offset_m);
    if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
    // sec
    sprintf(buf, "%d", time_offset_s);
    if (time_offset_s<10)   time.append("0").append(buf);
    else time.append(buf);
    
    CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
    pLblTime->setColor(COLOR_GRAY);
    pLblTime->setTag(701);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f-accp(80.0f), accp(HIT_GAUGE_BACKGROUND_Y_POS-19)), pLblTime, 110);
    

    if ((remainHp <= 0) || (limitTime <= curTime))
    {
        if (remainHp > 0)
        {
            TraceLayer::getInstance()->bRemoveDetail = true;
        }
        
        this->unschedule(schedule_selector(TraceRivalLayer::timeCounter));
        removeChildByTag(701, true);
        
        CCLabelTTF* pLblTime = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
        pLblTime->setColor(COLOR_GRAY);
        pLblTime->setTag(701);
        registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f-accp(80.0f), accp(HIT_GAUGE_BACKGROUND_Y_POS-19)), pLblTime, 110);
    }
}


const char* TraceRivalLayer::textAdjust(const char *input)
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

void TraceRivalLayer::refreshRivalHp(int yy)
{
    removeRivalHp();
    int x = RIVAL_HP_GAUGE_LENGTH / 2.0f + 15.0f;
    float scale = 1.0f;
    char buffer[10];
    sprintf(buffer, "%d", (int)remainHp);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan=0; scan<length; scan++)
    {
        int number = (value % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
        rivalHp[scan] = createNumber(number, ccp(accp(x), yy + accp(50.5f)), scale);
        this->addChild(rivalHp[scan], 15);
        CCSize size = rivalHp[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 1;
        rivalHp[scan]->setPosition(ccp(accp(x), yy + accp(50.5f)));
    }
}

void TraceRivalLayer::refreshRivalMaxHp(int yy)
{
    removeRivalMaxHp();
    bool skip = true;
    int x = RIVAL_HP_GAUGE_LENGTH / 2.0f + 15.0f + 27.0f;
    float scale = 1.0f;
    for (int scan=9; scan>-1; scan--)
    {
        int number = ((int)maxHp % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        rivalMaxHp[scan] = createNumber(number, ccp(accp(x), yy + accp(50.5f)), scale);
        this->addChild(rivalMaxHp[scan], 15);
        CCSize size = rivalMaxHp[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceRivalLayer::removeRivalHp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (rivalHp[scan] != NULL)
            this->removeChild(rivalHp[scan], true);
        rivalHp[scan] = NULL;
    }
}

void TraceRivalLayer::removeRivalMaxHp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (rivalMaxHp[scan] != NULL)
            this->removeChild(rivalMaxHp[scan], true);
        rivalMaxHp[scan] = NULL;
    }
}


void TraceRivalLayer::loadHitGauge()
{
    char buffer[64];
    for (int scan=0; scan<3; scan++)
    {
        sprintf(buffer, "ui/quest/trace/trace_gage_yellow%d.png", scan+1);
        hitGauge[scan] = CCSprite::create(buffer);
        hitGauge[scan]->setAnchorPoint(ccp(0.0f, 0.0f));
        this->addChild(hitGauge[scan], 10);
    }
}

void TraceRivalLayer::refreshHitGauge()
{
    float ratio = 0;
    if (npcInfo->hp == 0)ratio=0;
    else ratio = remainHp / maxHp;// npcInfo->hp;// 1;//0.05f;
    if (ratio > 1)  ratio = 1;
//    CCLog("remainHp :%f maxHp : %f hp gage ratio :%f", remainHp, maxHp, ratio);//npcInfo->hp, ratio);
        
    //float ratio = 1.0f;//0.0f;
    //for (int scan=0; scan<2; scan++)
    //    hitGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
        hitGauge[1]->setVisible((ratio <= 0.0f) ? false : true);
    
    
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



void TraceRivalLayer::actionRival1()
{
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    CCSize size = this->getContentSize();
    
    ///////////
    //
    // 배경 액션
    
    CCSprite* pSprRivalBg = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_bg.jpg");
    pSprRivalBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprRivalBg->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprRivalBg->setScale(0.9f);
    pSprRivalBg->setOpacity(0.0f);
    pSprRivalBg->setTag(10);
    
    CCActionInterval* bgFadeIn = CCFadeTo::create((74.0f - 54.0f) / 30.0f, 255.0f);
    CCActionInterval* bgScaleUp = CCScaleTo::create((74.0f - 54.0f) / 30.0f, 1.1f);
    pSprRivalBg->runAction(CCSequence::create(CCSpawn::create(bgFadeIn, bgScaleUp ,NULL), NULL));
    
    CCActionInterval* bgRotate = CCRotateBy::create((144.0f - 54.0f) / 30.0f, -45.0f);
    pSprRivalBg->runAction(CCRepeatForever::create(bgRotate));
    
    this->addChild(pSprRivalBg, -10);
    
    ///////////////
    //
    // 불꽃 애니메이션
    
    CCSpriteFrame* fireFrame0  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_00.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame1  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_01.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame2  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_02.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame3  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_03.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame4  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_04.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame5  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_05.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame6  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_06.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame7  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_07.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame8  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_08.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame9  = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_09.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame10 = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_10.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame11 = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_11.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame12 = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_12.png", CCRectMake(0, 0, accp(640), accp(185)));
    CCSpriteFrame* fireFrame13 = CCSpriteFrame::create("ui/quest/trace/quest_rival/quest_rival_fire_02_13.png", CCRectMake(0, 0, accp(640), accp(185)));
    
    frameToFire = new CCArray();
    frameToFire->addObject(fireFrame0);
    frameToFire->addObject(fireFrame1);
    frameToFire->addObject(fireFrame2);
    frameToFire->addObject(fireFrame3);
    frameToFire->addObject(fireFrame4);
    frameToFire->addObject(fireFrame5);
    frameToFire->addObject(fireFrame6);
    frameToFire->addObject(fireFrame7);
    frameToFire->addObject(fireFrame8);
    frameToFire->addObject(fireFrame9);
    frameToFire->addObject(fireFrame10);
    frameToFire->addObject(fireFrame11);
    frameToFire->addObject(fireFrame12);
    frameToFire->addObject(fireFrame13);
    
    CCAnimation* animationToFire = CCAnimation::create();
    for (int i=0; i<frameToFire->count(); i++)
    {
        animationToFire->addSpriteFrame((CCSpriteFrame* )frameToFire->objectAtIndex(i));
    }
    animationToFire->setLoops(1);
    animationToFire->setDelayPerUnit((74.0f - 68.0f) / 30.0f / 5.0f);
    
    CCSpriteFrame* fireFrame = (CCSpriteFrame* )frameToFire->objectAtIndex(0);
    
    // 상단 불꽃
    
    spriteToHighFire = CCSprite::createWithSpriteFrame(fireFrame);
    spriteToHighFire->setAnchorPoint(ccp(0.5f, 1.0f));
    spriteToHighFire->setPosition(ccp(size.width/2.0f, size.width+accp(80.0f)));
    spriteToHighFire->setRotation(180.0f);
    spriteToHighFire->setOpacity(0.0f);
    spriteToHighFire->setTag(11);
    
    CCActionInterval* highFireDelay = CCDelayTime::create((68.0f - 54.0f) / 30.0f);
    CCActionInterval* highFireFadeIn = CCFadeTo::create((68.0f - 54.0f) / 30.0f, 255.0f);
    spriteToHighFire->runAction(CCSequence::create(highFireDelay, highFireFadeIn, NULL));
    
    CCAnimate* animateToHighFire = CCAnimate::create(animationToFire);
    spriteToHighFire->runAction(CCRepeatForever::create(animateToHighFire));
    
    this->addChild(spriteToHighFire, -9);
    
    // 하단 불꽃
    
    spriteToLowFire = CCSprite::createWithSpriteFrame(fireFrame);
    spriteToLowFire->setAnchorPoint(ccp(0.5f, 0.0f));
    spriteToLowFire->setPosition(ccp(size.width/2.0f, accp(80.0f)));
    spriteToLowFire->setOpacity(0.0f);
    spriteToLowFire->setTag(12);
    
    CCActionInterval* lowFireDelay = CCDelayTime::create((68.0f - 54.0f) / 30.0f);
    CCActionInterval* lowFireFadeIn = CCFadeTo::create((68.0f - 54.0f) / 30.0f, 255.0f);
    spriteToLowFire->runAction(CCSequence::create(lowFireDelay, lowFireFadeIn, NULL));
    
    /*
     * 같은 Frame을 사용하더라도 CCAnimate는 다르게 만들어주어야 각자의 애니메이션을 갖는다.
     */
    CCAnimate* animateToLowFire = CCAnimate::create(animationToFire);
    spriteToLowFire->runAction(CCRepeatForever::create(animateToLowFire));
    
    this->addChild(spriteToLowFire, -9);
    
    //////////////
    //
    // 준비, 땅 액션
    
    CCSprite* pSprReadyGo = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_line.png");
    pSprReadyGo->setTag(13);
    
    CCActionInterval* readyGoDelay = CCDelayTime::create((62.0f - 54.0f) / 30.0f);
    
    if (TraceLayer::getInstance()->receivedTraceResultInfo->enemy_type == 2)        // 라이벌인 경우
    {
        CCLog("Rival Action!");
        
        pSprReadyGo->setAnchorPoint(ccp(0.0f, 0.5f));
        pSprReadyGo->setPosition(ccp(size.width, size.height/2.0f));
        
        CCActionInterval* readyGoMoveToLeft = CCMoveBy::create((70.0f - 62.0f) / 30.0f, accp(-1175.0f, 0.0f));
        pSprReadyGo->runAction(CCSequence::create(readyGoDelay, readyGoMoveToLeft, NULL));
    }
    else if (TraceLayer::getInstance()->receivedTraceResultInfo->enemy_type == 3)   // 히든 라이벌인 경우
    {
        CCLog("Hidden Rival Action!");
        
        pSprReadyGo->setAnchorPoint(ccp(1.0f, 0.5f));
        pSprReadyGo->setPosition(ccp(0.0f, size.height/2.0f));
        
        CCActionInterval* readyGoMoveToRight = CCMoveBy::create((70.0f - 62.0f) / 30.0f, accp(1175.0f, 0.0f));
        pSprReadyGo->runAction(CCSequence::create(readyGoDelay, readyGoMoveToRight, NULL));
    }
    
    this->addChild(pSprReadyGo, -9);
    
    //////////////////
    //
    // 도전자 타이틀 액션
    
    // 추적 배경 음악 정지
    if (PlayerInfo::getInstance()->getBgmOption()) {
        soundBG->stopBackgroundMusic();
    }
    
    // HERE COMES
    
    CCSprite* pSprHereComes = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_here_1.png");
    pSprHereComes->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprHereComes->setPosition(ccp(size.width/2.0f-accp(610.0f), size.height/2.0f));
    pSprHereComes->setOpacity(0.0f);
    pSprHereComes->setTag(14);
    
    CCActionInterval* hereComesDelay1 = CCDelayTime::create((66.0f - 54.0f) / 30.0f);
    CCActionInterval* hereComesMoveToRight = CCMoveTo::create((72.0f - 66.0f) / 30.0f, ccp(size.width/2.0f, size.height/2.0f));
    CCActionInterval* hereComesFadeIn = CCFadeTo::create((72.0f - 66.0f) / 30.0f, 255.0f);
    CCCallFunc* cbPlayHit = CCCallFunc::create(this, callfunc_selector(MyUtil::soundGo));
    CCActionInterval* hereComesDelay2 = CCDelayTime::create((92.0f - 72.0f) / 30.0f);//(132.0f - 72.0f) / 30.0f);
    CCCallFunc* cbPlayRival = CCCallFunc::create(this, callfunc_selector(MyUtil::soundRival));
    CCActionInterval* hereComesDelay3 = CCDelayTime::create((132.0f - 92.0f) / 30.0f);
    CCActionInterval* hereComesFadeOut = CCFadeTo::create((138.0f - 132.0f) / 30.0f, 0.0f);
    CCActionInterval* hereComesScaleUp = CCScaleBy::create((138.0f - 132.0f) / 30.0f, 4.0f);
    pSprHereComes->runAction(CCSequence::create(hereComesDelay1, CCSpawn::create(hereComesMoveToRight, hereComesFadeIn, NULL), cbPlayHit, hereComesDelay2, CCSpawn::create(cbPlayRival, hereComesDelay3,NULL), CCSpawn::create(hereComesFadeOut, hereComesScaleUp, NULL), NULL));
    
    this->addChild(pSprHereComes, -9);
    
    // A NEW CHALLENGER
    
    CCSprite* pSprChallenger = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_here_2.png");
    pSprChallenger->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprChallenger->setPosition(ccp(size.width/2.0f+accp(610.0f), size.height/2.0f));
    pSprChallenger->setOpacity(0.0f);
    pSprChallenger->setTag(15);
    
    CCActionInterval* challengerDelay1 = CCDelayTime::create((66.0f - 54.0f) / 30.0f);
    CCActionInterval* challengerMoveToLeft = CCMoveTo::create((72.0f - 66.0f) / 30.0f, ccp(size.width/2.0f, size.height/2.0f));
    CCActionInterval* challengerFadeIn = CCFadeTo::create((72.0f - 66.0f) / 30.0f, 255.0f);
    CCActionInterval* challengerDelay2 = CCDelayTime::create((132.0f - 72.0f) / 30.0f);
    CCActionInterval* challengerFadeOut = CCFadeTo::create((138.0f - 132.0f) / 30.0f, 0.0f);
    CCActionInterval* challengerScaleUp = CCScaleBy::create((138.0f - 132.0f) / 30.0f, 4.0f);
    pSprChallenger->runAction(CCSequence::create(challengerDelay1, CCSpawn::create(challengerMoveToLeft, challengerFadeIn, NULL), challengerDelay2, CCSpawn::create(challengerFadeOut, challengerScaleUp, NULL), NULL));
    
    this->addChild(pSprChallenger, -9);
    
    // POPUP TITLE
    
    CCSprite* pSprPopupTitle = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_here_4.png");
    pSprPopupTitle->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopupTitle->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprPopupTitle->setOpacity(0.0f);
    pSprPopupTitle->setTag(16);
    
    CCActionInterval* popupTitleDelay = CCDelayTime::create((72.0f - 54.0f) / 30.0f);
    CCActionInterval* popupTitleShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* popupTitleFadeOut = CCFadeTo::create((82.0f - 72.0f) / 30.0f, 0.0f);
    CCActionInterval* popupTitleScaleUp = CCScaleBy::create((82.0f - 72.0f) / 30.0f, 2.0f);
    pSprPopupTitle->runAction(CCSequence::create(popupTitleDelay, popupTitleShow, CCSpawn::create(popupTitleFadeOut, popupTitleScaleUp, NULL), NULL));
    
    this->addChild(pSprPopupTitle, -9);
    
    // WHITE TITLE
    
    CCSprite* pSprWhiteTitle = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_here_3.png");
    pSprWhiteTitle->setAnchorPoint(ccp(0.5f ,0.5f));
    pSprWhiteTitle->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprWhiteTitle->setOpacity(0.0f);
    pSprWhiteTitle->setTag(17);
    
    CCActionInterval* whiteTitleDelay1 = CCDelayTime::create((72.0f - 54.0f) / 30.0f);
    CCActionInterval* whiteTitleFadeIn = CCFadeTo::create((82.0f - 72.0f) / 30.0f, 255.0f);
    CCActionInterval* whiteTitleFadeOut = CCFadeTo::create((92.0f - 82.0f) / 30.0f, 0.0f);
    CCActionInterval* whiteTitleDelay2 = CCDelayTime::create((102.0f - 92.0f) / 30.0f);
    pSprWhiteTitle->runAction(CCSequence::create(whiteTitleDelay1, CCRepeat::create(CCSequence::create(whiteTitleFadeIn, whiteTitleFadeOut, whiteTitleDelay2, NULL), 3), NULL));
    
    this->addChild(pSprWhiteTitle, -9);
    
    ////////////////
    //
    // 타이틀 배경 액션
    
    CCSprite* pSprTitleBg = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_here_bg.png");
    pSprTitleBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTitleBg->setPosition(ccp(size.width/2.0f, size.height/2.0f-accp(40)));
    pSprTitleBg->setOpacity(0.0f);
    pSprTitleBg->setTag(18);
    
    CCActionInterval* titleBgDelay1 = CCDelayTime::create((73.0f - 54.0f) / 30.0f);
    CCActionInterval* titleBgShow = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* titleBgDelay2 = CCDelayTime::create((138.0f - 73.0f) / 30.0f);
    pSprTitleBg->runAction(CCSequence::create(titleBgDelay1, titleBgShow, titleBgDelay2, NULL));
    
    this->addChild(pSprTitleBg, -10);
    
    ///////////////
    //
    // 섬광 애니메이션
    
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
    spriteToSpotlight->setTag(19);
    
    CCAnimation* animationToSpotlight = CCAnimation::create();
    CCLog("frame count : %d", frameToSpotlight->count());
    for (int i=0; i<frameToSpotlight->count(); i++)
    {
        animationToSpotlight->addSpriteFrame((CCSpriteFrame* )frameToSpotlight->objectAtIndex(i));
    }
    animationToSpotlight->setLoops(1);
    animationToSpotlight->setDelayPerUnit((78.0f - 71.0f) / 30.0f / 5.0f);
    
    CCAnimate* animateToSpotlight = CCAnimate::create(animationToSpotlight);
    
    CCActionInterval* spotlightDelay = CCDelayTime::create((71.0f - 54.0f) / 30.0f);
    CCActionInterval* spotlightShow = CCFadeTo::create(0.0f, 150.0f);
    CCActionInterval* spotlightHide = CCFadeTo::create(0.0f, 0.0f);
    spriteToSpotlight->runAction(CCSequence::create(spotlightDelay, spotlightShow, animateToSpotlight, spotlightHide, NULL));
    
    this->addChild(spriteToSpotlight, -10);
    
    //////////////
    //
    // 배경 전환 액션
    
    CCSprite* pSprChanger = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_bglight_03.png");
    pSprChanger->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprChanger->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprChanger->setOpacity(0.0f);
    pSprChanger->setTag(20);
    
    CCActionInterval* changerDelay1 = CCDelayTime::create((134.0f - 54.0f) / 30.0f);
    CCActionInterval* changerFadeIn = CCFadeTo::create((143.0f - 134.0f) / 30.0f, 255.0f);
    CCActionInterval* changerScaleUp = CCScaleBy::create((143.0f - 134.0f) / 30.0f, 10.0f);
    CCActionInterval* changerDelay2 = CCDelayTime::create((158.0f - 143.0f) / 30.0f);
    
    CCCallFunc* cbRival2 = NULL;
    cbRival2 = CCCallFunc::create(this, callfunc_selector(TraceRivalLayer::actionRival2));
    CCCallFunc* cbPlayRivalBG = CCCallFunc::create(this, callfunc_selector(MyUtil::soundRivalBG));
    pSprChanger->runAction(CCSequence::create(changerDelay1, CCSpawn::create(changerFadeIn, changerScaleUp, NULL), changerDelay2, CCSpawn::create(cbPlayRivalBG, cbRival2, NULL), NULL));
    
    this->addChild(pSprChanger, 0);
}

void TraceRivalLayer::actionRival2()
{
    for(int i=10;i<=20;i++){
        this->removeChildByTag(i, true);
    }
    
    CCSize size = this->getContentSize();
    
    ////////////////
    //
    // 전체 흔드는 액션
    
    CCLayer* layer = new CCLayer;
    layer->setAnchorPoint(ccp(0.5, 0.5f));
    layer->setPosition(ccp(0.0f, 0.0f));
    layer->setTag(10);
    
    CCActionInterval* delay = CCDelayTime::create(7.0f / 30.0f);
    CCActionInterval* shake = CCShake::create((13.0f - 7.0f) / 30.0f, 28.5, 0.0f, 0.0f);
    layer->runAction(CCSequence::create(delay, shake, NULL));
    
    this->addChild(layer, -10);
    
    // 흔들 때만 생성하는 배경 스프라이트

    string bgPath = CCFileUtils::sharedFileUtils()->getDocumentPath();
    CCLog("quest id : %d", questID);
    if (questID == 0)
    {
        Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(receivedRivalInfo->quest_id);
        CCLog("quest id : %d", receivedRivalInfo->quest_id);
        bgPath = bgPath + questInfo->stageBG_L;
    }
    else
    {
        Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
        CCLog("quest Id : %d", questID);
        bgPath = bgPath + questInfo->stageBG_L;
    }
    
//    CCSprite* pSprBackground = CCSprite::create("ui/main_bg/bg_1.png");
    CCSprite* pSprBackground = CCSprite::create(bgPath.c_str());
    pSprBackground->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprBackground->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprBackground->setScale(2.5f);    // 배경 이미지 250% 확대
    pSprBackground->setTag(11);
    
    CCActionInterval* bgDelay = CCDelayTime::create(13.0f / 30.0f);
    CCActionInterval* bgHide = CCFadeTo::create(0.0f, 0.0f);
    pSprBackground->runAction(CCSequence::create(bgDelay, bgHide, NULL));
    
    layer->addChild(pSprBackground, -11);
    
    ////////////////
    //
    // 라이벌 등장 액션
    
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string path = "ui/cha/";
    path.append(npcInfo->npcImagePath);
    
    CCSprite* pSprEnemy = CCSprite::create(path.c_str());
    CCLog("Enemy Image Address : %s", path.c_str());
    //CCSprite* pSprEnemy = CCSprite::create("ui/cha/sf_gouki.png");
//    CCSprite* pSprEnemy = CCSprite::create(path.c_str());
    pSprEnemy->setAnchorPoint(ccp(0.5f, 0.0f));
    pSprEnemy->setPosition(ccp(size.width/2.0f-accp(25.0f), accp(85.0f)));
    pSprEnemy->setScale(1.5f);
    pSprEnemy->setTag(12);
    
    layer->addChild(pSprEnemy, -10);
    //    this->addChild(pSprEnemy, -10);
    
    
    ////////////////////////
    //
    // 라이벌 등장 시 인트로 액션
    
    // 흰색에서 전환
    
    CCSprite* pSprCurtain = CCSprite::create("ui/quest/trace/quest_ko/quest_ko_bg_white.png");
    pSprCurtain->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprCurtain->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprCurtain->setTag(13);
    
    CCActionInterval* curtainFadeOut = CCFadeTo::create(11.0f / 30.0f, 0.0f);
    pSprCurtain->runAction(curtainFadeOut);
    
    layer->addChild(pSprCurtain, -10);
    //    this->addChild(pSprCurtain, -10);
    
    // 라이벌 타이틀
    
    CCSprite* pSprRivalTitle = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_normal_1.png");
    pSprRivalTitle->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprRivalTitle->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprRivalTitle->setOpacity(0.0f);
    pSprRivalTitle->setRotation(-45.0f);
    pSprRivalTitle->setScale(10.0f);
    pSprRivalTitle->setTag(14);
    
    CCActionInterval* rivalTitleFadeIn = CCFadeTo::create(7.0f / 30.0f, 255.0f);
    CCActionInterval* rivalTitleScaleDown = CCScaleTo::create(7.0f / 30.0f, 1.0f);
    CCActionInterval* rivalTitleRotate1 = CCRotateTo::create(3.0f / 30.0f, -34.5f);
    CCActionInterval* rivalTitleRotate2 = CCRotateTo::create((7.0f - 3.0f) / 30.0f, 0.0f);
    CCCallFunc* cbPlayHit = CCCallFunc::create(this, callfunc_selector(MyUtil::soundHit));
    CCActionInterval* rivalTitleDelay = CCDelayTime::create((66.0f - 7.0f) / 30.0f);
    CCActionInterval* rivalTitleStretch = CCScaleBy::create((72.0f - 66.0f) / 30.0f, 25.0f, 0.3f);
    CCActionInterval* rivalTitleFadeOut = CCFadeTo::create((72.0f - 66.0f) / 30.0f, 0.0f);
    CCCallFunc* cbRival2 = NULL;
    cbRival2 = CCCallFunc::create(this, callfunc_selector(TraceRivalLayer::callBackRival));
    
    pSprRivalTitle->runAction(CCSequence::create(CCSpawn::create(rivalTitleFadeIn, rivalTitleScaleDown, CCSequence::create(rivalTitleRotate1, rivalTitleRotate2, cbPlayHit, NULL), NULL), rivalTitleDelay, CCSpawn::create(rivalTitleStretch, rivalTitleFadeOut, NULL), cbRival2, NULL));
    
    layer->addChild(pSprRivalTitle, -9);
    //    this->addChild(pSprRivalTitle, -9);
    
    // 라이벌 타이틀 효과
    
    CCSprite* pSprTitleEffect = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_normal_02.png");
    pSprTitleEffect->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTitleEffect->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprTitleEffect->setOpacity(0.0f);
    pSprTitleEffect->setTag(15);
    
    CCActionInterval* titleEffectDelay = CCDelayTime::create(10.0f / 30.0f);
    CCActionInterval* titleEffectFadeIn = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* titleEffectScaleUp = CCScaleBy::create((20.0f - 10.0f) / 30.0f, 3.0f);
    CCActionInterval* titleEffectFadeOut = CCFadeTo::create((20.0f - 10.0f) / 30.0f, 0.0f);
    pSprTitleEffect->runAction(CCSequence::create(titleEffectDelay, titleEffectFadeIn, CCSpawn::create(titleEffectScaleUp, titleEffectFadeOut, NULL), NULL));
    
    layer->addChild(pSprTitleEffect, -9);
    //    this->addChild(pSprTitleEffect, -9);
    
    // 섬광 효과
    
    CCSprite* pSprSpotlight = CCSprite::create("ui/quest/trace/quest_rival/quest_rival_bglight_03.png");
    pSprSpotlight->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprSpotlight->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprSpotlight->setOpacity(0.0f);
    pSprSpotlight->setTag(16);
    
    CCActionInterval* spotlightDelay1 = CCDelayTime::create(8.0f / 30.0f);
    CCActionInterval* spotlightFadeIn = CCFadeTo::create((20.0f - 8.0f) / 30.0f, 255.0f);
    CCActionInterval* spotlightFadeOut = CCFadeTo::create((40.0f - 20.0f) / 30.0f, 0.0f);
    
    
    
    pSprSpotlight->runAction(CCSequence::create(spotlightDelay1, spotlightFadeIn, spotlightFadeOut, NULL));
    
    layer->addChild(pSprSpotlight, -10);
    //    this->addChild(pSprSpotlight, -10);
    
}

