
using System.Collections.Generic;

// DATA TYPE
public class P_Hero
{
	public string name;
	public Dictionary<string, string> selEqts;
}

public class P_ChampionResult
{
	public string result;
	public Dictionary<string, int> defenceHeroState; // 0 : DIE, 1: LIVE
	public Dictionary<string, int> attackHeroState; // 0 : DIE, 1: LIVE
}

public class P_Champion
{
	public int memberId;
	public string userId;
	public string nickname;
	public string imageUrl;
	public int showPhoto;
	public int score;
	public int rank;
	public int attackWin;
	public int attackLose;
	public int defenceWin;
	public int defenceLose;
	public Dictionary<string, P_ChampionResult> attackRounds;
	public Dictionary<string, P_ChampionResult> defenceRounds;
	public int coolTime;
	public int revengeCoolTime;
}

public class P_DefenceRecord
{
	public string attackerId; 	// USERID
	public int rank;
	public int score;
	public string round;
	public int didDefend;		// 1:YES, 0:NO
	public int date;
	public string nickname;
	public string imageUrl;
	public int showPhoto;
}

public class P_UserRank
{
	public int rank;
	public string userId;
	public int score;
	public string nickname;
	public string imageUrl;		// IF URL IS NULL, SHOW DUMMY IMAGE
}

public class P_FriendRank
{
	public int rank;
	public string userId;
	public int score;
}

public class P_FriendData
{
	public string userId;
	public int league;
	public string nickname;
	public int blockMessage;
	public int fpWaitingTime;
	public int receivedFP; 		// 1:YES, 0:NO
	public int fLevel;
	public int fExpGauge;
	public int maxAct;
	public int maxStage;
	public int pvpReward;		// EXP Reward.
}

public class P_FriendRewardRow
{
	public int league;
	public int energy1;
	public int energy3;
	public string rune1;
	public string rune3;
	public int ruby1;
	public int ruby3;
	public int gold1;
	public int gold3;
}

public class P_Product
{
	public string id;
	public string subcategory;
	public string title;
	public int count;
	public string priceType;	// 'GOLD', 'RUBY', 'MONEY'
	public float price;
	public string desc;
	public string imgUrl;
	public string revision;
}

public class P_Package
{
	public string id;
	public string category;
	public string subcategory;
	public int showWeight;
	public string title;
	public string priceType;
	public float price;
	public int buyCount;
	public string[] showViews; 	// 'LOBBY', 'SHOP', 'WORLD'
	public string startDate;	// 20141025
	public string endDate;		// 20141111
	public string imageUrl;
	public string desc;
	public string revision;
}

// 한번에 구매 상품을 받지 않고 연금처럼 여러번에 나눠서 받는 상품.
public class P_Annuity
{
	public string annuityId;	// OS 구분 없이 사용하기 위한 연금 상품 아이디
	public string productId;	// 구매시 필요한 상품 아이디, OS마다 다른 아이디가 존재한다.
	public string title;
	public string desc;
	public string priceType;
	public float price;
	public string[] product;	// ex) RUBY_30, SKILL_S, UNIT_A, ITEM_B,
	public int didBuy;			// YES:1, NO:0
	public int didConsume;		// YES:1, NO:0
	public string endDate;
	public int remainedTime;
	public string imageUrl;
	public string revision;
}

public class P_Shop
{
	public Dictionary<string, P_Product> energyProducts;
	public Dictionary<string, P_Product> itemProducts;
	public Dictionary<string, P_Product> goldProducts;
	public Dictionary<string, P_Product> rubyProducts;
	public Dictionary<string, P_Product> giftProducts;
}

public class P_Jackpot
{
	public string userId;
	public string nickname;
	public string itemId;
	public string product;
	public int date;
}

public class P_Message
{
	public string id;
	public string type;		// 'INVITE_REWARD', 'GIFT'
	public string senderId;	// 'admin', '88xxxxxxxxxxxxxxxx'
	public string[] itemList;
	public string text;
	public int receivedTime;
	public int expiredTime;
	public int isNew;
	public int reservedI;
	public string reservedS;
	public string[] reservedL;
	public Dictionary<string, string> options;
}

public class P_Reward
{
	public string code;			// 'ENERGY', 'GOLD', 'RUBY', 'FRPNT', 'GACHA', 'ITEM'
	public int count;
	public string gacha;
	public string itemId;
}

public class P_Mission
{
	public string id;
	public string kind;
	public string category;
	public int state;			// 0:DOING, 1:CLEAR, 2:CLOSE
	public int visible;			// YES:1, NO:0
	public string title;
	public P_Reward[] rewards;
	public int openDate;
	public int closeDate;
	public string clearVariable;
	public string clearOperator;
	public string clearConstant;
	public string desc;
}

public class P_MissionNotification
{
	public string id;
	public int state; 			// 0:DOING, 1:CLEAR, 2:CLOSE
	public string title;
	public string desc;
	public string category;
	public P_Reward[] rewards;
}

public class P_Reinforce
{
	public int nx;	// NEED EXP
	public int gx;	// GIVE EXP
	public int rp;	// REINFORCE PRICE
	public int sp;	// SELL PRICE
}

public class P_Popup
{
	public string type;
	public string size;
	public string title;
	public string text;
	public string image;
	public int delay;

	public string buttonType;
	public string eventId;
	public string eventValue;
	public string actionType;
	public string actionData;
	public Dictionary<string, string> options;
}

