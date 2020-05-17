#include "Step1_Type2.h"
#include <algorithm>

enum
{
    kTagWord = 0,
    kTagAlphabet,
    kTagDrawObject = 10,
    
    kTagFinish_Animation,
    kTagFinish_Sentence,
};


namespace drawobject {
    enum
    {
        OBJ = 0,
        CLIPPER,
        STENCIL,
        CONTENT,
        LINE
    };
}


Step1_Type2::Step1_Type2():
m_count(0),
m_objectImageData(nullptr),
m_brushImageData(nullptr),
m_brushRendertexture(nullptr)
{

}

Step1_Type2::~Step1_Type2()
{
}

Scene* Step1_Type2::createScene()
{
    auto scene = Scene::create();

    auto layer = Step1_Type2::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Step1_Type2::init()
{
    if ( !Step1_Base::init() )
    {
        return false;
    }
    
    std::string file = StringUtils::format("b01_c01_s1_t2_%02d.json", ProductManager::getInstance()->getCurrentAlphabet());
    PsdParser::parseToPsdJSON(file, &m_psdDictionary);
    
    initView();
    
    return true;
}


void Step1_Type2::onEnter()
{
    Step1_Base::onEnter();
    
    onViewLoad();
}

void Step1_Type2::onExit()
{
    Step1_Base::onExit();
    
}

void Step1_Type2::onViewLoad()
{
    Step1_Base::onViewLoad();
    
}

void Step1_Type2::onViewLoaded()
{
    Step1_Base::onViewLoaded();
}


#pragma mark - touch

bool Step1_Type2::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    if(Step1_Base::onTouchBegan(pTouch, pEvent))
    {
        return false;
    }
    
    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false || MGTSoundManager::getInstance()->isAnyNarrationPlaying()) {
        return false;
    }
    
    if (m_eState == step1::STATE_PLAY)
    {
        
        
        if (isTouchDrawObject(tp))
        {
            log("touch");
            stopAffordanceTimer();
            
            hideAffordance();
            
            //        stopIndicate();
            eraseCoverAtPoint(tp);
            
            
            m_isTouchObject = true;
            
            return true;
        }
        
        if (MGTUtils::hitTestPoint(m_word, tp, false))
        {
            playWordSound();
            
        }
    }
    
    return false;
}

void Step1_Type2::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchMoved(pTouch, pEvent);
    
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
    
    
    Vec2 tp = pTouch->getLocation();
    Vec2 prevTp = pTouch->getPreviousLocation();
    
    auto currentTouch = convertToNodeSpace(tp);
    auto prevTouch = convertToNodeSpace(prevTp);
    
    if (m_isTouchObject)
    {
        float distance = currentTouch.getDistance(prevTp);
        
        if(distance > 1)
        {
            int d = (int)distance;
//
//            for (int i = 0; i < d; ++i)
//            {
//                brush = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
//                m_temp_brushs.pushBack(brush);
//            }
//            
//            
            for (int i = 0; i < d; i++)
            {
                float difx = prevTouch.x - currentTouch.x;
                float dify = prevTouch.y - currentTouch.y;
                float delta = (float)i / distance;
                
                Vec2 pos = Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta));
                eraseCoverAtPoint(pos);
                
//                m_temp_brushs.at(i)->setPosition(Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta)));
//                MGTUtils::setPositionForParent(_currentDrawObject, m_temp_brushs.at(i));
//                m_temp_brushs.at(i)->visit();
//            
//
//                m_brushes.pushBack(brush);
            }
        }
    }

}

void Step1_Type2::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Step1_Base::onTouchEnded(pTouch, pEvent);
    Vec2 tp = pTouch->getLocation();
    
    if (m_isTouchObject)
    {
        if (checkIntersectionPercentage())
        {
            eraseComplete();
        }
        else
        {
            startAffordanceTimer();
        }
    }
    
    m_isTouchObject = false;
}

void Step1_Type2::onTouchCancelled(Touch *touch, Event *event)
{
    Step1_Base::onTouchCancelled(touch, event);
}


