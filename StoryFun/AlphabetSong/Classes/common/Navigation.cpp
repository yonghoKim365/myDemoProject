
#include "Navigation.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "ProductManager.h"
#include "MGTMenu.h"
#include "MGTSoundManager.h"

// For IOS, android Height gap.
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#define OS_HEIGHT_GAP                                   24
#endif
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#define OS_HEIGHT_GAP                                   24
#endif


// For Navigation Positions.
#define POS_TOPLEFT                                     Vec2(28, naviWinSize.height-28)
#define POS_TOPRIGHT                                    Vec2(naviWinSize.width-28, naviWinSize.height-28)
#define POS_CENTERLEFT                                  Vec2(30, winHalfSize.height)
#define POS_CENTERRIGHT                                 Vec2(naviWinSize.width - 30, winHalfSize.height)
#define POS_BOTTOMLEFT                                  Vec2(49, 49 + 24 - OS_HEIGHT_GAP)
#define POS_BOTTOMRIGHT                                 Vec2(naviWinSize.width-28, 23)
#define POS_CENTER                                      Vec2(winHalfSize.width, winHalfSize.height)

//#define POS_TOPRIGHT_1                                  Vec2(naviWinSize.width - 60, naviWinSize.height- 60)
//#define POS_TOPRIGHT_2                                  Vec2(naviWinSize.width - 154, naviWinSize.height- 56)
//#define POS_TOPRIGHT_3                                  Vec2(naviWinSize.width - 254, naviWinSize.height- 56)




// For Navigation button menu depth.
#define nNavigation_Depth                               1000

////////////////////////////////////////////////////////
//
// Class initalize functions.
//
////////////////////////////////////////////////////////
Navigation::Navigation()
{
    _strResPrefix = ProductManager::getInstance()->getProductID();
    _fTouchDelay = 0.5;
}


Navigation::~Navigation()
{
    _strResPrefix.clear();
}



