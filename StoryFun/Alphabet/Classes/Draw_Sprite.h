
#ifndef __Draw_Sprite__
#define __Draw_Sprite__

#include "cocos2d.h"

USING_NS_CC;

typedef enum
{
    COLOR_NONE,
    COLOR_YELLOW,
    COLOR_BLUE,
    COLOR_PINK,
    COLOR_GREEN,
    COLOR_RED
} ColorType;

typedef struct _imageData
{
public:
    _imageData(ColorType colorType, Vec2 position, std::string boforeImage, std::string afterImage, std::string subImagePath = "")
    :_colorType(colorType),
    _imagePosition(position),
    _beforeImage(boforeImage),
    _afterImage(afterImage),
    _subImage(subImagePath)
    {};
    ColorType _colorType;
    Vec2 _imagePosition;
    std::string _beforeImage;
    std::string _afterImage;
    std::string _subImage;
}ImageData;

class Draw_Sprite : public Node
{
public:
    Draw_Sprite();
    virtual ~Draw_Sprite();
    
    static Draw_Sprite *create(ColorType type, const std::string beforeImagePath, const std::string afterImagePath, std::string subImagePath = "");
    bool init(ColorType type, const std::string beforeImagePath, const std::string afterImagePath, std::string subImage = "");
    
//    virtual bool onTouchBegan(Touch *touch, Event *unused_event);
    virtual void onTouchesMoved(const std::vector<Touch*>& touches, Event *unused_event);
    virtual void onTouchesEnded(const std::vector<Touch*>& touches, Event *unused_event);
    virtual void onTouchesCancelled(const std::vector<Touch*>&touches, Event *unused_event);
    
public:
    bool tapsOnNonTransparent(const Vec2 &tap);
    void setCorrect();
    void clearRenderTexture();
    bool checkDraw();
    
private:
    bool isZeroPixel(unsigned char *pixel);
    
    

    bool isTestPixel(unsigned char *pixel, int r, int g, int b, int a);
private:
    typedef std::function<void(Draw_Sprite *)> Delegate_t;
    
    CC_SYNTHESIZE_READONLY(std::string, _beforeImagePath, BeforeImagePath);
    CC_SYNTHESIZE_READONLY(std::string, _afterImagePath, AfaterImagePath);
    CC_SYNTHESIZE_READONLY(std::string, _subImagePath, SubImagePath);
    CC_SYNTHESIZE(Sprite *, _currentImageSprite, CurrentImageSprite);
    CC_SYNTHESIZE(Sprite *, _subImageSprite, SubImageSprite);
    CC_SYNTHESIZE(std::string, _penColor, PenColor);
    CC_SYNTHESIZE(bool, _isSelected, Selected);
    CC_SYNTHESIZE(bool, _isCorrect, Correct);
    CC_SYNTHESIZE(ColorType, _colorType, ColorType);
    CC_SYNTHESIZE(Color4B, _colorRGB, ColorRGB);
    CC_SYNTHESIZE(Delegate_t, _delegate, Delegate);
    
    
    Image *_imageData;
//    RenderTexture *_renderTexture;
    CC_SYNTHESIZE(RenderTexture *, _renderTexture, RenterTextrue);
    Vector<Sprite *> _brushs;
    
};

#endif /* defined(__JuniorTemplate__Draw_Sprite__) */
