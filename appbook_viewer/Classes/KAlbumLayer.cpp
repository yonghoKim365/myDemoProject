#pragma warning(disable:4819)
#include "KAlbumLayer.h"

#include "Page.h"
#include "KDataProvider.h"
#include "strconvert.h"
#include "KPopupLayer.h"

/*Jump to page생성*/
KAlbumLayer* KAlbumLayer::create(string normal, string selected, Size dotSize, float dotY,  Layer* target, int curPage){

	KAlbumLayer * pAlbumLayer = new KAlbumLayer(normal, selected, dotSize, dotY,target, curPage);
	if (pAlbumLayer && pAlbumLayer->init()) {
		pAlbumLayer->autorelease();
		return pAlbumLayer;
	}
	CC_SAFE_DELETE(pAlbumLayer);
	return nullptr;
}
KAlbumLayer::KAlbumLayer() {
	KAlbumLayer("", "", Size(54.0f / 2, 54.0f / 2), (1426 / 2),nullptr, 0);
}

/*초기 화면 구성*/
KAlbumLayer::KAlbumLayer(string normal, string selected, Size dotSize, float dotY, Layer* target, int curPage) {
	mTotalItemCount = 0;
	mCurrentPageInPreview = 0;
	mCurrentPageInBook = curPage;
	mDotSize = Size(31,30); // dotSize;
	mDotY = dotY+5;
	mDotNormal = normal;
	mDotSelected = selected;

	mpDotParent = Node::create();
	addChild(mpDotParent);

	if (target == nullptr) return;
	
	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	
	/*
	m_pBlockingSprite = Sprite::create();
	m_pBlockingSprite->setTextureRect(Rect(0, 0, visibleSize.width, visibleSize.height));
	m_pBlockingSprite->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	m_pBlockingSprite->setColor(Color3B(0, 0, 255));
	m_pBlockingSprite->setOpacity(255);
	target->addChild(m_pBlockingSprite);
	m_pBlockingSprite->setVisible(false);
	*/
	
	
	// Swipe
	pEventListerBlock = EventListenerTouchOneByOne::create();
	pEventListerBlock->setSwallowTouches(false);
	pEventListerBlock->onTouchBegan = CC_CALLBACK_2(KAlbumLayer::onTouchBegan, this);
	pEventListerBlock->onTouchMoved = CC_CALLBACK_2(KAlbumLayer::onTouchMoved, this);
	pEventListerBlock->onTouchEnded = CC_CALLBACK_2(KAlbumLayer::onTouchEnded, this);
	pEventListerBlock->onTouchCancelled = CC_CALLBACK_2(KAlbumLayer::onTouchCancelled, this);

	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(pEventListerBlock, this);
	mVisibleSize = Director::getInstance()->getVisibleSize();
	mIsTouchDown = false;
	mInitialTouchPos.x = mInitialTouchPos.y = 0;
	
	KDataProvider::getInstance()->bJtpWarningPopupOpen = false;

	this->scheduleUpdate();

}

KAlbumLayer::~KAlbumLayer() {
	log("KAlbumLayer::~KAlbumLayer()");
}

void KAlbumLayer::setBlocked(bool bflag){
	pEventListerBlock->setSwallowTouches(bflag);
}

