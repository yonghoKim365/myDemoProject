using UnityEngine;
using System.Collections;

/// <summary>
/// 실제 스폰작업을 수행할 인터페이스
/// </summary>
public interface ISpawner
{
    SpawnGroup Owner { set; get; }
    /// <summary>
    /// 실제 소환
    /// </summary>
    void Spawn();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="evtType"></param>
    void SendEvent(int evtType);

    void Preload();
}
