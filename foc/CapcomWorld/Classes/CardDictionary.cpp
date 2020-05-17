//
//  CardDictionary.cpp
//  CapcomWorld
//
//  Created by changmin on 12. 10. 9..
//
//

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "CardDictionary.h"
#include "CCHttpRequest.h"

#define CARD_DATABASE_FILENAME      "cards.xml"


using namespace cocos2d;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

CardDictionary *CardDictionary::instance = NULL;

CardDictionary *CardDictionary::sharedCardDictionary()
{
    if (!instance)
        instance = new CardDictionary();
    return instance;
}

CardDictionary::CardDictionary()
{
    dictionary = new CCDictionary;
}

CardDictionary::~CardDictionary()
{
    CC_SAFE_DELETE(dictionary);
}

bool CardDictionary::init()
{
    if (parse() == false)
        return false;
    recalcExpCap();
    return true;
}

CardInfo *CardDictionary::getInfo(int cardId)
{
    assert(dictionary != NULL);
    return (CardInfo*)dictionary->objectForKey(cardId);
}

void CardDictionary::recalcExpCap()
{
    CCArray *cards = dictionary->allKeys();
    for (int scan = 0;scan < cards->count();scan++)
    {
        CCInteger *integer = (CCInteger*) cards->objectAtIndex(scan);
        CardInfo *info = getInfo(integer->getValue());
        if (info)
            info->recalcExpCap();
    }
}

bool CardDictionary::parse()
{
    unsigned long length = 0;
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(CARD_DATABASE_FILENAME);
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return false;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, CARD_DATABASE_FILENAME, NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    parseXml(root_element, NULL);
    xmlFreeDoc(doc);
    CC_SAFE_DELETE_ARRAY(data);
    return true;
}

void CardDictionary::parseXml(xmlNode * node, CardInfo *parentInfo)
{
    xmlNode *currentNode = NULL;
    CardInfo *info = parentInfo;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {

        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            info = new CardInfo;
//            memset(info, 0, sizeof(CardInfo));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_code") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
            {
                info->setId(atoi((const char*)child->content));
                //info->SetFusionLevel(info->getId()%10);
                dictionary->setObject(info, info->getId());
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_name") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setName((const char*)child->content);
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_series") == 0)
        {
            xmlNode *child = currentNode->children;
            if (child){
                info->series = atoi((const char*)child->content);
                //CCLog("info->series %d", info->series);
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_grade") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child){
                info->setRare(atoi((const char*)child->content));
            }
            
            if (info->getRare()==100)info->setRare(0);
            
        }
//        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_training") == 0)
//        {
//            // 해당 카드의 단련 가능 여부
//            xmlNode *child = currentNode->children;
//            if (child){
//                if (atoi((const char*)child->content) == 0)info->bTrainging = false;
//                else info->bTrainging = true;
//            }
//        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_training_meterial") == 0)
        {
            // 1 == 단련 재료로 사용할수 있는 카드
            xmlNode *child = currentNode->children;
            if (child){
                if (atoi((const char*)child->content) == 0)info->bTraingingMaterial = false;
                else {
                    info->bTraingingMaterial = true;
                    info->setRare(0);
                }
            }
        }
//        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_training_exp") == 0)
//        {
//            // 단련시 획득 경험치
//            xmlNode *child = currentNode->children;
//            if (child){
//                info->training_exp = atoi((const char*)child->content);
//            }
//        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_lv") == 0)
        {
            // 레벨 디폴트값
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setLevel(atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_attribute") == 0)
        {
            // 1 == 타격, 2 == 방어, 3 == 잡기
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setAttribute(atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_atk") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
            {
                info->setAttack(atoi((const char*)child->content));
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_hp") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
            {
                info->setDefence(atoi((const char*)child->content));
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_blt") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
            {
                info->setBp(atoi((const char*)child->content));
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_skill") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
            {
                info->setSkillType(atoi((const char*)child->content));
            }
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_skillplus") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setSkillPlus(atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_fusion") == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setFusionCount(atoi((const char*)child->content));
            //info->SetFusionLevel(atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_sell") == 0)
        {
            // 판매 가능한 카드 
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->enableToSell((child->content[0] == '0') ? false : true);
        }
        ///////////////////////////
        /*
        else if (currentNode->type == XML_ELEMENT_NODE && strncmp((const char*)currentNode->name, "c_exp_", 6) == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setExpCap(atoi((const char*)&currentNode->name[6]), atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strncmp((const char*)currentNode->name, "c_texp_", 7) == 0)
        {
            assert(info != NULL);
            xmlNode *child = currentNode->children;
            if (child)
                info->setTrainingExp(atoi((const char*)&currentNode->name[7]), atoi((const char*)child->content));
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_sellvalue") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            if (child)
                info->setPrice(atoi((const char*)child->content));
        }*//*
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_1") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_2") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_3") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_4") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_5") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_6") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_7") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_8") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_9") == 0)
        {
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "c_svitality_10") == 0)
        {
        }*/ 
        
        parseXml(currentNode->children, info);
    }
}
