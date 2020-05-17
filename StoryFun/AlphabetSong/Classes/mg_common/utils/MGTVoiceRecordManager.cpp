
#include "MGTVoiceRecordManager.h"


#ifndef AUTO_LOCK
#define AUTO_LOCK(mtx)	std::lock_guard<std::mutex> lock(mtx)
#endif

using namespace cocos2d;
using namespace experimental;

MGTVoiceRecordManager::MGTVoiceRecordManager()
{
    init();
}

MGTVoiceRecordManager::~MGTVoiceRecordManager()
{

}


bool MGTVoiceRecordManager::init()
{
    
    return true;
}

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
void MGTVoiceRecordManager::setRecSession()
{
    MGTVoiceRecorderInterface::getInstance()->setRecSession();
}

bool MGTVoiceRecordManager::getRecSession()
{
    return MGTVoiceRecorderInterface::getInstance()->getRecSession();
}
#endif

void MGTVoiceRecordManager::startVoiceRecording(std::string filePath)
{
    MGTVoiceRecorderInterface::getInstance()->recVoiceRecording(filePath);
}

void MGTVoiceRecordManager::stopVoiceRecording()
{
    MGTVoiceRecorderInterface::getInstance()->stopVoiceRecording();
}

void MGTVoiceRecordManager::pauseVoiceRecording()
{
    MGTVoiceRecorderInterface::getInstance()->pauseVoiceRecording();
}

void MGTVoiceRecordManager::resumeVoiceRecording()
{
    MGTVoiceRecorderInterface::getInstance()->resumeVoiceRecording();
}

void MGTVoiceRecordManager::playRecordedVoice(std::string filePath)
{
    MGTVoiceRecorderInterface::getInstance()->playRecordedVoice(filePath);
}

void MGTVoiceRecordManager::stopRecordedVoice()
{
    MGTVoiceRecorderInterface::getInstance()->stopRecordedVoice();
}

void MGTVoiceRecordManager::pauseRecordedVoice()
{
    MGTVoiceRecorderInterface::getInstance()->pauseRecordedVoice();
}

void MGTVoiceRecordManager::resumeRecordedVoice()
{
    MGTVoiceRecorderInterface::getInstance()->resumeRecordedVoice();
}

