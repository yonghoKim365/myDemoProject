//
//  QuestResult.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 11..
//
//

#ifndef __CapcomWorld__QuestResult__
#define __CapcomWorld__QuestResult__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"
#include "ACardMaker.h"
#include "LevelUpAction.h"

using namespace cocos2d;
using namespace std;

class QuestRewardLayer;
/*
class QuestResultLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    QuestResultLayer(CCSize layersize);
    ~QuestResultLayer();
    
    void InitUI(ResponseQuestUpdateResultInfo* _questResult);
    
    void RunAction();
    
    void ClearAction();
    void GradeAction();
    
    void increaseGreenGauge();
    void increaseRedGauge();
    void decreaseYellowGauge();
    
    void InitRewardLayer();
private:
    
    CCSprite* pGreenGauge;
    CCSprite* pRedGauge;
    CCSprite* pYellowGauge;
    
    CCSprite* pRewardBG;
    
    QuestRewardLayer* pRewardLayer;
    
    ResponseQuestUpdateResultInfo* questResult;
    
    float YPos;
    
    float QuestProgressRatio;
    float increaseQuestProgressRatio;
    
    float QuestPointRatio;
    float decreaseQuestPointRatio;
    
    float UesrExpRatio;
    float increaseUesrExpRatio;
};
*/
class QuestRewardLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    QuestRewardLayer(CCSize layerSize);
    ~QuestRewardLayer();
    
    void InitUI(ResponseQuestUpdateResultInfo* _questResult);
    
    void RunAction();
    
    void InitLevelUplayer();
    void InitUPointLayer();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void tutorial();
    
    void enableMainMenu();

private:
    
    CCSprite* pQuestSelectBtn;
    CCSprite* pStageSelectBtn;
    
    float YPos;
    
    ACardMaker *cardMaker;
    ResponseQuestUpdateResultInfo *questResult;
    bool continueToPlay;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    bool bLevelUp;
    bool bUpgradePoint;
    
    LevelUpAction* actionLevelUP;
};

#endif /* defined(__CapcomWorld__QuestResult__) */
