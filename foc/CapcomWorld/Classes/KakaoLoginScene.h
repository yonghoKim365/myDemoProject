//
//  KakaoLoginScene.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 22..
//
//

#ifndef __CapcomWorld__KakaoLoginScene__
#define __CapcomWorld__KakaoLoginScene__

#include <iostream>
#include "cocos2d.h"
#include "XBridge.h"
#include "MyUtil.h"

class KakaoLoginScene : public cocos2d::CCLayer, public MyUtil
{
public:
    // Here's a difference. Method 'init' in cocos2d-x returns bool, instead of returning 'id' in cocos2d-iphone
    virtual bool init();
    static cocos2d::CCScene* scene();
    CREATE_FUNC(KakaoLoginScene);
    
    static KakaoLoginScene *instance;
    
    static KakaoLoginScene *getInstance()
    {
        return instance;
    }
    void MenuCallback(CCObject* pSender);
    
    void showButtons(bool flag);
    
    //void goTitleScene();
    void switchMainScene();
    void goMainScene();
    void changeImage();
    bool bExitPopup;    
private:
    
    void gameLogic(cocos2d::CCTime dt);
    
    void keyBackClicked();

    
};

#endif /* defined(__CapcomWorld__KakaoLoginScene__) */
