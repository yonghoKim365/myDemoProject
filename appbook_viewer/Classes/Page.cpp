#include "Page.h"
#include "KXMLReader.h"
#include "KGlobalManager.h"
#include "SimpleAudioEngine.h"
#include "AudioEngine.h"
#include "KDataProvider.h"
#include "KCameraManager.h"
#include "KJSONDataManager.h"
#include "KPopupLayer.h"
#include "strconvert.h"
#include "KRecordingScene.h"
#include "HitDetectHelper.h"
#include "KBlankLayer.h"

/* 페이지 생성한다. */
CPage* CPage::create(HPAGE_INFO* pPageInfo, STBOOK_INFO * pBookInfo){
	CPage* page = new CPage();
	srand(uint16_t(time(NULL)));
	if (page && page->init(pPageInfo, pBookInfo)) {
		page->autorelease();
		return page;
	}
	CC_SAFE_RELEASE(page);
	return nullptr;
}

/* 페이지 생성시 변수 초기화 */
CPage::CPage() {
	mAlbum = nullptr;
	mIsPopup = false;
	mAudioEngineID = AUDIOENGINE_UNDEFINED;
	mPopupLength = 0;
	mPageLength = 0;
}


CPage::~CPage() {
	if (m_pPageInfo != nullptr) log("CPage::~CPage() type=%s pageno=%s", m_pPageInfo->type.c_str(), m_pPageInfo->pageno.c_str());
}

/* 초기화 작업을 진행한다. */
bool CPage::init(HPAGE_INFO* pPageInfo, STBOOK_INFO * pBookInfo){

	m_pPageInfo = pPageInfo;  /*책 정보 저장*/
	m_pBookInfo = pBookInfo;  /*페이지 정보 저장*/

	mpJtpButtonInfo = nullptr;    /*Jump to Page 초기화*/
	mpExitButtonInfo = nullptr;
	mBtn = nullptr;           /* 듣기 모드 pause 시 재시작 버튼 초기화 */

	mPlayNarrationPos = 0;    /* narration list 위치 */
	mTimerDuratin = 0;        /* 0.1마다 시간증가 애니메이션 나올 시간 확인 -> schedule_selector(CPage::callAfterAniStart), 0.1f)  */
	vtAni.clear();            /* 바로 시작하지 않는 애니메이션 저장 */

	prevSoundAniSpr = nullptr;
	lastAniSpr = nullptr;

	lastCommandOfButton = COMMAND_NULL;

	int nCurAudio = KGlobalManager::getInstance()->getCurrentAudio();
	if (nCurAudio != AUDIOENGINE_UNDEFINED)
	{
		cocos2d::experimental::AudioEngine::stop(nCurAudio);
	}
	
	int nCurPage = atoi(pPageInfo->pageno.c_str()); /*현제 페이지 번호 */
	int nPageCount = KXMLReader::getInstance()->getPageCount(); /* 전체 페이지수 */
	if (nCurPage == 999)      /* endpage는 999로 지정됨 (제너레이터에서 관리) */
		nCurPage = nPageCount - 1;

	mCurPage = nCurPage;

	if (mCurPage == 1000){ // embadded end of page
		return initEndPage();
	}

	if (mCurPage == 0){
		KGlobalManager::getInstance()->setAutoPlay(false); /*읽기모드로 초기화*/
		//KDataProvider::getInstance()->setListenMode(1);
	}

	/*해당페이지 사용자 녹음파일이 있는지 확인*/
	m_userPlaysound = StringUtils::format("%s/%s/%d.mp3", KRecordingManager::getInstance()->getAndroidTargetPath().c_str()
		, pBookInfo->projectcode.c_str(), nCurPage);

	m_isUserSound = false; /*사용자 녹음파일 제어변수*/
	
	/*사용자 녹음파일 존재 유무 확인*/
	if (FileUtils::getInstance()->isFileExist(m_userPlaysound)) m_isUserSound = true;
	/* 사용자 녹음된 음성 play면 true / 컨텐츠 나래이션 play이면 false */
	bool bUserMode = KGlobalManager::getInstance()->getNarration();

	if (bUserMode) /* User Narration Mode */
	{
		if (m_isUserSound) /* 녹음된 파일이 있다. */
		{
			HVOICE_INFO * pvoice = new HVOICE_INFO();
			pvoice->autorelease();

			vtVoiceManager.pushBack(pvoice);
			pvoice->sort = "0";
			pvoice->sound = m_userPlaysound;
		}
	}

	this->scheduleUpdate();

	// 배경용 레이어
	mBackground = Layer::create();
	this->addChild(mBackground);

	// 오브젝트 레이어 
	mContent = Layer::create();
	this->addChild(mContent);

	// UI용 레이어
	mUIObject = Layer::create();
	this->addChild(mUIObject);

	/*페이지 백그라운드 적용*/
	STBACKGROUND backgroundInfo = pPageInfo->stBackground;
	setBackground(backgroundInfo, mBackground);

	/*팝업관리 초기화*/
	mSetPopupIDs.clear();

	/*페이지 속성중 나레이션이 있으면 적용*/
	if (pPageInfo->narration.empty() == false)
	{
		STCONTENT_INFO * pContentInfo = new STCONTENT_INFO();
		pContentInfo->group = "TEXTGROUP";
		pContentInfo->sort = "0";
		pContentInfo->sound = pPageInfo->narration;
		pContentInfo->id = "page_na";

		playNarrationList(pContentInfo);
	}

	/*페이지 아이템들 종류별 구성*/
	for (int i = 0; i < pPageInfo->vtContents.size(); ++i) {
		STCONTENT_INFO *  pContentInfo = pPageInfo->vtContents.at(i);
		
		if (pContentInfo->type == "action") { /*액션 구성*/
			appendButton(pContentInfo, mUIObject);
		}
		else if (pContentInfo->type == "animation") {  /*애니메이션 구성*/
			if (pContentInfo->startani.empty()) pContentInfo->startani = "0";
			
			if (pContentInfo->startani == "0"){    /*애니메이션 페이지 시작시 바로 구성*/
				appendAnimation(pContentInfo, mContent);
			}
			else{                                   /*startani 시간이 되면 화면에 나타나게 구성*/
				appendAnimation(pContentInfo, mContent , false);
				vtAni.pushBack(pContentInfo);
			}
			addSoundEvent(pContentInfo, mContent);  /*사운드 처리*/
		}
		else if (pContentInfo->type == "image") {   /*이미지 구성*/
			appendImage(pContentInfo, mContent);
			playNarrationList(pContentInfo);
			addSoundEvent(pContentInfo, mContent);  /*사운드 처리*/ 
		}
		else if (pContentInfo->type == "text") {     /*텍스트 구성*/
			appendText(pContentInfo, mContent);
			playNarrationList(pContentInfo);
			addSoundEvent(pContentInfo, mContent);   /*사운드 처리*/ 
		}
		else if (pContentInfo->type == "textinput") {    /*텍스트 인풋 구성*/
			appendTextInput(pContentInfo, mContent);
		}
		else if (pContentInfo->type == "particlearea") { /*파티클 구성*/
			appendParticle(pContentInfo, mUIObject);
		}
	}

	set<string>::iterator iter;
	/*아이템 구성중 팝업 사용 아이디 있을 경우 등록됨*/
	for (iter = mSetPopupIDs.begin(); iter != mSetPopupIDs.end(); iter++) {
		mPopupLength++;
	}

	mPopup = new Layer*[mPopupLength];
	int nTempCount = 0;

	

	/*해당 페이지에 적용할 팝업 구성*/
	for (iter = mSetPopupIDs.begin(); iter != mSetPopupIDs.end(); iter++) {
		/* xml에서 읽은 팝업 정보를 가지고 온다.*/
		HPOPUP_INFO * pPopupInfo = KXMLReader::getInstance()->getPopupReference(*iter);
		if (pPopupInfo == nullptr) {
			log("Error !!! Occurred...  Happened.. Caution... can not reference target popup id in the page entity. Please reference to the valid one. from K.");
			log("Can't make Referenced Popup..");
			break;
		}
		mPopup[nTempCount] = Layer::create();   /*Popup Layer구성*/
		this->addChild(mPopup[nTempCount]);



		mPopup[nTempCount]->setName(pPopupInfo->id); 

		setBackground(pPopupInfo->stBackground, mPopup[nTempCount]); /*레이어 배경 적용*/

		if (pPopupInfo->id == "album") {                  /*album이면 Jump To Page 팝업임*/
			appendAlbum(pPopupInfo, mPopup[nTempCount]);  /*팝업 생성*/
			mPopup[nTempCount]->setVisible(false);
			nTempCount++;
			continue;     /*Jump to Page는 초기 기획된 내용 변경되어 자체 구성함*/
		}

		mIsCameraActionOn = false;  /*카메라 on/off 제어 변수*/

		/*팝업내에 구성요소 생성*/
		for (int j = 0; j < pPopupInfo->vtContents.size(); ++j) {
			STCONTENT_INFO *  pContentInfo = pPopupInfo->vtContents.at(j);

			if (pContentInfo->type == "action") {
				appendButton(pContentInfo, mPopup[nTempCount]);
			}
			else if (pContentInfo->type == "animation") {
				appendAnimation(pContentInfo, mPopup[nTempCount]);
			}
			else if (pContentInfo->type == "image") {
				appendImage(pContentInfo, mPopup[nTempCount]);
			}
			else if (pContentInfo->type == "text") {
				appendText(pContentInfo, mPopup[nTempCount]);
			}
			else if (pContentInfo->type == "textinput") {
				appendTextInput(pContentInfo, mPopup[nTempCount]);
			}
		}
		/*아이템 구성요소중 카메라 사용이 있으면 mIsCameraActionOn = true 된다.*/
		if (mIsCameraActionOn == true) {
			/*카마라로찍으 사진 Node받아 온다.*/
			cocos2d::Node * pShowingImageNode = KCameraManager::getInstance()->getActionTargetNode();
			cocos2d::Sprite * pShowingCameraImage = static_cast<cocos2d::Sprite*>(pShowingImageNode);

			if (pShowingCameraImage != nullptr) {  /*찍은 사진이 있으면 적용한다.*/
				/*사진 풀경로를 받아온다.*/
				std::string sPictureName = KCameraManager::getInstance()->getFullpathnameForPicture();
				/*파일이 있는지 확인한다.*/
				long nSize = FileUtils::getInstance()->getFileSize(sPictureName);
				if (nSize > 10) {
					log("picture file exists... ");
					Director::getInstance()->getTextureCache()->removeTextureForKey(sPictureName);

					/*카메라 스프라이트를 만든다.*/
					Sprite * pSprite = KCameraManager::getInstance()->getSprite();
					pSprite->initWithFile(sPictureName);  /*찍은 사진을 가져온다.*/

					cocos2d::Size size = pSprite->getContentSize();

					Node * pTargetNode = KCameraManager::getInstance()->getActionTargetNode();
					if (pTargetNode == nullptr) {
						log("can't find the right target node.. check plz..");
						return false;
					}
					/*지정된 영역에 사진 적용*/
					cocos2d::Size targetSize = pTargetNode->getContentSize();
					float scaleX = targetSize.width / size.width;
					float scaleY = targetSize.height / size.height;
					float scaleTotal = scaleX > scaleY ? scaleY : scaleX;
					pSprite->setScale(scaleTotal);
					KCameraManager::getInstance()->getActionTargetLayer()->addChild(pSprite);
					pSprite->setPosition(pTargetNode->getPosition());

				}
				else {
					log("picture file not exists...");
				}

			}
			/*입력 받은 사용자 이름 받아오기 적용*/
			auto pTextField = KCameraManager::getInstance()->getTextFieldObject();
			std::string sResult = KJSONDataManager::getInstance()->getCameraData(KXMLReader::getInstance()->getBookInfo()->projectcode);
			const std::string & refString = sResult;
			pTextField->setString(sResult);
		}

		mPopup[nTempCount]->setVisible(false);
		nTempCount++;
	}
	
	log("Page::init,, mCurPage :%d", mCurPage);
	log("Page::init,, getIsFirst :%d", KDataProvider::getInstance()->getIsFirst());
	if (mCurPage != 0){ // 첫 페이지에서는 continueCheck - 다시보기/이어보기 팝업이후에 sound play 시켜준다. // continueCheck 안뜰때는?
		playBgmAndNarrationAndStartAni();
	}
	else if (KDataProvider::getInstance()->getIsFirst() == false && mCurPage == 0){ // 시작한후 다시 되돌아왔을때 
		playBgmAndNarrationAndStartAni();
	}

	//  e-book 이고 첫 페이지가 아니면 pause화면 생성해 둠 
	if (KGlobalManager::getInstance()->getAutoPlay() && mCurPage != 0)
	{
		appendBlankSprite();
	}

	// e-book 이고 첫페이지이고 안내팝업을 띄운적이 없다면 안내팝업을 띄운다. 
	// auto play 모드에서는 페이지가 자동으로 넘어가고 Read to Me 모드에서는..

	if (KDataProvider::getInstance()->getMediaType() == "APE_ZIP" && mCurPage == 0)
	{
		string c = m_pBookInfo->projectcode;
		c.append("_ebook_guidePopup_nomore_check");
		bool bEBook_GuidePopup_shown = UserDefault::getInstance()->getBoolForKey(c.c_str(), false);
		//log(" %s bEBook_GuidePopup_shown : %d",c.c_str(), bEBook_GuidePopup_shown);

		if (bEBook_GuidePopup_shown == false){
			KPopupLayer * pLayer = KPopupLayer::createLayer();
			this->addChild(pLayer);
			pLayer->setEBookIBookGuidePopupCallback(c, true, [&](cocos2d::Ref*){
				//setEBookGUidePopupReadCheck();
				// 팝업 안에서 하도록 변경
			});
		}
	}

	// 스토리를 들어요. 다시 듣고 싶을때는 그림 또는 문장을 터치해 보세요. 빨간색으로 표시된 파닉스 워드를 터치하면 소리와 뜻을 확인할 수 있어요
	if (KDataProvider::getInstance()->getMediaType() == "API_ZIP" && mCurPage == 0){
		string c = m_pBookInfo->projectcode;
		c.append("_ibook_guidePopupChecked");
		bool bGuidePopupChecked = UserDefault::getInstance()->getBoolForKey(c.c_str(), false);
		//if (KDataProvider::getInstance()->getIsFirst() == 1 || bGuidePopupChecked == false){

		if (bGuidePopupChecked == false){
			KPopupLayer * pLayer = KPopupLayer::createLayer();
			this->addChild(pLayer);

			pLayer->setEBookIBookGuidePopupCallback(c, false, [&](cocos2d::Ref*){
				//guidePopupCheck();
			});
		}
	}
	
	//}
	/* // end of page를 따로 추가하여 마지막 페이지가 end of page가 아니므로 음악을 멈추지 않게 한다. 160718 kyh
	else{ 
		
		// 마지막 페이지 bgm stop 자신 bgm만 play
		//stopBGM();
		//if (KDataProvider::getInstance()->getSoundState() && !pPageInfo->backgroundMusic.empty()) {
		//	setBgm(pPageInfo->backgroundMusic);
		//}
	}
	*/
	
	log(" Page::init(), ================ mCurPage:%d, nPageCount:%d, getIsFirst():%d", mCurPage, nPageCount, KDataProvider::getInstance()->getIsFirst());

	if(KDataProvider::getInstance()->getIsFirst() == false){      /* 다시 하기로 replay하는 상황이 아니면 */
		KDataProvider::getInstance()->setProgressPage(nCurPage);  /*현재 진도정보 전달*/
		          
		if (mCurPage == (nPageCount - 1)){                        /*마지막페이지이면*/
			KDataProvider::getInstance()->setFinished();          /*완독상태 알림, MSLP.finish 아님.*/ 
			KDataProvider::getInstance()->releaseCpuLock();       /*슬립모드 가동되게 요청*/
		}
		return true; 
	}

	int nVol = KDataProvider::getInstance()->getSystemVolumn();   // 시스템 볼륨 받아옴
	
	if (nVol == 0 && mCurPage == 0)
	{
		KPopupLayer * pLayer = KPopupLayer::createLayer();
		this->addChild(pLayer);
		pLayer->volumeMuteWarningPopupCallback([&](cocos2d::Ref*){

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
			continueCheck();
#endif
		});
	}
	else{
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		continueCheck();
#endif
	}


	// win32 환경에서는 continueCheck를 하지 않으므로 첫페이지에서 음악이 나오지 않음.
	// 버그로 오인하는 케이스가 있어 사운드를 플레이 시켜줌. 
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	if (mCurPage == 0){
		playBgmAndNarrationAndStartAni();
	}
#endif

	// Swipe  애니메이션 북 특성에 맞지않음 (책내 상호작용 이벤트 많음)
	/*
	auto listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(CPage::onTouchBegan, this);
	listener->onTouchMoved = CC_CALLBACK_2(CPage::onTouchMoved, this);
	listener->onTouchEnded = CC_CALLBACK_2(CPage::onTouchEnded, this);
	listener->onTouchCancelled = CC_CALLBACK_2(CPage::onTouchCancelled, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, this);
	mVisibleSize = Director::getInstance()->getVisibleSize();
	mIsTouchDown = false;
	mInitialTouchPos.x = mInitialTouchPos.y = 0;
	*/
	return true;
}

