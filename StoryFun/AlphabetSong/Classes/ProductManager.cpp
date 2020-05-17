/*
 *  ProductManager.cpp
 *
 *
 *   on 12. 10. 4..
 */

#include "ProductManager.h"

#include "MSLPManager.h"
#include "DRMManager.h"
#include "GAFManager.h"
#include "DeviceUtilManager.h"

#include "Debug_Index.h"
#include "ProductIndex.h"

static ProductManager *m_spManagement;

ProductManager::ProductManager()
{
    m_prefix = "";
}

ProductManager::~ProductManager()
{
    releaseInstance();
}

ProductManager* ProductManager::getInstance()
{
    if(!m_spManagement)
    {
        m_spManagement  = new ProductManager();
        
    }
    return m_spManagement;
}


void ProductManager::releaseInstance()
{
    if (m_spManagement)
    {
        delete m_spManagement;
        m_spManagement = NULL;
    }
}

void ProductManager::initSet()
{
    //get android intent data
    
    int bookNum = 1;
    int dayNum = MSLPManager::getInstance()->getDayNum();
    int alphabetNum = MSLPManager::getInstance()->getAlphabetNum();
    int stepNum = MSLPManager::getInstance()->getProgress();
    bool isFinished = MSLPManager::getInstance()->isFinished();
    
    if (getIsResEmbeded())
    {
        alphabetNum = 1;
        stepNum = 1;
    }
    
    if (stepNum <= 0)
    {
        stepNum = 1;
    }

    log("----->DAY:%d, ALPHABET:%d, LASTSTEP:%d ", dayNum, alphabetNum, stepNum);
    
    
    // for test
    ProductManager::getInstance()->setRootPath(DRMManager::getInstance()->getTempDirPath());
    ProductManager::getInstance()->setProduct(bookNum, dayNum, alphabetNum, stepNum, isFinished);
    ProductManager::getInstance()->setCommonResourcePath("common");
    
    
    // asset
    //    ProductManager::getInstance()->setRootPath(StringUtils::format("%s/day%d/", ProductManager::getInstance()->getProductID().c_str(), ProductManager::getInstance()->getCurrentDay()));
    
    
    // for release
//    ProductManager::getInstance()->setProduct(dayNum, alphabetNum, stepNum, statusNum);
    
//    ProductManager::getInstance()->setRootPath(MSLPManager::getInstance()->getRootPath());
    
    // mnt/sdcard
    //        ProductManager::getInstance()->setRootPath(StringUtils::format("/mnt/sdcard/ResourcesTest/Korean/%s/day%d/", ProductManager::getInstance()->getProductID().c_str(), ProductManager::getInstance()->getCurrentDay()));
    
    
    
}

#pragma mark - contents controll.

void ProductManager::setProduct(int bookNum, int dayNum, int alphabetNum, int stepNum, bool bFinished)
{
    m_productInfo.bookNum = bookNum;
    m_productInfo.dayNum = dayNum;
    m_productInfo.alphabetNum = alphabetNum;
    m_productInfo.lastStepNum = stepNum;
    m_productInfo.isFinished = bFinished;
    
    
    
    m_prefix = StringUtils::format("%02d", alphabetNum);
    
    std::string imgPath;
    std::string sndPath;
    std::string gafPath;
    std::string jsonPath;
    std::string xmlPath;
    
    if (getIsResEmbeded() == false)
    {
        std::string rootPath = getRootPath();
        imgPath = rootPath;
        sndPath = rootPath;
//        gafPath = rootPath;
//        jsonPath = rootPath;
//        xmlPath = rootPath;
        
        gafPath = DRMManager::getInstance()->getResourcePath();
        jsonPath = DRMManager::getInstance()->getResourcePath();
        xmlPath = DRMManager::getInstance()->getResourcePath();
    }
    else
    {
        imgPath = m_prefix;
        imgPath = imgPath.append("/");
        
        sndPath = imgPath;
        gafPath = imgPath;
        jsonPath = imgPath;
        xmlPath = imgPath;
    }
    
    setResourcePath(ResourceType::IMAGE, imgPath);
    setResourcePath(ResourceType::SOUND, sndPath);
    setResourcePath(ResourceType::GAF, gafPath);
    setResourcePath(ResourceType::JSON, jsonPath);
    setResourcePath(ResourceType::XML, xmlPath);
    
    ProductManager::getInstance()->setCurrentStep(stepNum);
}


