//
//  ARequestSender.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#ifndef __CapcomWorld__ARequestSender__
#define __CapcomWorld__ARequestSender__

#include <iostream>
#include "XBridge.h"
#include "ResponseBasic.h"
#include "MyUtil.h"

using namespace std;
using namespace cocos2d;

enum requestType
{
    REQ_BATTLE_LIST = 0,
    REQ_CHAPTER_LIST,
    REQ_STAGE_LIST,
    REQ_FRIEND_LIST,
    REQ_ITEM_LIST,
    REQ_GIFT_LIST,
    REQ_FUSION,
    REQ_COLLECT,
    REQ_MEDAL_COUNT,
    REQ_BATTLE_LOG,
    REQ_TRAINING,
    REQ_BG_LIST,
    REQ_ROULETTE,
};

class ARequestSender : public MyUtil
{
    
private:
    ARequestSender(){
        
        serverURL = FOC_GAME_SERV_URL;
                
        //xb = new XBridge();
    }
    ARequestSender(const ARequestSender& other);
    ~ARequestSender() {}
    static ARequestSender* instance;
    
    //XBridge *xb;
    
    std::string serverURL;
    
public:
    
    static ARequestSender *getInstance()
    {
        if (instance == 0) instance = new ARequestSender();
        return instance;
    }

    std::string getSigMD5();
    std::string md5(const std::string strMd5);
    const char* getUserID();
    
    //void requestRegisterToGameServer();
    //void requestLoginToGameServer();
    void requestRegister2();
    void requestLogin2();
    void requestFriendsToGameServer();
    
    // ??????????∑∏??????
    bool requestUpgrade(int _addAttack, int _addDefense, int _addQuest);
    void requestBgList();
    // Î∞∞Í≤Ω Î≥?≤Ω ??©¥??? ??Î∞∞Í≤Ω?????
    bool requestSelectBg(int bgId);
    // ??Ïπ¥Î??? ?§Ï???Î∞?æº??
    bool requestEditTeam(int teamType, int teamIdx, int* cardlist);
    bool requestTeamlist();
    // ?????? Î™©Î?????≤≠???. 
    bool requestOpponent();
    // battle
    ResponseBattleInfo* requestBattle(int _teamIdx, long long userID);
    // notice
    ResponseNoticeInfo* requestNotice();
    // notice detail
    ResponseDetailNoticeInfo* requestDetailNotice(int noticeID);
    // quest list
    ResponseQuestListInfo* requestQuestList();
    // chapter list
    ResponseQuestListInfo* requestChapterList();
    // stage list
    ResponseQuestListInfo* requestStageList();
    // quest
    ResponseQuestUpdateResultInfo* requestUpdateQuestResult(int questID, int action, int nTeam, bool bWin);
    // refresh
    ResponseRefreshInfo* requestRefresh();
    // card collection
    ResponseCollectionInfo* requestCollection();
    // fusion
    ResponseFusionInfo* requestFusion(int targetSrl, int sourceSrl);
    //training
    ResponseTrainingInfo* requestTraining(int targetSrl, int sourceSrl);
    // item list
    ResponseItemInfo* requestItemList();
    void requestItemListAsync();
    // gift list
    ResponseGiftInfo* requestGiftList();
    void requestGiftListAsync();
    // buy
    ResponseBuyInfo* requestBuyItem(int itemID);
    // receive
    void requestGiftReceive(int itemSrlID);
    // use
    ResponseUseInfo* requestUseItem(int itemID);
    // sell
    ResponseSellInfo* requestSellCard(int cardSrl);
    // tb treasure box
    ResponseTbInfo* requestTb(int tbid);
    // tutorial
    ResponseTutorial* requestTutorialProgress(int progress);
    // send medal
    bool requestSendMedal(long long userID);
    
    // medal count
    ResponseMedal* requestMedalCount();
    // roulette
    ResponseRoulette* requestRoulette(const std::string& symbol, int _match);
    // event
    ResponseEvent* requestEvent();
    
    ResponseSMexclude* requestSMExclude();
    
    ResponseRivalBattle* requestRBattle(int rid, int nTeam);
    
    ResponseRivalList* requestRivalList();
    
    ResponseRivalReward* requestRivalReward(int rid);
    
    
    
    void retry(char *url, int reqType);
};
#endif /* defined(__CapcomWorld__ARequestSender__) */
