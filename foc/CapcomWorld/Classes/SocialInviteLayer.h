//
//  SocialInviteLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 14..
//
//

#ifndef __CapcomWorld__SocialInviteLayer__
#define __CapcomWorld__SocialInviteLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "SocialListLayer.h"
#include "XBridge.h"

using namespace cocos2d;

class SocialInviteLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    SocialInviteLayer(CCSize layerSize);
    ~SocialInviteLayer();
    
    void InitUI();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    SocialListLayer* pSocialListlayer;
    void InitFriendListLayer();
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    
    //virtual void visit();
    
    static const int HELP_TEXT_ZONE_HEIGHT = 90;
    
    static SocialInviteLayer *instance;
    
    static SocialInviteLayer *getInstance()
    {
        if (instance == NULL)
            printf("SocialInviteLayer instance is NULL\n");
        return instance;
    }
    
private:
    
    bool touchMoved;
    
    int nLastTouchedTag;
};


#endif /* defined(__CapcomWorld__SocialInviteLayer__) */
