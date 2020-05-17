#include "WordActor.h"
#include "common/AssetUtils.h"
#include "audio/include/AudioEngine.h"
#include "data/PlayInfo.h"
#include "common/Utils.h"

using namespace experimental;


#define FADE_OUT_SPEED	0.5f

WordActor::WordActor() {}
WordActor::~WordActor(){}

WordActor * WordActor::create(gaf::GAFObject * object, int index, WordActorEventListener * _listener,bool visible)
{
	WordActor * ref = new WordActor();
	if (ref && ref->init(object, index, _listener, visible))
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

bool WordActor::init(gaf::GAFObject * object, int index, WordActorEventListener * _listener, bool visible)
{
	_eventListener = _listener;

	jsonInfo = JsonInfo::create();
	isFadeRun = false;
	wordIndex = index;
	
	wordVisibleMode = visible;
	gafObj = object;

	imageHolder = gafObj->getObjectByName("imageHolder");
	isAddedExample = false;
	isFirstPlay = true;
	return true;
}

void WordActor::setLoadEventListener(PreLoadEventListener * listener)
{
	eventListener = listener;
}


void WordActor::onExampleAnimationFinished()
{
}

void WordActor::showWordExample(int index)
{
	// TODO: 예제 추가 
	isHaveExample = true;
	createExampleAndAdd(index);
}

void WordActor::createExampleAndAdd(int index)
{
	if (isAddedExample)
		exampleObject->setAnimationFinishedPlayDelegate(nullptr);

	if (isFadeRun)
	{
		gafObj->stopAction(fadeAction);
		gafObj->setOpacity(255);

		exampleObject->stopAction(holderAction);
		isFadeRun = false;
	}
	
	if (isAddedExample)
	{
		imageHolder->removeChildByName(oldExampleName);
		gafObj->setAnimationFinishedPlayDelegate(nullptr);
	}
	
	exampleIndex = index;

	std::string newExampleName = StringUtils::format("example_%d", exampleIndex);
	oldExampleName = newExampleName;

	std::string path = jsonInfo->trackExampleList.at(index);

	if (path.length() > 0)
	{
		//auto wordAsset = AssetUtils::createAutoSoundAsset(path.c_str());
		auto wordAsset = AssetUtils::createAutoSoundAsset(index);
		exampleObject = wordAsset->createObjectAndRun(true);
		
		if (exampleObject)
		{
			CCLOG("exampleObject was not null");
			exampleObject->retain();
			exampleObject->autorelease();
		}
	}

	if (exampleObject == nullptr)
	{
		imageHolder->setVisible(false);
		exampleObject->setVisible(false);
	}
	else
	{
		int lastEffectId = PlayInfo::create()->getLastEffectID();
		if (lastEffectId != -1)
		{
			cocos2d::experimental::AudioEngine::stop(lastEffectId);
		}
		exampleObject->setName(newExampleName);
		exampleObject->setPosition(Vec2(-150, 170));
		imageHolder->addChild(exampleObject, 1);
		exampleObject->setVisible(true);
		exampleObject->gotoAndStop(1);
		imageHolder->setVisible(true);
		
		isAddedExample = true;
		
	}
}

void WordActor::setHide()
{
	gafObj->gotoAndStop(1);
	gafObj->setVisible(false);
}

void WordActor::setFirstPlayValue(bool value)
{
	isFirstPlay = value;
}

void WordActor::startReadyAnimation()
{
	gafObj->setVisible(true);
	gafObj->playSequence("Ready", true);

}

void WordActor::startIntroAnimation()
{
	gafObj->setVisible(true);
	gafObj->playSequence("Intro", false);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedIntroAnimation, this)));
}

void WordActor::startExampleAnimation()
{
	// TODO: 예제 존재 여부에 따른 분기 대응 
	
	isFirstPlay = false;
	startTouchAnimation();
	
}

void WordActor::startBasicAnimation(bool autoReply)
{
	gafObj->setVisible(true);
	gafObj->playSequence("Basic", false);
}

void WordActor::hideExample()
{
	if (isAddedExample)
		exampleObject->setAnimationFinishedPlayDelegate(nullptr);

	isHaveExample = false;
	if (isFadeRun)
	{
		gafObj->stopAction(fadeAction);
		isFadeRun = false;
	}
	gafObj->setOpacity(255);
	imageHolder->setVisible(false);
}

void WordActor::startTouchAnimation(bool autoReply)
{
	lastExplodeSoundId = PlayInfo::create()->getLastEffectID();
	if (lastExplodeSoundId != -1)
	{
		AudioEngine::stop(lastExplodeSoundId);
	}

	isFirstPlay = false;
	
	PlayInfo::create()->setLastEffectID(AudioEngine::play2d(EFFECT_EXPLODE_PATH));
	gafObj->setVisible(true);
	gafObj->playSequence("Exit", false);
	//gafObj->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedTouchAnimation, this)));

	if (isHaveExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedExampleAnimation, this)));
	}
}

void WordActor::startExitAnimation(bool autoReply)
{
	isFirstPlay = false;
	
	gafObj->setVisible(true);
	gafObj->playSequence("Exit", false);
	gafObj->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedExitAnimation, this)));
}

void WordActor::startExit2Animation(bool autoReply)
{
	gafObj->setVisible(true);
	gafObj->playSequence("Exit2", false);
	//gafObj->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedExit2Animation, this)));
}

bool WordActor::getFirstPlay()
{
	return isFirstPlay; 
}
void WordActor::onFinishedIntroAnimation(gaf::GAFObject * object)
{
	if (isHaveExample)
		imageHolder->setVisible(true);
	else
		imageHolder->setVisible(false);

	gafObj->setAnimationFinishedPlayDelegate(nullptr);
	if (isFirstPlay)
	{
		gafObj->playSequence("Ready", true);
		//gafObj->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedReadyAnimation, this)));
	}
	else
	{
		gafObj->playSequence("Basic", false);
	}
	/**/
}

void WordActor::onFinishedReadyAnimation(gaf::GAFObject * object)
{
	gafObj->playSequence("Basic", false);
}

void WordActor::onFinishedTouchAnimation(gaf::GAFObject * object)
{
	gafObj->setAnimationFinishedPlayDelegate(nullptr);
	gafObj->playSequence("Exit", false);
	//isFirstPlay = false;

	if (isHaveExample)
	{
		exampleObject->playSequence("Touch", false);
		exampleObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(WordActor::onFinishedExampleAnimation, this)));
	}
}

void WordActor::onFinishedExampleAnimation(gaf::GAFObject * object)
{
	exampleObject->setAnimationFinishedPlayDelegate(nullptr);
	CCLOG("CALL WordActor::onFinishedExampleAnimation");
	fadeAction = FadeOut::create(FADE_OUT_SPEED);
	gafObj->runAction(fadeAction);
	//exampleObject->runAction(fadeAction);

	holderAction = FadeOut::create(FADE_OUT_SPEED);
	exampleObject->runAction(holderAction);

	isFadeRun = true;
}

void WordActor::onFinishedExitAnimation(gaf::GAFObject * object)
{
	//_eventListener->onWordExampleExit(wordIndex);	
		
}

int WordActor::getWordIndex()
{
	return wordIndex;
}

GAFObject * WordActor::getGAFObject()
{
	return gafObj;
}
