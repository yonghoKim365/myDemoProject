#include "MachineActor.h"

USING_NS_GAF;

MachineActor::MachineActor() {}
MachineActor::~MachineActor() {}


MachineActor* MachineActor::create(gaf::GAFObject* object, bool autoReply)
{
	MachineActor * ref = new MachineActor();
	if (ref && ref->init(object, autoReply))
	{
		ref->autorelease();
		return ref;
	}

	CC_SAFE_RELEASE(ref);
	return nullptr;
}

bool MachineActor::init(gaf::GAFObject* object, bool autoReply)
{
	if (object == nullptr)
	{
		return false;
	}

	isPlay = false;
	gafObj = object;
	gafObj->gotoAndStop(0);
	//gafObj->playSequence("Basic", true);
	return true;
}

void MachineActor::isFirstPlay(bool value)
{
	isPlay = value;
}

void MachineActor::startReadyAnimation()
{
	gafObj->playSequence("Ready", false);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(MachineActor::onFinishedReadySequence, this)));

}

void MachineActor::stop()
{
	gafObj->gotoAndStop(1);
}

void MachineActor::play()
{
	gafObj->playSequence("Basic", true);
}

void MachineActor::startBasicAnimation(bool autoReply)
{
	gafObj->playSequence("Basic", true);
}

void MachineActor::startTouchAnimation(bool autoReply)
{
	gafObj->playSequence("Touch", false);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationStartedNextLoopDelegate_t(CC_CALLBACK_1(MachineActor::onFinishedTouchSequence, this)));
}

void MachineActor::onFinishedReadySequence(gaf::GAFObject* object)
{
	gafObj->playSequence("Basic", true);
}

void MachineActor::onFinishedTouchSequence(gaf::GAFObject* object)
{
	gafObj->playSequence("Basic", true);
}

void MachineActor::changeMotion()
{
	auto ball = gafObj->getObjectByName("colorBall");

	int frame = ball->getCurrentFrameIndex();
	int nextFrame = frame + 1;
	if (nextFrame == 3)
		nextFrame = 0;

	ball->gotoAndStop(nextFrame);
}