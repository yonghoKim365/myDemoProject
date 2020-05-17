
//
//  GAFObject_Creator.h
//  MGT_Template
//
//
//
//

#ifndef __GAFObject_Creator__
#define __GAFObject_Creator__

#include "cocos2d.h"
#include "GAF.h"

USING_NS_CC;
USING_NS_GAF;


class GAFObject_Creator : public Node
{
public:
    
    GAFObject_Creator();
    ~GAFObject_Creator();
    
    static GAFObject_Creator* create(std::string path, Vec2 pos);

    virtual bool init();
    
    virtual bool initWithPath(std::string path, Vec2 pos);
    
    //override
    virtual void onEnter() override;
    virtual void onExit() override;
    
    CREATE_FUNC(GAFObject_Creator);
    
public:
    
    GAFAsset* m_asset;
    GAFObject* m_mainObject;
    
    GAFObject* getMainObject();
    void start(bool isLoop);
    
};

#endif /* defined(__MGT_Template__GAFObject_Creator__) */
