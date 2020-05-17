//
//   ShopTreasure.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 20..
//
//

#ifndef __CapcomWorld___ShopTreasure__
#define __CapcomWorld___ShopTreasure__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ShopLayer.h"
#include "PopUp.h"
#include "PlayerInfo.h"

using namespace cocos2d;
using namespace std;

typedef struct
{
    int CardID;
    int Coin;
    int Honor;
    
}Reward;

class ShopTreasure : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ShopTreasure(CCSize layerSize);
    ~ShopTreasure();
    
    void InitUI();
    void SetTreasureData(CCArray* arryItem);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void makeCell(const string& _SprPath, const string& _BoxName, const string& _Desc, const string& _HonorPoint, int _tag);
    
    void pageBtn(int _curPage, int itemCount);
    
    void InitPopUp(void *data);
    void RemovePopUp();
    
    void requestTreasure(CCNode* sender, void* _boxInfo);
    
    void eff1();
    void eff2();
    void whiteBG();
    void releaseEff();
    void releaseBG();
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    float StartYPos;
    int curPage;
    int maxpage;
    
    CCArray* MyTreasure;
    
    BasePopUP* basePopUp;
    
    Reward* reward;
    
    bool bTouchMove;
};

#endif /* defined(__CapcomWorld___ShopTreasure__) */
