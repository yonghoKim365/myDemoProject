#include "PlayScene.h"
#include "MainScene.h"

#include "Manager/VideoRecordManager.h"
#include "Contents/MyBookSnd.h"
#include "Scene/EndingScene.h"


#define ANI_INTERVAL_CHAR 1.0f
#define ANI_INTERVAL_STAR 1.0f
#define ANI_INTERVAL_CHAR_EFFECT 0.5f
#define ANI_INTERVAL_SCENE  10.0f //10.0f

#define ANI_MOTION_POS_UNIT 30.0f
#define ANI_DELAY_NARRATION 0.5f
// 캐릭터 보정값
#define ANI_MOTION_POS_Y_INIT  900.0f


// for AudioEngine
using namespace cocos2d::experimental;

/*
* @brief  씬생성
* @return  Scene 생성된 씬
*/
Scene* PlayScene::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = PlayScene::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}


Scene* PlayScene::createScene(Node* parent, int weekData)
{
	CCLOG("createScene ...........playscene");
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = PlayScene::create();
	layer->mParent = parent;
	layer->mWeekData = weekData;
	layer->currImageIndex = 1;

	// add layer as a child to scene
	scene->addChild(layer);
	
	// return the scene
	return scene;
}


Scene* PlayScene::scene(int index)
{
	index = MBJson::getInstance()->currentIndex;
	CCLOG("scene::::::::::;: %d", index);
	auto scene = Scene::create();
	auto layer = PlayScene::create();
	layer->mWeekData = MBJson::getInstance()->getWeekData();
	
	layer->currImageIndex = index;
	if (index == 0)
	{
		CCLOG("scene...loadDataCoverModify.................");
		layer->loadDataCoverModify();
	}
	else
	{
		layer->loadData();
	}

	//if (index == 6)
	//{
	//	// 스퀀스로 처리		
	//	layer->createUi();
	//}	
	//layer->createUi();
	scene->addChild(layer);
	return scene;
}


Layer* PlayScene::createLayer()
{
	CCLOG("createLayer no param...........playscene");
	// 'layer' is an autorelease object
	auto layer = PlayScene::create();

	// return the scene
	return layer;
}


Layer* PlayScene::createLayer(Node* parent, int weekData)
{
	//Device::setKeepScreenOn(true);

	CCLOG("createLayer...........playscene");
	// 'layer' is an autorelease object
	auto layer = PlayScene::create();

	layer->mParent = parent;
	layer->mWeekData = weekData;

	// 테스트
	//layer->mWeekData = 7;
	layer->loadDataCoverModify();
	layer->startPlay();
	CCLOG("createLayer...........playscene2");
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	layer->playAni();
#endif
	CCLOG("createLayer...........playscene3");
	// return the scene
	return layer;
}

/**
* @brief  책 내용
*/
void PlayScene::setDrawLayer()
{
	MBJson* json = MBJson::getInstance();
	Vec2 pos = Pos::getCenterPt();
	//float height = 902.f / 2.f;
	pos.y = Pos::getScreenSize().height * ((SCRREN_SIZE_HEIGHT - 600.f) / SCRREN_SIZE_HEIGHT);

	// 배경
	Sprite* spImg = Sprite::create(json->mDrawFileNames[currImageIndex].c_str());
	spImg->setPosition(pos);
	this->addChild(spImg, DEPTH_LAYER_PLAYSCENE::INIT);

	//  사용자가 그린 이미지
	//std::string strFullPath = strPath + StringUtils::format(FILENAME_DRAW_CUBE_IMG_X, mWeekData, currImageIndex);
	std::string strFullPath = MBJson::getInstance()->getDrawFileName(mWeekData, currImageIndex);
	if (FileUtils::getInstance()->isFileExist(strFullPath))
	{
		CCLOG("EXIST.... FILE [%s]", strFullPath.c_str());
		Sprite* spDrawImg = Sprite::create(strFullPath.c_str());
		spDrawImg->setPosition(pos);
		this->addChild(spDrawImg, DEPTH_LAYER_PLAYSCENE::INIT);
	}
	
	// 먹선 이미지
	std::string strFullPathLine = MBJson::getInstance()->getDrawLineImageFileName(currImageIndex);
	Sprite* spLineImg = Sprite::create(strFullPathLine.c_str());
	spLineImg->setPosition(pos);
	this->addChild(spLineImg, DEPTH_LAYER_PLAYSCENE::INIT);

	//  사용자가 그린 스티커
	std::string strFullPathSticker = MBJson::getInstance()->getDrawStickerFileName(mWeekData, currImageIndex);
	if (FileUtils::getInstance()->isFileExist(strFullPathSticker))
	{
		CCLOG("EXIST.... FILE [%s]", strFullPathSticker.c_str());
		Sprite* spDrawImg = Sprite::create(strFullPathSticker.c_str());
		spDrawImg->setPosition(pos);
		this->addChild(spDrawImg, DEPTH_LAYER_PLAYSCENE::INIT);
	}
}


