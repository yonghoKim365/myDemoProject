using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaGameInfo : GameInfoBase
{
    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.ARENA; } }  //<---- 지금 이건 무진장 많아서 나중에 전무 찾아서 수정해 준다.
    
    public ulong RoomNo { set; get; }
    public static bool DisableHit = false; //애도  Unit TakeDamage에서 임시로 사용하고 있는 거 같다.
    //private GameObject[] ActionCam;//처음 시작연출용 카메라 2대

    protected override void InitDatas()
    {
        base.InitDatas();

        //< 컷씬 정보 미리 로드
        CutSceneMgr.LoadCutScene();

        //ActionCam = new GameObject[] {
        //    GameObject.Find("LeftCam"),
        //    GameObject.Find("RightCam")
        //};

        //ActionCam[0].SetActive(false);
        //ActionCam[1].SetActive(false);
    }

    protected override void InitManagers()
    {
        base.InitManagers();

        G_GameInfo.CharacterMgr.allrUUIDDic.Clear();

        //내데이터 만듬
        _PlayerSyncData syncData = NetData.instance._playerSyncData;
        CreatePlayerController(syncData.playerSyncDatas[0], syncData.partnerSyncDatas);

        //상대편 데이터 만듬 - 임시로 나와동일한 캐릭

        Vector3 spawnPos = GetSpawnTransform(eTeamType.Team2) != null ? GetSpawnTransform(eTeamType.Team2).position : Vector3.zero;
        Quaternion spawnRot = GetSpawnTransform(eTeamType.Team2) != null ? GetSpawnTransform(eTeamType.Team2).rotation : Quaternion.identity;

        ////게임 인포를 통한 유닛 생성   
        GameObject EnemyLeader = G_GameInfo.GameInfo.SpawnUnit(1234567, (byte)eTeamType.Team2, syncData.playerSyncDatas[1], spawnPos, spawnRot, false);

        for(int idx=0;idx< syncData.partnerSyncDatas.Count; idx++)
        {
            if (syncData.partnerSyncDatas[idx]._TeamID != (byte)eTeamType.Team2)
                continue;
            
            Vector3 pos = spawnPos + (idx == 0 ? new Vector3(1.5f, 0, -1.5f) : new Vector3(-1.5f, 0, -1.5f));
            G_GameInfo.GameInfo.SpawnPartner((ulong)(1234567 + idx + 1), (byte)eTeamType.Team2, syncData.partnerSyncDatas[idx], pos, spawnRot, true, EnemyLeader.GetComponent<Unit>());
        }
        
        InitSpawnCtlr();
    }

    protected override void InitUI()
    {
        base.InitUI();
        //인게임 HUD패널 생성
        HudPanel = UIMgr.Open("UIPanel/InGameHUDPanel").GetComponent<InGameHUDPanel>();
        //	   Debug.LogWarning("2JW : InitUI() In - " + HudPanel);
    }

    void InitSpawnCtlr()
    {
        List<string> SpawnEffects = new List<string>();

        //< 이펙트를 풀에 생성해둔다
        for (int i = 0; i < SpawnEffects.Count; i++)
            FillSpawnPool(effectPool, SpawnEffects[i], 3);
    }

    protected override IEnumerator GameStartReady()
    {
        while (true)
        {
            if ( ArenaGameState.IsMapLoad)
                break;

            yield return null;
        }

        if (HudPanel == null)
        {
            while (HudPanel == null)
            {
                yield return null;
            }
        }
        
        //yield return new WaitForSeconds(1f);
        ChangeLeader(0, false);
        SceneManager.instance.ShowLoadingTipPanel(false);
        CameraManager.instance.RtsCamera.Distance = 16f;
        HudPanel.StartEffCountDown();
        yield return new WaitForSeconds(3f);
        
        //Fx_IN_countdown_02B번체
        //Fx_IN_countdown_02G간체
        if(SystemDefine.LocalEff.Equals("_B") )
            UIHelper.CreateEffectInGame(HudPanel.transform, "Fx_IN_countdown_02B");
        else
            UIHelper.CreateEffectInGame(HudPanel.transform, "Fx_IN_countdown_02G");

        yield return new WaitForSeconds(1f);
        //HudPanel.StartTweenScale(false);

        TimeLimit = _LowDataMgr.instance.GetEtcTableValue<float>(EtcID.PvPTime);
        GameStart();
    }

    protected override void OnDieUnit(Unit deadUnit)
    {
        base.OnDieUnit(deadUnit);

        if (!deadUnit.IsPartner)
        {
            if (deadUnit.TeamID == (int)eTeamType.Team1)//내가 죽음
            {
                PrepareEndGame(false);
                StartCoroutine(RestoreTimeScale(0.25f, () =>
                {
                    TempCoroutine.instance.FrameDelay(2f, () =>
                    {
                        EndGame(false);
                    });
                }));
            }
            else if (deadUnit.TeamID == (int)eTeamType.Team2)//적이 죽음
            {
                PrepareEndGame(true);
                StartCoroutine(RestoreTimeScale(0.25f, () =>
                {
                    TempCoroutine.instance.FrameDelay(2f, () =>
                    {
                        EndGame(true);
                    });
                }));
            }
        }
    }

    public override void PrepareEndGame(bool win = false)
    {
        base.PrepareEndGame(win);
    }

    public override void EndGame(bool isSuccess)
    {
        base.EndGame(isSuccess);

        HudPanel.GameEnd(isSuccess);
        
        StartCoroutine(ResultAction(isSuccess));
    }

    IEnumerator ResultAction(bool isSuccess)
    {
        //yield return new WaitForSeconds(1);

        UIMgr.instance.UICamera.enabled = true;

        HudPanel.Hide();
        UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();

        NetworkClient.instance.SendPMsgArenaFightCompleteC(isSuccess);

        yield return null;
    }

    /// <summary>
    /// InGameHUDPanel에서 초기화 할 것이 있어서 재정의함.
    /// </summary>
    public override void RevivePlayer()
    {
        base.RevivePlayer();

        HudPanel.Revive();   //난투장에서는 이 부분도 따로 처리해야 한다. 
    }

}
