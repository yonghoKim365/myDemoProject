#include "Step2_Type1.h"

enum
{
    kTagBg = 0,
    kTagRainbow,
    kTagObjectShadow,
    kTagQuestion,
    kTagWord,
    kTagAlphabet,
    kTagObject,
    kTagObjectOn,
    kTagRainEffect,
    
    kTagParticle,
    kTagExample = 50,
    
    
    kTagFinish_Animation,
    kTagFinish_Sentence,
};

namespace example
{
    enum
    {
        LINE = 0,
        ALPHABET,
    };
}

#define TOUCH_THRESHOLD 0.2

Step2_Type1::Step2_Type1():
m_count(0),
m_currentAlphabet(0),
m_correctExampleObj(nullptr),
m_correctQuestionObj(nullptr),
m_maskSprite(nullptr),
m_question(nullptr)
{

}

Step2_Type1::~Step2_Type1()
{
}

Scene* Step2_Type1::createScene()
{
    auto scene = Scene::create();

    auto layer = Step2_Type1::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step2_Type1::init()
{
    if ( !Step2_Base::init() )
    {
        return false;
    }
    
    
    m_currentAlphabet = ProductManager::getInstance()->getCurrentAlphabet();
    
    std::string file = StringUtils::format("b01_c01_s2_t1_%02d.json", m_currentAlphabet);
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    initView();
    
    return true;
}


void Step2_Type1::onEnter()
{
    Step2_Base::onEnter();
    
    onViewLoad();
}

void Step2_Type1::onExit()
{
    Step2_Base::onExit();
    
}

void Step2_Type1::onViewLoad()
{
    Step2_Base::onViewLoad();
}

void Step2_Type1::onViewLoaded()
{
    Step2_Base::onViewLoaded();
}


#pragma mark - touch

bool Step2_Type1::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    if(Step2_Base::onTouchBegan(pTouch, pEvent))
    {
        return false;
    }
    
    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    m_touch = pTouch;
    m_isTouchMoved = false;
    
    m_touchBeganTime = getCurrentTime();
    
    if (m_eState == step2::STATE_PLAY)
    {
        stopAffordanceTimer();
        
        if(m_DragAndDropManager->getTouchCheckExample(tp))
        {
            m_touchEnabled = false;
            m_isTouchObject = true;
            
            hideAffordance();
            
//            unschedule(CC_SCHEDULE_SELECTOR(Step2_Type1::onExampleTabActivate));
//            schedule(CC_SCHEDULE_SELECTOR(Step2_Type1::onExampleTabActivate), 0.05f);
            
            auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
            touchExam->touchAction(1.0f);
            
            auto set = m_exampleSets.at(m_currentAlphabet-1);
            auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
            int alphabetNum = set->at(examObj->getExampleNum()-1);

            playPhonicsSound(alphabetNum);
            
            return true;
        }
        
        
        
        if (MGTUtils::hitTestPoint(m_word, tp, false))
        {
            playWordSound();
            hideAffordance();
            
            startAffordanceTimer();
            
            return false;
        }
        
        if (MGTUtils::hitTestPoint(m_question, tp, false))
        {
            playPhonicsSound(m_currentAlphabet);
            hideAffordance();
            
            startAffordanceTimer();
            
            return false;
        }
        
        std::string file = StringUtils::format("b01_c01_s2_t1_%02d_object_off.png", m_currentAlphabet);
        Image* image = MGTResourceUtils::getInstance()->getImageData(getFilePath(ResourceType::IMAGE, "img", file));
        if (MGTUtils::hitTestPointExact(m_object, image, tp, false))
        {
            playWordSound();
            hideAffordance();
            
            startAffordanceTimer();
            
            return false;
        }
        
        startAffordanceTimer();
        
    }

    return false;
}

