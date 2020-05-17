/*
 *  MGTUtils.cpp
 *
 *
 *   on 12. 10. 4..
 *  
 */

#include "MGTUtils.h"
#include "DRMManager.h"

static MGTUtils *m_spManagement;

MGTUtils::MGTUtils()
{
}

MGTUtils::~MGTUtils()
{
    releaseInstance();
}

MGTUtils* MGTUtils::getInstance()
{
    if(!m_spManagement)
    {
        m_spManagement  = new MGTUtils();
        
    }
    return m_spManagement;
}

  
void MGTUtils::releaseInstance()
{
    if (m_spManagement)
    {
        delete m_spManagement;
        m_spManagement = NULL;
    }
}



#pragma mark - MSLP control
// Lms control
//void MGTUtils::setLmsStart(std::string menuStr)
//{
//}
//void MGTUtils::setLmsEnd()
//{
//}


// Fix 
//void MGTUtils::setLmsMenuEnd(int iMenuValue)
//{
//    MGTMSLPContentInterface::getInstance()->setMSLPMenuEnd(iMenuValue);
//}
//
//std::string MGTUtils::getMSLPFileName(int iMenuID, eFileType eFileType,
//                                    std::string strExtend1/* = ""*/, std::string strExtend2/* = ""*/)
//{
//    return MGTMSLPContentInterface::getInstance()->getMSLPFileName(iMenuID, eFileType, strExtend1, strExtend2);
//}
//
//bool MGTUtils::saveMSLPFile()
//{
//    return MGTMSLPContentInterface::getInstance()->saveMSLPFile();
//}

#pragma mark - memory controll

void MGTUtils::removeAllchildren(Node *node)
{
    if (node == NULL)
        return;
    
    node->stopAllActions();
    node->unscheduleUpdate();
    node->unscheduleAllCallbacks();
    
    if (node->getChildrenCount() > 0)
    {
        for (int i=0; i < node->getChildrenCount(); i++)
        {
            auto child = node->getChildren().at(i);
            
            if (child->getChildrenCount() != 0)
            {
                removeAllchildren(child);
            }
        }
    }
    
    node->removeAllChildrenWithCleanup(true);

    
}

#pragma mark - action controll

void MGTUtils::stopAllAnimations(Node *node)
{
    node->stopAllActions();
    
    if (node->getChildrenCount() == 0) {
        return;
    }
    
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            stopAllAnimations(child);
        }
        else
        {
            child->stopAllActions();
        }
    }
}

void MGTUtils::pauseAllAnimations(Node *node)
{
    node->pause();
    
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            pauseAllAnimations(child);
        }
        else
        {
            child->pause();
        }
    }
}


void MGTUtils::resumeAllAnimations(Node *node)
{
    node->resume();
    
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            resumeAllAnimations(child);
        }
        else
        {
            child->resume();
        }
    }
}


void MGTUtils::enterBackground()
{
    //MGTPlatformBridge::sharedPlatformBridge()->pauseVideo();
    
    if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    {
        //MGTPlatformBridge::sharedPlatformBridge()->pauseAllSounds();
    }
}

void MGTUtils::enterForeground()
{
    //MGTPlatformBridge::sharedPlatformBridge()->resumeVideo();
    
    if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
    {
        //MGTPlatformBridge::sharedPlatformBridge()->resumeAllSounds();
    }
}


#pragma mark - touch

bool MGTUtils::hitTestPoint(Node* pNode, const Vec2& pos, bool bCenter)
{
    Vec2 local = pNode->convertToNodeSpace(pos);
    Rect r = MGTUtils::getRect(pNode);
    r.origin = Vec2::ZERO;
    if (r.containsPoint( local))
    {
        if (bCenter == true)
        {
            float moveX = local.x - r.size.width/2;
            float moveY = local.y - r.size.height/2;
            
            pNode->setPosition(Vec2(pNode->getPosition().x+moveX, pNode->getPosition().y+moveY));
        }
        
        return true;
    }
    
    return false;
}


