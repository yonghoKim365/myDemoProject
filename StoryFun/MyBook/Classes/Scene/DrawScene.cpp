#include "DrawScene.h"
#include "MainScene.h"
#include "Contents/MyBookResources.h"
#include "Util/Pos.h"
#include "Contents/MyBookSnd.h"
// Json
#include  "Util/MBJson.h"
#include  "Manager/ScreenManager.h"

#include "MSLPManager.h"

using namespace ui;

char strNormalColors[TOOL::MAX_COLOR][FILENAME_LENGTH] = {
	FILENAME_DRAW_TOOL_COLOR_APRICOT_N, 
	FILENAME_DRAW_TOOL_COLOR_ORANGE_N,
	FILENAME_DRAW_TOOL_COLOR_PINK_N, 
	FILENAME_DRAW_TOOL_COLOR_RED_N,
	FILENAME_DRAW_TOOL_COLOR_YELLOW_N,
	FILENAME_DRAW_TOOL_COLOR_GREEN_N,
	FILENAME_DRAW_TOOL_COLOR_BLUE_N,
	FILENAME_DRAW_TOOL_COLOR_NAVY_N,
	FILENAME_DRAW_TOOL_COLOR_VIOLET_N,
	FILENAME_DRAW_TOOL_COLOR_BROWN_N,
	FILENAME_DRAW_TOOL_COLOR_GREY_N,
	FILENAME_DRAW_TOOL_COLOR_BLACK_N};

//char strPressColors[TOOL::MAX_COLOR][FILENAME_LENGTH] = {
//	FILENAME_DRAW_TOOL_COLOR_APRICOT_P,
//	FILENAME_DRAW_TOOL_COLOR_ORANGE_P,
//	FILENAME_DRAW_TOOL_COLOR_PINK_P,
//	FILENAME_DRAW_TOOL_COLOR_RED_P,
//	FILENAME_DRAW_TOOL_COLOR_YELLOW_P,
//	FILENAME_DRAW_TOOL_COLOR_GREEN_P,
//	FILENAME_DRAW_TOOL_COLOR_BLUE_P,
//	FILENAME_DRAW_TOOL_COLOR_NAVY_P,
//	FILENAME_DRAW_TOOL_COLOR_VIOLET_P,
//	FILENAME_DRAW_TOOL_COLOR_BROWN_P,
//	FILENAME_DRAW_TOOL_COLOR_GREY_P,
//	FILENAME_DRAW_TOOL_COLOR_BLACK_P};

// 스티커 14종
char strSticker[STICKER::MAX_STICKER][FILENAME_LENGTH] = {
	FILENAME_DRAW_STICKER_BALL_N, FILENAME_DRAW_STICKER_BALLOON_N,
	FILENAME_DRAW_STICKER_BEAR_N, FILENAME_DRAW_STICKER_BUS_N,
	FILENAME_DRAW_STICKER_CANDY_N, FILENAME_DRAW_STICKER_CLOUD_N,
	FILENAME_DRAW_STICKER_FLOWER_N, FILENAME_DRAW_STICKER_GRASS_N,
	FILENAME_DRAW_STICKER_HEART_N, FILENAME_DRAW_STICKER_LEAF_N,
	FILENAME_DRAW_STICKER_PLANE_N, FILENAME_DRAW_STICKER_RIBBON_N,
	FILENAME_DRAW_STICKER_STAR_N, FILENAME_DRAW_STICKER_TREE_N};
// 스티커 14종(프레스드이미지)
char strStickerPressed[STICKER::MAX_STICKER][FILENAME_LENGTH] = {
	FILENAME_DRAW_STICKER_BALL_P, FILENAME_DRAW_STICKER_BALLOON_P,
	FILENAME_DRAW_STICKER_BEAR_P, FILENAME_DRAW_STICKER_BUS_P,
	FILENAME_DRAW_STICKER_CANDY_P, FILENAME_DRAW_STICKER_CLOUD_P,
	FILENAME_DRAW_STICKER_FLOWER_P, FILENAME_DRAW_STICKER_GRASS_P,
	FILENAME_DRAW_STICKER_HEART_P, FILENAME_DRAW_STICKER_LEAF_P,
	FILENAME_DRAW_STICKER_PLANE_P, FILENAME_DRAW_STICKER_RIBBON_P,
	FILENAME_DRAW_STICKER_STAR_P, FILENAME_DRAW_STICKER_TREE_P };
// 스티커 14종(비활성)
//char strStickerDisabled[STICKER::MAX_STICKER][FILENAME_LENGTH] = {
//	FILENAME_DRAW_STICKER_BALL_D, FILENAME_DRAW_STICKER_BALLOON_D,
//	FILENAME_DRAW_STICKER_BEAR_D, FILENAME_DRAW_STICKER_BUS_D,
//	FILENAME_DRAW_STICKER_CANDY_D, FILENAME_DRAW_STICKER_CLOUD_D,
//	FILENAME_DRAW_STICKER_FLOWER_D, FILENAME_DRAW_STICKER_GRASS_D,
//	FILENAME_DRAW_STICKER_HEART_D, FILENAME_DRAW_STICKER_LEAF_D,
//	FILENAME_DRAW_STICKER_PLANE_D, FILENAME_DRAW_STICKER_RIBBON_D,
//	FILENAME_DRAW_STICKER_STAR_D, FILENAME_DRAW_STICKER_TREE_D };

// 지우기 (DrawNode용)
//ccBlendFunc blendFunc = { GL_ONE, GL_ZERO };
BlendFunc blendFunc = { GL_ONE, GL_ONE };
//ccBlendFunc blendFunc = { GL_ONE, GL_ONE_MINUS_SRC_ALPHA };

// 지우기 (스프라이트용)
BlendFunc blendFuncForSprite = { GL_ZERO, GL_ONE_MINUS_SRC_ALPHA };


Scene* DrawScene::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = DrawScene::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

Layer* DrawScene::createLayer()
{	
	// 'layer' is an autorelease object
	auto layer = DrawScene::create();

	// return the scene
	return layer;
}


Layer* DrawScene::createLayer(Node* parent, int weekData, int ord, std::string filename)
{
	// 'layer' is an autorelease object
	auto layer = DrawScene::create();
	
	layer->mParent        = parent;
	layer->mWeekData      = weekData;
	layer->mOrderNum      = ord;
	layer->mFilename      = filename;	
	layer->mIsEnabledSave = false; // 버튼 활성화 여부
		
	// 배경 그림 초기화 및 스티커 영역 초기화	
	layer->initBG();	
	layer->initDraw();	
	layer->loadData();	  // 기존 데이터가 존재하는 경우 불러오기
	layer->createPopup(); // 최초 집입시 팝업 생성

	layer->processUserTouch();
	
	// return the scene
	return layer;
}

// 2분동안 미입력시 화면 끄기
void DrawScene::processUserTouch()
{
	// 스크린 메니져
	ScreenManager::getInstance()->screenOff(mParent);
}

// 버튼 활성화 여부
bool DrawScene::getIsEnabledSave()
{
	return mIsEnabledSave;
}

// 버튼 활성/ 비활성
void DrawScene::setEnableTouchBtn(bool b)
{
	//CCLOG("setEnableTouchBtn................: %d", b);
	////////////////////////////////////////
	// 칼러 툴
	int cnt = mToolMgr->mColors.size();
	for (int i = 0; i < cnt; i++)
	{
		mToolMgr->mColors[i]->setTouchEnabled(b);
	} // end of for
	
	cnt = mToolMgr->mBtns.size();
	for (int i = 0; i < cnt; i++)
	{
		mToolMgr->mBtns[i]->setTouchEnabled(b);
	} // end of for

	//////////////////////////////////////////
	//// 스티커
	ui::ScrollView* scrollView = mStickerMgr->getScrollView();
	cnt = scrollView->getChildrenCount();
	for (int i = 0; i < cnt; i++)
	{
		ui::Button* stickerBtn = dynamic_cast<ui::Button*>(scrollView->getChildren().at(i));				
		stickerBtn->setEnabled(b);
	} // end of for
	
	cnt = mStickerMgr->mBtns.size();
	for (int i = 0; i < cnt; i++)
	{
		mStickerMgr->mBtns[i]->setTouchEnabled(b);		
	} // end of for

	// 나가기 버튼	
	mBtnBack->setEnabled(b);	
}


