#ifndef _TOUCH_LAYER_PLAY_H_
#define _TOUCH_LAYER_PLAY_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/def.h"
#include "data/JsonInfo.h"
#include "actor/SimpleTouchActor.h"
#include "actor/MachineActor.h"
#include "actor/WordActor.h"

#include "layer/AbsTouchLayer.h"

USING_NS_CC;
USING_NS_GAF;

class TouchLayerPlay : public AbsTouchLayer
{
public:
	static TouchLayerPlay * create();

	virtual void onPlaybyTickEvent(int tickCount) override;
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode) override;
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode) override;

	virtual void onWordExampleExit(int id) override;
	virtual void onWordExampleExit2(int id) override;
public:
	void resetLayer() override;
	void startAnimation() override;
	void removeTouchEvent() override;
private:
	void initData();
	void initGUI();
	void initEvent();
	void touchEvent(cocos2d::Touch* touch, Vec2 _p);
	bool isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch);
	int getUIDepth();

private:
	bool	isAlreadyShow;
	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;
	JsonInfo	* jsonInfo;

	MachineActor *	machine;
	GAFObject *		machineObject;

	int				uiDepth;

	std::vector<GAFObject *> touchObject;

	std::vector<SimpleTouchActor *>actorList;
	std::vector<WordActor *>wordActorList;

	std::string currentExamplePath;
	std::string lastExamplePath;

	int examplePosition[3];

};
#endif