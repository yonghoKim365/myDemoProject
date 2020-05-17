//
//  TouchInteractionObject.h
//  
//
//
//
//


#ifndef TouchInteractionObject_h
#define TouchInteractionObject_h

#include "cocos2d.h"

USING_NS_CC;

class TouchInteractionObject : public Ref
{
public:

    
protected:
    bool _isTouched;
    bool _isCorrect;
    Node* _object;
    Node* _textObject;
    
    Vec2    _guidePos;
    
public:
    
    TouchInteractionObject();
    ~TouchInteractionObject();
    
    void setObject(Node* node);
    Node* getObject();
    
    void setTextObject(Node* node);
    Node* getTextObject();
    
    void setIsTouched(bool isTouched);
    bool getIsTouched();
    
    void setIsCorrect(bool isCorrect);
    bool getIsCorrect();
    
    
    void setGuidePosition(Vec2 pos);
    Vec2 getGuidePosition();
    
};

#endif
