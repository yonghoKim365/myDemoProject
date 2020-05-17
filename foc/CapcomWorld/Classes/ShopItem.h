//
//  ShopItem.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 19..
//
//

#ifndef __CapcomWorld__ShopItem__
#define __CapcomWorld__ShopItem__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ShopLayer.h"
#include "PopUp.h"

using namespace cocos2d;

//struct ShopItem;

class ShopItem : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ShopItem(CCSize layerSize);
    ~ShopItem();
    
    void InitUI(int _curPage);
    void SetItemData(CCArray* arryItem);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void makeCell(Item_Data* _item, int tag);
    
    void pageBtn(int _curPage, int itemCount);
    
    void InitPopUp(void *data);
    void RemovePopUp();
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    float StartYPos;
    int curPage;
    int maxpage;
    
    CCArray* MyItem;
    
    BasePopUP* basePopUp;
    
    bool bTouchMove;
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
};

#endif /* defined(__CapcomWorld__ShopItem__) */
