#include "TouchLayerPlayTypeC.h"
#include "audio/include/AudioEngine.h"

using namespace experimental;

TouchLayerPlayTypeC * TouchLayerPlayTypeC::create()
{
	TouchLayerPlayTypeC *ref = new TouchLayerPlayTypeC();
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

void TouchLayerPlayTypeC::initData()
{
	uiDepth = 0;
	isAlreadyShow = false;

	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;

	jsonInfo = JsonInfo::create();
}

void TouchLayerPlayTypeC::initGUI()
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

	auto leftContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_LEFT, nullptr);
	auto centerContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_CENTER, nullptr);
	auto rightContainerAsset = GAFAsset::create(GAF_CONTENTS_HOLDER_BLUE_RIGHT, nullptr);
	contentsHolderList.push_back(leftContainerAsset->createObjectAndRun(false));
	contentsHolderList.push_back(centerContainerAsset->createObjectAndRun(false));
	contentsHolderList.push_back(rightContainerAsset->createObjectAndRun(false));

	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		GAFObject * contentHolder = contentsHolderList.at(i);
		contentHolder->setPosition(Vec2(0, 1200));
		contentHolder->gotoAndStop(1);
		addChild(contentHolder);

		contentHolder->retain();
	}

	for (int i = 0; i < contentsHolderList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);
		auto asset = GAFAsset::create(exampleInfo.gafPath.c_str(), nullptr);
		auto touchObj = asset->createObjectAndRun(false);
		if (touchObj)
		{
			touchObj->retain();
			touchObj->autorelease();
			touchObj->getObjectByName("ContentsContainer")->gotoAndStop(1);
		}

		itemHolderList.push_back(touchObj);
		auto contentsHolder = contentsHolderList.at(i);
		GAFObject * contentContainer = contentsHolder->getObjectByName("ContentsHolder");
		contentContainer->addChild(touchObj);
		
		
		touchObj->setPosition(Vec2(-400, 400));
		//contentsHolder->setVisible(false);



		//srand((unsigned int)time(NULL));

		touchObj->gotoAndStop(1);
		int frame = (rand() % 5) + 1;
		CCLOG("frame %d", frame);
		auto ballFront = touchObj->getObjectByName("Ball_Front");
		auto ballBack = touchObj->getObjectByName("Ball_Back");
		ballFront->gotoAndStop(frame - 1);
		ballBack->gotoAndStop(frame - 1);

		int ballFrontIndex = ballFront->getCurrentFrameIndex();
		int ballBackIndex = ballBack->getCurrentFrameIndex();
		CCLOG("BALL FRONT =%d, BALL BACK = %d", ballFrontIndex, ballBackIndex);
		touchObj->setVisible(false);
	}
}

void TouchLayerPlayTypeC::initEvent()
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

void TouchLayerPlayTypeC::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{
	bool isAnimTouch = false;
	for (int i = 0; i < touchObject.size(); i++)
	{
		GAFObject * obj = touchObject.at(i);
		bool isTouch = isObjectTouch(obj, touch);

		
		if (isTouch)
		{
			SimpleTouchActor * obj = actorList.at(i);
			obj->startTouchAnimation(true);
			isAnimTouch = true;
			/*if (i == 0)
			AudioEngine::play2d("b02/music/b02_c08_s2_t1_sfx_06.mp3");
			else
			AudioEngine::play2d("b02/music/b02_c08_s2_t1_sfx_07.mp3");*/
			break;
		}
	}

	if (isAnimTouch)	return;

	for (int i = 0; i < jsonInfo->exampleInfoList.size(); i++)
	{
		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);

		CCRect * rect = exampleInfo.touchBox;

		if (rect->containsPoint(_p) && examplePosition[i] == 1)
		{
			//wordActorList.at(i)->startTouchAnimation();
			touchIndex = i;
			GAFObject * ballon = itemHolderList.at(i);
			ballon->playSequence("Touch", false);
			ballon->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(TouchLayerPlayTypeC::onFinishedTouchAnimation, this)));
			examplePosition[i] = 0;
			break;
		}
	}
}

