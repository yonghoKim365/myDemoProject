#ifndef __VOICE_RECORD_MANAGER_H__
#define __VOICE_RECORD_MANAGER_H__

#include "cocos2d.h"

USING_NS_CC;

/**
* @brief 녹음하기 관리자
*/
class VoiceRecordManager
{
public:
	// single ton
	static VoiceRecordManager *getInstance();
	Node* mParent;

	////////////////////////////////////////
	// voice operation
	//bool checkMic();
	static void start(Node* parent, int week, int ord);
	static void stop(Node* parent);
	static void doneStop(Node* parent);
	static void stop();
	static void play();

	// Jni Callback
	static void infromVoiceRecord();
	static void informVoiceRecordCancel();
	static void infromVoiceRecordStop();

	bool isRecording();
	VoiceRecordManager();
	virtual ~VoiceRecordManager();
	bool mIsRecording;
	int  mMode = 0;

protected:

	virtual bool init();

private:
	static VoiceRecordManager* _voiceRecordManager;
};


#endif //__VOICE_RECORD_MANAGER_H__
