#include "Step2_Base.h"

enum
{
    kTagEnding_Animation,
    kTagEnding_Cover,
    kTagEnding_Sentence
};


Step2_Base::Step2_Base():
m_navigation(nullptr),
m_dimmedLayer(nullptr),
m_commonPopup(nullptr),
m_commonGuide(nullptr),
m_title(nullptr),
m_playContainer(nullptr),
m_endContainer(nullptr),
m_eState(step2::STATE_TITLE),
m_orientation(0),
m_sleepBeganTime(0.0f),
m_touchBeganTime(0.0f),
m_touch(nullptr),
m_isTouchMoved(false),
m_isTouchObject(false)
{

}

Step2_Base::~Step2_Base()
{
}


// on "init" you need to initialize your instance
bool Step2_Base::init()
{
    if ( !Base_Layer::init() )
    {
        return false;
    }
    
    int stepNum = ProductManager::getInstance()->getCurrentStep();
    int typeNum = ProductManager::getInstance()->getCurrentType();
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    m_strBgmSound =  StringUtils::format("common_b01_c01_s%d_bgm.mp3", stepNum);
    m_strTitleSound = StringUtils::format("common_b01_c01_s%d_snd_title.mp3", stepNum);
    m_strGuideSound = StringUtils::format("common_b01_c01_s%d_snd_guide.mp3", stepNum);
    
    m_strWordSound = StringUtils::format("b01_c01_snd_word%02d.mp3", alphabetNum);
    m_strPhonicsSound = StringUtils::format("common_b01_c01_snd_phonics%02d.mp3", alphabetNum);
    m_strSentenceSound = StringUtils::format("b01_c01_snd_sentence%02d.mp3", alphabetNum);

    
    m_playContainer = Sprite::create();
    m_playContainer->setAnchorPoint(Vec2::ZERO);
    m_playContainer->setPosition(Vec2::ZERO);
    this->addChild(m_playContainer, kTagPlayContainer, kTagPlayContainer);
    
    
    int examSet [26][4] = { {1, 3, 2, 4}, {2, 4, 3, 6}, {3, 7, 2, 4}, {4, 2, 3, 8},
                            {5, 6, 7, 8}, {6, 7, 10, 8}, {7, 6, 11, 8}, {8, 6, 7, 12},
                            {9, 10, 11, 12}, {10, 13, 11, 12}, {11, 12, 10, 14}, {12, 10, 11, 16},
                            {13, 16, 17, 14}, {14, 18, 16, 13}, {15, 13, 16, 14}, {16, 14, 19, 13},
                            {17, 18, 20, 19}, {18, 17, 20, 19}, {19, 17, 18, 20}, {20, 17, 18, 19},
                            {21, 22, 24, 23}, {22, 21, 24, 23}, {23, 21, 24, 22}, {24, 21, 22, 23},
                            {25, 24, 23, 26}, {26, 23, 24, 25} };
    
    for (int i = 0; i<26; i++)
    {
        std::vector<int>* set = new std::vector<int>();
        
        for (int j = 0; j<4; j++)
        {
            set->push_back(examSet[i][j]);
        }
        
        m_exampleSets.push_back(set);
    }

    
    createNavigation(this, navi::eNavigationColor_None, navi::eNavigationType_Step2);
    m_navigation->showInitBtn();
    m_navigation->setEnableButton(false, navi::eNavigationButton_Guide);
    
    
    // guide animation
    std::string file = StringUtils::format("b01_c01_s%d_t%d_guide.gaf", ProductManager::getInstance()->getCurrentStep(), ProductManager::getInstance()->getCurrentType());
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
        m_gafGuide->setSequenceDelegate(CC_CALLBACK_2(Step2_Base::onFinishSequence, this));
    }
    
    
    createCommonGuide(common_guide::GUIDE, true);
    
    //    showTitle();
    guideStart();
    
    
    onTimer();
    
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_transition.mp3"));
    return true;
}


void Step2_Base::onEnter()
{
    Base_Layer::onEnter();
}

void Step2_Base::onExit()
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

void Step2_Base::onViewLoad()
{
    Base_Layer::onViewLoad();
    
    MGTSoundManager::getInstance()->playBgm(getCommonFilePath("snd", m_strBgmSound));
}

void Step2_Base::onViewLoaded()
{
    Base_Layer::onViewLoaded();
    
}

#pragma mark - touch

bool Step2_Base::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    m_orientation = DeviceUtilManager::getInstance()->getScreenOrientation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    if (m_eState  == step2::STATE_GUIDE)
    {
        if(MSLPManager::getInstance()->isFinished())
        {
            if (m_gafGuide->isVisible() == true)
            {
                m_touchEnabled = false;
                removeGuideAnimation();
                
                m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                                            CallFunc::create( CC_CALLBACK_0(Step2_Base::playStart, this)),
                                                            nullptr));
                
                return true;
            }
        }
        
    }
    
    return false;
}

void Step2_Base::onTouchMoved(Touch *pTouch, Event *pEvent)
{
}

