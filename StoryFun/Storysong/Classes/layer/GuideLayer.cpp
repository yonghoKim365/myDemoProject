#include "GuideLayer.h"
#include "data/PlayInfo.h"
#include "common/AssetUtils.h"

GuideLayer::GuideLayer(){}
GuideLayer::~GuideLayer(){}

GuideLayer* GuideLayer::create(std::string guidePath, GuideFInishedEventListener * listener)
{
	GuideLayer * ref = new GuideLayer();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->initLayer(guidePath, listener);
		return ref;
	}
	else
	{
		CC_SAFE_RELEASE(ref);
		return nullptr;
	}
}

void GuideLayer::initLayer(std::string guidePath, GuideFInishedEventListener * listener)
{
	this->initWithColor(Color4B(0, 0, 0, 100));
	finishedListener = listener;

	//auto asset = GAFAsset::create(guidePath.c_str(), nullptr);
	auto asset = AssetUtils::createAutoSoundAsset(guidePath.c_str());
	introObject = asset->createObjectAndRun(true);
	if (introObject)
		introObject->retain();

	introObject->setPosition(Vec2(0, 1200));
	introObject->playSequence("Intro", false);
	introObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(GuideLayer::onIntroGuideFinished, this)));
	introObject->setName("guideAnimation");
	addChild(introObject);
}

void GuideLayer::onIntroGuideFinished(GAFObject * object)
{
	finishedListener->onFInishedGuideAnimation();
	removeChildByName("guideAnimation");
	this->removeFromParent();
	
}

void GuideLayer::onEnter()
{
	LayerColor::onEnter();
	_touchListener = EventListenerTouchOneByOne::create();
	_touchListener->setSwallowTouches(true);

	_touchListener->onTouchBegan = CC_CALLBACK_2(GuideLayer::onTouchBegan, this);

	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(_touchListener, this);

}

bool GuideLayer::onTouchBegan(cocos2d::Touch* touch, cocos2d::Event* event)
{
	/*if (PlayInfo::create()->getGuidePlayed())
	{
		finishedListener->onFInishedGuideAnimation();
		removeChildByName("guideAnimation");
		this->removeFromParent();
	}*/
	return true;
}

void GuideLayer::onExit()
{
	PlayInfo::create()->setGuidePlayed(true);

	Director::getInstance()->getEventDispatcher()->removeEventListener(_touchListener);
	LayerColor::onExit();
}