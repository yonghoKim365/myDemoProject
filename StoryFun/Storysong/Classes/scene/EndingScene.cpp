#include "EndingScene.h"
#include "audio/include/AudioEngine.h"
#include "data/def.h"
#include "scene/SongListenScene.h"
#include "common/Utils.h"
#include "common/AndroidHelper.h"
#include "mg_common/utils/DRMManager.h"
#include "mg_common/utils/MSLPManager.h"

using namespace experimental;

Scene * EndingScene::createScene(EndingPopupCloseEventListener * endingListener)
{
	auto scene = Scene::create();
	auto layer = EndingScene::create();
	scene->addChild(layer);
	layer->setEndingPopupCloseEvent(endingListener);
	return scene;
}

Scene * EndingScene::createScene()
{
	auto scene = Scene::create();
	auto layer = EndingScene::create();
	scene->addChild(layer);
	
	return scene;
}
bool EndingScene::init()
{
	if (!Layer::init())
	{
		return false;
	}
	initData();
	initGUI();
	initEvent();

	return true; 
}

void EndingScene::initData()
{
	srand((unsigned int)time(NULL));
	endingIndex = (rand() % 4);
	
	pressedObject = nullptr;

	soundPath[0] = "common/sound/com_c08_ending_01.mp3";
	soundPath[1] = "common/sound/com_c08_ending_02.mp3";
	soundPath[2] = "common/sound/com_c08_ending_03.mp3";
	soundPath[3] = "common/sound/com_c08_ending_04.mp3";

	labelName[0] = "Label1";
	labelName[1] = "Label2";
	labelName[2] = "Label3";
	labelName[3] = "Label4";
	
}

void EndingScene::initGUI()
{
	
	mcNames[0] = "word_mc1";
	mcNames[1] = "word_mc2";
	mcNames[2] = "word_mc3";
	mcNames[3] = "word_mc4";

	auto asset = GAFAsset::create("common/gaf/common_EndOfPage2/common_EndOfPage2.gaf", nullptr);
	auto endingObject = asset->createObjectAndRun(true);
	endingObject->setPosition(0, 1200);
	addChild(endingObject);

	endingObject->retain();
	
	mainObject = endingObject->getObjectByName("main_mc");
	title = mainObject->getObjectByName("title_mc");
	title->gotoAndStop(labelName[endingIndex]);
	//title->setVisible(false);
	word_mc = title->getObjectByName(mcNames[endingIndex]);
	word_mc->gotoAndStop(1);

	auto ani_mc = mainObject->getObjectByName("anim_mc");
	charector_mc = ani_mc->getObjectByName("charector_mc");
	charector_mc->playSequence("Play", false);
	charector_mc->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(EndingScene::onFinishedAnimation, this)));


	reply_btn = GAFBUtton::create(mainObject->getObjectByName("replay_mc"));
	next_btn = GAFBUtton::create(mainObject->getObjectByName("next_mc"));	
	exit_btn = GAFBUtton::create(mainObject->getObjectByName("exit_mc"));

	if (!MSLPManager::getInstance()->getHasNext())
	{
		GAFObject * nextBtn = mainObject->getObjectByName("next_mc");
		nextBtn->setVisible(false);

		float currentPosition = mainObject->getObjectByName("replay_mc")->getPositionX();
		mainObject->getObjectByName("replay_mc")->setPositionX(currentPosition + 110);

		float currentExitPosition = mainObject->getObjectByName("exit_mc")->getPositionX();
		mainObject->getObjectByName("exit_mc")->setPositionX(currentExitPosition - 142);

	}

	

	effectID = AudioEngine::play2d("common/sound/common_sfx_14.mp3");
	AudioEngine::setFinishCallback(effectID, [&](int id, const std::string& filePath){
		startNarration();
	});

	MSLPManager::getInstance()->finishProgress(true);
}

void EndingScene::startNarration()
{
	CCLOG("Narration Start ");
	effectID = AudioEngine::play2d(soundPath[endingIndex].c_str());
	AudioEngine::setFinishCallback(effectID, [&](int id, const std::string& filePath){
		startStarAnimation();
	});
	title->setVisible(true);

	
	word_mc->playSequence("Play", false);
	word_mc->setAnimationFinishedPlayDelegate(GAFAnimationFinishedPlayDelegate_t(CC_CALLBACK_1(EndingScene::onFinishedWordAnimation, this)));
}


