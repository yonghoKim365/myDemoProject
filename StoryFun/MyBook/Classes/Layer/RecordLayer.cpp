#include "RecordLayer.h"
#include "Contents/MyBookSnd.h"
#include "CCFileUtils.h"

#include  "Manager/ScreenManager.h"

// for AudioEngine
using namespace cocos2d::experimental;

Layer* RecordLayer::createLayer(Node* parent, int weekData, int ord, std::string filename)
{
	// 'layer' is an autorelease object
	auto layer = RecordLayer::create();
	layer->mParent = parent;
	layer->mWeekData = weekData;
	layer->mOrderNum = ord;
	layer->mFileName = filename;
	layer->mIsAutoMode = false;

	log("INIT [%d, %d]", layer->mWeekData, layer->mOrderNum);

	layer->initLayout();
	layer->createUi();

	// 녹음전 처리과정
	//layer->preprocessRecord();
	layer->processAutoMode();
	layer->processUserTouch();
	// return the scene
	return layer;
}


/*
0. 녹음중일때 정지 버튼 클릭시
 - 녹음된 내용 삭제
 - 녹음전 상태로 변경

1. 녹음전 툴바(최초)
녹음버튼, 스피커버튼 활성화, 재생버튼과 저장버튼은 비활성화

2. 녹음중
  2.1 원어민 나레이션중 모든 버튼 비활성화
  2.2 녹음기능 시작
     -> 녹음버튼은 정지버튼으로 변경(효과음 송출~삐~ 아이목소리 녹음)
	 -> 프로그레스바 시작(10초)
	 -> 완료후 자동 재생, 휴지통 활성화(원어민, 아이목소리)
3. 녹음후
  3.1 재생
*/

/*
* 녹음 플로우 수정
* 
*/


// on "init" you need to initialize your instance
bool RecordLayer::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}

	// 보이스 레코드
	initVoiceRecordManager();	
	
	return true;
}


/**
* @brief  레코드 레이어 정보 변경
*/
void RecordLayer::nextRecordLayer(int ordNum, std::string filename)
{	
	// 초기화
	mIsTouchPlayBtn = false;
	mIsClose = false;
	mRecordSndId = INVALID_INDEX;
	// 진행중인 정보 멈춤		
	initProgressBar();
	
	// 프로그래스바 및 저장중인 경우 처리
	// 녹음중일때 버튼 클릭시 현재 녹음중이던 데이터는 삭제됨
	//stopRecord();

	//this->runAction(Sequence::create(
	//	DelayTime::create(0.05f),
	//	CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::delayCloseCallbackDone)),	
	//	nullptr));
	// 권별 정보 반영
	mOrderNum = ordNum;
	mFileName = filename;
	CCLOG("=============NEXT RECORD===============");
	CCLOG("WEEK : %d, ORDER: %d, mFileName: %s", mWeekData, mOrderNum, mFileName.c_str());
	// 권별 정보 반영하여 버튼 설정
	setMode(RECORD_MODE::INIT);
	
	// 나레이션 파일 명 설정
	setNarFileName();

	auto director = Director::getInstance();
	// 나레이션 텍스트 설정
	Sprite* spText = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::TEXT));
	CC_ASSERT(spText != nullptr);
	/*int oNum = (mWeekData - 2) * RECORD_TEXT_CNT + mOrderNum;
	std::string strText = StringUtils::format(FILENAME_RECORD_TEXT_X, mWeekData, mWeekData, oNum);*/
	std::string strText = getStringRecordNarrationText();
	spText->setTexture(director->getTextureCache()->addImage(strText.c_str()));

	// 기존 저장된 이미지가 존재하는 경우
	if (isExistDrawFile())
	{
		CCLOG("nextRecordLayer isExistDrawFile is true");
		// 그림 및 배경 설정
		// 배경 설정
		Sprite* spCubeBg = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::CUBE_BG));
		CC_ASSERT(spCubeBg != nullptr);
		spCubeBg->setTexture(director->getTextureCache()->addImage(mFileName.c_str()));

		// 사용자 그림 설정
		Sprite* spCube = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::DRAW));		
		if (spCube == nullptr)
		{			
			setDrawLayer();
		}
		else
		{
			CCLOG("nextRecordLayer spCube is not null");
			spCube->setTexture(director->getTextureCache()->addImage(
						MBJson::getInstance()->getDrawFileName(mWeekData, mOrderNum).c_str()));
			// 스티커 이미지
			Sprite* spCubeSticker = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::STICKER));
			if (spCubeSticker != nullptr)
			{
				spCubeSticker->setTexture(director->getTextureCache()->addImage(
					MBJson::getInstance()->getDrawStickerFileName(mWeekData, mOrderNum).c_str()));
			}
			// 먹선 이미지
			Sprite* spCubeLine = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::LINE));
			if (spCubeLine != nullptr)
			{
				spCubeLine->setTexture(director->getTextureCache()->addImage(
					MBJson::getInstance()->getDrawLineImageFileName(mOrderNum).c_str()));
			}
		}
	}
	else
	{
		// 사용자 그림 설정 지우기
		Sprite* spCube = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::DRAW));
		if (spCube != nullptr)
		{
			this->removeChildByTag(TAG_RECORD::DRAW);
		}		
		// 스티커 이미지 지우기
		Sprite* spCubeSticker = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::STICKER));
		if (spCubeSticker != nullptr)
		{
			this->removeChildByTag(TAG_RECORD::STICKER);
		}	
		
		Sprite* bgCube = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::CUBE_BG));		
		bgCube->setTexture(director->getTextureCache()->addImage(MBJson::getInstance()->getDrawColorImageFileName(mOrderNum).c_str()));

		// 고정형 이미지가 아닐경우
		if (MBJson::getInstance()->mLockCubes[mOrderNum] == false)
		{
			Sprite* bgCubeLine = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::LINE));
			if (bgCubeLine == nullptr)
			{
				// 먹선 이미지 추가				
				Sprite* spLine = Sprite::create(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum));
				spLine->setPosition(Pos::getRecordLayerCubeBG());
				spLine->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
				float ratio = getRatioDrawImage(spLine->getContentSize());
				spLine->setScale(ratio);
				this->addChild(spLine, CanvasLayerOrder::kDepthCanvasLine, TAG_RECORD::LINE);
			}
			else
			{
				bgCubeLine->setTexture(director->getTextureCache()->addImage(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum).c_str()));
			}			
		}		
		else
		{
			// 먹선 이미지 지우기
			Sprite* spCubeLine = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::LINE));
			if (spCubeLine != nullptr)
			{
				this->removeChildByTag(TAG_RECORD::LINE);
			}
		}
	}

	// 넥스트 버튼 처리
	setNextBtn();

	// Auto Mode
	processAutoMode();
}


void RecordLayer::setNextBtn()
{
	// 넥스트 버튼 처리
	int idx = INVALID_INDEX;
	auto json = MBJson::getInstance();
	for (int i = mOrderNum; i <= SIZE_CUBE; i++)
	{
		if (json->getCubeSuccessIndex(i))
		{
			idx = i;
			CCLOG("NEXT BTN INDEX BREAK(SET INDEX) : %d", idx);
			break;
		}
		//CCLOG("FOR NEXT BTN INDEX[%d] : %d", i, idx);
	} // end of for

	//CCLOG("NEXT BTN INDEX : %d", idx);
	if (idx == INVALID_INDEX)
	{		
		setVisibleForNextBtn(false);
	}
	else
	{	
		setVisibleForNextBtn(true);
	}
}


// 보이스 관리자 초기화
void RecordLayer::initVoiceRecordManager()
{
	mRecordManager = VoiceRecordManager::getInstance();	
}


/**
* @brief  녹음한 이력 확인용 파일 이름
* @return  string
*/
std::string RecordLayer::getRecordFileName()
{
	std::string strPath = FileUtils::getInstance()->getWritablePath();
	strPath = StringUtils::format("%s", FILENAME_SND_RECORD_VOICE_BASE_X);
	std::string strFileName = StringUtils::format(FILENAME_SND_RECORD_VOICE_X, mWeekData, mOrderNum);
	std::string strFullPath = strPath + strFileName;
	return (strFullPath);
}


/**
* @brief  그림 그린 이력이 있을경우
* @return  bool
*/
bool RecordLayer::isExistDrawFile()
{
	bool flag;	
	if (FileUtils::getInstance()->isFileExist(MBJson::getInstance()->getDrawFileName(mWeekData, mOrderNum)))
	{
		// 존재하는 경우		
		flag = true;
	}
	else
	{
		// 존재하지 않는 경우
		flag = false;
	}
	
	return flag;
}

/**
* @brief  그림 그린 이력이 있을경우
* @return  bool
*/
bool RecordLayer::isExistDrawStickerFile()
{
	bool flag;
	if (FileUtils::getInstance()->isFileExist(MBJson::getInstance()->getDrawStickerFileName(mWeekData, mOrderNum)))
	{
		// 존재하는 경우		
		flag = true;
	}
	else
	{
		// 존재하지 않는 경우
		flag = false;
	}

	return flag;
}


/**
* @brief  녹음한 이력이 있을경우
* @return  bool
*/
bool RecordLayer::isExistRecordFile()
{
	bool flag;
	if (FileUtils::getInstance()->isFileExist(getRecordFileName()))
	{
		// 존재하는 경우
		flag = true;
	}
	else
	{
		// 존재하지 않는 경우
		flag = false;
	}

	return flag;
}


/**
* @brief  이력이 있을경우
* @return  bool
*/
bool RecordLayer::isExistRecordHistory()
{		
	return UserDefault::getInstance()->getBoolForKey(StringUtils::format(USERDEFAULT_KEY_RECORD_HISTORY_X, mWeekData).c_str(), false);;
}


