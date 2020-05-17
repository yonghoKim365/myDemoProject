#ifndef __STEP1_TYPE2_H__
#define __STEP1_TYPE2_H__

#include "cocos2d.h"
#include "Step1_Base.h"

USING_NS_CC;


class DrawObject : public MGTSprite
{
public:
    DrawObject(){};
    ~DrawObject(){};
    
    static DrawObject* create(const char *pszFileName)
    {
        DrawObject *pobSprite = new DrawObject();
        if (pobSprite && pobSprite->initWithFile(pszFileName))
        {
            pobSprite->autorelease();
            return pobSprite;
        }
        CC_SAFE_DELETE(pobSprite);
        return NULL;
    }
    static DrawObject* createWithFullPath(const char *pszFileName)
    {
        DrawObject *pobSprite = new DrawObject();
        
        if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img",pszFileName).c_str()))
        {
            pobSprite->autorelease();
            return pobSprite;
        }
        CC_SAFE_DELETE(pobSprite);
        return NULL;
    }
    static DrawObject* createWithCommonPath(const char *pszFileName)
    {
        DrawObject *pobSprite = new DrawObject();
        std::string str = pszFileName;
        if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getCommonFilePath("img", pszFileName).c_str()))
        {
            pobSprite->autorelease();
            return pobSprite;
        }
        CC_SAFE_DELETE(pobSprite);
        return NULL;
    }
    
    virtual bool initWithFile(const char *pszFilename)
    {
        if (MGTSprite::initWithFile(pszFilename)) {
            return true;
        }
        return false;
    }
    
    virtual bool init()
    {
        if (MGTSprite::init()) {
            return true;
        }
        return false;
    }
    
    CREATE_FUNC(DrawObject);
    
private:
    std::string         imgPath;
    ClippingNode*       coverClipper;
    Sprite*             stencil;
    std::string         areaImgPath;
    
    
public:
    
    CC_SYNTHESIZE(int, _num, Num);
    
    void setImagePath(const std::string path)
    {
        imgPath = path;
    }
    
    std::string getImagePath()
    {
        return imgPath;
    }
    
    void setCoverClipper( ClippingNode* clipper)
    {
        coverClipper = clipper;
    }
    
    ClippingNode* getCoverClipper()
    {
        return coverClipper;
    }
    
    void setEraseStencil(Sprite* sp)
    {
        stencil = sp;
    }
    
    Sprite* getEraseStencil()
    {
        return stencil;
    }
    
    void setAreaImagePath(const std::string path)
    {
        areaImgPath = path;
    }
    
    std::string getAreaImagePath()
    {
        return areaImgPath;
    }
};



class Step1_Type2 : public Step1_Base
{
public:
    
    
    Step1_Type2();
    ~Step1_Type2();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step1_Type2);
    
public:
    // Touch handler (pass touches to the Gesture recognizer to process)
    virtual bool onTouchBegan(Touch* touch, Event* event) override;
    virtual void onTouchMoved(Touch* touch, Event* event) override;
    virtual void onTouchEnded(Touch* touch, Event* event) override ;
    virtual void onTouchCancelled(Touch *touch, Event *event) override;
    
    
public:
    virtual void onNarrationFinishedCallback(std::string fileName) override;
    
    virtual void showAffordance() override;
    virtual void hideAffordance() override;
    
    virtual void playStart() override;
    virtual void playComplete() override;
    
public:
    virtual void onTouchedNavigationButtonAtExit() override;
    virtual void onTouchedNavigationButtonAtNext() override;
    
    virtual void onTouchedPopupButtonAtNo() override;
    virtual void onTouchedPopupButtonAtYes() override;
    
    
    
    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName) override;
    virtual void onTexturePreLoad(std::string& path) override;
    virtual void onFramePlayed(GAFObject* object, uint32_t frame) override;
    
    
private:
    Sprite*                 m_word;
    Sprite*                 m_alphabet;
    
    bool                    m_isTouchObject;
    int                     m_count;
    
public:
    
    DrawObject*                 m_currentDrawObject;
    Vector<DrawObject*>         m_drawObjects;
    std::vector<Vec2>           m_brushes;
    
    Image*                      m_objectImageData;
    RenderTexture*              m_brushRendertexture;
    Image*                      m_brushImageData;
    
    void initView();
    void createObject();
    
    bool isTouchDrawObject(Vec2 point);
    void eraseCoverAtPoint(Vec2 point);
    
    void createRenderTexture();

    
    bool checkIntersectionPercentage();
    void saveTempImageFile();
    std::string getTempImageFile();
    
    void eraseComplete();
    
    void objectShowAll();
    void wordAnimation();
    

    
    void interactionStart();
    void nextInteraction();
};

#endif // __STEP1_TYPE2_H__
