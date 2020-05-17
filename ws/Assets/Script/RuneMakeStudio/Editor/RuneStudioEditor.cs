using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(RuneStudioInit))]
public class RuneStudioEditor : Editor 
{
	RuneStudioInit _editor;

	void OnEnable()
	{
		_editor = target as RuneStudioInit;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("프리팹 분해"))
		{
			_editor.clear();



			for(int i  = 1; i < 11; ++i)
			{
				Material mat;

				Vector2 v = new Vector2(0.31f, 0.31f);
				Vector2 off = new Vector2();

				if(i < 10)
				{
					mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/12 UI_STUDIO/Materials/10card_0"+i+".mat",typeof(Material));
				}
				else mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/12 UI_STUDIO/Materials/10card_"+i+".mat",typeof(Material));

				switch(i)
				{
				case 1: off = new Vector2(-0.005f, 0.65f);break;
				case 2: off = new Vector2(0.24f, 0.65f);break;
				case 3: off = new Vector2(0.49f, 0.65f);break;
				case 4: off = new Vector2(0.73f, 0.65f);break;
				case 5: off = new Vector2(-0.005f, 0.344f);break;
				case 6: off = new Vector2(0.24f, 0.344f);break;
				case 7: off = new Vector2(0.49f, 0.344f);break;
				case 8: off = new Vector2(0.73f, 0.344f);break;
				case 9: off = new Vector2(-0.005f, 0.038f);break;
				case 10: off = new Vector2(0.24f, 0.038f);break;

				}

				mat.SetTextureScale("_MainTex",v);
				mat.SetTextureOffset("_MainTex",off);

			}



		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("강화 시작"))
		{
			_editor.studioMain.sendEvent(_editor.studioMain.reinforceStarter);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("제작 시작"))
		{
			_editor.studioMain.sendEvent(_editor.studioMain.makeStarter);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("진화SS"))
		{
			RuneStudioMain.instance.playEvolveResult("UN61401001_0_0", "UN51401001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[4]);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("진화S"))
		{
			RuneStudioMain.instance.playEvolveResult("UN51401001_0_0", "UN41401001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[3]);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("진화A"))
		{
			RuneStudioMain.instance.playEvolveResult("UN41401001_0_0", "UN31401001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[2]);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("진화B"))
		{
			RuneStudioMain.instance.playEvolveResult("UN31401001_0_0", "UN21401001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[1]);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("진화C"))
		{
			RuneStudioMain.instance.playEvolveResult("UN21401001_0_0", "UN11301001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[0]);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("초월"))
		{
			RuneStudioMain.instance.playTranscendResult("UN21401001_0_0", "UN11301001_0_0", GameIDData.Type.Unit);
			//_editor.studioMain.sendEvent(_editor.studioMain.evolveController[0]);
		}
		EditorGUILayout.EndHorizontal();



		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("인게임 리셋 - 강화"))
		{
			_editor.reset( RuneStudioMain.Type.Reinforce );
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("인게임 리셋 - 제작"))
		{
			_editor.reset( RuneStudioMain.Type.UnitMake );
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("인게임 리셋 - 초월"))
		{
			_editor.reset( RuneStudioMain.Type.Transcend );
		}
		EditorGUILayout.EndHorizontal();

	}
}