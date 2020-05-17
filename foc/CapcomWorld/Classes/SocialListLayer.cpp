//
//  SocialListLayer.cpp
//  CapcomWorld
//
//  Created by Donghwa Lee on 12. 11. 5..
//
//

#include "SocialListLayer.h"
#include "MainScene.h"
#include "AKakaoUser.h"
#include "PopUp.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif

SocialListLayer::SocialListLayer(CCSize layerSize, int _cellKind) : MyFriendList(NULL), SMexcludeInfo(NULL)
{
    clip_start_y = 70;
    //this->setContentSize(layerSize);
    this->setClipsToBounds(true);
    
    //ARequestSender::getInstance()->requestFriendsToGameServer();
    
    cellKind = _cellKind;
    
    bTouchMove = false;
    
    cardMaker = new ACardMaker();
    
    //xb = new XBridge();
}

SocialListLayer::~SocialListLayer()
{
    this->unschedule(schedule_selector(SocialListLayer::MakeCells));
    this->removeAllChildrenWithCleanup(true);
    //cardMaker = NULL;
    CC_SAFE_DELETE(cardMaker);
    CC_SAFE_DELETE(sortedAppFriendList);
    //xb = NULL;
}

void SocialListLayer::InitLayerSize(CCSize layerSize)
{
    this->setContentSize(layerSize);
}

void SocialListLayer::InitFriendData()
{
 
    ////////////////////////////////////////////////////
    // for test
    /*
    for(int i=0;i<5;i++){
        FriendsInfo *finfo = new FriendsInfo();
        
        finfo->level = 1;
        finfo->ranking = 1;
        finfo->nickname = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        finfo->leaderCard = 30012;
        
        PlayerInfo::getInstance()->gameFriendsInfo->addObject(finfo);
        
        AKakaoUser *user = new AKakaoUser();
        user->nickname = "aaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbb";
        
        PlayerInfo::getInstance()->kakaoFriendsInfo->addObject(user);
        
    }
    */
    ////////////////////////////////////////////////////
    
    NumOfFriend = (cellKind == 1) ? PlayerInfo::getInstance()->kakaoFriendsInfo->count() : PlayerInfo::getInstance()->gameFriendsInfo->count();
    
    if(0 == cellKind){
        
        SMexcludeInfo =  ARequestSender::getInstance()->requestSMExclude();
        
        // sort by world ranking
        CCArray *tempList = new CCArray();
        
        CCLog(" num of game friend :%d ", PlayerInfo::getInstance()->gameFriendsInfo->count());
        
        for(int i=0;i<PlayerInfo::getInstance()->gameFriendsInfo->count();i++){
            FriendsInfo *info = (FriendsInfo*)PlayerInfo::getInstance()->gameFriendsInfo->objectAtIndex(i);
            FriendsInfo *finfo = new FriendsInfo();
            
            finfo->userID = info->userID;
            finfo->level = info->level;
            finfo->ranking = info->ranking;
            finfo->nickname = info->nickname;
            finfo->leaderCard = info->leaderCard;
            finfo->attack = info->attack;
            finfo->defense = info->defense;
            finfo->profileURL = info->profileURL;
            
            //CCLog("nickname : %s", finfo->nickname.c_str());
            
            if (finfo->ranking == 0)finfo->ranking = 999998;
            
            tempList->addObject(finfo);
        }
        
        // add myself
        if (NumOfFriend > 0){
            FriendsInfo *info = new FriendsInfo();
            info->userID = PlayerInfo::getInstance()->userID;
            info->level = PlayerInfo::getInstance()->getLevel();
            info->nickname = PlayerInfo::getInstance()->myNickname;
            CardInfo* leadercard = PlayerInfo::getInstance()->GetCardInDeck(1, 0, 2);
            if (leadercard != NULL){
                info->leaderCard = leadercard->getId();
                info->attack = leadercard->getAttack();
                info->defense = leadercard->getDefence();
            }
            info->ranking = PlayerInfo::getInstance()->getRanking();
            if (info->ranking == 0)info->ranking = 999998;
            
            info->profileURL =PlayerInfo::getInstance()->profileImageUrl;
            tempList->addObject(info);
            //CCLog("my url:%s",info->profileURL.c_str());
            
            NumOfFriend++;
        }
        
        sortedAppFriendList = new CCArray();
        //sortedAppFriendList->autorelease();
        int list_length = tempList->count();
        for(int k=0;k<list_length;k++){
            
            int min_value = 999999;
            int min_index = -1;
            
            for(int i=0;i<tempList->count();i++){
                FriendsInfo *info = (FriendsInfo*)tempList->objectAtIndex(i);
                
                if (info->ranking < min_value){
                    min_value = info->ranking;
                    min_index = i;
                }
            }
            
            FriendsInfo *info = (FriendsInfo*)tempList->objectAtIndex(min_index);
            
            sortedAppFriendList->addObject(info);
            //CCLog("sort, info->ranking:%d",info->ranking);
            tempList->removeObject(info);
        }
        tempList->autorelease();
        
    }
    
    
}

