//
//  ACardTableView.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 9..
//
//

#include "ACardTableView.h"
#include "PlayerInfo.h"
#include "CustomCCTableViewCell.h"
#include "MainScene.h"

using namespace cocos2d;

#pragma mark


ACardTableView* ACardTableView::instance = NULL;

ACardTableView::ACardTableView(CCRect mRect, CCArray *_cardList, int _callFrom, CardListCellBtnDelegate *_cellBtnDelegate, bool _BackBtn, int _belongToTeamNotiFilter, int _defaultFiler)
{
    //CCLog("ACardTableView 1");
    
    instance = this;
    selectedSortRow = 0;
    nCurPage = 0;
    clipRect = mRect;
    originCardList = _cardList;
    bBackBtn = _BackBtn;
    cardList = new CCArray();
    for(int i=0;i<originCardList->count();i++){
        cardList->addObject(originCardList->objectAtIndex(i));
    }
    callFrom = _callFrom;
    cellBtnDelegate = _cellBtnDelegate;
    /*
    const char* aaa[] = {
        "공격력 높은 순으로 보기",
        "체력 높은 순으로 보기",
        "배틀 포인트 높은 순으로 보기",
        "가격 높은 순으로 보기",
        "레벨 높은 순으로 보기",
        "합성 등급 높은 순으로 보기",
        "스킬 있는 것만 보기"};
    
    for (int i=0;i<7;i++){
        sortText[i] = aaa[i];
    }
     */
    //xb = new XBridge();
    teamNotiFilter = _belongToTeamNotiFilter;
    
    //CCLog("ACardTableView 100");
    
    sortList(_defaultFiler);// SortByAttack();
    
    nSelectedFilteringOption = _defaultFiler;
    nCurPage = 0;
    InitTable(nCurPage, true);
    this->setTouchEnabled(true);
    //this->setClipsToBounds(true);
    
    //CCLog("ACardTableView 200");
    
}

ACardTableView::~ACardTableView(){
	
    this->stopAllActions();
    CCDirector::sharedDirector()->getTouchDispatcher()->removeDelegate(this);
	this->removeAllChildrenWithCleanup(true);
    cardList->autorelease();
}


void ACardTableView::SetPage(int n){
    nCurPage = n;
}

// 필터링된 카드리스트를 초기화하고 table을 다시 만든다.
void ACardTableView::ResetTable(int _page){
    
    cardList = NULL;
    cardList = new CCArray();
    for(int i=0;i<originCardList->count();i++){
        cardList->addObject(originCardList->objectAtIndex(i));
    }
    
    nCurPage = _page;
    this->removeAllChildrenWithCleanup(true);
    InitTable(nCurPage,false);
}


void ACardTableView::RefreshPage()
{
    float old_y = this->getPositionY();
    //ResetTable(nCurPage);
    
    this->removeAllChildrenWithCleanup(true);
    InitTable(nCurPage,false);
    
    this->setPositionY(old_y);
}

