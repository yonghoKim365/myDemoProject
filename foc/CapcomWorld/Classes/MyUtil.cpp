//
//  MyUtil.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#include "MyUtil.h"
#include "md5.h"
#include "PopupLayer.h"
#include "MainScene.h"
#include "PopupOkCancel.h"
#include "PopupRetry.h"
using namespace cocos2d;

bool MyUtil::popupOpened = true;
int MyUtil::tutorialProgress = 0;

CCPoint MyUtil::accp(float x, float y){
    
    //CCPoint c = *new CCPoint(x / SCREEN_ZOOM_RATE, y / SCREEN_ZOOM_RATE);
    
    //return c;
    
    return CCPoint(x / SCREEN_ZOOM_RATE, y / SCREEN_ZOOM_RATE);
    
}

float MyUtil::accp(float x){
    
    return x/SCREEN_ZOOM_RATE;
    
}

int MyUtil::accp(int  x){
    
    return x/SCREEN_ZOOM_RATE;
    
}

CCSprite *MyUtil::createNumber(int number, cocos2d::CCPoint pos, float scale) {
    assert(number >= 0 && number < 10);
    char buffer[32];
    sprintf(buffer, "ui/home/number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

void MyUtil::registerLabel(CCLayer *_layer,  CCPoint anchor, CCPoint pos, CCLabelTTF* pLabel, int z){
    pLabel->setAnchorPoint(anchor);
    pLabel->setPosition(pos);
    _layer->addChild(pLabel, z);
}

void MyUtil::regSprite(cocos2d::CCLayer *_layer, cocos2d::CCPoint anchor, cocos2d::CCPoint pos, cocos2d::CCSprite *pSpr, int z){
    pSpr->setAnchorPoint(anchor);
    pSpr->setPosition(pos);
    _layer->addChild(pSpr, z);
}

void MyUtil::regSprite(cocos2d::CCLayer *_layer, cocos2d::CCPoint anchor, cocos2d::CCPoint pos, cocos2d::CCSprite *pSpr, int z, int _tag){
    pSpr->setAnchorPoint(anchor);
    pSpr->setPosition(pos);
    pSpr->setTag(_tag);
    _layer->addChild(pSpr, z);
}

void MyUtil::regSprites(cocos2d::CCLayer *_layer, int nSprite, std::string _paths[], float anc[], float pos[], float z[], int tags_start)
{
    /*
    CCLog("regSprites, paths->length():%d",_paths->length());
    CCLog("regSprites, sizeof(path):%d",sizeof(_paths));
    CCLog("regSprites, _paths->size():%d",_paths->size());
    CCLog("regSprites, _paths->capacity():%d",_paths->capacity());
    CCLog("regSprites, _paths->max_size():%d",_paths->max_size());
    CCLog("regSprites, _paths->npos:%d",_paths->npos);
     */
    //for(int i=0;i<sizeof(_paths);i++){
    for(int i=0;i<nSprite;i++){
        //CCLog("i:%d, paths = %s", i, _paths[i].c_str());
        //CCLog("i:%d, anc, %f %f",i, anc[i*2], anc[i*2+1]);
        CCSprite *pSpr = CCSprite::create(_paths[i].c_str());
        pSpr->setAnchorPoint(CCPoint(anc[i*2], anc[i*2+1]));
        pSpr->setPosition(accp(pos[i*2], pos[i*2+1]));
        pSpr->setTag(i+tags_start);
        _layer->addChild(pSpr,z[i]);
    }
}

void MyUtil::CheckLayerSize(cocos2d::CCLayer *layer)
{
    CCSize s = layer->getContentSize();
    
    CCSprite* pSprTest     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(0,0), accp(0,0), pSprTest, 10000);
    
    CCSprite* pSprTest12     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(0,0), accp(50,0), pSprTest12, 10000);
    
    CCSprite* pSprTest13     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(0,0), accp(100,0), pSprTest13, 10000);
    
    //CCSprite* pSprTest14     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    //regSprite(layer, ccp(0,0), accp(150,50), pSprTest14, 10000);
    
    CCSprite* pSprTest21     = CCSprite::create("ui/home/ui_menu_sub_01_2.png");
    //CCSprite* pSprTest22     = CCSprite::create("ui/home/ui_menu_sub_01_2.png");
    //CCSprite* pSprTest23     = CCSprite::create("ui/home/ui_menu_sub_01_2.png");
    //CCSprite* pSprTest24     = CCSprite::create("ui/home/ui_menu_sub_01_2.png");

    regSprite(layer, ccp(0,1), ccp(0,s.height), pSprTest21, 10000);
    
    //regSprite(layer, ccp(0,1), ccp(100,100), pSprTest22, 10000);
    
    //regSprite(layer, ccp(0,0), ccp(50,100), pSprTest23, 10000);
    
    
}

