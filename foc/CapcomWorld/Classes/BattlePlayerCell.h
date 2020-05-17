//
//  BattlePlayerCell.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 23..
//
//

#ifndef __CapcomWorld__BattlePlayerCell__
#define __CapcomWorld__BattlePlayerCell__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "DeckInfo.h"
#include "PlayerInfo.h"
#include "UserInfo.h"
#include "ACardMaker.h"
#include "CardDetailViewLayer.h"

using namespace cocos2d;
using namespace std;



class BattlePlayerCellButtonDelegate{
public:
	virtual void ButtonBattle(UserInfo *_user){}
};

class BattlePlayerCell : public cocos2d::CCLayer, MyUtil, GameConst, DetailViewCloseDelegate
{
public:
    BattlePlayerCell(UserInfo *_user, BattlePlayerCellButtonDelegate *_delegate);
    ~BattlePlayerCell();
    
    void InitUI();
    
    //bool ccTouchBegan(CCTouch *pTouch, CCEvent *pEvent);
    //void ccTouchEnded(CCTouch *pTouch, CCEvent *pEvent);
    //void ccTouchMoved(CCTouch *pTouch, CCEvent *pEvent);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void MenuCallback(CCObject* pSender);
    
    //CC_SYNTHESIZE(CellLayerDelegate *,delegate,Delegate);
     
    //DeckInfo *_deckInfo;
    //CCArray *_cards;
    
    UserInfo *user;
    
    void ButtonBattle();
    
    BattlePlayerCellButtonDelegate *delegate;
    
    ACardMaker *cardMaker;
    
    CardInfo *cards[5];
    
    void updateUserProfileImage();
    void registerUserProfileImg(std::string filename);
    void profileImgDownloaded(cocos2d::CCObject *pSender, void *data);
    
    CardDetailViewLayer *cardDetailViewLayer;
    void CloseDetailView();
    
    
    cocos2d::CCPoint startPosition, lastPosition;
    cocos2d::cc_timeval startTime, lastTime;
    bool moving;
    
    void SetTouchArea(CCPoint *cFrom, CCPoint *cEnd);
    CCPoint touchStartPos;
    CCPoint touchEndPos;
    
    //bool bTouchBegan;
    
    //void onEnter();
    //void onExit();
};



#endif /* defined(__CapcomWorld__BattlePlayerCell__) */
