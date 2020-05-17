using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary> 필요한 값들 이곳에서 정의. </summary>
public static class SystemDefine
{
    public const int BagMAX = 100;//인벤토리 최대 개수
    public const uint MinUseItemId = 550000;//로우 데이터에서 소모아이템인지를 판별 하기보다는 이것으로 판별하자.
    public const ushort MaxPartnerEnchantLevel = 10;//강화 최대 레벨
    //public const ushort MaxEvolveGrade = 10;//최대 등급(별등급이 10개있어야 1올라감)
    //public const ushort MaxEvolveMinorGrade = 10;//최대 별 등급
    public const ushort MaxEvolveMinorPartner = 5;//파트너 작은 별 최대 등급
    public const ushort MaxEvolveGradePartner = 5;//파트너 등급 최대 등급
    
    public const byte MaxJewelCount = 4;//보석 삽입 최대 개수
    //public const byte MaxInvenSlot = 100;//인벤토리 최대 개수
    public const byte MaxChatCount = 100;//채팅 최대 보관 개수
    
    //파트너 경험치석 아이디. 추후에는 하드코딩말고 따로 써야함.
    public const uint ExpItemLowDataID_1 = 560400;
    public const uint ExpItemLowDataID_2 = 560401;
    public const uint ExpItemLowDataID_3 = 560402;
    public const uint ExpItemLowDataID_4 = 560403;
    public const uint ExpItemLowDataID_5 = 560404;
    public const uint ExpItemLowDataID_6 = 560405;

    public const uint SweepItemLowDataID_1 = 599007;//일반 스테이지 용
    public const uint SweepItemLowDataID_2 = 599010;//콜로세움 용

    //획득 아이템 연출에 필요한 정보
    //public const uint DummyEnergyId = 10;
    //public const uint DummyExpId = 11;
    //public const uint DummyGoldId = 12;
    //public const uint DummyCashId = 13;
    
    public const uint ResetBuffUseItem = 599008;//버프스킬리스트 초기화 시키는 아이템 아이디
    public const uint BadgeUseItem = 599002;//휘장 아이템 아이디

    public const uint MaxChapter = 10;//챕터 최대 수. 테이블에서 판독할 수 없으므로 이걸로 판별.
    public const uint MaxStage = 10;//스테이지 최대 개수. 이거는 

    public const uint MaxTowerRanking = 10;//랭킹 보여주는 최대 값
    public const byte MaxBuffLevel = 80;//버프 최대 렙
    public const byte MaxBuff = 4;//버프 개수

    public const uint HeartCooltime = 86400;    //하트쿨타임 초기값

    public const float EvolveValue = 1.01f;//승급 증가량
    public const float EnchantValue = 1.06f;//강화 증가량
    public const float CostumeValue = 1.001f;//코스튬 승급 증가량

    /*중국어 간체
    public const string LocalGuide = "抵制不良游戏，拒绝盗版游戏。\n注意自我保护，谨防受骗上当。\n适度游戏益脑，沉迷游戏伤身。\n合理安排时间，享受健康游戏。";
    public const string LocalLoadingDesc = "正在读取数据";
    public const string LocalLoadingPer = "加载 {0}%";
    public const string LocalLoginState_0 = "获取服务器列表";
    public const string LocalLoginState_1 = "成功连接登陆服务器";
    public const string LocalLoginState_2 = "正在连接游戏服务器";
    public const string LocalVersion = "{0}版本";
    public const string LocalEff = "_G";
    */
    /*중국어 번체 */
    public const string LocalGuide = "抵制不良遊戲，拒絕盜版遊戲。\n注意自我保護，謹防受騙上當。\n適度遊戲益腦，沉迷遊戲傷身。\n合理安排時間，享受健康遊戲。";
    public const string LocalLoadingDesc = "正在讀取資料";
    public const string LocalLoadingPer = "載入 {0}%";
    public const string LocalLoginState_0 = "獲取伺服器列表";
    public const string LocalLoginState_1 = "成功連接登入伺服器";
    public const string LocalLoginState_2 = "正在連接遊戲伺服器";
    public const string LocalVersion = "{0}版本號";
    public const string LocalEff = "_B";
    
}

public enum eLoginType : int
{
    GUEST = 0,
    GOOGLE = 1,
    FACEBOOK = 2,
}

public enum eAttackType : int
{
    Melee = 1,
    Range = 3,  // Range는 All과 같다고 보면됨.
    All = Melee | 2,
}

public enum MoveType : int
{
    None = 0,
    Ground = 1,
    Air = 2,
}

public enum ModelQuality { LOW = 0, HIGH, UI };

public enum BuffDurationType : int
{
    Normal = 0,             //일반버프
    SkillAttach = 1,        //스킬에 종속(스킬풀리면 풀림)
    P_Extra = 2,            //맹장 특수버프(버프가 풀리면 특수상태 종료)   
    AuraType = 3,           //버프를 걸은 유저가 죽을시 해제 
}

///<summary>
/// 유닛 애니메이션
/// 넘버링 변경. 어택과 스킬이 중간부분에 들어있던것들을 뒤쪽으로 빼고 추가될 여지를 남겨 놓는다.
/// </summary>
public enum eAnimName : int
{

