//
//  Tutorial.h
//  CapcomWorld
//
//  Created by APD-MBA11 on 13. 2. 20..
//
//

#ifndef __CapcomWorld__Tutorial__
#define __CapcomWorld__Tutorial__

#include <iostream>
#include "PopUp.h"

class NewTutorialPopUp : public BasePopUP, GameConst
{
public:
    NewTutorialPopUp(bool _IsTutorial);
    ~NewTutorialPopUp();
    
    virtual void InitUI(void *data);
    
    void InitFrameUI(const char* title, int btn_type);
    void InitSubTitleUI(const char* title);
    void InitTalkUI(const char* text, int talk_type);
    
    void Tutorial_Summary_001();     // -- tutorial summary
    void Tutorial_Summary_002();     // -- tutorial summary
    void Tutorial_Summary_003();     // -- tutorial summary
    void Tutorial_Summary_004();     // -- tutorial summary
    
    void Tutorial_GetCard_001();     // -- get card
    void Tutorial_GetCard_002();     // -- get card
    
    void Tutorial_GetCardComplete_001();     // -- get card
    
    void Tutorial_CardDescription_001();     // -- card description
    void Tutorial_CardDescription_002();     // -- card description
    void Tutorial_CardDescription_003();     // -- card description
    void Tutorial_CardDescription_004();     // -- card description
    void Tutorial_CardDescription_005();     // -- card description
    void Tutorial_CardDescription_006();     // -- card description
    
    void Tutorial_CardManagement_001();     // -- card management
    void Tutorial_CardManagement_002();     // -- card management
    void Tutorial_CardManagement_003();     // -- card management
    
    void Tutorial_TeamSetting_001();     // -- team setting
    void Tutorial_TeamSetting_002();     // -- team setting
    void Tutorial_TeamSetting_003();     // -- team setting
    
    void Tutorial_TeamSetting_Preview_001(); // -- team setting
    void Tutorial_TeamSetting_Preview_002(); // -- team setting
    void Tutorial_TeamSetting_Preview_003();
    void Tutorial_TeamSetting_Preview_004();
    void Tutorial_TeamSetting_Preview_005();
    void Tutorial_TeamSetting_Preview_006();
    void Tutorial_TeamSetting_Preview_007();
    void Tutorial_TeamSetting_Preview_008();
    void Tutorial_TeamSetting_Preview_009();

    void Tutorial_Quest_001();     // -- quest
    void Tutorial_Quest_002();     // -- quest
    void Tutorial_Quest_003();     // -- quest
    void Tutorial_Quest_004();     // -- quest
    void Tutorial_Quest_005();     // -- quest
    void Tutorial_Quest_006();     // -- quest
    void Tutorial_Quest_007();     // -- quest
    
    void Tutorial_Fusion_001();     // -- fusion
    void Tutorial_Fusion_002();     // -- fusion
    void Tutorial_Fusion_003();     // -- fusion
    void Tutorial_Fusion_004();     // -- fusion
    void Tutorial_Fusion_005();     // -- fusion
    
    void Tutorial_Training_001();     // -- training
    void Tutorial_Training_002();     // -- training
    void Tutorial_Training_003();     // -- training
    void Tutorial_Training_004();     // -- training
    void Tutorial_Training_005();     // -- training
    void Tutorial_Training_006();     // -- training

    void Tutorial_SpecialAttack_001();     // -- specialattack
    void Tutorial_SpecialAttack_002();     // -- specialattack
    void Tutorial_SpecialAttack_003();     // -- specialattack

    void Tutorial_Friend_001();     // -- friend
    void Tutorial_Friend_002();     // -- friend
    void Tutorial_Friend_003();     // -- friend
    void Tutorial_Friend_004();     // -- friend
    void Tutorial_Friend_005();     // -- friend
    void Tutorial_Friend_006();     // -- friend
    
    void Tutorial_Spin_001();
    void Tutorial_Spin_002();
    void Tutorial_Spin_003();
    void Tutorial_Spin_004();
    void Tutorial_Spin_005();
    
    void Tutorial_Battle_001();     // -- battle
    void Tutorial_Battle_002();     // -- battle
    void Tutorial_Battle_003();     // -- battle
    void Tutorial_Battle_004();     // -- battle
    void Tutorial_Battle_005();     // -- battle
    void Tutorial_Battle_006();     // -- battle
    
    void Tutorial_Honor_001();     // -- honor
    void Tutorial_Honor_002();     // -- honor
    void Tutorial_Honor_003();     // -- honor
    void Tutorial_Honor_004();     // -- honor

    void Tutorial_QuestBattle_001();     // -- QuestBattle
    void Tutorial_QuestBattle_002();     // -- QuestBattle
//    void Tutorial_QuestBattle_003();     // -- QuestBattle
    
    void Tutorial_BossBattle_001();     // -- BossBattle
    void Tutorial_BossBattle_002();     // -- BossBattle
    void Tutorial_BossBattle_003();     // -- BossBattle
    
    void Tutorial_RivalBattle_001();    // -- RivalBattle
    void Tutorial_RivalBattle_002();    // -- RivalBattle
    void Tutorial_RivalBattle_003();    // -- RivalBattle
    void Tutorial_RivalBattle_004();    // -- RivalBattle
    void Tutorial_RivalBattle_005();    // -- RivalBattle
    void Tutorial_RivalBattle_006();    // -- RivalBattle
    void Tutorial_RivalBattle_007();    // -- RivalBattle
    
    void Tutorial_RivalHistory_001();   // -- RivalHistory
    void Tutorial_RivalHistory_002();   // -- RivalHistory
    
    void Tutorial_HiddenRival_001();    // -- HiddenRival
    void Tutorial_HiddenRival_002();    // -- HiddenRival
    void Tutorial_HiddenRival_003();    // -- HiddenRival

    void CardPackOpen();
    void SetTouchEnable();
    
    void Restore();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
private:
    
    vector<string> ImageName;
    
    bool IsTutorialMode;
    
    bool IsTouchEnable;
    
    enum POPUP_BTN_TYPE
    {
        BTN_NEXT = 0,
        BTN_CLOSE,
        BTN_CARDPACK_OPEN,
        
        BTN_TOTAL,
    };
    
    enum TALK_TYPE
    {
        TALK_BIG = 0,
        TALK_SMALL,
    };
};

#endif /* defined(__CapcomWorld__Tutorial__) */
