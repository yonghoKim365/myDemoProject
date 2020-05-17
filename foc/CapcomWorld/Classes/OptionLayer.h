//
//  OptionLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#ifndef __CapcomWorld__OptionLayer__
#define __CapcomWorld__OptionLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"

using namespace cocos2d;

class OptionLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    OptionLayer(CCSize layerSize);
    ~OptionLayer();
    
    void InitUI();
    void InitLayer(int a);
    void SubUICallback(CCObject* pSender);
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    int selectedLayer;
    
};


#endif /* defined(__CapcomWorld__OptionLayer__) */
