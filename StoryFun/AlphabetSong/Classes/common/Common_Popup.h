
#ifndef __Common_Popup__
#define __Common_Popup__


#include "cocos2d.h"
#include "MGTLayer.h"
#include "DimmedLayer.h"

USING_NS_CC;

namespace common_popup {
    typedef enum
    {
        SHOW          = 0,
        HIDE
    } eState;
    
    typedef enum
    {
        ALL =0,
        EXIT,
        DRAWING_SAVE,
        DRAWING_DELETE,
        DRAWING_SAVING,
        DRAWING_SAVE_COMPLETE
    } ePopupType;
}


class Common_Popup : public MGTLayer
{
public:
    
    Common_Popup();
    ~Common_Popup();
    
    static Common_Popup * create(common_popup::ePopupType type, bool hasDimmed = false);
    
    virtual bool initWithSetting(common_popup::ePopupType type, bool hasDimmed);
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Common_Popup);
    
    
    virtual bool onTouchBegan(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchMoved(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchEnded(cocos2d::Touch *touch, cocos2d::Event *event) override;
    
    virtual void onViewLoad() override;
    virtual void onViewLoaded() override;
    
    
public:
    cocos2d::EventListenerTouchOneByOne*            m_touchlistener;

    __Dictionary                                    m_psdDictionary;
    
    common_popup::eState                            m_eState;
    common_popup::ePopupType                        m_ePopupType;
    
    bool                                            m_bTouchEnabled;
    bool                                            m_hasDimmed;
    DimmedLayer*                                    m_dimmedLayer;
    
    Sprite*                                         m_content;
    std::string                                     m_sndFile;
    
    
    int                                             m_count;
    bool                                            m_isSaveComplete;
public:
    common_popup::ePopupType getType();
    
public:
    void show(common_popup::ePopupType type, float duration = 0.0f);
    void showComplete();
    void hide(float duration = 0.0f);
    void hideComplete();
    
    
    void alertMenuCallback(Ref* sender);
    

protected:         //narration finished callback override function
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
    
    
public:
    std::function<void(bool)>                   m_Delegate;
    
    /// @note do not forget to call setSequenceDelegate(nullptr) before deleting your subscriber
    void setDelegate(std::function<void(bool)> delegate);
    
    void saveComplete();
};


#endif /* defined(__Common_Popup__) */
