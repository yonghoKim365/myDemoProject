#ifndef __MYBOOK_SND_H__
#define __MYBOOK_SND_H__

#include "cocos2d.h"

// Sound
#include "SimpleAudioEngine.h" 
#include "AudioEngine.h"

USING_NS_CC;
using namespace CocosDenshion;
// for AudioEngine
//using namespace cocos2d::experimental;

// 재생하기
#define  SND_VOLUME_MP3            0.7f
// 음원 재생후 2초후 페이지 이동
#define  SND_PLAY_CORRECTION_TIME  2.0f
// 녹음한 음원 보정값
#define  SND_CORRECTION_VALUE      0.6f
#define  SND_CORRECTION_VALUE_MSEC  300 //100
#define  USER_DATA_PLAY_TIME_KEY_X  "PLAY_TIME_%d_%d"

//-----------------------------------------------
// FILENAME
//-----------------------------------------------
// BGM
#define  FILENAME_SND_BGM_CUBE  "res/common/sound/bgm/common_c09_bgm_01.mp3" // 배열화면
#define  FILENAME_SND_BGM_DRAW  "res/common/sound/bgm/common_c09_bgm_01.mp3" // 색칠화면
//#define  FILENAME_SND_BGM_MOV   "res/common/sound/bgm/mov_cover_bgm.mp3" // 책커버 사운드
#define  FILENAME_SND_BGM_M4A   "res/common/sound/bgm/cube_bgm.m4a"      // 배열화면

////////////////////////////////////////
// 나레이션
// 타이틀 나레이션
#define  FILENAME_SND_TITLE_NAR           "res/common/sound/narration/common_c09_Title_01.mp3"
// 화면에 들어갔을때
#define  FILENAME_SND_CUBE_NAR1           "res/common/sound/narration/common_c09_NAR_01.mp3"
#define  FILENAME_SND_CUBE_NAR2           "res/common/sound/narration/common_c09_NAR_02.mp3"
// 녹음하기
#define  FILENAME_SND_RECORD_OPENNING     "res/common/sound/narration/common_c09_NAR_03.mp3"
#define  FILENAME_SND_RECORD_PLAY_ENABLE  "res/common/sound/narration/common_c09_NAR_04.mp3" // 녹음하기 버튼이 활성화 되었을때
// 색칠하기
#define  FILENAME_SND_DRAW_OPENNING       "res/common/sound/narration/common_c09_NAR_05.mp3"


////////////////////////////////////////
// 녹음하기

// 녹음한 음성 파일 네임
//"/Android/data/com.mybook/record/"
//#define  FILENAME_SND_RECORD_VOICE_BASE_X "/storage/emulated/0/DCIM/record/"
#define  FILENAME_SND_RECORD_VOICE_BASE_X  "/storage/emulated/0/Android/data/net.minigate.smartdoodle.storyfun.viewer.mybook/record/"
#define  FILENAME_SND_RECORD_VOICE_X       "mybook_%d_%d.mp3"

// 녹음하기 텍스트 파일 네임(권별)
#define  FILENAME_SND_RECORD_TEXT_BASE_X  "snd/record/"
#define  FILENAME_SND_RECORD_TEXT_X       "b%02d_c09_0%d.mp3"

////////////////////////////////////////
// 재생하기(권별)
#define  FILENAME_SND_PLAY_VALUME_X   "snd/play/b%02d_c09_Title_01.mp3"


////////////////////////////////////////
// 효과음
#define  FILENAME_SND_EFFECT_BTN	           "res/common/sound/common_sfx_btn_01.mp3"
#define  FILENAME_SND_EFFECT_DRAW_LINE         "res/common/sound/common_sfx_05.mp3" // 그리기
#define  FILENAME_SND_EFFECT_DRAW_ERASE        "res/common/sound/common_sfx_06.mp3" // 지우기
#define  FILENAME_SND_EFFECT_DRAW_ALL_ERASE    "res/common/sound/common_sfx_07.mp3" // 전체지우기
#define  FILENAME_SND_EFFECT_DRAW_SET_STICKER  "res/common/sound/common_sfx_08.mp3" // 스티커붙이기
#define  FILENAME_SND_EFFECT_RECORD_COMPLETE   "res/common/sound/common_sfx_09.mp3" // 녹음완료소리
//#define  FILENAME_SND_EFFECT_RECORD_START_ON   "res/snd/effect/common_sfx_10.mp3" // 녹음시작소리
#define  FILENAME_SND_EFFECT_RECORD_START_ON   "res/common/sound/common_sfx_16.mp3" // 녹음시작소리
#define  FILENAME_SND_EFFECT_RECORD_DONE       "res/common/sound/common_sfx_17.mp3" // 녹음완료 효과음
#define  FILENAME_SND_EFFECT_RECORD_PLAY_ON    "res/common/sound/common_sfx_12.mp3" // 재생하기 효과음
//#define  FILENAME_SND_EFFECT_RECORD_DELETE     "res/common/sound/common_sfx_13.mp3" // 녹음 삭제 효과음
#define  FILENAME_SND_EFFECT_RECORD_DELETE     "res/common/sound/common_sfx_05.mp3" // 녹음 삭제 효과음
#define  FILENAME_SND_EFFECT_COMPLETE_POPUP    "res/common/sound/common_sfx_popup_01.mp3" // 종료후 옵션 창 자동 팝업
#define  FILENAME_SND_EFFECT_DRAW_COLOR        "res/common/sound/common_sfx_btn_02.mp3" // 컬러 선택, 지우기, 전체지우기

#define  FILENAME_SND_EFFECT_BTN_CLICK         "res/common/sound/common_sfx_btn_02.mp3" // 버튼 효과음
#define  FILENAME_SND_EFFECT_GNB_BTN_CLICK     "res/common/sound/common_sfx_btn_01.mp3" // 버튼 효과음
// 배열하기(성공, 실패, 텝)
#define  FILENAME_SND_EFFECT_CUBE_SUCCESS      "res/common/sound/common_sfx_correct_01.mp3"  // 배열하기 그림 배열할때
#define  FILENAME_SND_EFFECT_CUBE_FAIL         "res/common/sound/common_sfx_wrong_01.mp3" // 정답 실패
#define  FILENAME_SND_EFFECT_CUBE_TAP          "res/common/sound/common_sfx_btn_03.mp3" // 텝할때 효과음

#define  FILENAME_SND_EFFECT_CUBE_BLINK        "res/common/sound/common_sfx_scale_01.mp3" // 깜빡 거릴때
#define  FILENAME_SND_EFFECT_CUBE_REORDER      "res/common/sound/common_c09_sfx_01.mp3" // 재배열시 장면 사라질때

// 재생하기
#define  FILENAME_SND_EFFECT_PLAY_TRANSITION   "res/common/sound/common_c09_sfx_02.mp3"

#endif // __MYBOOK_SND_H__