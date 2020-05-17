#include "AppOperator.h"
#include "KXMLReader.h"
#include "KDataProvider.h"
#include "KGlobalManager.h"
#include "EndScene.h"

#pragma warning(disable:4819)

USING_NS_CC;


/* AppOperator ���� */
AppOperator* AppOperator::create() {
	AppOperator* op = new AppOperator();
	if (op && op->init()) {
		return op;
	}
	return nullptr;
}

/* ������ ���� */
bool AppOperator::setPage(int index, bool isEffect = true)
{
	log("AppOperator::setPage 10");

	if (mppArrData == nullptr) return false;

	log("AppOperator::setPage 20");
	
	if (index >= mLength) {
		if (index == mLength){
			// insert end of page
			setEndPage();
		}
		return false;
	}
	if (index < 0) {
		return false;
	}

	log("AppOperator::setPage 30");

	/* �ش� �������� ���� �о� �´�. */
	HPAGE_INFO * pPageInfo = *(mppArrData + index);
	CPage* page = CPage::create(pPageInfo, mpReader->getBookInfo() );
	float transTime = 1.0f;
	if (isEffect) {
		bool backword = (mCurrentPage > index ? true : false);
		Scene * transition;
		string ptype = pPageInfo->ptype;
		string firstptype = "";
		if (ptype.empty() || ptype.length() < 2) firstptype = "00";
		else									 firstptype = ptype.substr(0, 2);
		/*������ �ѱ� ȿ�� ����*/
		if (firstptype == "00") transition = TransitionPageTurn::create(transTime, page, backword);
		else if (firstptype == "01") transition = TransitionCrossFade::create(transTime, page);
		else if (firstptype == "02") transition = backword ? TransitionMoveInL::create(transTime, page) : TransitionMoveInR::create(transTime, page);
		else if (firstptype == "03") transition = backword ? TransitionSlideInL::create(transTime, page) : TransitionSlideInR::create(transTime, page);
		else if (firstptype == "04") transition = TransitionFade::create(transTime, page);
		else if (firstptype == "05") transition = TransitionTurnOffTiles::create(transTime, page);
		else if (firstptype == "06") transition = TransitionSplitCols::create(transTime, page);
		else if (firstptype == "07") transition = TransitionSplitRows::create(transTime, page);
		else if (firstptype == "08") transition = backword ? TransitionFadeTR::create(transTime, page) : TransitionFadeBL::create(transTime, page);
		else if (firstptype == "09") transition = backword ? TransitionFadeUp::create(transTime, page) : transition = TransitionFadeDown::create(transTime, page);
		else if (firstptype == "10") transition = backword ? TransitionProgressRadialCCW::create(transTime, page) : transition = TransitionProgressRadialCW::create(transTime, page);
		else if (firstptype == "11") transition = TransitionProgressHorizontal::create(transTime, page);
		else if (firstptype == "12") transition = backword ? TransitionProgressInOut::create(transTime, page) : transition = TransitionProgressOutIn::create(transTime, page);
		else if (firstptype == "13") transition = TransitionFade::create(transTime, page, Color3B::WHITE);
		else if (firstptype == "14") transition = backword ? CCTransitionFlipX::create(transTime, page, tOrientation::LEFT_OVER) : transition = CCTransitionFlipX::create(transTime, page, tOrientation::RIGHT_OVER);
		else if (firstptype == "15") transition = backword ? CCTransitionFlipY::create(transTime, page, tOrientation::UP_OVER) : transition = CCTransitionFlipY::create(transTime, page, tOrientation::DOWN_OVER);
		else {
			transition = nullptr;
		}

		log("AppOperator::setPage 40");
		if (transition == nullptr) Director::getInstance()->replaceScene(page);
		else Director::getInstance()->replaceScene(transition);
		
	}
	else {

		log("AppOperator::setPage 50");

		Director::getInstance()->replaceScene(page);
	}
	mCurrentPage = index;
	return true;
}

/* ������ ���� */
bool AppOperator::setEndPage()
{
	
	bool isEffect = true;

	Scene* page = EndScene::createScene();

	float transTime = 1.0f;
	if (isEffect) {
		bool backword = false;// (mCurrentPage > index ? true : false);
		Scene * transition;
		
		// /*������ �ѱ� ȿ�� ����
		transition = TransitionPageTurn::create(transTime, page, backword);
		
		if (transition == nullptr) Director::getInstance()->replaceScene(page);
		else Director::getInstance()->replaceScene(transition);
	}
	else {
		Director::getInstance()->replaceScene(page);
	}
	mCurrentPage = 999;// index;
	
	return true;
	
}


/* �̺�Ʈ �ʱ�ȭ */
bool AppOperator::init()
{
	/* Global Event ��� */
	addEventListener(COMMAND_SETPAGE, CC_CALLBACK_1(AppOperator::onSetPage, this));
	addEventListener(COMMAND_PREV, CC_CALLBACK_0(AppOperator::onPrevPage, this));
	addEventListener(COMMAND_NEXT, CC_CALLBACK_0(AppOperator::onNextPage, this));
	addEventListener(COMMAND_PREV_NOPAGING, CC_CALLBACK_0(AppOperator::onPrevPageNopaging, this));
	addEventListener(COMMAND_NEXT_NOPAGING, CC_CALLBACK_0(AppOperator::onNextPageNopaging, this));
	 
	return true;
}

/* ������ ���� �⺻ ���� */
void AppOperator::requestInitProcess() {
	
	mpReader = KXMLReader::getInstance();
	mLength = mpReader->getPageCount();


	//������ ���� ������ ����
	mppArrData = new HPAGE_INFO*[mLength];
	for (int i = 0; i < mLength; i++) {
		*(mppArrData + i) = mpReader->getPageReference(i);
	}
}
 
/* Global Event ��� */
void AppOperator::addEventListener(const std::string& eventName, const std::function<void(EventCustom*)>& callback)
{
	Director::getInstance()->getEventDispatcher()->addCustomEventListener(eventName, callback);	
}

/* �ش� �������� �̵� */
void AppOperator::onSetPage(EventCustom* event) {
	int* index = (int*)event->getUserData();
	setPage(*index);
}

/* ���� �������� �̵� */
void AppOperator::onPrevPage() {
	setPage(mCurrentPage - 1);
	
}

/* ���� �������� �̵� */
void AppOperator::onNextPage() {
	setPage(mCurrentPage + 1);
}

/* ���� �������� �̵� �ѱ�ȿ�� ���� */
void AppOperator::onPrevPageNopaging() {
	setPage(mCurrentPage - 1, false);
}

/* ���� �������� �̵� �ѱ�ȿ�� ���� */
void AppOperator::onNextPageNopaging() {
	setPage(mCurrentPage + 1, false);
}

/* AppOperator ����  */
AppOperator::AppOperator() {
	mpReader = nullptr;
	mppArrData = nullptr;
}

/* AppOperator �Ҹ�  */
AppOperator::~AppOperator() {
	if (mppArrData != nullptr) {
		delete [] mppArrData ;
	}
	if (mpReader != nullptr) {
		delete mpReader;
		mpReader = nullptr;
	}
	KGlobalManager::removeInstance();
}
