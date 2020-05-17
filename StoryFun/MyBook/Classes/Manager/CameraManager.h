#ifndef __CAMERA_MANAGER_H__
#define __CAMERA_MANAGER_H__

#include "cocos2d.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif

/**
* @brief 카메라 촬영 이벤트 리스너 
*/
class CameraManagerListener 
{
public:
	/**
	* 사진 촬영 종료
	* @param result true->사진 촬영, false->사진 촬영 취소
	*/
	virtual void onCameraFinished(bool result) = 0;
	/// 사진 촬영 후 촬영 된 이미지 셋팅 완료 
	virtual void onCameraFinishedReal() = 0;
};

/**
* @brief 카메라 촬영 관리자
*/
class CameraManager : public cocos2d::Node 
{
public:
	/// 촬영한 이미지 구분자
	static const int IMAGE_TAG;
	/**
	* 카메라 메니저 생성
	* @param Node* pTarget 촬영된 이미지를 담은 node
	* @param CameraManagerListener* pListener 카메라 메니저 이벤트 리스너
	*/
	static bool shootCamera(cocos2d::Node* pTarget, CameraManagerListener* pListener);
	/// 촬영 취소
	static void dismissed();
	/// 촬영 
	static void ready();

private:
	static CameraManager* camera;
	/// 촬영된 이미지를 담은 node
	cocos2d::Node* target;
	/// 이벤트 리스너
	CameraManagerListener* listener;
	/// 촬영 여부
	bool isReady;
	/// 카메라 메니저 활성 상태
	static bool isWork;

	CameraManager() {};
	/// 안드로이드에 카메라 요청
	void start();
	/// 촬영 후 촬영 된 이미지 적용
	void finished(float delta);
};

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C" {
#endif
	void Java_org_cocos2dx_cpp_AppActivity_informNative(JNIEnv* env, jobject thiz);
	void Java_org_cocos2dx_cpp_AppActivity_informNativeCancel(JNIEnv* env, jobject thiz);
#ifdef __cplusplus
}
#endif

#endif // if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#endif //__CAMERA_MANAGER_H__
