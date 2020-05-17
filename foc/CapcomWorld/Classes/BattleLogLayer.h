//
//  BattleLogLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#ifndef __CapcomWorld__BattleLogLayer__
#define __CapcomWorld__BattleLogLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "BattleLogListLayer.h"
#include "ResponseBasic.h"

using namespace cocos2d;

#define BATTLE_LOG_CELL_HEIGHT (120)

class BattleLogLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    static BattleLogLayer* getInstance();
    
    BattleLogLayer(CCSize layerSize);
    ~BattleLogLayer();
    
    void InitUI();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

    BattleLogListLayer* pBattleLogListLayer;
    
private:
    
    static BattleLogLayer* instance;
    
    ResponseNoticeInfo* noticeInfo;
    
  
};


#endif /* defined(__CapcomWorld__BattleLogLayer__) */
