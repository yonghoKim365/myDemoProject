//
//  DojoLayerCollect.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 26..
//
//

#ifndef __CapcomWorld__DojoLayerCollect__
#define __CapcomWorld__DojoLayerCollect__

#include <iostream>
#include "CollectListLayer.h"

#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"

class DojoLayerCollect : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    DojoLayerCollect(CCSize layerSize);
    ~DojoLayerCollect();
    
    void InitUI();
    
    static DojoLayerCollect *instance;
    
    static DojoLayerCollect *getInstance()
    {
        if (instance == NULL)
            printf("DojoLayerCollect instance is NULL\n");
        return instance;
    }

    ResponseCollectionInfo* collectionInfo;
    
    void setCollectData(ResponseCollectionInfo* _collectionInfo);
    
private:
    
    CollectListLayer* m_pCollectListlayer;
    
    
};

#endif /* defined(__CapcomWorld__DojoLayerCollect__) */

