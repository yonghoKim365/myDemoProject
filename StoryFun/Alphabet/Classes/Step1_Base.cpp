#include "Step1_Base.h"

enum
{
    kTagEnding_Animation,
    kTagEnding_Cover,
    kTagEnding_Sentence,
    kTagEnding_Area
};

Step1_Base::Step1_Base():
m_navigation(nullptr),
m_dimmedLayer(nullptr),
m_commonPopup(nullptr),
m_commonGuide(nullptr),
m_title(nullptr),
m_gafGuide(nullptr),
m_playContainer(nullptr),
m_endContainer(nullptr),
m_eState(step1::STATE_TITLE),
m_orientation(0),
m_sleepBeganTime(0.0f)
{

}

Step1_Base::~Step1_Base()
{
}

// on "init" you need to initialize your instance
bool Step1_Base::init()
{
    if ( !Base_Layer::init() )
    {
        return false;
    }
    
    log("STEP1 BASE");
    
    ProductManager::getInstance()->setCurrentStep(1);
    
    int stepNum = ProductManager::getInstance()->getCurrentStep();
    int typeNum = ProductManager::getInstance()->getCurrentType();
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    m_strBgmSound =  StringUtils::format("common_b01_c01_s%d_bgm.mp3", stepNum);
    m_strTitleSound = StringUtils::format("common_b01_c01_s%d_snd_title.mp3", stepNum);
    m_strGuideSound = StringUtils::format("common_b01_c01_s%d_t%d_snd_guide.mp3", stepNum, typeNum);
    m_strWordSound = StringUtils::format("b01_c01_snd_word%02d.mp3", alphabetNum);
    m_strPhonicsSound = StringUtils::format("common_b01_c01_snd_phonics%02d.mp3", alphabetNum);
    m_strSentenceSound = StringUtils::format("b01_c01_snd_sentence%02d.mp3", alphabetNum);

    PsdParser::parseToPsdJSON("common_b01_c01_s1.json", &m_psdDictionary, true);

    
    createNavigation(this, navi::eNavigationColor_None, navi::eNavigationType_Step1);
    m_navigation->showInitBtn();
    
    
    m_playContainer = Sprite::create();
    //    m_playContainer->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
    //    m_playContainer->setColor(Color3B(255, 247, 233));
    m_playContainer->setAnchorPoint(Vec2::ZERO);
    m_playContainer->setPosition(Vec2::ZERO);
    this->addChild(m_playContainer, kTagPlayContainer, kTagPlayContainer);
    
    m_endContainer = Sprite::create();
//    m_endContainer->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
//    m_endContainer->setColor(Color3B::WHITE);
    m_endContainer->setAnchorPoint(Vec2::ZERO);
    m_endContainer->setPosition(Vec2::ZERO);
    this->addChild(m_endContainer, kTagEndContainer, kTagEndContainer);
    
    std::string file = StringUtils::format("b01_c01_s1_ani%02d.gaf", ProductManager::getInstance()->getCurrentAlphabet());
    
    
    log("base 1");
    m_gafObject = createGAFObject(m_endContainer, kTagEnding_Animation, getFilePath(ResourceType::GAF, "flash", file), true, Vec2(0, 1200.0f));
    log("base 2");
    m_gafObject->stop();
    m_gafObject->setVisible(false);
    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step1_Base::onFinishSequence, this));
    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
    log("base 3");
    
    
    
    
    // guide animation
    file = StringUtils::format("b01_c01_s%d_t%d_guide.gaf", ProductManager::getInstance()->getCurrentStep(), ProductManager::getInstance()->getCurrentType());
    GAFAsset* asset = GAFAsset::create(getCommonFilePath("flash", file), nullptr);
//    asset->setSoundDelegate(std::bind(&Base_Layer::onSoundEvent, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
    
    if (asset)
    {
        m_gafGuide = asset->createObjectAndRun(false);
        
        float scaleFactor = cocos2d::Director::getInstance()->getContentScaleFactor();
    
        m_gafGuide->setLocator(true);
        
        m_gafGuide->setPosition(Vec2(0, 1200.0f));
        this->addChild(m_gafGuide, kTagGuide, kTagGuide);
        
        m_gafGuide->stop();
        m_gafGuide->setVisible(false);
        m_gafGuide->setSequenceDelegate(CC_CALLBACK_2(Step1_Base::onFinishSequence, this));
    }
    

    log("base 4");
    
    createCommonGuide(common_guide::GUIDE, true);
    
    
    //    showTitle();
    guideStart();
    
    onTimer();
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_transition.mp3"));
    
    return true;
}


