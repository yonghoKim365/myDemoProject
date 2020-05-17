using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//< 플레이어 한명당 유닛들의 정보등을 저장하기위한 클래스
public class PlayerController : MonoBehaviour
{
    public ulong AccountUUID
    {
        get
        {
            return unitSyncData._AccountUUID;
        }
    }

    //< 모든 로드(ㅣ모델, 데이터 등등)가 완료되었는지
    public bool         LoadingDone = false;

    public List<Pc>     Units = new List<Pc>();
    public List<Pc>	    Partners = new List<Pc>();
    public Unit         Leader { get; set; }

    //< 공격을 했을때 몇번때렸는지에 대한 콤보(누적)
    public int          CurCombo { get; set; }
    public int          MaxCombo { get; set; }
    public float ComboTieCheckTime, NextComboTime;
    //int ComboAfterApplyCount = 0;

    //플레이어 유닛만의 싱크데이터
    public PlayerUnitData   unitSyncData { get; protected set; }

    // NPC 죽인 카운트 (인덱스로도 쓰임)
    public int NpcKillCount = 0;

    //==== 친구소환용
    public float    Combat_Friend_Time; // 친구 소환 후 유지 시간(초)
    public float    Combat_Friend_UnitRecall_CoolTime; // 인게임 친구 소환 후 재소환까지 걸리는 시간 (초)
    public int      Combat_Friend_LimitCount;
    public int      friendSummonCount;

    //==== Hp리젠용
    float curTime = 0;
    float regenHpDelay = 1f;
    
    public void SetInstanceID(ulong a_rUUID)
    {
        unitSyncData._AccountUUID = a_rUUID;
    }

    /// 컨트롤러 초기화 (플레이어에 필요한 Pc, Units 생성)
    public void Init(PlayerUnitData _playerData, List<PlayerUnitData> partnerList)
    {
        NpcKillCount = 0;
        unitSyncData = _playerData;

        G_GameInfo.GameInfo.SetPlayerCtrl(this);

        //ComboAfterApplyCount = (int)LowDataMgr.GetConfigValue( "ComboAfterApplyCount" );
        ComboTieCheckTime = 0;// LowDataMgr.GetConfigValue( "ComboTieCheckTime" ) / GameDefine.ConvertMilliToSec;

        Combat_Friend_Time = 0;// LowDataMgr.GetConfigValue( "Combat_Friend_Time" );
        Combat_Friend_UnitRecall_CoolTime = 0;//LowDataMgr.GetConfigValue( "Combat_Friend_UnitRecall_CoolTime" );
        Combat_Friend_LimitCount = 0;//(int)LowDataMgr.GetConfigValue( "Combat_Friend_Count" );
        friendSummonCount = 0;

        // 내 유닛리스트 생성
        Unit leader = SpawnPc(unitSyncData._TeamID, unitSyncData).GetComponent<Unit>();
        leader.m_rUUID = NetData.instance._userInfo._charUUID;

        //내유닛이 avoidancePriority 늘려준다(못밀게)
        leader.navAgent.avoidancePriority = 40;

        if (G_GameInfo.GameInfo.GameMode != GAME_MODE.FREEFIGHT)
        {
            if(partnerList != null)
            {
                int count = 0;
                for (int i = 0; i < partnerList.Count; i++)
                {
                    if (unitSyncData._TeamID == partnerList[i]._TeamID)
                    {
                        Unit partner = SpawnPartner(partnerList[i]._TeamID, partnerList[i], leader, count).GetComponent<Unit>();
                        partner.m_rUUID = partnerList[i]._TCPUUID;
                        G_GameInfo.CharacterMgr.AddRoomUnit(partner);

                        count++;
                    }
                }
            }            
        }

        Leader = Units[0];

        G_GameInfo.CharacterMgr.AddRoomUnit(leader);     //빠른 검색을 위한 리스트 추가...

        if (SceneManager.instance.IsRTNetwork)
        {
            if (!G_GameInfo.CharacterMgr.allTeamDic.ContainsKey(1))
                G_GameInfo.CharacterMgr.allTeamDic.Add(1, new List<Unit>());
        }
    }

