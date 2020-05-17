//
//  PlayerInfo.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#include "PlayerInfo.h"
#include "cocos2d.h"
#include "CardDictionary.h"
#include "AKakaoUser.h"
#include "ARequestSender.h"
#include "UserStatLayer.h"

PlayerInfo* PlayerInfo::inst = NULL;

void PlayerInfo::Init(){
    myCash = 0;
    myCoin = 0;
    
    myLevel = 0;
    
    staminaPoint = 20;
    maxStaminaPoint = 0;
    
    battlePoint = 1;
    maxBattlePoint = 1;
    
    //defenceBP = 0;
    //maxDefensePoints = 0;
    
    numOfKakaoAppFriends = 9;
    numOfKakaoFriends = 0;
    
    //deviceID ="aa";
    
    kakaoFriendsInfo = new CCArray();
    kakaoAppFriendsInfo = new CCArray();
    gameFriendsInfo = new CCArray();
    
    MyItemCount = 0;
    
    getSoundEffectOption();
    getBgmOption();
    
    getTutorialCompleted();
    getTutorialProgress();
    TutorialLeaderCardID = 0;
    
}


void PlayerInfo::InitCardList(){
//    //cocos2d::CCLog("playerInfo.initCardList");
//    myCards = new CCArray();
//
//    
//    int tempMyCard[] = {
//         30752,30753,31831,31832,31833,31881,31882,31883,31884,30882
//        ,30883,30803,30804,32821,32822,30711,30712,31921,31922,31923
//        ,31972,31973,30762,30763,30764,32083,32084,32731,32732,32734
//        ,31522,30542,30543,30544,30841,30843,30844,32911,32913,32914
//        ,30662,30663,30664,30391,30571,30572,30573,30574,32542,32543
//        ,31622,31623,30501,30742,32782,32452,30912,30913,30914,30952
//        ,30953,32672,32673,31523,31524};
//    
//       //32481,32483,31583,31584,31781,31782,32501,32503,30941,30942
//       //31731,31732,31733,31861,31862,32391,32393
//    
//    for( int i=0;i<65;i++){
//    //for( int i=0;i<10;i++){
//        CardInfo *info = CardDictionary::sharedCardDictionary()->getInfo(tempMyCard[i]);
//        if (info == NULL)
//            continue;
//        
//        info->SetFusionLevel(info->getId()%10);
//        info->setRare(i%10);
//        //CCLog(" card id :%d fusionlevel:%d",info->getId(), info->GetFusionLevel());
//        myCards->addObject(info); // myCardsÎ•????Î°?Îß????    }
//    }
//    /*
//    for(int i=0;;i++){
//        //for(int i=0;i<3;i++){
//        CardInfo *info = CardDictionary::sharedCardDictionary()->getInfo(i+1 + 30010);
//        if (info == NULL)
//            continue;
//        
//        info->SetFusionLevel(i%4);
//        
//        info->setLevel(10*i + i);
//        info->setAttack(10+i);
//        info->setDefence(10*i+i);
//        info->setBp(i*12);
//        info->setRare(i*2);
//        
//        myCards->addObject(info); // myCardsÎ•????Î°?Îß????        if (myCards->count() == 10)
//            break;
//    }
//     */
}



void PlayerInfo::InitDeck()
{
    myDeck = new DeckInfo();
}


CardInfo** PlayerInfo::getCardDeck(int a){
    
    switch(a){
        case 0:
            return myDeck->atkDeck1;
        case 1:
            return myDeck->atkDeck2;
        case 2:
            return myDeck->atkDeck3;
        case 3:
            return myDeck->atkDeck4;
        case 4:
            return myDeck->defDeck1;
    }
    return NULL;
}

void PlayerInfo::SetCardToDeck(int _team, int _id, int _n, CardInfo *card){
    myDeck->SetCardToDeck(_team, _id, _n, card);
}

void PlayerInfo::SetCardToDeck(ResponseDeckinfo *info)
{
    myDeck->SetCardToDeck(info);
}

void PlayerInfo::EmptyDeck(int _team, int _id){
    myDeck->EmptyDeck(_team, _id);
}

