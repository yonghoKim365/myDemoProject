
#include "Common_Popup.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "ProductManager.h"
#include "MGTSoundManager.h"
#include "DeviceUtilManager.h"
enum
{
    kDepth_dimmed = 0,
    kDepth_Content,

    kDepth_Bg,
    kDepth_Yooni,
    kDepth_Text,
    kDepth_Text1,
    kDepth_Text2,
    kDepth_Text3,
    kTagMenu,
    
    kTagMenuItem_No,
    kTagMenuItem_Yes,
};

Common_Popup::Common_Popup()
{
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(cocos2d::Layer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(cocos2d::Layer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(cocos2d::Layer::onTouchEnded, this);
    
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);

    
    addSoundObserver();
}


Common_Popup::~Common_Popup()
{
    removeSoundObserver();
    
    _eventDispatcher->removeEventListener(m_touchlistener);
}


Common_Popup * Common_Popup::create(common_popup::ePopupType type, bool hasDimmed)
{
    Common_Popup * pLayer = new Common_Popup();
    if( pLayer && pLayer->initWithSetting(type, hasDimmed))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool Common_Popup::initWithSetting(common_popup::ePopupType type, bool hasDimmed)
{
    if (!MGTLayer::init())
    {
        return false;
    }
    
    auto winSize = Director::getInstance()->getWinSize();
    
    m_eState = common_popup::HIDE;
    m_ePopupType = type;
    m_sndFile = "";
    m_hasDimmed = hasDimmed;
    m_bTouchEnabled = false;
    m_isSaveComplete = false;
    m_count = 0;
    
    PsdParser::parseToPsdJSON("common_popup_alert.json", &m_psdDictionary, true);

    
    
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
    
    std::string yoonifile = "common_popup_alert_yooni.png";
    Sprite* yooni = Sprite::create(getCommonFilePath("img", yoonifile));
    yooni->setPosition(PsdParser::getPsdPosition("common_popup_yooni", &m_psdDictionary));
    m_content->addChild(yooni, kDepth_Yooni, kDepth_Yooni);
    

//    std::string textfile1 = "common_popup_text_out.png";
//    std::string textfile2 = "common_popup_text_save.png";
//    std::string textfile3 = "common_popup_text_delete.png";
//    
//    Sprite* text1 = Sprite::create(getCommonFilePath("img", textfile1));
//    text1->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
//    text1->setVisible(false);
//    m_content->addChild(text1, kDepth_Text1, kDepth_Text1);
//    
//    Sprite* text2 = Sprite::create(getCommonFilePath("img", textfile2));
//    text2->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
//    text2->setVisible(false);
//    m_content->addChild(text2, kDepth_Text2, kDepth_Text2);
//    
//    Sprite* text3 = Sprite::create(getCommonFilePath("img", textfile3));
//    text3->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
//    text3->setVisible(false);
//    m_content->addChild(text3, kDepth_Text3, kDepth_Text3);
    
    
    
    auto noItem = MenuItemImage::create(getCommonFilePath("img", "common_popup_btn_no_n.png"),
                                           getCommonFilePath("img", "common_popup_btn_no_p.png"),
                                           getCommonFilePath("img", "common_popup_btn_no_p.png"),
                                           CC_CALLBACK_1(Common_Popup::alertMenuCallback, this));
    
    auto yesItem = MenuItemImage::create(getCommonFilePath("img", "common_popup_btn_yes_n.png"),
                                        getCommonFilePath("img", "common_popup_btn_yes_p.png"),
                                        getCommonFilePath("img", "common_popup_btn_yes_p.png"),
                                        CC_CALLBACK_1(Common_Popup::alertMenuCallback, this));
    
    
    noItem->setTag(kTagMenuItem_No);
    noItem->setPosition(PsdParser::getPsdPosition("common_popup_btn_no_n", &m_psdDictionary));
    
    yesItem->setTag(kTagMenuItem_Yes);
    yesItem->setPosition(PsdParser::getPsdPosition("common_popup_btn_yes_n", &m_psdDictionary));
    
    Menu* menu = Menu::create(noItem, yesItem, nullptr);
    menu->setPosition(Vec2::ZERO);
    m_content->addChild(menu, kTagMenu, kTagMenu);
    
    
//    show(0.0f);
    
    return true;
}


void Common_Popup::onEnter()
{
    MGTLayer::onEnter();
    onViewLoad();
}

void Common_Popup::onExit()
{
    MGTLayer::onExit();
}

#pragma mark touches function

bool Common_Popup::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    if (m_bTouchEnabled)
    {
        
    }
    
    return false;
}

void Common_Popup::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    Vec2 prePoint = pTouch->getPreviousLocation();
    
    
}
void Common_Popup::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}


void Common_Popup::onViewLoad()
{
    MGTLayer::onViewLoad();
    
    
}


void Common_Popup::onViewLoaded()
{
    MGTLayer::onViewLoaded();
}

common_popup::ePopupType Common_Popup::getType()
{
    return m_ePopupType;
}

