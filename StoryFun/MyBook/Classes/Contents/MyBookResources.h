#ifndef __MYBOOK_RESOURCES_H__
#define __MYBOOK_RESOURCES_H__

#pragma once
#include "cocos2d.h"
#include "Cocos2dxGAFPlayer-release-gaf-5/Library/Sources/GAF.h"

USING_NS_CC;

// 배경 스케일
#define  BACKGROUND_SCALE  (1920.f/1280.f)

// 패키지 네임
#define PACKAGE_NAME  "net.minigate.smartdoodle.storyfun.viewer.mybook"

// 노티피케이션 설정
#define NOTI_STR_VOICE_RECORD  "noti_voice_record"
#define NOTI_CODE_VOICE_RECORD_CLOSE  1 // 보이스 레이어 닫기
#define NOTI_CODE_VOICE_RECORD_HELP   2 // 보이스 도움말

#define NOTI_STR_PLAY  "noti_play"
#define NOTI_CODE_PLAY_CLOSE         3 // 재생하기 레이어 닫기
#define NOTI_CODE_PLAY_ENDING_CANCEL 4 // 재생하기 엔딩 팝업에서 호출

#define SCREEN_LOCK_TIME (5.0f) // (2.0f * 60.0f)

//-----------------------------------------------
// ENUM
//-----------------------------------------------
// 파레트 색상 구분자
namespace TOOL 
{
	// 12 color, 1 random
	// ARPICOT, ORANGE, PINK, RED, YELLOW, GREEN, BLUE, NAVY, VIOLET, BROWN, GREY, BLACK
	enum COLOR { ARPICOT = 0, ORANGE, PINK, RED, YELLOW, GREEN, 
				 BLUE, NAVY, VIOLET, BROWN, GREY, BLACK, MAX_COLOR };
}

// 스티커 구분자
namespace STICKER
{
	// 총 15종 -> 총 14종
	// BALL, BALLOON, BEAR, BUS, CANDY, CLOUD, FLOWER,
	// GRASS, HEART, LEAF, PLANE, RIBBON, STAR, TREE
	enum ITEM { BALL = 0, BALLOON, BEAR, BUS,   CANDY,  CLOUD, FLOWER, 
				GRASS,    HEART,   LEAF, PLANE, RIBBON, STAR,  TREE, MAX_STICKER };
}

// 그리기 모드 정의
// START : 시작, 
// DRAW : 그리기, 
// STICKER_ON : 스티커 선택 가능 상태, 
// STICKER_SET  : 스티커 선택 상태, 
// STICKER_EDIT : 스티커 편집 상태,
// DONE : 완료, 
// CLEAR : 지우기
// UNDO
// SAVE
// NOT_USED
namespace DRAW_MODE
{
	enum MODE { START = 0, DRAW, STICKER_ON, STICKER_SET, STICKER_EDIT, DONE, 
			    CLEAR, ALL_CLEAR, UNDO, SAVE, NOT_USED };
}

namespace RECORD_MODE
{
	enum MODE {PRE = 0, INIT, NARATION, RECORDING, PRE_PLAY, PLAY, AUTO_PLAY, STOP, DONE, SPEAKER, CLOSE, NOT_USED};
}

// Zorder 구분자
// DrawScene
namespace DEPTH_LAYER
{	
	enum ORDER { INIT = 0, CUBE, DRAW, RECORD, STICKER, MAX_ZORDER };
}

// PlayScene
namespace DEPTH_LAYER_PLAYSCENE
{
	enum ORDER { INIT = 0, BTN, MAX_ZORDER };
}

// 그리기 구분자
namespace CUBE_STATE
{
	// INIT : 최초 위치
	// NONE : 미정의
	// COMPLETE : 완료된 상태
	enum STATE { INIT = 0, NONE, COMPLETE };
}

// 스티커 구분자
namespace STICKER_STATE
{
	// DELETE : 스티커 삭제
	// ADD : 스티커 추가	
	enum STATE { DELETE = 0, ADD, MAX_STICKER_STATE };
}

////////////////////////////////////////
// 버튼 TAG
namespace BTN_TAG
{
	// BACK : 뒤로가기, HELP : 도움말, CAMERA, PLAY: mp4, 
	// REORDER: 재배열, DRAW : 그리기버튼, RECORD: 녹음버튼
	enum TAG { BACK = 101, HELP, CAMERA, PLAY_REORDER, RECORD, DRAW };
}

namespace BTN_TAG_DRAW
{
	// BACK : 뒤로가기, HELP : 도움말, CAMERA, PLAY: mp4, 
	// REORDER: 재배열, DRAW : 그리기버튼, RECORD: 녹음버튼
	enum TAG {BACK = 201, TOOL, STICKER_SAVE, STICKER_LEFT, STICKER_RIGHT, STICKER};
}

namespace BTN_TAG_RECORD
{
	enum TAG {BACK = 301, HELP, NEXT, SPEAKER, PLAY, PLAY_STOP, RECORD, RECORD_STOP, TRASH};
}

namespace TAG_RECORD
{
	enum TAG {TEXT = 310, CUBE_BG, DRAW, STICKER, LINE, NAR};
}