// 최초 진입시 팝업 생성
void DrawScene::createPopup()
{	
	std::string strKey = StringUtils::format(USERDEFAULT_KEY_DRAW_HELP_CHECK, MSLPManager::getInstance()->getBookNum());
	if (UserDefault::getInstance()->getBoolForKey(strKey.c_str()) == false)
	{		
		auto mPopupInit = PopupLayer::getIns(this, HELP_DRAW);
		((PopupLayer*)mPopupInit)->showHide(true);	
		
		// 버튼 클릭 막기
		setEnableTouchBtn(false);
	}	
	else
	{
		// 진입 나레이션
		// narration		
		// 최초만 재생하기 위해서 저장
		if (MBJson::getInstance()->getIsOpenningDraw() == false)
		{
			SimpleAudioEngine::getInstance()->playEffect(FILENAME_SND_DRAW_OPENNING);	
			MBJson::getInstance()->setIsOpenningDraw(true);
		}
	}	
}



// 맞는 배경 이미지 불러오기
void DrawScene::initBG()
{	
	//CCLOG("initBG............");
	// 맞는 배경 이미지 불러오기
	Sprite* spBG = Sprite::create(mFilename);
	spBG->setPosition(mCanvas->getPosition());	
	this->addChild(spBG);
	
	// 먹선 이미지
	int size = mFilename.size();
	std::string strLineFileName = mFilename.substr(0, size - 4);
	std::string strFileName = StringUtils::format(FILENAME_DRAW_LINE, strLineFileName.c_str());
	Sprite* spLine = Sprite::create(strFileName);
	spLine->setPosition(mCanvas->getPosition());
	this->addChild(spLine, DEPTH_LAYER_DRAW_LINE);
	
	// 라운드 처리된 이미지 배경
	Sprite* spRoundBG = Sprite::create(FILENAME_DRAW_CUBE_BG);
	spRoundBG->setPosition(Pos::getCenterPt());
	this->addChild(spRoundBG, DEPTH_LAYER_DRAW_CANVAS_BG);	
	
	////////////////////////////////////////////
	// 스티커 적용 영역 저장
	mBGBoundingBoxForSticker = spBG->getBoundingBox();
}


// 그림판 이미지 및 스티커 로드
void DrawScene::loadData()
{
	//CCLOG("loadData......");

	Vec2 pos = mCanvas->getPosition();
	
	////////////////////////////////////////
	// 기존 저장된 이미지가 존재하는 경우
	// 불러오기
	std::string strPath = FileUtils::getInstance()->getWritablePath();	
	std::string strFullPath = strPath + StringUtils::format(FILENAME_DRAW_CUBE_IMG_X, mWeekData, mOrderNum);

	// 파일 존재 유무 확인
	if (FileUtils::getInstance()->isFileExist(strFullPath) == false)
	{
		CCLOG("NOT FOUND FILE [%s]", strFullPath.c_str());
		return;
	}

	////////////////////////////////////////
	// 기존 그림이 존재하는 경우 불러오기
	auto brushTexture = TextureCache::getInstance()->addImage(strFullPath.c_str());
	//CCLOG("IMG [%s]", strFullPath.c_str());
	// 캔버스 위치 보정값
	Vec2 initPos = Pos::getRendererTexturePt();
	pos -= initPos;
	//Size bsize = brushTexture->getContentSize();
	// mValue = 2 일경우 bsize /2 로 처리
	/*if (mValue == 2)
	{
		pos -= (initPos + bsize / 2);
	}
	else
	{
		pos -= (initPos + bsize);
	}*/		

	mCanvas->begin();
	Sprite* brush = Sprite::createWithTexture(brushTexture);	
	brush->setPosition(pos);	
	brush->visit();
	mCanvas->end();
	mRenderer->render();
	// 기존에 추가된 텍스처를 삭제
	TextureCache::getInstance()->removeTexture(brushTexture);

	////////////////////////////////////////
	// 기존 저장된 스티커 이미지가 존재하는 경우
	// 불러오기		
	std::string strFullPathSticker = strPath + StringUtils::format(FILENAME_DRAW_CUBE_IMG_STICKER_X, mWeekData, mOrderNum);

	// 파일 존재 유무 확인
	if (FileUtils::getInstance()->isFileExist(strFullPathSticker) == false)
	{
		CCLOG("NOT FOUND FILE [%s]", strFullPathSticker.c_str());
		return;
	}

	////////////////////////////////////////
	// 기존 그림이 존재하는 경우 불러오기
	auto brushTextureSticker = TextureCache::getInstance()->addImage(strFullPathSticker.c_str());
	
	// 캔버스 위치 보정값
	//Vec2 initPos = Pos::getRendererTexturePt();
	//pos -= initPos;

	mCanvasSticker->begin();
	Sprite* brushSticker = Sprite::createWithTexture(brushTextureSticker);
	brushSticker->setPosition(pos);
	brushSticker->visit();
	mCanvasSticker->end();
	mRenderer->render();
	// 기존에 추가된 텍스처를 삭제
	TextureCache::getInstance()->removeTexture(brushTextureSticker);
}


// on "init" you need to initialize your instance
bool DrawScene::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}
	//CCLOG("INIT START...");
	visibleSize = Director::getInstance()->getVisibleSize();
	origin = Director::getInstance()->getVisibleOrigin();

	// brush
	//initDraw();
	// bgm
	//initBGM();

	// touch event
	initTouchEvent();
	// init Canvas
	initCanvas();	
	initSizeMaxSticker(SIZE_MAX_STICKER);

	//CCLOG("INIT FINISH...");
	return true;
}

//void DrawScene::initBGM()
//{
//	if (SimpleAudioEngine::getInstance()->isBackgroundMusicPlaying())
//	{
//		SimpleAudioEngine::getInstance()->stopBackgroundMusic(true);		
//	}
//	SimpleAudioEngine::getInstance()->playBackgroundMusic(FILENAME_SND_BGM_DRAW, true);
//}

void DrawScene::initSizeMaxSticker(int maxCnt)
{
	for (int i = 0; i < STICKER::MAX_STICKER; i++)
	{
		mStickerMaxCnt[i] = maxCnt;
	} // end of for
}

/**
* @brief  켄버스 초기화
* @return void
*/
void DrawScene::initCanvas()
{
	mCanvas = getRenderTexture();
	mCanvas->clear(0, 0, 0, 0);	
	this->addChild(mCanvas, DEPTH_LAYER_DRAW_CANVAS);	

	mCanvasSticker = getRenderTexture();
	mCanvasSticker->clear(0, 0, 0, 0);
	this->addChild(mCanvasSticker, DEPTH_LAYER_DRAW_STICKER);
}

