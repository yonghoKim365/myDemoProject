#ifndef _LISTEN_OBJECT_ACTOR_H_
#define _LISTEN_OBJECT_ACTOR_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/JsonInfo.h"

USING_NS_CC; 
USING_NS_GAF;

class ListenAnimationFinishedEventListener
{
public:
	virtual void onFinishedAnimation(GAFObject * gafObject) = 0;
};

class ListenObjectActor : public Ref
{
public:
	ListenObjectActor();
	~ListenObjectActor();

	static ListenObjectActor * create(GAFObject * object, ListenAnimationFinishedEventListener * listener);

public:
	void startExitAnimation();

private:
	bool init(GAFObject * object, ListenAnimationFinishedEventListener * listener);
	void onFinishedIntroAnimation(GAFObject * object);
	void onFinishedTouchAnimation(GAFObject * object);

private:
	bool	isAnimationRun = false;
	GAFObject *		gafObject;
	GAFObject *		imageHolder;
	
	JsonInfo	*	jsonInfo;
	ListenAnimationFinishedEventListener * animationListener;
};
#endif