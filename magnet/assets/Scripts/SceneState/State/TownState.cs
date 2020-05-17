using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Sw;

public class TownState : SceneStateBase
{

    public struct TownNickData
    {
        public GameObject Obj;
        public string Nick;
        public uint Prefix;
        public uint Suffix;

        public TownNickData(GameObject go, string nick, uint pre, uint suf)
        {
            Obj = go;
            Nick = nick;
            Prefix = pre;
            Suffix = suf;
        }
    }
    public List<TownNickData> TownNickList = new List<TownNickData>();
    
    public struct ActivityInfo
    {
        public uint GetPoint;
        public uint ResetTime;
        public uint RewardInfo;//2진수 값 1,3,7,13,21
        public bool IsSend;
        public ActivityInfo(uint point, uint time, uint reward)
        {
            GetPoint = point;
            ResetTime = time;
            RewardInfo = reward;
            IsSend = false;
        }
    }

    //public ActivityInfo _ActivityInfo;

    //임시로 타운유닛 매니저를 가지고 있자 - 나는 일단 패스
    private Dictionary<long, TownUnit> townUnitList = new Dictionary<long, TownUnit>();
    
	private bool bAddtiveSceneLoadFinish = false;

    public bool IsEndSceneLoad
    {
        get {
            return bAddtiveSceneLoadFinish;
        }
    }

    public TownUnit GetTownUnit(long UllRoleId)
    {
        if(townUnitList.ContainsKey(UllRoleId))
        {
            return townUnitList[UllRoleId];
        }

        return null;
    }
    
    public void TownUserLeave(PMsgMapLeaveMapS leaveInfo)
    {
        TownUnit unit = null;
        if (townUnitList.TryGetValue(leaveInfo.UllRoleId, out unit))
        {
            //다로딩이 완료된애만
            string path = "Effect/_UI/_INGAME/Fx_IN_goout_01";
            GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            effGo.transform.parent = unit.transform;
            effGo.transform.localScale = unit.transform.localScale;
            effGo.transform.localPosition = Vector3.zero;

            TempCoroutine.instance.FrameDelay(0.5f, () =>
            {
                Destroy(unit.gameObject);
            });

            townUnitList.Remove(leaveInfo.UllRoleId);
        }
        else
        {
            //내꺼는 보통 없다
            //Debug.LogError("not found TownUnit error " + leaveInfo.UllRoleId);
        }
        /*
        if(townUnitList.ContainsKey(leaveInfo.UllRoleId))
        {
            Destroy(townUnitList[leaveInfo.UllRoleId].gameObject);
            townUnitList.Remove(leaveInfo.UllRoleId);
        }
        */
    }

	public void TownUserEnterAsync(PMsgMapRoleInfoS anotherUserInfo)
	{
		StartCoroutine (TownUserEnter (anotherUserInfo));
	}

    public IEnumerator TownUserEnter(PMsgMapRoleInfoS anotherUserInfo)
    {
		

        //이미 있는경우는 추가하지 않는다.
        TownUnit unit = null;
        if (townUnitList.TryGetValue(anotherUserInfo.UllRoleId, out unit))
        {
            uint head = 0, cloth = 0, weapon = 0;
            int equipCount = anotherUserInfo.UnEquipmentId.Count;
            for (int i = 0; i < equipCount; i++)
            {
                int equipId = anotherUserInfo.UnEquipmentId[i];
                if (equipId <= 0)
                    continue;

                Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equipId);
                if (equipInfo == null)
                {
                    Debug.LogError(string.Format("friend equip item not found lowdataID={0} error", equipId));
                    continue;
                }

                if (equipInfo.UseParts == (byte)ePartType.CLOTH)
                    cloth = (uint)equipId;
                //else if (equipInfo.UseParts == (byte)ePartType.HELMET)
                //head = (uint)equipId;
                else if (equipInfo.UseParts == (byte)ePartType.WEAPON)
                    weapon = (uint)equipId;
            }
            
            unit.ChangeSkin(anotherUserInfo.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE, (uint)anotherUserInfo.UnCostumeId, head, cloth, weapon );
            unit.ChangeTitle(anotherUserInfo.UnTitlePrefix, anotherUserInfo.UnTitleSuffix);
            /*
            UIBasePanel readyPopup = UIMgr.GetUIBasePanel("UIPopup/ReadyPopup");
            if (readyPopup != null)
            {
                if ((readyPopup as ReadyPopup).IsRoom)
                {
                    (readyPopup as ReadyPopup).OnChangeUserInfo(anotherUserInfo.UllRoleId);
                }
            }
            */
			yield break;
        }

