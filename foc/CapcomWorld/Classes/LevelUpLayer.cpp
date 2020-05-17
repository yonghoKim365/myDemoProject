//
//  LevelUpLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 13..
//
//

#include "LevelUpLayer.h"
#include "MainScene.h"
#include "ARequestSender.h"
#include "TraceLayer.h"
#define BTN_WIDTH (82)

LevelUpLayer::LevelUpLayer(CCSize layerSize)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    
    setDisableWithRunningScene();
    
    InitUI();
}

LevelUpLayer::~LevelUpLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void LevelUpLayer::InitUI()
{
    //CCSize size = this->getContentSize();
    
    CCSize size = GameConst::WIN_SIZE;
    YPos = (size.height)*SCREEN_ZOOM_RATE - 620.0f - 71.0f - MAIN_LAYER_TOP_UI_HEIGHT;
    
    CCSprite* plevelBG = CCSprite::create("ui/quest/levelup_bg.png");
    plevelBG->setAnchorPoint(ccp(0, 0));
    plevelBG->setPosition(accp(10, YPos));
    this->addChild(plevelBG, 60);
    
    // -- 퀘스트 포인트 마이너스 버튼
    CCSprite* pQp_Btn_M = CCSprite::create("ui/quest/levelup_btn_m1.png");
    pQp_Btn_M->setTag(QUEST_POINT_MINUS);
    pQp_Btn_M->setAnchorPoint(ccp(0, 0));
    pQp_Btn_M->setPosition(accp(10 + 344, YPos + 306.0f));
    this->addChild(pQp_Btn_M, 60);
    
    // -- 퀘스트 포인트 플러스 버튼
    CCSprite* pQp_Btn_P = CCSprite::create("ui/quest/levelup_btn_p1.png");
    pQp_Btn_P->setTag(QUEST_POINT_PLUS);
    pQp_Btn_P->setAnchorPoint(ccp(0, 0));
    pQp_Btn_P->setPosition(accp(10 + 431, YPos + 306.0f));
    this->addChild(pQp_Btn_P, 60);
    
    // -- 퀘스트 포인트 ALL 버튼
    CCSprite* pQp_Btn_A = CCSprite::create("ui/quest/levelup_btn_a1.png");
    pQp_Btn_A->setTag(QUEST_POINT_ALL);
    pQp_Btn_A->setAnchorPoint(ccp(0, 0));
    pQp_Btn_A->setPosition(accp(10 + 518, YPos + 306.0f));
    this->addChild(pQp_Btn_A, 60);
    
    // -- 어택 포인트 마이너스 버튼
    CCSprite* pAp_Btn_M = CCSprite::create("ui/quest/levelup_btn_m1.png");
    pAp_Btn_M->setTag(ATTACK_POINT_MINUS);
    pAp_Btn_M->setAnchorPoint(ccp(0, 0));
    pAp_Btn_M->setPosition(accp(10 + 344, YPos + 182.0f));
    this->addChild(pAp_Btn_M, 60);
    
    // -- 어택 포인트 플러스 버튼
    CCSprite* pAp_Btn_P = CCSprite::create("ui/quest/levelup_btn_p1.png");
    pAp_Btn_P->setTag(ATTACK_POINT_PLUS);
    pAp_Btn_P->setAnchorPoint(ccp(0, 0));
    pAp_Btn_P->setPosition(accp(10 + 431, YPos + 182.0f));
    this->addChild(pAp_Btn_P, 60);
    
    // -- 어택 포인트 ALL 버튼
    CCSprite* pAp_Btn_A = CCSprite::create("ui/quest/levelup_btn_a1.png");
    pAp_Btn_A->setTag(ATTACK_POINT_ALL);
    pAp_Btn_A->setAnchorPoint(ccp(0, 0));
    pAp_Btn_A->setPosition(accp(10 + 518, YPos + 182.0f));
    this->addChild(pAp_Btn_A, 60);
    
    // --  유저 데이터
    PlayerInfo *pInfo = PlayerInfo::getInstance();
    
    QuestPoint      = pInfo->getMaxStamina(); // QuestPoint();// getQp();
    AttackPoint     = pInfo->getMaxBattlePoint();// getAttackPoint();
    //DefensePoint    = pInfo->getMaxDefencePoint();// getDefensePoint();
    
    // -- 이 값보다 작아질 수 없다.
    minQuestPoint   = pInfo->getMaxStamina();// QuestPoint();
    minAttackPoint  = pInfo->getMaxBattlePoint();
    //minDefensePoint = pInfo->getMaxDefencePoint();
    
    curUpgradePoint = pInfo->getUpgradePoint();
    maxUpgradePoint = pInfo->getUpgradePoint();
    
    //curUpgradePoint = 10;
    //maxUpgradePoint = 10;
    
    
    // -- 라벨
    CCLabelTTF* pTitleLabel = CCLabelTTF::create("유저 속성 업그레이드", "HelveticaNeue-Bold", 12);
    pTitleLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(1.0, 0.5f), accp(609, YPos + 525), pTitleLabel, 60);
    
    char Qbuff[5];
    sprintf(Qbuff, "%d", QuestPoint);
    pQUestLabel = CCLabelTTF::create(Qbuff, "HelveticaNeue-Bold", 15);
    pQUestLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(1.0, 0.5f), accp(304, YPos + 347), pQUestLabel, 60);
    
    char Abuff[5];
    sprintf(Abuff, "%d", AttackPoint);
    pAttackLabel = CCLabelTTF::create(Abuff, "HelveticaNeue-Bold", 15);
    pAttackLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(1.0, 0.5f), accp(304, YPos + 223), pAttackLabel, 60);
    
    // -- 업그레이드 포인트
    DrawNumber(maxUpgradePoint, accp(581, YPos + 439));
    
    // --  하단 버튼
    CCSprite* pBack = CCSprite::create("ui/quest/levelup_btn_c1.png");
    pBack->setTag(BACK);
    pBack->setAnchorPoint(ccp(0, 0));
    pBack->setPosition(accp(30, YPos + 70));
    this->addChild(pBack, 60);
    
    CCLabelTTF* pBackLabel = CCLabelTTF::create("닫기", "HelveticaNeue-Bold", 12);
    pBackLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.5f), accp(123.0f, YPos + 70.0f + 29.0f), pBackLabel, 65);
    
    
    CCSprite* pReset = CCSprite::create("ui/quest/levelup_btn_c1.png");
    pReset->setTag(RESET);
    pReset->setAnchorPoint(ccp(0, 0));
    pReset->setPosition(accp(227, YPos + 70));
    this->addChild(pReset, 60);
    
    CCLabelTTF* pResetLabel = CCLabelTTF::create("초기화", "HelveticaNeue-Bold", 12);
    pResetLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.5f), accp(320.0f, YPos + 70.0f + 29.0f), pResetLabel, 65);
    
    CCSprite* pConfirm = CCSprite::create("ui/quest/levelup_btn_c1.png");
    pConfirm->setTag(CONFIRM);
    pConfirm->setAnchorPoint(ccp(0, 0));
    pConfirm->setPosition(accp(424, YPos + 70));
    this->addChild(pConfirm, 60);
    
    CCLabelTTF* pConfirmLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    pConfirmLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.5f), accp(517.0f, YPos + 70.0f + 29.0f), pConfirmLabel, 65);
}

