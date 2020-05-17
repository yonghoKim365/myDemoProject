#ifndef _EXAMPLE_CONTAINER_ACTOR_H_
#define _EXAMPLE_CONTAINER_ACTOR_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

class ExampleContainerActor : public Ref
{
public:
	static ExampleContainerActor * create(GAFObject * object);

	bool init(gaf::GAFObject* object);

private:
	void initData();

private:
	bool		isFirstShow;
	GAFObject * gafObj;
};

#endif