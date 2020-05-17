
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIGameResultString))]
public class UIGameResultStringEditor : Editor 
{
	UIGameResultString _editor;
	
	void OnEnable()
	{
		_editor = target as UIGameResultString;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("플레이"))
		{
			_editor.test();
		}
		EditorGUILayout.EndHorizontal();

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}