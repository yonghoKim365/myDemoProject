//
//  CardDeckLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 11..
//
//

#ifndef __CapcomWorld__CardDeckLayer__
#define __CapcomWorld__CardDeckLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "DeckListLayer.h"
#include "CellLayerDelegate.h"
#include "TeamEditLayer.h"
#include "CardListLayer.h"
#include "CardListCellBtnDelegate.h"
#include "CardDetailViewLayer.h"
#include "CardDictionary.h"
#include "PlayerInfo.h"
#include "TeamEditBtnBackDelegate.h"
//#include "XBridge.h"

USING_NS_CC;





class CardDeckLayer : public cocos2d::CCLayer, MyUtil, GameConst, CellLayerDelegate, CardListCellBtnDelegate,TeamEditBtnBackDelegate
{

public:
    CardDeckLayer(DeckInfo *di, CCSize LayerSize);
    ~CardDeckLayer();
    void InitLayer(int _uiDepth, int _teamID);
    
    static CardDeckLayer *instance;
    
    static CardDeckLayer *getInstance()
    {
        if (instance == NULL)
            printf("CardDeckLayer instance is NULL\n");
        return instance;
    }
    
    //void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    //cocos2d::CCPoint touchStartPoint;
    //bool bTouchPressed;
    
    //static const int TOP_UI_SPACE_1 = 16; // 버튼 상단 공간
    //static const int CARD_DECK_TOP_UI_SPACE_2 = 54; // attack team, defence team 버튼 영역 height
    //static const int CARD_DECK_TOP_UI_SPACE_3 = 10; // 버튼 하단 공간
    
    static const int CARD_DECK_SELL_H = 324;
    static const int CARD_DECK_SELL_SPACE = 10; // cell과 cell사이 수직 여백
    static const int NUM_OF_ATTACK_DECK = 1;//4;
    static const int NUM_OF_DEFENCE_DECK = 1;
    
    DeckInfo *_deckInfo;
    
    void MenuCallback(CCObject* pSender);
    
    CCLayer *subLayer;
    
    int selected_team; // 0 == attack team, 1 == defense team
    
    int uiDepth;
    
    void InitDeckLayer(int whichTeam);
    
    DeckListLayer *_deckListLayer;
    CardListLayer *cardListLayer;
    TeamEditLayer *teamEditLayer;
    
    //CardDetailViewLayer *_cardDetailView;
    
    //CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    //virtual void visit();
	//virtual void preVisitWithClippingRect(CCRect clipRect);
	//virtual void postVisit();
    
    void ButtonEdit(CCObject *cell, int teamID);
    //void ButtonCopy(CCObject *cell);
    void ButtonRemove(CCObject *cell);
    
    void ButtonA(CardInfo* _card);
    void CardImagePressed(CardInfo* card);
    void CardListBackBtnPressed();
    
    void ButtonBack();
    
    AttackDeckCell *selectedCell;
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    //XBridge *xb;
#endif
    //const char* TeamLabel[4];
    
    //void copyTeam(int _to);

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    void refreshLayer();
#endif
    
    pthread_t threads;
    void initThread();
    static void *threadAction(void *threadid);
    void threadCallBack();
    void threadTimeoutCallback();
    
    void refreshCardList();//float layer_y, int teamID);
    
    CCArray *cardList;
    

};



#endif /* defined(__CapcomWorld__CardDeckLayer__) */
