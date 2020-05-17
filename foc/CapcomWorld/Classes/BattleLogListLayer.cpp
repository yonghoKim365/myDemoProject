//
//  BattleLogListLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#include "BattleLogListLayer.h"
#include "ARequestSender.h"
#include "BattleDuelLayer.h"
#include "DojoLayerDojo.h"
#include "PlayerInfo.h"

BattleLogListLayer* BattleLogListLayer::instance = NULL;

BattleLogListLayer* BattleLogListLayer::getInstance()
{
    if (instance == NULL)
        return NULL;
    return instance;
}

BattleLogListLayer::BattleLogListLayer(CCSize layerSize) : resultLog(NULL)
{
    instance = this;
    this->setClipsToBounds(true);
    yStart = 0;
    bTouchMove = false;
}

BattleLogListLayer::~BattleLogListLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void BattleLogListLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);    
}

void BattleLogListLayer::InitUI()
{
    StartYpos = this->getContentSize().height - 5.0f;
    
    if(noticeInfo)
    {
        const int num = noticeInfo->notices->count();

        for(int i=0; i<num; ++i)
        {
            NoticeInfo *info = (NoticeInfo*)noticeInfo->notices->objectAtIndex(i);
            
            time_t curtime = info->date;
            struct tm *loctime;
            loctime = localtime(&curtime);
            
            char buf[100];
            
            if(loctime->tm_min < 10)
                sprintf(buf, "%d/%d/%d %d:0%d", loctime->tm_year + 1900, loctime->tm_mon + 1, loctime->tm_mday, loctime->tm_hour, loctime->tm_min);
            else
                sprintf(buf, "%d/%d/%d %d:%d", loctime->tm_year + 1900, loctime->tm_mon + 1, loctime->tm_mday, loctime->tm_hour, loctime->tm_min);
            
            // -- 배틀만 표시
            if(0 == info->type)
                MakeCell((IconType)info->result, info->nick, buf, i);
        }
    }
}

int BattleLogListLayer::GetNumOfLog() const
{
    if(noticeInfo)
        return noticeInfo->notices->count();
    else
        return 0;
}

void BattleLogListLayer::MakeCell(IconType logType, const string& name, const string& time, int tag)
{
    CCSprite* pSprBtn = CCSprite::create("ui/home/ui_home_battlelog_btn1.png");
    pSprBtn->setAnchorPoint(ccp(0, 0));
    pSprBtn->setPosition(accp(544, StartYpos + 30));
    pSprBtn->setTag(tag);
    this->addChild(pSprBtn, 60);
    
    CCSprite* pSprIcon = NULL;
    
    std::string result;
    std::string user;
    
    switch (logType)
    {
        case eWIN:      pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_win.png");   user="님에게"; result = "승리 했습니다."; break;
        case eLOSE:     pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_lose.png");  user="님에게"; result = "패배 했습니다."; break;
        case eDRAW:     pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_draw.png");  user="님과"; result = "비겼습니다.";  break;
        case eINVITE:   pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_friend.png");break;
        case eTRADE:    pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_trade.png"); break;
        case eGIFT:     pSprIcon = CCSprite::create("ui/home/ui_home_battlelog_gift.png");  break;
        default:    break;
    }

    pSprIcon->setAnchorPoint(ccp(0, 0));
    pSprIcon->setPosition(accp(25, StartYpos));
    //pSprType->setTag(tag);
    this->addChild(pSprIcon, 60);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create(name.c_str(), "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(143, StartYpos + 64), pLabel1, 60);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create(user.c_str(), "HelveticaNeue-Bold", 12);
    pLabel2->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0, 0), accp(143, StartYpos + 32), pLabel2, 60);
    
    CCLabelTTF* pLabel4 = CCLabelTTF::create(result.c_str(), "HelveticaNeue-Bold", 12);
    pLabel4->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(215, StartYpos + 32), pLabel4, 60);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create(time.c_str(), "HelveticaNeue", 12);
    pLabel3->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0, 0), accp(143, StartYpos), pLabel3, 60);

    StartYpos -= 12;
    
    CCSprite* pLine = CCSprite::create("ui/home/ui_home_bg_line.png");
    pLine->setAnchorPoint(ccp(0, 0));
    pLine->setPosition(accp(15, StartYpos));
    this->addChild(pLine, 60);
    
    StartYpos-=108;
}