/**
* @brief  책 내용
*/
void PlayScene::loadDataForEndingPage()
{
	/*
	// 1. 이미지 로드
	1.1 칼라 이미지
	- 녹음 활동만 한 경우 또는 활동 없이 배열하기만 한 경우
	1.2 먹선 이미지
	- 색칠하기 작업을 한 경우
		
	*/
	CCLOG("loadDataForEndingPage.................");
	currImageIndex = 6;
	// 배경이미지
	Vec2 pos = Pos::getCenterPt();
	auto layerbg = Sprite::create(FILENAME_PLAY_VOLUME_BG);
	layerbg->setPosition(pos);
	layerbg->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	layerbg->setScale(BACKGROUND_SCALE);
	this->addChild(layerbg);

	// 그린 이미지 위치 수정
	//float height = 902.f / 2.f;
	pos.y = Pos::getScreenSize().height * ((SCRREN_SIZE_HEIGHT - 600.f) / SCRREN_SIZE_HEIGHT);

	MBJson* json = MBJson::getInstance();
	std::string strPath = FileUtils::getInstance()->getWritablePath();
	// 먹선 이미지 표시 여부 설정
	if (json->mLockCubes[currImageIndex])
	{
		// 고정 이미지 칼라 이미지 표시 -> 파일명 그대로
		int s = json->mDrawFileNames[currImageIndex].size();
		std::string filename = json->mDrawFileNames[currImageIndex].substr(0, s - 4);
		std::string colorFile = StringUtils::format("%s.png", filename.c_str());
		CCLOG("NOT EXT.....FILE [%s]", colorFile.c_str());

		// 배경 그림 불러오기(수정 필요)
		Sprite* spImg = Sprite::create(colorFile.c_str());
		spImg->setPosition(pos);
		this->addChild(spImg, DEPTH_LAYER_PLAYSCENE::INIT);
	}
	else
	{
		// 먹선 이미지 설정			
		setDrawLayer();
	}

	/*
	if (json->getIsDraw())
	{
		// 먹선 이미지 표시 여부 설정
		if (json->mLockCubes[currImageIndex])
		{
			// 고정 이미지 칼라 이미지 표시 -> 파일명 그대로
			int s = json->mDrawFileNames[currImageIndex].size();
			std::string filename = json->mDrawFileNames[currImageIndex].substr(0, s - 4);
			std::string colorFile = StringUtils::format("%s.png", filename.c_str());
			CCLOG("NOT EXT.....FILE [%s]", colorFile.c_str());

			// 배경 그림 불러오기(수정 필요)
			Sprite* spImg = Sprite::create(colorFile.c_str());
			spImg->setPosition(pos);
			this->addChild(spImg, DEPTH_LAYER_PLAYSCENE::INIT);
		}
		else
		{
			// 먹선 이미지 설정			
			setDrawLayer();
		}
	}
	else
	{
		// 칼라 이미지 표시
		int s = json->mDrawFileNames[currImageIndex].size();
		std::string filename = json->mDrawFileNames[currImageIndex].substr(0, s - 4);
		std::string colorFile = StringUtils::format("%s.png", filename.c_str());
		
		if (json->mLockCubes[currImageIndex])
		{
			colorFile = StringUtils::format("%s.png", filename.c_str());
		}
		else
		{
			colorFile = StringUtils::format(FILENAME_DRAW_COLOR, filename.c_str());
		}
		CCLOG("NOT EXT.....FILE [%s]", colorFile.c_str());

		// 배경 그림 불러오기(수정 필요)	
		Sprite* spImg = Sprite::create(colorFile.c_str());
		spImg->setPosition(pos);
		this->addChild(spImg, DEPTH_LAYER_PLAYSCENE::INIT);
	}
	*/
}

void PlayScene::playNarrationDelay(std::string filename, float delay)
{
	//CCLOG("playNarrationDelay.......: %s", filename.c_str());
	mFileNameNarration = filename;
	this->runAction(Sequence::create(DelayTime::create(delay),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(PlayScene::playNarration)),
		nullptr));
}

void PlayScene::playNarration(Node* pSender)
{
	//CCLOG("playNarration.......");
	int audioId = AudioEngine::play2d(mFileNameNarration.c_str());
	AudioEngine::setVolume(audioId, 1.0f);
	AudioEngine::setFinishCallback(audioId, CC_CALLBACK_0(PlayScene::finishPlayRecordCallback, this));
}


