using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPlayStageClear))]
public class UIPlayStageClearEditor : Editor 
{
	UIPlayStageClear _editor;
	
	void OnEnable()
	{
		_editor = target as UIPlayStageClear;
	}
	
	public override void OnInspectorGUI()
	{
		
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("UI 초기화."))
		{
			_editor.collectUI();
		}
		EditorGUILayout.EndHorizontal();
		

		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
