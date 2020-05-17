//
//  BattlePrebattleLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 20..
//
//

#include "BattlePrebattleLayer.h"
#include "BattlePrebattleLayer.h"
#include "BattlePlayerCell.h"
#include "MainScene.h"
#include "ARequestSender.h"


BattlePrebattleLayer* BattlePrebattleLayer::instance = NULL;

BattlePrebattleLayer::BattlePrebattleLayer(CCSize layerSize, UserInfo *user, int _team)
{
    instance = this;
    selectedTeam = _team;
    bTouchPressed = false;
    
    this->setContentSize(layerSize);
    this->setAnchorPoint(ccp(0,0));
    this->setTouchEnabled(true);
    
    cardMaker = new ACardMaker();
    
    //xb = new XBridge();
    
    const char* aaa[] = {
        "팀 1",
        "팀 2",
        "팀 3",
        "팀 4",
        "자동화팀"
    };
    
    for (int i=0;i<5;i++){
        TeamLabel[i] = aaa[i];
    }
    
    battleUser = user;
    
    InitLayer();
    
    
}


BattlePrebattleLayer::~BattlePrebattleLayer()
{
    
    this->removeAllChildrenWithCleanup(true);
    MainScene::getInstance()->removePopup();
    
    //delete xb;
    
}

void BattlePrebattleLayer::changeTeam(int n){
    
    selectedTeam = n;
    
    CCLog("selectedTeam 333 %d ", selectedTeam);
    
    this->removeChildByTag(201, true);
    
    int yy = 635 + 85;
    /*
    CCLabelTTF* pLabel1 = CCLabelTTF::create(TeamLabel[n]  , "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_WHITE);
    pLabel1->setTag(201);
    registerLabel( this,ccp(0,0), accp(22, yy - 14), pLabel1,10); // 635
    */
     yy = 402 + 87 + 48;
    if (teamLayer != NULL){
//        teamLayer->removeAllChildrenWithCleanup(true);
        this->removeChild(teamLayer,true);
        teamLayer = NULL;
    }
    teamLayer = new CardTeamSimpleDrawLayer(0,n);
    teamLayer->setAnchorPoint(ccp(0,0));
    teamLayer->setPosition(accp(0,yy+50));
    this->addChild(teamLayer, 10);
    
    ///// label
    this->removeChildByTag(202, true);
    int tot_attack = 0;
    int tot_battlePoint = 0;
    PlayerInfo *pi = PlayerInfo::getInstance();
    for (int i=0;i<5;i++){
        CardInfo *card = pi->GetCardInDeck(0, selectedTeam, i);
        if (card != NULL){
            tot_attack += card->getAttack();
            tot_battlePoint += card->getBp();
        }
    }
    char buf1[10];
    sprintf(buf1,"%d", tot_attack);
    char buf2[10];
    sprintf(buf2,"%d", tot_battlePoint);
    std::string team_desc = "공격력 ";
    team_desc.append(buf1).append(" 배틀포인트 ").append(buf2);
    CCLabelTTF* pLabel2 = CCLabelTTF::create(team_desc.c_str(), "HelveticaNeue-Bold", 12);
    pLabel2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.5,0), accp(320, yy - 12), pLabel2,10);
    pLabel2->setTag(202);
    
}


