#include "MainScene.h"
#include "DrawScene.h"
#include "PlayScene.h"
#include "Layer/RecordLayer.h"
#include "Layer/PopupLayer.h"
#include "Contents/MyBookSnd.h"

#include "mg_common/utils/MSLPManager.h"

#include  "Manager/ScreenManager.h"

// 버튼 및 그림 앵글 각도
//float angleForCube[SIZE_CUBE] = { 2.0f, 1.0f, 0.0f, -0.4f, -1.8f, -2.0f }; 
float angleForCube[SIZE_CUBE] = { 2.0f, 1.0f, 0.0f, -0.75f, -1.8f, -2.0f };
// for AudioEngine
using namespace cocos2d::experimental;

Scene* MainScene::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = MainScene::create();
	
	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}


// on "init" you need to initialize your instance
bool MainScene::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}
	
	////////////////////////////////////////
	// 씬 초기화
	// 호 번호
	mWeekData = MBJson::getInstance()->getWeekData();
	// 최초 집입 애니시 터치 불가
	mIsPlayCubeAni = true;
	mTouchFlag = false;
	////////////////////////////////////////
	// 노티피케이션 설정
	NotificationCenter::getInstance()->addObserver(this, 
												   callfuncO_selector(MainScene::doNotification), 
												   NOTI_STR_VOICE_RECORD, 
												   NULL);
	////////////////////////////////////////
	// 스크린 사이즈 구하기
	mScreenSize = Pos::getScreenSize();

	initBGM();

	readJsonFile();
	initCube();

	createUi();	
	initCamera();
	// 레이아웃 초기화
	initLayout();
	// 터치 초기화
	initTouchEvent();

	// 프로필 사진 설정
	//std::string path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE_TEMP);
	std::string path = UserDefault::getInstance()->getStringForKey(USERDEFAULT_KEY_PROFILE_PATH);
	if (isExistProfile(path))
	{
		initProfile();
	}
	else
	{
		createProfileWithUni();
	}	
	
	//initGaf();
	aniReorderCube();

	// 시작 나레이션
	playNarStart(this, 0);

	return true;
}


//------------------------------------------------------------
// 사운드 
//------------------------------------------------------------	
void MainScene::initBGM()
{
	SimpleAudioEngine::getInstance()->playBackgroundMusic(FILENAME_SND_BGM_DRAW, true);
}

/**
* @brief  나레이션 플레이
* @return void
*/
void MainScene::playNarStart(Node* pSender, int idx)
{
	//CCLOG("play nar start......");
	// 시작 효과음 재생
	//playNarSnd(0);
	int id;
	mNarIndex = idx;

	if (idx == 0)
	{
		id = AudioEngine::play2d(FILENAME_SND_CUBE_NAR1);
	}
	else
	{
		id = AudioEngine::play2d(FILENAME_SND_CUBE_NAR2);
	}
	
	AudioEngine::setFinishCallback(id, CC_CALLBACK_0(MainScene::finishNarTextCallback, this));
}


/**
* @brief 나레이션 완료 콜백 함수
*/
void MainScene::finishNarTextCallback()//int id, const std::string &file)
{
	//CCLOG("finishNarTextCallback........");
	if (mNarIndex == 0)
	{
		// 다음 나레이션
		// 1초뒤 나레이션으로 변경
		//playNarSnd(mNarIndex + 1);
	}
	else
	{
		// 종료
		mNarIndex = INVALID_INDEX;
		mHelpItem->setVisible(true);
		mHelpItem->setEnabled(true);

		processUserTouch();
		// 버튼 터치 허용
		//mUseTouch = true;

		// 녹음 버튼 활성화
		/*for (int i = 0; i < SIZE_CUBE; i++)
		{
			setEnableRecordButtonWithIndex(mBtnRecordInfo[i], i);
		}*/
		
	}
	// 활성화
	mCameraItem->setEnabled(true);

}

//------------------------------------------------------------
// 노티피케이션 
//------------------------------------------------------------	
void MainScene::doNotification(Ref* obj)
{
	// 노티
	String *pParam = (String*)obj;	
	//CCLOG("doNotification : %s", pParam->getCString());
	int notiCode = pParam->intValue();
	switch (notiCode)
	{
	case NOTI_CODE_VOICE_RECORD_CLOSE:
		// 보이스 레이어 닫기
		closeRecordLayer();		
		break;

	case NOTI_CODE_VOICE_RECORD_HELP:
		break;

	case NOTI_CODE_PLAY_CLOSE:
		closePlayLayer();		
		break;

	case NOTI_CODE_PLAY_ENDING_CANCEL:
		resumePlayLayerEndingPage();
		break;		
	} // case of switch
}


//------------------------------------------------------------
// 초기화 
//------------------------------------------------------------	
/**
* @brief 프로필 사진 초기화(유니)
* @return void
*/
void MainScene::createProfileWithUni()
{
	//CCLOG("createProfileWithUni...........");

	// Path
	std::string path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE_UNI);
	CC_ASSERT(FileUtils::getInstance()->isFileExist(path));

	//CCLOG("PATH : %s", path.c_str());

	Sprite* spr = Sprite::create(path.c_str());
	spr->setPosition(Pos::getCenterPt());	
	
	////////////////////////////////////////
	// profile size 변경
	auto profileTexture = TextureCache::getInstance()->addImage(path.c_str());	

	// 프로필 사진 크기
	Size realProfileSize = spr->getContentSize();	
	Rect profileRect = Rect(0, 0, realProfileSize.width, realProfileSize.height);

	// 메인씬 프로필 사진 사이즈
	Size sizeForMain = Pos::getCameraPhotoSizeForMainScene();
	float scale = sizeForMain.width / realProfileSize.width;

	/*CCLOG("profilePos : %f, %f, ratioForCH : %f, %f, Scale:%f",
		0, 0, realProfileSize.width, realProfileSize.height, scale);*/

	auto profileSp = Sprite::createWithTexture(profileTexture, profileRect);
	profileSp->setScale(scale);
	profileSp->setFlippedX(true);

	Size contSize = profileSp->getContentSize();
	contSize.width = contSize.width * scale;

	////////////////////////////////////////
	// Stencil
	// setup stencil shape
	DrawNode* shape = DrawNode::create();
	// drawCircle(center, radius, angle, segments, drawLineToCenter, lineWidth, color)	
	shape->drawSolidCircle(Pos::getCenterPt(), contSize.width / 2, CC_DEGREES_TO_RADIANS(0), 200, true, 1, Color4F(1, 0, 0, 1));

	// add shape in stencil
	ClippingNode* clip = ClippingNode::create();
	clip->setStencil(shape);
	clip->setAnchorPoint(Vec2::ANCHOR_MIDDLE);	

	Vec2 pos = Pos::getCameraPhotoPtForProfile();
	pos.x = (pos.x - Pos::getScreenSize().width / 2) + contSize.width / 2;
	pos.y = (pos.y - Pos::getScreenSize().height / 2) - contSize.width / 2;
	// 프로필 위치 설정
	clip->setPosition(pos);

	// setup content		
	auto visibleSize = Director::getInstance()->getVisibleSize();
	profileSp->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	clip->addChild(profileSp);

	mCameraLayer->addChild(clip);
}


