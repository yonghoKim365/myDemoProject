//
//  MGTMenu.h
//  MGT_Template
//
// on 13. 5. 31..
//
//

#ifndef MGT_Template_MGTMenu_h
#define MGT_Template_MGTMenu_h

#include "MGTLayer.h"
#include "cocos2d.h"

using namespace cocos2d;

class MGTMenu : public Menu {
    

public:
    MGTMenu();
    ~MGTMenu();
    
    bool isTouching;
    Vec2 m_touchedStart;
    Vec2 m_touchesEnd;
    
    static MGTMenu* create();
    static MGTMenu* create(MenuItem* item, ...);
    static MGTMenu* createWithArray(const Vector<MenuItem*>& arrayOfItems);
    static MGTMenu* createWithItem(MenuItem* item);
    static MGTMenu* createWithItems(MenuItem *firstItem, va_list args);
    
    virtual bool init();
    virtual bool initWithArray(const Vector<MenuItem*>& arrayOfItems);

    virtual bool onTouchBegan(Touch* touch, Event* event);
    virtual void onTouchEnded(Touch* touch, Event* event);
    virtual void onTouchMoved(Touch* touch, Event* event);
    virtual void onTouchCancelled(Touch* touch, Event* event);
    
    // Fix 
//    virtual void registerWithTouchDispatcher();
};

#endif

