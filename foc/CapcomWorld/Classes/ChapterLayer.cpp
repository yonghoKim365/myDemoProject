//
//  ChapterLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 8..
//
//

#include "ChapterLayer.h"
#include "MainScene.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

ChapterLayer* ChapterLayer::instance = NULL;

ChapterLayer* ChapterLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

ChapterLayer::ChapterLayer(CCSize layerSize) : pStageLayer(NULL), QuestLocalList(NULL), LockStageList(NULL), curChapter(NULL), UnlockChapterList(NULL), UnLockStageList(NULL), questStory(NULL),
stageList(NULL), questInfo(NULL)
{
    this->setTouchEnabled(true);
    //this->setClipsToBounds(true);
    
//    soundMainBG();
    
    bTouchMove = false;
    
    instance = this;
    
    LockStartYPos = 0.0f;
    StartYPos = 0.0f;
    StageIndex = 0;
}

ChapterLayer::~ChapterLayer()
{
    CC_SAFE_DELETE(ChapterList);
    CC_SAFE_DELETE(LockStageList);
    CC_SAFE_DELETE(UnLockStageList);
        
    this->removeAllChildrenWithCleanup(true);
}

void ChapterLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
}

void ChapterLayer::SetChapterData(CCArray *questLocalList, CCArray* questServerList)
{
    // -- 전체 퀘스트 리스트 (챕터 + 스테이지)
    QuestLocalList = questLocalList;
    
    // -- 언락된 리스트
    unlockedQuestList = questServerList;

    ChapterList = new CCArray;

    // 로컬에서 읽어온 전체 리스트 중 챕터 리스트만
    for(int i=0; i<questLocalList->count(); ++i)
    {
        QuestInfo *info = (QuestInfo*)QuestLocalList->objectAtIndex(i);
        
        if(0 == i%5)
        {
            ChapterList->addObject(info);
        }
    }
    
    int ChapterStartId = 20011;
    UnlockChapterList = new CCArray;
/*
    for (int i=0;i<PlayerInfo::getInstance()->questList->count();i++){
        QuestInfo *info = (QuestInfo*)PlayerInfo::getInstance()->questList->objectAtIndex(i);
        if (info->lockState != 0){
            CCLog(" quest id:%d unlocked", info->questID);
        }
    }
*/
    // -- 서버에서 받아온 언락된 챕터 + 스테이지
    for(int k=0; k<unlockedQuestList->count(); ++k)
    {
        QuestInfo* info = (QuestInfo*)unlockedQuestList->objectAtIndex(k);
        
        if(ChapterStartId == info->questID && info->lockState != 0)
        {
            UnlockChapterList->addObject(info);
            
            //CCLOG("언락 챕터 아이디 %d", info->questID);
            ChapterStartId+=10;
        }
    }
}

int ChapterLayer::GetCountOfChapter() const
{
    return ChapterList->count();
}

void ChapterLayer::InitUI()
{
    removeLoadingAni();
    
    CCSize size = this->getContentSize();
    
    LockStartYPos = (size.height) - MAIN_LAYER_TOP_MARGIN - CHAPTER_HEIGHT + 24;
    StartYPos = (size.height) - MAIN_LAYER_TOP_MARGIN - CHAPTER_HEIGHT + 24;
 
/*
 * 잠긴 챕터들은 보이지 않음
    
    const int ChapterNum = ChapterList->count();
    
    for(int k=0; k<ChapterNum; ++k)
    {
        QuestInfo* tempChapter = (QuestInfo*)ChapterList->objectAtIndex(k);
        LockChapterCell(tempChapter, k);
    }
 */
    const int UnlockChapterNum = getUnlockChapterCount();

    for (int i=UnlockChapterNum-1; i>-1; --i)
    {
        QuestInfo* tempChapter = (QuestInfo*)UnlockChapterList->objectAtIndex(i);
        MakeChapterCell(tempChapter, i);
    }
}

