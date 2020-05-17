//
//  ShopRoulette.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 21..
//
//

#ifndef __CapcomWorld__ShopRoulette__
#define __CapcomWorld__ShopRoulette__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ShopLayer.h"
#include "PopUp.h"

using namespace cocos2d;
using namespace std;

enum ROULETTE_ICON
{
    ROULETTE_ICON_REDGEM = 0,
    ROULETTE_ICON_GREENGEM,
    ROULETTE_ICON_CAPCOM,
    ROULETTE_ICON_CARD,
    ROULETTE_ICON_COIN,
    ROULETTE_ICON_KO,
    
    ROULETTE_ICON_TOTAL,
};

class RouletteMachine : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    RouletteMachine(CCSize layerSize);
    ~RouletteMachine();
    
    void InitUI();
    
    void Repeat(int iconIndex, int addRepeat);

    void removeSpr(CCNode* sender, void* _data);
    
    void RoulettePlay(CCSprite* pSprAni, CCArray *aniFrames, CCPoint pos, int _tag, int _z, SEL_CallFuncND selector, int repeat, int selectedIcon);
    
private:
    float Ypos1[3];
    
    CCSprite* slot[3];
    
    CCArray *aniFrame1;
    CCArray *aniFrame2;
    CCArray *aniFrame3;
    CCAnimation *animation;

    string IconName[6];
    
    int soundID;
    
    typedef struct
    {
        int removetag;
        int selectedIcon;
    }Roulette;
};

// -- 룰렛 배경
class ShopRoulette : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    static ShopRoulette* getInstance();

    ShopRoulette(CCSize layerSize);
    ~ShopRoulette();
    
    void InitUI();
    void InitRouletteMachine();
    
    void CalRoulette(int& OutMatch, string& OutSymbol, int* OutSlot);
    void StartRoulette();
    void RunRoulette(ResponseRoulette* _rouletteData);
    
    void SetTouchEnable();
    
    void InitPopUp(CCNode* sender, void *data);
    void RemovePopUp();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void setMedalCount(int _count);
    
private:
    
    static ShopRoulette* instance;

    BasePopUP* basePopUp;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    int selectedIcon1;
    int selectedIcon2;
    int selectedIcon3;
    
    RouletteMachine* rouletteMachine1;
    RouletteMachine* rouletteMachine2;
    RouletteMachine* rouletteMachine3;
    
    int medalCount;
    
    int slot[3];
};


#endif /* defined(__CapcomWorld__ShopRoulette__) */
