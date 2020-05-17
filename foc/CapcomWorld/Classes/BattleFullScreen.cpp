//
//  BattleFullScreen.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 18..
//
//

#include "BattleFullScreen.h"
#include "BattleDuelLayer.h"
#include "DojoLayerDojo.h"
#include "TraceLayer.h"

BattleEnemyLayer::BattleEnemyLayer(CCSize layerSize, int _questID)
{
    this->setContentSize(layerSize);
    
    this->setClipsToBounds(true);
    
    for(int i=0; i<5; ++i)
    {
        enemyCharacter[i]   = NULL;
        enemySkill[i]       = NULL;
    }
    
    DefensePoint = 0;
    SkillDefensePoint = 0;
    
    activeCritical = false;

    questID = _questID;
    InitUI();
}

BattleEnemyLayer::~BattleEnemyLayer()
{
    CC_SAFE_DELETE(aniFrame);
    
    this->removeAllChildrenWithCleanup(true);
}

void BattleEnemyLayer::InitUI()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();

    PlayerInfo* pInfo = (PlayerInfo*)PlayerInfo::getInstance();
    DojoLayerDojo* dojo = (DojoLayerDojo*)DojoLayerDojo::getInstance();
    Bg_List* bglist = (Bg_List*)dojo->BgDictionary->objectForKey(pInfo->getBackground());
    
    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        if (questID != 0)
        {
            string bgPath = CCFileUtils::sharedFileUtils()->getDocumentPath();
            Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
            bgPath = bgPath + questInfo->stageBG_L;
            enemyBG = CCSprite::create(bgPath.c_str(), CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        else
        {
            enemyBG = CCSprite::create("ui/main_bg/bg_1.png", CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        enemyBG->setAnchorPoint(ccp(0.5f, 0.5f));
        enemyBG->setScale(1.4f);
    }
    else
    {
        if (questID != 0)
        {
            string bgPath = CCFileUtils::sharedFileUtils()->getDocumentPath();
            Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
            bgPath = bgPath + questInfo->stageBG_L;
            enemyBG = CCSprite::create(bgPath.c_str(), CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        else
        {
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=bglist->L_ImgPath;
            enemyBG = CCSprite::create(path.c_str(), CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        enemyBG->setAnchorPoint(ccp(0.5f, 0.5f));
        enemyBG->setScale(1.4f);
    }
    
    CCSize enemyBGSize = enemyBG->getTexture()->getContentSizeInPixels();
    
    float scale = enemyBG->getScale();
    float tempX = enemyBGSize.width * scale;
    float tempY = enemyBGSize.height * scale;
    
    enemyBGPos = CCPoint(accp(tempX/2.0f + 70.0f, (tempY/2.0f)- 28.0f));

    enemyBG->setPosition(enemyBGPos);
    
    this->addChild(enemyBG, 10);
    
    enemySkillX[0] = 492.0f + (135.0f/2.0f);
    enemySkillY[0] = (size.height*SCREEN_ZOOM_RATE - 64.0f - 135.0f + (135.0f/2.0f)) - 500.0f;
    
    enemySkillX[1] = 376.0f + (135.0f/2.0f);
    enemySkillY[1] = (size.height*SCREEN_ZOOM_RATE - 0.0f  - 135.0f + (135.0f/2.0f)) - 500.0f;
    
    enemySkillX[2] = 246.0f + (135.0f/2.0f);
    enemySkillY[2] = (size.height*SCREEN_ZOOM_RATE - 78.0f  - 135.0f + (135.0f/2.0f)) - 500.0f;
    
    enemySkillX[3] = 100.0f + (135.0f/2.0f);
    enemySkillY[3] = (size.height*SCREEN_ZOOM_RATE - 68.0f - 135.0f + (135.0f/2.0f)) - 500.0f;
    
    enemySkillX[4] = 0.0f + (135.0f/2.0f);
    enemySkillY[4] = (size.height*SCREEN_ZOOM_RATE - 8.0f - 135.0f + (135.0f/2.0f)) - 500.0f;

    InitEnemyCharacter();
    SlideEnemyCharacter();
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0,0,128,128));
    
    aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);
}


void BattleEnemyLayer::InitEnemyCharacter()
{
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    CCSize size = GameConst::WIN_SIZE;
        
    //CCLog("===== 적 데이터 세팅 =====");
    CardInfo* card = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[4]);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[4]);
        if(cardInfo)
        {
            //TotalDefensePoint +=
            enemyPosX[0] = 220.0 + (512.0f*1.2f)/2;
            enemyPosY[0] = (size.height*SCREEN_ZOOM_RATE) -70.0f -476.0f - (512.0f*1.2f) + (512.0f*1.2f)/2;
            
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyCharacter[0] = CCSprite::create(path.c_str());
            enemyCharacter[0]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[0]->setScale(1.2f);
            enemyCharacter[0]->setPosition(accp(-(512.0f*1.2f)/2, enemyPosY[0]));
            enemyCharacter[0]->setTag(200);
            this->addChild(enemyCharacter[0], 509);
        }
    }
    
    card = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[3]);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[3]);
        if(cardInfo)
        {
            enemyPosX[1] = 172.0f + (512.0f*0.95f)/2;
            enemyPosY[1] = (size.height*SCREEN_ZOOM_RATE) -25.0f -476.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyCharacter[1] = CCSprite::create(path.c_str());
            enemyCharacter[1]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[1]->setScale(0.95f);
            enemyCharacter[1]->setPosition(accp(-(512.0f*0.95f)/2, enemyPosY[1]));
            enemyCharacter[1]->setTag(201);
            this->addChild(enemyCharacter[1], 508);
        }
    }
    
    card = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[2]);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[2]);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;

            enemyLeader = path.c_str();
            enemyPosX[2] = -130.0f + (512.0f*1.55f)/2;
            enemyPosY[2] = (size.height*SCREEN_ZOOM_RATE) -60.0f -476.0f - (512.0f*1.55f) + (512.0f*1.55f)/2;
            enemyCharacter[2] = CCSprite::create(path.c_str());
            enemyCharacter[2]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[2]->setScale(1.55f);
            enemyCharacter[2]->setPosition(accp(-(512.0f*1.55f)/2, enemyPosY[2]));
            enemyCharacter[2]->setTag(202);
            this->addChild(enemyCharacter[2], 510);
        }
    }
    
    card = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[1]);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[1]);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            enemyPosX[3] = -140.0f + (512.0f*1.08f)/2;;
            enemyPosY[3] = (size.height*SCREEN_ZOOM_RATE) -80.0f -476.0f - (512.0f*1.08f) + (512.0f*1.08f)/2;
            enemyCharacter[3] = CCSprite::create(path.c_str());
            enemyCharacter[3]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[3]->setScale(1.08f);
            enemyCharacter[3]->setPosition(accp(-(512.0f*1.08f)/2, enemyPosY[3]));
            enemyCharacter[3]->setTag(203);
            this->addChild(enemyCharacter[3], 509);
        }
    }
    
    card = CardDictionary::sharedCardDictionary()->getInfo(pInfo->battleResponseInfo->opponent_card[0]);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[0]);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;

            enemyPosX[4] = -205.0f + (512.0f*0.95f)/2;;
            enemyPosY[4] = (size.height*SCREEN_ZOOM_RATE) -35.0f -476.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            enemyCharacter[4] = CCSprite::create(path.c_str());
            enemyCharacter[4]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemyCharacter[4]->setScale(0.95f);
            enemyCharacter[4]->setPosition(accp(-(512.0f*0.95f)/2, enemyPosY[4]));
            enemyCharacter[4]->setTag(204);
            this->addChild(enemyCharacter[4], 508);
        }
    }    
}

void BattleEnemyLayer::enemyLeaderScaleUp()
{
    CCSprite* enemyLeaderImg = CCSprite::create(enemyLeader.c_str());
    enemyLeaderImg->setAnchorPoint(ccp(0.5f, 0.5f));
    enemyLeaderImg->setOpacity(177);
    enemyLeaderImg->setScale(2.1f);
    enemyLeaderImg->setPosition(accp(enemyPosX[2], enemyPosY[2]));
    enemyLeaderImg->setTag(569);
    this->addChild(enemyLeaderImg, 1000);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.300f);
    
    int *leadertag = new int(569);
    CCCallFuncND *callBack1 = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)leadertag);
    
    enemyLeaderImg->runAction(CCSequence::actions(delay2, callBack1, NULL));
}

void BattleEnemyLayer::SlideEnemyCharacter()
{
    if(enemyCharacter[0])
    {
        CCEaseOut *layerMove0 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.165f, accp(enemyPosX[0], enemyPosY[0])), 2.0f);
        enemyCharacter[0]->runAction(layerMove0);
    }
    
    if(enemyCharacter[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.033f);
        CCEaseOut *layerMove1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.231f, accp(enemyPosX[1], enemyPosY[1])), 2.0f);
        enemyCharacter[1]->runAction(CCSequence::actions(delay1, layerMove1, NULL));
    }
    
    if(enemyCharacter[2])
    {
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.165f);
        CCEaseOut *layerMove2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.198f, accp(enemyPosX[2], enemyPosY[2])), 2.0f);
        enemyCharacter[2]->runAction(CCSequence::actions(delay2, layerMove2, NULL));
    }
    
    if(enemyCharacter[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.330f);
        CCEaseOut *layerMove3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.099f, accp(enemyPosX[3], enemyPosY[3])), 2.0f);
        enemyCharacter[3]->runAction(CCSequence::actions(delay3, layerMove3, NULL));
    }
    
    if(enemyCharacter[4])
    {
        CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.396f);
        CCEaseOut *layerMove4 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.066f, accp(enemyPosX[4], enemyPosY[4])), 2.0f);
        enemyCharacter[4]->runAction(CCSequence::actions(delay4, layerMove4, NULL));
    }
}

void BattleEnemyLayer::Attack()
{
    CCCallFunc *call1 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::actionAttack));
    //CCDelayTime *delay1 = CCDelayTime::actionWithDuration(1.0f);
    //CCCallFunc *call2 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::actionAttackHitNShake));
    
    this->runAction(CCSequence::actions(call1, NULL));
}

