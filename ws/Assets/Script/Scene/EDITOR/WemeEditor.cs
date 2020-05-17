using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(Weme))]
public class WemeEditor : Editor 
{
	Weme _editor;
	
	void OnEnable()
	{
		_editor = target as Weme;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("초기화"))
		{
			_editor.editorInit();
		}
		EditorGUILayout.EndHorizontal();
	}
}