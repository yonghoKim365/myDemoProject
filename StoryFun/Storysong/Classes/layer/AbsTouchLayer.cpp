#include "AbsTouchLayer.h"


AbsTouchLayer * AbsTouchLayer::create()
{
	AbsTouchLayer *ref = new AbsTouchLayer();
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

void AbsTouchLayer::initData()
{
	uiDepth = 0;
	isAlreadyShow = false;

	currentExamplePath = "";
	examplePosition[0] = 0;
	examplePosition[1] = 0;
	examplePosition[2] = 0;

	jsonInfo = JsonInfo::create();
}

void AbsTouchLayer::initGUI()
{
	if (JsonInfo::create()->isDebugMode)
	{
		for (int i = 0; i < JsonInfo::create()->touchObjectRect.size(); i++)
		{
			CCRect * touchBox = JsonInfo::create()->touchObjectRect.at(i);
			Vec2 vertices[] =
			{
				Vec2(touchBox->getMaxX(), touchBox->getMaxY()),
				Vec2(touchBox->getMaxX(), touchBox->getMinY()),
				Vec2(touchBox->getMinX(), touchBox->getMinY()),
				Vec2(touchBox->getMinX(), touchBox->getMaxY())
			};

			auto rectNode = DrawNode::create();
			rectNode->drawPolygon(vertices, 4, Color4F(1.0f, 0.3f, 0.3f, 1), 3, Color4F(0.2f, 0.2f, 0.2f, 1));
			addChild(rectNode);
		}
	}
}

void AbsTouchLayer::initEvent()
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

void AbsTouchLayer::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{

}

void AbsTouchLayer::removeTouchEvent()
{
	_eventDispatcher->removeEventListener(listener);
}

bool AbsTouchLayer::isObjectTouch(GAFObject * gaf, cocos2d::Touch* touch)
{
	Vec2 localPoint = gaf->convertTouchToNodeSpace(touch);
	
	localPoint = cocos2d::PointApplyTransform(localPoint, gaf->getNodeToParentTransform());
	Rect r = gaf->getBoundingBoxForCurrentFrame();
	
	if (r.containsPoint(localPoint))
		return true;
	else
		return false;
}

int AbsTouchLayer::getUIDepth()
{
	uiDepth++;
	return uiDepth;
}
void AbsTouchLayer::setunfreezing()
{
	isFreezing = false;
}

void AbsTouchLayer::startAnimation()
{
	
}

void AbsTouchLayer::onPlaybyTickEvent(int tickCount)
{
	
}

void AbsTouchLayer::resetLayer()
{
	
}

void AbsTouchLayer::onPlayModeChange(PLAY_MODE::PlayType playMode)
{

}

void AbsTouchLayer::onPlayTouchChange(PLAY_MODE::TouchType touchMode)
{

}

void AbsTouchLayer::onWordExampleExit(int id)
{

}

void AbsTouchLayer::onWordExampleExit2(int id)
{

}