//
//  GameConst.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#ifndef __CapcomWorld__GameConst__
#define __CapcomWorld__GameConst__

#include <iostream>
#include "cocos2d.h"


#define FOC_IMAGE_SERV_URL          "http://devfoc.apdgames.com/"
#define FOC_GAME_SERV_URL          "https://devfoc.apdgames.com/foc/infinite?cmd=";

//#define FOC_IMAGE_SERV_URL          "http://foc.apdgames.com/"
//#define FOC_GAME_SERV_URL          "https://foc.apdgames.com/foc/infinite?cmd=";

class GameConst{
    
    public:
    
    
    
        GameConst(){
            subBtn_color_normal = cocos2d::ccc3(194,194,194);
            subBtn_color_selected = cocos2d::ccc3(255,192, 0);
            COLOR_WHITE  = cocos2d::ccc3(255,255,255);
            COLOR_YELLOW = cocos2d::ccc3(255,192, 0);
            COLOR_TUTORIAL = cocos2d::ccc3(200,200,200);
            COLOR_ORANGE = cocos2d::ccc3(255,128,0);
            COLOR_GRAY = cocos2d::ccc3(194,194,194);
            COLOR_BLUE = cocos2d::ccc3(0,132,255);
            COLOR_RED = cocos2d::ccc3(255,0,0);
            COLOR_BLACK = cocos2d::ccc3(0,0,0);
            COLOR_DARK_GRAY = cocos2d::ccc3(48,48,48);
    
            TutorialBooleanTable[0] = "bTutorialQuestBattle";
            TutorialBooleanTable[1] = "bTutorialBossBattle";
            TutorialBooleanTable[2] = "bTutorialFusion";
            TutorialBooleanTable[3] = "bTutorialTraining";
            TutorialBooleanTable[4] = "bTutorialFriend";
            TutorialBooleanTable[5] = "bTutorialBattle";
            TutorialBooleanTable[6] = "bTutorialRivalBattle";
            TutorialBooleanTable[7] = "bTutorialHiddenRivalBattle";
            
            
            //WIN_SIZE = cocos2d::CCSizeMake(640, 960);
        }
    
        cocos2d::ccColor3B subBtn_color_normal;
        cocos2d::ccColor3B subBtn_color_selected;
        cocos2d::ccColor3B COLOR_BLACK;
        cocos2d::ccColor3B COLOR_WHITE;
        cocos2d::ccColor3B COLOR_YELLOW;
        cocos2d::ccColor3B COLOR_TUTORIAL;
        cocos2d::ccColor3B COLOR_ORANGE;
        cocos2d::ccColor3B COLOR_GRAY;
        cocos2d::ccColor3B COLOR_BLUE;
        cocos2d::ccColor3B COLOR_RED;
        cocos2d::ccColor3B COLOR_DARK_GRAY;
        static cocos2d::CCSize WIN_SIZE;
    
    
    
        static const int CARD_TAB_SUB_0 = 0;   // my card
        static const int CARD_TAB_SUB_1 = 1;   // team
        static const int CARD_TAB_SUB_2 = 2;   // fusion
        static const int CARD_TAB_SUB_3 = 3;   // training
        static const int CARD_TAB_SUB_4 = 4;   // trade

        //static const int TOP_UI_HEIGHT  = 70;       // 화면 상단 UI (QUEST, Atk, def, coin.., 세로 480 기준)
        //static const int MAIN_BTNS_UI_HEIGHT  = 70; // main btn + card btn size (세로 480기준)
    
        static const int MAIN_LAYER_TOP_MARGIN = 23;
        static const int MAIN_LAYER_TOP_UI_HEIGHT = 116;//140; // mainScene 상단 UI 영역 height
        static const int MAIN_LAYER_BTN_HEIGHT = 73; // mainScene 하단 버튼 영역 height, Home, Cards, Battle, Quest, Shop 버튼 (세로 960 기준)

        static const int CARDS_LAYER_BTN_HEIGHT = 88; // DojoLayerCard 하단 버튼 영역 height (세로 960 기준), MyCard, Team, Fusion, Training, Trade 버튼들.
    
        //static const int CARD_DECK_TOP_UI_SPACE_2 = 54; // attack team, defence team 버튼 영역 height
        static const int CARD_DECK_TOP_UI_SPACE_2 = 0; // 공격팀, 방어팀 탭 삭제됨. 영역크기를 0으로 잡음
        static const int CARD_DECK_TOP_UI_SPACE_3 = 10; // 버튼 하단 공간
    
        static const int CARD_DECK_EDIT_LAYER_H = 331;
    
