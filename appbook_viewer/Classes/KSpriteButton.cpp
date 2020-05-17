#include "KSpriteButton.h"
#include "cocos2d.h"
#include <string>
using namespace std;

KSpriteButton::KSpriteButton(bool aShouldActLikeButton) {
	shouldActLikeButton = aShouldActLikeButton;
	mpEventListener = nullptr;
}
KSpriteButton::~KSpriteButton() {
	//log("KSpriteButton::~KSpriteButton()");

}
KSpriteButton* KSpriteButton::create(bool aShouldActLikeButton) {
	KSpriteButton * pSprite = new KSpriteButton(aShouldActLikeButton);
	if (pSprite && pSprite->init()) {
		pSprite->autorelease();
		pSprite->addEvents();
		return pSprite;
	}
	CC_SAFE_DELETE(pSprite);
	return nullptr;
}
void KSpriteButton::addEvents() {
	if (mpEventListener != nullptr) return;

	mpEventListener = cocos2d::EventListenerTouchOneByOne::create();
	mpEventListener->setSwallowTouches(true);
	mpEventListener->onTouchBegan = [=](cocos2d::Touch * touch, cocos2d::Event * event) {
		auto target = event->getCurrentTarget();
		Vec2 ConvertPos = target->convertToNodeSpace(touch->getLocation());
		Rect rect = Rect(0, 0, target->getContentSize().width, target->getContentSize().height);
		if (rect.containsPoint(ConvertPos)) {
			if (shouldActLikeButton) {
				float scaleX = target->getScaleX();
				float scaleY = target->getScaleY();
		//		scaleX *= 1.15f;
		//		scaleY *= 1.15f;
				target->setScaleX(scaleX);
				target->setScaleY(scaleY);
				target->setOpacity(200);
			}
			return true;
		}
		return false;
	};
	mpEventListener->onTouchEnded = [=](cocos2d::Touch* touch, cocos2d::Event * event) {
		auto target = event->getCurrentTarget();
		if (shouldActLikeButton) {
			float scaleX = target->getScaleX();
			float scaleY = target->getScaleY();
			//scaleX /= 1.15f;
			//scaleY /= 1.15f;
			target->setScaleX(scaleX);
			target->setScaleY(scaleY);
			target->setOpacity(255);
		}
		Vec2 ConvertPos = target->convertToNodeSpace(touch->getLocation());
		Rect rect = Rect(0, 0, target->getContentSize().width, target->getContentSize().height);
		if (rect.containsPoint(ConvertPos)) {
			KSpriteButton::touchEvent(touch, event);
		}
	};
	mpEventListener->onTouchCancelled = [=](cocos2d::Touch* touch, cocos2d::Event * event) {
		auto target = event->getCurrentTarget();
		if (shouldActLikeButton) {
			float scaleX = target->getScaleX();
			float scaleY = target->getScaleY();
		//	scaleX /= 1.15f;
		//	scaleY /= 1.15f;
			target->setScaleX(scaleX);
			target->setScaleY(scaleY);
			target->setOpacity(255);
		}
	};
	cocos2d::Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(mpEventListener, this);
}
void KSpriteButton::removeEvents(){
	if (mpEventListener == nullptr) return;
	cocos2d::Director::getInstance()->getEventDispatcher()->removeEventListener(mpEventListener);
	mpEventListener = nullptr;
}
void KSpriteButton::touchEvent(Touch * pTouch, Event * pEvent) {
	//log("confirmed..%s", pEvent->getCurrentTarget()->getName().c_str());
	if (mCallback != nullptr) {
		mCallback(pTouch, pEvent);
	}
}
void KSpriteButton::setCallback(function<void(Touch * pTouch, Event * pEvent)> aCallback) {
	mCallback = aCallback;
}