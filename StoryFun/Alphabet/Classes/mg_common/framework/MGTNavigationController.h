//
//  MGTNavigationController.h
//
//
//  Created by Dongjin on 13. 7. 8..
//  Copyright (c) 2013년 MGT. All rights reserved.
//

#ifndef __MGTNavigationController__
#define __MGTNavigationController__

#include "cocos2d.h"
#include "MGTMenu.h"


namespace MGTNavigation
{
    // Navigation button structure.
    struct MGTNavigationButton
    {
        
        cocos2d::Sprite                       *imgNormal;
        cocos2d::Sprite                       *imgDisable;
        cocos2d::Sprite                       *imgSelect;

        
        cocos2d::MenuItemSprite               *menuItemSprite;
        cocos2d::MenuItemImage                *menuItemImage;
        
        Menu                         *menu;
    };
}



/**
 * @author          Dongjin
 * @brief           MGTNavigation super class.
 * @version         1.0
 * @date            2013.07.08
 * @since           2013.07.08
 */
class MGTNavigationController : public Ref
{
////////////////////////////////////////////////////////
//
// Class variables.
//
////////////////////////////////////////////////////////
protected:
    /**
     * @brief       Window size variable.
     */
    cocos2d::Size                             naviWinSize;
    
    /**
     * @brief       Window half size variable.
     */
    cocos2d::Size                             winHalfSize;
    
    
    /**
     * @brief       Navigation button add target node.
     */
    cocos2d::Node*                              parentNode;
    
    /**
     * @brief       Navigation button array. (Used MGTNavigationButton structure)
     */
    MGTNavigation::MGTNavigationButton*           btnNavigations[20];
    
    /**
     * @brief       Use navigation button type indexs.
     */
    std::vector<int>                            btnNavigationIdx;
    
    
    
    
    
    
    
////////////////////////////////////////////////////////
//
// Class Creator & Destoryer.
//
////////////////////////////////////////////////////////
public:
    MGTNavigationController();
    virtual ~MGTNavigationController();

    
    


    
////////////////////////////////////////////////////////
//
// Class oeprate functions.
//
////////////////////////////////////////////////////////
protected:
    /**
     * @author      Dongjin
     * @brief       Initialize function.
     * @version     1.0
     * @param       Node *pTargetLayer    -   Parent layer node.
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void init(cocos2d::Node *pTargetLayer);
    
    

    
    
    
    
////////////////////////////////////////////////////////
//
// Class utility function.
//
////////////////////////////////////////////////////////
    /**
     * @author      Dongjin
     * @brief       Make CallFunc callback function.
     * @version     1.0
     * @param       SEL_CallFunc aSelector  -   Callback target function.
     *              float fDelayTime        -   Call callback function after the fDelayTime.
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
//    void callFunctionWithDelay(cocos2d::SEL_CallFunc aSelector, float fDelayTime);

    
    
    
    
    
    
////////////////////////////////////////////////////////
//
// Class oeprate functions.
//
////////////////////////////////////////////////////////
private:
    void _showNavigation();
    void _hideNavigation();

    void _setEnableNavigation();
    void _setDisableNavigation();

    void _setVisibleNavigation();
    void _setInvisibleNavigation();

public:
    /**
     * @author      Dongjin
     * @brief       Show navigation buttons.
     * @version     1.0
     * @param       float fDelayTime    - Showing navigation delay time. (default 0.0f)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void showNavigation(float fDelayTime = 0.0f);
    
    /**
     * @author      Dongjin
     * @brief       Hide navigation buttons.
     * @version     1.0
     * @param       float fDelayTime    - Hiding navigation delay time. (default 0.0f)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void hideNavigation(float fDelayTime = 0.0f);

    /**
     * @author      Dongjin
     * @brief       Set added all navigation button touch enable.
     * @version     1.0
     * @param       bool bIsEnable      - true : enable / false : disable (default enable)
     *              float fDelayTime    - Enablity navigation delay time. (default 0.0f)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void setEnableNavigation(bool bIsEnable, float fDelayTime = 0.0f);
    
    /**
     * @author      Dongjin
     * @brief       Set added all navigation button visible.
     * @version     1.0
     * @param       bool bIsVisible     - true : visible / false : invisible (default invisible)
     *              float fDelayTime    - Visiblity navigation delay time. (default 0.0f)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void setVisibleNavigation(bool bIsVisible, float fDelayTime = 0.0f);

    /**
     * @author      Dongjin
     * @brief       Set navigation button touch enable.
     * @version     1.0
     * @param       bool bIsEnable              - true : enable / false : disable (default enable)
     *              int eNavigationButtonType   - navigation button type. (eNavigationButtonType)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void setEnableButton(bool bIsEnable, int eNavigationButtonType);
    virtual void setEnableButtons(bool bIsEnable, int eNavigationButtonType, ...);

    
    /**
     * @author      Dongjin
     * @brief       set navigation button visible.
     * @version     1.0
     * @param       bool bIsVisible             - true : visible / false : invisible (default invisible)
     *              int eNavigationButtonType   - navigation button type. (eNavigationButtonType)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void setVisibleButton(bool bIsVisible, int eNavigationButtonType);
    virtual void setVisibleButtons(bool bIsVisible, int eNavigationButtonType, ...);

    Menu* getNavigationButton(int eNavigationButtonType);
    
    virtual void resetPosition(int eNavigationButtonType, Vec2 pos);
    
    
    
////////////////////////////////////////////////////////
//
// Navigation button callback vitual function.
//
////////////////////////////////////////////////////////
    /**
     * @author      Dongjin
     * @brief       Callback function when Navigation button clicked.
     * @version     1.0
     * @param       Node *pNode       - MenuitemImage(CCMemuItemSprite) object. (Use object tag value.)
     * @return      void
     * @exception
     * @date        2013.07.08
     * @since       2013.07.08
     */
    virtual void onTouchedNavigationButton(cocos2d::Node *pNode) {}
};


#endif /* defined(__MGTNavigationController__) */
