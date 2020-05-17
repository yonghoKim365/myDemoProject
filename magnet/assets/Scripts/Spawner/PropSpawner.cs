using UnityEngine;
using System.Collections;

public class PropSpawner : MonoBehaviour, ISpawner
{
    public eTeamType teamType = eTeamType.Team2;
    public int propId = 0;

    public SpawnGroup Owner { set; get; }

    void Start()
    { 
        // 게임내 필요없는 객체는 삭제 (Dummy 객체 삭제)
        transform.DestroyChildren( false );
    }
    
    public void Preload()
    {
        
    }

    /// <summary>
    /// ISpawner의 실구현
    /// </summary>
    public void Spawn()
    {
        if (G_GameInfo.GameInfo != null)
        {
            uint propID = 0;
            Mob.PropGroupInfo propGroup = _LowDataMgr.instance.GetPropGroup((uint)propId);

            if(propGroup != null)
            {
                //프롭그룹이 있으면

                uint randValue = (uint)Random.Range(0f, 1000f);
                uint randMax = 0;
                bool find = false;

                for(int i=0;i<propGroup.propIdx.Count; i++)
                {
                    uint range = uint.Parse(propGroup.propRate[i]);

                    randMax += range;

                    if( randValue < randMax )
                    {
                        //찾음
                        find = true;
                        propID = uint.Parse(propGroup.propIdx[i]);
                        break;
                    }
                }

                if(!find)
                {
                    propID = 0;
                }
            }
            else
            {
                propID = (uint)propId;
            }

            if(propID != 0)
            {
                GameObject npcGo = G_GameInfo.GameInfo.SpawnProp(propID, teamType, 1000, transform.position, transform.rotation);
                npcGo.GetComponent<Prop>().spawnner = this as ISpawner;
            }
        }
    }

    public void SendEvent(int evtType)
    {
        switch ((SpawnGroup.eEvent)evtType)
        {
            case SpawnGroup.eEvent.Spawned:
                Owner.SendMessage( "OnEvent", new SpawnEventArgs() { sender = Owner, eventType = SpawnGroup.eEvent.Spawned } );
                break;

            case SpawnGroup.eEvent.Dead:
                Owner.SendMessage( "OnEvent", new SpawnEventArgs() { sender = Owner, eventType = SpawnGroup.eEvent.Dead } );
                break;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (0 == propId)
            return;

        string imgName = propId.ToString();        
        Gizmos.DrawIcon( transform.position + new Vector3( 0, 1, 0 ), "Unit/" + imgName.ToString(), true );
        UnityEditor.Handles.ArrowCap( 0, transform.position, transform.rotation, 2f );
        UnityEditor.Handles.DrawWireArc( transform.position, transform.up, transform.right, 360, 1f );
    }
#endif
}