void BattlePrebattleLayer::InitLayer()
{
    int yy = this->getContentSize().height* SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN;
    
    
    /*
    yy -= 48;
    CCSprite* pSpr1 = CCSprite::create("ui/battle_tab/battle_duel_ready_team_name.png");
    pSpr1->setAnchorPoint(accp(0,0));
    pSpr1->setPosition( accp(10,yy) );
    pSpr1->setTag(20);
    this->addChild(pSpr1, 0);
     
    */
    
    // draw card deck
    yy -= 233;
    CCSprite* pSpr2 = CCSprite::create("ui/battle_tab/battle_duel_ready_team_bg.png");
    pSpr2->setAnchorPoint(accp(0,0));
    pSpr2->setPosition( accp(10,yy) );
    this->addChild(pSpr2, 0);
    
    changeTeam(selectedTeam);
    
    /*
    teamLayer = new CardTeamSimpleDrawLayer(0,0);
    teamLayer->setAnchorPoint(ccp(0,0));
    teamLayer->setPosition(accp(0,yy+50));
    this->addChild(teamLayer, 10);
    */
    // space
    yy-=5;
    
    // team edit button
    yy -= 54;//43;
    CCSprite* pSpr3 = CCSprite::create("ui/battle_tab/battle_duel_ready_team_edit.png");
    pSpr3->setAnchorPoint(accp(0,0));
    pSpr3->setPosition( accp(10,yy) );
    pSpr3->setTag(21);
    this->addChild(pSpr3, 0);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("공격팀 다시구성"  , "HelveticaNeue-Bold", 12);
    pLabel3->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0), accp(320, yy + 6), pLabel3,10);
    yy-=5;
    
    ///////////////////////////////////////// battle start
    yy-=140+10;
    /*
     CCSprite* pSpr4 = CCSprite::create("ui/battle_tab/battle_duel_ready_start_1.png");
     pSpr4->setAnchorPoint(accp(0,0));
     pSpr4->setPosition( accp(10,yy) );
     pSpr4->setTag(22);
     this->addChild(pSpr4, 0);
     */
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_1.png", CCRectMake(0,0,accp(620),accp(140)));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_2.png", CCRectMake(0,0,accp(620),accp(140)));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_3.png", CCRectMake(0,0,accp(620),accp(140)));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_4.png", CCRectMake(0,0,accp(620),accp(140)));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->autorelease();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    
    regAni(aniFrame, this, ccp(0,0), accp(10,yy), 22, 0);
    
    //////////////////////////////////////////
    // cell
    
    //yy -= (156-10);
    yy -= (156 + 10);
    BattlePlayerCell *cell = new BattlePlayerCell(battleUser, NULL);
    cell->setAnchorPoint(accp(0,0));
    cell->setPosition(accp(10,yy));
    this->addChild(cell, 0);
    
    yy-=62;//52;
    CCSprite* pSprBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btn.png");
    pSprBtn->setAnchorPoint(ccp(0,0));
    pSprBtn->setPosition( accp(218,yy) );
    pSprBtn->setTag(23);
    this->addChild(pSprBtn, 0);
    
    CCLabelTTF* pLabelBtn = CCLabelTTF::create("배틀리스트"  , "HelveticaNeue-Bold", 12);
    pLabelBtn->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0), accp(320, yy + 6), pLabelBtn, 10);
    
}


void BattlePrebattleLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //    touchStartPoint = location;
    bTouchPressed = true;
    
    
}

void BattlePrebattleLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //    bTouchPressed = false;
    if (bTouchPressed==false)return;
    
    bTouchPressed = false;
    
    if (MainScene::getInstance()->popupCnt>0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 20, localPoint)){
        CCLog(" button - 20, team select, selectedTeam:%d ",selectedTeam);
        
        // test
        //MainScene::getInstance()->ShowMainMenu();

//#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
        PlayerInfo::getInstance()->xb->DropDownView(TeamLabel, 5, selectedTeam, 10);
//#endif
        

    }

    if (GetSpriteTouchCheckByTag(this, 21, localPoint)){
        CCLog(" button - 21m team edit, selectedTeam:%d",selectedTeam);
        // ??Edit
        
        if (selectedTeam == 4){
            popupOk("자동팀은 수정할 수 없습니다");
        }
        else{
            BattleDuelLayer::getInstance()->InitLayer(10, selectedTeam);
        }
        
        
        // test
        //MainScene::getInstance()->HideMainMenu();
        //this->setPosition(ccp(0,-100));
    }
    
    // battle point 의 b 좌상단 클릭시좌표
    // 1 - 72, 177
    // 2 - 66, 155
    
    if (GetSpriteTouchCheckByTag(this, 22, localPoint)){
                
        soundButton2();
        
        //PlayerInfo::getInstance()->setAttackPoint(1);
        //int a = PlayerInfo::getInstance()->getTeamBattlePoint(0, selectedTeam);
        //int b = PlayerInfo::getInstance()->getAttackPoint();
        //CCLog(" team attack :%d my battle point :%d",a,b);
        
        int totSrl = 0;
        for(int i=0;i<5;i++){
            CardInfo *card = PlayerInfo::getInstance()->GetCardInDeck(0, selectedTeam, i);
            if (card != NULL){
                totSrl += card->getSrl();
            }
        }
        
        
        //if (PlayerInfo::getInstance()->getTeamBattlePoint(0, selectedTeam) == 0){
        if (totSrl == 0){
            // empty team
            popupOk("카드가 설정된 팀을 선택해야 합니다");
        }
        else if (PlayerInfo::getInstance()->getTeamBattlePoint(0, selectedTeam) > PlayerInfo::getInstance()->getBattlePoint()){
            //CCLog(" button - 23, battle start");
            popupOk("배틀포인트가 부족합니다 \n 블루 젬을 사용하세요");
        }
        else
        {
            ResponseBattleInfo* battleInfo = ARequestSender::getInstance()->requestBattle(selectedTeam, battleUser->userID);
            if(NULL == battleInfo)
            {
                popupOk("서버와의 연결이 원활하지 않습니다. \n 잠시후에 다시 시도해 주세요.");
            }
            else
            {
                PlayerInfo::getInstance()->battleResponseInfo = battleInfo;
                if (atoi(PlayerInfo::getInstance()->battleResponseInfo->res) == 0)
                {
                    CCLog(" PlayerInfo::getInstance()->battleResponseInfo->reward_coin:%d", PlayerInfo::getInstance()->battleResponseInfo->reward_coin);
                    CCLog(" PlayerInfo::getInstance()->battleResponseInfo->reward_fame:%d", PlayerInfo::getInstance()->battleResponseInfo->reward_fame);
                    if(PlayerInfo::getInstance()->battleResponseInfo->skills)
                    {
                        for(int i=0; i<PlayerInfo::getInstance()->battleResponseInfo->skills->count(); ++i)
                        {
                            OpponentSkillInfo* skill = (OpponentSkillInfo*)PlayerInfo::getInstance()->battleResponseInfo->skills->objectAtIndex(i);
                            
                            CCLOG("스킬 card id  : %d", skill->cardID);
                            CCLOG("스킬 side     : %d", skill->side);
                            CCLOG("스킬 skill id : %d", skill->skillID);
                        }
                    }
                    // battle start
                    this->removeAllChildrenWithCleanup(true);
                    BattleDuelLayer::getInstance()->selectedTeam = selectedTeam;
                    BattleDuelLayer::getInstance()->InitLayer(2);
                }
                else{
                    popupNetworkError(PlayerInfo::getInstance()->battleResponseInfo->res,PlayerInfo::getInstance()->battleResponseInfo->msg, "requestBattle");
                }
            }
            
        }
        
        

    }
    
    if (GetSpriteTouchCheckByTag(this, 23, localPoint)){
        CCLog(" button - back to duel battle list");
        this->removeAllChildrenWithCleanup(true);
        //nBattleStep = 0;
        //InitLayer(0);
        BattleDuelLayer::getInstance()->InitLayer(0);
        MainScene::getInstance()->removePopup();
    }
    
    //CCLog(" sublayer y =%f, height=%f",y,a);
}


void BattlePrebattleLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){

}

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

void BattlePrebattleLayer::reserveRefresh(int r)
{
    CCLog("BattlePrebattleLayer::reserveRefresh, row=%d",r);
    
    reserveRefreshVal = r;
    
    BattlePrebattleLayer::getInstance()->schedule(schedule_selector(BattlePrebattleLayer::doRefresh),0.3);
    
}

void BattlePrebattleLayer::doRefresh(){
    
    CCLog("BattlePrebattleLayer::doRefresh");
    
    BattlePrebattleLayer::getInstance()->changeTeam(reserveRefreshVal);
    
    BattlePrebattleLayer::getInstance()->unschedule(schedule_selector(BattlePrebattleLayer::doRefresh));
}
#endif

