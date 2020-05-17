
#include "cocos2d.h"
#include "KRecordingManager.h"
#include "AudioEngine.h"


KRecordingManager* KRecordingManager::getInstance(){
	if (_instance == nullptr) {
		_instance = new KRecordingManager();
		_instance->initData();
	}
	return _instance;
}
KRecordingManager * KRecordingManager::_instance = nullptr;

/*녹음 모듈 초기화*/
void KRecordingManager::initData() {
	_delegate = nullptr;
	mStartSoundFilename = START_SOUND_FILENAME;
	m_nDuration = DEFAULT_DURATION;

	//log("KCameraManager::initData()....");
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodInfo;
	if (JniHelper::getStaticMethodInfo(methodInfo, KRECORDING_JAVA_REFERENCE_CLASS
		, "getInstance", "()Lcom/kid/KRecordingManager;"))
	{
		//log("KCameraManager::initData...");
		mRefJObj = (jobject)methodInfo.env->CallStaticObjectMethod(methodInfo.classID, methodInfo.methodID);
		mRefJObj = methodInfo.env->NewGlobalRef(mRefJObj);
		methodInfo.env->DeleteLocalRef(methodInfo.classID);
	}

	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "requestAndroidTargetPath", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		mAndroidTargetPath = JniHelper::jstring2string(strRet);
		log("AndroidTargetPath=%s" , mAndroidTargetPath.c_str());
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif

}

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) /*녹음 종료 callback*/
extern "C" void Java_com_kid_KRecordingManager_callNativeRecordingEnded(JNIEnv * env, jobject jobj, jstring aName) {
	const char * filename = env->GetStringUTFChars(aName, NULL);
	KRecordingManager::getInstance()->recordingEnded(filename);
	env->ReleaseStringUTFChars(aName, filename);
}
extern "C" void Java_com_kid_KRecordingManager_callNativeListeningEnded(JNIEnv * env, jobject jobj) {
	KRecordingManager::getInstance()->listeningEnded();
}


#endif

/*녹음시작*/
void KRecordingManager::startRecording(){
	int nAudioID = cocos2d::experimental::AudioEngine::play2d(mStartSoundFilename, false, 1.0f);
	cocos2d::experimental::AudioEngine::setFinishCallback(nAudioID, [&](int andioID, const std::string & filename) {
		cocos2d::log("GGGGGGGGGGGGGGGGGGGGGGGGGGG");
		callForAndroidNative();
	});
}

/*녹음 멈춤*/
void KRecordingManager::stopRecording(){

	callForAndroidNative_StopRecording();

}

/*녹음종료*/
void KRecordingManager::recordingEnded(std::string aName){
	if (_delegate != nullptr)
		_delegate->onRecordingEnded();
	cocos2d::log("-------------------------> 녹음 끝... ");
}
/*녹음파일된 파일*/
void KRecordingManager::setStartSoundFilename(std::string aName){
	mStartSoundFilename = aName;
}
/*녹음파일 지정*/
void KRecordingManager::setRecordingFilename(std::string aName){
	mRecordingFilename = aName;
}

/*녹음시간*/
void KRecordingManager::setDuration(int aDuration){
	m_nDuration = aDuration;
}

/*녹음시간 넘겨주기*/
int KRecordingManager::getDuration(){
	return m_nDuration;
}

/*JIN 녹음시작 알린다.*/
void KRecordingManager::callForAndroidNative() {
	if (_delegate != nullptr)
		_delegate->onRecordingStarted();
	cocos2d::log("callForAndroidNative");

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "startRecordingFromOutside", "(Ljava/lang/String;I)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(mRecordingFilename.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg,m_nDuration );
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif

}

/*녹음중지 알린다.*/
void KRecordingManager::callForAndroidNative_StopRecording() {
	cocos2d::log("callForAndroidNative_StopRecording");
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "stopRecordingFromOutside", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/*delegate 설정*/
void KRecordingManager::setDelegate(KRecordingManagerDelegate * delegate){
	_delegate = delegate;
}

/*JNI 녹음된 파일 듣기*/
void KRecordingManager::startListening(std::string sPath){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "startListeningFromOutside", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sPath.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/*JNI 듣기 종료*/
void KRecordingManager::stopListening(){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "stopListeningFromOutside", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/*듣기 종료 */
void KRecordingManager::listeningEnded() {
	// --> Delegate에 통신을 전달하는 역활을 한다.
	log("KRecordingManager::listeningEnded()");
	if (_delegate != nullptr)
		_delegate->onListeningEnded();
}

/*녹음된 파일 경로*/
std::string KRecordingManager::getRecordFullPath(){
	return mRecordFullPath;
}
/*안드로이드 녹음파일 path*/
std::string KRecordingManager::getAndroidTargetPath(){
	return mAndroidTargetPath;
}
/*폴더 없으면 생성요청 JNI*/
void KRecordingManager::makesureFolderExists(std::string sPath){
	// 이것으로 풀 Path가 결정이 된다.
	mRecordFullPath = sPath;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "makesureFolderExists", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sPath.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/*안드로이드 폴더*/
std::string KRecordingManager::getActionTargetName(){
	return sActionTargetName;
}

/*action 이름저장*/
void KRecordingManager::setActionTargetName(std::string aTargetName){
	sActionTargetName = aTargetName;
}

/*toast 팝업 전달 JNI*/
void KRecordingManager::showToastForJNI(std::string sMessage){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KRECORDING_JAVA_REFERENCE_CLASS, "showToastForJNI", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sMessage.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}