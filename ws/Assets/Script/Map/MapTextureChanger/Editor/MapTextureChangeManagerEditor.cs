using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(MapTextureChangeManager))]

public class MapTextureChangeManagerEditor : Editor 
{
	MapTextureChangeManager _manager;
	
	void OnEnable()
	{
		_manager = target as MapTextureChangeManager;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Start"))
		{
			_manager.start();
		}
		EditorGUILayout.EndHorizontal();	

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Collect"))
		{
			_manager.targetObjects = _manager.GetComponentsInChildren<MapTextureChangeObject>(true);
		}
		EditorGUILayout.EndHorizontal();	
	}
}