/*
void ChapterLayer::LockChapterCell(QuestInfo* pChapter, int tag)
{
    CCSprite* pChapterBG = CCSprite::create("ui/quest/quest_chapter_bg_lock.png");
    pChapterBG->setAnchorPoint(ccp(0,0));
    pChapterBG->setPosition( accp(10, LockStartYPos) );
    this->addChild(pChapterBG, 0);
 
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(pChapter->questID);
    string pathBG = "ui/main_bg/";
    pathBG = pathBG + questInfo->chapterLockImg;
    
    //CCLog("ChapterLayer::LockChapterCell, pathBG:%s",pathBG.c_str() );
    
    CCSprite* pChapterImg = CCSprite::create(pathBG.c_str());
    pChapterImg->setAnchorPoint(ccp(0,0));
    pChapterImg->setPosition( accp(20, LockStartYPos + 10) );
    this->addChild(pChapterImg, 0);
    
    char buffer[32];
    sprintf(buffer, "20%.2d_name", tag + 1);
    std::string text = LocalizationManager::getInstance()->get(buffer);
    text = text + " ";
    CCLabelTTF* pChapterTitle = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pChapterTitle->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(218, LockStartYPos + 118), pChapterTitle, 10);
    
    string chapter = "CHAPTER";
    
    char buff[5];
    sprintf(buff, "%d", tag+1);
    string chapterNum = buff;

    chapter = chapter + chapterNum;
    
    CCLabelTTF* pChapterNumber = CCLabelTTF::create(chapter.c_str(), "HelveticaNeue-Bold", 12);
    pChapterNumber->setColor(COLOR_GRAY);
    registerLabel( this,ccp(0, 0), accp(26, LockStartYPos + 132), pChapterNumber, 10);
    
    sprintf(buffer, "20%.2d_msg", tag + 1);
    text = LocalizationManager::getInstance()->get(buffer);
    CCLabelTTF* pChapterDesc = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 11);
    pChapterDesc->setColor(COLOR_WHITE);
    pChapterDesc->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(222, (text.find("\n") < text.length()) ? LockStartYPos + 28 : LockStartYPos + 63), pChapterDesc, 110);

    CCSprite* pEnterBtn = CCSprite::create("ui/quest/quest_btn_lock.png");
    pEnterBtn->setAnchorPoint(ccp(0,0));
    pEnterBtn->setPosition( accp(527, LockStartYPos + 17));
    this->addChild(pEnterBtn, 15);
    
    CCLabelTTF* pEnterLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("questlock_btn"), "HelveticaNeue-Bold", 12);
    pEnterLabel->setColor(COLOR_GRAY);
    registerLabel( this,ccp(0, 0), accp(550, LockStartYPos + 42), pEnterLabel, 17);
    
    LockStartYPos = LockStartYPos - CHAPTER_HEIGHT - 10;    
}
 */

