#include "SongIntro.h"
#include "common/GafFeatures.h"
#include "../scene/SongScene.h"
#include "../scene/EndingScene.h"
#include "../scene/SongListenScene.h"
#include "../scene/SongPlayScene.h"
#include "audio/include/AudioEngine.h"
#include "data/def.h"
#include "common/Utils.h"
#include "common/GafFeatures.h"
#include "GAF/Library/Sources/GAFTimelineAction.h"
#include "common/AndroidHelper.h"
#include "mg_common/utils/DRMManager.h"
#include "mg_common/utils/MSLPManager.h"


using namespace experimental;


Scene * SongIntro::createScene()
{
	auto scene = Scene::create();
	auto layer = SongIntro::create();
	scene->addChild(layer);
	return scene;
}

bool SongIntro::init()
{
	if (!Layer::init())
	{
		return false;
	}

	//AndroidHelper::showProgress();

	Sprite * bg = Sprite::create(STEP1_BACKGROUND_IMAGE);
	bg->setPosition(Vec2(960.0f, 600.0f));
	addChild(bg, 1);

	uiLayer = UILayer::create();
	addChild(uiLayer, 2);
	uiLayer->hideSkipButton();

	contentsLoader = ContentsLoader::create();

	
	auto scheduler = Director::getInstance()->getScheduler();
	scheduler->performFunctionInCocosThread(CC_CALLBACK_0(SongIntro::parseJson, this));

	initEvent();

	
	return true;
}

void SongIntro::onSelectPlay()
{

}

void SongIntro::onSelectListen()
{

}

void SongIntro::onSelectQuit()
{

}

void SongIntro::onSelectClose()
{
	CCLOG("SongIntro::onSelectClose");
}

