//
//  MyItemList.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 6..
//
//

#ifndef __CapcomWorld__MyItemList__
#define __CapcomWorld__MyItemList__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"

using namespace cocos2d;
using namespace std;

class MyItemListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    MyItemListLayer(CCSize layerSize);
    ~MyItemListLayer();
    
private:
};

#endif /* defined(__CapcomWorld__MyItemList__) */
