#include "KRecordingScene.h"
#include "KXMLReader.h"
#include "KDataProvider.h"

#include "KRecordingManager.h"
#include "strconvert.h"

USING_NS_CC;
#include "ui/CocosGUI.h"

#include "Page.h"

/*녹음 레이어 생성*/
Scene* KRecordingLayer::createScene(){
    auto scene = Scene::create();
	auto layer = KRecordingLayer::create();
    scene->addChild(layer);
    return scene;
}

// on "init" you need to initialize your instance
bool KRecordingLayer::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }

	std::string sPath = StringUtils::format("%s/%s", KRecordingManager::getInstance()->getAndroidTargetPath().c_str()
		, KXMLReader::getInstance()->getBookInfo()->projectcode.c_str());

	KRecordingManager::getInstance()->makesureFolderExists(sPath);
	KRecordingManager::getInstance()->setDelegate(this);
    
    Size visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();

	mLayerRecord = Layer::create();
	this->addChild(mLayerRecord, 10);

	//레코딩에 관련된 UI정의하기.
	constructUIForRecord();

	mpReader = KXMLReader::getInstance();

	mLayerContent = Layer::create();
	this->addChild(mLayerContent , 1);

	mCurIndex = 0;
	mDataLength = mpReader->getPageCount();

	if (canGoFurther(this->mCurIndex)) {
		addAndRemoveChildren();
	}


	// Blocking을 위해서.. 작업함.
	m_pBlockingSprite = Sprite::create();
	m_pBlockingSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	m_pBlockingSprite->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	m_pBlockingSprite->setColor(Color3B(0, 0, 0));
	m_pBlockingSprite->setOpacity(50);
	this->addChild(m_pBlockingSprite,1000);
//	m_pBlockingSprite->setVisible(false);

	pEventListerBlock = EventListenerTouchOneByOne::create();
	pEventListerBlock->setSwallowTouches(false);
	pEventListerBlock->onTouchBegan = [&](Touch * touch, Event * event) ->bool {
		return true;
	};
	_eventDispatcher->addEventListenerWithSceneGraphPriority(pEventListerBlock, m_pBlockingSprite);

	refreshRecordControlUI();
    return true;
}

bool KRecordingLayer::canGoFurther(int nTargetIndex){
	if (nTargetIndex < 0) return false;
	if (nTargetIndex >= mDataLength) return false;
	return true;
}

/*녹음 페이지 기존거 지우고 페이지별 레이어 생성*/
void KRecordingLayer::addAndRemoveChildren(){
	Size visibleSize = Director::getInstance()->getVisibleSize();

	for (mIter = mLayers.begin(); mIter != mLayers.end(); ) {
		int nDifference = mCurIndex - (*mIter)->getTag();
		if (nDifference > 1 || nDifference < -1) {
			mLayerContent->removeChild( *mIter );
			mIter = mLayers.erase(mIter);
		}
		else {
			mIter++;
		}
	}
	/*녹음할 페이지별 레이어 구성*/
	for (int i = -1; i <= 1; i++) {
		int nIndex = mCurIndex + i;
		if (nIndex < 0) continue;
		if (nIndex >= mDataLength) continue;

		bool isFound = false;
		for (mIter = mLayers.begin(); mIter != mLayers.end(); mIter++) {
			if ((*mIter)->getTag()  == nIndex) {
				isFound = true;
				break;
			}
		}
		if (isFound == true) continue;

		//자여기서 등록을 하자.. 
		auto layer = Layer::create();
		layer->setTag(nIndex);
		layer->setPosition(Vec2(visibleSize.width * nIndex, 0));
		mLayers.pushBack(layer);
		mLayerContent->addChild(layer);

		constructUIForPages(layer, nIndex);
	}
	
}