void ProductManager::setProduct(int bookNum, int dayNum, int alphabetNum)
{
    m_productInfo.bookNum = bookNum;
    m_productInfo.dayNum = dayNum;
    m_productInfo.alphabetNum = alphabetNum;
    
    m_prefix = StringUtils::format("%02d", alphabetNum);

    
    std::string imgPath;
    std::string sndPath;
    std::string gafPath;
    std::string jsonPath;
    std::string xmlPath;
    
    if (getIsResEmbeded() == false)
    {
        std::string rootPath = getRootPath();
        
        imgPath = rootPath;
        sndPath = rootPath;
        gafPath = rootPath;
        jsonPath = rootPath;
        xmlPath = rootPath;
    }
    else
    {
        imgPath = m_prefix;
        imgPath = imgPath.append("/");
        sndPath = imgPath;
        gafPath = imgPath;
        jsonPath = imgPath;
        xmlPath = imgPath;
    }
    
    setResourcePath(ResourceType::IMAGE, imgPath);
    setResourcePath(ResourceType::SOUND, sndPath);
    setResourcePath(ResourceType::GAF, gafPath);
    setResourcePath(ResourceType::JSON, jsonPath);
    setResourcePath(ResourceType::XML, xmlPath);
}


void ProductManager::setProduct(std::string prefix)
{
    m_prefix = prefix;
    
    std::string imgPath;
    std::string sndPath;
    std::string gafPath;
    std::string jsonPath;
    std::string xmlPath;
    
    if (getIsResEmbeded() == false)
    {
        std::string rootPath = getRootPath();
        
        imgPath = rootPath;
        sndPath = rootPath;
        gafPath = rootPath;
        jsonPath = rootPath;
        xmlPath = rootPath;
    }
    else
    {
        imgPath = m_prefix;
        imgPath = imgPath.append("/");
        sndPath = imgPath;
        gafPath = imgPath;
        jsonPath = imgPath;
        xmlPath = imgPath;
    }
    
    setResourcePath(ResourceType::IMAGE, imgPath);
    setResourcePath(ResourceType::SOUND, sndPath);
    setResourcePath(ResourceType::GAF, gafPath);
    setResourcePath(ResourceType::JSON, jsonPath);
    setResourcePath(ResourceType::XML, xmlPath);
}

int ProductManager::getCurrentBook()
{
    log("getCurrentBook : %d", m_productInfo.bookNum);
    return m_productInfo.bookNum;
}

int ProductManager::getCurrentAlphabet()
{
    log("getCurrentAlphabet : %d", m_productInfo.alphabetNum);
    return m_productInfo.alphabetNum;
}

int ProductManager::getCurrentDay()
{
    log("getCurrentDay : %d", m_productInfo.dayNum);
    return m_productInfo.dayNum;
}

void ProductManager::setTypeNum(int type)
{
    m_productInfo.typeNum = type;
    log("setTypeNum : %d", m_productInfo.typeNum);
}

int ProductManager::getCurrentType()
{
    log("getCurrentType : %d", m_productInfo.typeNum);
    return m_productInfo.typeNum;
}

ProductInfo* ProductManager::getProductInfo()
{
    return &m_productInfo;
}

std::string ProductManager::getProductID()
{
    return m_prefix;
}

std::string ProductManager::getStringAppendPrefix(std::string strTemp)
{
    std::string result = m_prefix;
    result = result.append(strTemp);
    return result;
}


bool ProductManager::getIsDebugMode()
{
#if defined(PRODUCT_DEBUGMODE) && (PRODUCT_DEBUGMODE == 1)
    return true;
#endif
    
    return false;
}



void ProductManager::setCurrentStep(int index)
{
    std::string key = StringUtils::format("b01_c01_%02d", getCurrentAlphabet());
    
    setUserDefaultIntegerForKey(key.c_str(), index);
    
    MSLPManager::getInstance()->progress(index);
}

int ProductManager::getCurrentStep()
{
    std::string key = StringUtils::format("b01_c01_%02d", getCurrentAlphabet());
    
    return  getUserDefaultIntegerForKey(key.c_str());
}

void ProductManager::nextStep()
{
    int bookNum = 1;
    int dayNum = ProductManager::getInstance()->getCurrentDay();
    int alphaNum = ProductManager::getInstance()->getCurrentAlphabet();
    int stepNum = ProductManager::getInstance()->getCurrentStep() + 1;
    
    
    ProductManager::getInstance()->setProduct(bookNum, dayNum, alphaNum , stepNum, 0);
    
    Director::getInstance()->replaceScene(ProductIndex::getContentScene());
}

void ProductManager::replay()
{
    ProductManager::getInstance()->setCurrentStep(1);
    
    int bookNum = 1;
    int dayNum = ProductManager::getInstance()->getCurrentDay();
    int alphaNum = ProductManager::getInstance()->getCurrentAlphabet();
    int stepNum = getCurrentStep();
    
    ProductManager::getInstance()->setProduct(bookNum, dayNum, alphaNum , stepNum, 0);
    
    MSLPManager::getInstance()->beginProgress(stepNum);
    Director::getInstance()->replaceScene(ProductIndex::getContentScene());
}

