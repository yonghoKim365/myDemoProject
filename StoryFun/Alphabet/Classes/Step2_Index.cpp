
#include "Step2_Index.h"
#include "ProductManager.h"

#include "Step2_Type1.h"
#include "Step2_Type2.h"

Step2_Index::Step2_Index()
{

}

Step2_Index::~Step2_Index()
{
}

Scene* Step2_Index::getContentScene()
{
    Scene* scene;
    
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    if ( alphabetNum == 2 || alphabetNum == 4 || alphabetNum == 6 || alphabetNum == 8 || alphabetNum == 11 || alphabetNum == 12 || alphabetNum == 14 || alphabetNum == 16 || alphabetNum == 17 || alphabetNum == 18 || alphabetNum == 20 || alphabetNum == 21 || alphabetNum == 22 || alphabetNum == 23 )
    {
        ProductManager::getInstance()->setTypeNum(1);
        SCENE_FUNC(scene, Step2_Type1);
    }
    else if ( alphabetNum == 1 || alphabetNum == 3 || alphabetNum == 5 || alphabetNum == 7 || alphabetNum == 9 || alphabetNum == 10 || alphabetNum == 13 || alphabetNum == 15 || alphabetNum == 19 || alphabetNum == 24 || alphabetNum == 25 || alphabetNum == 26 )
    {
        ProductManager::getInstance()->setTypeNum(2);
        SCENE_FUNC(scene, Step2_Type2);
    }

    return scene;
}

void Step2_Index::runContentScene()
{
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    if ( alphabetNum == 2 || alphabetNum == 4 || alphabetNum == 6 || alphabetNum == 8 || alphabetNum == 11 || alphabetNum == 12 || alphabetNum == 14 || alphabetNum == 16 || alphabetNum == 17 || alphabetNum == 18 || alphabetNum == 20 || alphabetNum == 21 || alphabetNum == 22 || alphabetNum == 23 )
    {
        log("step2 === 1");
        ProductManager::getInstance()->setTypeNum(1);
        runSceneMacro(Step2_Type1);
    }
    else if ( alphabetNum == 1 || alphabetNum == 3 || alphabetNum == 5 || alphabetNum == 7 || alphabetNum == 9 || alphabetNum == 10 || alphabetNum == 13 || alphabetNum == 15 || alphabetNum == 19 || alphabetNum == 24 || alphabetNum == 25 || alphabetNum == 26 )
    {
        log("step2 === 2");
        ProductManager::getInstance()->setTypeNum(2);
        runSceneMacro(Step2_Type2);
    }
}