        static const int SOCIAL_INVITE_FRIEND_BAR_H = 54;
        static const int SOCIAL_HELP_TEXT_ZONE_H    = 110;
    
    
        static const int CARD_LIST_CELL_HEIGHT = 260;
    
        static const int CALL_CARDLIST_FROM_MYCARD  = 0;
        static const int CALL_CARDLIST_FROM_DECK    = 10;
        static const int CALL_CARDLIST_FROM_FUSION  = 20;
        static const int CALL_CARDLIST_FROM_TRAINING= 30;
        static const int CALL_CARDLIST_FROM_TRADE_1 = 40;
        static const int CALL_CARDLIST_FROM_TRADE_2 = 50;
        static const int CALL_CARDLIST_FROM_TRADE_REG = 60;
        //static const int CALL_CARDLIST_FROM_BATTLE  = 70; // option 버튼 없음
    
        //static const int CARDLIST_LAYER_BUTTON_ZONE_HEIGHT = 170;//150 // my card - 카드 리스트 상단 메뉴 버튼 2단 + 글씨 1단 height
    
        static const int CARDLIST_PREV_BTN_UPPER_SPACE = 10; 
        static const int CARDLIST_PREV_BTN_H = 50; // 카드 리스트의 이전버튼영역의 h, imageH 50 + space 10 = 60
    
        static const int CARDLIST_LAYER_TOP_SPACE = 20;
        static const int CARDLIST_LAYER_BUTTON_ZONE_UPPER_SPACE = 10;
        static const int CARDLIST_LAYER_BUTTON_ZONE_HEIGHT = 88;//130;//150 // my card - 카드 리스트 상단 메뉴 버튼 2단
        static const int CARDLIST_LAYER_BUTTON_ZONE_BOTTOM_SPACE = 10;
    
        static const int UI_NUM_OF_CARDS_LABEL_H = 30;  // 리스트 상단 - 보유한 카드 10/60 - UI의 height
        static const int UI_LIST_SPACE1_H = 10;         // 카드 리스트와 상단 UI사이의 공백
        static const int UI_LIST_SPACE2_H = 10;         // 카드 리스트와 하단 UI사이의 공백
        static const int UI_PAGE_NAVI_H = 40;           // 리스트 하단 좌우 페이지 버튼 영역의 height
        static const int UI_LIST_SPACE3_H = 10;         // 카드 리스트 제일 하단 여백
        static const int MAX_CELL_PER_PAGE = 8;         // 한 페이지당 cell 갯수
    
        static const int RIVAL_HISTORY_BACK_BTN_SPACE  = 50;
        static const int RIVAL_HISTORY_BACK_BTN_MARGIN = 10;
    
        enum CARD_DIRECTION
        {
            DIRECTION_NONE = 0,
            DIRECTION_NEWCARD,
            DIRECTION_CARDPACK_OPEN,
            
            DIRECTION_TOTAL,
        };
    
        enum TUTORIAL_STATE
        {
            TUTORIAL_SUMMARY_1 = 0,
            TUTORIAL_SUMMARY_2,
            TUTORIAL_SUMMARY_3,
            TUTORIAL_SUMMARY_4,
            
            TUTORIAL_GET_CARD_1,            //-- 4
            TUTORIAL_GET_CARD_2,
            
            TUTORIAL_GET_CARD_COMPLETE,     //-- 6
            
            TUTORIAL_CARD_DESCRIPTION_1,    //-- 7
            TUTORIAL_CARD_DESCRIPTION_2,
            TUTORIAL_CARD_DESCRIPTION_3,
            TUTORIAL_CARD_DESCRIPTION_4,
            TUTORIAL_CARD_DESCRIPTION_5,
            TUTORIAL_CARD_DESCRIPTION_6,
        
            TUTORIAL_CARD_MANAGEMENT_1,     //-- 13
            TUTORIAL_CARD_MANAGEMENT_2,
            TUTORIAL_CARD_MANAGEMENT_3,
            
            TUTORIAL_TEAM_SETTING_1,        //-- 16
            TUTORIAL_TEAM_SETTING_2,
            TUTORIAL_TEAM_SETTING_3,
            
            TUTORIAL_TEAM_SETTING_PREVIEW_1,    // -- 19
            TUTORIAL_TEAM_SETTING_PREVIEW_2,
            TUTORIAL_TEAM_SETTING_PREVIEW_3,
            TUTORIAL_TEAM_SETTING_PREVIEW_4,
            TUTORIAL_TEAM_SETTING_PREVIEW_5,
            TUTORIAL_TEAM_SETTING_PREVIEW_6,    // -- 24
            TUTORIAL_TEAM_SETTING_PREVIEW_7,
            TUTORIAL_TEAM_SETTING_PREVIEW_8,
            TUTORIAL_TEAM_SETTING_PREVIEW_9,
            