        if (NetData.instance.GetUserInfo().GetCharUUID() == (ulong)anotherUserInfo.UllRoleId)
			yield break;

        NetData.RecommandFriendData friend = new NetData.RecommandFriendData();

        friend.c_usn = (ulong)anotherUserInfo.UllRoleId;
        friend.character_id = (uint)anotherUserInfo.UnType;
        friend.nickname = anotherUserInfo.SzName;
        friend.level = (uint)anotherUserInfo.UnLevel;
        friend.costume_id = (uint)anotherUserInfo.UnCostumeId;
        friend.isHideCostume = (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE == anotherUserInfo.UnCostumeShowFlag;
        friend.Prefix = anotherUserInfo.UnTitlePrefix;
        friend.Sufffix = anotherUserInfo.UnTitleSuffix;

        if (friend.character_id < 10000)
        {
            friend.character_id = 12000;
        }

        if (friend.costume_id <= 0)
        {
            Debug.LogError(string.Format("RoleID: {0}, costume idx = {1} error charIdx={2}", anotherUserInfo.UllRoleId, friend.costume_id, friend.character_id));

            if (friend.character_id == 11000)
            {
                friend.costume_id = 100;
            }
            else if (friend.character_id == 12000)
            {
                friend.costume_id = 110;
            }
            else if (friend.character_id == 13000)
            {
                friend.costume_id = 120;
            }

        }
        
        int count = anotherUserInfo.UnEquipmentId.Count;
        for (int i = 0; i < count; i++)
        {
            int equipId = anotherUserInfo.UnEquipmentId[i];
            if (equipId <= 0)
                continue;

            Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equipId);
            if(equipInfo == null)
            {
                Debug.LogError(string.Format("friend equip item not found lowdataID={0} error", equipId) );
                continue;
            }

            if (equipInfo.UseParts == (byte)ePartType.CLOTH)
                friend.EquipClothItemIdx = (uint)equipId;
            //else if (equipInfo.UseParts == (byte)ePartType.HELMET)
                //friend.EquipHeadItemIdx = (uint)equipId;
            else if (equipInfo.UseParts == (byte)ePartType.WEAPON)
                friend.EquipWeaponItemIdx = (uint)equipId;
        }

        /*
        if (friend.character_id == 11000)
        {
            friend.costume_id = (uint)100;
        }
        else if (friend.character_id == 12000)
        {
            friend.costume_id = (uint)110;
        }
        else if (friend.character_id == 13000)
        {
            friend.costume_id = (uint)120;
        }
        else
        {
            Debug.LogError(string.Format("error not found character id {0}, is setting default id:{1}", friend.character_id, 11000));
            friend.character_id = 11000;
            friend.costume_id = (uint)100;
        }
        */

        object[] initialData = new object[4] { 0, 0, (int)UnitType.TownNpc, friend };

        //TownUnit tu = TownUnit.CreateTownUnit(anotherUserInfo.UnPosX, anotherUserInfo.UnPosY, initialData);
		TownUnit tu = null;
		yield return StartCoroutine(TownUnit.CreateTownUnitAsync (anotherUserInfo.UnPosX, anotherUserInfo.UnPosY, (retval)=>{ tu = retval; }, initialData));
		
        tu.ChangeState(UnitState.Idle);

        if( !townUnitList.ContainsKey(anotherUserInfo.UllRoleId))
            townUnitList.Add(anotherUserInfo.UllRoleId, tu);
        else
        {
            Debug.LogError(string.Format("is already key error key={0}, name={1}, isMyChar={2}"
                , anotherUserInfo.UllRoleId, anotherUserInfo.SzName, ((ulong)anotherUserInfo.UllRoleId)==NetData.instance.GetUserInfo()._charUUID) );
        }

