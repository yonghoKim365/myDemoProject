/*
 *  MGTUtils.h
 *
 *
 *   on 12. 10. 4..
 *  
 */


#ifndef MGT_COMMON_MANAGEMENT_CLASS_H
#define MGT_COMMON_MANAGEMENT_CLASS_H

 
#define MGT_DEBUGMODE true // true(1):show log , false(0):hide log

#include <sstream>
#include <string>
#include <iomanip>
#include <algorithm>
#include <cctype>

#include "cocos2d.h" 

USING_NS_CC;

class MGTUtils
{
    
private:
    
public:
    MGTUtils();
    ~MGTUtils();
    
    
    static MGTUtils* getInstance();
    static void     releaseInstance();
    


    
    // AppDelegate control
    void resignActive();
    void becomeActive();
    void enterBackground();
    void enterForeground();
    
    // string control
    static std::string getStringMerge(std::string str1, std::string str2);
    static std::string getStringMerge(std::string str1, int nNumber);
    static std::string getStringMerge(std::string str1, int nNumber, std::string str2);
    static std::string getStringFormatInteger(std::string format, int nInteger);
    static std::string getStringFormatString(std::string format, std::string str);
    static std::string stringTokenizer(std::string sentence, std::string tok, bool first);
    static std::string stringTokenizer(std::string sentence, std::string tok, int index);
    
    typedef std::vector<std::string> strArray;
    static void split(std::string src, const char* token, strArray& vect);
    static std::vector<char> splitCharacter(const std::string& str);
    
    // touch bound
    static bool containRect(Layer* layer, Node* node, Vec2 touch);
    static bool containRect(Layer* layer, Node* node, Vec2 touch, float band);
    static bool containPointWithBand(Vec2 targetPoint, Vec2 touch, float bandSize);
    static bool hitTestPoint(Node* pNode, const Vec2& pos, bool bCenter);
    static bool hitTestPointExact(Node* pNode, Image* imageData, const Vec2& pos, bool bCenter);
    static bool hitTestObjects(Node* pNode1, Node* pNode2);
    static cocos2d::Rect getRect(Node* pNode);
    
    static bool isZeroPixel(unsigned char *pixel);
    static bool isTestPixel(unsigned char *pixel, int r, int g, int b, int a);
    
    // position control
    static void setPositionForParent(Node* parent, Node* child);
    static void setAnchorPointForPosition(Node* node, Vec2 anchor);
    static void setTransformCenterPosition(Node* node, Vec2 center);
    static Vec2 getTransformCenterPoint(Vec2 origin, Vec2 center);
    
    // make random
    static float    randomFloat(float low, float hi);
    static int      randomInteger(int low, int hi);
    static int      randomIntegerWithoutValue(unsigned int nMax, unsigned int nWithoutValue);
    static bool     randomBoolean();
    static int*     randomIntegerArr(const unsigned nMax);
    static float    roundValue(float value, int pos);
    static int      random_user(int n);
    
    // @author
    // @brief	performSelector
    static void performSelector(cocos2d::Ref *pTarget,
                                cocos2d::SEL_CallFunc aSelector,
                                float afterDelay = 0.0f,
                                int tag = 0);
    static void performSelector(cocos2d::Ref *pTarget,
                                cocos2d::SEL_CallFuncND aSelector,
                                void *pObject,
                                float afterDelay = 0.0f,
                                int tag = 0);
    static void performSelectorCancel(cocos2d::Ref *pTarget,
                                      int tag);

    
    //Action
    static void stopAllAnimations(Node *node);
    static void pauseAllAnimations(Node *node);
    static void resumeAllAnimations(Node *node);
    static void removeAllchildren(Node *node);
    
    static void fadeInAllchildren(Node* node, float duration);
    static void fadeOutAllchildren(Node* node, float duration);
    static void fadeToAllchildren(Node* node, float duration, GLubyte opacity, float delaytime = 0.0f);
    
    // @author
    // @brief	actions
    static ActionInterval* getSeqAction(int nRepeatCount,
                                          float fSeqTime,
                                          std::string strSeqImageName, ...);
    static ActionInterval* getSeqAction(int nRepeatCount,
                                          float fSeqTime,
                                          Ref *pCallbackTarget,
                                          SEL_CallFunc aCallbackSelector,
                                          std::string strSeqImageName, ...);
    
    
    static Animate* getAnimation(float fFrameDelay, std::string strSeqImageName, ...);
    
    static ActionInterval* getAnimations(int nRepeatCount,
                                           float fSeqTime,
                                           Animate *pAnimation, ...);
    static ActionInterval* getAnimations(int nRepeatCount,
                                           float fSeqTime,
                                           Ref *pCallbackTarget,
                                           SEL_CallFunc aCallbackSelector,
                                           Animate *pAnimation, ...);

    

    
};

void MGTDialog(const char*);
void MGTLog(const char * pszFormat, ...) CC_FORMAT_PRINTF(1, 2);




#endif
