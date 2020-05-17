#ifndef __KPRELOADING_PAGE_H__
#define __KPRELOADING_PAGE_H__
#include "ui/CocosGUI.h"
USING_NS_CC;
using namespace ui;
#include "cocos2d.h"

#include "AppOperator.h"


class KPreloadingPage : public cocos2d::Layer
{
public:
	static cocos2d::Scene* createScene(AppOperator * pOperator);
	virtual bool init();
	void update(float) override;
	CREATE_FUNC(KPreloadingPage);
private:
	ui::LoadingBar* mLoadingBar;
	cocos2d::Label* mpLabel;
	AppOperator * mpOperator;
	int m_nCurrentCount;
	int m_nTotalCount;
	Sprite* msprite;
	Vector<Sprite*> manimFrames;
	
public:
	void setOperator(AppOperator * pOperator);
private:
	void threadStart(float dt);

	void makeImageStringFromPageInfo(set<string> & setImageFilename);
	void makeImageStringFromPopupInfo(set<string> & setImageFilename);
	void makeSoundStringFromPageInfo(set<string> & setSoundFilename);
	void makeSoundStringFromPopupInfo(set<string> & setSoundFilename);

	void LoadingScreen(float ft);
	void ShowLoading(int npos);
	
};

#endif // __KPRELOADING_PAGE_H__