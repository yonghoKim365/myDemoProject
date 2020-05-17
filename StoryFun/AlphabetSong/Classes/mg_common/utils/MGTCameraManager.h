

#ifndef __MGTCameraManager_h__
#define __MGTCameraManager_h__


#include "SingletonBase.h"
#include "cocos2d.h"
#include "MGTDefines.h"
#include "MGTUtils.h"
#include "MGTCameraInterface.h"

USING_NS_CC;


class MGTCameraManager : public CSingletonBase<MGTCameraManager>
{
public:
    MGTCameraManager();
    ~MGTCameraManager();
	bool init();

    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    void setGallerySession();
    bool getGallerySession();
    void saveToPhotoLibraryWithFileName(std::string strFileName);
#endif
    
    void showCamera(std::string frameImgPath, int direction, int x, int y, int width, int height);
    void startCameraPreview();
    void takePicture(std::string fileName);
    void startVideoRecording(std::string contentPath);
    void stopVideoRecording();
    void removeCamera();
    bool isShowCameraPreivew();
    void moveToCameraView(int x, int y);
    

};


#endif