void RecordLayer::setNarFileName()
{
	////////////////////////////////////////
	// 원어민 나레이션 파일 네임
	std::string strNarFilePath = StringUtils::format(FILENAME_SND_RECORD_TEXT_BASE_X);
	std::string strNarFileName = StringUtils::format(FILENAME_SND_RECORD_TEXT_X, mWeekData, mOrderNum);
	mNarFileName = strNarFilePath + strNarFileName;
	
	CCLOG("PLAY.... NARRATION.........FILENAME: %s", mNarFileName.c_str());
}


/**
* @brief  그림이 존재하는 경우 표시
* @return  void
*/
void RecordLayer::setDrawLayer()
{
	CCLOG("setDrawLayer ......");
	// 캔버스 이미지
	auto brushTexture = TextureCache::getInstance()->addImage(
		MBJson::getInstance()->getDrawFileName(mWeekData, mOrderNum).c_str());
	Sprite* brush = Sprite::createWithTexture(brushTexture);

	Size screenSize = Pos::getScreenSize();
	Size size = brush->getContentSize();

	float x1 = screenSize.width * (size.width / SCREEN_SIZE_WIDTH);
	float x2 = screenSize.width * (RECORD_SIZE_CUBE / SCREEN_SIZE_WIDTH);
	float ratio = (1.0f / x1) * x2;

	brush->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	brush->setPosition(Pos::getRecordLayerCubeBG());
	brush->setScale(ratio);
	this->addChild(brush, CanvasLayerOrder::kDepthCanvasDraw, TAG_RECORD::DRAW);
	TextureCache::getInstance()->removeTexture(brushTexture);

	// 파일 존재유무
	if (isExistDrawStickerFile())
	{		
		////////////////////////////////////////		
		// 캔버스 이미지
		auto brushTextureSticker = TextureCache::getInstance()->addImage(
			MBJson::getInstance()->getDrawStickerFileName(mWeekData, mOrderNum).c_str());
		Sprite* brushSticker = Sprite::createWithTexture(brushTextureSticker);
		
		Size size = brushSticker->getContentSize();
		x1 = screenSize.width * (size.width / SCREEN_SIZE_WIDTH);
		x2 = screenSize.width * (RECORD_SIZE_CUBE / SCREEN_SIZE_WIDTH);
		float ratioSticker = (1.0f / x1) * x2;

		brushSticker->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
		brushSticker->setPosition(Pos::getRecordLayerCubeBG());
		brushSticker->setScale(ratioSticker);
		this->addChild(brushSticker, CanvasLayerOrder::kDqpthCanvasSticker, TAG_RECORD::STICKER);

		TextureCache::getInstance()->removeTexture(brushTextureSticker);		
	}

	Sprite* bgCubeLine = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::LINE));
	if (bgCubeLine == nullptr)
	{
		// 먹선 이미지 추가				
		Sprite* spLine = Sprite::create(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum));
		spLine->setPosition(Pos::getRecordLayerCubeBG());
		spLine->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
		float ratio = getRatioDrawImage(spLine->getContentSize());
		spLine->setScale(ratio);
		this->addChild(spLine, CanvasLayerOrder::kDepthCanvasLine, TAG_RECORD::LINE);
	}
	else
	{
		bgCubeLine->setTexture(Director::getInstance()->getTextureCache()->addImage(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum).c_str()));
	}

	//// 먹선 이미지
	//Sprite* spLine = Sprite::create(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum));
	//spLine->setPosition(Pos::getRecordLayerCubeBG());
	//spLine->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	//spLine->setScale(ratio);
	//this->addChild(spLine, 0, TAG_RECORD::LINE);
}


// 나레이션 텍스트
std::string RecordLayer::getStringRecordNarrationText()
{
	//int oNum = (mWeekData - 2) * RECORD_TEXT_CNT + mOrderNum;
	return StringUtils::format(FILENAME_RECORD_TEXT_X, mWeekData, mOrderNum);
}


float RecordLayer::getRatioDrawImage(Size size)
{
	Size screenSize = Pos::getScreenSize();
	// 1580
	//Size size = bgCube->getContentSize();
	float x1 = screenSize.width * (size.width / SCREEN_SIZE_WIDTH);
	float x2 = screenSize.width * (RECORD_SIZE_CUBE / SCREEN_SIZE_WIDTH);

	float ratio = (1.0f / x1) * x2;
	return ratio;
}


/**
* @brief  레이아웃 초기화
* @return  void
*/
void RecordLayer::initLayout()
{
	CCLOG("WEEK [%d] OrderNumber[%d]", mWeekData, mOrderNum);

	// 나레이션 파일 명 설정
	setNarFileName();

	////////////////////////////////////////
	// 배경
	Sprite* bg = Sprite::create(FILENAME_RECORD_BG);
	bg->setPosition(Pos::getCenterPt());
	bg->setScale(BACKGROUND_SCALE);
	this->addChild(bg, 0);

	// 기존 저장된(그린) 이미지가 존재하는 경우
	if (isExistDrawFile())
	{
		// 존재
		// 배경 그림 불러오기(수정 필요)	
		Sprite* bgCube = Sprite::create(mFileName.c_str());		
		//Size screenSize = Pos::getScreenSize();
		//// 1580
		//Size size = bgCube->getContentSize();
		//float x1 = screenSize.width * (size.width / SCREEN_SIZE_WIDTH);
		//float x2 = screenSize.width * (RECORD_SIZE_CUBE / SCREEN_SIZE_WIDTH);

		//float ratio = (1.0f / x1) * x2;
		float ratio = getRatioDrawImage(bgCube->getContentSize());
		bgCube->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
		bgCube->setPosition(Pos::getRecordLayerCubeBG());
		bgCube->setScale(ratio);


		this->addChild(bgCube, CanvasLayerOrder::kDepthCanvasBG, TAG_RECORD::CUBE_BG);

		// 캔버스 이미지
		setDrawLayer();
	}	
	else
	{
		// 미존재
		// 기존의 그려진 그림 초기화		
		// 사용자 그림 설정 지우기
		Sprite* spCube = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::DRAW));
		if (spCube != nullptr)
		{
			this->removeChildByTag(TAG_RECORD::DRAW);
		}
		// 기존에 그려진 스티커 지우기
		Sprite* spCubeSticker = dynamic_cast<Sprite*> (this->getChildByTag(TAG_RECORD::STICKER));
		if (spCubeSticker != nullptr)
		{
			this->removeChildByTag(TAG_RECORD::STICKER);
		}
				
		// 배경 그림 불러오기(수정 필요)	
		Sprite* bgCube = Sprite::create(MBJson::getInstance()->getDrawColorImageFileName(mOrderNum).c_str());
		//Size screenSize = Pos::getScreenSize();
		//// 1580
		//Size size = bgCube->getContentSize();
		//float x1 = screenSize.width * (size.width / SCREEN_SIZE_WIDTH);
		//float x2 = screenSize.width * (RECORD_SIZE_CUBE / SCREEN_SIZE_WIDTH);

		//float ratio = (1.0f / x1) * x2;
		float ratio = getRatioDrawImage(bgCube->getContentSize());
		bgCube->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
		bgCube->setPosition(Pos::getRecordLayerCubeBG());
		bgCube->setScale(ratio);
		this->addChild(bgCube, CanvasLayerOrder::kDepthCanvasBG, TAG_RECORD::CUBE_BG);

		// 고정형 이미지가 아닐경우 먹선이미지 표시
		if (MBJson::getInstance()->mLockCubes[mOrderNum] == false)
		{
			// 먹선 이미지 추가
			Sprite* spLine = Sprite::create(MBJson::getInstance()->getDrawLineImageFileName(mOrderNum));
			spLine->setPosition(Pos::getRecordLayerCubeBG());
			spLine->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
			spLine->setScale(ratio);
			this->addChild(spLine, CanvasLayerOrder::kDepthCanvasLine, TAG_RECORD::LINE);
		}
	}

	// 사용자 그림 위에 덮어질 배경(그림 박스 배경)
	Sprite* bgCubeBG = Sprite::create(FILENAME_RECORD_BG_CUBE);
	bgCubeBG->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	Vec2 bgPos = Pos::getRecordLayerBG();
	Vec2 cubeBgPos = Pos::getCenterPt();
	Size cubeBgSize = bgCubeBG->getContentSize();

	cubeBgPos.x -= (cubeBgSize.width / 2);
	cubeBgPos.y = bgPos.y;

	bgCubeBG->setPosition(cubeBgPos);
	//bgCubeBG->setPosition(Pos::getRecordLayerBG());
	this->addChild(bgCubeBG, DEPTH_LAYER_RECORD_ROUND_BG);

	////////////////////////////////////////
	// 스피커 / 문장 제시영역
	// 문장
	//int oNum = (mWeekData - 2) * RECORD_TEXT_CNT + mOrderNum;
	//std::string strText = StringUtils::format(FILENAME_RECORD_TEXT_X, mWeekData, mWeekData, oNum);	
	std::string strText = getStringRecordNarrationText();
	CCLOG("TEXT : %s", strText.c_str());

	auto speakText = Sprite::create(strText.c_str());
	speakText->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	speakText->setPosition(Pos::getRecordLayerSpeakerText());
	this->addChild(speakText, DEPTH_LAYER_RECORD_CLOSE_BTN, TAG_RECORD::TEXT);

	// 스피커 
	mSpeakerItem = MenuItemImage::create(FILENAME_RECORD_BTN_SPEAKER_N, FILENAME_RECORD_BTN_SPEAKER_P, FILENAME_RECORD_BTN_SPEAKER_P,
		CC_CALLBACK_1(RecordLayer::menuSpeakerCallback, this));
	mSpeakerItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpeakerItem->setPosition(Pos::getRecordLayerSpeaker());
	// create menu, it's an autorelease object
	mCtlSpeaker = Menu::create(mSpeakerItem, NULL);
	mCtlSpeaker->setPosition(Vec2::ZERO);
	this->addChild(mCtlSpeaker, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::SPEAKER);

	////////////////////////////////////////
	// 버튼 영역
	// 그림 박스 배경
	Sprite* bgControlBG = Sprite::create(FILENAME_RECORD_BG_CONTROL_BOX);
	bgControlBG->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	bgControlBG->setPosition(Pos::getRecordLayerControlBoxBG());
	this->addChild(bgControlBG);
}