void ACardTableView::InitTable(int _page, bool _loadBySchedule){
    
    int total_cell = cardList->count();
    int total_page = total_cell / MAX_CELL_PER_PAGE;
    if (total_cell % MAX_CELL_PER_PAGE != 0)total_page++;
    
    nMaxPage = total_page;
    start_cell = MAX_CELL_PER_PAGE * _page;
    end_cell  = start_cell + MAX_CELL_PER_PAGE-1;
    if (end_cell > total_cell-1)end_cell = total_cell-1;
    int num_of_cell_per_page = end_cell - start_cell + 1;
    
    int content_h = accp(CARDLIST_LAYER_TOP_SPACE + CARDLIST_LAYER_BUTTON_ZONE_UPPER_SPACE + CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + CARDLIST_LAYER_BUTTON_ZONE_BOTTOM_SPACE + UI_NUM_OF_CARDS_LABEL_H + UI_LIST_SPACE1_H + (CARD_LIST_CELL_HEIGHT * num_of_cell_per_page) + UI_LIST_SPACE2_H + UI_PAGE_NAVI_H + UI_LIST_SPACE3_H);
    
    if (bBackBtn){
        content_h += accp(CARDLIST_PREV_BTN_UPPER_SPACE);
        content_h += accp(CARDLIST_PREV_BTN_H); // upper btn
        content_h += accp(CARDLIST_PREV_BTN_H); // botton back btn
    }
    
    //CCLog(" content_h =%d", content_h);
    
    CCSize layerSize = CCSize(clipRect.size.width, content_h);
    this->setContentSize(layerSize);
    
    //CheckLayerSize(this);
    
    int yy = getContentSize().height;
    
    yy -= accp(CARDLIST_LAYER_TOP_SPACE);
    
    if (bBackBtn){
        // 이전 버튼 여기에 추가
        
        yy -= accp(CARDLIST_PREV_BTN_UPPER_SPACE);
        yy -= accp(CARDLIST_PREV_BTN_H);
        
        CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(ACardTableView::BackBtnCallback));
        pSprBtn1->setAnchorPoint( ccp(0,0));
        pSprBtn1->setPosition( ccp(0,0));//size.width/5 * 0,0));
        pSprBtn1->setTag(0);
        
        CCMenu* pMenu = CCMenu::create(pSprBtn1, NULL);
        
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( ccp(0, yy));
        pMenu->setTag(199);
        
        this->addChild(pMenu, 100);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기 "   , "HelveticaNeue-Bold", 12);
        pLabel1->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0.5,0.5), ccp( getContentSize().width/2, yy + accp(24)), pLabel1, 130);
        
        //yy+= accp(10);
        // 아래 buttonZone상단에서 주는 공백이 많아서 여기서 공백을 10만큼 줄여줌.
    }
    yy -= accp(CARDLIST_LAYER_BUTTON_ZONE_UPPER_SPACE);
    
    InitButtonZone(yy);
    
    yy -= accp(CARDLIST_LAYER_BUTTON_ZONE_HEIGHT);
    
    yy -= accp(CARDLIST_LAYER_BUTTON_ZONE_BOTTOM_SPACE);

    int numOfMyCards = PlayerInfo::getInstance()->myCards->count();
    int numOfMyCardsLimit = 200;//60;
    char num[10];
    sprintf(num, "%d / %d", numOfMyCards, numOfMyCardsLimit);//cardList->count());
    
    std::string strText = "보유한 카드 " ;
    strText.append(num);
    //CCLabelTTF* pLabel1 = CCLabelTTF::create("보유한 카드"   , "Arial-ItalicMT", 13);
    CCLabelTTF* pLabel1 = CCLabelTTF::create(strText.c_str()   , "HelveticaNeue-Bold", 12);
    pLabel1->setAnchorPoint(ccp(0,1));
    pLabel1->setPosition(ccp(2,yy));
    pLabel1->setColor(COLOR_ORANGE);// subBtn_color_normal);
    this->addChild(pLabel1, 50);

    yy -= accp(UI_NUM_OF_CARDS_LABEL_H);
    
    yy -= accp(UI_LIST_SPACE1_H);
    
    cellMakeY = yy;
    cellMakeCnt = start_cell;
    
    if (_loadBySchedule == false){
        CCSize size = GameConst::WIN_SIZE;
        CCPoint *posA = new CCPoint(0, MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE + CARDS_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE);
        CCPoint *posB = new CCPoint(0, size.height - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN/SCREEN_ZOOM_RATE - CARD_DECK_TOP_UI_SPACE_2/SCREEN_ZOOM_RATE+CARD_DECK_TOP_UI_SPACE_3/SCREEN_ZOOM_RATE);
        
        if (callFrom == CALL_CARDLIST_FROM_DECK){
            posA = new CCPoint(0, accp(CARD_DECK_EDIT_LAYER_H));
        }
        
        for(int i=start_cell;i<=end_cell;i++){
            
            yy -= accp(CARD_LIST_CELL_HEIGHT);
            
            CardInfo *ci = (CardInfo*)cardList->objectAtIndex(i);
            
            CustomCCTableViewCell * cell = new CustomCCTableViewCell();

            cell->MakeCell(ci, callFrom, teamNotiFilter);
            
            cell->setDelegate(cellBtnDelegate);
            cell->setAnchorPoint(ccp(0,0));
            cell->setPosition(ccp(0,yy));
            cell->SetTouchArea(posA, posB);
         
            this->addChild(cell);
        }
        
        
        CC_SAFE_DELETE(posA);
        CC_SAFE_DELETE(posB);
    }
    else{
        yy -= accp(CARD_LIST_CELL_HEIGHT) * num_of_cell_per_page;
    }
    
    yy -= accp(UI_LIST_SPACE2_H);
    
    yy-= accp(UI_PAGE_NAVI_H);
    
    char buf3[10];
    sprintf(buf3, "%d / %d",_page+1, total_page);
    CCLabelTTF* pLabel6 = CCLabelTTF::create(buf3 , "Arial-ItalicMT", 13);
    pLabel6->setAnchorPoint(ccp(0.5,0.5));
    pLabel6->setPosition(ccp(this->getContentSize().width/2,yy+accp(UI_PAGE_NAVI_H)/2));
    pLabel6->setColor(subBtn_color_normal);
    this->addChild(pLabel6);
    
    cocos2d::CCSprite* pSprBtnPrev = CCSprite::create("ui/card_tab/page_arrow_p1.png");
    pSprBtnPrev->setAnchorPoint(ccp(0,0));
    pSprBtnPrev->setPosition(ccp(0, yy));
    pSprBtnPrev->setTag(11);
    this->addChild(pSprBtnPrev);
    
    cocos2d::CCSprite* pSprBtnNext = CCSprite::create("ui/card_tab/page_arrow_n1.png");
    pSprBtnNext->setAnchorPoint(ccp(1,0));
    pSprBtnNext->setPosition(ccp(this->getContentSize().width, yy));
    pSprBtnNext->setTag(12);
    this->addChild(pSprBtnNext);
    
    yy -= accp(UI_LIST_SPACE3_H);
    
    
    if (bBackBtn){
        yy -= accp(CARDLIST_PREV_BTN_H);
        
        CCLog(" yy CARDLIST_PREV_BTN_H :%d",yy);
        
        CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/team/cards_bt_back_a1.png","ui/card_tab/team/cards_bt_back_a2.png",this,menu_selector(ACardTableView::BackBtnCallback2));
        pSprBtn1->setAnchorPoint( ccp(0,0));
        pSprBtn1->setPosition( ccp(0,0));//size.width/5 * 0,0));
        pSprBtn1->setTag(0);
        
        CCMenu* pMenu = CCMenu::create(pSprBtn1, NULL);
        
        pMenu->setAnchorPoint(ccp(0,0));
        pMenu->setPosition( ccp(0, yy));
        pMenu->setTag(199);
        
        this->addChild(pMenu, 100);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("뒤로 가기 "   , "HelveticaNeue-Bold", 12);
        pLabel1->setColor(COLOR_YELLOW);
        registerLabel( this,ccp(0.5,0.5), ccp( getContentSize().width/2, yy + accp(24)), pLabel1, 130);
    }
    
    this->setPositionY(0 - this->getContentSize().height  + clipRect.size.height);
    yStart = this->getPositionY();
    
    if (_loadBySchedule){
        this->schedule(schedule_selector(ACardTableView::makeCellBySchedule),0.1,-1,0.1);
    }
}

