//
//  DojoLayerBg.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 24..
//
//

#include "DojoLayerBg.h"
#include "DojoLayerDojo.h"
#include "FileManager.h"
#include "ARequestSender.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

DojoLayerBg::DojoLayerBg(CCSize layerSize) : m_pSelected(NULL), m_DojoLayerDojo(NULL), m_pTopbar(NULL), m_pBottombar(NULL)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    this->setContentSize(layerSize);
    

    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0, -50) );
    this->addChild(pSprite, 0);
    
    selectBgID = 0;
    
    InitUI();
}

DojoLayerBg::~DojoLayerBg()
{
    this->removeAllChildrenWithCleanup(true);
}

void DojoLayerBg::InitUI()
{
    int XPos = 20;
    //int YPos = (this->getContentSize().height * SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_MARGIN - MAIN_LAYER_BTN_HEIGHT;// - 40 ;
    int YPos = (this->getContentSize().height) - accp(MAIN_LAYER_TOP_MARGIN + MAIN_LAYER_BTN_HEIGHT);// - 40 ;
    
    //CheckLayerSize(this);

    /*
    CCMenuItemImage *pConfirmBtn = CCMenuItemImage::create("ui/home/ui_bg_confirm.png", NULL, this, menu_selector(DojoLayerBg::ConfirmCallback));
    pConfirmBtn->setAnchorPoint( ccp(0, 0));
    pConfirmBtn->setPosition( accp(10, YPos));
    pConfirmBtn->setTag(100);
    
    CCMenu* pMenu = CCMenu::create(pConfirmBtn, NULL);
    pMenu->setPosition( CCPointZero );
    this->addChild(pMenu, 50);
*/
    
    /////////////////////////
    
    YPos -= accp(CARDLIST_PREV_BTN_UPPER_SPACE);
    YPos -= accp(CARDLIST_PREV_BTN_H);
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(DojoLayerBg::ConfirmCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp(10,0));//size.width/5 * 0,0));
    pSprBtn1->setTag(0);
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( ccp(0, YPos));
    pMenu->setTag(199);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기 "   , "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5,0.5), ccp( getContentSize().width/2, YPos + accp(24)), pLabel1, 130);
    
    ///////////////////////////
    
    YPos -= accp(CARDLIST_PREV_BTN_UPPER_SPACE);
    
    YPos -= accp(10);
    
    m_pTopbar = CCSprite::create("ui/main_bg/ui_contents_bg1.png");
    regSprite(this,  ccp(0, 0), ccp(accp(10), YPos), m_pTopbar, 100);

    YPos -= accp(112);
    
    int MiddelBarcount = 0;
    
    int i=0;
    //ARequestSender::getInstance()->requestBgList();
    
    DojoLayerDojo* dojo = DojoLayerDojo::getInstance();
    BgCount = PlayerInfo::getInstance()->bgList->size();
    for(vector<int>::iterator itor = PlayerInfo::getInstance()->bgList->begin(); itor !=PlayerInfo::getInstance()->bgList->end() ; ++itor)
    {
        Bg_List* bg = (Bg_List*)dojo->BgDictionary->objectForKey((*itor));
        CCSprite* pSpr = CCSprite::create(bg->S_IMgPath.c_str());
        pSpr->setTag(i);
        regSprite(this,  ccp(0, 0), ccp(accp(XPos), YPos), pSpr, 100);
        
        BG_ID.push_back(*itor);
        
        XPos += 192 + 12;
        
        if((2 == i % 3) && (i != BgCount-1))
        {
            //m_pMiddlebar[MiddelBarcount] = CCSprite::create("ui/main_bg/ui_contents_bg2.png");
            CCSprite* pSpr1 = CCSprite::create("ui/main_bg/ui_contents_bg2.png");
            regSprite(this,  ccp(0, 0), ccp(accp(10), YPos - accp(10)), pSpr1, 90);
            
            CCSprite* middle2 = CCSprite::create("ui/main_bg/ui_contents_bg4.png");
            regSprite(this,  ccp(0, 0), ccp(accp(10), YPos), middle2, 90);
            
            ++MiddelBarcount;
            
            XPos = 20;
            YPos -= accp(10+ 112);
        }
        
        ++i;
    }
    
    int userBG = PlayerInfo::getInstance()->getBackground();
    const int startBG = 5001;
    RenderSelectedImg(userBG - startBG);
    
    CCSprite* middle = CCSprite::create("ui/main_bg/ui_contents_bg4.png");
    regSprite(this,  ccp(0, 0), ccp(accp(10), YPos), middle, 90);
    
    m_pBottombar = CCSprite::create("ui/main_bg/ui_contents_bg3.png");
    regSprite(this,  ccp(0, 0), ccp(accp(10), YPos - accp(10)), m_pBottombar, 100);
}