void BattleEnemyLayer::actionAttack()
{
     playEffect("audio/attack_01.mp3");
    
    CCFiniteTimeAction* actionScaleBG = CCScaleTo::actionWithDuration(0.264f, 1.68f);
    enemyBG->runAction(CCSequence::actions(actionScaleBG, CCScaleTo::actionWithDuration(0.207f, 1.4f), NULL));
    
    if(enemyCharacter[0])
    {
        CCFiniteTimeAction* actionScale0 = CCScaleTo::actionWithDuration(0.264f, 1.08f);
        enemyCharacter[0]->runAction(CCSequence::actions(actionScale0, CCScaleTo::actionWithDuration(0.207f, 1.2f), NULL));
        
        CCFiniteTimeAction* actionMove0 = CCMoveTo::actionWithDuration(0.264f, accp(enemyPosX[0] + 57.0f, enemyPosY[0] - 27.0f));
        enemyCharacter[0]->runAction(CCSequence::actions(actionMove0, CCMoveTo::actionWithDuration(0.207f, accp(enemyPosX[0], enemyPosY[0])), NULL));
    }
    
    if(enemyCharacter[1])
    {
        CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.264f, 0.85f);
        enemyCharacter[1]->runAction(CCSequence::actions(actionScale1, CCScaleTo::actionWithDuration(0.207f, 0.95f), NULL));
        
        CCFiniteTimeAction* actionMove1 = CCMoveTo::actionWithDuration(0.264f, accp(enemyPosX[1] + 57.0f, enemyPosY[1] - 27.0f));
        enemyCharacter[1]->runAction(CCSequence::actions(actionMove1, CCMoveTo::actionWithDuration(0.207f, accp(enemyPosX[1], enemyPosY[1])), NULL));
    }
    
    if(enemyCharacter[3])
    {
        CCFiniteTimeAction* actionScale3 = CCScaleTo::actionWithDuration(0.264f, 0.97f);
        enemyCharacter[3]->runAction(CCSequence::actions(actionScale3, CCScaleTo::actionWithDuration(0.207f, 1.08f), NULL));
        
        CCFiniteTimeAction* actionMove3 = CCMoveTo::actionWithDuration(0.264f, accp(enemyPosX[3] - 57.0f, enemyPosY[3] - 27.0f));
        enemyCharacter[3]->runAction(CCSequence::actions(actionMove3, CCMoveTo::actionWithDuration(0.207f, accp(enemyPosX[3], enemyPosY[3])), NULL));
    }
    
    if(enemyCharacter[4])
    {
        CCFiniteTimeAction* actionScale4 = CCScaleTo::actionWithDuration(0.264f, 0.85f);
        enemyCharacter[4]->runAction(CCSequence::actions(actionScale4, CCScaleTo::actionWithDuration(0.207f, 0.95f), NULL));
        
        CCFiniteTimeAction* actionMove4 = CCMoveTo::actionWithDuration(0.264f, accp(enemyPosX[4] - 57.0f, enemyPosY[4] - 27.0f));
        enemyCharacter[4]->runAction(CCSequence::actions(actionMove4, CCMoveTo::actionWithDuration(0.207f, accp(enemyPosX[4], enemyPosY[4])), NULL));
    }
    
    if(enemyCharacter[2])
    {
        // -- 리더케릭
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.007f);
        CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.092f, 2.0f);
        CCDelayTime *delay22 = CCDelayTime::actionWithDuration(0.300f);
        CCCallFunc *LeaderScaleUP = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::enemyLeaderScaleUp));
        enemyCharacter[2]->runAction(CCSequence::actions(delay2, actionScale2, LeaderScaleUP, delay22, CCScaleTo::actionWithDuration(0.207f, 1.55f), NULL));
    }
    
    // -- 공격이펙트 opacity
    CCDelayTime *delayOpacity = CCDelayTime::actionWithDuration(0.138f);
    CCDelayTime *delayOpacity1 = CCDelayTime::actionWithDuration(0.415f);
    
    CCSprite* Attack = CCSprite::create("ui/battle/eff_attack.png");
    Attack->setTag(568);
    Attack->setOpacity(0);
    Attack->setAnchorPoint(ccp(0.0f, 0.0f));
    Attack->setPosition(accp(0.0f, 0.0f));
    this->addChild(Attack, 600);
    
    int *tag = new int(568);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.132f);
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.132f);
    
    Attack->runAction(CCSequence::actions(delayOpacity, fadein, delayOpacity1, fadeout, callBack, NULL));
}

void BattleEnemyLayer::actionHitNShake()
{
    if(enemyCharacter[2])
    {
        playEffect("audio/hit_01.mp3");
        enemyCharacter[2]->runAction(CCShake::create(0.2f, 15.8f, enemyPosX[2], enemyPosY[2]));
        HitPlay2();
    }
    
    if(enemyCharacter[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
        enemyCharacter[1]->runAction(CCSequence::actions(delay1, CCShake::create(0.2f, 11.5f, enemyPosX[1], enemyPosY[1]), NULL));
        
        CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::HitPlay1));
        this->runAction(CCSequence::actions(delay1, callBack1, NULL));
    }
    
    if(enemyCharacter[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.25f);
        enemyCharacter[3]->runAction(CCSequence::actions(delay3, CCShake::create(0.2f, 11.5f, enemyPosX[3], enemyPosY[3]), NULL));
        
        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::HitPlay3));
        this->runAction(CCSequence::actions(delay3, callBack3, NULL));
    }
    
    if(enemyCharacter[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.3f);
        enemyCharacter[0]->runAction(CCSequence::actions(delay0, CCShake::create(0.2f, 11.5f, enemyPosX[0], enemyPosY[0]), NULL));
        
        CCCallFunc *callBack0 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::HitPlay0));
        this->runAction(CCSequence::actions(delay0, callBack0, NULL));
    }
    
    if(enemyCharacter[4])
    {
        CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.2f);
        enemyCharacter[4]->runAction(CCSequence::actions(delay4, CCShake::create(0.2f, 11.5f, enemyPosX[4], enemyPosY[4]), NULL));
        
        CCCallFunc *callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleEnemyLayer::HitPlay4));
        this->runAction(CCSequence::actions(delay4, callBack4, NULL));
    }
    
    // -- 피격 이펙트 opacity
    CCDelayTime *delayOpacity = CCDelayTime::actionWithDuration(0.138f);
    CCDelayTime *delayOpacity1 = CCDelayTime::actionWithDuration(0.415f);
    
    CCSprite* Defense = CCSprite::create("ui/battle/eff_defense.png");
    Defense->setTag(568);
    Defense->setOpacity(0);
    Defense->setAnchorPoint(ccp(0.0f, 0.0f));
    Defense->setPosition(accp(0.0f, 0.0f));
    this->addChild(Defense, 600);
    
    int *tag = new int(568);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.132f);
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.132f);
    
    Defense->runAction(CCSequence::actions(delayOpacity, fadein, delayOpacity1, fadeout, callBack, NULL));
}

void BattleEnemyLayer::HitPlay0()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 103.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[0], aniFrame, this, ccp(0.0f, 0.0f), accp(404.0f - 40.0f, t), 3.0f, 600, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleEnemyLayer::HitPlay1()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 8.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[1], aniFrame, this, ccp(0.0f, 0.0f), accp(230.0f + 20.0f,  t), 3.0f, 601, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleEnemyLayer::HitPlay2()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 121.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[2], aniFrame, this, ccp(0.0f, 0.0f), accp(154.0f - 40.0f, t), 3.0f, 602, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
    
    // (960.0f - 470.0f - 121.0f)/2 - 480.0f
    // 184.5 - 480 = -295.5
    // accp(-295.5) = -147.75
    
}

void BattleEnemyLayer::HitPlay3()
{
    playEffect("audio/hit_01.mp3");
    int t= (960.0f - 470.0f - 70.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[3], aniFrame, this, ccp(0.0f, 0.0f), accp(36.0f - 70.0f, t), 3.0f, 603, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleEnemyLayer::HitPlay4()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 4.0f)/SCREEN_ZOOM_RATE - (240 * SCREEN_ZOOM_RATE);
    AniPlay(enemyHitEffect[4], aniFrame, this, ccp(0.0f, 0.0f), accp(-98.0f - 40.0f, t), 3.0f, 604, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleEnemyLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
	if (clipsToBounds)
    {
        CCRect scissorRect = CCRect(0.0f, 0.0f, this->getContentSize().width, winSize.height/2);// 240.0f);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (clipsToBounds)
        glDisable(GL_SCISSOR_TEST);
}

float BattleEnemyLayer::GetDefensePoint() const
{
    return DefensePoint;
}

float BattleEnemyLayer::GetSkillDefensePoint() const
{
    return SkillDefensePoint;
}

bool BattleEnemyLayer::IsFeatureSkill(CCArray* skills)
{
    if(!skills)
        return false;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(1 == skill->side)
            return true;
    }
    
    return false;
}

bool BattleEnemyLayer::IsFeatureHealSkill(CCArray* skills)
{
    if(!skills)
        return false;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(1 == skill->side)
        {
            if(2 == skill->skillID)
                return true;
        }
    }
    
    return false;
}

void BattleEnemyLayer::ActiveSkill(CCArray* skills, const ABattleRound* _RoundData)
{
    CCSize size = GameConst::WIN_SIZE;
    
    if(!skills)     return;
    if(!_RoundData) return;
    
    const ABattleRound* RoundData = _RoundData;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(1 == skill->side)
        {
            if(1 == skill->skillID)
                enemySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_atkplus.png");
            if(2 == skill->skillID && RoundData->defender_heal > 0)
                enemySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_heal.png");
            if(3 == skill->skillID)
            {
                activeCritical = true;
                enemySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_critical.png");
            }
        }
    }

    for(int i=0; i<5; ++i)
    {
        if(enemySkill[i] && enemyCharacter[i])
        {
            playEffect("audio/skill_01.mp3");
            
            enemySkill[i]->setAnchorPoint(ccp(0.5f, 0.5f));
            enemySkill[i]->setPosition(accp(enemySkillX[i], enemySkillY[i]));
            enemySkill[i]->setTag(40001 + i);
            this->addChild(enemySkill[i], 7000);
            int *tag0 = new int(40001 + i);
            CCCallFuncND *callBack0 = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag0);
            
            CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
            
            CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.5f);
            
            enemySkill[i]->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, 1.5f), delay, fadeout, CCScaleTo::actionWithDuration(0.5f, 1.0f), callBack0, NULL));
        }
    }
}