    void OnDestroy()
    {
        //if (G_GameInfo.GameInfo != null)
        //    G_GameInfo.GameInfo.RemovePlayerController(this);
    }

    void Update()
    {
        if (G_GameInfo.GameInfo == null)
            return;

        // 1초마다        
        if (Time.time - curTime < regenHpDelay)
            return;

        curTime = Time.time + regenHpDelay;

        //// 각자의 RegenHp에 맞는 수치대로 체력을 회복시켜준다.
        //for (int i = 0; i < Units.Count; i++)
        //{
        //    if (null == Units[i] || Units[i].CharInfo.IsDead)
        //        continue;

        //    //u.SetHp( null, u.CharInfo.RegenHp );
        //    Units[i].CharInfo.Hp += (int)Units[i].CharInfo.RegenHp;
        //}
    }

    /// 현 컨트롤러에서 필요한 모든 로딩이 완료되었는지 알려준다.
    public void CheckLoadingComplete()
    {
        LoadingDone = Units.TrueForAll( (unit) => unit.InitDone && unit.Model.IsReady );

        if (!LoadingDone)
            return;

        // 모든 유닛들이 로드가 완료되었다고 GameInfoBase에 알려준다.
        G_GameInfo.GameInfo.SendMessage("OnLoadingComplete");
    }

    /// 살아있는 유닛수
    public int AliveCount()
    {
        int alive = 0;
        for (int i = 0; i < Units.Count; i++)
        {
            if (!Units[i].CharInfo.IsDead)
            {
                alive++;
            }
        }

        return alive;
    }