/**
* @brief  렌더러 텍스처 구하기
* @return RenderTexture*
*/
RenderTexture* DrawScene::getRenderTexture()
{	
	// 디자인 사이즈 (1920, 1200)
	Size renderSize = Size(SCREEN_SIZE_WIDTH, SCRREN_SIZE_HEIGHT);

	/*renderSize.width *= (1.0f - (RENDERTEXTURE_WIDTH_FOR_SCALE / SCREEN_SIZE_WIDTH));
	renderSize.height *= (1.0f - (RENDERTEXTURE_HEIGHT_FOR_SCALE / SCRREN_SIZE_HEIGHT));*/
	renderSize.width  *= (RENDERTEXTURE_SIZE_WIDTH / SCREEN_SIZE_WIDTH);
	renderSize.height *= (RENDERTEXTURE_SIZE_HEIGHT / SCRREN_SIZE_HEIGHT);

	// 해상도 문제로 인하여 사이즈 값 설정(1920, 1200) -> 1280,800 둘다 지원
	if (visibleSize.height == LGGPad2ResulutionSize.height &&
		visibleSize.width  == LGGPad2ResulutionSize.width)
	{		
		// 1280, 800		
		// 디자인 사이즈가 1920, 1200 이므로 스케일링 필요
		renderSize = renderSize * 2 / 3;
	}
	else
	{
		// 1920, 1200
	}

	// 초기 위치값
	mInitRendererTexturePt = Pos::getRendererTexturePt();
	RenderTexture* rt = RenderTexture::create(renderSize.width / mValue,
											  renderSize.height / mValue,
											  Texture2D::PixelFormat::RGBA8888);
	
	rt->setPosition(Vec2(renderSize.width  / 2 + mInitRendererTexturePt.x,
						 renderSize.height / 2 + mInitRendererTexturePt.y));
	rt->retain();	
	rt->setScale(mValue);
	rt->setTag(TAG_CANVAS);

	/*CCLOG("=================================================================");
	CCLOG("CREATE CANVAS SIZE [%f, %f], SCALE : %f", renderSize.width / mValue, renderSize.height / mValue, mValue);
	CCLOG("ORIGIN SIZE [%f, %f]", origin.x, origin.y);
	CCLOG("POS [%f, %f]", renderSize.width / 2, renderSize.height / 2);
	CCLOG("INIT POS [%f, %f]", mInitRendererTexturePt.x, mInitRendererTexturePt.y);
	CCLOG("=================================================================");*/
	return rt;
}

/**
* @brief  그리기 초기화
* @return void
*/
void DrawScene::initDraw()
{	
	// Button
	createUi();

	// preloading
	mLoadingLayer = LoadingLayer::createLayer(this, mWeekData, mOrderNum);
	this->addChild(mLoadingLayer, 100);
	mLoadingLayer->setVisible(false);
	
	// Tool & Sticker
	initLayout();
	
	// 그리기 초기화 (브러쉬 & 컬러)
	initBrush();
}

/**
* @brief  그리기 초기화 (브러쉬 & 컬러)
* @return void
*/
void DrawScene::initBrush()
{
	Sprite* brush = Sprite::create(FILENAME_DRAW_TOOL_BRUSH_ERASER);
	//brush->retain();
	//mBrushSize = brush->getBoundingBox().size / 2;	
	mBrushTexture = TextureCache::getInstance()->addImage(FILENAME_DRAW_TOOL_BRUSH_ERASER);	

	// 파레트 색상
	mColorList = std::vector<Color3B>();
	mColorList.push_back(COLOR_ARPICOT);   // arpicot
	mColorList.push_back(COLOR_ORANGE);    // (255, 165, 0)); // orange
	mColorList.push_back(COLOR_PINK);  // PINK
	mColorList.push_back(COLOR_RED);   // Color3B(255, 0, 0)); // red	
	mColorList.push_back(COLOR_YELLOW);//(255, 255, 0)); // yellow	
	mColorList.push_back(COLOR_GREEN); // (0, 128, 0)); // green
	mColorList.push_back(COLOR_BLUE);  // (0, 0, 255)); // blue
	mColorList.push_back(COLOR_NAVY);  // 남색
	mColorList.push_back(COLOR_VIOLET); // 보라색
	mColorList.push_back(COLOR_BROWN);  // 갈색
	mColorList.push_back(COLOR_GRAY);   // 회색
	mColorList.push_back(COLOR_BLACK);  // (0, 0, 0)); // black	

	// Renderer
	mRenderer = Director::getInstance()->getRenderer();
}


/**
* @brief  UI 만들기
* @return void
*/
void DrawScene::createUi()
{
	// createUi
	// 메인씬으로 돌아가기 버튼
	mBtnBack = MenuItemImage::create(FILENAME_BTN_CLOSE_N,
		FILENAME_BTN_CLOSE_P,
		CC_CALLBACK_1(DrawScene::onBackBtnClick, this));
	mBtnBack->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mBtnBack->setPosition(Pos::getBackBtnPt());

	mMenuBack = Menu::create(mBtnBack, nullptr);
	mMenuBack->setPosition(Vec2::ZERO);
	this->addChild(mMenuBack, DEPTH_LAYER_DRAW_INIT_LAYOUT, BTN_TAG_DRAW::BACK);
}


