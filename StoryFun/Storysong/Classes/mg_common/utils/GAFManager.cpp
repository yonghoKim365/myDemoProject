#include "GAFManager.h"

GAFManager::GAFManager()
{
    m_gafAssetMap.clear();
    
    init();
}

GAFManager::~GAFManager()
{
    removeAllGafAssets();
}

bool GAFManager::init()
{
    return true;
}

void GAFManager::addGafAsset(std::string filePath)
{
    log("GAFManager :%s", filePath.c_str());
    
    GAFAsset* asset = GAFAsset::create(filePath, nullptr);
    asset->retain();
    
//    GAFObject* object;
//    if (asset)
//    {
//        object = asset->createObject();
//    }
    
    
//    std::string fileName = getFileName(filePath);
//    log("GAFManager NAME:%s", fileName.c_str());
    
    m_gafAssetMap.insert(std::pair<std::string, GAFAsset*> (filePath, asset));
}

void GAFManager::removeGafAsset(std::string filePath)
{
    auto it = m_gafAssetMap.find(filePath);
    if (it != m_gafAssetMap.end())
    {
        m_gafAssetMap.erase(it);
    }
}

void GAFManager::removeAllGafAssets()
{
    m_gafAssetMap.clear();
}


GAFAsset* GAFManager::getGafAsset(std::string filePath, bool isCommonRes)
{
    GAFAsset* asset;
    
    if (getIsResEmbeded() == false)
    {
        if(isCommonRes == false)
        {
//            std::map<std::string, GAFAsset*>::iterator it = m_gafAssetMap.find(filePath);
//            if (it != m_gafAssetMap.end())
//            {
//                asset = (GAFAsset*)it->second;
//            }
            
            asset = GAFAsset::create(filePath, nullptr);
            
            log("GAFManager getAsset:%s", filePath.c_str());
        }
        else
        {
            asset = GAFAsset::create(filePath, nullptr);
            
            log("GAFManager COMMON getAsset:%s", filePath.c_str());
        }
    }
    else
    {
        asset = GAFAsset::create(filePath, nullptr);
    }
    
    
    return asset;
}

GAFAsset* GAFManager::getCommonGafAsset(std::string filePath)
{
    return getGafAsset(filePath, true);
}

std::string GAFManager::getFileName(std::string filePath)
{
    std::string tempStr = filePath;
    
    int start, stop, n = (int)tempStr.length();
    
    for(start = (int)tempStr.find_first_not_of("/"); 0 <= start && start < n; start = (int)tempStr.find_first_not_of("/", stop + 1))
    {
        stop = (int)tempStr.find_first_of("/", start);
        
        if (stop < 0 || stop > n)
            stop = n;
        
        if (stop == n)
        {
            return tempStr.substr(start, stop - start);
        }
    }
    
    return "error";
}

