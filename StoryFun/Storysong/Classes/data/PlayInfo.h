#ifndef _PLAY_INFO_H_
#define _PLAY_INFO_H_

#include "data/def.h"
#include "cocos2d.h"

USING_NS_CC;


class PlayInfo
{
private:
	static PlayInfo * obj;
	PlayInfo();
	~PlayInfo();

public:
	static PlayInfo * create();
	static void releaseInstance();


public:
	void setCurrentPlayType(PLAY_MODE::PlayType playType);
	PLAY_MODE::PlayType getCurrentPlayType();
	std::string getNarrationPath();

	void setLastSoundID(int id);
	int  getLastSoundID();

	void setLastEffectID(int id);
	int  getLastEffectID();

	void setBGMID(int id);
	int getBGMID();

	void setGuidePlayed(bool value);
	bool getGuidePlayed();

private:
	PLAY_MODE::PlayType		currentMode;
	bool		isPlayed = false;
	int			soundId;
	int			effectId;
	int			bgmID;
};

#endif