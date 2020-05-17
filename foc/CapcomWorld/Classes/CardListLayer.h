//
//  CardListLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#ifndef __CapcomWorld__CardListLayer__
#define __CapcomWorld__CardListLayer__

#include <iostream>
#include <list>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
//#include "CellLayerDelegate.h"
#include "CardListCellBtnDelegate.h"
#include "ACardMaker.h"
#include "ACardTableView.h"

USING_NS_CC;

//class CardListLayer : public cocos2d::CCLayer, CCTableViewDelegate,CCTableViewDataSource, MyUtil, GameConst
class CardListLayer : public cocos2d::CCLayer, MyUtil, GameConst
{
public:
    //virtual bool init();
    //CREATE_FUNC(CardListLayer); //LAYER_NODE_FUNC(UserInterfaceLayer);
    
    CardListLayer(CCRect mRect, CCArray *cardList, int _nCallFrom, CardListCellBtnDelegate *_btnDelegate, bool _BackBtn, int _belongToTeamNotiFilter);
    ~CardListLayer();
    unsigned int rowCount;
    
    //CCTableView * _tableView;
    ACardTableView *_tableView;

public:
    /*
    float cellHeightForRowAtIndexPath(CCIndexPath & mIndexPath,CCTableView * mTableView);
    float cellHeightForRowAtIndexPath(CCTableView * mTableView);
	void didSelectRowAtIndexPath(CCIndexPath & mIndexPath,CCTableView * mTableView);
	
	unsigned int numberOfRowsInSection(unsigned int mSection,CCTableView * mTableView);
	unsigned int numberOfSectionsInCCTableView(CCTableView * mTableView);
    
	CCTableViewCell * cellForRowAtIndexPath(CCIndexPath &mIndexPath,CCTableView * mTableView);
	virtual void ccTableViewCommitCellEditingStyleForRowAtIndexPath(CCTableView * mTableView,CCTableViewCellEditingStyle mCellEditStyle,CCIndexPath &mIndexPath);
	virtual void ccTableViewWillReloadCellForRowAtIndexPath(CCIndexPath &mIndexPath,CCTableViewCell * mTableViewCell,CCTableView * mTableView);
    */
    
    //void InitCardDetailLayer(CardInfo *ci);
    //void RemoveDetailLayer();
    
    //CustomCCTableViewCell * InitCell(CardInfo *cardInfo);
    //void InitCell(CustomCCTableViewCell * cell, CardInfo *cardInfo);
    //void MakeCell(CustomCCTableViewCell * cell, CardInfo *cardInfo);
    
    CCArray *_cardList;
    //CardDetailLayer *detailLayer;
    
    //void InitCardList();
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    
    //void CellOptionButtonPressed();
    
    CardListCellBtnDelegate *cellBtnDelegate;
    
    
    void FilteringCardList(int _filter);
    
    void setCellTouch(bool a);
    
    //void MakeCardThumb(CCLayer *layer, CardInfo *card, CCPoint ccp, int z, int _tag);
    //void regNumber(CCLayer *layer, int num, CCPoint pos);
    
    //CCSprite *level[3];
    //CCSprite *createNumber(int number, cocos2d::CCPoint pos, float scale);
    //void refreshLevel(CCLayer *_layer, int _level, int x, int y);
    
    //ACardMaker *cardMaker;
    
public:
	virtual void draw();
    
private:
    
    typedef struct {
        std::string url;
        void *cell;
    } UrlData;
    
    std::list<UrlData*> urls;
    
    //void requestImage(std::string &url, void *cell);
    
    //void onHttpRequestCompleted(CCObject *pSender, void *data);
    
};

#endif /* defined(__CapcomWorld__CardListLayer__) */

