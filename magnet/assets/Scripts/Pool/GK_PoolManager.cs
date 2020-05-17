using UnityEngine;
using System.Collections.Generic;
using PathologicalGames;

/// <summary>
/// PoolManager5를 관리해주는 클래스
/// </summary>
/// <remarks>FxMaker와 함께 사용시 필요한 기능들로 구성됨</remarks>
public class GK_PoolManager : Immortal<GK_PoolManager>
{
    #region :: Spawn / Despawn ::

    /// <summary>
    /// PoolManger에 존재하는 객체를 찾아서 Spawn해준다.
    /// </summary>
    /// <param name="poolName">원하는 객체가 들어있는 SpawnPool이름</param>
    /// <param name="prefabName">Spawn할 객체이름</param>
    /// <param name="spawnPos">Spawn할 객체의 위치값</param>
    /// <param name="spawnRot">Spawn할 객체의 회전값</param>
    /// <param name="parent">Spawn한 객체의 부모</param>
    /// <param name="scale"></param>
    /// <param name="speed"></param>
    /// <returns>없으면 null</returns>
    public Transform Spawn(string poolName, string prefabName, Vector3 spawnPos, Quaternion spawnRot, Transform parent, Vector3 scale, float speed = 1f)
    {
        SpawnPool spawnPool = null;
        if (!PoolManager.Pools.TryGetValue(poolName, out spawnPool))
            return null;

        if (!spawnPool.prefabs.ContainsKey( prefabName ))
            return null;

        Transform spawned = spawnPool.Spawn( prefabName, spawnPos, spawnRot, parent );
        
        spawned.localScale = scale;

        //// for FxMaker : 최대 이펙트 스피드는 0.5f배로. 4배이상은 안나오는게 많음.
        //NsEffectManager.AdjustSpeedRuntime( spawned.gameObject, Mathf.Clamp( speed, 0.5f, 4f ) );

        return spawned;
    }

    /// <summary>
    /// PoolManager에 존재하는 객체를 찾아서 Spawn해준다. 존재하지 않는다면, Pool에 등록후 Spawn해준다.
    /// </summary>
    public Transform Spawn(string poolName, Transform prefabTrans, Vector3 spawnPos, Quaternion spawnRot, Transform parent, Vector3 scale, float speed = 1f)
    {
        SpawnPool spawnPool = null;
        if (!PoolManager.Pools.TryGetValue(poolName, out spawnPool))
            return null;

        Transform spawned = spawnPool.Spawn( prefabTrans, spawnPos, spawnRot, parent );
        
        spawned.localScale = scale;

        //// for FxMaker : 최대 이펙트 스피드는 0.5f배로. 4배이상은 안나오는게 많음.
        NsEffectManager.AdjustSpeedRuntime( spawned.gameObject, Mathf.Clamp( speed, 0.5f, 4f ) );

        return spawned;
    }

    /// <summary>
    /// 모든 SpawnPool을 검사해서 생성된 Pool에서 삭제되도록 한다.
    /// </summary>
    public void Despawn(Transform target)
    { 
        foreach (KeyValuePair<string, SpawnPool> pair in PoolManager.Pools)
        {
            if (pair.Value.IsSpawned( target ))
                Despawn( pair.Value, target );
        }
    }

    /// <summary>
    /// 지정된 SpawnPool에서 삭제되도록한다.
    /// </summary>
    public void Despawn(string poolName, Transform target)
    { 
        SpawnPool spawnPool = null;
        if (!PoolManager.Pools.TryGetValue(poolName, out spawnPool))
            return;

        Despawn( spawnPool, target );
    }

    /// <summary>
    /// 지정된 SpawnPool에서 삭제되도록한다.
    /// </summary>
    public void Despawn(SpawnPool spawnPool, Transform target)
    {
        if (null == spawnPool || null == target)
            return;

        if (spawnPool.IsSpawned(target))
            spawnPool.Despawn( target, spawnPool.transform );
    }

    #endregion

    public void AddPrefabPool()
    {
        //
    }

    /// <summary>
    /// 새로운 SpawnPool을 생성한다.
    /// </summary>
    /// <param name="poolName">주어진 문자열에서 "Pool"은 제외한 이름이 SpawnPool 이름으로 사용됨.</param>
    public SpawnPool CreatePool(string poolName)
    {
        SpawnPool pool = null;
        if (PoolManager.Pools.TryGetValue( poolName, out pool ))
            return pool;

        pool = PoolManager.Pools.Create( poolName );
        pool.transform.SetParent( transform );

        return pool;
    }

    public bool DestroyPool(string poolName)
    {
        return PoolManager.Pools.Destroy( poolName );
    }

#if UNITY_EDITOR
    public bool showDebugView = false;
    Vector2 scrollPos;
    Vector2 scrollPos2;
    List<Transform> testSpawnedList = new List<Transform>();
    void OnGUI()
    {
        if (!showDebugView)
            return;

        if (GUILayout.Button("Create \"TestPool\""))
        {
            CreatePool( "Test" );
        }
        if (GUILayout.Button("Destroy \"TestPool\""))
        {
            DestroyPool( "Test" );
        }
        if (GUILayout.Button("[Test Spawn] unregistered Object"))
        {
            GameObject obj = ResourceMgr.Load( "Effect/Dummy_Test/skelgi_Dummy_01") as GameObject;
            Transform spawned = Spawn( "Test", obj.transform, Vector3.zero, Quaternion.identity, null, Vector3.one );
            if (null != spawned)
                testSpawnedList.Add( spawned );
        }

        GUILayout.BeginHorizontal();
        { 
            GUILayout.BeginVertical();
            GUILayout.Label( "Spawnable List" );

            scrollPos = GUILayout.BeginScrollView( scrollPos );
            foreach (KeyValuePair<string, SpawnPool> pair in PoolManager.Pools)
            {
                foreach (KeyValuePair<string, PrefabPool> ppPair in pair.Value.prefabPools)
                {
                    if (GUILayout.Button(ppPair.Key))
                    {
                        Transform spawned = Spawn( pair.Key, ppPair.Key, Vector3.zero, Quaternion.identity, null, Vector3.one );
                        testSpawnedList.Add(spawned);
                    }
                }            
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label( "Despawnable List" );
            
            scrollPos2 = GUILayout.BeginScrollView( scrollPos2 );
            List<Transform> removing = new List<Transform>();
            foreach(Transform spawned in testSpawnedList)
            {
                if (GUILayout.Button(spawned.ToString()))
                {
                    Despawn( spawned );
                    removing.Add( spawned );
                }
            }        
            GUILayout.EndScrollView();
            // 자동삭제된 객체들 모두 제거
            testSpawnedList.RemoveAll( t => null == t );
            testSpawnedList.RemoveAll( t => removing.Contains(t) );
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
#endif
}
