#include "Step3_Base.h"
#include "End_Page.h"


Step3_Base::Step3_Base():
m_navigation(nullptr),
m_dimmedLayer(nullptr),
m_commonPopup(nullptr),
m_commonGuide(nullptr),
m_title(nullptr),
m_playContainer(nullptr),
m_endContainer(nullptr),
m_eState(step3::STATE_TITLE),
m_orientation(0),
m_sleepBeganTime(0.0f),
m_touchBeganTime(0.0f)
{

}

Step3_Base::~Step3_Base()
{
}

// on "init" you need to initialize your instance
bool Step3_Base::init()
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
    m_strAlphabetSound = StringUtils::format("b01_c01_snd_alphabet%02d.mp3", alphabetNum);
    m_strSentenceSound = StringUtils::format("b01_c01_snd_sentence%02d.mp3", alphabetNum);

    m_playContainer = Sprite::create();
    m_playContainer->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
    m_playContainer->setColor(Color3B(255, 255, 255));
    //    m_playContainer->setAnchorPoint(Vec2::ZERO);
    m_playContainer->setPosition(winSize.width/2, winSize.height/2);
    this->addChild(m_playContainer, kTagPlayContainer, kTagPlayContainer);
    
    
    
    showTitle();
    
    createNavigation(this, navi::eNavigationColor_None, navi::eNavigationType_Step3);
    m_navigation->showInitBtn();
    m_navigation->setEnableButton(false, navi::eNavigationButton_Guide);
    
    
    createCommonGuide(common_guide::GUIDE, true);
    createCommonPopup(true);
    
    
    
    // guide animation
    //    file = StringUtils::format("b01_c02_guide.gaf", ProductManager::getInstance()->getCurrentStep(), ProductManager::getInstance()->getCurrentType());
    GAFAsset* asset = GAFAsset::create(getCommonFilePath("flash", "common_guide_touch.gaf"), nullptr);
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
        m_gafGuide->setSequenceDelegate(CC_CALLBACK_2(Step3_Base::onFinishSequence, this));
    }
    
    
    
    onTimer();
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_transition.mp3"));
    return true;
}


void Step3_Base::onEnter()
{
    Base_Layer::onEnter();
}

void Step3_Base::onExit()
{
    stopTimer();
    Base_Layer::onExit();
    
}

void Step3_Base::onViewLoad()
{
    Base_Layer::onViewLoad();
    
    MGTSoundManager::getInstance()->playBgm(getCommonFilePath("snd", m_strBgmSound));
}

void Step3_Base::onViewLoaded()
{
    Base_Layer::onViewLoaded();
    
}


#pragma mark - touch

bool Step3_Base::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    m_orientation = DeviceUtilManager::getInstance()->getScreenOrientation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    if (m_eState  == step3::STATE_GUIDE)
    {
        if(MSLPManager::getInstance()->isFinished())
        {
            if (m_gafGuide->isVisible() == true)
            {
                m_touchEnabled = false;
                removeGuideAnimation();
                
                playStart();
                
                //            this->runAction(Sequence::create(DelayTime::create(0.5f),
                //                                             CallFunc::create( CC_CALLBACK_0(Step3_Base::playStart, this)),
                //                                             nullptr));
                
                return true;
            }
        }
    }
    
    return false;
}

void Step3_Base::onTouchMoved(Touch *pTouch, Event *pEvent)
{
}

void Step3_Base::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    LayerColor::onTouchEnded(pTouch, pEvent);
}

void Step3_Base::onTouchCancelled(Touch *touch, Event *event)
{
    LayerColor::onTouchCancelled(touch, event);
    log("TOUCH CANCELLED");
    onTouchEnded(touch, event);
}

void Step3_Base::onNarrationFinishedCallback(std::string fileName)
{
    if (fileName == m_strTitleSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(Step3_Base::removeTitle, this)),
                                         nullptr));
    }
    else if(fileName == m_strPhonicsSound)
    {
        
    }
}

