//
//  SocialListLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 5..
//
//

#ifndef __CapcomWorld__SocialListLayer__
#define __CapcomWorld__SocialListLayer__

#include <iostream>

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ACardMaker.h"
#include "CCHttpRequest.h"
#include "XBridge.h"
#include "AKakaoUser.h"
#include "ARequestSender.h"
#include "FriendsInfo.h"
#include "CollectListLayer.h"

using namespace cocos2d;
using namespace std;

#define SOCIAL_MEDAL_HEIGHT (156)
#define SOCIAL_INVITE_HEIGHT (110)

class SocialPopUp;

struct UserMedalInfo : CCObject
{
    FriendsInfo* user;
    int          tag;
};

class SocialListLayer : public cocos2d::CCLayer, MyUtil, GameConst, ThumbLayerDelegate
{
public:
    SocialListLayer(CCSize layerSize, int _cellKind);
    ~SocialListLayer();
    
    void InitLayerSize(CCSize layerSize);
    void InitUI();
    void InitFriendData();
    
    
    
    void MakeMedalCell(FriendsInfo *info,int tag, bool sendMedal = true);
    //void MakeMedalCell(const string& imgPath, const string& Name, int level, int Ranking, int tag, int leaderCard, bool sendMedal = true);
    void MakeInviteCell(const string& imgPath, const string& Name, bool bMsgBlock, int tag);
    int GetNumOfFriend();
    
    void callProfileImg(cocos2d::CCObject *pSender, void *data);
    //void onEnter();
    //void onExit();

    void InitMedalPopUp(void *data);
    void InitInvitePopUp(AKakaoUser *data);
    void RemovePopUp();

    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();

    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    int clip_start_y;
    float LayerStartPos;
    //bool bMakeOptionBtn;
    int cellKind;

    void registerProfileImg(std::string filename);
    //XBridge *xb;
    
    void MakeCells();
    int cellMakeCnt;
    int cellMakeCnt2;
    
    ACardMaker *cardMaker;
    
    void beforeOpenDetailView();
    void afterCloseDetailView();
    void setEnableTouchAfterDelay();
    void setThumbTouch(bool b);
    
    void setDisableInviteBtn(long long userID);
    
private:
    
    class PortraitUrl : public cocos2d::CCObject
    {
    public:
        std::string url;
        int y;
    };
    CCDictionary portraitDic;
    
    CCArray* MyFriendList;

    float StartYpos;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    int NumOfFriend;
    
    //string ProfileUrl;
    float  ProfileYPos;
    
    SocialPopUp* basePopUp;

    ResponseSMexclude* SMexcludeInfo;

    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    CCArray *sortedAppFriendList;
};


#endif /* defined(__CapcomWorld__SocialListLayer__) */
