//
//  EventInfo.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 12. 10..
//
//

#ifndef __CapcomWorld__EventInfo__
#define __CapcomWorld__EventInfo__

#include <iostream>
#include "cocos2d.h"

using namespace std;

class EventInfo : public cocos2d::CCObject
{
public:
    string eventBanner;
    string eventDetail;
    string eventDescription;
};


#endif /* defined(__CapcomWorld__EventInfo__) */
