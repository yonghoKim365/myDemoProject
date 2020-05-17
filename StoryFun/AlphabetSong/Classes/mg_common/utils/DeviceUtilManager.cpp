#include "DeviceUtilManager.h"

static DeviceUtilManager *m_spManagement;

DeviceUtilManager::DeviceUtilManager()
{
    init();
}

DeviceUtilManager::~DeviceUtilManager()
{
    
}

bool DeviceUtilManager::init()
{
    return true;
}

int DeviceUtilManager::getDeviceVolume()    // 0~100
{
    return MGTDeviceInterface::getInstance()->getSystemVolumn();
}

void DeviceUtilManager::setSleepMode(bool isSleep)
{
    MGTDeviceInterface::getInstance()->setSleepMode(isSleep);
}

void DeviceUtilManager::showToast(std::string text)
{
    MGTDeviceInterface::getInstance()->showToast(text);
}

void DeviceUtilManager::exit()
{
    MGTDeviceInterface::getInstance()->exit();
}


void DeviceUtilManager::showSavePopup()
{
    MGTDeviceInterface::getInstance()->showSavePopup();
}

void DeviceUtilManager::saveDone()
{
    MGTDeviceInterface::getInstance()->saveDone();
}

void DeviceUtilManager::hideSavePopup()
{
    MGTDeviceInterface::getInstance()->hideSavePopup();
}

int DeviceUtilManager::getScreenOrientation()
{
    return MGTDeviceInterface::getInstance()->getScreenOrientation();
}
