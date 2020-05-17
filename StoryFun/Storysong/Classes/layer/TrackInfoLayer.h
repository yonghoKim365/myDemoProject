#ifndef _TRACK_INFO_H_
#define _TRACK_INFO_H_

#include "cocos2d.h"

USING_NS_CC;

class SongTickEventListener
{
public:
	virtual void onTickEvent(int index) = 0;
	virtual void onFinishedEvent() = 0;
};

class SongTickManager : public Layer
{
public:
	SongTickManager();
	~SongTickManager();

	static SongTickManager * create(std::vector<float> tickList, SongTickEventListener * listener);
	void start(std::string bgmPath);
	void stop();
	virtual void update(float dt);

private:
	void initData(std::vector<float> tickList);
	void initData();
	void setTickEventListener(SongTickEventListener * listener);
	void pauseUpdate();
	void resumeUpdate();
	void onFinishCallBack(int audioID, std::string filePath);

	void startBGM();
	void stopBGM();

private:
	SongTickEventListener * _listener;
	int						_tickEventCount;
	float					_playTime;
	std::vector<float>		_tickList;
	bool					isScheduleRunning;
	int						bgmID;
	float					_totlaTime;
	float					_runningTime;

	
};

#endif