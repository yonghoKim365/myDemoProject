#ifndef __KSPRITEBUTTON_H__
#define __KSPRITEBUTTON_H__

#include "cocos2d.h"
USING_NS_CC;
using namespace std;

class KSpriteButton : public cocos2d::Sprite
{
public:
	~KSpriteButton();
	KSpriteButton(bool aShouldActLikeButton = false);
	static KSpriteButton * create(bool aShouldActLikeButton = false);
	void setCallback(function<void(Touch * pTouch, Event * pEvent)> aCallback);
	void addEvents();
	void removeEvents();
private:
	void touchEvent(cocos2d::Touch * touch, Event * event);
	std::function<void(Touch * pTouch, Event * pEvent)> mCallback;
	bool shouldActLikeButton;
	EventListenerTouchOneByOne * mpEventListener;
};

#endif // __KSPRITEBUTTON_H__