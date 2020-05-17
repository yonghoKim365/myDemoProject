//
//  DeckListLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 16..
//
//

#include <math.h>
#include "DeckListLayer.h"
#include "PlayerInfo.h"
#include "CardDeckLayer.h"

using namespace cocos2d;

DeckListLayer *DeckListLayer::instance = NULL;

DeckListLayer::DeckListLayer(int team, CCSize layerSize, CellLayerDelegate *_dele){
    //cardDeckArray = _cardDeckArray;
    setDelegate(_dele);
    this->setContentSize(layerSize);
    nTeam = team;
    InitUI(team);
    instance = this;
    
    this->setTouchEnabled(true);
    this->setClipsToBounds(true);
    
    moving = false;
}

DeckListLayer::~DeckListLayer(){
	
    CCDirector::sharedDirector()->getTouchDispatcher()->removeDelegate(this);
	this->removeAllChildrenWithCleanup(true);
}

void DeckListLayer::InitUI(int team)
{
 
    /*
     CCSprite* pSprTest     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
     regSprite(this, ccp(0,0), accp(10,0), pSprTest, 100);
     CCSprite* pSprTest2     = CCSprite::create("ui/home/ui_menu_sub_01_2.png");
     CCSize s = this->getContentSize();
     regSprite(this, ccp(0,1), ccp(0,s.height), pSprTest2, 100);
     */
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    
    /*
    CardInfo* aaa[5];
    aaa[0] = new CardInfo();
    aaa[1] = new CardInfo();
    aaa[2] = new CardInfo();
    aaa[3] = new CardInfo();
    aaa[4] = new CardInfo();
    */
    
    
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCPoint *posA = new CCPoint(0, MAIN_LAYER_BTN_HEIGHT/2 + CARDS_LAYER_BTN_HEIGHT/2);
    CCPoint *posB = new CCPoint(0, size.height - accp(MAIN_LAYER_TOP_UI_HEIGHT) - accp(MAIN_LAYER_TOP_MARGIN) - accp(CARD_DECK_TOP_UI_SPACE_2)+accp(CARD_DECK_TOP_UI_SPACE_3));
    posA->autorelease();
    posB->autorelease();
    if (team == 0){
        
        int yy = 0;
        AttackDeckCell *cell1 = new AttackDeckCell(pi->getCardDeck(0), 0, 0, "");
        cell1->SetTouchArea(posA, posB);
        cell1->setPosition(ccp(0,yy));//(cell1->getContentSize().height)*3 + 10));
        cell1->setTouchEnabled(true);
        this->addChild(cell1,20);
        
        cell1->setDelegate(getDelegate());
        
        // 팀1만 남기고 팀 2,3,4 삭제. 2013-02-25 김용호
        
        /*
        AttackDeckCell *cell4 = new AttackDeckCell(pi->getCardDeck(3), 0, 3,"");
        cell4->SetTouchArea(posA, posB);
        cell4->setPosition(ccp(0,0));
        cell4->setTouchEnabled(true);
        this->addChild(cell4,20);
        
        yy+= cell4->getContentSize().height + accp(CardDeckLayer::CARD_DECK_SELL_SPACE);
        
        AttackDeckCell *cell3 = new AttackDeckCell(pi->getCardDeck(2), 0, 2,"");
        cell3->SetTouchArea(posA, posB);
        cell3->setPosition(ccp(0,yy));//(cell1->getContentSize().height+5)));
        cell3->setTouchEnabled(true);
        this->addChild(cell3,20);
        
        yy+= cell4->getContentSize().height + accp(CardDeckLayer::CARD_DECK_SELL_SPACE);
        
        AttackDeckCell *cell2 = new AttackDeckCell(pi->getCardDeck(1), 0, 1,"");
        cell2->SetTouchArea(posA, posB);
        cell2->setPosition(ccp(0,yy));//(cell1->getContentSize().height)*2 + 10));
        cell2->setTouchEnabled(true);
        this->addChild(cell2,20);
        
        yy+= cell4->getContentSize().height + accp(CardDeckLayer::CARD_DECK_SELL_SPACE);
        
        AttackDeckCell *cell1 = new AttackDeckCell(pi->getCardDeck(0), 0, 0, "");
        cell1->SetTouchArea(posA, posB);
        cell1->setPosition(ccp(0,yy));//(cell1->getContentSize().height)*3 + 10));
        cell1->setTouchEnabled(true);
        this->addChild(cell1,20);
        
        cell1->setDelegate(getDelegate());
        cell2->setDelegate(getDelegate());
        cell3->setDelegate(getDelegate());
        cell4->setDelegate(getDelegate());
         */
    }
    else{
        AttackDeckCell *cell4 = new AttackDeckCell(pi->getCardDeck(4), 1, 0, "");
        cell4->SetTouchArea(posA, posB);
        cell4->setPosition(ccp(0,1));
        cell4->setTouchEnabled(true);
        this->addChild(cell4,20);
        
        cell4->setDelegate(getDelegate());
        
    }
}

