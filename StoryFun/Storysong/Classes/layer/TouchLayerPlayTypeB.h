#ifndef _TOUCH_LAYER_TYPE_B_H_
#define _TOUCH_LAYER_TYPE_B_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/def.h"
#include "data/JsonInfo.h"
#include "actor/SimpleTouchActor.h"
#include "actor/MachineActor.h"
#include "actor/WordActor.h"
#include "actor/ExampleActor.h"
#include "layer/AbsTouchLayer.h"

USING_NS_CC;
USING_NS_GAF;

class TouchLayerPlayTypeB : public AbsTouchLayer
{
public:
	static TouchLayerPlayTypeB * create();

	virtual void onPlaybyTickEvent(int tickCount) override;
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode) override;
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode) override;

	virtual void onWordExampleExit(int id);
	virtual void onWordExampleExit2(int id);

public:
	void resetLayer() override;
	void startAnimation() override;
	void removeTouchEvent() override;

public:
	void initData();
	void initGUI();
	void initEvent();
	void touchEvent(cocos2d::Touch* touch, Vec2 _p);
	//bool isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch);
	int getUIDepth();
	void onFinishedTouchAnimation(gaf::GAFObject * object);
	void onFinishedExampleAnimation(gaf::GAFObject * object);

protected:
	bool	isAlreadyShow;
	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;
	JsonInfo	* jsonInfo;

	MachineActor *	machine;
	GAFObject *		machineObject;

	int				uiDepth;

	std::vector<GAFObject *> touchObject;

	std::vector<SimpleTouchActor *>actorList;
	//std::vector<SimpleWordActor *>wordActorList;
	std::vector<ExampleActor * > exampleList;

	std::string currentExamplePath;
	std::string lastExamplePath;

	int examplePosition[3];

	GAFObject * exampleAnim;
	//
	std::vector<GAFObject *>contentsHolderList;
	std::vector<GAFObject *>itemHolderList;
	std::string				oldExampleName = "not added";

private:
	void preloadExample();
};
#endif