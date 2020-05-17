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

/* ������ �����Ѵ�. */
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

/* ������ ������ ���� �ʱ�ȭ */
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

/* �ʱ�ȭ �۾��� �����Ѵ�. */
bool CPage::init(HPAGE_INFO* pPageInfo, STBOOK_INFO * pBookInfo){

	m_pPageInfo = pPageInfo;  /*å ���� ����*/
	m_pBookInfo = pBookInfo;  /*������ ���� ����*/

	mpJtpButtonInfo = nullptr;    /*Jump to Page �ʱ�ȭ*/
	mpExitButtonInfo = nullptr;
	mBtn = nullptr;           /* ��� ��� pause �� ����� ��ư �ʱ�ȭ */

	mPlayNarrationPos = 0;    /* narration list ��ġ */
	mTimerDuratin = 0;        /* 0.1���� �ð����� �ִϸ��̼� ���� �ð� Ȯ�� -> schedule_selector(CPage::callAfterAniStart), 0.1f)  */
	vtAni.clear();            /* �ٷ� �������� �ʴ� �ִϸ��̼� ���� */

	prevSoundAniSpr = nullptr;
	lastAniSpr = nullptr;

	lastCommandOfButton = COMMAND_NULL;

	int nCurAudio = KGlobalManager::getInstance()->getCurrentAudio();
	if (nCurAudio != AUDIOENGINE_UNDEFINED)
	{
		cocos2d::experimental::AudioEngine::stop(nCurAudio);
	}
	
	int nCurPage = atoi(pPageInfo->pageno.c_str()); /*���� ������ ��ȣ */
	int nPageCount = KXMLReader::getInstance()->getPageCount(); /* ��ü �������� */
	if (nCurPage == 999)      /* endpage�� 999�� ������ (���ʷ����Ϳ��� ����) */
		nCurPage = nPageCount - 1;

	mCurPage = nCurPage;

	if (mCurPage == 1000){ // embadded end of page
		return initEndPage();
	}

	if (mCurPage == 0){
		KGlobalManager::getInstance()->setAutoPlay(false); /*�б���� �ʱ�ȭ*/
		//KDataProvider::getInstance()->setListenMode(1);
	}

	/*�ش������� ����� ���������� �ִ��� Ȯ��*/
	m_userPlaysound = StringUtils::format("%s/%s/%d.mp3", KRecordingManager::getInstance()->getAndroidTargetPath().c_str()
		, pBookInfo->projectcode.c_str(), nCurPage);

	m_isUserSound = false; /*����� �������� �����*/
	
	/*����� �������� ���� ���� Ȯ��*/
	if (FileUtils::getInstance()->isFileExist(m_userPlaysound)) m_isUserSound = true;
	/* ����� ������ ���� play�� true / ������ �����̼� play�̸� false */
	bool bUserMode = KGlobalManager::getInstance()->getNarration();

	if (bUserMode) /* User Narration Mode */
	{
		if (m_isUserSound) /* ������ ������ �ִ�. */
		{
			HVOICE_INFO * pvoice = new HVOICE_INFO();
			pvoice->autorelease();

			vtVoiceManager.pushBack(pvoice);
			pvoice->sort = "0";
			pvoice->sound = m_userPlaysound;
		}
	}

	this->scheduleUpdate();

	// ���� ���̾�
	mBackground = Layer::create();
	this->addChild(mBackground);

	// ������Ʈ ���̾� 
	mContent = Layer::create();
	this->addChild(mContent);

	// UI�� ���̾�
	mUIObject = Layer::create();
	this->addChild(mUIObject);

	/*������ ��׶��� ����*/
	STBACKGROUND backgroundInfo = pPageInfo->stBackground;
	setBackground(backgroundInfo, mBackground);

	/*�˾����� �ʱ�ȭ*/
	mSetPopupIDs.clear();

	/*������ �Ӽ��� �����̼��� ������ ����*/
	if (pPageInfo->narration.empty() == false)
	{
		STCONTENT_INFO * pContentInfo = new STCONTENT_INFO();
		pContentInfo->group = "TEXTGROUP";
		pContentInfo->sort = "0";
		pContentInfo->sound = pPageInfo->narration;
		pContentInfo->id = "page_na";

		playNarrationList(pContentInfo);
	}

	/*������ �����۵� ������ ����*/
	for (int i = 0; i < pPageInfo->vtContents.size(); ++i) {
		STCONTENT_INFO *  pContentInfo = pPageInfo->vtContents.at(i);
		
		if (pContentInfo->type == "action") { /*�׼� ����*/
			appendButton(pContentInfo, mUIObject);
		}
		else if (pContentInfo->type == "animation") {  /*�ִϸ��̼� ����*/
			if (pContentInfo->startani.empty()) pContentInfo->startani = "0";
			
			if (pContentInfo->startani == "0"){    /*�ִϸ��̼� ������ ���۽� �ٷ� ����*/
				appendAnimation(pContentInfo, mContent);
			}
			else{                                   /*startani �ð��� �Ǹ� ȭ�鿡 ��Ÿ���� ����*/
				appendAnimation(pContentInfo, mContent , false);
				vtAni.pushBack(pContentInfo);
			}
			addSoundEvent(pContentInfo, mContent);  /*���� ó��*/
		}
		else if (pContentInfo->type == "image") {   /*�̹��� ����*/
			appendImage(pContentInfo, mContent);
			playNarrationList(pContentInfo);
			addSoundEvent(pContentInfo, mContent);  /*���� ó��*/ 
		}
		else if (pContentInfo->type == "text") {     /*�ؽ�Ʈ ����*/
			appendText(pContentInfo, mContent);
			playNarrationList(pContentInfo);
			addSoundEvent(pContentInfo, mContent);   /*���� ó��*/ 
		}
		else if (pContentInfo->type == "textinput") {    /*�ؽ�Ʈ ��ǲ ����*/
			appendTextInput(pContentInfo, mContent);
		}
		else if (pContentInfo->type == "particlearea") { /*��ƼŬ ����*/
			appendParticle(pContentInfo, mUIObject);
		}
	}

	set<string>::iterator iter;
	/*������ ������ �˾� ��� ���̵� ���� ��� ��ϵ�*/
	for (iter = mSetPopupIDs.begin(); iter != mSetPopupIDs.end(); iter++) {
		mPopupLength++;
	}

	mPopup = new Layer*[mPopupLength];
	int nTempCount = 0;

	

	/*�ش� �������� ������ �˾� ����*/
	for (iter = mSetPopupIDs.begin(); iter != mSetPopupIDs.end(); iter++) {
		/* xml���� ���� �˾� ������ ������ �´�.*/
		HPOPUP_INFO * pPopupInfo = KXMLReader::getInstance()->getPopupReference(*iter);
		if (pPopupInfo == nullptr) {
			log("Error !!! Occurred...  Happened.. Caution... can not reference target popup id in the page entity. Please reference to the valid one. from K.");
			log("Can't make Referenced Popup..");
			break;
		}
		mPopup[nTempCount] = Layer::create();   /*Popup Layer����*/
		this->addChild(mPopup[nTempCount]);



		mPopup[nTempCount]->setName(pPopupInfo->id); 

		setBackground(pPopupInfo->stBackground, mPopup[nTempCount]); /*���̾� ��� ����*/

		if (pPopupInfo->id == "album") {                  /*album�̸� Jump To Page �˾���*/
			appendAlbum(pPopupInfo, mPopup[nTempCount]);  /*�˾� ����*/
			mPopup[nTempCount]->setVisible(false);
			nTempCount++;
			continue;     /*Jump to Page�� �ʱ� ��ȹ�� ���� ����Ǿ� ��ü ������*/
		}

		mIsCameraActionOn = false;  /*ī�޶� on/off ���� ����*/

		/*�˾����� ������� ����*/
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
		/*������ ��������� ī�޶� ����� ������ mIsCameraActionOn = true �ȴ�.*/
		if (mIsCameraActionOn == true) {
			/*ī��������� ���� Node�޾� �´�.*/
			cocos2d::Node * pShowingImageNode = KCameraManager::getInstance()->getActionTargetNode();
			cocos2d::Sprite * pShowingCameraImage = static_cast<cocos2d::Sprite*>(pShowingImageNode);

			if (pShowingCameraImage != nullptr) {  /*���� ������ ������ �����Ѵ�.*/
				/*���� Ǯ��θ� �޾ƿ´�.*/
				std::string sPictureName = KCameraManager::getInstance()->getFullpathnameForPicture();
				/*������ �ִ��� Ȯ���Ѵ�.*/
				long nSize = FileUtils::getInstance()->getFileSize(sPictureName);
				if (nSize > 10) {
					log("picture file exists... ");
					Director::getInstance()->getTextureCache()->removeTextureForKey(sPictureName);

					/*ī�޶� ��������Ʈ�� �����.*/
					Sprite * pSprite = KCameraManager::getInstance()->getSprite();
					pSprite->initWithFile(sPictureName);  /*���� ������ �����´�.*/

					cocos2d::Size size = pSprite->getContentSize();

					Node * pTargetNode = KCameraManager::getInstance()->getActionTargetNode();
					if (pTargetNode == nullptr) {
						log("can't find the right target node.. check plz..");
						return false;
					}
					/*������ ������ ���� ����*/
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
			/*�Է� ���� ����� �̸� �޾ƿ��� ����*/
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
	if (mCurPage != 0){ // ù ������������ continueCheck - �ٽú���/�̾�� �˾����Ŀ� sound play �����ش�. // continueCheck �ȶ㶧��?
		playBgmAndNarrationAndStartAni();
	}
	else if (KDataProvider::getInstance()->getIsFirst() == false && mCurPage == 0){ // �������� �ٽ� �ǵ��ƿ����� 
		playBgmAndNarrationAndStartAni();
	}

	//  e-book �̰� ù �������� �ƴϸ� pauseȭ�� ������ �� 
	if (KGlobalManager::getInstance()->getAutoPlay() && mCurPage != 0)
	{
		appendBlankSprite();
	}

	// e-book �̰� ù�������̰� �ȳ��˾��� ������� ���ٸ� �ȳ��˾��� ����. 
	// auto play ��忡���� �������� �ڵ����� �Ѿ�� Read to Me ��忡����..

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
				// �˾� �ȿ��� �ϵ��� ����
			});
		}
	}

	// ���丮�� ����. �ٽ� ��� �������� �׸� �Ǵ� ������ ��ġ�� ������. ���������� ǥ�õ� �Ĵн� ���带 ��ġ�ϸ� �Ҹ��� ���� Ȯ���� �� �־��
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
	/* // end of page�� ���� �߰��Ͽ� ������ �������� end of page�� �ƴϹǷ� ������ ������ �ʰ� �Ѵ�. 160718 kyh
	else{ 
		
		// ������ ������ bgm stop �ڽ� bgm�� play
		//stopBGM();
		//if (KDataProvider::getInstance()->getSoundState() && !pPageInfo->backgroundMusic.empty()) {
		//	setBgm(pPageInfo->backgroundMusic);
		//}
	}
	*/
	
	log(" Page::init(), ================ mCurPage:%d, nPageCount:%d, getIsFirst():%d", mCurPage, nPageCount, KDataProvider::getInstance()->getIsFirst());

	if(KDataProvider::getInstance()->getIsFirst() == false){      /* �ٽ� �ϱ�� replay�ϴ� ��Ȳ�� �ƴϸ� */
		KDataProvider::getInstance()->setProgressPage(nCurPage);  /*���� �������� ����*/
		          
		if (mCurPage == (nPageCount - 1)){                        /*�������������̸�*/
			KDataProvider::getInstance()->setFinished();          /*�ϵ����� �˸�, MSLP.finish �ƴ�.*/ 
			KDataProvider::getInstance()->releaseCpuLock();       /*������� �����ǰ� ��û*/
		}
		return true; 
	}

	int nVol = KDataProvider::getInstance()->getSystemVolumn();   // �ý��� ���� �޾ƿ�
	
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


	// win32 ȯ�濡���� continueCheck�� ���� �����Ƿ� ù���������� ������ ������ ����.
	// ���׷� �����ϴ� ���̽��� �־� ���带 �÷��� ������. 
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	if (mCurPage == 0){
		playBgmAndNarrationAndStartAni();
	}
#endif

	// Swipe  �ִϸ��̼� �� Ư���� �������� (å�� ��ȣ�ۿ� �̺�Ʈ ����)
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

/*�������� ����� �������� Ȯ�� �� �۵�*/
void CPage::continueCheck()
{
	int nPageCount = KXMLReader::getInstance()->getPageCount();   /*������ ��ü�� �޾ƿ�*/
	KDataProvider::getInstance()->isFinished();                   /*�ϵ��������� JNIȣ��*/
	KDataProvider::getInstance()->getProcess();                   /*�������� ����� �������� ��û JNI*/

	log("CPage::continueCheck(), getOneFinished:%s", KDataProvider::getInstance()->getOneFinished().c_str());

	int nProgressPage = KDataProvider::getInstance()->getProcessPage();/*�������� ����� �������� �޾ƿ���*/
	if (nProgressPage == (nPageCount - 1)) nProgressPage = 0;
	KDataProvider::getInstance()->setFirst(false); /*ó�� ������ �ƴ��� �˸�*/
	/*���� ������ ������ �˸� �޼��� ������*/

	if (nProgressPage > 0)  
	{
		KPopupLayer * pLayer = KPopupLayer::createLayer();
		this->addChild(pLayer);

		pLayer->continueYesNoTypeCallback(
			[&](cocos2d::Ref*){
			AlertCallback(0, NativeAlert::ButtonType::RETURN); // �̾��
		},
			[&](cocos2d::Ref*){
			AlertCallback(0, NativeAlert::ButtonType::CANCEL); // ó������ ����
		});
		

		/*android �Ϲ� �˾� ���� */
		//NativeAlert::showWithCallback( __TX("Ȯ��").c_str(),  __TX("ó������ ������(��)\n���� �������� ������(���)�� �����ּ���.").c_str(), __TX("���").c_str(), __TX("��").c_str(), "", 404, CC_CALLBACK_2(CPage::AlertCallback, this));
	}
	else {

		playBgmAndNarrationAndStartAni();

		KDataProvider::getInstance()->setBeginBook(0, nPageCount - 1, 0);  /*�н� ���� �˸� JNI*/
	}
}

/*����� �˾� ������ event Ȯ��*/
void CPage::AlertCallback(int tag, NativeAlert::ButtonType type)
{ 

	log("AlertCallback %d", tag);
	int nPageCount = KXMLReader::getInstance()->getPageCount();  /*��ü �������� �޾ƿ�*/
	int nProgressPage = KDataProvider::getInstance()->getProcessPage(); /*�������� �޾ƿ�*/
	/*������ ����� ��� �б� ��� ��������*/
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
	case NativeAlert::ButtonType::OTHER:  /*�����ϴ� �̺�Ʈ Native Popup�� �ʿ�*/
		break;
	case NativeAlert::ButtonType::CANCEL:  /*ó������ ����*/
		log("AlertCallback first ");
		
		playBgmAndNarrationAndStartAni();

		KDataProvider::getInstance()->setBeginBook(0, nPageCount - 1, nMode);

		break;
	case NativeAlert::ButtonType::RETURN:  /*�̾��*/
		log("AlertCallback current page continue ");
		i = nProgressPage;
		if (i < 0) i = 0;
		KDataProvider::getInstance()->setBeginBook(i, nPageCount - 1, nMode); // �н����۾˸� JNI
		page = new int[1]{i};
		// �ش� �������� �̵� 

		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		break;

	}

}

