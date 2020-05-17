//
//  ProductIndex.h
//  
//
//
//
//

#ifndef __ProductIndex__
#define __ProductIndex__

#include "MGTLayer.h"

class ProductIndex
{
public:
    ProductIndex();
    virtual ~ProductIndex();
    
    static Scene* getContentScene();
    
    static void runContentScene();
};


#endif /* defined(__ProductIndex__) */