void Step2_Type1::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Step2_Base::onTouchMoved(pTouch, pEvent);
    
    log("TOUCH MOVED");
    
    
    if (m_touchEnabled == true)
    {
        return;
    }
    
    if(m_orientation != DeviceUtilManager::getInstance()->getScreenOrientation())
    {
        log("CHANGE ORIENTATION");
        onTouchCancelled(pTouch, pEvent);
        return;
    }
    
    log("TOUCH MOVED   1");
    
    Vec2 tp = pTouch->getLocation();
    Vec2 prevTp = pTouch->getPreviousLocation();
    
    auto currentTouch = convertToNodeSpace(tp);
    auto prevTouch = convertToNodeSpace(prevTp);

    Vec2 movePos = tp - prevTp;
    

    
    //    if (m_touch->getID() == pTouch->getID())
    //    {
    //        if (currentTouch.getDistance(prevTp) > 10)
    //        {
    //            m_isTouchMoved = true;
    //        }
    //    }
    
    if (m_isTouchObject == true)
    {
        //    auto sp = m_touchExample.sprite;
        auto touchExam = m_DragAndDropManager->getCurrentTouchedExample()->getObject();
        Vec2 targetPos = touchExam->getPosition() + movePos;
        touchExam->setPosition(targetPos);
    }
    
}

void Step2_Type1::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Step2_Base::onTouchEnded(pTouch, pEvent);
    
    Vec2 tp = pTouch->getLocation();
    
    float elapsedTime = getCurrentTime() - m_touchBeganTime;
        
    if (m_isTouchObject == true)
    {
//        if(elapsedTime < TOUCH_THRESHOLD && m_isTouchMoved == false)
//        {
//            auto set = m_exampleSets.at(ProductManager::getInstance()->getCurrentAlphabet()-1);
//            auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
//            int alphabetNum = set->at(examObj->getExampleNum()-1);
//            
//            playPhonicsSound(alphabetNum);
//            
//            startAffordanceTimer();
//            m_touchEnabled = true;
//            
//        }
//        else
//        {
            if(m_DragAndDropManager->confirmCorrectAnswer())
            {
                auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
                auto questionObj = m_DragAndDropManager->getCurrentTouchedExample()->getCorrectQuestion();
                
                if (m_DragAndDropManager->getTotalQuestionNum() == m_DragAndDropManager->getSolvedNum())
                {
                    m_touchEnabled = false;
                    
                    correctAnswer();
                }
            }
            else
            {
                auto hitObject = m_playContainer->getChildByTag(kTagQuestion);
                
                auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
                examObj->returnAction();
                
                
                if (MGTUtils::hitTestPoint(examObj->getAreaCheckObject(), hitObject->getPosition(), false))
                {
                    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_wrong_01.mp3"));
                }
                
                startAffordanceTimer();
                m_touchEnabled = true;
            }
//        }
        
        m_DragAndDropManager->setTouchExample(nullptr);
    }
    
    
    m_touch = nullptr;
    m_isTouchMoved = false;
    m_isTouchObject = false;
}

void Step2_Type1::onTouchCancelled(Touch *touch, Event *event)
{
    Step2_Base::onTouchCancelled(touch, event);
}


void Step2_Type1::onNarrationFinishedCallback(std::string fileName)
{
    Step2_Base::onNarrationFinishedCallback(fileName);
    log("Narration Finished fileName : %s", fileName.c_str());
    
    if (fileName == m_strWordSound)
    {
        if (m_eState == step2::STATE_PLAY)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                                        CallFunc::create( CC_CALLBACK_0(Step2_Type1::interactionStart, this)),
                                                        nullptr));
        }
        else if(m_eState == step2::STATE_PLAY_COMPLETE)
        {
            log("play complete");
            m_playContainer->runAction(Sequence::create( DelayTime::create(2.0f),
                                             CallFunc::create( CC_CALLBACK_0(Step2_Base::nextStep, this)),
                                             nullptr));
        }
    }
}


#pragma mark gaf delegate function
void Step2_Type1::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step2_Base::onFinishSequence(object, sequenceName);
    
    if(sequenceName.compare("Animation") == 0 )
    {
        log("gaf finish");
        
        m_gafObject->setSequenceDelegate(nullptr);
        m_gafObject->setSoundPlayDelegate(nullptr);
        
        if(m_gafObject)
        {
            m_gafObject->pauseAllAnimation();
        }
    }
    else if(sequenceName.compare("animation") == 0 )
    {
        log("loop");
        m_gafObject->setSequenceDelegate(nullptr);
        m_gafObject->setSoundPlayDelegate(nullptr);
        
        m_gafObject->playSequence("loop", true);
    }
}

void Step2_Type1::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step2_Base::onFramePlayed(object, frame);
}

void Step2_Type1::onTexturePreLoad(std::string& path)
{
    Step2_Base::onTexturePreLoad(path);
}


