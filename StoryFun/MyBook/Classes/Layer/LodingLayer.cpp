#include "LoadingLayer.h"
#include "Contents/MyBookSnd.h"
#include "CCFileUtils.h"

// for AudioEngine
using namespace cocos2d::experimental;


LoadingLayer* LoadingLayer::createLayer(Node* parent, int weekData, int ord)
{
	// 'layer' is an autorelease object
	auto layer = LoadingLayer::create();
	layer->mParent = parent;
	layer->mWeekData = weekData;
	layer->mOrderNum = ord;

	log("[LOADING]initLayout [%d, %d]", layer->mWeekData, layer->mOrderNum);
	layer->initLayout();	
	log("[LOADING]createAnimation ");
	//layer->createAnimation();
	log("[LOADING]initLayout .... createLayout");
	// return the scene
	return layer;
}


// on "init" you need to initialize your instance
bool LoadingLayer::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Layer::init())
	{
		return false;
	}	
	
	return true;
}


void LoadingLayer::initLayout()
{

	// 기준 Sprite
	if (mBaseSp == nullptr)
	{
		mBaseSp = Sprite::create();
		mBaseSp->retain();
	}

	// Dim 처리
	LayerColor *bgLayer = LayerColor::create(Color4B(0, 0, 0, 179));
	mBaseSp->addChild(bgLayer);	


	/*mSpAnimate = Sprite::create(StringUtils::format(FILENAME_ANI_PRELOAD_X, 1));
	mSpAnimate->setPosition(Pos::getPreloadingPt());
	mSpAnimate->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);	
	mBaseSp->addChild(mSpAnimate);*/

	// bg	
	auto bg = Sprite::create(FILENAME_POPUP_BG);
	bg->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	bg->setPosition(Pos::getPopupPt());
	mBaseSp->addChild(bg);

	// yoonie 	
	auto charYoonie = Sprite::create(StringUtils::format("%s", FILENAME_POPUP_OPTION_YOONIE).c_str());
	charYoonie->setAnchorPoint(Vec2::ANCHOR_TOP_LEFT);
	charYoonie->setPosition(Pos::getPopupYooniePt());
	mBaseSp->addChild(charYoonie);
	
	this->addChild(mBaseSp);
}

/*
void LoadingLayer::createAnimation()
{
	CCLOG("createAnimation........");
	//CCAnimation 만들기
	Animation* animation = Animation::create();

	//직접 추가하기 그림 자원
	for (int i = 1; i <= ANI_PRELOAD_MAX_INDEX; i++)
	{
		animation->addSpriteFrameWithFileName(StringUtils::format(FILENAME_ANI_PRELOAD_X, i)); //애니메이션 프레임 추가
	} // end of for
	
	//설정
	animation->setRestoreOriginalFrame(true); //환원 첫 프레임
	animation->setDelayPerUnit(3.0f / ANI_PRELOAD_MAX_INDEX);     //단위 프레임 간격
	animation->setLoops(-1);                  //-1 무한 사이클
	
	//Animate 만들기
	mAnimate = Animate::create(animation);
	mAnimate->retain();
	
}
*/

void LoadingLayer::show(bool b)
{
	CCLOG("[LOADING]show...: %d", b);
	mBaseSp->setVisible(b);

	if (b)
	{
		//mParent->addChild(this);
		// play
		//mSpAnimate->runAction(mAnimate);
	}
	else
	{
		// stop
		//mSpAnimate->stopAction(mAnimate);

		//mParent->removeChild(this);
	}
	CCLOG("[LOADING]show..2.: %d", b);
}