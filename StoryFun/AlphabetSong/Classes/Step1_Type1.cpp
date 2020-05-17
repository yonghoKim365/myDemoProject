#include "Step1_Type1.h"

enum
{
    kTagBg = 0,
    kTagWord,
    kTagAlphabet,
    kTagObject,
    kTagLine,
    
    
};


Step1_Type1::Step1_Type1():
m_count(0)
{

}

Step1_Type1::~Step1_Type1()
{
}

Scene* Step1_Type1::createScene()
{
    auto scene = Scene::create();

    auto layer = Step1_Type1::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step1_Type1::init()
{
    if ( !Step1_Base::init() )
    {
        return false;
    }
    
    std::string file = StringUtils::format("b01_c01_s1_t1_%02d.json", ProductManager::getInstance()->getCurrentAlphabet());
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    
    initView();
    
    return true;
}


void Step1_Type1::onEnter()
{
    Step1_Base::onEnter();
    
    onViewLoad();
}

void Step1_Type1::onExit()
{
    Step1_Base::onExit();
    
}

void Step1_Type1::onViewLoad()
{
    Step1_Base::onViewLoad();
}

void Step1_Type1::onViewLoaded()
{
    Step1_Base::onViewLoaded();
}


#pragma mark - touch

bool Step1_Type1::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    if(Step1_Base::onTouchBegan(pTouch, pEvent))
    {
        return false;
    }
    
    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    if (m_eState == step1::STATE_PLAY)
    {
        stopAffordanceTimer();

        std::string file = StringUtils::format("b01_c01_s1_t1_%02d_object_on.png", ProductManager::getInstance()->getCurrentAlphabet());
        std::string imgPath = getFilePath(ResourceType::IMAGE, "img", file);
        
        log("TOUCH BEGAN img path:%s", imgPath.c_str());
        
        Image* image = MGTResourceUtils::getInstance()->getImageData(imgPath);
        
        
        if (MGTUtils::hitTestPointExact(m_object, image, tp, false))
        {
            m_isTouchObject = true;
            touchObject();
            
            hideAffordance();
            
            return true;
        }

        
        if (MGTUtils::hitTestPoint(m_word, tp, false))
        {
            playWordSound();
            hideAffordance();
        }
        
        
        startAffordanceTimer();
    }

    return false;
}

void Step1_Type1::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchMoved(pTouch, pEvent);
    
    Vec2 tp = pTouch->getLocation();
    
}

void Step1_Type1::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchEnded(pTouch, pEvent);
    Vec2 tp = pTouch->getLocation();
    
    if (!m_isTouchObject)
    {
        
    }
    
    m_isTouchObject = false;
}


void Step1_Type1::onNarrationFinishedCallback(std::string fileName)
{
    Step1_Base::onNarrationFinishedCallback(fileName);
    
    log("Narration Finished fileName : %s", fileName.c_str());
    
    if (fileName == m_strWordSound)
    {
        if(m_eState == step1::STATE_PLAY)
        {
            if (m_count == 0)
            {
                m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                                            CallFunc::create( CC_CALLBACK_0(Step1_Type1::interactionStart, this)),
                                                            nullptr));
            }
        }
        else if(m_eState == step1::STATE_PLAY_COMPLETE)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                             CallFunc::create( CC_CALLBACK_0(Step1_Base::createEnding, this)),
                                             nullptr));
        }
        
    }
    else if(fileName == m_strPhonicsSound)
    {
        if (m_count == 1 || m_count == 2 || m_count == 3)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                             CallFunc::create( CC_CALLBACK_0(Step1_Type1::nextInteraction, this)),
                                             nullptr));
        }
    }
}


#pragma mark gaf delegate function
void Step1_Type1::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step1_Base::onFinishSequence(object, sequenceName);
}

void Step1_Type1::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step1_Base::onFramePlayed(object, frame);
}

void Step1_Type1::onTexturePreLoad(std::string& path)
{
    Step1_Base::onTexturePreLoad(path);
}


#pragma mark navi touch override function
void Step1_Type1::onTouchedNavigationButtonAtExit()
{
    Step1_Base::onTouchedNavigationButtonAtExit();
    
}

void Step1_Type1::onTouchedNavigationButtonAtNext()
{
    Step1_Base::onTouchedNavigationButtonAtNext();
}


void Step1_Type1::onTouchedPopupButtonAtNo()
{
    Step1_Base::onTouchedPopupButtonAtNo();
}

void Step1_Type1::onTouchedPopupButtonAtYes()
{
    Step1_Base::onTouchedPopupButtonAtYes();
}


#pragma mark step1_type1

