using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

using System.Diagnostics;

public enum GAME_MODE
{
    NONE = 0,
    //CLIENT = 1,//?
    SINGLE = 1,//모험
    SPECIAL_EXP = 2,//스페셜 던전(경험치, 골드)
    SPECIAL_GOLD=3,
    TOWER = 4,//마탑
    //PVP = 3,//Arena 인가??
    //SPARRING = 4,//??
    RAID = 5,//보스 레이드
    ARENA = 6,//Pvp
    GUILD_WAR=7,
    FREEFIGHT = 8,//난투장
    COLOSSEUM=9,//콜로세움
    MULTI_RAID=10,//멀티 보스 레이드
    TUTORIAL =88,//튜토리얼 게임 

    SKILLTOOL = 99,//??
}

public partial class SceneManager : Immortal<SceneManager>
{
    /*
    // 싱글 자동 전투 모드
    //public bool AutoMode = false;
    GameObject _AtlasMgr;
	public GameObject AtlasMgr
    {
        get
        {
            if (_AtlasMgr == null)
                _AtlasMgr = GameObject.Find("AtlasMgr");

            return _AtlasMgr;
        }
    }
    */
    /*
    GAME_MODE curGameMode = GAME_MODE.NONE;
    static public GAME_MODE GameMode
    {
        get { return SceneManager.instance.curGameMode; }
        set { SceneManager.instance.curGameMode = value; }
    }
    */

    public Shader g_Tran_Cutout = null;  //"Unlit/Transparent Cutout"
    public Shader g_Tex_AddColor = null;

    private bool IsRtNetwork;
    public bool IsRTNetwork
    {
        set {
            IsRtNetwork = value;
        }
        get{
            return IsRtNetwork;
        }
    }
    
    public OptionData optionData;

    
    public class OptionData
    {
        
        //======= 옵션창에서 상태
        public bool Bgm;
        public bool Soundfx;
        public bool Voice;
        public byte Quality;
        public float SoundVolume;//볼륨
        public string ShowHpBar;  //체력표시하기 ( 0000전부, 나머지는 0,1로구분)
        public string BlockInvite;  //초개거부 ( 0000전부, 나머지는 0,1로구분)
        public string ShowName; //이름표시 (0000전부, 나머지는 0,1로구분)
        public string BlockWhisper;   //귓속말거부 (0000전부, 나머지는 0,1로구분)
        public string OffAlram;   //알림끄기 (0000전부, 나머지는 0,1로구분)
        public string ServerName;   //접속중 서버이름

        public OptionData()
        {
            bool isDefault = true;
            if (PlayerPrefs.HasKey("OptionData.json"))//저장한것 불러오기
            {
                string optionDataStr = PlayerPrefs.GetString("OptionData.json");
                if (!string.IsNullOrEmpty(optionDataStr))
                {
                    JSONObject a_ParseJs = new JSONObject(optionDataStr);//JSON형태로 값을 불러온다.
                    Bgm = bool.Parse(a_ParseJs["Bgm"].ToString());//str 변수를 사용하면 x
                    Soundfx = bool.Parse(a_ParseJs["Soundfx"].ToString());
                    Voice = bool.Parse(a_ParseJs["Voice"].ToString());
                    ShowHpBar = a_ParseJs["ShowHpBar"] == null ? "2222" : a_ParseJs["ShowHpBar"].ToString();
                    Quality = byte.Parse(a_ParseJs["Quality"].ToString());
                    BlockInvite = a_ParseJs["BlockInvite"] == null ? "1111" : a_ParseJs["BlockInvite"].ToString();
                    ShowName = a_ParseJs["ShowName"] == null ? "222" : a_ParseJs["ShowName"].ToString();
                    BlockWhisper = a_ParseJs["BlockWhisper"] == null ? "111" : a_ParseJs["BlockWhisper"].ToString();
                    OffAlram = a_ParseJs["OffAlram"] == null ? "1111" : a_ParseJs["OffAlram"].ToString();
                    ServerName = a_ParseJs["ServerName"] == null ? "" : a_ParseJs["ServerName"].ToString();
                    SoundVolume = a_ParseJs["SoundVolume"] == null ? 1f : float.Parse(a_ParseJs["SoundVolume"].ToString());

                    isDefault = false;
                }
            }

            if(isDefault)//기본 , 처음시작했을때 셋팅값
            {
                Bgm = true;
                Soundfx = true;
                Voice = false;
                Quality = (byte)QUALITY.QUALITY_HIGH;
                ShowHpBar = "2222";
                BlockInvite = "1111";
                ShowName = "222";
                BlockWhisper = "111";
                OffAlram = "1111";
                ServerName = "";
                SoundVolume = 1f;
            }
        }

