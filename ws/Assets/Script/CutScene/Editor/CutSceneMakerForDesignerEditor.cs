using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(CutSceneMakerForDesigner))]
public class CutSceneMakerForDesignerEditor : Editor 
{
	CutSceneMakerForDesigner _editor;
	SerializedObject _csm;
	
	void OnEnable()
	{
		_editor = target as CutSceneMakerForDesigner;
		_csm = new SerializedObject(_editor);
	}
	
	string nowCutSceneId = "";
	float nowCutScenePlayTime = 0;
	public override void OnInspectorGUI()
	{
		_csm.Update();
		
		base.DrawDefaultInspector();

		if(_editor.useCutSceneMaker)
		{
			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));
			
			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("재시작"))
			{
				UnityEditor.EditorApplication.isPaused = false;
				
				if(Application.isPlaying)
				{
					_editor.play();
				}
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));
			

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button(_editor.targetSearchTime + "초로 이동"))
			{
				UnityEditor.EditorApplication.isPaused = false;
				
				if(Application.isPlaying)
				{
					_editor.goToTime(_editor.targetSearchTime);
				}
			}
			EditorGUILayout.EndHorizontal();

		}
		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
		_csm.ApplyModifiedProperties();
		
	}
	
}

