using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UISummonDetailPopup))]
public class UISummonDetailPopupEditor : Editor 
{
	UISummonDetailPopup _target;
	
	void OnEnable()
	{
		_target = target as UISummonDetailPopup;
	}

	private float shotSize = 1.0f;

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();

		shotSize = EditorGUILayout.FloatField(shotSize);

		if(GUILayout.Button("사이즈 수정"))
		{
			_target.resize(shotSize);
		}

		EditorGUILayout.EndHorizontal();

		base.DrawDefaultInspector();

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}