void BattleEnemyLayer::ActiveCriticalSkill()
{
    if(!this->featureCritical()) return;
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    
    CardListInfo* cardListInfo = FileManager::sharedFileManager()->GetCardInfo(pInfo->battleResponseInfo->opponent_card[2]);
    if(cardListInfo)
    {
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        
        // -- width = 640
        CCSprite* bg = CCSprite::create("ui/battle/skill_cha_bg.png");
        bg->setFlipX(true);
        bg->setAnchorPoint(ccp(0.0f, 0.0f));
        bg->setPosition(accp(size.width*SCREEN_ZOOM_RATE, 66.0f));
        this->addChild(bg, 8000);

        CCEaseOut *move1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(0.0f, 66.0f)), 2.0f);
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
        CCEaseOut *moveBack1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(size.width*SCREEN_ZOOM_RATE, 66.0f)), 2.0f);
        bg->runAction(CCSequence::actions(move1, delay1, moveBack1, NULL));
        
        // -- width = 512
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=cardListInfo->smallBattleImg;
        
        CCSprite* leader = CCSprite::create(path.c_str());
        leader->setScale(1.23f);
        leader->setFlipX(true);
        leader->setAnchorPoint(ccp(0.0f, 0.0f));
        leader->setPosition(accp(size.width*SCREEN_ZOOM_RATE, 70.0f));
        this->addChild(leader, 8000);
        
        CCEaseOut *move2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(10.0f, 70.0f)), 2.0f);
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.5f);
        CCEaseOut *moveBack2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(size.width*SCREEN_ZOOM_RATE, 66.0f)), 2.0f);
        leader->runAction(CCSequence::actions(move2, delay2, moveBack2, NULL));
    }
}

//////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////
BattleMyLayer::BattleMyLayer(CCSize layerSize, int selectedTeam, int _questID)
{
    this->setContentSize(layerSize);
    
    selectTeam = selectedTeam;
    
    for(int i=0; i<5; ++i)
    {
        myCharacter[i]  = NULL;
        mySkill[i]      = NULL;
    }
    
    AttackPoint = 0;
    SkillAttackPoint = 0;

    activeCritical = false;
    
    questID = _questID;    
    InitUI();
}


BattleMyLayer::~BattleMyLayer()
{
    CC_SAFE_DELETE(aniFrame);
    
    this->removeAllChildrenWithCleanup(true);
}

void BattleMyLayer::InitUI()
{
    PlayerInfo* pInfo = (PlayerInfo*)PlayerInfo::getInstance();
    DojoLayerDojo* dojo = (DojoLayerDojo*)DojoLayerDojo::getInstance();
    Bg_List* bglist = (Bg_List*)dojo->BgDictionary->objectForKey(pInfo->getBackground());

    if(12 > PlayerInfo::getInstance()->getTutorialProgress())
    {
        if (questID != 0)
        {
            string bgPath = CCFileUtils::sharedFileUtils()->getDocumentPath();
            Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
            bgPath = bgPath + questInfo->stageBG_L;
            myBG = CCSprite::create(bgPath.c_str(), CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        else
        {
            myBG = CCSprite::create("ui/main_bg/bg_1.png", CCRectMake(accp(0.0f), accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        myBG->setAnchorPoint(ccp(0.5f, 0.5f));
        myBG->setScale(1.4f);
    }
    else
    {
        if (questID != 0)
        {
            string bgPath = CCFileUtils::sharedFileUtils()->getDocumentPath();
            Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
            bgPath = bgPath + questInfo->stageBG_L;
            myBG = CCSprite::create(bgPath.c_str(), CCRectMake(0, accp(40.0f), accp(600.0f), accp(345.0f)));
        }
        else
        {
            string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=bglist->L_ImgPath;
            myBG = CCSprite::create(path.c_str(), CCRectMake(accp(0.0f), accp(40.0f), accp(600.0f), accp(345.0f)));
        }
            myBG->setAnchorPoint(ccp(0.5f, 0.5f));
            myBG->setScale(1.4f);
    }

    CCSize myBGSize = myBG->getTexture()->getContentSizeInPixels();
    
    float scale = myBG->getScale();
    float tempX = myBGSize.width * scale;
    float tempY = myBGSize.height * scale;
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    float PosY = (size.height*SCREEN_ZOOM_RATE);
    
    myBGPos = CCPoint(accp(tempX/2.0f, (PosY/2.0f) + (tempY/2.0f)));
    myBG->setPosition(myBGPos);
    
    this->addChild(myBG, 10);
    
    mySkillX[0] = 492.0f + (135.0f/2.0f);
    mySkillY[0] = size.height*SCREEN_ZOOM_RATE - 64.0f - 135.0f + (135.0f/2.0f);
    
    mySkillX[1] = 376.0f + (135.0f/2.0f);
    mySkillY[1] = size.height*SCREEN_ZOOM_RATE - 0.0f  - 135.0f + (135.0f/2.0f);
    
    mySkillX[2] = 246.0f + (135.0f/2.0f);
    mySkillY[2] = size.height*SCREEN_ZOOM_RATE - 78.0f  - 135.0f + (135.0f/2.0f);
    
    mySkillX[3] = 100.0f + (135.0f/2.0f);
    mySkillY[3] = size.height*SCREEN_ZOOM_RATE - 68.0f - 135.0f + (135.0f/2.0f);
    
    mySkillX[4] = 0.0f + (135.0f/2.0f);
    mySkillY[4] = size.height*SCREEN_ZOOM_RATE - 8.0f - 135.0f + (135.0f/2.0f);
    
    InitMyCharacter();
    SlideMyCharacter();
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle/eff001.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle/eff002.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle/eff003.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle/eff004.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/battle/eff005.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/battle/eff006.png", CCRectMake(0,0,128,128));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/battle/eff007.png", CCRectMake(0,0,128,128));
    
    aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);
}

void BattleMyLayer::InitMyCharacter()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CCAssert(pInfo->battleResponseInfo, "result info is NULL");
    
    //CCLog("===== 아군 데이터 세팅 =====");
    
    CardInfo *card = pInfo->GetCardInDeck(0, selectTeam, 4);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[0] = 220.0 + (512.0f*1.2f)/2;
            myPosY[0] = (size.height*SCREEN_ZOOM_RATE) -70.0f - (512.0f*1.2f) + (512.0f*1.2f)/2;
            myCharacter[0] = CCSprite::create(path.c_str());
            myCharacter[0]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[0]->setScale(1.2f);
            myCharacter[0]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*1.2f)/2, myPosY[0]));
            myCharacter[0]->setTag(300);
            this->addChild(myCharacter[0], 99);
        }
    }
    
    card = pInfo->GetCardInDeck(0, selectTeam, 3);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[1] = 172.0f + (512.0f*0.95f)/2;
            myPosY[1] = (size.height*SCREEN_ZOOM_RATE) -25.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            myCharacter[1] = CCSprite::create(path.c_str());
            myCharacter[1]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[1]->setScale(0.95f);
            myCharacter[1]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*0.95f)/2, myPosY[1]));
            myCharacter[1]->setTag(301);
            this->addChild(myCharacter[1], 98);
        }
    }
    
    card = pInfo->GetCardInDeck(0, selectTeam, 2);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myLeader = path.c_str();
            myPosX[2] = -130.0f + (512.0f*1.55f)/2;
            myPosY[2] = (size.height*SCREEN_ZOOM_RATE) -60.0f - (512.0f*1.55f) + (512.0f*1.55f)/2;
            myCharacter[2] = CCSprite::create(path.c_str());
            myCharacter[2]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[2]->setScale(1.55f);
            myCharacter[2]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*1.55f)/2, myPosY[2]));
            myCharacter[2]->setTag(302);
            this->addChild(myCharacter[2], 100);
        }
    }
    
    card = pInfo->GetCardInDeck(0, selectTeam, 1);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[3] = -140.0f + (512.0f*1.08f)/2;;
            myPosY[3] = (size.height*SCREEN_ZOOM_RATE) -80.0f - (512.0f*1.08f) + (512.0f*1.08f)/2;
            myCharacter[3] = CCSprite::create(path.c_str());
            myCharacter[3]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[3]->setScale(1.08f);
            myCharacter[3]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*1.08f)/2, myPosY[3]));
            myCharacter[3]->setTag(303);
            this->addChild(myCharacter[3], 99);
        }
    }
    
    card = pInfo->GetCardInDeck(0, selectTeam, 0);
    if(card)
    {
        CardListInfo* cardInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
        if(cardInfo)
        {
            string path = "ui/cha/";
            //string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
            path+=cardInfo->largeBattleImg;
            
            myPosX[4] = -205.0f + (512.0f*0.95f)/2;;
            myPosY[4] = (size.height*SCREEN_ZOOM_RATE) -35.0f - (512.0f*0.95f) + (512.0f*0.95f)/2;
            myCharacter[4] = CCSprite::create(path.c_str());
            myCharacter[4]->setAnchorPoint(ccp(0.5f, 0.5f));
            myCharacter[4]->setScale(0.95f);
            myCharacter[4]->setPosition(accp(size.width*SCREEN_ZOOM_RATE + (512.0f*0.95f)/2, myPosY[4]));
            myCharacter[4]->setTag(304);
            this->addChild(myCharacter[4], 98);
        }
    }
}

void BattleMyLayer::SlideMyCharacter()
{
    if(myCharacter[4])
    {
        CCEaseOut *layerMove4 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.165f, accp(myPosX[4], myPosY[4])), 2.0f);
        myCharacter[4]->runAction(layerMove4);
    }
    
    if(myCharacter[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.033f);
        CCEaseOut *layerMove3 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.231f, accp(myPosX[3], myPosY[3])), 2.0f);
        myCharacter[3]->runAction(CCSequence::actions(delay3, layerMove3, NULL));
    }
    
    if(myCharacter[2])
    {
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.165f);
        CCEaseOut *layerMove2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.198f, accp(myPosX[2], myPosY[2])), 2.0f);
        myCharacter[2]->runAction(CCSequence::actions(delay2, layerMove2, NULL));
    }
    
    if(myCharacter[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.330f);
        CCEaseOut *layerMove1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.099f, accp(myPosX[1], myPosY[1])), 2.0f);
        myCharacter[1]->runAction(CCSequence::actions(delay1, layerMove1, NULL));
    }
    
    if(myCharacter[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.396f);
        CCEaseOut *layerMove0 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.066f, accp(myPosX[0], myPosY[0])), 2.0f);
        myCharacter[0]->runAction(CCSequence::actions(delay0, layerMove0, NULL));
    }
}

