using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;


public class WindSoulConfigEditor : EditorWindow 
{
	[MenuItem ("WindSoul Editor/Open Setup", false, 0) ]
	static void Init () 
	{
		EditorWindow.GetWindow<WindSoulConfigEditor>(false, "Config", true).Show();
	}	

	void OnFocus()
	{
		init();
	}


	public DebugManager dm;
	public UnitSkillCamMaker um;
	public ResourceManager rm;

	void OnGUI () 
	{

		dm = (DebugManager)EditorGUILayout.ObjectField(dm, typeof(DebugManager), true);
		um = (UnitSkillCamMaker)EditorGUILayout.ObjectField(um, typeof(UnitSkillCamMaker), true);
		rm = (ResourceManager)EditorGUILayout.ObjectField(rm, typeof(ResourceManager), true);

		NGUIEditorTools.DrawSeparator();
		GUILayout.Space(10f);
		GUILayout.Space(10f);
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if(GUILayout.Button("전투 에디터 모드 (디버그 & PC 데이터)"))
		{
			dm.useDebug = true;
			rm.useAssetDownload = false;
			um.useEffectSkillCamEditor = false;
			um.useUnitSkillCamEditor = false;
			um.useUnitSkillCamMaker = false;
			um.gameResourceErrorCheck = false;
		}
		GUILayout.Space(20f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);


		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if(GUILayout.Button("실제 게임용 (인터넷 연결 & 리소스 다운로드)"))
		{
			dm.useDebug = false;
			rm.useAssetDownload = true;
			um.useEffectSkillCamEditor = false;
			um.useUnitSkillCamEditor = false;
			um.useUnitSkillCamMaker = false;
			um.gameResourceErrorCheck = false;
		}
		GUILayout.Space(20f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if(GUILayout.Button("유닛캠 (디버그 & PC 리소스) & 에러체크"))
		{
			dm.useDebug = true;
			rm.useAssetDownload = false;

			um.useUnitSkillCamMaker = true;
			um.useEffectSkillCamEditor = false;
			um.useUnitSkillCamEditor = true;

			um.gameResourceErrorCheck = true;

		}
		GUILayout.Space(20f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);


		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if(GUILayout.Button("이펙트 에디터 (디버그 & PC 리소스)"))
		{
			dm.useDebug = true;
			rm.useAssetDownload = false;

			um.useUnitSkillCamMaker = true;
			um.useEffectSkillCamEditor = true;
			um.useUnitSkillCamEditor = false;

			um.gameResourceErrorCheck = false;
			um.disablePlayerAttack = true;

			dm.debugUnitId = new string[5];
			dm.debugRoundId = "TEST";

		}
		GUILayout.Space(20f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);

	}


	void OnEnable () 
	{ 
		init();
	}


	void init()
	{
		if(Application.isPlaying ) return;

		GameObject go = GameObject.Find("00 DebugManager");
		
		if(go != null)
		{
			dm = go.GetComponent<DebugManager>();
		}
		
		go = GameObject.Find("00_2 UnitCamMaker");
		
		if(go != null)
		{
			um = go.GetComponent<UnitSkillCamMaker>();
		}
		
		go = GameObject.Find("Managers/ResourceManager");
		
		if(go != null)
		{
			rm = go.GetComponent<ResourceManager>();
		}
	}

	void OnDisable () 
	{ 

	}
	
}
