//
//  DojoLayerDojo.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 27..
//
//

#include "DojoLayerDojo.h"
#include "PlayerInfo.h"
#include "MyUtil.h"
#include "MainScene.h"
#include "Tutorial.h"
#include "KakaoLoginScene.h"
#include <stdlib.h>
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#include <fstream.h>
using namespace cocos2d::extension;
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include <fstream>
#include <ios>
#include <iostream>
#endif
using namespace cocos2d;
bool bTouchPressed;

DojoLayerDojo *DojoLayerDojo::instance = NULL;

DojoLayerDojo::DojoLayerDojo(CCSize layerSize) : pDojoLayerCollect(NULL), pEventLayer(NULL), pBattleLogLayer(NULL), pSocialLayer(NULL), pItemLayer(NULL), pOptionLayer(NULL), pSprDefenceLeader(NULL), pSprAttackLeader(NULL), pBattleLogListLayer(NULL), BattleLogIcon(NULL), EventLogIcon(NULL), GiftIcon(NULL), BgIcon(NULL), pTraceHistoryLayer(NULL)
{
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
    } while (0);
    
    subBtn_color_normal = ccc3(197,197,197);
    BgDictionary = new CCDictionary;
    
    battlelogActive = false;
    eventlogActive = false;
    //bSkipRefreshRivalList = false;
    bExitPopup = false;
    setKeypadEnabled(true);
    init();
}

void DojoLayerDojo::keyBackClicked()
{
    CCLog(" keyBackClicked");
    if (bExitPopup == false){
        ExitWarningPopup *popup = new ExitWarningPopup();
        popup->InitUI();
        popup->setTag(123);
        MainScene::getInstance()->addPopup(popup,1000);
        //this->addChild(popup,1000);
        bExitPopup = true;
    }
}

DojoLayerDojo::~DojoLayerDojo()
{
    this->removeAllChildrenWithCleanup(true);
    CC_SAFE_DELETE(BgDictionary);
    
}

//bool DojoLayerDojo::init()
void DojoLayerDojo::init()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();

    instance = this;
    InitLayer();
    
    InitMainBGNameList();
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    // add "HelloWorld" splash screen"
    
    // -- 최초 접속
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        //int defaultBgID = 5001;
        if(0 != pInfo->getBackground())
        {
            //defaultBgID = pInfo->getBackground();
        }
        
        Bg_List* bglist = (Bg_List*)BgDictionary->objectForKey(5001);
        
        string path = "ui/main_bg/";
        path+=bglist->L_ImgPath;
        pBgSprite = CCSprite::create(path.c_str());
        
        pBgSprite->setScale(MAIN_BG_SCALE);
        pBgSprite->setAnchorPoint(ccp(0.0f, 0.5f));
        pBgSprite->setPosition( ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2) );
        this->addChild(pBgSprite, 0);
                
        const char *fullPath = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("ui/main_bg/bg_1.png");
        ifstream source(fullPath, ios::binary);
        
        if(NULL == source)
            return;
        string path1 = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path1+="bg_1.png";

        ofstream dest(path1.c_str(), ios::binary);
        
        dest << source.rdbuf();
        
        source.close();
        dest.close();
        
    }
    else
    {
        int defaultBgID = 5001;
        if(0 != pInfo->getBackground())
        {
            defaultBgID = pInfo->getBackground();
        }
        
        Bg_List* bglist = (Bg_List*)BgDictionary->objectForKey(defaultBgID);
        
        if(!FileManager::sharedFileManager()->IsFileExist(bglist->L_ImgPath.c_str()))
        {
            // -- 게임을 삭제한 후 다시 시작한 유저들은 서버에서 받은 배경이 없을 수 있음
            std::vector<std::string> downloads;
            
            string downPath = FOC_IMAGE_SERV_URL;
            downPath.append("images/bg/");
            downPath+=bglist->L_ImgPath;
            downloads.push_back(downPath);
            
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            requestor->addDownloadTask(downloads, this, NULL);
            
            // 다운받는중에 기본 배경으로 뿌려준다.
            pBgSprite = CCSprite::create("ui/main_bg/bg_1.png");
            
            pBgSprite->setScale(MAIN_BG_SCALE);
            pBgSprite->setAnchorPoint(ccp(0.0f, 0.5f));
            pBgSprite->setPosition( ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2) );
            this->addChild(pBgSprite, 0);

        }
        else
        {
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=bglist->L_ImgPath;
            pBgSprite = CCSprite::create(path.c_str());
            
            pBgSprite->setScale(MAIN_BG_SCALE);
            
            pBgSprite->setAnchorPoint(ccp(0.0f, 0.5f));
            pBgSprite->setPosition( ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2) );
            
            this->addChild(pBgSprite, 0);
        }
    }
    
    CCFiniteTimeAction* actionMove1 = CCMoveTo::actionWithDuration(10.0f, ccp(-180.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2));
    CCFiniteTimeAction* actionMove2 = CCMoveTo::actionWithDuration(10.0f, ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2));
    CCRepeatForever *repeat = CCRepeatForever::actionWithAction((CCSequence*)CCSequence::actions(actionMove1, actionMove2, NULL));
    pBgSprite->runAction(repeat);
    
    ///////////////////////
    /////////////////////////////////////////////////////////////
    //
    // 라이벌 이력
    
    InitRivalUI(false);

    //////////////////////////////////////////////////
    
    bTouchPressed = false;
        
    m_DojoLayerBg = NULL;
    
    loadCharImg();
    
}

void DojoLayerDojo::InitRivalUI(bool bNew)
{
    this->removeChildByTag(50, true);
    this->removeChildByTag(180, true);
    
    CCSprite* pSprTrace;
    CCLabelTTF* pLblTrace;
    
    if (bNew){
        pSprTrace = CCSprite::create("ui/home/rival_btn_appear_a1.png");
        pLblTrace = CCLabelTTF::create("라이벌 추적", "HelveticaNeue-Bold", 15);
    }
    else{
        pSprTrace = CCSprite::create("ui/home/rival_btn_list_a1.png");
        pLblTrace = CCLabelTTF::create("라이벌 이력", "HelveticaNeue-Bold", 15);
    }
    
    pSprTrace->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprTrace->setPosition(accp(7.0f, 650.0f));
    pSprTrace->setTag(QUICK_ICON_TRACER);
    this->addChild(pSprTrace, 50);
    
    pLblTrace->setColor(COLOR_WHITE);
    pLblTrace->setTag(180);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(accp(185.0f), accp(700.0f)), pLblTrace, 50);
    
}