            TUTORIAL_QUEST_1, // -- 28
            TUTORIAL_QUEST_2,
            TUTORIAL_QUEST_3,
            TUTORIAL_QUEST_4,
            TUTORIAL_QUEST_5,
            TUTORIAL_QUEST_6,
            TUTORIAL_QUEST_7,
            
            TUTORIAL_TOTAL, // --  서버 저장은 여기까지 이후는 로컬에 상태값 저장
            
            TUTORIAL_FUSION_1,
            TUTORIAL_FUSION_2,
            TUTORIAL_FUSION_3,
            TUTORIAL_FUSION_4,
            TUTORIAL_FUSION_5, // -- 40
            
            TUTORIAL_TRAINING_1,
            TUTORIAL_TRAINING_2,
            TUTORIAL_TRAINING_3,
            TUTORIAL_TRAINING_4,
            TUTORIAL_TRAINING_5,
            TUTORIAL_TRAINING_6, // -- 46
            
            TUTORIAL_SPECIALATTACK_1,
            TUTORIAL_SPECIALATTACK_2,
            TUTORIAL_SPECIALATTACK_3, // --49
            
            TUTORIAL_FRIEND_1,
            TUTORIAL_FRIEND_2,
            TUTORIAL_FRIEND_3,
            TUTORIAL_FRIEND_4,
            TUTORIAL_FRIEND_5,
            TUTORIAL_FRIEND_6, // -- 55
            
            TUTORIAL_SPIN_1,
            TUTORIAL_SPIN_2,
            TUTORIAL_SPIN_3, // -- 58
            TUTORIAL_SPIN_4,
            TUTORIAL_SPIN_5,
            
            TUTORIAL_BATTLE_1,
            TUTORIAL_BATTLE_2,
            TUTORIAL_BATTLE_3,
            TUTORIAL_BATTLE_4,
            TUTORIAL_BATTLE_5,
            TUTORIAL_BATTLE_6, // -- 66
            
            TUTORIAL_HONOR_1,
            TUTORIAL_HONOR_2,
            TUTORIAL_HONOR_3,
            TUTORIAL_HONOR_4, // -- 70
            
            TUTORIAL_QUESTBATTLE_1,
            TUTORIAL_QUESTBATTLE_2,
//            TUTORIAL_QUESTBATTLE_3,     // -- 73
            
            TUTORIAL_BOSSBATTLE_1,
            TUTORIAL_BOSSBATTLE_2,
            TUTORIAL_BOSSBATTLE_3,      // -- 76-1
            
            TUTORIAL_RIVALBATTLE_1,
            TUTORIAL_RIVALBATTLE_2,
            TUTORIAL_RIVALBATTLE_3,
            TUTORIAL_RIVALBATTLE_4,
            TUTORIAL_RIVALBATTLE_5,
            TUTORIAL_RIVALBATTLE_6,
            TUTORIAL_RIVALBATTLE_7,     // -- 83-1
            
            TUTORIAL_RIVALHISTORY_1,
            TUTORIAL_RIVALHISTORY_2,    // -- 85-1
            
            TUTORIAL_HIDDENRIVAL_1,
            TUTORIAL_HIDDENRIVAL_2,
            TUTORIAL_HIDDENRIVAL_3,     // -- 88-1
        };

        enum TUTORIAL_BOOLEAN
        {
            TUTORIAL_QUESTBATTLE = 0,
            TUTORIAL_BOSSBATTLE,
            TUTORIAL_FUSION,
            TUTORIAL_TRAINING,
            TUTORIAL_FRIEND,
            TUTORIAL_BATTLE,
            TUTORIAL_RIVALBATTLE,
//            TUTORIAL_RIVALHISTORY,
            TUTORIAL_HIDDENRIVAL,
            
            TUTORIAL_BOOLEAN_TOTAL,     // 체크
        };
    
        std::string TutorialBooleanTable[TUTORIAL_BOOLEAN_TOTAL];
    
    
    static const int NUM_OF_MAX_CARD = 200; // 카드 최대 보유량
    
    
    ///////////////////////////////////////////////////////////// layer Z order
    static const int HOME_DOJO_LAYER_Z = 20;
    static const int HOME_CARD_LAYER_Z = 20;
    static const int HOME_BATTLE_LAYER_Z = 20;
    static const int HOME_QUEST_LAYER_Z = 20;
    static const int HOME_SHOP_LAYER_Z = 20;
    
    static const int LEVEL_UP_LAYER_Z = 10000;
    
    static const int TUTORIAL_BATTLE_LAYER_Z = 9000;

};



#endif /* defined(__CapcomWorld__GameConst__) */
