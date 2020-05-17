#ifndef __DIALOG_H__
#define __DIALOG_H__

#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "cocostudio/CocoStudio.h"
#include "SimpleAudioEngine.h"
#include "GAF/Library/Sources/GAF.h"


USING_NS_CC;
USING_NS_GAF;

using namespace ui;
using namespace cocostudio;
using namespace CocosDenshion;

#define DIALOG_CREATE_FUNC(someDialog) \
public: \
	static KGDialog* createDialog() \
{ \
	someDialog* dialog = new someDialog(); \
if (dialog && dialog->initWithColor(Color4B(0, 0, 0, 100))) \
{ \
	dialog->autorelease(); \
} \
	else \
{ \
	CC_SAFE_DELETE(dialog); \
} \
	return dialog; \
}

class DialogSelectListener
{
public:
	virtual void onSelectPlay() = 0;
	virtual void onSelectListen() = 0;
	virtual void onSelectQuit() = 0;
	virtual void onSelectClose() = 0;
};

namespace DIALOG_SELECT_MODE
{
	enum selectMode
	{
		SELECT_PLAY = 0,
		SELECT_LISTEN,
		SELECT_QUIT,
		SELECT_CLOSE
	};
}

class KGDialog : public LayerColor
{
public:
	std::string _chantUrl;

	DIALOG_CREATE_FUNC(KGDialog);

	virtual bool initWithColor(const Color4B& color);
	virtual void onEnter();
	virtual void onExit();
	virtual bool onTouchBegan(cocos2d::Touch* touch, cocos2d::Event* event);
	void setDialogSelectListener(DialogSelectListener * listener);

protected:
	GAFObject * Affordance;
	CheckBox * cb;
	Size _visibleSize;
	EventListenerTouchOneByOne* _touchListener;

	DialogSelectListener * _eventListener;

	void onSelectMode(DIALOG_SELECT_MODE::selectMode type);
};

#endif // __DIALOG_H__
