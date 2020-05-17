//
//  PopUp.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 20..
//
//

#ifndef __CapcomWorld__PopUp__
#define __CapcomWorld__PopUp__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"
#include "ACardMaker.h"
#include "CardDictionary.h"
#include "MainScene.h"
#include "AKakaoUser.h"
#include "XBridge.h"
#include "CardPackOpen.h"
#include "SocialListLayer.h"

using namespace cocos2d;


class BasePopUP : public cocos2d::CCLayer, public MyUtil
{
public:
    BasePopUP();
    virtual ~BasePopUP();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

private:
    
protected:
    
};

class ItemPopUp : public BasePopUP, GameConst
{
public:
    ItemPopUp();
    ~ItemPopUp();
    
    virtual void InitUI(void* data);

    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void PossiblePurchase(void* data);
    void ImpossiblePurchase();

private:
    
    int itemID;
    
    enum POPUP_BTN_TYPE
    {
        PURCHASE_BTN = 0,
        CANCEL_BTN,
        CHARGE_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };

};

class GoldPopUp : public BasePopUP, GameConst
{
public:
    GoldPopUp();
    ~GoldPopUp();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    Item_Data* itemData;
    
    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        RIGHT_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class TreasurePopUp : public BasePopUP, GameConst
{
public:
    TreasurePopUp();
    ~TreasurePopUp();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void Coin(int coin);
    void Card(int card);
    void LowHonor();
    void MaxCard();
    
private:
    
    enum POPUP_BTN_TYPE
    {
        MYCARD_BTN = 0,
        BATTLE_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class RoulettePopUP : public BasePopUP, GameConst
{
public:
    RoulettePopUP();
    ~RoulettePopUP();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void Coin(int coin);
    void Card(ResponseRoulette* data);
    void Item(int itemId);
    void MaxCard();
    
private:
    
    enum POPUP_BTN_TYPE
    {
        MYCARD_BTN = 0,
        CLOSE_BTN,
        BTN_TOTAL,
    };
};

class QuestPopUp : public BasePopUP, GameConst
{
public:
    QuestPopUp();
    ~QuestPopUp();
    
    virtual void InitUI(void *data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

    void TeamEdit();
    void Charge();
    void Replay();
    void NoReward();
    
private:

    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        TEAM_EDIT_BTN,
        MYCARD_BTN,
        REPLAY_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class ItemUsePopUp : public BasePopUP, GameConst
{
public:
    
    ItemUsePopUp();
    ~ItemUsePopUp();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    int itemID;
    
    CardPackOpen* cardOpen;
    
    enum POPUP_BTN_TYPE
    {
        USE_BTN = 0,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class GiftPopUp : public BasePopUP, GameConst
{
public:
    GiftPopUp();
    ~GiftPopUp();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    int srlID;
    GiftInfo* gift;
    
    enum POPUP_BTN_TYPE
    {
        RECEIVE_BTN = 0,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class SocialPopUp : public BasePopUP, GameConst
{
public:
    SocialPopUp();
    ~SocialPopUp();
    
    virtual void InitUI(void* data);
    
    void InitUI2(AKakaoUser *user);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    UserMedalInfo *friendInfo;
    AKakaoUser *kakaoInfo;
    XBridge *xb;
    
    enum POPUP_BTN_TYPE
    {
        SEND_MADAL_BTN = 0,
        INVITE_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};


class TutorialPopUp : public BasePopUP, GameConst
{
public:
    TutorialPopUp();
    ~TutorialPopUp();
    
    virtual void InitUI(void *data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void displayPage0();
    void displayPage1();
    void displayPage2();
    void displayPage3();
    void displayPage4();
    void displayPage5();
    void displayPage6();
    void displayPage7();
    void displayPage8();
    void displayPage9();
    void displayPage10();
    void displayPage11();
    
private:
    
    int page;
    
    enum POPUP_BTN_TYPE
    {
        NEXT_BTN = 0,
        QUEST_BTN,
        FUSION_BTN,
        TEAM_BTN,
        BATTLE_BTN,
        DONE_BTN,
        REPLAY_BTN,
        CLOSE_BTN,
        ULTRACOMBO_BTN,
        
        BTN_TOTAL,
    };
};

class Purchase2Popup : public BasePopUP, GameConst
{
public:
    Purchase2Popup();
    ~Purchase2Popup();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        RIGHT_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class FusionPopup : public BasePopUP, GameConst
{
public:
    FusionPopup();
    ~FusionPopup();
    
    virtual void InitUI(void* data, int cost);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
//    void setCost(int v);
//    int  cost;
    
private:
    
    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        RIGHT_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class TrainingPopup : public BasePopUP, GameConst
{
public:
    TrainingPopup();
    ~TrainingPopup();
    
    virtual void InitUI(void* data, int cost);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
//    void setCost(int v);
//    int  cost;
    
private:
    
    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        RIGHT_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

class AlertPopup : public BasePopUP, GameConst
{
public:
    AlertPopup();
    ~AlertPopup();
    
    virtual void InitUI(void* data);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    
    
    
private:
    
    Item_Data* itemData;
    
    enum POPUP_BTN_TYPE
    {
        CHARGE_BTN = 0,
        RIGHT_BTN,
        CLOSE_BTN,
        
        BTN_TOTAL,
    };
};

#endif