CardInfo* PlayerInfo::GetCardInDeck(int _team, int _id, int _n){
    if (_team == 0){
        switch(_id){
            case 0: return myDeck->atkDeck1[_n];
            case 1: return myDeck->atkDeck2[_n];
            case 2: return myDeck->atkDeck3[_n];
            case 3: return myDeck->atkDeck4[_n];
            case 4: return myDeck->atkDeck5[_n];
        }
    }
    else if (_team == 1){
        return myDeck->defDeck1[_n];
    }
    
    return myDeck->atkDeck1[0];
}


CardInfo* PlayerInfo::GetCardInTeam(int _n){
    return myDeck->atkDeck1[_n];
}

void PlayerInfo::LogTeamInfo()
{
    for(int i=0;i<5;i++){
        CardInfo *cardInTeam = PlayerInfo::getInstance()->GetCardInDeck(0,0,i);
        if (cardInTeam != NULL){
            CCLog("team[%d] card id:%d srl:%d",i, cardInTeam->getId(), cardInTeam->getSrl() );
        }
        else{
            CCLog("team[%d] is Empty");
        }
    }
}

bool PlayerInfo::isBelongInTeam(CardInfo *card)
{
    if (card == NULL)return false;
    
    for(int i=0;i<4;i++){
        for(int j=0;j<5;j++){
            CardInfo *c = GetCardInDeck(0,i,j);
            if (c != NULL){
                if (card->getSrl() == c->getSrl())return true;
            }
        }
    
    }
    /*
    for(int j=0;j<5;j++){
        CardInfo *c = GetCardInDeck(1,0,j);
        if (c!= NULL){
            if (card->getSrl() == c->getSrl())return true;
        }
    }
     */
    
    return false;
}

bool PlayerInfo::isBelongInTeam(int attackTeamID, CardInfo *card)
{
    if (card == NULL)return false;
    
    for(int j=0;j<5;j++){
        CardInfo *c = GetCardInDeck(0,attackTeamID,j);
        if (c != NULL){
            if (card->getSrl() == c->getSrl())return true;
        }
    }
    
    return false;
}

int PlayerInfo::getTeamBattlePoint(int _team, int _id)
{
    int tot_battlePoint = 0;
    for (int i=0;i<5;i++){
        CardInfo *card = GetCardInDeck(_team, _id, i);
        if (card != NULL){
            //tot_attack += card->getAttack();
            tot_battlePoint += card->getBp();
        }
    }
    return tot_battlePoint;
}

bool PlayerInfo::isTeamEmpty(int _n)
{
    int cardCnt = 0;
    for (int i=0;i<5;i++){
        CardInfo *card = GetCardInDeck(0, _n, i);
        if (card != NULL)cardCnt++;
    }
    
    if (cardCnt==0)return true;
    
    return false;    
}

/*
void PlayerInfo::SetDeviceID(const char* str){
    deviceID = str;
}
*/
const char* PlayerInfo::GetDeviceID(){
    //return deviceID;
    return xb->getDeviceID();
}


void PlayerInfo::SetCardsFromLoginResponse(CCArray *cards){
    
    if (myCards != NULL){
        myCards->removeAllObjects();
        myCards = NULL;
    }
    
    //CCLog(" SetCardsFromLoginResponse ");
    
    myCards = new CCArray();
    for(int i=0;i<cards->count();i++){
        CardInfo *_card = (CardInfo*)cards->objectAtIndex(i);
        
        addCard(_card->getId(), _card);
    }
}

void PlayerInfo::addCard(int cardID, CardInfo* _card)
{
    CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(cardID);
    
    //CCLog("addcard, ci id:%d bTraingingMaterial:%d", ci->getId(), ci->bTraingingMaterial);
    
    CardInfo* acard = new CardInfo();
    acard->setId(ci->cardId);
    acard->cardAtrb = ci->cardAtrb;
    acard->fusionCount = ci->fusionCount;
    acard->rareVal = ci->rareVal;
    acard->expVal = ci->expVal;
    acard->cardLevel = ci->cardLevel;
    acard->ap = ci->ap;
    acard->dp = ci->dp;
    acard->battlePoint = ci->battlePoint;
//    acard->skill = ci->skill;
    acard->skillPlus = ci->skillPlus;
    acard->cardName = ci->cardName;
    acard->cardCharacter = ci->cardCharacter;
    //acard->grade = ci->grade;
    acard->availableToSale = ci->availableToSale;
    acard->cardPrice = ci->cardPrice;
    acard->srlId = ci->srlId;
    acard->skillEffect = _card->skillEffect;
    acard->skillType = ci->getSkillType();
    acard->SetFusionLevel(_card->getId()%10);
    acard->setSrl(_card->getSrl());
    acard->setLevel(_card->getLevel());
    acard->setExp(_card->getExp());
    acard->setAttack(_card->getAttack());
    acard->setDefence(_card->getDefence());
    //acard->updateRare();
    acard->series = ci->series;
    acard->bTraingingMaterial = ci->bTraingingMaterial;
    
    //CCLog("PlayerInfo::addCard, id:%d rare:%d", acard->getId(), acard->getRare());// acard->series:%d", acard->series);
    
    myCards->addObject(acard);
    
}

