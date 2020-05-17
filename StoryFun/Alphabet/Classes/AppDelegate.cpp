#include "AppDelegate.h"
#include "Preload.h"

#include "ProductManager.h"
#include "DRMManager.h"
#include "MSLPManager.h"
#include "GAFManager.h"

#include "Debug_Index.h"
#include "Step3.h"
#include "ProductIndex.h"

USING_NS_CC;

static cocos2d::Size designResolutionSize = cocos2d::Size(480, 320);
static cocos2d::Size smallResolutionSize = cocos2d::Size(480, 320);
static cocos2d::Size mediumResolutionSize = cocos2d::Size(1024, 768);
static cocos2d::Size largeResolutionSize = cocos2d::Size(2048, 1536);

AppDelegate::AppDelegate() {

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

bool AppDelegate::applicationDidFinishLaunching() {
    // initialize director
    auto director = Director::getInstance();
    auto glview = director->getOpenGLView();
    if(!glview) {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) || (CC_TARGET_PLATFORM == CC_PLATFORM_MAC) || (CC_TARGET_PLATFORM == CC_PLATFORM_LINUX)
        glview = GLViewImpl::createWithRect("Alphabet", Rect(0, 0, designResolutionSize.width, designResolutionSize.height));
#else
        glview = GLViewImpl::create("Alphabet");
#endif
        director->setOpenGLView(glview);
    }

    // turn on display FPS
    director->setDisplayStats(false);

    // set FPS. the default value is 1.0/60 if you don't call this
    director->setAnimationInterval(1.0 / 60);

    
    glview->setDesignResolutionSize(1920, 1200, ResolutionPolicy::SHOW_ALL);
    
//    // Set the design resolution
//    glview->setDesignResolutionSize(designResolutionSize.width, designResolutionSize.height, ResolutionPolicy::NO_BORDER);
//    Size frameSize = glview->getFrameSize();
//    // if the frame's height is larger than the height of medium size.
//    if (frameSize.height > mediumResolutionSize.height)
//    {        
//        director->setContentScaleFactor(MIN(largeResolutionSize.height/designResolutionSize.height, largeResolutionSize.width/designResolutionSize.width));
//    }
//    // if the frame's height is larger than the height of small size.
//    else if (frameSize.height > smallResolutionSize.height)
//    {        
//        director->setContentScaleFactor(MIN(mediumResolutionSize.height/designResolutionSize.height, mediumResolutionSize.width/designResolutionSize.width));
//    }
//    // if the frame's height is smaller than the height of medium size.
//    else
//    {        
//        director->setContentScaleFactor(MIN(smallResolutionSize.height/designResolutionSize.height, smallResolutionSize.width/designResolutionSize.width));
//    }

    register_all_packages();
    
    bool isResEmbeded = false;
    
    GAFManager::getInstance()->setIsResEmbeded(isResEmbeded);
    MGTResourceUtils::getInstance()->setIsResEmbeded(isResEmbeded);
    ProductManager::getInstance()->setIsResEmbeded(isResEmbeded);
    
    
    int stepNum = MSLPManager::getInstance()->getProgress();
    if ( stepNum <=0 )
    {
        stepNum = 1;
    }
    
    MSLPManager::getInstance()->progress(stepNum);
    
//    ProductManager::getInstance()->setCurrentStep(stepNum);
    ProductManager::getInstance()->initSet();
    
    
    // create a scene. it's an autorelease object
//    auto scene = Debug_Index::createScene();
    
    
    
//    Scene* scene = Debug_Index::createScene();
//    director->runWithScene(scene);
    

//    MSLPManager::getInstance()->beginProgress(ProductManager::getInstance()->getCurrentStep());
    
    if (isResEmbeded)
    {
        Scene* scene = Debug_Index::createScene();
        director->runWithScene(scene);
    }
    else
    {
        log("cocos2d timeCheck scene load Start");
        ProductIndex::runContentScene();
        log("cocos2d timeCheck scene load Finish");
    }

    DRMManager::getInstance()->hideProgress();
    
    // run
//    director->runWithScene(scene);
    return true;
}

// This function will be called when the app is inactive. When comes a phone call,it's be invoked too
void AppDelegate::applicationDidEnterBackground() {
    Director::getInstance()->stopAnimation();

    MGTSoundManager::getInstance()->pauseAllEffects();
    MGTSoundManager::getInstance()->pauseAllNarrations();
    MGTSoundManager::getInstance()->pauseAllBgm();
    // if you use SimpleAudioEngine, it must be pause
    // SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
}

// this function will be called when the app is active again
void AppDelegate::applicationWillEnterForeground() {
    Director::getInstance()->startAnimation();

    MGTSoundManager::getInstance()->resumeAllEffects();
    MGTSoundManager::getInstance()->resumeAllNarrations();
    MGTSoundManager::getInstance()->resumeAllBgm();
    
    // if you use SimpleAudioEngine, it must resume here
    // SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
}
