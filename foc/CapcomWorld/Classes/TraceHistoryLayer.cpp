//
//  TraceHistoryLayer.cpp
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 12..
//
//

#include "TraceHistoryLayer.h"
#include "MainScene.h"
#include "ResponseBasic.h"
#include "TraceLayer.h"
#include "CCHttpRequest.h"
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif
#include <algorithm>

TraceHistoryLayer* TraceHistoryLayer::instance = NULL;



void TraceHistoryLayer::init()
{
    
    //const int RIVAL_CELL_HEIGHT = 166;
    
    this->removeAllChildrenWithCleanup(true);
    
    this->setContentSize(CCSize(GameConst::WIN_SIZE.width, GameConst::WIN_SIZE.height - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN)));
    
    CCSize size = this->getContentSize();
    
    // 서버에서 정보를 받는 루틴
    
    refreshRivalListInfo();
    
    /*
     // for test
     for(int i=0;i<5;i++){
     AReceivedRival *rival = new AReceivedRival();
     rival->type = 0;
     rival->max_hp = 500;
     rival->cur_hp = 400;
     rival->npc_lv = 5;
     rival->npc_id = 1001;
     rival->rid = 1181;
     rival->birth = 1362030971;
     rival->limit = 1362031571;
     rival->ownerUserID = 724070835002638447;
     rival->bRewardReceived = true;
     
     rivalListinfo->rivals->addObject(rival);
     }
     */
    
    traceHistoryScrollLayer = new TraceHistoryScrollLayer(size);
    traceHistoryScrollLayer->setTouchEnabled(true);
    this->addChild(traceHistoryScrollLayer,0);
    
    
    traceHistoryMenuLayer = new TraceHistoryMenuLayer(size);
    traceHistoryMenuLayer->setAnchorPoint(ccp(0,0));
    traceHistoryMenuLayer->setPosition(ccp(0,0));//this->getPositionY()));
    traceHistoryMenuLayer->setTag(909);
    this->addChild(traceHistoryMenuLayer,201);
    
    //CheckLayerSize(this);
    
    
}



void TraceHistoryLayer::closeLayer()
{
    
    if (MainScene::getInstance()->getChildByTag(586)){
        
        MainScene::getInstance()->removeChildByTag(586, true);
        MainScene::getInstance()->removeChild(this, true);
        
        if (DojoLayerDojo::getInstance()){
            DojoLayerDojo::getInstance()->ShowMenu();
            DojoLayerDojo::getInstance()->setTouchEnabled(true);
            MainScene::getInstance()->ShowMainMenu();
            this->removeAllChildrenWithCleanup(true);
        }
        
        MainScene::getInstance()->setRivalListRefresh();
    }
    
    //DojoLayerDojo::getInstance()->bSkipRefreshRivalList = false;
}

void TraceHistoryLayer::cbBackBtn()//CCObject* pSender)
{
    soundButton1();
    
    DojoLayerDojo::getInstance()->refreshRivalNotiUI(rivalListinfo);
    
    traceHistoryScrollLayer->removeAllChildrenWithCleanup(true);
    
    this->removeChildByTag(909, true);
    //removeChild(traceHistoryMenuLayer, true);
    removeChild(traceHistoryScrollLayer, true);
    
    closeLayer();
    
}

void TraceHistoryLayer::cbReward()//CCObject* pSender)
{
    soundButton1();
    
    CCLog("cbReward 1111");
    PlayerInfo::getInstance()->LogTeamInfo();
    
    ResponseRivalReward* rewardInfo = ARequestSender::getInstance()->requestRivalReward(-1);
    if (atoi(rewardInfo->res) == 0){
        if (rewardInfo->rewardCards->count()==0){
            popupOk("받을 카드가 없습니다.");
        }
        else{
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
                
                CCLog("cbReward 2222");
                PlayerInfo::getInstance()->LogTeamInfo();
                
                PlayerInfo::getInstance()->addToMyCardList(newCard);
                
                CCLog("cbReward 3333");
                PlayerInfo::getInstance()->LogTeamInfo();
                
                CC_SAFE_DELETE(cardInfo);
            }
            
            CCLog("cbReward 4444");
            PlayerInfo::getInstance()->LogTeamInfo();
            
            char buf[10];
            sprintf(buf,"%d",rewardInfo->rewardCards->count());
            string text = "";
            text.append(buf).append("장의 보상 카드를 받았습니다.");
            popupOk(text.c_str());//"보상 카드를 받았습니다.");
        }
        
    }
    else{
        popupNetworkError(rewardInfo->res, "reward receive error","");
    }
}

/*
 void TraceHistoryLayer::callProfileImg(cocos2d::CCObject *pSender, void *data)
 {
 HttpResponsePacket *response = (HttpResponsePacket *)data;
 
 if(response->request->reqType == kHttpRequestDownloadFile)
 {
 if (response->succeed) {
 std::vector<std::string>::iterator iter;
 for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter) {
 std::string url = *iter;
 
 std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(url);
 
 registerProfileImg(filename);
 
 }
 }
 else {
 
 }
 }
 }
 
 void TraceHistoryLayer::registerProfileImg(std::string filename)
 {
 std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + filename;
 CCSize size = GameConst::WIN_SIZE;
 CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
 
 PortraitUrl *portrait = (PortraitUrl*)portraitDic.objectForKey(filename);
 
 if (pSprFace != NULL && portrait)
 {
 CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
 float cardScale = (float)78 / aa.height;
 pSprFace->setScale(cardScale);
 regSprite( this, ccp(0, 0), accp(26, portrait->y + 20), pSprFace, 3000);
 }
 }
 



void TraceHistoryLayer::timeTest()
{
    time_t limit = 1360850400;//1360846800;// 1360904400;
    time_t current = time(NULL);
    
    //CCLog("%s", asctime(localtime(&limit)));
    
    int cur_sec = localtime(&current)->tm_sec;
    int cur_min = localtime(&current)->tm_min;
    int cur_hour = localtime(&current)->tm_hour;
    
    int lim_sec = localtime(&limit)->tm_sec;
    int lim_min = localtime(&limit)->tm_min;
    int lim_hour = localtime(&limit)->tm_hour;
    
    int cur_total_sec = cur_sec + cur_min*60 + cur_hour * 3600;
    int lit_total_sec = lim_sec + lim_min*60 + lim_hour * 3600;
    
    int time_offset = lit_total_sec - cur_total_sec;
    int time_offset_h = time_offset/3600;
    time_offset = time_offset%3600;
    int time_offset_m = time_offset/60;
    time_offset = time_offset%60;
    int time_offset_s =  time_offset;
    
    
    CCLog("--------------------------------------------------");
    CCLog("lim :  %d %d %d", lim_hour, lim_min, lim_sec);
    CCLog("cur :  %d %d %d", cur_hour, cur_min, cur_sec);
    CCLog("--  :  %d %d %d", time_offset_h, time_offset_m, time_offset_s);
}
*/

