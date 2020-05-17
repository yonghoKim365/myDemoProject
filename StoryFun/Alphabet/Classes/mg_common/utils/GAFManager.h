
#ifndef __GAFManager_H__
#define __GAFManager_H__

#pragma once

#include "SingletonBase.h"
#include "cocos2d.h"

#include "GAF.h"
#include "GAFTimelineAction.h"

NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;
USING_NS_CC;

    
class GAFManager: public CSingletonBase<GAFManager>
{

public:
    GAFManager();
    ~GAFManager();
    bool init();

public:
    
    CC_SYNTHESIZE(bool, _isResEmbeded, IsResEmbeded);
    
    std::map <std::string, GAFAsset*>	m_gafAssetMap;
    
    /* gaf 파일의 GAFAsset를 GAFManager에 추가시킨다 */
    void addGafAsset(std::string filePath);
    
    /* gaf 파일의 GAFAsset를 GAFManager에서 제거한다 */
    void removeGafAsset(std::string filePath);
    
    /* GAFManager에서 관리하는 GAFAsset을 전부 제거한다 */
    void removeAllGafAssets();
    
    /* gaf 파일의 GAFAsset을 리턴 */
    GAFAsset* getGafAsset(std::string filePath, bool isCommonRes = false);
    
    /* common gaf 파일의 GAFAsset을 리턴 */
    GAFAsset* getCommonGafAsset(std::string filePath);

private:
    /* 파일명만 리턴 */
    std::string getFileName(std::string filePath);
};


#endif