/*
?xml version="1.0" encoding="utf-8"?><response><res>0</res><message></message>
<quests>
<quest id="20011" begin="1360121910" end="1360892740" progress="846" clear="1" max_progress="999" enemy="0"></quest>
<quest id="20012" begin="1360180906" end="0" progress="170" clear="0" max_progress="999" enemy="0"></quest>
<quest id="20013" begin="1360668273" end="0" progress="212" clear="0" max_progress="999" enemy="0"></quest>
<quest id="20014" begin="1360676192" end="0" progress="121" clear="0" max_progress="999" enemy="0"></quest>
<quest id="20015" begin="1360770144" end="0" progress="106" clear="0" max_progress="999" enemy="0"></quest>
<quest id="20021" begin="1360857765" end="0" progress="150" clear="0" max_progress="999" enemy="0"></quest>
<quest id="20022" begin="1360869787" end="0" progress="80" clear="0" max_progress="999" enemy="0"></quest>
</quests>
</response>
*/
void ChapterLayer::MakeChapterCell(QuestInfo* pChapter, int tag)
{
    CCSprite* pChapterBG = CCSprite::create("ui/quest/quest_chapter_bg.png");
    pChapterBG->setAnchorPoint(ccp(0,0));
    pChapterBG->setPosition( accp(10, StartYPos) );
    this->addChild(pChapterBG, 100);
  
    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(pChapter->questID);
    string pathBG = "ui/quest_story/";
    pathBG = pathBG + questInfo->chapterUnLockImg;

    CCSprite* pChapterImg = CCSprite::create(pathBG.c_str());
    pChapterImg->setAnchorPoint(ccp(0,0));
    pChapterImg->setPosition( accp(20, StartYPos + 10) );
    this->addChild(pChapterImg, 100);

    char buffer[32];
    sprintf(buffer, "20%.2d_name", tag + 1);
    std::string text = LocalizationManager::getInstance()->get(buffer);
    text = text + " ";
    CCLabelTTF* pChapterTitle = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pChapterTitle->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(218, StartYPos + 118), pChapterTitle, 110);
    
    string chapter = "CHAPTER";
    
    char buff[5];
    sprintf(buff, "%d", tag+1);
    string chapterNum = buff;
    
    chapter = chapter + chapterNum;
    CCLabelTTF* pChapterNumber = CCLabelTTF::create(chapter.c_str(), "HelveticaNeue-Bold", 12);
    pChapterNumber->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(26, StartYPos + 132), pChapterNumber, 110);
    
    
    sprintf(buffer, "20%.2d_msg", tag + 1);
    text = LocalizationManager::getInstance()->get(buffer);
    CCLabelTTF* pChapterDesc = CCLabelTTF::create(text.c_str(), "HelveticaNeue", 11);
    pChapterDesc->setColor(COLOR_GRAY);
    pChapterDesc->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0, 0), accp(222, (text.find("\n") < text.length()) ? StartYPos + 28 : StartYPos + 63), pChapterDesc, 110);
    
    
    CCSprite* pEnterBtn = CCSprite::create("ui/quest/quest_btn_a1.png");
    pEnterBtn->setAnchorPoint(ccp(0,0));
    pEnterBtn->setPosition( accp(527, StartYPos + 17));
    pEnterBtn->setTag(tag);
    this->addChild(pEnterBtn, 111);
    
    CCLabelTTF* pEnterLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("chapter_btn"), "HelveticaNeue-Bold", 12);
    pEnterLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(550, StartYPos + 42), pEnterLabel, 112);
    
    StartYPos = StartYPos - CHAPTER_HEIGHT - 10;
}

void ChapterLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    const int ChapterCount = UnlockChapterList->count();

    for(int i=0; i<ChapterCount; ++i)
    {
        //if(false == bTouchMove)
        {
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                ChangeSpr(this, i, "ui/quest/quest_btn_a2.png", 100);
            }
        }
    }
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void ChapterLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float y = this->getPositionY();

#if (1)
    if (moving == true)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos > 0)
            endPos = 0;
        else if (endPos < LayerStartPos)
            endPos = LayerStartPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ChapterLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    const int ChapterCount = UnlockChapterList->count();
    
    for(int i=0; i<ChapterCount; ++i)
    {
        ChangeSpr(this, i, "ui/quest/quest_btn_a1.png", 100);
        
        if(false == bTouchMove)
        {
            if (GetSpriteTouchCheckByTag(this, i, localPoint))
            {
                soundButton1();
                
                this->stopAllActions();
                
                StageIndex = i;
                
                /*
                 * 퀘스트 스토리 종료 시점(ccTouchesEnded)에 스테이지를 요청했던 것을
                 * 챕터 종료 시점(ccTouchesEnded)에 스테이지를 요청하도록 하고 그 안에서 퀘스트 스토리 팝업
                 *
                 * 다만, 트리거로 챕터 종료 시점(ccTouchesEnded)에서는 항상 퀘스트 스토리 팝업하도록 설정
                 */
                //loadQuestStory();
                popupTrigger = true;
                ARequestSender::getInstance()->requestStageList();
            }
        }
    }

    bTouchMove = false;
}

void ChapterLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    float distance = fabs(startPosition.y - location.y);
    if (distance > 5.0f)
        bTouchMove = true;
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
    if (distance < 15.0f && timeDelta > 50.0f)
    {
        moving = false;
        startPosition = location;
        startTime = currentTime;
    }
    else if (distance > 5.0f)
        moving = true;
    if (moving)
    {
        distance = fabs(lastPosition.y - location.y);
        timeDelta = (currentTime.tv_usec - lastTime.tv_usec) / 1000.0f + (currentTime.tv_sec - lastTime.tv_sec) * 1000.0f;
        if (distance < 15.0f && timeDelta > 50.0f)
        {
            moving = false;
            startPosition = location;
            startTime = currentTime;
        }
    }
    
    lastPosition = location;
    lastTime = currentTime;
    
    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("챕터 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;
    
}

void ChapterLayer::loadQuestStory()
{
    addPageLoading();
    
    bool startQuestStory = true;
   
    const int stardID = 20011 + (10 * StageIndex);
    
    for(int i=stardID; ; ++i)
    {
        questInfo = FileManager::sharedFileManager()->GetQuestInfo(i);
        if(1 == questInfo->level) break;
    }
    
    FileManager* fmanager = FileManager::sharedFileManager();
    std::string basePathL = FOC_IMAGE_SERV_URL;
    basePathL.append("images/cha/cha_l/");
    
    std::string basePathBG = FOC_IMAGE_SERV_URL;
    basePathBG.append("images/bg/");
    
    std::vector<std::string> downloads;
    
    if(0 != questInfo->questEnemy3[0])
    {
        if(!fmanager->IsFileExist(questInfo->questEnemy3.c_str()))
        {
            startQuestStory = false;
            
            string downPath = basePathL + questInfo->questEnemy3;
            downloads.push_back(downPath);
        }
    }

    if(0 != questInfo->stageBG_L[0])
    {
        if(!fmanager->IsFileExist(questInfo->stageBG_L.c_str()))
        {
            startQuestStory = false;
            
            string downPath = basePathBG + questInfo->stageBG_L;
            downloads.push_back(downPath);
        }
    }
    
    if(false == startQuestStory)
    {
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        requestor->addDownloadTask(downloads, this, callfuncND_selector(ChapterLayer::onHttpRequestCompleted));
    }
    else
    {
        removePageLoading();
        
        questStory = new QuestStory(this->getContentSize(), questInfo, StageIndex);
        questStory->setAnchorPoint(ccp(0.0f, 0.0f));
        questStory->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(questStory, 10000);
    }
}

void ChapterLayer::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("quest story image download complete");
        
        removePageLoading();
        
        questStory = new QuestStory(this->getContentSize(), questInfo, StageIndex);
        questStory->setAnchorPoint(ccp(0.0f, 0.0f));
        questStory->setPosition(accp(0.0f, 0.0f));
        MainScene::getInstance()->addChild(questStory, 10000);        
    }
}


void ChapterLayer::scrollingEnd()
{
    this->stopAllActions();
}

void ChapterLayer::InitStagelayer(QuestInfo* info)
{
    //this->setClipsToBounds(false);
        
    if (pStageLayer != NULL)
    {
        this->removeChild(pStageLayer, true);
        pStageLayer = NULL;
    }

    CCLOG("챕터  %d", info->questID);
    
    LockStageList = new CCArray;
    
    const int startStageID = info->questID;
    
    // -- 전체 리스트 중 선택된 챕터 ID와 체크 후
    for(int i=0; i<QuestLocalList->count(); ++i)
    {
        QuestInfo* qInfo = (QuestInfo*)QuestLocalList->objectAtIndex(i);
            
        // 현재 챕터 ID에서 +10한 값보다 작으면 스테이지 리스트에 넣는다
        if(startStageID <= qInfo->questID && startStageID + 10 > qInfo->questID)
        {
            CCLOG("스테이지 id : %d", qInfo->questID);
            LockStageList->addObject((QuestInfo*)QuestLocalList->objectAtIndex(i));
        }
    }
    
    UnLockStageList = new CCArray;
    
    // -- 언락 스테이지
    for(int k=0; k<unlockedQuestList->count(); ++k)
    {
        QuestInfo* qInfo = (QuestInfo*)unlockedQuestList->objectAtIndex(k);
        
        if(startStageID <= qInfo->questID && startStageID + 10 > qInfo->questID && qInfo->lockState != 0)
        {
            UnLockStageList->addObject(qInfo);
            CCLOG("언락 스테이지 id : %d", qInfo->questID);
        }
    }
    
    pStageLayer = new StageLayer();
    
    pStageLayer->SetStageData(LockStageList, UnLockStageList, this->stageList);
    
    const int unlockCount = pStageLayer->getUnlockStageCount();
    const float LayerHeight = STAGE_HEIGHT * unlockCount + 10 * unlockCount + 130;
    
    pStageLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
    
    pStageLayer->setAnchorPoint(ccp(0, 0));
    
    pStageLayer->LayerStartPos = (748 - LayerHeight)/SCREEN_ZOOM_RATE;

    pStageLayer->setPosition(accp(0, 748 - LayerHeight));
    
    pStageLayer->InitUI();
    
    this->addChild(pStageLayer, 100);
}

