using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfiniteSpawnGroup : SpawnGroup
{    
    public override void Init(GameObject _mainAgent, bool preload = false)
    {
        Ready(null);

        mainAgent = _mainAgent;
    }

    protected override void OnEvent(object obj)
    {
        SpawnEventArgs args = obj as SpawnEventArgs;
        if (null == args)
            return;

        switch (args.eventType)
        {
            case eEvent.Dead:
                {
                    // 소환된 모든 NPC들이 죽었는지 검사!
                    if (G_GameInfo.CharacterMgr.unitGroupDic.ContainsKey(groupNo))
                    {
                        List<Unit> enemyList = G_GameInfo.CharacterMgr.unitGroupDic[groupNo];

                        bool isAllDead = enemyList.TrueForAll( (unit) => unit.CharInfo.IsDead );
                        if (isAllDead)
                            OnEvent( new SpawnEventArgs() { sender = this, eventType = eEvent.AllDead } );
                    }
                }
                break;

            case eEvent.AllDead:
                // 다음 웨이브 시작되도록 하기
                if (null != G_GameInfo.GameInfo)
                    G_GameInfo.GameInfo.SendMessage( "NextWave", SendMessageOptions.RequireReceiver );
                break;
        }
    }

    /// <summary>
    /// 설정된 웨이브 정보 기반으로 게임 시작!
    /// </summary>
    public void SpawnNpcs()
    {
        // 스폰한번하고 난뒤에 비활성화 되기 때문에, 복구
        enabled = true;

        Execute();
    }
}
