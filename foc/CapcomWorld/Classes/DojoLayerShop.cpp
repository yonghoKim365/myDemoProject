//
//  DojoLayerShop.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#include "DojoLayerShop.h"
#include "GameConst.h"

using namespace cocos2d;

DojoLayerShop::DojoLayerShop(CCSize layerSize) : shopLayer(NULL)
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
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite, 0);
    
    InitShopLayer();
}

DojoLayerShop::~DojoLayerShop()
{
    this->removeAllChildrenWithCleanup(true);
}

void DojoLayerShop::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
}

void DojoLayerShop::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //CCLog("touch moved,x:%f",location.x);
    
}

void DojoLayerShop::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
}

void DojoLayerShop::InitShopLayer()
{
    shopLayer = new ShopLayer(this->getContentSize());
    
    shopLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    
    shopLayer->setPosition(accp(0.0f, 0.0f));
    
    this->addChild(shopLayer);
}