//레코딩에 관련된 UI정의하기.
void KRecordingLayer::constructUIForRecord() {
	HPOPUP_INFO * pPopupInfo = KXMLReader::getInstance()->getPopupReference("pagerecord");
	if (pPopupInfo == nullptr) {
		log("There is no Identity for pagerecord..");
		log("Can't make Referenced hpopup_info(pagerecord)..");
		return;
	}
	
	CPage::setBackground(pPopupInfo->stBackground, mLayerRecord);
	for (int j = 0; j < pPopupInfo->vtContents.size(); ++j) {
		STCONTENT_INFO *  pContentInfo = pPopupInfo->vtContents.at(j);

		if (pContentInfo->type == "action") {
			appendButton(pContentInfo, mLayerRecord);
		}
		else if (pContentInfo->type == "image") {
			CPage::appendImage(pContentInfo, mLayerRecord);
		}
		else if (pContentInfo->type == "text") {
			CPage::appendText(pContentInfo, mLayerRecord);
		}
	}
	m_pProgressSprite = (Sprite *)mLayerRecord->getChildByName(KRecordingManager::getInstance()->getActionTargetName());
	if (m_pProgressSprite != nullptr) {
		Vec2 pos = m_pProgressSprite->getPosition();
		Size size = m_pProgressSprite->getContentSize();
		
		m_pProgressSprite->setPosition(Vec2(pos.x - size.width / 2, pos.y + size.height / 2));
		m_pProgressSprite->setAnchorPoint(Vec2(0, 1));
	}
}

/*action 버튼 생성*/
void KRecordingLayer::appendButton(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target){
	string image = pContentInfo->image;
	string select = pContentInfo->selected;
	string disabled = pContentInfo->disabled;

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	string sActionTarget = pContentInfo->action.param;
	if (!sActionTarget.empty() && pContentInfo->action.name == COMMAND_RECORDING) {
		log("%s", sActionTarget.c_str());
		KRecordingManager::getInstance()->setActionTargetName(sActionTarget);
	}

	Button * button = nullptr;
	if (!image.empty()) {
		// 버튼을 불러온다.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;

		button = Button::create(KCTV(image), KCTV(select), KCTV(disabled));
		if (pContentInfo->visible == "false") button->setVisible(false);

		ACTION_PARAM* param = new ACTION_PARAM();
		param->autorelease();
		vtRefCountManager.pushBack(param);
		param->name = pContentInfo->action.name;
		param->param = pContentInfo->action.param;
		param->param2 = pContentInfo->action.param2;
		button->setUserData((void*)param);

		CViewUtils::setSize(button, x, y, width, height);

		target->addChild(button);


		if (!pContentInfo->id.empty()) {
			button->setName(pContentInfo->id);
		}

		button->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);
			//button->getParent();
			
			ACTION_PARAM* parma = (ACTION_PARAM*)button->getUserData();

			float scaleX = button->getScaleX();
			float scaleY = button->getScaleY();
			string action = parma->name;
			string param = parma->param;
			string param2 = parma->param2;

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

				command(action, param, param2, sender);
				break;
			default:
				break;
			}
		});
	}

	if (pContentInfo->action.name == COMMAND_RECORDING) {
		m_btnRecord = button;
	}
	if (pContentInfo->action.name == COMMAND_RECORDPLAY) {
		m_btnPlay = button;
	}
	if (pContentInfo->action.name == COMMAND_RECFILEDEL) {
		m_btnDelete = button;
	}
	if (pContentInfo->action.name == COMMAND_RECORDSTOP) {
		m_btnRecordStop = button;
	}
	
}