void Step1_Type2::onNarrationFinishedCallback(std::string fileName)
{
    Step1_Base::onNarrationFinishedCallback(fileName);
    
    log("Narration Finished fileName : %s", fileName.c_str());
    
    if (fileName == m_strWordSound)
    {
        
        if(m_eState == step1::STATE_PLAY)
        {
            if (m_count == 0)
            {
                m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                                            CallFunc::create( CC_CALLBACK_0(Step1_Type2::interactionStart, this)),
                                                            nullptr));
            }
        }
        else if(m_eState == step1::STATE_PLAY_COMPLETE)
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                             CallFunc::create( CC_CALLBACK_0(Step1_Base::createEnding, this)),
                                             nullptr));
        }
    }
    else if(fileName == m_strPhonicsSound)
    {
        if (m_count == 1 || m_count == 2 || m_count == 3 )
        {
            m_playContainer->runAction(Sequence::create(DelayTime::create(0.2f),
                                             CallFunc::create( CC_CALLBACK_0(Step1_Type2::nextInteraction, this)),
                                             nullptr));
        }
    }
}


#pragma mark gaf delegate function
void Step1_Type2::onFinishSequence( GAFObject * object, const std::string& sequenceName )
{
    Step1_Base::onFinishSequence(object, sequenceName);
}

void Step1_Type2::onFramePlayed(GAFObject *object, uint32_t frame)
{
    Step1_Base::onFramePlayed(object, frame);
}

void Step1_Type2::onTexturePreLoad(std::string& path)
{
    Step1_Base::onTexturePreLoad(path);
}


#pragma mark navi touch override function
void Step1_Type2::onTouchedNavigationButtonAtExit()
{
    Step1_Base::onTouchedNavigationButtonAtExit();
}

void Step1_Type2::onTouchedNavigationButtonAtNext()
{
    Step1_Base::onTouchedNavigationButtonAtNext();
}


void Step1_Type2::onTouchedPopupButtonAtNo()
{
    Step1_Base::onTouchedPopupButtonAtNo();
}

void Step1_Type2::onTouchedPopupButtonAtYes()
{
    Step1_Base::onTouchedPopupButtonAtYes();
}


#pragma mark step1_type2
void Step1_Type2::initView()
{
    m_playContainer = Sprite::create();
    m_playContainer->setTextureRect( Rect(0, 0, winSize.width, winSize.height) );
    m_playContainer->setColor(Color3B(255, 247, 233));
//    m_playContainer->setAnchorPoint(Vec2::ZERO);
    m_playContainer->setPosition(winSize.width/2, winSize.height/2);
    this->addChild(m_playContainer, kTagPlayContainer, kTagPlayContainer);
    
    std::string file = StringUtils::format("b01_c01_s1_t2_%02d_word", ProductManager::getInstance()->getCurrentAlphabet());
    
    Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
    m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_off.png")));
    m_word->setPosition(pos);
    m_playContainer->addChild(m_word, kTagWord, kTagWord);
    
    createObject();
}

