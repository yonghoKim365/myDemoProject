//
//  PopupRetry.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 21..
//
//

#ifndef __CapcomWorld__PopupRetry__
#define __CapcomWorld__PopupRetry__

#include <iostream>

#include "cocos2d.h"
#include "GameConst.h"
#include "PopupRetryDelegate.h"
#include "MyUtil.h"

using namespace cocos2d;



class PopupRetry : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    PopupRetry();
    ~PopupRetry();
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    
    void setText(const char* text1,const char* text2, const char* text3);
    void setText(const char* text1);
    void setText_quest(const char* text1);
    
    PopupRetryDelegate *delegate;
    void setDelegate(PopupRetryDelegate *dele);
    void Retry();
    
};
#endif /* defined(__CapcomWorld__PopupRetry__) */