void MyUtil::CheckSize(cocos2d::CCLayer *layer, cocos2d::CCRect rect)
{
    
    CCSprite* pSprBL     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(0,0), ccp(rect.origin.x,rect.origin.y), pSprBL, 10000);
    
    CCSprite* pSprBR     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(1,0), ccp(rect.size.width,rect.origin.y), pSprBR, 10000);
    
    CCSprite* pSprTL     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(0,1), ccp(rect.origin.x,rect.size.height), pSprTL, 10000);
    
    CCSprite* pSprTR     = CCSprite::create("ui/home/ui_menu_sub_01_1.png");
    regSprite(layer, ccp(1,1), ccp(rect.size.width,rect.size.height), pSprTR, 10000);
    
}

/**
const char* MyUtil::GetCharImgPath(int nCharId, int nSize){
    
    std::string path = "images/card/thumbnail/chunli_star01_s.png";
    
    switch(nCharId){
        case 1:
            break;
        default:
            path = "images/card/thumbnail/bison_star01_s.png";
            break;
    }
    
    return path.c_str();
}
**/
void MyUtil::ToTopZPriority(CCLayer *_layer)
{
    
    bool bLoop = true;
    CCLayer *curLayer = _layer;
    CCLayer *childLayer = _layer;
    int cnt=0;
    while(bLoop){
        
        layerYPositions[cnt] = curLayer->getPositionY();//().y;
        layerZorders[cnt] = curLayer->getZOrder();
        
        curLayer->setPositionY(0);
        
//      CCLog("layerYPositions[cnt]:%f",layerYPositions[cnt]);
        
        if (curLayer->getParent()!=NULL){
            curLayer->getParent()->reorderChild(childLayer, 10000);
            childLayer = curLayer;
            curLayer = (CCLayer*)curLayer->getParent();
            
            //CCLog(" ToTopZPriority ");
            cnt++;
        }
        else bLoop = false;
    }
}

void MyUtil::RestoreZProirity(CCLayer *_layer)
{
    bool bLoop = true;
    CCLayer *curLayer = _layer;
    CCLayer *childLayer = _layer;
    int cnt=0;
    while(bLoop){
        
        curLayer->setPositionY(layerYPositions[cnt]);
        
        if (curLayer->getParent()!=NULL){
            curLayer->getParent()->reorderChild(childLayer, layerZorders[cnt]);
            childLayer = curLayer;
            curLayer = (CCLayer*)curLayer->getParent();
            
            //CCLog(" RestoreZProirity ");
            
            cnt++;
        }
        else bLoop = false;
    }
}

bool MyUtil::GetSpriteTouchCheckByTag(CCLayer *layer, int tag, cocos2d::CCPoint localPoint)
{
    CCSprite *btn1 = (CCSprite*)layer->getChildByTag(tag);
    if (btn1 != NULL)
    {
        CCRect *pic1rect = new CCRect(btn1->boundingBox());
        
        if (pic1rect != NULL){
            if (pic1rect->containsPoint(localPoint))
            {
                CC_SAFE_DELETE(pic1rect);
                return true;
            }
            else
            {
                CC_SAFE_DELETE(pic1rect);
            }
        }
    }
    return false;
}

void MyUtil::removeNumSprites(CCLayer *_layer)
{
    for (int scan = 0;scan < 7;scan++)
    {
        if (numSprs[scan] != NULL)
            _layer->removeChild(numSprs[scan], true);
        numSprs[scan] = NULL;
    }
}


