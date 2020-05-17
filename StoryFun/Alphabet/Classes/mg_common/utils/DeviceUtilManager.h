
#ifndef __DeviceUtilManager_H__
#define __DeviceUtilManager_H__

#pragma once

#include "SingletonBase.h"
#include "cocos2d.h"

#include "MGTDeviceInterface.h"

USING_NS_CC;

    
class DeviceUtilManager: public CSingletonBase<DeviceUtilManager>
{

public:
    DeviceUtilManager();
    ~DeviceUtilManager();
    bool init();

public:
    
    CC_SYNTHESIZE(bool, _isDrm, IsDrm);
    
    int getDeviceVolume();
    
    void setSleepMode(bool isSleep);
    
    void showToast(std::string text);
    
    void exit();
    
    void showSavePopup();
    void saveDone();
    void hideSavePopup();
    
    int getScreenOrientation();
};


#endif
