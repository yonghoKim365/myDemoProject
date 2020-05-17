
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPlayTagSlot))]
public class UIPlayTagSlotEditor : Editor 
{
	UIPlayTagSlot _editor;
	
	void OnEnable()
	{
		_editor = target as UIPlayTagSlot;
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("플레이"))
		{
			_editor.testCode();
		}
		EditorGUILayout.EndHorizontal();
		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
		
	}
}