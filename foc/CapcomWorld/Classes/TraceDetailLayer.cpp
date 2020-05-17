//
//  TraceDetailLayer.cpp
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 14..
//
//

//#include "TraceDetailLayer.h"
#include "MainScene.h"
#include "TraceLayer.h"
#include "CCHttpRequest.h"
#include <stdlib.h>
#include <string.h>
#include <algorithm>
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif
using namespace std;


TraceDetailLayer* TraceDetailLayer::instance = NULL;

void TraceDetailLayer::init()
{
    time_t curTime = time(NULL);
    CCSize size = this->getContentSize();

    CCLayerColor* layer = CCLayerColor::create(ccc4(200, 200, 200, 0), size.width, size.height);
    layer->setAnchorPoint(ccp(0.0f, 0.0f));
    layer->setPosition(ccp(0.0f, 0.0f));
    layer->setTouchEnabled(true);
    layer->setTag(0);
    this->addChild(layer, 0);
 
    const float DETAIL_BACK_BTN_SPACE = 50.0f;
    const float DETAIL_BACK_BTN_MARGIN = 10.0f;
    const float DETAIL_CHARACTER_SPACE = 230.0f;
    const float COLLEAGUE_CELL_HEIGHT = 166.0f;
    const float BACKGROUND_STROKE_THICKNESS = 5.0f;
    const float RIVAL_STATBAR_HEIGHT = 82.0f;
    const int LAYER_CLIP_HEIGHT = size.height - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN);
    
    const int colleagueCount = rivalInfo->colleagues->count();
    CCLog("colleagueCount : %d", colleagueCount);
    const float layerHeight = accp(DETAIL_BACK_BTN_SPACE + DETAIL_BACK_BTN_MARGIN*2 + DETAIL_CHARACTER_SPACE + DETAIL_BACK_BTN_MARGIN*2 + COLLEAGUE_CELL_HEIGHT*colleagueCount + DETAIL_BACK_BTN_MARGIN*colleagueCount + RIVAL_STATBAR_HEIGHT + DETAIL_BACK_BTN_MARGIN);
    LayerStartPos = LAYER_CLIP_HEIGHT - layerHeight;
    layer->setContentSize(CCSize(size.width, layerHeight));
    layer->setPosition(ccp(0.0f, LayerStartPos + accp(6.0f)));

    int yy = layer->getContentSize().height;
    
    //////////////
    //
    // Back Button
    
    yy -= accp(DETAIL_BACK_BTN_SPACE + DETAIL_BACK_BTN_MARGIN/2.0f);
    
    CCMenuItemImage* pSprBackBtn = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png", "ui/card_tab/team/cards_bt_back_a2.png", this, menu_selector(TraceDetailLayer::cbBackBtn));
    pSprBackBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprBackBtn->setPosition(ccp(0.0f, 0.0f));

    CCLabelTTF* pLblBackBtn = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 12);
    pLblBackBtn->setColor(COLOR_YELLOW);
    pLblBackBtn->setTag(1);
    registerLabel(layer, ccp(0.5f, 0.0f), ccp(size.width/2.0f, yy + accp(10.0f)), pLblBackBtn, 1);
    
    CCMenu* pMenu = CCMenu::create(pSprBackBtn, NULL);
    pMenu->setAnchorPoint(ccp(0.0f, 0.0f));
    pMenu->setPosition(ccp(accp(10.0f), yy));
    pMenu->setTag(3);
    layer->addChild(pMenu, 0);
    
    /*
    yy -= accp(DETAIL_BACK_BTN_SPACE + DETAIL_BACK_BTN_MARGIN/2.0f);
    
    CCMenuItemImage* pSprRefreshBtn = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png", "ui/card_tab/team/cards_bt_back_a2.png", this, menu_selector(TraceDetailLayer::cbRefresh));
    pSprRefreshBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprRefreshBtn->setPosition(ccp(0.0f, 0.0f));
    
    CCLabelTTF* pBtn1 = CCLabelTTF::create("??? ????¬Æ", "HelveticaNeue-Bold", 12);
    pBtn1->setColor(COLOR_YELLOW);
    pBtn1->setTag(1);
    registerLabel(layer, ccp(0.5f, 0.0f), ccp(size.width/2.0f, yy + accp(10.0f)), pBtn1, 1);
    
    CCMenu* pMenu2 = CCMenu::create(pSprRefreshBtn, NULL);
    pMenu2->setAnchorPoint(ccp(0.0f, 0.0f));
    pMenu2->setPosition(ccp(accp(10.0f), yy));
    pMenu2->setTag(3);
    layer->addChild(pMenu2, 0);
    */
    
    ///////////////////
    //
    // 상황에 따라 다른 것
    
    // 상세 결과
    CCSprite* pSprDetail = NULL;
