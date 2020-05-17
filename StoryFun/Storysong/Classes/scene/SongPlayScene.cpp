#include "SongPlayScene.h"
#include "audio/include/AudioEngine.h"
#include "scene/SongListenScene.h"
#include "common/AndroidHelper.h"
#include "mg_common/utils/MSLPManager.h"

using namespace experimental;

Scene * SongPlayScene::createScene()
{
	auto scene = Scene::create();
	auto layer = SongPlayScene::create();
	scene->addChild(layer);
	return scene;
}

bool SongPlayScene::init()
{
	if (!Layer::init())
	{
		return false;
	}

	initData();

	initGUI();

	return true;
}

void SongPlayScene::initData()
{
	SongScene::initData();

	setCurrentScene(SceneType::SCENE_PLAY);
}

void SongPlayScene::initGUI()
{
	SongScene::initGUI();
	MSLPManager::getInstance()->progress(2);

	contentsLoader = ContentsLoader::create();
	addChild(contentsLoader);

	uiLayer->makeStep2Scene();

	bgSprite->setTexture(jsonInfo->mainBGPath.c_str());
	
	auto director = Director::getInstance();
	Size pixelSize = director->getWinSizeInPixels();
	CCLOG("screen width = %f", pixelSize.width);
	if (pixelSize.width == 1920.0f)
	{
		CCLOG("contents width = %f", bgSprite->getContentSize().width);
		//bgSprite->setContentSize(Size(1920.0f, 1200.0f));
		float scale = pixelSize.width / bgSprite->getContentSize().width;
		bgSprite->setScale(scale);
	}


	if (jsonInfo->currentAnimationType != ANIMATION_TYPE::ANIMATION_TYPE_A)
		touchLayer = TouchLayerPlayTypeB::create();
	else
		touchLayer = (AbsTouchLayer *)TouchLayerPlay::create();
	
	addObserver(touchLayer);
	addChild(touchLayer, UIDepth_Animation);

	AndroidHelper::hideProgress();

	startNarration();
}

void SongPlayScene::onFinishedNarration()
{
	bool showPopup = UserDefault::getInstance()->getBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_PLAY_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false);

	if (jsonInfo->isFirstPlay)
	{	
		if (!showPopup)
			showHelpPopup();
		else
		{
			guideLayer = GuideLayer::create(jsonInfo->introAnimPath, this);
			addChild(guideLayer, 10000);
		}
	}
	else
	{
		if (!showPopup)
			showHelpPopup();
		else
			startSong();
	}
}

void SongPlayScene::showHelpPopup()
{
	AudioEngine::play2d("common/sound/common_sfx_btn_01.mp3");
	HelpDialog * dlg = (HelpDialog *)KGDialog::createDialog();
	dlg->initDialog(this);
	addChild(dlg, UIDepth_Popup);
}

void SongPlayScene::startSong()
{
	touchLayer->setunfreezing();
	touchLayer->startAnimation();

	SongScene::startSong();
}

void SongPlayScene::onFInishedGuideAnimation()
{
	touchLayer->startAnimation();
	startSong();
}

void SongPlayScene::onSelectPlay()
{
	startNarration();
}

void SongPlayScene::onSelectListen()
{
	stopNarrationCallback();
	touchLayer->removeTouchEvent();

	Scene* s = SongListenScene::createScene();
	Director::getInstance()->replaceScene(s);
}

void SongPlayScene::onSkipScene()
{
	//stopNarrationCallback();
	touchLayer->removeTouchEvent();

	SongScene::onSelectQuit();
}

void SongPlayScene::onSelectQuit()
{
	stopNarrationCallback();
	touchLayer->removeTouchEvent();

	SongScene::onSelectQuit();
}

void SongPlayScene::onSelectClose()
{
	if (jsonInfo->isFirstPlay)
	{
		guideLayer = GuideLayer::create(jsonInfo->introAnimPath, this);
		addChild(guideLayer, 10000);
	}
	else
	{
		startSong();
	}
}


void SongPlayScene::onFinishedEvent()
{
	touchLayer->resetLayer();
	lyricsLayer->resetLayer();

	/*SelectDialogFinish * dlg = (SelectDialogFinish*)KGDialog::createDialog();
	dlg->initDialog(this);
	addChild(dlg, UIDepth_Popup);*/

	if (jsonInfo->isFirstPlay)
		uiLayer->showSkipButton(true);
	else
		uiLayer->showSkipButton(false);

	jsonInfo->isFirstPlay = false;
}