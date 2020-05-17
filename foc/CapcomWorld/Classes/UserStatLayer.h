//
//  UserStatLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 13..
//
//

#ifndef __CapcomWorld__UserStatLayer__
#define __CapcomWorld__UserStatLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"

using namespace cocos2d;

class UserStatLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    UserStatLayer();
    ~UserStatLayer();
    
    static UserStatLayer *instance;
    
    static UserStatLayer *getInstance()
    {
        if (instance == NULL)
            printf("UserStatLayer instance is NULL\n");
        return instance;
    }
    
    //PlayerInfo *playerInfo;
    
    CCSprite *createNumber(int number, CCPoint pos, float scale = 1.0f);
    CCSprite *createComma(CCPoint pos, float scale = 1.0f);
    
    
    
    
    
//    CCSprite *questPointsGauge[3];
//    void loadQuestPointsGauge();
//    void refreshQuestPointsGauge();
    
    CCSprite *attackPointsGauge[3];
    void loadAttackPointsGauge();
    void refreshAttackPointsGauge();
    
    CCSprite *defensePointsGauge[3];
    void loadDefensePointsGauge();
    void refreshDefensePointsGauge();
    
    CCSprite* staminaGauge[3];
    void loadStaminaGauge();
    void refreshStaminaGauge();
    
    CCSprite* battlePointsGauge[3];
    void loadBattlePointsGauge();
    void refreshBattlePointsGauge();
    
    
    
    
    
    CCSprite *xpGauge;
    void loadXpGauge();
    void refreshXpGauge();
    
    
    
    
    
//    CCSprite *questPoints[3];
//    CCSprite *maxQuestPoints[3];
//    void refreshQuestPoints();
//    void refreshMaxQuestPoints();
//    void removeQuestPoints();
//    void removeMaxQuestPoints();
    
//    CCSprite *attackPoints[3];
//    CCSprite *maxAttackPoints[3];
    void refreshAttackPoints();
//    void refreshMaxAttackPoints();
//    void removeAttackPoints();
//    void removeMaxAttackPoints();
    
//    CCSprite *defensePoints[3];
//    CCSprite *maxDefensePoints[3];
//    void refreshDefensePoints();
//    void refreshMaxDefensePoints();
//    void removeDefensePoints();
//    void removeMaxDefensePoints();
    
    CCSprite* stamina[3];
    CCSprite* maxStamina[3];
    void refreshStamina();
    void refreshMaxStamina();
    void removeStamina();
    void removeMaxStamina();
    
    CCSprite* battlePoints[3];
    CCSprite* maxBattlePoints[3];
    void refreshBattlePoints();
    void refreshMaxBattlePoints();
    void removeBattlePoints();
    void removeMaxBattlePoints();

    
    
    
    
    CCSprite *cash[10];
    void refreshCash();
    void removeCash();
    
    CCSprite *pSprCoin[10];
    void refreshCoin();
    void removeCoin();
    
    CCSprite *level[3];
    void refreshLevel();
    void removeLevel();
    
    CCSprite *friendCount[3];
    void refreshFriendCount();
    void removeFriendCount();
    
    void updateUserProfileImage();
    void registerUserProfileImg(std::string filename);
    void profileImgDownloaded(cocos2d::CCObject *pSender, void *data);
    //std::string getUserProfileFilename(std::string path);
    //bool IsFileExist(const char* FilePath);
    
    void refreshUI();
    
    void topUICallback(CCObject* pSender);
    
    void HideMenu();
    void ShowMenu();
    void setEnableMenu(bool flag);
    
    void AddLevelUpIcon();
    void RemoveLevelUpIcon();
    void LevelUpCallback(CCObject* pSender);
    CCMenuItemImage* pLevelUpIcon;
    CCMenu* pLevelUpMenu;
};


#endif /* defined(__CapcomWorld__UserStatLayer__) */
