//
//  FileManager.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 23..
//
//

#include "CCHttpRequest.h"
#include "FileManager.h"
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
#include <fstream.h>
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include <fstream>
using std::filebuf;
using std::ifstream;
using std::ofstream;
using std::fstream;
using std::streampos;
#endif

using namespace cocos2d;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

#define CARD_LIST_FILENAME      "UpdateCardImageList.xml"
#define QUEST_DATA              "quest_data.xml"
#define ITEM_DATA               "item_data.xml"
#define IMAGE_DB                "imagedb.xml"

FileManager *FileManager::Fminstance = NULL;

FileManager *FileManager::sharedFileManager()
{
    if (!Fminstance)
        Fminstance = new FileManager();
    
    return Fminstance;
}


FileManager::FileManager()
{
    dictionary  = new CCDictionary;
    dictImageDB = new CCDictionary;
    m_Count     = 0;
    
    m_pSprInfo  = NULL;
}

FileManager::~FileManager()
{
    CC_SAFE_DELETE(dictionary);
    CC_SAFE_DELETE(dictImageDB);
}

void FileManager::Init()
{    
    OpenXml();
    Load_QuestData();
    Load_ItemData();
    Load_ImageDB();
}

CardListInfo* FileManager::GetCardInfo(int cardId)
{
    assert(dictionary != NULL);
    return (CardListInfo*)dictionary->objectForKey(cardId);    
}

Quest_Data* FileManager::GetQuestInfo(int questID)
{
    assert(dictionary != NULL);
    return (Quest_Data*)dictionary->objectForKey(questID);
}

Item_Data* FileManager::GetItemInfo(int itemID)
{
    assert(dictionary != NULL);
    return (Item_Data*)dictionary->objectForKey(itemID);
}

Image_DB* FileManager::GetImgSize(const char* fileName)
{
    assert(dictImageDB != NULL);
    return (Image_DB*)dictImageDB->objectForKey(fileName);
}

bool FileManager::OpenXml()
{
    unsigned long length    = 0;
    std::string pathKey     = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(CARD_LIST_FILENAME);
    unsigned char *data     = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    
    if(data == NULL || length == 0)
        return false;
    
    xmlDocPtr doc           = xmlReadMemory((const char *)data, length, CARD_LIST_FILENAME, NULL, 0);
    xmlNode *root_element   = xmlDocGetRootElement(doc);
    
    ParserXml(root_element, NULL);
    
    xmlFreeDoc(doc);
    
    CC_SAFE_DELETE_ARRAY(data);
    
    return true;
}

bool FileManager::Load_QuestData()
{
    unsigned long length    = 0;
    std::string pathKey     = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(QUEST_DATA);
    unsigned char *data     = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    
    if(data == NULL || length == 0)
        return false;
    
    xmlDocPtr doc           = xmlReadMemory((const char *)data, length, QUEST_DATA, NULL, 0);
    xmlNode *root_element   = xmlDocGetRootElement(doc);
    
    ParserQuestDataXml(root_element, NULL);
    
    xmlFreeDoc(doc);
    
    CC_SAFE_DELETE_ARRAY(data);
    
    return true;
}

bool FileManager::Load_ItemData()
{
    unsigned long length    = 0;
    std::string pathKey     = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(ITEM_DATA);
    unsigned char *data     = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    
    if(data == NULL || length == 0)
        return false;
    
    xmlDocPtr doc           = xmlReadMemory((const char *)data, length, ITEM_DATA, NULL, 0);
    xmlNode *root_element   = xmlDocGetRootElement(doc);
    
    ParserItemDataXml(root_element, NULL);
    
    xmlFreeDoc(doc);
    
    CC_SAFE_DELETE_ARRAY(data);
    
    return true;
}

