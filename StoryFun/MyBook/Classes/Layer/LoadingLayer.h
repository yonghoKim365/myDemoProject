#ifndef __LOADING_LAYER_H__
#define __LOADING_LAYER_H__

#pragma once
#include "cocos2d.h"
#include "Contents/MyBookResources.h"
#include "Util/Pos.h"


USING_NS_CC;


class LoadingLayer : public Layer
{
private:
	Node* mParent;
	int mWeekData;
	int mOrderNum;
	Sprite* mBaseSp = nullptr;
	Sprite* mSpAnimate = nullptr;
	Animate* mAnimate = nullptr;
	
public:	
	static LoadingLayer* createLayer(Node* parent, int weekData, int ord);
	
	virtual bool init();
	void initLayout();
	void show(bool b);
	//void createAnimation();
	//virtual void onExit();

	// a selector callback	
	
	// implement the "static create()" method manually
	CREATE_FUNC(LoadingLayer);	
};

#endif // __LOADING_LAYER_H__

