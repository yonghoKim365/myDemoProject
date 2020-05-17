//
//  AResponseParser.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#include "AResponseParser.h"
#include "PlayerInfo.h"
#include "CardInfo.h"
#include "UserStatLayer.h"
#include "DojoLayerDojo.h"
#include "MainScene.h"
#include "ARequestSender.h"
#include "ResponseBgList.h"
#include "TitleScene.h"
#include "KakaoLoginScene.h"
#include "PopupLayer.h"
#include "ItemInfo.h"
#include "GiftInfo.h"
#include "EventInfo.h"

AResponseParser *AResponseParser::instance = NULL;

bool AResponseParser::responseBasic(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    ResponseBasic *responseInfo = new ResponseBasic();
    
    CCLog("responseBasic : %s", data);
    
    parseBasic(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        // ok
        delete responseInfo;
        return true;
    }
    else{
        // error
        
        popupNetworkError(responseInfo->res,responseInfo->msg, NULL);
        
        delete responseInfo;
        return false;
    }
}

void AResponseParser::parseBasic(xmlNode * node, ResponseBasic *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBasic *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseBgList currentNode->name :%s", currentNode->name);
        
        //bool bSKipChild = false;
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseSelectBg(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
                CCLog(" info->res :%s",info->res);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
                CCLog(" info->msg :%s",info->msg);
            }
        }
    }
}

void AResponseParser::popupNetworkError(const char* text1, const char* text2, const char* text3)
{
    PopupLayer *popup = new PopupLayer();
    //_layer->addChild(popup,1000);
    
    
    popup->setText(text1, text2, text3);
    
    if (MainScene::getInstance()!=NULL){
        MainScene::getInstance()->addChild(popup,1000);
    }
    else{
        if (TitleScene::getInstance()){
            TitleScene::getInstance()->addChild(popup,1000);
        }
    }
    
    
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
}

void AResponseParser::popupLoginError(const char* text1, const char* text2, const char* text3)
{
    PopupLayer *popup = new PopupLayer();
    
    popup->nCallFrom = 1;
    
    popup->setText(text1, text2, text3);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    KakaoLoginScene::getInstance()->addChild(popup,1000);
}

void AResponseParser::responseLogin(const char* data)
{
    //PlayerInfo *playerInfo = PlayerInfo::getInstance();
    
    //CCLog("responseLogin : %s", data);
    CCLog("responseLogin : /////////////////");//%s", data);
    
    if (data == NULL){
        popupLoginError("네트워크 문제로 인해 로그인이","실패하였습니다.", "잠시후 다시 시도해 주세요");
        //popupNetworkError("Login Error","Login Error", "responseLogin ERROR");
        return;
    }
    
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    ResponseLoginInfo *loginResponse = new ResponseLoginInfo();
    
    parseLoginResponse(root_element, loginResponse);
    
    //loginResponse->res = "1010";
    
    CCLog("responseLogin : res:%s ", loginResponse->res);
    
    if (atoi(loginResponse->res) == 0){
        
        PlayerInfo::getInstance()->friends_bonus = loginResponse->friends_bonus;
        
        PlayerInfo::getInstance()->setUserInfo(loginResponse);
        
        PlayerInfo::getInstance()->SetCardsFromLoginResponse(loginResponse->cardList);
        
        if (ARequestSender::getInstance()->requestTeamlist()){
            //TitleScene::getInstance()->switchMainScene();
            //KakaoLoginScene::getInstance()->switchMainScene();
            if (KakaoLoginScene::getInstance() == NULL){
                //로그인된 상태라면 이쪽으로 온다.
                TitleScene::getInstance()->switchMainScene();
            }
            else{
                // 게스트이거나 방금 로그인했으면 이쪽으로.
                KakaoLoginScene::getInstance()->switchMainScene();
            }
        }
    }
    else if (atoi(loginResponse->res) == 2001){
        CCLog(" unregister user, need register");
        PlayerInfo::getInstance()->setBgmOption(true);
        PlayerInfo::getInstance()->setSoundEffectOption(true);
        //ARequestSender::getInstance()->requestRegisterToGameServer();
        ARequestSender::getInstance()->requestRegister2();
    }
    else{
        CCLog(" responseLogin ERROR");
        popupNetworkError(loginResponse->res,loginResponse->msg, "responseLogin ERROR");
    }
    
    delete loginResponse;
}


void AResponseParser::parseLoginResponse(xmlNode * node, ResponseLoginInfo *loginInfo){
    xmlNode *currentNode = NULL;
    ResponseLoginInfo *info = loginInfo;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseLoginResponse currentNode->name :%s", currentNode->name);
        
        bool bSKipChild = false;
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child)
                info->res = (const char*)child->content;
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child)
                info->msg = (const char*)child->content;
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "char") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                parseUser(child, info);
                bSKipChild = true;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "cards") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                parseCards(child, info->cardList);
                bSKipChild = true;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "friends_bonus") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->friends_bonus = atoi((const char*)child->content);
            }
        }
        if (bSKipChild==false){
            parseLoginResponse(currentNode->children, info);
        }
    }
}


void AResponseParser::parseUser(xmlNode * node, ResponseLoginInfo *info){
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE){
            xmlNode *child = currentNode->children;
            
            //CCLog("currentNode->name:%s",currentNode->name);
            //CCLog("child->name:%s",child->name);
            //CCLog("child->content:%s", child->content);
            
            if (strcmp((const char*)currentNode->name, "lev") == 0){
                info->setLevel(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "exp") == 0){
                info->setXp(atoi((const char*)child->content));
                xmlAttr *xa = currentNode->properties;
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max") == 0){
                    info->setMaxXp(atoi((const char*)xa->children->content));
                }
            }
            else if (strcmp((const char*)currentNode->name, "coin") == 0){
                info->setCoin(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "gold") == 0){
                info->setCash(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "fame") == 0){
                info->setFame(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "quest_pnt") == 0){
                info->setQuestPoint(atoi((const char*)child->content));
                xmlAttr *xa = currentNode->properties;
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max") == 0){
                    info->setMaxQuestPoint(atoi((const char*)xa->children->content));
                }
            }
            else if (strcmp((const char*)currentNode->name, "attack_pnt") == 0){
                info->setAttackPoint(atoi((const char*)child->content));
                xmlAttr *xa = currentNode->properties;
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max") == 0){
                    info->setMaxAttackPoint(atoi((const char*)xa->children->content));
                }
            }
            else if (strcmp((const char*)currentNode->name, "defense_pnt") == 0){
                info->setDefensePoint(atoi((const char*)child->content));
                xmlAttr *xa = currentNode->properties;
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max") == 0){
                    info->setMaxDefensePoint(atoi((const char*)xa->children->content));
                }
            }
            else if (strcmp((const char*)currentNode->name, "upgrade_pnt") == 0){
                info->setUpgradePoint(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "background") == 0){
                info->setBackground(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "revenge") == 0){
                info->setRevengePoint(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "tutorial_progress") == 0){
                info->setTutorial(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "battle_cnt") == 0){
                info->setBattleCnt(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "victory_cnt") == 0){
                info->setVictoryCnt(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "draw_cnt") == 0){
                info->setDrawCnt(atoi((const char*)child->content));
            }
            else if (strcmp((const char*)currentNode->name, "ranking") == 0){
                info->setDrawCnt(atoi((const char*)child->content));
            }
            
            /*
             else if (strcmp((const char*)currentNode->name, "max_quest_pnt") == 0){
             info->setMaxQuestPoint(atoi((const char*)child->content));
             }
             else if (strcmp((const char*)currentNode->name, "max_attack_pnt") == 0){
             info->setMaxAttackPoint(atoi((const char*)child->content));
             }
             else if (strcmp((const char*)currentNode->name, "max_defense_pnt") == 0){
             info->setMaxDefensePoint(atoi((const char*)child->content));
             }
             */
        }
    }
}

