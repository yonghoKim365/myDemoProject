//
//  QuestFullScreen.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

#ifndef __CapcomWorld__QuestFullScreen__
#define __CapcomWorld__QuestFullScreen__
/*
#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "QuestEnemy.h"
#include "CardDetailViewLayer.h"
#include "PlayerInfo.h"
#include "QuestResult.h"
#include "CardDictionary.h"

using namespace cocos2d;
using namespace std;

class QuestFullScreen : public cocos2d::CCLayer, MyUtil, GameConst, DetailViewCloseDelegate
{
public:
    
    QuestFullScreen(CCSize layerSize);
    ~QuestFullScreen();
    
    static QuestFullScreen* getInstance();
    
    void InitUI();
    
    void decreaseGauge();
    void decreaseRedGauge();
    
    void ShowRewardCard();
    void RunKO(int ultraComb);
    void RunUltra();
    
    //void Result();
    
    CCSprite* YellowGauge;
    CCSprite* RedGauge;
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void EnemyLayerTouchTrue();

    int ultraComboCount;

private:
    EnemyLayer* pEnemyLayer;
    //QuestResultLayer *pResultLayer;
    
    float GaugeXPos;
    
    static QuestFullScreen* instance;
    
    CardDetailViewLayer* _cardDetailView;
    
    bool OneTimeInit;
    
    int rewardCardID;

    ResponseQuestUpdateResultInfo* questResult;
    
    //CardDetailViewLayer *cardDetailViewLayer;
    void CloseDetailView();
};
*/

#endif /* defined(__CapcomWorld__QuestFullScreen__) */
