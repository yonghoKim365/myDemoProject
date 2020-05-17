//
//  TradeLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 19..
//
//

#ifndef __CapcomWorld__TradeLayer__
#define __CapcomWorld__TradeLayer__

#include <iostream>

#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "DeckListLayer.h"
//#include "CellLayerDelegate.h"
#include "TeamEditLayer.h"
#include "CardListLayer.h"
#include "CardListCellBtnDelegate.h"
/*
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#   include "../CCScrollLayerExt/CCScrollLayerExt.h"
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#   include "CCScrollLayerExt/CCScrollLayerExt.h"
#endif
 */
#include "CardDetailViewLayer.h"
#include "DetailViewCloseDelegate.h"

USING_NS_CC;

class TradeLayer : public cocos2d::CCLayer, MyUtil, GameConst, CardListCellBtnDelegate, DetailViewCloseDelegate
{
    
public:
    TradeLayer(CCSize layerSize);
    ~TradeLayer();
    
    void InitLayer(int _step);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void MenuCallback(CCObject* pSender);
    //void SortMenuCallback(CCObject *pSender);
    
    void InitTab(int _selectedTab);
    void InitSubLayer(int _selectedTab);
    
    static const int tab_btn_h = 50;
    static const int arrange_btn_h = 44;
    static const int _space1_h = 4; // 탭과 정렬 버튼간의 공백
    static const int text_offy = 6; // 버튼위에 올라가는 텍스트의 위치 오프셋, 버튼과 텍스트 오프셋
    
    void ButtonA(CardInfo* card);
    void CardImagePressed(CardInfo* card);
    int nSelectedTab; // 0 == 검색, 1 == 등록, 2 == 트레이드상태
    int nSelectedSortOption;
    
    CardListLayer *cardListLayer;
    CardDetailViewLayer *_cardDetailView;
    void CloseDetailView();
    
    int cardRegisterStep;
    void InitRegisterLayer(CardInfo *card);
    CardInfo *cardForReg;

};
#endif /* defined(__CapcomWorld__TradeLayer__) */
