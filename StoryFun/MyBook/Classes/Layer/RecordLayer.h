#ifndef __RECORD_LAYER_H__
#define __RECORD_LAYER_H__

#pragma once
#include "cocos2d.h"
// 녹음하기
#include "Manager/VoiceRecordManager.h"
#include "Contents/MyBookResources.h"
#include "Util/Pos.h"
#include "Util/MBJson.h"
#include "Layer/PopupLayer.h"

USING_NS_CC;

//16.10.22 fixed by shh 
enum CanvasLayerOrder
{
	kDepthCanvasBG,
	kDepthCanvasDraw,
	kDepthCanvasLine,
	kDqpthCanvasSticker
};

class RecordLayer : public Layer
{
private:
	bool mIsTouchPlayBtn = false;
	// 삭제 효과음 송출 유무
	bool mIsHelp = false;
	// 도움말 버튼 활성/비활성 유무
	bool mIsVisibleHelp = false;

	Node* mParent;
	int mWeekData;
	int mOrderNum;
	// 녹음한 음원 식별자
	//int id;
	int mRecordSndId = INVALID_INDEX;
	// 나레이션 식별자
	int mNarId = INVALID_INDEX;

	// 나레이션 id
	int mSecondNarId = INVALID_INDEX;
	//int mPlayEffectRecordSnd = -1;
	//int mPlayNarRecordSnd = -1;
	// 그리기 배경 파일 네임
	std::string mFileName;
	// 원어민 나레이션 파일 네임
	std::string mNarFileName;
	// 텍스트 사운드 식별자
	int mTextAudioID;
	VoiceRecordManager* mRecordManager;
	Sequence* mProgressTimeBarSeq = nullptr;
	// 녹음 버튼 클릭 정보 저장
	bool mIsClickRecording     = false;
	bool mIsClickRecordingStop = false;

	Sprite* mSpRecordStroke = nullptr;
	Sprite* mSpPlayStroke = nullptr;
	
	// 프로그래스 바 
	ProgressTimer* mProgressTimeBar = nullptr;
	// rounded bar
	Sprite* mRoundedBar;
	void setProgressBarAction(Node* pSender);
	void setProgressBarRoundedAction(Node* pSender);

	// 버튼 스프라이트
	// 공통 컨트롤박스 첫번째 스프라이트
	Menu* mCtlRecord;
	MenuItemImage* mRecordItem;
	// 공통 컨트롤박스 두번째 스프라이트
	Menu* mCtlPlay;
	MenuItemImage* mPlayItem;
	// 공통 컨트롤박스 메뉴(스탑)
	Menu* mCtlRecordStop;
	MenuItemImage* mRecordStopItem;
	Menu* mCtlPlayStop;
	MenuItemImage* mPlayStopItem;
	Menu* mCtlTrash;
	MenuItemImage* mTrashItem;
	// 스피커
	Menu* mCtlSpeaker;
	MenuItemImage* mSpeakerItem;
	// 도움말
	Menu* mCtlHelp;
	MenuItemImage* mHelpItem;
	// 넥스트 
	Menu* mCltnext;
	MenuItemImage* mNextItem;
	// 닫기 버튼	
	Menu* mCltBack;

	// 모드
	RECORD_MODE::MODE mMode;
	// 자동 진입 모드인지 구분
	bool mIsAutoMode = false;
	
	// PopupLayer
	PopupLayer* mPopup;

	// callback
	void finishHelpClickCallback();
	void finishNextClickCallback();
	void finishCloseClickCallback();
	// 원어민 나레이션 완료후
	void finishNarTextCallback();
	// 플레이 완료후
	void finishPlayRecordCallback();
	// 녹음전 효과음 재생 완료후
	void finishEffectRecordCallback();
	// 녹음하기 최초 진입시 나레이션완료후
	void finishPreprocessRecordCallback();
	void finishPreprocessRecordSecondCallback();
	// 닫기 버튼 콜백
	void finishCloseBtnCallback();

	// 스피커 버튼 나레이션 완료후 호출
	void finishSpeakerSnd();
	// 녹음한 음성 재생하기 전 효과음 처리를 위한
	void finishEffectPlayRecordSndCallback();

	// 비율 구하기
	float getRatioDrawImage(Size size);

	// 레이아웃 초기화
	void initLayout();
	// 보이스 레코드 매니져 초기화
	void initVoiceRecordManager();
	
	void createUi();
	// 녹음전 처리
	// 녹음버튼 활성화, 재생버튼과 저장버튼은 비활성화
	void preprocessRecord();
	// 자동으로 녹음모드 진입
	void processAutoMode();
	void processCtlBtn();
	// 스티커 설정(Json파일로 부터 읽어 들인 정보)
	void initStickerForJsonFileWithRatio(STICKER::ITEM type, Vec2 pos, float ratio);

	// 녹음한 이력이 있는 경우 확인
	bool isExistRecordFile();
	bool isExistRecordHistory();
	// 이력 저장
	void saveRecordHistory();

