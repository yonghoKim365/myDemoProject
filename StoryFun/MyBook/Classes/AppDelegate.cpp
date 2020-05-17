#include "AppDelegate.h"
//#include "Scene/MainScene.h"
#include  "mg_common/utils/DRMManager.h"
#include  "mg_common/utils/MSLPManager.h"
#include  "mg_common/utils/GAFManager.h"

#include "Util/Pos.h"
#include "Scene/IntroScene.h"
// MBJ	
#include "Util/MBJson.h"


USING_NS_CC;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)
// 윈도우 테스트용
//static cocos2d::Size designResolutionSize = cocos2d::Size(1280, 800); // 단말기 해상도 기준
static cocos2d::Size designResolutionSize = cocos2d::Size(1920, 1200); // 단말기 해상도 기준
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
// 단말 테스트용(이것임)
static cocos2d::Size designResolutionSize = cocos2d::Size(1920, 1200); // 단말기 해상도 기준
#endif

static cocos2d::Size smallResolutionSize = cocos2d::Size(480, 320);
static cocos2d::Size mediumResolutionSize = cocos2d::Size(1024, 768);
//static cocos2d::Size mediumResolutionSize = cocos2d::Size(1024, 720);
static cocos2d::Size largeResolutionSize = cocos2d::Size(2048, 1536);
//static cocos2d::Size largeResolutionSize = cocos2d::Size(1920, 1200);


AppDelegate::AppDelegate() 
{
}

AppDelegate::~AppDelegate() 
{
}

//if you want a different context,just modify the value of glContextAttrs
//it will takes effect on all platforms
void AppDelegate::initGLContextAttrs()
{
    //set OpenGL context attributions,now can only set six attributions:
    //red,green,blue,alpha,depth,stencil
    GLContextAttrs glContextAttrs = {8, 8, 8, 8, 24, 8};

    GLView::setGLContextAttrs(glContextAttrs);
}

// If you want to use packages manager to install more packages, 
// don't modify or remove this function
static int register_all_packages()
{
    return 0; //flag for packages manager
}

bool AppDelegate::applicationDidFinishLaunching() 
{
    // initialize director
    auto director = Director::getInstance();
    auto glview = director->getOpenGLView();
    if(!glview) 
	{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)
        glview = GLViewImpl::createWithRect("mybook", Rect(0, 0, designResolutionSize.width, designResolutionSize.height));
		//glview = GLViewImpl::createWithRect("mybook", Rect(0, 0, scaleResolutionSize.width, scaleResolutionSize.height));
#else
        glview = GLViewImpl::create("mybook");
#endif
        director->setOpenGLView(glview);
    }

    // turn on display FPS
    //director->setDisplayStats(true);

    // set FPS. the default value is 1.0/60 if you don't call this
    director->setAnimationInterval(1.0 / 60);

	// 2d setting
	//director->setProjection(cocos2d::Director::Projection::_2D);

    // Set the design resolution
	// original size 1920, 1200
    glview->setDesignResolutionSize(designResolutionSize.width, designResolutionSize.height, ResolutionPolicy::EXACT_FIT);
	
	Size frameSize = glview->getFrameSize();

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)
	// 단말으로 사용시 커멘트 처리 필요
    // if the frame's height is larger than the height of medium size.
    if (frameSize.height > mediumResolutionSize.height)
    {    
		CCLOG("scale 1: %f", MIN(largeResolutionSize.height / designResolutionSize.height, largeResolutionSize.width / designResolutionSize.width));
        director->setContentScaleFactor(MIN(largeResolutionSize.height/designResolutionSize.height, largeResolutionSize.width/designResolutionSize.width));
    }
    // if the frame's height is larger than the height of small size.
    else if (frameSize.height > smallResolutionSize.height)
    {     
		CCLOG("scale 2: %f", MIN(mediumResolutionSize.height / designResolutionSize.height, mediumResolutionSize.width / designResolutionSize.width));
        director->setContentScaleFactor(MIN(mediumResolutionSize.height/designResolutionSize.height, mediumResolutionSize.width/designResolutionSize.width));
    }
    // if the frame's height is smaller than the height of medium size.
    else
    {        
		CCLOG("scale 3: %f", MIN(smallResolutionSize.height / designResolutionSize.height, smallResolutionSize.width / designResolutionSize.width));
        director->setContentScaleFactor(MIN(smallResolutionSize.height/designResolutionSize.height, smallResolutionSize.width/designResolutionSize.width));
    }
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)	
	//CCLOG("scale : %f, %f", scaleResolutionSize.height / designResolutionSize.height, scaleResolutionSize.width / designResolutionSize.width);
	//	
	director->setContentScaleFactor(1.0f);	
#endif
	
    register_all_packages();

	// 권별 정보 가져오기
#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;

	if (JniHelper::getStaticMethodInfo(t
		, "org/cocos2dx/cpp/AppActivity"
		, "getStudyWeek"
		, "()I"));
	{
		jint week = t.env->CallStaticIntMethod(t.classID, t.methodID);
		mCurrentWeek = week;
		t.env->DeleteLocalRef(t.classID);
	}
#else
	mCurrentWeek = 2;
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	/* 학습시작 호출   */
	//MSLPManager::getInstance()->beginProgress(stepnum);
	int mCurrentWeek = MSLPManager::getInstance()->getBookNum();
#endif
	/*
	CCLOG("=======================================================");
	CCLOG("CURRENT WEEK : %d", mCurrentWeek);
	CCLOG("SCREEN SIZE : [%f, %f]", Pos::getScreenSize().width, Pos::getScreenSize().height);
	CCLOG("FRAME SIZE  : [%f, %f]", frameSize.width, frameSize.height);
	CCLOG("=======================================================");
	*/
	auto scene = IntroScene::createScene(mCurrentWeek);
	
    // run
	director->runWithScene(scene);

	/* PROGRESS 화면 종료   */
	//DRMManager::getInstance()->hideProgress();

    return true;
}

// This function will be called when the app is inactive. When comes a phone call,it's be invoked too
void AppDelegate::applicationDidEnterBackground() 
{
    Director::getInstance()->stopAnimation();

    // if you use SimpleAudioEngine, it must be pause
    // SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
}

// this function will be called when the app is active again
void AppDelegate::applicationWillEnterForeground() {
    Director::getInstance()->startAnimation();

    // if you use SimpleAudioEngine, it must resume here
    // SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
}
