/*
 *  CustomCCTableViewCell.h
 *  SkeletonX
 *
 *  Created by mac on 11-11-23.
 *  Copyright 2011 GeekStudio. All rights reserved.
 *
 */

#ifndef __CapcomWorld__CustomCCTableViewCell__
#define __CapcomWorld__CustomCCTableViewCell__

#include "cocos2d.h"
USING_NS_CC;

//#include "CCTableViewCell.h"
//#include "CCTableView.h"
#include "CardInfo.h"
#include "CardListCellBtnDelegate.h"
#include "MyUtil.h"
#include "GameConst.h"
#include "PlayerInfo.h"

class CustomCCTableViewCell : public cocos2d::CCLayer, MyUtil, GameConst {
public:
	CustomCCTableViewCell();//const char * mCellIdentifier);
	~CustomCCTableViewCell();
    
    CardInfo *_cardInfo;
    CCMenuItem *optionButton;
    
    CC_SYNTHESIZE(CardListCellBtnDelegate*,delegate,Delegate);
    
    void ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    void ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event);
    
    const char *getGradeString(int grade);
    
    void ButtonA(CardInfo *_card);

    CCPoint touchStartPos;
    CCPoint touchEndPos;
    CCPoint startPosition, endPosition;
    bool moving;

    void SetTouchArea(CCPoint *cFrom, CCPoint *cEnd);
    void MakeCell(CardInfo *card, int nCallFrom, int nTeamID);
    void MakeCardThumb(CCLayer *layer, CardInfo *card, CCPoint ccp, int z, int _tag);
    //CCSprite *createNumber(int number, cocos2d::CCPoint pos, float scale, int rareLv);
    void refreshLevel(CCLayer *_layer, int _level, int x, int y, int rareLv);
    bool bTouchSkip;
    void setSkipTouch(bool a);
    
    int LayerIndex;
};

#endif 