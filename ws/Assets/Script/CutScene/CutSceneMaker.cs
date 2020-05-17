using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class CutSceneMaker : MonoBehaviour {

	[HideInInspector]
	public DebugManager debugManager;

	[HideInInspector]
	public ResourceManager resourceManager;

	public bool usePrefabEffect = false;

	public int searchTimeOffset = 5;

	public int targetSearchTime = 10;

	public static CutSceneMaker instance;

	public bool useCutSceneMaker = false;

	public string nowCutSceneId = "";

	public string source;

	public static bool useSearchMode = false;
	public static float targetTime = 0.0f;


	void Awake () {
		instance = this;
#if UNITY_EDITOR

#else
		useCutSceneMaker = false;
#endif

	}


	public void restart()
	{
		useSearchMode = false;
		GameManager.me.startQuickRestart();
	}


	public void play()
	{
		useSearchMode = false;

		parseSource();

		GameManager.me.cutSceneManager.tfPlayTime.gameObject.SetActive(true);
		GameManager.me.cutSceneManager.tfPlayTime.enabled = true;

		GameManager.me.cutSceneManager.lbCutSceneId.gameObject.SetActive(true);
		GameManager.me.cutSceneManager.lbCutSceneId.enabled = true;


		if(nowCutSceneId != null && GameManager.info.cutSceneData.ContainsKey(nowCutSceneId))
		{
			GameManager.info.roundData["PVP"].cutSceneId = nowCutSceneId;
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData["PVP"],GameType.Mode.Championship);
			GameManager.me.startGame(0);
		}
	}

	public void goToTime(float time)
	{
		if(time > 1)
		{
			useSearchMode = true;
			targetTime = time;
		}

		GameManager.me.startQuickRestart();
	}


	public bool parseSource()
	{
		Dictionary<string, CutSceneData> cutSceneData = new Dictionary<string, CutSceneData>(StringComparer.Ordinal);
		Dictionary<string, int> k = new Dictionary<string, int>();
		k["ID"] = 0;
		k["HEADER"] = 1;
		k["COMMAND"] = 2;
		k["ATTR1"] = 3;
		k["ATTR2"] = 4;
		k["ATTR3"] = 5;
		k["ATTR4"] = 6;
		k["ATTR5"] = 7;
		k["ATTR6"] = 8;
		k["ATTR7"] = 9;
		k["ATTR8"] = 10;

		if(string.IsNullOrEmpty(source)) return false;

		string[] lines = source.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);

		int len = lines.Length;

		List<object> list = new List<object>();

		bool isFirst = true;

		for(int i = 0; i < len; ++i)
		{
			if(string.IsNullOrEmpty(lines[i])) continue;

			string[] singleLines = Util.CsvParser(lines[i]);

			if(singleLines[0] != "CS") continue;

			list.Clear();

			int len2 = singleLines.Length;

			for(int j = 1; j < 12; ++j)
			{
				if(j < len2)
				{
					list.Add( singleLines[j] );
				}
				else
				{
					list.Add("");
				}
			}


			string id = (string)list[k["ID"]];
			
			if(cutSceneData.ContainsKey(id) == false)
			{
				cutSceneData.Add(id, new CutSceneData());

				if(isFirst)
				{
					nowCutSceneId = id;
					isFirst = false;
				}
			}

			cutSceneData[id].id = id;
			cutSceneData[id].setData(list, k);
		}

		foreach(KeyValuePair<string, CutSceneData> kv in cutSceneData)
		{
			kv.Value.setDataFinallize();
		}

		if(cutSceneData == null || cutSceneData.Count < 1) return false;

		if(Application.isEditor && Application.isPlaying)
		{
			GameManager.info.cutSceneData = cutSceneData;
		}
		else return false;

		return true;

	}


}
