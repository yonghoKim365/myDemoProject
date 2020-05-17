/*
 *  CustomCCTableViewCell.cpp
 *  SkeletonX
 *
 *  Created by mac on 11-11-23.
 *  Copyright 2011 GeekStudio. All rights reserved.
 *
 */

//#include "DecoratedBox.h"

#include "CustomCCTableViewCell.h"
#include "FileManager.h"
#include "CardDictionary.h"

CustomCCTableViewCell::CustomCCTableViewCell()//const char * mCellIdentifier):CCTableViewCell(mCellIdentifier)
{
    /*
    optionButton = CCMenuItemImage::create("ui/card_tab/card_list_btn.png", "ui/card_tab/card_list_btn.png", this, menu_selector(CustomCCTableViewCell::ButtonA));
    optionButton->setAnchorPoint(ccp(0,0));
    optionButton->setPosition(ccp(0,0));//accp(522,92));
    menuItem = CCMenu::create(optionButton, NULL);
    menuItem->setPosition(accp(522,92));

    this->addChild(menuItem,0);
    */
    this->setTouchEnabled(true);
    CCSprite *pSprBtn = CCSprite::create("ui/card_tab/card_list_btn.png");
    pSprBtn->setAnchorPoint(ccp(0,0));
    pSprBtn->setPosition(accp(520,91));
    pSprBtn->setTag(10);
    this->addChild(pSprBtn,0);
    
    LayerIndex = 0;
    
	
}

CustomCCTableViewCell::~CustomCCTableViewCell()
{
	
}

void CustomCCTableViewCell::ButtonA(CardInfo *_card)
{
    if (delegate != NULL){
        delegate->ButtonA(_card);
    }
                                               
}


bool bCellTouchBegan = false;



CCPoint touchBeganPoint;
void CustomCCTableViewCell::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //if (bTouchSkip)return;
//    if (bCellTouchBegan)return;
    bCellTouchBegan = true;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (GetSpriteTouchCheckByTag(this, 33, localPoint)){
        touchBeganPoint = location;
        //CCLog(" touchStartPoint.x:%f,y:%f",touchBeganPoint.x,touchBeganPoint.y);
    }
    
    startPosition = location;
    moving = false;
    
    //CCLog(" CustomCCTableViewCell::ccTouchesBegan ");
    
}

void CustomCCTableViewCell::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);

    int delta = fabs(location.y - startPosition.y);
    if (delta > 10)
        moving = true;
}


void CustomCCTableViewCell::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
//    if (bTouchSkip)return;
    
//    if (bCellTouchBegan==false)return;
//    bCellTouchBegan= false;
    
    //CCLog(" CustomCCTableViewCell::ccTouchesEnded ");
    
    if(CALL_CARDLIST_FROM_MYCARD == LayerIndex && tutorialProgress < TUTORIAL_TOTAL-1)
        return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (location.y < touchStartPos.y || location.y > touchEndPos.y){
        //CCLog("out of touch range in CustomCCTableViewCell");
        return;
    }
    
    if (moving == true)
        return;
    
    if (GetSpriteTouchCheckByTag(this, 33, localPoint)){
        
        //CCLog(" location:%f,y:%f",location.x, location.y);
        
        if ( fabs(touchBeganPoint.x - location.x) < 20 && fabs(touchBeganPoint.y - location.y) < 20){
        //if (touchBeganPoint.x == location.x && touchBeganPoint.y == location.y){
            //CCLog(" CustomCCTableViewCell::ccTouchesEnded ");
            //CCLog("location.y:%f",location.y);
            //CCLog("localPoint:%f",localPoint.y);
            if (delegate != NULL)
            {
                if (bCellTouchBegan && tutorialProgress != TUTORIAL_TEAM_SETTING_PREVIEW_3)
                { // 중복터치 방지
                    soundButton1();
                    delegate->CardImagePressed(_cardInfo);//, this);
                    bCellTouchBegan = false;
                }
            }
        }
    }
    
    if (GetSpriteTouchCheckByTag(this, 10, localPoint)){
        if (delegate != NULL){
            if (bCellTouchBegan && tutorialProgress != TUTORIAL_TEAM_SETTING_PREVIEW_3)
            { // 중복터치 방지
                soundButton1();
                //delegate->CardImagePressed(_cardInfo);
                ButtonA(_cardInfo);
                bCellTouchBegan = false;
            }
        }
    }
}

