using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(ParticleResizeUtil))]

public class ParticleResizeUtilEditor : Editor 
{
	ParticleResizeUtil _util;
	
	void OnEnable()
	{
		_util = target as ParticleResizeUtil;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("초기화"))
		{
//			GameObject go = Selection.activeGameObject;
			_util.init();
		}
		EditorGUILayout.EndHorizontal();	

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("변경"))
		{
			GameObject go = Selection.activeGameObject;
			_util.resize();
		}
		EditorGUILayout.EndHorizontal();	
	}
}