////////////////////////////////////////
namespace PLAY_MODE
{
	enum PlayType
	{
		MODE_LISTEN = 1,
		MODE_INTRO,
		MODE_PLAY
	};

	enum TouchType
	{
		MODE_TOUCH = 1,
		MODE_AUTO
	};
}

////////////////////////////////////////

// 화면 사이즈
// LG G Pad2
static Size LGGPad2ResulutionSize = cocos2d::Size(1280, 800);

#define  SCREEN_SIZE_WIDTH    1920.0f
#define  SCRREN_SIZE_HEIGHT   1200.0f
//#define  RENDERTEXTURE_SIZE_SCALE  (1.5f)
#define  RENDERTEXTURE_POS_X       300.0f // 그리기 영역 위치 300.0
#define  RENDERTEXTURE_POS_Y       270.0f // 그리기 영역 위치 250.0
#define  RENDERTEXTURE_SIZE_WIDTH_MARGIN   120.0f //50.0f // 여백
#define  RENDERTEXTURE_SIZE_HEIGHT_MARGIN  100.0f //50.0f // 여백

// 그림판 영역 1592 X 902
//#define  RENDERTEXTURE_SIZE_WIDTH   1580.0f
#define  RENDERTEXTURE_SIZE_WIDTH   1592.0f
#define  RENDERTEXTURE_SIZE_HEIGHT  902.0f
//#define  RENDERTEXTURE_WIDTH_FOR_SCALE  (RENDERTEXTURE_POS_X + RENDERTEXTURE_SIZE_WIDTH_MARGIN)
//#define  RENDERTEXTURE_HEIGHT_FOR_SCALE (RENDERTEXTURE_POS_Y + RENDERTEXTURE_SIZE_HEIGHT_MARGIN)

#define  INVALID_INDEX         -1
#define  FILENAME_LENGTH       64

////////////////////////////////////////////
// 배열하기
#define  VALID_INDEX_CUBE           1     // 완료된 인덱스 값
#define  SIZE_SCALE_CUBE            0.74f //0.75f, 0.72f
#define  TIME_MOTION_CUBE_POS       0.2f  // 잘못된 그림이 최초 위치로 이동 시간
#define  ZORDER_CUBE                10    // 이동시 그림의 Zorder
#define  ZORDER_CUBE_FINISH         1     // 완료된 그림의 Zorder
#define  ZORDER_CUBE_INIT           1     // 최초 그림의 Zorder
#define  TIME_MOTION_CUBE_INIT      0.5f  // 최초 위치로 이동 시간
#define  TIME_MOTION_CUBE_BLINK     0.5f  // 깜빡이는 시간
#define  TIME_MOTION_CUBE_FADE_OUT  0.5f
 
////////////////////////////////////////////
// 색칠하기
#define  SIZE_BTN_DRAW             6
#define  SIZE_BTN_RECORD           6
#define  SIZE_CUBE                 6  // 전체 액자 갯수
#define  SIZE_USER_CUBE            4  // 사용자 그림 갯수
#define  SIZE_USER_CUBE_LOCK       2  // 적용된 고정 그림 갯수(고정)
#define  SIZE_SCROLL_WITH_BTN_POS  20.0f // 스크롤 버튼 적용시 위치값

#define  ZORDER_DRAW_STICKER       10   // 이동시 그림의 Zorder
#define  ZORDER_DRAW_STICKER_INIT  0    // 최초 그림의 Zorder
#define  SIZE_ERASE_COUNT_UNIT     15  // eraserCanvas 에서 사용되는 증가값

////////////////////////////////////////////
// 스티커 스크롤 타임
#define  SCROLL_TIME               0.8f
#define  TIME_MOTION_STICKER_INIT  0.2f  //최초 위치로 이동 시간
#define  SIZE_MAX_STICKER          10000 //5     // 개별 스티커 갯수

////////////////////////////////////////////
// 녹음하기
#define  TIME_VOICE_RECORD  10.0f
//#define  RECORD_SIZE_CUBE   1090.0f
#define  RECORD_SIZE_CUBE   1168.f//1144.0f
#define  RECORD_TEXT_CNT    6

// 스티커 보정 값
#define  THRESHOLD_STICKER_X  20.0f  
#define  THRESHOLD_STICKER_Y  20.0f

// 재생하기
#define  TIME_MOTION_STAR_BLINK 1.f  //깜빡이는 시간

////////////////////////////////////////////
// 사진찍기
#define  SIZE_CAMERA_PROFILE_PHOTO_WIDTH  930.0f


//-----------------------------------------------
// 색상 값 및 브러쉬 정의
//-----------------------------------------------
#define  COLOR_ARPICOT Color3B(255, 201, 168)
#define  COLOR_ORANGE  Color3B(255, 120, 0)
#define  COLOR_PINK    Color3B(252, 88,  157)
#define  COLOR_RED     Color3B(244, 26,  3)
#define  COLOR_YELLOW  Color3B(255, 220, 0)
#define  COLOR_GREEN   Color3B(52,  153, 20)
#define  COLOR_BLUE    Color3B(1,   146, 238)
#define  COLOR_NAVY    Color3B(48,  74,  126)
#define  COLOR_VIOLET  Color3B(176, 76,  197)
#define  COLOR_BROWN   Color3B(121, 63,  28)
#define  COLOR_GRAY    Color3B(128, 128, 128)
#define  COLOR_BLACK   Color3B(38,  38,  38)
//#define  COLOR_BLACK   Color3B(17,  17,  17)

