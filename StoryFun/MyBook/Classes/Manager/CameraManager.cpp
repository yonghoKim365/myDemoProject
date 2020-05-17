#include "CameraManager.h"
#include "Util/Pos.h"
#include "Contents/MyBookResources.h"

USING_NS_CC;

const int CameraManager::IMAGE_TAG = 7777;
bool CameraManager::isWork = false;
CameraManager* CameraManager::camera = NULL;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C" {
#endif
	void Java_org_cocos2dx_cpp_AppActivity_informNative(JNIEnv* env, jobject thiz) 
	{
		CameraManager::ready();
	}

	void Java_org_cocos2dx_cpp_AppActivity_informNativeCancel(JNIEnv* env, jobject thiz) 
	{
		CameraManager::dismissed();
	}
#ifdef __cplusplus
}
#endif

#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

// 메인씬에서 카메라 버튼 클릭시 호출
/**
* @brief 메인씬에서 카메라 버튼 클릭시 호출
* @param  pTarget   카메라 레이어
* @param  pListener 카메라 메니져 리스너
* @return  bool     성공유무
*/
bool CameraManager::shootCamera(Node* pTarget, CameraManagerListener* pListener) 
{
	//CCLOG("CameraManager::shootCamera");

	if (!isWork) 
	{
		if (camera) 
		{
			delete camera;
			CCLOG("delete camera");
		}

		camera = new CameraManager();
		camera->target = pTarget;
		camera->listener = pListener;
		camera->isReady = false;
		camera->isWork = true;
		camera->retain();
		camera->schedule(schedule_selector(CameraManager::finished), 0.2f);

		camera->start();
		pTarget->addChild(camera);

		//CCLOG("new camera finish!");

		return true;
	}
	return false;
}

// 카메라 화면 열기
void CameraManager::start() 
{
	//CCLOG("CameraManager::start");
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "shootCamera", "()V")) {
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}

