
#include "MGTImageManager.h"


#ifndef AUTO_LOCK
#define AUTO_LOCK(mtx)	std::lock_guard<std::mutex> lock(mtx)
#endif

using namespace cocos2d;
using namespace experimental;

MGTImageManager::MGTImageManager()
{
    init();
}

MGTImageManager::~MGTImageManager()
{

}


bool MGTImageManager::init()
{
    
    return true;
}

void MGTImageManager::addFrameImage(std::string frameImgPath, std::string strTag, int x, int y, int width, int height)
{
    MGTImageInterface::getInstance()->addFrameImage(frameImgPath,
                                                    strTag,
                                                    x,
                                                    y,
                                                    width,
                                                    height);
}

void MGTImageManager::removeFrameImage(std::string strTag)
{
    MGTImageInterface::getInstance()->removeFrameImage(strTag);
}
