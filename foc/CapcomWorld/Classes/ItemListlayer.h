//
//  ItemListlayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 6..
//
//

#ifndef __CapcomWorld__ItemListlayer__
#define __CapcomWorld__ItemListlayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ItemInfo.h"

using namespace cocos2d;
using namespace std;

#define ITEM_HEIGHT (145)

typedef struct : public cocos2d::CCObject
{
    string  itemImgPath;
    string  itemName;
    string  itemDescription;
    int     itemCount;
}Item;

class BasePopUP;

class ItemListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ItemListLayer(CCSize layerSize);
    ~ItemListLayer();
    
    void InitUI();
    
    void InitLayerSize(CCSize layerSize);
    
    void SetItemData(CCArray* _item);
    
    int GetCountOfItem() const;
    
    void MakeItemCell(ItemInfo* pItem, int tag);
    void EmptyItem();
    
    void UseItem(int tag);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();

    void InitPopUp(void *data);
    void RemovePopUp();

    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    
    float LayerStartPos;
    
    CCArray* myItem;
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    float StartYPos;

    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    BasePopUP* basePopUp;
};

#endif /* defined(__CapcomWorld__ItemListlayer__) */
