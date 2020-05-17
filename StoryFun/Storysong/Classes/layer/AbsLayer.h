#ifndef _ABS_LAYER_H_
#define _ABS_LAYER_H_

#include "cocos2d.h"
#include "ui/CocosGUI.h"

USING_NS_CC;

using namespace ui;

class AbsLayer : public Layer
{
public:
	Button * createSimpleButton(std::string normalPath, std::string pressedPath);

};


#endif