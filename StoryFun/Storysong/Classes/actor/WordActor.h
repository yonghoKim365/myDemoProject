#ifndef _WORD_ACTOR_H_
#define _WORD_ACTOR_H_


#include "cocos2d.h"
#include "GAF/Library/Sources/GAF.h"
#include "actor/SimpleTouchActor.h"
//#include "actor/ExampleActor.h"
#include "common/AssetUtils.h"
#include "data/def.h"
#include "data/JsonInfo.h"


USING_NS_CC;
USING_NS_GAF;

class WordActorEventListener
{
public:
	virtual void onWordExampleExit(int id) = 0;
	virtual void onWordExampleExit2(int id) = 0;
};


class WordActor : public SimpleTouchActor
{
public:
	WordActor();
	~WordActor();

	static WordActor * create(gaf::GAFObject * object, int index, WordActorEventListener * _listener, bool visible = true);

public:
	bool init(gaf::GAFObject * object, int index, WordActorEventListener * _listener, bool visible);
	void startIntroAnimation();
	void startReadyAnimation();
	virtual void startBasicAnimation(bool autoReply = true);
	virtual void startTouchAnimation(bool autoReply = true);
	virtual void startExitAnimation(bool autoReply = true);
	void startExampleAnimation();
	void startExit2Animation(bool autoReply = true);
	void setFirstPlayValue(bool value);
	
	int getWordIndex();
	void setHide();
//	void setWordExamplePath(std::vector<std::string>exampleList);
	GAFObject * getGAFObject();
	void showWordExample(int index);
	void setLoadEventListener(PreLoadEventListener * listener);
	virtual void onExampleAnimationFinished();
	void hideExample();
	bool getFirstPlay();

private:
	void initData(std::string path, int index);
	void initUI();

	void onFinishedIntroAnimation(gaf::GAFObject * object);
	void onFinishedReadyAnimation(gaf::GAFObject * object);
	void onFinishedTouchAnimation(gaf::GAFObject * object);
	void onFinishedExitAnimation(gaf::GAFObject * object);
	
	void onFinishedExampleAnimation(gaf::GAFObject * object);
	void createExampleAndAdd(int index);
	//void preloadExample(std::vector<std::string>exampleList);
	//void onSpriteSheetLoad(Texture2D * texture);


private:
	WordActorEventListener * _eventListener;
	JsonInfo	* jsonInfo;

	int				wordIndex;
	int				exampleIndex;
	int				lastSoundId = -1;
	int				lastExplodeSoundId = -1;

	std::string		wordPath;
	GAFObject *		imageHolder;
	GAFObject *		exampleObject;
	bool			wordVisibleMode;
	bool			isFirstPlay;
	bool			isAddedExample;
	
	std::string					oldExampleName;
	//std::vector<std::string>	_examplePathList;
	std::vector<GAFObject*>		_exampleGafList;
	//std::vector<ExampleActor*>	_exampleActorList;
//	ExampleActor*				currentActor;
	PreLoadEventListener	*	eventListener;
	int				currentLoadedPath = 0;
	int				totlaExamplePath = 0;
	bool			isHaveExample;
	
	FadeOut * fadeAction;
	FadeOut * holderAction;
	bool		isFadeRun;
};

#endif