void SongIntro::parseJson()
{
	jsonInfo = JsonInfo::create();
	std::string jsonPath = "";
	if (jsonInfo->currentWeek < 10)
		jsonPath = StringUtils::format("b0%d/data/songdata.json", jsonInfo->currentWeek);
	else
		jsonPath = StringUtils::format("b%d/data/songdata.json", jsonInfo->currentWeek);

	std::string data = FileUtils::getInstance()->getStringFromFile(jsonPath.c_str());
	jsonDom.Parse<0>(data.c_str());

	const rapidjson::Value& songInfo = jsonDom["defaultInfo"];

	if (songInfo.IsArray())
	{
		jsonInfo->machineEffectPath = songInfo[0]["machineEffectPath"].GetString();
		jsonInfo->songPath = songInfo[0]["songPath"].GetString();
		jsonInfo->introBGPath = songInfo[0]["introBGPath"].GetString();
		jsonInfo->mainBGPath = songInfo[0]["backgroundPath"].GetString();
		jsonInfo->introAnimPath = songInfo[0]["introPath"].GetString();
	}
	
	jsonInfo->step1ObjectPath = STEP1_TOUCH_OBJECT;
	jsonInfo->introAnimListenPath = STEP1_GUIDE_ANIM_PATH;

	const rapidjson::Value& dom = jsonDom["objectInfo"];

	const rapidjson::Value & step1Info = dom["step1touchObject"];

	jsonInfo->step1ObjectList.push_back(STEP1_TOUCH_OBJECT_PATH0);
	jsonInfo->step1ObjectList.push_back(STEP1_TOUCH_OBJECT_PATH1);
	jsonInfo->step1ObjectList.push_back(STEP1_TOUCH_OBJECT_PATH2);
	jsonInfo->step1ObjectList.push_back(STEP1_TOUCH_OBJECT_PATH3);
	jsonInfo->step1ObjectList.push_back(STEP1_TOUCH_OBJECT_PATH4);


	const rapidjson::Value & objectInfo = dom["touchObject"];

	for (int i = 0; i < objectInfo.Size(); i++)
	{
		std::string gafPath = objectInfo[i]["filePath"].GetString();

		jsonInfo->touchPathList.push_back(gafPath);

		std::string strXPosition = objectInfo[i]["postionX"].GetString();
		std::string strYPosition = objectInfo[i]["postionY"].GetString();

		std::string recX = objectInfo[i]["recX"].GetString();
		std::string recY = objectInfo[i]["recY"].GetString();
		std::string recWidth = objectInfo[i]["recWidth"].GetString();
		std::string recHeight = objectInfo[i]["recHeight"].GetString();

		CCRect * touchBox = new Rect(atof(recX.c_str()), atof(recY.c_str()), atof(recWidth.c_str()), atof(recHeight.c_str()));

		jsonInfo->touchObjectRect.push_back(touchBox);

		GafObjectInfo touchObject;
		touchObject.gafPath = gafPath;
		touchObject.positionX = atof(strXPosition.c_str());
		touchObject.positionY = atof(strYPosition.c_str());

		jsonInfo->touchObjectList.push_back(touchObject);
	}

	const rapidjson::Value & machineDom = dom["machineObject"];
	machinePath = machineDom[0]["filePath"].GetString();
	std::string machineX = machineDom[0]["postionX"].GetString();
	std::string machineY = machineDom[0]["postionY"].GetString();

	GafObjectInfo _machineInfo;
	_machineInfo.gafPath = machinePath;
	_machineInfo.positionX = atof(machineX.c_str());
	_machineInfo.positionY = atof(machineY.c_str());

	jsonInfo->machineInfo = _machineInfo;

	const rapidjson::Value & exampleDom = dom["exampleAnim"];

	for (int i = 0; i < exampleDom.Size(); i++)
	{
		ExampleInfo exampleInfo;
		std::string examplePath = exampleDom[i]["path"].GetString();
		exampleInfo.gafPath = examplePath;
		jsonInfo->wordExampleList.push_back(examplePath);
		
		std::string recX = exampleDom[i]["recX"].GetString();
		std::string recY = exampleDom[i]["recY"].GetString();
		std::string recWidth = exampleDom[i]["recWidth"].GetString();
		std::string recHeight = exampleDom[i]["recHeight"].GetString();

		CCRect * touchBox = new Rect(atof(recX.c_str()), atof(recY.c_str()), atof(recWidth.c_str()), atof(recHeight.c_str()));
		exampleInfo.touchBox = touchBox;

		jsonInfo->exampleInfoList.push_back(exampleInfo);
	}
	
	const rapidjson::Value& exampleRectDom = dom["exampleRect"];

	for (int i = 0; i < exampleRectDom.Size(); i++)
	{
		std::string recX = exampleRectDom[i]["recX"].GetString();
		std::string recY = exampleRectDom[i]["recY"].GetString();
		std::string recWidth = exampleRectDom[i]["recWidth"].GetString();
		std::string recHeight = exampleRectDom[i]["recHeight"].GetString();

		ExampleInfo exampleInfo = jsonInfo->exampleInfoList.at(i);
		
		CCRect * touchBox = new Rect(atof(recX.c_str()), atof(recY.c_str()), atof(recWidth.c_str()), atof(recHeight.c_str()));
		exampleInfo.touchBox = touchBox;
	}

	const rapidjson::Value& trackInfo = jsonDom["timeInfo"];

	if (trackInfo.IsArray())
	{
		for (int i = 0; i < trackInfo.Size(); i++)
		{
			std::string sec_str = trackInfo[i]["second"].GetString();
			jsonInfo->trackTimeList.push_back(atof(sec_str.c_str()));

			jsonInfo->trackLyricsList.push_back(trackInfo[i]["lyricsImage"].GetString());
			jsonInfo->trackExampleList.push_back(trackInfo[i]["exampleImage"].GetString());
			jsonInfo->trackTouchList.push_back(trackInfo[i]["machineAction"].GetString());
		}

		maxLoadCount = 0;

		for (int i = 0; i < jsonInfo->trackLyricsList.size(); i++)
		{
			std::string imgPath = jsonInfo->trackLyricsList.at(i);
			
			Director::getInstance()->getTextureCache()->addImageAsync(imgPath.c_str(), CC_CALLBACK_1(SongIntro::onLyricsImageLoad, this));
		}
	}
}

void SongIntro::onLyricsImageLoad(Texture2D * texture)
{
	maxLoadCount++;

	if (maxLoadCount >= jsonInfo->trackLyricsList.size())
	{
		maxLoadCount = 0;
		for (int i = 0; i < jsonInfo->trackExampleList.size(); i++)
		{
			std::string imgPath = jsonInfo->trackExampleList.at(i);
			std::string * gafPath = Utils::tringSplit(imgPath, ".");
			gafPath->append(".png");

			Director::getInstance()->getTextureCache()->addImageAsync(gafPath->c_str(), CC_CALLBACK_1(SongIntro::onSpriteSheetLoad, this));
		}
	}
}

