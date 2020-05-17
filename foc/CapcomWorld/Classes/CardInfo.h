//
//  CardInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#ifndef __CapcomWorld__CardInfo__
#define __CapcomWorld__CardInfo__

#include <iostream>
#include "cocos2d.h"

enum CARD_ATRB
{
    ATRB_SMASH = 1,
    ATRB_GUARD,
    ATRB_THROW
};

class CardInfo : public cocos2d::CCObject
{
public:
    
    static const int MAX_LEVEL = 40;
    CardInfo();
    ~CardInfo();
    void setId(int cardId)
    {
        this->cardId = cardId;
        SetFusionLevel(cardId%10);
    }
    
    int getId()
    {
        return cardId;
    }
    
    void setName(const char *name)
    {
        cardName = name;
    }
    
    const char *getName()
    {
        return cardName.c_str();
    }
    
    void setLevel(int value)
    {
        cardLevel = value;
    }
    
    int getLevel()
    {
        return cardLevel;
    }
    
    void setExp(int value)
    {
        expVal = value;
    }
    
    int getExp()
    {
        return expVal;
    }

//    void setGrade(int value)
//    {
//        grade = value;
//    }
//    
//    int getGrade()
//    {
//        return grade;
//    }
    
    void setAttribute(int attribute)
    {
        cardAtrb = attribute;
    }
    
    int getAttribute()
    {
        return cardAtrb;
    }
    
    void setAttack(int value)
    {
        ap = value;
        //updateRare();
    }
    
    int getAttack()
    {
        return ap;
    }
    
    void setDefence(int v){
        dp = v;
        //updateRare();
    }
    
    int getDefence(){
        return dp;
    }
    
    void setBp(long v){
        battlePoint = v;
    }
    
    long getBp(){
        return battlePoint;
    }
    
    void setRare(int v){
        rareVal = v;
    }
    
    int getRare(){
        return rareVal;
    }
    
    /*
    void updateRare() {
        int value = dp + ap;
        if (value <= 100)
            rareVal = 1;
        else if (value <= 300)
            rareVal = 2;
        else if (value <= 600)
            rareVal = 3;
        else if (value <= 1000)
            rareVal = 4;
        else if (value <= 1500)
            rareVal = 5;
        else if (value <= 3000)
            rareVal = 6;
        else if (value <= 4500)
            rareVal = 7;
        else if (value <= 6000)
            rareVal = 8;
        else if (value <= 7500)
            rareVal = 9;
        else
            rareVal = 10;
    }
    */
    
    void setFusionCount(int count) {
        fusionCount = count;
    }
    
    int getFusionCount() {
        return fusionCount;
    }
    
    void SetFusionLevel(int n){
        fusionLevel = n;
    }
    
    int GetFusionLevel(){
        return fusionLevel;
    }
    
    void enableToSell(bool flag) {
        availableToSale = flag;
    }
    
    bool availableToSell() {
        return availableToSale;
    }
    
    /*
    void setPrice(int value) {
        cardPrice = value;
    }
    */
    int getPrice() {
        const int cost_per_level [] ={
        0,
         100, 110, 121, 134, 148,
         163, 180, 198, 218, 240,
         264, 291, 321, 354, 390,
         429, 472, 520, 572, 630,
         693, 763, 840, 924,1017,
        1119,1231,1355,1491,1641,
        1806,1987,2186,2405,2646,
        2911,3203,3524,3877,4265};
        
        const int plus_val_per_rare[] = { 200,630,1985,6253,19697, 30};
        
        if (rareVal == 100){
            cardPrice = cost_per_level[cardLevel] + plus_val_per_rare[5];
        }
        else{
            cardPrice = cost_per_level[cardLevel] + plus_val_per_rare[rareVal];
        }
        
        
        
        return cardPrice;
    }
    
    /*
    void setSkill(int a){
        skill = a;
    }
    int getSkill(){
        return skill;
    }
    */
    
    void setSkillPlus(int value) {
        skillPlus = value;
    }
    
    int getSkillPlus() {
        return skillPlus;
    }
    
    void setSkillEffect(int a){
        skillEffect = a;
    }
    int getSkillEffect(){
        return skillEffect;
    }
    
    void setSkillType(int a)
    {
        skillType = a;
    }
    
    int getSkillType()
    {
        return skillType;
    }
    
    void setSrl(int a){
        srlId = a;
    }
    int getSrl(){
        return srlId;
    }
    /*
    void setExpCap(int level, int cap)
    {
        expCap[level - 1] = cap;
    }
    */
    
    int getExpCap()
    {
        
        const int capArray[] = {
                 0,   200,   500,   845,  1242,
              1699,  2225,  2830,  3526,  4327, //~9
              5249,  6310,  7531,  8936, 10552,
             12411, 14549, 17008, 19836, 23089, // ~19
             26830, 31133, 36082, 41774, 48320,
             55848, 64506, 74463, 85914, 99083, // ~29
            114228,131645,151675,174710,201201,
            231666,266701,306992,353327,406613  // ~39
        };
                
        if (cardLevel < 40){
            return capArray[cardLevel];
        }
        else {
            return capArray[39];
        }
        /*
        if (cardLevel < 10)
            return expCap[cardLevel];
        else
            return 200000000;
         */
    }
    
    void recalcExpCap()
    {
        for (int scan = 9;scan > 1;scan--)
        {
            int value = 0;
            for (int loop = scan;loop > 0;loop--)
            {
                value += expCap[loop];
            }
            expCap[scan] = value;
        }
    }
    
    /*
    void setTrainingExp(int level, int value)
    {
        trainingExp[level - 1] = value;
    }
    */
    int getTrainingExp()
    {
        return trainingExp[cardLevel - 1];
    }
    
    int cardId;

    int cardAtrb;       // 공격(SMASH), 방어, 던지기
    int fusionLevel;    // 카드 합성 등급
    int fusionCount;    // 카드 합성횟수
    //std::string cardImagePath;
    int rareVal;        // 레어도
    int expVal;         // 경험치
    int cardLevel;      // 카드 레벨
    long ap;            // attack power;
    long dp;            //defencePower
    long battlePoint;
    //int skill;
    std::string cardName;   // 카드 이름
    int cardCharacter;      // 케릭터 종류
    //int grade;
    bool availableToSale;
    int cardPrice;
    int expCap[10];
    int trainingExp[10];
    
    
    int skillType;
    int skillPlus;
    int skillEffect;    // skillPlus가 누적된 값이 skillEffect
    int srlId;    // card unique id
    int series;
    
    //bool bTrainging;
    bool bTraingingMaterial;
    //int  training_exp; // 단련시 획득 경험치, 단련이 가능한 카드에만 해당.
    
    
};
#endif /* defined(__CapcomWorld__CardInfo__) */
