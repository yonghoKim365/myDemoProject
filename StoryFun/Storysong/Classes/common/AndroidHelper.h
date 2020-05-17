#ifndef _ANDROID_HELPER_H_
#define _ANDROID_HELPER_H_

#include <string>
#include "cocos2d.h"
#include "data/JsonInfo.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif // CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID

USING_NS_CC;

class AndroidHelper
{
public:
	static void showProgress()
	{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t
			, "org/cocos2dx/cpp/AppActivity"
			, "showProgress"
			, "()V"))
		{
			t.env->CallStaticVoidMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
			//currentWeek = editionSeq;
		}

		CCLOG("showProgress()");
#endif
	}


	static void hideProgress()
	{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t
			, "org/cocos2dx/cpp/AppActivity"
			, "hideProgress"
			, "()V"))
		{
			t.env->CallStaticVoidMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
			//currentWeek = editionSeq;
		}

		CCLOG("hideProgress()");
#endif
	}

	static void getCurrentWeek()
	{
		int currentWeek = 22;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t
			, "org/cocos2dx/cpp/AppActivity"
			, "getEditionName"
			, "()I"))
		{
			jint editionSeq = t.env->CallStaticIntMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
			currentWeek = editionSeq;
		}

#endif
		JsonInfo::create()->setCurrentWeek(currentWeek); 
	}

	static void getStartStep()
	{
		int startStep = 1;

#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t
			, "org/cocos2dx/cpp/AppActivity"
			, "getStartStep"
			, "()I"))
		{
			jint editionSeq = t.env->CallStaticIntMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
			startStep = editionSeq;
		}
#endif
		CCLOG("getStartStep = start Step : %d", startStep);
		JsonInfo::create()->setStartStep(startStep);
		
	}

	static void getDebugMode()
	{
		int mode = 0;

#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t
			, "org/cocos2dx/cpp/AppActivity"
			, "getdebugMode"
			, "()I"))
		{
			jint editionSeq = t.env->CallStaticIntMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
			mode = editionSeq;
		}
#endif
		if (mode == 0)
			JsonInfo::create()->isDebugMode = false;
		else
			JsonInfo::create()->isDebugMode = true;
	}

	static void exitGame()
	{
		
#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		JniMethodInfo t;

		if (JniHelper::getStaticMethodInfo(t
			, "net/minigate/smartdoodle/storyfun/viewer/song/AppActivity"
			, "exitGame"
			, "()V"))
		{

			t.env->CallStaticVoidMethod(t.classID, t.methodID);
			t.env->DeleteLocalRef(t.classID);
		}
#else
		Director::getInstance()->end();
#endif
	}

	static void setContentPath()
	{
		std::string contentPath = "C:/develop/yoons/yoons_contents";

		FileUtils::getInstance()->addSearchPath(contentPath.c_str());
	}
};
#endif
