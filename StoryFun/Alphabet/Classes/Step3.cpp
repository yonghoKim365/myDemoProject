#include "Step3.h"
#include "Debug_Index.h"

enum
{
    kTagStep2Animation = 0,
    kTagStep2AlphaOff,
    kTagStep2SaveRenderTexture,
    kTagStep2DrawObject = 10,
    
    kTagStep3DrawingArea,
    
    
    kTagAffordance,
    

    kTagObject,
    
    kTagStep3DrawingRenderTexture,
    kTagStep3RenderTexture,
    kTagStep3StickerRenderTexture,
    kTagStep3StickerContainer,
    
    kTagWord,
    kTagAlphabet,
    
    kTagStep3CrayonBox,
    kTagStep3StickerBox,
    kTagStep3TempSticker,
    
    kTagCrayonMenu,
    kTagEraserMenu,
    kTagStickerMenu,
    kTagSaveMenu,
    kTagSaveButton,
    
    
    kTagResultWord,
    kTagResultClipper,
    kTagResultStencil,
    kTagResultImg,
    kTagResultAlphabet,
    kTagResultRenderTexture,
    kTagResultCover,
};

namespace drawobject {
    enum
    {
        OBJ = 0,
        AFFORDANCE,
        CLIPPER,
        STENCIL,
        CONTENT,
        COMPLETE,
        LINE
    };
}

namespace stickerobject {
    enum
    {
        AREABOX = 0,
        DELETEMENU
    };
}


#define TOUCH_THRESHOLD 0.2

Step3::Step3():
m_count(0),
m_playNum(0),
m_objectImageData(nullptr),
m_brushImageData(nullptr),
m_brushRendertexture(nullptr),
m_saveRendertexture(nullptr),
m_affordance(nullptr)
{

}

Step3::~Step3()
{
}

Scene* Step3::createScene()
{
    auto scene = Scene::create();

    auto layer = Step3::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step3::init()
{
    if ( !Step3_Base::init() )
    {
        return false;
    }
    
//    auto bgcolor = Color4B(255, 247, 233, 255);
//    this->initWithColor(bgcolor);
    
//    Size visibleSize = Director::getInstance()->getVisibleSize();
//    Vec2 origin = Director::getInstance()->getVisibleOrigin();
    
    
    PsdParser::parseToPsdJSON("common_b01_c01_s3.json", &m_psdDictionary, true);
    
    std::string file = StringUtils::format("b01_c01_s3_t1_%02d.json", ProductManager::getInstance()->getCurrentAlphabet());
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    initView();
    
    return true;
}


void Step3::onEnter()
{
    Step3_Base::onEnter();
    
    onViewLoad();
}

void Step3::onExit()
{
    Step3_Base::onExit();
    
}

void Step3::onViewLoad()
{
    Step3_Base::onViewLoad();
}

void Step3::onViewLoaded()
{
    Step3_Base::onViewLoaded();
}


#pragma mark - touch

bool Step3::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    if(Step3_Base::onTouchBegan(pTouch, pEvent))
    {
        return false;
    }

    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }

    m_touch = pTouch;
    m_isTouchMoved = false;
    
    m_touchBeganTime = getCurrentTime();
    
    stopAffordanceTimer();

    if (m_eState == step3::STATE_PLAY1)
    {
        if (MGTUtils::hitTestPoint(m_alphabet, tp, false))
        {
            m_isTouchObject = true;
            touchAlphabet();
            
            hideAffordance();
            
            return true;
        }
        
        startAffordanceTimer();
        
    }
    else if (m_eState == step3::STATE_PLAY2)
    {
        if (isTouchDrawObject(tp))
        {
            log("touch");
            log("m_currentDrawObject : %d", m_currentDrawObject->getNum());
            eraseCoverAtPoint(tp);
            
            m_isTouchObject = true;
            
            hideAffordance();
            return true;
        }
        
        startAffordanceTimer();
    }
    else if (m_eState == step3::STATE_PLAY3)
    {

//        // made sticker touch check
//        for(int i = m_madeStickers.size(); i>0; i--)
//        {
//            auto sticker = m_madeStickers.at(i-1);
//            
//            if (MGTUtils::hitTestPointExact(sticker, sticker->getImagePath(), tp, false))
//            {
//                m_isTouchObject = true;
//                m_Play3TouchState = Play3_TouchState::TOUCHSTATE_STICKER_MADE;
//                
//                m_touchedSticker = sticker;
//                
//                pressSticker();
//                
//                Scheduler* scheduler = Director::getInstance()->getScheduler();
//                scheduler->unschedule(schedule_selector(Step3::onStickerPressActivate), this);
////                    scheduler->schedule(schedule_selector(Step3::onStickerPressActivate), this, 0, 0, 0.5f, false);
//                
//                unschedule(CC_SCHEDULE_SELECTOR(Step3::onStickerPressActivate));
//                schedule(CC_SCHEDULE_SELECTOR(Step3::onStickerPressActivate), 0.05f);
//                
//                return true;
//            }
//        }
//        
        
        // sticker box item touch check
        for (int i = 0; i< m_stickerItems.size(); i++)
        {
            auto stickerItem = m_stickerItems.at(i);
            
            if (stickerItem->getIsEnabled())
            {
                if (MGTUtils::hitTestPoint(stickerItem, tp, false))
                {
                    m_isTouchObject = true;
                    m_Play3TouchState = TOUCHSTATE_STICKER_MENUITEM;
                    
                    m_touchedSticker = stickerItem;
                    
                    touchStickerItem(stickerItem->getTag());
                    
                    unselectCrayonMenu();
                    
                    return true;
                }
            }
        }
        
        if (m_Play3State == Play3_State::STATE_DRAWING)
        {
            // drawing area touch check
            auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
            if (MGTUtils::hitTestPoint(drawingArea, tp, false))
            {
                m_isTouchObject = true;
                
                // drawing color
                if (m_paletteState == Play3_PaletteState::STATE_CRAYON)
                {
                    m_Play3TouchState = Play3_TouchState::TOUCHSTATE_DRAWING;
                    
                    m_step3DrawingRenderTexture->begin();
                    drawCrayonAtPoint(tp);
                    
                    m_step3DrawingRenderTexture->end();
                    Director::getInstance()->getRenderer()->render();
                    
                    return true;
                }
                // erase
                else if(m_paletteState == Play3_PaletteState::STATE_ERASE)
                {
                    m_Play3TouchState = Play3_TouchState::TOUCHSTATE_ERASE;
                    
                    m_step3DrawingRenderTexture->begin();
                    eraseAtPoint(tp);
                    m_step3DrawingRenderTexture->end();
                    Director::getInstance()->getRenderer()->render();
                    
//                    m_step3StickerRendertexture->begin();
//                    eraseAtPoint(tp);
//                    m_step3StickerRendertexture->end();
//                    Director::getInstance()->getRenderer()->render();
                    
                    return true;
                }
            }
        }
        else if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
        {
            if (MGTUtils::hitTestPoint(m_selectedSticker, tp, false) == false)
            {
                unselectSticker();
            }
        }

    }
    else if (m_eState == step3::STATE_END)
    {
        auto alphabet = m_endContainer->getChildByTag(kTagResultAlphabet);
        auto content = m_endContainer->getChildByTag(kTagResultImg);
        
        std::string file = StringUtils::format("b01_c01_s3_t1_%02d_word.png", ProductManager::getInstance()->getCurrentAlphabet());
        std::string imgPath = getFilePath(ResourceType::IMAGE, "img", file);
        
        
        Image* image = MGTResourceUtils::getInstance()->getImageData(imgPath);
        
        if (MGTUtils::hitTestPoint(alphabet, tp, false))
        {
            playAlphabetSound();
            return true;
        }
        else if (MGTUtils::hitTestPointExact(m_word, image, tp, false))
        {
            playWordSound();
        }
        
    }
    
    return false;
}

void Step3::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Step3_Base::onTouchMoved(pTouch, pEvent);
    
    
    log("TOUCH MOVED");
    
    if (m_isTouchObject == false)
    {
        return;
    }
    
    if(m_orientation != DeviceUtilManager::getInstance()->getScreenOrientation())
    {
        log("CHANGE ORIENTATION");
        onTouchCancelled(pTouch, pEvent);
        return;
    }
    
    log("TOUCH MOVED  1");
    
    Vec2 tp = pTouch->getLocation();
    Vec2 prevTp = pTouch->getPreviousLocation();
