#include "VideoRecordManager.h"
#include "Scene/PlayScene.h"
#include "Contents/MyBookSnd.h"

USING_NS_CC;

// singleton
VideoRecordManager* VideoRecordManager::_videoRecordManager = nullptr;

#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#include "platform/android/jni/JniHelper.h"
#include <jni.h>

#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C" {
#endif
	void Java_org_cocos2dx_cpp_AppActivity_informNativeVideoRecord(JNIEnv* env, jobject thiz)
	{
		VideoRecordManager::getInstance()->playAnimation();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVideoRecordCancel(JNIEnv* env, jobject thiz)
	{
		VideoRecordManager::getInstance()->stop();
		VideoRecordManager::getInstance()->returnMainScene();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVideoRecordEndingPage(JNIEnv* env, jobject thiz)
	{
		VideoRecordManager::getInstance()->playAnimation();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVideoRecordCancelEndingPage(JNIEnv* env, jobject thiz)
	{
		VideoRecordManager::getInstance()->stop();
		VideoRecordManager::getInstance()->returnPlayScene();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeVideoRecordCancelForSleep(JNIEnv* env, jobject thiz)
	{		
		VideoRecordManager::getInstance()->returnMainScene();
	}

#ifdef __cplusplus
}
#endif

#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

// singleton
VideoRecordManager* VideoRecordManager::getInstance()
{	
	if (_videoRecordManager == nullptr)
	{
		_videoRecordManager = new VideoRecordManager();
	}	

	return _videoRecordManager;
}


bool VideoRecordManager::init()
{
	//CCLOG("VoiceRecordManager init...");	

	return true;
}


VideoRecordManager::VideoRecordManager()
{
	//CCLOG("VoiceRecordManager create...");	
	mIsRecording = false;	
}


VideoRecordManager::~VideoRecordManager()
{
	free(_videoRecordManager);
}

// 배열하기 화면에서 진입시
void VideoRecordManager::start(Node* parent, int week, std::string strPath)
{
	mIsRecording = true;	
	mParent = parent;

	//CCLOG("VIDEORECORD START.....%d, %s", week, strPath.c_str());	
	
	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "startRecordingVideo", "(Ljava/lang/String;I)V")) {		
		jint param2 = week;	
		const char* strParam = strPath.c_str();
		jstring param1 = info.env->NewStringUTF(strParam);
		info.env->CallStaticVoidMethod(info.classID, info.methodID, param1, param2);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


// 엔딩 팝업에서 호출
void VideoRecordManager::startForEndingPage(Node* parent, int week, std::string strPath)
{
	mIsRecording = true;
	mParent = parent;

	//CCLOG("VIDEORECORD START.....%d", week);
	
	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "startRecordingVideoForEndingPage", "(I;Ljava/lang/String;)V")) {
		jint param1 = week;
		const char* strParam = strPath.c_str();
		jstring param2 = info.env->NewStringUTF(strParam);
		info.env->CallStaticVoidMethod(info.classID, info.methodID, param1, param2);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


void VideoRecordManager::stop()
{
	mIsRecording = false;
	//CCLOG("VIDEORECORD STOP.....");
	
	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "stopRecordingVideo", "()V")) {		
		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

}


void VideoRecordManager::returnMainScene()
{
	mIsRecording = false;
	//CCLOG("VIDEORECORD STOP.....AND RETURN MAINSCENE.......");
	CC_ASSERT(mParent != nullptr);
	((PlayScene*)mParent)->closePlay();
}

// 엔딩 팝업에서 취소 했을경우 처리
void VideoRecordManager::returnPlayScene()
{
	mIsRecording = false;
	//CCLOG("VIDEORECORD STOP.....AND RETURN PLAYSCENE...");
	CC_ASSERT(mParent != nullptr);
	((PlayScene*)mParent)->resumePlayEndingPage();	
	
}


bool VideoRecordManager::isRecording()
{
	return mIsRecording;
}


void VideoRecordManager::playAnimation()
{
	//CCLOG("playAnimation");
	// 최초에는 팝업 창이 없으므로 확인 필요		

	
	// 메인 씬 버튼 비활성화
	CCLOG("playAnimation disabled main button");
	CC_ASSERT(mParent != nullptr);
	((PlayScene*)mParent)->setBtnMainScene(false);

	// AniMation		
	((PlayScene*)mParent)->playAni();	

}


void VideoRecordManager::returnMainSceneForSleep()
{
	mIsRecording = false;
	//CCLOG("VIDEORECORD STOP.....AND RETURN MAINSCENE.......");
	CC_ASSERT(mParent != nullptr);
	//((PlayScene*)mParent)->stopAllActions();

	((PlayScene*)mParent)->closePlay();
}