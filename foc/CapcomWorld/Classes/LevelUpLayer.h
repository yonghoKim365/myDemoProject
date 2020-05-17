//
//  LevelUpLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 13..
//
//

#ifndef __CapcomWorld__LevelUpLayer__
#define __CapcomWorld__LevelUpLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"

using namespace cocos2d;
using namespace std;

class LevelUpLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    LevelUpLayer(CCSize layerSize);
    ~LevelUpLayer();
    
    void InitUI();
    
    CCSprite* createNumber(int _value, CCPoint pos, float scale);
    void DrawNumber(int _value, CCPoint _pos);
    void ReleaseNumber();
    
    void TouchButton(int tag);
    void TouchEndButton(int tag);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void QuestProc(int tag);
    void AttackProc(int tag);
    void DefenseProc(int tag);
    void BackProc(int tag);
    void ResetProc(int tag);
    void Confirm(int tag);
    
    void APointLabel(int OriPoint, int AddPoint);
    void QPointLabel(int OriPoint, int AddPoint);
    
private:
    
    int QuestPoint;
    int AttackPoint;
    int DefensePoint;
    
    int minQuestPoint;
    int minAttackPoint;
    int minDefensePoint;
    
    float YPos;
    
    int curUpgradePoint;
    int maxUpgradePoint;
    
    int nLastTouchTag;
    
    CCSprite *UpgradePoints[3];
    
    CCLabelTTF* pQUestLabel;
    CCLabelTTF* pAttackLabel;
    CCLabelTTF* pDefenseLabel;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    enum BTN_TAG
    {
        QUEST_POINT_MINUS = 0,
        QUEST_POINT_PLUS,
        QUEST_POINT_ALL,
        ATTACK_POINT_MINUS,
        ATTACK_POINT_PLUS,
        ATTACK_POINT_ALL,
        DEFENSE_POINT_MINUS,
        DEFENSE_POINT_PLUS,
        DEFENSE_POINT_ALL,
        BACK,
        RESET,
        CONFIRM,
        
        BTN_TAG_TOTAL,
    };
};

#endif /* defined(__CapcomWorld__LevelUpLayer__) */
