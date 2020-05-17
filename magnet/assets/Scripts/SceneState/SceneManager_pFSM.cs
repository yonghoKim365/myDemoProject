using UnityEngine;
using System.Collections;

public enum _STATE
{
    START = 0,
    LOGIN,
    SINGLE_GAME,
    //INFINITE_GAME,
    //PVP_GAME,
    //SPARRING_GAME,
    RAID_GAME,
    FREEFIGHT_GAME,
    SPECIAL_GAME,
    ARENA_GAME,
    TOWN,
    COLOSSEUM,
    MULTI_RAID,
	TOWN_CUTSCENE,
    //GAMEREADY,
    //SKILL_SIMULATION,
    SELECT_HERO, //영웅선택스테이트
    TOWER_GAME,
    TUTORIAL_GAME,//튜토리얼
	BOSS_SCENE_TEST,
	SPAWN_TEST,
	DEVILDOM_TEST
}

public enum _ACTION
{
    GO_LOGIN,//로그인
    GO_SELECT,//캐릭터 선택
    GO_SHOP,//상점
    GO_RETRY,//재도전
    GO_NEXT,//다음지역
    PLAY_SINGLE,//싱글플레이
    PLAY_FREEFIGHT,//난투장플레이
    SPECIAL_STAGE,//골드, 경험치
    PLAY_ARENA, //1vs1 PVP
    PLAY_TOWER,//마계의 탑
    PLAY_RAID,
    PLAY_TUTORIAL,//튜토리얼
    GO_LOGO,//로고로 이동
	GO_CUTSCENE,
    /*
    PLAY_INFINITE,
    PLAY_PVP,
    PLAY_TEAMPVP,
    PLAY_SPARRING,
    PLAY_RAID,
    GO_SPARRING_NEXT,
    */
    //TEST,
    GO_TOWN,//마을가기
    //GO_MINE,//광산가기
    //GO_MINE_PVP,//광산약탈
    GO_MAP,//전투지역선택화면가기
    //GO_RESTART,//다시시작
    //GO_GUILDWAR,//길드전
    GO_SECRETDUNGEON,//비밀던전
    //GO_FRIEND_TOWN,//친구마을가기

    //GO_UnitManagerPanel,//유닛관리창가기
    //GO_InventoryPanel,//인벤토리가기
    GO_UnitSummonPanel,//유닛뽑기가기
}

public partial class SceneManager
{
    FSM.FSM<_ACTION, _STATE, SceneManager> FSM = null;
    public bool IsYieldAction {
        set;
        get;
    }

    void Init_FSM()
    {
        FSM = new FSM.FSM<_ACTION, _STATE, SceneManager>( this );

        InitFSM_ForClient();
    }

