//
//  ItemLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#ifndef __CapcomWorld__ItemLayer__
#define __CapcomWorld__ItemLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "GiftListLayer.h"
#include "ItemListlayer.h"
#include "ARequestSender.h"


using namespace cocos2d;

class ItemLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ItemLayer(CCSize layerSize, int tab);
    ~ItemLayer();
    
    void InitUI();
    
    void InitGiftLayer();
    void InitMyItemlayer();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    bool bTouchPressed;
    bool bTouchMove;
    
    GiftListLayer* pGiftLayer;
    ItemListLayer* pItemListlayer;
    int CurrentTag;
};

#endif /* defined(__CapcomWorld__ItemLayer__) */
