using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class SpawnUnitData
{
    public int idx;
    public GameObject FirstObj;
    public GameObject SpawnUnit;
}

public class ShowTargetData
{
    public Unit Targetunit;
    public Vector3 SetPos;
}
*/
public class RaidBossAIBase : MonoBehaviour
{

    public enum PattenType
    {
        TimeLater = 101,//스폰 후 시간

        MyHp = 201,//자신 체력
        EnemyTotalHp = 211,//적의 평균 체력

        LifeCount = 301,//생존한 아군 수
        EnemyLifeCount = 311,//생존한 적군 수

        DeadCount = 401,//사망한 아군 수
        EnemyDeadCount = 411,//사망한 적군 수
    }

    public enum ConditionType
    {
        Over = 1,//이상
        Under = 2,//이하
    }

    public enum ActionType
    {
        MovePoint = 1,//특정 박스포인트로 이동 BoxID
        Spawn = 2,//소환 BoxID
        Skill = 3,//SkillID
    }

    public class PattenData
    {
        public PattenType PaType;
        public ConditionType ConType;
        public ActionType AcType;
        public byte GroupNumber;
        public ushort PattenValue;
        public float RunTime;
        public uint ActionValue;



        public PattenData(byte group, uint pattenType, byte condition, ushort pattenValue, byte actionType, uint actionValue)
        {
            GroupNumber = group;
            PaType = (PattenType)pattenType;
            ConType = (ConditionType)condition;
            PattenValue = pattenValue;
            AcType = (ActionType)actionType;
            ActionValue = actionValue;

            RunTime = 0;
        }

        public void PattenInit()
        {
            RunTime = 0;
        }

        public void TimeUpdate()
        {
            RunTime += Time.deltaTime;
        }
    }

    public Npc parent = null;
    public List<PattenData> PattenList = new List<PattenData>();
    //public Dictionary<uint, Transform> PattenPosDic = new Dictionary<uint, Transform>();//보류 조건

    // Use this for initialization
    void Start()
    {
        parent = GetComponent<Npc>();

        Mob.MobInfo monInfo = _LowDataMgr.instance.GetMonsterInfo(parent.NpcLowID);
        List<Mob.PattenInfo> pattenList = _LowDataMgr.instance.GetLowDataPattenList(monInfo.Pattenid);

        List<uint> skillList = new List<uint>();
        for (int i = 0; i < pattenList.Count; i++)
        {
            Mob.PattenInfo pattenInfo = pattenList[i];
            PattenData data = new PattenData(pattenInfo.Typerank, pattenInfo.MainType, pattenInfo.SubType, pattenInfo.TypeValue, pattenInfo.ActionType, pattenInfo.Actionvalue);
            if (data.AcType == ActionType.Skill)
            {
                skillList.Add(pattenInfo.Actionvalue);
                //data.SkillData.Init()
            }
            //else if(data.AcType == ActionType.MovePoint)//보류 조건
            //{
            //    GameObject go = GameObject.Find(string.Format("BossPattenPos_{0}", data.ActionValue));
            //    if (go == null)
            //    {
            //        Debug.LogError(string.Format("not found BossPattenPos_{0} error", data.ActionValue));
            //        continue;
            //    }

            //    PattenPosDic.Add(data.ActionValue, go.transform);
            //}

            PattenList.Add(data);
        }

        //SkillCtlr.InitBossRaidSkill(parent, skillList.ToArray() );
        if(0 < skillList.Count)
            parent.SkillCtlr.AddRaidSkill(skillList.ToArray());
    }

