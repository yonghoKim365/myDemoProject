//
//  DojoLayerDojo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 27..
//
//

#ifndef __CapcomWorld__DojoLayerDojo__
#define __CapcomWorld__DojoLayerDojo__





//#include "TracerLayer.h"
#include "TraceHistoryLayer.h"




#include <iostream>

#include "cocos2d.h"
#include "Cardinfo.h"
#include "DojoLayerBg.h"
#include "DojoLayerCollect.h"
#include "EventLayer.h"
#include "BattleLogLayer.h"
#include "SocialLayer.h"
#include "ItemLayer.h"
#include "OptionLayer.h"


using namespace cocos2d;

#define MAIN_BG_SCALE (2.0f)

typedef struct : public cocos2d::CCObject
{
    int ID;
    std::string L_ImgPath;
    std::string S_IMgPath;
    
}Bg_List;

enum QUICK_ICON
{
    QUICK_ICON_BATTLELOG = 1000,
    QUICK_ICON_EVENTLOG,
    QUICK_ICON_GIFT,
    QUICK_ICON_BG,
    
    
    
    
    
    QUICK_ICON_TRACER,
    
    
    
    
    
    QUICK_ICON_TOTAL,
};

class DojoLayerDojo : public cocos2d::CCLayer, GameConst, MyUtil
{
public:
    //virtual bool init();
    
    DojoLayerDojo(CCSize layerSize);
    ~DojoLayerDojo();
    void init();
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerDojo);
    
    static DojoLayerDojo *instance;
    
    static DojoLayerDojo *getInstance()
    {
        if (instance == NULL)
            printf("UserStatLayer instance is NULL\n");
        return instance;
    }
    
    
    CCSprite *revengeGauge[3 * 4];
    void loadRevengeGauge();
    void refreshRevengeGauge();
    void drawSingleRevengeGauge(int index, int x, float scale);
    void hideRevengeGauge();

    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    void BattleLogTourchEnd();
    void BattleLogTourchStart();
    void EventLogTourchEnd();
    void EventLogTourchStart();

    DojoLayerBg *m_DojoLayerBg;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    cocos2d::CCSprite* pBgSprite;
    
    void InitMainBGNameList();
    void ChangeBGImg(int idx);
    
    int curLayerTag;
    void InitLayer();
    void SubUICallback(CCObject* pSender);
    void SetNormalSubBtns();
    void SetSelectedSubBtn(int i);
    
    void InitDojoCollectLayer(ResponseCollectionInfo* _collectionInfo);
    
    void InitBgSelectLayer();
    
    void ReleaseLayer();
    
    
    
    
    
    void initTracerLayer();
    void initTraceHistoryLayer();
    void onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data);
    
    
    
    
    
    void releaseEvent();
    void releaseEventImmediately();
    void InitDetailEventLayer();
    void RemoveDetailEventLayer();
    void InitEventLayer();
    void RemoveEventLogLayer();
    
    void releaseBattleLog();
    void releaseBattleLogImmediately();
    void InitBattleLogLayer();
    void RemoveBattleLogLayer();
    void InitDetailBattleLogLayer();
    void RemoveDetailBattleLogLayer();
    
    void InitSocialLayer();
    void InitItemLayer(int tab);
    void InitOptionLayer();
    
    void RefreshUsername();
    
    ccColor3B subBtn_color_normal;
    
    cocos2d::CCDictionary*   BgDictionary;
    
    BattleLogLayer*     pBattleLogLayer;
    
    void loadCharImg();
    
    void HideMenu();
    void ShowMenu();

    void setEnableSubMenu(bool flag);
    
    BattleLogListLayer* pBattleLogListLayer;

     bool battlelogActive;
    
    void keyBackClicked();
    
    bool isExistRival;
    
    //bool bSkipRefreshRivalList;
    void refreshRivalEvent();     
    bool checkNewRivalEvent(ResponseRivalList* pRivalListInfo);
    void InitRivalUI(bool bNew);
    void refreshRivalNotiUI(ResponseRivalList* pRivalListInfo);
    
    bool bExitPopup;
private:
    
    cocos2d::CCSprite*  pSprAttackLeader;
    cocos2d::CCSprite*  pSprDefenceLeader;
    
    cocos2d::CCSprite*  pSprEvent;
    
    DojoLayerCollect*   pDojoLayerCollect;
    
    EventLayer*         pEventLayer;
    
    
    
    
    
    //TracerLayer*        pTracerLayer;
    TraceHistoryLayer*  pTraceHistoryLayer;

    
    
    SocialLayer*        pSocialLayer;
    ItemLayer*          pItemLayer;
    OptionLayer*        pOptionLayer;

    bool eventlogActive;
    
//    float startX;
//    float endX;
//    int curPage;
    
    CCSprite* BattleLogIcon;
    CCSprite* EventLogIcon;
    CCSprite* GiftIcon;
    CCSprite* BgIcon;
    
    
    
    
    
    CCSprite* TracerIcon;
    
    
    
    
    
    
    /////////
    // test
    /////////
    /*
    void InitEnemyCharacter();
    float           enemyPosX[5];
    float           enemyPosY[5];
    CCSprite*       enemyCharacter[5];
     */
};

class ExitWarningPopup : public cocos2d::CCLayer, public MyUtil, public GameConst
{
public:
    ExitWarningPopup();
    ~ExitWarningPopup();
    
    void InitUI();
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    int nCallFrom; // 0 == mainScene, 1 == LoginScene
private:
    CocosDenshion::SimpleAudioEngine* soundBG;
    
    
};

#endif /* defined(__CapcomWorld__DojoLayerDojo__) */