void MyUtil::format_commas(int n, char *out)
{
    int c;
    char buf[20];
    char *p;
    
    sprintf(buf, "%d", n);
    c = 2 - strlen(buf) % 3;
    for (p = buf; *p != 0; p++) {
        *out++ = *p;
        if (c == 1) {
            *out++ = ',';
        }
        c = (c + 1) % 3;
    }
    *--out = 0;
}

CCSprite *MyUtil::createComma(cocos2d::CCPoint pos, float scale) {
    
    CCSprite *sprite = CCSprite::create("ui/home/number_comma.png");
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

void MyUtil::RefreshNumber(int point, int x, int y, CCLayer *layer)
{
    removeNumSprites(layer);
    //int x = 589;
    float scale = 1.0f;
    char buffer[7];
    format_commas(point, buffer);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        if (buffer[scan] == ',')
            //cash[scan] = createComma(accp(x, 917), scale);
            numSprs[scan] = createComma(accp(x, y-3), scale);
        else
        {
            int number = buffer[scan] - '0';
            //cash[scan] = createNumber(number, accp(x, 920), scale);
            numSprs[scan] = createNumber(number, accp(x, y), scale);
        }
        layer->addChild(numSprs[scan], 2000);
        CCSize size = numSprs[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        if (buffer[scan] == ',')
            numSprs[scan]->setPosition(accp(x, y-3));//917));
        else
            numSprs[scan]->setPosition(accp(x, y));//920));
    }
}

void MyUtil::regAni(CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, int _tag, int _z, float delay, float scale)
{
    /*
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_1.png", CCRectMake(0,0,310,70));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_2.png", CCRectMake(0,0,310,70));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_3.png", CCRectMake(0,0,310,70));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/battle_tab/battle_duel_ready_start_4.png", CCRectMake(0,0,310,70));
    */
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrames->objectAtIndex(0);
    
    //CCSprite* pSprAni = CCSprite::spriteWithSpriteFrame(aa1);
    CCSprite* pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(anc);
	pSprAni->setPosition(pos);
    pSprAni->setScale(scale);
    pSprAni->setTag(_tag);
	_layer->addChild(pSprAni,_z);
    
    /*
     CCArray *aniFrame = new CCArray();
     aniFrame->addObject(aa1);
     aniFrame->addObject(aa2);
     aniFrame->addObject(aa3);
     aniFrame->addObject(aa4);
     
     //CCAnimation *animation = CCAnimation::create(aniFrame, 0.2f,10);
     */
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrames->count();i++){
            animation->addSpriteFrame((CCSpriteFrame*)aniFrames->objectAtIndex(i));
    }
    animation->setLoops(-1);
    animation->setDelayPerUnit(delay);
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
    CCRepeatForever *repeat = CCRepeatForever::actionWithAction(animate);
	pSprAni->runAction( repeat );
}

void MyUtil::drawInt(cocos2d::CCLayer *layer, int number, cocos2d::ccColor3B color, cocos2d::CCPoint pos)
{
        char buf1[10];
    if (number == 0){
        sprintf(buf1, "%s", "-");
    }
    else{
        sprintf(buf1, "%d", number);
    }
    
        //cocos2d::ccColor3B COLOR_WHITE  = cocos2d::ccc3(255,255,255);
        
        CCLabelTTF* pLabel1 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
        pLabel1->setColor(color);
        registerLabel( layer ,ccp(0,0), pos, pLabel1, 100);
}

void MyUtil::requestCardImg(CCLayer *player, int CardID, int SizeType, CCPoint pos, float scale, int z,  int tag){
    
    
    
    //FileManager::sharedFileManager()->requestCardImg(player, CardID, SizeType, pos, scale, z, tag);
    
    
}

unsigned long MyUtil::GetTimeTick()
{
    timeval time;
    gettimeofday(&time, NULL);
    unsigned long millisecs = (time.tv_sec * 1000) + (time.tv_usec / 1000);
    
    return millisecs;
}

