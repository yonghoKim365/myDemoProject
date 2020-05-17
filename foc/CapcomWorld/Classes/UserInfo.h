//
//  UserInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 23..
//
//

#ifndef __CapcomWorld__UserInfo__
#define __CapcomWorld__UserInfo__

#include <iostream>

#include "cocos2d.h"
#include "KakaoInfo.h"
#include "ResponseLoginInfo.h"
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>

#define USERSTAT_EXP_NUM            100

using namespace std;

class UserInfo : public cocos2d::CCObject, public KakaoInfo
{
    
public:
    UserInfo();
    ~UserInfo();
    cocos2d::CCSprite* pPlayerFaceImg;
    int myCoin;
    int myCash;
    short myLevel;
    int myFame;
    //string myNickname;
    //string profileImageUrl;
    
    int xp, _maxXp;
    
    int battlePoint;
    int maxBattlePoint;
    //int defenceBP;
    //int maxDefensePoints;
    int revengePoint;
    int backgroundID;
    int upgradePoint;
    
    
    
    
    
    //int stamina;
    //int maxStamina;
    
    //int _battlePoint;
    //int maxBattlePoint;
    
    
    
    
    int staminaPoint;
    int maxStaminaPoint;
    int tutorialProgress;
    int battleCnt;
    int victoryCnt;
    int drawCnt;
    
    // battle에서 쓰임
    int ranking;
    int leaderCard;
    int attack;
    int defense;
    
    
    void setLevel(int a){
        myLevel = a;
    }
    int getLevel(){
        return myLevel;
    }
    
    void setXp(int a){
        xp = a;
    }
    int getXp(){
        return xp;
    }
    void setMaxXp(int a){
        _maxXp = a;
    }
    int getMaxXp(){
        return _maxXp;
    }
    
    void setCoin(int a){
        myCoin = a;
    }
    int getCoint(){
        return myCoin;
    }
    int addCoin(int a){
        myCoin+=a;
        return myCoin;
    }
    
    void setCash(int a){
        myCash = a;
    }
    int getCash(){
        return myCash;
    }
    void setFame(int a){
        myFame = a;
    }
    int getFame(){
        return myFame;
    }
    
    void setBattlePoint(int a){ // old - setAttackPoint
        battlePoint = a;
    }
    int getBattlePoint(){
        return battlePoint;
    }
    void setMaxBattlePoint(int a){
        maxBattlePoint = a;
    }
    int getMaxBattlePoint(){
        return maxBattlePoint;
    }
    
    
    
    void setBackground(int a){
        backgroundID = a;
    }
    int getBackground(){
        return backgroundID;
    }
    
    void setRevengePoint(int a){
        revengePoint = a;
    }
    int getRevengePoint(){
        return revengePoint;
    }
    
    void setUpgradePoint(int a){
        upgradePoint = a;
    }
    int getUpgradePoint(){
        return upgradePoint;
    }
    
    void setStamina(int a) {
        staminaPoint = a;
    }
    
    int getStamina() {
        return staminaPoint;
    }
    
    void setMaxStamina(int a) {
        maxStaminaPoint = a;
    }
    
    int getMaxStamina() {
        return maxStaminaPoint;
    }
    
    void decreaseQuestPoint(int a){
        staminaPoint -= a;
    }
    int addStamina(int a){
        staminaPoint += a;
        if (staminaPoint > maxStaminaPoint){
            staminaPoint = maxStaminaPoint;
        }
    }
    
    
    void setTutorial(int a){
        tutorialProgress = a;
    }
    
    int getTutorial(){
        return tutorialProgress;
    }
    
    void SetUserProfileUrl(const char* _url){
        profileImageUrl = _url;
    }
    
    const char* GetUserProfileUrl(){
        return profileImageUrl.c_str();
    }
    
    void SetName(const char* _name){
        myNickname = _name;
    }
    
    const char* GetName(){
        return myNickname.c_str();
    }
    
    
    
    void setBattleCnt(int a){
        battleCnt = a;
    }
    
    void setVictoryCnt(int a){
        victoryCnt = a;
    }
    
    void setDrawCnt(int a){
        drawCnt = a;
    }
    
    int getBattleCnt(){
        return battleCnt;
    }
    int getVictoryCnt(){
        return victoryCnt;
    }
    int getDrawCnt(){
        return drawCnt;
    }
    void setRanking(int a){
        ranking = a;
    }
    int getRanking(){
        return ranking;
    }
    
    
    void setUserInfo(ResponseLoginInfo *info);
    
    //void setQuestPoint(int a){
    //    staminaPoint = a;
    //}
    //int getQuestPoint(){
    //    return staminaPoint;
    //}
    //void setMaxQuestPoint(int a){
    //    maxStaminaPoint = a;
    //}
    //int getMaxQuestPoint(){
    //    return maxStaminaPoint;
    //}
    /*
     int getExpCap()
     {
     if (myLevel < USERSTAT_EXP_NUM)
     return expCap[myLevel];
     return expCap[USERSTAT_EXP_NUM - 1];
     }
     
     int getExpBase()
     {
     if (myLevel == 0)
     return 0;
     else if (myLevel < USERSTAT_EXP_NUM)
     return expCap[myLevel - 1];
     return 0; //expCap[USERSTAT_EXP_NUM - 1];
     }
     */
    /*
     void setBattlePoint(int a) {
     battlePoint = a;
     }
     
     int getBattlePoint() {
     return battlePoint;
     }
     void setMaxBattlePoint(int a) {
     maxBattlePoint = a;
     }
     
     int getMaxBattlePoint() {
     return maxBattlePoint;
     }
     */
    
    //void setDefensePoint(int a){
    //    defenceBP = a;
    //}
    //int getDefensePoint(){
    //    return defenceBP;
    //}
    //void setMaxDefencePoint(int a){
    //    maxDefensePoints = a;
    //}
    //short getMaxDefencePoint(){
    //    return maxDefensePoints;
    //}

    
private:
    /*
    static void parse();
    static void parseXml(xmlNode *node);
    static void recalcExpCap();
    
    static bool parsed;
    
    static int expCap[USERSTAT_EXP_NUM];
     */

};
#endif /* defined(__CapcomWorld__UserInfo__) */