/**
* @brief  레이아웃 초기화
* @return void
*/
void DrawScene::initLayout()
{
	//CCLOG("[DRAWSCENE] initLayout");
	//  화면 사이즈 구하기
	Size screenSize = Pos::getScreenSize();
	
	////////////////////////////////////////
	// 칼러선택 레이아웃
	Layout* layout = Layout::create();
	
	Point basePt = Pos::getToolBasePt();
	Vec2 pos;
	pos.x = basePt.x;

	// 칼라선택 레이아웃 배경
	Sprite* bg = Sprite::create(FILENAME_DRAW_TOOL_PANEL);
	bg->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);	
	bg->setPosition(basePt);
	this->addChild(bg, DEPTH_LAYER_DRAW_INIT_LAYOUT);

	pos.x = 33.0f / SCREEN_SIZE_WIDTH * screenSize.width;
	pos.y = 30.0f / SCRREN_SIZE_HEIGHT * screenSize.height;

	// 초기 여백 위치값
	const float initX = 33.0f / SCREEN_SIZE_WIDTH * screenSize.width;
	const float initY = basePt.y - (30.0f / SCRREN_SIZE_HEIGHT * screenSize.height);

	// 칼라간 간격
	const float unitX = 18.0f / SCREEN_SIZE_WIDTH * screenSize.width;
	const float unitY = 23.0f / SCRREN_SIZE_HEIGHT * screenSize.height;

	// 칼라 배치
	for (int i = 0; i < TOOL::MAX_COLOR; i++)
	{
		ui::Button* sp = ui::Button::create(strNormalColors[i]);		
		Size size = sp->getContentSize();
		
		if (i % 2 == 0)
		{
			// 짝수인 경우 (0, 2, 4 ..)
			// 왼쪽 칼라 : ARPICOT, PINK, YELLOW, BLUE, VIOLET, GREY
			pos.x = initX + size.width / 2;			
			pos.y = (initY - size.height / 2) - (unitY + size.height) *  i / 2;
		}
		else
		{
			// 홀수인 경우 			
			// 오른쪽 칼라 : ORANGE, RED, GREEN, NAVY, BROWN, BLACK
			pos.x = initX + size.width + unitX + (size.width / 2);
			//pos.y = (initY + size.height / 2) + (unitY + size.height) *  i / 2;
			//CCLOG("1 : %f, 2 : %f, 3 : %d", (initY + size.height / 2), (unitY + size.height), i / 2);
		}		
		
		sp->setPosition(pos);		
		layout->addChild(sp);
	} // end of for

	// 지우기 버튼
	ui::Button* btn = ui::Button::create(FILENAME_DRAW_TOOL_PANEL_CLEAR_N);	
	Vec2 posBtn = Pos::getToolColorErasePt();
	Size btnSize = btn->getContentSize();
	posBtn.x += btnSize.width / 2;
	posBtn.y -= btnSize.height / 2;
	btn->setPosition(posBtn);
	layout->addChild(btn);

	// 모두 지우기 버튼	
	ui::Button* btnAll = ui::Button::create(FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_N);	
	posBtn = Pos::getToolColorAllErasePt();
	btnSize = btnAll->getContentSize();
	posBtn.x += btnSize.width / 2;
	posBtn.y -= btnSize.height / 2;
	btnAll->setPosition(posBtn);
	layout->addChild(btnAll);

	// 칼라 툴 레이어
	this->addChild(layout, DEPTH_LAYER_DRAW_INIT_LAYOUT, BTN_TAG_DRAW::TOOL);
	mToolMgr = new ToolManager(layout, this);
	
	////////////////////////////////////////
	// 스티커 레이아웃
	Layout* stikerLayout = Layout::create();
	
	// 기본 스티커 위치
	basePt = Pos::getStickerBasePt();		

	// 스티커 레이아웃 배경
	Sprite* bgSticker = Sprite::create(FILENAME_DRAW_STICKER_PANEL);
	bgSticker->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	bgSticker->setPosition(basePt);
	this->addChild(bgSticker, DEPTH_LAYER_DRAW_INIT_LAYOUT);

	////////////////////////////////////////
	// 스크롤 뷰
	mScrollView = ui::ScrollView::create();
	mScrollView->setDirection(ui::ScrollView::Direction::HORIZONTAL);

	Size scrollViewContentSize = bgSticker->getContentSize();
	scrollViewContentSize.height -= (30.0f / SCRREN_SIZE_HEIGHT * screenSize.height);
	scrollViewContentSize.width  -= (120.0f / SCREEN_SIZE_WIDTH * screenSize.width);
	mScrollView->setContentSize(scrollViewContentSize);
	mScrollView->setScrollBarEnabled(false);
	
	//total scroll view size(아래 스티커 크기 계산후 설정)
	//scrollview->setInnerContainerSize(Size(scrollViewContentSize.width * 1.8, scrollViewContentSize.height));

	mScrollView->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);	
	mScrollView->setPosition(Pos::getScrollViewWithLeftBtnPt());
	mScrollView->setTouchEnabled(false);
	////////////////////////////////////////

	// 스티커 패드 크기 계산
	Size bgsize = bgSticker->getContentSize(); 		
	pos.x = 0.0f;
	pos.y = bgsize.height / 2;
	const float unitXSticker      = (30.0f / SCREEN_SIZE_WIDTH) * screenSize.width; // 35
	const float unitXStickerLarge = (40.0f / SCREEN_SIZE_WIDTH) * screenSize.width; // 40

	// 스티커 설정
	for (int i = 0; i < STICKER::MAX_STICKER; i++)
	{		
		//setSwallowTouches touch enabled를 false로 처리 해야 됨.			
		//ui::Button* btn = ui::Button::create(strSticker[i], strStickerPressed[i], strStickerDisabled[i]);					
		ui::Button* btn = ui::Button::create(strSticker[i], strStickerPressed[i], strSticker[i]);
		btn->setZoomScale(0.0f);		
		Size size = btn->getContentSize();
		pos.x += size.width / 2;
		btn->setPosition(pos);
		if (i < STICKER::MAX_STICKER / 2)
		{
			pos.x += unitXStickerLarge + size.width / 2;
		}
		else
		{
			pos.x += unitXSticker + size.width / 2;
		}		
				
		mScrollView->addChild(btn);		
	} // end of for
	
	////////////////////////////////////////	
	// 전체 스크롤뷰 크기 계산	
	mScrollView->setInnerContainerSize(Size(pos.x, scrollViewContentSize.height));
	// 이벤트 리스너 등록
	initTouchEventForScrollView();
	////////////////////////////////////////

	// 스티커 레이어에 추가
	stikerLayout->addChild(mScrollView, 1, TAG_BTN_STICKER);

	////////////////////////////////////////
	// 버튼
	// 저장 버튼	
	ui::Button* btnSave = ui::Button::create(FILENAME_DRAW_TOOL_SAVE_N, FILENAME_DRAW_TOOL_SAVE_P, FILENAME_DRAW_TOOL_SAVE_P);
	btnSave->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	btnSave->setPosition(Pos::getDrawSaveBtnPt());
	stikerLayout->addChild(btnSave, 0, BTN_TAG_DRAW::STICKER_SAVE);


	// 스크롤 좌 버튼
	ui::Button* btnLeft = ui::Button::create(FILENAME_DRAW_STICKER_LEFT_BTN_N, FILENAME_DRAW_STICKER_LEFT_BTN_P);	
	btnLeft->setPosition(Pos::getStickerLeftBtnPt());
	stikerLayout->addChild(btnLeft, 0, BTN_TAG_DRAW::STICKER_LEFT);
	// 스크롤 우 버튼
	ui::Button* btnRight = ui::Button::create(FILENAME_DRAW_STICKER_RIGHT_BTN_N, FILENAME_DRAW_STICKER_RIGHT_BTN_P);	
	btnRight->setPosition(Pos::getStickerRightBtnPt());
	stikerLayout->addChild(btnRight, 0, BTN_TAG_DRAW::STICKER_RIGHT);
	////////////////////////////////////////

	this->addChild(stikerLayout, DEPTH_LAYER_DRAW_INIT_LAYOUT);

	// 저장 버튼
	mSpBtnDisabledSave = Sprite::create(FILENAME_DRAW_TOOL_SAVE_D);
	mSpBtnDisabledSave->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	mSpBtnDisabledSave->setPosition(Pos::getDrawSaveBtnPt());
	this->addChild(mSpBtnDisabledSave, DEPTH_LAYER_DRAW_INIT_LAYOUT);

	mStickerMgr = new StickerManager(stikerLayout, this);
		
	// 스티커 레이어
	mStickerLayer = Layer::create();
	mStickerLayer->setAnchorPoint(Vec2::ZERO);
	this->addChild(mStickerLayer, DEPTH_LAYER_DRAW_STICKER, BTN_TAG_DRAW::STICKER);

	// 스티커 초기화
	mStickerSp = nullptr;	
}


/**
* @brief  돌아가기 버튼 이벤트
* @return void
*/
void DrawScene::onBackBtnClick(Ref* sender)
{	
	//효과음
	cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_GNB_BTN_CLICK);

	//CCLOG("onBackBtnClick..........");
	mPopup = PopupLayer::getIns(this, DRAW_CLOSE);
	mPopup->showHide(true, false);
	// 플레그 초기화
	mDrawMode = DRAW_MODE::NOT_USED;

	setEnableTouchBtn(false);	
}


/**
* @brief  팝업에서 돌아가기 버튼 이벤트(메인씬으로 돌아가기)
* @return void
*/
void DrawScene::backToMainScene()
{

	mPopup->showHide(false, false);
	((MainScene*)mParent)->closeDrawLayer();	
}


//------------------------------------------------------------
// 터치 이벤트
//------------------------------------------------------------
/**
* @brief  터치 초기화
* @return void
*/
void DrawScene::initTouchEvent()
{
	mIsStickerTouch = false;
	// touch event	
	//
	/*
	mListener = EventListenerTouchOneByOne::create();
	mListener->onTouchBegan = CC_CALLBACK_2(DrawScene::onTouchBegan, this);
	mListener->onTouchMoved = CC_CALLBACK_2(DrawScene::onTouchMoved, this);
	mListener->onTouchEnded = CC_CALLBACK_2(DrawScene::onTouchEnded, this);
	mListener->setSwallowTouches(false);
	// 터치리스너의 우선순위를 (노드가) 화면에 그려진 순서대로 한다.
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(mListener, this);
	*/
	
	mListener = EventListenerTouchAllAtOnce::create();
	mListener->onTouchesBegan = CC_CALLBACK_2(DrawScene::onTouchesBegan, this);
	mListener->onTouchesMoved = CC_CALLBACK_2(DrawScene::onTouchesMoved, this);
	mListener->onTouchesEnded = CC_CALLBACK_2(DrawScene::onTouchesEnded, this);	
	mListener->onTouchesCancelled = CC_CALLBACK_2(DrawScene::onTouchesCancelled, this);
	
	Director::getInstance()->getEventDispatcher()->addEventListenerWithFixedPriority(mListener, 1);		
}


void DrawScene::onTouchesBegan(const std::vector<Touch*>& touches, Event* event)
//bool DrawScene::onTouchBegan(Touch* touch, Event* event)
{
	//CCLOG("[DRAW] TOUCH BEGAN COLOR : %d, Mode : %d", mSelectColor, mDrawMode);
	
	// 위치 확인하여 스티커 모드 확인
	mTouchStart = touches[0]->getLocation();
	mTouch = touches[0];
	/*
	mTouchStart = touch->getLocation();
	mTouch = touch;
	*/

	if (mDrawMode == DRAW_MODE::DRAW)
	{
		if (mSelectColor >= 0 && mSelectColor < TOOL::MAX_COLOR)			
		{
			mBrushColor = mColorList[mSelectColor];
		}
	}	

}