void MyUtil::moveSprite(CCSprite* pSpr, float x, float y, float duration, CCObject* pObj, SEL_CallFunc selector)
{
    CCFiniteTimeAction* actionMove = CCMoveTo::actionWithDuration(duration, accp(x, y));
    
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(pObj, selector);
    
    pSpr->runAction(CCSequence::actions(actionMove, callBack, NULL));
}


void MyUtil::AniPlay(CCSprite* pSprAni, CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, float scale, int _tag, int _z, SEL_CallFunc selector)
{
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrames->objectAtIndex(0);
    
    pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(anc);
	pSprAni->setPosition(pos);
    pSprAni->setScale(scale);
    pSprAni->setTag(_tag);
	_layer->addChild(pSprAni,_z);
    
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrames->count();i++)
    {
        animation->addSpriteFrame((CCSpriteFrame*)aniFrames->objectAtIndex(i));
    }
    animation->setLoops(1);
    animation->setDelayPerUnit(0.03f);
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
    CCCallFunc *callBack = CCCallFunc::actionWithTarget(_layer, selector);
	pSprAni->runAction(CCSequence::actions(animate, callBack, NULL));
}

void MyUtil::AniPlay(CCSprite* pSprAni, CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, float scale, int _tag, int _z, SEL_CallFuncND selector, int repeat)
{
    CCSpriteFrame *aa1 = (CCSpriteFrame*)aniFrames->objectAtIndex(0);
    
    pSprAni = CCSprite::createWithSpriteFrame(aa1);
    pSprAni->setAnchorPoint(anc);
	pSprAni->setPosition(pos);
    pSprAni->setScale(scale);
    pSprAni->setTag(_tag);
	_layer->addChild(pSprAni,_z);
    
    CCAnimation *animation = CCAnimation::create();
    
    for(int i=0;i<aniFrames->count();i++)
    {
        animation->addSpriteFrame((CCSpriteFrame*)aniFrames->objectAtIndex(i));
    }
    animation->setLoops(repeat);
    animation->setDelayPerUnit(0.03f);
    
    int *tag = new int(_tag);
    
    CCAnimate *animate = CCAnimate::actionWithAnimation(animation);
    CCCallFuncND *callBack = CCCallFuncND::actionWithTarget(_layer, selector, (void*)tag);
	pSprAni->runAction(CCSequence::actions(animate, callBack, NULL));

}

void MyUtil::ChangeSpr(CCLayer *_layer, int _tag, std::string newSprPath, int z)
{
    CCNode* pNode = _layer->getChildByTag(_tag);
    
    if(pNode)
    {
        CCSprite* pSpr = (CCSprite*)pNode;
        float x = pSpr->getPositionX();
        float y = pSpr->getPositionY();
        
        _layer->removeChildByTag(_tag, true);
        
        CCSprite* pBtn = NULL;
        
        pBtn  = CCSprite::create(newSprPath.c_str());
        
        pBtn->setTag(_tag);
        pBtn->setAnchorPoint(ccp(0, 0));
    pBtn->setPosition(accp(x*SCREEN_ZOOM_RATE, y*SCREEN_ZOOM_RATE));
        _layer->addChild(pBtn, z);
    }
}

void MyUtil::ChangeSpr(CCLayer *_layer, int _tag, int _newTag, std::string newSprPath, int z)
{
    CCNode* pNode = _layer->getChildByTag(_tag);
    CCSprite* pSpr = (CCSprite*)pNode;
    float x = pSpr->getPositionX();
    float y = pSpr->getPositionY();
    
    _layer->removeChildByTag(_tag, true);
    
    CCSprite* pBtn = NULL;
    
    pBtn  = CCSprite::create(newSprPath.c_str());
    
    pBtn->setTag(_newTag);
    pBtn->setAnchorPoint(ccp(0, 0));
    pBtn->setPosition(accp(x*2, y*2));
    _layer->addChild(pBtn, z);
}