/**
* @brief  책 내용
*/
void PlayScene::loadData()
{
	/*
	// 1. 이미지 로드
	   1.1 칼라 이미지
	    - 녹음 활동만 한 경우 또는 활동 없이 배열하기만 한 경우
		1.2 먹선 이미지
		- 색칠하기 작업을 한 경우
	
	// mp3 플레이(녹음 음성)
	   1. mp3 파일이 존재한 경우는 mp3 재생

	// 인덱스 증가
	*/
	// 배경이미지
	//CCLOG("loadData.................");
	Vec2 pos = Pos::getCenterPt();
	auto layerbg = Sprite::create(FILENAME_PLAY_VOLUME_BG);
	layerbg->setPosition(pos);
	layerbg->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	layerbg->setScale(BACKGROUND_SCALE);	
	this->addChild(layerbg);

	// 그린 이미지 위치 수정
	float height = 902.f / 2.f;	
	pos.y = Pos::getScreenSize().height * ((SCRREN_SIZE_HEIGHT - 600.f) / SCRREN_SIZE_HEIGHT);

	MBJson* json = MBJson::getInstance();
	std::string strPath = FileUtils::getInstance()->getWritablePath();

	// 먹선 이미지 표시 유무(고정형 일경우 칼라 이미지 표시)
	if (json->mLockCubes[currImageIndex])
	{
		// 칼라 이미지 표시
		int s = json->mDrawFileNames[currImageIndex].size();
		std::string filename = json->mDrawFileNames[currImageIndex].substr(0, s - 4);
		std::string colorFile = StringUtils::format("%s.png", filename.c_str());
		//CCLOG("NOT EXT.....FILE [%s]", colorFile.c_str());

		// 배경 그림 불러오기(수정 필요)
		Sprite* spImg = Sprite::create(colorFile.c_str());
		spImg->setPosition(pos);
		this->addChild(spImg, DEPTH_LAYER_PLAYSCENE::INIT);
	}
	else
	{
		// 먹선 이미지 표시
		setDrawLayer();
	}
	
	//CCLOG("[loadData] WEEKDATA : %d.....", mWeekData);
	// mp3 파일 재생
	// 녹음한 파일이 있으면 녹음한 파일 재생
	std::string strFullPathNameSnd = StringUtils::format(FILENAME_SND_RECORD_VOICE_BASE_X) +
		StringUtils::format(FILENAME_SND_RECORD_VOICE_X, mWeekData, currImageIndex);
	if (FileUtils::getInstance()->isFileExist(strFullPathNameSnd.c_str()))
	{
		// 존재하는 경우 녹음 파일 재생
		//CCLOG("EXIST FILE...... %s", strFullPathNameSnd.c_str());
		
		/*int audioId = AudioEngine::play2d(strFullPathNameSnd.c_str());		
		AudioEngine::setVolume(audioId, 1.0f);		
		AudioEngine::setFinishCallback(audioId, CC_CALLBACK_0(PlayScene::finishPlayRecordCallback, this));*/
		playNarrationDelay(strFullPathNameSnd, ANI_DELAY_NARRATION);

	}
	else
	{
		// 존재하지 않는 경우는 나래이션 재생
		// 원어민 나레이션 파일 네임		
		std::string strNarFileName = StringUtils::format(FILENAME_SND_RECORD_TEXT_BASE_X) +
			StringUtils::format(FILENAME_SND_RECORD_TEXT_X, mWeekData, currImageIndex);		
		//CCLOG("NAR...... %s", strNarFileName.c_str());
		
		/*int audioId = AudioEngine::play2d(strNarFileName.c_str());				
		AudioEngine::setVolume(audioId, SND_VOLUME_MP3);		
		AudioEngine::setFinishCallback(audioId, CC_CALLBACK_0(PlayScene::finishPlayRecordCallback, this));*/
		playNarrationDelay(strNarFileName, ANI_DELAY_NARRATION);
	}

	// 10초 뒤 이동시 
	//finishPlayRecordCallback();
}


/**
* @brief  책 커버
*/
void PlayScene::loadDataCoverModify()
{
	//CCLOG("loadDataCoverModify");
	////////////////////////////////////////
	// 먹선 또는 칼라 결정
	MBJson* json = MBJson::getInstance();
	json->setIsDraw(mWeekData);
	json->currentIndex = 0;

	// 배경
	Sprite* spBg = Sprite::create(FILENAME_PLAY_COVER_BG);
	spBg->setPosition(Pos::getCenterPt());
	spBg->setScale(BACKGROUND_SCALE);
	this->addChild(spBg, DEPTH_LAYER_PLAYSCENE::INIT);
	
	////////////////////////////////////////
	// 케릭터 최초 위치 설정후 애니매이션 시작(올라오기)
	// 케릭터 설정
	// 기존에 사진이 존재하는 경우 사진으로 변경
	// 없는 경우
	loadCharModify();
	////////////////////////////////////////

	//// 회전 이미지
	mSpShine = Sprite::create(FILENAME_PLAY_SHINE_FIRST);
	Size size = mSpShine->getContentSize();
	Vec2 posS = Pos::getCenterPt();
	posS.x = size.width / 2;	
	mSpShine->setPosition(posS);
	this->addChild(mSpShine, DEPTH_LAYER_PLAYSCENE::INIT);

	//// 책 이미지
	std::string strFilename = StringUtils::format(FILENAME_PLAY_COVER_X, mWeekData);
	Sprite* spCover = Sprite::create(strFilename.c_str());
	spCover->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	spCover->setPosition(Pos::getPlayCoverModifyPt());
	this->addChild(spCover, DEPTH_LAYER_PLAYSCENE::INIT);

	// 왼쪽, 오른쪽 별
	mSpStarLeftUp = Sprite::create(FILENAME_PLAY_STAR_B);
	mSpStarLeftUp->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpStarLeftUp->setPosition(Pos::getPlayAniLeftUpStarPt());
	this->addChild(mSpStarLeftUp, DEPTH_LAYER_PLAYSCENE::INIT);

	mSpStarLeftDn = Sprite::create(FILENAME_PLAY_STAR_O);
	mSpStarLeftDn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpStarLeftDn->setPosition(Pos::getPlayAniLeftDnStarPt());
	this->addChild(mSpStarLeftDn, DEPTH_LAYER_PLAYSCENE::INIT);

	mSpStarRight = Sprite::create(FILENAME_PLAY_STAR_Y);
	mSpStarRight->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpStarRight->setPosition(Pos::getPlayAniRightStarPt());
	this->addChild(mSpStarRight, DEPTH_LAYER_PLAYSCENE::INIT);

	// 최초 숨김
	mSpStarLeftUp->setVisible(false);
	mSpStarLeftDn->setVisible(false);
	mSpStarRight->setVisible(false);

	//// 스퀀스로 처리
	/*this->runAction(Sequence::create(
		DelayTime::create(5.0f),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(PlayScene::nextScene)),
		nullptr));*/
}

void PlayScene::loadCharModify()
{
	//CCLOG("loadCharModify.................");
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	Sprite* spr;
	//Sprite* profileSp;
	std::string path;
	int rotation = 0;
	// Clipping Node
	clip = ClippingNode::create();

	// 프로필 사진 존재 확인		
	// 사진 정보 불러오기		
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID) 
	//path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE_TEMP);
	path = UserDefault::getInstance()->getStringForKey(USERDEFAULT_KEY_PROFILE_PATH);
#endif //CC_PLATFORM_ANDROID

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	std::string strPath = FileUtils::getInstance()->getWritablePath();
	path = strPath + StringUtils::format("%s", "mybook_profile.jpg");
