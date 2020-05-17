#include "SongScene.h"
#include "audio/include/AudioEngine.h"
#include "scene/EndingScene.h"
#include "layer/UILayer.h"
#include "common/Utils.h"

using namespace experimental;

SongScene::SongScene() {}
SongScene::~SongScene() {}

Scene * SongScene::createScene()
{
	auto scene = Scene::create();
	auto layer = SongScene::create();
	scene->addChild(layer);
	return scene;
}

bool SongScene::init()
{
	if (!Layer::init())
	{
		return false;
	}

	initData();
	
	initGUI();
	
	return true;
}

void SongScene::initData()
{
	jsonInfo = JsonInfo::create();
	isFirstPlay = true;
}

void SongScene::initGUI()
{
	bgSprite = CCSprite::create();
	bgSprite->setPosition(Vec2(960.0f, 600.0f));
	addChild(bgSprite, UIDepth_Background);
	
	trackSecondManager = SongTickManager::create(jsonInfo->trackTimeList, this);
	addChild(trackSecondManager, UIDepth_TickManager);

	lyricsLayer = SongLyricsLayer::create(jsonInfo->trackLyricsList);
	addChild(lyricsLayer, UIDepth_Lyrics);

	uiLayer = UILayer::create();
	addChild(uiLayer, UIDepth_UI);
	uiLayer->setSkipButtonEventListener(this);
	addObserver(lyricsLayer);
}

void SongScene::onSkipScene()
{
	CCLOG("SongScene::onSkipScene");
}
void SongScene::addObserver(IBaiscSongPlay * object)
{
	songPlayObserable.push_back(object);
}

void SongScene::setCurrentScene(SCENE_TYPE::SceneType scene)
{
	currentScene = scene;
}

void SongScene::onTickEvent(int index)
{
	for (int i = 0; i < songPlayObserable.size(); i++)
	{
		IBaiscSongPlay * observer = (IBaiscSongPlay *)songPlayObserable.at(i);
		observer->onPlaybyTickEvent(index);
	}
}

void SongScene::startNarration()
{
	Utils::timeLog("SongScene::startNarration");
	std::string narrationPath;

	if (currentScene == SCENE_LISTEN)
		narrationPath = "common/sound/com_c08_guide_01.mp3";
	else
		narrationPath = "common/sound/com_c08_guide_03.mp3";

	narrationID = AudioEngine::play2d(narrationPath.c_str());
	AudioEngine::setFinishCallback(narrationID, [&](int id, const std::string& filePath){
		onFinishedNarration();
	});
}

void SongScene::onFinishedNarration()
{
	CCLOG("SongScene::onFinishedNarration");
}

void SongScene::onFInishedGuideAnimation()
{

}
void SongScene::onFinishedEvent()
{

}

void SongScene::startSong()
{
	trackSecondManager->start(jsonInfo->songPath);
}

void SongScene::onSelectPlay()
{

}

void SongScene::onSelectListen()
{

}

void SongScene::onSelectQuit()
{
	//stopNarrationCallback();
	Scene* s = EndingScene::createScene();
	Director::getInstance()->replaceScene(s);
}

void SongScene::onSelectClose()
{

}

void SongScene::stopNarrationCallback()
{
	AudioEngine::setFinishCallback(narrationID, nullptr);
}