bool TraceHistoryLayer::refreshRivalListInfo()
{
    bool isExistedAliveRival = false;
    
    rivalListinfo = ARequestSender::getInstance()->requestRivalList();
    for (int i=0; i<rivalListinfo->rivals->count(); i++)
    {
        if ((((AReceivedRival* )rivalListinfo->rivals->objectAtIndex(i))->cur_hp > 0) && (((AReceivedRival* )rivalListinfo->rivals->objectAtIndex(i))->limit - time(NULL)))
        {
            isExistedAliveRival = true;
        }
    }
    TraceHistoryScrollLayer::getInstance()->sortDescByLimitTime();
    
    return isExistedAliveRival;
}

AReceivedRival* TraceHistoryLayer::getRivalInfo(long long _ownerId, int _rid, int _birth)
{
    CCLog(" getRivalInfo, owner:%lld rid:%d birth:%d", _ownerId, _rid, _birth);
    
    for(int i=0;i<rivalListinfo->rivals->count();i++){
        AReceivedRival* rivalInfo = (AReceivedRival*)rivalListinfo->rivals->objectAtIndex(i);
        
        CCLog("getRivalInfo, rivalInfo:onwerid :%lld Rid:%d birth:%d", rivalInfo->ownerUserID, rivalInfo->rid, rivalInfo->birth);
        
        if (rivalInfo->ownerUserID == _ownerId && rivalInfo->rid == _rid && rivalInfo->birth == _birth){
            CCLog(" getRivalInfo, rivalInfo index:%d", i);
            return rivalInfo;
        }
    }
    CCLog(" cannot find rival info");
    return NULL;
}

void TraceHistoryLayer::callbackFromDetail()
{
    this->removeAllChildrenWithCleanup(true);
    this->setVisible(true);
    this->setTouchEnabled(true);
    init();
}

/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////
//
//                                  TextClippingLayer
//
/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////


TextClippingLayer::TextClippingLayer(const char* _text){
    this->text = _text;
    this->setClipsToBounds(true);
    
    CCLabelTTF* pLabel = CCLabelTTF::create(this->text, "HelveticaNeue", 12);
    pLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0,0), ccp(0,0), pLabel, 100);
    
    clip_w = GameConst::WIN_SIZE.width;
}

TextClippingLayer::~TextClippingLayer()
{
    
}

void TextClippingLayer::setClipX(int _w)
{
    clip_w = _w;
}

void TextClippingLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = 0;//accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT + 30);
    int clip_h = winSize.height;
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, clip_w, clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
    
}

/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////
//
//                              TraceHistoryMenuLayer
//
/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////


TraceHistoryMenuLayer* TraceHistoryMenuLayer::instance = NULL;

TraceHistoryMenuLayer::TraceHistoryMenuLayer(CCSize layerSize)
{
    this->setContentSize(layerSize);
    instance = this;
    init();
}

TraceHistoryMenuLayer::~TraceHistoryMenuLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void TraceHistoryMenuLayer::init()
{
    
    CCSize size = this->getContentSize();
    int yy = getContentSize().height;
    
    //CheckLayerSize(this);
    
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png", CCRectMake(0,0,size.width, accp(RIVAL_HISTORY_BACK_BTN_SPACE+MAIN_LAYER_TOP_MARGIN)));
    pSprBG->setAnchorPoint(ccp(0,1));
    pSprBG->setPosition(ccp(0.0f, yy+accp(MAIN_LAYER_TOP_MARGIN)));
    this->addChild(pSprBG, 0);
    
    //CheckLayerSize(this);
    yy -= accp(RIVAL_HISTORY_BACK_BTN_SPACE);
    
    CCMenuItemImage* pSprBackBtn = CCMenuItemImage::create("ui/quest/trace/history_btn_back_a1.png", "ui/quest/trace/history_btn_back_a2.png", this, menu_selector(TraceHistoryMenuLayer::cbBackBtn));
    pSprBackBtn->setAnchorPoint(ccp(0.0f, 0));
    pSprBackBtn->setPosition(ccp(0.0f, 0.0f));
    pSprBackBtn->setTag(1);
    
    CCLabelTTF* pLblBackBtn = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 12);
    pLblBackBtn->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(158.0f + 10.0f), yy + accp(10.0f)), pLblBackBtn, 100);
    
    CCMenuItemImage* pSprReward = CCMenuItemImage::create("ui/quest/trace/history_btn_reward_a1.png", "ui/quest/trace/history_btn_reward_a2.png", this, menu_selector(TraceHistoryMenuLayer::cbReward));
    pSprReward->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprReward->setPosition(ccp(size.width/2.0f, 0.0f));
    pSprReward->setTag(2);
    
    CCLabelTTF* pLblReward = CCLabelTTF::create("보상 모두 받기", "HelveticaNeue-Bold", 12);
    pLblReward->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(315.0f + 168.0f), yy + accp(10.0f)), pLblReward, 100);
    
    CCMenu* pMenu = CCMenu::create(pSprBackBtn, pSprReward, NULL);
    pMenu->setAnchorPoint(ccp(0.0f, 0.0f));
    pMenu->setPosition(ccp(accp(10.0f), yy));
    pMenu->setTag(3);
    this->addChild(pMenu, 90);
}

void TraceHistoryMenuLayer::cbBackBtn(CCObject* pSender)
{
    this->removeAllChildrenWithCleanup(true);
    TraceHistoryLayer::getInstance()->cbBackBtn();
}


