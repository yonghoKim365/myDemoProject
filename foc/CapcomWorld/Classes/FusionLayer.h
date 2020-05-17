//
//  FusionLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 18..
//
//

#ifndef __CapcomWorld__FusionLayer__
#define __CapcomWorld__FusionLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "DeckListLayer.h"
//#include "CellLayerDelegate.h"
#include "TeamEditLayer.h"
#include "CardListLayer.h"
#include "CardListCellBtnDelegate.h"
//#include "../CCScrollLayerExt/CCScrollLayerExt.h"
#include "CardDetailViewLayer.h"
#include "DetailViewCloseDelegate.h"
#include "ACardMaker.h"

USING_NS_CC;

class FusionLayer : public cocos2d::CCLayer, MyUtil, GameConst, CardListCellBtnDelegate, DetailViewCloseDelegate,PopupDelegate
{
    
public:
    static FusionLayer* getInstance();
    
    FusionLayer(CCSize layerSize);
    ~FusionLayer();
    
    void InitLayer(int _step);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void MenuCallback(CCObject* pSender);
    
    void requestFusion();
    
    void DoCardFusionAction();
    void ArrowAction();
    void removeArrow();
    void CenterButtonAction();
    
    int fusionStep;
    int selectedFusionDeck;
    
    CardInfo *fusionCard1;
    CardInfo *fusionCard2;
    CardInfo *selectedCard;
    
    CardListLayer *cardListLayer;
    CardDetailViewLayer *cardDetailViewLayer;
    
    void ButtonA(CardInfo* card);
    void CardListBackBtnPressed();
    
    void AddCardToFusionDeck(CardInfo *card, int deck);
    
    void CloseDetailView();
    
    void setFusionData(ResponseFusionInfo* _data);
    void DoCardFusion(CardInfo *card1, CardInfo *card2);
    
    ACardMaker *cardMaker;
    
    int originAttack;
    int originDefense;
    
    bool bTouchBegan;
    bool bTouchMove;
    int popupCnt;
    void ButtonOK(CardInfo *card);
    void ButtonCancel();
    
    ResponseFusionInfo* fusionInfo;
    
    CCPoint touchBeganLocation;
private:
    static FusionLayer* instance;
    
};

#endif /* defined(__CapcomWorld__FusionLayer__) */