CardInfo* PlayerInfo::makeCard(int cardID, CardInfo* detailInfoCard){
    CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(cardID);
    
    // DB record + User record
    CardInfo* acard = new CardInfo();
    acard->setId(ci->cardId);
    acard->cardAtrb = ci->cardAtrb;
    acard->fusionCount = ci->fusionCount;
    acard->rareVal = ci->rareVal;
    acard->expVal = ci->expVal;
    acard->cardLevel = ci->cardLevel;
    acard->ap = ci->ap;
    acard->dp = ci->dp;
    acard->battlePoint = ci->battlePoint;
//    acard->skill = ci->skill;
    acard->skillPlus = ci->skillPlus;
    acard->cardName = ci->cardName;
    acard->cardCharacter = ci->cardCharacter;
    //acard->grade = ci->grade;
    acard->availableToSale = ci->availableToSale;
    acard->cardPrice = ci->cardPrice;
    acard->srlId = ci->srlId;
    acard->series = ci->series;
    acard->bTraingingMaterial = ci->bTraingingMaterial;
    
    acard->SetFusionLevel(detailInfoCard->getId()%10);
    acard->setSrl(detailInfoCard->getSrl());
    acard->setLevel(detailInfoCard->getLevel());
    acard->setExp(detailInfoCard->getExp());
    acard->setAttack(detailInfoCard->getAttack());
    acard->setDefence(detailInfoCard->getDefence());
    //acard->updateRare();
    
    
    //CCLog("PlayerInfo::makeCard, cardid:%d rare:%d", acard->getId(), acard->getRare());
    
    return acard;
}

void PlayerInfo::addToMyCardList(CardInfo *_card)
{
    myCards->addObject(_card);
}


// iphone4s 88623212837755537
// iphone5  88610764144128545

/*
 Cocos2d: kakaoFriend id:88610764144063049 nick:Íπ????Î¶??)
 Cocos2d: kakaoFriend id:88610764128939196 nick:Íπ???? Cocos2d: kakaoFriend id:88610764113220199 nick:???
 Cocos2d: kakaoFriend id:88610764132389607 nick:????? Cocos2d: kakaoAppFriend id:88610764144063049 nick:Íπ????Î¶??)*/

void PlayerInfo::addKakaoFriend(long long _userId, const char* _nickName, const char* _profileURL, const char* _friend_nickName, bool _msg_block){
    AKakaoUser *user = new AKakaoUser();
    user->userID = _userId;
    user->nickname = _nickName;
    user->profileImageUrl = _profileURL;
    user->friendsNickName = _friend_nickName;
    user->bMessageBlocked = _msg_block;
    
//    if (user->userID<0)user->userID*=-1;
    
    CCLog("kakaoFriend id:%lld nick:%s msgblock:%d", user->userID, user->nickname.c_str(), user->bMessageBlocked);
    
    kakaoFriendsInfo->addObject(user);
}
void PlayerInfo::addKakaoAppFriend(long long _userId, const char* _nickName, const char* _profileURL, const char* _friend_nickName, bool _msg_block){
    AKakaoUser *user = new AKakaoUser();
    user->userID = _userId;
    
    user->friendsNickName = _friend_nickName;
    user->bMessageBlocked = _msg_block;
    if (user->userID<0)user->userID*=-1;
    
    std::string nick = _nickName;
    user->nickname = nick;
    std::string imgUrl = _profileURL;
    user->profileImageUrl = imgUrl;
    
    CCLog("kakaoAppFriend id:%lld nick:%s msgBlock:%d", user->userID, user->nickname.c_str(), user->bMessageBlocked);
    
    kakaoAppFriendsInfo->addObject(user);
}

