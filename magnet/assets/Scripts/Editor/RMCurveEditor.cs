using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(RMCurve))]
public class RMCurveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (null == target)
            return;

        RMCurve curData = target as RMCurve;

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField( "Matched Animation Name", GUILayout.Width( 200 ) );
        EditorGUILayout.LabelField( curData.Name, new GUIStyle( "textfield" ), GUILayout.ExpandWidth(true) );
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField( "Matched Animation Length", GUILayout.Width( 200 ) );
        EditorGUILayout.LabelField( curData.Length.ToString(), new GUIStyle( "textfield" ), GUILayout.ExpandWidth(true) );
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical("box");
        {   
            EditorGUILayout.LabelField( "Motion Curve Data" );

            AnimationCurve[] curves = new AnimationCurve[] { curData.XCurve, curData.YCurve, curData.ZCurve };
            for (int i = 0; i < curves.Length; i++)
            {
                AnimationCurve curve = curves[i];
                if (null == curve)
                    continue;

                curve = EditorGUILayout.CurveField( curve, Color.green, new Rect( 0, 0, 0, 0 ), GUILayout.ExpandWidth( true ), GUILayout.Height( 100 ) );
            }
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty( curData );
            serializedObject.ApplyModifiedProperties();
        }
    }
}