/*
 * 팝업 메세지
 * type => 출력 형식, 'TXT'=>텍스트, 'IMG'=>이미지, 'PRD'=>상품구매(특가)
   - 다만 P_Event내의 eventPopup으로 호출 된 경우에는 다른 타입들을 가지고 있는다.
 * size => 'S', 'M', 'L', 'WXH', nullable : 텍스트와 이미지 팝업일 경우에만 사용, 값이 S/M/L이 아닐 경우 디폴트 M으로 처리. 커스텀예시(600*400) => '600X400'
 * title => 팝업 타이틀, nullable
   - NOTICE : 상단 이미지 공지사항
   - GIFTBOX : 상단 이미지 선물상자
   - GOLD,RUBY : 각각의 이미지
 * delay => 팝업 노출시간:버튼 노출 시간 딜레이(초 단위)
 * buttonType => 버튼 출력 형식 액션:타입[,액션:타입], nullable (예외는 OK:OK로 처리)
   - 타입은 OK, CLOSE, ACTION 3종류만 있음
   - 모양은 정하는 대로 많음(OK,CLOSE,DETAIL,CANCEL,YES,NO,HELP,PURCHASE,GIVEUP,REPLAY 등등...)
   - 예1) OK:DETAIL,CLOSE:CANCEL
   - 예2) OK:YES,CLOSE:NO
   - 예3) CLOSE:CLOSE
   - 예4) OK:OK
   - 예5) OK:CLOSE
   - 예6) ACTION:DETAIL => action을 수행=>OK와 동일, 다만 팝업을 닫지는 않음
 * eventId => null이 아니고, 확인 버튼 누를경우 ToS_EVENT호출(eventVal 값을 참조), nullbable
 * actionType => 확인 버튼 누를경우, 아래 행동을 취함, nullable
   - REVIEW : 리뷰 팝업으로 이동
   - GAMEUI : 게임에 있는 특정 UI혹은 팝업 노출(actionData값 및 options 값을 참조)
   - LINK : actionData의 링크로 이동
   - WEME : actionData의 링크로 이동(위미 웹 팝업 사용)
   - SENDMSG : 카카오톡으로 메세지 전송(actionData값 참조) actionData => to|text|templateId(생략가능)|image(생략가능)|param(생략가능)
   - RESTART : 클라이언트 재시작
   - LOGOUT : 카카오톡 혹은 Device인증 로그아웃
   - EXIT : 클라이언트 종료(안드로이드만)
   - PAYMENT : 캐시 상품을 구매
   - PURCHASE : 그외 모든 상품 구매(ToS_PURCHASE_PRODUCT 프로토콜로 구매)
 * options
   - OFF_CODE : 동일한 코드의 팝업을 하루동안 다시 보지 않기 기능(클라이언트에서 구현)
   - MOVE_TO : 팝업 순서를 지정 'TAIL'=>맨뒤, 맨마지막, 'HEAD'=>맨앞, 맨처음
   - ON_SHOW : 팝업 노출시 실행할 Action 현재는 T_POPUP_ACTION::UNREGIST 만 적용되어 있음
   - ON_CLOSE : 팝업 닫을때 실행할 Action 현재는 T_POPUP_ACTION::CLIENT_EXIT 만 적용되어 있음
   - 각종 커스텀 UI데이터를 여기에 실어서 보낼 수 있으면 좋을듯
*/

public class P_Attend
{
	public string title;
	public string desc;
	public int today;
	public int closeDate;							// 20141231, 출석 테이블 유효 날짜. 현재 기획에서는 사용하지 않음.
	public Dictionary<string, string> rewardTable; 	// Dictionary<day, reward> reward[RUBY_N, GOLD_N, ENERGY_N, ATTEND_X_Y, X => U(소환수),S(스킬룬),E(장비), Y => S, A, B, C, D]
}

public class P_Sigong
{
	public string id;
	public string roundId;
	public string category;
	public string title;
	public string desc;
	public int enterMax;
	public int enterCount;
	public int enterCoolTime;
	public int enterWaitingTime;
	public string enterDepId;		// ENTER 종속 아이디
	public int clearMax;
	public int clearCount;
	public int closingTime;
	public string priceType;
	public int price;
	public string bgm;
	public string map;
	public string cutscene;
	public int autoPlay;
	public string forcedDeck;
	public string handicap;
	public Dictionary<string, int> rewards;
}

public class To_CMD
{
	public string cmd;
}

public class ToS_AUTH : To_CMD
{
	public string userId;
	public string authToken;
	public string version; 	// X.XX
	public string deviceName;
	public string os;			// 'A':AOS, 'I':IOS
	public string osInfo;
	public string macAddress;
	public string appId;
}

public class ToC_AUTH : To_CMD
{
	public string resourceUrl;
	public string gameUrl;
	public string accessToken;
}

// BASE
public class ToS_BASE : To_CMD
{
	public string userId;
	public string accessToken;
}

public class ToC_BASE : To_CMD
{
	public Dictionary<string, P_Mission> C_Missions;
	public Dictionary <string, P_MissionNotification> C_MissionNotifications;
	public P_Shop C_Shop;
	public Dictionary<string, P_Message> C_Messages;
	public int C_ChampStatus; 	// 1:OPEN, 2:CLOSE
	public int C_ChampDefence; 	// 1:YES, 0:NO
	public Dictionary<string, P_Package> C_Packages;
	public Dictionary<string, P_Popup> C_Popups;
}

// ERROR
public class ToC_ERROR : To_CMD
{
	public int action; 		// 0:NONE, 1:ALERT, 2:STORE, 3:START, 4:LOGIN, 5:LOGOUT, 6:UNREGISTER
	public string message;
}

public class ToS_INIT : ToS_BASE
{
	public string imageUrl;
	public string[] friendIds;
}

