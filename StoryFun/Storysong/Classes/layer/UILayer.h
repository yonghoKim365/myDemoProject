#ifndef _UI_LAYER_H_
#define _UI_LAYER_H_

#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

using namespace ui;

class SkipButtonEventListener
{
public:
	virtual void onSkipScene() = 0;
};

class UILayer : public Layer
{
public:
	static UILayer * create();

public:
	void showSkipButton(bool isShowAffodance);
	void setSkipButtonEventListener(SkipButtonEventListener * listener);
	void makeStep2Scene();
	void hideSkipButton();
	void showExitButton();
private:
	void initGUI();
	void exitGame();
	void skipScene();
	
private:
	Button * skipButton;
	//GAFObject * skipEffect;
	SkipButtonEventListener * skipListener;
	bool isSkipPressed;

	bool isHideSkip = false;
};

#endif