void TraceHistoryMenuLayer::cbReward(CCObject* pSender)
{
    TraceHistoryLayer::getInstance()->cbReward();
}



/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////
//
//                                  TraceHistoryScrollLayer
//
/////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////

TraceHistoryScrollLayer* TraceHistoryScrollLayer::instance = NULL;

void TraceHistoryScrollLayer::init()
{
    
    
    CCSize size = this->getContentSize();
    
    this->setClipsToBounds(true);
    
    // 서버에서 정보를 받는 루틴
    const int rivalCount = TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count();
    const int layer_clip_height = GameConst::WIN_SIZE.height - accp(MAIN_LAYER_TOP_UI_HEIGHT) - accp(MAIN_LAYER_TOP_MARGIN) - accp(RIVAL_HISTORY_BACK_BTN_SPACE);
    
    //float LayerHeight =  accp(RIVAL_HISTORY_BACK_BTN_SPACE*3 + RIVAL_CELL_HEIGHT * rivalCount +  RIVAL_HISTORY_BACK_BTN_MARGIN * rivalCount);
    //float LayerHeight =  accp(RIVAL_HISTORY_BACK_BTN_SPACE*2 + RIVAL_CELL_HEIGHT * rivalCount +  RIVAL_HISTORY_BACK_BTN_MARGIN * rivalCount);
    float LayerHeight =  accp(RIVAL_HISTORY_BACK_BTN_SPACE + RIVAL_CELL_HEIGHT * rivalCount +  RIVAL_HISTORY_BACK_BTN_MARGIN * rivalCount);
    
    if (LayerHeight < layer_clip_height){
        LayerHeight = layer_clip_height;
    }
    
    LayerStartPos = layer_clip_height - LayerHeight;
    
    this->setContentSize(CCSize(size.width, LayerHeight));
    this->setPosition(ccp(0.0f, LayerStartPos));
    
    int yy = getContentSize().height;
    
    //yy -= accp(RIVAL_HISTORY_BACK_BTN_SPACE);
    
    if (rivalCount > 0){
        
        sortDescByLimitTime();
        /*
         addPageLoading();
         bool startTrace = true;
         
         FileManager* fManager = FileManager::sharedFileManager();
         std::vector<std::string> downloads;
         std::string enemyImgPath = FOC_IMAGE_SERV_URL;
         enemyImgPath.append("images/cha/cha_l/");
         */
        // 서버에서 받은 data로 셀을 만든다!
        
        /*
        for (int i=0; i<rivalCount; i++)
        {
            yy -= accp(RIVAL_HISTORY_BACK_BTN_MARGIN + RIVAL_CELL_HEIGHT);
         
            AReceivedRival* rivalInfo = (AReceivedRival*)TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i);
            makeRivalCell(rivalInfo, yy, i);
        }
        */
        cellMakeY = yy;
        yy -= rivalCount-1 * accp(RIVAL_HISTORY_BACK_BTN_MARGIN + RIVAL_CELL_HEIGHT);
        
        yy -= accp(RIVAL_HISTORY_BACK_BTN_SPACE + RIVAL_HISTORY_BACK_BTN_MARGIN);
    }
    else{
        CCLabelTTF* pLblBackBtn = CCLabelTTF::create("라이벌 이력이 없습니다.", "HelveticaNeue-Bold", 12);
        pLblBackBtn->setColor(COLOR_WHITE);
        registerLabel(this, ccp(0.5f, 0.0f), ccp(this->getContentSize().width/2, this->getContentSize().height/2), pLblBackBtn, 100);
    }
    
    
    /*
     if (rivalCount > 0){
     
     // list 마지막에 있는 뒤로 가기 버튼
     CCMenuItemImage* pSprBackBtn2 = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png", "ui/card_tab/team/cards_bt_back_a2.png", this, menu_selector(TraceHistoryLayer::cbBackBtn));
     pSprBackBtn2->setAnchorPoint(ccp(0.0f, 0));
     pSprBackBtn2->setPosition(ccp(0.0f, 0.0f));
     pSprBackBtn2->setTag(-1);
     
     CCLabelTTF* pLblBackBtn2 = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 12);
     pLblBackBtn2->setColor(COLOR_YELLOW);
     registerLabel(this, ccp(0.5f, 0.0f), ccp(size.width/2.0f, yy + accp(10.0f)), pLblBackBtn2, 100);
     
     CCMenu* pMenu2 = CCMenu::create(pSprBackBtn2, NULL);
     pMenu2->setAnchorPoint(ccp(0.0f, 0.0f));
     pMenu2->setPosition(ccp(accp(10.0f), yy));
     pMenu2->setTag(-3);
     this->addChild(pMenu2, 90);
     }
     */
    //CheckLayerSize(this);
    
    bMakeCellFinished = false;
    cellMakeCnt = 0;
    if (rivalCount > 0){
        this->schedule(schedule_selector(TraceHistoryScrollLayer::makeCells),0.1, -1, 0);
    }
    
}

void TraceHistoryScrollLayer::makeCells()
{
    
    //CCLog(" cellMakeCnt:%d", cellMakeCnt);
    
    cellMakeY -= accp(RIVAL_HISTORY_BACK_BTN_MARGIN + RIVAL_CELL_HEIGHT);
    AReceivedRival* rivalInfo = (AReceivedRival*)TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(cellMakeCnt);
    
    //CCLog(" makecell 10");
    
    makeRivalCell(rivalInfo, cellMakeY, cellMakeCnt);
    
    //CCLog(" makecell 20");
    
    cellMakeCnt++;
    if (cellMakeCnt == TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count()){
        // cell make end
        this->unschedule(schedule_selector(TraceHistoryScrollLayer::makeCells));
        bMakeCellFinished = true;
    }
    else{
    }
    
    
    //CCLog(" makecell 30");
}


/*
 void TraceHistoryScrollLayer::onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data)
 {
 HttpResponsePacket* response = (HttpResponsePacket* )data;
 if (response->request->reqType == kHttpRequestDownloadFile)
 {
 CCLog("trace history image download complete");
 removePageLoading();
 
 makeRivalCell(rivalInfo, rivalInfoYy, rivalInfoNo);
 }
 }
 */
