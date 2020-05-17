//
//  ShopLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 19..
//
//

#ifndef __CapcomWorld__ShopLayer__
#define __CapcomWorld__ShopLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
//#include "ShopItem.h"

using namespace cocos2d;
using namespace std;

#define ITEM_PER_PAGE (15)

typedef struct : public CCObject
{
    string  ItemName;
    string  ItemDesc1;
    string  ItemDesc2;
    int     ItemAmount;
    int     ItemPrice;
    
}tempShopItem;

typedef struct : public CCObject
{
    string  ItemName;
    string  ItemDesc1;
    string  ItemDesc2;
    int     ItemAmount;
    int     ItemPrice;
    
}tempShopGold;

class ShopItem;
class ShopGold;
class ShopTreasure;
class ShopRoulette;

class ShopLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    static ShopLayer* getInstance();
    
    ShopLayer(CCSize layerSize);
    ~ShopLayer();
    
    void InitUI();
        
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    
    void InitItemList();
    void InitItemLayer(int curPage);
    
    void InitGoldList();
    void InitGoldlayer(int curPage);
    
    void InitTreasureLayer();
    
    void InitRouletteLayer(int _medalCount);
    
    void ReleaseSubLayer();
    
    void InitBtn(int curTab);
    
private:
    
    static ShopLayer* instance;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    ShopItem*       shopItemLayer;
    ShopGold*       shopGoldLayer;
    ShopTreasure*   shopTreasureLayer;
    ShopRoulette*   shopRouletteLayer;
    
    CCArray* arryItem;
    CCArray* arryGold;

    float LayerEndPos;
    
    CCPoint movingVector;
    
    enum TAB_TYPE
    {
        ITEM_TAB =0,
        GOLD_TAB,
        TREASURE_TAB,
        ROULETTE_TAB,
        
        TOTAL_TAB,
    };
    
    bool bTouchMove;
    
    int curTab;
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
};


#endif /* defined(__CapcomWorld__ShopLayer__) */