void CPage::playBgmAndNarrationAndStartAni(){
	playBGM();

	if (vtVoiceManager.size() > 0) {  // �����̼� ����Ʈ�� �ִ� ���� �÷���
		this->scheduleOnce(schedule_selector(CPage::callAfterOneSecond), 1.0f);
	}
	if (vtAni.size() > 0) {   // �ִϸ��̼� �ڽ��� ���;� �Ǵ� �ð��� ������ �ϱ�
		this->schedule(schedule_selector(CPage::callAfterAniStart), 0.1f);
	}
}

/*�����̼� ������� �����ϱ�*/
bool sortByX(const Ref* obs1, const Ref* obs2)
{
	HVOICE_INFO* ob1 = (HVOICE_INFO*)obs1;
	HVOICE_INFO* ob2 = (HVOICE_INFO*)obs2;
	return ob1->sort < ob2->sort;
}

/*�����̼� �÷��� ����Ʈ ����*/
void CPage::playNarrationList(STCONTENT_INFO *  pContentInfo)
{
	//playingcolor

	bool bUserMode = KGlobalManager::getInstance()->getNarration();

	if (bUserMode) /* User Narration ��� (���� : ����� ���� ������ ������ ������ �ý��� Narration�����.)*/
	{
		if (m_isUserSound) /*������ ������ ����  init���� ��������� �������*/
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

	if (nSize == 0) {  /*�ִϸ��̼��� ��� ȭ�鿡 ������ unschedule �Ѵ�. */
		this->unschedule(schedule_selector(CPage::callAfterAniStart));
	}

	for (int i = 0; i < nSize; ++i) {
		STCONTENT_INFO *  pContentInfo = vtAni.at(i);
		ft = atof(pContentInfo->startani.c_str());
		if (ft <= mTimerDuratin){  /*�ִϸ��̼��� ���;��� �ð��̸� ������ �Ѵ�. �׸��� vtAni������ ����*/
			
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

/*�����̼Ǹ���Ʈ �÷��� �����ϱ�*/
void CPage::callAfterOneSecond(float dt){
	log("Start Duration...%f",dt);
	playNarrationList();
}

/*�����̼� �ϳ��� �÷��� �ϱ�*/
void CPage::playNarrationList()
{
	string sFile = getNarrationPostion(); /*�÷����� ���� ���� �޾ƿ´�.*/
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();  /*��������� �б������� �޾ƿ´�*/
	if (sFile == "")
	{
		if (KGlobalManager::getInstance()->getAutoPlay() == false) return;
		/*��� ����̰� �� ������� �ڵ����� ���� �������� �̵��Ѵ�.*/
		command(COMMAND_NEXT, "");
		return;
	}
	/*��� ����� ��� ȭ�� ��ġ�� pauseȭ���� ����Ǹ� �÷��̸� ���ش�.*/
	if (mBtn != nullptr){
		if (mBtn->isVisible()){
			return;
		}
	}
	bool isUser = false;
	bool bUserMode = KGlobalManager::getInstance()->getNarration(); /*������ �������� �ƴ��� �����´�.*/
	if (m_isUserSound)
	{
		if (bUserMode) isUser = true;
	}

	
	if (bAuto == false)  /*�б����� ���*/
	{
		/*������ �÷����� �϶� ������� �Ǹ� �ȵ�*/
		KDataProvider::getInstance()->acquireCpuWakeLock(); /*������� ���� JNIȣ��*/
	}

	if (isUser){ /*����� ���� �����̸� ������ ���� �÷���*/
		mAudioEngineID = playSound(sFile);
	}
	else{
		mAudioEngineID = playSound(KCTT(sFile));
		mNarrationAudioID = mAudioEngineID;
	}

	KGlobalManager::getInstance()->setCurrentAudio(mAudioEngineID);
	/*���� ����� ���� �Ǹ� callback*/
	cocos2d::experimental::AudioEngine::setFinishCallback(mAudioEngineID, [&](int audioID, const std::string & fileName) {
		mAudioEngineID = AUDIOENGINE_UNDEFINED;
		mNarrationAudioID = AUDIOENGINE_UNDEFINED;
		bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
		if (bAuto == false)
		{
			/*���� ����� ������� �����ǰ� JNI*/
			KDataProvider::getInstance()->releaseCpuLock();
		}
		setTextOrg(); /*�ؽ�Ʈ ���̶���Ʈ ����*/
		playNarrationList(); /*���� ���� �б�*/
	});
}

/*���� ��� �Ǻ��� �����̼� �÷��� �ǰ� �۵�*/
int CPage::playSound(std::string sFile)
{
	return playSound(sFile, false);
}

int CPage::playSound(std::string sFile, bool bLoop)
{
	float fSoundVol = 0.0f;
	bool bSound = KDataProvider::getInstance()->getSoundState(); /*����������� �Ǻ� (Jump to Page���� ������� ��������)*/
	int nAudio;
	
	if (bSound)  //������
	{
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), bLoop);
	}
	else {       //*������� :  ������� �� ��� ���� �Ǿ�� �ϹǷ� ������ ���δ�.
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), bLoop, fSoundVol);
	}

	return	nAudio;
	
}