/**
* @brief 프로필 사진 초기화
* @return void
*/
void MainScene::initProfile()
{
	//CCLOG("initProfile...........");
	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID ) 
	// Path
	//std::string path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE_TEMP);
	std::string path = UserDefault::getInstance()->getStringForKey(USERDEFAULT_KEY_PROFILE_PATH);
	int rotation = 0;
	rotation = UserDefault::getInstance()->getFloatForKey(USERDEFAULT_KEY_PROFILE_ROTATION, 0.0f);
	CC_ASSERT(FileUtils::getInstance()->isFileExist(path));
	
	//CCLOG("PATH : %s", path.c_str());

	Sprite* spr = Sprite::create(path.c_str());
	auto visibleSize = Director::getInstance()->getVisibleSize();

	float sw = visibleSize.width / spr->getBoundingBox().size.width;
	float sh = visibleSize.height / spr->getBoundingBox().size.height;
	(sw > sh) ? spr->setScale(sw) : spr->setScale(sh);
	// 프로필 사진 설정
	spr->setPosition(visibleSize / 2);
	spr->setTag(CameraManager::IMAGE_TAG);

	//log("Captured Photo size [%f, %f]", spr->getBoundingBox().size.width, spr->getBoundingBox().size.height);
	//log("Captured Photo Contents size : [%f, %f]", spr->getContentSize().width, spr->getContentSize().height);

	////////////////////////////////////////
	// profile size 변경
	auto profileTexture = TextureCache::getInstance()->addImage(path.c_str());
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
	// 프로필 png 사이즈 (1280, 800)
	Size sizeForContent = spr->getContentSize();

	realProfileSize.width = sizeForCh.width * sizeForContent.width / spr->getBoundingBox().size.width;
	realProfileSize.height = sizeForCh.height * sizeForContent.height / spr->getBoundingBox().size.height;

	Rect profileRect = Rect(profilePt.x, profilePt.y, realProfileSize.width, realProfileSize.height);

	// 메인씬 프로필 사진 사이즈
	Size sizeForMain = Pos::getCameraPhotoSizeForMainScene();
	float scale = sizeForMain.width / realProfileSize.width;

	//log("profilePos : %f, %f, width:height[%f, %f], ratioForCH : %f, %f, Scale:%f",
	//	profilePt.x, profilePt.y, sizeForCh.width, sizeForCh.height, realProfileSize.width, realProfileSize.height, scale);

	auto profileSp = Sprite::createWithTexture(profileTexture, profileRect);
	profileSp->setScale(scale);
	//profileSp->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	//profileSp->setPosition(Pos::getCameraPhotoPt());
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
	// drawCircle(center, radius, angle, segments, drawLineToCenter, lineWidth, color)	
	shape->drawSolidCircle(Pos::getCenterPt(), contSize.width / 2, CC_DEGREES_TO_RADIANS(0), 200, true, 1, Color4F(1, 0, 0, 1));

	// add shape in stencil
	ClippingNode* clip = ClippingNode::create();
	clip->setStencil(shape);
	clip->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	//clip->setPosition(ccp(origin.x, origin.y));

	Vec2 pos = Pos::getCameraPhotoPtForProfile();
	pos.x = (pos.x - Pos::getScreenSize().width / 2) + contSize.width / 2;
	pos.y = (pos.y - Pos::getScreenSize().height / 2) - contSize.width / 2;
	// 프로필 위치 설정
	clip->setPosition(pos);

	// setup content		
	profileSp->setPosition(Vec2(visibleSize.width / 2 , visibleSize.height / 2 ));
	clip->addChild(profileSp);

	mCameraLayer->addChild(clip);
#endif // CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID 
}


// 존재하면 true, 존재하지 않으면 false
bool MainScene::isExistProfile(std::string strFilePath)
{	
	if (!FileUtils::getInstance()->isFileExist(strFilePath))
	{
		//CCLOG("initProfile empty....[%s]", strFilePath.c_str());
		return false;
	}
	//CCLOG("initProfile exist....[%s]", strFilePath.c_str());
	return true;
}

/**
* @brief  랜덤 넘버 생성
* @return void
*/
void MainScene::GenRandIndex()
{
	// 랜덤 넘버 초기화
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		mRandNumber[i] = INVALID_INDEX;
	} // end of for

	// 랜덤 넘버 설정
	int maxNumber = SIZE_USER_CUBE;
	int r;
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		if (mMBJson->mLockCubes[i + 1])
		{
			// 고정형 이미지일 경우 스킵
			continue;
		}

		r = (rand() % (maxNumber));

		// 배열내에 중복 되는지 체크
		while (checkRandNumber(r, i))
		{
			r = (rand() % (maxNumber));
		} // end of while
		mRandNumber[i] = r;
		//CCLOG("[RAND] [%d, %d]", i, r);
	} // end of for	
}


/**
* @brief  버튼 생성
* @return void
*/
void MainScene::createUi()
{
	Size screenSize = Pos::getScreenSize();

	////////////////////////////////////////
	// 배경
	Sprite* bg = Sprite::create(FILENAME_BG_MAINSCENE);
	bg->setPosition(Pos::getCenterPt());	
	bg->setScale(BACKGROUND_SCALE);
	this->addChild(bg);

	// title (인덱스는 1부터)
	std::string titleFileName = StringUtils::format(FILENAME_CUBE_TITLE_X, (mWeekData));	
	
	Sprite* bgTitle = Sprite::create(titleFileName.c_str());
	bgTitle->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	bgTitle->setPosition(Vec2(0, screenSize.height));
	this->addChild(bgTitle);

	// 집게
	Sprite* bgTongs = Sprite::create(FILENAME_BG_MAINSCENE_TONGS);
	bgTongs->setPosition(Pos::getCenterPt());
	this->addChild(bgTongs, DEPTH_LAYER_CUBE_PLAY_REORDER);

	////////////////////////////////////////
	// 닫기 버튼
	auto quitItem = MenuItemImage::create(
		FILENAME_BTN_QUIT_N, FILENAME_BTN_QUIT_P,
		CC_CALLBACK_1(MainScene::menuCloseCallback, this));
	quitItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	quitItem->setPosition(Pos::getBackBtnPt());

	auto menuQuit = Menu::create(quitItem, NULL);
	menuQuit->setPosition(Vec2::ZERO);
	this->addChild(menuQuit, DEPTH_LAYER_CUBE, BTN_TAG::BACK);

	////////////////////////////////////////
	// 도움말 버튼
	mHelpItem = MenuItemImage::create(
		FIlENAME_BTN_HELP_N, FIlENAME_BTN_HELP_P,
		CC_CALLBACK_1(MainScene::menuHelpCallback, this));
	mHelpItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mHelpItem->setPosition(Pos::getHelpBtnPt());

	// create menu, it's an autorelease object
	auto menuHelp = Menu::create(mHelpItem, NULL);
	menuHelp->setPosition(Vec2::ZERO);
	this->addChild(menuHelp, DEPTH_LAYER_CUBE, BTN_TAG::HELP);
	// 최초 비활성화
	mHelpItem->setVisible(false);
	mHelpItem->setEnabled(false);

	////////////////////////////////////////
	// 카메라 버튼 처리	
	mCameraItem = MenuItemImage::create(
		FILENAME_CUBE_BTN_CAMERA_N, FILENAME_CUBE_BTN_CAMERA_P,
		CC_CALLBACK_1(MainScene::menuCameraCallback, this));
	mCameraItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mCameraItem->setPosition(Pos::getCameraBtnPt());

	// create menu, it's an autorelease object
	auto menu = Menu::create(mCameraItem, NULL);
	menu->setPosition(Vec2::ZERO);
	this->addChild(menu, DEPTH_LAYER_CUBE, BTN_TAG::CAMERA);
	// 최초 비활성화
	mCameraItem->setEnabled(false);

	////////////////////////////////////////
	// 녹음 버튼 처리	
	Vec2 pos = Pos::getFirstRecorderBtnPt();
	Sprite* sp = Sprite::create(FILENAME_CUBE_BTN_ENABLE_RECORD_N);
	Size size = sp->getContentSize();	

	// 색칠하기 버튼이 없는 경우 가운데로 위치
	for (int i = 0; i < SIZE_BTN_RECORD; i++)
	{
		recordItems[i] = MenuItemImage::create(
			FILENAME_CUBE_BTN_ENABLE_RECORD_N, FILENAME_CUBE_BTN_ENABLE_RECORD_P, FILENAME_CUBE_BTN_DISABLE_RECORD_N,
			CC_CALLBACK_1(MainScene::menuRecordCallback, this, i + 1));
		recordItems[i]->setRotation(angleForCube[i]);
		
		if (mMBJson->mLockCubes[i + 1])
		{
			// 고정형 그림인 경우 녹음버튼을 가운데 위치
			pos.x += (Pos::getScreenSize().width * ((size.width / 2) / SCREEN_SIZE_WIDTH));//size.width / 2;			

			// 정보 저장
			MBJson::getInstance()->setCubeSuccessIndex(i, true);
		}

		recordItems[i]->setPosition(pos);
		
		switch (i)
		{
		case 0:			
			pos = Pos::getSecondRecorderBtnPt();
			break;
		case 1:			
			pos = Pos::getThirdRecorderBtnPt();
			break;
		case 2:			
			pos = Pos::getFouthRecorderBtnPt();
			break;
		case 3:			
			pos = Pos::getFifthRecorderBtnPt();
			break;
		case 4:			
			pos = Pos::getSixthRecorderBtnPt();
			break;
		} // end of switch

		
	} // end of for		

	// create menu, it's an autorelease object
	auto menuRecord = Menu::create(recordItems[0], recordItems[1], recordItems[2],
								   recordItems[3], recordItems[4], recordItems[5], nullptr);
	menuRecord->setPosition(Vec2::ZERO);
	this->addChild(menuRecord, DEPTH_LAYER_CUBE, BTN_TAG::RECORD);

	////////////////////////////////////////
	// 그리기 버튼 처리
	int cIdx = 0;
	Vec2 posd = Pos::getFirstDrawBtnPt();

	for (int i = 0; i < SIZE_BTN_DRAW; i++)
	{
		drawItems[i] = MenuItemImage::create(
			FILENAME_CUBE_BTN_ENABLE_DRAW_N, FILENAME_CUBE_BTN_ENABLE_DRAW_P, FILENAME_CUBE_BTN_DISABLE_DRAW_N,
			CC_CALLBACK_1(MainScene::menuDrawCallback, this, i + 1));		
		drawItems[i]->setRotation(angleForCube[i]);
		drawItems[i]->setPosition(posd);		

		switch (i)
		{
		case 0:
			posd = Pos::getSecondDrawBtnPt();
			break;
		case 1:
			posd = Pos::getThirdDrawBtnPt();
			break;
		case 2:
			posd = Pos::getFouthDrawBtnPt();
			break;
		case 3:
			posd = Pos::getFifthDrawBtnPt();
			break;
		case 4:
			posd = Pos::getSixthDrawBtnPt();
			break;
		} // end of switch

		////////////////////////////////////////
		// 그리기 버튼 비활성화 처리
		// 사용자 색칠하기 기능있는 인덱스 저장
		if (mMBJson->mLockCubes[i + 1] == false)
		{
			mIndex[cIdx] = i;			
			cIdx++;
		}
	} // end of for		
		
	// create menu, it's an autorelease object
	// 고정 그림은 색칠하기 버튼 제외
	auto menuDraw = Menu::create(drawItems[mIndex[0]],
		drawItems[mIndex[1]],
		drawItems[mIndex[2]],
		drawItems[mIndex[3]], nullptr);
	menuDraw->setPosition(Vec2::ZERO);
	this->addChild(menuDraw, DEPTH_LAYER_CUBE, BTN_TAG::DRAW);

	// 최초 모든 버튼 비활성화
	setResetAllCubeButton();	
	// 버튼 활성화 정보 저장
	//setBtnInfo();

	//// 녹음 버튼 비활성화
	//for (int i = 0; i < SIZE_CUBE; i++)
	//{
	//	setEnableRecordButtonWithIndex(false, i);
	//}
	
	////////////////////////////////////////
	// 배열하기 완료후 재생하기/재배열하기 버튼
	// 재생하기 버튼
	auto playItem = MenuItemImage::create(
		FILENAME_BTN_PLAY_N, FILENAME_BTN_PLAY_P, CC_CALLBACK_1(MainScene::menuPlayCallback, this));
	playItem->setPosition(Pos::getBtnPlayPt());
	// 재배열하기
	auto reorderItem = MenuItemImage::create(
		FILENAME_BTN_REORDER_N, FILENAME_BTN_REORDER_P, CC_CALLBACK_1(MainScene::menuReorderCallback, this));
	reorderItem->setPosition(Pos::getBtnReorderPt());

	// create menu, it's an autorelease object
	menuPlayAndOrder = Menu::create(playItem, reorderItem, NULL);
	//menuPlayAndOrder->retain();
	menuPlayAndOrder->setPosition(Vec2::ZERO);
	this->addChild(menuPlayAndOrder, DEPTH_LAYER_CUBE_PLAY_REORDER, BTN_TAG::PLAY_REORDER);
	// 최초 숨기기
	visibleBtnPlayAndReorder(false);
	////////////////////////////////////////
}