void Step1_Type2::createObject()
{
 
    std::vector<int> _parts;
    for (int i = 0; i<4; i++)
    {
        _parts.push_back(i);
    }
    std::random_shuffle(_parts.begin(), _parts.end(), MGTUtils::random_user);
    
    
    for (int i = 0; i<4; i++)
    {
        int tag = kTagDrawObject + i;
        
        int objNum = _parts.at(i)+1;
        log("objNum:%d", objNum);
        
        std::string file;
        
        file = StringUtils::format("b01_c01_s1_t2_%02d_object%02d",ProductManager::getInstance()->getCurrentAlphabet(), objNum);
        Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
        
        
        DrawObject* drawObject = DrawObject::createWithFullPath(file.append("_off.png").c_str());
        drawObject->setPosition(pos);
        m_playContainer->addChild(drawObject, tag, tag);
        
        drawObject->setNum(objNum);
        drawObject->setImagePath(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", file).c_str());
        
        
        file = StringUtils::format("b01_c01_s1_t2_%02d_area%02d.png", ProductManager::getInstance()->getCurrentAlphabet(), objNum);
        drawObject->setAreaImagePath(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", file).c_str());
        
        
        file = StringUtils::format("b01_c01_s1_t2_%02d_line%02d.png", ProductManager::getInstance()->getCurrentAlphabet(), objNum);
        
        Sprite* line = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file));
        line->setPosition(pos);
        MGTUtils::setPositionForParent(drawObject, line);
        line->setColor(Color3B(226, 221, 213));
        drawObject->addChild(line, drawobject::LINE, drawobject::LINE);
        
        
        
        // clipper
        ClippingNode* clipper = ClippingNode::create();
        clipper->setContentSize( drawObject->getContentSize() );
        clipper->setAnchorPoint( Vec2(0.5, 0.5) );
        clipper->setPosition(pos);
        MGTUtils::setPositionForParent(drawObject, clipper);
        clipper->setAlphaThreshold(0.05f);
        
        drawObject->addChild(clipper, drawobject::CLIPPER, drawobject::CLIPPER);
        drawObject->setCoverClipper( clipper );
        
        
        // stencil
        Sprite* stencil = Sprite::create();
        stencil->setContentSize(drawObject->getContentSize());
        stencil->setPosition(pos);
        MGTUtils::setPositionForParent(drawObject, stencil);
        clipper->setStencil( stencil );

        drawObject->setEraseStencil( stencil );
        
        
        // content
        file = StringUtils::format("b01_c01_s1_t2_%02d_object%02d_on.png", ProductManager::getInstance()->getCurrentAlphabet(), objNum);
        auto cover = MGTSprite::create(getFilePath(ResourceType::IMAGE, "img", file).c_str());
        cover->setPosition(pos);
        MGTUtils::setPositionForParent(drawObject, cover);
        clipper->addChild(cover, drawobject::CONTENT, drawobject::CONTENT);
        
        
        m_drawObjects.pushBack(drawObject);
        
        log("for tag :%d", drawObject->getTag());
    }
}

bool Step1_Type2::isTouchDrawObject(Vec2 point)
{
    std::string imgPath = m_currentDrawObject->getAreaImagePath();
    
    Image* image = MGTResourceUtils::getInstance()->getImageData(imgPath);;
    
    if (MGTUtils::hitTestPointExact(m_currentDrawObject, image, point, false))
    {
        return true;
    }
    
    return false;
}

void Step1_Type2::eraseCoverAtPoint(Vec2 point)
{
    
    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
    drawDot->setScale(2.0f);
//    drawDot->setOpacity(50.0f);
    drawDot->setPosition( point );
    MGTUtils::setPositionForParent(m_currentDrawObject, drawDot);
    
    
//    const Vec2& center, float radius, float angle, unsigned int segments, float scaleX, float scaleY, const Color4F &color
//    
//    auto drawDot = DrawNode::create();
//    drawDot->drawSolidCircle( point, 70, 0, 50, 1.0f, 1.0f, Color4F(1.0f, 1.0f, 1.0f, 1.0f));
//    drawDot->setPosition(point);
//    MGTUtils::setPositionForParent(m_currentDrawObject, drawDot);
//    
    
    m_currentDrawObject->getEraseStencil()->addChild(drawDot);
    drawDot->retain();
    
    m_brushes.push_back(drawDot->getPosition());
}


