//
//  BattlePlayerCell.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 23..
//
//

#include "BattlePlayerCell.h"
#include "CardDictionary.h"
#include "CCHttpRequest.h"
#include "CardDetailViewLayer.h"
#include "MainScene.h"

using namespace cocos2d;
using namespace std;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif


BattlePlayerCell::BattlePlayerCell(UserInfo *_user, BattlePlayerCellButtonDelegate *_delegate)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCPoint *posA = new CCPoint(0, MAIN_LAYER_BTN_HEIGHT/SCREEN_ZOOM_RATE);
    CCPoint *posB = new CCPoint(0, size.height - MAIN_LAYER_TOP_UI_HEIGHT/SCREEN_ZOOM_RATE - MAIN_LAYER_TOP_MARGIN/SCREEN_ZOOM_RATE);
    SetTouchArea(posA, posB);
    
    CC_SAFE_DELETE(posA);
    CC_SAFE_DELETE(posB);
    
    user = _user;
    delegate = _delegate;
    cardMaker = new ACardMaker();
    this->setTouchEnabled(true);
    
    //CCTouchDispatcher:: setDispatchEvents(true);
    //CheckLayerSize(this);
    
    InitUI();
    
}
/*
void BattlePlayerCell::onEnter()
{
    CCDirector::sharedDirector()->getTouchDispatcher()->addTargetedDelegate(this, 0, true);
    
    CCLayer::onEnter();
    
}

void BattlePlayerCell::onExit()
{
    CCDirector::sharedDirector()->getTouchDispatcher()->removeDelegate(this);
    
    CCLayer::onExit();
    
}
*/

BattlePlayerCell::~BattlePlayerCell(){
    this->removeAllChildrenWithCleanup(true);
    cardMaker = NULL;
}

extern "C" {
    const char *getUTF8String(const char *text);
};

void BattlePlayerCell::InitUI()
{
    
    CCSprite* pSprite = CCSprite::create("ui/battle_tab/battle_duel_list_bg.png");
    pSprite->setAnchorPoint(ccp(0,0));
    pSprite->setPosition( ccp(0,0) );
    this->addChild(pSprite);
    
    this->setContentSize(CCSize(this->getContentSize().width, accp(pSprite->getTexture()->getContentSizeInPixels().height) ));
    
    
    
    // draw level 
    int x = 93;
    int y = 38;
    float scale = 0.9f;
    char buffer[10];
    if (user->myLevel<10)x-=15;
    
    sprintf(buffer, "%d", user->myLevel);
    int length = strlen(buffer);
    CCSprite *level[3];
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        level[scan] = createNumber(number, accp(x, y), scale);
        this->addChild(level[scan], 2000);
        CCSize size = level[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        level[scan]->setPosition(accp(x, y));
    }
    
    string name = user->myNickname;
    name+=" ";
    CCLabelTTF* pLabel2 = CCLabelTTF::create(name.c_str(), "HelveticaNeue-BoldItalic", 14);
    pLabel2->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0), accp(245,84), pLabel2, 100);
    
    CCSprite* pSprite2 = CCSprite::create("ui/battle_tab/battle_duel_list_bg.png", CCRectMake(500/SCREEN_ZOOM_RATE,0,120/SCREEN_ZOOM_RATE,156/SCREEN_ZOOM_RATE));
    pSprite2->setAnchorPoint(ccp(0,0));
    pSprite2->setPosition( accp(500,0) );
    this->addChild(pSprite2,100);
    
    
    if (delegate != NULL){
        
        CCSprite *pSprBtn = CCSprite::create("ui/battle_tab/battle_duel_list_btl_btn.png");
        pSprBtn->setAnchorPoint(ccp(0,0));
        pSprBtn->setPosition(accp(522,28));
        pSprBtn->setTag(20);
        this->addChild(pSprBtn,100);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create("배틀", "HelveticaNeue-Bold", 12);
        pLabel1->setColor(COLOR_WHITE);
        registerLabel( this, ccp(0.5,0), accp(566,54), pLabel1, 101);
    }
    
    //printf("%s\n", getUTF8String(user->myNickname.c_str()));
    
    char buf1[10];
    sprintf(buf1, "%d", user->numOfKakaoAppFriends);
    char buf2[10];
    if (user->ranking > 0)
        sprintf(buf2, "%d", user->ranking);
    else
        sprintf(buf2, "-");
    std::string text3 = "친구 ";
    
    text3.append(buf1).append(" 랭킹 ").append(buf2);
    CCLabelTTF* pLabel3 = CCLabelTTF::create(text3.c_str(), "Arial-ItalicMT", 13);
    pLabel3->setColor(COLOR_WHITE);
    registerLabel( this, ccp(0,0), accp(245,25), pLabel3, 100);
        
    CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(user->leaderCard);
    if (ci != NULL){
        ci->SetFusionLevel(user->leaderCard%10);
        ci->setAttack(user->attack);
        ci->setDefence(user->defense);
    
        cardMaker->MakeCardThumb(this, ci, ccp(116,13), 130, 0, 10);
    }
    
    //CCSprite* pSprFace = CCSprite::create("ui/battle_tab/photo_01.png");
    //regSprite( this, ccp(0,0), accp(10, 62), pSprFace, 100);
    
    std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(user->profileImageUrl);
    
    if (filename.size() > 0){
        if (FileManager::sharedFileManager()->IsProfileFileExist(filename.c_str())){
            registerUserProfileImg(filename);
        }
        else{
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            std::vector<std::string> downloads;
            downloads.push_back(user->profileImageUrl);
            requestor->addDownloadTask(downloads, this, callfuncND_selector(BattlePlayerCell::profileImgDownloaded));
        }
    }
}