/*
void DojoLayerDojo::InitEnemyCharacter()
{
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    //CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    //CCLog("===== 적 데이터 세팅 =====");
    // 제일 우측
    CardInfo* card1 = CardDictionary::sharedCardDictionary()->getInfo(30011);//pInfo->battleResponseInfo->opponent_card[4]);
    if(card1)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(30011);//pInfo->battleResponseInfo->opponent_card[4]);
        if(cardInfo)
        {
            //TotalDefensePoint +=
            enemyPosX[0] = 220.0 + (512.0f*1.2f)/2;
            enemyPosY[0] = (size.height*SCREEN_ZOOM_RATE) -70.0f -476.0f - (512.0f*1.2f) + (512.0f*1.2f)/2;
            
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyCharacter[0] = CCSprite::create(path.c_str());
            enemyCharacter[0]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[0]->setScale(1.2f);
            //enemyCharacter[0]->setPosition(accp(-(512.0f*1.2f)/2, enemyPosY[0]));
            enemyCharacter[0]->setPosition(accp(enemyPosX[0], enemyPosY[0]));
            enemyCharacter[0]->setTag(200);
            this->addChild(enemyCharacter[0], 10509);
        }
    }

    // 오른쪽에서 2번째
    CardInfo* card2 = CardDictionary::sharedCardDictionary()->getInfo(30011);//pInfo->battleResponseInfo->opponent_card[3]);
    if(card2)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(30011);//pInfo->battleResponseInfo->opponent_card[3]);
        if(cardInfo)
        {
            enemyPosX[1] = 172.0f + (512.0f*0.95f)/2;
            enemyPosY[1] = (size.height*SCREEN_ZOOM_RATE) -25.0f -476.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            
            //string path = "ui/cha/";
            string path2 = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path2+=cardInfo->largeBattleImg;
            
            enemyCharacter[1] = CCSprite::create(path2.c_str());
            enemyCharacter[1]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[1]->setScale(0.95f);
            //enemyCharacter[1]->setPosition(accp(-(512.0f*0.95f)/2, enemyPosY[1]));
            enemyCharacter[1]->setPosition(accp(enemyPosX[1], enemyPosY[1]));
            enemyCharacter[1]->setTag(201);
            this->addChild(enemyCharacter[1], 10508);
        }
    }
    
    
    CardInfo* card3 = CardDictionary::sharedCardDictionary()->getInfo(30011);//pInfo->battleResponseInfo->opponent_card[2]);
    if(card3)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(30011);//pInfo->battleResponseInfo->opponent_card[2]);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path3 = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path3+=cardInfo->largeBattleImg;
            
            //enemyLeader = path.c_str();
            enemyPosX[2] = -130.0f + (512.0f*1.55f)/2;
            enemyPosY[2] = (size.height*SCREEN_ZOOM_RATE) -60.0f -476.0f - (512.0f*1.55f) + (512.0f*1.55f)/2;
            enemyCharacter[2] = CCSprite::create(path3.c_str());
            enemyCharacter[2]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[2]->setScale(1.55f);
            //enemyCharacter[2]->setPosition(accp(-(512.0f*1.55f)/2, enemyPosY[2]));
            enemyCharacter[2]->setPosition(accp(enemyPosX[2], enemyPosY[2]));
            enemyCharacter[2]->setTag(202);
            this->addChild(enemyCharacter[2], 10510);
        }
    }
    
    //return;
    
    CardInfo* card4 = CardDictionary::sharedCardDictionary()->getInfo(30011);//pInfo->battleResponseInfo->opponent_card[1]);
    if(card4)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(30011);//pInfo->battleResponseInfo->opponent_card[1]);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyPosX[3] = -140.0f + (512.0f*1.08f)/2;;
            enemyPosY[3] = (size.height*SCREEN_ZOOM_RATE) -80.0f -476.0f - (512.0f*1.08f) + (512.0f*1.08f)/2;
            enemyCharacter[3] = CCSprite::create(path.c_str());
            enemyCharacter[3]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[3]->setScale(1.08f);
            //enemyCharacter[3]->setPosition(accp(-(512.0f*1.08f)/2, enemyPosY[3]));
            enemyCharacter[3]->setPosition(accp(enemyPosX[3], enemyPosY[3]));
            enemyCharacter[3]->setTag(203);
            this->addChild(enemyCharacter[3], 10509);
        }
    }
    
    
    CardInfo* card5 = CardDictionary::sharedCardDictionary()->getInfo(30011);//pInfo->battleResponseInfo->opponent_card[0]);
    if(card5)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(30011);//pInfo->battleResponseInfo->opponent_card[0]);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyPosX[4] = -205.0f + (512.0f*0.95f)/2;;
            enemyPosY[4] = (size.height*SCREEN_ZOOM_RATE) -35.0f -476.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            enemyCharacter[4] = CCSprite::create(path.c_str());
            enemyCharacter[4]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[4]->setScale(0.95f);
            //enemyCharacter[4]->setPosition(accp(-(512.0f*0.95f)/2, enemyPosY[4]));
            enemyCharacter[4]->setPosition(accp(enemyPosX[4], enemyPosY[4]));
            enemyCharacter[4]->setTag(204);
            this->addChild(enemyCharacter[4], 10508);
        }
    }
}
*/
void DojoLayerDojo::loadCharImg()
{
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    
    // -- 첫 접속이면 최초 지급받은 5장중 랜덤으로
    if(5 >= PlayerInfo::getInstance()->getTutorialProgress())
    {
        PlayerInfo* pi = PlayerInfo::getInstance();
        const int myCardCount = pi->myCards->count();
        
        const int index1 = rand()%myCardCount;
        CardInfo* card1 = (CardInfo*)pi->myCards->objectAtIndex(index1);
        
        if(card1)
        {
            CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card1->cardId);
            if(cardInfo)
            {
                string path = "ui/cha/";
                path+=cardInfo->GetLargeBattleImg();
                pSprAttackLeader    = CCSprite::create(path.c_str());
                pSprAttackLeader->setAnchorPoint(ccp(0, 0));
                pSprAttackLeader->setPosition(accp(15, -96-MAIN_LAYER_BTN_HEIGHT));
                pSprAttackLeader->setScale(1.8f);
                this->addChild(pSprAttackLeader,10);
            }
        }
        
        const int index2 = rand()%myCardCount;
        CardInfo* card2 = (CardInfo*)pi->myCards->objectAtIndex(index2);
        
        if(card2)
        {
            CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card2->cardId);
            if(cardInfo)
            {
                string path = "ui/cha/";
                path+=cardInfo->GetLargeBattleImg();
                pSprDefenceLeader    = CCSprite::create(path.c_str());
                pSprDefenceLeader->setAnchorPoint(ccp(0, 0));
                pSprDefenceLeader->setPosition(accp(-79, 70-MAIN_LAYER_BTN_HEIGHT));
                pSprDefenceLeader->setScale(1.2f);
                this->addChild(pSprDefenceLeader,5);
            }
        }
    }
    else
    {
        bool download = false;
        
        std::vector<std::string> downloads;
        std::string basePathL  = FOC_IMAGE_SERV_URL;
        basePathL.append("images/cha/cha_l/");
        
        std::string basePathS  = FOC_IMAGE_SERV_URL;
        basePathS.append("images/cha/cha_s/");
        
        CardInfo *card = pInfo->GetCardInDeck(0, 0, 2);
        if(card)
        {
            CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
            if(cardInfo)
            {
                if(FileManager::sharedFileManager()->IsFileExist(cardInfo->largeBattleImg.c_str()))
                {
                    
                    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
                    path+=cardInfo->GetLargeBattleImg();
                    pSprAttackLeader    = CCSprite::create(path.c_str());
                    pSprAttackLeader->setAnchorPoint(ccp(0, 0));
                    pSprAttackLeader->setPosition(accp(15, -96-MAIN_LAYER_BTN_HEIGHT));
                    pSprAttackLeader->setScale(1.8f);
                    this->addChild(pSprAttackLeader,9);
                    
                }
                else
                {
                    // -- 게임 삭제 후 다시 시작한 유저
                    
                    download= true;
                    
                    string downPath1 = basePathL + cardInfo->largeBattleImg;
                    string downPath2 = basePathS + cardInfo->smallBattleImg;
                    downloads.push_back(downPath1);
                    downloads.push_back(downPath2);
                }
            }
        }
        
        card = pInfo->GetCardInDeck(1, 0, 2);
        if(card)
        {
            CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
            if(cardInfo)
            {
                if(FileManager::sharedFileManager()->IsFileExist(cardInfo->largeBattleImg.c_str()))
                {
                    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
                    path+=cardInfo->GetLargeBattleImg();
                    pSprDefenceLeader   = NULL;
                    pSprDefenceLeader    = CCSprite::create(path.c_str());
                    pSprDefenceLeader->setAnchorPoint(ccp(0, 0));
                    pSprDefenceLeader->setPosition(accp(-79, 70-MAIN_LAYER_BTN_HEIGHT));
                    pSprDefenceLeader->setScale(1.2f);
                    this->addChild(pSprDefenceLeader,5);
                }
                else
                {
                    // -- 게임 삭제 후 다시 시작한 유저
                    
                    download = true;
                    
                    string downPath1 = basePathL + cardInfo->largeBattleImg;
                    string downPath2 = basePathS + cardInfo->smallBattleImg;
                    downloads.push_back(downPath1);
                    downloads.push_back(downPath2);                    
                }
            }
        }
        
        if(true == download)
        {
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            requestor->addDownloadTask(downloads, this, NULL);
        }
    }
}

