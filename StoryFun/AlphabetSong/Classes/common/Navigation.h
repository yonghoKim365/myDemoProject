
#ifndef Navigation_h
#define Navigation_h


//#include "Navigation_Delegates.h"

#include "MGTNavigationController.h"

using namespace cocos2d;
using namespace MGTNavigation;

namespace navi
{
    typedef enum
    {
        eNavigationColor_None = 0,
        eNavigationColor_White,
        eNavigationColor_Yellow,
    } eNavigationColor;
    
    
    // Navigation button types
    typedef enum
    {
        eNavigationButton_Prev          = 0,
        eNavigationButton_Next,
        eNavigationButton_Guide,
        eNavigationButton_Exit,
    } eNavigationButton;
    
    typedef enum
    {
        eNavigationType_Step1           = 0,
        eNavigationType_Step2,
        eNavigationType_Step3,
    } eNavigationType;
}
typedef std::function<void(int)>                                          NavigationDelegate_t;

class Navigation : public MGTNavigationController
{
public:
    // Navigation class Creator & Destoryer
    Navigation();
    ~Navigation();


////////////////////////////////////////////////////////
//
// Class initalize functions.
//
////////////////////////////////////////////////////////
private:
    /**
     * @brief       content id varialbe.
     */
    std::string                                         _strResPrefix;
    
    float                                               _fTouchDelay;

    
    navi::eNavigationType                               _currentPage;
    navi::eNavigationColor                              _currentColorType;
    
private:

    void _setNavigationButtons();
    
public:

    void initWithButtonTypes(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, int type, ...);
    
    void initWithNavigationType(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType eNavigationType);
    
    void showInitBtn();
    
    void setTouchDelay(float delay){ _fTouchDelay = delay; };
    
    void setDimmedDownMenuEnabled(bool isEnabled);

    
    virtual void playButtonAction(int eNavigationButtonType);
    virtual void stopButtonAction(int eNavigationButtonType);
    
    void resumeNavigation();
    
public:
    NavigationDelegate_t                   m_navigationDelegate;
    
    /// @note do not forget to call setSequenceDelegate(nullptr) before deleting your subscriber
    void setNavigationDelegate(NavigationDelegate_t delegate);
    
//////////////////////////////////////////////////////////
////
//// Class operate functions.
////
//////////////////////////////////////////////////////////
public:
    // ---------- Implemente MGTNavigationController virtual functions.
    void onTouchedNavigationButton(Ref *sender);
    // Implemente MGTNavigationController virtual functions ----------.
    
    
//    // Navigation buttons callback functions.
//    virtual void onTouchedNavigationButtonAtPrev()          {}
//    virtual void onTouchedNavigationButtonAtNext()          {}
//    virtual void onTouchedNavigationButtonAtTrackiingOn()   {}
//    virtual void onTouchedNavigationButtonAtTrackiingOff()  {}
//    virtual void onTouchedNavigationButtonAtProgress()      {}
//    virtual void onTouchedNavigationButtonAtSoundOn()       {}
//    virtual void onTouchedNavigationButtonAtSoundOff()      {}
//    virtual void onTouchedNavigationButtonAtPause()         {}
//    virtual void onTouchedNavigationButtonAtPlay()          {}
//    virtual void onTouchedNavigationButtonAtReplay()        {}
//    virtual void onTouchedNavigationButtonAtClose()         {}
//    virtual void onTouchedNavigationButtonAtExit()          {}
    
};


#endif