    void InitFSM_ForClient()
    {
        FSM.AddState( _STATE.START, gameObject.AddComponent<StartState>() );
        FSM.AddState( _STATE.LOGIN, gameObject.AddComponent<LoginState>() );
        FSM.AddState(_STATE.SELECT_HERO, gameObject.AddComponent<SelectHeroState>());
        FSM.AddState( _STATE.SINGLE_GAME, gameObject.AddComponent<SingleGameState>() );           // 싱글게임용        
        FSM.AddState( _STATE.FREEFIGHT_GAME, gameObject.AddComponent<FreeFightGameState>() );      // 난투장용
        FSM.AddState( _STATE.TOWN, gameObject.AddComponent<TownState>() ); // ppzz2
        FSM.AddState(_STATE.SPECIAL_GAME, gameObject.AddComponent<SpecialGameState>());      // 골드던전
        FSM.AddState(_STATE.ARENA_GAME, gameObject.AddComponent<ArenaGameState>());      // 골드던전
        FSM.AddState(_STATE.RAID_GAME, gameObject.AddComponent<RaidGameState>());      // 골드던전
        FSM.AddState(_STATE.TOWER_GAME, gameObject.AddComponent<TowerGameState>());      // 마계의탑
        FSM.AddState(_STATE.TUTORIAL_GAME, gameObject.AddComponent<TutorialGameState>());      
		FSM.AddState(_STATE.BOSS_SCENE_TEST, gameObject.AddComponent<BossSceneTestState>());
		FSM.AddState(_STATE.SPAWN_TEST, gameObject.AddComponent<SpawnTestState>());
		FSM.AddState(_STATE.DEVILDOM_TEST, gameObject.AddComponent<DevilDomTestState>());
		FSM.AddState(_STATE.TOWN_CUTSCENE, gameObject.AddComponent<CutSceneState>());

		//FSM.AddState( _STATE.INFINITE_GAME, gameObject.AddComponent<InfiniteGameState>() );
		//FSM.AddState( _STATE.PVP_GAME, gameObject.AddComponent<PVPGameState>() );
		//FSM.AddState( _STATE.SPARRING_GAME, gameObject.AddComponent<SparringGameState>() );
		//FSM.AddState( _STATE.RAID_GAME, gameObject.AddComponent<RaidGameState>() );
		//FSM.AddState(_STATE.GAMEREADY, gameObject.AddComponent<GameReadyState>());

        // State Transition 설정
        {
            // for ReadyState
            FSM.RegistEvent( _STATE.START, _ACTION.GO_NEXT, _STATE.LOGIN );

            // for LoinState
            FSM.RegistEvent( _STATE.LOGIN, _ACTION.GO_TOWN, 	_STATE.TOWN );
            //FSM.RegistEvent( _STATE.LOGIN, _ACTION.GO_NEXT, _STATE.SKILL_SIMULATION);     //본게임에서는 쓰이지 않고 스킬 시뮬레이션 씬에서만 쓰임
            //FSM.RegistEvent( _STATE.LOGIN, _ACTION.PLAY_SINGLE, _STATE.SINGLE_GAME);     //< 튜토리얼 전투
            FSM.RegistEvent(_STATE.LOGIN, _ACTION.GO_SELECT, 	_STATE.SELECT_HERO);           //캐릭터 선택
            FSM.RegistEvent(_STATE.LOGIN, _ACTION.GO_LOGO,		_STATE.START);           //로고로 이동
            FSM.RegistEvent(_STATE.LOGIN, _ACTION.PLAY_TUTORIAL,_STATE.TUTORIAL_GAME);           //튜토리얼 이동
			FSM.RegistEvent(_STATE.LOGIN, _ACTION.GO_CUTSCENE, 	_STATE.TOWN_CUTSCENE);   // cutscene

            FSM.RegistEvent(_STATE.LOGIN, _ACTION.PLAY_FREEFIGHT, _STATE.FREEFIGHT_GAME);           //튜토리얼 이동

            // for SelectHeroState
            FSM.RegistEvent(_STATE.SELECT_HERO, _ACTION.GO_TOWN, _STATE.TOWN);              //캐릭선택 후 마을로
            FSM.RegistEvent(_STATE.SELECT_HERO, _ACTION.GO_LOGIN, _STATE.LOGIN);           //캐릭터 선택


            // for TownState
            FSM.RegistEvent( _STATE.TOWN, _ACTION.PLAY_SINGLE, _STATE.SINGLE_GAME );        // 싱글게임
            //FSM.RegistEvent( _STATE.TOWN, _ACTION.PLAY_INFINITE, _STATE.INFINITE_GAME );    // 무한대전
            //FSM.RegistEvent( _STATE.TOWN, _ACTION.PLAY_PVP, _STATE.PVP_GAME );              // PVP
            //FSM.RegistEvent( _STATE.TOWN, _ACTION.PLAY_SPARRING, _STATE.SPARRING_GAME );    // 대련장(Sparring)
            //FSM.RegistEvent( _STATE.TOWN, _ACTION.PLAY_RAID, _STATE.RAID_GAME );            // 보스 레이드
            FSM.RegistEvent(_STATE.TOWN, _ACTION.PLAY_FREEFIGHT, _STATE.FREEFIGHT_GAME);     // 난투장
            FSM.RegistEvent(_STATE.TOWN, _ACTION.SPECIAL_STAGE, _STATE.SPECIAL_GAME);        // 골드
            FSM.RegistEvent(_STATE.TOWN, _ACTION.PLAY_ARENA, _STATE.ARENA_GAME);        // 골드
            FSM.RegistEvent(_STATE.TOWN, _ACTION.PLAY_RAID, _STATE.RAID_GAME);        // 레이드
            FSM.RegistEvent(_STATE.TOWN, _ACTION.PLAY_TOWER, _STATE.TOWER_GAME);        // 마계의 탑
            FSM.RegistEvent(_STATE.TOWN, _ACTION.PLAY_TUTORIAL, _STATE.TUTORIAL_GAME);            //튜토리얼

            FSM.RegistEvent( _STATE.TOWN, _ACTION.GO_LOGIN, _STATE.LOGIN);                  //< 로그인화면으로 이동
            FSM.RegistEvent(_STATE.TOWN, _ACTION.GO_SELECT, _STATE.SELECT_HERO);            //캐릭선택화면으로 이동

            // for SingleGameState
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_SHOP, _STATE.TOWN );
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_RETRY, _STATE.TOWN );
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_SECRETDUNGEON, _STATE.TOWN);
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_NEXT, _STATE.TOWN);
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_UnitSummonPanel, _STATE.TOWN);
            //FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_RESTART, _STATE.GAMEREADY );
            //FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.PLAY_SINGLE, _STATE.GAMEREADY);
            FSM.RegistEvent( _STATE.SINGLE_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동
            /*
            // for InfiniteGameState
            FSM.RegistEvent( _STATE.INFINITE_GAME, _ACTION.GO_NEXT, _STATE.TOWN );
            FSM.RegistEvent( _STATE.INFINITE_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent(_STATE.INFINITE_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.INFINITE_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent(_STATE.INFINITE_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                  //< 로그인화면으로 이동

            // for PVPGameState
            FSM.RegistEvent( _STATE.PVP_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.PLAY_PVP, _STATE.GAMEREADY);
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.GO_FRIEND_TOWN, _STATE.TOWN);
            FSM.RegistEvent( _STATE.PVP_GAME, _ACTION.GO_NEXT, _STATE.TOWN );
            FSM.RegistEvent( _STATE.PVP_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.GO_MINE, _STATE.TOWN);
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.GO_MINE_PVP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.GO_GUILDWAR, _STATE.TOWN);
            FSM.RegistEvent(_STATE.PVP_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                       //< 로그인화면으로 이동

            // for SparringGameState
            FSM.RegistEvent( _STATE.SPARRING_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent( _STATE.SPARRING_GAME, _ACTION.GO_NEXT, _STATE.TOWN );
            FSM.RegistEvent( _STATE.SPARRING_GAME, _ACTION.GO_SPARRING_NEXT, _STATE.TOWN );
            FSM.RegistEvent(_STATE.SPARRING_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                  //< 로그인화면으로 이동
            FSM.RegistEvent(_STATE.SPARRING_GAME, _ACTION.PLAY_SPARRING, _STATE.GAMEREADY);

            // for RaidGameState
            FSM.RegistEvent( _STATE.RAID_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent( _STATE.RAID_GAME, _ACTION.GO_NEXT, _STATE.TOWN );
            FSM.RegistEvent( _STATE.RAID_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.PLAY_RAID, _STATE.GAMEREADY);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                      //< 로그인화면으로 이동
            */

            // for FreeFightGameState
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_TOWN, _STATE.TOWN );
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_SHOP, _STATE.TOWN);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_SECRETDUNGEON, _STATE.TOWN);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_NEXT, _STATE.TOWN);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_UnitSummonPanel, _STATE.TOWN);
            //FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_RESTART, _STATE.GAMEREADY);
            //FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.PLAY_FREEFIGHT, _STATE.GAMEREADY);
            FSM.RegistEvent( _STATE.FREEFIGHT_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동

            // for GoldDungeon
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_TOWN, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_SHOP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_SECRETDUNGEON, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_NEXT, _STATE.TOWN);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_UnitSummonPanel, _STATE.TOWN);
            //FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_RESTART, _STATE.GAMEREADY);
            //SPACIAL_GAMEFSM.RegistEvent(_STATE.SPACIAL_GAME, _ACTION.PLAY_GOLD_DUNGEON, _STATE.GAMEREADY);
            FSM.RegistEvent(_STATE.SPECIAL_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동


            //ArenaGame - 1vs1
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_TOWN, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_SHOP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_SECRETDUNGEON, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_NEXT, _STATE.TOWN);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_UnitSummonPanel, _STATE.TOWN);
            //FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_RESTART, _STATE.GAMEREADY);
            //FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.PLAY_FREEFIGHT, _STATE.GAMEREADY);
            FSM.RegistEvent(_STATE.ARENA_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동


            // for Raid
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_TOWN, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_SHOP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_RETRY, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_SECRETDUNGEON, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_NEXT, _STATE.TOWN);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_UnitSummonPanel, _STATE.TOWN);
            //FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_RESTART, _STATE.GAMEREADY);
            //SPACIAL_GAMEFSM.RegistEvent(_STATE.SPACIAL_GAME, _ACTION.PLAY_GOLD_DUNGEON, _STATE.GAMEREADY);
            FSM.RegistEvent(_STATE.RAID_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동



            //< 
            //FSM.RegistEvent(_STATE.GAMEREADY, _ACTION.PLAY_SINGLE, _STATE.SINGLE_GAME);
            //FSM.RegistEvent(_STATE.GAMEREADY, _ACTION.PLAY_PVP, _STATE.PVP_GAME);
            //FSM.RegistEvent(_STATE.GAMEREADY, _ACTION.PLAY_RAID, _STATE.RAID_GAME);
            //FSM.RegistEvent(_STATE.GAMEREADY, _ACTION.PLAY_SPARRING, _STATE.SPARRING_GAME);
            //            FSM.RegistEvent(_STATE.GAMEREADY, _ACTION.PLAY_FREEFIGHT, _STATE.FREEFIGHT_GAME);      //<---???

            //for Tower
            FSM.RegistEvent(_STATE.TOWER_GAME, _ACTION.GO_TOWN, _STATE.TOWN);
            FSM.RegistEvent(_STATE.TOWER_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.TOWER_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);

            //for Tutorial
            FSM.RegistEvent(_STATE.TUTORIAL_GAME, _ACTION.GO_TOWN, _STATE.TOWN);
            FSM.RegistEvent(_STATE.TUTORIAL_GAME, _ACTION.GO_MAP, _STATE.TOWN);
            FSM.RegistEvent(_STATE.TUTORIAL_GAME, _ACTION.GO_LOGIN, _STATE.LOGIN);                   //< 로그인화면으로 이동
			FSM.RegistEvent(_STATE.TUTORIAL_GAME, _ACTION.GO_CUTSCENE, _STATE.TOWN_CUTSCENE);   // cutscene

			FSM.RegistEvent(_STATE.TOWN_CUTSCENE, _ACTION.GO_TOWN, _STATE.TOWN);   // cutscene
        }

