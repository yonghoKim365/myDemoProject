
#include "Common_Ending.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "ProductManager.h"

enum
{
    kDepth_dimmed = 0,
    kDepth_Content,
    kDepth_Gaf,
    kDepth_Text,
    kDepth_Box,
};

Common_Ending::Common_Ending()
{
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(cocos2d::Layer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(cocos2d::Layer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(cocos2d::Layer::onTouchEnded, this);
    
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);

    
    addSoundObserver();
}


Common_Ending::~Common_Ending()
{
    removeSoundObserver();
    
    _eventDispatcher->removeEventListener(m_touchlistener);

}


Common_Ending * Common_Ending::create(common_ending::eEndingType type, bool hasDimmed)
{
    Common_Ending * pLayer = new Common_Ending();
    if( pLayer && pLayer->initWithSetting(type, hasDimmed))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool Common_Ending::initWithSetting(common_ending::eEndingType type, bool hasDimmed)
{
    if (!MGTLayer::init())
    {
        return false;
    }
    
    auto winSize = Director::getInstance()->getWinSize();
    
    m_eState = common_ending::HIDE;
    m_eEndingType = type;
    m_sndFile = "";
    m_hasDimmed = hasDimmed;
    m_bTouchEnabled = false;
    
//    PsdParser::parseToPsdJSON("d_b001_day1_ta02.json", &m_psdDictionary);

    std::string filename;
    if (m_eEndingType == common_ending::STEP1)
    {
        
    }
    else if (m_eEndingType == common_ending::STEP2)
    {
        
    }
    else if (m_eEndingType == common_ending::STEP3)
    {
        
    }
    
    if (m_hasDimmed)
    {
        m_dimmedLayer = DimmedLayer::create(Color4B(0, 0, 0, 255*0.7f));
        m_dimmedLayer->setOpacity(0.0f);
        m_dimmedLayer->setVisible(false);
        this->addChild(m_dimmedLayer, kDepth_dimmed);
    }
    
    
    m_content = Sprite::create();
    m_content->setContentSize(Size(winSize.width, winSize.height));
    m_content->setAnchorPoint(Vec2::ZERO);
    m_content->setPosition(Vec2(0, winSize.height - 120.0f));
    m_content->setVisible(false);
    this->addChild(m_content, kDepth_Content);
    
    
    Sprite* text = Sprite::create("");
    text->setPosition(Vec2(639, 352));
    m_content->addChild(text, kDepth_Text, kDepth_Text);
    
    Sprite* box = Sprite::create();
    box->setContentSize(Size(450, 450));
    box->setTextureRect(Rect(0, 0, box->getContentSize().width, box->getContentSize().height));
//    box->setColor(Color3B::RED);
    box->setOpacity(0);
    box->setPosition(Vec2(winSize.width/2, winSize.height/2));
    m_content->addChild(box, kDepth_Box, kDepth_Box);
    
    show();
    
    return true;
}


void Common_Ending::onEnter()
{
    MGTLayer::onEnter();
    onViewLoad();
}

void Common_Ending::onExit()
{
    MGTLayer::onExit();
}

#pragma mark touches function

bool Common_Ending::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    if (m_bTouchEnabled)
    {
        
    }
    
    return false;
}

void Common_Ending::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    Vec2 prePoint = pTouch->getPreviousLocation();
    
    
}
void Common_Ending::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}


void Common_Ending::onViewLoad()
{
    MGTLayer::onViewLoad();
    
    
}


void Common_Ending::onViewLoaded()
{
    MGTLayer::onViewLoaded();
}



void Common_Ending::show()
{
    m_eState = common_ending::SHOW;
    
    if (m_hasDimmed)
    {
        m_dimmedLayer->show(0.5f);
    }
    
    m_content->setPosition(Vec2(0, winSize.height- 120.0f));
    
    auto action = EaseBackOut::create(MoveTo::create(0.5f, Vec2(0, 0.0f)));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Ending::showComplete, this)),
                                nullptr);
    m_content->runAction(seq);
    
    m_content->setVisible(true);
}

void Common_Ending::showComplete()
{
    m_bTouchEnabled = true;
}

void Common_Ending::hide()
{
    m_eState = common_ending::HIDE;
    m_bTouchEnabled = false;
    
    if (m_hasDimmed)
    {
        m_dimmedLayer->hide(0.5f);
    }

    
    auto action = EaseBackOut::create(MoveTo::create(0.5f, Vec2(0.0f, winSize.height- 120.0f)));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Ending::hideComplete, this)),
                                nullptr);
    m_content->runAction(seq);
}

void Common_Ending::hideComplete()
{
    m_content->setVisible(false);
}


#pragma mark narration finished callback function

void Common_Ending::onNarrationFinishedCallback(std::string fileName)
{
    std::string name = MGTUtils::getInstance()->stringTokenizer(m_sndFile, "/", false);
    
    if (fileName == name)
    {
        
    }
}


void Common_Ending::setTouchedDelegate(std::function<void()> delegate)
{
    m_touchedDelegate = delegate;
}
