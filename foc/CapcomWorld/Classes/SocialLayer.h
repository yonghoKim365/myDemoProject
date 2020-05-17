//
//  SocialLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#ifndef __CapcomWorld__SocialLayer__
#define __CapcomWorld__SocialLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "SocialListLayer.h"
#include "SocialInviteLayer.h"

using namespace cocos2d;

class SocialLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    SocialLayer(CCSize layerSize);
    ~SocialLayer();
    
    static SocialLayer* getInstance();
    
    void InitUI();
    
    void InitSocialListLayer();
    void InitInviteLayer();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    SocialListLayer* pSocialListlayer;
    SocialInviteLayer* pInviteLayer;
    
private:
    
    static SocialLayer* instance;
    
    bool touchMoved;
   
};


#endif /* defined(__CapcomWorld__SocialLayer__) */
