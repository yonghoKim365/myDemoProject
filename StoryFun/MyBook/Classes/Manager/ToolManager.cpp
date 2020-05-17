/**
 * @class	ToolManager
 * @brief	툴 관리 클래스
 * 
 * @author	
 * @date	2016. 4. 1
 */

#include "Manager/ToolManager.h"
#include "Contents/MyBookSnd.h"

// for AudioEngine
//using namespace cocos2d::experimental;

/**
 * @brief 툴 관리자 생성자
 * @param  pLayout
 * @param  pListener
 */
ToolManager::ToolManager(ui::Layout* pLayout, ToolManagerListener* pListener)
{
	mLayout = pLayout;
	mListener = pListener;

	mColors = std::vector<ui::Button*>();
	mBtns = std::vector<ui::Button*>();

	int count = mLayout->getChildrenCount(); 
	int colorBaseIdx = TOOL::ARPICOT; // base color

	//CCLOG("TOOL >>>> count : %d", count);	

	// 칼라 설정
	for (int i = 0; i < count - 2; i++)
	{
		ui::Button* btnColor = dynamic_cast<ui::Button*>(mLayout->getChildren().at(i));
		btnColor->setTag(colorBaseIdx + i);
		btnColor->addTouchEventListener(CC_CALLBACK_2(ToolManager::onColor, this));
		
		mColors.push_back(btnColor);
		//CCLOG("[TOOL] Color index : %d", colorBaseIdx + i);		
	} // end of for

	// 이벤트 설정 
	// Clear & All Clear 이벤트 설정
	ui::Button* btnClear = dynamic_cast<ui::Button*>(mLayout->getChildren().at(count - 2));	
	btnClear->addTouchEventListener(CC_CALLBACK_2(ToolManager::onClear, this));

	ui::Button* btnAllClear = dynamic_cast<ui::Button*>(mLayout->getChildren().at(count - 1));
	btnAllClear->addTouchEventListener(CC_CALLBACK_2(ToolManager::onAllClear, this));

	mBtns.push_back(btnClear);
	mBtns.push_back(btnAllClear);

	///////////////////////////////////////////
	// 선택 이미지 설정
	// 최초 숨기기
	mSpColorSelect = Sprite::create(FILENAME_DRAW_TOOL_COLOR_SELECT);
	mSpColorSelect->retain();
	mSpColorSelect->setPosition(Vec2::ZERO);
	mSpColorSelect->setVisible(false);
	mLayout->addChild(mSpColorSelect, 1);
	///////////////////////////////////////////
	
	// 스티커 설정
	//ui::Widget* btnSticker = dynamic_cast<ui::Widget*>(mLayout->getChildByName(BTN_STICKER));
	//btnSticker->addTouchEventListener(CC_CALLBACK_2(ToolManager::onSticker, this));

	// 저장
	//ui::Widget* btnPlay = dynamic_cast<ui::Widget*>(mLayout->getChildByName(BTN_DRAW_DONE));
	//btnPlay->addTouchEventListener(CC_CALLBACK_2(ToolManager::onDone, this));

	// 휴지통(되돌리기)
	//ui::Widget* btnTrash = dynamic_cast<ui::Widget*>(control->getChildByName("btnTrash"));
	//btnTrash->addTouchEventListener(CC_CALLBACK_2(ToolManager::onUndo, this));

	mShowY = mLayout->getPosition().y;
	mHideY = -mLayout->getBoundingBox().size.height;
	mSelectColor = TOOL::COLOR::RED;

	mLayout->setPosition(Vec2(mLayout->getPosition().x, mHideY));
}


/**
* @brief  선택 이미지 초기화
* @param b
*/
void ToolManager::resetColorTool()
{
	mSpColorSelect->setVisible(false);
	
}