void DrawScene::onTouchesMoved(const std::vector<Touch*>& touches, Event* event)
//void DrawScene::onTouchMoved(Touch* touch, Event* event)
{	
	mTouch = touches[0];
	//mTouch = touch;
	
	// Draw
	switch (mDrawMode)
	{
		case DRAW_MODE::START:
			break;

		case DRAW_MODE::DRAW:
			drawCanvas(touches[0]);
			//drawCanvas(touch);
			break;

		//case DRAW_MODE::STICKER_ON:
			//break;

		case DRAW_MODE::STICKER_SET:	
			if (mStickerMgr->mIsMoveInitPositionSticker == false)
			{
				dragSticker(touches[0]);
				//dragSticker(touch);
			}			
			break;

		case DRAW_MODE::CLEAR:
			eraserCanvas(touches[0]);
			//eraserCanvas(touch);
			break;

		default:
			break;
	} // end of switch	

	// 변경 된 경우만 변경
	if (mIsEnabledSave != mIsEnabledSaveBefore)
	{
		//CCLOG("SET SAVE ENABLED....");
		// 비활성화에서 활성화로 변경
		mSpBtnDisabledSave->setVisible(false);
		mStickerMgr->mBtns[0]->setEnabled(true);
		mIsEnabledSaveBefore = mIsEnabledSave;
	}
}


void DrawScene::onTouchesEnded(const std::vector<Touch*>& touches, Event* event)
//void DrawScene::onTouchEnded(Touch* touch, Event* event)
{
	//CCLOG("[DRAW] TOUCH END MODE : %d", mDrawMode);
	mTouch = touches[0];
	//mTouch = touch;

	// Draw
	switch (mDrawMode)
	{
	//case DRAWMODE::ALL_CLEAR: // 터치후 실행
	//	allClearCanvas();
	//	break;

	case DRAW_MODE::UNDO:
		break;

	case DRAW_MODE::DONE:
		break;

	case DRAW_MODE::STICKER_SET:
		if (mStickerMgr->mIsMoveInitPositionSticker == false)
		{
			// 스티커 완료
			//CCLOG("STICKER COMPLETE....");
			setSticker();
			// 기본으로 그리기 모드에서 DONE 모드로 변경
			setDrawMode(DRAW_MODE::DONE);
			//setDrawMode(DRAW_MODE::DRAW);
			// 색칠하기 버튼 초기화
			mToolMgr->resetColorTool();

			// 초기화
			mIsStickerTouch = false;
		}		
		break;

	default:
		break;
	} // end of switch	

	// 변경 된 경우만 변경
	if (mIsEnabledSave != mIsEnabledSaveBefore)
	{
		//CCLOG("SET SAVE ENABLED....");
		// 비활성화에서 활성화로 변경
		mSpBtnDisabledSave->setVisible(false);
		mStickerMgr->mBtns[0]->setEnabled(true);
		mIsEnabledSaveBefore = mIsEnabledSave;
	}

}


void DrawScene::onTouchesCancelled(const std::vector<Touch*>& touches, Event* event)
{
	if (mStickerMgr->mIsMoveInitPositionSticker == false)
	{
		// 스티커 완료
		//CCLOG("STICKER COMPLETE....");
		setSticker();
		// 기본으로 그리기 모드에서 DONE 모드로 변경
		setDrawMode(DRAW_MODE::DONE);
		//setDrawMode(DRAW_MODE::DRAW);
		// 색칠하기 버튼 초기화
		mToolMgr->resetColorTool();

		// 초기화
		mIsStickerTouch = false;
	}

	// 변경 된 경우만 변경
	if (mIsEnabledSave != mIsEnabledSaveBefore)
	{
		//CCLOG("SET SAVE ENABLED....");
		// 비활성화에서 활성화로 변경
		mSpBtnDisabledSave->setVisible(false);
		mStickerMgr->mBtns[0]->setEnabled(true);
		mIsEnabledSaveBefore = mIsEnabledSave;
	}
}

/*
void DrawScene::onTouchCancelled(Touch* touch, Event* event)
{

}
*/

//------------------------------------------------------------
// Drawing / Eraser / Drag Sticker
//------------------------------------------------------------
/**
* @brief  그리기 
* @return void
*/
void DrawScene::drawCanvas(Touch* touch)
{
	start = touch->getLocation();
	end   = touch->getPreviousLocation();
	
	mCanvas->begin();

	distance = start.getDistance(end);
	if (distance > 1)
	{
		d = (int)distance;

		difx = end.x - start.x;
		dify = end.y - start.y;		

		for (int i = 0; i < d; ++i)
		{			
			delta = (float)i / distance;
			x = start.x + (difx * delta) - mInitRendererTexturePt.x;
			y = start.y + (dify * delta) - mInitRendererTexturePt.y;

			Sprite* brush = Sprite::createWithTexture(mBrushTexture);
			brush->setColor(mBrushColor);
			//brush->setOpacity(15);			
			brush->setScale(0.7f);
			//brush->setScale(1 / mValue);
			
			////////////////////////////////////////
			// 크레파스 처리
			//brush->setOpacity(50);
			//brush->setRotation(rand() % 360);
			//scale = rand() % 50 / 200.f + 0.15f;
			//brush->setScale(scale);
			//CCLOG("SCALE: %f", scale);
			////////////////////////////////////////			
			
			//brush->setPosition(Vec2(x / mValue, y / mValue));
			brush->setPosition(Vec2(x, y));
			brush->visit();	
			mIsEnabledSave = true;
		} // end of for
	} // end of if
	
	mCanvas->end();	
	mRenderer->render();
}

/**
* @brief  켄버스에서 지우기
* @return void
*/
void DrawScene::eraserCanvas(Touch* touch)
{
	start = touch->getLocation();
	end = touch->getPreviousLocation();
	
	mCanvas->begin();
	distance = start.getDistance(end);
	if (distance > 1)
	{
		d = (int)distance;

		difx = end.x - start.x;
		dify = end.y - start.y;
		
		for (int i = 0; i < d; i = i + SIZE_ERASE_COUNT_UNIT)
		{			
			delta = (float)i / distance;
			mErasePos.x = start.x + (difx * delta) - mInitRendererTexturePt.x;
			mErasePos.y = start.y + (dify * delta) - mInitRendererTexturePt.y;
			
			Sprite* brush = Sprite::createWithTexture(mBrushTexture);			
			brush->setBlendFunc(blendFuncForSprite);			
			//brush->setScale(1 / mValue);
			//brush->setPosition(Vec2(x / mValue, y / mValue));
			brush->setPosition(mErasePos);
			brush->visit();				
			
		} // end of for
	} // end of if	
	mCanvas->end();
	mRenderer->render();

	// 스티커 지우기
	mCanvasSticker->begin();	
	if (distance > 1)
	{				
		for (int i = 0; i < d; i = i + SIZE_ERASE_COUNT_UNIT)
		{			
			delta = (float)i / distance;
			mErasePos.x = start.x + (difx * delta) - mInitRendererTexturePt.x;
			mErasePos.y = start.y + (dify * delta) - mInitRendererTexturePt.y;

			Sprite* brush = Sprite::createWithTexture(mBrushTexture);
			brush->setBlendFunc(blendFuncForSprite);						
			brush->setPosition(mErasePos);
			brush->visit();
		} // end of for
	} // end of if

	mCanvasSticker->end();
	mRenderer->render();	
}


/**
* @brief  스티커 레이어에 스티커 움직이기
* @return void
*/
void DrawScene::dragSticker(Touch* touch)
{		
	start = touch->getLocation();
	end = touch->getPreviousLocation();

	float yinit = Pos::getStickerSelectPt();
	
	distance = start.getDistance(end);
	if (distance > 1)
	{
		d = (int)distance;

		for (int i = 0; i < d; ++i)
		{
			difx = end.x - start.x;
			dify = end.y - start.y;
			delta = (float)i / distance;
			x = start.x + (difx * delta);
			y = start.y + (dify * delta) + yinit;

			mStickerSp->setPosition(Vec2(x, y));	
			
		} // end of for
	}
}


