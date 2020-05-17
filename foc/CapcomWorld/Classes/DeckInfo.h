//
//  DeckInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#ifndef __CapcomWorld__DeckInfo__
#define __CapcomWorld__DeckInfo__

#include <iostream>

#include "CardInfo.h"
#include "ResponseTeamlist.h"

using namespace cocos2d;


class DeckInfo
{
public:
    //CardInfo *AttackDeck;
    //CardInfo *DefenceDeck;
    
    //CCArray *attackDeck;
    //CCArray *defenceDeck;
    
    CardInfo* atkDeck1[5];
    CardInfo* atkDeck2[5];
    CardInfo* atkDeck3[5];
    CardInfo* atkDeck4[5];
    CardInfo* atkDeck5[5];
    
    CardInfo* defDeck1[5];
    
    /*
    CCArray *atkDeck1;
    CCArray *atkDeck2;
    CCArray *atkDeck3;
    CCArray *atkDeck4;
    
    CCArray *defDeck1;
    */
    DeckInfo();
    ~DeckInfo();
    
    void SetCardToDeck(int _team, int _id, int _n, CardInfo *card);
    void SetCardToDeck(ResponseDeckinfo *info);
    void EmptyDeck(int _team, int _id);

    //CCArray *GetAckDeck();
    //CCArray *GetDefDeck();
    
};

#endif /* defined(__CapcomWorld__DeckInfo__) */