void AResponseParser::parseCards(xmlNode * node, CCArray *cards){
    
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("2. parseCards. currentNode->name :%s", currentNode->name);
        
        if (currentNode->type == XML_ELEMENT_NODE){
            //xmlNode *child = currentNode->children;
            if (strcmp((const char*)currentNode->name, "card") == 0){
                CardInfo *card = new CardInfo();
                card->autorelease();
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                        if (xa->children){
                            card->setSrl(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                        if (xa->children){
                            card->setId(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                        if (xa->children){
                            card->setLevel(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                        if (xa->children){
                            card->setExp(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                        if (xa->children){
                            card->setAttack(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                        if (xa->children){
                            card->setDefence(atoi((const char*)xa->children->content));
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                        if (xa->children){
                            card->setSkillEffect(atoi((const char*)xa->children->content));
                        }
                    }
                }
                //CCLog("parse card, id:%d srl:%d lev:%d atk:%d def:%d skill:%d", card->getId(), card->getSrl(), card->getLevel(), card->getAttack(), card->getDefence(), card->getSkillEffect());
                cards->addObject(card);
            }
        }
    }
}


void AResponseParser::responseFriends(const char *data)
{
    // for test
    /*
    const char* data2 ="<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><ranking>58</ranking><friends><friend userid=\"88317156972820625\" lev=\"11\" ranking=\"37\" leadercard=\"0\" attack=\"0\" defense=\"0\"></friend><friend userid=\"88330866058245825\" lev=\"4\" ranking=\"54\" leadercard=\"0\" attack=\"0\" defense=\"0\"></friend><friend userid=\"88587733986316256\" lev=\"21\" ranking=\"36\" leadercard=\"0\" attack=\"0\" defense=\"0\"></friend><friend userid=\"88623212837755537\" lev=\"1\" ranking=\"40\" leadercard=\"0\" attack=\"0\" defense=\"0\"></friend></friends></response>";
    */
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    
    xmlNode *root_element = xmlDocGetRootElement(doc);
    ResponseFriendsInfo *responseInfo = new ResponseFriendsInfo();
    
    //CCLog("responseFriends : %s", data);
    CCLog("responseFriends : ");
        
    parseFriendsResponse(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        PlayerInfo::getInstance()->setGameFriends(responseInfo->friendsList);
        PlayerInfo::getInstance()->setRanking(responseInfo->myRanking);
    }
    else{
        // error
        popupNetworkError(responseInfo->res,responseInfo->msg, "responseFriends error");
    }
    
    delete responseInfo;
}

void AResponseParser::parseFriendsResponse(xmlNode * node, ResponseFriendsInfo *_info){
    xmlNode *currentNode = NULL;
    ResponseFriendsInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseFriendsResponse currentNode->name :%s", currentNode->name);
        
        bool bSKipChild = false;
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
                CCLog(" info->res :%s",info->res);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
                CCLog(" info->msg :%s",info->msg);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "ranking") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->myRanking = atoi((const char*)child->content);
            }
            
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "friends") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                parseFriend(child, info->friendsList);
                bSKipChild = true;
            }
        }
        if (bSKipChild==false){
            parseFriendsResponse(currentNode->children, info);
        }
    }
}


void AResponseParser::parseFriend(xmlNode * node, CCArray *_friends){
    
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        
        if (currentNode->type == XML_ELEMENT_NODE){
     
            if (strcmp((const char*)currentNode->name, "friend") == 0){
                
                FriendsInfo *fr = new FriendsInfo();
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "userid")==0){
                        if (xa->children){
                            fr->userID = atoll((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                        if (xa->children){
                            fr->level = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ranking")==0){
                        if (xa->children){
                            fr->ranking = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "leadercard")==0){
                        if (xa->children){
                            fr->leaderCard = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                        if (xa->children){
                            fr->attack = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                        if (xa->children){
                            fr->defense = atoi((const char*)xa->children->content);
                        }
                    }
                }
                
                CCLog("friend userid:%lld lev:%d leadercard:%d", fr->userID, fr->level,fr->leaderCard);
                _friends->addObject(fr);
                
                //CCLog("fr->userID:%lld",fr->userID);
                //CCLog("fr->level:%d",fr->level);
                //CCLog("fr->ranking:%d",fr->ranking);
            }
        }
    }
}

void AResponseParser::responseBgList(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    ResponseBgList *responseInfo = new ResponseBgList();
    
    parseBgList(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        PlayerInfo::getInstance()->bgList = responseInfo->bgList;
    }
    else{
        popupNetworkError(responseInfo->res,responseInfo->msg, "responseBgList");
    }
    
    delete responseInfo;
}

void AResponseParser::parseBgList(xmlNode * node, ResponseBgList *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBgList *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseBgList currentNode->name :%s", currentNode->name);
        
        //bool bSKipChild = false;
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseBgList(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
                CCLog(" info->res :%s",info->res);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
                CCLog(" info->msg :%s",info->msg);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "backgrounds") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->bgList = new vector<int>;
                parseBg(child, info->bgList);
                //bSKipChild = true;
            }
        }
        //if (bSKipChild==false){
        //    parseFriendsResponse(currentNode->children, info);
        //}
    }
}

void AResponseParser::parseBg(xmlNode * node, std::vector<int> *v)
{
    
    xmlNode *currentNode = NULL;
    //v = new vector<int>;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("2. parseBg. currentNode->name :%s", currentNode->name);
        
        if (currentNode->type == XML_ELEMENT_NODE){
//            xmlNode *child = currentNode->children;
            if (strcmp((const char*)currentNode->name, "background") == 0){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "bg")==0){
                        if (xa->children){
                            v->push_back(atoi((const char*)xa->children->content));
                        }
                    }
                }
            }
            else if (strcmp((const char*)currentNode->name, "bg") == 0){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                        if (xa->children){
                            //v->push_back(atoi((const char*)xa->children->content));
                            int a = atoi((const char*)xa->children->content);;
                            v->push_back(a);
                            CCLog(" bg list :%d",a);
                        }
                    }
                }
            }
        }
    }
}


bool AResponseParser::responseSelectBg(const char* data){
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    ResponseBasic *responseInfo = new ResponseBasic();
    
    parseSelectBg(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        // ok
        delete responseInfo;
        return true;
    }
    else{
        // error
        popupNetworkError(responseInfo->res, responseInfo->msg, "responseSelectBg");
        delete responseInfo;
        return false;
    }
}

void AResponseParser::parseSelectBg(xmlNode * node, ResponseBasic *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBasic *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseBgList currentNode->name :%s", currentNode->name);
        
        //bool bSKipChild = false;
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseSelectBg(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
                CCLog(" info->res :%s",info->res);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
                CCLog(" info->msg :%s",info->msg);
            }
        }
    }
}


/*
const char *test_msg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message><teams><team type=\"a\" idx=\"0\" deck=\"10,11,12,13,14\"/><team type=\"a\" idx=\"1\" deck=\"20,21,22,13,14\"/><team type=\"a\" idx=\"2\" deck=\"30,31,32,13,34\"/><team type=\"a\" idx=\"3\" deck=\"40,41,42,43,44\"/></teams></response>";
*/


bool AResponseParser::responseTeamlist(const char* data)
{
    if (data == NULL){
        popupNetworkError("Network Error","Network Error","responseTeamlist");
        
        return false;
    }
    
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseTeamlist : %s", data);
    
    ResponseTeamlist *responseInfo = new ResponseTeamlist();
    
    parseTeamlist(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        // ok
        for (int i=0;i<responseInfo->_deck->count();i++){
            ResponseDeckinfo *deck = (ResponseDeckinfo *)responseInfo->_deck->objectAtIndex(i);

            if (deck->idx == 0){
                PlayerInfo::getInstance()->SetCardToDeck(deck);
            }
        }
        
        responseInfo->_deck->autorelease();
        delete responseInfo;
        return true;
    }
    else{
        popupNetworkError(responseInfo->res, responseInfo->msg, "responseTeamlist");
        
        delete responseInfo;
        return false;
    }
}

void AResponseParser::parseTeamlist(xmlNode * node, ResponseTeamlist *_info)
{
    xmlNode *currentNode = NULL;
    ResponseTeamlist *info = _info;
    _info->_deck = new CCArray();
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog("1. parseBgList currentNode->name :%s", currentNode->name);
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseTeamlist(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "teams") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                //info->msg = (const char*)child->content;
                
                parseTeam(child, info);
            }
        }
    }
}

void AResponseParser::parseTeam(xmlNode * node, ResponseTeamlist *_info)
{
    xmlNode *currentNode = NULL;
    
    int team_cnt = 0;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s", currentNode->name);
        ResponseDeckinfo *deck = new ResponseDeckinfo();
        deck->autorelease();
        if (currentNode->type == XML_ELEMENT_NODE){
     
            if (strcmp((const char*)currentNode->name, "team") == 0){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    //CCLog("xa->name :%s",xa->name);
//                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "type")==0){
//                        if (xa->children){
//                            if (strcmp((const char*)xa->children->content, "a") == 0){
//                                deck->teamType = 0;
//                            }
//                            else if (strcmp((const char*)xa->children->content, "b") == 0){
//                                deck->teamType = 1;
//                            }
//                        }
//                    }
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "idx")==0){
                        if (xa->children){
                            deck->idx = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "deck")==0){
                        if (xa->children){
                            
                            int srl[5];
                            int cnt = 0;
                            char *aaa = (char*)xa->children->content;
                            
                            char *bbb = strtok(aaa,",");
                            srl[cnt] = atoi(bbb);
                            cnt++;
                            while(bbb != NULL){
                                if (cnt < 5){
                                
                                    bbb = strtok(NULL,",");
                                    if (bbb != NULL){
                                        srl[cnt] = atoi(bbb);
                                    }
                                    cnt++;
                                }
                                else{
                                    bbb= NULL;
                                }
                            }
                            
                            for(int i=0;i<5;i++){
                                deck->cardSrl[i] = srl[i];
                            }
                            /*
                            deck->cardSrl[0] = srl[0];
                            deck->cardSrl[1] = srl[1];
                            deck->cardSrl[2] = srl[2];
                            deck->cardSrl[3] = srl[3];
                            deck->cardSrl[4] = srl[4];
                            */
                            CCLog("parseTeam, idx[%d], card srl= %d, %d, %d, %d, %d", deck->idx, deck->cardSrl[0], deck->cardSrl[1], deck->cardSrl[2], deck->cardSrl[3], deck->cardSrl[4]);
                            
                            //CCLog(" xa->children->content :%s", xa->children->content);
                            //v->push_back(atoi((const char*)xa->children->content));
                        }
                    }
                }
            }
        }
        _info->_deck->addObject(deck);
        team_cnt++;
    }
}

bool AResponseParser::responseUpgrade(const char *data){
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseUpgrade : %s", data);
    
    ResponseUpgradeInfo *responseInfo = new ResponseUpgradeInfo();
    
    parseUpgrade(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        PlayerInfo::getInstance()->setMaxStamina(responseInfo->questPoint);
        PlayerInfo::getInstance()->setMaxBattlePoint(responseInfo->attackPoint);
        //PlayerInfo::getInstance()->setMaxDefencePoint(responseInfo->defensePoint);
        PlayerInfo::getInstance()->setUpgradePoint(responseInfo->remainPoint);
        
        delete responseInfo;
        return true;
    }
    else{
        popupNetworkError(responseInfo->res, responseInfo->msg, "responseUpgrade");
        
        delete responseInfo;
        return false;
    }
}

bool AResponseParser::parseUpgrade(xmlNode * node, ResponseUpgradeInfo *_info){
    xmlNode *currentNode = NULL;
    ResponseUpgradeInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseUpgrade(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "attack_pnt") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->attackPoint = atoi((const char*)child->content);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "defense_pnt") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->defensePoint = atoi((const char*)child->content);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "quest_pnt") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->questPoint = atoi((const char*)child->content);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "upgrade_pnt") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->remainPoint = atoi((const char*)child->content);
            }
        }
    }
}

bool AResponseParser::responseOpponent(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseOpponent : %s", data);
    CCLog("responseOpponent :");
    
    ResponseOpponent *responseInfo = new ResponseOpponent();
    
    parseOpponent(root_element, responseInfo);
    
    if (atoi(responseInfo->res) == 0){
        
        PlayerInfo *pi = PlayerInfo::getInstance();
        
        pi->battleList = new CCArray();
        
        for(int i=0;i<responseInfo->battleUserList->count();i++){
            UserInfo *user = new UserInfo();
            BattleUserInfo *buser = (BattleUserInfo*)responseInfo->battleUserList->objectAtIndex(i);
            user->userID = buser->userId;
            user->myNickname = buser->nickName;
            user->myLevel = buser->level;
            user->numOfKakaoAppFriends = buser->numOfAppFriends;
            user->ranking = buser->ranking;
            user->profileImageUrl = buser->profileImgUrl;
            user->leaderCard = buser->leaderCard;
            user->attack = buser->attack;
            user->defense = buser->defense;
            
            CCLog(" user id:%lld nick:%s ",user->userID, user->myNickname.c_str());//, user->profileImageUrl.c_str());
            //CCLog(" user url:%s",user->profileImageUrl.c_str());
            
            pi->battleList->addObject(user);
        }
        delete responseInfo;
        return true;
    }
    else{
        popupNetworkError(responseInfo->res, responseInfo->msg, "responseOpponent");
        delete responseInfo;
        return false;
    }
}

