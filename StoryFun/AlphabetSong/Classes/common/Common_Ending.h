
#ifndef __Common_Ending__
#define __Common_Ending__


#include "cocos2d.h"
#include "MGTLayer.h"
#include "DimmedLayer.h"


USING_NS_CC;

namespace common_ending {
    typedef enum
    {
        SHOW          = 0,
        HIDE
    } eState;
    
    typedef enum
    {
        STEP1          = 0,
        STEP2,
        STEP3
    } eEndingType;
}


class Common_Ending : public MGTLayer
{
public:
    
    Common_Ending();
    ~Common_Ending();
    
    static Common_Ending * create(common_ending::eEndingType type, bool hasDimmed = false);
    
    virtual bool initWithSetting(common_ending::eEndingType type, bool hasDimmed);
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Common_Ending);
    
    
    virtual bool onTouchBegan(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchMoved(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchEnded(cocos2d::Touch *touch, cocos2d::Event *event) override;
    
    virtual void onViewLoad() override;
    virtual void onViewLoaded() override;
    
    
public:
    cocos2d::EventListenerTouchOneByOne*            m_touchlistener;

    __Dictionary                                    m_psdDictionary;

    
    common_ending::eState                           m_eState;
    common_ending::eEndingType                      m_eEndingType;
    
    bool                                            m_bTouchEnabled;
    bool                                            m_hasDimmed;
    DimmedLayer*                                    m_dimmedLayer;
    
    Sprite*                                         m_content;
    std::string                                     m_sndFile;
    
public:
    void show();
    void showComplete();
    void hide();
    void hideComplete();
    

protected:         //narration finished callback override function
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
    
    
public:
    std::function<void()>                   m_touchedDelegate;
    
    /// @note do not forget to call setSequenceDelegate(nullptr) before deleting your subscriber
    void setTouchedDelegate(std::function<void()> delegate);
};


#endif /* defined(__Common_Ending__) */
