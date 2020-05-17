
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

/* cocos2d-x  ���� */
bool AppDelegate::applicationDidFinishLaunching() {
	auto director = Director::getInstance();
	auto glview = director->getOpenGLView();

	KDataProvider::getInstance()->loadXmlData("ebookinfo.xml");

	KXMLReader * pReader = KXMLReader::getInstance();
	pReader->setXmlData(KDataProvider::getInstance()->getXmlData());
	bool bReturn = pReader->parseFileForBookInfo();
	if (bReturn == false) {
		log("can't read the file. :: ȭ���� ������ �����ϴ�.");
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
		/* windows �̸������ Ÿ��Ʋ�� windows������ */
		glview = GLViewImpl::createWithRect("eBook Preview", Rect(0, 0, 1280, 800));// , 768));
		director->setOpenGLView(glview);
	}
	STBOOK_INFO * pBookInfo = pReader->getBookInfo();
	ResolutionPolicy nRpolicy = ResolutionPolicy::SHOW_ALL;
	if (pBookInfo->screenType.empty() == false)
	{
		if (pBookInfo->screenType =="EXACT_FIT") /* ����̽� ȭ�鿡 Ǯ������� ���� */
		{
			nRpolicy = ResolutionPolicy::EXACT_FIT;  
			log("screen type %s", pBookInfo->screenType.c_str());
		}
		else nRpolicy = ResolutionPolicy::SHOW_ALL; /* ������ �°� ��������*/
	}
	glview->setDesignResolutionSize(nWidth, nHeight, nRpolicy);     

	if (pBookInfo->backgroundColor.empty() == false)
	{
		Color3B col = CPage::convertRGB(pBookInfo->backgroundColor.c_str());
		director->setClearColor(Color4F(col));
	}
	else director->setClearColor(Color4F(0, 0, 0, 255)); /* ���� ����� �� �� ���� */
	 
	director->setDisplayStats(false);
	director->setAnimationInterval(1.0f / 60.f);

	pOperator = AppOperator::create();

	/* �����δ� ȣ��  */
	director->runWithScene(KPreloadingPage::createScene(pOperator));

    return true;
}

/* app ��Ȱ��ȭ �� �� */
void AppDelegate::applicationDidEnterBackground() {
    Director::getInstance()->stopAnimation();
	
	CocosDenshion::SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
	cocos2d::experimental::AudioEngine::pauseAll();

}

/* app Ȱ��ȭ �� ��  */
void AppDelegate::applicationWillEnterForeground() {
    Director::getInstance()->startAnimation();
	CocosDenshion::SimpleAudioEngine::getInstance()->resumeBackgroundMusic();
	cocos2d::experimental::AudioEngine::resumeAll();
}







