//
//  TrainingLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 19..
//
//

#ifndef __CapcomWorld__TrainingLayer__
#define __CapcomWorld__TrainingLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "DeckListLayer.h"
//#include "CellLayerDelegate.h"
#include "TeamEditLayer.h"
#include "CardListLayer.h"
#include "CardListCellBtnDelegate.h"
/*
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#   include "../CCScrollLayerExt/CCScrollLayerExt.h"
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#   include "CCScrollLayerExt/CCScrollLayerExt.h"
#endif
 */
#include "CardDetailViewLayer.h"
#include "DetailViewCloseDelegate.h"

USING_NS_CC;

class TrainingLayer : public cocos2d::CCLayer, MyUtil, GameConst, CardListCellBtnDelegate, DetailViewCloseDelegate, PopupDelegate
{
    
public:
    static TrainingLayer* getInstance();
    TrainingLayer(CCSize layerSize);
    ~TrainingLayer();
    
    void InitLayer(int _step);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void MenuCallback(CCObject* pSender);
    
    int fusionStep;
    int selectedFusionDeck;
    
    CardInfo *fusionCard1;
    CardInfo *fusionCard2;
    
    CardListLayer *cardListLayer;
    CardDetailViewLayer *cardDetailViewLayer;
    
    void ButtonA(CardInfo* card);
    void CardListBackBtnPressed();
    
    void AddCardToFusionDeck(CardInfo *card, int deck);
    
    void CloseDetailView();
    
    //void DoCardTraining(CardInfo *card1, CardInfo *card2);
    void setTrainingData(ResponseTrainingInfo* _data);
    void DoCardTraining();
    void actionSlide();
    void actionScale();
    void actionShake();
    void actionFadein();
    
    void actionHitLT(); // -- left top
    void actionHitLB(); // -- left bottom
    void actionHitM();  // -- middle
    void actionHitRT(); // -- right top
    void actionHitRB(); // -- left bottom
    
    void removeActions();
    
    void Detailview();
    
    void loadImage();
    
    ACardMaker *cardMaker;
    
    int originAttack;
    int originDefense;
    int originExp;
    int originLevel;
    int originExpCap;
    
    CardInfo *selectedCard;
    
    void InitGauge(int _from, int _to, int _max);
    void InitNextGauge();
    void increaseGreenGauge();
    void showLevelUp();
    void RunAction();
    int progressStartVal; // from
    int progressTargetVal; // to
    float ProgressMaxVal;
    int progressNextTargetVal;
    float ProgressTargetRatio;
    bool bTouchBegan;
    
    bool bTouchMove;
    int popupCnt;
    void ButtonOK(CardInfo *card);
    void ButtonCancel();
    CCPoint touchBeganLocation;
    
    ResponseTrainingInfo* trainingInfo;
    
    void onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    
    int trainingCard1IDforLevelUp;

private:
    static TrainingLayer* instance;
    CCSprite*   topCha;
    CCSprite*   bottomCha;
    CCSprite*   effectHit[5];
    
    CCArray*    aniFrame;
};
#endif /* defined(__CapcomWorld__TrainingLayer__) */