void CPage::setEBookGUidePopupReadCheck(){
	string c = m_pBookInfo->projectcode;
	c.append("_ebook_guidePopup_nomore_check");
	UserDefault::getInstance()->setBoolForKey(c.c_str(), true);

	bool bEBook_GuidePopup_shown = UserDefault::getInstance()->getBoolForKey(c.c_str(), false);
	//log("setEBookGUidePopupReadCheck, code:%s, bEBook_GuidePopup_shown:%d", c.c_str(), bEBook_GuidePopup_shown);
}

bool CPage::initEndPage(){
	return true;
}

void CPage::guidePopupCheck()
{
//	UserDefault::getInstance()->setBoolForKey("guidePopupChecked", true);
//	UserDefault::getInstance()->flush();

	int a = 0;

	//KPopupLayer *popupLayer = (KPopupLayer*)pSender;
	//this->retain();
	//popupLayer->removeFromParent();
}

/*마지막에 저장된 진도정보 확인 후 작동*/
void CPage::continueCheck()
{
	int nPageCount = KXMLReader::getInstance()->getPageCount();   /*페이지 전체수 받아옴*/
	KDataProvider::getInstance()->isFinished();                   /*완독상태인지 JNI호출*/
	KDataProvider::getInstance()->getProcess();                   /*마지막에 저장된 진도정보 요청 JNI*/

	log("CPage::continueCheck(), getOneFinished:%s", KDataProvider::getInstance()->getOneFinished().c_str());

	int nProgressPage = KDataProvider::getInstance()->getProcessPage();/*마지막에 저장된 진도정보 받아오기*/
	if (nProgressPage == (nPageCount - 1)) nProgressPage = 0;
	KDataProvider::getInstance()->setFirst(false); /*처음 시작이 아님을 알림*/
	/*진도 정보가 있으면 알림 메세지 보내기*/

	if (nProgressPage > 0)  
	{
		KPopupLayer * pLayer = KPopupLayer::createLayer();
		this->addChild(pLayer);

		pLayer->continueYesNoTypeCallback(
			[&](cocos2d::Ref*){
			AlertCallback(0, NativeAlert::ButtonType::RETURN); // 이어보기
		},
			[&](cocos2d::Ref*){
			AlertCallback(0, NativeAlert::ButtonType::CANCEL); // 처음부터 보기
		});
		

		/*android 일반 팝업 사용시 */
		//NativeAlert::showWithCallback( __TX("확인").c_str(),  __TX("처음부터 보려면(예)\n이전 진도부터 보려면(계속)을 눌러주세요.").c_str(), __TX("계속").c_str(), __TX("예").c_str(), "", 404, CC_CALLBACK_2(CPage::AlertCallback, this));
	}
	else {

		playBgmAndNarrationAndStartAni();

		KDataProvider::getInstance()->setBeginBook(0, nPageCount - 1, 0);  /*학습 시작 알림 JNI*/
	}
}

/*사용자 팝업 선택한 event 확인*/
void CPage::AlertCallback(int tag, NativeAlert::ButtonType type)
{ 

	log("AlertCallback %d", tag);
	int nPageCount = KXMLReader::getInstance()->getPageCount();  /*전체 페이지수 받아옴*/
	int nProgressPage = KDataProvider::getInstance()->getProcessPage(); /*진도정보 받아옴*/
	/*마지막 저장된 듣기 읽기 모드 가져오기*/
	int nMode = KDataProvider::getInstance()->getListenMode();

	int i = 0;
	int* page;
	
	if (nMode == 0) {
		KGlobalManager::getInstance()->setAutoPlay(true); // Listen Mode
		KDataProvider::getInstance()->acquireCpuWakeLock();
	}
	else KGlobalManager::getInstance()->setAutoPlay(false);  //READ Mode

	switch (type)
	{
	case NativeAlert::ButtonType::OTHER:  /*사용안하는 이벤트 Native Popup시 필요*/
		break;
	case NativeAlert::ButtonType::CANCEL:  /*처음부터 보기*/
		log("AlertCallback first ");
		
		playBgmAndNarrationAndStartAni();

		KDataProvider::getInstance()->setBeginBook(0, nPageCount - 1, nMode);

		break;
	case NativeAlert::ButtonType::RETURN:  /*이어보기*/
		log("AlertCallback current page continue ");
		i = nProgressPage;
		if (i < 0) i = 0;
		KDataProvider::getInstance()->setBeginBook(i, nPageCount - 1, nMode); // 학습시작알림 JNI
		page = new int[1]{i};
		// 해당 페이지로 이동 

		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		break;

	}

}

void CPage::playBgmAndNarrationAndStartAni(){
	playBGM();

	if (vtVoiceManager.size() > 0) {  // 나레이션 리스트에 있는 음원 플레이
		this->scheduleOnce(schedule_selector(CPage::callAfterOneSecond), 1.0f);
	}
	if (vtAni.size() > 0) {   // 애니메이션 자신이 나와야 되는 시간에 나오게 하기
		this->schedule(schedule_selector(CPage::callAfterAniStart), 0.1f);
	}
}

/*나레이션 순번대로 정렬하기*/
bool sortByX(const Ref* obs1, const Ref* obs2)
{
	HVOICE_INFO* ob1 = (HVOICE_INFO*)obs1;
	HVOICE_INFO* ob2 = (HVOICE_INFO*)obs2;
	return ob1->sort < ob2->sort;
}

/*나레이션 플레이 리스트 관리*/
void CPage::playNarrationList(STCONTENT_INFO *  pContentInfo)
{
	//playingcolor

	bool bUserMode = KGlobalManager::getInstance()->getNarration();

	if (bUserMode) /* User Narration 모드 (주의 : 사용자 모드라도 녹음된 파일이 없으면 시스템 Narration등록함.)*/
	{
		if (m_isUserSound) /*녹음된 파일이 있음  init에서 사용자음원 등록했음*/
		{
			return;
		}
	}

	if (pContentInfo->group.length() > 0)
	{
		HVOICE_INFO * pvoice = new HVOICE_INFO();
		pvoice->autorelease();

		pvoice->sort = pContentInfo->sort;
		pvoice->sound = pContentInfo->sound;
		pvoice->id = pContentInfo->id;
		pvoice->playingcolor = pContentInfo->playingcolor;
		vtVoiceManager.pushBack(pvoice);


		std::sort(vtVoiceManager.begin(), vtVoiceManager.end(), sortByX);
	}
}

