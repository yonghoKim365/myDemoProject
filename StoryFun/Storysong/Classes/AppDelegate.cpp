#include "AppDelegate.h"
#include "HelloWorldScene.h"
#include "scene/SongIntro.h"
#include "scene/SongPlayScene.h"
#include "common/Utils.h"
#include "audio/include/AudioEngine.h"
#include "data/PlayInfo.h"
#include "data/JsonInfo.h"
#include "common/AndroidHelper.h"
#include "mg_common/utils/DRMManager.h"
#include "mg_common/utils/MSLPManager.h"

using namespace experimental;

USING_NS_CC;

static cocos2d::Size designResolutionSize = cocos2d::Size(1920, 1200);
static cocos2d::Size smallResolutionSize = cocos2d::Size(480, 320);
static cocos2d::Size mediumResolutionSize = cocos2d::Size(960, 600);
static cocos2d::Size largeResolutionSize = cocos2d::Size(1920, 1200);

AppDelegate::AppDelegate() {

}

AppDelegate::~AppDelegate()
{
	PlayInfo::releaseInstance();
	JsonInfo::releaseInstance();
}

//if you want a different context,just modify the value of glContextAttrs
//it will takes effect on all platforms
void AppDelegate::initGLContextAttrs()
{
	//set OpenGL context attributions,now can only set six attributions:
	//red,green,blue,alpha,depth,stencil
	GLContextAttrs glContextAttrs = { 8, 8, 8, 8, 24, 8 };

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
	
	
	auto director = Director::getInstance();
	auto glview = director->getOpenGLView();
	if (!glview) {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)
		glview = GLViewImpl::createWithRect("storyfun", Rect(0, 0, designResolutionSize.width, designResolutionSize.height));
#else
		glview = GLViewImpl::create("storyfun");
#endif
		director->setOpenGLView(glview);
	}

	// turn on display FPS
	director->setDisplayStats(false);

	// set FPS. the default value is 1.0/60 if you don't call this
	director->setAnimationInterval(1.0 / 60);

	// Set the design resolution
	glview->setDesignResolutionSize(designResolutionSize.width, designResolutionSize.height, ResolutionPolicy::NO_BORDER);
	Size frameSize = glview->getFrameSize();
	register_all_packages();


	//MSLPManager::getInstance()->beginProgress(0);
	
	CCLOG("=======================================================");
	CCLOG("CURRENT WEEK : %d", MSLPManager::getInstance()->getBookNum());
	CCLOG("=======================================================");

	
	std::string tempFolder = DRMManager::getInstance()->getTempDirPath();
    std::string resFolder = DRMManager::getInstance()->getResourcePath();
	
	/*std::string rootPath;
	if (MSLPManager::getInstance()->getBookNum() < 10)
		rootPath = StringUtils::format("B0%d_SNG/", MSLPManager::getInstance()->getBookNum());
	else
		rootPath = StringUtils::format("B%d_SNG/", MSLPManager::getInstance()->getBookNum());

	tempFolder.append(rootPath);
	*/
	CCFileUtils *fileUtils = CCFileUtils::sharedFileUtils();
	CCLOG("------------------ADD SEARCH PATH %s", tempFolder.c_str());
	fileUtils->addSearchPath(tempFolder);
    fileUtils->addSearchPath(resFolder);

	JsonInfo * jsonInfo = JsonInfo::create();
	
	if (!MSLPManager::getInstance()->isFinished())
		jsonInfo->isFirstPlay = true;
	else
		jsonInfo->isFirstPlay = false;
	CCLOG("First Play %d", jsonInfo->isFirstPlay);
	
	jsonInfo->setCurrentWeek(MSLPManager::getInstance()->getBookNum());
	CCLOG("WEEK :: %d", MSLPManager::getInstance()->getBookNum());
	auto scene = SongIntro::createScene();
	Utils::moveToScene(scene);

	
	
	//DRMManager::getInstance()->hideProgress();

	return true;
}

// This function will be called when the app is inactive. When comes a phone call,it's be invoked too
void AppDelegate::applicationDidEnterBackground() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	Director::getInstance()->stopAnimation();
	Director::getInstance()->getEventDispatcher()->dispatchCustomEvent("ON_PAUSE");
#endif
	/*Director::getInstance()->pause();
	AudioEngine::pause(PlayInfo::create()->getBGMID());*/

}

// this function will be called when the app is active again
void AppDelegate::applicationWillEnterForeground() {
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	Director::getInstance()->getEventDispatcher()->dispatchCustomEvent("ON_RESUME");
	Director::getInstance()->startAnimation();
#endif
	/*Director::getInstance()->resume();
	AudioEngine::resume(PlayInfo::create()->getBGMID());*/

	
}
