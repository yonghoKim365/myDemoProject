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
    GameObject ModelObj;    //< ������ ������Ʈ

    Vector3 dir;            //< �̵��� ����
    bool exploded = false;  //< �ߺ� ���� ����
    float moveDistance = 0; //< ��ŭ �̵��ߴ��� �����
    int damage;             //< ���� �����
    byte penetrateCount;    //< �����ϰ�� �ִ� �浹ī��Ʈ

    float DestroyTime = 0;  //Type�� 1�ϰ�� �ڸ��ϴ� �ð�
    float NextDamageTime = 0; //Type 1�ϰ�� ������ ƽ

    ulong ProjectTileId = 0;

    Unit Owner, TargetUnit;
    List<Unit> TargetLists = new List<Unit>(); //< �浹�� ��� ����Ʈ�� �����صα�����(�����Ͻ� �������� �� ���� ���ϱ�����)
    List<Unit> TakeDamLists = new List<Unit>();

    //��Ʈ��ũ�� ���������� �ֵ�� ������ �ֵ�
    List<Unit> netDamageList = new List<Unit>();

    //< �浹 Ÿ��
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

        //< ���̾� ����
        if (_owner != null)
            this.gameObject.layer = _owner.gameObject.layer;

        //< ���� ����
        projectileInfo = _projecttile;
        ProjectTileId = _projectTileId;
        damage = _Damage;
        dir = _dir;
        Owner = _owner;
        TargetUnit = _target;
        normal = _normal;
        penetrateCount = projectileInfo.penetrateCount;
        abilityData = _ability;

        ////< �ùķ��̼� �����϶��� ���� �����
        //if (SimulationGameInfo.SimulationGameCheck)
        //{
        //    if (projectileInfo.penetrate == 1 && penetrateCount == 1)
        //        UIMgr.instance.OpenToastPanel("[Error] penetrate == 1 && penetrateCount == 1");
        //}

        //< Ÿ�� ����
        ProjectileType = TargetUnit != null ? eProjectileType.OneTarget : eProjectileType.NonTarget;

        //< Ÿ���� ������ ��������ġ�� ��´�
        if (TargetUnit != null)
        {
            BoxCollider collide = TargetUnit.GetComponent<BoxCollider>();
            float height = (collide != null ? (collide.bounds.max.y / 2) : 1);

            if (height < 0)
                height *= -1;

            //OffsetPos = height;
        }

        //< �� ����Ʈ�� ���´�.
        SetTargetList();

        //< �� ����
        CreateModel();

        //��� ���󰡴� ��ų�� ��츸
        if(projectileInfo.Type == 0)
        {
            //< �̵��ӵ��� 0�̶��, ��뿡�� �ٷ� �̵������ش�
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

        //< ����� ����Ʈ�� ��� �߰�(��Ÿ���̰ų�, �����Ͻ�)
        if (ProjectileType == eProjectileType.NonTarget || projectileInfo.penetrate == 1)
            TargetLists = G_GameInfo.CharacterMgr.liveList_All((byte)(Owner.TeamID == 0 ? 1 : 0));
        else
            TargetLists.Add(TargetUnit);
    }

    //< ������ �� ����
    void CreateModel()
    {
        //�߻�ü Ǯ���� �𵨸� �δ� �ϵ��� ���� �ؾ���
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

                //< ���� ����
                if (projectileInfo.ProjectSound != 0)
                {
                    //SoundHelper.PlaySfxSound(projectileInfo.ProjectSound, 1);
                    SoundManager.instance.PlaySfxSound(projectileInfo.ProjectSound);
                }
            });
        }
    }

    //< ������Ʈ
    void FixedUpdate()
    {
        if (!bNetInit)
            return;

        if (projectileInfo.Type == 0)
        {
            //< �̵�
            MoveUpdate();
        }
        else if (projectileInfo.Type == 1)
        {
            //���ڸ��� �ӹ��� Ÿ��
            if (ProjectileType == eProjectileType.NonTarget)
                floatUpdate();

            if (DestroyTime <= Time.time)
                ModelDestroy();
        }
    }

    //< ��Ÿ�� ������Ʈ
    void floatUpdate()
    {
        bool DestroyCheck = false;
        NextDamageTime -= Time.deltaTime;
        if (NextDamageTime <= 0)
        {

            NextDamageTime = projectileInfo.damageinterval;

            SetTargetList();

            //< �浹�� üũ�Ѵ�.
            //for (int i = 0; i < TargetLists.Count; i++)
            {
                //< �浹 üũ
                //if (TargetLists[i] == null || TargetLists[i].IsDie)
                //    return;

                //ȭ���� Ÿ�ٿ��� ������ ���
                //Vector3 a_ArrowPos = ModelObj.transform.position;
                //Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< ���̷� ���� ������ ��������ֱ⿡ y�� 0���� ����
                //a_ArrowPos.y = a_TargetPos.y = 0;

                //< �浹�� �߻������� ó��
                //if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< �浹 ����� �ѹ��� üũ�Ѵ�.
                    //if (TakeDamLists.Contains(TargetLists[i]))
                    //    continue;

                    //TakeDamLists.Add(TargetLists[i]);

                    //< �� ������ ��� ��� ������� �ش�.
                    AllTakeDamage(false);

                    //< �����ϰ�쿡�� ����ī��Ʈ�� ����ش�
                    //if (projectileInfo.penetrate == 1)
                    //{
                    //    penetrateCount--;

                    //    //< ����ī��Ʈ�� 0�̶�� �׳� ��ǥ�� �浹�� ���߾ �ı�
                    //    if (penetrateCount <= 0)
                    //        DestroyCheck = true;
                    //}
                    ////< ������ �ƴҰ�쿡�� �׳� �ı�ó��
                    //else
                    //    DestroyCheck = true;
                }
            }
        }

        //< �ı�üũ�� �Ǿ����� �ı�
        if (DestroyCheck)
            ModelDestroy();
    }

    // �̵�
    float TargetCheckDelay = 0.05f;
    void MoveUpdate()
    {
        if (ModelObj == null || !ModelObj.activeSelf)
            return;

        //< ���¿� ���� ������Ʈ�� �����Ѵ�(�������� ���Ͽ� ������Ʈ�� �и��������)
        if (ProjectileType == eProjectileType.OneTarget)
            OneTargetUpdate();
        else if (ProjectileType == eProjectileType.NonTarget)
            NonTargetUpdate();

        //< �̵�
        float speed = projectileInfo.moveSpeed * Time.deltaTime;
        moveDistance += speed;
        CachedTrans.position += (dir * speed);

        //�ƽ��� ���� �ڸ�!!
        if (moveDistance >= projectileInfo.maxDistance)
            ModelDestroy();

        //< ���� Y�� ����ó��
        if (projectileInfo.yLocation == 1)
        {
            if (ModelObj != null)
                ModelObj.transform.localPosition = new Vector3(0, -(this.transform.localPosition.y), 0);
        }
    }

    //< ��Ÿ�� ������Ʈ
    void OneTargetUpdate()
    {
        bool DestroyCheck = false;
        TargetCheckDelay -= Time.deltaTime;
        if (TargetCheckDelay <= 0)
        {
            SetTargetList();

            TargetCheckDelay = 0.05f;

            //< �ش� Ÿ�� �������� ����ش�.
            if (TargetUnit != null)
                dir = ((TargetUnit.transform.position + (Vector3.up * OffsetPos)) - CachedTrans.position).normalized;

            //< �̵��� ������ �ٶ󺻴�.
            if (TargetUnit != null && exploded == false)
                CachedTrans.transform.LookAt(TargetUnit.transform.position + (Vector3.up * OffsetPos));

            //< �浹�� üũ�Ѵ�.
            for (int i = 0; i < TargetLists.Count; i++)
            {
                //< �浹 üũ
                if (TargetLists[i] == null)
                    continue;

                //ȭ���� Ÿ�ٿ��� ������ ���
                Vector3 a_ArrowPos = ModelObj.transform.position;
                Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< ���̷� ���� ������ ��������ֱ⿡ y�� 0���� ����
                a_ArrowPos.y = a_TargetPos.y = 0;

                //< �浹�� �߻������� ó��
                if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< �浹 ����� �ѹ��� üũ�Ѵ�.
                    if (TakeDamLists.Contains(TargetLists[i]))
                        continue;

                    TakeDamLists.Add(TargetLists[i]);

                    //< �浹 ����� ���� ��ǥ�� �������
                    if (TargetLists[i] == TargetUnit)
                    {
                        //< ���� �ı���Ų��.
                        DestroyCheck = true;
                    }

                    if (TargetLists[i].IsDie)
                        continue;

                    //< �� ������ ��� ��� ������� �ش�.
                    AllTakeDamage();

                    //< �����ϰ�쿡�� ����ī��Ʈ�� ����ش�
                    if (projectileInfo.penetrate == 1)
                    {
                        penetrateCount--;

                        //< ����ī��Ʈ�� 0�̶�� �׳� ��ǥ�� �浹�� ���߾ �ı�
                        if (penetrateCount <= 0)
                            DestroyCheck = true;
                    }
                }
            }
        }

        //< Ÿ���� �������ų�, �ı�üũ�� �Ǿ����� �ı�
        if (TargetUnit == null || !TargetUnit.Usable || DestroyCheck)
            ModelDestroy();
    }

    //< ��Ÿ�� ������Ʈ
    void NonTargetUpdate()
    {
        bool DestroyCheck = false;
        TargetCheckDelay -= Time.deltaTime;
        if (TargetCheckDelay <= 0)
        {
            SetTargetList();

            TargetCheckDelay = 0.05f;

            //< �浹�� üũ�Ѵ�.
            for (int i = 0; i < TargetLists.Count; i++)
            {
                //< �浹 üũ
                if (TargetLists[i] == null || TargetLists[i].IsDie)
                    return;

                //ȭ���� Ÿ�ٿ��� ������ ���
                Vector3 a_ArrowPos = ModelObj.transform.position;
                Vector3 a_TargetPos = TargetLists[i].transform.position;

                //< ���̷� ���� ������ ��������ֱ⿡ y�� 0���� ����
                a_ArrowPos.y = a_TargetPos.y = 0;

                //< �浹�� �߻������� ó��
                if ((a_TargetPos - a_ArrowPos).magnitude < 1.5f)
                {
                    //< �浹 ����� �ѹ��� üũ�Ѵ�.
                    if (TakeDamLists.Contains(TargetLists[i]))
                        continue;

                    TakeDamLists.Add(TargetLists[i]);

                    //< �� ������ ��� ��� ������� �ش�.
                    AllTakeDamage();

                    //< �����ϰ�쿡�� ����ī��Ʈ�� ����ش�
                    if (projectileInfo.penetrate == 1)
                    {
                        penetrateCount--;

                        //< ����ī��Ʈ�� 0�̶�� �׳� ��ǥ�� �浹�� ���߾ �ı�
                        if (penetrateCount <= 0)
                            DestroyCheck = true;
                    }
                    //< ������ �ƴҰ�쿡�� �׳� �ı�ó��
                    else
                        DestroyCheck = true;
                }
            }
        }

        //< �ı�üũ�� �Ǿ����� �ı�
        if (DestroyCheck)
            ModelDestroy();
    }

    //< �����ȿ� ������� �����Ѵ�.
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

            //< �����ȿ� ������� �����!
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
            //��� ���⿡�� ����
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

            //�����ϰ��� ������ üũ�� ����
            //if ((SkillType)abilityData.skillType == SkillType.Attack)
            {
                //�������� ���
                NetworkClient.instance.SendPMsgRoleAttackC((int)_LowDataMgr.GetSkillAction(abilityData.Idx).idx, 0, (int)abilityData.notiIdx, 1, ref targetdata, (long)ProjectTileId);
            }

            targetdata.Clear();

            //�ϴ� �������� ������ ����
            netDamageList.Clear();
        }
    }

    //< �� ����
    void ModelDestroy()
    {
        if (ModelObj == null)
        {
            MyDestroy();
            return;
        }

        if (!bNetInit)
            return;

        //< ������Ʈ�� ��������
        bNetInit = false;

        //< ����Ʈ�� ��µǰ��������� �ֱ⶧���� ����Ŀ� ����������
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

            //< ���� ����
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

    //< �ܺο��� �ı���Ű�� ����(�ȿ� �������� Ǯ�� ���Ͻ��Ѿ��ϱ⶧��)
    public void Clear()
    {
        exploded = true;
        ModelDestroy();
    }

    // �浹������ ȣ��Ǵ� �κ�
    void Explode(Unit target)
    {
        if (!bNetInit)
            return;

        //< �浹������ ����Ʈ ���
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

        //< �浹������ ���� ���
        if (projectileInfo.colideSound != 0 && target != null)
        {
            //SoundHelper.PlaySfxSound(projectileInfo.colideSound, 1);
            SoundManager.instance.PlaySfxSound(projectileInfo.colideSound);
        }
            
    }

    //< �������� ����� ó��
    void ExplodeUpdate(Unit target)
    {
        if (!bNetInit)
            return;

        //< ������� �ش�
        if(SceneManager.instance.IsRTNetwork)
        {
            //��Ʈ��ũ�� ����Ʈ�� ��Ƽ� ������ ����
            netDamageList.Add(target);
        }
        else
        {
            //��Ʈ��ũ�� �ƴϸ� �ٷ� ������
            target.TakeDamage(Owner, 1, damage, 0, eAttackType.All, !normal, abilityData, false, true);
        }        

        //< ������ ������� ������ ȣ���Ѵ�
        //if(projectileInfo != null && projectileInfo.callBuffIdx != 0 && Owner != null && !Owner.IsDie)
        //{
        //    if (target.BuffCtlr != null)
        //        //Ȳ��ȫ ������Ʈ - �̰� �´��� �𸣰ڴ� ������Ÿ�Ͽ� ����/����� �޸��� üũ
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