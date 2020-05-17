//
//  DojoLayerCard.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#include "DojoLayerCard.h"
//#include "extensions/CCListView/CCListView.h"
//using namespace cocos2d::extension;



#include "CardDictionary.h"
#include "PlayerInfo.h"
#include "ARequestSender.h"
#include "CCHttpRequest.h"
#include "MainScene.h"
#include "Tutorial.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

DojoLayerCard *DojoLayerCard::instance = NULL;

DojoLayerCard::DojoLayerCard(CCSize layerSize)
{

    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
    //    bRet = true;
    } while (0);
    
    instance = this;
    
    this->setContentSize(layerSize);
    
    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite);

    //CheckLayerSize(this);
    
    InitSubLayer(CARD_TAB_SUB_0);
    
    InitUI();
    
    //popupNetworkError("1234",NULL, NULL);
    //initThread();
    
    //return bRet;
}

DojoLayerCard::DojoLayerCard(CCSize layerSize, int sublayer)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    instance = this;
    
    this->setContentSize(layerSize);
    
    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite);
    
    InitSubLayer(sublayer);
    
    InitUI();

    // -- 카드 서브 레이어
    CCMenu* pMenu1 = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
    
    for (int i=11; i<15; ++i)
    {
        CCMenuItemImage *item = (CCMenuItemImage*)pMenu1->getChildByTag(i);
        item->unselected();
    }
    
    CCMenuItemImage *item = (CCMenuItemImage*)pMenu1->getChildByTag(12);
    item->selected();
    
    SetNormalSubBtns();
    SetSelectedSubBtn(12);
    
    curLayerTag = 12;
     
}

DojoLayerCard::~DojoLayerCard()
{
    this->removeAllChildrenWithCleanup(true);
}



