
#include "Draw_Sprite.h"
#include "MGTSoundManager.h"
#include "MGTUtils.h"
#include "MGTResourceUtils.h"

Draw_Sprite::Draw_Sprite() :
_subImageSprite(nullptr)
{
    
}

Draw_Sprite::~Draw_Sprite()
{
    delete _imageData;
    CC_SAFE_RELEASE(_renderTexture);
    CCLOG("~GriaffeSprite");
}

Draw_Sprite *Draw_Sprite::create(ColorType type, const std::string beforeImagePath, const std::string afterImagePath, std::string subImagePath)
{
    auto ret = new (std::nothrow)Draw_Sprite();
    if (ret && ret->init(type, beforeImagePath, afterImagePath, subImagePath))
    {
        ret->autorelease();
        return ret;
    }
    CC_SAFE_RELEASE(ret);
    return nullptr;
}

bool Draw_Sprite::init(ColorType type, const std::string beforeImagePath, const std::string afterImagePath, std::string subImagePath)
{
    if (Node::init())
    {
        _beforeImagePath = beforeImagePath;
        _afterImagePath = afterImagePath;
        _subImagePath = subImagePath;
        _isSelected = false;
        _penColor =  MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", "d_b001_day1_ta03_pointer_blue.png");
        _colorType = type;
        _isCorrect = false;
        
        switch (getColorType())
        {
            case COLOR_YELLOW :
                setColorRGB(Color4B(0,0,0,0));
                break;
            case COLOR_BLUE :
                setColorRGB(Color4B(0,0,0,0));
                break;
            case COLOR_PINK :
                setColorRGB(Color4B(0,0,0,0));
                break;
            case COLOR_GREEN :
                setColorRGB(Color4B(0,0,0,0));
                break;
            case COLOR_RED :
                setColorRGB(Color4B(237,28,36,255));
                break;
            default:
                break;
        }
        
        _imageData = new Image();
        if(!_imageData->initWithImageFile(afterImagePath))
        {
            return false;
        }
        
        //터치잡기
        auto touchListener = EventListenerTouchAllAtOnce::create();
        touchListener->onTouchesMoved = CC_CALLBACK_2(Draw_Sprite::onTouchesMoved, this);
        touchListener->onTouchesEnded = CC_CALLBACK_2(Draw_Sprite::onTouchesEnded, this);
        touchListener->onTouchesCancelled = CC_CALLBACK_2(Draw_Sprite::onTouchesCancelled, this);
        Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(touchListener, this);
        
        // 조각 이미지
        _currentImageSprite = Sprite::create(beforeImagePath);
        _currentImageSprite->setPosition(_currentImageSprite->getContentSize()/2);
        this->addChild(_currentImageSprite);
        
        this->setContentSize(_currentImageSprite->getContentSize());
        this->setAnchorPoint(Vec2(0.5f, 0.5f));
        
        //조각 서브 이미지
        if (!(_subImagePath.compare("") == 0))
        {
            _subImageSprite = Sprite::create(_subImagePath);
            _subImageSprite->setPosition(_subImageSprite->getContentSize()/2);
            this->addChild(_subImageSprite,3);
        }
        
        //그릴 랜더 텍스쳐
        _renderTexture = RenderTexture::create(this->getContentSize().width, this->getContentSize().height, Texture2D::PixelFormat::RGBA8888);
        _renderTexture->retain();
        _renderTexture->setPosition(this->getAnchorPointInPoints());
        
        //클리핑
        auto clipper = ClippingNode::create(_currentImageSprite);
        clipper->setAlphaThreshold(0.5f);
        clipper->addChild(_renderTexture);
        this->addChild(clipper,2);
        
        return true;
    }
    return false;
}

void Draw_Sprite::onTouchesMoved(const std::vector<Touch*>& touches, Event *unused_event)
{
    if (!getSelected())
    {
        return;
    }
    
    Rect rect = this->getBoundingBox();
    auto touch = touches[0];
    auto currentTouch = convertToNodeSpace(touch->getLocation());
    auto prevTouch = convertToNodeSpace(touch->getPreviousLocation());
//    CCLOG("Sprite Moved:: %d :: %f , %f", this->getTag(), currentTouch.x , currentTouch.y);
    if(tapsOnNonTransparent(touches[0]->getLocation()))
    {
        
        //팬보이기
        _renderTexture->begin();
        float distance = currentTouch.getDistance(prevTouch);
        if(distance > 1)
        {
            int d = (int)distance;
            _brushs.clear();
            for (int i = 0; i < d; ++i)
            {
                Sprite *sprite = Sprite::create(_penColor.c_str());
                _brushs.pushBack(sprite);
            }
            
            
            for (int i = 0; i < d; i++)
            {
                float difx = prevTouch.x - currentTouch.x;
                float dify = prevTouch.y - currentTouch.y;
                float delta = (float)i / distance;
                _brushs.at(i)->setPosition(Vec2(currentTouch.x + (difx * delta), currentTouch.y + (dify * delta)));
                _brushs.at(i)->visit();
            }
        }
        _renderTexture->end();

    }
}

