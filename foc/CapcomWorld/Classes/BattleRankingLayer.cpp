//
//  BattleRankingLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#include "BattleRankingLayer.h"


BattleRankingLayer::BattleRankingLayer(CCSize layerSize)
{
    this->setContentSize(layerSize);
    InitUI();
}


BattleRankingLayer::~BattleRankingLayer()
{
    
}

void BattleRankingLayer::InitUI()
{
    this->removeAllChildrenWithCleanup(true);
    
    CCSprite* pSpr_Back = CCSprite::create("ui/battle_tab/battle_duel_bg.png");
    pSpr_Back->setAnchorPoint(ccp(0,0));
    pSpr_Back->setPosition( ccp(0,0) );
    regSprite(this,  ccp(0,0), accp(0,0), pSpr_Back, 10);
    
    CCSize size = this->getContentSize();
    
    CCSprite* pSpr_Friend = CCSprite::create("ui/battle_tab/battle_ranking_tab_a1.png");
    pSpr_Friend->setAnchorPoint(ccp(0,0));
    pSpr_Friend->setPosition( ccp(0,0) );
    pSpr_Friend->setTag(0);
    regSprite(this,  ccp(0,0), accp(10, 600), pSpr_Friend, 10);
    
    CCSprite* pSpr_World = CCSprite::create("ui/battle_tab/battle_ranking_tab_b1.png");
    pSpr_World->setAnchorPoint(ccp(0,0));
    pSpr_World->setPosition( ccp(0,0) );
    pSpr_World->setTag(1);
    regSprite(this,  ccp(0,0), accp(320, 600), pSpr_World, 10);
    
}

void BattleRankingLayer::ccTouchesBegan(cocos2d::CCSet *touches, cocos2d::CCEvent* event)
{
    
    
}
