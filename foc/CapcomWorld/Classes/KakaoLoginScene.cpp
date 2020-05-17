//
//  KakaoLoginScene.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 22..
//
//

#include "KakaoLoginScene.h"
#include "TitleScene.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "CardDictionary.h"
#include "FileManager.h"
#include "ARequestSender.h"
#include "PlayerInfo.h"
#include "MainScene.h"

using namespace cocos2d;


KakaoLoginScene *KakaoLoginScene::instance = NULL;
CCScene* KakaoLoginScene::scene()
{
    //CCLog("titlescene debug msg");
    
    // 'scene' is an autorelease object
    CCScene *scene = CCScene::create();
    
    // 'layer' is an autorelease object
    KakaoLoginScene *layer = KakaoLoginScene::create();
    
    // add layer as a child to scene
    scene->addChild(layer);
    
    //CCLog("titlescene debug msg 2");
    
    
    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool KakaoLoginScene::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !CCLayer::init() )
    {
        return false;
    }
    
    instance = this;
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCSprite* pSprite = CCSprite::create("title/login_bg_s.png");//HelloWorld.png");
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    this->setTag(10);
    
    pSprite = CCSprite::create("title/copyright.png");
    pSprite->setPosition( ccp(size.width/2, 75) );
    pSprite->setTag(55);
    pSprite->setVisible(false);
    this->addChild(pSprite, 0);

    CCMenuItemImage *pSprMenu1 = CCMenuItemImage::create("title/login_btn_kakao1.png","title/login_btn_kakao2.png",this,menu_selector(KakaoLoginScene::MenuCallback));
    pSprMenu1->setAnchorPoint( ccp(0,0));
    pSprMenu1->setPosition( accp( 65,130));//size.width/5 * 0,0));
    pSprMenu1->setTag(0);
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    CCMenuItemImage *pSprMenu2 = CCMenuItemImage::create("title/login_btn_guest1.png","title/login_btn_guest2.png",this,menu_selector(KakaoLoginScene::MenuCallback));
    pSprMenu2->setAnchorPoint( ccp(0,0));
    pSprMenu2->setPosition( accp( 65,55));//size.width/5 * 0,0));
    pSprMenu2->setTag(1);
    
    CCMenu* pMenu = CCMenu::create(pSprMenu1,pSprMenu2, NULL);
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    CCMenu* pMenu = CCMenu::create(pSprMenu1, NULL);
#endif
    
    pMenu->setPosition( CCPointZero );
    pMenu->setTag(99);
    this->addChild(pMenu, 50);
    
    
    
    
    //CardDictionary::sharedCardDictionary()->init();
    //FileManager::sharedFileManager()->Init();
    
    //    this->schedule(schedule_selector(TitleScene::goMainScene),1.0);
    
    //xb = new XBridge(); //XBridge *xb = new XBridge();
    //PlayerInfo::getInstance()->SetDeviceID(xb->getDeviceID());
    
    //PlayerInfo::getInstance()->xb = new XBridge();
    //PlayerInfo::getInstance()->xb->kakao();
    
    setKeypadEnabled(true);
    
    return true;
}


void KakaoLoginScene::keyBackClicked()
{
    CCLog(" keyBackClicked");
    if (bExitPopup == false){
        ExitWarningPopup *popup = new ExitWarningPopup();
        popup->InitUI();
        popup->setTag(123);
        popup->nCallFrom = 1;
        this->addChild(popup,1000);
        bExitPopup = true;
    }
}

void KakaoLoginScene::showButtons(bool flag)
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    if (pMenu){
        pMenu->setVisible(flag);
    }

    CCSprite *sprite = (CCSprite*) this->getChildByTag(55);
    sprite->setVisible(!flag);
}

extern "C" {
    void showKakoLoginButton(bool flag)
    {
        KakaoLoginScene::getInstance()->showButtons(flag);
    }
}

void KakaoLoginScene::MenuCallback(CCObject* pSender){
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    //CCMenu *menu = (CCMenu*)node->getParent();
    
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    
    if (bExitPopup)return;
    
    switch(tag){
        case 0:
            changeImage();
            PlayerInfo::getInstance()->xb->goUserLogin();
            break;
        case 1:
            // for test
            //keyBackClicked();
            PlayerInfo::getInstance()->xb->goGuestLogin();
            break;
    }
}

void KakaoLoginScene::changeImage()
{
    this->removeChildByTag(0, true);
    this->removeChildByTag(10, true);
    this->removeChildByTag(99, true);
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCSprite* pSprite = CCSprite::create("title/title_bg_s.png");//HelloWorld.png");
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    this->setTag(10);
}

void KakaoLoginScene::switchMainScene()
{
    this->schedule(schedule_selector(KakaoLoginScene::goMainScene),1.0);

}

void KakaoLoginScene::goMainScene()
{
    CCDirector::sharedDirector()->replaceScene(MainScene::scene());
}
/*
void KakaoLoginScene::gameLogic(cocos2d::CCTime dt)
{
    CCLog(" call game logic");
}

void TitleScene::switchMainScene()
{
    this->schedule(schedule_selector(TitleScene::goMainScene),1.0);
}

void KakaoLoginScene::goMainScene()
{
    CCDirector::sharedDirector()->replaceScene(MainScene::scene());
}
*/