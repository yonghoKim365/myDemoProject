
#include "DimmedLayer.h"

#pragma mark - init


DimmedLayer::DimmedLayer()
{
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(DimmedLayer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(DimmedLayer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(DimmedLayer::onTouchEnded, this);
    
//    _eventDispatcher->addEventListenerWithFixedPriority(m_touchlistener, -100);
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);
}

DimmedLayer::~DimmedLayer()
{
    _eventDispatcher->removeEventListener(m_touchlistener);
}




DimmedLayer * DimmedLayer::create(const Color4B& color)
{
    DimmedLayer * pLayer = new DimmedLayer();
    if(pLayer && pLayer->initWithColor(color))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}

DimmedLayer * DimmedLayer::create(const Color4B& color, GLfloat width, GLfloat height)
{
    DimmedLayer * pLayer = new DimmedLayer();
    if( pLayer && pLayer->initWithColor(color,width,height))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool DimmedLayer::init()
{
    if (!LayerColor::init()) {
        return false;
    }
    
    return true;
}

bool DimmedLayer::initWithColor(const Color4B& color)
{
    if (!LayerColor::initWithColor(color)) {
        return false;
    }
    
    
    return true;
}

bool DimmedLayer::initWithColor(const Color4B& color, GLfloat w, GLfloat h)
{
    if (!LayerColor::initWithColor(color,w,h))
    {
        return false;
    }
    return true;
}

void DimmedLayer::onEnter()
{
    LayerColor::onEnter();
}

void DimmedLayer::onExit()
{
    LayerColor::onExit();
}


#pragma mark touches function

bool DimmedLayer::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();

    if (this->isVisible())
    {
        return true;
    }
    return false;
}

void DimmedLayer::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    Vec2 prePoint = pTouch->getPreviousLocation();
    
    
}
void DimmedLayer::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}



void DimmedLayer::show(float duration, float opacity)
{
    m_eState = eDimmedState_Show;
    
    this->setVisible(true);
    
    float val = 255.0f*opacity;
    log("val:%f", val);
    auto action = EaseSineOut::create(FadeTo::create(duration, val));
    auto seq = Sequence::create(action,
                                CallFunc::create( CC_CALLBACK_0(DimmedLayer::showComplete, this)),
                                nullptr);
    
    this->runAction(seq);
}

void DimmedLayer::showComplete()
{
    
}

void DimmedLayer::hide(float duration)
{
    m_eState = eDimmedState_Hide;
    
    auto action = EaseSineOut::create(FadeTo::create(duration, 0.0f));
    auto seq = Sequence::create(action,
                                CallFunc::create( CC_CALLBACK_0(DimmedLayer::hideComplete, this)),
                                nullptr);
    
    this->runAction(seq);
}

void DimmedLayer::hideComplete()
{
    this->setVisible(false);
}