void DojoLayerCard::InitUI(){
    // top ui
    /*
    pSprBtn = CCSprite::create("btn/social.png");
    
    //pSprBtn->setAnchorPoint(ccp(0.5f,1));
    pSprBtn->setPosition( ccp(0,0) );
    this->addChild(pSprBtn, 1000);
    */
    //////////////////////////////////////////////////
    // sub button

    CCSprite* pSprSubUIBg     = CCSprite::create("ui/home/ui_menu_sub_bg_b.png");
    pSprSubUIBg->setAnchorPoint(ccp(0,0));
    pSprSubUIBg->setPosition(accp(0.0f, 0.0f));
    pSprSubUIBg->setTag(698);
    MainScene::getInstance()->addChild(pSprSubUIBg, 100);

    CCMenuItemImage *pSpr1 = CCMenuItemImage::create("ui/card_tab/ui_card_sub_01_1.png","ui/card_tab/ui_card_sub_01_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    CCMenuItemImage *pSpr2 = CCMenuItemImage::create("ui/card_tab/ui_card_sub_02_1.png","ui/card_tab/ui_card_sub_02_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    CCMenuItemImage *pSpr3 = CCMenuItemImage::create("ui/card_tab/ui_card_sub_02_1.png","ui/card_tab/ui_card_sub_02_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    CCMenuItemImage *pSpr4 = CCMenuItemImage::create("ui/card_tab/ui_card_sub_03_1.png","ui/card_tab/ui_card_sub_03_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    
    /*
    CCMenuItemImage *pSpr3 = CCMenuItemImage::create("ui/home/ui_menu_sub_02_1.png","ui/home/ui_menu_sub_02_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    CCMenuItemImage *pSpr4 = CCMenuItemImage::create("ui/home/ui_menu_sub_02_1.png","ui/home/ui_menu_sub_02_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    CCMenuItemImage *pSpr5 = CCMenuItemImage::create("ui/home/ui_menu_sub_03_1.png","ui/home/ui_menu_sub_03_2.png",this,menu_selector(DojoLayerCard::SubUICallback));
    */
    pSpr1->setTag(11);
    pSpr2->setTag(12);
    pSpr3->setTag(13);
    pSpr4->setTag(14);
//    pSpr5->setTag(15);
    
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr2->setAnchorPoint(ccp(0,0));
    pSpr3->setAnchorPoint(ccp(0,0));
    pSpr4->setAnchorPoint(ccp(0,0)); //0.5,0));
//    pSpr5->setAnchorPoint(ccp(0.5,0));
    
    pSpr1->setPosition(accp(7,88));// 70,16));
    pSpr2->setPosition(accp(155,88));//194,16));
    pSpr3->setPosition(accp(313,88));//320,16));
    pSpr4->setPosition(accp(471,88));//447,16));
//    pSpr5->setPosition(accp(570,16));
    
    //CCMenu* pSubMenu = CCMenu::create(pSpr1, pSpr2, pSpr3, pSpr4, pSpr5, NULL);
    CCMenu* pSubMenu = CCMenu::create(pSpr1, pSpr2, pSpr3, pSpr4, NULL);
  
    pSubMenu->setPosition( accp(0,0));
    pSubMenu->setTag(699);
    MainScene::getInstance()->addChild(pSubMenu,120);

    pSpr1->selected();
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("내 카드"  , "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("팀"     , "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("합성"   , "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("단련" , "HelveticaNeue-Bold", 11);
    CCLabelTTF* pLabel5 = CCLabelTTF::create("Trade"    , "HelveticaNeue-Bold", 11);
    
    /*
    CCLabelTTF* pLabel1 = CCLabelTTF::create("My Card"  , "Arial-ItalicMT", 13);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("Team"     , "Arial-ItalicMT", 13);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("Fusion"   , "Arial-ItalicMT", 13);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("Training" , "Arial-ItalicMT", 13);
    CCLabelTTF* pLabel5 = CCLabelTTF::create("Trade"    , "Arial-ItalicMT", 13);
    */
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel3->setColor(subBtn_color_normal);
    pLabel4->setColor(subBtn_color_normal);
    pLabel5->setColor(subBtn_color_normal);
    pLabel1->setTag(61);
    pLabel2->setTag(62);
    pLabel3->setTag(63);
    pLabel4->setTag(64);
    pLabel5->setTag(65);
    
    pLabel1->setColor(subBtn_color_selected);
    
    // position the label on the center of the screen
    /*
    registerLabel( this,ccp(0.5,0), accp( 70,MENU_LABEL_Y), pLabel1,130);
    registerLabel( this,ccp(0.5,0), accp(194,MENU_LABEL_Y), pLabel2,130);
    registerLabel( this,ccp(0.5,0), accp(320,MENU_LABEL_Y), pLabel3,130);
    registerLabel( this,ccp(0.5,0), accp(447,MENU_LABEL_Y), pLabel4,130);
    registerLabel( this,ccp(0.5,0), accp(570,MENU_LABEL_Y), pLabel5,130);
    */
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp( 84, 102), pLabel1,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(241, 102), pLabel2,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(399, 102), pLabel3,130);
    registerLabel( MainScene::getInstance(),ccp(0.5,0), accp(553, 102), pLabel4,130);
    //registerLabel( this,ccp(0.5,0), accp(570,MENU_LABEL_Y), pLabel5,130);
    // layer size 확인용
    /*
    CCSprite* pSprTest     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(this, ccp(0,0), accp(0,0), pSprTest, 50);
    
    CCSprite* pSprTest2     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(this, ccp(0,1), ccp(0,410), pSprTest2, 100);
    */
    
}


void DojoLayerCard::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //if (CCRect::CCRectContainsPoint(pSprBtn->boundingBox(), location)){
    /*
    CCRect *rect = new CCRect(pSprBtn->boundingBox());
    if (rect->containsPoint(location)){
        CCLog("sprite buttn press");
        _tableview->_tableView->SortCell();
        
    }
     */
    
}


void DojoLayerCard::SubUICallback(CCObject* pSender){
    
    if (MainScene::getInstance() != NULL){
        if (MainScene::getInstance()->popupCnt>0)return;
    }
    
    
    soundButton1();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if(TUTORIAL_CARD_DESCRIPTION_6 == tutorialProgress && tag != 11)
        return;

    if(TUTORIAL_CARD_MANAGEMENT_3 == tutorialProgress && tag != 12)
        return;
    else
        removeClickIcon(MainScene::getInstance());

    if (tag >= 11 && tag <= 15)
    {
        CCMenu *menu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
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
    
    CCMenu *menu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
    CCMenuItemImage *item = (CCMenuItemImage *)menu->getChildByTag(tag);
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    item->selected();

    
    SetNormalSubBtns();
    SetSelectedSubBtn(tag);
    
    if (curLayerTag == tag)
        return;
    
    //curLayerTag = 1; // cards layer tag
    
    releaseSubLayer();
    
    

    switch(tag){
        case 11: // my card
            InitSubLayer(CARD_TAB_SUB_0);
            
            if (tutorialProgress == TUTORIAL_CARD_DESCRIPTION_6)
            {
                MainScene::getInstance()->removeChildByTag(98765, true);

                const bool TutorialMode = true;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                int CurrentState = TUTORIAL_CARD_MANAGEMENT_1;
                basePopUp->InitUI(&CurrentState);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
                
                CCMenu *menu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
                menu->setEnabled(false);
                
                ACardTableView::getInstance()->setTouchEnabled(false);
                MyCardLayer::getInstance()->setTouchEnabled(false);
            }

            //item->selected();
            break;
        case 12: // team
            InitSubLayer(CARD_TAB_SUB_1);
            
            if (tutorialProgress == TUTORIAL_CARD_MANAGEMENT_3)
            {
                MainScene::getInstance()->removeChildByTag(98765, true);

                const bool TutorialMode = true;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                int CurrentState = TUTORIAL_TEAM_SETTING_1;
                basePopUp->InitUI(&CurrentState);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
                
                CCMenu *menu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
                menu->setEnabled(false);
                
                ACardTableView::getInstance()->setTouchEnabled(false);
                MyCardLayer::getInstance()->setTouchEnabled(false);
            }

            //item->selected();
            break;
        case 13: // fusion
        {            
            InitSubLayer(CARD_TAB_SUB_2);
            
            const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_FUSION);
            
            if(false == TutorialCompleted)
            {
                const bool TutorialMode = false;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                tutorialProgress = TUTORIAL_FUSION_1;
                basePopUp->InitUI(&tutorialProgress);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
            }

        }
            break;
        case 14: // training
        {
            InitSubLayer(CARD_TAB_SUB_3);
            
            const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_TRAINING);
            
            if(false == TutorialCompleted)
            {
                const bool TutorialMode = false;
                NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                tutorialProgress = TUTORIAL_TRAINING_1;
                basePopUp->InitUI(&tutorialProgress);
                basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                basePopUp->setPosition(accp(0.0f, 0.0f));
                basePopUp->setTag(98765);
                MainScene::getInstance()->addChild(basePopUp, 9000);
            }
        }
            break;
        case 15: // trade
            InitSubLayer(CARD_TAB_SUB_4);
            //item->selected();
            break;
    }
    
    curLayerTag = tag;
}



void DojoLayerCard::SetNormalSubBtns(){
    //for(int i=0;i<5;i++){
    for(int i=0;i<4;i++){
        CCLabelTTF* pLabel0 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(61+i);
        pLabel0->setColor(subBtn_color_normal);
    }
}

void DojoLayerCard::SetSelectedSubBtn(int tag){
    CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(tag+50);
    if (pLabel1 != 0){
        pLabel1->setColor(COLOR_YELLOW);
    }
}

void DojoLayerCard::InitSubLayer(int a){
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    PlayerInfo *pi = PlayerInfo::getInstance();
    switch(a){
        case CARD_TAB_SUB_0:
            
            myCardLayer = new MyCardLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)) );// - accpMAIN_BTNS_UI_HEIGHT));
            myCardLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));//MAIN_BTNS_UI_HEIGHT));
            this->addChild(myCardLayer,20);
            myCardLayer->setTouchEnabled(true);
            break;
        case CARD_TAB_SUB_1:
            //myDeckLayer = new CardDeckLayer( pi->myDeck, CCSize(size.width, size.height- TOP_UI_HEIGHT - MAIN_BTNS_UI_HEIGHT));
            myDeckLayer = new CardDeckLayer( pi->myDeck, CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)) );
            myDeckLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));
            this->addChild(myDeckLayer,20);
            myDeckLayer->setTouchEnabled(true);
            break;
        case CARD_TAB_SUB_2:
            //fusionLayer = new FusionLayer(CCSize(size.width, size.height- TOP_UI_HEIGHT - MAIN_BTNS_UI_HEIGHT));
            fusionLayer = new FusionLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)));
            fusionLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));
            this->addChild(fusionLayer,20);
            fusionLayer->setTouchEnabled(true);
            
            break;
        case CARD_TAB_SUB_3:
            trainingLayer = new TrainingLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)));
            trainingLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));
            this->addChild(trainingLayer,20);
            trainingLayer->setTouchEnabled(true);
            break;
        case CARD_TAB_SUB_4:
            tradeLayer = new TradeLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)));
            tradeLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));
            this->addChild(tradeLayer,20);
            tradeLayer->setTouchEnabled(true);
            break;
    }
}

