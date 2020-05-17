
#ifndef __PsdParser__
#define __PsdParser__

#include "cocos2d.h"
#include "MGTSprite.h"

//#include "../../rapidjson/include/rapidjson/document.h"
//#include "../../rapidjson/include/rapidjson/rapidjson.h"

#include "external/json/rapidjson.h"
#include "external/json/document.h"
#include "external/json/stringbuffer.h"
#include "external/json/writer.h"
#include "external/json/filestream.h"
#include "external/json/prettywriter.h"

using namespace cocos2d;

// PSD dataset.
struct PsdData
{
    std::string                                             key;
    std::string                                             name;
    std::string                                             file;
    
    cocos2d::Vec2                                        pos;
    cocos2d::Size                                         size;
};


class PsdParser : public cocos2d::SAXDelegator
{
    static rapidjson::Document initParse(const char *pageName);
    
    
private:
    /**
     * @brief       __Dictionary temp address variable.
     */
    cocos2d::__Dictionary                                   *_psdDatas;
    
    /**
     * @brief       PSDData struct variable.
     */
    PsdData                                                 *_data;
    
    
    
    
private:
    /**
     * @brief       PsdData variable initialization.
     */
    void _resetPsdData();

    /**
     * @brief       PsdData(__Dictionary) address variable setting.
     */
    void _setPsdData(cocos2d::__Dictionary *pPsdDict);
    
    /**
     * @brief       Util function for string compare.
     */
    bool _isStringCompare(std::string firstStr, std::string secondStr);
    
    
    // CCSAXDelegator OVERIDING.
    void startElement(void *ctx, const char *name, const char **atts);
    void endElement(void *ctx, const char *name);
    void textHandler(void *ctx, const char *s, int len);
    
    
public:
    
    void setElement(rapidjson::Document document);
    
    
    
public:
    /**
     * @brief       Psd data parsing.
     */
    
    
    static void parseToPsdJSON(std::string strPsdFileName,
                               cocos2d::__Dictionary *pPsdDict,
                               bool bIsCommonPath = false);
    
    static void parseToPsdXML(std::string strPsdFileName,
                              cocos2d::__Dictionary *pPsdDict,
                              bool bIsCommonPath = false);
    
    // PsdParser Creator & Destroyer.
    PsdParser();
    virtual ~PsdParser();
    
    
public:
    /**
     * @brief       Get PsdData to __Dictionary object.
     */
    static PsdData*     getPsdData(cocos2d::__Dictionary *pPsdDatas);
    
    /**
     * @brief       Get Positioning sprite to __Dictionary.
     */
    static MGTSprite*    getPsdSprite(std::string strResName, __Dictionary *pPsdDatas);
    
    /**
     * @brief       Get Positioning common sprite to __Dictionary.
     */
    static MGTSprite*    getPsdCommonSprite(std::string strResName, __Dictionary *pPsdDatas);
    
    static MGTSprite*    getPsdSpriteInFolder(std::string strFolder, std::string strResName, __Dictionary *pPsdDatas);

    /**
     * @brief       Get image position to __Dictionary.
     */
    static Vec2      getPsdPosition(std::string strResName, __Dictionary *pPsdDatas);
    
    /**
     * @brief       Get image size to __Dictionary.
     */
    static Size       getPsdSize(std::string strResName, __Dictionary *pPsdData);
};





#endif /* defined(__PsdParser__) */
