//
//  CellLayerDelegate.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 16..
//
//

#ifndef CapcomWorld_CellLayerDelegate_h
#define CapcomWorld_CellLayerDelegate_h


class CellLayerDelegate{
public:
	virtual void ButtonEdit(CCObject *cell, int teamID){}
    virtual void ButtonCopy(CCObject *cell){}
    virtual void ButtonRemove(CCObject *cell){}
};

#endif