    Anim_none = 0,
    Anim_idle = 1,
    Anim_walk = 2,	//walk가 없을때는 run 으로 대체
    Anim_move = 3,
    Anim_die = 4,
    Anim_damage = 5,
    Anim_stand = 6,
    Anim_down = 7,
    Anim_stun = 8,      // 스턴동작
    Anim_victory = 9,   // 승리동작
    Anim_battle_idle = 10, 

    Anim_lose = 11,
    Anim_lose_idle = 12,
    Anim_intro = 13,    //파트너 소환ㅡ

    Anim_attack1 = 14,
    Anim_attack2 = 15,
    Anim_attack3 = 16,
    Anim_attack4 = 17, // 4타는 추가될수도있고 아닐수도있고

    Anim_skillStart = 18,

    Anim_skill1 = 19,
    Anim_skill2 = 20,
    Anim_skill3 = 21,
    Anim_skill4 = 22,
    Anim_skill5 = 23, //더 추가된 파트너 스킬용
    Anim_skill6 = 24,
    Anim_skill7 = 25,
    Anim_skill8 = 26,

	Anim_Chain = 27,
    Anim_Extra = 28,

    Anim_intro_start = 29,
	Anim_intro_end = 30,

    Anim_Max,

    //삭제된 놈들
    Anim_dash,
    Anim_Evasion,
    Anim_floating,
    Anim_floating2,
    Anim_attack1_f,
}

// 유닛 상태
public enum UnitState
{
    Idle,           //
    Wander,         //
    //ForceReturn,
    Move,           //
    Attack,         //
    ManualAttack,   //
    //Dash,
    Skill,
    Push,           //
    Stun,           // 스턴, 수면, 마비
    //Fear,           // 공포
    Floating,       // 떠있는 상태
    StandUp,
    Flying,         // 이륙 또는 착륙.
    Dying,          //
    Dead,           
    // Special,        
    // Evasion,        //< 구르기
    Event,          //< 이벤트용
    Flee,           //도망다니는
    End,
}

public enum UnitEvent
{
    Idle,
    Wander,
    ForceReturn,
    Move,
    Attack,
    Dash,
    Skill,
    Push,
    Boading,
    Down,
    StandUp,
    Dying,
    Died,
    Special,
    Flee,
    End,
}

// 클래스 정보
public enum eClassWeaponType
{
NONE    =   0,
Fighter =   1,  //권술사      //권(근접, 리치 짧음)
Pojol   =   2,  //포졸        //봉(근접, 리치 김)
Doctor  =   3,  //(한)의사    //큰 부채(원거리)
}

//아이템의 타입
public enum eItemType
{
    NONE = 0,
    USE = 1,    //사용아이템
    EQUIP = 2,  //장비아이템
}

public enum eUseitemType
{
    NONE = 0,
    COSTUME_SLICE = 1,
    PARTNER_SLICE = 2,
    MATERIAL = 3,
    ALL = 4,
}

public enum ePartType
{
    NONE = 0,       // 미착용
    HELMET = 1,     // 투구
    CLOTH = 2,      // 의복
    WEAPON = 3,     // 무기
    SHOES = 4,      // 신발
    NECKLACE = 5,   // 목걸이
    RING = 6,       // 반지
    PART_MAX = 7,
}

public enum SkillType
{
    Attack          = 1,
    Projecttile,
    Buff,
    Heal,
    EffectSkill,
    AttackBuff,
}

public enum BuffType
{

    MAXHP_INCREASE = 1,//HP 증가
    DAMAGE_INCREASE = 2,//데미지 증가
    HITRATE_INCREASE = 3,//명중률 증가
    DODGERATE_INCREASE = 4,//회피율 증가
    CRITICALRATE_INCREASE = 5,//치명타 확률 증가
    CRITICALRES_INCREASE = 6,//치명타 저항 증가
    CRITICALDAMAGE_INCREASE = 7,//치명타 데미지 증가
    LIFESTEAL_INCREASE = 8,//생명력 흡수 증가
    IGNOREDEF_INCREASE = 9,//방어력 무시 증가
    DAMAGEREDUCE_INCRESE = 10,//데미지 감소 증가
    DAMAGEREDUCEPER_INCREASE = 11,//데미지 감소율 증가
    COOLTIMEREDUCE_INCREASE = 12,//쿨타임 감소 (애니메이션 시간 보존 필요)
    ATTACKSPEED_INCREASE = 13,//공격 속도 증가
    EXP_INCREASE = 14,//획득 경험치 증가 (몬스터 처치 시 획득 경험치)
    ALLSTAT_INCREASE = 15,//모든 능력치 증가 (1~14 중 12,14를 제외한 수치 값 증가)
    ANGLEDEFUP = 101,  //방향기준 방어력업
    CHARGEATTACK = 102,  //방향기준 방어력업

    Attack = 161,  //< 공격력(%)
    Def = 162,  //< 방어력(%)
    MaxHp = 163,  //< 전체체력(%)
    CriticalRate = 167,  //< 크리티컬확률(%)
    CriticalDam = 168,  //< 크리티컬대미지(%)
    AttackSpeed = 169,  //< 공격속도(%)
    AllImmune = 170,  //< 모든공격 무적
    AttackImmune = 171,  //< 일반공격 무적
    SkillImmune = 172,  //< 스킬공격 무적