bool MGTUtils::hitTestPointExact(Node* pNode, Image* imageData, const Vec2& pos, bool bCenter)
{
    if (!pNode->getBoundingBox().containsPoint(pos))
        return false;
    
    auto nodeTap = pNode->convertToNodeSpace(pos);
    unsigned x = unsigned(nodeTap.x) % imageData->getWidth();
    unsigned y = unsigned(imageData->getHeight() - nodeTap.y) % imageData->getHeight();
    unsigned index = x + y * imageData->getWidth();
    ssize_t dataLen = imageData->getDataLen();
    CCAssert(index < dataLen, "index is bigger than image size.");
    unsigned char *pixel = imageData->getData() + (4 * index);
    return !isZeroPixel(pixel);
}


bool MGTUtils::isZeroPixel(unsigned char *pixel)
{
//    log("%u, %u, %u ,%u", pixel[0], pixel[1], pixel[2], pixel[3]);
    
    if ((int)pixel[0] == 0 && (int)pixel[1] == 0 && (int)pixel[2] == 0 && (int)pixel[3] == 0)
    {
        return true;
    }
    return false;
}

bool MGTUtils::isTestPixel(unsigned char *pixel, int r, int g, int b, int a)
{
    //0, 173, 255, 255 파랑
    return r == pixel[0] || g == pixel[1] || b == pixel[2] || a == pixel[3];
}


bool MGTUtils::hitTestObjects(Node* pNode1, Node* pNode2)
{
    Vec2 node2worldPos = pNode2->convertToWorldSpace(Vec2::ZERO);
    
    Vec2 node2local = pNode1->convertToNodeSpace(node2worldPos);
    
    Rect r1 = MGTUtils::getRect(pNode1);
    r1.origin = Vec2::ZERO;
    
    Rect r2 = MGTUtils::getRect(pNode2);
    r2.origin = node2local;
    
    if (r1.intersectsRect(r2))
    {
        return true;
    }
    
    return false;
}



bool MGTUtils::containRect(Layer* layer, Node* node, Vec2 touch)
{
    if (node == NULL) return false;
    
    float width = node->getPosition().x;
    float height = node->getPosition().y;
    
    Size size = Size(node->getBoundingBox().size.width, node->boundingBox().size.height);
    Vec2 anchor = node->getAnchorPoint();
    
    float anchorWidth = width-(size.width*anchor.x);
    float anchorHeight = height-(size.height*anchor.y);
    
    
    Vec2 modifyPoint = Vec2(anchorWidth
                                      , anchorHeight);
    Rect modifyRect = Rect(modifyPoint.x
                                   , modifyPoint.y, size.width, size.height);
    
    float s = sinf(node->getRotation()*(M_PI/180));
    float c = cosf(node->getRotation()*(M_PI/180));
    
    Vec2 modifyTouch = Vec2(width+((touch.x-width)*c-(touch.y-height)*s)
                              , height+((touch.x-width)*s+(touch.y-height)*c));
    
    return modifyRect.containsPoint(modifyTouch);
}

bool MGTUtils::containRect(Layer* layer, Node* node, Vec2 touch, float band)
{
	float x = node->getPosition().x;
	float y = node->getPosition().y;
    
	Size size = node->getBoundingBox().size;
	Vec2 anchor = node->getAnchorPoint();
    
	float anchorWidth = x-((size.width+band)*anchor.x);
	float anchorHeight = y-((size.height+band)*anchor.y);
    
	Vec2 modifyPoint = Vec2(anchorWidth
                                      , anchorHeight);
	Rect modifyRect = Rect(modifyPoint.x
                                   , modifyPoint.y, size.width+band, size.height+band);
    
	float s = sinf(node->getRotation()*(M_PI/180));
	float c = cosf(node->getRotation()*(M_PI/180));
    
	Vec2 modifyTouch = Vec2(x+((touch.x-x)*c-(touch.y-y)*s)
                              , y+((touch.x-x)*s+(touch.y-y)*c));
    
	return modifyRect.containsPoint(modifyTouch);
}

bool MGTUtils::containPointWithBand(Vec2 targetPoint, Vec2 touch, float bandSize)
{
    
	float x = targetPoint.x;
	float y = targetPoint.y;
    
	Vec2 modifyPoint = Vec2(x-bandSize/2 , y-bandSize/2);
	Rect modifyRect = Rect(modifyPoint.x, modifyPoint.y, bandSize, bandSize);
    
    //LayerColor* layer1 = LayerColor::layerWithColorWidthHeight(Color4F(50,200,50,0), bandSize, bandSize);
    //layer1->setPosition(modifyPoint);
    //layer->addChild(layer1,100);
    
	return modifyRect.containsPoint(touch);
}