QuestStory::QuestStory(CCSize layerSize, Quest_Data* _questInfo, int _chapter) : questInfo(NULL)
{
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    this->setClipsToBounds(true);
    
    setDisableWithRunningScene();
    
    questInfo = _questInfo;
    chapter = _chapter;
    instance = this;
    InitUI();
}

QuestStory::~QuestStory()
{
    this->removeAllChildrenWithCleanup(true);
}

QuestStory* QuestStory::instance = NULL;

QuestStory* QuestStory::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}


void QuestStory::InitUI()
{
    //CCLog("QuestStory::InitUI() 0 ");
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    // ios : size = 320 * 480
    
    CCSprite* pSpr = CCSprite::create("ui/quest_story/quest_story_bg.png");//"ui/quest/quest_intro_frame.png");
    pSpr->setAnchorPoint(ccp(0.0f, 0.0f));
    pSpr->setPosition(accp(0.0f, 0.0f));
    pSpr->setTag(0);
    this->addChild(pSpr, 100);
    
    char buff[10];
    sprintf(buff, "%d", chapter + 1);
    string count = buff;
    string chapter = "CHAPTER";
    chapter = chapter + " " + count;
    CCLabelTTF* labelChapter = CCLabelTTF::create(chapter.c_str(), "HelveticaNeue-BoldItalic", 14);
    labelChapter->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.5f, 0.0f), ccp(size.width/2, accp(size.height*SCREEN_ZOOM_RATE - 65.0f)), labelChapter , 110);
    
    string chapterImgPath = "ui/quest_story/quest_story_img0";
    chapterImgPath.append(buff).append(".png");
    CCSprite* pSprChapterImg = CCSprite::create(chapterImgPath.c_str());
    pSprChapterImg->setAnchorPoint(ccp(0.5f, 1.0f));
    pSprChapterImg->setPosition(ccp(size.width/2.0f, size.height - accp(78.0f)));
    pSprChapterImg->setTag(3);
    this->addChild(pSprChapterImg, 100);
    
