//
//  MGTSprite.cpp
//  MGT_Template
//
// on 13. 6. 21..
//
//

#include "MGTSprite.h"
#include "MGTUtils.h"
#include "MGTResourceUtils.h"

MGTSprite::MGTSprite()
{
    _defaultTouchAction = NULL;
    _defaultTouchActionReverse = NULL;
    
    _touchAction = NULL;
    _touchActionReverse = NULL;
    
    _bIsTouchEnable = false;
    _bTouchComplete = false;

    _callfuncSelector = NULL;
    _delegate = NULL;
}

MGTSprite::~MGTSprite()
{
    MGTUtils::stopAllAnimations(this);
    MGTUtils::removeAllchildren(this);
    
    CC_SAFE_RELEASE(_defaultTouchAction);
    CC_SAFE_RELEASE(_defaultTouchActionReverse);
    _defaultTouchAction = NULL;
    _defaultTouchActionReverse = NULL;
    
    CC_SAFE_RELEASE(_touchAction);
    CC_SAFE_RELEASE(_touchActionReverse);
    _touchAction = NULL;
    _touchActionReverse = NULL;
    
    _bIsTouchEnable = false;
}


MGTSprite* MGTSprite::create(const char *pszFileName)
{
    MGTSprite *pobSprite = new MGTSprite();
    if (pobSprite && pobSprite->initWithFile(pszFileName))
    {
        pobSprite->autorelease();
        return pobSprite;
    }
    CC_SAFE_DELETE(pobSprite);
    return NULL;

}

MGTSprite* MGTSprite::createWithFullPath(const char *pszFileName)
{
    MGTSprite *pobSprite = new MGTSprite();

    if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", pszFileName).c_str()))
    {
        pobSprite->autorelease();
        return pobSprite;
    }
    CC_SAFE_DELETE(pobSprite);
    return NULL;
}

MGTSprite* MGTSprite::createWithCommonPath(const char *pszFileName)
{
    MGTSprite *pobSprite = new MGTSprite();
    std::string str = pszFileName;
    if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getCommonFilePath("img", pszFileName).c_str()))
    {
        pobSprite->autorelease();
        return pobSprite;
    }
    CC_SAFE_DELETE(pobSprite);
    return NULL;

}

//MGTSprite* MGTSprite::createWithTexture(Texture2D *pTexture, const Rect& rect)
//{
//    MGTSprite *pobSprite = new MGTSprite();
//    if (pobSprite && pobSprite->initWithTexture(pTexture, rect))
//    {
//        pobSprite->autorelease();
//        return pobSprite;
//    }
//    CC_SAFE_DELETE(pobSprite);
//    return NULL;
//}

bool MGTSprite::initWithFile(const char *pszFilename)
{
    if (Sprite::initWithFile(pszFilename)) {
        setDefualtOption();
        return true;
    }
    return false;
}


bool MGTSprite::init()
{
    if (Sprite::init()) {
        setDefualtOption();
        return true;
    }
    return false;
}

void MGTSprite::onEnter()
{
    Sprite::onEnter();
    m_originPosition = this->getPosition();
}

void MGTSprite::onExit()
{
    
    Sprite::onExit();
}

#pragma mark - addchild override

void MGTSprite::addChild(Node *child)
{
    Sprite::addChild(child);
}

void MGTSprite::addChild(Node *child, int zOrder)
{
    Sprite::addChild(child, zOrder);
}

void MGTSprite::addChild(Node *child, int zOrder, int tag)
{
    if (child->getParent() != NULL) {
        child->removeFromParent();
    }
    Sprite::addChild(child, zOrder, tag);
    
}

void MGTSprite::addChildIgnoreParent(Node *child)
{
    if (child!= NULL && child->getParent() == this)
        return;
    
    addChild(child);
    this->_setPositionForParent(child);
}

void MGTSprite::addChildIgnoreParent(Node *child, int zOrder)
{
    if (child->getParent() == this)
        return;
    
    addChild(child, zOrder);
    this->_setPositionForParent(child);
}


void MGTSprite::addChildIgnoreParent(Node *child, int zOrder, int tag)
{
    if (child->getParent() == this)
        return;
    
    addChild(child, zOrder, tag);
    this->_setPositionForParent(child);
}



#pragma mark - touch delegate

bool MGTSprite::onTouchBegan(Touch* touch, Event* event)
{
    
    if (!_bIsTouchEnable)
        return false;
    
    Vec2 sLocalPos = convertToNodeSpace(touch->getLocation());
    Rect sRC = Rect(getPositionX() - getContentSize().width * getAnchorPoint().x,
                        getPositionY() - getContentSize().height * getAnchorPoint().y,
                        getContentSize().width, getContentSize().height);
    
    
    sRC.origin = Vec2::ZERO;
    bool isTouched = sRC.containsPoint(sLocalPos);
    if(isTouched)
    {
//        onTouchEvent();
        
        return true;
    }
    
    return false;
}

void MGTSprite::onTouchMoved(Touch* touch, Event* event)
{
    
}

void MGTSprite::onTouchEnded(Touch* touch, Event* event)
{
    
}