    /*
    NatureAttack    = 121,  //< 숲속성 공격력 상승(%)
    PoisonAttack    = 122,  //< 독속성 공격력 상승(%)
    WaterAttack     = 123,  //< 물속성 공격력 상승(%)
    MetalAttack     = 124,  //< 철속성 공격력 상승(%)
    FireAttack      = 125,  //< 불속성 공격력 상승(%)
    HollyAttack     = 126,  //< 빛속성 공격력 상승(%)
    DarkAttack      = 127,  //< 어둠속성 공격력 상승(%)
    */

    Heal = 131,  //< 회복(즉시)
    HealDot = 132,  //< 회복(지속)
    IgnoreDef = 133,  //< 방어력무시(%)

    //BleedingDot = 134,  //< 출혈(도트)
    BurnDot         = 135,  //< 화상(도트)
    PoisoningDot    = 136,  //< 독(도트)
    //IceDot          = 137,  //< 동상(도트)

    Shield = 138,  //< 방어막(%)
    SkillObject = 140,  //< 스킬오브젝트


    //Freeze          = 144,  //< 얼림
    //StoneCurse      = 145,  //< 석화

    Stun = 147,  //< 기절
    Down = 148,  //< 다운
    //Clone           = 149,  //< 분신
    //Summon          = 150,  //< 소환
    //Berserker       = 151,  //< 광폭화
    Knockback = 152,  //< 넉백

    Splash = 160,  //< 장판(주변적 공격)
}


public enum ePassiveType
{
    AttackRate          = 101,  // 공격력(%)
    DefenceRate         = 102,  // 방어력(%)
    MaxHpRate           = 103,  // 체력(%)
    CriticalChance      = 107,  // 크리티컬 발생 확률
    CriticalDamageRate  = 108,  // 크리티컬 데미지
    AtkSpeedRate        = 109,  // 공격속도(%)

    NatureAttack        = 121,  //< 숲속성 공격력 상승(%)
    PoisonAttack        = 122,  //< 독속성 공격력 상승(%)
    IceAttack           = 123,  //< 물속성 공격력 상승(%)
    MetalAttack         = 124,  //< 철속성 공격력 상승(%)
    FireAttack          = 125,  //< 불속성 공격력 상승(%)
    HolyAttack          = 126,  //< 빛속성 공격력 상승(%)
    DarkAttack          = 127,  //< 어둠속성 공격력 상승(%)

    AddExp              = 201,  //< 같은 속성일경우 경험치 증가
}

/// <summary>
/// 스탯용으로 사용됨
/// </summary>
public enum AbilityType
{
    NONE                    =    0,//
    HP                      =    1,//생명력
    DAMAGE                  =    2,//공격력
    HIT_RATE                =    3,//명중률
    DODGE_RATE              =    4,//회피율
    CRITICAL_RATE           =    5,//치명타 확률
    CRITICAL_RES            =    6,//치명타 저항
    CRITICAL_DAMAGE         =    7,//치명타 피해
    DRAIN_HP                =    8,//생명력 흡수
    DEFENCE_IGNORE          =    9,//방어력 무시
    DAMAGE_DECREASE         =   10,//데미지 감소
    DAMAGE_DECREASE_RATE    =   11,//데미지 감소율
    COOLTIME			    =   12,//쿨타임
    ATTACK_SPEED		    =   13,//공격 속도
    EXP_UP                  =   14,//경험치 증가
    ALLSTAT_RATE            =   15,//모든 스텟 증가률

    ATTACK_RANGE		    =	  16,//공격 유효 거리
    ATTACK_ANGLE		    =	  17,//공격 유효 각도

    MOVE_SPEED			    =	  18,//이동 속도

    DETECTED_RANGE		    =	  19,//타겟 인지 거리

    SUPERARMOR              =   20, //슈퍼 아머값
    SUPERARMOR_RECOVERY_TIME=   21, //슈퍼 아머 자동회복 시간
    SUPERARMOR_RECOVERY_RATE=   22, //슈퍼 아머 자동회복 %
    SUPERARMOR_RECOVERY     =   23, //슈퍼 아머 자동회복 절대값

    WEIGHT                  =   24, //무게값
}

public enum FormulaType
{
    NONE                    =   0,//없음
    EVADE                   =   1,//회피
    REDUCE_DAMAGE           =   2,//데미지 감소 비율
    LIFE_STEAL              =   3,//생명력 흡수
    CRITICAL_RATE           =   4,//크리티컬 확률
    CRITICAL_DAMAGE         =   5,//크리티컬 데미지
    ATTACK_SPEED            =   6,//공격 속도
    COOLTIME_REDUCE         =   7,//쿨타임감소
    COSTUME_UP              =   8,//코스튬 승급
    EQUIP_UP                =   9,//장비 승급
    EQUIP_REFINE            =   10,//장비 강화
    PARTNER_UP              =   11,//파트너 승급
    PARTNER_REFINE          =   12,//파트너 강화
    EVADE_CAP               =   13,//회피상한
    REDUCE_CAP              =   14,//대미지감소 상한
    LIFE_STEAL_CAP          =   15,//생흡 상한
    CRITICAL_RATE_CAP       =   16,//치명타 확률 상한
    ATTACK_SPEED_CAP        =   17,//공속 상한
    COOLTIME_REDUCE_CAP     =   18,//쿨감소 상한
    LEVELUP_CALIBRATION     =   19,//레벨업 보정치
	SUB_CHAR_UP   		  	=   20,//부캐릭터 레벨업
}