#pragma mark navi touch override function
void Step2_Type1::onTouchedNavigationButtonAtExit()
{
    Step2_Base::onTouchedNavigationButtonAtExit();
    
}

void Step2_Type1::onTouchedNavigationButtonAtNext()
{
    Step2_Base::onTouchedNavigationButtonAtNext();
}


void Step2_Type1::onTouchedPopupButtonAtNo()
{
    Step2_Base::onTouchedPopupButtonAtNo();
}

void Step2_Type1::onTouchedPopupButtonAtYes()
{
    Step2_Base::onTouchedPopupButtonAtYes();
}


#pragma mark step2_type1

void Step2_Type1::initView()
{
    std::string file;
    
    int alphabetNum = m_currentAlphabet;
    if ( alphabetNum == 2 || alphabetNum == 11 || alphabetNum == 14 || alphabetNum == 18 || alphabetNum == 21 || alphabetNum == 22)
    {
        file = "common_b01_c01_s2_t1_bg01.jpg";
    }
    else if ( alphabetNum == 8 || alphabetNum == 12 || alphabetNum == 16 || alphabetNum == 17 || alphabetNum == 20 || alphabetNum == 23)
    {
        file = "common_b01_c01_s2_t1_bg02.jpg";
    }
    else if ( alphabetNum == 4 || alphabetNum == 6)
    {
        file = "common_b01_c01_s2_t1_bg03.jpg";
    }
    
    Sprite* bg = Sprite::create(getCommonFilePath("img", file));
    bg->setPosition(winSize.width/2, winSize.height/2);
    m_playContainer->addChild(bg, kTagBg, kTagBg);
    
    file = StringUtils::format("b01_c01_s2_t1_%02d_word_01", alphabetNum);
    m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    file = StringUtils::format("b01_c01_s2_t1_%02d_object", alphabetNum);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_object = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_off.png")));
    m_object->setPosition(pos);
    m_playContainer->addChild(m_object, kTagObject, kTagObject);
    
    
    if (alphabetNum == 8 || alphabetNum == 12 || alphabetNum == 17 || alphabetNum == 20 || alphabetNum == 23)
    {
        file = StringUtils::format("b01_c01_s2_t1_%02d_object_shadow", alphabetNum);
        pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
        Sprite* shadow = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
        shadow->setPosition(pos);
        m_playContainer->addChild(shadow, kTagObjectShadow, kTagObjectShadow);
    }

    createQuestion();
    
    
    file = StringUtils::format("b01_c01_s2_t1_%02d_object", m_currentAlphabet);
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    m_renderTexture = RenderTexture::create(m_object->getContentSize().width, m_object->getContentSize().height);
    m_renderTexture->setPosition(pos);
    m_renderTexture->clear(0, 0, 0, 0);
    m_playContainer->addChild(m_renderTexture, kTagObjectOn);
    
    
    m_gafObject = createCommonGAFObject(m_playContainer, kTagRainbow, getCommonFilePath("flash", "common_b01_c01_s2_t1_rainbow.gaf"), true, Vec2(0, 1200.0f));
    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step2_Type1::onFinishSequence, this));
    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
    
    m_gafObject->setVisible(false);
    m_gafObject->stop();
    
    
    
    m_dummySprite = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_mask.png"));
    m_dummySprite->setAnchorPoint(Vec2(0.5f, 1.0f));
    m_dummySprite->setPosition(Vec2(m_dummySprite->getContentSize().width/2, m_object->getContentSize().height/2 + m_object->getPosition().x + m_dummySprite->getContentSize().height ));
    m_dummySprite->setOpacity(0.0f);
    m_playContainer->addChild(m_dummySprite, 0);
    
    
    file = StringUtils::format("b01_c01_s2_t1_%02d_object", m_currentAlphabet);
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    m_objectOn = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
    m_objectOn->setPosition(pos);
    MGTUtils::setPositionForParent(m_object, m_objectOn);
    m_objectOn->setOpacity(0.0f);
//    m_objectOn->setBlendFunc((BlendFunc){GL_DST_ALPHA, GL_ZERO});
    m_playContainer->addChild(m_objectOn, 0);
    
    m_maskSprite = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_mask.png"));
    m_maskSprite->setAnchorPoint(Vec2(0.5f, 1.0f));
    m_maskSprite->setPosition(m_dummySprite->getPosition());
    m_maskSprite->setOpacity(0.0f);
    m_playContainer->addChild(m_maskSprite, 0);
    
    
    
    schedule(CC_SCHEDULE_SELECTOR(Step2_Type1::scheduleShowObject), 0.05f);
}

