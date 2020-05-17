
#include "DragAndDrop_Question.h"

namespace draganddrop{

    DragAndDrop_Question::DragAndDrop_Question()
    {
        _object = nullptr;
        _areaObject = nullptr;
        
        _isComplete = false;
//        _correctExample = nullptr;
    }

    DragAndDrop_Question::~DragAndDrop_Question()
    {
        
    }

    void DragAndDrop_Question::setObject(Node* node)
    {
        _object = node;
    }

    Node* DragAndDrop_Question::getObject()
    {
        return _object;
    }

    void DragAndDrop_Question::setAreaCheckObject(Node* node)
    {
        _areaObject = node;
    }
    
    Node* DragAndDrop_Question::getAreaCheckObject()
    {
        return _areaObject;
    }

//    void DragAndDrop_Question::setCorrectExample(draganddrop::DragAndDrop_Example* example)
//    {
//        _correctExample = example;
//    }
//    
//    draganddrop::DragAndDrop_Example* DragAndDrop_Question::getCorrectExample()
//    {
//        return _correctExample;
//    }

    void DragAndDrop_Question::setIsComplete(bool isComplete)
    {
        _isComplete = isComplete;
    }
        
    bool DragAndDrop_Question::getIsComplete()
    {
        return _isComplete;
    }

    void DragAndDrop_Question::setCorrectNum(int num)
    {
        _correctNum = num;
    }
    
    int DragAndDrop_Question::getCorrectNum()
    {
        return _correctNum;
    }

#pragma mark action function

    void DragAndDrop_Question::correctAction()
    {
        
    }
    void DragAndDrop_Question::wrongAction()
    {
        
    }
    


}   //draganddrop namespace