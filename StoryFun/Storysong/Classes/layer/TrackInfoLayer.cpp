#include "TrackInfoLayer.h"
#include "audio/include/AudioEngine.h"
#include "data/PlayInfo.h"
#include "data/JsonInfo.h"
#include "common/Utils.h"

using namespace experimental;

SongTickManager::SongTickManager() {}
SongTickManager::~SongTickManager(){}

const float PAUSE_DELAY = 0.5f;

SongTickManager * SongTickManager::create(std::vector<float> tickList, SongTickEventListener * listener)
{
	SongTickManager *ref = new SongTickManager();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->setTickEventListener(listener);
		ref->initData(tickList);
		return ref;
	}
	else
	{
		CC_SAFE_DELETE(ref);
		return nullptr;
	}
}


void SongTickManager::setTickEventListener(SongTickEventListener * listener)
{
	_listener = listener;
}

void SongTickManager::initData(std::vector<float> tickList)
{
	EventListenerCustom* onPause = EventListenerCustom::create("ON_PAUSE", CC_CALLBACK_0(SongTickManager::pauseUpdate, this));
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(onPause, this);

	EventListenerCustom* onResume = EventListenerCustom::create("ON_RESUME", CC_CALLBACK_0(SongTickManager::resumeUpdate, this));
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(onResume, this);

	_tickList = tickList;
	initData();
}

void SongTickManager::initData()
{
	isScheduleRunning = false;
	_tickEventCount = 0;
	_playTime = 0;
	_runningTime = 0;
}

void SongTickManager::pauseUpdate()
{
	if (bgmID != AudioEngine::INVALID_AUDIO_ID)
	{
		AudioEngine::pause(bgmID);
	}

	unscheduleUpdate();

}
void SongTickManager::stop()
{
	if (bgmID != AudioEngine::INVALID_AUDIO_ID)
	{
		AudioEngine::stop(bgmID);
	}

	unscheduleUpdate();
}

void SongTickManager::resumeUpdate()
{
	if (bgmID != AudioEngine::INVALID_AUDIO_ID)
	{
		AudioEngine::setCurrentTime(bgmID, _playTime);
		AudioEngine::resume(bgmID);
	}

	scheduleUpdate();
}


void SongTickManager::start(std::string bgmPath)
{
	if (isScheduleRunning)
	{
		unscheduleUpdate();
	}
	
	bgmID = AudioEngine::play2d(bgmPath.c_str());
	startBGM();
	PlayInfo::create()->setBGMID(bgmID);
	_totlaTime = AudioEngine::getDuration(bgmID);
	AudioEngine::setFinishCallback(bgmID, CC_CALLBACK_2(SongTickManager::onFinishCallBack, this));

	initData();
	scheduleUpdate();
	isScheduleRunning = true;
}

void SongTickManager::startBGM()
{
	Device::setKeepScreenOn(true);
	/*
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t
		, "net/minigate/smartdoodle/storyfun/viewer/song/AppActivity"
		, "StartBGM"
		, "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
	}
#endif
	*/
}

void SongTickManager::stopBGM()
{
	Device::setKeepScreenOn(false);
	/*
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t
		, "net/minigate/smartdoodle/storyfun/viewer/song/AppActivity"
		, "StopBGM"
		, "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
	}
#endif
	*/
}

void SongTickManager::update(float dt)
{
	if (AudioEngine::getState(bgmID) == AudioEngine::AudioState::INITIALZING)
	{
		return;
	}

	_playTime += dt;
	_runningTime += dt;

	if (_playTime > AudioEngine::getCurrentTime(bgmID))
	{
		AudioEngine::setVolume(bgmID, 0);
		float targetSecond = _tickList.at(_tickEventCount);
	
		_playTime -= dt;
		if (_runningTime > targetSecond)
		{
			unscheduleUpdate();
			isScheduleRunning = false;
			//_listener->onFinishedEvent();
		}
		else
		{
			return;
		}
		
	}
	else
	{
		if (AudioEngine::getVolume(bgmID) == 0)
		{
			AudioEngine::setVolume(bgmID, 1);
		}
	}

	if (_playTime > AudioEngine::getCurrentTime(bgmID) )
	{
		_playTime = AudioEngine::getCurrentTime(bgmID);
		_runningTime = AudioEngine::getCurrentTime(bgmID);
	}

	if (_tickEventCount < _tickList.size())
	{
		float targetSecond = _tickList.at(_tickEventCount);
		
		if (_playTime > targetSecond)
		{
			if (_tickEventCount >= (_tickList.size() - 1))
			{
				unscheduleUpdate();
				isScheduleRunning = false;
				
			}
			else
			{
				_listener->onTickEvent(_tickEventCount);
				_tickEventCount++;
			}
		}
	}
	
}

void SongTickManager::onFinishCallBack(int audioID, std::string filePath)
{
	unscheduleUpdate();
	_listener->onFinishedEvent();
	stopBGM();
}