/*화면 구성요소 배치*/
void KAlbumLayer::addContent(string path,bool bDim){

	mTotalItemCount++;

	int nModCount = (mTotalItemCount - 1) % 9;

	float width = 268;// 288.0f;// 230.0f;
	float height = 172;// 180.0f;

	int _indexX = nModCount % 3;
	int _indexY = nModCount / 3;

	int x_gap = 41;// 27;
	int y_gap = 32;// 24;

	Size visibleSize = Director::getInstance()->getVisibleSize();
	int nPanelSizeW = width * 3 + x_gap * 2;
	int nPanelSizeH = height * 3 + y_gap * 2;

	int nPanelSizeX = (visibleSize.width - nPanelSizeW) / 2;
	int nPanelSizeY = (visibleSize.height - nPanelSizeH) / 2 + y_gap;

	// 1번째및 썸네일일 9개씩 추가할때마다 하단 dot을 추가한다. 
	if ( (mTotalItemCount -1 ) % MAX_CONTENT == 0) {
		KSpriteButton * pSpriteButton = KSpriteButton::create(false);
		mVectorPage.pushBack(pSpriteButton);
		addChild(pSpriteButton);

		Sprite * pDotSprite = Sprite::create(KCTV(mDotNormal));
		mpDotParent->addChild(pDotSprite);

		int nDotY = (visibleSize.height - (nPanelSizeH + nPanelSizeY)) / 2;
		nDotY += nPanelSizeH + nPanelSizeY - y_gap;
		int nX = mVectorDot.size() * mDotSize.width * 1.5;
		CViewUtils::setSize(pDotSprite, nX, nDotY, 20, 20);// 31, 30);
		mVectorDot.pushBack(pDotSprite);

		float width = (mDotSize.width*mVectorDot.size()) + ((mDotSize.width / 2)*(mVectorDot.size() - 1));
		float x1 = (CViewUtils::STAGE_WIDTH / 2) - (width / 2);
		float y1 = (CViewUtils::STAGE_HEIGHT - (mDotSize.height / 2) - mDotY);
		mpDotParent->setPosition(Vec2(x1, 0.0f));
	}
	


	float x = _indexX * width;
	float y = _indexY * height;
	char szTemp[100];
	char szTempDis[100];
	sprintf(szTemp, "%d", mTotalItemCount - 1);
	sprintf(szTempDis, "%d", mTotalItemCount);


	x += nPanelSizeX;// 140.0f;	// left base
	x += (_indexX * x_gap);	// gap
	
	y += nPanelSizeY;// 125.0f;	// top base
	y += (_indexY * y_gap);	// gap
	
	Sprite* tmbBg = appendImage(x, y, width, height, "syscommon/jumpto_thumb_bg2.png",-1);
	//Sprite* tmbNum = appendImage(x + 189, y + 141, 35, 31, "syscommon/jumpto_thumb_numer_bg.png", 1);

	//Label* label = Label::createWithSystemFont(szTempDis, sFontFile, 14, Size((int)35, (int)31), TextHAlignment::CENTER, TextVAlignment::CENTER);
	//Color3B color = ccc3(0, 0, 0);
	//label->setTextColor(Color4B(color));
	//CViewUtils::setSize(label, x + 189, y + 141, 35, 31);
	Sprite* imgPocus;
	if (mCurrentPageInBook == mTotalItemCount -1){
		//imgPocus = appendImage(x, y, width, height, "syscommon/jumptopage_img_pressed2.png", -1);
		imgPocus = appendImage(x, y, width, height, "syscommon/thumbnail_page_on.png", -1);
	}

	

	// thumbnail img
	x = x + 5;// 15;// 12;
	y = y + 5;// 13;
	width = 256;// 250;// 205;
	height = 160;// 156;// 153;
	Button* pButton = Button::create(KCTV(path), KCTV(path), KCTV(path));

	pButton->setName(szTemp);

	if (!bDim) {  /* 완독되면 페이지 자유롭게 이동 */
		pButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);
			switch (type)
			{
			case Widget::TouchEventType::BEGAN:
				button->setOpacity(170);
				break;
			case Widget::TouchEventType::CANCELED:
				button->setOpacity(255);
				break;
			case Widget::TouchEventType::ENDED: 
				button->setOpacity(255);
				playSound("common/snd/common_sfx_btn_01.mp3");
				int nCount = atoi(button->getName().c_str());
				//log("------------------------------> nCount=%d", nCount);
				int* page = new int[1]{nCount};
				Director::getInstance()->getEventDispatcher()->dispatchCustomEvent(COMMAND_SETPAGE, (void*)page);
				delete page;

				break;
			}
		});
	}
	else/* 완독이 안되어있으면 모든페이지 이동 안됨 (첫페이지 제외) */
	{
		pButton->addTouchEventListener([&](Ref* sender, Widget::TouchEventType type){
			Button* button = ((Button*)sender);
			switch (type)
			{
			case Widget::TouchEventType::BEGAN:
				button->setOpacity(170);
				break;
			case Widget::TouchEventType::CANCELED:
				button->setOpacity(255);
				break;
			case Widget::TouchEventType::ENDED:
				button->setOpacity(255);
				playSound("common/snd/common_sfx_btn_01.mp3");
				// 최초 1회 학습을 마쳐야만 원하는 페이지를 선택하여 이동할 수 있습니다. 
				if (KDataProvider::getInstance()->bJtpWarningPopupOpen == false){
					KDataProvider::getInstance()->bJtpWarningPopupOpen = true;
					KPopupLayer * pLayer = KPopupLayer::createLayer();
					this->addChild(pLayer, 9);
					pLayer->setJtpWarningCallback(
						[&](cocos2d::Ref*){
						setJtpWarningOff();
						}
					);
				}

				break;
			}
		});
	}

	mVectorPage.at(mVectorPage.size() - 1)->addChild(tmbBg);
	mVectorPage.at(mVectorPage.size() - 1)->addChild(pButton);
	//mVectorPage.at(mVectorPage.size() - 1)->addChild(tmbNum);
	//mVectorPage.at(mVectorPage.size() - 1)->addChild(label);
	if (mCurrentPageInBook == mTotalItemCount - 1){
		mVectorPage.at(mVectorPage.size() - 1)->addChild(imgPocus);
	}
	CViewUtils::setSize(pButton, x, y, width, height);
	
	refreshPosition();
	refreshDot();
	
}