	std::string getRecordFileName();
	// 그린 이력이 있는 경우 확인
	bool isExistDrawFile();	
	// 그린 이력이 있는 경우 확인(스티커)
	bool isExistDrawStickerFile();
	// 스티커 그림이 존재하는 경우 표시
	void setDrawLayer();
	//std::string getDrawFileName();
	void setCtlBtnForExistRecordFile();
	// 나레이션 텍스트
	std::string getStringRecordNarrationText();

	// 모드별 처리
	void preMode();
	void initMode();
	void narMode();
	void recordingMode();
	//void autoPlayMode();
	// 멈추기 버튼 클릭시
	void stopMode();
	// 녹음 완료시
	void doneMode();	
	// 스피커 재생 모드
	void speakerMode();
	//  플레이 모드
	void prePlayMode();
	//  플레이 모드
	void playMode();
	// 모드 및 버튼 설정
	void setMode(RECORD_MODE::MODE m);
	void delayMotionDone(Node* pSender);
	// 데이터 삭제를 위한 딜레이함수
	void delayCloseCallbackDone(Node* pSender);
	void delayHelpCloseCallbackDone(Node* pSender);
	// 닫기 버튼 누름 플레그
	int mIsClose = false;

	void setProgressBar(float duration);
	void initProgressBar();

	// 텍스트 버튼 처리
	void nextRecordLayer(int ordNum, std::string filename);
	void setNarFileName();

	// 버튼 활성/비활성
	void setEnableTouchBtn(bool b);
	// 넥스트 버튼 활성/비활성 설정
	void setNextBtn();

	// 넥스트 버튼 알파 모션
	void createNextButtonMotion();
	RepeatForever* mNextButtonMotion = nullptr;
	void setVisibleForNextBtn(bool b);
	// 중지 버튼 
	void createStopButtonMotion();
	RepeatForever* mStopButtonMotion = nullptr;	
	Sprite* mRecordStopFx = nullptr;
	Sprite* mRecordStopImg = nullptr;
	void setVisibleForStopBtn(bool b);
	// 도움말 버튼 활성/비활성(보이기/숨기기)
	void setVisibleForHelpBtn(bool b);
	bool mDisabledHelpBtn = false;

	// 녹음 플레이 타임 저장
	void setUserDataPlayTime(float time);
	// 녹음 플레이 타임 가져오기
	float getUserDataPlayTime();
	unsigned long getCurrentRecordTime();
	void setPlayTime();
	unsigned long mPlayStartTime;
	float  mProgressBarDurationTime;

public:	
	static Layer* createLayer(Node* parent, int weekData, int ord, std::string filename);
	
	virtual bool init();
	virtual void onExit();

	// a selector callback	
	// 닫기
	void menuCloseCallback(Ref* pSender);
	// 휴지통
	void menuTrashCallback(Ref* pSender);
	// 다음
	void menuNextCallback(Ref* pSender);
	// 도움말
	void menuHelpCallback(Ref* pSender);
	// 녹음하기
	void menuRecordCallback(Ref* pSender);
	// 플레이
	void menuPlayCallback(Ref* pSender);
	// Stop
	void menuRecordStopCallback(Ref* pSender);
	void menuPlayStopCallback(Ref* pSender);
	// 스피커
	void menuSpeakerCallback(Ref* pSender);
	void menuPopupClose();

	// implement the "static create()" method manually
	CREATE_FUNC(RecordLayer);	
	bool mIsStartRecord;
	// 녹음전 처리(권별 나레이션)
	void preprocessStartRecord();
	// 녹음전 처리(녹음 효과음재생)
	void processStartRecord();
	// 녹음
	void startRecord();// (Node* pSender);
	// 녹음중지
	void stopRecord();
	// 나레이션 플레이
	void playNarSnd(); // (Node* pSender);
	// 나레이션 플레이(스피커 버튼)
	void playNarSndForSpeaker(); // (Node* pSender);
	void finishNarTextForSpeakerCallback();
	// 녹음한 음성 재생
	void playRecordSnd();
	// 녹음전 효과음 재생
	void playEffectRecordSnd();
	// 녹음 완료 효과음 재생
	void playRecordDoneSnd();
	// 녹음 한 음성 재생하기전 효과음 재생
	void playBeforePlayRecordSnd();
	// 녹음완료 처리
	void doneRecord();
	// 삭제 처리
	void trashRecord();
	// 레이어 닫기
	void closeRecord();
	// 삭제 확인시
	void deleteRecord();

	// 10초 후 콜백함수
	void motionProgressBarDone(Node* pSender); // (Node* pSender);
	// 메인씬으로 돌아가기
	//void backToMainScene();
	//void setEnableTouchAllRecordBtn(bool b);

	//-------------------------------------------------------------
	// JNI 콜 후 처리
	//-------------------------------------------------------------
	void processJniStartSuccess();
	void processJniStopSuccess();
	void processJniStopSuccessForDone(); // 녹음 완료후 호출

	void processUserTouch();
};

#endif // __RECORD_LAYER_H__

