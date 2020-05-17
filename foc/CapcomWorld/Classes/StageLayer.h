//
//  StageLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

#ifndef __CapcomWorld__StageLayer__
#define __CapcomWorld__StageLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "QuestFullScreen.h"
#include "QuestLayer.h"
#include "FileManager.h"
#include "CCHttpRequest.h"
#include "ResponseBasic.h"
#include "QuestInfo.h"
//#include "PopUp.h"

using namespace cocos2d;
using namespace std;

#define STAGE_HEIGHT (200) //(382)

class BasePopUP;

class StageLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    StageLayer();
    ~StageLayer();
    
    static StageLayer* getInstance();

    void InitLayerSize(CCSize layerSize);
    void InitUI();
    void SetStageData(CCArray *stageList, CCArray* UnlockStage, ResponseQuestListInfo* stageServerList);
    void MakeLockStageCell(QuestInfo* pStage, int tag);
    void MakeStageCell(QuestInfo* pStage, int tag);
    void InitQuestFullScreen();
    void ReleaseAndInitQuestFullScreen();
    
    int GetCountOfStage() const;
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    /*
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    */
    float LayerStartPos;
    
    void onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    void onHttpRequestCompletedForTrace(cocos2d::CCObject* pSender, void* data);
    
    int getQuestID() {return QuestID;};
    int getUnlockStageCount() { return UnlockStageList->count(); }
    
    void BackBtnCallback(CCObject* pSender);
    
    void initPopUp(void *data);
    void removePopUp();
    
    void QuestLoad();
    void QuestStart();
    void QuestStartAction();
    
    QuestInfo *getQuestInfo(int questID);
    ResponseQuestListInfo* questListFromServer;
    
    int lastLayerY;
    
    

private:
    
    static StageLayer* instance;
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;
    
    CCArray* StageList;
    CCArray* UnlockStageList;
    
    float StartYPos;
    float UnlockStartYPos;
    
    //QuestFullScreen* pQuestFullScreen;
    
    //QuestLayer* pQuestLayer;
    
    std::vector<string> download_url;
    
    int QuestID;
    int qProgress;
    
    //ACardMaker *cardMaker;
    
    BasePopUP* basePopUp;
    
    AQuestInfo *getAQuestInfo(int questID);

    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    void InitTraceLayer(int questID, int questProgress);
};



#endif /* defined(__CapcomWorld__StageLayer__) */
