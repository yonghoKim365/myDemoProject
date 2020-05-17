//
//  ARequestSender.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#include "ARequestSender.h"
#include "PlayerInfo.h"
#include "AResponseParser.h"
#include "AKakaoUser.h"
#include "XBridge.h"
#include "md5.h"
using namespace cocos2d;
using namespace std;

ARequestSender *ARequestSender::instance = NULL;

const char *HTTP_getData(char *url);

void HTTP_getDataAsync(char *url, int reqType);
//void HTTP_getQuestData(char *url);
//void HTTP_getStageData(char *url);
//void HTTP_getFreindData(char *url);

std::string ARequestSender::getSigMD5(){
    
    PlayerInfo *playerInfo = PlayerInfo::getInstance();

    if (strlen(playerInfo->accessToken.c_str()) == 0){
        char cUserId[100];
        sprintf(cUserId, "%lld",playerInfo->userID);
        
        std::string sig = cUserId;
        sig.append( playerInfo->GetDeviceID()).append("ud;tkdak/fntahga'5hda");
        
        std::string sigMd5 = md5(sig);
        return sigMd5;
    
    }

    return playerInfo->accessToken;
    
}

//88265866873285105ud;tkdak/fntahga'5hda
//f71f309fe8036d51da380eb9373635f9

//88707934957560768ud;tkdak/fntahga'5hda
//804722526caa7b3a3074ce792e56ea22 - our sig, ok
// md5 gen sig =
//7486a55a2c017f4b7ff9d5004e3860de but, wrong sig

std::string ARequestSender::md5(const std::string strMd5){
    md5_state_t state;
    md5_byte_t digest[16];
    char hex_output[16*2 + 1];
    int di;
    
    md5_init(&state);
    md5_append(&state, (const md5_byte_t *)strMd5.c_str(), strMd5.length());
    md5_finish(&state, digest);
    
    for (di = 0; di < 16; ++di)
        sprintf(hex_output + di * 2, "%02x", digest[di]);
    
    return hex_output;
}

const char ConvertToHex( const char cSource )
{
    return "0123456789abcdef"[ 0x0f & cSource ];
}

const std::string URLEncoding( const std::string& kInput )
{
    std::string kOutput;
    
    std::string::const_iterator string_iter = kInput.begin();
    while( string_iter != kInput.end() )
    {
        const std::string::value_type element = (*string_iter);
        
        if( isascii( element )      // ASCII ?¬®??? ?¬•??
           && isalnum( element ) ) // ????¬®???, ?????¬∂¬®????¬¥??
        {
            kOutput += element;
        }
        else // ??????¬®???????¢¬?? %16???? ????¬∞?????
        {
            kOutput += "%";
            kOutput += ConvertToHex( element >> 4 );
            kOutput += ConvertToHex( element );
        }
        
        ++string_iter;
    }
    
    return kOutput;
}



const char* ARequestSender::getUserID(){
    char cUserId[100];
    sprintf(cUserId, "%lld",PlayerInfo::getInstance()->userID);
    return cUserId;
}
/*
void ARequestSender::requestRegisterToGameServer(){
    
    std::string url = "https://devfoc.apdgames.com/foc/infinite?cmd=register&userid=";
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    //pi->SetDeviceID(xb->getDeviceID());
    
    char cUserId[100];
    sprintf(cUserId, "%lld",pi->userID);
    char buf1[10];
    sprintf(buf1, "%d", pi->numOfKakaoFriends);
    char buf2[10];
    sprintf(buf2, "%d", pi->numOfKakaoAppFriends);
    url.append(cUserId).append("&deviceid=").append(pi->GetDeviceID()).append("&devicetype=i");
    url.append("&friends=").append(buf1).append("&appfriends=").append(buf2);
    url.append("&nick=").append(URLEncoding(pi->myNickname)).append("&sig="+getSigMD5()).append("&imgurl="+pi->profileImageUrl);
    
    
    CCLog("requestRegister, url:%s",url.c_str());
    
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    AResponseParser::getInstance()->responseLogin(data);
}

void ARequestSender::requestLoginToGameServer(){
    //CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
    
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    
    char cUserId[100];
    sprintf(cUserId, "%lld",playerInfo->userID);
    
    std::string sigMd5 = getSigMD5();
    
    std::string url = "https://devfoc.apdgames.com/foc/infinite?cmd=login&userid=";
    url.append(cUserId).append("&sig=").append(sigMd5).append("&imgurl=").append(playerInfo->profileImageUrl);

    CCLog("requestLogin, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    AResponseParser::getInstance()->responseLogin(data);
        
}
*/