void TraceHistoryScrollLayer::sortDescByLimitTime()
{
    std::sort(TraceHistoryLayer::getInstance()->rivalListinfo->rivals->data->arr, TraceHistoryLayer::getInstance()->rivalListinfo->rivals->data->arr + TraceHistoryLayer::getInstance()->rivalListinfo->rivals->data->num, TraceHistoryScrollLayer::compare);
}

int TraceHistoryScrollLayer::compare(const void* n1, const void* n2)
{
    AReceivedRival* num1 = ((AReceivedRival* )n1);
    AReceivedRival* num2 = ((AReceivedRival* )n2);
    
    return num1->limit > num2->limit;
}



void TraceHistoryScrollLayer::makeRivalCell(AReceivedRival* pRival, int yy, int tag)
{
    
    //CCLog("makeRivalCell 0");
    
    time_t current = time(NULL);
    
    ///////////////////
    //
    // 상황에 따라 다른 것
    
    // 셀 배경
    CCSprite* pSprRivalBG;
    // 시간
    CCSprite* pSprTimeLimit;
    // 결과
    CCSprite* pSprResultBtn;
    CCLabelTTF* pLblResultBtn;
    // 발견자 색깔
    CCLabelTTF* pLblFinder = CCLabelTTF::create("발견자", "HelveticaNeue", 12);
    // 라이벌 색깔
    CCLabelTTF* pLblRival = CCLabelTTF::create("라이벌", "HelveticaNeue", 12);
    // 상황 색깔
    CCLabelTTF* pLblStatus = CCLabelTTF::create("상황", "HelveticaNeue", 12);
    CCLabelTTF* pLblStatusResult;
    
    //CCLog("makeRivalCell 10");
    
    if(((pRival->limit - current) <= 0) || ((pRival->cur_hp <= 0)))     // 상황이 종료된 경우
    {
        // 셀 배경
        pSprRivalBG = CCSprite::create("ui/quest/trace/history_bg_end.png");
        // 시간
        pSprTimeLimit = CCSprite::create("ui/quest/trace/history_time_tint.png");
        CCLabelTTF* timeReulst = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
        timeReulst->setColor(COLOR_GRAY);
        timeReulst->setAnchorPoint(ccp(0.0f, 0.0f));
        timeReulst->setPosition(ccp(accp(355.0f), yy + accp(128.0f)));
        timeReulst->setTag(tag+801);
        this->addChild(timeReulst, 105);
        // 결과 버튼
        pSprResultBtn = CCSprite::create("ui/shop/list_btn1.png");
        pLblResultBtn = CCLabelTTF::create("기록", "HelveticaNeue-Bold", 12);
        pLblResultBtn->setColor(COLOR_YELLOW);
        // 발견자 색깔
        pLblFinder->setColor(COLOR_GRAY);
        // 라이벌 색깔
        pLblRival->setColor(COLOR_GRAY);
        // 상황 색깔
        pLblStatus->setColor(COLOR_GRAY);
        if ((pRival->cur_hp) <= 0) {
            pLblStatusResult = CCLabelTTF::create("승리", "HelveticaNeue", 12);
        } else {
            pLblStatusResult = CCLabelTTF::create("패배", "HelveticaNeue", 12);
        }
    }
    else    // 대결 중인 경우
    {
        // 셀 배경
        pSprRivalBG = CCSprite::create("ui/quest/trace/history_bg_progress.png");
        // 시간
        pSprTimeLimit = CCSprite::create("ui/quest/trace/history_time.png");
        
        makeTimer(pRival->limit, current, yy, tag);
        
        // 결과
        pSprResultBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btl_btn.png");
        pLblResultBtn = CCLabelTTF::create("결투", "HelveticaNeue-Bold", 12);
        pLblResultBtn->setColor(COLOR_WHITE);
        // 발견자 색깔
        pLblFinder->setColor(COLOR_ORANGE);
        // 라이벌 색깔
        pLblRival->setColor(COLOR_ORANGE);
        // 상황 색깔
        pLblStatus->setColor(COLOR_ORANGE);
        pLblStatusResult = CCLabelTTF::create("결투중", "HelveticaNeue", 12);
        
        //        CCLog("Limit Time   : %s", asctime(localtime((time_t*)&pRival->limit)));
        //        CCLog("Current Time : %s", asctime(localtime(&current)));
    }
    
    //CCLog("makeRivalCell 20");
    
    // 셀 배경
    pSprRivalBG->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprRivalBG->setPosition(ccp(accp(10), yy));//795.0f - 118.0f));
    pSprRivalBG->setTag(tag+1000);
    this->addChild(pSprRivalBG, 100);
    
    // 시간
    pSprTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprTimeLimit->setPosition(ccp(accp(236.0f), yy + accp(133.0f)));
    pSprTimeLimit->setTag(tag+1500);
    this->addChild(pSprTimeLimit, 100);
    
    // 결과
    pSprResultBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprResultBtn->setPosition(ccp(accp(520.0f), yy + accp(40.0f)));
    pSprResultBtn->setTag(tag+2000);
    //    CCLog("pSprResultBtn Y Pos : %f", pSprResultBtn->getPositionY()); // 20
    this->addChild(pSprResultBtn, 100);
    
    pLblResultBtn->setTag(tag+2500);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(520.0f) + accp(24.0f), yy + accp(40.0f) + accp(25.0f)), pLblResultBtn, 100);
    
    //CCLog("makeRivalCell 30");
    
    // NPC 이미지
    string npcImgUrl = CCFileUtils::sharedFileUtils()->getDocumentPath();
    npcImgUrl.append(MainScene::getInstance()->getNpc(pRival->npc_id)->npcImagePath);
    CCSprite* pSprNpcImg = CCSprite::create(npcImgUrl.c_str());
    
    //CCLog("makeRivalCell 33");
    if (pSprNpcImg != NULL)
    {
        float cardScale = (float)160.0f / pSprNpcImg->getTexture()->getContentSizeInPixels().height;
        pSprNpcImg->setScale(cardScale);
        regSprite(this, ccp(0.0f, 0.0f), ccp(accp(30.0f), yy + accp(5.0f)), pSprNpcImg, 110);
    }
    
    
    // 발견자
    pLblFinder->setTag(tag+3000);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), yy + accp(86.0f)), pLblFinder, 100);
    
    string nickname ="";
    if (pRival->ownerUserID == PlayerInfo::getInstance()->userID) {
        //nickname = PlayerInfo::getInstance()->myNickname;
        nickname = PlayerInfo::getInstance()->displayName;
    }
    else {
        nickname = PlayerInfo::getInstance()->getGameUserNickname(pRival->ownerUserID);
    }
    
        
    TextClippingLayer *nameLayer = new TextClippingLayer(nickname.c_str());
    nameLayer->setAnchorPoint(ccp(0,0));
    nameLayer->setPosition(ccp(accp(236.0f + 110.0f), yy + accp(86.0f)));
    nameLayer->setClipX(accp(500));
    TraceHistoryScrollLayer::getInstance()->addChild(nameLayer,105);
    /*
     CCLabelTTF* pLblFinderId = CCLabelTTF::create(nickname.c_str(), "HelveticaNeue", 12);
     pLblFinderId->setColor(COLOR_GRAY);
     registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), yy + accp(86.0f)), pLblFinderId, 105);
     */
    // 라이벌
    pLblRival->setTag(tag+3500);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), yy + accp(51.0f)), pLblRival, 100);
    
    string npcRivalInfo = "";
    NpcInfo* npc = MainScene::getInstance()->getNpc(pRival->npc_id);
    char levelBuf[5];
    sprintf(levelBuf, "%d", pRival->npc_lv);
    npcRivalInfo.append(npc->npcName).append("(Lv").append(levelBuf).append(")");
    CCLabelTTF* pLblRivalInfo = CCLabelTTF::create(npcRivalInfo.c_str(), "HelveticaNeue", 12);
    pLblRivalInfo->setColor(COLOR_GRAY);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), yy + accp(51.0f)), pLblRivalInfo, 105);
    
    
    // 상황
    pLblStatus->setTag(tag+4000);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), yy + accp(16.0f)), pLblStatus, 100);
    
    pLblStatusResult->setTag(tag+4500);
    pLblStatusResult->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), yy + accp(16.0f)), pLblStatusResult, 100);
    
}

