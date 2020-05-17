//
//  TraceDetailLayer.h
//  CapcomWorld
//
//  Created by APD_MAD on 13. 2. 14..
//
//

#ifndef __CapcomWorld__TraceDetailLayer__
#define __CapcomWorld__TraceDetailLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ResponseBasic.h"
#include "TraceHistoryLayer.h"
#include "CardDetailViewLayer.h"

using namespace cocos2d;
using namespace std;

const float RIVAL_HP_GAUGE_LENGTH = 475.0f;

class Time;

class TraceDetailLayer : public CCLayer, MyUtil, GameConst
{
public:
    void init();
    void cbBackBtn();
    void cbRefresh();
    void cbReward(CCObject* pSender);
    void makeColleagueCell(CCLayerColor* layer, AReceivedColleague* pColleague, int yy, int tag);
    void cbDetail();
    void timeCounter();
    
    CCSprite* rivalHp[10];
    CCSprite* rivalMaxHp[10];
    void refreshRivalHp(CCLayerColor* layer, int yy);
    void refreshRivalMaxHp(CCLayerColor* layer, int yy);
    void removeRivalHp(CCLayerColor* layer);
    void removeRivalMaxHp(CCLayerColor* layer);
    CCSprite* rivalHpGauge[2];
    void loadRivalHpGauge(CCLayerColor* layer);
    void refreshRivalHpGauge(CCLayerColor* layer, int yy);
    static int compare(const void* n1, const void* n2);
    void sortDescByLimitTime();
//    void refreshTraceResult();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    static TraceDetailLayer* getInstance()
    {
        if (!instance)
            return NULL;
        return instance;
    }
    
    TraceDetailLayer(AReceivedRival* info, CCSize layerSize)
    {
        this->setContentSize(layerSize);
        instance = this;
        rivalInfo = info;
        
        init();
    }
    
    ~TraceDetailLayer()
    {
        this->removeAllChildrenWithCleanup(true);
        
        delete rivalInfo;
        delete instance;
    }
    
    float LayerStartPos;
    
    float old_layer_y;
    void callBackFromTraceLayer();
    
    void refresh();
    
    int  showCardCnt;
    void FadeWhite04();
    void RemoveFade04();
    CCArray* rewardCards;
    void showRewardCard();
    CardDetailViewLayer* cardDetailViewLayer;
    void CreateBtn();
    void startCardAction();
    void closeCardAction();
    
    AReceivedRival* getRivalInfo() { return rivalInfo; }
    
private:
    static TraceDetailLayer* instance;
    AReceivedRival* rivalInfo;
    
//    Time* battleRemainTime;
    
    CCPoint touchStartPoint;
    CCPoint touchMovePoint;
    bool bTouchPressed;
    bool bTouchMove;
    CCPoint startPosition;
    CCPoint lastPosition;
    cc_timeval startTime;
    cc_timeval lastTime;
    bool moving;
    
    CocosDenshion::SimpleAudioEngine* soundBG;
    
    enum CARDPACK_TAG
    {
        BLACK_BG = 800,
        CARDPACK,
        CUT_LINE,
        ANI_TOUCH_ICON,
        CARDPACK_LEFT,
        CUT_CARD_1,
        CUT_CARD_2,
        FADE_01,
        FADE_02,
        FADE_03,
        FADE_04,
        
        CARD_CLOSE_BTN,
        LABEL,
        
    };
};

#endif /* defined(__CapcomWorld__TraceDetailLayer__) */