/**
* @brief  버튼 활성/비활성 - 최초 진입시 또는 팝업시 기존 버튼 비활성 처리
* @return void
*/
void MainScene::setEnableBtn(bool b)
{
	// 팝업시 버튼 활성 또는 비활성
	// 최초 진입시 버튼 비활성
	auto back = dynamic_cast<Menu*>(this->getChildByTag(BTN_TAG::BACK));	
	back->setTouchEnabled(b);

	auto help = dynamic_cast<Menu*>(this->getChildByTag(BTN_TAG::HELP));	
	help->setTouchEnabled(b);
}

/**
* @brief  사용자 그림 리셋
*
*/
void MainScene::resetUserCube()
{
	//CCLOG("RESET USER CUBE.......................................");
	// 기존 것 삭제
	for (int i = 0; i < SIZE_USER_CUBE; i++)
	{		
		CC_SAFE_RELEASE(mSpCubes[i]);
		this->removeChild(mSpCubes[i]);		
	} // end of for

	// 렌덤 넘버 생성
	GenRandIndex();

	////////////////////////////////////////
	// 배열하기(사용자) 	
	mSpCubes.clear();
	mSpCubesLock.clear();
		
	int posIdx = 0;	
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		// 고정용 이미지 체크
		if (mMBJson->mLockCubes[i + 1] == false)
		{
			//CCLOG("index: %d, RandNumber[%d]: %d, FILENAME: %s", i, i, mRandNumber[i], mUserCubeFileNames[mRandNumber[i]].c_str());
			createUserCube(i, posIdx);
			posIdx++;
		}
		else
		{
			// 고정용 이미지			
			//CCLOG("LOCK SPRITE.......: %d", i + 1);
		}
	} // end of for			

	// 터치 입력 허용
	mIsPlayCubeAni = false;
	// 호출한 레이어 기본 설정
	mLayerIndex = DEPTH_LAYER::CUBE;
}


/**
* @brief   배열하기용 이미지 생성
* @param  idx   상단 이미지 인덱스(0, 1, 2, 3, 4, 5)
* @param  bIdx  위치 계산용 인덱스(0, 1, 2, 3)
* @return  void
*/
void MainScene::createUserCube(int idx, int bIdx)
{	
	Sprite* sp  = Sprite::create(mUserCubeFileNames[mRandNumber[idx]].c_str());	
	sp->setPosition(Pos::getUserFirstCubeForAniPt());
	// 정답 위치 설정 (고정 이미지 위치 제외)
	// 0, 1, 2, 3, 4, 5
	//sp->setTag(mRandNumber[idx]);
	
	sp->setTag(mIndex[mRandNumber[idx]]);
	sp->retain();
	this->addChild(sp, DEPTH_LAYER_CUBE);

	// 벡터 설정
	mSpCubes.push_back(sp);	

	std::string strFileName = StringUtils::format("%s", mUserCubeFileNames[mRandNumber[idx]].c_str());
	int size = strFileName.size();
	std::string str = strFileName.substr(0, size - 12);
	std::string tempFileName = strFileName.substr(size - 9, size);
	strFileName = StringUtils::format("%s01_%s", str.c_str(), tempFileName.c_str());
	
	Sprite* spLock = Sprite::create(strFileName.c_str());
	spLock->setPosition(Pos::getUserFirstCubeForAniPt());
	spLock->setTag(mRandNumber[idx]);
	spLock->retain();
	spLock->setVisible(false);
	this->addChild(spLock, DEPTH_LAYER_CUBE);
	
	mSpCubesLock.push_back(spLock);

	// 애니 완료 위치 설정	
	Vec2 posu;
	// 위치값 계산
	switch (bIdx)
	{
	case 0:
		posu = Pos::getUserFirstCubePt();
		break;
	case 1:
		posu = Pos::getUserSecondCubePt();
		break;
	case 2:
		posu = Pos::getUserThirdCubePt();
		break;
	case 3:
		posu = Pos::getUserFouthCubePt();
		break;
	} // end of switch

	// 최초 위치 설정(애니 완료 위치 설정)
	mCubePos[bIdx] = posu;
	//CCLOG("POS : %d --> %s", bIdx, mUserCubeFileNames[mRandNumber[idx]].c_str());	
}


