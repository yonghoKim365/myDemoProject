#ifndef __POPUP_LAYER_H__
#define __POPUP_LAYER_H__

#pragma once
#include "cocos2d.h"
#include "ui/CocosGUI.h"

USING_NS_CC;

enum POPUP_TYPE
{	
	HELP_MAIN = 0,
	HELP_RECORD,
	HELP_DRAW,
	HELP_CAMERA,	
	OPTION_POP,
	DRAW_CLOSE,
	DRAW_SAVE,
	DRAW_ERASE_ALL,
	RECORD_CLOSE,
	RECORD_DELETE,
	PLAY_CLOSE,  // 재생하기 - 닫기 버튼 팝업
	PLAY_ENDING, // 재생하기 - 끝 팝업
	PRELOADING,
	MAIN_EXIT
};

// Usage
////Popup* p = Popup::getIns(this, GAME_SUCCESS, mGameLevel.getLevel(), mGameLevel.getStage());		
// mPopup = Popup::getIns(this, GAME_SUCCESS, mGameLevel.getLevel(), mGameLevel.getStage());
// ((Popup*)mPopup)->showHide(true, false);

class PopupLayer : public Layer
{
private:
	Node*  mParent;
	POPUP_TYPE mType;
	bool  mVisible;
	// btn
	MenuItemImage* mBtnClose;
	Menu* mBtnCloseMenu;

	// container
	Sprite* mPopupSp = nullptr;
	//Sprite* mPopupSpPreloding = nullptr;
	Vec2 mCenter;

	MenuItemToggle* toggle;

	//----------------------------------------------------------------------
	//	ui
	//----------------------------------------------------------------------
	void createUI();
	void createHelpMain();
	void createHelpRecord();
	void createHelpDraw();
	//void createHelpCamera();
	void createOption();
	void createCloseDraw();
	void createSaveDraw();
	void createEraseAllDraw();
	void createCloseRecord();  // 녹음 닫기 버튼
	void createDeleteRecord(); // 녹음 삭제 버튼
	void createClosePlay();    // 재생하기 
	void createEndingPlay();   // 재생하기 끝 팝업
	void createExitMain();   // 메인씬 종료
	void createPreloading(); // 로딩..

	void onTouchText(Ref* pSender, ui::Widget::TouchEventType type);

	////////////////////////////////////////
	// 버튼 콜백
	// 팝업창 닫기(X) 버튼
	void onPopupCloseBtnClick(Ref* sender);
	// Draw
	void onDrawCloseBtnClick(Ref* sender);
	void onDrawResumeBtnClick(Ref* sender);
	// 녹음하기
	void onRecordCloseBtnClick(Ref* sender);
	void onRecordResumeBtnClick(Ref* sender);
	// 저장 
	void onDrawSaveBtnClick(Ref* sender);
	// 모두 지우기
	void onDrawAllEraseBtnClick(Ref* sender);
	void helpDrawCallback(Ref* sender);


	// 녹음 하기 삭제 버튼
	void onRecordDeleteBtnClick(Ref* sender);
	void onRecordDeleteResumeBtnClick(Ref* sender);


	// 재생하기
	void onPlayCloseBtnClick(Ref* sender);
	void onPlayResumeBtnClick(Ref* sender);
	// 재생하기 Endgin 버튼
	void onPlayEndingReplayBtnClick(Ref* sender);
	void onPlayEndingNextBtnClick(Ref* sender);

	// 메인씬 닫기 버튼
	void onMainCloseBtnClick(Ref* sender);
	void onMainResumeBtnClick(Ref* sender);

public:
	virtual bool init();
	static PopupLayer* getIns(Node* parent, POPUP_TYPE type);

	static PopupLayer* create(Node* parent, POPUP_TYPE type);
	PopupLayer();
	PopupLayer(Node* parent, POPUP_TYPE type);
	virtual ~PopupLayer(void);
	
	//----------------------------------------------------------------------
	//	public
	//----------------------------------------------------------------------
	PopupLayer* showHide(bool visible, bool immediate = false);


	//----------------------------------------------------------------------
	//	get / set
	//----------------------------------------------------------------------
	POPUP_TYPE getPopupType() const
	{
		return mType;
	}

	void setPopupType(POPUP_TYPE type)
	{
		this->mType = type;
	}
};

#endif // __POPUP_LAYER_H__

