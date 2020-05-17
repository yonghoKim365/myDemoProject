//
//  BattleRankingLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#ifndef __CapcomWorld__BattleRankingLayer__
#define __CapcomWorld__BattleRankingLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"

using namespace cocos2d;

class BattleRankingLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    BattleRankingLayer(CCSize layerSize);
    ~BattleRankingLayer();
    
    void InitUI();
    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerCard);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    
};

#endif /* defined(__CapcomWorld__BattleRankingLayer__) */