public class ToC_INIT : ToC_BASE
{
	public string nickname;
	public int energy;
	public Dictionary<string, int> maxEnergies;
	public int chargeTime;
	public int friendPoint;
	public int gold;
	public int ruby;
	public int rstone;
	public int blockMessage;	// 1:YES, 0:NO
	public string selectHeroId;
	public string selectSubHeroId;
	public Dictionary<string, P_Hero> heroes;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectSkillRunes;
	
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Kiley;

	public Dictionary<string, string> selectUnitRunes_Chloe;
	public Dictionary<string, string> selectSkillRunes_Chloe;

	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;
	public int maxAct;
	public int maxStage;
	public int maxRound;
	public int champLeague;		// 1:BRONZE, 2:SILVER, 3:GOLD, 4:MASTER, 5:PLATINUM, 6:LEGEND
	public int champGroupId;
	public int champMemberId; 	// -1:NOT JOINING IN CHAMPIONSHIP
	public int inviteCount;
	public int completeTutorial;
	public Dictionary<string, int> tutorialHistory;
	public int lastPlayStatus; 	// 0:NONE, 1:EPIC, 2:HELL, 3:CHAMPIONSHIP, 4:FRIENDPVP
	public int goldForRuby;
	public string hellSave;
	public int serviceMode; 	// 0:NORMAL, 1:IOS SUBMIT
	public int timestamp; 		// Unix Time Stamp
	public P_Attend attendEvent;
	public Dictionary<string, P_Annuity> annuityProducts;
	public Dictionary<string, int> heroPrices;
}

// GET REINFORCE, COMPOSE DATA
public class ToS_GET_RECOM : ToS_BASE
{
}

public class ToC_GET_RECOM
{
	public Dictionary<string, P_Reinforce> eqtReinforce;
	public Dictionary<string, P_Reinforce> skillReinforce;
	public Dictionary<string, P_Reinforce> unitReinforce;
	public Dictionary<string, int> composePrices;
	public Dictionary<string, int> evolvePrices;
	public Dictionary<string, int> reforgePrices;
}

public class ToS_SET_NICKNAME : ToS_BASE
{
	public string nickname;
}

public class ToC_SET_NICKNAME : ToC_BASE
{
	public string nickname;
}

public class ToS_GET_TIMESTAMP : ToS_BASE
{
}

public class ToC_GET_TIMESTAMP
{
	public int timestamp; 		// Unix Time Stamp
}

public class ToS_CHEAT_LOG : ToS_BASE
{
	public string type;			// ROOTING, CHEAT_APP
	public string extra;		// Extra Data
}

public class ToC_CHEAT_LOG
{
}

// EVENT
// 위미 플랫폼뷰를 통해 유저가 액션한 이후에 이벤트 관련 정보를 갱신하기 위한 목적.
// 갱신된 정보는 서버에서 팝업이나 미션 클리어 알림으로 통보한다.
// 바로 보상이 있더라고 이벤트는 모두 메시지함으로 통해 지급한다.
public class ToS_REFRESH_EVENT : ToS_BASE
{
	public string eventId;
}

public class ToC_REFRESH_EVENT : ToC_BASE
{
	public Dictionary<string, string> resultData;
}

// 리뷰 이벤트처럼 유저 액션에 반응(서버에서 체크 불가)하는 이벤트 용도.
public class ToS_ACTION_EVENT : ToS_BASE
{
	public string eventId;
	public Dictionary<string, string> eventData;
}

public class ToC_ACTION_EVENT : ToC_BASE
{
	public Dictionary<string, string> resultData;
}

public class ToS_COUPON_URL : ToS_BASE
{
}

public class ToC_COUPON_URL
{
	public string url;
}

public class ToS_EVENT_URL : ToS_BASE
{
}

public class ToC_EVENT_URL
{
	public string url;
}

// EPIC MODE
public class ToS_ENERGY_TICK : ToS_BASE
{
}

public class ToC_ENERGY_TICK : ToC_BASE
{
	public int energy;
	public int chargeTime;
}

public class ToS_PLAY_EPIC : ToS_BASE
{
	public int act;
	public int stage;
	public int round;
	public string tutorialId;
}

public class ToC_PLAY_EPIC : ToC_BASE
{
	public int energy;
	public int chargeTime;
}

public class ToS_FINISH_EPIC : ToS_BASE
{
	public int isSuccess;	//	0:LOSE, 1:WIN, 2:GIVE UP, 3:NOT CONTINUE
	public Dictionary<string, int> playData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public int timestamp;
	public int diff;
}

public class ToC_FINISH_EPIC : ToC_BASE
{
	public int gold;
	public int maxAct;
	public int maxStage;
	public int maxRound;

	public string[] roundItems;
	public Dictionary<string, P_Hero> heroes;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectUnitRunes;

	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Kiley;

	public Dictionary<string, string> selectUnitRunes_Chloe;
	public Dictionary<string, string> selectSkillRunes_Chloe;

	// STAGE CLEAR
	public string stageItem;
	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;
}

// HELL MODE
public class ToS_PREPARE_HELL : ToS_BASE
{
}

public class ToC_PREPARE_HELL : ToC_BASE
{
	public int remainedTime;
	public int maxTicket;
	public int hellTicket;
	public int dScore;
	public int dWave;
	public int dMonster;
	public int dDistance;
	public int wRank;
	public float wGroup;
	public int wScore;
	public int wWave;
	public int wMonster;
	public int wDistance;
	public int tRank;
	public int tScore;
	public int tWave;
	public int tMonster;
	public int tDistance;
	public Dictionary<string, P_FriendRank> fRankData;	// FRIEND RANK
	public int fRank;	// FRIEND RANK
}

public class ToS_GET_HELL_FRIEND_RANK : ToS_BASE
{
	public string start;
}

