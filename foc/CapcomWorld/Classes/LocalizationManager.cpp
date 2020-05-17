//
//  LocalizationManager.cpp
//  CapcomWorld
//
//  Created by changmin on 12. 10. 9..
//
//

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "LocalizationManager.h"
#include "CCHttpRequest.h"

#define DIC_FILENAME      "dic.xml"


using namespace cocos2d;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

LocalizationManager *LocalizationManager::instance = NULL;

LocalizationManager *LocalizationManager::getInstance()
{
    if (!instance)
        instance = new LocalizationManager();
    return instance;
}

LocalizationManager::LocalizationManager()
{
    dictionary = new CCDictionary;
}

LocalizationManager::~LocalizationManager()
{
    CC_SAFE_DELETE(dictionary);    
}

bool LocalizationManager::init()
{
    if (parse() == false)
        return false;
    return true;
}

const char *LocalizationManager::get(const char *string)
{
    assert(dictionary != NULL);
    Node *node = ((Node*)dictionary->objectForKey(string));
    assert(node != NULL);
    return (const char *)node->text.c_str();
}

bool LocalizationManager::parse()
{
    unsigned long length = 0;
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(DIC_FILENAME);
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return false;
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, DIC_FILENAME, NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    parseXml(root_element);
    xmlFreeDoc(doc);
    CC_SAFE_DELETE_ARRAY(data);
    return true;
}

void LocalizationManager::parseXml(xmlNode * node)
{
    xmlNode *currentNode = NULL;
    std::string id;
    for (currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "id") == 0)
        {
            xmlNode *child = currentNode->children;
            if (!child)
                continue;
            id = (const char*)child->content;
        }
        else if (currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "desc") == 0)
        {
            xmlNode *child = currentNode->children;
            if (!child)
                continue;
            Node *node = new Node;
            node->text = adjust((const char*)child->content);
            dictionary->setObject(node, id.c_str());
        }
        parseXml(currentNode->children);
    }
}

const char *LocalizationManager::adjust(const char *input)
{
    std::string text = input;
    do {
        int pos = text.find("\\n");
        if (pos >= text.length())
            return text.c_str();
        text = text.replace(pos, 2, "\n");
    } while (1);
    return text.c_str();
}