#endif //CC_PLATFORM_WIN32

	Vec2 pos = Pos::getPlayCharModifyPt();
	if (!FileUtils::getInstance()->isFileExist(path))
	{
		//CCLOG("loadCharModify.................no profile: %f", pos.y - ANI_MOTION_POS_Y_INIT);
		// 프로필 사진이 없는 경우
		mSpChar = Sprite::create(FILENAME_PLAY_CHAR_01);
		mSpChar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);		
		mSpChar->setPosition(Vec2(pos.x, pos.y - ANI_MOTION_POS_Y_INIT));
		this->addChild(mSpChar, DEPTH_LAYER_PLAYSCENE::INIT);
		mIsExistProfilePhoto = false;
		return;
	}
	else
	{
		//CCLOG("loadCharModify.................profile");
	}

	// 프로필 사진 존재
	mIsExistProfilePhoto = true;	
	rotation = UserDefault::getInstance()->getFloatForKey(USERDEFAULT_KEY_PROFILE_ROTATION, 0.0f);

	spr = Sprite::create(path.c_str());	
	auto visibleSize = Director::getInstance()->getVisibleSize();
	float sw = visibleSize.width / spr->getBoundingBox().size.width;
	float sh = visibleSize.height / spr->getBoundingBox().size.height;
	(sw > sh) ? spr->setScale(sw) : spr->setScale(sh);
	// 프로필 사진 설정		
	spr->setPosition(visibleSize / 2);
	spr->setTag(CameraManager::IMAGE_TAG);

	// 프로필 png 사이즈 (1280, 800)
	Size sizeForContent = spr->getContentSize();

	////////////////////////////////////////
	// profile size 변경
	auto profileTexture = TextureCache::getInstance()->addImage(path.c_str());
	Vec2 profilePt = Pos::getCameraProfilePt();
	if (rotation == 0)
	{
		profilePt.y -= 70;
	}
	Size sizeForCh = Pos::getCameraProfilSizeForCharater();

	// 프로필 사진 크기
	Size realProfileSize;
	realProfileSize.width = sizeForCh.width * sizeForContent.width / spr->getBoundingBox().size.width;
	realProfileSize.height = sizeForCh.height * sizeForContent.height / spr->getBoundingBox().size.height;

	Rect profileRect = Rect(profilePt.x, profilePt.y, realProfileSize.width, realProfileSize.height);

	// 메인씬 프로필 사진 사이즈
	//Size sizeForMain = Pos::getCameraPhotoSizeForMainScene();
	Size sizeForMain = Pos::getCameraPhotoSizeForPlayScene();
	float scale = sizeForMain.width / realProfileSize.width;

	mSpProfile = Sprite::createWithTexture(profileTexture, profileRect);
	mSpProfile->setScale(scale);
	mSpProfile->setFlippedX(true);
	// 뒤집어서 촬영
	if (rotation == 180)
	{
		mSpProfile->setRotation(rotation);
	}

	TextureCache::getInstance()->removeTexture(profileTexture);

	Size contSize = mSpProfile->getContentSize();
	contSize.width = contSize.width * scale;

	////////////////////////////////////////
	// 테스트용
	pos = Pos::getPlayCharModifyProfilePt();
	mSpProfile->setPosition(Vec2(pos.x, pos.y - ANI_MOTION_POS_Y_INIT));
	if (rotation == 180)
	{
		mSpProfile->setAnchorPoint(Vec2::ANCHOR_BOTTOM_RIGHT);
	}
	else
	{
		mSpProfile->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);		
	}
	
	this->addChild(mSpProfile, DEPTH_LAYER_PLAYSCENE::INIT);

	//CCLOG("contents size: %f, %f, scale: %f", contSize.width, contSize.height, scale);
	//CCLOG("Profile Pos [%f, %f]", mSpProfile->getPosition().x, mSpProfile->getPosition().y);
	////////////////////////////////////////

	// 투명 케릭터 이미지
	mSpChar = Sprite::create(FILENAME_PLAY_CHAR_02);
	mSpChar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	pos = Pos::getPlayCharModifyPt();
	mSpChar->setPosition(Vec2(pos.x, pos.y - ANI_MOTION_POS_Y_INIT));
	this->addChild(mSpChar, DEPTH_LAYER_PLAYSCENE::INIT);
	//CCLOG("END............Char [%f, %f], Profile[%f, %f]", mSpChar->getPosition().x, mSpChar->getPosition().y,
	//	mSpProfile->getPosition().x, mSpProfile->getPosition().y);
}

// 삭제 예정
void PlayScene::loadChar()
{
	//CCLOG("loadChar.................");
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	
	Sprite* spr;
	Sprite* profileSp;	
	std::string path;	
	int rotation = 0;
	// Clipping Node
	clip = ClippingNode::create();
	
	// 프로필 사진 존재 확인		
	// 사진 정보 불러오기		
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID) 
	//path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE_TEMP);
	path = UserDefault::getInstance()->getStringForKey(USERDEFAULT_KEY_PROFILE_PATH);
#endif //CC_PLATFORM_ANDROID

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	std::string strPath = FileUtils::getInstance()->getWritablePath();
	path = strPath + StringUtils::format("%s", "mybook_profile.jpg");