void DojoLayerDojo::RefreshUsername(){
    DojoLayerDojo *dojoInstance = DojoLayerDojo::getInstance();
    if (dojoInstance->getChildByTag(1909) != NULL)
        dojoInstance->removeChildByTag(1909, true);
    PlayerInfo *pi = PlayerInfo::getInstance();
    std::string name = (pi->GetName() && strlen(pi->GetName())) ? pi->GetName() : "Capcom";
    name = pi->displayName;
    CCLabelTTF* pLabel6 = CCLabelTTF::create(name.c_str(), "HelveticaNeue-BoldItalic", 15);
    pLabel6->setColor(COLOR_YELLOW);
    pLabel6->setDimensions(CCSizeMake(238 / 2, 28));
    pLabel6->setHorizontalAlignment(kCCTextAlignmentCenter);
    pLabel6->setVerticalAlignment(kCCVerticalTextAlignmentCenter);
    pLabel6->setAnchorPoint(ccp(0,0));
    pLabel6->setPosition(accp(7, 775 - 20 - MAIN_LAYER_BTN_HEIGHT));
    pLabel6->setTag(1909);
    dojoInstance->addChild(pLabel6, 50);
}

void DojoLayerDojo::InitLayer(){
    //////////////////////////////////////////////////
    
    
    // build version
    CCLabelTTF* pLabel0 = CCLabelTTF::create("130319", "Verdana", 14);
    pLabel0->setColor(COLOR_WHITE);
    pLabel0->setAnchorPoint(accp(0, 0));
    pLabel0->setPosition(accp(10, 90));
    this->addChild(pLabel0, 5000);

    CCSprite* pSprSubUIBg     = CCSprite::create("ui/home/ui_menu_sub_bg_a.png");
    pSprSubUIBg->setAnchorPoint(ccp(0,0));
    pSprSubUIBg->setPosition(accp(0.0f, 0.0f));
    pSprSubUIBg->setTag(598);
    MainScene::getInstance()->addChild(pSprSubUIBg, 100);

    CCMenuItemImage *pSpr1 = CCMenuItemImage::create("ui/home/ui_menu_sub_01_1.png","ui/home/ui_menu_sub_01_2.png",this,menu_selector(DojoLayerDojo::SubUICallback));
    CCMenuItemImage *pSpr2 = CCMenuItemImage::create("ui/home/ui_menu_sub_02_1.png","ui/home/ui_menu_sub_02_2.png",this,menu_selector(DojoLayerDojo::SubUICallback));
    CCMenuItemImage *pSpr3 = CCMenuItemImage::create("ui/home/ui_menu_sub_02_1.png","ui/home/ui_menu_sub_02_2.png",this,menu_selector(DojoLayerDojo::SubUICallback));
    CCMenuItemImage *pSpr4 = CCMenuItemImage::create("ui/home/ui_menu_sub_02_1.png","ui/home/ui_menu_sub_02_2.png",this,menu_selector(DojoLayerDojo::SubUICallback));
    CCMenuItemImage *pSpr5 = CCMenuItemImage::create("ui/home/ui_menu_sub_03_1.png","ui/home/ui_menu_sub_03_2.png",this,menu_selector(DojoLayerDojo::SubUICallback));
    
    pSpr1->setTag(11);
    pSpr2->setTag(12);
    pSpr3->setTag(13);
    pSpr4->setTag(14);
    pSpr5->setTag(15);
    
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr2->setAnchorPoint(ccp(0,0));
    pSpr3->setAnchorPoint(ccp(0,0));
    pSpr4->setAnchorPoint(ccp(0,0));
    pSpr5->setAnchorPoint(ccp(0,0));
    
    pSpr1->setPosition(accp(7,88));
    pSpr2->setPosition(accp(124,88));
    pSpr3->setPosition(accp(250,88));
    pSpr4->setPosition(accp(376,88));
    pSpr5->setPosition(accp(502,88));
    
    CCMenu* pSubMenu = CCMenu::create(pSpr1, pSpr2, pSpr3, pSpr4, pSpr5, NULL);
    pSubMenu->setPosition( accp(0,0));
    pSubMenu->setTag(599);
    //this->addChild(pSubMenu,120);
    MainScene::getInstance()->addChild(pSubMenu,120);
    
    pSpr1->selected();
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("홈", "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("친구", "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("아이템", "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("카드수집", "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel5 = CCLabelTTF::create("설정", "HelveticaNeue-Bold", 11);
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel3->setColor(subBtn_color_normal);
    pLabel4->setColor(subBtn_color_normal);
    pLabel5->setColor(subBtn_color_normal);
    pLabel1->setTag(21);
    pLabel2->setTag(22);
    pLabel3->setTag(23);
    pLabel4->setTag(24);
    pLabel5->setTag(25);
    
    pLabel1->setColor(subBtn_color_selected);
    
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp( 70, 100), pLabel1,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(194, 100), pLabel2,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(320, 100), pLabel3,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(447, 100), pLabel4,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(570, 100), pLabel5,130);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    BattleLogIcon = CCSprite::create("ui/home/home_btn_a1.png");
    BattleLogIcon->setTag(QUICK_ICON_BATTLELOG);
    BattleLogIcon->setAnchorPoint(ccp(0.0f, 0.0f));
    BattleLogIcon->setPosition(accp(7.0f, 598.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT));
    this->addChild(BattleLogIcon, 10);
    
    // -- 아이콘 w = 91
    CCLabelTTF* battleLabel = CCLabelTTF::create("최근방어","Thonburi", 8);
    battleLabel->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(7.0f + (91.0f/2.0f), 598.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT + 12.0f), battleLabel, 11);
    
    EventLogIcon = CCSprite::create("ui/home/home_btn_b1.png");
    EventLogIcon->setTag(QUICK_ICON_EVENTLOG);
    EventLogIcon->setAnchorPoint(ccp(0.0f, 0.0f));
    EventLogIcon->setPosition(accp(7.0f, 515.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT));
    this->addChild(EventLogIcon, 10);
    
    CCLabelTTF* eventLabel = CCLabelTTF::create("이벤트","Thonburi", 8);
    eventLabel->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(7.0f + (91.0f/2.0f), 515.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT + 12.0f), eventLabel, 11);

    
    GiftIcon = CCSprite::create("ui/home/home_btn_c1.png");
    GiftIcon->setTag(QUICK_ICON_GIFT);
    GiftIcon->setAnchorPoint(ccp(0.0f, 0.0f));
    GiftIcon->setPosition(accp(7.0f, 432.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT));
    this->addChild(GiftIcon, 10);
    
    CCLabelTTF* giftLabel = CCLabelTTF::create("우편함","Thonburi", 8);
    giftLabel->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(7.0f + (91.0f/2.0f), 432.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT + 12.0f), giftLabel, 11);
    
    BgIcon = CCSprite::create("ui/home/home_btn_d1.png");
    BgIcon->setTag(QUICK_ICON_BG);
    BgIcon->setAnchorPoint(ccp(0.0f, 0.0f));
    BgIcon->setPosition(accp(7.0f, 349.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT));
    this->addChild(BgIcon, 10);
    
    CCLabelTTF* bgLabel = CCLabelTTF::create("배경설정","Thonburi", 8);
    bgLabel->setColor(subBtn_color_normal);
    registerLabel( this,ccp(0.5,0), accp(7.0f + (91.0f/2.0f), 349.0f + 40.0f - MAIN_LAYER_BTN_HEIGHT + 12.0f), bgLabel, 11);
    
    
