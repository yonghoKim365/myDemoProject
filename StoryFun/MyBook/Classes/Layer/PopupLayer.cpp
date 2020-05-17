#include "PopupLayer.h"
#include "Contents/MyBookSnd.h"
#include "Contents/MyBookResources.h"
#include "Util/Pos.h"
#include "Scene/MainScene.h"
#include "Scene/DrawScene.h"
#include "Scene/PlayScene.h"
#include "Layer/RecordLayer.h"
#include "MSLPManager.h"

// for AudioEngine
//using namespace cocos2d::experimental;
/**
* @brief 생성자
* @param parent
* @prarm type
*/
PopupLayer::PopupLayer(Node* parent, POPUP_TYPE type)
{
	mParent = parent;
	mType = type;	
}

/**
* @brief 소멸자
*/
PopupLayer::~PopupLayer()
{
	CCLOG("POPUP Destroy......");
	CC_SAFE_RELEASE(mPopupSp);	
}

/**
* @brief  초기화 함수
* @param  parent
* @param  type
* @return Popup
*/
PopupLayer* PopupLayer::create(Node* parent, POPUP_TYPE type)
{
	PopupLayer* p = new(std::nothrow) PopupLayer(parent, type);
	if (p && p->init())
	{
		p->autorelease();
		return p;
	}
	CC_SAFE_RELEASE(p);
	p = nullptr;
	return nullptr;
}

/**
* @brief  초기화 함수
* @param  parent
* @param  type
* @return Popup
*/
PopupLayer* PopupLayer::getIns(Node* parent, POPUP_TYPE type)
{
	auto layer = PopupLayer::create(parent, type);

	return layer;
}


/**
* @brief  초기화 함수
* @return bool 초기화 성공 유무
*/
bool PopupLayer::init()
{
	if (!Layer::init())
	{
		return false;
	}
	mCenter = Pos::getCenterPt();

	createUI();
	return true;
}

//----------------------------------------------------------------------
//	public
//----------------------------------------------------------------------
/**
* @brief  파라메터에 따른 Show / Hide
* @param  visible true : Show , false : Hide
* @param  immediate 화면 정지
* @return Popup* 초기화 성공 유무
*/
PopupLayer* PopupLayer::showHide(bool visible, bool immediate)
{
	this->mVisible = visible;

	if (visible)
	{
		if (immediate)
		{
			Director::getInstance()->pause();
		}

		CCLOG("POPUP SHOWHIDE ........ VISIBLE[%d]", visible);
		mParent->addChild(this, 100);
	}
	else
	{
		CCLOG("POPUP SHOWHIDE FALSE........ VISIBLE[%d]", visible);
		Director::getInstance()->resume();
		this->removeFromParentAndCleanup(true);
	}
	return this;
}


