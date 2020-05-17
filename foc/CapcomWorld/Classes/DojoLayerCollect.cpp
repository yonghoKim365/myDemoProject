//
//  DojoLayerCollect.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 26..
//
//


#include "DojoLayerCollect.h"
#include "cocos2d.h"
#include "ARequestSender.h"

DojoLayerCollect* DojoLayerCollect::instance = NULL;
DojoLayerCollect::DojoLayerCollect(CCSize layerSize) : m_pCollectListlayer(NULL), collectionInfo(NULL)
{
    //bool bRet = false;
    do
    {   // super init first
        CC_BREAK_IF(! CCLayer::init());
        
        //todo, init stuff here
        
        //bRet = true;
    } while (0);
    
    instance = this;
    
    
    
    this->setContentSize(layerSize);
    //this->setContentSize(CCSize(this->getContentSize().width, this->getContentSize().height * 5));
    //InitUI();
    
    //CheckLayerSize(this);
}

DojoLayerCollect::~DojoLayerCollect()
{
    this->removeAllChildrenWithCleanup(true);
}

void DojoLayerCollect::setCollectData(ResponseCollectionInfo* _collectionInfo)
{
    collectionInfo = _collectionInfo;
}

void DojoLayerCollect::InitUI()
{
    CCSize size = this->getContentSize();
    
    CCSprite* pSprBG = CCSprite::create("ui/home/ui_BG.png");
    pSprBG->setAnchorPoint(ccp(0,0));
    pSprBG->setPosition( accp(0,0) );
    this->addChild(pSprBG);
    
    /*
    for(int i=0;i<collectionInfo->cardlist->count();i++)
    {
        CardInfo* card = (CardInfo*)collectionInfo->cardlist->objectAtIndex(i);
        CCLog(" card id:%d", card->getId());
    }
    */
    
    /*
     for(int i=0;i<originCollectList->count();i++){
     CardInfo* card = (CardInfo*)originCollectList->objectAtIndex(i);
     CCLog("card->getId():%d", card->getId());
     CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(card->getId());
     int a = ci->series;
     card->series = a;//ci->series;
     }
     */
    
    
    m_pCollectListlayer = new CollectListLayer(CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height), collectionInfo->cardlist);
    this->setAnchorPoint(ccp(0,0));
    this->addChild(m_pCollectListlayer, 40);
    

    /*
    ResponseCollectionInfo *collectionInfo = ARequestSender::getInstance()->requestCollection();
    
    if(collectionInfo)
    {
        if (atoi(collectionInfo->res)!=0){
            popupNetworkError(collectionInfo->res, collectionInfo->msg, "requestCollection");
        }
        
        CCArray* cardlist = new CCArray();
        m_pCollectListlayer = new CollectListLayer(CCRectMake(0,0,this->getContentSize().width-10,this->getContentSize().height),collectionInfo->cardlist);
        this->setAnchorPoint(ccp(0,0));
    //    this->setPosition(ccp(0,0));
        this->addChild(m_pCollectListlayer, 40);
    }
    else
        popupOk("콜렉션 리스트를 불러오는데 실패했습니다. \n잠시후에 다시 시도해주세요");
    */
}