#pragma once

#include "cocos2d.h"
#include "CCFileUtils.h"
#include <string>
USING_NS_CC;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
#include "jni.h"
#include "jni/JniHelper.h"
#define KJAVA_REFERENCE_CLASS "com/kid/KDataProvider"
#endif

#define KVIRTUAL_PATH	"/virtual/"
#define KTEMP_PATH		"/mnt/sdcard/Storyfun_ibook/"

#define KCTT(inner) KDataProvider::cTT(inner)
#define KCTV(inner) KDataProvider::cTV(inner)

class KDataProvider  {

public:
	static KDataProvider* getInstance();
	static void removeInstance();
	static std::string modifyEntityPath(std::string aPath);
	static std::string convertToTempPath(const std::string & sEntity);
	static std::string convertToVirtual(const std::string & sEntity);
	static std::string cTT(const std::string & sEntity);
	static std::string cTV(const std::string & sEntity);
	static std::string mDebugMode;
private:
	static KDataProvider * _instance;
	static std::string replaceString(std::string subject, const std::string & search, const std::string & replace);
	static std::string mTempDirPath;
	static std::string mVirtualPath;
	
	 
	
private:
	std::string misFindshed;
	int mCurPage;
	int mListenMode;
	std::string mXmlData;
	bool 	   mIsFirst;
	bool	   mIsSound;
	void initData();
	bool		mIsMainBgm;   //전체 백그라운드 bgm == true
	

public:
	void loadXmlData(std::string sFilename);
	std::string getXmlData();
	std::string saveDataToFile(std::string sEntity);
	std::string saveDataToCache(std::string sEntity);
	std::string getMediaType();
	
	void addEntryName(std::string name);
	//void setEntryList(std::vector<std::string> sEntity);
	void preload();
	void openDrmZipFile();
	void closeDrmZipFile();
	long getMillTime();

	void isFinished();
	void getProcess();
	int getProcessPage() { return mCurPage; } // mCurPage;
	bool hasNext();

	std::string getOneFinished() { return misFindshed; }
	void setFinished() { misFindshed = "y"; }
	void setBeginBook(int ncurpage,int ntotalpage,int isauto);
	void setProgressPage(int ncurpage);
	void currentBookFinish(std::string sIsFinish);
	void nextBook();
	bool getIsFirst() { return mIsFirst; }
	/* 처음 시작함을 알림 */
	void setFirst(bool v) { mIsFirst = v; }
	void setSoundState(bool bSound) {mIsSound = bSound;}
	bool getSoundState() {return mIsSound;}
	/* 읽기 듣기 모드  */
	void setListenMode(int nMode);
	int getListenMode();
	int getCurListenMode() { return mListenMode; }
	int getSystemVolumn();
	void acquireCpuWakeLock();
	void releaseCpuLock();
	void setMainBgm(bool bflag) {mIsMainBgm = bflag;}
	bool getMainBgm() { return mIsMainBgm; }

	bool bJtpWarningPopupOpen;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
private:
	jobject mRefJObj;
	int getBytesFromEntity(std::string sEntity, unsigned char ** ppData);
	int getBytesFromEntityInOpenedFile(std::string sEntity, unsigned char ** ppData);
public:
	void setProgress(int aProgress);
	void showToastForJNI(std::string sMessage);
	
#endif

	

};