
#include "Base_Layer.h"
#include "Navigation.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "MusicFade.h"
#include "GAFManager.h"

enum
{
    kTagDebugSpeedBg = 10000,
    kTagDebugSpeedMenu,
    kTagDebugSpeedBtn1,
    kTagDebugSpeedBtn3,
};

////////////////////////////////////////////////////////
//
// Class initalize functions.
//
////////////////////////////////////////////////////////

Base_Layer::Base_Layer()
:
m_asset(nullptr),
m_gafObject(nullptr),
m_touchEnabled(false)
{
    m_fadeVolume_Duration = 0.5f;
    m_fadeVolume_Min = 0.3f;
    m_fadeVolume_Min = 1.0f;
    
    
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(cocos2d::Layer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(cocos2d::Layer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(cocos2d::Layer::onTouchEnded, this);
    
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);
    
    addSoundObserver();
}


Base_Layer::~Base_Layer()
{
    removeSoundObserver();
    
    _eventDispatcher->removeEventListener(m_touchlistener);
    
    CC_SAFE_RELEASE(m_asset);
}


bool Base_Layer::init()
{
    if (!MGTLayer::init()) {
        return false;
    }
    
    MGTSoundManager::getInstance()->stopAllSounds();
    
    
    return true;
}


bool Base_Layer::initWithColor(const Color4B& color)
{
    if (!MGTLayer::initWithColor(color)) {
        return false;
    }
    return true;
}

void Base_Layer::onEnter()
{
    MGTLayer::onEnter();
}

void Base_Layer::onExit()
{
    MGTLayer::onExit();
    
    Director::getInstance()->getScheduler()->setTimeScale(1.0f);
}


void Base_Layer::onViewLoad()
{
    MGTLayer::onViewLoad();
    
    log("Base_Layer onviewload");
    
//#if PRODUCT_DEBUGMODE_SPEED == 1
//    Director::getInstance()->getScheduler()->setTimeScale(1.0f);
//    
//    Sprite* debugBg = MGTSprite::createWithCommonPath("common_debug_speed_bg.png");
//    debugBg->setPosition(Vec2(winSize.width/2, winSize.height - debugBg->getContentSize().height/2));
//    this->addChild(debugBg, kTagDebugSpeedBg, kTagDebugSpeedBg);
//    
//    
//    
//    Sprite* normalSp;
//    Sprite* selectedSp;
//    Sprite* disableSp;
//    
//    normalSp = MGTSprite::createWithCommonPath("common_debug_btn_speed1_n.png");
//    selectedSp = MGTSprite::createWithCommonPath("common_debug_btn_speed1_s.png");
//    disableSp = MGTSprite::createWithCommonPath("common_debug_btn_speed1_d.png");
//    auto speed1Btn = MenuItemSprite::create(normalSp,
//                                            selectedSp,
//                                            disableSp,
//                                            CC_CALLBACK_1(Base_Layer::debugSpeedCallback, this) );
//    
//    Size btnSize = speed1Btn->getContentSize();
//    
//    Vec2 pos;
//    pos = Vec2(winSize.width/2-30, winSize.height - btnSize.height/2 - 6);
//    speed1Btn->setPosition(pos);
//    speed1Btn->setTag(kTagDebugSpeedBtn1);
//    speed1Btn->setEnabled(false);
//    
//    log("pos:%f, %f", winSize.height, btnSize.height);
//    
//    normalSp = MGTSprite::createWithCommonPath("common_debug_btn_speed3_n.png");
//    selectedSp = MGTSprite::createWithCommonPath("common_debug_btn_speed3_s.png");
//    disableSp = MGTSprite::createWithCommonPath("common_debug_btn_speed3_d.png");
//    
//    auto speed3Btn = MenuItemSprite::create(normalSp,
//                                            selectedSp,
//                                            disableSp,
//                                            CC_CALLBACK_1(Base_Layer::debugSpeedCallback, this) );
//    
//    pos = Vec2(winSize.width/2+30, winSize.height - btnSize.height/2 - 6);
//    speed3Btn->setPosition(pos);
//    speed3Btn->setTag(kTagDebugSpeedBtn3);
//    log("pos:%f, %f", pos.x, pos.y);
//    
//    auto debugSpeedMenu = Menu::create(speed1Btn, speed3Btn, nullptr);
//    debugSpeedMenu->setPosition(Vec2::ZERO);
//    this->addChild(debugSpeedMenu, kTagDebugSpeedMenu, kTagDebugSpeedMenu);
//#endif
    
}


void Base_Layer::onViewLoaded()
{
    MGTLayer::onViewLoaded();
}

void Base_Layer::debugSpeedCallback(Ref* sender)
{
    auto btn = (MenuItemSprite*)sender;
    if (btn->getTag() == kTagDebugSpeedBtn1)
    {
        Director::getInstance()->getScheduler()->setTimeScale(1.0f);
        btn->selected();
        btn->setEnabled(false);
        
        auto menu = this->getChildByTag(kTagDebugSpeedMenu);
        MenuItemSprite* btn = (MenuItemSprite*)menu->getChildByTag(kTagDebugSpeedBtn3);
        btn->setEnabled(true);
        
        
    }
    else
    {
        Director::getInstance()->getScheduler()->setTimeScale(3.0f);
        btn->selected();
        btn->setEnabled(false);
        
        auto menu = this->getChildByTag(kTagDebugSpeedMenu);
        MenuItemSprite* btn = (MenuItemSprite*)menu->getChildByTag(kTagDebugSpeedBtn1);
        btn->setEnabled(true);
    }
}

#pragma mark gaf function
GAFObject* Base_Layer::createGAFObject(std::string path, bool isloop, Vec2 position)
{
    return createGAFObject(this, path, isloop, position);
}

GAFObject* Base_Layer::createGAFObject(Node* parent, std::string path, bool isloop, Vec2 position)
{
    GAFObject* object = nullptr;
    GAFAsset* asset = GAFManager::getInstance()->getGafAsset(path);
    
    if (asset)
    {
        object = asset->createObjectAndRun(isloop);
        
        float scaleFactor = cocos2d::Director::getInstance()->getContentScaleFactor();
//        object->setAnchorPoint(cocos2d::Vec2(0.5, 0.5));
//        Size screenSize = winSize / scaleFactor;
//        Vec2 pos = Vec2(screenSize.width/2, screenSize.height/2);
//        object->setPosition(pos);
        
        if (object)
        {
            object->setLocator(true);
            object->setPosition(position);
            parent->addChild(object);
        }
        
        asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
    }
    return object;
}

GAFObject* Base_Layer::createGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position)
{
    GAFObject* object = nullptr;
    GAFAsset* asset = GAFManager::getInstance()->getGafAsset(path);
    
    
    log("createGAFObjcet path:%s", path.c_str());
    
//    asset = GAFAsset::create(path, nullptr);
    
    log("createGAFObjcet 2");
    if (asset)
    {
        log("createGAFObjcet 3");
        
        
        object = asset->createObjectAndRun(isloop);
        
        if (object)
        {
            log("createGAFObjcet 4");
            object->setLocator(true);
            log("createGAFObjcet 5");
            object->setPosition(position);
            parent->addChild(object, zOrder);
        }
        
        asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        
    }
    
    log("createGAFObjcet 6");
    return object;
}