int SocialListLayer::GetNumOfFriend()
{
    //return MyFriendList->count();
    return NumOfFriend;
}

void SocialListLayer::InitUI()
{
    StartYpos = this->getContentSize().height - 80;
    if (cellKind==1){
        StartYpos = this->getContentSize().height - accp(SocialInviteLayer::HELP_TEXT_ZONE_HEIGHT);
    }
    
    
    
    
    //ARequestSender::getInstance()->requestFriendsToGameServer();
    /*
    for(int i=0;i<PlayerInfo::getInstance()->gameFriendsInfo->count();i++)
    {
        FriendsInfo *info = (FriendsInfo*)PlayerInfo::getInstance()->gameFriendsInfo->objectAtIndex(i);
        info->profileURL = PlayerInfo::getInstance()->getGameUserProfileURL(info->userID);
        CCLog("player id :%lld", info->userID);
        CCLog("player nick :%s", info->nickname);
        CCLog(" player lev :%d", info->level);
        CCLog(" player ranking:%d", info->ranking);
    }
    */

    //NumOfFriend = PlayerInfo::getInstance()->gameFriendsInfo->count();
    
    //addPageLoading();
    
    cellMakeCnt = 0;
    cellMakeCnt2 = 0;
    this->schedule(schedule_selector(SocialListLayer::MakeCells),0.1);
    
    //if (cellKind == 1){
    //    CCLog(" num of kakao friend :%d",PlayerInfo::getInstance()->kakaoFriendsInfo->count());
    //}
    
    
}

void SocialListLayer::setDisableInviteBtn(long long userID)
{
    for(int i=0;i<PlayerInfo::getInstance()->kakaoFriendsInfo->count();i++)
    {
        AKakaoUser *user = (AKakaoUser*)PlayerInfo::getInstance()->kakaoFriendsInfo->objectAtIndex(i);
        if (user->userID == userID){
            
            CCPoint p1 = this->getChildByTag(990+i)->getPosition();
            CCPoint p2 = this->getChildByTag(1990+i)->getPosition();
            this->removeChildByTag(990+i,true);
            this->removeChildByTag(1990+i,true);
            
            CCSprite* pVisit = CCSprite::create("ui/card_tab/page_btn1.png");
            pVisit->setAnchorPoint(ccp(0, 0));
            pVisit->setScaleX(1.5f);
            pVisit->setScaleY(2.05f);
            pVisit->setPosition(p1);
            this->addChild(pVisit, 60);
            
            CCLabelTTF* pVisit1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_invite"), "HelveticaNeue-Bold", 12);
            pVisit1->setColor(COLOR_DARK_GRAY);
            registerLabel( this,ccp(0, 0.5), p2, pVisit1, 60);
            
            break;
        }
    }
}