void DojoLayerCard::releaseSubLayer(){
    if (myCardLayer != NULL){
        this->removeChild(myCardLayer, true);
        myCardLayer = NULL;
    }
    if (myDeckLayer != NULL){
        this->removeChild(myDeckLayer, true);
        myDeckLayer = NULL;
    }
    if (fusionLayer != NULL){
        this->removeChild(fusionLayer,true);
        fusionLayer = NULL;
    }
    if (trainingLayer != NULL){
        this->removeChild(trainingLayer,true);
        trainingLayer = NULL;
    }
    if (tradeLayer != NULL){
        this->removeChild(tradeLayer,true);
        tradeLayer = NULL;
    }
}


void DojoLayerCard::HideMenu()
{
    CCSprite* pSprSubUIBg     = (CCSprite*)MainScene::getInstance()->getChildByTag(698);
    pSprSubUIBg->setPosition(ccp(10000,-10000));
    
    CCMenu* pSubMenu = (CCMenu*) MainScene::getInstance()->getChildByTag(699);
    pSubMenu->setPosition(ccp(10000,-10000));
    
    for(int i=0;i<4;i++){
        CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(61+i);
        pLabel1->setPositionY(pLabel1->getPositionY()-1000);
    }

}

void DojoLayerCard::ShowMenu()
{
    CCSprite* pSprSubUIBg     = (CCSprite*)MainScene::getInstance()->getChildByTag(698);
    pSprSubUIBg->setPosition(CCPointZero);
    
    CCMenu* pSubMenu = (CCMenu*) MainScene::getInstance()->getChildByTag(699);
    pSubMenu->setPosition( CCPointZero);
    
    for(int i=0;i<4;i++){
        CCLabelTTF* pLabel1 = (CCLabelTTF*)MainScene::getInstance()->getChildByTag(61+i);
        pLabel1->setPositionY(accp(100.0f));
    }
}

