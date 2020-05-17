//
//  ResponseLoginInfo.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 12..
//
//

#include "ResponseLoginInfo.h"


ResponseLoginInfo::ResponseLoginInfo()
{
    //user = new UserInfo();
    cardList = new CCArray();
    cardList->autorelease();

}

ResponseLoginInfo::~ResponseLoginInfo(){
    cardList =  NULL;
}