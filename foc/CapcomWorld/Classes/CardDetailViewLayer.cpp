//
//  CardDetailViewLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 18..
//
//

#include "CardDetailViewLayer.h"
#include "FileManager.h"

CardDetailViewLayer::CardDetailViewLayer(CCSize layerSize, CardInfo *_card, DetailViewCloseDelegate *_delegate, int _directionType)
{
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
    } while (0);
    
    this->setContentSize(layerSize);
    
    this->setTouchEnabled(true);
    
    card = _card;
    
    delegate = _delegate;
    
    this->setClipsToBounds(true);
    
    directionType = _directionType;
    
    
    InitLayer();
    
}

void CardDetailViewLayer::setKeyBlock(bool a){
    if (a){
        setDisableWithRunningScene();
        this->setTouchEnabled(true);
    }
}

CardDetailViewLayer::~CardDetailViewLayer()
{
    //CC_SAFE_DELETE(aniFrames);
    
    CCDirector::sharedDirector()->getTouchDispatcher()->removeDelegate(this);
    this->removeAllChildrenWithCleanup(true);
}


void CardDetailViewLayer::InitLayer()
{
#if(0)
    // 모리건 애니메이션 재생
    
    if(directionType != DIRECTION_CARDPACK_OPEN)
    {
        CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
        pSpr0->setTag(100);
        regSprite(this, ccp(0,0), accp(0,0), pSpr0, 0);
    }
    
    // frame
    std::string framePath = "ui/card_detail/card_frame_L_4s_05.png";
    CCSprite* pSprFrame = CCSprite::create(framePath.c_str());
    regSprite(this, ccp(0.0f, 0.0f), accp(13.0f, 17.0f), pSprFrame, 10);

    std::string charPath = "ui/card_ani/Morrigan_";
    CCSpriteFrame* pSprFrameChar[10];
    
    pSprFrameChar[0] = CCSpriteFrame::create("ui/card_ani/Morrigan_01.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[1] = CCSpriteFrame::create("ui/card_ani/Morrigan_02.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[2] = CCSpriteFrame::create("ui/card_ani/Morrigan_03.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[3] = CCSpriteFrame::create("ui/card_ani/Morrigan_04.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[4] = CCSpriteFrame::create("ui/card_ani/Morrigan_05.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[5] = CCSpriteFrame::create("ui/card_ani/Morrigan_06.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[6] = CCSpriteFrame::create("ui/card_ani/Morrigan_07.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[7] = CCSpriteFrame::create("ui/card_ani/Morrigan_08.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[8] = CCSpriteFrame::create("ui/card_ani/Morrigan_09.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    pSprFrameChar[9] = CCSpriteFrame::create("ui/card_ani/Morrigan_10.png", CCRectMake(0, 0, 616/SCREEN_ZOOM_RATE, 926/SCREEN_ZOOM_RATE));
    
    aniFrames = new CCArray();
    for (int i=0; i<10; i++)
    {
        aniFrames->addObject(pSprFrameChar[i]);
    }
    
    CCSize size = GameConst::WIN_SIZE;
    
    regAni(aniFrames, this, ccp(0.5f, 0.5f), ccp(size.width/2.0f, size.height/2.0f), 22, 0, 0.1f);
#endif
#if(1)
    if(directionType != DIRECTION_CARDPACK_OPEN)
    {
        CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
        pSpr0->setTag(100);
        regSprite(this, ccp(0,0), accp(0,0), pSpr0, 0);
    }
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/card/card_load_01.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/card/card_load_02.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/card/card_load_03.png", CCRectMake(0,0,222/SCREEN_ZOOM_RATE,191/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);

    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    
    regAni(aniFrame, this, ccp(0.5f, 0.5f), ccp(size.width/2, size.height/2), 22, 0);
    
    FileManager::sharedFileManager()->requestCardImg(this, card->getId(), L_SIZE, accp(23+13,107+17), 1.0, 0, 33, directionType);
    
    // frame
    std::string path = "ui/card_detail/card_frame_L_4s_0";

    char path2[1];
    
    sprintf(path2, "%d", card->getRare()+1);
    path.append(path2).append(".png");
    cocos2d::CCSprite* pSpr2 = CCSprite::create(path.c_str());
    regSprite(this, ccp(0,0), accp(13,17), pSpr2, 10);//34);
    
    // attribute
    if (card->getAttribute() == ATRB_GUARD){
        CCSprite *pSpr3 = CCSprite::create("ui/card_detail/card_attribute_guard.png");
        regSprite(this, ccp(0,0), accp(21+13,815+17), pSpr3, 20);//34);
    }
    else if (card->getAttribute() == ATRB_SMASH){
        CCSprite *pSpr3 = CCSprite::create("ui/card_detail/card_attribute_smash.png");
        regSprite(this, ccp(0,0), accp(21+13,815+17), pSpr3, 20);//34);
    }
    else if (card->getAttribute() == ATRB_THROW){
        CCSprite *pSpr3 = CCSprite::create("ui/card_detail/card_attribute_throw.png");
        regSprite(this, ccp(0,0), accp(21+13,815+17), pSpr3, 20);//34);
    }
    
    if (card->GetFusionLevel() > 1)
    {
        for(int i=2;i<=card->GetFusionLevel();i++){
            if (card->getRare() == 4){
                std::string gagePath = "ui/card_detail/card_fusion_L_05_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(223+(85*(i-2)), 101), pSpr, 20);
            }
            else if (card->getRare() == 3){
                std::string gagePath = "ui/card_detail/card_fusion_L_04_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(223+(85*(i-2)), 101), pSpr, 20);
            }
            else{
                std::string gagePath = "ui/card_detail/card_fusion_L_0";
                char temp[1];
                sprintf(temp, "%d", card->getRare()+1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                regSprite(this, ccp(0,0), accp(223+(85*(i-2)), 101), pSpr, 20);
            }
            
        }
    }
    
    int xx = 538+13+35;
    
    if (card->getLevel() > 0){
        regNumber(card->getLevel(), ccp(xx,61+17), card->getRare());
    }
    
    cocos2d::ccColor3B COLOR1 = cocos2d::ccc3(254,215,0);
    cocos2d::ccColor3B COLOR2 = cocos2d::ccc3(230,69,6);
    cocos2d::ccColor3B COLOR3 = cocos2d::ccc3(0,170,255);
    
    if (card->getRare() >= 4){
        COLOR1 = COLOR_YELLOW;
        COLOR2 = COLOR_YELLOW;
        COLOR3 = COLOR_YELLOW;
    }
    
    drawInt(this, card->getDefence(),  COLOR1, accp(143+12,37+14));
    drawInt(this, card->getAttack(), COLOR2, accp(240+16,37+14));
    drawInt(this, card->getBp(), COLOR3, accp(338+16,37+14));
    
    aniFrame->autorelease();
#endif
}

// 우측 정렬하여 숫자 찍음 
void CardDetailViewLayer::regNumber(int num, CCPoint pos, int rareLv)
{
    if (num > 99)num=99;
    
    int nn = 1;
    if (num > 9)nn = 2; // 자리수
    int xx = pos.x;
    
    //CCLog("regNumber:%d, jarisu:%d",num,nn);
    
        for (int i=0;i<nn;i++)
        {
            int n = num;
            if (i==0)n = num % 10; // 1 자리
            else if (i==1)n = num / 10; // 10 자리
            
            if (num < 10)n = num;
            
            std::string path = "ui/card_detail/card_number_";//0.png";
            
            if (rareLv >=3){
                char f[5];
                sprintf(f, "%ds_", rareLv);
                path.append(f);
            }
            
            char path2[1];
            sprintf(path2, "%d", n);
            path.append(path2).append(".png");
            
            CCSprite *pSpr5 = CCSprite::create(path.c_str());
            regSprite(this, ccp(1,0), accp(xx,pos.y), pSpr5, 100);
            xx -= pSpr5->getTexture()->getContentSize().width*2;
        }
}

void CardDetailViewLayer::MenuCallback(cocos2d::CCObject *pSender)
{
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    switch(tag){
        case 0:
            break;
    }
}

/*
void CardDetailViewLayer::registerWithTouchDispatcher()
{
    CCDirector::sharedDirector()->getTouchDispatcher()->addTargetedDelegate(this, 10, true);//false);
}
*/
 



void CardDetailViewLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    //CCLog("touch began");
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartLocation = location;
    
//    return true;
    
}

void CardDetailViewLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    if(fabs(touchStartLocation.x - location.x) < 10 && fabs(touchStartLocation.y - location.y) < 10)
    {
        CCPoint localPoint = this->convertToNodeSpace(location);
        
        if(GetSpriteTouchCheckByTag(this, 100, location))
        {
            if (delegate != NULL)
            {
                restoreTouchDisable();
                
                delegate->CloseDetailView();
            }
        }
    }
    
}

void CardDetailViewLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event){
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
}

void CardDetailViewLayer::visit()
{
    
	if (clipsToBounds)
    {
        //CCRect scissorRect = CCRect(0.0f, 10.0f, CCDirector::sharedDirector()->getWinSize().width, CCDirector::sharedDirector()->getWinSize().height - 20.0f);
        
        CCRect scissorRect = CCRect(0.0f, 10.0f, GameConst::WIN_SIZE.width, GameConst::WIN_SIZE.height - 20.0f);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();
    
    if (clipsToBounds)
        glDisable(GL_SCISSOR_TEST);
     
}
