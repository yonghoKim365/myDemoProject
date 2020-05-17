#include "ListenObjectActor.h"

ListenObjectActor::ListenObjectActor() {}
ListenObjectActor::~ListenObjectActor(){}

ListenObjectActor * ListenObjectActor::create(GAFObject * object, ListenAnimationFinishedEventListener * listener)
{
	ListenObjectActor * ref = new ListenObjectActor();
	if (ref && ref->init(object, listener))
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

bool ListenObjectActor::init(gaf::GAFObject * object, ListenAnimationFinishedEventListener * listener)
{
	if (object == nullptr)
	{
		return false;
	}

	jsonInfo = JsonInfo::create();

	animationListener = listener;

	gafObject = object;
	gafObject->retain();

	srand((unsigned int)time(NULL));
	int index = (rand() % 5);
	CCLOG("index %d", index);
	
	imageHolder = gafObject->getObjectByName("ImageHolder");
	
	Sprite * skin = Sprite::create(jsonInfo->step1ObjectList.at(index).c_str());
	skin->setScale(0.065f);

	imageHolder->addChild(skin);
	
	gafObject->playSequence("Intro", false);
	gafObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ListenObjectActor::onFinishedIntroAnimation, this)));
	
	return true;
}

void ListenObjectActor::startExitAnimation()
{
	if (isAnimationRun)	return;

	isAnimationRun = true;
	gafObject->playSequence("Touch", false);
	gafObject->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(ListenObjectActor::onFinishedTouchAnimation, this)));
}


void ListenObjectActor::onFinishedIntroAnimation(GAFObject * object)
{
	gafObject->playSequence("Basic", true);
}


void ListenObjectActor::onFinishedTouchAnimation(GAFObject * object)
{
	isAnimationRun = false;

	gafObject->playSequence("Basic", true);

	animationListener->onFinishedAnimation(object);
}
