//
//  Tutorial.cpp
//  CapcomWorld
//
//  Created by APD-MBA11 on 13. 2. 20..
//
//

#include "Tutorial.h"
#include "TraceLayer.h"

NewTutorialPopUp::NewTutorialPopUp(bool _IsTutorial)
{
    IsTouchEnable = false;
    
    ImageName.push_back("ui/tutorial/tutorial_01_overview01.png"); // -- 0
    ImageName.push_back("ui/tutorial/tutorial_01_overview02.png");
    ImageName.push_back("ui/tutorial/tutorial_01_overview03.png");
    ImageName.push_back("ui/tutorial/tutorial_01_overview04.png");
    ImageName.push_back("ui/tutorial/tutorial_02_gain01.png");
    ImageName.push_back("ui/tutorial/tutorial_02_gain02.png");
    ImageName.push_back("ui/tutorial/tutorial_03_gainok01.png");
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default01.png");
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default02.png");
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default03.png");
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default04.png"); // -- 10
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default05.png");
    ImageName.push_back("ui/tutorial/tutorial_04_carde_default06.png");
    ImageName.push_back("ui/tutorial/tutorial_05_card_manage01.png");
    ImageName.push_back("ui/tutorial/tutorial_05_card_manage02.png");
    ImageName.push_back("ui/tutorial/tutorial_05_card_manage03.png");
    ImageName.push_back("ui/tutorial/tutorial_06_team01.png");
    ImageName.push_back("ui/tutorial/tutorial_06_team02.png");
    ImageName.push_back("ui/tutorial/tutorial_06_team03.png");
    ImageName.push_back("ui/tutorial/tutorial_07_preview01.png");
    ImageName.push_back("");                                            // -- 20
    ImageName.push_back("ui/tutorial/tutorial_07_preview02.png");
    ImageName.push_back("");
    ImageName.push_back("ui/tutorial/tutorial_07_preview03.png");       // -- 23
    ImageName.push_back("");
    ImageName.push_back("");
    ImageName.push_back("ui/tutorial/tutorial_07_preview04.png");       // -- 26
    ImageName.push_back("ui/tutorial/tutorial_07_preview05.png");       // -- 27
    ImageName.push_back("");
    ImageName.push_back("ui/tutorial/tutorial_08_quest01.png");
    ImageName.push_back("ui/tutorial/tutorial_08_quest02.png");
    ImageName.push_back("ui/tutorial/tutorial_08_quest03.png");
    ImageName.push_back("ui/tutorial/tutorial_08_quest04.png");
    ImageName.push_back("ui/tutorial/tutorial_08_quest05.png");
    ImageName.push_back("ui/tutorial/tutorial_08_quest06.png");
    
    IsTutorialMode = _IsTutorial;
    
    MainScene::getInstance()->setEnableMainMenu(false);
    DojoLayerDojo::getInstance()->setEnableSubMenu(false);
    UserStatLayer::getInstance()->setEnableMenu(false);
    DojoLayerDojo::getInstance()->setTouchEnabled(false);
    
    //setDisableWithRunningScene();
}

NewTutorialPopUp::~NewTutorialPopUp()
{
    MainScene::getInstance()->setEnableMainMenu(true);
    DojoLayerDojo::getInstance()->setEnableSubMenu(true);
    UserStatLayer::getInstance()->setEnableMenu(true);
    DojoLayerDojo::getInstance()->setTouchEnabled(true);
    
    this->removeAllChildrenWithCleanup(true);
}

void NewTutorialPopUp::Restore()
{
    MainScene::getInstance()->removeChildByTag(98765, true);
    restoreTouchDisable();
    
    MainScene::getInstance()->setEnableMainMenu(true);
    DojoLayerDojo::getInstance()->setEnableSubMenu(true);
    UserStatLayer::getInstance()->setEnableMenu(true);
    DojoLayerDojo::getInstance()->setTouchEnabled(true);
}