void Step2_Base::onTouchEnded(Touch *pTouch, Event *pEvent)
{
}

void Step2_Base::onTouchCancelled(Touch *touch, Event *event)
{
    log("TOUCH CANCELLED");
    onTouchEnded(touch, event);
}


void Step2_Base::onNarrationFinishedCallback(std::string fileName)
{
    if (fileName == m_strTitleSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(Step2_Base::removeTitle, this)),
                                         nullptr));
    }
    else if(fileName == m_strGuideSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                         CallFunc::create( CC_CALLBACK_0(Step2_Base::playGuideAnimation, this)),
                                         nullptr));
    }
}

#pragma mark gaf delegate function
void Step2_Base::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    if(sequenceName.compare("Animation") == 0 )
    {
        log("gaf finish");

    }
    else if(sequenceName.compare("guide_animation") == 0)
    {
        m_gafGuide->setSequenceDelegate(nullptr);
        m_gafGuide->pauseAllAnimation();
        m_gafGuide->setVisible(false);
        
        m_touchEnabled = false;
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                         CallFunc::create( CC_CALLBACK_0(Step2_Base::playStart, this)),
                                         nullptr));
    }
}

void Step2_Base::onFramePlayed(GAFObject *object, uint32_t frame)
{
    
}

void Step2_Base::onTexturePreLoad(std::string& path)
{
    CCLOG("Loading texture %s", path.c_str());
}


#pragma mark title show function

void Step2_Base::showTitle()
{
    m_eState = step2::STATE_TITLE;
    
    std::string file = StringUtils::format("common_b01_c01_s%d_title.jpg", ProductManager::getInstance()->getCurrentStep());
    m_title = Sprite::create(getCommonFilePath("img", file));
    m_title->setPosition(Vec2(winSize.width/2, winSize.height/2));
    this->addChild(m_title, kTagTitle, kTagTitle);
    
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                                 CallFunc::create( CC_CALLBACK_0(Step2_Base::playTitleSound, this)),
                                                 nullptr));
}

void Step2_Base::removeTitle()
{
    m_title->runAction( Sequence::create(EaseSineOut::create(FadeTo::create(0.5f, 0.0f)),
                                         CallFunc::create( CC_CALLBACK_0(Step2_Base::guideStart, this)),
                                         nullptr));
    
}

void Step2_Base::guideStart()
{
    m_eState = step2::STATE_GUIDE;
    if (m_title)
    {
        m_title->removeFromParentAndCleanup(true);
        m_title = nullptr;
    }
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step2_Base::playGuideSound, this)),
                                      nullptr));
}

void Step2_Base::playGuideAnimation()
{
    m_gafGuide->start();
    m_gafGuide->playSequence("guide_animation", false);
    m_gafGuide->setVisible(true);
    
    m_touchEnabled = true;
}

void Step2_Base::removeGuideAnimation()
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

void Step2_Base::createEnding()
{
    m_eState = step2::STATE_END;
    
}


#pragma mark step2 onPause / onResume function
void Step2_Base::onPause()
{
    if (m_eState == step2::STATE_TITLE)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_title);
    }
    else if (m_eState == step2::STATE_GUIDE || m_eState == step2::STATE_PLAY)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_playContainer);
    }
    else if (m_eState == step2::STATE_END)
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

void Step2_Base::onResume()
{
    if (m_eState == step2::STATE_TITLE)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_title);
    }
    else if (m_eState == step2::STATE_GUIDE || m_eState == step2::STATE_PLAY)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_playContainer);
    }
    else if (m_eState == step2::STATE_END)
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

#pragma mark step2 start / complete function

void Step2_Base::playStart()
{
    m_eState = step2::STATE_PLAY;
    
    m_touchEnabled = false;
    
    removeGuideAnimation();
    
    m_navigation->setVisibleButton(true, navi::eNavigationButton_Guide);
    m_navigation->setEnableButton(true, navi::eNavigationButton_Guide);
    
    playWordSound();
}

void Step2_Base::playComplete()
{
    m_touchEnabled = false;
    m_eState = step2::STATE_PLAY_COMPLETE;
    
    m_navigation->setVisibleButton(false, navi::eNavigationButton_Guide);
}

void Step2_Base::nextStep()
{
    m_eState = step2::STATE_EXIT;
    
    if (m_gafGuide)
    {
        m_gafGuide->stop();
    }
    
    if (m_gafObject)
    {
        m_gafObject->pauseAllAnimation();
    }
    
    
    m_playContainer->runAction(Sequence::create(DelayTime::create(0.0f),
                                                CallFunc::create( CC_CALLBACK_0(Step2_Base::gotoNextStep, this)),
                                                nullptr));
    
}

#pragma mark navi function

void Step2_Base::createNavigation(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType type)
{
    m_navigation = new Navigation();
    m_navigation->setNavigationDelegate(std::bind(&Step2_Base::onTouchedNavigation, this, std::placeholders::_1));
    
    m_navigation->initWithNavigationType(pTargetNode, eNavigationColor, type);
    m_navigation->showInitBtn();
}