void Step1_Base::onEnter()
{
    Base_Layer::onEnter();
}

void Step1_Base::onExit()
{
    stopTimer();
    Base_Layer::onExit();
    
//    if (m_navigation)
//    {
//        m_navigation->setNavigationDelegate(nullptr);
//    }
//    
//    if (m_commonGuide)
//    {
//        m_commonGuide->setDelegate(nullptr);
//    }
//    
//    if (m_commonPopup)
//    {
//        m_commonPopup->setDelegate(nullptr);
//    }
    
}

void Step1_Base::onViewLoad()
{
    Base_Layer::onViewLoad();
    
    MGTSoundManager::getInstance()->playBgm(getCommonFilePath("snd", m_strBgmSound));
}

void Step1_Base::onViewLoaded()
{
    Base_Layer::onViewLoaded();
    
}


#pragma mark - touch

bool Step1_Base::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    m_orientation = DeviceUtilManager::getInstance()->getScreenOrientation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    if (m_eState  == step1::STATE_GUIDE)
    {
        if(MSLPManager::getInstance()->isFinished())
        {
            if (m_gafGuide->isVisible() == true)
            {
                m_touchEnabled = false;
                removeGuideAnimation();
                
                m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                                            CallFunc::create( CC_CALLBACK_0(Step1_Base::playStart, this)),
                                                            nullptr));
                
                return true;
            }
        }
    }
    else if (m_eState == step1::STATE_ENDING_COMPLETE)
    {
        auto sentence =  m_endContainer->getChildByTag(kTagEnding_Sentence);
        auto aniArea = m_endContainer->getChildByTag(kTagEnding_Area);
        if (MGTUtils::hitTestPoint(sentence, tp, false))
        {
            m_touchEnabled = false;
            
            m_gafObject->playSequence("Animation", true);
            m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step1_Base::onFinishSequence, this));
            m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
            
            return true;
        }
        else if (MGTUtils::hitTestPoint(aniArea, tp, false))
        {
            m_touchEnabled = false;
            
            m_gafObject->playSequence("Animation", true);
            m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step1_Base::onFinishSequence, this));
            m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
            
            return true;
        }
    }
    
    return false;
}

void Step1_Base::onTouchMoved(Touch *pTouch, Event *pEvent)
{
}

void Step1_Base::onTouchEnded(Touch *pTouch, Event *pEvent)
{
}

void Step1_Base::onTouchCancelled(Touch *touch, Event *event)
{
    log("TOUCH CANCELLED");
    onTouchEnded(touch, event);
}


void Step1_Base::onNarrationFinishedCallback(std::string fileName)
{
    if (fileName == m_strTitleSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::removeTitle, this)),
                                         nullptr));
    }
    else if(fileName == m_strGuideSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::playGuideAnimation, this)),
                                         nullptr));
    }
}


#pragma mark gaf delegate function
void Step1_Base::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    if(sequenceName.compare("Animation") == 0 )
    {
        log("gaf finish");
        
        if (m_eState == step1::STATE_ENDING)
        {
            m_eState = step1::STATE_ENDING_COMPLETE;
            m_touchEnabled = true;
            
            m_navigation->setVisibleButton(true, navi::eNavigationButton_Next);
            
            m_navigation->playButtonAction(navi::eNavigationButton_Next);
            
            Sprite* btn_bg = Sprite::create(getCommonFilePath("img", "common_btn_bg.png"));
            btn_bg->setPosition(Vec2(winSize.width-btn_bg->getContentSize().width/2 + 4, btn_bg->getContentSize().height/2 - 2));
            this->addChild(btn_bg, 20);
            
            m_gafObject->setSequenceDelegate(nullptr);
            m_gafObject->setSoundPlayDelegate(nullptr);
            
            m_gafObject->pauseAllAnimation();
        }
        else if(m_eState == step1::STATE_ENDING_COMPLETE)
        {
            m_touchEnabled = true;
            
            m_gafObject->setSequenceDelegate(nullptr);
            m_gafObject->setSoundPlayDelegate(nullptr);
            
            m_gafObject->pauseAllAnimation();
        }
    }
    else if(sequenceName.compare("guide_animation") == 0)
    {
        m_gafGuide->setSequenceDelegate(nullptr);
        m_gafGuide->pauseAllAnimation();
        m_gafGuide->setVisible(false);
        
        m_touchEnabled = false;
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::playStart, this)),
                                         nullptr));
    }
}

void Step1_Base::onFramePlayed(GAFObject *object, uint32_t frame)
{
    
}

