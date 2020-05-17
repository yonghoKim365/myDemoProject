
#ifndef __DRMManager_H__
#define __DRMManager_H__

#include "SingletonBase.h"
#include "cocos2d.h"

#include "DRMInterface.h"

USING_NS_CC;

    
class DRMManager: public CSingletonBase<DRMManager>
{

public:
    DRMManager();
    ~DRMManager();
    bool init();

public:    
    /* entryName으로 bytebuffer와 size를 리턴 */
    int getBytesFromEntity(std::string sEntity, unsigned char ** ppData);
    
    /* temp 폴더 경로를 리턴 */
    std::string getTempDirPath();
    
    /* resource 폴더 경로를 리턴 */
    std::string getResourcePath();
    
    /* entryName으로 ImageData를 리턴한다. */
    Image* getImageDataFromEntity(std::string sEntity);
    
    /* 경로에서 entryName만 리턴 */
    std::string getEntryName(std::string fullPath);
    std::string eraseString(std::string str, std::string eraseStr);
    
    void showProgress();
    void hideProgress();
};


#endif
