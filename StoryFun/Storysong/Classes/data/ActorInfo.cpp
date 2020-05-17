#include "ActorInfo.h"


ActorInfo::ActorInfo(){}
ActorInfo::~ActorInfo(){}

void ActorInfo::setPosition(Vec2 position)
{
	positionX = position.x;
	positionY = position.y;
}

Vec2 ActorInfo::getPosition()
{
	return Vec2(positionX, positionY);
}

void ActorInfo::setFilePath(std::string path)
{
	filePath = path;
}

std::string ActorInfo::getFilePath()
{
	return filePath;
}

void ActorInfo::setRect(CCRect * rect)
{
	touchRect = rect;
}

CCRect * ActorInfo::getRect()
{
	return touchRect;
}