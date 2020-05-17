//
//  CollectListLayer.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 29..
//
//

#ifndef __CapcomWorld__CollectListLayer__
#define __CapcomWorld__CollectListLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "ACardMaker.h"
#include "CardDictionary.h"
#include "PlayerInfo.h"
#include "FileManager.h"
#include "DetailViewCloseDelegate.h"
#include "CardDetailViewLayer.h"
//#define NumOfCard (16)
//#define RowOfCard (4)


class ThumbLayerDelegate
{
public:
    virtual void beforeOpenDetailView(){};
    virtual void afterCloseDetailView(){};
};

class CollectScrollLayer  : public cocos2d::CCLayer, MyUtil, GameConst, ThumbLayerDelegate
{
public:
    
    CollectScrollLayer(CCRect mRect, CCArray *cardList);
    ~CollectScrollLayer();
    
    void InitUI();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SetStartYPos(int YPos);
    void scrollingEnd();
    
    static CollectScrollLayer *instance;
    
    static CollectScrollLayer *getInstance()
    {
        if (instance == NULL)
            printf("CollectListLayer instance is NULL\n");
        return instance;
    }
    
    void setThumbTouch(bool b);
    void setEnableTouchAfterDelay();
    void beforeOpenDetailView();
    void afterCloseDetailView();
    
private:
    
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    int StartXPos;
    //CCSprite* pSprLockedImg[NumOfCard];
    
    //ACardMaker *cardMaker;
    
    //CCArray * allkey;
    
    CCRect layerRect;
    //CCRect listLayerRect;
    
    int nPage;
    int nMaxPage;
    int total_cell;
    //int start_cell;
    //int num_of_cell_per_page;
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    CCArray *collectList;
    
    bool hasCard(int _cardId);
    
    int layerDepth;
    
    int pressedTag;
    int selectedSeries;
};

class CollectListLayer : public cocos2d::CCLayer, MyUtil, GameConst //, ThumbLayerDelegate
{
    
public:
    
    CollectListLayer(CCRect mRect, CCArray *cardList);
    ~CollectListLayer();
    
    void InitUI1();
    void InitUI2(int _series);
    void InitLayer(int _depth, int _series);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);

    void SetStartYPos(int YPos);
    void scrollingEnd();
    
    //void MenuCallback(CCObject* pSender);
    void PageNaviMenuCallBack(CCObject* pSender);
    
    static CollectListLayer *instance;
    
    static CollectListLayer *getInstance()
    {
        if (instance == NULL)
            printf("CollectListLayer instance is NULL\n");
        return instance;
    }
    
    //bool bSkipTouch;
    
    /*
    void setThumbTouch(bool b);
    void setEnableTouchAfterDelay();
    void beforeOpenDetailView();
    void afterCloseDetailView();
    */
private:
  
    cocos2d::CCPoint touchStartPoint;
    cocos2d::CCPoint touchMovePoint;
    
    bool bTouchPressed;
    int StartYPos;
    //CCSprite* pSprLockedImg[NumOfCard];
    
    ACardMaker *cardMaker;
    
    CCArray * allkey;
    
    CCRect layerRect;
    CCRect listLayerRect;
    
    int nPage;
    int nMaxPage;
    int total_cell;
    int start_cell;
    int num_of_cell_per_page;
    
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    CCArray *originCollectList;
    CCArray *collectList;
    
    bool hasCard(int _cardId);
    
    int layerDepth;
    
    int pressedTag;
    int selectedSeries;
    
    CollectScrollLayer *pCollectScrollLayer;
    
    void BackBtnCallback(CCObject* pSender);
};




class CardThumbLayer : public cocos2d::CCLayer, MyUtil, GameConst, public DetailViewCloseDelegate
{
public:
    CardThumbLayer(CardInfo* card, CCPoint pos, int thumbnail_h);
    ~CardThumbLayer();
    
    CardInfo* _card;
    
    void Init();
    
    ACardMaker *cardMaker;
    CardDetailViewLayer *_cardDetailView;
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    CCPoint touchBeganPoint;
    
    void OpenDetailView(CardInfo* card);
    void CloseDetailView();
    int CardDetailViewMakeCnt;
    
    float MainScene_y;
    float MainScene_z;
    float DojoLayerDojo_y;
    float DojoLayerDojo_z;
    float DojoLayerCollect_y;
    float DojoLayerCollect_z;
    float CollectListLayer_y;
    float CollectListLayer_z;
    
    void enableTouch();
    
    ThumbLayerDelegate *delegate;
    void setDelegate(ThumbLayerDelegate *d);
    
};
#endif /* defined(__CapcomWorld__CollectListLayer__) */