void AResponseParser::parseOpponent(xmlNode *node, ResponseOpponent *_info)
{
    xmlNode *currentNode = NULL;
    ResponseOpponent *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseOpponent(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "opponents") == 0)
        {
            _info->battleUserList = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                parseOpponentSub(child, info);
            }
        }
    
    }
}


void AResponseParser::parseOpponentSub(xmlNode * node, ResponseOpponent *_info)
{
    xmlNode *currentNode = NULL;
    
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s", currentNode->name);
        
        BattleUserInfo *user = new BattleUserInfo();
        
        if (currentNode->type == XML_ELEMENT_NODE){
            
            if (strcmp((const char*)currentNode->name, "opponent") == 0){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "userid")==0){
                        if (xa->children){
                            //deck->idx = atoi((const char*)xa->children->content);
                            user->userId = atoll((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "nick")==0){
                        if (xa->children){
                            user->nickName = (const char *)xa->children->content;
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                        if (xa->children){
                            user->level = atoi((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "appfriends")==0){
                        if (xa->children){
                            user->numOfAppFriends = atoi((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ranking")==0){
                        if (xa->children){
                            user->ranking = atoi((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "imgurl")==0){
                        if (xa->children){
                            user->profileImgUrl = (const char *)xa->children->content;
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "leadercard")==0){
                        if (xa->children){
                            user->leaderCard = atoi((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                        if (xa->children){
                            user->attack = atoi((const char *)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                        if (xa->children){
                            user->defense = atoi((const char *)xa->children->content);
                        }
                    }
                }
            }
        }
        
        _info->battleUserList->addObject(user);
    }
    
}
ResponseBattleInfo* AResponseParser::responseBattle(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseBattle : %s", data);
    
    ResponseBattleInfo *responseInfo = new ResponseBattleInfo();
    
    parseBattle(root_element, responseInfo);
    
    /*
    if (atoi(responseInfo->res) == 0){
        
        responseInfo;
        return true;
    }
    else{
        delete responseInfo;
        return false;
    }
    */
    return responseInfo;
    
}

void AResponseParser::parseBattle(xmlNode *node, ResponseBattleInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBattleInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseBattle(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "battle") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                parseBattleSub(child,info);
            }
        }
        
    }

}

void AResponseParser::parseBattleSub(xmlNode *node, ResponseBattleInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBattleInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                if (strcmp((const char*)child->content,"win")==0)info->battleResult=0;
                else if (strcmp((const char*)child->content,"lose")==0)info->battleResult=1;
                else if (strcmp((const char*)child->content,"draw")==0)info->battleResult=2;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "reward") == 0)
        {
            //xmlNode *child = currentNode->children;
            //if (child){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                        if (xa->children){
                            info->reward_coin = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                        if (xa->children){
                            info->reward_fame = atoi((const char*)xa->children->content);
                        }
                    }
                }
            //}
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "used_bp") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->used_battle_point = atoi((const char*)child->content);
                // 이름변경되었음. 들어오는지 확인 
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "max_hp") == 0)
        {
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attacker")==0){
                    if (xa->children){
                        info->user_max_hp = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender")==0){
                    if (xa->children){
                        info->rival_max_hp = atoi((const char*)xa->children->content);
                    }
                }
            }
            
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "first_attack") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                if (strcmp((const char*)child->content, "attacker") == 0){
                    info->first_attack = 0;
                }
                else if (strcmp((const char*)child->content, "defender") == 0){
                    info->first_attack = 1;
                }
            }
            
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "opponent_cards") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                int card_cnt = 0;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0){
                        info->opponent_card[card_cnt] = atoi((const char*)childNode->children->content);
                        card_cnt++;
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "opponent_stat") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "nick") == 0){
                        info->enemy_nick =(const char*)childNode->children->content;
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "lev") == 0){
                        info->enemy_level = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "app_friends") == 0){
                        info->enemy_appFriends = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "battle_cnt") == 0){
                        info->enemy_battle_pnt = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "victory_cnt") == 0){
                        info->enemy_victory_cnt = atoi((const char*)childNode->children->content);
                    }
                    //else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "defense_pnt") == 0){
                    //    info->enemy_defense_pnt = atoi((const char*)childNode->children->content);
                    //}
                    //else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "draw_cnt") == 0){
                    //    info->enemy_draw_cnt = atoi((const char*)childNode->children->content);
                    //}
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "rounds") == 0)
        {
            info->rounds = new CCArray();
            info->skills = new CCArray();
            xmlNode *childNode1 = currentNode->children;
            if (childNode1){
                
                xmlNode *childNode2 = NULL;
                for (childNode2 = childNode1; childNode2; childNode2 = childNode2->next)
                {
                    if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "round") == 0)
                    {
                        ABattleRound* round = new ABattleRound();
                        
                        xmlAttr *xa = NULL;
                        for (xa = childNode2->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    round->round_id = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                        
                        if (childNode2->children){
                            parseRound(childNode2->children, round);
                        }
                        
                        // attacker_point, defender_point들 합산
                        info->user_attack_tot += round->attacker_point;
                        info->user_friends_tot += round->attacker_friend;
                        info->user_ext_tot += round->attacker_ext;
                        
                        info->rival_attack_tot += round->defender_point;
                        info->rival_friends_tot += round->defender_friend;
                        info->rival_ext_tot += round->defender_ext;
                        
                        info->rounds->addObject(round);
                        //마지막 라운드의 hp로 유저와 라이벌의 남은 hp세팅
                        info->user_remain_hp = round->attacker_hp;
                        info->rival_remain_hp = round->defender_hp;
                        
                        // skill 합산
                        if (round->skills->count() > 0){
                            for(int i=0;i<round->skills->count();i++){
                                OpponentSkillInfo *skillInfo = (OpponentSkillInfo*)round->skills->objectAtIndex(i);
                                info->skills->addObject(skillInfo);
                            }
                        }
                    }
                }
            }
            
        }
        /*
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "powers") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "attacker") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next)
                        {
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "point")==0){
                                if (xa->children){
                                    info->attackerPoint =atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ext")==0){
                                if (xa->children){
                                    info->attackerSkillPoint =atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if(childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "defender") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next)
                        {
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "point")==0){
                                if (xa->children){
                                    info->defenderPoint =atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ext")==0){
                                if (xa->children){
                                    info->defenderSkillPoint =atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "skills") == 0)
        {
            info->skills = new CCArray();
            xmlNode *child = currentNode->children;
            if (child)
            {
                parseSkill(child, info->skills);
            }
        }*/
        
    }
    
}

void AResponseParser::parseSkill(xmlNode * node, CCArray *skills)
{
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE)
        {
            if (strcmp((const char*)currentNode->name, "skill") == 0)
            {
                OpponentSkillInfo *skillInfo = new OpponentSkillInfo();
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next)
                {
                    //CCLog(" xa->name :%s",xa->name);
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "card")==0)
                    {
                        if (xa->children)
                        {
                            skillInfo->cardID =atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "side")==0)
                    {
                        if (xa->children)
                        {
                            if (strcmp((const char*)xa->children->content, "attacker") == 0)skillInfo->side=0;
                            else if (strcmp((const char*)xa->children->content, "defender") == 0)skillInfo->side=1;
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill")==0)
                    {
                        if (xa->children)
                        {
                            skillInfo->skillID = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "slot")==0)
                    {
                        if (xa->children)
                        {
                            skillInfo->slot = atoi((const char*)xa->children->content);
                        }
                    }
                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0)
                    {
                        if (xa->children)
                        {
                            skillInfo->skillID = atoi((const char*)xa->children->content);
                        }
                    }
                    
                }
                skills->addObject(skillInfo);
            }
        }
    }
}



ResponseNoticeInfo* AResponseParser::responseNotice(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseNotice : %s", data);
    CCLog("responseNotice ");
    
    ResponseNoticeInfo *responseInfo = new ResponseNoticeInfo();
    
    parseNotice(root_element, responseInfo);
    
    return responseInfo;
    /*
    if (atoi(responseInfo->res) == 0){

        delete responseInfo;
        return true;
    }
    else{
        delete responseInfo;
        return false;
    }
     */
    
}
/*
<response>
 <res>0</res>
 <message />
 <notices>
    <notice type=”battle” id=”xxx”>
    <res>xxx</res>
    <nick>bbb</nick>
    <date>xxx</date>
 </notice>
</notices>
</response>
 */