#endif //CC_PLATFORM_WIN32

	Vec2 pos = Pos::getPlayCharModifyPt();
	if (!FileUtils::getInstance()->isFileExist(path))	
	{
		//CCLOG("loadChar.................no profile");
		// 프로필 사진이 없는 경우
		mSpChar = Sprite::create(FILENAME_PLAY_CHAR_01);
		mSpChar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);		
		mSpChar->setPosition(pos);
		this->addChild(mSpChar, DEPTH_LAYER_PLAYSCENE::INIT);
		mIsExistProfilePhoto = false;
		return;
	}
	else
	{
		//CCLOG("loadChar.................profile");		
	}

	// 프로필 사진 존재
	mIsExistProfilePhoto = true;

	rotation = UserDefault::getInstance()->getFloatForKey(USERDEFAULT_KEY_PROFILE_ROTATION, 0.0f);
	spr = Sprite::create(path.c_str());
	auto visibleSize = Director::getInstance()->getVisibleSize();
	float sw = visibleSize.width / spr->getBoundingBox().size.width;
	float sh = visibleSize.height / spr->getBoundingBox().size.height;
	(sw > sh) ? spr->setScale(sw) : spr->setScale(sh);
	// 프로필 사진 설정		
	spr->setPosition(visibleSize / 2);
	spr->setTag(CameraManager::IMAGE_TAG);

	// 프로필 png 사이즈 (1280, 800)
	Size sizeForContent = spr->getContentSize();

	////////////////////////////////////////
	// profile size 변경
	auto profileTexture = TextureCache::getInstance()->addImage(path.c_str());
	Vec2 profilePt = Pos::getCameraProfilePt();
	Size sizeForCh = Pos::getCameraProfilSizeForCharater();

	// 프로필 사진 크기
	Size realProfileSize;
	realProfileSize.width = sizeForCh.width * sizeForContent.width / spr->getBoundingBox().size.width;
	realProfileSize.height = sizeForCh.height * sizeForContent.height / spr->getBoundingBox().size.height;

	Rect profileRect = Rect(profilePt.x, profilePt.y, realProfileSize.width, realProfileSize.height);

	// 메인씬 프로필 사진 사이즈
	//Size sizeForMain = Pos::getCameraPhotoSizeForMainScene();
	Size sizeForMain = Pos::getCameraPhotoSizeForPlayScene();
	float scale = sizeForMain.width / realProfileSize.width;
	
	profileSp = Sprite::createWithTexture(profileTexture, profileRect);
	profileSp->setScale(scale);
	//profileSp->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	//profileSp->setPosition(Pos::getCameraPhotoPt());
	profileSp->setFlippedX(true);
	// 뒤집어서 촬영
	if (rotation == 180)
	{
		profileSp->setRotation(rotation);
	}
	
	TextureCache::getInstance()->removeTexture(profileTexture);

	Size contSize = profileSp->getContentSize();
	contSize.width = contSize.width * scale;
	//CCLOG("contents size: %f, %f, scale: %f", contSize.width, contSize.height, scale);

	////////////////////////////////////////
	// Stencil
	// setup stencil shape
	DrawNode* shape = DrawNode::create();
	// drawCircle(center, radius, angle, segments, drawLineToCenter, lineWidth, color)	
	shape->drawSolidCircle(Pos::getCenterPt(), contSize.width / 2, CC_DEGREES_TO_RADIANS(0), 200, true, 1, Color4F(1, 0, 0, 1));

	// add shape in stencil		
	clip->setStencil(shape);
	clip->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	//clip->setPosition(ccp(origin.x, origin.y));

	//pos = Pos::getCameraPhotoPtForProfile();
	pos = Pos::getPlayCharModifyPt();
	pos.x = (pos.x - Pos::getScreenSize().width / 2) + contSize.width / 2 + 75.0f;
	pos.y = (pos.y - Pos::getScreenSize().height / 2) - contSize.width / 2 - 95.0f;
	// 프로필 위치 설정
	clip->setPosition(pos);
	// setup content		
	profileSp->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	clip->addChild(profileSp);	

	/*CCLOG("POS : clip [%f, %f], profileSp [%f, %f]", clip->getPosition().x, clip->getPosition().y,
		profileSp->getPosition().x, profileSp->getPosition().y);*/
	
	this->addChild(clip, DEPTH_LAYER_PLAYSCENE::INIT);

	// 투명 케릭터 이미지
	//mSpChar = Sprite::create();
	mSpChar = Sprite::create(FILENAME_PLAY_CHAR_02);
	mSpChar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpChar->setPosition(Pos::getPlayCharModifyPt());
	this->addChild(mSpChar, DEPTH_LAYER_PLAYSCENE::INIT);	
}


/**
* @brief  메인씬으로 이동
*/
void PlayScene::createUi()
{
	//CCLOG("createUi.................");

	createNextBtn(this);
}


void PlayScene::createNextBtn(Node* pSender)
{
	// 메인씬으로 돌아가기 버튼
	mBtnBack = MenuItemImage::create(FILENAME_BTN_CLOSE_N,
		FILENAME_BTN_CLOSE_P,
		CC_CALLBACK_1(PlayScene::onBackBtnClick, this));
	mBtnBack->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mBtnBack->setPosition(Pos::getBackBtnPt());

	mMenuBack = Menu::create(mBtnBack, nullptr);
	mMenuBack->setPosition(Vec2::ZERO);
	this->addChild(mMenuBack, DEPTH_LAYER_DRAW_INIT_LAYOUT);
}


/*
1. 이미지 로드
2. 녹음 파일 로드
*/
// on "init" you need to initialize your instance
bool PlayScene::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}
	//CCLOG("PlayScene init............................");
	//loadDataCover();

	return true;
}


