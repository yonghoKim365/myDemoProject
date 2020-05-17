#ifndef __DRAW_SCENE_H__
#define __DRAW_SCENE_H__

#pragma once
#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "Manager/ToolManager.h"
#include "Manager/StickerManager.h"
//#include "Contents/MyBookSticker.h"
#include "Layer/PopupLayer.h"
#include "Layer/LoadingLayer.h"

#include "Cocos2dxGAFPlayer-release-gaf-5/Library/Sources/GAF.h"
#include "common/AssetUtils.h"

USING_NS_CC;
USING_NS_GAF;

class DrawScene : public Layer, 
				  public ToolManagerListener, // 컬러 이벤트 리스너
				  public StickerManagerListnener // 스티커 이벤트 리스너
{
public:
	// 스티커 멀티 터치 막기
	bool mIsStickerTouch = false;

	static Scene* createScene();
	static Layer* createLayer();	
	static Layer* createLayer(Node* parent, int weekData, int ord, std::string filename);

	virtual bool init();
	virtual void onExit();

	// a selector callback
	//void menuCloseCallback(Ref* pSender);
	////////////////////////////////////////
	// 터치 이벤트 (그리기)
	virtual void onTouchesBegan(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesMoved(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesEnded(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesCancelled(const std::vector<Touch*>& touches, Event* event);
	
	/*
	virtual bool onTouchBegan(Touch* touch, Event* event);	
	virtual void onTouchMoved(Touch* touch, Event* event);
	virtual void onTouchCancelled(Touch* touch, Event* event);
	virtual void onTouchEnded(Touch* touch, Event* event);
	*/
	////////////////////////////////////////

	////////////////////////////////////////
	// 터치 이벤트 (스크롤뷰)
	ui::ScrollView* mScrollView;
	void initTouchEventForScrollView();
	/*virtual void onTouchesBeganForScrollView(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesMovedForScrollView(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesEndedForScrollView(const std::vector<Touch*>& touches, Event* event);*/
	////////////////////////////////////////

	// implement the "static create()" method manually
	CREATE_FUNC(DrawScene);	

	RenderTexture* getRenderTexture();

	// 메인 씬으로 돌아가기
	void backToMainScene();
	// 그리기 모드 설정
	void setDrawMode(DRAW_MODE::MODE mode);
	////////////////////////////////////////////////////
	// 팝업창에서 호출
	// 저장하기
	void saveFileDraw();
	// 모두지우기
	void clearAllDraw();
	// 팝업 메뉴에서 닫기 선택시
	void menuPopupClose(bool f);
	void setEnableTouchBtn(bool b);

	bool getIsEnabledSave();

private:
	void processUserTouch();

	Node* mParent;
	PopupLayer* mPopup;
	LoadingLayer* mLoadingLayer;
	// 버튼 활성화 여부
	bool mIsEnabledSave = false;
	bool mIsEnabledSaveBefore = false;
	Sprite* mSpBtnDisabledSave = nullptr;

	//GAFObject* mPreloadGaf;
		
	//PopupLayer* mPopupInit;
	Menu* mMenuBack;
	MenuItemImage* mBtnBack;

	EventListenerTouchAllAtOnce* mListener;
	//EventListenerTouchOneByOne* mListener; // 싱글 터치로 변경
	// 호 정보
	int mWeekData = 1;
	// 배열된 그림 순서
	int mOrderNum = 1;
	// 그리기용 배경 파일
	std::string mFilename;
	// 그림그리기의 퍼포먼스 이슈로 그림 그리는 영역을 현재 해상도의 절반 크기로 나누기 위한 변수
	float mValue = 1;// 2;//3;
	Renderer* mRenderer;

	// 현재 컨텐츠 크기
	Size visibleSize;
	// 현재 컨텐츠의 x, y 좌표
	Vec2 origin;
	// 스티커 최초 위치로 이동 중인지 확인 // false 일때 스티커 선택및 이동 가능
	bool mIsMoveInitPositionSticker = false;

	// 사용자가 그리거나 스티커를 붙힐 때 각 스프라이트를 구분하기 위한 구분자
	//int mHistoryCnt = 0;

	// 초기화 메소드
	void initTouchEvent();
	void initDraw();
	void initBrush();
	void createUi();
	void initLayout();
	// 캔버스 초기화
	void initCanvas();
	// 배경 그림 초기화
	void initBG();
	// 기존 데이터가 존재하는 경우 불러오기(true: 기존데이터존재, false: 미존재)
	void loadData();
	// 최초 진입시 팝업 생성
	void createPopup();

	// bgm -> main bgm으로 대체
	//void initBGM();
	// 돌아가기 버튼 이벤트
	void onBackBtnClick(Ref* pSender);

	//------------------------------------------------------------
	// 그리기 / 지우기 / 스티커 드래그 메소드 / 저장하기
	//------------------------------------------------------------
	// 지우개
	//DrawNode *mEraser = nullptr;	
	//Sprite* mEraser = nullptr;

	// 그리기 영역 설정을 위한 값
	Vec2  mInitRendererTexturePt;	

	////////////////////////////////////////
	// 칼라 그리기 메소드(drawCanvas)에서 선언된 로컬 변수 선언
	Vec2 start;
	Vec2 end;
	float distance;
	int d;
	float difx;
	float dify;
	float delta;
	float x;
	float y;
	float scale;
	////////////////////////////////////////

	// 그리기
	void drawCanvas(Touch* touch);
	// 지우기
	void eraserCanvas(Touch* touch);
	// 스티커 드래그
	void dragSticker(Touch* touch);
	// 모든 내용 지우기 
	void allClearCanvas();
	// 모든 스티커 지우기 
	void allClearSticker();
	// 저장하기
	//void saveCapture();
	// 저장하기(callback)
	void afterSaved(RenderTexture* rt, const std::string& path);
	void afterSavedForSticker(RenderTexture* rt, const std::string& path);

	// 그리기 모드
	DRAW_MODE::MODE  mDrawMode = DRAW_MODE::MODE::START;

	// Blend Function
	
	//------------------------------------------------------------
	// 칼라
	//------------------------------------------------------------
	// 터치 좌표 배열
	std::vector<Point> mTouchList; // touchs

	// 브러쉬 사이즈
	Size mBrushSize;
	// 크레파스 브러쉬 텍스쳐 
	Texture2D* mBrushTexture;	
	// 색상 컬러값
	Color3B mBrushColor;

	// 현재 선택된 칼라값
	TOOL::COLOR mSelectColor; // color
	// 사용자 컬러 배열 (파레트 색상)
	std::vector<Color3B> mColorList; // colors

	// 그림 그리기 영역
	RenderTexture* mCanvas = nullptr;
	// 스티커 그리기 영역
	RenderTexture* mCanvasSticker = nullptr;
	//std::vector<Vec2> mTouchLists;
	
	Vec2 mErasePos;
	// 그리기 툴 관리자
	ToolManager* mToolMgr = nullptr;

	//------------------------------------------------------------
	// 스티커
	//------------------------------------------------------------
	// 스티커 적용 영역 저장
	Rect mBGBoundingBoxForSticker;
	// 스티커 최초 위치 저장
	Vec2 mStickerOrginalPos;
	// 스티커별 사용 횟수
	int mStickerMaxCnt[STICKER::MAX_STICKER];
	// 스티커 Zorder
	int mStickerZorderForDelete;
	// 선택된 스티커
	Sprite* mSpForDelete = nullptr;

	// 스티커의 최소 스케일 배율
	//float minScale = 0.3f;
	// 스티커의 기본 스케일 배율
	//float initScale = 0.6f;
	// 스티커의 최대 스케일 배율
	//float maxScale = 1.0f;

	// 스티커 버튼
	ui::Button* mStickerItem; //stickerItem;
	// 사용자가 선택한 스티커를 담을 객체
	Sprite* mStickerSp; // stickerSpr;
	
	// 상위 스티커 레이어 (사용자가 스티커 추가시 가장 상단 뎁스로 올려져야 해서)
	Layer* mStickerLayer; //stickerLayer;
	// 스티커 메니저
	StickerManager* mStickerMgr;
		
	// schedule (롱탭 처리)
	Vec2  mTouchStart;
	//Vec2  mTouchEnd;	
	Touch* mTouch;
	
	// 스티커 저장 리스트
	//std::vector<Sprite*> mStickerList; 
	//std::vector<MyBookSticker*> mMBStickerList;

	// 개별 스티커 갯수 초기화
	void initSizeMaxSticker(int maxCnt);
	// 개별 스티커 갯수 수정(0 : 스티커 삭제, 1: 스티커 추가)
	void setStickerMaxCnt(STICKER_STATE::STATE mode, int idx);

	// 스크롤 뷰에서 스티커 선택을 위한 스케쥴
	//void scheduleUpdateSticker(float dt);
	// 삭제를 위해서 선택한 스티커 롱탭 처리(기획서 변경으로 삭제 5.18)
	/*
	//void scheduleUpdateStickerForDelete(float dt);
	// 삭제를 위한 롱탭으로 선택한 스티커 index
	//int mIdxStickerForDelete = INVALID_INDEX;	
	// 스티커 닫기 버튼 이벤트(기획서변경으로 삭제 5.18)
	//void menuStickerPopupCloseCallback(Ref* pSender);
	*/
	//  스티커 설정 완료
	void setSticker();
	// 스티커 설정(Json파일로 부터 읽어 들인 정보) --> 기획서 변경으로 삭제 5.18
	void initStickerForJsonFile(STICKER::ITEM type, Vec2 pos);	
	// 스티커 설정
	Sprite* stickerForCanvas(STICKER::ITEM type, Vec2 pos);

	////////////////////////////////////////
	// 스티커 애니매이션
	// 스티커 최초 위치로 이동 애니매이션	
	void motionInitPosForSticker();
	// 스티커 최초 위치로 이동 애니매이션 완료
	void motionInitPosForStickerActionDone(Node *pSender);

	//------------------------------------------------------------
	// Listener
	//------------------------------------------------------------
	
	////////////////////////////////////////
	// 칼러 리스너
	// 툴 메니저 리스너 (툴 상태)
	void onToolShow(bool pIs);
	// 툴 메니저 리스너 (애니메이션 종료)
	void onToolShowFinished();
	// 툴 메니저 리스너 (컬러 선택)
	void onColor(TOOL::COLOR pColor);

	// 툴 메니저 리스너 (undo 클릭)
	void onUndo();
	// 툴 메니저 리스너 (완료 버튼 클릭)
	void onDone();
	// 툴 메니저 리스너 (지우기)
	void onClear();
	// 툴 메니저 리스너 (모두지우기)
	void onAllClear();
	
	////////////////////////////////////////
	// 스티커 리스너

	// 툴 메니저 리스너 (스티커 선택 모드)
	//void onSticker();
	// 스티커 메니저 리스너 (스티커 선택)
	void onSelectSticker(cocos2d::Ref* pSender);
	// 스티커 메니저 리스너 (스티커 선택창 닫기);
	//void onCloseSticker();
	// 저장하기
	void onSave();
	// 저장
	void saveToFileCanvas(Node *pSender);
	void saveToFileCanvasSticker(Node *pSender);

	void saveToFileAfter(Node *pSender);
	void saveToFileAfterForDismiss(Node *pSender);
	void saveToFileAfterForDismissDelay(Node *pSender);

	void saveToFileBefore(); // (Node *pSender);

	void saveToFileCanvasAction(Node *pSender);
	void saveToFileCanvasRunAction();

	/// 사용자가 완료 선택시 화면 캡쳐 완료 후 리스너
	void afterCaptured(bool succeed, const std::string &outputFile);
};


#endif // __DRAW_SCENE_H__