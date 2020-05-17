
#ifndef Preload_h
#define Preload_h

#include "cocos2d.h"

using namespace cocos2d;

class Preload : public cocos2d::LayerColor {
private:
     
public:
    ~Preload();
    Preload();
    
    static Scene *createScene();
    bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Preload);
    
public:
    void gotoContent();
};

#endif
