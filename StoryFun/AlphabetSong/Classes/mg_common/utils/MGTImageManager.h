

#ifndef __MGTImageManager_h__
#define __MGTImageManager_h__


#include "SingletonBase.h"
#include "cocos2d.h"
#include "MGTDefines.h"
#include "MGTUtils.h"
#include "MGTImageInterface.h"

USING_NS_CC;


class MGTImageManager : public CSingletonBase<MGTImageManager>
{
public:
    MGTImageManager();
    ~MGTImageManager();
	bool init();

    void addFrameImage(std::string frameImgPath, std::string strTag, int x, int y, int width, int height);
    void removeFrameImage(std::string strTag);
};


#endif
