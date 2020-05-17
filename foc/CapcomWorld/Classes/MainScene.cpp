//
//  MainScene.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 18..
//
//

#include "MainScene.h"
#include "XBridge.h"
#include "CCHttpRequest.h"
#include "AKakaoUser.h"
#include "AResponseParser.h"
#include "ARequestSender.h"
#include "TitleScene.h"
#include "KakaoLoginScene.h"
#include "ResponseBasic.h"
#include "EventInfo.h"
#include "Tutorial.h"
#include "ShopRoulette.h"

using namespace cocos2d;
using namespace CocosDenshion;// CocosDenshion;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif

MainScene *MainScene::instance = NULL;

CCScene* MainScene::scene()
{
    CCLog("mainScene.scene()");
    
    // 'scene' is an autorelease object
    CCScene *scene = CCScene::create();
    
    // 'layer' is an autorelease object
    MainScene *layer = MainScene::create();
    
    layer->setTag(9);
    // add layer as a child to scene
    scene->addChild(layer);
    //UserInterfaceLayer *newLayer = UserInterfaceLayer::create();
    //newLayer->setPosition(ccp(0,100));
    //scene->addChild(newLayer);
    
        
    //CCLog("titlescene debug msg 2");
    
    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool MainScene::init()
{
    m_battleLayer = NULL;

//    CCLog("mainScene.init()");
    
    this->setTouchEnabled(true);
    
    instance = this;
    
    //////////////////////////////
    // 1. super init first
    if ( !CCLayer::init() )
    {
        return false;
    }
    
    resultBG->preloadEffect("audio/bgm_Results_01.mp3");
    
//    playerInfo = PlayerInfo::getInstance();
    
    readNPCData();
    readQuestNpcData();
    
    /*
     // getQuestNpc  사용예
     CCArray* npcCodeList = getQuestNpc(20011);
     
     for(int i=0;i<npcCodeList->count();i++){
         CCInteger* a =  (CCInteger*)npcCodeList->objectAtIndex(i);
         int val = a->getValue();
         CCLog("npc id:%d",val);
     }
    */
    
    nCardListFilter = 0;
    
    initUI();
    
    //pLevelUpIcon = NULL;
    
    this->schedule(schedule_selector(MainScene::refreshUserStat),60.0);
    
    setRivalListRefresh();
    
/*    xb = new XBridge(); //XBridge *xb = new XBridge();
    playerInfo->SetDeviceID(xb->getDeviceID());
    xb->kakao();*/
    
    //initUI();
    
    return true;
}



/*
CCPoint MainScene::getCCP(CCPoint a){
    
    CCPoint c = *new CCPoint(a.x / 2, a.y / 2);
    
    return c;
    
}

CCPoint MainScene::accp(float x, float y){
    
    CCPoint c = *new CCPoint(x / 2, y / 2);
    return c;
    
}
*/


void MainScene::initUI(){
    
    CCLog("MainScene.initUI");
    
    soundMainBG();
    
    initDojoLayer();
    //curLayerTag = curLayerTag;
    
    CCSize size = GameConst::WIN_SIZE; //CCDirector::sharedDirector()->getWinSize();
    
    //CCSprite* pSprBottomUIBg = CCSprite::create("ui/main/img-main-bg02.png");//HelloWorld.png");
    
    CCLog("MainScene::initUI, win size, w:%f h:%f",size.width, size.height);
    
    CCSprite* pSprBottomUIBg = CCSprite::create("ui/home/ui_menu_bg.png");
    pSprBottomUIBg->setAnchorPoint(ccp(0,0));
    pSprBottomUIBg->setPosition( ccp(0,0) );
    pSprBottomUIBg->setTag(98);
    this->addChild(pSprBottomUIBg, 49);

    //main_menu_btn1
    
    
    CCMenuItemImage *pSprBottomUIBtnDojo = CCMenuItemImage::create("ui/home/ui_menu_home1.png","ui/home/ui_menu_home2.png",this,menu_selector(MainScene::bottomUICallback));
    pSprBottomUIBtnDojo->setAnchorPoint( ccp(0,0));
    pSprBottomUIBtnDojo->setPosition( accp( 1,1));//size.width/5 * 0,0));
    pSprBottomUIBtnDojo->setTag(0);
    pSprBottomUIBtnDojo->selected();
    
    
    
    CCLabelTTF* dojoLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("main_menu_btn1"), "HelveticaNeue-Bold", 11);
    dojoLabel->setColor(subBtn_color_selected);
    dojoLabel->setTag(11);
    registerLabel(this ,ccp(0.5,0), accp(66, 4), dojoLabel, 51);

    CCMenuItemImage *pSprBottomUIBtnCard = CCMenuItemImage::create("ui/home/ui_menu_card1.png","ui/home/ui_menu_card2.png",this,menu_selector(MainScene::bottomUICallback));
    pSprBottomUIBtnCard->setAnchorPoint( ccp(0,0));
    pSprBottomUIBtnCard->setPosition( accp(116,1));//size.width/5 * 1,0));
    pSprBottomUIBtnCard->setTag(1);
    
    CCLabelTTF* cardLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("main_menu_btn2"), "HelveticaNeue-Bold", 11);
    cardLabel->setColor(subBtn_color_normal);
    cardLabel->setTag(12);
    registerLabel(this ,ccp(0.5,0), accp(200, 4), cardLabel, 51);
    
    CCMenuItemImage *pSprBottomUIBtnBattle = CCMenuItemImage::create("ui/home/ui_menu_vs1.png","ui/home/ui_menu_vs2.png",this,menu_selector(MainScene::bottomUICallback));
    pSprBottomUIBtnBattle->setAnchorPoint( ccp(0,0));
    pSprBottomUIBtnBattle->setPosition( accp(244, 1));//ccp( size.width/5 * 2,0));
    pSprBottomUIBtnBattle->setTag(2);
    
    CCLabelTTF* battleLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("main_menu_btn3"), "HelveticaNeue-Bold", 11);
    battleLabel->setColor(subBtn_color_normal);
    battleLabel->setTag(13);
    registerLabel(this ,ccp(0.5,0), accp(330, 4), battleLabel, 51);

    
    CCMenuItemImage *pSprBottomUIBtnItem = CCMenuItemImage::create("ui/home/ui_menu_quest1.png","ui/home/ui_menu_quest2.png",this,menu_selector(MainScene::bottomUICallback));
    pSprBottomUIBtnItem->setAnchorPoint( ccp(0,0));
    pSprBottomUIBtnItem->setPosition( accp( 373, 1));//size.width/5 * 3,0));
    pSprBottomUIBtnItem->setTag(3);
    
    CCLabelTTF* questLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("main_menu_btn4"),"HelveticaNeue-Bold", 11);

    questLabel->setColor(subBtn_color_normal);
    questLabel->setTag(14);
    registerLabel(this ,ccp(0.5,0), accp(458, 4), questLabel, 51);
    
    CCMenuItemImage *pSprBottomUIBtnSocial = CCMenuItemImage::create("ui/home/ui_menu_shop1.png","ui/home/ui_menu_shop2.png",this,menu_selector(MainScene::bottomUICallback));
    pSprBottomUIBtnSocial->setAnchorPoint( ccp(0,0));
    pSprBottomUIBtnSocial->setPosition( accp( 501,1));//size.width/5 * 4,0));
    pSprBottomUIBtnSocial->setTag(4);
    
    CCLabelTTF* shopLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("main_menu_btn5"), "HelveticaNeue-Bold", 11);
    shopLabel->setColor(subBtn_color_normal);
    shopLabel->setTag(15);
    registerLabel(this ,ccp(0.5,0), accp(586, 4), shopLabel, 51);
    
    CCMenu* pMenu = CCMenu::create(pSprBottomUIBtnBattle,pSprBottomUIBtnCard,pSprBottomUIBtnDojo,pSprBottomUIBtnItem,pSprBottomUIBtnSocial, NULL);

    pMenu->setPosition( CCPointZero );
    pMenu->setTag(99);
    this->addChild(pMenu, 50);

	initUserStatLayer();
    
    tutorialProgress = PlayerInfo::getInstance()->getTutorialProgress();

    //if (tutorialProgress < TUTORIAL_TOTAL-1)

    if (tutorialProgress == 0)
    {
        const bool TutorialMode = true;
        NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
        basePopUp->InitUI(&tutorialProgress);
        basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
        basePopUp->setPosition(accp(0.0f, 0.0f));
        basePopUp->setTag(98765);
        this->addChild(basePopUp, 9000);
    }

    this->refreshLevelUpIcon();
}
/*
void MainScene::HitPlay0()
{
    int t = (960.0f - 470.0f - 103.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[0], aniFrame, this, ccp(0.0f, 0.0f), accp(404.0f - 40.0f, t), 3.0f, 600, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void MainScene::HitPlay1()
{
    int t = (960.0f - 470.0f - 8.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[1], aniFrame, this, ccp(0.0f, 0.0f), accp(230.0f + 20.0f, t), 3.0f, 601, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}


void MainScene::HitPlay3()
{

    int t= (960.0f - 470.0f - 70.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[3], aniFrame, this, ccp(0.0f, 0.0f), accp(36.0f - 70.0f, t), 3.0f, 603, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void MainScene::HitPlay4()
{
    int t = (960.0f - 470.0f - 4.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[4], aniFrame, this, ccp(0.0f, 0.0f), accp(-98.0f - 40.0f, t), 3.0f, 604, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void MainScene::HitPlay2()
{
    
    //float t = (960.0f - 470.0f - 121.0f)/2 - 480.0f;
    int t = (960.0f - 470.0f - 121.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    
    CCLog("t=%f",t);
    
    AniPlay(enemyHitEffect[2], aniFrame, this, ccp(0.0f, 0.0f), accp(154.0f - 40.0f, t), 3.0f, 602, 1500, callfuncND_selector(MainScene::removeSpr));
    
    // (960.0f - 470.0f - 121.0f)/2 - 480.0f
    // 184.5 - 480 = -295.5
    
    // accp(-295.5) = -147.75
    
}

void MainScene::InitMyCharacter()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    //CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    //CCLog("===== 아군 데이터 세팅 =====");
    
    CardInfo *card = pInfo->GetCardInDeck(0, 0, 4);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[0] = 220.0 + (512.0f*1.2f)/2;
            myPosY[0] = (size.height*SCREEN_ZOOM_RATE) -70.0f - (512.0f*1.2f) + (512.0f*1.2f)/2;
            myCharacter[0] = CCSprite::create(path.c_str());
            myCharacter[0]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[0]->setScale(1.2f);
            //myCharacter[0]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*1.2f)/2, myPosY[0]));
            myCharacter[0]->setPosition(accp(myPosX[0], myPosY[0]));
            myCharacter[0]->setTag(300);
            this->addChild(myCharacter[0], 99);
        }
    }
    
    card = pInfo->GetCardInDeck(0, 0, 3);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[1] = 172.0f + (512.0f*0.95f)/2;
            myPosY[1] = (size.height*SCREEN_ZOOM_RATE) -25.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            myCharacter[1] = CCSprite::create(path.c_str());
            myCharacter[1]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[1]->setScale(0.95f);
            //myCharacter[1]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*0.95f)/2, myPosY[1]));
            myCharacter[1]->setPosition(accp(myPosX[1], myPosY[1]));
            myCharacter[1]->setTag(301);
            this->addChild(myCharacter[1], 98);
        }
    }
    
    card = pInfo->GetCardInDeck(0, 0, 2);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            //myLeader = path.c_str();
            myPosX[2] = -130.0f + (512.0f*1.55f)/2;
            myPosY[2] = (size.height*SCREEN_ZOOM_RATE) -60.0f - (512.0f*1.55f) + (512.0f*1.55f)/2;
            myCharacter[2] = CCSprite::create(path.c_str());
            myCharacter[2]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[2]->setScale(1.55f);
            //myCharacter[2]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*1.55f)/2, myPosY[2]));
            myCharacter[2]->setPosition(accp(myPosX[2], myPosY[2]));
            myCharacter[2]->setTag(302);
            this->addChild(myCharacter[2], 100);
        }
    }
    
    card = pInfo->GetCardInDeck(0, 0, 1);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[3] = -140.0f + (512.0f*1.08f)/2;;
            myPosY[3] = (size.height*SCREEN_ZOOM_RATE) -80.0f - (512.0f*1.08f) + (512.0f*1.08f)/2;
            myCharacter[3] = CCSprite::create(path.c_str());
            myCharacter[3]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[3]->setScale(1.08f);
            myCharacter[3]->setPosition(accp(myPosX[3], myPosY[3]));
            myCharacter[3]->setTag(303);
            this->addChild(myCharacter[3], 99);
        }
    }
    
    card = pInfo->GetCardInDeck(0, 0, 0);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            //string path = "ui/cha/";
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[4] = -205.0f + (512.0f*0.95f)/2;;
            myPosY[4] = (size.height*SCREEN_ZOOM_RATE) -35.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            myCharacter[4] = CCSprite::create(path.c_str());
            myCharacter[4]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[4]->setScale(0.95f);
            myCharacter[4]->setPosition(accp(myPosX[4], myPosY[4]));
            myCharacter[4]->setTag(304);
            this->addChild(myCharacter[4], 98);
        }
    }
}

void MainScene::DefensePlay0()
{
    playEffect("audio/hit_01.mp3");
    
    int t = (960.0f - 470.0f - 103.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[0], aniFrame, this, ccp(0.0f, 0.0f), accp(404.0f - 40.0f, t), 3.0f, 605, 1500, callfuncND_selector(MainScene::removeSpr));
}

void MainScene::DefensePlay1()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 8.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[1], aniFrame, this, ccp(0.0f, 0.0f), accp(230.0f + 20.0f, t), 3.0f, 606, 1500, callfuncND_selector(MainScene::removeSpr));
}

void MainScene::DefensePlay2()
{
    playEffect("audio/hit_01.mp3");
    int t= (960.0f - 470.0f - 121.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[2], aniFrame, this, ccp(0.0f, 0.0f), accp(154.0f - 40.0f, t), 3.0f, 607, 1500, callfuncND_selector(MainScene::removeSpr));
}

void MainScene::DefensePlay3()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 70.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[3], aniFrame, this, ccp(0.0f, 0.0f), accp(36.0f - 70.0f, t), 3.0f, 608, 1500, callfuncND_selector(MainScene::removeSpr));
}

void MainScene::DefensePlay4()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 4.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[4], aniFrame, this, ccp(0.0f, 0.0f), accp(-98.0f - 40.0f, t), 3.0f, 609, 1500, callfuncND_selector(MainScene::removeSpr));
}


void MainScene::removeSpr(CCNode* sender, void* _tag)
{

    int* tag = (int*)_tag;
    this->removeChildByTag(*tag, true);
    delete tag;
}
*/