ResponseNoticeInfo* AResponseParser::parseNotice(xmlNode * node, ResponseNoticeInfo *_info)
{
    
    xmlNode *currentNode = NULL;
    ResponseNoticeInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseNotice(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "notices") == 0)
        {
            info->notices = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                parseNoticeSub(child,info);
            }
        }
        
    }
    return info;
}
/*
const char* testData = "
<?xml version=\"1.0\" encoding=\"utf-8\"?><response><res>0</res><message></message>
<notices>
<notice type=\"battle\" id=\"58\"><res>lose</res><nick>noname</nick><date>1353590985</date></notice>
<notice type=\"battle\" id=\"56\"><res>lose</res><nick>noname</nick><date>1353581124</date></notice>
<notice type=\"battle\" id=\"52\"><res>lose</res><nick>noname</nick><date>1353570807</date></notice>
<notice type=\"battle\" id=\"51\"><res>lose</res><nick>noname</nick><date>1353570793</date></notice>
<notice type=\"battle\" id=\"34\"><res>lose</res><nick>noname</nick><date>1353561458</date></notice>
</notices></response>";
*/
void AResponseParser::parseNoticeSub(xmlNode * node, ResponseNoticeInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseNoticeInfo *info = _info;
    //NoticeInfo *noticeInfo = new NoticeInfo();
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        NoticeInfo *noticeInfo = new NoticeInfo();
        //CCLog(" currentNode->name :%s",currentNode->name);
        
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "notice") == 0){
            
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                //CCLog(" xa->name :%s",xa->name);
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "type")==0){
                    if (xa->children){
                        if (strcmp((const char*)xa->children->content, "battle") == 0){
                            noticeInfo->type = 0;
                        }
                        else if (strcmp((const char*)xa->children->content, "trade") == 0){
                            noticeInfo->type = 1;
                        }
                        else if (strcmp((const char*)xa->children->content, "gift") == 0){
                            noticeInfo->type = 2;
                        }
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                    if (xa->children){
                        noticeInfo->noticeID = atoi((const char*)xa->children->content);
                    }
                }
            }
            if (currentNode->children){
                xmlNode *childNode = NULL;
                for (childNode = currentNode->children; childNode; childNode = childNode->next)
                {
                    //CCLog(" childNode->name :%s",childNode->name);
                    
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "res") == 0){
                        xmlNode *child = childNode->children;
                        if (child){
                            if (strcmp((const char*)child->content,"win")== 0){
                                noticeInfo->result = 0;
                            }
                            else if (strcmp((const char*)child->content,"lose")== 0){
                                noticeInfo->result = 1;
                            }
                            else if (strcmp((const char*)child->content,"draw")== 0){
                                noticeInfo->result = 2;
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "nick") == 0){
                        xmlNode *child = childNode->children;
                        if (child){
                            noticeInfo->nick = (const char*)child->content;
                            //CCLog("noticeInfo->nick:%s",noticeInfo->nick);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "date") == 0){
                        xmlNode *child = childNode->children;
                        if (child){
                            noticeInfo->date = atoi((const char*)child->content);
                            //CCLog("child->content :%s",child->content);
                            //CCLog("noticeInfo->date :%d ",noticeInfo->date);
                        }
                    }
                }
            }
        }
        
        info->notices->addObject(noticeInfo);
        
    }
    
}



ResponseDetailNoticeInfo* AResponseParser::responseDetailNotice(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseDetailNotice : %s", data);
    CCLog("responseDetailNotice");
    
    ResponseDetailNoticeInfo *responseInfo = new ResponseDetailNoticeInfo();
    
    parseDetailNotice(root_element, responseInfo);
    
    return responseInfo;
    

}

void AResponseParser::parseDetailNotice(xmlNode * node, ResponseDetailNoticeInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseDetailNoticeInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseDetailNotice(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "battle") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                parseDetailNoticeSub(child,info);
            }
        }
        
    }
    
}

void AResponseParser::parseDetailNoticeSub(xmlNode *node, ResponseDetailNoticeInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseDetailNoticeInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                if (strcmp((const char*)child->content,"win")==0)info->battleResult=0;
                else if (strcmp((const char*)child->content,"lose")==0)info->battleResult=1;
                else if (strcmp((const char*)child->content,"draw")==0)info->battleResult=2;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "reward") == 0)
        {
            //xmlNode *child = currentNode->children;
            //if (child){
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                    if (xa->children){
                        info->reward_coin = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                    if (xa->children){
                        info->reward_coin = atoi((const char*)xa->children->content);
                    }
                }
            }
            //}
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "opponent_cards") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                int card_cnt = 0;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0){
                        info->opponent_card[card_cnt] = atoi((const char*)childNode->children->content);
                        card_cnt++;
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "opponent_stat") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "nick") == 0){
                        info->enemy_nick =(const char*)childNode->children->content;
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "lev") == 0){
                        info->enemy_level = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "app_friends") == 0){
                        info->enemy_appFriends = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "defense_pnt") == 0){
                        info->enemy_defense_pnt = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "battle_cnt") == 0){
                        info->enemy_battle_pnt = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "victory_cnt") == 0){
                        info->enemy_victory_cnt = atoi((const char*)childNode->children->content);
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "draw_cnt") == 0){
                        info->enemy_draw_cnt = atoi((const char*)childNode->children->content);
                    }
                }
            }
        }
        
    }
    
}

ResponseQuestListInfo* AResponseParser::responseQuestList(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseQuestList : %s", data);
    CCLog("responseQuestList -");//: %s", data);
    
    ResponseQuestListInfo *responseInfo = new ResponseQuestListInfo();
    
    parseQuestList(root_element, responseInfo);
    
    return responseInfo;
}


void AResponseParser::parseQuestList(xmlNode * node, ResponseQuestListInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseQuestListInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseQuestList(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "quests") == 0)
        {
            info->questList = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                parseQuestListSub(child,info);
            }
        }
        
    }
}

void AResponseParser::parseQuestListSub(xmlNode * node, ResponseQuestListInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseQuestListInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
//        CCLog(" currentNode->name :%s",currentNode->name);
        AQuestInfo *questInfo = new AQuestInfo();
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "quest") == 0){
            
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
//                CCLog(" xa->name :%s",xa->name);
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                    if (xa->children){
                        questInfo->questID = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "begin")==0){
                    if (xa->children){
                        questInfo->beginTime = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "end")==0){
                    if (xa->children){
                        questInfo->endTime = atoi((const char*)xa->children->content);
                    }
                }
                //else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "complete_cnt")==0){
                //    if (xa->children){
                //        questInfo->completeCnt = atoi((const char*)xa->children->content);
                //    }
                //}
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "progress")==0){
                    if (xa->children){
                        questInfo->progress = atoi((const char*)xa->children->content);
                        //CCLog(" questInfo->progress :%d", questInfo->progress);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "clear")==0){
                    if (xa->children){
                        questInfo->clear = atoi((const char*)xa->children->content);
                        //CCLog(" questInfo->clear :%d", questInfo->clear);
                        CCLog("quest id:%d progress:%d clear:%d", questInfo->questID, questInfo->progress, questInfo->clear);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max_progress")==0){
                    if (xa->children){
                        questInfo->progressMax = atoi((const char*)xa->children->content);
                        //CCLog(" questInfo->progressMax :%d", questInfo->progressMax);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "enemy")==0){
                    if (xa->children){
                        questInfo->enemy = atoi((const char*)xa->children->content);
                    }
                }
            }
            
            //CCLog("questInfo->questID :%d end:%d progress:%d clear:%d", questInfo->questID, questInfo->endTime, questInfo->progress, questInfo->clear);
                        
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "progress") == 0){
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            questInfo->progress = atoi((const char*)cchild->content);
                            // 여기 들어오면 안됨, 삭제되었음
                        }
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max")==0){
                                if (xa->children){
                                    questInfo->progressMax= atoi((const char*)xa->children->content);
                                }
                            }
                        }
                            
                    }
                    /*
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card_0") == 0){
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            questInfo->card1 = atoi((const char*)cchild->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card_1") == 0){
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            questInfo->card2 = atoi((const char*)cchild->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card_2") == 0){
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            questInfo->card3 = atoi((const char*)cchild->content);
                        }
                    }
                     */
                }
            }
            
        }
        info->questList->addObject(questInfo);
    }
}

ResponseQuestUpdateResultInfo* AResponseParser::responseQuestResultUpdate(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseQuestResultUpdate : %s", data);
    
    
    ResponseQuestUpdateResultInfo *responseInfo = new ResponseQuestUpdateResultInfo();
    
    parseQuestResultUpdate(root_element, responseInfo);
    
    /*
    CCLog("responseQuestResultUpdate ");
    CCLog("responseInfo progress max:%d progress:%d", responseInfo->progressMax, responseInfo->progress);
    CCLog("coin:%d exp:%d ",responseInfo->coin, responseInfo->exp);
    CCLog("card srl:%d id:%d lev:%d exp:%d attack:%d", responseInfo->card_srl, responseInfo->card_id, responseInfo->card_level, responseInfo->card_exp, responseInfo->card_attack);
    CCLog("card item1:%d item2:%d", responseInfo->item1, responseInfo->item2);
    CCLog("user stat exp:%d lev:%d u_pnt:%d coin:%d", responseInfo->user_exp, responseInfo->user_lev, responseInfo->user_uPnt, responseInfo->user_coin);
    */
    
    CCLog("responseQuestResultUpdate : , res:%s", responseInfo->res);
    return responseInfo;
}




