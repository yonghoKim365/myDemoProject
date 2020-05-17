#include "KPopupLayer.h"
#include "Page.h"

USING_NS_CC;

KPopupLayer* KPopupLayer::createLayer(Color4B color4B) {
	KPopupLayer * pRet = new (std::nothrow) KPopupLayer();
	if (pRet && pRet->init(color4B)) {
		pRet->autorelease();
		return pRet;
	}
	delete pRet;
	pRet = nullptr;
	return nullptr;
}

// on "init" you need to initialize your instance
bool KPopupLayer::init(Color4B color4B)
{
    //////////////////////////////
    // 1. super init first
    if ( !LayerColor::initWithColor(color4B))
    {
        return false;
    }

    return true;
}

void KPopupLayer::setEBookIBookGuidePopupCallback(std::string userdefaultVal, bool bEbook, const  KPopupLayerCallback & callbackChkbox) {
	mCallChkBox = callbackChkbox;
	_userdefaultVal = userdefaultVal;

	//makeCheckBoxUI();

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	auto background = Sprite::create();
	background->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	background->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	background->setColor(Color3B(0, 0, 0));
	background->setOpacity(128);
	this->addChild(background);

	auto sprBackground = Sprite::create("syscommon/book_popup_bg.png");
	sprBackground->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	sprBackground->setPosition(Vec2(134, 120));
	this->addChild(sprBackground);

	if (bEbook){
		auto imgText1 = Sprite::create("syscommon/popup_text_img_nofollowing.png");
		imgText1->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
		imgText1->setPosition(Vec2(356, 245));
		this->addChild(imgText1);
	}
	else{
		auto imgText1 = Sprite::create("syscommon/book_popup_text_guide.png");
		imgText1->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
		imgText1->setPosition(Vec2(356, 245));
		this->addChild(imgText1);
	}

	auto imgText2 = Sprite::create("syscommon/book_popup_text_noreview.png");
	imgText2->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	imgText2->setPosition(Vec2(522 + 47, 176));
	this->addChild(imgText2);

	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, background);

	//check box
	auto pChkBox = ui::CheckBox::create();
	pChkBox->setTouchEnabled(true);
	pChkBox->loadTextureBackGround("syscommon/book_popup_btn_check_n.png");
	pChkBox->loadTextureBackGroundSelected("syscommon/book_popup_btn_check_s.png");
	pChkBox->loadTextureFrontCross("syscommon/book_popup_btn_check_s.png");

	pChkBox->setSelected(false);
	bChkBoxChecked = false;

	//CViewUtils::setSize(pChkBox, visibleSize.width / 2 - (pChkBox->getContentSize().width / 2) - 50,	visibleSize.height / 2 + 170,
	//	pChkBox->getContentSize().width, pChkBox->getContentSize().height);
	pChkBox->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	pChkBox->setPosition(Vec2(522, 176));
	this->addChild(pChkBox);
	pChkBox->addTouchEventListener([=](Ref* pSender, ui::Widget::TouchEventType type){
		mCallChkBox(pSender);
		switch (type)
		{
		case ui::Widget::TouchEventType::BEGAN:			break;
		case ui::Widget::TouchEventType::ENDED:			
			if (pChkBox->isSelected()){
				bChkBoxChecked = true;
			}
			else{
				bChkBoxChecked = false;
			}
			break;
		default:
			break;
		}
	});

	//button close
	auto pBtnClose = ui::Button::create("syscommon/common_btn_close_n.png",
		"syscommon/common_btn_close_p.png",
		"syscommon/common_btn_close_n.png");

	pBtnClose->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	pBtnClose->setPosition(Vec2(951, 800 - 144 - 99));
	//CViewUtils::setSize(pBtnClose, visibleSize.width / 2 - (pBtnClose->getContentSize().width / 2) + 400, visibleSize.height / 2 - 250,
	//	pBtnClose->getContentSize().width, pBtnClose->getContentSize().height);

	this->addChild(pBtnClose);
	pBtnClose->addClickEventListener([&](cocos2d::Ref* pSender) {
		UserDefault::getInstance()->setBoolForKey(_userdefaultVal.c_str(), bChkBoxChecked);		
		UserDefault::getInstance()->flush();
		this->removeFromParent();
		cocos2d::experimental::AudioEngine::play2d("common/snd/AB_common_sfx_btn_01.mp3", false);
	});
}



