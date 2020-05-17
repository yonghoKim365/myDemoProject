#include "EndingScene.h"
#include "audio/include/AudioEngine.h"
#include "Contents/MyBookResources.h"
#include "Contents/MyBookSnd.h"
#include "Scene/IntroScene.h"
#include "Scene/MainScene.h"
#include "Util/MBJson.h"

#include  "mg_common/utils/MSLPManager.h"

//#include "scene/SongListenScene.h"
//#include "common/Utils.h"
//#include "common/AndroidHelper.h"

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

	Device::setKeepScreenOn(false);

	initData();
	initGUI();
	initEvent();

	// 학습 완료 처리
	finishWeek();
	// 포트폴리오 저장
	finishPortfolio();

	return true; 
}

// 포트폴리오 저장
void EndingScene::finishPortfolio()
{	
	MBJson* json = MBJson::getInstance();
	std::string strFilename = json->getPorfolioPath();

	CCLOG("finishPortfolio2 : %s", strFilename.c_str());
	MSLPManager::getInstance()->finishPortfolio(PORTFOLIO_ID, strFilename);
}

// 학습 완료 처리
void EndingScene::finishWeek()
{
	MSLPManager::getInstance()->finishProgress(true);
}

void EndingScene::initData()
{
	srand((unsigned int)time(NULL));
	endingIndex = (rand() % 4);
	
	pressedObject = nullptr;

	soundPath[0] = "res/common/sound/com_c08_ending_01.mp3";
	soundPath[1] = "res/common/sound/com_c08_ending_02.mp3";
	soundPath[2] = "res/common/sound/com_c08_ending_03.mp3";
	soundPath[3] = "res/common/sound/com_c08_ending_04.mp3";

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

	auto asset = GAFAsset::create("res/common/gaf/common_EndOfPage2/common_EndOfPage2.gaf", nullptr);
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
	//next_btn = GAFBUtton::create(mainObject->getObjectByName("next_mc"));
	exit_btn = GAFBUtton::create(mainObject->getObjectByName("exit_mc"));

	//
	if (MSLPManager::getInstance()->getHasNext())
	{
		next_btn = GAFBUtton::create(mainObject->getObjectByName("next_mc"));
	}
	else
	{
		//mainObject->getObjectByName("next_mc")->setVisible(false);

		GAFObject * nextBtn = mainObject->getObjectByName("next_mc");
		nextBtn->setVisible(false);

		float currentPosition = mainObject->getObjectByName("replay_mc")->getPositionX();
		mainObject->getObjectByName("replay_mc")->setPositionX(currentPosition + 110);

		float currentExitPosition = mainObject->getObjectByName("exit_mc")->getPositionX();
		mainObject->getObjectByName("exit_mc")->setPositionX(currentExitPosition - 142);
	}

	effectID = AudioEngine::play2d("res/common/sound/common_sfx_14.mp3");
	AudioEngine::setFinishCallback(effectID, [&](int id, const std::string& filePath){
		startNarration();
	});

	//title->setVisible(true);
}

