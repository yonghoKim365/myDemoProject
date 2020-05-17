//
//  ProductIndex.cpp
//
//
//
//
//

#include "ProductIndex.h"
#include "ProductManager.h"
#include "MSLPManager.h"
#include "Step1_Index.h"
#include "Step2_Index.h"
#include "Step3_Index.h"

ProductIndex::ProductIndex()
{

}

ProductIndex::~ProductIndex()
{
}

Scene* ProductIndex::getContentScene()
{
    ProductInfo* productInfo = ProductManager::getInstance()->getProductInfo();

    int stepNum = ProductManager::getInstance()->getCurrentStep();

    Scene* scene;

    if ( stepNum == 1 )
    {
        scene = Step1_Index::getContentScene();
    }
    else if ( stepNum == 2 )
    {
        scene = Step2_Index::getContentScene();
    }
    else if ( stepNum == 3 )
    {
        scene = Step3_Index::getContentScene();
    }

    return scene;
}


void ProductIndex::runContentScene()
{
    ProductInfo* productInfo = ProductManager::getInstance()->getProductInfo();

    int stepNum = ProductManager::getInstance()->getCurrentStep();


    if ( stepNum == 1 )
    {
        log("product index: step 1");
        Step1_Index::runContentScene();
    }
    else if ( stepNum == 2 )
    {
        log("product index: step 2");
        Step2_Index::runContentScene();
    }
    else if ( stepNum == 3 )
    {
        log("product index: step 3");
        Step3_Index::runContentScene();
    }
}
