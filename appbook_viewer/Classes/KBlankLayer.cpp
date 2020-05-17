#include "KBlankLayer.h"



KBlankLayer::~KBlankLayer()
{
}

KBlankLayer* KBlankLayer::createLayer(Color4B color4B) {
	KBlankLayer * pRet = new (std::nothrow) KBlankLayer();
	if (pRet && pRet->init(color4B)) {
		pRet->autorelease();
		return pRet;
	}
	delete pRet;
	pRet = nullptr;
	return nullptr;
}

// on "init" you need to initialize your instance
bool KBlankLayer::init(Color4B color4B)
{
	//////////////////////////////
	// 1. super init first
	if (!LayerColor::initWithColor(color4B))
	{
		return false;
	}

	return true;
}

KBlankLayer::KBlankLayer()
{
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();


	m_pBlockingSprite = Sprite::create();
	m_pBlockingSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	m_pBlockingSprite->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	m_pBlockingSprite->setColor(Color3B(0, 0, 0));
	m_pBlockingSprite->setOpacity(0);
	this->addChild(m_pBlockingSprite);



	//button left
	mBtn = ui::Button::create("syscommon/viewer_btn_play_n.png",
		"syscommon/viewer_btn_play_p.png",
		"syscommon/viewer_btn_play_n.png");
	CViewUtils::setSize(mBtn, visibleSize.width / 2 -95 , visibleSize.height / 2 -95 , 190, 190);
	this->addChild(mBtn);
	mBtn->setVisible(false);
	mBtn->addClickEventListener([&](cocos2d::Ref* pSender) {
		mBtn->setVisible(false);
		m_pBlockingSprite->setOpacity(0);
	});

	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		mBtn->setVisible(true);
		m_pBlockingSprite->setOpacity(100);
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, m_pBlockingSprite);

}