void DojoLayerBg::ConfirmCallback(CCObject* pSender)
{
    soundButton1();
    
    if(0 == selectBgID)
    {
        this->removeAllChildrenWithCleanup(true);
        
        DojoLayerDojo *Dojolayer = (DojoLayerDojo*)this->getParent();
        Dojolayer->setTouchEnabled(true);

        return;
    }
    
    downloadBG();
}

void DojoLayerBg::downloadBG()
{
    addPageLoading();
    
    bool bgStart = true;
    
    FileManager* fmanager = FileManager::sharedFileManager();
    std::string basePath = FOC_IMAGE_SERV_URL;
    basePath.append("images/bg/");
    
    Bg_List* bglist = (Bg_List*)DojoLayerDojo::getInstance()->BgDictionary->objectForKey(selectBgID);
    
    std::vector<std::string> downloads;
    
    if(!fmanager->IsFileExist(bglist->L_ImgPath.c_str()))
    {
        bgStart = false;
        string downPath = basePath + bglist->L_ImgPath;
        downloads.push_back(downPath);
    }
    
    if(false == bgStart)
    {
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        requestor->addDownloadTask(downloads, this, callfuncND_selector(DojoLayerBg::onHttpRequestCompleted));
    }
    
    if(true == bgStart)
    {
        removePageLoading();
        
        this->removeAllChildrenWithCleanup(true);

        DojoLayerDojo *Dojolayer = (DojoLayerDojo*)this->getParent();
        Dojolayer->setTouchEnabled(true);
        
        if (!ARequestSender::getInstance()->requestSelectBg(selectBgID))
            CCLog("send fail");
        
        Dojolayer->ChangeBGImg(selectBgID);
    }
}

void DojoLayerBg::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        removePageLoading();
        
        this->removeAllChildrenWithCleanup(true);

        DojoLayerDojo *Dojolayer = (DojoLayerDojo*)this->getParent();
        Dojolayer->setTouchEnabled(true);
        
        if (!ARequestSender::getInstance()->requestSelectBg(selectBgID))
            CCLog("send fail");
        
        Dojolayer->ChangeBGImg(selectBgID);
    }
}

void DojoLayerBg::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void DojoLayerBg::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void DojoLayerBg::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for(int i=0; i<BgCount; ++i)
    {
        if (GetSpriteTouchCheckByTag(this, i, localPoint))
        {
            soundButton1();
            
            RenderSelectedImg(i);
            
            selectBgID = BG_ID[i];
        }
    }
}

void DojoLayerBg::RenderSelectedImg(int PosIdx)
{
    this->removeChild(m_pSelected, true);

    CCNode* pNode = this->getChildByTag(PosIdx);
    CCSprite* pSpr = (CCSprite*)pNode;
    float x = pSpr->getPositionX();
    float y = pSpr->getPositionY();
    
    m_pSelected = CCSprite::create("ui/home/ui_bg_check.png");
    m_pSelected->setAnchorPoint(ccp(0, 0));
    m_pSelected->setPosition(ccp(0, 0));
    regSprite(this,  ccp(0, 0), accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE), m_pSelected, 100);    
}