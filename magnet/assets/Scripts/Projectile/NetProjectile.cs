using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public enum eProjectileType
{
    OneTarget,
    NonTarget
}
public class NetProjectile : MonoBehaviour
{
    public Transform CachedTrans;

    bool bNetInit = false;
    GameObject ModelObj;    //< 생성한 오브젝트

    Vector3 dir;            //< 이동할 방향
    bool exploded = false;  //< 중복 폭발 방지
    float moveDistance = 0; //< 얼만큼 이동했는지 저장용
    int damage;             //< 적용 대미지
    byte penetrateCount;    //< 관통일경우 최대 충돌카운트

    float DestroyTime = 0;  //Type이 1일경우 자멸하는 시간
    float NextDamageTime = 0; //Type 1일경우 데미지 틱

    ulong ProjectTileId = 0;

    Unit Owner, TargetUnit;
    List<Unit> TargetLists = new List<Unit>(); //< 충돌된 대상 리스트를 저장해두기위함(관통일시 같은적을 또 공격 안하기위함)
    List<Unit> TakeDamLists = new List<Unit>();

    //네트워크상 데미지받을 애들로 선정된 애들
    List<Unit> netDamageList = new List<Unit>();

    //< 충돌 타입
    public eProjectileType ProjectileType = eProjectileType.OneTarget;
    SkillTables.ProjectTileInfo projectileInfo;
    AbilityData abilityData;
    public float OffsetPos = 0.8f;

    void Reset()
    {
        bNetInit = false;
        exploded = false;
        Owner = null;
        TargetUnit = null;
        moveDistance = 0;

        TakeDamLists.Clear();
        TargetLists.Clear();
    }

    public bool normal;
    public void Setup(SkillTables.ProjectTileInfo _projecttile, AbilityData _ability, int _Damage, int _teamIndex, Vector3 _dir, Unit _owner, Unit _target, ulong _projectTileId, bool _normal = false)
    {
        Reset();

        //< 레이어 설정
        if (_owner != null)
            this.gameObject.layer = _owner.gameObject.layer;

        //< 정보 대입
        projectileInfo = _projecttile;
        ProjectTileId = _projectTileId;
        damage = _Damage;
        dir = _dir;
        Owner = _owner;
        TargetUnit = _target;
        normal = _normal;
        penetrateCount = projectileInfo.penetrateCount;
        abilityData = _ability;

        ////< 시뮬레이션 상태일때는 에러 뱉어줌
        //if (SimulationGameInfo.SimulationGameCheck)
        //{
        //    if (projectileInfo.penetrate == 1 && penetrateCount == 1)
        //        UIMgr.instance.OpenToastPanel("[Error] penetrate == 1 && penetrateCount == 1");
        //}

        //< 타입 설정
        ProjectileType = TargetUnit != null ? eProjectileType.OneTarget : eProjectileType.NonTarget;

        //< 타겟이 있을시 오프셋위치를 잡는다
        if (TargetUnit != null)
        {
            BoxCollider collide = TargetUnit.GetComponent<BoxCollider>();
            float height = (collide != null ? (collide.bounds.max.y / 2) : 1);

            if (height < 0)
                height *= -1;

            //OffsetPos = height;
        }

        //< 적 리스트를 얻어온다.
        SetTargetList();

        //< 모델 생성
        CreateModel();

        //즉시 날라가는 스킬의 경우만
        if(projectileInfo.Type == 0)
        {
            //< 이동속도가 0이라면, 상대에게 바로 이동시켜준다
            if (ProjectileType == eProjectileType.OneTarget && projectileInfo.moveSpeed == 0 && _target != null)
                this.transform.position = _target.transform.position;
        }
        else if (_projecttile.Type == 1)
        {
            DestroyTime = Time.time + projectileInfo.durationTime;
            NextDamageTime = projectileInfo.damageinterval;
        }
    }

    void SetTargetList()
    {
        TargetLists.Clear();

        //< 상대팀 리스트를 모두 추가(논타겟이거나, 관통일시)
        if (ProjectileType == eProjectileType.NonTarget || projectileInfo.penetrate == 1)
            TargetLists = G_GameInfo.CharacterMgr.liveList_All((byte)(Owner.TeamID == 0 ? 1 : 0));
        else
            TargetLists.Add(TargetUnit);
    }

