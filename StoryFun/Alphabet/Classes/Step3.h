#ifndef __STEP3_H__
#define __STEP3_H__

#include "cocos2d.h"
#include "Step3_Base.h"

USING_NS_CC;


enum Play3_PaletteState
{
    STATE_CRAYON = 0,
    STATE_ERASE
};

enum Play3_TouchState
{
    TOUCHSTATE_NONE = 0,
    TOUCHSTATE_DRAWING,
    TOUCHSTATE_ERASE,
    TOUCHSTATE_STICKER_MENUITEM,
    TOUCHSTATE_STICKER_MADE,
};

enum Play3_State
{
    STATE_NONE = 0,
    STATE_DRAWING,
    STATE_SELECTED_STICKER,
};



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
        
        if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img", pszFileName).c_str()))
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





class StickerObject : public MGTSprite
{
public:
    StickerObject(){};
    ~StickerObject(){};
    
    static StickerObject* create(const char *pszFileName)
    {
        StickerObject *pobSprite = new StickerObject();
        if (pobSprite && pobSprite->initWithFile(pszFileName))
        {
            pobSprite->autorelease();
            return pobSprite;
        }
        CC_SAFE_DELETE(pobSprite);
        return NULL;
    }
    static StickerObject* createWithFullPath(const char *pszFileName)
    {
        StickerObject *pobSprite = new StickerObject();
        
        if (pobSprite && pobSprite->initWithFile(MGTResourceUtils::getInstance()->getFilePath(ResourceType::IMAGE, "img",pszFileName).c_str()))
        {
            pobSprite->autorelease();
            return pobSprite;
        }
        CC_SAFE_DELETE(pobSprite);
        return NULL;
    }
    static StickerObject* createWithCommonPath(const char *pszFileName)
    {
        StickerObject *pobSprite = new StickerObject();
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
            
            setUseCount(0);
            setEnabled(true);
            
            return true;
        }
        return false;
    }
    
    virtual bool init()
    {
        if (MGTSprite::init()) {
            
            setUseCount(0);
            setEnabled(true);
            
            return true;
        }
        return false;
    }
    
    CREATE_FUNC(StickerObject);
    
private:
    std::string     imgPath;
    int             useCount;
    bool            isEnabled;
    
public:
    
    CC_SYNTHESIZE(int, _num, Num);
    
    void setUseCount(int num)
    {
        useCount = num;
        
//        if (useCount == 5)
//        {
//            setEnabled(false);
//        }
//        else
//        {
//            setEnabled(true);
//        }
    }
    
    int getUseCount()
    {
        return useCount;
    }
    
    void setEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
    
    bool getIsEnabled()
    {
        return isEnabled;
    }
    
    void setImagePath(const std::string path)
    {
        imgPath = path;
    }
    
    std::string getImagePath()
    {
        return imgPath;
    }
};




class Step3 : public Step3_Base
{
public:
    
    
    Step3();
    ~Step3();
    
    static cocos2d::Scene* createScene();

    virtual bool init() override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    virtual void onViewLoad() override ;
    virtual void onViewLoaded() override ;
    
    // implement the "static create()" method manually
    CREATE_FUNC(Step3);
    
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
    
public:         //gaf delegate function
    virtual void onFinishSequence(GAFObject* object, const std::string& sequenceName) override;
    virtual void onTexturePreLoad(std::string& path) override;
    virtual void onFramePlayed(GAFObject* object, uint32_t frame) override;

public:
    virtual void onTouchedNavigationButtonAtExit() override;
    virtual void onTouchedNavigationButtonAtNext() override;
    
    virtual void onTouchedPopupButtonAtNo() override;
    virtual void onTouchedPopupButtonAtYes() override;
    
    
private:
    Sprite*                 m_object;
    Sprite*                 m_word;
    Sprite*                 m_alphabet;
    Sprite*                 m_affordance;
    
    
    int                     m_playNum;
    int                     m_count;

    Touch                   *m_touch;
    
    bool                    m_isTouchMoved;
    bool                    m_isTouchObject;
    

    
public:
    void initView();
    
    
private:    //play1
    
    void createPlay1();
    void blinkAlphabetAnimation();
    void touchAlphabet();
    void alphabetAnimation();
    
    
private:    //play2 - draw alphabet
    DrawObject*                 m_currentDrawObject;
    Vector<DrawObject*>         m_drawObjects;
    std::vector<Vec2>           m_brushes;
    
    RenderTexture*              m_saveRendertexture;
    RenderTexture*              m_brushRendertexture;
    Image*                      m_objectImageData;
    Image*                      m_brushImageData;
    
    
    void createPlay2();
    void createDrawAlphabet();
    void setDrawWrite();
    bool isTouchDrawObject(Vec2 point);
    void eraseCoverAtPoint(Vec2 point);
    
    void createRenderTexture();
    
    bool checkIntersectionPercentage();
    void saveTempImageFile();
    std::string getTempImageFile();
    
    void saveAlphabetImageFile();
    std::string getAlphabetImageFile();
    
    void writeComplete();
    void makeWordAnimation();
    
    
private:    //play3 - drawing & sticker
    int                     m_crayonNum;
    int                     m_stickerNum;
    
    bool                    m_isDrawComplete;
    
    Sprite*                 m_crayonBox;
    Sprite*                 m_stickerBox;
    
    Vector<MenuItem*>               m_crayonItems;
    Vector<StickerObject*>          m_stickerItems;
    Vector<Sprite*>                 m_dotsArr;
    
    RenderTexture*          m_step3DrawingRenderTexture;
//    RenderTexture*          m_step3StickerRendertexture;
    RenderTexture*          m_step3tempRenderTexture;
    
    Color3B                 m_crayonColor;
    
    
    cocos2d::Vector<StickerObject*> m_madeStickers;
    
    
    Sprite*                 m_stickerContainer;
    StickerObject*          m_touchedSticker;
    StickerObject*          m_selectedSticker;
    
    Sprite*                 m_tempSticker;
    int                     m_madeStickerCount;
    
    Size                    m_touchObjectSize;
    
    Play3_PaletteState              m_paletteState;
    Play3_TouchState                m_Play3TouchState;
    Play3_State                     m_Play3State;
    
    void createPlay3();
    void createCrayonBox();
    void unselectCrayonMenu();
    void crayonMenuCallback(Ref* sender);
    void setCrayonEnabled(int currentNum);
    void setCrayon();
    void drawCrayonAtPoint(Vec2 point);
    void eraseAtPoint(Vec2 point);
    
    
    void createStickerBox();
    void touchStickerItem(int num);
    void makeSticker();
    void pressSticker();
    void selectSticker();
    void unselectSticker();
    void deleteSticker(StickerObject* object);
    void stickerDeleteMenuCallback(Ref* sender);
    void onStickerPressActivate(float dt);
    
    void createSaveButton();
    void saveButtonActivation();
    void saveMenuCallback(Ref* sender);
    void captureImage();
    void savePortfolio();
    void showResult();
    
    
private:
    void nextPlay();
    void interactionStart();
    void nextInteraction();
    void interactionComplete();
    
    void finishAnimation();
    
    std::string getAlphabetString(int num);
};

#endif // __STEP3_H__