void Step1_Type2::createRenderTexture()
{
    if(m_brushRendertexture)
    {
        m_brushRendertexture->removeFromParentAndCleanup(true);
    }
    
    m_brushRendertexture = RenderTexture::create(m_currentDrawObject->getContentSize().width, m_currentDrawObject->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
    m_brushRendertexture->setPosition(Vec2(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2));
    m_brushRendertexture->setVisible(false);
    this->addChild(m_brushRendertexture, 100, 100);
}


bool Step1_Type2::checkIntersectionPercentage()
{
    
//    std::string file = StringUtils::format("b01_c01_s1_t2_02_object%02d_white", m_currentDrawObject->getNum());
//    Sprite* sp = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append(".png")));
//    sp->setColor(Color3B::RED);
//    sp->setPosition(m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2);
    
    
    
    
    m_brushRendertexture->beginWithClear(0, 0, 0, 0);
    
//    log("brush size:%d", m_brushes.size());
    
//    Vector<Node*> tempBrushes;
    for (int i = 0; i< m_brushes.size(); i++)
    {
        Sprite* drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
        drawDot->setColor(Color3B::GREEN);
        drawDot->setScale(2.0f);
        drawDot->setPosition(m_brushes.at(i));
        drawDot->visit();
        
//        auto drawDot = DrawNode::create();
//        drawDot->drawSolidCircle( Vec2::ZERO, 70, 0, 50, 1.0f, 1.0f, Color4F(0.0f, 1.0f, 0.0f, 1.0f));
//        drawDot->setPosition(m_brushes.at(i));
//        drawDot->visit();
//        
////        tempBrushes.pushBack(drawDot);
        drawDot->retain();
    }

    m_brushRendertexture->end();
    Director::getInstance()->getRenderer()->render();
    
    saveTempImageFile();
    
    
    m_brushImageData = MGTResourceUtils::getInstance()->getImageData(getTempImageFile(), true);
//    m_brushImageData->initWithImageFile(getTempImageFile());
    
    
    
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
            
            Color4B brushColor = Color4B(0, 255, 0, 255);
            
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
    if (percent > 70)
    {
        ret = true;
    }
    return ret;
}

void Step1_Type2::saveTempImageFile()
{
    
    std::string name = "b01_c01_step1_type2.png";
    
    m_brushRendertexture->saveToFile(name, Image::Format::PNG, true);
    //Add this function to avoid crash if we switch to a new scene.
    Director::getInstance()->getRenderer()->render();
    
    
//    std::string file = FileUtils::getInstance()->getWritablePath();
//    file.append(name);
//    auto sprite = Sprite::create(file);
//    this->addChild(sprite, 100);
//    sprite->setPosition(Vec2(winSize.width - sprite->getContentSize().width/2, sprite->getContentSize().height/2));
    
}

std::string Step1_Type2::getTempImageFile()
{
    std::string file = FileUtils::getInstance()->getWritablePath();
    file.append("b01_c01_step1_type2.png");
    return file;
}


void Step1_Type2::eraseComplete()
{
    m_touchEnabled = false;
    
    MGTSprite* line = (MGTSprite*)m_currentDrawObject->getChildByTag(drawobject::LINE);
    line->stopAllActions();
    line->setOpacity(0.0f);
    
    
    auto drawDot = Sprite::create(getCommonFilePath("img", "common_b01_c01_s1_t2_brush.png"));
    drawDot->setScale(0.0f);
    drawDot->setPosition( m_currentDrawObject->getContentSize().width/2, m_currentDrawObject->getContentSize().height/2 );
    m_currentDrawObject->getEraseStencil()->addChild(drawDot);
    
    auto scaleT = EaseSineOut::create( ScaleTo::create(0.5f, 10.0f));

    auto seq = Sequence::create(scaleT,
                                CallFunc::create( CC_CALLBACK_0(Step1_Type2::objectShowAll, this)),
                                CallFunc::create( CC_CALLBACK_0(Step1_Type2::wordAnimation, this)),
                                nullptr);

    drawDot->runAction(seq);
    
}

void Step1_Type2::objectShowAll()
{
    MGTUtils::removeAllchildren(m_currentDrawObject);
    
    std::string file = StringUtils::format("b01_c01_s1_t2_%02d_object%02d_on.png", ProductManager::getInstance()->getCurrentAlphabet(), m_currentDrawObject->getNum());
    m_currentDrawObject->setTexture(Director::getInstance()->getTextureCache()->addImage(getFilePath(ResourceType::IMAGE, "img", file)));
}

void Step1_Type2::wordAnimation()
{
    if (m_count < 4)
    {
        if (m_count == 1)
        {
            m_word->removeFromParent();
            
            std::string file;
            file = StringUtils::format("b01_c01_s1_t2_%02d_word_02", ProductManager::getInstance()->getCurrentAlphabet());
            m_word = PsdParser::getPsdSprite(file, &m_psdDictionary);
            m_playContainer->addChild(m_word, kTagWord, kTagWord);
            
            file = StringUtils::format("b01_c01_s1_t2_%02d_word_01", ProductManager::getInstance()->getCurrentAlphabet());
            m_alphabet = PsdParser::getPsdSprite(file, &m_psdDictionary);
            
            if (ProductManager::getInstance()->getCurrentAlphabet() == 24)
            {
                MGTUtils::setAnchorPointForPosition(m_alphabet, Vec2(0.0f, 0.0f));
            }
            else
            {
                MGTUtils::setAnchorPointForPosition(m_alphabet, Vec2(1.0f, 0.0f));
            }
            
            m_alphabet->setScale(1.0f);
            m_playContainer->addChild(m_alphabet, kTagAlphabet, kTagAlphabet);
        }
        
        
        auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.6f));
        auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 1.0f));
        
        auto action = Sequence::create(scaleT1,
                                       scaleT2,
                                       nullptr);
        
        
        m_alphabet->runAction(action);
        
        playPhonicsSound();
    }
    else if(m_count == 4)
    {
        m_word->removeFromParent();
        m_alphabet->removeFromParentAndCleanup(true);
        
        
        std::string file = StringUtils::format("b01_c01_s1_t2_%02d_word", ProductManager::getInstance()->getCurrentAlphabet());
        Vec2 pos = PsdParser::getPsdPosition(file, &m_psdDictionary);
        m_word = Sprite::create(getFilePath(ResourceType::IMAGE, "img", file.append("_on.png")));
        m_word->setPosition(pos);
        
        m_playContainer->addChild(m_word, kTagWord, kTagWord);
        
        auto scaleT1 = EaseSineOut::create(ScaleTo::create(0.25f, 1.6f));
        auto scaleT2 = EaseSineIn::create(ScaleTo::create(0.25f, 1.0f));
        
        auto action = Sequence::create(scaleT1,
                                       scaleT2,
                                       nullptr);
        
        
        m_word->runAction(action);
        
        playWordSound();
        playComplete();
    }
    
}