// 버튼 초기화
void RecordLayer::createUi()
{	
	CCLOG("[RECORD] CREATE UI.......");
	////////////////////////////////////////
	// 공통 UI
	// 나가기 버튼
	auto closeItem = MenuItemImage::create(
		FILENAME_BTN_CLOSE_N, FILENAME_BTN_CLOSE_P,
		CC_CALLBACK_1(RecordLayer::menuCloseCallback, this));
	closeItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	closeItem->setPosition(Pos::getBackBtnPt());

	// create menu, it's an autorelease object
	mCltBack = Menu::create(closeItem, NULL);
	mCltBack->setPosition(Vec2::ZERO);
	this->addChild(mCltBack, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::BACK);
	// 최초 터치 막기
	mCltBack->setEnabled(false);	

	// 도움말 버튼
	mHelpItem = MenuItemImage::create(
		FIlENAME_BTN_HELP_N, FIlENAME_BTN_HELP_P,
		CC_CALLBACK_1(RecordLayer::menuHelpCallback, this));
	mHelpItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mHelpItem->setPosition(Pos::getHelpBtnPt());

	// create menu, it's an autorelease object
	mCtlHelp = Menu::create(mHelpItem, NULL);
	mCtlHelp->setPosition(Vec2::ZERO);
	this->addChild(mCtlHelp, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::HELP);
	// 도움말 최초 비활성화
	setVisibleForHelpBtn(false);
	mIsVisibleHelp = false;
		
	// 넥스트 버튼
	int idx = INVALID_INDEX;
	auto json = MBJson::getInstance();
	for (int i = mOrderNum; i <= SIZE_CUBE; i++)
	{
		// next check
		// base index is zero(0, 1, 2, 3, 4, 5)
		if (json->getCubeSuccessIndex(i))
		{			
			idx = i;
			CCLOG("FOR NEXT INDEX : %d", idx);
			break;
		}		
	} // end of for

	CCLOG("NEXT INDEX : %d", idx);
	//mNextItem = nullptr;
	// next	
	mNextItem = MenuItemImage::create(
		FIlENAME_BTN_NEXT_N, FIlENAME_BTN_NEXT_P,
		CC_CALLBACK_1(RecordLayer::menuNextCallback, this));
	mNextItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mNextItem->setPosition(Pos::getRecordLayerBtnNext());
	
	// create menu, it's an autorelease object
	mCltnext = Menu::create(mNextItem, NULL);
	mCltnext->setPosition(Vec2::ZERO);
	this->addChild(mCltnext, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::NEXT);
	
	// 넥스트 버튼 효과 만들기
	createNextButtonMotion();
	if (idx != INVALID_INDEX)
	{
		setVisibleForNextBtn(true);
	}
	else
	{
		setVisibleForNextBtn(false);
	}

	// 휴지통 버튼
	mTrashItem = MenuItemImage::create(
		FILENAME_COMMON_BTN_TRASH_N, FILENAME_COMMON_BTN_TRASH_P, FILENAME_COMMON_BTN_TRASH_D,
		CC_CALLBACK_1(RecordLayer::menuTrashCallback, this));
	mTrashItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mTrashItem->setPosition(Pos::getRecordLayerControlBoxBtnTrash());
	
	// create menu, it's an autorelease object
	mCtlTrash = Menu::create(mTrashItem, NULL);
	mCtlTrash->setPosition(Vec2::ZERO);
	this->addChild(mCtlTrash, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::TRASH);
	
	////////////////////////////////////////
	// 콘트롤 박스
	// Record	
	mRecordItem = MenuItemImage::create(
		FILENAME_COMMON_BTN_RECORD_N, FILENAME_COMMON_BTN_RECORD_P, FILENAME_COMMON_BTN_RECORD_D,
		CC_CALLBACK_1(RecordLayer::menuRecordCallback, this));
	mRecordItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mRecordItem->setPosition(Pos::getRecordLayerControlBoxBtnRecord());
	// create menu, it's an autorelease object
	mCtlRecord = Menu::create(mRecordItem, NULL);
	mCtlRecord->setPosition(Vec2::ZERO);
	this->addChild(mCtlRecord, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::RECORD);

	mSpRecordStroke = Sprite::create(FILENAME_COMMON_BTN_RECORD_STROKE);
	mSpRecordStroke->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpRecordStroke->setPosition(Pos::getRecordLayerControlBoxBtnRecord());
	this->addChild(mSpRecordStroke, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::RECORD);
	// 최초 숨김
	mSpRecordStroke->setVisible(false);

	// Stop(Record) 
	// 어포던스 버튼으로 변경
	mRecordStopItem = MenuItemImage::create(
		FILENAME_COMMON_BTN_STOP_BG, FILENAME_COMMON_BTN_STOP_BG, FILENAME_COMMON_BTN_STOP_D,
		CC_CALLBACK_1(RecordLayer::menuRecordStopCallback, this));
	mRecordStopItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mRecordStopItem->setPosition(Pos::getRecordLayerControlBoxBtnRecord());
	// create menu, it's an autorelease object
	mCtlRecordStop = Menu::create(mRecordStopItem, NULL);
	mCtlRecordStop->setPosition(Vec2::ZERO);
	this->addChild(mCtlRecordStop, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::RECORD_STOP);

	// FX
	mRecordStopFx = Sprite::create(FILENAME_COMMON_BTN_STOP_FX);	
	mRecordStopFx->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mRecordStopFx->setPosition(Pos::getRecordLayerControlBoxBtnRecord());
	this->addChild(mRecordStopFx, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::RECORD_STOP);

	mRecordStopImg = Sprite::create(FILENAME_COMMON_BTN_STOP_IMG);
	mRecordStopImg->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mRecordStopImg->setPosition(Pos::getRecordLayerControlBoxBtnRecord());
	this->addChild(mRecordStopImg, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::RECORD_STOP);
	// 버튼 효과 만들기
	createStopButtonMotion();
	// 최초 숨김
	setVisibleForStopBtn(false);
		
	// Play
	mPlayItem = MenuItemImage::create(
		FILENAME_COMMON_BTN_PLAY_N, FILENAME_COMMON_BTN_PLAY_N, FILENAME_COMMON_BTN_PLAY_D,
		CC_CALLBACK_1(RecordLayer::menuPlayCallback, this));
	mPlayItem->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mPlayItem->setPosition(Pos::getRecordLayerControlBoxBtnPlay());
	// create menu, it's an autorelease object
	mCtlPlay = Menu::create(mPlayItem, NULL);
	mCtlPlay->setPosition(Vec2::ZERO);
	this->addChild(mCtlPlay, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::PLAY);
	// 초기화
	mIsTouchPlayBtn = false;

	// 플레이 스트로크
	mSpPlayStroke = Sprite::create(FILENAME_COMMON_BTN_PLAY_STROKE);
	mSpPlayStroke->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpPlayStroke->setPosition(Pos::getRecordLayerControlBoxBtnPlay());
	this->addChild(mSpPlayStroke, DEPTH_LAYER_RECORD_CLOSE_BTN, BTN_TAG_RECORD::PLAY);
	// 최초 숨김
	mSpPlayStroke->setVisible(false);

	// 프로그레스바
	// Progress Timer 표시 및 초기화
	Sprite* timeOutline = Sprite::create(FILENAME_COMMON_PROGRESS_BG);
	timeOutline->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	timeOutline->setPosition(Pos::getRecordLayerControlBoxProgress());
	timeOutline->setVisible(true);
	this->addChild(timeOutline, DEPTH_LAYER_RECORD_CLOSE_BTN);
	
	Sprite* timeBar = Sprite::create(FILENAME_COMMON_PROGRESS_BAR);	
	mProgressTimeBar = ProgressTimer::create(timeBar);
	mProgressTimeBar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mProgressTimeBar->setPosition(Pos::getRecordLayerControlBoxProgress());	
	mProgressTimeBar->setPercentage(100.0f);
	mProgressTimeBar->setMidpoint(Point(0, 0.5f));	
	mProgressTimeBar->setBarChangeRate(Point(1, 0));
	mProgressTimeBar->setType(ProgressTimerType::BAR);		
	
	this->addChild(mProgressTimeBar, DEPTH_LAYER_RECORD_CLOSE_BTN);

	// rounded bar
	/*mRoundedBar = Sprite::create("res/common/bar_round.png");
	mRoundedBar->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mRoundedBar->setPosition(Pos::getRecordLayerControlBoxProgress());
	this->addChild(mRoundedBar, DEPTH_LAYER_RECORD_CLOSE_BTN);*/
}


//-----------------------------------------------
// 버튼 리스너
//-----------------------------------------------
// 스피커 버튼
// 원어민 나레이션 재생 버튼 비활성화후 나레이션 재생후 활성화
void RecordLayer::menuSpeakerCallback(Ref* pSender)
{
	mDisabledHelpBtn = true;
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}	
	CCLOG("menuSpeakerCallback.....");	
	
	// 변경 (07.05)
	//setMode(RECORD_MODE::SPEAKER); --> speakerMode();
	speakerMode();
	
	playNarSndForSpeaker();
}