void Navigation::_setNavigationButtons()
{
    // Make navigation buttons.
    
    int totalNum = (int)btnNavigationIdx.size();
    
    for(int buttonIdx = 0; buttonIdx < totalNum; buttonIdx++)
    {
        // current navigation button index.
        int             navigationIdx       = (int)btnNavigationIdx[buttonIdx];
        // navigation button position.
        Vec2         menuPos             = Vec2::ZERO;
        // navigation button normal image name.
        std::string     strNormalImageName  = _strResPrefix;
        // navigation button select image name.
        std::string     strSelectImageName  = _strResPrefix;
        // navigation button disable image name.
        std::string     strDisableImageName = _strResPrefix;
        
        auto navigationBtn = btnNavigations[navigationIdx];
        switch(navigationIdx)
        {
            case navi::eNavigationButton_Next:
                menuPos = POS_BOTTOMRIGHT;
                
                if (_currentColorType == navi::eNavigationColor_None)
                {
                    navigationBtn->imgNormal    = MGTSprite::createWithCommonPath("common_btn_next_n.png");
                    navigationBtn->imgSelect    = MGTSprite::createWithCommonPath("common_btn_next_p.png");
                }
//                else if (_currentColorType == navi::eNavigationColor_White)
//                {
//                    navigationBtn->imgNormal    = MGTSprite::createWithCommonPath("common_btn_next_n.png");
//                    navigationBtn->imgSelect    = MGTSprite::createWithCommonPath("common_btn_next_p.png");
//                }
//                else if (_currentColorType == navi::eNavigationColor_Yellow)
//                {
//                    navigationBtn->imgNormal    = MGTSprite::createWithCommonPath("common_btn_next_n.png");
//                    navigationBtn->imgSelect    = MGTSprite::createWithCommonPath("common_btn_next_p.png");
//                }
                
                menuPos = Vec2(menuPos.x - navigationBtn->imgNormal->getContentSize().width/2, menuPos.y + navigationBtn->imgNormal->getContentSize().height/2);
                break;
                
            case navi::eNavigationButton_Guide:
                menuPos = POS_TOPRIGHT;
                
                navigationBtn->imgNormal    = MGTSprite::createWithCommonPath("common_btn_guide_n.png");
                navigationBtn->imgSelect    = MGTSprite::createWithCommonPath("common_btn_guide_p.png");
                
                menuPos = Vec2(menuPos.x - navigationBtn->imgNormal->getContentSize().width/2, menuPos.y - navigationBtn->imgNormal->getContentSize().height/2);
                break;
                
            case navi::eNavigationButton_Exit:
                menuPos = POS_TOPLEFT;
                
                navigationBtn->imgNormal    = MGTSprite::createWithCommonPath("common_btn_exit_n.png");
                navigationBtn->imgSelect    = MGTSprite::createWithCommonPath("common_btn_exit_p.png");
                
                menuPos = Vec2(menuPos.x + navigationBtn->imgNormal->getContentSize().width/2, menuPos.y - navigationBtn->imgNormal->getContentSize().height/2);
                break;

            default:
                break;
        }
        
        // Make Menu item
        
        navigationBtn->menuItemSprite = MenuItemSprite::create(navigationBtn->imgNormal,
                                                               navigationBtn->imgSelect,
                                                               nullptr,
                                                               CC_CALLBACK_1(Navigation::onTouchedNavigationButton, this)
                                                               );

        
        // add navigation tag.
        navigationBtn->menuItemSprite->setTag(navigationIdx);
        

        navigationBtn->menu = Menu::createWithItem(btnNavigations[navigationIdx]->menuItemSprite);
        navigationBtn->menu->setPosition(menuPos);
        this->setVisibleButton(false, navigationIdx);
        

        
        if (navigationIdx == navi::eNavigationButton_Prev || navigationIdx == navi::eNavigationButton_Next || navigationIdx == navi::eNavigationButton_Guide)
        {
            // Make CCMenu.
//            navigationBtn->menu = Menu::createWithItem(btnNavigations[navigationIdx]->menuItemSprite);
//            navigationBtn->menu->setPosition(menuPos);
//            this->setVisibleButton(false, navigationIdx);
            
            parentNode->addChild(btnNavigations[navigationIdx]->menu, 30);
        }
        else
        {
            // Make MGTMenu.
//            navigationBtn->menu = MGTMenu::createWithItem(btnNavigations[navigationIdx]->menuItemSprite);
//            navigationBtn->menu->setPosition(menuPos);
//            this->setVisibleButton(false, navigationIdx);
            
            parentNode->addChild(btnNavigations[navigationIdx]->menu, 80);
        }
    }
}

void Navigation::setNavigationDelegate(NavigationDelegate_t delegate)
{
    m_navigationDelegate = delegate;
}

void Navigation::initWithButtonTypes(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, int type, ...)
{
    va_list args;
    va_start(args, type);
    while(type != -1)
    {
        btnNavigationIdx.push_back(type);
        
        type = va_arg(args, int);
        if(type == -1)
        {
            break ;
        }
    }
    va_end(args);
    
    MGTNavigationController::init(pTargetNode);
    this->_setNavigationButtons();
}

void Navigation::initWithNavigationType(cocos2d::Node *pTargetNode, navi::eNavigationColor eNavigationColor, navi::eNavigationType type)
{
    _currentPage = type;
    _currentColorType = eNavigationColor;
    
    if(_currentPage == navi::eNavigationType_Step1)
    {
        this->initWithButtonTypes(pTargetNode,
                                  eNavigationColor,
                                  navi::eNavigationButton_Exit,
                                  navi::eNavigationButton_Guide,
                                  navi::eNavigationButton_Next,
                                  -1);
    }
    else if(_currentPage == navi::eNavigationType_Step2)
    {
        this->initWithButtonTypes(pTargetNode,
                                  eNavigationColor,
                                  navi::eNavigationButton_Exit,
                                  navi::eNavigationButton_Guide,
                                  navi::eNavigationButton_Next,
                                  -1);
    }
    else if(_currentPage == navi::eNavigationType_Step3)
    {
        this->initWithButtonTypes(pTargetNode,
                                  eNavigationColor,
                                  navi::eNavigationButton_Exit,
                                  navi::eNavigationButton_Guide,
                                  navi::eNavigationButton_Next,
                                  -1);
    }
}