#if UNITY_EDITOR
		if (GameObject.Find("CutSceneTestManager")!= null){
			//FSM.Current_State = _STATE.TOWN;
			//FSM.Enable (_STATE.SINGLE_GAME);
			FSM.ChangeState(_STATE.BOSS_SCENE_TEST);
		}
		else if (GameObject.Find("SpawnTestManager")!= null){
			FSM.ChangeState(_STATE.SPAWN_TEST);
		}
		else if (GameObject.Find("DevilDomTestManager")!= null){
			FSM.ChangeState(_STATE.DEVILDOM_TEST);
		}
		else{
			FSM.Enable( _STATE.START );
		}
#else
		FSM.Enable( _STATE.START );
#endif
    }

    public _STATE CurrState()
    {
        if (FSM == null)
            return _STATE.START;

        return FSM.Current_State;
    }

    public SceneStateBase CurrentStateBase()
    {
        return (SceneStateBase)FSM.Current();
    }

    public T GetState<T>() where T : SceneStateBase
    {
        return gameObject.GetComponent<T>();
    }

    public void ActionEvent(_ACTION NewAction)
    {
        //SceneManager.isRTNetworkMode = GAME_MODE.NONE;
        IsRTNetwork = false;

        //LobbyActionEventCheck = false;
        FSM.ChangeState( NewAction );
    }

    public _STATE ChangeState;
    public _ACTION ChangeAction;
    //public static bool LobbyActionEventCheck = false;
    public void LobbyActionEvent(_STATE State, _ACTION Action, UI_OPEN_TYPE openType)
    {
        ChangeState = State;
        ChangeAction = Action;
        IsYieldAction = true;
        UiOpenType = openType;

        NetworkClient.instance.SendPMsgReturnMainMapC();
    }
    
    public void TownAction()
    {
        if (!IsYieldAction) {
			if (NeedPlayTownCutScene()){
				ActionEvent (_ACTION.GO_CUTSCENE);
			}
			else{
				ActionEvent (_ACTION.GO_TOWN);
			}
		}
        else
        {
            FSM.ChangeState(ChangeAction);
            StartCoroutine(StateEvent(ChangeState, ChangeAction));
        }

		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("go Town");
    }

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
		bPlayCutScene = false;
		#endif

		return bPlayCutScene;
		
	}

    public void TutorialAction()
    {
        ShowLoadingTipPanel(true, GAME_MODE.TUTORIAL, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(false);//파트너없다.

            TutorialGameState.StageName = "1zone";
            TutorialGameState.lastSelectStageId = 1;
            TutorialGameState.verifyToken = 1023;

            TutorialGameState.IsTutorial = true;
            ActionEvent(_ACTION.PLAY_TUTORIAL);

            //base.GotoInGame();
        });
    }
    
    IEnumerator StateEvent(_STATE State, _ACTION Action)
    {
        WaitForSeconds waitfor = new WaitForSeconds(0.1f);

        while(true)
        {
            //< UI관련 로딩이 끝났을때까지 대기
            if (TownState.IsUILoad && TownState.IsMapLoad)// && StructureMgr.instance.IsLoadSuccess)
            {
                yield return waitfor;
                break;
            }
                
            yield return null;
        }

        NetData.instance.bEventPopupCheck = false;
        //yield return waitfor;
        switch (Action)
        {
            case _ACTION.GO_TOWN://마을로 이동
                //UIMgr.OpenTown();
                //yield return waitfor;

                //yield return waitfor;
                break;
            case _ACTION.GO_MAP://해당 모드에 맞는 UI띄워준다.
                if (State == _STATE.ARENA_GAME)
                    UIMgr.OpenArenaPanel();
                else if (State == _STATE.FREEFIGHT_GAME)
                    UIMgr.OpenDogFight();
                else if (State == _STATE.COLOSSEUM)
                    UIMgr.OpenColosseumPanel(0);
                //else if (State == _STATE.MULTI_RAID)
                //    UIMgr.OpenSpecial();
                //else if (State == _STATE.RAID_GAME)
                //    UIMgr.OpenSpecial();
                //else if (State == _STATE.SPECIAL_GAME)
                //    UIMgr.OpenSpecial();
                else if(State == _STATE.MULTI_RAID 
                    || State == _STATE.RAID_GAME 
                    || State == _STATE.SPECIAL_GAME )
                    UIMgr.OpenDungeonPanel();
                else if (State == _STATE.TOWER_GAME)
                    UIMgr.OpenTowerPanel();
                else
                    UIMgr.OpenChapter(null);

                UIBasePanel townPanel = UIMgr.GetTownBasePanel();
                if(townPanel != null)
                    townPanel.Hide();

                //UiOpenType = UI_OPEN_TYPE.NONE;
                break;

            default://혹시 모를 예외처리. 원하는 처리 정의 해줘야함.
                //UIMgr.OpenTown();
                //yield return waitfor;
                Debug.LogError(string.Format("UnDefined type {0} please defined", Action));
                break;
        }

        //< 로비 액션이 끝났을때 처리할것들
        yield return new WaitForSeconds(0.1f);
        ShowLoadingTipPanel(false);
        IsYieldAction = false;
        NetData.instance.bEventPopupCheck = true;
        
        yield return null;
    }
    
}