//animation starttime control
void CPage::callAfterAniStart(float dt){
	float ft = 0;
	int nSize = vtAni.size();
	mTimerDuratin += dt;

	if (nSize == 0) {  /*애니메이션이 모두 화면에 나오면 unschedule 한다. */
		this->unschedule(schedule_selector(CPage::callAfterAniStart));
	}

	for (int i = 0; i < nSize; ++i) {
		STCONTENT_INFO *  pContentInfo = vtAni.at(i);
		ft = atof(pContentInfo->startani.c_str());
		if (ft <= mTimerDuratin){  /*애니메이션이 나와야할 시간이면 나오게 한다. 그리고 vtAni에서는 제거*/
			
			Vector<Node *>  vecChildren = mContent->getChildren();
			for (int j = 0; j < vecChildren.size(); j++) {
				Node * pNode = vecChildren.at(j);
				std::string txtname = (std::string) pNode->getName();
				if (std::strcmp(txtname.c_str(), pContentInfo->id.c_str()) == 0) {
					playAnimation(pContentInfo, mContent, (Sprite*)pNode);
					break;
				}
			}
			vtAni.erase(i);
			return;
		}
	}
	
}

/*나레이션리스트 플레이 시작하기*/
void CPage::callAfterOneSecond(float dt){
	log("Start Duration...%f",dt);
	playNarrationList();
}

/*나레이션 하나씩 플레이 하기*/
void CPage::playNarrationList()
{
	string sFile = getNarrationPostion(); /*플레이할 음원 파일 받아온다.*/
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();  /*듣기모드인지 읽기모드인지 받아온다*/
	if (sFile == "")
	{
		if (KGlobalManager::getInstance()->getAutoPlay() == false) return;
		/*듣기 모드이고 다 들었으면 자동으로 다음 페이지로 이동한다.*/
		command(COMMAND_NEXT, "");
		return;
	}
	/*듣기 모드일 경우 화면 터치시 pause화면이 노출되면 플레이를 멈준다.*/
	if (mBtn != nullptr){
		if (mBtn->isVisible()){
			return;
		}
	}
	bool isUser = false;
	bool bUserMode = KGlobalManager::getInstance()->getNarration(); /*녹음된 음성인지 아닌지 가져온다.*/
	if (m_isUserSound)
	{
		if (bUserMode) isUser = true;
	}

	
	if (bAuto == false)  /*읽기모드일 경우*/
	{
		/*음원이 플레이중 일때 슬립모드 되면 안됨*/
		KDataProvider::getInstance()->acquireCpuWakeLock(); /*슬립모드 방지 JNI호출*/
	}

	if (isUser){ /*사용자 음원 설정이면 녹음된 파일 플레이*/
		mAudioEngineID = playSound(sFile);
	}
	else{
		mAudioEngineID = playSound(KCTT(sFile));
		mNarrationAudioID = mAudioEngineID;
	}

	KGlobalManager::getInstance()->setCurrentAudio(mAudioEngineID);
	/*음원 재생이 종료 되면 callback*/
	cocos2d::experimental::AudioEngine::setFinishCallback(mAudioEngineID, [&](int audioID, const std::string & fileName) {
		mAudioEngineID = AUDIOENGINE_UNDEFINED;
		mNarrationAudioID = AUDIOENGINE_UNDEFINED;
		bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
		if (bAuto == false)
		{
			/*음원 종료시 슬립모드 가동되게 JNI*/
			KDataProvider::getInstance()->releaseCpuLock();
		}
		setTextOrg(); /*텍스트 하이라이트 원복*/
		playNarrationList(); /*다음 문장 읽기*/
	});
}

/*묵음 모드 판별후 나레이션 플레이 되게 작동*/
int CPage::playSound(std::string sFile)
{
	return playSound(sFile, false);
}

int CPage::playSound(std::string sFile, bool bLoop)
{
	float fSoundVol = 0.0f;
	bool bSound = KDataProvider::getInstance()->getSoundState(); /*묵음모드인지 판별 (Jump to Page에서 묵음모드 설정가능)*/
	int nAudio;
	
	if (bSound)  //정상모드
	{
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), bLoop);
	}
	else {       //*묵음모드 :  묵음모드 라도 계속 진행 되어야 하므로 볼륨을 줄인다.
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), bLoop, fSoundVol);
	}

	return	nAudio;
	
}

/*플레이 할 나레이션 음원 파일을 받아온다.*/
string CPage::getNarrationPostion()
{
	int nList = vtVoiceManager.size();
	if (nList == 0) return "";

	if (mPlayNarrationPos >= nList) return "";
	/*mPlayNarrationPos 위치 관리*/
	HVOICE_INFO *  pVoice = (HVOICE_INFO *)vtVoiceManager.at(mPlayNarrationPos);
	mPlayNarrationPos++;

	/*나레이션에 해당되는 텍스트를 찾아 하이라이트 한다.*/
	searchObject(pVoice->id,pVoice->playingcolor);

	return pVoice->sound;
}

/*나레이션관 연관되어 있는 텍스트를 초기값 색으로 복원한다.*/
void CPage::setTextOrg()
{
	STCONTENT_INFO *  pContentInfo = nullptr;
	for (int i = 0; i < m_pPageInfo->vtContents.size(); ++i) {
		pContentInfo = m_pPageInfo->vtContents.at(i);

		setTextOrgColorReal(pContentInfo->id, pContentInfo->fontcolor);
	}
	
}

/*나래이션에 해당되는 text를 찾아 색을 바꾼다.*/
void CPage::searchObject(string id,string playingcolor)
{
	Vector<Node *>  vecChildren = mContent->getChildren();

	for (int i = 0; i < vecChildren.size(); i++) {
		Node * pNode = vecChildren.at(i);
		std::string txtname = (std::string) pNode->getName();
		if (std::strcmp(txtname.c_str(), id.c_str()) == 0) {
			if (std::strncmp(txtname.c_str(), "text", 4) == 0) {
				Label* label = (Label*)pNode;
				Color3B color;
				if (playingcolor.empty()) color = convertRGB("#ff00ff"); //지정된 색이 없으면 
				else color = convertRGB(playingcolor.c_str());
				label->setTextColor(Color4B(color));
				return;
			}
		}
	}
}

/*나레이션에 해당하는 Text 초기값 색으로 적용하기*/
void CPage::setTextOrgColor(string id)
{
	STCONTENT_INFO *  pContentInfo = nullptr;
	for (int i = 0; i < m_pPageInfo->vtContents.size(); ++i) {
		pContentInfo = m_pPageInfo->vtContents.at(i);

		if (pContentInfo->id == id) {
			break;
		}
	}
	if (pContentInfo == nullptr) return;
	setTextOrgColorReal(id, pContentInfo->fontcolor);
}

/*아이디에 해당되는 텍스트 찾아 원래 색으로 적용*/
void CPage::setTextOrgColorReal(string id,string orgColor)
{
	Vector<Node *>  vecChildren = mContent->getChildren();
	for (int i = 0; i < vecChildren.size(); i++) {
		Node * pNode = vecChildren.at(i);
		std::string txtname = (std::string) pNode->getName();
		if (std::strcmp(txtname.c_str(), id.c_str()) == 0) {
			if (std::strncmp(txtname.c_str(), "text", 4) == 0) {
				Label* label = (Label*)pNode;
				Color3B color;
				if (orgColor.empty()) color = convertRGB("#000000");
				else color = convertRGB(orgColor.c_str());
				label->setTextColor(Color4B(color));
				return;
			}
		}
	}
}