void EndingScene::startNarration()
{
	//CCLOG("Narration Start ");
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
	effectID = AudioEngine::play2d("res/common/sound/common_sfx_15.mp3");
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

/**
* @brief  이벤트 처리(터치 다운)
* @return void
*/
void EndingScene::actionDownEvent(cocos2d::Touch* touch, Vec2 _p)
{
	//_eventDispatcher->removeEventListener(listener);

	if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("replay_mc");
		mainObject->getObjectByName("replay_mc")->gotoAndStop("Press");
		AudioEngine::play2d(FILENAME_SND_EFFECT_BTN);
	}
	else if (isButtonClick(mainObject->getObjectByName("next_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("next_mc");
		mainObject->getObjectByName("next_mc")->gotoAndStop("Press");
		AudioEngine::play2d(FILENAME_SND_EFFECT_BTN);
	}
	else if (isButtonClick(mainObject->getObjectByName("exit_mc"), touch))
	{
		pressedObject = mainObject->getObjectByName("exit_mc");
		mainObject->getObjectByName("exit_mc")->gotoAndStop("Press");
		AudioEngine::play2d(FILENAME_SND_EFFECT_BTN);
	}
}

void EndingScene::setEndingPopupCloseEvent(EndingPopupCloseEventListener * endingListener)
{
	_closeListener = endingListener;
}


/**
* @brief  이벤트 처리(터치 업)
* @return void
*/
void EndingScene::actionUpEvent(cocos2d::Touch* touch, Vec2 _p)
{
	//_eventDispatcher->removeEventListener(listener);

	if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch) )
	{
		mainObject->getObjectByName("replay_mc")->gotoAndStop("Normal");
		if (isButtonClick(mainObject->getObjectByName("replay_mc"), touch))
		{
			_eventDispatcher->removeEventListener(listener);
			AudioEngine::stop(effectID);
			
			replayGame();			
		}
	}
	else if (isButtonClick(mainObject->getObjectByName("next_mc"), touch))
	{
		mainObject->getObjectByName("next_mc")->gotoAndStop("Normal");
		if (pressedObject == mainObject->getObjectByName("next_mc"))
		{
			// Next 
			nextGame();
		}
	}
	else if (isButtonClick(mainObject->getObjectByName("exit_mc"), touch))
	{
		mainObject->getObjectByName("exit_mc")->gotoAndStop("Normal");
		if (pressedObject == mainObject->getObjectByName("exit_mc"))
		{
			// 종료
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
	//word_mc->stop();
	//word_mc->getObjectByName("star_mc")->stop();
	word_mc->setAnimationFinishedPlayDelegate(nullptr);
	word_mc->stop();

	//CCLOG("Effect Start");
	//word_mc->playSequence("Star", false);
	//effectID = AudioEngine::play2d("common/sound/common_sfx_15.mp3");
	//AudioEngine::setFinishCallback(effectID, [&](int id, const std::string& filePath){
	//	//startStarAnimation();
	//});
}

void EndingScene::startStarAnimation()
{
	effectID = AudioEngine::play2d("res/common/sound/common_sfx_15.mp3");
	word_mc->playSequence("Star", false);
}

void EndingScene::replayGame()
{
	//CCLOG("replayGame.........");
	
	//MSLPManager::getInstance()->beginProgress(1);

	// 돌아가기
	// 리플레이스 씬...... 적용
	// 타이틀 씬 부터 다시 
	//Scene* s = IntroScene::createScene();
	//Director::getInstance()->replaceScene(s);

	// 메인씬으로
	Director::getInstance()->popToRootScene();

	auto scene = MBJson::getInstance()->getMainScene();
	//((MainScene*)scene)->removeChild(((MainScene*)scene)->mPlayLayer);
	((MainScene*)scene)->closePlayLayerForEndingScene();

	//_closeListener->onCloseEnding();
	//Director::getInstance()->popScene();
	//Utils::moveToScene(s);

}

void EndingScene::exitGame()
{
	//AndroidHelper::exitGame();
	// 종료
	//CCLOG("End.........");
	// 학습 완료
//	finishWeek();

	// JNI Call
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo info;
	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "exitGame", "()V")) {
		info.env->CallStaticVoidMethod(info.classID, info.methodID);
		info.env->DeleteLocalRef(info.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


void EndingScene::nextGame()
{	
	// 다음 컨텐츠 설정하고 종료
	MSLPManager::getInstance()->startNextContent();
	
	// JNI Call
//#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
//	JniMethodInfo info;
//	if (JniHelper::getStaticMethodInfo(info, "org/cocos2dx/cpp/AppActivity", "exitGame", "()V")) {
//		info.env->CallStaticVoidMethod(info.classID, info.methodID);
//		info.env->DeleteLocalRef(info.classID);
//	}
//#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}