// 지우개 사이즈
#define  SIZE_ERASER_RADIUS   20.0f

//-----------------------------------------------
// TAG
//-----------------------------------------------
#define TAG_BTN_DRAW_DONE      "BTN_DRAW_DONE_TAG"
#define TAG_BTN_STICKER        "BTN_STICKER_TAG"
#define TAG_BTN_STICKER_CLOSE  "BTN_STICKER_TAG_CLOSE"
#define TAG_LAYER_DRAW         "LAYER_DRAW_TAG"

// RecordLayer Tag
#define TAG_LAYER_RECORD_BG    "LAYER_RECORD_BG_TAG"

// 그림그리기 rendertexture를 구분하기 위한 구분자
const int TAG_CANVAS = 1000;

////////////////////////////////////////
// ZOrder 
// 메인씬에서의 레이어
#define  DEPTH_LAYER_CUBE                    1
#define  DEPTH_LAYER_CUBE_PLAY_REORDER       2
#define  DEPTH_LAYER_DRAW                    10
#define  DEPTH_LAYER_RECORD                  (DEPTH_LAYER_DRAW)

// 그리기 레이어 내에서의 ZOrder
#define  DEPTH_LAYER_DRAW_STICKER_CLOSE_BTN  1
#define  DEPTH_LAYER_DRAW_CANVAS             2   // 캔버스 영역
#define  DEPTH_LAYER_DRAW_LINE               4   // 먹선 이미지 레이어
#define  DEPTH_LAYER_DRAW_CANVAS_BG          5   // 그림판 라운드 처리된 배경 이미지
#define  DEPTH_LAYER_DRAW_INIT_LAYOUT        10  // 칼라 툴 및 스티커 툴 레이어
#define  DEPTH_LAYER_DRAW_STICKER            50  // 스티커 붙여지는 레이어
#define  DEPTH_LAYER_CUBE_POPUP              100 // 팝업 레이어

// RecordLayer 내에서의 ZOrder
#define  DEPTH_LAYER_RECORD_ROUND_BG         10
#define  DEPTH_LAYER_RECORD_CLOSE_BTN        10

//-----------------------------------------------
// JSON
//-----------------------------------------------
// 배열하기 순서 Json
#define  JSON_CUBE_BASE             "LEVELS"
#define  JSON_CUBE_ID               "WEEK"
#define  JSON_CUBE_TITLE            "STUDY_TITLE"
#define  JSON_CUBE_LOCK_X           "LOCK_%d"
#define  JSON_CUBE_FILENAME_X       "FILENAME_CUBE_%d"
#define  JSON_CUBE_DRAW_FILENAME_X  "FILENAME_DRAW_%d"

// write json
// sticker file 쓰기용 -> MBJson.h로 이전
//#define  JSON_STICKER_IDX   "S_IDX"
//#define  JSON_STICKER_POS_X	"S_POS_X"
//#define  JSON_STICKER_POS_Y	"S_POS_Y"

//-----------------------------------------------
// UserDefault(이력관리용)
//-----------------------------------------------
// 권수에 따른 플레이수
#define  USERDEFAULT_KEY_PLAY_WEEKDATA_X "MB_PLAY_WEEKDATA_%d"
// 그리기 도움말 관련 체크박스 체크 정보 저장
#define  USERDEFAULT_KEY_DRAW_HELP_CHECK "MB_DRAW_HELP_CHECK_%d"
#define  USERDEFAULT_KEY_PROFILE_PATH     "MB_PROFILE_PATH"
#define  USERDEFAULT_KEY_PROFILE_ROTATION "MB_PROFILE_ROTATION"
#define  USERDEFAULT_KEY_RECORD_HISTORY_X "MB_RECORD_HISTORY_%d"

//-----------------------------------------------
// FILENAME
//-----------------------------------------------

////////////////////////////////////////
// 공통 
//#define  FILENAME_COMMON_DIR                 "b%d/c09/" // b01/c09 : 1호, .. , b10/c09 10호
#define  FILENAME_COMMON_DIR           "res/"
#define  FILENAME_COMMON_TYPE_IMG      "img"
#define  FILENAME_COMMON_TYPE_SND      "snd"
#define  FILENAME_COMMON_TYPE_flash    "flash"
#define  FILENAME_COMMON_TYPE_MOV      "mp4"

// 타이틀 화면
#define  FILENAME_MAIN_TITLE           "res/title_mybook.png"

// 팝업 배경
#define  FILENAME_POPUP_BG             "res/common/common_popup_bg.png"
#define  FILENAME_POPUP_HELP_YOONIE    "res/common/common_popup_guide_yoonie.png"
#define  FILENAME_POPUP_OPTION_YOONIE  "res/common/common_popup_chant_storysong_yoonie.png"