void MGTSprite::setTouchEnable(bool enable)
{
    // Fix 
//    if (enable == true) {
//        if(Director::getInstance()->getTouchDispatcher()->findHandler(this) == NULL){
//            Director* pDirector = Director::getInstance();
//            pDirector->getTouchDispatcher()->addTargetedDelegate(this, 0, false);
//        }
//    }

    _bIsTouchEnable = enable;
};


#pragma mark - set sprite option

void MGTSprite::setDefualtOption()
{
    _scale = this->getScale();
    _opacity = this->getOpacity();
    
    _defaultTouchAction = EaseElasticOut::create(ScaleTo::create(0.2, 1.2) , 0.7);
    _defaultTouchActionReverse = EaseElasticOut::create(ScaleTo::create(0.2, 1.0) , 0.7);
    _defaultTouchAction->retain();
    _defaultTouchActionReverse->retain();
}



//void MGTSprite::onTouchEvent()
//{
//    if (_bTouchComplete)
//        return;
//    
//    if (_callfuncSelector && _delegate) {
//        _delegate->runAction(CallFunc::create(_delegate, _callfuncSelector));
//    }
//}

void MGTSprite::setTouchEvent(Node* delegate, SEL_CallFunc selector)
{
    _delegate = delegate;
    _callfuncSelector = selector;
}





void MGTSprite::setTouchAction(Action* action)
{
    if (action) {
        _touchAction = action;
        _touchAction->retain();
    }

}

void MGTSprite::setTouchActionReverse(Action* action)
{
    if (action) {
        _touchActionReverse = action;
        _touchActionReverse->retain();
    }
}

void MGTSprite::onTouchActionEvent()
{
    if (_bIsTouchEnable)
    {
        stopAllActions();
        if (_touchAction) {
//            this->stopAction(_touchAction);
            this->runAction(_touchAction);
        }else{
//            this->stopAction(_defaultTouchAction);
            this->runAction(_defaultTouchAction);
        }
    }
}

void MGTSprite::onTouchEndActionEvent()
{
    if (_bIsTouchEnable)
    {
        stopAllActions();
        if (_touchActionReverse) {
//            this->stopAction(_touchActionReverse);
            this->runAction(_touchActionReverse);
        }else{

//            this->stopAction(_defaultTouchActionReverse);
            this->runAction(_defaultTouchActionReverse);
        }
    }
}


#pragma mark - position
void MGTSprite::setPositionForParent()
{
    Node* parent = this->getParent();
    Node* child = this;
    
    Size parentSize = parent->getContentSize();
    Vec2 parentAnchor = parent->getAnchorPoint();
    Vec2 childModifyPoint = child->getPosition() - parent->getPosition();
    child->setPosition(
                       Vec2(
                           childModifyPoint.x+parentSize.width*parentAnchor.x,
                           childModifyPoint.y+parentSize.height*parentAnchor.y
                           )
                       );

}

void MGTSprite::setPositionForRootParent(Node* rootParents)
{
    Node* parent = this->getParent();
    Node* child = this;
    
    while (parent != rootParents) {
        Size parentSize = parent->getContentSize();
        Vec2 parentAnchor = parent->getAnchorPoint();
        Vec2 childModifyPoint = child->getPosition() - parent->getPosition();
        child->setPosition(
                           Vec2(
                               childModifyPoint.x+parentSize.width*parentAnchor.x,
                               childModifyPoint.y+parentSize.height*parentAnchor.y
                               )
                           );
        
        parent = parent->getParent();
    }
    
}


void MGTSprite::_setPositionForParent(Node* child)
{
    MGTSprite* parent = this;
    
    Size parentSize = parent->getContentSize();
	Vec2 parentAnchor = parent->getAnchorPoint();
	Vec2 childModifyPoint = child->getPosition() - parent->getPosition();
	child->setPosition(
                       Vec2(
                           childModifyPoint.x+parentSize.width*parentAnchor.x,
                           childModifyPoint.y+parentSize.height*parentAnchor.y
                           )
                       );
}


void MGTSprite::setAnchorPointWithoutPosition(Vec2 anchor)
{
    Size size = this->getContentSize();
    Vec2 position = this->getPosition();
    Vec2 originAnchor = this->getAnchorPoint();
    
    this->setPosition(Vec2(position.x-(size.width*(originAnchor.x-anchor.x)), position.y-(size.height*(originAnchor.y-anchor.y))));
    this->setAnchorPoint(anchor);
}

#pragma mark - texture 

void MGTSprite::changeTexture(std::string filePath)
{
    this->setTexture(Director::getInstance()->getTextureCache()->addImage(filePath.c_str()));
}

void MGTSprite::changeTextureWithFullPath(std::string fileName)
{
    this->setTexture(Director::getInstance()->getTextureCache()->addImage( MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img",fileName).c_str() ));
}

void MGTSprite::changeTextureWithCommonPath(std::string fileName)
{
    this->setTexture(Director::getInstance()->getTextureCache()->addImage( MGTResourceUtils::getInstance()->getCommonFilePath("img",fileName).c_str() ));
}

