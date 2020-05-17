//
//  FriendsInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#ifndef __CapcomWorld__FriendsInfo__
#define __CapcomWorld__FriendsInfo__

#include <iostream>
#include "cocos2d.h"

class FriendsInfo : public cocos2d::CCObject
{
    
public:
    FriendsInfo(){
        
    }
    ~FriendsInfo(){
        
    }
    long long userID;
    int level;
    int ranking;
    int leaderCard;
    int attack;
    int defense;
    std::string nickname;
    std::string profileURL;
    
};

#endif /* defined(__CapcomWorld__FriendsInfo__) */
