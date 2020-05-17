
#include "Step3_Index.h"
#include "ProductManager.h"

#include "Step3.h"

Step3_Index::Step3_Index()
{

}

Step3_Index::~Step3_Index()
{
}

Scene* Step3_Index::getContentScene()
{
    Scene* scene;
    
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    ProductManager::getInstance()->setTypeNum(1);
    SCENE_FUNC(scene, Step3);

    return scene;
}

void Step3_Index::runContentScene()
{
    int alphabetNum = ProductManager::getInstance()->getCurrentAlphabet();
    
    ProductManager::getInstance()->setTypeNum(1);
    runSceneMacro(Step3);
}