/**
* @brief  스티커 정보 저장
*         
*/
void DrawScene::setSticker()
{
	// 터치 종료후 스티커 정보 저장
	//mStickerList.push_back()

	// 스티커 최종 위치가 그리기 영역이외의 영역인 경우 원래 위치로 이동
	// 원래 위치 :mStickerOrginalPos
	CC_ASSERT(mStickerSp != nullptr);
	
	Rect rect = mStickerSp->getBoundingBox();

	// 스티커 중심점이 그리기 영역내에 존재하는 경우 
	if (mBGBoundingBoxForSticker.containsPoint(Vec2(rect.getMidX(), rect.getMidY())))
	{
		CCLOG("SUCCESS.....");
		//  효과음 재생		
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_DRAW_SET_STICKER);


		float minX = rect.getMinX();
		float minY = rect.getMinY();
		float maxX = rect.getMaxX();
		float maxY = rect.getMaxY();

		float bgMinX = mBGBoundingBoxForSticker.getMinX();
		float bgMinY = mBGBoundingBoxForSticker.getMinY();
		float bgMaxX = mBGBoundingBoxForSticker.getMaxX();
		float bgMaxY = mBGBoundingBoxForSticker.getMaxY();

		Vec2 pos = mStickerSp->getPosition();
		
		// 위치 이동이 필요한 경우 위치 이동
		// 최소값 
		if (minX < bgMinX)
		{			
			pos.x += (bgMinX - minX) + THRESHOLD_STICKER_X;
		}
		
		if (minY < bgMinY)
		{
			pos.y += (bgMinY - minY) + THRESHOLD_STICKER_Y;
		}

		if (maxX > bgMaxX)
		{
			pos.x -= (maxX - bgMaxX) + THRESHOLD_STICKER_X;
		}

		if (maxY > bgMaxY)
		{
			pos.y -= (maxY - bgMaxY) + THRESHOLD_STICKER_Y;
		}

		// 위치 재 설정
		mStickerSp->setPosition(pos);

		// 캔버스 위치 보정값
		pos.x -= mInitRendererTexturePt.x;
		pos.y -= mInitRendererTexturePt.y;

		////////////////////////////////////////
		// 캔버스로 스티커 이동
		//mCanvas->begin();	
		mCanvasSticker->begin();	

		Sprite* stickerSp = stickerForCanvas((STICKER::ITEM)mStickerSp->getTag(), pos);
		stickerSp->setPosition(pos / mValue);		
		stickerSp->setScale(1 / mValue);
		stickerSp->visit();

		//mCanvas->end();
		mCanvasSticker->end();
		mRenderer->render();
		////////////////////////////////////////

		mStickerLayer->removeChild(mStickerSp);
		
		mIsEnabledSave = true;
		// 스티커 갯수 재설정(기획서 변경 삭제)
		//setStickerMaxCnt(STICKER_STATE::ADD, mStickerSp->getTag());		
	}
	else
	{
		CCLOG("FAILURE...");
		motionInitPosForSticker();		
	}
}


/**
* @brief  모든 그린 내용 지우기
* @return void
*/
void DrawScene::allClearCanvas()
{
	CCLOG("allClearCanvas.....");
	mCanvas->clear(0,0,0,0);
	mCanvasSticker->clear(0, 0, 0, 0);
}


/**
* @brief  모든 그린 내용 지우기
* @return void
*/
void DrawScene::allClearSticker()
{
	CCLOG("allClearSticker.....");
	mStickerLayer->removeAllChildren();
}

/**
* @brief  저장하기
* @return void
*/
//void DrawScene::saveCapture()
//{
//	CCLOG("saveCapture.....");
//	//utils::captureScreen(CC_CALLBACK_2(DrawScene::afterCaptured, this), "CaptureScreenTest.png");
//	
//}

//------------------------------------------------------------
// Private Method
//------------------------------------------------------------
/**
* @brief  그리기 모드 설정
* @param  mode  설정할 모드값
* @return void
*/
void DrawScene::setDrawMode(DRAW_MODE::MODE mode)
{
	mDrawMode = mode;
}


////////////////////////////////
// 칼라 관리자 리스너
void DrawScene::onToolShow(bool pls)
{

}


void DrawScene::onToolShowFinished()
{

}


void DrawScene::onColor(TOOL::COLOR pColor)
{

	CCLOG("[DRAW] onColor......");
	mSelectColor = pColor;
	setDrawMode(DRAW_MODE::DRAW);
	
}

//void DrawScene::onSticker()
//{
//	CCLOG("[DRAW] onSticker......");
//	setDrawMode(DRAW_MODE::STICKER_SET);
//}


void DrawScene::onUndo()
{
	setDrawMode(DRAW_MODE::UNDO);
}


void DrawScene::onDone()
{
	setDrawMode(DRAW_MODE::DONE);
	mIsStickerTouch = false;
}


/**
* @brief 내용 지우기
* @return void
*/
void DrawScene::onClear()
{
	CCLOG("[DRAW] onClear......");
	setDrawMode(DRAW_MODE::CLEAR);
}


/**
* @brief 모든 내용 지우기
* @return void
*/
void DrawScene::onAllClear()
{
	CCLOG("[DRAW] onAllClear......");
	setEnableTouchBtn(false);

	setDrawMode(DRAW_MODE::ALL_CLEAR);

	// popup
	mPopup = PopupLayer::getIns(this, DRAW_ERASE_ALL);
	mPopup->showHide(true, false);
}


/**
* @brief 모든 내용 지우기(팝업창에서 호출됨)
* @return void
*/
void DrawScene::clearAllDraw()
{
	// 그린내용 지우기
	allClearCanvas();
	// 스티커 지우기
	allClearSticker();

	setEnableTouchBtn(true);
	mIsStickerTouch = false;
}


////////////////////////////////
// 스티커 관리자 리스너
/**
* @brief json 파일로 부터 스티커 설정(기획 변경으로 미사용)
*
*/
void DrawScene::initStickerForJsonFile(STICKER::ITEM type, Vec2 pos)
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
	sp->setPosition(pos);
	sp->setTag(type);

	//mStickerList.push_back(sp);
	mStickerLayer->addChild(sp);
	mStickerLayer->setTouchEnabled(false);
}

/**
* @brief 선택한 스티커 설정
* @param  pSender  호출자
* @return void
*/
void DrawScene::onSelectSticker(Ref* pSender)
{
	CCLOG("[DrawScene] onSelectSticker...: %d", mIsStickerTouch);
	if (mIsStickerTouch)
	{
		return;
	}
	mIsStickerTouch = true;

	mStickerItem = (ui::Button*) pSender;
	if (mStickerSp != nullptr)
	{
		CC_SAFE_RELEASE(mStickerSp);
		mStickerSp = nullptr;
	}

	// 스티커 설정 모드로 변경
	DrawScene::setDrawMode(DRAW_MODE::MODE::STICKER_SET);

	switch ((STICKER::ITEM) mStickerItem->getTag())
	{
	case STICKER::BALL:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_BALL_P);
		break;

	case STICKER::BALLOON:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_BALLOON_P);
		break;

	case STICKER::BEAR:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_BEAR_P);
		break;

	case STICKER::BUS:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_BUS_P);
		break;

	case STICKER::CANDY:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_CANDY_P);
		break;

	case STICKER::CLOUD:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_CLOUD_P);
		break;

	case STICKER::FLOWER:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_FLOWER_P);
		break;

	case STICKER::GRASS:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_GRASS_P);
		break;

	case STICKER::HEART:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_HEART_P);
		break;

	case STICKER::LEAF:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_LEAF_P);
		break;

	case STICKER::PLANE:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_PLANE_P);
		break;

	case STICKER::RIBBON:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_RIBBON_P);
		break;

	case STICKER::STAR:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_STAR_P);
		break;

	case STICKER::TREE:
		mStickerSp = Sprite::create(FILENAME_DRAW_STICKER_TREE_P);
		break;
	}

	Vec2 pos = mStickerItem->getWorldPosition();
	pos.y += Pos::getStickerSelectPt();

	mStickerSp->retain();
	mStickerSp->setBlendFunc(BlendFunc::ALPHA_PREMULTIPLIED);	
	mStickerSp->setPosition(pos);
	mStickerSp->setTag(mStickerItem->getTag());
	
	// 최초 위치 저장
	mStickerOrginalPos = mStickerItem->getWorldPosition();	
	mStickerLayer->addChild(mStickerSp);
	mStickerLayer->setTouchEnabled(false);
}


