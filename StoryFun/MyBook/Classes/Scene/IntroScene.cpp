#include "IntroScene.h"
#include "MainScene.h"
#include "Contents/MyBookSnd.h"

#include "mg_common/utils/DRMManager.h"
#include "mg_common/utils/MSLPManager.h"

// for AudioEngine
using namespace cocos2d::experimental;

/*
* @brief  씬생성
* @return  Scene 생성된 씬
*/
Scene* IntroScene::createScene(int weekData)
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = IntroScene::create();
	layer->mWeekData = weekData;
	layer->initData();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

// on "init" you need to initialize your instance
bool IntroScene::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}

	// 테스트
	//mWeekData = FILENAME_WEEK_DATA_TEST;
	//CCLOG("[INTRO] mWeekData : %d", mWeekData);
	//mSeqAction = nullptr;
	//// 터치 이벤트 초기화
	//initTouchEvent();

	//// 레이아웃 설정
	//initLayout();
	//// 로딩
	////preLoadingData();
	//preLoadingData();
	//preLoadingImage();
	//preLoadingSound();
	////setPlayTime();
	//// 학습 이력 확인
	//checkWeekData();

	return true;
}


void IntroScene::initData()
{
	Device::setKeepScreenOn(true);

	CCLOG("[INTRO] setKeepScreenOn : true");
	CCLOG("[INTRO] mWeekData : %d", mWeekData);

	std::string tempFolder = DRMManager::getInstance()->getTempDirPath();
	CCFileUtils *fileUtils = CCFileUtils::sharedFileUtils();
	//CCLOG("------------------ADD SEARCH PATH");
	fileUtils->addSearchPath(tempFolder);
	
	/*this->runAction(Sequence::create(DelayTime::create(5.0f),
		CallFuncN::create(CC_CALLBACK_1(IntroScene::delayScene, this)), nullptr));*/
	preLoadingData();
	preLoadingImage();
	preLoadingSound();
}

void IntroScene::delayScene(Node* pSender)
{	
	// 로딩
	//preLoadingData();
	preLoadingData();
	preLoadingImage();
	preLoadingSound();	
}


void IntroScene::displayScene()
{
	mSeqAction = nullptr;
	// 학습 이력 확인
	checkWeekData();

	// 터치 이벤트 초기화
	initTouchEvent();

	// 레이아웃 설정
	initLayout();
	//setPlayTime();
}


/*
* 1. 최초 학습시
*    - 나레이션 끝나면 활동으로 자동 이동
* 2. 학습 이력 존재시
*    - 터치시 활동으로 이동
*/

/**
* @brief 학습 이력 확인 및 포폴 네임 저장
* @return void
*/
void IntroScene::checkWeekData()
{
	// weekdata
	MBJson* json = MBJson::getInstance();
	json->setWeekData(mWeekData);

	// 포트폴리오 패스 저장 --> fullpath + 파일명 저장으로 변경
	//json->setPorfolioPath(MSLPManager::getInstance()->getPortfolioPath());
	std::string strFileName      = StringUtils::format(PLAYSCENE_PORTFOLIO_FILENAME, mWeekData);
	std::string fullPathFileName = MSLPManager::getInstance()->getPortfolioPath() + strFileName;
	json->setPorfolioPath(fullPathFileName);

	std::string key = StringUtils::format(USERDEFAULT_KEY_PLAY_WEEKDATA_X, mWeekData);
	if (UserDefault::getInstance()->getIntegerForKey(key.c_str()) == 0)
	{
		// 최초 학습시 나레이션 종료후 이동
		mUsedWeekData = false;
		// 학습이력 저장
		setPlayTime();
	}
	else
	{
		// 학습 이력 존재시 터치로 이동
		mUsedWeekData = true;
		mEnableTouch  = true;
	}	

	// 나레이션 재생	
	this->runAction(Sequence::create(DelayTime::create(2.f),
		CallFuncN::create(CC_CALLBACK_1(IntroScene::playNarTitle, this)),
		nullptr));
}


/**
* @brief  나레이션 플레이
* @return void
*/
void IntroScene::playNarTitle(Node* pSender)
{
	auto id = AudioEngine::play2d(FILENAME_SND_TITLE_NAR);
	AudioEngine::setFinishCallback(id, CC_CALLBACK_0(IntroScene::finishNarTitleCallback, this));
}


/**
* @brief 콜백 함수
*/
void IntroScene::finishNarTitleCallback()//int id, const std::string &file)
{
	//CCLOG("finishNarTitleCallback........: %d", MBJson::getInstance()->mTouch);
	/*if (mUsedWeekData)
	{
	return;
	}*/
	
	if (MBJson::getInstance()->mTouch)
	{
		return;
	}
	mEnableTouch = true;
	this->runAction(Sequence::create(DelayTime::create(1.0f),
		CallFuncN::create(CC_CALLBACK_1(IntroScene::toScene, this)), nullptr));
}