//    tp.x = (int)tp.x;
//    tp.y = (int)tp.y;
//    
//    prevTp.x = (int)prevTp.x;
//    prevTp.y = (int)prevTp.y;
    
    auto currentTouch = convertToNodeSpace(tp);
    auto prevTouch = convertToNodeSpace(prevTp);
    
    if (m_touch->getID() == pTouch->getID())
    {
        if (currentTouch.getDistance(prevTp) > 10)
        {
            m_isTouchMoved = true;
        }
    }
    
    if (m_isTouchObject)
    {
        if (m_eState == step3::STATE_PLAY2)
        {
            
            float distance = currentTouch.getDistance(prevTp);
            
            if(distance > 1)
            {
//                int d = distance;
                
                for (int i = 0; i < distance; i++)
                {
                    float difx = prevTouch.x - currentTouch.x;
                    float dify = prevTouch.y - currentTouch.y;
                    float delta = (float)i / distance;
                    
                    Vec2 pos = Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta));
                    eraseCoverAtPoint(pos);
                }
            }
        }
        else if (m_eState == step3::STATE_PLAY3)
        {
            if (m_Play3TouchState == Play3_TouchState::TOUCHSTATE_DRAWING)
            {
                
                float distance = currentTouch.getDistance(prevTouch);
                
                if(distance > 1)
                {
                    m_step3DrawingRenderTexture->begin();
                    
//                    int d = (int)distance;
                    
                    for (int i = 0; i < distance; i++)
                    {
                        float difx = prevTouch.x - currentTouch.x;
                        float dify = prevTouch.y - currentTouch.y;
                        float delta = (float)i / distance;
                        
                        Vec2 pos = Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta));
                        
                        drawCrayonAtPoint(pos);
                    }
                    
                    m_step3DrawingRenderTexture->end();
                    Director::getInstance()->getRenderer()->render();
                }
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_ERASE)
            {
                float distance = currentTouch.getDistance(prevTouch);
                
                if(distance > 1)
                {
                    
                    
//                    int d = (int)distance;
                    
//                    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
//                    drawDot->setColor(Color3B::BLACK);
//                    BlendFunc blendFunc = { GL_ZERO, GL_ONE_MINUS_SRC_ALPHA };
//                    drawDot->setBlendFunc(blendFunc);
//                    
                    m_step3DrawingRenderTexture->begin();
                    
                    for (int i = 0; i < distance; i++)
                    {
                        float difx = prevTouch.x - currentTouch.x;
                        float dify = prevTouch.y - currentTouch.y;
                        float delta = (float)i / distance;
                        
                        Vec2 pos = Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta));
                        
//                        eraseAtPoint(pos);
                        
                        
                        auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
                        
                        auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
                        
//                        auto drawDot = Sprite::createWithSpriteFrame(dot->getSpriteFrame());
//                        auto drawDot = Sprite::createWithSpriteFrameName("common_b01_c01_s1_t2_brush");
                        
                        drawDot->setColor(Color3B::BLACK);
                        drawDot->setPosition(pos);
                        
                        MGTUtils::setPositionForParent(drawingArea, drawDot);
                        
                        BlendFunc blendFunc = { GL_ZERO, GL_ONE_MINUS_SRC_ALPHA };
                        drawDot->setBlendFunc(blendFunc);
                        
//                        drawDot->visit();
                        
                        m_dotsArr.pushBack(drawDot);
//                        m_dotsPosArr.push_back(drawDot->getPosition());
                        m_touchObjectSize = drawDot->getContentSize();
                        
                        
                        
                        drawDot->visit();
                        
                        
//                        m_step3StickerRendertexture->begin();
//                        drawDot->visit();
//                        m_step3StickerRendertexture->end();
//                        Director::getInstance()->getRenderer()->render();
                        
                    }
                    
                    m_step3DrawingRenderTexture->end();
                    Director::getInstance()->getRenderer()->render();
                    
//                    m_step3Rendertexture->begin();
//                    
//                    for (int i = 0; i<m_dotsArr.size(); i++)
//                    {
//                        auto drawDot = m_dotsArr.at(i);
//                        drawDot->visit();
//                    }
//                    
//                    m_step3Rendertexture->end();
//                    Director::getInstance()->getRenderer()->render();
                    
                    
                    
//                    m_step3StickerRendertexture->begin();
//                    
//                    for (int i = 0; i<m_dotsArr.size(); i++)
//                    {
//                        auto drawDot = m_dotsArr.at(i);
////                        auto pos = m_dotsPosArr.at(i);
////                        drawDot->setPosition(pos);
//                        
//                        drawDot->visit();
//                        drawDot->retain();
//                    }
//                    
//                    m_step3StickerRendertexture->end();
//                    Director::getInstance()->getRenderer()->render();
                }
                
//                m_dotsPosArr.clear();
                m_dotsArr.clear();
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MENUITEM)
            {
//                float minX = m_stickerBox->getPosition().x - m_stickerBox->getContentSize().width/2 + m_touchObjectSize.width/2;
//                float minY = m_stickerBox->getPosition().y + m_stickerBox->getContentSize().height/2 + 20 + m_touchObjectSize.height/2;;
//                
//                if (currentTouch.x < minX)
//                {
//                    currentTouch.x = minX;
//                }
//
//                if (prevTouch.x < minX)
//                {
//                    prevTouch.x = minX;
//                }
//
//                if (currentTouch.y < minY)
//                {
//                    currentTouch.y = minY;
//                }
//                
//                if (prevTouch.y < minY)
//                {
//                    prevTouch.y = minY;
//                }
                
                
                Vec2 pos = m_tempSticker->getPosition() + (currentTouch - prevTouch);
//                pos.x = (int) pos.x;
//                pos.y = (int) pos.y;
                m_tempSticker->setPosition(pos);
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MADE)
            {
                
            }
        }
    }
    
}

void Step3::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Step3_Base::onTouchEnded(pTouch, pEvent);
    
    Vec2 tp = pTouch->getLocation();
    
//    tp.x = (int)tp.x;
//    tp.y = (int)tp.y;
    
    float elapsedTime = getCurrentTime() - m_touchBeganTime;
    
    if (m_isTouchObject)
    {
        if (m_eState == step3::STATE_PLAY2)
        {
            if (checkIntersectionPercentage())
            {
                writeComplete();
            }
            else
            {
                startAffordanceTimer();
            }
        }
        else if (m_eState == step3::STATE_PLAY3)
        {
            if (m_Play3State == Play3_State::STATE_NONE)
            {
                if (m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MADE)
                {
                    Scheduler* scheduler = Director::getInstance()->getScheduler();
                    scheduler->unschedule(schedule_selector(Step3::onStickerPressActivate), this);
                    
                    unselectSticker();
                }
                
            }
            
            
            
            if (m_Play3TouchState == Play3_TouchState::TOUCHSTATE_DRAWING)
            {
                if (m_isDrawComplete == false)
                {
                    saveButtonActivation();
                    m_isDrawComplete = true;
                }
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_ERASE)
            {
                
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MENUITEM)
            {
                auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
                
                if (MGTUtils::hitTestPoint(drawingArea, m_tempSticker->getPosition(), false))
                {
                    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
                    
                    std::string file = StringUtils::format("b01_c01_s3_t1_%02d_word.png", ProductManager::getInstance()->getCurrentAlphabet());
                    
//                    if (!MGTUtils::hitTestPointExact(m_word, MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", file), m_tempSticker->getPosition(), false))
//                    {
//                        makeSticker();
//                    }
                    makeSticker();
                    
                    if (m_isDrawComplete == false)
                    {
                        saveButtonActivation();
                        m_isDrawComplete = true;
                    }
                }
                
                m_tempSticker->removeFromParentAndCleanup(true);
                
            }
            else if(m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MADE)
            {
                
            }
        }
    }
    
    m_touch = nullptr;
    m_isTouchMoved = false;
    m_isTouchObject = false;
    m_touchedSticker = nullptr;
    m_Play3TouchState = Play3_TouchState::TOUCHSTATE_NONE;
}

void Step3::onTouchCancelled(Touch *touch, Event *event)
{
    Step3_Base::onTouchCancelled(touch, event);
}

void Step3::onNarrationFinishedCallback(std::string fileName)
{
    Step3_Base::onNarrationFinishedCallback(fileName);
    
    log("Narration Finished fileName : %s", fileName.c_str());
    
    if (fileName == m_strWordSound)
    {
        if (m_eState == step3::STATE_PLAY2)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                                        CallFunc::create( CC_CALLBACK_0(Step3::nextPlay, this)),
                                                        nullptr));
        }
    }
    else if(fileName == m_strGuideSound)
    {
        m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                         CallFunc::create( CC_CALLBACK_0(Step3::interactionStart, this)),
                                         nullptr));
    }
    else if(fileName == m_strPhonicsSound)
    {
        log("phonics sound finish");
        
        if (m_eState == step3::STATE_GUIDE)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                             CallFunc::create( CC_CALLBACK_0(Step3_Base::playGuideAnimation, this)),
                                             nullptr));
        }
        else
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                                        CallFunc::create( CC_CALLBACK_0(Step3::nextPlay, this)),
                                                        nullptr));
        }
    }

}


#pragma mark gaf delegate function
void Step3::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step3_Base::onFinishSequence(object, sequenceName);
    
    if(sequenceName.compare("Animation") == 0 )
    {
        m_gafObject->setSequenceDelegate(nullptr);
        m_gafObject->setSoundPlayDelegate(nullptr);
        
        m_gafObject->pauseAllAnimation();
        playPhonicsSound();
    }
}

void Step3::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step3_Base::onFramePlayed(object, frame);
}

void Step3::onTexturePreLoad(std::string& path)
{
    Step3_Base::onTexturePreLoad(path);
}



#pragma mark navi touch override function
void Step3::onTouchedNavigationButtonAtExit()
{
    Step3_Base::onTouchedNavigationButtonAtExit();
    
    if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
    {
        unselectSticker();
        
        m_Play3State = Play3_State::STATE_NONE;
    }
}