//----------------------------------------------------------------------
//	UI
//----------------------------------------------------------------------
/**
* @brief  사용자 인터페이스 생성
* @return void
*/
void PopupLayer::createUI()
{
	CCLOG("POPUP createUI TYPE[%d]", mType);
		
	// 기준 Sprite
	if (mPopupSp == nullptr)
	{		
		mPopupSp = Sprite::create();
		mPopupSp->retain();
	}

	// Dim 처리
	LayerColor *bgLayer = LayerColor::create(Color4B(0, 0, 0, 179));
	mPopupSp->addChild(bgLayer);

	// bg	
	auto bg = Sprite::create(FILENAME_POPUP_BG);
	bg->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	bg->setPosition(Pos::getPopupPt());
	mPopupSp->addChild(bg);

	// yoonie 
	std::string yoonie;
	if (mType < OPTION_POP)
	{
		yoonie = StringUtils::format("%s", FILENAME_POPUP_HELP_YOONIE);
	}
	else
	{
		yoonie = StringUtils::format("%s", FILENAME_POPUP_OPTION_YOONIE);
	}

	auto charYoonie = Sprite::create(yoonie.c_str());
	charYoonie->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	charYoonie->setPosition(Pos::getPopupYooniePt());
	mPopupSp->addChild(charYoonie);

	//close button
	// 옵션팝업에는 닫기(X) 버튼이 없다. 도움 팝업에만 있음.
	if (mType < OPTION_POP)
	{
		auto btnClose = MenuItemImage::create(FIlENAME_BTN_POPUP_CLOSE_N, 
											  FIlENAME_BTN_POPUP_CLOSE_P,
											  CC_CALLBACK_1(PopupLayer::onPopupCloseBtnClick, this));
		btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
		btnClose->setPosition(Pos::getPopupHelpCloseBtnPt());

		auto m2 = Menu::create(btnClose, nullptr);		
		m2->setPosition(Vec2::ZERO);
		mPopupSp->addChild(m2);
	}	

	// view
	// 타입에 따른 내용 표시
	switch (mType)
	{
	case HELP_MAIN:
		createHelpMain();
		break;

	case HELP_RECORD:
		createHelpRecord();
		break;

	case HELP_DRAW:
		createHelpDraw();
		break;

	case HELP_CAMERA:
		break;

	case OPTION_POP:
		createOption();
		break;
		
	case DRAW_SAVE:
		createSaveDraw(); 
		break;

	case DRAW_ERASE_ALL:
		createEraseAllDraw();
		break;

	case DRAW_CLOSE:
		createCloseDraw();
		break;

	case RECORD_CLOSE:
		createCloseRecord();
		break;

	case PLAY_CLOSE:
		createClosePlay();
		break;

	case RECORD_DELETE:
		createDeleteRecord();
		break;

	case PLAY_ENDING:
		createEndingPlay();
		break;

	case MAIN_EXIT:
		createExitMain();
		break;

	case PRELOADING:
		createPreloading();
		break;

	default:
		break;
	} // end of switch

	this->addChild(mPopupSp, DEPTH_LAYER_CUBE_POPUP);
}

/**
* @brief  재생하기 : 끝 팝업
* @return void
*/
void PopupLayer::createPreloading()
{
	
}