// 도움말 버튼
void RecordLayer::menuHelpCallback(Ref* pSender)
{
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}

	// 재생중인 녹음한 음원이 있는 경우 잠시 중지
	//if (mRecordSndId != INVALID_INDEX)
	//{
	//	CCLOG("RECORD ID : %d", mRecordSndId);
	//	AudioEngine::pause(mRecordSndId);
	//	// 프로그래스바 잠시 멈춤
	//}

	setEnableTouchBtn(false);

	//효과음
	auto aid = AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);
	AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishHelpClickCallback, this));
	
	PopupLayer* popup = PopupLayer::getIns(this, HELP_RECORD);
	popup->showHide(true);	
}


// 휴지통 버튼
void RecordLayer::menuTrashCallback(Ref* pSender)
{
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}
	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);	

	setEnableTouchBtn(false);
	CCLOG("menuTrashCallback.....");
	mPopup = PopupLayer::getIns(this, RECORD_DELETE);
	mPopup->showHide(true);
	
	//trashRecord();
}


// 다음 버튼 이벤트
void RecordLayer::menuNextCallback(Ref* pSender)
{
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}
	AudioEngine::stopAll();
	//효과음
	AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);
	//AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishNextClickCallback, this));	
	// 도움말 버튼 비활성화
	mHelpItem->setEnabled(false);
		
	// 다음 컨텐츠 로드
	// 그림, 버튼 초기화
	int idx   = INVALID_INDEX;
	auto json = MBJson::getInstance();	
	for (int i = mOrderNum; i <= SIZE_CUBE; i++)
	{
		// next check
		// base index is zero(0, 1, 2, 3, 4, 5)
		if (json->getCubeSuccessIndex(i))
		{
			idx = i;
			break;
		}		
	} // end of for
	
	CCLOG("menuNextCallback.....mOrdNum: %d, idx : %d", mOrderNum, idx);
	if (idx != INVALID_INDEX)
	{
		// next
		idx++;
		nextRecordLayer(idx, MBJson::getInstance()->mDrawFileNames[idx]);
	}	
}


// 녹음하기 버튼
void RecordLayer::menuRecordCallback(Ref* pSender)
{
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}

	AudioEngine::stopAll();

	if (mIsClickRecording)
	{
		CCLOG("menuRecordCallback : %d", mIsClickRecording);
		return;
	}

	mIsClickRecording = true;

	CCLOG("menuRecordCallback.....MODE: %d", mMode);
	setMode(RECORD_MODE::NARATION);
	// 원어민 나레이션 -> 녹음 시작		
	playNarSnd();
}


// 재생하기 버튼
void RecordLayer::menuPlayCallback(Ref* pSender)
{
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}

	if (mRecordSndId != INVALID_INDEX)
	{
		AudioEngine::stop(mRecordSndId);
		mRecordSndId = INVALID_INDEX;
	}

	if (mIsTouchPlayBtn)
	{
		return;
	}
	mIsTouchPlayBtn = true;

	CCLOG("menuPlayCallback.....");
	initProgressBar();
	// 녹음한 음원 play		
	setMode(RECORD_MODE::PRE_PLAY);	
	playBeforePlayRecordSnd();
}


// 멈추기 버튼(녹음하기)
void RecordLayer::menuRecordStopCallback(Ref* pSender)
{
	CCLOG("menuStopCallback...mIsClickRecording: %d", mIsClickRecordingStop);
	if (mIsClickRecordingStop)
	{
		return;
	}
	mIsClickRecordingStop = true;

	// 녹음 중지
	stopRecord();
}


// 멈추기 버튼(재생하기)
void RecordLayer::menuPlayStopCallback(Ref* pSender)
{
	CCLOG("menuPlayStopCallback.....");
	// 중지버튼 비활성화
	setMode(RECORD_MODE::INIT);
	// 프로그래스 바 초기화
	// 녹음 초기화
	if (mProgressTimeBarSeq)
	{
		mProgressTimeBar->stopAction(mProgressTimeBarSeq);
		mProgressTimeBarSeq->release();
		mProgressTimeBarSeq = nullptr;
	}
	
	mProgressTimeBar->setPercentage(0.0f);
}


// 닫기
void RecordLayer::menuCloseCallback(Ref* pSender)
{
	CCLOG("menuCloseCallback : %d", mIsClose);
	if (mIsClose)
	{		
		return;
	}
	mIsClose = true;
	
	setEnableTouchBtn(false);

	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}

	//효과음	
	auto aid = AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);	
	AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishCloseBtnCallback, this));
	CCLOG("menuCloseCallback aid : %d", aid);
	
	// 레이어 닫기 팝업(삭제)
	/*mPopup = PopupLayer::getIns(this, RECORD_CLOSE);
	((PopupLayer*)mPopup)->showHide(true, false);*/
	
	// 팝업창 없이 바로 종료
	/*menuPopupClose();
	closeRecord();*/
}


/**
* @brief   콜백 함수(데이터 삭제를 위한 딜레이함수)
* @return  void
*/
void RecordLayer::delayCloseCallbackDone(Node* pSender)
{
	CCLOG("delayCloseCallbackDone..... mIsClickRecording: %d, mIsClose: %d", mIsClickRecording, mIsClose);
	if (mIsClickRecording)
	{
		// 데이터 삭제
		trashRecord();
		mIsClickRecording = false;
	}	

	if (mIsClose)
	{
		menuPopupClose();
		closeRecord();
	}	
}

/**
* @brief  도움말창 콜백 함수(데이터 삭제를 위한 딜레이함수)
* @return  void
*/
void RecordLayer::delayHelpCloseCallbackDone(Node* pSender)
{
	CCLOG("delayHelpCloseCallbackDone..... mIsClickRecording: %d", mIsClickRecording);
	if (mIsClickRecording)
	{
		// 데이터 삭제
		trashRecord();
		mIsClickRecording = false;	
	}
}


/**
* @brief 레이어 닫기
* @return void
*/
void RecordLayer::closeRecord()
{
	CCLOG("closeRecord.......");
	//mPopup->showHide(false, false);
	// 노티 보내기 -> 메인씬에서 레이어 닫기
	String* pNotiParam = String::create(StringUtils::format("%d", NOTI_CODE_VOICE_RECORD_CLOSE));
	NotificationCenter::getInstance()->postNotification(NOTI_STR_VOICE_RECORD, pNotiParam);
}


/**
* @brief 레이어 닫기
* @return void
*/
void RecordLayer::deleteRecord()
{
	mPopup->showHide(false, false);
	// 데이터 삭제
	trashRecord();
}


/**
* @brief 팝업 닫기
* @return void
*/
void RecordLayer::menuPopupClose()
{
	CCLOG("menuPopupClose.........");
	setEnableTouchBtn(true);	

	// 재생중인 음원 있는 경우 재생
	/*if (mRecordSndId != INVALID_INDEX)
	{
		AudioEngine::resume(mRecordSndId);
	}*/
}


//-----------------------------------------------
// 모드별 컨트롤 처리
//-----------------------------------------------

void RecordLayer::setEnableTouchBtn(bool b)
{	
	CCLOG("setEnableTouchBtn");
	
	auto dispatcher = Director::getInstance()->getEventDispatcher();
	for (int i = BTN_TAG_RECORD::BACK; i <= BTN_TAG_RECORD::TRASH; i++)
	{
		auto btn = this->getChildByTag(i);
		if (btn == nullptr)
		{
			CCLOG("COUNTIUE......TAG : %d", i);
			continue;
		}

		if (b)
		{
			dispatcher->resumeEventListenersForTarget(btn);
		}
		else
		{
			dispatcher->pauseEventListenersForTarget(btn);
		}
	} // end of for
}


/**
* @brief 녹음한 이력이 있는 경우 버튼 활성화, 없는 경우 비활성화
* @return void
*/
void RecordLayer::setCtlBtnForExistRecordFile()
{
	//mPlayItem->setVisible(true);
	mSpPlayStroke->setVisible(false);
	if (isExistRecordFile())
	{
		CCLOG("EXIST... RECORD FILE : %s", getRecordFileName().c_str());
		// 이력이 있는 경우
		mPlayItem->setEnabled(true);		
		mTrashItem->setEnabled(true);
		
	}
	else
	{
		CCLOG("NO... RECORD FILE : %s", getRecordFileName().c_str());
		mPlayItem->setEnabled(false);
		mTrashItem->setEnabled(false);
		
	}
}


/**
* @brief  전처리 모드
*/
void RecordLayer::preMode()
{
	// 최초 진입시 나레이션 재생중 모드
	// 모든 버튼 비활성화
	mRecordItem->setEnabled(false);	
	mSpeakerItem->setEnabled(false);
	// 플레이 비활성화
	mPlayItem->setEnabled(false);
	mSpPlayStroke->setVisible(false);

	// 휴지통 비활성화
	mTrashItem->setEnabled(false);
	// 도움말 버튼 비활성화
	setVisibleForHelpBtn(false);

	// 넥스트 버튼 비활성화
	if (mOrderNum < SIZE_CUBE)
	{
		if (mNextItem != nullptr)
		{			
			setVisibleForNextBtn(false);
		}		
	}
}


/**
* @brief 초기화 모드
* @return void
*/
void RecordLayer::initMode()
{
	CCLOG("initMode...");

	// 스피커 버튼 활성화
	//mSpeakerItem->setEnabled(true);
	//mSpeakerItem->setVisible(true);

	// 녹음 버튼 활성화		
	mRecordItem->setEnabled(true);
	mSpRecordStroke->setVisible(false);

	setVisibleForStopBtn(false);

	// 도움말 버튼 활성화 -> 비활성화
	// 오토 모드일경우 비활성화	
	//setVisibleForHelpBtn(true);
	if (mIsVisibleHelp)
	{
		setVisibleForHelpBtn(true);
	}
	// 녹음 이력에 따라 활성화
	setCtlBtnForExistRecordFile();

	// 넥스트 버튼 활성화
	if (mOrderNum < SIZE_CUBE)
	{
		setNextBtn();		
	}
}


