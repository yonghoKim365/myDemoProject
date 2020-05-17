#ifndef __STEP2_TYPE1_H__
#define __STEP2_TYPE1_H__

#include "cocos2d.h"
#include "Step2_Base.h"

#include "QType_DragAndDrop.h"

USING_NS_CC;

class Step2_Type1 : public Step2_Base
{
public:
    
    
    Step2_Type1();
    ~Step2_Type1();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step2_Type1);
    
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
    
    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName) override;
    virtual void onTexturePreLoad(std::string& path) override;
    virtual void onFramePlayed(GAFObject* object, uint32_t frame) override;
    
    
public:
    virtual void onTouchedNavigationButtonAtExit() override;
    virtual void onTouchedNavigationButtonAtNext() override;
    
    virtual void onTouchedPopupButtonAtNo() override;
    virtual void onTouchedPopupButtonAtYes() override;
    
    
    
    
private:
    QType_DragAndDrop*          m_DragAndDropManager;
    
    draganddrop::DragAndDrop_Example*                       m_correctExampleObj;
    draganddrop::DragAndDrop_Question*                      m_correctQuestionObj;
    
    Sprite*                  m_object;
    Sprite*                  m_word;
    Sprite*                  m_alphabet;
    
    Sprite*                     m_objectOn;
    Sprite*                     m_maskSprite;
    Sprite*                     m_dummySprite;
    
    
    RenderTexture*              m_renderTexture;
    
    std::vector<Vec2>           m_positions;
    Vector<Sprite*>             m_examples;
    
    Sprite*                     m_question;
    
    int                         m_count;
    int                         m_currentAlphabet;
    
//    Vector<ParticleSystemQuad*>     m_emitters;
    
    
    
public:
    void initView();
    void createQuestion();
    

    void correctAnswer();
    void correctShowObject();
    void correctAnimation();
    
    void removeWrongExamples();
    
    void cloudMoveAnimation();
    void showRainEffect(int num);
    void scheduleRainEffect(float dt);
    void unscheduleRainEffect();
    
    void objectShowAnimation();
    void scheduleShowObject(float dt);
    void unscheduleShowObject();

    void showEndingAnimation();
    void objectScaleAnimation();
    void wordAnimation();
    
    void interactionStart();
    void nextInteraction();
    
    void finishAnimation();
    
    void onExampleTabActivate(float dt);
};

#endif // __STEP2_TYPE1_H__
