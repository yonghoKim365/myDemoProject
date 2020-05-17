

#ifndef __MGTVoiceRecordManager_h__
#define __MGTVoiceRecordManager_h__


#include "SingletonBase.h"
#include "cocos2d.h"
#include "MGTDefines.h"
#include "MGTUtils.h"
#include "MGTVoiceRecorderInterface.h"

USING_NS_CC;


class MGTVoiceRecordManager : public CSingletonBase<MGTVoiceRecordManager>
{
public:
    MGTVoiceRecordManager();
    ~MGTVoiceRecordManager();
	bool init();

    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    void setRecSession();
    bool getRecSession();
#endif
    void startVoiceRecording(std::string filePath = "");
    void stopVoiceRecording();
    void pauseVoiceRecording();
    void resumeVoiceRecording();
    
    
    void playRecordedVoice(std::string filePath = "");
    void stopRecordedVoice();
    void pauseRecordedVoice();
    void resumeRecordedVoice();
    

};


#endif