    /// 현재 유닛 컨트롤이 가능한 대상을 얻어온다.
    public Unit CurLeaderUnit()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            if (!Units[i].CharInfo.IsDead && Units[i].IsLeader)
                return Units[i];
        }

        return null;
    }

    //< 해당 유닛을 일점사 처리해준다
    GameObject targetingEffect;
    public void SetTargetUnit(Unit target)
    {
        //< 활성화되어있는 유닛들의 일점사 대상을 설정해준다
        for (int i = 0; i < Units.Count; i++)
        {
            if (!Units[i].Usable)
                continue;

            Units[i].ForcedAttackTarget = target;

            ////< 구르기 쿨타임이 된다면 구르게해줌
            //if(Units[i].IsLeader)
            //{
            //    if (G_GameInfo._GameInfo.HudPanel != null)
            //    {
            //        if (!G_GameInfo._GameInfo.HudPanel.OnEvasionCheck())
            //            continue;
            //        else
            //            G_GameInfo._GameInfo.HudPanel.EvasionTime = 0;

            //        Units[i].ChangeState(UnitState.Evasion);
            //    }
            //}
            //else
            //    Units[i].ChangeState(UnitState.Evasion);
        }

        if (targetingEffect != null)
            Destroy(targetingEffect);

        //< 해당 유닛에게 타겟팅 이펙트를 붙여준다.
        target.SpawnEffect("Fx_Targeting", 1, target.transform, target.transform, true, (effect) =>
        {
            targetingEffect = effect.gameObject;
            targetingEffect.transform.localPosition = Vector3.up * 0.2f;
            targetingEffect.transform.localRotation = Quaternion.identity;
        });

    }

    //< 해당 유닛의 스폰 위치를 지정해준다
    public IEnumerator SetUnitSpawnPos(Unit target, Unit NewUnit, System.Action call = null)
    {
        if(SpawnGroup.ActiveSpawnGroup != null)
        {
            //< 메인 플레이어와 다음 이동타겟의 방향벡터를 구한후 그룹방향으로 먼저 체크
            Vector3 look = SpawnGroup.ActiveSpawnGroup.transform.position - target.transform.position;
            look.Normalize();

            NewUnit.transform.position = GetSpawnPos(target, look);
            NewUnit.transform.rotation = target.transform.rotation;
        }
        else
        {
            NewUnit.transform.position = GetSpawnPos(target, target.transform.forward);
            NewUnit.transform.rotation = target.transform.rotation;

        }
        
        Vector3 NowPos = NewUnit.transform.position;

        yield return null;

        //< 다르다면 왼쪽을 기준으로 다시 체크한다.
        if (NowPos != NewUnit.transform.position)
        {
            NewUnit.transform.position = GetSpawnPos(target, -target.transform.right);
            NewUnit.transform.rotation = target.transform.rotation;
            NowPos = NewUnit.transform.position;

            yield return null;

            if (NowPos != NewUnit.transform.position)
            {
                NewUnit.transform.position = GetSpawnPos(target, target.transform.right);
                NewUnit.transform.rotation = target.transform.rotation;
                NowPos = NewUnit.transform.position;

                yield return null;

                if (NowPos != NewUnit.transform.position)
                {
                    NewUnit.transform.position = GetSpawnPos(target, target.transform.forward);
                    NewUnit.transform.rotation = target.transform.rotation;
                    NowPos = NewUnit.transform.position;

                    yield return null;
                }
            }
        }


        if (call != null)
            call();
    }

    //< 소환할시에 해당 방향의 스폰가능한 위치를 리턴해준다
    Vector3 GetSpawnPos(Unit leader, Vector3 look)
    {
        //< 전방에 위치할수있는지 검사한다
        NavMeshHit navHit;

        Vector3 TargetPos = leader.transform.position + look * Random.Range(3.2f, 3.5f);
        if (NavMesh.SamplePosition(TargetPos, out navHit, Vector3.Distance(TargetPos, transform.position), 9))
            TargetPos = navHit.position;

        return TargetPos;
    }
    
    /// 실제 유닛이 로드 되어 유닛을 셋팅 하는 함수
    public void AddUnit(Unit unit)
    {
        switch (unit.UnitType)
        {
            case UnitType.Unit:
                {
				if (unit.IsPartner == true) {
				    //Debug.LogWarning("2JW : Partner Adding. " + unit, unit);
				    Partners.Add(unit as Pc);
                        break;
				}

                    //Pc pc = unit as Pc;

                    Units.Add(unit as Pc);
                }
                break;
        }
    }

    /// 콤보 카운팅을 해주도록 한다.
    public int AddCombo()
    {
        // 연속콤보유효시간이 지났다면, 콤보초기화
        if (Time.time >= NextComboTime)
        {
            MaxCombo = Mathf.Max( CurCombo, MaxCombo );
            CurCombo = 0;            
        }
        NextComboTime = Time.time + ComboTieCheckTime;

        return ++CurCombo;
    }

    //< 유닛 스폰
    private GameObject SpawnPc(byte teamID, PlayerUnitData unitSyncData, bool isPartner = false)
    {
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = transform.rotation;

        ////게임 인포를 통한 유닛 생성   
        return G_GameInfo.GameInfo.SpawnUnit(this.AccountUUID, teamID, unitSyncData, spawnPos, spawnRot, isPartner);
    }
    
    /// <summary>
    /// 파트너 스폰
    /// </summary>
    private GameObject SpawnPartner(byte teamID, PlayerUnitData unitSyncData, Unit leader, int idx = 0)
    {
        //따라다닐 더미 생성해서 leader에 붙이고 더미를 타겟으로 넣게 수정
        Unit DummyUnit = new GameObject().AddComponent<Unit>();
        DummyUnit.gameObject.name = string.Format("DummyUnit_{0}", idx);
        Transform dummyTr = DummyUnit.gameObject.transform;
        dummyTr.parent = leader.transform;
        dummyTr.transform.localScale = Vector3.one;

        Vector3 spawnPos = transform.position + (idx == 0 ? new Vector3(1.5f, 0, -1.5f) : new Vector3(-1.5f, 0, -1.5f));
        Quaternion spawnRot = transform.rotation;

        dummyTr.transform.position = spawnPos;

        ////게임 인포를 통한 유닛 생성   
        return G_GameInfo.GameInfo.SpawnPartner(this.AccountUUID, teamID, unitSyncData, spawnPos, spawnRot, true, DummyUnit);
    }
}