/*
    
    
    CCSprite* TracerIcon = CCSprite::create("ui/home/home_btn_b1.png");
    TracerIcon->setTag(QUICK_ICON_TRACER);
    TracerIcon->setAnchorPoint(ccp(0.0f, 0.0f));
    TracerIcon->setPosition(accp(7.0f, 266.0f - MAIN_LAYER_BTN_HEIGHT));
    this->addChild(TracerIcon, 10);
    
    CCLabelTTF* tracerLabel = CCLabelTTF::create("추적자", "Thonburi", 8);
    tracerLabel->setColor(subBtn_color_normal);
    registerLabel(this, ccp(0.5, 0), accp(7.0f + (91.0f/2.0f), 266.0f - MAIN_LAYER_BTN_HEIGHT + 12.0f), tracerLabel, 11);
    
  */  
    
 
    
}

void DojoLayerDojo::setEnableSubMenu(bool flag)
{
    CCMenu* pMenu = (CCMenu*)MainScene::getInstance()->getChildByTag(599);
    
    if (pMenu != NULL){
        pMenu->setEnabled(flag);
    }
}

void DojoLayerDojo::SubUICallback(CCObject* pSender){
    
    MainScene::getInstance()->removePopup();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if (tag >= 11 && tag <= 15)
    {
        CCMenu *menu = (CCMenu*)MainScene::getInstance()->getChildByTag(599);
        if (menu)
        {
            for (int scan = 0;scan < 5;scan++)
            {
                CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(scan + 11);
                if (!item)
                    continue;
                item->unselected();
            }
        }
    }
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    item->selected();
    
    if (curLayerTag == tag)return;
    
    curLayerTag = tag;

    soundButton1();
    
    SetNormalSubBtns();
    SetSelectedSubBtn(tag);
    
    if (tag == 11){
        MainScene::getInstance()->setRivalListRefresh();
    }
    else{
        MainScene::getInstance()->cancelRivalListRefresh();
    }
    
    switch(tag){
        case 11: // home
            ReleaseLayer();
            this->setTouchEnabled(true);
            break;
        case 12: // social
        {
            ARequestSender::getInstance()->requestFriendsToGameServer();
            
            const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_FRIEND);
            
            if(false == TutorialCompleted)
            {
                const bool TutorialMode = false;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                tutorialProgress = TUTORIAL_FRIEND_1;
                basePopUp->InitUI(&tutorialProgress);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
            }
        }
            break;
        case 13: // item
            ARequestSender::getInstance()->requestItemListAsync();
            //ReleaseLayer();
            //InitItemLayer(0);
            break;
        case 14: // collection
            ARequestSender::getInstance()->requestCollection();
            //ReleaseLayer();
            //InitDojoCollectLayer();
            break;
        case 15: // option
            ReleaseLayer();
            InitOptionLayer();
            break;
    }
}

