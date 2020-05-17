#include "SelectDialog.h"
#include "audio/include/AudioEngine.h"
#include "data/def.h"
#include "data/PlayInfo.h"
#include "../../data/JsonInfo.h"
#include "mg_common/utils/MSLPManager.h"

using namespace experimental;

#define UI_BUTTON_POSITION_Y 400

#define UI_CHARECTOR_POSITION_X 405
#define UI_GUIDE_POSITION_Y		670

#define CB_CHECK_PATH	"common/popup/cb_check.png"
#define CB_UNCHECK_PATH "common/popup/cb_uncheck.png"
#define CB_TEXT_PATH	"common/popup/notShow.png"	

void SelectDialogStart::initDialog(DialogSelectListener* listener)
{
	Director::getInstance()->pause();

	_eventListener = listener;

	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg);

	Sprite * charector = Sprite::create("common/popup/common_popup_chant_storysong_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector, 2);

	Sprite * guide = Sprite::create("common/popup/common_popup_stoysong_guide2.png");
	guide->setPosition(Vec2(960, UI_GUIDE_POSITION_Y));
	addChild(guide, 3);

	AudioEngine::play2d(POPUP_OPTION_SOUND_PATH);

	auto playButton = Button::create("common/popup/common_popup_stoysong_btn_play_n.png", "common/popup/common_popup_stoysong_btn_play_p.png", "common/popup/common_popup_stoysong_btn_play_p.png");
	playButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_PLAY);
			break;
		default:
			break;
		}
	});

	float margin = 20.0f;
	playButton->setPosition(Vec2(797, UI_BUTTON_POSITION_Y));
	this->addChild(playButton);


	auto listenButton = Button::create("common/popup/common_popup_stoysong_btn_listen_n.png", "common/popup/common_popup_stoysong_btn_listen_p.png", "common/popup/common_popup_stoysong_btn_listen_p.png");
	listenButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_LISTEN);
			break;
		default:
			break;
		}
	});

	listenButton->setPosition(Vec2(1123, UI_BUTTON_POSITION_Y));
	this->addChild(listenButton);
}



void SelectDialogFinish::initDialog(DialogSelectListener* listener)
{
	Director::getInstance()->pause();
	_eventListener = listener;

	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg);

	Sprite * charector = Sprite::create("common/popup/common_popup_chant_storysong_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector, 2);

	Sprite * guide = Sprite::create("common/popup/common_popup_stoysong_guide2.png");
	

	guide->setPosition(Vec2(960, UI_GUIDE_POSITION_Y));
	addChild(guide, 3);
	AudioEngine::play2d(POPUP_OPTION_SOUND_PATH);

	auto playButton = Button::create("common/popup/common_popup_stoysong_btn_play_n.png", "common/popup/common_popup_stoysong_btn_play_p.png", "common/popup/common_popup_stoysong_btn_play_p.png");
	playButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_PLAY);
			break;
		default:
			break;
		}
	});

	float margin = 20.0f;
	playButton->setPosition(Vec2(676, UI_BUTTON_POSITION_Y));
	this->addChild(playButton);


	auto listenButton = Button::create("common/popup/common_popup_stoysong_btn_listen_n.png", "common/popup/common_popup_stoysong_btn_listen_p.png", "common/popup/common_popup_stoysong_btn_listen_p.png");
	listenButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_LISTEN);
			break;
		default:
			break;
		}
	});

	listenButton->setPosition(Vec2(960, UI_BUTTON_POSITION_Y));
	this->addChild(listenButton);

	auto quitButton = Button::create("common/popup/common_popup_btn_quit_n.png", "common/popup/common_popup_btn_quit_p.png", "common/popup/common_popup_btn_quit_p.png");
	quitButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			//_listener->onStartListen();
			onSelectMode(DIALOG_SELECT_MODE::SELECT_QUIT);
			break;
		default:
			break;
		}
	});

	quitButton->setPosition(Vec2(1244, UI_BUTTON_POSITION_Y));
	this->addChild(quitButton);
}