void SongIntro::onSkipScene()
{

}
void SongIntro::onSpriteSheetLoad(Texture2D * texture)
{
	maxLoadCount++;
	if (maxLoadCount >= jsonInfo->trackExampleList.size())
	{
		maxLoadCount = 0;
		for (int i = 0; i < jsonInfo->touchPathList.size(); i++)
		{
			std::string imgPath = jsonInfo->touchPathList.at(i);
			std::string * gafPath = Utils::tringSplit(imgPath, ".");
			gafPath->append(".png");

			Director::getInstance()->getTextureCache()->addImageAsync(gafPath->c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));
		}

		for (int i = 0; i < jsonInfo->wordExampleList.size(); i++)
		{
			std::string imgPath = jsonInfo->wordExampleList.at(i);
			std::string * gafPath = Utils::tringSplit(imgPath, ".");
			gafPath->append(".png");

			Director::getInstance()->getTextureCache()->addImageAsync(gafPath->c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));
		}

		std::string bubbleGafPath = jsonInfo->step1ObjectPath;
		std::string * gafPath = Utils::tringSplit(bubbleGafPath, ".");
		gafPath->append(".png");
		Director::getInstance()->getTextureCache()->addImageAsync(gafPath->c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));

		for (int i = 0; i < jsonInfo->step1ObjectList.size(); i++)
		{
			std::string imgPath = jsonInfo->step1ObjectList.at(i);
			//std::string * gafPath = Utils::tringSplit(imgPath, ".");
			//gafPath->append(".png");

			Director::getInstance()->getTextureCache()->addImageAsync(imgPath.c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));
		}

		//Director::getInstance()->getTextureCache()->addImageAsync(gafPath->c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));
		Director::getInstance()->getTextureCache()->addImageAsync(machinePath.c_str(), CC_CALLBACK_1(SongIntro::onTouchLoad, this));
		
	}	
}

void SongIntro::onTouchLoad(Texture2D * texture)
{
	maxLoadCount++;
	int maxCount = jsonInfo->touchPathList.size() + jsonInfo->wordExampleList.size() + jsonInfo->step1ObjectList.size() + 2;
	if (maxLoadCount >= maxCount)
	{
		maxLoadCount = 0;
		//removeChild(loadingDlg);
		//contentsLoader->hideContentsLoader();
		//AndroidHelper::hideProgress();
		//this->runAction(Sequence::create(DelayTime::create(10.0f),CallFuncN::create(CC_CALLBACK_1(SongIntro::delayScene, this)), nullptr));
		
		DRMManager::getInstance()->hideProgress();
		CCLOG("END PRELOAD");

		narrationID = AudioEngine::play2d("common/sound/com_c08_title.mp3");
		AudioEngine::setFinishCallback(narrationID, CC_CALLBACK_2(SongIntro::onFinishCallBack, this));

		
		//memTest();
	}
}
void SongIntro::delayScene(Node* pSender)
{
	DRMManager::getInstance()->hideProgress();
	CCLOG("END PRELOAD");

	narrationID = AudioEngine::play2d("common/sound/com_c08_title.mp3");
	AudioEngine::setFinishCallback(narrationID, CC_CALLBACK_2(SongIntro::onFinishCallBack, this));
}

void SongIntro::onFinishCallBack(int audioID, std::string filePath)
{
	CCLOG("jsonInfo->isFirstPlay = %d", jsonInfo->isFirstPlay);
	//if (jsonInfo->isFirstPlay)
		this->schedule(schedule_selector(SongIntro::moveNextScene), 1.0f);
}

void SongIntro::moveNextScene(float delta)
{
	moveNextScene();
}

void SongIntro::moveNextScene()
{
	_eventDispatcher->removeEventListener(listener);
	AndroidHelper::getDebugMode();
	AndroidHelper::getStartStep();

	Scene* s;
	if (jsonInfo->getStartStep() == 1)
		s = SongListenScene::createScene();
	else
		s = SongPlayScene::createScene();

	Director::getInstance()->replaceScene(s);

}



void SongIntro::memTest()
{
	CCLOG("CALL memTest");
	auto asset = GAFAsset::create("b02/gaf/b02_cottoncandy_center_anim_test/b02_cottoncandy_center_anim_test.gaf", nullptr);
	/*for (int i = 0; i < 3; i++)
	{
		auto touchObj = asset->createObjectAndRun(false);
		touchObj->gotoAndStop(1);
		touchObj->setPosition(Vec2((400), 600));
		std::string objName = StringUtils::format("obj%d", i);
		touchObj->setName(objName);
		if (touchObj)
			touchObj->retain();

		addChild(touchObj,1000 + i);

		jsonInfo->touchObjList.push_back(touchObj);
	}*/

	touchObj = asset->createObjectAndRun(false);
	touchObj->gotoAndStop(1);
	//touchObj->setPosition(Vec2((400), 600));
	touchObj->setPosition(Vec2(0, 1200));
	std::string objName = StringUtils::format("obj%d", 0);
	touchObj->playSequence("Basic",true);
	touchObj->setName(objName);
	if (touchObj)
		touchObj->retain();

	addChild(touchObj, 1000);

	container = touchObj->getObjectByName("ImageContainer");
	imageHolder = touchObj->getObjectByName("imageHolder");
	auto exampleAsset = GAFAsset::create("b02/gaf/b02_c08_babybottle/b02_c08_babybottle.gaf", nullptr);
	example = exampleAsset->createObjectAndRun(true);
	example->playSequence("Basic", false);
	example->setPosition(Vec2(100, 400));
	container->addChild(example);
	//jsonInfo->touchObjList.push_back(touchObj);
}