#pragma mark gaf delegate function
void Step3_Base::onFinishSequence( GAFObject * object, const std::string& sequenceName )
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
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                         CallFunc::create( CC_CALLBACK_0(Step3_Base::playStart, this)),
                                         nullptr));
    }
}

void Step3_Base::onFramePlayed(GAFObject *object, uint32_t frame)
{
    
}

void Step3_Base::onTexturePreLoad(std::string& path)
{
    CCLOG("Loading texture %s", path.c_str());
}

#pragma mark title show function

void Step3_Base::showTitle()
{
    m_eState = step3::STATE_TITLE;
    
    std::string file = StringUtils::format("common_b01_c01_s%d_title.jpg", ProductManager::getInstance()->getCurrentStep());
    m_title = Sprite::create(getCommonFilePath("img", file));
    m_title->setPosition(Vec2(winSize.width/2, winSize.height/2));
    this->addChild(m_title, kTagTitle, kTagTitle);
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step3_Base::playTitleSound, this)),
                                      nullptr));
}

void Step3_Base::removeTitle()
{
    m_title->runAction( Sequence::create(EaseSineOut::create(FadeTo::create(0.5f, 0.0f)),
                                      CallFunc::create( CC_CALLBACK_0(Step3_Base::guideStart, this)),
                                      nullptr));
    
}

void Step3_Base::guideStart()
{
    m_eState = step3::STATE_GUIDE;
    m_title->removeFromParentAndCleanup(true);
    m_title = nullptr;
    
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step3_Base::playPhonicsSound, this)),
                                      nullptr));
}

void Step3_Base::playGuideAnimation()
{
    m_gafGuide->start();
    m_gafGuide->playSequence("guide_animation", false);
    m_gafGuide->setVisible(true);
    
    m_touchEnabled = true;
}

void Step3_Base::removeGuideAnimation()
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

void Step3_Base::createEnding()
{
    m_eState = step3::STATE_END;
    

}

#pragma mark step1 onPause / onResume function
void Step3_Base::onPause()
{
    MGTSoundManager::getInstance()->pauseAllEffects();
    MGTSoundManager::getInstance()->pauseAllNarrations();
//    MGTSoundManager::getInstance()->pauseAllBgm();
    
    if (m_eState == step3::STATE_TITLE)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_title);
    }
    else if (m_eState == step3::STATE_GUIDE || m_eState == step3::STATE_PLAY1 || m_eState == step3::STATE_PLAY2 || m_eState == step3::STATE_PLAY3)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_playContainer);
        
        if(m_gafObject && m_eState == step3::STATE_PLAY1)
        {
            m_gafObject->pauseAllAnimation();
        }
    }
    else if (m_eState == step3::STATE_END)
    {
        MGTUtils::getInstance()->pauseAllAnimations(m_endContainer);
    }
    
    stopAffordanceTimer();
}

void Step3_Base::onResume()
{
    MGTSoundManager::getInstance()->resumeAllEffects();
    MGTSoundManager::getInstance()->resumeAllNarrations();
//    MGTSoundManager::getInstance()->resumeAllBgm();
    
    if (m_eState == step3::STATE_TITLE)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_title);
    }
    else if (m_eState == step3::STATE_GUIDE || m_eState == step3::STATE_PLAY1 || m_eState == step3::STATE_PLAY2 || m_eState == step3::STATE_PLAY3)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_playContainer);
        
        if(m_gafObject && m_eState == step3::STATE_PLAY1)
        {
            m_gafObject->resumeAllAnimation();
        }
    }
    else if (m_eState == step3::STATE_END)
    {
        MGTUtils::getInstance()->resumeAllAnimations(m_endContainer);
    }
    
    startAffordanceTimer();
}

#pragma mark step3 complete function
void Step3_Base::playStart()
{
    m_eState = step3::STATE_PLAY1;
    
    m_touchEnabled = false;
    
    removeGuideAnimation();
}

