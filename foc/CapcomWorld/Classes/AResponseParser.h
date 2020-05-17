//
//  AResponseParser.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#ifndef __CapcomWorld__AResponseParser__
#define __CapcomWorld__AResponseParser__

#include <iostream>

#include "cocos2d.h"
#include <list>
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>

#include "ResponseLoginInfo.h"
#include "ResponseFriendsInfo.h"
#include "ResponseBgList.h"
#include "ResponseTeamlist.h"

class AResponseParser
{

private:
    AResponseParser(){
        
    }
    AResponseParser(const AResponseParser& other);
    ~AResponseParser() {}
    static AResponseParser* instance;
public:
    
    static AResponseParser *getInstance()
    {
        if (instance == 0) instance = new AResponseParser();
        return instance;
    }
    
    void popupNetworkError(const char* text1, const char* text2, const char* text3);
    void popupLoginError(const char* text1, const char* text2, const char* text3);
    
    bool responseBasic(const char* data);
    void parseBasic(xmlNode * node, ResponseBasic *_info);
    
    // upgrade
    bool responseUpgrade(const char *data);
    bool parseUpgrade(xmlNode * node, ResponseUpgradeInfo *info);
    
    
    // logion
    void responseLogin(const char *data);
    void parseLoginResponse(xmlNode * node, ResponseLoginInfo *loginInfo);
    void parseUser(xmlNode * node, ResponseLoginInfo *info);
    void parseCards(xmlNode * node, CCArray *cards);
    
    // friends
    void responseFriends(const char *data);
    void parseFriendsResponse(xmlNode * node, ResponseFriendsInfo *info);
    void parseFriend(xmlNode * node, CCArray *_friends);
    
    // bg list
    void responseBgList(const char* data);
    void parseBg(xmlNode * node, std::vector<int> *v);
    void parseBgList(xmlNode * node, ResponseBgList *_info);
    
    // select bg
    bool responseSelectBg(const char* data);
    void parseSelectBg(xmlNode * node, ResponseBasic *_info);
    
    // edit team
    
    // team list
    bool responseTeamlist(const char* data);
    void parseTeamlist(xmlNode * node, ResponseTeamlist *_info);
    void parseTeam(xmlNode * node, ResponseTeamlist *_info);
    
    // opponents
    bool responseOpponent(const char* data);
    void parseOpponent(xmlNode * node, ResponseOpponent *_info);
    void parseOpponentSub(xmlNode * node, ResponseOpponent *_info);
    
    // battle
    ResponseBattleInfo* responseBattle(const char* data);
    void parseBattle(xmlNode * node, ResponseBattleInfo *_info);
    void parseBattleSub(xmlNode * node, ResponseBattleInfo *_info);
    void parseSkill(xmlNode * node, CCArray *skills);
    
    // notice
    ResponseNoticeInfo* responseNotice(const char* data);
    ResponseNoticeInfo* parseNotice(xmlNode * node, ResponseNoticeInfo *_info);
    void parseNoticeSub(xmlNode * node, ResponseNoticeInfo *_info);
    
    //detail notice
    ResponseDetailNoticeInfo* responseDetailNotice(const char* data);
    void parseDetailNotice(xmlNode * node, ResponseDetailNoticeInfo *_info);
    void parseDetailNoticeSub(xmlNode * node, ResponseDetailNoticeInfo *_info);
    
    // quest list
    ResponseQuestListInfo* responseQuestList(const char* data);
    void parseQuestList(xmlNode * node, ResponseQuestListInfo *_info);
    void parseQuestListSub(xmlNode * node, ResponseQuestListInfo *_info);
    
    // quest (result) update
    ResponseQuestUpdateResultInfo* responseQuestResultUpdate(const char* data);
    void parseQuestResultUpdate(xmlNode * node, ResponseQuestUpdateResultInfo *_info);
    void parseQuestResultUpdateSub(xmlNode * node, ResponseQuestUpdateResultInfo *_info);
    
    //refresh
    ResponseRefreshInfo* responseRefresh(const char* data);
    void parseRefresh(xmlNode * node, ResponseRefreshInfo *_info);
    
    //collection
    ResponseCollectionInfo* responseCollection(const char* data);
    void parseCollection(xmlNode * node, ResponseCollectionInfo *_info);
    
    // fusion
    ResponseFusionInfo* responseFusion(const char* data);
    void parseFusion(xmlNode * node, ResponseFusionInfo *_info);
    
    //training
    ResponseTrainingInfo* responseTraining(const char* data);
    void parseTraining(xmlNode * node, ResponseTrainingInfo *_info);
    
    //item list
    ResponseItemInfo* responseItemList(const char* data);
    void parseItemList(xmlNode * node, ResponseItemInfo *_info);
    
    // gift list
    ResponseGiftInfo* responseGiftList(const char* data);
    void parseGiftList(xmlNode * node, ResponseGiftInfo *_info);
    
    // buy item
    ResponseBuyInfo* responseBuyItem(const char* data);
    void parseBuyItem(xmlNode * node, ResponseBuyInfo *_info);
    
    // use item
    ResponseUseInfo* responseUseItem(const char* data);
    void parseUseItem(xmlNode * node, ResponseUseInfo *_info);
    
    // sell card
    ResponseSellInfo* responseSell(const char* data);
    void parseSell(xmlNode * node, ResponseSellInfo *_info);
    
    ResponseTbInfo * responseTb(const char* data);
    void parseTb(xmlNode * node, ResponseTbInfo *_info);

    // tutorial
    ResponseTutorial* responseTutorial(const char* data);
    void parseTutorial(xmlNode * node, ResponseTutorial *_info);
    
    // medal
    ResponseMedal* responseMedal(const char* data);
    void parseMedal(xmlNode * node, ResponseMedal *_info);
    
    // roulette
    ResponseRoulette* responseRoulette(const char* data);
    void parseRoulette(xmlNode * node, ResponseRoulette *_info);
    
    // event
    ResponseEvent* responseEvent(const char* data);
    void parseEvent(xmlNode * node, ResponseEvent *_info);
    
    ResponseSMexclude* responseSMExclude(const char* data);
    void parseSMEX(xmlNode * node, ResponseSMexclude *_info);
    
    void readQuest();
    void parseQuestXML(xmlNode * node, CCArray *_questList);
    
    // trace
    ResponseTrace* responseTrace(const char* data);
    void parseTrace(xmlNode * node, ResponseTrace *_info);
    
    ResponseRivalBattle* responseRBattle(const char* data);
    void parseRBatle(xmlNode * node, ResponseRivalBattle *_info);
    void parseRound(xmlNode * node, ABattleRound *_round);
    
    ResponseRivalList* responseRivalList(const char* data);
    void parseRivalList(xmlNode * node, ResponseRivalList *_info);
    void parseRivalSub(xmlNode * node, AReceivedRival *rival_info);
    
    ResponseRivalReward* responseRivalReward(const char* data);
    void parseRivalReward(xmlNode * node, ResponseRivalReward *_info);
    
};

#endif /* defined(__CapcomWorld__AResponseParser__) */
