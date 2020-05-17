#ifndef __KBLANKLAYER_LAYER_H__
#define __KBLANKLAYER_LAYER_H__

#include "cocos2d.h"
#include "ViewUtils.h"
#include "ui/CocosGUI.h"
#include "ui/UIButton.h"
#include "KSpriteButton.h"
#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
#include "jni.h"
#include "jni/JniHelper.h"
#endif

using namespace cocos2d;
using namespace std;
using namespace ui;

class KBlankLayer : public cocos2d::LayerColor
{
public:
	static KBlankLayer* createLayer(cocos2d::Color4B color4B = cocos2d::Color4B(255, 255, 255, 0));
	bool init(cocos2d::Color4B color4B);

	KBlankLayer();

	~KBlankLayer();

	ui::Button* mBtn;
	Sprite * m_pBlockingSprite;
	EventListenerTouchOneByOne * pEventListenerBack;
	

};

#endif