void ACardTableView::makeCellBySchedule()
{
    //CCLog(" makeCellBySchedule, cellMakeCnt:%d", cellMakeCnt);
    
    int yy = cellMakeY;
    
    /////////////////////////////
    
    CCSize size = GameConst::WIN_SIZE;
    CCPoint *posA = new CCPoint(0, MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE + CARDS_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE);
    CCPoint *posB = new CCPoint(0, size.height - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN/SCREEN_ZOOM_RATE - CARD_DECK_TOP_UI_SPACE_2/SCREEN_ZOOM_RATE+CARD_DECK_TOP_UI_SPACE_3/SCREEN_ZOOM_RATE);
    
    if (callFrom == CALL_CARDLIST_FROM_DECK){
        posA = new CCPoint(0, accp(CARD_DECK_EDIT_LAYER_H));
    }
    /////////////////////////////
    
    yy -= accp(CARD_LIST_CELL_HEIGHT);
    
    CardInfo *ci = (CardInfo*)cardList->objectAtIndex(cellMakeCnt);
    CustomCCTableViewCell * cell = new CustomCCTableViewCell();
    
    cell->MakeCell(ci, callFrom, teamNotiFilter);
    cell->setDelegate(cellBtnDelegate);
    cell->setAnchorPoint(ccp(0,0));
    cell->setPosition(ccp(0,yy));
    cell->SetTouchArea(posA, posB);
    this->addChild(cell);
    
    /////////////////////////////////
    CC_SAFE_DELETE(posA);
    CC_SAFE_DELETE(posB);
    
    /////////////////////////////////
    
    cellMakeY = yy;
    cellMakeCnt++;
    if (cellMakeCnt > end_cell){
        this->unschedule(schedule_selector(ACardTableView::makeCellBySchedule));
    }
}