void Step3_Base::playComplete()
{
    m_touchEnabled = false;

    m_navigation->setVisibleButton(false, navi::eNavigationButton_Guide);
    
    hideCommonPopup();
    onResume();
    
    createEnding();
}




#pragma mark navi function

void Step3_Base::createNavigation(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType type)
{
    m_navigation = new Navigation();
    m_navigation->setNavigationDelegate(std::bind(&Step3_Base::onTouchedNavigation, this, std::placeholders::_1));
    
    m_navigation->initWithNavigationType(pTargetNode, eNavigationColor, type);
    m_navigation->showInitBtn();
}

void Step3_Base::onTouchedNavigation(int buttonType)
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

void Step3_Base::onTouchedNavigationButtonAtNext()
{
    if (m_eState != step3::STATE_EXIT)
    {
        m_eState = step3::STATE_EXIT;
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                         CallFunc::create( CC_CALLBACK_0(Step3_Base::gotoEndPage, this)),
                                         nullptr));
    }
}

void Step3_Base::onTouchedNavigationButtonAtGuide()
{
    onPause();
    
//    createCommonGuide(common_guide::GUIDE, true);
    showCommonGuide();
}

void Step3_Base::onTouchedNavigationButtonAtExit()
{
    log("exit");
    
    
    if (m_eState == step3::STATE_EXIT)
    {
        return;
    }
    else if (m_eState == step3::STATE_TITLE)
    {
        if (m_gafGuide)
        {
            m_gafGuide->stop();
        }
        
        if (m_gafObject)
        {
            m_gafObject->stop();
        }
        
        m_eState = step3::STATE_EXIT;
        
        MSLPManager::getInstance()->finishProgress(false);
        
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                                    CallFunc::create( CC_CALLBACK_0(Step3_Base::gotoExit, this)),
                                                    nullptr));        
    }
    else
    {
        onPause();
        
        //    createCommonPopup(common_popup::EXIT, true);
        showCommonPopup(common_popup::EXIT);
    }
}


#pragma mark dimmed layer function

void Step3_Base::createDimmedLayer()
{
    m_dimmedLayer = DimmedLayer::create(Color4B(0, 0, 0, 255*0.5f));
    m_dimmedLayer->setOpacity(0.0f);
    m_dimmedLayer->setVisible(false);
    this->addChild(m_dimmedLayer, kDepth_dimmed);
}

#pragma mark common popup function

void Step3_Base::createCommonPopup(bool hasDimmed)
{
    m_commonPopup = Common_Popup::create(common_popup::ALL, hasDimmed);
    this->addChild(m_commonPopup, kDepth_popup);
    
    m_commonPopup->setDelegate(std::bind(&Step3_Base::onTouchedPopupButton, this, std::placeholders::_1));
}


void Step3_Base::showCommonPopup(common_popup::ePopupType type)
{
    m_commonPopup->show(type);
    m_commonPopup->setVisible(true);
}

void Step3_Base::hideCommonPopup()
{
    m_commonPopup->hide();
    m_commonPopup->setVisible(false);
}


void Step3_Base::onTouchedPopupButton(bool result)
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

void Step3_Base::onTouchedPopupButtonAtNo()
{
//    if (m_commonPopup)
//    {
//        MGTUtils::removeAllchildren(m_commonPopup);
//        m_commonPopup->removeFromParentAndCleanup(true);
//    }

    hideCommonPopup();
    
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


void Step3_Base::onTouchedPopupButtonAtYes()
{
//    if (m_commonPopup)
//    {
//        MGTUtils::removeAllchildren(m_commonPopup);
//        m_commonPopup->removeFromParentAndCleanup(true);
//    }
    
    
    if (m_commonPopup->getType() == common_popup::DRAWING_DELETE)
    {
        hideCommonPopup();
        onResume();
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVE)
    {
        
        log("-----------------YES   DRAWING_SAVE");
        showCommonPopup(common_popup::DRAWING_SAVING);
        
//        hideCommonPopup();
//        onResume();
    }
    else if (m_commonPopup->getType() == common_popup::EXIT)
    {
        if (m_eState != step3::STATE_EXIT)
        {
            if (m_gafGuide)
            {
                m_gafGuide->stop();
            }
            
            if (m_gafObject)
            {
                m_gafObject->stop();
            }
            
            m_eState = step3::STATE_EXIT;
            
            MSLPManager::getInstance()->finishProgress(false);
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.3f),
                                                        CallFunc::create( CC_CALLBACK_0(Step3_Base::gotoExit, this)),
                                                        nullptr));
        }
    }
}