void BattleMyLayer::Attack()
{
    CCCallFunc *call1 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::actionAttack));
    //CCDelayTime *delay1 = CCDelayTime::actionWithDuration(1.0f);
    //CCCallFunc *call2 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::actionDefenseHitNShake));
    
    this->runAction(CCSequence::actions(call1, NULL));
}

void BattleMyLayer::actionAttack()
{
    playEffect("audio/attack_01.mp3");
    
    CCFiniteTimeAction* actionScaleBG = CCScaleTo::actionWithDuration(0.264f, 1.68f);
    myBG->runAction(CCSequence::actions(actionScaleBG, CCScaleTo::actionWithDuration(0.207f, 1.4f), NULL));
    
    if(myCharacter[0])
    {
        CCFiniteTimeAction* actionScale0 = CCScaleTo::actionWithDuration(0.264f, 1.08f);
        myCharacter[0]->runAction(CCSequence::actions(actionScale0, CCScaleTo::actionWithDuration(0.207f, 1.2f), NULL));
        
        CCFiniteTimeAction* actionMove0 = CCMoveTo::actionWithDuration(0.264f, accp(myPosX[0] + 57.0f, myPosY[0] - 27.0f));
        myCharacter[0]->runAction(CCSequence::actions(actionMove0, CCMoveTo::actionWithDuration(0.207f, accp(myPosX[0], myPosY[0])), NULL));
    }
    
    if(myCharacter[1])
    {
        CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.264f, 0.85f);
        myCharacter[1]->runAction(CCSequence::actions(actionScale1, CCScaleTo::actionWithDuration(0.207f, 0.95f), NULL));
        
        CCFiniteTimeAction* actionMove1 = CCMoveTo::actionWithDuration(0.264f, accp(myPosX[1] + 57.0f, myPosY[1] - 27.0f));
        myCharacter[1]->runAction(CCSequence::actions(actionMove1, CCMoveTo::actionWithDuration(0.207f, accp(myPosX[1], myPosY[1])), NULL));
    }
    
    if(myCharacter[3])
    {
        CCFiniteTimeAction* actionScale3 = CCScaleTo::actionWithDuration(0.264f, 0.97f);
        myCharacter[3]->runAction(CCSequence::actions(actionScale3, CCScaleTo::actionWithDuration(0.207f, 1.08f), NULL));
        
        CCFiniteTimeAction* actionMove3 = CCMoveTo::actionWithDuration(0.264f, accp(myPosX[3] - 57.0f, myPosY[3] - 27.0f));
        myCharacter[3]->runAction(CCSequence::actions(actionMove3, CCMoveTo::actionWithDuration(0.207f, accp(myPosX[3], myPosY[3])), NULL));
    }
    
    if(myCharacter[4])
    {
        CCFiniteTimeAction* actionScale4 = CCScaleTo::actionWithDuration(0.264f, 0.85f);
        myCharacter[4]->runAction(CCSequence::actions(actionScale4, CCScaleTo::actionWithDuration(0.207f, 0.95f), NULL));
        
        CCFiniteTimeAction* actionMove4 = CCMoveTo::actionWithDuration(0.264f, accp(myPosX[4] - 57.0f, myPosY[4] - 27.0f));
        myCharacter[4]->runAction(CCSequence::actions(actionMove4, CCMoveTo::actionWithDuration(0.207f, accp(myPosX[4], myPosY[4])), NULL));
    }
    
    if(myCharacter[2])
    {
        // -- 리더케릭
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.007f);
        CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.092f, 2.0f);
        CCDelayTime *delay22 = CCDelayTime::actionWithDuration(0.300f);
        CCCallFunc *LeaderScaleUP = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::myLeaderScaleUp));
        myCharacter[2]->runAction(CCSequence::actions(delay2, actionScale2, LeaderScaleUP, delay22, CCScaleTo::actionWithDuration(0.207f, 1.55f), NULL));
    }
    
    // -- 공격이펙트 opacity
    CCDelayTime *delayOpacity = CCDelayTime::actionWithDuration(0.138f);
    CCDelayTime *delayOpacity1 = CCDelayTime::actionWithDuration(0.415f);
    
    CCSprite* Attack = CCSprite::create("ui/battle/eff_attack.png");
    Attack->setTag(568);
    Attack->setOpacity(0);
    Attack->setAnchorPoint(ccp(0.0f, 0.0f));
    Attack->setPosition(accp(0.0f, 480.0f));
    this->addChild(Attack, 300);
    
    int *tag = new int(568);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.132f);
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.132f);
    
    Attack->runAction(CCSequence::actions(delayOpacity, fadein, delayOpacity1, fadeout, callBack, NULL));
}

void BattleMyLayer::actionHitNShake()
{
    if(myCharacter[2])
    {
        playEffect("audio/hit_01.mp3");
        myCharacter[2]->runAction(CCShake::create(0.2f, 15.8f, myPosX[2], myPosY[2]));
        DefensePlay2();
    }
    
    if(myCharacter[1])
    {
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.1f);
        myCharacter[1]->runAction(CCSequence::actions(delay1, CCShake::create(0.2f, 11.5f, myPosX[1], myPosY[1]), NULL));
        
        CCCallFunc *callBack1 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::DefensePlay1));
        this->runAction(CCSequence::actions(delay1, callBack1, NULL));
    }
    
    if(myCharacter[3])
    {
        CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.25f);
        myCharacter[3]->runAction(CCSequence::actions(delay3, CCShake::create(0.2f, 11.5f, myPosX[3], myPosY[3]), NULL));
        
        CCCallFunc *callBack3 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::DefensePlay3));
        this->runAction(CCSequence::actions(delay3, callBack3, NULL));
    }
    
    if(myCharacter[0])
    {
        CCDelayTime *delay0 = CCDelayTime::actionWithDuration(0.3f);
        myCharacter[0]->runAction(CCSequence::actions(delay0, CCShake::create(0.2f, 11.5f, myPosX[0], myPosY[0]), NULL));
        
        CCCallFunc *callBack0 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::DefensePlay0));
        this->runAction(CCSequence::actions(delay0, callBack0, NULL));
    }
    
    if(myCharacter[4])
    {
        CCDelayTime *delay4 = CCDelayTime::actionWithDuration(0.2f);
        myCharacter[4]->runAction(CCSequence::actions(delay4, CCShake::create(0.2f, 11.5f, myPosX[4], myPosY[4]), NULL));
        
        CCCallFunc *callBack4 = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::DefensePlay4));
        this->runAction(CCSequence::actions(delay4, callBack4, NULL));
    }
    
    // -- 피격 opacity
    CCDelayTime *delayOpacity = CCDelayTime::actionWithDuration(0.138f);
    CCDelayTime *delayOpacity1 = CCDelayTime::actionWithDuration(0.415f);
    
    CCSprite* Defense = CCSprite::create("ui/battle/eff_defense.png");
    Defense->setTag(578);
    Defense->setOpacity(0);
    Defense->setAnchorPoint(ccp(0.0f, 0.0f));
    Defense->setPosition(accp(0.0f, 480.0f));
    this->addChild(Defense, 600);
    
    int *tag = new int(578);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag);
    
    CCFadeIn* fadein = CCFadeIn::actionWithDuration(0.132f);
    CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.132f);
    
    Defense->runAction(CCSequence::actions(delayOpacity, fadein, delayOpacity1, fadeout, callBack, NULL));
}

void BattleMyLayer::DefensePlay0()
{
    playEffect("audio/hit_01.mp3");
    
    int t = (960.0f - 470.0f - 103.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[0], aniFrame, this, ccp(0.0f, 0.0f), accp(404.0f - 40.0f, t), 3.0f, 605, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleMyLayer::DefensePlay1()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 8.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[1], aniFrame, this, ccp(0.0f, 0.0f), accp(230.0f + 20.0f, t), 3.0f, 606, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleMyLayer::DefensePlay2()
{
    playEffect("audio/hit_01.mp3");
    int t= (960.0f - 470.0f - 121.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[2], aniFrame, this, ccp(0.0f, 0.0f), accp(154.0f - 40.0f, t), 3.0f, 607, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleMyLayer::DefensePlay3()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 70.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[3], aniFrame, this, ccp(0.0f, 0.0f), accp(36.0f - 70.0f, t), 3.0f, 608, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleMyLayer::DefensePlay4()
{
    playEffect("audio/hit_01.mp3");
    int t = (960.0f - 470.0f - 4.0f)/SCREEN_ZOOM_RATE;
    if (SCREEN_ZOOM_RATE==1)t+=170;
    
    AniPlay(myHitEffect[4], aniFrame, this, ccp(0.0f, 0.0f), accp(-98.0f - 40.0f, t), 3.0f, 609, 1500, callfuncND_selector(BattleFullScreen::removeSpr));
}

void BattleMyLayer::myLeaderScaleUp()
{
    CCSprite* myLeaderImg = CCSprite::create(myLeader.c_str());
    myLeaderImg->setAnchorPoint(ccp(0.5f, 0.5f));
    myLeaderImg->setOpacity(177);
    myLeaderImg->setScale(2.1f);
    myLeaderImg->setPosition(accp(myPosX[2], myPosY[2]));
    myLeaderImg->setTag(569);
    this->addChild(myLeaderImg, 100);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.300f);
    
    int *leadertag = new int(569);
    CCCallFuncND *callBack1 = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)leadertag);
    
    myLeaderImg->runAction(CCSequence::actions(delay2, callBack1, NULL));
}

float BattleMyLayer::GetAttackPoint() const
{
    return AttackPoint;
}

float BattleMyLayer::GetSkillAttackPoint() const
{
    return SkillAttackPoint;
}

bool BattleMyLayer::IsFeatureSkill(CCArray* skills)
{
    if(!skills)
        return false;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(0 == skill->side)
            return true;
    }
    
    return false;
}

