//
//  DojoLayerBattle.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#ifndef __CapcomWorld__DojoLayerBattle__
#define __CapcomWorld__DojoLayerBattle__

#include <iostream>

#include "cocos2d.h"
#include "BattleDuelLayer.h"
#include "BattleRankingLayer.h"
#include "GameConst.h"

using namespace cocos2d;

class DojoLayerBattle : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    //virtual bool init();
    
    DojoLayerBattle(CCSize layerSize);
    ~DojoLayerBattle();
    
    static DojoLayerBattle *instance;
    
    static DojoLayerBattle *getInstance()
    {
        if (instance == NULL)
            printf("DojoLayerBattle instance is NULL\n");
        return instance;
    }
    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerBattle);
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    BattleDuelLayer *duelBattleLayer;
    BattleRankingLayer *rankingBattleLayer;

    void InitSubLayer(int a);
    void InitUI();
    void InitLayerBtn(int selected);
    void ReleaseSubLayer();
    
    void HideMenu();
    void ShowMenu();
    void MenuCallback(CCObject* pSender);
    
    void HideSubMenu();
    void ShowSubMenu();
    int curLayerTag;

};



#endif /* defined(__CapcomWorld__DojoLayerBattle__) */
