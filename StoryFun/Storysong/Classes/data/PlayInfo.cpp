#include "data/PlayInfo.h"



PlayInfo * PlayInfo::obj = NULL;

PlayInfo::PlayInfo()
{
	currentMode = PLAY_MODE::MODE_LISTEN;
}

PlayInfo::~PlayInfo(){}

PlayInfo * PlayInfo::create()
{
	if (obj == NULL)
		obj = new PlayInfo();

	return obj;
}

void PlayInfo::releaseInstance()
{
	if (obj != NULL)
		delete obj;
}

void PlayInfo::setCurrentPlayType(PLAY_MODE::PlayType playType)
{
	currentMode = playType;
}

PLAY_MODE::PlayType PlayInfo::getCurrentPlayType()
{
	return currentMode;
}

std::string PlayInfo::getNarrationPath()
{
	switch (currentMode)
	{
	case PLAY_MODE::MODE_LISTEN:
		return GUID_LISTEN_SOUND_PATH;

	default:
		return GUID_PLAY_SOUND_PATH;
		break;
	}
}

void PlayInfo::setLastSoundID(int id)
{
	soundId = id;
}

int  PlayInfo::getLastSoundID()
{
	CCLOG("PlayInfo::getLastSoundID =%d", soundId);
	return soundId;
}

void PlayInfo::setLastEffectID(int id)
{
	effectId = id;
}

int PlayInfo::getLastEffectID()
{
	return effectId;
}

void PlayInfo::setBGMID(int id)
{
	bgmID = id;
}

int PlayInfo::getBGMID()
{
	return bgmID;
}


void PlayInfo::setGuidePlayed(bool value)
{
	isPlayed = value;
}

bool PlayInfo::getGuidePlayed()
{
	return isPlayed;
}