#include "Step2_Type2.h"

enum
{
    kTagBg = 0,
    kTagBox,
    kTagWord,
    kTagAlphabet,
    kTagObject,
    kTagLight,
    kTagParticle,
    kTagObjectOn,
    kTagQuestion,
    kTagExample = 50,
    
    kTagFinish_Animation,
    kTagFinish_Sentence,
};

namespace example
{
    enum
    {
        SHADOW = 0,
        LINE,
        OBJ,
        ALPHABET,
    };
}


#define TOUCH_THRESHOLD 0.2

Step2_Type2::Step2_Type2():
m_count(0),
m_currentAlphabet(0),
m_correctExampleObj(nullptr),
m_correctQuestionObj(nullptr),
m_question(nullptr)
{

}

Step2_Type2::~Step2_Type2()
{
}

Scene* Step2_Type2::createScene()
{
    auto scene = Scene::create();

    auto layer = Step2_Type2::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step2_Type2::init()
{
    if ( !Step2_Base::init() )
    {
        return false;
    }
    
    m_currentAlphabet = ProductManager::getInstance()->getCurrentAlphabet();
    
    PsdParser::parseToPsdJSON("common_b01_c01_s2_t2.json", &m_psdDictionary, true);
    
    std::string file = StringUtils::format("b01_c01_s2_t2_%02d.json", m_currentAlphabet);
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    initView();
    
    return true;
}


void Step2_Type2::onEnter()
{
    Step2_Base::onEnter();
    
    onViewLoad();
}

void Step2_Type2::onExit()
{
    Step2_Base::onExit();
    
}

void Step2_Type2::onViewLoad()
{
    Step2_Base::onViewLoad();
}

void Step2_Type2::onViewLoaded()
{
    Step2_Base::onViewLoaded();
}


#pragma mark - touch

bool Step2_Type2::onTouchBegan(Touch *pTouch, Event *pEvent)
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
            
//            unschedule(CC_SCHEDULE_SELECTOR(Step2_Type2::onExampleTabActivate));
//            schedule(CC_SCHEDULE_SELECTOR(Step2_Type2::onExampleTabActivate), 0.05f);
            
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
        
        if (MGTUtils::hitTestPoint(m_object, tp, false))
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
        
        startAffordanceTimer();
    }
    
    return false;
}

void Step2_Type2::onTouchMoved(Touch *pTouch, Event *pEvent)
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

void Step2_Type2::onTouchEnded(Touch *pTouch, Event *pEvent)
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
                
                
                if (MGTUtils::hitTestObjects(examObj->getAreaCheckObject(), hitObject))
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

void Step2_Type2::onTouchCancelled(Touch *touch, Event *event)
{
    Step2_Base::onTouchCancelled(touch, event);
}

void Step2_Type2::onNarrationFinishedCallback(std::string fileName)
{
    Step2_Base::onNarrationFinishedCallback(fileName);
    log("Narration Finished fileName : %s", fileName.c_str());
    
    if (fileName == m_strWordSound)
    {
        if (m_eState == step2::STATE_PLAY)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                                        CallFunc::create( CC_CALLBACK_0(Step2_Type2::interactionStart, this)),
                                                        nullptr));
        }
        else if(m_eState == step2::STATE_PLAY_COMPLETE)
        {
            m_playContainer->runAction(Sequence::create( DelayTime::create(2.0f),
                                             CallFunc::create( CC_CALLBACK_0(Step2_Base::nextStep, this)),
                                             nullptr));
        }
    }
}


#pragma mark gaf delegate function
void Step2_Type2::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step2_Base::onFinishSequence(object, sequenceName);
}

void Step2_Type2::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step2_Base::onFramePlayed(object, frame);
}

void Step2_Type2::onTexturePreLoad(std::string& path)
{
    Step2_Base::onTexturePreLoad(path);
}



#pragma mark navi touch override function
void Step2_Type2::onTouchedNavigationButtonAtExit()
{
    Step2_Base::onTouchedNavigationButtonAtExit();
    
}

