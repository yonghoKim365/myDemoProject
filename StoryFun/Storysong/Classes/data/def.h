#ifndef _DEF_H_
#define _DEF_H_

#include "../external/json/rapidjson.h"
#include "../external/json/document.h"
#include "../external/json/filestream.h"
#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"


USING_NS_GAF;

#define GAF_NAME_INTRO				"Intro GAF Animation"

#define BUTTON_EFFECT_SOUND_PATH	"common/sound/common_sfx_btn_01.mp3"
#define BUTTON_EFFECT_SOUND_PATH2	"common/sound/common_sfx_btn_02.mp3"

#define POPUP_OPTION_SOUND_PATH		"common/sound/com_c08_guide_02.mp3"

#define GUID_LISTEN_SOUND_PATH		"common/sound/com_c08_guide_01.mp3"
#define GUID_PLAY_SOUND_PATH		"common/sound/com_c08_guide_03.mp3"


#define EFFECT_EXPLODE_PATH			"common/sound/b02_c08_s2_t1_sfx_02.mp3"

#define USER_DATA_SHOW_POPUP		"show popup"	
#define USER_DATA_SHOW_LISTEN_GUIDE	"show listen guide"		
#define USER_DATA_SHOW_PLAY_GUIDE	"show play guide"		


#define STEP1_BACKGROUND_IMAGE		"common/img/title.png"
#define STEP1_TOUCH_OBJECT			"common/gaf/step1_object/step1_object.gaf"
#define STEP1_GUIDE_ANIM_PATH		"common/gaf/step1_guide/step1_guide.gaf"

#define STEP1_TOUCH_OBJECT_PATH0	"common/img/step1_object0.png"
#define STEP1_TOUCH_OBJECT_PATH1	"common/img/step1_object1.png"
#define STEP1_TOUCH_OBJECT_PATH2	"common/img/step1_object2.png"
#define STEP1_TOUCH_OBJECT_PATH3	"common/img/step1_object3.png"
#define STEP1_TOUCH_OBJECT_PATH4	"common/img/step1_object4.png"

#define BUTTON_EXIT_NORMAL_PATH		"common/img/common_btn_quit_n.png"
#define BUTTON_EXIT_PRESS_PATH		"common/img/common_btn_quit_n.png"
#define BUTTON_NEXT_NORMAL_PATH		"common/img/common_btn_next_n.png"
#define BUTTON_NEXT_PRESS_PATH		"common/img/common_btn_next_p.png"

#define GAF_CONTENTS_HOLDER_LETF	"common/gaf/storysong_intro_left/storysong_intro_left.gaf"
#define GAF_CONTENTS_HOLDER_CENTER	"common/gaf/storysong_intro_center/storysong_intro_center.gaf"
#define GAF_CONTENTS_HOLDER_RIGHT	"common/gaf/storysong_intro_right/storysong_intro_right.gaf"

#define GAF_CONTENTS_HOLDER_BLUE_LEFT	"common/gaf/storysong_intro_left_blue/storysong_intro_left_blue.gaf"
#define GAF_CONTENTS_HOLDER_BLUE_CENTER	"common/gaf/storysong_intro_center_blue/storysong_intro_center_blue.gaf"
#define GAF_CONTENTS_HOLDER_BLUE_RIGHT	"common/gaf/storysong_intro_right_blue/storysong_intro_right_blue.gaf"

#define GAF_CONTENTS_HOLDER_WHITE_LEFT	"common/gaf/storysong_intro_left_white/storysong_intro_left_white.gaf"
#define GAF_CONTENTS_HOLDER_WHITE_CENTER	"common/gaf/storysong_intro_center_white/storysong_intro_center_white.gaf"
#define GAF_CONTENTS_HOLDER_WHITE_RIGHT	"common/gaf/storysong_intro_right_white/storysong_intro_right_white.gaf"


#define GAF_CONTENTS_HOLDER_YELLOW_LEFT		"common/gaf/storysong_intro_left_yellow/storysong_intro_left_yellow.gaf"
#define GAF_CONTENTS_HOLDER_YELLOW_CENTER	"common/gaf/storysong_intro_center_yellow/storysong_intro_center_yellow.gaf"
#define GAF_CONTENTS_HOLDER_YELLOW_RIGHT	"common/gaf/storysong_intro_right_yellow/storysong_intro_right_yellow.gaf"

class PreLoadEventListener
{
public:
	virtual void onLoadComplete() = 0;
};

namespace ANIMATION_TYPE
{
	enum SongType
	{
		ANIMATION_TYPE_A = 1,
		ANIMATION_TYPE_B,
		ANIMATION_TYPE_C,
		ANIMATION_TYPE_D
	};
}
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

namespace SCENE_TYPE
{
	enum SceneType
	{
		SCENE_LISTEN = 1,
		SCENE_PLAY
	};
}

class IBaiscSongPlay
{
public:
	virtual void onPlaybyTickEvent(int tickCount) = 0;
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode) = 0;
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode) = 0;
};

class TocuchLayerEventListener
{
public:
	virtual void onIntroAnimationFinished() = 0;
};


#define BASIC_POSITION Vec2(0,1200)
#endif