void KAlbumLayer::setJtpWarningOff()
{
	KDataProvider::getInstance()->bJtpWarningPopupOpen = false;
}

void KAlbumLayer::playSound(std::string sFile)
{
	float fSoundVol = 0.0f;
	bool bSound = KDataProvider::getInstance()->getSoundState(); /*묵음모드인지 판별 (Jump to Page에서 묵음모드 설정가능)*/
	int nAudio;
	if (bSound)  /*정상모드*/
	{
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), false);
	}
	else {       /*묵음모드 :  묵음모드 라도 계속 진행 되어야 하므로 볼륨을 줄인다.*/
		nAudio = cocos2d::experimental::AudioEngine::play2d(sFile.c_str(), false, fSoundVol);
	}
}

/*페이지 표시 DOT*/
void KAlbumLayer::refreshDot() {
	for (int i = 0; i < mVectorDot.size(); i++) {
		if (mCurrentPageInPreview == i) {
			mVectorDot.at(i)->initWithFile(KCTV(mDotSelected));
		}
		else {
			mVectorDot.at(i)->initWithFile(KCTV(mDotNormal));
		}
	}
}


int KAlbumLayer::getTotalPage()
{
	return mVectorPage.size();
}

int KAlbumLayer::getCurrentPage()
{
	return mCurrentPageInPreview; // 0,1,2..
}

/*다음 페이지*/
void KAlbumLayer::next() {
	if (mCurrentPageInPreview < mVectorPage.size() - 1) {
		mCurrentPageInPreview++;
		refreshPosition();
	}
}

/*이전 페이지*/
void KAlbumLayer::prev() {
	if (mCurrentPageInPreview > 0) {
		mCurrentPageInPreview--;
		refreshPosition();
	}
}

void KAlbumLayer::setJtpPageToCurrentPage(int curPage){
	int targetPage = (curPage / MAX_CONTENT);

	mCurrentPageInPreview = targetPage;
	refreshPosition();
}

