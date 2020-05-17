//
//  OptionLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#include "OptionLayer.h"
#include "PlayerInfo.h"
#include "MainScene.h"
#include "PopupUnregister.h"
#include "PopupLogout.h"

OptionLayer::OptionLayer(CCSize layerSize)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    InitUI();
}

OptionLayer::~OptionLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void OptionLayer::InitUI()
{
    
    
    //CheckLayerSize(this);
    
    selectedLayer = 0;
    
    InitLayer(selectedLayer);
    
    /*
    CCSprite* pSprBG = CCSprite::create("");
    CCSprite* pSprBG = CCSprite::create("ui/home/option_item_tab_a2.png");
    CCSprite* pSprBG = CCSprite::create("ui/home/option_item_tab_b1.png");
    CCSprite* pSprBG = CCSprite::create("ui/home/option_item_tab_b2.png");
    */
    // 
    // option_btn_on.png
    // option_btn_off.png
    
}

void OptionLayer::InitLayer(int a){
    
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0,0));
    pSprBG->setPosition( accp(0,0) );
    this->addChild(pSprBG);
    
    int yy = this->getContentSize().height;
    
    yy-= accp(26);
    
    int menu_y = yy;
    
    CCMenuItemImage *pSpr1 = CCMenuItemImage::create("ui/home/home_item_tab_a1.png","ui/home/home_item_tab_a2.png",this,menu_selector(OptionLayer::SubUICallback));
    CCMenuItemImage *pSpr2 = CCMenuItemImage::create("ui/home/home_item_tab_b1.png","ui/home/home_item_tab_b2.png",this,menu_selector(OptionLayer::SubUICallback));
    
    pSpr1->setTag(0);
    pSpr2->setTag(1);
    
    pSpr1->setAnchorPoint(ccp(0,1));
    pSpr2->setAnchorPoint(ccp(0,1));
    
    pSpr1->setPosition(accp(10,0));
    pSpr2->setPosition(accp(10+310,0));
    
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("게임설정"  , "HelveticaNeue-Bold", 13);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("고객문의"     , "HelveticaNeue-Bold", 13);
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel1->setTag(21);
    pLabel2->setTag(22);
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    registerLabel( this,ccp(0.5,0), ccp(accp(10+310/2),yy-25+3), pLabel1,130);
    registerLabel( this,ccp(0.5,0), ccp(accp(10+310+310/2),yy-25+3), pLabel2,130);
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    registerLabel( this,ccp(0.5f,0.5f), ccp(accp(10+310/2),yy-25-5), pLabel1,130);
    registerLabel( this,ccp(0.5f,0.5f), ccp(accp(10+310+310/2),yy-25-5), pLabel2,130);
#endif
    
    
    
    if (a==0){ // game setting
        
        pSpr1->selected();
        
        yy-= accp(54);
        
        yy-= accp(10); // space
        
        CCSprite* pSprFrame1 = CCSprite::create("ui/home/option_bg01.png");
        pSprFrame1->setAnchorPoint(ccp(0,1));
        pSprFrame1->setPosition( ccp(accp(10),yy) );
        this->addChild(pSprFrame1);
        
        CCLabelTTF* pSubTitle = CCLabelTTF::create("사운드 설정"  , "HelveticaNeue-Bold", 12);
        pSubTitle->setColor(COLOR_WHITE);
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
        registerLabel( this,ccp(0,0), ccp(accp(25),yy-25+5), pSubTitle, 130);
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
        registerLabel( this,ccp(0,0.5), ccp(accp(25),yy-25-2), pSubTitle, 130);
#endif
        yy -= accp(105);
        CCLabelTTF* pLabelOption1 = CCLabelTTF::create("배경음악"  , "HelveticaNeue-Bold", 12);
        pLabelOption1->setColor(COLOR_WHITE);
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelOption1, 130);
        
        yy -= accp(70);
        CCLabelTTF* pLabelOption2 = CCLabelTTF::create("효과음"  , "HelveticaNeue-Bold", 12);
        pLabelOption2->setColor(COLOR_WHITE);
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelOption2, 130);
        
        yy -= accp(170);
        char buf[20];
        sprintf(buf, "%lld", PlayerInfo::getInstance()->userID);
        std::string strUserId = "유저아이디 : ";
        strUserId.append(buf);
        CCLabelTTF* pLabelOption3 = CCLabelTTF::create(strUserId.c_str(), "HelveticaNeue-Bold", 12);
        pLabelOption3->setColor(COLOR_WHITE);
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        registerLabel( this,ccp(0.5f,0), ccp(size.width/2,yy), pLabelOption3, 130);
        
        yy -= accp(25);
        CCSprite* pLine = CCSprite::create("ui/home/ui_home_bg_line.png");
        pLine->setAnchorPoint(ccp(0, 0));
        pLine->setPosition(ccp(accp(15), yy));
        this->addChild(pLine, 60);
        
        yy -=10;
        CCSprite *pSprBtn1 = CCSprite::create("ui/card_tab/cards_trade_regist_btnb1.png");
        pSprBtn1->setAnchorPoint( ccp(0.5,1));
        pSprBtn1->setPosition( ccp(this->getContentSize().width/2,yy));
        pSprBtn1->setTag(46);
        this->addChild(pSprBtn1,10);
        
        CCLabelTTF* pBtnLabel = CCLabelTTF::create("회원탈퇴", "HelveticaNeue-Bold", 12);
        pBtnLabel->setColor(COLOR_YELLOW);
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
        registerLabel( this,ccp(0.5,1), ccp(this->getContentSize().width/2,yy-3), pBtnLabel, 130);
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
        registerLabel( this,ccp(0.5,1), ccp(this->getContentSize().width/2,yy-6), pBtnLabel, 130);