cocos2d::Rect MGTUtils::getRect(Node* pNode)
{
    return cocos2d::Rect(pNode->getPosition().x - (pNode->getContentSize().width * pNode->getScaleX()) * pNode->getAnchorPoint().x,
                        pNode->getPosition().y - (pNode->getContentSize().height * pNode->getScaleY()) * pNode->getAnchorPoint().y,
                        pNode->getContentSize().width * pNode->getScaleX(),
                        pNode->getContentSize().height * pNode->getScaleY());
}


#pragma mark - edit string

std::string MGTUtils::getStringMerge(std::string str1, std::string str2)
{
    std::string tempStr;
    tempStr.clear();
    
    tempStr.append(str1);
    tempStr.append(str2);
    
    return tempStr;
}

std::string MGTUtils::getStringMerge(std::string str1, int nNumber)
{
    std::string tempStr;
    std::stringstream tempStream;
    
    tempStr.clear();
    tempStream.clear();
    
    tempStream << nNumber;
    
    tempStr.append(str1);
    tempStr.append(tempStream.str());
    
    return tempStr;
}

std::string MGTUtils::getStringMerge(std::string str1, int nNumber, std::string str2)
{
    std::string tempStr;
    std::stringstream tempStream;
    
    tempStr.clear();
    tempStream.clear();
    
    tempStream << nNumber;
    
    tempStr.append(str1);
    tempStr.append(tempStream.str());
    tempStr.append(str2);
    
    return tempStr;
}


std::string MGTUtils::getStringFormatInteger(std::string format, int nInteger)
{
    return cocos2d::__String::createWithFormat(format.c_str(), nInteger)->getCString();
}

std::string MGTUtils::getStringFormatString(std::string format, std::string str)
{
    return cocos2d::__String::createWithFormat(format.c_str(), str.c_str())->getCString();
}


std::string MGTUtils::stringTokenizer(std::string sentence, std::string tok, bool first)
{
    std::string tempStr = sentence;
    
    int start, stop, n = (int)tempStr.length();
    
    for(start = (int)tempStr.find_first_not_of(tok); 0 <= start && start < n; start = (int)tempStr.find_first_not_of(tok, stop + 1))
    {
        stop = (int)tempStr.find_first_of(tok, start);
        
        if (stop < 0 || stop > n)
            stop = n;
        
        if (first)
        {
            return tempStr.substr(start, stop - start);
        }
        else if (stop == n)
        {
            return tempStr.substr(start, stop - start);
        }
    }
    
    return "error";
}

std::string MGTUtils::stringTokenizer(std::string sentence, std::string tok, int index)
{
    std::string tempStr = sentence;
    int i = 1;
    
    int start, stop, n = (int)tempStr.length();
    
    for(start = (int)tempStr.find_first_not_of(tok);
        0 <= start && start < n;
        start = (int)tempStr.find_first_not_of(tok, stop + 1))
    {
        stop = (int)tempStr.find_first_of(tok, start);
        if (stop < 0 || stop > n) stop = n;
        
        if (i == index)
        {
            return tempStr.substr(start, stop - start);
        }
        i++;
    }
    
    return "error";
}



// string toolkit
void MGTUtils::split(std::string src, const char* token, strArray& vect)
{
    int nend=0;
    int nbegin=0;
    while(nend != -1)
    {
        nend = (int)src.find(token, nbegin);
        if(nend == -1)
            vect.push_back(src.substr(nbegin, src.length()-nbegin));
        else
            vect.push_back(src.substr(nbegin, nend-nbegin));
        nbegin = nend + (int)strlen(token);
    }
}

std::vector<char> MGTUtils::splitCharacter(const std::string& str)
{
    std::vector<char> result;
    
//    for (char ch : str)
//    {
////        if (isalnum(ch))
////        {
//            result.push_back(ch);
////        }
//    }
    
    
    for (int i =0; i<str.length(); i++)
    {
        result.push_back(str.at(i));
    }
    
    return result;
}