public enum BattlePointType
{
    None = 0,
    DAMAGE              ,//데미지
    HP                  ,//체력
    DODGE_RATE          ,//회피
    HIT_RATE            ,//명중
    DEFENCE_IGNORE      ,//방어력 무시
    DAMAGE_DECREASE     ,//데미지 감소
    DAMAGE_DECREASE_RATE,//데미지 감소율
    CRITICAL_RATE       ,//크리티컬 확률
    CRITICAL_RES        ,//키리티컬 저항
    CRITICAL_DAMAGE     ,//크리티컬 데미지
    COOLTIME            ,//쿨타임
    ATTACK_SPEED        ,//공격속도
    DRAIN_HP            ,//생명력 흡수

    MAX,
}

/// <summary>
/// 스테이지 난이도
/// </summary>
public enum eDifficulty
{
    Normal = 1,
    Hard,
    Nightmare,
    Hell,
    Max = Hell,
}

public enum UnitType : int
{
    None         = 0,
    Unit         = 1 << 0,  // 주 캐릭터
    Npc          = 1 << 1,
    Boss         = 1 << 2,
    Building     = 1 << 3,
    Trap         = 1 << 4,
    Prop         = 1 << 5,
    Event        = 1 << 6,
    TownUnit     = 1 << 7,  // 마을용 주 캐릭터
    TownNpc      = 1 << 8,
    TownNINPC    = 1 << 9,  // 마을에서 그냥 돌아다니는 NPC
    All          = int.MaxValue
}

public enum TargetType
{
    None        = 0, // 없음
    Pc          = 1, // 자신
    Ally        = 2, // 아군(자신포함)
    Enemy       = 3, // 적
    ExceptPc    = 4, // 아군(자신 제외)
    All         = 5, // 모두
}

public enum AIType
{
    Skill,          // 스킬발동관련
    Attack,         // 공격타입에 따른 행동
    Intelligence,   // 각각의 지능
}

public enum LandType
{
    NONE    = 0,
	PREAH   = 1,
	LATINA  = 2,
	AZEN    = 3,
	AKRA    = 4,
    MAX     = AKRA,
};

public enum INVENTYPE
{
    NPC = 1,
	ITEM = 2,
	RUNE = 3,
	RUNEPAGE = 4,
}

public enum BuildType
{
    MAINBUILD       = 1001,
    SMITHY          = 1011,
    SUMMON          = 1021,
    SUMMONENCHANT   = 1031,
    GATEOFADVERSITY = 1041,
    AWAKEN          = 1051,
    MAGICSTORE      = 1061,
    WISH            = 1071,
    EXTRACTION      = 1081,
    CRISTAL         = 1091,
    SPARRING        = 1101,
    TRAINING        = 1111,
    MIX             = 1121,
    GUILD           = 1131,
    GOLDMINE        = 1141,
    DEPOSITORY      = 1151,
    SANCTUM         = 2001,
    PLANTS          = 2011,
    PROTECT         = 2021,
    WATER           = 2031,
    FIRE            = 2041,
    METAL           = 2051,
    POISON          = 2061,
    NATURE          = 2071,
    LIGHT           = 2081,
    SWORD           = 2091,
    ALTAR           = 2101,
    TOTEM           = 2111,
    GUARDIAN        = 2121,
}

public enum BuildSubType
{
	MAINBUILD = 1,
	SMITHY = 2,
	SMITHYLAB = 3,
	SUMMON = 4,
	UNITMIX = 5,
	MINE = 6,
	CASHMINE = 7,
	STORAGE = 8,
	STORAGEGOLD = 9,
	GUILD = 10,
    UNITENCHANT = 11,
    TRAINING =  12,
    SPARRING = 13,
    SKILLENCHANT = 14,
    RUNEENCHANT = 15,
    OPTIONUPGRADE = 16,
    GOLDENKNIGHTS = 17,
    GATEOFADVERSITY = 18,
};

public enum RuneSizeType
{
	SMALL = 1,
	BIG = 2,
	ALL = 3,

}

/// <summary>
/// Stat에 적용할때 기본값으로 무엇을 사용하는지 여부
/// </summary>
public enum StatValueType
{
    ABSOLUTE = 1,   // 절대값
    PERCENT = 2,    // 퍼센트값
}

/// <summary>
/// Stat에 적용할때 기존값과 어떻게 계산될것인지 결정
/// </summary>
public enum StatCalcType
{
    ADD = 1,    // 합
    MUL = 2,    // 곱
}