/**
* @brief  툴 보이기/숨기기
* @param b
*/
void ToolManager::setShow(bool b)
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
* @brief  칼라 설정
* @param  color  설정된 칼라
* @return void
*/
void ToolManager::setColor(TOOL::COLOR color)
{
	mSelectColor = color;
	CCLOG("[TOOL] SETCOLOR : %d", color);
	// 선택된 칼라색 위치 설정
	/*for (ui::Button* btn : mColors)
	{
		if (btn->getTag() == mSelectColor)
		{
			btn->setPosition(Vec2(btn->getPosition().x, mColorSelectY));
		}
		else
		{
			btn->setPosition(Vec2(btn->getPosition().x, mColorHideY));
		}
	}*/ // end of for
}

/**
 * @brief   크레파스 색상 선택
 * @param   Ref* pSender 
 * @param   Ref* type
 * @return  void
 */
void ToolManager::onColor(Ref* pSender, ui::Widget::TouchEventType type)
{
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_DRAW_COLOR);

		ui::Button* btn = (ui::Button*) pSender;		
		mSpColorSelect->setPosition(btn->getPosition());
		mSpColorSelect->setVisible(true);
		CCLOG("[onColor] getTag : %d", btn->getTag());

		// 기존의 지우개 버튼 초기화	
		ui::Button* btnClear = dynamic_cast<ui::Button*>(mBtns.at(0));
		btnClear->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_CLEAR_N);
		ui::Button* btnAllClear = dynamic_cast<ui::Button*>(mBtns.at(1));
		btnAllClear->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_N);

		mListener->onColor((TOOL::COLOR) btn->getTag());
		setColor((TOOL::COLOR) btn->getTag());		
	}
}


/**
* @brief   스티커 버튼 클릭
* @param   Ref* pSender
* @param   Ref* type
* @return  void
*/
//void ToolManager::onSticker(Ref* pSender, ui::Widget::TouchEventType type)
//{
//	
//}

/**
* @brief   되돌리기 버튼 클릭
* @param   Ref* pSender
* @param   Ref* type
* @return  void
*/
void ToolManager::onUndo(Ref* pSender, ui::Widget::TouchEventType type)
{

}

/**
* @brief   완료 버튼 클릭
* @param   Ref* pSender
* @param   Ref* type
* @return  void
*/
void ToolManager::onDone(Ref* pSender, ui::Widget::TouchEventType type)
{

}


/**
* @brief   지우기 버튼 클릭
* @param   Ref* pSender
* @param   Ref* type
* @return  void
*/
void ToolManager::onClear(Ref* pSender, ui::Widget::TouchEventType type)
{
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		//auto audio = SimpleAudioEngine::getInstance();
		//audio->playEffect(audio_effect_btn_crayons);
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_DRAW_COLOR);
		mSpColorSelect->setVisible(false);

		ui::Button* btnAllClear = dynamic_cast<ui::Button*>(mBtns.at(1));
		btnAllClear->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_N);

		ui::Button* btn = (ui::Button*) pSender;
		btn->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_CLEAR_S);

		mListener->onClear();
	}
}


/**
* @brief   모두 지우기 버튼 클릭
* @param   Ref* pSender
* @param   Ref* type
* @return  void
*/
void ToolManager::onAllClear(Ref* pSender, ui::Widget::TouchEventType type)
{
	if (type == ui::Widget::TouchEventType::ENDED)
	{
		//auto audio = SimpleAudioEngine::getInstance();
		//audio->playEffect(audio_effect_btn_crayons);
		// 효과음
		cocos2d::experimental::AudioEngine::play2d(FILENAME_SND_EFFECT_DRAW_COLOR);
		mSpColorSelect->setVisible(false);
		ui::Button* btnClear = dynamic_cast<ui::Button*>(mBtns.at(0));
		btnClear->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_CLEAR_N);

		ui::Button* btn = (ui::Button*) pSender;
		btn->loadTextureNormal(FILENAME_DRAW_TOOL_PANEL_ALL_CLEAR_S);
		mListener->onAllClear();
		//setColor((TOOL::COLOR) btn->getTag());
	}
}


cocos2d::ui::Layout* ToolManager::getLayout()
{
	return mLayout;
}
