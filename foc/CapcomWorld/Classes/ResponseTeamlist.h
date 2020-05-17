//
//  ResponseTeamlist.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 15..
//
//

#ifndef CapcomWorld_ResponseTeamlist_h
#define CapcomWorld_ResponseTeamlist_h

#include "ResponseBasic.h"


class ResponseDeckinfo : public CCObject
{
public:
    //int teamType;
    int idx;
    int cardSrl[5];
};

class ResponseTeamlist : public ResponseBasic
{
    
public:
    //ResponseDeckinfo _deck[4];
    CCArray *_deck;
};

#endif
