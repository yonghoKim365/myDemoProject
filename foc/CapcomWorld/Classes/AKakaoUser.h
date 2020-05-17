//
//  AKakaoUser.h
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 14..
//
//

#ifndef CapcomWorld_AKakaoUser_h
#define CapcomWorld_AKakaoUser_h

class AKakaoUser : public cocos2d::CCObject
{
public:
    long long userID;
    
    //const char *nickname;
    std::string nickname;
    
    std::string profileImageUrl;
    
    const char *friendsNickName; // 내가 설정한 친구 닉네임
    
    bool bMessageBlocked;           // 메시지 차단 여부
    
};




#endif