void KPopupLayer::setYesTypeCallback( std::string sTitle, std::string sContent,
	std::string sYes, const  KPopupLayerCallback & callbackYes) {	
	mCallbackYes = callbackYes;

	makeCommonUI(sTitle, sContent, "syscommon/1_popup_top_volume.png");

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();


	//button left
	auto pBtnLeft = ui::Button::create("syscommon/1_btn_ok_n.png",
		"syscommon/1_btn_ok_p.png",
		"syscommon/1_btn_ok_n.png");
	CViewUtils::setSize(pBtnLeft, visibleSize.width / 2 - (pBtnLeft->getContentSize().width / 2),
		(visibleSize.height / 2 - ((740 / 2)) / 2) + 160 / 2 + 428 / 2 + 1 + 14, 
		pBtnLeft->getContentSize().width, pBtnLeft->getContentSize().height);
	this->addChild(pBtnLeft);
	pBtnLeft->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackYes(pSender);
		this->removeFromParent();
	});

	/*
	std::string sFontFile = "fonts/NanumGothic.ttf";

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	if (sFontFile == "fonts/NanumPen.ttf")	sFontFile = "³ª´®¼Õ±Û¾¾ Ææ";
	else if (sFontFile == "fonts/NanumGothic.ttf")	sFontFile = "³ª´®°íµñ";
#endif


	//Label* label = Label::createWithTTF("Result", sFontFile, nFontSize);

	Label* label = Label::createWithSystemFont(sYes, sFontFile, 18
		, Size(160, 50), TextHAlignment::CENTER, TextVAlignment::CENTER);

	Color3B color = ccc3(0, 0, 0);
	label->setTextColor(Color4B(color));

	CViewUtils::setSize(label, visibleSize.width / 2 - (160 / 2) , (visibleSize.height / 2 - ((740 / 2)) / 2) + 160 / 2 + 428 / 2 + 1 + 14, 160, 50);
	this->addChild(label);

	*/
}


void KPopupLayer::removePopupFromParent(Node* pNode){
	mCallbackYes(this);
	pNode->removeFromParent();
}

void KPopupLayer::setJtpWarningCallback(const KPopupLayerCallback & callbackYes) {

	mCallbackYes = callbackYes;

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	auto sprBackground = Sprite::create("syscommon/book_toast_bg.png");
	sprBackground->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	sprBackground->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	this->addChild(sprBackground);

	auto imgText = Sprite::create("syscommon/book_toast_text_jumpto.png");
	imgText->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	imgText->setPosition(Vec2(36, 46));
	sprBackground->addChild(imgText);

	CallFuncN *act1 = CallFuncN::create(CC_CALLBACK_1(KPopupLayer::removePopupFromParent, this));
	auto act = Sequence::createWithTwoActions(DelayTime::create(4.0f), act1);
	this->runAction(act);

	// for touch block
	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, sprBackground);


}



void KPopupLayer::continueYesNoTypeCallback(const KPopupLayerCallback & callbackYes, const KPopupLayerCallback & callbackNo)
{
	mCallbackYes = callbackYes;
	mCallbackNo = callbackNo;

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	// UI
	auto sprBackground = Sprite::create("syscommon/popup_alert_bg.png");
	//	pTitleB->setTextureRect(Rect(0, 0, 854 / 2, 152 / 2));
	sprBackground->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	CViewUtils::setSizeByScale(sprBackground, 0, 0);// visibleSize.width / 2, visibleSize.height / 2);
	this->addChild(sprBackground);
	
	auto imgPopupBg = Sprite::create("syscommon/popup_alert_board_new.png");
	imgPopupBg->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	CViewUtils::setPosAndSizeByScale(imgPopupBg, 246, 221);
	this->addChild(imgPopupBg);

	auto imgText1 = Sprite::create("syscommon/popup_text_img_following.png");
	imgText1->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	CViewUtils::setPosAndSizeByScale(imgText1, 246 + 295, 221 + 746 - 39);
	this->addChild(imgText1);

	//button left
	auto pBtnLeft = ui::Button::create("syscommon/popup_btn_following_n.png",
		"syscommon/popup_btn_following_p.png",
		"syscommon/popup_btn_following_n.png");
	
	pBtnLeft->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	CViewUtils::setPosAndSizeByScale(pBtnLeft, 246 + 351, 221 + 45); // 278 + 321, 221 + 89);
	this->addChild(pBtnLeft);
	pBtnLeft->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackYes(pSender);
		this->removeFromParent();
		cocos2d::experimental::AudioEngine::play2d("common/snd/AB_common_sfx_btn_01.mp3", false);
	});

	//button right
	auto pBtnRight = ui::Button::create("syscommon/popup_btn_restart_n.png",
		"syscommon/popup_btn_restart_p.png",
		"syscommon/popup_btn_restart_n.png");

	pBtnRight->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	CViewUtils::setPosAndSizeByScale(pBtnRight, 246 + 351 + 380 + 33, 221 + 45);// 278 + 321 + 375 + 38, 221 + 89);
	this->addChild(pBtnRight);
	pBtnRight->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackNo(pSender);
		this->removeFromParent();
		cocos2d::experimental::AudioEngine::play2d("common/snd/AB_common_sfx_btn_01.mp3", false);
	});

	
	// for touch block
	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, sprBackground);
	
	
}