void Navigation::showInitBtn()
{
    auto type = _currentPage;
    
    if(type == navi::eNavigationType_Step1)
    {
        for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
        {
            auto buttonType = btnNavigationIdx[buttonIdx];
            
            if (buttonType == navi::eNavigationButton_Exit )
            {
                this->setVisibleButton(true, buttonType);
            }
            else
            {
                this->setVisibleButton(false, buttonType);
            }
        }
    }
    else if(type == navi::eNavigationType_Step2)
    {
        for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
        {
            auto buttonType = btnNavigationIdx[buttonIdx];
            
            if (buttonType == navi::eNavigationButton_Exit )
            {
                this->setVisibleButton(true, buttonType);
            }
            else
            {
                this->setVisibleButton(false, buttonType);
            }
        }
    }
    else if(type == navi::eNavigationType_Step3)
    {
        for(int buttonIdx=0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
        {
            auto buttonType = btnNavigationIdx[buttonIdx];
            
            if (buttonType == navi::eNavigationButton_Exit )
            {
                this->setVisibleButton(true, buttonType);
            }
            else
            {
                this->setVisibleButton(false, buttonType);
            }
        }
    }
}

void Navigation::setDimmedDownMenuEnabled(bool isEnabled)
{
//    setEnableButton(isEnabled, navi::eNavigationButton_Prev);
//    setEnableButton(isEnabled, navi::eNavigationButton_Next);
//    setEnableButton(isEnabled, navi::eNavigationButton_TrackingOff);
//    setEnableButton(isEnabled, navi::eNavigationButton_TrackingOn);
//    setEnableButton(isEnabled, navi::eNavigationButton_Progress);
}


void Navigation::playButtonAction(int eNavigationButtonType)
{
    
//    auto seq_scale = Sequence::create(ScaleTo::create(0.2f, 1.1f),
//                                      ScaleTo::create(0.2f, 1.0f),
//                                      nullptr);
    
    auto seq_fade = Sequence::create(FadeTo::create(0.5f, 255.0f*0.3f),
                                     FadeTo::create(0.5f, 255.0f),
                                     nullptr);
    
    auto spawn = Spawn::create(seq_fade, nullptr);
    
    auto repeat = RepeatForever::create(spawn);
    
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        auto image = btnNavigations[eNavigationButtonType]->menuItemImage;
        image->runAction(repeat);
    }
    else if(NULL != btnNavigations[eNavigationButtonType]->menuItemSprite)
    {
        auto sp = btnNavigations[eNavigationButtonType]->menuItemSprite;
        sp->runAction(repeat);
    }
}

void Navigation::stopButtonAction(int eNavigationButtonType)
{
    if(NULL != btnNavigations[eNavigationButtonType]->menuItemImage)
    {
        auto image = btnNavigations[eNavigationButtonType]->menuItemImage;
        image->stopAllActions();
        image->setScale(1.0f);
        image->setOpacity(255.0f);
    }
    else if(NULL != btnNavigations[eNavigationButtonType]->menuItemSprite)
    {
        auto sp = btnNavigations[eNavigationButtonType]->menuItemSprite;
        sp->stopAllActions();
        sp->setScale(1.0f);
        sp->setOpacity(255.0f);
    }
}


void Navigation::resumeNavigation()
{
    for(int buttonIdx = 0; buttonIdx < btnNavigationIdx.size(); buttonIdx++)
    {
        int navigationIdx = (int)btnNavigationIdx[buttonIdx];
        auto navigationBtn = btnNavigations[navigationIdx];
        MGTUtils::getInstance()->resumeAllAnimations(navigationBtn->menu);
    }
}



////////////////////////////////////////////////////////
//
// Class oeprate functions.
//
////////////////////////////////////////////////////////
void Navigation::onTouchedNavigationButton(Ref *sender)
{
//    this->setEnableNavigation(false);
//    this->setEnableNavigation(true, _fTouchDelay);
    
    Node* pNode = (Node*)sender;
    m_navigationDelegate(pNode->getTag());
    
    
    MGTSoundManager::getInstance()->playEffect(ProductManager::getInstance()->getCommonFilePath("snd", "common_sfx_btn_01.mp3"));
    
}