////////////////////////////////////////////////////////////////////////

void MainScene::openPurchasePopup(int itemId)
{
    char buffer[64];
    sprintf(buffer, "%d_name", itemId);
    std::string text = LocalizationManager::getInstance()->get(buffer);
    text = text + LocalizationManager::getInstance()->get("purchase_popup_text");
    popupOk(text.c_str());
}

void MainScene::SetNormalSubBtns(){
    for(int i=0;i<5;i++){
        CCLabelTTF* pLabel0 = (CCLabelTTF*)this->getChildByTag(11+i);
        pLabel0->setColor(subBtn_color_normal);
    }
}

void MainScene::SetSelectedSubBtn(int tag){
    CCLabelTTF* pLabel1 = (CCLabelTTF*)this->getChildByTag(tag + 11);
    if (pLabel1 != 0){
        pLabel1->setColor(COLOR_YELLOW);
    }
}

void MainScene::refreshLevelUpIcon()
{
    //if(0!=PlayerInfo::getInstance()->getUpgradePoint())
    //{
    UserStatLayer::getInstance()->AddLevelUpIcon();
    //}
}

/*
void MainScene::AddLevelUpIcon()
{
    RemoveLevelUpIcon();
    
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    if (pMenu->getPositionY()!=0)return;
        
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    pLevelUpIcon = CCMenuItemImage::create("ui/quest/levelup_statbtn1.png", "ui/quest/levelup_statbtn2.png", this, menu_selector(MainScene::LevelUpCallback));
    pLevelUpIcon->setAnchorPoint(ccp(0,0));
    pLevelUpIcon->setPosition(accp(66, (size.height*SCREEN_ZOOM_RATE) - 94));
    pLevelUpIcon->setTag(248);
    
    pLevelUpMenu = CCMenu::create(pLevelUpIcon, NULL);
    pLevelUpMenu->setPosition( CCPointZero );
    pLevelUpMenu->setTag(249);
    this->addChild(pLevelUpMenu, 1500);
}

void MainScene::RemoveLevelUpIcon()
{
    if(NULL != pLevelUpIcon)
    {
        this->removeChild(pLevelUpIcon, true);
        pLevelUpIcon = NULL;
    }
    
    if(NULL != pLevelUpMenu)
    {
        this->removeChild(pLevelUpMenu, true);
        pLevelUpMenu = NULL;
    }
}

void MainScene::LevelUpCallback(CCObject* pSender)
{
    
    if (TraceHistoryLayer::getInstance() != 0){
        TraceHistoryLayer::getInstance()->closeLayer();
    }
    if (TraceDetailLayer::getInstance() != 0){
        MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(), true);
    }
    
    CCMenu *menu = (CCMenu*)this->getChildByTag(249);
    CCMenuItemImage *item = (CCMenuItemImage*)menu->getChildByTag(248);
    item->unselected();

    this->initLevelUpLayer();
}
 */




