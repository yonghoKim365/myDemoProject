#include "EndScene.h"
#include "common/Actor.h"
#include "GAF.h"

#include "KDataProvider.h"
#include "KGlobalManager.h"


GAFObject* _endOfPage;

EndScene::EndScene()
{
	CCLOG("EndScene::EndScene()");
	replayBtn = new Actor();
	nextBtn = new Actor();
	exitBtn = new Actor();
}

EndScene::~EndScene()
{
	CCLOG("EndScene::~EndScene()");
}

void EndScene::onEnter()
{
	Layer::onEnter();

	_listener = EventListenerTouchOneByOne::create();
	_listener->setSwallowTouches(true);
	_listener->onTouchBegan = CC_CALLBACK_2(EndScene::onTouchBegan, this);
	_listener->onTouchMoved = CC_CALLBACK_2(EndScene::onTouchMoved, this);
	_listener->onTouchEnded = CC_CALLBACK_2(EndScene::onTouchEnded, this);
	_listener->onTouchCancelled = CC_CALLBACK_2(EndScene::onTouchCancelled, this);
	_eventDispatcher->addEventListenerWithSceneGraphPriority(_listener, this);
}

void EndScene::onExit()
{
	Layer::onExit();
	_eventDispatcher->removeEventListener(_listener);
}


Scene* EndScene::createScene()
{
	auto scene = Scene::create();

	auto layer = EndScene::create();

	scene->addChild(layer);

	return scene;
}


EndScene* EndScene::create()
{
	EndScene *pRet = new EndScene();
	if (pRet && pRet->init())
	{
		pRet->autorelease();
		return pRet;
	}
	else
	{
		delete pRet;
		pRet = NULL;
		return pRet;
	}
}

bool EndScene::init()
{
	CCLOG("EndScene::init()");
	if (!Layer::init())
		return false;

	// do anything
	winSize = Director::getInstance()->getVisibleSize();
	_sndPath = "common/snd/";
	_sndCount = 0;
	_selectedActor = NULL;
	_endOfPage = NULL;

	_idx = random(0, 3);

	setBackGround();

	if (CocosDenshion::SimpleAudioEngine::getInstance()->isBackgroundMusicPlaying()){
		CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();
	}
	
	// end of pgae는 시간 지나면 화면 꺼짐.
	KDataProvider::getInstance()->releaseCpuLock();

	return true;
}


void EndScene::setBackGround()
{

	auto bgLayer = Layer::create();
	bgLayer->setAnchorPoint(Vec2::ZERO);
	bgLayer->setPosition(Vec2::ZERO);
	addChild(bgLayer, 0);

	Size visibleSize = Director::getInstance()->getVisibleSize();
	float width = visibleSize.width;
	float height = visibleSize.height;
	/*
	float opacity = content.opacity;
	string image = content.image;
	string color = content.color;


	auto* sprite = Sprite::create();

		auto size = sprite->getContentSize();
		sprite->setScaleX(width / size.width);
		sprite->setScaleY(height / size.height);
		*/
	_endOfPage = createObjectAndRun("common/flash/common_EndOfPage.gaf", false, Vec2(0.0f, winSize.height));
	auto size = _endOfPage->getContentSize();
	_endOfPage->setScaleX(width / size.width);
	_endOfPage->setScaleY(height / 1200);// size.height);
	_endOfPage->setPosition(Vec2(0.0f, height));


	_endOfPage->setLooped(false);
	_endOfPage->getObjectByName("main_mc")->setLooped(false);
	_endOfPage->getObjectByName("main_mc")->getObjectByName("title_mc")->getObjectByName("text_mc")->setLooped(false);
	_endOfPage->getObjectByName("main_mc")->getObjectByName("title_mc")->getObjectByName("text_mc")->stop();
	_endOfPage->getObjectByName("main_mc")->getObjectByName("title_mc")->getObjectByName("text_mc")->setFrame(_idx);
	_endOfPage->start();
	playSound(_sndPath + "common_sfx_14.mp3");
	_endOfPage->getObjectByName("main_mc")->getObjectByName("title_mc")->getObjectByName("star_mc")->getObjectByName("twinkle_mc")->setLooped(true);

	replayBtn->init(_endOfPage->getObjectByName("main_mc")->getObjectByName("replay_mc"));
	nextBtn->init(_endOfPage->getObjectByName("main_mc")->getObjectByName("next_mc"));
	exitBtn->init(_endOfPage->getObjectByName("main_mc")->getObjectByName("exit_mc"));

	unselected(replayBtn);
	unselected(nextBtn);
	unselected(exitBtn);

	if (KDataProvider::getInstance()->hasNext() == false){
		nextBtn->setVisible(false);
	}

	bgLayer->addChild(_endOfPage);

	auto act = Sequence::createWithTwoActions(DelayTime::create(1.9f), CallFunc::create(CC_CALLBACK_0(EndScene::sndCallback, this)));
	runAction(act);
}

#define SND_ENDING01_PATH	"common/c11/snd/YSF_ending01.mp3"
#define SND_ENDING02_PATH	"common/c11/snd/YSF_ending02.mp3"
#define SND_ENDING03_PATH	"common/c11/snd/YSF_ending03.mp3"
#define SND_ENDING04_PATH	"common/c11/snd/YSF_ending04.mp3"


