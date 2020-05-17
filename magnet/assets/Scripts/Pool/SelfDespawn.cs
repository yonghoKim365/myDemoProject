using UnityEngine;
using System.Collections;
using PathologicalGames;

/// <summary>
/// PoolManager에 의해 자동으로 Despawn되게 해주는 Component
/// </summary>
public class SelfDespawn : MonoBehaviour
{
    /// <summary>
    /// 현 객체가 Despawn될 SpawnPool Name
    /// </summary>
    public string poolName;

    public bool ActivePool = true;

    /// <summary>
    /// 자동으로 Despawn될 시간
    /// </summary>
    public float duration = 1f;

    void OnEnable()
    {
        StartCoroutine( DespawnAfterDelay() );
    }

    IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        ClearPartileSystem( transform );

        // 현재 Spawn된 객체라면 Despwan시켜준다.
        if (ActivePool)
        {
            if (PoolManager.Pools.ContainsKey(poolName) || PoolManager.Pools[poolName].IsSpawned(transform))
                PoolManager.Pools[poolName].Despawn(transform);
        }
        
    }

    #region :: Static Functions ::

    /// <summary>
    /// 파티클이 들어있는 객체를 재사용하기 위해서는 Despawn시 Clear를 해주어야 재사용시 잔상이 남지 않음!
    /// </summary>
    /// <param name="target">Particle이 있는 객체</param>
    public static void ClearPartileSystem(Transform target)
    {
        // 파티클 시스템이 있다면 강제 클리어필요
        foreach (ParticleSystem ps in target.GetComponentsInChildren<ParticleSystem>())
            ps.Clear();
    }

    /// <summary>
    /// 파티클시스템이 있다면, LifeTime만큼 플레이하고 Despawn, 아니면 바로 Despawn
    /// </summary>
    public static float DespawnPartileSystem(string poolName, Transform target, float duration = 0, bool manualDuration = false)
    {
        float lifeTime = duration;
        
        // 파괴시간을 강제로 설정안했다면.
        if (!manualDuration)
        {
            foreach (ParticleSystem ps in target.GetComponentsInChildren<ParticleSystem>())
            {
                float particleLife = ps.duration + ps.startDelay + ps.startLifetime;
                if (lifeTime < particleLife)
                    lifeTime = particleLife;
            }
        }

        new Task( DespawnDelayForGame( poolName, lifeTime, target ) );

        return lifeTime;
    }

    static IEnumerator DespawnDelayForGame(string poolName, float duration, Transform target)
    {
        yield return new WaitForSeconds(duration);

        if (null == target)
            yield break;

        SpawnPool pool = PoolManager.Pools[poolName];

        if (pool.IsSpawned(target))
            pool.Despawn(target, pool.group);
        else
            Destroy(target.gameObject);
    }

    #endregion
}
