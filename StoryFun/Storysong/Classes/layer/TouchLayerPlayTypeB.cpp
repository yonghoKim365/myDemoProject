#include "TouchLayerPlayTypeB.h"
#include "audio/include/AudioEngine.h"
#include "common/Utils.h"
#include "data/def.h"

using namespace experimental;

TouchLayerPlayTypeB * TouchLayerPlayTypeB::create()
{
	TouchLayerPlayTypeB *ref = new TouchLayerPlayTypeB();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->initData();
		ref->initGUI();
		ref->initEvent();
		return ref;
	}
	else
	{
		CC_SAFE_DELETE(ref);
		return nullptr;
	}
}

void TouchLayerPlayTypeB::initData()
{
	uiDepth = 0;
	isAlreadyShow = false;

	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;

	jsonInfo = JsonInfo::create();
}

void TouchLayerPlayTypeB::initGUI()
{
	for (int i = 0; i < jsonInfo->touchObjectList.size(); i++)
	{
		GafObjectInfo touchObjectInfo = jsonInfo->touchObjectList.at(i);
		//auto asset = GAFAsset::create(touchObjectInfo.gafPath.c_str(), nullptr);
		auto asset = AssetUtils::createAutoSoundAsset(jsonInfo->touchPathList.at(i).c_str());
		
		auto touchObj = asset->createObjectAndRun(true);
		touchObj->setPosition(Vec2(touchObjectInfo.positionX, touchObjectInfo.positionY));
		addChild(touchObj, getUIDepth());

		SimpleTouchActor * actor = SimpleTouchActor::create(touchObj);
		if (actor)
		{
			actor->retain();
		}
		actorList.push_back(actor);
		touchObject.push_back(touchObj);
	}

	auto asset = GAFAsset::create(jsonInfo->machineInfo.gafPath.c_str(), nullptr);
	machineObject = asset->createObjectAndRun(true);
	machineObject->setPosition(Vec2(jsonInfo->machineInfo.positionX, jsonInfo->machineInfo.positionY));

	addChild(machineObject, getUIDepth());

	machine = MachineActor::create(machineObject);

	if (machine)
	{
		machine->retain();
	}
	
	GAFAsset * leftContainerAsset;
	GAFAsset * centerContainerAsset;
	GAFAsset * rightContainerAsset;

	if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_D)
	{
		leftContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_WHITE_LEFT, nullptr);
		centerContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_WHITE_CENTER, nullptr);
		rightContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_WHITE_RIGHT, nullptr);
	}
	else if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
	{
		leftContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_YELLOW_LEFT, nullptr);
		centerContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_YELLOW_CENTER, nullptr);
		rightContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_YELLOW_RIGHT, nullptr);

	}
	else
	{
		leftContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_LEFT, nullptr);
		centerContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_CENTER, nullptr);
		rightContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_RIGHT, nullptr);
	}
	 
	contentsHolderList.push_back(leftContainerAsset->createObjectAndRun(false));
	contentsHolderList.push_back(centerContainerAsset->createObjectAndRun(false));
	contentsHolderList.push_back(rightContainerAsset->createObjectAndRun(false));

	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		GAFObject * contentHolder = contentsHolderList.at(i);
		contentHolder->setPosition(Vec2(0, 1200));
		contentHolder->gotoAndStop(1);
		addChild(contentHolder, getUIDepth());
		
		contentHolder->retain();
	}
	
	/*auto scheduler = cocos2d::Director::getInstance()->getScheduler();
	scheduler->performFunctionInCocosThread(CC_CALLBACK_0(TouchLayerPlayTypeB::preloadExample, this));*/

	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);
		auto asset = GAFAsset::create(exampleInfo.gafPath.c_str(), nullptr);
		//auto asset = AssetUtils::createAutoSoundAsset(exampleInfo.gafPath.c_str());
		CCLOG("path = %s", exampleInfo.gafPath.c_str());
		auto touchObj = asset->createObjectAndRun(false);

		ExampleActor * example;
		if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
			example = ExampleActorBall::create(touchObj, i);
		else
			example = ExampleActor::create(touchObj, i);

		if (example)
			example->retain();
		exampleList.push_back(example);

		auto contentsHolder = contentsHolderList.at(i);
		GAFObject * contentContainer = contentsHolder->getObjectByName("ContentsHolder");
		contentContainer->addChild(touchObj);
		touchObj->setPosition(Vec2(-400, 400));
	}

	/*CCRect * touchBox = new Rect(atof(recX.c_str()), atof(recY.c_str()), atof(recWidth.c_str()), atof(recHeight.c_str()));

	jsonInfo->touchObjectRect.push_back(touchBox);*/
	
	AbsTouchLayer::initGUI();
}

void TouchLayerPlayTypeB::preloadExample()
{
	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);
		auto asset = GAFAsset::create(exampleInfo.gafPath.c_str(), nullptr);
		//auto asset = AssetUtils::createAutoSoundAsset(exampleInfo.gafPath.c_str());
		CCLOG("path = %s", exampleInfo.gafPath.c_str());
		auto touchObj = asset->createObjectAndRun(false);

		ExampleActor * example;
		if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
			example = ExampleActorBall::create(touchObj, i);
		else
			example = ExampleActor::create(touchObj, i);

		if (example)
			example->retain();
		exampleList.push_back(example);

		auto contentsHolder = contentsHolderList.at(i);
		GAFObject * contentContainer = contentsHolder->getObjectByName("ContentsHolder");
		contentContainer->addChild(touchObj);
		touchObj->setPosition(Vec2(-400, 400));
	}
}

