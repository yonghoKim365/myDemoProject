//
//  CardTeamSimpleDrawLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 20..
//
//

#ifndef __CapcomWorld__CardTeamSimpleDrawLayer__
#define __CapcomWorld__CardTeamSimpleDrawLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "BattlePlayerCell.h"
#include "BattleFullScreen.h"
#include "BattleListLayer.h"

using namespace cocos2d;

class CardTeamSimpleDrawLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    CardTeamSimpleDrawLayer(int nTeam, int nIdx);
    ~CardTeamSimpleDrawLayer();
    
    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void InitLayer(int _nTeam, int _nIdx);
    
    ACardMaker *cardMaker;
    
    //CardInfo *cards[5];
    
};
#endif /* defined(__CapcomWorld__CardTeamSimpleDrawLayer__) */
