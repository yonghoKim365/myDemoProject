#include "KDataProvider.h"

#include "KStringUtil.h"

/*
	�ȵ���̵� com/kid/KDataProvider Ŭ������ JNI ���
*/

KDataProvider * KDataProvider::_instance = nullptr;
/* KDataProvider Instance �Ѱ��ش�. */
KDataProvider * KDataProvider::getInstance(){
	if (_instance == nullptr) {
		_instance = new KDataProvider();
		_instance->initData();
	}
	return _instance;
}

/* KDataProvider Instance Delete */
void KDataProvider::removeInstance() {
	if (_instance == nullptr) return;
	delete _instance;
	_instance = nullptr;
}

/* ��ƿ string replace ���� */
std::string KDataProvider::replaceString(std::string subject, const std::string & search, const std::string & replace) {
	size_t pos = 0;
	while ((pos = subject.find(search, pos)) != std::string::npos) {
		subject.replace(pos, search.length(), replace);
		pos += replace.length();
	}
	return subject;
}


std::string KDataProvider::modifyEntityPath(std::string aPath){
	return replaceString(aPath, "/","_");
}
std::string KDataProvider::mVirtualPath = "";
std::string KDataProvider::mTempDirPath = "";
std::string KDataProvider::mDebugMode = "";

/* �ʱ�ȭ */
void KDataProvider::initData() {
	//log("KDataProvider::initData()....");
	setFirst(true);
	mListenMode = 0;
	mIsSound = true;
	mIsMainBgm = false;
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	/* JNI ���� ��ü ������ �޾� ����  */
	JniMethodInfo methodInfo;
	if (JniHelper::getStaticMethodInfo(methodInfo, KJAVA_REFERENCE_CLASS, "getInstance", "()Lcom/kid/KDataProvider;"))
	{
		mRefJObj = (jobject)methodInfo.env->CallStaticObjectMethod(methodInfo.classID, methodInfo.methodID);
		mRefJObj = methodInfo.env->NewGlobalRef(mRefJObj);
		methodInfo.env->DeleteLocalRef(methodInfo.classID);
	}

	//mVirtualPath = KVIRTUAL_PATH;

	/* �ȵ���̵忡 ������ Temp ��� �޾ƿ���  */
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "getTempDirPath", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		mTempDirPath = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
		log("getTempDirPath mTempDirPath= %s", mTempDirPath.c_str());
	}

	/* ���� Debug ���� Ȯ�� */
	JniMethodInfo debugmode;
	if (JniHelper::getMethodInfo(debugmode, KJAVA_REFERENCE_CLASS, "getDebugMode", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)debugmode.env->CallObjectMethod(mRefJObj, debugmode.methodID);
		mDebugMode = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(debugmode.classID);
		log("getTempDirPath mDebugMode= %s", mDebugMode.c_str());
	}

	if (mDebugMode == "true"){
		mVirtualPath = KVIRTUAL_PATH;
	}else{
		mVirtualPath = KTEMP_PATH;
	}

#endif
}


/* ���ҽ� Temp ��� ���� */
std::string KDataProvider::cTT(const std::string & sEntity){
	return convertToTempPath(sEntity);
}

/* Virtual Path���� (�̹����� cocos2d cache��� �̶� ���� �̸�)  */
std::string KDataProvider::cTV(const std::string & sEntity){
	return convertToVirtual(sEntity);
}

/* Temp Path Ȯ��  */
std::string KDataProvider::convertToTempPath(const std::string & sEntity){
	std::string syscommon = "syscommon/";
	std::string sCompared = sEntity.substr(0, syscommon.size());
	/* syscommon�� �ִ� �����̸� �ڽ� �Ѱ��� */
	if (syscommon == sCompared) {  
		return sEntity;
	}
	/* windows �̸������� ��� �ڽ� �Ѱ���*/
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	std::string sReturn = sEntity;
	return sReturn;
#else
	std::string sTempPath = mTempDirPath;
	if(mDebugMode == "false"){
		sTempPath += modifyEntityPath(sEntity);
		//log("convertToTempPath,sTempPath:%s", sTempPath.c_str());
	}
	else
		sTempPath += sEntity;

	//log("convertToTempPath, sTempPath:%s", sTempPath.c_str());
	return sTempPath;
#endif
}