void Step2_Type1::createQuestion()
{
    m_DragAndDropManager = new QType_DragAndDrop();
    
    std::string file;
    
    file = "common_b01_c01_s2_t1_que_object_default";
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_question = Sprite::create(getCommonFilePath("img", file.append(".png")));
    m_question->setPosition(pos);
    m_playContainer->addChild(m_question, kTagQuestion, kTagQuestion);
    
    
//    int rotation[4] = {-12, 10, -4, 14};
    
    m_positions.push_back(Vec2(245, winSize.height-750 - 20));
    m_positions.push_back(Vec2(363, winSize.height-446 - 25));
    m_positions.push_back(Vec2(1556, winSize.height-446 - 25));
    m_positions.push_back(Vec2(1672, winSize.height-755 - 20));
    
    srand(unsigned(time(NULL)));
    std::random_shuffle(m_positions.begin(), m_positions.end(), MGTUtils::random_user);
    
    
    std::vector<int> _colors;
    for (int i = 0; i<4; i++)
    {
        _colors.push_back(i);
    }
    std::random_shuffle(_colors.begin(), _colors.end(), MGTUtils::random_user);
    
    for (int i = 0; i<4; i++)
    {
        Sprite* exam = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_que_object_paste.png"));
        exam->setPosition(m_positions.at(i));
        exam->setScale(0.9f);
        int tag = kTagExample + i;
        m_playContainer->addChild(exam, tag, tag);
        
        
        file = StringUtils::format("b01_c01_s2_t1_%02d_exam_word%02d.png", m_currentAlphabet, i+1);
        Sprite* alphabet = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
        
        Color3B color;
        if (_colors.at(i) == 0)
        {
            color = Color3B(240, 99, 167);
        }
        else if( _colors.at(i) == 1)
        {
            color = Color3B(104, 175, 229);
        }
        else if( _colors.at(i) == 2)
        {
            color = Color3B(255, 189, 69);
        }
        else if( _colors.at(i) == 3)
        {
            color = Color3B(180, 212, 12);
        }
        alphabet->setColor(color);
        
        Vec2 pos = Vec2(exam->getContentSize().width/2, exam->getContentSize().height/2);
        alphabet->setPosition(pos);
        exam->addChild(alphabet, example::ALPHABET, example::ALPHABET);
        
        Sprite* line = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_que_object_line.png"));
        line->setPosition(Vec2(exam->getContentSize().width/2, exam->getContentSize().height/2));
        line->setOpacity(0.0f);
        exam->addChild(line, example::LINE, example::LINE);

        m_examples.pushBack(exam);
    }
    
    
    //set draganddrop manager
    
    draganddrop::DragAndDrop_Question* dragQuestion = new draganddrop::DragAndDrop_Question();
    dragQuestion->setObject(m_question);
    dragQuestion->setAreaCheckObject(m_question);
    
    m_DragAndDropManager->setQustion(dragQuestion);
    
    int correctAnswerNum = 1;
    
    for (int i = 0; i<m_examples.size(); i++)
    {
//        float rot = rotation[i];
        
        float rot = 0;
        auto exam = m_examples.at(i);
        draganddrop::DragAndDrop_Example* dragExample = new draganddrop::DragAndDrop_Example();
        dragExample->setObject(exam);
        dragExample->setAreaCheckObject(exam);
        dragExample->setIsComplete(false);
        dragExample->setIsTouchEnabled(true);
        dragExample->setOriginPosition(exam->getPosition());
        dragExample->setOriginScale(exam->getScale());
        dragExample->setOriginRotation(rot);
        dragExample->setOriginZorder(exam->getLocalZOrder());
        dragExample->setExampleNum(i+1);
        
        
        if(correctAnswerNum == i+1)
        {
            dragExample->setCorrectQuestion(dragQuestion);
            m_DragAndDropManager->setExample(dragExample, dragQuestion);
        }
        else
        {
            m_DragAndDropManager->setExample(dragExample, nullptr);
        }
        
        
        Vec2 originPos = exam->getPosition();
    }
}

