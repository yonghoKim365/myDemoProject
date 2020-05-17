#ifndef __DEBUG_INDEX_H__
#define __DEBUG_INDEX_H__

#include "cocos2d.h"
#include "Base_Layer.h"

USING_NS_CC;

class Debug_Index : public Base_Layer
{
public:
    
    
    Debug_Index();
    ~Debug_Index();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Debug_Index);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override ;
    
    

public:
    
    int _stepNum;
    int _alphabetNum;
    
    void initView();
    
    void createStepMenu();
    void stepMenuCallback(Ref* sender);
    void setStepEnabled(int currentNum);
    
    void createAlphabetMenu();
    void alphabetMenuCallback(Ref* sender);
    void setAlphabetEnabled(int currentNum);
    
    void createStartMenu();
    void startMenuCallback(Ref* sender);
    
};

#endif // __DEBUG_INDEX_H__
