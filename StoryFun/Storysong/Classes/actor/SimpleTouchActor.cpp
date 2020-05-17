#include "SimpleTouchActor.h"

SimpleTouchActor::SimpleTouchActor()
{

}

SimpleTouchActor::~SimpleTouchActor()
{

}

SimpleTouchActor * SimpleTouchActor::create(gaf::GAFObject* object, bool autoReply)
{
	SimpleTouchActor * ref = new SimpleTouchActor();
	if (ref && ref->init(object, autoReply))
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

bool SimpleTouchActor::init(gaf::GAFObject* object, bool autoReply)
{
	if (object == nullptr)
	{
		return false;
	}
	
	gafObj = object;
	gafObj->gotoAndStop(1);
	isPlay = true;
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(SimpleTouchActor::onFinishedBasicSequence, this)));
	return true;
}

void SimpleTouchActor::startBasicAnimation(bool autoReply)
{
	gafObj->playSequence("Basic", autoReply);
	//gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(SimpleTouchActor::onFinishedBasicSequence, this)));
}

void SimpleTouchActor::startTouchAnimation(bool autoReply)
{
	gafObj->playSequence("Touch", false);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(SimpleTouchActor::onFinishedTouchSequence, this)));
}

void SimpleTouchActor::startExitAnimation(bool autoReply)
{
	gafObj->playSequence("Exit", autoReply);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(SimpleTouchActor::onFinishedExitSequence, this)));
}

void SimpleTouchActor::onFinishedBasicSequence(gaf::GAFObject* object)
{
	
}

void SimpleTouchActor::onFinishedTouchSequence(gaf::GAFObject* object)
{	
	gafObj->playSequence("Basic", true);
}

void SimpleTouchActor::onFinishedExitSequence(gaf::GAFObject* object)
{
	
}

void SimpleTouchActor::setVisible(bool visible)
{
	gafObj->setVisible(visible);
}