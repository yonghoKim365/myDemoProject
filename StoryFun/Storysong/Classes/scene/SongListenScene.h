#ifndef _SONG_LISTEN_SCENE_H_
#define _SONG_LISTEN_SCENE_H_

#include "scene/SongScene.h"
#include "layer/TouchLayerListen.h"
#include "common/dialogs/SelectDialog.h"
#include "layer/ContentsLoader.h"

class SongListenScene : public SongScene
{
public:
	static Scene * createScene();
	CREATE_FUNC(SongListenScene);

	virtual bool init();

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
	void startSong() override;
	void onSkipScene() override;

private:
	LoadingDialog * loadingDlg;
	TouchLayerListen * touchLayer;
	void goToNextScene();
	void showHelpPopup();
private:
	ContentsLoader * contentsLoader;
};


#endif