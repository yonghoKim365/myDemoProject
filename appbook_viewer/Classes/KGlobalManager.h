#pragma once

#include <string>
#include <map>
using namespace std;
#include "cocos2d.h"

USING_NS_CC;

class KGlobalManager {
public:
    KGlobalManager();
    ~KGlobalManager();

    int mAudioEngineID;

public:
    static KGlobalManager* getInstance();
    static void removeInstance();
    void setAutoPlay(bool aGoNext = true);
	bool getAutoPlay();
    void setNarration(bool aGoNext);
    bool getNarration();
    void setCurrentAudio(int nAudioEngineID);
    int getCurrentAudio();

private:
    static KGlobalManager * _instance;
    bool bAutoPlay;
    bool shouldNarrationType;
};


