
#include "Common_Intro_Top.h"
#include "PsdParser.h"
#include "MGTUtils.h"

////////////////////////////////////////////////////////
//
// Class initalize functions.
//
////////////////////////////////////////////////////////

enum
{
    kDepth_Bg = 0,
    kDepth_Character,
    kDepth_TextBox,
    kDepth_Text
};

Common_Intro_Top::Common_Intro_Top()
:
m_asset(nullptr)
{
    m_touchlistener = EventListenerTouchOneByOne::create();
    m_touchlistener->setSwallowTouches(true);
    
    m_touchlistener->onTouchBegan = CC_CALLBACK_2(cocos2d::Layer::onTouchBegan, this);
    m_touchlistener->onTouchMoved = CC_CALLBACK_2(cocos2d::Layer::onTouchMoved, this);
    m_touchlistener->onTouchEnded = CC_CALLBACK_2(cocos2d::Layer::onTouchEnded, this);
    
    _eventDispatcher->addEventListenerWithSceneGraphPriority(m_touchlistener, this);
    
//    addSoundObserver();
}


Common_Intro_Top::~Common_Intro_Top()
{
//    removeSoundObserver();
    
    _eventDispatcher->removeEventListener(m_touchlistener);
    
    CC_SAFE_RELEASE(m_asset);
}


Common_Intro_Top * Common_Intro_Top::create(common_intro_top::eCharacterType chaType, std::string textfile, std::string sndfile)
{
    Common_Intro_Top * pLayer = new Common_Intro_Top();
    if( pLayer && pLayer->initWithSetting(chaType, textfile, sndfile))
    {
        pLayer->autorelease();
        return pLayer;
    }
    CC_SAFE_DELETE(pLayer);
    return NULL;
}


bool Common_Intro_Top::initWithSetting(common_intro_top::eCharacterType chaType, std::string textfile, std::string sndfile)
{
    if (!MGTLayer::init())
    {
        return false;
    }
    
//    winSize = Director::getInstance()->getWinSize();
    
    m_eState = common_intro_top::HIDE;
    m_eCharacterType = chaType;
    m_sndFile = sndfile;
    
    
    this->setPosition(Vec2(0, 400.0f));

    std::string filename;
    if (m_eCharacterType == common_intro_top::ELLO)
    {
        filename = "common_intro_guide_ello.gaf";
    }
    else if (m_eCharacterType == common_intro_top::FONG)
    {
        filename = "common_intro_guide_fong.gaf";
    }
    else if (m_eCharacterType == common_intro_top::RENJI)
    {
        filename = "common_intro_guide_renji.gaf";
    }
    else if (m_eCharacterType == common_intro_top::RALLA)
    {

    }
    
    m_mainObject = createGAFObject(this, kDepth_Character, getCommonFilePath("flash", filename), false, Vec2(0+374.0f, 200.0f+702.0f));
    
    m_character = m_mainObject->getObjectByName("mc_character");
    m_character->playSequence("normal", true);
    
    Sprite* bg = Sprite::create(getCommonFilePath("img", "common_intro_top_dim.png"));
    bg->setAnchorPoint(Vec2(0.0f, 1.0f));
    bg->setPosition(Vec2(0, winSize.height));
    this->addChild(bg, kDepth_Bg, kDepth_Bg);
    
    Sprite* textBox = Sprite::create(getCommonFilePath("img", "common_intro_top_textbox.png"));
    textBox->setAnchorPoint(Vec2::ZERO);
    textBox->setPosition(Vec2(467.0f, 634.0f));
    textBox->setScale(0.0f);
    this->addChild(textBox, kDepth_TextBox, kDepth_TextBox);
    
    
    Sprite* text = Sprite::create(textfile);
    text->setPosition(Vec2(312.0f, 75.0f));
    text->setOpacity(0.0f);
    textBox->addChild(text, kDepth_Text, kDepth_Text);

    show();
    
    return true;
}


void Common_Intro_Top::onEnter()
{
    MGTLayer::onEnter();
    onViewLoad();
}

void Common_Intro_Top::onExit()
{
    MGTLayer::onExit();
}


void Common_Intro_Top::onViewLoad()
{
    MGTLayer::onViewLoad();
    
    
}


void Common_Intro_Top::onViewLoaded()
{
    MGTLayer::onViewLoaded();
}



void Common_Intro_Top::setFinishedDelegate(std::function<void()> delegate)
{
    m_finishedDelegate = delegate;
}



