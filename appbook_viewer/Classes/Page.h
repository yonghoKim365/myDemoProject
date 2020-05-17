#pragma once
#include "cocos2d.h"
#include "ui/CocosGUI.h"
#include "ui/UIButton.h"

#include "SimpleAudioEngine.h"
#include "AudioEngine.h"
#include "KAlbumLayer.h"
#include "ViewUtils.h"
#include "KStringUtil.h"

#include "KCameraManager.h"
#include "CCNativeAlert.h"
#include <map>



//Command 부분.. 상수 정의

#define COMMAND_SETPAGE						"PAGE"
#define COMMAND_PREV						"PREV"
#define COMMAND_NEXT						"NEXT"
#define COMMAND_PREV_NOPAGING				"PREV_NOPAGING"
#define COMMAND_NEXT_NOPAGING				"NEXT_NOPAGING"
#define COMMAND_VOICE_SYSTEM				"VOICE_SYSTEM"
#define COMMAND_VOICE_USER					"VOICE_USER"

#define COMMAND_EXIT						"EXIT"
#define COMMAND_AUTO_PLAY					"LISTEN"
#define COMMAND_MANUAL_PLAY					"READ"
#define COMMAND_HIDE						"HIDE"
#define COMMAND_POPUP						"POPUP"
#define COMMAND_POPUPEXIT					"POPUPEXIT"
#define COMMAND_COVERPAGE					"COVERPAGE"
#define COMMAND_NEXTBOOK					"NEXTBOOK"
#define COMMAND_ALBUMPREV					"ALBUMPREV"
#define COMMAND_ALBUMNEXT					"ALBUMNEXT"

#define COMMAND_CAMERA						"CAMERA"
#define COMMAND_RECORD						"RECORD"
#define COMMAND_GOTO						"GOTO"

#define COMMAND_SOUNDON						"SOUNDON"
#define COMMAND_SOUNDOFF					"SOUNDOFF"
#define COMMAND_NULL						"COMMAND_NULL"


#define AUDIOENGINE_UNDEFINED				-11111

#define DATA_SAVE_FOLDER					".ctfolder/"

using namespace cocos2d;
using namespace ui;
using namespace std;

/*
	//특정부분 뽑아내기..
	string sValue = mapTable["kimdo1"];
	if (sValue == "")	cout << "can't found.." << endl;
	cout << "foud... " << sValue << endl;

	//전체 뽑아내기.. ㅋㅋ.
	map<string, string>::iterator iter;
	for (iter = mapTable.begin(); iter != mapTable.end(); iter++) {
		cout << "iter->first" << iter->first << "  iter->second" << iter->second << endl;
	}
*/

struct STBOOK_INFO {
	int version;
	string projectcode;
	string title;
	int width;
	int height;
	string backgroundImage;
	string backgroundSound;
	string transfereffect;
	string makeuser;
	string makedate;
	string modifydate;
	string screenType;
	string backgroundColor;
	map<string, string> mapCommon;
};


struct STBACKGROUND {
	string image;
	string color;
	float opacity;
	float width;
	float height;
};


struct STANIMATION : public Ref{
	string image;
	int width;
	int height;
	STANIMATION() {
		autorelease();
	}
	~STANIMATION() {
		//log("--------------------STANIMATION");
	}
};

struct STCONTENT_INFO;

struct ACTION_PARAM : public Ref{
	string name;
	string param;
	string param2;
	STCONTENT_INFO* pContentInfo;
	Layer* target;
	Sprite* sprite;
	~ACTION_PARAM() {
		//log("~~~~ACTION_PARAM name=%s , param=%s", name.c_str(), param.c_str());
	}
};

