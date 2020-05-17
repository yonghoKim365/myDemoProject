//
//  MGTNavigationController.cpp
//  
//
// on 13. 7. 8..
//  Copyright (c) 2013년 MGT. All rights reserved.
//

#include "MGTNavigationController.h"

using namespace cocos2d;


MGTNavigationController::MGTNavigationController()
{
    parentNode              = NULL;
    naviWinSize             = cocos2d::Director::getInstance()->getWinSize();
    winHalfSize             = cocos2d::Size(naviWinSize.width/2, naviWinSize.height/2);

    
    btnNavigationIdx.clear();
}

MGTNavigationController::~MGTNavigationController()
{
    for(int buttonIdx=0; buttonIdx<btnNavigationIdx.size(); buttonIdx++)
    {
        delete btnNavigations[btnNavigationIdx[buttonIdx]];
    }
    
    
    parentNode              = NULL;
    naviWinSize             = cocos2d::Size::ZERO;
    winHalfSize             = cocos2d::Size::ZERO;
    

    btnNavigationIdx.clear();
}



void MGTNavigationController::init(cocos2d::Node *pTargetLayer)
{
    parentNode = pTargetLayer;
    
    int totalNum = (int)btnNavigationIdx.size();
    for(int i=0; i < totalNum; i++)
    {
        btnNavigations[btnNavigationIdx[i]] = new MGTNavigation::MGTNavigationButton();
    }
    
//    for(int buttonIdx=0; buttonIdx<btnNavigationIdx.size(); buttonIdx++)
//    {
//        log("INIT IDX:%d", buttonIdx);
//        btnNavigations[btnNavigationIdx[buttonIdx]]     = new MGTNavigation::MGTNavigationButton();
//    }
    
}


//void MGTNavigationController::callFunctionWithDelay(cocos2d::SEL_CallFunc aSelector, float fDelayTime)
//{
//    cocos2d::ActionManager *manager = cocos2d::Director::getInstance()->getActionManager();
//    cocos2d::CallFunc *callFunc = cocos2d::CallFunc::create((cocos2d::Ref *)this, aSelector);
//    cocos2d::DelayTime *delay = cocos2d::DelayTime::create(fDelayTime);
//    cocos2d::Sequence *action = cocos2d::Sequence::create(delay, callFunc, nullptr);
//    
//    manager->addAction(Sequence::create(action,NULL), (cocos2d::Node *)this, false);
//}


void MGTNavigationController::_showNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setVisibleButton(true, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::_hideNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setVisibleButton(false, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::_setEnableNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setEnableButton(true, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::_setDisableNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setEnableButton(false, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::_setVisibleNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setVisibleButton(true, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::_setInvisibleNavigation()
{
    for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        this->setVisibleButton(false, btnNavigationIdx[buttonIdx]);
    }
}

void MGTNavigationController::showNavigation(float fDelayTime)
{
    if (fDelayTime == 0)
    {
        _showNavigation();
    }
    else
    {
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_showNavigation), fDelayTime);
    }
}

void MGTNavigationController::hideNavigation(float fDelayTime)
{
    if (fDelayTime == 0) {
        _hideNavigation();
    }else{
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_hideNavigation), fDelayTime);
    }
}

void MGTNavigationController::setEnableNavigation(bool bIsEnable, float fDelayTime)
{
    if(bIsEnable == true)
    {
        _setEnableNavigation();
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_setEnableNavigation), fDelayTime);
    }
    else
    {
        _setDisableNavigation();
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_setDisableNavigation), fDelayTime);
    }
}

void MGTNavigationController::setVisibleNavigation(bool bIsVisible, float fDelayTime)
{
    if(true == bIsVisible)
    {
        _setVisibleNavigation();
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_setVisibleNavigation), fDelayTime);
    }
    else
    {
        _setInvisibleNavigation();
//        this->callFunctionWithDelay(CC_CALLFUNC_SELECTOR(MGTNavigationController::_setInvisibleNavigation), fDelayTime);
    }
}

void MGTNavigationController::setEnableButton(bool bIsEnable, int eNavigationButtonType)
{
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        btnNavigations[eNavigationButtonType]->menuItemImage->setEnabled(bIsEnable);
    }
    else if(nullptr != btnNavigations[eNavigationButtonType]->menuItemSprite->getNormalImage())
    {
        btnNavigations[eNavigationButtonType]->menuItemSprite->setEnabled(bIsEnable);
    }
}

void MGTNavigationController::setEnableButtons(bool bIsEnable, int eNavigationButtonType, ...)
{
    va_list args;
    va_start(args, eNavigationButtonType);
    while(eNavigationButtonType != -1)
    {
        this->setEnableButton(bIsEnable, eNavigationButtonType);
        
        eNavigationButtonType = va_arg(args, int);
        if(eNavigationButtonType == -1)
        {
            break ;
        }
    }
    va_end(args);
}

void MGTNavigationController::setVisibleButton(bool bIsVisible, int eNavigationButtonType)
{
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        btnNavigations[eNavigationButtonType]->menuItemImage->setVisible(bIsVisible);
    }
    else if(NULL != btnNavigations[eNavigationButtonType]->menuItemSprite)
    {
        btnNavigations[eNavigationButtonType]->menuItemSprite->setVisible(bIsVisible);
    }
}

void MGTNavigationController::setVisibleButtons(bool bIsVisible, int eNavigationButtonType, ...)
{
    va_list args;
    va_start(args, eNavigationButtonType);
    while(eNavigationButtonType != -1)
    {
        this->setVisibleButton(bIsVisible, eNavigationButtonType);
        
        eNavigationButtonType = va_arg(args, int);
        if(eNavigationButtonType == -1)
        {
            break ;
        }
    }
    va_end(args);
}

Menu* MGTNavigationController::getNavigationButton(int eNavigationButtonType)
{
    Menu* menu = nullptr;
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        menu = btnNavigations[eNavigationButtonType]->menu;
    }
    else if(NULL != btnNavigations[eNavigationButtonType]->menuItemSprite)
    {
        menu = btnNavigations[eNavigationButtonType]->menu;
    }
    
    return menu;
}

void MGTNavigationController::resetPosition(int eNavigationButtonType, Vec2 pos)
{
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        btnNavigations[eNavigationButtonType]->menu->setPosition(pos);
    }
    else if(NULL != btnNavigations[eNavigationButtonType]->menuItemSprite)
    {
        btnNavigations[eNavigationButtonType]->menu->setPosition(pos);
    }
}