// 버튼
// 닫기 버튼, 도움말, 다음, 팝업닫기
#define  FILENAME_BTN_CLOSE_N         "res/common/common_btn_back_n.png"
#define  FILENAME_BTN_CLOSE_P         "res/common/common_btn_back_p.png"
#define  FILENAME_BTN_QUIT_N          "res/common/common_btn_quit_n.png"
#define  FILENAME_BTN_QUIT_P          "res/common/common_btn_quit_p.png"
#define  FIlENAME_BTN_HELP_N          "res/common/common_btn_guide_n.png"
#define  FIlENAME_BTN_HELP_P          "res/common/common_btn_guide_p.png"
#define  FIlENAME_BTN_NEXT_N          "res/common/common_btn_next_n.png"
#define  FIlENAME_BTN_NEXT_P          "res/common/common_btn_next_p.png"
#define  FIlENAME_BTN_POPUP_CLOSE_N   "res/common/common_popup_btn_close_n.png"
#define  FIlENAME_BTN_POPUP_CLOSE_P   "res/common/common_popup_btn_close_p.png"

// 공통 버튼(레코드)
#define  FILENAME_COMMON_BTN_RECORD_STROKE "res/common/common_record_btn_record.png"
#define  FILENAME_COMMON_BTN_PLAY_STROKE   "res/common/common_record_btn_play.png"
#define  FILENAME_COMMON_BTN_STOP_BG       "res/common/btn_bg.png"
#define  FILENAME_COMMON_BTN_STOP_FX       "res/common/img_stop_fx.png"
#define  FILENAME_COMMON_BTN_STOP_IMG      "res/common/img_stop.png"
#define  FILENAME_COMMON_BTN_RECORD_N      "res/common/comon_record_btn_record_able_n.png"  // 마이크
#define  FILENAME_COMMON_BTN_RECORD_P      "res/common/comon_record_btn_record_able_p.png"
#define  FILENAME_COMMON_BTN_RECORD_D      "res/common/comon_record_btn_record_disable.png"
#define  FILENAME_COMMON_BTN_PLAY_N        "res/common/comon_record_btn_play_able_n.png"  // 재생하기
#define  FILENAME_COMMON_BTN_PLAY_P        "res/common/comon_record_btn_play_able_p.png"
#define  FILENAME_COMMON_BTN_PLAY_D        "res/common/comon_record_btn_play_disable.png"
#define  FILENAME_COMMON_BTN_STOP_N        "res/common/comon_record_btn_stop_able_n.png"  // 재생하기
#define  FILENAME_COMMON_BTN_STOP_P        "res/common/comon_record_btn_stop_able_p.png"
#define  FILENAME_COMMON_BTN_STOP_D        "res/common/comon_record_btn_stop_disable.png"
#define  FILENAME_COMMON_BTN_SAVE_N        "res/common/comon_record_btn_save_able_n.png"  // 저장하기
#define  FILENAME_COMMON_BTN_SAVE_P        "res/common/comon_record_btn_save_able_p.png"
#define  FILENAME_COMMON_BTN_SAVE_D        "res/common/comon_record_btn_save_disable.png"
#define  FILENAME_COMMON_BTN_TRASH_N       "res/common/comon_record_btn_trash_able_n.png"  // 휴지통
#define  FILENAME_COMMON_BTN_TRASH_P       "res/common/comon_record_btn_trash_able_p.png"
#define  FILENAME_COMMON_BTN_TRASH_D       "res/common/comon_record_btn_trash_disable.png"

// 프로그레스바(공통)
#define  FILENAME_COMMON_PROGRESS_BG       "res/common/common_progress_bg.png"
#define  FILENAME_COMMON_PROGRESS_BAR      "res/common/common_progress_bar.png"

// 종료 팝업 버튼
#define  FILENAME_COMMON_POPUP_BTN_YES_N  "res/common/common_c09_yes3_normal.png"
#define  FILENAME_COMMON_POPUP_BTN_YES_P  "res/common/common_c09_yes_pressed.png"
#define  FILENAME_COMMON_POPUP_BTN_NO_N   "res/common/common_c09_no_normal.png"
#define  FILENAME_COMMON_POPUP_BTN_NO_P   "res/common/common_c09_no_pressed.png"


////////////////////////////////////////
// 배열하기 도움말 팝업 텍스트
#define  FILENAME_COMMON_POPUP_CUBE_TEXT               "res/common/common_c09_cube.png"
#define  FILENAME_COMMON_POPUP_DRAW_EXIT_TXT           "res/common/common_c09_draw_exit_popup_txt.png"  // 그리기 종료 팝업
#define  FILENAME_COMMON_POPUP_DRAW_ALL_ERASE_TXT      "res/common/common_c09_erase.png"  // 그리기 모두 지우기 팝업
#define  FILENAME_COMMON_POPUP_DRAW_SAVE_TXT           "res/common/common_c09_save.png"  // 그리기 저장 팝업
// 그리기 도움말 팝업 텍스트
#define  FILENAME_COMMON_POPUP_DRAW_HELP_TXT           "res/common/common_c09_draw.png"
#define  FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_C    "res/common/checkbox_check.png"
#define  FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_N    "res/common/checkbox_normal.png"
#define  FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT  "res/common/text_nomorereview.png"
// 녹음하기 도움말/ 종료/ 삭제 텍스트
#define  FILENAME_COMMON_POPUP_RECORD_HELP_TXT         "res/common/common_c09_record.png"
#define  FILENAME_COMMON_POPUP_RECORD_EXIT_TXT         "res/common/common_c09_exit.png"
#define  FILENAME_COMMON_POPUP_RECORD_DELETE_TXT       "res/common/common_c09_record_delete.png"
// 재생하기 재생끝 팝업 텍스트
#define  FILENAME_COMMON_POPUP_PLAY_ENDING_TXT         "res/common/common_popup_mybook_guide2.png"

