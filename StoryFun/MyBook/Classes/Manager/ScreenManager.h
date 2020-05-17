#ifndef __SCREEN_MANAGER_H__
#define __SCREEN_MANAGER_H__

#include "cocos2d.h"

USING_NS_CC;

/**
* @brief 녹음하기 관리자
*/
class ScreenManager : public Ref
{
public:
	// single ton
	static ScreenManager *getInstance();

	////////////////////////////////////////
	// operation	
	void screenOn(Node* parent);
	void screenOff(Node* parent);

	// Jni Callback
	static void infromScreenManager();

	ScreenManager();
	virtual ~ScreenManager();
	

protected:	
	virtual bool init();

private:
	static ScreenManager* _screenManager;
	Node* mParent;	
	void scheduleForScreenOff(float delta);
};

#endif //__SCREEN_MANAGER_H__
