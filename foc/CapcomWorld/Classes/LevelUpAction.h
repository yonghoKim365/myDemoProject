//
//  LevelUpAction.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 12. 12..
//
//

#ifndef __CapcomWorld__LevelUpAction__
#define __CapcomWorld__LevelUpAction__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"

using namespace cocos2d;
using namespace std;

class LevelUpAction : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    LevelUpAction(CCSize layerSize, int _uPoint);
    ~LevelUpAction();
    
    void InitUI();
    void TextAction();
    void setTouch();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
private:
    int uPoint;
};

#endif /* defined(__CapcomWorld__LevelUpAction__) */