bool FileManager::Load_ImageDB()
{
    unsigned long length    = 0;
    std::string pathKey     = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(IMAGE_DB);
    unsigned char *data     = CCFileUtils::sharedFileUtils()->getFileData(pathKey.c_str(), "rb", &length);
    
    if(data == NULL || length == 0)
        return false;
    
    xmlDocPtr doc           = xmlReadMemory((const char *)data, length, IMAGE_DB, NULL, 0);
    xmlNode *root_element   = xmlDocGetRootElement(doc);
    
    ParserImageDBXml(root_element, NULL);
    
    xmlFreeDoc(doc);
    
    CC_SAFE_DELETE_ARRAY(data);

    return true;    
}

void FileManager::ParserXml(xmlNode *node, CardListInfo* parentInfo)
{
    xmlNode *currentNode    = NULL;
    
    CardListInfo *info      = parentInfo;

    for(currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
             info = new CardListInfo;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "CardId") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
            {
                info->SetCardID(atoi((const char*)child->content));
            
                dictionary->setObject(info, info->GetCardID());
                
                info->SetCount(m_Count);
                
                ++m_Count;

            }
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "Character") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->SetCharacterFileName((const char*)child->content);                                
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "Thumbnail") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->SetThumbnailFileName((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "BattleLarge") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->SetLargeBattleImg((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "BattleSmall") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->SetSmallBattleImg((const char*)child->content);
        }
        
        ParserXml(currentNode->children, info);
    }
}

void FileManager::ParserQuestDataXml(xmlNode *node, Quest_Data* parentInfo)
{
    xmlNode* currentNode    = NULL;
    
    Quest_Data* info      = parentInfo;

    for(currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            info = new Quest_Data;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "q_code") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
            {
                info->SetQuestID(atoi((const char*)child->content));
                
                dictionary->setObject(info, info->GetQuestID());
            }
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "chapter_lockimage") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->chapterLockImg = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "chapter_unlockimage") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->chapterUnLockImg = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "stage_smallimage") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->stageBG_s = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "stage_biglimage") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->stageBG_L = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "level") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->level = (atoi((const char*)child->content));
            
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "image_3") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->questEnemy3 = (const char*)child->content;
        }
        /*
        
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "image_1") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->questEnemy1 = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "image_2") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->questEnemy2 = (const char*)child->content;
        }
        
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "image_4") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->questEnemy4 = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "image_5") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->questEnemy5 = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "attack") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->nomalComboCnt = atoi((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "ultrarepeat") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->ultraRepeat = atoi((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "combotime") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->ultraComboTimeOut = atof((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "iconscaleup") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->comboScaleUPSpeed = atof((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "icondelay") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->comboCreateDelay = atof((const char*)child->content);
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "iconoverlap") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->comboNumScreen = atoi((const char*)child->content);
        }
        */



        ParserQuestDataXml(currentNode->children, info);
    }
}

void FileManager::ParserItemDataXml(xmlNode *node, Item_Data* parentInfo)
{
    xmlNode* currentNode    = NULL;
    
    Item_Data* info      = parentInfo;
    
    for(currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            info = new Item_Data;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_code") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
            {
                info->itemID = (atoi((const char*)child->content));
                
                dictionary->setObject(info, info->itemID);
            }
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_cvalue") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->itemPrice = (const char*)child->content;
            
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_info") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->itemAmount = (atoi((const char*)child->content));
            
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_image") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->itemImgPath = (const char*)child->content;
            
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_namekr") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->itemName = (const char*)child->content;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "i_description") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->itemDesc = (const char*)child->content;
        }
                    
        ParserItemDataXml(currentNode->children, info);
    }
}

