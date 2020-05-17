#ifndef _CONTENTS_LOADER_H_
#define _CONTENTS_LOADER_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

class ContentsLoader : public Layer
{
public:
	static ContentsLoader * create();

public:
	void showContentsLoader();
	void hideContentsLoader();
	void showContentsLoadderWidthContentPath(std::vector<std::string> contentsPath);

private:
	void initGUI();
	std::string getLoaderAnimPath();
	Vec2 getCenterPosition();

private:
	Layer * animLayer;
};

#endif