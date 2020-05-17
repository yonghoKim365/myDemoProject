#ifndef _ABS_TOUCH_LAYER_H_
#define _ABS_TOUCH_LAYER_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/def.h"
#include "data/JsonInfo.h"
#include "actor/SimpleTouchActor.h"
#include "actor/MachineActor.h"
#include "actor/WordActor.h"
#include "audio/include/AudioEngine.h"
//#include "actor/SimpleWordActor.h"
#include "common/AssetUtils.h"

using namespace experimental;
USING_NS_CC;
USING_NS_GAF;

class AbsTouchLayer : public Layer, public IBaiscSongPlay, public WordActorEventListener
{
public:
	static AbsTouchLayer * create();

	virtual void resetLayer();
	virtual void startAnimation();
	virtual void removeTouchEvent();

	virtual void onPlaybyTickEvent(int tickCount);
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode);
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode);

	virtual void onWordExampleExit(int id);
	virtual void onWordExampleExit2(int id);

	virtual void initData();
	virtual void initGUI();
	virtual void initEvent();
	virtual void touchEvent(cocos2d::Touch* touch, Vec2 _p);
	virtual bool isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch);
	virtual int getUIDepth();

	void setunfreezing();
protected:
	bool	isAlreadyShow;
	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;
	JsonInfo	* jsonInfo;

	MachineActor *	machine;
	GAFObject *		machineObject;

	int				uiDepth;

	int				EXAMPLE_WIDTH = 300;
	int				EXAMPLE_HEIGHT = 300;
	std::vector<GAFObject *> touchObject;

	std::vector<SimpleTouchActor *>actorList;
	//std::vector<WordActor *>wordActorList;
	//std::vector<SimpleWordActor *>wordActorList;

	std::string currentExamplePath;
	std::string lastExamplePath;

	int examplePosition[3];

	int lastAddedIndex;
	int touchIndex;
	bool isFadeRun = false;
	bool isFreezing = true;
	float FADE_OUT_SPEED = 0.5f;

	FadeOut * fadeAction;
	FadeOut * holderAction;
};

#endif