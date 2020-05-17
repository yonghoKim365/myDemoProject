#ifndef __STEP1_TYPE1_H__
#define __STEP1_TYPE1_H__

#include "cocos2d.h"
#include "Step1_Base.h"

USING_NS_CC;

class Step1_Type1 : public Step1_Base
{
public:
    
    
    Step1_Type1();
    ~Step1_Type1();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step1_Type1);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override;
    
    
public:
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
    virtual void showAffordance() override;
    virtual void hideAffordance() override;
    
    virtual void playStart() override;
    virtual void playComplete() override;
    
public:
    virtual void onTouchedNavigationButtonAtExit() override;
    virtual void onTouchedNavigationButtonAtNext() override;

    virtual void onTouchedPopupButtonAtNo() override;
    virtual void onTouchedPopupButtonAtYes() override;
    

    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName) override;
    virtual void onTexturePreLoad(std::string& path) override;
    virtual void onFramePlayed(GAFObject* object, uint32_t frame) override;
    
    
public:
    Sprite*                 m_object;
    Sprite*                 m_word;
    Sprite*                 m_alphabet;
    Sprite*                 m_line;
    
    bool                    m_isTouchObject;
    
    int                     m_count;
    
    
public:
    void initView();
    
    void touchObject();
    void objectAnimation();
    void wordAnimation();
    
    void interactionStart();
    void nextInteraction();
    
};

#endif // __STEP1_TYPE1_H__
