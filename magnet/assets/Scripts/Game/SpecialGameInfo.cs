using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialGameInfo : GameInfoBase {
    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode {
        get {
            return SpecialGameState.IsGoldStage ? GAME_MODE.SPECIAL_GOLD : GAME_MODE.SPECIAL_EXP;//GAME_MODE.SPECIAL;
        }
    }

    public uint GetTotalValue;//골드, 경험치가 쌓일 변수
    public uint MaxValue;//골드, 경험치 최대 량.
    //public int MaxBoxCount;
    //private Dictionary<uint, DropData> DropRewardDic = new Dictionary<uint, DropData>();//보상 목록. Key는 몬스터 아이디.
    //private List<Npc> SpawnNpcList = new List<Npc>();
    private List<Unit> SpawnUnitList = new List<Unit>();
    //private List<Npc> SpawnDisturbList = new List<Npc>();

    //private List<uint> DisturbMobList;//방해 몬스터 아이들
    //private float DisturbRegenTime;//방해몬스터 다시 나오는 시간.
    private List<MonsterData> MonDataList = new List<MonsterData>();
    //private Dictionary<uint, int> PropMaxCount = new Dictionary<uint, int>();

    private short CLeft = 0;
    private short CTop = 0;
    private short CRight = 0;
    private short CBottom = 0;

    private short MLeft = 0;
    private short MTop = 0;
    private short MRight = 0;
    private short MBottom = 0;

    //private int DisturbMaxCount;
    //class DropData
    //{
    //    public uint CurValue;
    //    public uint OutValue;
    //    public float SaveHpPer;
    //}

    class MonsterData
    {
        public bool IsProp;
        public bool IsDieMob;
        public int DropGold;
        public int DropPoint;
        public uint MonsterId;
        public float RegenTime;
        public float RunTime;

        public MonsterData(uint id, float time, int gold, int point, bool isProp)
        {
            MonsterId = id;
            RegenTime = time;
            RunTime = RegenTime+1;

            DropGold = gold;
            DropPoint = point;
            IsDieMob = true;
            IsProp = isProp;
        }

        public bool RegenUpdate()
        {
            if (!IsDieMob)
                return false;

            RunTime += Time.deltaTime;
            if (RunTime < RegenTime)
                return false;

            RunTime = 0;
            IsDieMob = false;
            return true;
        }
    }

    protected override void InitDatas()
    {
        //_STATE state = SceneManager.instance.CurrState();
        
        base.InitDatas();

        //< 컷씬 정보 미리 로드
        CutSceneMgr.LoadCutScene();
    }

    protected override void InitManagers()
    {
        base.InitManagers();
        
        G_GameInfo.CharacterMgr.allrUUIDDic.Clear();

        _PlayerSyncData syncData = NetData.instance._playerSyncData;
        CreatePlayerController(syncData.playerSyncDatas[0], syncData.partnerSyncDatas);

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
        if (SpecialGameState.IsGoldStage)//리젠 영역잡기
        {
            DungeonTable.EquipInfo mLowData = _LowDataMgr.instance.GetLowDataEquipBattle((byte)StageId);
            if (mLowData != null)
            {
                //regenCount = goldLowData.regenCount;
                List<string>  charPos = mLowData.RegenCharacter.list;
                CLeft = short.Parse(charPos[0]);
                CTop = short.Parse(charPos[1]);
                CRight = short.Parse(charPos[2]);
                CBottom = short.Parse(charPos[3]);

                List<string> mobPos = mLowData.RegenMob.list;
                MLeft = short.Parse(mobPos[0]);
                MTop = short.Parse(mobPos[1]);
                MRight = short.Parse(mobPos[2]);
                MBottom = short.Parse(mobPos[3]);

                for(int i=0; i < mLowData.DummyMonster.Count; i++)//몬스터
                {
                    MonsterData data = new MonsterData(uint.Parse(mLowData.DummyMonster[i]), float.Parse(mLowData.DummyMonsterTime[i]), int.Parse(mLowData.DropGold[i]), 0, false );
                    MonDataList.Add(data);
                }
                
                for(int i=0; i < mLowData.RegenMobCount.Count; i++)//프랍 생성
                {
                    int count = int.Parse(mLowData.RegenMobCount[i]);//해당 개수만큼 해당 번쨰 프랍 생성.
                    for(int j=0; j < count; j++)
                    {
                        MonsterData data = new MonsterData(uint.Parse(mLowData.MonsterIndex[i]), float.Parse(mLowData.RegenMobTime[i]), 0, int.Parse(mLowData.GetMobPoint[i]), true);
                        MonDataList.Add(data);
                    }
                }
            }
        }
        else
        {
            DungeonTable.SkillInfo skillLowData = _LowDataMgr.instance.GetLowDataSkillBattle((byte)StageId);
            if (skillLowData != null)
            {
                List<string>  charPos = skillLowData.RegenCharacter.list;
                CLeft = short.Parse(charPos[0]);
                CTop = short.Parse(charPos[1]);
                CRight = short.Parse(charPos[2]);
                CBottom = short.Parse(charPos[3]);

                List<string> mobPos = skillLowData.RegenMob.list;
                MLeft = short.Parse(mobPos[0]);
                MTop = short.Parse(mobPos[1]);
                MRight = short.Parse(mobPos[2]);
                MBottom = short.Parse(mobPos[3]);

                for (int i = 0; i < skillLowData.regenCount.Count; i++)//프랍 생성
                {
                    int count = int.Parse(skillLowData.regenCount[i]);//해당 개수만큼 해당 번쨰 프랍 생성.
                    for (int j = 0; j < count; j++)
                    {
                        MonsterData data = new MonsterData(uint.Parse(skillLowData.MonsterIndex[i]), float.Parse(skillLowData.MonsterTime[i]), int.Parse(skillLowData.DropGold[i]), int.Parse(skillLowData.DropPoint[i]), false);
                        MonDataList.Add(data);
                    }
                }
            }
        }
        
        SpawnDisturbMob(true);

        //MaxBoxCount = SpawnPropList.Count;
        Debug.Log("SpacialDungeon Reward TotalValue= " + MaxValue);
    }

    //private float SpawnTime;
    void SpawnDisturbMob(bool isStart)
    {
        for (int i=0; i < MonDataList.Count; i++)//소환
        {
            if (!MonDataList[i].RegenUpdate() )
                continue;
            
            Vector3 pos = Vector3.one;
            while (true)
            {
                pos.x = Random.Range(MLeft, MRight);
                pos.z = Random.Range(MBottom, MTop);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pos, out hit, 10, -5))
                    pos = hit.position;

                if ((pos.x < CLeft || CRight < pos.x) && (pos.z < CBottom || CTop < pos.z))
                {
                    break;
                }
            }

            GameObject npcGo = null;
            Unit unit = null;
            if (MonDataList[i].IsProp)
            {
                npcGo = G_GameInfo.GameInfo.SpawnProp(MonDataList[i].MonsterId, eTeamType.Team2, 1, pos, Quaternion.identity);
                Prop npc = npcGo.GetComponent<Prop>();
                npc.spawnner = this as ISpawner;
                npc.SpwanUnitId = (uint)i;

                unit = npc;
            }
            else
            {
                npcGo = G_GameInfo.GameInfo.SpawnNpc(MonDataList[i].MonsterId, eTeamType.Team2, 1, pos, Quaternion.identity);
                Npc npc = npcGo.GetComponent<Npc>();
                npc.spawnner = this as ISpawner;
                npc.SetNpcPostingInfo((uint)i);

                unit = npc;
            }
            
            if (isStart)
                npcGo.SetActive(false);
            else
            {
                string path = "Effect/_UI/_INGAME/Fx_IN_enter_01";
                GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
                effGo.transform.parent = npcGo.transform;
                effGo.transform.localScale = npcGo.transform.localScale;
                effGo.transform.localPosition = Vector3.zero;
                unit.StaticState(false);
            }

            unit.SetObstacleAvoidance(false);
            SpawnUnitList.Add(unit);
        }
    }

    protected override IEnumerator GameStartReady()
    {
        SoundManager.instance.PlayBgmSoundClip(null);

        while (true)
        {
            if (SpecialGameState.IsMapLoad)
                break;

            yield return null;
        }

        if(HudPanel == null)
        {
            while(HudPanel == null)
            {
                yield return null;
            }
        }
        
        SceneManager.instance.ShowLoadingTipPanel(false);
        yield return new WaitForSeconds(1f);

        if (GameMode == GAME_MODE.SPECIAL_EXP)
            SoundManager.instance.PlaySfxSound(eUISfx.UI_count_exp_dungeon, true);
        else
            SoundManager.instance.PlaySfxSound(eUISfx.UI_count_gold_dungeon, true);

        HudPanel.StartEffCountDown();
        int count = 3;
        while (0 < count)
        {
            --count;
            yield return new WaitForSeconds(1f);
        }
        
        //플레이 시간 저장.
        if (SpecialGameState.IsGoldStage)
        {
            DungeonTable.EquipInfo goldLowData = _LowDataMgr.instance.GetLowDataEquipBattle((byte)StageId);
            TimeLimit = 9999;// goldLowData.LimitTime;
        }
        else
        {
            DungeonTable.SkillInfo expLowData = _LowDataMgr.instance.GetLowDataSkillBattle((byte)StageId);
            TimeLimit = expLowData.LimitTime;
        }
        
        int loopCount = SpawnUnitList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            SpawnUnitList[i].gameObject.SetActive(true);
            SpawnUnitList[i].SetObstacleAvoidance(false);
            SpawnUnitList[i].StaticState(false);

            if (SpawnUnitList[i] is Npc)
            {
                string path = "Effect/_UI/_INGAME/Fx_IN_enter_01";
                GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
                effGo.transform.parent = SpawnUnitList[i].transform;
                effGo.transform.localScale = SpawnUnitList[i].transform.localScale;
                effGo.transform.localPosition = Vector3.zero;
            }
        }
        
        GameStart();
        
        yield return new WaitForSeconds(0.1f);
        SceneManager.instance.CurrentStateBase().PlayMapBGM(Application.loadedLevelName);
    }
    
    protected override void OnDieUnit(Unit deadUnit)
    {
        int dropGold=0, dropPoint=0;
        //if (deadUnit.UnitType != UnitType.Prop && deadUnit.UnitType != UnitType.Boss)
        //{
            if (deadUnit is Npc || deadUnit is Prop) {
                for (int i = 0; i < SpawnUnitList.Count; i++)
                {
                    if (SpawnUnitList[i] != deadUnit)
                        continue;

                    uint lowDataId = SpawnUnitList[i] is Npc ? (SpawnUnitList[i] as Npc).NpcLowID : (SpawnUnitList[i] as Prop).PropLowID;
                    for (int j = 0; j < MonDataList.Count; j++)//리젠 카운트
                    {
                        if (MonDataList[j].IsDieMob || !MonDataList[j].MonsterId.Equals(lowDataId) )
                            continue;

                        MonDataList[j].IsDieMob = true;
                        MonDataList[j].RunTime = 0;

                        dropGold = MonDataList[j].DropGold;
                        dropPoint = MonDataList[j].DropPoint;
                        break;
                    }

                    SpawnUnitList.RemoveAt(i);
                    break;
                }
            }
            else if(deadUnit is Pc)
            {
                base.OnDieUnit(deadUnit);
                return;
            }

        //    return;
        //}
        
        AddGold(deadUnit, dropGold);//일단 골드만
        //AddGold(deadUnit, dropPoint);//다른 방식으로 HUD페널에 전달해야할듯

        //HudPanel.COIN_LOOTING((float)data.CurValue);

        if (HudPanel != null)//이거 죽여야하나???
        {
            HudPanel.SpecialKillCount(0);
        }

        //if (DropRewardDic.ContainsKey(deadNpc.SpwanUnitId))
        //    DropRewardDic.Remove(deadNpc.SpwanUnitId);
        /*
        if (SpawnPropList.Contains(deadNpc))
            SpawnPropList.Remove(deadNpc);
            
        if (0 < SpawnPropList.Count)
            return;

        SpawnPropList.Clear();
        PrepareEndGame(true);
        EndGame(true);
        */
    }
    /*
    protected override void OnRemoveUnit(Unit removedUnit)
    {
        if (null == removedUnit || isEnd)
            return;

        if (removedUnit is Pc && removedUnit.IsLeader) {//실패
            EndGame(false);
        }

        if (SpawnPropList != null && SpawnPropList.Count <= 0)
            EndGame(true);
    }
    */
    public override void EndGame(bool isSuccess)
    {
        base.EndGame(isSuccess);
        
        HudPanel.GameEnd(isSuccess, true);

        PlayerController playerCtlr = G_GameInfo.PlayerController;
        StartCoroutine(ResultAction(isSuccess, playerCtlr ));
    }

    IEnumerator ResultAction(bool isSuccess, PlayerController playerCtlr)
    {
        yield return new WaitForSeconds(2);

        UIMgr.instance.UICamera.enabled = true;

        HudPanel.Hide();
        UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();

        yield return null;

        Debug.Log(string.Format("End {0} GetValue={1}/{2}", SpecialGameState.IsGoldStage ? "Gold":"Exp", GetTotalValue, MaxValue ));
        //if (!SpecialGameState.IsGoldStage)
        //{
        //    NetworkClient.instance.SendPMsgExpBattleCompleteC(StageId, isSuccess ? 1 : 0, (int)GetTotalValue);
        //}
        //else
        //{
        //    NetworkClient.instance.SendPMsgCoinBattleCompleteC(StageId, isSuccess ? 1 : 0, (int)GetTotalValue);
        //}

        //초기화.
        GetTotalValue = 0;
        //DropRewardDic.Clear();
        //SpecialGameState.MonList.Clear();
        OpenResultPanel(isSuccess);
        yield return null;
    }

    public override void PlayTimeUpdate()
    {
        SpawnDisturbMob(false);
        base.PlayTimeUpdate();
    }

    /// <summary>
    /// InGameHUDPanel에서 초기화 할 것이 있어서 재정의함.
    /// </summary>
    public override void RevivePlayer()
    {
        base.RevivePlayer();

        HudPanel.Revive();
    }
    /*
    public override void AddGold(Unit unit, float value)
    {
        //if (SpecialGameState.IsGoldStage)
        //{
        //    if (unit is Prop == false)//이럴 수 있음 방해 몬스터라서
        //        return;
        //}
        //else
        //{
        //    if (unit is Npc == false)
        //        return;
        //}

        //Unit npc = unit as Unit;
        if(unit is Prop)
        {

        }

        DropData data = null;
        if (!DropRewardDic.TryGetValue(npc.SpwanUnitId, out data))
        {
            Debug.LogError("DropReward NotFound SpawnUnitNum " + npc.SpwanUnitId);
            return;
        }

        int curHP = npc.CharInfo.Hp;
        int maxHP = npc.CharInfo.MaxHp;
        
        if (curHP <= 0 )//사망
        {
            GetTotalValue += data.CurValue;
            if (SpecialGameState.IsGoldStage)
                base.AddGold(unit, (int)data.CurValue);
            else //경험치
                HudPanel.COIN_LOOTING((float)data.CurValue);
            //{
            //    HudPanel.ExpUpdate(GetTotalValue, (float)GetTotalValue / (float)MaxValue);
            //}
            return;
        }

        float nowHpValue = (float)curHP / (float)maxHP;
        nowHpValue = (float)System.Math.Floor(nowHpValue * 10) / 10;
        float perValue = ((data.SaveHpPer - nowHpValue)*10)/10;
        if (perValue < 0.1f)//조건에 만족하지 못함.
            return;
        
        uint getValue = (uint)(data.OutValue * (perValue * 10));
        data.SaveHpPer = nowHpValue;
        data.CurValue -= getValue;
        GetTotalValue += getValue;

        if (SpecialGameState.IsGoldStage)
            base.AddGold(unit, (int)getValue);
        else //경험치
            HudPanel.COIN_LOOTING((float)getValue);

    }
    */
}
