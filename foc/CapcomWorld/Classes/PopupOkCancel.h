//
//  PopupOkCancel.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 3..
//
//

#ifndef __CapcomWorld__PopupOkCancel__
#define __CapcomWorld__PopupOkCancel__

#include <iostream>
#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"
#include "CardInfo.h"
#include "PopupDelegate.h"

using namespace cocos2d;

class PopupOkCancel : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    PopupOkCancel(CardInfo* card, PopupDelegate* _delegate);
    ~PopupOkCancel();
    
    CardInfo* sellCard;
    PopupDelegate* delegate;
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    //void SubUICallback(CCObject* pSender);
    
    //void setText(const char* text1,const char* text2, const char* text3);
    //void setText(const char* text1);
    
};

class PopupCardSelelctForFusion : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    
    PopupCardSelelctForFusion(CardInfo* card, const char* text, PopupDelegate *_delegate);
    ~PopupCardSelelctForFusion();
    
    CardInfo* sellCard;
    PopupDelegate* delegate;
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    //void SubUICallback(CCObject* pSender);
    
    //void setText(const char* text1,const char* text2, const char* text3);
    //void setText(const char* text1);
    
};

#endif /* defined(__CapcomWorld__PopupOkCancel__) */