// 프로필 파일
// net.minigate.smartdoodle.storyfun.viewer.mybook
//#define  FILENAME_COMMON_CAPTURED_PROFILE  "/storage/emulated/0/Android/data/com.mybook/profile/mybook_profile.jpg"
#define  FILENAME_COMMON_CAPTURED_PROFILE       "/storage/emulated/0/Android/data/net.minigate.smartdoodle.storyfun.viewer.mybook/profile/mybook_profile.jpg"
#define  FILENAME_COMMON_CAPTURED_PROFILE_TEMP  "/storage/emulated/0/Android/data/net.minigate.smartdoodle.storyfun.viewer.mybook/profile/camerashot_temp.jpg"
#define  FILENAME_COMMON_CAPTURED_PROFILE_UNI   "res/cube/uni_char_face.png"


////////////////////////////////////////
////////////////////////////////////////
// 배열하기
// Json
#define  FILENAME_JSON_CUBE                  "res/json/mainscene_cube.json"

#define  FILENAME_BG_MAINSCENE               "res/cube/main_bg3.png"
#define  FILENAME_BG_MAINSCENE_TONGS         "res/cube/main_tongs.png"
#define  FILENAME_CUBE_BG                    "res/cube/bg_cube_panel.png"
// 카메라 버튼
#define  FILENAME_CUBE_BTN_CAMERA_N          "res/cube/common_btn_camera_normal.png"
#define  FILENAME_CUBE_BTN_CAMERA_P          "res/cube/common_btn_cameralpress.png"
// 재생하기/재배열하기 버튼
#define  FILENAME_BTN_PLAY_N                 "res/cube/btn_plus_play_normal.png"  // 재생하기
#define  FILENAME_BTN_PLAY_P                 "res/cube/btn_plus_play_press.png"   // 재생하기
#define  FILENAME_BTN_REORDER_N              "res/cube/btn_plus_arrange_normal.png"  // 재배열하기
#define  FILENAME_BTN_REORDER_P              "res/cube/btn_plus_arrange_press.png"  // 재배열하기
// 녹음하기 버튼
#define  FILENAME_CUBE_BTN_ENABLE_RECORD_N   "res/cube/btn_rec2_normal_t.png" // 녹음하기(활성화)
#define  FILENAME_CUBE_BTN_ENABLE_RECORD_P   "res/cube/btn_rec2_press_t.png"  // 녹음하기(활성화)
#define  FILENAME_CUBE_BTN_DISABLE_RECORD_N  "res/cube/btn_rec_normal_t.png" //  녹음하기(비활성화)
#define  FILENAME_CUBE_BTN_DISABLE_RECORD_P  "res/cube/btn_rec_press.png"    //  녹음하기(비활성화)
// 그리기 버튼
#define  FILENAME_CUBE_BTN_ENABLE_DRAW_N     "res/cube/btn_edit2_normal_t.png" // 그리기 (활성화) // "res/cube/btn_edit2_normal.png"
#define  FILENAME_CUBE_BTN_ENABLE_DRAW_P     "res/cube/btn_edit2_press_t.png"  // 그리기 (활성화)
#define  FILENAME_CUBE_BTN_DISABLE_DRAW_N    "res/cube/btn_edit_normal_t.png"  // 그리기 (비활성화)
#define  FILENAME_CUBE_BTN_DISABLE_DRAW_P    "res/cube/btn_edit_press.png"     // 그리기 (비활성화)

// 브러쉬
// 50pixel : brush_medium.png, 100 pixel : brush_large.png, 25pixel : brush_small.png
#define  FILENAME_DRAW_TOOL_BRUSH            "res/draw/brush_medium2.png"  // 50 pixel
#define  FILENAME_DRAW_TOOL_BRUSH_ERASER     "res/draw/brush_medium3.png"  // 75 pixel

#define  FILENAME_CUBE_POPUP_BG              "res/common/popup_bg.png" 
#define  FILENAME_CUBE_PROFILE_PHOTO         "mybook_profile.jpg"

// 레고 블럭
#define  FILENAME_CUBE_REGO_BLOCK            "res/cube/bg_block.png"

////////////////////////////////////////
////////////////////////////////////////
// 색칠하기 
// 그림 영역 배경
#define  FILENAME_DRAW_CUBE_BG              "res/draw/draw_cube_bg.png"

// 버튼
#define  FILENAME_DRAW_BTN_CLOSE_N          "res/common/common_popup_btn_close_n.png"
#define  FILENAME_DRAW_BTN_CLOSE_P          "res/common/common_popup_btn_close_p.png"