void AResponseParser::parseQuestResultUpdate(xmlNode * node, ResponseQuestUpdateResultInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseQuestUpdateResultInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseQuestResultUpdate(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "quest") == 0)
        {
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "sp")==0){
                    if (xa->children){
                        info->sp = atoi((const char*)xa->children->content);
                        CCLog("quest->sp:%d",info->sp);
                    }
                }
            }
                
            xmlNode *child = currentNode->children;
            if (child){
                parseQuestResultUpdateSub(child, info);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "user_stat") == 0)
        {
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                //CCLog(" xa->name :%s",xa->name);
                
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                    if (xa->children){
                        info->user_lev =atoi((const char*)xa->children->content);
                        
                        //CCLog("info->user_lev :%d", info->user_lev);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                    if (xa->children){
                        info->user_exp =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max_exp")==0){
                    if (xa->children){
                        info->user_exp_max =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                    if (xa->children){
                        info->user_coin =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                    if (xa->children){
                        info->user_uPnt =atoi((const char*)xa->children->content);
                        //CCLog("info->user_uPnt :%d", info->user_uPnt);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                    if (xa->children){
                        info->user_revenge =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                    if (xa->children){
                        info->user_questPoint =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                    if (xa->children){
                        info->user_attackPoint =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                    if (xa->children){
                        //info->user_defensePoint =atoi((const char*)xa->children->content);
                    }
                }
            }
            
        }
        
    }
    
}



void AResponseParser::parseQuestResultUpdateSub(xmlNode * node, ResponseQuestUpdateResultInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseQuestUpdateResultInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s",currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "progress") == 0){
            
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                //CCLog(" xa->name :%s",xa->name);
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max")==0){
                    if (xa->children){
                        info->progressMax =atoi((const char*)xa->children->content);
                    }
                }
                //else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "complete_cnt")==0){
                //    if (xa->children){
                //        info->completeCnt =atoi((const char*)xa->children->content);
                //        CCLog(" quest->completeCnt:%d",info->completeCnt);
                //    }
                //}
            }
            
            xmlNode *child = currentNode->children;
            if (child){
                info->progress = atoi((const char*)child->content);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "coin") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->coin = atoi((const char*)child->content);
                CCLog("trace reward coin:%d",info->coin);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "exp") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->exp = atoi((const char*)child->content);
                CCLog("trace reward exp:%d",info->exp);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "bp") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->bp = atoi((const char*)child->content);
                CCLog("trace reward bp:%d",info->bp);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "sp") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->sp = atoi((const char*)child->content);
                CCLog("trace reward sp:%d",info->sp);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "cards") == 0){
            xmlNode *child = currentNode->children;
            
            info->cards = new CCArray();
            
            if (child){
                //if (strcmp((const char*)child->name, "slotIndex")==0){
                //info->slotIndex = atoi((const char*)child->content);
                //}
                
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0)
                    {
                        QuestRewardCardInfo *card = new QuestRewardCardInfo();
                        
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    card->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    card->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    card->card_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    card->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    card->card_attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    card->card_defense = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                if (xa->children){
                                    card->card_skillEffect = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_lev")==0){
                                if (xa->children){
                                    card->card_skillLev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                        
                        info->cards->addObject(card);
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "enemy") == 0){
            xmlNode *child = currentNode->children;
            if (child){
                info->enemy_code = atoi((const char*)child->content);
                //CCLog("enemy_code:%d", info->enemy_code); // 제대로 들어오는지 확인!
            }
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "type")==0){
                    if (xa->children){
                        info->enemy_type =atoi((const char*)xa->children->content);
                        
                        if (strcmp((const char*)xa->children->content, "none")==0){
                            info->enemy_type = 0;
                        }
                        else if (strcmp((const char*)xa->children->content, "normal")==0){
                            info->enemy_type = 1;
                        }
                        else if (strcmp((const char*)xa->children->content, "rival")==0){
                            info->enemy_type = 2;
                        }
                        else if (strcmp((const char*)xa->children->content, "hidden")==0){
                            info->enemy_type = 3;
                        }
                        else if (strcmp((const char*)xa->children->content, "boss")==0){
                            info->enemy_type = 4;
                        }
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "rid")==0){
                    if (xa->children){
                        info->rid =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "hp")==0){
                    if (xa->children){
                        info->enemy_hp =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max_hp")==0){
                    if (xa->children){
                        info->enemy_hp_max =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lv")==0){
                    if (xa->children){
                        info->enemy_level =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "limit")==0){
                    if (xa->children){
                        info->battle_limit_time =atoi((const char*)xa->children->content);
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "type") == 0){
            //xmlNode *child = currentNode->children;
            //if (child){
                xmlAttr *xa = NULL;
                for (xa = currentNode->properties; xa; xa = xa->next){
                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "action")==0){
                        if (xa->children){
                            if (strcmp((const char*)xa->children->content, "trace")==0){
                                info->action = 0;
                            }
                            else if (strcmp((const char*)xa->children->content, "battle")==0){
                                info->action = 1;
                            }
                            else if (strcmp((const char*)xa->children->content, "avoid")==0){
                                info->action = 2;
                            }
                        }
                    }
                }
            
                xmlNode *child = currentNode->children;
                if (child){
                    xmlNode *childNode = NULL;
                    for (childNode = child; childNode; childNode = childNode->next)
                    {
                        if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "res") == 0)
                        {
                            xmlNode *childNode1 = childNode->children;
                            if (childNode1){
                                //info->battleResult = (const char*)childNode1->content;
                                if (strcmp((const char*)childNode1->content, "win")==0){
                                    info->questBattleResult=1;
                                }
                                else if (strcmp((const char*)childNode1->content, "lose")==0){
                                    info->questBattleResult=0;
                                }
                            }
                        }
                        else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "max_hp") == 0)
                        {
                            xmlAttr *xa = NULL;
                            for (xa = childNode->properties; xa; xa = xa->next){
                                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attacker")==0){
                                    if (xa->children){
                                        info->user_max_hp = atoi((const char*)xa->children->content);
                                    }
                                }
                                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender")==0){
                                    if (xa->children){
                                        info->rival_max_hp = atoi((const char*)xa->children->content);
                                    }
                                }
                                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender_cur")==0){
                                    if (xa->children){
                                        info->rival_hp_before_battle = atoi((const char*)xa->children->content);
                                    }
                                }
                            }
                        }
                        else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "rounds") == 0)
                        {
                            info->rounds = new CCArray();
                            xmlNode *childNode1 = childNode->children;
                            if (childNode1){
                                
                                xmlNode *childNode2 = NULL;
                                for (childNode2 = childNode1; childNode2; childNode2 = childNode2->next)
                                {
                                    if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "round") == 0)
                                    {
                                        ABattleRound* round = new ABattleRound();
                                        
                                        xmlAttr *xa = NULL;
                                        for (xa = childNode2->properties; xa; xa = xa->next){
                                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                                if (xa->children){
                                                    round->round_id = atoi((const char*)xa->children->content);
                                                }
                                            }
                                        }
                                        
                                        if (childNode2->children){
                                            parseRound(childNode2->children, round);
                                        }
                                        
                                        info->user_attack_tot += round->attacker_point;
                                        info->user_friends_tot += round->attacker_friend;
                                        info->user_ext_tot += round->attacker_ext;
                                        
                                        info->rival_attack_tot += round->defender_point;
                                        info->rival_friends_tot += round->defender_friend;
                                        info->rival_ext_tot += round->defender_ext;
                                        
                                        info->rounds->addObject(round);
                                        //마지막 라운드의 hp로 유저와 라이벌의 남은 hp세팅
                                        info->user_remain_hp = round->attacker_hp;
                                        info->rival_hp_after_battle = round->defender_hp;
                                    }
                                }
                            }
                            //////////////////////////////////////////////////
                        }
                        
                    }
                }
            //}
        }
//        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "items") == 0){
//            xmlNode *child = currentNode->children;
//            // item항목 삭제됨. 들어오면 안됨 
//            if (child){
//                int itemId[2];
//                int cnt = 0;
//                char *aaa = (char*)child->content;
//                
//                char *bbb = strtok(aaa,",");
//                itemId[cnt] = atoi(bbb);
//                cnt++;
//                while(bbb != NULL){
//                    bbb = strtok(NULL,",");
//                    if (bbb != NULL)itemId[cnt] = atoi(bbb);
//                    cnt++;
//                    //CCLog(" bbb=%s",bbb);
//                }
//                info->item1 = itemId[0];
//                info->item2 = itemId[1];
//            }
//        }
        
    }
}

ResponseRefreshInfo* AResponseParser::responseRefresh(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseRefresh : %s", data);
    
    ResponseRefreshInfo *responseInfo = new ResponseRefreshInfo();
    
    parseRefresh(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseRefresh(xmlNode * node, ResponseRefreshInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseRefreshInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseRefresh(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "user_stat") == 0)
        {
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){

                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                    if (xa->children){
                        info->user_level =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                    if (xa->children){
                        info->exp =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max_exp")==0){
                    if (xa->children){
                        info->max_exp = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                    if (xa->children){
                        info->coin =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                    if (xa->children){
                        info->gold =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                    if (xa->children){
                        info->user_questPoint = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                    if (xa->children){
                        info->user_defensePoint = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                    if (xa->children){
                        info->user_attackPoint = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                    if (xa->children){
                        info->user_uPoint =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                    if (xa->children){
                        info->user_fame = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                    if (xa->children){
                        info->user_revenge =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "battle_cnt")==0){
                    if (xa->children){
                        info->battleCnt =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "victory_cnt")==0){
                    if (xa->children){
                        info->victoryCnt =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "draw_cnt")==0){
                    if (xa->children){
                        info->drawCnt =atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ranking")==0){
                    if (xa->children){
                        info->user_ranking = atoi((const char*)xa->children->content);
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "notice") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->notice = atoi((const char*)child->content);
            }
        }
        //fame="" q_pnt="" d_pnt="" a_pnt="" u_pnt="" coin="" exp="" lev=""/>
        
    }
    
}


ResponseCollectionInfo* AResponseParser::responseCollection(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseCollection : %s", data);
    CCLog("responseCollection,");
    
    
    ResponseCollectionInfo *responseInfo = new ResponseCollectionInfo();
    
    parseCollection(root_element, responseInfo);
    
    CCLog("responseCollection, responseInfo->cardlist->count():%d", responseInfo->cardlist->count());
    
    return responseInfo;
}


void AResponseParser::parseCollection(xmlNode * node, ResponseCollectionInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseCollectionInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseCollection(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "cards") == 0)
        {
            info->cardlist = new CCArray();
            
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    AColloctionCard* card = new AColloctionCard();
                    xmlNode *cchild = childNode->children;
                    if (cchild){
                        card->cardId = atoi((const char*)cchild->content);
                    }
                    
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "own")==0){
                                if (xa->children){
                                    card->bOwn = false;
                                    if (strcmp((const char*)xa->children->content, "y") == 0){
                                        card->bOwn = true;
                                    }
                                    
                                }
                            }
           
                        }
                    }
                    info->cardlist->addObject(card);
                }
            }
            
        }
    }
}

ResponseFusionInfo* AResponseParser::responseFusion(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseFusion : %s", data);
    
    ResponseFusionInfo *responseInfo = new ResponseFusionInfo();
    
    parseFusion(root_element, responseInfo);
    
    return responseInfo;
}


void AResponseParser::parseFusion(xmlNode * node, ResponseFusionInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseFusionInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseFusion(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "fusion") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    info->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->card_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    info->attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    info->defense = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "cost") == 0)
                    {
                        xmlNode *child = childNode->children;
                        if (child){
                            info->cost = atoi((const char*)child->content);
                        }
                    }
                }
                
            }
        }
    }
}

ResponseTrainingInfo* AResponseParser::responseTraining(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseTraining : %s", data);
    
    ResponseTrainingInfo *responseInfo = new ResponseTrainingInfo();
    
    parseTraining(root_element, responseInfo);
    
    return responseInfo;
}


void AResponseParser::parseTraining(xmlNode * node, ResponseTrainingInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseTrainingInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseTraining(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "training") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    info->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->card_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    info->attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    info->defense = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                if (xa->children){
                                    info->skill_effect = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "cost") == 0)
                    {
                        xmlNode *child = childNode->children;
                        if (child){
                            info->cost = atoi((const char*)child->content);
                        }
                    }
                }
                
            }
        }
    }
}

