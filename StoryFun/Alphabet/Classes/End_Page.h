#ifndef __END_PAGE_H__
#define __END_PAGE_H__

#include "cocos2d.h"
#include "Base_Layer.h"

USING_NS_CC;


class End_Page : public Base_Layer
{
public:
    
    
    End_Page();
    ~End_Page();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(End_Page);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override;
    
    
public:
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    

    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName);
    virtual void onTexturePreLoad(std::string& path);
    virtual void onFramePlayed(GAFObject* object, uint32_t frame);
    
    
public:
    Sprite*                 m_container;
    
    int                     m_nTitleNum;
    
    void initView();
    void menuCallback(Ref* sender);
    void gotoReplay();
    void gotoNext();
    void gotoExit();
};

#endif // __END_PAGE_H__