void MainScene::initUserStatLayer()
{
    // 이전의 유저 스탯 레이어가 남아 있어 추적 진입 시 나타나던 것을 없애기 위해 유저 스탯 레이어 생성 이전에 무조건 존재하던 것을 없애고 만드는 방식으로 수정
    if (userStatLayer != NULL) {
        userStatLayer->removeFromParentAndCleanup(true);
        delete userStatLayer;
    }

    userStatLayer = new UserStatLayer();
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    userStatLayer->setContentSize(size);
    
    userStatLayer->setAnchorPoint(ccp(0,0));
    userStatLayer->setPosition(ccp(0,0));//this->getContentSize().height));
    userStatLayer->setVisible(true);
    this->addChild(userStatLayer,1000);
    
    refreshLevelUpIcon();
}

void MainScene::removeUserStatLayer()
{
    this->removeChild(userStatLayer, true);
    UserStatLayer::getInstance()->setVisible(false);
    UserStatLayer::getInstance()->RemoveLevelUpIcon();
}


/*
void MainScene::registerLabel( CCPoint anchor, CCPoint pos, CCLabelTTF* pLabel, int z){
    pLabel->setAnchorPoint(anchor);
    pLabel->setPosition(pos);
    this->addChild(pLabel, z);
}
*/

void MainScene::bottomUICallback(CCObject* pSender){
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    //CCLog("bottomUICallBack, pSender tag=%d"+tag);
    
    if(tutorialProgress < TUTORIAL_TOTAL-1)
    {
        if(TUTORIAL_CARD_DESCRIPTION_6 == tutorialProgress && tag != 1)
        {
            return;
        }
        else if(TUTORIAL_QUEST_1 == tutorialProgress && tag != 3)
        {
            return;
        }
        else
        {
            if(TUTORIAL_QUEST_1 != tutorialProgress)
            {
                removeClickIcon(this);
                addClickIcon(this, 160.0f, 90.0f);
                // 임시로 막아놓음 2.15
            }
            
            if(TUTORIAL_QUEST_1 == tutorialProgress && tag == 3)
            {
                removeClickIcon(this);
                
                MainScene::getInstance()->removeChildByTag(98765, true);

                const bool TutorialMode = true;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                int temp = TUTORIAL_QUEST_2;
                basePopUp->InitUI(&temp);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);

            }
        }
    }
    
    CCMenu *menu = (CCMenu*)node->getParent(); ;
    for (int i=0;i<5;i++)
    {
        CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(i);
        item->unselected();
    }
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    item->selected();

    if (curLayerTag == tag)return;
    
    soundMainBG();
    
    SetNormalSubBtns();
    SetSelectedSubBtn(tag);
    
    CocosDenshion::SimpleAudioEngine::sharedEngine()->stopAllEffects();
    
    soundButton1();
    
    if (tag != 0){
        cancelRivalListRefresh();
    }
    else{
        setRivalListRefresh();
    }
    
    switch(tag){
        case 0: // dojo
            releaseSubLayers();
            initDojoLayer();
            break;
        case 1: // card
            releaseSubLayers();
            addPageLoading();
            this->schedule(schedule_selector(MainScene::MoveToCardLayer));
            break;
        case 2:
        {
            ARequestSender::getInstance()->requestOpponent();
            
            const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_BATTLE);
            
            if(false == TutorialCompleted)
            {
                const bool TutorialMode = false;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                tutorialProgress = TUTORIAL_BATTLE_1;
                basePopUp->InitUI(&tutorialProgress);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(ccp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, TUTORIAL_BATTLE_LAYER_Z);
            }
        }
            break;
        case 3:
            ARequestSender::getInstance()->requestChapterList();
            break;
        case 4:
            releaseSubLayers();
            initShopLayer();
            break;
    }
    
    curLayerTag = tag;
    //
}

void MainScene::MoveToCardLayer()
{
    initCardLayer();
    this->unschedule(schedule_selector(MainScene::MoveToCardLayer));
    removePageLoading();
}


void MainScene::unregisterKakao(){
    PlayerInfo::getInstance()->xb->UnregisterKakao();
}

void MainScene::logoutKakao(){
    PlayerInfo::getInstance()->xb->LogoutKakao();
}
void MainScene::removeDojoSubMenu()
{
    // -- ??¬∞????? ?¬©??
    this->removeChildByTag(599, true);
    this->removeChildByTag(21, true);
    this->removeChildByTag(22, true);
    this->removeChildByTag(23, true);
    this->removeChildByTag(24, true);
    this->removeChildByTag(25, true);
    this->removeChildByTag(598, true);
}

void MainScene::removeCardSubMenu()
{
    // -- ??¬•?? ??? ?¬©??
    this->removeChildByTag(699, true);
    this->removeChildByTag(61, true);
    this->removeChildByTag(62, true);
    this->removeChildByTag(63, true);
    this->removeChildByTag(64, true);
    this->removeChildByTag(65, true);
    this->removeChildByTag(698, true);

}

void MainScene::releaseSubLayers(){
    
    removeDojoSubMenu();
    removeCardSubMenu();
    
    if (m_dojoLayer != NULL){
        this->removeChild(m_dojoLayer, true);
        m_dojoLayer = NULL;
    }
    
    if (m_cardLayer != NULL){
        this->removeChild(m_cardLayer, true);
        m_cardLayer = NULL;
    }
    
    if (m_battleLayer != NULL){
        this->removeChild(m_battleLayer, true);
        m_battleLayer->release();
        m_battleLayer = NULL;
    }
    else
        CCLOG("m_battleLayer = NULL;");
    
    if (m_questLayer != NULL){
        this->removeChild(m_questLayer, true);
        m_questLayer = NULL;
    }
    
    if (m_levelupLayer != NULL){
        this->removeChild(m_levelupLayer, true);
        m_levelupLayer = NULL;
    }
    
    if (m_shopLayer != NULL){
        this->removeChild(m_shopLayer, true);
        m_shopLayer = NULL;
    }
    removePopup();
}

void MainScene::initDojoLayer(){
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    if (m_dojoLayer == NULL){
        //m_dojoLayer = DojoLayerDojo::create();
        m_dojoLayer = new DojoLayerDojo(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_dojoLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_dojoLayer,HOME_DOJO_LAYER_Z);
        m_dojoLayer->setTouchEnabled(true);
    }
    else{
        m_dojoLayer->_setZOrder(HOME_DOJO_LAYER_Z);
    }
}

void MainScene::initCardLayer(){
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();

    if (m_cardLayer == NULL){
        
        m_cardLayer = new DojoLayerCard
(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE)); // 960 ???????¬¨????¬¨??480?????? /2 ?¬¨???        //m_cardLayer->setPosition(ccp(0,0));
        m_cardLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));//73));
        this->addChild(m_cardLayer,HOME_CARD_LAYER_Z);
    }
    else{
        m_cardLayer->_setZOrder(HOME_CARD_LAYER_Z);
    }
    m_cardLayer->setTouchEnabled(true);
}

void MainScene::initTemaEditLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    
    if (m_cardLayer == NULL)
    {
        m_cardLayer = new DojoLayerCard(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE), CARD_TAB_SUB_1);
        m_cardLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));//73));
        this->addChild(m_cardLayer,20);
    }
    else
    {
        m_cardLayer->_setZOrder(20);
    }
    m_cardLayer->setTouchEnabled(true);
}


void MainScene::initBattleLayer(){
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (m_battleLayer == NULL){
        //m_battleLayer = DojoLayerBattle::create();
        
        CCLOG("initBattleLayer");
        
        m_battleLayer = new DojoLayerBattle(
        CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_battleLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_battleLayer,HOME_BATTLE_LAYER_Z);
        m_battleLayer->setTouchEnabled(true);
    }
    else{
        m_battleLayer->_setZOrder(HOME_BATTLE_LAYER_Z);
    }
}

void MainScene::initQuestLayer()
{
    //initLevelUpLayer();
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (m_questLayer == NULL){
        m_questLayer = new DojoLayerQuest(
                                            CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_questLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_questLayer,HOME_QUEST_LAYER_Z);
        m_questLayer->setTouchEnabled(true);
    }
    else{
        m_questLayer->_setZOrder(HOME_QUEST_LAYER_Z);
    }
}


