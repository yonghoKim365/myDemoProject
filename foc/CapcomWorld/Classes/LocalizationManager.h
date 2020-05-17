//
//  LocalizationManager.h
//  CapcomWorld
//
//  Created by changmin on 12. 10. 9..
//
//

#ifndef CapcomWorld_LocalizationManager_h
#define CapcomWorld_LocalizationManager_h

#include "cocos2d.h"
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>


class LocalizationManager
{
public:
    static LocalizationManager* getInstance();

    LocalizationManager();
    ~LocalizationManager();
    
    bool init();
    
    const char *get(const char *string);
private:
    
    class Node : public cocos2d::CCObject
    {
    public:
        std::string text;
    };
    
    static LocalizationManager *instance;
    cocos2d::CCDictionary *dictionary;
    
    bool parse();
    void parseXml(xmlNode *node);
    
    const char *adjust(const char *input);
};

#endif