void ARequestSender::requestRegister2(){
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    //pi->SetDeviceID(xb->getDeviceID());
    
    std::string svrUrl = serverURL;
    std::string cmd = "register2";
    
    char cUserId[100];
    sprintf(cUserId, "%lld",pi->userID);
    char buf1[10];
    sprintf(buf1, "%d", pi->numOfKakaoFriends);
    char buf2[10];
    sprintf(buf2, "%d", pi->numOfKakaoAppFriends);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(cUserId).append("&deviceid=").append(pi->GetDeviceID()).append("&devicetype=i");
    url.append("&friends=").append(buf1).append("&appfriends=").append(buf2);
    url.append("&nick=").append(URLEncoding(pi->myNickname)).append("&imgurl="+pi->profileImageUrl).append("&sig="+getSigMD5());
    
    CCLog("requestRegister2, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    pi = NULL;
    
    AResponseParser::getInstance()->responseLogin(data);
}

void ARequestSender::requestLogin2(){
    //CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
    
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    
    std::string svrUrl = serverURL;
    std::string cmd = "login2";
    
    char numOfAppFriend[100];
    sprintf(numOfAppFriend, "%d", playerInfo->numOfKakaoAppFriends);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&imgurl=");
    url.append(playerInfo->profileImageUrl).append("&appfriends=").append(numOfAppFriend).append("&nick=").append(URLEncoding(playerInfo->myNickname));
    
    CCLog("requestLogin2, url:%s",url.c_str());
        
    const char* data=HTTP_getData((char*)url.c_str());
    
    playerInfo = NULL;
    
    AResponseParser::getInstance()->responseLogin(data);
    
}




void ARequestSender::requestFriendsToGameServer(){
    
    std::string svrUrl = serverURL;
    std::string cmd = "friends";
    std::string url = svrUrl.append(cmd).append("&friends=");
    
    int count = PlayerInfo::getInstance()->kakaoAppFriendsInfo->count();
    if (count >= 100)
        count = 99;
    for (int i=0;i<count;i++){
        AKakaoUser *user = (AKakaoUser*)PlayerInfo::getInstance()->kakaoAppFriendsInfo->objectAtIndex(i);
        char buf[100];
        sprintf(buf, "%lld",user->userID);
        if (i!=0)url.append(",");
        
        url.append(buf);
    }
    
    char cUserId[100];
    sprintf(cUserId, "%lld",PlayerInfo::getInstance()->userID);
    
    
    
    
    url.append("&sig=").append(getSigMD5()).append("&userid=").append(cUserId);
    
    CCLog("requestFriends, url:%s",url.c_str());
    
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_FRIEND_LIST);
    //HTTP_getFreindData((char*)url.c_str());
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
    {
        popupOk("Ïπ?µ¨ Î™©Î???Î∂???§Î????§Ì???????. \n?????? ?§Ï? ????¥Ï£º?∏Ï?.");
        return;
    }
    
    AResponseParser::getInstance()->responseFriends(data);
     */
}



void ARequestSender::requestBgList(){
    // ??????§Œ???¢¬©√??????? ?¬µ???? ?¬©?? ??¬©¬•??? ??????§Œ?????§Œ??????????§Œ©√?
    //?cmd=bglist&userid=xxx&sig=xxxx
    std::string svrUrl = serverURL;
    std::string cmd = "bglist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestBgList, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_BG_LIST);
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
    {
        popupOk("Î∞∞Í≤Ω Î™©Î???Î∂???§Î????§Ì???????. \n?????? ?§Ï? ????¥Ï£º?∏Ï?.");
        return;
    }

    AResponseParser::getInstance()->responseBgList(data);
    */
}