/*
    string pathBG = CCFileUtils::sharedFileUtils()->getDocumentPath();
    pathBG+=questInfo->stageBG_L;
    CCSprite* bg = CCSprite::create(pathBG.c_str());
    bg->setAnchorPoint(ccp(0.0f, 0.0f));
    bg->setPosition(accp(20.0f, 444.0f));
    bg->setScale(1.2f);
    bg->setTag(3);
    this->addChild(bg, 0);

    CCSprite* top = CCSprite::create("ui/quest/quest_intro_bg_up.png");
    top->setAnchorPoint(ccp(0.0f, 0.0f));
    top->setPosition(accp(22.0f, 786.0f));
    top->setTag(1);
    this->addChild(top, 11);
 
    string pathCha = CCFileUtils::sharedFileUtils()->getDocumentPath();
    pathCha+=questInfo->questEnemy3;
    CCSprite* cha = CCSprite::create(pathCha.c_str());
    cha->setAnchorPoint(ccp(0.0f, 0.0f));
    cha->setPosition(accp(0.0f, 212.0f));
    cha->setScale(1.3f);
    cha->setTag(4);
    this->addChild(cha, 12);

    CCSprite* bottom = CCSprite::create("ui/quest/quest_intro_bg_down.png");
    bottom->setAnchorPoint(ccp(0.0f, 0.0f));
    bottom->setPosition(accp(21.0f, 92.0f));
    bottom->setTag(2);
    this->addChild(bottom, 13);
*/
    CCSprite* btn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    btn->setAnchorPoint(ccp(0.0f, 0.0f));
    btn->setPosition(accp(167.0f, 28.0f));
    btn->setTag(10);
    this->addChild(btn, 100);
    
    CCLabelTTF* questStart = CCLabelTTF::create("퀘스트 시작하기", "HelveticaNeue-Bold", 12);
    questStart->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.5f, 0.0f), ccp(size.width/2, accp(38.0f)), questStart , 110);

    
    //char key1[20];
    sprintf(key1, "questStorytitle_%d", this->chapter);
    
    //char key2[20];
//    sprintf(key2, "questStorySubtitle_%d", this->chapter);
    
    //char key3[20];
    sprintf(key3, "questStory_%d", this->chapter);
    
    sprintf(key4, "questStory_%d_1", this->chapter);
    /*
    const char* tmp1 = LocalizationManager::getInstance()->get(key1);
    const char* tmp2 = LocalizationManager::getInstance()->get(key2);
    const char* tmp3 = LocalizationManager::getInstance()->get(key3);
    */
    
    initThread();
    
    /*
    CCLabelTTF* title = CCLabelTTF::create(LocalizationManager::getInstance()->get(key1), "HelveticaNeue-Bold", 14);
    title->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0.0f, 0.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 500.0f), title , 110);
    
    CCLabelTTF* subtitle = CCLabelTTF::create(LocalizationManager::getInstance()->get(key2), "HelveticaNeue-Bold", 12);
    subtitle->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.0f, 0.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 535.0f), subtitle , 110);
    
    CCLabelTTF* desc = CCLabelTTF::create(LocalizationManager::getInstance()->get(key3), "HelveticaNeue", 12);
    desc->setColor(subBtn_color_normal);
    desc->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0.0f, 0.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 845.0f), desc , 110);
     */
    
    
    
    
}

void QuestStory::InitText()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCLabelTTF* title = CCLabelTTF::create(text1, "HelveticaNeue-Bold", 14);
    title->setColor(ccc3(255.0f, 188.0f, 0.0f));//COLOR_ORANGE);
    registerLabel( this,ccp(0.0f, 0.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 500.0f), title , 110);
/*
    CCLabelTTF* subtitle = CCLabelTTF::create(text2, "HelveticaNeue-Bold", 12);
    subtitle->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.0f, 0.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 535.0f), subtitle , 110);
*/    
    CCLabelTTF* desc1 = CCLabelTTF::create(text3, "HelveticaNeue", 12);
    desc1->setColor(COLOR_WHITE);
    desc1->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0.0f, 1.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 545.0f), desc1, 110); //845.0f), desc1 , 110);
    
    
    CCLabelTTF* desc2 = CCLabelTTF::create(text4, "HelveticaNeue", 12);
    desc2->setColor(COLOR_WHITE);
    desc2->setHorizontalAlignment(kCCTextAlignmentLeft);
    registerLabel( this,ccp(0.0f, 1.0f), accp(34.0f, size.height*SCREEN_ZOOM_RATE - 545.0f - 170.0f), desc2, 110); //845.0f), desc1 , 110);
}