void MainScene::initShopLayer()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (m_shopLayer == NULL){
        m_shopLayer = new DojoLayerShop(
                                          CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_shopLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_shopLayer,HOME_SHOP_LAYER_Z);
        m_shopLayer->setTouchEnabled(true);
    }
    else{
        m_shopLayer->_setZOrder(HOME_SHOP_LAYER_Z);
    }
    
}

void MainScene::initLevelUpLayer()
{
    releaseLevelUpLayer();
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    if (m_levelupLayer == NULL)
    {
        m_levelupLayer = new LevelUpLayer(CCSize(size.width,this->getContentSize().height - MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE));
        m_levelupLayer->setPosition(accp(0,MAIN_LAYER_BTN_HEIGHT));
        this->addChild(m_levelupLayer,LEVEL_UP_LAYER_Z);//20);
        m_levelupLayer->setTouchEnabled(true);
    }
    else{
        m_shopLayer->_setZOrder(20);
    }
}

void MainScene::releaseLevelUpLayer()
{
    if (m_levelupLayer != NULL)
    {
        this->removeChild(m_levelupLayer, true);
        m_levelupLayer = NULL;
    }
}

extern "C" {
    
    
    void setKakaoAceessToken(const char* at){
        PlayerInfo *pi =PlayerInfo::getInstance();
        
        if (at != NULL){
            pi->accessToken = at;
        }
        else{
            pi->accessToken = "";
        }
    }
    
    //// 1
    void setKakaoUserToGame(const char* userid, const char* nickName, const char* profileUrl, const char *displayName){
        
        CCLog(" setKakaoUserToGame ===============, a:%s",nickName);
        
        PlayerInfo *pi =PlayerInfo::getInstance();
        
        if (strlen(nickName)==0){
            nickName = "noname";
            displayName = "noname";
        }
        
        pi->userID = atoll(userid);
        pi->SetName(nickName);
        pi->SetUserProfileUrl(profileUrl);
        pi->displayName = displayName;
        
//        UserStatLayer::getInstance()->updateUserProfileImage();
//        MainScene::getInstance()->requestKakaoFriendsInfo();
    }
    
    //// 2
    void setKakaoFriendsToGame(int numOfFriends, int numOfAppFriends){
        PlayerInfo::getInstance()->setKakaoFriendsInfo(numOfFriends, numOfAppFriends);
        ARequestSender::getInstance()->requestLogin2(); //ARequestSender::getInstance()->requestLoginToGameServer();
        
        /*
        // -- ?¬•??§¬???¬ß???¬∞??
        PlayerInfo::getInstance()->eventList = new CCArray();

        EventInfo* event1 = new EventInfo();
        event1->eventBanner = "ui_home_event_test01.png";
        event1->eventDetail = "ui_home_event_test01.png";
        event1->eventDescription = "test";
        
        EventInfo* event2 = new EventInfo();
        event2->eventBanner = "ui_home_event_test02.png";
        event2->eventDetail = "ui_home_event_test02.png";
        event2->eventDescription = "test";
        
        EventInfo* event3 = new EventInfo();
        event3->eventBanner = "ui_home_event_test03.png";
        event3->eventDetail = "ui_home_event_test03.png";
        event3->eventDescription = "test";
        
        PlayerInfo::getInstance()->eventList->addObject(event1);
        PlayerInfo::getInstance()->eventList->addObject(event2);
        PlayerInfo::getInstance()->eventList->addObject(event3);
        
        MainScene::getInstance()->eventDownload();
        */
        
        CCLog("setKakaoFriendsToGame 10");
        
        ResponseEvent* eventInfo= ARequestSender::getInstance()->requestEvent();
        
        //CCLog("setKakaoFriendsToGame 20");
        
        if(eventInfo)
        {
            if (atoi((const char*)eventInfo->res) == 0){
                PlayerInfo::getInstance()->eventList = new CCArray();
                
                for(int i=0;i<eventInfo->eventList->count();i++)
                {
                    EventInfo* event = (EventInfo*)eventInfo->eventList->objectAtIndex(i);
                                      
                    PlayerInfo::getInstance()->eventList->addObject(event);
                }
            }
            else{
                //popupNetworkError(eventInfo->res, eventInfo->msg,"requestGiftList");
                return;
            }

        }
        
        //CCLog("setKakaoFriendsToGame 30");
        
        CC_SAFE_DELETE(eventInfo);
        
        //CCLog("setKakaoFriendsToGame 40");
        
        MainScene::getInstance()->eventDownload();
        
        //CCLog("setKakaoFriendsToGame 50");
    }
    
    void addKakaoFriendToGame(const char* userId, const char* nickName, const char* profileURL, const char* friend_nickName, bool msg_block){
        PlayerInfo::getInstance()->addKakaoFriend(atoll(userId), nickName, profileURL, friend_nickName, msg_block);
    }
    void addKakaoAppFriendToGame(const char* userId, const char* nickName, const char* profileURL, const char* friend_nickName, bool msg_block){
        PlayerInfo::getInstance()->addKakaoAppFriend(atoll(userId), nickName, profileURL, friend_nickName, msg_block);
    }
    
    void BacktoTitle(){
        MainScene::getInstance()->switchTitleScene();
    }
    
    void MoveToLoginScene(){
        TitleScene::getInstance()->switchLoginScene();
    }
}