/*녹음 화면 command 제어*/
void KRecordingLayer::command(string command, string param, string param2, Ref * sender) {
	if (command.empty()) {
		return;
	}
	if (m_btnRecord != nullptr) {
		bool isrec = m_btnRecord->isEnabled();  // 녹음중이고 
		if (isrec == false && command != COMMAND_RECORDSTOP) // 녹음 중지 아니면 리턴 (녹음중에는 event무시)
			return;
	}
	if (command == COMMAND_RECORDEXIT) {     /*녹음 종료*/
		Director::getInstance()->popScene();
		int* page = new int[1]{0};
		Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
		delete page;
	}
	else if (command == COMMAND_RECORDPLAY) {  /*녹음된 파일 듣기*/
		
		if (FileUtils::getInstance()->isFileExist(getFullPathName())){
			this->showNoInputPopup();
			KRecordingManager::getInstance()->startListening(getFullPathName());
			if (m_pProgressSprite != nullptr) {
				m_pProgressSprite->setScaleX(0.f);
				m_pProgressSprite->runAction(ScaleTo::create(KRecordingManager::getInstance()->getDuration() * 0.001f, 1.f));
				m_pProgressSprite->setVisible(true);
			}
		}
		else {
			log("COMMAND_RECORDPLAY  file is not exists..");
		}
	}
	else if (command == COMMAND_RECORDING) {  /*녹음 시작*/
		
		this->showNoInputPopup();
		KRecordingManager::getInstance()->setRecordingFilename( getFullPathName() );
		KRecordingManager::getInstance()->startRecording();
		log("recButton is Clicked...");
	}
	else if (command == COMMAND_RECFILEDEL) {   /*녹음된 파일 삭제*/
		if (FileUtils::getInstance()->isFileExist(getFullPathName())){
			FileUtils::getInstance()->removeFile(getFullPathName());
			this->refreshRecordControlUI();
			
			KRecordingManager::getInstance()->showToastForJNI(__TX("삭제하였습니다."));
		}
		else {
			log("COMMAND_RECFILEDEL  file is not exists..");
		}

		log("COMMAND_RECFILEDEL");
	}
	else if (command == COMMAND_RECORDSTOP) {  /*녹음 중지*/
		KRecordingManager::getInstance()->stopRecording();
	}
	else if (command == COMMAND_PREV) {        /*이전화면 이동*/
		if (this->canGoFurther(this->mCurIndex - 1)) {
			this->mCurIndex--;
			this->addAndRemoveChildren();
		}
		else {
			log("can't go further.");
			return;
		}
		
		Size visibleSize = Director::getInstance()->getVisibleSize();
		mLayerContent->runAction(
			Sequence::create(
			CallFunc::create([&]() {
			this->showNoInputPopup();
		}),
			MoveTo::create(0.3f, Vec2(this->mCurIndex * visibleSize.width * -1, 0))
			,
			CallFunc::create([&]() {
			this->hideNoInputPopup();
			this->refreshRecordControlUI();
		}), nullptr
			)
			);
	}
	else if (command == COMMAND_NEXT) {   /*다음 화면 이동*/
		if (this->canGoFurther(this->mCurIndex + 1)) {
			this->mCurIndex++;
			this->addAndRemoveChildren();
		}
		else {
			log("can't go further.");
			return;
		}
		Size visibleSize = Director::getInstance()->getVisibleSize();
		mLayerContent->runAction(
			Sequence::create(
			CallFunc::create([&]() {
			this->showNoInputPopup();
				
			}),
				MoveTo::create(0.3f, Vec2(this->mCurIndex * visibleSize.width * -1, 0))
				,
				CallFunc::create([&]() {
					this->hideNoInputPopup();
					this->refreshRecordControlUI();
				}), nullptr
			)
			);
	}

	log("command %s", command.c_str());
}
void KRecordingLayer::showNoInputPopup(){
//	pEventListerBlock->setSwallowTouches(true);
}
void KRecordingLayer::hideNoInputPopup(){
//	pEventListerBlock->setSwallowTouches(false);

}

/*녹음화면 구성*/
void KRecordingLayer::constructUIForPages(cocos2d::Layer * layer, int nIndex){
	HPAGE_INFO* pPageInfo = mpReader->getPageReference(nIndex);

	auto background = Layer::create();
	layer->addChild(background);

	auto content = Layer::create();
	layer->addChild(content);

	auto uiobject = Layer::create();
	layer->addChild(uiobject);

	STBACKGROUND backgroundInfo = pPageInfo->stBackground;
	CPage::setBackground(backgroundInfo, background);

	for (int i = 0; i < pPageInfo->vtContents.size(); ++i) {
		STCONTENT_INFO *  pContentInfo = pPageInfo->vtContents.at(i);

		if (pContentInfo->type == "action") {
			appendButtonForRecord(pContentInfo, uiobject);
		}
		else if (pContentInfo->type == "animation") {
			appendAnimationForRecord(pContentInfo, content);
		}
		else if (pContentInfo->type == "image") {
			appendImageForRecord(pContentInfo, content);
		}
		else if (pContentInfo->type == "text") {
			appendTextForRecord(pContentInfo, content);
		}
	}

}