void SongIntro::initEvent()
{
	listener = EventListenerTouchOneByOne::create();
	//listener->setSwallowTouches(true);

	listener->onTouchBegan = [&](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		cocos2d::Rect rect = this->getBoundingBox();

		if (rect.containsPoint(p))
		{
			return true; // to indicate that we have consumed it.
		}

		return true; // we did not consume this event, pass thru.
	};

	listener->onTouchEnded = [=](cocos2d::Touch* touch, cocos2d::Event* event)
	{
		Vec2 p = touch->getLocation();
		touchEvent(touch, p);
	};

	_eventDispatcher = cocos2d::Director::getInstance()->getEventDispatcher();
	_eventDispatcher->addEventListenerWithFixedPriority(listener, 30);
	
}

void SongIntro::onHitEvent(void* params)
{
	auto customEvent = reinterpret_cast<EventCustom*>(params);
	auto gafEvent = reinterpret_cast<gaf::GAFTimelineAction*>(customEvent->getUserData());
	std::string eventName = gafEvent->getParam(GAFTimelineAction::PI_EVENT_TYPE);	
}

bool SongIntro::isAnimClick(GAFObject * gaf, cocos2d::Touch* touch)
{
	Vec2 localPoint = gaf->convertTouchToNodeSpace(touch);
	localPoint = cocos2d::PointApplyTransform(localPoint, gaf->getNodeToParentTransform());
	Rect r = gaf->getBoundingBoxForCurrentFrame();

	if (r.containsPoint(localPoint))
		return true;
	else
		return false;
}

void SongIntro::touchEvent(cocos2d::Touch* touch, Vec2 _p)
{
	if (jsonInfo->isFirstPlay)
		CCLOG("isFirstPlay true");
	else
		CCLOG("isFirstPlay false");

	if (jsonInfo->isFirstPlay)	return;
	
	if (narrationID != -1)		AudioEngine::stop(narrationID);

	moveNextScene();	
}

void SongIntro::onFinishedTouchSequence(gaf::GAFObject* object, cocos2d::Touch* touch)
{
	finishedCount++;
	touchObj->setAnimationFinishedPlayDelegate(nullptr);
	CCLOG("finishedCount =%d", finishedCount);
	touchObj->playSequence("Exit", false);
	example->playSequence("Touch", false);

	auto fade = FadeOut::create(0.5f);
	//imageHolder->runAction(fade);
	touchObj->runAction(fade);

	/*if (finishedCount >= 3)
	{
		finishedCount = 0;
		for (int i = 0; i < 3; i++)
		{
			std::string objName = StringUtils::format("obj%d", i);
			removeChildByName(objName);
		}
		
		jsonInfo->touchObjList.clear();

		std::string gafPath = "";
		if (!isChanged)
		{
			isChanged = true;
			gafPath = "b02/gaf/b02_c08_pilow/b02_c08_pilow.gaf";
		}
		else
		{
			isChanged = false;
			gafPath = "b02/gaf/b02_c08_kiss/b02_c08_kiss.gaf";
		}
		auto asset = GAFAsset::create(gafPath.c_str(), nullptr);
		for (int i = 0; i < 3; i++)
		{
			auto touchObj = asset->createObjectAndRun(false);
			touchObj->gotoAndStop(1);
			touchObj->setPosition(Vec2((i * 400), 600));
			std::string objName = StringUtils::format("obj%d", i);
			touchObj->setName(objName);
			if (touchObj)
				touchObj->retain();

			addChild(touchObj, 1000 + i);
			jsonInfo->touchObjList.push_back(touchObj);
		}

	}*/
}

void SongIntro::onFinishTouchSequence(gaf::GAFObject* object)
{
	//_eventDispatcher->removeEventListener(listener);

	////Scene* s = SongMain::createScene();
	//Scene* s = EndingScene::createScene();
	//Utils::moveToScene(s);
}
