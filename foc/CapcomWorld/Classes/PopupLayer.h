//
//  PopupLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 28..
//
//

#ifndef __CapcomWorld__PopupLayer__
#define __CapcomWorld__PopupLayer__

#include <iostream>

#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"

using namespace cocos2d;



class PopupLayer : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    
    PopupLayer();
    ~PopupLayer();
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    
    void setText(const char* text1,const char* text2, const char* text3);
    void setText(const char* text1);
    void setText_quest(const char* text1);
    
    int nCallFrom;
};



#endif /* defined(__CapcomWorld__PopupLayer__) */