/*모든 액션 처리*/
void CPage::command(string command, string param, string param2, Ref * sender ) {
	if (command.empty()) {
		return;
	}
	 
	/*액션이 되면 상호작용이 된 경우 이므로 */
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
	if (bAuto == false) 
	{
		KDataProvider::getInstance()->releaseCpuLock();  /*슬립모드 작동되게 JNI 호출*/
	}

	if (command == COMMAND_PREV || command == COMMAND_NEXT
		|| command == COMMAND_PREV_NOPAGING || command == COMMAND_NEXT_NOPAGING
		|| command == COMMAND_AUTO_PLAY || command == COMMAND_MANUAL_PLAY
		|| command == COMMAND_COVERPAGE || command == COMMAND_SETPAGE
		|| command == COMMAND_POPUP 
		|| command == COMMAND_CAMERA || command == COMMAND_RECORD
		) {

		unscheduleAllCallbacks();

		if (mAudioEngineID != AUDIOENGINE_UNDEFINED)
			cocos2d::experimental::AudioEngine::stop(mAudioEngineID);
	}
	
	if (command == COMMAND_PREV) {     /*이전 페이지로 이동*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_PREV);
	}
	else if (command == COMMAND_NEXT) { /*다음 페이지로 이동*/
		int nCurPage = atoi(m_pPageInfo->pageno.c_str()); /*현제 페이지 번호 */
		//int nPageCount = KXMLReader::getInstance()->getPageCount(); /* 전체 페이지수 */

		if (nCurPage == 0){
			// 첫 페이지의 next는 i-playbook 에만 있음. i-playbook에는 setListenMode를 해주는곳이 없어서 첫페이지의 next를 setListenmode로 사용한다.
			KGlobalManager::getInstance()->setAutoPlay(false); /*읽기모드 사용*/
			KDataProvider::getInstance()->setListenMode(1);                /*모드 변경을 알린다. JNI*/
			KDataProvider::getInstance()->releaseCpuLock();
		}
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_PREV_NOPAGING) {   /*이전 페이지로 이동 넘김효과 없이..*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_PREV_NOPAGING);
	}
	else if (command == COMMAND_VOICE_SYSTEM) {    /*나레이션 컨텐트 사용*/
		KGlobalManager::getInstance()->setNarration(false);
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_VOICE_USER) {               /*나레이션 사용자 녹음 사용*/ 
		KGlobalManager::getInstance()->setNarration(true);  /*사용자 녹음된 것 사용*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_AUTO_PLAY) { /*리슨모드 나레이션 auto 값을 설정한다. 모드가 바뀌면 맨 처음 부터 시작한다.*/
		KGlobalManager::getInstance()->setAutoPlay(true); /*듣기모드 사용*/
		KDataProvider::getInstance()->setListenMode(0);               /*모드 변경을 알린다. JNI*/
		/*리슨모드는 자동으로 페이지가 넘어간다. 슬립모드 방지*/
		KDataProvider::getInstance()->acquireCpuWakeLock();
		
		int i = 1;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		
	}
	else if (command == COMMAND_MANUAL_PLAY) {  /*읽기모드*/
		KGlobalManager::getInstance()->setAutoPlay(false); /*읽기모드 사용*/
		KDataProvider::getInstance()->setListenMode(1);                /*모드 변경을 알린다. JNI*/
	    /*읽기모드는 슬립모드 해제*/
		KDataProvider::getInstance()->releaseCpuLock();
		//기획 변경 되어 유저 선택 없어짐
	    //showPopup("selectedUser");
		
		int i = 1;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		
	}
	else if (command == COMMAND_COVERPAGE) { /*커버 페이지로 이동*/
		int i = 0;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
	}

	else if (command == COMMAND_SETPAGE || command == COMMAND_GOTO) { /*지정된 페이지로 이동*/
		int i = atoi(param.c_str());
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) /*읽지 않은 페이지는 이동 한됨*/
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();
		int nProgressPage = KDataProvider::getInstance()->getProcessPage();
		if (isFindshed == "n"){
			if (i > nProgressPage){
				MessageBox(__TX("읽지 않은 페이지로 이동할 수 없습니다.").c_str(), __TX("확인").c_str());
				return;
			}
		}
#endif
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
	}
	else if (command == COMMAND_EXIT) {    /*종료*/
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();  /*완독되었는지 확인*/
		/*완독여부 전달 JNI*/
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif	
		Director::getInstance()->end();
		
	}
	else if (command == COMMAND_NEXTBOOK){    /*다음 컨텐트로 이동*/
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32)  /*먼저 완독정보 전달*/
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif		
		KDataProvider::getInstance()->nextBook();  /*다음 컨텐트 이동 요청 JNI*/
		Director::getInstance()->end();
	}
	else if (command == COMMAND_POPUPEXIT) {       /*해당 팝업 종료*/
		Node * pNode = (Node *)sender;
		pNode->getParent()->setVisible(false);
		mIsPopup = false; /*팝업 활성여부 관리*/

		playBGM();

		/*애니메이션 가동*/
		Vector<Node *>  vecChildren = mContent->getChildren();
		for (int i = 0; i < vecChildren.size(); i++) {
			Node * pNode = vecChildren.at(i);
			std::string aniname = (std::string) pNode->getName();
			if (std::strncmp(aniname.c_str(), "ani",3) == 0) {
				if (pNode->isVisible())
					pNode->resume();
			}
		} 
		if (mAlbum != nullptr) /*Jump to Page 팝업 종료 처리 처리*/
		{
			mPopup[mJTPpopupIndex]->setVisible(false);

			mAlbum->setBlocked(false);

			/*나래이션 재 가동*/

			//cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
			cocos2d::experimental::AudioEngine::resume(mNarrationAudioID);

			//if (mPlayNarrationPos != 0) mPlayNarrationPos--;
			//playNarrationList();

			if (mPlayNarrationPos != 0 && mNarrationAudioID != AUDIOENGINE_UNDEFINED) mPlayNarrationPos--;

			playNarrationList();

			// sound off 상태라도 이북에서는 사운드종료 이벤트받아 다음페이지가야 하므로 볼륨을 0으로 하고 플레이 시킨다.
			float fSoundVol = 0.0f;
			if (KDataProvider::getInstance()->getSoundState()){
				fSoundVol = 1.0f;
			}
			cocos2d::experimental::AudioEngine::setVolume(fSoundVol, mAudioEngineID);

			bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
			if (bAuto)
			{
				pEventListenerBack->setSwallowTouches(true);
				m_pBlockingSprite->setVisible(true);
				mBlankLayers->setVisible(true);
				
				/*나래이션 재 가동*/ // 위로 이동. 
				//cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
				//if (mPlayNarrationPos != 0) mPlayNarrationPos--;
				//playNarrationList();
			}
		}
			
	}
	else if (command == COMMAND_HIDE) {  /*hide 액션 처리*/
		if (sender != nullptr)   /*자신은 기본 적으로 hide*/
		{
			Node * pNode = (Node *)sender;
			Sequence * seq = Sequence::create(FadeOut::create(0.3f),  nullptr);
			pNode->stopAllActions();
			pNode->runAction(seq);
			Layer * pParent = (Layer *)pNode->getParent();
		}
		if (param.empty() == false) {  /*hide 될 리스트 숨기기*/
			vector<string> vtResult;
			KStringUtil::tokenize(param, vtResult);
			for (int i = 0; i < (int)vtResult.size();i++) {
				Node * pTargetNode = getChildByNameForLocal(vtResult.at(i));
				if (pTargetNode == nullptr) {
					log(" hul  .. can't find targetNode..");
				}
				else {
					Sequence * seq = Sequence::create(FadeOut::create(0.3f),  nullptr);
					pTargetNode->stopAllActions();
					pTargetNode->runAction(seq);
				}
			}
		}

		if (param2.empty() == false) {  /*보여져야할 아이템 보이게 하기*/
			vector<string> vtResult;
			KStringUtil::tokenize(param2, vtResult);
			for (int i = 0; i < (int)vtResult.size(); i++) {
				Node * pTargetNode = getChildByNameForLocal(vtResult.at(i));
				if (pTargetNode == nullptr) {
					log(" hul  .. can't find targetNode..");
				}
				else {
					Sequence * seq = Sequence::create( FadeTo::create(0.3f,255), nullptr);
					pTargetNode->stopAllActions();
					pTargetNode->runAction(seq);
					std::string aniname = (std::string) pTargetNode->getName();
					if (std::strncmp(aniname.c_str(), "ani", 3) == 0) {
						ACTION_PARAM* parma = (ACTION_PARAM*)pTargetNode->getUserData();
						pTargetNode->setOpacity(255);
						playAnimation(parma->pContentInfo, parma->target, (Sprite*)pTargetNode,true,true);
					}
				}
			}
		}
		 
	}
	else if (command == COMMAND_POPUP) {  /*팝업 오픈 액션*/
		if (param.empty()) {
			log("POPUP's target is not set.");
			return;
		}
		if (param == "record"){
			return;
		}
		//CocosDenshion::SimpleAudioEngine::getInstance()->pauseBackgroundMusic(); /*배경음원 멈준다.*/
		stopBGM();

		setTextOrg();   /*나레이션 텍스트 초기색으로 복원*/
		int nIndex = getPopupIndexFromTargetID(param);  /*해당 팝업 정보 받아오기*/
		if (nIndex == -1) {
			log("There is no popups with targetID.. check please targetid.");
			return;
		}
		/*팝업 열릴때 작동 멈추기*/
		Vector<Node *>  vecChildren = mContent->getChildren();
		for (int i = 0; i < vecChildren.size(); i++) {
			Node * pNode = vecChildren.at(i);
			std::string aniname = (std::string) pNode->getName();
			if (std::strncmp(aniname.c_str(), "ani", 3) == 0) {
				if (pNode->isVisible())
					pNode->pause();
			}
		}

		/*Jump to page 별도 블락 처리  아래 화면 터치 안되게 하기*/
		if (param == "album"){
			mAlbum->setBlocked(true);
			mAlbum->refreshPosition();
			mAlbum->setJtpPageToCurrentPage(mCurPage);
			refreshPrevNextBtnInJtp();
			mJTPpopupIndex = nIndex;
		}
		mPopup[nIndex]->setVisible(true);
		mIsPopup = true;   /*팝업 활성 제어 변수*/
	}
	else if (command == COMMAND_ALBUMPREV) {    /*Jump to page 이전페이지로 이동*/
		if (mAlbum != nullptr) {
			mAlbum->prev();
			refreshPrevNextBtnInJtp();
		}
	}
	else if (command == COMMAND_ALBUMNEXT) {    /*Jump to page 다음페이지로 이동*/
		if (mAlbum != nullptr) {
			mAlbum->next();
			refreshPrevNextBtnInJtp();
		}
	}
	else if (command == COMMAND_CAMERA) {       /*카메라 작동*/
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
		MessageBox("미리보기에서 작동되지 않습니다.", "확인");
		return;
#endif
		/*이름이 입력 안되어있으면 이름부터 입력 받아야함*/
		ui::TextField * textfield = KCameraManager::getInstance()->getTextFieldObject();
		std::string sCurrentText = textfield->getString();
		if (sCurrentText.empty()) {
			MessageBox(__TX("이름을 입력해 주세요.").c_str(), __TX("확인").c_str());
			return;
		}
		KCameraManager::getInstance()->setDelegate(this);  /*Delegate 설정*/

		/*카메라 테이타 전송 (프로젝트 코드 , 이름)*/
		KJSONDataManager::getInstance()->setCameraData(KXMLReader::getInstance()->getBookInfo()->projectcode, textfield->getString());
		
		/*경로 받아오기*/
		std::string sFilename = KCameraManager::getInstance()->getFullpathnameForPicture();
		/*카메라 작동 요청 JNI*/
		KCameraManager::getInstance()->requestPicture(sFilename);
	} 
	else if (command == COMMAND_RECORD) {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
		MessageBox("미리보기에서 작동되지 않습니다.", "확인");
		return;
#endif
		Scene * pScene = KRecordingLayer::createScene(); /*녹음 구동 KRecordingLayer 처리함*/
		Director::getInstance()->pushScene(pScene);
	}
	else if (command == COMMAND_SOUNDON) {  /*사운드 ON (Jump to page에서 구동시킴)*/
		log("Sound On");
		KDataProvider::getInstance()->setSoundState(true);
	}
	else if (command == COMMAND_SOUNDOFF) {  /*사운드 OFF (Jump to page에서 구동시킴)*/
		log("Sound Off");
		KDataProvider::getInstance()->setSoundState(false);
		stopBGM();
	}
}

/* popup open */
void CPage::showPopup(string param)
{
	if (param.empty()) {
		log("POPUP's target is not set.");
		return;
	}

	int nIndex = getPopupIndexFromTargetID(param);
	if (nIndex == -1) {
		log("There is no popups with targetID.. check please targetid.");
		return;
	}

	Vector<Node *>  vecChildren = mContent->getChildren();
	for (int i = 0; i < vecChildren.size(); i++) {
		Node * pNode = vecChildren.at(i);
		std::string aniname = (std::string) pNode->getName();
		if (std::strncmp(aniname.c_str(), "ani", 3) == 0) {
			if (pNode->isVisible())
				pNode->pause();
		}
	}

	mPopup[nIndex]->setVisible(true);
	mIsPopup = true;
}

/*@id 해당 팝업 포인터 받기*/
int	CPage::getPopupIndexFromTargetID(string id){
	for (int i = 0; i < mPopupLength; i++) {
		if (mPopup[i]->getName() == id) {
			return i;
		}
	}
	return -1;
}


void CPage::playBGM(){
	//CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();  /*배경음원 가동*/
	/*
	bool bSound = KDataProvider::getInstance()->getSoundState(); // 묵음모드인지 판별 (Jump to Page에서 묵음모드 설정가능)
	if (bSound){
		if (m_pPageInfo->backgroundMusic.empty() && m_pPageInfo->backgroundMusic.length() == 0)
		{
			bool bmainbgm = KDataProvider::getInstance()->getMainBgm();
			//MainBgm 이 아니면 중지 시킨다.
			if (bmainbgm == false) stopBGM();

			if (isBgmPlaying() == false)
			{
				stopBGM();
				if (m_pPageInfo->backgroundMusic.empty()) {  ///해당페이지 bgm없으면 프로젝트정보에 있는 bgm적용
					if (!m_pBookInfo->backgroundSound.empty()){
						setBgm(m_pBookInfo->backgroundSound);
						KDataProvider::getInstance()->setMainBgm(true);
					}
				}
				else {
					setBgm(m_pPageInfo->backgroundMusic);    // 해당페이지 자신 bgm이 있으면 플레이
					KDataProvider::getInstance()->setMainBgm(false);
				}
			}
		}
		else // 해당페이지 자신 bgm이 있으면 플레이
		{
			stopBGM();
			setBgm(m_pPageInfo->backgroundMusic);
			KDataProvider::getInstance()->setMainBgm(false);
		}
	}
	*/
	
	bool bSound = KDataProvider::getInstance()->getSoundState(); // 묵음모드인지 판별 (Jump to Page에서 묵음모드 설정가능)
	if (bSound){   
		if (m_pPageInfo->backgroundMusic.empty() && m_pPageInfo->backgroundMusic.length() == 0)
		{
			bool bmainbgm = KDataProvider::getInstance()->getMainBgm();
			//MainBgm 이 아니면 중지 시킨다.
			if (bmainbgm == false) CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();

			if (!CocosDenshion::SimpleAudioEngine::getInstance()->isBackgroundMusicPlaying())
			{
				CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();
				if (m_pPageInfo->backgroundMusic.empty()) {  ///해당페이지 bgm없으면 프로젝트정보에 있는 bgm적용
					if (!m_pBookInfo->backgroundSound.empty()){
						setBgm(m_pBookInfo->backgroundSound);
						KDataProvider::getInstance()->setMainBgm(true);
					}
				}
				else {
					setBgm(m_pPageInfo->backgroundMusic);    // 해당페이지 자신 bgm이 있으면 플레이
					KDataProvider::getInstance()->setMainBgm(false);
				}
			}
		}
		else // 해당페이지 자신 bgm이 있으면 플레이
		{
			CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();
			setBgm(m_pPageInfo->backgroundMusic);
			KDataProvider::getInstance()->setMainBgm(false);
		}
	}
	
}