void *QuestStory::threadAction(void *threadid)
{
    /*
    char key1[20];
    sprintf(key1, "questStorytitle_%d", QuestStory::getInstance()->chapter);
    
    char key2[20];
    sprintf(key2, "questStorySubtitle_%d", QuestStory::getInstance()->chapter);
    
    char key3[20];
    sprintf(key3, "questStory_%d", QuestStory::getInstance()->chapter);
    */
//    CCLog(" QuestStory::getInstance()->chapter :%d key1:%s",QuestStory::getInstance()->chapter, QuestStory::getInstance()->key1);
//    CCLog(" QuestStory::getInstance()->chapter :%d key2:%s",QuestStory::getInstance()->chapter, QuestStory::getInstance()->key2);
//    CCLog(" QuestStory::getInstance()->chapter :%d key3:%s",QuestStory::getInstance()->chapter, QuestStory::getInstance()->key3);
//    CCLog(" QuestStory::getInstance()->chapter :%d key3:%s",QuestStory::getInstance()->chapter, QuestStory::getInstance()->key4);
    
    QuestStory::getInstance()->text1 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key1);
//    QuestStory::getInstance()->text2 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key2);
    
    const char* tmp1 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key3);
    const char* tmp2 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key4);
    
    string txt1 = tmp1;
//    txt.append(tmp2);
    string txt2 = tmp2;
    
    QuestStory::getInstance()->text3 = txt1.c_str();
    QuestStory::getInstance()->text4 = txt2.c_str();
    
    //LocalizationManager::getInstance()->get(QuestStory::getInstance()->key3);
    //QuestStory::getInstance()->text3 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key3);
    //QuestStory::getInstance()->text4 = LocalizationManager::getInstance()->get(QuestStory::getInstance()->key4);
    
    
//    CCLog("QuestStory::threadAction tmp1:%s", QuestStory::getInstance()->text1);
//    CCLog("QuestStory::threadAction tmp2:%s", QuestStory::getInstance()->text2);
//    CCLog("QuestStory::threadAction tmp3:%s", QuestStory::getInstance()->text3);
    
    QuestStory::getInstance()->schedule(schedule_selector(QuestStory::threadCallBack));
    pthread_exit(NULL);
    
}

/*
<desc>매드 기어라는 갱단이 장악하고 있는 범죄의 도시!\n"메트로 시티"\n들리는 소문에 의하면 갱단의 행동 대장인 댐드를 중심으로\n세계의 격투가들을 납치 하고 있다고 한다.\n왜? 그들을 납치하는 것일까? \n이 의문을 밝히는 키는 댐드에게 있다.\n매드 기어의 횡포에 맞서 위험에 처한 격투가들을 \n구출해야 한다.</desc>

 <desc>매드 기어라는 갱단이 장악하고 있는 범죄의 도시! 메트로 시티 들리는 소문에 의하면 갱단의 행동 대장인 댐드를 중심으로 세계의 격투가들을 납치 하고 있다고 한다. 왜? 그들을 납치하는 것일까? 이 의문을 밝히는 키는 댐드에게 있다.매드 기어의 횡포에 맞서 위험에 처한 격투가들을 구출해야 한다.</desc>
 */
void QuestStory::initThread()
{
    int t = 0;
    pthread_create(&threads, NULL, QuestStory::threadAction, (void *)t);
}

void QuestStory::threadCallBack()
{
    //CCLog("QuestStory::threadCallBack !!!!!!!!!!");
    
    QuestStory::getInstance()->InitText();
    
    this->unschedule(schedule_selector(QuestStory::threadCallBack));
}

void QuestStory::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 10, localPoint))
    {
        soundButton1();
        
        ChangeSpr(this, 10, "ui/tutorial/tutorial_btn_a2.png", 100);
    }
}

void QuestStory::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    
    if (GetSpriteTouchCheckByTag(this, 10, localPoint))
    {
        this->removeAllChildrenWithCleanup(true);
        restoreTouchDisable();
        ChangeSpr(this, 10, "ui/tutorial/tutorial_btn_a1.png", 100);
        
//        ARequestSender::getInstance()->requestStageList();
    }
    /*
    if (GetSpriteTouchCheckByTag(this, 2, localPoint))
    {
        this->removeAllChildrenWithCleanup(true);
        restoreTouchDisable();
    }
     */
}

void QuestStory::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(5.0f, 5.0f, winSize.width - 12.0f, winSize.height);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}