/*썸네일 화면이동시 재구성*/
void KAlbumLayer::refreshPosition() {
	float position = -1 * CViewUtils::STAGE_WIDTH * (mCurrentPageInPreview);
	for (int i = 0; i < mVectorPage.size(); ++i) {
		KSpriteButton * pSpriteButton = mVectorPage.at(i);
		pSpriteButton->setPositionX(position);
		
		position += CViewUtils::STAGE_WIDTH;
	}
	refreshDot();
}
/*현재 썸네일 페이지*/
int KAlbumLayer::selected() {
	return mCurrentPageInPreview;
}

/*이미지 생성*/
Sprite* KAlbumLayer::appendImage(float x1, float y1, float width1, float height1, std::string file, int depth) {

	float x = x1;
	float y = y1;
	float width = width1;
	float height = height1;

	Sprite* sprite = Sprite::create(file);
	if (sprite == nullptr) {
		log("Error::: from K. check the pass first [%s]", KCTV(file).c_str());
		return nullptr;
	}

	//this->addChild(sprite,depth);

	CViewUtils::setSize(sprite, x, y, width, height);

	return sprite;
}


bool KAlbumLayer::onTouchBegan(Touch *touch, Event *event) {
	mCurrentTouchPos = mInitialTouchPos = touch->getLocation();
	mIsTouchDown = true;
	return true;
}

void KAlbumLayer::onTouchMoved(Touch *touch, Event *event){
	mCurrentTouchPos = touch->getLocation();
}

void KAlbumLayer::onTouchEnded(Touch *touch, Event *event)
{
	mIsTouchDown = false;
}

void KAlbumLayer::onTouchCancelled(Touch *touch, Event *event)
{
	onTouchEnded(touch, event);
}

void KAlbumLayer::update(float dt)
{

	if (true == mIsTouchDown)
	{
		if (mInitialTouchPos.x - mCurrentTouchPos.x > mVisibleSize.width * 0.05)
		{
			log("SWIPED LEFT");

			mIsTouchDown = false;
			next();
			
		}
		else if (mInitialTouchPos.x - mCurrentTouchPos.x < -mVisibleSize.width * 0.05)
		{
			log("SWIPED RIGHT");

			mIsTouchDown = false;
			prev();
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
}

void KAlbumLayer::setParentPage(CPage* _page){
	mParentPage = _page;
}

void KAlbumLayer::appendButtonAlbum(float x1, float y1, float width1, float height1,
	std::string orgimg, std::string selimg, std::string disimg, std::string action){

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
		if (action == "ALBUMPREV") mParentPage->mBtnPrevPageInJtp = button;
		if (action == "ALBUMNEXT") mParentPage->mBtnNextPageInJtp = button;

		ACTION_PARAM* param = new ACTION_PARAM();
		vtRefCountManager.pushBack(param);
		param->autorelease();
		param->name = action;
		param->param = "";
		param->param2 = "";
		param->pContentInfo = nullptr;
		button->setUserData((void*)param);

		CViewUtils::setSize(button, x, y, width, height);

		this->addChild(button);

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
				
				cocos2d::experimental::AudioEngine::stop(mParentPage->getAudioEnginID());
				CocosDenshion::SimpleAudioEngine::getInstance()->stopEffect(mParentPage->getAudioEnginID());

				if (mSoundOn == button)  //묵음버튼 온 오프
				{
					// off -> on
					button->setVisible(false);
					mSoundOff->setVisible(true);
					 int n = cocos2d::experimental::AudioEngine::play2d("common/snd/common_sfx_btn_01.mp3");
					 mParentPage->setAudioEnginID(n);
				}
				else if (mSoundOff == button)
				{
					// on -> off
					button->setVisible(false);
					mSoundOn->setVisible(true);
					int n = cocos2d::experimental::AudioEngine::play2d("syscommon/Button.mp3");
					mParentPage->setAudioEnginID(n);
				}
				else{
					// 효과음이 있으면 플레이
					int n = mParentPage->playSound(param);// cocos2d::experimental::AudioEngine::play2d(param);
					mParentPage->setAudioEnginID(n);
				}

				// 해당 액션 실행
				mParentPage->command(action, "", "", sender);

				break;
			default:
				break;
			}
		});
	}
}