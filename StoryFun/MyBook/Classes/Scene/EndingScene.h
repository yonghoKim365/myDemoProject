#ifndef _ENDING_SCENE_H_
#define _ENDING_SCENE_H_

#include "cocos2d.h"
#include "Cocos2dxGAFPlayer-release-gaf-5/Library/Sources/GAF.h"
#include "common/AssetUtils.h"

USING_NS_CC;
USING_NS_GAF;

class EndingEventListener
{
public:
	virtual void onClickRefresh() = 0;
	virtual void onClickNext() = 0;
	virtual void onClickExit() = 0;
};

class EndingPopupCloseEventListener
{
public:
	virtual void onCloseEnding() = 0;
};

class EndingScene : public Layer
{
public:
	static Scene * createScene(EndingPopupCloseEventListener * endingListener);
	static Scene * createScene();
	virtual bool init();

	CREATE_FUNC(EndingScene);

public:
	void setClickEventListener(EndingEventListener * listener);
	void setEndingPopupCloseEvent(EndingPopupCloseEventListener * endingListener);

private:
	void initData();
	void initGUI();
	void initEvent();
	void onFinishedAnimation(gaf::GAFObject * object);
	void onFinishedWordAnimation(gaf::GAFObject * object);
	void actionUpEvent(cocos2d::Touch* touch, Vec2 _p);
	void actionDownEvent(cocos2d::Touch* touch, Vec2 _p);
	bool isButtonClick(GAFObject * button, cocos2d::Touch* touch);
	void exitGame();
	void nextGame();
	void replayGame();
	void finishWeek();
	void finishPortfolio();

	void startNarration();
	void startEffect();
	void startStarAnimation();
private:
	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;

	EndingEventListener * _clickListener;
	EndingPopupCloseEventListener * _closeListener;

	int endingIndex;
	std::string soundPath[4];
	std::string labelName[4];
	GAFObject * mainObject;

	GAFObject * pressedObject;
	GAFObject * title;

	GAFObject * charector_mc;
	GAFObject * star_mc;
	GAFObject * word_mc;

	GAFBUtton * reply_btn;
	GAFBUtton * next_btn;
	GAFBUtton * exit_btn;

	int effectID;
	int narrationID;
	std::string mcNames[4];
};

#endif