//
//  CardTeamSimpleDrawLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 20..
//
//

#include "CardTeamSimpleDrawLayer.h"



CardTeamSimpleDrawLayer::CardTeamSimpleDrawLayer(int nTeam, int nIdx)
{
    this->setAnchorPoint(ccp(0,0));
    this->setTouchEnabled(false);
    
    cardMaker = new ACardMaker();
    InitLayer(nTeam, nIdx);
    
}

CardTeamSimpleDrawLayer::~CardTeamSimpleDrawLayer()
{
    CC_SAFE_DELETE(cardMaker);
    
    this->removeAllChildrenWithCleanup(true);
}



void CardTeamSimpleDrawLayer::InitLayer(int _nTeam, int _nIdx)
{
    PlayerInfo *pi = PlayerInfo::getInstance();
    for (int i=0;i<5;i++){
        CardInfo *card = pi->GetCardInDeck(_nTeam, _nIdx, i);
        if (card != NULL){
            cardMaker->MakeCardThumb(this, card, ccp(24+i*120, -22), 170, 100, 10+i);
        }
    }
}