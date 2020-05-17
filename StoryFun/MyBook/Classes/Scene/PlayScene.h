#ifndef __PLAY_SCENE_H__
#define __PLAY_SCENE_H__

#pragma once
#include "cocos2d.h"
#include "Layer/PopupLayer.h"

USING_NS_CC;

class PlayScene : public Layer
{
private:
	/*std::vector<std::string> myImages = {
		"image001.png",
		"image002.png",
		"image003.png"
	};*/
	bool DEBUG = false; // 표지만 재생(디버그용)
	int preloadingIndex;
	float delayTimeSndEffect;

	int currImageIndex;
	int aniEffectIndex;
	
	int mWeekData = 2;
	bool mUsedWeekData = false;
	bool mIsExistProfilePhoto = false;
	Node* mParent;
	ClippingNode* clip;
	Sprite* mSpChar = nullptr;
	Sprite* mSpStarLeftUp;
	Sprite* mSpStarLeftDn;
	Sprite* mSpStarRight;
	Sprite* mSpCharEffect;
	Sprite* mSpShine;
	// 프로필 사진 스프라이트
	Sprite* mSpProfile;

	// 권별 타이틀 나레이션 재생
	void playNarrationTitle(Node* pSender);
	// 음원 나레이션 및 녹음 음원 재생
	std::string mFileNameNarration;
	void playNarrationDelay(std::string filename, float delay);
	void playNarration(Node* pSender);

	void initLayout();	
	void createUi();
	void createNextBtn(Node* pSender);

	//void loadDataCover();
	void loadData();
	void loadDataForEndingPage();
	void loadChar();
	// 테스트용
	void loadCharModify();
	// 책커버 수정
	void loadDataCoverModify();
	//void playAni();
	// 그림 설정
	void setDrawLayer();
	// 프리로딩 체크
	void checkPreloadingDelay();

	// JNI CALL
	void startPlay();
	void stopPlay();

	void finishPlayRecordCallback();

	// 애니메이션
	void motionSpine(Node* pSender);
	void motionSpineFirst(Node* pSender);
	void motionSpineSecond(Node* pSender);
	void motionCharJump(Node* pSender);
	
	void motionChar(Node* pSender);
	void motionStar(Node* pSender);
	void motionCharEffect(Node* pSender);
	void motionStarEffect(Node* pSender);	
	void nextScene(Node* pSender);
	void nextSceneSndEffect(Node* pSender);

	// 메인씬으로 이동
	void toScene(float dt);//(Node* pSender);

	// a selector callback
	//void menuNextSceneCallback(Ref* pSender);
	void onBackBtnClick(Ref* sender);

	MenuItemImage* mBtnBack;
	Menu* mMenuBack;

	Vec2 pointStartPos, pointEndPos;

public:	
	PopupLayer* mPopup;	
	Layer* mLayer;

	static Layer* createLayer();
	static Layer* createLayer(Node* parent, int weekData);
	static Scene* createScene();
	static Scene* createScene(Node* parent, int weekData);

	// 엔딩 팝업
	void replayPlayScene(); // 재생하기 replay
	void toEndingScene();   // Ending Scene으로 이동

	Scene* scene(int index);
	void nextPage();
	void playAni();

	virtual bool init();
	virtual void onExit();
	//virtual void onEnter();

	void menuPopupClose();
	void closePlay();	
	void resumePlayEndingPage();

	// 메인 씬 버튼 비활성화
	void setBtnMainScene(bool b);

	// implement the "static create()" method manually
	CREATE_FUNC(PlayScene);
};

#endif // __PLAY_SCENE_H__