/*bgm play*/
void CPage::setBgm(string sound = ""){
	if (sound == "") return;
	
	//KDataProvider::getInstance()->mBgmAudioID = playSound(sound, true);
	int a = 0;

	
	CocosDenshion::SimpleAudioEngine::getInstance()->playBackgroundMusic(KCTT(sound).c_str(), true);

}

bool CPage::isBgmPlaying(){

	/*
	if (cocos2d::experimental::AudioEngine::getState(KDataProvider::getInstance()->mBgmAudioID) == cocos2d::experimental::AudioEngine::AudioState::PLAYING){
		return true;
	}
	return false;
	*/
	if (CocosDenshion::SimpleAudioEngine::getInstance()->isBackgroundMusicPlaying()){
		return true;
	}
	return false;
}

void CPage::stopBGM(){

	//if (cocos2d::experimental::AudioEngine::INVALID_AUDIO_ID != KDataProvider::getInstance()->mBgmAudioID){
	//	cocos2d::experimental::AudioEngine::stop(KDataProvider::getInstance()->mBgmAudioID);
	//}
	CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();
}

/*string color 변환*/
Color3B CPage::convertRGB(const char* input)
{
	char* pStop;
	
	int num = strtol(input + 1, &pStop, 16);
	int b1 = (num & 0xFF0000) >> 16;
	int b2 = (num & 0xFF00) >> 8;
	int b3 = num & 0xFF;
	return Color3B(b1, b2, b3);
}

/* 페이지 백그라운드 이미지 있으면 적용*/
void CPage::setBackground(STBACKGROUND &item, Layer* target){
	STBACKGROUND content = item;

	Size visibleSize = Director::getInstance()->getVisibleSize();

	string image = content.image;
	float width = visibleSize.width;
	float height = visibleSize.height;
	float opacity = content.opacity;
	string color = content.color;


	auto* sprite = Sprite::create();

	if (!image.empty()) {
		sprite->initWithFile(KCTV(image));
		auto size = sprite->getContentSize();
		sprite->setScaleX(width / size.width);
		sprite->setScaleY(height / size.height);
	}
	else {

		Color3B col;
		if (color.empty()) col = ccc3(255, 255, 255);
		else col = convertRGB(color.c_str());
		sprite->setTextureRect(Rect(0, 0, width, height));
		sprite->setColor(col);
		sprite->setOpacity(opacity*255.0f);
	}
	sprite->setPosition(Vec2(width / 2, height / 2));
	
	target->addChild(sprite);
}

/*액션 버튼 생성*/
void CPage::appendButton(STCONTENT_INFO * pContentInfo, Layer* target){
	
	string image = pContentInfo->image;
	string select = pContentInfo->selected;
	string disabled = pContentInfo->disabled;

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	//listen mode 일 경우 next와 prev 안보이게 한다.
	string sActionTarget = pContentInfo->action.param;
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
	if (bAuto)  /*듣기(LISTEN) 모드 */
	{
		/*듣기 모드에서는 생성하지 않는다. 자동으로 다음 페이지 이동*/
		if (pContentInfo->action.name == COMMAND_PREV || pContentInfo->action.name == COMMAND_NEXT)
		{
			return;
		}
		if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_POPUP) {
			if (sActionTarget == "album")  /*독자적으로 처리 되어 mSetPopupIDs 관리에 넣어 둔다. */
			{
				mpJtpButtonInfo = pContentInfo;
				mSetPopupIDs.insert(sActionTarget);
				return;
			}
		}
		if (pContentInfo->action.name == COMMAND_EXIT){
			mpExitButtonInfo = pContentInfo;
			return;
		}
	}

	if (pContentInfo->action.name == COMMAND_AUTO_PLAY || pContentInfo->action.name == COMMAND_MANUAL_PLAY)
	{
		//기획 변경으로 막아둠
			mSetPopupIDs.insert("selectedUser");
	}
	else if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_POPUP) {
		mSetPopupIDs.insert(sActionTarget);
	}
	else if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_CAMERA) {
		/*카메라 요청 액션이면 기본정보 구성해 둠*/
		KCameraManager::getInstance()->setActionTargetName(sActionTarget);
		KCameraManager::getInstance()->setActionTargetLayer(target);
		mIsCameraActionOn = true;
	}

	if (!image.empty()) {
		// 버튼을 불러온다.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;
		std::string sPictureName ="";

		if (pContentInfo->action.name == COMMAND_VOICE_USER) /*액션이 사용자 나레이션 사용할 경우*/
		{
			sPictureName = KCameraManager::getInstance()->getFullpathnameForPicture();
			long nSize = FileUtils::getInstance()->getFileSize(sPictureName);
			if (nSize < 11)   sPictureName = KCTV(image);
		}
		else	sPictureName = KCTV(image);

		auto button = Button::create(sPictureName, KCTV(select), KCTV(disabled));
		if (pContentInfo->visible == "false") button->setVisible(false);
		
		ACTION_PARAM* param = new ACTION_PARAM();  /*이벤트 에서 사용될 값 저장해 둔다.*/
		param->autorelease();
		vtRefCountManager.pushBack(param);
		param->name = pContentInfo->action.name;     /*액션이름*/
		param->param = pContentInfo->action.param;   /*파라메타값1*/
		param->param2 = pContentInfo->action.param2; /*파라메타값2*/
		param->pContentInfo = pContentInfo;          /*해당액션 정보 point*/
		button->setUserData((void*)param);           /*주요 정보 저장*/

		CViewUtils::setSize(button, x, y, width, height);  /*지정된 크기에 맞게 조정*/
	
		target->addChild(button);

		
		if (!pContentInfo->id.empty()) {
			button->setName(pContentInfo->id);
		}

		/*버튼 이벤트 발생시 처리*/
		button->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);
			if (mIsPopup) {
				if (button->getParent() == mUIObject) {
					return;
				}
			}

			ACTION_PARAM* parma = (ACTION_PARAM*)button->getUserData();
			
			float scaleX = button->getScaleX();
			float scaleY = button->getScaleY();
			string action = parma->name;
			string param = parma->param;
			string param2 = parma->param2;
			STCONTENT_INFO * pCntInfo = parma->pContentInfo;

			// prevent double touch about command_next or command_prev
			if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV){
				return;
			}

			/*터치 상태에 따라 scale효과 적용*/
			switch (type)
			{
				case Widget::TouchEventType::BEGAN:
					scaleX *= 1.15f;
					scaleY *= 1.15f;
					button->setScaleX(scaleX);
					button->setScaleY(scaleY);
					break;
				case Widget::TouchEventType::CANCELED:
					scaleX /= 1.15f;
					scaleY /= 1.15f;
					button->setScaleX(scaleX);
					button->setScaleY(scaleY);
					break;
				case Widget::TouchEventType::ENDED:
					scaleX /= 1.15f;
					scaleY /= 1.15f;
					button->setScaleX(scaleX);
					button->setScaleY(scaleY);

					if (!pCntInfo->sound.empty())  /*액션에 음원이 있을 경우 음원 플레이후 command작동시킴*/
					{
						if (pCntInfo->sound.length() > 0)
						{
							mpContentInfo = pCntInfo;
							cocos2d::experimental::AudioEngine::stop(mAudioEngineID);
							CocosDenshion::SimpleAudioEngine::getInstance()->stopEffect(mAudioEngineID);
							string sfile = pCntInfo->sound;
							
							mAudioEngineID = playSound(KCTT(sfile).c_str()); // cocos2d::experimental::AudioEngine::play2d(KCTT(sfile).c_str());

							lastCommandOfButton = pCntInfo->action.name;

							cocos2d::experimental::AudioEngine::setFinishCallback(mAudioEngineID, [&](int audioID, const std::string & fileName) {
								mAudioEngineID = AUDIOENGINE_UNDEFINED;
								Node* pNode = getChildByNameForLocal(mpContentInfo->id);
								command(mpContentInfo->action.name, mpContentInfo->action.param, mpContentInfo->action.param2, pNode);
							});
						}
						else command(action, param, param2, sender);
					}
					else command(action, param, param2, sender);
					
					
					break;
				default:
					break;
			}
		});
	}
}

/*애니메이션 생성 시작*/
void CPage::appendAnimation(STCONTENT_INFO* pContentInfo, Layer* target,bool bplay) {
	/*기본 이미지로 스프라이트를 구성한다.*/
	Sprite* sprite = appendImage(pContentInfo, target);
	sprite->setName(pContentInfo->id);

	/*normaltype == true 이면 이벤트를 받지 않고 애니메이션 동작만 한다.*/
	if (!(pContentInfo->normaltype.empty() || pContentInfo->normaltype.length() == 0))
	{
		if (pContentInfo->normaltype == "False")
		{
			appendAnimationEvent(pContentInfo, target, sprite); /* click 1times play */
		}
	}
	else
	{
			appendAnimationEvent(pContentInfo, target, sprite); /* click 1times play */
	}
	playAnimation(pContentInfo, target, sprite,bplay);
}

/*애니메이션 터치시 작동 */
void CPage::appendAnimationEvent(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	/*이벤트 작동시 필요한 정보 저장해 둠*/
	ACTION_PARAM* param = new ACTION_PARAM();
	param->autorelease();
	param->name = pContentInfo->action.name;
	vtRefCountManager.pushBack(param);
	param->pContentInfo = pContentInfo;
	param->target = target;
	param->param = pContentInfo->action.param;
	param->param2 = pContentInfo->action.param2;

	sprite->setUserData((void*)param);

	/*터치시 이벤트 처리*/
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(true);
	listener1->onTouchBegan = [&](Touch * touch, Event * event) {
		auto target1 = static_cast<Sprite*>(event->getCurrentTarget());
		if (mIsPopup) {  /*팝업이 활성화 되었으면 무시하고 리턴한다.*/
			if (target1->getParent() == mUIObject) {
				return false;
			}
		}

		if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV)return false;

		Point locationInNode = target1->convertToNodeSpace(touch->getLocation());
		Size s = target1->getContentSize();
		Rect rect = Rect(0, 0, s.width, s.height);

		if (rect.containsPoint(locationInNode))  /*애니메이션 영역에 들어 왔는가?*/
		{
			ACTION_PARAM* parma = (ACTION_PARAM*)target1->getUserData();
			string action = parma->name;
			string param = parma->param;
			string param2 = parma->param2;

			/* 연구가 필요.
			if (isSpriteTransparentInPoint(target1, locationInNode) == true)
			{
				if (!(parma->name.empty() || parma->name.length() == 0))
				{
					command(action, param, param2, target1);
				}
				log("isSpriteTransparentInPoint(false)= %s", "false");
				return false;
			}
			*/

			/* ani click.... action isExist true */
			if (!(parma->name.empty() || parma->name.length() == 0))
			{
				command(action, param, param2, target1);
			}
			else /* ?회 애니후 멈춤 */
			{
				target1->stopAllActions();
				/*애니메이션 실제 작동*/
				playAnimation(parma->pContentInfo, parma->target, target1);
			}
			return true;
		}
		return false;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener1, sprite);
}

/*연구 필요 현재 사용안 함*/
bool CPage::isSpriteTransparentInPoint(Sprite* sprite, Point& location) {

	return HitDetectHelper::hitTest(sprite, location);

}
void CPage::stopPrevAnimation()
{
	// B 애니를 플레이시키려고 할때 다른 애니A가 동작중이면 A를 멈추게 한다. 
	if (lastAniSpr != nullptr){
		if (lastAniSpr->numberOfRunningActions() > 0){
			lastAniSpr->stopAllActions();
		}
	}
}


