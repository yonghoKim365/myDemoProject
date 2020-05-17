#ifndef _TOUCH_LAYER_LISTEN_H_
#define _TOUCH_LAYER_LISTEN_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/def.h"
#include "data/JsonInfo.h"
#include "actor/ListenObjectActor.h"

USING_NS_CC;
USING_NS_GAF;

class TouchLayerListen : public Layer, 
						public IBaiscSongPlay, public ListenAnimationFinishedEventListener
{
public:
	TouchLayerListen();
	~TouchLayerListen();

	static TouchLayerListen * create();

	virtual void onPlaybyTickEvent(int tickCount);
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode);
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode);

	virtual void onFinishedAnimation(GAFObject * gafObject);

	void setFreezing(bool freez);
public:
	void resetLayer();
	void removeTouchEvent();
	void setTouchEnable();
private:
	void initData();
	void initGUI();
	void initEvent();
	void touchEvent(cocos2d::Touch* touch, Vec2 _p);
	bool isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch);

private:
	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;
	int			bubbleId = 0;
	int			currentBubbleCount;
	int			maxBubbleCount;

	JsonInfo	* jsonInfo;

	std::vector<GAFObject *> objectList;
	std::vector<ListenObjectActor*> actorList;

	GAFAsset	* bubbleAsset;
	bool			isFreezing;
};
#endif



