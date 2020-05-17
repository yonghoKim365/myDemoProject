#include "AssetUtils.h"
#include "audio/include/AudioEngine.h"
#include "data/def.h"
#include "data/PlayInfo.h"
#include "data/JsonInfo.h"


GAFAsset * AssetUtils::createAutoSoundAsset(const std::string& gafFilePath, GAFTextureLoadDelegate_t delegate, GAFLoader* customLoader)
{
	auto asset = GAFAsset::create(gafFilePath, delegate, customLoader);

	asset->setSoundDelegate([asset](GAFSoundInfo* sound, int32_t repeat, GAFSoundInfo::SyncEvent syncEvent){
		std::string path = asset->getGAFFileName();
		int slashPos = path.find_last_of("/");
		path = path.substr(0, slashPos + 1);
		path.append(sound->source);

		cocos2d::experimental::AudioEngine::play2d(path, repeat == -1);
	});

	return asset;
}

GAFAsset * AssetUtils::createAutoSoundAsset(const std::string& gafFilePath)
{
	auto asset = GAFAsset::create(gafFilePath);

	asset->setSoundDelegate([asset](GAFSoundInfo* sound, int32_t repeat, GAFSoundInfo::SyncEvent syncEvent){
		int lastSound = PlayInfo::create()->getLastSoundID();
		if (lastSound != -1)
		{
			cocos2d::experimental::AudioEngine::stop(lastSound);
		}

		std::string path = asset->getGAFFileName();
		int slashPos = path.find_last_of("/");
		path = path.substr(0, slashPos + 1);
		path.append(sound->source);

		int soundId = cocos2d::experimental::AudioEngine::play2d(path, repeat == -1);
		PlayInfo::create()->setLastSoundID(soundId);
	});

	return asset;
}


GAFAsset * AssetUtils::createAutoSoundAsset(int index)
{
	auto asset = JsonInfo::create()->getGafAsset(index);
	
	asset->setSoundDelegate([asset](GAFSoundInfo* sound, int32_t repeat, GAFSoundInfo::SyncEvent syncEvent){
		int lastSound = PlayInfo::create()->getLastSoundID();
		if (lastSound != -1)
		{
			cocos2d::experimental::AudioEngine::stop(lastSound);
		}

		std::string path = asset->getGAFFileName();
		int slashPos = path.find_last_of("/");
		path = path.substr(0, slashPos + 1);
		path.append(sound->source);

		int soundId = cocos2d::experimental::AudioEngine::play2d(path, repeat == -1);
		PlayInfo::create()->setLastSoundID(soundId);
	});

	return asset;
}

GAFBUtton::GAFBUtton(){}
GAFBUtton::~GAFBUtton(){}

GAFBUtton * GAFBUtton::create(gaf::GAFObject * object)
{
	GAFBUtton * button = new GAFBUtton();
	if (button && button->init(object))
	{
		button->autorelease();
		return button;
	}
	else
	{
		CC_SAFE_RELEASE(button);
		return nullptr;
	}
}

bool GAFBUtton::init(gaf::GAFObject * object)
{
	button = object;
	button->gotoAndStop("Normal");

	return true;
}

void GAFBUtton::onPressed()
{
	button->gotoAndStop("Press");
	cocos2d::experimental::AudioEngine::play2d(BUTTON_EFFECT_SOUND_PATH);
}

void GAFBUtton::onReleased()
{
	button->gotoAndStop("Normal");
}

void GAFBUtton::onTouchOut()
{
	button->gotoAndStop("Normal");
}
