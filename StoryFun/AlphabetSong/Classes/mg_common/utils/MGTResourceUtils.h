#ifndef RESOURCEUTILS_H
#define RESOURCEUTILS_H

#include "cocos2d.h"

namespace resourceutils {
    typedef enum
    {
        IMAGE= 0,
        SOUND,
        GAF,
        JSON,
        XML,
    } ResourceType;
}


USING_NS_CC;
using namespace resourceutils;

class MGTResourceUtils
{
    
private:
    
    std::string m_rootPath;                 // '/mnt/sdcard/' or 'asset/' or '/data/data/'
    
    std::string m_resImagePath;             // 'm_rootPath' + 'm_contentID'
    std::string m_resSoundPath;             // 'm_rootPath' + 'm_contentID'
    std::string m_resGafPath;             // 'm_rootPath' + 'm_contentID'
    std::string m_resJsonPath;             // 'm_rootPath' + 'm_contentID'
    std::string m_resXmlPath;             // 'm_rootPath' + 'm_contentID'
    std::string m_commonResourcePath;       // for common resource (asset)
    
public:
    MGTResourceUtils();
    ~MGTResourceUtils();
    
    
    static MGTResourceUtils* getInstance();
    static void     releaseInstance();
    
    CC_SYNTHESIZE(bool, _isResEmbeded, IsResEmbeded);
    
    // path control
    void            setRootPath(std::string tempPath);
    std::string     getRootPath();
    
    void            setResourcePath(ResourceType type, std::string tempPath);
    std::string     getResourcePath(ResourceType type);
    
    void            setCommonResourcePath(std::string commonPath);
    std::string     getCommonResourcePath();
    
    // get file path
    std::string     getFilePath(ResourceType type, std::string strFileName);
    std::string     getFilePath(ResourceType type, std::string strFolderName, std::string strFileName);
    std::string     getCommonFilePath(std::string strFileName);
    std::string     getCommonFilePath(std::string strFolderName, std::string strFileName);
    
    Image*          getImageData(std::string strFilePath, bool isCommonRes = false);
    
};





#endif