void SocialListLayer::MakeCells()
{
    //this->unschedule(schedule_selector(SocialListLayer::MakeCells));
    
    if (cellKind == 1)
    {
        if (PlayerInfo::getInstance()->kakaoFriendsInfo == NULL)
            return;
        
        if (cellMakeCnt2 == NumOfFriend){
            this->unschedule(schedule_selector(SocialListLayer::MakeCells));
            //removePageLoading();
            
            if (NumOfFriend == 0){
                
                CCSize size = GameConst::WIN_SIZE;
                
                CCLabelTTF* pNoFriend = CCLabelTTF::create("초대할 친구가 없습니다.", "HelveticaNeue-Bold", 12);
                pNoFriend->setColor(COLOR_YELLOW);
                registerLabel( this,ccp(0.5, 0.5), ccp(size.width/2, 0), pNoFriend, 60);
            }
            
            return;
        }
        
        int scan = cellMakeCnt2;
        AKakaoUser *user = (AKakaoUser*)PlayerInfo::getInstance()->kakaoFriendsInfo->objectAtIndex(scan);
        if (user != NULL){
            
            if (user->profileImageUrl.length() > 0)
            {
                PortraitUrl *portrait = new PortraitUrl;
                //portrait->url = user->profileImageUrl;
                portrait->url = FileManager::sharedFileManager()->getUserProfileFilename(user->profileImageUrl);
                portrait->y = StartYpos + 16;//41;
                portraitDic.setObject(portrait, portrait->url);//user->profileImageUrl);
                portrait->autorelease();
            }
        
            bool bBlockInvite = false;
            
            //if (user->bMessageBlocked || PlayerInfo::getInstance()->isEnableSendMedal(user->userID)==false){
            if (PlayerInfo::getInstance()->isEnableSendMedal(user->userID)==false){
                bBlockInvite = true;
            }
                
            MakeInviteCell(user->profileImageUrl, user->nickname, bBlockInvite,  scan);
            StartYpos -= (SOCIAL_INVITE_HEIGHT + 10);

            cellMakeCnt2++;
        }
    }
    else{
        
        if (cellMakeCnt == sortedAppFriendList->count()){
            
            this->unschedule(schedule_selector(SocialListLayer::MakeCells));
            
            if (PlayerInfo::getInstance()->gameFriendsInfo->count() == 0){
                
                CCSize size = GameConst::WIN_SIZE;
                
                CCLabelTTF* pNoFriend = CCLabelTTF::create("추가된 친구가 없습니다.", "HelveticaNeue-Bold", 12);
                pNoFriend->setColor(COLOR_YELLOW);
                registerLabel( this,ccp(0.5, 0.5), ccp(size.width/2, -20), pNoFriend, 60);
                
            }
            return;
        }
    
        int i = cellMakeCnt;
        FriendsInfo *info = (FriendsInfo*)sortedAppFriendList->objectAtIndex(i);
        if (info->profileURL.length() > 0)
        {
            PortraitUrl *portrait = new PortraitUrl;
            portrait->url = FileManager::sharedFileManager()->getUserProfileFilename(info->profileURL);
            portrait->y = StartYpos + 41;
            portraitDic.setObject(portrait, portrait->url);//info->profileURL);
        }
        
        bool sendMedal = true;
        
        if(SMexcludeInfo->userlist)
        {
            for(vector<long long>::iterator itor = SMexcludeInfo->userlist->begin(); itor != SMexcludeInfo->userlist->end() ; ++itor)
            {
                long long _userId = *itor;

                if (info->userID == _userId){
                    sendMedal=false;
                    break;
                }
            }
        }
        
        if (info->userID == PlayerInfo::getInstance()->userID)sendMedal=false; // cannot send to myself
        
        MakeMedalCell(info, i, sendMedal);
        StartYpos -= (SOCIAL_MEDAL_HEIGHT + 10); // 144;//185;
        
        cellMakeCnt++;
        
    }
    
    
    //removePageLoading();
    
}


