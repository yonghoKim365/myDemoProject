//
//  DojoLayerCard.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 26..
//
//

#ifndef __CapcomWorld__DojoLayerCard__
#define __CapcomWorld__DojoLayerCard__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "CardListLayer.h"
#include "CardDeckLayer.h"
#include "MyCardLayer.h"
#include "FusionLayer.h"
#include "TrainingLayer.h"
#include "TradeLayer.h"

using namespace cocos2d;

class DojoLayerCard : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    
    DojoLayerCard(CCSize layerSize);
    DojoLayerCard(CCSize layerSize, int sublayer);
    ~DojoLayerCard();
    
    void InitCardList();
    
    void InitUI();
    
    static DojoLayerCard *instance;
    
    static DojoLayerCard *getInstance()
    {
        if (instance == NULL)
            printf("DojoLayerCard instance is NULL\n");
        return instance;
    }

    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerCard);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    void SetNormalSubBtns();
    void SetSelectedSubBtn(int i);
    
    cocos2d::CCArray *myCardList;
    cocos2d::CCSprite* pSprBtn;

    
    MyCardLayer *myCardLayer;
    CardDeckLayer *myDeckLayer;
    FusionLayer *fusionLayer;
    TrainingLayer *trainingLayer;
    TradeLayer *tradeLayer;
    
    int curLayerTag;
    
    void InitSubLayer(int a);
    
    void releaseSubLayer();
    
    float oldZOrder;
    CCPoint oldPosition;
    void ToTopZPriority();
    void RestoreZProirity();
    
    static const int MENU_LABEL_Y = 21;
    
    void HideMenu();
    void ShowMenu();
    
    void setEnableMainMenu(bool flag);
    
    //virtual void didAccelerate(cocos2d::CCAcceleration* pAccelerationValue);
    
    /*
    pthread_t threads[5];
    void initThread();
    static void *PrintHello(void *threadid);
    
    static void *PrintHello2(void *threadid);
    static void *PrintHello3(void *threadid);
    static void *PrintHello4(void *threadid);
*/
};


#endif /* defined(__CapcomWorld__DojoLayerCard__) */
