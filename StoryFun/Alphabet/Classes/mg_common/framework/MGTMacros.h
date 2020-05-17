//
//  MGTDefine.h
//  MGT_Template
//
// on 13. 6. 10..
//
//

#ifndef e_MGTMacros_h
#define e_MGTMacros_h

#include "cocos2d.h"
using namespace cocos2d;

//==================
#pragma mark - color
#define COLOR_BACKGROUND_WHITE              Color4B(255, 255, 255, 255)
#define COLOR_BACKGROUND_BALCK              Color4B(0, 0, 0, 255)
#define COLOR_BACKGROUND_DIMMED             Color4B(0, 0, 0, 255*0.85f)
#define COLOR_BACKGROUND_TRANSPARENT        Color4B(0, 0, 0, 0)

//==================
#pragma mark - frame
//#define x(a) a->getPositionX()
//#define y(a) a->getPositionY()
//#define width(a) a->getContentSize().width
//#define height(a) a->getContentSize().height

//==================

#define SCENE_FUNC(v, classType)\
do {\
v = classType::createScene();\
}\
while(false)

#define replaceSceneNoTransition(classType)\
do {\
Scene *scene = Scene::create();\
classType *layer = classType::create();\
scene->addChild(layer);\
Director::getInstance()->replaceScene(scene);\
}\
while(false)


#define replaceSceneTransitionFadeOut(classType)\
do {\
\
Scene *scene = Scene::create();\
classType *layer = classType::create();\
scene->addChild(layer);\
Director::getInstance()->replaceScene(TransitionFade::create(0.5, scene, Color3B(0, 0, 0)));\
}\
while(false)

#define runSceneMacro(classType)\
do {\
Scene *scene = Scene::create();\
classType *layer = classType::create();\
scene->addChild(layer);\
Director::getInstance()->runWithScene(scene);\
}\
while(false)



#endif
