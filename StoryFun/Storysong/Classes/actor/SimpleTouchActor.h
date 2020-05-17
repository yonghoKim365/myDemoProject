#ifndef _SIMPLE_TOUCH_ACTOCR_H_
#define _SIMPLE_TOUCH_ACTOCR_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

class SimpleTouchActor : public cocos2d::Ref
{
public:
	SimpleTouchActor();
	~SimpleTouchActor();

	bool init(gaf::GAFObject* object, bool autoReply);

	static SimpleTouchActor * create(gaf::GAFObject* object, bool autoReply = true);
	void startBasicAnimation(bool autoReply = true);
	void startTouchAnimation(bool autoReply = true);
	void startExitAnimation(bool autoReply = true);

	void onFinishedBasicSequence(gaf::GAFObject* object);
	void onFinishedTouchSequence(gaf::GAFObject* object);
	void onFinishedExitSequence(gaf::GAFObject* object);

	void setVisible(bool visible);
public:
	gaf::GAFObject * gafObj;
	bool			isPlay;
};












#endif