#include "ExampleActor.h"
#include "GAF/Library/Sources/GAFTimelineAction.h"
#include "audio/include/AudioEngine.h"
#include "data/PlayInfo.h"

#define FADE_OUT_SPEED	0.5f

using namespace experimental;

ExampleActor * ExampleActor::create(GAFObject * obj, int index)
{
	ExampleActor * ref = new ExampleActor();
	if (ref && ref->init(obj, index))
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

bool ExampleActor::init(GAFObject * obj, int index)
{
	if (obj)
	{
		exampleAnimation = obj;
		exampleAnimation->retain();
		exampleAnimation->autorelease();
	}
	else
	{
		return false;
	}

	exampleIndex = index;

	contentsHolder = exampleAnimation->getObjectByName("ContentsContainer");
	contentsHolder->setVisible(true);
	contentsHolder->gotoAndStop(1);

	exampleAnimation->setVisible(false);

	isAddedExample = false;
	isPlayedExample = false;
	isFadeRun = false;

	jsonInfo = JsonInfo::create();

	return true;
}

int	ExampleActor::getIndexId()
{
	return exampleIndex;
}

void ExampleActor::addExample(int index, int frame)
{
	int lastSound = PlayInfo::create()->getLastSoundID();
	if (lastSound != -1)
	{
		AudioEngine::stop(lastSound);
	}

	hideExample(frame);

	isAddedExample = true;
	contentsHolder->setVisible(true);
	std::string newExampleName = StringUtils::format("example_%d", exampleIndex);
	oldExampleName = newExampleName;

	auto wordAsset = AssetUtils::createAutoSoundAsset(index);
	exampleObject = wordAsset->createObjectAndRun(true);

	if (exampleObject)
	{
		exampleObject->retain();
		exampleObject->autorelease();

		if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_B)
			exampleObject->setPosition(Vec2((EXAMPLE_WIDTH / 2) * -1, EXAMPLE_HEIGHT - 100));
		else
			exampleObject->setPosition(Vec2((EXAMPLE_WIDTH / 2) * -1, (EXAMPLE_HEIGHT / 2)));

		

		exampleObject->setName(newExampleName);
		exampleObject->gotoAndStop(1);
		contentsHolder->addChild(exampleObject);
	}
	
}

void ExampleActor::hideExample(int frame)
{
	if (isFadeRun)
	{
		isFadeRun = false;
		exampleObject->stopAction(fadeAction); 
		exampleObject->setOpacity(255);
	}

	if (isAddedExample)
	{
		contentsHolder->removeChildByName(oldExampleName);
		
		isAddedExample = false;
	}

	if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
		contentsHolder->setVisible(false);
}

void ExampleActor::startIntroAnimation()
{
	exampleAnimation->setVisible(true);
	contentsHolder->gotoAndStop(1);
	if (!isPlayedExample)
	{
		exampleAnimation->playSequence("Ready", true);
	}
	else
	{
		exampleAnimation->playSequence("Basic", true);
	}

	isPlayedExample = true;
}

void ExampleActor::startTouchAnimation()
{
	//exampleAnimation->playSequence("Touch", false);
	//exampleAnimation->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActor::onFinishedTouchAnimation, this)));

	AudioEngine::play2d(jsonInfo->getEffectSoundPath().c_str());

	exampleAnimation->playSequence("Exit", false);
	contentsHolder->playSequence("Touch", false);
	exampleAnimation->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActor::onFinishedExitAnimation, this)));
	if (isAddedExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActor::onFinishedExampleAnimation, this)));
	}
}

void ExampleActor::onFinishedTouchAnimation(GAFObject * object)
{
	AudioEngine::play2d(jsonInfo->getEffectSoundPath().c_str());

	exampleAnimation->playSequence("Exit", false);
	contentsHolder->playSequence("Touch", false);
	exampleAnimation->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActor::onFinishedExitAnimation, this)));
	if (isAddedExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActor::onFinishedExampleAnimation, this)));
	}
	
}

void ExampleActor::onFinishedExitAnimation(GAFObject * object)
{
	exampleAnimation->setAnimationFinishedPlayDelegate(nullptr);
}

