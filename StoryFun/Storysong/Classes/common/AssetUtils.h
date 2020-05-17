#ifndef _ASSET_UTILS_H_
#define _ASSET_UTILS_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"


USING_NS_CC;
USING_NS_GAF;


class AssetUtils
{
public:
	static GAFAsset * createAutoSoundAsset(const std::string& gafFilePath, GAFTextureLoadDelegate_t delegate, GAFLoader* customLoader = nullptr);
	static GAFAsset * createAutoSoundAsset(const std::string& gafFilePath);
	static GAFAsset * createAutoSoundAsset(int index);
};


class GAFBUtton : public Ref
{
public:
	GAFBUtton();
	~GAFBUtton();

	static GAFBUtton * create(gaf::GAFObject * object);

public:
	void onPressed();
	void onReleased();
	void onTouchOut();

private:
	gaf::GAFObject * button;
	bool init(gaf::GAFObject * object);
};
#endif