/*�÷��� �� �����̼� ���� ������ �޾ƿ´�.*/
string CPage::getNarrationPostion()
{
	int nList = vtVoiceManager.size();
	if (nList == 0) return "";

	if (mPlayNarrationPos >= nList) return "";
	/*mPlayNarrationPos ��ġ ����*/
	HVOICE_INFO *  pVoice = (HVOICE_INFO *)vtVoiceManager.at(mPlayNarrationPos);
	mPlayNarrationPos++;

	/*�����̼ǿ� �ش�Ǵ� �ؽ�Ʈ�� ã�� ���̶���Ʈ �Ѵ�.*/
	searchObject(pVoice->id,pVoice->playingcolor);

	return pVoice->sound;
}

/*�����̼ǰ� �����Ǿ� �ִ� �ؽ�Ʈ�� �ʱⰪ ������ �����Ѵ�.*/
void CPage::setTextOrg()
{
	STCONTENT_INFO *  pContentInfo = nullptr;
	for (int i = 0; i < m_pPageInfo->vtContents.size(); ++i) {
		pContentInfo = m_pPageInfo->vtContents.at(i);

		setTextOrgColorReal(pContentInfo->id, pContentInfo->fontcolor);
	}
	
}

/*�����̼ǿ� �ش�Ǵ� text�� ã�� ���� �ٲ۴�.*/
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
				if (playingcolor.empty()) color = convertRGB("#ff00ff"); //������ ���� ������ 
				else color = convertRGB(playingcolor.c_str());
				label->setTextColor(Color4B(color));
				return;
			}
		}
	}
}