/**
* @brief 스티커 설정
* @param type
* @param pos
* @return Sprite
*/
Sprite* DrawScene::stickerForCanvas(STICKER::ITEM type, Vec2 pos)
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
	sp->setBlendFunc(BlendFunc::ALPHA_PREMULTIPLIED);//ALPHA_PREMULTIPLIED);
	sp->setPosition(pos);	
	sp->setTag(type);

	return sp;
}


/**
* @brief  스티커 완료
* @return void
*/
//void DrawScene::onCloseSticker()
//{
//	CCLOG("[DRAW] onCloseSticker...[%d]", mStickerMgr->mStickerId);	
//}

//------------------------------------------------------------
//  저장 / 캡쳐 관련 메소드
//------------------------------------------------------------
/**
* @brief  저장하기(팝업창에서 예를 선택한 경우 저장)
* @return void
*/
void DrawScene::saveFileDraw()
{
	//CCLOG("saveFileDraw.................................");	
	// 로딩바 처리 필요!!!!
	const float delaytime = 0.5f;	
	Director::getInstance()->getScheduler()->performFunctionInCocosThread(CC_CALLBACK_0(DrawScene::saveToFileBefore, this));		
	Director::getInstance()->getScheduler()->performFunctionInCocosThread(CC_CALLBACK_0(DrawScene::saveToFileCanvasRunAction, this));

	mIsStickerTouch = false;		
	//CCLOG("saveFileDraw END..............................");
}


/**
* @brief  저장하기
* @return void
*/
void DrawScene::saveToFileCanvasAction(Node *pSender)
{
	Director::getInstance()->getScheduler()->performFunctionInCocosThread(CC_CALLBACK_0(DrawScene::saveToFileCanvasRunAction, this));
	
}


