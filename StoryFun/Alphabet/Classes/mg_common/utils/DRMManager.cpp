#include "DRMManager.h"

DRMManager::DRMManager()
{
    init();
}

DRMManager::~DRMManager()
{
    
}

bool DRMManager::init()
{
    return true;
}

int DRMManager::getBytesFromEntity(std::string sEntity, unsigned char ** ppData)
{
    int ret = 0;

    ret = DRMInterface::getInstance()->getBytesFromEntity(sEntity, ppData);
    return ret;
}


std::string DRMManager::getTempDirPath()
{
    std::string tempPath;
    
    tempPath = DRMInterface::getInstance()->getTempDirPath();

    return tempPath;
}

std::string DRMManager::getResourcePath()
{
    std::string resPath;
    
    resPath = DRMInterface::getInstance()->getResourcePath();
    
    return resPath;
}

Image* DRMManager::getImageDataFromEntity(std::string sEntity)
{
    unsigned char * pData = NULL;
    
    cocos2d::log("getImageDataFromEntity = %s", sEntity.c_str() );
    int nSize = DRMManager::getInstance()->getBytesFromEntity(sEntity, &pData);
    if( nSize == 0) {
        cocos2d::log("preloadImage size = 0" );
        return nullptr;
    }
    
    
    Image * image = new (std::nothrow) Image();
    image->initWithImageData((unsigned char *)pData, nSize);
    
    return image;
}

std::string DRMManager::getEntryName(std::string fullPath)
{
    std::string entryName;
    std::string string = fullPath;
    std::string rootpath = getTempDirPath();
    
    
    
    entryName = eraseString(fullPath, rootpath);
    
    log("ENTRY NAME :%s", entryName.c_str());
    
    return entryName;
}

std::string DRMManager::eraseString(std::string str, std::string eraseStr)
{
    if ((str.find(eraseStr) >= 0) & (str.find(eraseStr) < str.length()))
    {
        size_t findNum = str.find(eraseStr);
        str.erase(findNum, eraseStr.length());
    }
    
    return str;
}

void DRMManager::showProgress()
{
    DRMInterface::getInstance()->showProgress();
}

void DRMManager::hideProgress()
{
    DRMInterface::getInstance()->hideProgress();
}