void Step1_Base::onTexturePreLoad(std::string& path)
{
    CCLOG("Loading texture %s", path.c_str());
}


#pragma mark title show function

void Step1_Base::showTitle()
{
    m_eState = step1::STATE_TITLE;
    
    m_title = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_title.jpg"));
    m_title->setPosition(Vec2(winSize.width/2, winSize.height/2));
    this->addChild(m_title, kTagTitle, kTagTitle);
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step1_Base::playTitleSound, this)),
                                      nullptr));
}

void Step1_Base::removeTitle()
{
    m_title->runAction( Sequence::create(EaseSineOut::create(FadeTo::create(0.5f, 0.0f)),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::guideStart, this)),
                                         nullptr));
    
}

void Step1_Base::guideStart()
{
    m_eState  = step1::STATE_GUIDE;
    if (m_title)
    {
        m_title->removeFromParentAndCleanup(true);
        m_title = nullptr;
    }
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step1_Base::playGuideSound, this)),
                                      nullptr));
}

void Step1_Base::playGuideAnimation()
{
    m_gafGuide->start();
    m_gafGuide->playSequence("guide_animation", false);
    m_gafGuide->setVisible(true);
    
    m_touchEnabled = true;
}

void Step1_Base::removeGuideAnimation()
{
    if (m_gafGuide)
    {
        m_gafGuide->stop();
        m_gafGuide->setSequenceDelegate(nullptr);
        MGTUtils::removeAllchildren(m_gafGuide);
        m_gafGuide->removeFromParentAndCleanup(true);
        m_gafGuide = nullptr;
    }
}

void Step1_Base::createEnding()
{
    m_eState = step1::STATE_ENDING;

    
    Sprite* cover = Sprite::create(getCommonFilePath("img", "common_ani_cover.png"));
    cover->setPosition(Vec2(winSize.width/2, winSize.height/2));
    m_endContainer->addChild(cover, kTagEnding_Cover, kTagEnding_Cover);
    
    std::string file;
    file = StringUtils::format("b01_c01_s1_%02d_sentence.png", ProductManager::getInstance()->getCurrentAlphabet());
    Sprite* sentence = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    sentence->setPosition(PsdParser::getPsdPosition("b01_c01_s1_sentence", &m_psdDictionary));
    m_endContainer->addChild(sentence, kTagEnding_Sentence, kTagEnding_Sentence);
    
    
//    file = StringUtils::format("b01_c01_s1_ani%02d.gaf", ProductManager::getInstance()->getCurrentAlphabet());
//    m_gafObject = createGAFObject(m_endContainer, kTagEnding_Animation, getFilePath("flash", file), true, Vec2(0, 1200.0f));
//    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step1_Base::onFinishSequence, this));
//    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
    
//    MGTUtils::fadeOutAllchildren(m_playContainer, 0.3f);
    
    m_playContainer->setVisible(false);
    
//    auto action = Sequence::create(DelayTime::create(0.5f),
//                                   CallFunc::create( CC_CALLBACK_0(Step1_Base::playEndingAnimation, this)),
//                                   nullptr);
//    
//    m_playContainer->runAction(action);
    
    m_gafObject->setVisible(true);

    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step1_Base::playEndingAnimation, this)),
                                      nullptr));
}

void Step1_Base::playEndingAnimation()
{
    m_gafObject->start();
    m_gafObject->playSequence("Animation", true);
    
    Sprite* aniArea = Sprite::create();
    aniArea->setContentSize(Size(1856, 944) );
    aniArea->setPosition(Vec2(960, 506));
    m_endContainer->addChild(aniArea, kTagEnding_Area, kTagEnding_Area);
    
}

#pragma mark step1 onPause / onResume function
void Step1_Base::onPause()
{
    
    if (m_eState == step1::STATE_TITLE)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_title);
    }
    else if (m_eState == step1::STATE_GUIDE || m_eState == step1::STATE_PLAY)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_playContainer);
    }
    else if (m_eState == step1::STATE_ENDING)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_endContainer);
    }
    
    if (m_gafGuide)
    {
        m_gafGuide->pauseAllAnimation();
    }
    
    if(m_gafObject)
    {
        m_gafObject->pauseAllAnimation();
    }
    
    MGTSoundManager::getInstance()->pauseAllEffects();
    MGTSoundManager::getInstance()->pauseAllNarrations();
//    MGTSoundManager::getInstance()->pauseAllBgm();
    
    stopAffordanceTimer();
}