void EndingScene::startEffect()
{
	effectID = AudioEngine::play2d("common/sound/common_sfx_15.mp3");
}

void EndingScene::initEvent()
{
	listener = EventListenerTouchOneByOne::create();

	listener->onTouchBegan = [&](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		cocos2d::Rect rect = this->getBoundingBox();

		if (rect.containsPoint(p))
		{
			actionDownEvent(touch, p);
			return true;
		}

		return true;
	};

	listener->onTouchEnded = [=](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		actionUpEvent(touch, p);
	};

	_eventDispatcher = cocos2d::Director::getInstance()->getEventDispatcher();
	_eventDispatcher->addEventListenerWithFixedPriority(listener, 30);
}

void EndingScene::actionDownEvent(cocos2d::Touch* touch, Vec2 _p)
{
	//_eventDispatcher->removeEventListener(listener);

	if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("replay_mc");
		mainObject->getObjectByName("replay_mc")->gotoAndStop("Press");
		AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
	}
	else if (isButtonClick(mainObject->getObjectByName("next_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("next_mc");
		mainObject->getObjectByName("next_mc")->gotoAndStop("Press");
		AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
	}
	else if (isButtonClick(mainObject->getObjectByName("exit_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("exit_mc");
		mainObject->getObjectByName("exit_mc")->gotoAndStop("Press");
		AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
	}
}

void EndingScene::setEndingPopupCloseEvent(EndingPopupCloseEventListener * endingListener)
{
	_closeListener = endingListener;
}

void EndingScene::actionUpEvent(cocos2d::Touch* touch, Vec2 _p)
{
	//_eventDispatcher->removeEventListener(listener);

	if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch) )
	{
		mainObject->getObjectByName("replay_mc")->gotoAndStop("Normal");
		if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch))
		{
			//MSLPManager::getInstance()->beginProgress(1);

			_eventDispatcher->removeEventListener(listener);
			AudioEngine::stop(effectID);
			Scene* s = SongListenScene::createScene();
			Director::getInstance()->replaceScene(s);
			
		}
	}
	else if (isButtonClick(mainObject->getObjectByName("next_mc"), touch))
	{
		mainObject->getObjectByName("next_mc")->gotoAndStop("Normal");
		if (pressedObject == mainObject->getObjectByName("next_mc"))
		{
			CCLOG("CLICK NEXT");
			MSLPManager::getInstance()->startNextContent();
			
			//exitGame();
		}
	}
	else if (isButtonClick(mainObject->getObjectByName("exit_mc"), touch))
	{
		mainObject->getObjectByName("exit_mc")->gotoAndStop("Normal");
		if (pressedObject == mainObject->getObjectByName("exit_mc"))
		{
			CCLOG("CLICK EXIT");
			exitGame();
			
		}
	}
	pressedObject = nullptr;
}

bool EndingScene::isButtonClick(GAFObject * button, cocos2d::Touch* touch)
{
	Vec2 localPoint = button->convertTouchToNodeSpace(touch);
	localPoint = cocos2d::PointApplyTransform(localPoint, button->getNodeToParentTransform());
	Rect r = button->getBoundingBoxForCurrentFrame();

	if (r.containsPoint(localPoint))
		return true;
	else
		return false;
}

void EndingScene::setClickEventListener(EndingEventListener * listener)
{
	_clickListener = listener;
}

void EndingScene::onFinishedAnimation(gaf::GAFObject * object)
{

}

void EndingScene::onFinishedWordAnimation(gaf::GAFObject * object)
{
	word_mc->setAnimationFinishedPlayDelegate(nullptr);
	word_mc->stop();
}

void EndingScene::startStarAnimation()
{
	effectID = AudioEngine::play2d("common/sound/common_sfx_15.mp3");
	word_mc->playSequence("Star", false);
}

void EndingScene::exitGame()
{
	AndroidHelper::exitGame();
}