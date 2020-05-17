
#include "DragAndDrop_Example.h"

namespace draganddrop {
    
    DragAndDrop_Example::DragAndDrop_Example()
    {
        m_object = nullptr;
        _areaObject = nullptr;
        _isTouchEnabled = false;
        _isComplete = false;
        _originZorder = 0;
        _originPos = Vec2::ZERO;
        _originScale = 1.0f;
        _originRotation = 0.0f;
    }

    DragAndDrop_Example::~DragAndDrop_Example()
    {
        
    }
    
    void DragAndDrop_Example::setObject(Node* node)
    {
        m_object = node;
    }
    
    Node* DragAndDrop_Example::getObject()
    {return m_object;
    }
    
    void DragAndDrop_Example::setImageData(Image* imageData)
    {
        _imageData = imageData;
    }
    
    Image* DragAndDrop_Example::getImageData()
    {
        return _imageData;
    }
    
    
    void DragAndDrop_Example::setOriginPosition(const Vec2& pos)
    {
        _originPos = pos;
    }
    
    Vec2 DragAndDrop_Example::getOriginPosition()
    {
        return _originPos;
    }
    
    void DragAndDrop_Example::setOriginScale(float scale)
    {
        _originScale = scale;
    }
    
    float DragAndDrop_Example::getOriginScale()
    {
        return _originScale;
    }
    
    void DragAndDrop_Example::setOriginRotation(float rotation)
    {
        _originRotation = rotation;
    }
    
    float DragAndDrop_Example::getOriginRotation()
    {
        return _originRotation;
    }
    
    void DragAndDrop_Example::setOriginZorder(int zorder)
    {
        _originZorder = zorder;
    }
    int DragAndDrop_Example::getOriginZorder()
    {
        return _originZorder;
    }
    

    void DragAndDrop_Example::setAreaCheckObject(Node* areaObject)
    {
        _areaObject = areaObject;
    }
    Node* DragAndDrop_Example::getAreaCheckObject()
    {
        return _areaObject;
    }
    
    void DragAndDrop_Example::setCorrectQuestion(DragAndDrop_Question* question)
    {
        _correctQuestion = question;
    }
    
    DragAndDrop_Question* DragAndDrop_Example::getCorrectQuestion()
    {
        return _correctQuestion;
    }
    
    void DragAndDrop_Example::setIsTouchEnabled(bool isEnabled)
    {
        _isTouchEnabled = isEnabled;
    }
    bool DragAndDrop_Example::getIsTouchEnabled()
    {
        return _isTouchEnabled;
    }
    
    void DragAndDrop_Example::setIsComplete(bool isComplete)
    {
        _isComplete = isComplete;
    }
    
    bool DragAndDrop_Example::getIsComplete()
    {
        return _isComplete;
    }
    
    void DragAndDrop_Example::setExampleNum(int num)
    {
        _exampleIndex = num;
    }
    
    int DragAndDrop_Example::getExampleNum()
    {
        return _exampleIndex;
    }
    
    #pragma mark action function
    
    void DragAndDrop_Example::showAction()
    {
        
    }

    void DragAndDrop_Example::touchAction(float scale, float duration)
    {
        _isTouchEnabled = false;
        m_object->setLocalZOrder(1000);
        
        auto action1 = EaseSineOut::create(ScaleTo::create(duration, scale));
        auto action2 = EaseSineOut::create(RotateTo::create(duration, 0.0f));
        
        auto spawn = Spawn::create(action1, action2, nullptr);
        
        m_object->runAction(spawn);
    }
    
    void DragAndDrop_Example::moveAction()
    {
        
    }
    void DragAndDrop_Example::correctAction()
    {
        
    }
    void DragAndDrop_Example::hideAction()
    {
        auto action1 = EaseSineOut::create(FadeTo::create(0.2f, 0.0f));
        m_object->runAction(action1);
    }
    
    void DragAndDrop_Example::removeAction()
    {
        auto action1 = EaseSineOut::create(FadeTo::create(0.2f, 0.0f));
        auto seq = Sequence::create(action1,
                                    CallFunc::create( CC_CALLBACK_0(DragAndDrop_Example::removeComplete, this)),
                                    nullptr);
        m_object->runAction(seq);
    }
    
    void DragAndDrop_Example::removeComplete()
    {
        _isComplete = true;
        _isTouchEnabled = false;
        m_object->stopAllActions();
        m_object->removeAllChildren();
        m_object->removeFromParent();
    }
    
    void DragAndDrop_Example::wrongAction()
    {
        
    }
    
    void DragAndDrop_Example::returnAction(float duration)
    {
        log("pos :%f, %f", getOriginPosition().x, getOriginPosition().y);
        
        auto action1 = EaseSineOut::create(ScaleTo::create(duration, 1.0f));
        auto action2 = EaseSineOut::create(MoveTo::create(duration, getOriginPosition()));
        auto action3 = EaseSineOut::create(RotateTo::create(duration, getOriginRotation()));
        auto action4 = EaseSineOut::create(ScaleTo::create(duration, getOriginScale()));
        
        auto spawn = Spawn::create(action1, action2, action3, action4, nullptr);
        
        auto seq = Sequence::create(spawn,
                                    CallFunc::create( CC_CALLBACK_0(DragAndDrop_Example::returnComplete, this)),
                                    nullptr);
        
        m_object->runAction(seq);
    }
    
    void DragAndDrop_Example::returnComplete()
    {
        m_object->setLocalZOrder(_originZorder);
        _isTouchEnabled = true;
    }
    
}   //draganddrop namespace