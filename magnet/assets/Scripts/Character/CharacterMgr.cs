using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// InGame플레이시 모든 캐릭터, 유닛, NPC 관리
/// </summary>
/// <remarks>
/// PC == Character (3개 파츠로 조합되어 만들어짐)
/// NPC == Unit,Monster,Boss, etc...
/// </remarks>
public class CharacterMgr : MonoBehaviour
{
    /// 관리되던 유닛이 살거나 추가되면 호출될 콜백함수
    public System.Action<Unit> AddUnitCallback;

    /// 관리되던 유닛이 죽거나 삭제되면 호출될 콜백함수
    public System.Action<Unit> RemovUnitCallback;    

    /// 인게임 상에 존재하는 모든 유닛들 저장
    public Dictionary<int, Unit> allUnitDic = new Dictionary<int, Unit>();

    //유저들의 리스트
    public Dictionary<ulong, Unit> allrUUIDDic = new Dictionary<ulong, Unit>();

    //NPC - 몬스터등의 리스트
    public Dictionary<ulong, Unit>  NPCDic= new Dictionary<ulong, Unit>();

    public List<Unit> allUnitList = new List<Unit>();

    public Boss BossUnit = null;


    //플레이어 관리차원의 리스트
    public Dictionary<ulong, Unit> RoomUserAllDic = new Dictionary<ulong, Unit>();

	List<SkillController> SkillControllers = new List<SkillController>();

    public void RoomPlayerEnter(ulong uuID, Unit Player)
    {
        if( RoomUserAllDic.ContainsKey(uuID) )
        {
            //데이터에 이미 있음 무시
        }
        else
        {
            //데이터에 없음
            RoomUserAllDic.Add(uuID, Player);
        }
    }
    public void RoomPlayerLeave(ulong uuID)
    {
        if (RoomUserAllDic.ContainsKey(uuID))
        {
            //데이터에 이미 있음 무시
            RoomUserAllDic.Remove(uuID);
        }
    }
    public Unit GetRoomPlayer(ulong uuID)
    {
        if (RoomUserAllDic.ContainsKey(uuID))
        {
            Unit player = null;
            if(RoomUserAllDic.TryGetValue(uuID, out player))
            {
                return player;
            }
        }

        return null;
    }