void TouchLayerPlayTypeB::initEvent()
{
	listener = EventListenerTouchOneByOne::create();
	listener->onTouchBegan = [&](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		cocos2d::Rect rect = this->getBoundingBox();

		if (rect.containsPoint(p))
		{
			return true;
		}

		return false;
	};
	
	listener->onTouchEnded = [=](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		touchEvent(touch, p);
	};
	
	_eventDispatcher = cocos2d::Director::getInstance()->getEventDispatcher();
	_eventDispatcher->addEventListenerWithFixedPriority(listener, 30);
	
}

void TouchLayerPlayTypeB::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{
	if (isFreezing) return;

	bool isAnimTouch = false;

	
	for (int i = 0; i < jsonInfo->exampleInfoList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);

		CCRect * rect = exampleInfo.touchBox;

		if (rect->containsPoint(_p) && examplePosition[i] == 1)
		{
			touchIndex = i;
			examplePosition[i] = 0;
			auto example = exampleList.at(i);
			example->startTouchAnimation();

			isAnimTouch = true;
			break;
		}
	}

	if (isAnimTouch)	return;

	for (int i = 0; i < JsonInfo::create()->touchObjectRect.size(); i++)
	{
		CCRect * touchBox = JsonInfo::create()->touchObjectRect.at(i);
		if (touchBox->containsPoint(_p))
		{
			isAnimTouch = true;
			SimpleTouchActor * obj = actorList.at(i);
			obj->startTouchAnimation(true);
		}
	}



	/*for (int i = 0; i < touchObject.size(); i++)
	{
		GAFObject * obj = touchObject.at(i);
		bool isTouch = isObjectTouch(obj, touch);

		if (isTouch)
		{
			SimpleTouchActor * obj = actorList.at(i);
			obj->startTouchAnimation(true);
			
			break;
		}
	}*/
}

void TouchLayerPlayTypeB::onFinishedTouchAnimation(gaf::GAFObject * object)
{
	GAFObject * ballon = itemHolderList.at(touchIndex);
	ballon->playSequence("Exit", false);
	ballon->setAnimationFinishedPlayDelegate(nullptr);
	ballon->getObjectByName("ContentsContainer")->playSequence("Touch", false);

	if (lastAddedIndex == touchIndex)
	{
		exampleAnim->playSequence("Touch", false);
		exampleAnim->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(TouchLayerPlayTypeB::onFinishedExampleAnimation, this)));
	}
	
}

void TouchLayerPlayTypeB::onFinishedExampleAnimation(GAFObject * object)
{
	exampleAnim->setAnimationFinishedPlayDelegate(nullptr);

	fadeAction = FadeOut::create(FADE_OUT_SPEED);
	itemHolderList.at(touchIndex)->runAction(fadeAction);
	//exampleObject->runAction(fadeAction);

	holderAction = FadeOut::create(FADE_OUT_SPEED);
	exampleAnim->runAction(holderAction);
		
	isFadeRun = true;
}

void TouchLayerPlayTypeB::removeTouchEvent()
{
	_eventDispatcher->removeEventListener(listener);
}


int TouchLayerPlayTypeB::getUIDepth()
{
	uiDepth++;
	return uiDepth;
}

void TouchLayerPlayTypeB::startAnimation()
{
	for (int i = 0; i < actorList.size(); i++)
	{
		SimpleTouchActor * actor = actorList.at(i);
		actor->startBasicAnimation(true);
	}
}

void TouchLayerPlayTypeB::onPlaybyTickEvent(int tickCount)
{
	if (jsonInfo->trackExampleList.at(tickCount).size() > 0)
	{
		currentExamplePath = jsonInfo->trackExampleList.at(tickCount);

		srand((unsigned int)time(NULL));
		int index = (rand() % 3);
		
		if (!isAlreadyShow)
		{
			for (int i = 0; i < contentsHolderList.size(); i++)
			{
				examplePosition[i] = 1;
				auto touchObj = exampleList.at(i);
				
				int frame = (rand() % 5) + 1;
				

				if (i == index)
				{	
					if (!currentExamplePath.compare("null contents"))
						touchObj->hideExample(frame);
					else
						touchObj->addExample(tickCount, frame);
				}
				else
					touchObj->hideExample(frame);
				
				auto contentsHolder = contentsHolderList.at(i);
				contentsHolder->setVisible(true);
				contentsHolder->playSequence("Intro",false);
				touchObj->startIntroAnimation();
			}
			
			machine->startTouchAnimation();
			AudioEngine::play2d(jsonInfo->machineEffectPath.c_str());
			isAlreadyShow = true;
		}
		else
		{
			for (int i = 0; i < contentsHolderList.size(); i++)
			{
				examplePosition[i] = 1;
				auto obj = exampleList.at(i);
				
				int frame = (rand() % 5) + 1;
				
				if (!currentExamplePath.compare("null contents"))
				{
					obj->hideExample(frame);
				}
				else
				{
					if (i == index)
					{
						obj->addExample(tickCount, frame);
					}
					else
					{
						obj->hideExample(frame);
					}
				}

				obj->startIntroAnimation();
			}

		}

		lastExamplePath = currentExamplePath;
	}

	if (jsonInfo->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_D)
	{
		machine->changeMotion();
	}
}

void TouchLayerPlayTypeB::resetLayer()
{
	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;
	isAlreadyShow = false;
	isFreezing = true;

	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		examplePosition[i] = 0;
		auto contentsHolder = contentsHolderList.at(i);
		contentsHolder->setVisible(false);
		
	}
}

void TouchLayerPlayTypeB::onPlayModeChange(PLAY_MODE::PlayType playMode){}

void TouchLayerPlayTypeB::onPlayTouchChange(PLAY_MODE::TouchType touchMode){}

void TouchLayerPlayTypeB::onWordExampleExit(int id){}

void TouchLayerPlayTypeB::onWordExampleExit2(int id){}