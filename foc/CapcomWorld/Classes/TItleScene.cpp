//
//  TitleScene.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 9. 18..
//
//

#include "CCHttpRequest.h"
#include "TitleScene.h"
#include "SimpleAudioEngine.h"
#include "MainScene.h"
#include <libxml/tree.h>
#include <libxml/parser.h>
#include <libxml/xmlstring.h>
#include <libxml/xpath.h>
#include <libxml/xpathInternals.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "CardDictionary.h"
#include "FileManager.h"
#include "ARequestSender.h"
#include "KakaoLoginScene.h"
#include "LocalizationManager.h"


using namespace cocos2d;
using namespace CocosDenshion;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    using namespace cocos2d::extension;
#endif

TitleScene *TitleScene::titleInst = NULL;
CCScene* TitleScene::scene()
{
    //CCLog("titlescene debug msg");
    
    // 'scene' is an autorelease object
    CCScene *scene = CCScene::create();
    
    // 'layer' is an autorelease object
    TitleScene *layer = TitleScene::create();
    
    // add layer as a child to scene
    scene->addChild(layer);
    
    //CCLog("titlescene debug msg 2");
    
    
    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool TitleScene::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !CCLayer::init() )
    {
        return false;
    }
    
    
    // ask director the window size
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    //CCSprite* pSprite = CCSprite::create("title/START_03_CW.png");//HelloWorld.png");
    CCSprite* pSprite = CCSprite::create("title/title_bg_s.png");//HelloWorld.png");
    pSprite->setPosition( ccp(size.width/2, size.height/2) );
    this->addChild(pSprite, 0);
    addPageLoading(this);
    
    if (titleInst == NULL)
    {
//        CardDictionary::sharedCardDictionary()->init();
//        FileManager::sharedFileManager()->Init();
//        LocalizationManager::getInstance()->init();

        titleInst = this;

        PlayerInfo::getInstance()->xb = new XBridge();
        PlayerInfo::getInstance()->xb->kakao();
    }
    else
    {
        titleInst = this;
        PlayerInfo::getInstance()->xb->rekakao();
    }
//    this->schedule(schedule_selector(TitleScene::goMainScene),1.0);
    
    //xb = new XBridge(); //XBridge *xb = new XBridge();
    //PlayerInfo::getInstance()->SetDeviceID(xb->getDeviceID());
    
    return true;
}


/*
- (void)authLoginViewControllerdidFinishLogin:(KakaoAuthLoginViewController *)authLoginViewController
{
    [self showAuthTestView];
}
*/

void TitleScene::gameLogic(cocos2d::CCTime dt)
{
    CCLog(" call game logic");
}

void TitleScene::switchMainScene()
{
    titleInst->schedule(schedule_selector(TitleScene::goMainScene),1.0);
    //CCDirector::sharedDirector()->replaceScene(MainScene::scene());
}

void TitleScene::goMainScene()
{
    removePageLoading(this);
    CCDirector::sharedDirector()->replaceScene(MainScene::scene());
}

void TitleScene::switchLoginScene()
{
    CCLog(" TitleScene::switchLoginScene() ----- ");
    removePageLoading(this);
    this->schedule(schedule_selector(TitleScene::goLoginScene),1.0);
}

void TitleScene::goLoginScene()
{
    CCLog(" TitleScene::goLoginScene ----- ");
    removePageLoading(this);
    CCDirector::sharedDirector()->replaceScene(KakaoLoginScene::scene());
    this->unschedule(schedule_selector(TitleScene::goLoginScene));
}

xmlDocPtr doc;

static void
print_element_names(xmlNode * a_node)
{
    xmlNode *cur_node = NULL;
    
    for (cur_node = a_node; cur_node; cur_node = cur_node->next) {
        if (cur_node->type == XML_ELEMENT_NODE) {
            printf("node type: Element, name: %s\n", cur_node->name);
        }
        
        print_element_names(cur_node->children);
    }
}

void processXml()
{
    std::string fileName = CCFileUtils::sharedFileUtils()->getDocumentPath() + "/samplebook.xml";
    FILE *filePointer = fopen(fileName.data(), "rt");
    fseek(filePointer, 0, SEEK_END);
    int length = ftell(filePointer);
    fseek(filePointer, 0, SEEK_SET);
    char *buffer = new char[length + 1];
    fread(buffer, length, 1, filePointer);
    fclose(filePointer);

    doc = xmlReadMemory(buffer, length, "samplebook.xml", NULL, 0);
    xmlNode *root_element = xmlDocGetRootElement(doc);
    print_element_names(root_element);
    xmlFreeDoc(doc);
    
    CC_SAFE_DELETE_ARRAY(buffer);
}

void TitleScene::onHttpRequestCompleted(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if (response->request->reqType == kHttpRequestGet) {
        if (response->succeed) {
            CCLog("Get Request Completed!");
            CCLog("Content: %s", response->responseData.c_str());
        } else {
            CCLog("Get Error: %s", response->responseData.c_str());
        }
    } else if (response->request->reqType == kHttpRequestPost) {
        if (response->succeed) {
            CCLog("Post Request Completed!");
            CCLog("Content: %s", response->responseData.c_str());
        } else {
            CCLog("Post Error: %s", response->responseData.c_str());
        }
    } else if (response->request->reqType == kHttpRequestDownloadFile) {
        if (response->succeed) {
            CCLog("Download Request Completed! Downloaded:");
            
            std::vector<std::string>::iterator iter;
            for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter) {
                std::string url = *iter;
                CCLog("%s", url.c_str());
            }
      
//            processXml();
            
/*            std::string saveFileName = CCFileUtils::sharedFileUtils()->getDocumentPath() + "/START_03_CW.png";
            char buffer[512];
            getDocumentPath(buffer);
            strcat(buffer, "/START_03_CW.png");
            printf("%s\n", saveFileName.data());

            CCSprite* pSprite = CCSprite::create(saveFileName.data());//"START_03_CW.png");//HelloWorld.png");
            CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
            
            // position the sprite on the center of the screen
            pSprite->setPosition( ccp(size.width/2, size.height/2) );
            
            // add the sprite as a child to this layer
            this->addChild(pSprite, 0);
            
            //this->schedule(schedule_selector(TitleScene::gameLogic),1.0);
            
            this->schedule(schedule_selector(TitleScene::goMainScene),1.0);*/
        } else {
            CCLog("Download Error: %s", response->responseData.c_str());
        }
    }
}
