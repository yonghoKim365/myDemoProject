
#include "AppDelegate.h"
#include "AppOperator.h"
#include "KPreloadingPage.h"

#include "KDataProvider.h"

USING_NS_CC;



AppDelegate::AppDelegate() {	
	pOperator = nullptr;
}
AppDelegate::~AppDelegate()  {
	if (pOperator != nullptr) {
		delete pOperator;
		pOperator = nullptr;
	}
}
void AppDelegate::initGLContextAttrs() {
    GLContextAttrs glContextAttrs = {8, 8, 8, 8, 24, 8};
    GLView::setGLContextAttrs(glContextAttrs);
}
static int register_all_packages() {
    return 0; //flag for packages manager
}

/* cocos2d-x  시작 */
bool AppDelegate::applicationDidFinishLaunching() {
	auto director = Director::getInstance();
	auto glview = director->getOpenGLView();

	KDataProvider::getInstance()->loadXmlData("ebookinfo.xml");

	KXMLReader * pReader = KXMLReader::getInstance();
	pReader->setXmlData(KDataProvider::getInstance()->getXmlData());
	bool bReturn = pReader->parseFileForBookInfo();
	if (bReturn == false) {
		log("can't read the file. :: 화일을 읽을수 없습니다.");
		pReader = nullptr;
		return true;
	}

	int nWidth = pReader->getContentWidth();
	int nHeight = pReader->getContentHeight();
	//int nRealHeight = (nHeight * 1024) / nWidth;
	int nRealHeight = (nHeight * 1280) / nWidth;
	CViewUtils::STAGE_WIDTH = nWidth;
	CViewUtils::STAGE_HEIGHT = nHeight;

	if (!glview) {
		/* windows 미리보기용 타이틀과 windows사이즈 */
		glview = GLViewImpl::createWithRect("eBook Preview", Rect(0, 0, 1280, 800));// , 768));
		director->setOpenGLView(glview);
	}
	STBOOK_INFO * pBookInfo = pReader->getBookInfo();
	ResolutionPolicy nRpolicy = ResolutionPolicy::SHOW_ALL;
	if (pBookInfo->screenType.empty() == false)
	{
		if (pBookInfo->screenType =="EXACT_FIT") /* 디바이스 화면에 풀사이즈로 나옴 */
		{
			nRpolicy = ResolutionPolicy::EXACT_FIT;  
			log("screen type %s", pBookInfo->screenType.c_str());
		}
		else nRpolicy = ResolutionPolicy::SHOW_ALL; /* 비율에 맞게 나오게함*/
	}
	glview->setDesignResolutionSize(nWidth, nHeight, nRpolicy);     

	if (pBookInfo->backgroundColor.empty() == false)
	{
		Color3B col = CPage::convertRGB(pBookInfo->backgroundColor.c_str());
		director->setClearColor(Color4F(col));
	}
	else director->setClearColor(Color4F(0, 0, 0, 255)); /* 여백 생기는 곳 색 지정 */
	 
	director->setDisplayStats(false);
	director->setAnimationInterval(1.0f / 60.f);

	pOperator = AppOperator::create();

	/* 프리로더 호출  */
	director->runWithScene(KPreloadingPage::createScene(pOperator));

    return true;
}

/* app 비활성화 될 때 */
void AppDelegate::applicationDidEnterBackground() {
    Director::getInstance()->stopAnimation();
	
	CocosDenshion::SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
	cocos2d::experimental::AudioEngine::pauseAll();

}

/* app 활성화 될 때  */
void AppDelegate::applicationWillEnterForeground() {
    Director::getInstance()->startAnimation();
	CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
	cocos2d::experimental::AudioEngine::resumeAll();
}







