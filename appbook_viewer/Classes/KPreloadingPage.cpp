#include "KPreloadingPage.h"
#include "KDataProvider.h"

#include "KCameraManager.h"
#include "KJSONDataManager.h"
#include "KRecordingManager.h"


Scene* KPreloadingPage::createScene( AppOperator * pOperator)
{
	auto scene = Scene::create();
	auto layer = KPreloadingPage::create();
	layer->setOperator(pOperator);
	scene->addChild(layer);
	return scene;
}

void KPreloadingPage::setOperator(AppOperator * pOperator){
	mpOperator = pOperator;
}

// on "init" you need to initialize your instance
bool KPreloadingPage::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	LoadingScreen(0);
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	Sprite * backSprite = Sprite::create();
	backSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	backSprite->setAnchorPoint(Vec2(0, 0));
	backSprite->setOpacity(127);
	backSprite->setColor(Color3B(0, 0, 0));
	backSprite->setPosition(0,0);

	this->addChild(backSprite);

	for (int i = 1; i < 11; i++)
	{
		string frame_sprite_name = StringUtils::format("syscommon/preloader_contents_loading_%02d.png", i);
		Sprite * pAnimSprite = Sprite::create(frame_sprite_name);
		manimFrames.pushBack(pAnimSprite);
		pAnimSprite->setAnchorPoint(Vec2(0.5, 0.5));
		pAnimSprite->setPosition(Vec2(visibleSize.width / 2.0f, visibleSize.height / 2.0f));
		pAnimSprite->setVisible(false);
		this->addChild(pAnimSprite);
	}
#endif	

	m_nCurrentCount = 0;
	this->scheduleUpdate();
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	auto scheduler = Director::getInstance()->getScheduler();
	scheduler->performFunctionInCocosThread(CC_CALLBACK_0(KPreloadingPage::threadStart, this, 0.1f));
#else
	this->scheduleOnce(schedule_selector(KPreloadingPage::threadStart), 0.1f);
#endif	
	KCameraManager::getInstance();
	KCameraManager::getInstance()->determineFullpathnameForPicture(KXMLReader::getInstance()->getBookInfo()->projectcode);
	KJSONDataManager::sTargetName = StringUtils::format("%s/%s",
										KCameraManager::getInstance()->getAndroidTargetPath().c_str()
										, "localdata.json");
	KRecordingManager::getInstance();
	return true;
}

/*window 프리로딩 화면구성*/
void KPreloadingPage::LoadingScreen(float dt)
{
	// Create the sprite animation
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Sprite* spritebk = Sprite::create("syscommon/preloader_contents_bg.png");
	if (spritebk == nullptr) {
		return ;
	}
	spritebk->setOpacity(255);
	CViewUtils::setSize(spritebk, 0, 0, visibleSize.width, visibleSize.height);
	this->addChild(spritebk);


	
}
void KPreloadingPage::update(float delta)
{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	ShowLoading(m_nCurrentCount++);
#endif
}

void KPreloadingPage::ShowLoading(int npos)
{
	int aniPos = npos % 10;
	Sprite* sprite;
	
	log("loading file %d", aniPos);
	try{
		if (manimFrames.size() > 10) return;
		sprite = manimFrames.at(aniPos);
		if (sprite == NULL) return;
		if (sprite->getTexture() == NULL) return;
		sprite->setVisible(true);
		if (aniPos != 0)
		{
			aniPos = aniPos - 1;
			sprite = manimFrames.at(aniPos);
			sprite->setVisible(false);
		}
		else{
			sprite = manimFrames.at(9);
			sprite->setVisible(false);
		}
	}
	catch(exception &e){

	}

	
	
}


void KPreloadingPage::threadStart(float dt){

	KXMLReader * mpReader = KXMLReader::getInstance();

	bool bReturn = mpReader->parseFileOtherThanBookInfo();
	if (bReturn == false) {
		log("can't read the file. :: 화일을 읽을수 없습니다.");
		mpReader = nullptr;
		return;
	}
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	KDataProvider::getInstance()->acquireCpuWakeLock();
#endif
//	mLoadingBar->setPercent(0);
	mpOperator->requestInitProcess();
	
	//preload할 화일 구성하기.
	set<string> setImageFilename;
	makeImageStringFromPageInfo(setImageFilename);
	makeImageStringFromPopupInfo(setImageFilename);

	m_nTotalCount = setImageFilename.size();

	set<string> setSoundFilename;
	makeSoundStringFromPageInfo(setSoundFilename);
	makeSoundStringFromPopupInfo(setSoundFilename);

	//테스트를 위해서 넣어줌.
	//setImageFilename.erase(setImageFilename.find("common/end.png"));

	log("threadStart, mDebugMode :%s", KDataProvider::getInstance()->mDebugMode.c_str());

	set<string>::iterator iter_set;
	int nLocalCount = 0;

	if (KDataProvider::getInstance()->mDebugMode.compare("true") == 0){ // == "true"){

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 

		log(" KPreloadingPage::threadStart 0, imageload start  ");

		for (iter_set = setImageFilename.begin(); iter_set != setImageFilename.end(); iter_set++) {
			KDataProvider::getInstance()->saveDataToCache(*iter_set);
		}

		log(" KPreloadingPage::threadStart 5, imageload end  ");

		for (iter_set = setSoundFilename.begin(); iter_set != setSoundFilename.end(); iter_set++) {
			KDataProvider::getInstance()->saveDataToFile(*iter_set);
		}
#endif

		log(" KPreloadingPage::threadStart 10 ");

	}
	else{

		log(" KPreloadingPage::threadStart debug is false");

		///////////////////////////////////////////////////////////////
		//			Load Image 
		///////////////////////////////////////////////////////////////

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 

		std::vector<string> entryList;

		KDataProvider::getInstance()->addEntryName("Images/");
		//KDataProvider::getInstance()->addEntryName("Sounds/");


		for (iter_set = setImageFilename.begin(); iter_set != setImageFilename.end(); iter_set++) {
			entryList.push_back(*iter_set);
		}

		std::string syscommon = "syscommon/";

		for (std::string var : entryList)
		{
			std::string sCompared = var.substr(0, syscommon.size());
			if (syscommon == sCompared) {
				continue;
			}
			//log(" var %s", var.c_str());
			KDataProvider::getInstance()->addEntryName(var);
		}

		KDataProvider::getInstance()->preload();

		////////////////////////////////////////////////////////////////////
		//			Load Sound
		////////////////////////////////////////////////////////////////////
		
		KDataProvider::getInstance()->openDrmZipFile();

		for (iter_set = setSoundFilename.begin(); iter_set != setSoundFilename.end(); iter_set++) {
			KDataProvider::getInstance()->saveDataToFile(*iter_set);
		}

		KDataProvider::getInstance()->closeDrmZipFile();
#endif

		int nPageCount = mpReader->getPageCount();
		log("threadStart = Total Page %d", nPageCount);
		
	}
	

	log(" KPreloadingPage::threadStart 20 ");

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
	KDataProvider::getInstance()->setProgress(100);
#endif
	KDataProvider::getInstance()->releaseCpuLock();

	log(" KPreloadingPage::threadStart 30 ");
		
	m_nCurrentCount = -1;
	mpOperator->setPage(0, false);

	log(" KPreloadingPage::threadStart 40 ");
	
	
}


