using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor( typeof( MovingRoute ) )]
public class MovingRouteInspector : Editor
{
    readonly float btnHeight = 50f;

    MovingRoute Target;
    GameObject selectedNode;

    GUIStyle foldoutStyle;

    void OnEnable()
    {
        Target = target as MovingRoute;
        if (Target.RouteLink.Count > 0)
            SetSelectNode( Target.RouteLink[0] );

        SetStyles();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SetStyles();

        EditorGUILayout.LabelField( "Route Link Count", Target.Count.ToString() );

        ShowListInInspector();

        if (Target.IsEmpty)
        {
            StartGUI();
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Validate Positions", "NavMesh영역내에서 유효한지 검사해서, 위치를 잡아준다"), GUILayout.Height(30)) )
            {
                ValiatePositions();
            }
 
            ButtonBarGUI();
        }
        
        if (GUI.changed)
            SceneView.RepaintAll();
    }

    void OnSceneGUI()
    {
        List<GameObject> routeLink = Target.RouteLink;

        Tools.current = Tool.None;

        Color savedColor = Handles.color;
        foreach (GameObject node in routeLink)
        {
            Transform trans = node.transform;
            Handles.Label( trans.position, node.name + " : " + trans.position );
            Vector3 tempPos = trans.position;
            trans.position = Handles.PositionHandle( trans.position, Quaternion.identity );

            // 위치가 수정된 노드를 선택노드로 변경한다.
            if (tempPos != trans.position)
                SetSelectNode( node );

            Handles.color = node == selectedNode ? Color.red : savedColor;
            Handles.SphereCap( 0, trans.position, Quaternion.identity, 1f );
        }
        Handles.color = savedColor;

        // 모든 라인그리기
        Handles.DrawAAPolyLine( 5f, Target.GetPositions().ToArray() );

        if (GUI.changed)
            EditorUtility.SetDirty( target );
    }

    void ShowListInInspector()
    {
        List<GameObject> routeLink = Target.RouteLink;

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < routeLink.Count; i++)
        {
            GUI.SetNextControlName( "focus" );

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel( "Node " + i.ToString() );
            EditorGUILayout.ObjectField( routeLink[i], typeof( GameObject ), true );
            EditorGUILayout.EndHorizontal();

            // 선택된 노드를 Inspector상에 표시해준다.
            if (selectedNode == routeLink[i])
                EditorGUI.FocusTextInControl( "focus" );
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty( target );
    }


    private void StartGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button( "START", BoldFontStyle( 20 ), GUILayout.ExpandWidth( true ), GUILayout.Height( btnHeight ) ))
        {
            Target.CreateStartPosition();

            SetSelectNode( Target.RouteLink[0] );

            EditorUtility.SetDirty( target );
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox( "[START] 버튼을 클릭하면, 현재 객체기준으로 경로생성을 시작합니다.", MessageType.None );
    }

    private void ButtonBarGUI()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical();
        { 
            EditorGUILayout.Foldout( true, "Current Selected Node", foldoutStyle );
            EditorGUILayout.BeginHorizontal();
            SetSelectNode( EditorGUILayout.ObjectField( "", selectedNode, typeof( GameObject ), true ) as GameObject );
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Frame Selected (Focusing)"))
                FrameSelected( selectedNode );
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();
        GUILayout.BeginHorizontal();
        {
            GameObject newSelectedNode = null;
            int foundIndex = Target.RouteLink.FindIndex( (go) => go == selectedNode );

            // 왼쪽으로 이동가능한지에 따라 활성화시키기
            GUI.enabled = foundIndex != -1 && ( foundIndex - 1 ) >= 0;
            if (GUILayout.Button(new GUIContent( "<<", "현재 선택되어 있는 노드의 이전 노드로 이동"), BoldFontStyle( 20 ), GUILayout.Width( 40 ), GUILayout.Height( btnHeight ) ))
            {
                newSelectedNode = Target.RouteLink[foundIndex - 1];
            }

            GUI.enabled = true;
            if (GUILayout.Button( "Add Prev", BoldFontStyle(), GUILayout.ExpandWidth( true ), GUILayout.Height( btnHeight ) ))
            {
                newSelectedNode = Target.CreatePrev( selectedNode );
            }
            if (GUILayout.Button( "Remove", BoldFontStyle(), GUILayout.ExpandWidth( true ), GUILayout.Height( btnHeight ) ))
            {
                if (Target.Remove( selectedNode ))
                {
                    newSelectedNode = Target.RouteLink.Count > 0 ? Target.RouteLink[0] : null;
                }
            }
            if (GUILayout.Button( "Add Next", BoldFontStyle(), GUILayout.ExpandWidth( true ), GUILayout.Height( btnHeight ) ))
            {
                newSelectedNode = Target.CreateNext( selectedNode );
            }

            // 오른쪽으로 이동가능한지에 따라 활성화시키기
            GUI.enabled = foundIndex != -1 && ( foundIndex + 1 ) < Target.RouteLink.Count;
            if (GUILayout.Button(new GUIContent( ">>", "현재 선택되어 있는 노드의 다음 노드로 이동"), BoldFontStyle( 20 ), GUILayout.Width( 40 ), GUILayout.Height( btnHeight ) ))
            {
                newSelectedNode = Target.RouteLink[foundIndex + 1];
            }

            SetSelectNode( newSelectedNode );
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    void SetSelectNode(GameObject newNode)
    {
        if (null == newNode)
            return;

        bool redraw = newNode != selectedNode;
        
        if (redraw)
        {
            selectedNode = newNode;
            Repaint();
        }
    }

    private GUIStyle BoldFontStyle(int size = 13)
    {
        GUIStyle foldOutStyle = new GUIStyle( EditorStyles.miniButtonMid );
        foldOutStyle.fontStyle = FontStyle.Bold;
        foldOutStyle.fontSize = size;
        foldOutStyle.stretchWidth = false;

        return foldOutStyle;
    }

    private void SetStyles()
    {
        if (null != foldoutStyle)
            return;

        foldoutStyle = new GUIStyle( EditorStyles.foldout );

        Color foldoutColor = new Color( 0.0f, 0.0f, 0.2f );

        foldoutStyle.onNormal.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onFocused.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onActive.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onHover.background = EditorStyles.boldLabel.onNormal.background;

        foldoutStyle.normal.textColor = foldoutColor;
        foldoutStyle.focused.textColor = foldoutColor;
        foldoutStyle.active.textColor = foldoutColor;
        foldoutStyle.hover.textColor = foldoutColor;
        foldoutStyle.fixedWidth = 500;
        foldoutStyle.stretchWidth = true;
    }

    /// <summary>
    /// SceneView에서 객체 포커싱시키기
    /// </summary>
    void FrameSelected(GameObject focusTarget)
    {
        GameObject storedGo = Selection.activeGameObject;
        Selection.activeGameObject = focusTarget;
        SceneView.lastActiveSceneView.FrameSelected();
        Selection.activeGameObject = storedGo;
    }

    /// <summary>
    /// NavMesh영역안에 잘 맞는지 검사하고, 보정해준다.
    /// </summary>
    void ValiatePositions()
    { 
        List<GameObject> routeLink = Target.RouteLink;
        for (int i = 0; i < routeLink.Count; i++)
        {
            // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 NavMesh가능 위치를 찾아준다.
            NavMeshHit navHit;
            Vector3 targetPos = routeLink[i].transform.position;
            if (NavMesh.SamplePosition( targetPos, out navHit, 20f, 9 ))
            {
                // 9 == Terrain
                routeLink[i].transform.position = navHit.position;
            }
        }
    }
}
