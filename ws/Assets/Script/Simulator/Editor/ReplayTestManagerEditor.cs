using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ReplayTestManager))]
public class ReplayTestManagerEditor : Editor 
{
	ReplayTestManager _editor;
	
	void OnEnable()
	{
		_editor = target as ReplayTestManager;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("플레이"))
		{
			_editor.init();
			_editor.play();
		}
		EditorGUILayout.EndHorizontal();


		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}