/**
* @brief  
* @return void
*/
void PlayScene::playAni()
{
	Device::setKeepScreenOn(true);
	//CCLOG("play ANi.........");
	/*
	1. 케릭터 올라옴
	2. 별 반짝임
	*/	
	//CCLOG("play ANi.........2");
	this->runAction(Sequence::create(	
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionSpine, this)),
		DelayTime::create(0.25f),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::playNarrationTitle, this)),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionCharJump, this)),		
		DelayTime::create(1.1f),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionChar, this)),
		DelayTime::create(ANI_INTERVAL_CHAR_EFFECT),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionCharEffect, this)),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionStar, this)),
		DelayTime::create(4.0f),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::nextScene, this)),
		nullptr));	
}


/**
* @brief  권별 나레이션 재생
* @return void
*/
void PlayScene::playNarrationTitle(Node* pSender)
{
	// 권별 나레이션 재생
	std::string strFileName = StringUtils::format(FILENAME_SND_PLAY_VALUME_X, mWeekData);
	auto audio = SimpleAudioEngine::getInstance();
	//audio->setEffectsVolume(SND_VOLUME_MP3);
	audio->playEffect(strFileName.c_str());
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionSpine(Node* pSender)
{
	//CCLOG("Motion Spine........");
	
	auto seq = Sequence::create(CallFuncN::create(CC_CALLBACK_1(PlayScene::motionSpineFirst, this)),
		DelayTime::create(0.15f),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionSpineSecond, this)),
		DelayTime::create(0.15f),
		nullptr);

	mSpShine->runAction(RepeatForever::create(seq));	
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionSpineFirst(Node* pSender)
{
	// FILENAME_PLAY_SHINE_FIRST, FILENAME_PLAY_SHINE_SECOND	
	mSpShine->setTexture(TextureCache::getInstance()->addImage(FILENAME_PLAY_SHINE_SECOND));
}

/**
* @brief  모션
* @return void
*/
void PlayScene::motionSpineSecond(Node* pSender)
{
	mSpShine->setTexture(TextureCache::getInstance()->addImage(FILENAME_PLAY_SHINE_FIRST));
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionCharJump(Node* pSender)
{
	//CCLOG("motionCharJump.....");
	Vec2 posj = Pos::getPlayCharModifyPt();
	//mSpChar->runAction(EaseInOut::create(MoveTo::create(1.0f, posj), 3.0f));
	mSpChar->runAction(EaseBackOut::create(MoveTo::create(1.1f, posj)));
	
	// 프로필 사진이 존재하는 경우 모션 처리
	if (mIsExistProfilePhoto)
	{		
		// 테스트
		posj = Pos::getPlayCharModifyProfilePt();
		mSpProfile->runAction(EaseBackOut::create(MoveTo::create(1.1f, posj)));
	}
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionChar(Node* pSender)
{
	//CCLOG("motionChar.................");
	// 스퀀스로 처리
	Vec2 pos = Pos::getPlayCharModifyPt();
	Vec2 posd = pos;
	pos.y += ANI_MOTION_POS_UNIT;
	posd.y -= ANI_MOTION_POS_UNIT;
	auto seq = Sequence::create(EaseInOut::create(MoveTo::create(1.0f, pos), 1.0f),
		DelayTime::create(0.1f),		
		EaseInOut::create(MoveTo::create(1.0f, posd), 1.0f),
		nullptr);	
	//CCLOG("motionChar.................");
	// 프로필 (clipping)
	/*pos = clip->getPosition();
	//pos = mSpProfile->getPosition();	
	posd = pos;
	pos.y += ANI_MOTION_POS_UNIT;
	posd.y -= ANI_MOTION_POS_UNIT;
	auto seqClip = Sequence::create(EaseInOut::create(MoveTo::create(1.0f, pos), 1.0f),
		DelayTime::create(0.1f),
		EaseInOut::create(MoveTo::create(1.0f, posd), 1.0f),
		nullptr);*/
	
	// 테스트를 위해서 커멘트 처리 했음.... 다시 원복해야 됨~!!!!!!!
	mSpChar->runAction(RepeatForever::create(seq));

	// 프로필 사진이 존재하는 경우 모션 처리
	if (mIsExistProfilePhoto)
	{		
		//clip->runAction(RepeatForever::create(seqClip));
		// 프로필 (No clipping)
		pos = mSpProfile->getPosition();
		posd = pos;
		pos.y += ANI_MOTION_POS_UNIT;
		posd.y -= ANI_MOTION_POS_UNIT;
		auto seqProfile = Sequence::create(EaseInOut::create(MoveTo::create(1.0f, pos), 1.0f),
			DelayTime::create(0.1f),
			EaseInOut::create(MoveTo::create(1.0f, posd), 1.0f),
			nullptr);
		// 테스트
		mSpProfile->runAction(RepeatForever::create(seqProfile));
	}	
	//CCLOG("motionChar.................END");
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionCharEffect(Node* pSender)
{
	//CCLOG("motionCharEffect.................");
	// 왼쪽, 오른쪽 
	mSpCharEffect = Sprite::create(FILENAME_PLAY_EFFECT_01);
	mSpCharEffect->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpCharEffect->setPosition(Pos::getPlayAniArmEffectPt());
	this->addChild(mSpCharEffect, DEPTH_LAYER_PLAYSCENE::INIT);
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionStar(Node* pSender)
{
	//CCLOG("motionStar.................");
	// Blink
	mSpStarLeftUp->setVisible(true);
	mSpStarLeftDn->setVisible(true);
	mSpStarRight->setVisible(true);

	aniEffectIndex = 0;

	this->runAction(Sequence::create(		
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionStarEffect, this)),
		DelayTime::create(ANI_INTERVAL_CHAR_EFFECT),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionStarEffect, this)),
		DelayTime::create(ANI_INTERVAL_CHAR_EFFECT),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::motionStarEffect, this)),
		nullptr));
}


