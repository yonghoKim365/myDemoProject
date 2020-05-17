#pragma once

#ifndef __KALBUM_LAYER_H__
#define __KALBUM_LAYER_H__

#include "cocos2d.h"
#include "ViewUtils.h"
#include "ui/CocosGUI.h"
#include "ui/UIButton.h"
#include "KSpriteButton.h"

class CPage;
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
#include "jni.h"
#include "jni/JniHelper.h"
#endif

using namespace cocos2d;
using namespace std;
using namespace ui;

#define MAX_CONTENT 9

class KAlbumLayer : public Node
{
public:
	static KAlbumLayer* create(string normal, string selected, Size dotSize, float dotY,  Layer* target, int curPage);
	KAlbumLayer();
	KAlbumLayer(string normal, string selected, Size dotSize, float dotY, Layer* target, int curPage);
	~KAlbumLayer();
private:
	Vector<KSpriteButton *> mVectorPage;
	int			mCurrentPageInPreview;
	int			mCurrentPageInBook;
	Size		mDotSize;
	float		mDotY;
	string		mDotNormal;
	string		mDotSelected;
	Vector<Sprite *> mVectorDot;
	int			mTotalItemCount;
	Node *		mpDotParent;
	Sprite * m_pBlockingSprite;
	EventListenerTouchOneByOne * pEventListerBlock;


	bool				mIsTouchDown;
	Size				mVisibleSize;
	Vec2				mInitialTouchPos;
	Vec2				mCurrentTouchPos;
	void				update(float dt);
	//bool				bJtpWarningPopupOpen;
	void				setJtpWarningOff();

public:
	void addContent(string img,bool bDim=false);
	void next();
	void prev();
	int selected();
	void setBlocked(bool bflag);
	int					getTotalPage();
	int					getCurrentPage();
	void				refreshPosition();
	void				setJtpPageToCurrentPage(int curPage);

	Vector<Ref *>		vtRefCountManager;
	void				appendButtonAlbum(float x1, float y1, float width1, float height1,
		std::string orgimg, std::string selimg, std::string disimg, std::string action);

	void				setParentPage(CPage* _page);
	CPage*				mParentPage;

	Button*				mSoundOn;
	Button*				mSoundOff;
	//Button*				mBtnPrevPageInJtp;
	//Button*				mBtnNextPageInJtp;

private:
	void refreshDot();
	Sprite* appendImage(float x, float y, float width, float height, std::string file, int depth);
	void showToastForJNI(std::string sMessage);
	
	// swipe
	bool				onTouchBegan(Touch *touch, Event *event);
	void				onTouchMoved(Touch *touch, Event *event);
	void				onTouchEnded(Touch *touch, Event *event);
	void				onTouchCancelled(Touch *touch, Event *event);

	void				playSound(std::string sFile);

};

#endif