void KPopupLayer::volumeMuteWarningPopupCallback(const KPopupLayerCallback & callbackOk)
{
	mCallbackYes = callbackOk;

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	// UI
	auto sprBackground = Sprite::create("syscommon/popup_alert_bg.png");
	//	pTitleB->setTextureRect(Rect(0, 0, 854 / 2, 152 / 2));
	sprBackground->setAnchorPoint(Vec2::ANCHOR_MIDDLE);
	CViewUtils::setSizeByScale(sprBackground, 0, 0);// visibleSize.width / 2, visibleSize.height / 2);
	this->addChild(sprBackground);

	auto imgPopupBg = Sprite::create("syscommon/popup_alert_board_new.png");
	imgPopupBg->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	CViewUtils::setPosAndSizeByScale(imgPopupBg, 246, 221);
	this->addChild(imgPopupBg);

	auto imgText1 = Sprite::create("syscommon/popup_text_img_soundoff.png");
	imgText1->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	CViewUtils::setPosAndSizeByScale(imgText1, 246 + 295, 221 + 746 - 39);
	this->addChild(imgText1);

	//button ok
	auto pBtnLeft = ui::Button::create("syscommon/common_popup_alert_btn_ok_n.png",
		"syscommon/common_popup_alert_btn_ok_p.png",
		"syscommon/common_popup_alert_btn_ok_n.png");


	pBtnLeft->setAnchorPoint(Vec2::ANCHOR_BOTTOM_LEFT);
	CViewUtils::setPosAndSizeByScale(pBtnLeft, 246 + 563, 221 + 45); // 278 + 321, 221 + 89);
	this->addChild(pBtnLeft);
	pBtnLeft->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackYes(pSender);
		this->removeFromParent();
		cocos2d::experimental::AudioEngine::play2d("common/snd/AB_common_sfx_btn_01.mp3", false);
	});


	// for touch block
	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, sprBackground);

}

void KPopupLayer::setYesNoTypeCallback( std::string sTitle, std::string sContent
	, std::string sYes, std::string sNo, const KPopupLayerCallback & callbackYes
					,  const KPopupLayerCallback & callbackNo) {	

	mCallbackYes = callbackYes;
	mCallbackNo = callbackNo;

	makeCommonUI(sTitle, sContent, "syscommon/1_popup_top.png");

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();


	//button left
	auto pBtnLeft = ui::Button::create("syscommon/1_btn_fromthefirst_n.png",
		"syscommon/1_btn_fromthefirst_p.png",
		"syscommon/1_btn_fromthefirst_n.png");
	int nPanelWidth = (854 / 2);
	int nPanelHeight = (740 / 2);
	int nWidth = pBtnLeft->getContentSize().width;
	int nHeight = pBtnLeft->getContentSize().height;

	CViewUtils::setSize(pBtnLeft, visibleSize.width / 2 - nWidth - 10,
		(visibleSize.height / 2 - ((740 / 2)) / 2) + 160 / 2 + 428 / 2 + 1 + 14, pBtnLeft->getContentSize().width,
		pBtnLeft->getContentSize().height);
	this->addChild(pBtnLeft);
	pBtnLeft->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackYes(pSender);
		this->removeFromParent();
	});

	//button right
	auto pBtnRight = ui::Button::create("syscommon/1_btn_continually_n.png",
		"syscommon/1_btn_continually_p.png",
		"syscommon/1_btn_continually_n.png");

	int nrWidth = pBtnRight->getContentSize().width;
	int nrHeight = pBtnRight->getContentSize().height;

	CViewUtils::setSize(pBtnRight, visibleSize.width / 2 + 10,
		(visibleSize.height / 2 - ((740 / 2)) / 2) + 160 / 2 + 428 / 2 + 1 + 14,
		pBtnRight->getContentSize().width, pBtnRight->getContentSize().height);
	this->addChild(pBtnRight);
	pBtnRight->addClickEventListener([&](cocos2d::Ref* pSender) {
		mCallbackNo(pSender);
		this->removeFromParent();
	});

}