/**
* @brief  모션
* @return void
*/
void PlayScene::motionStarEffect(Node* pSender)
{	
	Blink* blink = Blink::create(TIME_MOTION_STAR_BLINK, 2);	
	Sequence* action = Sequence::create(Blink::create(TIME_MOTION_STAR_BLINK, 2), 
		FadeOut::create(1.0f),
		nullptr);

	Sequence* actionCharEffect = Sequence::create(DelayTime::create(TIME_MOTION_STAR_BLINK),
		FadeOut::create(1.0f),
		nullptr);

	switch (aniEffectIndex)
	{
	case 0:		
		mSpStarLeftDn->runAction(action);
		break;

	case 1:
		mSpStarLeftUp->runAction(action);		
		break;

	case 2:
		mSpStarRight->runAction(action);
		// 팔효과 사라짐
		mSpCharEffect->runAction(actionCharEffect);
		break;
	} // end of switch
	
	aniEffectIndex++;
}


// 최초 진입시 또는 엔딩팝업에서 진입 처리 
void PlayScene::startPlay()
{
	std::string strPath = MBJson::getInstance()->getPorfolioPath();
	//std::string strPath = StringUtils::format("start/path");
	//CCLOG("startPlay..........%d, %s", MBJson::getInstance()->mCallingPageIndex , strPath.c_str());
	// JNI CALL	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)	
	if (MBJson::getInstance()->mCallingPageIndex == PLAYSCENE_CALLING::ENDING_POPUP)
	{
		VideoRecordManager::getInstance()->startForEndingPage(this, mWeekData, strPath);
	}
	else
	{
		VideoRecordManager::getInstance()->start(this, mWeekData, strPath);
	}	
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


void PlayScene::stopPlay()
{
	//CCLOG("stopPlay..........");
	// JNI CALL
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)	
	VideoRecordManager::getInstance()->stop();
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


/**
* @brief  돌아가기 버튼 이벤트
* @return void
*/
void PlayScene::onBackBtnClick(Ref* sender)
{
	//CCLOG("onBackBtnClick..........");
	stopPlay();

	/*mPopup = PopupLayer::getIns(this, PLAY_CLOSE);
	((PopupLayer*)mPopup)->showHide(true, false);*/	
	// 메인 씬으로 이동
	//menuPopupClose();
	closePlay();
}


/**
* @brief  메인씬으로 이동
*/
void PlayScene::toScene(float dt)//(Node* pSender)
{
	//CCLOG("TO SCENE:;:....");		

	auto scene = MainScene::createScene();
	// run
	Director::getInstance()->replaceScene(scene);
}


/**
* @brief 레이아웃 초기화
* @return void
*/
void PlayScene::initLayout()
{
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin      = Director::getInstance()->getVisibleOrigin();
}


/**
* @brief 레이어 닫기
* @return void
*/
void PlayScene::closePlay()
{
	//CCLOG("closePlay...............");
	// 노티 보내기 -> 메인씬에서 레이어 닫기
	String* pNotiParam = String::create(StringUtils::format("%d", NOTI_CODE_PLAY_CLOSE));
	NotificationCenter::getInstance()->postNotification(NOTI_STR_VOICE_RECORD, pNotiParam);
}


/**
* @brief 레이어 닫기(캡처 시스템 팝업에서 취소시)
* @return void
*/
void PlayScene::resumePlayEndingPage()
{
	//CCLOG("resumePlayEndingPage...............");		
	
	this->removeChild(mLayer);
	// ending에서 호출시 
	loadDataForEndingPage();

	// Ending Popup(엔딩 옵션 팝업)
	mPopup = PopupLayer::getIns(this, PLAY_ENDING);
	mPopup->showHide(true);
	//Director::getInstance()->popScene();
	
	// 노티 보내기 -> 메인씬에서 레이어 유지(엔딩 팝업 유지)
	/*String* pNotiParam = String::create(StringUtils::format("%d", NOTI_CODE_PLAY_ENDING_CANCEL));
	NotificationCenter::getInstance()->postNotification(NOTI_STR_VOICE_RECORD, pNotiParam);*/
}


// 팝업 닫기
void PlayScene::menuPopupClose()
{
	//CCLOG("menuPopupClose.........");
	//setEnableTouchBtn(true);
}


void PlayScene::onExit()
{
	// 상위 클래스 호출
	Layer::onExit();	
	//CCLOG("[PLAYSCENE] onExit");
	//this->stopAllActions();	
}


/**
* @brief 콜백 함수(녹음한 음성 재생 완료후)
* @return void
*/
void PlayScene::finishPlayRecordCallback()//int id, const std::string &file)
{	
	//CCLOG("finishPlayRecordCallback........%d", currImageIndex);
	// 스퀀스로 처리	
	// 음원 재생 완료후 2초후 페이지 이동
	this->runAction(Sequence::create(		
		DelayTime::create(SND_PLAY_CORRECTION_TIME),
		CallFuncN::create(CC_CALLBACK_1(PlayScene::nextScene, this)),
		nullptr));

	// 10초 후 이동
	/*this->runAction(Sequence::create(
		DelayTime::create(ANI_INTERVAL_SCENE),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(PlayScene::nextScene)),
		nullptr));*/
}

void PlayScene::nextSceneSndEffect(Node* pSender)
{
	AudioEngine::play2d(FILENAME_SND_EFFECT_PLAY_TRANSITION);
}

/**
* @brief 콜백 함수(녹음한 음성 재생 완료후)
* @return void
*/

void PlayScene::nextScene(Node* pSender)
{	
	if (DEBUG)
	{
		auto audio = SimpleAudioEngine::getInstance();
		audio->setEffectsVolume(1.0f);

		stopPlay();

		// Ending Popup(엔딩 옵션 팝업) -> 삭제
		/*mPopup = PopupLayer::getIns(this, PLAY_ENDING);
		mPopup->showHide(true);*/
		toEndingScene();
		return;
	}
	// 다음 씬으로 이동
	// 넥스트		
	currImageIndex = ++MBJson::getInstance()->currentIndex;
	//CCLOG("nextScene.....%d", currImageIndex);

	if (currImageIndex > SIZE_CUBE)
	{
		//CCLOG("STOP MP4 PLAY.........: %d", currImageIndex);
		// 원래대로 소리 복원
		auto audio = SimpleAudioEngine::getInstance();
		audio->setEffectsVolume(1.0f);		

		stopPlay();

		// Ending Popup(엔딩 옵션 팝업) -> 삭제
		/*mPopup = PopupLayer::getIns(this, PLAY_ENDING);
		mPopup->showHide(true);*/
		toEndingScene();
		return;
	}

	checkPreloadingDelay();

	//nextSceneSndEffect(this);

	// 원복 해야 됨...
	Director::getInstance()->setDepthTest(true);
	Director::getInstance()->pushScene(TransitionPageTurn::create(PLAYSCENE_PAGE_TRANSITION_DELAY, PlayScene::scene(currImageIndex), false));
}


// 프리 로딩
void PlayScene::checkPreloadingDelay()
{
	MBJson* json = MBJson::getInstance();
	std::string strPath = FileUtils::getInstance()->getWritablePath();

	if (json->mLockCubes[currImageIndex])
	{
		delayTimeSndEffect = 0.0f;
	}
	else
	{
		delayTimeSndEffect = 0.0f;
		//  사용자가 그림
		std::string strFullPath = MBJson::getInstance()->getDrawFileName(mWeekData, currImageIndex);
		if (FileUtils::getInstance()->isFileExist(strFullPath))
		{
			//delayTimeSndEffect += 0.10f;
		}

		//  사용자가 그린 스티커
		std::string strFullPathSticker = MBJson::getInstance()->getDrawStickerFileName(mWeekData, currImageIndex);
		if (FileUtils::getInstance()->isFileExist(strFullPathSticker))
		{
			//delayTimeSndEffect += 0.10f;
		}		
	}
	//CCLOG("=============================DELAY TIME : %f", delayTimeSndEffect);

	this->runAction(Sequence::create(
		DelayTime::create(delayTimeSndEffect),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(PlayScene::nextSceneSndEffect)),
		nullptr));
}