void TraceHistoryScrollLayer::makeTimer(time_t limitTime, time_t curTime, int yy, int tag)
{
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    
    int lim_sec  = localtime(&limitTime)->tm_sec;
    int lim_min  = localtime(&limitTime)->tm_min;
    int lim_hour = localtime(&limitTime)->tm_hour;
    
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
    
    //CheckLayerSize(this);
    
    //    Time* userTime = new Time(localtime(&limitTime)->tm_hour - localtime(&curTime)->tm_hour, localtime(&limitTime)->tm_min - localtime(&curTime)->tm_min, localtime(&limitTime)->tm_sec - localtime(&curTime)->tm_sec);
    this->schedule(schedule_selector(TraceHistoryScrollLayer::battleTimeCounter), 1.0f);
    
    string time = "";
    char buf[2];
    // hour
    
    //    string strHour = "";
    //    char bufHour[1];
    sprintf(buf, "%d", time_offset_h);
    if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
    //    CCLabelTTF* pLblHour = CCLabelTTF::create(strHour.c_str(), "HelveticaNeue-Bold", 14);
    //    pLblHour->setColor(COLOR_RED);
    //    pLblHour->setTag(tag+701);
    //    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f), yy + accp(128.0f)), pLblHour, 110);
    
    // min
    
    //    string strMin = "";
    //    char bufMin[3];
    sprintf(buf, "%d",  time_offset_m);
    if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
    else time.append(buf).append(" : ");
    
    //    CCLabelTTF* pLblMin = CCLabelTTF::create(strMin.c_str(), "HelveticaNeue-Bold", 14);
    //    pLblMin->setColor(COLOR_RED);
    //    pLblMin->setTag(tag+801);
    //    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f + 55.0f), yy + accp(128.0f)), pLblMin, 110);
    
    // sec
    
    //    string strSec = "";
    //    char bufSec[3];
    sprintf(buf, "%d", time_offset_s);
    if (time_offset_s<10)   time.append("0").append(buf);
    else time.append(buf);
    
    CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
    pLblTime->setColor(COLOR_RED);
    pLblTime->setTag(tag+701);
    //    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f + 110.0f), yy + accp(128.0f)), pLblSec, 110);
    registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.5f), yy + accp(128.0f)), pLblTime, 110);
    
}