public enum MaterialToTimeType
{
	LANDMATERIALTOTIME = 6000,
	SOULSTONETOTIME = 1200,
	RIFTSTONETOTIME = 1200,
	GOLDTOTIME = 4,
	EXPTOTIME = 15,
}
public enum MaterialType
{
	NONE = 0,
	LANDMATERIALPREAH = 1,
	LANDMATERIALLATINA = 2,
	LANDMATERIALAZEN = 3,
	LANDMATERIALAKRA = 4,
	SOULSTONE = 5,
	RIFTSTONE = 6,
	GOLD = 7,
	EXP = 8,
}

//public enum GK_AtlasType
//{
//	None = 0,
//	EquipItem = 1,
//	ItemData = 2,
//	Build = 3,
//	Ability = 4,
//	Gacha = 5,
//	Avatar = 6,
//	UnitData = 7,
//	RuneData = 8,
//    InGame = 9,
//}

public enum UnitSlotType
{
	None = 0,
	Inventory = 1,
	BuildDefense = 2,
}

public enum MaxSlotType
{
	DefenseMaxSlot = 4,
}

// PVP 등급
public enum ePvpGradeType
{
    Bronze,
    Silver,
    Gold,
    Dia,
}


public enum eItemLowdataType
{
    Equipment = 1,
    Data = 2,
    Build = 3,
    Ability = 4,
    Gacha = 5,
    Avatar = 6,
    Unit = 7,
    Rune = 8,
}

public enum eQuestType
{
    FinishNow = 0,          //목표값없음(즉시완료)
    Energy = 1,             //에너지 소비
    Sword = 2,              //칼 소비
    Key = 3,                //열쇠 소비
    AccountLevelUp = 11,    //계정 레벨 달성
    Training = 21,          //훈련장 사용 횟수
    Enchant = 31,           //장비 강화
    Summon = 41,            //용병 소환
    DailyMission = 61,      //일일미션 완료(즉시완료)
    StageClearRank = 81,    //별 3개달성
    StageClear = 101,       //목표 던전 클리어
    StageClearRandom = 102, //아무던전이나 클리어
    GetEquipment = 111,     //장비획득
    GetItemAll  = 112,      // 모든 장비 획득
    GetUnit = 121,          //유닛획득
    GetAllUnit  =   122,    //< 모든 유닛 획득
    UnitEvolution = 123,    //< 유닛 진화
    UnitAwakenProperty = 124,      //< 유닛 각성(해당지역)
    UnitAwaken      = 125,  //< 모든 유닛 각성
    UnitMaxLevel    = 126,  //< 유닛 최초 만렙
    UnitMix         = 127,  //< 유닛 조합
    MonsterKill = 131,      //몬스터 처치
    AllMonsterKill  = 132,  //< 모든 몬스터 처치
    AllBossMonsterKill = 133,  //< 모든 보스 몬스터 처치
    RaidBossKillProperty    = 134,  //< 해당 지역 레이드보스 처치
    RaidBossKill = 135,  //< 레이드보스 처치
    CreateBuilding = 141,   //건물 건설
    UpgradeBuilding = 151,  //건물 업그레이드
    TakeFriend  = 161,  //< 친구 소환
    SendHeart   = 162,  //< 하트보내기
    GetGold     = 171,  //< 골드 획득
    SellItem    = 172,  //< 아이템 판매
    UnitExtract = 181,  //< 유닛 추출
    UnitDie         = 301,  //< 유닛 사망
}

public enum GuildMemberPosition
{
    Request = 0,
    Master = 1,
    SubMaster = 2,
    NormalMember = 3,
}
// 상점 타입
public enum ShopType
{
	Build,
	Honor,
	Cash,
	Special,
    InGame,
    Ruby,
}

// 상점 캐쉬 타입
public enum ShopCashType
{
    None = 0,
    Energy = 10,
    Sword = 11,
    Gold = 3,
    Ruby = 12,

    BonusReward = 23,
    AutoSkill   = 24,
}

// 직업 특성 아이콘 타입 
public enum ProfessionType
{
	n_icon_unittype_01 = 1,
	n_icon_unittype_03 = 2,
	n_icon_unittype_04 = 3,
}
// 유닛 속성
public enum UnitPropertiType
{
	n_icon_attribute_04 = 1,
	n_icon_attribute_05 = 2,
	n_icon_attribute_02 = 3,
	n_icon_attribute_03 = 4,
	n_icon_attribute_01 = 5,
	n_icon_attribute_06 = 6,
}

public enum GachaRewardType
{
    None = 0,   // None
    Character,  // 1 캐릭터
    Equipment,  // 2 장비
    Gold,       // 3 골드
    Exp,        // 4 경험치(사용안함)
    Cash,       // 5 루비
    Heart,      // 6 하트
    Etc,        // 7 기타 아이템(정수/소환석/다이아 등)
    Energy,     // 10 에너지
}

public enum StageModeType
{
    Single,
    Raid,
    Infinite,
}
/*
public enum ChatType
{
    Normal  = 0,
    Guild   = 1,
    Notice  = 2,
    System  = 3,
    GetItem = 4,
    GetUnit = 5,
    EnchantItem = 6,
    EvolutionUnit = 7,
    Emergency = 8,
}
*/

