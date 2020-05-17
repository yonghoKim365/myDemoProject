#include "KCameraManager.h"

#include "ui/CocosGUI.h"

KCameraManager * KCameraManager::_instance = nullptr;
KCameraManager * KCameraManager::getInstance(){
	if (_instance == nullptr) {
		_instance = new KCameraManager();
		_instance->initData();
	}
	return _instance;
}
void KCameraManager::removeInstance() {
	if (_instance == nullptr) return;
	delete _instance;
	_instance = nullptr;
}


void KCameraManager::initData() {
	delegate = nullptr;
	//log("KCameraManager::initData()....");
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	/* JNI �ȵ���̵忡 �ִ� KCameraManager Instance�� �޾� �´�.*/
	JniMethodInfo methodInfo;
	if (JniHelper::getStaticMethodInfo(methodInfo, KCAMERA_JAVA_REFERENCE_CLASS
			, "getInstance", "()Lcom/kid/KCameraManager;"))
	{
		//log("KCameraManager::initData...");
		mRefJObj = (jobject)methodInfo.env->CallStaticObjectMethod(methodInfo.classID, methodInfo.methodID);
		mRefJObj = methodInfo.env->NewGlobalRef(mRefJObj);
		methodInfo.env->DeleteLocalRef(methodInfo.classID);
	}
	/*JNI �ȵ���̵� Temp������ �޾ƿ´�. */
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KCAMERA_JAVA_REFERENCE_CLASS, "requestAndroidTargetPath", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		mAndroidTargetPath = JniHelper::jstring2string(strRet);
		log("AndroidTargetPath=%s" , mAndroidTargetPath.c_str());
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}

#endif

	//autorelease�� ���ġ �ʴ´�.
	mpSprite = new Sprite();
}

void KCameraManager::setDelegate(KCameraManagerDelegate * delegate){
	this->delegate = delegate;
}

/* �ȵ���̵忡�� cocos2d-x�� ����� �̸��� �Ѱ��ش�. */
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
extern "C" void Java_com_kid_KCameraManager_callNativePictureTaken(JNIEnv * env, jobject jobj, jstring aName) {
	const char * filename = env->GetStringUTFChars(aName, NULL );
	KCameraManager::getInstance()->refreshUI( filename );
	env->ReleaseStringUTFChars(aName, filename);
}
#endif

/*������� �� �� ȭ���� RefreshUI */
void KCameraManager::refreshUI(std::string aFilename) {
	//log("KCameraManager::refreshUI");
	if (delegate == nullptr) return;
	delegate->onRefreshUI(aFilename);
}

/*JNI @sFilename �����̸� ���� ������� ��û ������.*/
void KCameraManager::requestPicture(std::string sFilename){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KCAMERA_JAVA_REFERENCE_CLASS, "requestPicture", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sFilename.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}


void KCameraManager::setActionTargetName(std::string aTargetName){
	sActionTargetName = aTargetName;
}

void KCameraManager::setActionTargetLayer(Layer * pLayer){
	mpActionTargetLayer = pLayer;
}

Node * KCameraManager::getActionTargetNode() {
	if (mpActionTargetLayer == nullptr) {
		log("can't find the right target Layer..");
		return nullptr;
	}
	if (sActionTargetName.empty()) {
		log("can't find the right target node name.. ");
		return nullptr;
	}
	cocos2d::Node * pReturnNode = mpActionTargetLayer->getChildByName(sActionTargetName);
	return pReturnNode;
}
Layer * KCameraManager::getActionTargetLayer(){
	if (mpActionTargetLayer == nullptr) {
		log("can't find the right target Layer..");
		return nullptr;
	}
	return mpActionTargetLayer;
}

Sprite * KCameraManager::getSprite(){
	return mpSprite;
}

std::string KCameraManager::getAndroidTargetPath(){
	return mAndroidTargetPath;
}

void KCameraManager::setTextFieldObject(ui::TextField* aTextField){
	mTextField = aTextField;
}
ui::TextField* KCameraManager::getTextFieldObject(){
	return mTextField;
}

/*XML project code�� �����̸��� ����ȴ�. å�� 1���� ����(��å)*/
void KCameraManager::determineFullpathnameForPicture(std::string sProjectcode){
	mFileFullPath = StringUtils::format("%s/%s.png", getAndroidTargetPath().c_str(), sProjectcode.c_str());
	log("KCameraManager:determineFullPathname  %s", mFileFullPath.c_str());
}
std::string KCameraManager::getFullpathnameForPicture(){
	return mFileFullPath;
}