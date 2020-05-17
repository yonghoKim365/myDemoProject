//
//  BattleDuelLayer.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 22..
//
//

#ifndef __CapcomWorld__BattleDuelLayer__
#define __CapcomWorld__BattleDuelLayer__

#include <iostream>
#include "cocos2d.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "BattlePlayerCell.h"
#include "BattleFullScreen.h"
#include "BattleListLayer.h"
#include "BattlePrebattleLayer.h"
#include "CardListLayer.h"
#include "TeamEditLayer.h"
#include "CardListCellBtnDelegate.h"
#include "DojoLayerBg.h"

using namespace cocos2d;

class BattleDuelLayer : public cocos2d::CCLayer, MyUtil, GameConst, BattlePlayerCellButtonDelegate, CardListCellBtnDelegate, TeamEditBtnBackDelegate
{
public:
    
    BattleDuelLayer(CCSize layerSize);
    ~BattleDuelLayer();
    
    static BattleDuelLayer *instance;
    
    static BattleDuelLayer *getInstance()
    {
        if (instance == NULL)
            printf("MainScene instance is NULL\n");
        return instance;
    }
    
    void InitUI();
    
    //LAYER_NODE_FUNC(UserInterfaceLayer);
    //CREATE_FUNC(DojoLayerCard);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    void SubUICallback(CCObject* pSender);
    
    void InitLayer(int _step);
    void InitLayer(int _step, int _selectedTeam);
    void InitLayer0();
    void InitLayer1(int _team);
    void InitLayer3();
    void InitBattleLayer(int selectTeam);
    void InitTeamEditLayer();
    
    BattleListLayer *battleListLayer;
    BattlePrebattleLayer *battlePrebattleLayer;
    CardListLayer *cardListLayer;
    TeamEditLayer *teamEditLayer;
    
    UserInfo *selectedUser;
    
    void ButtonBattle(UserInfo *_user);
    
    void ButtonBack();
    void ButtonA(CardInfo* _card);
    void CardImagePressed(CardInfo* card);
    void CardListBackBtnPressed();
    
    void BattleLoad();
    void BattleStart();
    void BattleStartAction();
    void onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    
    int nBattleStep;
    int nBattleStepOld;
    
    ACardMaker *cardMaker;
    
    CardInfo *cards[5];
    
    BattleFullScreen* battleLayer;
    
    void registerUserProfileImg(cocos2d::CCObject *pSender, void *data);
    
    int selectedTeam;
    
    pthread_t threads;
    void initThread();
    static void *threadAction(void *threadid);
    void threadCallBack();
    void threadTimeoutCallback();

    CocosDenshion::SimpleAudioEngine* soundBG;
    
    void refreshCardList();//float layer_y, int teamID);
    
    /*
    void RefreshHonorPoint(int point, int x, int y, CCLayer *layer);
    void removeNumSprites(CCLayer *_layer);
    CCSprite *numSprs[7];
    void format_commas(int n, char *out);
    CCSprite *createComma(CCPoint pos, float scale = 1.0f);
     */
    
    /*
    cocos2d::CCPoint touchStartPoint;
    bool bTouchPressed;
    float yStart;
    float contentClippingH;
    void scrollingEnd();
    CCRect clipRect;
    CC_SYNTHESIZE(bool,clipsToBounds,ClipsToBounds);
    virtual void visit();
	virtual void preVisitWithClippingRect(CCRect clipRect);
	virtual void postVisit();
     */
};

#endif /* defined(__CapcomWorld__BattleDuelLayer__) */