void Step1_Base::onResume()
{
    if (m_eState == step1::STATE_TITLE)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_title);
    }
    else if (m_eState == step1::STATE_GUIDE || m_eState == step1::STATE_PLAY)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_playContainer);
    }
    else if (m_eState == step1::STATE_ENDING)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_endContainer);
    }
    
    if (m_gafGuide)
    {
        m_gafGuide->resumeAllAnimation();
    }
    
    if(m_gafObject)
    {
        m_gafObject->resumeAllAnimation();
    }
    
    MGTSoundManager::getInstance()->resumeAllEffects();
    MGTSoundManager::getInstance()->resumeAllNarrations();
//    MGTSoundManager::getInstance()->resumeAllBgm();
    
    startAffordanceTimer();
}

#pragma mark step1 start / complete function

void Step1_Base::playStart()
{
    m_eState = step1::STATE_PLAY;

    m_touchEnabled = false;
    
    removeGuideAnimation();
    
    m_navigation->setVisibleButton(true, navi::eNavigationButton_Guide);
    m_navigation->setEnableButton(true, navi::eNavigationButton_Guide);
    
    
    
    MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", m_strWordSound));
}

void Step1_Base::playComplete()
{
    m_touchEnabled = false;
    m_eState = step1::STATE_PLAY_COMPLETE;
    
    m_navigation->setVisibleButton(false, navi::eNavigationButton_Guide);
}

#pragma mark navi function

void Step1_Base::createNavigation(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType type)
{
    m_navigation = new Navigation();
    m_navigation->setNavigationDelegate(std::bind(&Step1_Base::onTouchedNavigation, this, std::placeholders::_1));
    
    m_navigation->initWithNavigationType(pTargetNode, eNavigationColor, type);
    m_navigation->showInitBtn();
}

void Step1_Base::onTouchedNavigation(int buttonType)
{
    log("touch navi");
    navi::eNavigationButton btnType = (navi::eNavigationButton)buttonType;
    
    switch(btnType)
    {
        case navi::eNavigationButton_Next:                    onTouchedNavigationButtonAtNext();              break;
        case navi::eNavigationButton_Guide:                   onTouchedNavigationButtonAtGuide();              break;
        case navi::eNavigationButton_Exit:                    onTouchedNavigationButtonAtExit();              break;
        default:                                                                                              break;
    }
}


void Step1_Base::onTouchedNavigationButtonAtNext()
{
    if (m_eState != step1::STATE_EXIT)
    {
        if (m_gafGuide)
        {
            m_gafGuide->stop();
        }
        
        if (m_gafObject)
        {
            m_gafObject->stop();
        }
        
        m_eState = step1::STATE_EXIT;
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::gotoNextStep, this)),
                                         nullptr));
    }
}

void Step1_Base::onTouchedNavigationButtonAtGuide()
{
    onPause();
    
//    createCommonGuide(common_guide::GUIDE, true);
    showCommonGuide();
}

void Step1_Base::onTouchedNavigationButtonAtExit()
{
    log("exit");
    
//    onPause();
//    createCommonPopup(common_popup::EXIT, true);
    
    if (m_gafGuide)
    {
        m_gafGuide->stop();
    }
    
    if (m_gafObject)
    {
        m_gafObject->stop();
    }
    
    if (m_eState != step1::STATE_EXIT)
    {
        m_eState = step1::STATE_EXIT;
        
        MSLPManager::getInstance()->finishProgress(false);
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                         CallFunc::create( CC_CALLBACK_0(Step1_Base::gotoExit, this)),
                                         nullptr));
    }
}

#pragma mark dimmed layer function

void Step1_Base::createDimmedLayer()
{
    m_dimmedLayer = DimmedLayer::create(Color4B(0, 0, 0, 255*0.5f));
    m_dimmedLayer->setOpacity(0.0f);
    m_dimmedLayer->setVisible(false);
    this->addChild(m_dimmedLayer, kDepth_dimmed);
}

#pragma mark common popup function

void Step1_Base::createCommonPopup(common_popup::ePopupType type, bool hasDimmed)
{
    m_commonPopup = Common_Popup::create(type, hasDimmed);
    this->addChild(m_commonPopup, kDepth_popup);
    
    m_commonPopup->setDelegate(std::bind(&Step1_Base::onTouchedPopupButton, this, std::placeholders::_1));
}

void Step1_Base::onTouchedPopupButton(bool result)
{
    if (result == false)
    {
        onTouchedPopupButtonAtNo();
    }
    else
    {
        onTouchedPopupButtonAtYes();
    }
}