public class ToC_GET_HELL_FRIEND_RANK : ToC_BASE
{
	public Dictionary<string, P_FriendRank> fRankData;
}

public class ToS_GET_HELL_USER_RANK : ToS_BASE
{
	public string start;
}

public class ToC_GET_HELL_USER_RANK : ToC_BASE
{
	public Dictionary<string, P_UserRank> uRankData;
}

public class ToS_GET_HELL_USER_INFO : ToS_BASE
{
	public string hellUserId;
}

public class ToC_GET_HELL_USER_INFO : ToC_BASE
{
	public int league;
	public int act;
	public P_Hero hero;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectSkillRunes;

	public P_Hero subHero;
	public Dictionary<string, string> selectSubUnitRunes;
	public Dictionary<string, string> selectSubSkillRunes;
}

public class ToS_PLAY_HELL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_PLAY_HELL : ToC_BASE
{
	public int hellTicket;
}

public class ToS_SAVE_HELL : ToS_BASE
{
	public string hellSave;
}

public class ToC_SAVE_HELL : ToC_BASE
{
}

public class ToS_FINISH_HELL : ToS_BASE
{
	public int result;	// 0:HERO DEATH, 1:TIME OVER, 2:GIVE UP, 3:NOT CONTINUE, 4:CLEAR
	public int wave;
	public int score;
	public int monster;
	public int distance;
	public Dictionary<string, int> playData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public int timestamp;
	public int diff;
}

public class ToC_FINISH_HELL : ToC_BASE
{
	// REWARD
	public int gold;
	public string[] rewardItems;
	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;

	// REFRESH SCORE
	public int dScore;
	public int dWave;
	public int dMonster;
	public int dDistance;
	public int wRank;
	public int wScore;
	public int wWave;
	public int wMonster;
	public int wDistance;
	public float wGroup;
	public int tRank;
	public int tScore;
	public int tWave;
	public int tMonster;
	public int tDistance;
	public Dictionary<string, P_FriendRank> fRankData;	// FRIEND RANK
	public int fRank;	// FRIEND RANK
}

// CHAMPIONSHIP
public class ToS_REJOIN_CHAMPIONSHIP : ToS_BASE
{
}

public class ToC_REJOIN_CHAMPIONSHIP : ToC_BASE
{
	public int champGroupId;
	public int champMemberId; // -1:NOT JOINING IN CHAMPIONSHIP
}

public class ToS_CHAMPIONSHIP_DATA : ToS_BASE
{
}

public class ToC_CHAMPIONSHIP_DATA : ToC_BASE
{
	public Dictionary<string, P_Champion> champions;
	public int remainedTime;
	public int reattackRuby;
	public P_DefenceRecord[] defenceRecords;
}

public class ToS_LAST_CHAMPIONSHIP_DATA : ToS_BASE
{
}

public class ToC_LAST_CHAMPIONSHIP_DATA : ToC_BASE
{
	public int week;
	public int league;
	public Dictionary<string, P_Champion> lastChampions;
}

public class ToS_ENEMY_DATA : ToS_BASE
{
	public string enemyId;
}

public class ToC_ENEMY_DATA : ToC_BASE
{
	public int act;
	public P_Hero eHero;
	public Dictionary<string, string> eSelectUnitRunes;
	public Dictionary<string, string> eSelectSkillRunes;

	public P_Hero eSubHero;
	public Dictionary<string, string> eSubSelectUnitRunes;
	public Dictionary<string, string> eSubSelectSkillRunes;
}

public class ToS_PLAY_PVP : ToS_BASE
{
	public string enemyId;
	public string round; // 'R0', 'R1', 'R2'
	public string aiId;

	public string tutorialId;
}

public class ToC_PLAY_PVP : ToC_BASE
{
	public int ruby;
	// ENEMY DATA
	public P_Hero eHero;
	public Dictionary<string, string> eSelectUnitRunes;
	public Dictionary<string, string> eSelectSkillRunes;

	public P_Hero eSubHero;
	public Dictionary<string, string> eSelectUnitRunes_Sub;
	public Dictionary<string, string> eSelectSkillRunes_Sub;
}

public class ToS_FINISH_PVP : ToS_BASE
{
	public int didWin;	// 0:LOSE, 1:WIN, 2:GIVE UP, 3:NOT CONTINUE
	public string replayData;
	public Dictionary<string, int> playData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public Dictionary<string, int> playSubData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public Dictionary<string, int> myHeroState; // 0 : DIE, 1: LIVE
	public Dictionary<string, int> eHeroState; // 0 : DIE, 1: LIVE
	public int timestamp;
	public int diff;
}

public class ToC_FINISH_PVP : ToC_BASE
{
	public int gold;
	public int score;  
	public int didwin; // 0:LOSE, 1:WIN, 2:GIVE UP, 3:NOT CONTINUE
	public Dictionary<string, P_Champion> champions;
}

public class ToS_GET_REPLAY : ToS_BASE
{
	public int isMyAttack;
	public string enemyId;
	public string round; // 'R0', 'R1', 'R2'
}

public class ToC_GET_REPLAY : ToC_BASE
{
	public string replayData;
	public string aiId;
	// ME
	public P_Hero hero;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectSkillRunes;

	public P_Hero subHero;
	public Dictionary<string, string> selectSubUnitRunes;
	public Dictionary<string, string> selectSubSkillRunes; 

	// ENEMY DATA
	public P_Hero eHero;
	public Dictionary<string, string> eSelectUnitRunes;
	public Dictionary<string, string> eSelectSkillRunes;

	public P_Hero eSubHero;
	public Dictionary<string, string> eSubSelectUnitRunes;
	public Dictionary<string, string> eSubSelectSkillRunes;
}