//    CCMenuItemImage* pMenuImgDetail = NULL;
    CCMenu* pMenuDetail = NULL;
    CCLabelTTF* pLblDetal;
    

    if((rivalInfo->cur_hp <= 0) || (rivalInfo->limit <= curTime))  // 끝난 결과가 있는 경우
    {
        if (rivalInfo->cur_hp > 0)  // 패배한 결과
        {
            // 상세 결과
            pSprDetail = CCSprite::create("ui/battle/battle_duel_result_lose.png");
            pSprDetail->setAnchorPoint(ccp(0.0f, 0.0f));
            pSprDetail->setPosition(ccp(size.width/2.0f - accp(50.0f), yy - accp(DETAIL_CHARACTER_SPACE + DETAIL_BACK_BTN_MARGIN*2.0f) + accp(55.0f)));
            pSprDetail->setTag(2200);
            layer->addChild(pSprDetail, 10);
            
            // 시간 종료
            CCLabelTTF* pLblTimeLimit = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
            pLblTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
            pLblTimeLimit->setPosition(ccp(accp(170.0f), yy - accp(333.0f)));
            pLblTimeLimit->setColor(COLOR_GRAY);
            pLblTimeLimit->setTag(2100);
            layer->addChild(pLblTimeLimit, 2);
            CCLog("victory");
        }
        else                        // 승리한 결과
        {
            bool bEnableReward = false;
            for(int j=0;j<rivalInfo->colleagues->count();j++)
            {
                AReceivedColleague *collInfo = (AReceivedColleague*)rivalInfo->colleagues->objectAtIndex(j);
                if (collInfo->userid == PlayerInfo::getInstance()->userID){
                    bEnableReward=true;
                    break;
                }
            }
            
            if (bEnableReward && rivalInfo->bRewardReceived == false)
            {
                layer->removeChildByTag(1, true);
                layer->removeChildByTag(3, true);
                
                // 승리하였고 보상 시그널이 참인 경우 상위 메뉴가 뒤로 가기와 보상 받기로 변경
                CCMenuItemImage* pSprNewBackBtn = CCMenuItemImage::create("ui/quest/trace/history_btn_back_a1.png", "ui/quest/trace/history_btn_back_a2.png", this, menu_selector(TraceDetailLayer::cbBackBtn));
                pSprNewBackBtn->setAnchorPoint(ccp(0.0f, 0.0f));
                pSprNewBackBtn->setPosition(ccp(0.0f, 0.0f));
                
                CCLabelTTF* pLblNewBackBtn = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 12);
                pLblNewBackBtn->setColor(COLOR_YELLOW);
                pLblNewBackBtn->setTag(1);
                registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.0f, 0.0f), ccp(accp(125.0f), yy + accp(10.0f)), pLblNewBackBtn, 1);
                
                CCMenuItemImage* pSprNewReward = CCMenuItemImage::create("ui/quest/trace/history_btn_reward_a1.png", "ui/quest/trace/history_btn_reward_a2.png", this, menu_selector(TraceDetailLayer::cbReward));
                pSprNewReward->setAnchorPoint(ccp(0.0f, 0.0f));
                pSprNewReward->setPosition(ccp(this->getContentSize().width/2.0f, 0.0f));
                
                CCLabelTTF* pLblNewReward = CCLabelTTF::create("보상 받기", "HelveticaNeue-Bold", 12);
                pLblNewReward->setColor(COLOR_WHITE);
                pLblNewReward->setTag(2);
                registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.0f, 0.0f), ccp(size.width/2.0f + accp(125.0f), yy + accp(10.0f)), pLblNewReward, 1);
                
                CCMenu* pNewMenu = CCMenu::create(pSprNewBackBtn, pSprNewReward, NULL);
                pNewMenu->setAnchorPoint(ccp(0.0f, 0.0f));
                pNewMenu->setPosition(ccp(accp(10.0f), yy));
                pNewMenu->setTag(3);
                this->getChildByTag(0)->addChild(pNewMenu, 0);
            }
            
            // 상세 결과
            pSprDetail = CCSprite::create("ui/battle/battle_duel_result_win.png");
            pSprDetail->setAnchorPoint(ccp(0.0f, 0.0f));
            pSprDetail->setPosition(ccp(size.width/2.0f - accp(50.0f), yy - accp(DETAIL_CHARACTER_SPACE + DETAIL_BACK_BTN_MARGIN*2.0f) + accp(55.0f)));
            pSprDetail->setTag(2200);
            layer->addChild(pSprDetail, 10);

            // 시간 종료
            CCLabelTTF* pLblTimeLimit = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
            pLblTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
            pLblTimeLimit->setPosition(ccp(accp(170.0f), yy - accp(333.0f)));
            pLblTimeLimit->setColor(COLOR_GRAY);
            pLblTimeLimit->setTag(2100);
            layer->addChild(pLblTimeLimit, 2);
            CCLog("Lose");
        }
    }
    else
    {
        // 상세 결과
        CCMenuItemImage* pSprDetail = CCMenuItemImage::create("ui/quest/trace/trace_btn_red_a1.png", "ui/quest/trace/trace_btn_red_a2.png", this, menu_selector(TraceDetailLayer::cbDetail));
        pMenuDetail = CCMenu::create(pSprDetail, NULL);
        pMenuDetail->setAnchorPoint(ccp(0.0f, 0.0f));
        pMenuDetail->setPosition(ccp(accp(510.0f), yy - accp(138.0f)));
        pMenuDetail->setTag(2200);
        layer->addChild(pMenuDetail, 1);
        
        pLblDetal = CCLabelTTF::create("결투", "HelveticaNeue-Bold", 14);
        pLblDetal->setColor(COLOR_GRAY);
        pLblDetal->setTag(2300);
        registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(428.0f) + accp(40.0f), yy - accp(150.0f)), pLblDetal, 1);
        

        int cur_sec  = localtime(&curTime)->tm_sec;
        int cur_min  = localtime(&curTime)->tm_min;
        int cur_hour = localtime(&curTime)->tm_hour;
        
        int lim_sec  = localtime((time_t* )&(rivalInfo->limit))->tm_sec;
        int lim_min  = localtime((time_t* )&(rivalInfo->limit))->tm_min;
        int lim_hour = localtime((time_t* )&(rivalInfo->limit))->tm_hour;
        
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
        
        this->schedule(schedule_selector(TraceDetailLayer::timeCounter), 1.0f);
 
        string time = "";
        char buf[5];
        // hour
        
//        string strHour = "";
//        char bufHour[5];
        sprintf(buf, "%d", time_offset_h);
        if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
        else time.append(buf).append(" : ");
        
//        CCLabelTTF* pLblHour = CCLabelTTF::create(strHour.c_str(), "HelveticaNeue-Bold", 14);
//        pLblHour->setColor(COLOR_GRAY);
//        pLblHour->setTag(701);
//        registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(170.0f), yy - accp(332.0f)), pLblHour, 110);
        
        // min
        
//        string strMin = "";
//        char bufMin[5];
        sprintf(buf, "%d", time_offset_m);
        if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
        else time.append(buf).append(" : ");;
        
//        CCLabelTTF* pLblMin = CCLabelTTF::create(strMin.c_str(), "HelveticaNeue-Bold", 14);
//        pLblMin->setColor(COLOR_GRAY);
//        pLblMin->setTag(801);
//        registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(170.0f + 55.0f), yy - accp(332.0f)), pLblMin, 110);
        
        // sec
        
