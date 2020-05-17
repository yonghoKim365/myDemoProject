using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sw;

public class FreeFightGameInfo : GameInfoBase
{
    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.FREEFIGHT; } }
    //public Vector4 SafetyZone;//min=x,y  max=z,w

    public bool IsSafetyZone;   //안전지역인가
    
    public void FreeFightNPCEnter(PMsgBattleNpcInfoS NPCInfo)
    {
        if (!(G_GameInfo.CharacterMgr.FindRoomNPC((ulong)NPCInfo.UllNpcId) != null))
        {
            Vector3 pos = NaviTileInfo.instance.GetTilePos(NPCInfo.NPosX, NPCInfo.NPosY);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 20, -1))
            {
                pos = navHit.position;
            }

            G_GameInfo.GameInfo.SpawnNetworkNpc((ulong)NPCInfo.UllNpcId, (uint)NPCInfo.UnNpcType, eTeamType.Team2, 0, pos, Quaternion.identity, false, false);

            Unit a_mUnit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)NPCInfo.UllNpcId);
            if (a_mUnit != null)
            {
                a_mUnit.CharInfo.MaxHp = NPCInfo.UnMaxHp;
                a_mUnit.CharInfo.Hp = NPCInfo.UnHp;
                a_mUnit.CharInfo.SuperArmor = (uint)NPCInfo.UnSuperArmor;
                a_mUnit.CharInfo.MaxSuperArmor = (uint)NPCInfo.UnMaxSuperArmor;

                a_mUnit.StaticState(false);
                a_mUnit.SetTarget(GameDefine.unassignedID);
            }
            else
            {
                Debug.Log("Notfound NPC:" + NPCInfo.UllNpcId);
            }

            return;
        }
        else
        {
            Unit unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)NPCInfo.UllNpcId);

            if (unit != null)
            {
                if (unit.navAgent != null && unit.navAgent.enabled == true)
                {
                    unit.navAgent.Stop();
                    unit.navAgent.enabled = false;
                }

                BoxCollider[] collider = unit.gameObject.GetComponentsInChildren<BoxCollider>();
                for (int ai = 0; ai < collider.Length; ai++)
                    collider[ai].enabled = false;

                if (GameDefine.skillPushTest)
                {
                    CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
                    for (int j = 0; j < colider2.Length; j++)
                        colider2[j].enabled = false;
                }

                unit.EffectClear();

                if (unit.ExtraModels != null)
                {
                    for (int bi = 0; bi < unit.ExtraModels.Count; bi++)
                        unit.ExtraModels[bi].gameObject.SetActive(false);
                }

                G_GameInfo.CharacterMgr.RemoveUnit(unit);
                unit.Model.Main.SetActive(false);
                unit.DeleteShadow();
                Destroy(unit.gameObject);
                unit = null;
            }

            Vector3 pos = NaviTileInfo.instance.GetTilePos(NPCInfo.NPosX, NPCInfo.NPosY);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 20, -1))
            {
                pos = navHit.position;
            }

            G_GameInfo.GameInfo.SpawnNetworkNpc((ulong)NPCInfo.UllNpcId, (uint)NPCInfo.UnNpcType, eTeamType.Team2, 0, pos, Quaternion.identity, false, false);

            Unit a_mUnit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)NPCInfo.UllNpcId);
            if (a_mUnit != null)
            {
                a_mUnit.CharInfo.MaxHp = NPCInfo.UnHp;
                a_mUnit.CharInfo.Hp = NPCInfo.UnHp;
                a_mUnit.CharInfo.SuperArmor = (uint)NPCInfo.UnSuperArmor;
                a_mUnit.CharInfo.MaxSuperArmor = (uint)NPCInfo.UnMaxSuperArmor;

                a_mUnit.StaticState(false);
                a_mUnit.SetTarget(GameDefine.unassignedID);
            }
        }
    }

    public void FreeFightUserEnter(PMsgBattleMapRoleInfoS anotherUserInfo)
    {
        if (!(G_GameInfo.CharacterMgr.FindRoomUnit((ulong)anotherUserInfo.UllRoleId) != null))
        {
            Vector3 pos = NaviTileInfo.instance.GetTilePos(anotherUserInfo.UnPosX, anotherUserInfo.UnPosY);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 20, -1))
            {
                pos = navHit.position;
            }

            _PlayerSyncData syncData = NetData.instance.OtherPcSyncData((ulong)anotherUserInfo.UllRoleId, (ulong)anotherUserInfo.UllRoleId
                                                                        , anotherUserInfo.SzName, (uint)anotherUserInfo.UnType, (uint)anotherUserInfo.UnCostumeId, 1
                                                                        , anotherUserInfo.UnTitlePrefix, anotherUserInfo.UnTitleSuffix, (uint)anotherUserInfo.UnMaxSuperArmor, 0);  //싱크 데이터를 하나 만들어 주고 

            byte TeamID = (byte)eTeamType.Min;
            if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                TeamID = (byte)eTeamType.Team2;
            }
            else if (FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM || FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID)
            {
                TeamID = (byte)eTeamType.Team1;
            }

            G_GameInfo.GameInfo.SpawnNetworkUnit((ulong)anotherUserInfo.UllRoleId, TeamID, syncData.playerSyncDatas[0], pos, Quaternion.identity, false);

            Unit player = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)anotherUserInfo.UllRoleId);
            if (player != null)
            {
                player.CharInfo.MaxHp = anotherUserInfo.UnMaxHp;
                player.CharInfo.Hp = anotherUserInfo.UnHp;
                player.CharInfo.SuperArmor = (uint)anotherUserInfo.UnSuperArmor;
                player.CharInfo.MaxSuperArmor = (uint)anotherUserInfo.UnMaxSuperArmor;

                player.StaticState(false);
                player.SetTarget(GameDefine.unassignedID);

                //버프도 적용
                for (int i = 0; i < anotherUserInfo.CInfo.Count; i++)
                {
                    SkillTables.BuffInfo buff = _LowDataMgr.GetBuffData((uint)anotherUserInfo.CInfo[i].UnBuffId);

                    player.BuffCtlr.AttachBuff(null, player, buff, 10000, (uint)anotherUserInfo.CInfo[i].FBuffTime);
                }

                G_GameInfo.CharacterMgr.RoomPlayerEnter((ulong)anotherUserInfo.UllRoleId, player);
            }


            UIBasePanel board = UIMgr.GetUIBasePanel("UIPanel/InGameBoardPanel");
            if (board != null)
            {
                (board as InGameBoardPanel).ChangeNameColor();
            }

            return;
        }


        UIBasePanel board1 = UIMgr.GetUIBasePanel("UIPanel/InGameBoardPanel");
        if (board1 != null)
        {
            (board1 as InGameBoardPanel).ChangeNameColor();
        }

    }

    public void FreeFightUserLeave(ulong ullRoleID)
    {
        Unit leaveUser = G_GameInfo.CharacterMgr.GetRoomPlayer(ullRoleID);
        if (leaveUser != null)
        {
            if (leaveUser == null)
                return;

            if (leaveUser.navAgent != null && leaveUser.navAgent.enabled == true)
            {
                leaveUser.navAgent.Stop();
                leaveUser.navAgent.enabled = false;
            }

            BoxCollider[] collider = leaveUser.gameObject.GetComponentsInChildren<BoxCollider>();
            for (int ai = 0; ai < collider.Length; ai++)
                collider[ai].enabled = false;

            if (GameDefine.skillPushTest)
            {
                CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
                for (int j = 0; j < colider2.Length; j++)
                    colider2[j].enabled = false;
            }

            leaveUser.EffectClear();

            if (leaveUser.ExtraModels != null)
            {
                for (int bi = 0; bi < leaveUser.ExtraModels.Count; bi++)
                    leaveUser.ExtraModels[bi].gameObject.SetActive(false);
            }

            G_GameInfo.CharacterMgr.RemoveUnit(leaveUser);
            leaveUser.Model.Main.SetActive(false);
            leaveUser.DeleteShadow();
            Destroy(leaveUser.gameObject);
            G_GameInfo.CharacterMgr.RoomPlayerLeave(ullRoleID);
        }
    }

    public void FreeFightNPCLeave(ulong UllRoleID)
    {

    }

    /*
    public static bool IsSafetyZone(Vector3 a_CacMyPos)                                       
    {
        a_CacMyPos.y = 0.0f;

        Vector3 a_SafetyZon = G_GameInfo.G_SafetyZone;
        a_SafetyZon.y = 0.0f;

        if ((a_SafetyZon - a_CacMyPos).magnitude < 25.0f)
            return true;  //<--- 공격 불가능

        return false;  //<--- 공격 가능
    }
    */
    public ulong RoomNo { set; get; }
    public static bool DisableHit = false; //애도  Unit TakeDamage에서 임시로 사용하고 있는 거 같다.

    protected override void InitDatas()
    {
        base.InitDatas();

        //< 컷씬 정보 미리 로드
        CutSceneMgr.LoadCutScene();

        //if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
        //{
        //    DungeonTable.FreefightTableInfo freeInfo = _LowDataMgr.instance.GetLowDataFreeFight(FreeFightGameState.StageName);
        //    if (freeInfo !=null && 4 <= freeInfo.SafetyZone.list.Count)
        //    {
        //        SafetyZone.x = float.Parse(freeInfo.SafetyZone.list[0]);
        //        SafetyZone.y = float.Parse(freeInfo.SafetyZone.list[1]);
        //        SafetyZone.z = float.Parse(freeInfo.SafetyZone.list[2]);
        //        SafetyZone.w = float.Parse(freeInfo.SafetyZone.list[3]);
        //    }
        //}
    }

    protected override void InitManagers()
    {
        base.InitManagers();
        G_GameInfo.CharacterMgr.allrUUIDDic.Clear();
        //NetData.instance.MakePlayerSyncData(false);
        _PlayerSyncData syncData = NetData.instance._playerSyncData;
        CreatePlayerController(syncData.playerSyncDatas[0], null);
        
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

        //여기서 몬스터들을 서버로부터 받아서... 스폰(로딩)한다.

        //< 이펙트를 풀에 생성해둔다
        for (int i = 0; i < SpawnEffects.Count; i++)
            FillSpawnPool(effectPool, SpawnEffects[i], 3);
    }
    public class BossData
    {
        public int Daed; //0생존1사망
        public int PosX;//x좌표
        public int PosY;//y좌표
        public int ReviveTime;  //초기화시간

    }
    public static BossData _Boss = new BossData();
 
    public void SetBossData(int daed, int x, int y , int revivalTime = 0)
    {
        _Boss.Daed = daed;
        _Boss.PosX = x;
        _Boss.PosY = y;
        _Boss.ReviveTime = revivalTime;

        HudPanel.AlertBoss(_Boss);
    }
    public BossData GetBossData()
    {
        if (_Boss == null)
            return null;

        return _Boss;
    }
    
    //난투장드랍아이템 
    public class FreefightDropReward
    {
        public int honor = 0;   //획득명예
        public int coin = 0;//골드
        public int exp = 0; //경험치
        public int totalItemCnt = 0;    //획득한아이템총갯수 
        public List<NetData.DropItem> item = new List<NetData.DropItem>();

    }
    public static FreefightDropReward _FreeDropReward = new FreefightDropReward();
    public void SetFreefightReward(int honor, int coin, int exp, List<NetData.DropItem> item, bool firstEnter/*처음입장?*/)
    {

        _FreeDropReward.honor += honor;
        _FreeDropReward.coin += coin;
        _FreeDropReward.exp += exp;

        if (item != null)
        {
            _FreeDropReward.totalItemCnt += item.Count;
            //for (int i = 0; i < item.Count; i++)
            //{
            //    _FreeDropReward.totalItemCnt += item[i].Amount;
            //}
        }


        HudPanel.SetFreeFightReward(_FreeDropReward, firstEnter);

    }
    public void ResetDropData()
    {
        _FreeDropReward.honor = 0;
        _FreeDropReward.coin = 0;
        _FreeDropReward.exp = 0;
        _FreeDropReward.item = null;
        _FreeDropReward.totalItemCnt = 0;
    }

    public static uint _SelectLastFreeFightType = 0; //마지막선택한 난투장ㅌㅏ입(방정보 불러올떄 사용할것)
    public uint SelectLastFreeFightType
    {
        get
        {
            return _SelectLastFreeFightType;
        }
        set
        {
            _SelectLastFreeFightType = value;
        }
    }


    //난투장PK데이터
    public class FreeFightUserKillData
    {
        public int KillCount;  //난투장PK킬수
        public string UserName;    //죽은유저이름

    }
    public FreeFightUserKillData _FreeFightUserKillData = new FreeFightUserKillData();

    public void SetFreeFightUserKillData(int count, string name)
    {
        _FreeFightUserKillData.KillCount = count;
        _FreeFightUserKillData.UserName = name;

        HudPanel.SetPKText(_FreeFightUserKillData);

    }

    //나를죽인유저 
    public List<long> KillAttackerRoleId = new List<long>();

    public FreefightDropReward GetFreefightDropReward()
    {
        if (_FreeDropReward == null)
            return null;

        return _FreeDropReward;
    }

    protected override IEnumerator GameStartReady()
    {
        while (true)
        {
            if (FreeFightGameState.IsMapLoad)
                break;

            yield return null;
        }

        float delay = 1f + Time.time;
        while (true)
        {
            if (delay < Time.time)
                break;

            yield return null;
        }

        GameStart();
        //지금까지 쌓인 유저/몬스터들을 로딩해준다
        List<PMsgBattleNpcInfoS> beforeNpcLoadingList = NetworkClient.instance.beforeNpcLoadingList;

        for (int i = 0; i < beforeNpcLoadingList.Count; i++)
        {
            FreeFightNPCEnter(beforeNpcLoadingList[i]);
        }

        NetworkClient.instance.beforeNpcLoadingList.Clear();

        List<PMsgBattleMapRoleInfoS> beforeUnitLoadingList = NetworkClient.instance.beforeUnitLoadingList;

        for (int i = 0; i < beforeUnitLoadingList.Count; i++)
        {
            FreeFightUserEnter(beforeUnitLoadingList[i]);
        }

        NetworkClient.instance.beforeUnitLoadingList.Clear();

        SceneManager.instance.ShowLoadingTipPanel(false);
    }

    protected override void GameStart()
    {
        base.GameStart();

        /*
        if (SceneManager.instance.IsRTNetwork)
        {
            if (0 < SceneManager.g_FrFtInfo.roomList.Count)
            {
                PlayerController a_PCtrl = G_GameInfo.GameInfo.FindOwnerPlayerController();
                if (a_PCtrl != null && a_PCtrl.Leader != null)
                {
                    List<uint> mySkillList = new List<uint>();
                    mySkillList.Add(NetData.instance._playerSyncData.playerSyncDatas[0].NormalAttackData[0]._SkillLevel);
                    for (int i = 0; i < NetData.instance._playerSyncData.playerSyncDatas[0].SkillData.Length; i++)
                    {
                        if (NetData.instance._playerSyncData.playerSyncDatas[0].SkillData[i] != null)
                            mySkillList.Add(NetData.instance._playerSyncData.playerSyncDatas[0].SkillData[i]._SkillLevel);
                    }

                    //iFunClient.instance.ReqEnterRoom(RoomNo, NetData.instance._userInfo._charUUID);  //채널 입장 <-- 나중에 정보를 미리 받아 놓는 걸로 수정한다. 
                    //콜백으로...
                    // iFunClient.   여기서 받아온 캐릭터 정보 UUID를 다시 셋팅해 주어야 한다.                   
                }
            }

            G_GameInfo.G_SafetyZone = GetSpawnTransform((eTeamType)eTeamType.Team1) != null ? GetSpawnTransform((eTeamType)eTeamType.Team1).position : Vector3.zero;
        }
        */
    }

    protected override void OnDieUnit(Unit deadUnit)
    {
        base.OnDieUnit(deadUnit);
        if(FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM )
        {
            NetworkClient.instance.SendPMsgColosseumCompleteC(StageId, true, 0);
        }
        else if(FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID)
        {
            NetworkClient.instance.SendPMsgMultiBossCompleteC(StageId, true, 0);
        }
        else if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
        {
          if(deadUnit is Pc && deadUnit.m_rUUID == NetData.instance.GetUserInfo()._charUUID)
            {
                //내가죽었을때 부활팝업
                UIMgr.OpenRevivePopup(null, true);
            }
        }
    }

    public override void EndGame(bool isSuccess)
    {
        base.EndGame(isSuccess);

        HudPanel.GameEnd(isSuccess);

        PlayerController playerCtlr = G_GameInfo.PlayerController;

        uint clearGrade = 3;

        //Debug.LogWarning("2JW : SingleGame EndGame In TotalGold - " + playerCtlr.TotalGold);
        /*
        TotalGachaData totalData = new TotalGachaData();
        totalData.gold = playerCtlr.TotalGold;

        if (isSuccess)
        {
            ///그냥 형식만. 획득한 아이템이 3개라는 것만 알린다. UIHelper에서 들고있는 GetFakeItemData에서 다 처리함 (난투장 획득 아이템에 대한 부분은 별도 퍼리해야할 것 같다.)
            for (int i = 0; i < 4; i++)
            {
                NetData.GachaData item_0 = new NetData.GachaData();
                item_0.userItem = new NetData.ItemData();
                item_0.userItem.item_id = (uint)i + 1;
                totalData.Add(item_0);
            }
        }
        */
        StartCoroutine(ResultAction(isSuccess, playerCtlr, clearGrade));//, totalData));, (ulong)(playingTime * 1000f)

        //< 승리했을때에만 보스처리(승리했다면 무조건 보스를 잡았다는 뜻이므로)
        //if (isSuccess)
        //MissionManager.AllBossMonsterKillQuestCheck(1);
    }

    IEnumerator ResultAction(bool isSuccess, PlayerController playerCtlr, uint clearGrade)//, TotalGachaData totalData)
    {
        UIMgr.instance.UICamera.enabled = true;

        HudPanel.Hide();
        UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();

        yield return null;
        
        //iFunClient.instance.ReqLeaveRoom();
        yield return null;

        if (!isSuccess)
        {
            //< 모든 유닛을 숨김
            Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
            characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);
            foreach (Unit unit in allUnits)
                unit.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1);

        if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            SceneManager.instance.LobbyActionEvent(_STATE.SINGLE_GAME, _ACTION.GO_TOWN, UI_OPEN_TYPE.NONE);
        else
        {
            if (isSuccess)
            {
                Time.timeScale = 0.1f;
                float endSlowDuration = 0.25f;
                if (G_GameInfo.CharacterMgr != null && G_GameInfo.CharacterMgr.BossUnit != null)
                    G_GameInfo.CharacterMgr.BossUnit.BossEndShaderChange();

                StartCoroutine(RestoreTimeScale(endSlowDuration, () => {
                    //UIMgr.Open("UIPanel/ResultRewardStarPanel", isSuccess);
                    OpenResultPanel(true);
                }));

                if (_GameEndEventPanel != null)
                    _GameEndEventPanel.StartEvent(1, rtsCamera);
            }
            else
                OpenResultPanel(false); //UIMgr.Open("UIPanel/ResultRewardStarPanel", false);
        }

        //여기서 초기화. 실제로 이것은 Frefight일 경우에만 사용하므로.
        // SceneManager.GameMode = GAME_MODE.NONE;
        //G_GameInfo.GameMode = GAME_MODE.NONE;

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

    public override void PlayTimeUpdate()
    {
        //PlayTime += Time.deltaTime;
        //float time = Mathf.Round(PlayTime - 0.5f);
        //time = TimeLimit - time;

        //if (HudPanel != null)
        //    HudPanel.TimeUpdate(time);
    }

 

    /// <summary> 무료 부활 카운트 다운 </summary>
    public void SetRevivalCountDown(float sec)
    {
        if (HudPanel == null)
            return;

        StartCoroutine("RevivalCountDown", sec);
    }

    IEnumerator RevivalCountDown(float sec)
    {
        HudPanel.SetActiveUI(false);
        HudPanel.SetCountDownActive(true);
        while (0 < sec)
        {
            int s = (int)sec;
            HudPanel.SetCountDown(s);
            yield return new WaitForSeconds(1f);
            sec -= 1;
        }

        HudPanel.SetActiveUI(true);
        HudPanel.SetCountDownActive(false);
        yield return null;
    }

    GUIStyle txtColStyle;
    void OnEnable()
    {
        txtColStyle = new GUIStyle();
        txtColStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        if(Debug.isDebugBuild)
        {
           // if (FreeFightGameState.StateActive)
           // {
                GUILayout.BeginVertical("box");

                GUILayout.Label("<b>Multiplay GameMode:</b>" + GameMode.ToString(), txtColStyle);

                var enumerator = G_GameInfo.CharacterMgr.allrUUIDDic.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Unit unit = enumerator.Current.Value;

                    if (unit != null)
                    {
                        if (unit.m_rUUID == NetData.instance._userInfo._charUUID)
                        {
                            GUILayout.Label("<b>" + enumerator.Current.Key + ": " + (unit as Pc).CharInfo.UnitName + " HP:" + unit.CharInfo.Hp + "/" + unit.CharInfo.MaxHp + " SA:" + unit.CharInfo.SuperArmor + "/" + unit.CharInfo.MaxSuperArmor + "</b>", txtColStyle);
                        }
                        else
                        {
                            GUILayout.Label(enumerator.Current.Key + ": " + (unit as Pc).CharInfo.UnitName + " HP:" + unit.CharInfo.Hp + "/" + unit.CharInfo.MaxHp + " SA:" + unit.CharInfo.SuperArmor + "/" + unit.CharInfo.MaxSuperArmor, txtColStyle);
                        }
                    }
                }

                GUILayout.EndVertical();
            }
        }
    //}
}