bool ARequestSender::requestSelectBg(int bgId){
    // ??????§Œ?????§Œ???¬©¬•??? ????????§Œ???????? ??- ?¬µ¬®??
    //?cmd=selectbg&userid=xxx&bgid=xxx&sig=yyy
    
    //CCLog(" serverURL =%s", serverURL.c_str());
    std::string svrUrl = serverURL;
    std::string cmd = "selectbg";
    
    char cbgID[10];
    sprintf(cbgID, "%d",bgId);
        
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&bgid=").append(cbgID).append("&sig=").append(getSigMD5());
    
    CCLog("requestSelectBg, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    return AResponseParser::getInstance()->responseSelectBg(data);
    
}




bool ARequestSender::requestEditTeam(int teamType, int teamIdx, int *cardList)
{
    //?cmd=editteam&userid=xxx&type=x&idx=x&deck=xx,xx,xx,xx,xx&sig=xxx
    std::string svrUrl = serverURL;
    std::string cmd = "editteam";
    
    std::string cTeamType = "a";
    if (teamType == 1)cTeamType = "b";
    
    char cTeamIndex[10];
    sprintf(cTeamIndex, "%d",teamIdx);
    
    //std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&type=").append(cTeamType);
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&idx=").append(cTeamIndex).append("&deck=");
    for(int i=0;i<5;i++){
        char cCardSrl[10];
        
        sprintf(cCardSrl, "%d",cardList[i]);
        
        if (i!=0)url.append(",");
        
        url.append(cCardSrl);
    }
    url.append("&sig=").append(getSigMD5());
    
    CCLog("requestEditTeam, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
    {
        popupOk("?????? ?∞Í≤∞???????? ??????. \n ?????? ?§Ï? ?????Ï£ºÏ???");
        return false;
    }
    else
    {
        return AResponseParser::getInstance()->responseBasic(data);
    }
}

bool ARequestSender::requestTeamlist()
{
    std::string svrUrl = serverURL;
    std::string cmd = "teamlist";
        
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestTeamlist, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    //const char* data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><teams><team idx=\"0\" deck=\"0,0,24583,0,0\"></team><team idx=\"4\" deck=\"24586,24584,24583,24582,24585\"></team></teams></response>";
    
    return AResponseParser::getInstance()->responseTeamlist(data);
}


bool ARequestSender::requestUpgrade(int _addAttack, int _addDefense, int _addQuest)
{
    std::string svrUrl = serverURL;
    std::string cmd = "upgrade";
    
    char cAttack[10];
    sprintf(cAttack, "%d", _addAttack);
    char cDefense[10];
    sprintf(cDefense, "%d", 0);//_addDefense);
    char cQuest[10];
    sprintf(cQuest, "%d", _addQuest);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID());
    url.append("&attack=").append(cAttack).append("&defense=").append(cDefense).append("&quest=").append(cQuest).append("&sig=").append(getSigMD5());
    
    CCLog("requestUpgrade, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    return AResponseParser::getInstance()->responseUpgrade(data);
}

bool ARequestSender::requestOpponent()
{
    
    //?cmd=opponents&userid=xxx&sig=xxx
    std::string svrUrl = serverURL;
    std::string cmd = "opponents";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestOpponent, url:%s",url.c_str());
    
    //const char* data=HTTP_getData((char*)url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_BATTLE_LIST);
    /*
    if(NULL == data)
        return false;
    else
        return AResponseParser::getInstance()->responseOpponent(data);
     */
}


ResponseBattleInfo* ARequestSender::requestBattle(int _teamIdx, long long userID)
{
    //?cmd=battle&userid=xx&team=x&opponent=xx&sig=xxx
    std::string svrUrl = serverURL;
    std::string cmd = "battle";
    
    char buf1[10];
    sprintf(buf1, "%d", _teamIdx);
    
    char buf2[20];
    sprintf(buf2, "%lld", userID);
        
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&team=").append(buf1);
    url.append("&opponent=").append(buf2).append("&sig=").append(getSigMD5());
    
    CCLog("requestBattle, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseBattle(data);
    /*
    const char* testData = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><battle><res>win</res><reward coin=\"100\" fame=\"5\"></reward><used_attack_pnt>28</used_attack_pnt><opponent_cards><card>40021</card><card>50021</card><card>30031</card><card>40011</card><card>30021</card></opponent_cards><opponent_stat><nick>test3</nick><lev>1</lev><app_friends>0</app_friends><defense_pnt>300</defense_pnt><battle_cnt>22</battle_cnt><victory_cnt>0</victory_cnt><draw_cnt>0</draw_cnt></opponent_stat><skills><skill side=\"attacker\" card=\"30191\" skill=\"1\"></skill></skills></battle></response>";
    
    return AResponseParser::getInstance()->responseBattle(testData);
    */

}

ResponseNoticeInfo* ARequestSender::requestNotice()
{
    std::string svrUrl = serverURL;
    std::string cmd = "notice";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestNotice, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_BATTLE_LOG);
    
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseNotice(data);
     */
    
    //const char* testData = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><notices><notice type=\"battle\" id=\"2\"><res>lose</res><nick>iphone 5</nick><date>1353555355</date></notice><notice type=\"battle\" id=\"1\"><res>lose</res><nick>iphone 5</nick><date>1353555204</date></notice></notices></response>";
    
    /*
    const char* testData = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><notices><notice type=\"battle\" id=\"58\"><res>lose</res><nick>noname</nick><date>1353590985</date></notice><notice type=\"battle\" id=\"56\"><res>lose</res><nick>noname</nick><date>1353581124</date></notice><notice type=\"battle\" id=\"52\"><res>lose</res><nick>noname</nick><date>1353570807</date></notice><notice type=\"battle\" id=\"51\"><res>lose</res><nick>noname</nick><date>1353570793</date></notice><notice type=\"battle\" id=\"34\"><res>lose</res><nick>noname</nick><date>1353561458</date></notice></notices></response>";
    
    return AResponseParser::getInstance()->responseNotice(testData);
    */
}

ResponseDetailNoticeInfo* ARequestSender::requestDetailNotice(int noticeID)
{
    std::string svrUrl = serverURL;
    std::string cmd = "detailnotice";
    
    char buf1[10];
    sprintf(buf1, "%d", noticeID);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&nid=").append(buf1).append("&sig=").append(getSigMD5());
    
    CCLog("requestDetailNotice, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
    {
        popupOk("?∏Î???≥¥Î•?Î∂???§Î????§Ì???????. \n?????? ?§Ï? ????¥Ï£º?∏Ï?");
        return NULL;
    }
    else
        return AResponseParser::getInstance()->responseDetailNotice(data);
    
}

ResponseQuestListInfo* ARequestSender::requestQuestList()
{
    
    //?cmd=questlist&userid=000&sig=000
    
    std::string svrUrl = serverURL;
    std::string cmd = "questlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestQuestList, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
    {
        popupOk("?????Î™©Î???Î∂???§Î????§Ì???????. \n?????? ?§Ï? ????¥Ï£º?∏Ï?");
        return NULL;
    }
    else
        return AResponseParser::getInstance()->responseQuestList(data);
 
}

ResponseQuestListInfo* ARequestSender::requestChapterList()
{
    std::string svrUrl = serverURL;
    std::string cmd = "questlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestChapterList, url:%s",url.c_str());
    
    //HTTP_getQuestData((char*)url.c_str());
    HTTP_getDataAsync((char*)url.c_str(), REQ_CHAPTER_LIST);
}

ResponseQuestListInfo* ARequestSender::requestStageList()
{
    std::string svrUrl = serverURL;
    std::string cmd = "questlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestStageList, url:%s",url.c_str());
    
    //HTTP_getStageData((char*)url.c_str());
    HTTP_getDataAsync((char*)url.c_str(), REQ_STAGE_LIST);
}






ResponseRefreshInfo* ARequestSender::requestRefresh()
{
    std::string svrUrl = serverURL;
    std::string cmd = "refresh";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestRefresh, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseRefresh(data);
}


ResponseCollectionInfo* ARequestSender::requestCollection()
{
    // ?cmd=collection&userid=00&sig=00
    
    std::string svrUrl = serverURL;
    std::string cmd = "collection";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestCollection, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_COLLECT);
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseCollection(data);
     */
}

