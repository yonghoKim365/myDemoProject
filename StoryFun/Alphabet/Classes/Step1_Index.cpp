
#include "Step1_Index.h"
#include "ProductManager.h"

#include "Step1_Type1.h"
#include "Step1_Type2.h"
#include "Step1_Type3.h"

Step1_Index::Step1_Index()
{

}

Step1_Index::~Step1_Index()
{
}

Scene* Step1_Index::getContentScene()
{
    Scene* scene;
    
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    if ( alphabetNum == 1 || alphabetNum == 9 || alphabetNum == 11 || alphabetNum == 15 || alphabetNum == 17 || alphabetNum == 19 || alphabetNum == 21 || alphabetNum == 25 )
    {
        ProductManager::getInstance()->setTypeNum(1);
        SCENE_FUNC(scene, Step1_Type1);
    }
    else if ( alphabetNum == 2 || alphabetNum == 6 || alphabetNum == 13 || alphabetNum == 14 || alphabetNum == 20 || alphabetNum == 23 || alphabetNum == 24 )
    {
        ProductManager::getInstance()->setTypeNum(2);
        SCENE_FUNC(scene, Step1_Type2);
    }
    else if ( alphabetNum == 3 || alphabetNum == 4 || alphabetNum == 5 || alphabetNum == 7 || alphabetNum == 8 || alphabetNum == 10 || alphabetNum == 12 || alphabetNum == 16 || alphabetNum == 18 || alphabetNum == 22 || alphabetNum == 26 )
    {
        ProductManager::getInstance()->setTypeNum(3);
        SCENE_FUNC(scene, Step1_Type3);
    }

    return scene;
}

void Step1_Index::runContentScene()
{
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    if ( alphabetNum == 1 || alphabetNum == 9 || alphabetNum == 11 || alphabetNum == 15 || alphabetNum == 17 || alphabetNum == 19 || alphabetNum == 21 || alphabetNum == 25 )
    {
        log("step1 === 1");
        ProductManager::getInstance()->setTypeNum(1);
        runSceneMacro(Step1_Type1);
    }
    else if ( alphabetNum == 2 || alphabetNum == 6 || alphabetNum == 13 || alphabetNum == 14 || alphabetNum == 20 || alphabetNum == 23 || alphabetNum == 24 )
    {
        log("step1 === 2");
        ProductManager::getInstance()->setTypeNum(2);
        runSceneMacro(Step1_Type2);
    }
    else if ( alphabetNum == 3 || alphabetNum == 4 || alphabetNum == 5 || alphabetNum == 7 || alphabetNum == 8 || alphabetNum == 10 || alphabetNum == 12 || alphabetNum == 16 || alphabetNum == 18 || alphabetNum == 22 || alphabetNum == 26 )
    {
        log("step1 === 3");
        ProductManager::getInstance()->setTypeNum(3);
        runSceneMacro(Step1_Type3);
    }
}