/**
* @brief   초기화 (레이아웃)
*          - 장난감 표시, 배열하기 그림 표시
* @return  void
*/
void MainScene::initLayout()
{
	//CCLOG("INIT LAYOUT............................");
	////////////////////////////////////////
	// 배열하기
	// 완료된 인덱스값 초기화
	resetCubeIndex();

	mSpCubeBgs = std::vector<Sprite*>(); // 배경 그림
	
	Vec2 posc = Pos::getFirstCubePt();
	float angle = angleForCube[0];//2.0f;
	int idx = 0;
	for (int i = 0; i < SIZE_BTN_DRAW; i++)
	{
		//CCLOG("[DRAW] [%d][%f, %f]", i, posc.x, posc.y);
		Sprite* sp = Sprite::create(FILENAME_CUBE_BG);			
		// 크기
		Size size = sp->getContentSize();
		posc.x += size.width / 2;
		posc.y -= size.height / 2;		
		
		// 배열 base 인덱스 값은 1부터
		if (mMBJson->mLockCubes[i + 1])
		{
			// 고정 사용자 그림 위치시킬 기준점 설정		
			Sprite* spCube = Sprite::create(mMBJson->mCubeFileNames[i + 1].c_str());
			spCube->getTexture()->setAntiAliasTexParameters();
			spCube->setPosition(posc);
			spCube->setRotation(angle);
			spCube->setTag(-1);
			spCube->setScale(SIZE_SCALE_CUBE);
			this->addChild(spCube, DEPTH_LAYER_CUBE);	

			// 테스트
			mSpCubeBgs.push_back(spCube);
		}
		else
		{
			// 사용자 그림 처리 하기 위한 배경 이미지 저장
			// 사용자 그림 위치시키기
			sp->getTexture()->setAntiAliasTexParameters();
			sp->setRotation(angle);
			sp->setPosition(posc);			
			sp->setTag(i);			
			this->addChild(sp, DEPTH_LAYER_CUBE);

			mUserCubeFileNames[idx] = mMBJson->mCubeFileNames[i + 1];
			idx++;
			mSpCubeBgs.push_back(sp);
		}

		// 위치값 계산
		angle = angleForCube[i + 1];
		switch (i)
		{
		case 0:
			//angle = angleForCube[i + 1]; // 1.0f;
			posc = Pos::getSecondCubePt();
			break;
		case 1:
			//angle = angleForCube[i + 1]; //0.0f;
			posc = Pos::getThirdCubePt();
			break;
		case 2:
			//angle = -0.75f;
			posc = Pos::getFouthCubePt();
			break;
		case 3:
			//angle = -1.8f;
			posc = Pos::getFifthCubePt();
			break;
		case 4:
			//angle = -2.0f;
			posc = Pos::getSixthCubePt();
			break;
		}
	} // end of for	
	////////////////////////////////////////

	// 사용자 그림 갯수 확인 (4개)
	CC_ASSERT(idx == SIZE_USER_CUBE);

	// 렌덤넘버 생성
	GenRandIndex();

	////////////////////////////////////////
	// 배열하기(사용자) 
	mSpCubes = std::vector<Sprite*>(); // 사용자 그림	
	mSpCubesLock = std::vector<Sprite*>(); // 고정용 그림
	mSpCubes.clear();
	mSpCubesLock.clear();

	int posIdx = 0;
	//CCLOG("============================================================================");
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		// 고정용 이미지 체크
		if (mMBJson->mLockCubes[i + 1] == false)
		{
			//CCLOG("index: %d, RandNumber[%d]: %d, FILENAME: %s", i, i, mRandNumber[i], mUserCubeFileNames[mRandNumber[i]].c_str());
			createUserCube(i, posIdx);
			posIdx++;
		}
		else
		{
			// 고정용 이미지			
			//CCLOG("LOCK SPRITE.......: %d", i + 1);
		}								
	} // end of for		
	//CCLOG("============================================================================");

	// 터치 입력 허용
	mUseTouch = true;
	// 호출한 레이어 기본 설정
	mLayerIndex = DEPTH_LAYER::CUBE;

	// 장난감 표시
	Sprite* regoSp = Sprite::create(FILENAME_CUBE_REGO_BLOCK);
	Vec2 regoPos = Pos::getCenterPt();
	regoPos.y = Pos::getScreenSize().height * ((SCRREN_SIZE_HEIGHT - 1110.0f) / SCRREN_SIZE_HEIGHT);	
	regoSp->setPosition(regoPos);
	this->addChild(regoSp, DEPTH_LAYER_CUBE + 1);
}


/**
* @brief   json file 읽기
* @return  void
*/
void MainScene::readJsonFile()
{	
	// read json
	ssize_t filesize;
	unsigned char* streamMove = FileUtils::getInstance()->getFileData(FILENAME_JSON_CUBE, "rb", &filesize);
	std::string str((const char*)streamMove, filesize);
	
	mMBJson = MBJson::getInstance();
	CC_ASSERT(mMBJson != nullptr);	
}


/**
* @brief   초기화
* @return  void
*/
void MainScene::initCube()
{
	// random seed
	srand(time(NULL));
	// 렌덤 넘버 생성
	GenRandIndex();

	mMBJson->initCubeSuccessIndex();
}

//void MainScene::initGaf()
//{
//	auto asset = GAFAsset::create(FILENAME_GAF_TITLE);
//	auto mgafObj = asset->createObjectAndRun(true);		
//	mgafObj->setPosition(0, 1200);
//	mgafObj->setLooped(true);
//	//mgafObj->stop();
//	this->addChild(mgafObj, 100);
//}

/**
* @brief   카메라 초기화
* @return  void
*/
void MainScene::initCamera()
{
	// Camera
	if (mCameraLayer == nullptr)
	{
		mCameraLayer = Layer::create();
		mCameraLayer->retain();
		mCameraLayer->setAnchorPoint(Vec2::ZERO);
		mCameraLayer->setColor(Color3B::BLACK);
		this->addChild(mCameraLayer, 0);
	}	
}


//------------------------------------------------------------
// Touch Event
//------------------------------------------------------------
/**
* @brief  터치 초기화
* @return void
*/
void MainScene::initTouchEvent()
{
	// touch event
	mListener = EventListenerTouchAllAtOnce::create();
	mListener->onTouchesBegan = CC_CALLBACK_2(MainScene::onTouchesBegan, this);
	mListener->onTouchesMoved = CC_CALLBACK_2(MainScene::onTouchesMoved, this);
	mListener->onTouchesCancelled = CC_CALLBACK_2(MainScene::onTouchesCancelled, this);
	mListener->onTouchesEnded = CC_CALLBACK_2(MainScene::onTouchesEnded, this);

	Director::getInstance()->getEventDispatcher()->addEventListenerWithFixedPriority(mListener, 1);
}

void MainScene::onTouchesBegan(const std::vector<Touch*>& touches, Event* event)
{	
	//CCLOG("onTouchesBegan....");
	if (mIsPlayCubeAni)
	{
		//CCLOG("CUBE ANI........");
		return;
	}
	// 나레이션 완료후 true로 변경
	if (mUseTouch == false)
	{
		//CCLOG("CUBE ANI........");
		return;
	}

	if (mTouchFlag)
	{
		//CCLOG("mTouchFlag : %d", mTouchFlag);
		return;
	}
	//mTouchFlag = true;

	initDragCube(touches[0]);
}

void MainScene::onTouchesMoved(const std::vector<Touch*>& touches, Event* event)
{
	if (mSelectCubeFlag == false)
	{
		return;
	}
	// 그림 움직임 처리
	// 드래그
	dragCube(touches[0]);
}


void MainScene::onTouchesEnded(const std::vector<Touch*>& touches, Event* event)
{	
	//CCLOG("onTouchesEnded....%d", mSelectCubeFlag);
	if (mSelectCubeFlag == false)
	{
		return;
	}
	// 드래그 처리
	// 터치 완료후 위치가 맞을경우 그위치 그대로 처리
	// 위치가 다른경우 최초 위치로 이동
	dragCubeFinish(touches[0]);
}


void MainScene::onTouchesCancelled(const std::vector<Touch*>& touches, Event* event)
{	
	//CCLOG("onTouchesCancelled ....");	
	dragCubeFinish(touches[0]);
}


//------------------------------------------------------------
// 버튼 이벤트
//------------------------------------------------------------
void MainScene::exitPlay()
{
	//CCLOG("End.........");
	// 학습 종료
	MSLPManager::getInstance()->finishProgress(false);

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "exitGame", "()V")) {
		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	Director::getInstance()->end();
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
	exit(0);
#endif
}


