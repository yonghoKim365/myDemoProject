//
//  ShakeAction.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 14..
//
//

#ifndef __CapcomWorld__ShakeAction__
#define __CapcomWorld__ShakeAction__

#include "actions/CCActionInterval.h"
#include "MyUtil.h"

class CCShake : public cocos2d::CCActionInterval, MyUtil
{
    // Code by Francois Guibert
    // Contact: www.frozax.com - http://twitter.com/frozax - www.facebook.com/frozax
public:
    CCShake();
    
    // Create the action with a time and a strength (same in x and y)
    static CCShake* create(float d, float strength, float x, float y);
    // Create the action with a time and strengths (different in x and y)
    static CCShake* createWithStrength(float d, float strength_x, float strength_y, float x, float y);
    bool initWithDuration(float d, float strength_x, float strength_y, float x, float y );
    
protected:
    
    void startWithTarget(cocos2d::CCNode *pTarget);
    void update(float time);
    void stop(void);
    
    
    // Initial position of the shaked node
    float m_initial_x, m_initial_y;
    // Strength of the action
    float m_strength_x, m_strength_y;
};

#endif /* defined(__CapcomWorld__ShakeAction__) */
