using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BattleSimulator))]
public class BattleSimulatorHelper : Editor 
{
	BattleSimulator _editor;
	
	void OnEnable()
	{
		_editor = target as BattleSimulator;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("시뮬레이션 시작"))
		{
			_editor.startSimulation();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("배치 시뮬레이션 시작"))
		{
			_editor.batchStart();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("파일 로드"))
		{
			_editor.loadFile();
		}
		if(GUILayout.Button("파일 저장"))
		{
			_editor.saveFile();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("빠른재실행"))
		{
			_editor.restartGame();
		}
		EditorGUILayout.EndHorizontal();



		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}