/*애니메이션 구동*/
void CPage::playAnimation(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite, bool bplay, bool bforceshow) {

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;
	int length = pContentInfo->vtAnimation.size();
	Vector<SpriteFrame*> animFrames;

	// B 애니를 플레이시키려고 할때 다른 애니A가 동작중이면 A를 멈추게 한다. 
	stopPrevAnimation();

	sprite->stopAllActions();
	lastAniSpr = sprite;

	/*프레임 이미지 구성*/
	for (int i = 0; i < length; i++)
	{
		Sprite * pAnimSprite = Sprite::create(KCTV(pContentInfo->vtAnimation.at(i)->image));
		Size sizeAni = pAnimSprite->getContentSize();
		if (sizeAni.width == 0 || sizeAni.height == 0) continue;
		CViewUtils::setSize(pAnimSprite, x, y, width, height);
		animFrames.pushBack(pAnimSprite->getSpriteFrame());
	}
	/*프레임당 전환시간 설정*/
	float fDelay = atof(pContentInfo->delay.c_str()) * 0.001;
	int   nLoop = 1;
	
	if (pContentInfo->aniloop.empty() == false)  /*무한루프가 아니면 지정된 횟수만큼*/
	{
		string aniloop = pContentInfo->aniloop.substr(0, 1);
		nLoop = atoi(aniloop.c_str());
		if (nLoop == 0) nLoop = -1;
	}
	
	/*애니매이션 생성*/
	
	auto animation = Animation::createWithSpriteFrames(animFrames, fDelay, nLoop); //-1
	auto animate = Animate::create(animation);
	pContentInfo->image = pContentInfo->vtAnimation.at(0)->image;
	CViewUtils::setSize(sprite, x, y, width, height);
	sprite->runAction(animate);
	
	float fTime = atof(pContentInfo->time.c_str()) * 0.001f; /*이동시간 설정*/
	float dx = atof(pContentInfo->dx.c_str());   /* x축 방향 이동 거리*/
	float dy = atof(pContentInfo->dy.c_str());   /* y축 방향 이동 거리*/
	auto moveby = MoveBy::create(fTime, Vec2(dx, dy));  /*움직임 객체 생성*/
	auto reverse = moveby->reverse();                   
	auto ease1 = EaseInOut::create(moveby->clone(), 3.0f);
	auto east2 = EaseInOut::create(reverse->clone(), 3.0f);
	auto seqMove = Sequence::create(ease1, east2, nullptr);  /*왔다갔다 지정*/
	/* 보이게 할 건지 가릴 건지 판단*/
	if (bforceshow) sprite->setVisible(true);
	else{
		if (pContentInfo->visible == "false" || pContentInfo->visible == "False") sprite->setVisible(false);
		else sprite->setVisible(true);
	}
	/*애니메이션의 여러 동작을 담아 두고 동시메 구동한다.*/
	cocos2d::Vector<FiniteTimeAction *> actions;
	actions.pushBack(seqMove);

	/* Rotate 추가 20160403 */
	float fRotateDu = -1;
	float fRotateAn = -1;

	if (pContentInfo->rotate.empty() == false)  /*회전하기 구동 설정*/
	{
		if (pContentInfo->rotate == "True")
		{
			sprite->setRotation(0);
			fRotateDu = atof(pContentInfo->rotatedu.c_str()) * 1.0f / 1000.0f;
			fRotateAn = atof(pContentInfo->rotatean.c_str()) * 1.0f;

			auto rotate = RotateBy::create(fRotateDu, fRotateAn);
			auto rotateOrg = rotate->reverse();
			auto rotateAll = Sequence::create(rotate, rotateOrg, NULL);
			if (pContentInfo->rotateloop == "True")
			{
				RepeatForever * rerotate = RepeatForever::create(rotateAll);;
				sprite->runAction(rerotate);
			}
			else actions.pushBack(rotateAll);
		}
	}


	/* Scale 추가 20160403 */
	float fScaleDu = -1;
	float fScaleSS = -1;
	if (pContentInfo->scale.empty() == false)  /*크기애니적용 설정*/
	{
		if (pContentInfo->scale == "True")
		{
			sprite->setScale(1.0f);
			fScaleDu = atof(pContentInfo->scaledu.c_str()) * 1.0f / 1000.0f;
			fScaleSS = atof(pContentInfo->scaless.c_str()) * 1.0f;

			auto ScaleUp = ScaleBy::create(fScaleDu, fScaleSS);
			auto ScaleDown = ScaleUp->reverse();
			auto upDown = Sequence::create(ScaleUp, ScaleDown, NULL);
			if (pContentInfo->scaleloop == "True")
			{
				RepeatForever * rescale = RepeatForever::create(upDown);;
				sprite->runAction(rescale);
			}
			else actions.pushBack(upDown);
		}
	}

	auto allAni = Spawn::create(actions);
	
	if (nLoop == -1)  /*무한 반복*/
	{
		RepeatForever * repeat;
		repeat = RepeatForever::create(allAni);
		sprite->runAction(repeat);
	}
	else              /*지정된 횟수 만큼*/
	{
		Repeat * repeat1;
		repeat1 = Repeat::create(allAni, nLoop);
		sprite->runAction(repeat1);
	}
	/*작동을 멈추어야 할 때*/
	if (!bplay) sprite->stopAllActions();
}

/*이미지 아이템 생성*/
Sprite* CPage::appendImage(STCONTENT_INFO* pContentInfo, Layer* target) {

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;
	
	Sprite* sprite = Sprite::create(KCTV(pContentInfo->image));
	if (sprite == nullptr) {
		log("Error::: from K. check the pass first [%s]", KCTV(pContentInfo->image).c_str() );
		return nullptr;
	}
	if (!pContentInfo->alpha.empty()) {
		sprite->setOpacity(atoi(pContentInfo->alpha.c_str()));
	}
	target->addChild(sprite);

	if (!pContentInfo->id.empty()) {
		sprite->setName(pContentInfo->id);
	}
	CViewUtils::setSize(sprite, x, y, width, height);
	if (pContentInfo->visible == "false") sprite->setVisible(false);


	return sprite;
}

/*터치시 사운드 이벤트가 동작해야 하는 경우*/
/*처음 의도 터치시 텍스트 칼라 변경과 사운드 플레이용으로 만들어짐 이후로 애니메이션 사운트 터시 사용함*/
void CPage::addSoundEvent(STCONTENT_INFO* pContentInfo, Layer* target) {

	myPage = this;
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	if (pContentInfo->sound.empty() == true) return;
	if (pContentInfo->sound.length() == 0) return;

	auto sprite = Sprite::create();
	sprite->setTextureRect(Rect(0, 0, width, height));
	Color3B red = Color3B::RED;
	sprite->setColor(red);

	sprite->setOpacity(0); // 영역만 가지고 보이게 하지는 않는다.

	/* 터치시 구동 정보 저장*/
	ACTION_PARAM* param = new ACTION_PARAM();
	param->autorelease();
	param->name = "thisistext";
	vtRefCountManager.pushBack(param);
	param->target = target;
	param->param = pContentInfo->id;
	param->param2 = pContentInfo->sound;
	param->pContentInfo = pContentInfo;
	sprite->setUserData((void*)param);

	target->addChild(sprite);

	CViewUtils::setSize(sprite, x, y, width, height);

	/*터치 이벤트 설정*/
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(true);
	listener1->onTouchBegan = [&](Touch * touch, Event * event) {

		if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV)return false;

		// 사운드가 플레이중이면 터치인식 안하게 한다. 
		//if (mAudioEngineID != AUDIOENGINE_UNDEFINED)return true;

		auto target1 = static_cast<Sprite*>(event->getCurrentTarget());

		Point locationInNode = target1->convertToNodeSpace(touch->getLocation());
		Size s = target1->getContentSize();
		Rect rect = Rect(0, 0, s.width, s.height);
		if (rect.containsPoint(locationInNode))
		{
			ACTION_PARAM* parma = (ACTION_PARAM*)target1->getUserData();

			if (!(parma->param2.empty() || parma->param2.length() == 0))
			{
				/*아이디에 해당하는 sprite가져오기*/
				Sprite* pSprite = (Sprite*)getChildByNameForLocal(parma->pContentInfo->id);
				if (pSprite){ /*해당 객체가 가려져 있으면 무시한다.*/
					if (pSprite->getOpacity()==0 || 
						pSprite->isVisible() == false) return true;
				}
				
				setTextOrg(); /*텍스트 초기색으로 복원한다.*/
				cocos2d::experimental::AudioEngine::stop(mAudioEngineID); //playing sound stop
				CocosDenshion::SimpleAudioEngine::getInstance()->stopEffect(mAudioEngineID);
				string sfile = parma->param2;
				/*사운드 플레이한다.*/
				log("playSound, file:%s", sfile.c_str());
				mAudioEngineID = playSound(KCTT(sfile));

				/*텍스트 영역 터치시 색 바꾼다.*/
				if (std::strncmp(parma->param.c_str(), "text", 4) == 0) {
					// 인터렉션간 간섭을 막기위해 텍스트 터치시 애니를 스톱시킨다.
					myPage->stopPrevAnimation();
					searchObject(parma->param, parma->pContentInfo->playingcolor);
				}
				/*애니메이션 터치 사운드 플레이 하고 동작하게 보낸다.*/
				if (std::strncmp(parma->param.c_str(), "ani", 3) == 0) {
					
					if (pSprite){
						// 다른 클릭시 기존 애니객체를 없애거나 안보이게 하고 싶으나 animation을 팝업처럼 쓰는 현재 방식으로는 안됨.
						// 정상적으로 남아있어야할 애니들도 사라져버리는 문제가 생김.
						//clearAllSoundAniEventObj(); 
						//prevSoundAniSpr = pSprite;
						playAnimation(parma->pContentInfo, parma->target, pSprite,true,true);
						/* ani click.... action isExist true */
						if (!(parma->pContentInfo->action.name.empty() || parma->pContentInfo->action.name.length() == 0))
						{
							command(parma->pContentInfo->action.name, 
								parma->pContentInfo->action.param,
								parma->pContentInfo->action.param2, pSprite);
						}
						
					}
				}
				/*사운드 완료 되면 텍스트 초기값으로 원복한다.*/
				cocos2d::experimental::AudioEngine::setFinishCallback(mAudioEngineID, [&](int audioID, const std::string & fileName) {
					mAudioEngineID = AUDIOENGINE_UNDEFINED;
					setTextOrg();
				});
			}
			return true;
		}
		return false;
	};

	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener1, sprite);
}


void CPage::clearAllSoundAniEventObj(){
	if (prevSoundAniSpr != nullptr){
		prevSoundAniSpr->stopAllActions();
		//prevSoundAniSpr->removeAllChildren();
		prevSoundAniSpr->setOpacity(0);
	}
}


/*텍스트 아이템을 생성한다.*/
void CPage::appendText(STCONTENT_INFO* pContentInfo, Layer* target) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	int nFontSize = atoi(pContentInfo->fontsize.c_str()) + 10; /*window보이는 크기보다 작게 보여 +10 해줌*/
	if (nFontSize == 0) nFontSize = 10;
	std::string sResult = "";
	std::string sFontFile = "";

	if (pContentInfo->id == "username") /*고정아이디 : username이면 저장된이름 받아오기*/
	{
		sResult = KJSONDataManager::getInstance()->getCameraData(KXMLReader::getInstance()->getBookInfo()->projectcode);
	}
	else sResult = pContentInfo->text;

	/*폰트 정보 지정 기본 : 나눔고딕*/
	if (pContentInfo->fontfile.empty()) sFontFile = "fonts/NanumGothic.ttf";
	else sFontFile = pContentInfo->fontfile;
		
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	sFontFile = pContentInfo->fontname;       /*window용은 폰트이름으로 넘긴다.*/
	if (sFontFile.empty())
	{
		if(sFontFile == "fonts/NanumPen.ttf")	sFontFile = "나눔손글씨 펜";
		else if (sFontFile == "fonts/NanumGothic.ttf")	sFontFile = "나눔고딕";
		else sFontFile = "나눔고딕";
	}
	
