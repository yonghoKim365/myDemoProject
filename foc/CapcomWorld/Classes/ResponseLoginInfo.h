//
//  ResponseLoginInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 12..
//
//

#ifndef __CapcomWorld__ResponseLoginInfo__
#define __CapcomWorld__ResponseLoginInfo__

#include <iostream>

#include "cocos2d.h"
//#include "UserInfo.h"
//#include "CardInfo.h"

using namespace cocos2d;
//using namespace std;


class ResponseLoginInfo : public cocos2d::CCObject
{
    
public:
    
    ResponseLoginInfo();
    ~ResponseLoginInfo();
    
    const char* res;
    const char* msg;

    CCArray *cardList;
    
    int myCoin;
    int myCash;
    int myLevel;
    int myFame;
    int xp,maxXp;
    int attackBP;
    int maxAttackBP;
    int defenceBP;
    int maxDefenceBP;
    int revengePoint;
    int backgroundID;
    int upgradePoint;
    int questPoints;
    int maxQuestPoints;
    int tutorialProgress;
    int battleCnt;
    int victoryCnt;
    int drawCnt;
    int myRanking;
    
    int friends_bonus;
    
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
        maxXp = a;
    }
    int getMaxXp(){
        return maxXp;
    }
    
    void setCoin(int a){
        myCoin = a;
    }
    int getCoin(){
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
    
    void setAttackPoint(int a){
        attackBP = a;
    }
    int getAttackPoint(){
        return attackBP;
    }
    void setMaxAttackPoint(int a){
        maxAttackBP = a;
    }
    int getMaxAttackPoint(){
        return maxAttackBP;
    }
    
    void setDefensePoint(int a){
        defenceBP = a;
    }
    int getDefensePoint(){
        return defenceBP;
    }
    void setMaxDefensePoint(int a){
        maxDefenceBP = a;
    }
    int getMaxDefensePoint(){
        return maxDefenceBP;
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
    
    void setQuestPoint(int a){
        questPoints = a;
    }
    int getQuestPoint(){
        return questPoints;
    }
    void setMaxQuestPoint(int a){
        maxQuestPoints = a;
    }
    int getMaxQuestPoint(){
        return maxQuestPoints;
    }
    
    void setTutorial(int a){
        tutorialProgress = a;
    }
    
    int getTutorial(){
        return tutorialProgress;
    }
    
    void setVictoryCnt(int a){
        victoryCnt = a;
    }
    int getVictoryCnt(){
        return victoryCnt;
    }
    void setBattleCnt(int a){
        battleCnt = a;
    }
    int getBattleCnt(){
        return battleCnt;
    }
    void setDrawCnt(int a){
        drawCnt = a;
    }
    int getDrawCnt(){
        return drawCnt;
    }
    void setRanking(int a){
        myRanking = a;
    }
    int getRanking(){
        return myRanking;
    }
    
};
#endif /* defined(__CapcomWorld__ResponseLoginInfo__) */
