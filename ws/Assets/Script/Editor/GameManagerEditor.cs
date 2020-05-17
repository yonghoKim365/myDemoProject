using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor 
{
	GameManager _editor;
	SerializedObject _csm;
	
	void OnEnable()
	{
		_editor = target as GameManager;
		_csm = new SerializedObject(_editor);
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("게임 재시작"))
			{
				NetworkManager.RestartApplication();
			}

		EditorGUILayout.EndHorizontal();
			
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
		_csm.ApplyModifiedProperties();
		
	}
	
}


