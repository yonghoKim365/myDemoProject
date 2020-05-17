
#include "Common_Guide.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "ProductManager.h"
#include "MGTSoundManager.h"

enum
{
    kDepth_dimmed = 0,
    kDepth_Content,

    kDepth_Bg,
    kDepth_Yooni,
    kDepth_Text,
    kTagMenu,
    
    kTagMenuItem_Close
};

Common_Guide::Common_Guide()
{
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(cocos2d::Layer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(cocos2d::Layer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(cocos2d::Layer::onTouchEnded, this);
    
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);

    
    addSoundObserver();
}


Common_Guide::~Common_Guide()
{
    removeSoundObserver();
    
    _eventDispatcher->removeEventListener(m_touchlistener);
}


Common_Guide * Common_Guide::create(common_guide::eGuideType type, bool hasDimmed)
{
    Common_Guide * pLayer = new Common_Guide();
    if( pLayer && pLayer->initWithSetting(type, hasDimmed))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool Common_Guide::initWithSetting(common_guide::eGuideType type, bool hasDimmed)
{
    if (!MGTLayer::init())
    {
        return false;
    }
    
    auto winSize = Director::getInstance()->getWinSize();
    
    m_eState = common_guide::HIDE;
    m_eGuideType = type;
    m_sndFile = "";
    m_hasDimmed = hasDimmed;
    m_bTouchEnabled = false;
    
    PsdParser::parseToPsdJSON("common_popup_guide.json", &m_psdDictionary, true);

    std::string textfile;
    std::string yoonifile;

    
    yoonifile = "common_popup_guide_yooni.png";
    textfile = StringUtils::format("common_popup_guide_text_%d_%d.png", ProductManager::getInstance()->getCurrentStep(), ProductManager::getInstance()->getCurrentType());
    
    
    if (m_hasDimmed)
    {
        m_dimmedLayer = DimmedLayer::create(Color4B(0, 0, 0, 255*0.5f));
        m_dimmedLayer->setOpacity(0.0f);
        m_dimmedLayer->setVisible(false);
        this->addChild(m_dimmedLayer, kDepth_dimmed);
    }
    
    
    m_content = Sprite::create();
    m_content->setContentSize(Size(winSize.width, winSize.height));
    m_content->setPosition(Vec2(winSize.width/2, winSize.height/2));
    m_content->setVisible(false);
    this->addChild(m_content, kDepth_Content);
    
    Sprite* bg = Sprite::create(getCommonFilePath("img", "common_popup_board.png"));
    bg->setPosition(PsdParser::getPsdPosition("common_popup_board", &m_psdDictionary));
    m_content->addChild(bg, kDepth_Bg, kDepth_Bg);
    
    Sprite* yooni = Sprite::create(getCommonFilePath("img", yoonifile));
    yooni->setPosition(PsdParser::getPsdPosition("common_popup_yooni", &m_psdDictionary));
    m_content->addChild(yooni, kDepth_Yooni, kDepth_Yooni);
    
    
    Sprite* text = Sprite::create(getCommonFilePath("img", textfile));
    text->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
    m_content->addChild(text, kDepth_Text, kDepth_Text);
    
    
    auto closeItem = MenuItemImage::create(getCommonFilePath("img", "common_popup_btn_close_n.png"),
                                          getCommonFilePath("img", "common_popup_btn_close_p.png"),
                                          getCommonFilePath("img", "common_popup_btn_close_p.png"),
                                          CC_CALLBACK_1(Common_Guide::guideMenuCallback, this));
    
    closeItem->setTag(kTagMenuItem_Close);
    closeItem->setPosition(PsdParser::getPsdPosition("common_popup_btn_close_n", &m_psdDictionary));
    
    
    Menu* menu = Menu::create(closeItem, nullptr);
    menu->setPosition(Vec2::ZERO);
    m_content->addChild(menu, kTagMenu, kTagMenu);
    
    
    
//    show(0.0f);
    
    return true;
}


void Common_Guide::onEnter()
{
    MGTLayer::onEnter();
    onViewLoad();
}

void Common_Guide::onExit()
{
    MGTLayer::onExit();
}

#pragma mark touches function

bool Common_Guide::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    if (m_bTouchEnabled)
    {
        
    }
    
    return false;
}

void Common_Guide::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    Vec2 prePoint = pTouch->getPreviousLocation();
    
    
}
void Common_Guide::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}


void Common_Guide::onViewLoad()
{
    MGTLayer::onViewLoad();
    
    
}


void Common_Guide::onViewLoaded()
{
    MGTLayer::onViewLoaded();
}



void Common_Guide::show(float duration)
{
    m_eState = common_guide::SHOW;
    
    if (m_hasDimmed)
    {
        m_dimmedLayer->show(duration);
    }
    
    m_content->setOpacity(0.0f);
    auto seq = Sequence::create(
                                EaseSineOut::create(FadeTo::create(duration, 255.0f)),
                                CallFunc::create( CC_CALLBACK_0(Common_Guide::showComplete, this)),
                                nullptr);
    m_content->runAction(seq);
    
    m_content->setVisible(true);
}

void Common_Guide::showComplete()
{
    m_bTouchEnabled = true;
    
}

void Common_Guide::hide(float duration)
{
    m_eState = common_guide::HIDE;
    m_bTouchEnabled = false;
    
    if (m_hasDimmed)
    {
        m_dimmedLayer->hide(duration);
    }

    
    auto seq = Sequence::create(
                                EaseSineOut::create(FadeTo::create(duration, 0.0f)),
                                CallFunc::create( CC_CALLBACK_0(Common_Guide::hideComplete, this)),
                                nullptr);
    m_content->runAction(seq);
}

void Common_Guide::hideComplete()
{
    m_content->setVisible(false);
}



void Common_Guide::guideMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    
    MGTSoundManager::getInstance()->playEffect(ProductManager::getInstance()->getCommonFilePath("snd", "common_sfx_btn_02.mp3"));
    if (item->getTag() == kTagMenuItem_Close)
    {
        m_Delegate();
    }
}


#pragma mark narration finished callback function

void Common_Guide::onNarrationFinishedCallback(std::string fileName)
{
    std::string name = MGTUtils::getInstance()->stringTokenizer(m_sndFile, "/", false);
    
    if (fileName == name)
    {
        
    }
}


void Common_Guide::setDelegate(std::function<void()> delegate)
{
    m_Delegate = delegate;
}
