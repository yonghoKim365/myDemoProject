using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UIHeroPreviewDataSetter))]
public class UIHeroDetailEditor : Editor 
{
	UIHeroPreviewDataSetter _target;
	
	void OnEnable()
	{
		_target = target as UIHeroPreviewDataSetter;
	}
	
	public override void OnInspectorGUI()
	{

		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("출력"))
		{
			_target.getPreviewPositionValue();
		}

		if(GUILayout.Button("입력"))
		{
			_target.setPreviewPositionValue();
		}

		EditorGUILayout.EndHorizontal();


		base.DrawDefaultInspector();


		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}