#endif

	Label* label = Label::createWithSystemFont(sResult, sFontFile, nFontSize
				, Size((int)width , (int)height), TextHAlignment::LEFT, TextVAlignment::TOP);
    
	Color3B color = convertRGB(pContentInfo->fontcolor.c_str());
	label->setTextColor(Color4B(color));

	if (pContentInfo->outline.empty() == false)
	{
		if (pContentInfo->outline == "True")  /*outline이 있으면 외각에 그린다.*/
		{
			int nTick = 5;
			if (pContentInfo->outlinetick.empty() == false) nTick = atoi(pContentInfo->outlinetick.c_str());
			Color3B coloroutline = convertRGB(pContentInfo->outlinecolor.c_str());
			label->enableOutline(Color4B(coloroutline), nTick);
		}
	}

	target->addChild(label);

	if (!pContentInfo->id.empty()) {
		label->setName(pContentInfo->id);
	}

	CViewUtils::setSize(label, x, y, width, height);
	
	if (pContentInfo->visible == "false") label->setVisible(false);
}

/*textinput 객체를 생성한다. 카메라 이름 받는 곳에서 요청된다.*/
void CPage::appendTextInput(STCONTENT_INFO* pContentInfo, Layer* target) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	
	int nFontSize = atoi(pContentInfo->fontsize.c_str()) + 10;
	if (nFontSize == 0) nFontSize = 10;

	auto textfield = ui::TextField::create(pContentInfo->text, "나눔고딕", nFontSize);
	Color3B color = convertRGB(pContentInfo->fontcolor.c_str());
	textfield->setTextColor(Color4B(color));
	target->addChild(textfield);

	KCameraManager::getInstance()->setTextFieldObject(textfield);

	if (!pContentInfo->id.empty()) {
		textfield->setName(pContentInfo->id);
	}

	CViewUtils::setSize(textfield, x, y, width, height);

	if (pContentInfo->visible == "false") textfield->setVisible(false);
}

/*Jump to Page를 생성한다.*/
void CPage::appendAlbum(HPOPUP_INFO* pPopupInfo, Layer* target) {

	if (mAlbum == nullptr) {     /*팝업을 생성한다.*/
		Size size = Size(54, 54);
		mAlbum = KAlbumLayer::create(pPopupInfo->albumthumbsrc, pPopupInfo->albumthumbsrc2, size, atoi(pPopupInfo->albumthumby.c_str()),  target, mCurPage);
	}
	else {
		return;
	}
	
	target->addChild(mAlbum);

	if (!pPopupInfo->id.empty()) {
		mAlbum->setName(pPopupInfo->id);
	}
	std::string isFindshed = KDataProvider::getInstance()->getOneFinished();	/*완독여부*/

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	// for test
	//isFindshed = "n";
	// for test
#endif

	
	log("appendAlbum is Finished  %s", isFindshed.c_str());
	int nProgressPage = KDataProvider::getInstance()->getProcessPage();			// 진행중 페이지
	log("appendAlbum getProcessPage Finished  %d", nProgressPage);
	int nPageCount = KXMLReader::getInstance()->getPageCount();					// 전체 페이지 수
	for (int i = 0; i < nPageCount; i++) {
		HPAGE_INFO * pPageInfo = KXMLReader::getInstance()->getPageReference(i);  // xml에서 읽은 팝업정보
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		if (isFindshed == "n"){  // 정책 완독전에는 첫 페이지만 선택 가능함 
			if (i != 0){
				mAlbum->addContent(pPageInfo->thumbnailImage, true);
				//log("Dim page %d", i);
				continue;
			}
		}
#endif
		//mAlbum->addContent(pPageInfo->thumbnailImage);
		mAlbum->addContent(pPageInfo->thumbnailImage, true);
	}

	 // 제어 버튼  배치 한다
	Size visibleSize = Director::getInstance()->getVisibleSize();
	int nWidth = visibleSize.width;

	mAlbum->setParentPage(this);

	/*
	mAlbum->appendButtonAlbum(nWidth - 15 - 93, 18, 93, 93, "syscommon/common_btn_close_n.png",
		"syscommon/common_btn_close_p.png",
		"syscommon/common_btn_close_n.png", "POPUPEXIT");// , target);

	mAlbum->appendButtonAlbum(15, 17, 93, 93, "syscommon/common_btn_sound_off_n.png",
		"syscommon/common_btn_sound_off_p.png",
		"syscommon/common_btn_sound_off_n.png", "SOUNDON");// , target);

	mAlbum->appendButtonAlbum(15, 17, 93, 93, "syscommon/common_btn_sound_on_n.png",
		"syscommon/common_btn_sound_on_p.png",
		"syscommon/common_btn_sound_on_n.png", "SOUNDOFF");// , target);
		*/

	// JTP 좌우스크롤은 JTP layer에, 사운드 및 exit 버튼은 popup layer에 추가시킨다. 
	if (nPageCount > 9){
		mAlbum->appendButtonAlbum(15, 17 + 98 + 243, 96, 110, "syscommon/btn_page_prev_n.png",
			"syscommon/btn_page_prev_p.png",
			"syscommon/btn_page_prev_n.png", "ALBUMPREV");// , target);

		mAlbum->appendButtonAlbum(nWidth - 15 - 96, 17 + 98 + 243, 96, 110, "syscommon/btn_page_next_n.png",
			"syscommon/btn_page_next_p.png",
			"syscommon/btn_page_next_n.png", "ALBUMNEXT");// , target);
	}
	
	
	
	
	appendButtonAlbum(nWidth - 15 - 93, 18, 93, 93, "syscommon/common_btn_close_n.png",
		"syscommon/common_btn_close_p.png",
		"syscommon/common_btn_close_n.png", "POPUPEXIT", target);

	appendButtonAlbum(15, 17, 93, 93, "syscommon/common_btn_sound_off_n.png",
		"syscommon/common_btn_sound_off_p.png",
		"syscommon/common_btn_sound_off_n.png", "SOUNDON", target);

	appendButtonAlbum(15, 17, 93, 93, "syscommon/common_btn_sound_on_n.png",
		"syscommon/common_btn_sound_on_p.png",
		"syscommon/common_btn_sound_on_n.png", "SOUNDOFF", target);
	/*
	if (nPageCount > 9){
		appendButtonAlbum(15, 17 + 98 + 243, 96, 110, "syscommon/btn_page_prev_n.png",
			"syscommon/btn_page_prev_p.png",
			"syscommon/btn_page_prev_n.png", "ALBUMPREV", target);

		appendButtonAlbum(nWidth - 15 - 96, 17 + 98 + 243, 96, 110, "syscommon/btn_page_next_n.png",
			"syscommon/btn_page_next_p.png",
			"syscommon/btn_page_next_n.png", "ALBUMNEXT", target);
	}
	*/

	bool bSound = KDataProvider::getInstance()->getSoundState();
	if (!bSound)
	{
		mSoundOn->setVisible(true);
		mSoundOff->setVisible(false);
	}


}





/*파티클 생성한다.*/
void CPage::appendParticle(STCONTENT_INFO* pContentInfo, Layer* target){

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	auto sprite = Sprite::create();
	sprite->setTextureRect(Rect(0, 0, width, height));
	Color3B red = Color3B::RED;
	sprite->setColor(red);

	sprite->setOpacity(0);
	target->addChild(sprite);

	/*파티클 영역 터치시 필요정보 저장*/
	ACTION_PARAM* param = new ACTION_PARAM();
	param->autorelease();
	vtRefCountManager.pushBack(param);
	param->name = "thisisparticle";
	param->param = pContentInfo->ptype;   /*파티클 타입*/
	param->param2 = pContentInfo->sound;  /*터치시 사운드파일*/
	sprite->setUserData((void*)param);

	CViewUtils::setSize(sprite, x, y, width, height);

	if (!pContentInfo->id.empty()) {
		sprite->setName(pContentInfo->id);
	}
	/*터치 이벤트 생성*/
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(false);
	listener1->onTouchBegan = [&](Touch * touch, Event * event) {
		auto target = static_cast<Sprite*>(event->getCurrentTarget());
		if (mIsPopup) {
			if (target->getParent() == mUIObject) {
				return false;
			}
		}

		if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV)return false;

		Point locationInNode = target->convertToNodeSpace(touch->getLocation());
		Size s = target->getContentSize();
		Rect rect = Rect(0, 0, s.width, s.height);
		if (rect.containsPoint(locationInNode))
		{
			ACTION_PARAM* parma = (ACTION_PARAM*)target->getUserData();
			if (!(parma->param2.empty() || parma->param2.length() == 0)) /*사운드가 있으면 효과음 플레이*/
			{
				CocosDenshion::SimpleAudioEngine::getInstance()->playEffect(KCTT(parma->param2).c_str());
			}
			/*파티클 효과 발생시키기*/
			generateParticleEffect(parma->param, Point(touch->getLocation().x, touch->getLocation().y), 0.3f);
			return true;
		}
		return false;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener1, sprite);
	if (pContentInfo->visible == "false") sprite->setVisible(false);
}

/*아이템 아이에 해당하는 Node 리턴*/
Node * CPage::getChildByNameForLocal(const string&  sName){
	Node * pReturn = nullptr;
	if (mBackground->getChildByName(sName) != nullptr)		return mBackground->getChildByName(sName);
	if (mContent->getChildByName(sName) != nullptr)			return mContent->getChildByName(sName);
	if (mUIObject->getChildByName(sName) != nullptr)		return mUIObject->getChildByName(sName);
	return nullptr;
}

bool isItNumber(char pInput) {
	if (pInput >= '0' && pInput <= '9') return true;
	return false;
}

/*파티클 효과 발생*/
void CPage::generateParticleEffect(string stype, Point pt, float duration ){
	
	string firststype = "";
	if (stype.empty() || stype.length() <2) firststype = "00";
	else {
		if (isItNumber(*stype.substr(0, 1).c_str()) && isItNumber(*stype.substr(1, 1).c_str()) ){
			firststype = stype.substr(0, 2);
		}
		else firststype = stype;
	}
		
	/*제너레이터에 지징된 번호에 밎게 구동*/
	ParticleSystemQuad * emitter = nullptr;
	if (firststype == "00")  emitter = ParticleFire::create();
	else if (firststype == "01")  emitter = ParticleSun::create();
	else if (firststype == "02")  emitter = ParticleGalaxy::create();
	else if (firststype == "03")  emitter = ParticleFlower::create();
	else if (firststype == "04")  emitter = ParticleMeteor::create();
	else if (firststype == "05")  emitter = ParticleSpiral::create();
	else if (firststype == "06")  emitter = ParticleExplosion::create();
	else if (firststype == "07")  emitter = ParticleSmoke::create();
	else if (firststype.size() >2) emitter = ParticleSystemQuad::create(firststype);
	else
			emitter = ParticleFire::create();

	emitter->setPosition(pt);
	emitter->setDuration(duration);
	emitter->setAutoRemoveOnFinish(true);
	addChild(emitter);
}