void Step3::onTouchedNavigationButtonAtNext()
{
    Step3_Base::onTouchedNavigationButtonAtNext();
}



void Step3::onTouchedPopupButtonAtNo()
{
    Step3_Base::onTouchedPopupButtonAtNo();
    
    if (m_commonPopup->getType() == common_popup::DRAWING_DELETE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::EXIT)
    {
        
    }
}

void Step3::onTouchedPopupButtonAtYes()
{
    Step3_Base::onTouchedPopupButtonAtYes();
    
    if (m_commonPopup->getType() == common_popup::DRAWING_DELETE)
    {
        m_step3DrawingRenderTexture->clear(0, 0, 0, 0);
//        m_step3StickerRendertexture->clear(0, 0, 0, 0);
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVE)
    {
        
    }
    else if (m_commonPopup->getType() == common_popup::DRAWING_SAVING)
    {
        log("-----------------YES   DRAWING DRAWING_SAVING");
        
        m_playContainer->runAction( Sequence::create(DelayTime::create(0.2f),
                                                     CallFunc::create( CC_CALLBACK_0(Step3::captureImage, this)),
                                                     nullptr));
    }
    else if (m_commonPopup->getType() == common_popup::EXIT)
    {
        
    }
}


#pragma mark step1_type1

void Step3::initView()
{
    createSaveButton();
    
    this->createCrayonBox();
    this->createStickerBox();
    
    
    createPlay1();
    
    
    Size conSize = m_alphabet->getContentSize();
    
    std::string file = StringUtils::format("b01_c01_s3_t1_%02d_alpha", ProductManager::getInstance()->getCurrentAlphabet());
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    pos = Vec2(pos.x - conSize.width/2, pos.y + conSize.height/2);
    
    file = StringUtils::format("b01_c01_s3_alphabet%02d.gaf", ProductManager::getInstance()->getCurrentAlphabet());
    m_gafObject = createGAFObject(m_playContainer, kTagStep2Animation, getFilePath(ResourceType::GAF, "flash", file), true, pos);
    m_gafObject->stop();
    m_gafObject->setVisible(false);
    m_gafObject->setSequenceDelegate(CC_CALLBACK_2(Step3::onFinishSequence, this));
    m_gafObject->setSoundPlayDelegate(CC_CALLBACK_2(Base_Layer::onSoundPlay, this));
}


#pragma mark - step1
void Step3::createPlay1()
{
    m_playNum = 1;
    
    Vec2 pos;
    std::string file;
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha", ProductManager::getInstance()->getCurrentAlphabet());
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_alphabet = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    m_alphabet->setColor(Color3B(127, 126, 124));
    m_alphabet->setPosition(pos);
    m_playContainer->addChild(m_alphabet, kTagAlphabet, kTagAlphabet);
    
    
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_line2.png", ProductManager::getInstance()->getCurrentAlphabet());
    m_affordance = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    m_affordance->setPosition(pos);
    m_affordance->setColor(Color3B( 255, 126, 0));
    m_affordance->setOpacity(0.0f);
    m_affordance->setVisible(false);
    m_playContainer->addChild(m_affordance, kTagAffordance, kTagAffordance);
    
    
    pos = PsdParser::getPsdPosition("common_b01_c01_s3_object", &m_psdDictionary);
    file = StringUtils::format("b01_c01_s3_t1_%02d_object.png", ProductManager::getInstance()->getCurrentAlphabet());
    m_object = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    m_object->setPosition(pos);
    m_playContainer->addChild(m_object, kTagObject, kTagObject);
    
    
    m_gafGuide->setPosition(Vec2(m_gafGuide->getPosition().x + m_alphabet->getPosition().x, m_gafGuide->getPosition().y + m_alphabet->getPosition().y));
    
}

void Step3::blinkAlphabetAnimation()
{
    auto seq = Sequence::create(EaseSineOut::create(FadeTo::create(0.5f, 255.0f*0.5f)),
                                EaseSineOut::create(FadeTo::create(0.5f, 255.0f)),
                                nullptr);
    
    auto repeatforever = RepeatForever::create(seq);
    
    m_alphabet->runAction(repeatforever);
}

void Step3::touchAlphabet()
{
    m_touchEnabled = false;
    
    m_alphabet->stopAllActions();
    m_alphabet->setOpacity(255.0f);
    alphabetAnimation();
    
//    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_correct_01.mp3"));
}

void Step3::alphabetAnimation()
{
    Size conSize = m_alphabet->getContentSize();
    m_alphabet->removeFromParentAndCleanup(true);
    
    std::string file;
    
    m_gafObject->start();
    m_gafObject->setVisible(true);
    m_gafObject->playSequence("Animation", false);
    m_gafObject->setScale(2.0f);
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s3_sfx_alphabet_ani.mp3"));
}


#pragma mark - step2
void Step3::createPlay2()
{
    m_eState = step3::STATE_PLAY2;
    
    createDrawAlphabet();

    setDrawWrite();
    
    m_playContainer->runAction( Sequence::create(DelayTime::create(0.5f),
                                      CallFunc::create( CC_CALLBACK_0(Step3_Base::playGuideSound, this)),
                                      nullptr));
    
}

void Step3::createDrawAlphabet()
{
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    int totalDrawNum;
    
    if(m_gafObject)
    {
        m_gafObject->stop();
        MGTUtils::removeAllchildren(m_gafObject);
        m_gafObject->removeFromParentAndCleanup(true);
        
        m_gafObject = nullptr;
    }
    
//    if(alphabetNum == 3 || alphabetNum == 12 || alphabetNum == 15 || alphabetNum == 19 )
//    {
//        totalDrawNum = 1;
//    }
//    else if(alphabetNum == 1 || alphabetNum == 2 || alphabetNum == 4 || alphabetNum == 5 || alphabetNum == 6 || alphabetNum == 7 || alphabetNum == 8 || alphabetNum == 9 || alphabetNum == 10 || alphabetNum == 14 || alphabetNum == 16 || alphabetNum == 17 || alphabetNum == 18 || alphabetNum == 20 || alphabetNum == 21 || alphabetNum == 22 || alphabetNum == 24 || alphabetNum == 25 )
//    {
//        totalDrawNum = 2;
//    }
//    else if(alphabetNum == 11 || alphabetNum == 13 || alphabetNum == 26 )
//    {
//        totalDrawNum = 3;
//    }
//    else if( alphabetNum == 23 )
//    {
//        totalDrawNum = 4;
//    }
    
    totalDrawNum = 1;
    
    std::string file;
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha", alphabetNum);
    Vec2 alphaPosition = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    
//    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha.png",ProductManager::getInstance()->getCurrentAlphabet());
//    Sprite* alpha_off = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
//    alpha_off->setPosition(alphaPosition);
//    m_playContainer->addChild(alpha_off, kTagStep2AlphaOff, kTagStep2AlphaOff);
    
    
    for (int i = 0; i<totalDrawNum; i++)
    {
        int tag = kTagStep2DrawObject + i;
        
        int objNum = i+1;
        log("objNum:%d", objNum);
        
        file = StringUtils::format("b01_c01_s3_t1_%02d_alpha.png", alphabetNum);
        DrawObject* drawObject = DrawObject::create(getFilePath(ResourceType::IMAGE, "img", file).c_str());
        drawObject->setPosition(alphaPosition);
        m_playContainer->addChild(drawObject, tag, tag);
        
        drawObject->setNum(objNum);
        
        file = StringUtils::format("b01_c01_s3_t1_%02d_alpha.png", alphabetNum);
        drawObject->setImagePath(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", file).c_str());
        
        
        file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_area.png", alphabetNum);
        drawObject->setAreaImagePath(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", file).c_str());
        
        
        file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_line.png", alphabetNum);
        Sprite* line = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
        line->setPosition(alphaPosition);
        MGTUtils::setPositionForParent(drawObject, line);
        line->setColor(Color3B(0, 0, 0));
        line->setVisible(false);
        drawObject->addChild(line, drawobject::LINE, drawobject::LINE);
        
        // clipper
        ClippingNode* clipper = ClippingNode::create();
        clipper->setContentSize( drawObject->getContentSize() );
        clipper->setAnchorPoint( Vec2(0.5, 0.5) );
        clipper->setPosition(alphaPosition);
//        clipper->setOpacity(0.0f);
        MGTUtils::setPositionForParent(drawObject, clipper);
        clipper->setAlphaThreshold(0.05f);
        
        drawObject->addChild(clipper, drawobject::CLIPPER, drawobject::CLIPPER);
        drawObject->setCoverClipper( clipper );
        
        
        // stencil
        Sprite* stencil = Sprite::create();
        stencil->setContentSize(drawObject->getContentSize());
        stencil->setPosition(alphaPosition);
        MGTUtils::setPositionForParent(drawObject, stencil);
        clipper->setStencil( stencil );
        
        drawObject->setEraseStencil( stencil );
        
        
        // content
        file = StringUtils::format("b01_c01_s3_t1_%02d_alpha.png", alphabetNum);
        auto cover = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file).c_str());
        cover->setPosition(alphaPosition);
        cover->setColor(Color3B(237, 28, 36));
        MGTUtils::setPositionForParent(drawObject, cover);
        clipper->addChild(cover, drawobject::CONTENT, drawobject::CONTENT);
        
        
        std::string file = StringUtils::format("b01_c01_s3_t1_%02d_alpha", ProductManager::getInstance()->getCurrentAlphabet());
        Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
        
        m_affordance = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
        m_affordance->setPosition(pos);
        m_affordance->setColor(Color3B(127, 126, 124));
        m_affordance->setOpacity(0.0f);
        m_affordance->setVisible(false);
        MGTUtils::setPositionForParent(drawObject, m_affordance);
        drawObject->addChild(m_affordance, drawobject::AFFORDANCE, drawobject::AFFORDANCE);
        
        
        m_drawObjects.pushBack(drawObject);
        
        log("for tag :%d", drawObject->getTag());
    }
}