    float HpRegenTime = 0;
    void Update()
    {
        //< HP리젠을 시켜준다
        if ((Time.time - HpRegenTime) >= 1)
        {
            HpRegenTime = Time.time;
            for (int i = 0; i < allUnitList.Count; i++)
            {
                //if (allUnitList[i] != null && allUnitList[i].CurrentState != UnitState.Dying && allUnitList[i].CurrentState != UnitState.Dead && allUnitList[i].hp > 0)
                //    allUnitList[i].CharInfo.Hp += (int)allUnitList[i].CharInfo.Stats[AbilityType.HpRegen].FinalValue;
            }
        }

        //< 버프 컨트롤러 업데이트
        for (int i = 0; i < BuffControllers.Count; i++)
        {
		  if (BuffControllers[i].Owner != null)
		  {
			 BuffControllers[i].FixedUpdate();
		  }
		  else
		  {
			 BuffControllers.RemoveAt(i);
			 i--;
		  }
        }

        //< 스킬컨트롤러 업데이트
        for (int i=0; i<SkillControllers.Count; i++)
        {
            if (SkillControllers[i].Owner != null)
                SkillControllers[i].FixedUpdate();
            else
            {
                SkillControllers.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// 팀별로 저장
    /// eTeamType말고 다 vs 다를 위해 번호별로 관리하려함.
    /// </summary>
    public Dictionary<byte, List<Unit>> allTeamDic = new Dictionary<byte, List<Unit>>();

    /// 유닛 타입별로 저장
    public Dictionary<UnitType, List<Unit>> allTypeDic = new Dictionary<UnitType, List<Unit>>();

    public Dictionary<int, List<Unit>> unitGroupDic = new Dictionary<int, List<Unit>>();
    
    public void Reset()
    {
        allrUUIDDic.Clear();
        allUnitDic.Clear();
        allTypeDic.Clear();
        allTeamDic.Clear();
        unitGroupDic.Clear();

        NPCDic.Clear();

        AddUnitCallback = null;
        RemovUnitCallback = null;
    }

    #region Buff & Skill
    List<BuffController> BuffControllers = new List<BuffController>();

    public BuffController AddBuffCtrl(Unit Owner)
    {
        BuffController ctrl = new BuffController();
        ctrl.Owner = Owner;
        BuffControllers.Add(ctrl);

        return ctrl;
    }

    public void DeleteBuffCtrl(Unit Owner)
    {
        for(int i=0; i<BuffControllers.Count; i++)
        {
            if(BuffControllers[i].Owner == Owner)
            {
                BuffControllers.RemoveAt(i);
			    Owner.BuffCtlr = null;
                break;
            }
        }
    }
   
    public SkillController AddSkillCtrl(Unit Owner)
    {
        SkillController ctrl = new SkillController();
        ctrl.Owner = Owner;
        SkillControllers.Add(ctrl);

        return ctrl;
    }

    public void DeleteSkillCtrl(Unit Owner)
    {
        for(int i=0; i<SkillControllers.Count; i++)
        {
            if(SkillControllers[i].Owner == Owner)
            {
                SkillControllers.RemoveAt(i);
                break;
            }
        }
    }

    #endregion

    /// <summary>
    /// 관리될 유닛들을 추가합니다.
    /// </summary>
    /// <param name="unit">생성된 유닛</param>
    public void AddUnit(Unit unit)
    {
        if (!allTypeDic.ContainsKey(unit.UnitType))
            allTypeDic.Add(unit.UnitType, new List<Unit>());

        if (!allTeamDic.ContainsKey(unit.TeamID))
            allTeamDic.Add(unit.TeamID, new List<Unit>());

        allTypeDic[unit.UnitType].Add(unit);
        allTeamDic[unit.TeamID].Add(unit);
        allUnitList.Add(unit);

        // NPC만 그룹화되도록 하기 (타입별 그룹화 정해지면 모든 유닛 그룹화 하기)
        if (unit.UnitType == UnitType.Npc || unit.UnitType == UnitType.Boss)
        {
            if (!unitGroupDic.ContainsKey(unit.GroupNo))
                unitGroupDic.Add(unit.GroupNo, new List<Unit>());

            unitGroupDic[unit.GroupNo].Add( unit );

		  if (unit.UnitType == UnitType.Boss) BossUnit = unit as Boss;
        }

        if (!allUnitDic.ContainsKey(unit.GetInstanceID()))
            allUnitDic.Add(unit.GetInstanceID(), unit);

        if (null != AddUnitCallback)
            AddUnitCallback( unit );
    }

    public void AddRoomUnit(Unit unit)
    {
        if (!allrUUIDDic.ContainsKey(unit.m_rUUID))
        {
            allrUUIDDic.Add(unit.m_rUUID, unit);
        }
    }

    public Unit FindRoomUnit(ulong a_rUUID)
    {
        if (allrUUIDDic.ContainsKey(a_rUUID))
            return allrUUIDDic[a_rUUID];

        return null;
    }

    public void AddRoomNPC(Unit unit)
    {
        if (!NPCDic.ContainsKey(unit.m_rUUID))
        {
            NPCDic.Add(unit.m_rUUID, unit);
        }
    }

    public Unit FindRoomNPC(ulong a_rUUID)
    {
        if (NPCDic.ContainsKey(a_rUUID))
            return NPCDic[a_rUUID];

        return null;
    }

    public void RemoveUnit(Unit unit)
    {

        allTeamDic[unit.TeamID].Remove(unit);
        allTypeDic[unit.UnitType].Remove( unit );
        allUnitDic.Remove( unit.GetInstanceID() );
        allUnitList.Remove(unit);

        // NPC만 그룹화되도록 하기 (타입별 그룹화 정해지면 모든 유닛 그룹화 하기)
        if (unit.UnitType == UnitType.Npc || unit.UnitType == UnitType.Boss)
        {
            unitGroupDic[unit.GroupNo].Remove(unit);
            NPCDic.Remove(unit.m_rUUID);
        }
        else
        {
            allrUUIDDic.Remove(unit.m_rUUID); //Room별 UUID
        }
            

        //< 버프컨트롤러 삭제
        DeleteBuffCtrl(unit);

        //< 스킬 컨트롤러 삭제
        if (unit.UnitType != UnitType.Unit)
            DeleteSkillCtrl(unit);

        if (null != RemovUnitCallback)
            RemovUnitCallback( unit );
    }

    public bool ContainsUnit(Unit unit)
    {
        return allUnitDic.ContainsKey( unit.GetInstanceID() );
    }

    public bool ContainsUnits(params Unit[] units)
    {
        for (int i = 0; i < units.Length; i++)
        {
            if (!ContainsUnit( units[i] ))
                return false;
        }

        return true;
    }
    
    /// <summary>
    /// 무효한 타겟인지 검사
    /// </summary>
    /// <param name="netViewID">무효한지 검사할 NetViewID</param>
    /// <param name="target">무효하다면 채워짐</param>
    /// <returns>무효한 타겟 유무</returns>
    public bool InvalidTarget(int instanceID, ref Unit target)
    {
        if (GameDefine.HelperTargetSearch)
        {
            return instanceID == GameDefine.unassignedID
            || !allUnitDic.TryGetValue(instanceID, out target)
            || null == target
            || !target.Usable;
        }

        return instanceID == GameDefine.unassignedID
            || !allUnitDic.TryGetValue(instanceID, out target)
            || null == target
            || !target.Usable;
    }
    
    /// <summary>
    /// 타겟으로 삼을수 있는지 검사
    /// </summary>
    /// <param name="target">검사할 대상</param>
    public bool CanTarget(eAttackType attackType, Unit target)
    {
        if (GameDefine.HelperTargetSearch)
        {
            return null != target
            && allUnitDic.ContainsKey(target.GetInstanceID())
            && target.Usable
            && target.UnitType != UnitType.Trap
            && !target.IsHelper;
        }
        //Debug.LogWarning("2JW : CharacterMgr In CanTarget : target - " + target + " : t_instanceId - " + target.GetInstanceID() + " : t_usable - " + target.Usable + " : t_unitType - " + target.UnitType);
        return null != target
            && allUnitDic.ContainsKey(target.GetInstanceID())
            && target.Usable
            && target.UnitType != UnitType.Trap;
    }

    /// <summary>
    /// 적팀들 모두 구하기.
    /// </summary>
    /// <param name="exceptTeamID">해당팀만 제외</param>
    public List<Unit> EnemyListBy(byte exceptTeamID)
    {
        return null;
    }

    /// <summary>
    /// 살아있는 유닛들만 알려준다.
    /// </summary>
    public List<Unit> AliveList(byte teamID)
    {
        List<Unit> aliveList = new List<Unit>();

        if (!allTeamDic.ContainsKey( teamID ))
            return aliveList;

        foreach (Unit unit in allTeamDic[teamID])
        {
            if (unit.UnitType == UnitType.Prop || unit.UnitType == UnitType.Trap)
                continue;

            if (unit.Usable)
                aliveList.Add( unit );
        }

        return aliveList;
    }

    //< 모든 유닛중 살아있는 유닛을 리턴
    public List<Unit> AliveList()
    {
        List<Unit> aliveList = new List<Unit>();

        foreach (Unit unit in allTeamDic[0])
        {
            if (unit.UnitType == UnitType.Prop || unit.UnitType == UnitType.Trap)
                continue;

            if (unit.Usable)
                aliveList.Add(unit);
        }

        foreach (Unit unit in allTeamDic[1])
        {
            if (unit.UnitType == UnitType.Prop || unit.UnitType == UnitType.Trap)
                continue;

            if (unit.Usable)
                aliveList.Add(unit);
        }

        return aliveList;
    }

    public List<Unit> liveList_All(byte teamID)
    {
        List<Unit> aliveList = new List<Unit>();

        if (!allTeamDic.ContainsKey(teamID))
            return aliveList;

        foreach (Unit unit in allTeamDic[teamID])
        {
            if (unit == null)
                continue;

            if (unit.UnitType == UnitType.Trap)
                continue;

            if (unit.Usable)
                aliveList.Add(unit);
        }

        return aliveList;
    }

    /// <summary>
    /// 지정한 위치에서 가장 가까운 적을 찾아준다.
    /// </summary>
    /// <param name="exceptTeamID">해당 팀번호를 제외한 나머지 팀에 대한 탐색</param>
    /// <param name="attackType">공격 타입.</param>
    /// <param name="point">시작점</param>
    /// <param name="radius">시작점에서 찾고자하는 반경 (최대 1000m)</param>
    /// <param name="withStructure">구조물도 함께 찾을 것인지 유무</param>
    public Unit NearestTarget(byte exceptTeamID, eAttackType attackType, Vector3 point, float radius = 1000, bool withStructure = false, Unit exceptUnit = null)
    {
        //난투장 모드에서는 SkillState 한군데서만 이 함수를 사용하고 있다.

        Unit nearest = null;
        //Debug.LogWarning("2JW : CharacterMgr NearestTarget Function In - AllTeamDic Count : " + allTeamDic.Count );
        foreach (KeyValuePair<byte, List<Unit>> pair in allTeamDic)
        {
            if (exceptTeamID == pair.Key)
                continue;

            nearest = NearestTargetByTeam(pair.Key, attackType, point, radius, withStructure, exceptUnit);
        }

        //for (int i = 0; i < allUnitList.Count; i++ ) //??????????????????????? 이 부분은 부하만 잡아 먹고 있고 위ㅔ 코드로 검색만으로도 충분할 텐데 이 부분을 왜?? 했을까?
        //{
        //    if (allUnitList[i].TeamID == exceptTeamID)
        //        continue;

        //    nearest = NearestTargetByTeam(allUnitList[i].TeamID, attackType, point, radius, withStructure, exceptUnit);
        //}

        return nearest;
    }

    float PropCheckDis = 100;
    public Unit NearestTargetByTeam(byte teamID, eAttackType attackType, Vector3 point, float radius = 1000, bool withStructure = false, Unit exceptUnit = null)
    {
        if (!allTeamDic.ContainsKey(teamID))
            return null;

        float       shortestDistance = radius * radius;
        Unit        nearest = null;
        List<Unit>  unitList = allTeamDic[teamID];
        //Debug.LogWarning("2JW : " + unitList.Count);
        for (int i = 0; i < unitList.Count; i++)
        {
            Unit target = unitList[i];
            //Debug.LogWarning("2JW : for - " + i + " : " + target + " : " + attackType , target);
            if (!CanTarget(attackType, target))
                continue;
            if (!withStructure && target.UnitType == UnitType.Prop)
                continue;

            ////< 이벤트 상태라면 패스
            if (target.CurrentState == UnitState.Event)// && RaidBossAIBase.NotTargeting
                continue;

            if (exceptUnit != null && exceptUnit == target)
                continue;

            if (target.CurrentState == UnitState.Dying || target.CurrentState == UnitState.Dead)
                continue;

            if(target.UnitType == UnitType.Prop)
            {
                if ((target as Prop).PropInfo.AutoTarget == 0)
                    continue;
            }
            

            float distance = (target.transform.position - point).sqrMagnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = target;
            }
        }

        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        /*
        if(SceneManager.instance.IsRTNetwork)
        {
            if (exceptUnit != null)
            {
                if (exceptUnit.m_isOtherPc == true || exceptUnit.UnitType == UnitType.Npc)
                {
                    //서버로부터 중계되어 온 닉네임을 타겟으로 잡는다.
                    if (exceptUnit.SvSkillTgUnit != null)
                    {
                        shortestDistance = 0.1f;
                        nearest = exceptUnit.SvSkillTgUnit;
                    }
                }
                else
                {
                    //exceptUnit.TeamID //같은 팀중에서 OtherPc놈들만 찾는다.
                    List<Unit> MyTeamList = allTeamDic[exceptUnit.TeamID];
                    for (int i = 0; i < MyTeamList.Count; i++)
                    {
                        Unit target = MyTeamList[i];

                        if (target.m_isOtherPc == false)
                            continue;

                        if (!CanTarget(attackType, target))
                            continue;

                        if (!withStructure && target.UnitType == UnitType.Prop)
                            continue;

                        ////< 이벤트 상태라면 패스
                        if (target.CurrentState == UnitState.Evnet && RaidBossAIBase.NotTargeting)
                            continue;

                        if (exceptUnit != null && exceptUnit == target)
                            continue;

                        if (target.CurrentState == UnitState.Dying || target.CurrentState == UnitState.Dead)
                            continue;

                        float distance = (target.transform.position - point).sqrMagnitude;
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            nearest = target;
                        }
                    }

                }
            }
        }//if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        */

        //Debug.LogWarning("2JW : " + nearest, nearest);
        return nearest;
    }

    public Unit FindTargetByTeam(byte teamID, Unit criterionUnit, float radius = 1000, float angle = 360f, bool withStructure = false)
    {
        if (!allTeamDic.ContainsKey(teamID))
            return null;

        List<Unit>  unitList = allTeamDic[teamID];
        float       casterRadius = criterionUnit.Radius;
        eAttackType attackType = criterionUnit.CharInfo.AttackType;

        List<Unit> units = new List<Unit>();
        for (int i = 0; i < unitList.Count; i++)
        {
            Unit target = unitList[i];

            if (!CanTarget(attackType, target))
                continue;

            if (!withStructure && target.UnitType == UnitType.Prop)
                continue;
            else if (withStructure && target.UnitType == UnitType.Prop)
            {
                //< 프롭은 거리를 체크해야함
                float distance2 = (target.transform.position - criterionUnit.transform.position).sqrMagnitude;
                if (distance2 > PropCheckDis)
                    continue;
            }

            if (target.CurrentState == UnitState.Dying || target.CurrentState == UnitState.Dead)
                continue;

            Vector3 distance = (target.transform.position - criterionUnit.transform.position);

            if (!MathHelper.IsInRange(distance, radius, criterionUnit.transform.forward, angle, casterRadius, target.Radius))
                continue;

            units.Add(target);
        }

        /*
        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        if(SceneManager.instance.IsRTNetwork)
        {
            if (criterionUnit.m_isOtherPc == true)
            {
                if (criterionUnit.SvTargetUnit != null)
                {
                    units.Clear();
                    units.Add(criterionUnit.SvTargetUnit);
                }
            }
            else //if (criterionUnit.m_isOtherPc == false)
            {
                List<Unit> MyTeamList = allTeamDic[0];
                for (int i = 0; i < MyTeamList.Count; i++)
                {
                    Unit target = MyTeamList[i];

                    if (!CanTarget(attackType, target))
                        continue;

                    if (criterionUnit == target)
                        continue;

                    if (target.m_isOtherPc == false)
                        continue;

                    //{
                    //    //Debug.LogWarning(target.Owner.GetPlayerNickName());
                    //}

                    if (target.CurrentState == UnitState.Dying || target.CurrentState == UnitState.Dead)
                        continue;

                    Vector3 distance = (target.transform.position - criterionUnit.transform.position);

                    //radius == 15 //공격 유효 반경
                    //angle  = 225 //공격 각도
                    //Debug.LogWarning(distance.magnitude + " : " + radius + " : " + angle + " : " + casterRadius + " : " + target.Radius + " : " + Vector3.Angle(distance.normalized, criterionUnit.transform.forward));

                    radius = 1.0f; //임시적으로 강제로 근처에 있을 때만 때리도록...

                    if (!MathHelper.IsInRange(distance, radius, criterionUnit.transform.forward, angle, casterRadius, target.Radius))
                        continue;

                    units.Add(target);
                }
            }//if (criterionUnit.m_isOtherPc == false)
        }
        */

        //< 제일 가까운 녀석을 찾는다.
        units.Sort(delegate(Unit tmp1, Unit tmp2) 
        {
            return (criterionUnit.transform.position - tmp1.transform.position).magnitude.CompareTo((criterionUnit.transform.position - tmp2.transform.position).magnitude); 
        });

        if (units.Count > 0)
            return units[0];

        return null;
    }

    public List<Unit> FindTargets(byte exceptTeamID, Unit criterionUnit, float radius = 1000, float angle = 360f, bool withStructure = false)
    { 
        List<Unit>  foundList = new List<Unit>();

        foreach (KeyValuePair<byte, List<Unit>> pair in allTeamDic)
        {
            if (exceptTeamID == pair.Key)
                continue;

            foundList.AddRange( FindTargetsByTeam( pair.Key, criterionUnit, radius, angle, withStructure ) );
        }

        return foundList;
    }
    
    public List<Unit> FindTargetsByTeam(byte teamID, Unit criterionUnit, float radius = 1000, float angle = 360f, bool withStructure = false)
    { 
        if (!allTeamDic.ContainsKey(teamID))
            return null;

        List<Unit>  foundList = new List<Unit>();
        List<Unit>  unitList = allTeamDic[teamID];
        float       casterRadius = criterionUnit.Radius;
        eAttackType attackType = criterionUnit.CharInfo.AttackType;

        for (int i = 0; i < unitList.Count; i++)
        {
            Unit target = unitList[i];

            if (!CanTarget(attackType, target))
                continue;

            if (!withStructure && target.UnitType == UnitType.Prop)
                continue;
            else if (withStructure && target.UnitType == UnitType.Prop)
            {
                //< 프롭은 거리를 체크해야함
                float distance2 = (target.transform.position - criterionUnit.transform.position).sqrMagnitude;
                if (distance2 > PropCheckDis)
                    continue;
            }

            Vector3 distance = (target.transform.position - criterionUnit.transform.position);

            if (!MathHelper.IsInRange(distance, radius, criterionUnit.transform.forward, angle, casterRadius, target.Radius))
                continue;

            foundList.Add( target );
        }

        return foundList;
    }

    //< 자신을 제외한 팀원을 찾아 리턴해준다
    public Unit FindTargetWithTeam(Unit _Caster, byte TeamID)
    {
        //< 자신이 아닌 대상을 찾아준다.
        Unit unit = null;
        for (int i = 0; i < allTeamDic[TeamID].Count; i++ )
        {
            unit = allTeamDic[TeamID][i];
            if (unit != null && unit.Usable && unit != _Caster)
                break;
        }

        return unit;
    }
    /// <summary>
    /// 어그로 기반해서 먼저 찾고, 못찾으면 가까운 유닛을 찾도록 한다.
    /// </summary>
    /// <param name="detector">타겟을 찾고자 하는 사람</param>
    /// <param name="radius">detector의 위치 기반으로 찾고자하는 반경 (최대 1000m)</param>
    /// <returns>구해진 타겟</returns>
    public Unit FindTargetWithAggro(Unit detector, float radius = 1000, Unit exceptUnit = null, bool prop = false)
    {
        //지금 난투전 모드에서는 주인공은 AttackState 로 들어가지도 읺을 것이고, MoveToSkillState 로 들어가지도 않을 것이고, UnitStateBase::SearchTarget(); 도 하지 않을 것이다.
        //즉  public Unit FindTargetWithAggro(Unit detector, float radius = 1000, Unit exceptUnit = null, bool prop = false) 함수는 사용하지 않는다.

        Unit newTarget = NearestTarget(detector.TeamID, detector.CharInfo.AttackType, detector.transform.position, radius, prop, exceptUnit);

        return newTarget;
    }

    #region :: 게임 플레이 정보 저장용 클래스 작성 필요 ::

    public Dictionary<byte, Dictionary<uint, Dictionary<int, int>>> KilledNpcDic = new Dictionary<byte, Dictionary<uint, Dictionary<int, int>>>();
    
    public void AddKilledNpc(byte killerTeamId, uint npcId, int level)
    {
        if (!KilledNpcDic.ContainsKey( killerTeamId ))
            KilledNpcDic.Add( killerTeamId, new Dictionary<uint, Dictionary<int, int>>() );

        Dictionary<uint, Dictionary<int, int>> npcData = KilledNpcDic[killerTeamId];
        if (!npcData.ContainsKey( npcId ))
            npcData.Add( npcId, new Dictionary<int, int>() );

        // 같은 레벨의 Npc가 잡혔다면, 카운팅해준다.
        Dictionary<int, int> killedLvDic = npcData[npcId];
        if (!killedLvDic.ContainsKey( level ))
            killedLvDic.Add( level, 0 );

        killedLvDic[level] += 1;
    }

    #endregion

    /*
    #region 테스트용 전광판
    public Dictionary<ulong, int> TempScore = new Dictionary<ulong, int>();

    public void PlusKill(ulong uid)
    {
        if(TempScore.ContainsKey(uid))
        {
            TempScore[uid]++;
        }
        else
        {
            TempScore.Add(uid, 1);
        }
    }

    void OnGUI()
    {
        if (G_GameInfo.GameMode != GAME_MODE.FREEFIGHT)
            return;

        var enumerator = RoomUserAllDic.GetEnumerator();
        int i = 0;
        while(enumerator.MoveNext())
        {
            Unit user = enumerator.Current.Value;

            int Score = 0;
            if ( TempScore.TryGetValue(enumerator.Current.Key, out i) )
            {
                Score = i;
            }

            GUI.Label(new Rect(50, 100 * i + 100, 1000, 50), user.name + ": " + Score);

            i++;
        }
    }

    #endregion
    */
}