/**
* @brief   닫기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuCloseCallback(Ref* pSender)
{	
	if (mUseTouch == false)
	{
		return;
	}

	//CCLOG("menuClose......");
	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);

	exitPlay();
	/*
	// Popup 
	auto popup = PopupLayer::getIns(this, MAIN_EXIT);
	popup->showHide(true);

	// 선택 플래그 초기화
	mSelectCubeFlag = false;
	// 사용자 그림 터치 막기
	mIsPlayCubeAni = true;

	////////////////////////////////////////	
	// 버튼 정보 저장
	setBtnInfo();
	// 그리기를 위한 배열하기 버튼 비활성화
	setEnableTouchBtn(false);
	setEnableBtn(false);
	*/
}


/**
* @brief   카메라 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuCameraCallback(Ref* pSender)
{	
	if (mUseTouch == false)
	{
		return;
	}
	//CCLOG("menuCameraCallback......");
	AudioEngine::stopAll();
	this->stopAllActions();

	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);
	// 모두 완료시만 데이터 유지
	// 그외 모두 완료하지 못했을 경우는 초기화
	if (!isAllFinish())
	{
		// 초기화
		// 그림 인덱스 초기화
		resetCubeIndex();
		// 버튼 비활성화
		setResetAllCubeButton();

		// 상단 고정용 이미지 초기화
		for (int i = 0; i < SIZE_USER_CUBE; i++)
		{
			mSpCubesLock[i]->setVisible(false);

			mSpCubes[i]->setVisible(true);
			mSpCubes[i]->setScale(1.0f);
			mSpCubes[i]->setPosition(mCubePos[i]);
			mSpCubes[i]->setLocalZOrder(ZORDER_CUBE_INIT);
			mSpCubes[i]->setRotation(0.0f);
		} // end of for

		mMBJson->initCubeSuccessIndex();
	}

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	CameraManager::shootCamera(mCameraLayer, this);
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
	//exit(0);
#endif
}


/**
* @brief   녹음하기버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuRecordCallback(Ref* pSender, int idx)
{
	if (mUseTouch == false)
	{
		return;
	}
	//CCLOG("menuRecordCallback..... : %d", idx);
	
	// 배경음 정지
	auto audio = SimpleAudioEngine::getInstance();
	if (audio->isBackgroundMusicPlaying())
	{
		// stop
		audio->stopBackgroundMusic();
	}
	AudioEngine::stopAll();
	this->stopAllActions();

	AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);

	// 호출된 레이어 설정
	mLayerIndex = DEPTH_LAYER::RECORD;

	mRecordLayer = RecordLayer::createLayer(this, mWeekData, idx, mMBJson->mDrawFileNames[idx].c_str());
	this->addChild(mRecordLayer, DEPTH_LAYER_RECORD);

	processMoveToLayer();
}


/**
* @brief   그리기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuDrawCallback(Ref* pSender, int idx)
{
	if (mUseTouch == false)
	{
		return;
	}
	//CCLOG("menuDrawCallback..... : %d, %s", idx, mMBJson->mDrawFileNames[idx].c_str());
	AudioEngine::stopAll();
	this->stopAllActions();

	AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);

	// 호출된 레이어 설정
	mLayerIndex = DEPTH_LAYER::DRAW;

	// 그리기 호출	
	mDrawLayer = DrawScene::createLayer(this, mWeekData, idx, mMBJson->mDrawFileNames[idx].c_str());
	this->addChild(mDrawLayer, DEPTH_LAYER_DRAW);

	processMoveToLayer();
}


/**
* @brief   재생하기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuPlayCallback(Ref* pSender)
{
	//CCLOG("menuPlayCallback..... : ");
	AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	
	// 버튼 숨기기
	visibleBtnPlayAndReorder(false);

	// 재생하기 호출	
	MBJson::getInstance()->currentIndex = 1;

	stopBGM();
	// 메인씬 저장
	MBJson::getInstance()->setMainScene(this);

	// 비활성화
	setEnableAllCubeButton(false);
	setEnableTouchBtn(false);

	mPlayLayer = PlayScene::createLayer(this, mWeekData);	
	this->addChild(mPlayLayer, DEPTH_LAYER_RECORD);	
	//CCLOG("menuPlayCallback..... : END....");
}


/**
* @brief   재배열하기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuReorderCallback(Ref* pSender)
{
	//CCLOG("menuReorderCallback..... ");
	AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	
	// 재배열
	reorderCube();
}


/**
* @brief   그리기 닫기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuDrawCloseCallback(Ref* pSender)
{
	//CCLOG("menuDrawCloseCallback..... ");
}


/**
* @brief   팝업창 닫기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuPopupCloseCallback(Ref* pSender)
{
	//CCLOG("menuPopupDrawCloseCallback..... ");

	//////////////////////////////////////////
	//// 그리기를 위한 배열하기 버튼 활성화
	//setEnableAllCubeButton(true);
	setEnableTouchBtn(true);
	//recoverEnableBtn();
}


// 팝업창 닫기
void MainScene::menuPopupClose()
{
	//CCLOG("menuPopupClose..... ");

	//////////////////////////////////////////
	// 그리기를 위한 배열하기 버튼 활성화
	setEnableTouchBtn(true);
	// 사용자 그림 터치 허용
	mIsPlayCubeAni = false;
	//recoverEnableBtn();
	//setEnableAllCubeButton(true);	
}

/**
* @brief   그리기 닫기 버튼 이벤트(팝업창 그리기 유지 버튼)
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuPopupResumeCallback(Ref* pSender)
{
	//CCLOG("menuPopupResumeCallback..... ");
	// 팝업창 숨기기
	//mPopupSp->setVisible(false);	
}


/**
* @brief   도움말 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
void MainScene::menuHelpCallback(Ref* pSender)
{
	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);

	//CCLOG("menuHelpCallback..... ");
	PopupLayer* popup = PopupLayer::getIns(this, HELP_MAIN);
	((PopupLayer*)popup)->showHide(true, false);

	mSelectCubeFlag = false;

	// 사용자 그림 터치 막기
	mIsPlayCubeAni = true;

	////////////////////////////////////////	
	// 버튼 정보 저장
	setBtnInfo();
	// 그리기를 위한 배열하기 버튼 비활성화
	setEnableTouchBtn(false);
	//setEnableAllCubeButton(false);

	setEnableBtn(false);
}

/**
* @brief   기능 이동
* @return  void
*/
void MainScene::processMoveToLayer()
{
	// 리스너 중지
	mListener->setEnabled(false);
	mSelectCubeFlag = false;

	// 버튼 정보 저장
	setBtnInfo();
	
	////////////////////////////////////////
	// 그리기를 위한 배열하기 버튼 비활성화
	setEnableTouchBtn(false);
	//setEnableAllCubeButton(false);
}

//------------------------------------------------------------
// 배열하기
//------------------------------------------------------------
/**
* @brief   재배열 모션
* @return  void
*/
void MainScene::motionReorderCube()
{
	//CCLOG("motionReorderCube............");
	mIsPlayCubeAni = true;
	mAniReorderCubeIdx = SIZE_USER_CUBE - 1;
	// 1. 깜빡임
	// 2. 사라짐
	// 3. 등장 모션
	this->runAction(Sequence::create(
		// 깜빡임
		
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::soundEffectReorder, this)),
		DelayTime::create(TIME_MOTION_CUBE_BLINK * 2),		
		
		// 사라짐
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionFadeOutCubeIndex, this, 0)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionFadeOutCubeIndex, this, 1)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionFadeOutCubeIndex, this, 2)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionFadeOutCubeIndex, this, 3)),
		DelayTime::create(TIME_MOTION_CUBE_FADE_OUT* 2),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionFadeOutCubeDone, this)),
		nullptr));
}


/**
* @brief    재배열시 사라질때 효과음
* @param   pSender  호출자
* @return  void
*/
void MainScene::soundEffectReorder(Node* pSender)
{
	AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_REORDER);
}


/**
* @brief   모션
* @return  void
*/
void MainScene::motionInitIntervalCube()
{
	// 효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_BLINK);

	this->runAction(Sequence::create(
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionInitCubeIndex, this)),
		DelayTime::create(0.25f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionInitCubeIndex, this)),
		DelayTime::create(0.25f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionInitCubeIndex, this)),
		DelayTime::create(0.25f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionInitCubeIndex, this)),
		DelayTime::create(TIME_MOTION_CUBE_INIT),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionInitCubeDone, this)),
		DelayTime::create(0.1f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeIndex, this)),
		DelayTime::create(TIME_MOTION_CUBE_BLINK),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionBlickInitCubeDone, this)),
		nullptr));

}