/*찍은 사진 그리기*/
void CPage::afterSeconds(float dt) {

	std::string filename = mCameraFilename;
	Director::getInstance()->getTextureCache()->removeTextureForKey(filename);

	Sprite * pSprite = KCameraManager::getInstance()->getSprite();
	pSprite->initWithFile(filename);

	cocos2d::Size size = pSprite->getContentSize();

	Node * pTargetNode = KCameraManager::getInstance()->getActionTargetNode();
	if (pTargetNode == nullptr) {
		log("can't find the right target node.. check plz..");
		return;
	}

	cocos2d::Size targetSize = pTargetNode->getContentSize();
	float scaleX = targetSize.width / size.width;
	float scaleY = targetSize.height / size.height;
	float scaleTotal = scaleX > scaleY ? scaleY : scaleX;
	pSprite->setScale(scaleTotal);
	KCameraManager::getInstance()->getActionTargetLayer()->addChild(pSprite);
	pSprite->setPosition(pTargetNode->getPosition());
}

/*사직찍고 refresh*/
void CPage::onRefreshUI(std::string filename) {
	mCameraFilename = filename;
	scheduleOnce(schedule_selector(CPage::afterSeconds), 0.5f);
}
int CPage::getAudioEnginID()
{
	return mAudioEngineID;
}

void CPage::setAudioEnginID(int _id)
{
	mAudioEngineID = _id;
}

/*Jump to page에 사용되는 버튼 생성*/
void CPage::appendButtonAlbum(float x1,float y1,float width1,float height1,
	std::string orgimg, std::string selimg, std::string disimg, std::string action, Layer* target){

	string image = orgimg;
	string select = selimg;
	string disabled = disimg;

	float x = x1;
	float y = y1;
	float width = width1;
	float height = height1;


	if (!image.empty()) {
		// 버튼을 불러온다.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;

		auto button = Button::create(image, select, disabled);

		if (action == "SOUNDON"){   
			mSoundOn = button;
			button->setVisible(false);
		}
		if (action == "SOUNDOFF") mSoundOff = button;
		if (action == "ALBUMPREV") mBtnPrevPageInJtp = button;
		if (action == "ALBUMNEXT") mBtnNextPageInJtp = button;

		ACTION_PARAM* param = new ACTION_PARAM();
		vtRefCountManager.pushBack(param);
		param->autorelease();
		param->name = action;
		param->param = "";
		param->param2 = "";
		param->pContentInfo = nullptr;
		button->setUserData((void*)param);

		CViewUtils::setSize(button, x, y, width, height);

		target->addChild(button);

		button->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);

			ACTION_PARAM* parma = (ACTION_PARAM*)button->getUserData();

			
			float scaleX = button->getScaleX();
			float scaleY = button->getScaleY();
			string action = parma->name;
			string param = "syscommon/Button.mp3";
			//string param2 = parma->param2;

			switch (type)
			{
			case Widget::TouchEventType::BEGAN:
				break;
			case Widget::TouchEventType::CANCELED:
				break;
			case Widget::TouchEventType::ENDED:

				cocos2d::experimental::AudioEngine::stop(mAudioEngineID);
				CocosDenshion::SimpleAudioEngine::getInstance()->stopEffect(mAudioEngineID);

				if (mSoundOn == button)  /*묵음버튼 온 오프*/
				{
					// off -> on
					button->setVisible(false);
					mSoundOff->setVisible(true);
					mAudioEngineID = cocos2d::experimental::AudioEngine::play2d("common/snd/common_sfx_btn_01.mp3");
				}
				else if (mSoundOff == button)
				{
					// on -> off
					button->setVisible(false);
					mSoundOn->setVisible(true);
					mAudioEngineID = cocos2d::experimental::AudioEngine::play2d("syscommon/Button.mp3");
				}
				else{
					/*효과음이 있으면 플레이*/
					mAudioEngineID = playSound(param);// cocos2d::experimental::AudioEngine::play2d(param);
				}

				/*효과음이 있으면 플레이*/
				//mAudioEngineID = playSound(param);// cocos2d::experimental::AudioEngine::play2d(param);
				// sound on/off에 상관없이 효과음을 play 한다. 160830 kyh
				//mAudioEngineID = cocos2d::experimental::AudioEngine::play2d(param);
				/*해당 액션 실행*/
				command(action, "", "", sender);

				break;
			default:
				break;
			}
		});
	}
}

/*듣기(listen)모드 일 때 pause모드 화면 구성*/
void CPage::appendBlankSprite()
{
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	/*백그라운드 생성*/
	mBlankLayers = Layer::create();
	if (mpJtpButtonInfo != nullptr){
		appendButtonToBlankSprite(mpJtpButtonInfo);  /*Jump to Page 버튼 생성*/
		appendButtonToBlankSprite(mpExitButtonInfo); // exit button 
	}

	m_pBlockingSprite = Sprite::create();
	m_pBlockingSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	m_pBlockingSprite->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	m_pBlockingSprite->setColor(Color3B(0, 0, 0));
	m_pBlockingSprite->setOpacity(0);
	mBlankLayers->addChild(m_pBlockingSprite);

	
	/*가운데 플레이버튼 생성*/
	mBtn = ui::Button::create("syscommon/viewer_btn_play_n.png",
		"syscommon/viewer_btn_play_p.png",
		"syscommon/viewer_btn_play_n.png");
	CViewUtils::setSize(mBtn, visibleSize.width / 2 - 95, visibleSize.height / 2 - 95, 190, 190);
	mBlankLayers->addChild(mBtn);
	mBtn->setVisible(false);
	mBtn->addClickEventListener([&](cocos2d::Ref* pSender) {
		/*플레이 버튼 클릭시 처리 pause화면 가리기*/
		mBtn->setVisible(false);
		m_pBlockingSprite->setOpacity(0);

		//CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
		
		playBGM();

		/*나레이션 구동시작*/
		if (mAudioEngineID != AUDIOENGINE_UNDEFINED){
			cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
		}
		else{
			if (mPlayNarrationPos != 0) mPlayNarrationPos--;
			playNarrationList();
		}
		
	});

	/*아래 화면 이벤트 안받게 처리 및 백판 알파 적용*/
	this->addChild(mBlankLayers);
	pEventListenerBack = EventListenerTouchOneByOne::create();
	pEventListenerBack->setSwallowTouches(true);
	pEventListenerBack->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		if (mIsPopup) return true;
		mBtn->setVisible(true);
		m_pBlockingSprite->setOpacity(100);

		CocosDenshion::SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
		cocos2d::experimental::AudioEngine::pauseAll();

		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListenerBack, mBlankLayers);

}



/*Jump to page 생성*/
/*layer가 듣기 모드일 경우 blanksprite가 위에 있어서 blanksprite위에 배치한다.*/
void CPage::appendButtonToBlankSprite(STCONTENT_INFO * pContentInfo){

	string image = pContentInfo->image;
	string select = pContentInfo->selected;
	string disabled = pContentInfo->disabled;

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;


	string sActionTarget = pContentInfo->action.param;
	

	if (!image.empty()) {
		// 버튼을 불러온다.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;
		std::string sPictureName = "";

		sPictureName = KCTV(image);

		auto button = Button::create(sPictureName, KCTV(select), KCTV(disabled));
		if (pContentInfo->visible == "false") button->setVisible(false);

		ACTION_PARAM* param = new ACTION_PARAM();
		param->autorelease();
		vtRefCountManager.pushBack(param);
		param->name = pContentInfo->action.name;
		param->param = pContentInfo->action.param;
		param->param2 = pContentInfo->action.param2;
		param->pContentInfo = pContentInfo;
		button->setUserData((void*)param);

		CViewUtils::setSize(button, x, y, width, height);


		mBlankLayers->addChild(button);


		if (!pContentInfo->id.empty()) {
			button->setName(pContentInfo->id);
		}

		button->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);
			if (mIsPopup) {
				if (button->getParent() == mUIObject) {
					return;
				}
			}
			if (mBtn->isVisible()) return;

			ACTION_PARAM* parma = (ACTION_PARAM*)button->getUserData();

			float scaleX = button->getScaleX();
			float scaleY = button->getScaleY();
			string action = parma->name;
			string param = parma->param;
			string param2 = parma->param2;
			STCONTENT_INFO * pCntInfo = parma->pContentInfo;


			switch (type)
			{
			case Widget::TouchEventType::BEGAN:
				scaleX *= 1.15f;
				scaleY *= 1.15f;
				button->setScaleX(scaleX);
				button->setScaleY(scaleY);
				break;
			case Widget::TouchEventType::CANCELED:
				scaleX /= 1.15f;
				scaleY /= 1.15f;
				button->setScaleX(scaleX);
				button->setScaleY(scaleY);
				break;
			case Widget::TouchEventType::ENDED:
				scaleX /= 1.15f;
				scaleY /= 1.15f;
				button->setScaleX(scaleX);
				button->setScaleY(scaleY);

				pEventListenerBack->setSwallowTouches(false);
				m_pBlockingSprite->setVisible(false);
				if (!pCntInfo->sound.empty())
				{
					if (pCntInfo->sound.length() > 0)
					{
						mpContentInfo = pCntInfo;
						
						cocos2d::experimental::AudioEngine::pauseAll();
						string sfile = pCntInfo->sound;
						playSound(KCTT(sfile).c_str()); //cocos2d::experimental::AudioEngine::play2d(KCTT(sfile).c_str());
						command(pCntInfo->action.name, pCntInfo->action.param, pCntInfo->action.param2, button);
					}
					else command(action, param, param2, sender);
				}
				else command(action, param, param2, sender);

				mBlankLayers->setVisible(false);
				
				break;
			default:
				break;
			}
		});
	}
}



/*사용 안함*/

bool CPage::onTouchBegan(Touch *touch, Event *event) {
	/*
	mCurrentTouchPos = mInitialTouchPos = touch->getLocation();
	mIsTouchDown = true;
	*/
	return true;
}

void CPage::onTouchMoved(Touch *touch, Event *event){
	/*
	mCurrentTouchPos = touch->getLocation();
	*/
}

void CPage::onTouchEnded(Touch *touch, Event *event)
{
	/*
	mIsTouchDown = false;
	*/
}

void CPage::onTouchCancelled(Touch *touch, Event *event)
{
	/*
	onTouchEnded(touch, event);
	*/
}

void CPage::update(float dt)
{
	/*
	if (true == mIsTouchDown)
	{
	if (mInitialTouchPos.x - mCurrentTouchPos.x > mVisibleSize.width * 0.05)
	{
	log("SWIPED LEFT");

	mIsTouchDown = false;
	if (!mSwipeLeft.name.empty()) {
	command(mSwipeLeft.name, mSwipeLeft.param);
	}
	}
	else if (mInitialTouchPos.x - mCurrentTouchPos.x < -mVisibleSize.width * 0.05)
	{
	log("SWIPED RIGHT");

	mIsTouchDown = false;
	if (!mSwipeRight.name.empty()) {
	command(mSwipeRight.name, mSwipeRight.param);
	}

	}
	if (mInitialTouchPos.y - mCurrentTouchPos.y > mVisibleSize.width * 0.05)
	{
	log("SWIPED DOWN");
	mIsTouchDown = false;
	}
	else if (mInitialTouchPos.y - mCurrentTouchPos.y < -mVisibleSize.width * 0.05)
	{
	log("SWIPED UP");
	mIsTouchDown = false;
	}
	}
	*/
}


void CPage::refreshPrevNextBtnInJtp(){
	if (KXMLReader::getInstance()->getPageCount() <= 9)return;
	
	if (mBtnPrevPageInJtp != nullptr){
		mBtnPrevPageInJtp->setVisible(false);
		mBtnNextPageInJtp->setVisible(false);
	}

	int tot = mAlbum->getTotalPage();
	int cur = mAlbum->getCurrentPage();


	if (mAlbum->getCurrentPage() > 0){
		mBtnPrevPageInJtp->setVisible(true);
	}

	if (mAlbum->getCurrentPage() + 1 < mAlbum->getTotalPage()){
		mBtnNextPageInJtp->setVisible(true);
	}
	
}