void Draw_Sprite::onTouchesEnded(const std::vector<Touch*>& touches, Event *unused_event)
{
    if (!getSelected())
    {
        return;
    }
    
    if(checkDraw() && !getCorrect())
    {
        setCorrect();
        MGTSoundManager::getInstance()->playEffect(MGTResourceUtils::getInstance()->getCommonFilePath("snd", "common_sfx_inobj.mp3"));
        
        this->runAction(Sequence::create(
                                         ScaleTo::create(0.15f, 1.2f),
                                         ScaleTo::create(0.15f, 1),
                                         nullptr));
    }
    
    getDelegate()(this);
}

void Draw_Sprite::onTouchesCancelled(const std::vector<Touch*>&touches, Event *unused_event)
{

}

bool Draw_Sprite::checkDraw()
{
    if (!_renderTexture)
    {
        return false;
    }
    
    auto image = _renderTexture->newImage();
    int totalArea = image->getDataLen()/4;
    int drawArea = 0;
    int drawingArea = 0;
    int TranArea = 0;
    
    
    for (int i = 0; image->getDataLen()/4 > i; i++)
    {
        unsigned char *pixel = image->getData() + (4 * i);
//        CCLOG("%u, %u, %u ,%u", pixel[0], pixel[1], pixel[2], pixel[3]);
        if (isTestPixel(image->getData() + (4 * i), getColorRGB().r, getColorRGB().g, getColorRGB().b , getColorRGB().a))
        {
            drawingArea++;
        }
        else if(isZeroPixel(image->getData() + (4 * i)))
        {
            TranArea++;
        }
    }
    CC_SAFE_RELEASE(image);
//    drawArea = totalArea - TranArea;
    CCLOG("Total == %d, DrawColor == %d , Persent == %f%%", totalArea, drawingArea, (float)drawingArea/totalArea * 100);
    //전체 - 투명 = 토탈, 드로우한 부분 = 드로우컬러 , 토탈/(토탈-드로우한컬러) = 퍼센트
    //일단 토탈로 바꾸자 ( 투명영역신경안쓰기 )
    
    if ((float)drawingArea/totalArea * 100 > 30)
    {
        return true;
    }
    else
    {
        clearRenderTexture();
        return false;
    }
}

bool Draw_Sprite::isTestPixel(unsigned char *pixel, int r, int g, int b, int a)
{
    //0, 173, 255, 255 파랑
    return r == pixel[0] && g == pixel[1] && b == pixel[2] && a == pixel[3];
}

bool Draw_Sprite::tapsOnNonTransparent(const Vec2 &tap)
{
    if (!this->boundingBox().containsPoint(tap))
        return false;
    
    auto nodeTap = this->convertToNodeSpace(tap);
    unsigned x = unsigned(nodeTap.x) % _imageData->getWidth();
    unsigned y = unsigned(_imageData->getHeight() - nodeTap.y) % _imageData->getHeight();
    unsigned index = x + y * _imageData->getWidth();
    ssize_t dataLen = _imageData->getDataLen();
    CCAssert(index < dataLen, "index is bigger than image size.");
    unsigned char *pixel = _imageData->getData() + (4 * index);
    return !isZeroPixel(pixel);
}

void Draw_Sprite::setCorrect()
{
    _currentImageSprite->setSpriteFrame(Sprite::create(_afterImagePath)->getSpriteFrame());
    if (_subImageSprite)
    {
        this->removeChild(_subImageSprite);
    }
    _isCorrect = true;
}

void Draw_Sprite::clearRenderTexture()
{
    _renderTexture->clear(0, 0, 0, 0);
}

bool Draw_Sprite::isZeroPixel(unsigned char *pixel)
{
//    CCLOG("%u, %u, %u ,%u", pixel[0], pixel[1], pixel[2], pixel[3]);
    return 0 == pixel[0] && 0 == pixel[1] && 0 == pixel[2] && 0 == pixel[3];
}
