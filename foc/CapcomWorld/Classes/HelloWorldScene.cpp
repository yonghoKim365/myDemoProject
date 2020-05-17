#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"
#include "CardDictionary.h"
#include "TitleScene.h"
#include "KakaoLoginScene.h"
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
//#include "XBridge.h"
#endif


using namespace cocos2d;
using namespace CocosDenshion;

CCScene* HelloWorld::scene()
{
    // 'scene' is an autorelease object
    CCScene *scene = CCScene::create();
    
    // 'layer' is an autorelease object
    HelloWorld *layer = HelloWorld::create();
    
    // add layer as a child to scene
    scene->addChild(layer);
    
    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !CCLayer::init() )
    {
        return false;
    }
    
    CardDictionary::sharedCardDictionary()->init();
    FileManager::sharedFileManager()->Init();
    LocalizationManager::getInstance()->init();
    
    this->removeChildByTag(99, true);
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    //CCSprite* pSprite = CCSprite::create("title/START_01_CAPCOM.png");
    CCSprite* pSprite = CCSprite::create("title/logo_kakao.PNG");
    pSprite->setTag(99);
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    
    //this->schedule(schedule_selector(HelloWorld::logoApd),2.0);
    this->schedule(schedule_selector(HelloWorld::logoCapcom),1.0);
    
    return true;
}
/*
void HelloWorld::logoKakao()
{
    this->removeChildByTag(99, true);
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* pSprite = CCSprite::create("title/logo_kakao.png");
    pSprite->setTag(99);
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    
    this->schedule(schedule_selector(HelloWorld::goTitleScene),1.0);
    
}
 */

void HelloWorld::logoCapcom()
{
    this->removeChildByTag(99, true);
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* pSprite = CCSprite::create("title/START_01_CAPCOM.png");
    pSprite->setTag(99);
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    
    this->schedule(schedule_selector(HelloWorld::logoApd),1.0);
    
}


void HelloWorld::logoApd()
{
    this->removeChildByTag(99, true);
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* pSprite = CCSprite::create("title/START_02_APD.png");
    pSprite->setTag(99);
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    
    this->schedule(schedule_selector(HelloWorld::goTitleScene),1.0);
    
}


void HelloWorld::goTitleScene(){
    CCDirector::sharedDirector()->replaceScene(TitleScene::scene());
    //CCDirector::sharedDirector()->replaceScene(KakaoLoginScene::scene());
}