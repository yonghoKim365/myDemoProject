
#ifndef __Common_Intro_Top__
#define __Common_Intro_Top__


#include "cocos2d.h"
#include "MGTLayer.h"

#include "GAF.h"
#include "GAFTimelineAction.h"

NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;
USING_NS_CC;

namespace common_intro_top {
    typedef enum
    {
        SHOW          = 0,
        HIDE
    } eState;
    
    typedef enum
    {
        ELLO          = 0,
        FONG,
        RENJI,
        RALLA
    } eCharacterType;
}


class Common_Intro_Top : public MGTLayer
{
public:
    
    Common_Intro_Top();
    ~Common_Intro_Top();
    
    static Common_Intro_Top * create(common_intro_top::eCharacterType chaType, std::string textfile, std::string sndfile);

    virtual bool initWithSetting(common_intro_top::eCharacterType chaType, std::string textfile, std::string sndfile);
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Common_Intro_Top);
    
    
    virtual void onViewLoad();
    virtual void onViewLoaded();
    
    
public:
    std::function<void()>                   m_finishedDelegate;
    
    /// @note do not forget to call setSequenceDelegate(nullptr) before deleting your subscriber
    void setFinishedDelegate(std::function<void()> delegate);
    
    
public:
    cocos2d::EventListenerTouchOneByOne*            m_touchlistener;

    __Dictionary                                    m_psdDictionary;
    
    GAFAsset*                                       m_asset;
    GAFObject*                                      m_mainObject;
    
    common_intro_top::eState                        m_eState;
    common_intro_top::eCharacterType                m_eCharacterType;
    
    GAFObject*                                      m_character;
    
    std::string                                     m_sndFile;
    
    
    
    
public:
    void show();
    void showComplete();
    void textBoxScaleUpFinished();
    void textFadeInFinished();
    
    void hide();
    void hideComplete();
    
    
public:         //GAF function
    GAFObject* createGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position);
    bool hitTestGAFObject(GAFObject* obj, Touch* touch);
    
public:         //common function
    Vec2 getLocalPoint(Node* childnode);
    
    
    
public:         //delegate function
    void onSoundPlay(GAFObject * object, const std::string& soundName);
    
protected:         //gaf sound play override function
    virtual void onNarrationPlayForGAF(const std::string filename);
    virtual void onEffectPlayForGAF(const std::string filename);

protected:         //narration finished callback override function
    virtual void onNarrationFinishedCallback(std::string fileName);
    
};


#endif /* defined(__Common_Intro_Top__) */
