#include "TouchLayerPlay.h"
#include "audio/include/AudioEngine.h"

using namespace experimental;
TouchLayerPlay * TouchLayerPlay::create()
{
	TouchLayerPlay *ref = new TouchLayerPlay();
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

void TouchLayerPlay::initData()
{
	uiDepth = 0;
	isAlreadyShow = false;
	
	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;

	jsonInfo = JsonInfo::create();
}

void TouchLayerPlay::initGUI()
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

	for (int i = 0; i < jsonInfo->exampleInfoList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);

		auto asset = GAFAsset::create(exampleInfo.gafPath.c_str(), nullptr);
		auto touchObj = asset->createObjectAndRun(false);
		touchObj->setPosition(Vec2(0, 1200));
		addChild(touchObj);

		WordActor * actor = WordActor::create(touchObj, i, this);
		
		actor->setHide();

		if (actor)
		{
			actor->retain();
		}
		wordActorList.insert(wordActorList.begin() + i, actor);
	}

	AbsTouchLayer::initGUI();
}

void TouchLayerPlay::initEvent()
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

void TouchLayerPlay::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{
	if (isFreezing) return;

	bool isAnimTouch = false;

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

	if (isAnimTouch)	return;

	for (int i = 0; i < jsonInfo->exampleInfoList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);

		CCRect * rect = exampleInfo.touchBox;

		if (rect->containsPoint(_p) && examplePosition[i] == 1)
		{
			wordActorList.at(i)->startTouchAnimation();
			examplePosition[i] = 0;
			break;
		}
	}
}

void TouchLayerPlay::removeTouchEvent()
{
	_eventDispatcher->removeEventListener(listener);
}

bool TouchLayerPlay::isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch)
{
	Vec2 localPoint = gaf->convertTouchToNodeSpace(touch);
	localPoint = cocos2d::PointApplyTransform(localPoint, gaf->getNodeToParentTransform());
	Rect r = gaf->getBoundingBoxForCurrentFrame();

	if (r.containsPoint(localPoint))
		return true;
	else
		return false;
}

int TouchLayerPlay::getUIDepth()
{
	uiDepth++;
	return uiDepth;
}

void TouchLayerPlay::startAnimation()
{
	for (int i = 0; i < actorList.size(); i++)
	{
		SimpleTouchActor * actor = actorList.at(i);
		actor->startBasicAnimation(true);
	}
}

void TouchLayerPlay::onPlaybyTickEvent(int tickCount)
{
	if (jsonInfo->trackExampleList.at(tickCount).size() > 0)
	{
		currentExamplePath = jsonInfo->trackExampleList.at(tickCount);

		srand((unsigned int)time(NULL));
		int index = (rand() % 3);
		//int index = 2;
		if (!isAlreadyShow)
		{
			for (int i = 0; i < wordActorList.size(); i++)
			{
				examplePosition[i] = 1;
				WordActor * actor = wordActorList.at(i);
				actor->setVisible(true);
				if (!currentExamplePath.compare("null contents"))
				{
					actor->hideExample();
				}
				else
				{
					if (i == index)
						actor->showWordExample(tickCount);
					else
						actor->hideExample();
				}
				
				actor->startIntroAnimation();
			}

			machine->startTouchAnimation();
			AudioEngine::play2d(jsonInfo->machineEffectPath.c_str());
			isAlreadyShow = true;
		}
		else
		{
			//if (currentExamplePath.compare(lastExamplePath))
			{
				for (int i = 0; i < wordActorList.size(); i++)
				{
					examplePosition[i] = 1;
					WordActor * actor = wordActorList.at(i);
					if (!currentExamplePath.compare("null contents"))
					{
						actor->hideExample();
					}
					else
					{
						if (i == index)
							actor->showWordExample(tickCount);
						else
							actor->hideExample();
					}

					actor->startBasicAnimation();
				}

				//machine->startTouchAnimation();
			}
			
		}

		lastExamplePath = currentExamplePath;
	}
}

void TouchLayerPlay::resetLayer()
{
	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;
	CCLOG("TouchLayerPlay::resetLayer");
	isAlreadyShow = false;

	for (int i = 0; i < wordActorList.size(); i++)
	{
		WordActor * actor = wordActorList.at(i);	
		actor->setVisible(false);
	}
}

void TouchLayerPlay::onPlayModeChange(PLAY_MODE::PlayType playMode)
{

}

void TouchLayerPlay::onPlayTouchChange(PLAY_MODE::TouchType touchMode)
{

}

void TouchLayerPlay::onWordExampleExit(int id)
{

}

void TouchLayerPlay::onWordExampleExit2(int id)
{

}