void DeckListLayer::MenuCallback(cocos2d::CCObject *pSender)
{
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    switch(tag){
        case 0:
            break;
        case 1:
            break;
        case 2:
            break;
        case 3:
            break;
        case 4:
            break;
    }
}

void DeckListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    /*
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    startPosition = location;
    moving = false;
     */
}

void DeckListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    /*
    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //float a = this->getContentSize().height;
    float y = this->getPositionY();
    
    //CCLog("DeckListLayer layer y:%f, starty:%f",y,yStart);
    
    //float yy = this->getPositionY() + this->getContentSize().height;
    
    //CCLog("DeckListLayer layer y:%f, y+h:%f",y,yy);
    
    if (nTeam == 0){

        if (y < 0){
            if (y < yStart){
                //CCActionInterval *action = CCActionInterval::initWithDuration(0.3);
                //CCActionEase *move = CCActionEase::initWithAction(cocos2d::CCActionInterval *pAction)
                
                
                
                CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(5,this->yStart)), 3);
                CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(DeckListLayer::scrollingEnd));
                this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
            }
        }
        else if (y > 0){

            CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(5,0)), 3);
            CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(DeckListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    else if (nTeam == 1){
        CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(5,this->yStart)), 3);
        CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(DeckListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }

    //CCLog(" sublayer y =%f, height=%f",y,a);
     */
}

void DeckListLayer::scrollingEnd()
{
    this->stopAllActions();
	//this->setIsScrolling(false);
}

void DeckListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    /*
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    int delta = fabs(location.y - startPosition.y);
    if (delta > 4)
        moving = true;
    if (bTouchPressed){
        if (touchStartPoint.y != location.y){
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
            //CCLog("deckListLayer.y:%f",this->getPositionY());
        }

    }
     */
}


void DeckListLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT);
    int clip_h = winSize.height - accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN + CARD_DECK_TOP_UI_SPACE_2+CARD_DECK_TOP_UI_SPACE_3);
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, this->getContentSize().width, clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}

void DeckListLayer::preVisitWithClippingRect(CCRect clipRect)
{
    if (!this->isVisible())// getIsVisible())
        return;
	
    glEnable(GL_SCISSOR_TEST);
	
    //CCDirector *director = CCDirector::sharedDirector();
    CCSize size = GameConst::WIN_SIZE;// director->getWinSize();
    CCPoint origin = this->convertToWorldSpaceAR(clipRect.origin);
    CCPoint topRight =this->convertToWorldSpaceAR(ccpAdd(clipRect.origin, ccp(clipRect.size.width, clipRect.size.height)));
    CCRect scissorRect = CCRectMake(origin.x, origin.y, topRight.x-origin.x, topRight.y-origin.y);
	
    // Handle Retina
    scissorRect = CC_RECT_POINTS_TO_PIXELS(scissorRect);
	
    glScissor((GLint) scissorRect.origin.x, (GLint) scissorRect.origin.y,
              (GLint) scissorRect.size.width, (GLint) scissorRect.size.height);
}

void DeckListLayer::postVisit()
{
    glDisable(GL_SCISSOR_TEST);
}