void Step1_Type2::showAffordance()
{
    Step1_Base::showAffordance();
    
    MGTSprite* line = (MGTSprite*)m_currentDrawObject->getChildByTag(drawobject::LINE);
    
    if (line)
    {
        playAffordance(line);
    }
}

void Step1_Type2::hideAffordance()
{
    Step1_Base::hideAffordance();
    
    MGTSprite* line = (MGTSprite*)m_currentDrawObject->getChildByTag(drawobject::LINE);
    
    if (line)
    {
        stopAffordance(line, true);
    }
}


void Step1_Type2::interactionStart()
{
    m_touchEnabled = true;
    
    if (m_count == 0)
    {
        m_count++;
        
        m_currentDrawObject = m_drawObjects.at( m_drawObjects.size()-m_count);
        
//        m_objectImageData = new Image();
//        m_objectImageData->initWithImageFile( DRMManager::getInstance()->getEntryName( m_currentDrawObject->getImagePath()));
        
        m_objectImageData = MGTResourceUtils::getInstance()->getImageData(m_currentDrawObject->getImagePath());
        
        createRenderTexture();
        
        showAffordance();
    }
}

void Step1_Type2::nextInteraction()
{
    m_count++;
    m_touchEnabled = true;
    
    if (m_count <= 4)
    {
        m_brushes.clear();
        
//        m_currentDrawObject = m_drawObjects.at(m_count-1);
        m_currentDrawObject = m_drawObjects.at( m_drawObjects.size()-m_count);
        m_playContainer->reorderChild(m_currentDrawObject, 100);
        
        
        if (m_objectImageData)
        {
            CC_SAFE_RELEASE(m_objectImageData);
        }
        
        m_objectImageData = MGTResourceUtils::getInstance()->getImageData(m_currentDrawObject->getImagePath());
        
        createRenderTexture();
        
//        indicateObject();
        
        showAffordance();
    }
}

void Step1_Type2::playStart()
{
    Step1_Base::playStart();
}

void Step1_Type2::playComplete()
{
    Step1_Base::playComplete();
}


