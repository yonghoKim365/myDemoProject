using UnityEngine;
using System.Collections;
using System;
using System.Text;

// tag build
// trunk build
// 2

/// <summary>
///  프로그래머 주도하에 변경해야 할 것들은 이곳에, 기획자가 수정해야 할 것은 KeyData에
/// </summary>
public class GameDefine
{
    public static bool TestMode = true;            //< 테스트모드일때만 켜줌, 배포 내릴때에는 무조건 꺼줘야함 

    public static bool skillPushTest = true;

    public static bool ServerLiveVersion = false;   //< 실제 오픈했을경우는 True로 변경해줘야함
    //<===============================================
    //<     빌드를 내릴떄 꼭 수정해야할 부분!!!
    //<===============================================
    public const string GameName = "wongfeihung";
    public static string BuildVersion = "0.0.1";
    public const string NowBuildVersion = "0";  //< 빌드를 내릴때 체크할 빌드버전
    public static ePlatformType PlatformType = ePlatformType.GOOGLEPLAY;
    public static eCountryType CountryType = eCountryType.KR;
    public static eMaketType MaketType = eMaketType.ANDROID;
    public static eLangaugeType LanaugeType = eLangaugeType.Korean;

    //< 보석관련 활성화체크
    public static bool GemPanelActive = false;

    //< 화면의 오리지널 사이즈
    static public Vector2 ScreenOrgSize = new Vector2();

    static public float ClickThresholds = 2.5f; // 클릭시 주변에서 타겟을 검색할 수 있는 거리 (반지름)

    static public Color32 UnitRimColor = new Color32(130, 130, 130, 0);
    static public Color DamageColor = new Color(1, 1, 1); // 피격시 변할 색상값

    static public float SearchTargetCheckDelay = 0.05f;    // 인지 범위 갱신 시간.

    static public bool HelperTargetSearch = false;  //< 헬퍼도 타겟으로 인정할것인지?

    static public int MaxActiveSkillCount = 4;

    static public int MaxUnitCount = 4;

    //< 카메라 거리 셋팅
    public static byte CameraDis_Pvp = 18;

    public const int unassignedID = -1;

    public static float DefaultTimeScale = 1f;

    public static int PlayerTageHP = 30;

    public const float CriticalDamageRate = 2;  // 크리티컬 발생시 적용될 데미지 증가 배수
    public const float ConvertValueToRate = 1000000;  // 100% == 1,000,000 (확률로 변환시 사용)
    public const float ConvertValueToRateOne = 10000;
    public const float ConvertMilliToSec = 1000; // millisecond 에서 second로 변환

    public const float NextMoveDelta = 0.5f; // Pressing상태에서 업데이트 간격, 다음 이동계산까지 Step 등등
    public const float FixedAggroPoint = 1000000f; // 강제로 타겟을 지정해 주기 위한 어그로 수치

    public const string AutoMode_Suffix = "_AutoMode";

    //public static string ServerErrorMsg = "네트워크가 불안정합니다.\n게임이 종료되므로, 다시 실행해주세요.";
	public static string ServerErrorMsg = "Network error.";
}

public enum eMaketType
{
    ANDROID = 12,
    IOS     = 11,
    NAVER   = 18,
}

public enum eCountryType
{
    KR,
    US,
    CN,
}

public enum ePlatformType
{
    KAKAO       = 1,
    LINE        = 2,
    BAND        = 3,
    FACEBOOK    = 4,
    GUEST       = 5,    //< 게스트
    GAMECENTER  = 6,    //< IOS
    GOOGLEPLAY  = 7,    //< 구글
    PC          = 99,   //< 개발용으로 PC

}
