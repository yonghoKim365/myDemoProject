//
//  GiftInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 30..
//
//

#ifndef CapcomWorld_GiftInfo_h
#define CapcomWorld_GiftInfo_h


#include "cocos2d.h"
using namespace std;

class GiftInfo : public cocos2d::CCObject
{
public:
    int giftID;
    int giftSrl;
    int count;
    int receiveDate;
    const char* nick;
    int from;
};

#endif
