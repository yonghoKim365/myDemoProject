using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UIHeroInvenList))]
public class UIHeroInvenListTester : Editor 
{
	UIHeroInvenList _list;
	
	void OnEnable()
	{
		_list = target as UIHeroInvenList;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("추가"))
		{
			_list.addTest();
		}
		EditorGUILayout.EndHorizontal();

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}