void Step2_Type1::correctAnswer()
{
    m_correctExampleObj = m_DragAndDropManager->getCurrentTouchedExample();
    m_correctQuestionObj = m_correctExampleObj->getCorrectQuestion();
    
    
    auto examSprite = m_correctExampleObj->getObject();
    auto pos = m_correctQuestionObj->getObject()->getPosition();
    
    auto moveT = EaseSineOut::create(MoveTo::create(0.25f, pos));
    
    auto seq = Sequence::create(moveT,
                                CallFunc::create( CC_CALLBACK_0(Step2_Type1::removeWrongExamples, this)),
                                DelayTime::create(0.5f),
                                CallFunc::create( CC_CALLBACK_0(Step2_Type1::correctAnimation, this)),
                                nullptr);
    examSprite->runAction(seq);
    
    scheduleRainEffect(0.0f);
    schedule(CC_SCHEDULE_SELECTOR(Step2_Type1::scheduleRainEffect), 0.7f);
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_correct_01.mp3"));

    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s2_t1_sfx_rain.mp3"));
}

void Step2_Type1::correctAnimation()
{
    auto examSprite = m_correctExampleObj->getObject();
    auto queSprite = m_correctQuestionObj->getObject();
    
    queSprite->setVisible(false);
    
    
//    this->removeWrongExamples();
    
    
    this->cloudMoveAnimation();
    this->objectShowAnimation();
    
    m_word->runAction( EaseSineOut::create( FadeTo::create(0.5f, 0.0f)));
    
//    m_gafObject->start();
//    m_gafObject->setVisible(true);
//    m_gafObject->playSequence("animation", false);
    
    
    playComplete();
}

void Step2_Type1::removeWrongExamples()
{
    auto examples = m_DragAndDropManager->getExamples();
    
    for (int i = 0; i< examples->size(); i++)
    {
        auto example = examples->at(i);
        if (example != m_correctExampleObj)
        {
            example->getObject()->setVisible(false);
            
            auto emitter = ParticleSystemQuad::create(getCommonFilePath("img", "common_b01_c01_s2_t1_particle_cloud.plist"));
            emitter->setPosition(example->getObject()->getPosition());
            m_playContainer->addChild(emitter, kTagParticle, kTagParticle);
        }
    }
}

void Step2_Type1::cloudMoveAnimation()
{
    auto examSprite = m_correctExampleObj->getObject();
    auto queSprite = m_correctQuestionObj->getObject();
    
    auto word = m_playContainer->getChildByTag(kTagWord);
    
    Vec2 targetPos = Vec2(word->getPosition().x + word->getContentSize().width/2 + examSprite->getContentSize().width/2, word->getPosition().y);
    float duration = 0.003f * queSprite->getContentSize().width;
    
    log("duration :%f", duration);
    
    auto moveT = MoveTo::create(duration, targetPos);
    
    auto moveT1 = MoveBy::create(1.0f, Vec2(examSprite->getContentSize().width, 50));
    auto rotT1 = RotateTo::create(1.0f, 0.0f) ;
    auto fadeT1 = FadeTo::create(1.0f, 0.0f);
    auto spawn = Spawn::create(moveT1, rotT1, fadeT1, nullptr);
    
    auto seq = Sequence::create(
                                DelayTime::create(0.2f),
                                moveT,
                                spawn,
                                nullptr);
    
    examSprite->runAction(seq);
    
    
    m_playContainer->runAction(Sequence::create(DelayTime::create(duration),
                                     CallFunc::create( CC_CALLBACK_0(Step2_Type1::unscheduleRainEffect, this)),
                                     nullptr));
    
    auto alphabet = examSprite->getChildByTag(example::ALPHABET);
    alphabet->runAction(Sequence::create(DelayTime::create(duration),
                                         FadeTo::create(0.5f, 0.0f),
                                         nullptr));
    
}


