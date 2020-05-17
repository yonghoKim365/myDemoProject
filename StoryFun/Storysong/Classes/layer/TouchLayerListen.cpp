#include "TouchLayerListen.h"

TouchLayerListen::TouchLayerListen(){}
TouchLayerListen::~TouchLayerListen(){}

TouchLayerListen * TouchLayerListen::create()
{
	TouchLayerListen *ref = new TouchLayerListen();
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

void TouchLayerListen::initData()
{
	bubbleId = 0;
	currentBubbleCount = 0;
	maxBubbleCount = 5;
	isFreezing = true;
	jsonInfo = JsonInfo::create();
}

void TouchLayerListen::initGUI()
{
	bubbleAsset = GAFAsset::create(jsonInfo->step1ObjectPath.c_str());
	bubbleAsset->retain();
	
}

void TouchLayerListen::initEvent()
{
	listener = EventListenerTouchOneByOne::create();

	listener->onTouchBegan = [&](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		//cocos2d::Rect rect = this->getBoundingBox();
		cocos2d::Rect rect = Rect(0, 0, 1920, 910);
		CCLOG("y = %f", p.y);
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

void TouchLayerListen::setTouchEnable()
{
	isFreezing = false;
}

void TouchLayerListen::setFreezing(bool freez)
{
	isFreezing = freez;
}

void TouchLayerListen::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{
	if (isFreezing)	return;
	bool isObjecTouch = false;

	std::string touchName;
	for (int i = 0; i < objectList.size(); i++)
	{
		GAFObject * obj = objectList.at(i);
		auto p = obj->getObjectByName("ImageHolder");

		if (isObjectTouch(obj, touch))
		{
			touchName = obj->getName();
			break;
		}
	}

	for (int i = 0; i < objectList.size(); i++)
	{
		GAFObject * obj = objectList.at(i);
		std::string currentName = obj->getName();

		if (!currentName.compare(touchName))
		{
			ListenObjectActor * actor = actorList.at(i);
			actor->startExitAnimation();
			isObjecTouch = true;
			break;
		}
	}

	
	if (currentBubbleCount < maxBubbleCount && !isObjecTouch)
	{
		//auto asset = GAFAsset::create(jsonInfo->step1ObjectPath.c_str(), nullptr);
		auto touchObj = bubbleAsset->createObjectAndRun(true);

		float objW = touchObj->getContentSize().width/2;
		float objh = touchObj->getContentSize().height;
		float positionX = _p.x - objW;
		touchObj->setPosition(Vec2(_p.x - 250, _p.y + 250));
		
		std::string bubbleIdName = StringUtils::format("bubble_%d", bubbleId);
		touchObj->setName(bubbleIdName);
		
		addChild(touchObj);
		
		ListenObjectActor * actor = ListenObjectActor::create(touchObj, this);
		if (actor)
			actor->retain();

		objectList.push_back(touchObj);
		actorList.push_back(actor);
		bubbleId++;
		currentBubbleCount++;
	}
}

bool TouchLayerListen::isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch)
{
	Vec2 localPoint = gaf->convertTouchToNodeSpace(touch);
	localPoint = cocos2d::PointApplyTransform(localPoint, gaf->getNodeToParentTransform());
	Rect r = gaf->getBoundingBoxForCurrentFrame();

	if (r.containsPoint(localPoint))
		return true;
	else
		return false;
}

void TouchLayerListen::onFinishedAnimation(GAFObject * gafObject)
{
	currentBubbleCount--;

	for (int i = 0; i < objectList.size(); i++)
	{
		GAFObject * obj = objectList.at(i);
		std::string currentName = obj->getName();
		std::string targetName = gafObject->getName();
		
		if (!currentName.compare(targetName))
		{
			objectList.erase(objectList.begin() + i);
			actorList.erase(actorList.begin() + i);
			break;
		}
	}
	
	removeChild(gafObject); 
}

void TouchLayerListen::resetLayer()
{
	bubbleId = 0;
	currentBubbleCount = 0;
	maxBubbleCount = 5;
	isFreezing = true;
	for (int i = 0; i < objectList.size(); i++)
	{
		GAFObject * obj = objectList.at(i);
		removeChild(obj);
	}

	objectList.clear();
	actorList.clear();
}

void TouchLayerListen::removeTouchEvent()
{
	_eventDispatcher->removeEventListener(listener);
}

void TouchLayerListen::onPlaybyTickEvent(int tickCount)
{

}

void TouchLayerListen::onPlayModeChange(PLAY_MODE::PlayType playMode)
{

}

void TouchLayerListen::onPlayTouchChange(PLAY_MODE::TouchType touchMode)
{

}