        UIBasePanel basePanel = UIMgr.GetTownBasePanel();
        if (basePanel != null)
        {
            (basePanel as TownPanel).CreateHeadObjet(tu.gameObject, friend.nickname, friend.Prefix, friend.Sufffix, false);

            //string path = string.Format("Effect/_UI/_INGAME/{0}", name);
            CreateAccessEff(tu.gameObject);
        }
        else
        {
            TownNickList.Add(new TownNickData(tu.gameObject, friend.nickname, friend.Prefix, friend.Sufffix) );
        }
    }
    
    public void CreateAccessEff(GameObject go)
    {
        string path = "Effect/_UI/_INGAME/Fx_IN_enter_01";
        GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
        effGo.transform.parent = go.transform;
        effGo.transform.localScale = go.transform.localScale;
        effGo.transform.localPosition = Vector3.zero;
    }


    //< 현재 타운인지 아닌지를 체크하기위함
    public static bool TownActive = false;
    //public static Vector3 SavePosition = Vector3.zero;


    public override void OnEnter(System.Action callback)
    {
        IsUILoad = false;

        //LoadAllData();

        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () => 
        {
            UIHelper.SetMainCameraActive(true);
            CameraManager.instance.RtsCamera.Reset();

            base.OnEnter(callback);

            if (!PoolManager.Pools.ContainsKey("Effect"))
                PoolManager.Pools["Effect"].DespawnAll();
            if (!PoolManager.Pools.ContainsKey("Projectile"))
                PoolManager.Pools["Projectile"].DespawnAll();

            CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<ColorCorrectionCurves>().enabled = true;
            CameraManager.instance.mainCamera.clearFlags = CameraClearFlags.Skybox;

            LoadLevelAsync("maintown");

            //CameraManager.instance.ActiveCamEff(true);

            //NetData.instance.AutoStartStageReset();
        });

        Time.timeScale = 1;
    }


    public override void OnExit(System.Action callback)
    {
        //캐릭터 정보도 다삭제
        var enumerator = townUnitList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Destroy(enumerator.Current.Value);
        }
        townUnitList.Clear();

		if (CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<ColorCorrectionCurves> ()) {
			CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<ColorCorrectionCurves> ().enabled = false;
		}
        CameraManager.instance.mainCamera.clearFlags = CameraClearFlags.Skybox;

        //< 마을에서 사용했던 리소스들은 삭제
        AssetbundleLoader.ClearAssetList();
        SoundManager.instance.Clean();

        effectPool.SetDestroy();

        UIHelper.SetMainCameraActive(true);
        base.OnExit( callback );

        TownActive = false;

        //StructureMgr.instance.DemolishAll();
        //NetData.instance.VillageUnitDic.Clear();

    }

    public static bool IsUILoad = false;

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName != "maintown")
            return;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		sw.Start ();
		long time1 = SceneManager.instance.showStopWatchTimer ("MainTown scene loaded,  TownState.OnLevelWasLoaded start");

		ResourceMgr.Clear ();

		bAddtiveSceneLoadFinish = false;
	
        NativeHelper.instance.DisableNavUI();

        TownActive = true;

        //< 인게임에서 사용했던 리소스들도 삭제
        AssetbundleLoader.ClearAssetList();
        SoundManager.instance.Clean();

        //< 맵 로드
        IsMapLoad = true;

        SceneSetting();

        SetupMainCamera(true, GAME_MODE.NONE);
		
        effectPool = PoolManager.Pools["Effect"];

        //CameraManager.instance.mainCamera.GetComponent<CameraMouseZoom>().SetInitPos();

		MapEnvironmentSetting(Application.loadedLevelName); // move to here.

		sendQueryForHeroAndTown();
		
		// 여기서 컷씬플레이여부 비교판단 해야함.
