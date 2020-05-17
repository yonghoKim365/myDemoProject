#ifndef __KRecordingManager_H__
#define __KRecordingManager_H__

#include "cocos2d.h"
#include "CCFileUtils.h"
#include <string>
USING_NS_CC;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
#include "jni.h"
#include "jni/JniHelper.h"
#define KRECORDING_JAVA_REFERENCE_CLASS "com/kid/KRecordingManager"
#endif

#define START_SOUND_FILENAME "recsound.mp3"
#define DEFAULT_DURATION 15000;

class KRecordingManagerDelegate {
public:
	virtual void onRecordingStarted() = 0;
	virtual void onRecordingEnded() = 0;
	virtual void onListeningEnded() = 0;
};

class KRecordingManager {
public:
	static KRecordingManager * getInstance();

private:
	static KRecordingManager * _instance;
	void initData();

	void callForAndroidNative();
	void callForAndroidNative_StopRecording();
	std::string mStartSoundFilename;
	std::string mRecordingFilename;
	int m_nDuration;

private:
	KRecordingManagerDelegate * _delegate;

	std::string mAndroidTargetPath;
	std::string mRecordFullPath;

	std::string sActionTargetName;

	

public:
	void startRecording();
	void stopRecording();
	void setStartSoundFilename(std::string aName);
	void setRecordingFilename(std::string aName);

	void setDuration(int aDuration);
	int getDuration();

	void recordingEnded(std::string aName);
	void listeningEnded();

	void startListening(std::string  sPath);
	void stopListening();

	std::string getAndroidTargetPath();
	std::string getRecordFullPath();
	void makesureFolderExists(std::string sPath);

	void setActionTargetName(std::string aTargetName);
	std::string getActionTargetName();

	void showToastForJNI(std::string sMessage);

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
private:
	jobject mRefJObj;
#endif

public:
	void setDelegate(KRecordingManagerDelegate * delegate);
};

#endif
