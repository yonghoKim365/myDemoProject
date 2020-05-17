


#ifndef __FRMWRKCOMMON__Actor_H
#define __FRMWRKCOMMON__Actor_H

#pragma once
#include "cocos2d.h"
#include <spine/spine-cocos2dx.h>
#include <Math.h>
#include "GAF.h"


NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;
USING_NS_CC;



class Actor : public cocos2d::LayerColor
{
public:
	Actor();
	~Actor();


private:
	GAFObject * m_pActor;
	Vec3	* m_pInitPos;
	Vec3	* m_crntDisplayPos;
	Vec3	m_zeroVec3;

	bool	m_isOnGrnd;
	float	m_fVrtSpeed;
	float	m_fHozSpeed;

	//	sequence state.
	bool	isResume;
	bool	isLoop;
	std::string	crntSeqName;


public:
	void init(GAFObject * s_gaf);
	//void onInitPos(float dt);
	void onInitPos();
	void updatePos();

	GAFObject*	getGAFObject();
	Vec3*	getInitDisplayPosition();
	Vec3*	getDisplayPosition();
	void	setDisplayPosition(float x, float y);
	void	setDisplayPositionX(float x);
	void	setDisplayPositionY(float y);

	void	moveX(float x);
	void	moveY(float y);
	void	moveXY(float x, float y);
	void	setLooped(bool looped, bool recursive);
	void	playSequence(const std::string& name, bool looped, bool resume /*= true*/);
	std::string&	getCurrentSequenceName();

	bool getIsOnGround();
	void setIsOnGround( bool flag);
		
	float getVerticalSpeed();
	void setVerticalSpeed(float f);
	void addVerticalSpeed(float f);

	float getHorizontalSpeed();
	void setHorizontalSpeed(float f);
	void addHorizontalSpeed(float f);

	bool getVisible();
	void setVisible(bool flag);
	bool checkContainsPoint(Point s_wrldPnt);
	bool checkContainsActor(Actor * s_actor);	//	actor 와의 충돌 여부 check.

	void setScaleTo( float scale );
	float getScaleTo();
	void setAnimationRunning(bool value, bool recursive);
	bool getIsAnimationRunning();
	cocos2d::Vec3	getWorldTransformDisplayPoint();
	cocos2d::Vec3 convertToLocalPoint(Vec3 s_wrldPnt);
	//cocos2d::Rect	getWorldTransformRect(GAFObject	* s_gafBox);	//	actor 내 gaf object bounding box의 world 좌표를 얻음.
};

#endif
