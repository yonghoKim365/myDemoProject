#include "VoiceRecordManager.h"
#include "Layer/RecordLayer.h"

USING_NS_CC;

// singleton
VoiceRecordManager* VoiceRecordManager::_voiceRecordManager = nullptr;

#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#include "platform/android/jni/JniHelper.h"
#include <jni.h>

#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C" {
#endif
	void Java_org_cocos2dx_cpp_AppActivity_informNativeVoiceRecord(JNIEnv* env, jobject thiz)
	{
		VoiceRecordManager::infromVoiceRecord();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVoiceRecordCancel(JNIEnv* env, jobject thiz)
	{
		VoiceRecordManager::stop();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVoiceRecordStop(JNIEnv* env, jobject thiz)
	{
		VoiceRecordManager::infromVoiceRecordStop();
	}
#ifdef __cplusplus
}
#endif

#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

// singleton
VoiceRecordManager* VoiceRecordManager::getInstance()
{
	if (_voiceRecordManager == nullptr)
	{
		_voiceRecordManager = new VoiceRecordManager();
	}

	return _voiceRecordManager;
}


bool VoiceRecordManager::init()
{
	CCLOG("VoiceRecordManager init...");

	return true;
}


VoiceRecordManager::VoiceRecordManager()
{
	CCLOG("VoiceRecordManager create...");
	mIsRecording = false;
}


VoiceRecordManager::~VoiceRecordManager()
{
	free(_voiceRecordManager);
}

// 녹음 시작
void VoiceRecordManager::start(Node* parent, int week, int ord)
{
	VoiceRecordManager::getInstance()->mIsRecording = true;
	CCLOG("VOICERECORD START.....%d, %d", week, ord);
	
	VoiceRecordManager::getInstance()->mParent = parent;

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "recordingVoice", "(II)V")) {
		//jstring strParam = info.env->NewStringUTF(str.c_str());
		jint param1 = week;
		jint param2 = ord;
		info.env->CallStaticVoidMethod(info.classID, info.methodID, param1, param2);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


// 중지 
void VoiceRecordManager::doneStop(Node* parent)
{
	//mIsRecording = false;
	CCLOG("VOICERECORD DONE..STOP.....");
	VoiceRecordManager::getInstance()->mParent = parent;

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "stopRecordingVoice", "()V")) {

		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

	VoiceRecordManager::getInstance()->mIsRecording = false;
	VoiceRecordManager::getInstance()->mMode = 1; // done 체크

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	infromVoiceRecordStop();
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
}


// 중지 
void VoiceRecordManager::stop(Node* parent)
{
	//mIsRecording = false;
	CCLOG("VOICERECORD STOP.....PARENT.....");
	VoiceRecordManager::getInstance()->mParent = parent;

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "stopRecordingVoice", "()V")) {

		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

	VoiceRecordManager::getInstance()->mIsRecording = false;

}


// 중지 
void VoiceRecordManager::stop()
{
	CCLOG("VOICERECORD STOP.....");	

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "stopRecordingVoice", "()V")) {

		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

	//VoiceRecordManager::getInstance()->mIsRecording = false;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	infromVoiceRecordStop();
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
}


// 플레이 
void VoiceRecordManager::play()
{
	//mIsRecording = false;
	CCLOG("VOICERECORD PLAY.....");

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "playRecordingVoice", "()V")) {

		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


bool VoiceRecordManager::isRecording()
{
	return mIsRecording;
}


// JNI CALL 성공
void VoiceRecordManager::infromVoiceRecord()
{
	CCLOG("infromVoiceRecord...............");
	auto mgr = VoiceRecordManager::getInstance();
	// 녹음중 처리
	((RecordLayer*)(mgr->mParent))->processJniStartSuccess();
}


// JNI CALL 성공(Stop 성공)
void VoiceRecordManager::infromVoiceRecordStop()
{
	CCLOG("infromVoiceRecordStop...............: %d, MODE : %d", 
		VoiceRecordManager::getInstance()->mIsRecording,
		VoiceRecordManager::getInstance()->mMode);
	auto mgr = VoiceRecordManager::getInstance();	
	// 녹음중 처리
	if (mgr->mMode == 0)
	{
		((RecordLayer*)(mgr->mParent))->processJniStopSuccess();
	}
	else if (mgr->mMode == 1)
	{
		((RecordLayer*)(mgr->mParent))->processJniStopSuccessForDone();
	}
}


// JNI CALL 실패
void VoiceRecordManager::informVoiceRecordCancel()
{
	CCLOG("informVoiceRecordCancel...............");
}

