#include "ScreenManager.h"
#include "Contents/MyBookResources.h"

USING_NS_CC;

// singleton
ScreenManager* ScreenManager::_screenManager = nullptr;

#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#include "platform/android/jni/JniHelper.h"
#include <jni.h>

#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C" {
#endif
	void Java_org_cocos2dx_cpp_AppActivity_infromScreenManager(JNIEnv* env, jobject thiz)
	{
		ScreenManager::infromScreenManager();
	}

#ifdef __cplusplus
}
#endif

#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

// singleton
ScreenManager* ScreenManager::getInstance()
{
	if (_screenManager == nullptr)
	{
		_screenManager = new ScreenManager();
	}

	return _screenManager;
}


bool ScreenManager::init()
{
	CCLOG("ScreenManager init...");

	return true;
}


ScreenManager::ScreenManager()
{
	CCLOG("_screenManager create...");	
}


ScreenManager::~ScreenManager()
{
	free(_screenManager);
}

void ScreenManager::scheduleForScreenOff(float delta)
{
	Device::setKeepScreenOn(false);
	CCLOG("scheduleForScreenOff ........");
	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	/*
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "screenOff", "()V")) {
		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
	*/
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}

void ScreenManager::screenOn(Node* parent)
{	
	CCLOG("screenOn ........");
	Device::setKeepScreenOn(true);
	//mParent = parent;
	//CC_ASSERT(mParent != nullptr);
	//mParent->unschedule(schedule_selector(ScreenManager::scheduleForScreenOff));
	//mParent->scheduleOnce(schedule_selector(ScreenManager::scheduleForScreenOff), SCREEN_LOCK_TIME);	

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	/*
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "recordingVoice", "(II)V")) {		
		jint param1 = week;
		jint param2 = ord;
		info.env->CallStaticVoidMethod(info.classID, info.methodID, param1, param2);
		info.env->DeleteLocalRef(info.classID);
	}
	*/
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


void ScreenManager::screenOff(Node* parent)
{	
	Device::setKeepScreenOn(false);
	// 화면 잠그기
	CCLOG("ScreenManager screenOff.....");
}


// JNI CALL
void ScreenManager::infromScreenManager()
{
	CCLOG("infromScreenManager...............");
}

