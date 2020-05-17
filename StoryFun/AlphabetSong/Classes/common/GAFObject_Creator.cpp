//
//  GAFObject_Creator.cpp
//  MGT_Template
//
//
//
//

#include "GAFObject_Creator.h"

GAFObject_Creator::GAFObject_Creator()
{
    
}

GAFObject_Creator::~GAFObject_Creator()
{
    
}

GAFObject_Creator* GAFObject_Creator::create(std::string path, Vec2 pos)
{
    GAFObject_Creator* ret = new (std::nothrow) GAFObject_Creator();
    if (ret && ret->initWithPath(path, pos))
    {
        ret->autorelease();
    }
    else
    {
        CC_SAFE_DELETE(ret);
    }
    return ret;
}

bool GAFObject_Creator::initWithPath(std::string path, Vec2 pos)
{
    if (!Node::init()) {
        return false;
    }
    
    m_asset = GAFAsset::create(path, nullptr);
    m_mainObject = m_asset->createObject();
    m_mainObject->setPosition(pos);
    this->addChild(m_mainObject);

    return true;
}

bool GAFObject_Creator::init()
{
    if (!Node::init()) {
        return false;
    }
    
    return true;
}


void GAFObject_Creator::onEnter()
{
    Node::onEnter();
}
void GAFObject_Creator::onExit()
{
    Node::onExit();
}

GAFObject* GAFObject_Creator::getMainObject()
{
    return m_mainObject;
}

void GAFObject_Creator::start(bool isLoop)
{
    m_mainObject->setLooped(isLoop);
    m_mainObject->start();
}