void SocialListLayer::MakeMedalCell(FriendsInfo *info, int tag, bool sendMedal)
{
    //CCLog("메이크 셀");
    
    float cell_y = StartYpos;
   
    CCSprite* pSprListBg = CCSprite::create("ui/home/home_social_listbg.png");
    pSprListBg->setAnchorPoint(ccp(0, 0));
    pSprListBg->setPosition(accp(10, cell_y));
    this->addChild(pSprListBg, 60);
    
    

    cell_y += 41;
    
    std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(info->profileURL);// imgPath);
        
    if (filename.size() > 0){
        if (FileManager::sharedFileManager()->IsProfileFileExist(filename.c_str())){
            registerProfileImg(filename);
        }
        else{
            ProfileYPos = cell_y;
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            std::vector<std::string> downloads;
            downloads.push_back(info->profileURL);//imgPath);
            requestor->addDownloadTask(downloads, this, callfuncND_selector(SocialListLayer::callProfileImg));
        }
    }
    
    if (info->leaderCard !=0){ // leaderCard != 0){
        CardInfo *ci = CardDictionary::sharedCardDictionary()->getInfo(info->leaderCard);// leaderCard);
        
        if (ci != NULL){
            
            ci->setAttack(info->attack);
            ci->setDefence(info->defense);
            
            //cardMaker->MakeCardThumb(this, ci, ccp(122,cell_y-29), 130, 100, 10);
            
            CardThumbLayer *thumbLayer = new CardThumbLayer(ci, ccp(122,cell_y-29), 130);
            thumbLayer->setDelegate(this);
            thumbLayer->setTag(9000+tag);
            this->addChild(thumbLayer,1000);
        }
    }
    
    CCLabelTTF* pNickName1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_nickname"), "HelveticaNeue", 12);
    pNickName1->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(130+90, cell_y + 45), pNickName1, 60);
    
    // 한글자당 25
    //CCLabelTTF* pNickName2 = CCLabelTTF::create(Name.c_str(), "HelveticaNeue-Bold", 12);
    CCLabelTTF* pNickName2 = CCLabelTTF::create(info->nickname.c_str(), "HelveticaNeue-Bold", 12);
    pNickName2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(205+90, cell_y + 45), pNickName2, 60);
    
    
    CCSprite* pSprListBg2 = CCSprite::create("ui/home/home_social_listbg.png", CCRectMake(500/SCREEN_ZOOM_RATE,0,120/SCREEN_ZOOM_RATE,156/SCREEN_ZOOM_RATE));
    pSprListBg2->setAnchorPoint(ccp(0,0));
    pSprListBg2->setPosition(accp(10+500, cell_y-41));
    this->addChild(pSprListBg2, 60);
    
    
    /*
    CCLabelTTF* pCode1 = CCLabelTTF::create("코드", "Thonburi", 13);
    pCode1->setColor(COLOR_YELLOW);
    registerLabel( this,ccp(0, 0), accp(130, StartYpos + 10), pCode1, 60);
    
    CCLabelTTF* pCode2 = CCLabelTTF::create(Code.c_str(), "Thonburi", 13);
    pCode2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(180, StartYpos + 10), pCode2, 60);
    */
    
    if(sendMedal)
    {
        CCSprite* pVisit = CCSprite::create("ui/shop/spin_m_btn_send1.png");
        pVisit->setAnchorPoint(ccp(0, 0));
        pVisit->setPosition(accp(530, cell_y-15+20));
        pVisit->setTag(990+tag);
        this->addChild(pVisit, 60);
    }
    else
    {
        CCSprite* pVisit = CCSprite::create("ui/shop/spin_m_btn_lock2.png");
        pVisit->setAnchorPoint(ccp(0, 0));
        pVisit->setPosition(accp(530, cell_y-15+20));
        //pVisit->setTag(990+tag);
        this->addChild(pVisit, 60);            
    }

    CCLabelTTF* pRanking1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_battle_ranking"), "HelveticaNeue", 12);
    pRanking1->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0), accp(130+90, cell_y - 25 + 20), pRanking1, 60);
    
    // ranking 이 0 이면 랭킹이 없는것임. 999998로 설정하여 최하위로 만든다. 
    char buff[10];
    //if (Ranking == 999998){
    if (info->ranking == 999998){
        sprintf(buff, "%s", "-");
    }
    else{
        sprintf(buff, "%d", info->ranking);
    }
    CCLabelTTF* pRanking2 = CCLabelTTF::create(buff, "HelveticaNeue-Bold", 12);
    pRanking2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0), accp(230+90, cell_y - 25 + 20), pRanking2, 60);

    if (info->level>= 10){
        CCSprite* number1 = createNumber(info->level / 10, accp(80-10, cell_y - 25 + 20), 1.0f);
        CCSprite* number0 = createNumber(info->level % 10, accp(80+10-5, cell_y - 25 + 20), 1.0f);
        this->addChild(number1, 500);
        this->addChild(number0, 500);
    }
    else{
        //CCSprite* number = maker->createNumber(level, accp(80, cell_y - 25 + 20), 1.0f);
        CCSprite* number = createNumber(info->level, accp(80, cell_y - 25 + 20), 1.0f);
        this->addChild(number, 500);
    }
    //maker->autorelease();

    
}

