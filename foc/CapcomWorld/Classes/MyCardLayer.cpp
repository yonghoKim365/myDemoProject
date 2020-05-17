//
//  MyCardLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#include "MyCardLayer.h"
#include "BattlePrebattleLayer.h"
#include "CardDeckLayer.h"
#include "MainScene.h"
#include "DojoLayerCard.h"
#include "ACardTableView.h"
#include "PopupOkCancel.h"


using namespace cocos2d;

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif


MyCardLayer *MyCardLayer::MyCardLayerInstance = NULL;

MyCardLayer::MyCardLayer(CCSize layerSize)
{
    selectedSortRow = 0;
    this->setContentSize(layerSize);
    
    InitCardArray();
    InitScrollLayer();
    //setTouchEnabled(true);
    
    MyCardLayerInstance = this;
    //xb = new XBridge();
    
}

MyCardLayer::~MyCardLayer(){
    this->removeAllChildrenWithCleanup(true);
}


void MyCardLayer::InitUI(){
    
}

void MyCardLayer::initSortBarLabel(int row)
{

}


void MyCardLayer::InitScrollLayer(){
    
    listLayer = new CardListLayer( CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height), myCardList,CALL_CARDLIST_FROM_MYCARD,this, false, -1);
    listLayer->setAnchorPoint(ccp(0,0));
    listLayer->setPosition(accp(10,0));
    this->addChild(listLayer);//note that you have to release it by yourself.
}


void MyCardLayer::InitCardArray(){
    
    PlayerInfo *pi = PlayerInfo::getInstance();
    myCardList = pi->myCards;
    
}

void MyCardLayer::SortMenuCallback(CCObject *pSender)
{
}


#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)

extern "C"{
    
#endif
    
void didSelectLowInPickerView(int row, int _callFrom){
    CCLog("selected sort row :%d",row);
    
    if (_callFrom == 0){
        //MyCardLayer::getInstance()->removeChildByTag(25,true);
        //MyCardLayer::getInstance()->initSortBarLabel(row);
        //MyCardLayer::getInstance()->selectedSortRow = row;
        
        ACardTableView::getInstance()->selectedSortRow = row;
        
        switch(row){
            case 0:
                //MyCardLayer::getInstance()->listLayer->SortCellByAttack();
                ACardTableView::getInstance()->SortByAttack();
                break;
            case 1:
                //방어력
                //MyCardLayer::getInstance()->listLayer->SortByDefence();
                ACardTableView::getInstance()->SortByDefence();
                break;
            case 2:
                // battle point
                //MyCardLayer::getInstance()->listLayer->SortByBattlePoint();
                ACardTableView::getInstance()->SortByBattlePoint();
                break;
            case 3: // 가격 높은순
                //MyCardLayer::getInstance()->listLayer->SortByPrice();
                ACardTableView::getInstance()->SortByPrice();
                break;
            case 4: // 등급 높은순
                //MyCardLayer::getInstance()->listLayer->SortByCardLevel();
                ACardTableView::getInstance()->SortByCardLevel();
                break;
            case 5: // 합성 등급 높은순
                //MyCardLayer::getInstance()->listLayer->SortByFusionLevel();
                ACardTableView::getInstance()->SortByFusionLevel();
                break;
            case 6: // 스킬 있는것만 보기
                //MyCardLayer::getInstance()->listLayer->SortBySkill();
                ACardTableView::getInstance()->SortBySkill();
                break;
        }
        ACardTableView::getInstance()->removeChildByTag(25,true);
        ACardTableView::getInstance()->initSortBarLabel(row);
    }
    else if (_callFrom == 10){
        BattlePrebattleLayer::getInstance()->changeTeam(row);
    }
    //else if (_callFrom == 20){
    //    CardDeckLayer::getInstance()->copyTeam(row);
    //}
}
    

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#ifdef __cplusplus
}
#endif
#endif

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)

