#include "KGDialog.h"

bool KGDialog::initWithColor(const Color4B& color){
	if (!LayerColor::initWithColor(color)){
		return false;
	}

	setBlendFunc(BlendFunc::ALPHA_PREMULTIPLIED);
	_visibleSize = Director::getInstance()->getVisibleSize();

	return true;
}

void KGDialog::onEnter()
{
	LayerColor::onEnter();
	
	_touchListener = EventListenerTouchOneByOne::create();
	// Swallow touches only available in OneByOne mode.
	_touchListener->setSwallowTouches(true);

	_touchListener->onTouchBegan = CC_CALLBACK_2(KGDialog::onTouchBegan, this);
	_touchListener->onTouchMoved = CC_CALLBACK_2(KGDialog::onTouchMoved, this);
	_touchListener->onTouchEnded = CC_CALLBACK_2(KGDialog::onTouchEnded, this);

	// The priority of the touch listener is based on the draw order of sprite
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(_touchListener, this);
	
}

void KGDialog::setDialogSelectListener(DialogSelectListener * listener)
{
	_eventListener = listener;
}

void KGDialog::onExit()
{
	Director::getInstance()->getEventDispatcher()->removeEventListener(_touchListener);

	LayerColor::onExit();
}

bool KGDialog::onTouchBegan(Touch* touch, Event* event)
{
	auto touchPoint = touch->getLocation();
	return true;
}


void KGDialog::onSelectMode(DIALOG_SELECT_MODE::selectMode type)
{
	Director::getInstance()->resume();
	switch (type)
	{
	case DIALOG_SELECT_MODE::SELECT_LISTEN:
		_eventListener->onSelectListen();
		this->removeFromParent();
		break;

	case DIALOG_SELECT_MODE::SELECT_PLAY:
		_eventListener->onSelectPlay();
		this->removeFromParent();
		break;

	case DIALOG_SELECT_MODE::SELECT_QUIT:
		_eventListener->onSelectQuit();
		this->removeFromParent();
		break;

	case DIALOG_SELECT_MODE::SELECT_CLOSE:
		_eventListener->onSelectClose();
		this->removeFromParent();
		break;
	default:
		break;
	}
}