void SocialListLayer::MakeInviteCell(const string& imgPath, const string& Name, bool bMsgBlock, int tag)
{
    
    float cell_y = StartYpos;
    
    CCSprite* pSprListBg = CCSprite::create("ui/home/friend_list_bg.png");
    pSprListBg->setAnchorPoint(ccp(0, 0));
    pSprListBg->setPosition(accp(25, cell_y));
    this->addChild(pSprListBg, 60);
    
    std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(imgPath);
        
    if (filename.size() > 0){
        if (FileManager::sharedFileManager()->IsProfileFileExist(filename.c_str())){
            registerProfileImg(filename);
        }
        else{
            ProfileYPos = cell_y;
            CCHttpRequest *requestor = CCHttpRequest::sharedHttpRequest();
            std::vector<std::string> downloads;
            downloads.push_back(imgPath);
            requestor->addDownloadTask(downloads, this, callfuncND_selector(SocialListLayer::callProfileImg));
        }
    }
    
    CCLabelTTF* pNickName1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_nickname"), "HelveticaNeue", 12);
    pNickName1->setColor(COLOR_ORANGE);
    registerLabel( this,ccp(0, 0.5), accp(140, cell_y + 80), pNickName1, 60);
    
    // 한글자당 25
    CCLabelTTF* pNickName2 = CCLabelTTF::create(Name.c_str(), "HelveticaNeue-Bold", 12);
    pNickName2->setColor(COLOR_WHITE);
    registerLabel( this,ccp(0, 0.5), accp(215, cell_y + 80), pNickName2, 60);
    
    CCSprite* pSprListBg2 = CCSprite::create("ui/home/friend_list_bg.png", CCRectMake(468/SCREEN_ZOOM_RATE,0,120/SCREEN_ZOOM_RATE,110/SCREEN_ZOOM_RATE));
    pSprListBg2->setAnchorPoint(ccp(0, 0));
    pSprListBg2->setPosition(accp(25+468, cell_y));
    this->addChild(pSprListBg2, 60);
    
    if (bMsgBlock==false){
        CCSprite* pVisit = CCSprite::create("ui/card_tab/card_list_btn.png");
        pVisit->setAnchorPoint(ccp(0, 0));
        pVisit->setPosition(accp(530-20, cell_y+14));
        pVisit->setTag(990+tag);
        this->addChild(pVisit, 60);
        
        CCLabelTTF* pVisit1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_invite"), "HelveticaNeue-Bold", 12);
        pVisit1->setColor(COLOR_YELLOW);
        pVisit1->setTag(1990+tag);
        registerLabel( this,ccp(0, 0.5), accp(555-20, cell_y + 54), pVisit1, 60);
    }
    else{
        CCSprite* pVisit = CCSprite::create("ui/card_tab/page_btn1.png");
        pVisit->setAnchorPoint(ccp(0, 0));
        pVisit->setScaleX(1.5f);
        pVisit->setScaleY(2.05f);
        pVisit->setPosition(accp(530-20, cell_y+14));
        //pVisit->setTag(990+tag);
        this->addChild(pVisit, 60);
        
        CCLabelTTF* pVisit1 = CCLabelTTF::create(LocalizationManager::getInstance()->get("social_list_cell_invite"), "HelveticaNeue-Bold", 12);
        pVisit1->setColor(COLOR_DARK_GRAY);
        registerLabel( this,ccp(0, 0.5), accp(555-20, cell_y + 54), pVisit1, 60);
    }
    
}