struct STCONTENT_INFO : Ref {
	string type;
	string image;
	string selected;
	string disabled;
	float x;
	float y;
	float width;
	float height;
	ACTION_PARAM action;
	string id;
	Vector<STANIMATION *> vtAnimation;
	string text;
	string fontsize;
	string fontname;
	string fontcolor;
	string ptype;
	string delay;
	string dx;
	string dy;
	string time;
	string visible;
	string sound;
	string group;
	string sort;
	string depth;
	string aniloop;
	string soundloop;
	string normaltype;
	/* rotate , scale 추가 */
	string rotate;
	string rotateloop;
	string rotatedu;
	string rotatean;
	string scale;
	string scaleloop;
	string scaledu;
	string scaless;
	/* 20160405  Text OutLine Insert  */
	string fontfile;
	string outline;
	string outlinecolor;
	string outlinetick;
	/* 20160421 GOTO */
	string gotopage;
	string playingcolor;
	string startani;
	string alpha;


	STCONTENT_INFO() {
		autorelease();
	}
};

struct HPOPUP_INFO : public Ref{
	string id;
	STBACKGROUND stBackground;
	Vector<STCONTENT_INFO *> vtContents;
	string albumthumby;
	string albumthumbsrc;
	string albumthumbsrc2;
	HPOPUP_INFO() {
		autorelease();
	}
	~HPOPUP_INFO() {
		//log("~HPOPUP_INFO is called");
	}
};



struct HPAGE_INFO : public Ref{
	ACTION_PARAM loadAction;
	STBACKGROUND stBackground;
	Vector<STCONTENT_INFO *> vtContents;
	ACTION_PARAM swipeLeft;
	ACTION_PARAM swipeRight;
	string backgroundMusic;	
	string type;
	string pageno;
	string thumbnailImage;
	string narration;
	string ptype;
	HPAGE_INFO() {
		autorelease();
	}
	~HPAGE_INFO() {
		//log("~HPAGE_INFO IS CALLED");
	}
};


/* 페이지 나레이션 리스트 */
struct HVOICE_INFO : public Ref{
	string sort;
	string sound;
	string id;
	string playingcolor;
	~HVOICE_INFO() {
		//log("~HPAGE_INFO IS CALLED");
	}
};

bool sortByX(const Ref* obs1, const Ref* obs2);

class CPage : public Scene, public KCameraManagerDelegate
{
public:
	static	CPage*		create(HPAGE_INFO* info, STBOOK_INFO * pBookInfo);

	CPage();
	~CPage();

private:
	bool init(HPAGE_INFO* info, STBOOK_INFO * pBookInfo);
	bool initEndPage();

private:
	Layer*				mBackground;
	Layer*				mContent;
	Layer*				mUIObject;
	Layer**				mPopup;
	KAlbumLayer*		mAlbum;
	bool				mIsPopup;
	set <string>		mSetPopupIDs;
	Vector<Ref *>		vtRefCountManager;
	Vector<Ref *>		vtVoiceManager;
	CPage*				myPage;
	int					mPopupLength;
	int					mPageLength;
	int					mCurPage;
	int					mPlayNarrationPos;
	float				mTimerDuratin;
	int					mJTPpopupIndex;

	HPAGE_INFO  *		m_pPageInfo;
	STBOOK_INFO *		m_pBookInfo;
	string				m_userPlaysound;
	bool				m_isUserSound;
	//std::string			misFindshed;
	STCONTENT_INFO *    mpContentInfo;  // action sound 있을시 임시저장용
	STCONTENT_INFO *    mpJtpButtonInfo;  // jump to page button info  임시저장용
	STCONTENT_INFO *    mpExitButtonInfo;  // exit button info,  임시저장용
	


	Vector<STCONTENT_INFO *> vtAni;
	//향후에 사용이 될 거.. 왼쪽 오른쪽 swiping..
	ACTION_PARAM		mSwipeLeft;
	ACTION_PARAM		mSwipeRight;

