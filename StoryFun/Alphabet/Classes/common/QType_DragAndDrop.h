
#ifndef __QType_DragAndDrop__
#define __QType_DragAndDrop__

#include "cocos2d.h"

USING_NS_CC;

#include "DragAndDrop_Example.h"
#include "DragAndDrop_Question.h"

#include "GAF.h"
#include "GAFTimelineAction.h"

NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;


class QType_DragAndDrop : public Ref
{
public:
    
    QType_DragAndDrop();
    ~QType_DragAndDrop();

    
public:

    int     m_nTotalQuestionNum;
    int     m_nSolvedNum;

    
    
    Vector<draganddrop::DragAndDrop_Question*>      m_questions;
    Vector<draganddrop::DragAndDrop_Example*>       m_examples;
    
    draganddrop::DragAndDrop_Example* _currentTouchedExample;

public:
    
    void setQustion(draganddrop::DragAndDrop_Question* question);
    Vector<draganddrop::DragAndDrop_Question*>* getQuestions();
    draganddrop::DragAndDrop_Question* getQuestion(int num);
    void setExample(draganddrop::DragAndDrop_Example* example, draganddrop::DragAndDrop_Question* question);
    Vector<draganddrop::DragAndDrop_Example*>* getExamples();
    
    int getTotalQuestionNum();
    int getSolvedNum();

    void setTouchExample(draganddrop::DragAndDrop_Example* example);
    bool getTouchCheckExample(const Vec2& pos);
    draganddrop::DragAndDrop_Example* getTouchCheckExampleForGAF(const Vec2& pos);
    draganddrop::DragAndDrop_Example* getCurrentTouchedExample();
    
    bool confirmCorrectAnswer();
    bool confirmCorrectAnswerPoint();
    bool hitTestPointForGAF(gaf::GAFObject* obj, const Vec2& pos);
    
    void removeAllExamples();
    
    void reset();

};

#endif /* defined(__QType_DragAndDrop__) */