#ifdef __cplusplus
extern "C"{
#endif
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    int val1_from_java;
    int val2_from_java;
    
    void Java_com_capcom_FOC_FOC_nativeDidSelectLowInPickerView(JNIEnv* env, jobject thisObj, jint row, jint _callFrom){
        
        CCLog("nativeDidSelectLowInPickerView  row:%d _callFrom:%d", row, _callFrom);
        
        val1_from_java = row;
        val2_from_java = _callFrom;
        
        if (_callFrom == 20){
            //MyCardLayer::getInstance()->schedule(schedule_selector(MyCardLayer::ActionAfterSpinner));
            MyCardLayer::getInstance()->ActionAfterSpinner();
        }
        else if (_callFrom == 10){
            
            //MyCardLayer::getInstance()->schedule(schedule_selector(MyCardLayer::ActionAfterSpinner));
            
        }
        else{
            MyCardLayer::getInstance()->schedule(schedule_selector(MyCardLayer::ActionAfterSpinner),0.3);
        }
        
        


        
        
    }
    
    void Java_com_capcom_FOC_FOC_nativeDidSelectLowInPickerView10(JNIEnv* env, jobject thisObj, jint row){
        
        CCLog("nativeDidSelectLowInPickerView10  row:%d _callFrom:%d", row, 10);
        
        val1_from_java = row;
        val2_from_java = 10;//_callFrom;
        
        //MyCardLayer::getInstance()->ActionAfterSpinner();
        
        //BattlePrebattleLayer::getInstance()->schedule(schedule_selector(BattlePrebattleLayer::changeTeam),1);
        
        BattlePrebattleLayer::getInstance()->reserveRefresh(row);
                
    }

    
/////////////////////////////////////////////////////////////////////
#ifdef __cplusplus
}
#endif

void MyCardLayer::ActionAfterSpinner()
{
    
    int row = val1_from_java;
    int _callFrom = val2_from_java;

    CCLog("ActionAfterSpinner, row:%d _callFrom:%d", row, _callFrom);
    
    if (_callFrom == 0){
        ACardTableView::getInstance()->selectedSortRow = row;
    
        switch(row){
            case 0:
                ACardTableView::getInstance()->SortByAttack();
                break;
            case 1:
                //방어력
                ACardTableView::getInstance()->SortByDefence();
                break;
            case 2:
                // battle point
                ACardTableView::getInstance()->SortByBattlePoint();
                break;
            case 3: // 가격 높은순
                ACardTableView::getInstance()->SortByPrice();
                break;
            case 4: // 등급 높은순
                ACardTableView::getInstance()->SortByCardLevel();
                break;
            case 5: // 합성 등급 높은순
                ACardTableView::getInstance()->SortByFusionLevel();
                break;
            case 6: // 스킬 있는것만 보기
                ACardTableView::getInstance()->SortBySkill();
                break;
        }
        ACardTableView::getInstance()->removeChildByTag(25,true);
        ACardTableView::getInstance()->initSortBarLabel(row);
    }
    else if (_callFrom == 10){
        BattlePrebattleLayer::getInstance()->changeTeam(row);
    }
//    else if (_callFrom == 20){
//        CardDeckLayer::getInstance()->copyTeam(row);
//    }
    
    MyCardLayer::getInstance()->unschedule(schedule_selector(MyCardLayer::ActionAfterSpinner));
}

#endif


void MyCardLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("MyCardLayer::touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    /*
     CCRect *rect = new CCRect(pSprBtn->boundingBox());
     if (rect->containsPoint(location)){
     CCLog("sprite buttn press");
     _tableview->_tableView->SortCell();
     
     }
     */
    
}

void MyCardLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("MyCardLayer::ccTouchesEnded");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    
}

void MyCardLayer::ButtonA(CardInfo* card)
{
    if(!this->isTouchEnabled()) return;
    
    CCLog("MyCardLayer::ButtonA");
    
    if (PlayerInfo::getInstance()->myCards->count()>2){
        SellPopup(card);
    }
    else{
        popupOk("최소 2장의 카드는 소지해야 합니다.");
    }

}




void MyCardLayer::SellPopup(CardInfo* card)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    PopupOkCancel *popup = new PopupOkCancel(card, this);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
    
    this->setTouchEnabled(false);
    ACardTableView::getInstance()->setTouchEnabled(false);
}