void ACardTableView::DrawTutorialIcon()
{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    float yyy = getContentSize().height - 160;//30;//160.0f;
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    float yyy = getContentSize().height - 320;//50;//100;//160.0f;
#endif
    
    CCLog("DrawTutorialIcon, layer.height=%f",getContentSize().height);
    
    for(int i=0;i<cardList->count();i++)
    {
        CardInfo *ci = (CardInfo*)cardList->objectAtIndex(i);

        if (!PlayerInfo::getInstance()->isBelongInTeam(ci))
        {
            addClickIcon(this, 580.0f, yyy*SCREEN_ZOOM_RATE);
        }
        /*
        CCSprite* BG = CCSprite::create("ui/tutorial/tutorial_preview_click.png");
        BG->setAnchorPoint(ccp(0.0f, 0.0f));
        BG->setPosition(ccp(270.0f, yyy));
        this->addChild(BG, 1000);
         */
        
        yyy-= 130.0f * (2/SCREEN_ZOOM_RATE);
        
        CCLog("yyy:%f",yyy);
    }
}

void ACardTableView::InitButtonZone(int yy)
{
    
    //CCSize size = this->getContentSize();// CCDirector::sharedDirector()->getWinSize();
    
    CCMenuItemImage *pSprBtn1 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn01_1.png","ui/card_tab/card_sort_attribute_btn01_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn1->setAnchorPoint( ccp(0,0));
    pSprBtn1->setPosition( accp( 2,44));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn2 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn02_1.png","ui/card_tab/card_sort_attribute_btn02_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn2->setAnchorPoint( ccp(0,0));
    pSprBtn2->setPosition( accp( 157,44));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn3 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn02_1.png","ui/card_tab/card_sort_attribute_btn02_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn3->setAnchorPoint( ccp(0,0));
    pSprBtn3->setPosition( accp( 311,44));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn4 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn03_1.png","ui/card_tab/card_sort_attribute_btn03_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn4->setAnchorPoint( ccp(0,0));
    pSprBtn4->setPosition( accp( 465,44));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn5 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn01_1.png","ui/card_tab/card_sort_attribute_btn01_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn5->setAnchorPoint( ccp(0,0));
    pSprBtn5->setPosition( accp( 2,0));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn6 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn02_1.png","ui/card_tab/card_sort_attribute_btn02_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn6->setAnchorPoint( ccp(0,0));
    pSprBtn6->setPosition( accp( 157,0));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn7 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn02_1.png","ui/card_tab/card_sort_attribute_btn02_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn7->setAnchorPoint( ccp(0,0));
    pSprBtn7->setPosition( accp( 311,0));//size.width/5 * 0,0));
    
    CCMenuItemImage *pSprBtn8 = CCMenuItemImage::create("ui/card_tab/card_sort_attribute_btn03_1.png","ui/card_tab/card_sort_attribute_btn03_2.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn8->setAnchorPoint( ccp(0,0));
    pSprBtn8->setPosition( accp( 465,0));//size.width/5 * 0,0));
    
    /*
    CCMenuItemImage *pSprBtn5 = CCMenuItemImage::create("ui/card_tab/card_sort_bar.png","ui/card_tab/card_sort_bar.png",this,menu_selector(ACardTableView::SortMenuCallback));
    pSprBtn5->setAnchorPoint( ccp(0,0));
    pSprBtn5->setPosition( accp( 2,0));//size.width/5 * 0,0));
     */
    
    pSprBtn1->setTag(0);
    pSprBtn2->setTag(1);
    pSprBtn3->setTag(2);
    pSprBtn4->setTag(3);
    pSprBtn5->setTag(4);
    pSprBtn6->setTag(5);
    pSprBtn7->setTag(6);
    pSprBtn8->setTag(7);
    
    //CCMenu* pMenu = CCMenu::create(pSprBtn1, pSprBtn2, pSprBtn3, pSprBtn4, pSprBtn5, NULL);
    CCMenu* pMenu = CCMenu::create(pSprBtn1, pSprBtn2, pSprBtn3, pSprBtn4, pSprBtn5, pSprBtn6, pSprBtn7, pSprBtn8,NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    //pMenu->setPosition( accp(0, ((yy*SCREEN_ZOOM_RATE)-(44+44+4+30))) );
    pMenu->setPosition( accp(0, ((yy*SCREEN_ZOOM_RATE)-CARDLIST_LAYER_BUTTON_ZONE_HEIGHT)) );
    
    pMenu->setTag(99);
    //pSprBtn1->selected();
    switch(nSelectedFilteringOption){
            case 0: pSprBtn1->selected(); break;
            case 1: pSprBtn2->selected(); break;
            case 2: pSprBtn3->selected(); break;
            case 3: pSprBtn4->selected(); break;
            case 4: pSprBtn5->selected(); break;
            case 5: pSprBtn6->selected(); break;
            case 6: pSprBtn7->selected(); break;
            case 7: pSprBtn8->selected(); break;
    }
    
    this->addChild(pMenu, 100);
    
    cocos2d::CCSprite* pSpr1 = CCSprite::create("ui/card_tab/card_sort_bg.png");
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr1->setPosition(accp(0, ((yy*SCREEN_ZOOM_RATE)-(CARDLIST_LAYER_BUTTON_ZONE_HEIGHT))-2 ) );
    this->addChild(pSpr1);
    
    
    /*
    CCLabelTTF* pLabel1 = CCLabelTTF::create("전체속성 "   , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("공격속성 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("방어속성 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("잡기속성 " , "HelveticaNeue-BoldItalic", 12);
    */
    CCLabelTTF* pLabel1 = CCLabelTTF::create("공격 "   , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel2 = CCLabelTTF::create("체력 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel3 = CCLabelTTF::create("배틀포인트 " , "HelveticaNeue-BoldItalic", 10);
    CCLabelTTF* pLabel4 = CCLabelTTF::create("레벨 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel5 = CCLabelTTF::create("레어 "   , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel6 = CCLabelTTF::create("합성 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel7 = CCLabelTTF::create("단련 " , "HelveticaNeue-BoldItalic", 12);
    CCLabelTTF* pLabel8 = CCLabelTTF::create("스킬 " , "HelveticaNeue-BoldItalic", 12);
    pLabel1->setTag(21);
    pLabel2->setTag(22);
    pLabel3->setTag(23);
    pLabel4->setTag(24);
    pLabel5->setTag(25);
    pLabel6->setTag(26);
    pLabel7->setTag(27);
    pLabel8->setTag(28);
    
    pLabel1->setColor(subBtn_color_normal);
    pLabel2->setColor(subBtn_color_normal);
    pLabel3->setColor(subBtn_color_normal);
    pLabel4->setColor(subBtn_color_normal);
    pLabel5->setColor(subBtn_color_normal);
    pLabel6->setColor(subBtn_color_normal);
    pLabel7->setColor(subBtn_color_normal);
    pLabel8->setColor(subBtn_color_normal);
    
    // position the label on the center of the screen
    registerLabel( this,ccp(0.5,0.5), accp( 78, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 + 44 -22), pLabel1, 130);
    registerLabel( this,ccp(0.5,0.5), accp(233, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 + 44 -22), pLabel2, 130);
    registerLabel( this,ccp(0.5,0.5), accp(386, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 + 44 -22), pLabel3, 130);
    registerLabel( this,ccp(0.5,0.5), accp(539, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 + 44 -22), pLabel4, 130);
    registerLabel( this,ccp(0.5,0.5), accp( 78, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 -22), pLabel5, 130);
    registerLabel( this,ccp(0.5,0.5), accp(233, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 -22), pLabel6, 130);
    registerLabel( this,ccp(0.5,0.5), accp(386, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 -22), pLabel7, 130);
    registerLabel( this,ccp(0.5,0.5), accp(539, yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 44 -22), pLabel8, 130);
    
    button_yy = yy;
    //initSortBarLabel(0);
    
}

void ACardTableView::initSortBarLabel(int row)
{
    CCLabelTTF* pLabel5 = CCLabelTTF::create(sortText[row], "Thonburi", 13);
    //CCLog(" initSortBarLabel :%s", sortText[row]);
    pLabel5->setTag(25);
    pLabel5->setColor(subBtn_color_normal);
    //registerLabel( this,ccp(  0,0), accp( 21+10, button_yy*SCREEN_ZOOM_RATE - 90-30), pLabel5, 130);
    registerLabel( this,ccp(  0,0.5), accp( 21+10, button_yy*SCREEN_ZOOM_RATE - CARDLIST_LAYER_BUTTON_ZONE_HEIGHT + 22), pLabel5, 130);
}

void ACardTableView::BackBtnCallback(CCObject *pSender)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    soundButton1();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if (tag == 0){
        if (cellBtnDelegate != NULL){
            cellBtnDelegate->CardListBackBtnPressed();
        }
    }
}

void ACardTableView::BackBtnCallback2(CCObject *pSender)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    soundButton1();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    if (tag == 0){
        if (cellBtnDelegate != NULL){
            cellBtnDelegate->CardListBackBtnPressed();
        }
    }
}

void ACardTableView::SortMenuCallback(CCObject *pSender)
{
    if(tutorialProgress < TUTORIAL_TOTAL-1)
        return;
    
    if (MainScene::getInstance()->popupCnt>0)return;
    
    soundButton1();
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    sortList(tag);
    SetButtonToSelected(tag);
    
}

void ACardTableView::sortList(int _filter)
{
    if (nSelectedFilteringOption == 6){
        ResetTable(0);
    }
    
    switch(_filter){
        case 0:
            SortByAttack();
            break;
        case 1:
            SortByDefence();
            break;
        case 2:
            SortByBattlePoint();
            break;
        case 3:
            SortByCardLevel();
            break;
        case 4:
            SortByRareLevel();
            break;
        case 5:
            SortByFusionLevel();
            break;
        case 6:
            filteringByTraingMaterial(originCardList);
            break;
        case 7:
            SortBySkill();
            break;
    }
    nSelectedFilteringOption = _filter;
    MainScene::getInstance()->nCardListFilter = _filter;
}

void ACardTableView::SetButtonToSelected(int a){
    CCMenu *menu = (CCMenu*)this->getChildByTag(99);
    //for(int i=0;i<4;i++){
    for(int i=0;i<8;i++){
        CCMenuItemImage *item1 = (CCMenuItemImage*)menu->getChildByTag(i);
        item1->unselected();
        if (i == a){
            item1->selected();
        }
    }
}

#pragma mark touch

void ACardTableView::MenuCallback(cocos2d::CCObject *pSender)
{
    /*
    CCNode* node = (CCNode*) pSender;
    
    int tag = node->getTag();
    switch(tag){
        case 11:
            CCLog("Page prev");
            nCurPage--;
            if (nCurPage < 0)nCurPage = 0;
            else{
                this->removeAllChildrenWithCleanup(true);
                InitTable(nCurPage);
            }
            break;
        case 12:
            CCLog("Page next");
            nCurPage++;
            if (nCurPage == nMaxPage)nCurPage = nMaxPage-1;
            else{
                this->removeAllChildrenWithCleanup(true);
                InitTable(nCurPage);
            }
            break;
    }
     */
}

void ACardTableView::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void ACardTableView::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    bTouchPressed = false;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 11, localPoint)){
        CCLog("Page prev");
        nCurPage--;
        if (nCurPage < 0)nCurPage = 0;
        else{
            this->removeAllChildrenWithCleanup(true);
            InitTable(nCurPage, true);
        }
    }
    else if (GetSpriteTouchCheckByTag(this, 12, localPoint)){
        CCLog("Page next");
        nCurPage++;
        if (nCurPage == nMaxPage)nCurPage = nMaxPage-1;
        else{
            this->removeAllChildrenWithCleanup(true);
            InitTable(nCurPage, true);
        }
    }
    
    if (clipRect.size.height > this->getContentSize().height)return;
    
#if (1)
    if (moving == true)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos > 0)
            endPos = 0;
        else if (endPos < yStart)
            endPos = yStart;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ACardTableView::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    
    
    
    float y = this->getPositionY();
    
     if (y < 0){
         if (y < yStart){
             //CCActionInterval *action = CCActionInterval::initWithDuration(0.3);
             //CCActionEase *move = CCActionEase::initWithAction(cocos2d::CCActionInterval *pAction)
             
             CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0,this->yStart)), 3);
             CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ACardTableView::scrollingEnd));
             this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
         }
     }
     else if (y > 0){
     
         CCEaseOut * fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3, ccp(0,0)), 3);
         CCCallFunc * callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(ACardTableView::scrollingEnd));
         this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
     }
 
}