void Step3::setDrawWrite()
{
    m_brushes.clear();
    
    if (m_objectImageData)
    {
        CC_SAFE_RELEASE(m_objectImageData);
    }
    if (m_count == 0)
    {
        m_currentDrawObject = m_drawObjects.at(0);
    }
    else
    {
        m_currentDrawObject = m_drawObjects.at( m_count-1);
    }
    
    m_playContainer->reorderChild(m_currentDrawObject, 100);
    
    m_objectImageData = MGTResourceUtils::getInstance()->getImageData(m_currentDrawObject->getImagePath());
    
    auto line = m_currentDrawObject->getChildByTag(drawobject::LINE);
    line->setVisible(true);
    
    createRenderTexture();
    
}

bool Step3::isTouchDrawObject(Vec2 point)
{
    std::string imgPath = m_currentDrawObject->getAreaImagePath();
    
    Image* image = MGTResourceUtils::getInstance()->getImageData(imgPath);
    
    if (MGTUtils::hitTestPointExact(m_currentDrawObject, image, point, false))
    {
        return true;
    }
    
    return false;
}

void Step3::eraseCoverAtPoint(Vec2 point)
{
    
    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
    drawDot->setScale(2.0f);
    //    drawDot->setOpacity(50.0f);
    drawDot->setPosition( point );
    MGTUtils::setPositionForParent(m_currentDrawObject, drawDot);

//    const Vec2& center, float radius, float angle, unsigned int segments, float scaleX, float scaleY, const Color4F &color
//    auto drawDot = DrawNode::create();
//    drawDot->drawSolidCircle( point, 70, 0, 50, 1.0f, 1.0f, Color4F(1.0f, 1.0f, 1.0f, 1.0f));
//    drawDot->setPosition(point);
//    MGTUtils::setPositionForParent(m_currentDrawObject, drawDot);
    
    m_currentDrawObject->getEraseStencil()->addChild(drawDot);
    drawDot->retain();
    
    m_brushes.push_back(drawDot->getPosition());
}