void ListenFinishedDialog::initDialog(DialogSelectListener * listener)
{
	Director::getInstance()->pause();
	_eventListener = listener;

	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg);

	Sprite * charector = Sprite::create("common/popup/common_popup_chant_storysong_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector, 2);

	Sprite * guide = Sprite::create("common/popup/common_popup_stoysong_guide2.png");
	guide->setPosition(Vec2(960, UI_GUIDE_POSITION_Y));
	addChild(guide, 3);
	AudioEngine::play2d(POPUP_OPTION_SOUND_PATH);

	auto asset = GAFAsset::create("common/popup/Affordance/Affordance.gaf");
	Affordance = asset->createObjectAndRun(true);

	auto playButton = Button::create("common/popup/common_popup_stoysong_btn_play_n.png", "common/popup/common_popup_stoysong_btn_play_p.png", "common/popup/common_popup_stoysong_btn_play_p.png");
	playButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_PLAY);
			break;
		default:
			break;
		}
	});

	float margin = 20.0f;
	


	auto listenButton = Button::create("common/popup/common_popup_stoysong_btn_listen_n.png", "common/popup/common_popup_stoysong_btn_listen_p.png", "common/popup/common_popup_stoysong_btn_listen_p.png");
	listenButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_LISTEN);
			break;
		default:
			break;
		}
	});

	if (!JsonInfo::create()->isFirstPlay)
	{
		playButton->setPosition(Vec2(676, UI_BUTTON_POSITION_Y));
		this->addChild(playButton);

		listenButton->setPosition(Vec2(960, UI_BUTTON_POSITION_Y));
		this->addChild(listenButton);

		auto quitButton = Button::create("common/popup/common_popup_btn_quit_n.png", "common/popup/common_popup_btn_quit_p.png", "common/popup/common_popup_btn_quit_p.png");
		quitButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			switch (type)
			{
			case Widget::TouchEventType::BEGAN:
				AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
				break;
			case Widget::TouchEventType::ENDED:
				//_listener->onStartListen();
				onSelectMode(DIALOG_SELECT_MODE::SELECT_QUIT);
				break;
			default:
				break;
			}
		});

		quitButton->setPosition(Vec2(1244, UI_BUTTON_POSITION_Y));
		this->addChild(quitButton);
	}
	else
	{
		Director::getInstance()->resume();

		Affordance->setPosition(Vec2(797-125, UI_BUTTON_POSITION_Y+100));
		Affordance->retain();
		Affordance->playSequence("Affordance", true); 
		this->addChild(Affordance,100);

		playButton->setPosition(Vec2(797, UI_BUTTON_POSITION_Y));
		this->addChild(playButton);

		listenButton->setPosition(Vec2(1123, UI_BUTTON_POSITION_Y));
		this->addChild(listenButton);
	}
}


void ListenStartDialog::initDialog(DialogSelectListener * listener)
{
	_eventListener = listener;

	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg, 1);

	Sprite * charector = Sprite::create("common/popup/common_popup_guide_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector, 2);

	Sprite * guide = Sprite::create("common/popup/common_popup_stoysong_guide3.png");
	guide->setPosition(Vec2(960, UI_GUIDE_POSITION_Y));
	addChild(guide, 3);

	cb = CheckBox::create(CB_UNCHECK_PATH, CB_CHECK_PATH);
	cb->setPosition(Vec2(800, 400));
	addChild(cb, 100);
	cb->setSelected(false);
	cb->addEventListener([=](cocos2d::Ref* target, cocos2d::ui::CheckBox::EventType event_type) {
		AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH2);
        
        if (event_type == cocos2d::ui::CheckBox::EventType::SELECTED)
            UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), true);
        else
            UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false);

		UserDefault::getInstance()->flush();
	});

	
	auto text = Button::create(CB_TEXT_PATH, CB_TEXT_PATH, CB_TEXT_PATH);
	text->setPosition(Vec2(1000, 400));
	addChild(text, 101);
	text->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH2);
			break;
		case Widget::TouchEventType::ENDED:
			if (cb->getSelectedState())
            {
                UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false);
				UserDefault::getInstance()->flush();
				cb->setSelected(false);
			}
			else
            {
                UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), true);
				UserDefault::getInstance()->flush();
				cb->setSelected(true);
			}
			break;
		default:
			break;
		}
	});

	auto quitButton = Button::create("common/popup/common_popup_btn_close_n.png", "common/popup/common_popup_btn_close_p.png", "common/popup/common_popup_btn_close_p.png");
	quitButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_CLOSE);
			break;
		default:
			break;
		}
	});

	quitButton->setPosition(Vec2(1432, 891));
	this->addChild(quitButton, 4);
}



