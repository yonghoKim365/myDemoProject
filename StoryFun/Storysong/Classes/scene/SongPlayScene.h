#ifndef _SONG_PLAY_SCENE_H_
#define _SONG_PLAY_SCENE_H_

#include "scene/SongScene.h"
#include "layer/TouchLayerPlay.h"
#include "layer/TouchLayerPlayTypeB.h"
#include "layer/TouchLayerPlayTypeC.h"
#include "layer/ContentsLoader.h"

class SongPlayScene : public SongScene
{
public:
	static Scene * createScene();
	CREATE_FUNC(SongPlayScene);

	virtual bool init();
	void startScene();
	void makeAnimationLayer();
protected:
	void initGUI();
	void initData();

	void onFInishedGuideAnimation() override;

	void onSelectPlay() override;
	void onSelectListen() override;
	void onSelectQuit() override;
	void onSelectClose() override;

	void onFinishedNarration() override;
	void onFinishedEvent() override;
	void onSkipScene() override;
	void startSong() override;

private:
	void showHelpPopup();


private:
	//TouchLayerPlay * touchLayer;
	//TouchLayerPlayTypeB * touchLayer;
	AbsTouchLayer * touchLayer;
	ContentsLoader * contentsLoader;
};

#endif