#endif
        
        
        
        
        
        CCMenuItemImage *pSprBtn11 = CCMenuItemImage::create("ui/home/option_btn_on_d.png","ui/home/option_btn_on.png",this,menu_selector(OptionLayer::SubUICallback));
        CCMenuItemImage *pSprBtn12 = CCMenuItemImage::create("ui/home/option_btn_off_d.png","ui/home/option_btn_off.png",this,menu_selector(OptionLayer::SubUICallback));
        pSprBtn11->setTag(11);
        pSprBtn12->setTag(12);
        
        pSprBtn11->setAnchorPoint(ccp(0,1));
        pSprBtn12->setAnchorPoint(ccp(0,1));
        
        pSprBtn11->setPosition(accp(380,-(270-141)));
        pSprBtn12->setPosition(accp(380+121,-(270-141)));
        
        CCMenuItemImage *pSprBtn21 = CCMenuItemImage::create("ui/home/option_btn_on_d.png","ui/home/option_btn_on.png",this,menu_selector(OptionLayer::SubUICallback));
        CCMenuItemImage *pSprBtn22 = CCMenuItemImage::create("ui/home/option_btn_off_d.png","ui/home/option_btn_off.png",this,menu_selector(OptionLayer::SubUICallback));
        pSprBtn21->setTag(21);
        pSprBtn22->setTag(22);
        
        pSprBtn21->setAnchorPoint(ccp(0,1));
        pSprBtn22->setAnchorPoint(ccp(0,1));
        
        pSprBtn21->setPosition(accp(380,-(345-141)));
        pSprBtn22->setPosition(accp(380+121,-(345-141)));
        
        pSprBtn11->selected();
        pSprBtn21->selected();
        
        CCMenuItemImage *pSprBtn31 = CCMenuItemImage::create("ui/home/option_btn_logout1.png","ui/home/option_btn_logout2.png",this,menu_selector(OptionLayer::SubUICallback));
        pSprBtn31->setTag(31);
        pSprBtn31->setAnchorPoint(ccp(0,1));
        pSprBtn31->setPosition(accp(10,-275));
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            pSprBtn11->selected();
            pSprBtn12->unselected();
        }
        else{
            pSprBtn11->unselected();
            pSprBtn12->selected();
        }
        
        if (PlayerInfo::getInstance()->getSoundEffectOption()){
            pSprBtn21->selected();
            pSprBtn22->unselected();
        }
        else{
            pSprBtn21->unselected();
            pSprBtn22->selected();
        }
        
        CCMenu* pSubMenu = CCMenu::create(pSpr1, pSpr2, pSprBtn11, pSprBtn12, pSprBtn21, pSprBtn22, pSprBtn31, NULL);
        
        pSubMenu->setAnchorPoint(ccp(0,0));
        pSubMenu->setPosition( ccp(0,menu_y));
        pSubMenu->setTag(99);
        this->addChild(pSubMenu,120);
        
        
        
        
        
    }
    else if (a==1){ // game faq
        
        pSpr1->unselected();
        pSpr2->selected();
        
        
        
                
        CCMenu* pSubMenu = CCMenu::create(pSpr1, pSpr2, NULL);
        
        pSubMenu->setAnchorPoint(ccp(0,0));
        pSubMenu->setPosition( ccp(0,menu_y));
        pSubMenu->setTag(99);
        this->addChild(pSubMenu,120);
        
        yy -= 60;
        
        int add_y = 20;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)        
        yy -= 40;
        add_y = 30;
