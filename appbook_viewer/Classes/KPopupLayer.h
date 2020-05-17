#ifndef __KPOPUP_LAYER_H__
#define __KPOPUP_LAYER_H__

#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "ViewUtils.h"
using namespace cocos2d;

typedef std::function<void(cocos2d::Ref*)> KPopupLayerCallback;

class KPopupLayer : public cocos2d::LayerColor
{
public:
	static KPopupLayer* createLayer(cocos2d::Color4B color4B = cocos2d::Color4B(255,255,255,100));
    bool init(cocos2d::Color4B color4B);
	
	void setYesTypeCallback(std::string sTitle, std::string sContent, std::string sYes, const KPopupLayerCallback & callbackYes);
	// jump to page에서 학습미완료시 페이지 점프 안된다는 가이드 팝업
	void setJtpWarningCallback(const KPopupLayerCallback & callbackYes);
	// ibook, ebook 에서 최초 한번 guide 팝업
	void setEBookIBookGuidePopupCallback(std::string userdefaultVal, bool bEbook, const KPopupLayerCallback & callbackChkbox);
	// ebook 에서 최초 한번 guide 팝업
	//void setEBookGuidePopupCallback(std::string userdefaultVal,const KPopupLayerCallback & callbackYes);
	void setYesNoTypeCallback(std::string sTitle, std::string sContent
								, std::string sYes
								, std::string sNo
								, const KPopupLayerCallback & callbackYes 
								, const KPopupLayerCallback & callbackNo);

	// 이어하기 yes/no 팝업
	void continueYesNoTypeCallback(const KPopupLayerCallback & callbackYes, const KPopupLayerCallback & callbackNo);
	void volumeMuteWarningPopupCallback(const KPopupLayerCallback & callbackOk);

	void removePopupFromParent(Node* pNode);

	KPopupLayerCallback mCallChkBox;
	bool				bChkBoxChecked;
	std::string			_projectCode;
	std::string			_userdefaultVal;
	KPopupLayerCallback mCallbackYes;
	KPopupLayerCallback mCallbackNo;

	EventListenerTouchOneByOne * pEventListenerBack;

	EventListenerTouchOneByOne * pEventListenerYes;
	EventListenerTouchOneByOne * pEventListenerNo;
private:
	void makeCommonUI(std::string sTitle, std::string sContent, std::string sTitlePng);
	void makeCheckBoxUI();

	ui::Button* mBtnYes;
	ui::Button* mBtnNo;
	
};

#endif // __KPOPUP_LAYER_H__