/**
* @brief replay
* @return void
*/
void PlayScene::replayPlayScene()
{
	//CCLOG("replayPlayScene..................... POPUP CLOSE");
	// 팝업 닫기 -> 리턴 실패일때 호출
	//mPopup->showHide(false, false);
		
	MBJson::getInstance()->currentIndex = 1;
	// 콜링 페이지 인덱스 설정
	MBJson::getInstance()->mCallingPageIndex = PLAYSCENE_CALLING::ENDING_POPUP;

	mLayer = PlayScene::createLayer(this, mWeekData);
	this->addChild(mLayer, DEPTH_LAYER_RECORD);

	//CCLOG("replayPlayScene..................... POPUP CLOSE END...");
}


/**
* @brief toEndingScene
* @return void
*/
void PlayScene::toEndingScene()
{
	// 엔딩 씬으로 이동
	//CCLOG("toEndingScene......");	

	//Director::getInstance()->setDepthTest(true);
	Scene* s = EndingScene::createScene();
	//Director::getInstance()->replaceScene(s);	
	Director::getInstance()->pushScene(s);
}


void PlayScene::setBtnMainScene(bool b)
{
	//CCLOG("setBtnMainScene......");
	MainScene* scene = (MainScene*)MBJson::getInstance()->getMainScene();
	((MainScene*)scene)->setEnableAllCubeButton(b);
	((MainScene*)scene)->setEnableTouchBtn(b);
}


/*
void PlayScene::onEnter()
{
	Layer::onEnter();
	CCLOG("onEnter.......: %d", currImageIndex);
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(true);
	listener1->onTouchBegan = [&](Touch* touch, Event* event)
	{
	pointStartPos = touch->getLocation();
	return true;
	};

	listener1->onTouchMoved = [&](Touch* touch, Event* event)
	{
	};

	listener1->onTouchEnded = [=](Touch* touch, Event* event)
	{
	pointEndPos = touch->getLocation();
	bool is_left;
	unsigned int i_index = this->currImageIndex;
	if (pointEndPos.x - pointStartPos.x >0)
	{
	is_left = true;
	i_index++;
	}
	else
	{
	is_left = false;
	i_index--;
	}

	if (i_index == 0) i_index = 3;//myImages.size();
	if (i_index > myImages.size()) i_index = 1;
	this->currImageIndex = i_index;
	//SimpleAudioEngine::sharedEngine()->playEffect("record.mp3");
	Director::getInstance()->setDepthTest(true);
	//Director::getInstance()->replaceScene(TransitionPageTurn::create(2.f, PlayScene::scene(i_index), is_left));
	Director::getInstance()->pushScene(TransitionPageTurn::create(2.f, PlayScene::scene(i_index), is_left));

	//if (is_left)Director::getInstance()->replaceScene(TransitionSlideInL::create(0.5f, HelloWorld::scene(i_index)));
	//else Director::getInstance()->replaceScene(TransitionSlideInR::create(0.5f, HelloWorld::scene(i_index)));
	};
	auto dispatcher = Director::getInstance()->getEventDispatcher();
	dispatcher->addEventListenerWithSceneGraphPriority(listener1, this);
	
}*/