void TouchLayerPlayTypeC::onFinishedTouchAnimation(gaf::GAFObject * object)
{
	GAFObject * ballon = itemHolderList.at(touchIndex);
	int position = object->getTag();

	auto color = ballon->getObjectByName("Ball_Front")->getCurrentFrameIndex();

	std::string frameLabel = StringUtils::format("Exit%d", color+1);
	CCLOG("color = %d, frameLabel= %s", color, frameLabel.c_str());



	ballon->playSequence(frameLabel.c_str(), false);
	ballon->setAnimationFinishedPlayDelegate(nullptr);
	ballon->getObjectByName("ContentsContainer")->playSequence("Touch", false);

	if (lastAddedIndex == touchIndex)
	{
		exampleAnim->playSequence("Touch", false);
		exampleAnim->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(TouchLayerPlayTypeC::onFinishedExampleAnimation, this)));
	}

}

void TouchLayerPlayTypeC::onFinishedExampleAnimation(GAFObject * object)
{
	exampleAnim->setAnimationFinishedPlayDelegate(nullptr);

	fadeAction = FadeOut::create(FADE_OUT_SPEED);
	itemHolderList.at(touchIndex)->runAction(fadeAction);
	//exampleObject->runAction(fadeAction);

	holderAction = FadeOut::create(FADE_OUT_SPEED);
	exampleAnim->runAction(holderAction);

	isFadeRun = true;
}

void TouchLayerPlayTypeC::removeTouchEvent()
{
	_eventDispatcher->removeEventListener(listener);
}


int TouchLayerPlayTypeC::getUIDepth()
{
	uiDepth++;
	return uiDepth;
}

void TouchLayerPlayTypeC::startAnimation()
{
	for (int i = 0; i < actorList.size(); i++)
	{
		SimpleTouchActor * actor = actorList.at(i);
		actor->startBasicAnimation(true);
	}
}