void MyUtil::addLoadingAni()
{
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setAnchorPoint(ccp(0.5f, 0.5f));
    BG->setPosition(accp(320.0f, 480.0f));
    BG->setTag(101012);
    MainScene::getInstance()->addChild(BG, 10000);
    
    CCSprite* loadingBG = CCSprite::create("ui/home/loading_bg.png");
    loadingBG->setAnchorPoint(ccp(0.5f, 0.5f));
    loadingBG->setPosition(accp(320.0f, 480.0f));
    loadingBG->setTag(101010);
    MainScene::getInstance()->addChild(loadingBG, 10000);
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/home/loading_1.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/home/loading_2.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/home/loading_3.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/home/loading_4.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/home/loading_5.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/home/loading_6.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/home/loading_7.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa8 = CCSpriteFrame::create("ui/home/loading_8.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);
    aniFrame->addObject(aa8);
    aniFrame->autorelease();
    regAni(aniFrame, MainScene::getInstance(), ccp(0.5f, 0.5f), accp(320.0f - 70.0f, 480.0f), 101011, 10000, 0.1f);
}

void MyUtil::removeLoadingAni()
{
    MainScene::getInstance()->removeChildByTag(101010, true);
    MainScene::getInstance()->removeChildByTag(101011, true);
    MainScene::getInstance()->removeChildByTag(101012, true);
}

void MyUtil::removeLoadingAni(CCLayer *layer)
{
    layer->removeChildByTag(101010, true);
    layer->removeChildByTag(101011, true);
    layer->removeChildByTag(101012, true);
}

void MyUtil::addPageLoading()
{
    CCNode* node = MainScene::getInstance()->getChildByTag(101011);
    
    if(!node)
    {
        CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/home/loading_1.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/home/loading_2.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/home/loading_3.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/home/loading_4.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/home/loading_5.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/home/loading_6.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/home/loading_7.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa8 = CCSpriteFrame::create("ui/home/loading_8.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        
        CCArray *aniFrame = new CCArray();
        aniFrame->addObject(aa1);
        aniFrame->addObject(aa2);
        aniFrame->addObject(aa3);
        aniFrame->addObject(aa4);
        aniFrame->addObject(aa5);
        aniFrame->addObject(aa6);
        aniFrame->addObject(aa7);
        aniFrame->addObject(aa8);
        aniFrame->autorelease();
        regAni(aniFrame, MainScene::getInstance(), ccp(0.5f, 0.5f), accp(320.0f, 480.0f), 101011, 10000, 0.1f, 2.0f);
    }
}

void MyUtil::addPageLoading(CCLayer *layer)
{
    CCNode* node = layer->getChildByTag(101011);
    
    if(!node)
    {
        CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/home/loading_1.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/home/loading_2.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/home/loading_3.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/home/loading_4.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/home/loading_5.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/home/loading_6.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/home/loading_7.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        CCSpriteFrame *aa8 = CCSpriteFrame::create("ui/home/loading_8.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
        
        CCArray *aniFrame = new CCArray();
        aniFrame->addObject(aa1);
        aniFrame->addObject(aa2);
        aniFrame->addObject(aa3);
        aniFrame->addObject(aa4);
        aniFrame->addObject(aa5);
        aniFrame->addObject(aa6);
        aniFrame->addObject(aa7);
        aniFrame->addObject(aa8);
        aniFrame->autorelease();
        regAni(aniFrame, layer, ccp(0.5f, 0.5f), accp(640/2, 960/2-280), 101011, 10000, 0.1f, 2.0f);
    }
}

void MyUtil::removePageLoading()
{
    MainScene::getInstance()->removeChildByTag(101011, true);
}

void MyUtil::removePageLoading(CCLayer *layer)
{
    layer->removeChildByTag(101011, true);
}

void MyUtil::addLoadingAni(CCLayer *layer)
{
    CCSprite* BG = CCSprite::create("ui/home/ui_BG.png");
    BG->setAnchorPoint(ccp(0.5f, 0.5f));
    BG->setPosition(ccp(layer->getContentSize().width/2, layer->getContentSize().height/2));
    BG->setTag(101012);
    layer->addChild(BG, 10000);
    
    CCSprite* loadingBG = CCSprite::create("ui/home/loading_bg.png");
    loadingBG->setAnchorPoint(ccp(0.5f, 0.5f));
    loadingBG->setPosition(ccp(layer->getContentSize().width/2, layer->getContentSize().height/2));
    loadingBG->setTag(101010);
    layer->addChild(loadingBG, 10000);
    
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/home/loading_1.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/home/loading_2.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa3 = CCSpriteFrame::create("ui/home/loading_3.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa4 = CCSpriteFrame::create("ui/home/loading_4.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa5 = CCSpriteFrame::create("ui/home/loading_5.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa6 = CCSpriteFrame::create("ui/home/loading_6.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa7 = CCSpriteFrame::create("ui/home/loading_7.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa8 = CCSpriteFrame::create("ui/home/loading_8.png", CCRectMake(0,0,41/SCREEN_ZOOM_RATE,41/SCREEN_ZOOM_RATE));
    
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->addObject(aa3);
    aniFrame->addObject(aa4);
    aniFrame->addObject(aa5);
    aniFrame->addObject(aa6);
    aniFrame->addObject(aa7);
    aniFrame->addObject(aa8);
    aniFrame->autorelease();
    //regAni(aniFrame, layer, ccp(0.5f, 0.5f), accp(320.0f - 70.0f, 480.0f), 101011, 10000, 0.1f);
    regAni(aniFrame, layer, ccp(0.5f, 0.5f), ccp(layer->getContentSize().width/2 - accp(70.0f), layer->getContentSize().height/2), 101011, 10000, 0.1f);
}

void MyUtil::soundMainBG()
{
    if (PlayerInfo::getInstance()->getBgmOption()){
        if(!CocosDenshion::SimpleAudioEngine::sharedEngine()->isBackgroundMusicPlaying())
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->playBackgroundMusic("audio/bgm_main_01.mp3", true);
        }
    }
}

void MyUtil::resultBG_On()
{
    if (PlayerInfo::getInstance()->getBgmOption()){
        resultBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        resultBG->playEffect("audio/bgm_Results_01.mp3", true);
    }
}

void MyUtil::resultBG_Off()
{
    resultBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
    resultBG->stopAllEffects();
}

void MyUtil::soundButton1()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()){
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/button_01.mp3");
    }
}

void MyUtil::soundButton2()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()){
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/button_02.mp3");
    }
}

int MyUtil::playEffect(const char* pszFilePath){
    if (PlayerInfo::getInstance()->getSoundEffectOption()){
        return CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect(pszFilePath);
    }
    return 0;
}

void MyUtil::runEffect()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/run_effect.mp3");
    }
}

