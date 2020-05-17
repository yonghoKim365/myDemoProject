//
//  MGTPopupMenu.h
//  MGTTemplate
//
//   on 13. 1. 1..
//
//

#ifndef __MGTTemplate__MGTPopupMenu__
#define __MGTTemplate__MGTPopupMenu__

#include "cocos2d.h"

USING_NS_CC;

class MGTPopupMenu : public Menu
{
public:
    
    /** creates a MGTPopupMenu with MenuItem objects */
    static MGTPopupMenu* create(MenuItem* item, ...);
    
    /** creates a MGTPopupMenu with a __Array of MenuItem objects */
    static MGTPopupMenu* createWithArray(const Vector<MenuItem*>& arrayOfItems);
    
    /** creates a MGTPopupMenu with it's item, then use addChild() to add
     * other items. It is used for script, it can't init with undetermined
     * number of variables.
     */
    static MGTPopupMenu* createWithItem(MenuItem* item);
    
    /** creates a Menu with MenuItem objects */
    static MGTPopupMenu* createWithItems(MenuItem *firstItem, va_list args);
    
    // Fix 
//	virtual void registerWithTouchDispatcher();
	
protected:
	
};

#endif /* defined(__MGTTemplate__MGTPopupMenu__) */