#pragma mark - position & anchorPoint Function

void MGTUtils::setPositionForParent(Node* parent, Node* child)
{
	Size parentSize = parent->getContentSize();
	Vec2 parentAnchor = parent->getAnchorPoint();
	Vec2 childModifyPoint = child->getPosition() - parent->getPosition();
	child->setPosition(
                       Vec2(
                           childModifyPoint.x+parentSize.width*parentAnchor.x,
                           childModifyPoint.y+parentSize.height*parentAnchor.y
                           )
                       );
}

void MGTUtils::setAnchorPointForPosition(Node* node, Vec2 anchor)
{
    Size size = node->getContentSize();
    Vec2 position = node->getPosition();
    Vec2 originAnchor = node->getAnchorPoint();
    
    node->setPosition(Vec2(position.x-((size.width*node->getScaleX())*(originAnchor.x-anchor.x)), position.y-((size.height*node->getScaleY())*(originAnchor.y-anchor.y))));
    node->setAnchorPoint(anchor);
}

void MGTUtils::setTransformCenterPosition(Node* node, Vec2 center)
{
    Vec2 originPoint = node->getPosition();
    Size winSize = Director::getInstance()->getWinSize();
    node->setPosition(Vec2(center.x-(winSize.width/2-originPoint.x), center.y-(winSize.height/2-originPoint.y)));
}

Vec2 MGTUtils::getTransformCenterPoint(Vec2 origin, Vec2 center)
{
    Size winSize = Director::getInstance()->getWinSize();
    return Vec2(center.x-(winSize.width/2-origin.x), center.y-(winSize.height/2-origin.y));
}

#pragma mark - random Function
float MGTUtils::randomFloat(float low, float hi)
{
    float r = (float)(std::rand() & 32767);
    r /= 32767;
    r = (hi - low) * r + low;
    return r;
}

/*  randomInteger(0,5) = {0,1,2,3,4,5} */
int MGTUtils::randomInteger(int low, int hi)
{
    int r = arc4random() % (hi+1 - low) + low;
    return r;
}

int MGTUtils::randomIntegerWithoutValue(unsigned int nMax, unsigned int nWithoutValue)
{
    int randValue = 0;
    
    do{
        randValue = arc4random()%nMax+1;
    }while(randValue == nWithoutValue);
    
    return randValue;
}

bool MGTUtils::randomBoolean()
{
    return arc4random()%2 == 1 ? true : false;
}

float MGTUtils::roundValue(float value, int pos)
{
    float temp;
    
    temp = value*pow(10, pos);
    temp = floor(temp+0.5);
    temp *= pow(10, -pos);
    
    return temp;
}


int* MGTUtils::randomIntegerArr(const unsigned nMax)
{
    int count = nMax;
    int *array = new int[nMax];
    
    for(int i=0; i<nMax; i++)
    {
        array[i] = i;
    }
    for(int i=0; i<nMax; i++)
    {
        CC_SWAP(array[arc4random()%count], array[count-1], int);
        count -= 1;
    }
    
    return array;
}


int MGTUtils::random_user(int n)
{
    return std::rand() % n;
};


#pragma mark - performSelector

void MGTUtils::performSelector(cocos2d::Ref *pTarget,
                                        cocos2d::SEL_CallFunc aSelector,
                                        float afterDelay,
                                        int tag)
{
    cocos2d::FiniteTimeAction *performSeq = cocos2d::Sequence::create(
                                                                      cocos2d::DelayTime::create(afterDelay),
                                                                      cocos2d::CallFunc::create(pTarget, aSelector), nullptr);
    performSeq->setTag(tag);
    
    ((cocos2d::Node *)pTarget)->runAction(performSeq);
}

void MGTUtils::performSelector(cocos2d::Ref *pTarget,
                                        cocos2d::SEL_CallFuncND aSelector,
                                        void *pObject,
                                        float afterDelay,
                                        int tag)
{
    cocos2d::FiniteTimeAction *performSeq = cocos2d::Sequence::create(cocos2d::DelayTime::create(afterDelay),
                                                                      cocos2d::__CCCallFuncND::create(pTarget, aSelector, pObject),
                                                                      nullptr);
    performSeq->setTag(tag);
    
    ((cocos2d::Node *)pTarget)->runAction(performSeq);
}

