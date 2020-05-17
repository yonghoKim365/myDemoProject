using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldMapEffectManager))]
public class WorldMapEffectManagerEditor : Editor 
{
	WorldMapEffectManager _editor;
	
	void OnEnable()
	{
		_editor = target as WorldMapEffectManager;
	}
	
	public override void OnInspectorGUI()
	{

		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("이펙트 발사~"))
		{
			_editor.start();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("UI 초기화."))
		{
			_editor.collectUI();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("버튼위치변경"))
		{
			_editor.refreshPosition();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("일반이동"))
		{
			_editor.play(false);
		}

		if(GUILayout.Button("액트오픈"))
		{
			_editor.play(true);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("FSM 초기화"))
		{
			_editor.resetPlayMaker();
		}

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("스테이지 애니동작"))
		{
			_editor.playStageAni();
		}

		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
