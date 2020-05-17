//
//  BattleLogListLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 1..
//
//

#ifndef __CapcomWorld__BattleLogListLayer__
#define __CapcomWorld__BattleLogListLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ResponseBasic.h"
#include <time.h>
#include "ACardMaker.h"

using namespace std;

enum IconType
{
    eWIN = 0,
    eLOSE,
    eDRAW,
    eINVITE,
    eTRADE,
    eGIFT,    
};

typedef struct : public cocos2d::CCObject
{
    int         LogType;
    std::string Name;
    std::string Time;
}tempLog;

class ResultBattleLog : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ResultBattleLog(CCSize layerSize);
    ~ResultBattleLog();
    
    void InitUI(ResponseDetailNoticeInfo *detailInfo);
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

private:
    ACardMaker *cardMaker;
    
};

class BattleLogListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    BattleLogListLayer(CCSize layerSize);
    ~BattleLogListLayer();
    
    void InitUI();
    void InitLayerSize(CCSize layerSize);
    
    void MakeCell(IconType logType, const string& name, const string& time, int tag);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    
    int GetNumOfLog() const;

    void InitResultLog(ResponseDetailNoticeInfo *detailInfo);
    void RemoveResultLog();
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
	    
    float LayerStartPos;
    
    static BattleLogListLayer *getInstance();
    
    ResponseNoticeInfo* noticeInfo;
    
private:

    static BattleLogListLayer *instance;

    int StartYpos;
    
    float yStart;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    ResultBattleLog* resultLog;

};

#endif /* defined(__CapcomWorld__BattleLogListLayer__) */
