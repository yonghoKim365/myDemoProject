#ifndef _SONG_INTRO_
#define _SONG_INTRO_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "common/dialogs/SelectDialog.h"
#include "data/JsonInfo.h"
#include "layer/UILayer.h"
#include "ui/CocosGUI.h"
#include "layer/ContentsLoader.h"

USING_NS_CC;
USING_NS_GAF;
using namespace ui;

class SongIntro : public Layer, public DialogSelectListener, public SkipButtonEventListener
{
public:
	static Scene * createScene();
	virtual bool init();

	void initEvent();
	void touchEvent(cocos2d::Touch* touch, Vec2 _p);

	virtual void onSelectPlay();
	virtual void onSelectListen();
	virtual void onSelectQuit();
	virtual void onSelectClose();
	virtual void onSkipScene();

	CREATE_FUNC(SongIntro);

private:
	void onHitEvent(void* params);
	void onFinishTouchSequence(gaf::GAFObject* object);
	void parseJson();

	void onLyricsImageLoad(Texture2D * texture);
	void onSpriteSheetLoad(Texture2D * texture);
	void onTouchLoad(Texture2D * texture);

	void memTest();
	bool isAnimClick(GAFObject * gaf, cocos2d::Touch* touch);
	void onFinishedTouchSequence(gaf::GAFObject* object, cocos2d::Touch* touch);
	void onFinishCallBack(int audioID, std::string filePath);
	void moveNextScene(float delta);
	void moveNextScene();

	void delayScene(Node* pSender);
private:
	int narrationID = -1;
	
	GAFObject * container;
	GAFObject * example;
	GAFObject * touchObj;
	GAFObject * imageHolder;

	UILayer * uiLayer;

	EventDispatcher * _eventDispatcher;
	EventListenerTouchOneByOne * listener;

	LoadingDialog * loadingDlg;
	ContentsLoader * contentsLoader;

	rapidjson::Document	jsonDom;

	JsonInfo * jsonInfo;

	std::string machinePath;

	//std::vector<std::string>	touchPathList;
	//std::vector<std::string>	trackLyricsList;
	//std::vector<std::string>	trackExampleList;
	//std::vector<std::string>	wordExampleList;
	//std::vector<GAFObject *>	touchObjList;
	int maxLoadCount;

	int finishedCount = 0;
	bool isChanged = false;
};

#endif