#pragma mark - contents controll.

void ProductManager::setRootPath(std::string tempPath)
{
    MGTResourceUtils::getInstance()->setRootPath(tempPath);
}

std::string ProductManager::getRootPath()
{
    return MGTResourceUtils::getInstance()->getRootPath();
}

void ProductManager::setResourcePath(ResourceType type, std::string tempPath)
{
    MGTResourceUtils::getInstance()->setResourcePath(type, tempPath);
}

std::string ProductManager::getResourcePath(ResourceType type)
{
    return MGTResourceUtils::getInstance()->getResourcePath(type);
}

void ProductManager::setCommonResourcePath(std::string commonPath)
{
    MGTResourceUtils::getInstance()->setCommonResourcePath(commonPath);
}

std::string ProductManager::getCommonResourcePath()
{
    return MGTResourceUtils::getInstance()->getCommonResourcePath();
}


std::string ProductManager::getFilePath(ResourceType type, std::string strFolderName, std::string strFileName)
{
    return MGTResourceUtils::getInstance()->getFilePath(type, strFolderName, strFileName);
    
}

std::string ProductManager::getFilePath(ResourceType type, std::string strFileName)
{
    return MGTResourceUtils::getInstance()->getFilePath(type, strFileName);
}

std::string ProductManager::getCommonFilePath(std::string strFileName)
{
    return MGTResourceUtils::getInstance()->getCommonFilePath(strFileName);
}

std::string ProductManager::getCommonFilePath(std::string strFolderName, std::string strFileName)
{
    return MGTResourceUtils::getInstance()->getCommonFilePath(strFolderName, strFileName);
    
}

#pragma mark - userdefault

void ProductManager::setUserDefaultBoolForKey(std::string pKey , bool value)
{
    std::string key = getStringAppendPrefix(pKey);
    UserDefault::getInstance()->setBoolForKey(pKey.c_str(), value);
}

void ProductManager::setUserDefaultIntegerForKey(std::string pKey , int value)
{
    std::string key = getStringAppendPrefix(pKey);
    UserDefault::getInstance()->setIntegerForKey(pKey.c_str(), value);
}

void ProductManager::setUserDefaultFloatForKey(std::string pKey , float value)
{
    std::string key = getStringAppendPrefix(pKey);
    UserDefault::getInstance()->setFloatForKey(pKey.c_str(), value);
}

void ProductManager::setUserDefaultStringForKey(std::string pKey , std::string value)
{
    std::string key = getStringAppendPrefix(pKey);
    UserDefault::getInstance()->setStringForKey(pKey.c_str(), value);
}

void ProductManager::setUserDefaultDoubleForKey(std::string pKey , double value)
{
    std::string key = getStringAppendPrefix(pKey);
    UserDefault::getInstance()->setDoubleForKey(pKey.c_str(), value);
}


bool ProductManager::getUserDefaultBoolForKey(std::string pKey)
{
    std::string key = getStringAppendPrefix(pKey);
    return UserDefault::getInstance()->getBoolForKey(pKey.c_str());
}

int ProductManager::getUserDefaultIntegerForKey(std::string pKey)
{
    std::string key = getStringAppendPrefix(pKey);
    return UserDefault::getInstance()->getIntegerForKey(pKey.c_str());
}

float ProductManager::getUserDefaultFloatForKey(std::string pKey)
{
    std::string key = getStringAppendPrefix(pKey);
    return UserDefault::getInstance()->getFloatForKey(pKey.c_str());
}

std::string ProductManager::getUserDefaultStringForKey(std::string pKey)
{
    std::string key = getStringAppendPrefix(pKey);
    return UserDefault::getInstance()->getStringForKey(pKey.c_str());
}

double ProductManager::getUserDefaultDoubleForKey(std::string pKey)
{
    std::string key = getStringAppendPrefix(pKey);
    return UserDefault::getInstance()->getDoubleForKey(pKey.c_str());
}

void ProductManager::next()
{
    log("next");
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    MSLPManager::getInstance()->startNextContent();
#endif
}

void ProductManager::exit()
{
//    replaceSceneNoTransition(Debug_Index);
//    MGTSoundManager::getInstance()->stopAllSounds();
    
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    replaceSceneNoTransition(Debug_Index);
    MGTSoundManager::getInstance()->stopAllSounds();
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    
    DeviceUtilManager::getInstance()->exit();
    
#endif
}