bool BattleMyLayer::IsFeatureHealSkill(CCArray* skills)
{
    if(!skills)
        return false;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(0 == skill->side)
        {
            if(2 == skill->skillID)
                return true;
        }
    }
    
    return false;
}


void BattleMyLayer::ActiveSkill(CCArray* skills, const ABattleRound* _RoundData)
{
    CCSize size = GameConst::WIN_SIZE;
    
    if(!skills)     return;
    if(!_RoundData) return;
    
    const ABattleRound* RoundData = _RoundData;
    
    for(int i=0; i<skills->count(); ++i)
    {
        OpponentSkillInfo* skill = (OpponentSkillInfo*)skills->objectAtIndex(i);
        
        if(0 == skill->side)
        {
            if(1 == skill->skillID)
            {
                mySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_atkplus.png");
            }
            if(2 == skill->skillID && RoundData->attacker_heal > 0)
            {
                mySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_heal.png");
            }
            if(3 == skill->skillID)
            {
                activeCritical = true;
                mySkill[abs(skill->slot-4)] = CCSprite::create("ui/battle/duel_skill_critical.png");
            }
        }
    }
    
    for(int i=0; i<5; ++i)
    {
        if(mySkill[i] && myCharacter[i])
        {
            playEffect("audio/skill_01.mp3");
            
            mySkill[i]->setAnchorPoint(ccp(0.5f, 0.5f));
            mySkill[i]->setPosition(accp(mySkillX[i], mySkillY[i]));
            mySkill[i]->setTag(40001+i);
            this->addChild(mySkill[i], 7000);
            int *tag0 = new int(40001+i);
            CCCallFuncND *callBack0 = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag0);
            
            CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
            
            CCFadeOut* fadeout = CCFadeOut::actionWithDuration(0.5f);
            
            mySkill[i]->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, 1.5f), delay, fadeout, CCScaleTo::actionWithDuration(0.5f, 1.0f), callBack0, NULL));
        }
    }
    
    if(activeCritical)
    {
        CCDelayTime *delay = CCDelayTime::actionWithDuration(1.5f);
        
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleMyLayer::ActiveCriticalSkill));
        
        this->runAction(CCSequence::actions(delay, callBack, NULL));
    }
}

void BattleMyLayer::ActiveCriticalSkill()
{
    if(!this->featureCritical()) return;
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
    CardInfo* card = pInfo->GetCardInDeck(0, selectTeam, 2);
    
    CardListInfo* cardListInfo = FileManager::sharedFileManager()->GetCardInfo(card->cardId);
    if(cardListInfo)
    {
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        
        // -- width = 640
        CCSprite* bg = CCSprite::create("ui/battle/skill_cha_bg.png");
        bg->setAnchorPoint(ccp(0.0f, 0.0f));
        bg->setPosition(accp(-640.0f, 574.0f));
        bg->setTag(921);
        this->addChild(bg, 8000);
        
        CCEaseOut *move1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(0.0f, 574.0f)), 2.0f);
        CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
        CCEaseOut *moveBack1 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(-640.0f, 574.0f)), 2.0f);
        bg->runAction(CCSequence::actions(move1, delay1, moveBack1, NULL));
        
        // -- width = 512
        string path = CCFileUtils::sharedFileUtils()->getDocumentPath();
        path+=cardListInfo->smallBattleImg;
        
        CCSprite* leader = CCSprite::create(path.c_str());
        leader->setScale(1.23f);
        leader->setAnchorPoint(ccp(0.0f, 0.0f));
        leader->setPosition(accp(-512.0f * 1.23f, 580.0f));
        leader->setTag(922);
        this->addChild(leader, 8000);
        
        CCEaseOut *move2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(0.0f, 580.0f)), 2.0f);
        CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.5f);
        CCEaseOut *moveBack2 = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.2f, accp(-512.0f * 1.23f, 580.0f)), 2.0f);
        leader->runAction(CCSequence::actions(move2, delay2, moveBack2, NULL));
    }
}

void BattleMyLayer::RemoveCriticalSkillRes()
{
    this->removeChildByTag(921, true);
    this->removeChildByTag(922, true);
}

//////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////

BattleFullScreen::BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom) : EnemyLayer(NULL), MyLayer(NULL), resultInfo(NULL)
{
    this->setContentSize(layerSize);
    
    CurrentRound = 0;
    
    nCallFromNpcBattle = callFrom;
    nFirstAttack = PlayerInfo::getInstance()->battleResponseInfo->first_attack;
    
    EnemyLayer = new BattleEnemyLayer(layerSize);
    EnemyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    EnemyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(EnemyLayer, 1000);
    
    MyLayer = new BattleMyLayer(layerSize, selectedTeam);
    MyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    MyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(MyLayer, 100);
    
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        soundBG->playBackgroundMusic("audio/bgm_battle_01.mp3", true);
    }
    
    // -- 1라운드에 2번 공방
    RoundCount = PlayerInfo::getInstance()->battleResponseInfo->rounds->count();
    ConstRoundCount = PlayerInfo::getInstance()->battleResponseInfo->rounds->count();
    
    CCLOG("라운드     : %d", RoundCount);
    BattleSequence = PlayerInfo::getInstance()->battleResponseInfo->rounds;
    
    AttackCount = 0;
    
    enemyHP     = PlayerInfo::getInstance()->battleResponseInfo->rival_max_hp;
    enemyMaxHp  = PlayerInfo::getInstance()->battleResponseInfo->rival_max_hp;
    
    myMaxHp     = PlayerInfo::getInstance()->battleResponseInfo->user_max_hp;
    myHP        = PlayerInfo::getInstance()->battleResponseInfo->user_max_hp;
    
    ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
    
    MyLayer->SetAttackPoint(RoundData->attacker_point);
    
    EnemyLayer->SetDefensePoint(RoundData->defender_point);
    
    
    CCLOG("아군 HP     : %f", myMaxHp);
    CCLOG("=====================================================");
    CCLOG("적군 HP     : %f", enemyMaxHp);
}

BattleFullScreen::BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom, ResponseRivalBattle* _rivalBattleResult, int _questID)
    : EnemyLayer(NULL), MyLayer(NULL), resultInfo(NULL), BattleSequence(NULL), QuestUpdateResultInfoFromTrace(NULL)
{
    this->setContentSize(layerSize);
       
    CurrentRound = 0;
    
    nCallFromNpcBattle = callFrom;
    nFirstAttack = 0;
    
    EnemyLayer = new BattleEnemyLayer(layerSize, _questID);
    EnemyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    EnemyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(EnemyLayer, 1000);
    
    MyLayer = new BattleMyLayer(layerSize, selectedTeam, _questID);
    MyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    MyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(MyLayer, 100);
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        soundBG->playBackgroundMusic("audio/bgm_battle_01.mp3", true);
    }
    
    // -- 1라운드에 2번 공방
    RoundCount = _rivalBattleResult->rounds->count();
    ConstRoundCount = _rivalBattleResult->rounds->count();
    
    BattleSequence = _rivalBattleResult->rounds;
    
    AttackCount = 0;
    
    rivalBattleInfoFromTrace = _rivalBattleResult;
    
    enemyHP     = _rivalBattleResult->rival_origin_hp;
    enemyMaxHp  = _rivalBattleResult->rival_max_hp;
    
    myMaxHp     = _rivalBattleResult->user_max_hp;
    myHP        = _rivalBattleResult->user_max_hp;

    ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
    
    MyLayer->SetAttackPoint(RoundData->attacker_point);
   
    EnemyLayer->SetDefensePoint(RoundData->defender_point);
      
    CCLOG("아군 HP     : %f", myMaxHp);
    CCLOG("=====================================================");
    CCLOG("적군 HP     : %f", enemyMaxHp);
}

BattleFullScreen::BattleFullScreen(CCSize layerSize, int selectedTeam, int callFrom, ResponseQuestUpdateResultInfo* _rivalBattleResult, int _questID)
: QuestUpdateResultInfoFromTrace(NULL)
{
    this->setContentSize(layerSize);
    
    CurrentRound = 0;
    
    nCallFromNpcBattle = callFrom;
    nFirstAttack = 0;
    
    EnemyLayer = new BattleEnemyLayer(layerSize, _questID);
    EnemyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    EnemyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(EnemyLayer, 1000);
    
    MyLayer = new BattleMyLayer(layerSize, selectedTeam, _questID);
    MyLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    MyLayer->setPosition(accp(0.0f, 0.0f));
    this->addChild(MyLayer, 100);
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        soundBG->playBackgroundMusic("audio/bgm_battle_01.mp3", true);
    }
    
    QuestUpdateResultInfoFromTrace = _rivalBattleResult;
    // -- 1라운드에 2번 공방
    RoundCount = _rivalBattleResult->rounds->count();
    ConstRoundCount = _rivalBattleResult->rounds->count();
    
    BattleSequence = _rivalBattleResult->rounds;
    
    AttackCount = 0;
    
    enemyHP     = _rivalBattleResult->rival_hp_before_battle;
    enemyMaxHp  = _rivalBattleResult->rival_max_hp;
    
    myMaxHp     = _rivalBattleResult->user_max_hp;
    myHP        = _rivalBattleResult->user_max_hp;
    
    ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
    
    MyLayer->SetAttackPoint(RoundData->attacker_point);
    
    EnemyLayer->SetDefensePoint(RoundData->defender_point);
    
    
    CCLOG("아군 HP     : %f", myMaxHp);
    CCLOG("=====================================================");
    CCLOG("적군 HP     : %f", enemyMaxHp);
}

BattleFullScreen::~BattleFullScreen()
{
    CC_SAFE_DELETE(EnemyLayer);
    CC_SAFE_DELETE(MyLayer);
    
    this->removeAllChildrenWithCleanup(true);
}

void BattleFullScreen::SetTouchTrue()
{
    this->setTouchEnabled(true);
}

void BattleFullScreen::SetBattleResult(void *data)
{
    resultInfo = (ResponseBattleInfo*)data;
}

