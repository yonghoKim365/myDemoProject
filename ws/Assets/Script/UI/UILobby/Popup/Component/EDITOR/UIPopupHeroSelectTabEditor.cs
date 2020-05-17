using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(UIPoupHeroSelectTab))]

public class UIPoupHeroSelectTabEditor : Editor 
{
	UIPoupHeroSelectTab _manager;
	
	void OnEnable()
	{
		_manager = target as UIPoupHeroSelectTab;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("초기화"))
		{
			_manager.changeTab();
		}
	}
}