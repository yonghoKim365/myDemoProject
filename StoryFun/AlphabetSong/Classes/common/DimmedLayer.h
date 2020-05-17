
#ifndef DimmedLayer_h
#define DimmedLayer_h

#include "cocos2d.h"

using namespace cocos2d;

typedef enum
{
    eDimmedState_Show = 0,
    eDimmedState_Hide,
} eDimmedState;


class DimmedLayer : public cocos2d::LayerColor {
private:
     
public:
    ~DimmedLayer();
    DimmedLayer();
    
    static DimmedLayer* create(const Color4B& color);
    static DimmedLayer * create(const Color4B& color, GLfloat width, GLfloat height);
    
    virtual bool init() override;
    virtual bool initWithColor(const Color4B& color);
    virtual bool initWithColor(const Color4B& color, GLfloat w, GLfloat h);
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(DimmedLayer);
    
    
    virtual bool onTouchBegan(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchMoved(cocos2d::Touch *touch, cocos2d::Event *event) override;
    virtual void onTouchEnded(cocos2d::Touch *touch, cocos2d::Event *event) override;
    
public:
    
    cocos2d::EventListenerTouchOneByOne*    m_touchlistener;
    
    eDimmedState    m_eState;
    
    void show(float duration, float opacity=0.5f);
    void showComplete();
    void hide(float duration);
    void hideComplete();
};

#endif