//        string strSec = "";
//        char bufSec[3];
        
        sprintf(buf, "%d", time_offset_s);
        if (time_offset_s<10)  time.append("0").append(buf);
        else time.append(buf);
        
        CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
        pLblTime->setColor(COLOR_GRAY);
        pLblTime->setTag(701);
        registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(170.5f), yy - accp(333.0f)), pLblTime, 110);
    }
    
    
    
    

    
    yy -= accp(DETAIL_CHARACTER_SPACE + DETAIL_BACK_BTN_MARGIN*2.0f);
    
    // NPC 배경
    CCSprite* pSprCharacterBg = CCSprite::create("ui/quest/trace/history_cha_bg.png");
    pSprCharacterBg->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprCharacterBg->setPosition(ccp(accp(15.0f), yy));
    pSprCharacterBg->setTag(3);
    layer->addChild(pSprCharacterBg, 0);
    
    
    // NPC 이미지
    string npcImg_path = CCFileUtils::sharedFileUtils()->getDocumentPath();
//    string npcImg_path = "ui/cha/";
    npcImg_path.append(MainScene::getInstance()->getNpc(rivalInfo->npc_id)->npcImagePath);
    
    CCSprite* npcImg = CCSprite::create(npcImg_path.c_str());
    CCSize npcImgSize = npcImg->getTexture()->getContentSizeInPixels();
    float cardScale = (float)480 / npcImgSize.height;
    npcImg->setScale(cardScale);
    npcImg->setAnchorPoint(ccp(0.0f, 0.0f));
    npcImg->setPosition(ccp(accp(-50.0f), yy - accp(250.0f)));
    layer->addChild(npcImg, 0);
    
    yy -= accp(BACKGROUND_STROKE_THICKNESS);
    
    CCSprite* pSprBgStroke = CCSprite::create("ui/quest/trace/history_cha_stroke.png");
    pSprBgStroke->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprBgStroke->setPosition(ccp(accp(10.0f), yy));
    pSprBgStroke->setTag(6);
    layer->addChild(pSprBgStroke, 1);
    
    yy -= accp(RIVAL_STATBAR_HEIGHT);
    

    // 라이벌 상태 바
    CCSprite* pSprRivalStatBg = CCSprite::create("ui/quest/trace/trace_gage_bg.png");

    pSprRivalStatBg->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprRivalStatBg->setPosition(ccp(accp(10.0f), yy));
    pSprRivalStatBg->setTag(7);
    layer->addChild(pSprRivalStatBg, 2);
    

    // 라이벌 게이지 슬래시
    CCSprite* pSprHpGaugeSlash = CCSprite::create("ui/quest/trace/trace_level_gage_slash.png");
    pSprHpGaugeSlash->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprHpGaugeSlash->setPosition(ccp(accp(RIVAL_HP_GAUGE_LENGTH / 2.0f + 27.0f), yy + accp(60.5f)));
    pSprHpGaugeSlash->setScale(1.7f);
    layer->addChild(pSprHpGaugeSlash, 3);
    
    // 라이벌 게이지
    refreshRivalHp(layer, yy);
    refreshRivalMaxHp(layer, yy);
    loadRivalHpGauge(layer);
    refreshRivalHpGauge(layer, yy);

    // 라이벌 시간
    CCSprite* pSprRivalTime = CCSprite::create("ui/quest/trace/trace_gage_rival_time.png");
    pSprRivalTime->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprRivalTime->setPosition(ccp(accp(13.0f), yy));
    pSprRivalTime->setTag(8);
    layer->addChild(pSprRivalTime, 1);
    
    // 라이벌 정보
    string npcRivalInfo = "";
    NpcInfo* npc = MainScene::getInstance()->getNpc(rivalInfo->npc_id);
    char levelBuf[5];
    sprintf(levelBuf, "%d", rivalInfo->npc_lv);
    npcRivalInfo.append("Lv").append(levelBuf).append(". ").append(npc->npcName).append(" ");
    CCLabelTTF* pLblRivalInfo = CCLabelTTF::create(npcRivalInfo.c_str(), "HelveticaNeue-BoldItalic", 13.5f);
    pLblRivalInfo->setColor(COLOR_YELLOW);
    pLblRivalInfo->setTag(9);
    registerLabel(layer, ccp(1.0f, 0.0f), ccp(size.width - accp(130.0f), yy + accp(7.0f)), pLblRivalInfo, 2);
    
    // 라이벌 마크
    CCSprite* rivalMark = CCSprite::create("ui/quest/trace/trace_gage_rival_vs.png");
    rivalMark->setAnchorPoint(ccp(0.0f, 0.0f));
    rivalMark->setPosition(ccp(size.width/2.0f + accp(195.0f), yy));
    rivalMark->setTag(10);
    layer->addChild(rivalMark, 2);
    

    // NPC 가리개
    CCLayerColor* curtain = CCLayerColor::create(ccc4(0.0f, 0.0f, 0.0f, 255.0f), size.width-accp(20.0f), accp(-70.0f));
    curtain->setAnchorPoint(ccp(0.0f, 0.0f));
    curtain->setPosition(accp(10.0f), yy + accp(50.0f));
    layer->addChild(curtain, 0);
    
    yy -= accp(DETAIL_BACK_BTN_MARGIN);

/*  // 라이벌에 대한 동료 정보를 가져오는 부분 참고
 
    for(int j=0;j<rivalInfo->colleagues->count();j++)
    {
        AReceivedColleague *collInfo = (AReceivedColleague*)rivalInfo->colleagues->objectAtIndex(j);
        CCLog("collInfo->name : %s", collInfo->name);
        CCLog("collInfo->imgUrl : %s", collInfo->imgUrl);
        CCLog("collInfo->damages: %d",collInfo->damages);
    }
*/
    