void PlayerInfo::setGameFriends(CCArray *friends){
    
    gameFriendsInfo = NULL;
    gameFriendsInfo = new CCArray();
    
    for(int i=0;i<friends->count();i++){
        FriendsInfo *myFriend = new FriendsInfo();
        
        FriendsInfo *f = (FriendsInfo*)friends->objectAtIndex(i);
        
        myFriend->userID = f->userID;
        myFriend->level = f->level;
        myFriend->ranking = f->ranking;
        myFriend->profileURL = getGameUserProfileURL(f->userID);
        std::string nick = getGameUserNickname(f->userID);
        myFriend->nickname = nick;
        myFriend->leaderCard = f->leaderCard;
        myFriend->attack = f->attack;
        myFriend->defense = f->defense;
        
        gameFriendsInfo->addObject(myFriend);
      
        CCLog("PlayerInfo::setGameFriends");
        CCLog("myFriend->nickname :%s", myFriend->nickname.c_str());
        CCLog("myFriend->userID :%lld", myFriend->userID);
        CCLog("myFriend->level :%d", myFriend->level);
        CCLog("myFriend->ranking:%d", myFriend->ranking);
        CCLog("myFriend->profileURL:%s",myFriend->profileURL.c_str());
        CCLog("myFriend->leaderCard:%d",myFriend->leaderCard);
        
    }
}

std::string PlayerInfo::getGameUserProfileURL(long long _userId){
    for(int i=0;i<this->kakaoAppFriendsInfo->count();i++){
        AKakaoUser *user = (AKakaoUser*)this->kakaoAppFriendsInfo->objectAtIndex(i);
        if (user->userID == _userId){
            return user->profileImageUrl;
        }
    }
    return "";
}


std::string PlayerInfo::getGameUserNickname(long long _userId){
    for(int i=0;i<this->kakaoAppFriendsInfo->count();i++){
        AKakaoUser *user = (AKakaoUser*)this->kakaoAppFriendsInfo->objectAtIndex(i);
        if (user->userID == _userId){
            //CCLog("getGameUserNickname, nickname:%s",user->nickname.c_str());
            return user->nickname;
        }
    }
    //return NULL;
    return "NONAME";
}


CardInfo* PlayerInfo::getCardBySrl(int _srl)
{
    for(int i=0;i<myCards->count();i++){
        CardInfo *_card = (CardInfo*)myCards->objectAtIndex(i);
        
        if (_card->getSrl() == _srl)return _card;
    }
    return NULL;
}

void PlayerInfo::refreshUserStat()
{
    ResponseRefreshInfo* refreshInfo = ARequestSender::getInstance()->requestRefresh();

    if(refreshInfo)
    {
        //CCLog(" refreshInfo user stat fame:%d, exp:%d", refreshInfo->user_fame, refreshInfo->exp);
                
        if (atoi(refreshInfo->res) == 0){
            this->setFame(refreshInfo->user_fame);
            this->setBattlePoint(refreshInfo->user_attackPoint);
            //this->setDefensePoint(refreshInfo->user_defensePoint);
            this->setStamina(refreshInfo->user_questPoint);
            this->setCoin(refreshInfo->coin);
            this->setCash(refreshInfo->gold);
            this->setXp(refreshInfo->exp);
            this->setMaxXp(refreshInfo->max_exp);
            this->setRevengePoint(refreshInfo->user_revenge);
            this->setLevel(refreshInfo->user_level);
            this->setBattleCnt(refreshInfo->battleCnt);
            this->setVictoryCnt(refreshInfo->victoryCnt);
            this->setDrawCnt(refreshInfo->drawCnt);
            this->setRanking(refreshInfo->user_ranking);
            //delete responseInfo;
            if (UserStatLayer::getInstance()){
                UserStatLayer::getInstance()->refreshUI();
            }
        }
        else{
            //delete responseInfo;
            // error
        
        }
    }
    
    CC_SAFE_DELETE(refreshInfo);
}



void PlayerInfo::removeCard(CardInfo *card)
{
    myCards->removeObject(card);
}

