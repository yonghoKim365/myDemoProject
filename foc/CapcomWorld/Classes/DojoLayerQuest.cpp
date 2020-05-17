//
//  DojoLayerQuest.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#include "DojoLayerQuest.h"
#include "ARequestSender.h"
#include "AResponseParser.h"
#include "PlayerInfo.h"

DojoLayerQuest* DojoLayerQuest::instance = NULL;

DojoLayerQuest* DojoLayerQuest::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

DojoLayerQuest::DojoLayerQuest(CCSize layerSize) : pChapterLayer(NULL)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    instance = this;
    this->setContentSize(layerSize);
    
    //readQuest();
        
    CCSprite* pSprite = CCSprite::create("ui/home/ui_BG.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite, 0);
    
    InitUI();    
}

DojoLayerQuest::~DojoLayerQuest()
{
    this->removeAllChildrenWithCleanup(true);
}

void DojoLayerQuest::InitUI()
{
    pChapterLayer = new ChapterLayer(this->getContentSize());

    unsigned long length = 0;
    std::string pathKey = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath("quests.xml");
    
    CCLog("DojoLayerQuest::InitUI,pathKey=%s", pathKey.c_str() );
    
    unsigned char *data = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    if (data == NULL || length == 0)
        return;
    
    //CCLog("DojoLayerQuest::InitUI,data:%d", data);
    
    xmlDocPtr doc = xmlReadMemory((const char *)data, length, "test.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    
    CCArray *questLocalList = new CCArray();
    AResponseParser::getInstance()->parseQuestXML(root_element, questLocalList);

    pChapterLayer->SetChapterData(questLocalList, PlayerInfo::getInstance()->questList);
    
    const int unlockCount = pChapterLayer->getUnlockChapterCount();
    const float LayerHeight = CHAPTER_HEIGHT * unlockCount + 10 * unlockCount;
    
    pChapterLayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
    
    pChapterLayer->InitUI();
    
    pChapterLayer->LayerStartPos = (748 - LayerHeight)/SCREEN_ZOOM_RATE;
    
    pChapterLayer->setAnchorPoint(ccp(0, 0));
    
    pChapterLayer->setPosition(accp(0, 748 - LayerHeight));
    
    //pChapterLayer->setPosition(accp(0, 0));
    
    this->addChild(pChapterLayer);

    
}

void DojoLayerQuest::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
}

void DojoLayerQuest::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    //CCLog("touch moved,x:%f",location.x);
    
}

void DojoLayerQuest::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: Ï¢??Î•?Í∞??????? ?∞Ï?Î•?Ï∂???©Î???
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    //CCPoint location = touch->locationInView(touch->view()); // deprecated
    CCPoint location = touch->getLocationInView();
    
    //: UI Ï¢??Î•?GLÏ¢??Î°?Î≥?≤Ω?©Î???    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
}


/*
 
 <?xml version="1.0" encoding="utf-8"?><response><res>0</res><message></message><quests><quest id="20011" begin="1360894529" end="1360898679" progress="100" clear="1" max_progress="999" enemy="0"></quest><quest id="20012" begin="1360900226" end="1360904386" progress="100" clear="1" max_progress="999" enemy="0"></quest></quests></response>
*/