////////////////////////////////////////
// 색상 
#define  FILENAME_DRAW_TOOL_COLOR_RED		 "res/draw/tool/color_red.png"
#define  FILENAME_DRAW_TOOL_COLOR_ORRAGE	 "res/draw/tool/color_orrange.png"
#define  FILENAME_DRAW_TOOL_COLOR_YELLOW	 "res/draw/tool/color_yellow.png"
#define  FILENAME_DRAW_TOOL_COLOR_GREEN		 "res/draw/tool/color_green.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLUE		 "res/draw/tool/color_blue.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLUISH	 "res/draw/tool/color_blue.png"
#define  FILENAME_DRAW_TOOL_COLOR_PURPLE	 "res/draw/tool/color_blue.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLACK      "res/draw/tool/color_black.png"
#define  FILENAME_DRAW_TOOL_PANEL_CLEAR      "res/draw/tool/all_clear.png"
#define  FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR  "res/draw/tool/all_clear.png"

// 색상 (최종)
// ARPICOT, ORANGE, PINK, RED, YELLOW, GREEN, BLUE, NAVY, VIOLET, BROWN, GREY, BLACK
#define  FILENAME_DRAW_TOOL_COLOR_APRICOT_N  "res/draw/tool/common_c09_color_apricot_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_APRICOT_P  "res/draw/tool/common_c09_color_apricot_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_ORANGE_N   "res/draw/tool/common_c09_color_orange_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_ORANGE_P   "res/draw/tool/common_c09_color_orange_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_PINK_N     "res/draw/tool/common_c09_color_pink_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_PINK_P     "res/draw/tool/common_c09_color_pink_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_RED_N      "res/draw/tool/common_c09_color_red_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_RED_P      "res/draw/tool/common_c09_color_red_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_YELLOW_N   "res/draw/tool/common_c09_color_yellow_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_YELLOW_P   "res/draw/tool/common_c09_color_yellow_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_GREEN_N    "res/draw/tool/common_c09_color_green_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_GREEN_P    "res/draw/tool/common_c09_color_green_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLUE_N     "res/draw/tool/common_c09_color_blue_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLUE_P     "res/draw/tool/common_c09_color_blue_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_NAVY_N     "res/draw/tool/common_c09_color_navy_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_NAVY_P     "res/draw/tool/common_c09_color_navy_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_VIOLET_N   "res/draw/tool/common_c09_color_violet_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_VIOLET_P   "res/draw/tool/common_c09_color_violet_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_BROWN_N    "res/draw/tool/common_c09_color_brown_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_BROWN_P    "res/draw/tool/common_c09_color_brown_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_GREY_N     "res/draw/tool/common_c09_color_grey_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_GREY_P     "res/draw/tool/common_c09_color_grey_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLACK_N    "res/draw/tool/common_c09_color_black_n.png"
#define  FILENAME_DRAW_TOOL_COLOR_BLACK_P    "res/draw/tool/common_c09_color_black_p.png"
#define  FILENAME_DRAW_TOOL_COLOR_SELECT     "res/draw/tool/common_c09_color_p.png"

// 지우개 버튼
#define  FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_N  "res/draw/tool/common_c09_eraser_all_n.png"
#define  FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_P  "res/draw/tool/common_c09_eraser_all_p.png"
#define  FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_S  "res/draw/tool/common_c09_eraser_all_s.png"
#define  FILENAME_DRAW_TOOL_PANEL_CLEAR_N      "res/draw/tool/common_c09_eraser_n.png"
#define  FILENAME_DRAW_TOOL_PANEL_CLEAR_P      "res/draw/tool/common_c09_eraser_p.png"
#define  FILENAME_DRAW_TOOL_PANEL_CLEAR_S      "res/draw/tool/common_c09_eraser_s.png"

// 칼라 보드(배경)
#define  FILENAME_DRAW_TOOL_PANEL		       "res/draw/tool/common_c09_color_board.png"
#define  FILENAME_DRAW_TOOL_SAVE_N             "res/draw/tool/common_c09_btn_save_n.png"
#define  FILENAME_DRAW_TOOL_SAVE_P             "res/draw/tool/common_c09_btn_save_p.png"
#define  FILENAME_DRAW_TOOL_SAVE_D             "res/draw/tool/common_c09_btn_save_Disable.png"

////////////////////////////////////////
// 스티커 (14종)
// 스티커 패널
#define  FILENAME_DRAW_STICKER_PANEL          "res/draw/tool/common_c09_board.png"

