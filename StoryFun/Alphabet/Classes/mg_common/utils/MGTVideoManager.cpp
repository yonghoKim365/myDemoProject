
#include "MGTVideoManager.h"


#ifndef AUTO_LOCK
#define AUTO_LOCK(mtx)	std::lock_guard<std::mutex> lock(mtx)
#endif

using namespace cocos2d;
using namespace experimental;

MGTVideoManager::MGTVideoManager()
{
    init();
}

MGTVideoManager::~MGTVideoManager()
{

}


bool MGTVideoManager::init()
{
    
    return true;
}

void MGTVideoManager::playVideo(std::string fileName, bool isControllerVisible)
{
    MGTVideoInterface::getInstance()->playVideo(fileName, isControllerVisible);
}

void MGTVideoManager::pauseVideo()
{
    MGTVideoInterface::getInstance()->pauseVideo();
}

void MGTVideoManager::resumeVideo()
{
    MGTVideoInterface::getInstance()->resumeVideo();
}

void MGTVideoManager::stopVideo()
{
    MGTVideoInterface::getInstance()->stopVideo();
}

void MGTVideoManager::addButtonVideo(std::string btnPath, float x, float y, int tag)
{
    MGTVideoInterface::getInstance()->addVideoBtn(btnPath,
                                                 x,
                                                 y,
                                                 tag);
}

void MGTVideoManager::playFrameVideo(std::string frameImgPath, std::string contentPath, int x, int y, int width, int height, int position)
{
    MGTVideoInterface::getInstance()->playFrameVideo(frameImgPath,
                                                    contentPath,
                                                    x,
                                                    y,
                                                    width,
                                                    height,
                                                    position);
}

void MGTVideoManager::removeFrameVideo()
{
    MGTVideoInterface::getInstance()->removeFrameVideo();
}
