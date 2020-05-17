
#include "Preload.h"
#include "ProductManager.h"
#include "MGTMacros.h"
#include "ProductIndex.h"


Preload::Preload()
{
}

Preload::~Preload()
{
}


Scene *Preload::createScene()
{
    auto scene = Scene::create();
    auto layer = Preload::create();
    scene->addChild(layer);
    return scene;
}
bool Preload::init()
{
    if (!LayerColor::init()) {
        return false;
    }
    
    return true;
}

void Preload::onEnter()
{
    LayerColor::onEnter();
    
    auto winSize = Director::getInstance()->getWinSize();

    this->runAction(Sequence::create(DelayTime::create(0.2f),
                                     CallFunc::create( CC_CALLBACK_0(Preload::gotoContent,this)),
                                     nullptr));
}

void Preload::onExit()
{
    LayerColor::onExit();
}

void Preload::gotoContent()
{
    Director::getInstance()->replaceScene(ProductIndex::getContentScene());
}