void MGTUtils::performSelectorCancel(cocos2d::Ref *pTarget,
                                              int tag)
{
    ((cocos2d::Node *)pTarget)->stopActionByTag(tag);
}


#pragma mark - action

void MGTUtils::fadeOutAllchildren(Node* node, float duration)
{
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            fadeOutAllchildren(child, duration);
        }
        else
        {
            ActionInterval* fadeTo_child = FadeTo::create(duration, 0.0f);
            child->runAction(fadeTo_child);
        }
    }
    
    ActionInterval* fadeTo = FadeTo::create(duration, 0.0f);
    node->runAction(fadeTo);
}

void MGTUtils::fadeInAllchildren(Node* node, float duration)
{
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            fadeInAllchildren(child, duration);
        }
        else
        {
            ActionInterval* fadeTo_child = FadeTo::create(duration, 255.0f);
            child->runAction(fadeTo_child);
        }
    }
    
    ActionInterval* fadeTo = FadeTo::create(duration, 255.0f);
    node->runAction(fadeTo);
}

void MGTUtils::fadeToAllchildren(Node* node, float duration, GLubyte opacity, float delaytime)
{
    for (int i=0; i < node->getChildrenCount(); i++)
    {
        auto child = node->getChildren().at(i);
        
        if (child->getChildrenCount() != 0)
        {
            fadeToAllchildren(child, duration, opacity, delaytime);
        }
        else
        {
            if (delaytime == 0.0f)
            {
                ActionInterval* fadeTo_child = FadeTo::create(duration, opacity);
                child->runAction(fadeTo_child);
            }
            else
            {
                ActionInterval* fadeTo_child = FadeTo::create(duration, opacity);
                
                auto seq = Sequence::create(DelayTime::create(delaytime),
                                            fadeTo_child,
                                            nullptr);
                child->runAction(seq);
            }
        }
    }
    
    ActionInterval* fadeTo = FadeTo::create(duration, opacity);
    node->runAction(fadeTo);
}



ActionInterval* MGTUtils::getSeqAction(int nRepeatCount,
                                        float fSeqTime,
                                        std::string strSeqImageName, ...)
{
    Animation *animation = Animation::create();
    
    va_list args;
    va_start(args, strSeqImageName);
    
    const char *pszSeqImageName = strSeqImageName.c_str();
    while(pszSeqImageName != NULL)
    {
        animation->addSpriteFrameWithFile(pszSeqImageName);
        animation->setDelayPerUnit(fSeqTime);
        animation->setRestoreOriginalFrame(false);
        
        pszSeqImageName = va_arg(args, const char *);
        if(pszSeqImageName == NULL)
        {
            break ;
        }
    }
    va_end(args);
    
    
    Animate *seqAction = Animate::create(animation);
    
    if(nRepeatCount == -1)
    {
        return RepeatForever::create(seqAction);
    }
    else
    {
        return Repeat::create(seqAction, nRepeatCount);
    }
}

ActionInterval* MGTUtils::getSeqAction(int nRepeatCount,
                                        float fSeqTime,
                                        Ref *pCallbackTarget,
                                        SEL_CallFunc aCallbackSelector,
                                        std::string strSeqImageName, ...)
{
    Animation *animation = Animation::create();
    
    va_list args;
    va_start(args, strSeqImageName);
    
    const char *pszSeqImageName = strSeqImageName.c_str();
    while(pszSeqImageName != NULL)
    {
        animation->addSpriteFrameWithFile(pszSeqImageName);
        animation->setDelayPerUnit(fSeqTime);
        animation->setRestoreOriginalFrame(false);
        
        pszSeqImageName = va_arg(args, const char *);
        if(pszSeqImageName == NULL)
        {
            break ;
        }
    }
    va_end(args);
    
    
    Animate *seqAction = Animate::create(animation);
    
    if(nRepeatCount == -1)
    {
        return RepeatForever::create(seqAction);
    }
    else
    {
        auto repeatAction = Repeat::create(seqAction, nRepeatCount);
        auto callbackAction = CallFunc::create(pCallbackTarget, aCallbackSelector);
        
        return (ActionInterval *)Sequence::create(repeatAction, callbackAction, nullptr);
    }
}





