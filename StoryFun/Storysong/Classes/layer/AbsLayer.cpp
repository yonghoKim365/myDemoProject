#include "AbsLayer.h"

Button * AbsLayer::createSimpleButton(std::string normalPath, std::string pressedPath)
{
	return Button::create(normalPath, pressedPath, pressedPath);
}