//
//  BattleFullScreen.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 18..
//
//

#ifndef __CapcomWorld__BattleFullScreen__
#define __CapcomWorld__BattleFullScreen__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"
#include "ShakeAction.h"
#include "TraceResultLayer.h"

using namespace cocos2d;
using namespace std;

#define NEW_BATTLE  (1)
#define HP_COUNT    (4)

class BattleEnemyLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    BattleEnemyLayer(CCSize layerSize, int _questID=0);
    ~BattleEnemyLayer();
    
    void InitUI();
    
    void InitEnemyCharacter();
    void SlideEnemyCharacter();
    
    void            Attack();
    void            actionAttack();
    void            actionHitNShake();

    void            enemyLeaderScaleUp();
    
    void            HitPlay0();
    void            HitPlay1();
    void            HitPlay2();
    void            HitPlay3();
    void            HitPlay4();
    
    float           GetDefensePoint() const;
    void            SetDefensePoint(int def) {DefensePoint = def;};
    
    float           GetSkillDefensePoint() const;
    void            SetSkillDefensePoint(int att) {SkillDefensePoint = att;};
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();

    void            ActiveSkill(CCArray* skills, const ABattleRound* _RoundData);
    void            ActiveCriticalSkill();
    
    bool            IsFeatureSkill(CCArray* skills);
    bool            IsFeatureHealSkill(CCArray* skills);
    bool            featureCritical() {return activeCritical;};
    
private:
    
    CCSprite*       enemyBG;
    CCSprite*       enemyCharacter[5];
    CCSprite*       enemySkill[5];
    float           enemyPosX[5];
    float           enemyPosY[5];
    float           enemySkillX[5];
    float           enemySkillY[5];
       
    CCPoint         enemyBGPos;
    
    
    CCSprite*       enemyYellowGauge;
    CCSprite*       enemyRedGauge;
    
    CCSprite*       myYellowGauge;
    CCSprite*       myRedGauge;
    
    string          enemyLeader;

    CCArray*        aniFrame;
    CCAnimation*    animation;
    
    CCSprite*       enemyHitEffect[5];
    
    float           DefensePoint;
    float           SkillDefensePoint;
    
    bool            activeCritical;
    int             questID;
};

class BattleMyLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    BattleMyLayer(CCSize layerSize, int selectedTeam, int _questID=0);
    ~BattleMyLayer();
    
    void InitUI();
    
    void            InitMyCharacter();
    void            SlideMyCharacter();
    
    void            Attack();
    void            actionAttack();
    void            actionHitNShake();

    void            myLeaderScaleUp();
    
    void            DefensePlay0();
    void            DefensePlay1();
    void            DefensePlay2();
    void            DefensePlay3();
    void            DefensePlay4();
    
    float           GetAttackPoint() const;
    void            SetAttackPoint(int att) {AttackPoint = att;};

    float           GetSkillAttackPoint() const;
    void            SetSkillAttackPoint(int att) {SkillAttackPoint = att;};
    
    void            ActiveSkill(CCArray* skills, const ABattleRound* _RoundData);
    void            ActiveCriticalSkill();
    void            RemoveCriticalSkillRes();
    
    bool            IsFeatureSkill(CCArray* skills);
    bool            IsFeatureHealSkill(CCArray* skills);
    bool            featureCritical() {return activeCritical;};
    
private:
    
    CCSprite*       myBG;
    CCSprite*       myCharacter[5];
    CCSprite*       mySkill[5];
    float           myPosX[5];
    float           myPosY[5];
    float           mySkillX[5];
    float           mySkillY[5];
    CCPoint         myBGPos;
    
    string          myLeader;

    CCArray*        aniFrame;
    CCAnimation*    animation;

    CCSprite*       myHitEffect[5];
    
    int             selectTeam;
    
    float           AttackPoint;
    float           SkillAttackPoint;
    
    bool            activeCritical;
    int             questID;
};

class BattleFullScreen : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom);
    BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom, ResponseRivalBattle* _rivalBattleResult, int _questID);
    BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom, ResponseQuestUpdateResultInfo* _rivalBattleResult, int _questID);
    
    ~BattleFullScreen();
    
    void            InitUI();
    void            InitGame();
    
    void            SetBattleResult(void *data);
    
    void            actionVS();
      
    void            removeSpr(CCNode* sender, void* _tag);
    
    void            ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void            BattleProc();
    void            DrawBattleProc();
    
    void            DrawAttack();
    void            EnemyAttack(CCNode* sender, void *data);
    void            MyAttack(CCNode* sender, void *data);
    
    //void            MySkillActive();
    void            MyCriticalSkillActive();
    //void            EnemySkillActive();
    void            EnemyCriticalSkillActive();
    
    void            HitEnemy();
    void            HitMy();
    
    void            Win();
    void            Lose();
    void            Draw();

    void            cbWin();
    void            Result();
    
    void            CheckGameEnd();
    
    void            RemoveDamage();
    
    void            DrawHP(int PLAYER_TYPE, float HP, float maxHP);
    void            DrawPlayerHP();
    void            DrawEnemyHP();
    
    void            Skip();
    void            SkipInit();
    
    void            CreateAttackScoreBG(float x, float y);
    void            CreateAttackScore(int* _damage, float x, float y);
    
    void            CreateHealScoreBG(float x, float y);
    void            CreateHealScore(int _heal, float x, float y);
    
    int             nCallFromNpcBattle;
    int             nFirstAttack;
    
    void            SetTouchTrue();
    ResponseRivalBattle* rivalBattleInfoFromTrace;
    ResponseQuestUpdateResultInfo* QuestUpdateResultInfoFromTrace;
private:
        
    CCSprite*       enemyYellowGauge;
    CCSprite*       enemyRedGauge;
    
    CCSprite*       myYellowGauge;
    CCSprite*       myRedGauge;
    
    BattleEnemyLayer*   EnemyLayer;
    BattleMyLayer*      MyLayer;
    
    ResponseBattleInfo* resultInfo;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
    
    bool    play;
    int     RoundCount;
    int     ConstRoundCount;
    int     AttackCount;
    
    float enemyHP;
    float myHP;
    
    float myMaxHp;
    float enemyMaxHp;

    CCArray* BattleSequence;
    int CurrentRound;
    
    enum HP_LABEL
    {
        HP_PLAYER = 922,
        HP_ENEMY,
    };

    float   PlayerLastHP;
    float   EnemyLastHP;
    int     count;
    
    bool    IsSkipped;
    
};

#endif /* defined(__CapcomWorld__BattleFullScreen__) */
