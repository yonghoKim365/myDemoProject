
#include "MGTMenu.h"

MGTMenu::MGTMenu()
{
    
}

MGTMenu::~MGTMenu()
{
    
}

MGTMenu* MGTMenu::create()
{
    return MGTMenu::create(nullptr, nullptr);
}

MGTMenu * MGTMenu::create(MenuItem* item, ...)
{
    va_list args;
    va_start(args,item);
    
    MGTMenu *pRet = MGTMenu::createWithItems(item, args);
    
    va_end(args);
    
    return pRet;
}

MGTMenu* MGTMenu::createWithArray(const Vector<MenuItem*>& arrayOfItems)
{
    auto ret = new (std::nothrow) MGTMenu();
    if (ret && ret->initWithArray(arrayOfItems))
    {
        ret->autorelease();
    }
    else
    {
        CC_SAFE_DELETE(ret);
    }
    
    return ret;
}

MGTMenu* MGTMenu::createWithItems(MenuItem* item, va_list args)
{
    Vector<MenuItem*> items;
    if( item )
    {
        items.pushBack(item);
        MenuItem *i = va_arg(args, MenuItem*);
        while(i)
        {
            items.pushBack(i);
            i = va_arg(args, MenuItem*);
        }
    }
    
    return MGTMenu::createWithArray(items);
}

MGTMenu* MGTMenu::createWithItem(MenuItem* item)
{
    return MGTMenu::create(item, nullptr);
}



bool MGTMenu::init()
{
    return initWithArray(Vector<MenuItem*>());
}


bool MGTMenu::initWithArray(const Vector<MenuItem*>& arrayOfItems)
{
//    if (Menu::init())
//    {
//        auto touchListener = EventListenerTouchOneByOne::create();
//        touchListener->setSwallowTouches(true);
//        
//        touchListener->onTouchBegan = CC_CALLBACK_2(MGTMenu::onTouchBegan, this);
//        touchListener->onTouchMoved = CC_CALLBACK_2(MGTMenu::onTouchMoved, this);
//        touchListener->onTouchEnded = CC_CALLBACK_2(MGTMenu::onTouchEnded, this);
//        touchListener->onTouchCancelled = CC_CALLBACK_2(MGTMenu::onTouchCancelled, this);
//        
////        _eventDispatcher->addEventListenerWithSceneGraphPriority(touchListener, this);
//        _eventDispatcher->addEventListenerWithFixedPriority(touchListener, -100);
//        
//        return true;
//    }
//    return false;

    if (Layer::init())
    {
        _enabled = true;
        // menu in the center of the screen
        Size s = Director::getInstance()->getWinSize();
        
        this->ignoreAnchorPointForPosition(true);
        setAnchorPoint(Vec2(0.5f, 0.5f));
        this->setContentSize(s);
        
        setPosition(s.width/2, s.height/2);
        
        int z=0;
        
        for (auto& item : arrayOfItems)
        {
            this->addChild(item, z);
            z++;
        }
        
        _selectedItem = nullptr;
        _state = Menu::State::WAITING;
        
        // enable cascade color and opacity on menus
        setCascadeColorEnabled(true);
        setCascadeOpacityEnabled(true);
        
        
        auto touchListener = EventListenerTouchOneByOne::create();
        touchListener->setSwallowTouches(true);
        
        touchListener->onTouchBegan = CC_CALLBACK_2(Menu::onTouchBegan, this);
        touchListener->onTouchMoved = CC_CALLBACK_2(Menu::onTouchMoved, this);
        touchListener->onTouchEnded = CC_CALLBACK_2(Menu::onTouchEnded, this);
        touchListener->onTouchCancelled = CC_CALLBACK_2(Menu::onTouchCancelled, this);
        
//        _eventDispatcher->addEventListenerWithSceneGraphPriority(touchListener, this);
        _eventDispatcher->addEventListenerWithFixedPriority(touchListener, -256);
        
        return true;
    }
    return false;
}







// Fix 
//void MGTMenu::registerWithTouchDispatcher()
//{
//    Director* pDirector = Director::getInstance();
//    pDirector->getTouchDispatcher()->addTargetedDelegate(this, this->getTouchPriority(), false);
//}


bool MGTMenu::onTouchBegan(Touch* touch, Event* event)
{
    m_touchedStart = touch->getStartLocation();
    
    return Menu::onTouchBegan(touch, event);
}

void MGTMenu::onTouchEnded(Touch *touch, Event* event)
{
    m_touchesEnd = touch ->getLocation();
    
    Vec2 different = m_touchesEnd - m_touchedStart;
    
    if (different.x > 5.0f || different.y < -5.0f)
    {
        Menu::onTouchCancelled(touch, event);
    }
    else if( different.x < -5.0f || different.y < -5.0f)
    {
        Menu::onTouchCancelled(touch, event);
    }
    else
    {
        Menu::onTouchEnded(touch, event);
    }
}

void MGTMenu::onTouchMoved(Touch* touch, Event* event)
{
    Menu::onTouchMoved(touch, event);
}

void MGTMenu::onTouchCancelled(Touch* touch, Event* event)
{
    Menu::onTouchCancelled(touch, event);

}