//
//  ACardTableView.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 9..
//
//

#ifndef __CapcomWorld__ACardTableView__
#define __CapcomWorld__ACardTableView__

#include <iostream>



#include "cocos2d.h"
#include "MyUtil.h"
#include "AttackDeckCell.h"
#include "CellLayerDelegate.h"
#include "GameConst.h"
#include "CardListCellBtnDelegate.h"
#include "XBridge.h"

USING_NS_CC;

class ACardTableView : public cocos2d::CCLayer, MyUtil, GameConst 
{
    
public:
    //virtual bool init();
    
    ACardTableView(CCRect mRect, CCArray *_cardList, int _callFrom, CardListCellBtnDelegate *_cellBtnDelegate, bool _BackBtn, int _belongToTeamNotiFilter, int _defaultFiler);
    ~ACardTableView();
    
    static ACardTableView *instance;
    
    static ACardTableView *getInstance()
    {
        if (instance == NULL)
            printf("ACardTableView instance is NULL\n");
        return instance;
    }
    
    CCRect clipRect;
    CardListCellBtnDelegate *cellBtnDelegate;
    
    void ResetTable(int _page);
    CC_SYNTHESIZE(CellLayerDelegate *,delegate,Delegate);
    void RefreshPage();
    
/*
    static const int UI_NUM_OF_CARDS_LABEL_H = 30;  // 리스트 상단 - 보유한 카드 10/60 - UI의 height
    static const int UI_LIST_SPACE1_H = 10;         // 카드 리스트와 상단 UI사이의 공백
    static const int UI_LIST_SPACE2_H = 10;         // 카드 리스트와 하단 UI사이의 공백 
    static const int UI_PAGE_NAVI_H = 40;           // 리스트 하단 좌우 페이지 버튼과 페이지 1/3 UI의 height
    static const int UI_LIST_SPACE3_H = 10;         // 카드 리스트와 제일 하단 여백
    static const int MAX_CELL_PER_PAGE = 8;         // 한 페이지당 cell 갯수
  */  
    
    void SortByAttack();
    void SortByDefence();
    void SortByBattlePoint();
    void SortByCardLevel();
    void SortByRareLevel();
    void SortByFusionLevel();
    void SortByTraingMaterial();
    void SortBySkill();
    void SortByPrice();
    void filteringByTraingMaterial(CCArray *cards);

    void SwapCell(int indexA, int indexB);
    void filteringByAttribute(CCArray *cards, int _filter);
    void SetPage(int n);
    
    void initSortBarLabel(int row);
    const char* sortText[7];
    int selectedSortRow;
    void SortMenuCallback(CCObject* pSender);
    void BackBtnCallback(CCObject* pSender);
    void BackBtnCallback2(CCObject* pSender);
    void SetButtonToSelected(int a);
    //XBridge *xb;
    
    void ShowMenu();
    void HideMenu();
    
    bool bBackBtn;
    
    void DrawTutorialIcon();

private:
    void InitUI();
    void InitButtonZone(int yy);
    void InitTable(int _page, bool _loadBySchedule);
    
    CCArray *originCardList;
    CCArray *cardList;
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    cocos2d::CCPoint touchStartPoint;
    bool bTouchPressed;
    
    void MenuCallback(CCObject* pSender);
    float yStart;
    //float contentClippingH;
    void scrollingEnd();
    int callFrom;
    int nCurPage;
    int nMaxPage;
    int button_yy;
    int start_cell;
    int end_cell;
    
    int cellMakeCnt;
    int cellMakeY;
    void makeCellBySchedule();
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    int teamNotiFilter;
    
    int nSelectedFilteringOption;
    void sortList(int _filter);
};

#endif /* defined(__CapcomWorld__ACardTableView__) */