        public void SaveOptionData()//JSON방식으로 데이터를 저장한다.
        {
            string optionDataStr = "{";
            optionDataStr += string.Format("\"Bgm\":{0}", Bgm);
            optionDataStr += string.Format(",\"Soundfx\":{0}", Soundfx);
            optionDataStr += string.Format(",\"Voice\":{0}", Voice);
            optionDataStr += string.Format(",\"Quality\":{0}", Quality);
            optionDataStr += string.Format(",\"ShowHpBar\":{0}", ShowHpBar);
            optionDataStr += string.Format(",\"BlockInvite\":{0}", BlockInvite);
            optionDataStr += string.Format(",\"ShowName\":{0}", ShowName);
            optionDataStr += string.Format(",\"BlockWhisper\":{0}", BlockWhisper);
            optionDataStr += string.Format(",\"OffAlram\":{0}", OffAlram);
            optionDataStr += string.Format(",\"ServerName\":{0}", ServerName);
            optionDataStr += string.Format(",\"SoundVolume\":{0}", SoundVolume);
            optionDataStr += "}";

            UnityEngine.Debug.Log(optionDataStr);
            PlayerPrefs.SetString("OptionData.json", optionDataStr);
        }

       
    }


    // same as Awake()
    protected override void Init()
    {
        
        base.Init();
        if (gameObject == null)
            return;

        // 옵션상태 
        optionData = new OptionData();
        //optionData.BlockInvite = "1111";
        //optionData.ShowHpBar = "1111";
        //optionData.ShowName = "111";
        //optionData.BlockWhisper = "111";
        //optionData.OffAlram = "1111";
        //optionData.SaveOptionData();
        //optionData = null;

        //optionData = new OptionData();

        QualityManager.instance.Initialize();
        QualityManager.instance.SetQuality((QUALITY)optionData.Quality);



        // Tween 플러그인 사용을 위한 초기 설정.
        //DOTween.Init();
        //WebSocketBase.instance.ConnectChatServer("ws://192.168.0.53:9998/");

        // 화면안꺼지도록 하기
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        UIMgr.instance.Clear(UIMgr.UIType.Default, false);

        StartConsoleView();

        //if (!SimulationGameInfo.SimulationGameCheck)
        Init_FSM();

//       PNClient.instance.MyAwake(); //초기화

        g_Tran_Cutout  = Shader.Find("Unlit/Transparent Cutout");
        g_Tex_AddColor = Shader.Find("Unlit/Texture_AddColor");

		testData = new TestData ();
        
        Init_UI();
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        NativeHelper.instance.DisableNavUI();
#endif
    }