#endif
        
        const char* text1 = "FOC를 즐겨주셔서 감사합니다.";
        const char* text2 = "게임 방법에 관련된 자세한 내용은 홈페이지를 ";
        const char* text3 = "참조해 주세요.";
        const char* text4 = "그밖에 게임 관련 최신 뉴스, 업데이트 사항, 이벤트 등";
        const char* text5 = "다양한 소식과 내용을 확인하실 수 있습니다.";
        const char* text6 = "";
        
        CCLabelTTF* pLabelFaq1 = CCLabelTTF::create(text1, "HelveticaNeue-Bold", 12);
        CCLabelTTF* pLabelFaq2 = CCLabelTTF::create(text2, "HelveticaNeue-Bold", 12);
        CCLabelTTF* pLabelFaq3 = CCLabelTTF::create(text3, "HelveticaNeue-Bold", 12);
        CCLabelTTF* pLabelFaq4 = CCLabelTTF::create(text4, "HelveticaNeue-Bold", 12);
        CCLabelTTF* pLabelFaq5 = CCLabelTTF::create(text5, "HelveticaNeue-Bold", 12);
        CCLabelTTF* pLabelFaq6 = CCLabelTTF::create(text6, "HelveticaNeue-Bold", 12);
        
        pLabelFaq1->setColor(COLOR_WHITE);
        pLabelFaq2->setColor(COLOR_WHITE);
        pLabelFaq3->setColor(COLOR_WHITE);
        pLabelFaq4->setColor(COLOR_WHITE);
        pLabelFaq5->setColor(COLOR_WHITE);
        pLabelFaq6->setColor(COLOR_WHITE);
        
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq1, 130);
        yy -= add_y;
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq2, 130);
        yy -= add_y;
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq3, 130);
        yy -= add_y;
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq4, 130);
        yy -= add_y;
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq5, 130);
        yy -= add_y;
        registerLabel( this,ccp(0,0), ccp(accp(30),yy), pLabelFaq6, 130);
        
        yy -= 10;
        CCSprite *pSprBtn1 = CCSprite::create("ui/card_tab/cards_trade_regist_btnb1.png");
        pSprBtn1->setAnchorPoint( ccp(0.5,1));
        pSprBtn1->setPosition( ccp(this->getContentSize().width/2,yy));
        pSprBtn1->setTag(45);
        this->addChild(pSprBtn1,10);
        
        CCLabelTTF* pBtnLabel = CCLabelTTF::create("홈페이지로 이동", "HelveticaNeue-Bold", 12);
        pBtnLabel->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0.5,1), ccp(this->getContentSize().width/2,yy-3), pBtnLabel, 130);
    }
}

void OptionLayer::SubUICallback(CCObject* pSender){
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    CCMenu *menu = (CCMenu*)this->getChildByTag(99);
    
    CCMenuItemImage *item0 = (CCMenuItemImage *)menu->getChildByTag(0);
    CCMenuItemImage *item1 = (CCMenuItemImage *)menu->getChildByTag(1);
    CCMenuItemImage *item11 = (CCMenuItemImage *)menu->getChildByTag(11);
    CCMenuItemImage *item12 = (CCMenuItemImage *)menu->getChildByTag(12);
    CCMenuItemImage *item21 = (CCMenuItemImage *)menu->getChildByTag(21);
    CCMenuItemImage *item22 = (CCMenuItemImage *)menu->getChildByTag(22);
    
    soundButton1();
    
    switch(tag){
        case 0:
            item0->selected();
            item1->unselected();
            if (selectedLayer != 0){
                selectedLayer = 0;
                this->removeAllChildrenWithCleanup(true);
                InitLayer(selectedLayer);
            }
            
            break;
        case 1:
            item0->unselected();
            item1->selected();
            if (selectedLayer != 1){
                selectedLayer = 1;
                this->removeAllChildrenWithCleanup(true);
                InitLayer(selectedLayer);
            }
            break;
        case 11:
            PlayerInfo::getInstance()->setBgmOption(true);
            item11->selected();
            item12->unselected();
            if (CocosDenshion::SimpleAudioEngine::sharedEngine()->isBackgroundMusicPlaying()){
                CocosDenshion::SimpleAudioEngine::sharedEngine()->resumeBackgroundMusic();
            }
            else{
                soundMainBG();
            }
            break;
        case 12:
            PlayerInfo::getInstance()->setBgmOption(false);
            item11->unselected();
            item12->selected();
            if (CocosDenshion::SimpleAudioEngine::sharedEngine()->isBackgroundMusicPlaying()){
                CocosDenshion::SimpleAudioEngine::sharedEngine()->pauseBackgroundMusic();
            }
            break;
        case 21:
            PlayerInfo::getInstance()->setSoundEffectOption(true);
            item21->selected();
            item22->unselected();
            break;
        case 22:
            PlayerInfo::getInstance()->setSoundEffectOption(false);
            item21->unselected();
            item22->selected();
            break;
        case 31:
            PopupLogout *popup = new PopupLogout();
            //PopupLogout *popup = new PopupLogout();
            popup->setAnchorPoint(ccp(0,0));
            popup->setPosition(ccp(0,0));
            popup->setTag(123);
            
            MainScene::getInstance()->addPopup(popup,1000);
            
            //MainScene::getInstance()->unregisterKakao();
            break;
            
    }
    
    
}

void OptionLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 45, localPoint)){ // COPY
        CCLog(" move to FOC home page");
    }
    if (GetSpriteTouchCheckByTag(this, 46, localPoint)){ //
        CCLog(" unregister");
        PopupUnregister *popup = new PopupUnregister();
        popup->setAnchorPoint(ccp(0,0));
        popup->setPosition(ccp(0,0));
        popup->setTag(123);
        
        MainScene::getInstance()->addPopup(popup,1000);
    }
}