void CustomCCTableViewCell::SetTouchArea(cocos2d::CCPoint *cFrom, cocos2d::CCPoint *cEnd){
    touchStartPos.y = cFrom->y;
    touchEndPos.y = cEnd->y;
}

const char *CustomCCTableViewCell::getGradeString(int grade)
{
    if (grade == 0)
        return "노멀";
    else if (grade == 1)
        return "레어";
    else if (grade == 2)
        return "슈퍼";
    else if (grade == 3)
        return "울트라";
    else if (grade == 4)
        return "에픽";	
}

void CustomCCTableViewCell::MakeCell(CardInfo *cardInfo, int nCallFrom, int nTeamID)
{
    
    _cardInfo = cardInfo;
    
    LayerIndex = nCallFrom;
    
    //_cardInfo->setRare(1);
    //_cardInfo->SetFusionLevel(4);
    //_cardInfo->setLevel(81);
    
    
    
    CCSprite *pSprBg1 = CCSprite::create("ui/card_tab/card_list_bg.png");
    
    if (pSprBg1 == NULL){
        CCLog("pSprBg1 null");
    }
    
    
    
    pSprBg1->setAnchorPoint(ccp(0,0));
    pSprBg1->setPosition(ccp(0,0));
    this->addChild(pSprBg1, 0);
    
    //cardMaker->MakeCardThumb(cell, cardInfo, accp(12,10), 0, 33);
    
    //CCLog(" CustomCCTableViewCell::MakeCell 10");
    
    MakeCardThumb(this, cardInfo, accp(12,10),0,33);
    
    
    /////////////////////////////////////
    //if (true)return; - ok
    /////////////////////////////////////
        
    CCLabelTTF* pLabel1 = CCLabelTTF::create(cardInfo->getName()   , "HelveticaNeue-Bold", 12);
    pLabel1->setColor(COLOR_WHITE);
    pLabel1->setTag(1);
    registerLabel( this,ccp(0,0.5f), accp(189, 228), pLabel1, 0);
    
    int rare = cardInfo->getRare();
    if (rare > 10)rare = 10;
    //for(int i=0;i<10;i++){
    for(int i=0;i<5;i++){
        if (i > rare){
            cocos2d::CCSprite* pSprStar1 = CCSprite::create("ui/card_tab/card_list_star1.png");
            regSprite(this, ccp(0,0.5f), accp(189+(i*26),196), pSprStar1, 0);
        }
        else{
            cocos2d::CCSprite* pSprStar = CCSprite::create("ui/card_tab/card_list_star2.png");
            regSprite(this, ccp(0,0.5f), accp(189+(i*26),196), pSprStar, 0);
        }
    }
    
    //CCLog(" CustomCCTableViewCell::MakeCell 30");
    
    
    char buff[10];
    sprintf(buff, "Lv. %d", cardInfo->getLevel());
    CCLabelTTF* pLabel2 = CCLabelTTF::create(buff , "HelveticaNeue-Bold", 12);
    pLabel2->setColor(COLOR_WHITE);
    pLabel2->setTag(1);
    registerLabel( this, ccp(0,0.5f), accp(189, 158), pLabel2, 0);
    
    CardInfo *infoFromDic = CardDictionary::sharedCardDictionary()->getInfo(cardInfo->getId());
    infoFromDic->setLevel(cardInfo->getLevel());
    
    double rate = (cardInfo->getLevel() == 40) ? 1.0 : (double)cardInfo->getExp() / (double)cardInfo->getExpCap();
    
    cocos2d::CCSprite* pSprExpGageBg = CCSprite::create("ui/card_tab/card_list_gage_bg.png");
    regSprite(this, ccp(0,0), accp(259, 149), pSprExpGageBg, 0);
    
    cocos2d::CCSprite* pSprExpGage = CCSprite::create("ui/card_tab/card_list_gage.png");
    pSprExpGage->setScaleX(rate);
    regSprite(this, ccp(0,0), accp(265, 155), pSprExpGage, 0);

    sprintf(buff, "(%s)", getGradeString(cardInfo->getRare()));
    CCLabelTTF* pLabel2t = CCLabelTTF::create(buff , "HelveticaNeue", 11);
    pLabel2t->setColor(subBtn_color_normal);
    registerLabel( this, ccp(0,0.5f), accp(329, 196), pLabel2t, 0);

    //cocos2d::ccColor3B text_color1 = cocos2d::ccc3(195,102,8);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create("공격력 ", "HelveticaNeue", 12);
    pLabel3->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(0,0.5f), accp(189, 128), pLabel3, 0);
    
    char labelStr31[10];
	sprintf(labelStr31, "%d ",cardInfo->getAttack());
    CCLabelTTF* pLabel31 = CCLabelTTF::create(labelStr31, "HelveticaNeue-Bold", 12);
    pLabel31->setColor(COLOR_WHITE);
    //registerLabel( this, ccp(0,0.5f), accp(189+70, 128), pLabel31, 0);
    registerLabel( this, ccp(0,0.5f), accp(189+130, 128), pLabel31, 0);
    
    CCLabelTTF* pLabel32 = CCLabelTTF::create("체력 ", "HelveticaNeue", 12);
    pLabel32->setColor(COLOR_ORANGE);
    //registerLabel( this, ccp(0,0.5f), accp(189+85+40+10, 128), pLabel32, 0);
    registerLabel( this, ccp(0,0.5f), accp(189, 98), pLabel32, 0);
    
    char labelStr33[10];
	sprintf(labelStr33, "%d ",cardInfo->getDefence());
    CCLabelTTF* pLabel33 = CCLabelTTF::create(labelStr33, "HelveticaNeue-Bold", 12);
    pLabel33->setColor(COLOR_WHITE);
    //registerLabel( this, ccp(0,0.5f), accp(189+85+40+65+5, 128), pLabel33, 0);
    registerLabel( this, ccp(0,0.5f), accp(189+130, 98), pLabel33, 0);
    //////////////////
    
    CCLabelTTF* pLabel4 = CCLabelTTF::create("배틀포인트"   , "HelveticaNeue", 12);
    pLabel4->setColor(COLOR_ORANGE);
    registerLabel( this, ccp(0,0.5f), accp(189, 68), pLabel4, 0);
    char labelStr41[10];
    sprintf(labelStr41, "%ld ",cardInfo->getBp());
    CCLabelTTF* pLabel41 = CCLabelTTF::create(labelStr41, "HelveticaNeue-Bold", 12);
    pLabel41->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0.5f), accp(189+130, 68), pLabel41, 0);
    
    //if (_cardInfo->bTraingingMaterial==false){
    CCLabelTTF* pLabel5 = CCLabelTTF::create("스킬"   , "HelveticaNeue", 12);
    pLabel5->setColor(COLOR_ORANGE);
    //registerLabel( this, ccp(0,0.5f), accp(189, 68), pLabel5, 0);
    registerLabel( this, ccp(0,0.5f), accp(189, 38), pLabel5, 0);
    
    //CCLog(" CustomCCTableViewCell::MakeCell 50");
    
    
    if (cardInfo->getSkillEffect() > 0)// && cardInfo->getSkill() > 0)
        {
            
            CCLog("cardInfo->getSkillType():%d", cardInfo->getSkillType());
            
            char buff[11];
            sprintf(buff, "cardskill_%d", cardInfo->getSkillType());
            
            char buff1[50];
            sprintf(buff1, LocalizationManager::getInstance()->get(buff), cardInfo->getSkillEffect());

            
            CCLabelTTF* skillDescription = CCLabelTTF::create(buff1, "HelveticaNeue-Bold", 12);
            skillDescription->setColor(COLOR_WHITE);
            registerLabel( this, ccp(0,0.5f), accp(240, 38), skillDescription, 0);
        }
        else
        {
            CCLabelTTF* skillDescription = CCLabelTTF::create(LocalizationManager::getInstance()->get("cardskill_0"), "HelveticaNeue-Bold", 12);
            skillDescription->setColor(COLOR_WHITE);
            registerLabel( this, ccp(0,0.5f), accp(250, 38), skillDescription, 0);
        }
    //}
    //char labelStr61[10];
	//sprintf(labelStr61, "%ld ",cardInfo->get getBp());
    
    //CCLog("card id:%d card level :%d, rare:%d, price :%d", _cardInfo->getId(),  _cardInfo->getLevel(),  _cardInfo->getRare(), _cardInfo->getPrice());
    
    
    char labelStr61[10];
	sprintf(labelStr61, "%d ",cardInfo->getPrice());
    CCLabelTTF* pLabel6 = CCLabelTTF::create(labelStr61, "HelveticaNeue-Bold", 12);
    pLabel6->setColor(COLOR_YELLOW);
    //registerLabel( this, ccp(0,0.5f), accp(189+30, 28), pLabel6, 0);
    registerLabel( this, ccp(0,0.5f), accp(400+30, 128), pLabel6, 0);
    
    cocos2d::CCSprite* pSprCoin = CCSprite::create("ui/card_tab/card_list_coin.png");
    //regSprite(this, ccp(0,0.5f), accp(189, 28), pSprCoin, 0);
    //regSprite(this, ccp(0,0.5f), accp(400, 38), pSprCoin, 0);
    regSprite(this, ccp(0,0.5f), accp(400, 128), pSprCoin, 0);
    
    if (nCallFrom == CALL_CARDLIST_FROM_MYCARD){
        CCLabelTTF* pLabel7 = CCLabelTTF::create("팔기"   , "HelveticaNeue-Bold", 12);
        pLabel7->setColor(COLOR_YELLOW);
        registerLabel(this, ccp(0.5,0.5), accp(568, 132), pLabel7, 0);
    }
    else if (nCallFrom == CALL_CARDLIST_FROM_DECK){
        cocos2d::CCSprite* pSprStar = CCSprite::create("ui/card_tab/team/cards_team_list_select_icon.png");
        regSprite(this, ccp(0,0), accp(550,120), pSprStar, 0);
    }
    else if (nCallFrom == CALL_CARDLIST_FROM_FUSION || nCallFrom == CALL_CARDLIST_FROM_TRAINING){
        CCLabelTTF* pLabel7 = CCLabelTTF::create("선택"   , "HelveticaNeue-Bold", 12);
        pLabel7->setColor(COLOR_YELLOW);
        registerLabel(this, ccp(0.5,0.5), accp(568, 132), pLabel7, 0);
    }
    else if (nCallFrom == CALL_CARDLIST_FROM_TRADE_1){
        CCLabelTTF* pLabel7 = CCLabelTTF::create("TRADE"   , "HelveticaNeue-Bold", 12);
        pLabel7->setColor(subBtn_color_normal);
        registerLabel(this, ccp(0.5,0.5), accp(568, 132), pLabel7, 0);
    }
    else if (nCallFrom == CALL_CARDLIST_FROM_TRADE_2){
        CCLabelTTF* pLabel7 = CCLabelTTF::create("등록"   , "HelveticaNeue-Bold", 12);
        pLabel7->setColor(subBtn_color_normal);
        registerLabel(this, ccp(0.5,0.5), accp(568, 132), pLabel7, 0);
    }
    else if (nCallFrom == CALL_CARDLIST_FROM_TRADE_REG){
        this->removeChildByTag(10, true);
    }
        
    if (PlayerInfo::getInstance()->isBelongInTeam(cardInfo)){
        bool bShowTeamNoti = false;
        if (nTeamID == -1){
            bShowTeamNoti = true;
        }
        else{
            if (PlayerInfo::getInstance()->isBelongInTeam(nTeamID,cardInfo)){
                bShowTeamNoti = true;
            }
        }
        
        if (bShowTeamNoti){
            cocos2d::CCSprite* pSprTeamLabel = CCSprite::create("ui/card_tab/card_list_teamset.png");
            regSprite(this, ccp(0,0), accp(520,50), pSprTeamLabel, 0);
            
            CCLabelTTF* pLabel8 = CCLabelTTF::create("장착중"   , "HelveticaNeue-Bold", 12);
            pLabel8->setColor(cocos2d::ccc3(255,0,0));
            registerLabel( this, ccp(0.5,0.5), accp(565, 70), pLabel8, 0);
        }
        
    }
    
    
}