#pragma mark common guide function

void Step3_Base::createCommonGuide(common_guide::eGuideType type, bool hasDimmed)
{
    m_commonGuide = Common_Guide::create(type, hasDimmed);
    this->addChild(m_commonGuide, kDepth_popup);
    
    m_commonGuide->setDelegate(std::bind(&Step3_Base::onTouchedGuideButton, this));
}

void Step3_Base::showCommonGuide()
{
    m_commonGuide->show();
    m_commonGuide->setVisible(true);
}

void Step3_Base::hideCommonGuide()
{
    m_commonGuide->hide();
    m_commonGuide->setVisible(false);
}

void Step3_Base::onTouchedGuideButton()
{
    onTouchedGuideButtonAtClose();
}

void Step3_Base::onTouchedGuideButtonAtClose()
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

void Step3_Base::scheduleTimer(float dt)
{
    float time = getCurrentTime() - m_sleepBeganTime;
    
    if (time >= 10.0f)
    {
        showAffordance();
        
        stopAffordanceTimer();
    }
}

void Step3_Base::startAffordanceTimer()
{
    m_sleepBeganTime = getCurrentTime();
    schedule(CC_SCHEDULE_SELECTOR(Step3_Base::scheduleTimer));
}

void Step3_Base::stopAffordanceTimer()
{
    unschedule(CC_SCHEDULE_SELECTOR(Step3_Base::scheduleTimer));
}

void Step3_Base::playAffordance(Sprite* sp)
{
    if(m_eState == step3::STATE_PLAY1)
    {
        sp->setOpacity(0.0f);
        sp->setColor(Color3B( 255, 126, 0));
        
        
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }
    else if(m_eState == step3::STATE_PLAY2)
    {
        sp->setOpacity(0.0f);
        sp->setColor(Color3B( 127, 126, 124));
        
        auto seq = Sequence::create(EaseSineIn::create(FadeTo::create(0.4f, 255.0f)),
                                    EaseSineOut::create(FadeTo::create(0.4f, 0.0f)),
                                    nullptr);
        
        auto repeat = RepeatForever::create(seq);
        sp->runAction(repeat);
    }

    sp->setVisible(true);
}

void Step3_Base::stopAffordance(Sprite* sp, bool isVisible)
{
    sp->setVisible(isVisible);
    sp->setOpacity(0.0f);
    sp->stopAllActions();
}

void Step3_Base::showAffordance()
{
    
}

void Step3_Base::hideAffordance()
{
    
}


#pragma mark narration play function

void Step3_Base::playTitleSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strTitleSound));
}

void Step3_Base::playGuideSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strGuideSound));
}

void Step3_Base::playPhonicsSound()
{
    MGTSoundManager::getInstance()->playNarration(getCommonFilePath("snd", m_strPhonicsSound));
}

void Step3_Base::playWordSound()
{
    MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", m_strWordSound));
}

void Step3_Base::playAlphabetSound()
{
    MGTSoundManager::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", m_strAlphabetSound));
}


void Step3_Base::gotoEndPage()
{
    replaceSceneNoTransition(End_Page);
}

void Step3_Base::gotoExit()
{
    ProductManager::getInstance()->exit();
}
