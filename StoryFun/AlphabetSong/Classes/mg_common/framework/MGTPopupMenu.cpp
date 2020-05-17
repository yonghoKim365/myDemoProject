//
//  MGTPopupMenu.cpp
//  MGTTemplate
//
//   on 13. 1. 1..
//
//

#include "MGTPopupMenu.h"
#include "MGTDefines.h"

MGTPopupMenu *MGTPopupMenu::create(MenuItem* item, ...)
{
    va_list args;
    va_start(args,item);
    
    MGTPopupMenu *pRet = (MGTPopupMenu*)MGTPopupMenu::createWithItems(item, args);

    va_end(args);
    
    return pRet;
}

MGTPopupMenu* MGTPopupMenu::createWithArray(const Vector<MenuItem*>& arrayOfItems)
{
    auto pRet = new MGTPopupMenu();
    if (pRet && pRet->initWithArray(arrayOfItems))
    {
        pRet->autorelease();
    }
    else
    {
        CC_SAFE_DELETE(pRet);
    }
    
    return pRet;
}

MGTPopupMenu* MGTPopupMenu::createWithItem(MenuItem* item)
{
    return MGTPopupMenu::create(item, nullptr);
}

MGTPopupMenu* MGTPopupMenu::createWithItems(MenuItem* item, va_list args)
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
    
    return MGTPopupMenu::createWithArray(items);
}

// Fix 
//void MGTPopupMenu::registerWithTouchDispatcher()
//{
//    Director::getInstance()->getTouchDispatcher()->addTargetedDelegate(this, MGTTOUCH_PRIORITY_POPUPMENU, true);
//}
