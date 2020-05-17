
/**
* @class	ToolManager
* @brief	관리 클래스
*
* @author	
* @date	2015. 10. 5
*/

#ifndef __TOOL_MANAGER_H__
#define __TOOL_MANAGER_H__

#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "Contents/MyBookResources.h"

USING_NS_CC;


/**
* @brief 툴 이벤트 리스너
*/
class ToolManagerListener
{
public:
	// 툴이 보이는지 여부
	virtual void onToolShow(bool pIs) = 0;
	// 툴 등장 애니메이션 종료
	virtual void onToolShowFinished() = 0;
	// 툴 파레트 색상 클릭 이벤트
	virtual void onColor(TOOL::COLOR pColor) = 0;
	// 스티커 클릭
	//virtual void onSticker() = 0;
	// 완료 클릭
	virtual void onDone() = 0;

	// 지우기
	virtual void onClear() = 0;
	// 삭제(모두 삭제)
	virtual void onAllClear() = 0;

	//-----------------------------------------------
	// undo 클릭(Not Used)
	//-----------------------------------------------
	virtual void onUndo() = 0;
};


class ToolManager
{
private:
	// 화면에 보일시 y 좌표
	int mShowY;//showy;
	// 화면에서 사라졌을 때 y 좌표
	int mHideY; // hidey;
	// 선택된 크레파스 y좌표
	int mColorSelectY; //colorsy;
	// 선택 안된 크레파스 y좌표
	int mColorHideY; // colorhy;
	// 선택된 크레파스 색상
	TOOL::COLOR mSelectColor;//selectColor;

	// 현재 화면에 보이는지 여부
	bool isShow = false;
	// 완료 버튼을 제외한 부분이 화면에서 안보이는지 여부
	bool isControlHide = false;
	// 애니메이션 중인지 여부
	bool isAni = false;
	
	// ui
	cocos2d::ui::Layout* mLayout; // layout;
	// 완료 버튼을 뺀 나머지 widget
	//cocos2d::ui::Widget* control;
	// 이벤트 리스너
	ToolManagerListener* mListener; // listener;
	// 칼라 선택시 추가
	Sprite* mSpColorSelect;

	// 애니메이션 종료
	//void onAniFinished();
	// 크레파스 색상 선택
	void onColor(Ref* pSender, ui::Widget::TouchEventType type);  
	/// 스티커 버튼 클릭
	//void onSticker(Ref* pSender, ui::Widget::TouchEventType type);
	/// undo 버튼 클릭
	void onUndo(Ref* pSender, ui::Widget::TouchEventType type);
	/// 완료 버튼 클릭
	void onDone(Ref* pSender, ui::Widget::TouchEventType type);
	// 지우기
	void onClear(Ref* pSender, ui::Widget::TouchEventType type);
	// 모두 지우기
	void onAllClear(Ref* pSender, ui::Widget::TouchEventType type);

public:
	/**
	* 생성자
	* @param Layout* pLayout 툴 ui
	* @param ToolManagerListener* pListener 이벤트 리스너
	*/
	ToolManager(ui::Layout* pLayout, ToolManagerListener* pListener);
	~ToolManager();

	/// 열기 또는 닫기
	void setShow(bool b);
	/// 애니메이션 여부
	bool getAni();
	/// 색상 선택
	void setColor(TOOL::COLOR pColor);
	/// 완료 버튼을 뺀 나머지 부분 열기 또는 닫기
	void setShowControl(bool pIs);
	/// 툴 활성화 상태 제어
	void setLayoutEnabled(bool pIs);
	// 초기화(아무것도 선택되지 않음)
	void resetColorTool();

	// 크레파스 버튼 배열
	std::vector<ui::Button*> mColors;//colors;
	// 지우기/모두지우기 버튼 배열
	std::vector<ui::Button*> mBtns;
	cocos2d::ui::Layout* getLayout();
	//------------------------------------------------------------
	// Common
	//------------------------------------------------------------
};

#endif // __TOOL_MANAGER_H__