void Step2_Type1::showRainEffect(int num)
{
    auto examSprite = m_correctExampleObj->getObject();

    std::string filename = StringUtils::format("common_b01_c01_s2_t1_rain%02d.png", num+1);
    Sprite* rain = Sprite::create(getCommonFilePath("img", filename));
    
    int randNum = arc4random()%300;
    float bandwidthX = randNum - 150;
    
    rain->setPosition(Vec2( examSprite->getPosition().x + bandwidthX, examSprite->getPosition().y ));
    rain->setScale(1.3f);
    m_playContainer->addChild(rain, kTagRainEffect, kTagRainEffect);
    
    auto moveT = EaseSineIn::create( MoveTo::create(0.8f, Vec2(rain->getPosition().x, -100)) );
    auto scaleT = EaseSineIn::create( ScaleTo::create(0.8f, 3.0f));
    
    auto seq_fade = Sequence::create(DelayTime::create(0.5f),
                                     EaseSineIn::create(FadeTo::create(0.3f, 0.0f)),
                                     nullptr);
    
    auto spawn = Spawn::create(moveT, scaleT, seq_fade, nullptr);
    
    auto seq = Sequence::create(spawn,
                                nullptr);
    
    rain->runAction(seq);
}

void Step2_Type1::scheduleRainEffect(float dt)
{
    log("rain effect");

    auto examSprite = m_correctExampleObj->getObject();
    
    
    std::vector<int> _colors;
    for (int i = 0; i<7; i++)
    {
        _colors.push_back(i);
    }
    std::random_shuffle(_colors.begin(), _colors.end(), MGTUtils::random_user);
    
    
    for (int i = 0; i<7; i++)
    {
        float duration = i*0.1f;
        auto seq = Sequence::create(DelayTime::create(duration),
                                    CallFunc::create( CC_CALLBACK_0(Step2_Type1::showRainEffect,this, _colors.at(i))),
                                    nullptr);
        m_playContainer->runAction(seq);
    }
}

void Step2_Type1::unscheduleRainEffect()
{
    log("rain STOP");
    unschedule(CC_SCHEDULE_SELECTOR(Step2_Type1::scheduleRainEffect));
    
    
    m_gafObject->start();
    m_gafObject->setVisible(true);
    m_gafObject->playSequence("animation", false);
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s2_t1_sfx_rainbow.mp3"));
}

void Step2_Type1::objectShowAnimation()
{
    auto queSprite = m_correctQuestionObj->getObject();
    float duration = 0.003f * queSprite->getContentSize().width;
    
    auto moveT = MoveTo::create(duration, Vec2(m_dummySprite->getContentSize().width/2, m_object->getContentSize().height/2 + m_object->getPosition().y));
    
    auto seq = Sequence::create(moveT,
                                CallFunc::create( CC_CALLBACK_0(Step2_Type1::unscheduleShowObject, this)),
                                DelayTime::create(1.0f),
                                CallFunc::create( CC_CALLBACK_0(Step2_Type1::showEndingAnimation, this)),
                                nullptr);
    
    m_dummySprite->runAction(seq);

}

void Step2_Type1::scheduleShowObject(float dt)
{
    std::string file = StringUtils::format("b01_c01_s2_t1_%02d_object", m_currentAlphabet);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
//    m_objectOn->setOpacity(255.0f);
//    m_objectOn->setBlendFunc((BlendFunc){GL_DST_ALPHA, GL_ZERO});
    
    m_objectOn = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
    m_objectOn->setPosition(pos);
    MGTUtils::setPositionForParent(m_object, m_objectOn);
    
    
    
    m_maskSprite = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_mask.png"));
    m_maskSprite->setAnchorPoint(Vec2(0.5f, 1.0f));
    m_maskSprite->setPosition(m_dummySprite->getPosition());
    MGTUtils::setPositionForParent(m_object, m_maskSprite);

    m_maskSprite->setBlendFunc((BlendFunc){GL_ONE, GL_ZERO});
    m_objectOn->setBlendFunc((BlendFunc){GL_DST_ALPHA, GL_ZERO});

    
//    log("schedule SHOW OBJECT");
//    log("POS:%f %f", m_dummySprite->getPosition().x, m_dummySprite->getPosition().y);
//    log("POS:%f %f", m_maskSprite->getPosition().x, m_maskSprite->getPosition().y);
    
    
    
    m_renderTexture->beginWithClear(0, 0, 0, 0);
    m_maskSprite->visit();
    m_objectOn->visit();
    m_renderTexture->end();
    Director::getInstance()->getRenderer()->render();
}


