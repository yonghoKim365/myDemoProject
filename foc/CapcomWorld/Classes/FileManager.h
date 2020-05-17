//
//  FileManager.h
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 10. 23..
//
//

#ifndef __CapcomWorld__FileManager__
#define __CapcomWorld__FileManager__

#include <iostream>
#include "cocos2d.h"
#include <list>
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>
#include "GameConst.h"

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

enum ImgSizeType
{
    L_SIZE = 0,
    S_SIZE,
};

struct CardListInfo : public cocos2d::CCObject
{
    int         CardID;
    int         Count;
    std::string CharacterFileName;
    std::string ThumbnailFileName;
    std::string largeBattleImg;
    std::string smallBattleImg;
    
    void SetCardID(int id)
    {
        CardID = id;
    }
    
    int GetCardID() const
    {
        return CardID;
    }
    
    void SetCount(int count)
    {
        Count = count;
    }
    
    int GetCount() const
    {
        return Count;
    }
    
    void SetCharacterFileName(const char *path)
    {
        CharacterFileName = path;
    }
    
    const char *GetCharacterFileName() const
    {
        return CharacterFileName.c_str();
    }

    void SetThumbnailFileName(const char *path)
    {
        ThumbnailFileName = path;
    }
    
    const char *GetThumbnailFileName() const
    {
        return ThumbnailFileName.c_str();
    }
    
    void SetLargeBattleImg(const char *name)
    {
        largeBattleImg = name;
    }
    
    const char* GetLargeBattleImg() const
    {
        return largeBattleImg.c_str();
    }
    
    void SetSmallBattleImg(const char *name)
    {
        smallBattleImg = name;
    }

    const char* GetSmallBattleImg() const
    {
        return smallBattleImg.c_str();
    }
};

struct Quest_Data : public cocos2d::CCObject
{
public:
    int questID;
    std::string chapterLockImg;
    std::string chapterUnLockImg;
    std::string stageBG_s;
    std::string stageBG_L;
    int         level; // -- 1:보스
    std::string questEnemy3;
    /*
    int         level; // -- 1:보스
    std::string questEnemy1;
    std::string questEnemy2;
    std::string questEnemy3;
    std::string questEnemy4;
    std::string questEnemy5;
    int     nomalComboCnt;
    int     ultraRepeat;
    float   ultraComboTimeOut;
    float   comboScaleUPSpeed;
    float   comboCreateDelay;
    int     comboNumScreen;
    */
    void SetQuestID(int id)
    {
        questID = id;
    }
    
    int GetQuestID() const
    {
        return questID;
    }
};

struct Item_Data : public cocos2d::CCObject
{
public:
    int         itemID;
    std::string itemPrice;
    int         itemAmount;
    std::string itemImgPath;
    std::string itemName;
    std::string itemDesc;
};

struct Image_DB : public cocos2d::CCObject
{
public:
    std::string fileName;
    int         fileSize;
};

typedef struct : public cocos2d::CCObject
{
    std::string url;
    void *cell;
} UrlData;


struct SprInfo : public cocos2d::CCObject
{
    CCLayer*                    m_pLayer;
    CCPoint                     m_ccPos;
    int                         m_z;
    int                         m_tag;
    float                       m_scale;
    int                         m_directionType;
      
    SprInfo()
    {
        m_pLayer    = NULL;
        m_scale     = 1.0f;
        m_directionType = 0;
    }
};


class FileManager : public cocos2d::CCObject
{
public:
    static FileManager*         sharedFileManager();

    void                        Init();
    
    bool                        OpenXml();
    bool                        Load_QuestData();
    bool                        Load_ItemData();
    bool                        Load_ImageDB();
    
    void                        ParserXml(xmlNode *node, CardListInfo* parentInfo);
    void                        ParserQuestDataXml(xmlNode *node, Quest_Data* parentInfo);
    void                        ParserItemDataXml(xmlNode *node, Item_Data* parentInfo);
    void                        ParserImageDBXml(xmlNode *node, Image_DB* parentInfo);
    
    CardListInfo*               GetCardInfo(int cardId);
    Quest_Data*                 GetQuestInfo(int questID);
    Item_Data*                  GetItemInfo(int itemID);
    Image_DB*                   GetImgSize(const char* fileName);

    bool                        IsFileExist(const char* FilePath);
    void                        requestCardImg(CCLayer *player, int CardID, ImgSizeType SizeType, const CCPoint& Pos, float scale, int z, int tag, int directionType = 0);
    void                        onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data);
    void                        DownLoadImg(ImgSizeType SizeType, const char* ImgName);
                                FileManager();
                                ~FileManager();
    unsigned int                GetNumOfCard();
    int                         GetCardCount(int CardID);
    
    void                        downloadLeaderImg(int cardID);
    void                        downloadLeaderCallback(cocos2d::CCObject *pSender, void *data);
    
    void                        newCardDirection(CCLayer* _layer);
    
    std::list<UrlData*>         m_urls;
    
    int FileSize(const char* sFileName)
    {
        std::ifstream f;
        f.open(sFileName, std::ios_base::binary | std::ios_base::in);
       
        if( f.eof())
            return 0;
        if(!f.is_open())
            return 0; 
        f.seekg(0, std::ios_base::beg);
        std::ifstream::pos_type begin_pos = f.tellg();
        f.seekg(0, std::ios_base::end);
        return static_cast<int>(f.tellg() - begin_pos);
    }
    
    std::string getUserProfileFilename(std::string path){
        std::string str = path;
        size_t found;
        //cout << "Splitting: " << str << endl;
        found=str.find_last_of("/\\");
        //cout << " folder: " << str.substr(0,found) << endl;
        //std::cout << " file: " << str.substr(found+1) << std::endl;
        return str.substr(found+1);
    }
    
    bool IsProfileFileExist(const char* FilePath)
    {
        bool flag = false;
        
        std::string DocumentPath    = CCFileUtils::sharedFileUtils()->getDocumentPath();
        DocumentPath                += FilePath;
        
        fstream fin;
        fin.open(DocumentPath.c_str(), std::ios::in);
        
        if(fin.is_open()==true)
        {
            flag = true;
        }
        
        
        fin.close();
        
        return flag;   
    }
    
private:
    static FileManager          *Fminstance;
    cocos2d::CCDictionary       *dictionary;
    cocos2d::CCDictionary       *dictImageDB;
    
    
    std::string                 DownFilepath;
    
    SprInfo*                    m_pSprInfo;
    
    int                         m_Count;
    
    

};


#endif /* defined(__CapcomWorld__FileManager__) */