/**
* @brief  저장하기
* @return void
*/
void DrawScene::saveToFileCanvasRunAction()
{
	const float delaytime = 0.01f;
	auto saveSeq = Sequence::create(DelayTime::create(delaytime),
		CallFuncN::create(CC_CALLBACK_1(DrawScene::saveToFileCanvas, this)),
		DelayTime::create(0.01f),
		CallFuncN::create(CC_CALLBACK_1(DrawScene::saveToFileCanvasSticker, this)),
		DelayTime::create(0.05f),
		//CallFuncN::create(CC_CALLBACK_1(DrawScene::saveToFileAfter, this)),
		nullptr);
	this->runAction(saveSeq);
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::saveToFileCanvas(Node *pSender)
{
	CCLOG("saveToFileCanvas");
	// save to png file	
	// kCCImageFormatJPEG, kCCImageFormatPNG	
	mCanvas->saveToFile(StringUtils::format(FILENAME_DRAW_CUBE_IMG_X, mWeekData, mOrderNum), 
		Image::Format::PNG, true, CC_CALLBACK_2(DrawScene::afterSaved, this));
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::saveToFileCanvasSticker(Node *pSender)
{
	CCLOG("saveToFileCanvasSticker");
	// 스티커 이미지 저장
	mCanvasSticker->saveToFile(StringUtils::format(FILENAME_DRAW_CUBE_IMG_STICKER_X, mWeekData, mOrderNum),
		Image::Format::PNG, true, CC_CALLBACK_2(DrawScene::afterSavedForSticker, this));
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::saveToFileBefore()//(Node *pSender)
{
	CCLOG("saveToFileBefore");

	// 플레그 초기화
	mDrawMode = DRAW_MODE::NOT_USED;
	setEnableTouchBtn(false);

	if (mPopup != nullptr)
	{
		//mPopup->showHide(false);
		this->removeChild(mPopup);
	}

	// loading
	//mLoadingLayer = LoadingLayer::createLayer(this, mWeekData, mOrderNum);	
	mLoadingLayer->show(true);
	mLoadingLayer->setVisible(true);
	//this->addChild(mLoadingLayer, 100);	

	// JNI CALL	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "showSaveUi", "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::saveToFileAfter(Node *pSender)
{
	CCLOG("saveToFileAfter");
	// JNI CALL	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "chagneSaveUi", "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

	this->runAction(Sequence::create(DelayTime::create(1.0f), 
		CallFuncN::create(CC_CALLBACK_1(DrawScene::saveToFileAfterForDismiss, this)),
		DelayTime::create(0.05f),
		CallFuncN::create(CC_CALLBACK_1(DrawScene::saveToFileAfterForDismissDelay, this)),
		nullptr));
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::saveToFileAfterForDismiss(Node *pSender)
{
	CCLOG("saveToFileAfterForDismiss");
	// JNI CALL	
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	JniMethodInfo t;
	if (JniHelper::getStaticMethodInfo(t, "org/cocos2dx/cpp/AppActivity", "dismissSaveUi", "()V"))
	{
		t.env->CallStaticVoidMethod(t.classID, t.methodID);
		t.env->DeleteLocalRef(t.classID);
	}
#endif //(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

}


void DrawScene::saveToFileAfterForDismissDelay(Node *pSender)
{
	if (mLoadingLayer)
	{
		mLoadingLayer->show(false);
		mLoadingLayer->setVisible(false);
	}
	//this->removeChild(mPreloadGaf);

	setEnableTouchBtn(true);
	// 저장 버튼 활성화 
	mStickerMgr->mBtns[0]->setEnabled(true);
}


/**
* @brief  저장하기(리스너에서 호출)
* @return void
*/
void DrawScene::onSave()
{
	CCLOG("[DRAW] onSave...");
	setEnableTouchBtn(false);

	setDrawMode(DRAW_MODE::SAVE);

	// popup -> 삭제 , 바로 저장
	/*mPopup = PopupLayer::getIns(this, DRAW_SAVE);
	mPopup->showHide(true, false);	*/	
	saveFileDraw();
}

/**
* @brief  저장하기 callback
*         - saveToFile 의 callback 
*/
void DrawScene::afterSaved(RenderTexture* rt, const std::string& path)
{
	CCLOG("SAVE SUCCESS .....[%s]", path.c_str());
	// Setting File name	
	//saveToFileAfter(this);
}


/**
* @brief  저장하기 callback
*         - saveToFile 의 callback
*/
void DrawScene::afterSavedForSticker(RenderTexture* rt, const std::string& path)
{
	CCLOG("SAVE SUCCESS FOR STICKER .....[%s]", path.c_str());
	
	//setEnableTouchBtn(true);
	saveToFileAfter(this);
}

/**
* @brief  저장하기
   -  Call
   -  utils::captureScreen(CC_CALLBACK_2(CaptureScreenTest::afterCaptured, this),"CaptureScreenTest.png");
* @param  succeed     성공 유무
* @param  outputFile  저장할 파일 명
* @return void
* 
*/
void DrawScene::afterCaptured(bool succeed, const std::string &outputFile)
{
	if (succeed)
	{
		
	}
	else
	{
		CCLOG("Capture screen failed....");
	}
}

/**
* @brief   그리기 닫기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
//void DrawScene::menuCloseCallback(Ref* pSender)
//{
//	CCLOG("[DmenuCloseCallback......");	
//}

/**
* @brief   그리기 닫기 호출후 레이어 종료 처리
* @return  void
*/
void DrawScene::onExit()
{
	CCLOG("[DrawScene] onExit......");
	// 상위 클래스 호출
	Layer::onExit();

	//CC_SAFE_RELEASE(mEraser);	
	CC_SAFE_RELEASE(mCanvas);
	//CC_SAFE_RELEASE(mCanvasS);
	CC_SAFE_RELEASE(mSpForDelete);

	// 이벤트 정리		
	Director::getInstance()->getEventDispatcher()->removeEventListener(mListener);
	this->removeAllChildrenWithCleanup(true);
}

//------------------------------------------------------------
//  스케줄(롱탭) 관련 메소드
//------------------------------------------------------------
/**
* @brief  LongTap으로 스티커 선택시(스크롤 뷰일 경우)
* @param  dt 
*/
//void DrawScene::scheduleUpdateSticker(float dt)
//{	
//	if (mTouchStart.distance(mTouch->getLocation()) < LONG_TAP_THESHOLD)
//	{
//		// Select....		
//		if (mStickerMgr->mStickerId != INVALID_INDEX &&
//			mStickerMgr->mStickerId >= 0 && mStickerMgr->mStickerId < STICKER::MAX_STICKER)
//		{
//			this->onSelectSticker(mScrollView->getChildren().at(mStickerMgr->mStickerId));
//		}			
//	}
//}

/**
* @brief LongTap으로 스티커 선택시(삭제를 위한)
* @param  dt
*/
/* 기획서 변경으로 삭제(5.18)
void DrawScene::scheduleUpdateStickerForDelete(float dt)
{
	if (mTouchStart.distance(mTouch->getLocation()) > LONG_TAP_THESHOLD)
	{
		return;
	}

	if (mIdxStickerForDelete != INVALID_INDEX)
	{		
		Sprite* sp = dynamic_cast<Sprite*>(mStickerLayer->getChildren().at(mIdxStickerForDelete));
		
		CC_ASSERT(sp != nullptr);

		mSpForDelete = sp;
		mSpForDelete->retain();		
		CCLOG("set local zorder... %d", ZORDER_DRAW_STICKER);
		sp->setLocalZOrder(ZORDER_DRAW_STICKER);

		auto pt = sp->getPosition();
		auto size = sp->boundingBox().size;
		// 닫기 버튼
		auto popCloseItem = MenuItemImage::create(
			FILENAME_DRAW_STICKER_CLOSE_BTN, FILENAME_DRAW_STICKER_CLOSE_BTN,
			CC_CALLBACK_1(DrawScene::menuStickerPopupCloseCallback, this));		
		popCloseItem->setPosition(Vec2(size.width, size.height));

		// create menu, it's an autorelease object
		auto popCloseMenu = Menu::create(popCloseItem, NULL);
		popCloseMenu->setPosition(Vec2::ZERO);	
		sp->addChild(popCloseMenu, DEPTH_LAYER_DRAW_STICKER_CLOSE_BTN, TAG_BTN_STICKER_CLOSE);
	}
}
*/


/**
* @brief   스티커 닫기 버튼 이벤트
* @param   pSender  호출자
* @return  void
*/
/* (기획서 변경으로 삭제 5.18)
void DrawScene::menuStickerPopupCloseCallback(Ref* pSender)
{
	//CCLOG("[menuStickerPopupCloseCallback......[%d]", mIdxStickerForDelete);
	if (mIdxStickerForDelete != INVALID_INDEX)
	{
		auto sp = mStickerLayer->getChildren().at(mIdxStickerForDelete);
		int idx = sp->getTag();
		mStickerLayer->removeChild(sp);			

		// 삭제된 스티커의 갯수 수정
		setStickerMaxCnt(STICKER_STATE::DELETE, idx); 
		
	}
}
*/
/**
* @brief   스티커 갯수 처리
* @param   mode   0 : 스티커 삭제, 1: 스티커 추가
* @param   idx    인덱스
* @return  void
*/
void DrawScene::setStickerMaxCnt(STICKER_STATE::STATE mode, int idx)
{
	CCLOG("mode: %d, count[%d] : %d", mode, idx, mStickerMaxCnt[idx]);
	if (mode == STICKER_STATE::DELETE)
	{
		// 스티커 삭제		
		mStickerMaxCnt[idx]++;
		if (mStickerMaxCnt[idx] >= SIZE_MAX_STICKER)
		{
			mStickerMaxCnt[idx] = SIZE_MAX_STICKER;					
		}

		if (mStickerMaxCnt[idx] > 0)
		{
			// 스티커 활성화
			auto btn = dynamic_cast<ui::Button*>(mScrollView->getChildren().at(idx));
			btn->setEnabled(true);
		}
		
	}
	else if (mode == STICKER_STATE::ADD)
	{
		// 스티커 추가
		mStickerMaxCnt[idx]--;
		if (mStickerMaxCnt[idx] <= 0)
		{
			mStickerMaxCnt[idx] = 0;
			// 스티커 비활성화			
			/*auto btn = dynamic_cast<ui::Button*>(mScrollView->getChildren().at(idx));
			btn->setEnabled(false);*/
		}
	}
}


//------------------------------------------------------------
// 애니매이션
//------------------------------------------------------------
// 스티커 최초 위치로 이동 애니매이션
void DrawScene::motionInitPosForSticker()
{
	// 스티커 최초 위치로 이동 중인지 확인 // false 일때 스티커 선택및 이동 가능
	mStickerMgr->mIsMoveInitPositionSticker = true;
	//mIsMoveInitPositionSticker = true;
	// 최초 위치 이동 애니매이션
	mStickerSp->runAction(Sequence::create(
		MoveTo::create(TIME_MOTION_STICKER_INIT, mStickerOrginalPos),
		CallFuncN::create(CC_CALLBACK_1(DrawScene::motionInitPosForStickerActionDone, this)),		
		nullptr));
}

// 스티커 최초 위치로 이동 애니매이션 완료
void DrawScene::motionInitPosForStickerActionDone(Node *pSender)
{
	CCLOG("STICKER. COMPLETE....");
	mStickerLayer->removeChild(mStickerSp);
	mStickerSp = nullptr;
	mStickerMgr->mIsMoveInitPositionSticker = false;
}

// 팝업 메뉴 닫기 (f : true 나레이션 재생)
void DrawScene::menuPopupClose(bool f)
{
	setEnableTouchBtn(true);
	// 나레이션 송출
	// narration
	if (f)
	{
		//효과음
		if (MBJson::getInstance()->getIsOpenningDraw() == false)
		{
			cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_DRAW_OPENNING);
			MBJson::getInstance()->setIsOpenningDraw(true);
		}		
	}	
}


//------------------------------------------------------------
// 터치 이벤트(스크롤뷰)
//------------------------------------------------------------
/**
* @brief  터치 초기화
* @return void
*/
void DrawScene::initTouchEventForScrollView()
{
	CCLOG("[DRAWSCENE] initTouchEventForScrollView........");
	// touch event
	/*auto listener = EventListenerTouchAllAtOnce::create();
	listener->onTouchesBegan = CC_CALLBACK_2(DrawScene::onTouchesBeganForScrollView, this);
	listener->onTouchesMoved = CC_CALLBACK_2(DrawScene::onTouchesMovedForScrollView, this);
	listener->onTouchesEnded = CC_CALLBACK_2(DrawScene::onTouchesEndedForScrollView, this);

	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, mScrollView);
	*/
	// 삼키지 말것
	//mScrollView->setSwallowTouches(false);

}

/*
void DrawScene::onTouchesBeganForScrollView(const std::vector<Touch*>& touches, Event* event)
{
CCLOG("[SCROLLVIEW] BEGAN.....");
if (mDrawMode == DRAW_MODE::DRAW)
{
return;
}

}


void DrawScene::onTouchesMovedForScrollView(const std::vector<Touch*>& touches, Event* event)
{
if (mDrawMode == DRAW_MODE::DRAW)
{
return;
}
CCLOG("[SCROLLVIEW] MOVED.....");
}


void DrawScene::onTouchesEndedForScrollView(const std::vector<Touch*>& touches, Event* event)
{
if (mDrawMode == DRAW_MODE::DRAW)
{
return;
}
CCLOG("[SCROLLVIEW] ENDED.....");
}
*/