GAFObject* Base_Layer::createCommonGAFObject(std::string path, bool isloop, Vec2 position)
{
    return createCommonGAFObject(this, path, isloop, position);
}

GAFObject* Base_Layer::createCommonGAFObject(Node* parent, std::string path, bool isloop, Vec2 position)
{
    GAFObject* object = nullptr;
    GAFAsset* asset = GAFManager::getInstance()->getCommonGafAsset(path);
    
//    if (!asset)
//    {
//        asset = GAFAsset::create(path, nullptr);
//        asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
//        
//        CC_SAFE_RETAIN(asset);
//    }
    
    if (asset)
    {
        asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        object = asset->createObjectAndRun(isloop);
        
        float scaleFactor = cocos2d::Director::getInstance()->getContentScaleFactor();
        //        object->setAnchorPoint(cocos2d::Vec2(0.5, 0.5));
        //        Size screenSize = winSize / scaleFactor;
        //        Vec2 pos = Vec2(screenSize.width/2, screenSize.height/2);
        //        object->setPosition(pos);
        
        if (object)
        {
            object->setLocator(true);
            object->setPosition(position);
            parent->addChild(object);
        }
    }
    return object;
}



GAFObject* Base_Layer::createCommonGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position)
{
    GAFObject* object = nullptr;
    GAFAsset* asset = GAFManager::getInstance()->getCommonGafAsset(path);
    
    if (asset)
    {
        asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        
        object = asset->createObjectAndRun(isloop);
        object->setPosition(position);
        parent->addChild(object, zOrder);
    }
    return object;
}

void Base_Layer::removeGAFObject(GAFObject* object)
{
    removeChild(object);
}

void Base_Layer::restartGAFObject(GAFObject* object)
{
    object->stop();
    object->start();
}

void Base_Layer::playGAFObject(GAFObject* object)
{
    if (!object->getIsAnimationRunning())
    {
        object->resumeAnimation();
    }
}

void Base_Layer::pauseGAFObject(GAFObject* object)
{
    if(object->getIsAnimationRunning())
    {
        object->pauseAnimation();
    }
}

int Base_Layer::maxFrameNumberGAFObject(GAFObject* object)
{
    return object->getTotalFrameCount();
}

void Base_Layer::setFrameNumberGAFObject(GAFObject* object, int aFrameNumber)
{
    object->setFrame(aFrameNumber);
}

int Base_Layer::getCurrentFrameNumberGAFObject(GAFObject* object)
{
    return object->getCurrentFrameIndex();
}

