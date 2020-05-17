#ifndef _SONG_LYRICS_LAYER_H_
#define _SONG_LYRICS_LAYER_H_

#include "cocos2d.h"
#include "data/def.h"

USING_NS_CC;

class SongLyricsLayer : public Layer, public IBaiscSongPlay
{
public:
	SongLyricsLayer();
	~SongLyricsLayer();

	static SongLyricsLayer * create(std::vector<std::string> lyricsList);

	virtual void onPlaybyTickEvent(int tickCount);
	virtual void onPlayModeChange(PLAY_MODE::PlayType playMode);
	virtual void onPlayTouchChange(PLAY_MODE::TouchType touchMode);

public:
	void resetLayer();

private:
	void initData(std::vector<std::string> lyricsList);
	void initGUI();
	void runObject(int tickCount);
private:
	Sprite *					_subLyricsSprite;
	std::vector<std::string>	_lyricsList;

};

#endif