#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C"{
#endif
    /*
     void MoveToLoginScene()
     TitleScene::getInstance()->switchLoginScene();
     }
     */
    // 1
    
    void Java_com_capcom_FOC_AKakao_nativeMoveToLoginScene(JNIEnv* env, jobject thisObj){  //void MoveToLoginScene()
        TitleScene::getInstance()->switchLoginScene();
    }

    /////////////////////////// LoginActivity class
    
     JNIEXPORT void JNICALL Java_com_capcom_FOC_LoginActivity_nativeSetKakaoUserToGame(JNIEnv* env, jobject thisObj, jstring _userid, jstring _nickname, jstring _profileUrl, jstring _displayName){
        //CCLog(" setKakaoUserToGame ===============");
        
    
        const char* userid = env->GetStringUTFChars(_userid,NULL);
        const char* nickName = env->GetStringUTFChars(_nickname,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileUrl,NULL);
        const char* displayName = env->GetStringUTFChars(_displayName,NULL);
         
         CCLog(" setKakaoUserToGame ===============, a:%s",nickName);
        
        PlayerInfo *pi =PlayerInfo::getInstance();
        
        if (strlen(nickName)==0){
            nickName = "noname";
            displayName = "noname";
        }
        
        pi->userID = atoll(userid);
        pi->SetName(nickName);
        pi->SetUserProfileUrl(profileUrl);
        pi->displayName = displayName;
         
    }
    
    void Java_com_capcom_FOC_LoginActivity_nativeAddKakaoFriendToGame(JNIEnv* env, jobject thisObj, jstring _userId, jstring _nickName, jstring  _profileURL, jstring _friend_nickName, jboolean _msg_block){
        
        const char* userid = env->GetStringUTFChars(_userId,NULL);
        const char* nickName = env->GetStringUTFChars(_nickName,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileURL,NULL);
        const char* friend_nickName = env->GetStringUTFChars(_friend_nickName,NULL);
        bool msg_block = false;
        if (_msg_block == JNI_TRUE)msg_block= true;
        
        PlayerInfo::getInstance()->addKakaoFriend(atoll(userid), nickName, profileUrl, friend_nickName, msg_block);
    }
    void Java_com_capcom_FOC_LoginActivity_nativeAddKakaoAppFriendToGame(JNIEnv* env, jobject thisObj, jstring _userId, jstring _nickName, jstring  _profileURL, jstring _friend_nickName, jboolean _msg_block){
        
        const char* userid = env->GetStringUTFChars(_userId,NULL);
        const char* nickName = env->GetStringUTFChars(_nickName,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileURL,NULL);
        const char* friend_nickName = env->GetStringUTFChars(_friend_nickName,NULL);
        bool msg_block = false;
        if (_msg_block == JNI_TRUE)msg_block= true;
        
        PlayerInfo::getInstance()->addKakaoAppFriend(atoll(userid), nickName, profileUrl, friend_nickName, msg_block);
    }
    
    void Java_com_capcom_FOC_LoginActivity_nativeSetKakaoFriendsToGame(JNIEnv* env, jobject thisObj,jint numOfFriends, jint numOfAppFriends){
        
        int a = (int)numOfFriends;
        int b = (int)numOfAppFriends;
        
        PlayerInfo::getInstance()->setKakaoFriendsInfo(numOfFriends, numOfAppFriends);
        //ARequestSender::getInstance()->requestLoginToGameServer();
        ARequestSender::getInstance()->requestLogin2();
        
        ResponseEvent* eventInfo= ARequestSender::getInstance()->requestEvent();
        
        if(eventInfo)
        {
            if (atoi((const char*)eventInfo->res) == 0){
                PlayerInfo::getInstance()->eventList = new CCArray();
                
                for(int i=0;i<eventInfo->eventList->count();i++)
                {
                    EventInfo* event = (EventInfo*)eventInfo->eventList->objectAtIndex(i);
                                      
                    PlayerInfo::getInstance()->eventList->addObject(event);
                }
            }
            else{
                //popupNetworkError(eventInfo->res, eventInfo->msg,"requestGiftList");
                return;
            }

        }
        
        MainScene::getInstance()->eventDownload();
    }
    
    
    void Java_com_capcom_FOC_LoginActivity_nativeKakaoLoginSuccess(JNIEnv* env, jobject thisObj){
        CCLog(" LoginActivity_nativeKakaoLoginSuccess ");
        //CCDirector::sharedDirector()->replaceScene(TitleScene::scene());
        
        //MainScene::getInstance()->switchTitleScene();
        PlayerInfo::getInstance()->xb->kakao();
    }
    
    
    /////////////////////////////////////// AKakao class
    
    /////////////////////////// FOC class
    
    void Java_com_capcom_FOC_AKakao_nativeSetKakaoAceessToken(JNIEnv* env, jobject thisObj, jstring _accessToken){
        
        const char* at = env->GetStringUTFChars(_accessToken,NULL);
        
        PlayerInfo *pi =PlayerInfo::getInstance();
        
        if (at != NULL){
            pi->accessToken = at;
        }
        else{
            pi->accessToken = "";
        }
    }
    
    JNIEXPORT void JNICALL Java_com_capcom_FOC_AKakao_nativeSetKakaoUserToGame(JNIEnv* env, jobject thisObj, jstring _userid, jstring _nickname, jstring _profileUrl, jstring _displayName){
        //CCLog(" setKakaoUserToGame ===============");
        
        
        const char* userid = env->GetStringUTFChars(_userid,NULL);
        const char* nickName = env->GetStringUTFChars(_nickname,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileUrl,NULL);
        const char* displayName = env->GetStringUTFChars(_displayName,NULL);
        
        CCLog(" setKakaoUserToGame, user nickName:%s",nickName);
        
        PlayerInfo *pi =PlayerInfo::getInstance();
        
        if (strlen(nickName)==0){
            nickName = "noname";
            displayName = "noname";
        }
        
        pi->userID = atoll(userid);
        pi->SetName(nickName);
        pi->SetUserProfileUrl(profileUrl);
        pi->displayName = displayName;
        
    }
    
    void Java_com_capcom_FOC_AKakao_nativeAddKakaoFriendToGame(JNIEnv* env, jobject thisObj, jstring _userId, jstring _nickName, jstring  _profileURL, jstring _friend_nickName, jboolean _msg_block){
        
        const char* userid = env->GetStringUTFChars(_userId,NULL);
        const char* nickName = env->GetStringUTFChars(_nickName,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileURL,NULL);
        const char* friend_nickName = env->GetStringUTFChars(_friend_nickName,NULL);
        bool msg_block = false;
        if (_msg_block == JNI_TRUE)msg_block= true;
        
        PlayerInfo::getInstance()->addKakaoFriend(atoll(userid), nickName, profileUrl, friend_nickName, msg_block);
    }
    void Java_com_capcom_FOC_AKakao_nativeAddKakaoAppFriendToGame(JNIEnv* env, jobject thisObj, jstring _userId, jstring _nickName, jstring  _profileURL, jstring _friend_nickName, jboolean _msg_block){
        
        const char* userid = env->GetStringUTFChars(_userId,NULL);
        const char* nickName = env->GetStringUTFChars(_nickName,NULL);
        const char* profileUrl = env->GetStringUTFChars(_profileURL,NULL);
        const char* friend_nickName = env->GetStringUTFChars(_friend_nickName,NULL);
        bool msg_block = false;
        if (_msg_block == JNI_TRUE)msg_block= true;
        
        PlayerInfo::getInstance()->addKakaoAppFriend(atoll(userid), nickName, profileUrl, friend_nickName, msg_block);
    }
    
    void Java_com_capcom_FOC_AKakao_nativeSetKakaoFriendsToGame(JNIEnv* env, jobject thisObj,jint numOfFriends, jint numOfAppFriends){
        
        CCLog("AKakao_nativeSetKakaoFriendsToGame");
        
        int a = (int)numOfFriends;
        int b = (int)numOfAppFriends;
        
        
        PlayerInfo::getInstance()->setKakaoFriendsInfo(numOfFriends, numOfAppFriends);
        
        //ARequestSender::getInstance()->requestLoginToGameServer();
        ARequestSender::getInstance()->requestLogin2();
        
        ResponseEvent* eventInfo= ARequestSender::getInstance()->requestEvent();
        
        if(eventInfo)
        {
            if (atoi((const char*)eventInfo->res) == 0){
                PlayerInfo::getInstance()->eventList = new CCArray();
                
                for(int i=0;i<eventInfo->eventList->count();i++)
                {
                    EventInfo* event = (EventInfo*)eventInfo->eventList->objectAtIndex(i);
                                      
                    PlayerInfo::getInstance()->eventList->addObject(event);
                }
            }
            else{
                //popupNetworkError(eventInfo->res, eventInfo->msg,"requestGiftList");
                return;
            }

        }
        
        MainScene::getInstance()->eventDownload();
        
    }
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoUnregisterSuccess(JNIEnv* env, jobject thisObj){
        CCLog(" nativeKakaoUnregisterSuccess ");
        //MainScene::getInstance()->popupOk("언레지스터 되었습니다."); // 1
        MainScene::getInstance()->reservePopup(1);
        MainScene::getInstance()->switchTitleScene();
    };
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoUnregisterFail(JNIEnv* env, jobject thisObj){
        CCLog(" nativeKakaoUnregisterFail ");
        //MainScene::getInstance()->popupOk("언레지스터 실패");
        MainScene::getInstance()->reservePopup(2);
        
    };
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoLogoutSuccess(JNIEnv* env, jobject thisObj){
        CCLog(" nativeKakaoLogoutSuccess ");
        //MainScene::getInstance()->popupOk("로그아웃 되었습니다.");
        MainScene::getInstance()->reservePopup(3);
        MainScene::getInstance()->switchTitleScene();
        
        
    };
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoLogoutFail(JNIEnv* env, jobject thisObj){
        CCLog(" nativeKakaoLogoutFail");
        //MainScene::getInstance()->popupOk("로그아웃 실패");
        MainScene::getInstance()->reservePopup(4);
        
    };
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoSendMessageSuccess(JNIEnv* env, jobject thisObj, jstring _userId){
        
        const char* userid = env->GetStringUTFChars(_userId,NULL);
        
        PlayerInfo::getInstance()->recordMedalSentTime(atoll(userid));
        
        SocialInviteLayer::getInstance()->pSocialListlayer->setDisableInviteBtn(atoll(userid));

        //MainScene::getInstance()->popupOk("초대 메시지를 전송하였습니다.");
        MainScene::getInstance()->reservePopup(5);
        
    };
    
    void Java_com_capcom_FOC_AKakao_nativeKakaoSendMessageFail(JNIEnv* env, jobject thisObj){
        //CCLog(" nativeKakaoSendMessageFail ");
        //MainScene::getInstance()->popupOk("초대 메시지 전송 실패");
        MainScene::getInstance()->reservePopup(6);
    };
    
    void Java_com_capcom_FOC_HttpSender_nativeHttpSenderCallBack(JNIEnv* env, jobject thisObj, jint reqType, jstring response){
        //CCLog(" nativeKakaoSendMessageFail ");
        
        const char* cstring = env->GetStringUTFChars(response,NULL);
        
        CCLog("nativeHttpSenderCallBack, reqType:%d response:%s", reqType, cstring);
        //sleep(1000);
        //MainScene::getInstance()->httpCallBack(reqType, cstring);
        
        MainScene::getInstance()->callBack_reqtype = reqType;
        MainScene::getInstance()->callBack_data = cstring;
        
        MainScene::getInstance()->schedule(schedule_selector(MainScene::httpCallBack),0.3);
    };
    
    
#ifdef __cplusplus
}
#endif



