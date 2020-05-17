//
//  SceneManager.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 7..
//
//

#ifndef __CapcomWorld__SceneManager__
#define __CapcomWorld__SceneManager__

#include <iostream>
//#include "MainScene.h"

class SceneManager
{
    
private:
    SceneManager() {
    }
    SceneManager(const SceneManager& other);
    ~SceneManager() {}
    static SceneManager* inst;
public:
    static SceneManager* getInstance(){
        if (inst == 0) inst = new SceneManager();
        return inst;
    }
    
//    MainScene *mainScene;
};

#endif /* defined(__CapcomWorld__SceneManager__) */
