

#ifndef __MGTVideoManager_h__
#define __MGTVideoManager_h__


#include "SingletonBase.h"
#include "cocos2d.h"
#include "MGTDefines.h"
#include "MGTUtils.h"
#include "MGTVideoInterface.h"

USING_NS_CC;


class MGTVideoManager : public CSingletonBase<MGTVideoManager>
{
public:
    MGTVideoManager();
    ~MGTVideoManager();
	bool init();

    void playVideo(std::string fileName, bool isControllerVisible);
    void pauseVideo();
    void resumeVideo();
    void stopVideo();
    void addButtonVideo(std::string btnPath, float x, float y, int tag);
    void playFrameVideo(std::string frameImgPath, std::string contentPath, int x, int y, int width, int height, int position = 0);
    void removeFrameVideo();
};


#endif
