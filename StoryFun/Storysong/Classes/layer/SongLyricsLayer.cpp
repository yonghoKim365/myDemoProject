#include "SongLyricsLayer.h"

SongLyricsLayer::SongLyricsLayer(){}
SongLyricsLayer::~SongLyricsLayer() {}

SongLyricsLayer * SongLyricsLayer::create(std::vector<std::string> lyricsList)
{
	SongLyricsLayer *ref = new SongLyricsLayer();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->initData(lyricsList);
		ref->initGUI();
		return ref;
	}
	else
	{
		CC_SAFE_DELETE(ref);
		return nullptr;
	}
}

void SongLyricsLayer::initData(std::vector<std::string> lyricsList)
{
	_lyricsList = lyricsList;
}

void SongLyricsLayer::initGUI()
{
	_subLyricsSprite = Sprite::create();
	_subLyricsSprite->setPosition(Vec2(960, 1050));
	addChild(_subLyricsSprite);
}

void SongLyricsLayer::resetLayer()
{
	_subLyricsSprite->setVisible(false);
}

void SongLyricsLayer::onPlaybyTickEvent(int tickCount)
{
	std::string lyricsPath = _lyricsList.at(tickCount);
	_subLyricsSprite->setVisible(true);
	_subLyricsSprite->setTexture(lyricsPath.c_str());

	/*auto scheduler = Director::getInstance()->getScheduler();
	scheduler->performFunctionInCocosThread(CC_CALLBACK_0(SongLyricsLayer::runObject, this, tickCount));*/
}

void SongLyricsLayer::runObject(int tickCount)
{
	std::string lyricsPath = _lyricsList.at(tickCount);
	_subLyricsSprite->setTexture(lyricsPath.c_str());
}

void SongLyricsLayer::onPlayModeChange(PLAY_MODE::PlayType playMode)
{
	if (playMode == PLAY_MODE::MODE_PLAY || playMode == PLAY_MODE::MODE_LISTEN)
	{
		_subLyricsSprite->setTexture("");
		this->setVisible(true);
	}
	else
		this->setVisible(false);

}

void SongLyricsLayer::onPlayTouchChange(PLAY_MODE::TouchType touchMode)
{

}