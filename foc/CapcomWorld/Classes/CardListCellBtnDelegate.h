//
//  CardListCellBtnDelegate.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 17..
//
//

#ifndef CapcomWorld_CardListCellBtnDelegate_h
#define CapcomWorld_CardListCellBtnDelegate_h

//#include "CustomCCTableViewCell.h"
#include "CardInfo.h"

class CardListCellBtnDelegate{
public:
    virtual void ButtonA(CardInfo* card){};
    virtual void CardImagePressed(CardInfo* card){};//, CCObject *sender){};
    virtual void CardListBackBtnPressed(){};
};

#endif