void ACardTableView::scrollingEnd()
{
    this->stopAllActions();
	//this->setIsScrolling(false);
}

void ACardTableView::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    if (clipRect.size.height > this->getContentSize().height)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    //CCLog("touch began");
    if (bTouchPressed){
        
        if (touchStartPoint.y != location.y){
            this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
            touchStartPoint.y = location.y;
            //CCLog("deckListLayer.y:%f",this->getPositionY());
        }
    }
    float distance = fabs(startPosition.y - location.y);
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    //printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
    
    if (distance < 15.0f && timeDelta > 50.0f)
    {
        moving = false;
        startPosition = location;
        startTime = currentTime;
    }
    else if (distance > 5.0f)
        moving = true;
    
    if (moving)
    {
        distance = fabs(lastPosition.y - location.y);
        timeDelta = (currentTime.tv_usec - lastTime.tv_usec) / 1000.0f + (currentTime.tv_sec - lastTime.tv_sec) * 1000.0f;
        if (distance < 15.0f && timeDelta > 50.0f)
        {
            moving = false;
            startPosition = location;
            startTime = currentTime;
        }
    }
    
    lastPosition = location;
    lastTime = currentTime;
}

#pragma mark sort,filter

void ACardTableView::SwapCell(int indexA, int indexB){
    
    CustomCCTableViewCell *cell0 = (CustomCCTableViewCell*)(cardList->objectAtIndex(indexA));
    CustomCCTableViewCell *cell1 = (CustomCCTableViewCell*)(cardList->objectAtIndex(indexB));
    
    float cell0_y = cell0->getPositionY();
    cell0->setPositionY(cell1->getPositionY());
    cell1->setPositionY(cell0_y);
    
    cardList->exchangeObjectAtIndex(indexA, indexB);
}