void FileManager::ParserImageDBXml(xmlNode *node, Image_DB* parentInfo)
{
    xmlNode* currentNode    = NULL;
    
    Image_DB* info      = parentInfo;
    
    for(currentNode = node; currentNode; currentNode = currentNode->next)
    {
        if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "row") == 0)
        {
            info = new Image_DB;
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "file") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
            {
                std::string fileName = (const char*)child->content;
                
                dictImageDB->setObject(info, fileName.c_str());
            }
        }
        else if(currentNode->type == XML_ELEMENT_NODE && strcmp((const char*)currentNode->name, "size") == 0)
        {
            assert(info != NULL);
            
            xmlNode *child = currentNode->children;
            
            if(child)
                info->fileSize = atoi((const char*)child->content);
        }
        
        ParserImageDBXml(currentNode->children, info);
    }
}

bool FileManager::IsFileExist(const char* FilePath)
{
    bool flag = false;
    
    std::string DocumentPath    = CCFileUtils::sharedFileUtils()->getDocumentPath();
    DocumentPath                += FilePath;

    // -- 사이즈 체크
    FILE* h_file = fopen(DocumentPath.c_str(), "r");
    
    long file_size = 0;
    
    if(h_file)
    {
        fseek(h_file, 0, SEEK_END);
        file_size = ftell(h_file);
        fclose(h_file);
    }
    
    fstream fin;
    fin.open(DocumentPath.c_str(), std::ios::in);
    
    if(fin.is_open()==true)
    {
        Image_DB* db = this->GetImgSize(FilePath);
        
        if(db)
        {
            if(file_size != db->fileSize)
                flag = false;
            else
                flag = true;
        }
    }
    
    
    fin.close();
    
    return flag;
}

void FileManager::requestCardImg(CCLayer *player, int CardID, ImgSizeType SizeType, const CCPoint& Pos, float scale, int z,  int tag, int directionType)
{
    assert(dictionary != NULL);
    CardListInfo* pInfo = (CardListInfo*)dictionary->objectForKey(CardID);
    
    if(NULL == pInfo)
        CCLog("CardListInfo error  cardID : %d", CardID);

    CCSprite* pSpr = NULL;
    
    m_pSprInfo = new SprInfo;
    m_pSprInfo->m_pLayer        = player;
    m_pSprInfo->m_scale         = scale;
    m_pSprInfo->m_tag           = tag;
    m_pSprInfo->m_ccPos         = Pos;
    m_pSprInfo->m_z             = z;
    m_pSprInfo->m_directionType = directionType;
    
    //m_pSprInfo->autorelease();
    
    std::string CharacterPath   = FOC_IMAGE_SERV_URL;
    CharacterPath.append("images/card/card_detail/card_l/");
    
    std::string ThumbnailPath   = FOC_IMAGE_SERV_URL;
    ThumbnailPath.append("images/card/card_detail/card_s/");
    
    std::string DocumentPath    = CCFileUtils::sharedFileUtils()->getDocumentPath();
    
    std::string FileName;
    std::string FullPath;
    std::string Localpath;
    int         Size;
    
    if(S_SIZE == SizeType)
    {
        if (pInfo != NULL){
            FileName = pInfo->GetThumbnailFileName();
            FullPath = CharacterPath + pInfo->GetThumbnailFileName();
            DocumentPath += pInfo->GetThumbnailFileName();
        }
        Localpath = "ui/card_detail/cardimg_s/";
        Size = S_SIZE;
    }
    
    if(L_SIZE == SizeType)
    {
        if (pInfo != NULL){
            FileName = pInfo->GetCharacterFileName();
            FullPath = CharacterPath + pInfo->GetCharacterFileName();
            DocumentPath += pInfo->GetCharacterFileName();
        }
        Localpath = "ui/card_detail/cardimg_l/";
        Size = L_SIZE;
    }
    
    // -- 로컬 파일 읽기
    /*
    std::string temppath = Localpath+FileName;
    pSpr = CCSprite::create(temppath.c_str());
    
    if(pSpr)
    {
        pSpr->setAnchorPoint(ccp(0, 0));
        pSpr->setPosition(Pos);
        pSpr->setScale(scale);
        player->addChild(pSpr, z);
    }
     */

    if(IsFileExist(FileName.c_str()))
    {
        bool found = false;
        for (std::list<UrlData*>::iterator urlIter = m_urls.begin();urlIter != m_urls.end();++urlIter)
        {
            if (FullPath == (*urlIter)->url)
            {
                found = true;
            }
        }

        if(false == found)
        {
            pSpr = CCSprite::create(DocumentPath.c_str());
            
            if(pSpr)
            {
                //CC_SAFE_DELETE(m_pSprInfo);
                
                pSpr->setAnchorPoint(ccp(0, 0));
                pSpr->setPosition(Pos);
                pSpr->setScale(scale);
                player->addChild(pSpr, z);
                
                if(1 == directionType)
                    newCardDirection(player);
            }
            else
            {
                DownLoadImg((ImgSizeType)Size, FileName.c_str());
            }
        }
    }
    else
    {
        DownLoadImg((ImgSizeType)Size, FileName.c_str());
    }
}