void BattleLogListLayer::InitResultLog(ResponseDetailNoticeInfo *detailInfo)
{
    //this->setClipsToBounds(false);
    //this->setTouchEnabled(false);
 
    RemoveResultLog();

    resultLog = new ResultBattleLog(this->getContentSize());
    resultLog->setAnchorPoint(ccp(0.0f, 0.0f));
    resultLog->setPosition(accp(0.0f, 0.0f));
    resultLog->InitUI(detailInfo);
    
    BattleLogLayer* layer = (BattleLogLayer*)this->getParent();
    layer->addChild(resultLog, 1000);
}

void BattleLogListLayer::RemoveResultLog()
{
    if(resultLog)
    {
        //this->setClipsToBounds(true);
        BattleLogLayer* layer = (BattleLogLayer*)this->getParent();
        layer->removeChild(resultLog, true);
        resultLog = NULL;
    }
}

void BattleLogListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
}

void BattleLogListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("배틀 로그 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
    bTouchMove = true;
}

void BattleLogListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);

    float y = this->getPositionY();
    
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleLogListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleLogListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleLogListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleLogListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(false == bTouchMove)
    {
        for(int i=0; i<GetNumOfLog(); ++i)
        {
            if(GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                soundButton1();
                
                NoticeInfo *info = (NoticeInfo*)noticeInfo->notices->objectAtIndex(i);
                ResponseDetailNoticeInfo *detailInfo = ARequestSender::getInstance()->requestDetailNotice(info->noticeID);
                
                if(detailInfo)
                {
                    if (atoi(detailInfo->res)==0){
                        InitResultLog(detailInfo);
                    }
                    else{
                        popupNetworkError(detailInfo->res, detailInfo->msg, "requestDetailNotice");
                    }
                }
            }
        }
    }
    
    bTouchMove = false;
}

void BattleLogListLayer::scrollingEnd()
{
    this->stopAllActions();
}


void BattleLogListLayer::visit()
{
    /*
	if (clipsToBounds)
    {
        CCRect scissorRect = CCRect(0, 101, this->getContentSize().width, 300);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (clipsToBounds)
        glDisable(GL_SCISSOR_TEST);
     */
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT + 30);
    int clip_h = winSize.height - accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN) - accp(50)-accp(34);
    
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

ResultBattleLog::ResultBattleLog(CCSize layerSize)
{
    BattleLogListLayer::getInstance()->setTouchEnabled(false);
    DojoLayerDojo::getInstance()->setTouchEnabled(false);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    this->setContentSize(size);
 
    this->setTouchEnabled(true);
    
    cardMaker = new ACardMaker();
}

ResultBattleLog::~ResultBattleLog()
{
    CC_SAFE_DELETE(cardMaker);
    this->removeAllChildrenWithCleanup(true);
}