//		if (NeedPlayTownCutScene ()) {
//			return;
//		}

		StartCoroutine (OnLevelWasLoaded2Part1 ());
		StartCoroutine (OnLevelWasLoaded2Part2 ());

		long time2 = SceneManager.instance.showStopWatchTimer ("TownState.OnLevelWasLoaded end");
		Debug.Log ("<color=green>[StopWatch]</color> OnLevelLoaded take time " + ((time2 - time1) / 1000f));
		sw.Stop ();
	}

	bool bLoadedMyTownUnit;
	IEnumerator OnLevelWasLoaded2Part1(){

		bLoadedMyTownUnit = false;

		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		sw.Start ();

		yield return StartCoroutine (CreateMyTownUnitAsync ());

		Debug.Log ("<color=green>[StopWatch]</color> OnLevelWasLoaded2Part1 :" + sw.ElapsedMilliseconds / 1000f);

		bLoadedMyTownUnit = true;
	}

	IEnumerator OnLevelWasLoaded2Part2(){
		
		//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		//sw.Start ();

		//CreateMyTownUnit(); // take 1.9 sec, // prev location, but move to OnLevelWasLoaded2Part1 
		//StartCoroutine (CreateMyTownUnitAsync ());
		
		while (!bLoadedMyTownUnit) { //yield return StartCoroutine (CreateMyTownUnitAsync ()); // CreateMyTownUnit();
			yield return null;
		}



        UIMgr.instance.UICamera.enabled = false;
        //UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> OnLevelWasLoaded2Part2 5: " + sw.ElapsedMilliseconds / 1000f);

        //TownNpcMgr.instance.Init(); // 원래 순서는 여기

        ShaderHelper.InitMapPostProcess(LayerMask.NameToLayer("Obstacle"), "Custom/DiffuseAlpha", "_Texture");

		UIMgr.OpenTown ();
		
		//sendQueryForHeroAndTown(); // 원래 순서는 여기
		
		//LoadOtherPlayers(); // 원래 순서는 여기
		
		MissionLoad();

		//UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> OnLevelWasLoaded2Part2 20: " + sw.ElapsedMilliseconds / 1000f);

		//MapEnvironmentSetting(Application.loadedLevelName); // // 원래 순서는 여기
		
		//라이트셋팅
		setShadowStrength ();

		//NetworkClient.instance.SendPMsgMapEnterMapReadyC(); // 원래 순서는 여기
		
		SceneManager.instance.LoadingTipPanel.changeLoadingBar (1f);
		
		//< 패널들을 미리 로드시켜놓음
		StartCoroutine(TownUILoadUpdate()); // face, head..loadingTip off

		//UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> OnLevelWasLoaded2Part2 30: " + sw.ElapsedMilliseconds / 1000f);
		
		if (InputManager.instance != null) {}//일단 생성

		StartCoroutine (DoLater ()); // TownNpcMgr.init, LoadOtherPlayers, SendPMsgMapEnterMapReadyC
		
		StartCoroutine (LoadAddtiveTownScene ());

		StartCoroutine (WaitAndStartTutorial ());
		
		//UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> OnLevelWasLoaded2Part2 end:" + sw.ElapsedMilliseconds / 1000f);
		//sw.Stop ();

	}
	
	IEnumerator LoadAddtiveTownScene(){

		bAddtiveSceneLoadFinish = false;

		yield return new WaitForSeconds (1f);

		Application.backgroundLoadingPriority = ThreadPriority.Low;

		AsyncOperation async = Application.LoadLevelAdditiveAsync ("maintown_addtive");

		yield return async;

		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

		bAddtiveSceneLoadFinish = true;
		
	}

	IEnumerator WaitAndStartTutorial(){
		while (bAddtiveSceneLoadFinish == false) {
			yield return null;
		}

        //ulong key = NetData.instance.GetUserInfo().GetCharUUID();
        //if (PlayerPrefs.HasKey(string.Format("TutorialData_{0}.json", key)))
        //{
        //    PlayerPrefs.DeleteKey(string.Format("TutorialData_{0}.json", key));
        //}

        //SceneManager.instance.CheckTutorial();

        // condition check and do tutorial here.
        //SceneManager.instance.CurTutorial = TutorialType.INVEN_AUTO_EQUIP;
        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        //SceneManager.instance.CurTutorial = TutorialType.CATEGORY;
        UIMgr.OpenNoticePanel(NoticeType.Max);
        if( !townPanel.IsHidePanel)
        {
            //SceneManager.instance.PopRecommendData();
            (townPanel as TownPanel).OnEndStateLoad();
        }

        UIMgr.instance.UICamera.enabled = true;
    }

    // 씬 생성후 나중에 해도 될일들 처리.
    IEnumerator DoLater(){

		//TownLight = GameObject.Find("Directional light");//라이트가 있다면 꺼주게 한다.
        GameObject light = GameObject.Find("Directional light");
        if (light != null)
            light.SetActive(false);
		
		TownNpcMgr.instance.Init();
        
        StartCoroutine (LoadOtherPlayers ());

		NetworkClient.instance.SendPMsgMapEnterMapReadyC();
		
		yield return null;

	}

    IEnumerator TownUILoadUpdate()
    {

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        while( null == userInfo.GetEquipCostume() || townPanel == null)//서버에서 받아올때까지 대기한다
        {
            yield return new WaitForSeconds(0.1f);
            if(townPanel == null)
                townPanel = UIMgr.GetTownBasePanel();
        }
        
        //while (_ActivityInfo.IsSend)//서버에서 받아올때까지 대기한다
        //{
        //    yield return new WaitForSeconds(0.1f);
        //}
        
        IsUILoad = true;

        //(townPanel as TownPanel).ActivityPoint(_ActivityInfo.GetPoint);
        (townPanel as TownPanel).CreatePcFaceObj();
        //Npc
        //List<InputTownModel> npcList = TownNpcMgr.instance.GetNpcList();
        //if (npcList != null)
        //{
        //    int loopCount = npcList.Count;
        //    for (int i = 0; i < loopCount; i++)
        //    {
        //        (townPanel as TownPanel).CreateHeadObjet(npcList[i].gameObject, npcList[i].NPCName, 0, 0, false);
        //    }
        //}


        //yield return new WaitForSeconds(0.1f);

        if (!SceneManager.instance.IsYieldAction)
            SceneManager.instance.ShowLoadingTipPanel(false);

    }

    /// <summary>
    /// 타운용 유닛
    /// </summary>
    public MyTownUnit MyHero;
    /// <summary>
    /// 맵로드가 끝나면 캐릭터 생성.
    /// </summary>
    void CreateMyTownUnit()
    {
        int regenX;
        int regenY;
        NetworkClient.instance.GetRegenPos(out regenX, out regenY);

        NaviTileInfo.instance.LoadTile("maintown"); // take 0.8sec
        
        MyHero = MyTownUnit.CreateTownControllUnit( regenX, regenY ); // take 0.9 sec

        MyHero.m_rUUID = NetData.instance._userInfo._charUUID;
    }

	IEnumerator CreateMyTownUnitAsync()
	{
		int regenX;
		int regenY;
		NetworkClient.instance.GetRegenPos(out regenX, out regenY);
		
		//NaviTileInfo.instance.LoadTile("maintown"); // take 0.8sec

		yield return StartCoroutine (NaviTileInfo.instance.LoadTitleAsync ("maintown"));
		
		yield return StartCoroutine (MyTownUnit.CreateTownControllUnitAsync (regenX, regenY, (retVal) => { MyHero = retVal;}));
		
		MyHero.m_rUUID = NetData.instance._userInfo._charUUID;

		yield return null;
	}

    public void MissionLoad(bool isHideTown = false)
    {
        //UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/TownPanel");

        if (isHideTown)
        {
            UIBasePanel panel = UIMgr.GetTownBasePanel();
            if(panel != null)
                panel.Hide();
            //(basePanel as TownPanel).Hide();
        }

        //미션데이터 초기화
        _LowDataMgr.instance.InitializeMission();

        /*
        {
            UIBasePanel panel = UIMgr.GetTownBasePanel();
            if (panel != null)
                (panel as TownPanel).MissionListSetting();
        }
        */
    }

	void setShadowStrength(){
		float CharLightValue;
		float ShadowStrength;
		_LowDataMgr.instance.GetMapShadowData(Application.loadedLevelName, out CharLightValue, out ShadowStrength);
		GameObject go = GameObject.Find("C_shadow_light");
		if (go != null)
		{
			Light li = go.GetComponent<Light>();
			li.shadowStrength = ShadowStrength;
		}
	}

	private void sendQueryForHeroAndTown(){

		NetworkClient.instance.SendPMsgEquipmentQueryC();
		NetworkClient.instance.SendPMsgItemQueryC();
		NetworkClient.instance.SendPMsgCostumeQueryC();
		NetworkClient.instance.SendPMsgHeroQueryC();
		NetworkClient.instance.SendPMsgTaskQueryInfoC();
		NetworkClient.instance.SendPMsgDailyTaskQueryInfoC();
		NetworkClient.instance.SendPMsgColosseumQueryC();//콜로세움 정보 받아오기
		NetworkClient.instance.SendPMsgMultiBossQueryC();//멀티보스레이드 정보 받아오기
        NetworkClient.instance.SendPMsgStageChapterQueryC(1);//일반
        NetworkClient.instance.SendPMsgStageChapterQueryC(2);//하드
        NetworkClient.instance.SendPMsgArenaRankInfoC();//차관
        

        //NetworkClient.instance.SendPMsgActivePointsQueryInfoC();
        //_ActivityInfo.IsSend = true;

        if (NetData.instance.GetUserInfo().ClearSingleStageDic.Count <= 0)//최초 접속시에만 호출 or 아무것도 없을 경우
		{
            NetworkClient.instance.SendPMsgEquipmentSetQueryC();

            NetworkClient.instance.SendPMsgStageQueryC(1);
            NetworkClient.instance.SendPMsgStageQueryC(2);
            NetworkClient.instance.SendPMsgTitleQueryInfoC();//칭호 정보 갱신
			
			NetworkClient.instance.SendPMsgExpBattleQueryC();//경험치
			NetworkClient.instance.SendPMsgCoinBattleQueryC();//골드
			NetworkClient.instance.SendPMsgTowerBattleQueryC();//마탑
			NetworkClient.instance.SendPMsgBossBattleQueryC(1);//레이드
			NetworkClient.instance.SendPMsgBossBattleQueryC(2);//레이드
			NetworkClient.instance.SendPMsgBossBattleQueryC(3);//레이드

            NetworkClient.instance.SendPMsgRoleActiveSkillListC();
            NetworkClient.instance.SendPMsgRolePassiveSkillListC();
        }
    }

	private IEnumerator LoadOtherPlayers()
    {   
        //서버로 부터 친구 추천 유닛 리스트를 받아온다.
        //로딩이 덜되었을때 받은 유닛 리스트를 만들어준다

		//yield return new WaitForSeconds (2.0f);

//		while (!bAddtiveSceneLoadDone) {
//			yield return null;
//		}

        List<PMsgMapRoleInfoS> townUnitLoadingList = NetworkClient.instance.townUnitLoadingList;

        //Debug.Log("townUnitLoadingList: " + townUnitLoadingList.Count);

        for (int i = 0; i < townUnitLoadingList.Count; i++)
        {
			yield return StartCoroutine(TownUserEnter(townUnitLoadingList[i]));

			yield return new WaitForSeconds (0.5f);
        }

        NetworkClient.instance.townUnitLoadingList.Clear();

		yield return null;
    }

    //Vector3 CameraFirstPos;
    //Quaternion CameraFirstQua;
    //IEnumerator CameraRectSet(float time)
    //{
    //    //yield return new WaitForSeconds(time);

    //    //< 카메라 초기 위치를 저장해둠
    //    //CameraFirstPos = CameraManager.instance.mainCamera.transform.localPosition;
    //    //CameraFirstQua = CameraManager.instance.mainCamera.transform.localRotation;

    //    SceneManager.instance.RemoveIndicator();
    //    //CameraManager.instance.mainCamera.GetComponent<CameraMouseZoom>().MoveRectReset();

    //    UIMgr.instance.UICamera.enabled = true;
    //    yield return null;
    //}
    //void CameraEffectShow()
    //{
    //    CameraScript camscript = CameraManager.instance.mainCamera.gameObject.GetComponent<CameraScript>();
    //    if (null == camscript) return;

    //    SceneManager.instance.AddIndicator(false);
    //    float time = 1.5f;
    //    camscript.InitCamSet(GameObject.Find("maintown"), Quaternion.Euler(
    //            CameraManager.instance.mainCamera.GetComponent<CameraMouseZoom>().ResetSetRot), time);
    //    new Task(CameraRectSet(time + 0.2f));
    //}

    public Dictionary<long, TownUnit> GetTownUnits()
    {
        return townUnitList;
    }

    public static Transform SpawnEffect(string prefabName, Vector3 posAndRot, Quaternion spawnRot, float speed = 1)
    {
        //	    Debug.LogError("2JW : G_GameInfo.SpawnEffect() In - " + prefabName);
        if (!PoolManager.Pools["Effect"].prefabs.ContainsKey(prefabName))
        {
            CreateNewEffectPoolItem(prefabName, () => { SpawnEffect(prefabName, posAndRot, spawnRot, speed); });
            return null;
        }

        Transform spawned;

        if (posAndRot != Vector3.zero)
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName, posAndRot, spawnRot);
        else
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName);

        // 최대 이펙트 스피드는 0.5f배로. 4배이상은 안나오는게 많음.
        speed = Mathf.Clamp(speed, 0.5f, 4f);
        NsEffectManager.AdjustSpeedRuntime(spawned.gameObject, speed);

        return spawned;
    }

    static Dictionary<string, GameObject> TempOriEffectsDic = new Dictionary<string, GameObject>();
    static Dictionary<string, List<System.Action>> EffectLoads = new Dictionary<string, List<System.Action>>();
    static SpawnPool effectPool = null;
    public static bool CreateNewEffectPoolItem(string prefabName, System.Action call)
    {
        //Debug.LogWarning("2JW :  <color=red>Commmons Effect</color> - " + prefabName);
        if (EffectLoads.ContainsKey(prefabName))
        {
            EffectLoads[prefabName].Add(call);
            return false;
        }

        EffectLoads.Add(prefabName, new List<System.Action>());
        EffectLoads[prefabName].Add(call);
        GameObject oriEff = null;
        if (TempOriEffectsDic.ContainsKey(prefabName))
        {
            oriEff = TempOriEffectsDic[prefabName];
        }
        else
        {
            oriEff = ResourceMgr.Load(string.Format("Effect/_Common/{0}", prefabName)) as GameObject;
         
            if (oriEff == null)
            {
                Debug.LogWarning("2JW : not effect <color=red>Commmons Effect</color> - " + prefabName + " : " + oriEff, oriEff);
                EffectLoads[prefabName].Clear();
                EffectLoads.Remove(prefabName);
                return false;
            }
            //Debug.LogWarning("2JW : " + oriEff);
            TempOriEffectsDic.Add(prefabName, oriEff);
        }
        //AssetbundleLoader.GetEffect(prefabName, (obj) =>
        {
            //Debug.LogWarning("2JW : " + _GameInfo);
            //Debug.LogWarning("2JW : " + _GameInfo.effectPool);
            //CreatePoolItem(_GameInfo.effectPool, oriEff);
            
            PrefabPool prefabPool = new PrefabPool(((GameObject)oriEff).transform);
            prefabPool.preloadAmount = 1;
            prefabPool.AddSpawnCount = 3;
            effectPool.CreatePrefabPool(prefabPool, true);

            for (int i = 0; i < EffectLoads[prefabName].Count; i++)
                EffectLoads[prefabName][i]();

            EffectLoads[prefabName].Clear();
            EffectLoads.Remove(prefabName);
        }//, false);

        return false;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
	   /*//존재하지 않는 페널. 골든나이츠에서 사용하던 것같아서 주석처리했습니다.
        if (GUI.Button(new Rect(0, 0, 50, 30), "Edit"))
            UIMgr.Open("GameEditPanel");
	   */
        //GUILayout.BeginArea( new Rect( Screen.width * 0.5f, 0, Screen.width * 0.3f, Screen.height ) );

        //if (GUILayout.Button( "Go Raid" ))
        //    new Task( GoReadyForRaid() );
        //if (GUILayout.Button( "Go Single" ))
        //    new Task( GoReadyForSingle() );
        //if (GUILayout.Button( "Test Single" ))
        //{
        //    GoTestSingle( 1 );
        //}

        //GUILayout.BeginVertical();
        //if (OptionView)
        //{
        //    unitIdStr = GUILayout.TextField(unitIdStr, GUILayout.Width(200));
        //    uint unitId;
        //    if (uint.TryParse(unitIdStr, out unitId))
        //    {
        //        if (GUILayout.Button("NpcGet"))
        //        {
        //            WebSender.instance.P_UNIT_GET(unitId, (response) => { });
        //        }
        //    }


        //    if (uint.TryParse(unitIdStr, out unitId))
        //    {
        //        if (GUILayout.Button("MaterialGet"))
        //        {
        //            WebSender.instance.P_MATERIAL_GET(unitId, 10, (response) => { });
        //        }
        //    }
        //}
        //else
        //{
        //    if (GUILayout.Button("View On"))
        //        OptionView = true;
        //}
    }
    /*
    void GoTestSingle(uint stageID)
    {
        SceneManager.instance.GetState<SingleGameState>().SetStageNum( stageID );
        SingleGameState.lastSelectStageId = stageID;
        SceneManager.instance.ActionEvent( _ACTION.PLAY_SINGLE );
    }
    */