/**
* @brief 원어민 나레이션
* @return void
*/
void RecordLayer::narMode()
{
	CCLOG("narMode");
	// 녹음 버튼 비활성화 -> 스트로크 버튼으로 변경
	mRecordItem->setEnabled(false);	
	mSpRecordStroke->setVisible(true);	

	mPlayItem->setEnabled(false);
	mSpPlayStroke->setVisible(false);
	mTrashItem->setEnabled(false);
	// 스피커 버튼 비활성화	
	mSpeakerItem->setEnabled(false);
	//mSpeakerItem->setVisible(false);
	
	// 도움말 버튼 활성화 처리 (나레이션 중에도 도움말 버튼 클릭 유지) 
	// 6.10자 이슈로 삭제
	//mHelpItem->setEnabled(false);
	// 도움말 버튼 비활성화 
	setVisibleForHelpBtn(false);

	// next button 비활성화 처리
	setVisibleForNextBtn(false);
}


/**
* @brief 녹음중...
* @return void
*/
void RecordLayer::recordingMode()
{
	CCLOG("recordingMode......");
	// 녹음 버튼 비활성화
	mRecordItem->setEnabled(false);
	mSpRecordStroke->setVisible(false);
	//mRecordItem->setVisible(false);
	// 멈춤 버튼 보이기
	setVisibleForStopBtn(true);

	// 스피커 버튼 비활성화	-> 활성화로 변경
	//mSpeakerItem->setEnabled(false);
	mSpeakerItem->setEnabled(true);
	
	mPlayItem->setEnabled(false);
	mSpPlayStroke->setVisible(false);
	mTrashItem->setEnabled(false);
		
	// 도움말 버튼 비활성화 
	setVisibleForHelpBtn(false);
}


// 완료후 자동 재생 상태
//void RecordLayer::autoPlayMode()
//{
//	mMode = RECORD_MODE::AUTO_PLAY;
//	// 원어민 과 아이목소리 자동 재생
//	// 1. 원어민 재생
//	playNarSnd();
//	// 2. 녹음한 음성 재생
//	//playRecordSnd();
//	playBeforePlayRecordSnd();
//}


// 녹음 멈춤 상태
void RecordLayer::stopMode()
{
	CCLOG("stopMode..");
	// 녹음 버튼 활성화
	mRecordItem->setEnabled(true);	
	mSpRecordStroke->setVisible(false);

	// 멈춤 버튼 숨기기		
	setVisibleForStopBtn(false);
	
	// 스피커 버튼 활성화
	mSpeakerItem->setEnabled(true);	
	
	// 파일이 존재할 경우 활성화
	// 파일이 없는 경우 비활성화
	setCtlBtnForExistRecordFile();

	// 다음이 존재하는 경우 활성화처리 해야됨..	
	//setNextBtn();
	
	// 도움말 버튼 활성화 처리
	//mHelpItem->setEnabled(true);	
}


// 녹음 완료 상태
void RecordLayer::doneMode()
{
	CCLOG("doneMode.....: %d", mIsVisibleHelp);
	// 스피커버튼 활성화, 정지버튼->녹음버튼으로변경 활성화,
	// 플레이버튼 활성화, 휴지통 버튼 활성화

	// 스피커 버튼 활성화
	//mSpeakerItem->setEnabled(true);	

	// 녹음 버튼 활성화
	mRecordItem->setEnabled(true);		
	mSpRecordStroke->setVisible(false);

	// 멈춤 버튼 숨기기
	setVisibleForStopBtn(false);

	// 플레이 버튼 활성화	
	mPlayItem->setEnabled(true);
	mSpPlayStroke->setVisible(false);	

	// 휴지통 활성화
	mTrashItem->setEnabled(true);

	// 도움말 버튼 활성화 처리	
	if (mIsVisibleHelp)
	{
		setVisibleForHelpBtn(true);
	}	

	// 다음이 존재하는 경우 활성화처리 해야됨..
	setNextBtn();

	// 초기화
	mIsClickRecording = false;
	mIsClickRecordingStop = false;
	// play 버튼 중복 방지
	mIsTouchPlayBtn = false;
}


/**
* @brief  스피커 모드(나레이션 재생)
* @return void
*/
void RecordLayer::speakerMode()
{
	CCLOG("speakerMode...........");
	// 스피커 버튼 비활성화
	mSpeakerItem->setEnabled(false);	
	// 도움말 버튼 비활성화 
	setVisibleForHelpBtn(false);

	/*
	// 녹음 버튼 활성화	(비활성화, 스크로크 이미지)
	mRecordItem->setEnabled(false);	
	mSpRecordStroke->setVisible(true);
	
	// 플레이 버튼 비활성화
	mPlayItem->setEnabled(false);
	mSpPlayStroke->setVisible(false);
	// 휴지통 활성화
	mTrashItem->setEnabled(false);

	// 도움말 버튼 활성화 처리
	mHelpItem->setEnabled(false);
	*/
}


/**
* @brief 프리 플레이 모드 (녹음 음원 재생전 모드)
* @return void
*/
void RecordLayer::prePlayMode()
{
	CCLOG("prePlayMode...........");
	// 스피커 버튼 비활성화
	//mSpeakerItem->setEnabled(false);	
	
	// 녹음 버튼 비 활성화	
	mRecordItem->setEnabled(false);
	mSpRecordStroke->setVisible(false);

	// 플레이 버튼 비 활성화 
	//mPlayItem->setEnabled(false);		
	//mSpPlayStroke->setVisible(false);
	
	// 휴지통 비 활성화
	mTrashItem->setEnabled(false);

	// 도움말 버튼 비활성화 
	setVisibleForHelpBtn(false);
}


/**
* @brief  플레이 모드(녹음 음원 재생)
* @return void
*/
void RecordLayer::playMode()
{
	CCLOG("playMode...........");
	// 스피커 버튼 비활성화 -> 활성화
	mSpeakerItem->setEnabled(true);	
	
	// 녹음 버튼 비 활성화	
	mRecordItem->setEnabled(false);	
	mSpRecordStroke->setVisible(false);
	// 녹음 정지 비 활성화
	setVisibleForStopBtn(false);

	// 플레이 버튼 활성화 -> 스트로크 이미지
	mPlayItem->setEnabled(false);	
	mSpPlayStroke->setVisible(true);

	// 휴지통 비 활성화
	mTrashItem->setEnabled(false);

	// 도움말 버튼 비활성화 
	setVisibleForHelpBtn(false);
}


/**
* @brief  모드별 컨트롤 버튼 및 상황 처리
*         녹음중일 경우 도움말 비활성화 처리
*/
void RecordLayer::processCtlBtn()
{
	CCLOG("processCtlBtn mode: %d", mMode);
	switch (mMode)
	{
	case RECORD_MODE::PRE:
		preMode();
		break;

	case RECORD_MODE::INIT:
		initMode();
		break;

	case RECORD_MODE::NARATION:
		narMode();
		break;

	case RECORD_MODE::RECORDING:
		recordingMode();
		break;

	/*case RECORD_MODE::AUTO_PLAY:
		autoPlayMode();
		break;*/

	case RECORD_MODE::DONE:
		doneMode();
		break;

	case RECORD_MODE::STOP:
		stopMode();
		break;

	case RECORD_MODE::SPEAKER:
		speakerMode();
		break;

	case RECORD_MODE::PRE_PLAY:
		prePlayMode();
		break;

	case RECORD_MODE::PLAY:
		playMode();
		break;

	// 미사용
	case RECORD_MODE::NOT_USED:
		CCLOG("ASSERT processCtlBtn mode: %d", mMode);
		break;

	default:
		CCLOG("ASSERT processCtlBtn mode: %d", mMode);
		//CC_ASSERT(false);
		break;
	}
}


// 프로그래스바 초기화
void RecordLayer::initProgressBar()
{
	CCLOG("initProgressBar........");
	if (mProgressTimeBarSeq)
	{
		CCLOG("initProgressBar........mProgressTimeBarSeq is not null...");
		mProgressTimeBar->stopAction(mProgressTimeBarSeq);		
		mProgressTimeBarSeq = nullptr;
	}
	CCLOG("initProgressBar........setPercentage");
	mProgressTimeBar->setPercentage(0.0f);
}


/**
* @brief  녹음 상태 설정
* @return void
*/
void RecordLayer::setMode(RECORD_MODE::MODE m)
{
	mMode = m;
	processCtlBtn();
}


/**
* @brief  녹음 전
*         - 녹음버튼 활성화, 재생버튼과 저장버튼은 비활성화
* @return void
*/
//void RecordLayer::preprocessRecord()
//{
//	setMode(RECORD_MODE::PRE);
//	// 사운드 처리(녹음하기 집입시)
//	//AudioEngine::play2d(FILENAME_SND_RECORD_OPENNING);
//	auto aid = AudioEngine::play2d(FILENAME_SND_RECORD_OPENNING);
//	AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishPreprocessRecordCallback, this));
//}

/**
* @brief 콜백 함수
*/
//void RecordLayer::finishPreprocessRecordCallback()
//{
//	CCLOG("finishPreprocessRecordCallback........");
//	mCltBack->setEnabled(true);
//
//	// 도움말 버튼 보이기
//	setMode(RECORD_MODE::INIT);
//
//	// 진입 나레이션
//	mSecondNarId = AudioEngine::play2d(FILENAME_SND_RECORD_PLAY_ENABLE);
//	AudioEngine::setFinishCallback(mSecondNarId, CC_CALLBACK_0(RecordLayer::finishPreprocessRecordSecondCallback, this));
//}