// 스티커 (14종)
// BALL, BALLOON, BEAR, BUS, CANDY, CLOUD, FLOWER, GRASS, HEART, LEAF, PLANE, RIBBON, STAR, TREE
#define  FILENAME_DRAW_STICKER_BALL_N      "res/draw/sticker/common_c09_sticker_ball_n.png"
#define  FILENAME_DRAW_STICKER_BALL_P      "res/draw/sticker/common_c09_sticker_ball_paste.png"
#define  FILENAME_DRAW_STICKER_BALL_D      "res/draw/sticker/common_c09_sticker_ball_d.png"
#define  FILENAME_DRAW_STICKER_BALLOON_N   "res/draw/sticker/common_c09_sticker_balloon_n.png"
#define  FILENAME_DRAW_STICKER_BALLOON_P   "res/draw/sticker/common_c09_sticker_balloon_paste.png"
#define  FILENAME_DRAW_STICKER_BALLOON_D   "res/draw/sticker/common_c09_sticker_balloon_d.png"
#define  FILENAME_DRAW_STICKER_BEAR_N      "res/draw/sticker/common_c09_sticker_bear_n.png"
#define  FILENAME_DRAW_STICKER_BEAR_P      "res/draw/sticker/common_c09_sticker_bear_paste.png"
//#define  FILENAME_DRAW_STICKER_BEAR_P      "res/draw/sticker/common_c09_sticker_bear_paste_80.png"
#define  FILENAME_DRAW_STICKER_BEAR_D      "res/draw/sticker/common_c09_sticker_bear_d.png"
#define  FILENAME_DRAW_STICKER_BUS_N       "res/draw/sticker/common_c09_sticker_bus_n.png"
#define  FILENAME_DRAW_STICKER_BUS_P       "res/draw/sticker/common_c09_sticker_bus_paste.png"
#define  FILENAME_DRAW_STICKER_BUS_D       "res/draw/sticker/common_c09_sticker_bus_d.png"
#define  FILENAME_DRAW_STICKER_CANDY_N     "res/draw/sticker/common_c09_sticker_candy_n.png"
#define  FILENAME_DRAW_STICKER_CANDY_P     "res/draw/sticker/common_c09_sticker_candy_paste.png"
#define  FILENAME_DRAW_STICKER_CANDY_D     "res/draw/sticker/common_c09_sticker_candy_d.png"
#define  FILENAME_DRAW_STICKER_CLOUD_N     "res/draw/sticker/common_c09_sticker_cloud_n.png"
#define  FILENAME_DRAW_STICKER_CLOUD_P     "res/draw/sticker/common_c09_sticker_cloud_paste.png"
#define  FILENAME_DRAW_STICKER_CLOUD_D     "res/draw/sticker/common_c09_sticker_cloud_d.png"
#define  FILENAME_DRAW_STICKER_FLOWER_N    "res/draw/sticker/common_c09_sticker_flower_n.png"
#define  FILENAME_DRAW_STICKER_FLOWER_P    "res/draw/sticker/common_c09_sticker_flower_paste.png"
#define  FILENAME_DRAW_STICKER_FLOWER_D    "res/draw/sticker/common_c09_sticker_flower_d.png"
#define  FILENAME_DRAW_STICKER_GRASS_N     "res/draw/sticker/common_c09_sticker_grass_n.png"
#define  FILENAME_DRAW_STICKER_GRASS_P     "res/draw/sticker/common_c09_sticker_grass_paste.png"
#define  FILENAME_DRAW_STICKER_GRASS_D     "res/draw/sticker/common_c09_sticker_grass_d.png"
#define  FILENAME_DRAW_STICKER_HEART_N     "res/draw/sticker/common_c09_sticker_heart_n.png"
#define  FILENAME_DRAW_STICKER_HEART_P     "res/draw/sticker/common_c09_sticker_heart_paste.png"
#define  FILENAME_DRAW_STICKER_HEART_D     "res/draw/sticker/common_c09_sticker_heart_d.png"
#define  FILENAME_DRAW_STICKER_LEAF_N      "res/draw/sticker/common_c09_sticker_leaf_n.png"
#define  FILENAME_DRAW_STICKER_LEAF_P      "res/draw/sticker/common_c09_sticker_leaf_paste.png"
#define  FILENAME_DRAW_STICKER_LEAF_D      "res/draw/sticker/common_c09_sticker_leaf_d.png"
#define  FILENAME_DRAW_STICKER_PLANE_N     "res/draw/sticker/common_c09_sticker_plane_n.png"
#define  FILENAME_DRAW_STICKER_PLANE_P     "res/draw/sticker/common_c09_sticker_plane_paste.png"
#define  FILENAME_DRAW_STICKER_PLANE_D     "res/draw/sticker/common_c09_sticker_plane_d.png"
#define  FILENAME_DRAW_STICKER_RIBBON_N    "res/draw/sticker/common_c09_sticker_ribbon_n.png"
#define  FILENAME_DRAW_STICKER_RIBBON_P    "res/draw/sticker/common_c09_sticker_ribbon_paste.png"
#define  FILENAME_DRAW_STICKER_RIBBON_D    "res/draw/sticker/common_c09_sticker_ribbon_d.png"
#define  FILENAME_DRAW_STICKER_STAR_N      "res/draw/sticker/common_c09_sticker_star_n.png"
#define  FILENAME_DRAW_STICKER_STAR_P      "res/draw/sticker/common_c09_sticker_star_paste.png"
#define  FILENAME_DRAW_STICKER_STAR_D      "res/draw/sticker/common_c09_sticker_star_d.png"
#define  FILENAME_DRAW_STICKER_TREE_N      "res/draw/sticker/common_c09_sticker_tree_n.png"
#define  FILENAME_DRAW_STICKER_TREE_P      "res/draw/sticker/common_c09_sticker_tree_paste.png"
#define  FILENAME_DRAW_STICKER_TREE_D      "res/draw/sticker/common_c09_sticker_tree_d.png"

// 스티커 왼쪽 버튼
#define  FILENAME_DRAW_STICKER_LEFT_BTN_P    "res/draw/sticker/common_c09_left_p.png"
#define  FILENAME_DRAW_STICKER_LEFT_BTN_N    "res/draw/sticker/common_c09_left_n.png"
#define  FILENAME_DRAW_STICKER_RIGHT_BTN_P   "res/draw/sticker/common_c09_right_p.png"
#define  FILENAME_DRAW_STICKER_RIGHT_BTN_N   "res/draw/sticker/common_c09_right_n.png"

