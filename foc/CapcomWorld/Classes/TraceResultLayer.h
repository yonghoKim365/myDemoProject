//
//  TraceResultLayer.h
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 19..
//
//

#ifndef __CapcomWorld__TraceResultLayer__
#define __CapcomWorld__TraceResultLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"

class TraceResultLayer : public CCLayer, MyUtil, GameConst
{
public:
    
    void startFight();
    void cbStartFight();
    void startKo();
    void cbRemoveKo();
    
    TraceResultLayer* getInstance()
    {
        return instance;
    }
    
    TraceResultLayer()
    {
        instance = this;
    }
    
    ~TraceResultLayer()
    {
        CC_SAFE_DELETE(frameToSpotlight);
//        CC_SAFE_DELETE(spriteToSpotlight);
        CC_SAFE_DELETE(frameToFadeOut);
//        CC_SAFE_DELETE(spriteToFadeOut);
        CC_SAFE_DELETE(aniFrames);
        
        
        this->removeAllChildrenWithCleanup(true);
    }
//
private:
    
    CCArray* frameToSpotlight;
    CCSprite* spriteToSpotlight;
    CCArray* frameToFadeOut;
    CCSprite* spriteToFadeOut;
    CCArray* aniFrames;
    
    TraceResultLayer* instance;
};

#endif /* defined(__CapcomWorld__TraceResultLayer__) */
