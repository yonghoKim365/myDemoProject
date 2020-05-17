//
//  CardDetailViewLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 18..
//
//

#ifndef __CapcomWorld__CardDetailViewLayer__
#define __CapcomWorld__CardDetailViewLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "DeckListLayer.h"
//#include "CellLayerDelegate.h"
#include "TeamEditLayer.h"
#include "CardListLayer.h"
#include "CardListCellBtnDelegate.h"
#include "DetailViewCloseDelegate.h"

USING_NS_CC;

class CardDetailViewLayer : public cocos2d::CCLayer, MyUtil, GameConst, CardListCellBtnDelegate
{
    
public:
    CardDetailViewLayer(CCSize layerSize, CardInfo *_card, DetailViewCloseDelegate *_delegate, int _directionType = DIRECTION_NONE);
    ~CardDetailViewLayer();
    
    void InitLayer();
    
    virtual void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void MenuCallback(CCObject* pSender);
    
    CardInfo *card;
    
    DetailViewCloseDelegate *delegate;
    
    void regNumber(int num, CCPoint pos, int rareLv);
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();

    int directionType;
    //virtual void registerWithTouchDispatcher();
    //virtual void unregisterScriptTouchHandler();
    
    void setKeyBlock(bool a);
    
    CCPoint touchStartLocation;
    CCArray* aniFrames;
};

#endif /* defined(__CapcomWorld__CardDetailViewLayer__) */
