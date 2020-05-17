
#include "MGTLayer.h"
#include "MGTDefines.h"

//Scene* MGTLayer::scene()
//{
//    Scene *scene = Scene::create();
//    MGTLayer* layer = MGTLayer::create();
//    scene->addChild(layer);
//    return scene;
//}

#pragma mark - init

MGTLayer::MGTLayer()
{
    _imgBackground = NULL;
//    _vTouchEnableMGTSprites.clear();
}

MGTLayer::~MGTLayer()
{
//    _vTouchEnableMGTSprites.clear();
}


MGTLayer * MGTLayer::create(const Color4B& color)
{
    MGTLayer * pLayer = new MGTLayer();
    if(pLayer && pLayer->initWithColor(color))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}

MGTLayer * MGTLayer::create(const Color4B& color, GLfloat width, GLfloat height)
{
    MGTLayer * pLayer = new MGTLayer();
    if( pLayer && pLayer->initWithColor(color,width,height))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool MGTLayer::init()
{
    if (!LayerColor::init()) {
        return false;
    }
    
    _currentTime = 0;
    
    m_bIsCompleteFlashParsing = false;
    m_bIsCompleteTransition = false;
    
    winSize = Director::getInstance()->getWinSize();
    winHalfSize = Size(winSize.width/2, winSize.height/2);
    winCenter = Vec2(winSize.width/2, winSize.height/2);
    
    return true;
}

bool MGTLayer::initWithColor(const Color4B& color)
{
    if (!LayerColor::initWithColor(color)) {
        return false;
    }
    return true;
}

bool MGTLayer::initWithColor(const Color4B& color, GLfloat w, GLfloat h)
{
    if (!LayerColor::initWithColor(color,w,h))
    {
        return false;
    }
    return true;
}


void MGTLayer::onEnter()
{
    LayerColor::onEnter();
    

//#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
//    winCenter = Vec2(winSize.width/2, (winSize.height+24)/2);3
//#endif
    
//    setTouchEnableMGTLayer(NULL, true);
//    MGTUtils::performSelector(this, callfuncND_selector(MGTLayer::setTouchEnableMGTLayer), (void*)true , 0.5);
    
}

void MGTLayer::onEnterTransitionDidFinish()
{
    LayerColor::onEnterTransitionDidFinish();
    
    onViewLoaded();
}

void MGTLayer::onExit()
{
    MGTUtils::stopAllAnimations(this);
    MGTUtils::removeAllchildren(this);
//    Director::getInstance()->getTextureCache()->removeUnusedTextures();
//    Director::getInstance()->getTextureCache()->removeAllTextures();

    LayerColor::onExit();
}

#pragma mark - load view

void MGTLayer::onViewLoad()
{
    /* override */
}


void MGTLayer::onViewLoaded()
{
    /* override */
}


void MGTLayer::setBackgroundColor(const Color4B& color)
{
    // default blend function
    _blendFunc = BlendFunc::ALPHA_NON_PREMULTIPLIED;
    
    _displayedColor.r = _realColor.r = color.r;
    _displayedColor.g = _realColor.g = color.g;
    _displayedColor.b = _realColor.b = color.b;
    _displayedOpacity = _realOpacity = color.a;
    
    for (size_t i = 0; i<sizeof(_squareVertices) / sizeof( _squareVertices[0]); i++ )
    {
        _squareVertices[i].x = 0.0f;
        _squareVertices[i].y = 0.0f;
    }
    
    updateColor();
    setGLProgramState(GLProgramState::getOrCreateWithGLProgramName(GLProgram::SHADER_NAME_POSITION_COLOR_NO_MVP));
}

void MGTLayer::addChild(Node *child, int zOrder, int tag)
{
    CCASSERT( child != nullptr, "Argument must be non-nil");
    if (child->getParent() != NULL) {
        child->removeFromParent();
    }
    Node::addChild(child, zOrder, tag);
}

void MGTLayer::addChild(Node *child, int zOrder)
{
    CCASSERT( child != nullptr, "Argument must be non-nil");
    Node::addChild(child, zOrder);
}

void MGTLayer::addChild(Node *child)
{
    CCASSERT( child != nullptr, "Argument must be non-nil");
    Node::addChild(child);
}



#pragma mark - touch

bool MGTLayer::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    return false;
}

void MGTLayer::onTouchMoved(Touch *pTouch, Event *pEvent)
{
}

void MGTLayer::onTouchEnded(Touch *pTouch, Event *pEvent)
{
}

void MGTLayer::onTouchCancelled(Touch *pTouch, Event *pEvent)
{
    
}


//void MGTLayer::setTouchEnableMGTLayer(Node* sender, bool bDispatchEvents)
//{
//
//    // Fix 
////    if(Director::getInstance()->getTouchDispatcher()->findHandler(this) == NULL){
////        Director::getInstance()->getTouchDispatcher()->addTargetedDelegate(this, 0, true);
////    }
////    Director::getInstance()->getTouchDispatcher()->setDispatchEvents(bDispatchEvents);
//}
//
//void MGTLayer::setTouchEnableMGTSprite(Node* sender, bool bDispatchEvents)
//{
//    if (bDispatchEvents) {
//        //MGTSprite setTouchEnable->true (just resume)
//        if (_vTouchEnableMGTSprites.size() > 0)
//        {
//            for (int i = 0 ; i < _vTouchEnableMGTSprites.size(); i++)
//            {
//                _vTouchEnableMGTSprites[i]->setTouchEnable(true);
//            }
//            _vTouchEnableMGTSprites.clear();
//        }
//        
//    }else{
//        //MGTSprite setTouchEnable->false
//        for(int i=0; i < this->getChildrenCount(); i++)
//        {
//            auto child = this->getChildren().at(i);
//            if(dynamic_cast<MGTSprite *>(child))
//            {
//                MGTSprite* lhSp = (MGTSprite*)child;
//                if (lhSp->isTouchEnabled())
//                {
//                    
//                    lhSp->setTouchEnable(false);
//                    _vTouchEnableMGTSprites.push_back(lhSp);
//                }
//            }
//        }
//    }
//}

