//
//  MusicFade.h
//
//   on 1/30/14.
//  
//

#ifndef __MusicFade__
#define __MusicFade__


#include "cocos2d.h"


class MusicFade : public cocos2d::ActionInterval
{
public:
    MusicFade();
    
    static MusicFade* create(float d, float volume, bool pauseOnComplete = false );
    bool initWithDuration(float d, float volume, bool pauseOnComplete );
    
    virtual void startWithTarget(cocos2d::Node *pTarget);
    virtual void update(float time);
    virtual void stop(void);
    
protected:
    float m_targetVal;
    float m_initialVal;
    bool m_bPauseOnComplete;
};


#endif /* defined(__MusicFade__) */