void SocialListLayer::callProfileImg(cocos2d::CCObject *pSender, void *data)
{
    HttpResponsePacket *response = (HttpResponsePacket *)data;
    
    if(response->request->reqType == kHttpRequestDownloadFile)
    {
        if (response->succeed) {
            std::vector<std::string>::iterator iter;
            for (iter = response->request->files.begin(); iter != response->request->files.end(); ++iter) {
                std::string url = *iter;
                
                std::string filename = FileManager::sharedFileManager()->getUserProfileFilename(url);
                
                registerProfileImg(filename);
                
            }
        }
        else {
            
        }
    }
}

void SocialListLayer::registerProfileImg(std::string filename)
{
    std::string DocumentPath = CCFileUtils::sharedFileUtils()->getDocumentPath() + filename;//url.substr(found+1);
    CCSize size = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    CCSprite* pSprFace = CCSprite::create(DocumentPath.c_str());
    
    //PortraitUrl *portrait = (PortraitUrl*)portraitDic.objectForKey(url.c_str());
    PortraitUrl *portrait = (PortraitUrl*)portraitDic.objectForKey(filename);
    
    if (pSprFace != NULL && portrait)
    {
        CCSize aa = pSprFace->getTexture()->getContentSizeInPixels();
        float cardScale = (float)78 / aa.height;
        pSprFace->setScale(cardScale);
        if (cellKind == 0){
            regSprite( this, ccp(0, 0), accp(26, portrait->y + 20), pSprFace, 1200);
        }
        else{
            regSprite( this, ccp(0, 0), accp(40, portrait->y), pSprFace, 1200);
        }
    }
    
}

void SocialListLayer::InitMedalPopUp(void *data)
{
    this->setTouchEnabled(false);
    if (cellKind == 0){
        this->setThumbTouch(false);
    }
    
    basePopUp = new SocialPopUp();
    basePopUp->InitUI(data);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    basePopUp->setPosition(accp(0.0f, -this->getPositionY() *  SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 3000);
}

void SocialListLayer::InitInvitePopUp(AKakaoUser *data)
{
    this->setTouchEnabled(false);
    
    basePopUp = new SocialPopUp();
    basePopUp->InitUI2(data);
    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
    
    basePopUp->setPosition(accp(0.0f, -this->getPositionY() * SCREEN_ZOOM_RATE));
    
    this->addChild(basePopUp, 3000);

}

void SocialListLayer::RemovePopUp()
{
    this->setTouchEnabled(true);
    if (cellKind == 0){
        this->setThumbTouch(true);
    }
    
    if(basePopUp)
    {
        this->removeChild(basePopUp, true);
        basePopUp = NULL;
    }
}


void SocialListLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    if (NumOfFriend == 0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    touchStartPoint = location;
    bTouchPressed = true;

    moving = false;
    startPosition = location;
    CCTime::gettimeofdayCocos2d(&startTime, NULL);
}

void SocialListLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    if (NumOfFriend == 0)return;
    
    //: 좌표를 가져올 임의 터치를 추출합니다.
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI 좌표를 GL좌표로 변경합니다
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);

    float y = this->getPositionY();

