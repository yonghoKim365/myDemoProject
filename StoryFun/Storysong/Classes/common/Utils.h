#ifndef _UTILS_H_
#define _UTILS_H_

#include "cocos2d.h"
#include "ui/CocosGUI.h"

USING_NS_CC;
using namespace cocos2d::ui;

class SimpleButtonEventListener
{
public:
	virtual void onButtonClick(int buttonId) = 0;
};

class SimpleButton : public Button
{
public:
	static Button* create(SimpleButtonEventListener * listener, int uinqId,
		const std::string& normalImage,
		const std::string& selectedImage = "",
		const std::string& disableImage = "",
		TextureResType texType = TextureResType::LOCAL);

private:
	void initEvent(SimpleButtonEventListener * listener, int uinqId);

private:
	int buttonID;
	SimpleButtonEventListener * eventListener;

};

class Utils
{
public:
	static std::string * tringSplit(std::string strTarget, std::string strTok);
	static void moveToScene(Scene * nextScene);
	static void timeLog(std::string message);
};
#endif