void Common_Popup::show(common_popup::ePopupType type, float duration)
{
    m_eState = common_popup::SHOW;
    
    if (m_hasDimmed)
    {
        log("common popup show");
        m_dimmedLayer->show(duration);
    }
    
    
//    Sprite* text1 = (Sprite*)m_content->getChildByTag(kDepth_Text1);
//    Sprite* text2 = (Sprite*)m_content->getChildByTag(kDepth_Text2);
//    Sprite* text3 = (Sprite*)m_content->getChildByTag(kDepth_Text3);
//    
//    text1->setVisible(false);
//    text2->setVisible(false);
//    text3->setVisible(false);
    
    std::string textfile;
    
    m_ePopupType = type;
    
    
    Sprite* text = (Sprite*)m_content->getChildByTag(kDepth_Text);
    if (text)
    {
        text->removeFromParentAndCleanup(true);
    }
    
    if (type == common_popup::EXIT)
    {
        Menu* menu = (Menu*)m_content->getChildByTag(kTagMenu);
        menu->setVisible(true);
        
        textfile = "common_popup_text_out.png";
        
        
        Sprite* text = Sprite::create(MGTResourceUtils::getInstance()->getCommonFilePath("img", textfile));
        text->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
        m_content->addChild(text, kDepth_Text, kDepth_Text);
    }
    else if (type == common_popup::DRAWING_SAVE)
    {
        Menu* menu = (Menu*)m_content->getChildByTag(kTagMenu);
        menu->setVisible(true);
        
        
        textfile = "common_popup_text_save.png";
        
        Sprite* text = Sprite::create(MGTResourceUtils::getInstance()->getCommonFilePath("img", textfile));
        text->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
        m_content->addChild(text, kDepth_Text, kDepth_Text);
        
    }
    else if (type == common_popup::DRAWING_DELETE)
    {
        Menu* menu = (Menu*)m_content->getChildByTag(kTagMenu);
        menu->setVisible(true);
        
        textfile = "common_popup_text_delete.png";
        
        Sprite* text = Sprite::create(MGTResourceUtils::getInstance()->getCommonFilePath("img", textfile));
        text->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
        m_content->addChild(text, kDepth_Text, kDepth_Text);
    }
    else if (type == common_popup::DRAWING_SAVING)
    {
        log("POPUP DRAWING SAVING ");
        Menu* menu = (Menu*)m_content->getChildByTag(kTagMenu);
        menu->setVisible(false);
        
        DeviceUtilManager::getInstance()->showSavePopup();
    }
    else if (type == common_popup::DRAWING_SAVE_COMPLETE)
    {
        log("POPUP DRAWING COMPLETE ");
        
        Menu* menu = (Menu*)m_content->getChildByTag(kTagMenu);
        menu->setVisible(false);
        
        m_isSaveComplete = true;
        
        DeviceUtilManager::getInstance()->saveDone();
        saveComplete();
        
        log("------------------------------------------------SAVE COMPLETE");
    }
    
    
    
    m_content->setOpacity(0.0f);
    auto seq = Sequence::create(
                                EaseSineOut::create(FadeTo::create(duration, 255.0f)),
                                CallFunc::create( CC_CALLBACK_0(Common_Popup::showComplete, this)),
                                nullptr);
    m_content->runAction(seq);
    
    m_content->setVisible(true);
}

void Common_Popup::showComplete()
{
    m_bTouchEnabled = true;
    
}

void Common_Popup::hide(float duration)
{
    m_eState = common_popup::HIDE;
    m_bTouchEnabled = false;
    
    if (m_hasDimmed)
    {
        m_dimmedLayer->hide(duration);
    }

    
    auto seq = Sequence::create(
                                EaseSineOut::create(FadeTo::create(duration, 0.0f)),
                                CallFunc::create( CC_CALLBACK_0(Common_Popup::hideComplete, this)),
                                nullptr);
    m_content->runAction(seq);
}

void Common_Popup::hideComplete()
{
    m_content->setVisible(false);
}


void Common_Popup::alertMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    
    MGTSoundManager::getInstance()->playEffect(ProductManager::getInstance()->getCommonFilePath("snd", "common_sfx_btn_02.mp3"));
    
    if (item->getTag() == kTagMenuItem_No)
    {
        m_Delegate(false);
    }
    else if (item->getTag() == kTagMenuItem_Yes)
    {
        m_Delegate(true);
    }
}

#pragma mark narration finished callback function

void Common_Popup::onNarrationFinishedCallback(std::string fileName)
{
    std::string name = MGTUtils::getInstance()->stringTokenizer(m_sndFile, "/", false);
    
    if (fileName == name)
    {
        
    }
}

void Common_Popup::setDelegate(std::function<void(bool)> delegate)
{
    m_Delegate = delegate;
}


void Common_Popup::saveComplete()
{
//    Sprite* text = Sprite::create(MGTResourceUtils::getInstance()->getCommonFilePath("img", "common_popup_text_storage_finish.png"));
//    text->setPosition(PsdParser::getPsdPosition("common_popup_text", &m_psdDictionary));
//    m_content->addChild(text, kDepth_Text, kDepth_Text);
}