
#ifndef __BASE_LAYER_H__
#define __BASE_LAYER_H__


#include "cocos2d.h"

#include "GAF.h"
#include "GAFTimelineAction.h"

#include "MGTLayer.h"
#include "ProductManager.h"
#include "MGTSoundManager.h"
#include "DRMManager.h"
#include "GAFManager.h"
#include "MSLPManager.h"

NS_GAF_BEGIN
class GAFObject;
class GAFAsset;
NS_GAF_END

USING_NS_GAF;
USING_NS_CC;
using namespace cocos2d::experimental;

typedef enum{
    kDepth_background               = 0,
    kDepth_dimmed_down_menu         = 80,
    kDepth_dimmed                   = 100,
    kDepth_popup                    = 110,
    kDepth_dimmed_up_menu           = 120
}eBaseLayerDepth;


class Base_Layer : public MGTLayer
{
public:
    
    Base_Layer();
    ~Base_Layer();
    
    static Base_Layer* create(const Color4B& color);
    static Base_Layer * create(const Color4B& color, GLfloat width, GLfloat height);
    
    virtual bool init() override;
    virtual bool initWithColor(const Color4B& color) override;
    
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(Base_Layer);
    
    
    virtual void onViewLoad() override;
    virtual void onViewLoaded() override;
    
    
public:
    cocos2d::EventListenerTouchOneByOne*            m_touchlistener;
    
    __Dictionary                                    m_psdDictionary;
    
    bool                                            m_touchEnabled;
    
    GAFAsset*                                       m_asset;
    GAFObject*                                      m_gafObject;
    
    
    
protected:
    float                                           m_fadeVolume_Duration;
    float                                           m_fadeVolume_Min;
    float                                           m_fadeVolume_Max;
    
    
public:
    void debugSpeedCallback(Ref* sender);
    
//public:         //GAF function
    GAFObject* createGAFObject(std::string path, bool isloop, Vec2 position);
    GAFObject* createGAFObject(Node* parent, std::string path, bool isloop, Vec2 position);
    GAFObject* createGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position);
    
    GAFObject* createCommonGAFObject(std::string path, bool isloop, Vec2 position);
    GAFObject* createCommonGAFObject(Node* parent, std::string path, bool isloop, Vec2 position);
    GAFObject* createCommonGAFObject(Node* parent, int zOrder, std::string path, bool isloop, Vec2 position);
    
    void removeGAFObject(GAFObject* object);
    void restartGAFObject(GAFObject* object);
    void playGAFObject(GAFObject* object);
    void pauseGAFObject(GAFObject* object);
    int  maxFrameNumberGAFObject(GAFObject* object);
    void setFrameNumberGAFObject(GAFObject* object, int aFrameNumber);
    int  getCurrentFrameNumberGAFObject(GAFObject* object);
    bool hitTestGAFObject(GAFObject* obj, Touch* touch);
    
public:         //common function
    Vec2 getLocalPoint(Node* childnode);
    
    
    
public:         //delegate function
    void onSoundPlay(GAFObject * object, const std::string& soundName);
    

public:
    void onSoundEvent(GAFSoundInfo* sound, int32_t repeat, GAFSoundInfo::SyncEvent syncEvent);
    
protected:         //gaf sound play override function
    virtual void onNarrationPlayForGAF(const std::string filename);
    virtual void onEffectPlayForGAF(const std::string filename);
    virtual void onBGMPlayForGAF(const std::string filename);
    virtual void onBGMFadeInForGAF();
    virtual void onBGMFadeOutForGAF();
    
};


#endif /* defined(__BASE_LAYER_H__) */