/**
* @brief   모션
* @param   pSender  호출자
* @return  void
*/
void MainScene::motionInitCubeIndex(Node* pSender)
{
	// 최초 위치 이동 애니메이션
	mSpCubes[mAniReorderCubeIdx]->runAction(Sequence::create(
		EaseSineOut::create(MoveTo::create(TIME_MOTION_CUBE_INIT, mCubePos[mAniReorderCubeIdx])),
		nullptr));

	mAniReorderCubeIdx--;
}

void MainScene::motionInitCubeDone(Node* pSender)
{	
	// 최초 이동 애니매이션 완료
	mIsPlayCubeAni = false;
	mAniReorderCubeIdx = SIZE_USER_CUBE - 1;
	//CCLOG("MOTION COMPLETE.......");
	// 효과음
	//AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_BLINK);
}

/**
* @brief   모션
* @param   pSender  호출자
* @return  void
*/
void MainScene::motionBlickInitCubeIndex(Node* pSender)
{
	//CCLOG("motionInitCube index : aniReorder : %d", mAniReorderCubeIdx);

	// 최초 위치 이동 애니메이션
	mSpCubes[mAniReorderCubeIdx]->runAction(Sequence::create(
		Blink::create(TIME_MOTION_CUBE_BLINK, 2),
		nullptr));

	mAniReorderCubeIdx--;
}


void MainScene::motionBlickInitCubeDone(Node* pSender)
{
	// 최초 이동 애니매이션 완료
	mIsPlayCubeAni = false;
	//CCLOG("MOTION BLINK COMPLETE.......");

	// 1초뒤 나레이션 
	this->runAction(Sequence::create(DelayTime::create(1.0f), 
		CallFuncN::create(CC_CALLBACK_1(MainScene::playNarStart, this, 1)),
		nullptr));
}

/**
* @brief   사라지는 모션
* @param   pSender  호출자
* @return  void
*/
void MainScene::motionFadeOutCubeIndex(Node* pSender, int idx)
{
	//CCLOG("motionFadeOutCubeIndex index : aniReorder : %d", idx);
	
	// 애니메이션
	mSpCubes[idx]->runAction(Sequence::create(FadeOut::create(TIME_MOTION_CUBE_FADE_OUT), nullptr));
}

/**
* @brief   사라지는 모션(완료)
* @param   pSender  호출자
* @return  void
*/
void MainScene::motionFadeOutCubeDone(Node* pSender)
{
	//CCLOG("motionFadeOutCubeDone..............");
	
	mAniReorderCubeIdx = SIZE_USER_CUBE - 1;

	// 그림 인덱스 초기화
	resetCubeIndex();
	// 사용자 그림 리셋(렌덤 적용)
	resetUserCube();
	// 등장 모션
	motionInitIntervalCube();
}

/**
* @brief  최초 진입 모션
*
*/
void MainScene::aniReorderCube()
{
	//CCLOG("ANI..... REORDER...");

	////////////////////////////////////////
	// 최초 진입 모션
	mAniReorderCubeIdx = SIZE_USER_CUBE - 1;		
	motionInitIntervalCube();
}


/**
* @brief 중복된 렌덤 값인지 확인하는 함수
* @param rand  렌덤 값
* @param max   최대 개수 값
* @return bool true : 중복 값 존재, false : 미 존재
*/
bool MainScene::checkRandNumber(int rand, int max)
{
	for (int i = 0; i < max; i++)
	{
		if (mRandNumber[i] == rand)
		{
			return true;
		}
	} // end of for

	return false;
}


/**
* @brief 배열하기 모든 버튼 비활성화 및 녹음화기 버튼 활성화
* @return  void
*/
void MainScene::setResetAllCubeButton()
{
	//CCLOG("setResetAllCubeButton.......");
	// 최초 모든 버튼 비활성화
	setEnableAllCubeButton(false);
	// 고정 그림일 경우 그리기 버튼은 제외, 녹음하기 버튼은 활성화
	// -> 모든 버튼 비활성화로 변경
	//for (int i = 0; i < SIZE_BTN_DRAW; i++)
	//{
	//	if (mMBJson->mLockCubes[i + 1])
	//	{
	//		setEnableRecordButtonWithIndex(true, i);
	//	}
	//} // end of for
	
}

/**
* @brief 버튼의 활성/비활성 정보 저장
* @return  void
*/
void MainScene::setBtnInfo()
{
	for (int i = 0; i < SIZE_BTN_RECORD; i++)
	{
		mBtnRecordInfo[i] = recordItems[i]->isEnabled();
		if (i < SIZE_USER_CUBE)
		{
			mBtnDrawInfo[mIndex[i]] = drawItems[mIndex[i]]->isEnabled();
		}		
	} // end of for
}

// 터치기능 활성/ 비활성
void MainScene::setEnableTouchBtn(bool b)
{
	auto dispatcher = Director::getInstance()->getEventDispatcher();
	for (int i = BTN_TAG::BACK; i <= BTN_TAG::DRAW; i++)
	{
		auto btn = this->getChildByTag(i);
		if (b)
		{
			dispatcher->resumeEventListenersForTarget(btn);
		}
		else
		{
			dispatcher->pauseEventListenersForTarget(btn);
		}		
	} // end of for	
}

/**
* @brief 저장된 버튼 활성/비활성 정보 복원(기능 삭제가능)
* @return  void
*/
void MainScene::recoverEnableBtn()
{
	for (int i = 0; i < SIZE_BTN_RECORD; i++)
	{		
		// 녹음하기 버튼
		setEnableRecordButtonWithIndex(mBtnRecordInfo[i], i);
		// 그리기 버튼
		setEnableDrawButtonWithIndex(mBtnDrawInfo[mIndex[i]], i);				
	} // end of for
}

// 활성/ 비활성
void MainScene::setEnableAllCubeButton(bool b)
{
	for (int i = 0; i < SIZE_BTN_RECORD; i++)
	{
		recordItems[i]->setEnabled(b);
		
		if (i < SIZE_USER_CUBE)
		{
			drawItems[mIndex[i]]->setEnabled(b);
		}		
	} // end of for
}

// 활성/ 비활성
void MainScene::setEnableRecordButtonWithIndex(bool b, int index)
{
	recordItems[index]->setEnabled(b);	
}

// 활성/ 비활성
void MainScene::setEnableDrawButtonWithIndex(bool b, int index)
{
	if (index < SIZE_USER_CUBE)
	{		
		drawItems[mIndex[index]]->setEnabled(b);
	}		
}


// 배열하기 인덱스 재설정
void MainScene::resetCubeIndex()
{	
	//CCLOG("resetCubeIndex");
	// 완료된 인덱스값 초기화
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		mCubeFinishIdxs[i] = INVALID_INDEX;	
		mCubeStats[i] = CUBE_STATE::NONE;
	} // end of for

	mCubeCompleteCount = 0;
}

/**
* @brief   초기화 (배열하기)
* @param   result
* @return  void
*/
void MainScene::initDragCube(Touch* touch)
{
	//CCLOG("initDragCube...");
	mSelectCubeFlag = false;
	mCubeSelectIdx = INVALID_INDEX;

	Rect rect;
	int zorder = 0;
	for (int i = 0; i < SIZE_USER_CUBE; i++)
	{
		if (mCubeStats[i] == CUBE_STATE::COMPLETE)
		{
			//CCLOG("SKIP...INDEX : %d", i);
			continue;
		}

		Sprite* sp = mSpCubes[i];
		rect = sp->getBoundingBox();
		if (rect.containsPoint(touch->getLocation()))
		{
			// 효과음 재생
			AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_TAP);
			// 선택된 그림 인덱스 저장
			mCubeSelectIdx = i;
			mSelectCubeFlag = true;
			mTouchFlag = true;

			zorder = mSpCubes[SIZE_USER_CUBE - 1]->getLocalZOrder();
			sp->setLocalZOrder(zorder + 1);
			
			// 선택된 그림 사이즈 저장
			mSelectCubeSize = rect.size;			
			break;
		}
	} // end of for
}