void DojoLayerDojo::SetNormalSubBtns(){
    for(int i=0;i<5;i++){
        CCLabelTTF* pLabel0 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(21+i);
        pLabel0->setColor(subBtn_color_normal);
    }
}

void DojoLayerDojo::SetSelectedSubBtn(int tag){
    CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(tag+10);
    if (pLabel1 != 0){
        pLabel1->setColor(COLOR_YELLOW);
    }
}

void DojoLayerDojo::loadRevengeGauge()
{
    char buffer[64];
    for (int loop = 0;loop < 4;loop++)
    {
        for (int scan = 0;scan < 3;scan++)
        {
            sprintf(buffer, "ui/home/gage_revenge0%d.png", scan + 1);
            revengeGauge[loop * 3 + scan] = CCSprite::create(buffer);
            revengeGauge[loop * 3 + scan]->setAnchorPoint(ccp(0, 0));
            this->addChild(revengeGauge[loop * 3 + scan], 50);
        }
    }
}

void DojoLayerDojo::hideRevengeGauge()
{
    for (int scan = 0;scan < 12;scan++)
    {
        revengeGauge[scan]->setOpacity(0);
    }
}

void DojoLayerDojo::refreshRevengeGauge()
{
    hideRevengeGauge();
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
//    playerInfo->revengePoint = 9;
    float ratio = (float)playerInfo->revengePoint / 10.0f;
    int x = 13;
    if (ratio <= 0.0f)
        return;
    drawSingleRevengeGauge(0, x, (ratio >= 0.25f) ? 1.0f : ratio);
    if (ratio >= 0.25f)
    {
        drawSingleRevengeGauge(1, x + 52 + 6, (ratio >= 0.5f) ? 1.0f : fmod(ratio, 0.25f));
        if (ratio >= 0.5f)
        {
            drawSingleRevengeGauge(2, x + (52 + 6) * 2, (ratio >= 0.75f) ? 1.0f : fmod(ratio, 0.25f));
            if (ratio >= 0.75f)
                drawSingleRevengeGauge(3, x + (52 + 6) * 3, fmod(ratio, 0.25f));
        }
    }
}

void DojoLayerDojo::drawSingleRevengeGauge(int index, int x, float scale)
{
    revengeGauge[index * 3 + 0]->setPosition(accp(x, 714-MAIN_LAYER_BTN_HEIGHT));
    revengeGauge[index * 3 + 0]->setOpacity(255);
    x += 4;
    float value = scale * 44.0f;
    revengeGauge[index * 3 + 1]->setPosition(accp(x, 714-MAIN_LAYER_BTN_HEIGHT));
    revengeGauge[index * 3 + 1]->setScaleX(value);
    revengeGauge[index * 3 + 1]->setOpacity(255);
    x += (value);
    revengeGauge[index * 3 + 2]->setPosition(accp(x, 714-MAIN_LAYER_BTN_HEIGHT));
    revengeGauge[index * 3 + 2]->setOpacity(255);
}

void DojoLayerDojo::InitBgSelectLayer()
{
    if (m_DojoLayerBg != NULL)
    {
        this->removeChild(m_DojoLayerBg, true);
        m_DojoLayerBg = NULL;
    }
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (m_DojoLayerBg == NULL)
    {
        m_DojoLayerBg = new DojoLayerBg(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_DojoLayerBg->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_DojoLayerBg,60);
        this->setTouchEnabled(false);
        m_DojoLayerBg->setTouchEnabled(true);
    }
    else
    {
        m_DojoLayerBg->_setZOrder(20);
    }

}

void DojoLayerDojo::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    //    return;
    
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=1000; i<QUICK_ICON_TOTAL; ++i)
    {
        if(GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            if(1000 == i)
                ChangeSpr(this, i, "ui/home/home_btn_a2.png", 10);
            if(1001 == i)
                ChangeSpr(this, i, "ui/home/home_btn_b2.png", 10);
            if(1002 == i)
                ChangeSpr(this, i, "ui/home/home_btn_c2.png", 10);
            if(1003 == i)
                ChangeSpr(this, i, "ui/home/home_btn_d2.png", 10);
            if(1004 == i)
                addPageLoading();
            
            
            
            /*
            if(i == QUICK_ICON_TRACER)
            {
                ResponseRivalList* rivalListInfo = ARequestSender::getInstance()->requestRivalList();
                isExistRival = false;
                
                for (int i=0; i<rivalListInfo->rivals->count(); i++)
                {
                    if (((AReceivedRival* )rivalListInfo->rivals->objectAtIndex(i))->cur_hp > 0
                        && ((AReceivedRival* )rivalListInfo->rivals->objectAtIndex(i))->limit > time(NULL))
                    {
                        isExistRival = true;
                    }
                }
                
                if (isExistRival == true)
                {
                    ChangeSpr(this, i, "ui/home/rival_btn_appear_a2.png", 10);
                }
                else
                {
                    ChangeSpr(this, i, "ui/home/rival_btn_list_a2.png", 10);
                }
            }
             */
 
        }
    }
    
    bTouchPressed = true;
}

void DojoLayerDojo::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void DojoLayerDojo::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    //    return;
    
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=1000; i<QUICK_ICON_TOTAL; ++i)
    {
        if(GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            soundButton1();
            
            MainScene::getInstance()->removePopup();
            
            switch (i)
            {
                case QUICK_ICON_BATTLELOG:
                {
                    ChangeSpr(this, i, "ui/home/home_btn_a1.png", 10);
                    
                    if(false == battlelogActive)
                    {
                        this->setTouchEnabled(false);
                        InitBattleLogLayer();
                    }
                    
                    if(pBattleLogLayer)
                    {
                        CCEaseOut *fadeOut3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 5.0f);
                        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::BattleLogTourchStart));
                        pBattleLogLayer->runAction(CCSequence::actionOneTwo(fadeOut3, callBack3));
                    }                    
                }
                    break;
                    
                case QUICK_ICON_EVENTLOG:
                {
                    ChangeSpr(this, i, "ui/home/home_btn_b1.png", 10);
                    
                    if(false == eventlogActive)
                    {
                        this->setTouchEnabled(false);
                        InitEventLayer();
                    }
                    
                    if(pEventLayer)
                    {
                        CCEaseOut *fadeOut3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 5.0f);
                        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::EventLogTourchStart));
                        pEventLayer->runAction(CCSequence::actionOneTwo(fadeOut3, callBack3));
                    }
                    
                }
                    break;
                    
                case QUICK_ICON_GIFT:
                {
                    ChangeSpr(this, i, "ui/home/home_btn_c1.png", 10);
                
                    ARequestSender::getInstance()->requestGiftListAsync();
                }
                    break;
                    
                case QUICK_ICON_BG:
                {
                    ChangeSpr(this, i, "ui/home/home_btn_d1.png", 10);
                    
                    ARequestSender::getInstance()->requestBgList();
                    //InitBgSelectLayer();
                }
                    break;
                    
                    
                    
                    
