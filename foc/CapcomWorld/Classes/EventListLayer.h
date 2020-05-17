//
//  EventListLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 31..
//
//

#ifndef __CapcomWorld__EventListLayer__
#define __CapcomWorld__EventListLayer__

#include <iostream>

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
//#include "EventDetailLayer.h"

using namespace cocos2d;
using namespace std;

typedef struct : public cocos2d::CCObject
{
    std::string ImgPath;
    std::string EventMsg;
}tempEvent;

class EventDetaillayer;

class EventListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    EventListLayer(CCSize layerSize);
    ~EventListLayer();
    
    void InitLogData();
    void InitUI();
    void InitLayerSize(CCSize layerSize);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    
    void MakeEventCell(const string& EventImg, int tag);
    
    int GetNumOfLog() const;
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
	
    float LayerStartPos;

private:
    //CCSprite* pSprEvent;
    CCArray* Event;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    int StartYpos;
    int NumOfEvent;
    
    int EndYPos;
    
    float yStart;
    
    EventDetaillayer* pEventDetailLayer;
};


#endif /* defined(__CapcomWorld__EventListLayer__) */
