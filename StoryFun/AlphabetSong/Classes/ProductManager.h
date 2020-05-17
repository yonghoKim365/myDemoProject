/*
 *  ProductManager.h
 *
 *
 *   on 12. 10. 4..
 *  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
 */


#ifndef PRODUCTMANAGER_H
#define PRODUCTMANAGER_H

#include "cocos2d.h"
#include "MGTResourceUtils.h"

USING_NS_CC;
using namespace resourceutils;

#define PRODUCT_DEBUGMODE 1
#define PRODUCT_DEBUGMODE_SPEED 0

typedef struct _productInfo
{
    int bookNum;
    int dayNum;
    int alphabetNum;
    int lastStepNum;
    int typeNum;
    bool isFinished;
}ProductInfo;



class ProductManager
{
    
private:
    
    ProductInfo     m_productInfo;
    std::string     m_prefix;
    
public:
    ProductManager();
    ~ProductManager();
    
    
    static          ProductManager* getInstance();
    static void     releaseInstance();
    
    CC_SYNTHESIZE(bool, _isResEmbeded, IsResEmbeded);
    
    void            initSet();
    
    void            setProduct(int bookNum, int dayNum, int alphabetNum, int stepNum, bool bFinished);
    void            setProduct(int bookNum, int dayNum, int alphabetNum);
    void            setProduct(std::string prefix);
    
    int             getCurrentBook();
    int             getCurrentAlphabet();
    int             getCurrentDay();
    void            setTypeNum(int type);
    int             getCurrentType();

    
    ProductInfo*    getProductInfo();
    std::string     getProductID();
    std::string     getStringAppendPrefix(std::string strTemp);
    
    bool            getIsDebugMode();
    
    
    void            setCurrentStep(int index);
    int             getCurrentStep();
    
    void            nextStep();
    void            replay();
    
    
    //set the userdefalut key value that contentID is added.
    void            setUserDefaultBoolForKey(std::string pKey , bool value);
    void            setUserDefaultIntegerForKey(std::string pKey , int value);
    void            setUserDefaultFloatForKey(std::string pKey , float value);
    void            setUserDefaultStringForKey(std::string pKey , std::string value);
    void            setUserDefaultDoubleForKey(std::string pKey , double value);
    
    
    // get the userdefalut key value that contentID is added.
    bool            getUserDefaultBoolForKey(std::string pKey);
    int             getUserDefaultIntegerForKey(std::string pKey);
    float           getUserDefaultFloatForKey(std::string pKey);
    std::string     getUserDefaultStringForKey(std::string pKey);
    double          getUserDefaultDoubleForKey(std::string pKey);
    
    
    
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
    
    
    //  exit
    void            next();
    void            exit();
    
};

#endif
