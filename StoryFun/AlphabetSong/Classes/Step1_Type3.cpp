#include "Step1_Type3.h"
#include "Debug_Index.h"
#include "DeviceUtilManager.h"

enum
{
    kTagBoard,
    kTagWord,
    kTagAlphabet,
    kTagObject,
    kTagObjectOn,
    kTagRainEffect,
    kTagQuestion,
    kTagExample = 50,
    
    kTagFinish_Animation,
    kTagFinish_Sentence,
};

namespace example
{
    enum
    {
        ALPHABET = 0,
        LINE,
    };
}


Step1_Type3::Step1_Type3():
m_count(0),
m_correctExampleObj(nullptr),
m_correctQuestionObj(nullptr)
{

}

Step1_Type3::~Step1_Type3()
{
}

Scene* Step1_Type3::createScene()
{
    auto scene = Scene::create();

    auto layer = Step1_Type3::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step1_Type3::init()
{
    if ( !Step1_Base::init() )
    {
        return false;
    }
    
    std::string file = StringUtils::format("b01_c01_s1_t3_%02d.json", ProductManager::getInstance()->getCurrentAlphabet());
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    initView();
    
    return true;
}


void Step1_Type3::onEnter()
{
    Step1_Base::onEnter();
    
    onViewLoad();
}

void Step1_Type3::onExit()
{
    Step1_Base::onExit();
    
}

void Step1_Type3::onViewLoad()
{
    Step1_Base::onViewLoad();

}

void Step1_Type3::onViewLoaded()
{
    Step1_Base::onViewLoaded();
}


#pragma mark - touch

bool Step1_Type3::onTouchBegan(Touch *pTouch, Event *pEvent)
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
        
        if(m_DragAndDropManager->getTouchCheckExample(tp))
        {
            m_touchEnabled = false;
            
            hideAffordance();
            
            auto touchExam = m_DragAndDropManager->getCurrentTouchedExample();
            touchExam->touchAction(1.0f);
            
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

void Step1_Type3::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchMoved(pTouch, pEvent);
    
    log("TOUCH MOVED");
    
    if (m_DragAndDropManager->getCurrentTouchedExample() == nullptr)
    {
        return;
    }
    
    if(m_orientation != DeviceUtilManager::getInstance()->getScreenOrientation())
    {
        log("CHANGE ORIENTATION");
        onTouchCancelled(pTouch, pEvent);
        return;
    }
    
    log("TOUCH MOVED 1");
    
    Vec2 tp = pTouch->getLocation();
    Vec2 prevTp = pTouch->getPreviousLocation();
    
    Vec2 movePos = tp - prevTp;
    

    //    auto sp = m_touchExample.sprite;
    auto touchExam = m_DragAndDropManager->getCurrentTouchedExample()->getObject();
    Vec2 targetPos = touchExam->getPosition() + movePos;
    touchExam->setPosition(targetPos);
    
    
    log("ORIENTATION %d", DeviceUtilManager::getInstance()->getScreenOrientation());
//    log("TP posX:%f, posY:%f", tp.x, tp.y);
//    log("MOVE posX:%f, posY:%f", movePos.x, movePos.y);
//    log("TARGET posX:%f, posY:%f", targetPos.x, targetPos.y);
//    

}

void Step1_Type3::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchEnded(pTouch, pEvent);
    
    log("TOUCH ENDED");
    
    Vec2 tp = pTouch->getLocation();
    
    
    if(m_DragAndDropManager->confirmCorrectAnswer())
    {
        auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
        auto questionObj = m_DragAndDropManager->getCurrentTouchedExample()->getCorrectQuestion();
        
        
        log("m_DragAndDropManager->getTotalQuestionNum : %d", m_DragAndDropManager->getTotalQuestionNum());
        
        
        log("m_DragAndDropManager->getSolvedNum() : %d", m_DragAndDropManager->getSolvedNum());
        
        correctAnswer();
        
        if (m_DragAndDropManager->getTotalQuestionNum() == m_DragAndDropManager->getSolvedNum())
        {
            m_touchEnabled = false;
        }
    }
    else
    {
        auto examObj = m_DragAndDropManager->getCurrentTouchedExample();
        auto questions = m_DragAndDropManager->getQuestions();
        
        examObj->returnAction();
        
        for (int i = 0; i<questions->size(); i++)
        {
            auto hitObject = questions->at(i)->getAreaCheckObject();
            
            if (MGTUtils::hitTestObjects(examObj->getAreaCheckObject(), hitObject))
            {
                MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_wrong_01.mp3"));
                break;
            }
        }
        
        startAffordanceTimer();
        m_touchEnabled = true;
    }
    
    m_DragAndDropManager->setTouchExample(nullptr);
}

void Step1_Type3::onTouchCancelled(Touch *touch, Event *event)
{
    Step1_Base::onTouchCancelled(touch, event);
}


