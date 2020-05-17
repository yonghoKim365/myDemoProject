
/**
* @class	StickerManager
* @brief	스티커 관리 클래스
*
* @author	
* @date	2016. 3. 30
*/

#ifndef __STICKER_MANAGER_H__
#define __STICKER_MANAGER_H__

#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "Contents/MyBookResources.h"


USING_NS_CC;


/**
* @brief 스티커 이벤트 리스너
*/
class StickerManagerListnener 
{
public:
	// 선택된 스티커 정보
	virtual void onSelectSticker(Ref* pSender) = 0;
	// 스티커 선택창 닫힘
	//virtual void onCloseSticker() = 0;
	virtual void onSave() = 0;
};


/**
 * @brief 스티커 관리자
 */
class StickerManager 
{
private:
	////////////////////////////////////////
	// 이전 좌표	
	Vec2 mTouchStart;
	Vec2 mTouchEnd;
	////////////////////////////////////////
	// 스크롤 뷰
	ui::ScrollView* mScrollView;
	ui::Button* mBtnRight;
	ui::Button* mBtnLeft;

	/// 닫기 버튼
	ui::Button* btnClose;
	/// ui
	ui::Layout* mLayout = nullptr; // layout
	/// 스티커 버튼 
	//ui::Widget* mStickers; // pStickers
	/// 이벤트 리스너
	StickerManagerListnener* mListener;// listener;
	/// 화면에 보일때 y좌표
	int showy;
	/// 화면에 안 보일 때 y좌표
	int hidey;
	/// 현재 애니메이션 중인지 여부
	//bool isAni = false;
	/// 현재 화면에 보이는지 여부
	//bool visible = false;

	// 애니메이션 종료
	//void onAniFinished();
	// 스티커 선택
	void onTouchSticker(Ref* pSender, ui::Widget::TouchEventType type);
	// 스티커 선택창 닫기 버튼 클릭
	//void onTouchStickerClose(Ref* pSender, ui::Widget::TouchEventType type);
	// 스티커 선택창 저장 버튼 클릭
	void onTouchSave(Ref* pSender, ui::Widget::TouchEventType type);

	// 스티커 좌 스크롤
	void onTouchScrollLeft(Ref* pSender, ui::Widget::TouchEventType type);
	// 스티커 우 스크롤
	void onTouchScrollRight(Ref* pSender, ui::Widget::TouchEventType type);
	void setEnableForLeftBtn(bool b);
public:

	//------------------------------------------------------------
	// Common
	//------------------------------------------------------------
	/**
	* 생성자
	* @param Layout* pLayout 스티커 선택창 ui
	* @param StickerManagerListener* pListener 이벤트 리스너
	*/
	StickerManager(ui::Layout* pLayout, StickerManagerListnener* pListener);
	~StickerManager();
	// show or hide
	void setShow(bool b);
	// 현재 show 여부 리턴
	//bool getShow();
	// 현재 애니메이션 중인지 여부 리턴
	//bool getAni();
	// 지우기/모두지우기 버튼 배열
	std::vector<ui::Button*> mBtns;
	//std::vector<ui::Button*> mStickers;

	ui::Layout* getLayout();
	ui::ScrollView* getScrollView();
	////////////////////////////////////////
	// DrawScene과의 인터페이스
	void setSticker();
	int mStickerId;

	// 최초 위치로 이동
	bool mIsMoveInitPositionSticker = false;
};

#endif // __STICKER_MANAGER_H__