void Step2_Type2::onTouchedNavigationButtonAtNext()
{
    Step2_Base::onTouchedNavigationButtonAtNext();
}


void Step2_Type2::onTouchedPopupButtonAtNo()
{
    Step2_Base::onTouchedPopupButtonAtNo();
}

void Step2_Type2::onTouchedPopupButtonAtYes()
{
    Step2_Base::onTouchedPopupButtonAtYes();
}



#pragma mark step2_type2

void Step2_Type2::initView()
{
    Sprite* bg = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t2_bg.png"));
    bg->setPosition(winSize.width/2, winSize.height/2);
    m_playContainer->addChild(bg, kTagBg, kTagBg);
    
    
    std::string file;
    file = StringUtils::format("b01_c01_s2_t2_%02d_word_01", m_currentAlphabet);
    m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    m_box = PsdParser::getPsdCommonSprite("common_b01_c01_s2_t2_box_close", &m_psdDictionary);
    m_playContainer->addChild(m_box, kTagBox, kTagBox);
    
    file = StringUtils::format("b01_c01_s2_t2_%02d_object_off", m_currentAlphabet);
    m_object = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(m_object, kTagObject, kTagObject);

    
    createQuestion();
}

void Step2_Type2::createQuestion()
{
    m_DragAndDropManager = new QType_DragAndDrop();
    
    std::string file;
    file = StringUtils::format("b01_c01_s2_t2_%02d_object_key", m_currentAlphabet);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_question = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    m_question->setPosition(pos);
    m_playContainer->addChild(m_question, kTagQuestion, kTagQuestion);
    
    
//    int rotation[4] = {-12, 10, -4, 14};
    
    m_positions.push_back(Vec2(270, 736));
    m_positions.push_back(Vec2(206, 338));
    m_positions.push_back(Vec2(1646, 735));
    m_positions.push_back(Vec2(1720, 340));
    
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
        std::string examImg = StringUtils::format("common_b01_c01_s2_t2_object_%02d.png", _colors.at(i)+1);
        
        
        Sprite* exam = Sprite::create(getCommonFilePath("img", examImg));
        exam->setPosition(m_positions.at(i));
        exam->setOpacity(0.0f);
        exam->setScale(0.9f);
        int tag = kTagExample + i;
        m_playContainer->addChild(exam, tag, tag);
        
        Sprite* shadow = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t2_object_shadow.png"));
        shadow->setPosition(Vec2(shadow->getContentSize().width/2, shadow->getContentSize().height/2));
        shadow->setVisible(false);
        exam->addChild(shadow, example::SHADOW, example::SHADOW);
        
        Sprite* obj = Sprite::create(getCommonFilePath("img", examImg));
        obj->setPosition(Vec2(obj->getContentSize().width/2, obj->getContentSize().height/2));
        exam->addChild(obj, example::OBJ, example::OBJ);
        
        Sprite* line = Sprite::create(getCommonFilePath("img", "common_b01_c01_s2_t2_object_line.png"));
        line->setPosition(Vec2(exam->getContentSize().width/2 - 3, exam->getContentSize().height/2 +3));
        line->setOpacity(0.0f);
        exam->addChild(line, example::LINE, example::LINE);
        
        
        
        std::string alphabetImg = StringUtils::format("b01_c01_s2_t2_%02d_exam_word%02d.png", m_currentAlphabet, i+1);
        Sprite* alphabet = Sprite::create(getFilePath(ResourceType::IMAGE, "img", alphabetImg));
        
        Color3B color;
        if (_colors.at(i) == 0)
        {
            color = Color3B(132, 0, 64);
        }
        else if( _colors.at(i) == 1)
        {
            color = Color3B(173, 77, 0);
        }
        else if( _colors.at(i) == 2)
        {
            color = Color3B(0, 55, 167);
        }
        else if( _colors.at(i) == 3)
        {
            color = Color3B(78, 133, 0);
        }
        alphabet->setColor(color);
        
        
        Vec2 pos = Vec2(exam->getContentSize().width/2 - 3, exam->getContentSize().height/2 - 17);
        alphabet->setPosition(pos);
        exam->addChild(alphabet, example::ALPHABET, example::ALPHABET);
        

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

void Step2_Type2::correctAnswer()
{
    m_correctExampleObj = m_DragAndDropManager->getCurrentTouchedExample();
    m_correctQuestionObj = m_correctExampleObj->getCorrectQuestion();
    
    
    auto examSprite = m_correctExampleObj->getObject();
    auto pos = Vec2( m_correctQuestionObj->getObject()->getPosition().x + 4, m_correctQuestionObj->getObject()->getPosition().y - 3);
    
    auto moveT = EaseCircleActionOut::create(MoveTo::create(0.25f, pos));
    
    auto seq = Sequence::create(moveT,
                                CallFunc::create( CC_CALLBACK_0(Step2_Type2::removeWrongExamples, this)),
                                DelayTime::create(1.0f),
                                CallFunc::create( CC_CALLBACK_0(Step2_Type2::correctAnimation, this)),
                                nullptr);
    examSprite->runAction(seq);
    
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_correct_01.mp3"));
}

void Step2_Type2::correctAnimation()
{
    auto examSprite = m_correctExampleObj->getObject();
    auto queSprite = m_correctQuestionObj->getObject();
    
    queSprite->setVisible(false);
    examSprite->setVisible(false);
    m_word->removeFromParentAndCleanup(true);
    
    std::string file = StringUtils::format("b01_c01_s2_t2_%02d_word_02", m_currentAlphabet);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    m_word->setPosition(pos);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    
    this->objectShowAnimation();
    
    playComplete();
}

void Step2_Type2::removeWrongExamples()
{
    auto examples = m_DragAndDropManager->getExamples();
    
    for (int i = 0; i< examples->size(); i++)
    {
        auto example = examples->at(i);
        if (example != m_correctExampleObj)
        {
//            example->getObject()->setVisible(false);
            
            Sprite* examSp = (Sprite*)example->getObject();
            Vec2 pos = examSp->getPosition();
            
            if (pos.x < winSize.width/2)
            {
                MGTUtils::setAnchorPointForPosition(examSp, Vec2(0.5f, 0.9f));
                
                auto spawn = Spawn::create(EaseBackIn::create( RotateTo::create(0.5f, 90)),
                                           EaseSineOut::create( FadeTo::create(0.5f, 0.0f)),
                                           nullptr);
                examSp->runAction(spawn);
                
                MGTUtils::fadeToAllchildren(examSp, 0.2f, 0.0f, 0.4f);
                
//                examSp->runAction( EaseSineOut::create( MoveTo::create(0.5f, Vec2(-200, pos.y))));
            }
            else if (pos.x > winSize.width/2)
            {
                MGTUtils::setAnchorPointForPosition(examSp, Vec2(0.5f, 0.9f));
                
                auto spawn = Spawn::create(EaseBackIn::create( RotateTo::create(0.5f, -90)),
                                           EaseSineOut::create( FadeTo::create(0.5f, 0.0f)),
                                           nullptr);
                examSp->runAction(spawn);
                
                MGTUtils::fadeToAllchildren(examSp, 0.2f, 0.0f, 0.4f);
                
//                examSp->runAction( EaseSineOut::create( MoveTo::create(0.5f, Vec2(winSize.width + 200, pos.y))));
            }
        }
    }
}

void Step2_Type2::objectShowAnimation()
{
    m_object->removeFromParentAndCleanup(true);
    m_box->removeFromParentAndCleanup(true);
    
    m_box = PsdParser::getPsdCommonSprite("common_b01_c01_s2_t2_box_open", &m_psdDictionary);
    m_playContainer->addChild(m_box, kTagBox, kTagBox);
    
    
    
    Sprite* light = PsdParser::getPsdCommonSprite("common_b01_c01_s2_t2_light", &m_psdDictionary);
    light->setOpacity(0.0f);
    light->setScale(0.5f);
    m_playContainer->addChild(light, kTagLight, kTagLight);
    
    light->runAction(Spawn::create(EaseSineOut::create( ScaleTo::create(0.5f, 1.0f) ),
//                                   EaseSineOut::create( FadeTo::create(0.5f, 255.0f) ),
                                   nullptr));
                     
    
    light->runAction(RepeatForever::create( RotateBy::create(7.0f, 360)));
    
    light->runAction(RepeatForever::create( Sequence::create(FadeTo::create(0.6f, 255.0f*0.5f),
                                                             FadeTo::create(0.6f, 255.0f),
                                                             nullptr)));
    
    
    
    std::string file;
    file = StringUtils::format("b01_c01_s2_t2_%02d_object_on", m_currentAlphabet);
    m_objectOn = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_objectOn->setPosition(Vec2(m_objectOn->getPosition().x, m_objectOn->getPosition().y -200));
    m_objectOn->setScale(0.4f, 0.2f);
    m_playContainer->addChild(m_objectOn, kTagObjectOn, kTagObjectOn);
    
    auto spawn = Spawn::create(EaseBackOut::create( ScaleTo::create(0.5f, 1.0f)),
                               EaseBackOut::create( MoveBy::create(0.5f, Vec2(0, 200))),
                               nullptr);
    
    m_objectOn->runAction( Sequence::create(spawn,
                                            DelayTime::create(0.5f),
                                            CallFunc::create( CC_CALLBACK_0(Step2_Type2::objectScaleAnimation, this)),
                                            CallFunc::create( CC_CALLBACK_0(Step2_Base::playWordSound, this)),
                                            nullptr));
    
    
    auto emitter = ParticleSystemQuad::create(getCommonFilePath("img", "common_b01_c01_s2_t2_particle_star.plist"));
    emitter->setPosition(light->getPosition());
    emitter->setTexture(Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", "common_b01_c01_s2_t2_star.png")));
    m_playContainer->addChild(emitter, kTagParticle, kTagParticle);
    
    
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s2_t2_sfx_open.mp3"));
}

void Step2_Type2::objectScaleAnimation()
{
    m_objectOn->runAction(Sequence::create( EaseSineIn::create(ScaleTo::create(0.4f, 1.15f)),
                                           EaseSineOut::create(ScaleTo::create(0.4f, 1.0f)),
                                           nullptr));
}

void Step2_Type2::wordAnimation()
{
    auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.0f));
    auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 0.5f));
    
    auto action = Sequence::create(scaleT1,
                                   scaleT2,
                                   nullptr);
    
    
    m_alphabet->runAction(action);
}