void MainScene::httpCallBack(){ //int reqType, const char *data){
    
    int reqType = callBack_reqtype;
    const char *data = callBack_data;
    
    MainScene::getInstance()->removePageLoading();
    
    switch (reqType)
    {
        case REQ_BATTLE_LIST:
        {
            if(MAIN_LAYER_BATTLE == MainScene::getInstance()->curLayerTag)
            {
                MainScene::getInstance()->AsyncProcess(REQ_BATTLE_LIST, data);
            }
        }break;
        case REQ_FRIEND_LIST:
        {
            MainScene::getInstance()->AsyncProcess(REQ_FRIEND_LIST, data);
        }break;
        case REQ_CHAPTER_LIST:
        {
            if(MAIN_LAYER_QUEST == MainScene::getInstance()->curLayerTag)
            {
                MainScene::getInstance()->AsyncProcess(REQ_CHAPTER_LIST, data);
            }
        }break;
        case REQ_STAGE_LIST:
        {
            if(MAIN_LAYER_QUEST == MainScene::getInstance()->curLayerTag)
            {
                MainScene::getInstance()->AsyncProcess(REQ_STAGE_LIST, data);
            }
        }break;
        case REQ_ITEM_LIST:
        {
            MainScene::getInstance()->AsyncProcess(REQ_ITEM_LIST, data);
        }break;
            
        case REQ_GIFT_LIST:
        {
            MainScene::getInstance()->AsyncProcess(REQ_GIFT_LIST, data);
        }break;
            
        case REQ_FUSION:
        {
            MainScene::getInstance()->AsyncProcess(REQ_FUSION, data);
        }break;
            
        case REQ_COLLECT:
        {
            MainScene::getInstance()->AsyncProcess(REQ_COLLECT, data);
            
        }break;
            
        case REQ_MEDAL_COUNT:
        {
            MainScene::getInstance()->AsyncProcess(REQ_MEDAL_COUNT, data);
        }break;
            
        case REQ_BATTLE_LOG:
        {
            MainScene::getInstance()->AsyncProcess(REQ_BATTLE_LOG, data);
        }break;
            
        case REQ_TRAINING:
        {
            MainScene::getInstance()->AsyncProcess(REQ_TRAINING, data);
        }break;
            
        case REQ_BG_LIST:
        {
            MainScene::getInstance()->AsyncProcess(REQ_BG_LIST, data);
        }break;
            
        case REQ_ROULETTE:
        {
            MainScene::getInstance()->AsyncProcess(REQ_ROULETTE, data);
        }break;
    }
    
    MainScene::getInstance()->unschedule(schedule_selector(MainScene::httpCallBack));
}


#endif

void MainScene::reservePopup(int popupIdx)
{
    // 일단 막아둠.
    
    reservePopupIdx = popupIdx;
    MainScene::getInstance()->schedule(schedule_selector(MainScene::addMsgPopup),0.3);
}

void MainScene::addMsgPopup()
{
    //MainScene::getInstance()->popupOk("언레지스터 되었습니다."); // 1
    //MainScene::getInstance()->popupOk("언레지스터 실패"); // 2
    //MainScene::getInstance()->popupOk("로그아웃 되었습니다."); // 3
    //MainScene::getInstance()->popupOk("로그아웃 실패"); // 4
    //MainScene::getInstance()->popupOk("초대 메시지를 전송하였습니다."); // 5
    //MainScene::getInstance()->popupOk("초대 메시지를 전송 실패."); // 6
    
    MainScene::getInstance()->unschedule(schedule_selector(MainScene::addMsgPopup));
    
    switch(reservePopupIdx){
        case 1:
            MainScene::getInstance()->popupOk("탈퇴 되었습니다."); // 1
            break;
        case 2:
            MainScene::getInstance()->popupOk("네트워크 에러\n탈퇴하지 못했습니다."); // 2
            break;
        case 3:
            MainScene::getInstance()->popupOk("로그아웃 되었습니다."); // 3
            break;
        case 4:
            MainScene::getInstance()->popupOk("로그아웃 실패하였습니다."); // 4
            break;
        case 5:
            MainScene::getInstance()->popupOk("초대 메시지를 전송하였습니다."); // 5
            break;
        case 6:
            MainScene::getInstance()->popupOk("초대 메시지를 전송하지 못했습니다."); // 6
            break;
    }
    
}

void MainScene::requestKakaoFriendsInfo(){
    PlayerInfo::getInstance()->xb->RequestKakaoFriendsInfo();
}

void MainScene::switchTitleScene()
{
    this->schedule(schedule_selector(MainScene::gotoTitleScene),1.0);
}

void MainScene::gotoTitleScene()
{
    CCDirector::sharedDirector()->replaceScene(TitleScene::scene());
}

void MainScene::HideMainMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition( ccp(0,-10000));
    }
    
    CCSprite* pSprBottomUIBg = (CCSprite*)this->getChildByTag(98);
    if (pSprBottomUIBg != NULL){
        pSprBottomUIBg->setPosition(ccp(0,-1000));
    }
    
    for(int i=0; i<5; ++i)
    {
        CCLabelTTF* dojoLabel = (CCLabelTTF*)this->getChildByTag(11+i);
        if (dojoLabel != NULL){
            dojoLabel->setPositionY(-1000.0f);
        }
    }
}

void MainScene::ShowMainMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition( CCPointZero);
    }
    
    CCSprite* pSprBottomUIBg = (CCSprite*)this->getChildByTag(98);
    if (pSprBottomUIBg != NULL){
        pSprBottomUIBg->setPosition(CCPointZero);
    }
    
    for(int i=0; i<5; ++i)
    {
        CCLabelTTF* dojoLabel = (CCLabelTTF*)this->getChildByTag(11+i);
        if (dojoLabel != NULL){
            dojoLabel->setPositionY(2.0f);
        }
    }
}

void MainScene::setEnableMainMenu(bool flag)
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setEnabled(flag);
    }
}


void MainScene::eventDownload()
{
    if(PlayerInfo::getInstance()->eventList)
    {
        std::vector<std::string> downloads;

        std::string basePath  = FOC_IMAGE_SERV_URL;
        basePath.append("images/event/");
        
        const int eventCount = PlayerInfo::getInstance()->eventList->count();
        
        for(int i=0; i<eventCount; ++i)
        {
            EventInfo* info = (EventInfo*)PlayerInfo::getInstance()->eventList->objectAtIndex(i);
            
            if(!FileManager::sharedFileManager()->IsFileExist(info->eventBanner.c_str()))
            {
                string downPath = basePath + info->eventBanner;
                downloads.push_back(downPath);
            }
            
            if(!FileManager::sharedFileManager()->IsFileExist(info->eventDetail.c_str()))
            {
                string downPath = basePath + info->eventDetail;
                downloads.push_back(downPath);
            }

        }
        
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        requestor->addDownloadTask(downloads, this, callfuncND_selector(MainScene::onHttpRequestCompleted));
    }
}


void MainScene::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("event image download complete");
    }
}


void MainScene::refreshUserStat()
{
    PlayerInfo::getInstance()->refreshUserStat();
}





void MainScene::addPopup(CCNode* child, int z)
{
    if (popupCnt > 0)return;
    
    popupCnt++;
    MainScene::getInstance()->addChild(child,z);
}

void MainScene::removePopup()
{
    MainScene::getInstance()->removeChildByTag(123, true);
    if (popupCnt>0)popupCnt--;
}

void MainScene::RetryPopup(const char *text1, char *url, int reqType){
    retry_url = url;
    retry_reqType = reqType;
    popupRetry(text1, this);
}

void MainScene::BtnRetry()
{
    switch (retry_reqType)
    {
        case REQ_BATTLE_LIST:
            {
                ARequestSender::getInstance()->requestOpponent();
            }
            break;
            
        case REQ_CHAPTER_LIST:
            {
                ARequestSender::getInstance()->requestChapterList();
            }
            break;
        case REQ_STAGE_LIST:
            {
                ARequestSender::getInstance()->requestStageList();
            }
            break;
            
        case REQ_FRIEND_LIST:
            {
                ARequestSender::getInstance()->requestFriendsToGameServer();

            }
            break;
            
        case REQ_ITEM_LIST:
            {
                ARequestSender::getInstance()->requestItemListAsync();
            }
            break;
            
        case REQ_GIFT_LIST:
            {
                ARequestSender::getInstance()->requestGiftListAsync();
            }
            break;
        case REQ_FUSION:
            {
                ARequestSender::getInstance()->requestFusion(FusionLayer::getInstance()->fusionCard1->getSrl(), FusionLayer::getInstance()->fusionCard2->getSrl());
                
            }
            break;
        case REQ_COLLECT:
            {
                ARequestSender::getInstance()->requestCollection();
            }
            break;
        case REQ_MEDAL_COUNT:
            {
                ARequestSender::getInstance()->requestMedalCount();
            }
            break;
            
        case REQ_BATTLE_LOG:
            {
                ARequestSender::getInstance()->requestNotice();
            }
            break;
            
        case REQ_TRAINING:
            {
                ARequestSender::getInstance()->requestTraining(TrainingLayer::getInstance()->fusionCard1->getSrl(), TrainingLayer::getInstance()->fusionCard2->getSrl());
            }
            break;
            
        case REQ_ROULETTE:
            {
                ARequestSender::getInstance()->requestMedalCount();
            }
            break;
            
        default:
            break;
    }
    
    
    //ARequestSender::getInstance()->retry(retry_url, retry_reqType);
    
    MainScene::getInstance()->unschedule(schedule_selector(MainScene::BtnRetry));
}

