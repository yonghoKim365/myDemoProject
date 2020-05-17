//
//  DojoLayerBg.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 24..
//
//

#ifndef __CapcomWorld__DojoLayerBg__
#define __CapcomWorld__DojoLayerBg__

#include <iostream>

#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"

#define BG_IMG_NUM (15)

class DojoLayerDojo;

class DojoLayerBg : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    DojoLayerBg(CCSize layerSize);
    ~DojoLayerBg();
    
    void            InitUI();
    void            InitFilename();
    
    void            ConfirmCallback(CCObject* pSender);
    
    void            RenderSelectedImg(int PosIdx);
    
    void            ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void            ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void            ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void            downloadBG();
    void            onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    
    
private:
    DojoLayerDojo*  m_DojoLayerDojo;
    CCSprite*       m_pSelected;
    
    CCSprite*       m_pTopbar;
    CCSprite*       m_pBottombar;
    //CCSprite*       m_pMiddlebar[4];
    
    int             BgCount;
    
    std::vector<int> BG_ID;
    
    int             selectBgID;
};

#endif /* defined(__CapcomWorld__DojoLayerBg__) */