/**
* @brief  메인씬으로 이동
*/
void IntroScene::toScene(Node* pSender)
{
	//CCLOG("AUTO MODE...TO SCENE:;:....");
	
	if (mEnableTouch)
	{
		//CCLOG("AUTO MODE...TO SCENE:;:....%d", MBJson::getInstance()->mTouch);
		if (MBJson::getInstance()->mTouch)
		{		
			MBJson::getInstance()->mTouch = false;
			return;
		}
		auto scene = MainScene::createScene();
		// run
		Director::getInstance()->replaceScene(scene);
	}
}




/**
* @brief 미리 로딩
*
*/
void IntroScene::preLoadingData()
{
	////////////////////////////////////////
	// Json으로 부터 파일 로딩
	ssize_t filesize;
	unsigned char* streamMove = FileUtils::getInstance()->getFileData(FILENAME_JSON_CUBE, "rb", &filesize);
	std::string str((const char*)streamMove, filesize);
	auto mbJson = MBJson::getInstance();
	CC_ASSERT(mbJson != nullptr);
	mbJson->readJsonFileForMain(str.c_str(), mWeekData);

	// 프리로드 리소스 갯수
	mResCount = 21;
}


/**
* @brief 데이터 로딩
*
*/
void IntroScene::preLoadingImage()
{
	////////////////////////////////////////
	// BGM 로딩
	auto audio = SimpleAudioEngine::getInstance();
	//audio->preloadBackgroundMusic(FILENAME_SND_BGM_CUBE);
	audio->preloadBackgroundMusic(FILENAME_SND_BGM_DRAW);
	//set volume
	audio->setEffectsVolume(1.0f);

	////////////////////////////////////////
	// 이미지 로딩
	auto texCache = Director::getInstance()->getTextureCache();	
	texCache->addImageAsync(FILENAME_MAIN_TITLE, CC_CALLBACK_1(IntroScene::loadingCallBack, this));
	texCache->addImageAsync(FILENAME_BG_MAINSCENE, CC_CALLBACK_1(IntroScene::loadingCallBack, this));	
	texCache->addImageAsync(FILENAME_BG_MAINSCENE_TONGS, CC_CALLBACK_1(IntroScene::loadingCallBack, this));	
	texCache->addImageAsync(FILENAME_CUBE_BG, CC_CALLBACK_1(IntroScene::loadingCallBack, this));	
	texCache->addImageAsync(FILENAME_RECORD_BG, CC_CALLBACK_1(IntroScene::loadingCallBack, this));	
	texCache->addImageAsync(FILENAME_DRAW_CUBE_BG, CC_CALLBACK_1(IntroScene::loadingCallBack, this));
	//for (int i = 1; i <= ANI_PRELOAD_MAX_INDEX; i++)
	//{
	//	texCache->addImageAsync(StringUtils::format(FILENAME_ANI_PRELOAD_X, i).c_str(), CC_CALLBACK_1(IntroScene::loadingCallBack, this));		
	//} // end of for
	//FILENAME_RECORD_BG
	// 책커버 로딩
	auto mbJson = MBJson::getInstance();
	for (int i = 1; i <= SIZE_CUBE; i++)
	{
		texCache->addImageAsync(mbJson->mCubeFileNames[i], CC_CALLBACK_1(IntroScene::loadingCallBack, this));
		texCache->addImageAsync(mbJson->mDrawFileNames[i], CC_CALLBACK_1(IntroScene::loadingCallBack, this));
		if (mbJson->mLockCubes[i] == false)
		{
			std::string strTemp = mbJson->mDrawFileNames[i].c_str();
			int size = strTemp.size();
			std::string strFilename = strTemp.substr(0, size - 4);
			//texCache->addImageAsync(StringUtils::format(FILENAME_DRAW_COLOR, strFilename.c_str()), CC_CALLBACK_1(IntroScene::loadingCallBack, this));
			texCache->addImageAsync(StringUtils::format(FILENAME_DRAW_LINE, strFilename.c_str()), CC_CALLBACK_1(IntroScene::loadingCallBack, this));
		}		
	} // end of for		
	
}


void IntroScene::loadingCallBack(cocos2d::Texture2D *texture)
{
	//CCLOG("loadingCallBack..%d", mResCount);
	mResCount--;
	if (mResCount == 0)
	{		
		delayTitle();
	}
}