void BattleFullScreen::InitUI()
{
    CCSize size = GameConst::WIN_SIZE;
  
    // -- 게이지
    CCSprite* gaugeBg = CCSprite::create("ui/battle/battle_gage_bg.png");
    gaugeBg->setAnchorPoint(ccp(0.0f, 0.0f));
    gaugeBg->setPosition(accp(0.0f, 400.0f));
    this->addChild(gaugeBg, 2500);
    
    float x1 = size.width*SCREEN_ZOOM_RATE - 20.0f;

    // -- 적 닉네임
    string enemyNick = resultInfo->enemy_nick;
    
    enemyNick+= " ";
    CCLabelTTF* pEnemyID = CCLabelTTF::create(enemyNick.c_str(), "HelveticaNeue-BoldItalic", 14);
    pEnemyID->setColor(COLOR_WHITE);
    registerLabel( this,ccp(1.0f, 0.0f), accp(x1, 420.0f), pEnemyID, 3000);
    
    const int len = enemyNick.length();
    
    x1 = x1 - (len*15.0f);
    char buff1[10];
    sprintf(buff1, "%d", resultInfo->enemy_level);
    /*
    string tempLevel = buff1;
    string enemyLevel = " LV." + tempLevel;
    // 적 레벨
    CCLabelTTF* pEnemyLV = CCLabelTTF::create(enemyLevel.c_str(), "HelveticaNeue-Bold", 12);
    pEnemyLV->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(1.0f, 0.0f), accp(x1, 420.0f), pEnemyLV, 3000);
    */
    
    PlayerInfo* pInfo = PlayerInfo::getInstance();
        
    float x = 20.0f;
    
    // -- 플레이어 닉네임
    string MyNick = pInfo->displayName;
    MyNick+=" ";
    const int len1 = MyNick.length();

    CCLabelTTF* pMyNick = CCLabelTTF::create(MyNick.c_str(), "HelveticaNeue-BoldItalic", 14);
    pMyNick->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.0f, 0.0f), accp(x, 510.0f), pMyNick, 3000);
    
    //  플레이어 레벨
    x = x + (len1*15.0f);
    /*
    char buff2[10];
    sprintf(buff2, "%d", pInfo->getLevel());
    string tempLevel1 = buff2;
    string MyLevel1 = " LV." + tempLevel1;
    // 플레이어 레벨
    CCLabelTTF* pMyLV = CCLabelTTF::create(MyLevel1.c_str(), "HelveticaNeue-Bold", 12);
    pMyLV->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0.0f, 0.0f), accp(x, 510.0f), pMyLV, 3000);
*/
    // -- 아군 빨강 게이지
    myRedGauge = CCSprite::create("ui/battle/battle_gage_1p_b.png");
    myRedGauge->setAnchorPoint(ccp(1.0f, 0.5f));
    myRedGauge->setPosition(accp(9.0f + 277.0f, 468.0f + 12.0f));
    myRedGauge->setScaleX(1.0f);
    this->addChild(myRedGauge, 2550);
    
    // -- 아군 노랑 게이지
    myYellowGauge = CCSprite::create("ui/battle/battle_gage_1p_a.png");
    myYellowGauge->setAnchorPoint(ccp(1.0f, 0.5f));
    myYellowGauge->setPosition(accp(9.0f + 277.0f, 468.0f + 12.0f));
    myYellowGauge->setScaleX(1.0f);
    this->addChild(myYellowGauge, 2560);

    float gauge_scale = 1.0f;
    if (nCallFromNpcBattle > 0){
        gauge_scale = enemyHP / enemyMaxHp;// resultInfo->rival_hp / resultInfo->rival_hp_max;
        if (gauge_scale > 1)gauge_scale=1;
    }
    // -- 적 빨강 게이지
    enemyRedGauge = CCSprite::create("ui/battle/battle_gage_2p_b.png");
    enemyRedGauge->setAnchorPoint(ccp(0.0f, 0.5f));
    enemyRedGauge->setPosition(accp(354.0f, 468.0f + 12.0f));
    enemyRedGauge->setScaleX(gauge_scale);//1.0f);
    this->addChild(enemyRedGauge, 2550);
    
    // -- 적 노랑 게이지
    enemyYellowGauge = CCSprite::create("ui/battle/battle_gage_2p_a.png");
    enemyYellowGauge->setAnchorPoint(ccp(0.0f, 0.5f));
    enemyYellowGauge->setPosition(accp(354.0f, 468.0f + 12.0f));
    enemyYellowGauge->setScaleX(gauge_scale);//1.0f);
    this->addChild(enemyYellowGauge, 2560);
    
    PlayerLastHP = myHP;
    EnemyLastHP = enemyHP;
    
    count = HP_COUNT;

    DrawPlayerHP();
    DrawEnemyHP();
    
    IsSkipped = false;
}

void BattleFullScreen::SkipInit()
{
    // -- 마지막 라운드
    CurrentRound = ConstRoundCount - 1;
    
    // -- 맨 마지막 상황을 연출하기 위해서는 전전 패킷값으로 세팅
    const ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound-1);
    
    myHP = RoundData->attacker_hp;
    enemyHP = RoundData->defender_hp;
    
    PlayerLastHP = myHP;
    EnemyLastHP = enemyHP;
    
    DrawHP(HP_PLAYER, RoundData->attacker_hp, myMaxHp);
    DrawHP(HP_ENEMY, RoundData->defender_hp, enemyMaxHp);
    
    if(0 == nCallFromNpcBattle)
    {
    }
    else if(1 == nCallFromNpcBattle)
    {
        // -- 보스가 이기는 경우 BattleSequence의 맨 마지막 값으로 hp 세팅
        if(0 == rivalBattleInfoFromTrace->battleResult)
        {
            const ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
            DrawHP(HP_ENEMY, RoundData->defender_hp, enemyMaxHp);

            enemyHP = RoundData->defender_hp;
        }
    }
    else if(2 == nCallFromNpcBattle)
    {
        if(0 == QuestUpdateResultInfoFromTrace->questBattleResult)
        {
            const ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
            DrawHP(HP_ENEMY, RoundData->defender_hp, enemyMaxHp);

            enemyHP = RoundData->defender_hp;
        }
    }
    
    myYellowGauge->setScaleX(myHP/myMaxHp);
    myRedGauge->setScaleX(myHP/myMaxHp);
    
//    enemyYellowGauge->setScaleX(enemyHP/enemyMaxHp);
//    enemyRedGauge->setScaleX(enemyHP/enemyMaxHp);

    CCActionInterval* curYellowScale = CCScaleTo::create(1.2f, enemyHP/enemyMaxHp, 1.0f);
    CCActionInterval* curRedScale = CCScaleTo::create(2.0f, enemyHP/enemyMaxHp, 1.0f);
    
    enemyYellowGauge->runAction(curYellowScale);
    enemyRedGauge->runAction(curRedScale);
    
    RoundCount = 1;
    AttackCount = 0;
}

void BattleFullScreen::Skip()
{
    if(IsSkipped) return;
    
    CCLOG("BattleFullScreen::Skip");
    
    CCSize size = GameConst::WIN_SIZE;
    
    IsSkipped = true;
    
    CCSprite* whiteBG = CCSprite::create("ui/home/ui_BG.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setOpacity(0);
    this->addChild(whiteBG, 12000);

    this->stopAllActions();
    
    if(0 == nCallFromNpcBattle)
    {
        if(0 == resultInfo->battleResult)   nFirstAttack = 0;
        if(1 == resultInfo->battleResult)   nFirstAttack = 1;
    }
    else if(1 == nCallFromNpcBattle)
    {
        if(0 == rivalBattleInfoFromTrace->battleResult)   nFirstAttack = 1;
        if(1 == rivalBattleInfoFromTrace->battleResult)   nFirstAttack = 0;
        
    }
    else if(2 == nCallFromNpcBattle)
    {
        if(0 == QuestUpdateResultInfoFromTrace->questBattleResult)   nFirstAttack = 1;
        if(1 == QuestUpdateResultInfoFromTrace->questBattleResult)   nFirstAttack = 0;
    }
    
    this->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                        CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::SkipInit)),
                                        CCDelayTime::actionWithDuration(3.0f),
                                        CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::BattleProc)),
                                        NULL));
   
    whiteBG->runAction(CCSequence::actions(CCFadeTo::actionWithDuration(0.5f, 255),
                                           CCDelayTime::actionWithDuration(1.5f),
                                           CCFadeTo::actionWithDuration(0.5f, 0),
                                           NULL));
}

void BattleFullScreen::DrawHP(int PLAYER_TYPE, float HP, float maxHP)
{
    CCSize size = GameConst::WIN_SIZE;
    
    char buf1[10];
    sprintf(buf1,"%d", (int)HP);
    char buf2[10];
    sprintf(buf2,"%d", (int)maxHP);
    
    std::string strHp;
    strHp.append(buf1).append(" / ").append(buf2);
    CCLabelTTF* pLabel = CCLabelTTF::create(strHp.c_str(), "HelveticaNeue-Bold", 11);
    pLabel->setColor(COLOR_WHITE);
    
    if(HP_PLAYER == PLAYER_TYPE)
    {
        this->removeChildByTag(HP_PLAYER, true);
        //registerLabel(this, ccp(0.5f, 0.5f), accp(size.width/2.0f, 480.0f), pLabel, 3000);
        registerLabel(this, ccp(0.5f, 0.5f), ccp((size.width/2.0f)/2, accp(480.0f)), pLabel, 3000);
        pLabel->setTag(PLAYER_TYPE);
    }
    else
    {
        this->removeChildByTag(HP_ENEMY, true);
        registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2 + (size.width/2.0f)/2, accp(480.0f)), pLabel, 3000);
        pLabel->setTag(HP_ENEMY);
    }
}

void BattleFullScreen::DrawPlayerHP()
{
    CCSize size = GameConst::WIN_SIZE;
    
    //this->removeChildByTag(HP_PLAYER, true);
    
    if(PlayerLastHP <= 0) PlayerLastHP = 0;
    
    DrawHP(HP_PLAYER, PlayerLastHP, myMaxHp);
   
    if(myHP != myMaxHp)
    {
        const int decreaseHP = (PlayerLastHP - myHP) / HP_COUNT;
        PlayerLastHP-=decreaseHP;
        --count;
        this->schedule(schedule_selector(BattleFullScreen::DrawPlayerHP), 0.01f);
    }
    
    if(count<=0)
    {
        //this->removeChildByTag(HP_PLAYER, true);
        
        count = HP_COUNT;
        PlayerLastHP = myHP;
        
        if(PlayerLastHP <= 0) PlayerLastHP = 0;
        
        DrawHP(HP_PLAYER, PlayerLastHP, myMaxHp);
       
        this->unschedule(schedule_selector(BattleFullScreen::DrawPlayerHP));
    }
}

