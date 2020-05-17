//
//  EventLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 31..
//
//

#ifndef __CapcomWorld__EventLayer__
#define __CapcomWorld__EventLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "EventListLayer.h"

using namespace cocos2d;

#define EVENT_CELL_HEIGHT (182)

//class EventListLayer;

class EventLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    EventLayer(CCSize layerSize);
    ~EventLayer();
    
    void InitUI();
    
    void PosReInit();

    EventListLayer* pEventlistLayer;

    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    CCSprite* pSprEvent;    
};

#endif /* defined(__CapcomWorld__EventLayer__) */
