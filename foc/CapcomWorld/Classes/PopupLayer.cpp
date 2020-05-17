//
//  PopupLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 28..
//
//

#include "PopupLayer.h"
#include "MainScene.h"
#include "kakaoLoginScene.h"

PopupLayer::PopupLayer()
{
    setDisableWithRunningScene();
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    setContentSize(size);
    
    CCSprite *pSprBg = CCSprite::create("ui/shop/popup_bg_a.png");
    pSprBg->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprBg, 0);
    
    CCMenuItemImage *pSpr1 = CCMenuItemImage::create("ui/shop/popup_btn_c1.png", "ui/shop/popup_btn_c2.png",this,menu_selector(PopupLayer::SubUICallback));
    pSpr1->setTag(0);
    pSpr1->setAnchorPoint(ccp(0.5,0));
    pSpr1->setPosition(accp(0,0));
    CCMenu* pSubMenu = CCMenu::create(pSpr1, NULL);
    
    pSubMenu->setPosition( ccp(size.width/2,size.height/2-80));
    pSubMenu->setAnchorPoint(ccp(0.5,0));
    pSubMenu->setTag(99);
    this->addChild(pSubMenu,100);
    
    CCLabelTTF* pBtnLabel1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn")  , "HelveticaNeue-Bold", 12);
    pBtnLabel1->setColor(COLOR_YELLOW);
    pBtnLabel1->setTag(21);
    registerLabel( this, ccp(0.5,0), ccp(size.width/2,size.height/2-80+3), pBtnLabel1,130);
    
    this->setTouchEnabled(true);
    //CCLog(" getcontenty:%d",this->getPositionY());
    
    nCallFrom = 0;
}

PopupLayer::~PopupLayer()
{
    removeAllChildrenWithCleanup(true);
    
}

void PopupLayer::setText(const char *text1)
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

void PopupLayer::setText_quest(const char* text1)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int text_yy = size.height/2;
    
    if (text1 != NULL){
        CCLabelTTF* pLabel2 = CCLabelTTF::create(text1, "Thonburi", 12);
        pLabel2->setColor(subBtn_color_normal);
        pLabel2->setTag(21);
        registerLabel( this, ccp(0.5,0.5), ccp(size.width/2, text_yy), pLabel2,130);
    }
}

void PopupLayer::setText(const char *text1, const char *text2, const char *text3)
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

    text_yy -= 30;
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

void PopupLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
}

void PopupLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
}

void PopupLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCLog("popupLayer, touch end");
 
}

void PopupLayer::SubUICallback(CCObject* pSender){
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    CCLog("popupLayer, SubUICallback, tag:%d",tag);
    
    switch(tag){
        case 0: // my card
            //MainScene::getInstance()->removeChildByTag(123, true);
            
            CCLog("popupLayer, SubUICallback, 2 ");

            soundButton1();
            
            restoreTouchDisable();
            
            if (nCallFrom == 0){
                MainScene::getInstance()->removePopup();
            }
            else if (nCallFrom == 1){
                KakaoLoginScene::getInstance()->removeChildByTag(123, true);
                KakaoLoginScene::getInstance()->showButtons(true);
            }
            
            CCLog("popupLayer, SubUICallback, 233333 ");
            break;
    }
}

