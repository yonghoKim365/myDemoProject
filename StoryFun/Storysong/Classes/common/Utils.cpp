#include "Utils.h"
#include "audio/include/AudioEngine.h"
#include "ui/CocosGUI.h"
#include "data/def.h"

USING_NS_CC;
using namespace ui;
using namespace experimental;

Button * SimpleButton::create(SimpleButtonEventListener * listener, int uinqId,
	const std::string& normalImage,
	const std::string& selectedImage,const std::string& disableImage ,TextureResType texType)
{
	SimpleButton *btn = new (std::nothrow) SimpleButton;
	if (btn && btn->init(normalImage, selectedImage, disableImage, texType))
	{
		btn->initEvent(listener, uinqId);
		btn->autorelease();
		return btn;
	}
	CC_SAFE_DELETE(btn);
	return nullptr;

}

void SimpleButton::initEvent(SimpleButtonEventListener * listener, int uinqId)
{
	buttonID = uinqId;
	eventListener = listener;

	this->addTouchEventListener([&](Ref* sender, TouchEventType type){
		switch (type)
		{
		case TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case TouchEventType::ENDED:
			eventListener->onButtonClick(buttonID);
			break;
		}
	});
}

std::string * Utils::tringSplit(std::string strTarget, std::string strTok)
{
	int     nCutPos;
	int     nIndex = 0;
	std::string* strResult = new std::string[256];

	while ((nCutPos = strTarget.find_first_of(strTok)) != strTarget.npos)
	{
		if (nCutPos > 0)
		{
			strResult[nIndex++] = strTarget.substr(0, nCutPos);
		}
		strTarget = strTarget.substr(nCutPos + 1);
	}

	if (strTarget.length() > 0)
	{
		strResult[nIndex++] = strTarget.substr(0, nCutPos);
	}

	return strResult;
}

void Utils::moveToScene(Scene * nextScene)
{
	TransitionFade* fade = TransitionFade::create(0.3f, nextScene, Color3B::BLACK);
	Director::getInstance()->replaceScene(fade);
}

void Utils::timeLog(std::string message)
{
#if(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	struct tm *tm;
	time_t timep;

	timeval tv;
	cocos2d::gettimeofday(&tv, nullptr);
	timep = tv.tv_sec;

	tm = localtime(&timep);
	int min = tm->tm_min;
	int second = tm->tm_sec;

	CCLOG("%s %d:%d", message.c_str(), min, second);
#else
	CCLOG("%s", message.c_str());
#endif
}