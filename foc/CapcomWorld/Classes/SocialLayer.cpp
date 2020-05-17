//
//  SocialLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 3..
//
//

#include "SocialLayer.h"
#include "MainScene.h"
#include "FriendsInfo.h"
#include "ARequestSender.h"

SocialLayer* SocialLayer::instance = NULL;

SocialLayer* SocialLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}

SocialLayer::SocialLayer(CCSize layerSize) : pSocialListlayer(NULL), pInviteLayer(NULL)
{
    this->setContentSize(layerSize);
    this->setTouchEnabled(true);
    
    instance = this;
    
    touchMoved = false;
    
//    MainScene *mainScene = MainScene::getInstance();
    //ARequestSender::getInstance()->requestFriendsToGameServer();
    /*
    for(int i=0;i<PlayerInfo::getInstance()->gameFriendsInfo->count();i++){
        FriendsInfo *info = (FriendsInfo*)PlayerInfo::getInstance()->gameFriendsInfo->objectAtIndex(i);
        
        info->profileURL = PlayerInfo::getInstance()->getGameUserProfileURL(info->userID);
        
        CCLog("player id :%lld", info->userID);
        CCLog("player nick :%s", info->nickname);
        CCLog(" player lev :%d", info->level);
        CCLog(" player ranking:%d", info->ranking);
        CCLog(" info->profileURL:%s", info->profileURL);
         
    }
    )*/
    InitUI();
}

SocialLayer::~SocialLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void SocialLayer::InitUI()
{
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0, 0));
    pSprBG->setPosition( accp(0, 0) );
    this->addChild(pSprBG, 60);
    
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    float YPos = (size.height*SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_UI_HEIGHT - 150;
    
    CCSprite* pVisit = CCSprite::create("ui/home/home_invite_btn1.png");
    pVisit->setAnchorPoint(ccp(0, 0));
    pVisit->setPosition(accp(10, YPos));
    pVisit->setTag(0);
    this->addChild(pVisit, 60);
    
    CCLabelTTF* pVisitLabel = CCLabelTTF::create("친구를 초대하세요", "HelveticaNeue-Bold", 12);
    pVisitLabel->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(263, YPos + 12), pVisitLabel, 65);
    
    YPos-= SOCIAL_HELP_TEXT_ZONE_H;
    
    
    //현재 친구 : X1 / 현재 추가 공격력 : +1
    
    std::string text = "친구가 늘어날 때마다 공격력이 늘어나고 \n친구로부터 메달을 받을 수 있는 기회가 많아집니다!\n 현재친구 : ";
    char buf1[3];
    sprintf(buf1, "%d", PlayerInfo::getInstance()->numOfKakaoAppFriends);
    char buf2[3];
    sprintf(buf2, "%d", PlayerInfo::getInstance()->numOfKakaoAppFriends * PlayerInfo::getInstance()->friends_bonus);
    text.append(buf1).append("  추가 공격력 : ").append(buf2);
    
    CCLabelTTF* pHelpTextLabel = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pHelpTextLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0.5, 0), ccp(size.width/2, accp(YPos)), pHelpTextLabel, 65);
    
    InitSocialListLayer();
}

void SocialLayer::InitSocialListLayer()
{
    int layer_h = 610 - SOCIAL_HELP_TEXT_ZONE_H;
    pSocialListlayer = new SocialListLayer(CCSize(this->getContentSize().width, this->getContentSize().height), 0);
    
    pSocialListlayer->InitFriendData();
    
    const float ListLayerHeight = SOCIAL_MEDAL_HEIGHT * pSocialListlayer->GetNumOfFriend() + pSocialListlayer->GetNumOfFriend() * 10;
    
    pSocialListlayer->InitLayerSize(CCSize(this->getContentSize().width, ListLayerHeight));
    
    pSocialListlayer->LayerStartPos = (layer_h - ListLayerHeight)/SCREEN_ZOOM_RATE;
    
    pSocialListlayer->InitUI();
    
    pSocialListlayer->setAnchorPoint(ccp(0, 0));
    
    pSocialListlayer->setPosition(accp(0, layer_h - ListLayerHeight));
    
    //pSocialListlayer->setPosition(accp(0, 0));
    
    pSocialListlayer->setTouchEnabled(true);
    
    this->addChild(pSocialListlayer, 60);
}

void SocialLayer::InitInviteLayer()
{
    pInviteLayer = new SocialInviteLayer(CCSize(this->getContentSize().width, this->getContentSize().height));
    
    pInviteLayer->setAnchorPoint(ccp(0, 0));
    
    pInviteLayer->setPosition(accp(0, 0));
    
    pInviteLayer->setTouchEnabled(true);
    
    this->addChild(pInviteLayer, 100);
}

void SocialLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 0, localPoint))
    {
        ChangeSpr(this, 0, "ui/home/home_invite_btn2.png", 60);
    }
}

void SocialLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(false == touchMoved)
    {
        if(GetSpriteTouchCheckByTag(this, 0, localPoint))
        {
            CCLog("press friend invite layer");
            
            soundButton1();
            
            ChangeSpr(this, 0, "ui/home/home_invite_btn1.png", 60);
            
            this->setTouchEnabled(false);
            pSocialListlayer->setTouchEnabled(false);
            InitInviteLayer();
        }
    }
    
    touchMoved = false;
}

void SocialLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    touchMoved = true;
}