// SIGONG MODE
public class ToS_PREPARE_SIGONG : ToS_BASE
{
}

public class ToC_PREPARE_SIGONG : ToC_BASE
{
	public Dictionary<string, P_Sigong> sigongList;
}

public class ToS_PLAY_SIGONG : ToS_BASE
{
	public string sigongId;
}

public class ToC_PLAY_SIGONG : ToC_BASE
{
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	public int rstone;
}

public class ToS_FINISH_SIGONG : ToS_BASE
{
	public int result;	//	0:LOSE, 1:WIN, 2:GIVE UP
	public Dictionary<string, int> playData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public int timestamp;
	public int diff;
	public string usedHeroId;
}

public class ToC_FINISH_SIGONG : ToC_BASE
{
	public string[] rewardItems;
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	public int rstone;

	public Dictionary<string, P_Hero> heroes;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectUnitRunes;

	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	
	public Dictionary<string, string> selectUnitRunes_Chloe;
	public Dictionary<string, string> selectSkillRunes_Chloe;


	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;

	public Dictionary<string, P_Sigong> sigongList;
}

// 게임 비정상 종료 후 업데이트되서 이어하기가 불가능한 경우 게임 무효화한다.
public class ToS_INVALIDATE_GAME : ToS_BASE
{
}

public class ToC_INVALIDATE_GAME : ToC_BASE
{
}

// HERO
public class ToS_CHANGE_HERO : ToS_BASE
{
	public string heroId;
	public string subHeroId;
}

public class ToC_CHANGE_HERO : ToC_BASE
{
	public string heroId;
	public string subHeroId;
}

public class ToS_BUY_HERO : ToS_BASE
{
	public string heroId;
}

public class ToC_BUY_HERO : ToC_BASE
{
	public int ruby;
	public string selectHeroId;
	public string selectSubHeroId;
	public Dictionary<string, P_Hero> heroes;
}

public class ToS_CHANGE_EQUIPMENT : ToS_BASE
{
	public string equipmentId;
}

public class ToC_CHANGE_EQUIPMENT : ToC_BASE
{
	public Dictionary<string, P_Hero> heroes;
	public string[] equipments;
}

public class ToS_SELL_EQUIPMENT : ToS_BASE
{
	public string[] equipmentIds;
}

public class ToC_SELL_EQUIPMENT : ToC_BASE
{
	public string[] equipments;
	public int gold;
}

public class ToS_REFORGE_RUNE : ToS_BASE
{
	public string heroId;
	public string runeId;
	public string sourceId;
	public int didLoad;
}

public class ToC_REFORGE_RUNE : ToC_BASE
{
	public string newRuneId;
	public Dictionary<string, P_Hero> heroes;
	public Dictionary<string, string> selectRunes;
	public Dictionary<string, string> selectRunes_Kiley;
	public Dictionary<string, string> selectRunes_Chloe;
	public string[] runes;
	public int gold;
	public int ruby;
}

public class ToS_REINFORCE_EQUIPMENT : ToS_BASE
{
	public string equipmentId;
	public string[] sourceIds;
	public int didLoad; // 1:YES, 0:NO

	public string tutorialId;
}

public class ToC_REINFORCE_EQUIPMENT : ToC_BASE
{
	public string newEquipmentId;
	public Dictionary<string, P_Hero> heroes;
	public string[] equipments;
	public int gold;
	public int ruby;
}

public class ToS_COMPOSE_EQUIPMENT : ToS_BASE
{
	public string equipmentId;
	public string sourceId;
	public int didLoad; // 1:YES, 0:NO

	public string tutorialId;
}

public class ToC_COMPOSE_EQUIPMENT : ToC_BASE
{
	public string newEquipmentId;
	public Dictionary<string, P_Hero> heroes;
	public string[] equipments;
	public int gold;
	public int ruby;
}

public class ToS_EVOLVE_EQUIPMENT : ToS_BASE
{
	public string equipmentId;
	public int didLoad; // 1:YES, 0:NO
}

public class ToC_EVOLVE_EQUIPMENT : ToC_BASE
{
	public string newEquipmentId;
	public Dictionary<string, P_Hero> heroes;
	public string[] equipments;
	public int gold;
	public int ruby;
	public int rstone;
}

public class ToS_EQUIPMENT_HISTORY : ToS_BASE
{
	public string heroId;	// 'LEO', 'KILEY'
}

public class ToC_EQUIPMENT_HISTORY : ToC_BASE
{
	public Dictionary<string, int> historyEquipments;
}

// SKILLRUNE
public class ToS_CHANGE_SKILLRUNE : ToS_BASE
{
	public string heroId;
	public string slot;
	public string skillRuneId;
}

public class ToC_CHANGE_SKILLRUNE : ToC_BASE
{
	public string[] skillRunes;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Chloe;
}

public class ToS_UNLOAD_SKILLRUNE : ToS_BASE
{
	public string heroId;
	public string slot;
}

public class ToC_UNLOAD_SKILLRUNE : ToC_BASE
{
	public string[] skillRunes;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Chloe;
}

public class ToS_SELL_SKILLRUNE : ToS_BASE
{
	public string[] skillRuneIds;
}

public class ToC_SELL_SKILLRUNE : ToC_BASE
{
	public int gold;
	public string[] skillRunes;
}

public class ToS_REINFORCE_SKILLRUNE : ToS_BASE
{
	public string skillRuneId;
	public string[] sourceIds;
	public int didLoad; // 1:YES, 0:NO
	public string heroId;

	public string tutorialId;
}

public class ToC_REINFORCE_SKILLRUNE : ToC_BASE
{
	public string newSkillRuneId;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Chloe;
	public string[] skillRunes;
	public int gold;
	public int ruby;
}

