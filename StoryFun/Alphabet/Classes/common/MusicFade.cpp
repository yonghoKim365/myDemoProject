//
//  MusicFade.cpp
//
//   on 1/30/14.
//
//

#include "MusicFade.h"
#include "SimpleAudioEngine.h"

using namespace cocos2d;
using namespace CocosDenshion;

MusicFade::MusicFade()
{
    m_initialVal = 0;
    m_targetVal = 0;
}

MusicFade* MusicFade::create(float duration, float volume, bool pauseOnComplete)
{
    MusicFade *pAction = new MusicFade();
    pAction->initWithDuration(duration, volume, pauseOnComplete);
    pAction->autorelease();

    return pAction;
}

bool MusicFade::initWithDuration(float duration, float volume, bool pauseOnComplete)
{
    if (ActionInterval::initWithDuration(duration))
    {
        m_targetVal = volume;
        m_bPauseOnComplete = pauseOnComplete;
        return true;
    }

    return false;
}

void MusicFade::update(float time)
{
    float vol = m_initialVal + time*(m_targetVal - m_initialVal);
    SimpleAudioEngine::getInstance()->setBackgroundMusicVolume(vol);

}

void MusicFade::startWithTarget(Node *pTarget)
{
    ActionInterval::startWithTarget( pTarget );
    m_initialVal = SimpleAudioEngine::getInstance()->getBackgroundMusicVolume();
}

void MusicFade::stop(void)
{
    if(m_bPauseOnComplete) SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
    ActionInterval::stop();
}
