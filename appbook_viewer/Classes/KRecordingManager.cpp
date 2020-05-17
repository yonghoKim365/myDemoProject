
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

/*���� ��� �ʱ�ȭ*/
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

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) /*���� ���� callback*/
extern "C" void Java_com_kid_KRecordingManager_callNativeRecordingEnded(JNIEnv * env, jobject jobj, jstring aName) {
	const char * filename = env->GetStringUTFChars(aName, NULL);
	KRecordingManager::getInstance()->recordingEnded(filename);
	env->ReleaseStringUTFChars(aName, filename);
}
extern "C" void Java_com_kid_KRecordingManager_callNativeListeningEnded(JNIEnv * env, jobject jobj) {
	KRecordingManager::getInstance()->listeningEnded();
}


#endif

/*��������*/
void KRecordingManager::startRecording(){
	int nAudioID = cocos2d::experimental::AudioEngine::play2d(mStartSoundFilename, false, 1.0f);
	cocos2d::experimental::AudioEngine::setFinishCallback(nAudioID, [&](int andioID, const std::string & filename) {
		cocos2d::log("GGGGGGGGGGGGGGGGGGGGGGGGGGG");
		callForAndroidNative();
	});
}

/*���� ����*/
void KRecordingManager::stopRecording(){

	callForAndroidNative_StopRecording();

}

/*��������*/
void KRecordingManager::recordingEnded(std::string aName){
	if (_delegate != nullptr)
		_delegate->onRecordingEnded();
	cocos2d::log("-------------------------> ���� ��... ");
}
/*�������ϵ� ����*/
void KRecordingManager::setStartSoundFilename(std::string aName){
	mStartSoundFilename = aName;
}
/*�������� ����*/
void KRecordingManager::setRecordingFilename(std::string aName){
	mRecordingFilename = aName;
}

/*�����ð�*/
void KRecordingManager::setDuration(int aDuration){
	m_nDuration = aDuration;
}

/*�����ð� �Ѱ��ֱ�*/
int KRecordingManager::getDuration(){
	return m_nDuration;
}

/*JIN �������� �˸���.*/
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

/*�������� �˸���.*/
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

/*delegate ����*/
void KRecordingManager::setDelegate(KRecordingManagerDelegate * delegate){
	_delegate = delegate;
}

/*JNI ������ ���� ���*/
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

/*JNI ��� ����*/
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

/*��� ���� */
void KRecordingManager::listeningEnded() {
	// --> Delegate�� ����� �����ϴ� ��Ȱ�� �Ѵ�.
	log("KRecordingManager::listeningEnded()");
	if (_delegate != nullptr)
		_delegate->onListeningEnded();
}

/*������ ���� ���*/
std::string KRecordingManager::getRecordFullPath(){
	return mRecordFullPath;
}
/*�ȵ���̵� �������� path*/
std::string KRecordingManager::getAndroidTargetPath(){
	return mAndroidTargetPath;
}
/*���� ������ ������û JNI*/
void KRecordingManager::makesureFolderExists(std::string sPath){
	// �̰����� Ǯ Path�� ������ �ȴ�.
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

/*�ȵ���̵� ����*/
std::string KRecordingManager::getActionTargetName(){
	return sActionTargetName;
}

/*action �̸�����*/
void KRecordingManager::setActionTargetName(std::string aTargetName){
	sActionTargetName = aTargetName;
}

/*toast �˾� ���� JNI*/
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