ResponseItemInfo* AResponseParser::responseItemList(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //printf("responseItemList : %s", data);
    printf("responseItemList");
    
    ResponseItemInfo *responseInfo = new ResponseItemInfo();
    
    parseItemList(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseItemList(xmlNode * node, ResponseItemInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseItemInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseItemList(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "items") == 0)
        {
            info->itemList = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    ItemInfo *item = new ItemInfo();
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "item") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    item->itemID = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "count")==0){
                                if (xa->children){
                                    item->count = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    CCLog("item id:%d count:%d", item->itemID, item->count);
                    info->itemList->addObject(item);
                }
                
            }
        }
    }
}

ResponseGiftInfo* AResponseParser::responseGiftList(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
//    CCLog("responseGiftList : %s", data);
    
    ResponseGiftInfo *responseInfo = new ResponseGiftInfo();
    
    parseGiftList(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseGiftList(xmlNode * node, ResponseGiftInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseGiftInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseGiftList(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "gifts") == 0)
        {
            info->giftList = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    GiftInfo *item = new GiftInfo();
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "item") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next)
                        {
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0)
                            {
                                if (xa->children){
                                    item->giftID = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "count")==0){
                                if (xa->children){
                                    item->count = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "recv_date")==0){
                                if (xa->children){
                                    item->receiveDate = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    item->giftSrl = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "nick")==0){
                                if (xa->children){
                                    item->nick = (const char*)xa->children->content;
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "from")==0){
                                if (xa->children){
                                    item->from = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    info->giftList->addObject(item);
                }
                
            }
        }
    }
}

ResponseBuyInfo* AResponseParser::responseBuyItem(const char *data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseBuyItem : %s", data);
    
    ResponseBuyInfo *responseInfo = new ResponseBuyInfo();
    
    parseBuyItem(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseBuyItem(xmlNode * node, ResponseBuyInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseBuyInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseBuyItem(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "buy") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "cost") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->cost_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->cost_coin = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "item") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "count")==0){
                                if (xa->children){
                                    info->item_count = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->itemID = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_stat") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->user_stat_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->user_stat_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                                if (xa->children){
                                    info->user_stat_revenge = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                                if (xa->children){
                                    info->user_stat_fame = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                                if (xa->children){
                                    info->user_stat_q_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                                if (xa->children){
                                    //info->user_stat_d_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                                if (xa->children){
                                    info->user_stat_a_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                                if (xa->children){
                                    info->user_stat_u_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->user_stat_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->user_stat_lev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    
                }
                
            }
        }
    }
}

ResponseUseInfo* AResponseParser::responseUseItem(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseUseItem : %s", data);
    CCLog("responseUseItem");
    
    ResponseUseInfo *responseInfo = new ResponseUseInfo();
    
    parseUseItem(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseUseItem(xmlNode * node, ResponseUseInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseUseInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseUseItem(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "use") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_stat") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                                if (xa->children){
                                    info->user_stat_fame = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                                if (xa->children){
                                    info->user_stat_q_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                                if (xa->children){
                                    //info->user_stat_d_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                                if (xa->children){
                                    info->user_stat_a_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                                if (xa->children){
                                    info->user_stat_u_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->user_stat_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->user_stat_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->user_stat_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->user_stat_lev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "items") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->item_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "count")==0){
                                if (xa->children){
                                    info->item_count = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "cards") == 0)
                    {
                        /////////////////////////////////////////
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            info->cards = new CCArray();
                            xmlNode *cchildNode = NULL;
                            for (cchildNode = cchild; cchildNode; cchildNode = cchildNode->next)
                            {
                                if (cchildNode->type == XML_ELEMENT_NODE && strcmp((const char*)cchildNode->name, "card") == 0){
                                    AReceivedCard* receivedCard = new AReceivedCard();
                                    xmlAttr *xa = NULL;
                                    for (xa = cchildNode->properties; xa; xa = xa->next){
                                        if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                            if (xa->children){
                                                receivedCard->card_srl = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                            if (xa->children){
                                                receivedCard->card_id = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                            if (xa->children){
                                                receivedCard->card_lev = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                            if (xa->children){
                                                receivedCard->card_exp = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                            if (xa->children){
                                                receivedCard->card_attack = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                            if (xa->children){
                                                receivedCard->card_defense = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                            if (xa->children){
                                                receivedCard->card_skill_effect = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_lev")==0){
                                            if (xa->children){
                                                receivedCard->card_skill_lev = atoi((const char*)xa->children->content);
                                            }
                                        }
                                        
                                    }
                                    info->cards->addObject(receivedCard);
                                }
                                
                            }
                        }
                        /////////////////////////////////////////
                        
                    }

                }
            }

        }
    }
}

ResponseSellInfo* AResponseParser::responseSell(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseSell : %s", data);
    
    ResponseSellInfo *responseInfo = new ResponseSellInfo();
    
    parseSell(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseSell(xmlNode * node, ResponseSellInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseSellInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseSell(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "sell") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_stat") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                                if (xa->children){
                                    info->user_stat_revenge = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                                if (xa->children){
                                    info->user_stat_fame = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                                if (xa->children){
                                    info->user_stat_q_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                                if (xa->children){
                                    //info->user_stat_d_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                                if (xa->children){
                                    info->user_stat_a_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                                if (xa->children){
                                    info->user_stat_u_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->user_stat_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->user_stat_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->user_stat_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->user_stat_lev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "coin") == 0)
                    {
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            //CCLog("cchild name :%s", cchild->content);
                            info->coin = atoi((const char*)cchild->content);
                        }
                    }
                }
            }
            
        }
    }
}


ResponseTbInfo* AResponseParser::responseTb(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseTb : %s", data);
    
    
    ResponseTbInfo *responseInfo = new ResponseTbInfo();
    
    parseTb(root_element, responseInfo);
    
    CCLog("responseTb, res:%d",responseInfo->res);
    
    return responseInfo;
}

void AResponseParser::parseTb(xmlNode * node, ResponseTbInfo *_info)
{
    xmlNode *currentNode = NULL;
    ResponseTbInfo *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseTb(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "treasurebox") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_stat") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                                if (xa->children){
                                    info->user_stat_revenge = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "fame")==0){
                                if (xa->children){
                                    info->user_stat_fame = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                                if (xa->children){
                                    info->user_stat_q_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                                if (xa->children){
                                    //info->user_stat_d_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                                if (xa->children){
                                    info->user_stat_a_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                                if (xa->children){
                                    info->user_stat_u_pnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->user_stat_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->user_stat_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->user_stat_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->user_stat_lev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0){
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->card_lev = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                if (xa->children){
                                    info->card_skill_effect = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    info->card_defense = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    info->card_attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    info->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "coin") == 0)
                    {
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            //CCLog("cchild name :%s", cchild->content);
                            info->coin = atoi((const char*)cchild->content);
                        }
                    }
                }
            }
            
        }
    }
}

ResponseTutorial* AResponseParser::responseTutorial(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseTutorial : %s", data);
    
    ResponseTutorial *responseInfo = new ResponseTutorial();
    
    parseTutorial(root_element, responseInfo);
   
    return responseInfo;
}

void AResponseParser::parseTutorial(xmlNode * node, ResponseTutorial *_info)
{
    xmlNode *currentNode = NULL;
    ResponseTutorial *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseTutorial(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "tutorial_progress") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->tutorialProgress = atoi((const char*)child->content);
            }
        }
    }
}

ResponseMedal* AResponseParser::responseMedal(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseMedal : %s", data);
    
    ResponseMedal *responseInfo = new ResponseMedal();
    
    parseMedal(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseMedal(xmlNode * node, ResponseMedal *_info)
{
    xmlNode *currentNode = NULL;
    ResponseMedal *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseMedal(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "medal") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->medalCount = atoi((const char*)child->content);
            }
        }
    }
}

ResponseRoulette* AResponseParser::responseRoulette(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseRoulette : %s", data);
    
    ResponseRoulette *responseInfo = new ResponseRoulette();
    
    parseRoulette(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseRoulette(xmlNode * node, ResponseRoulette *_info)
{
    xmlNode *currentNode = NULL;
    ResponseRoulette *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseRoulette(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "prize") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "card") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                if (xa->children){
                                    info->card_skill_effect = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    info->card_defense = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    info->card_attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->card_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    info->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "item") == 0){
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    info->item_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "count")==0){
                                if (xa->children){
                                    info->item_count = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "coin") == 0)
                    {
                        xmlNode *cchild = childNode->children;
                        if (cchild){
                            //CCLog("cchild name :%s", cchild->content);
                            info->coin = atoi((const char*)cchild->content);
                        }
                    }
                }
            }
            
        }
    }
}

ResponseEvent* AResponseParser::responseEvent(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //    CCLog("responseEvent : %s", data);
    
    ResponseEvent *responseInfo = new ResponseEvent();
    
    parseEvent(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseEvent(xmlNode * node, ResponseEvent *_info)
{
    xmlNode *currentNode = NULL;
    ResponseEvent *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseEvent(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "events") == 0)
        {
            info->eventList = new CCArray;
            info->eventList->autorelease();
            
            xmlNode *child = currentNode->children;
            if (child)
            {
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    EventInfo* event = new EventInfo();
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "event") == 0)
                    {
                        xmlNode *child = childNode->children;
                        
                        if (child)
                        {
                            xmlNode *childNode = NULL;
                            for (childNode = child; childNode; childNode = childNode->next)
                            {
                                if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "banner_img") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventBanner = (const char*)child->content;
                                    }                                    
                                }
                                else if(childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "detail_img") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventDetail = (const char*)child->content;
                                    }
                                }
                                else if(childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "content") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventDescription = (const char*)child->content;
                                    }
                                }
                            }
                        }
                    }
                    
                    info->eventList->addObject(event);
                }
                
            }
        }
    }
}