void Step2_Base::onTouchedNavigation(int buttonType)
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


void Step2_Base::onTouchedNavigationButtonAtNext()
{
    if (m_eState != step2::STATE_EXIT)
    {
        if (m_gafObject)
        {
            m_gafObject->stop();
        }
        
        m_eState = step2::STATE_EXIT;
//        ProductManager::getInstance()->exit();
    }
}

void Step2_Base::onTouchedNavigationButtonAtGuide()
{
    onPause();
    
//    createCommonGuide(common_guide::GUIDE, true);
    showCommonGuide();
}

void Step2_Base::onTouchedNavigationButtonAtExit()
{
    log("exit");
    
//    onPause();
//    createCommonPopup(common_popup::EXIT, true);
    
    if (m_eState != step2::STATE_EXIT)
    {
        if (m_gafGuide)
        {
            m_gafGuide->stop();
        }
        
        if (m_gafObject)
        {
            m_gafObject->stop();
        }
        
        m_eState = step2::STATE_EXIT;
        
        MSLPManager::getInstance()->finishProgress(false);
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                         CallFunc::create( CC_CALLBACK_0(Step2_Base::gotoExit, this)),
                                         nullptr));
    }
}

#pragma mark dimmed layer function

void Step2_Base::createDimmedLayer()
{
    m_dimmedLayer = DimmedLayer::create(Color4B(0, 0, 0, 255*0.5f));
    m_dimmedLayer->setOpacity(0.0f);
    m_dimmedLayer->setVisible(false);
    this->addChild(m_dimmedLayer, kDepth_dimmed);
}

#pragma mark common popup function

void Step2_Base::createCommonPopup(common_popup::ePopupType type, bool hasDimmed)
{
    m_commonPopup = Common_Popup::create(type, hasDimmed);
    this->addChild(m_commonPopup, kDepth_popup);
    
    m_commonPopup->setDelegate(std::bind(&Step2_Base::onTouchedPopupButton, this, std::placeholders::_1));
}

void Step2_Base::onTouchedPopupButton(bool result)
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

void Step2_Base::onTouchedPopupButtonAtNo()
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


void Step2_Base::onTouchedPopupButtonAtYes()
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

void Step2_Base::createCommonGuide(common_guide::eGuideType type, bool hasDimmed)
{
    m_commonGuide = Common_Guide::create(type, hasDimmed);
    this->addChild(m_commonGuide, kDepth_popup);
    
    m_commonGuide->setDelegate(std::bind(&Step2_Base::onTouchedGuideButton, this));
}

void Step2_Base::showCommonGuide()
{
    m_commonGuide->show();
    m_commonGuide->setVisible(true);
}

void Step2_Base::hideCommonGuide()
{
    m_commonGuide->hide();
    m_commonGuide->setVisible(false);
}

void Step2_Base::onTouchedGuideButton()
{
    onTouchedGuideButtonAtClose();
}

void Step2_Base::onTouchedGuideButtonAtClose()
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

void Step2_Base::scheduleTimer(float dt)
{
    float time = getCurrentTime() - m_sleepBeganTime;
    
    if (time >= 10.0f)
    {
        showAffordance();
        
        stopAffordanceTimer();
    }
}

void Step2_Base::startAffordanceTimer()
{
    m_sleepBeganTime = getCurrentTime();
    schedule(CC_SCHEDULE_SELECTOR(Step2_Base::scheduleTimer));
}

void Step2_Base::stopAffordanceTimer()
{
    unschedule(CC_SCHEDULE_SELECTOR(Step2_Base::scheduleTimer));
}

void Step2_Base::playAffordance(Sprite* sp)
{
    sp->setOpacity(0.0f);
    sp->setVisible(true);
    sp->setColor(Color3B(255, 126, 0));
    
    int typeNum = ProductManager::getInstance()->getCurrentType();
    if ( typeNum == 1)
    {
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }
    else if ( typeNum == 2)
    {
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }
}

void Step2_Base::stopAffordance(Sprite* sp, bool isVisible)
{
    sp->setVisible(isVisible);
    sp->setOpacity(0.0f);
    sp->stopAllActions();
}

void Step2_Base::showAffordance()
{
    
}

void Step2_Base::hideAffordance()
{
    
}


#pragma mark narration play function

void Step2_Base::playTitleSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strTitleSound));
}

void Step2_Base::playGuideSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strGuideSound));
}

void Step2_Base::playWordSound()
{
    MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", m_strWordSound));
}


void Step2_Base::playPhonicsSound(int num)
{
    std::string file = StringUtils::format("common_b01_c01_snd_phonics%02d.mp3", num);
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", file));
}


void Step2_Base::gotoNextStep()
{
    ProductManager::getInstance()->nextStep();
}

void Step2_Base::gotoExit()
{
    ProductManager::getInstance()->exit();
}
