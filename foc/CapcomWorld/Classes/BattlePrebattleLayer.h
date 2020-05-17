//
//  BattlePrebattleLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 20..
//
//

#ifndef __CapcomWorld__BattlePrebattleLayer__
#define __CapcomWorld__BattlePrebattleLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "BattlePlayerCell.h"
#include "BattleFullScreen.h"
#include "BattleListLayer.h"
#include "CardTeamSimpleDrawLayer.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
//#include "XBridge.h"
#endif


using namespace cocos2d;

class BattlePrebattleLayer : public cocos2d::CCLayer, MyUtil, GameConst, BattlePlayerCellButtonDelegate
{
public:
    
    BattlePrebattleLayer(CCSize layerSize, UserInfo *user, int _team);
    ~BattlePrebattleLayer();
    
    static BattlePrebattleLayer *instance;
    
    static BattlePrebattleLayer *getInstance()
    {
        if (instance == NULL)
            printf("BattlePrebattleLayer instance is NULL\n");
        return instance;
    }
    
    void InitUI();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void InitLayer();
    
    UserInfo *battleUser;
    
    ACardMaker *cardMaker;
    
    CardTeamSimpleDrawLayer *teamLayer;
    
    void changeTeam(int a);
    
    //CardInfo *cards[5];
    
    //XBridge *xb;
    const char* TeamLabel[5];
    int selectedTeam;
    
    bool bTouchPressed;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    void reserveRefresh(int r);
    int reserveRefreshVal;
    void doRefresh();
#endif

};

#endif /* defined(__CapcomWorld__BattlePrebattleLayer__) */