void LevelUpLayer::ReleaseNumber()
{
    if(UpgradePoints[0])
    {
        this->removeChild(UpgradePoints[0], true);
        UpgradePoints[0] = NULL;
    }
    
    if(UpgradePoints[1])
    {
        this->removeChild(UpgradePoints[1], true);
        UpgradePoints[1] = NULL;
    }
    
    if(UpgradePoints[2])
    {
        this->removeChild(UpgradePoints[2], true);
        UpgradePoints[2] = NULL;
    }
}

void LevelUpLayer::DrawNumber(int _value, CCPoint _pos)
{
    if(_value < 0 || _value > maxUpgradePoint) return;
    
    ReleaseNumber();
    
    int x = 580;
    char buffer[3];
    float scale = 1.0f;
    sprintf(buffer, "%d", _value);
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        UpgradePoints[scan] = createNumber(number, accp(x, _pos.y*SCREEN_ZOOM_RATE), scale);
        this->addChild(UpgradePoints[scan], 2000);
        CCSize size = UpgradePoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale + 2;
        UpgradePoints[scan]->setPosition(accp(x, _pos.y*SCREEN_ZOOM_RATE));
    }
}

CCSprite* LevelUpLayer::createNumber(int number, CCPoint pos, float scale)
{
    assert(number >= 0 && number < 10);
    char buffer[32];
    sprintf(buffer, "ui/card/card_number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

void LevelUpLayer::TouchButton(int tag)
{
    CCNode* pNode = this->getChildByTag(tag);
    CCSprite* pSpr = (CCSprite*)pNode;
    float x = pSpr->getPositionX();
    float y = pSpr->getPositionY();
    
    this->removeChildByTag(tag, true);
    
    CCSprite* pBtn = NULL;
    
    if(QUEST_POINT_PLUS == tag || ATTACK_POINT_PLUS == tag || DEFENSE_POINT_PLUS == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_p2.png");
    if(QUEST_POINT_MINUS == tag || ATTACK_POINT_MINUS == tag || DEFENSE_POINT_MINUS == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_m2.png");
    if(QUEST_POINT_ALL == tag || ATTACK_POINT_ALL == tag || DEFENSE_POINT_ALL == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_a2.png");
    if(BACK == tag || RESET == tag || CONFIRM == tag){
        pBtn  = CCSprite::create("ui/quest/levelup_btn_c2.png");
        pBtn->setScaleY(1.325f);
    }
    
    pBtn->setTag(tag);
    pBtn->setAnchorPoint(ccp(0, 0));
    pBtn->setPosition(accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE));
    this->addChild(pBtn, 60);
    
}

void LevelUpLayer::TouchEndButton(int tag)
{
    CCNode* pNode = this->getChildByTag(tag);
    CCSprite* pSpr = (CCSprite*)pNode;
    float x = pSpr->getPositionX();
    float y = pSpr->getPositionY();
    
    this->removeChildByTag(tag, true);
    
    CCSprite* pBtn = NULL;
    
    if(QUEST_POINT_PLUS == tag || ATTACK_POINT_PLUS == tag || DEFENSE_POINT_PLUS == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_p1.png");
    if(QUEST_POINT_MINUS == tag || ATTACK_POINT_MINUS == tag || DEFENSE_POINT_MINUS == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_m1.png");
    if(QUEST_POINT_ALL == tag || ATTACK_POINT_ALL == tag || DEFENSE_POINT_ALL == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_a1.png");
    if(BACK == tag || RESET == tag || CONFIRM == tag)
        pBtn  = CCSprite::create("ui/quest/levelup_btn_c1.png");
    
    
    pBtn->setTag(tag);
    pBtn->setAnchorPoint(ccp(0, 0));
    pBtn->setPosition(accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE));
    this->addChild(pBtn, 60);
}

void LevelUpLayer::QPointLabel(int OriPoint, int AddPoint)
{
    char QPoint[5];
    sprintf(QPoint, "%d", OriPoint);
    
    char AddQPoint[5];
    sprintf(AddQPoint, "%d", AddPoint - OriPoint);
    
    string strQPoint = QPoint;
    
    if(0 == (OriPoint - AddPoint))
    {
        pQUestLabel = CCLabelTTF::create(QPoint, "HelveticaNeue-Bold", 15);
        pQUestLabel->setColor(COLOR_WHITE);
        registerLabel( this,ccp(1.0f, 0.5f), accp(304, YPos + 347.0f), pQUestLabel, 60);
    }
    else
    {
        pQUestLabel = CCLabelTTF::create(strQPoint.append("(+").append(AddQPoint).append(")").c_str(), "HelveticaNeue-Bold", 15);
        pQUestLabel->setColor(COLOR_WHITE);
        registerLabel( this,ccp(1.0f, 0.5f), accp(304, YPos + 347.0f), pQUestLabel, 60);
    }
}

void LevelUpLayer::QuestProc(int tag)
{
    switch (tag)
    {
        case QUEST_POINT_MINUS:
        {
            if(minQuestPoint < QuestPoint)
            {
                ++curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(581, YPos + 439));
                
                this->removeChild(pQUestLabel, true);
                
                --QuestPoint;
                
                QPointLabel(minQuestPoint, QuestPoint);
            }
        }break;
            
        case QUEST_POINT_PLUS:
        {
            if(curUpgradePoint > 0)
            {
                --curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(581, YPos + 439));
                
                this->removeChild(pQUestLabel, true);
                
                ++QuestPoint;
                
                QPointLabel(minQuestPoint, QuestPoint);
            }
            
        }break;
            
        case QUEST_POINT_ALL:
        {
            if(curUpgradePoint > 0)
            {
                DrawNumber(0, accp(581, YPos + 439));
                
                this->removeChild(pQUestLabel, true);
                
                QuestPoint += curUpgradePoint;
                
                QPointLabel(minQuestPoint, QuestPoint);
                
                curUpgradePoint = 0;
            }
        }
            
        default:
            break;
    }
}

void LevelUpLayer::APointLabel(int OriPoint, int AddPoint)
{
    char APoint[5];
    sprintf(APoint, "%d", OriPoint);
    
    char AddAPoint[5];
    sprintf(AddAPoint, "%d", AddPoint - OriPoint);
    
    string strQPoint = APoint;
    
    if(0 == (OriPoint - AddPoint))
    {
        pAttackLabel = CCLabelTTF::create(APoint, "HelveticaNeue-Bold", 15);
        pAttackLabel->setColor(COLOR_WHITE);
        registerLabel( this,ccp(1.0f, 0.5f), accp(304, YPos + 223), pAttackLabel, 60);
    }
    else
    {
        pAttackLabel = CCLabelTTF::create(strQPoint.append("(+").append(AddAPoint).append(")").c_str(), "HelveticaNeue-Bold", 15);
        pAttackLabel->setColor(COLOR_WHITE);
        registerLabel( this,ccp(1.0f, 0.5f), accp(304, YPos + 223), pAttackLabel, 60);
    }
}

void LevelUpLayer::AttackProc(int tag)
{
    switch (tag)
    {
        case ATTACK_POINT_MINUS:
        {
            if(minAttackPoint < AttackPoint)
            {
                ++curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(581, YPos + 439));
                
                this->removeChild(pAttackLabel, true);
                
                --AttackPoint;
                
                APointLabel(minAttackPoint, AttackPoint);
            }
        }break;
            
        case ATTACK_POINT_PLUS:
        {
            if(curUpgradePoint > 0)
            {
                --curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(581, YPos + 439));
                
                this->removeChild(pAttackLabel, true);
                
                ++AttackPoint;
                
                APointLabel(minAttackPoint, AttackPoint);
            }
            
        }break;
            
        case ATTACK_POINT_ALL:
        {
            if(curUpgradePoint > 0)
            {

                DrawNumber(0, accp(581, YPos + 439));
                
                this->removeChild(pAttackLabel, true);
                
                AttackPoint += curUpgradePoint;
                
                APointLabel(minAttackPoint, AttackPoint);
                
                curUpgradePoint = 0;
            }
        }
            break;
        default:
            break;
    }
}

void LevelUpLayer::DefenseProc(int tag)
{
    switch (tag)
    {
        case DEFENSE_POINT_MINUS:
        {
            if(minDefensePoint < DefensePoint)
            {
                ++curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(580, YPos + 515));
                
                this->removeChild(pDefenseLabel, true);
                
                --DefensePoint;
                
                char Qbuff[5];
                sprintf(Qbuff, "%d", DefensePoint);
                pDefenseLabel = CCLabelTTF::create(Qbuff, "HelveticaNeue-Bold", 14);
                pDefenseLabel->setColor(COLOR_WHITE);
                registerLabel( this,ccp(1.0, 0), accp(300, YPos + 158), pDefenseLabel, 60);
            }
        }break;
            
        case DEFENSE_POINT_PLUS:
        {
            if(curUpgradePoint > 0)
            {
                --curUpgradePoint;
                
                DrawNumber(curUpgradePoint, accp(580, YPos + 515));
                
                this->removeChild(pDefenseLabel, true);
                
                ++DefensePoint;
                
                char Qbuff[5];
                sprintf(Qbuff, "%d", DefensePoint);
                pDefenseLabel = CCLabelTTF::create(Qbuff, "HelveticaNeue-Bold", 14);
                pDefenseLabel->setColor(COLOR_WHITE);
                registerLabel( this,ccp(1.0, 0), accp(300, YPos + 158), pDefenseLabel, 60);
            }
            
        }break;
            
        case DEFENSE_POINT_ALL:
        {
            if(curUpgradePoint > 0)
            {
                DrawNumber(0, accp(580, YPos + 515));
                
                this->removeChild(pDefenseLabel, true);
                
                DefensePoint += curUpgradePoint;
                
                char Qbuff[5];
                sprintf(Qbuff, "%d", DefensePoint);
                pDefenseLabel = CCLabelTTF::create(Qbuff, "HelveticaNeue-Bold", 14);
                pDefenseLabel->setColor(COLOR_WHITE);
                registerLabel( this,ccp(1.0, 0), accp(300, YPos + 158), pDefenseLabel, 60);
                
                curUpgradePoint = 0;
            }
        }
            break;
        default:
            break;
    }
}

void LevelUpLayer::BackProc(int tag)
{
    if(BACK != tag) return;
    
    restoreTouchDisable();
    
    MainScene* main = MainScene::getInstance();
    main->releaseLevelUpLayer();
    UserStatLayer::getInstance()->AddLevelUpIcon();
    
    if (TraceLayer::getInstance() != NULL){
        TraceLayer::getInstance()->CloseLevelUpLayer();
    }
}

void LevelUpLayer::ResetProc(int tag)
{
    if(RESET != tag) return;
    
    restoreTouchDisable();
    
    MainScene* main = MainScene::getInstance();
    main->initLevelUpLayer();
}

void LevelUpLayer::Confirm(int tag)
{
    if(CONFIRM != tag) return;
    
    restoreTouchDisable();
    
    //ARequestSender::getInstance()->requestUpgrade(AttackPoint - minAttackPoint, DefensePoint - minDefensePoint, QuestPoint - minQuestPoint);
    ARequestSender::getInstance()->requestUpgrade(AttackPoint - minAttackPoint, 0, QuestPoint - minQuestPoint);
    
    MainScene::getInstance()->releaseLevelUpLayer();
    
    if (PlayerInfo::getInstance()->getUpgradePoint()==0){
        UserStatLayer::getInstance()->RemoveLevelUpIcon();
    }
    else
        UserStatLayer::getInstance()->AddLevelUpIcon();
    
    if (UserStatLayer::getInstance()){
        UserStatLayer::getInstance()->refreshUI();
    }
    
    if (TraceLayer::getInstance() != NULL){
        TraceLayer::getInstance()->CloseLevelUpLayer();
    }
}

void LevelUpLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    for(int i=0; i<BTN_TAG_TOTAL; ++i)
    {
        if(GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            nLastTouchTag = i;
            TouchButton(i);
        }
    }
}

void LevelUpLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=0; i<BTN_TAG_TOTAL; ++i)
    {
        if(GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            if (nLastTouchTag == i){
                soundButton1();
                
                TouchEndButton(i);
                
                QuestProc(i);
                AttackProc(i);
                DefenseProc(i);
                BackProc(i);
                ResetProc(i);
                Confirm(i);
            }
            
            nLastTouchTag = -1;
        }
    }
    
    touchStartPoint = location;
    bTouchPressed = true;
}

void LevelUpLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}
