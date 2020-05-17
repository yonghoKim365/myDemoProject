
#ifndef __DragAndDrop_Example__
#define __DragAndDrop_Example__

#include "cocos2d.h"
#include "DragAndDrop_Question.h"

USING_NS_CC;


namespace draganddrop {
    
class DragAndDrop_Example : public Ref
{
public:
    
    DragAndDrop_Example();
    ~DragAndDrop_Example();

    
protected:
    Node*   m_object;
    Node*   _areaObject;
    bool    _isTouchEnabled;
    bool    _isComplete;
    Vec2    _originPos;
    float   _originScale;
    float   _originRotation;
    int     _originZorder;
    int     _exampleIndex;
    Image*   _imageData;
    
    
    draganddrop::DragAndDrop_Question* _correctQuestion;
    
public:
    
    void setObject(Node* node);
    Node* getObject();
    
    void setImageData(Image* imageData);
    Image* getImageData();
    
    void setOriginPosition(const Vec2& pos);
    Vec2 getOriginPosition();
    
    void setOriginScale(float scale);
    float getOriginScale();
    
    void setOriginRotation(float rotation);
    float getOriginRotation();
    
    void setOriginZorder(int zorder);
    int getOriginZorder();
    
    void setAreaCheckObject(Node* areaObject);
    Node* getAreaCheckObject();
    
    void setCorrectQuestion(draganddrop::DragAndDrop_Question* node);
    draganddrop::DragAndDrop_Question* getCorrectQuestion();
    
    void setIsTouchEnabled(bool isEnabled);
    bool getIsTouchEnabled();
    
    void setIsComplete(bool isComplete);
    bool getIsComplete();
    
    void setExampleNum(int num);
    int getExampleNum();
    
    void showAction();
    void touchAction(float scale =1.2f, float duration = 0.2f);
    void moveAction();
    void correctAction();
    void hideAction();
    void removeAction();
    void removeComplete();
    void wrongAction();
    void returnAction(float duration = 0.4f);
    void returnComplete();
};
    
}   //draganddrop namespace

#endif /* defined(__DragAndDrop_Example__) */