bool Base_Layer::hitTestGAFObject(GAFObject* obj, Touch* touch)
{
    Vec2 localPoint = obj->convertTouchToNodeSpace(touch);
    localPoint = cocos2d::PointApplyTransform(localPoint, obj->getNodeToParentTransform());
    
    Rect r = obj->getBoundingBoxForCurrentFrame();
    return r.containsPoint(localPoint);
}



#pragma mark common function

Vec2 Base_Layer::getLocalPoint(Node* childnode)
{
    float fAnchorX = childnode->getAnchorPoint().x;
    float fAnchorY = childnode->getAnchorPoint().y;
    
    auto nodeCenterPt = Vec2(childnode->getBoundingBox().origin.x + childnode->getBoundingBox().size.width * fAnchorX,
                             childnode->getBoundingBox().origin.y + childnode->getBoundingBox().size.height * fAnchorY);
    
    return childnode->getParent()->convertToWorldSpace(nodeCenterPt);
}

#pragma mark gaf sound delegate function

void Base_Layer::onSoundPlay( GAFObject * object, const std::string& soundName )
{
    std::string::size_type pos;
    
    log("origin sound name:%s", soundName.c_str());
    
    std::string tempStr         = soundName;
    std::string delimiterAt     = "@";
    tempStr = tempStr.substr(0, tempStr.find(delimiterAt));
    
    std::string sndName = tempStr;
    
    log("sound name:%s", sndName.c_str());
    
    pos = sndName.find("bgm");
    if (pos != std::string::npos)
    {
        std::string name = sndName.substr(pos + 5);
//        std::string name = sndName;
        
        if (name == "in")
        {
            this->onBGMFadeInForGAF();
        }
        else if( name == "out")
        {
            this->onBGMFadeOutForGAF();
        }
        else
        {
            std::string bgmName = name + ".mp3";
            this->onBGMPlayForGAF(bgmName);
        }
    }
    
    pos = sndName.find("snd");
    if (pos != std::string::npos)
    {
        std::string narrationName = sndName.substr(pos + 5) + ".mp3";
//        std::string narrationName = sndName + ".mp3";
        onNarrationPlayForGAF(narrationName);
    }
    
    pos = sndName.find("sfx");
    if (pos != std::string::npos)
    {
        std::string effectName = sndName.substr(pos + 5) + ".mp3";
//        std::string effectName = sndName + ".mp3";
        onEffectPlayForGAF(effectName);
    }
}


void Base_Layer::onSoundEvent(GAFSoundInfo* sound, int32_t repeat, GAFSoundInfo::SyncEvent syncEvent)
{
    cocos2d::log("%s", sound->source.c_str());
    
    std::string sndFile = sound->source;
    std::string::size_type pos;
    
    pos = sndFile.find("common");
    if (pos != std::string::npos)
    {
        pos = sndFile.find("snd");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", sndFile));
            return;
        }
        
        pos = sndFile.find("sfx");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", sndFile));
            return;
        }
        
        pos = sndFile.find("bgm");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playBgm(getCommonFilePath("snd", sndFile));
            return;
        }
    }
    else
    {
        pos = sndFile.find("snd");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", sndFile));
            return;
        }
        
        pos = sndFile.find("sfx");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playEffect(getFilePath(ResourceType::SOUND, "snd", sndFile));
            return;
        }
        
        pos = sndFile.find("bgm");
        if (pos != std::string::npos)
        {
            MGTSoundManager::getInstance()->playBgm(getFilePath(ResourceType::SOUND, "snd", sndFile));
            return;
        }
    }
    
}

#pragma mark gaf sound play override function

void Base_Layer::onNarrationPlayForGAF(const std::string filename)
{
    std::string::size_type pos;
    
    pos = filename.find("common");
    if (pos != std::string::npos)
    {
        MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", filename));
    }
    else
    {
        MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", filename));
    }
}

void Base_Layer::onEffectPlayForGAF(const std::string filename)
{
    std::string::size_type pos;
    
    pos = filename.find("common");
    if (pos != std::string::npos)
    {
        MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", filename));
    }
    else
    {
        MGTSoundManager::getInstance()->playEffect(getFilePath(ResourceType::SOUND, "snd", filename));
    }
}

void Base_Layer::onBGMPlayForGAF(const std::string filename)
{
    std::string::size_type pos;
    
    pos = filename.find("common");
    if (pos != std::string::npos)
    {
        MGTSoundManager::getInstance()->playBgm(getCommonFilePath("snd", filename));
    }
    else
    {
        MGTSoundManager::getInstance()->playBgm(getFilePath(ResourceType::SOUND, "snd", filename));
    }
}

void Base_Layer::onBGMFadeInForGAF()
{
    auto action = MusicFade::create(m_fadeVolume_Duration, m_fadeVolume_Max);
    this->runAction(action);
}

void Base_Layer::onBGMFadeOutForGAF()
{
    auto action = MusicFade::create(m_fadeVolume_Duration, m_fadeVolume_Min);
    this->runAction(action);
}

