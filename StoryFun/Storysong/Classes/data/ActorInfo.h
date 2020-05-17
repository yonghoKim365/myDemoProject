#ifndef _ACTOR_INFO_H_
#define _ACTOR_INFO_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"

USING_NS_CC;
USING_NS_GAF;

class ActorInfo
{
public:
	ActorInfo();
	~ActorInfo();

	void setPosition (Vec2 position);
	Vec2 getPosition ();
	void setFilePath (std::string path);
	std::string getFilePath();
	void setRect(CCRect * rect);
	CCRect * getRect();

private:
	std::string filePath;
	float       positionX;
	float       positionY;
	CCRect	*   touchRect;
	
};

#endif