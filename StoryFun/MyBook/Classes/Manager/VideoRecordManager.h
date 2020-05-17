#ifndef __VIDEO_RECORD_MANAGER_H__
#define __VIDEO_RECORD_MANAGER_H__

#include "cocos2d.h"
USING_NS_CC;

/**
* @brief 재생하기 관리자
*/
class VideoRecordManager 
{
public:
	// single ton
	static VideoRecordManager *getInstance();
	
	////////////////////////////////////////
	// voice operation
	//bool checkMic();
	void start(Node* parent, int week, std::string strPath);
	// 엔딩 팝업에서 호출
	void startForEndingPage(Node* parent, int week, std::string strPath);
	void stop();	
	void returnMainScene();
	void returnPlayScene();
	void playAnimation();

	bool isRecording();
	VideoRecordManager();
	virtual ~VideoRecordManager();
	bool mIsRecording;
	// ADD 10.19
	void returnMainSceneForSleep();
protected:	
	virtual bool init();

private:	
	static VideoRecordManager* _videoRecordManager;
	Node* mParent;
};


#endif //__VIDEO_RECORD_MANAGER_H__
