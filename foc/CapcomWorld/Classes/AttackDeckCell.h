//
//  AttackDeckCell.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 15..
//
//

#ifndef __CapcomWorld__AttackDeckCell__
#define __CapcomWorld__AttackDeckCell__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "DeckInfo.h"
#include "CellLayerDelegate.h"
#include "ACardMaker.h"

USING_NS_CC;



class AttackDeckCell : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    static AttackDeckCell* getInstance();
    
    //virtual bool init();
    
    //AttackDeckCell(CCArray *cards, int _team, int _nId, const char *label);
    AttackDeckCell(CardInfo* _cards[], int _team, int _nId, const char *label);
    ~AttackDeckCell();
    
    //CREATE_FUNC(AttackDeckCell); //LAYER_NODE_FUNC(UserInterfaceLayer);
    
    void InitLayer();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void MenuCallback(CCObject* pSender);
    
    CC_SYNTHESIZE(CellLayerDelegate *,delegate,Delegate);
    
    //DeckInfo *_deckInfo;
    //CCArray *_cards;
    CardInfo *cards[5];
    std::string _label;
    
    //void ButtonEdit();
    
    int whichTeam;
    int nId;
    
    ACardMaker *cardMaker;
    
    void SetTouchArea(CCPoint *cFrom, CCPoint *cEnd);
    CCPoint touchStartPos;
    CCPoint touchEndPos;
private:
    static AttackDeckCell* instance;
    int numOfCardsInDeck;
};
#endif /* defined(__CapcomWorld__AttackDeckCell__) */
