#include "UILayer.h"
#include "audio/include/AudioEngine.h"
#include "data/def.h"
#include "data/JsonInfo.h"
#include "mg_common/utils/MSLPManager.h"

using namespace experimental;

UILayer * UILayer::create()
{
	UILayer * ref = new UILayer();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->initGUI();
		return ref;
	}
	else
	{
		CC_SAFE_RELEASE(ref);
		return nullptr;
	}
}

void UILayer::initGUI()
{
	isSkipPressed = false;
	auto exitButton = Button::create("common/img/common_btn_quit_n.png", "common/img/common_btn_quit_p.png", "common/img/common_btn_quit_p.png");
	exitButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
			break;
		case Widget::TouchEventType::ENDED:
			exitGame();
			break;
		default:
			break;
		}
	});
	//1098
	exitButton->setPosition(Vec2(100, 1098));
	addChild(exitButton);

	skipButton = Button::create("common/img/common_btn_next_n.png", "common/img/common_btn_next_p.png", "common/img/common_btn_next_p.png");
	skipButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
		switch (type)
		{
		case Widget::TouchEventType::BEGAN:
			if (isSkipPressed)	return;

			CCLOG("Skip button touch began --- ");
			AudioEngine::play2d("common/sound/common_sfx_btn_01.mp3");
			CCLOG("Skip button touch began --- end");
			break;
		case Widget::TouchEventType::ENDED:
			if (isSkipPressed)	return;
			isSkipPressed = true;
			skipScene();
			break;
		default:
			break;
		}
	});
	skipButton->setPosition(Vec2(1820, 98));
	addChild(skipButton);

	JsonInfo * jsonInfo = JsonInfo::create();
	
	if (jsonInfo->isFirstPlay)
	{
		skipButton->setVisible(false);
	}
}

void UILayer::showExitButton()
{
	//exitButton
}

void UILayer::showSkipButton(bool isShowAffodance)
{
	if (isHideSkip)	return;

	skipButton->setVisible(true);
	if (isShowAffodance)
	{
		FadeTo * fadeOut = FadeTo::create(0.5f, 76.5);
		FadeTo * fadeIn = FadeTo::create(0.5f, 255);
		Sequence * sequence = Sequence::create(fadeOut, fadeIn, NULL);
		RepeatForever *	action = RepeatForever::create(sequence);

		skipButton->runAction(action);
	}
	/*if (isShowAffodance)
		skipEffect->setVisible(true);
	else
		skipEffect->setVisible(false);*/
}

void UILayer::hideSkipButton()
{
	isHideSkip = true;
	skipButton->setVisible(false);
}

void UILayer::makeStep2Scene()
{
	skipButton->setVisible(false);
	/*skipEffect->setVisible(false);*/
}

void UILayer::skipScene()
{
	skipListener->onSkipScene();
}

void UILayer::setSkipButtonEventListener(SkipButtonEventListener * listener)
{
	skipListener = listener;
}

void UILayer::exitGame()
{
	MSLPManager::getInstance()->finishProgress(false);

	
#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;

	if (JniHelper::getStaticMethodInfo(t
		, "net/minigate/smartdoodle/storyfun/viewer/song/AppActivity"
		, "exitGame"
		, "()V"))
	{

		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#else
	Director::getInstance()->end();
#endif

}