void Common_Intro_Top::show()
{
    m_eState = common_intro_top::SHOW;
 
    this->setVisible(true);
    
    auto action = EaseSineOut::create(MoveTo::create(0.5f, Vec2(0, 0.0f)));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Intro_Top::showComplete, this)),
                                nullptr);
    this->runAction(seq);
}

void Common_Intro_Top::showComplete()
{
    Sprite* textBox = (Sprite*)this->getChildByTag(kDepth_TextBox);
    
    auto action = EaseSineOut::create(ScaleTo::create(0.3f, 1.0f));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Intro_Top::textBoxScaleUpFinished, this)),
                                nullptr);
    textBox->runAction(seq);
    
}

void Common_Intro_Top::textBoxScaleUpFinished()
{
    Sprite* textBox = (Sprite*)this->getChildByTag(kDepth_TextBox);
    Sprite* text = (Sprite*)textBox->getChildByTag(kDepth_Text);
    
    auto action = EaseSineOut::create(FadeTo::create(0.5f, 255.0f));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Intro_Top::textFadeInFinished, this)),
                                nullptr);
    text->runAction(seq);
}

void Common_Intro_Top::textFadeInFinished()
{
    m_character->playSequence("speak", true);
//    MGTMultimedia::getInstance()->playNarration(m_sndFile);
}

void Common_Intro_Top::hide()
{
    m_eState = common_intro_top::HIDE;
    
    auto action = EaseSineOut::create(MoveTo::create(0.5f, Vec2(0, 400.0f)));
    auto seq = Sequence::create(
                                action,
                                CallFunc::create( CC_CALLBACK_0(Common_Intro_Top::hideComplete, this)),
                                nullptr);
    this->runAction(seq);
}

void Common_Intro_Top::hideComplete()
{
    this->setVisible(false);
    
    
    m_finishedDelegate();
}


#pragma mark gaf function

GAFObject* Common_Intro_Top::createGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position)
{
    if (!m_asset)
    {
        m_asset = GAFAsset::create(path, nullptr);
        CC_SAFE_RETAIN(m_asset);
    }
    
    GAFObject* object = nullptr;
    if (m_asset)
    {
        object = m_asset->createObjectAndRun(isloop);
        object->setPosition(position);
        parent->addChild(object, zOrder);
    }
    return object;
}

bool Common_Intro_Top::hitTestGAFObject(GAFObject* obj, Touch* touch)
{
    Vec2 localPoint = obj->convertTouchToNodeSpace(touch);
    localPoint = cocos2d::PointApplyTransform(localPoint, obj->getNodeToParentTransform());
    
    Rect r = obj->getBoundingBoxForCurrentFrame();
    return r.containsPoint(localPoint);
}



#pragma mark common function

Vec2 Common_Intro_Top::getLocalPoint(Node* childnode)
{
    float fAnchorX = childnode->getAnchorPoint().x;
    float fAnchorY = childnode->getAnchorPoint().y;
    
    auto nodeCenterPt = Vec2(childnode->getBoundingBox().origin.x + childnode->getBoundingBox().size.width * fAnchorX,
                             childnode->getBoundingBox().origin.y + childnode->getBoundingBox().size.height * fAnchorY);
    
    return childnode->getParent()->convertToWorldSpace(nodeCenterPt);
}


#pragma mark gaf sound delegate function

void Common_Intro_Top::onSoundPlay( GAFObject * object, const std::string& soundName )
{
    std::string::size_type pos;
    
    log("sound name:%s", soundName.c_str());

    pos = soundName.find("snd::");
    if (pos != std::string::npos)
    {
        std::string narrationName = soundName.substr(pos + 5) + ".mp3";
        onNarrationPlayForGAF(narrationName);
    }
    
    pos = soundName.find("sfx::");
    if (pos != std::string::npos)
    {
        std::string effectName = soundName.substr(pos + 5) + ".mp3";
        onEffectPlayForGAF(effectName);
    }
}




#pragma mark gaf sound play override function

void Common_Intro_Top::onNarrationPlayForGAF(const std::string filename)
{
//    MGTMultimedia::getInstance()->playNarration(getFilePath(ResourceType::SOUND, "snd", filename));
}

void Common_Intro_Top::onEffectPlayForGAF(const std::string filename)
{
//    MGTMultimedia::getInstance()->playEffect(getFilePath(ResourceType::SOUND, "snd", filename));
}


#pragma mark narration finished callback function

void Common_Intro_Top::onNarrationFinishedCallback(std::string fileName)
{
    std::string name = MGTUtils::getInstance()->stringTokenizer(m_sndFile, "/", false);
    
    if (fileName == name)
    {
        m_character->playSequence("normal", false);
        
        this->hide();
    }
}