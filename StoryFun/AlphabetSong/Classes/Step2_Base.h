#ifndef __STEP2_BASE_H__
#define __STEP2_BASE_H__

#include "cocos2d.h"
#include "Base_Layer.h"

#include "Navigation.h"
#include "Common_Popup.h"
#include "Common_Guide.h"
#include "DeviceUtilManager.h"

enum
{
    kTagPlayContainer = 0,
    kTagEndContainer,
    kTagGuide,
    kTagTitle = 50,
};

namespace step2 {
    typedef enum
    {
        STATE_TITLE          = 0,
        STATE_GUIDE,
        STATE_PLAY,
        STATE_PLAY_COMPLETE,
        STATE_END,
        STATE_EXIT
    } eState;
}

USING_NS_CC;

class Step2_Base : public Base_Layer
{
public:
    
    
    Step2_Base();
    ~Step2_Base();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step2_Base);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override;
    virtual void onTouchCancelled(Touch *touch, Event *event) override;
    
public:
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName);
    virtual void onTexturePreLoad(std::string& path);
    virtual void onFramePlayed(GAFObject* object, uint32_t frame);
    
    void showTitle();
    void removeTitle();
    void guideStart();
    void playGuideAnimation();
    void removeGuideAnimation();
    void createEnding();
    
    void onPause();
    void onResume();

    virtual void playStart();
    virtual void playComplete();
    
    void nextStep();
    
public:
    std::string             m_strBgmSound;
    std::string             m_strTitleSound;
    std::string             m_strGuideSound;
    std::string             m_strWordSound;
    std::string             m_strPhonicsSound;
    std::string             m_strSentenceSound;
    
    float                   m_sleepBeganTime;
    float                   m_touchBeganTime;
    
    GAFObject*              m_gafGuide;
    
    Touch                   *m_touch;
    bool                    m_isTouchMoved;
    bool                    m_isTouchObject;
    
    Sprite*                 m_title;
    Sprite*                 m_playContainer;
    Sprite*                 m_endContainer;
    
    step2::eState           m_eState;
    
    int                     m_orientation;
    
    std::vector<std::vector<int>*>       m_exampleSets;
    

public:
    Navigation*                                 m_navigation;
    
    
    void createNavigation(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType type);
    void onTouchedNavigation(int buttonType);
    
    virtual void onTouchedNavigationButtonAtNext();
    virtual void onTouchedNavigationButtonAtGuide();
    virtual void onTouchedNavigationButtonAtExit();
    
    
public:
    DimmedLayer*                                m_dimmedLayer;
    
    void createDimmedLayer();

public:
    Common_Popup*                               m_commonPopup;
    
    void createCommonPopup(common_popup::ePopupType type, bool hasDimmed = true);
    void onTouchedPopupButton(bool result);
    
    virtual void onTouchedPopupButtonAtNo();
    virtual void onTouchedPopupButtonAtYes();
    
public:
    Common_Guide*                               m_commonGuide;
    
    void createCommonGuide(common_guide::eGuideType type, bool hasDimmed = true);
    void showCommonGuide();
    void hideCommonGuide();
    void onTouchedGuideButton();
    
    virtual void onTouchedGuideButtonAtClose();
    
    
    
public:
    void scheduleTimer(float dt);
    void startAffordanceTimer();
    void stopAffordanceTimer();
    
    void playAffordance(Sprite* sp);
    void stopAffordance(Sprite* sp, bool isVisible = false);
    
    virtual void showAffordance();
    virtual void hideAffordance();

public:
    void playTitleSound();
    void playGuideSound();
    void playWordSound();
    void playPhonicsSound(int num);
    
public:
    void gotoNextStep();
    void gotoExit();
};

#endif // __STEP2_BASE_H__
