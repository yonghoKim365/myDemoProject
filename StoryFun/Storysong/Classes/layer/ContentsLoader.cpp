#include "ContentsLoader.h"

ContentsLoader* ContentsLoader::create()
{
	ContentsLoader * ref = new ContentsLoader();
	if (ref && ref->init())
	{
		ref->autorelease();
		ref->initGUI();
		return ref;
	}
	else
	{
		CC_SAFE_RELEASE(ref);
		return nullptr;
	}
}

void ContentsLoader::initGUI()
{
	animLayer = Layer::create();
	addChild(animLayer);
}

void ContentsLoader::showContentsLoader()
{
	auto asset = GAFAsset::create(getLoaderAnimPath().c_str());
	auto object = asset->createObjectAndRun(true);
	object->setPosition(getCenterPosition());
	
	animLayer->addChild(object);
}

void ContentsLoader::hideContentsLoader()
{
	removeFromParent();
}

void ContentsLoader::showContentsLoadderWidthContentPath(std::vector<std::string> contentsPath)
{

}

std::string ContentsLoader::getLoaderAnimPath()
{
	auto director = Director::getInstance();
	Size pixelSize = director->getWinSizeInPixels();
	
	if (pixelSize.width == 1920.0f)
		return "common/gaf/contents_preloader_1920_160615/contents_preloader_1920_160615.gaf";
	else
		return "common/gaf/contents_preloader_1280_160615/contents_preloader_1280_160615.gaf";
}

Vec2 ContentsLoader::getCenterPosition()
{
	auto director = Director::getInstance();
	Size pixelSize = director->getWinSizeInPixels();

	if (pixelSize.width == 1920.0f)
		return Vec2(0.0f, 1200.0f);
	else
		return Vec2(0.0f, 800.0f);
}