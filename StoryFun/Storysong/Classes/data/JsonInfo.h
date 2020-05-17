#ifndef _JSON_INFO_H_
#define _JSON_INFO_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "data/def.h"

USING_NS_CC; 
USING_NS_GAF;

class GafObjectInfo
{
public:
	std::string gafPath;
	float		positionX;
	float		positionY;
};

class ExampleInfo : public GafObjectInfo
{
public:
	CCRect * touchBox;
};

class JsonInfo
{
private:
	static JsonInfo * obj;
	JsonInfo();
	~JsonInfo();

public:
	static JsonInfo * create();
	static void releaseInstance();

	GAFAsset * getGafAsset(int index);

public:
	bool						isDebugMode; 
	int							startStep;
	int							currentWeek;
	GafObjectInfo				machineInfo;

	std::vector<std::string>	touchPathList;
	std::vector<std::string>	trackLyricsList;
	std::vector<std::string>	trackExampleList;
	std::vector<std::string>	wordExampleList;
	std::vector<GAFObject *>	touchObjList;
	std::vector<std::string>	trackTouchList;
	
	std::vector<std::string>	step1ObjectList;

	std::vector<float>			trackTimeList; 

	std::vector<CCRect *>		touchObjectRect;
	std::vector<GafObjectInfo>	touchObjectList;

	std::vector<ExampleInfo>	exampleInfoList;

	std::string					songPath;
	std::string					introBGPath;
	std::string					mainBGPath;
	std::string					introAnimPath;
	std::string					introAnimListenPath;
	std::string					step1ObjectPath;
	std::string					machineEffectPath;

	bool						isFirstPlay = true;
	ANIMATION_TYPE::SongType	currentAnimationType;

	void setStartStep(int step);
	int getStartStep();

	void setCurrentWeek(int week)
	{
		currentWeek = week;

		if (currentWeek == 2 || currentWeek == 9 || currentWeek == 13 || currentWeek == 17 || currentWeek == 17 || currentWeek == 24)
			currentAnimationType = ANIMATION_TYPE::ANIMATION_TYPE_A;
		else if (currentWeek == 3 || currentWeek == 6 || currentWeek == 11 || currentWeek == 15 || currentWeek == 18 || currentWeek == 19 || currentWeek == 23)
			currentAnimationType = ANIMATION_TYPE::ANIMATION_TYPE_B;
		else if (currentWeek == 4 || currentWeek == 7 || currentWeek == 12 || currentWeek == 14 || currentWeek == 22 )
			currentAnimationType = ANIMATION_TYPE::ANIMATION_TYPE_C;
		else
			currentAnimationType = ANIMATION_TYPE::ANIMATION_TYPE_D;

	}

	std::string getEffectSoundPath()
	{
		if (currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_B)
			return "common/sound/storysong_guidemotion_balloonmachine_2_sound20.mp3";
		else if (currentAnimationType == ANIMATION_TYPE::ANIMATION_TYPE_C)
			return "common/sound/storysong_ball_sound26.mp3";
		else
			return "common/sound/storysong_colors_orange_sound11.mp3";
	}
};

#endif