public class ToS_COMPOSE_SKILLRUNE : ToS_BASE
{
	public string skillRuneId;
	public string sourceId;
	public string heroId;
	public int didLoad; // 1:YES, 0:NO

	public string tutorialId;
}

public class ToC_COMPOSE_SKILLRUNE : ToC_BASE
{
	public string newSkillRuneId;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Chloe;
	public string[] skillRunes;
	public int gold;
	public int ruby;
}

public class ToS_EVOLVE_SKILLRUNE : ToS_BASE
{
	public string skillRuneId;
	public string heroId;
	public int didLoad; // 1:YES, 0:NO
}

public class ToC_EVOLVE_SKILLRUNE : ToC_BASE
{
	public string newSkillRuneId;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectSkillRunes_Kiley;
	public Dictionary<string, string> selectSkillRunes_Chloe;
	public string[] skillRunes;
	public int gold;
	public int ruby;
	public int rstone;
}

public class ToS_SKILLRUNE_HISTORY : ToS_BASE
{
}

public class ToC_SKILLRUNE_HISTORY : ToC_BASE
{
	public Dictionary<string, int> historySkillRunes;
}

// UNITRUNE
public class ToS_CHANGE_UNITRUNE : ToS_BASE
{
	public string heroId;
	public string slot;
	public string unitRuneId;
}

public class ToC_CHANGE_UNITRUNE : ToC_BASE
{
	public string[] unitRunes;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectUnitRunes_Chloe;
}

public class ToS_UNLOAD_UNITRUNE : ToS_BASE
{
	public string heroId;
	public string slot;
}

public class ToC_UNLOAD_UNITRUNE : ToC_BASE
{
	public string[] unitRunes;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectUnitRunes_Chloe;
}

public class ToS_SELL_UNITRUNE : ToS_BASE
{
	public string[] unitRuneIds;
}

public class ToC_SELL_UNITRUNE : ToC_BASE
{
	public int gold;
	public string[] unitRunes;
}

public class ToS_REINFORCE_UNITRUNE : ToS_BASE
{
	public string unitRuneId;
	public string[] sourceIds;
	public int didLoad; // 1:YES, 0:NO
	public string heroId;

	public string tutorialId;
}

public class ToC_REINFORCE_UNITRUNE : ToC_BASE
{
	public string newUnitRuneId;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectUnitRunes_Chloe;
	public string[] unitRunes;
	public int gold;
	public int ruby;
}

public class ToS_COMPOSE_UNITRUNE : ToS_BASE
{
	public string unitRuneId;
	public string sourceId;
	public int didLoad; // 1:YES, 0:NO
	public string heroId;

	public string tutorialId;
}

public class ToC_COMPOSE_UNITRUNE : ToC_BASE
{
	public string newUnitRuneId;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectUnitRunes_Chloe;
	public string[] unitRunes;
	public int gold;
	public int ruby;
}

public class ToS_EVOLVE_UNITRUNE : ToS_BASE
{
	public string unitRuneId;
	public int didLoad; // 1:YES, 0:NO
	public string heroId;
}

public class ToC_EVOLVE_UNITRUNE : ToC_BASE
{
	public string newUnitRuneId;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectUnitRunes_Kiley;
	public Dictionary<string, string> selectUnitRunes_Chloe;
	public string[] unitRunes;
	public int gold;
	public int ruby;
	public int rstone;
}

public class ToS_UNITRUNE_HISTORY : ToS_BASE
{
}

public class ToC_UNITRUNE_HISTORY : ToC_BASE
{
	public Dictionary<string, int> historyUnitRunes;
}

// MESSAGE
public class ToS_MESSAGE_LIST : ToS_BASE
{
}

public class ToC_MESSAGE_LIST : ToC_BASE
{
}

public class ToS_CONFIRM_MESSAGE : ToS_BASE
{
	public string messageId;
}

public class ToC_CONFIRM_MESSAGE : ToC_BASE
{
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	public int rstone;
	public int friendPoint;
	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;
}

// FRIEND
public class ToS_GET_FRIENDS : ToS_BASE
{
	public int needReward;	// 1:YES, 0:NO
}

public class ToC_GET_FRIENDS : ToC_BASE
{
	public Dictionary<string, P_FriendData> friendDatas;
	public int slotMachinePrice;
	public Dictionary<string, P_Reward> slotRewardList;
	public string[] fpvpIds;
	public int fpvpMax;
	public int inviteCount;
	public Dictionary<string, int> inviteReward;
	public Dictionary<string, P_FriendRewardRow> fRewardTable;
}

public class ToS_GET_FRIEND_DETAIL : ToS_BASE
{
	public string friendId;
}

public class ToC_GET_FRIEND_DETAIL
{
	public int act;
	public string nickname;
	public P_Hero selectHero;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectUnitRunes;

	public P_Hero selectSubHero;
	public Dictionary<string, string> selectSubSkillRunes;
	public Dictionary<string, string> selectSubUnitRunes;
}

public class ToS_SEND_FRIEND_POINT : ToS_BASE
{
	public string friendId;
	public int withMessage;	// 1:YES, 0:NO
}

public class ToC_SEND_FRIEND_POINT : ToC_BASE
{
	public int friendPoint;
	public int fpWaitingTime;
	public int fLevel;
	public int fExpGauge;
}

public class ToS_RECEIVE_FRIEND_POINT : ToS_BASE
{
	public string friendId;
}

public class ToC_RECEIVE_FRIEND_POINT : ToC_BASE
{
	public int friendPoint;
	public int fLevel;
	public int fExpGauge;
}

public class ToS_CHECK_SEND_FP_TIME : ToS_BASE
{
	public string friendId;
}

