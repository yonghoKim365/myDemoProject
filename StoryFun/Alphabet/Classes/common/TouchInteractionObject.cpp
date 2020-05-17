//
//  TouchInteractionObject.cpp
//
//
//
//
//

#include "TouchInteractionObject.h"


TouchInteractionObject::TouchInteractionObject()
{
    _isTouched = false;
    _isCorrect = false;
    _object = nullptr;
    _textObject = nullptr;
    _guidePos = Vec2::ZERO;
}

TouchInteractionObject::~TouchInteractionObject()
{

}


void TouchInteractionObject::setObject(Node* node){
    _object = node;
}

void TouchInteractionObject::setTextObject(Node* node)
{
    _textObject = node;
}

Node* TouchInteractionObject::getObject()
{
    return _object;
}

Node* TouchInteractionObject::getTextObject()
{
    return _textObject;
}

void TouchInteractionObject::setIsTouched(bool isTouched)
{
    _isTouched = isTouched;
}

bool TouchInteractionObject::getIsTouched()
{
    return _isTouched;
}

void TouchInteractionObject::setIsCorrect(bool isCorrect)
{
    _isCorrect = isCorrect;
}

bool TouchInteractionObject::getIsCorrect()
{
    return _isCorrect;
}

void TouchInteractionObject::setGuidePosition(Vec2 pos)
{
    _guidePos = pos;
}
Vec2 TouchInteractionObject::getGuidePosition()
{
    return _guidePos;
}