void KPopupLayer::makeCommonUI(std::string sTitle, std::string sContent,std::string sTitlePng) {
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();	

	auto background = Sprite::create();
	background->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	background->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	background->setColor(Color3B(0, 0, 0));
	background->setOpacity(100);
	this->addChild(background);


	auto pTitle = Sprite::create(sTitlePng); // 1_popup_top.png");
//	pTitle->setTextureRect(Rect(0, 0, 854 / 2, 160 / 2));
//	pTitle->setColor(Color3B(196, 229, 229));
	CViewUtils::setSize(pTitle, visibleSize.width / 2 - pTitle->getContentSize().width / 2, visibleSize.height / 2 - (740 / 2) / 2, pTitle->getContentSize().width, pTitle->getContentSize().height);
	this->addChild(pTitle);

	auto pTitleC = Sprite::create();
	pTitleC->setTextureRect(Rect(0, 0, pTitle->getContentSize().width, 428 / 2));
	pTitleC->setColor(Color3B(255, 255, 255));
	CViewUtils::setSize(pTitleC, visibleSize.width / 2 - pTitle->getContentSize().width / 2, visibleSize.height / 2 - (740 / 2) / 2 + 160 / 2, pTitle->getContentSize().width, 428 / 2);
	this->addChild(pTitleC);

	auto pTitleB = Sprite::create("syscommon/1_popup_bottom.png");
//	pTitleB->setTextureRect(Rect(0, 0, 854 / 2, 152 / 2));
	pTitleB->setColor(Color3B(255, 255, 255));
	CViewUtils::setSize(pTitleB, visibleSize.width / 2 - pTitleB->getContentSize().width / 2, (visibleSize.height / 2 - ((740 / 2)) / 2) + 160 / 2 + 428 / 2 , pTitle->getContentSize().width, pTitle->getContentSize().height);
	this->addChild(pTitleB);

	/*
	auto title = ui::Button::create();
	title->setTitleText(sTitle);
	title->setTitleFontSize(30);
	title->setTitleColor(Color3B(0, 0, 0));
	title->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2 + (740 / 2) / 2 - 40));
	
	title->setEnabled(false);
	this->addChild(title);
	*/
	
	std::string sFontFile = "fonts/NanumGothic.ttf";

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	if (sFontFile == "fonts/NanumPen.ttf")	sFontFile = "³ª´®¼Õ±Û¾¾ Ææ";
	else if (sFontFile == "fonts/NanumGothic.ttf")	sFontFile = "³ª´®°íµñ";
#endif


	//Label* label = Label::createWithTTF("Result", sFontFile, nFontSize);

	Label* label = Label::createWithSystemFont(sContent, sFontFile, 18
		, Size(854 / 2, 428 / 2), TextHAlignment::CENTER, TextVAlignment::CENTER);

	Color3B color = ccc3(0, 0, 0);
	label->setTextColor(Color4B(color));

	CViewUtils::setSize(label, visibleSize.width / 2 - (854 / 2) / 2, visibleSize.height / 2 - (740 / 2) / 2 + 160 / 2, 854 / 2, 428 / 2);
	this->addChild(label);

	


	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {		
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, background);
}


void KPopupLayer::makeCheckBoxUI() {
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	auto background = Sprite::create("syscommon/book_popup_bg.png");
	background->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	this->addChild(background);
	
	auto imgText1 = Sprite::create("syscommon/book_popup_text_guide.png");
	imgText1->setPosition(Vec2(visibleSize.width / 2 + 60, visibleSize.height / 2));
	this->addChild(imgText1);

	auto imgText2 = Sprite::create("syscommon/book_popup_text_noreview.png");
	imgText2->setPosition(Vec2(visibleSize.width / 2 + 100, visibleSize.height / 2 - 200));
	this->addChild(imgText2);

	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, background);
}