void BattleFullScreen::DrawEnemyHP()
{
    CCSize size = GameConst::WIN_SIZE;
    
    //this->removeChildByTag(HP_ENEMY, true);
    
    if(EnemyLastHP <= 0) EnemyLastHP = 0;

    DrawHP(HP_ENEMY, EnemyLastHP, enemyMaxHp);

    if(enemyHP != enemyMaxHp)
    {
        int decreaseHP = (EnemyLastHP - enemyHP) / HP_COUNT;
        EnemyLastHP-=decreaseHP;
        --count;
        this->schedule(schedule_selector(BattleFullScreen::DrawEnemyHP), 0.001f);
    }

    if(count<=0)
    {
        //this->removeChildByTag(HP_ENEMY, true);
        
        count = HP_COUNT;
        EnemyLastHP = enemyHP;
        
        if(EnemyLastHP <= 0) EnemyLastHP = 0;
        
        DrawHP(HP_ENEMY, EnemyLastHP, enemyMaxHp);
        
        this->unschedule(schedule_selector(BattleFullScreen::DrawEnemyHP));
    }
}

void BattleFullScreen::InitGame()
{
    if(2 == resultInfo->battleResult)
    {
        this->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::actionVS)),
                                            CCDelayTime::actionWithDuration(2.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::SetTouchTrue)),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::DrawBattleProc)),
                                            NULL));
    }
    else
    {
        this->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::actionVS)),
                                            CCDelayTime::actionWithDuration(2.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::SetTouchTrue)),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::BattleProc)),
                                            NULL));
    }
}

void BattleFullScreen::actionVS()
{
    playEffect("audio/ready_01.mp3");
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    CCSprite* VS = CCSprite::create("ui/battle/battle_vs.png");
    VS->setTag(567);
    VS->setAnchorPoint(ccp(0.5f, 0.5f));
    VS->setScale(5.0f);
    int image_width = VS->getTexture()->getContentSizeInPixels().width;
    int image_height = VS->getTexture()->getContentSizeInPixels().height;
    //VS->setPosition(accp(size.width/2 + (344.0f/2.0f), size.height/2 + (494.0f/2.0f)));
    //VS->setPosition(ccp(size.width/2 + accp(image_width)/2, size.height/2 + accp(image_height)/2));
    VS->setPosition(ccp(size.width/2, size.height/2));
    this->addChild(VS, 5550);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.1f, 1.0f);
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.8f);
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.1f, 8.0f, 0.0f);
    //CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.0f);
    
    int *tag = new int(567);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)tag);
    
    VS->runAction(CCSequence::actions(actionScale1, delay1, actionScale2, callBack, NULL));
}

void BattleFullScreen::DrawBattleProc()
{
    CCCallFuncND *Attack = CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::DrawAttack), NULL);
   
    this->runAction(Attack);
}

void BattleFullScreen::BattleProc()
{
        if(RoundCount > 0)
        {
            CCLOG("round start");
            
            const ABattleRound* RoundData = (ABattleRound*)BattleSequence->objectAtIndex(CurrentRound);
            
            if(0 == nFirstAttack%2)
            {
                CCLOG("아군 공격력 : %d", RoundData->attacker_point);
                
                CCDelayTime* delay = CCDelayTime::actionWithDuration(0.0f);

                if(MyLayer->IsFeatureSkill(RoundData->skills))
                {
                    delay = CCDelayTime::actionWithDuration(1.0f);
                    MyLayer->ActiveSkill(RoundData->skills, RoundData);
                    
                    if(0 != RoundData->attacker_heal)
                    {
                        myHP += RoundData->attacker_heal;
                        
                        CreateHealScoreBG(320.0f, 648.0f);
                        CreateHealScore(RoundData->attacker_heal, 320.0f, 652.0f);
                        
                        DrawHP(HP_PLAYER, myHP, myMaxHp);
                        
                        float ratio = myHP / myMaxHp;
                        
                        if(ratio >= 1.0f) ratio = 1.0f;
                        
                        myYellowGauge->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f), NULL));
                        myRedGauge->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f), NULL));
                        CCLOG("heal");
                    }
                }
                
                int* myDamage = new int(RoundData->attacker_point);
                
                this->runAction(CCSequence::actions(delay,
                                                    CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::MyAttack), (void*)myDamage),
                                                    CCDelayTime::actionWithDuration(4.0f),
                                                    CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::BattleProc)),
                                                    NULL));
            }
            else
            {
                CCLOG("적군 공격력 : %d", RoundData->defender_point);
                
                CCDelayTime* delay = CCDelayTime::actionWithDuration(0.0f);
                
                if(EnemyLayer->IsFeatureSkill(RoundData->skills))
                {
                    delay = CCDelayTime::actionWithDuration(1.0f);
                    EnemyLayer->ActiveSkill(RoundData->skills, RoundData);
                    
                    if(0 != RoundData->defender_heal)
                    {
                        enemyHP += RoundData->defender_heal;
                        
                        CreateHealScoreBG(320.0f, 168.0f);
                        CreateHealScore(RoundData->defender_heal, 320.0f, 172.0f);
                        
                        DrawHP(HP_ENEMY, enemyHP, enemyMaxHp);
                            
                        float ratio = enemyHP / enemyMaxHp;
                            
                        if(ratio >= 1.0f) ratio = 1.0f;
                            
                        enemyYellowGauge->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f), NULL));
                        enemyRedGauge->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f), NULL));
                            
                        CCLOG("heal");
                    }

                }

                int* enemyDamage = new int(RoundData->defender_point);
                
                this->runAction(CCSequence::actions(delay,
                                                    CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::EnemyAttack), (void*)enemyDamage),
                                                    CCDelayTime::actionWithDuration(4.0f),
                                                    CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::BattleProc)),
                                                    NULL));
            }
            
            ++nFirstAttack;
            
            ++AttackCount;
            
            if(AttackCount >=2)
            {
                AttackCount = 0;
                ++CurrentRound;
                --RoundCount;
                
                CCLOG("남은 라운드     : %d", RoundCount);
            }
        }        
}

void BattleFullScreen::CheckGameEnd()
{
    if(enemyHP <= 0 || myHP <= 0)
    {
        CCLOG("게임 엔드");
        
        CCCallFunc *callBack = NULL;
        
        if(0 == resultInfo->battleResult)
            callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Lose));
        if(1 == resultInfo->battleResult)
            callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Win));
        if(2 == resultInfo->battleResult)
            callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Draw));
        
        CCDelayTime *delay2sec = CCDelayTime::actionWithDuration(1.0f);
        this->runAction(CCSequence::actions(delay2sec, callBack, NULL));
        
        RoundCount = -5;
    }
}

void BattleFullScreen::DrawAttack()
{
    enemyHP = 0;
    myHP = 0;
    
    EnemyLayer->Attack();
    CCDelayTime *delay1sec = CCDelayTime::actionWithDuration(1.5f);
    CCDelayTime *delay3sec = CCDelayTime::actionWithDuration(1.8f);
    myYellowGauge->runAction(CCSequence::actions(delay1sec, CCScaleTo::actionWithDuration(0.3f, 0.0f, 1.0f), NULL));
    myRedGauge->runAction(CCSequence::actions(delay3sec, CCScaleTo::actionWithDuration(0.2f, 0.0f, 1.0f), NULL));
    
    CCDelayTime *delay2sec = CCDelayTime::actionWithDuration(1.0f);
    CCCallFunc *hitMy = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::HitMy));
    this->runAction(CCSequence::actions(delay2sec, hitMy, delay2sec, NULL));

    MyLayer->Attack();
    
    enemyYellowGauge->runAction(CCSequence::actions(delay1sec, CCScaleTo::actionWithDuration(0.3f, 0.0f, 1.0f), NULL));
    enemyRedGauge->runAction(CCSequence::actions(delay3sec, CCScaleTo::actionWithDuration(0.2f, 0.0f, 1.0f), NULL));
    
    CCCallFunc *hitEnemy = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::HitEnemy));
    CCCallFunc *call = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::CheckGameEnd));
    this->runAction(CCSequence::actions(delay2sec, hitEnemy, delay2sec,  call, NULL));
}

void BattleFullScreen::MyCriticalSkillActive()
{
    MyLayer->ActiveCriticalSkill();
}

void BattleFullScreen::EnemyCriticalSkillActive()
{
    EnemyLayer->ActiveCriticalSkill();
}
/*
void BattleFullScreen::CreateAttackScore(int* _damage, float x, float y)
{
    char buff[10];
    sprintf(buff, "%d", *_damage);
    CCLabelTTF* Damage = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 17);
    Damage->setScale(0.0f);
    Damage->setTag(654);
    Damage->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(x, y), Damage, 1000);
    
    Damage->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                          CCScaleTo::actionWithDuration(0.0f, 1.0f),
                                          CCShake::create(0.1f, 10.0f, x, y),
                                          CCDelayTime::actionWithDuration(0.5f),
                                          CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::RemoveDamage)),
                                          NULL));

}

void BattleFullScreen::CreateHealScore(int _heal, float x, float y)
{
    char buff[10];
    sprintf(buff, "%d", _heal);
    CCLabelTTF* Heal = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 17);
    Heal->setScale(0.0f);
    Heal->setTag(654);
    Heal->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(x, y), Heal, 1000);
    
    Heal->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, 1.0f),
                                        CCDelayTime::actionWithDuration(0.5f),
                                        CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::RemoveDamage)),
                                        NULL));
}
*/
void BattleFullScreen::EnemyAttack(CCNode* sender, void *data)
{
    if(myHP > 0 && enemyHP > 0)
    {
        int* damage = NULL;
        
        if(NULL == data)
        {
            myHP = 0;
            CCLOG("적군 공격");
        }
        else
        {
            damage = (int*)data;
            
            myHP = myHP - ((*damage));
            CCLOG("아군 HP : %f", myHP);
        }
        
        EnemyLayer->Attack();

        float ratio = myHP / myMaxHp;

        if(ratio <= 0.0f) ratio = 0.0f;
        
        myYellowGauge->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.5f),
                                                     CCScaleTo::actionWithDuration(0.3f, ratio, 1.0f),
                                                     NULL));
        myRedGauge->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.8f),
                                                  CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f),
                                                  NULL));
        
        this->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::DrawPlayerHP)),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::HitMy)),
                                            CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::CheckGameEnd)),
                                            NULL));
        
        CreateAttackScoreBG(320.0f, 648.0f);
        
        CreateAttackScore(damage, 320.0f, 652.0f);
    }
    
    CC_SAFE_DELETE(data);
}