public class ToC_CHECK_SEND_FP_TIME : ToC_BASE
{
	public int fpWaitingTime;
}

public class ToS_INVITE_FRIEND : ToS_BASE
{
	public string friendId;
}

public class ToC_INVITE_FRIEND : ToC_BASE
{
	public int friendPoint;
	public int inviteCount;
	public string rewardCode;  // null:NO REWARD,
}

public class ToS_INVITE_FRIEND_LIST : ToS_BASE
{
}

public class ToC_INVITE_FRIEND_LIST : ToC_BASE
{
	public string[] inviteFriendIds;
}

public class ToS_PLAY_FRIEND_PVP : ToS_BASE
{
	public string friendId;
}

public class ToC_PLAY_FRIEND_PVP : ToC_BASE
{
	// FRIEND DATA
	public P_Hero fHero;
	public Dictionary<string, string> fSelectUnitRunes;
	public Dictionary<string, string> fSelectSkillRunes;

	public P_Hero fSubHero;
	public Dictionary<string, string> fSubSelectUnitRunes;
	public Dictionary<string, string> fSubSelectSkillRunes;

	public string[] fpvpIds;
}

public class ToS_FINISH_FRIEND_PVP : ToS_BASE
{
	public int didWin;	// 0:LOSE, 1:WIN, 2:GIVE UP, 3:NOT CONTINUE
	public Dictionary<string, int> playData; // Key : PERIOD, U1, U2, U3, U4, U5, S1, S2, S3
	public Dictionary<string, int> playSubData;
	public int timestamp;
	public int diff;
}

public class ToC_FINISH_FRIEND_PVP : ToC_BASE
{
	public string[] fpvpIds;
	public int gold;
}

public class ToS_PULL_SLOT_MACHINE : ToS_BASE
{
	public string tutorialId;
}

public class ToC_PULL_SLOT_MACHINE : ToC_BASE
{
	public int slotMachinePrice;
	public string rewardId;
	public int friendPoint;
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	// FOR RUNE'S REWARD
	public string rewardItem;
	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;
}

// MISSION
public class ToS_MISSION_DATA : ToS_BASE
{
}

public class ToC_MISSION_DATA : ToC_BASE
{
}

public class ToS_REQUEST_MISSION_REWARD : ToS_BASE
{
	public string missionId;
}

public class ToC_REQUEST_MISSION_REWARD : ToC_BASE
{
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	public int rstone;
	public int friendPoint;
	// ITEM
	public string[] gachaRewards;
	public string[] equipments;
	public string[] unitRunes;
	public string[] skillRunes;
}

// SHOP
public class ToS_GET_JACKPOT_USERS : ToS_BASE
{
}

public class ToC_GET_JACKPOT_USERS
{
	public Dictionary<string, P_Jackpot> jackpots;
}

// 게임 유저 데이터 가져오기, 잭팟 유저 조회에 사용.
public class ToS_GET_OTHER_DATA : ToS_BASE
{
	public string otherId;
}

public class ToC_GET_OTHER_DATA
{
	public int act;
	public int league;
	public string nickname;
	public string imageUrl;
	public P_Hero hero;
	public Dictionary<string, string> selectSkillRunes;
	public Dictionary<string, string> selectUnitRunes;

	public P_Hero subHero;
	public Dictionary<string, string> selectSubSkillRunes;
	public Dictionary<string, string> selectSubUnitRunes;
}

// 현금 결제 상품은 다른 상품들과 달리 취소가 불가능하다(이미 결제 완료 후에 게임 서버에 지급 요청).
// 구매 전에 미리 리비전, 상품아이디를 확인하자.
public class ToS_CHECK_PRODUCT : ToS_BASE
{
	public string type;			// 'SHOP', 'PACKAGE' 패키지는 서버에서 발동시키기 때문에 불필요한듯. 일단 타입 넣자.;;
	public string productId;
	public string revision;
}

public class ToC_CHECK_PRODUCT : ToC_BASE
{
	public int result;	// -1:REFRESH, 0:FAIL, 1:SUCCESS
	public string message;
}

public class ToS_BUY_ENERGY : ToS_BASE
{
	public string revision;
	public string productId;
}

public class ToC_BUY_ENERGY : ToC_BASE
{
	public int result;	// -1:REFRESH, 0:FAIL, 1:SUCCESS
	public string message;
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;
	public string[] newItems;
	public string[] equipments;
	public string[] skillRunes;
	public string[] unitRunes;
}

public class ToS_BUY_GOLD : ToS_BASE
{
	public string revision;
	public string productId;
}

public class ToC_BUY_GOLD : ToC_BASE
{
	public int result;	// -1:REFRESH, 0:FAIL, 1:SUCCESS
	public string message;
	public int gold;
	public int ruby;
}

public class ToS_BUY_ITEM : ToS_BASE
{
	public string revision;
	public string productId;

	public string tutorialId;
}

public class ToC_BUY_ITEM : ToC_BASE
{
	public int result;	// -1:REFRESH, 0:FAIL, 1:SUCCESS
	public string message;
	public int gold;
	public int ruby;
	public int rstone;
	public string[] newItems;
	public string[] equipments;
	public string[] skillRunes;
	public string[] unitRunes;
}

public class ToS_BUY_BY_MONEY : ToS_BASE
{
	public string revision;		// 현금 결제 실패시 이후 클라이언트에서 결제 재시도를 한다. 그때는 상품아이디, 리비젼을 보장할 수 없기 때문에 모두 null 전송하고 서버도 체크하지 않는다.
	public string productId;
	public string signedData;
	public string signature;
	public string transactionData;
}

