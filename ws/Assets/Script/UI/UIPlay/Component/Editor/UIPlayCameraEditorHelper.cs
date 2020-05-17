using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPlayCameraEditor))]
public class UIPlayCameraEditorHelper : Editor 
{
	UIPlayCameraEditor _editor;
	
	void OnEnable()
	{
		_editor = target as UIPlayCameraEditor;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Shot"))
		{
			_editor.playShot();
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Move"))
		{
			_editor.playMove();
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Rotate"))
		{
			_editor.playRotate();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("PlayCodes"))
		{
			_editor.startPlayCodes();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("GetCamPosition"))
		{
			_editor.getPositionCameraLocalPosition();
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Get&ApplyCamPosition"))
		{
			_editor.getPositionCameraLocalPosition();
			_editor.copyCamLocalPositionToValue();
		}
		EditorGUILayout.EndHorizontal();

		
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Reset"))
		{
			_editor.play.resetCamera();
			GameManager.me.mapManager.inGameMap.setVisible(true);

		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Reset & use GameCam"))
		{
			GameManager.me.cutSceneManager.useCutSceneCamera = false;
			_editor.play.resetCamera();
			GameManager.me.mapManager.inGameMap.setVisible(true);
			
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("getCode"))
		{
			_editor.paramToCode();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("setCode"))
		{
			_editor.codeToParam();
		}
		EditorGUILayout.EndHorizontal();



		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}