//
//  TeamEditLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 16..
//
//

#ifndef __CapcomWorld__TeamEditLayer__
#define __CapcomWorld__TeamEditLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "CardInfo.h"
#include "ACardMaker.h"
#include "TeamEditBtnBackDelegate.h"

USING_NS_CC;

class TeamEditLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
    
public:
    static TeamEditLayer* getInstance();
    
    static TeamEditLayer* instance;
    
    TeamEditLayer(int _nTeam, int _nId, CardInfo* _teamCards[], TeamEditBtnBackDelegate *_delegate, int _callFrom);
    //TeamEditLayer(int _nTeam, CCArray* _teamCards[]);
    ~TeamEditLayer();
    
    void InitLayer();
  
    int nTeam;
    int nId;
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    //CCSprite* pSprBackBtn;
    int selectedDeck;
    bool bEmptyDeckSelected;
    
    bool bCardSelected;
    CardInfo *selectedCard;
    void CardSelected(CardInfo *_card);
    void AddCardToDeck(CardInfo *_card);
    
    bool bInitDone;
    //CCArray *cardsInDeck;
    
    CardInfo *originTeam[5];
    CardInfo *cardInDeck[5];
    
    int originCardSrl[5];
    
    ACardMaker *cardMaker;
    int isDuplicateCardInDeck(CardInfo *_card);
    
    TeamEditBtnBackDelegate *delegate;
    void MenuCallback(CCObject* pSender);
    void buttonConfirm();
    void buttonCancel();
    
    int nCallFrom;
};

#endif /* defined(__CapcomWorld__TeamEditLayer__) */
