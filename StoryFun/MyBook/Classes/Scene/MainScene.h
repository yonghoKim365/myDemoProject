#ifndef __MAIN_SCENE_H__
#define __MAIN_SCENE_H__

#pragma once
#include "cocos2d.h"
#include "Contents/MyBookSnd.h"
#include "Manager/CameraManager.h"
//#include "Manager/VoiceRecordManager.h"
#include "Util/Pos.h"
#include "Contents/MyBookResources.h"
#include "Util/MBJson.h"

// Json
#include "json/rapidjson.h"
#include "json/document.h"
#include "json/filestream.h"

// GAF Player
#include "Cocos2dxGAFPlayer-release-gaf-5/Library/Sources/GAF.h"
#include "Cocos2dxGAFPlayer-release-gaf-5/Library/Sources/GAFPrecompiled.h"
//#include "GAFPlayer/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

//using namespace rapidjson;

class MainScene : public Layer, public CameraManagerListener
{
public:
	static Scene* createScene();
	virtual bool init();
	virtual void onExit();
	

	// a selector callback
	void menuCloseCallback(Ref* pSender);	

	// implement the "static create()" method manually
	CREATE_FUNC(MainScene);	

	void doNotification(Ref *obj);
	// 배열하기 닫기
	void closeDrawLayer();
	// 팝업창 닫기 버튼(팝업레이어에서 호출)
	void menuPopupClose();

	// 메인씬 닫기
	void exitPlay();
	Layer*  mPlayLayer;   // 재생하기 레이어

	// 배열하기 완료후 플레이 및 재배열 버튼 보이기/숨기기
	void visibleBtnPlayAndReorder(bool b);
	void closePlayLayerForEndingScene();
	// 배열하기 버튼 활성화/비활성화
	void setEnableAllCubeButton(bool b);
	// 터치 기능 활성 비활성(팝업용, 레이어이동시)
	void setEnableTouchBtn(bool b);

private:
	// 카메라 버튼
	MenuItemImage* mCameraItem;
	// 카메라 버튼
	MenuItemImage* mHelpItem;
	// 멀티 터치 막기
	bool mTouchFlag = false;

	// 실패 효과음 재생 여부
	bool mIsInterSect = false;
	// 프로필 설정
	void initProfile();
	void createProfileWithUni();
	bool isExistProfile(std::string strFilePath);
	void renameProfileFileName(Node* pSender);
	// 배열하기에서 그리기, 녹음하기 기능으로 이동시 
	// 리스너 및 버튼 비활성화
	void processMoveToLayer();

	// 시작 나레이션
	void playNarStart(Node* pSender, int idx);
	void finishNarTextCallback();
	int mNarIndex;
	//------------------------------------------------------------
	// COMMON
	//------------------------------------------------------------
	// screen size
	Size mScreenSize;

	Menu* mCloseMenu;

	float fx;
	float fy;
	Point ptStart;
	Point ptEnd;
	float fdistance;
	float fd;
	float fdifx;
	float fdify;
	float fdelta;
	
	// 호 번호	
	int mWeekData;

	// Json 파일로 부터 이미지 로드
	void initCube();
	// 레이아웃 초기화
	void initLayout();
	// GAF 초기화
	//void initGaf();
	// 배경 및 버튼 만들기
	void createUi();

	////////////////////////////////////////
	// 터치 이벤트 리스너
	EventListenerTouchAllAtOnce* mListener;
	// 터치 초기화
	void initTouchEvent();