#pragma mark - getFilePath



std::string MGTLayer::getFilePath(ResourceType type, std::string path)
{
    std::string fullPath = MGTResourceUtils::getInstance()->getFilePath(type, path);
    return fullPath;
}

std::string MGTLayer::getFilePath(ResourceType type, std::string strFolderName, std::string strFileName)
{
    std::string fullPath = MGTResourceUtils::getInstance()->getFilePath(type, strFolderName, strFileName);
    return fullPath;
    
}

std::string MGTLayer::getCommonFilePath(std::string path)
{
    std::string fullPath = MGTResourceUtils::getInstance()->getCommonFilePath(path);
    return fullPath;
}

std::string MGTLayer::getCommonFilePath(std::string strFolderName, std::string strFileName)
{
    std::string fullPath = MGTResourceUtils::getInstance()->getCommonFilePath(strFolderName, strFileName);
    return fullPath;
    
}

std::string MGTLayer::getImageFilePath(std::string path)
{
    std::string imagePath = MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", path);
    return imagePath;
}

std::string MGTLayer::getSoundFilePath(std::string path)
{
    std::string soundPath = MGTResourceUtils::getInstance()->getFilePath(ResourceType::SOUND, "snd",path);
    return soundPath;
}


#pragma mark - scheduler
void MGTLayer::onTimer()
{
    _currentTime = 0;
    schedule(CC_SCHEDULE_SELECTOR(MGTLayer::_tick), 1.0/10.0f);
}

void MGTLayer::stopTimer()
{
    unschedule(CC_SCHEDULE_SELECTOR(MGTLayer::_tick));
    _currentTime = 0;
}

void MGTLayer::_tick(float dt)
{
    _currentTime = _currentTime+1;
}

float MGTLayer::getCurrentTime()
{
    return (float)_currentTime/10.0f;
}




#pragma mark - background
////////////////////////////////////////////////////
//
// Background
//
////////////////////////////////////////////////////
void MGTLayer::_setBackground(Sprite *pSprite)
{
    if(_imgBackground != NULL)
    {
        if(_imgBackground->getParent() != NULL)
        {
            this->removeChild(_imgBackground, true);
        }
    }
    
    _imgBackground = pSprite;
    _imgBackground->setPosition(winCenter);
    
    this->addChild(_imgBackground, 0);
}


void MGTLayer::setBackground(Sprite *pSprite)
{
    if(pSprite != NULL)
    {
        this->_setBackground(pSprite);
    }
}

void MGTLayer::setBackground(std::string strImageName, bool bIsLanguage)
{
    MGTSprite *tempSprite = MGTSprite::createWithFullPath(strImageName.c_str());
    
    this->setBackground(tempSprite);
}

void MGTLayer::setBackgroundPosition(Vec2 aPoint)
{
    if(_imgBackground != NULL)
    {
        _imgBackground->setPosition(aPoint);
    }
}

void MGTLayer::setBackgroundScale(float fScale)
{
    if(_imgBackground != NULL)
    {
        _imgBackground->setScale(fScale);
    }
}


Sprite* MGTLayer::getBackgroundSprite()
{
    if(_imgBackground != NULL)
    {
        return _imgBackground;
    }
    else
    {
        return NULL;
    }
}


#pragma mark - utils

//void MGTLayer::removeAllpointArray(PointArray* pointArray)
//{
//    int count = (int)pointArray->count();
//    for (int i = 0; i < count; i++) {
//        pointArray->removeControlPointAtIndex(count-1-i);
//    }
//}

const char* MGTLayer::stringFromRect(Node* node)
{
    printf("cclogframe:");
    __String* rect = __String::createWithFormat("(%.1f, %.1f), (%.1f, %.1f)",node->getPositionX(),node->getPositionY(),node->getContentSize().width,node->getContentSize().height);
    return rect->getCString();
}




#pragma mark - Sound

void MGTLayer::addSoundObserver()
{
    __NotificationCenter::getInstance()->addObserver(
                                                     this,
                                                     CC_CALLFUNCO_SELECTOR(MGTLayer::_onNarrationFinishedCallback),
                                                     MGTNOTIFICATION_NAR_FINISHCALL,
                                                     nullptr);
}

void MGTLayer::removeSoundObserver()
{
    __NotificationCenter::getInstance()->removeObserver(this, MGTNOTIFICATION_NAR_FINISHCALL);
}


// private : Don't use this function
void MGTLayer::_onNarrationFinishedCallback(Ref *sender)
{
    
    cocos2d::Value* fullPath = (cocos2d::Value*)sender;
    
    std::string name = MGTUtils::getInstance()->stringTokenizer(fullPath->asString(), "/", false);
    if (this) {
        onNarrationFinishedCallback( name );
    }
    
}

//// public : using this function to override
//void Base_Layer::onNarrationFinishedCallback(std::string fileName)
//{
//    /* override me */
//}