#endif

/*
	bool  NeedPlayTownCutScene (){
		
		bool bPlayCutScene = false;
		if (PlayerPrefs.GetInt (string.Format ("FirstCutSeqPlayed{0}", NetData.instance.GetUserInfo ()._charUUID), 0) == 0) {
			// play first cutscene.
			PlayerPrefs.SetInt (string.Format ("FirstCutSeqPlayed{0}", NetData.instance.GetUserInfo ()._charUUID), 1);
			bPlayCutScene = true;

			if (NetData.instance.GetUserInfo ()._Level != 1){
				bPlayCutScene = false;
			}
		}

#if UNITY_EDITOR
		// for test
		//bPlayCutScene = true;

#endif
		if (bPlayCutScene){
			SceneManager.instance.StartCoroutine( LoadAndPlayCutScene());
			return true;
		}
		
		return false;
		
	}
	
	IEnumerator LoadAndPlayCutScene(){

		//SceneManager.instance.ShowNetProcess("maintown_cutscene");
		
		AsyncOperation async = Application.LoadLevelAdditiveAsync("maintown_cutscene"); // this scene include sequence objects.
		
		async.allowSceneActivation = false;
		
		while (async.progress < 0.9f)
		{
			yield return null;
		}
		
		async.allowSceneActivation = true;
		
		while (!async.isDone)
			yield return null;
		
		yield return new WaitForEndOfFrame();
		
		yield return new WaitForSeconds(0.1f);
		
		//SceneManager.instance.EndNetProcess("maintown_cutscene");

		SceneManager.instance.ShowLoadingTipPanel(false);
		
		string[] CharObjNamesForInactive = new string[2];
		
		if (NetData.instance.GetUserInfo ()._userCharIndex == 11000) {
			CharObjNamesForInactive[0] = "pc_p_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 12000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 13000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_p_cutscene_01";
		}
	
		if (GameObject.Find ("TownCutSceneManager")) {
			TownCutSceneManager tcm = GameObject.Find ("TownCutSceneManager").GetComponent<TownCutSceneManager> (); 
			tcm.InActiveObj(CharObjNamesForInactive);
			tcm.playSeq( ()=>{StartCoroutine( OnLevelWasLoaded2Part1());}, ()=>{StartCoroutine( OnLevelWasLoaded2Part2());});
		}
	}
	*/
}