void Step1_Type1::initView()
{
    Sprite* bg = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t1_bg.jpg"));
    bg->setPosition(winSize.width/2, winSize.height/2);
    m_playContainer->addChild(bg, kTagBg, kTagBg);
    
    std::string file;
    Vec2 pos;
    file = StringUtils::format("b01_c01_s1_t1_%02d_word", ProductManager::getInstance()->getCurrentAlphabet());
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_off.png")));
    m_word->setPosition(pos);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    file = StringUtils::format("b01_c01_s1_t1_%02d_object", ProductManager::getInstance()->getCurrentAlphabet());
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_object = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_off.png")));
    m_object->setPosition(pos);
    m_playContainer->addChild(m_object, kTagObject, kTagObject);
    
    file = StringUtils::format("b01_c01_s1_t1_%02d_line", ProductManager::getInstance()->getCurrentAlphabet());
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_line = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    m_line->setPosition(pos);
    m_line->setColor(Color3B(255, 126, 0));
    m_line->setVisible(false);
    m_playContainer->addChild(m_line, kTagLine, kTagLine);
    
}

void Step1_Type1::touchObject()
{
    m_touchEnabled = false;

    objectAnimation();
    wordAnimation();
}

void Step1_Type1::objectAnimation()
{
    if (m_count == 1)
    {
        m_object->removeFromParent();
        
        std::string file = StringUtils::format("b01_c01_s1_t1_%02d_object", ProductManager::getInstance()->getCurrentAlphabet());
        Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
        m_object = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
        m_object->setPosition(pos);
        m_object->setOpacity(0.0f);
        m_playContainer->addChild(m_object, kTagObject, kTagObject);
    }
    
    float opacity;
    if (m_count == 1)
    {
        opacity = 0.15f * 255.0f;
    }
    else if (m_count == 2)
    {
        opacity = 0.35f * 255.0f;
    }
    else if (m_count == 3)
    {
        opacity = 0.5f * 255.0f;
    }
    else if (m_count == 4)
    {
        opacity = 1.0f * 255.0f;
    }
    
    auto fadeT1 = EaseSineOut::create( FadeTo::create(0.5f, opacity));
    
//    auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.05f));
//    auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 1.0f));
//    
//    auto scale_seq = Sequence::create(scaleT1, scaleT2, nullptr);
    
    auto jumpT =  EaseSineInOut::create( JumpTo::create(0.5f, m_object->getPosition(), 20, 1) );
    
    auto spawn = Spawn::create(fadeT1, jumpT, nullptr);
    
    m_object->runAction(spawn );
}

void Step1_Type1::wordAnimation()
{
    if (m_count < 4)
    {
        if (m_count == 1)
        {
            m_word->removeFromParent();
            
            std::string file;
            file = StringUtils::format("b01_c01_s1_t1_%02d_word_02", ProductManager::getInstance()->getCurrentAlphabet());
            m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
            m_playContainer->addChild(m_word, kTagWord, kTagWord);
            
            file = StringUtils::format("b01_c01_s1_t1_%02d_word_01", ProductManager::getInstance()->getCurrentAlphabet());
            m_alphabet = PsdParser::getPsdSprite(file, &m_psdDictionary);
            MGTUtils::setAnchorPointForPosition(m_alphabet, Vec2(1.0f, 0.0f));
            m_alphabet->setScale(1.0f);
            m_playContainer->addChild(m_alphabet, kTagAlphabet, kTagAlphabet);
        }
        
        
        auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.6f));
        auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 1.0f));
        
        auto action = Sequence::create(scaleT1,
                                       scaleT2,
                                       nullptr);
        
        
        m_alphabet->runAction(action);
        
        playPhonicsSound();
    }
    else if(m_count == 4)
    {
        m_word->removeFromParent();
        m_alphabet->removeFromParentAndCleanup(true);
        
        std::string file = StringUtils::format("b01_c01_s1_t1_%02d_word", ProductManager::getInstance()->getCurrentAlphabet());
        Vec2 pos = Vec2(PsdParser::getPsdPosition(file, &m_psdDictionary));
        m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
        m_word->setPosition(pos);
        m_playContainer->addChild(m_word, kTagWord, kTagWord);
        
        auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.6f));
        auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 1.0f));
        
        auto action = Sequence::create(scaleT1,
                                       scaleT2,
                                       nullptr);
        
        
        m_word->runAction(action);
        
        playWordSound();
        playComplete();
    }
}


void Step1_Type1::showAffordance()
{
    Step1_Base::showAffordance();
    
    if (m_line)
    {
        playAffordance(m_line);
    }
}

void Step1_Type1::hideAffordance()
{
    Step1_Base::hideAffordance();
    
    if (m_line)
    {
        stopAffordance(m_line);
    }
}


void Step1_Type1::interactionStart()
{
    m_touchEnabled = true;
    
    if (m_count == 0)
    {
        m_count++;
        showAffordance();
    }
}

void Step1_Type1::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    startAffordanceTimer();
}

void Step1_Type1::playStart()
{
    Step1_Base::playStart();
}

void Step1_Type1::playComplete()
{
    Step1_Base::playComplete();
}

