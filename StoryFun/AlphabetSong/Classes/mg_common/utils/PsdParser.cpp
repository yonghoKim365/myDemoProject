
#include "PsdParser.h"
#include "MGTUtils.h"
#include "MGTResourceUtils.h"

using namespace cocos2d;


#define ENCODE_TYPE_UTF8                                "UTF-8"



rapidjson::Document PsdParser::initParse(const char *pageName)
{
    
    rapidjson::Document document;
    
    std::string filename = FileUtils::getInstance()->fullPathForFilename(pageName);
    CCLOG("PSDParser filename %s", filename.c_str());

    auto content = FileUtils::getInstance()->getStringFromFile(filename);
    
    if (document.Parse<0>(content.c_str()).HasParseError())
    {
        CCLOG("document ParseError");
    }
    else
    {
        CCLOG("document ParseSuccess");
    }
    
    return document;
}


void PsdParser::parseToPsdJSON(std::string strPsdFileName,
                                cocos2d::__Dictionary *pPsdDict,
                                bool bIsCommonPath /* = false */)
{
    PsdParser *psdParser = new PsdParser();
    psdParser->_setPsdData(pPsdDict);
    
    std::string rootPath;
    if (!bIsCommonPath)
    {
        
        rootPath = MGTResourceUtils::getInstance()->getResourcePath(ResourceType::JSON);
    }
    else
    {
        rootPath = MGTResourceUtils::getInstance()->getCommonResourcePath();
    }
    std::string tempFileName = rootPath.append("json/");
    tempFileName.append(strPsdFileName);

    log("PsdParser filename:%s", tempFileName.c_str());
    
    psdParser->setElement(PsdParser::initParse(tempFileName.c_str()));
    
    delete psdParser;
}

void PsdParser::parseToPsdXML(std::string strPsdFileName,
                                cocos2d::__Dictionary *pPsdDict,
                                bool bIsCommonPath /* = false */)
{
    PsdParser *psdParser = new PsdParser();
    psdParser->_setPsdData(pPsdDict);
    
    std::string rootPath;
    if (!bIsCommonPath)
    {
            rootPath = MGTResourceUtils::getInstance()->getResourcePath(ResourceType::XML);
    }
    else
    {
        rootPath = MGTResourceUtils::getInstance()->getCommonResourcePath();
    }
    std::string tempFileName = rootPath.append("xml/");
    tempFileName.append(strPsdFileName);

    
    SAXParser xmlParser;
    xmlParser.init(ENCODE_TYPE_UTF8);
    xmlParser.setDelegator(psdParser);
    xmlParser.parse(tempFileName.c_str());
    
    delete psdParser;
}

PsdParser::PsdParser()
{
    _psdDatas       = NULL;
    _data           = new PsdData();
}

PsdParser::~PsdParser()
{
    _psdDatas       = NULL;
    delete          _data;
}


void PsdParser::setElement(rapidjson::Document document)
{
    if (document.HasMember("layer"))
    {
        const rapidjson::Value& layer = document["layer"];
            
        for (int layerIdx = 0; layerIdx < layer.Size(); layerIdx++)
        {
            const rapidjson::Value& layerObj = layer[layerIdx];
            
            __Dictionary *tempDict = __Dictionary::create();
        
            if (layerObj.HasMember("name"))
            {
                _data->key = layerObj["name"].GetString();
                _data->name = layerObj["name"].GetString();

                tempDict->setObject(__String::create(_data->name), "name");
                
                std::string tempStr         = __String::create(_data->name)->getCString();
                std::string delimiterDot    = ".";
                std::string delimiterAt     = "@";
                tempStr = tempStr.substr(0, tempStr.find(delimiterDot));
                tempStr = tempStr.substr(0, tempStr.find(delimiterAt));
                
                tempDict->setObject(__String::create(tempStr), "file");
            }
            
            if(layerObj.HasMember("width"))
            {
                _data->size.width = layerObj["width"].GetDouble();
                tempDict->setObject(__Float::create(_data->size.width), "width");

            }
            
            if(layerObj.HasMember("height"))
            {
                _data->size.height = layerObj["height"].GetDouble();
                tempDict->setObject(__Float::create(_data->size.height), "height");
            }
            
            if(layerObj.HasMember("posX"))
            {
                _data->pos.x = layerObj["posX"].GetDouble();
                tempDict->setObject(__Float::create(_data->pos.x), "posX");
            }
            
            if(layerObj.HasMember("posY"))
            {
                _data->pos.y = layerObj["posY"].GetDouble();
                tempDict->setObject(__Float::create(_data->pos.y), "posY");
            }
            
            _psdDatas->setObject(tempDict, _data->key);
            
            this->_resetPsdData();
        }
    }
}

void PsdParser::startElement(void *ctx, const char *name, const char **atts)
{
    CC_UNUSED_PARAM(ctx);
    
    if(true == _isStringCompare(name, "layer"))
    {
        while(NULL != *atts)
        {
            if(true == _isStringCompare(*atts, "name"))
            {
                _data->key = * ++atts;
                _data->name = * atts;
            }
            else if(true == _isStringCompare(*atts, "openGLpositionCenterX"))
            {
                _data->pos.x = __String::create(* ++atts)->floatValue();
            }
            else if(true == _isStringCompare(*atts, "openGLpositionCenterY"))
            {
                _data->pos.y = __String::create(* ++atts)->floatValue();
            }
            else if(true == _isStringCompare(*atts, "layerwidth"))
            {
                _data->size.width = __String::create(* ++atts)->floatValue();
            }
            else if(true == _isStringCompare(*atts, "layerheight"))
            {
                _data->size.height = __String::create(* ++atts)->floatValue();
            }
            
            atts++;
        }
    }
}