/// <summary>
/// 에디터에서만 구분해서 볼수있는 로그
/// </summary>
public class LOGEX
{
    public enum LogType { NONE, WARNING, ERROR }
    public enum ColorType { WHITE, RED, YELLOW, GREEN, BLUE, PURPLE, GRAY, BLACK }
    public static void LogEx(string message, LogType _lt, ColorType _ct = ColorType.WHITE, Object _obj = null)
    {
#if UNITY_EDITOR
	   string color = "white";
	   switch (_ct)
	   {
		  case ColorType.WHITE:
			 color = "white";
			 break;
		  case ColorType.BLACK:
			 color = "black";
			 break;
		  case ColorType.RED:
			 color = "red";
			 break;
		  case ColorType.YELLOW:
			 color = "yellow";
			 break;
		  case ColorType.BLUE:
			 color = "blue";
			 break;
		  case ColorType.GREEN:
			 color = "green";
			 break;
		  case ColorType.GRAY:
			 color = "gray";
			 break;
		  case ColorType.PURPLE:
			 color = "purple";
			 break;
	   }
	   string msg = string.Format("<color={0}>{1}</color>", color, message);
	   switch (_lt)
	   {
		  case LogType.NONE:
			 Debug.Log(msg, _obj);
			 break;
		  case LogType.WARNING:
			 Debug.LogWarning(msg, _obj);
			 break;
		  case LogType.ERROR:
			 Debug.LogError(msg, _obj);
			 break;
	   }
#endif//UNITY_EDITOR
    }


}