void Step1_Type3::onNarrationFinishedCallback(std::string fileName)
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
                                                            CallFunc::create( CC_CALLBACK_0(Step1_Type3::interactionStart, this)),
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
                                             CallFunc::create( CC_CALLBACK_0(Step1_Type3::nextInteraction, this)),
                                             nullptr));
        }
    }
}


#pragma mark gaf delegate function
void Step1_Type3::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step1_Base::onFinishSequence(object, sequenceName);
}

void Step1_Type3::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step1_Base::onFramePlayed(object, frame);
}

void Step1_Type3::onTexturePreLoad(std::string& path)
{
    Step1_Base::onTexturePreLoad(path);
}


#pragma mark navi touch override function
void Step1_Type3::onTouchedNavigationButtonAtExit()
{
    Step1_Base::onTouchedNavigationButtonAtExit();
    
}

void Step1_Type3::onTouchedNavigationButtonAtNext()
{
    Step1_Base::onTouchedNavigationButtonAtNext();
}


void Step1_Type3::onTouchedPopupButtonAtNo()
{
    Step1_Base::onTouchedPopupButtonAtNo();
}

void Step1_Type3::onTouchedPopupButtonAtYes()
{
    Step1_Base::onTouchedPopupButtonAtYes();
}


#pragma mark step1_type3

void Step1_Type3::initView()
{
    m_playContainer = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t3_bg.jpg"));
    //    m_playContainer->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
    //    m_playContainer->setColor(Color3B(255, 247, 233));
//    m_playContainer->setAnchorPoint(Vec2::ZERO);
    m_playContainer->setPosition(winSize.width/2, winSize.height/2);
    this->addChild(m_playContainer, kTagPlayContainer, kTagPlayContainer);
    
    std::string file;
    
    file = StringUtils::format("b01_c01_s1_t3_%02d_word_off", ProductManager::getInstance()->getCurrentAlphabet());
    m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);

    
    file = StringUtils::format("b01_c01_s1_t3_%02d_board", ProductManager::getInstance()->getCurrentAlphabet());
    Sprite* board = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(board, kTagBoard, kTagBoard);
    
//    m_object = PsdParser::getPsdSprite("b01_c01_s2_t1_02_object_off", &m_psdDictionary);
//    m_playContainer->addChild(m_object, kTagObject, kTagObject);

    createPuzzle();
}

void Step1_Type3::createPuzzle()
{
    m_DragAndDropManager = new QType_DragAndDrop();

    

    std::vector<int> m_puzzles;
    for (int i = 1; i<=6; i++)
    {
        m_puzzles.push_back(i);
    }
    std::random_shuffle(m_puzzles.begin(), m_puzzles.end(), MGTUtils::random_user);
    
    
    
    m_positions.push_back(Vec2(204, 692));
    m_positions.push_back(Vec2(204, 338));
    m_positions.push_back(Vec2(1716, 692));
    m_positions.push_back(Vec2(1703, 322));
    
    srand(unsigned(time(NULL)));
    std::random_shuffle(m_positions.begin(), m_positions.end(), MGTUtils::random_user);
    
    
    for (int i = 0; i<6; i++)
    {
        
        std::string examImg = StringUtils::format("b01_c01_s1_t3_%02d_object_%02d", ProductManager::getInstance()->getCurrentAlphabet(), m_puzzles.at(i));
        
        Sprite* exam = PsdParser::getPsdSprite(examImg, &m_psdDictionary);
        
        if (i < m_positions.size())
        {
            exam->setScale(0.8f);
//            exam->setPosition(m_positions.at(i));
        }
        
        exam->setVisible(false);
        
        int tag = kTagExample + i;
        m_playContainer->addChild(exam, tag, tag);
        
        std::string file = StringUtils::format("common_b01_c01_s1_t3_line_%02d.png", m_puzzles.at(i));
        Sprite* line = Sprite::create(getCommonFilePath("img", file));
        line->setPosition(Vec2(exam->getContentSize().width/2, exam->getContentSize().height/2));
        line->setColor(Color3B(255, 126, 0));
        line->setVisible(false);
        exam->addChild(line, example::LINE, example::LINE);
        
        
        m_examples.pushBack(exam);
    }
    
    
    //set draganddrop manager
    for (int i = 0; i<4; i++)
    {
        auto exam = m_examples.at(i);
        
        Sprite* question = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t3_question.png"));
        question->setPosition(exam->getPosition());
        int tag = kTagQuestion + i;
        question->setVisible(false);
        m_playContainer->addChild(question, tag, tag);
    
        
        draganddrop::DragAndDrop_Question* dragQuestion = new draganddrop::DragAndDrop_Question();
        dragQuestion->setObject(question);
        dragQuestion->setAreaCheckObject(question);
        m_DragAndDropManager->setQustion(dragQuestion);
        
        
        float rot = 0;
        
        exam->setPosition(m_positions.at(i));
        Vec2 originPos = exam->getPosition();
        exam->setVisible(true);
        
        draganddrop::DragAndDrop_Example* dragExample = new draganddrop::DragAndDrop_Example();
        dragExample->setObject(exam);
        dragExample->setAreaCheckObject(exam);
        
        
        dragExample->setOriginPosition(exam->getPosition());
        dragExample->setOriginScale(exam->getScale());
        dragExample->setOriginRotation(rot);
        dragExample->setOriginZorder(exam->getLocalZOrder());
        
        
        
        
        
        dragExample->setIsComplete(false);
        dragExample->setIsTouchEnabled(true);
        
        dragExample->setCorrectQuestion(dragQuestion);
        m_DragAndDropManager->setExample(dragExample, dragQuestion);
    }
    
    
    
    for (int j = 0 ; j < m_examples.size()-m_positions.size(); j++)
    {
        auto exam = m_examples.at(m_positions.size()+j);
        exam->setVisible(true);
        
    }

}


