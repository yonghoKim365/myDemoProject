//
//  CapcomWorldAppDelegate.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 19..
//  Copyright __MyCompanyName__ 2012년. All rights reserved.
//

#include "AppDelegate.h"

#include "cocos2d.h"
#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h" 
#include "GameConst.h"

USING_NS_CC;

AppDelegate::AppDelegate()
{

}

AppDelegate::~AppDelegate()
{
    
}

bool AppDelegate::applicationDidFinishLaunching()
{
    // initialize director
    CCDirector *pDirector = CCDirector::sharedDirector();
    pDirector->setOpenGLView(CCEGLView::sharedOpenGLView());
    
    
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    

    GameConst::WIN_SIZE = CCSizeMake(320, 480);
    
    // enable High Resource Mode(2x, such as iphone4) and maintains low resource on other devices.
    if (!pDirector->enableRetinaDisplay(true))
        pDirector->setContentScaleFactor(2.0f);


//    GameConst::WIN_SIZE = CCSizeMake(640,960);
//    CCEGLView::sharedOpenGLView()->setDesignResolutionSize(GameConst::WIN_SIZE.width, GameConst::WIN_SIZE.height, kResolutionShowAll);
    
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    
    GameConst::WIN_SIZE = CCSizeMake(640, 960);
    
    //    CCEGLView::sharedOpenGLView()->setFrameSize(size.width,size.height);
    CCEGLView::sharedOpenGLView()->setDesignResolutionSize(GameConst::WIN_SIZE.width, GameConst::WIN_SIZE.height, kResolutionShowAll);
#endif

    // turn on display FPS
    pDirector->setDisplayStats(false);//true);
    
    // set FPS. the default value is 1.0/60 if you don't call this
    pDirector->setAnimationInterval(1.0 / 60);

    // create a scene. it's an autorelease object
    CCScene *pScene = HelloWorld::scene();

    // run
    pDirector->runWithScene(pScene);

    return true;
}

// This function will be called when the app is inactive. When comes a phone call,it's be invoked too
void AppDelegate::applicationDidEnterBackground()
{
    CCDirector::sharedDirector()->stopAnimation();
    
    CCDirector::sharedDirector()->pause();
    
    CocosDenshion::SimpleAudioEngine::sharedEngine()->pauseBackgroundMusic();
    
    // if you use SimpleAudioEngine, it must be pause
    // SimpleAudioEngine::sharedEngine()->pauseBackgroundMusic();
}

// this function will be called when the app is active again
void AppDelegate::applicationWillEnterForeground()
{
    CCDirector::sharedDirector()->stopAnimation();
    
    CCDirector::sharedDirector()->resume();
    
    CCDirector::sharedDirector()->startAnimation();
    
    CocosDenshion::SimpleAudioEngine::sharedEngine()->resumeBackgroundMusic();
    // if you use SimpleAudioEngine, it must resume here
    // SimpleAudioEngine::sharedEngine()->resumeBackgroundMusic();
}