/**
* @brief   그림 이동 처리 
* @param   touch
* @return  void
*/
void MainScene::dragCube(Touch* touch)
{		
	Vec2 touchPos = touch->getLocation();
	// 짤리지 않도록 수정
	// x
	if (touchPos.x < mSelectCubeSize.width / 2)
	{
		touchPos.x = mSelectCubeSize.width / 2;
	}
	if (touchPos.x > mScreenSize.width - mSelectCubeSize.width / 2)
	{
		touchPos.x = mScreenSize.width - mSelectCubeSize.width / 2;
	}
	
	// y	
	if (touchPos.y < mSelectCubeSize.height / 2)
	{
		touchPos.y = mSelectCubeSize.height / 2;
	}
	if (touchPos.y > mScreenSize.height - mSelectCubeSize.height / 2)
	{
		touchPos.y = mScreenSize.height - mSelectCubeSize.height / 2;
	}

	mSpCubes[mCubeSelectIdx]->setPosition(touchPos);
	mIsCubeMove = true;
	//CCLOG("M[%f,%f]", touchPos.x, touchPos.y);
}


/**
* @brief   터치 완료 처리
* @param   touch
* @return  void
*/
void MainScene::dragCubeFinish(Touch* touch)
{	
	//CCLOG("dragCubeFinish......");
	CC_ASSERT(mCubeSelectIdx != INVALID_INDEX);
	ptStart = touch->getLocation();	

	int bIdx = 0;
	int lockCount = 0;
	//for (int i = 0; i < SIZE_USER_CUBE; i++)
	for (int i = 0; i < SIZE_CUBE; i++)
	{
		// 인덱스는 1부터
		if (MBJson::getInstance()->mLockCubes[i + 1])
		{
			// 고정 이미지 스킵
			//CCLOG("SKIP.......INDEX : %d", i);
			lockCount++;
			continue;
		}		

		if (mCubeFinishIdxs[i] == VALID_INDEX_CUBE)
		{
			continue;
		}

		//CCLOG("index : %d.......check cube success", i);
		if (mSpCubeBgs[i]->getBoundingBox().containsPoint(ptStart))
		{
			// 정답인지 확인 필요
			if (isCubeSuccess(i))
			{
				// 효과음
				AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_SUCCESS);
				bIdx = i - lockCount;

				// 그림이 포함된 경우
				processCubeFinish(i, bIdx);
				return;
			}
			else
			{
				mIsInterSect = true;
				break;
			}			
		}
		else if (mSpCubeBgs[i]->getBoundingBox().intersectsRect(mSpCubes[mCubeSelectIdx]->getBoundingBox()))
		{
			mIsInterSect = true;
		}
		bIdx++;
	} // end of for

	// 터치 완료 처리(최초 위치로 이동)
	if (mIsCubeMove)
	{
		processCubeFinishFail();
	}	
}

/**
* @brief   터치 완료시 정답 확인
* @param   idx   for 루프의 파라메터, 상단 인덱스 (0, 1, 2, 3, 4, 5)
			     - mCubeIdx : 하단 배열할 그림 인덱스(0, 1, 2, 3)
* @return  void
*/
bool MainScene::isCubeSuccess(int idx)
{	
	//CCLOG("isCubeSuccess................");
	if (mMBJson->mLockCubes[idx + 1])
	{
		idx++;
	}

	if (mMBJson->mLockCubes[idx + 1])
	{
		idx++;
	}

	int sIdx = mSpCubes[mCubeSelectIdx]->getTag();

	// mCubeSelectIdx : 사용자 선택 인덱스(0, 1, 2, 3)	
	//CCLOG("isCubeSuccess...mCubeSelectIdx:%d, idx:%d, success Index(sIdx):%d", mCubeSelectIdx, idx, sIdx);
	if (sIdx == idx)
	{
		return true;
	}
	
	return false;
}

/**
* @brief   터치 완료 처리
* @param   idx   - for index, 상단 인덱스 (0, 1, 2, 3) //상단 인덱스 (0, 1, 2, 3, 4, 5)
                 - mCubeIdx : 하단 배열할 그림 인덱스(0, 1, 2, 3)
			btnIdx  - 그리기 버튼용 인덱스
* @return  void
*/
void MainScene::processCubeFinish(int idx, int btnIdx)
{	
	mSpCubes[mCubeSelectIdx]->setPosition(mSpCubeBgs[idx]->getPosition());
	mSpCubes[mCubeSelectIdx]->setScale(SIZE_SCALE_CUBE);
	// 로테이션
	mSpCubes[mCubeSelectIdx]->setRotation(angleForCube[mSpCubeBgs[idx]->getTag()]);
	mSpCubes[mCubeSelectIdx]->setLocalZOrder(ZORDER_CUBE_FINISH);
	mSpCubes[mCubeSelectIdx]->setVisible(false);
	//////////////////////////////
	// 상단 교정 이미지로 교체
	mSpCubesLock[mCubeSelectIdx]->setPosition(mSpCubeBgs[idx]->getPosition());
	mSpCubesLock[mCubeSelectIdx]->setScale(SIZE_SCALE_CUBE);
	// 로테이션
	mSpCubesLock[mCubeSelectIdx]->setRotation(angleForCube[mSpCubeBgs[idx]->getTag()]);
	mSpCubesLock[mCubeSelectIdx]->setLocalZOrder(ZORDER_CUBE_FINISH);
	mSpCubesLock[mCubeSelectIdx]->setVisible(true);
	
	// 상단 영역 배치 인덱스 : mSpCubeBgs[idx]->getTag()
	// 완료된 인덱스 저장 (사용자:하단 그림 나열 인덱스)
	mCubeFinishIdxs[idx] = VALID_INDEX_CUBE; 
	// 녹음하기에서 사용하기 위한 정보 저장	
	mMBJson->setCubeSuccessIndex(mSpCubeBgs[idx]->getTag(), true);	

	// 상태 저장(initDragCube)
	mCubeStats[mCubeSelectIdx] = CUBE_STATE::COMPLETE;

	/*CCLOG("FINISH idx: %d, Button Idex: %d, select index [%d], background ANGLE[TAG: %d]: %f", 
		idx, btnIdx, mCubeSelectIdx, mSpCubeBgs[idx]->getTag(), angleForCube[mSpCubeBgs[idx]->getTag()]);*/

	//  완료된 그림 버튼 활성화(삭제)
	/*
	setEnableDrawButtonWithIndex(true, btnIdx);
	setEnableRecordButtonWithIndex(true, mSpCubeBgs[idx]->getTag());
	*/

	mCubeCompleteCount++; 
	if (isAllFinish())
	{
		// 모두 성공한 경우
		allCubeFinish();
	}
	
	// 터치 허용
	mUseTouch       = true;
	mSelectCubeFlag = false;

	// 멀티 터치 막기 초기화
	mTouchFlag = false;
}

/**
* @brief   터치 완료 처리(최초 위치로 이동)
* @param   touch
* @return  void
*/
void MainScene::processCubeFinishFail()
{
	// 터치 막음
	mUseTouch = false;
	mSelectCubeFlag = false;
	//CCLOG("processCubeFinishFail...........");
	if (mIsInterSect)
	{
		// 효과음
		AudioEngine::play2d(FILENAME_SND_EFFECT_CUBE_FAIL);
		mIsInterSect = false;
	}

	// 최초 위치 이동 애니매이션
	mSpCubes[mCubeSelectIdx]->runAction(Sequence::create(
		MoveTo::create(TIME_MOTION_CUBE_POS, mCubePos[mCubeSelectIdx]),
		CallFuncN::create(CC_CALLBACK_1(MainScene::motionCubeFinishFailActionDone, this)),
		nullptr));	
}

/**
* @brief   터치 완료 처리(최초 위치로 이동 완료)
* @param   touch
* @return  void
*/
void MainScene::motionCubeFinishFailActionDone(Node* pSender)
{			
	//CCLOG("motionCubeFinishFailActionDone...........");
	mSpCubes[mCubeSelectIdx]->setLocalZOrder(ZORDER_CUBE_INIT);
	// 터치 허용
	mUseTouch = true;
	mSelectCubeFlag = false;
	mIsCubeMove     = false;

	// 멀티 터치 막기 초기화
	mTouchFlag = false;
}


/**
* @brief  배열하기 성공 유무
* @param  void
* @return bool  true: 모두 성공, false: 미 성공
*/
bool MainScene::isAllFinish()
{
	//CCLOG("isAllFinish");
	if (mCubeCompleteCount >= SIZE_USER_CUBE)
	{
		return true;
	}

	return false;
}


/**
* @brief  배열하기 성공 - 재배열, 재생하기 버튼 보이기
* @param  void
* @return void
*/
void MainScene::allCubeFinish()
{
	//CCLOG("allCubeFinish");
	// 녹음하기/ 색칠하기 모든 버튼 활성화
	setEnableAllCubeButton(true);
	// 버튼 보이기
	visibleBtnPlayAndReorder(true);
}


