//
//  PopupRetry.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 21..
//
//

#include "PopupRetry.h"
#include "MainScene.h"


PopupRetry::PopupRetry()
{
//    setDisableWithRunningScene();
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    setContentSize(size);
    
    CCSprite *pSprBg = CCSprite::create("ui/shop/popup_bg_a.png");
    pSprBg->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprBg, 0);
    
    CCMenuItemImage *pSpr1 = CCMenuItemImage::create("ui/shop/popup_btn_c1.png", "ui/shop/popup_btn_c2.png",this,menu_selector(PopupRetry::SubUICallback));
    pSpr1->setTag(0);
    pSpr1->setAnchorPoint(ccp(0.5,0));
    pSpr1->setPosition(accp(0,0));
    CCMenu* pSubMenu = CCMenu::create(pSpr1, NULL);
    
    pSubMenu->setPosition( ccp(size.width/2,size.height/2-80));
    pSubMenu->setAnchorPoint(ccp(0.5,0));
    pSubMenu->setTag(99);
    this->addChild(pSubMenu,100);
    
    CCLabelTTF* pBtnLabel1 = CCLabelTTF::create("재시도"  , "Thonburi", 13);
    pBtnLabel1->setColor(COLOR_YELLOW);
    pBtnLabel1->setTag(21);
    registerLabel( this, ccp(0.5,0), ccp(size.width/2,size.height/2-80+3), pBtnLabel1,130);
    
    //CCLog(" getcontenty:%d",this->getPositionY());
}

PopupRetry::~PopupRetry()
{
    removeAllChildrenWithCleanup(true);
    
}

void PopupRetry::setText(const char *text1)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int lineNum = 1;
    int len = strlen(text1);
    for (int scan = 0;scan < len;scan++)
    {
        if (text1[scan] == '\n' || text1[scan] == '\r')
            lineNum++;
    }
    
    int text_yy = size.height/2 - (lineNum - 1) * 6;
    
    if (text1 != NULL){
        CCLabelTTF* pLabel2 = CCLabelTTF::create(text1, "Thonburi", 13);
        pLabel2->setColor(subBtn_color_normal);
        pLabel2->setTag(21);
        registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabel2,130);
    }
}

void PopupRetry::setText_quest(const char* text1)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int text_yy = size.height/2;
    
    if (text1 != NULL){
        CCLabelTTF* pLabel2 = CCLabelTTF::create(text1, "Thonburi", 10);
        pLabel2->setColor(subBtn_color_normal);
        pLabel2->setTag(21);
        registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy - 30.0f), pLabel2,130);
    }
}

void PopupRetry::setText(const char *text1, const char *text2, const char *text3)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    int text_yy = size.height/2+70;
    CCLabelTTF* pLabelTitle = CCLabelTTF::create("Network Error"  , "Thonburi", 13);
    pLabelTitle->setColor(subBtn_color_normal);
    pLabelTitle->setTag(21);
    registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabelTitle,130);
    
    text_yy = size.height/2+70;
    text_yy -= 20;
    
    std::string userID = "user ID :";
    
    char buf1[30];
    sprintf(buf1, "%lld", PlayerInfo::getInstance()->userID);
    userID.append(buf1);
    CCLabelTTF* pLabel1 = CCLabelTTF::create(userID.c_str(), "Thonburi", 13);
    pLabel1->setColor(subBtn_color_normal);
    pLabel1->setTag(21);
    registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabel1,130);
    
    text_yy -= 20;
    if (text1 != NULL){
        CCLabelTTF* pLabel2 = CCLabelTTF::create(text1, "Thonburi", 13);
        pLabel2->setColor(subBtn_color_normal);
        pLabel2->setTag(21);
        registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabel2,130);
    }
    
    text_yy -= 20;
    
    if (text2 != NULL){
        CCLabelTTF* pLabel3 = CCLabelTTF::create(text2, "Thonburi", 13);
        pLabel3->setColor(subBtn_color_normal);
        pLabel3->setTag(21);
        registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabel3, 130);
    }
    
    text_yy -= 20;
    
    if (text3 != NULL){
        CCLabelTTF* pLabel4 = CCLabelTTF::create(text3, "Thonburi", 13);
        pLabel4->setColor(subBtn_color_normal);
        pLabel4->setTag(21);
        registerLabel( this, ccp(0.5,0), ccp(size.width/2, text_yy), pLabel4, 130);
    }
    
}

void PopupRetry::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
}

void PopupRetry::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
}

void PopupRetry::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void PopupRetry::setDelegate(PopupRetryDelegate *dele)
{
    delegate = dele;
    
}

void PopupRetry::SubUICallback(CCObject* pSender){
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    
    switch(tag){
        case 0: // my card
            //MainScene::getInstance()->removeChildByTag(123, true);
            
            soundButton1();
            
            restoreTouchDisable();
            
            if (delegate){
                //delegate->BtnRetry();
                MainScene::getInstance()->schedule(schedule_selector(MainScene::BtnRetry),0.1);
                
            }
            
            MainScene::getInstance()->removePopup();
            break;
    }
}

void PopupRetry::Retry()
{
    if (delegate){
        delegate->BtnRetry();
    }
}