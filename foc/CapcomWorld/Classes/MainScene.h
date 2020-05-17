//
//  MainScene.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 18..
//
//

#ifndef __CapcomWorld__MainScene__
#define __CapcomWorld__MainScene__

#include <iostream>

#include "cocos2d.h"
#include "DojoLayerBattle.h"
#include "DojoLayerCard.h"
#include "DojoLayerDojo.h"
#include "DojoLayerQuest.h"
#include "DojoLayerShop.h"
#include "PlayerInfo.h"
#include "CardDeckLayer.h"
#include "GameConst.h"
#include "MyUtil.h"
#include "FileManager.h"
#include "LevelUpLayer.h"
#include "ResponseLoginInfo.h"
#include "ResponseFriendsInfo.h"
#include "UserStatLayer.h"
#include "SimpleAudioEngine.h" 
#include "PopupRetryDelegate.h"
#include "NpcInfo.h"

#define ASYNCHRONOUS (1)

class MainScene : public cocos2d::CCLayer, GameConst, public MyUtil, public PopupRetryDelegate
{
public:
    // Here's a difference. Method 'init' in cocos2d-x returns bool, instead of returning 'id' in cocos2d-iphone
    virtual bool init();
    CREATE_FUNC(MainScene);
    
    // there's no 'id' in cpp, so we recommand to return the exactly class pointer
    static cocos2d::CCScene* scene();
    
    static MainScene *instance;
    
    static MainScene *getInstance()
    {
        if (instance == NULL)
            printf("MainScene instance is NULL\n");
        return instance;
    }
    
    void openPurchasePopup(int itemId);
    
    void initUI();
        
    void bottomUICallback(CCObject* pSender);
    
    // implement the "static node()" method manually
    
    
    DojoLayerBattle *m_battleLayer;
    DojoLayerCard *m_cardLayer;
    DojoLayerDojo *m_dojoLayer;
    DojoLayerQuest *m_questLayer;
    DojoLayerShop  *m_shopLayer;
    LevelUpLayer* m_levelupLayer;
    //CardDeckLayer *m_deckLayer;
    
    void releaseSubLayers();
    
    void initDojoLayer();
    void initBattleLayer();
    void initCardLayer();
    void initTemaEditLayer();
    void initQuestLayer();
    void initShopLayer();
    //void initDeckLayer();
    
    void refreshLevelUpIcon();
    void initLevelUpLayer();
    void releaseLevelUpLayer();
    //void AddLevelUpIcon();
    //void RemoveLevelUpIcon();
    //CCMenuItemImage* pLevelUpIcon;
    //CCMenu* pLevelUpMenu;
    //void LevelUpCallback(CCObject* pSender);

    int curLayerTag;
    
//    PlayerInfo *playerInfo;
    
    //XBridge *xb;
    
//    CCPoint getCCP( CCPoint a);
//    CCPoint accp( float x, float y);
//    void registerLabel( CCPoint anchor, CCPoint pos, CCLabelTTF* pLabel, int z);

    //ccColor3B subBtn_color_normal;
    //ccColor3B subBtn_color_selected;
    
    
    void requestKakaoFriendsInfo();
    
    void onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    
    void initUserStatLayer();
    void removeUserStatLayer();
    UserStatLayer *userStatLayer;
    
    void HideMainMenu();
    void ShowMainMenu();
    
    void setEnableMainMenu(bool flag);
    
    void unregisterKakao();
    void logoutKakao();
    void gotoTitleScene();
    void switchTitleScene();
    
    void refreshUserStat();
    void refreshRivalList();
    
    int popupCnt;
    void addPopup(CCNode* child, int z);
    void removePopup();
    
    void eventDownload();
    
    void removeDojoSubMenu();
    void removeCardSubMenu();
    
    void SetNormalSubBtns();
    void SetSelectedSubBtn(int i);
    
    void MoveToCardLayer();
    
    void AsyncProcess(int reqType, const char *data);
    
    void RetryPopup(const char *text1, char* url, int reqType);
    void BtnRetry();
    char *retry_url;
    int  retry_reqType;
    
    void readNPCData();
    void parseNPCXML(xmlNode * node, CCArray *npcList);
    CCArray *npcList;
    NpcInfo* getNpc(int code);
    
    void readQuestNpcData();
    void parseQuestNpcXML(xmlNode * node, CCArray *npcList);
    CCArray *questNpcList;
    CCArray* getQuestNpc(int questID);
    
    
    //void moveToShopLayer();
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    void httpCallBack();//int reqType, const char *data);
    int callBack_reqtype;
    const char* callBack_data;
#endif
    
    void reservePopup(int popupIdx);
    void addMsgPopup();
    int reservePopupIdx;
    
    void goMainScene(int nSubLayer);
    
    static const int MAIN_LAYER_DOJO    = 0;
    static const int MAIN_LAYER_CARD    = 1;
    static const int MAIN_LAYER_BATTLE  = 2;
    static const int MAIN_LAYER_QUEST   = 3;
    static const int MAIN_LAYER_SHOP    = 4;
    
    int getCurLayer();
    
    // 카드 정렬 필터값. 전역 유지함.
    int nCardListFilter;
    
    void setRivalListRefresh();
    void cancelRivalListRefresh();
    
    /////////////////
    // test
    /////////////////
    /*
    CCArray*        aniFrame;
    void HitPlay0();
    void HitPlay1();
    void HitPlay2();
    void HitPlay3();
    void HitPlay4();
    void DefensePlay0();
    void DefensePlay1();
    void DefensePlay2();
    void DefensePlay3();
    void DefensePlay4();
    void            removeSpr(CCNode* sender, void* _tag);
    CCSprite*       enemyHitEffect[5];
    CCSprite*       myHitEffect[5];
    
    
    void InitEnemyCharacter();
    void InitMyCharacter();
    float           enemyPosX[5];
    float           enemyPosY[5];
    CCSprite*       enemyCharacter[5];
    float           myPosX[5];
    float           myPosY[5];
    CCSprite*       myCharacter[5];
     */
private:
    //const char* textAdjust(const char *input);
};



#endif /* defined(__CapcomWorld__MainScene__) */