void MyUtil::soundGo()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/button_02.mp3");
    }
}

void MyUtil::soundRing()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/Prepare.mp3");//ring_01.mp3");
    }
}

void MyUtil::soundReady()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/ready_01.mp3");
    }
}

void MyUtil::soundHit()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/hit_01.mp3");
    }
}

void MyUtil::soundCoin()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/coin_01.mp3");
    }
}

void MyUtil::soundExpUp()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/exp_up.mp3");
    }
}

void MyUtil::soundKo()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/ko_01.mp3");
    }
}

void MyUtil::soundBreakDrum()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/Break_1.mp3");
    }
}

void MyUtil::soundRival()
{
    if (PlayerInfo::getInstance()->getSoundEffectOption()) {
        CocosDenshion::SimpleAudioEngine::sharedEngine()->playEffect("audio/New_Challenger.mp3");
    }
}

void MyUtil::soundRivalBG()
{
    if (PlayerInfo::getInstance()->getBgmOption()){
        if (!CocosDenshion::SimpleAudioEngine::sharedEngine()->isBackgroundMusicPlaying())
        {
            CocosDenshion::SimpleAudioEngine::sharedEngine()->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
        }
    }
}

std::string MyUtil::md5(const std::string strMd5){
    md5_state_t state;
    md5_byte_t digest[16];
    char hex_output[16*2 + 1];
    int di;

    md5_init(&state);
    md5_append(&state, (const md5_byte_t *)strMd5.c_str(), strMd5.length());
    md5_finish(&state, digest);
    
    for (di = 0; di < 16; ++di)
        sprintf(hex_output + di * 2, "%02x", digest[di]);
    
    return hex_output;
}