	virtual void onTouchesBegan(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesMoved(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesEnded(const std::vector<Touch*>& touches, Event* event);
	virtual void onTouchesCancelled(const std::vector<Touch*>& touches, Event* event);
		
	////////////////////////////////////////
	// 레이어
	Layer*  mCubeLayer;   // 배열하기 레이어
	Layer*  mDrawLayer;   // 그리기   레이어
	Layer*  mRecordLayer; // 녹음하기 레이어
	//Layer*  mPlayLayer;   // 재생하기 레이어
	//Layer*  mPlayLayer2;  // 재생하기 레이어


	// 호출하는 레이어
	// 1 : 배열하기 레이어, 2: 그리기 레이어, 3: 녹음하기레이어
	DEPTH_LAYER::ORDER  mLayerIndex; 

	//  사운드
	void initBGM();
	void stopBGM();
	// 배경음 플레이
	void playBGM();

	//------------------------------------------------------------
	// 배열하기
	//------------------------------------------------------------	
	// 이동되었는지 체크 -> 효과음 재생 여부 판단기준
	bool mIsCubeMove = false;
	// 선택된 그림 사이즈 저장(화면 밖으로 이동 금지)
	Size mSelectCubeSize;
	// 애니매이션 중인지 체크
	bool mIsPlayCubeAni;
	// 팝업 창
	Sprite* mPopupSp;

	// 녹음하기 버튼
	std::vector<MenuItemImage*> mBtnRecords;
	MenuItemImage* recordItems[SIZE_BTN_RECORD];
	// 그리기 버튼
	std::vector<MenuItemImage*> mBtnDraws;
	MenuItemImage* drawItems[SIZE_BTN_DRAW];
	// 고정용 이미지일경우 버튼 제외처리 용 인덱스(고정 인덱스 제외)
	int mIndex[SIZE_USER_CUBE];

	// 활성 / 비활성 정보 저장(팝업 또는 레이어 이동시 필요)
	bool mBtnRecordInfo[SIZE_BTN_RECORD];
	bool mBtnDrawInfo[SIZE_BTN_DRAW];

	// 사용자가 선택한 그림 인덱스
	int  mCubeSelectIdx = INVALID_INDEX; // 0 ~ 3
	// 그림 선택
	bool  mSelectCubeFlag = false; 


	/////////////////////////////////////////////////////
	// JSON 으로 부터 읽어 들인 내용
	// 그림 움직임 유무, base index는 1부터
	// true : 고정(그리기없음), false : 움직임 처리
	//bool  mLockCubes[SIZE_CUBE + 1];  
	//// 그림 파일 명(배열하기용), base index는 1부터 : FILENAME_CUBE_X
	//std::string  mCubeFileNames[SIZE_CUBE + 1]; 
	//// 그림 파일 명(그리기), base index는 1부터 : FILENAME_DRAW_X
	//std::string  mDrawFileNames[SIZE_CUBE + 1];
	// MBJson
	MBJson* mMBJson;
	// 사용자 그림 파일 명
	std::string  mUserCubeFileNames[SIZE_USER_CUBE];

	// 플레이하기 & 재배열하기
	Menu*  menuPlayAndOrder;
	// 완료된 인덱스 값
	int mCubeFinishIdxs[SIZE_CUBE];
	// 완료된 그림 갯수
	int mCubeCompleteCount = 0; 

	// 터치 입력 유무
	// 나레이션 완료후 true로 변경
	bool mUseTouch = false;
	// 사용자 그림 최초 위치
	Vec2 mCubePos[SIZE_USER_CUBE];

	// 그림 배경(위치시킬 기준점)
	std::vector<Sprite*> mSpCubeBgs;
	// 사용자 선택 그림(최종)
	std::vector<Sprite*> mSpCubes;	
	std::vector<Sprite*> mSpCubesLock; // 상단 고정용
	// 사용자 선택 그림 성공 유무
	CUBE_STATE::STATE mCubeStats[SIZE_USER_CUBE];

	// play 배열 / 재배열
	short mRandNumber[SIZE_CUBE];
	// 렌덤 넘버 생성
	void GenRandIndex();

	// 애니메이션을 위한 인덱스값
	int mAniReorderCubeIdx;
	bool checkRandNumber(int rand, int max);	
	// 애니메이션
	void aniReorderCube();
	// 사용자 그림 리셋
	void resetUserCube();

	////////////////////////////////////////
	// 배열하기용 이미지 생성
	void createUserCube(int idx, int bIdx);
	// 배열하기 최초 이동 모션
	void motionInitIntervalCube();
	// 재배열하기 모션
	void motionReorderCube();
	// 그림 이동 모션
	void motionInitCubeIndex(Node* pSender);
	void motionBlickInitCubeIndex(Node* pSender);
	//void motionInitCube(int idx);
	void motionInitCubeDone(Node* pSender);	
	void motionBlickInitCubeDone(Node* pSender);
	// 사라지는 모션
	void motionFadeOutCubeIndex(Node* pSender, int idx);
	void motionInitCubeIndexDone(Node* pSender);
	void motionFadeOutCubeDone(Node* pSender);
	// 재배열시 사라질때 효과음
	void soundEffectReorder(Node* pSender);
	// Json file 읽기
	void readJsonFile();
	// 배열하기 터치 초기화
	void initDragCube(Touch* touch);
	// 카메라 버튼 이벤트 콜백
	void menuCameraCallback(Ref* pSender);
	// 녹음하기 버튼 콜백
	void menuRecordCallback(Ref *sender, int idx);
	// 그리기 버튼 콜백
	void menuDrawCallback(Ref *sender, int idx);
	// 재생하기 버튼
	void menuPlayCallback(Ref* pSender);
	// 재배열하기 버튼
	void menuReorderCallback(Ref* pSender);
	// 그리기 닫기 이벤트 처리
	void menuDrawCloseCallback(Ref* pSender);
	// 팝업창 닫기 버튼
	void menuPopupCloseCallback(Ref* pSender);

	// 팝업창 그리기 유지 버튼
	void menuPopupResumeCallback(Ref* pSender);
	// 도움말 창 버튼
	void menuHelpCallback(Ref* pSender);
	void delayMediaScanning(Node* pSender);

	// 완료된 인덱스값 초기화	
	void resetCubeIndex();
	// 드래그 처리
	void dragCube(Touch* touch);
	// 터치 완료후 처리
	void dragCubeFinish(Touch* touch);
	// 터치 완료후 그림이 포함된 경우
	void processCubeFinish(int idx, int btnIdx);
	// 터치 완료후 그림이 포함되지 않은 경우 최초위치로 이동
	void processCubeFinishFail();
	// 터치 완료후 그림이 포함되지 않은 경우 최초위치로 이동 완료
	void motionCubeFinishFailActionDone(Node *pSender);
	// 배열하기 완료후 플레이 및 재배열 버튼 보이기/숨기기
	//void visibleBtnPlayAndReorder(bool b);
	// 배열하기 성공 유무
	bool isAllFinish();
	// 배열하기 성공후 처리
	void allCubeFinish();
	// 재배열하기
	void reorderCube();
	
	// 배열하기 버튼 활성화/비활성화
	//void setEnableAllCubeButton(bool b);
	void setEnableRecordButtonWithIndex(bool b, int index);
	void setEnableDrawButtonWithIndex(bool b, int index);
	/*bool getEnableDrawButtonWithIndex(int index);
	bool getEnableRecordButtonWithIndex(int index);*/
	void setBtnInfo();
	// 저장된 버튼 활성/비활성 정보 복원
	void recoverEnableBtn();
	// 터치 기능 활성 비활성(팝업용, 레이어이동시)
	//void setEnableTouchBtn(bool b);
	// 배열하기 모든 버튼 비활성화 및 녹음화기 버튼 활성화
	void setResetAllCubeButton();

	// 배열하기 정답확인
	bool isCubeSuccess(int idx);
	// 최초 진입시 또는 팝업시 버튼 비활성
	void setEnableBtn(bool b);

	//------------------------------------------------------------
	// 녹음하기
	//------------------------------------------------------------	
	// 녹음하기
	//VoiceRecordManager* mVoiceManager;
	// 닫기
	void closeRecordLayer();

	void closePlayLayer();
	void resumePlayLayerEndingPage();

	//------------------------------------------------------------
	// 그리기
	//------------------------------------------------------------
	// 그리기 초기화
	//void initDraw();

	//------------------------------------------------------------
	// 카메라
	//------------------------------------------------------------
	// 배경 레이어 (사진 촬영이 해당 레이어에 추가)
	Layer* mCameraLayer = nullptr; // bgLayer;

	// 카메라 초기화
	void initCamera();

	/// 카메라 메니저 리스너 (카메라 종료)
	void onCameraFinished(bool result);
	/// 카메라에서 찍은 이미지 셋팅
	void onCameraFinishedReal();
	
	void processUserTouch();
};


#endif // __MAIN_SCENE_H__