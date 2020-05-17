//
//  QuestInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#ifndef __CapcomWorld__QuestInfo__
#define __CapcomWorld__QuestInfo__

#include <iostream>
#include "cocos2d.h"

class QuestInfo : public cocos2d::CCObject
{
public:
    int questID;
    int q_sp;
    const char *q_name;
    int bUnlock;
    
    int q_ranks;
    int q_ranka;
    int q_rankb;
    int q_rankc;
    int questBP;
    /*
    int questClass;
    int questPeriodTime;
    int contition;
    int nInfo;
    
    int progress;
    int replay;
    int replayTime;
    int q_exp;
    int q_coin;
    int q_cardprob;
    int q_cardnum;
    int q_card1;
    int q_card2;
    int q_card3;
    int q_item1;
    int q_item2;
    
    */
    int lockState; // 0 == lock, 1 == unlock
    
    
    
};

#endif /* defined(__CapcomWorld__QuestInfo__) */

