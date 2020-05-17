//
//  PopupUnregister.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 11..
//
//

#ifndef __CapcomWorld__PopupUnregister__
#define __CapcomWorld__PopupUnregister__

#include <iostream>
#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"
#include "CardInfo.h"
#include "PopupDelegate.h"

using namespace cocos2d;

class PopupUnregister : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    //PopupUnregister(PopupDelegate* _delegate);
    PopupUnregister();
    ~PopupUnregister();
    
    //PopupDelegate* delegate;
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    //void SubUICallback(CCObject* pSender);
    
    //void setText(const char* text1,const char* text2, const char* text3);
    //void setText(const char* text1);
    
};

#endif /* defined(__CapcomWorld__PopupUnregister__) */