void FileManager::DownLoadImg(ImgSizeType SizeType, const char* ImgName)
{
    std::string CharacterPath   = FOC_IMAGE_SERV_URL;
    CharacterPath.append("images/card/card_detail/card_l/");
    
    std::string ThumbnailPath   = FOC_IMAGE_SERV_URL;
    ThumbnailPath.append("images/card/card_detail/card_s/");

    std::string FullPath;
    
    if(S_SIZE == SizeType)
        FullPath = ThumbnailPath += ImgName;
    if(L_SIZE == SizeType)
        FullPath = CharacterPath += ImgName;
        
    bool found = false;
    for (std::list<UrlData*>::iterator urlIter = m_urls.begin();urlIter != m_urls.end();++urlIter)
    {
        if (FullPath == (*urlIter)->url)
            found = true;
    }
    
    if(false == found)
    {
        CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
        std::vector<std::string> downloads;
        downloads.push_back(FullPath);
        requestor->addDownloadTask(downloads, m_pSprInfo, callfuncND_selector(FileManager::onHttpRequestCompleted));
    }
    
    UrlData *urlData = new UrlData;
    urlData->url = FullPath;
    urlData->cell = (void*)m_pSprInfo;
    m_urls.push_back(urlData);
}

void FileManager::newCardDirection(CCLayer* _layer)
{
    CCSprite* spr = CCSprite::create("ui/card_tab/card_eff_glow.png");
    spr->setAnchorPoint(ccp(0.0f, 0.0f));
    spr->setPosition(ccp(8.0f, -299.0f)); // -- 498.0 이미지 height
    _layer->addChild(spr, 1000);
    
    CCFiniteTimeAction* actionMove = CCMoveTo::actionWithDuration(0.8f, ccp(8.0f, CCDirector::sharedDirector()->getWinSize().height));
    spr->runAction(actionMove);
    
    CCSprite* spr1 = CCSprite::create("ui/card_tab/card_eff_01.png");
    spr1->setAnchorPoint(ccp(0.0f, 0.0f));
    spr1->setPosition(ccp(-50.0f, 245.0f));
    spr1->setOpacity(0);
    _layer->addChild(spr1, 1000);
    
    CCFadeIn* in1 = CCFadeIn::actionWithDuration(0.25f);
    CCFadeOut* out1 = CCFadeOut::actionWithDuration(0.2f);
    spr1->runAction(CCSequence::actions(in1, out1, in1, out1, in1, out1, NULL));
    
    CCSprite* spr2 = CCSprite::create("ui/card_tab/card_eff_01.png");
    spr2->setAnchorPoint(ccp(0.0f, 0.0f));
    spr2->setPosition(ccp(190.0f, 325.0f));
    _layer->addChild(spr2, 1000);
    
    CCDelayTime *delay2 = CCDelayTime::actionWithDuration(0.2f);
    CCFadeIn* in2 = CCFadeIn::actionWithDuration(0.3f);
    CCFadeOut* out2 = CCFadeOut::actionWithDuration(0.3f);
    spr2->runAction(CCSequence::actions(delay2, in2, out2, in2, out2, in2, out2, NULL));
    
    CCSprite* spr3 = CCSprite::create("ui/card_tab/card_eff_01.png");
    spr3->setAnchorPoint(ccp(0.0f, 0.0f));
    spr3->setPosition(ccp(170.0f, 40.0f));
    _layer->addChild(spr3, 1000);
    
    CCDelayTime *delay3 = CCDelayTime::actionWithDuration(0.4f);
    CCFadeIn* in3 = CCFadeIn::actionWithDuration(0.3f);
    CCFadeOut* out3 = CCFadeOut::actionWithDuration(0.3f);
    spr3->runAction(CCSequence::actions(delay3, in3, out3, in3, out3, in3, out3, NULL));

}

