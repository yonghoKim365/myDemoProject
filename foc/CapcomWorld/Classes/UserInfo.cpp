//
//  UserInfo.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 23..
//
//

#include "UserInfo.h"

#define USERSTAT_DATABASE_FILENAME      "userstat.xml"

//bool UserInfo::parsed = false;
//int UserInfo::expCap[USERSTAT_EXP_NUM];

UserInfo::UserInfo()
{
    myNickname = "Capcom";
    //profileImageUrl = NULL;
    numOfKakaoFriends = 0;
    numOfKakaoAppFriends = 0;
    battlePoint = 0;
    //defenceBP= 0;
    maxBattlePoint= 0;
    //maxDefensePoints= 0;
    revengePoint= 0;
    backgroundID= 0;
    upgradePoint= 0;
    staminaPoint= 0;
    maxStaminaPoint= 0;
    //parse();
}

UserInfo::~UserInfo()
{
    
}
/*
void UserInfo::parse()
{
    if (parsed == true)
        return;
    unsigned long length = 0;
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(USERSTAT_DATABASE_FILENAME);
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, USERSTAT_DATABASE_FILENAME, NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    parseXml(root_element);
    xmlFreeDoc(doc);
    CC_SAFE_DELETE_ARRAY(data);
//    recalcExpCap();
    parsed = true;
}

void UserInfo::parseXml(xmlNode * node)
{
    static int level = 0;
    xmlNode *currentNode = NULL;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "Level") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child)
                level = (atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "Exp") == 0)
        {
            xmlNode *child = currentNode->children;
            int exp = 0;
            if (child)
            {
                exp = atoi((const char*)child->content);
                expCap[level] = exp;
            }
        }
        parseXml(currentNode->children);
    }
}

void UserInfo::recalcExpCap()
{
    for (int scan = USERSTAT_EXP_NUM - 1;scan > 1;scan--)
    {
        int value = 0;
        for (int loop = scan;loop > 0;loop--)
        {
            value += expCap[loop];
        }
        expCap[scan] = value;
    }
}
 */

void UserInfo::setUserInfo(ResponseLoginInfo *info){
    this->setLevel(info->getLevel());
    this->setXp(info->getXp());
    this->setMaxXp(info->getMaxXp());
    this->setCoin(info->getCoin());
    this->setCash(info->getCash());
    this->setStamina(info->getQuestPoint());
    this->setMaxStamina(info->getMaxQuestPoint());
    this->setBattlePoint(info->getAttackPoint());
    //this->setDefensePoint(info->getDefensePoint());
    this->setBackground(info->getBackground());
    this->setRevengePoint(info->getRevengePoint());
    this->setUpgradePoint(info->getUpgradePoint());
    this->setMaxBattlePoint(info->getMaxAttackPoint());
    //this->setMaxDefencePoint(info->getMaxDefensePoint());
    this->setFame(info->getFame());
    this->setTutorial(info->getTutorial());
    this->setBattleCnt(info->getBattleCnt());
    this->setVictoryCnt(info->getVictoryCnt());
    this->setDrawCnt(info->getDrawCnt());
    this->setRanking(info->getRanking());
}