#if(1)
                case QUICK_ICON_TRACER:
                {
//                    addPageLoading();
                    
//                    ChangeSpr(this, i, "ui/home/home_btn_b1.png", 10);
                    
//                    initTracerLayer();
                    /*
                    ResponseRivalList* rivalListInfo = ARequestSender::getInstance()->requestRivalList();
                    isExistRival = false;
                    
                    for (int i=0; i<rivalListInfo->rivals->count(); i++)
                    {
                        if (((AReceivedRival* )rivalListInfo->rivals->objectAtIndex(i))->cur_hp > 0
                            && ((AReceivedRival* )rivalListInfo->rivals->objectAtIndex(i))->limit > time(NULL))
                        {
                            isExistRival = true;
                        }
                    }
                    
                    if (isExistRival == true)
                    {
                        ChangeSpr(this, i, "ui/home/rival_btn_appear_a1.png", 10);
                    }
                    else
                    {
                        ChangeSpr(this, i, "ui/home/rival_btn_list_a1.png", 10);
                    }
                    */
                    this->HideMenu();
                    this->setTouchEnabled(false);
                    MainScene::getInstance()->HideMainMenu();

                    initTraceHistoryLayer();
                }
                    break;
#endif
                    
                    
                    
                    
                default:
                    break;
            }
        }
        else{
            removePageLoading();
        }
    }
}

void DojoLayerDojo::ChangeBGImg(int idx)
{
    if(0 ==idx) return;
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    PlayerInfo* pInfo = (PlayerInfo*)PlayerInfo::getInstance();
    pInfo->setBackground(idx);
    
    Bg_List* bglist = (Bg_List*)BgDictionary->objectForKey(idx);
    
    this->removeChild(pBgSprite, true);
    
    string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
    path+=bglist->L_ImgPath;
    
    pBgSprite = CCSprite::create(path.c_str());
    pBgSprite->setScale(MAIN_BG_SCALE);
    pBgSprite->setAnchorPoint(ccp(0.0f, 0.5f));
    pBgSprite->setPosition( ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2) );

    
    this->addChild(pBgSprite);
    
    CCFiniteTimeAction* actionMove1 = CCMoveTo::actionWithDuration(10.0f, ccp(-180.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2));
    CCFiniteTimeAction* actionMove2 = CCMoveTo::actionWithDuration(10.0f, ccp(0.0f, size.height/2 - MAIN_LAYER_BTN_HEIGHT/2));
    CCRepeatForever *repeat = CCRepeatForever::actionWithAction((CCSequence*)CCSequence::actions(actionMove1, actionMove2, NULL));
    pBgSprite->runAction(repeat);
}

void DojoLayerDojo::InitMainBGNameList()
{
    int BgID = 5001;
    
    for(int i=1; i<300; ++i)
    {
        char L_Path[120] = {0};
        char S_Path[120] = {0};
        
        sprintf(L_Path, "bg_%d.png", i);
        sprintf(S_Path, "ui/main_bg/bg_%ds.png", i);
        
        Bg_List* bgList = new Bg_List;
        bgList->L_ImgPath = L_Path;
        bgList->S_IMgPath = S_Path;
        
        BgDictionary->setObject(bgList, BgID);
        ++BgID;
    }
}

void DojoLayerDojo::scrollingEnd()
{
    this->stopAllActions();
}

void DojoLayerDojo::releaseBattleLog()
{
    if(pBattleLogLayer)
    {
        CCEaseOut *fadeOut4 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(700/SCREEN_ZOOM_RATE, 0)), 5.0f);
        CCCallFunc *callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::BattleLogTourchEnd));
        pBattleLogLayer->runAction(CCSequence::actionOneTwo(fadeOut4, callBack4));
    }
}

void DojoLayerDojo::releaseBattleLogImmediately()
{
    if(pBattleLogLayer)
    {
        CCEaseOut *fadeOut4 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.0f, ccp(700/SCREEN_ZOOM_RATE, 0)), 5.0f);
        CCCallFunc *callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::BattleLogTourchEnd));
        pBattleLogLayer->runAction(CCSequence::actionOneTwo(fadeOut4, callBack4));
    }
}

void DojoLayerDojo::BattleLogTourchEnd()
{
    RemoveDetailBattleLogLayer();
    RemoveBattleLogLayer(); // android 에서는 이안에서 에러
}

void DojoLayerDojo::BattleLogTourchStart()
{
    InitDetailBattleLogLayer();
}





void DojoLayerDojo::initTracerLayer()
{

}

void DojoLayerDojo::initTraceHistoryLayer()
{
    CCSize size = GameConst::WIN_SIZE;  //CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0.0f, 0.0f));
    pSprBG->setContentSize(CCSize(size.width, size.height));
    pSprBG->setPosition(accp(0.0f, 0.0f));
    pSprBG->setTag(586);
    MainScene::getInstance()->addChild(pSprBG, 200);
    
    MainScene::getInstance()->cancelRivalListRefresh();

//    addPageLoading();
    bool startTrace = true;
    
    FileManager* fManager = FileManager::sharedFileManager();
    std::vector<std::string> downloads;
    std::string enemyImgPath = FOC_IMAGE_SERV_URL;
    enemyImgPath.append("images/cha/cha_l/");
    
    ResponseRivalList* rivalListInfo = ARequestSender::getInstance()->requestRivalList();
    if (atoi(rivalListInfo->res) != 0){
//        removePageLoading();
        popupNetworkError(rivalListInfo->res, "rival list error","");
        return;
    }
    
    for (int i=0; i<rivalListInfo->rivals->count(); i++)
    {
        AReceivedRival* rivalInfo = (AReceivedRival* )rivalListInfo->rivals->objectAtIndex(i);
        NpcInfo* npc = MainScene::getInstance()->getNpc(rivalInfo->npc_id);
        CCLog("Npc Image List for Trace History : %s", npc->npcImagePath);
        if (!fManager->IsFileExist(npc->npcImagePath))
        {
            startTrace = false;
            string downPath = enemyImgPath + npc->npcImagePath;
            downloads.push_back(downPath);
            CCLog("Downloads Npc Image for Trace History : %s", downPath.c_str());
        }
    }
    
    
    if (false == startTrace)
    {
        CCHttpRequest* requestActor = CCHttpRequest::sharedHttpRequest();
        requestActor->addDownloadTask(downloads, this, callfuncND_selector(DojoLayerDojo::onHttpRequestCompleted));
    }
    else
    {
        CCLog("downloading enemy image for trace history is completed!");
//        removePageLoading();
        
        /////////////////////////////
        //
        // Create Trace History Layer
        
        pTraceHistoryLayer = NULL;
        
        //DojoLayerDojo::getInstance()->bSkipRefreshRivalList = true;

        if (pTraceHistoryLayer == NULL)
        {
            pTraceHistoryLayer = new TraceHistoryLayer(CCSize(size.width, size.height));
            pTraceHistoryLayer->setAnchorPoint(ccp(0.0f, 0.0f));
            MainScene::getInstance()->addChild(pTraceHistoryLayer, 200);
            pTraceHistoryLayer->setTouchEnabled(true);
        }
        else
        {
            MainScene::getInstance()->addChild(pTraceHistoryLayer, 200);
        }
    }

}

