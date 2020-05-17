#include "Debug_Index.h"
#include "ProductIndex.h"
#include "MSLPManager.h"
#include "DRMManager.h"
enum
{
    kTagStepMenu,
    kTagAlphabetMenu,
    kTagStartMenu,
    
    
    kTagStepMenuItem = 100,
    
    kTagAlphabetMenuItem = 200,
    
    kTagStartMenuItem = 300,
};


Debug_Index::Debug_Index():
_stepNum(0),
_alphabetNum(0)
{

}

Debug_Index::~Debug_Index()
{
}

Scene* Debug_Index::createScene()
{
    auto scene = Scene::create();

    auto layer = Debug_Index::create();

    scene->addChild(layer);

    return scene;
}

// on "init" you need to initialize your instance
bool Debug_Index::init()
{
    if ( !Base_Layer::init() )
    {
        return false;
    }
    
    auto bgcolor = Color4B(255, 255, 255, 255);
    this->initWithColor(bgcolor);
    
    initView();

    return true;
}


void Debug_Index::onEnter()
{
    Base_Layer::onEnter();
    
    onViewLoad();
}

void Debug_Index::onExit()
{
    Base_Layer::onExit();
    
}

void Debug_Index::onViewLoad()
{
    Base_Layer::onViewLoad();

    
}

void Debug_Index::onViewLoaded()
{
    Base_Layer::onViewLoaded();
    
    Director::getInstance()->getTextureCache()->removeUnusedTextures();
    Director::getInstance()->getTextureCache()->removeAllTextures();
}


#pragma mark - touch

bool Debug_Index::onTouchBegan(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
    if (m_touchEnabled == false) {
        return false;
    }
    

    return false;
}

void Debug_Index::onTouchMoved(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}

void Debug_Index::onTouchEnded(Touch *pTouch, Event *pEvent)
{
    Vec2 tp = pTouch->getLocation();
    
}



#pragma mark debug index

void Debug_Index::initView()
{
    this->createStepMenu();
    this->createStartMenu();
    this->createAlphabetMenu();
    
    this->setStepEnabled(0);
    this->setAlphabetEnabled(0);
}

void Debug_Index::createStepMenu()
{
    Vector<MenuItem*> arrayOfItems;
    
    for (int i = 0; i<3; i++)
    {
        int tag = kTagStepMenuItem + i;
        
        std::string normal = StringUtils::format("debug_btn_step%d_n.png", i+1);
        std::string select = StringUtils::format("debug_btn_step%d_s.png", i+1);
        
        auto item = MenuItemImage::create(getCommonFilePath("img", normal),
                                          getCommonFilePath("img", select),
                                          getCommonFilePath("img", select),
                                          CC_CALLBACK_1(Debug_Index::stepMenuCallback, this));
        
        item->setTag(tag);
        item->setAnchorPoint(Vec2::ZERO);
        Vec2 pos = Vec2(200 + (300 * (i)), item->getContentSize().height/2);
        item->setPosition(pos);
        
        arrayOfItems.pushBack(item);
    }
    
    auto menu = Menu::createWithArray(arrayOfItems);
    menu->setPosition(Vec2::ZERO);
    this->addChild(menu, kTagStepMenu, kTagStepMenu);
}

void Debug_Index::stepMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    int tag = item->getTag() - kTagStepMenuItem;
    
    this->setStepEnabled(tag);
}

void Debug_Index::setStepEnabled(int currentNum)
{
    Menu* menu = (Menu*)this->getChildByTag(kTagStepMenu);
    
    
    for (int i = 0; i < 3; i++)
    {
        MenuItem* item = (MenuItem*)menu->getChildByTag(kTagStepMenuItem + i);
        
        int stepNum = item->getTag()-kTagStepMenuItem;
        
        if (stepNum == currentNum)
        {
            item->setEnabled(false);
            _stepNum = stepNum+1;
        }
        else
        {
            item->setEnabled(true);
        }
    }
}


void Debug_Index::createAlphabetMenu()
{
    Vector<MenuItem*> arrayOfItems;
    
    for (int i = 0; i<26; i++)
    {
        int tag = kTagAlphabetMenuItem + i;
        
        std::string normal = StringUtils::format("debug_btn_alphabet%02d_n.png", i+1);
        std::string select = StringUtils::format("debug_btn_alphabet%02d_s.png", i+1);
        
        auto item = MenuItemImage::create(getCommonFilePath("img", normal),
                                          getCommonFilePath("img", select),
                                          getCommonFilePath("img", select),
                                          CC_CALLBACK_1(Debug_Index::alphabetMenuCallback, this));
        
        item->setTag(tag);
        
        int row = i/10;
        int col = i%10;
        
        
        Vec2 pos = Vec2(200 + (item->getContentSize().width*col + (col*40)), (winSize.height - 300) - 200*row);
        item->setAnchorPoint(Vec2::ZERO);
        item->setPosition(pos);
        
        arrayOfItems.pushBack(item);
    }
    
    auto menu = Menu::createWithArray(arrayOfItems);
    menu->setPosition(Vec2::ZERO);
    this->addChild(menu, kTagAlphabetMenu, kTagAlphabetMenu);
}

void Debug_Index::alphabetMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    int tag = item->getTag() - kTagAlphabetMenuItem;
    
    this->setAlphabetEnabled(tag);
}

void Debug_Index::setAlphabetEnabled(int currentNum)
{
    Menu* menu = (Menu*)this->getChildByTag(kTagAlphabetMenu);
    
    
    for (int i = 0; i < 26; i++)
    {
        MenuItem* item = (MenuItem*)menu->getChildByTag(kTagAlphabetMenuItem + i);
        
        int alphabetNum = item->getTag()-kTagAlphabetMenuItem;
        
        if (alphabetNum == currentNum)
        {
            item->setEnabled(false);
            _alphabetNum = alphabetNum+1;
        }
        else
        {
            item->setEnabled(true);
        }
    }
}

void Debug_Index::createStartMenu()
{
    int tag = kTagStartMenuItem;
    
    auto item = MenuItemImage::create(getCommonFilePath("img", "debug_btn_start_n.png"),
                                      getCommonFilePath("img", "debug_btn_start_n.png"),
                                      getCommonFilePath("img", "debug_btn_start_n.png"),
                                      CC_CALLBACK_1(Debug_Index::startMenuCallback, this));
    
    item->setTag(tag);
    item->setAnchorPoint(Vec2::ZERO);
    item->setPosition(Vec2(winSize.width - item->getContentSize().width*2, item->getContentSize().height/2));
    
    auto menu = Menu::create(item, nullptr);
    menu->setPosition(Vec2::ZERO);
    this->addChild(menu, kTagStartMenu, kTagStartMenu);
}

void Debug_Index::startMenuCallback(Ref* sender)
{
    MenuItem* item = (MenuItem*)sender;
    
    item->setEnabled(false);
    
    log("_alphabetNum:%d", _alphabetNum);
    
    std::string zipFileName = StringUtils::format("B01_APT_%02d.zip", _alphabetNum);
//    DRMManager::getInstance()->preload(zipFileName);
    
//    DRMManager::getInstance()->preload();
    
    int bookNum = 1;
    int dayNum = ProductManager::getInstance()->getCurrentDay();
    
    ProductManager::getInstance()->setProduct(bookNum, dayNum, _alphabetNum, _stepNum, 0);
    Director::getInstance()->replaceScene(ProductIndex::getContentScene());
}

