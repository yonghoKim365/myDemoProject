using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor( typeof( SpawnController ) )]
public class SpawnControllerEditor : Editor
{
    List<ISpawner> spawnerList = new List<ISpawner>();
    List<Transform> transList = new List<Transform>();
    Dictionary<uint, List<NPCSpawner>> npcDic = new Dictionary<uint, List<NPCSpawner>>();

    GUIStyle txtColStyle;
    GUIStyle richStyle;

    void OnEnable()
    {
        FillSpawnerList();

        txtColStyle = new GUIStyle( EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).label );
        txtColStyle.normal.textColor = Color.white;
        richStyle = new GUIStyle( EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).label );
        richStyle.richText = true;

        SceneView.onSceneGUIDelegate += OnScene;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    void FillSpawnerList()
    {
        SpawnController spawnCtlr = target as SpawnController;
        if (null == spawnCtlr)
            return;

        Transform[] gos = spawnCtlr.GetComponentsInChildren<Transform>() as Transform[];
        if (null == gos)
            return;

        spawnerList.Clear();

        for (int i = 0; i < gos.Length; i++)
        {
            ISpawner spawner = (ISpawner)gos[i].GetComponent( "ISpawner" );
            if (null != spawner)
            {
                spawnerList.Add( spawner );
                transList.Add( gos[i] );
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button( "Arrange for Spawners", GUILayout.Height( 100 ) ))
        {
            EditorGUILayout.LabelField( "All Count : " + transList.Count );
            for (int i = 0; i < transList.Count; i++)
            {
                if (null != transList[i])
                    ValidatePosition( transList[i] );
            }
        }
    }

    void ValidatePosition(Transform target)
    {
        NavMeshHit navHit;
        Vector3 targetPos = target.position;
        if (NavMesh.SamplePosition( targetPos, out navHit, 20f, 9 ))
        {
            // 9 == Terrain
            target.position = navHit.position;
        }
    }

    void OnScene(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.Label( "<b>Unit Spawn Information (Total count : " + spawnerList.Count + ")</b>", richStyle );

        npcDic.Clear();
        for (int i = 0; i < spawnerList.Count; i++)
        {
            ISpawner spawner = spawnerList[i];
            if (spawner is NPCSpawner)
            {
                NPCSpawner nSpawner = spawner as NPCSpawner;
                uint npcID = (uint)nSpawner.unitId;
                if (!npcDic.ContainsKey( npcID ))
                    npcDic.Add( npcID, new List<NPCSpawner>() );

                npcDic[npcID].Add( nSpawner );
            }
        }

        foreach (KeyValuePair<uint, List<NPCSpawner>> pair in npcDic)
        {
            GUILayout.Label( "NPCID : " + pair.Key + " | Count : " + pair.Value.Count + (pair.Value[0].isBoss ? " [BOSS]" : string.Empty), txtColStyle );
        }

        Handles.EndGUI();
    }
}
