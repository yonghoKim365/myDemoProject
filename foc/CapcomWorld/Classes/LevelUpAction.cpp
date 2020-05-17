//
//  LevelUpAction.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 12. 12..
//
//

#include "LevelUpAction.h"
#include "MainScene.h"

LevelUpAction::LevelUpAction(CCSize layerSize, int _uPoint)
{
    this->setTouchEnabled(false);
    this->setContentSize(layerSize);
    
    uPoint = _uPoint;
    
    setDisableWithRunningScene();
    
    InitUI();
}

LevelUpAction::~LevelUpAction()
{
    this->removeAllChildrenWithCleanup(true);
}

void LevelUpAction::InitUI()
{
    CCSprite* blackBG = CCSprite::create("ui/shop/bg_black.png");
    blackBG->setAnchorPoint(ccp(0.0f, 0.0f));
    blackBG->setPosition(accp(0.0f, 0.0f));
    blackBG->setOpacity(0);
    blackBG->setTag(15000);
    MainScene::getInstance()->addChild(blackBG, 13000);

    CCFadeIn* in1 = CCFadeIn::actionWithDuration(0.3f);
    blackBG->runAction(in1);

    CCSprite* levelBG = CCSprite::create("ui/quest/lvup_bg.png");
    levelBG->setAnchorPoint(ccp(0.0f, 0.0f));
    levelBG->setPosition(accp(0.0f, 0.0f));
    levelBG->setOpacity(0);
    levelBG->setTag(15001);
    MainScene::getInstance()->addChild(levelBG, 14000);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.3f);
    CCFadeIn* in2 = CCFadeIn::actionWithDuration(0.3f);
    levelBG->runAction(CCSequence::actions(delay2, in2, NULL));
    
    CCSprite* effect = CCSprite::create("ui/quest/lvup_bg_eff.png");
    effect->setAnchorPoint(ccp(0.0f, 0.0f));
    effect->setOpacity(0);
    effect->setPosition(accp(0.0f, 0.0f));
    effect->setTag(15002);
    MainScene::getInstance()->addChild(effect, 14000);
    
    CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.8f);
    CCFadeIn* in3 = CCFadeIn::actionWithDuration(0.2f);
    effect->runAction(CCSequence::actions(delay3, in3, NULL));
    
    CCSprite* levelUP = CCSprite::create("ui/quest/lvup_a.png");
    levelUP->setAnchorPoint(ccp(0.5f, 0.5f));
    levelUP->setScale(0.0f);
    levelUP->setOpacity(20);
    levelUP->setPosition(accp(94.0f + (454.0f/2.0f), 457.0f + (204.0f/2.0f)));
    levelUP->setTag(15003);
    MainScene::getInstance()->addChild(levelUP, 15000);
    
    CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.8f);
    CCFiniteTimeAction* actionScale = CCScaleTo::actionWithDuration(0.2f, 1.0f);
    CCFadeIn* in4 = CCFadeIn::actionWithDuration(0.2f);
    
    CCCallFunc* callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(LevelUpAction::TextAction));

    CCDelayTime *delay5 = CCDelayTime::actionWithDuration(1.0f);
    CCCallFunc* callBack2 = CCCallFunc::actionWithTarget(this, callfunc_selector(LevelUpAction::setTouch));

    
    levelUP->runAction(CCSequence::actions(delay4, in4, actionScale, delay2, callBack, delay5, callBack2, NULL));
}

void LevelUpAction::TextAction()
{
    CCSprite* pSpr = CCSprite::create("ui/card_tab/cards_text_bg.png");
    pSpr->setAnchorPoint(ccp(0.0f, 0.0f));
    pSpr->setPosition(accp(60.0f, 224.0f));
    pSpr->setTag(15004);
    MainScene::getInstance()->addChild(pSpr, 14000);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("레벨", "HelveticaNeue-Bold", 13);
    pLabel1->setColor(COLOR_ORANGE);
    pLabel1->setTag(15005);
    registerLabel(MainScene::getInstance(), ccp(0.0f, 0.0f), accp(80.0f, 330.0f), pLabel1, 16001);
    
    CCLabelTTF* pLabel4 = CCLabelTTF::create("+1", "HelveticaNeue-Bold", 13);
    pLabel4->setColor(COLOR_WHITE);
    pLabel4->setTag(15006);
    registerLabel(MainScene::getInstance(), ccp(0.0f, 0.0f), accp(130.0f, 330.0f), pLabel4, 16001);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("능력치", "HelveticaNeue-Bold", 13);
    pLabel2->setColor(COLOR_ORANGE);
    pLabel2->setTag(15007);
    registerLabel(MainScene::getInstance(), ccp(0.0f, 0.0f), accp(80.0f, 300.0f), pLabel2, 16001);
    
    string strUpoint = "+";
    char buf[10];
    sprintf(buf, "%d", uPoint);
    string UPoint1 = buf;
    
    strUpoint = strUpoint + UPoint1;
    
    CCLabelTTF* pLabel5 = CCLabelTTF::create(strUpoint.c_str(), "HelveticaNeue-Bold", 13);
    pLabel5->setColor(COLOR_WHITE);
    pLabel5->setTag(15008);
    registerLabel(MainScene::getInstance(), ccp(0.0f, 0.0f), accp(155.0f, 300.0f), pLabel5, 16001);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("모든 소모 포인트가 전부 회복 되었습니다.", "HelveticaNeue-Bold", 13);
    pLabel3->setColor(COLOR_WHITE);
    pLabel3->setTag(15009);
    registerLabel(MainScene::getInstance(), ccp(0.0f, 0.0f), accp(80.0f, 248.0f), pLabel3, 16001);
    
    //this->setTouchEnabled(true);
}

void LevelUpAction::setTouch()
{
    this->setTouchEnabled(true);
}

void LevelUpAction::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{

}

void LevelUpAction::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    MainScene::getInstance()->removeChildByTag(15000, true);
    MainScene::getInstance()->removeChildByTag(15001, true);
    MainScene::getInstance()->removeChildByTag(15002, true);
    MainScene::getInstance()->removeChildByTag(15003, true);
    MainScene::getInstance()->removeChildByTag(15004, true);
    MainScene::getInstance()->removeChildByTag(15005, true);
    MainScene::getInstance()->removeChildByTag(15006, true);
    MainScene::getInstance()->removeChildByTag(15007, true);
    MainScene::getInstance()->removeChildByTag(15008, true);
    MainScene::getInstance()->removeChildByTag(15009, true);

    restoreTouchDisable();
    
    MainScene::getInstance()->removeChildByTag(9999, true);
    
    this->removeAllChildrenWithCleanup(true);
    
    MainScene* main = MainScene::getInstance();
    main->initLevelUpLayer();
}

void LevelUpAction::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}