    //< 화면이 꺼졌다 켜졌을경우 처리
//#if !UNITY_EDITOR && UNITY_ANDROID
    public void OnApplicationPause(bool type)
    {
        if (type)
        {
            PlayerPrefs.SetString("OnApplicationPauseTime", dateTimeToUnixTime(System.DateTime.Now).ToString());
        }
        else
        {
            NativeHelper.instance.DisableNavUI();

            System.DateTime EndTime = UnixTimeToDateTime(uint.Parse(PlayerPrefs.GetString("OnApplicationPauseTime", "0")));
            EndTime = EndTime.AddMinutes(5);
            long tick = (EndTime - System.DateTime.Now).Ticks;
            if (tick <= 0)
            {
                if (instance.CurrState() != _STATE.LOGIN && SceneManager.instance.CurrState() != _STATE.START /*&& SceneManager.instance.CurrState() != _STATE.GAMEREADY*/)
                {
                    string msg = _LowDataMgr.instance.GetStringCommon(1021);
                    string title = _LowDataMgr.instance.GetStringCommon(141);
                    string ok = _LowDataMgr.instance.GetStringCommon(117);
                    AddPopup(0, msg, title, ok, null, null, () => {
                        ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
                        {
                            UITextList.ClearTextList();
                            NetData.instance.InitUserData();
                            NetData.instance.ClearCharIdc();

                            ActionEvent(_ACTION.GO_LOGIN);
                        });

                    }, null, null);
                    //UIMgr.instance.OpenPopup("게임을 장시간 플레이하지 않아,\n타이틀로 돌아갑니다.", () =>
                    //{
                        //SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
                    //}, false);
                }
            }
        }

#if UNITY_ANDROID
        Object[] objects = GameObject.FindObjectsOfType(typeof(SkinnedMeshRenderer));
        foreach (SkinnedMeshRenderer s in objects)
        {
            s.sharedMesh.vertices = s.sharedMesh.vertices;
            s.sharedMesh.colors = s.sharedMesh.colors;
            s.sharedMesh.colors32 = s.sharedMesh.colors32;
            s.sharedMesh.uv = s.sharedMesh.uv;
        }
#endif
    }
    //#endif

    public static int dateTimeToUnixTime(System.DateTime dt)
    {
        var date = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        var unixTimestamp = System.Convert.ToInt32((dt - date.ToLocalTime()).TotalSeconds);

        return unixTimestamp;
    }

    public static System.DateTime UnixTimeToDateTime(uint time)
    {
        System.DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dt = dt.AddSeconds(time).ToLocalTime();

        return dt;
    }

    public bool IsPlayBgm
    {
        get {
            if (optionData == null)
                return true;

            return optionData.Bgm;
        }
    }

    public bool IsPlaySoundFx
    {
        get {
            if (optionData == null)
                return true;

            return optionData.Soundfx;
        }
    }

    public void SetUnitLight(byte r, byte g, byte b, byte a, float intensity)
    {
        GameObject lightObj = null;
        if (TownState.TownActive)
        {
            lightObj = GetState<TownState>().MyHero.LightObj;
        }
        else
        {
            lightObj = G_GameInfo.PlayerController.Leader.LightObj;
        }

        if(lightObj != null)
        {
            Light li = lightObj.GetComponent<Light>();
            li.color = new Color32(r, g, b, a);
            li.intensity = intensity;
        }
    }

    public void SetShadowLight(float stength)
    {
        GameObject go = GameObject.Find("main light");
        if (go == null)
            return;

        Light li = go.GetComponent<Light>();
        li.shadowStrength = stength;
    }

    public void GetUnitLight(ref byte r, ref byte g, ref byte b, ref byte a, ref float intensity)
    {
        GameObject lightObj = null;
        if (TownState.TownActive)
        {
            lightObj = GetState<TownState>().MyHero.LightObj;
        }
        else
        {
            lightObj = G_GameInfo.PlayerController.Leader.LightObj;
        }

        if (lightObj != null)
        {
            Light li = lightObj.GetComponent<Light>();
            r = (byte)(li.color.r*255);
            g = (byte)(li.color.g*255);
            b = (byte)(li.color.b*255);
            a = (byte)(li.color.a*255);

            intensity = li.intensity;
        }
    }

    public void GetShadowLight(ref byte r, ref byte g, ref byte b, ref byte a, ref float intensity)
    {
        GameObject mainlightobj = GameObject.Find("main light");

        if (mainlightobj != null)
        {
            Light li = mainlightobj.GetComponent<Light>();

            r = (byte)(li.color.r * 255);
            g = (byte)(li.color.g * 255);
            b = (byte)(li.color.b * 255);
            a = (byte)(li.color.a * 255);

            intensity = li.intensity;
        }
    }

    public void SetShadowLight(byte r, byte g, byte b, byte a, float intensity)
    {
        GameObject mainlightobj = GameObject.Find("main light");

        if (mainlightobj != null)
        {
            Light li = mainlightobj.GetComponent<Light>();
            li.color = new Color32(r, g, b, a);
            li.intensity = intensity;
        }
    }

    public void GetShadowLight(ref float stength)
    {
        GameObject go = GameObject.Find("main light");
        if (go == null)
            return;

        Light li = go.GetComponent<Light>();
        stength = li.shadowStrength;
    }

	public TestData testData;
	public class TestData{
		public bool bSingleSceneTestStart;
		public bool bQuestTestStart;
		public bool bCutSceneTest;
		public int nTestStageId; // 101, 102, 103..
		public int nextStageId;

		public TestData()
		{
			nTestStageId = 101;
			bSingleSceneTestStart = false;
			bQuestTestStart = false;
		}
	}

	public string NumberToString(ushort val){
		return val.ToString ();
		//return val.ToString(); // ToString("#,##");
	}

	public string NumberToString(ulong val){
		return val.ToString ();
		//return val.ToString(); // ToString("#,##");
	}

	public string NumberToString(uint val){
		return val.ToString ();
		//return val.ToString(); // ToString("#,##");
	}
	public string NumberToString(int val){
		return val.ToString ();
		//nowLevelLowData.CostGold == ( 0 ? "0" : nowLevelLowData.CostGold.ToString("#,##") );
		//return val == 0 ? "0" : val.ToString(); // ToString("#,##");
	}


	public System.Diagnostics.Stopwatch sw = new Stopwatch();

	public long showStopWatchTimer(string msg){

		UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> "+msg+" " + sw.ElapsedMilliseconds / 1000f);

		return sw.ElapsedMilliseconds;
		
	}

}