/* �̹��� cache ���� */
std::string KDataProvider::convertToVirtual(const std::string & sEntity) {
	std::string syscommon = "syscommon/";
	std::string sCompared = sEntity.substr(0, syscommon.size());
	if (syscommon == sCompared) {
		return sEntity;// .substr(syscommon.size());
	}
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	std::string sReturn = sEntity;
	return sReturn;
#else
	std::string sVirtual = mVirtualPath;
	sVirtual += sEntity;

	//log("convertToVirtual, sVirtual:%s", sVirtual.c_str());

	return sVirtual;
#endif
}

/* JNI xml data �ޱ�  */
void KDataProvider::loadXmlData(std::string sFilename) {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	mXmlData = FileUtils::getInstance()->getStringFromFile(sFilename);
#else
	/* JNI xml ����Ÿ ��û  */
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "getXmlData", "(Ljava/lang/String;)Ljava/lang/String;")) {
		jstring jstr = methodLocal.env->NewStringUTF(sFilename.c_str());
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID, jstr);
		std::string sReturn = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(jstr);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		mXmlData = sReturn;

		log("xxxxxxxxx sReturn= %s", mXmlData.c_str());
	}
#endif
}

/* JNI �ϵ����� �޾ƿ��� */
void KDataProvider::isFinished() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "isFinished", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		std::string sReturn = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		misFindshed = sReturn;

		log("[appBookView] isFinished = %s", misFindshed.c_str());
	}
#endif
}

/* JNI ���� ���� �޾ƿ��� */
void KDataProvider::getProcess() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "getProgress", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		std::string sReturn = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		mCurPage = atoi(sReturn.c_str());

		log("[appBookView] getProcess = %d", mCurPage);
	}
#endif
}

/* JNI å ���� ���� �Ѱ��ֱ� */
void KDataProvider::setBeginBook(int ncurpage,int ntotalpage,int isauto) {
	if(mDebugMode == "true") return;
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "setBegin", "(III)V")) {
		methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID,ncurpage, ntotalpage,isauto);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

	}
#endif
}

/* JNI ���� ����Ǵ� ������ �ѱ��ֱ� */
void KDataProvider::setProgressPage(int aProgress){

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "setProgressPage", "(I)V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, aProgress);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
		if (aProgress > mCurPage) mCurPage = aProgress;
	}
#endif
}

/* JNI  �ϱ�/��� ��� ���� */
void KDataProvider::setListenMode(int nMode){

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "setListenMode", "(I)V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, nMode);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
		 mListenMode = nMode;

		 log("[Viewer] setListenMode = %d", mListenMode);
	}
#endif
}

/* JNI ������ �ϱ�/��� ��� �޾ƿ��� */
int KDataProvider::getListenMode(){
	if(mDebugMode == "true") return 1;
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "getListenMode", "()I")) {
		jint nRet = (jint)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);

		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		mListenMode = nRet;

		log("[Viewer] getListenMode = %d", mListenMode);
		return mListenMode;
	}
#endif
	return 0;
}

/* JNI ������ �ϱ�/��� ��� �޾ƿ��� */
int KDataProvider::getSystemVolumn(){

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "getSystemVolumn", "()I")) {
		jint nRet = (jint)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		
		
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
		int nVol = (int)nRet;

		log("[ViewerC] getSystemVolumn = %d", nVol);
		return nVol;
	}
#endif
	return 10;
}

/* JNI ������� ���� ��û */
void KDataProvider::acquireCpuWakeLock() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "acquireCpuWakeLock", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/* JNI ������� ���� ���� ��û */
void KDataProvider::releaseCpuLock() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "releaseCpuLock", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/* JNI å ����� ȣ�� */
void KDataProvider::currentBookFinish(std::string sIsFinish) {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "setFinish", "(Ljava/lang/String;)V")) {
		jstring jstr  = methodLocal.env->NewStringUTF(sIsFinish.c_str());
	
		methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID, jstr);
		methodLocal.env->DeleteLocalRef(jstr);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

/* JNI ���� ����Ʈ ȣ�� */
void KDataProvider::nextBook() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	//log("KCameraManager::requestPicture is called..");
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "setNextBook", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

bool KDataProvider::hasNext(){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "hasNext", "()Z")) {
		jboolean nRet = (jboolean)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);

		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		return nRet;
	}