void PlayerInfo::UpdateQuestLockState(ResponseQuestListInfo *receivedQuestList){
    
    // lock / unlock compare
    for (int i=0;i<this->questList->count()-1;i++){
        QuestInfo *info = (QuestInfo*)this->questList->objectAtIndex(i);
        
        if (i==0)info->lockState = 1;
        
        for(int j=0;j<receivedQuestList->questList->count();j++){
            AQuestInfo *receivedQuestInfo = (AQuestInfo*)receivedQuestList->questList->objectAtIndex(j);
            if (receivedQuestInfo->questID == info->questID){
                if (receivedQuestInfo->clear == 1){// || receivedQuestInfo->progress > 100){
                    //if (receivedQuestInfo->progress == 1){
                    info->lockState = 1; // unlock
                    QuestInfo *nextInfo = (QuestInfo*)this->questList->objectAtIndex(i+1);
                    nextInfo->lockState = 1;
                }
            }
        }
    }
}

/*
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
 */


void PlayerInfo::recordMedalSentTime(long long userId)
{
    
    CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
    char buf1[30];
    sprintf(buf1, "kakao%lld", userId);
    
    time_t curTime = time(NULL);
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    int cur_total_sec = cur_sec + cur_min*60 + cur_hour * 3600;
    
    int t = curTime;
    
    CCLog("recordMedalSentTime,user:%lld time:%d", userId, t);
    
    char buf2[20];
    sprintf(buf2, "%d", t);
    
    gameData->setStringForKey(buf1, buf2);
    gameData->flush();
    
    int time_offset_h = cur_total_sec/3600;
    cur_total_sec = cur_total_sec%3600;
    int time_offset_m = cur_total_sec/60;
    cur_total_sec = cur_total_sec%60;
    int time_offset_s =  cur_total_sec;
    
    
    CCLog(" %dH %dM %dS",time_offset_h, time_offset_m, time_offset_s);
}

bool PlayerInfo::isEnableSendMedal(long long userId)
{
    
    char buf1[30];
    sprintf(buf1, "kakao%lld", userId);
    
    CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
    
    std::string strRecordedTime = gameData->getStringForKey(buf1);
    
    if (atoi(strRecordedTime.c_str())==0)return true;
    
    int nRecordedTimeSec = atoi(strRecordedTime.c_str());
    
    CCLog("isEnableSendMedal,user:%lld recored time:%d", userId, nRecordedTimeSec);
    
    time_t recTime = time(NULL);
    recTime = nRecordedTimeSec;
    int recTime_sec  = localtime(&recTime)->tm_sec;
    int recTime_min  = localtime(&recTime)->tm_min;
    int recTime_hour = localtime(&recTime)->tm_hour;
    
    CCLog("recorded time:  %dH %dM %dS",recTime_hour, recTime_min, recTime_sec);
    //////////////////////////////////////////
        
    time_t curTime = time(NULL);
    int cur_sec  = localtime(&curTime)->tm_sec;
    int cur_min  = localtime(&curTime)->tm_min;
    int cur_hour = localtime(&curTime)->tm_hour;
    int cur_t = curTime;
    CCLog("curTime time:%d %dH %dM %dS",cur_t, cur_hour, cur_min, cur_sec);
    
    int timeOffsetSec = 60 * 60 * 24 * 30;// 60sec(1m) * 60 minute * 24 hour = 1 day * 30 day;
    //int timeOffsetSec = 60; // 60 sec
    
    if (nRecordedTimeSec + timeOffsetSec <= cur_t){
        // limit time over
        CCLog("limit time over");
        return true;
    }
    else{
        // limit time not over
        CCLog("limit time not over");
    }
    
    return false;
}

/*
void PlayerInfo::addRewardCardToMyCardList(QuestRewardCardInfo *rewardCard)
{
    CardInfo *cardInfo = new CardInfo();
    cardInfo->autorelease();
    cardInfo->setId(rewardCard->card_id);
    cardInfo->setSrl(rewardCard->card_srl);
    cardInfo->setExp(rewardCard->card_exp);
    cardInfo->setLevel(rewardCard->card_level);
    cardInfo->setAttack(rewardCard->card_attack);
    cardInfo->setDefence(rewardCard->card_defense);
    cardInfo->setSkillEffect(rewardCard->card_skillEffect);
    //cardInfo->getRare()
    
    CardInfo* newCard = this->makeCard(rewardCard->card_id, cardInfo);
    this->addToMyCardList(newCard);
}
*/