/**
* @brief  재배열하기 처리
* @param  void
* @return void
*/
void MainScene::reorderCube()
{
	//CCLOG("reorderCube");
	// 버튼 숨기기
	visibleBtnPlayAndReorder(false);
	// 버튼 비활성화
	setResetAllCubeButton();
	// 사라지는 모션
	motionReorderCube();

	// 상단 고정용 이미지 초기화
	for (int i = 0; i < SIZE_USER_CUBE; i++)
	{
		mSpCubesLock[i]->setVisible(false);
	}

	mMBJson->initCubeSuccessIndex();
	mTouchFlag = false;

	// 그림 인덱스 초기화
	//resetCubeIndex();
	// 사용자 그림 리셋(렌덤 적용)
	//resetUserCube();
	// 최초 진입 모션
	//aniReorderCube();
}


/**
* @brief   그리기 레이어 닫기
* @return  void
*/
void MainScene::closeDrawLayer()
{
	// 20160927
	//cocos2d::experimental::AudioEngine::stopAll();
	
	this->removeChild(mDrawLayer);

	// 리스너 활성화
	mListener->setEnabled(true);
	// 버튼 기능 활성화	
	setEnableTouchBtn(true);
	//recoverEnableBtn();
	//setEnableAllCubeButton(true);
	
	playBGM();
	mTouchFlag = false;
}


/**
* @brief   녹음하기 레이어 닫기
* @return  void
*/
void MainScene::closeRecordLayer()
{
	//CCLOG("closeRecordLayer");
	// 사용자 그림은 현재 위치에
	mSelectCubeFlag = false;

	this->removeChild(mRecordLayer);

	// 리스너 활성화
	mListener->setEnabled(true);	
	// 배열하기 버튼 활성화
	setEnableTouchBtn(true);

	playBGM();
	mTouchFlag = false;
}


/**
* @brief   재생하기 레이어 닫기(엔딩 페이지)
* @return  void
*/
void MainScene::resumePlayLayerEndingPage()
{
	//CCLOG("resumePlayLayerEndingPage................");
		
	//Director::getInstance()->popScene();
	if (mPlayLayer)
	{
		CCLOG("resumePlayLayerEndingPage.....stopAllActions");		
		((PlayScene*)mPlayLayer)->removeChild(((PlayScene*)mPlayLayer)->mLayer);
		
		//mPlayLayer->stopAllActions();
		/*((PlayScene*)mPlayLayer)->mPopup = PopupLayer::getIns(this, PLAY_ENDING);
		((PlayScene*)mPlayLayer)->mPopup->showHide(true);*/

		/*auto popup = PopupLayer::getIns(this, PLAY_ENDING);
		popup->showHide(true);*/
	}	
}


/**
* @brief   재생하기 레이어 닫기
* @return  void
*/
void MainScene::closePlayLayer()
{	
	//CCLOG("closePlayLayer................");
	visibleBtnPlayAndReorder(true);

	// 사용자 그림은 현재 위치에
	mSelectCubeFlag = false;
		
	this->removeChild(mPlayLayer);		
	Director::getInstance()->popToRootScene();
	
	// 리스너 활성화	
	//mListener->setEnabled(true);
	initTouchEvent();
	
	// 배열하기 버튼 활성화
	//setEnableTouchBtn(true);

	setEnableAllCubeButton(true);
	setEnableTouchBtn(true);
	
	playBGM();
	// 초기화
	MBJson::getInstance()->mCallingPageIndex = PLAYSCENE_CALLING::INIT;
	mTouchFlag = false;
}


/**
* @brief   재생하기 레이어 닫기
* @return  void
*/
void MainScene::closePlayLayerForEndingScene()
{
	//CCLOG("closePlayLayerForEndingScene................");
	visibleBtnPlayAndReorder(true);

	// 사용자 그림은 현재 위치에
	mSelectCubeFlag = false;

	this->removeChild(mPlayLayer);
	
	// 리스너 활성화	
	//mListener->setEnabled(true);
	initTouchEvent();

	// 배열하기 버튼 활성화
	setEnableAllCubeButton(true);
	setEnableTouchBtn(true);

	playBGM();
	// 초기화
	MBJson::getInstance()->mCallingPageIndex = PLAYSCENE_CALLING::INIT;
	mTouchFlag = false;
}


/**
* @brief  배열하기 완료후 플레이 및 재배열 버튼 보이기/숨기기
* @param  bool   true : 보이기, false: 숨기기
* @return void
*/
void MainScene::visibleBtnPlayAndReorder(bool b)
{
	//CCLOG("visibleBtnPlayAndReorder");
	if (menuPlayAndOrder != nullptr)
	{
		menuPlayAndOrder->setVisible(b);
	}	
}


/**
* @brief  배경음은 중지시키고 새로 배경음 재생
* @return void
*/
void MainScene::playBGM()
{
	// 플레이 중인 배경음 정지
	auto audio = SimpleAudioEngine::getInstance();
	if (audio->isBackgroundMusicPlaying())
	{
		// stop
		//audio->stopBackgroundMusic(true);
	}
	else
	{
		// play
		audio->setBackgroundMusicVolume(1.0f);
		audio->playBackgroundMusic(FILENAME_SND_BGM_DRAW, true);
	}	
}


/**
* @brief  플레이중인 배경음은 중지
* @return void
*/
void MainScene::stopBGM()
{
	// 플레이 중인 배경음 정지
	auto audio = SimpleAudioEngine::getInstance();
	if (audio->isBackgroundMusicPlaying())
	{
		// stop
		audio->stopBackgroundMusic(true);
	}
}


//------------------------------------------------------------
// 카메라
//------------------------------------------------------------
////////////////////////////////////////
// camera listener
/**
* @brief   카메라 완료(카메라 메니저)
* @param   result  
* @return  void
*/
void MainScene::onCameraFinished(bool result) 
{	
	//CCLOG("MGR::ready -> [MAINSCENE] onCameraFinished:: result: %d", result);
	if (result) 
	{
		/*if (word == Issue::Word::CAMERA) {
			bgLayer->removeChildByTag(CameraManager::IMAGE_TAG);
		}
		word = Issue::Word::CAMERA;
		menu->setQuickShow(false);
		MainScene::clear();
		MainScene::setContent();*/

		// 1. 레이어 초기화
		// 2. 메인씬 결과 처리		
	}
	else 
	{
		// 실패시
		//log("onCameraFinished false");
		//MainScene::setMode(MainMode::Mode::DRAW);
	}

	mTouchFlag = false;
}

/**
* @brief   카메라 완료(최종 처리)
* @return  void
*/
void MainScene::onCameraFinishedReal() 
{
	CCLOG("[MAINSCENE] onCameraFinishedReal....");
	// noting!
	//	MainScene::setContent();
	//InterfaceHelper::doStart();
	// 파일명 변경
	// 프로필 사진 변경		
	// 딜레이 action
	this->runAction(Sequence::create(DelayTime::create(0.2f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::renameProfileFileName, this)), 
		nullptr));	
}


void MainScene::renameProfileFileName(Node* pSender)
{
	CCLOG("Profile photo rename...........");
	//FileUtils *fileUtils = FileUtils::getInstance();	
	//fileUtils->renameFile(FILENAME_COMMON_CAPTURED_PROFILE_TEMP, FILENAME_COMMON_CAPTURED_PROFILE);
	
	/*this->runAction(Sequence::create(DelayTime::create(0.1f),
		CallFuncN::create(CC_CALLBACK_1(MainScene::delayMediaScanning, this)),
		nullptr));*/
}


void MainScene::delayMediaScanning(Node* pSender)
{	
	std::string path = StringUtils::format("%s", FILENAME_COMMON_CAPTURED_PROFILE);

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "galleryRefresh", "(Ljava/lang/String;)V")) {

		const char* strParam = path.c_str();
		jstring param1 = info.env->NewStringUTF(strParam);
		info.env->CallStaticVoidMethod(info.classID, info.methodID, param1);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}



/**
* @brief   종료
* @return  void
*/
void MainScene::onExit()
{
	//CCLOG("[MAINSCENE] onExit.....");
	Layer::onExit();

	//Director::getInstance()->end();	
	Director::getInstance()->getEventDispatcher()->removeEventListener(mListener);	
}


void MainScene::processUserTouch()
{
	// 스크린 메니져
	ScreenManager::getInstance()->screenOff(this);
}