Animate* MGTUtils::getAnimation(float fFrameDelay, std::string strSeqImageName, ...)
{
    Animation *animation = Animation::create();
    
    va_list args;
    va_start(args, strSeqImageName);
    
    const char *pszSeqImageName = strSeqImageName.c_str();
    while(pszSeqImageName != NULL)
    {
        animation->addSpriteFrameWithFile(pszSeqImageName);
        animation->setDelayPerUnit(fFrameDelay);
        animation->setRestoreOriginalFrame(false);
        
        pszSeqImageName = va_arg(args, const char *);
        if(pszSeqImageName == NULL)
        {
            break ;
        }
    }
    va_end(args);
    
    return Animate::create(animation);
}

ActionInterval* MGTUtils::getAnimations(int nRepeatCount,
                                         float fSeqTime,
                                         Animate *pAnimation, ...)
{
    Vector<FiniteTimeAction*> actions;
    
    va_list args;
    va_start(args, pAnimation);
    while (pAnimation != NULL)
    {
        actions.pushBack(pAnimation);
        actions.pushBack(DelayTime::create(fSeqTime));
        
        pAnimation = va_arg(args, Animate *);
        if(pAnimation == NULL)
        {
            break ;
        }
    }
    va_end(args);
    
    
    if(nRepeatCount == -1)
    {
        return RepeatForever::create(Sequence::create(actions));
    }
    else
    {
        return Repeat::create(Sequence::create(actions), nRepeatCount);
    }
}

ActionInterval* MGTUtils::getAnimations(int nRepeatCount,
                                         float fSeqTime,
                                         Ref *pCallbackTarget,
                                         SEL_CallFunc aCallbackSelector,
                                         Animate *pAnimation, ...)
{
    Vector<FiniteTimeAction*> actions;
    
    va_list args;
    va_start(args, pAnimation);
    while (pAnimation != NULL)
    {
        actions.pushBack(pAnimation);
        actions.pushBack(DelayTime::create(fSeqTime));
        
        pAnimation = va_arg(args, Animate *);
        if(pAnimation == NULL)
        {
            break ;
        }
    }
    va_end(args);
    
    if(nRepeatCount == -1)
    {
        return RepeatForever::create(Sequence::create(actions));
    }
    else
    {
        FiniteTimeAction *callback = CallFunc::create(pCallbackTarget, aCallbackSelector);
        actions.pushBack(callback);
        
        return Repeat::create(Sequence::create(actions), nRepeatCount);
    }
}



#pragma mark - macro define

void MGTDialog(const char* pszMessage)
{
    MessageBox("not file Exsit ", pszMessage);
}

void MGTLog(const char * pszFormat, ...)
{
    // MGTLog debug
#if defined(MGT_DEBUGMODE) && MGT_DEBUGMODE == true

    va_list args;
    va_start(args, pszFormat);

    // _log start
    
    char buf[MAX_LOG_LENGTH];
    
    vsnprintf(buf, MAX_LOG_LENGTH-3, pszFormat, args);
    strcat(buf, "\n");
    
#if CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID
    __android_log_print(ANDROID_LOG_DEBUG, "MGTLog debug info",  "%s", buf);
    
#elif CC_TARGET_PLATFORM ==  CC_PLATFORM_WIN32 || CC_TARGET_PLATFORM == CC_PLATFORM_WINRT || CC_TARGET_PLATFORM == CC_PLATFORM_WP8
    WCHAR wszBuf[MAX_LOG_LENGTH] = {0};
    MultiByteToWideChar(CP_UTF8, 0, buf, -1, wszBuf, sizeof(wszBuf));
    OutputDebugStringW(wszBuf);
    WideCharToMultiByte(CP_ACP, 0, wszBuf, -1, buf, sizeof(buf), nullptr, FALSE);
    printf("%s", buf);
    fflush(stdout);
#else
    // Linux, Mac, iOS, etc
    fprintf(stdout, "%s", buf);
    fflush(stdout);
#endif
    
    Director::getInstance()->getConsole()->log(buf);
    
    // _log end
    
    va_end(args);

#endif
}
