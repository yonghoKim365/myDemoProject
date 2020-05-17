//
//  QuestEnemy.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 9..
//
//

#ifndef __CapcomWorld__QuestEnemy__
#define __CapcomWorld__QuestEnemy__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ARequestSender.h"

using namespace cocos2d;
using namespace std;


#define MAX_TOUCH_MARK (25)

class EnemyLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    static EnemyLayer *getInstance()
    {
        if(instance)
            return instance;
        return NULL;
    }
    
    EnemyLayer(CCSize layerSize);
    ~EnemyLayer();
    
    void InitUI();
    void shakeUpdate(CCTime dt);
    void UltraUpdate(CCTime dt);
    
    void StopAction5();
    
    void RemoveAttackEffect();
    
    void HitPlay0();
    void HitPlay1();
    void HitPlay2();
    void HitPlay3();
    void HitPlay4();
    
    void RemoveHitEffect0();
    void RemoveHitEffect1();
    void RemoveHitEffect2();
    void RemoveHitEffect3();
    void RemoveHitEffect4();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SlideEnemy();
    
    void RunHitnShake();
    void RunShake();
    
    void RunKO();
    void RunNormalCombo();
    
    void removeTouchSpr(CCNode* sender, void* _tag);
    
    void SetTouchCreateTrue();
    
    int  getUncreatedTouchIdx();
    
    void regUltraUpdate();
    void unregUltraUpdate();
    
    void RenderComboCount();
    
    CCSprite* createNumber(int _value, CCPoint pos, float scale);
    void DrawNumber(int _value, CCPoint _pos);
    void ReleaseNumber();
    
    static void startUltraCombo()
    {
        ultraComboStarted = true;
    }
    
    static bool normalComboStarted;
    
    void UltraTouchAni(float x, float y);
    void removeUltraTouchAni();
    
    int UltraComboCount;
    
private:
    static EnemyLayer *instance;
    static bool ultraComboStarted;
    
    CCSprite* MyChar;
    CCSprite* pEnemy[5];
    float XPos[5];
    float YPos[5];
    
    CCSprite* pAttackEffect;
    CCSprite* pHitEffect[5];
   
    unsigned long UpdateStartTime;
    unsigned long AttackStartTime;
    
    bool bTouch;
    bool bOneExcute;
    
    int TouchCnt;
    
    bool bRunAction;
    
    CCArray *aniFrame;
    CCAnimation *animation;
    
    CCArray *aniFrameTouch;
    CCAnimation *animationTouch; // -- 울트라 콤보 터치 이펙트
    CCSprite* UltratouchAni;
    
    CCPoint TouchMarkPos[MAX_TOUCH_MARK];
    
    bool TouchMarkCreate;
    
    int LiveTouchMark;
    
    bool bCreated[MAX_TOUCH_MARK];
    int  arryTagTable[MAX_TOUCH_MARK];
    
    bool IsUltraMode;
    bool IsUltraModeAction;
    
    CCSprite *ComboCount[3];
    
    ResponseQuestUpdateResultInfo* questResult;
    
    // -- 스크립트 변수
    int   nomalComboCnt; // -- 울트라 콤보로 넘어가기 위한 터치수
    int   ultraRepeat;
    float ultraComboTimeOut;
    float comboScaleUPSpeed;
    float comboCreateDelay;
    float comboNumScreen; // -- 한 화면에 생성될 수 있는 콤보마크 수
    
     CocosDenshion::SimpleAudioEngine* soundBG;
    
    int TutorialLeaderCardID;
};


#endif /* defined(__CapcomWorld__QuestEnemy__) */