ResponseTrace* AResponseParser::responseTrace(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //    CCLog("responseEvent : %s", data);
    
    ResponseTrace *responseInfo = new ResponseTrace();
    
    parseTrace(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseTrace(xmlNode * node, ResponseTrace *_info)
{
    xmlNode *currentNode = NULL;
    ResponseTrace *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseTrace(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "events") == 0)
        {
            /*
            info->eventList = new CCArray;
            info->eventList->autorelease();
            
            xmlNode *child = currentNode->children;
            if (child)
            {
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    EventInfo* event = new EventInfo();
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "event") == 0)
                    {
                        xmlNode *child = childNode->children;
                        
                        if (child)
                        {
                            xmlNode *childNode = NULL;
                            for (childNode = child; childNode; childNode = childNode->next)
                            {
                                if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "banner_img") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventBanner = (const char*)child->content;
                                    }
                                }
                                else if(childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "detail_img") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventDetail = (const char*)child->content;
                                    }
                                }
                                else if(childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "content") == 0)
                                {
                                    xmlNode *child = childNode->children;
                                    if (child){
                                        event->eventDescription = (const char*)child->content;
                                    }
                                }
                            }
                        }
                    }
                    
                    info->eventList->addObject(event);
                }
                
            }
             */
        }
    }
    
}

//ResponseSMexclude* responseSMExclude(const char* data);
//void parseSMEX(xmlNode * node, ResponseSMexclude *_info);

ResponseSMexclude* AResponseParser::responseSMExclude(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseSMExclude : %s", data);
    
    ResponseSMexclude *responseInfo = new ResponseSMexclude();
    
    parseSMEX(root_element, responseInfo);
    
    return responseInfo;
}

void AResponseParser::parseSMEX(xmlNode * node, ResponseSMexclude *_info)
{
    xmlNode *currentNode = NULL;
    ResponseSMexclude *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseSMEX(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "exclude_ids") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->userlist = new vector<long long>;
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next){
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_id") == 0){
                        long long id = atoll((const char*)childNode->children->content);
                        info->userlist->push_back(id);
                    }
                }
            }
        }
    }
}

/*
 else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "backgrounds") == 0)
 {
 xmlNode *child = currentNode->children;
 if (child){
 info->bgList = new vector<int>;
 parseBg(child, info->bgList);
 bSKipChild = true;
 }
 }
 //if (bSKipChild==false){
 //    parseFriendsResponse(currentNode->children, info);
 //}
 }
 }
 
 void AResponseParser::parseBg(xmlNode * node, std::vector<int> *v)
 {
 
 xmlNode *currentNode = NULL;
 //v = new vector<int>;
 for (currentNode = node; currentNode; currentNode = currentNode->next)
 {
 //CCLog("2. parseBg. currentNode->name :%s", currentNode->name);
 
 if (currentNode->type == XML_ELEMENT_NODE){
 //            xmlNode *child = currentNode->children;
 if (strcmp((const char*)currentNode->name, "background") == 0){
 xmlAttr *xa = NULL;
 for (xa = currentNode->properties; xa; xa = xa->next){
 if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "bg")==0){
 if (xa->children){
 v->push_back(atoi((const char*)xa->children->content));
 }
 }
 }
 }
 else if (strcmp((const char*)currentNode->name, "bg") == 0){
 xmlAttr *xa = NULL;
 for (xa = currentNode->properties; xa; xa = xa->next){
 if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
 if (xa->children){
 //v->push_back(atoi((const char*)xa->children->content));
 int a = atoi((const char*)xa->children->content);;
 v->push_back(a);
 CCLog(" bg list :%d",a);
 }
 }
 }
 }
 }
 }
 }
 */

void AResponseParser::readQuest()
{
    unsigned long length = 0;
    //std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("Resources/data/quests.xml");
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("quests.xml");
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    PlayerInfo::getInstance()->questList = new CCArray();
    
    CCArray *questList = PlayerInfo::getInstance()->questList;
    
    parseQuestXML(root_element, questList);
    
    //for(int i=0;i<questList->count();i++)
    //{
    //    QuestInfo *info = (QuestInfo*)questList->objectAtIndex(i);
        //CCLog(" info->questID:%d",info->questID);
        // 20011~20105
    //}
}


void AResponseParser::parseQuestXML(xmlNode * node, CCArray *_questList)
{
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        //CCLog(" currentNode->name :%s", currentNode->name);
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "quests") == 0){
            parseQuestXML(currentNode->children, _questList);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                //CCLog(" child name:%s", child->name);
                QuestInfo *info = new QuestInfo();
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    //CCLog(" childNode name:%s", childNode->name);
                    
                    /*
                    q_name
                    q_sp
                    q_progressmax
                    q_exp_min	q_exp_max
                    q_moneyprob	q_money_min	q_money_max		q_card_min	q_card_max	q_cardnum_min	q_cardnum_max	q_normalprob	q_rivalprogress	q_rivalprob	q_hrivalcard	q_hrivalprob	q_bossprogress
                    */
                    
                    
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_code") == 0) // q_code
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questID = atoi((const char*)aaa->content);
                        }
                    }
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_sp") == 0) // q_code
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_sp = atoi((const char*)aaa->content);
                        }
                    }
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_name") == 0) // q_code
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_name = (const char*)aaa->content;
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_unlock") == 0)// q_unlock
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->bUnlock = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_questp") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questBP = atoi((const char*)aaa->content);
                        }
                    }
                    /*
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_class") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questClass = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_periodtime") == 0)// q_periodtime
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questPeriodTime = atoi((const char*)aaa->content);
                        }
                    }
                    
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_condition") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->contition = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_info") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->nInfo = atoi((const char*)aaa->content);
                        }
                    }
                    
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_progress") == 0) // q_progress
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->progress = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_replay") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questClass = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_replaytime") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->questClass = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_exp") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_exp = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_coin") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_coin = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_cardprob") == 0) // q_cardprob
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_cardprob = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_cardnum") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_cardnum = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_card1") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_card1 = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_card2") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_card2 = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_card3") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_card3 = atoi((const char*)aaa->content);
                        }
                    }else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_item1") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_item1 = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_item2") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_item2 = atoi((const char*)aaa->content);
                        }
                    }*/
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_ranks") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_ranks = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_ranka") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_ranka = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_rankb") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_rankb = atoi((const char*)aaa->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "q_rankc") == 0)
                    {
                        xmlNode *aaa = childNode->children;
                        if (aaa){
                            info->q_rankc = atoi((const char*)aaa->content);
                        }
                    }
                }
                
                _questList->addObject(info);
                
            }
        }
    }
}

ResponseRivalBattle* AResponseParser::responseRBattle(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseRBattle : %s", data);
    
    ResponseRivalBattle *responseInfo = new ResponseRivalBattle();
    
    parseRBatle(root_element, responseInfo);
    
    return responseInfo;
}


void AResponseParser::parseRBatle(xmlNode * node, ResponseRivalBattle *_info)
{
       
    xmlNode *currentNode = NULL;
    ResponseRivalBattle *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseRBatle(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "rbattle") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "res") == 0)
                    {
                        xmlNode *childNode1 = childNode->children;
                        if (childNode1){
                            //info->battleResult = (const char*)childNode1->content;
                            if (strcmp((const char*)childNode1->content, "win")==0){
                                info->battleResult=1;
                            }
                            else if (strcmp((const char*)childNode1->content, "lose")==0){
                                info->battleResult=0;
                            }
                            else if (strcmp((const char*)childNode1->content, "na")==0){
                                info->battleResult=2;
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "reward") == 0)
                    {
                        
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->reward_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->reward_exp = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                        xmlNode *childNode1 = childNode->children;
                        if (childNode1){
                            xmlNode *childNode2 = NULL;
                            for (childNode2 = childNode1; childNode2; childNode2 = childNode2->next)
                            {
                                if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "cards") == 0)
                                {
                                    info->reward_cards = new CCArray();
                                    /////////////////////////////////////////
                                    xmlNode *cchild = childNode2->children;
                                    if (cchild){
                                        xmlNode *cchildNode = NULL;
                                        for (cchildNode = cchild; cchildNode; cchildNode = cchildNode->next)
                                        {
                                            if (cchildNode->type == XML_ELEMENT_NODE && strcmp((const char*)cchildNode->name, "card") == 0){
                                                QuestRewardCardInfo *card = new QuestRewardCardInfo();
                                                xmlAttr *xa = NULL;
                                                for (xa = cchildNode->properties; xa; xa = xa->next){
                                                    if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                                        if (xa->children){
                                                            card->card_srl = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                                        if (xa->children){
                                                            card->card_id = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                                        if (xa->children){
                                                            card->card_level = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                                        if (xa->children){
                                                            card->card_exp = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                                        if (xa->children){
                                                            card->card_attack = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                                        if (xa->children){
                                                            card->card_defense = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                                        if (xa->children){
                                                            card->card_skillEffect = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_lev")==0){
                                                        if (xa->children){
                                                            card->card_skillLev = atoi((const char*)xa->children->content);
                                                        }
                                                    }
                                                    
                                                }
                                                info->reward_cards->addObject(card);
                                            }
                                            
                                        }
                                    }
                                    /////////////////////////////////////////
                                }
                            }
                        }
                        
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "used_bp") == 0)
                    {
                        xmlNode *childNode1 = childNode->children;
                        if (childNode1){
                            info->used_battle_point = atoi((const char*)childNode1->content);
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "max_hp") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attacker")==0){
                                if (xa->children){
                                    info->user_max_hp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender")==0){
                                if (xa->children){
                                    info->rival_max_hp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender_cur")==0){
                                if (xa->children){
                                    info->rival_origin_hp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lv")==0){
                                if (xa->children){
                                    info->rival_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "limit")==0){
                                if (xa->children){
                                    info->battle_limit_time = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "rounds") == 0)
                    {
                        info->rounds = new CCArray();
                        xmlNode *childNode1 = childNode->children;
                        if (childNode1){
                            
                            xmlNode *childNode2 = NULL;
                            for (childNode2 = childNode1; childNode2; childNode2 = childNode2->next)
                            {
                                if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "round") == 0)
                                {
                                    ABattleRound* round = new ABattleRound();
                                    
                                    xmlAttr *xa = NULL;
                                    for (xa = childNode2->properties; xa; xa = xa->next){
                                        if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                            if (xa->children){
                                                round->round_id = atoi((const char*)xa->children->content);
                                            }
                                        }
                                    }
                                    
                                    if (childNode2->children){
                                        parseRound(childNode2->children, round);
                                    }
                                    
                                    info->user_attack_tot += round->attacker_point;
                                    info->user_friends_tot += round->attacker_friend;
                                    info->user_ext_tot += round->attacker_ext;
                                    
                                    info->rival_attack_tot += round->defender_point;
                                    info->rival_friends_tot += round->defender_friend;
                                    info->rival_ext_tot += round->defender_ext;
                                    
                                    info->rounds->addObject(round);
                                    //마지막 라운드의 hp로 유저와 라이벌의 남은 hp세팅
                                    info->user_remain_hp = round->attacker_hp;
                                    info->rival_remain_hp = round->defender_hp;
                                }
                            }
                        }
                        
                    }
                    
                    else if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "user_stat") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "revenge")==0){
                                if (xa->children){
                                    info->user_revenge = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "q_pnt")==0){
                                if (xa->children){
                                    info->user_questPoint = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "d_pnt")==0){
                                if (xa->children){
                                    info->user_defensePoint = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "a_pnt")==0){
                                if (xa->children){
                                    info->user_attackPoint = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "u_pnt")==0){
                                if (xa->children){
                                    info->user_uPnt = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "gold")==0){
                                if (xa->children){
                                    info->user_gold = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "coin")==0){
                                if (xa->children){
                                    info->user_coin = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    info->user_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    info->user_lev = atoi((const char*)xa->children->content);
                                }
                            }
                        }
                    }
                }
                
                
            }
        }
    }
}