/**
* @brief  재생하기 : 끝 팝업
* @return void
*/
void PopupLayer::createExitMain()
{
	CCLOG("createExitMain............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onMainCloseBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onMainResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_RECORD_EXIT_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  재생하기 : 끝 팝업
* @return void
*/
void PopupLayer::createEndingPlay()
{
	CCLOG("createEndingPlay............");
	// Replay 버튼
	auto btnClose = MenuItemImage::create(FILENAME_PLAY_POPUP_BTN_REPLAY_N,
		FILENAME_PLAY_POPUP_BTN_REPLAY_P,
		CC_CALLBACK_1(PopupLayer::onPlayEndingReplayBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPlayEndingPopupFirstBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// Next 버튼
	auto btn = MenuItemImage::create(FILENAME_PLAY_POPUP_BTN_NEXT_N,
		FILENAME_PLAY_POPUP_BTN_NEXT_P,
		CC_CALLBACK_1(PopupLayer::onPlayEndingNextBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPlayEndingPopupSecondBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_PLAY_ENDING_TXT);
	Size size = Pos::getPlayEndingPopupTextSize();
	Vec2 pos = Pos::getPlayEndingPopupTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  닫기 버튼 : 팝업창 생성
* @return void
*/
void PopupLayer::createDeleteRecord()
{
	CCLOG("createDeleteRecord............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onRecordDeleteBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onRecordDeleteResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_RECORD_DELETE_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  닫기 버튼 : 팝업창 생성
* @return void
*/
void PopupLayer::createClosePlay()
{
	CCLOG("createClosePlay............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onPlayCloseBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onPlayResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_RECORD_EXIT_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  닫기 버튼 : 팝업창 생성
* @return void
*/
void PopupLayer::createCloseRecord()
{
	CCLOG("createCloseDraw............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onRecordCloseBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onRecordResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_RECORD_EXIT_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}

/**
* @brief  닫기 버튼 : 팝업창 생성
* @return void
*/
void PopupLayer::createCloseDraw()
{
	// 내용
	CCLOG("createCloseDraw............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onDrawCloseBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onDrawResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_DRAW_EXIT_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  
* @return void
*/
void PopupLayer::createSaveDraw()
{
	// 내용
	CCLOG("createSaveDraw............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onDrawSaveBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onDrawResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_DRAW_SAVE_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  
* @return void
*/
void PopupLayer::createEraseAllDraw()
{
	// 내용
	//CCLOG("createEraseAllDraw............");
	// Yes 버튼
	auto btnClose = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_YES_N,
		FILENAME_COMMON_POPUP_BTN_YES_P,
		CC_CALLBACK_1(PopupLayer::onDrawAllEraseBtnClick, this));
	btnClose->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnClose->setPosition(Pos::getPopupOptionSecondBtnPt());
	auto menu = Menu::create(btnClose, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// No 버튼
	auto btn = MenuItemImage::create(FILENAME_COMMON_POPUP_BTN_NO_N,
		FILENAME_COMMON_POPUP_BTN_NO_P,
		CC_CALLBACK_1(PopupLayer::onDrawResumeBtnClick, this));
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupOptionFirstBtnPt());
	auto menuClose = Menu::create(btn, nullptr);
	menuClose->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menuClose);

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_DRAW_ALL_ERASE_TXT);
	Size size = Pos::getPopupOptionTextSize();
	Vec2 pos = Pos::getPopupOptionTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  도움말 : 메인 도움말 생성
* @return void
*/
void PopupLayer::createHelpMain()
{
	// 내용
	CCLOG("createHelpMain............");
	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_CUBE_TEXT);
	Size size = Pos::getPopupHelpTextSize();
	Vec2 pos = Pos::getPopupHelpTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  도움말 : 녹음하기 도움말 생성
* @return void
*/
void PopupLayer::createHelpRecord()
{
	CCLOG("createHelpRecord............");
	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_RECORD_HELP_TXT);
	Size size = Pos::getPopupHelpTextSize();
	Vec2 pos = Pos::getPopupHelpTextPt();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);
}


/**
* @brief  도움말 : 그리기 도움말 생성
* @return void
*/
void PopupLayer::createHelpDraw()
{		
	CCLOG("createHelpDraw............");

	// 텍스트 구문
	auto text = Sprite::create(FILENAME_COMMON_POPUP_DRAW_HELP_TXT);	
	Vec2 pos = Pos::getPopupHelpDrawTextPt();
	Size size = Pos::getPopupHelpDrawTextSize();
	pos.x += size.width / 2;
	pos.y -= size.height / 2;
	text->setPosition(pos);
	mPopupSp->addChild(text);

	// 체크박스
	auto item  = MenuItemImage::create(FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_N, FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_N);
	auto item2 = MenuItemImage::create(FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_C, FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_C);
	toggle = MenuItemToggle::createWithCallback(CC_CALLBACK_1(PopupLayer::helpDrawCallback, this), item, item2, nullptr);
	toggle->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	toggle->setPosition(Pos::getPopupHelpDrawCheckBoxPt());
	auto menu = Menu::create(toggle, nullptr);
	menu->setPosition(Vec2::ZERO);
	mPopupSp->addChild(menu);

	// 체크박스 텍스트
	/*auto ctext = Sprite::create(FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT);	
	ctext->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	ctext->setPosition(Pos::getPopupHelpDrawCheckBoxTextPt());
	mPopupSp->addChild(ctext);*/

	// 체크박스 텍스트
	//cocos2d::ui::Button* btn = cocos2d::ui::Button::create(FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT);
	cocos2d::ui::Button* btn = cocos2d::ui::Button::create(FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT,
		FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT,
		FILENAME_COMMON_POPUP_DRAW_HELP_CHECKBOX_TXT);
	btn->setTouchEnabled(true);
	btn->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btn->setPosition(Pos::getPopupHelpDrawCheckBoxTextPt());
	btn->addTouchEventListener(CC_CALLBACK_2(PopupLayer::onTouchText, this));
	btn->setSwallowTouches(false);
	mPopupSp->addChild(btn);

	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);
}


// 그리기 도움말 콜백함수
void PopupLayer::helpDrawCallback(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	std::string strKey = StringUtils::format(USERDEFAULT_KEY_DRAW_HELP_CHECK, MSLPManager::getInstance()->getBookNum());
	MenuItemToggle *item = (MenuItemToggle *)sender;	

	if (item->getSelectedIndex() == 0)
	{
		UserDefault::getInstance()->setBoolForKey(strKey.c_str(), false);
	}
	else
	{
		UserDefault::getInstance()->setBoolForKey(strKey.c_str(), true);
	}
	//CCLOG("helpDrawCallback : %d", flag);		
}


/**
* @brief  옵션 : 도움말 생성
* @return void
*/
void PopupLayer::createOption()
{
	CCLOG("createOption............");
}

//----------------------------------------------------------------------
//	버튼 이벤트
//----------------------------------------------------------------------
/**
* @brief  저장 Yes 버튼(그리기에서)
* @param  sender sender
* @return void
*/
void PopupLayer::onDrawAllEraseBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("ALL ERASE POPUP......");
	CC_ASSERT(mParent != nullptr);
	((DrawScene*)mParent)->clearAllDraw();

	showHide(false, false);
}

/**
* @brief  저장 Yes 버튼(그리기에서)
* @param  sender sender
* @return void
*/
void PopupLayer::onDrawSaveBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	
	CCLOG("DRAW SAVE POPUP......");

	CC_ASSERT(mParent != nullptr);
	((DrawScene*)mParent)->saveFileDraw();
	
	showHide(false, false);	

	// JNI CALL	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "showSaveUi", "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}

/**
* @brief  도움말 닫기 버튼(그리기에서)
* @param  sender sender
* @return void
*/
void PopupLayer::onDrawCloseBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	/*auto audio = SimpleAudioEngine::getInstance();
	audio->playEffect(FILENAME_SND_EFFECT_BTN_CLICK);*/
	//CCLOG("CLOSE DRAW POPUP......");	
	CC_ASSERT(mParent != nullptr);
	((DrawScene*)mParent)->backToMainScene();		
}

/**
* @brief  도움말 닫기 버튼(그리기에서)
* @param  sender sender
* @return void
*/
void PopupLayer::onDrawResumeBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	//CCLOG("RESUME DRAW POPUP......");
	CC_ASSERT(mParent != nullptr);
	
	if (mType == HELP_DRAW)
	{
		((DrawScene*)mParent)->menuPopupClose(true);
	}
	else
	{
		// 나레이션 제외
		((DrawScene*)mParent)->menuPopupClose(false);
		//((DrawScene*)mParent)->setEnableTouchBtn(true);
	}
	
	showHide(false, false);
}

/**
* @brief  닫기 버튼(녹음하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onRecordCloseBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("CLOSE RECORD POPUP......");
	CC_ASSERT(mParent != nullptr);
	((RecordLayer*)mParent)->menuPopupClose();
	((RecordLayer*)mParent)->closeRecord();
}

/**
* @brief  도움말 닫기 버튼(녹음하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onRecordResumeBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("RESUME RECORD POPUP......");
	CC_ASSERT(mParent != nullptr);
	((RecordLayer*)mParent)->menuPopupClose();
	
	showHide(false, false);
}


/**
* @brief  닫기 버튼(재생하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onPlayCloseBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("CLOSE PLAY POPUP......");
	CC_ASSERT(mParent != nullptr);
	((PlayScene*)mParent)->menuPopupClose();
	((PlayScene*)mParent)->closePlay();	
}


/**
* @brief  닫기 버튼(재생하기) - 
* @param  sender sender
* @return void
*/
void PopupLayer::onPlayResumeBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("RESUME PLAY POPUP......");
	CC_ASSERT(mParent != nullptr);
	((PlayScene*)mParent)->menuPopupClose();
	
	showHide(false, false);
}


/**
* @brief  삭제 버튼(녹음하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onRecordDeleteBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("CLOSE RECORD POPUP......");
	CC_ASSERT(mParent != nullptr);
	((RecordLayer*)mParent)->menuPopupClose();
	((RecordLayer*)mParent)->deleteRecord();
}


/**
* @brief  삭제 버튼(녹음하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onRecordDeleteResumeBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("RESUME RECORD POPUP......");
	CC_ASSERT(mParent != nullptr);
	((RecordLayer*)mParent)->menuPopupClose();	
	showHide(false, false);
}


/**
* @brief  Replay 버튼(재생하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onPlayEndingReplayBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("ENDING POPUP REPLAY......");
	CC_ASSERT(mParent != nullptr);
	//((PlayScene*)mParent)->menuPopupClose();
	// Replay
	// 재생하기 호출	
	((PlayScene*)mParent)->replayPlayScene();
	showHide(false, false);
}


/**
* @brief  Next 버튼(재생하기)
* @param  sender sender
* @return void
*/
void PopupLayer::onPlayEndingNextBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("ENDING POPUP NEXT......");
	CC_ASSERT(mParent != nullptr);
	//((PlayScene*)mParent)->menuPopupClose();
	// Next......
	((PlayScene*)mParent)->toEndingScene();
	showHide(false, false);
}


/**
* @brief  닫기 버튼(메인씬)
* @param  sender sender
* @return void
*/
void PopupLayer::onMainCloseBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("EXIT MAIN POPUP......");
	CC_ASSERT(mParent != nullptr);
	((MainScene*)mParent)->menuPopupClose();
	((MainScene*)mParent)->exitPlay();
}


/**
* @brief  닫기 버튼(메인씬) -
* @param  sender sender
* @return void
*/
void PopupLayer::onMainResumeBtnClick(Ref* sender)
{
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	CCLOG("RESUME EXIT MAIN POPUP......");
	CC_ASSERT(mParent != nullptr);
	((MainScene*)mParent)->menuPopupClose();

	showHide(false, false);
}


/**
* @brief  닫기 버튼
* @param  sender sender
* @return void
*/
void PopupLayer::onPopupCloseBtnClick(Ref* sender)
{
	CCLOG("CLOSE POPUP......: %d", mType);
	// 효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
	bool flag = false;
	switch (mType)
	{
	case HELP_MAIN:
		CCLOG("MAIN CLOSE.....");
		((MainScene*)mParent)->menuPopupClose();
		break;

	case HELP_DRAW:
		// 체크 
		if (MBJson::getInstance()->getIsOpenningDraw() == false)
		{
			flag = true;
		}
		((DrawScene*)mParent)->menuPopupClose(flag);
		break;

	case HELP_RECORD:
	//case RECORD_CLOSE:
		((RecordLayer*)mParent)->menuPopupClose();
		break;
	} // end of switch

	showHide(false, false);
}


/**
* @brief   선택
* @param  Ref* pSender
* @param  ui::Widget::TouchEventType type
* @return void
*/
void PopupLayer::onTouchText(Ref* pSender, ui::Widget::TouchEventType type)
{
	switch (type)
	{
	case ui::Widget::TouchEventType::BEGAN:		
		break;

	case ui::Widget::TouchEventType::MOVED:
		// 취소
		//CCLOG("M");		
		break;

	case ui::Widget::TouchEventType::ENDED:
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
		if (toggle)
		{
			std::string strKey = StringUtils::format(USERDEFAULT_KEY_DRAW_HELP_CHECK, MSLPManager::getInstance()->getBookNum());
			int index = toggle->getSelectedIndex();
			if (index == 1)
			{				
				toggle->setSelectedIndex(0);
				UserDefault::getInstance()->setBoolForKey(strKey.c_str(), false);
			}
			else
			{				
				toggle->setSelectedIndex(1);
				UserDefault::getInstance()->setBoolForKey(strKey.c_str(), true);
			}
		}	
		
		break;

	case ui::Widget::TouchEventType::CANCELED:		
		break;
	} // end of switch
}