void CustomCCTableViewCell::MakeCardThumb(CCLayer *layer, CardInfo *card, CCPoint pos, int z, int _tag)
{
    
    int xx = pos.x;
    int yy = pos.y;
    
    std::string path = "ui/card_detail/card_frame_S_4s_0";
    /*
    char path2[1];
    sprintf(path2, "%d", card->GetFusionLevel());
    path.append(path2).append(".png");
    cocos2d::CCSprite* pSpr1 = CCSprite::create(path.c_str());
    pSpr1->setTag(_tag);
    regSprite(layer, ccp(0,0), accp(xx,yy), pSpr1, z+10);
    */
    ///////////////
    
    char path2[1];
    
    sprintf(path2, "%d", (card->getRare()+1));
    path.append(path2).append(".png");
    
    //CCLog("MakeCardThumb, card id:%d rare:%d ", card->getId(), card->getRare());
    
    cocos2d::CCSprite* pSpr2 = CCSprite::create(path.c_str());
    pSpr2->setTag(_tag);
    regSprite(this, ccp(0,0), accp(xx,yy), pSpr2, z+10);
    ///////////
    
    FileManager::sharedFileManager()->requestCardImg(layer, card->getId(), S_SIZE, accp(xx+5, yy+30),1, z, 0);
    
    if (card->getAttribute() == ATRB_GUARD){
        cocos2d::CCSprite* pSpr3 = CCSprite::create("ui/card_detail/card_attribute_guard_s.png");
        regSprite(layer, ccp(0,0), accp(xx+7,yy+197), pSpr3, z+10);
    }
    else if (card->getAttribute() == ATRB_SMASH){
        cocos2d::CCSprite* pSpr3 = CCSprite::create("ui/card_detail/card_attribute_smash_s.png");
        regSprite(layer, ccp(0,0), accp(xx+7,yy+197), pSpr3, z+10);
    }
    else if (card->getAttribute() == ATRB_THROW){
        cocos2d::CCSprite* pSpr3 = CCSprite::create("ui/card_detail/card_attribute_throw_s.png");
        regSprite(layer, ccp(0,0), accp(xx+7,yy+197), pSpr3, z+10);
    }
        
    if (card->GetFusionLevel() > 1)
    {
        for(int i=2;i<=card->GetFusionLevel();i++){
            if (card->getRare() == 4){//5){
                std::string gagePath = "ui/card_detail/card_fusion_S_05_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(55+(20*(i-2)), 26), pSpr, 20);
            }
            else if (card->getRare() == 3){//4){
                std::string gagePath = "ui/card_detail/card_fusion_S_04_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(55+(20*(i-2)), 26), pSpr, 20);
            }
            else{
                std::string gagePath = "ui/card_detail/card_fusion_S_0";
                char temp[1];
                sprintf(temp, "%d", card->getRare()+1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(55+(20*(i-2)), 26), pSpr, 20);
            }   
        }
    }
        
    int nLv = card->getLevel();
    if (nLv>99)nLv=0;
    
    //refreshLevel(layer, nLv,xx+150, yy+10, card->getRare());
    refreshLevel(layer, nLv,xx+152, yy+10, card->getRare());
     
}


