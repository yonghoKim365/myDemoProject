//
//  MyCardLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#ifndef __CapcomWorld__MyCardLayer__
#define __CapcomWorld__MyCardLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "CardListLayer.h"
#include "DetailViewCloseDelegate.h"
#include "CardListCellBtnDelegate.h"
#include "CardDetailViewLayer.h"
#include "CardDictionary.h"
#include "CardDetailViewLayer.h"
#include "PlayerInfo.h"
#include "CustomCCTableViewCell.h"
#include "PopupDelegate.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
//#include "XBridge.h"
#endif

class MyCardLayer : public cocos2d::CCLayer, MyUtil, GameConst, CardListCellBtnDelegate, DetailViewCloseDelegate, PopupDelegate
{
public:
    
    MyCardLayer(CCSize LayerSize);
    ~MyCardLayer();
    
    static MyCardLayer *MyCardLayerInstance;
    
    static MyCardLayer *getInstance()
    {
        return MyCardLayerInstance;
    }
    

    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //virtual bool init();
    //CREATE_FUNC(MyCardLayer);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    //void SubUICallback(CCObject* pSender);
    void SortMenuCallback(CCObject* pSender);
    
    cocos2d::CCArray *myCardList;
    
    cocos2d::CCSprite* pSprBtn;
    
    CardListLayer *listLayer;
    CardDetailViewLayer *_cardDetailView;
    
    void InitCardArray();
    void InitScrollLayer();
    void InitUI();
    void ButtonA(CardInfo* card);
    void CardImagePressed(CardInfo* card);//, CCObject *sender);
    void CloseDetailView();
    void initSortBarLabel(int row);
    //XBridge *xb;
    const char* sortText[7];
    int selectedSortRow; 
    
    CustomCCTableViewCell *detailViewSender;
    
    void SellPopup(CardInfo *card);
    
    void ButtonOK(CardInfo *card);
    void ButtonCancel();
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    void ActionAfterSpinner();
#endif
    
    // target-selector개념
    /*
    typedef void (CCLayer::*updateProcType)();
    CCLayer *target;
    updateProcType updateInfo;
    */
    
    
};

#endif /* defined(__CapcomWorld__MyCardLayer__) */
