#pragma once
#ifndef __END_SCENE_H__
#define __END_SCENE_H__

#include "cocos2d.h"
#include "SimpleAudioEngine.h"
#include "AudioEngine.h"
#include "GAF.h"
#include "Page.h"

NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;
USING_NS_CC;

#define TIME_ENDSND	3.0f

class Actor;
//enum Q_TYPE : short;

class EndScene : public Layer
{
public:
	EndScene();
	~EndScene();

	static Scene* createScene();
	//CREATE_FUNC(EndScene);
	static EndScene* create();

	virtual bool init();
	virtual void onEnter();
	virtual void onExit();

private:
	/**
	 *	Touch Function
	 */
	bool onTouchBegan(Touch* pTouch, Event* pEvent);
	void onTouchMoved(Touch* pTouch, Event* pEvent);
	void onTouchEnded(Touch* pTouch, Event* pEvent);
	void onTouchCancelled(Touch* pTouch, Event* pEvent);
	/**
	 *	Get Function
	 */
	Actor* getItemForTouch(Touch *touch);
	/**
	 *	Set Function
	 */
	void setBackGround();
	/**
	*	Callback Function
	*/
	void animFinishedCallback(int audioID, const std::string & filePath);
	void sndCallback();
	/**
	 *	Functions
	 */
	void selected(Actor* target);
	void unselected(Actor* target);
	void activate(Actor* target);
	/**
	 *	Variable
	 */
	EventListenerTouchOneByOne *_listener;
	Actor *replayBtn, *nextBtn, *exitBtn, *_selectedActor;
	Size winSize;
	Rect replayBtnRect, nextBtnRect, exitBtnRect;
	//Q_TYPE _qType;
	std::string _volume, _sndPath;
	//int _day;
	int _sndCount, _idx;
	//int _idx;
	bool _touchEnabled = false;

	//GAFAsset* getAssetByName(std::string path);
	GAFObject* createObjectAndRun(std::string path, bool isRun, const Vec2 &pnt);
	int	playSound(std::string sFile);
};

#endif // __END_SCENE_H__