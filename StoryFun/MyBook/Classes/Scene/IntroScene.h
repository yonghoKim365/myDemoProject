#ifndef __INTRO_SCENE_H__
#define __INTRO_SCENE_H__

#pragma once
#include "cocos2d.h"

USING_NS_CC;

class IntroScene : public Layer
{
private:
	int mWeekData = 1;
	bool mUsedWeekData = false;
	bool mEnableTouch = false;
	//bool mMultiTouch = false;
	Sequence* mSeqAction = nullptr;
	void initLayout();
	// 프리로딩
	void preLoadingData();
	void preLoadingImage();
	void preLoadingSound();

	// 메인씬으로 이동
	void toScene(Node* pSender);

	void loadingCallBack(cocos2d::Texture2D *texture);
	void checkWeekData();
	void playNarTitle(Node* pSender);
	void finishNarTitleCallback();

//	void delayTitle(Ref* pSender);
    void delayTitle();

	////////////////////////////////////////
	// 터치 이벤트 리스너
	EventListenerTouchAllAtOnce* mListener;

	virtual void onTouchesBegan(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesMoved(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesEnded(const std::vector<Touch*>& touches, Event* event);
	//virtual void onTouchesCancelled(const std::vector<Touch*>& touches, Event* event);

	void initTouchEvent();
	// a selector callback
	void menuNextSceneCallback(Ref* pSender);
	void menuCloseCallback(Ref* pSender);
	bool mUseTouch = false;
	int mResCount = 0;

public:
	
	static Scene* createScene(int weekData);
	void delayScene(Node* pSender);
//	void delayScene();
	void displayScene();

	void initData();
	virtual bool init();
	virtual void onExit();

	void exitPlay();
	// implement the "static create()" method manually
	CREATE_FUNC(IntroScene);

	// 플레이 횟수 체크
	bool setPlayTime();
};

#endif // __INTRO_SCENE_H__

