
#include "QType_DragAndDrop.h"
#include "MGTUtils.h"

QType_DragAndDrop::QType_DragAndDrop()
{
    m_nTotalQuestionNum = 0;
    m_nSolvedNum = 0;
}

QType_DragAndDrop::~QType_DragAndDrop()
{
    m_questions.clear();
    m_examples.clear();
}

void QType_DragAndDrop::setQustion(draganddrop::DragAndDrop_Question* question)
{
    m_questions.pushBack(question);
    m_nTotalQuestionNum++;
}


Vector<draganddrop::DragAndDrop_Question*>* QType_DragAndDrop::getQuestions()
{
    return &m_questions;
}

draganddrop::DragAndDrop_Question* QType_DragAndDrop::getQuestion(int num)
{
    return m_questions.at(num-1);
}

void QType_DragAndDrop::setExample(draganddrop::DragAndDrop_Example* example, draganddrop::DragAndDrop_Question* question)
{
    example->setCorrectQuestion(question);
    m_examples.pushBack(example);
}

Vector<draganddrop::DragAndDrop_Example*>* QType_DragAndDrop::getExamples()
{
    return &m_examples;
}


int QType_DragAndDrop::getTotalQuestionNum()
{
    return m_nTotalQuestionNum;
}

int QType_DragAndDrop::getSolvedNum()
{
    return m_nSolvedNum;
}

void QType_DragAndDrop::setTouchExample(draganddrop::DragAndDrop_Example *example)
{
    _currentTouchedExample = example;
}

bool QType_DragAndDrop::getTouchCheckExample(const Vec2& pos)
{
    for (int i = 0; i<m_examples.size(); i++)
    {
        auto example = m_examples.at(i);
        
        if(example->getIsComplete() == false)
        {
            if (example->getIsTouchEnabled() == true)
            {
                if(MGTUtils::hitTestPoint(example->getAreaCheckObject(), pos, false))
                {
                    _currentTouchedExample = example;
                    
                    return true;
                }
            }
        }
    }
    return false;
}

draganddrop::DragAndDrop_Example* QType_DragAndDrop::getTouchCheckExampleForGAF(const Vec2& pos)
{
    for (int i = 0; i<m_examples.size(); i++)
    {
        auto example = m_examples.at(i);
        
        if(hitTestPointForGAF((gaf::GAFObject*)example->getAreaCheckObject(), pos))
        {
            _currentTouchedExample = example;
            
            return example;
        }
    }
    return nullptr;
}



draganddrop::DragAndDrop_Example* QType_DragAndDrop::getCurrentTouchedExample()
{
    return _currentTouchedExample;
}


bool QType_DragAndDrop::confirmCorrectAnswer()
{
    bool bRet = false;
    auto example = getCurrentTouchedExample();
    auto question = example->getCorrectQuestion();
    
    if (question == nullptr)
    {
        return false;
    }
    
    log("example size : %f  %f", example->getAreaCheckObject()->getContentSize().width, example->getAreaCheckObject()->getContentSize().height);
    
    if (MGTUtils::hitTestPoint(example->getAreaCheckObject(), question->getAreaCheckObject()->getPosition(), false))
    {
        m_nSolvedNum++;
        example->setIsComplete(true);
        bRet = true;
    }
    
    return bRet;
}

bool QType_DragAndDrop::confirmCorrectAnswerPoint()
{
    bool bRet = false;
    auto example = getCurrentTouchedExample();
    auto question = example->getCorrectQuestion();
    
    if (question == nullptr)
    {
        return false;
    }
    
    if (MGTUtils::hitTestPoint(example->getAreaCheckObject(), question->getAreaCheckObject()->getPosition(), false))
    {
        m_nSolvedNum++;
        example->setIsComplete(true);
        bRet = true;
    }
    
    return bRet;
}



bool QType_DragAndDrop::hitTestPointForGAF(gaf::GAFObject* obj, const Vec2& pos)
{
    bool bRet = false;
    Vec2 localPoint = obj->convertToNodeSpace(pos);
    
    Rect r = obj->getBoundingBoxForCurrentFrame();
    localPoint = cocos2d::PointApplyTransform(localPoint, obj->getNodeToParentTransform());
    
    //    CCLOG("BOUNDING BOX SIZE:%f, %f ", r.size.width, r.size.height);
    
    if(r.containsPoint(localPoint))
    {
        bRet = true;
    }
    return bRet;
}


void QType_DragAndDrop::removeAllExamples()
{
    for (int i = 0; i<m_examples.size(); i++)
    {
        auto example = m_examples.at(i);
        example->removeAction();
    }
}

void QType_DragAndDrop::reset()
{
    m_nSolvedNum = 0;

}