void ExampleActor::onFinishedExampleAnimation(GAFObject * object)
{
	exampleObject->setAnimationFinishedPlayDelegate(nullptr);
	
	isFadeRun = true;

	fadeAction = FadeOut::create(FADE_OUT_SPEED);
	exampleObject->runAction(fadeAction);
}

ExampleActorBall * ExampleActorBall::create(GAFObject * obj, int index)
{
	ExampleActorBall * ref = new ExampleActorBall();
	if (ref && ref->init(obj, index))
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


bool ExampleActorBall::init(GAFObject * obj, int index)
{
	obj->gotoAndStop(1);

	bool result =  ExampleActor::init(obj, index);

	ballFront = exampleAnimation->getObjectByName("Ball_Front");
	ballBack = exampleAnimation->getObjectByName("Ball_Back");
	
	exampleAnimation->getEventDispatcher()->addCustomEventListener("reset", CC_CALLBACK_1(ExampleActorBall::onFrameResetEvent, this));

	srand((unsigned int)time(NULL));
	int frame = (rand() % 5) + 1;
	setRandomFrame(frame);

	return result;
}

void ExampleActorBall::onFrameResetEvent(void * params)
{

}

void ExampleActorBall::addExample(int index, int frame)
{
	
	exampleAnimation->setAnimationFinishedPlayDelegate(nullptr);
	exampleAnimation->gotoAndStop(0);
	exampleAnimation->setVisible(true);
	
	int current = exampleAnimation->getCurrentFrameIndex();
	
	ExampleActor::addExample(index, frame);

	setRandomFrame(frame);
}

void ExampleActorBall::hideExample(int frame)
{
	int current = exampleAnimation->getCurrentFrameIndex();
	
	exampleAnimation->gotoAndStop(0);
	setRandomFrame(frame);

	ExampleActor::hideExample(frame);
}

void ExampleActorBall::setRandomFrame(int frame)
{
	currentFrame = frame;
	
	exampleAnimation->getObjectByName("Ball_Front")->retain();
	exampleAnimation->getObjectByName("Ball_Back")->retain();
	exampleAnimation->getObjectByName("ContentsContainer")->retain();

	exampleAnimation->getObjectByName("Ball_Front")->gotoAndStop(frame);
	exampleAnimation->getObjectByName("Ball_Back")->gotoAndStop(frame);
	exampleAnimation->getObjectByName("ContentsContainer")->gotoAndStop(frame);

}

void ExampleActorBall::startTouchAnimation ()
{
	AudioEngine::play2d(jsonInfo->getEffectSoundPath().c_str());

	auto color = ballFront->getCurrentFrameIndex();
	std::string frameLabel = StringUtils::format("Exit%d", color + 1);
	exampleAnimation->playSequence(frameLabel.c_str(), false);
	exampleAnimation->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActorBall::onFinishedExitAnimation, this)));

	if (isAddedExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActorBall::onFinishedExampleAnimation, this)));
	}


}

void ExampleActorBall::onFinishedTouchAnimation(GAFObject * obj)
{
	AudioEngine::play2d(jsonInfo->getEffectSoundPath().c_str());

	auto color = ballFront->getCurrentFrameIndex();
	std::string frameLabel = StringUtils::format("Exit%d", color + 1);
	exampleAnimation->playSequence(frameLabel.c_str(), false);
	exampleAnimation->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActorBall::onFinishedExitAnimation, this)));

	if (isAddedExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ExampleActorBall::onFinishedExampleAnimation, this)));
	}


}

void ExampleActorBall::onFinishedExitAnimation(GAFObject * obj)
{
	//ExampleActor::onFinishedExitAnimation(obj);
	exampleAnimation->setAnimationFinishedPlayDelegate(nullptr);
	exampleAnimation->retain();
}

void ExampleActorBall::onFinishedExampleAnimation(GAFObject * obj)
{
	exampleAnimation->retain();
	exampleObject->setAnimationFinishedPlayDelegate(nullptr);

	isFadeRun = true;

	fadeAction = FadeOut::create(FADE_OUT_SPEED);
	exampleObject->runAction(fadeAction);

	//ExampleActor::onFinishedExampleAnimation(obj);
	/*exampleAnimation->gotoAndStop(1);
	exampleAnimation->setVisible(false);*/
}