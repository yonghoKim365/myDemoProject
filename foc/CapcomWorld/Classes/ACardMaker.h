//
//  ACardMaker.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 27..
//
//

#ifndef __CapcomWorld__ACardMaker__
#define __CapcomWorld__ACardMaker__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "CardInfo.h"
USING_NS_CC;



class ACardMaker : public cocos2d::CCLayer, MyUtil, GameConst
{
public :
    void MakeCardThumb(CCLayer *layer, CardInfo *card, CCPoint ccp, int thumb_h, int z, int _tag);
    CCSprite *createNumber(int number, cocos2d::CCPoint pos, float scale);
    void refreshLevel(CCLayer *_layer, int _level, int x, int y, float scale);
    
    
};


#endif /* defined(__CapcomWorld__ACardMaker__) */