void TraceHistoryScrollLayer::battleTimeCounter()
{
    //    time_t limit = 1360904400;
    
    time_t curTime = time(NULL);
    
    int yy[TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count()];
    /*
     for (int i=0; i<TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count(); i++)
     {
     if (((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->cur_hp <= 0
     || ((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit <= curTime)   // 상황이 종료된 경우
     {
     removeChildByTag(701 + i, true);
     continue;
     }
     
     if (getChildByTag(701 + i) != NULL){
     yy[i] = getChildByTag(701 + i)->getPositionY();
     removeChildByTag(701 + i, true);
     }
     }
     */
    for (int i=0; i<TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count(); i++)
    {
        if (((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->cur_hp <= 0
            || ((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit <= curTime)
        {
            if (getChildByTag(701 + i) != NULL)
            {
                CCLog("it's not null!");
                
                this->unschedule(schedule_selector(TraceHistoryScrollLayer::battleTimeCounter));
                
                CCLog("Time Out!");
                
                //////////
                //
                // 좌표 저장
                
                // 셀 배경
                int RivalBgY      = this->getChildByTag(i+1000)->getPositionY();
                // 시간
                int timeLimitY    = this->getChildByTag(i+1500)->getPositionY();
                int timeResultY   = this->getChildByTag(i+ 701)->getPositionY();
                // 결과
                int resultBtnY    = this->getChildByTag(i+2000)->getPositionY();
                int resultLblY    = this->getChildByTag(i+2500)->getPositionY();
                // 발견자
                int finderY       = this->getChildByTag(i+3000)->getPositionY();
                // 라이벌
                int rivalY        = this->getChildByTag(i+3500)->getPositionY();
                // 상황
                int statusY       = this->getChildByTag(i+4000)->getPositionY();
                int statusResultY = this->getChildByTag(i+4500)->getPositionY();
                
                ///////////////////
                //
                // 기존 스프라이트 제거
                
                // 셀 배경
                this->removeChildByTag(i+1000, true);
                // 시간
                this->removeChildByTag(i+1500, true);
                this->removeChildByTag(i+ 701, true);
                // 결과
                this->removeChildByTag(i+2000, true);
                this->removeChildByTag(i+2500, true);
                // 발견자
                this->removeChildByTag(i+3000, true);
                // 라이벌
                this->removeChildByTag(i+3500, true);
                // 상황
                this->removeChildByTag(i+4000, true);
                this->removeChildByTag(i+4500, true);
                
                //////////////////////////
                //
                // 종료 상황으로 스프라이트 갱신
                
                // 셀 배경
                CCSprite* pSprRivalBG = CCSprite::create("ui/quest/trace/history_bg_end.png");
                pSprRivalBG->setAnchorPoint(ccp(0.0f, 0.0f));
                pSprRivalBG->setPosition(ccp(accp(10), RivalBgY));//795.0f - 118.0f));
                pSprRivalBG->setTag(i+1000);
                this->addChild(pSprRivalBG, 100);
                // 시간
                CCSprite* pSprTimeLimit = CCSprite::create("ui/quest/trace/history_time_tint.png");
                pSprTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
                pSprTimeLimit->setPosition(ccp(accp(236.0f), timeLimitY));
                pSprTimeLimit->setTag(i+1500);
                this->addChild(pSprTimeLimit, 100);
                
                CCLabelTTF* timeReulst = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
                timeReulst->setColor(COLOR_GRAY);
                timeReulst->setAnchorPoint(ccp(0.0f, 0.0f));
                timeReulst->setPosition(ccp(accp(355.0f), timeResultY));
                timeReulst->setTag(i+801);
                this->addChild(timeReulst, 105);
                // 결과
                CCSprite* pSprResultBtn = CCSprite::create("ui/shop/list_btn1.png");
                pSprResultBtn->setAnchorPoint(ccp(0.0f, 0.0f));
                pSprResultBtn->setPosition(ccp(accp(520.0f), resultBtnY));
                pSprResultBtn->setTag(i+2000);  // 태그를 100 단위로 지정했기 때문에 이력이 100개 이상일 경우 버그 발생
                this->addChild(pSprResultBtn, 100);
                
                CCLabelTTF* pLblResultBtn = CCLabelTTF::create("기록", "HelveticaNeue-Bold", 12);
                pLblResultBtn->setColor(COLOR_YELLOW);
                pLblResultBtn->setTag(i+2500);
                registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(520.0f) + accp(24.0f), resultLblY), pLblResultBtn, 100);
                // 발견자 색깔
                CCLabelTTF* pLblFinder = CCLabelTTF::create("발견자", "HelveticaNeue", 12);
                pLblFinder->setColor(COLOR_GRAY);
                pLblFinder->setTag(i+3000);
                registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), finderY), pLblFinder, 100);
                // 라이벌 색깔
                CCLabelTTF* pLblRival = CCLabelTTF::create("라이벌", "HelveticaNeue", 12);
                pLblRival->setColor(COLOR_GRAY);
                pLblRival->setTag(i+3500);
                registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), rivalY), pLblRival, 100);
                // 상황 색깔
                CCLabelTTF* pLblStatus = CCLabelTTF::create("상황", "HelveticaNeue", 12);
                pLblStatus->setColor(COLOR_GRAY);
                pLblRival->setTag(i+4000);
                registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), statusY), pLblStatus, 100);
                
                CCLabelTTF* pLblStatusResult;
                if (((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->cur_hp <= 0) {
                    pLblStatusResult = CCLabelTTF::create("승리", "HelveticaNeue", 12);
                } else {
                    pLblStatusResult = CCLabelTTF::create("패배", "HelveticaNeue", 12);
                }
                pLblStatusResult->setTag(i+4500);
                pLblStatusResult->setColor(COLOR_YELLOW);
                registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), statusResultY), pLblStatusResult, 100);
                
                continue;
            }
            else
            {
                removeChildByTag(701 + i, true);
                continue;
            }
        }
        
        if (getChildByTag(701 + i) != NULL){
            yy[i] = getChildByTag(701 + i)->getPositionY();
            removeChildByTag(701 + i, true);
        }
        
        int cur_sec  = localtime(&curTime)->tm_sec;
        int cur_min  = localtime(&curTime)->tm_min;
        int cur_hour = localtime(&curTime)->tm_hour;
        
        int lim_sec  = localtime((time_t* )&((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit)->tm_sec;
        int lim_min  = localtime((time_t* )&((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit)->tm_min;
        int lim_hour = localtime((time_t* )&((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit)->tm_hour;
        
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
        
        
        
        
        
        /*
         // 24시간이 0으로 처리되는 부분에 대한 테스트 코드
         
         int test_cur_sec  = localtime(&curTime)->tm_sec;
         int test_cur_min  = localtime(&curTime)->tm_min;
         int test_cur_hour = localtime(&curTime)->tm_hour;
         
         int testErrorTime = 1363176300;
         int test_lim_sec = localtime((time_t* )&testErrorTime)->tm_sec;
         int test_lim_min = localtime((time_t* )&testErrorTime)->tm_min;
         int test_lim_hour = localtime((time_t* )&testErrorTime)->tm_hour;
         if (test_lim_hour == 21) {
         test_lim_hour = 0;
         }
         if (test_cur_hour == 21) {
         test_cur_hour = 0;
         }
         
         if (test_lim_hour < test_cur_hour)
         {
         test_lim_hour += 24;
         }
         
         int test_cur_total_sec = test_cur_sec + test_cur_min*60 + test_cur_hour * 3600;
         int test_lit_total_sec = test_lim_sec + test_lim_min*60 + test_lim_hour * 3600;
         
         int test_time_offset = test_lit_total_sec - test_cur_total_sec;
         int test_time_offset_h = test_time_offset/3600;
         test_time_offset = test_time_offset%3600;
         int test_time_offset_m = test_time_offset/60;
         test_time_offset = test_time_offset%60;
         int test_time_offset_s =  test_time_offset;
         
         
         CCLog("test_lim_hour : %d / test_lim_min : %d / test_lim_sec : %d", test_lim_hour, test_lim_min, test_lim_sec);
         CCLog("%d : %d : %d", test_time_offset_h, test_time_offset_m, test_time_offset_s);
         */
        
        
        
        
        
        string time = "";
        char buf[2];
        // hour
        
        //        string strHour = "";
        //        char bufHour[5];
        sprintf(buf, "%d", time_offset_h);
        if (time_offset_h<10)   time.append("0").append(buf).append(" : ");
        else time.append(buf).append(" : ");
        
        //        CCLabelTTF* pLblHour = CCLabelTTF::create(strHour.c_str(), "HelveticaNeue-Bold", 14);
        //        pLblHour->setColor(COLOR_RED);
        //        pLblHour->setTag(i+701);
        //        registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f), yyPos[i]), pLblHour, 110);
        
        // min
        
        //        string strMin = "";
        //        char bufMin[5];
        sprintf(buf, "%d", time_offset_m);
        if (time_offset_m<10)   time.append("0").append(buf).append(" : ");
        else time.append(buf).append(" : ");
        
        //        CCLabelTTF* pLblMin = CCLabelTTF::create(strMin.c_str(), "HelveticaNeue-Bold", 14);
        //        pLblMin->setColor(COLOR_RED);
        //        pLblMin->setTag(i+801);
        //        registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f + 55.0f), yyPos[i]), pLblMin, 110);
        
        // sec
        
        //        string strSec = "";
        //        char bufSec[3];
        sprintf(buf, "%d", time_offset_s);
        if (time_offset_s<10)   time.append("0").append(buf);
        else time.append(buf);
        
        CCLabelTTF* pLblTime = CCLabelTTF::create(time.c_str(), "HelveticaNeue-Bold", 14);
        pLblTime->setColor(COLOR_RED);
        pLblTime->setTag(i+701);
        //        registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.0f + 110.0f), yyPos[i]), pLblSec, 110);
        registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(355.5f), yy[i]), pLblTime, 110);
        
        /*
         AReceivedRival* rival = (AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i);
         CCLog("no : %d / cur_hp : %d / lim - cur : %d", i, rival->cur_hp, rival->limit - curTime);
         if ((((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->cur_hp <= 0) || (((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->limit <= curTime))   // 상황 종료 후 처리 사항
         {
         this->unschedule(schedule_selector(TraceHistoryScrollLayer::battleTimeCounter));
         //            delete battleRemainTime;
         CCLog("Time Out!");
         //        normalBattleStep = 20;
         
         //        hitGauge[1]->stopAllActions();
         //        hitGauge[2]->stopAllActions();
         
         //        TraceLayer::getInstance()->normalBattleResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 1, false);
         //        actionNormalTimeout();
         
         //            this->removeAllChildrenWithCleanup(true);
         //            init();
         
         //////////
         //
         // 좌표 저장
         
         // 셀 배경
         int RivalBgY      = this->getChildByTag(i+1000)->getPositionY();
         // 시간
         int timeLimitY    = this->getChildByTag(i+1100)->getPositionY();
         int timeResultY   = this->getChildByTag(i+ 701)->getPositionY();
         // 결과
         int resultBtnY    = this->getChildByTag(i+3000)->getPositionY();
         int resultLblY    = this->getChildByTag(i+1300)->getPositionY();
         // 발견자
         int finderY       = this->getChildByTag(i+1400)->getPositionY();
         // 라이벌
         int rivalY        = this->getChildByTag(i+1500)->getPositionY();
         // 상황
         int statusY       = this->getChildByTag(i+1600)->getPositionY();
         int statusResultY = this->getChildByTag(i+1700)->getPositionY();
         
         ///////////////////
         //
         // 기존 스프라이트 제거
         
         // 셀 배경
         this->removeChildByTag(i+1000, true);
         // 시간
         this->removeChildByTag(i+1100, true);
         this->removeChildByTag(i+ 701, true);
         // 결과
         this->removeChildByTag(i+3000, true);
         this->removeChildByTag(i+1300, true);
         // 발견자
         this->removeChildByTag(i+1400, true);
         // 라이벌
         this->removeChildByTag(i+1500, true);
         // 상황
         this->removeChildByTag(i+1600, true);
         this->removeChildByTag(i+1700, true);
         
         //////////////////////////
         //
         // 종료 상황으로 스프라이트 갱신
         
         // 셀 배경
         CCSprite* pSprRivalBG = CCSprite::create("ui/quest/trace/history_bg_end.png");
         pSprRivalBG->setAnchorPoint(ccp(0.0f, 0.0f));
         pSprRivalBG->setPosition(ccp(accp(10), RivalBgY));//795.0f - 118.0f));
         pSprRivalBG->setTag(i+1000);
         this->addChild(pSprRivalBG, 100);
         // 시간
         CCSprite* pSprTimeLimit = CCSprite::create("ui/quest/trace/history_time_tint.png");
         pSprTimeLimit->setAnchorPoint(ccp(0.0f, 0.0f));
         pSprTimeLimit->setPosition(ccp(accp(236.0f), timeLimitY));
         pSprTimeLimit->setTag(i+1100);
         this->addChild(pSprTimeLimit, 100);
         
         CCLabelTTF* timeReulst = CCLabelTTF::create("종료", "HelveticaNeue-Bold", 14);
         timeReulst->setColor(COLOR_GRAY);
         timeReulst->setAnchorPoint(ccp(0.0f, 0.0f));
         timeReulst->setPosition(ccp(accp(355.0f), timeResultY));
         timeReulst->setTag(i+701);
         this->addChild(timeReulst, 105);
         // 결과
         CCSprite* pSprResultBtn = CCSprite::create("ui/shop/list_btn1.png");
         pSprResultBtn->setAnchorPoint(ccp(0.0f, 0.0f));
         pSprResultBtn->setPosition(ccp(accp(520.0f), resultBtnY));
         pSprResultBtn->setTag(i+3000);  // 태그를 100 단위로 지정했기 때문에 이력이 100개 이상일 경우 버그 발생
         this->addChild(pSprResultBtn, 100);
         
         CCLabelTTF* pLblResultBtn = CCLabelTTF::create("기록", "HelveticaNeue-Bold", 12);
         pLblResultBtn->setColor(COLOR_YELLOW);
         pLblResultBtn->setTag(i+1300);
         registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(520.0f) + accp(24.0f), resultLblY), pLblResultBtn, 100);
         // 발견자 색깔
         CCLabelTTF* pLblFinder = CCLabelTTF::create("발견자", "HelveticaNeue", 12);
         pLblFinder->setColor(COLOR_GRAY);
         pLblFinder->setTag(i+1400);
         registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), finderY), pLblFinder, 100);
         // 라이벌 색깔
         CCLabelTTF* pLblRival = CCLabelTTF::create("라이벌", "HelveticaNeue", 12);
         pLblRival->setColor(COLOR_GRAY);
         pLblRival->setTag(i+1500);
         registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), rivalY), pLblRival, 100);
         // 상황 색깔
         CCLabelTTF* pLblStatus = CCLabelTTF::create("상황", "HelveticaNeue", 12);
         pLblStatus->setColor(COLOR_GRAY);
         pLblRival->setTag(i+1600);
         registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f), statusY), pLblStatus, 100);
         
         CCLabelTTF* pLblStatusResult;
         if (((AReceivedRival* )TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i))->cur_hp <= 0) {
         pLblStatusResult = CCLabelTTF::create("승리", "HelveticaNeue", 12);
         } else {
         pLblStatusResult = CCLabelTTF::create("패배", "HelveticaNeue", 12);
         }
         pLblStatusResult->setTag(i+1700);
         pLblStatusResult->setColor(COLOR_YELLOW);
         registerLabel(this, ccp(0.0f, 0.0f), ccp(accp(236.0f + 110.0f), statusResultY), pLblStatusResult, 100);
         }
         */
    }
}


void TraceHistoryScrollLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    /*
     const int ChapterCount = UnlockChapterList->count();
     
     for(int i=0; i<ChapterCount; ++i)
     {
     //if(false == bTouchMove)
     {
     if (GetSpriteTouchCheckByTag(this, i, localPoint))
     {
     ChangeSpr(this, i, "ui/quest/quest_btn_a2.png", 100);
     }
     }
     }
     */
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void TraceHistoryScrollLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float y = this->getPositionY();
    
#if (1)
    if (moving == true)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos > 0)
            endPos = 0;
        else if (endPos < LayerStartPos)
            endPos = LayerStartPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceHistoryScrollLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceHistoryScrollLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceHistoryScrollLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceHistoryScrollLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, accp(-30.0f))), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceHistoryScrollLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    
    int a = GameConst::WIN_SIZE.height - accp( MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN + RIVAL_HISTORY_BACK_BTN_SPACE + RIVAL_HISTORY_BACK_BTN_MARGIN);
    
    //CCLog("localPoint.y:%f, location.u:%f, a=%d", localPoint.y, location.y, a);
    
    if (location.y > a)return;
    
    //if (bMakeCellFinished==false)return;
    
    
    // 서버에서 정보를 받는 루틴
    const int rivalCount =  TraceHistoryLayer::getInstance()->rivalListinfo->rivals->count();
    
    for (int i=0; i<rivalCount; ++i)
    {
        if (false == bTouchMove)
        {
            if (GetSpriteTouchCheckByTag(this, i+2000, localPoint))
            {
                soundButton1();
                
                CCLog("i=%d", i);
                AReceivedRival* info = (AReceivedRival*)TraceHistoryLayer::getInstance()->rivalListinfo->rivals->objectAtIndex(i);
                
                initTraceDetailLayer(info);
                
            }
        }
    }
    bTouchMove = false;
}

