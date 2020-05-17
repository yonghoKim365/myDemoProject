//
//  DojoLayerShop.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#ifndef __CapcomWorld__DojoLayerShop__
#define __CapcomWorld__DojoLayerShop__

#include <iostream>

#include "cocos2d.h"
#include "GameConst.h"
#include "ShopLayer.h"

using namespace cocos2d;

class DojoLayerShop : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    //virtual bool init();
    
    DojoLayerShop(CCSize layerSize);
    ~DojoLayerShop();
    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerBattle);
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void InitShopLayer();
    
private:
    
    ShopLayer* shopLayer;
    
};

#endif /* defined(__CapcomWorld__DojoLayerShop__) */