void MyUtil::popupNetworkError(const char* text1, const char* text2, const char* text3)
{
    PopupLayer *popup = new PopupLayer();
    
    popup->setText(text1, text2, text3);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
}

void MyUtil::popupOk(const char *text1)
{
    PopupLayer *popup = new PopupLayer();
    
    popup->setText(text1);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
}

void MyUtil::popupRetry(const char *text1, PopupRetryDelegate *_delegate)
{
    PopupRetry *popup = new PopupRetry();
    
    popup->setDelegate(_delegate);
    popup->setText(text1);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
}

void MyUtil::popupQuest(const char *text1)
{
    PopupLayer *popup = new PopupLayer();
    
    popup->setText_quest(text1);
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
}

void MyUtil::popupOkCancel(CardInfo* card, const char *text1, PopupDelegate *_delegate)
{
    PopupCardSelelctForFusion *popup = new PopupCardSelelctForFusion(card, text1, _delegate);
    
    popup->setAnchorPoint(ccp(0,0));
    popup->setPosition(ccp(0,0));
    popup->setTag(123);
    
    MainScene::getInstance()->addPopup(popup,1000);
}

void MyUtil::_setDisableWithRunningScene(CCNode* node)
{
    CCMenuItem* menuItem = dynamic_cast<CCMenuItem*>(node);
    if(menuItem)
    {
        TouchEnableTable[menuItem] = menuItem->isEnabled();
        //CCLOG("table menu %d", menuItem);

        menuItem->setEnabled(false);
    }
    CCLayer* someLayer = dynamic_cast<CCLayer*>(node);
    if(someLayer)
    {
        TouchEnableTable[someLayer] = someLayer->isTouchEnabled();
        //CCLOG("table layer %d", someLayer);
        someLayer->setTouchEnabled(false);
    }
    if(node->getChildrenCount())
    {
        CCArray* children = node->getChildren();
        for(int i=0; i<children->count(); i++)
        {
            _setDisableWithRunningScene((CCNode*)children->objectAtIndex(i));
        }
    }
    
    //CCLOG("table size11 %d", TouchEnableTable.size());
}

void MyUtil::restoreTouchDisable()
{
    //CCLOG("table size22 %d", TouchEnableTable.size());
    
    for(std::map<CCObject*, bool>::iterator iter = TouchEnableTable.begin(); iter != TouchEnableTable.end(); ++iter)
    {
        //CCLOG("%d", iter->first);
        CCMenuItem* menuItem = dynamic_cast<CCMenuItem*>(iter->first);
        if(menuItem)
        {
            //CCLOG("table size22 %d", menuItem);
            menuItem->setEnabled(iter->second);
        }
        
        CCLayer* someLayer = dynamic_cast<CCLayer*>(iter->first);
        
        if(someLayer)
        {
            //CCLOG("table size22 %d", someLayer);
            someLayer->setTouchEnabled(iter->second);
        }
    }
}

void MyUtil::setDisableWithRunningScene()
{
    CCScene* runningScene = CCDirector::sharedDirector()->getRunningScene();
    _setDisableWithRunningScene(runningScene);
}

void MyUtil::addClickIcon(CCLayer *layer, float x, float y)
{
    CCSpriteFrame *aa1 = CCSpriteFrame::create("ui/tutorial/tutorial_preview_click01.png", CCRectMake(0,0,100/SCREEN_ZOOM_RATE,100/SCREEN_ZOOM_RATE));
    CCSpriteFrame *aa2 = CCSpriteFrame::create("ui/tutorial/tutorial_preview_click02.png", CCRectMake(0,0,100/SCREEN_ZOOM_RATE,100/SCREEN_ZOOM_RATE));
       
    CCArray *aniFrame = new CCArray();
    aniFrame->addObject(aa1);
    aniFrame->addObject(aa2);
    aniFrame->autorelease();
    regAni(aniFrame, layer, ccp(0.5f, 0.0f), accp(x, y), 321, 10000, 0.3f);
}

void MyUtil::removeClickIcon(CCLayer *layer)
{
    layer->removeChildByTag(321, true);
}