void TraceHistoryScrollLayer::scrollingEnd()
{
    this->stopAllActions();
	//this->setIsScrolling(false);
}

void TraceHistoryScrollLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
    const int layer_clip_height = GameConst::WIN_SIZE.height - accp(MAIN_LAYER_TOP_UI_HEIGHT) - accp(MAIN_LAYER_TOP_MARGIN);
    if (this->getContentSize().height <= layer_clip_height)return;
    
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
        
        //CCLog("추적 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
        
    }
    
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
}

void TraceHistoryScrollLayer::initTraceDetailLayer(AReceivedRival* rivalInfo)
{
    this->setVisible(false);
    this->setTouchEnabled(false);
    
    CCSize size = GameConst::WIN_SIZE;
    
    CCLog("initTraceDetailLayer, rid:%d, bRewardReceived:%d", rivalInfo->rid, rivalInfo->bRewardReceived);
    
    pTraceDetailLayer = new TraceDetailLayer(rivalInfo, CCSize(size.width, size.height));
    pTraceDetailLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    MainScene::getInstance()->addChild(pTraceDetailLayer, 220);
    pTraceDetailLayer->setTouchEnabled(true);
    
    TraceHistoryMenuLayer::getInstance()->setVisible(false);
    
}



void TraceHistoryScrollLayer::visit()
{
    
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = 0;//accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT + 30);
    int clip_h = winSize.height - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN + RIVAL_HISTORY_BACK_BTN_SPACE);
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, winSize.width, clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}
/*
 void TraceHistoryScrollLayer::preVisitWithClippingRect(CCRect clipRect)
 {
 if (!this->isVisible())// getIsVisible())
 return;
 
 glEnable(GL_SCISSOR_TEST);
 
 //CCDirector *director = CCDirector::sharedDirector();
 CCSize size = GameConst::WIN_SIZE;// director->getWinSize();
 CCPoint origin = this->convertToWorldSpaceAR(clipRect.origin);
 CCPoint topRight =this->convertToWorldSpaceAR(ccpAdd(clipRect.origin, ccp(clipRect.size.width, clipRect.size.height)));
 CCRect scissorRect = CCRectMake(origin.x, origin.y, topRight.x-origin.x, topRight.y-origin.y);
 
 // Handle Retina
 scissorRect = CC_RECT_POINTS_TO_PIXELS(scissorRect);
 
 glScissor((GLint) scissorRect.origin.x, (GLint) scissorRect.origin.y,
 (GLint) scissorRect.size.width, (GLint) scissorRect.size.height);
 }
 
 void TraceHistoryScrollLayer::postVisit()
 {
 glDisable(GL_SCISSOR_TEST);
 }
 */