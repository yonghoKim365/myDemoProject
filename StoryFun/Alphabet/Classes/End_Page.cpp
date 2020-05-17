#include "End_Page.h"

enum
{
    kTagContainer = 0,
    kTagBg,
    kTagGafObject,
    kTagText,
    
    kTagMenu,
    kTagMenuItem_Replay,
    kTagMenuItem_Next,
    kTagMenuItem_Exit,
};


End_Page::End_Page():
m_nTitleNum(0)
{

}

End_Page::~End_Page()
{
}

Scene* End_Page::createScene()
{
    auto scene = Scene::create();

    auto layer = End_Page::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool End_Page::init()
{
    if ( !Base_Layer::init() )
    {
        return false;
    }
    
    MSLPManager::getInstance()->finishProgress(true);
    
    PsdParser::parseToPsdJSON("common_end_page.json", &m_psdDictionary, true);
    
    initView();
    
    return true;
}


void End_Page::onEnter()
{
    Base_Layer::onEnter();
    
    onViewLoad();
}

void End_Page::onExit()
{
    Base_Layer::onExit();
    
}

void End_Page::onViewLoad()
{
    Base_Layer::onViewLoad();
}

void End_Page::onViewLoaded()
{
    Base_Layer::onViewLoaded();
}


#pragma mark - touch

bool End_Page::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false) {
        return false;
    }
    
    return false;
}

void End_Page::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}

void End_Page::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}


void End_Page::onNarrationFinishedCallback(std::string fileName)
{
    log("Narration Finished fileName : %s", fileName.c_str());

}


#pragma mark gaf delegate function
void End_Page::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    if(sequenceName.compare("Animation") == 0 )
    {   
        m_gafObject->playSequence("AnimationStar", false);
        
        
        std::string file = StringUtils::format("common_end_page_snd_%02d.mp3", m_nTitleNum);
        MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", file));
        
    }
    else if(sequenceName.compare("AnimationStar") == 0 )
    {
        m_gafObject->setSequenceDelegate(nullptr);
        m_gafObject->setSoundPlayDelegate(nullptr);
        
        m_gafObject->pauseAllAnimation();
    }
}

void End_Page::onFramePlayed(GAFObject *object, uint32_t frame)
{

}

void End_Page::onTexturePreLoad(std::string& path)
{
    
}


#pragma mark step1_type1

void End_Page::initView()
{
    m_container = Sprite::create();
//    m_container->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
    //    m_container->setColor(Color3B(255, 247, 233));
    //    m_container->setAnchorPoint(Vec2::ZERO);
//    m_container->setPosition(winSize.width/2, winSize.height/2);
    this->addChild(m_container, kTagContainer, kTagContainer);
    
    Sprite* bg = Sprite::create(getCommonFilePath("img", "common_end_bg.jpg"));
    bg->setPosition(Vec2(winSize.width/2, winSize.height/2));
    bg->setScale(1.5f);
    m_container->addChild(bg, kTagBg, kTagBg);
    
    
    m_gafObject = createCommonGAFObject(m_container, kTagGafObject, getCommonFilePath("flash", "common_end_page.gaf"), true, Vec2(0, 1200.0f));
    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(End_Page::onFinishSequence, this));
    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
    m_gafObject->playSequence("Animation", true);
    
//    m_nTitleNum = random(1, 4);
    
    m_nTitleNum  = MGTUtils::randomInteger(1, 4);
    
    log("END NUM:%d", m_nTitleNum);
    
    std::string file = StringUtils::format("common_end_title_%02d.png", m_nTitleNum);
    Vec2 pos = PsdParser::getPsdPosition("common_end_title", &m_psdDictionary);
    
    Sprite* text = Sprite::create(getCommonFilePath("img", file));
    text->setPosition(pos);
    m_container->addChild(text, kTagText, kTagText);
    
    
//    file = StringUtils::format("common_end_page_snd_%02d.mp3", rand);
//    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", file));
    
    
    auto replayItem = MenuItemImage::create(getCommonFilePath("img", "common_end_btn_tryagain_n.png"),
                                            getCommonFilePath("img", "common_end_btn_tryagain_p.png"),
                                            getCommonFilePath("img", "common_end_btn_tryagain_p.png"),
                                            CC_CALLBACK_1(End_Page::menuCallback, this));
    
    auto nextItem = MenuItemImage::create(getCommonFilePath("img", "common_end_btn_next_n.png"),
                                          getCommonFilePath("img", "common_end_btn_next_p.png"),
                                          getCommonFilePath("img", "common_end_btn_next_p.png"),
                                          CC_CALLBACK_1(End_Page::menuCallback, this));
    
    auto exitItem = MenuItemImage::create(getCommonFilePath("img", "common_end_btn_quit_n.png"),
                                          getCommonFilePath("img", "common_end_btn_quit_p.png"),
                                          getCommonFilePath("img", "common_end_btn_quit_p.png"),
                                          CC_CALLBACK_1(End_Page::menuCallback, this));
    
    
    replayItem->setTag(kTagMenuItem_Replay);
    replayItem->setPosition(PsdParser::getPsdPosition("common_end_btn_tryagain_n", &m_psdDictionary));
    
    nextItem->setTag(kTagMenuItem_Next);
    nextItem->setPosition(PsdParser::getPsdPosition("common_end_btn_next_n", &m_psdDictionary));
    
    exitItem->setTag(kTagMenuItem_Exit);
    exitItem->setPosition(PsdParser::getPsdPosition("common_end_btn_quit_n", &m_psdDictionary));
    
    if (MSLPManager::getInstance()->getHasNext() == false)
    {
        nextItem->setVisible(false);
        replayItem->setPositionX(replayItem->getPositionX()+110);
        exitItem->setPositionX(exitItem->getPositionX()-142);
    }
    
    Menu* menu = Menu::create(replayItem, nextItem, exitItem, nullptr);
    menu->setPosition(Vec2::ZERO);
    m_container->addChild(menu, kTagMenu, kTagMenu);
}

void End_Page::menuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    
    Menu* menu = (Menu*)m_container->getChildByTag(kTagMenu);
    menu->setEnabled(false);
    
    MGTSoundManager::getInstance()->playEffect(ProductManager::getInstance()->getCommonFilePath("snd", "common_sfx_btn_01.mp3"));
    
    if (m_gafObject)
    {
        m_gafObject->stop();
        m_gafObject->stopAllActions();
    }
    
    if (item->getTag() == kTagMenuItem_Replay)
    {
        this->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(End_Page::gotoReplay, this)),
                                         nullptr));
        
    }
    else if (item->getTag() == kTagMenuItem_Next)
    {
        this->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(End_Page::gotoNext, this)),
                                         nullptr));
        
    }
    else if (item->getTag() == kTagMenuItem_Exit)
    {
        this->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(End_Page::gotoExit, this)),
                                         nullptr));
        
    }
}

void End_Page::gotoReplay()
{
    ProductManager::getInstance()->replay();
}

void End_Page::gotoNext()
{
    ProductManager::getInstance()->next();
}

void End_Page::gotoExit()
{
    ProductManager::getInstance()->exit();
}