void Step2_Type1::unscheduleShowObject()
{
    log("unschedule show object");
    unschedule(CC_SCHEDULE_SELECTOR(Step2_Type1::scheduleShowObject));
    
    m_object->removeFromParentAndCleanup(true);
    m_renderTexture->removeFromParentAndCleanup(true);
    
    std::string file = StringUtils::format("b01_c01_s2_t1_%02d_object", m_currentAlphabet);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_objectOn = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
    m_objectOn->setPosition(pos);
    m_playContainer->addChild(m_objectOn, kTagObject);
    
    
}


void Step2_Type1::showEndingAnimation()
{
    auto examSprite = m_correctExampleObj->getObject();
    examSprite->setVisible(false);
    
    m_word->removeFromParentAndCleanup(true);
    
    std::string file = StringUtils::format("b01_c01_s2_t1_%02d_cloud_end", m_currentAlphabet);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    m_word->setPosition(pos);
    m_word->setOpacity(0.0f);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    m_word->runAction(Sequence::create(EaseSineOut::create(FadeTo::create(0.5f, 255.0f)),
                                       DelayTime::create(1.0f),
                                       CallFunc::create( CC_CALLBACK_0(Step2_Type1::objectScaleAnimation, this)),
                                       CallFunc::create( CC_CALLBACK_0(Step2_Base::playWordSound, this)),
                                       nullptr) );
    
    
//    Sprite* rainbow = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t1_rainbow.png"));
//    rainbow->setPosition(Vec2(winSize.width/2, winSize.height/2));
//    rainbow->setOpacity(0.0f);
//    m_playContainer->addChild(rainbow, kTagRainbow, kTagRainbow);
//    
//    rainbow->runAction(EaseSineOut::create( FadeTo::create(0.5f, 255.0f) ));
    
    
//    m_gafObject = createGAFObject(m_playContainer, kTagRainbow, getCommonFilePath("flash", "common_b01_c01_s2_t1_rainbow.gaf"), true, Vec2(0, 1200.0f));
//    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step2_Type1::onFinishSequence, this));
//    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
//    
//    m_gafObject->playSequence("Animation", false);
}

void Step2_Type1::objectScaleAnimation()
{
    m_objectOn->runAction(Sequence::create( EaseSineIn::create(ScaleTo::create(0.4f, 1.15f)),
                                           EaseSineOut::create(ScaleTo::create(0.4f, 1.0f)),
                                           nullptr));
}

void Step2_Type1::wordAnimation()
{
    auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.0f));
    auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 0.5f));
    
    auto action = Sequence::create(scaleT1,
                                   scaleT2,
                                   nullptr);
    
    
    m_alphabet->runAction(action);
}

void Step2_Type1::showAffordance()
{
    Step2_Base::showAffordance();
    
    for (int i = 0; i<m_examples.size(); i++)
    {
        auto exam = m_examples.at(i);
        Sprite* line = (Sprite*)exam->getChildByTag(example::LINE);
        
        if (line)
        {
            playAffordance(line);
        }
    }
}

void Step2_Type1::hideAffordance()
{
    Step2_Base::hideAffordance();
    
    for (int i = 0; i<m_examples.size(); i++)
    {
        auto exam = m_examples.at(i);
        Sprite* line = (Sprite*)exam->getChildByTag(example::LINE);
        
        if (line)
        {
            stopAffordance(line);
        }
    }
}

void Step2_Type1::interactionStart()
{
    m_touchEnabled = true;
    
    if (m_count == 0)
    {
        m_count++;
        showAffordance();
    }
}

void Step2_Type1::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    startAffordanceTimer();
}

void Step2_Type1::finishAnimation()
{

}

void Step2_Type1::playStart()
{
    Step2_Base::playStart();
}

void Step2_Type1::playComplete()
{
    Step2_Base::playComplete();
}


void Step2_Type1::onExampleTabActivate(float dt)
{
    float elapsedTime = getCurrentTime() - m_touchBeganTime;
    
    if(elapsedTime < TOUCH_THRESHOLD)
    {
        if (m_isTouchMoved == true )
        {
            auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
            touchExam->touchAction(1.0f);
            
            unschedule(CC_SCHEDULE_SELECTOR(Step2_Type1::onExampleTabActivate));
        }
    }
    else if(elapsedTime >= TOUCH_THRESHOLD)
    {
        if (m_isTouchObject == true)
        {
            auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
            touchExam->touchAction(1.0f);
        }
        
        unschedule(CC_SCHEDULE_SELECTOR(Step2_Type1::onExampleTabActivate));
    }
}
