//
//  NpcInfo.h
//  CapcomWorld
//
//  Created by yongho Kim on 13. 2. 13..
//
//

#ifndef CapcomWorld_NpcInfo_h
#define CapcomWorld_NpcInfo_h

class NpcInfo : public cocos2d::CCObject
{
public:
    int npcCode;
    int cardCode;
    const char* npcName;
    const char* npcImagePath;
    int normalBattleLimitTime;
    int touchUp;
    int gaugeDrop;
    const char* npcDesc;
    float hp;
    int sendCoin;
};

class QuestNpcInfo : public cocos2d::CCObject
{
public:
    int npcCode;
    int n_stage_min;
    int n_stage_max;
};

#endif