	Button*				mBtn;
	Sprite*				m_pBlockingSprite;
	EventListenerTouchOneByOne * pEventListenerBack;
	Layer *				mBlankLayers;
	Button*				mSoundOn;
	Button*				mSoundOff;
public:
	Button*				mBtnPrevPageInJtp;
	Button*				mBtnNextPageInJtp;
private:
	// swipe
	bool				onTouchBegan(Touch *touch, Event *event);
	void				onTouchMoved(Touch *touch, Event *event);
	void				onTouchEnded(Touch *touch, Event *event);
	void				onTouchCancelled(Touch *touch, Event *event);
	Size				mVisibleSize;
	bool				mIsTouchDown;

	Vec2				mInitialTouchPos;
	Vec2				mCurrentTouchPos;
	void				update(float dt);
public:
	static void			setBackground(STBACKGROUND&, Layer*);
	static Color3B		convertRGB(const char*);
	static Sprite*		appendImage(STCONTENT_INFO*, Layer*);
	static void			appendText(STCONTENT_INFO*, Layer*);
	void				command(string, string, string = "", Ref * = nullptr);
	int					getAudioEnginID();
	void				setAudioEnginID(int _id);
	int					playSound(std::string sFile);
	int					playSound(std::string sFile, bool bLoop);
	void				stopPrevAnimation();
private:

	void				setBgm(string);
	void				playBGM();
	void				stopBGM();
	bool				isBgmPlaying();
	void				appendButton(STCONTENT_INFO* , Layer*);
	void				appendButtonToBlankSprite(STCONTENT_INFO*);
	
	
	void				appendAnimation(STCONTENT_INFO* , Layer*,bool bplay=true);
	void				appendAlbum(HPOPUP_INFO*, Layer*);
	void				appendTextInput(STCONTENT_INFO*, Layer*);
	void				appendParticle(STCONTENT_INFO*, Layer*);
	void                addSoundEvent(STCONTENT_INFO*, Layer*);
	void                appendAnimationEvent(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite);
	void				playAnimation(STCONTENT_INFO* pContentInfo, Layer* target, Sprite* sprite,bool bplay=true,bool bforceshow=false);
	
	void				showPopup(string param);
	void				playNarrationList(STCONTENT_INFO *  pContentInfo);
	void				setTextOrg();
	void				setTextOrgColor(string id);
	void				setTextOrgColorReal(string id, string orgColor);
	
private:
	Node *				getChildByNameForLocal(const string&  sName);
	void				callAfterOneSecond(float dt);
	void				callAfterAniStart(float dt);
	void				playNarrationList();
	string				getNarrationPostion();
	//void				callAfterOneSecondSound(float dt);
	int					mAudioEngineID;
	int					mNarrationAudioID;
	void				generateParticleEffect(string stype, Point pt, float duration = 0.3f);
	int					getPopupIndexFromTargetID(string id);
	bool                isSpriteTransparentInPoint(Sprite* sprite, Point& point);
//	bool				sortByX(const HVOICE_INFO* obs1, const HVOICE_INFO* obs2);
	void				searchObject(string id, string playingcolor);
	void				appendButtonAlbum(float x1, float y1, float width1, float height1,
							std::string orgimg, std::string selimg, std::string disimg, std::string action, Layer* target);
	void				appendBlankSprite();
	void                continueCheck();
	
public:
	void                guidePopupCheck();
	void				refreshPrevNextBtnInJtp();

public:
	void onRefreshUI(std::string filename);
	void afterSeconds(float dt);
	void AlertCallback(int tag, NativeAlert::ButtonType type);

	
private:
	std::string			mCameraFilename;
	bool				mIsCameraActionOn;

	//Vector<Sprite*>		aniSprList;
	Sprite*				prevSoundAniSpr;
	void				clearAllSoundAniEventObj();

	void				playBgmAndNarrationAndStartAni();
public:
	string				lastCommandOfButton;

	Sprite*				lastAniSpr;

	void				setEBookGUidePopupReadCheck();
	
};