void DojoLayerDojo::onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data)
{
    CCSize size = GameConst::WIN_SIZE;
    
    HttpResponsePacket* response = (HttpResponsePacket* )data;
    if (response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLog("enemy images for trace history download complete");
//        removePageLoading();
        
        /////////////////////////////
        //
        // Create Trace History Layer
        
        pTraceHistoryLayer = NULL;
        
        //DojoLayerDojo::getInstance()->bSkipRefreshRivalList = true;
        
        if (pTraceHistoryLayer == NULL)
        {
            pTraceHistoryLayer = new TraceHistoryLayer(CCSize(size.width, size.height));
            pTraceHistoryLayer->setAnchorPoint(ccp(0.0f, 0.0f));
            MainScene::getInstance()->addChild(pTraceHistoryLayer, 200);
            pTraceHistoryLayer->setTouchEnabled(true);
        }
        else
        {
            MainScene::getInstance()->addChild(pTraceHistoryLayer, 200);
        }
    }
}




void DojoLayerDojo::EventLogTourchEnd()
{
    RemoveDetailEventLayer();
    RemoveEventLogLayer();
}

void DojoLayerDojo::EventLogTourchStart()
{
    InitDetailEventLayer();
}

void DojoLayerDojo::InitEventLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (pEventLayer != NULL)
    {
        this->removeChild(pEventLayer, true);
        delete pEventLayer;
        pEventLayer = NULL;
    }

    if(pEventLayer == NULL)
    {
        pEventLayer = new EventLayer(CCSize(size.width, size.height));
        pEventLayer->setAnchorPoint(ccp(0, 0));
        pEventLayer->setPosition(accp(-650, 0));
        this->addChild(pEventLayer, 60);
    }
    else
    {
        pEventLayer->_setZOrder(20);
    }
}

void DojoLayerDojo::releaseEvent()
{
    if(pEventLayer)
    {
        CCEaseOut *fadeOut3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(-(700/SCREEN_ZOOM_RATE), 0)), 5.0f);
        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::EventLogTourchEnd));
        pEventLayer->runAction(CCSequence::actionOneTwo(fadeOut3, callBack3));
    }
}

void DojoLayerDojo::releaseEventImmediately()
{
    if(pEventLayer)
    {
        CCEaseOut *fadeOut3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.0f, ccp(-700/SCREEN_ZOOM_RATE, 0)), 5.0f);
        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(DojoLayerDojo::EventLogTourchEnd));
        pEventLayer->runAction(CCSequence::actionOneTwo(fadeOut3, callBack3));
    }
}

void DojoLayerDojo::RemoveEventLogLayer()
{
    eventlogActive = false;
    
    if (pEventLayer != NULL)
    {
        this->removeChild(pEventLayer, true);
        pEventLayer->autorelease();// delete pEventLayer;
        pEventLayer = NULL;
    }
}

void DojoLayerDojo::InitDetailEventLayer()
{
    if(pEventLayer)
    {
        eventlogActive = true;
    }
}

void DojoLayerDojo::RemoveDetailEventLayer()
{
    
}

void DojoLayerDojo::InitBattleLogLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (pBattleLogLayer != NULL)
    {
        this->removeChild(pBattleLogLayer, true);
        delete pBattleLogLayer;
        pBattleLogLayer = NULL;
    }

    if (pBattleLogLayer == NULL)
    {
        pBattleLogLayer = new BattleLogLayer(CCSize(size.width, size.height));
        pBattleLogLayer->setAnchorPoint(ccp(0, 0));
        pBattleLogLayer->setPosition(accp(650, 0));
        this->addChild(pBattleLogLayer, 160);
    }
    else
    {
        pBattleLogLayer->_setZOrder(20);
    }
}

void DojoLayerDojo::RemoveBattleLogLayer()
{
    battlelogActive = false;
    
    if (pBattleLogLayer != NULL)
    {
        this->removeChild(pBattleLogLayer, true);
        pBattleLogLayer->autorelease();
        //delete pBattleLogLayer;
        pBattleLogLayer = NULL;
    }
}

void DojoLayerDojo::InitDetailBattleLogLayer()
{
    ARequestSender::getInstance()->requestNotice();
}

void DojoLayerDojo::RemoveDetailBattleLogLayer()
{
    if (pBattleLogListLayer != NULL)
    {
        pBattleLogLayer->removeChild(pBattleLogListLayer, true);
        delete pBattleLogListLayer;
        pBattleLogListLayer = NULL;
    }
}

void DojoLayerDojo::ReleaseLayer()
{
    MainScene::getInstance()->removePopup();
    
    releaseEventImmediately();
    releaseBattleLogImmediately();
   
    if (pSocialLayer != NULL)
    {
        this->removeChild(pSocialLayer, true);
        pSocialLayer = NULL;
    }

    if (pItemLayer != NULL)
    {
        this->removeChild(pItemLayer, true);
        pItemLayer = NULL;
    }
    
    if (pOptionLayer != NULL)
    {
        this->removeChild(pOptionLayer, true);
        pOptionLayer = NULL;
    }
    
    if (pDojoLayerCollect != NULL)
    {
        this->removeChild(pDojoLayerCollect, true);
        pDojoLayerCollect = NULL;
    }
    
    if (m_DojoLayerBg != NULL)
    {
        this->removeChild(m_DojoLayerBg, true);
        m_DojoLayerBg = NULL;        
    }
}

void DojoLayerDojo::InitSocialLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    this->setTouchEnabled(false);
    
    if (pSocialLayer == NULL)
    {
        pSocialLayer = new SocialLayer(CCSize(size.width, size.height));
        pSocialLayer->setAnchorPoint(ccp(0, 0));
        pSocialLayer->setPosition(accp(0, 0));
        this->addChild(pSocialLayer, 60);
    }
    else
    {
        pSocialLayer->_setZOrder(20);
    }
}

