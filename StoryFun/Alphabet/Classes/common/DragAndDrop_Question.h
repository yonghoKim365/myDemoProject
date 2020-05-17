
#ifndef __DragAndDrop_Question__
#define __DragAndDrop_Question__

#include "cocos2d.h"
//#include "DragAndDrop_Example.h"

USING_NS_CC;

class   DragAndDrop_Example;

namespace draganddrop {

    
class DragAndDrop_Question : public Ref
{
public:
    
    DragAndDrop_Question();
    ~DragAndDrop_Question();


protected:
    Node*   _object;
    Node*   _areaObject;
    
//    draganddrop::DragAndDrop_Example* _correctExample;
    
    bool    _isComplete;
    int     _correctNum;
    
public:
    void setObject(Node* node);
    Node* getObject();
    
    void setAreaCheckObject(Node* node);
    Node* getAreaCheckObject();
    
//    void setCorrectExample(draganddrop::DragAndDrop_Example* node);
//    draganddrop::DragAndDrop_Example* getCorrectExample();
    
    void setIsComplete(bool isComplete);
    bool getIsComplete();
    
    void correctAction();
    void wrongAction();
    
    void setCorrectNum(int num);
    int getCorrectNum();
    
    
};
    
}   //draganddrop namespace
#endif /* defined(__DragAndDrop_Question__) */
