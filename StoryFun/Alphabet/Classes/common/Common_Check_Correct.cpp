
#include "Common_Check_Correct.h"
#include "MGTResourceUtils.h"
#include "MGTSprite.h"

Common_Check_Correct::Common_Check_Correct()
{
    
}

Common_Check_Correct::~Common_Check_Correct()
{
    
}

Common_Check_Correct* Common_Check_Correct::create(bool isCorrect)
{
    Common_Check_Correct* ret = new (std::nothrow) Common_Check_Correct();
    if (ret && ret->initWithCorrect(isCorrect))
    {
        ret->autorelease();
    }
    else
    {
        CC_SAFE_DELETE(ret);
    }
    return ret;
}

bool Common_Check_Correct::initWithCorrect(bool isCorrect)
{
    if (!Node::init()) {
        return false;
    }
    
    if(isCorrect)
    {
        m_asset = GAFAsset::create(MGTResourceUtils::getInstance()->getFilePath(ResourceType::GAF, "flash", "e_b001_day4_ta01_check_o.gaf"), nullptr);
    }
    else
    {
        m_asset = GAFAsset::create(MGTResourceUtils::getInstance()->getFilePath(ResourceType::GAF, "flash", "e_b001_day4_ta01_check_x.gaf"), nullptr);
    }
    
    show();
    
    return true;

}

bool Common_Check_Correct::init()
{
    if (!Node::init()) {
        return false;
    }
//    m_asset = GAFAsset::create(MGTUtils::getInstance()->getFilePath("flash", "e_b001_day4_ta01_check_o.gaf"), nullptr);
    return true;
}


void Common_Check_Correct::onEnter()
{
    Node::onEnter();
}
void Common_Check_Correct::onExit()
{
    Node::onExit();
}

void Common_Check_Correct::playGuide()
{
    m_mainObject->start();
}

void Common_Check_Correct::stopGuide()
{
    m_mainObject->stop();
}

void Common_Check_Correct::show()
{
    m_mainObject = m_asset->createObjectAndRun(true);
    m_mainObject->setPosition(Vec2(0,110));
    this->addChild(m_mainObject);

    m_mainObject->playSequence("action", false);
    m_mainObject->setSequenceDelegate(std::bind(&Common_Check_Correct::onFinishSequence, this, std::placeholders::_1, std::placeholders::_2));
}

void Common_Check_Correct::loop()
{
    m_mainObject->playSequence("loop", true);
}


void Common_Check_Correct::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    if (sequenceName == "action")
    {
        m_mainObject->setSequenceDelegate(nullptr);
//        loop();
    }
}