void DojoLayerDojo::InitItemLayer(int tab)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    this->setTouchEnabled(false);
    
    if (pItemLayer == NULL)
    {
        pItemLayer = new ItemLayer(CCSize(size.width, size.height), tab);
        pItemLayer->setAnchorPoint(ccp(0, 0));
        pItemLayer->setPosition(accp(0, 0));
        this->addChild(pItemLayer, 60);
    }
    else
    {
        pSocialLayer->_setZOrder(20);
    }
}

void DojoLayerDojo::InitDojoCollectLayer(ResponseCollectionInfo* _collectionInfo)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    this->setTouchEnabled(false);
    
    if (pDojoLayerCollect == NULL)
    {
        pDojoLayerCollect = new DojoLayerCollect(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        pDojoLayerCollect->setPosition(accp(0,0));
        pDojoLayerCollect->setCollectData(_collectionInfo);
        pDojoLayerCollect->InitUI();
        this->addChild(pDojoLayerCollect,70);
        pDojoLayerCollect->setTouchEnabled(true);
    }
    else
    {
        pDojoLayerCollect->_setZOrder(20);
    }
}

void DojoLayerDojo::InitOptionLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    this->setTouchEnabled(false);
    
    if (pOptionLayer == NULL)
    {
        pOptionLayer = new OptionLayer(CCSize(size.width, this->getContentSize().height - accp(MAIN_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT)));
        pOptionLayer->setAnchorPoint(ccp(0, 0));
        pOptionLayer->setPosition(accp(0, 0));
        this->addChild(pOptionLayer, 60);
    }
    else
    {
        pOptionLayer->_setZOrder(20);
    }    
}

void DojoLayerDojo::HideMenu()
{
    CCSprite* pSprSubUIBg     = (CCSprite*)MainScene::getInstance()->getChildByTag(598);
    pSprSubUIBg->setPosition(ccp(10000,-10000));
    
    CCMenu* pSubMenu = (CCMenu*) MainScene::getInstance()->getChildByTag(599);
    pSubMenu->setPosition(ccp(10000,-10000));
    
    for(int i=0;i<4;i++){
        CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(21+i);
        pLabel1->setPositionY(pLabel1->getPositionY()-1000);
    }
}

void DojoLayerDojo::ShowMenu()
{
    CCSprite* pSprSubUIBg     = (CCSprite*)MainScene::getInstance()->getChildByTag(598);
    if (pSprSubUIBg){
        pSprSubUIBg->setPosition(CCPointZero);
    }
    
    CCMenu* pSubMenu = (CCMenu*) MainScene::getInstance()->getChildByTag(599);
    if (pSubMenu){
        pSubMenu->setPosition( CCPointZero);
    }
    
    for(int i=0;i<4;i++){
        CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(21+i);
        pLabel1->setPositionY(accp(100.0f));
    }
}


void DojoLayerDojo::refreshRivalEvent()
{
    //if (bSkipRefreshRivalList)return;
    
    if (MainScene::getInstance()->getCurLayer() == MainScene::MAIN_LAYER_DOJO){
    
        ResponseRivalList* pRivalListInfo = ARequestSender::getInstance()->requestRivalList();
    
        if (pRivalListInfo){
            if (atoi(pRivalListInfo->res) == 0){
                CCLog("refreshRivalList");
                if (checkNewRivalEvent(pRivalListInfo)){
                    InitRivalUI(true);
                }
                else{
                    InitRivalUI(false);
                }
            }
        }
    }
    else{
        MainScene::getInstance()->cancelRivalListRefresh();
    }
}

bool DojoLayerDojo::checkNewRivalEvent(ResponseRivalList* pRivalListInfo)
{
    bool isExistedAliveRival = false;
    
    if (pRivalListInfo){
        for (int i=0; i<pRivalListInfo->rivals->count(); i++)
        {
            if ((((AReceivedRival* )pRivalListInfo->rivals->objectAtIndex(i))->cur_hp > 0) && (((AReceivedRival* )pRivalListInfo->rivals->objectAtIndex(i))->limit - time(NULL) > 0))
            {
                isExistedAliveRival = true;
            }
        }
        
        return isExistedAliveRival;
    }
    return false;
    
}

void DojoLayerDojo::refreshRivalNotiUI(ResponseRivalList* pRivalListInfo)
{
    if (checkNewRivalEvent(pRivalListInfo)){
        InitRivalUI(true);
    }
    else{
        InitRivalUI(false);
    }
}


/////////////////////////////////////////////////////////////////////////////////////////////////////



ExitWarningPopup::ExitWarningPopup()
{
    InitUI();
}

ExitWarningPopup::~ExitWarningPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void ExitWarningPopup::InitUI()
{
    
    CCSize size = GameConst::WIN_SIZE;
    
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.5f, 0.5f));
    
    int yy = size.height/2 - accp(186);//accp(500);
    popupBG->setPosition(ccp(size.width/2, size.height/2));// accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    const char* text2 = " 'FOC'를 종료하시겠습니까?";
    CCLabelTTF* Label2 = CCLabelTTF::create(text2, "Thonburi", 12);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, yy + accp(195)), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(101);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(ccp(accp(93.0f), yy + accp(5)));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(ccp(accp(342.0f), yy + accp(5)));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(194.0f), yy + accp(15)), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), ccp(accp(443.0f), yy + accp(15)), RightLabel, 160);
    
    nCallFrom = 0;
    
}


void ExitWarningPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{

    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    

    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    ////////////////////////////////////////////////////////////// OK
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        this->removeAllChildrenWithCleanup(true);
        
        if (nCallFrom == 0){
            MainScene::getInstance()->removePopup();
            DojoLayerDojo::getInstance()->bExitPopup = false;
            
            ///////////////////
            //
            // 추적 배경 음악 정지
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG->stopBackgroundMusic();
            }
        }
        else if (nCallFrom == 1){
            KakaoLoginScene::getInstance()->removeChildByTag(123, true);
            KakaoLoginScene::getInstance()->bExitPopup = false;
        }
        
        CCDirector::sharedDirector()->end();
    }
    
    ////////////////////////////////////////////////////////////// cancel
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        this->removeAllChildrenWithCleanup(true);
        
        if (nCallFrom== 0){
            MainScene::getInstance()->removePopup();
            DojoLayerDojo::getInstance()->bExitPopup = false;
            
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
        }
        else if (nCallFrom == 1){
            KakaoLoginScene::getInstance()->removeChildByTag(123, true);
            KakaoLoginScene::getInstance()->bExitPopup = false;
        }
    }
}