/**
* @brief  활동 이력 저장
* @return void
*/
void RecordLayer::saveRecordHistory()
{	
	UserDefault::getInstance()->setBoolForKey(
		StringUtils::format(USERDEFAULT_KEY_RECORD_HISTORY_X, mWeekData).c_str(), true);
}


/**
* @brief  자동 모드 : 녹음 시작전 처리(나레이션, 효과음 재생후 녹음) -> startRecord 호출
* @return void
*/
void RecordLayer::processAutoMode()
{
	/*
	1. 진입 나레이션 재생
	2. 권별 나레이션 재생
	3. 녹음 효과음 재생
	4. 녹음 진행
	5. 저장 후 자동 재생
	*/
	/*
	// 파일이 존재하면 기존
	// 파일이 존재하지 않으면 오토모드 --> 변경, 최초 진입시만 나레이션 재생
	*/	
	//if (isExistRecordFile())
	if (isExistRecordHistory())
	{
		CCLOG("EXIST...... NOT... AUTO.....");
		mIsAutoMode = false;
	}
	else
	{
		CCLOG("NOT...EXIST...... AUTO.....");
		mIsAutoMode = true;
	}	

	// 이력 저장
	saveRecordHistory();

	preprocessRecord();
}


/**
* @brief  녹음 전
*         - 녹음버튼 활성화, 재생버튼과 저장버튼은 비활성화
* @return void
*/
void RecordLayer::preprocessRecord()
{
	//setMode(RECORD_MODE::PRE);
	//// 사운드 처리(녹음하기 집입시)
	//auto aid = AudioEngine::play2d(FILENAME_SND_RECORD_OPENNING);
	//AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishPreprocessRecordCallback, this));
	mCltBack->setEnabled(true);

	// 도움말 버튼 보이기(최초 비활성화로 변경)
	setMode(RECORD_MODE::INIT);

	// 진입 나레이션
	if (mIsAutoMode)
	{
		mSecondNarId = AudioEngine::play2d(FILENAME_SND_RECORD_PLAY_ENABLE);
		AudioEngine::setFinishCallback(mSecondNarId, CC_CALLBACK_0(RecordLayer::finishPreprocessRecordSecondCallback, this));
	}	
	else
	{
		finishPreprocessRecordSecondCallback();			
	}
}


/**
* @brief 콜백 함수(닫기버튼)
*/
void RecordLayer::finishCloseBtnCallback()
{
	CCLOG("finishCloseBtnCallback........");
	stopRecord();
}


/**
* @brief 콜백 함수
*/
void RecordLayer::finishPreprocessRecordCallback()
{
	CCLOG("finishPreprocessRecordCallback........");
	mCltBack->setEnabled(true);

	// 도움말 버튼 보이기
	setMode(RECORD_MODE::INIT);

	// 진입 나레이션
	mSecondNarId = AudioEngine::play2d(FILENAME_SND_RECORD_PLAY_ENABLE);
	AudioEngine::setFinishCallback(mSecondNarId, CC_CALLBACK_0(RecordLayer::finishPreprocessRecordSecondCallback, this));
}


/**
* @brief 콜백 함수
*/
void RecordLayer::finishPreprocessRecordSecondCallback()
{
	CCLOG("finishPreprocessRecordSecondCallback........");
	mSecondNarId = INVALID_INDEX;
	if ((mIsAutoMode == true) || (isExistRecordFile() == false))
	{
		// 자동 녹음 모드
		// 권별 나레이션
		preprocessStartRecord();
	}
}


/**
* @brief  녹음 시작전 처리(나레이션, 효과음 재생후 녹음) -> startRecord 호출
* @return void
*/
void RecordLayer::preprocessStartRecord()
{
	CCLOG("preprocessStartRecord....");
	if (mSecondNarId != INVALID_INDEX)
	{
		// stop narration
		AudioEngine::stop(mSecondNarId);
	}

	if (mIsClickRecording)
	{
		CCLOG("preprocessStartRecord : %d", mIsClickRecording);
		return;
	}
	mIsClickRecording = true;

	CCLOG("preprocessStartRecord.....MODE: %d", mMode);
	setMode(RECORD_MODE::NARATION);
	// 원어민 나레이션 -> 녹음 시작		
	playNarSnd();
}


/**
* @brief  녹음 시작전 처리(효과음 재생후 녹음) -> startRecord 호출
* @return void
*/
void RecordLayer::processStartRecord()
{
	CCLOG("preprocessStartRecord....");

	// 비프음후 사운드 녹음
	playEffectRecordSnd();	
}


// 녹음시작 버튼 클릭후 JNI 호출후 성공
void RecordLayer::processJniStartSuccess()
{
	CCLOG("JNI processJniStartSuccess......");
	// 녹음시작 설정
	mPlayStartTime = getCurrentRecordTime();
	CCLOG("mPlayStartTime : %ul", mPlayStartTime);

	// 프로그레스바 설정	
	setProgressBar(TIME_VOICE_RECORD);
	// 버튼 설정(중지버튼 설정)
	setMode(RECORD_MODE::RECORDING);
	// 녹음중인지 설정
	mRecordManager->mIsRecording = true;
}


// 녹음중지 후 JNI 호출후 성공
// 음원 녹음중에 중지 버튼을 클릭한 경우 성공 처리 루틴
void RecordLayer::processJniStopSuccess()
{	
	if (mIsClose)
	{
		CCLOG("JNI processJniStopSuccess......isClose");
		// 나레이션 재생중 인지 확인		
		AudioEngine::stopAll();
		delayCloseCallbackDone(this);
		return;
	}

	if (mRecordManager->mIsRecording)
	{
		CCLOG("JNI processJniStopSuccess......1: %d", mIsVisibleHelp);
		mRecordManager->mIsRecording = false;
		// 도움말 버튼 보이기
		mIsVisibleHelp = true;
		// 녹음 시간 저장
		setPlayTime();

		// 모드 변경
		setMode(RECORD_MODE::STOP);
		
		// 녹음 초기화	
		initProgressBar();				
		//CCLOG("JNI processJniStopSuccess INIT Progress......");
		// 자동 재생
		setMode(RECORD_MODE::PRE_PLAY);
		playBeforePlayRecordSnd();
	}
	else
	{
		CCLOG("JNI processJniStopSuccess......2 : %d", mIsVisibleHelp);
		// 나레이션 재생중 인지 확인		
		//AudioEngine::stopAll();
		// 녹음 초기화	
		initProgressBar();	
		if (mIsVisibleHelp)
		{
			setVisibleForHelpBtn(true);
		}
	}	
}


// 녹음중지 후 JNI 호출후 성공
// 음원 녹음 완료후에 성공 호출
void RecordLayer::processJniStopSuccessForDone()
{
	if (mIsClose)
	{
		CCLOG("JNI processJniStopSuccessForDone......isClose");
		// 나레이션 재생중 인지 확인		
		AudioEngine::stopAll();
		delayCloseCallbackDone(this);
		return;
	}

	if (mRecordManager->mIsRecording)
	{
		CCLOG("JNI processJniStopSuccessForDone......1");
		mRecordManager->mIsRecording = false;
		// 도움말 버튼 보이기
		mIsVisibleHelp = true;

		// 녹음 완료음 재생
		playRecordDoneSnd();

		// 녹음 시간 저장
		setPlayTime();

		// 녹음 초기화
		initProgressBar();
		//setMode(RECORD_MODE::INIT);		

		// 모드 변경
		//setMode(RECORD_MODE::STOP);
				
		// 자동 재생
		setMode(RECORD_MODE::PRE_PLAY);
		playBeforePlayRecordSnd();
	}
	else
	{
		CCLOG("JNI processJniStopSuccessForDone......2 : %d, mIsVisibleHelp : %d", mIsClose, mIsVisibleHelp);
		// 나레이션 재생중 인지 확인		
		//AudioEngine::stopAll();
		// 녹음 초기화
		initProgressBar();
		setMode(RECORD_MODE::INIT);
		if (mIsVisibleHelp)
		{
			setVisibleForHelpBtn(true);
		}
		
	}
}


/**
* @brief  녹음 시작
* @return void
*/
void RecordLayer::startRecord()
{
	CCLOG("START........RECORD.......");
	// 1. 스케줄러
	// 2. 프로그레스바 설정
	// JNI CALL
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)	
	VoiceRecordManager::start(this, mWeekData, mOrderNum);
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	processJniStartSuccess();
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
}


/**
* @brief  녹음전 비프음
* @return void
*/
void RecordLayer::playEffectRecordSnd()//(Node* pSender)
{
	CCLOG("playEffectRecordSnd.....");
	auto aid = AudioEngine::play2d(FILENAME_SND_EFFECT_RECORD_START_ON);
	AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishEffectRecordCallback, this));	
}


/**
* @brief 콜백 함수
* @return void
*/
void RecordLayer::finishEffectRecordCallback()
{
	CCLOG("finishEffectRecordCallback........");
	//auto id = AudioEngine::play2d(mNarFileName.c_str());
	startRecord();

}


/**
* @brief  녹음완료 효과음
* @return void
*/
void RecordLayer::playRecordDoneSnd()//(Node* pSender)
{
	auto aid = AudioEngine::play2d(FILENAME_SND_EFFECT_RECORD_DONE);
	//AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishEffectRecordCallback, this));
}


