#ifndef _SONG_SCENE_H_
#define _SONG_SCENE_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "layer/TrackInfoLayer.h"
#include "data/JsonInfo.h"
#include "layer/SongLyricsLayer.h"
#include "data/def.h"
#include "common/dialogs/SelectDialog.h"
#include "layer/GuideLayer.h"
#include "layer/UILayer.h"

USING_NS_CC;
USING_NS_GAF;

using namespace SCENE_TYPE;

class SongScene : public Layer, 
		public SongTickEventListener, public DialogSelectListener,
		public GuideFInishedEventListener, public SkipButtonEventListener
								
{
public:
	SongScene();
	~SongScene();
	static Scene * createScene();
	CREATE_FUNC(SongScene);

	virtual bool init();
	void startNarration();

protected:
	virtual void onSelectPlay();
	virtual void onSelectListen();
	virtual void onSelectQuit();
	virtual void onSelectClose();

	virtual void onFInishedGuideAnimation();

	virtual void onTickEvent(int index);
	virtual void onFinishedEvent();
	
	virtual void onSkipScene();

	void initGUI();
	void initData();
	
	virtual void startSong();
	void addObserver(IBaiscSongPlay * object);
	void setCurrentScene(SceneType scene);
	void stopNarrationCallback();
	virtual void onFinishedNarration();

protected:
	std::vector<IBaiscSongPlay *>	songPlayObserable;

	SongTickManager *	trackSecondManager;
	SongLyricsLayer *   lyricsLayer;
	JsonInfo		*	jsonInfo;
	Sprite			*	bgSprite;

	SceneType	currentScene;
	GuideLayer		*	guideLayer;
	UILayer			*	uiLayer;

	int					UIDepth_TickManager = 1;
	int					UIDepth_Background = 2;
	int					UIDepth_Animation = 3;
	int					UIDepth_Lyrics = 4;
	int					UIDepth_GUIDE = 7;
	int					UIDepth_UI = 6;
	int					UIDepth_Popup = 100;

	bool				isFirstPlay;
private:
	int					narrationID;
};


#endif