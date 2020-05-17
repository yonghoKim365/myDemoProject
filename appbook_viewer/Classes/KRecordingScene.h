#ifndef __KRecordingLayer_H__
#define __KRecordingLayer_H__

#include "cocos2d.h"
using namespace cocos2d;
#include "ui/CocosGUI.h"
#include "Page.h"

#include "KXMLReader.h"
#include "KRecordingManager.h"

#define COMMAND_RECORDEXIT						"RECORDEXIT"
#define COMMAND_RECORDPLAY						"RECORDPLAY"
#define COMMAND_RECORDING						"RECORDING"
#define COMMAND_RECFILEDEL						"RECFILEDEL"
#define COMMAND_RECORDSTOP						"RECORDSTOP"
#define COMMAND_PREV							"PREV"
#define COMMAND_NEXT							"NEXT"


class KRecordingLayer : public cocos2d::Layer, public KRecordingManagerDelegate
{
public:
    static cocos2d::Scene* createScene();

    virtual bool init();
    
	CREATE_FUNC(KRecordingLayer);

private:
	cocos2d::Vector<Layer *> mLayers;
	Vector<Layer *>::iterator mIter;
	int mCurIndex;

	int mDataLength;

	bool canGoFurther(int nTargetIndex);
	void addAndRemoveChildren();

	void constructUIForRecord();
	void appendButton(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * target);

	cocos2d::Layer * mLayerRecord;
	cocos2d::Layer * mLayerContent;

	ui::Button * mBtnPrev;
	ui::Button * mBtnNext;
	ui::Button * mBtnClose;

	Vector<Ref *>		vtRefCountManager;
	void				command(string, string, string = "", Ref * = nullptr);

	void constructUIForPages(cocos2d::Layer * layer, int nIndex);
	void appendButtonForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * layer);
	void appendAnimationForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * layer);
	void appendImageForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * layer);
	void appendTextForRecord(STCONTENT_INFO *  pContentInfo, cocos2d::Layer * layer);

	Sprite * m_pProgressSprite;

	ui::Button * m_btnPlay;
	ui::Button * m_btnRecord;
	ui::Button * m_btnRecordStop;
	ui::Button * m_btnDelete;

	Sprite * m_pBlockingSprite;
	EventListenerTouchOneByOne * pEventListerBlock;

private:
	KXMLReader * mpReader;

private:
	std::string getFullPathName();

public:
	void showNoInputPopup();
	void hideNoInputPopup();
	void refreshRecordControlUI();

public:
	void onRecordingStarted();
	void onRecordingEnded();
	void onListeningEnded();
};

#endif // __KRecordingLayer_H__
