using UnityEngine;
using System.Collections;
using UnityEditor;


//[CustomEditor(typeof(NPCSpawner))]
[CanEditMultipleObjects]
public class NPCSpawnerEditor : Editor {

	SerializedProperty LinkedNpcNum;

	void OnEnable()
	{
		LinkedNpcNum = serializedObject.FindProperty ("LinkedNpcNum");
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		EditorGUILayout.PropertyField (LinkedNpcNum);
		serializedObject.ApplyModifiedProperties ();
		EditorGUILayout.LabelField ("  Linked Npc Number ");
	}
}
