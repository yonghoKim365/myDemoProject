using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor 
{
	DebugManager _editor;
	SerializedObject _csm;
	
	void OnEnable()
	{
		_editor = target as DebugManager;
		_csm = new SerializedObject(_editor);
	}
	
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("unit1"))
		{
			_editor.loadDebugUnit(ClipboardHelper.clipBoard, 0);
		}

		if(GUILayout.Button("unit2"))
		{
			_editor.loadDebugUnit(ClipboardHelper.clipBoard, 1);
		}

		if(GUILayout.Button("pvp unit1"))
		{
			_editor.loadDebugUnit(ClipboardHelper.clipBoard, 2);
		}


		if(GUILayout.Button("pvp unit2"))
		{
			_editor.loadDebugUnit(ClipboardHelper.clipBoard, 3);
		}




		if(GUILayout.Button("skill1"))
		{
			_editor.loadDebugSkill(ClipboardHelper.clipBoard, 0);
		}
		
		if(GUILayout.Button("skill2"))
		{
			_editor.loadDebugSkill(ClipboardHelper.clipBoard, 1);
		}
		
		if(GUILayout.Button("pvp skill1"))
		{
			_editor.loadDebugSkill(ClipboardHelper.clipBoard, 2);
		}
		
		
		if(GUILayout.Button("pvp skill2"))
		{
			_editor.loadDebugSkill(ClipboardHelper.clipBoard, 3);
		}


		if(GUILayout.Button("장비1"))
		{
			_editor.loadDebugParts(ClipboardHelper.clipBoard, 0);
		}
		
		if(GUILayout.Button("장비2"))
		{
			_editor.loadDebugParts(ClipboardHelper.clipBoard, 1);
		}
		
		if(GUILayout.Button("pvp 장비"))
		{
			_editor.loadDebugParts(ClipboardHelper.clipBoard, 2);
		}
		
		
		if(GUILayout.Button("pvp 장비2"))
		{
			_editor.loadDebugParts(ClipboardHelper.clipBoard, 3);
		}



	}
	
}


