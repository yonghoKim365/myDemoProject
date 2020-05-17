//
//  BattleListLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 19..
//
//

#ifndef __CapcomWorld__BattleListLayer__
#define __CapcomWorld__BattleListLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "BattlePlayerCell.h"
#include "BattleFullScreen.h"

using namespace cocos2d;

class BattleListLayer : public cocos2d::CCLayer, MyUtil, GameConst, BattlePlayerCellButtonDelegate
{
public:
    
    BattleListLayer(CCSize layerSize);
    ~BattleListLayer();
    
    void InitUI();
        
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
//    void SubUICallback(CCObject* pSender);
    
    void InitLayer();
    
    void ButtonBattle(UserInfo *_user);

/*
    int nBattleStep = 0;
    
    ACardMaker *cardMaker;
    
    CardInfo *cards[5];
    
    BattleFullScreen* battleLayer;
*/
    ////////////////
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    cocos2d::CCPoint touchStartPoint;
    bool bTouchPressed;
    float yStart;
    float contentClippingH;
    void scrollingEnd();
    CCRect clipRect;
    
    virtual void visit();
    /*
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    
	virtual void preVisitWithClippingRect(CCRect clipRect);
	virtual void postVisit();
    */
    
};


#endif /* defined(__CapcomWorld__BattleListLayer__) */
