#ifndef _EXAMPLE_ACTOR_H_
#define _EXAMPLE_ACTOR_H_

#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "common/AssetUtils.h"
#include "data/def.h"
#include "data/JsonInfo.h"


USING_NS_CC;
USING_NS_GAF;

class ExampleActor : public Ref
{
public:
	static ExampleActor * create(GAFObject * obj, int index);

public:
	void startIntroAnimation();
	virtual void startTouchAnimation();
	virtual void addExample(int index, int frame);
	virtual void hideExample(int frame);
	int	 getIndexId();

protected:
	virtual bool init(GAFObject * obj, int index);
	virtual void onFinishedTouchAnimation(GAFObject * obj);
	virtual void onFinishedExitAnimation(GAFObject * obj);
	virtual void onFinishedExampleAnimation(GAFObject * obj);

protected:
	JsonInfo *		jsonInfo;

	std::string		oldExampleName;

	int				exampleIndex;
	int				EXAMPLE_WIDTH = 300;
	int				EXAMPLE_HEIGHT = 300;
	bool			isAddedExample;
	bool			isPlayedExample;
	bool			isFadeRun;

	FadeOut	  *		fadeAction;

	GAFObject *		exampleAnimation;
	GAFObject *		contentsHolder;
	GAFObject *		exampleObject;
};

class ExampleActorBall : public ExampleActor
{
public:
	static ExampleActorBall * create(GAFObject * obj, int index);

public:
	void addExample(int index, int frame) override;
	void hideExample(int frame) override;
	void startTouchAnimation() override;

protected:
	bool init(GAFObject * obj, int index) override;
	void onFinishedTouchAnimation(GAFObject * obj) override;
	void onFinishedExitAnimation(GAFObject * obj) override;
	void onFinishedExampleAnimation(GAFObject * obj) override;

private:
	void setRandomFrame(int frame);
	void onFrameResetEvent(void* params);
private:
	int currentFrame;
	int lastFrame = -1;

	GAFObject *		ballFront;
	GAFObject *		ballBack;
};
#endif