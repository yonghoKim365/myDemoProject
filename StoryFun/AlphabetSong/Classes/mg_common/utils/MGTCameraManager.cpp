
#include "MGTCameraManager.h"


#ifndef AUTO_LOCK
#define AUTO_LOCK(mtx)	std::lock_guard<std::mutex> lock(mtx)
#endif

using namespace cocos2d;
using namespace experimental;

MGTCameraManager::MGTCameraManager()
{
    init();
}

MGTCameraManager::~MGTCameraManager()
{

}


bool MGTCameraManager::init()
{
    
    return true;
}

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
void MGTCameraManager::setGallerySession()
{
    MGTCameraInterface::getInstance()->setGallerySession();
}

bool MGTCameraManager::getGallerySession()
{
    return MGTCameraInterface::getInstance()->getGallerySession();
}

void MGTCameraManager::saveToPhotoLibraryWithFileName(std::string strFileName)
{
    MGTCameraInterface::getInstance()->saveToPhotoLibraryWithFileName(strFileName);
}
#endif

void MGTCameraManager::showCamera(std::string frameImgPath, int direction, int x, int y, int width, int height)
{
    MGTCameraInterface::getInstance()->showCamera(frameImgPath,
                                                        direction,
                                                        x,
                                                        y,
                                                        width,
                                                        height);
}

void MGTCameraManager::startCameraPreview()
{
    MGTCameraInterface::getInstance()->startCameraPreview();
}

void MGTCameraManager::takePicture(std::string fileName)
{
    MGTCameraInterface::getInstance()->takePicture(fileName);
}

void MGTCameraManager::startVideoRecording(std::string contentPath)
{
    //Temp File Name
    //Get filename through LMS function of CJUtil class
    MGTCameraInterface::getInstance()->startRecording(contentPath);
}

void MGTCameraManager::stopVideoRecording()
{
    MGTCameraInterface::getInstance()->stopRecording();
}

void MGTCameraManager::removeCamera()
{
    MGTCameraInterface::getInstance()->removeCamera();
}

bool MGTCameraManager::isShowCameraPreivew()
{
    return MGTCameraInterface::getInstance()->isShowCameraPreview();
}

void MGTCameraManager::moveToCameraView(int x, int y)
{
    MGTCameraInterface::getInstance()->moveToCameraView(x, y);
}