void ACardTableView::filteringByAttribute(cocos2d::CCArray *cards, int _filter){
    this->removeAllChildrenWithCleanup(true);
    
    CCArray *filtedCards = new CCArray();
    for(int i=0;i<cards->count();i++)
    {
        CardInfo *card = (CardInfo*)cards->objectAtIndex(i);
        if (card->getAttribute() == _filter){
            filtedCards->addObject(card);
        }
    }
    cardList = filtedCards;
    nCurPage = 0;
    InitTable(nCurPage, true);
}

void ACardTableView::filteringByTraingMaterial(cocos2d::CCArray *cards){
    this->removeAllChildrenWithCleanup(true);
    
    CCArray *filtedCards = new CCArray();
    for(int i=0;i<cards->count();i++)
    {
        CardInfo *card = (CardInfo*)cards->objectAtIndex(i);
        if (card->bTraingingMaterial){
            filtedCards->addObject(card);
        }
    }
    cardList = filtedCards;
    nCurPage = 0;
    InitTable(nCurPage, true);
}

void ACardTableView::SortByAttack(){
    
    this->removeAllChildrenWithCleanup(false);
    
    //CCLog(" before sort");
    //for(int i=0;i<cardList->count();i++){
    //    CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
    //    CCLog(" cardInfo[%d]->getAttack() %d",i,card1->getAttack());
    //}
    
    //CCLog("sort-===========================");

    if (cardList->count() > 0)
	{
		for(int i=0;i<cardList->count()-1;i++){
            
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                
       //         CCLog(" cell1->_cardInfo->getAttack() %d",card1->getAttack());
       //         CCLog(" cell2->_cardInfo->getAttack() %d",card2->getAttack());
                                
                if (card1->getAttack() < card2->getAttack()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
        //CCLog("-----");
    }
    
    /*
        CCLog(" after  sort-==============");
    if (cardList->count() > 0)
	{
		for(int i=0;i<cardList->count();i++){
            CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
            CCLog(" cardInfo[%d]->getAttack() %d",i,card1->getAttack());
        }
    }
     */
    
    nCurPage = 0;
    InitTable(0, true);
}

void ACardTableView::SortByDefence(){
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->getDefence() < card2->getDefence()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
    
}
void ACardTableView::SortByBattlePoint(){
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->getBp() < card2->getBp()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
}

void ACardTableView::SortByPrice(){
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->getPrice() < card2->getPrice()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
}
void ACardTableView::SortByCardLevel(){
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->getLevel() < card2->getLevel()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
}

void ACardTableView::SortByRareLevel()
{
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->getRare() < card2->getRare()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
}
void ACardTableView::SortByTraingMaterial()
{
}

void ACardTableView::SortByFusionLevel(){
    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                if (card1->GetFusionLevel() < card2->GetFusionLevel()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
    
}
void ACardTableView::SortBySkill(){

    this->removeAllChildrenWithCleanup(false);
    
    if (cardList->count() > 0){
		for(int i=0;i<cardList->count()-1;i++){
            
            /*
            CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
            
            CCLog("card[%d] id:%d skillEffect:%d", i, card1->getId(), card1->getSkillEffect());
            
            if (card1->getSkillEffect() == 0){
                cardList->removeObject(card1);
                i = 0;
            }
            else{
                
            }
             */
            
            //if (card1->getSkillEffect() == 0)cardList->removeObjectAtIndex(i);// (card1);
            
            //if (card1->getSkillEffect() )
            
            
            for(int j=i+1; j<cardList->count();j++){
                
                CardInfo *card1 = (CardInfo*)cardList->objectAtIndex(i);
                CardInfo *card2 = (CardInfo*)cardList->objectAtIndex(j);
                
                
                if (card1->getSkillEffect() < card2->getSkillEffect()){
                    cardList->exchangeObjectAtIndex(i,j);
                }
            }
            
        }
    }
    
    nCurPage = 0;
    InitTable(0, true);
    
}


////////////////////////////////////////
#pragma mark clipping

//////////////////////////////////////////////

int oldPos;

void ACardTableView::HideMenu()
{
    this->setPositionX(10000);
//    CCMenu* pSubMenu = (CCMenu*) this->getChildByTag(99);
//    pSubMenu->setPosition(ccp(10000,-10000));
    
    
}

void ACardTableView::ShowMenu()
{
    this->setPositionX(0);
//    CCSize size = this->getContentSize();
//    CCMenu* pMenu = (CCMenu*) this->getChildByTag(99);
//    pMenu->setPosition( accp(0, ((size.height*2)-(44+44+4+30))) );
    
}
