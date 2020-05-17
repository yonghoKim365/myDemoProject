#ifndef __MODULE_SCENE_H__
#define __MODULE_SCENE_H__

#include "cocos2d.h"
#include "Page.h"

#define COCOS2D_DEBUG 1
using namespace cocos2d;

#include "KXMLReader.h"

class AppOperator : public Ref
{
public:
	static					AppOperator*	create();
	static					AppOperator*	getInstance();
	virtual	bool			init();
	bool					setPage(int, bool);
	bool					setEndPage();

private:
	EventListenerTouchOneByOne*	mListener;
	int					mCurrentPage;
	int					mLength;
	HPAGE_INFO **		mppArrData;
private:
	void addEventListener(const std::string&, const std::function<void(EventCustom*)>&);
	virtual void onSetPage(EventCustom* event);
	virtual void onPrevPage();
	virtual void onNextPage();
	virtual void onPrevPageNopaging();
	virtual void onNextPageNopaging();
public:
	AppOperator();
	~AppOperator();
private:
	KXMLReader * mpReader;

public:
	void requestInitProcess();
};

#endif // __MODULE_SCENE_H__
