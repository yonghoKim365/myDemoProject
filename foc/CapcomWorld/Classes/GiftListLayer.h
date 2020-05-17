//
//  GiftListLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 6..
//
//

#ifndef __CapcomWorld__GiftListLayer__
#define __CapcomWorld__GiftListLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "GiftInfo.h"


using namespace cocos2d;
using namespace std;

#define GIFT_HEIGHT (170)

class BasePopUP;

typedef struct : public cocos2d::CCObject
{
    string  giftImgPath;
    string  giftName;
    string  giftDescription;
    string  giftSenderName;
    int     giftCount;
    
}Gift;

class GiftListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    GiftListLayer(CCSize layerSize);
    ~GiftListLayer();
    
    void InitUI();
    
    void SetGiftData(CCArray* _gift);
    
    void MakeGiftCell(GiftInfo* pGift, int tag);
    
    void InitLayerSize(CCSize layerSize);
    
    void ReceiveGift(int Index);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    
    int GetCountOfGift() const;

    void removePopUp();
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    
    float LayerStartPos;

private:
    
    CCArray* myGift;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    float StartYPos;
    
    BasePopUP* basePopUp;
    
    int nLastTag;
};

#endif /* defined(__CapcomWorld__GiftListLayer__) */