void Step1_Type3::correctAnswer()
{
    log("correct");
    m_correctExampleObj = m_DragAndDropManager->getCurrentTouchedExample();
    m_correctQuestionObj = m_correctExampleObj->getCorrectQuestion();
    
    
    auto examSprite = m_correctExampleObj->getObject();
    auto pos = m_correctQuestionObj->getObject()->getPosition();
    
    auto moveT = EaseCircleActionOut::create(MoveTo::create(0.25f, pos));
    
    auto seq = Sequence::create(moveT,
                                DelayTime::create(0.25f),
                                CallFunc::create( CC_CALLBACK_0(Step1_Type3::correctAnimation, this)),
                                nullptr);
    examSprite->runAction(seq);

    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s1_t3_sfx_drop.mp3"));
}

void Step1_Type3::correctAnimation()
{
    auto examSprite = m_correctExampleObj->getObject();
    auto queSprite = m_correctQuestionObj->getObject();
    
    queSprite->setVisible(false);
    
    if (m_count == 4)
    {
        this->objectShowAnimation();
    }
    
    this->wordAnimation();
}

void Step1_Type3::wordAnimation()
{
    if (m_count < 4)
    {
        if (m_count == 1)
        {
            m_word->removeFromParent();
            
            std::string file;
            
            file = StringUtils::format("b01_c01_s1_t3_%02d_word_02", ProductManager::getInstance()->getCurrentAlphabet());
            m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
            m_playContainer->addChild(m_word, kTagWord, kTagWord);
            
            file = StringUtils::format("b01_c01_s1_t3_%02d_word_01", ProductManager::getInstance()->getCurrentAlphabet());
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
        
        std::string file = StringUtils::format("b01_c01_s1_t3_%02d_word_on", ProductManager::getInstance()->getCurrentAlphabet());
        m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
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


void Step1_Type3::objectShowAnimation()
{
    for (int i = 0; i < m_examples.size(); i++)
    {
        auto exam = m_examples.at(i);
        exam->setVisible(false);
    }
    
    std::string file = StringUtils::format("b01_c01_s1_t3_%02d_puzzle_all", ProductManager::getInstance()->getCurrentAlphabet());
    auto puzzle_complete = PsdParser::getPsdSprite(file, &m_psdDictionary);
    m_playContainer->addChild(puzzle_complete, kTagObjectOn, kTagObjectOn);
    
}


void Step1_Type3::showAffordance()
{
    Step1_Base::showAffordance();
    
    auto examples = m_DragAndDropManager->getExamples();
    
    for (int i = 0; i<examples->size(); i++)
    {
        auto exam = examples->at(i);
        
        if (exam->getIsComplete() == false)
        {
            auto examSp = exam->getObject();
            Sprite* line = (Sprite*)examSp->getChildByTag(example::LINE);
            
            if (line)
            {
                playAffordance(line);
            }
        }
    }
}

void Step1_Type3::hideAffordance()
{
    Step1_Base::hideAffordance();
    
    auto examples = m_DragAndDropManager->getExamples();
    
    for (int i = 0; i<examples->size(); i++)
    {
        auto exam = examples->at(i);
        
        if (exam->getIsComplete() == false)
        {
            auto examSp = exam->getObject();
            Sprite* line = (Sprite*)examSp->getChildByTag(example::LINE);
            
            if (line)
            {
                stopAffordance(line);
            }
        }
    }
}


void Step1_Type3::interactionStart()
{
    m_touchEnabled = true;
    
    if (m_count == 0)
    {
        m_count++;
        showAffordance();
    }
}

void Step1_Type3::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    startAffordanceTimer();
}


void Step1_Type3::playStart()
{
    Step1_Base::playStart();
}

void Step1_Type3::playComplete()
{
    Step1_Base::playComplete();
}