#else
	return true;
#endif
}

std::string KDataProvider::getXmlData(){
	return mXmlData;
}

/* Temp ������ ����� ���� ���� �� �д�. */
std::string KDataProvider::saveDataToFile(std::string sEntity) {

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	return sEntity;
#else

	log("saveDataToFile start :%s" , sEntity.c_str());
	//���� ó��.. ( ������ �ִ� ȭ���� ����� �ʿ䰡 ����.)
	std::string syscommon = "syscommon/";
	std::string sCompared = sEntity.substr(0, syscommon.size());
	if(syscommon == sCompared) {
		return sEntity;//.substr(syscommon.size());
	}
	unsigned char * pData = NULL;

	int nSize = 0;
	if (mDebugMode == "true"){
		nSize = getBytesFromEntity(sEntity, &pData);
	}
	else{
		nSize = getBytesFromEntityInOpenedFile(sEntity, &pData);
	}
	if( nSize == 0) {
		std::string sReturn = "";
		return sReturn;
	}

	std::string sTempPath = convertToTempPath(sEntity);

	Data data;
	data.copy(pData, nSize);
	FileUtils::getInstance()->writeDataToFile(data, sTempPath);
	free(pData);
	//log("saveDataToFile finish :%s" , sTempPath.c_str());
	return sTempPath;
#endif
}


long KDataProvider::getMillTime(){
	timeval time;
	gettimeofday(&time, NULL);
	long millisecs = (time.tv_sec * 1000) + (time.tv_usec / 1000);
	return millisecs;
}


/* in Memory�� �ִ� �̹����� �о TextureCache�� ��� �д�. */
std::string KDataProvider::saveDataToCache(std::string sEntity) {

	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	return KStringUtil::replaceAll(sEntity, "syscommon/", "");
#else

	log("saveDataToCach start %s" , sEntity.c_str());

	//���� ó��.. ( ������ �ִ� ȭ���� ����� �ʿ䰡 ����.)
	std::string syscommon = "syscommon/";
	std::string sCompared = sEntity.substr(0, syscommon.size());
	if(syscommon == sCompared) {
		return sEntity.substr(syscommon.size());
	}
	unsigned char * pData = NULL;

	int nSize = 0;
	if (mDebugMode == "true"){
		nSize = getBytesFromEntity(sEntity, &pData);
	}
	else{
		nSize = getBytesFromEntityInOpenedFile(sEntity, &pData);
	}
	if( nSize == 0) {
		log("saveDataToCach nSize==0");
		std::string sReturn = "";
		return sReturn;
	}

	log("saveDataToCach nSize=%d sEntity:%S", nSize, sEntity.c_str());
	
	std::string sVirtual = convertToVirtual(sEntity);
	unsigned char * data = nullptr;
	ssize_t dataLength = 0;
	Image * image = new (std::nothrow) Image();

	bool isOK = image->initWithImageData((unsigned char *)pData, nSize);

	Director::getInstance()->getTextureCache()->addImage(image, sVirtual);

	log("sVirtual=%s" , sVirtual.c_str()); // ex) /virtual/Images/thumb.jpg

	CC_SAFE_RELEASE(image);
	free(pData);

	log("saveDataToCache finish %s", sEntity.c_str());

	return sVirtual;
#endif
}

std::string KDataProvider::getMediaType() {

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "getMediaType", "()Ljava/lang/String;")) {
		jstring strRet = (jstring)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID);
		std::string sReturn = JniHelper::jstring2string(strRet);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		log("[appBookView] mediaTp = %s", sReturn.c_str());

		return sReturn;
		
	}
#else
	return "APE_ZIP";
	//return "API_ZIP";
#endif
}


#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
/* JNI DRM �ش� Entry ����Ÿ �޾ƿ���*/
int KDataProvider::getBytesFromEntity(std::string sEntity, unsigned char ** ppData) {

	log("getBytesFromEntity=%s" , sEntity.c_str());
	
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "getBytesFromEntity", "(Ljava/lang/String;)[B")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sEntity.c_str());
		jbyteArray newArray = (jbyteArray)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID, stringArg);
		
		if( newArray == NULL ) {
			
			//Entry�� �������� ���..
			methodLocal.env->DeleteLocalRef(stringArg);
			
			methodLocal.env->DeleteLocalRef(methodLocal.classID);
			
			return 0;
		}
		
		jsize theArrayLen = methodLocal.env->GetArrayLength(newArray);
		
		*ppData = (unsigned char *)malloc(sizeof(char) * theArrayLen);
		methodLocal.env->GetByteArrayRegion(newArray, 0, theArrayLen, (jbyte *)*ppData);
		methodLocal.env->DeleteLocalRef(newArray);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		//log("theArrayLen=%d", theArrayLen);
		return theArrayLen;
	}
	return -1;
}

