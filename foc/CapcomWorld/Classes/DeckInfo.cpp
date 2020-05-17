//
//  DeckInfo.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 4..
//
//

#include "DeckInfo.h"
#include "CardDictionary.h"
#include "PlayerInfo.h"

DeckInfo::DeckInfo(){

    //int tempDeck[] = {30752,30753,31831,31832,31833};
    
    //int count = 0;
    for (int i=0;i<5;i++){
        
        atkDeck1[i] = NULL;
        atkDeck2[i] = NULL;
        atkDeck3[i] = NULL;
        atkDeck4[i] = NULL;
        atkDeck5[i] = NULL;
        defDeck1[i] = NULL;
    }
    
   

}

DeckInfo::~DeckInfo(){
    
    
    /*
    atkDeck1->removeAllObjects();
    atkDeck1 = NULL;
    
    atkDeck2->removeAllObjects();
    atkDeck2 = NULL;
    
    atkDeck3->removeAllObjects();
    atkDeck3 = NULL;
    
    atkDeck4->removeAllObjects();
    atkDeck4 = NULL;
    
    defDeck1->removeAllObjects();
    defDeck1 = NULL;
    */
    /*
    attackDeck->removeAllObjects();
    attackDeck = NULL;
    
    defenceDeck->removeAllObjects();
    defenceDeck = NULL;
     */
    
    
}

void DeckInfo::SetCardToDeck(int _team, int _id, int _n, CardInfo *card){
    
    if(card != NULL){
        CCLog(" DeckInfo::SetCardToDeck, team:%d id:%d n:%d card id:%d srl:%d ", _team, _id, _n, card->getId(), card->getSrl());
    }
    else CCLog(" DeckInfo::SetCardToDeck, team:%d id:%d n:%d card is NULL", _team, _id, _n);
    
    if (card != NULL && card->getSrl()==0)return;
    
    if (_team == 0){
        if (_id == 0)atkDeck1[_n] = card;
        else if (_id == 1)atkDeck2[_n] = card;
        else if (_id == 2)atkDeck3[_n] = card;
        else if (_id == 3)atkDeck4[_n] = card;
        else if (_id == 4)atkDeck5[_n] = card;
    }
    else if (_team == 1){
        defDeck1[_n] = card;
    }
}

void DeckInfo::SetCardToDeck(ResponseDeckinfo *info)
{
    for(int i=0;i<5;i++){
        
        // srl에 해당하는 카드를 내 카드 리스트에서 찾아 deck에 넣어야함
        
        if (info->cardSrl[i] == 0){
            SetCardToDeck(0, info->idx, i, NULL);
        }
        else{
            CardInfo *card = PlayerInfo::getInstance()->getCardBySrl(info->cardSrl[i]);
            SetCardToDeck(0, info->idx, i, card); //SetCardToDeck(info->teamType, info->idx, i, card);
        }
    }
    
}



void DeckInfo::EmptyDeck(int _team, int _id)
{
    if (_team == 0){
        if (_id == 0){
            for (int i=0;i<5;i++){
                atkDeck1[i] = NULL;
            }
        }
        else if (_id == 1){
            for (int i=0;i<5;i++){
                atkDeck2[i] = NULL;
            }
        }
        else if (_id == 2){
            for (int i=0;i<5;i++){
                atkDeck3[i] = NULL;
            }
        }
        else if (_id == 3){
            for (int i=0;i<5;i++){
                atkDeck4[i] = NULL;
            }
        }
    }
    else if (_team == 1){
        for (int i=0;i<5;i++){
            defDeck1[i] = NULL;
        }
    }
}