//    qsort(rivalInfo->colleagues, sizeof(rivalInfo->colleagues)/sizeof(AReceivedColleague), sizeof(AReceivedColleague), TraceDetailLayer::ACDSort);
    
    std::sort(rivalInfo->colleagues->data->arr, rivalInfo->colleagues->data->arr + rivalInfo->colleagues->data->num, TraceDetailLayer::compare);
    
    // 서버에서 데이터를 가져오는 루틴
    for (int i=0; i<colleagueCount; i++)
    {
        yy -= accp(DETAIL_BACK_BTN_MARGIN + COLLEAGUE_CELL_HEIGHT);
        AReceivedColleague* collInfo = (AReceivedColleague* )rivalInfo->colleagues->objectAtIndex(i);
        
        makeColleagueCell(layer, collInfo, yy, i);
    }
    
    //////////////////////// RID test
    /*
    char buf[10];
    sprintf(buf, "%d",rivalInfo->rid);
    string textRid = "RID=";
    textRid.append(buf);
    CCLabelTTF* pSprRid = CCLabelTTF::create(textRid.c_str(), "HelveticaNeue-Bold", 14);
    pSprRid->setColor(COLOR_WHITE);
    pSprRid->setTag(701);
    registerLabel(layer, ccp(0.0f, 0.0f), ccp(100,50), pSprRid, 110);
     */
    /////////////////////////////////////////////////
}

int TraceDetailLayer::compare(const void* n1, const void* n2)
{
    AReceivedColleague* num1 = ((AReceivedColleague* )n1);
    AReceivedColleague* num2 = ((AReceivedColleague* )n2);
    
    return num1->damages > num2->damages;
}

void TraceDetailLayer::cbBackBtn()
{

    soundButton1();
    
    if (this->isTouchEnabled()==false)return;
    
    this->removeAllChildrenWithCleanup(true);
    MainScene::getInstance()->removeChild(this, true);
    
    TraceHistoryLayer::getInstance()->callbackFromDetail();
/*
    // for test
    rewardCards = new CCArray();
    for(int i=0;i<3;i++){
        CardInfo *cardInfo = new CardInfo();
        cardInfo->autorelease();
        cardInfo->setId(30011+i);
        CardInfo* newCard = PlayerInfo::getInstance()->makeCard(cardInfo->getId(), cardInfo);
        PlayerInfo::getInstance()->addToMyCardList(newCard);
        rewardCards->addObject(newCard);
    }
    
    //CC_SAFE_DELETE(cardInfo);
    startCardAction();
*/

}


void TraceDetailLayer::cbReward(CCObject* pSender)
{
    CCLog(" ==============================================");
    CCLog(" requestReward, owner:%lld birth:%d rid:%d bRewardreceived,%d",rivalInfo->ownerUserID,rivalInfo->birth,rivalInfo->rid, rivalInfo->bRewardReceived);
    CCLog(" ==============================================");
    
    soundButton1();
    rewardCards = new CCArray();
    ResponseRivalReward* rewardInfo = ARequestSender::getInstance()->requestRivalReward(this->rivalInfo->rid);
    if (atoi(rewardInfo->res) == 0){
        for(int i=0;i<rewardInfo->rewardCards->count();i++){
            QuestRewardCardInfo* rewardCard = (QuestRewardCardInfo*)rewardInfo->rewardCards->objectAtIndex(i);
            
            CardInfo *cardInfo = new CardInfo();
            cardInfo->autorelease();
            cardInfo->setId(rewardCard->card_id);
            cardInfo->setSrl(rewardCard->card_srl);
            cardInfo->setExp(rewardCard->card_exp);
            cardInfo->setLevel(rewardCard->card_level);
            cardInfo->setAttack(rewardCard->card_attack);
            cardInfo->setDefence(rewardCard->card_defense);
            cardInfo->setSkillEffect(rewardCard->card_skillEffect);
        
            CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
            PlayerInfo::getInstance()->addToMyCardList(newCard);
            rewardCards->addObject(newCard);
            //CC_SAFE_DELETE(cardInfo);
        }
        
        //popupOk("보상 카드를 받았습니다.");
        if (rewardInfo->rewardCards->count() >0){
            startCardAction();
        }
    }
    else{
        popupNetworkError(rewardInfo->res, "reward receive error","");
    }
}





void TraceDetailLayer::cbDetail()
{
    soundButton1();
    
    if (this->isTouchEnabled()==false)return;
    
    old_layer_y = this->getPositionY();
    this->setTouchEnabled(false);
    
    CCMenu* pMenuBackBtn = (CCMenu* )this->getChildByTag(0)->getChildByTag(3);
    pMenuBackBtn->setEnabled(false);
    CCMenu* pMenuDetailBtn = (CCMenu* )this->getChildByTag(0)->getChildByTag(2200);
    pMenuDetailBtn->setEnabled(false);

    MainScene::getInstance()->removeUserStatLayer();
    
    NpcInfo *npc = MainScene::getInstance()->getNpc(this->rivalInfo->npc_id);
    TraceLayer* traceLayer = new TraceLayer(this->rivalInfo->rid, npc, this->rivalInfo->type, this->rivalInfo);
    traceLayer->setContentSize(this->getContentSize());
    traceLayer->setAnchorPoint(ccp(0,0));
    traceLayer->setPosition(ccp(0,0));
    traceLayer->setTag(1010);
    this->addChild(traceLayer,1000);
    this->setTouchEnabled(false);
    
}

void TraceDetailLayer::callBackFromTraceLayer()
{
    
    ///////////////////
    //
    // 추적 배경 음악 정지
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG->stopBackgroundMusic();
    }
    
    ///////////////////
    //
    // 메인 배경 음악 재생
    
    soundMainBG();
    
    
    
    
    
    this->removeChildByTag(1010, true);
    this->setPositionY(old_layer_y);
    this->setTouchEnabled(true);
    MainScene::getInstance()->initUserStatLayer();
    refresh();
    this->removeAllChildrenWithCleanup(true);
    this->init();
}



