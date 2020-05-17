//
//  MGTSprite.h
//  MGT_Template
//
// on 13. 6. 21..
//
//

#ifndef MGT_Template_MGTSprite_h
#define MGT_Template_MGTSprite_h

#include "cocos2d.h"

using namespace cocos2d;

class MGTSprite : public Sprite {
public:
    Vec2         m_originPosition;
    Vec2         m_secondPosition;   // position for user mind
private:
    bool            _bTouchComplete;
    Action*       _defaultTouchAction;
    Action*       _defaultTouchActionReverse;
    
    Action*       _touchAction;
    Action*       _touchActionReverse;
    
    bool            _bIsTouchEnable;
    float           _scale;
    float           _opacity;
    
    SEL_CallFunc    _callfuncSelector;
    Node*         _delegate;
    
public:
    ~MGTSprite();
    MGTSprite();
    
    static MGTSprite* create(const char *pszFileName);
    static MGTSprite* createWithFullPath(const char *pszFileName);
    static MGTSprite* createWithCommonPath(const char *pszFileName);
//    static MGTSprite* createWithTexture(Texture2D *pTexture, const Rect& rect);
    CREATE_FUNC(MGTSprite);
    
    virtual bool initWithFile(const char *pszFilename);
    virtual bool init();
    virtual void onEnter();
    virtual void onExit();
    
    virtual void addChild(Node *child);
    virtual void addChild(Node *child, int zOrder);
    virtual void addChild(Node *child, int zOrder, int tag);
    
    void addChildIgnoreParent(Node *child);
    void addChildIgnoreParent(Node *child, int zOrder);
    void addChildIgnoreParent(Node *child, int zOrder, int tag);
    
    virtual bool onTouchBegan(Touch* touch, Event* event);
    virtual void onTouchMoved(Touch* touch, Event* event);
    virtual void onTouchEnded(Touch* touch, Event* event);
    
    //touch action
    void setTouchEnable(bool enable);
    bool isTouchEnabled(){return _bIsTouchEnable;};
    
//    void onTouchEvent();
    void setTouchEvent(Node* delegate, SEL_CallFunc selector);
    
    void setTouchAction(Action* action);
    void setTouchActionReverse(Action* action);
    
    void onTouchActionEvent();
    void onTouchEndActionEvent();
    
    //position
    void setAnchorPointWithoutPosition(Vec2 anchor);
    void setPositionForParent();
    void setPositionForRootParent(Node* rootParents);
    
    void setComplete(bool complete){_bTouchComplete = complete;};
    bool isComplete(){return _bTouchComplete;};
    
    //texture
    void changeTexture(std::string filePath);
    void changeTextureWithFullPath(std::string fileName);
    void changeTextureWithCommonPath(std::string fileName);
    
private:
    void _setPositionForParent(Node* child);
    void setDefualtOption();
    
    
    

};


#endif