// 스티커 삭제 버튼
#define  FILENAME_DRAW_STICKER_CLOSE_BTN   "res/draw/sticker/common_c09_btn_delete.png"

////////////////////////////////////////
////////////////////////////////////////
// 녹음하기
#define  FILENAME_RECORD_BG              "res/record/recordlayer_bg.png"
#define  FILENAME_RECORD_BG_CUBE         "res/record/recordlayer_cube_bg.png"
#define  FILENAME_RECORD_BG_CONTROL_BOX  "res/common/common_record_player_bg.png"

// 버튼
// 스피커 버튼
//#define  FILENAME_RECORD_BTN_SPEAKER_N    "res/record/btn_record_normal.png"
//#define  FILENAME_RECORD_BTN_SPEAKER_P    "res/record/btn_record_press.png"

#define  FILENAME_RECORD_BTN_SPEAKER_N    "res/common/common_record_btn_sound_n.png"
#define  FILENAME_RECORD_BTN_SPEAKER_P    "res/common/common_record_btn_sound_p.png"


////////////////////////////////////////
// 재생하기
#define  FILENAME_PLAY_COVER_BG      "res/play/bg.png"
#define  FILENAME_PLAY_VOLUME_BG     "res/play/bg_volume.png"
#define  FILENAME_PLAY_EFFECT_01     "res/play/dotdot.png"
#define  FILENAME_PLAY_CHAR_01       "res/play/cha0001.png"
#define  FILENAME_PLAY_CHAR_02       "res/play/cha0002.png"
#define  FILENAME_PLAY_STAR_B        "res/play/star_blue.png"
#define  FILENAME_PLAY_STAR_O        "res/play/star_orange.png"
#define  FILENAME_PLAY_STAR_Y        "res/play/star_yellow.png"
#define  FILENAME_PLAY_SHINE_FIRST   "res/play/sh0001.png"
#define  FILENAME_PLAY_SHINE_SECOND  "res/play/sh0002.png"

// 종료 팝업 버튼
#define  FILENAME_PLAY_POPUP_BTN_REPLAY_N  "res/common/common_popup_chant_btn_replay_n.png"
#define  FILENAME_PLAY_POPUP_BTN_REPLAY_P  "res/common/common_popup_chant_btn_replay_p.png"
#define  FILENAME_PLAY_POPUP_BTN_NEXT_N    "res/common/common_popup_chant_btn_next_n.png"
#define  FILENAME_PLAY_POPUP_BTN_NEXT_P    "res/common/common_popup_chant_btn_next_p.png"


////////////////////////////////////////
// 그리기 (배경: 먹선이미지) - JSON
// 사용자 그림 저장 파일명
#define  FILENAME_DRAW_CUBE_IMG_X           "caputre_%d_%d.png"
#define  FILENAME_DRAW_CUBE_IMG_STICKER_X   "caputre_sticker_%d_%d.png"  // 사용자 그림(스티커) 저장 파일명
#define  FILENAME_DRAW_BASE_STICKER_JSON_X  "caputre_%d_%d.json"   // 스티커 위치 정보
#define  FILENAME_DRAW_COLOR				"%s_color.png"  // 칼라 그림 파일 명
#define  FILENAME_DRAW_LINE			  	    "%s_line.png"  // 먹선만 있는 그림 파일 명

////////////////////////////////////////
// 권별 리소스
#define  FILENAME_CUBE_TITLE_X            "img/cube/book_title00%02d.png" // 배열하기 타이틀
#define  FILENAME_CUBE_BASE_BACKGROUND_X  "img/cube/%s" // 배열하기 파일 명(FullPathName)
#define  FILENAME_RECORD_TEXT_X           "img/record/b%02d_%04d.png" // 녹음하기 영문 텍스트
#define  FILENAME_DRAW_BASE_BACKGROUND_X  "img/draw/%s" // 그리기 파일 명(FullPathName)
#define  FILENAME_PLAY_COVER_X            "img/play/b%02d.png" // 권별 책표지

////////////////////////////////////////
// Preloading
#define  FILENAME_ANI_PRELOAD_X   "common/img/preload_ani_00%02d.png"
#define  ANI_PRELOAD_MAX_INDEX    57

////////////////////////////////////////
// GAF
//#define  FILENAME_GAF_TITLE               "res/title_151202.gaf"
//#define  FILENAME_GAF_PRELOAD             "res/common/preloading/gaf/contents_preloader_1920_160615.gaf"

// 페이지 넘김 처리 시간
#define  PLAYSCENE_PAGE_TRANSITION_DELAY  0.8f

// 포트폴리오 파일명
#define  PLAYSCENE_PORTFOLIO_FILENAME  "book%d_mybook.mp4"
//#define  PLAYSCENE_PORTFOLIO_FILENAME  "mybook_screen_%d.mp4"

// 포트폴리오 넘버
#define  PORTFOLIO_ID  20

// 테스트용 권 정보
//#define  FILENAME_WEEK_DATA_TEST          2

#endif // __MYBOOK_RESOURCES_H__