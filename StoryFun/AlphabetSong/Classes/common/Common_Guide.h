
#ifndef __Common_Guide__
#define __Common_Guide__


#include "cocos2d.h"
#include "MGTLayer.h"
#include "DimmedLayer.h"

USING_NS_CC;

namespace common_guide {
    typedef enum
    {
        SHOW          = 0,
        HIDE
    } eState;
    
    typedef enum
    {
        GUIDE          = 0,
    } eGuideType;
}


class Common_Guide : public MGTLayer
{
public:
    
    Common_Guide();
    ~Common_Guide();
    
    static Common_Guide * create(common_guide::eGuideType type, bool hasDimmed = false);
    
    virtual bool initWithSetting(common_guide::eGuideType type, bool hasDimmed);
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Common_Guide);
    
    
    virtual bool onTouchBegan(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchMoved(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchEnded(cocos2d::Touch *touch, cocos2d::Event *event) override;
    
    virtual void onViewLoad() override;
    virtual void onViewLoaded() override;
    
    
public:
    cocos2d::EventListenerTouchOneByOne*            m_touchlistener;

    __Dictionary                                    m_psdDictionary;

    
    common_guide::eState                            m_eState;
    common_guide::eGuideType                        m_eGuideType;
    
    bool                                            m_bTouchEnabled;
    bool                                            m_hasDimmed;
    DimmedLayer*                                    m_dimmedLayer;
    
    Sprite*                                         m_content;
    std::string                                     m_sndFile;
    
public:
    void show(float duration = 0.0f);
    void showComplete();
    void hide(float duration = 0.0f);
    void hideComplete();
    
    
    void guideMenuCallback(Ref* sender);
    

protected:         //narration finished callback override function
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
    
    
public:
    std::function<void()>                   m_Delegate;
    
    /// @note do not forget to call setSequenceDelegate(nullptr) before deleting your subscriber
    void setDelegate(std::function<void()> delegate);
};


#endif /* defined(__Common_Guide__) */