/**
* @brief  녹음 한 음성 재생하기전 효과음 
* @return void
*/
void RecordLayer::playBeforePlayRecordSnd()//(Node* pSender)
{
	CCLOG("playBeforePlayRecordSnd.....");
	auto aid = AudioEngine::play2d(FILENAME_SND_EFFECT_RECORD_PLAY_ON);
	AudioEngine::setFinishCallback(aid, CC_CALLBACK_0(RecordLayer::finishEffectPlayRecordSndCallback, this));
}


/**
* @brief  녹음한 음성 재생하기전 효과음 출력후 녹음한 음성 재생
* @return void
*/
void RecordLayer::finishEffectPlayRecordSndCallback()
{
	CCLOG("finishEffectPlayRecordSndCallback......");
	setMode(RECORD_MODE::PLAY);
	// 녹음한 음성 재생
	playRecordSnd();
}

/**
* @brief  나레이션 플레이
* @return void
*/
void RecordLayer::playNarSndForSpeaker()//(Node* pSender)
{
	auto audio = SimpleAudioEngine::getInstance();
	if (audio->isBackgroundMusicPlaying())
	{
		// stop bgm
		audio->stopBackgroundMusic();
	}
	// 나레이션
	mNarId = AudioEngine::play2d(mNarFileName.c_str());
	AudioEngine::setFinishCallback(mNarId, CC_CALLBACK_0(RecordLayer::finishNarTextForSpeakerCallback, this));
}


/**
* @brief  나레이션 플레이
* @return void
*/
void RecordLayer::playNarSnd()//(Node* pSender)
{
	auto audio = SimpleAudioEngine::getInstance();
	if (audio->isBackgroundMusicPlaying())
	{
		// stop bgm
		audio->stopBackgroundMusic();		
	}
	// 나레이션
	mNarId = AudioEngine::play2d(mNarFileName.c_str());
	AudioEngine::setFinishCallback(mNarId, CC_CALLBACK_0(RecordLayer::finishNarTextCallback, this));
}


/**
* @brief 콜백 함수
* @return  void
*/
void RecordLayer::finishHelpClickCallback()
{
	// 녹음중인 것을 멈추고 데이터 삭제
	CCLOG("menuCloseCallback..... mode : %d, mIsClickRecording: %d", mMode, mIsClickRecording);
	mIsHelp = true;

	// 녹음중일때 버튼 클릭시 현재 녹음중이던 데이터는 삭제됨
	// 변경 (07.05)
	/*stopRecord();

	this->runAction(Sequence::create(
		DelayTime::create(0.05f),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::delayHelpCloseCallbackDone)),
		nullptr));*/
}


/**
* @brief 콜백 함수
* @return  void
*/
void RecordLayer::finishNextClickCallback()
{
}


/**
* @brief 콜백 함수(닫기버튼)
* @return  void
*/
void RecordLayer::finishCloseClickCallback()
{
	CCLOG("menuCloseCallback..... mode : %d, mIsClickRecording: %d", mMode, mIsClickRecording);
	
	// 녹음중일때 나가기 버튼 클릭시 현재 녹음중이던 데이터는 삭제됨
	stopRecord();
	// 데이터 삭제를 위한 딜레이함수
	/*this->runAction(Sequence::create(
		DelayTime::create(0.05f),
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::delayCloseCallbackDone)),
		nullptr));*/

}


/**
* @brief 콜백 함수
* @return  void
*/
void RecordLayer::finishNarTextForSpeakerCallback()//int id, const std::string &file)
{
	//mPlayNarRecordSnd = -1;
	CCLOG("finishNarTextForSpeakerCallback........");
	finishSpeakerSnd();
}


/**
* @brief 콜백 함수
* @return  void
*/
void RecordLayer::finishNarTextCallback()//int id, const std::string &file)
{
	//mPlayNarRecordSnd = -1;
	CCLOG("finishNarTextCallback........");
	//if (mMode == RECORD_MODE::SPEAKER)
	//{
	//	finishSpeakerSnd();
	//}
	//else // RECORDING
	//{
	//	// 녹음 모드(전처리 과정: 효과음재생)		
	//	processStartRecord();
	//}
	processStartRecord();
}


/**
* @brief  스피커 버튼 클릭후 나레이션 완료후 호출
* @return  void
*/
void RecordLayer::finishSpeakerSnd()
{
	CCLOG("finishSpeakerSnd..... : %d", mMode);
	// 스피커 버튼 활성화
	//setMode(RECORD_MODE::INIT);
	mNarId = INVALID_INDEX;
	// 변경
	mSpeakerItem->setEnabled(true);
	if (mMode == RECORD_MODE::INIT || mMode == RECORD_MODE::DONE || mMode == RECORD_MODE::SPEAKER)
	{
		// 도움말 버튼 보이기
		mDisabledHelpBtn = false;
		if (mIsVisibleHelp)
		{
			setVisibleForHelpBtn(true);
		}		
	}	
}


/**
* @brief 녹음한 음성 재생
         1. playBeforePlayRecordSnd
		 2. playRecordSnd
* @return  void
*/
void RecordLayer::playRecordSnd()
{
	std::string strFullPathName = StringUtils::format(FILENAME_SND_RECORD_VOICE_BASE_X) + 
		StringUtils::format(FILENAME_SND_RECORD_VOICE_X, mWeekData, mOrderNum);

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	strFullPathName = StringUtils::format("%s", FILENAME_SND_RECORD_OPENNING);
#endif
	CCLOG("playRecordSnd.... RECORD SOUND......... FILENAME: %s", strFullPathName.c_str());	

	mRecordSndId = AudioEngine::play2d(strFullPathName.c_str());
	AudioEngine::setFinishCallback(mRecordSndId, CC_CALLBACK_0(RecordLayer::finishPlayRecordCallback, this));
	CCLOG("playRecordSnd RECORD ID : %d", mRecordSndId);

	// 플레이 타임으로 프로그레스 바 설정
	//float duration = AudioEngine::getDuration(id);
	CCLOG("PLAY RECORD SND : %f,", getUserDataPlayTime());
	//setProgressBar(((float)getUserDataPlayTime()));

	this->runAction(Sequence::create(		
		DelayTime::create(0.025f), // 0.025f
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::delayMotionDone)),			
		nullptr));	
}


/**
* @brief   콜백 함수(mp3 플레이타임을 구하기 위한 딜레이)
* @return  void
*/
void RecordLayer::delayMotionDone(Node* pSender)
{
	float duration = AudioEngine::getDuration(mRecordSndId);
	CCLOG("delayMotionDone......PLAY DURATION : %f", duration - 0.025f);
	/*duration += SND_CORRECTION_VALUE;
	if (duration > TIME_VOICE_RECORD)
	{
		duration = TIME_VOICE_RECORD;
	}*/
	setProgressBar(duration);
}


/**
* @brief 콜백 함수(녹음한 음성 재생 완료후)
* @return void
*/
void RecordLayer::finishPlayRecordCallback()//int id, const std::string &file)
{
	CCLOG("finishPlayRecordCallback........");	
	setMode(RECORD_MODE::DONE);
	// 프로그래스 바 초기화
	initProgressBar();
	// 녹음한 원음 식별자 초기화
	mRecordSndId = INVALID_INDEX;
}


/**
* @brief  녹음 중지
* @return void
*/
void RecordLayer::stopRecord()
{
	CCLOG("stopRecord.. mIsClickRecording : %d", mIsClickRecording);
			
	// JNI CALL
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	VoiceRecordManager::stop(this);
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	processJniStopSuccess();
	CCLOG("processJniStopSuccess.... ENDl....");
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	
}


/**
* @brief  녹음 완료 처리(10초 완료시)
* @return void
*/
void RecordLayer::doneRecord()
{
	CCLOG("DONE........RECORD.......mIsClickRecording : %d", mIsClickRecording);
		
	if (mIsClickRecording)
	{
		setMode(RECORD_MODE::DONE);
		// JNI CALL
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		VoiceRecordManager::doneStop(this);
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	}
	else
	{
		CCLOG("DONE........RECORD.......initProgressBar : %d", mIsClickRecording);
		// 녹음 초기화
		initProgressBar();
		setMode(RECORD_MODE::INIT);
	}
			
	//// 녹음 완료음 재생
	//playRecordDoneSnd();

	//// 녹음 초기화
	//initProgressBar();
	//setMode(RECORD_MODE::INIT);

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	processJniStopSuccessForDone();
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
}


/**
* @brief  프로그래스바 설정
* @param  duration
* @return void
*/
void RecordLayer::setProgressBar(float duration)
{
	mProgressBarDurationTime = duration;
	CCLOG("SET PROGRESS BAR : %f", duration);
	/*this->runAction(Spawn::create(CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::setProgressBarAction)), 
					CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::setProgressBarRoundedAction)),
					nullptr));*/
	// 프로그레스바 설정	
	ProgressFromTo* progressToTime = ProgressFromTo::create(duration, 0, 100);
	mProgressTimeBarSeq = Sequence::create(progressToTime,
										   CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::motionProgressBarDone)), 
										   nullptr);
	mProgressTimeBar->runAction(mProgressTimeBarSeq);	

	/*mRoundedBar->runAction(Sequence::create(progressToTime,
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::motionProgressBarDone)),
		nullptr));*/
}

void RecordLayer::setProgressBarAction(Node* pSender)
{
	// 프로그레스바 설정	
	ProgressFromTo* progressToTime = ProgressFromTo::create(mProgressBarDurationTime, 0, 100);
	
	mProgressTimeBarSeq = Sequence::create(progressToTime,
		CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::motionProgressBarDone)),
		nullptr);
	mProgressTimeBar->runAction(mProgressTimeBarSeq);
}