void HelpDialog::initDialog(DialogSelectListener * listener)
{
	_eventListener = listener;

	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg,1);

	Sprite * charector = Sprite::create("common/popup/common_popup_guide_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector,2);

	Sprite * guide;

	if (JsonInfo::create()->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_A)
		guide = Sprite::create("common/popup/common_popup_stoysong_guide.png");
	else if (JsonInfo::create()->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_B)
	guide = Sprite::create("common/popup/pop_text_01_re.png");
	else if (JsonInfo::create()->currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
		guide = Sprite::create("common/popup/pop_text_04_re.png");
	else
		guide = Sprite::create("common/popup/pop_text_03_re.png");

	guide->setPosition(Vec2(960, UI_GUIDE_POSITION_Y));
	addChild(guide, 3);

	cb = CheckBox::create(CB_UNCHECK_PATH, CB_CHECK_PATH);
	cb->setPosition(Vec2(800, 400));
	addChild(cb, 100);
	cb->setSelected(false);
	cb->addEventListener([=](cocos2d::Ref* target, cocos2d::ui::CheckBox::EventType event_type) {
		AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH2);
		if (event_type == cocos2d::ui::CheckBox::EventType::SELECTED)
		{
            
            UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_PLAY_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), true);
			UserDefault::getInstance()->flush();
		}
		else
        {
            UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_PLAY_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false);
			UserDefault::getInstance()->flush();
		}
	});

	auto text = Button::create(CB_TEXT_PATH, CB_TEXT_PATH, CB_TEXT_PATH);
	text->setPosition(Vec2(1000, 400));
	addChild(text, 101);
	text->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH2);
			break;
		case Widget::TouchEventType::ENDED:
			if (cb->getSelectedState())
            {
                UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_PLAY_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false);
				UserDefault::getInstance()->flush();
				cb->setSelected(false);
			}
			else
            {
                UserDefault::sharedUserDefault()->setBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_PLAY_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), true);
				UserDefault::getInstance()->flush();
				cb->setSelected(true);
			}
			break;
		default:
			break;
		}
	});



	auto quitButton = Button::create("common/popup/common_popup_btn_close_n.png", "common/popup/common_popup_btn_close_p.png", "common/popup/common_popup_btn_close_p.png");
	quitButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			onSelectMode(DIALOG_SELECT_MODE::SELECT_CLOSE);
			break;
		default:
			break;
		}
	});

	quitButton->setPosition(Vec2(1432, 891));
	this->addChild(quitButton,4);
}



void LoadingDialog::initDialog()
{
	Sprite * bg = Sprite::create("common/popup/common_popup_bg.png");
	bg->setPosition(Vec2(960, 600));
	addChild(bg, 1);

	Sprite * charector = Sprite::create("common/popup/common_popup_guide_yoonie.png");
	charector->setPosition(Vec2(UI_CHARECTOR_POSITION_X, 514));
	addChild(charector, 2);


	Sprite * guide = Sprite::create("common/popup/common_popup_stoysong_loading.png");
	guide->setPosition(Vec2(960, 600));
	addChild(guide, 3);
}


void DimLayer::initDialog()
{
	
}