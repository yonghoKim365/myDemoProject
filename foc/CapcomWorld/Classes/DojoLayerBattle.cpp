//
//  DojoLayerBattle.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#include "DojoLayerBattle.h"
#include "ARequestSender.h"
#include "MainScene.h"

using namespace cocos2d;

DojoLayerBattle *DojoLayerBattle::instance = NULL;

DojoLayerBattle::DojoLayerBattle(CCSize layerSize)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    instance = this;
    
    this->setContentSize(layerSize);
    
    //ARequestSender::getInstance()->requestOpponent();
    
    CCLOG("DojoLayerBattle::DojoLayerBattle");

    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite, 0);
    
    InitUI();
    
    curLayerTag = 0;
    
    InitSubLayer(curLayerTag);
    
}

DojoLayerBattle::~DojoLayerBattle()
{
    this->removeAllChildrenWithCleanup(true);
}

void DojoLayerBattle::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");

}

void DojoLayerBattle::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
       
    //CCLog("touch moved,x:%f",location.x);
    
}

void DojoLayerBattle::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    /*
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 0, localPoint)){
        CCLog(" button 1");
        ReleaseSubLayer();
        InitSubLayer(0);
        
        InitLayerBtn(0);
    }
    
    if (GetSpriteTouchCheckByTag(this, 1, localPoint)){
        
        CCLog("DojoLayerBattle button 2, block ");
        
        //ReleaseSubLayer();
        //InitSubLayer(1);
        //InitLayerBtn(1);
        
    }
     */
   
}

void DojoLayerBattle::InitUI()
{
    //InitLayerBtn(0);
    
}

void DojoLayerBattle::InitLayerBtn(int selected)
{
    this->removeChildByTag(2, false);
    this->removeChildByTag(3, false);
    
    if (selected==0){
        int yy = 16;
        CCLabelTTF* pLabel1 = CCLabelTTF::create("Duel"  , "Thonburi", 13);
        pLabel1->setColor(COLOR_YELLOW);
        pLabel1->setTag(2);
        registerLabel( this,ccp(0.5,0), accp(170,yy+5), pLabel1, 11);
        
        CCLabelTTF* pLabel2 = CCLabelTTF::create("Ranking"  , "Thonburi", 13);
        pLabel2->setColor(COLOR_WHITE);
        pLabel2->setTag(3);
        registerLabel( this,ccp(0.5,0), accp(476,yy+5), pLabel2, 11);
    }
    else{
        int yy = 16;
        CCLabelTTF* pLabel1 = CCLabelTTF::create("Duel"  , "Thonburi", 13);
        pLabel1->setColor(COLOR_WHITE);
        pLabel1->setTag(2);
        registerLabel( this,ccp(0.5,0), accp(170,yy+5), pLabel1, 11);
        
        CCLabelTTF* pLabel2 = CCLabelTTF::create("Ranking"  , "Thonburi", 13);
        pLabel2->setColor(COLOR_YELLOW);
        pLabel2->setTag(3);
        registerLabel( this,ccp(0.5,0), accp(476,yy+5), pLabel2, 11);
    }
    
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/battle_tab/battle_sub_01_1.png","ui/battle_tab/battle_sub_01_2.png",this,menu_selector(DojoLayerBattle::MenuCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp(8,0));
    pSprBtn1->setTag(0);
    if (selected==0)pSprBtn1->selected();
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/battle_tab/battle_sub_02_1.png","ui/battle_tab/battle_sub_02_2.png",this,menu_selector(DojoLayerBattle::MenuCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp(314,0));
    pSprBtn2->setTag(1);
    if (selected==1)pSprBtn2->selected();
    
    CCMenu* pMenu = CCMenu::create(pSprBtn1, pSprBtn2,NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( accp(0, 16));
    
    pMenu->setTag(99);
    this->addChild(pMenu, 10);
    
    //CheckLayerSize(this);
}

void DojoLayerBattle::HideSubMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition( ccp(0,-10000));
    }
    
    CCLabelTTF* pLabel1 = (CCLabelTTF*)this->getChildByTag(2);
    if(pLabel1)
    {
        pLabel1->setVisible(false);
    }
    
    CCLabelTTF* pLabel2 = (CCLabelTTF*)this->getChildByTag(3);
    if(pLabel2)
    {
        pLabel2->setVisible(false);
    }
}

void DojoLayerBattle::ShowSubMenu()
{
    CCMenu* pMenu = (CCMenu*)this->getChildByTag(99);
    
    if (pMenu != NULL){
        pMenu->setPosition(accp(0, 16));
    }
    
    CCLabelTTF* pLabel1 = (CCLabelTTF*)this->getChildByTag(2);
    if(pLabel1)
    {
        pLabel1->setVisible(true);
    }
    
    CCLabelTTF* pLabel2 = (CCLabelTTF*)this->getChildByTag(3);
    if(pLabel2)
    {
        pLabel2->setVisible(true);
    }

}


void DojoLayerBattle::InitSubLayer(int a)
{
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        //PlayerInfo *pi = PlayerInfo::getInstance();
    
        switch(a){
            case 0:
                //duelBattleLayer = new BattleDuelLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)) );
                duelBattleLayer = new BattleDuelLayer(CCSize(size.width, this->getContentSize().height) );
                duelBattleLayer->setAnchorPoint(ccp(0,0));
                duelBattleLayer->setPosition(accp(0,0));//CARDS_LAYER_BTN_HEIGHT));//MAIN_BTNS_UI_HEIGHT));
                //duelBattleLayer->setTouchEnabled(true);
                this->addChild(duelBattleLayer,20);
                
                break;
            case 1:
                rankingBattleLayer = new BattleRankingLayer(CCSize(size.width, this->getContentSize().height - accp(CARDS_LAYER_BTN_HEIGHT)) );
                rankingBattleLayer->setPosition(accp(0,CARDS_LAYER_BTN_HEIGHT));
                this->addChild(rankingBattleLayer,20);
                rankingBattleLayer->setTouchEnabled(true);
                break;
        }
}

void DojoLayerBattle::ReleaseSubLayer()
{
    if (duelBattleLayer != NULL){
        this->removeChild(duelBattleLayer, true);
        duelBattleLayer = NULL;
    }
    if (rankingBattleLayer != NULL){
        this->removeChild(rankingBattleLayer, true);
        rankingBattleLayer = NULL;
    }
}

void DojoLayerBattle::MenuCallback(CCObject *pSender){
    
    if (MainScene::getInstance()->popupCnt>0)return;
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    
    CCMenu *pMenu = (CCMenu*)this->getChildByTag(99);
    CCMenuItemImage *item1 = (CCMenuItemImage*)pMenu->getChildByTag(0);
    CCMenuItemImage *item2 = (CCMenuItemImage*)pMenu->getChildByTag(1);
    
    item1->unselected();
    item2->unselected();
    
    CCMenuItemImage *item = (CCMenuItemImage *)node;
    item->selected();
    
    if (curLayerTag == tag)return;
    
    curLayerTag = tag;
    
    
    switch(tag){
        case 0:
            CCLog("DojoLayerBattle button 1");
            ReleaseSubLayer();
            InitSubLayer(0);
            InitLayerBtn(0);
//            selected_team = 0;
            break;
        case 1:
            //item->selected();
//            selected_team = 1;
            
            CCLog("DojoLayerBattle button 2, block ");
            
            /*
             ReleaseSubLayer();
             InitSubLayer(1);
             InitLayerBtn(1);
             */
            break;
    }
    
    
}