    //< 프리팹 모델 생성
    void CreateModel()
    {
        //발사체 풀에서 모델링 로더 하도록 수정 해야함
        if (ModelObj == null)
        {
            G_GameInfo.SpawnProjectTile(projectileInfo.prefab, transform, (tileobj) =>
            {
                bNetInit = true;

                ModelObj = tileobj;
                if (ModelObj != null)
                {
                    ModelObj.gameObject.SetActive(true);
                }

                //< 사운드 생성
                if (projectileInfo.ProjectSound != 0)
                {
                    //SoundHelper.PlaySfxSound(projectileInfo.ProjectSound, 1);
                    SoundManager.instance.PlaySfxSound(projectileInfo.ProjectSound);
                }
            });
        }
    }

    //< 업데이트
    void FixedUpdate()
    {
        if (!bNetInit)
            return;

        if (projectileInfo.Type == 0)
        {
            //< 이동
            MoveUpdate();
        }
        else if (projectileInfo.Type == 1)
        {
            //제자리에 머무는 타입
            if (ProjectileType == eProjectileType.NonTarget)
                floatUpdate();

            if (DestroyTime <= Time.time)
                ModelDestroy();
        }
    }

    //< 논타겟 업데이트
    void floatUpdate()
    {
        bool DestroyCheck = false;
        NextDamageTime -= Time.deltaTime;
        if (NextDamageTime <= 0)
        {

            NextDamageTime = projectileInfo.damageinterval;

            SetTargetList();

            //< 충돌을 체크한다.
            //for (int i = 0; i < TargetLists.Count; i++)
            {
                //< 충돌 체크
                //if (TargetLists[i] == null || TargetLists[i].IsDie)
                //    return;

                //화살이 타겟에게 도착한 경우
                //Vector3 a_ArrowPos = ModelObj.transform.position;
                //Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< 높이로 인한 문제가 생길수도있기에 y는 0으로 보정
                //a_ArrowPos.y = a_TargetPos.y = 0;

                //< 충돌이 발생했을시 처리
                //if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< 충돌 대상은 한번만 체크한다.
                    //if (TakeDamLists.Contains(TargetLists[i]))
                    //    continue;

                    //TakeDamLists.Add(TargetLists[i]);

                    //< 내 주위에 모든 대상에 대미지를 준다.
                    AllTakeDamage(false);

                    //< 관통일경우에는 관통카운트를 깍아준다
                    //if (projectileInfo.penetrate == 1)
                    //{
                    //    penetrateCount--;

                    //    //< 관통카운트가 0이라면 그냥 목표에 충돌을 못했어도 파괴
                    //    if (penetrateCount <= 0)
                    //        DestroyCheck = true;
                    //}
                    ////< 관통이 아닐경우에는 그냥 파괴처리
                    //else
                    //    DestroyCheck = true;
                }
            }
        }