void ResultBattleLog::InitUI(ResponseDetailNoticeInfo *detailInfo)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,-50) );
    this->addChild(pSprite, 0);

    int yy = (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_MARGIN;
    
    yy -= 450;
    
    CCSprite* pSpr1 = CCSprite::create("ui/battle_tab/battle_duel_result_info_bg1.png");
    pSpr1->setAnchorPoint(accp(0,0));
    pSpr1->setPosition( accp(10,yy) );
    this->addChild(pSpr1, 100);

    PlayerInfo* pInfo = PlayerInfo::getInstance();

    CardInfo *card = pInfo->GetCardInDeck(0, 0, 2);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->getId());
        
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path += cardInfo->GetSmallBattleImg();
            
            CCSprite* pChar = CCSprite::create(path.c_str());//,160))
            if (pChar != NULL){
                pChar->setScale(0.63f);
                pChar->setAnchorPoint(accp(0,0));
                pChar->setPosition( accp(10, yy+94) );
                addChild(pChar,5);
            }
        }
    }
    
    //CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    DojoLayerDojo* dojo = (DojoLayerDojo*)DojoLayerDojo::getInstance();
    Bg_List* bglist = (Bg_List*)dojo->BgDictionary->objectForKey(pInfo->getBackground());
    
    CCSprite* pSprBg2 = NULL;
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        pSprBg2 = CCSprite::create("ui/main_bg/bg_1.png", CCRectMake(0,accp(112),accp(500),accp(131)));
    }
    else
    {
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=bglist->L_ImgPath;
        
        pSprBg2 = CCSprite::create(path.c_str(),CCRectMake(0,accp(112),accp(500),accp(131)));//,160));
    }
    
    pSprBg2->setScale(1.22f);
    pSprBg2->setAnchorPoint(accp(0,0));
    pSprBg2->setPosition( accp(15,yy+95) ); // -174
    addChild(pSprBg2,0);

    CCSprite* pSpr11 = NULL;
    if(0 == detailInfo->battleResult)
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_win.png");
    else if(1 == detailInfo->battleResult)
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_lose.png");
    else if(2 == detailInfo->battleResult)
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_draw.png");
    
    pSpr11->setAnchorPoint(accp(0,0));
    pSpr11->setPosition( accp(237,yy+114) );
    this->addChild(pSpr11, 6);
    
    CCLabelTTF* pLabel10 = CCLabelTTF::create("획득코인"  , "HelveticaNeue", 12);
    pLabel10->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(58, yy + 45), pLabel10, 10);
    
    char buf0[10];
    sprintf(buf0, "%d", detailInfo->reward_coin);
    
    CCLabelTTF* pLabel11 = CCLabelTTF::create(buf0  , "HelveticaNeue-Bold", 12);
    pLabel11->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(151, yy + 45), pLabel11, 10);
    
    CCLabelTTF* pLabel12 = CCLabelTTF::create("뺏어온 명성"  , "HelveticaNeue", 12);
    pLabel12->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(290, yy + 45), pLabel12, 10);
    
    char buf3[10];
    sprintf(buf3, "%d", detailInfo->reward_fame);
    CCLabelTTF* pLabel13 = CCLabelTTF::create(buf3, "HelveticaNeue-Bold", 12);
    pLabel13->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(422, yy + 45), pLabel13, 10);
    
    CCLabelTTF* pLabel14 = CCLabelTTF::create("잔여배틀포인트"  , "HelveticaNeue", 12);
    pLabel14->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(58, yy + 12), pLabel14, 10);
    
    char buf1[10];
    int attckPoint = pInfo->getBattlePoint();
    if(attckPoint <= 0) attckPoint= 0;
    
    sprintf(buf1, "%d", attckPoint);
    CCLabelTTF* pLabel15 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
    pLabel15->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(224, yy + 12), pLabel15, 10);
    /*
    CCLabelTTF* pLabel16 = CCLabelTTF::create("잔여방어포인트"  , "HelveticaNeue", 12);
    pLabel16->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0,0), accp(290, yy + 12), pLabel16, 10);
    
    char buf2[10];
    sprintf(buf2, "%d", pInfo->getDefensePoint());
    CCLabelTTF* pLabel17 = CCLabelTTF::create(buf2  , "HelveticaNeue-Bold", 12);
    pLabel17->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(460, yy + 12), pLabel17, 10);
    */
    //////// space
    yy -= 10;
    
    /////////
    yy -= 348;
    CCSprite* pSpr3 = CCSprite::create("ui/battle_tab/battle_duel_result_info_bg2.png");
    pSpr3->setAnchorPoint(accp(0,0));
    pSpr3->setPosition( accp(10,yy) );
    this->addChild(pSpr3, 0);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("배틀상대정보"  , "HelveticaNeue-Bold", 12);
    pLabel3->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(20, yy + 310), pLabel3,10);
    
    CCLabelTTF* pLabel31 = CCLabelTTF::create(detailInfo->enemy_nick  , "HelveticaNeue-Bold", 12);
    CCLabelTTF* pLabel32 = CCLabelTTF::create("Lv."    , "HelveticaNeue-Bold", 12);
    
    char buf38[10];
    sprintf(buf38, "%d", detailInfo->enemy_level);
    CCLabelTTF* pLabel38 = CCLabelTTF::create(buf38    , "HelveticaNeue-Bold", 12);
    pLabel38->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0,0), accp(150, yy + 236), pLabel38, 10);
    
    CCLabelTTF* pLabel33 = CCLabelTTF::create("친구"      , "HelveticaNeue", 12);
    
    char buf34[10];
    sprintf(buf34, "%d", detailInfo->enemy_appFriends);
    CCLabelTTF* pLabel34 = CCLabelTTF::create(buf34       , "HelveticaNeue-Bold", 12);
    
    /*
    CCLabelTTF* pLabel35 = CCLabelTTF::create("총 방어 포인트"  , "HelveticaNeue", 12);
    
    char buf39[10];
    sprintf(buf39, "%d",  detailInfo->enemy_defense_pnt);
    CCLabelTTF* pLabel39 = CCLabelTTF::create(buf39  , "HelveticaNeue-Bold", 12);
    registerLabel( this,ccp(0,0), accp(473, yy + 236), pLabel39, 10);
*/
    CCLabelTTF* pLabel36 = CCLabelTTF::create("전적"      , "HelveticaNeue", 12);
    
    char buf37[10];
    sprintf(buf37, "%d", detailInfo->enemy_victory_cnt);
    string enemyVictorCnt = buf37;
    string enemyVictory = enemyVictorCnt + "승";
    
    char buf40[10];
    float aaa = (float)(detailInfo->enemy_victory_cnt) / (float)(detailInfo->enemy_battle_pnt);
    int aaa1 = (int)(aaa * 100);
    sprintf(buf40, "%d", aaa1);
    string enemyVictoryPnt = buf40;
    
    enemyVictory = enemyVictory + "(" + enemyVictoryPnt + "%)";
    
    CCLabelTTF* pLabel37 = CCLabelTTF::create(enemyVictory.c_str()    , "HelveticaNeue-Bold", 12);
    pLabel31->setColor(COLOR_YELLOW);
    pLabel32->setColor(COLOR_WHITE);
    pLabel33->setColor(COLOR_ORANGE);
    pLabel34->setColor(COLOR_WHITE);
    //pLabel35->setColor(COLOR_ORANGE);
    pLabel36->setColor(COLOR_ORANGE);
    pLabel37->setColor(COLOR_WHITE);
    //pLabel39->setColor(COLOR_WHITE);
    
    registerLabel( this,ccp(0,0), accp(120, yy + 266), pLabel31, 10);
    registerLabel( this,ccp(0,0), accp(120, yy + 236), pLabel32, 10);
    registerLabel( this,ccp(0,0), accp(200, yy + 236), pLabel33, 10);
    registerLabel( this,ccp(0,0), accp(249, yy + 236), pLabel34, 10);
    //registerLabel( this,ccp(0,0), accp(313, yy + 236), pLabel35, 10);
    registerLabel( this,ccp(0,0), accp(120, yy + 206), pLabel36, 10);
    registerLabel( this,ccp(0,0), accp(174, yy + 206), pLabel37, 10);
    
    float x = 24;
    
    for (int i=0;i<5;i++)
    {
        CardInfo *cardInfo = CardDictionary::sharedCardDictionary()->getInfo(detailInfo->opponent_card[i]);
        
        if (cardInfo != NULL)
        {
            cardMaker->MakeCardThumb(this, cardInfo, ccp(x, yy+12), 170, 100, 10+i);
        }
        
        x+=120;
    }
    
    yy-=52;
    CCSprite* pSprBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pSprBtn->setAnchorPoint(ccp(0,0));
    pSprBtn->setPosition( accp(218,yy + 6) );
    pSprBtn->setTag(30);
    this->addChild(pSprBtn, 0);
    
    CCLabelTTF* pLabelBtn = CCLabelTTF::create("Back"  , "HelveticaNeue-Bold", 12);
    pLabelBtn->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0), accp(320, yy + 12), pLabelBtn, 10);
}

void ResultBattleLog::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 30, localPoint))
    {
        BattleLogListLayer::getInstance()->setTouchEnabled(true);
        DojoLayerDojo::getInstance()->setTouchEnabled(true);
        
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
    }

}