void Step3::createRenderTexture()
{
    if(m_brushRendertexture)
    {
        m_brushRendertexture->removeFromParentAndCleanup(true);
    }
    
    log("create render size width:%f  height:%f", m_currentDrawObject->getContentSize().width, m_currentDrawObject->getContentSize().height);
    
    m_brushRendertexture = RenderTexture::create(m_currentDrawObject->getContentSize().width, m_currentDrawObject->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    m_brushRendertexture->setPosition(Vec2(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2));
    m_brushRendertexture->setVisible(false);
    this->addChild(m_brushRendertexture, 100, 100);
}


bool Step3::checkIntersectionPercentage()
{
    
    //    std::string file = StringUtils::format("b01_c01_s1_t2_02_object%02d_white", m_currentDrawObject->getNum());
    //    Sprite* sp = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    //    sp->setColor(Color3B::RED);
    //    sp->setPosition(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2);
    
    m_brushRendertexture->beginWithClear(0, 0, 0, 0);
    
//    log("brush size:%d", m_brushes.size());
    
//    Vector<Sprite*> tempBrushes;
    for (int i = 0; i< m_brushes.size(); i++)
    {
        Sprite* drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
        drawDot->setColor(Color3B(237, 28, 36));
//        drawDot->setColor(Color3B::GREEN);
        drawDot->setScale(2.0f);
        drawDot->setPosition(m_brushes.at(i));
        drawDot->visit();
        
//        auto drawDot = DrawNode::create();
//        drawDot->drawSolidCircle( Vec2::ZERO, 70, 0, 50, 1.0f, 1.0f, Color4F(0.0f, 1.0f, 0.0f, 1.0f));
//        drawDot->setPosition(m_brushes.at(i));
//        drawDot->visit();
        
        
//        tempBrushes.pushBack(brush);
        drawDot->retain();
    }
    
    m_brushRendertexture->end();
    Director::getInstance()->getRenderer()->render();
    
    saveTempImageFile();
    
    m_brushImageData = MGTResourceUtils::getInstance()->getImageData(getTempImageFile(), true);
    
    //// BRUSH RGB CHECK
    int totalPixel = m_objectImageData->getDataLen()/4;
    int colorPixel = 0;
    int drawingPixel = 0;
    
    
    for (int i = 0; m_objectImageData->getDataLen()/4 > i; i++)
    {
        totalPixel++;
        
        unsigned char *pixel = m_objectImageData->getData() + (4 * i);
        unsigned char *drawpixel = m_brushImageData->getData() + (4 * i);
        
        //            CCLOG("%u, %u, %u ,%u", pixel[0], pixel[1], pixel[2], pixel[3]);
        
        if(!MGTUtils::isZeroPixel(pixel))
        {
            colorPixel++;
            
            Color4B brushColor = Color4B(237, 28, 36, 255);
//            Color4B brushColor = Color4B(0, 255, 0, 255);
            
//            if (MGTUtils::isTestPixel(drawpixel, brushColor.r, brushColor.g, brushColor.b , brushColor.a))
            
            if (brushColor.g == drawpixel[1] )
            {
                drawingPixel++;
            }
        }
    }
    
    CC_SAFE_RELEASE(m_brushImageData);
    
    log("totalPixel == %d, colorPixel == %d , drawingPixel == %d", totalPixel, colorPixel, drawingPixel);
    
    float percent = (float)((float)drawingPixel / (float)colorPixel) * 100;
    
    log("percent:%f", percent);
    
    bool ret = false;
    if (percent > 95)
    {
        ret = true;
    }
    return ret;
}


void Step3::saveTempImageFile()
{
    
    std::string name = "b01_c01_step3_temp.png";
    
    m_brushRendertexture->saveToFile(name, Image::Format::PNG, true);
    //Add this function to avoid crash if we switch to a new scene.
    Director::getInstance()->getRenderer()->render();
    
    
//    std::string file = FileUtils::getInstance()->getWritablePath();
//    file.append(name);
//    auto sprite = Sprite::create(file);
//    this->addChild(sprite, 100);
//    sprite->setPosition(Vec2(winSize.width - sprite->getContentSize().width/2, sprite->getContentSize().height/2));
    
}

std::string Step3::getTempImageFile()
{
    std::string file = FileUtils::getInstance()->getWritablePath();
    file.append("b01_c01_step3_temp.png");
    return file;
}

void Step3::saveAlphabetImageFile()
{
    auto m_brushRendertexture = RenderTexture::create(m_currentDrawObject->getContentSize().width, m_currentDrawObject->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    m_brushRendertexture->setPosition(Vec2(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2));
    m_brushRendertexture->setVisible(false);
    this->addChild(m_brushRendertexture, 100, 100);
    
    
    std::string name = "b01_c01_step3_draw_alphabet.png";
    
    m_brushRendertexture->saveToFile(name, Image::Format::PNG, true);
    //Add this function to avoid crash if we switch to a new scene.
    Director::getInstance()->getRenderer()->render();
    
}

std::string Step3::getAlphabetImageFile()
{
    std::string file = FileUtils::getInstance()->getWritablePath();
    file.append("b01_c01_step3_draw_alphabet.png");
    return file;
}


void Step3::writeComplete()
{
    m_touchEnabled = false;
    
    std::string file = StringUtils::format("b01_c01_s3_t1_%02d_alpha",ProductManager::getInstance()->getCurrentAlphabet());
    Vec2 alphaPosition = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    
    // create masked RenderTexture
    m_saveRendertexture = RenderTexture::create(m_currentDrawObject->getContentSize().width, m_currentDrawObject->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    m_saveRendertexture->setPosition(Vec2(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2));
    m_playContainer->addChild(m_saveRendertexture, kTagStep2SaveRenderTexture, kTagStep2SaveRenderTexture);
    

    
    Sprite* drawSp = Sprite::createWithTexture(m_brushRendertexture->getSprite()->getTexture());
    drawSp->setPosition(Vec2(drawSp->getContentSize().width/2, drawSp->getContentSize().height/2));
    drawSp->setFlippedY(true);
    
    
    Sprite* maskSp = Sprite::create(m_currentDrawObject->getImagePath());
    maskSp->setPosition(drawSp->getPosition());

    maskSp->setBlendFunc((BlendFunc){GL_ONE, GL_ZERO});
    drawSp->setBlendFunc((BlendFunc){GL_DST_ALPHA, GL_ZERO});
    
    
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_line.png", ProductManager::getInstance()->getCurrentAlphabet());
    Sprite* line = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    line->setPosition(drawSp->getPosition());
    line->setColor(Color3B(255, 255, 255));
    

    m_saveRendertexture->beginWithClear(0, 0, 0, 0);
    maskSp->visit();
    drawSp->visit();
//    line->visit();
    m_saveRendertexture->end();
    Director::getInstance()->getRenderer()->render();
    //////////////////////////////////////////////////////////////
    
    m_alphabet = Sprite::createWithTexture(m_saveRendertexture->getSprite()->getTexture());
    m_alphabet->setFlippedY(true);
//    m_alphabet->setPosition(Vec2(m_alphabet->getContentSize().width/2, m_alphabet->getPosition().y + m_alphabet->getContentSize().height*3));
    m_alphabet->setPosition(alphaPosition);
    // this is the default filterting
    m_alphabet->getTexture()->setAntiAliasTexParameters();

    

    MGTUtils::removeAllchildren(m_currentDrawObject);
    m_currentDrawObject->removeFromParentAndCleanup(true);
    m_playContainer->addChild(m_alphabet, kTagAlphabet, kTagAlphabet);

    
    m_saveRendertexture->removeFromParentAndCleanup(true);
    CC_SAFE_RETAIN(maskSp);
    CC_SAFE_RETAIN(drawSp);
    
    
    makeWordAnimation();
    
}

void Step3::makeWordAnimation()
{
    std::string file;
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_s",ProductManager::getInstance()->getCurrentAlphabet());
    Vec2 targetPos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    
    auto moveT = EaseSineOut::create( MoveTo::create(0.5f, targetPos));
    auto scaleT = EaseSineOut::create( ScaleTo::create(0.5f, 0.19f));
    
    auto spawn = Spawn::create(moveT,
                               scaleT,
                               nullptr);
    
    auto seq = Sequence::create(spawn,
                                CallFunc::create( CC_CALLBACK_0(Step3::playWordSound, this)),
                                nullptr);
    
    m_alphabet->runAction(seq);
    
    
    Vec2 pos = PsdParser::getPsdPosition("common_b01_c01_s3_word", &m_psdDictionary);
    
    file = StringUtils::format("b01_c01_s3_t1_%02d_word.png", ProductManager::getInstance()->getCurrentAlphabet());
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    m_word->setPosition(pos);
    m_word->setOpacity(0.0f);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    m_word->runAction( EaseSineOut::create( FadeTo::create(1.0f, 255.0f) ) );
    
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s3_sfx_write_complete.mp3"));
}


#pragma mark - step3
void Step3::createPlay3()
{
    m_eState = step3::STATE_PLAY3;
    
    m_Play3TouchState = Play3_TouchState::TOUCHSTATE_NONE;
    m_Play3State = Play3_State::STATE_NONE;
    m_isDrawComplete = false;
    
    m_affordance = nullptr;
    
//    m_navigation->setVisibleButton(false, navi::eNavigationButton_Guide);
    
//    Sprite* drawingArea = Sprite::create(getCommonFilePath("img", "common_b01_c01_s3_drawing_area.png"));
//    drawingArea->setPosition(Vec2(1026, 694));
    
    
    m_touchObjectSize = Size(0, 0);
    float minX = m_stickerBox->getPosition().x - m_stickerBox->getContentSize().width/2;
    float minY = m_stickerBox->getPosition().y + m_stickerBox->getContentSize().height/2 + 20;
    
    
    Sprite* drawingArea = Sprite::create();
    drawingArea->setContentSize(Size( winSize.width - m_crayonBox->getContentSize().width - 20, winSize.height - m_stickerBox->getContentSize().height - 20 ));
//    drawingArea->setTextureRect(Rect(0, 0, drawingArea->getContentSize().width, drawingArea->getContentSize().height));
//    drawingArea->setColor(Color3B::YELLOW);
//    drawingArea->setOpacity(0.0f);
    drawingArea->setPosition(Vec2(minX + drawingArea->getContentSize().width/2, minY + drawingArea->getContentSize().height/2));
    
    m_playContainer->addChild(drawingArea, kTagStep3DrawingArea, kTagStep3DrawingArea);
    
    m_stickerContainer = Sprite::create();
    m_stickerContainer->setContentSize(Size(winSize.width, winSize.height));
    m_stickerContainer->setPosition(winSize.width/2, winSize.height/2);
    m_playContainer->addChild(m_stickerContainer, kTagStep3StickerContainer, kTagStep3StickerContainer);
    
    
    for (int i = 0; i < m_crayonItems.size(); i++)
    {
        std::string file = StringUtils::format("common_b01_c01_s3_c_%02d_s.png", i+1);
        
        Sprite* disableSp = Sprite::create(getCommonFilePath("img", file));
        MenuItemImage* crayonItem = (MenuItemImage*)m_crayonItems.at(i);
        crayonItem->setDisabledImage(disableSp);
        crayonItem->setEnabled(true);
        
//        if (i == 0)
//        {
//            this->setCrayonEnabled(0);
//        }
    }

    for (int j = 0; j<m_stickerItems.size(); j++)
    {
        Sprite* stickerItem = m_stickerItems.at(j);
        
        std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_n.png", j+1);
        stickerItem->setTexture(Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file)));
    }
    
    m_step3DrawingRenderTexture = RenderTexture::create(drawingArea->getContentSize().width, drawingArea->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    m_step3DrawingRenderTexture->setPosition(Vec2(drawingArea->getPosition()));
    m_playContainer->addChild(m_step3DrawingRenderTexture, kTagStep3DrawingRenderTexture, kTagStep3DrawingRenderTexture);
    
//    m_step3StickerRendertexture = RenderTexture::create(drawingArea->getContentSize().width, drawingArea->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
//    m_step3StickerRendertexture->setPosition(Vec2(drawingArea->getPosition()));
//    m_playContainer->addChild(m_step3StickerRendertexture, kTagStep3StickerRenderTexture, kTagStep3StickerRenderTexture);
    
//    m_step3tempRenderTexture = RenderTexture::create(drawingArea->getContentSize().width, drawingArea->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
//    m_step3tempRenderTexture->setPosition(Vec2(-drawingArea->getContentSize().width/2, drawingArea->getContentSize().height/2));
//    m_playContainer->addChild(m_step3tempRenderTexture, 0, 0);

    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s3_sfx_drawing_on.mp3"));

    
    m_playContainer->runAction(Sequence::create(DelayTime::create(0.5f),
                                                CallFunc::create( CC_CALLBACK_0(Step3::interactionStart, this)),
                                                nullptr));
}


void Step3::createCrayonBox()
{
    m_crayonBox = PsdParser::getPsdCommonSprite("common_b01_c01_s3_b_01", &m_psdDictionary);
    m_playContainer->addChild(m_crayonBox, kTagStep3CrayonBox, kTagStep3CrayonBox);
    
    for (int i = 0; i<14; i++)
    {
        int tag = i;
        
        std::string normal = StringUtils::format("common_b01_c01_s3_c_%02d_n.png", i+1);
        std::string press = StringUtils::format("common_b01_c01_s3_c_%02d_p.png", i+1);
        std::string select = StringUtils::format("common_b01_c01_s3_c_%02d_d.png", i+1);
        
        auto item = MenuItemImage::create(getCommonFilePath("img", normal),
                                          getCommonFilePath("img", press),
                                          getCommonFilePath("img", select),
                                          CC_CALLBACK_1(Step3::crayonMenuCallback, this));
        
        item->setTag(tag);
        
        std::string name = StringUtils::format("common_b01_c01_s3_c_%02d", i+1);
        Vec2 pos = PsdParser::getPsdPosition(name, &m_psdDictionary);
        item->setPosition(pos);
        MGTUtils::setPositionForParent(m_crayonBox, item);
        
        item->setEnabled(false);
        m_crayonItems.pushBack(item);
    }
    
    auto menu = Menu::createWithArray(m_crayonItems);
    menu->setPosition(Vec2::ZERO);
//    menu->setEnabled(false);
    m_crayonBox->addChild(menu, kTagCrayonMenu, kTagCrayonMenu);
    
    log("crayon");
}

void Step3::unselectCrayonMenu()
{
    m_Play3State = Play3_State::STATE_NONE;
    
    Menu* menu = (Menu*)m_crayonBox->getChildByTag(kTagCrayonMenu);
    
    for (int i = 0; i < m_crayonItems.size(); i++)
    {
        MenuItemImage* crayonItem = (MenuItemImage*)m_crayonItems.at(i);
        crayonItem->setEnabled(true);
    }

}

void Step3::crayonMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    int tag = item->getTag();
    
    this->setCrayonEnabled(tag);
    
    unselectSticker();
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_btn_02.mp3"));
}

void Step3::setCrayonEnabled(int currentNum)
{
    Menu* menu = (Menu*)m_crayonBox->getChildByTag(kTagCrayonMenu);
    
    int prevCrayon = m_crayonNum;
    
    if (currentNum < 13)
    {
        m_crayonNum = currentNum;
    }
    
    for (int i = 0; i < 14; i++)
    {
        MenuItem* item = (MenuItem*)menu->getChildByTag(i);
        
        int crayonNum = item->getTag();
        
        if (m_crayonNum == crayonNum)
        {
            item->setEnabled(false);
            this->setCrayon();
        }
        else
        {
            item->setEnabled(true);
        }
    }
    
    if (currentNum == 13)
    {
        if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
        {
            unselectSticker();
            m_Play3State = Play3_State::STATE_NONE;
        }
        
        unselectCrayonMenu();
        onPause();
        
//        createCommonPopup(common_popup::DRAWING_DELETE, true);
        showCommonPopup(common_popup::DRAWING_DELETE);
    }
}

void Step3::setCrayon()
{
    log("crayon %d", m_crayonNum);
    
    if (m_crayonNum < 12)
    {
        m_Play3State = Play3_State::STATE_DRAWING;
        m_paletteState = Play3_PaletteState::STATE_CRAYON;
        
        if (m_crayonNum == 0)
        {
            m_crayonColor = Color3B(255, 201, 168);
        }
        else if (m_crayonNum == 1)
        {
            m_crayonColor = Color3B(255, 120, 0);
        }
        else if (m_crayonNum == 2)
        {
            m_crayonColor = Color3B(252, 88, 157);
        }
        else if (m_crayonNum == 3)
        {
            m_crayonColor = Color3B(244, 26, 3);
        }
        else if (m_crayonNum == 4)
        {
            m_crayonColor = Color3B(255, 220, 0);
        }
        else if (m_crayonNum == 5)
        {
            m_crayonColor = Color3B(52, 153, 20);
        }
        else if (m_crayonNum == 6)
        {
            m_crayonColor = Color3B(1, 146, 238);
        }
        else if (m_crayonNum == 7)
        {
            m_crayonColor = Color3B(48, 74, 126);
        }
        else if (m_crayonNum == 8)
        {
            m_crayonColor = Color3B(176, 76, 197);
        }
        else if (m_crayonNum == 9)
        {
            m_crayonColor = Color3B(121, 63, 28);
        }
        else if (m_crayonNum == 10)
        {
            m_crayonColor = Color3B(128, 128, 128);
        }
        else if (m_crayonNum == 11)
        {
            m_crayonColor = Color3B(17, 17, 17);
        }
    }
    else
    {
        if (m_crayonNum == 12)          //erase
        {
            m_Play3State = Play3_State::STATE_DRAWING;
            m_paletteState = Play3_PaletteState::STATE_ERASE;
        }
        else if (m_crayonNum == 13)     //erase all
        {
            
        }
    }
}


void Step3::drawCrayonAtPoint(Vec2 point)
{
    auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
    
    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
    
    
//    auto drawDot = Sprite::createWithSpriteFrameName("common_b01_c01_s1_t2_brush");
    
    //                        drawDot->setScale(2.0f);
    drawDot->setColor(m_crayonColor);
    drawDot->setPosition(point);
    MGTUtils::setPositionForParent(drawingArea, drawDot);
    
    
    //    const Vec2& center, float radius, float angle, unsigned int segments, float scaleX, float scaleY, const Color4F &color
//    auto drawDot = DrawNode::create();
//    drawDot->drawSolidCircle( point, 30, 0, 50, 1.0f, 1.0f, Color4F(m_crayonColor.r/255.0f, m_crayonColor.g/255.0f, m_crayonColor.b/255.0f, 1.0f));
//    drawDot->setPosition(point);
//    drawDot->setContentSize(Size(30, 30));
//    MGTUtils::setPositionForParent(drawingArea, drawDot);
    
    //    BlendFunc blendFunc = { GL_DST_COLOR, GL_ONE_MINUS_SRC_ALPHA };   // multiply
    //    BlendFunc blendFunc = { GL_ONE_MINUS_DST_COLOR, GL_ONE };         //
    //    brush->setBlendFunc(blendFunc);
    
    drawDot->visit();
    
    drawDot->retain();
    m_touchObjectSize = drawDot->getContentSize();
}

void Step3::eraseAtPoint(Vec2 point)
{
    auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
    
    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
//    auto drawDot = Sprite::createWithSpriteFrameName("common_b01_c01_s1_t2_brush");
    drawDot->setColor(Color3B::BLACK);
    drawDot->setPosition(point);
    
    MGTUtils::setPositionForParent(drawingArea, drawDot);
    
    //    const Vec2& center, float radius, float angle, unsigned int segments, float scaleX, float scaleY, const Color4F &color
//    auto drawDot = DrawNode::create();
//    drawDot->drawSolidCircle( point, 30, 0, 50, 1.0f, 1.0f, Color4F(0.0f, 0.0f, 0.0f, 1.0f));
//    drawDot->setPosition(point);
//    drawDot->setContentSize(Size(30, 30));
//    MGTUtils::setPositionForParent(drawingArea, drawDot);
    
    BlendFunc blendFunc = { GL_ZERO, GL_ONE_MINUS_SRC_ALPHA };
    drawDot->setBlendFunc(blendFunc);
    
    drawDot->visit();
    
    drawDot->retain();
    m_touchObjectSize = drawDot->getContentSize();
}

void Step3::createStickerBox()
{
    m_stickerBox = PsdParser::getPsdCommonSprite("common_b01_c01_s3_b_02", &m_psdDictionary);
    m_playContainer->addChild(m_stickerBox, kTagStep3StickerBox, kTagStep3StickerBox);
    
    for (int i = 0; i<7; i++)
    {
        int tag = i;
        
        std::string normal = StringUtils::format("common_b01_c01_s3_s_%02d_d.png", i+1);
        
        StickerObject* stickerItem = StickerObject::create(getCommonFilePath("img", normal).c_str());
        
        stickerItem->setTag(tag);
        
        std::string name = StringUtils::format("common_b01_c01_s3_s_%02d", i+1);
        Vec2 pos = PsdParser::getPsdPosition(name, &m_psdDictionary);
        stickerItem->setPosition(pos);
        MGTUtils::setPositionForParent(m_stickerBox, stickerItem);
        m_stickerBox->addChild(stickerItem, tag, tag);
        
        stickerItem->setNum(i);
        stickerItem->setUseCount(0);
        stickerItem->setEnabled(true);
        stickerItem->setImagePath(getCommonFilePath("img", normal).c_str());
        
        m_stickerItems.pushBack(stickerItem);
        
    }
    
    
    
    
    log("sticker");
}


void Step3::touchStickerItem(int num)
{
    auto tp = m_touch->getLocation();
    
    std::string file = StringUtils::format("common_b01_c01_s3_s_%02d", num+1);
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    pos = Vec2(tp.x, tp.y + 100);
    
    m_tempSticker = Sprite::create(getCommonFilePath("img", file.append("_b_n.png")));
    m_tempSticker->setPosition(pos);
    
    int tag = kTagStep3TempSticker+num;
    log("tag:%d", tag);
    m_playContainer->addChild(m_tempSticker, tag, tag);
    
    m_touchObjectSize = m_tempSticker->getContentSize();
}

void Step3::makeSticker()
{
    m_madeStickerCount++;
    
    int stickerNum = m_tempSticker->getTag() - kTagStep3TempSticker;
    
    
    float minX = m_stickerBox->getPosition().x - m_stickerBox->getContentSize().width/2 + m_touchObjectSize.width/2;
    float maxX = winSize.width - m_touchObjectSize.width/2 - 30;
    float minY = m_stickerBox->getPosition().y + m_stickerBox->getContentSize().height/2 + 20 + m_touchObjectSize.height/2;
    float maxY = winSize.height - m_touchObjectSize.height/2;
    
    log("MIN X:%f, MAX X:%f, MIN Y:%f MAX Y:%f", minX, maxX, minY, maxY);
    

    Vec2 pos = m_tempSticker->getPosition();
    log("pos X: %f, Y:%f", pos.x, pos.y);
    
    if (pos.x < minX)
    {
        pos.x = minX;
    }

    if (pos.x > maxX)
    {
        pos.x = maxX;
    }

    if (pos.y < minY)
    {
        pos.y = minY;
    }

    if (pos.y > maxY)
    {
        pos.y = maxY;
    }
    
    auto stickerItem = m_stickerItems.at(stickerNum);
    stickerItem->setUseCount(stickerItem->getUseCount()+1);
    
    std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_b_n.png", stickerNum+1);
    StickerObject* sticker = StickerObject::create(getCommonFilePath("img", file).c_str());
    
//    pos.x = (int) pos.x;
//    pos.y = (int) pos.y;
    
    
    
    m_step3DrawingRenderTexture->begin();

    auto drawingArea = (Sprite*)m_playContainer->getChildByTag(kTagStep3DrawingArea);
    sticker->setPosition(pos);
    MGTUtils::setPositionForParent(drawingArea, sticker);
    sticker->visit();
    
    m_step3DrawingRenderTexture->end();
    Director::getInstance()->getRenderer()->render();
    
//    sticker->setPosition(pos);
//    sticker->setScale(0.7f);
//    m_stickerContainer->addChild(sticker, m_madeStickerCount, m_madeStickerCount);
//    
//    sticker->runAction(EaseSineOut::create( ScaleTo::create(0.3f, 1.0f)));
//    
//    
//    sticker->setNum(stickerNum);
//    sticker->setUseCount(0);
//    sticker->setEnabled(true);
//    sticker->setImagePath(getCommonFilePath("img", file).c_str());
//    
//    m_madeStickers.pushBack(sticker);
//    
//    
//    
//    // delete btn
//    auto deleteItem = MenuItemImage::create(getCommonFilePath("img", "common_b01_c01_s3_btn_delete_n.png"),
//                                      getCommonFilePath("img", "common_b01_c01_s3_btn_delete_p.png"),
//                                      getCommonFilePath("img", "common_b01_c01_s3_btn_delete_p.png"),
//                                      CC_CALLBACK_1(Step3::stickerDeleteMenuCallback, this));
//    
//    
//    Vec2 deletepos = Vec2(sticker->getContentSize().width , sticker->getContentSize().height);
//    deleteItem->setPosition(deletepos);
//    
//    Menu* menu = Menu::create(deleteItem, nullptr);
//    menu->setPosition(Vec2::ZERO);
//    menu->setEnabled(false);
//    menu->setVisible(false);
//    sticker->addChild(menu, stickerobject::DELETEMENU, stickerobject::DELETEMENU);
//    
//    
//    // box area line
//
//    file = StringUtils::format("common_b01_c01_s3_s_%02d_line.png", stickerNum+1);
//    Sprite* areaBox = Sprite::create(getCommonFilePath("img", file));
//    areaBox->setAnchorPoint(Vec2::ZERO);
//    areaBox->setPosition(Vec2::ZERO);
//    sticker->addChild(areaBox, stickerobject::AREABOX, stickerobject::AREABOX);
//    
//    areaBox->setVisible(false);
////    Vec2 vertics[4] = { Vec2(0, 0), Vec2(sticker->getContentSize().width, 0), Vec2(sticker->getContentSize().width, sticker->getContentSize().height), Vec2(0, sticker->getContentSize().height) };
////    drawNode->drawPolygon(vertics, 4, Color4F(0, 0, 0, 0), 1, Color4F(111/255, 84/255, 84/255, 1));
//    
//    
//    
//    if (stickerItem->getIsEnabled() == false)
//    {
//        file = StringUtils::format("common_b01_c01_s3_s_%02d_d.png", stickerNum+1);
//        Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
//        stickerItem->setTexture(texture);
//    }
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_b01_c01_s3_sfx_sticker.mp3"));
}

void Step3::pressSticker()
{
    auto sticker = m_touchedSticker;
    
    
    std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_b_p.png", sticker->getNum()+1);
    Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
    sticker->setTexture(texture);
    
}

void Step3::selectSticker()
{
    auto sticker = m_touchedSticker;
    
    m_selectedSticker = sticker;
    
    Menu* deleteMenu = (Menu*)sticker->getChildByTag(stickerobject::DELETEMENU);
    deleteMenu->setVisible(true);
    deleteMenu->setEnabled(true);
    
    Sprite* areaBox = (Sprite*)sticker->getChildByTag(stickerobject::AREABOX);
    areaBox->setVisible(true);
    
//    sticker->runAction(TintTo::create(0.0f, 255.0f, 255.0f, 255.0f));
    
    std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_b_n.png", sticker->getNum()+1);
    Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
    sticker->setTexture(texture);
    
    
    int tag = m_madeStickerCount++;
    m_stickerContainer->reorderChild(sticker, tag);
    
    m_Play3State = Play3_State::STATE_SELECTED_STICKER;
}

void Step3::unselectSticker()
{
    
    if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
    {
        auto sticker = m_selectedSticker;
        
        Menu* deleteMenu = (Menu*)sticker->getChildByTag(stickerobject::DELETEMENU);
        deleteMenu->setVisible(false);
        deleteMenu->setEnabled(false);
        
        Sprite* areaBox = (Sprite*)sticker->getChildByTag(stickerobject::AREABOX);
        areaBox->setVisible(false);
        
//        sticker->runAction(TintBy::create(0.0f, 255.0f, 255.0f, 255.0f));
        
        std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_b_n.png", sticker->getNum()+1);
        Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
        sticker->setTexture(texture);
        
        
        m_selectedSticker = nullptr;
        m_Play3State = Play3_State::STATE_NONE;
    }
    else
    {
        if (m_Play3TouchState == Play3_TouchState::TOUCHSTATE_STICKER_MADE)
        {
//            m_touchedSticker->runAction(TintBy::create(0.0f, 255.0f, 255.0f, 255.0f));
            
            std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_b_n.png", m_touchedSticker->getNum()+1);
            Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
            m_touchedSticker->setTexture(texture);
            
            m_Play3State = Play3_State::STATE_NONE;
        }
    }
}

void Step3::deleteSticker(StickerObject* object)
{
    if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
    {
        auto sticker = object;
        m_madeStickers.eraseObject(sticker);
        
//        MGTUtils::removeAllchildren(sticker);
//        sticker->removeFromParentAndCleanup(true);
        sticker->setEnabled(false);
        sticker->setVisible(false);
        
        auto stickerItem = m_stickerItems.at(sticker->getNum());
        stickerItem->setUseCount(stickerItem->getUseCount()-1);
        
        if (stickerItem->getIsEnabled() == true)
        {
            std::string file = StringUtils::format("common_b01_c01_s3_s_%02d_n.png", stickerItem->getNum()+1);
            Texture2D* texture = Director::getInstance()->getTextureCache()->addImage(getCommonFilePath("img", file));
            stickerItem->setTexture(texture);
        }
        
        m_selectedSticker = nullptr;
        m_Play3State = Play3_State::STATE_NONE;
    }
}

void Step3::stickerDeleteMenuCallback(Ref* sender)
{
    MenuItem* deleteMenuItem = (MenuItem*)sender;
    Menu* menu = (Menu*)deleteMenuItem->getParent();
    StickerObject* sticker = (StickerObject*)menu->getParent();
    
    
    deleteSticker(sticker);
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_delete.mp3"));
}

void Step3::onStickerPressActivate(float dt)
{
    float elapsedTime = getCurrentTime() - m_touchBeganTime;
    
//    if (m_touch && !m_isTouchMoved)
//    {
//        selectSticker();
//    }
//    else
//    {
//        unselectSticker();
//    }
//    
    if (m_isTouchObject == true)
    {
        if(elapsedTime < TOUCH_THRESHOLD)
        {
            if (m_isTouchMoved == true )
            {
                unselectSticker();
                unschedule(CC_SCHEDULE_SELECTOR(Step3::onStickerPressActivate));
            }
        }
        else if(elapsedTime >= TOUCH_THRESHOLD)
        {
            if (m_isTouchMoved == false)
            {
                selectSticker();
            }
            else
            {
                unselectSticker();
            }
            unschedule(CC_SCHEDULE_SELECTOR(Step3::onStickerPressActivate));
        }
        
    }
    else
    {
        unschedule(CC_SCHEDULE_SELECTOR(Step3::onStickerPressActivate));
    }
}

void Step3::createSaveButton()
{
    // delete btn
    auto saveItem = MenuItemImage::create(getCommonFilePath("img", "common_b01_c01_s3_btn_save_n.png"),
                                            getCommonFilePath("img", "common_b01_c01_s3_btn_save_p.png"),
                                            getCommonFilePath("img", "common_b01_c01_s3_btn_save_d.png"),
                                            CC_CALLBACK_1(Step3::saveMenuCallback, this));
    
    
    Vec2 pos = PsdParser::getPsdPosition("common_b01_c01_s3_btn_save", &m_psdDictionary);
    saveItem->setPosition(pos);
    saveItem->setTag(kTagSaveButton);
    saveItem->setEnabled(false);
    
    Menu* menu = Menu::create(saveItem, nullptr);
    menu->setPosition(Vec2::ZERO);
    m_playContainer->addChild(menu, kTagSaveMenu, kTagSaveMenu);
}

void Step3::saveButtonActivation()
{
    Menu* menu = (Menu*)m_playContainer->getChildByTag(kTagSaveMenu);
    MenuItemImage* button = (MenuItemImage*)menu->getChildByTag(kTagSaveButton);
    button->setEnabled(true);
    
}

void Step3::saveMenuCallback(Ref* sender)
{
    log("save menu");
    
    if (m_Play3State == Play3_State::STATE_SELECTED_STICKER)
    {
        unselectSticker();
        m_Play3State = Play3_State::STATE_NONE;
    }
    
    onPause();
    
//    createCommonPopup(common_popup::DRAWING_SAVE, true);
    showCommonPopup(common_popup::DRAWING_SAVE);
    
    MGTSoundManager::getInstance()->playEffect(getCommonFilePath("snd", "common_sfx_btn_02.mp3"));
}

void Step3::captureImage()
{
    auto drawingArea = m_playContainer->getChildByTag(kTagStep3DrawingArea);
//    MGTUtils::setPositionForParent(drawingArea, m_object);
//    
//    MGTUtils::setPositionForParent(drawingArea, m_word);
//    
//    MGTUtils::setPositionForParent(drawingArea, m_alphabet);
    
    
    RenderTexture* renderTexture = RenderTexture::create(drawingArea->getContentSize().width, drawingArea->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    renderTexture->setPosition(Vec2(drawingArea->getPosition()));
    renderTexture->setVisible(false);
    m_playContainer->addChild(renderTexture, kTagStep3RenderTexture, kTagStep3RenderTexture);
    
    Vec2 pos;
    std::string file;
    Sprite* object = Sprite::createWithTexture(m_object->getTexture());
    object->setPosition(m_object->getPosition());
    MGTUtils::setPositionForParent(drawingArea, object);
    
    
    Sprite* word = Sprite::createWithTexture(m_word->getTexture());
    word->setPosition(m_word->getPosition());
    MGTUtils::setPositionForParent(drawingArea, word);
    
    Sprite* alphabet = Sprite::createWithTexture(m_alphabet->getTexture());
    alphabet->setPosition(m_alphabet->getPosition());
    alphabet->setScale(m_alphabet->getScale());
    MGTUtils::setPositionForParent(drawingArea, alphabet);
    alphabet->setFlippedY(true);
    
    
    Sprite* drawing = Sprite::createWithTexture(m_step3DrawingRenderTexture->getSprite()->getTexture());
    drawing->setFlippedY(true);
    drawing->setPosition(drawingArea->getPosition());
    MGTUtils::setPositionForParent(drawingArea, drawing);
    
    
    renderTexture->beginWithClear(1, 1, 1, 1);
    object->visit();
    drawing->visit();
    word->visit();
    alphabet->visit();
    renderTexture->end();
    Director::getInstance()->getRenderer()->render();
    
    
    std::string name = StringUtils::format("b01_c01_step3_draw%02d_complete.png", ProductManager::getInstance()->getCurrentAlphabet());
    
    renderTexture->saveToFile(name, Image::Format::PNG, true);
    //Add this function to avoid crash if we switch to a new scene.
    Director::getInstance()->getRenderer()->render();

    renderTexture->removeFromParentAndCleanup(true);
    
    
    
    log("--------------------CAPTURE IMAGE DRAWING_SAVE_COMPLETE");
    
    
    
    savePortfolio();
    
}

void Step3::savePortfolio()
{
    m_endContainer = Sprite::create();
    m_endContainer->setContentSize(Size(winSize.width, winSize.height));
    m_endContainer->setPosition(Vec2(winSize.width/2, winSize.height/2));
    m_endContainer->setVisible(false);
    this->addChild(m_endContainer, kTagEndContainer, kTagEndContainer);
    
    
    Vec2 pos;
    std::string file;
    pos = PsdParser::getPsdPosition("common_b01_c01_s3_word", &m_psdDictionary);
    file = StringUtils::format("b01_c01_s3_t1_%02d_word.png", ProductManager::getInstance()->getCurrentAlphabet());
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
    m_word->setPosition(pos-Vec2(120, 147));
    //    m_word->setScale(0.95f);
    m_word->setOpacity(0.0f);
    //    m_word->setColor(Color3B::RED);
    m_endContainer->addChild(m_word, kTagResultWord, kTagResultWord);
    
    
    //stencil
    pos = PsdParser::getPsdPosition("common_b01_c01_s3_mask", &m_psdDictionary);
    //    Sprite* stencil = Sprite::create(getCommonFilePath("img","common_b01_c01_s3_mask.png"));
    //    stencil->setPosition(pos);
    //    stencil->setTag( kTagResultStencil );
    //
    //    // clipper
    //    ClippingNode* clipper = ClippingNode::create();
    //    clipper->setContentSize( winSize );
    //    clipper->setAnchorPoint( Vec2(0.5, 0.5) );
    //    clipper->setPosition(Vec2( winSize.width/2, winSize.height/2 ));
    //    clipper->setAlphaThreshold(0.05f);
    //    clipper->setStencil(stencil);
    //    m_endContainer->addChild(clipper, kTagResultClipper, kTagResultClipper);
    
    std::string name = StringUtils::format("b01_c01_step3_draw%02d_complete.png", ProductManager::getInstance()->getCurrentAlphabet());
    file = FileUtils::getInstance()->getWritablePath();
    file.append(name);
    auto content = Sprite::create(file);
    content->setPosition(pos);
    content->setScale(0.95f);
    m_endContainer->addChild(content, kTagResultImg, kTagResultImg);
    
    
    file = StringUtils::format("b01_c01_s3_t1_%02d_alpha_end", ProductManager::getInstance()->getCurrentAlphabet());
    pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    Sprite* alphabet = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
    alphabet->setPosition(pos);
    m_endContainer->addChild(alphabet, kTagResultAlphabet, kTagResultAlphabet);
    
    
    Sprite* cover = Sprite::create(getCommonFilePath("img", "common_b01_c01_s3_result_cover.png"));
    cover->setPosition(Vec2(winSize.width/2, winSize.height/2));
    m_endContainer->addChild(cover, kTagResultCover, kTagResultCover);
    
    
    
    RenderTexture* renderTexture = RenderTexture::create(winSize.width, winSize.height, Texture2D::PixelFormat::RGBA8888);
    renderTexture->setPosition(Vec2(winSize.width/2, winSize.height/2));
    renderTexture->setVisible(false);
    m_endContainer->addChild(renderTexture, kTagResultRenderTexture, kTagResultRenderTexture);
    
    
    renderTexture->beginWithClear(1, 1, 1, 1);
    content->visit();
    cover->visit();
    alphabet->visit();
    renderTexture->end();
    Director::getInstance()->getRenderer()->render();
    
    
    //    renderTexture->saveToFile(filename, Image::Format::JPG, false);
    
    std::string filename = StringUtils::format("book1_%s.jpg", getAlphabetString( ProductManager::getInstance()->getCurrentAlphabet()).c_str());
    std::string path = MSLPManager::getInstance()->getPortfolioPath();
    std::string fullPath = StringUtils::format("%s%s",path.c_str(), filename.c_str());
    Image* image = renderTexture->newImage();
    
    image->saveToFile(fullPath);
    
    showCommonPopup(common_popup::DRAWING_SAVE_COMPLETE);
    
    this->runAction(Sequence::create(DelayTime::create(1.0f),
                                     CallFunc::create( CC_CALLBACK_0(Step3::showResult, this)),
                                     nullptr));
}


void Step3::showResult()
{
    playComplete();
    
    m_eState = step3::STATE_END;
    
    m_touchEnabled = true;
    
    DeviceUtilManager::getInstance()->hideSavePopup();
    
    
    m_navigation->setVisibleButton(true, navi::eNavigationButton_Next);
    m_navigation->playButtonAction(navi::eNavigationButton_Next);
    
    MGTUtils::removeAllchildren(m_playContainer);
    
    m_endContainer->setVisible(true);
    
    std::string filename = StringUtils::format("book1_%s.jpg", getAlphabetString( ProductManager::getInstance()->getCurrentAlphabet()).c_str());
    std::string path = MSLPManager::getInstance()->getPortfolioPath();
    path.append(filename);
    
    MSLPManager::getInstance()->finishPortfolio(ProductManager::getInstance()->getCurrentAlphabet(), path);
}

#pragma mark -

void Step3::nextPlay()
{
    m_count = 0;
    m_playNum++;
    
    if (m_playNum == 2)
    {
        m_gafObject->stop();
        MGTUtils::removeAllchildren(m_gafObject);
        m_gafObject->removeFromParentAndCleanup(true);
        m_gafObject = nullptr;
        
        
        createPlay2();
    }
    else if (m_playNum == 3)
    {
        createPlay3();
    }
}

void Step3::showAffordance()
{
    Step3_Base::showAffordance();
    
    if (m_affordance)
    {
        playAffordance(m_affordance);
    }
}

void Step3::hideAffordance()
{
    Step3_Base::hideAffordance();
    
    if (m_affordance)
    {
        stopAffordance(m_affordance);
    }
}

void Step3::interactionStart()
{
    m_count++;
    m_touchEnabled = true;
    
    if (m_playNum == 1)
    {
//        blinkAlphabetAnimation();

    }
    else if (m_playNum == 2)
    {
        m_navigation->setVisibleButton(true, navi::eNavigationButton_Guide);
        m_navigation->setEnableButton(true, navi::eNavigationButton_Guide);
    }
    else if (m_playNum == 3)
    {
        
    }
    
    showAffordance();
}

void Step3::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    if (m_playNum == 2)
    {
        if (m_count <= m_drawObjects.size())
        {
            setDrawWrite();
        }
    }
    
    startAffordanceTimer();
}

void Step3::interactionComplete()
{
    m_touchEnabled = false;
    

//    if (m_playNum == 1)
//    {
//        nextPlay();
//    }
//    else if(m_playNum == 2)
//    {
//        nextPlay();
//    }
//    else if(m_playNum == 3)
//    {
//        nextPlay();
//    }
}


void Step3::finishAnimation()
{
    
}


void Step3::playStart()
{
    Step3_Base::playStart();
    
    interactionStart();
}

void Step3::playComplete()
{
    Step3_Base::playComplete();
}


std::string Step3::getAlphabetString(int num)
{
    std::string alphabets[26] = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    
    return alphabets[num-1];
}
