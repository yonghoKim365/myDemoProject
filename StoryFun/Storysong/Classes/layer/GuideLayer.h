#ifndef __GUIDE__LAYER__
#define __GUIDE__LAYER__

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

class GuideFInishedEventListener
{
public:
	virtual void onFInishedGuideAnimation() = 0;
};



class GuideLayer : public LayerColor
{
public:
	GuideLayer();
	~GuideLayer();

	static GuideLayer * create(std::string guidePath, GuideFInishedEventListener * listener);
	virtual bool onTouchBegan(cocos2d::Touch* touch, cocos2d::Event* event);
	virtual void onEnter();
	virtual void onExit();
private:
	void initLayer(std::string guidePath, GuideFInishedEventListener * listener);
	void onIntroGuideFinished(GAFObject * object);
private:
	GuideFInishedEventListener * finishedListener;
	EventListenerTouchOneByOne* _touchListener;
	GAFObject *	introObject;
};
#endif