void DojoLayerCard::setEnableMainMenu(bool flag)
{
    CCMenu* pMenu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
    
    if (pMenu != NULL){
        pMenu->setEnabled(flag);
    }
}
/*
void DojoLayerCard::didAccelerate(cocos2d::CCAcceleration *pAccelerationValue)
{
    if (pAccelerationValue->timestamp == 0.0)
        return;
    
    char timeString[80];
    sprintf(timeString, "x:%f, y:%f, z:%f", pAccelerationValue->x, pAccelerationValue->y, pAccelerationValue->z);
    CCLog(timeString);
}
 */

/*
void *DojoLayerCard::PrintHello(void *threadid)
{
    long tid;
    tid = (long)threadid;
    printf("Thread Exit #%ld\n",tid);
    pthread_exit(NULL);
}

void *DojoLayerCard::PrintHello2(void *threadid)
{
    long tid;
    tid = (long)threadid;
    
    for(long i=0;i<900000;i++){
        
    }
    printf("Thread Exit #%ld\n",tid);
    pthread_exit(NULL);
}
void *DojoLayerCard::PrintHello3(void *threadid)
{
    long tid;
    tid = (long)threadid;
    
    
    for(long i=0;i<100000;i++){
        
    }
    
    printf("Thread Exit #%ld\n",tid);
    
    pthread_exit(NULL);
}
void *DojoLayerCard::PrintHello4(void *threadid)
{
    long tid;
    tid = (long)threadid;
    
    for(long i=0;i<500000;i++){
        
    }
    
    printf("Thread Exit #%ld\n",tid);
    
    pthread_exit(NULL);
}

void DojoLayerCard::initThread()
{
    int rc;
    long t;
    for(t=0;t<5;t++){
        CCLog("in main, create thread %;d\n",t);
        
        switch(t)
        {
            case 0:
                rc = pthread_create(&threads[t], NULL, DojoLayerCard::PrintHello, (void *)t);
                break;
            case 1:
                rc = pthread_create(&threads[t], NULL, DojoLayerCard::PrintHello, (void *)t);
                break;
            case 2:
                rc = pthread_create(&threads[t], NULL, DojoLayerCard::PrintHello2, (void *)t);
                break;
            case 3:
                rc = pthread_create(&threads[t], NULL, DojoLayerCard::PrintHello3, (void *)t);
                break;
            case 4:
                rc = pthread_create(&threads[t], NULL, DojoLayerCard::PrintHello4, (void *)t);
                break;
        }
        
        if (rc){
            printf("error; return code from pthread_create() is %d\n",rc);
            exit(-1);
        }
        
        
    }
    
    
}
*/

