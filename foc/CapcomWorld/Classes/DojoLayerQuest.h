//
//  DojoLayerQuest.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#ifndef __CapcomWorld__DojoLayerQuest__
#define __CapcomWorld__DojoLayerQuest__

#include <iostream>
#include "cocos2d.h"
#include "GameConst.h"
#include "MyUtil.h"
#include "ChapterLayer.h"

using namespace cocos2d;

class DojoLayerQuest : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    //virtual bool init();
    
    DojoLayerQuest(CCSize layerSize);
    ~DojoLayerQuest();
    
    static DojoLayerQuest* getInstance();
    
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void InitUI();
    
    //void readQuest();
    //void parseQuestXML(xmlNode * node, CCArray *_questList);

  
private:
    
    static DojoLayerQuest* instance;
    float StartYPos;
    
    ChapterLayer* pChapterLayer;
};

#endif /* defined(__CapcomWorld__DojoLayerQuest__) */
