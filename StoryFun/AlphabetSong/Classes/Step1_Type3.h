#ifndef __STEP1_TYPE3_H__
#define __STEP1_TYPE3_H__

#include "cocos2d.h"
#include "Step1_Base.h"

#include "QType_DragAndDrop.h"

USING_NS_CC;

class Step1_Type3 : public Step1_Base
{
public:
    
    
    Step1_Type3();
    ~Step1_Type3();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step1_Type3);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override ;
    virtual void onTouchCancelled(Touch *touch, Event *event) override;
    
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
    
    
    
private:
    QType_DragAndDrop*          m_DragAndDropManager;
    
    draganddrop::DragAndDrop_Example*                       m_correctExampleObj;
    draganddrop::DragAndDrop_Question*                      m_correctQuestionObj;
    
    MGTSprite*                  m_object;
    MGTSprite*                  m_word;
    MGTSprite*                  m_alphabet;
    
    Sprite*                     m_objectOn;
    Sprite*                     m_maskSprite;
    Sprite*                     m_dummySprite;
    
    
    RenderTexture*              m_renderTexture;
    
    std::vector<Vec2>           m_positions;
    Vector<Sprite*>             m_examples;
    
    
    int                         m_count;
    

    
    
public:
    void initView();
    void createPuzzle();
    
    
    void correctAnswer();
    void correctAnimation();
    
    void objectShowAnimation();
    void wordAnimation();
    
    
    void interactionStart();
    void nextInteraction();
};

#endif // __STEP1_TYPE3_H__