        //< 파괴체크가 되었을때 파괴
        if (DestroyCheck)
            ModelDestroy();
    }

    // 이동
    float TargetCheckDelay = 0.05f;
    void MoveUpdate()
    {
        if (ModelObj == null || !ModelObj.activeSelf)
            return;

        //< 상태에 따라 업데이트를 따로한다(관통으로 인하여 업데이트를 분리해줘야함)
        if (ProjectileType == eProjectileType.OneTarget)
            OneTargetUpdate();
        else if (ProjectileType == eProjectileType.NonTarget)
            NonTargetUpdate();

        //< 이동
        float speed = projectileInfo.moveSpeed * Time.deltaTime;
        moveDistance += speed;
        CachedTrans.position += (dir * speed);

        //맥스에 가면 자멸!!
        if (moveDistance >= projectileInfo.maxDistance)
            ModelDestroy();

        //< 모델의 Y값 보정처리
        if (projectileInfo.yLocation == 1)
        {
            if (ModelObj != null)
                ModelObj.transform.localPosition = new Vector3(0, -(this.transform.localPosition.y), 0);
        }
    }

    //< 원타겟 업데이트
    void OneTargetUpdate()
    {
        bool DestroyCheck = false;
        TargetCheckDelay -= Time.deltaTime;
        if (TargetCheckDelay <= 0)
        {
            SetTargetList();

            TargetCheckDelay = 0.05f;

            //< 해당 타겟 방향으로 잡아준다.
            if (TargetUnit != null)
                dir = ((TargetUnit.transform.position + (Vector3.up * OffsetPos)) - CachedTrans.position).normalized;

            //< 이동할 방향을 바라본다.
            if (TargetUnit != null && exploded == false)
                CachedTrans.transform.LookAt(TargetUnit.transform.position + (Vector3.up * OffsetPos));

            //< 충돌을 체크한다.
            for (int i = 0; i < TargetLists.Count; i++)
            {
                //< 충돌 체크
                if (TargetLists[i] == null)
                    continue;

                //화살이 타겟에게 도착한 경우
                Vector3 a_ArrowPos = ModelObj.transform.position;
                Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< 높이로 인한 문제가 생길수도있기에 y는 0으로 보정
                a_ArrowPos.y = a_TargetPos.y = 0;

                //< 충돌이 발생했을시 처리
                if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< 충돌 대상은 한번만 체크한다.
                    if (TakeDamLists.Contains(TargetLists[i]))
                        continue;

                    TakeDamLists.Add(TargetLists[i]);

                    //< 충돌 대상이 내가 목표한 대상인지
                    if (TargetLists[i] == TargetUnit)
                    {
                        //< 나를 파괴시킨다.
                        DestroyCheck = true;
                    }

                    if (TargetLists[i].IsDie)
                        continue;

                    //< 내 주위에 모든 대상에 대미지를 준다.
                    AllTakeDamage();

                    //< 관통일경우에는 관통카운트를 깍아준다
                    if (projectileInfo.penetrate == 1)
                    {
                        penetrateCount--;

                        //< 관통카운트가 0이라면 그냥 목표에 충돌을 못했어도 파괴
                        if (penetrateCount <= 0)
                            DestroyCheck = true;
                    }
                }
            }
        }

        //< 타겟이 없어졌거나, 파괴체크가 되었을때 파괴
        if (TargetUnit == null || !TargetUnit.Usable || DestroyCheck)
            ModelDestroy();
    }

    //< 논타겟 업데이트
    void NonTargetUpdate()
    {
        bool DestroyCheck = false;
        TargetCheckDelay -= Time.deltaTime;
        if (TargetCheckDelay <= 0)
        {
            SetTargetList();

            TargetCheckDelay = 0.05f;

            //< 충돌을 체크한다.
            for (int i = 0; i < TargetLists.Count; i++)
            {
                //< 충돌 체크
                if (TargetLists[i] == null || TargetLists[i].IsDie)
                    return;

                //화살이 타겟에게 도착한 경우
                Vector3 a_ArrowPos = ModelObj.transform.position;
                Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< 높이로 인한 문제가 생길수도있기에 y는 0으로 보정
                a_ArrowPos.y = a_TargetPos.y = 0;

                //< 충돌이 발생했을시 처리
                if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< 충돌 대상은 한번만 체크한다.
                    if (TakeDamLists.Contains(TargetLists[i]))
                        continue;

                    TakeDamLists.Add(TargetLists[i]);

                    //< 내 주위에 모든 대상에 대미지를 준다.
                    AllTakeDamage();

                    //< 관통일경우에는 관통카운트를 깍아준다
                    if (projectileInfo.penetrate == 1)
                    {
                        penetrateCount--;

                        //< 관통카운트가 0이라면 그냥 목표에 충돌을 못했어도 파괴
                        if (penetrateCount <= 0)
                            DestroyCheck = true;
                    }
                    //< 관통이 아닐경우에는 그냥 파괴처리
                    else
                        DestroyCheck = true;
                }
            }
        }

        //< 파괴체크가 되었을때 파괴
        if (DestroyCheck)
            ModelDestroy();
    }

    //< 범위안에 모든적을 공격한다.
    void AllTakeDamage(bool targetCountCheck = true)
    {
        Vector3 a_TargetPos;
        Vector3 a_ArrowPos = ModelObj.transform.position;

        byte targetCount = (byte)(projectileInfo.penetrateCount == 0 ? (byte)50 : projectileInfo.penetrateCount);
        float radius = projectileInfo.radius;
        List<Unit> targets = G_GameInfo.CharacterMgr.liveList_All((byte)(Owner.TeamID == 0 ? 1 : 0));
        for (int j = 0; j < targets.Count; j++)
        {
            if (targets[j] == null || targets[j].IsDie)
                continue;

            //< 범위안에 있을경우 대미지!
            a_TargetPos = targets[j].transform.position;
            a_ArrowPos.y = a_TargetPos.y = 0;

            if ((a_TargetPos - a_ArrowPos).magnitude < radius)
            {
                Explode(targets[j]);

                if(targetCountCheck)
                {
                    targetCount--;
                    if (targetCount == 0)
                        break;
                }
            }
        }
        if (SceneManager.instance.IsRTNetwork && (Owner.m_rUUID == NetData.instance._userInfo._charUUID))
        {
            //묶어서 여기에서 보냄
            List<Sw.TargetInfo> targetdata = new List<Sw.TargetInfo>();
            targetdata.Clear();

            for (int i = 0; i < netDamageList.Count; i++)
            {
                var Target = new Sw.TargetInfo();

                if (netDamageList[i].UnitType == UnitType.Npc || netDamageList[i].UnitType == UnitType.Boss)
                    Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_NPC;
                else if (netDamageList[i].UnitType == UnitType.Unit && !netDamageList[i].IsPartner)
                    Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_USER;
                else if (netDamageList[i].UnitType == UnitType.Unit && netDamageList[i].IsPartner)
                    Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_HERO;

                Target.UllTargetId = (long)netDamageList[i].m_rUUID;

                targetdata.Add(Target);

            }

            //어택일경우는 데미지 체크를 보냄
            //if ((SkillType)abilityData.skillType == SkillType.Attack)
            {
                //데미지일 경우
                NetworkClient.instance.SendPMsgRoleAttackC((int)_LowDataMgr.GetSkillAction(abilityData.Idx).idx, 0, (int)abilityData.notiIdx, 1, ref targetdata, (long)ProjectTileId);
            }

            targetdata.Clear();

            //일단 데미지를 줬으니 리셋
            netDamageList.Clear();
        }
    }

    //< 모델 삭제
    void ModelDestroy()
    {
        if (ModelObj == null)
        {
            MyDestroy();
            return;
        }

        if (!bNetInit)
            return;

        //< 업데이트를 막기위함
        bNetInit = false;

        //< 이펙트가 출력되고있을수도 있기때문에 잠시후에 삭제시켜줌
        ModelObj.gameObject.SetActive(false);
        TempCoroutine.instance.FrameDelay(2, () =>
        {
            if (ModelObj != null)
            {
                FxMakerPoolItem poolItem = ModelObj.transform.GetComponent<FxMakerPoolItem>();
                if (null == poolItem)
                    SelfDespawn.DespawnPartileSystem("Projectile", ModelObj.transform, 0.1f, true);
                else if (null != poolItem)
                    poolItem.ManualDespawn();

                ModelObj = null;
            }

            //< 나도 삭제
            MyDestroy();
        });
    }

    void MyDestroy()
    {
        if (this == null)
            return;

        if (PoolManager.Pools != null && PoolManager.Pools["Orgprojectile"] != null && PoolManager.Pools["Orgprojectile"].IsSpawned(gameObject.transform))
            PoolManager.Pools["Orgprojectile"].Despawn(gameObject.transform, PoolManager.Pools["Orgprojectile"].group);
        else
        {
            if (GameDefine.TestMode)
                Debug.Log("Not Pool Item " + this.name);

            Destroy(gameObject);
        }
    }

    //< 외부에서 파괴시키기 위함(안에 프리팹은 풀에 리턴시켜야하기때문)
    public void Clear()
    {
        exploded = true;
        ModelDestroy();
    }

    // 충돌했을시 호출되는 부분
    void Explode(Unit target)
    {
        if (!bNetInit)
            return;

        //< 충돌했을시 이펙트 출력
        if (projectileInfo.colideEffect != "0" && target != null)
        {
            Transform effectobj = null;
            target.SpawnEffect(projectileInfo.colideEffect, 1, target.transform, null, false, (effect) =>
            {
                effectobj = effect;
                NGUITools.SetChildLayer(effectobj, Owner.gameObject.layer);
                ExplodeUpdate(target);
            });
        }
        else
            ExplodeUpdate(target);

        //< 충돌했을시 사운드 출력
        if (projectileInfo.colideSound != 0 && target != null)
        {
            //SoundHelper.PlaySfxSound(projectileInfo.colideSound, 1);
            SoundManager.instance.PlaySfxSound(projectileInfo.colideSound);
        }
            
    }

    //< 실질적인 대미지 처리
    void ExplodeUpdate(Unit target)
    {
        if (!bNetInit)
            return;

        //< 대미지를 준다
        if(SceneManager.instance.IsRTNetwork)
        {
            //네트워크면 리스트에 담아서 서버로 전달
            netDamageList.Add(target);
        }
        else
        {
            //네트워크가 아니면 바로 데미지
            target.TakeDamage(Owner, 1, damage, 0, eAttackType.All, !normal, abilityData, false, true);
        }        

        //< 버프가 있을경우 버프를 호출한다
        //if(projectileInfo != null && projectileInfo.callBuffIdx != 0 && Owner != null && !Owner.IsDie)
        //{
        //    if (target.BuffCtlr != null)
        //        //황비홍 프로젝트 - 이거 맞는지 모르겠다 프로젝타일에 버프/디버프 달린거 체크
        //        target.BuffCtlr.AttachBuff(Owner, target, _LowDataMgr.GetBuffData(projectileInfo.callBuffIdx), null);
        //}
    }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        DisplayGizmos();
    }

    void DisplayGizmos()
    {
        if (projectileInfo == null)
            return;

        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, projectileInfo.radius);
        UnityEditor.Handles.Label(transform.position + (transform.right * projectileInfo.radius), "radius");
    }

#endif
}