void Step1_Base::onTouchedPopupButtonAtNo()
{
    
    if (m_commonPopup)
    {
        MGTUtils::removeAllchildren(m_commonPopup);
        m_commonPopup->removeFromParentAndCleanup(true);
    }
    
    if (m_commonPopup->getType() == common_popup::DRAWING_DELETE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::EXIT)
    {
        
    }

    
    onResume();
}


void Step1_Base::onTouchedPopupButtonAtYes()
{
    if (m_commonPopup)
    {
        MGTUtils::removeAllchildren(m_commonPopup);
        m_commonPopup->removeFromParentAndCleanup(true);
    }
    
    if (m_commonPopup->getType() == common_popup::DRAWING_DELETE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::EXIT)
    {
        ProductManager::getInstance()->exit();
    }
}

#pragma mark common guide function

void Step1_Base::createCommonGuide(common_guide::eGuideType type, bool hasDimmed)
{
    m_commonGuide = Common_Guide::create(type, hasDimmed);
    m_commonGuide->hide();
    m_commonGuide->setVisible(false);
    
    this->addChild(m_commonGuide, kDepth_popup);
    
    m_commonGuide->setDelegate(std::bind(&Step1_Base::onTouchedGuideButton, this));
}

void Step1_Base::showCommonGuide()
{
    m_commonGuide->show();
    m_commonGuide->setVisible(true);
}

void Step1_Base::hideCommonGuide()
{
    m_commonGuide->hide();
    m_commonGuide->setVisible(false);
}

void Step1_Base::onTouchedGuideButton()
{
    onTouchedGuideButtonAtClose();
}

void Step1_Base::onTouchedGuideButtonAtClose()
{
//    if (m_commonGuide)
//    {
//        MGTUtils::removeAllchildren(m_commonGuide);
//        m_commonGuide->removeFromParentAndCleanup(true);
//    }
    
    hideCommonGuide();
    onResume();
}

#pragma mark affordance function

void Step1_Base::scheduleTimer(float dt)
{
    float time = getCurrentTime() - m_sleepBeganTime;
    
    if (time >= 10.0f)
    {
        showAffordance();
        
        stopAffordanceTimer();
    }
}

void Step1_Base::startAffordanceTimer()
{
    m_sleepBeganTime = getCurrentTime();
    schedule(CC_SCHEDULE_SELECTOR(Step1_Base::scheduleTimer));
}

void Step1_Base::stopAffordanceTimer()
{
    unschedule(CC_SCHEDULE_SELECTOR(Step1_Base::scheduleTimer));
}

void Step1_Base::playAffordance(Sprite* sp)
{
    int typeNum = ProductManager::getInstance()->getCurrentType();
    if ( typeNum == 1)
    {
        sp->setOpacity(0.0f);
        sp->setVisible(true);
        
        sp->setColor(Color3B(255, 126, 0));
        
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
//                                    DelayTime::create(0.3f),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
        
    }
    else if ( typeNum == 2)
    {
        sp->setOpacity(255.0f);
        sp->setVisible(true);
        
        sp->setColor(Color3B(226, 221, 213));
        
        auto seq = Sequence::create(EaseSineIn::create(TintTo::create(0.4f, Color3B(255, 126, 0))),
                                    EaseSineOut::create(TintTo::create(0.4f, Color3B(226, 221, 213))),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }
    else if ( typeNum == 3)
    {
        sp->setOpacity(0.0f);
        sp->setVisible(true);
        
        sp->setColor(Color3B(255, 126, 0));
        
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }
}

void Step1_Base::stopAffordance(Sprite* sp, bool isVisible)
{
    int typeNum = ProductManager::getInstance()->getCurrentType();
    
    sp->setVisible(isVisible);
    sp->stopAllActions();
    
    if (typeNum == 1 || typeNum == 3)
    {
        sp->setOpacity(0.0f);
    }
    else
    {
        sp->setOpacity(255.0f);
        sp->setColor(Color3B(226, 221, 213));
    }
}

void Step1_Base::showAffordance()
{
    
}

void Step1_Base::hideAffordance()
{
    
}

#pragma mark narration play function

void Step1_Base::playTitleSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strTitleSound));
}

void Step1_Base::playGuideSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strGuideSound));
}

void Step1_Base::playPhonicsSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strPhonicsSound));
}

void Step1_Base::playWordSound()
{
    MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", m_strWordSound));
}


void Step1_Base::gotoNextStep()
{
    ProductManager::getInstance()->nextStep();
}

void Step1_Base::gotoExit()
{
    ProductManager::getInstance()->exit();
}
