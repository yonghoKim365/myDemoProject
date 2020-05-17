//
//  PlayerInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#ifndef __CapcomWorld__PlayerInfo__
#define __CapcomWorld__PlayerInfo__

#include <iostream>

#include "cocos2d.h"
using namespace std;

#include "CardInfo.h"
#include "DeckInfo.h"
#include "QuestInfo.h"
#include "FriendsInfo.h"
#include "UserInfo.h"
#include "ResponseBasic.h"
#include "XBridge.h"
//#include "MainScene.h"
#include "ARequestSender.h"

//class PlayerInfo : public cocos2d::CCObject, public UserInfo
class PlayerInfo : public UserInfo, public GameConst
{
    
private:
    PlayerInfo() {
        myCards = NULL;
        Init();
        //InitCardList();
        InitDeck();
        //selectedMainImgIdx = 0;
        eventList = NULL;
        traceTeam = 0;
    }
    PlayerInfo(const PlayerInfo& other);
    ~PlayerInfo() {}
    static PlayerInfo* inst;
public:
    static PlayerInfo* getInstance(){
        if (inst == 0) inst = new PlayerInfo();
        return inst;
    }
    
    // quest info
    QuestInfo questInfo;
        
    XBridge *xb;
    
    CCArray *myCards;
    void addCard(int cardID, CardInfo* _card);
    CardInfo* makeCard(int cardID, CardInfo* detailInfoCard);
    void addToMyCardList(CardInfo* _card);
    
    // deck info (attack deck, defence deck)
    DeckInfo *myDeck;
    void Init();
    
    void InitCardList();
    
    void InitDeck();
    
    //int  selectedMainImgIdx;
    
    
    //////////////////////////////////////// Team
    
    void SetCardToDeck(int _team, int _id, int _n, CardInfo *card);
    void SetCardToDeck(ResponseDeckinfo *info);
    void EmptyDeck(int _team, int _id);
    CardInfo* GetCardInDeck(int _team, int _id, int _n);
    CardInfo* GetCardInTeam(int _n);
    void LogTeamInfo();
    int getTeamBattlePoint(int _team, int _id);
    
    CardInfo** getCardDeck(int a);
    bool isBelongInTeam(CardInfo *card);
    bool isBelongInTeam(int attackTeamID, CardInfo *card);
    //void SetDeviceID(const char* str);
    bool isTeamEmpty(int _n);
    
    const char* GetDeviceID();
    //const char* deviceID;
    
    void SetCardsFromLoginResponse(CCArray *cards);
    
    // kakao friends info
    CCArray *kakaoFriendsInfo;      // AKakaoUser
    CCArray *kakaoAppFriendsInfo;   // AKakaoUser
    
    void addKakaoFriend(long long userId, const char* nickName, const char* profileURL, const char* friend_nickName, bool msg_block);
    void addKakaoAppFriend(long long userId, const char* nickName, const char* profileURL, const char* friend_nickName, bool msg_block);
    void setKakaoFriendsInfo(int numOfFriend, int numOfAppFriend){
        numOfKakaoFriends = numOfFriend;
        numOfKakaoAppFriends = numOfAppFriend;
    }

    // game friends info
    CCArray *gameFriendsInfo;       // FriendsInfo
    void setGameFriends(CCArray *friends);

    short getQp() const
    {
        return staminaPoint;
    }
    
    std::string getGameUserProfileURL(long long userId);
    std::string getGameUserNickname(long long _userId);
    
    void UpdateQuestLockState(ResponseQuestListInfo *receivedQuestList);
    
//private:
    std::vector<int> *bgList;
    
    CardInfo* getCardBySrl(int _srl); // srl Í∞??Î°?Ïπ¥Î?Î•?Ï∞æÏ? Î¶¨Ì????.
    
    CCArray *battleList; // battle tab??? ?¨Ï?. battle tab Ïß????Î¶¨Ï??∏Î? Î∞????? ?¨Í∏∞????????.
    
    ResponseBattleInfo *battleResponseInfo;
    
    void refreshUserStat();
    
    CCArray *questList;
    
    bool bBgmOn;
    bool bSoundEffectOn;
    bool bTutorialCompleted;
    int curProgress;
    
    void setBgmOption(bool a){
        bBgmOn = a;
        CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
        gameData->setBoolForKey("bBgmOption",bBgmOn);
        gameData->flush();
    }
    bool getBgmOption(){
//        if (CCUserDefault::isXMLFileExist() == false){
//            setBgmOption(true);
//            setSoundEffectOption(true);
//        }
        bBgmOn = CCUserDefault::sharedUserDefault()->getBoolForKey("bBgmOption");
        return bBgmOn;
    }
    bool getSoundEffectOption(){
        bSoundEffectOn = CCUserDefault::sharedUserDefault()->getBoolForKey("bSoundEffect");
        return bSoundEffectOn;
    }
    void setSoundEffectOption(bool a){
        bSoundEffectOn = a;
        CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
        gameData->setBoolForKey("bSoundEffect",bSoundEffectOn);
        gameData->flush();
    }
    void setTutorialCompleted(bool a)
    {
        bTutorialCompleted = a;
        CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
        gameData->setBoolForKey("bTutorial",bTutorialCompleted);
        gameData->flush();
    }
    bool getTutorialCompleted()
    {
        bTutorialCompleted = CCUserDefault::sharedUserDefault()->getBoolForKey("bTutorial");
        return bTutorialCompleted;
    }
    
    void setTutorialProgress(int _curProgress)
    {
        ARequestSender::getInstance()->requestTutorialProgress(_curProgress);
        this->setTutorial(_curProgress);
        /*
        curProgress = _curProgress;
        CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
        gameData->setIntegerForKey("TutorialProgress",curProgress);
        gameData->flush();
         */
    
    }

    int getTutorialProgress()
    {
        //return GameConst::TUTORIAL_TOTAL;//TUTORIAL_TOTAL;

        /*
        curProgress = CCUserDefault::sharedUserDefault()->getIntegerForKey("TutorialProgress");
        return curProgress;
        */
        return this->getTutorial();
    }
    
    CCArray *myItemList; //  array of ItemInfo class
    int     MyItemCount; // array??countÍ∞???? Í∞?ItemInfo??count;
    CCArray *myGiftList; //  array of GiftInfo class
    
    CCArray* eventList;
    
    int TutorialLeaderCardID;
    
    void removeCard(CardInfo* card);
    
    std::string accessToken;
    
    int traceTeam;
      
    void SetTutorialState(int state,  bool flag)
    {
        bool TutorialCompleted = flag;
        CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
        gameData->setBoolForKey(TutorialBooleanTable[state].c_str(), TutorialCompleted);
        gameData->flush();
    }
    
    bool GetTutorialState(int state)
    {
        return CCUserDefault::sharedUserDefault()->getBoolForKey(TutorialBooleanTable[state].c_str());
    }
    
    int friends_bonus; // 친구수에 따른 추가 공격력 수치. 친구수 * friends_bonus,
    
    void recordMedalSentTime(long long userId);
    bool isEnableSendMedal(long long userId);
    
    //void addRewardCardToMyCardList(QuestRewardCardInfo *card);

private:

    
    
    
    
    

   
};

#endif /* defined(__CapcomWorld__PlayerInfo__) */