void TraceDetailLayer::timeCounter()
{
    time_t curTime = time(NULL);
    
    float yy;
    if (this->getChildByTag(0)->getChildByTag(701) != NULL) {
        yy = this->getChildByTag(0)->getChildByTag(701)->getPositionY();
        this->getChildByTag(0)->removeChildByTag(701, true);
    }
    

//    this->getChildByTag(0)->removeChildByTag(801, true);
//    this->getChildByTag(0)->removeChildByTag(901, true);
/*
    battleRemainTime->setSec(battleRemainTime->getSec() - 1);
    if (battleRemainTime->getSec() == -1)
    {
        battleRemainTime->setSec(59);
    }
    
    if (battleRemainTime->getSec() == 59)
    {
        battleRemainTime->setMin(battleRemainTime->getMin() - 1);
        if (battleRemainTime->getMin() == -1)
        {
            battleRemainTime->setMin(59);
        }
        
        if (battleRemainTime->getMin() == 59)
            battleRemainTime->setHour(battleRemainTime->getHour() - 1);
    }
*/
    
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    
    int lim_sec  = localtime((time_t* )&(rivalInfo->limit))->tm_sec;
    int lim_min  = localtime((time_t* )&(rivalInfo->limit))->tm_min;
    int lim_hour = localtime((time_t* )&(rivalInfo->limit))->tm_hour;
    
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
    
//    string strHour = "";
//    char bufHour[5];
    sprintf(buf, "%d", time_offset_h);
    if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
//    CCLabelTTF* pLblHour = CCLabelTTF::create(strHour.c_str(), "HelveticaNeue-Bold", 14);
//    pLblHour->setColor(COLOR_GRAY);
//    pLblHour->setTag(701);
//    registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.0f, 0.0f), ccp(accp(170.0f), yy), pLblHour, 110);

    // min

//    string strMin = "";
//    char bufMin[5];
    sprintf(buf, "%d", time_offset_m);
    if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
//    CCLabelTTF* pLblMin = CCLabelTTF::create(strMin.c_str(), "HelveticaNeue-Bold", 14);
//    pLblMin->setColor(COLOR_GRAY);
//    pLblMin->setTag(801);
//    registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.0f, 0.0f), ccp(accp(170.0f + 55.0f), yy), pLblMin, 110);
    
    // sec
    
//    string strSec = "";
//    char bufSec[3];
    sprintf(buf, "%d", time_offset_s);
    if (time_offset_s<10)   time.append("0").append(buf);
    else time.append(buf);
    
    CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
    pLblTime->setColor(COLOR_GRAY);
    pLblTime->setTag(701);
    registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.0f, 0.0f), ccp(accp(170.5f), yy), pLblTime, 110);
    
    if ((rivalInfo->cur_hp <= 0) || (rivalInfo->limit <= curTime))//((time_offset_h == 0) && (time_offset_m == 0) && (time_offset_s == 0)) || rivalInfo->cur_hp <= 0)
    {
        this->unschedule(schedule_selector(TraceDetailLayer::timeCounter));
//        delete battleRemainTime;
        CCLog("Time Out!");
        //        normalBattleStep = 20;
        
        //        hitGauge[1]->stopAllActions();
        //        hitGauge[2]->stopAllActions();
        
        //        TraceLayer::getInstance()->normalBattleResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 1, false);
        //        actionNormalTimeout();
        

        // 결과 갱신
        refresh();
        
/*
        //////////
        //
        // 좌표 저장
        
        CCPoint backLblPos   = this->getChildByTag(0)->getChildByTag(1)->getPosition();
        CCPoint backMenuPos  = this->getChildByTag(0)->getChildByTag(3)->getPosition();
        // 상세 결과
        CCPoint detailPos    = this->getChildByTag(0)->getChildByTag(2200)->getPosition();
        // 시간 종료
        CCPoint timeLimitPos = this->getChildByTag(0)->getChildByTag( 701)->getPosition();
        
        ////////////////////
        //
        // 기존 스프라이트 삭제
        
        // 상세 결과
        this->getChildByTag(0)->removeChildByTag(2200, true);   // 결과 스프라이트
        this->getChildByTag(0)->removeChildByTag(2300, true);   // 결과 라벨
        // 시간 종료
        this->getChildByTag(0)->removeChildByTag( 701, true);
        
        ////////////////////////
        //
        // 갱신된 스프라이트로 그리기
        
        // 상세 결과
        CCSprite* pSprDetail;
        if ((rivalInfo->cur_hp < 0)) {
            pSprDetail = CCSprite::create("ui/battle/battle_duel_result_lose.png");
        } else {
            pSprDetail = CCSprite::create("ui/battle/battle_duel_result_win.png");
            
            this->getChildByTag(0)->removeChildByTag(1, true);  // 뒤로 가기 라벨
            this->getChildByTag(0)->removeChildByTag(3, true);  // 뒤로 가기 메뉴
            
            CCMenuItemImage* pSprNewBackBtn = CCMenuItemImage::create("ui/quest/trace/history_btn_back_a1.png", "ui/quest/trace/history_btn_back_a2.png", this, menu_selector(TraceDetailLayer::cbBackBtn));
            pSprNewBackBtn->setAnchorPoint(ccp(0.0f, 0.0f));
            pSprNewBackBtn->setPosition(ccp(0.0f, 0.0f));
            
            CCLabelTTF* pLblNewBackBtn = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 12);
            pLblNewBackBtn->setColor(COLOR_YELLOW);
            pLblNewBackBtn->setTag(1);
            registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.5f, 0.0f), ccp(backLblPos.x/2.0f+accp(10.0f), backLblPos.y), pLblNewBackBtn, 1);
            
            CCMenuItemImage* pSprNewReward = CCMenuItemImage::create("ui/quest/trace/history_btn_reward_a1.png", "ui/quest/trace/history_btn_reward_a2.png");
            pSprNewReward->setAnchorPoint(ccp(0.0f, 0.0f));
            pSprNewReward->setPosition(ccp(this->getContentSize().width/2.0f, 0.0f));
            
            CCLabelTTF* pLblNewReward = CCLabelTTF::create("보상 받기", "HelveticaNeue-Bold", 12);
            pLblNewReward->setColor(COLOR_WHITE);
            pLblNewReward->setTag(2);
            registerLabel((CCLayer* )this->getChildByTag(0), ccp(0.5f, 0.0f), ccp(this->getContentSize().width/2.0f + backLblPos.x/2.0f+accp(10.0f), backLblPos.y), pLblNewReward, 1);
            
            CCMenu* pNewMenu = CCMenu::create(pSprNewBackBtn, pSprNewReward, NULL);
            pNewMenu->setAnchorPoint(ccp(0.0f, 0.0f));
            pNewMenu->setPosition(ccp(backMenuPos.x, backMenuPos.y));
            pNewMenu->setTag(3);
            this->getChildByTag(0)->addChild(pNewMenu, 0);
        }
        pSprDetail->setAnchorPoint(ccp(0.0f, 0.0f));
        pSprDetail->setPosition(ccp(detailPos.x - accp(245.0f), detailPos.y - accp(60.0f)));
        pSprDetail->setTag(2200);
        this->getChildByTag(0)->addChild(pSprDetail, 10);
    
    

        // 시간 종료
        CCLabelTTF* pLblTimeLimit = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
        pLblTimeLimit->setPosition(ccp(timeLimitPos.x + accp(22.0f), timeLimitPos.y + accp(18.0f)));
        pLblTimeLimit->setColor(COLOR_GRAY);
        pLblTimeLimit->setTag(2100);
        this->getChildByTag(0)->addChild(pLblTimeLimit, 2);
 */
    }
}
/* // 호가 짜려다가 안된 리프레시 함수
void TraceDetailLayer::refreshTraceResult()
{
    ResponseRivalList* rivalListinfo = ARequestSender::getInstance()->requestRivalList();
    
    for (int i=0; i<rivalListinfo->rivals->count(); i++)
    {
        if (rivalInfo == rivalListinfo->rivals->objectAtIndex(i))
        {
            this->removeAllChildrenWithCleanup(true);
            
            TraceHistoryLayer::getInstance()->initTraceDetailLayer((AReceivedRival* )rivalListinfo->rivals->objectAtIndex(i));
        }
    }
}
*/
void TraceDetailLayer::refreshRivalHp(CCLayerColor* layer, int yy)
{
    removeRivalHp(layer);
    int x = RIVAL_HP_GAUGE_LENGTH / 2.0f + 10.0f;
    float scale = 1.0f;
    char buffer[10];
    sprintf(buffer, "%d", rivalInfo->cur_hp);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan=0; scan<length; scan++)
    {
        int number = (value % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
        rivalHp[scan] = createNumber(number, ccp(accp(x), yy + accp(52.5f)), scale);
        layer->addChild(rivalHp[scan], 3);
        CCSize size = rivalHp[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 1;
        rivalHp[scan]->setPosition(ccp(accp(x), yy + accp(52.5f)));
    }
}

void TraceDetailLayer::refreshRivalMaxHp(CCLayerColor* layer, int yy)
{
    removeRivalMaxHp(layer);
    bool skip = true;
    int x = RIVAL_HP_GAUGE_LENGTH / 2.0f + 27.0f + 20.0f;
    float scale = 1.0f;
    for (int scan=9; scan>-1; scan--)
    {
        int number = (rivalInfo->max_hp % ((int)powf(10.0f, scan+1.0f))) / (int)(pow(10, scan));
        if (number >  0)
            skip = false;
        if (skip)
            continue;
        rivalMaxHp[scan] = createNumber(number, ccp(accp(x), yy + accp(52.5f)), scale);
        layer->addChild(rivalMaxHp[scan], 3);
        CCSize size = rivalMaxHp[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceDetailLayer::removeRivalHp(CCLayerColor* layer)
{
    for (int scan=0; scan<10; scan++)
    {
        if (rivalHp[scan] != NULL)
            layer->removeChild(rivalHp[scan], true);
        rivalHp[scan] = NULL;
    }
}

void TraceDetailLayer::removeRivalMaxHp(CCLayerColor* layer)
{
    for (int scan=0; scan<10; scan++)
    {
        if (rivalMaxHp[scan] != NULL)
            layer->removeChild(rivalMaxHp[scan], true);
        rivalMaxHp[scan] = NULL;
    }
}

void TraceDetailLayer::loadRivalHpGauge(CCLayerColor* layer)
{
    char buffer[64];
    for (int scan=0; scan<2; scan++)
    {
        sprintf(buffer, "ui/quest/trace/trace_gage_yellow%d.png", scan+2);
        rivalHpGauge[scan] = CCSprite::create(buffer);
        rivalHpGauge[scan]->setAnchorPoint(ccp(0.0f, 0.0f));
        layer->addChild(rivalHpGauge[scan], 2);
    }
}

void TraceDetailLayer::refreshRivalHpGauge(CCLayerColor* layer, int yy)
{
    float ratio = (float)rivalInfo->cur_hp / (float)rivalInfo->max_hp;
    if (ratio > 1)ratio=1;
    
    CCLog("Rival cur_hp : %f", (float)rivalInfo->cur_hp);
    CCLog("Rival max_hp : %f", (float)rivalInfo->max_hp);
    for (int scan=0; scan<2; scan++)
        rivalHpGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
//    float x = 17;
//    rivalHpGauge[0]->setPosition(ccp(accp(x), yy+accp(1.0f) + accp(48.0f)));
    
    float x = 17.0f;
    rivalHpGauge[0]->setPosition(ccp(accp(x), yy + accp(48.0f)));
    rivalHpGauge[0]->setScaleX(ratio * (RIVAL_HP_GAUGE_LENGTH + 11.0f));
    x += (ratio * (RIVAL_HP_GAUGE_LENGTH + 11.0f));
    rivalHpGauge[1]->setPosition(ccp(accp(x), yy + accp(48.0f)));
}

void TraceDetailLayer::makeColleagueCell(CCLayerColor* layer, AReceivedColleague *pColleague, int yy, int tag)
{
    CCSprite* pSprColleagueBg = CCSprite::create("ui/quest/trace/history_bg_progress02.png");
    pSprColleagueBg->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprColleagueBg->setPosition(ccp(accp(10), yy));//795.0f - 118.0f));
    layer->addChild(pSprColleagueBg, 0);
    
    CCSprite* pSprRanking = CCSprite::create("ui/quest/trace/history_ranking.png");
    pSprRanking->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprRanking->setPosition(ccp(accp(236.0f), yy + accp(133.0f)));
    layer->addChild(pSprRanking, 0);
    
    string ranking = "";
    char rank[5];
    sprintf(rank, "%d", tag+1);
    ranking.append(rank).append("등");
    CCLabelTTF* pLblRanking = CCLabelTTF::create(ranking.c_str(), "HelveticaNeue-Bold", 13);
    pLblRanking->setAnchorPoint(ccp(0.0f, 0.0f));
    pLblRanking->setPosition(ccp(accp(236.0f) + accp(130.0f), yy + accp(130.0f)));
    pLblRanking->setColor(COLOR_GRAY);
    layer->addChild(pLblRanking, 0);
    
    
    CCLabelTTF* pLblNickname = CCLabelTTF::create("닉네임", "HelveticaNeue", 12);
    pLblNickname->setColor(COLOR_ORANGE);
    registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(236.0f), yy + accp(86.0f-13)), pLblNickname, 0);
    
    string nickname ="";
    if (pColleague->userid == PlayerInfo::getInstance()->userID) {
        nickname = PlayerInfo::getInstance()->myNickname;
    }
    else {
        nickname = PlayerInfo::getInstance()->getGameUserNickname(pColleague->userid);
    }

    /*
    CCLabelTTF* pLblUserNick = CCLabelTTF::create(nickname.c_str(), "HelveticaNeue", 12);//pColleague->name, "HelveticaNeue", 12);
    pLblUserNick->setColor(COLOR_GRAY);
    registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), yy + accp(86.0f-13)), pLblUserNick, 0);
    */
    
    TextClippingLayer *nameLayer = new TextClippingLayer(nickname.c_str());
    nameLayer->setAnchorPoint(ccp(0,0));
    nameLayer->setPosition(ccp(accp(236.0f + 110.0f), yy + accp(86.0f-13)));
    nameLayer->setClipX(accp(600));
    layer->addChild(nameLayer,0);
    
    

    CCLabelTTF* pLblDamage = CCLabelTTF::create("데미지", "HelveticaNeue", 12);

    pLblDamage->setColor(COLOR_ORANGE);
    registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(236.0f), yy + accp(51.0f-23)), pLblDamage, 0);
    
    string damage;
    char buf[10];
    sprintf(buf, "%d", pColleague->damages);
    damage.append(buf);
    CCLabelTTF* pLblDamageValue = CCLabelTTF::create(damage.c_str(), "HelveticaNeue", 12);
    pLblDamageValue->setColor(COLOR_YELLOW);
    registerLabel(layer, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), yy + accp(51.0f-23)), pLblDamageValue, 0);
    