void IntroScene::delayTitle()
{
	DRMManager::getInstance()->hideProgress();
	displayScene();
}


/**
* @brief 미리 로딩
*
*/
void IntroScene::preLoadingSound()
{	
}


/**
* @brief 터치 이벤트 초기화
*
*/
void IntroScene::initTouchEvent()
{
	//mMultiTouch = false;
	MBJson::getInstance()->mTouch = false;
	// touch event
	mListener = EventListenerTouchAllAtOnce::create();
	mListener->onTouchesBegan = CC_CALLBACK_2(IntroScene::onTouchesBegan, this);
	mListener->onTouchesMoved = CC_CALLBACK_2(IntroScene::onTouchesMoved, this);
	mListener->onTouchesEnded = CC_CALLBACK_2(IntroScene::onTouchesEnded, this);

	Director::getInstance()->getEventDispatcher()->addEventListenerWithFixedPriority(mListener, 1);
}


/**
* @brief 터치 이벤트 
*
*/
void IntroScene::onTouchesBegan(const std::vector<Touch*>& touches, Event* event)
{
	if (mEnableTouch && mUsedWeekData)
	{
		//CCLOG("onTouchesBegan:;:....");
		//this->unscheduleAllSelectors();
		this->unscheduleAllCallbacks();
		
		//if (mMultiTouch == false)
		if (MBJson::getInstance()->mTouch == false)
		{
			//mMultiTouch = true;
			MBJson::getInstance()->mTouch = true;
			//CCLOG("onTouchesBegan:;:....MBJson::getInstance()->mTouch : %d ", MBJson::getInstance()->mTouch);
			this->stopAllActions();
			this->unscheduleUpdate();

			auto scene = MainScene::createScene();
			// run
			Director::getInstance()->replaceScene(scene);			
		}		
	}
}


/**
* @brief 터치 이벤트 
*
*/
void IntroScene::onTouchesMoved(const std::vector<Touch*>& touches, Event* event)
{
	
}


/**
* @brief 터치 이벤트 
*
*/
void IntroScene::onTouchesEnded(const std::vector<Touch*>& touches, Event* event)
{
	
}


/**
* @brief  플레이 횟수 체크
*
*/
bool IntroScene::setPlayTime()
{
	// 각 권에 맞는 플레이한 이력이 있는지 검사
	std::string strWeekData = StringUtils::format(USERDEFAULT_KEY_PLAY_WEEKDATA_X, mWeekData);
	auto userDefault = UserDefault::getInstance();
	int weekDataCount = userDefault->getIntegerForKey(strWeekData.c_str(), 0);
	if (weekDataCount == 0)
	{
		// 최초 실행
		weekDataCount++;
		userDefault->setIntegerForKey(strWeekData.c_str(), weekDataCount);
	}	

	return true;
}


/**
* @brief 레이아웃 초기화
*
*/
void IntroScene::initLayout()
{
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	/////////////////////////////
	// 2. add a menu item with "X" image, which is clicked to quit the program
	//    you may modify it.
	auto sp = Sprite::create(FILENAME_MAIN_TITLE);
	sp->setPosition(Pos::getCenterPt());
	this->addChild(sp);

	////////////////////////////////////////
	// 닫기 버튼
	auto quitItem = MenuItemImage::create(
		FILENAME_BTN_QUIT_N, FILENAME_BTN_QUIT_P,
		CC_CALLBACK_1(IntroScene::menuCloseCallback, this));
	quitItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	quitItem->setPosition(Pos::getBackBtnPt());

	auto menuQuit = Menu::create(quitItem, NULL);
	menuQuit->setPosition(Vec2::ZERO);
	this->addChild(menuQuit);
	// 터치 허용
	mUseTouch = true;
	//Size size = sp->getContentSize();
}

void IntroScene::menuCloseCallback(Ref* pSender)
{
	if (mUseTouch == false)
	{
		return;
	}
	mUseTouch = false;
	//CCLOG("menuClose......");
	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);

	exitPlay();

}

void IntroScene::exitPlay()
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


void IntroScene::menuNextSceneCallback(Ref* pSender)
{	
	////auto scene = MainScene::createScene();
	//////auto scene = IntroScene::createScene();
	//////auto pscene = TransitionPageTurn::create(2.5f, scene, true);
	////// run
	////Director::getInstance()->replaceScene(scene);
}


void IntroScene::onExit()
{
	//CCLOG("onExit....");
	// 상위 클래스 호출
	Layer::onExit();
	//this->stopAllActions();
	//this->unscheduleUpdate();
	Director::getInstance()->getEventDispatcher()->removeEventListener(mListener);
}
