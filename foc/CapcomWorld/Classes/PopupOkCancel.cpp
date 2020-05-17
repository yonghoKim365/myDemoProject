//
//  PopupOkCancel.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 3..
//
//

#include "PopupOkCancel.h"
#include "MainScene.h"


PopupOkCancel::PopupOkCancel(CardInfo* card, PopupDelegate *_delegate)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    setContentSize(size);
    setTouchEnabled(true);
    
    delegate = _delegate;
    sellCard = card;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
    
    std::string strGrade = "노멀)";
    switch( card->getRare()){
        case 1: strGrade = "레어)";   break;
        case 2: strGrade = "슈퍼)";   break;
        case 3: strGrade = "울트라)"; break;
        case 4: strGrade = "에픽)";   break;
    }
    
    std::string itemName = "\"";
    itemName.append(card->getName()).append("(").append(strGrade).append("\"");
    CCLabelTTF* itemLabel = CCLabelTTF::create(itemName.c_str(), "Thonburi", 13);
    itemLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 480.0f), itemLabel, 160);
    
    CCLabelTTF* buyLabel = CCLabelTTF::create("파시겠습니까?", "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f), buyLabel, 160);
    
    std::string card_price = "판매시 획득 코인 :";
    char buf[10];
    sprintf(buf, "%d", card->getPrice());
    card_price.append(buf);
    
    CCLabelTTF* remainLabel = CCLabelTTF::create(card_price.c_str(), "Thonburi", 13);
    remainLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 390.0f), remainLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(701);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(702);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    
    
}

PopupOkCancel::~PopupOkCancel()
{
    removeAllChildrenWithCleanup(true);
    
}

void PopupOkCancel::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){ 
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
}

void PopupOkCancel::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void PopupOkCancel::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 701, localPoint))
    {
        soundButton1();
        if (delegate != NULL){
            delegate->ButtonOK(sellCard);
        }
    }
    
    if(GetSpriteTouchCheckByTag(this, 702, localPoint))
    {
        soundButton1();
        if (delegate != NULL){
            delegate->ButtonCancel();
        }
        MainScene::getInstance()->removePopup();
    }
}


////////////

PopupCardSelelctForFusion::PopupCardSelelctForFusion(CardInfo* card, const char* text, PopupDelegate *_delegate)
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    setContentSize(size);
    setTouchEnabled(true);
    
    delegate = _delegate;
    sellCard = card;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(89.0f, 220.0f));
    this->addChild(popupBG);
        
    CCLabelTTF* buyLabel = CCLabelTTF::create(text, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.0f), accp(319.0f, 450.0f-70), buyLabel, 160);
        
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(701);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, 225.0f));
    this->addChild(LeftBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, 235.0f), LeftLabel, 160);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(702);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, 225.0f));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, 235.0f), RightLabel, 160);
    
    
}

PopupCardSelelctForFusion::~PopupCardSelelctForFusion()
{
    removeAllChildrenWithCleanup(true);
    
}

void PopupCardSelelctForFusion::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
}

void PopupCardSelelctForFusion::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void PopupCardSelelctForFusion::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 701, localPoint))
    {
        soundButton1();
        if (delegate != NULL){
            delegate->ButtonOK(sellCard);
        }
    }
    
    if(GetSpriteTouchCheckByTag(this, 702, localPoint))
    {
        soundButton1();
        if (delegate != NULL){
            delegate->ButtonCancel();
        }
        MainScene::getInstance()->removePopup();
    }
}