void CustomCCTableViewCell::refreshLevel(CCLayer *_layer, int _level, int _x, int _y, int rareLv)
{   
    int x = _x;//84;
    float scale = 0.9f;
    
    /*
     // old
    char buffer[10];
    sprintf(buffer, "%d", _level);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        
        CCSprite *pSprNum = createNumber(number, accp(x, _y), scale, rareLv);
        _layer->addChild(pSprNum, 2000);
        CCSize size = pSprNum->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        pSprNum->setPosition(accp(x, _y));
    }
     */
    
    
    // new
    if (_level > 99)_level=99;
    
    int nn = 1;
    if (_level > 9)nn = 2; // 자리수
    int xx = _x;
    
    for (int i=0;i<nn;i++){
        int n = _level;
        if (i==0)n = _level % 10; // 1 자리
        else if (i==1)n = _level / 10; // 10 자리
        
        if (_level < 10)n = _level;
        
        std::string path = "ui/card_detail/card_number_";
        
        if (rareLv >=2){
            char f[3];
            sprintf(f, "%ds_", rareLv+1);
            path.append(f);
        }
        
        char path2[10];
        sprintf(path2, "%ds.png", n);
        path.append(path2);//.append("s.png");
        
        CCSprite *pSpr5 = CCSprite::create(path.c_str());
        pSpr5->setScale(scale);
        regSprite(this, ccp(1,0), accp(xx,_y), pSpr5, 100);
        xx -= pSpr5->getTexture()->getContentSize().width * 2 * scale - 2;
    }
}
/*
CCSprite *CustomCCTableViewCell::createNumber(int number, cocos2d::CCPoint pos, float scale, int rareLv) {
    if (number < 0)number =0;
    assert(number >= 0 && number < 10);
    char buffer[10];
    sprintf(buffer, "ui/home/number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}
*/
void CustomCCTableViewCell::setSkipTouch(bool a)
{
    if (a)CCLog(" setSkipTouch true");
    else CCLog(" setSkipTouch false");
    bTouchSkip = a;
}