void MainScene::AsyncProcess(int reqType, const char *data)
{
    switch (reqType)
    {
        case REQ_BATTLE_LIST:
        {
            MainScene::getInstance()->releaseSubLayers();
            AResponseParser::getInstance()->responseOpponent(data);
            MainScene::getInstance()->initBattleLayer();            
        }
            break;
            
        case REQ_CHAPTER_LIST:
        {
            AResponseParser::getInstance()->readQuest();
            
            ResponseQuestListInfo* questServerlist = AResponseParser::getInstance()->responseQuestList(data);
            
            PlayerInfo::getInstance()->UpdateQuestLockState(questServerlist);
                      
            MainScene::getInstance()->releaseSubLayers();
            MainScene::getInstance()->initQuestLayer();
        }
            break;
            
        case REQ_STAGE_LIST:
        {            
            ChapterLayer::getInstance()->removeAllChildrenWithCleanup(true);
            ChapterLayer::getInstance()->setTouchEnabled(false);
            ChapterLayer::getInstance()->setPosition(ccp(0.0f, 0.0f));

            ChapterLayer::getInstance()->stageList = AResponseParser::getInstance()->responseQuestList(data);
            
            PlayerInfo::getInstance()->UpdateQuestLockState(ChapterLayer::getInstance()->stageList);
            
            QuestInfo* questInfo = (QuestInfo*)ChapterLayer::getInstance()->getUnlockChapterList()->objectAtIndex(ChapterLayer::getInstance()->StageIndex);
            
            ChapterLayer::getInstance()->curChapter = questInfo;
            
            ChapterLayer::getInstance()->InitStagelayer(questInfo);
            
            /// 스테이지의 처음에만 quest story가 나오도록 수정.
            CCLog(" chapter id :%d", questInfo->questID);
            int firstStageProgress = -1;
            for(int i=0;i<ChapterLayer::getInstance()->stageList->questList->count();i++){
                AQuestInfo *qst = (AQuestInfo*)ChapterLayer::getInstance()->stageList->questList->objectAtIndex(i);
                //CCLog("qst->questID:%d", qst->questID);
                if (qst->questID == questInfo->questID){
                    firstStageProgress = qst->progress;
                    break;
                }
            }
            if (ChapterLayer::getInstance()->stageList->questList->count() == 0)firstStageProgress=0;
            
            CCLog(" firstProgress = %d", firstStageProgress);
            
            if (firstStageProgress == -1 || firstStageProgress == 0)ChapterLayer::getInstance()->popupTrigger = true;
            else ChapterLayer::getInstance()->popupTrigger = false;
            
            if (ChapterLayer::getInstance()->popupTrigger) {
                ChapterLayer::getInstance()->loadQuestStory();
                ChapterLayer::getInstance()->popupTrigger = false;
            }
        }
            break;
            
        case REQ_FRIEND_LIST:
        {
            AResponseParser::getInstance()->responseFriends(data);
            
            // -- ?¬©?? ???????? ????
            MainScene* main = MainScene::getInstance();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(0);
            item1->selected();
            
            main->curLayerTag = MAIN_LAYER_DOJO;
            
            main->releaseSubLayers();
            main->initDojoLayer();
            
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(0);
            
            // -- ??¬∞???????????
            DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
            
            CCMenu* pMenu2 = (CCMenu*)main->getChildByTag(599);
            
            for (int i=11 ;i<16; ++i)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu2->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item2 = (CCMenuItemImage *)pMenu2->getChildByTag(12);
            item2->selected();
            
            dojo->curLayerTag = 12;
            dojo->SetNormalSubBtns();
            dojo->SetSelectedSubBtn(12);
            
            dojo->ReleaseLayer();
            dojo->InitSocialLayer();
        }
            break;
            
        case REQ_ITEM_LIST:
        {
            MainScene* main = MainScene::getInstance();
            main->releaseSubLayers();
            main->initDojoLayer();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(0);
            item1->selected();
            
            main->curLayerTag = MAIN_LAYER_DOJO;
            
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(0);
            
            ////////////////////////////////////////////////////////
            DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
            
            CCMenu* pMenu2 = (CCMenu*)main->getChildByTag(599);
            
            for (int i=11 ;i<16; ++i)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu2->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item2 = (CCMenuItemImage *)pMenu2->getChildByTag(13);
            item2->selected();
            
            dojo->curLayerTag = 13;
            dojo->SetNormalSubBtns();
            dojo->SetSelectedSubBtn(13);
            
            ResponseItemInfo* itemInfo = AResponseParser::getInstance()->responseItemList(data);
            
            PlayerInfo::getInstance()->myItemList = new CCArray();
            
            for(int i=0;i<itemInfo->itemList->count();i++)
            {
                ItemInfo* item = (ItemInfo*)itemInfo->itemList->objectAtIndex(i);
                CCLog(" item id:%d count:%d", item->itemID, item->count);
                
                PlayerInfo::getInstance()->myItemList->addObject(item);
            }
            
            itemInfo->autorelease();
            
//            CCLog("PlayerInfo::getInstance()->myItemList->count:%d", PlayerInfo::getInstance()->myItemList->count());
            
            DojoLayerDojo::getInstance()->ReleaseLayer();
            DojoLayerDojo::getInstance()->InitItemLayer(0);
            
        }
            break;
            
        case REQ_GIFT_LIST:
        {
            MainScene* main = MainScene::getInstance();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(0);
            item1->selected();
            
            main->curLayerTag = MAIN_LAYER_DOJO;
            
            main->releaseSubLayers();
            main->initDojoLayer();
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(0);
            
            ///////////////////
            
            DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
            
            CCMenu* pMenu2 = (CCMenu*)main->getChildByTag(599);
            
            for (int i=11 ;i<16; ++i)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu2->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item2 = (CCMenuItemImage *)pMenu2->getChildByTag(13);
            item2->selected();
            
            dojo->curLayerTag = 13;
            dojo->SetNormalSubBtns();
            dojo->SetSelectedSubBtn(13);
            
            ResponseGiftInfo* giftInfo = AResponseParser::getInstance()->responseGiftList(data);
            
            PlayerInfo::getInstance()->myGiftList = new CCArray();
            
            for(int i=0;i<giftInfo->giftList->count();i++)
            {
                GiftInfo* gift = (GiftInfo*)giftInfo->giftList->objectAtIndex(i);
                CCLog("gift id:%d count:%d", gift->giftID, gift->count);
                
                PlayerInfo::getInstance()->myGiftList->addObject(gift);
            }
            
            DojoLayerDojo::getInstance()->ReleaseLayer();
            DojoLayerDojo::getInstance()->InitItemLayer(1);

        }
            break;
            
        case REQ_FUSION:
        {
            ResponseFusionInfo* fusionInfo = AResponseParser::getInstance()->responseFusion(data);
            
            MainScene::getInstance()->setEnableMainMenu(false);
            UserStatLayer::getInstance()->setEnableMenu(false);
            DojoLayerCard::getInstance()->setEnableMainMenu(false);
            FusionLayer::getInstance()->setTouchEnabled(false);
            CCMenu* Menu = (CCMenu*)FusionLayer::getInstance()->getChildByTag(99);
            Menu->setEnabled(false);
            FusionLayer::getInstance()->setFusionData(fusionInfo);
            FusionLayer::getInstance()->DoCardFusionAction();

        }
            break;
            
        case REQ_COLLECT:
        {
            ResponseCollectionInfo* collectionInfo = AResponseParser::getInstance()->responseCollection(data);
            
            DojoLayerDojo::getInstance()->ReleaseLayer();
            DojoLayerDojo::getInstance()->InitDojoCollectLayer(collectionInfo);
        }
            break;
            
        case REQ_MEDAL_COUNT:
        {
            ResponseMedal* medalInfo = AResponseParser::getInstance()->responseMedal(data);

            MainScene::getInstance()->releaseSubLayers();
            MainScene::getInstance()->initShopLayer();
            
            MainScene* main = MainScene::getInstance();
            
            CCMenu* pMenu1 = (CCMenu*)main->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(4);
            item1->selected();
            
            main->curLayerTag = MAIN_LAYER_SHOP;
            
            main->SetNormalSubBtns();
            main->SetSelectedSubBtn(4);
            
            ShopLayer* shoplayer = ShopLayer::getInstance();
            shoplayer->InitBtn(3);
            shoplayer->ReleaseSubLayer();
            shoplayer->InitRouletteLayer(medalInfo->medalCount);
        }
            break;
            
        case REQ_BATTLE_LOG:
        {
            DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
            
            if(dojo->pBattleLogLayer)
            {
                dojo->battlelogActive = true;
                
                dojo->pBattleLogListLayer = new BattleLogListLayer(CCSize(this->getContentSize().width, this->getContentSize().height));
                dojo->pBattleLogListLayer->noticeInfo = AResponseParser::getInstance()->responseNotice(data);
                const float LayerHeight = BATTLE_LOG_CELL_HEIGHT * dojo->pBattleLogListLayer->GetNumOfLog();
                dojo->pBattleLogListLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
                dojo->pBattleLogListLayer->InitUI();
                dojo->pBattleLogListLayer->LayerStartPos = (598 - LayerHeight)/2;
                dojo->pBattleLogListLayer->setAnchorPoint(ccp(0, 0));
                dojo->pBattleLogListLayer->setPosition(accp(0, 598 - LayerHeight));
                dojo->pBattleLogListLayer->setTouchEnabled(true);
                dojo->pBattleLogLayer->addChild(dojo->pBattleLogListLayer, 60);
            }
        }
            break;
            
        case REQ_TRAINING:
        {
            ResponseTrainingInfo* trainingInfo = AResponseParser::getInstance()->responseTraining(data);
            
            MainScene::getInstance()->setEnableMainMenu(false);
            UserStatLayer::getInstance()->setEnableMenu(false);
            DojoLayerCard::getInstance()->setEnableMainMenu(false);
            TrainingLayer::getInstance()->setTouchEnabled(false);
            CCMenu* Menu = (CCMenu*)TrainingLayer::getInstance()->getChildByTag(99);
            Menu->setEnabled(false);
            TrainingLayer::getInstance()->setTrainingData(trainingInfo);
            TrainingLayer::getInstance()->DoCardTraining();
        }
            break;
            
        case REQ_BG_LIST:
        {
            AResponseParser::getInstance()->responseBgList(data);
            DojoLayerDojo::getInstance()->InitBgSelectLayer();
        }
            break;
            
        case REQ_ROULETTE:
        {
            ResponseRoulette* RouletteInfo = AResponseParser::getInstance()->responseRoulette(data);
            ShopRoulette::getInstance()->RunRoulette(RouletteInfo);
        }
            break;
            
        default:
            break;
    }
}

