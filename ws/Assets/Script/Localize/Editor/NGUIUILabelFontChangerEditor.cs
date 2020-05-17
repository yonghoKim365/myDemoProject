using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(NGUIUILabelFontChanger))]

public class NGUIUILabelFontChangerEditor : Editor 
{
	NGUIUILabelFontChanger _util;
	
	void OnEnable()
	{
		_util = target as NGUIUILabelFontChanger;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("폰트 변경"))
		{
			_util.change(_util.root.GetComponentsInChildren<UILabel>(true));
		}
		EditorGUILayout.EndHorizontal();	
	}
}

