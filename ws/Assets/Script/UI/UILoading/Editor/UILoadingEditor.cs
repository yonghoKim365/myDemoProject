using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(UILoading))]

public class UILoadingEditor : Editor 
{
	UILoading _manager;
	
	void OnEnable()
	{
		_manager = target as UILoading;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("초기화"))
		{
			_manager.initEditMode();
		}

		if(GUILayout.Button("다음유닛"))
		{
			_manager.editModeNextMonster();
		}

		if(GUILayout.Button("사이즈변경"))
		{
			_manager.resize();
		}

		if(GUILayout.Button("제거"))
		{
			_manager.removeSample();
		}

		if(GUILayout.Button("코드저장"))
		{
			_manager.saveCode();
		}

		if(GUILayout.Button("코드분석"))
		{
			_manager.parseCode();
		}

		EditorGUILayout.EndHorizontal();	
	}
}
