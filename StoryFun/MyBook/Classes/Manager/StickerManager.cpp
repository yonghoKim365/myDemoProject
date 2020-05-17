#include "Manager/StickerManager.h"
#include "Util/Pos.h"
#include "Contents/MyBookSnd.h"

/**
* @brief  스티커 관리자 생성자
* @param  Layout* pLayout
* @param  StickerManagerListnener* pListener
* @return void
*/
StickerManager::StickerManager(ui::Layout* pLayout, StickerManagerListnener* pListener) 
{
	mLayout   = pLayout;
	mListener = pListener;

	int stickerBaseIdx = STICKER::BALL; // base sticker
	
	////////////////////////////////////////
	// 스크롤 뷰
	mScrollView = dynamic_cast<ui::ScrollView*>(mLayout->getChildren().at(0));	
	int count = mScrollView->getChildrenCount();
	
	for (int i = 0; i < count; i++)
	{
		ui::Button* sticker = dynamic_cast<ui::Button*>(mScrollView->getChildren().at(i));
		sticker->setTag(stickerBaseIdx + i);
		sticker->addTouchEventListener(CC_CALLBACK_2(StickerManager::onTouchSticker, this));	
		sticker->setSwallowTouches(false);		
	} // end of for			
	
	// 저장하기
	ui::Button* btn = dynamic_cast<ui::Button*>(mLayout->getChildren().at(1));
	btn->addTouchEventListener(CC_CALLBACK_2(StickerManager::onTouchSave, this));
	btn->setEnabled(false);

	// 스크롤 좌 버튼
	mBtnLeft = dynamic_cast<ui::Button*>(mLayout->getChildren().at(2));
	mBtnLeft->addTouchEventListener(CC_CALLBACK_2(StickerManager::onTouchScrollLeft, this));
	// 스크롤 우 버튼
	mBtnRight = dynamic_cast<ui::Button*>(mLayout->getChildren().at(3));	
	mBtnRight->addTouchEventListener(CC_CALLBACK_2(StickerManager::onTouchScrollRight, this));

	mBtns.push_back(btn);
	mBtns.push_back(mBtnLeft);
	mBtns.push_back(mBtnRight);

	// 왼쪽 버튼 먼저 보이기, 오른쪽 버튼 숨기기
	//setEnableForLeftBtn(true);

	showy   = mLayout->getPosition().y;
	hidey   = -mLayout->getBoundingBox().size.height;

	mLayout->setPosition(Point(mLayout->getPosition().x, hidey));

	// 오른쪽 버튼 먼저 보이기로 수정
	//CCLOG("[STICKER] onTouchScrollLeft......[ENDED]");
	mScrollView->setPosition(Pos::getScrollViewWithRightBtnPt());
	mScrollView->scrollToPercentHorizontal(100.0f, 0.01, true);
	setEnableForLeftBtn(false);

	//CCLOG("STICKER MGR COMPLETE.....");
}


StickerManager::~StickerManager() 
{
	CCLOG("Destory StickerManager......");
}

/**
* @brief  보이기/숨기기
* @param  b
* @return void
*/
void StickerManager::setShow(bool b) 
{
	if (b)
	{
		// 보이기
		mLayout->setVisible(true);
	}
	else
	{
		// 숨기기
		mLayout->setVisible(false);
	}
}


/**
* @brief  스티커 선택
* @param  Ref* pSender
* @param  ui::Widget::TouchEventType type
* @return void
*/
void StickerManager::onTouchSticker(Ref* pSender, ui::Widget::TouchEventType type) 
{		
	//CCLOG("[STICKER] .......... START...........");
	ui::Button* sticker = (ui::Button*) pSender;
	mStickerId = sticker->getTag();	

	switch (type)
	{
		case ui::Widget::TouchEventType::BEGAN:	
			CCLOG("[STICKER] onTouchSticker......[BEGAN][%d]", mStickerId);			
			if (mIsMoveInitPositionSticker == false)
			{
				mListener->onSelectSticker(pSender);
			}
			break;

		/*case ui::Widget::TouchEventType::MOVED:			
			break;*/

		case ui::Widget::TouchEventType::ENDED:		
			CCLOG("[STICKER] onTouchSticker......[END][%d]", mStickerId);		
			// 스티커 드래그 완료
			mStickerId = INVALID_INDEX;			
			break;

		case ui::Widget::TouchEventType::CANCELED:		
			CCLOG("[STICKER] onTouchSticker......[CANCELED]");			
			// 스티커 드래그 완료
			//mStickerId = INVALID_INDEX;
			break;
	} // end of switch
}


/**
* @brief  저장하기
* @param  Ref* pSender
* @param  ui::Widget::TouchEventType type
* @return void
*/
void StickerManager::onTouchSave(Ref* pSender, ui::Widget::TouchEventType type)
{
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		CCLOG("[STICKER] touch save.....");
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
		ui::Button* btn = (ui::Button*)pSender;
		if (btn != nullptr)
		{
			btn->setEnabled(false);					
		}
		// 눌림 상태로 변경
		mListener->onSave();
	}
}

/**
* @brief  스크롤 왼쪽버튼 클릭 
* @param  Ref* pSender
* @param  ui::Widget::TouchEventType type
* @return void
*/
void StickerManager::onTouchScrollLeft(Ref* pSender, ui::Widget::TouchEventType type)
{
	
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		CCLOG("[STICKER] onTouchScrollLeft......[ENDED]");	
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);

		mScrollView->setPosition(Pos::getScrollViewWithRightBtnPt());
		mScrollView->scrollToPercentHorizontal(100.0f, SCROLL_TIME, true);
		setEnableForLeftBtn(false);
	}
}

/**
* @brief  스크롤 오른쪽 버튼 클릭 
* @param  Ref* pSender
* @param  ui::Widget::TouchEventType type
* @return void
*/
void StickerManager::onTouchScrollRight(Ref* pSender, ui::Widget::TouchEventType type)
{	
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		CCLOG("[STICKER] onTouchScrollRight......[ENDED]");		
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_BTN_CLICK);
		mScrollView->setPosition(Pos::getScrollViewWithLeftBtnPt());
		mScrollView->scrollToPercentHorizontal(0.0f, SCROLL_TIME, true);
		// 버튼 숨기기 / 보이기
		// 오른쪽 버튼 숨기기, 왼쪽 버튼 보이기
		setEnableForLeftBtn(true);
	}
}

// 왼쪽 우선/ 
void StickerManager::setEnableForLeftBtn(bool b)
{
	mBtnLeft->setEnabled(b);
	mBtnLeft->setVisible(b);
	b = !b;
	mBtnRight->setEnabled(b);
	mBtnRight->setVisible(b);
}

//------------------------------------------------------------
// public method
//------------------------------------------------------------
ui::Layout* StickerManager::getLayout()
{
	return mLayout;
}

ui::ScrollView*  StickerManager::getScrollView()
{
	return mScrollView;
}

void StickerManager::setSticker()
{
	CCLOG("setSticker ID[%d]", mStickerId);
}