/*action 버튼 생성*/
void KRecordingLayer::appendButtonForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target){
	string image = pContentInfo->image;
	string select = pContentInfo->selected;
	string disabled = pContentInfo->disabled;

	float x = pContentInfo->x;
	float y = pContentInfo->y;
	float width = pContentInfo->width;
	float height = pContentInfo->height;

	if (!image.empty()) {
		// 버튼을 불러온다.
		if (select.empty()) select = image;
		if (disabled.empty()) disabled = image;

		auto button = Button::create(KCTV(image), KCTV(select), KCTV(disabled));
		if (pContentInfo->visible == "false") button->setVisible(false);
		CViewUtils::setSize(button, x, y, width, height);

		target->addChild(button);
		button->setEnabled(false);
	}
}

/*애니메이션 구성*/
void KRecordingLayer::appendAnimationForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target){
	int length = pContentInfo->vtAnimation.size();
	for (int i = 0; i < 1; i++)
	{
		Sprite * pAnimSprite = Sprite::create(KCTV(pContentInfo->vtAnimation.at(i)->image));
		Size sizeAni = pAnimSprite->getContentSize();
		if (sizeAni.width == 0 || sizeAni.height == 0) continue;
		pAnimSprite->setScaleX(pContentInfo->vtAnimation.at(i)->width / sizeAni.width);
		pAnimSprite->setScaleY(pContentInfo->vtAnimation.at(i)->height / sizeAni.height);
	}
	float fDelay = atof(pContentInfo->delay.c_str()) * 0.001;

	pContentInfo->image = pContentInfo->vtAnimation.at(0)->image;
	Sprite* sprite = CPage::appendImage(pContentInfo, target);
	sprite->setName("animation");

	if (pContentInfo->visible == "false") sprite->setVisible(false);
}

/*이미지 생성*/
void KRecordingLayer::appendImageForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target){
	CPage::appendImage(pContentInfo, target);
}

/*텍스트 생성*/
void KRecordingLayer::appendTextForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target){
	CPage::appendText(pContentInfo, target);
}



//녹음하고 녹음듣기 부분
void KRecordingLayer::onRecordingStarted(){
	this->showNoInputPopup();
	if (m_btnRecord != nullptr) {
		m_btnRecord->setEnabled(false);
	}
	if (m_pProgressSprite != nullptr) {
		m_pProgressSprite->setScaleX(0.f);
		m_pProgressSprite->runAction(ScaleTo::create(KRecordingManager::getInstance()->getDuration() * 0.001f, 1.f));
		m_pProgressSprite->setVisible(true);
	}
}

/*녹음 종료*/
void KRecordingLayer::onRecordingEnded(){
	this->hideNoInputPopup();
	if (m_btnRecord != nullptr) {
		m_btnRecord->setEnabled(true);
	}
	if (m_pProgressSprite != nullptr) {
		m_pProgressSprite->setVisible(false);
	}
	refreshRecordControlUI();
}

/*듣기 종료*/
void KRecordingLayer::onListeningEnded(){
	this->hideNoInputPopup();
}

/*파일이름 만든기*/
std::string KRecordingLayer::getFullPathName(){
	return StringUtils::format("%s/%d.mp3", KRecordingManager::getInstance()->getRecordFullPath().c_str(), mCurIndex);
}

/*화면 재구성*/
void KRecordingLayer::refreshRecordControlUI(){
	if (FileUtils::getInstance()->isFileExist(getFullPathName())) {
		if (m_btnPlay != nullptr) {
			m_btnPlay->setEnabled(true);
		}
		if (m_btnDelete != nullptr) {
			m_btnDelete->setEnabled(true);
		}
	}
	else {
		if (m_btnPlay != nullptr) {
			m_btnPlay->setEnabled(false);
		}
		if (m_btnDelete != nullptr) {
			m_btnDelete->setEnabled(false);
		}
	}
}