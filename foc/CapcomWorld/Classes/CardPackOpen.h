//
//  CardPackOpen.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 12. 13..
//
//

#ifndef __CapcomWorld__CardPackOpen__
#define __CapcomWorld__CardPackOpen__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ShopLayer.h"
#include "CardDetailViewLayer.h"

using namespace cocos2d;

class CardPackOpen : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    CardPackOpen(CCSize layerSize, CCArray* _cardpack, bool _IsTutorial = false);
    ~CardPackOpen();
    
    void InitUI();
    void ShowCard();
    
    void CardPackOpenAction();
    
    void CardPackCut_01();
    void CardPackCut_02();
    void RemoveCut02();
    
    void FadeWhite01();
    void FadeWhite02();
    void FadeWhite03();
    void FadeWhite04();
    
    void RemoveFade01();
    void RemoveFade02();
    void RemoveFade03();
    void RemoveFade04();
    
    void CreateBtn();

    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

private:
    
    CCArray*                cardPack;
    int                     showCount;
    bool                    IsTutorial;
    CardDetailViewLayer*    cardDetailViewLayer;
    
    bool                    IsTouchCardPack;
    
    enum CARDPACK_TAG
    {
        BLACK_BG = 800,
        CARDPACK,
        CUT_LINE,
        ANI_TOUCH_ICON,
        CARDPACK_LEFT,
        CUT_CARD_1,
        CUT_CARD_2,
        FADE_01,
        FADE_02,
        FADE_03,
        FADE_04,

        BTN,
        LABEL,
        
    };

};

#endif /* defined(__CapcomWorld__CardPackOpen__) */