void Step2_Type2::showAffordance()
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

void Step2_Type2::hideAffordance()
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

void Step2_Type2::interactionStart()
{
    m_touchEnabled = true;
    
    if (m_count == 0)
    {
        m_count++;
        showAffordance();
    }
}

void Step2_Type2::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    startAffordanceTimer();
}

void Step2_Type2::finishAnimation()
{
    
}


void Step2_Type2::playStart()
{
    Step2_Base::playStart();
}

void Step2_Type2::playComplete()
{
    Step2_Base::playComplete();
}



void Step2_Type2::onExampleTabActivate(float dt)
{
    float elapsedTime = getCurrentTime() - m_touchBeganTime;
    
    if (m_isTouchObject == true)
    {
        if(elapsedTime < TOUCH_THRESHOLD)
        {
            if (m_isTouchMoved == true )
            {
                auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
                touchExam->touchAction(1.0f);
                
                unschedule(CC_SCHEDULE_SELECTOR(Step2_Type2::onExampleTabActivate));
            }
        }
        else if(elapsedTime >= TOUCH_THRESHOLD)
        {
            auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
            touchExam->touchAction(1.0f);
            
            unschedule(CC_SCHEDULE_SELECTOR(Step2_Type2::onExampleTabActivate));
        }
    }
    else
    {
        unschedule(CC_SCHEDULE_SELECTOR(Step2_Type2::onExampleTabActivate));
    }
}
