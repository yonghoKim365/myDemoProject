//
//  TraceHistoryLayer.h
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 12..
//
//

#ifndef __CapcomWorld__TraceHistoryLayer__
#define __CapcomWorld__TraceHistoryLayer__

#include "TraceDetailLayer.h"

#include <iostream>
#include <time.h>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ACardMaker.h"
#include "ResponseBasic.h"

using namespace cocos2d;
using namespace std;

class AReceivedRival;
class Time;

class TraceHistoryMenuLayer : public CCLayer, MyUtil, GameConst
{
public:
    TraceHistoryMenuLayer(CCSize layerSize);
    ~TraceHistoryMenuLayer();
    void init();
    void cbBackBtn(CCObject* pSender);
    void cbReward(CCObject* pSender);
    
    static TraceHistoryMenuLayer* getInstance()
    {
        if (!instance)
            return NULL;
        
        return instance;
    }
private:
    static TraceHistoryMenuLayer* instance;
};

class TraceHistoryScrollLayer : public CCLayer, MyUtil, GameConst
{
public:
    void init();
//    void onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data);
    
    void makeRivalCell(AReceivedRival* pRival, int yy, int tag);
    void battleTimeCounter();
    void makeTimer(time_t limitTime, time_t curTime, int yy, int tag);
    void initTraceDetailLayer(AReceivedRival* rivalInfo);
    static int compare(const void* n1, const void* n2);
    void sortDescByLimitTime();
    
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    float LayerStartPos;
    
    static TraceHistoryScrollLayer* getInstance()
    {
        if (!instance)
            return NULL;
        
        return instance;
    }
    
    TraceHistoryScrollLayer(CCSize layerSize)
    {
        this->setContentSize(layerSize);
        
        instance = this;
        
        init();
    }
    
    ~TraceHistoryScrollLayer()
    {
        this->removeAllChildrenWithCleanup(true);
        
        this->stopAllActions();
        
        //delete rivalListinfo;
        delete pTraceDetailLayer;
    }
    
    TraceDetailLayer* pTraceDetailLayer;
    bool isExistRival;
    
    void callProfileImg(cocos2d::CCObject *pSender, void *data);
    void registerProfileImg(std::string filename);
    class PortraitUrl : public cocos2d::CCObject
    {
    public:
        std::string url;
        int y;
    };
    CCDictionary portraitDic;
private:
    static TraceHistoryScrollLayer* instance;
    ACardMaker* cardMaker;
    
    CCPoint touchStartPoint;
    CCPoint touchMovePoint;
    bool bTouchPressed;
    bool bTouchMove;
    CCPoint startPosition;
    CCPoint lastPosition;
    cc_timeval startTime;
    cc_timeval lastTime;
    bool moving;
    
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    
    virtual void visit();
    
    int cellMakeCnt;
    int cellMakeY;
    void makeCells();
    
    static const int RIVAL_CELL_HEIGHT = 166;
    bool bMakeCellFinished;
    
    void makeCellCaller();
    
    /*
    virtual void preVisitWithClippingRect(CCRect clipRect);
	virtual void postVisit();
     */
    
//    AReceivedRival* rivalInfo;
//    int rivalInfoYy;
//    int rivalInfoNo;
};

class TraceHistoryLayer : public CCLayer, MyUtil, GameConst
{
public:
  
    void init();
    void cbBackBtn();//CCObject* pSender);
    void cbReward();//CCObject* pSender);
    
//    void timeTest();
    void callbackFromDetail();
    
    TraceHistoryScrollLayer *traceHistoryScrollLayer;
    TraceHistoryMenuLayer   *traceHistoryMenuLayer;
    
    
    static TraceHistoryLayer* getInstance()
    {
        if (!instance)
            return NULL;
        
        return instance;
    }
    
    TraceHistoryLayer(CCSize layerSize)
    {
        this->setContentSize(layerSize);
        
        instance = this;
        
        init();
        removePageLoading();
    }
    
    ~TraceHistoryLayer()
    {
        this->removeAllChildrenWithCleanup(true);
        
        //delete rivalListinfo;
        delete traceHistoryScrollLayer;
        delete traceHistoryMenuLayer;
        //delete pTraceDetailLayer;
    }
    
    void closeLayer();
    
    bool refreshRivalListInfo();
    
    ResponseRivalList* rivalListinfo;
    AReceivedRival* getRivalInfo(long long _ownerId, int _rid, int _birth);
    
    
    
    
private:
    static TraceHistoryLayer* instance;
    
    
};

class TextClippingLayer : public CCLayer, MyUtil, GameConst
{
public:
    TextClippingLayer(const char* _text);
    ~TextClippingLayer();
    
    int clip_w;
    const char* text;
    void setClipX(int _w);
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    
};

class Time
{
public:
    int getHour() { return hour; }
    int getMin() { return min; }
    int getSec() { return sec; }
    
    void setHour(int aHour) { hour = aHour; }
    void setMin(int aMin) { min = aMin; }
    void setSec(int aSec) { sec = aSec; }
    
    Time(int aHour, int aMin, int aSec)
    : hour(aHour), min(aMin), sec(aSec)
    {
    }
    
//    ~Time();
    
private:
    int hour;
    int min;
    int sec;
};

#endif /* defined(__CapcomWorld__TraceHistoryLayer__) */