void AResponseParser::parseRound(xmlNode * node, ABattleRound *_round)
{
    
    xmlNode *currentNode = NULL;
    ABattleRound *round = _round;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "hp") == 0){
            xmlAttr *xa = NULL;
            for (xa = currentNode->properties; xa; xa = xa->next){
                if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attacker")==0){
                    if (xa->children){
                        round->attacker_hp = atoi((const char*)xa->children->content);
                    }
                }
                else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defender")==0){
                    if (xa->children){
                        round->defender_hp = atoi((const char*)xa->children->content);
                    }
                }
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "skills") == 0)
        {
            round->skills = new CCArray();
            xmlNode *child = currentNode->children;
            if (child)
            {
                parseSkill(child, round->skills);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "powers") == 0)
        {
            xmlNode *childNode1 = currentNode->children;
            if (childNode1){
                xmlNode *childNode2 = NULL;
                for (childNode2 = childNode1; childNode2; childNode2 = childNode2->next)
                {
                    if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "attacker") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode2->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "friends")==0){
                                if (xa->children){
                                    round->attacker_friend = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ext")==0){
                                if (xa->children){
                                    round->attacker_ext= atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "point")==0){
                                if (xa->children){
                                    round->attacker_point= atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "recovery")==0){
                                if (xa->children){
                                    round->attacker_heal= atoi((const char*)xa->children->content);
                                }
                            }

                        }
                        
                    }
                    else if (childNode2->type == XML_ELEMENT_NODE && strcmp((const char*)childNode2->name, "defender") == 0)
                    {
                        xmlAttr *xa = NULL;
                        for (xa = childNode2->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "friends")==0){
                                if (xa->children){
                                    round->defender_friend = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "ext")==0){
                                if (xa->children){
                                    round->defender_ext= atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "point")==0){
                                if (xa->children){
                                    round->defender_point= atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "recovery")==0){
                                if (xa->children){
                                    round->defender_heal= atoi((const char*)xa->children->content);
                                }
                            }

                        }
                    }
                }// for
                
            }
        }
    }
}

ResponseRivalList* AResponseParser::responseRivalList(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    //CCLog("responseRivalList: %s", data);
    CCLog("responseRivalList");// : %s", data);
    
    ResponseRivalList *responseInfo = new ResponseRivalList();
    
    parseRivalList(root_element, responseInfo);
    
    CCLog("responseRivalList, res:%s message:%s", responseInfo->res, responseInfo->msg);
    
    return responseInfo;
}



void AResponseParser::parseRivalList(xmlNode * node, ResponseRivalList *_info)
{
    xmlNode *currentNode = NULL;
    ResponseRivalList *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseRivalList(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "rivals") == 0)
        {
            info->rivals = new CCArray();
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *childNode = NULL;
                for (childNode = child; childNode; childNode = childNode->next)
                {
                    if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "rival") == 0)
                    {
                        AReceivedRival *rival = new AReceivedRival();
                        
                        xmlAttr *xa = NULL;
                        for (xa = childNode->properties; xa; xa = xa->next){
                            
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "type")==0){
                                if (xa->children){
                                    if (strcmp((const char*)xa->children->content, "rival") == 0){
                                        rival->type = 1;
                                    }
                                    else if (strcmp((const char*)xa->children->content, "hidden") == 0){
                                        rival->type = 2;
                                    }
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "max_hp")==0){
                                if (xa->children){
                                    rival->max_hp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "cur_hp")==0){
                                if (xa->children){
                                    rival->cur_hp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lv")==0){
                                if (xa->children){
                                    rival->npc_lv = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "nid")==0){
                                if (xa->children){
                                    rival->npc_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "qst_id")==0){
                                if (xa->children){
                                    rival->quest_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "rid")==0){
                                if (xa->children){
                                    rival->rid = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "birth")==0){
                                if (xa->children){
                                    rival->birth = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "limit")==0){
                                if (xa->children){
                                    rival->limit = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "reward")==0){
                                //if (rival->rid == 851){
                                //    CCLog("(const char*)xa->children->content:%s", (const char*)xa->children->content);
                                //}
                                if (xa->children){
                                    if (strcmp((const char*)xa->children->content, "y") == 0){
                                        rival->bRewardReceived = true;
                                    }
                                    else{
                                        rival->bRewardReceived = false;
                                    }
                                    
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "owner")==0){
                                if (xa->children){
                                    rival->ownerUserID = atoll((const char*)xa->children->content);
                                }
                            }
                        }
                        
                        xmlNode *aaa = childNode->children;
                        parseRivalSub(aaa, rival);
                        
                        //CCLog(" rival id:%d cur_hp:%d max_hp:%d nid:%d lv:%d birth:%d limit:%d reward:%d owner:%lld", rival->rid, rival->cur_hp, rival->max_hp, rival->npc_id, rival->npc_lv, rival->birth, rival->limit, rival->bRewardReceived, rival->ownerUserID);
                        

                        info->rivals->addObject(rival);
                        
                    }
                }
            }
            
            
        }
    }
}

void AResponseParser::parseRivalSub(xmlNode * node, AReceivedRival *rival_info)
{
    xmlNode *currentNode = NULL;
    AReceivedRival *info = rival_info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "colleages") == 0){
            xmlNode *child = currentNode->children;
            xmlNode *childNode = NULL;
            
            info->colleagues = new CCArray();
            
            for (childNode = child; childNode; childNode = childNode->next)
            {
                if (childNode->type == XML_ELEMENT_NODE && strcmp((const char*)childNode->name, "colleage") == 0){
                    xmlAttr *xa = NULL;
                    
                    AReceivedColleague *coll = new AReceivedColleague();
                    
                    for (xa = childNode->properties; xa; xa = xa->next){
                        
                        if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "damages")==0){
                            if (xa->children){
                                coll->damages = atoi((const char*)xa->children->content);
                            }
                        }
                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "userid")==0){
                            if (xa->children){
                                coll->userid = atoll((const char*)xa->children->content);
                            }
                        }
                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "leader_card_id")==0){
                            if (xa->children){
                                coll->leaderCard_id = atoi((const char*)xa->children->content);
                            }
                        }
                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "leader_card_lv")==0){
                            if (xa->children){
                                coll->user_lv = atoi((const char*)xa->children->content);
                            }
                        }
//                        if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "name")==0){
//                            if (xa->children){
//                                coll->name = (const char*)xa->children->content;
//                            }
//                        }
//                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "imgurl")==0){
//                            if (xa->children){
//                                coll->imgUrl = (const char*)xa->children->content;
//                            }
//                        }
//                        else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "damages")==0){
//                            if (xa->children){
//                                coll->damages = atoi((const char*)xa->children->content);
//                            }
//                        }
                    }
                    
                    info->colleagues->addObject(coll);
                }
            }
        }
    }
}



ResponseRivalReward* responseRivalReward(const char* data);
void parseRivalReward(xmlNode * node, ResponseRivalReward *_info);

ResponseRivalReward* AResponseParser::responseRivalReward(const char* data)
{
    xmlDocPtr doc = xmlReadMemory(data, strlen(data), "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCLog("responseRivalReward : %s", data);
    
    ResponseRivalReward *responseInfo = new ResponseRivalReward();
    
    parseRivalReward(root_element, responseInfo);
    
    return responseInfo;
}



void AResponseParser::parseRivalReward(xmlNode * node, ResponseRivalReward *_info)
{
    xmlNode *currentNode = NULL;
    ResponseRivalReward *info = _info;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "response") == 0){
            parseRivalReward(currentNode->children, info);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "res") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->res = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "message") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->msg = (const char*)child->content;
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "rreward") == 0)
        {
            info->rewardCards = new CCArray();
            /////////////////////////////////////////
            xmlNode *child = currentNode->children;
            if (child){
                xmlNode *cchildNode = NULL;
                for (cchildNode = child; cchildNode; cchildNode = cchildNode->next)
                {
                    if (cchildNode->type == XML_ELEMENT_NODE && strcmp((const char*)cchildNode->name, "card") == 0){
                        QuestRewardCardInfo *card = new QuestRewardCardInfo();
                        xmlAttr *xa = NULL;
                        for (xa = cchildNode->properties; xa; xa = xa->next){
                            if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "srl")==0){
                                if (xa->children){
                                    card->card_srl = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "id")==0){
                                if (xa->children){
                                    card->card_id = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "lev")==0){
                                if (xa->children){
                                    card->card_level = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "exp")==0){
                                if (xa->children){
                                    card->card_exp = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "attack")==0){
                                if (xa->children){
                                    card->card_attack = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "defense")==0){
                                if (xa->children){
                                    card->card_defense = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_effect")==0){
                                if (xa->children){
                                    card->card_skillEffect = atoi((const char*)xa->children->content);
                                }
                            }
                            else if (xa->type == XML_ATTRIBUTE_NODE && strcmp((const char*)xa->name, "skill_lev")==0){
                                if (xa->children){
                                    card->card_skillLev = atoi((const char*)xa->children->content);
                                }
                            }
                            
                        }
                        info->rewardCards->addObject(card);
                    }
                    
                }
            }
            
        }
    }
}
// owner : 88306183038747505 // coll  : 88265497908068977

