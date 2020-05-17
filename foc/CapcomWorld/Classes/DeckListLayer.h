//
//  DeckListLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 16..
//
//

#ifndef __CapcomWorld__DeckListLayer__
#define __CapcomWorld__DeckListLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "CellLayerDelegate.h"
#include "GameConst.h"

USING_NS_CC;

class DeckListLayer : public cocos2d::CCLayer, MyUtil, GameConst //, public CCScrollLayerExt,public CCScrollLayerExtDelegate
{
    
public:
    //virtual bool init();
    
    static DeckListLayer *instance;
    static DeckListLayer *getInstance()
    {
        return instance;
    }
    
    DeckListLayer(int team, CCSize LayerSize, CellLayerDelegate *_dele);
    ~DeckListLayer();
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(CardDeckLayer);
    
    void InitUI(int team);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    cocos2d::CCPoint touchStartPoint;
    bool bTouchPressed;
    
    void MenuCallback(CCObject* pSender);
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
	virtual void preVisitWithClippingRect(CCRect clipRect);
	virtual void postVisit();
    
    CC_SYNTHESIZE(CellLayerDelegate *,delegate,Delegate);
    
    float yStart;
    int     nTeam; // 0 == attack, 1 == defence
    float contentClippingH;
    
    CCArray *cardDeckArray;
    CCPoint startPosition, endPosition;
    bool moving;
    
    void scrollingEnd();
        
};
#endif /* defined(__CapcomWorld__DeckListLayer__) */