/*
pthread , passing argument
 /******************************************************************************
 * FILE: hello_arg2.c
 * DESCRIPTION:
 *   A "hello world" Pthreads program which demonstrates another safe way
 *   to pass arguments to threads during thread creation.  In this case,
 *   a structure is used to pass multiple arguments.
 * AUTHOR: Blaise Barney
 * LAST REVISED: 01/29/09
 ******************************************************************************/
/*
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#define NUM_THREADS	8

char *messages[NUM_THREADS];

struct thread_data
{
    int	thread_id;
    int  sum;
    char *message;
};

struct thread_data thread_data_array[NUM_THREADS];

void *PrintHello(void *threadarg)
{
    int taskid, sum;
    char *hello_msg;
    struct thread_data *my_data;
    
    sleep(1);
    my_data = (struct thread_data *) threadarg;
    taskid = my_data->thread_id;
    sum = my_data->sum;
    hello_msg = my_data->message;
    printf("Thread %d: %s  Sum=%d\n", taskid, hello_msg, sum);
    pthread_exit(NULL);
}

int main(int argc, char *argv[])
{
    pthread_t threads[NUM_THREADS];
    int *taskids[NUM_THREADS];
    int rc, t, sum;
    
    sum=0;
    messages[0] = "English: Hello World!";
    messages[1] = "French: Bonjour, le monde!";
    messages[2] = "Spanish: Hola al mundo";
    messages[3] = "Klingon: Nuq neH!";
    messages[4] = "German: Guten Tag, Welt!";
    messages[5] = "Russian: Zdravstvytye, mir!";
    messages[6] = "Japan: Sekai e konnichiwa!";
    messages[7] = "Latin: Orbis, te saluto!";
    
    for(t=0;t<NUM_THREADS;t++) {
        sum = sum + t;
        thread_data_array[t].thread_id = t;
        thread_data_array[t].sum = sum;
        thread_data_array[t].message = messages[t];
        printf("Creating thread %d\n", t);
        rc = pthread_create(&threads[t], NULL, PrintHello, (void *)
                            &thread_data_array[t]);
        if (rc) {
            printf("ERROR; return code from pthread_create() is %d\n", rc);
            exit(-1);
        }
    }
    pthread_exit(NULL);
}
*/