void TouchLayerPlayTypeC::onPlaybyTickEvent(int tickCount)
{
	if (jsonInfo->trackExampleList.at(tickCount).size() > 0)
	{
		if (isFadeRun)
		{
			itemHolderList.at(touchIndex)->stopAction(fadeAction);
			itemHolderList.at(touchIndex)->setOpacity(255);

			exampleAnim->stopAction(holderAction);
			exampleAnim->setOpacity(255);

			isFadeRun = false;
		}

		if (isAlreadyShow)
		{
			lastAddedIndex = -1;

			for (int i = 0; i < itemHolderList.size(); i++)
			{
				itemHolderList.at(i)->setAnimationFinishedPlayDelegate(nullptr);
				itemHolderList.at(i)->getObjectByName("ContentsContainer")->removeChildByName(oldExampleName);
			}
		}

		currentExamplePath = jsonInfo->trackExampleList.at(tickCount);

		srand((unsigned int)time(NULL));
		int index = (rand() % 3);

		
		std::string newExampleName = StringUtils::format("example_%d", tickCount);
		oldExampleName = newExampleName;

		if (!isAlreadyShow)
		{
			for (int i = 0; i < contentsHolderList.size(); i++)
			{
				//srand((unsigned int)time(NULL));
				int frame = (rand() % 5) + 1;
				
				exampleframe[i] = frame;
				examplePosition[i] = 1;
				auto touchObj = itemHolderList.at(i);
				touchObj->setAnimationFinishedPlayDelegate(nullptr);
				touchObj->setTag(i);
				touchObj->setVisible(true);

				
				if (i == index)
				{
					if (!currentExamplePath.compare("null contents"))
					{
						//obj->getObjectByName("ContentsContainer")->setVisible(false);
					}
					else
					{
						auto wordAsset = AssetUtils::createAutoSoundAsset(tickCount);
						auto exampleObject = wordAsset->createObjectAndRun(true);
						lastAddedIndex = i;

						if (exampleObject)
						{
							exampleObject->retain();
							exampleObject->autorelease();
							exampleObject->gotoAndStop(1);
							exampleObject->setName(newExampleName);
							exampleObject->setPosition(Vec2((EXAMPLE_WIDTH / 2) * -1, (EXAMPLE_HEIGHT/2)));
							exampleAnim = exampleObject;
						}
						touchObj->getObjectByName("ContentsContainer")->gotoAndStop(1);
						touchObj->getObjectByName("ContentsContainer")->setVisible(true);
						touchObj->getObjectByName("ContentsContainer")->addChild(exampleObject);
					}

				}
				else
				{
					touchObj->getObjectByName("ContentsContainer")->setVisible(false);
				}

				auto contentsHolder = contentsHolderList.at(i);
				contentsHolder->setVisible(true);
				contentsHolder->playSequence("Intro", false);
				touchObj->playSequence("Ready", false);
				
				/*CCLOG("frame %d", frame);
				auto ballFront = touchObj->getObjectByName("Ball_Front");
				auto ballBack = touchObj->getObjectByName("Ball_Back");
				ballFront->gotoAndStop(frame - 1);
				ballBack->gotoAndStop(frame - 1);

				int ballFrontIndex = ballFront->getCurrentFrameIndex();
				int ballBackIndex = ballBack->getCurrentFrameIndex();
				CCLOG("BALL FRONT =%d, BALL BACK = %d", ballFrontIndex, ballBackIndex);*/

			}
			
			machine->startTouchAnimation();
			AudioEngine::play2d(jsonInfo->machineEffectPath.c_str());
			isAlreadyShow = true;
		}
		else
		{
			//if (currentExamplePath.compare(lastExamplePath))
			{
				for (int i = 0; i < itemHolderList.size(); i++)
				{
					int frame = (rand() % 5) + 1;
					CCLOG("frame = %d", frame);
					exampleframe[i] = frame;
					examplePosition[i] = 1;
					//SimpleWordActor * actor = wordActorList.at(i);
					GAFObject * obj = itemHolderList.at(i);
					obj->setAnimationFinishedPlayDelegate(nullptr);
					obj->getObjectByName("ContentsContainer")->gotoAndStop(1);
					obj->setTag(i);
					if (!currentExamplePath.compare("null contents"))
					{
						//obj->getObjectByName("ContentsContainer")->setVisible(false);
					}
					else
					{
						if (i == index)
						{
							auto wordAsset = AssetUtils::createAutoSoundAsset(tickCount);
							auto exampleObject = wordAsset->createObjectAndRun(true);
							lastAddedIndex = i;

							if (exampleObject)
							{
								exampleObject->retain();
								exampleObject->autorelease();
								exampleObject->gotoAndStop(1);
								exampleObject->setName(newExampleName);
								exampleObject->setPosition(Vec2((EXAMPLE_WIDTH / 2) * -1, (EXAMPLE_HEIGHT / 2)));
								exampleAnim = exampleObject;
							}
							obj->getObjectByName("ContentsContainer")->gotoAndStop(1);
							obj->getObjectByName("ContentsContainer")->setVisible(true);
							obj->getObjectByName("ContentsContainer")->addChild(exampleObject);
							
						}
						else
							obj->getObjectByName("ContentsContainer")->setVisible(false);
					}
					obj->getObjectByName("Ball_Front")->gotoAndStop(frame);
					obj->getObjectByName("Ball_Back")->gotoAndStop(frame);
					obj->playSequence("Basic", false);
				}

				//machine->startTouchAnimation();
			}

		}

		lastExamplePath = currentExamplePath;
	}
}

void TouchLayerPlayTypeC::resetLayer()
{
	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;
	CCLOG("TouchLayerPlay::resetLayer");
	isAlreadyShow = false;

	//for (int i = 0; i < wordActorList.size(); i++)
	//{
	//	SimpleWordActor * actor = wordActorList.at(i);
	//	actor->setVisible(false);
	//}
}

void TouchLayerPlayTypeC::onPlayModeChange(PLAY_MODE::PlayType playMode){}

void TouchLayerPlayTypeC::onPlayTouchChange(PLAY_MODE::TouchType touchMode){}

void TouchLayerPlayTypeC::onWordExampleExit(int id){}

void TouchLayerPlayTypeC::onWordExampleExit2(int id){}