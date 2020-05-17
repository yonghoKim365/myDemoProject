//
//  ACardMaker.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 27..
//
//

#include "ACardMaker.h"

#include "FileManager.h"

void ACardMaker::MakeCardThumb(CCLayer *layer, CardInfo *card, CCPoint pos, int thumb_h, int z, int _tag)
{
    
    int xx = pos.x;
    int yy = pos.y;
    
    
    std::string path = "ui/card_detail/card_frame_S_4s_0";
    /*
    if (card->GetFusionLevel() <=0){
        card->SetFusionLevel(1);
    }
    else if (card->GetFusionLevel() > 10){
        card->SetFusionLevel(9);
    }
    char path2[1];
    sprintf(path2, "%d", card->GetFusionLevel());
    path.append(path2).append(".png");
    cocos2d::CCSprite* pSpr1 = CCSprite::create(path.c_str());
    pSpr1->setTag(_tag);

    
    CCSize aa = pSpr1->getTexture()->getContentSizeInPixels();
    //float cardScale = (float)170 / aa.height;
    float cardScale = (float)thumb_h / aa.height;
    pSpr1->setScale(cardScale);
    regSprite(layer, ccp(0,0), accp(xx,yy), pSpr1, z+10);
    */
    //////
    char path2[1];
    
    //CCLog(" card->getRare():%d", card->getRare());
    
    sprintf(path2, "%d", card->getRare()+1);
    path.append(path2).append(".png");
    cocos2d::CCSprite* pSpr1 = CCSprite::create(path.c_str());
    pSpr1->setTag(_tag);
    
    CCSize aa = pSpr1->getTexture()->getContentSizeInPixels();
    float cardScale = (float)thumb_h / aa.height;
    pSpr1->setScale(cardScale);
    regSprite(layer, ccp(0,0), accp(xx,yy), pSpr1, z+10);
    ///////
    
    FileManager::sharedFileManager()->requestCardImg(layer, card->getId(), S_SIZE, accp(xx+5*cardScale, yy+30*cardScale), cardScale, z, 0);
    //requestCardImg(layer, card->getId(), S_SIZE, accp(xx+5*cardScale, yy+30*cardScale), cardScale, z, 0);
    
    //cocos2d::CCSprite* pSpr2 = CCSprite::create("ui/card_detail/cardimg_s/sf_blanka_03_01.png");
    //pSpr2->setScale(cardScale);
    //regSprite(layer, ccp(0,0), accp(xx+5*cardScale,yy+30*cardScale), pSpr2, z);
    
    cocos2d::CCSprite* pSpr3;
    if (card->getAttribute() == ATRB_GUARD){
        pSpr3 = CCSprite::create("ui/card_detail/card_attribute_guard_s.png");
    }
    else if (card->getAttribute() == ATRB_SMASH){
        pSpr3 = CCSprite::create("ui/card_detail/card_attribute_smash_s.png");
    }
    else if (card->getAttribute() == ATRB_THROW){
        pSpr3 = CCSprite::create("ui/card_detail/card_attribute_throw_s.png");
    }
    pSpr3->setScale(cardScale);
    regSprite(layer, ccp(0,0), accp(xx+7*cardScale,yy+197*cardScale), pSpr3, z+10);
    
    
//    CCLog("cardScale:%f", cardScale);
    
    int rareVal = card->getRare();
    
    if (card->GetFusionLevel() > 1)
    {
        for(int i=2;i<=card->GetFusionLevel();i++){
            if (rareVal == 4){
                std::string gagePath = "ui/card_detail/card_fusion_S_05_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                pSpr->setScale(cardScale);
                regSprite(layer, ccp(0,0), accp(xx+49*cardScale+(20*(i-2)*cardScale), yy+21*cardScale), pSpr, z+10);
            }
            else if (card->getRare() == 3){
                std::string gagePath = "ui/card_detail/card_fusion_S_04_0";
                char temp[1];
                sprintf(temp, "%d", i-1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                pSpr->setScale(cardScale);
                regSprite(layer, ccp(0,0), accp(xx+49*cardScale+(20*(i-2)*cardScale), yy+21*cardScale), pSpr, z+10);
            }
            else{
                std::string gagePath = "ui/card_detail/card_fusion_S_0";
                char temp[1];
                sprintf(temp, "%d", card->getRare()+1);
                gagePath.append(temp).append(".png");
                
                CCSprite *pSpr = CCSprite::create(gagePath.c_str());
                pSpr->setScale(cardScale);
                regSprite(layer, ccp(0,0), accp(xx+49*cardScale+(20*(i-2)*cardScale), yy+21*cardScale), pSpr, z+10);
            }
        }
    }
    
    int nLv = card->getLevel();
    if (nLv>99)nLv=0;
    
    if (nLv > 0){
        refreshLevel(layer, nLv,xx+155*cardScale, yy+7*cardScale, cardScale);
    }
    
}


/////////////////////

void ACardMaker::refreshLevel(CCLayer *_layer, int _level, int _x, int _y, float scale)
{
    //for(int i=0;i<3;i++){
    //   level[i]= NULL;
    //}
    
    int x = _x;//84;
    //    float scale = 0.9f;
    char buffer[10];
    sprintf(buffer, "%d", _level);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        
        //level[scan] = createNumber(number, accp(x, 845), scale);
        //this->addChild(level[scan], 2000);
        //CCLog("refreshLevel, y=%d",_y);
        
        CCSprite *pSprNum = createNumber(number, accp(x, _y), scale);
        _layer->addChild(pSprNum, 2000);
        CCSize size = pSprNum->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        pSprNum->setPosition(accp(x, _y));
    }
}

CCSprite *ACardMaker::createNumber(int number, cocos2d::CCPoint pos, float scale) {
    assert(number >= 0 && number < 10);
    char buffer[32];
    sprintf(buffer, "ui/home/number_%d.png", number);
    CCSprite *sprite = CCSprite::create(buffer);
    sprite->setScale(scale);
    sprite->setAnchorPoint(ccp(0, 0));
    sprite->setPosition(pos);
    return sprite;
}