/*페이지에 이미지를 가져온다.*/
void KPreloadingPage::makeImageStringFromPageInfo(set<string>& setImageFilename) {
	KXMLReader * mpReader = KXMLReader::getInstance();
	for (int i = 0; i < mpReader->getPageCount(); i++) {
		HPAGE_INFO * pInfo = mpReader->getPageReference(i);
		if (pInfo->thumbnailImage.empty() == false)
			setImageFilename.insert(pInfo->thumbnailImage);
		if (pInfo->stBackground.image.empty() == false)
			setImageFilename.insert(pInfo->stBackground.image);

		for (int s = 0; s < pInfo->vtContents.size(); s++) {
			STCONTENT_INFO * pContentInfo = pInfo->vtContents.at(s);
			if (pContentInfo->image.empty() == false)
				setImageFilename.insert(pContentInfo->image);
			if (pContentInfo->disabled.empty() == false)
				setImageFilename.insert(pContentInfo->disabled);
			if (pContentInfo->selected.empty() == false)
				setImageFilename.insert(pContentInfo->selected);
			for (int t = 0; t < pContentInfo->vtAnimation.size(); t++) {
				STANIMATION * pAni = pContentInfo->vtAnimation.at(t);
				if (pAni->image.empty() == false)
					setImageFilename.insert(pAni->image);
			}
		}
	}
}


/*팝업내에 이미지를 가져온다.*/
void KPreloadingPage::makeImageStringFromPopupInfo(set<string>& setImageFilename) {
	KXMLReader * mpReader = KXMLReader::getInstance();
	for (int i = 0; i < mpReader->getPopupCount(); i++) {
		HPOPUP_INFO  * pPopup = mpReader->getPopupReference(i);
		if (pPopup->albumthumbsrc.empty() == false)
			setImageFilename.insert(pPopup->albumthumbsrc);
		if (pPopup->albumthumbsrc2.empty() == false)
			setImageFilename.insert(pPopup->albumthumbsrc2);
		if (pPopup->stBackground.image.empty() == false)
			setImageFilename.insert(pPopup->stBackground.image);

		for (int s = 0; s < pPopup->vtContents.size(); s++) {
			STCONTENT_INFO * pContentInfo = pPopup->vtContents.at(s);
			if (pContentInfo->image.empty() == false)
				setImageFilename.insert(pContentInfo->image);
			if (pContentInfo->disabled.empty() == false)
				setImageFilename.insert(pContentInfo->disabled);
			if (pContentInfo->selected.empty() == false)
				setImageFilename.insert(pContentInfo->selected);
			for (int t = 0; t < pContentInfo->vtAnimation.size(); t++) {
				STANIMATION * pAni = pContentInfo->vtAnimation.at(t);
				if (pAni->image.empty() == false)
					setImageFilename.insert(pAni->image);
			}
		}
	}
}

/*페이지내에 사운드 목록을 가져온다.*/
void KPreloadingPage::makeSoundStringFromPageInfo(set<string> & setSoundFilename){
	KXMLReader * mpReader = KXMLReader::getInstance();
	if (mpReader->getBookInfo()->backgroundSound.empty() == false) {
		setSoundFilename.insert(mpReader->getBookInfo()->backgroundSound );
	}
	for (int i = 0; i < mpReader->getPageCount(); i++) {
		HPAGE_INFO * pInfo = mpReader->getPageReference(i);
		if (pInfo->backgroundMusic.empty() == false)
			setSoundFilename.insert(pInfo->backgroundMusic);
		if (pInfo->narration.empty() == false)
			setSoundFilename.insert(pInfo->narration);

		for (int s = 0; s < pInfo->vtContents.size(); s++) {
			STCONTENT_INFO * pContentInfo = pInfo->vtContents.at(s);
			if (pContentInfo->sound.empty() == false)
				setSoundFilename.insert(pContentInfo->sound);

		}
	}
}

void KPreloadingPage::makeSoundStringFromPopupInfo(set<string> & setSoundFilename){
}