/*�����̼ǿ� �ش��ϴ� Text �ʱⰪ ������ �����ϱ�*/
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

/*���̵� �ش�Ǵ� �ؽ�Ʈ ã�� ���� ������ ����*/
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

/*��� �׼� ó��*/
void CPage::command(string command, string param, string param2, Ref * sender ) {
	if (command.empty()) {
		return;
	}
	 
	/*�׼��� �Ǹ� ��ȣ�ۿ��� �� ��� �̹Ƿ� */
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
	if (bAuto == false) 
	{
		KDataProvider::getInstance()->releaseCpuLock();  /*������� �۵��ǰ� JNI ȣ��*/
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
	
	if (command == COMMAND_PREV) {     /*���� �������� �̵�*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_PREV);
	}
	else if (command == COMMAND_NEXT) { /*���� �������� �̵�*/
		int nCurPage = atoi(m_pPageInfo->pageno.c_str()); /*���� ������ ��ȣ */
		//int nPageCount = KXMLReader::getInstance()->getPageCount(); /* ��ü �������� */

		if (nCurPage == 0){
			// ù �������� next�� i-playbook ���� ����. i-playbook���� setListenMode�� ���ִ°��� ��� ù�������� next�� setListenmode�� ����Ѵ�.
			KGlobalManager::getInstance()->setAutoPlay(false); /*�б��� ���*/
			KDataProvider::getInstance()->setListenMode(1);                /*��� ������ �˸���. JNI*/
			KDataProvider::getInstance()->releaseCpuLock();
		}
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_PREV_NOPAGING) {   /*���� �������� �̵� �ѱ�ȿ�� ����..*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_PREV_NOPAGING);
	}
	else if (command == COMMAND_VOICE_SYSTEM) {    /*�����̼� ����Ʈ ���*/
		KGlobalManager::getInstance()->setNarration(false);
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_VOICE_USER) {               /*�����̼� ����� ���� ���*/ 
		KGlobalManager::getInstance()->setNarration(true);  /*����� ������ �� ���*/
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_NEXT);
	}
	else if (command == COMMAND_AUTO_PLAY) { /*������� �����̼� auto ���� �����Ѵ�. ��尡 �ٲ�� �� ó�� ���� �����Ѵ�.*/
		KGlobalManager::getInstance()->setAutoPlay(true); /*����� ���*/
		KDataProvider::getInstance()->setListenMode(0);               /*��� ������ �˸���. JNI*/
		/*�������� �ڵ����� �������� �Ѿ��. ������� ����*/
		KDataProvider::getInstance()->acquireCpuWakeLock();
		
		int i = 1;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		
	}
	else if (command == COMMAND_MANUAL_PLAY) {  /*�б���*/
		KGlobalManager::getInstance()->setAutoPlay(false); /*�б��� ���*/
		KDataProvider::getInstance()->setListenMode(1);                /*��� ������ �˸���. JNI*/
	    /*�б���� ������� ����*/
		KDataProvider::getInstance()->releaseCpuLock();
		//��ȹ ���� �Ǿ� ���� ���� ������
	    //showPopup("selectedUser");
		
		int i = 1;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
		
	}
	else if (command == COMMAND_COVERPAGE) { /*Ŀ�� �������� �̵�*/
		int i = 0;
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
	}

	else if (command == COMMAND_SETPAGE || command == COMMAND_GOTO) { /*������ �������� �̵�*/
		int i = atoi(param.c_str());
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) /*���� ���� �������� �̵� �ѵ�*/
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();
		int nProgressPage = KDataProvider::getInstance()->getProcessPage();
		if (isFindshed == "n"){
			if (i > nProgressPage){
				MessageBox(__TX("���� ���� �������� �̵��� �� �����ϴ�.").c_str(), __TX("Ȯ��").c_str());
				return;
			}
		}
#endif
		int* page = new int[1]{i};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
	}
	else if (command == COMMAND_EXIT) {    /*����*/
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();  /*�ϵ��Ǿ����� Ȯ��*/
		/*�ϵ����� ���� JNI*/
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif	
		Director::getInstance()->end();
		
	}
	else if (command == COMMAND_NEXTBOOK){    /*���� ����Ʈ�� �̵�*/
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32)  /*���� �ϵ����� ����*/
		std::string isFindshed = KDataProvider::getInstance()->getOneFinished();
		if (isFindshed == "y") KDataProvider::getInstance()->currentBookFinish("true");
		else KDataProvider::getInstance()->currentBookFinish("false");
