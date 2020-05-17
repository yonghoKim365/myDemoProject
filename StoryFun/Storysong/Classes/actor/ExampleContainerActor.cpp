#include "ExampleContainerActor.h"

ExampleContainerActor * ExampleContainerActor::create(GAFObject * object)
{
	ExampleContainerActor * ref = new ExampleContainerActor();
	if (ref && ref->init(object))
	{
		ref->autorelease();
		return ref;
	}
	else
	{
		CC_SAFE_RELEASE(ref);
		return nullptr;
	}
}

bool ExampleContainerActor::init(gaf::GAFObject* object)
{
	if (object == nullptr)
	{
		return false;
	}

	gafObj = object;
	gafObj->gotoAndStop(1);
	
	initData();

	return true;
}

void ExampleContainerActor::initData()
{

}