    public virtual bool PattenUpdate()
    {
        bool isPatten = false;
        int count = PattenList.Count;
        for (int i = 0; i < count; i++)
        {
            bool isAction = false;
            PattenData data = PattenList[i];
            float value = data.PattenValue;

            switch (data.PaType)
            {
                case PattenType.TimeLater://소환된 후 시간
                    value = value * 0.01f;//100 == 1
                    if (value <= data.RunTime)//발동 조건
                        isAction = true;

                    break;

                case PattenType.MyHp://현재 체력
                    {
                        value = value * 0.1f;//10 == 1
                        float con = parent.CharInfo.Hp / parent.CharInfo.MaxHp;
                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= con)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (con < value)//발동 조건
                                isAction = true;
                        }
                    }
                    break;

                case PattenType.EnemyTotalHp://적의 총 체력 평균
                    {
                        int totalHp = 0, totalMaxHp = 0;
                        int playerCount = NetData.instance._playerSyncData.playerSyncDatas.Count;
                        for (int j = 0; j < playerCount; j++)
                        {
                            PlayerUnitData unitData = NetData.instance._playerSyncData.playerSyncDatas[j];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(unitData._AccountUUID);
                            totalHp += unit.CharInfo.Hp;
                            totalMaxHp += unit.CharInfo.MaxHp;
                        }

                        int parCount = NetData.instance._playerSyncData.partnerSyncDatas.Count;
                        for (int j = 0; j < parCount; j++)
                        {
                            PlayerUnitData parData = NetData.instance._playerSyncData.partnerSyncDatas[i];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(parData._AccountUUID);
                            totalHp += unit.CharInfo.Hp;
                            totalMaxHp += unit.CharInfo.MaxHp;
                        }

                        value = value * 0.1f;//10 == 1
                        float con = totalHp / totalMaxHp;
                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= con)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (con < value)//발동 조건
                                isAction = true;
                        }
                    }
                    break;

                case PattenType.LifeCount://팀의 살아있는 수 <- 몬스터
                    {
                        int teamLiveCount = 0;

                        int teamCount = NetData.instance._playerSyncData.playerSyncDatas.Count;
                        for (int j = 0; j < teamCount; j++)
                        {
                            PlayerUnitData teamData = NetData.instance._playerSyncData.playerSyncDatas[i];
                            if (teamData._TeamID == parent.TeamID)   //같은편
                            {
                                Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(teamData._AccountUUID);
                                if (!unit.IsDie) //살아있따
                                    teamLiveCount++;
                            }
                        }
                        int parCount = NetData.instance._playerSyncData.partnerSyncDatas.Count;
                        for (int j = 0; j < parCount; j++)
                        {
                            PlayerUnitData parData = NetData.instance._playerSyncData.partnerSyncDatas[i];
                            if (parData._TeamID == parent.TeamID)    //같은편
                            {
                                Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(parData._AccountUUID);
                                if (!unit.IsDie) //살아있따
                                    teamLiveCount++;
                            }

                        }

                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= teamLiveCount)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (teamLiveCount < value)//발동 조건
                                isAction = true;
                        }

                    }

                    break;

                case PattenType.EnemyLifeCount://적의 살아있는 수
                    {
                        int liveCount = 0;
                        int playerCount = NetData.instance._playerSyncData.playerSyncDatas.Count;
                        for (int j = 0; j < playerCount; j++)
                        {
                            PlayerUnitData unitData = NetData.instance._playerSyncData.playerSyncDatas[j];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(unitData._AccountUUID);
                            if (!unit.IsDie)    //살았다
                                liveCount++;
                        }

                        int parCount = NetData.instance._playerSyncData.partnerSyncDatas.Count;
                        for (int j = 0; j < parCount; j++)
                        {
                            PlayerUnitData parData = NetData.instance._playerSyncData.partnerSyncDatas[i];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(parData._AccountUUID);
                            if (!unit.IsDie)    //살았다
                                liveCount++;
                        }

                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= liveCount)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (liveCount < value)//발동 조건
                                isAction = true;
                        }
                    }
                    break;

                case PattenType.EnemyDeadCount://적의 죽어있는 수
                    {
                        int deadCount = 0;
                        int playerCount = NetData.instance._playerSyncData.playerSyncDatas.Count;
                        for (int j = 0; j < playerCount; j++)
                        {
                            PlayerUnitData unitData = NetData.instance._playerSyncData.playerSyncDatas[j];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(unitData._AccountUUID);
                            if (unit.IsDie)    //죽었다
                                deadCount++;
                        }

                        int parCount = NetData.instance._playerSyncData.partnerSyncDatas.Count;
                        for (int j = 0; j < parCount; j++)
                        {
                            PlayerUnitData parData = NetData.instance._playerSyncData.partnerSyncDatas[i];
                            Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(parData._AccountUUID);
                            if (unit.IsDie)    //죽었다
                                deadCount++;
                        }

                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= deadCount)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (deadCount < value)//발동 조건
                                isAction = true;
                        }
                    }
                    break;

                case PattenType.DeadCount://팀의 죽어있는 수 <- 몬스터
                    {
                        int teamDeadCount = 0;

                        int teamCount = NetData.instance._playerSyncData.playerSyncDatas.Count;
                        for (int j = 0; j < teamCount; j++)
                        {
                            PlayerUnitData teamData = NetData.instance._playerSyncData.playerSyncDatas[i];
                            if (teamData._TeamID == parent.TeamID)   //같은편
                            {
                                Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(teamData._AccountUUID);
                                if (unit.IsDie ) //죽었다
                                    teamDeadCount++;
                            }
                        }
                        int parCount = NetData.instance._playerSyncData.partnerSyncDatas.Count;
                        for (int j = 0; j < parCount; j++)
                        {
                            PlayerUnitData parData = NetData.instance._playerSyncData.partnerSyncDatas[i];
                            if (parData._TeamID == parent.TeamID)    //같은편
                            {
                                Unit unit = G_GameInfo.CharacterMgr.FindRoomUnit(parData._AccountUUID);
                                if (unit.IsDie) //죽었다
                                    teamDeadCount++;
                            }

                        }

                        if (data.ConType == ConditionType.Over)
                        {
                            if (value <= teamDeadCount)//발동 조건
                                isAction = true;
                        }
                        else if (data.ConType == ConditionType.Under)
                        {
                            if (teamDeadCount < value)//발동 조건
                                isAction = true;
                        }

                    }

                    break;
            }

            if (!isAction)//발동 조건 미흡
                continue;

            switch (data.AcType)
            {
                //case ActionType.MovePoint ://보류 조건
                //Transform movePos = null;
                //if(PattenPosDic.TryGetValue(data.ActionValue, out movePos))
                //{
                //    isPatten = parent.MovePosition(movePos.position);
                //    if (isPatten)
                //        Debug.Log("<color=yellow>Patten success skill id </color>" + data.ActionValue);
                //}

                //break;

                case ActionType.Skill:
                    for (int j = 0; j < parent.SkillCtlr.SkillList.Length; j++)
                    {
                        Skill sk = parent.SkillCtlr.SkillList[j];
                        if (sk == null || !sk.IsPatten || sk.GetSkillIndex() != data.ActionValue)
                            continue;

                        isPatten = parent.UseSkill(j);
                        if (isPatten)
                            Debug.Log("<color=yellow>Patten success skill id </color>" + data.ActionValue);
                        break;
                    }

                    //isPatten = UseSkill(data.ActionValue);
                    break;

                case ActionType.Spawn:
                    isPatten = G_GameInfo.GameInfo.spawnCtlr.MenualSpawnGroup((int)data.ActionValue);
                    if (isPatten)
                    {
                        Debug.Log("<color=yellow>Patten success Spawn Group id </color>" + data.ActionValue);
                        PattenList.Remove(data);//소환 성공했으니 해당 데이터 더 이상 사용하지 않는다. 추후에는 변경될지도 모름.
                    }
                    break;
            }

            if (isPatten)
                break;
        }

        return isPatten;
    }

    void Update()
    {
        int count = PattenList.Count;
        for (int i = 0; i < count; i++)
        {
            PattenList[i].TimeUpdate();
        }
    }

    public void EndPattenSkill(uint slot)
    {
        if (PattenList.Count <= 0)
            return;

        if (parent.SkillCtlr.SkillList.Length <= slot || parent.SkillCtlr.SkillList[slot] == null)
            return;

        uint skillIdx = parent.SkillCtlr.SkillList[slot].GetSkillIndex();
        for (int i = 0; i < PattenList.Count; i++)
        {
            PattenData data = PattenList[i];
            if (data.AcType != ActionType.Skill)
                continue;

            if (data.ActionValue != skillIdx)
                continue;

            data.PattenInit();
            Debug.Log("<color=yellow>end patten init skill id </color>" + data.ActionValue);
            break;
        }
    }

    /*
    bool UseSkill(uint skillIdx)
    {
        bool useSKill = false;
        for (int j = 0; j < SkillCtlr.SkillList.Length; j++)
        {
            Skill skill = SkillCtlr.SkillList[j];
            if (skill == null)
                continue;

            if (skill.GetSkillIndex() != skillIdx)
                continue;

            if (skill._SkillType == eActiveSkillType.Attack)
            {
                SkillActiveCondition condition = skill.GetCondition();
                if (condition == SkillActiveCondition.eAvailable || condition == SkillActiveCondition.eFarFromTarget)
                {
                    //그냥사용
                    if (SkillCtlr.IsAbleSkill(j + 1))
                    {
                        parent.ChangeState(UnitState.Skill);
                        useSKill = true;
                    }
                    else
                    {
                        Debug.LogError("들어오면 안됨2");
                    }
                }
            }
            else if (skill._SkillType == eActiveSkillType.Buff)
            {
                if (skill.GetCondition() == SkillActiveCondition.eAvailable)
                {
                    //< 공격할 대상이 화면안에 들어올때 버프를 사용함 - 버프가 왜??? 이건 버프 = 디버프라는 구조에서나 이렇게
                    //if (parentUnit.GetTarget() == null || (parentUnit.transform.position - parentUnit.GetTarget().transform.position).magnitude > 20)
                    //    continue;

                    //< 스킬 시전!
                    if (SkillCtlr.IsAbleSkill(j + 1))
                    {
                        useSKill = true;
                    }
                    else
                    {
                        Debug.LogError("들어오면 안됨4");
                    }
                }
            }
            else if (skill._SkillType == eActiveSkillType.Heal)
            {
                //힐은 일단 없다긴 하지만
                if (skill.GetCondition() == SkillActiveCondition.eAvailable)
                {
                    //< 스킬 시전!
                    if (SkillCtlr.IsAbleSkill(j + 1))
                    {
                        useSKill = true;
                    }
                    else
                    {
                        Debug.LogError("들어오면 안됨5");
                    }
                }
            }

            break;
        }

        return useSKill;
    }
    */

    /*
    static RaidBossAIBase CreateRaidBossAI;
    public static void SetRaidBossAI(byte areaType, Unit _RaidBoss)
    {
        ActionLive = false;
        if (_RaidBoss.UnitType != UnitType.Boss)
            return;

        if (G_GameInfo.GameMode != GAME_MODE.RAID)
            return;

        //< 헤카톤
        if(areaType == 1)
            CreateRaidBossAI = _RaidBoss.gameObject.AddComponent<RaidBossAI_Forest>();
        //< 타일런트
        else if (areaType == 2)
            CreateRaidBossAI = _RaidBoss.gameObject.AddComponent<RaidBossAI_Poison>();
        //< 아카드
        else if (areaType == 3)
            CreateRaidBossAI = _RaidBoss.gameObject.AddComponent<RaidBossAI_Ice>();
        //< 허큘러스
        else if (areaType == 4)
            CreateRaidBossAI = _RaidBoss.gameObject.AddComponent<RaidBossAI_Metal>();
        //< 불카누스
        else if (areaType == 5)
            CreateRaidBossAI = _RaidBoss.gameObject.AddComponent<RaidBossAI_Fire>();

        //< 셋업
        CreateRaidBossAI.Setup(_RaidBoss);
    }

    public static void EndRaidAI()
    {
        if(CreateRaidBossAI != null)
        {
            CreateRaidBossAI.EndAI();
            CreateRaidBossAI = null;
        }
    }

    //< 카메라 셰이크 데이터
    public class CameraShakeData
    {
        public float Delay;
        public byte type;
        public int numberOfShakes;
    }

    //< 각 패턴별로 정보가 들어가있다.
    public class PatternData
    {
        public float HPvalue;                   //< 체력 제한
        public float TimeValue;                 //< 반복할 시간
        public float SaveTimeValue;       //< 시간에 쓰이기위해 저장
        public bool TimeReplay;                 //< 시간마다 반복하게 할것인지?
        public bool BossStatusCheck;            //< 패턴 실행할시에 보스의 상태를 체크해야하는지
        public System.Action<PatternData> CB;   //< 실행시 호출할 함수

        public PatternData(float _HPvalue, bool _BossStatusCheck, bool _TimeReplay, float _time, System.Action<PatternData> _CB)
        {
            HPvalue = _HPvalue;
            SaveTimeValue = TimeValue = _time;
            TimeReplay = _TimeReplay;
            BossStatusCheck = _BossStatusCheck;
            CB = _CB;
        }
    }

    //< 현재 패턴이 진행중인지
    public static bool ActionLive = false;
    public static bool NotTargeting = false;
    bool Init = false;
    
    //< 실질적인 패턴을 넣는 부분
    List<PatternData> PatternList = new List<PatternData>();

    //< 실행을 하나도 무시 안되고 하기위해 큐로 저장
    Queue<PatternData> ActiveQueue = new Queue<PatternData>();

    List<PatternData> RePlayLists = new List<PatternData>();

    //< 대상 보스
    protected Unit RaidBoss;

    //< 레이드 타입에 따라 셋팅
    GameObject RaidTargetCamera;
    public void Setup(Unit _RaidBoss)
    {
        RaidBoss = _RaidBoss;

        SetPatternData();
        SetGameData();

        if (GameDefine.TestMode)
            Debug.Log("레이드 패턴 셋업 완료 " + PatternList.Count + "개");

        //< 업데이트 시작
        StartCoroutine(UsePatternUpdate());
        StartCoroutine(RePlayUpdate());

        NotTargeting = false;
        Init = true;

        //< 타겟팅 카메라를 로드해놓는다.
        RaidTargetCamera = (Instantiate(Resources.Load("Camera/RaidTargetCamera")) as GameObject);

        //< 이벤트 연결
        EventListner.instance.RegisterListner("Revive", RevivePlayer);
        EventListner.instance.RegisterListner("UnitAllDead", UnitAllDead);
    }

    void OnDestroy()
    {
        NotTargeting = false;
        //< 이벤트 삭제
        EventListner.instance.RemoveEvent("Revive", RevivePlayer);
        EventListner.instance.RemoveEvent("UnitAllDead", UnitAllDead);
    }

    //< 패턴에 관련된 데이터를 넣는다
    public virtual void SetPatternData(){ }

    //< 게임 설정 작업
    public virtual void SetGameData() { }

    //< 패턴을 추가한다.
    public void AddPattern(PatternData data)
    {
        PatternList.Add(data);
    }

    //< 카메라 셰이크를 추가해준다
    List<CameraShakeData> CameraShakes = new List<CameraShakeData>();
    public void AddCameraShake(float delay, byte type, int _numberOfShakes = 0)
    {
        CameraShakeData nData = new CameraShakeData();
        nData.Delay = delay;
        nData.type = type;
        nData.numberOfShakes = _numberOfShakes;
        CameraShakes.Add(nData);
    }

    void Update()
    {
        if (!Init || RaidBoss == null || RaidBoss.IsDie)
            return;

        //< 따로 사용해야할것이 있다면 이쪽에서 업데이트 처리
        PatternUpdate();

        //< 타겟 업데이트
        ShowTargetUpdate();

        //< 카메라 쉐이크 처리
        for (int i = 0; i < CameraShakes.Count; i++ )
        {
            CameraShakes[i].Delay -= Time.deltaTime;
            if(CameraShakes[i].Delay <= 0)
            {
                CameraManager.instance.Shake(RaidBoss.transform.forward, CameraShakes[i].type, null, CameraShakes[i].numberOfShakes);
                CameraShakes.RemoveAt(i);
                i--;
            }
        }

        //< 패턴 시작 조건 체크
        if (PatternList.Count > 0)
        {
            //< 체력 조건이 되었다면 패턴 실행
            if (((float)RaidBoss.CharInfo.Hp / (float)RaidBoss.CharInfo.MaxHp) <= PatternList[0].HPvalue)
                UsePattern(PatternList[0]);
        }
    }

    public void EndAI()
    {
        NotTargeting = false;
        ActionLive = false;
        StopAllCoroutines();
        ShowTargetCamera(false, null, Vector3.zero, true);
    }

    List<GameObject> UseEffectLists = new List<GameObject>();
    public void SpawnEffect(string effectname, Transform posAndRot = null, Transform parent = null, System.Action<Transform> call = null)
    {
        G_GameInfo.SpawnEffect(effectname, 1, posAndRot, parent, Vector3.one, (effect) =>
        {
            UseEffectLists.Add(effect.gameObject);

            //< 이벤트 Layer로 설정
            NGUITools.SetLayer(effect.gameObject, 13);
            if (call != null)
                call(effect);
        });
    }

    public void UseEffectClear()
    {
        for(int i=0; i<UseEffectLists.Count; i++)
        {
            if (UseEffectLists[i] == null)
                continue;

            UseEffectClear(UseEffectLists[i]);
        }
    }

    public void UseEffectClear(GameObject effect)
    {
        if (effect == null)
            return;

        FxMakerPoolItem obj = effect.GetComponent<FxMakerPoolItem>();
        if (obj != null)
            obj.ManualDespawn();
        else
        {
            if (!G_GameInfo.EffectPool.IsSpawned(effect.transform))
                Destroy(effect.gameObject);
            else
                G_GameInfo.EffectPool.Despawn(effect.transform, G_GameInfo.EffectPool.group);
        }
    }

    //< 패턴을 실행한다
    void UsePattern(PatternData SelectPattern)
    {
        //< 타임 반복이라면 리스트에 넣어줌
        if (SelectPattern.TimeReplay)
            RePlayLists.Add(SelectPattern);
        else
        {
            //< 패턴을 실행할때 보스의 상태를 체크해야한다면 큐에 추가
            if (SelectPattern.BossStatusCheck)
                ActiveQueue.Enqueue(SelectPattern);
            else
            {
                ActionLive = true;
                SelectPattern.CB(SelectPattern);
            }
        }

        NextPattern();
    }

    //< 다음 패턴을 찾는다.
    void NextPattern()
    {
        if (PatternList.Count == 0)
            return;

        PatternList.RemoveAt(0);
    }

    //< 패턴을 실행했을시 사용할수있는지 체크한후 패턴을 실행시킨다.
    IEnumerator UsePatternUpdate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while(true)
        {
            yield return wait;

            //< 큐가 쌓였다면 빼서 대기해준다.
            if(ActiveQueue.Count > 0)
            {
                PatternData ReadyData = ActiveQueue.Dequeue();
                while(true)
                {
                    yield return wait;

                    //<==========================================
                    //<     패턴을 실행할수있는지 검사한다.
                    //<==========================================

                    //< 1. 보스의 스테이트가 패턴을 실행시킬수없는지?
                    if (RaidBoss.FSM.Current_State == UnitState.Skill)
                        continue;

                    //< 2. 현재 카메라 연출을 쓰고있는지?
                    else if (SkillEventMgr.ActiveEvent)
                        continue;

                    //< 3. 패턴 이벤트가 실행중인가?
                    else if (ActionLive)
                        continue;

                    //< 4. 리더가 없는가
                    else if (G_GameInfo.PlayerController.Leader == null)
                        continue;

                    //< 5. 게임이 종료상태라면
                    else if (G_GameInfo._GameInfo.isEnd)
                        continue;

                    break;
                }

                //< 시전해준다.
                if (ReadyData.TimeReplay)
                    RePlayLists.Add(ReadyData);
                else
                {
                    ActionLive = true;
                    ReadyData.CB(ReadyData);
                }
            }
        }
    }

    //< 패턴을 시간마다 계속 업데이트 해야한다면 이곳이서 처리
    IEnumerator RePlayUpdate()
    {
        while(true)
        {
            yield return null;

            if (RePlayLists.Count > 0)
            {
                //< 1. 패턴 이벤트가 실행중인가?
                //if (ActionLive)
                //    continue;

                //< 2. 리더가 없는가
                if (G_GameInfo.PlayerController.Leader == null)
                    continue;

                //< 3. 게임이 종료상태라면
                else if (G_GameInfo._GameInfo.isEnd)
                    continue;

                for (int i = 0; i < RePlayLists.Count; i++)
                {
                    RePlayLists[i].TimeValue -= Time.deltaTime;
                    if (RePlayLists[i].TimeValue <= 0)
                    {
                        RePlayLists[i].TimeValue = RePlayLists[i].SaveTimeValue;

                        if (RePlayLists[i].BossStatusCheck)
                            ActiveQueue.Enqueue(RePlayLists[i]);
                        else
                            RePlayLists[i].CB(RePlayLists[i]);
                    }
                }
            }
        }
    }

    public virtual void PatternUpdate(){}

    //< 고정 패턴 시작 함수
    public virtual void ActivePattern(PatternData _PatternData) 
    {
        if (GameDefine.TestMode)
            Debug.Log("ActivePattern 실행");
    }

    //< 랜덤 패턴 시작함수
    public virtual void ActiveRandomPattern(PatternData _PatternData) 
    {
        if (GameDefine.TestMode)
            Debug.Log("ActiveRandomPattern 실행");
    }

    //< 플레이어 푸쉬
    public void PlayerPush()
    {
        List<Unit> lists = G_GameInfo.CharacterMgr.AliveList(0);
        for (int i = 0; i < lists.Count; i++)
        {
            if (!MathHelper.IsInRange((lists[i].transform.position - RaidBoss.transform.position), 20, RaidBoss.Radius, lists[i].Radius))
                continue;

            lists[i].SetPush(6, RaidBoss.gameObject, 2);
        }
    }

    //< 이펙트등에 컬리더박스를 연결해놓는다.
    public void SetTriggerEvent(GameObject effect, System.Action<Unit, ForwardTriggerEvent> CB)
    {
        Collider[] existCol = effect.GetComponentsInChildren<Collider>(true);
        for(int i=0; i<existCol.Length; i++)
        {
            ForwardTriggerEvent OrgtriggerEvt = existCol[i].gameObject.GetComponent<ForwardTriggerEvent>();
            if (OrgtriggerEvt != null)
                UIMgr.Destroy(OrgtriggerEvt);

            existCol[i].isTrigger = false;
            existCol[i].isTrigger = true;

            ForwardTriggerEvent triggerEvt = existCol[i].gameObject.AddComponent<ForwardTriggerEvent>();
            triggerEvt.Setup(RaidBoss);
            triggerEvt.TriggerEnter_Unit = CB;
        }
    }

    //< 이펙트등에 컬리더박스를 연결해놓는다(연속)
    public void SetTriggerEvent_Stay(GameObject effect, float Delay, System.Action<Unit, ForwardTriggerEvent> CB)
    {
        Collider[] existCol = effect.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < existCol.Length; i++)
        {
            ForwardTriggerEvent OrgtriggerEvt = existCol[i].gameObject.GetComponent<ForwardTriggerEvent>();
            if (OrgtriggerEvt != null)
                UIMgr.Destroy(OrgtriggerEvt);

            existCol[i].isTrigger = false;
            existCol[i].isTrigger = true;

            ForwardTriggerEvent triggerEvt = existCol[i].gameObject.AddComponent<ForwardTriggerEvent>();
            triggerEvt.Setup(RaidBoss);
            triggerEvt.StayDelay = Delay;
            triggerEvt.TriggerStay_Unit = CB;
        }
    }

    public void AllPlayerTakeDamage(float dam, bool notPush = false)
    {
        List<Unit> aliveList = G_GameInfo.CharacterMgr.AliveList(0);
        for (int i = 0; i < aliveList.Count; i++)
            PlayerTakeDamage(aliveList[i], dam, notPush);
    }

    public void PlayerTakeDamage(Unit player, float dam, bool notPush = false)
    {
        if (player == null || player.IsDie || RaidBoss == null || RaidBoss.IsDie || player.TeamID != 0)
            return;

        //< 플레이어에게 대미지를 줌
        player.TakeDamage(RaidBoss, 1, dam, 0, eAttackType.All, true);

        //< 푸쉬도 줌
        if (!notPush)
            player.SetPush(3, RaidBoss.gameObject, 1);
    }

    public static Unit SelectCameraTarget;
    List<ShowTargetData> ShowTargetList = new List<ShowTargetData>();
    public void ShowTargetCamera(bool type, GameObject target, Vector3 Pos,  bool Coercion = false)
    {
        //< 시작일경우 리스트에 추가해줌
        if (type)
        {
            ShowTargetData nData = new ShowTargetData();
            nData.Targetunit = target.GetComponent<Unit>();
            nData.SetPos = Pos;
            ShowTargetList.Add(nData);
        }
        else if (!type && Coercion)
        {
            ShowTargetList.Clear();

            SelectCameraTarget = null;
            RaidTargetCamera.SetActive(false);
        }
        else if (!type)
        {
            ShowTargetData nData = ShowTargetList.Find(data => data.Targetunit.gameObject == target);
            if (nData != null)
                ShowTargetList.Remove(nData);

            if(ShowTargetList.Count == 0)
            {
                SelectCameraTarget = null;
                RaidTargetCamera.SetActive(false);
            }
        }
    }

    void ShowTargetUpdate()
    {
        if (ShowTargetList.Count == 0)
        {
            SelectCameraTarget = null;
            return;
        }

        if (SelectCameraTarget == null || SelectCameraTarget.CurrentState == UnitState.Dead)
        {
            SelectCameraTarget = null;

            //< 다음 타겟을 찾는다
            for(int i=0; i<ShowTargetList.Count; i++)
            {
                if (ShowTargetList[i].Targetunit == null || ShowTargetList[i].Targetunit.IsDie)
                {
                    ShowTargetList.RemoveAt(i);
                    i--;
                }
                else
                {
                    SelectCameraTarget = ShowTargetList[i].Targetunit;

                    //< 위치 수정
                    RaidTargetCamera.SetActive(true);
                    RaidTargetCamera.transform.position = SelectCameraTarget.transform.position + (ShowTargetList[i].Targetunit.transform.forward * ShowTargetList[i].SetPos.x);
                    RaidTargetCamera.transform.position += Vector3.up * ShowTargetList[i].SetPos.y;
                    RaidTargetCamera.transform.LookAt(ShowTargetList[i].Targetunit.transform.position + Vector3.up * ShowTargetList[i].SetPos.z);
                    break;
                }
            }
        }

        //< 정보를 꺼준다.
        if(SelectCameraTarget == null || ShowTargetList.Count == 0)
        {
            RaidTargetCamera.SetActive(false);
        }
    }

    //< 유닛이 모두 죽었을시 호출
    void UnitAllDead(object obj)
    {
        RaidTargetCamera.SetActive(false);
    }

    //< 유닛이 부활했을시 호출
    void RevivePlayer(object obj)
    {
        if(SelectCameraTarget != null)
            RaidTargetCamera.SetActive(true);
    }

    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform[] t = go.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < t.Length; ++i)
        {
            t[i].gameObject.layer = layer;
        }
    }
    */
}