void BattlePlayerCell::registerUserProfileImg(std::string filename)
{
    std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + filename;
    
    //CCLog(" DocumentPath:%s", DocumentPath.c_str());
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
    
    if (pSprFace != NULL){
        CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
        float cardScale = (float)80 / aa.height;
        pSprFace->setScale(cardScale);
        regSprite( this, ccp(0,0), accp(15, 60), pSprFace, 100);
    }
    
}


void BattlePlayerCell::profileImgDownloaded(cocos2d::CCObject *pSender, void *data){
 
     HttpResponsePacket *response = (HttpResponsePacket *)data;
     
     if(response->request->reqType == kHttpRequestDownloadFile)
     {
         if (response->succeed){
             std::vector<std::string>::iterator iter;
             for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter) {
                 std::string str = *iter;
                 
                 registerUserProfileImg(FileManager::sharedFileManager()->getUserProfileFilename(str));

             }
         }
         else{
             // ERROR
         }
     }
 }


void BattlePlayerCell::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("ccTouchesBegan --------------");
    

    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    //CCLog(" ccTouchesBegan, getLocationInView, y:%f",location.y);
    location = CCDirector::sharedDirector()->convertToGL(location);
    //CCLog(" ccTouchesBegan, convertToGL, y:%f", location.y);
    CCPoint localPoint = this->convertToNodeSpace(location);
    //CCLog(" ccTouchesBegan, convertToNodeSpace, y:%f",localPoint.y);
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
    
}
/*
bool BattlePlayerCell::ccTouchBegan(CCTouch *pTouch, CCEvent *pEvent)
{
//    CCPoint touch=pTouch->getLocationInView();
    CCPoint location = pTouch->getLocationInView();
    CCLog(" getLocationInView, x:%f y:%f",location.x, location.y);
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCLog(" convertToGL, x:%f y:%f",location.x, location.y);
    CCPoint localPoint = this->convertToNodeSpace(location);
    CCLog(" convertToNodeSpace, x:%f y:%f",localPoint.x, localPoint.y);
    
    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);

//    if (bTouchBegan)return true;
//    bTouchBegan = true;
    
    CCLog("ccTouchBegan");
    
    return true;
}
*/
static int CardDetailViewMakeCnt;
void BattlePlayerCell::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("--------- ccTouchesEnded");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    //CCLog(" ccTouchesEnded, getLocationInView, y=%f", location.y);
    location = CCDirector::sharedDirector()->convertToGL(location);
    //CCLog(" ccTouchesEnded, convertToGL, y=%f", location.y);
    CCPoint localPoint = this->convertToNodeSpace(location);
    //CCLog(" ccTouchesEnded, convertToNodeSpace, y=%f", localPoint.y);
    
    if (location.y < touchStartPos.y || location.y > touchEndPos.y){
        //CCLog("out of touch range in CustomCCTableViewCell");
        return;
    }
    
    
    if (GetSpriteTouchCheckByTag(this, 10, localPoint) && moving == false){
        CCLog("img click");
        
        if (CardDetailViewMakeCnt>0)
            return;
        
        CardDetailViewMakeCnt++;
        MainScene::getInstance()->HideMainMenu();//ToTopZPriority(this);
        //this->setTouchEnabled(false);
        CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
        
        
        CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
        pSpr0->setAnchorPoint(ccp(0,0));
        pSpr0->setPosition( ccp(0,0) );
        pSpr0->setTag(88);
        MainScene::getInstance()->addChild(pSpr0, 10001);
        
        CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(user->leaderCard);
        cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width,size.height),ci, this);
        //cardDetailViewLayer->setPosition(ccp(-200,0));
        MainScene::getInstance()->addChild(cardDetailViewLayer,10002);
        cardDetailViewLayer->setKeyBlock(true);
    }
    
    if (BattleDuelLayer::getInstance()->nBattleStep == 0){
        if (GetSpriteTouchCheckByTag(this, 20, localPoint) && moving == false){
            if (delegate!= NULL){
                soundButton1();
                delegate->ButtonBattle(user);
            }
        }
    }
}

void BattlePlayerCell::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    float distance = fabs(startPosition.y - location.y);
    //cc_timeval currentTime;
    if (distance > 5.0f)
        moving = true;
    else
        moving = false;
}

void BattlePlayerCell::MenuCallback(CCObject *pSender){
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    /*
    CCPoint pos = node->getPosition();
    CCNode *parent = node->getParent();
    CCPoint parentPos = parent->getPosition();
    CCLog("parent pos:%f %f", parentPos.x, parentPos.y);
    
    CCNode *parent2 = parent->getParent();
    CCPoint parentPos2 = parent2->getPosition();
    
    CCLog("parent2 pos:%f %f", parentPos2.x, parentPos2.y);
    */
    switch(tag){
        case 0:
            //delegate->ButtonEdit(this);
            if (delegate!= NULL){
                soundButton1();
                delegate->ButtonBattle(user);
            }
            // edit
            break;
        case 1:
            // copy
            break;
        case 2:
            // remove
            break;
        case 3:
            break;
        case 4:
            break;
    }
}

void BattlePlayerCell::CloseDetailView()
{
    CardDetailViewMakeCnt--;
    MainScene::getInstance()->ShowMainMenu();//RestoreZProirity(this);
    
    this->removeChild(cardDetailViewLayer, true);
    
    MainScene::getInstance()->removeChildByTag(88,true);
    MainScene::getInstance()->removeChild(cardDetailViewLayer,true);
    
    //this->setTouchEnabled(true);
    
}

void BattlePlayerCell::SetTouchArea(cocos2d::CCPoint *cFrom, cocos2d::CCPoint *cEnd){
    touchStartPos.y = cFrom->y;
    touchEndPos.y = cEnd->y;
}