void MyCardLayer::ButtonOK(CardInfo* card){
    
    MainScene::getInstance()->removePopup();
    
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
    
    ResponseSellInfo* responseInfo = ARequestSender::getInstance()->requestSellCard(card->getSrl());
    
    if(responseInfo)
    {
        if (atoi(responseInfo->res) == 0){
            
            PlayerInfo* pi = PlayerInfo::getInstance();
            pi->setRevengePoint(responseInfo->user_stat_revenge);
            pi->setFame(responseInfo->user_stat_fame);
            pi->setStamina(responseInfo->user_stat_q_pnt);
            //pi->setDefensePoint(responseInfo->user_stat_d_pnt);
            pi->setBattlePoint(responseInfo->user_stat_a_pnt);
            pi->setUpgradePoint(responseInfo->user_stat_u_pnt);
            pi->setCash(responseInfo->user_stat_gold);
            pi->setCoin(responseInfo->user_stat_coin);
            pi->setXp(responseInfo->user_stat_exp);
            pi->setLevel(responseInfo->user_stat_lev);
            UserStatLayer::getInstance()->refreshUI();
            
            pi->removeCard(card);
            
            int old_y = ACardTableView::getInstance()->getPositionY();
            this->removeChild(listLayer, true);
            myCardList = NULL;
            InitCardArray();
            InitScrollLayer();
            ACardTableView::getInstance()->setPositionY(old_y);
        }
        else{
            popupNetworkError(responseInfo->res, responseInfo->msg, "requestSellCard");
        }
    }
    else
        popupOk("서버와의 연결이 원활하지 않습니다.\n잠시후에 다시 시도해주세요.");
}

void MyCardLayer::ButtonCancel()
{
    this->setTouchEnabled(true);
    ACardTableView::getInstance()->setTouchEnabled(true);
}

int CardDetailViewMakeCnt;
void MyCardLayer::CardImagePressed(CardInfo* card)//, CCObject *sender)
{
    CCLog("MyCardLayer::CardImagePressed");
    
    if (CardDetailViewMakeCnt>0)return;
    if (MainScene::getInstance()->popupCnt>0)return;
    
    CardDetailViewMakeCnt++;
    //CCLog("MyCardLayer::Card Pic pressed");
    this->setTouchEnabled(false);
    listLayer->setCellTouch(false);
    
    ToTopZPriority(this);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    _cardDetailView = new CardDetailViewLayer(CCSize(size.width,size.height),card, this);
    _cardDetailView->setKeyBlock(true);
    this->addChild(_cardDetailView,1000);
    this->setTouchEnabled(false);
    
    MainScene::getInstance()->HideMainMenu();
    DojoLayerCard::getInstance()->HideMenu();
    ACardTableView::getInstance()->HideMenu();
    UserStatLayer::getInstance()->HideMenu();
    
    CCLayer *parent = (CCLayer*)this->getParent();
    CCLayer *grandParent = (CCLayer*)parent->getParent();
    parent->setTouchEnabled(false);
    grandParent->setTouchEnabled(false);
    
    //detailViewSender = (CustomCCTableViewCell*)sender;
    //detailViewSender->setTouchEnabled(false);
    //detailViewSender->setSkipTouch(true);
}

void MyCardLayer::CloseDetailView(){
    
    CCLog(" MyCardLayer::CloseDetailView");
    //detailViewSender->setTouchEnabled(true);
    //detailViewSender->setSkipTouch(false);
    
    CardDetailViewMakeCnt--;

    this->removeChild(_cardDetailView, true);
    this->setTouchEnabled(true);
    listLayer->setCellTouch(true);
    
    MainScene::getInstance()->ShowMainMenu();
    DojoLayerCard::getInstance()->ShowMenu();
    ACardTableView::getInstance()->ShowMenu();
    UserStatLayer::getInstance()->ShowMenu();
    
    RestoreZProirity(this);
    
    CCLayer *parent = (CCLayer*)this->getParent();
    CCLayer *grandParent = (CCLayer*)parent->getParent();
    parent->setTouchEnabled(true);
    grandParent->setTouchEnabled(true);
    
}