public class ToC_BUY_BY_MONEY : ToC_BASE
{
	public int result;			// -1:REFRESH, 0:FAIL, 1:SUCCESS
	public string paymentId;	// result와 관계 없이 paymentId 있으면 consumed 처리
	public string message;		// FAIL 시에 에러 메시지, 선물하기는 성공시에도 별도 메시지 올 수 있다.

	public int ruby;
	public string productId;

	public Dictionary<string, P_Annuity> annuityProducts;
}

public class ToS_BUY_ERROR : ToS_BASE
{
	public string os;			// 'A', 'I'
	public string productId;
	public string error;
	public string desc;
}

public class ToC_BUY_ERROR
{
}

public class ToS_CONSUME_ANNUITY : ToS_BASE
{
	public string annuityId;
}

public class ToC_CONSUME_ANNUITY : ToC_BASE
{
	public Dictionary<string, P_Annuity> annuityProducts;
}

public class ToS_REFRESH_ANNUITY : ToS_BASE
{
}

public class ToC_REFRESH_ANNUITY : ToC_BASE
{
	public Dictionary<string, P_Annuity> annuityProducts;
}

public class ToS_REFRESH_SHOP : ToS_BASE
{

}

public class ToC_REFRESH_SHOP : ToC_BASE
{
	
}

// SETTINGS
public class ToS_GET_SETTINGS : ToS_BASE
{
}

public class ToC_GET_SETTINGS : ToC_BASE
{
	public int showPhoto;
	public int blockMessage;
}

public class ToS_SET_SETTINGS : ToS_BASE
{
	public int showPhoto;
	public int blockMessage;
}

public class ToC_SET_SETTINGS : ToC_BASE
{
	public int showPhoto;
	public int blockMessage;
}


// MIGRATE
public class ToS_MIGRATE_KAKAOID : ToS_BASE
{
	public string kakaoId;
	public string authToken;
	public int didConfirm; 		// 1:YES, 0:NO
}

public class ToC_MIGRATE_KAKAOID
{
	public int result;			// 0:FAIL, 1:SUCCESS, 2:SHOULD_CONFIRM
	public string message;
}

// TUTORIAL
public class ToS_PREPARE_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_PREPARE_TUTORIAL
{
	public int exp;
	public int energy;
	public int gold;
	public int ruby;
}

public class ToS_START_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_START_TUTORIAL : ToC_BASE
{
}

public class ToS_COMPLETE_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_COMPLETE_TUTORIAL : ToC_BASE
{
	public Dictionary<string, int> tutorialHistory;
}

public class ToS_REWARD_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_REWARD_TUTORIAL : ToC_BASE
{
	public int getEnergy;
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;

	// CHANGED INFO AFTER ADDING EXP
	public P_Hero hero;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectSkillRunes;
}

public class ToS_COMPLETE_REWARD_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_COMPLETE_REWARD_TUTORIAL : ToC_BASE
{
	public Dictionary<string, int> tutorialHistory;
	public int getEnergy;
	public int energy;
	public int chargeTime;
	public int gold;
	public int ruby;

	// CHANGED INFO AFTER ADDING EXP
	public P_Hero hero;
	public Dictionary<string, string> selectUnitRunes;
	public Dictionary<string, string> selectSkillRunes;
}

public class ToS_INVALIDATE_TUTORIAL : ToS_BASE
{
	public string tutorialId;
}

public class ToC_INVALIDATE_TUTORIAL : ToC_BASE
{
	public Dictionary<string, int> tutorialHistory;
}

public class ToS_COMPLETE_ALL_TUTORIAL : ToS_BASE
{
}

public class ToC_COMPLETE_ALL_TUTORIAL : ToC_BASE
{
	public Dictionary<string, int> tutorialHistory;
}


// FOR DEVELOPMENT
public class ToS_SET_ENERGY : ToS_BASE
{
	public int count;
}

public class ToC_SET_ENERGY : ToC_BASE
{
	public int energy;
	public int chargeTime;
}

public class ToS_SET_HELL_TICKET : ToS_BASE
{
	public int count;
}

public class ToC_SET_HELL_TICKET : ToC_BASE
{
	public int hellTicket;
}

public class ToS_SET_GOLD : ToS_BASE
{
	public int count;
}

public class ToC_SET_GOLD : ToC_BASE
{
	public int gold;
}

public class ToS_SET_RUBY : ToS_BASE
{
	public int count;
}

public class ToC_SET_RUBY : ToC_BASE
{
	public int ruby;
}

public class ToS_SET_FRIEND_POINT : ToS_BASE
{
	public int count;
}

public class ToC_SET_FRIEND_POINT : ToC_BASE
{
	public int friendPoint;
}

public class ToS_SET_EPIC_POSITION : ToS_BASE
{
	public int act;
	public int stage;
	public int round;
}

public class ToC_SET_EPIC_POSITION : ToC_BASE
{
	public int maxAct;
	public int maxStage;
	public int maxRound;
}

public class ToS_SET_EQUIPMENTS : ToS_BASE
{
	public string[] equipments;
}

public class ToC_SET_EQUIPMENTS : ToC_BASE
{
	public string[] equipments;
}

public class ToS_SET_SKILLRUNES : ToS_BASE
{
	public string[] skillRunes;
}

public class ToC_SET_SKILLRUNES : ToC_BASE
{
	public string[] skillRunes;
}

public class ToS_SET_UNITRUNES : ToS_BASE
{
	public string[] unitRunes;
}

public class ToC_SET_UNITRUNES : ToC_BASE
{
	public string[] unitRunes;
}



public class ToS_LOGOUT : ToS_BASE
{
}

public class ToC_LOGOUT
{
}

public class ToS_REMOVE_USER : ToS_BASE
{
}

public class ToC_REMOVE_USER
{
}
