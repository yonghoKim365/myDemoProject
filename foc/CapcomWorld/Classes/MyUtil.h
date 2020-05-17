//
//  MyUtil.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 10. 12..
//
//

#ifndef __CapcomWorld__MyUtil__
#define __CapcomWorld__MyUtil__

#include <iostream>
#include "cocos2d.h"
#include "FileManager.h"
#include "SimpleAudioEngine.h" 
#include "LocalizationManager.h"
#include "CardInfo.h"
#include "PopupDelegate.h"
#include "PopupRetryDelegate.h"
#include "GameConst.h"

using namespace cocos2d;
using namespace CocosDenshion;

class MyUtil
{
    public :
        cocos2d::CCPoint accp( float x, float y);
        float accp(float x);
        int   accp(int x);
    
        CCSprite *createNumber(int number, CCPoint pos, float scale = 1.0f);
    
        void registerLabel(CCLayer *_layer, CCPoint anchor, CCPoint pos, CCLabelTTF* pLabel, int z);
        void regSprite(CCLayer *_layer, CCPoint anchor, CCPoint pos, CCSprite *pSpr, int z);
        void regSprite(CCLayer *_layer, CCPoint anchor, CCPoint pos, CCSprite *pSpr, int z, int _tag);
        void regSprites(cocos2d::CCLayer *_layer, int nSprite, std::string paths[], float anc[], float pos[], float z[], int tags_start );
        void regAni(CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, int _tag, int _z, float delay = 0.2f, float scale = 1.0f);
    
#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    static const float SCREEN_ZOOM_RATE = 2;
#elif (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
        static const float SCREEN_ZOOM_RATE = 1;
#endif

//        const char* GetCharImgPath(int nCharId, int nSize);
    
        void CheckLayerSize(CCLayer *layer);
        void CheckSize(CCLayer *layer, CCRect rect);

        float layerYPositions[10];
        float layerZorders[10];
        void ToTopZPriority(CCLayer *_layer);
        void RestoreZProirity(CCLayer *_layer);
    
        bool GetSpriteTouchCheckByTag(CCLayer *layer, int tag, cocos2d::CCPoint localPoint);
    
        // draw number
        CCSprite *numSprs[7];
        void RefreshNumber(int point, int x, int y, CCLayer *layer);
        void removeNumSprites(CCLayer *_layer);
        void format_commas(int n, char *out);
        CCSprite *createComma(CCPoint pos, float scale = 1.0f);
    
        void drawInt(CCLayer *layer, int number, cocos2d::ccColor3B color, CCPoint pos);
    
        void requestCardImg(CCLayer *player, int CardID, int SizeType, CCPoint pos, float scale, int z,  int tag);
    
        unsigned long GetTimeTick();
    

        void moveSprite(CCSprite* pSpr, float x, float y, float duration, CCObject* pObj, SEL_CallFunc selector);
    
        void AniPlay(CCSprite* pSprAni, CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, float scale, int _tag, int _z, SEL_CallFunc selector);
    
        void AniPlay(CCSprite* pSprAni, CCArray *aniFrames, CCLayer *_layer, CCPoint anc, CCPoint pos, float scale, int _tag, int _z, SEL_CallFuncND selector, int repeat = 1);

        void ChangeSpr(CCLayer *_layer, int _tag, std::string newSprPath, int z);
        void ChangeSpr(CCLayer *_layer, int _tag, int _newTag, std::string newSprPath, int z);
    
        std::string md5(const std::string strMd5);
    
        void popupNetworkError(const char* text1, const char* text2, const char* text3);
        void popupOk(const char* text1);
        void popupRetry(const char *text1, PopupRetryDelegate *_delegate);
        void popupQuest(const char *text1);
        void popupOkCancel(CardInfo *card, const char* text1, PopupDelegate *_delegate);

        void soundMainBG();
        void soundButton1();
        void soundButton2();
        int playEffect(const char* pszFilePath);
        void runEffect();
        void soundGo();
        void soundRing();
        void soundReady();
        void soundHit();
        void soundCoin();
        void soundExpUp();
        void soundKo();
        void soundBreakDrum();
        void soundRival();
        void soundRivalBG();

        void addLoadingAni();
        void removeLoadingAni();
    
        void addLoadingAni(CCLayer *layer);
        void removeLoadingAni(CCLayer *layer);
    
        void addPageLoading();
        void removePageLoading();
    
        void addPageLoading(CCLayer *layer);
        void removePageLoading(CCLayer *layer);

        static bool popupOpened;
        static int tutorialProgress;
        static const int TUTORIAL_DONE = GameConst::TUTORIAL_TOTAL;
        
        std::map<CCObject*, bool> TouchEnableTable;
        void _setDisableWithRunningScene(CCNode* node);
        void restoreTouchDisable();
        void setDisableWithRunningScene();
    
        void resultBG_On();
        void resultBG_Off();
        CocosDenshion::SimpleAudioEngine* resultBG;

        void addClickIcon(CCLayer *layer, float x, float y);
        void removeClickIcon(CCLayer *layer);
    
};

#endif /* defined(__CapcomWorld__MyUtil__) */



