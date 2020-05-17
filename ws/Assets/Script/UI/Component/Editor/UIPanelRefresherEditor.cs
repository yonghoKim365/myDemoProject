using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(UIPanelRefresher))]

public class UIPanelRefresherEditor : Editor 
{
	UIPanelRefresher _manager;
	
	void OnEnable()
	{
		_manager = target as UIPanelRefresher;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Collect"))
		{
			_manager.widgets = _manager.GetComponentsInChildren<UIWidget>(true);
		}
		EditorGUILayout.EndHorizontal();	
	}
}
