using UnityEngine;
using System.Collections;
using PathologicalGames;

/// <summary>
/// PoolManager와 FxMaker의 Prefab의 재사용을 위해서 Prefab에 존재해야하는 붙여야하는 Component
/// </summary>
public class FxMakerPoolItem : MonoBehaviour 
{
    /// <summary>
    /// 파티클이 재사용될 수 있는지 유무 (FxMaker의 Duplicate 같은 기능 안쓴다면 True로 체크!)
    /// </summary>
    public bool supportReplay = false;

    /// <summary>
    /// 현 객체를 관리할 SpawnPool 객체 (필수)
    /// </summary>
    public string ownerPoolName = "Effect";

    /// <summary>
    /// 0이 아니라면, 해당 시간뒤에 자동으로 Despawn 수행.
    /// </summary>
    public float destroyTime = 0;
    float _destroyTime = 0;
    public  Unit Owner;
    private Transform cachedTransform = null;

    public void SetAttach(Transform trans)
    {
        cachedTransform = trans;
    }

    bool first = true;  //< 처음 생성되었을때에는 처리를 안하기위해 체크
    void Awake()
    {
        _destroyTime = destroyTime;
        supportReplay = true;

        if (PoolManager.Pools == null || !PoolManager.Pools.ContainsKey(ownerPoolName))
        {
            supportReplay = false;
            return;
        }

        // NsEffectManager.SetReplayEffect에서 SetActiveRecursively만 제외하고 사용하기.
        NsEffectManager.PreloadResource( gameObject );

        NcEffectBehaviour[] ncComs = gameObject.GetComponentsInChildren<NcEffectBehaviour>(true);
        for (int i = 0; i < ncComs.Length; i++)
        {
            ncComs[i].OnSetReplayState();

            //< 해당 스크립트는 재활용 안한다...
            if (ncComs[i] is NcCurveAnimation)
                supportReplay = false;
        }

        //< 피격 이펙트는 재활용하도록 처리해준다
        if (this.gameObject.name.Contains("Fx_beshot"))
            supportReplay = true;

        //공격들도 재활용 시도 
        if (this.gameObject.name.Contains("skill") || this.gameObject.name.Contains("attack"))
            supportReplay = true;

        //예외처리
        if (this.gameObject.name.Contains("Fx_par_jaejee_skill_04"))
            supportReplay = false;

    }

    public void SetEffectSpeed(float speed)
    {
        NcEffectBehaviour[] ncComs = gameObject.GetComponentsInChildren<NcEffectBehaviour>(true);
        foreach (NcEffectBehaviour ncCom in ncComs)
            ncCom.OnUpdateEffectSpeed(speed, true);
    }

    void OnEnable()
    {
        if (first)
            return;

        if (supportReplay)
            NsEffectManager.RunReplayEffect(gameObject, true);
    }

    void Update()
    {
        if(cachedTransform != null)
        {
            gameObject.transform.position = cachedTransform.transform.position;
            gameObject.transform.rotation = cachedTransform.transform.rotation;
        }

        if (_destroyTime > 0)
        {
            _destroyTime -= Time.deltaTime;
            if (_destroyTime <= 0)
                AutoDespawn();
        }
    }

    void AutoDespawn()
    {
        Owner = null;
        first = false;
        cachedTransform = null;

        if (supportReplay)
        {
            if (PoolManager.Pools[ownerPoolName].IsSpawned(gameObject.transform))
            {
                _destroyTime = destroyTime;
                PoolManager.Pools[ownerPoolName].Despawn(gameObject.transform, PoolManager.Pools[ownerPoolName].group);
            }
            else
            {
                if (GameDefine.TestMode)
                    Debug.Log("풀에 없어서 강제 파괴함 " + this.name);
                
                Destroy(gameObject);
            }
        }
        else
            Destroy( gameObject );
    }

    /// <summary>
    /// 수동으로 Despawn을 작동시킨다. 현재 Unit의 SpawnEffect함수에서 사용되기 위해 만듬.
    /// </summary>
    public void ManualDespawn()
    {
        if (!PoolManager.Pools[ownerPoolName].IsSpawned(transform))
        {
            Destroy(gameObject);
            return;
        }

        AutoDespawn();
    }
}