#endif		
		KDataProvider::getInstance()->nextBook();  /*���� ����Ʈ �̵� ��û JNI*/
		Director::getInstance()->end();
	}
	else if (command == COMMAND_POPUPEXIT) {       /*�ش� �˾� ����*/
		Node * pNode = (Node *)sender;
		pNode->getParent()->setVisible(false);
		mIsPopup = false; /*�˾� Ȱ������ ����*/

		playBGM();

		/*�ִϸ��̼� ����*/
		Vector<Node *>  vecChildren = mContent->getChildren();
		for (int i = 0; i < vecChildren.size(); i++) {
			Node * pNode = vecChildren.at(i);
			std::string aniname = (std::string) pNode->getName();
			if (std::strncmp(aniname.c_str(), "ani",3) == 0) {
				if (pNode->isVisible())
					pNode->resume();
			}
		} 
		if (mAlbum != nullptr) /*Jump to Page �˾� ���� ó�� ó��*/
		{
			mPopup[mJTPpopupIndex]->setVisible(false);

			mAlbum->setBlocked(false);

			/*�����̼� �� ����*/

			//cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
			cocos2d::experimental::AudioEngine::resume(mNarrationAudioID);

			//if (mPlayNarrationPos != 0) mPlayNarrationPos--;
			//playNarrationList();

			if (mPlayNarrationPos != 0 && mNarrationAudioID != AUDIOENGINE_UNDEFINED) mPlayNarrationPos--;

			playNarrationList();

			// sound off ���¶� �̺Ͽ����� �������� �̺�Ʈ�޾� �������������� �ϹǷ� ������ 0���� �ϰ� �÷��� ��Ų��.
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
				
				/*�����̼� �� ����*/ // ���� �̵�. 
				//cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
				//if (mPlayNarrationPos != 0) mPlayNarrationPos--;
				//playNarrationList();
			}
		}
			
	}
	else if (command == COMMAND_HIDE) {  /*hide �׼� ó��*/
		if (sender != nullptr)   /*�ڽ��� �⺻ ������ hide*/
		{
			Node * pNode = (Node *)sender;
			Sequence * seq = Sequence::create(FadeOut::create(0.3f),  nullptr);
			pNode->stopAllActions();
			pNode->runAction(seq);
			Layer * pParent = (Layer *)pNode->getParent();
		}
		if (param.empty() == false) {  /*hide �� ����Ʈ �����*/
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

		if (param2.empty() == false) {  /*���������� ������ ���̰� �ϱ�*/
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
	else if (command == COMMAND_POPUP) {  /*�˾� ���� �׼�*/
		if (param.empty()) {
			log("POPUP's target is not set.");
			return;
		}
		if (param == "record"){
			return;
		}
		//CocosDenshion::SimpleAudioEngine::getInstance()->pauseBackgroundMusic(); /*������� ���ش�.*/
		stopBGM();

		setTextOrg();   /*�����̼� �ؽ�Ʈ �ʱ������ ����*/
		int nIndex = getPopupIndexFromTargetID(param);  /*�ش� �˾� ���� �޾ƿ���*/
		if (nIndex == -1) {
			log("There is no popups with targetID.. check please targetid.");
			return;
		}
		/*�˾� ������ �۵� ���߱�*/
		Vector<Node *>  vecChildren = mContent->getChildren();
		for (int i = 0; i < vecChildren.size(); i++) {
			Node * pNode = vecChildren.at(i);
			std::string aniname = (std::string) pNode->getName();
			if (std::strncmp(aniname.c_str(), "ani", 3) == 0) {
				if (pNode->isVisible())
					pNode->pause();
			}
		}

		/*Jump to page ���� ��� ó��  �Ʒ� ȭ�� ��ġ �ȵǰ� �ϱ�*/
		if (param == "album"){
			mAlbum->setBlocked(true);
			mAlbum->refreshPosition();
			mAlbum->setJtpPageToCurrentPage(mCurPage);
			refreshPrevNextBtnInJtp();
			mJTPpopupIndex = nIndex;
		}
		mPopup[nIndex]->setVisible(true);
		mIsPopup = true;   /*�˾� Ȱ�� ���� ����*/
	}
	else if (command == COMMAND_ALBUMPREV) {    /*Jump to page ������������ �̵�*/
		if (mAlbum != nullptr) {
			mAlbum->prev();
			refreshPrevNextBtnInJtp();
		}
	}
	else if (command == COMMAND_ALBUMNEXT) {    /*Jump to page ������������ �̵�*/
		if (mAlbum != nullptr) {
			mAlbum->next();
			refreshPrevNextBtnInJtp();
		}
	}
	else if (command == COMMAND_CAMERA) {       /*ī�޶� �۵�*/
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
		MessageBox("�̸����⿡�� �۵����� �ʽ��ϴ�.", "Ȯ��");
		return;
#endif
		/*�̸��� �Է� �ȵǾ������� �̸����� �Է� �޾ƾ���*/
		ui::TextField * textfield = KCameraManager::getInstance()->getTextFieldObject();
		std::string sCurrentText = textfield->getString();
		if (sCurrentText.empty()) {
			MessageBox(__TX("�̸��� �Է��� �ּ���.").c_str(), __TX("Ȯ��").c_str());
			return;
		}
		KCameraManager::getInstance()->setDelegate(this);  /*Delegate ����*/

		/*ī�޶� ����Ÿ ���� (������Ʈ �ڵ� , �̸�)*/
		KJSONDataManager::getInstance()->setCameraData(KXMLReader::getInstance()->getBookInfo()->projectcode, textfield->getString());
		
		/*��� �޾ƿ���*/
		std::string sFilename = KCameraManager::getInstance()->getFullpathnameForPicture();
		/*ī�޶� �۵� ��û JNI*/
		KCameraManager::getInstance()->requestPicture(sFilename);
	} 
	else if (command == COMMAND_RECORD) {
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
		MessageBox("�̸����⿡�� �۵����� �ʽ��ϴ�.", "Ȯ��");
		return;
#endif
		Scene * pScene = KRecordingLayer::createScene(); /*���� ���� KRecordingLayer ó����*/
		Director::getInstance()->pushScene(pScene);
	}
	else if (command == COMMAND_SOUNDON) {  /*���� ON (Jump to page���� ������Ŵ)*/
		log("Sound On");
		KDataProvider::getInstance()->setSoundState(true);
	}
	else if (command == COMMAND_SOUNDOFF) {  /*���� OFF (Jump to page���� ������Ŵ)*/
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

/*@id �ش� �˾� ������ �ޱ�*/
int	CPage::getPopupIndexFromTargetID(string id){
	for (int i = 0; i < mPopupLength; i++) {
		if (mPopup[i]->getName() == id) {
			return i;
		}
	}
	return -1;
}


void CPage::playBGM(){
	//CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();  /*������� ����*/
	/*
	bool bSound = KDataProvider::getInstance()->getSoundState(); // ����������� �Ǻ� (Jump to Page���� ������� ��������)
	if (bSound){
		if (m_pPageInfo->backgroundMusic.empty() && m_pPageInfo->backgroundMusic.length() == 0)
		{
			bool bmainbgm = KDataProvider::getInstance()->getMainBgm();
			//MainBgm �� �ƴϸ� ���� ��Ų��.
			if (bmainbgm == false) stopBGM();

			if (isBgmPlaying() == false)
			{
				stopBGM();
				if (m_pPageInfo->backgroundMusic.empty()) {  ///�ش������� bgm������ ������Ʈ������ �ִ� bgm����
					if (!m_pBookInfo->backgroundSound.empty()){
						setBgm(m_pBookInfo->backgroundSound);
						KDataProvider::getInstance()->setMainBgm(true);
					}
				}
				else {
					setBgm(m_pPageInfo->backgroundMusic);    // �ش������� �ڽ� bgm�� ������ �÷���
					KDataProvider::getInstance()->setMainBgm(false);
				}
			}
		}
		else // �ش������� �ڽ� bgm�� ������ �÷���
		{
			stopBGM();
			setBgm(m_pPageInfo->backgroundMusic);
			KDataProvider::getInstance()->setMainBgm(false);
		}
	}
	*/
	
	bool bSound = KDataProvider::getInstance()->getSoundState(); // ����������� �Ǻ� (Jump to Page���� ������� ��������)
	if (bSound){   
		if (m_pPageInfo->backgroundMusic.empty() && m_pPageInfo->backgroundMusic.length() == 0)
		{
			bool bmainbgm = KDataProvider::getInstance()->getMainBgm();
			//MainBgm �� �ƴϸ� ���� ��Ų��.
			if (bmainbgm == false) CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();

			if (!CocosDenshion::SimpleAudioEngine::getInstance()->isBackgroundMusicPlaying())
			{
				CocosDenshion::SimpleAudioEngine::getInstance()->stopBackgroundMusic();
				if (m_pPageInfo->backgroundMusic.empty()) {  ///�ش������� bgm������ ������Ʈ������ �ִ� bgm����
					if (!m_pBookInfo->backgroundSound.empty()){
						setBgm(m_pBookInfo->backgroundSound);
						KDataProvider::getInstance()->setMainBgm(true);
					}
				}
				else {
					setBgm(m_pPageInfo->backgroundMusic);    // �ش������� �ڽ� bgm�� ������ �÷���
					KDataProvider::getInstance()->setMainBgm(false);
				}
			}
		}
		else // �ش������� �ڽ� bgm�� ������ �÷���
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

/*string color ��ȯ*/
Color3B CPage::convertRGB(const char* input)
{
	char* pStop;
	
	int num = strtol(input + 1, &pStop, 16);
	int b1 = (num & 0xFF0000) >> 16;
	int b2 = (num & 0xFF00) >> 8;
	int b3 = num & 0xFF;
	return Color3B(b1, b2, b3);
}

/* ������ ��׶��� �̹��� ������ ����*/
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

/*�׼� ��ư ����*/
void CPage::appendButton(STCONTENT_INFO * pContentInfo, Layer* target){
	
	string image = pContentInfo->image;
	string select = pContentInfo->selected;
	string disabled = pContentInfo->disabled;

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	//listen mode �� ��� next�� prev �Ⱥ��̰� �Ѵ�.
	string sActionTarget = pContentInfo->action.param;
	bool bAuto = KGlobalManager::getInstance()->getAutoPlay();
	if (bAuto)  /*���(LISTEN) ��� */
	{
		/*��� ��忡���� �������� �ʴ´�. �ڵ����� ���� ������ �̵�*/
		if (pContentInfo->action.name == COMMAND_PREV || pContentInfo->action.name == COMMAND_NEXT)
		{
			return;
		}
		if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_POPUP) {
			if (sActionTarget == "album")  /*���������� ó�� �Ǿ� mSetPopupIDs ������ �־� �д�. */
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
		//��ȹ �������� ���Ƶ�
			mSetPopupIDs.insert("selectedUser");
	}
	else if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_POPUP) {
		mSetPopupIDs.insert(sActionTarget);
	}
	else if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_CAMERA) {
		/*ī�޶� ��û �׼��̸� �⺻���� ������ ��*/
		KCameraManager::getInstance()->setActionTargetName(sActionTarget);
		KCameraManager::getInstance()->setActionTargetLayer(target);
		mIsCameraActionOn = true;
	}

	if (!image.empty()) {
		// ��ư�� �ҷ��´�.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;
		std::string sPictureName ="";

		if (pContentInfo->action.name == COMMAND_VOICE_USER) /*�׼��� ����� �����̼� ����� ���*/
		{
			sPictureName = KCameraManager::getInstance()->getFullpathnameForPicture();
			long nSize = FileUtils::getInstance()->getFileSize(sPictureName);
			if (nSize < 11)   sPictureName = KCTV(image);
		}
		else	sPictureName = KCTV(image);

		auto button = Button::create(sPictureName, KCTV(select), KCTV(disabled));
		if (pContentInfo->visible == "false") button->setVisible(false);
		
		ACTION_PARAM* param = new ACTION_PARAM();  /*�̺�Ʈ ���� ���� �� ������ �д�.*/
		param->autorelease();
		vtRefCountManager.pushBack(param);
		param->name = pContentInfo->action.name;     /*�׼��̸�*/
		param->param = pContentInfo->action.param;   /*�Ķ��Ÿ��1*/
		param->param2 = pContentInfo->action.param2; /*�Ķ��Ÿ��2*/
		param->pContentInfo = pContentInfo;          /*�ش�׼� ���� point*/
		button->setUserData((void*)param);           /*�ֿ� ���� ����*/

		CViewUtils::setSize(button, x, y, width, height);  /*������ ũ�⿡ �°� ����*/
	
		target->addChild(button);

		
		if (!pContentInfo->id.empty()) {
			button->setName(pContentInfo->id);
		}

		/*��ư �̺�Ʈ �߻��� ó��*/
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

			/*��ġ ���¿� ���� scaleȿ�� ����*/
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

					if (!pCntInfo->sound.empty())  /*�׼ǿ� ������ ���� ��� ���� �÷����� command�۵���Ŵ*/
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

/*�ִϸ��̼� ���� ����*/
void CPage::appendAnimation(STCONTENT_INFO* pContentInfo, Layer* target,bool bplay) {
	/*�⺻ �̹����� ��������Ʈ�� �����Ѵ�.*/
	Sprite* sprite = appendImage(pContentInfo, target);
	sprite->setName(pContentInfo->id);

	/*normaltype == true �̸� �̺�Ʈ�� ���� �ʰ� �ִϸ��̼� ���۸� �Ѵ�.*/
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

/*�ִϸ��̼� ��ġ�� �۵� */
void CPage::appendAnimationEvent(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	/*�̺�Ʈ �۵��� �ʿ��� ���� ������ ��*/
	ACTION_PARAM* param = new ACTION_PARAM();
	param->autorelease();
	param->name = pContentInfo->action.name;
	vtRefCountManager.pushBack(param);
	param->pContentInfo = pContentInfo;
	param->target = target;
	param->param = pContentInfo->action.param;
	param->param2 = pContentInfo->action.param2;

	sprite->setUserData((void*)param);

	/*��ġ�� �̺�Ʈ ó��*/
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(true);
	listener1->onTouchBegan = [&](Touch * touch, Event * event) {
		auto target1 = static_cast<Sprite*>(event->getCurrentTarget());
		if (mIsPopup) {  /*�˾��� Ȱ��ȭ �Ǿ����� �����ϰ� �����Ѵ�.*/
			if (target1->getParent() == mUIObject) {
				return false;
			}
		}

		if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV)return false;

		Point locationInNode = target1->convertToNodeSpace(touch->getLocation());
		Size s = target1->getContentSize();
		Rect rect = Rect(0, 0, s.width, s.height);

		if (rect.containsPoint(locationInNode))  /*�ִϸ��̼� ������ ��� �Դ°�?*/
		{
			ACTION_PARAM* parma = (ACTION_PARAM*)target1->getUserData();
			string action = parma->name;
			string param = parma->param;
			string param2 = parma->param2;

			/* ������ �ʿ�.
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
			else /* ?ȸ �ִ��� ���� */
			{
				target1->stopAllActions();
				/*�ִϸ��̼� ���� �۵�*/
				playAnimation(parma->pContentInfo, parma->target, target1);
			}
			return true;
		}
		return false;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener1, sprite);
}

/*���� �ʿ� ���� ���� ��*/
bool CPage::isSpriteTransparentInPoint(Sprite* sprite, Point& location) {

	return HitDetectHelper::hitTest(sprite, location);

}
void CPage::stopPrevAnimation()
{
	// B �ִϸ� �÷��̽�Ű���� �Ҷ� �ٸ� �ִ�A�� �������̸� A�� ���߰� �Ѵ�. 
	if (lastAniSpr != nullptr){
		if (lastAniSpr->numberOfRunningActions() > 0){
			lastAniSpr->stopAllActions();
		}
	}
}


/*�ִϸ��̼� ����*/
void CPage::playAnimation(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite, bool bplay, bool bforceshow) {

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;
	int length = pContentInfo->vtAnimation.size();
	Vector<SpriteFrame*> animFrames;

	// B �ִϸ� �÷��̽�Ű���� �Ҷ� �ٸ� �ִ�A�� �������̸� A�� ���߰� �Ѵ�. 
	stopPrevAnimation();

	sprite->stopAllActions();
	lastAniSpr = sprite;

	/*������ �̹��� ����*/
	for (int i = 0; i < length; i++)
	{
		Sprite * pAnimSprite = Sprite::create(KCTV(pContentInfo->vtAnimation.at(i)->image));
		Size sizeAni = pAnimSprite->getContentSize();
		if (sizeAni.width == 0 || sizeAni.height == 0) continue;
		CViewUtils::setSize(pAnimSprite, x, y, width, height);
		animFrames.pushBack(pAnimSprite->getSpriteFrame());
	}
	/*�����Ӵ� ��ȯ�ð� ����*/
	float fDelay = atof(pContentInfo->delay.c_str()) * 0.001;
	int   nLoop = 1;
	
	if (pContentInfo->aniloop.empty() == false)  /*���ѷ����� �ƴϸ� ������ Ƚ����ŭ*/
	{
		string aniloop = pContentInfo->aniloop.substr(0, 1);
		nLoop = atoi(aniloop.c_str());
		if (nLoop == 0) nLoop = -1;
	}
	
	/*�ִϸ��̼� ����*/
	
	auto animation = Animation::createWithSpriteFrames(animFrames, fDelay, nLoop); //-1
	auto animate = Animate::create(animation);
	pContentInfo->image = pContentInfo->vtAnimation.at(0)->image;
	CViewUtils::setSize(sprite, x, y, width, height);
	sprite->runAction(animate);
	
	float fTime = atof(pContentInfo->time.c_str()) * 0.001f; /*�̵��ð� ����*/
	float dx = atof(pContentInfo->dx.c_str());   /* x�� ���� �̵� �Ÿ�*/
	float dy = atof(pContentInfo->dy.c_str());   /* y�� ���� �̵� �Ÿ�*/
	auto moveby = MoveBy::create(fTime, Vec2(dx, dy));  /*������ ��ü ����*/
	auto reverse = moveby->reverse();                   
	auto ease1 = EaseInOut::create(moveby->clone(), 3.0f);
	auto east2 = EaseInOut::create(reverse->clone(), 3.0f);
	auto seqMove = Sequence::create(ease1, east2, nullptr);  /*�Դٰ��� ����*/
	/* ���̰� �� ���� ���� ���� �Ǵ�*/
	if (bforceshow) sprite->setVisible(true);
	else{
		if (pContentInfo->visible == "false" || pContentInfo->visible == "False") sprite->setVisible(false);
		else sprite->setVisible(true);
	}
	/*�ִϸ��̼��� ���� ������ ��� �ΰ� ���ø� �����Ѵ�.*/
	cocos2d::Vector<FiniteTimeAction *> actions;
	actions.pushBack(seqMove);

	/* Rotate �߰� 20160403 */
	float fRotateDu = -1;
	float fRotateAn = -1;

	if (pContentInfo->rotate.empty() == false)  /*ȸ���ϱ� ���� ����*/
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


	/* Scale �߰� 20160403 */
	float fScaleDu = -1;
	float fScaleSS = -1;
	if (pContentInfo->scale.empty() == false)  /*ũ��ִ����� ����*/
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
	
	if (nLoop == -1)  /*���� �ݺ�*/
	{
		RepeatForever * repeat;
		repeat = RepeatForever::create(allAni);
		sprite->runAction(repeat);
	}
	else              /*������ Ƚ�� ��ŭ*/
	{
		Repeat * repeat1;
		repeat1 = Repeat::create(allAni, nLoop);
		sprite->runAction(repeat1);
	}
	/*�۵��� ���߾�� �� ��*/
	if (!bplay) sprite->stopAllActions();
}

/*�̹��� ������ ����*/
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

/*��ġ�� ���� �̺�Ʈ�� �����ؾ� �ϴ� ���*/
/*ó�� �ǵ� ��ġ�� �ؽ�Ʈ Į�� ����� ���� �÷��̿����� ������� ���ķ� �ִϸ��̼� ���Ʈ �ͽ� �����*/
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

	sprite->setOpacity(0); // ������ ������ ���̰� ������ �ʴ´�.

	/* ��ġ�� ���� ���� ����*/
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

	/*��ġ �̺�Ʈ ����*/
	auto listener1 = EventListenerTouchOneByOne::create();
	listener1->setSwallowTouches(true);
	listener1->onTouchBegan = [&](Touch * touch, Event * event) {

		if (lastCommandOfButton == COMMAND_NEXT || lastCommandOfButton == COMMAND_PREV)return false;

		// ���尡 �÷������̸� ��ġ�ν� ���ϰ� �Ѵ�. 
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
				/*���̵� �ش��ϴ� sprite��������*/
				Sprite* pSprite = (Sprite*)getChildByNameForLocal(parma->pContentInfo->id);
				if (pSprite){ /*�ش� ��ü�� ������ ������ �����Ѵ�.*/
					if (pSprite->getOpacity()==0 || 
						pSprite->isVisible() == false) return true;
				}
				
				setTextOrg(); /*�ؽ�Ʈ �ʱ������ �����Ѵ�.*/
				cocos2d::experimental::AudioEngine::stop(mAudioEngineID); //playing sound stop
				CocosDenshion::SimpleAudioEngine::getInstance()->stopEffect(mAudioEngineID);
				string sfile = parma->param2;
				/*���� �÷����Ѵ�.*/
				log("playSound, file:%s", sfile.c_str());
				mAudioEngineID = playSound(KCTT(sfile));

				/*�ؽ�Ʈ ���� ��ġ�� �� �ٲ۴�.*/
				if (std::strncmp(parma->param.c_str(), "text", 4) == 0) {
					// ���ͷ��ǰ� ������ �������� �ؽ�Ʈ ��ġ�� �ִϸ� �����Ų��.
					myPage->stopPrevAnimation();
					searchObject(parma->param, parma->pContentInfo->playingcolor);
				}
				/*�ִϸ��̼� ��ġ ���� �÷��� �ϰ� �����ϰ� ������.*/
				if (std::strncmp(parma->param.c_str(), "ani", 3) == 0) {
					
					if (pSprite){
						// �ٸ� Ŭ���� ���� �ִϰ�ü�� ���ְų� �Ⱥ��̰� �ϰ� ������ animation�� �˾�ó�� ���� ���� ������δ� �ȵ�.
						// ���������� �����־���� �ִϵ鵵 ����������� ������ ����.
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
				/*���� �Ϸ� �Ǹ� �ؽ�Ʈ �ʱⰪ���� �����Ѵ�.*/
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


/*�ؽ�Ʈ �������� �����Ѵ�.*/
void CPage::appendText(STCONTENT_INFO* pContentInfo, Layer* target) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	int nFontSize = atoi(pContentInfo->fontsize.c_str()) + 10; /*window���̴� ũ�⺸�� �۰� ���� +10 ����*/
	if (nFontSize == 0) nFontSize = 10;
	std::string sResult = "";
	std::string sFontFile = "";

	if (pContentInfo->id == "username") /*�������̵� : username�̸� ������̸� �޾ƿ���*/
	{
		sResult = KJSONDataManager::getInstance()->getCameraData(KXMLReader::getInstance()->getBookInfo()->projectcode);
	}
	else sResult = pContentInfo->text;

	/*��Ʈ ���� ���� �⺻ : �������*/
	if (pContentInfo->fontfile.empty()) sFontFile = "fonts/NanumGothic.ttf";
	else sFontFile = pContentInfo->fontfile;
		
#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	sFontFile = pContentInfo->fontname;       /*window���� ��Ʈ�̸����� �ѱ��.*/
	if (sFontFile.empty())
	{
		if(sFontFile == "fonts/NanumPen.ttf")	sFontFile = "�����ձ۾� ��";
		else if (sFontFile == "fonts/NanumGothic.ttf")	sFontFile = "�������";
		else sFontFile = "�������";
	}
	
#endif

	Label* label = Label::createWithSystemFont(sResult, sFontFile, nFontSize
				, Size((int)width , (int)height), TextHAlignment::LEFT, TextVAlignment::TOP);
    
	Color3B color = convertRGB(pContentInfo->fontcolor.c_str());
	label->setTextColor(Color4B(color));

	if (pContentInfo->outline.empty() == false)
	{
		if (pContentInfo->outline == "True")  /*outline�� ������ �ܰ��� �׸���.*/
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

/*textinput ��ü�� �����Ѵ�. ī�޶� �̸� �޴� ������ ��û�ȴ�.*/
void CPage::appendTextInput(STCONTENT_INFO* pContentInfo, Layer* target) {
	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	
	int nFontSize = atoi(pContentInfo->fontsize.c_str()) + 10;
	if (nFontSize == 0) nFontSize = 10;

	auto textfield = ui::TextField::create(pContentInfo->text, "�������", nFontSize);
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

/*Jump to Page�� �����Ѵ�.*/
void CPage::appendAlbum(HPOPUP_INFO* pPopupInfo, Layer* target) {

	if (mAlbum == nullptr) {     /*�˾��� �����Ѵ�.*/
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
	std::string isFindshed = KDataProvider::getInstance()->getOneFinished();	/*�ϵ�����*/

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32) 
	// for test
	//isFindshed = "n";
	// for test
#endif

	
	log("appendAlbum is Finished  %s", isFindshed.c_str());
	int nProgressPage = KDataProvider::getInstance()->getProcessPage();			// ������ ������
	log("appendAlbum getProcessPage Finished  %d", nProgressPage);
	int nPageCount = KXMLReader::getInstance()->getPageCount();					// ��ü ������ ��
	for (int i = 0; i < nPageCount; i++) {
		HPAGE_INFO * pPageInfo = KXMLReader::getInstance()->getPageReference(i);  // xml���� ���� �˾�����
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
		if (isFindshed == "n"){  // ��å �ϵ������� ù �������� ���� ������ 
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

	 // ���� ��ư  ��ġ �Ѵ�
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

	// JTP �¿콺ũ���� JTP layer��, ���� �� exit ��ư�� popup layer�� �߰���Ų��. 
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





/*��ƼŬ �����Ѵ�.*/
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

	/*��ƼŬ ���� ��ġ�� �ʿ����� ����*/
	ACTION_PARAM* param = new ACTION_PARAM();
	param->autorelease();
	vtRefCountManager.pushBack(param);
	param->name = "thisisparticle";
	param->param = pContentInfo->ptype;   /*��ƼŬ Ÿ��*/
	param->param2 = pContentInfo->sound;  /*��ġ�� ��������*/
	sprite->setUserData((void*)param);

	CViewUtils::setSize(sprite, x, y, width, height);

	if (!pContentInfo->id.empty()) {
		sprite->setName(pContentInfo->id);
	}
	/*��ġ �̺�Ʈ ����*/
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
			if (!(parma->param2.empty() || parma->param2.length() == 0)) /*���尡 ������ ȿ���� �÷���*/
			{
				CocosDenshion::SimpleAudioEngine::getInstance()->playEffect(KCTT(parma->param2).c_str());
			}
			/*��ƼŬ ȿ�� �߻���Ű��*/
			generateParticleEffect(parma->param, Point(touch->getLocation().x, touch->getLocation().y), 0.3f);
			return true;
		}
		return false;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener1, sprite);
	if (pContentInfo->visible == "false") sprite->setVisible(false);
}

/*������ ���̿� �ش��ϴ� Node ����*/
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

/*��ƼŬ ȿ�� �߻�*/
void CPage::generateParticleEffect(string stype, Point pt, float duration ){
	
	string firststype = "";
	if (stype.empty() || stype.length() <2) firststype = "00";
	else {
		if (isItNumber(*stype.substr(0, 1).c_str()) && isItNumber(*stype.substr(1, 1).c_str()) ){
			firststype = stype.substr(0, 2);
		}
		else firststype = stype;
	}
		
	/*���ʷ����Ϳ� ��¡�� ��ȣ�� �G�� ����*/
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


/*���� ���� �׸���*/
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

/*������� refresh*/
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

/*Jump to page�� ���Ǵ� ��ư ����*/
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
		// ��ư�� �ҷ��´�.
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

				if (mSoundOn == button)  /*������ư �� ����*/
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
					/*ȿ������ ������ �÷���*/
					mAudioEngineID = playSound(param);// cocos2d::experimental::AudioEngine::play2d(param);
				}

				/*ȿ������ ������ �÷���*/
				//mAudioEngineID = playSound(param);// cocos2d::experimental::AudioEngine::play2d(param);
				// sound on/off�� ������� ȿ������ play �Ѵ�. 160830 kyh
				//mAudioEngineID = cocos2d::experimental::AudioEngine::play2d(param);
				/*�ش� �׼� ����*/
				command(action, "", "", sender);

				break;
			default:
				break;
			}
		});
	}
}

/*���(listen)��� �� �� pause��� ȭ�� ����*/
void CPage::appendBlankSprite()
{
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();

	/*��׶��� ����*/
	mBlankLayers = Layer::create();
	if (mpJtpButtonInfo != nullptr){
		appendButtonToBlankSprite(mpJtpButtonInfo);  /*Jump to Page ��ư ����*/
		appendButtonToBlankSprite(mpExitButtonInfo); // exit button 
	}

	m_pBlockingSprite = Sprite::create();
	m_pBlockingSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	m_pBlockingSprite->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	m_pBlockingSprite->setColor(Color3B(0, 0, 0));
	m_pBlockingSprite->setOpacity(0);
	mBlankLayers->addChild(m_pBlockingSprite);

	
	/*��� �÷��̹�ư ����*/
	mBtn = ui::Button::create("syscommon/viewer_btn_play_n.png",
		"syscommon/viewer_btn_play_p.png",
		"syscommon/viewer_btn_play_n.png");
	CViewUtils::setSize(mBtn, visibleSize.width / 2 - 95, visibleSize.height / 2 - 95, 190, 190);
	mBlankLayers->addChild(mBtn);
	mBtn->setVisible(false);
	mBtn->addClickEventListener([&](cocos2d::Ref* pSender) {
		/*�÷��� ��ư Ŭ���� ó�� pauseȭ�� ������*/
		mBtn->setVisible(false);
		m_pBlockingSprite->setOpacity(0);

		//CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
		
		playBGM();

		/*�����̼� ��������*/
		if (mAudioEngineID != AUDIOENGINE_UNDEFINED){
			cocos2d::experimental::AudioEngine::resume(mAudioEngineID);
		}
		else{
			if (mPlayNarrationPos != 0) mPlayNarrationPos--;
			playNarrationList();
		}
		
	});

	/*�Ʒ� ȭ�� �̺�Ʈ �ȹް� ó�� �� ���� ���� ����*/
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



/*Jump to page ����*/
/*layer�� ��� ����� ��� blanksprite�� ���� �־ blanksprite���� ��ġ�Ѵ�.*/
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
		// ��ư�� �ҷ��´�.
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



/*��� ����*/

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
