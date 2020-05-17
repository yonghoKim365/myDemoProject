//
//  ChapterLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

#ifndef __CapcomWorld__ChapterLayer__
#define __CapcomWorld__ChapterLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "StageLayer.h"

using namespace cocos2d;
using namespace std;

#define CHAPTER_HEIGHT (177)

typedef struct : public cocos2d::CCObject
{
    int     ChapterNum;
    string  ChapterImgPath;
    string  ChapterTitle;
    string  ChapterDescription;
}Quest_Chapter;

class QuestStory : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    QuestStory(CCSize layerSize, Quest_Data* _questInfo, int _chapter);
    ~QuestStory();
    
    void InitUI();
    void InitText();

    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
    
    pthread_t threads;
    void initThread();
    static void *threadAction(void *threadid);
    void threadCallBack();
    
    static QuestStory* getInstance();
    
private:
    
    Quest_Data* questInfo;
    int chapter;
    static QuestStory* instance;
    
    char key1[30];
    char key2[30];
    char key3[30];
    char key4[30];
    
    const char* text1;
    const char* text2;
    const char* text3;
    const char* text4;
};

class ChapterLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    ChapterLayer(CCSize layerSize);
    ~ChapterLayer();
    
    void InitLayerSize(CCSize layerSize);
    void InitUI();
    void SetChapterData(CCArray *questLocalList, CCArray* questServerList);
    
    void LockChapterCell(QuestInfo* pChapter, int tag);
    void MakeChapterCell(QuestInfo* pChapter, int tag);
    
    void InitStagelayer(QuestInfo* info);
    
    int GetCountOfChapter() const;
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void scrollingEnd();
    /*
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
*/
    float LayerStartPos;
    
    static ChapterLayer* getInstance();
    
    QuestInfo* curChapter;
    
    int StageIndex;
    bool popupTrigger;
    
    CCArray* getUnlockChapterList() { return UnlockChapterList; }
    int getUnlockChapterCount()     { return UnlockChapterList->count(); }
    
    ResponseQuestListInfo* stageList;
    
    void loadQuestStory();
    void onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    
private:
   
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    bool bTouchMove;

    CCArray* QuestLocalList;
    CCArray* unlockedQuestList;
    
    CCArray* ChapterList;
    
    CCArray* LockStageList;
    CCArray* UnLockStageList;
    CCArray* UnlockChapterList;
    
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;

    
    float StartYPos;
    float LockStartYPos;
    
    StageLayer* pStageLayer;
    
    static ChapterLayer* instance;
    
    QuestStory* questStory;
    
    Quest_Data* questInfo;
    
};


#endif /* defined(__CapcomWorld__ChapterLayer__) */