public enum ChatType
{
    Everything,
    World=1,//전역
    Whisper=2,//귓말
    Guild=3,//길드
    System=4,//서버에서 주거나 로컬에서 찍는 로그
    Map=5,//지역채팅

    //Friend=6,//친구와 1:1채팅(채팅UI가 아님 친구 UI에서 처리함)

}

public enum GuildChatType
{
    Default,
    Ban,        //추방
    Occupation, //점령
}

public enum DailyTicketType
{
    Mine = 1,
    Guild,
    Max,
}

public enum eGameOption
{
    BackGroundSound,
    FxSound,
    Notice,
    Light,
    GamePerformance,
    Max,
}

public enum eNotice
{
    Kakao,
    Daytime,
    Night,
    Max,
}

public enum eLangaugeType
{
    Korean,
    English,
    Chinese1,
    Chinese2,
    Max,
}

public enum eEventPopupType
{
    UnLimite = 0,
    Unit,
    Raid,
    Stage,
}

public enum eRaidType
{
    Raid,
    Secret,
}

public enum InvenType
{
    None = 0,
    Inven = 1,
    Keep = 2,
    Training = 4,
    Defense = 8,
    Mine = 16,
    Guild = 32,
}

public enum GuildPositionType
{
    //길드 직위(0:대기,1:길마,2:부길마,3:일반)
    StandBy = 0,
    Master,
    SubMaster,
    Normal,
}

public enum InGameBuffItemType
{
    ExpUp       = 4001,
    SpeedUp     = 4002,
    ItemUp      = 4003,
    AutoSkill   = 4004,

    GoldUp      = 5000,
}

//< 인게임내에서 대미지 감소 타입
public enum eDamResistType
{
    None,
    UseSkill,
    Evasion,
    Shield,
}

/// <summary>
/// 아이템의 타입 - 인게임 정보 교체시 코스튬,파트너 삭제
/// </summary>
public enum ItemType
{
    None,
    Costum,
    Partner,
    Weapon,
    Dress,
    Cap,
    NeckLace,
    Ring,
    Shoes,
}
/*
/// <summary>
/// 파트너의 타입
/// </summary>
public enum PartnerType
{
    None,
    Defence,//방어 형
    Damage,// 공격 형
    Buff,//버프 형
}
*/
//Class를 구분짓는 enum
public enum PartnerClassType
{
    None=0,//PartnerPanel에서 사용(모든 클래스)
    //지금은 0번부터? 바뀌면 바꿔야함
    Attack=1,//공격형
    Defence,//방어형
    Buff,//버프형
}
public enum TownRunTargetType
{
    Npc, Potal,
}

//게임 타입
public enum GameType {
     SINGLE = 1,//일반 던전
     D_FIGHTER = 2,//난투전
     COLO = 3,//콜로세움
     INF_GOLD = 4,//골드 던전
     INF_EXP = 5,//경험치 던전
     INF_TOWER = 6,//무한의 탑
     RAID = 7,//레이드
     PVP = 8,//1:1 pvp
     TOURNAMENT = 9,//토너먼트
 }

//재화 타입
//TODO 수정시 ITEM getAssetFieldName 수정해야함 
public enum AssetType {
    None=0,
    CostumeShard=1,//코스튬 조각
    PartnerShard=2,//파트너 조각
    Material=3,//재료
    Jewel=4,//보석
    Gacha =5,//뽑기

    Gold=6,//게임 머니
    Energy=7,//에너지
    Cash=8,//게임 캐쉬(원보)
    Badge=9,//휘장
    FAME=10,//성망
    Contribute =11,//공헌
    Honor=12,//명예
    LionBadge=13,//사자왕휘장
    Sweep=14,//소탕권
    Heart=15,//우정 포인트
    Item=16,//아이템(장비, 소모성)//무슨말인지는 모르겠다.
    Exp=17,//경험치석
    ColoSweep=18,//콜로세움 소탕권

    Everything = 99,//공통//보류
}

//스테이지 클리어 미션
public enum ClearQuestType
{
    NONE=0,
    NO_DIE_PARTNER=1,//파트너 n명 사망하지않고 클리어
    TIME_LIMIT=2,//시간안에 클리어
    HP_PERCENT=3,//HP n% 이상으로 클리어
    STAGE_CLEAR=4,//스테이지 클리어조건
    MAX_DAMAGE=5,//최대 n데미지 이상 넣기
    MINIMUM_HIT=6,//피격 횟수 n회 이하로 클리어
    UNEQUIP_PARTNER=7,//파트너 미장착 클리어
}

//미션 타입
public enum MissionType
{
    INSTANCE_CLEAR = 1,
    GUILDQUEST_CLEAR = 2,
    GOLD_BUY = 3,
    HEALTH_USE = 4,
    MONSTER_KILL = 5,
}

public enum MissionSubType
{
    SINGLE =2,//모험
    BOSS_RAID =5,//레이드
    SPECIAL =6,//골드,경험치
    DOGFIGHT =8,//난투장
    ARENA =9,//1:1Pvp
    TOWER =10,//마계의탑
    COLOSSEUM =11,//콜로세움
    GUILD_OCCUPY =12,//길드 점령전
}

