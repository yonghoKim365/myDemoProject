//
//  EventDetailLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#ifndef __CapcomWorld__EventDetailLayer__
#define __CapcomWorld__EventDetailLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "EventLayer.h"
#include "EventListLayer.h"
#include "EventInfo.h"

class EventDetaillayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    EventDetaillayer(CCSize layerSize, EventInfo* event);
    ~EventDetaillayer();
    
    void InitUI();
    
    const char* adjust(const char *input)
    {
        std::string text = input;
        do {
            int pos = text.find("\\n");
            if (pos >= text.length())
                return text.c_str();
            text = text.replace(pos, 2, "\n");
        } while (1);
        return text.c_str();
    }

    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;

    EventInfo* myEvent;
    
    EventListLayer* pEventlistLayer;

};

#endif /* defined(__CapcomWorld__EventDetailLayer__) */
