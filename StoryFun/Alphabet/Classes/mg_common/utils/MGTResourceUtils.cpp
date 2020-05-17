#include "MGTResourceUtils.h"
#include "DRMManager.h"

static MGTResourceUtils *m_spManagement;

MGTResourceUtils::MGTResourceUtils():
m_rootPath(""),
m_commonResourcePath("")
{
}

MGTResourceUtils::~MGTResourceUtils()
{
    releaseInstance();
}

MGTResourceUtils* MGTResourceUtils::getInstance()
{
    if(!m_spManagement)
    {
        m_spManagement  = new MGTResourceUtils();
        
    }
    return m_spManagement;
}

  
void MGTResourceUtils::releaseInstance()
{
    if (m_spManagement)
    {
        delete m_spManagement;
        m_spManagement = NULL;
    }
}
 
#pragma mark - contents controll.

void MGTResourceUtils::setRootPath(std::string tempPath)
{
    m_rootPath = tempPath;
}

std::string MGTResourceUtils::getRootPath()
{
    return m_rootPath;
}

void MGTResourceUtils::setResourcePath(ResourceType type, std::string tempPath)
{
    if (type == ResourceType::IMAGE){
        m_resImagePath = tempPath;
    }
    else if (type == ResourceType::SOUND){
        m_resSoundPath = tempPath;
    }
    else if (type == ResourceType::GAF){
        m_resGafPath = tempPath;
        
        log("MGTResourceUtils SET RESOURCE PATH GAF");
    }
    else if (type == ResourceType::JSON){
        m_resJsonPath = tempPath;
    }
    else if (type == ResourceType::XML){
        m_resXmlPath = tempPath;
    }
    
    
}

std::string MGTResourceUtils::getResourcePath(ResourceType type)
{
    std::string resourcePath;
    
    if (type == ResourceType::IMAGE){
        resourcePath = m_resImagePath;
    }
    else if (type == ResourceType::SOUND){
        resourcePath = m_resSoundPath;
    }
    else if (type == ResourceType::GAF){
        resourcePath = m_resGafPath;
    }
    else if (type == ResourceType::JSON){
        resourcePath = m_resJsonPath;
    }
    else if (type == ResourceType::XML){
        resourcePath = m_resXmlPath;
    }
    
    log("MGTResourceUtils getResourcePath:%s", resourcePath.c_str());
    
    return resourcePath;
}

void MGTResourceUtils::setCommonResourcePath(std::string commonPath)
{
    m_commonResourcePath = commonPath.append("/");
}

std::string MGTResourceUtils::getCommonResourcePath()
{
    std::string resourcePath = m_commonResourcePath;
    return resourcePath;
}


std::string MGTResourceUtils::getFilePath(ResourceType type, std::string strFolderName, std::string strFileName)
{
    std::string result;
    
    if (getIsResEmbeded() == false)
    {
        result = getResourcePath(type);
        result.append(strFolderName);
        result.append("/");
        result.append(strFileName);
    }
    else
    {
        result = getResourcePath(type);

        result.append(strFolderName);
        result.append("/");
        result.append(strFileName);
    }
    
    
    return result;

}

std::string MGTResourceUtils::getFilePath(ResourceType type, std::string strFileName)
{
    std::string result = getResourcePath(type);
    result.append(strFileName);
    return result;
}

std::string MGTResourceUtils::getCommonFilePath(std::string strFileName)
{

    std::string result = getCommonResourcePath();
    result.append(strFileName);
    return result;

}

std::string MGTResourceUtils::getCommonFilePath(std::string strFolderName, std::string strFileName)
{
    std::string result = getCommonResourcePath();
    result.append(strFolderName);
    result.append("/");
    result.append(strFileName);
    return result;
    
}

Image* MGTResourceUtils::getImageData(std::string strFilePath, bool isCommonRes)
{
    Image* image;
    
    if (getIsResEmbeded() == false)
    {
//        if (isCommonRes == false )
//        {
//            image = DRMManager::getInstance()->getImageDataFromEntity(DRMManager::getInstance()->getEntryName(strFilePath));
//        }
//        else
//        {
            image = new Image();
            image->initWithImageFile( strFilePath );
//        }
    }
    else
    {
        image = new Image();
        image->initWithImageFile( strFilePath );
    }
    
    return image;
}