/*
    string userImgUrl = "";
    if (pRival->ownerUserID == PlayerInfo::getInstance()->userID) {
        userImgUrl = PlayerInfo::getInstance()->profileImageUrl;
    } else {
        userImgUrl = PlayerInfo::getInstance()->getGameUserProfileURL(pRival->ownerUserID);
    }
    
    // 먼저 리소스를 받아야 함
    // 내부에 있는 이미지를 스프라이트로 그리는 것임
    CCSprite* pSprUserImg = CCSprite::create(userImgUrl.c_str());
    if (pSprUserImg != NULL)
    {
        float cardScale = (float)140.0f / pSprUserImg->getTexture()->getContentSizeInPixels().height;
        pSprUserImg->setScale(cardScale);
        regSprite(this, ccp(0.0f, 0.0f), ccp(accp(127.0f), yy*SCREEN_ZOOM_RATE + accp(13.0f)), pSprUserImg, 110);
    }
  */
    
    // 동료 이미지
    string userImgUrl = "";
    if (pColleague->userid == PlayerInfo::getInstance()->userID) {
        userImgUrl = PlayerInfo::getInstance()->profileImageUrl;
    } else {
        userImgUrl = PlayerInfo::getInstance()->getGameUserProfileURL(pColleague->userid);
    }
    CCLog("userImgUrl : %s", userImgUrl.c_str());
    
    std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(userImgUrl.c_str());
    if (filename.size() > 0)
    {
        if (FileManager::sharedFileManager()->IsProfileFileExist(filename.c_str()))
        {
            std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + filename;
            
            //CCLog(" DocumentPath:%s", DocumentPath.c_str());
            
            CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
            CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
            
            if (pSprFace != NULL){
                CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
                float cardScale = (float)90 / aa.height;
                pSprFace->setScale(cardScale);
                regSprite(layer, ccp(0,0), ccp(accp(25.0f), yy + accp(60.0f)), pSprFace, 100);
            }
        }
        else
        {
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            std::vector<std::string> downloads;
            downloads.push_back(PlayerInfo::getInstance()->getGameUserProfileURL(pColleague->userid));
            requestor->addDownloadTask(downloads, this, callfuncND_selector(BattlePlayerCell::profileImgDownloaded));
        }
    }

    // 동료 레벨
    string userLevel = "";
    char levelBuf[5];
    sprintf(levelBuf, "%d", pColleague->user_lv);
    userLevel.append("Lv. ").append(levelBuf);
    
    CCLabelTTF* level = CCLabelTTF::create(userLevel.c_str(), "HelveticaNeue-BoldItalic", 12);
    level->setAnchorPoint(ccp(0.5f, 0.5f));
    level->setPosition(ccp(accp(72.0f), yy + accp(32.0f)));
    level->setColor(COLOR_GRAY);
    layer->addChild(level, 100);
    
    CCLayerColor* levelBg = CCLayerColor::create(ccc4(0.0f, 0.0f, 0.0f, 150.0f), accp(92.0f), accp(38.0f));
    levelBg->setAnchorPoint(ccp(0.0f, 0.0f));
    levelBg->setPosition(ccp(accp(24.0f), yy + accp(12.0f)));
    layer->addChild(levelBg, 5);
    
    // 동료의 리더 카드