void NewTutorialPopUp::InitUI(void *data)
{
    const int CurrentState = *((int*)data);
    
    tutorialProgress = CurrentState;
    
    switch (CurrentState)
    {
        case TUTORIAL_SUMMARY_1: Tutorial_Summary_001(); break;
        case TUTORIAL_SUMMARY_2: Tutorial_Summary_002(); break;
        case TUTORIAL_SUMMARY_3: Tutorial_Summary_003(); break;
        case TUTORIAL_SUMMARY_4: Tutorial_Summary_004(); break;
            
        case TUTORIAL_GET_CARD_1: Tutorial_GetCard_001(); break;
        case TUTORIAL_GET_CARD_2: Tutorial_GetCard_002(); break;
            
        case TUTORIAL_GET_CARD_COMPLETE: Tutorial_GetCardComplete_001(); break;
            
        case TUTORIAL_CARD_DESCRIPTION_1: Tutorial_CardDescription_001(); break;
        case TUTORIAL_CARD_DESCRIPTION_2: Tutorial_CardDescription_002(); break;
        case TUTORIAL_CARD_DESCRIPTION_3: Tutorial_CardDescription_003(); break;
        case TUTORIAL_CARD_DESCRIPTION_4: Tutorial_CardDescription_004(); break;
        case TUTORIAL_CARD_DESCRIPTION_5: Tutorial_CardDescription_005(); break;
        case TUTORIAL_CARD_DESCRIPTION_6: Tutorial_CardDescription_006(); break;
            
        case TUTORIAL_CARD_MANAGEMENT_1: Tutorial_CardManagement_001(); break;
        case TUTORIAL_CARD_MANAGEMENT_2: Tutorial_CardManagement_002(); break;
        case TUTORIAL_CARD_MANAGEMENT_3: Tutorial_CardManagement_003(); break;
            
        case TUTORIAL_TEAM_SETTING_1: Tutorial_TeamSetting_001(); break;
        case TUTORIAL_TEAM_SETTING_2: Tutorial_TeamSetting_002(); break;
        case TUTORIAL_TEAM_SETTING_3: Tutorial_TeamSetting_003(); break;
            
        case TUTORIAL_TEAM_SETTING_PREVIEW_1: Tutorial_TeamSetting_Preview_001(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_2: Tutorial_TeamSetting_Preview_002(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_3: Tutorial_TeamSetting_Preview_003(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_4: Tutorial_TeamSetting_Preview_004(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_5: Tutorial_TeamSetting_Preview_005(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_6: Tutorial_TeamSetting_Preview_006(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_7: Tutorial_TeamSetting_Preview_007(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_8: Tutorial_TeamSetting_Preview_008(); break;
        case TUTORIAL_TEAM_SETTING_PREVIEW_9: Tutorial_TeamSetting_Preview_009(); break;
            
        case TUTORIAL_QUEST_1: Tutorial_Quest_001(); break;
        case TUTORIAL_QUEST_2: Tutorial_Quest_002(); break;
        case TUTORIAL_QUEST_3: Tutorial_Quest_003(); break;
        case TUTORIAL_QUEST_4: Tutorial_Quest_004(); break;
        case TUTORIAL_QUEST_5: Tutorial_Quest_005(); break;
        case TUTORIAL_QUEST_6: Tutorial_Quest_006(); break;
        case TUTORIAL_QUEST_7: Tutorial_Quest_007(); break;
        
        case TUTORIAL_FUSION_1: Tutorial_Fusion_001(); break;
        case TUTORIAL_FUSION_2: Tutorial_Fusion_002(); break;
        case TUTORIAL_FUSION_3: Tutorial_Fusion_003(); break;
        case TUTORIAL_FUSION_4: Tutorial_Fusion_004(); break;
        case TUTORIAL_FUSION_5: Tutorial_Fusion_005(); break;
            
        case TUTORIAL_TRAINING_1: Tutorial_Training_001(); break;
        case TUTORIAL_TRAINING_2: Tutorial_Training_002(); break;
        case TUTORIAL_TRAINING_3: Tutorial_Training_003(); break;
        case TUTORIAL_TRAINING_4: Tutorial_Training_004(); break;
        case TUTORIAL_TRAINING_5: Tutorial_Training_005(); break;
        case TUTORIAL_TRAINING_6: Tutorial_Training_006(); break;
        
        case TUTORIAL_SPECIALATTACK_1: Tutorial_SpecialAttack_001(); break;
        case TUTORIAL_SPECIALATTACK_2: Tutorial_SpecialAttack_002(); break;
        case TUTORIAL_SPECIALATTACK_3: Tutorial_SpecialAttack_003(); break;

        case TUTORIAL_FRIEND_1: Tutorial_Friend_001(); break;
        case TUTORIAL_FRIEND_2: Tutorial_Friend_002(); break;
        case TUTORIAL_FRIEND_3: Tutorial_Friend_003(); break;
        case TUTORIAL_FRIEND_4: Tutorial_Friend_004(); break;
        case TUTORIAL_FRIEND_5: Tutorial_Friend_005(); break;
        case TUTORIAL_FRIEND_6: Tutorial_Friend_006(); break;
        
        case TUTORIAL_SPIN_1: Tutorial_Spin_001(); break;
        case TUTORIAL_SPIN_2: Tutorial_Spin_002(); break;
        case TUTORIAL_SPIN_3: Tutorial_Spin_003(); break;
        case TUTORIAL_SPIN_4: Tutorial_Spin_004(); break;
        case TUTORIAL_SPIN_5: Tutorial_Spin_005(); break;

        case TUTORIAL_BATTLE_1: Tutorial_Battle_001(); break;
        case TUTORIAL_BATTLE_2: Tutorial_Battle_002(); break;
        case TUTORIAL_BATTLE_3: Tutorial_Battle_003(); break;
        case TUTORIAL_BATTLE_4: Tutorial_Battle_004(); break;
        case TUTORIAL_BATTLE_5: Tutorial_Battle_005(); break;
        case TUTORIAL_BATTLE_6: Tutorial_Battle_006(); break;
            
        case TUTORIAL_HONOR_1: Tutorial_Honor_001(); break;
        case TUTORIAL_HONOR_2: Tutorial_Honor_002(); break;
        case TUTORIAL_HONOR_3: Tutorial_Honor_003(); break;
        case TUTORIAL_HONOR_4: Tutorial_Honor_004(); break;
            
        case TUTORIAL_QUESTBATTLE_1: Tutorial_QuestBattle_001(); break;
        case TUTORIAL_QUESTBATTLE_2: Tutorial_QuestBattle_002(); break;
//        case TUTORIAL_QUESTBATTLE_3: Tutorial_QuestBattle_003(); break;
            
        case TUTORIAL_BOSSBATTLE_1: Tutorial_BossBattle_001(); break;
        case TUTORIAL_BOSSBATTLE_2: Tutorial_BossBattle_002(); break;
        case TUTORIAL_BOSSBATTLE_3: Tutorial_BossBattle_003(); break;
        
        case TUTORIAL_RIVALBATTLE_1: Tutorial_RivalBattle_001(); break;
        case TUTORIAL_RIVALBATTLE_2: Tutorial_RivalBattle_002(); break;
        case TUTORIAL_RIVALBATTLE_3: Tutorial_RivalBattle_003(); break;
        case TUTORIAL_RIVALBATTLE_4: Tutorial_RivalBattle_004(); break;
        case TUTORIAL_RIVALBATTLE_5: Tutorial_RivalBattle_005(); break;
        case TUTORIAL_RIVALBATTLE_6: Tutorial_RivalBattle_006(); break;
        case TUTORIAL_RIVALBATTLE_7: Tutorial_RivalBattle_007(); break;
            
        case TUTORIAL_RIVALHISTORY_1: Tutorial_RivalHistory_001(); break;
        case TUTORIAL_RIVALHISTORY_2: Tutorial_RivalHistory_002(); break;
            
        case TUTORIAL_HIDDENRIVAL_1: Tutorial_HiddenRival_001(); break;
        case TUTORIAL_HIDDENRIVAL_2: Tutorial_HiddenRival_002(); break;
        case TUTORIAL_HIDDENRIVAL_3: Tutorial_HiddenRival_003(); break;
    }
}

void NewTutorialPopUp::InitFrameUI(const char* title, int btn_type)
{
    CCSprite* sprFrameBg = CCSprite::create("ui/tutorial/tutorial_frame_bg.png");
    sprFrameBg->setAnchorPoint(ccp(0.0f, 0.0f));
    sprFrameBg->setPosition(accp(21.0f, 92.0f));
    this->addChild(sprFrameBg, 0);
    
    CCSprite* sprFrame = CCSprite::create("ui/tutorial/tutorial_frame_stroke.png");
    sprFrame->setAnchorPoint(ccp(0.0f, 0.0f));
    sprFrame->setPosition(accp(0.0f, 0.0f));
    this->addChild(sprFrame, 10);
    
    CCLabelTTF* Title = CCLabelTTF::create(title, "HelveticaNeue-Bold", 14);
    Title->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(320.0f, 915.0f), Title, 160);
    
    CCSprite* Btn = CCSprite::create("ui/tutorial/tutorial_btn_a1.png");
    Btn->setAnchorPoint(ccp(0.0f, 0.0f));
    Btn->setPosition(accp(167.0f, 28.0f));
    this->addChild(Btn, 10);

    CCLabelTTF* Label = NULL;
    
    if(BTN_NEXT == btn_type)
    {
        Btn->setTag(BTN_NEXT);
        Label = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_NextPage_Button"), "HelveticaNeue-Bold", 12);
    }
    else if (BTN_CLOSE == btn_type)
    {
        Btn->setTag(BTN_CLOSE);
        Label = CCLabelTTF::create(LocalizationManager::getInstance()->get("close_btn"), "HelveticaNeue-Bold", 12);
    }
    else if (BTN_CARDPACK_OPEN == btn_type)
    {
        Btn->setTag(BTN_CARDPACK_OPEN);
        Label = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_CardPack_Open"), "HelveticaNeue-Bold", 12);
    }
    
    Label->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.5f), accp(320.0f, 55.0f), Label, 160);
}

void NewTutorialPopUp::InitTalkUI(const char* text, int talk_type)
{
    CCSprite* sprTalkBox = NULL;
    
    if(TALK_BIG == talk_type)
    {
        sprTalkBox= CCSprite::create("ui/tutorial/tutorial_talk_big.png");
        sprTalkBox->setAnchorPoint(ccp(0.0f, 0.0f));
        sprTalkBox->setPosition(accp(40.0f, 122.0f));
        this->addChild(sprTalkBox, 10);
        
        CCLabelTTF* Talk = CCLabelTTF::create(text, "HelveticaNeue-Bold", 12);
        Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
        Talk->setColor(COLOR_WHITE);
        registerLabel(this, ccp(0.0f, 0.8f), accp(62.0f, 222.0f), Talk, 160);

    }
    else if (TALK_SMALL == talk_type)
    {
        sprTalkBox= CCSprite::create("ui/tutorial/tutorial_talk_small.png");
        sprTalkBox->setAnchorPoint(ccp(0.0f, 0.0f));
        sprTalkBox->setPosition(accp(40.0f, 152.0f));
        this->addChild(sprTalkBox, 10);
        
        CCLabelTTF* Talk = CCLabelTTF::create(text, "HelveticaNeue-Bold", 12);
        Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
        Talk->setColor(COLOR_WHITE);
        registerLabel(this, ccp(0.0f, 0.9f), accp(62.0f, 303.0f), Talk, 160);
    }
}

void NewTutorialPopUp::InitSubTitleUI(const char* title)
{
    CCSprite* sprFrameTitle = CCSprite::create("ui/tutorial/tutorial_frame_title.png");
    sprFrameTitle->setAnchorPoint(ccp(0.0f, 0.0f));
    sprFrameTitle->setPosition(accp(119.0f, 703.0f));
    this->addChild(sprFrameTitle, 10);
    
    CCLabelTTF* SubTitle = CCLabelTTF::create(title, "HelveticaNeue-Bold", 15);
    SubTitle->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.5f), accp(320.0f, 753.0f), SubTitle, 160);
}

void NewTutorialPopUp::Tutorial_Summary_001()
{
    CCLOG("Tutorial_Summary_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Summary_Title") ;
    string title2 = " 1/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Summary_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Summary_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Summary_002()
{
    CCLOG("Tutorial_Summary_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Summary_Title") ;
    string title2 = " 2/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Summary_002"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Summary_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Summary_003()
{
    CCLOG("Tutorial_Summary_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Summary_Title") ;
    string title2 = " 3/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Summary_003"), TALK_BIG);
    
    CCSprite* sprCharacterBG = CCSprite::create("ui/tutorial/tutorial_overview01.png");
    sprCharacterBG->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacterBG->setPosition(accp(21.0f, 280.0f));
    this->addChild(sprCharacterBG, 3);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Summary_004()
{
    CCLOG("Tutorial_Summary_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Summary_Title") ;
    string title2 = " 4/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Summary_004"), TALK_BIG);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCharacterBG = CCSprite::create("ui/tutorial/tutorial_overview01.png");
    sprCharacterBG->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacterBG->setPosition(accp(21.0f, 280.0f));
    this->addChild(sprCharacterBG, 3);
}


void NewTutorialPopUp::Tutorial_GetCard_001()
{
    CCLOG("Tutorial_GetCard_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_GetCard_Title");
    string title2 = " 1/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_GetCard_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_GetCard_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_GetCard_002()
{
    CCLOG("Tutorial_GetCard_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_GetCard_Title");
    string title2 = " 2/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_CARDPACK_OPEN);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_GetCard_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_gain01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_GetCardComplete_001()
{
    string title1 = LocalizationManager::getInstance()->get("Tutorial_GetCardComplete_Title");
    string title2 = " 1/1";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_GetCardComplete_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_gainok01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardDescription_001()
{
    CCLOG("Tutorial_CardDescription_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 1/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);

    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-22.0f, 12.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_CardDescription_002()
{
    CCLOG("Tutorial_CardDescription_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 2/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_002"), TALK_SMALL);

    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_card01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardDescription_003()
{
    CCLOG("Tutorial_CardDescription_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 3/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_card02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardDescription_004()
{
    CCLOG("Tutorial_CardDescription_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 4/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_card03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardDescription_005()
{
    CCLOG("Tutorial_CardDescription_05");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 5/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_card04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardDescription_006()
{
    CCLOG("Tutorial_CardDescription_05");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardDescription_Title");
    string title2 = " 6/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardDescription_006"), TALK_SMALL);

    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_morrigan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_gainok01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardManagement_001()
{
    CCLOG("Tutorial_CardManagement_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardManagement_Title");
    string title2 = " 1/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardManagement_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_CardManagement_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_vega.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(23.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_CardManagement_002()
{
    CCLOG("Tutorial_CardManagement_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardManagement_Title");
    string title2 = " 2/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardManagement_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_vega.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_manage01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_CardManagement_003()
{
    CCLOG("Tutorial_CardManagement_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_CardManagement_Title");
    string title2 = " 3/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_CardManagement_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_vega.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(163.0f, -165.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_manage02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_TeamSetting_001()
{
    CCLOG("Tutorial_TeamSetting_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Title");
    string title2 = " 1/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_cammy.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-57.0f, 15.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_TeamSetting_002()
{
    CCLOG("Tutorial_TeamSetting_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Title");
    string title2 = " 2/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);

    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_cammy.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(169.0f, -152.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_team01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_TeamSetting_003()
{
    CCLOG("Tutorial_TeamSetting_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Title");
    string title2 = " 3/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_cammy.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(169.0f, -152.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_team02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Quest_001()
{
    CCLOG("Tutorial_Quest_001");
}

void NewTutorialPopUp::Tutorial_Quest_002()
{
    CCLOG("Tutorial_Quest_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 1/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);

    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_002"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Quest_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-13.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Quest_003()
{
    CCLOG("Tutorial_Quest_003");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 2/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_003"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Quest_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-13.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Quest_004()
{
    CCLOG("Tutorial_Quest_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 3/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_004"), TALK_SMALL);

    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(168.0f, -133.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_quest01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Quest_005()
{
    CCLOG("Tutorial_Quest_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 4/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(168.0f, -133.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_quest01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Quest_006()
{
    CCLOG("Tutorial_Quest_006");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 5/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_006"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(168.0f, -133.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_quest02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Quest_007()
{
    CCLOG("Tutorial_Quest_007");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Quest_Title");
    string title2 = " 6/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Quest_007"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(168.0f, -133.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_quest03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_001()
{
    CCLOG("Tutorial_TeamSetting_Preview_001");
    
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setOpacity(0);
    BG->setAnchorPoint(ccp(0.0f, 0.0f));
    BG->setPosition(accp(0.0f, 0.0f));
    BG->setTag(BTN_NEXT);
    this->addChild(BG);
    
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_07_preview_bin.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Talk = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Preview_001"), "HelveticaNeue-Bold", 12);
    Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
    Talk->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.8f), accp(41.0f, 150.0f), Talk, 160);
    
    AttackDeckCell::getInstance()->setTouchEnabled(false);
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_002()
{
    CCLOG("Tutorial_TeamSetting_Preview_002");
    addClickIcon(AttackDeckCell::getInstance(), 170.0f, 10.0f);
    AttackDeckCell::getInstance()->setTouchEnabled(true);
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_003()
{
    CCLOG("Tutorial_TeamSetting_Preview_003");
    
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setOpacity(0);
    BG->setAnchorPoint(ccp(0.0f, 0.0f));
    BG->setPosition(accp(0.0f, 0.0f));
    BG->setTag(BTN_NEXT);
    this->addChild(BG);
    
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_07_preview_bin.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Talk = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Preview_003"), "HelveticaNeue-Bold", 12);
    Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
    Talk->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.8f), accp(41.0f, 150.0f), Talk, 160);
    
    ACardTableView::getInstance()->setTouchEnabled(false);
}


void NewTutorialPopUp::Tutorial_TeamSetting_Preview_004()
{
    CCLOG("Tutorial_TeamSetting_Preview_004");
    
    ACardTableView::getInstance()->DrawTutorialIcon();
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_005()
{
    CCLOG("Tutorial_TeamSetting_Preview_005");
    
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setOpacity(0);
    BG->setAnchorPoint(ccp(0.0f, 0.0f));
    BG->setPosition(accp(0.0f, 0.0f));
    BG->setTag(BTN_NEXT);
    this->addChild(BG);
    
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_07_preview_bin.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Talk = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Preview_005"), "HelveticaNeue-Bold", 12);
    Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
    Talk->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.8f), accp(41.0f, 150.0f), Talk, 160);}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_006()
{
    CCLOG("Tutorial_TeamSetting_Preview_006");
    ACardTableView::getInstance()->DrawTutorialIcon();
    TeamEditLayer::getInstance()->setTouchEnabled(true);
    //++tutorialProgress;
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_007()
{
    CCLOG("Tutorial_TeamSetting_Preview_007");
    
    addClickIcon(MainScene::getInstance() , 220.0f, 20.0f);
    
    CCMenu* menu = (CCMenu*)TeamEditLayer::getInstance()->getChildByTag(99);
    menu->setEnabled(true);
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_008()
{
    CCLOG("Tutorial_TeamSetting_Preview_008");
    
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setOpacity(0);
    BG->setAnchorPoint(ccp(0.0f, 0.0f));
    BG->setPosition(accp(0.0f, 0.0f));
    BG->setTag(BTN_NEXT);
    this->addChild(BG);
    
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_07_preview_bin.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Talk = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Preview_008"), "HelveticaNeue-Bold", 12);
    Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
    Talk->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.8f), accp(41.0f, 150.0f), Talk, 160);
}

void NewTutorialPopUp::Tutorial_TeamSetting_Preview_009()
{
    CCLOG("Tutorial_TeamSetting_Preview_009");
    
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setOpacity(0);
    BG->setAnchorPoint(ccp(0.0f, 0.0f));
    BG->setPosition(accp(0.0f, 0.0f));
    BG->setTag(BTN_CLOSE);
    this->addChild(BG);
    
    CCSprite* popupBG = CCSprite::create("ui/tutorial/tutorial_07_preview_bin.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    popupBG->setPosition(accp(0.0f, 0.0f));
    this->addChild(popupBG);
    
    CCLabelTTF* Talk = CCLabelTTF::create(LocalizationManager::getInstance()->get("Tutorial_TeamSetting_Preview_009"), "HelveticaNeue-Bold", 12);
    Talk->setHorizontalAlignment(kCCTextAlignmentLeft);
    Talk->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.0f, 0.8f), accp(41.0f, 150.0f), Talk, 160);}

void NewTutorialPopUp::Tutorial_Fusion_001()
{
    CCLOG("Tutorial_Fusion_001");
    
    setDisableWithRunningScene();

    string title1 = LocalizationManager::getInstance()->get("Tutorial_Fusion_Title");
    string title2 = " 1/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_lilith.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-55.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Fusion_002()
{
    CCLOG("Tutorial_Fusion_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Fusion_Title");
    string title2 = " 2/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_002"), TALK_SMALL);

    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_lilith.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(124.0f, -186.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fusion01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Fusion_003()
{
    CCLOG("Tutorial_Fusion_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Fusion_Title");
    string title2 = " 3/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_lilith.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(124.0f, -186.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fusion02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Fusion_004()
{
    CCLOG("Tutorial_Fusion_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Fusion_Title");
    string title2 = " 4/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_lilith.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(124.0f, -186.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fusion02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Fusion_005()
{
    CCLOG("Tutorial_Fusion_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Fusion_Title");
    string title2 = " 5/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Fusion_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/vs_lilith.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(124.0f, -186.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fusion03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Training_001()
{
    CCLOG("Tutorial_Training_001");
    
    setDisableWithRunningScene();
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 1/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Training_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-43.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Training_002()
{
    CCLOG("Tutorial_Training_002");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 2/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_train01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Training_003()
{
    CCLOG("Tutorial_Training_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 3/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_train01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Training_004()
{
    CCLOG("Tutorial_Training_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 4/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_train02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Training_005()
{
    CCLOG("Tutorial_Training_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 5/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_train02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Training_006()
{
    CCLOG("Tutorial_Training_006");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Training_Title");
    string title2 = " 6/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Training_006"), TALK_SMALL);

    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_train03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_SpecialAttack_001()
{
    CCLOG("Tutorial_SpecialAttack_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_Title");
    string title2 = " 1/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_skill01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_SpecialAttack_002()
{
    CCLOG("Tutorial_SpecialAttack_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_Title");
    string title2 = " 2/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_skill02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_SpecialAttack_003()
{
    CCLOG("Tutorial_SpecialAttack_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_Title");
    string title2 = " 3/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_SpecialAttack_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_dan.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_skill02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Friend_001()
{
    CCLOG("Tutorial_Friend_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 1/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Friend_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-57.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Friend_002()
{
    CCLOG("Tutorial_Friend_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 2/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_002"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Friend_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-57.0f, 14.0f));
    this->addChild(sprCharacter, 5);    
}

void NewTutorialPopUp::Tutorial_Friend_003()
{
    CCLOG("Tutorial_Friend_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 3/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Friend_004()
{
    CCLOG("Tutorial_Friend_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 4/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_friend02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Friend_005()
{
    CCLOG("Tutorial_Friend_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 5/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_friend03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 280.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Friend_006()
{
    CCLOG("Tutorial_Friend_006");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Friend_Title");
    string title2 = " 6/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Friend_006"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_friend03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 280.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Spin_001()
{
    CCLOG("Tutorial_Spin_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Spin_Title");
    string title2 = " 1/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Spin_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Spin_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.3f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-57.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Spin_002()
{
    CCLOG("Tutorial_Spin_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Spin_Title");
    string title2 = " 2/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Spin_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_coin01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Spin_003()
{
    CCLOG("Tutorial_Spin_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Spin_Title");
    string title2 = " 3/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Spin_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_coin02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Spin_004()
{
    CCLOG("Tutorial_Spin_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Spin_Title");
    string title2 = " 4/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Spin_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_coin03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);    
}

void NewTutorialPopUp::Tutorial_Spin_005()
{
    CCLOG("Tutorial_Spin_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Spin_Title");
    string title2 = " 5/5";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Spin_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_sakura.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(160.0f, -148.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_coin04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);    
}

void NewTutorialPopUp::Tutorial_Battle_001()
{
    CCLOG("Tutorial_Battle_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 1/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Battle_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-48.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Battle_002()
{
    CCLOG("Tutorial_Battle_002");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 2/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_002"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Battle_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-48.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Battle_003()
{
    CCLOG("Tutorial_Battle_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 3/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Battle_004()
{
    CCLOG("Tutorial_Battle_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 4/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle05.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Battle_005()
{
    CCLOG("Tutorial_Battle_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 5/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle06.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);    
}

void NewTutorialPopUp::Tutorial_Battle_006()
{
    CCLOG("Tutorial_Battle_006");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Battle_Title");
    string title2 = " 6/6";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Battle_006"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Honor_001()
{
    CCLOG("Tutorial_Honor_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Honor_Title");
    string title2 = " 1/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Honor_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_Honor_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-48.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_Honor_002()
{
    CCLOG("Tutorial_Honor_002");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_Honor_Title");
    string title2 = " 2/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Honor_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fame01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Honor_003()
{
    CCLOG("Tutorial_Honor_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Honor_Title");
    string title2 = " 3/4";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Honor_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fame02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_Honor_004()
{
    CCLOG("Tutorial_Honor_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_Honor_Title");
    string title2 = " 4/4";
    InitFrameUI(title1.append(title2).c_str(),  BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_Honor_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/ff_poison.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_fame02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_QuestBattle_001()
{
    CCLOG("Tutorial_QuestBattle_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_QuestBattle_Title");
    string title2 = " 1/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_QuestBattle_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
    
    CCMenu* normalEnemyFightMenu = (CCMenu* )TraceNormalEnemyLayer::getInstance()->getChildByTag(400);
    normalEnemyFightMenu->setEnabled(false);
}

void NewTutorialPopUp::Tutorial_QuestBattle_002()
{
    CCLOG("Tutorial_QuestBattle_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_QuestBattle_Title");
    string title2 = " 2/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);//BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_QuestBattle_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}
/*
void NewTutorialPopUp::Tutorial_QuestBattle_003()
{
    CCLOG("Tutorial_QuestBattle_003");
    
    InitFrameUI("퀘스트 배틀 3/3", BTN_CLOSE);
    
    InitTalkUI("게이지가 KO에 닿을 때까지 화면을\n연타하면 이깁니다.\n\n단, 제한 시간이 있어서 순발력이\n필요하지요.", TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_battle03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}
*/
void NewTutorialPopUp::Tutorial_BossBattle_001()
{
    CCLOG("Tutorial_BossBattle_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_BossBattle_Title");
    string title2 = " 1/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_BossBattle_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_bossbattle01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
    
//    CCMenu* bossFightMenu = (CCMenu* )TraceBossLayer::getInstance()->getChildByTag(400);
//    bossFightMenu->setEnabled(false);
}

void NewTutorialPopUp::Tutorial_BossBattle_002()
{
    CCLOG("Tutorial_BossBattle_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_BossBattle_Title");
    string title2 = " 2/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_BossBattle_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_team02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_BossBattle_003()
{
    CCLOG("Tutorial_BossBattle_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_BossBattle_Title");
    string title2 = " 3/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_BossBattle_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_chunli.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_bossbattle01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_001()
{
    CCLOG("Tutorial_RivalBattle_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 1/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_001"), TALK_BIG);
    
    InitSubTitleUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_SubTitle"));
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.25f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(-48.0f, 14.0f));
    this->addChild(sprCharacter, 5);
}

void NewTutorialPopUp::Tutorial_RivalBattle_002()
{
    CCLOG("Tutorial_RivalBattle_002");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 2/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_003()
{
    CCLOG("Tutorial_RivalBattle_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 3/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_004()
{
    CCLOG("Tutorial_RivalBattle_004");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 4/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_004"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_005()
{
    CCLOG("Tutorial_RivalBattle_005");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 5/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_005"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval03.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_006()
{
    CCLOG("Tutorial_RivalBattle_006");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 6/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_006"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_raval04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalBattle_007()
{
    CCLOG("Tutorial_RivalBattle_007");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalBattle_Title");
    string title2 = " 7/7";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalBattle_007"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_friend01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalHistory_001()
{
    CCLOG("Tutorial_RivalHistory_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalHistory_Title");
    string title2 = " 1/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalHistory_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_history01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_RivalHistory_002()
{
    CCLOG("Tutorial_RivalHistory_002");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_RivalHistory_Title");
    string title2 = " 2/2";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_RivalHistory_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_history02.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_HiddenRival_001()
{
    CCLOG("Tutorial_HiddenRival_001");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_HiddenRival_Title");
    string title2 = " 1/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_HiddenRival_001"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_hidden01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_HiddenRival_002()
{
    CCLOG("Tutorial_HiddenRival_002");

    string title1 = LocalizationManager::getInstance()->get("Tutorial_HiddenRival_Title");
    string title2 = " 2/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_NEXT);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_HiddenRival_002"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_hidden01.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::Tutorial_HiddenRival_003()
{
    CCLOG("Tutorial_HiddenRival_003");
    
    string title1 = LocalizationManager::getInstance()->get("Tutorial_HiddenRival_Title");
    string title2 = " 3/3";
    InitFrameUI(title1.append(title2).c_str(), BTN_CLOSE);
    
    InitTalkUI(LocalizationManager::getInstance()->get("Tutorial_HiddenRival_003"), TALK_SMALL);
    
    CCSprite* sprCharacter = CCSprite::create("ui/cha/sf_ryu.png");
    sprCharacter->setScale(1.2f);
    sprCharacter->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCharacter->setPosition(accp(134.0f, -185.0f));
    this->addChild(sprCharacter, 5);
    
    CCSprite* sprCard = CCSprite::create("ui/tutorial/tutorial_history04.png");
    sprCard->setAnchorPoint(ccp(0.0f, 0.0f));
    sprCard->setPosition(accp(21.0f, 350.0f));
    this->addChild(sprCard, 10);
}

void NewTutorialPopUp::CardPackOpen()
{
    PlayerInfo* info = PlayerInfo::getInstance();
    
    bool IsTutorialMode = true;
    class CardPackOpen* cardOpen = new class CardPackOpen(this->getContentSize(), info->myCards, IsTutorialMode);
    cardOpen->setAnchorPoint(ccp(0.0f, 0.0f));
    cardOpen->setPosition(accp(0.0f, 0.0f));
    cardOpen->setTag(94556);
    MainScene::getInstance()->addChild(cardOpen, 9000);
}

void NewTutorialPopUp::SetTouchEnable()
{
    IsTouchEnable = true;
    setDisableWithRunningScene();
}

void NewTutorialPopUp::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void NewTutorialPopUp::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    printf("tutorial touch\n");
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, BTN_NEXT, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        
        if(IsTutorialMode == true)
        {
            ++tutorialProgress;
            //PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
            InitUI(&tutorialProgress);
        }
        else
        {
            ++tutorialProgress;
            InitUI(&tutorialProgress);
        }
    }
    else if(GetSpriteTouchCheckByTag(this, BTN_CLOSE, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        
        if(TUTORIAL_FUSION_5 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_FUSION, true);
        }
        
        if(TUTORIAL_TRAINING_6 == tutorialProgress)
        {
            Restore();
            
            // --  필살기 설명으로 이어짐.
            const bool TutorialMode = false;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            tutorialProgress = TUTORIAL_SPECIALATTACK_1;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }

        if(TUTORIAL_SPECIALATTACK_3 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_TRAINING, true);
        }
        
        if(TUTORIAL_FRIEND_6 == tutorialProgress)
        {
            Restore();
            
            // -- 코인 설명으로 이어짐
            const bool TutorialMode = false;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            tutorialProgress = TUTORIAL_SPIN_1;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }
        
        if(TUTORIAL_SPIN_5 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_FRIEND, true);
        }
        
        if(TUTORIAL_BATTLE_6 == tutorialProgress)
        {
            Restore();
            
            // -- 명성샵 설명으로 이어짐
            const bool TutorialMode = false;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            tutorialProgress = TUTORIAL_HONOR_1;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }

        if(TUTORIAL_HONOR_4 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_BATTLE, true);
        }
        
        if(TUTORIAL_QUESTBATTLE_2 == tutorialProgress)//TUTORIAL_QUESTBATTLE_3 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_QUESTBATTLE, true);
            
            CCMenu* normalEnemyFightMenu = (CCMenu* )TraceNormalEnemyLayer::getInstance()->getChildByTag(400);
            normalEnemyFightMenu->setEnabled(true);
        }
        
        if(TUTORIAL_BOSSBATTLE_3 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_BOSSBATTLE, true);
            
            TraceBossLayer::getInstance()->actionBoss();
//            CCMenu* bossFightMenu = (CCMenu* )TraceBossLayer::getInstance()->getChildByTag(400);
//            bossFightMenu->setEnabled(true);
        }
        
        if(TUTORIAL_RIVALBATTLE_7 == tutorialProgress)
        {
            Restore();
            
            // -- 라이벌 이력으로 이어짐
            const bool TutorialMode = false;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            tutorialProgress = TUTORIAL_RIVALHISTORY_1;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }
        
        if(TUTORIAL_RIVALHISTORY_2 == tutorialProgress)
        {
            Restore();
            
            // -- 히든 라이벌로 이어짐
            const bool TutorialMode = false;
            NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
            tutorialProgress = TUTORIAL_HIDDENRIVAL_1;
            basePopUp->InitUI(&tutorialProgress);
            basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
            basePopUp->setPosition(accp(0.0f, 0.0f));
            basePopUp->setTag(98765);
            MainScene::getInstance()->addChild(basePopUp, 9000);
        }
        
        if(TUTORIAL_HIDDENRIVAL_3 == tutorialProgress)
        {
            Restore();
            PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_RIVALBATTLE, true);
            
            if(false == PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_HIDDENRIVAL)) {
                TraceRivalLayer::getInstance()->actionRival1();
            } else {
                TraceHiddenRivalLayer::getInstance()->actionHiddenRival1();
            }
        }

        if(tutorialProgress == TUTORIAL_TOTAL - 1)
        {
            MainScene::getInstance()->setEnableMainMenu(true);
            DojoLayerDojo::getInstance()->setEnableSubMenu(true);
            UserStatLayer::getInstance()->setEnableMenu(true);
            DojoLayerDojo::getInstance()->setTouchEnabled(true);
            DojoLayerCard::getInstance()->setEnableMainMenu(true);
            
            PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
            
            this->removeFromParentAndCleanup(true);
        }
        
        if(TUTORIAL_TEAM_SETTING_PREVIEW_9 == tutorialProgress)
        {
            addClickIcon(MainScene::getInstance(), 490.0f, 15.0f);
            
            MainScene::getInstance()->setEnableMainMenu(true);
            
            CCMenu* pMenu = (CCMenu*)MainScene::getInstance()->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(3);
            item1->selected();
            
            MainScene::getInstance()->SetNormalSubBtns();
            MainScene::getInstance()->SetSelectedSubBtn(3);
        }
        
        if(TUTORIAL_CARD_DESCRIPTION_6 == tutorialProgress)
        {
            addClickIcon(MainScene::getInstance(), 260.0f, 15.0f);
            
            MainScene::getInstance()->setEnableMainMenu(true);
            
            CCMenu* pMenu = (CCMenu*)MainScene::getInstance()->getChildByTag(99);
            for (int i=0;i<5;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(1);
            item1->selected();
            
            MainScene::getInstance()->SetNormalSubBtns();
            MainScene::getInstance()->SetSelectedSubBtn(1);
        }
        
        if(TUTORIAL_CARD_MANAGEMENT_3 == tutorialProgress)
        {
            addClickIcon(MainScene::getInstance(), 300.0f, 100.0f);
            
            CCMenu* pMenu = (CCMenu*)MainScene::getInstance()->getChildByTag(699);
            pMenu->setEnabled(true);
            
            for (int i=11;i<15;i++)
            {
                CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(i);
                item1->unselected();
            }
            
            CCMenuItemImage *item1 = (CCMenuItemImage *)pMenu->getChildByTag(12);
            item1->selected();
            
            DojoLayerCard::getInstance()->SetNormalSubBtns();
            DojoLayerCard::getInstance()->SetSelectedSubBtn(12);
        }
        
        if(IsTutorialMode == true
           && TUTORIAL_CARD_DESCRIPTION_6 != tutorialProgress
           && TUTORIAL_CARD_MANAGEMENT_3 != tutorialProgress)
        {
            //PlayerInfo::getInstance()->setTutorialProgress(tutorialProgress);
            tutorialProgress++;
            
            InitUI(&tutorialProgress);
        }
    }
    else if(GetSpriteTouchCheckByTag(this, BTN_CARDPACK_OPEN, localPoint))
    {
        soundButton1();
        this->removeAllChildrenWithCleanup(true);
        
        //tutorialProgress = TUTORIAL_CARD_DESCRIPTION_1;
        //InitUI(&tutorialProgress);
        
        CardPackOpen();
    }
}

void NewTutorialPopUp::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