public enum EtcID
{
    None                    = 0,
    Demagerange = 1,
    MaxLevel = 2,
    MaxSubEvolve = 3,
    FirstGatchaId = 4,
    TengatchaId = 5,
    InvenMax = 6,
    ItemMaxenchant = 7,
    ItemMaxevolve = 8,
    itemEvolveCount = 9,
    CostumeEvolveCount = 10,
    CostumeMaxevolve = 11,
    CostumeOpenSlot1 = 12,
    CostumeOpenSlot2 = 13,
    CostumeOpenSlot3 = 14,
    CostumeOpenSlot4 = 15,
    PartnerMaxenchant = 16,
    PartnerMaxevolve = 17,
    PartnerEvolveCount = 18,
    ServerfightEnterlv = 19,
    ComplimentReward = 20,
    GoodReward = 21,
    FreefightMax = 22,
    FreefightTime = 23,
    Pkgetpoint = 24,
    PvpEnterLv = 25,
    PvpenterCount = 26,
    PvPOpenTime = 27,
    PvPCloseTime = 28,
    PvpWinPoint = 29,
    PvpLosePoint = 30,
    PvpThreeWinPoint = 31,
    PvpThreeLosePoint = 32,
    HitValue = 33,
    MonsterAttackDelay = 34,
    AutoSkillDelay = 35,
    HitResist = 36,
    friendHeart = 37,
    maxFriend = 38,
    friendDel = 39,
    friendInvite = 40,
    buffResetMaxPoint = 41,
    buffResetGetPoint = 42,
    GuildCondition = 43,
    GulidPenalty = 44,
    GuildCost1 = 45,
    GuildCost2 = 46,
    UniqueItemNeedCash = 47,
    SINGLE_ReviveCount = 48,
    RAID_ReviveCount = 49,
    TOWER_ReviveCount = 50,
    MailMaxCount = 51,
    EnergyCount = 52,
    EnergyRecovery = 53,
    GoldCount = 54,
    PassCard = 55,
    maxFriendList = 56,
    partnerAddDamage = 57,
    ExpBattleCount = 58,
    GoldBattleCount = 59,
    FriendInviteLimit = 60,
    PartnerMaxGrade = 61,
    ShopItemcount = 62,
    TowerCount = 63,
    AddTime = 64,
    BossRaid1Count = 65,
    BossRaid2Count = 66,
    BossRaid3Count = 67,
    Demagerange2 = 68,
    shopListReflash = 69,
    FreeGatchaTime1 = 70,
    FreeGatchaTime2 = 71,
    FreeBattleItem = 72,
    FreeBattlePoint = 73,
    PvPTime = 74,
    PvpMatchingTime = 75,
    GuildMaxRequest = 76,
    GuildPrayNormal = 77,
    GuildPrayHigh = 78,
    GuilddonationMax = 79,
    Guildmasterreset = 80,
    FreeFightwatingtime = 81,
    FreeFightreviveCoin = 82,
    GuildReceiveMax = 83,
    MonsterFindRange = 84,
    MultyBossRaid1Count = 85,
    MultyBossRaid1Partymax = 86,
    ComboCheckStartYear = 87,
    ComboCheckStartMonth = 88,
    ComboCheckStartDay = 89,
    ComboCheckEndYear = 90,
    ComboCheckEndMonth = 91,
    ComboCheckEndDay = 92,
    PurchaseCountStartYear = 93,
    PurchaseCountStartMonth = 94,
    PurchaseCountStartDay = 95,
    PurchaseCountEndYear = 96,
    PurchaseCountEndMonth = 97,
    PurchaseCountEndDay = 98,
    ConsumeCountStartYear = 99,
    ConsumeCountStartMonth = 100,
    ConsumeCountStartDay = 101,
    ConsumeCountEndYear = 102,
    ConsumeCountEndMonth = 103,
    ConsumeCountEndDay = 104,
    ShopRefreshMoney = 105,
    ShopRefreshMountMoney = 106,
    ShopRefreshMountMaxMoney = 107,
    ColosseumCount = 108,
    ColosseumPartymax = 109,
    GuildQuestResetime = 110,
    ActiveRewardMax = 111,
    FreeFightChanelResettime = 112,
    FreeFightBossRegenTime = 113,
    FreeFightkilltext01 = 114,
    FreeFightkilltext02 = 115,
    FreeFightkilltext03 = 116,
    FreeFightkilltext04 = 117,
    FreeFightkilltext05 = 118,
    FreeFightkilltext06 = 119,
    PvPcashentercount = 120,
    PvPoriginalcoinmax = 121,
    PvPautorewardtime = 122,
    PvPovermatchingcoin = 123,
    PvPrankbattlemin = 124,
    PvPrankbattlemax = 125,
    Postmaxwarningvalue = 126,
    MaxPlayableStage = 127,
    NormalGatchaLimit = 128,
    RareGatchaLimit = 129,
    AddFriendagain = 130,
    GetEnergyMax = 131,
    FreeFightkillPoint01 = 132,
    FreeFightkillPoint02 = 133,
    FreeFightkillPoint03 = 134,
    FreeFightkillPoint04 = 135,
    FreeFightkillPoint05 = 136,
    FreeFightkillPoint06 = 137,
    FreeBattleOverPoint = 138,
    FreeFightOverkillPoint01 = 139,
    FreeFightOverkillPoint02 = 140,
    FreeFightOverkillPoint03 = 141,
    FreeFightOverkillPoint04 = 142,
    FreeFightOverkillPoint05 = 143,
    FreeFightOverkillPoint06 = 144,
    CharacterMaxSkillLevel = 145,
    InvenMaxWarning = 146,//인젠토리 경고
    InvenDefault    = 147,//인벤토리 기본 지급 칸수
    GuildMarkChangeCost = 148,//길드마크변경원보비용
    GuildNameChangeCost = 149,//길드이름변경원보비용
    GuildNameChangeCooltime = 150,  //길드이름변경쿨타임
    EquipSetOpen01CharLevel = 151,
    EquipSetOpen02CharLevel = 152,
    EquipSetOpen03CharLevel = 153,
    EquipSetOpen04CharLevel = 154,
    PvPcashvalue            = 155,
    PvPfirstgetcash         = 156,
    WinGetCoin_b            = 157,
    LoseGetCoin_b           = 158,
    PartnerMaxSkillLevel    = 159,
    Partner1GradeMaxLevel   = 160,    //파트너일반등급최대레벨
    Partner2GradeMaxLevel   = 161,    //파트너우수
    Partner3GradeMaxLevel   = 162,    //파트너 희귀
    Partner4GradeMaxLevel   = 163,    //파트너 고대
    Partner5GradeMaxLevel   = 164,    //파트너 전설
    Partner6GradeMaxLevel   = 165,    //파트너무쌍
}