void RecordLayer::setProgressBarRoundedAction(Node* pSender)
{
	//float duration = 0;
	//Vec2 pos = Vec2::ZERO;	
	//
	//Size size = mProgressTimeBar->getContentSize();
	//pos.x += size.width / 2;

	//// 시간 = 거리 / 속도	
	//duration = mProgressBarDurationTime;	

	//// 프로그레스바 설정	
	//CCLOG("duration : %f, Pos: %f, %f... ", duration, pos.x, pos.y);
	//mRoundedBar->runAction(Sequence::create(MoveBy::create(duration, pos),
	//	CallFuncN::create(this, CC_CALLFUNCN_SELECTOR(RecordLayer::motionProgressBarDone)),
	//	nullptr));
}


/**
* @brief   프로그레스바가 종료될때 호출되는 콜백함수
* @return  void
*/
void RecordLayer::motionProgressBarDone(Node* pSender)
{
	CCLOG("motionProgressBarDone..... RECORD COMPLETE.........");	

	doneRecord();

}


/**
* @brief   삭제처리
* @return  void
*/
void RecordLayer::trashRecord()
{
	// 파일 삭제 처리
	std::string recordFileName = getRecordFileName();
	if (FileUtils::getInstance()->isFileExist(recordFileName))
	{
		// 효과음 재생
		//(닫기 버튼이 아니고, 도움말이 아닌 경우 효과음 재생)
		if (mIsClose == false && mIsHelp == false)
		{
			AudioEngine::play2d(FILENAME_SND_EFFECT_RECORD_DELETE);				
		}		

		mIsHelp = false;

		// 파일 존재시 삭제
		FileUtils::getInstance()->removeFile(recordFileName);
		CCLOG("FILE DELETE : %s", recordFileName.c_str());

		// 플레이 타임 삭제
		setUserDataPlayTime(0.0f);
	}

	setMode(RECORD_MODE::INIT);
}


/**
* @brief   녹음하기 닫기 호출후 레이어 종료 처리
* @return  void
*/
void RecordLayer::onExit()
{
	CCLOG("[RecordLayer] onExit......");
	// 상위 클래스 호출
	Layer::onExit();
	
	AudioEngine::stopAll();
	// 이벤트 정리		
	//Director::getInstance()->getEventDispatcher()->removeEventListener(mListener);
	this->removeAllChildrenWithCleanup(true);	
}



/**
* @brief  스티커 설정(Json파일로 부터 읽어 들인 정보)
* @param  type
* @param  pos
* @return void
*/
void RecordLayer::initStickerForJsonFileWithRatio(STICKER::ITEM type, Vec2 pos, float ratio)
{
	Sprite* sp = nullptr;
	switch ((STICKER::ITEM) type)
	{
	case STICKER::BALL:
		sp = Sprite::create(FILENAME_DRAW_STICKER_BALL_P);
		break;

	case STICKER::BALLOON:
		sp = Sprite::create(FILENAME_DRAW_STICKER_BALLOON_P);
		break;

	case STICKER::BEAR:
		sp = Sprite::create(FILENAME_DRAW_STICKER_BEAR_P);
		break;

	case STICKER::BUS:
		sp = Sprite::create(FILENAME_DRAW_STICKER_BUS_P);
		break;

	case STICKER::CANDY:
		sp = Sprite::create(FILENAME_DRAW_STICKER_CANDY_P);
		break;

	case STICKER::CLOUD:
		sp = Sprite::create(FILENAME_DRAW_STICKER_CLOUD_P);
		break;

	case STICKER::FLOWER:
		sp = Sprite::create(FILENAME_DRAW_STICKER_FLOWER_P);
		break;

	case STICKER::GRASS:
		sp = Sprite::create(FILENAME_DRAW_STICKER_GRASS_P);
		break;

	case STICKER::HEART:
		sp = Sprite::create(FILENAME_DRAW_STICKER_HEART_P);
		break;

	case STICKER::LEAF:
		sp = Sprite::create(FILENAME_DRAW_STICKER_LEAF_P);
		break;

	case STICKER::PLANE:
		sp = Sprite::create(FILENAME_DRAW_STICKER_PLANE_P);
		break;

	case STICKER::RIBBON:
		sp = Sprite::create(FILENAME_DRAW_STICKER_RIBBON_P);
		break;

	case STICKER::STAR:
		sp = Sprite::create(FILENAME_DRAW_STICKER_STAR_P);
		break;

	case STICKER::TREE:
		sp = Sprite::create(FILENAME_DRAW_STICKER_TREE_P);
		break;
	}

	sp->retain();
	sp->setBlendFunc(BlendFunc::ALPHA_PREMULTIPLIED);

	// 스케일 변경으로 인한 스티커 위치 계산		
	Size size = Pos::getScreenSize();
	pos.x += size.width * ((RENDERTEXTURE_POS_X + 40.0f)/ SCREEN_SIZE_WIDTH);
	pos.y += size.height * ((RENDERTEXTURE_POS_Y + 130.0f) / SCRREN_SIZE_HEIGHT);
	pos *= (ratio);
	CCLOG("STICKER POS [%f, %f]", pos.x, pos.y);
	sp->setPosition(pos);

	// 기본 크기에서 줄임	
	sp->setScale(ratio);
	sp->setTag(type);

	this->addChild(sp);
}


/**
* @brief  녹음 플레이 타임 저장
* @return void
*/
void RecordLayer::setUserDataPlayTime(float time)
{	
	float setTime = 0.0f;
	if (time > 0.0f)
	{
		setTime = time / 1000;
	}		
	if (setTime > TIME_VOICE_RECORD)
	{
		setTime = TIME_VOICE_RECORD;
	}
	CCLOG("setUserDataPlayTime : %f, %f", time, setTime);
	std::string key = StringUtils::format(USER_DATA_PLAY_TIME_KEY_X, mWeekData, mOrderNum);
	UserDefault::getInstance()->setFloatForKey(key.c_str(), setTime);
}


/**
* @brief  녹음 플레이 타임 가져오기
* @return float
*/
float RecordLayer::getUserDataPlayTime()
{
	std::string key = StringUtils::format(USER_DATA_PLAY_TIME_KEY_X, mWeekData, mOrderNum);
	return UserDefault::getInstance()->getFloatForKey(key.c_str(), 0.0f);
}


/**
* @brief  녹음 플레이 타임 저장하기
* @return void
*/
void RecordLayer::setPlayTime()
{
	unsigned long current = getCurrentRecordTime();
	unsigned long diff = current - mPlayStartTime - SND_CORRECTION_VALUE_MSEC;// +SND_CORRECTION_VALUE_MSEC;
	CCLOG("setPlayTime : current: %ul, mPlayStartTime: %ul, diff :%ul", current, mPlayStartTime, diff);
	//float fdiff = (float)diff;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_WIN32)
	diff = 10023;
#endif 

	setUserDataPlayTime(diff);
}


/**
* @brief  녹음 플레이 타임 구하기 위한 현재시간 생성
* @return unsigned long
*/
unsigned long RecordLayer::getCurrentRecordTime()
{
	timeval time;
	gettimeofday(&time, NULL);
	unsigned long millisecs = (time.tv_sec * 1000) + (time.tv_usec / 1000);
	CCLOG("getCurrentRecordTime : %ul", millisecs);
	return millisecs;
	//return millisecs / 1000;
}


/**
* @brief  넥스트 버튼 모션 생성
* @return void
*/
void RecordLayer::createNextButtonMotion()
{
	CCLOG("createNextButtonMotion......");
	// action
	auto seq = Sequence::create(
		FadeTo::create(0.5f, 255),
		DelayTime::create(0.1f),
		FadeTo::create(0.5f, 255 * 0.3f),
		nullptr);

	if (mNextButtonMotion == nullptr)
	{
		mNextButtonMotion = RepeatForever::create(seq);
		mNextButtonMotion->retain();
	}	
}


void RecordLayer::setVisibleForNextBtn(bool b)
{
	mNextItem->setVisible(b);
	mNextItem->setEnabled(b);
	
	mNextItem->stopAction(mNextButtonMotion);

	if (b)
	{
		mNextItem->runAction(mNextButtonMotion);
	}
}


/**
* @brief  중지 버튼 모션 생성
* @return void
*/
void RecordLayer::createStopButtonMotion()
{
	CCLOG("createStopButtonMotion......");
	// action
	auto seq = Sequence::create(FadeTo::create(0.5f, 255.f), FadeTo::create(0.5f, 0.f), nullptr);

	if (mStopButtonMotion == nullptr)
	{
		mStopButtonMotion = RepeatForever::create(seq);
		mStopButtonMotion->retain();
	}
}


// 중지 버튼 모션 어포던스
void RecordLayer::setVisibleForStopBtn(bool b)
{
	mRecordStopItem->setVisible(b);
	mRecordStopItem->setEnabled(b);
	mRecordStopImg->setVisible(b);
	mRecordStopFx->setVisible(b);

	mRecordStopFx->stopAction(mStopButtonMotion);
	if (b)
	{
		mRecordStopFx->runAction(mStopButtonMotion);
	}	
}

// 도움말 버튼 활성/비활성
void RecordLayer::setVisibleForHelpBtn(bool b)
{
	if (mHelpItem == nullptr)
	{
		CCLOG("setVisibleForHelpBtn CHECK...... HELP BUTTON....");
		return;
	}
	if (b)
	{
		// 스피커 버튼 재생완료시 false
		// 스피커 버튼 재생완료중 true
		if (mDisabledHelpBtn)
		{
			CCLOG("setVisibleForHelpBtn mSpearkMode...... HELP BUTTON....DISABLED");
			return;
		}
	}

	mHelpItem->setVisible(b);
	mHelpItem->setEnabled(b);
}

// 2분 터치 
void RecordLayer::processUserTouch()
{
	// 스크린 메니져
	ScreenManager::getInstance()->screenOff(mParent);
}