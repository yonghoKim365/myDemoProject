#include "SongListenScene.h"
#include "scene/SongPlayScene.h"
#include "common/AndroidHelper.h"
#include "mg_common/utils/MSLPManager.h"

Scene * SongListenScene::createScene()
{
	auto scene = Scene::create();
	auto layer = SongListenScene::create();
	scene->addChild(layer);
	return scene;
}

bool SongListenScene::init()
{
	if (!Layer::init())
	{
		return false;
	}

	initData();

	initGUI();

	return true;
}

void SongListenScene::initData()
{
	SongScene::initData();

	setCurrentScene(SceneType::SCENE_LISTEN);
	
}

void SongListenScene::initGUI()
{
	SongScene::initGUI();

	MSLPManager::getInstance()->progress(1);

	bgSprite->setTexture(jsonInfo->introBGPath.c_str());

	auto director = Director::getInstance();
	Size pixelSize = director->getWinSizeInPixels();
	
	if (pixelSize.width == 1920.0f)
	{
		float scale = pixelSize.width / bgSprite->getContentSize().width;
		bgSprite->setScale(scale);
	}

	touchLayer = TouchLayerListen::create();
	addChild(touchLayer, UIDepth_Animation);

	if (!jsonInfo->isFirstPlay)
		uiLayer->showSkipButton(false);

	//startSong();
	startNarration();
}

void SongListenScene::onFinishedNarration()
{
	if (jsonInfo->isFirstPlay)
	{
		if (!UserDefault::getInstance()->getBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false))
		{
			showHelpPopup();
		}
		else
		{
			guideLayer = GuideLayer::create(jsonInfo->introAnimListenPath, this);
			addChild(guideLayer, 10000);
		}
	}
	else
	{
		if (!UserDefault::getInstance()->getBoolForKey(StringUtils::format("%s_%02d", USER_DATA_SHOW_LISTEN_GUIDE, MSLPManager::getInstance()->getBookNum()).c_str(), false))
		{
			showHelpPopup();
		}
		else
		{
			startSong();
		}
	}

	
}

void SongListenScene::showHelpPopup()
{
	AudioEngine::play2d("common/sound/common_sfx_btn_01.mp3");
	ListenStartDialog * dlg = (ListenStartDialog *)KGDialog::createDialog();
	dlg->initDialog(this);
	addChild(dlg, UIDepth_Popup);
}

void SongListenScene::startSong()
{
	SongScene::startSong();
	touchLayer->setTouchEnable();
}

void SongListenScene::onFinishedEvent()
{
	touchLayer->resetLayer();
	lyricsLayer->resetLayer();

	/*ListenFinishedDialog * dlg = (ListenFinishedDialog*)KGDialog::createDialog();
	dlg->initDialog(this);
	addChild(dlg, UIDepth_Popup);*/

	if (jsonInfo->isFirstPlay)
		uiLayer->showSkipButton(true);
	
}

void SongListenScene::onSkipScene()
{
	stopNarrationCallback();
	touchLayer->removeTouchEvent();
	trackSecondManager->stop();

	/*Scene* s = SongPlayScene::createScene();
	Director::getInstance()->replaceScene(s);*/
	goToNextScene();
}

void SongListenScene::onSelectPlay()
{
	stopNarrationCallback();
	touchLayer->removeTouchEvent();
	
	/*Scene* s = SongPlayScene::createScene();
	Director::getInstance()->replaceScene(s);*/
	goToNextScene();
}

void SongListenScene::onSelectListen()
{
	initData();
	
	startSong();
}

void SongListenScene::onSelectQuit()
{
	stopNarrationCallback();
	touchLayer->removeTouchEvent();

	SongScene::onSelectQuit();
}

void SongListenScene::onSelectClose()
{
	if (jsonInfo->isFirstPlay)
	{
		guideLayer = GuideLayer::create(jsonInfo->introAnimListenPath, this);
		addChild(guideLayer, 10000);
	}
	else
	{
		startSong();
	}
}

void SongListenScene::onFInishedGuideAnimation()
{
	startSong();
}

void SongListenScene::goToNextScene()
{
	AndroidHelper::showProgress();
	
	Scene* s = SongPlayScene::createScene();
	Director::getInstance()->replaceScene(s);
}