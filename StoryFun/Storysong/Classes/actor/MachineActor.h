#ifndef _MACHINE_ACTOR_H_
#define _MACHINE_ACTOR_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "actor/SimpleTouchActor.h"

class MachineActor : public SimpleTouchActor
{
public:
	static MachineActor* create(gaf::GAFObject* object, bool autoReply = true);
	MachineActor();
	~MachineActor();

public:
	virtual bool init(gaf::GAFObject* object, bool autoReply);
	void isFirstPlay(bool value);
	
	void stop();
	void play();

	void startReadyAnimation();
	virtual void startBasicAnimation(bool autoReply = true);
	virtual void startTouchAnimation(bool autoReply = true);
	

	void onFinishedReadySequence(gaf::GAFObject* object);
	virtual void onFinishedTouchSequence(gaf::GAFObject* object);
	
	void changeMotion();


};

#endif