void PsdParser::endElement(void *ctx, const char *name)
{
    
}

void PsdParser::textHandler(void *ctx, const char *s, int len)
{
    __Dictionary *tempDict = __Dictionary::create();
    
    std::string tempStr         = s;
    std::string delimiterDot    = ".";
    std::string delimiterAt     = "@";
    tempStr = tempStr.substr(0, tempStr.find(delimiterDot));
    tempStr = tempStr.substr(0, tempStr.find(delimiterAt));
    tempDict->setObject(__String::create(tempStr), "file");
    
    tempDict->setObject(__String::create(_data->name), "name");
    tempDict->setObject(__Float::create(_data->pos.x), "posX");
    tempDict->setObject(__Float::create(_data->pos.y), "posY");
    tempDict->setObject(__Float::create(_data->size.width), "width");
    tempDict->setObject(__Float::create(_data->size.height), "height");
    _psdDatas->setObject(tempDict, _data->key);

    this->_resetPsdData();
}

void PsdParser::_setPsdData(cocos2d::__Dictionary *pPsdDict)
{
    _psdDatas = NULL;
    _psdDatas = pPsdDict;
}




void PsdParser::_resetPsdData()
{
    _data->key      = "";
    _data->name     = "";
    _data->pos      = Vec2::ZERO;
    _data->size     = Size::ZERO;
}

bool PsdParser::_isStringCompare(std::string firstStr, std::string secondStr)
{
    return firstStr.compare(secondStr) == 0 ? true : false;
}


PsdData* PsdParser::getPsdData(cocos2d::__Dictionary *pPsdDatas)
{
    PsdData *tempData = new PsdData();    
    tempData->name          = ((__String *)pPsdDatas->objectForKey("name"))->getCString();
    tempData->file          = ((__String *)pPsdDatas->objectForKey("file"))->getCString();
    tempData->pos.x         = ((__Float *)pPsdDatas->objectForKey("posX"))->getValue();
    tempData->pos.y         = ((__Float *)pPsdDatas->objectForKey("posY"))->getValue();
    tempData->size.width    = ((__Float *)pPsdDatas->objectForKey("width"))->getValue();
    tempData->size.height   = ((__Float *)pPsdDatas->objectForKey("height"))->getValue();
    
    return tempData;
}

MGTSprite* PsdParser::getPsdSprite(std::string strResName, cocos2d::__Dictionary *pPsdDatas)
{
    PsdData *data = PsdParser::getPsdData((__Dictionary *)pPsdDatas->objectForKey(strResName));
    
//    std::string tempFileName = "img/";
//    tempFileName.append(data->name);
    
    std::string pngName = data->file;
    std::string jpgName = data->file;
    pngName.append(".png");
    jpgName.append(".jpg");
    
    
    MGTSprite *tempSprite = MGTSprite::createWithFullPath(pngName.c_str());
    
    if(NULL == tempSprite)
    {
        tempSprite = MGTSprite::createWithFullPath(jpgName.c_str());
    }
    if(NULL == tempSprite)
    {
        CCLOG("NO IMAGE FILE!");
    }
    
    tempSprite->setPosition(data->pos);

    delete data;
    
    return tempSprite;
}

MGTSprite* PsdParser::getPsdCommonSprite(std::string strResName, cocos2d::__Dictionary *pPsdDatas)
{
    
    PsdData *data = PsdParser::getPsdData((__Dictionary *)pPsdDatas->objectForKey(strResName));

    
    std::string pngName = data->file;
    std::string jpgName = data->file;
    pngName.append(".png");
    jpgName.append(".jpg");
    
    
    MGTSprite *tempSprite = MGTSprite::createWithCommonPath(pngName.c_str());
    
    if(NULL == tempSprite)
    {
        tempSprite = MGTSprite::createWithCommonPath(jpgName.c_str());
    }
    if(NULL == tempSprite)
    {
        CCLOG("NO IMAGE FILE!");
    }
    
    tempSprite->setPosition(data->pos);
    
    delete data;
    
    return tempSprite;
}


MGTSprite* PsdParser::getPsdSpriteInFolder(std::string strFolder, std::string strResName, __Dictionary *pPsdDatas)
{
    
    PsdData *data = PsdParser::getPsdData((__Dictionary *)pPsdDatas->objectForKey(strResName));
    
    
    std::string pngName = data->file;
    std::string jpgName = data->file;
    pngName.append(".png");
    jpgName.append(".jpg");
    
    
    MGTSprite *tempSprite = MGTSprite::createWithFullPath((strFolder +"/"+ pngName).c_str());
    
    if(NULL == tempSprite)
    {
        tempSprite = MGTSprite::createWithFullPath((strFolder +"/"+ pngName).c_str());
    }
    if(NULL == tempSprite)
    {
        CCLOG("NO IMAGE FILE!");
    }
    
    tempSprite->setPosition(data->pos);
    
    delete data;
    
    return tempSprite;
}


Vec2 PsdParser::getPsdPosition(std::string strResName, cocos2d::__Dictionary *pPsdDatas)
{
    PsdData *data = PsdParser::getPsdData((__Dictionary *)pPsdDatas->objectForKey(strResName));
    Vec2 pos = data->pos;
    delete data;
    
    return pos;
}

Size PsdParser::getPsdSize(std::string strResName, cocos2d::__Dictionary *pPsdData)
{
    PsdData *data = PsdParser::getPsdData((__Dictionary *)pPsdData->objectForKey(strResName));
    Size size = data->size;
    delete data;
    
    return size;
}
