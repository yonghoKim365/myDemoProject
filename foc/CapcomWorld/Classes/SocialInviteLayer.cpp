//
//  SocialInviteLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 14..
//
//

#include "SocialInviteLayer.h"
#include "SocialLayer.h"
#include "MainScene.h"
SocialInviteLayer *SocialInviteLayer::instance = NULL;

SocialInviteLayer::SocialInviteLayer(CCSize layerSize)
{
    this->setTouchEnabled(true);
    this->setContentSize(layerSize);
    this->setClipsToBounds(true);
    instance = this;
    
//    xb = new XBridge();
    touchMoved = false;
    
    InitUI();
}

SocialInviteLayer::~SocialInviteLayer()
{
    this->removeAllChildrenWithCleanup(true);
}

void SocialInviteLayer::InitUI()
{
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0, 0));
    pSprBG->setPosition( accp(0, 0) );
    this->addChild(pSprBG, 60);
    
    CCSprite* pInviteBG = CCSprite::create("ui/home/friend_list_frame.png");
    pInviteBG->setAnchorPoint(ccp(0, 0));
    pInviteBG->setPosition(accp(10, 75));
    this->addChild(pInviteBG, 70);
    
    CCSize size = this->getContentSize();
    
    float YPos = (size.height * SCREEN_ZOOM_RATE) - MAIN_LAYER_TOP_MARGIN - 235;
    
    CCLabelTTF* pVisitLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("invite_friend"), "HelveticaNeue-Bold", 12);
    pVisitLabel->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(30, YPos), pVisitLabel, 80);
    
    CCSprite* pXBtn = CCSprite::create("ui/home/friend_list_close_btn1.png");
    pXBtn->setTag(77);
    pXBtn->setAnchorPoint(ccp(0, 0));
    pXBtn->setPosition(accp(578, YPos-4));
    this->addChild(pXBtn, 80);
    
    CCLabelTTF* pGuideText = CCLabelTTF::create(LocalizationManager::getInstance()->get("invite_friend_guide_text"), "HelveticaNeue-Bold", 12);
    pGuideText->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0.5), accp(30, YPos-60), pGuideText, 80);
    
    InitFriendListLayer();
}

void SocialInviteLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    nLastTouchedTag = 0;
    
    //CCLog("SocialInviteLayer::ccTouchesBegan");
          
    if(GetSpriteTouchCheckByTag(this, 77, localPoint)){
        //CCLog("SocialInviteLayer::ccTouchesBegan, close btn pressed");
        nLastTouchedTag = 77;
    }
}

void SocialInviteLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    //CCLog("SocialInviteLayer::ccTouchesEnded, location:%f y:%f", location.x, location.y);
    //CCLog("SocialInviteLayer::ccTouchesEnded, localPoint.x:%f y:%f", localPoint.x, localPoint.y);
    
    //if(false == touchMoved)
    //{
        if(GetSpriteTouchCheckByTag(this, 77, localPoint))
        {
            if (nLastTouchedTag == 77){
            
                //CCLog("SocialInviteLayer::ccTouchesEnded, close btn pressed");
                
                soundButton1();

                //CCLog("클로즈 ");
                SocialLayer* layer = SocialLayer::getInstance();
                layer->setTouchEnabled(true);
                layer->pSocialListlayer->setTouchEnabled(true);
                layer->removeChild(this, true);

                nLastTouchedTag = 0;
            }
        }
    //}
    //else{
    //    CCLog("SocialInviteLayer::ccTouchesEnded, but touchMoved is TRUE");
    //}
    
    touchMoved = false;
}

void SocialInviteLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    touchMoved = true;
}


void SocialInviteLayer::InitFriendListLayer()
{
    pSocialListlayer = new SocialListLayer(CCSize(this->getContentSize().width, this->getContentSize().height), 1);
    
    pSocialListlayer->InitFriendData();
    
    const float LayerHeight = SOCIAL_INVITE_HEIGHT * pSocialListlayer->GetNumOfFriend() + 10 * pSocialListlayer->GetNumOfFriend();
    
    pSocialListlayer->InitLayerSize(CCSize(this->getContentSize().width, LayerHeight));
    
    pSocialListlayer->LayerStartPos = (610 - LayerHeight - HELP_TEXT_ZONE_HEIGHT)/SCREEN_ZOOM_RATE;
    
    pSocialListlayer->InitUI();
    
    pSocialListlayer->setAnchorPoint(ccp(0, 0));
    
    pSocialListlayer->setPosition(accp(0, 610 - LayerHeight - HELP_TEXT_ZONE_HEIGHT));
    
    //pSocialListlayer->setPosition(accp(0, 0));
    
    pSocialListlayer->setTouchEnabled(true);
    
    pSocialListlayer->clip_start_y = 79;
    
    this->addChild(pSocialListlayer, 61);
}
/*
void SocialInviteLayer::visit()
{
	if (clipsToBounds)
    {
        CCRect scissorRect = CCRect(0, 70, this->getContentSize().width, 310);
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (clipsToBounds)
        glDisable(GL_SCISSOR_TEST);
    
}

*/