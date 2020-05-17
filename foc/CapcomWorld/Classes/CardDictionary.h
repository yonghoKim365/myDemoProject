//
//  CardDictionary.h
//  CapcomWorld
//
//  Created by changmin on 12. 10. 9..
//
//

#ifndef CapcomWorld_CardDictionary_h
#define CapcomWorld_CardDictionary_h

#include "cocos2d.h"
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>
#include "CardInfo.h"


class CardDictionary
{
public:
    static CardDictionary* sharedCardDictionary();

    CardDictionary();
    ~CardDictionary();
    
    bool init();
    
    CardInfo *getInfo(int cardId);
    
    cocos2d::CCArray *getKeys()
    {
        return dictionary->allKeys();
    }
    
private:
    
    static CardDictionary *instance;
    cocos2d::CCDictionary *dictionary;
    
    bool parse();
    void parseXml(xmlNode *node, CardInfo *parentInfo);
    void recalcExpCap();
};

#endif
