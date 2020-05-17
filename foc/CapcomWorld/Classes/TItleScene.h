//
//  TitleScene.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 18..
//
//

#ifndef __CapcomWorld__TitleScene__
#define __CapcomWorld__TitleScene__

#include <iostream>

#include "cocos2d.h"
#include "XBridge.h"
#include "MyUtil.h"

class TitleScene : public cocos2d::CCLayer, public MyUtil
{
public:
    // Here's a difference. Method 'init' in cocos2d-x returns bool, instead of returning 'id' in cocos2d-iphone
    virtual bool init();
    
    void onHttpRequestCompleted(CCObject *pSender, void *data);

    // there's no 'id' in cpp, so we recommand to return the exactly class pointer
    static cocos2d::CCScene* scene();
    
    
    
    // implement the "static node()" method manually
    CREATE_FUNC(TitleScene);
/*
//    static std::string *kClientID = "88543098824610336";
//    static std::string *kClientSecret = "mQBOSCzOvub+conITjarmsj9vDPI+drfA7ZJpRp6JR5rmkimtC18kPPVpKLWcqOUAZaYX3YQajyxTM87uLaNTA==";
//    static std::string *kRedirectURL ="kakao88543098824610336://exec";
    std::string *kAccessTokenKey ="accessToken";
    std::string *kRefreshTokenKey ="refreshToken";
*/
    static TitleScene *titleInst;
    
    static TitleScene *getInstance()
    {
        if (titleInst == NULL){
            //int a = 0;
        }
        return titleInst;
    }
    
    void requestKakaoFriendsInfo();
    void switchMainScene();
    void goMainScene();
    void switchLoginScene();
    void goLoginScene();
    
    void callScaleAction();
    
private:
    
    void gameLogic(cocos2d::CCTime dt);
    
    
    //void kakaoLogin();
    //void setupKakao();
    
    //XBridge *xb;
    
    
//    id<KakaoAuthLoginViewControllerDelegate> _delegate;
//    - (void)authLoginViewControllerdidFinishLogin:(KakaoAuthLoginViewController *)authLoginViewController
};

#endif /* defined(__CapcomWorld__TitleScene__) */
