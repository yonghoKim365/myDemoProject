
#ifndef __MGTLAYER_H__
#define __MGTLAYER_H__

#include "cocos2d.h"
#include "MGTSprite.h"
#include "MGTMacros.h"
#include "PsdParser.h"
#include "MGTUtils.h"
#include "MGTResourceUtils.h"


using namespace cocos2d;
using namespace resourceutils;

class MGTLayer : public LayerColor {
    
public:
    
    Size      winSize;
    Size      winHalfSize;
	Vec2     winCenter;

protected:
    bool                            m_bIsCompleteTransition;
    bool                            m_bIsCompleteFlashParsing;
    
private:
    // scheduler time
    int                             _currentTime;
    
public:
    ~MGTLayer();
    MGTLayer();
    
    static MGTLayer* create(const Color4B& color);
    static MGTLayer * create(const Color4B& color, GLfloat width, GLfloat height);
    
    virtual bool init();
    virtual bool initWithColor(const Color4B& color);
    virtual bool initWithColor(const Color4B& color, GLfloat w, GLfloat h);
    
    virtual void onEnter();
    void         onEnterTransitionDidFinish();
    virtual void onExit();
    
    virtual bool onTouchBegan(Touch *pTouch, Event *pEvent);
	virtual void onTouchMoved(Touch *pTouch, Event *pEvent);
	virtual void onTouchEnded(Touch *pTouch, Event *pEvent);
    virtual void onTouchCancelled(Touch *pTouch, Event *pEvent);

    virtual void onViewLoad();
    virtual void onViewLoaded();
    
    virtual void addChild(Node *child, int zOrder, int tag);
    virtual void addChild(Node *child, int zOrder);
    virtual void addChild(Node *child);
    
    CREATE_FUNC(MGTLayer);
    
    void setBackgroundColor(const Color4B& color);

#pragma mark - getPath
protected:
    std::string getFilePath(ResourceType type, std::string path);
    std::string getFilePath(ResourceType type, std::string strFolderName, std::string strFileName);
    
    std::string getCommonFilePath(std::string path);
    std::string getCommonFilePath(std::string strFolderName, std::string strFileName);

    std::string getSoundFilePath(std::string path);
    std::string getImageFilePath(std::string path);

    
#pragma mark - timer
protected:
    void onTimer();
    void stopTimer();
    float getCurrentTime();
private:
    void _tick(float dt);

    
#pragma mark - touch controll
public:
//    void setTouchEnableMGTLayer(Node* sender, bool bDispatchEvents);
//    void setTouchEnableMGTSprite(Node* sender, bool bDispatchEvents);
//    std::vector<MGTSprite*>  _vTouchEnableMGTSprites;
    
#pragma mark - background
private:
    Sprite                                            *_imgBackground;
    void _setBackground(Sprite *pSprite);
    
protected:
    void setBackground(Sprite *pSprite);
    void setBackground(std::string strImageName, bool bIsLanguage = false);
    void setBackgroundPosition(Vec2 aPoint);
    void setBackgroundScale(float fScale);
    Sprite* getBackgroundSprite();

#pragma mark - utils
      
//    void removeAllpointArray(PointArray* pointArray);
    const char* stringFromRect(Node* node);
    
    
protected:
    virtual void setViewLoadedToTransition(bool completeTransition){m_bIsCompleteTransition = completeTransition;};
    virtual void setViewLoadedToParsing(bool completeFlashParsing){m_bIsCompleteFlashParsing = completeFlashParsing;};
    
    
#pragma mark - sound
protected:
    void addSoundObserver();
    void removeSoundObserver();
    //narration finished callback override function
    virtual void onNarrationFinishedCallback(std::string fileName){};
    
private:
    void _onNarrationFinishedCallback(Ref *sender);
};


#endif // __MGTLAYER_H__