void FileManager::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        std::vector<std::string>::iterator iter;
        for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter)
        {
            std::string url = *iter;
            
            for (std::list<UrlData*>::iterator urlIter = FileManager::sharedFileManager()->m_urls.begin();urlIter != FileManager::sharedFileManager()->m_urls.end();)
            {
                if (url == (*urlIter)->url)
                {
                    SprInfo* pInfo = (SprInfo*)response->request->pTarget;
                    
                    if(!pInfo)
                        CCLog("pInfo is NULL!!");
                    
                    std::string path = (*iter);
                    std::string CharacterPath   = FOC_IMAGE_SERV_URL;
                    CharacterPath.append("images/card/card_detail/card_l/");
                    
                    std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + &(path.c_str()[CharacterPath.length()]);
                    CCSprite* pSpr = CCSprite::create(DocumentPath.c_str());
                    
                    if(pSpr && (*urlIter)->cell)
                    {
                        pSpr->setAnchorPoint(ccp(0, 0));
                        pSpr->setPosition(pInfo->m_ccPos);
                        pSpr->setScale(pInfo->m_scale);
                        pSpr->setTag(pInfo->m_tag);

                        void* cell = (*urlIter)->cell;
                        SprInfo* layerInfo = (SprInfo*)cell;
                        layerInfo->m_pLayer->addChild(pSpr, pInfo->m_z);
                       
                        if(1 == pInfo->m_directionType)
                        {
                            newCardDirection(layerInfo->m_pLayer);
                        }
                    }
                    
                    FileManager::sharedFileManager()->m_urls.erase(urlIter);
                    //delete *urlIter;
                    //(*urlIter)->autorelease();

                    urlIter = FileManager::sharedFileManager()->m_urls.begin();
                    
                    //CC_SAFE_DELETE(pInfo);
                    //delete pInfo;
                }
                else
                    ++urlIter;
            }
        }
    }
}

void FileManager::downloadLeaderImg(int cardID)
{
    std::string basePathL  = FOC_IMAGE_SERV_URL;
    basePathL.append("images/cha/cha_l/");
    std::vector<std::string> downloads;
    
    CardListInfo* card = this->GetCardInfo(cardID);
    
    if(card)
    {
        if(!this->IsFileExist(card->GetLargeBattleImg()))
        {
            std::string downPath = basePathL + card->GetLargeBattleImg();
            downloads.push_back(downPath);
            
            CCLOG("리더 이미지 요청 / 아이디 : %d", card->CardID);
            
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            requestor->addDownloadTask(downloads, this, callfuncND_selector(FileManager::downloadLeaderCallback));
        }
    }
}

void FileManager::downloadLeaderCallback(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        CCLOG("리더이미지 다운로드 완료");
    }
}

unsigned int FileManager::GetNumOfCard()
{
    return dictionary->count();
}

int FileManager::GetCardCount(int CardID)
{
    CardListInfo* pInfo = (CardListInfo*)dictionary->objectForKey(CardID);

    if(!pInfo)
        CCLog("CardListInfo error  cardID : %d", CardID);
    
    return pInfo->GetCount();
}