void EndScene::sndCallback()
{
	CCLOG("EndScene::sndCallback");
	if (_sndCount == 0)
	{
		switch (_idx)
		{
		case 0:
			playSound("common/snd/YSF_ending01.mp3");
			break;
		case 1:
			playSound("common/snd/YSF_ending02.mp3");
			break;
		case 2:
			playSound("common/snd/YSF_ending03.mp3");
			break;
		case 3:
			playSound("common/snd/YSF_ending04.mp3");
			break;
		}
		auto act = Sequence::createWithTwoActions(DelayTime::create(3.3f), CallFunc::create(CC_CALLBACK_0(EndScene::sndCallback, this)));
		runAction(act);
		_sndCount++;
		_touchEnabled = true;
	}
	else if (_sndCount == 1)
	{
		auto audioId = playSound(_sndPath + "common_sfx_15.mp3");
		experimental::AudioEngine::setFinishCallback(audioId, CC_CALLBACK_2(EndScene::animFinishedCallback, this));
	}
}

void EndScene::animFinishedCallback(int audioID, const std::string & filePath)
{
	CCLOG("EndScene Animation Finished");
	//_touchEnabled = true;

	//_endOfPage->getObjectByName("main_mc")->getObjectByName("title_mc")->getObjectByName("star_mc")->setLooped(true);
}

Actor* EndScene::getItemForTouch(Touch *touch)
{
	Vec2 touchLocation = touch->getLocation();

	if (replayBtn->checkContainsPoint(touchLocation))
	{
		return replayBtn;
	}
	else if (nextBtn->checkContainsPoint(touchLocation))
	{
		return nextBtn;
	}
	else if (exitBtn->checkContainsPoint(touchLocation))
	{
		return exitBtn;
	}
	else
	{
		return NULL;
	}
}

void EndScene::selected(Actor* target)
{
	CCLOG("EndScene::selected");
	target->getGAFObject()->setFrame(1);
}

void EndScene::unselected(Actor* target)
{
	CCLOG("EndScene::unselected");
	target->getGAFObject()->setFrame(0);
}

void EndScene::activate(Actor* target)
{
	if (target == replayBtn)
	{
		cocos2d::experimental::AudioEngine::stopAll();
		playSound(_sndPath + "common_sfx_btn_01.mp3");

		KGlobalManager::getInstance()->setAutoPlay(false); /*읽기모드 사용*/
		KDataProvider::getInstance()->setListenMode(1);                /*모드 변경을 알린다. JNI*/
		/*읽기모드는 슬립모드 해제*/
		KDataProvider::getInstance()->releaseCpuLock();

		//기획 변경 되어 유저 선택 없어짐
		//showPopup("selectedUser");

		int i = 0;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;

	}
	else if (target == nextBtn)
	{
		cocos2d::experimental::AudioEngine::stopAll();
		playSound(_sndPath + "common_sfx_btn_01.mp3");

		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();
	// COMMAND_NEXTBOOK)    /*다음 컨텐트로 이동*/
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32)  /*먼저 완독정보 전달*/
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif		
		KDataProvider::getInstance()->nextBook();  /*다음 컨텐트 이동 요청 JNI*/
		Director::getInstance()->end();
	}
	else if (target == exitBtn)
	{
		cocos2d::experimental::AudioEngine::stopAll();
		playSound(_sndPath + "common_sfx_btn_01.mp3");
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();  /*완독되었는지 확인*/
		/*완독여부 전달 JNI*/
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif	
		Director::getInstance()->end();
	}
}

bool EndScene::onTouchBegan(Touch* pTouch, Event* pEvent)
{
	if (!_touchEnabled)
		return true;

	CCLOG("onTouchBegan");

	Point pPoint = pTouch->getLocation();

	if (_selectedActor = getItemForTouch(pTouch))
		selected(_selectedActor);

	return true;
}

void EndScene::onTouchMoved(Touch* pTouch, Event* pEvent)
{
	if (!_touchEnabled)
		return;

	Point pPoint = pTouch->getLocation();

	auto currentActor = getItemForTouch(pTouch);

	if (currentActor != _selectedActor)
	{
		if (_selectedActor)
		{
			unselected(_selectedActor);
		}
		_selectedActor = currentActor;
		if (_selectedActor)
		{
			selected(_selectedActor);
		}
	}
}

void EndScene::onTouchEnded(Touch* pTouch, Event* pEvent)
{
	if (!_touchEnabled)
		return;

	Point pPoint = pTouch->getLocation();
	CCLOG("onTouchEnded(x:%1f, y:%1f)", pPoint.x, pPoint.y);
	if (_selectedActor && _touchEnabled)
	{
		_touchEnabled = false;
		unselected(_selectedActor);
		activate(_selectedActor);
	}
}

void EndScene::onTouchCancelled(Touch* pTouch, Event* pEvent)
{
	if (_selectedActor)
	{
		unselected(_selectedActor);
	}
}


GAFObject* EndScene::createObjectAndRun(std::string path, bool isRun, const Vec2 &pnt)
{
	GAFObject*	gafObj;

	CCLOG("AssetUtils_d05_01::createObjectAndRun(0)  not nullptr.");

	auto asset = GAFAsset::create(path, nullptr);
	if (asset != nullptr)
	{
		gafObj = asset->createObjectAndRun(isRun);
		gafObj->setPosition(pnt);
		return gafObj;
	}
	
	return	nullptr;
}

int EndScene::playSound(std::string sFile)
{
	int nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str());
	/*
	float fSoundVol = 0.0f;
	bool bSound = KDataProvider::getInstance()->getSoundState();
	int nAudio;
	if (bSound)  // 정상모드
	{
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str());
	}
	else {       // 묵음모드 
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), false, fSoundVol);
	}
	*/


	return	nAudio;
}