// 촬영된 이미지 경로를 안드로이드로 부터 가지고 와서 설정
void CameraManager::finished(float delta) 
{
	CCLOG("CameraManager::finished");
	if (isReady) 
	{
		//unscheduleAllSelectors();
		unscheduleAllCallbacks();

		Vec2 origin = Director::getInstance()->getVisibleOrigin();

		Sprite* spr;
		Sprite* profileSp;
		ClippingNode* clip = ClippingNode::create();
		std::string path;
		int rotation = 0;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID) 				
		JniMethodInfo t;
		if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "getImagePath", "()Ljava/lang/String;")) 
		{
			jstring retFromJava = (jstring)t.env->CallStaticObjectMethod(t.classID, t.methodID);
			const char* str = t.env->GetStringUTFChars(retFromJava, 0);
			path = (std::string) str;
			t.env->ReleaseStringUTFChars(retFromJava, str);
			t.env->DeleteLocalRef(t.classID);
		}
		log("finished image path:: %s", path.c_str());		
		
		JniMethodInfo t2;
		if (JniHelper::getStaticMethodInfo(t2, "org/cocos2dx/cpp/AppActivity", "getImageRotation", "()I")) 
		{
			//jint retFromJava = (jstring)t2.env->CallStaticIntMethod(t2.classID, t2.methodID);
			int ret = t2.env->CallStaticIntMethod(t2.classID, t2.methodID);
			rotation = ret;			
			t2.env->DeleteLocalRef(t2.classID);
		}
		log("finished image rotation : %d", rotation);
		// 이전 사진 정보 처리(삭제)
		UserDefault* userDefault = UserDefault::getInstance();
		std::string preProfileFileName = userDefault->getStringForKey(USERDEFAULT_KEY_PROFILE_PATH);
		FileUtils::getInstance()->removeFile(preProfileFileName);

		// 사진 정보 저장
		userDefault->setStringForKey(USERDEFAULT_KEY_PROFILE_PATH, path);
		userDefault->setFloatForKey(USERDEFAULT_KEY_PROFILE_ROTATION, rotation);		

		spr = Sprite::create(path.c_str());				
		auto visibleSize = Director::getInstance()->getVisibleSize();
		float sw = visibleSize.width / spr->getBoundingBox().size.width;
		float sh = visibleSize.height / spr->getBoundingBox().size.height;
		(sw > sh) ? spr->setScale(sw) : spr->setScale(sh);

		// 프로필 사진 설정				
		spr->setPosition(visibleSize/2);			
		spr->setTag(CameraManager::IMAGE_TAG);
				
		// 프로필 png 사이즈 (1280, 800)
		Size sizeForContent = spr->getContentSize();

		////////////////////////////////////////
		// profile size 변경
		TextureCache *textureCahe = TextureCache::getInstance();
		auto profileTexture = textureCahe->addImage(path.c_str());
		Vec2 profilePt = Pos::getCameraProfilePt();
		//-----------------------------------------------------------
		// 이부분 수정해야됨....
		CCLOG("ROTATION : %d", rotation);
		//profilePt.x += 80;
		if (rotation == 180)
		{
			profilePt.y -= 150;
		}
		else
		{
			profilePt.y += 30;
		}
		//-----------------------------------------------------------
		Size sizeForCh = Pos::getCameraProfilSizeForCharater();

		// 프로필 사진 크기
		Size realProfileSize;
		realProfileSize.width = sizeForCh.width * sizeForContent.width / spr->getBoundingBox().size.width;
		realProfileSize.height = sizeForCh.height * sizeForContent.height / spr->getBoundingBox().size.height;
		
		Rect profileRect = Rect(profilePt.x, profilePt.y, realProfileSize.width, realProfileSize.height);		
		
		// 메인씬 프로필 사진 사이즈
		Size sizeForMain = Pos::getCameraPhotoSizeForMainScene();
		float scale = sizeForMain.width / realProfileSize.width;
				
		profileSp = Sprite::createWithTexture(profileTexture, profileRect);				
		profileSp->setScale(scale);
		profileSp->setFlippedX(true);
		// 뒤집어서 촬영
		if (rotation == 180)
		{
			profileSp->setRotation(rotation);
		}

		Size contSize = profileSp->getContentSize();
		contSize.width = contSize.width * scale;
		
		////////////////////////////////////////
		// Stencil
		// setup stencil shape
		DrawNode* shape = DrawNode::create();		
		/* drawCircle(center, radius, angle, segments, drawLineToCenter, lineWidth, color)	*/	
		Vec2 posCenter = Pos::getCenterPt();		
		shape->drawSolidCircle(Pos::getCenterPt(), contSize.width / 2, CC_DEGREES_TO_RADIANS(0), 200, true, 1, Color4F(1, 0, 0, 1));

		// add shape in stencil		
		clip->setStencil(shape);
		clip->setAnchorPoint(Vec2::ANCHOR_MIDDLE);		

		Vec2 pos = Pos::getCameraPhotoPtForProfile();				
		pos.x = (pos.x - Pos::getScreenSize().width / 2) + contSize.width/2;
		pos.y = (pos.y - Pos::getScreenSize().height / 2) - contSize.width/2;
		// 프로필 위치 설정
		clip->setPosition(pos);

		// setup content		
		//profileSp->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
		profileSp->setPosition(posCenter);
		clip->addChild(profileSp);
		
		textureCahe->removeTexture(profileTexture);
		////////////////////////////////////////
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		
		if (target) 
		{
			target->removeChild(camera);	
			CCLOG("CLIP refresh...........");			
			target->addChild(clip);	
		}

		isWork = false;

		if (listener)
		{
			listener->onCameraFinishedReal();
		}
	}
}


//------------------------------------------------------------
// ANDROID NATIVE
//------------------------------------------------------------
// 안드로이드 informNative에서 호출
void CameraManager::ready()
{
	CCLOG("ANDROID::informNative -> CameraManager::ready");

	if (camera->listener)
	{
		camera->listener->onCameraFinished(true);
	}

	camera->isReady = true;
}

// 안드로이드 informNativeCancel 에서 호출
void CameraManager::dismissed() 
{
	CCLOG("CameraManager::dismissed");
	if (camera->isScheduled(schedule_selector(CameraManager::finished)))
	{
		//camera->unscheduleAllSelectors();
		camera->unscheduleAllCallbacks();		
	}

	if (camera->target)
	{
		camera->target->removeChild(camera);
	}

	isWork = false;

	if (camera->listener)
	{
		camera->listener->onCameraFinished(false);
	}
}
