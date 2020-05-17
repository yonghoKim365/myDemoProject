using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(AnimationPlayer))]

public class AnimationPlayerEditor : Editor 
{
	AnimationPlayer _util;
	
	void OnEnable()
	{
		_util = target as AnimationPlayer;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("로직변경"))
		{
			_util.reset();
		}
		EditorGUILayout.EndHorizontal();	
	}
}