#if (1)
    if (moving == true)
    {
        float distance = startPosition.y - location.y;
        cc_timeval endTime;
        CCTime::gettimeofdayCocos2d(&endTime, NULL);
        long msec = endTime.tv_usec - startTime.tv_usec;
        float timeDelta = msec / 1000 + (endTime.tv_sec - startTime.tv_sec) * 1000.0f;
        float endPos;// = -(localPoint.y + distance * timeDelta / 10);
        float velocity = distance / timeDelta / 10;
        endPos = getPositionY() - velocity * 3500.f;
        if (endPos > 0)
            endPos = 0;
        else if (endPos < LayerStartPos)
            endPos = LayerStartPos;
        CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.6f, ccp(0, endPos)), 2.0f);
        CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(SocialListLayer::scrollingEnd));
        this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
    }
#endif
    if(LayerStartPos>0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(SocialListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        
        if (y > LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(SocialListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }

    if(LayerStartPos<0)
    {
        if (y < LayerStartPos)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, LayerStartPos)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(SocialListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
        
        
        if (y > 0)
        {
            CCEaseOut *fadeOut = CCEaseOut::actionWithAction(CCMoveTo::actionWithDuration(0.3f, ccp(0, 0)), 10.0f);
            CCCallFunc *callBack = CCCallFunc::actionWithTarget(this, callfunc_selector(SocialListLayer::scrollingEnd));
            this->runAction(CCSequence::actionOneTwo(fadeOut, callBack));
        }
    }
    
    if(false == bTouchMove)
    {
        
        for(int i=0; i<NumOfFriend; ++i)
        {
            if (GetSpriteTouchCheckByTag(this, 990+i, localPoint))
            {
                soundButton1();
                
                CCLog(" btn:%d",i);
                
                if (cellKind == 0)
                {
                    // -- 메달 보내기 팝업
                    
                    UserMedalInfo* info = new UserMedalInfo();
                    //info->user = (FriendsInfo*)PlayerInfo::getInstance()->gameFriendsInfo->objectAtIndex(i);
                    info->user = (FriendsInfo*)sortedAppFriendList->objectAtIndex(i);
                    info->tag = 990+i;
                    
                    InitMedalPopUp(info);
                    //info->autorelease(); // 해당 팝업에서 직접 삭제하게 수정.
                    
                }
                else if (cellKind == 1)
                {
                    // -- 초대메세지 팝업
                    AKakaoUser *user = (AKakaoUser*)PlayerInfo::getInstance()->kakaoFriendsInfo->objectAtIndex(i);
                    
                    InitInvitePopUp(user);
                }
            }
        }
    }
    
    bTouchMove = false;
}

//message request failed with Error Domain=KAErrorDomain Code=-500 "Invalid Receiver : Receiver doesn't exist." UserInfo=0x2312a5a0 {NSLocalizedDescription=Invalid Receiver : Receiver doesn't exist.

void SocialListLayer::ccTouchesMoved(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    if (MainScene::getInstance()->popupCnt>0)return;
    
    if (NumOfFriend == 0)return;
    
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    float distance = fabs(startPosition.y - location.y);
    cc_timeval currentTime;
    CCTime::gettimeofdayCocos2d(&currentTime, NULL);
    float timeDelta = (currentTime.tv_usec - startTime.tv_usec) / 1000.0f + (currentTime.tv_sec - startTime.tv_sec) * 1000.0f;
    //printf("moving distance:%f timeDelta: %f\n", distance, timeDelta);
    if (distance < 15.0f && timeDelta > 50.0f)
    {
        moving = false;
        startPosition = location;
        startTime = currentTime;
    }
    else if (distance > 5.0f)
    {
        moving = true;
        bTouchMove = true;
    }
    
    if (moving)
    {
        distance = fabs(lastPosition.y - location.y);
        timeDelta = (currentTime.tv_usec - lastTime.tv_usec) / 1000.0f + (currentTime.tv_sec - lastTime.tv_sec) * 1000.0f;
        if (distance < 15.0f && timeDelta > 50.0f)
        {
            moving = false;
            startPosition = location;
            startTime = currentTime;
        }
    }
    
    lastPosition = location;
    lastTime = currentTime;

    if (touchStartPoint.y != location.y)
    {
        this->setPositionY(this->getPosition().y + (location.y-touchStartPoint.y));
        touchStartPoint.y = location.y;
        
        //CCLog("소셜 레이어 좌표 %f, %f",this->getPosition().x, this->getPosition().y);
    }
    
    CCPoint movingVector;
    movingVector.x = location.x - touchMovePoint.x;
    movingVector.y = location.y - touchMovePoint.y;
    
    touchMovePoint = location;    
}

void SocialListLayer::scrollingEnd()
{
    this->stopAllActions();
}

void SocialListLayer::visit()
{
    CCSize winSize = GameConst::WIN_SIZE;//CCDirector::sharedDirector()->getWinSize();
    int clip_y = accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT);
    int clip_h = winSize.height - accp(MAIN_LAYER_BTN_HEIGHT + CARDS_LAYER_BTN_HEIGHT) - accp(MAIN_LAYER_TOP_UI_HEIGHT + MAIN_LAYER_TOP_MARGIN + SOCIAL_INVITE_FRIEND_BAR_H + SOCIAL_HELP_TEXT_ZONE_H);
    
    if (cellKind == 1){
        clip_h -= accp(SocialInviteLayer::HELP_TEXT_ZONE_HEIGHT);
        clip_h += accp(SOCIAL_HELP_TEXT_ZONE_H);
    }
    
    if (this->getClipsToBounds()){
        CCRect scissorRect = CCRect(0, clip_y, winSize.width-accp(10), clip_h);
        
        glEnable(GL_SCISSOR_TEST);
        
        CCEGLView::sharedOpenGLView()->setScissorInPoints(scissorRect.origin.x,scissorRect.origin.y,scissorRect.size.width,scissorRect.size.height);
    }
    
    CCNode::visit();

    if (this->getClipsToBounds()){
        glDisable(GL_SCISSOR_TEST);
    }
}


void SocialListLayer::beforeOpenDetailView()
{
    this->setTouchEnabled(false);
    this->setThumbTouch(false);
    SocialLayer::getInstance()->setTouchEnabled(false);
    // 친구를 초대하세요 및 다른 thumbLayer막아야 함.
}
void SocialListLayer::afterCloseDetailView()
{
    this->schedule(schedule_selector(SocialListLayer::setEnableTouchAfterDelay),0.3f);
    
}

void SocialListLayer::setThumbTouch(bool b)
{
    if (sortedAppFriendList == NULL)return;
    if (sortedAppFriendList->count() == 0)return;
    
        for (int i=0;i<sortedAppFriendList->count();i++){
            CardThumbLayer *thumbLayer = (CardThumbLayer*)this->getChildByTag(9000+i);
            if (thumbLayer != NULL){
                thumbLayer->setTouchEnabled(b);
                thumbLayer = NULL;
            }
        }
    
}

void SocialListLayer::setEnableTouchAfterDelay()
{
    this->setTouchEnabled(true);
    setThumbTouch(true);
    this->unschedule(schedule_selector(SocialListLayer::setEnableTouchAfterDelay));
    SocialLayer::getInstance()->setTouchEnabled(true);
    
}

