//
//  ResponseFriendsInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#ifndef __CapcomWorld__ResponseFriendsInfo__
#define __CapcomWorld__ResponseFriendsInfo__

#include <iostream>
#include "cocos2d.h"

using namespace cocos2d;

class ResponseFriendsInfo : public cocos2d::CCObject
{
public:
    
    ResponseFriendsInfo();
    ~ResponseFriendsInfo();
    
    const char* res;
    const char* msg;
    
    int myRanking;

    CCArray *friendsList;
    
};

    
#endif /* defined(__CapcomWorld__ResponseFriendsInfo__) */