int KDataProvider::getBytesFromEntityInOpenedFile(std::string sEntity, unsigned char ** ppData) {

	log("getBytesFromEntityInOpenedFile=%s" , sEntity.c_str());

	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS , "getBytesFromEntityInOpenedFile", "(Ljava/lang/String;)[B")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sEntity.c_str());
		jbyteArray newArray = (jbyteArray)methodLocal.env->CallObjectMethod(mRefJObj, methodLocal.methodID, stringArg);

		if( newArray == NULL ) {

			//Entry�� �������� ���..
			methodLocal.env->DeleteLocalRef(stringArg);

			methodLocal.env->DeleteLocalRef(methodLocal.classID);

			return 0;
		}

		jsize theArrayLen = methodLocal.env->GetArrayLength(newArray);

		*ppData = (unsigned char *)malloc(sizeof(char) * theArrayLen);
		methodLocal.env->GetByteArrayRegion(newArray, 0, theArrayLen, (jbyte *)*ppData);
		methodLocal.env->DeleteLocalRef(newArray);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);

		//log("theArrayLen=%d", theArrayLen);
		return theArrayLen;
	}
	return -1;
}



void KDataProvider::preload() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "preload", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}



void KDataProvider::openDrmZipFile() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "openDrmZipFile", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

void KDataProvider::closeDrmZipFile() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "closeDrmZipFile", "()V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}

#endif
}

/* JNI �����δ��� ���� �� �ִ� % �� (���� ����Ÿ �ε� %) */
void KDataProvider::setProgress(int aProgress){

	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "setProgress", "(I)V")) {
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, aProgress);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}

}

/* JNI Toast popup ���� ���� */
void KDataProvider::showToastForJNI(std::string sMessage){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "showToastForJNI", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(sMessage.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

void KDataProvider::addEntryName(std::string name){
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "addEntryName", "(Ljava/lang/String;)V")) {
		jstring stringArg = methodLocal.env->NewStringUTF(name.c_str());
		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, stringArg);
		methodLocal.env->DeleteLocalRef(stringArg);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif
}

#endif




/*
void KDataProvider::setEntryList(std::vector<std::string> vector)
{
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	
	JniMethodInfo methodLocal;
	if (JniHelper::getMethodInfo(methodLocal, KJAVA_REFERENCE_CLASS, "setEntryList", "(Ljava/util/ArrayList;)V")) {

		jclass		java_util_ArrayList = methodLocal.env->NewGlobalRef(methodLocal.env->FindClass("java/util/ArrayList"));
		jmethodID	java_util_ArrayList_ = methodLocal.env->GetMethodID(java_util_ArrayList, "<init>", "(I)V");
		//jmethodID	java_util_ArrayList_size = env->GetMethodID(java_util_ArrayList, "size", "()I");
		//jmethodID	java_util_ArrayList_get = env->GetMethodID(java_util_ArrayList, "get", "(I)Ljava/lang/Object;");
		jmethodID	java_util_ArrayList_add = methodLocal.env->GetMethodID(java_util_ArrayList, "add", "(Ljava/lang/Object;)V");

		jobject result = methodLocal.env->NewObject(java_util_ArrayList, java_util_ArrayList_, vector.size());
		for (std::string s : vector) {
			jstring element = methodLocal.env->NewStringUTF(s.c_str());
			methodLocal.env->CallVoidMethod(result, java_util_ArrayList_add, element);
			methodLocal.env->DeleteLocalRef(element);
		}

		methodLocal.env->CallVoidMethod(mRefJObj, methodLocal.methodID, result);
		methodLocal.env->DeleteLocalRef(result);
		methodLocal.env->DeleteLocalRef(methodLocal.classID);
	}
#endif

}
*/