//    CCLog("Colleague's Leadercard ID : %d", pColleague->leaderCard_id);
    CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(pColleague->leaderCard_id);
    if (cardInfo != NULL)
    {
        ACardMaker* cardMaker = new ACardMaker();
        cardMaker->MakeCardThumb(layer, cardInfo, ccp(127.0f, yy*SCREEN_ZOOM_RATE + 13.0f), 140, 110, 10); // MakeCardThumb 함수 안에서 accp 함수로 처리하고 있음
    }
}

void TraceDetailLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;

    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void TraceDetailLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL 좌표로 변경합니다.
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, CARD_CLOSE_BTN, localPoint))
    {
        //CCLOG("confirm");
        
        soundButton1();
        
        this->removeChild(cardDetailViewLayer, true);
        this->removeChildByTag(CARD_CLOSE_BTN, true);
        this->removeChildByTag(LABEL, true);
        
        showCardCnt++;
        
        if (showCardCnt >= rewardCards->count()){
            closeCardAction();
        }
        else{
            showRewardCard();
            return;
            // 이 이후의 동작을 못하게 한다. stopallAction되면 안됨.
        }
    }
    
    float y = this->getPositionY();

    if(LayerStartPos>0)
    {
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }

    if(LayerStartPos<0)
    {
        if (y < 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > this->getChildByTag(0)->getContentSize().height - 400.0f)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, this->getChildByTag(0)->getContentSize().height - 400.0f)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    bTouchMove = false;
    
    
}

void TraceDetailLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float distance = fabs(startPosition.y - location.y);
    if (distance > 5.0f)
        bTouchMove = true;
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    //printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
    if (distance < 15.0f && timeDelta > 50.0f)
    {
        moving = false;
        startPosition = location;
        startTime = currentTime;
    }
    else if (distance > 5.0f)
        moving = true;
    if (moving)
    {
        distance = fabs(lastPosition.y - location.y);
        timeDelta = (currentTime.tv_usec - lastTime.tv_usec) / 1000.0f + (currentTime.tv_sec - lastTime.tv_sec) * 1000.0f;
        if (distance < 15.0f && timeDelta > 50.0f)
        {
            moving = false;
            startPosition = location;
            startTime = currentTime;
        }
    }
    
    lastPosition = location;
    lastTime = currentTime;
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        CCLog("추적 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
}

void TraceDetailLayer::cbRefresh()
{
    if (this->isTouchEnabled()==false)return;
    
    refresh();
}


void TraceDetailLayer::refresh()
{
    long long myOnwerId = this->rivalInfo->ownerUserID;
    int myRid = this->rivalInfo->rid;
    int myEventBirth = this->rivalInfo->birth;
    TraceHistoryLayer::getInstance()->refreshRivalListInfo();
    CCLog("before refresh, rivalInfo:onwerid :%lld Rid:%d birth:%d", this->rivalInfo->ownerUserID, this->rivalInfo->rid, this->rivalInfo->birth);
    
    this->rivalInfo = TraceHistoryLayer::getInstance()->getRivalInfo(myOnwerId, myRid, myEventBirth);
    
    if (this->rivalInfo != NULL){
        this->removeAllChildrenWithCleanup(true);
        this->init();
    }
    else{
        CCLog("cannot find my Rival event");
        
        //////////
        //
        // 좌표 저장
        
        // 상세 결과
        CCPoint detailPos    = this->getChildByTag(0)->getChildByTag(2200)->getPosition();
        // 시간 종료
        CCPoint timeLimitPos = this->getChildByTag(0)->getChildByTag( 701)->getPosition();
        
        ////////////////////
        //
        // 기존 스프라이트 삭제

        // 상세 결과
//        this->getChildByTag(0)->removeChildByTag(1, true);  // 뒤로 가기 라벨
//        this->getChildByTag(0)->removeChildByTag(3, true);  // 뒤로 가기 메뉴
        // 상세 결과
        this->getChildByTag(0)->removeChildByTag(701, true);
        // 시간 종료
        this->getChildByTag(0)->removeChildByTag(2200, true);

        
        ////////////////////////
        //
        // 갱신된 스프라이트로 그리기
        
        // 상세 결과
        CCSprite* pSprDetail = CCSprite::create("ui/battle/battle_duel_result_lose.png");
        pSprDetail->setAnchorPoint(ccp(0.0f, 0.0f));
        pSprDetail->setPosition(ccp(detailPos.x - accp(245.0f), detailPos.y - accp(60.0f)));
        pSprDetail->setTag(2200);
        this->getChildByTag(0)->addChild(pSprDetail, 10);
        
        // 시간 종료
        CCLabelTTF* pLblTimeLimit = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
        pLblTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
        pLblTimeLimit->setPosition(ccp(timeLimitPos.x + accp(22.0f), timeLimitPos.y + accp(18.0f)));
        pLblTimeLimit->setColor(COLOR_GRAY);
        pLblTimeLimit->setTag(2100);
        this->getChildByTag(0)->addChild(pLblTimeLimit, 2);
        CCLog("The End!");
    }
}

void TraceDetailLayer::startCardAction()
{
    CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
    pSpr0->setAnchorPoint(ccp(0,0));
    pSpr0->setPosition( ccp(0,0) );
    pSpr0->setTag(88);
    //MainScene::getInstance()->addChild(pSpr0, 10001);
    this->addChild(pSpr0, 90);
    
    showCardCnt = 0;
    showRewardCard();
}

void TraceDetailLayer::closeCardAction()
{
    restoreTouchDisable();
    UserStatLayer::getInstance()->setVisible(true);
    this->removeChildByTag(88, true);
}

void TraceDetailLayer::showRewardCard()
{
    UserStatLayer::getInstance()->setVisible(false);
    
    CCSize size = GameConst::WIN_SIZE;
    
    CardInfo* card = (CardInfo*)rewardCards->objectAtIndex(showCardCnt);
    
    cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
    cardDetailViewLayer->setScale(0.0f);
    this->addChild(cardDetailViewLayer, 100);
    
    //CC_SAFE_DELETE(card);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 0.80f);
    cardDetailViewLayer->runAction(CCSequence::actions(actionScale1, NULL));
    
    CCCallFunc* call_Fade4 = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceDetailLayer::FadeWhite04));
    this->runAction(call_Fade4);
    
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
    CCCallFunc* call_btn = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceDetailLayer::CreateBtn));
    this->runAction(CCSequence::actions(delay, call_btn, NULL));
}

void TraceDetailLayer::FadeWhite04()
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
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceDetailLayer::RemoveFade04));
    whiteBG->runAction(CCSequence::actions(fadeout, callWhite, NULL));
}
void TraceDetailLayer::RemoveFade04()
{
    this->removeChildByTag(FADE_04, true);
}

void TraceDetailLayer::CreateBtn()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* btn = CCSprite::create("ui/shop/popup_btn_c1.png");
    btn->setAnchorPoint(ccp(0.5f, 0.5f));
    btn->setPosition((ccp(size.width/2, accp(60.0f))));
    btn->setTag(CARD_CLOSE_BTN);
    this->addChild(btn, 200);
    
    CCLabelTTF* confirm = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue", 13);
    confirm->setColor(COLOR_YELLOW);
    confirm->setTag(LABEL);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, accp(60.0f)), confirm, 201);
}


