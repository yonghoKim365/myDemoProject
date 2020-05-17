//
//  PopupUnregister.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 11..
//
//

#include "PopupUnregister.h"
#include "MainScene.h"


PopupUnregister::PopupUnregister()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    setContentSize(size);
    setTouchEnabled(true);
    
//    delegate = _delegate;
    
    int add_y = 100;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f+add_y));
    this->addChild(popupBG);
    
    //    std::string itemName = "언레지스터 하시겠습니까?";
    //    CCLabelTTF* itemLabel = CCLabelTTF::create(itemName.c_str(), "Thonburi", 13);
    //    itemLabel->setColor(COLOR_WHITE);
    //    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), itemLabel, 160);
    
    //    CCLabelTTF* buyLabel = CCLabelTTF::create("언레지스터 하시겠습니까?", "Thonburi", 13);
    //    buyLabel->setColor(COLOR_WHITE);
    //    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), buyLabel, 160);
    
    //    std::string card_price = "판매시 획득 코인 :";
    //    char buf[10];
    //    sprintf(buf, "%d", card->getPrice());
    //    card_price.append(buf);
    //
    CCLabelTTF* remainLabel = CCLabelTTF::create("회원탈퇴를 하시겠습니까?", "Thonburi", 13);
    remainLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 400+add_y), remainLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(701);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f+add_y));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f+add_y), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(702);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f+add_y));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f+add_y), RightLabel, 160);
    
}

PopupUnregister::~PopupUnregister()
{
    removeAllChildrenWithCleanup(true);
    
}

void PopupUnregister::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
}

void PopupUnregister::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void PopupUnregister::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 701, localPoint))
    {
        soundButton1();
//        if (delegate != NULL){
//            delegate->ButtonOK();
//        }
        
        MainScene::getInstance()->unregisterKakao();
        MainScene::getInstance()->removePopup();
    }
    
    if(GetSpriteTouchCheckByTag(this, 702, localPoint))
    {
        soundButton1();
//        if (delegate != NULL){
//            delegate->ButtonCancel();
//        }
        MainScene::getInstance()->removePopup();
    }
}