public enum UI_OPEN_TYPE
{
    NONE,
    RESTART,
    NEXT_ZONE,
}

public enum TutorialType
{
    NONE=-10,

    INGAME              =0, //최초 튜툐
    QUEST               =1, //퀘스트
    STAGE               =2, //스테이지 입장
    ENCHANT             =3, //장비 강화
    ACHIEVE             =4, //업적
    TITLE               =5, //칭호
    CHAPTER_REWARD      =6, //챕터보상
    CHAR_SKILL          =7, //캐릭터 스킬
    BENEFIT             =8, //혜택
    SOCIAL              =9, //소셜
    TOWER               =10,//마계의탑
    GACHA               =11,//뽑기
    PARTNER             =12,//파트너 소환 및 스킬
    SHOP                =13,//상점
    CATEGORY            =14,//재화 인벤
    FREEFIGHT           =15,//난투장
    EQUIP_SET           =16,//장비셋트
    CHAPTER_HARD        =17,//스테이지 어려움
    SKILL_DUNGEON       =18,//스킬 재료던전
    STATUS              =19,//신분 획득 및 스킬셋 선택
    ARENA               =20,//차관 입장
    EQUIP_DUNGEON       =21,//재료던전

    MAX,//최대값
    ALL_CLEAR=999//튜토리얼 모든거 클리어
}

public enum OpenTutorialType
{
    NONE=0,
    ARENA=7,//차관
    BOSS_RAID = 11,//보스레이드
    TOWER = 13,//마계의탑
    COLOSSEUM = 16,//콜로세움

    //FREEFIGHT=
    ALL_CLEAR = 99//튜토리얼 모든거 클리어
}

public enum AlramIconType
{
    //ACTIVITY = 0,//활동량
    BENEFIT = 0,//혜택
    SHOP = 1,//상점
    OPTION = 2,//옵션
    DUNGEON = 3,//던전
    SINGLE = 4,//싱글스테이지
    PARTNER = 5,//파트너
    CHAR = 6,//캐릭터
    CATEGORY = 7,//재화인벤
    //QEUST = 8,//퀘스트
    ACHIEVE = 8,//보상
    GUILD = 9,//길드
    SOCIAL = 10,//소셜
    RANK = 11,//랭킹
    _SOCIAL = 12,//소셜패널은 두개로구분되기때문에 한개더해줌
}

public enum ContentType
{
    TOWER = 1,
    SKILL_DUNGEON=2,
    EQUIP_DUNGEON=3,
    BOSS_RAID=4,
    ARENA=12,
}

/// <summary> 마을 컨텐츠들 열고 닫기 타입 </summary>
public enum OpenContentsType
{
    Char      = 1,  //오픈연출타입	캐릭터	
    Achiev    = 2,  //오픈연출타입	업적	
    Benefit   = 3,  //오픈연출타입	혜택	
    Social    = 4,  //오픈연출타입	소셜	
    Dungeon   = 5,  //오픈연출타입	컨텐츠	
    Shop      = 6,  //오픈연출타입	상품	
    Partner   = 7,  //오픈연출타입	파트너	
    FreeFight = 8,  //오픈연출타입	난투장	
    Rank      = 9,  //오픈연출타입	랭킹	
    Guilde    = 10, //오픈연출타입	길드	
    Category  = 11, //오픈연출타입	재화인벤
    Chapter   = 12, //오픈연출타입   챕터 페널
    Max
}