ResponseFusionInfo* ARequestSender::requestFusion(int targetSrl, int sourceSrl)
{
    // slot?????(?¬ß??¢‚???? ??¬•??????????¬•???????
    std::string svrUrl = serverURL;
    std::string cmd = "fusion";
    
    char buf1[10];
    sprintf(buf1, "%d", targetSrl);
    
    char buf2[10];
    sprintf(buf2, "%d", sourceSrl);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&source=").append(buf2).append("&target=").append(buf1);
    
    CCLog("requestFusion, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_FUSION);
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseFusion(data);
     */

}

ResponseTrainingInfo* ARequestSender::requestTraining(int targetSrl, int sourceSrl)
{
    // slot?????(?¬ß??¢‚???? ??¬•??????????¬•???????
    std::string svrUrl = serverURL;
    std::string cmd = "training";
    
    char buf1[10];
    sprintf(buf1, "%d", targetSrl);
    
    char buf2[10];
    sprintf(buf2, "%d", sourceSrl);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&source=").append(buf2).append("&target=").append(buf1);
    
    CCLog("requestTraining, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_TRAINING);

    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseTraining(data);
     */
    
}

void ARequestSender::requestItemListAsync()
{
    // ?cmd=collection&userid=00&sig=00
    
    std::string svrUrl = serverURL;
    std::string cmd = "itemlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestItemListAsync, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_ITEM_LIST);
}

ResponseItemInfo* ARequestSender::requestItemList()
{
    // ?cmd=collection&userid=00&sig=00
    
    std::string svrUrl = serverURL;
    std::string cmd = "itemlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestItemList, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseItemList(data);
     
}

ResponseGiftInfo* ARequestSender::requestGiftList()
{
    // ?cmd=collection&userid=00&sig=00
    
    std::string svrUrl = serverURL;
    std::string cmd = "giftlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
//    CCLog("requestGiftList, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseGiftList(data);
}

void ARequestSender::requestGiftListAsync()
{
    std::string svrUrl = serverURL;
    std::string cmd = "giftlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    //    CCLog("requestGiftList, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_GIFT_LIST);
}

ResponseBuyInfo* ARequestSender::requestBuyItem(int itemID)
{
    std::string svrUrl = serverURL;
    std::string cmd = "buy";
    
    char buf1[10];
    sprintf(buf1, "%d", itemID);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&itemid=").append(buf1);
    
    CCLog("requestGiftList, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseBuyItem(data);
}

void ARequestSender::requestGiftReceive(int itemSrlID)
{
    std::string svrUrl = serverURL;
    std::string cmd = "recv";
    
    char buf1[10];
    sprintf(buf1, "%d", itemSrlID);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&srl=").append(buf1);
    
    CCLog("requestGiftReceive, url:%s",url.c_str());
    
    HTTP_getData((char*)url.c_str());
}

ResponseUseInfo* ARequestSender::requestUseItem(int itemID)
{
    std::string svrUrl = serverURL;
    std::string cmd = "use";
    
    char buf1[10];
    sprintf(buf1, "%d", itemID);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&itemid=").append(buf1);
    
    CCLog("requestUseItem, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());

    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseUseItem(data);
}

ResponseSellInfo* ARequestSender::requestSellCard(int cardSrl)
{
    std::string svrUrl = serverURL;
    std::string cmd = "sell";
    
    char buf1[10];
    sprintf(buf1, "%d", cardSrl);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&srl=").append(buf1);
    
    CCLog("requestSellCard, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseSell(data);
}

ResponseTbInfo* ARequestSender::requestTb(int tbid)
{
    std::string svrUrl = serverURL;
    std::string cmd = "tb";
    
    char buf1[10];
    sprintf(buf1, "%d", tbid);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&tbid=").append(buf1);
    
    CCLog("requestTb, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseTb(data);
}

ResponseTutorial* ARequestSender::requestTutorialProgress(int progress)
{
    std::string svrUrl = serverURL;
    std::string cmd = "tutorial";
    
    char buf1[10];
    sprintf(buf1, "%d", progress);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&tp=").append(buf1);

    CCLog("requestTutorialProgress, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseTutorial(data);;
}

bool ARequestSender::requestSendMedal(long long userID)
{
    std::string svrUrl = serverURL;
    std::string cmd = "sm";
    
    char buf1[20];
    sprintf(buf1, "%lld", userID);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&to=").append(buf1);
    
    CCLog("requestSendMedal, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return false;
    else
        return AResponseParser::getInstance()->responseBasic(data);
}

ResponseMedal* ARequestSender::requestMedalCount()
{
    std::string svrUrl = serverURL;
    std::string cmd = "mcount";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestMedalCount, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_MEDAL_COUNT);
    
    /*
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseMedal(data);
     */
}

ResponseRoulette* ARequestSender::requestRoulette(const std::string& symbol, int _match)
{
    std::string svrUrl = serverURL;
    std::string cmd = "roulette";
    
    char match[10];
    sprintf(match, "%d", _match);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&symbol=").append(symbol).append("&match=").append(match);
    
    CCLog("requestRoulette, url:%s",url.c_str());
    
    HTTP_getDataAsync((char*)url.c_str(), REQ_ROULETTE);
    /*
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseRoulette(data);
     */
}

ResponseEvent* ARequestSender::requestEvent()
{
    std::string svrUrl = serverURL;
    std::string cmd = "eventlist";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestEvent, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());

    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseEvent(data);
}


ResponseSMexclude* ARequestSender::requestSMExclude()
{
    std::string svrUrl = serverURL;
    std::string cmd = "smexclude";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestSMExclude, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseSMExclude(data);
    
}

void ARequestSender::retry(char *url, int reqType)
{
    HTTP_getDataAsync(url, reqType);
}

/*
ResponseTrace* ARequestSender::requestTrace()
{
    std::string svrUrl = serverURL;
    std::string cmd = "trace";
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5());
    
    CCLog("requestTrace, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseTrace(data);
    
    
}
*/

ResponseQuestUpdateResultInfo* ARequestSender::requestUpdateQuestResult(int questID, int action, int nTeam, bool bWin)
{
    //?cmd=quest?userid=00&qid=00&sig=000
    
    std::string svrUrl = serverURL;
    std::string cmd = "quest";
    
    char buf1[10];
    sprintf(buf1, "%d", questID);
    
    char buf2[1];
    sprintf(buf2, "%d", nTeam);
    
    std::string actionStr = "trace";
    if (action == 1 || action == 3)actionStr = "battle";
    else if (action == 2)actionStr = "avoid";
    
    std::string p1 = "&p1=";
    if (action == 1){
        if (bWin)p1.append("win");
        else p1.append("lose");
    }
    else if (action == 3){
        p1.append("win");
    }
    else{
        p1.append("na");
    }
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&qid=").append(buf1).append("&sig=").append(getSigMD5()).append("&action=").append(actionStr);
    //url.append("&team=").append(buf2).append(p1);
    url.append(p1);
    
    CCLog("requestUpdateQuestResult, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());

    if(NULL == data)
        return NULL;
    else
    {
        ResponseQuestUpdateResultInfo* info = AResponseParser::getInstance()->responseQuestResultUpdate(data);
        
        if ((info->res) == 0){
            //if (info->progress == info->progressMax){
            if (info->progress >= 100){
                for(int i=0;i<PlayerInfo::getInstance()->questList->count()-1;i++){
                    QuestInfo* questInfo = (QuestInfo*)PlayerInfo::getInstance()->questList->objectAtIndex(i);
                    if (questInfo->questID == questID){
                        QuestInfo* nextQuestInfo = (QuestInfo*)PlayerInfo::getInstance()->questList->objectAtIndex(i+1);
                        nextQuestInfo->lockState = 1; // unlock next quest
                        break;
                    }
                }
            }
        }
        else{
            
        }
        
        return info;
    }
    
}



ResponseRivalBattle* ARequestSender::requestRBattle(int rid, int nTeam)
{
    std::string svrUrl = serverURL;
    std::string cmd = "rbattle";
    
    char buf[20];
    sprintf(buf, "%d", rid);
    
    char buf2[1];
    sprintf(buf2, "%d", nTeam);
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&rid=").append(buf).append(("&team=")).append(buf2);
    
    CCLog("requestRBattle, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    // for test
    
     // ?ºÏ?Î≤?∞∞???®Î∞∞
    /*
     const char* data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><rbattle><res>lose</res><reward coin=\"1313\" exp=\"0\"><cards></cards></reward><used_attack_pnt>20</used_attack_pnt><rival_stat><hp max=\"3060\">2901</hp></rival_stat><skills></skills><powers><attacker point=\"159\" ext=\"0\" friends=\"0\"></attacker><defender point=\"74\" ext=\"0\" friends=\"0\"></defender></powers></rbattle></response>";
    */
    
    /*
     // Î∞∞Ì??¥Ï? ?¥Í?
     const char* data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><rbattle><res>win</res><reward coin=\"1144\" exp=\"0\"><cards></cards></reward><used_attack_pnt>20</used_attack_pnt><rival_stat><hp max=\"3060\">0</hp></rival_stat><skills></skills><powers><attacker point=\"159\" ext=\"0\" friends=\"0\"></attacker><defender point=\"74\" ext=\"0\" friends=\"0\"></defender></powers></rbattle></response>";
    */
    
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseRBattle(data);
    
}



ResponseRivalList* ARequestSender::requestRivalList()
{
    std::string svrUrl = serverURL;
    std::string cmd = "rivallist";
        
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&friends=");

    if (PlayerInfo::getInstance()->kakaoAppFriendsInfo->count()>0){
        //url.append("&friends=");
        
        for(int i=0;i<PlayerInfo::getInstance()->kakaoAppFriendsInfo->count();i++){
            AKakaoUser *user = (AKakaoUser*)PlayerInfo::getInstance()->kakaoAppFriendsInfo->objectAtIndex(i);
            
            if (i>0)url.append(",");
            
            char cUserId[100];
            sprintf(cUserId, "%lld",user->userID);
            
            url.append(cUserId);
            
            //if (i>10)break;
        }   
    }
    
    CCLog("requestRivalList, url:%s",url.c_str());
    
    // for test
    const char* data=HTTP_getData((char*)url.c_str());
    /*
    const char* data =
    "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><rivals><rival type=\"rival\" max_hp=\"5050\" cur_hp=\"5050\" lv=\"1\" nid=\"2022\" qst_id=\"20031\" rid=\"3040\" birth=\"1363593241\" limit=\"1363595041\" owner=\"724070835002638447\" reward=\"n\"><colleages><colleage damages=\"0\" userid=\"724070835002638447\" leader_card_id=\"0\" leader_card_lv=\"0\"></colleage></colleages></rival></rivals></response>";
    */
    // 1361006944
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseRivalList(data);
    
    
}

ResponseRivalReward* ARequestSender::requestRivalReward(int rid)
{
    std::string svrUrl = serverURL;
    std::string cmd = "rreward";
    
    char buf[20];
    if (rid == -1){
        sprintf(buf, "%s", "all");
    }
    else{
        sprintf(buf, "%d", rid);
    }
    
    std::string url = svrUrl.append(cmd).append("&userid=").append(getUserID()).append("&sig=").append(getSigMD5()).append("&rid=").append(buf);
    
    CCLog("requestRivalReward, url:%s",url.c_str());
    
    const char* data=HTTP_getData((char*)url.c_str());
    
    if(NULL == data)
        return NULL;
    else
        return AResponseParser::getInstance()->responseRivalReward(data);
    
}

//712070835002638447


//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=791070835002638447

//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=752123642632497918 // touch

//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88265497908068977 /// ipad

// unregister
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88707934957560768
// iphone 5
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88610764144128545
// simulator
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88623572820391280
// 4s
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88623212837755537
// ricky iphone 4
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88610764144063049

//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88587733986316256 // touch 4g
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88707934957560768
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88610764144128545 // iphone5

//https://devfoc.apdgames.com/foc/infinite?cmd=testdelay&userid=776123642632497918&delay=5000

//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88265497908068977
//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88306183038747505
// apd Íπ????//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88587733986316256

//https://devfoc.apdgames.com/foc/infinite?cmd=unregister&userid=88306183038747505
/*
////////////////////////////////
// ????¨Î? Ïª¥Îß®??? ??¶¨
 
////////////////////////////////
// Î≥¥Ï? ?±Ï???progressÍ∞?0??????? 
////////////////////////////////
// ????∏Î? ?????? Î≥¥Ï?Î•?Ï£ΩÏ?Í≥?progressÍ∞?100?¥Î? end Í∞?0??   ?¥Ì? progressÍ∞?100??Ï¥?≥º??©¥ clearÍ∞?1????????.
 
*/

