//
//  PopupDelegate.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 12. 3..
//
//

#ifndef CapcomWorld_PopupDelegate_h
#define CapcomWorld_PopupDelegate_h


class PopupDelegate{
public:
    virtual void ButtonOK(CardInfo* card){};
    virtual void ButtonOK(){};
    virtual void ButtonCancel(){};
};

#endif
