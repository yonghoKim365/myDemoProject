//
//  ShopGold.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 20..
//
//

#ifndef __CapcomWorld__ShopGold__
#define __CapcomWorld__ShopGold__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ShopLayer.h"
#include "PopUp.h"

using namespace cocos2d;

//struct ShopItem;

class ShopGold : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ShopGold(CCSize layerSize);
    ~ShopGold();
    
    void InitUI(int _curPage);
    void SetGoldData(CCArray* arryItem);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void makeCell(Item_Data* _gold, int tag);
    
    void pageBtn(int _curPage, int itemCount);
    
    void InitPopUp(void *data);
    void RemovePopUp();
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    float StartYPos;
    int curPage;
    int maxpage;
    
    CCArray* MyGold;
    
    BasePopUP* basePopUp;
    
    bool bTouchMove;
};

#endif /* defined(__CapcomWorld__ShopGold__) */
