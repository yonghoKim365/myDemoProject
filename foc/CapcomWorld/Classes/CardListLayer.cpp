//
//  CardListLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#include "CCHttpRequest.h"
#include "CardListLayer.h"
#include "CardDictionary.h"
#include "FileManager.h"
#include "MainScene.h"

using namespace cocos2d;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

int nCallFrom;
CardListLayer::CardListLayer(CCRect mRect, CCArray *cardList, int _nCallFrom, CardListCellBtnDelegate *_btnDelegate, bool _BackBtn, int _belongToTeamNotiFilter)
{
    //mRect.size.height = 341.5f;
    //CCLog("CardListLayer 1");
    
	this->setTouchEnabled(true);
	this->setContentSize(mRect.size);
	this->setPosition(mRect.origin);
    _cardList = cardList;
	rowCount = _cardList->count();
    
    nCallFrom = _nCallFrom;
    cellBtnDelegate = _btnDelegate;
        
    _tableView = new ACardTableView(mRect, cardList, nCallFrom, cellBtnDelegate, _BackBtn, _belongToTeamNotiFilter, MainScene::getInstance()->nCardListFilter);
    _tableView->setAnchorPoint(ccp(0,0));
    // list 안에서 위치잡으므로 _tableView 의 위치를 여기서 setposition 하지 말것
    this->addChild(_tableView,100);
    
    //CheckLayerSize(this);
    
    CCLog("CardListLayer 2");
    
}

CardListLayer::~CardListLayer(){
    this->removeAllChildrenWithCleanup(true);
    _tableView = NULL;
    //cardMaker = NULL;
}

void CardListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
}


void CardListLayer::FilteringCardList(int _filter)
{
    _tableView->filteringByAttribute(_cardList, _filter);
}

void CardListLayer::setCellTouch(bool a){
    //_tableView->setCellTouch(a);
}







/////////////////

void CardListLayer::draw()
{
/*
    ccDrawColor4F(1,1,0,1.0); // 	glColor4f(1, 1, 0, 1.0);
	glLineWidth(2);
	ccDrawRect(ccp(1,1), ccp(this->getContentSize().width, this->getContentSize().height) );
	glLineWidth(1);
  */  
}