void MainScene::readNPCData()
{
    unsigned long length = 0;
    
    //std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("npc.xml");
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("npc_image.xml");
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    npcList = new CCArray();
    
    parseNPCXML(root_element, npcList);
    
}

void MainScene::parseNPCXML(xmlNode * node, CCArray *npcList)
{
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s", currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "npc") == 0){
            parseNPCXML(currentNode->children, npcList);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                //CCLog(" child name:%s", child->name);
                NpcInfo *npcInfo = new NpcInfo();
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    //CCLog(" childNode name:%s", childNode->name);
                    
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_code") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->npcCode = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_cardcode") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->cardCode = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_name") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->npcName = (const char*)aaa->content;
                            //CCLog("npc name:%s",npcInfo->npcName);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_image") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->npcImagePath = (const char*)aaa->content;
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "u_timeout") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->normalBattleLimitTime = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "u_touch") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->touchUp = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "u_dropqauge") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->gaugeDrop = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "u_description") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->npcDesc = (const char*)aaa->content;
                            //npcInfo->npcDesc = textAdjust((const char*)aaa->content);
                            //CCLog("npcInfo->npcDesc :%s",npcInfo->npcDesc);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_hp") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->hp = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_sendcoin") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            npcInfo->sendCoin = atoi((const char*)aaa->content);
                        }
                    }
                }
                npcList->addObject(npcInfo);
            }
        }
    }
}

//const char* MainScene::textAdjust(const char *input)
//{
//    std::string text = input;
//    do {
//        int pos = text.find("\\n");
//        if (pos >= text.length())
//            return text.c_str();
//        text = text.replace(pos, 2, "\n");
//    } while (1);
//    return text.c_str();
//}

NpcInfo* MainScene::getNpc(int code){
    for(int i=0;i<npcList->count();i++){
        NpcInfo *npcInfo = (NpcInfo*)npcList->objectAtIndex(i);
        if (npcInfo->npcCode == code)return npcInfo;
    }
    return NULL;
}

void MainScene::readQuestNpcData()
{
    unsigned long length = 0;
    
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("npc.xml");
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    questNpcList = new CCArray();
    
    parseQuestNpcXML(root_element, questNpcList);
    
    //CCLog("questNpcList.count:%d:", questNpcList->count());
}

void MainScene::parseQuestNpcXML(xmlNode * node, CCArray *_questNpcList)
{
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "npc") == 0){
            parseQuestNpcXML(currentNode->children, _questNpcList);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){

                QuestNpcInfo *questNpcInfo = new QuestNpcInfo();
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_code") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            questNpcInfo->npcCode = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_stage_min") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            questNpcInfo->n_stage_min = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "n_stage_max") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            questNpcInfo->n_stage_max = atoi((const char*)aaa->content);
                        }
                    }
                }
                //CCLog("id :%d min:%d max:%d", questNpcInfo->npcCode, questNpcInfo->n_stage_min, questNpcInfo->n_stage_max);
                
                _questNpcList->addObject(questNpcInfo);
            }
        }
    }
    
}

CCArray* MainScene::getQuestNpc(int questID)
{
    CCArray* aa = new CCArray();
    for(int i=0;i<questNpcList->count();i++){
        QuestNpcInfo *npcInfo = (QuestNpcInfo*)questNpcList->objectAtIndex(i);
        
        //CCLog("id :%d min:%d max:%d", npcInfo->npcCode, npcInfo->n_stage_min, npcInfo->n_stage_max);
        
        if (npcInfo->n_stage_min <= questID && npcInfo->n_stage_max >= questID){
            CCInteger* val = new CCInteger(npcInfo->npcCode);
            aa->addObject(val);
        }
    }
    /*
    CCLog("getQuestNpc, questID:%d",questID);
    for(int i=0;i<aa->count();i++){
        CCInteger* a =  (CCInteger*)aa->objectAtIndex(i);
        int val = a->getValue();
        CCLog("npc id:%d",val);
    }*/
    
    return aa;
}



/*
 // goMainScene과 기능이 중복되어 삭제. 
void MainScene::moveToShopLayer()
{
    releaseSubLayers();
    initShopLayer();
    
    CCMenu* pMenu1 = (CCMenu*)this->getChildByTag(99);
    for (int i=0;i<5;i++)
    {
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
        item1->unselected();
    }
    
    CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(4);
    item1->selected();
    
    this->curLayerTag = 4;
    this->SetNormalSubBtns();
    this->SetSelectedSubBtn(4);
}
 */

int MainScene::getCurLayer(){
    return curLayerTag;
}

void MainScene::goMainScene(int nSubLayer)
{
    this->releaseSubLayers();
    
    switch(nSubLayer)
    {
        case MAIN_LAYER_CARD:
            this->initCardLayer();
            break;
        case MAIN_LAYER_BATTLE:
            this->initBattleLayer();
            break;
        case MAIN_LAYER_SHOP:
            this->initShopLayer();
            break;

            
            
            
            
        case MAIN_LAYER_DOJO:
            this->initDojoLayer();
            ARequestSender::getInstance()->requestItemListAsync();
            break;
    }
    
    ////////////////////
    
    CCMenu* pMenu1 = (CCMenu*)this->getChildByTag(99);
    for (int i=0;i<5;i++)
    {
        CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(i);
        item1->unselected();
    }
    
    CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu1->getChildByTag(nSubLayer);
    item1->selected();
    
    ////////////////////
    
    this->curLayerTag = nSubLayer;
    
    this->SetNormalSubBtns();
    this->SetSelectedSubBtn(nSubLayer);
}

void MainScene::refreshRivalList()
{
    DojoLayerDojo::getInstance()->refreshRivalEvent();
}

void MainScene::setRivalListRefresh(){
    //CCLog(" SET RivalListRefresh");
    // 레이어 변경이 있을때마다 set, cancel하여 Home에서만 라이벌 리스트를 갱신요청 하게 한다.
    // Home으로 복귀할때마다 set하고 다른 레이어로 이동시 cancel 시킨다.
    this->schedule(schedule_selector(MainScene::refreshRivalList),300.0, -1, 1);
}

void MainScene::cancelRivalListRefresh(){
    //CCLog(" CANCEL RivalListRefresh");
    this->unschedule(schedule_selector(MainScene::refreshRivalList));
}