void BattleFullScreen::MyAttack(CCNode* sender, void *data)
{
    if(myHP > 0 && enemyHP > 0)
    {
        int* damage = NULL;
        
        if(NULL == data)
        {
            enemyHP = 0;
            CCLOG("아군 공격");
        }
        else
        {
            damage = (int*)data;
            
            enemyHP = enemyHP - ((*damage));
            CCLOG("적군 HP : %f", enemyHP);
        }
        
        MyLayer->Attack();
        
        float ratio = enemyHP / enemyMaxHp;

        if(ratio <= 0.0f) ratio = 0.0f;

        enemyYellowGauge->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.5f),
                                                        CCScaleTo::actionWithDuration(0.3f, ratio, 1.0f),
                                                        NULL));
        
        enemyRedGauge->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.8f),
                                                     CCScaleTo::actionWithDuration(0.2f, ratio, 1.0f),
                                                     NULL));
        
        this->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::DrawEnemyHP)),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::HitEnemy)),
                                            CCDelayTime::actionWithDuration(1.0f),
                                            CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::CheckGameEnd)),
                                            NULL));
        
        CreateAttackScoreBG(320.0f, 168.0f);
        
        CreateAttackScore(damage, 320.0f, 172.0f);
    }
    
    CC_SAFE_DELETE(data);
}


void BattleFullScreen::CreateAttackScoreBG(float x, float y)
{
    CCSprite* AttackBg = CCSprite::create("ui/battle/battle_score_red_01.png");
    AttackBg->setAnchorPoint(ccp(0.5f, 0.5f));
    AttackBg->setPosition(accp(x, y));
    AttackBg->setOpacity(0);
    AttackBg->setTag(589);
    this->addChild(AttackBg, 1000);
    
    int *AttackBgTag = new int(589);
    
    AttackBg->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.5f),
                                            CCFadeTo::actionWithDuration(0.3f, 255),
                                            CCDelayTime::actionWithDuration(0.87f),
                                            CCFadeTo::actionWithDuration(0.5f, 0),
                                            CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)AttackBgTag),
                                            NULL));
}

void BattleFullScreen::CreateAttackScore(int* _damage, float x, float y)
{
    char buff[10];
    sprintf(buff, "%d", *_damage);
    CCLabelTTF* Damage = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 20);
    Damage->setTag(654);
    Damage->setScale(0.0f);
    Damage->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(x, y), Damage, 1000);
    
    Damage->runAction(CCSequence::actions(CCDelayTime::actionWithDuration(1.5f),
                                          CCScaleTo::actionWithDuration(0.0f, 1.0f),
                                          CCShake::create(0.5f, 2.0f, x, y),
                                          CCDelayTime::actionWithDuration(0.5f),
                                          CCFadeTo::actionWithDuration(0.5f, 0),
                                          CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::RemoveDamage)),
                                          NULL));
    
}

void BattleFullScreen::CreateHealScoreBG(float x, float y)
{
    CCSprite* HealBg = CCSprite::create("ui/battle/battle_score_green.png");
    HealBg->setAnchorPoint(ccp(0.5f, 0.5f));
    HealBg->setPosition(accp(x, y));
    HealBg->setScale(0.0f);
    HealBg->setTag(589);
    this->addChild(HealBg, 1000);
    
    int *HealBgTag = new int(589);
    
    HealBg->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.1f, 1.0f),
                                          CCDelayTime::actionWithDuration(0.6f),
                                          CCCallFuncND::actionWithTarget(this, callfuncND_selector(BattleFullScreen::removeSpr), (void*)HealBgTag),
                                          NULL));
}

void BattleFullScreen::CreateHealScore(int _heal, float x, float y)
{
    char buff[10];
    sprintf(buff, "%d", _heal);
    CCLabelTTF* Heal = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 20);
    Heal->setScale(0.0f);
    Heal->setTag(654);
    Heal->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(x, y), Heal, 1000);
    
    Heal->runAction(CCSequence::actions(CCScaleTo::actionWithDuration(0.2f, 1.0f),
                                        CCDelayTime::actionWithDuration(0.5f),
                                        CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::RemoveDamage)),
                                        NULL));
}

void BattleFullScreen::RemoveDamage()
{
    this->removeChildByTag(654,true);
}

void BattleFullScreen::HitEnemy()
{
    EnemyLayer->actionHitNShake();
}

void BattleFullScreen::HitMy()
{
    MyLayer->actionHitNShake();
}

void BattleFullScreen::Win()
{

    
    CCCallFunc* callBackWin = CCCallFunc::create(this, callfunc_selector(BattleFullScreen::cbWin));
    CCCallFunc* callBackRes = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Result));
    
    this->runAction(CCSequence::create(callBackWin, callBackRes));
    
    
/*
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    float x = size.width;
    
    CCSprite* Ko = CCSprite::create("ui/quest/quest_ko.png");
    Ko->setAnchorPoint(ccp(0.5, 0.5));
    Ko->setScale(1.0f);
    
    if (nCallFromNpcBattle == 0 || nCallFromNpcBattle == 100){
        Ko->setPosition(ccp(x/2, accp(620 + 126)));
    }
    else{
        Ko->setPosition(ccp(x/2, accp(104 + 126)));
    }
*/
    /*
    if (nCallFromNpcBattle > 0){
        Ko->setPosition(ccp(x/2, accp(104 + 126)));
    }
    else{
        
        Ko->setPosition(ccp(x/2, accp(620 + 126)));
    }
     */
/*
    this->addChild(Ko, 1400);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.05f, 8.0f, 0.0f);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.0f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Result));
    
    Ko->runAction(CCSequence::actions(actionScale1, delay1, actionScale2, delay2, callBack, NULL));
    
    playEffect("audio/ko_01.mp3");
*/ 
}

void BattleFullScreen::cbWin()
{
    CCSize size = this->getContentSize();
    
    TraceResultLayer* pTraceResultLayer = new TraceResultLayer;
    pTraceResultLayer->setPosition(size.width/2.0f, size.height/2.0f);
    pTraceResultLayer->getInstance()->startKo();
    this->addChild(pTraceResultLayer, 3000);
}


void BattleFullScreen::Lose()
{
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    float x = size.width;
    
    CCSprite* Ko = CCSprite::create("ui/quest/quest_ko.png");
    Ko->setAnchorPoint(ccp(0.5, 0.5));
    Ko->setScale(1.0f);
    
    if (nCallFromNpcBattle == 0 || nCallFromNpcBattle == 100){
        Ko->setPosition(ccp(x/2, accp(104 + 126)));
    }
    else{
        Ko->setPosition(ccp(x/2, accp(620 + 126)));
    }
    /*
    if (nCallFromNpcBattle > 0){
        Ko->setPosition(ccp(x/2, accp(620 + 126)));
    }
    else{
        Ko->setPosition(ccp(x/2, accp(104 + 126)));
    }
     */
    
    this->addChild(Ko, 1400);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.05f, 8.0f, 0.0f);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.0f);
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Result));
    
    Ko->runAction(CCSequence::actions(actionScale1, delay1, actionScale2, delay2, callBack, NULL));
    
    playEffect("audio/ko_01.mp3");
}

void BattleFullScreen::Draw()
{
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    float x = size.width;
    
    CCSprite* Ko = CCSprite::create("ui/quest/quest_ko.png");
    Ko->setAnchorPoint(ccp(0.5, 0.5));
    Ko->setScale(1.0f);
    Ko->setPosition(accp(x, 620 + 126));
    this->addChild(Ko, 1400);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    CCDelayTime *delay1 = CCDelayTime::actionWithDuration(0.5f);
    CCFiniteTimeAction* actionScale2 = CCScaleTo::actionWithDuration(0.05f, 8.0f, 0.0f);
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(1.0f);
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(BattleFullScreen::Result));
    Ko->runAction(CCSequence::actions(actionScale1, delay1, actionScale2, delay2, callBack, NULL));
    //////////////////////////////////////////////////////////////////////
    
    CCSprite* Ko1 = CCSprite::create("ui/quest/quest_ko.png");
    Ko1->setAnchorPoint(ccp(0.5, 0.5));
    Ko1->setScale(1.0f);
    Ko1->setPosition(accp(x, 104 + 126));
    this->addChild(Ko1, 1400);
    
    CCFiniteTimeAction* actionScale3 = CCScaleTo::actionWithDuration(0.05f, 1.0f);
    CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.5f);
    CCFiniteTimeAction* actionScale4 = CCScaleTo::actionWithDuration(0.05f, 8.0f, 0.0f);
    CCDelayTime *delay4 = CCDelayTime::actionWithDuration(1.0f);

    Ko1->runAction(CCSequence::actions(actionScale3, delay3, actionScale4, delay4, NULL));
    
    playEffect("audio/ko_01.mp3");
}

void BattleFullScreen::Result()
{
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG->stopBackgroundMusic();
    }
    
    if (nCallFromNpcBattle == 0 || nCallFromNpcBattle == 100){
        BattleDuelLayer* layer = BattleDuelLayer::getInstance();
        layer->InitLayer(3);
    }
    else
    {
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->callBackFromBattle(nCallFromNpcBattle);
    }
}

void BattleFullScreen::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if(IsSkipped) return;

    if(ConstRoundCount > 1)
    {
        Skip();
    }
}

void BattleFullScreen::removeSpr(CCNode* sender, void* _tag)
{
    int* tag = (int*)_tag;
    this->removeChildByTag(*tag, true);
    CC_SAFE_DELETE(tag);
}

