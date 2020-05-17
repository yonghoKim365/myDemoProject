using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

public class BattleSimulator : MonoBehaviour {

	public const int ID = 1;
	public const int LV = 2;
	public const int HD = 3;
	public const int BD = 4;
	public const int WP = 5;
	public const int RD = 6;
	public const int U1 = 7;
	public const int U2 = 8;
	public const int U3 = 9;
	public const int U4 = 10;
	public const int U5 = 11;
	public const int S1 = 12;
	public const int S2 = 13;
	public const int S3 = 14;
	public const int AI = 15;
	public const int PVP_START = 16;
	public const int DRAW_TIME = 17;
	
	public const int PVP_ID = 18;
	public const int PVP_LV = 19;
	public const int PVP_HD = 20;
	public const int PVP_BD = 21;
	public const int PVP_WP = 22;
	public const int PVP_RD = 23;
	public const int PVP_U1 = 24;
	public const int PVP_U2 = 25;
	public const int PVP_U3 = 26;
	public const int PVP_U4 = 27;
	public const int PVP_U5 = 28;
	public const int PVP_S1 = 29;
	public const int PVP_S2 = 30;
	public const int PVP_S3 = 31;
	public const int PVP_AI = 32;

	public const int ROUNDID = 33;

	public const int GAMENUM = 34;
	
	public const int WIN = 35;
	public const int LOSE = 36;
	public const int DRAW = 37;
	public const int WINPER = 38;


	public string fileName = "batchsimulator.csv";
	public bool viewBackground = false;

	public int gameCount = 1;
	public static bool nowSimulation = false;

	public float skipTime = 50.0f;

	public static BattleSimulator instance;

	public int nowGameNum = 0;

	public int randomSeed = 4989;
	public bool wantSameResult = false;

	public int nowSeed = 0;

	void Awake()
	{
		if(instance != null && instance != this) Destroy(instance);
		instance = this;

#if UNITY_EDITOR
		loadFile();
#endif
	}

	public int nowBatchNum;
	public int BatchStartNum;
	public int BatchEndNum;

	public int win = 0;
	public int lose = 0;
	public int draw = 0;
	public float winPercent = 0.0f;

	public int rank1 = 0;
	public int rank2 = 0;
	public int rank3 = 0;

	public int maxPoint = 0;
	public int minPoint = 0;
	public float avgPoint = 0;


	List<string[]> csvList = new List<string[]>();
	Dictionary<int, int> csvIndexDic = new Dictionary<int, int>();

	public void loadFile()
	{
		csvIndexDic.Clear();
		csvList.Clear();
		string path = Application.dataPath;
		path = path.Replace("Assets","simulator/"+fileName);

		int arraySize = 39;

		try
		{
			StreamReader sr = new StreamReader(path, Encoding.GetEncoding("euc-kr"));
			while (!sr.EndOfStream)
			{
				string[] csvData = Util.CsvParser(sr.ReadLine());

				if(csvData.Length < arraySize)
				{
					Array.Resize(ref csvData,arraySize);
				}

				csvList.Add(csvData);
			}

			sr.Close();
			sr.Dispose();

			Debug.LogError("=== 파일 읽기 완료 ===");
		}
		catch(System.Exception e)
		{
			Debug.LogError("파일 읽기 에러!!!! : " + e.Message);
			return;
		}

		int temp;
		UnitData ud;

		for(int i = 0; i < csvList.Count; ++i)
		{
			temp = -1000;
			int.TryParse(csvList[i][0],out temp);
			
			if(temp > 0)
			{
				csvIndexDic[temp] = i;
//				Debug.Log(temp + "    INDEX: " + i);
			}
		}
	}

	public void saveFile()
	{
		try
		{
			string path = Application.dataPath;
			path = path.Replace("Assets","simulator/"+fileName);

			StringBuilder sb = new StringBuilder();
			
			for(int i = 0; i < csvList.Count; ++i)
			{
				sb.Append( string.Join(",",csvList[i])+"\r\n");
			}

			string data = sb.ToString();
			System.IO.File.WriteAllText(path,  data,Encoding.GetEncoding("utf-8"));
			System.IO.File.WriteAllText(path + ".bak",  data,Encoding.GetEncoding("utf-8"));
			
			Debug.LogError("저장 완료!");
		}
		catch(System.Exception e)
		{
			Debug.LogError("파일 쓰기 에러!!!! " + e.Message);
		}
	}


	string tStr = "";
	public void startSimulation()
	{
		win = 0;
		lose = 0;
		draw = 0;

		rank1 = 0;
		rank2 = 0;
		rank3 = 0;

		bTestMinPoint = -1;
		bTestMaxPoint = -1;
		bTestAvgPoint = 0;

		maxPoint = 0;
		minPoint = 0;
		avgPoint = 0;

		//if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_MENU)
		{
			nowSimulation = true;
			nowGameNum = 0;

			log ("=============================================");
			log ("* Player");
			log ("ID : "+DebugManager.instance.defaultHero);

			log ("Equip : "+DebugManager.instance.equipHead,DebugManager.instance.equipBody,DebugManager.instance.equipWeapon,DebugManager.instance.equipVehicle);

			tStr = "";
			for(int i = 0; i < DebugManager.instance.debugUnitId.Length; ++i)
			{
				tStr += DebugManager.instance.debugUnitId[i] + ",";
			}

			log ("Unit : "+tStr);

			tStr = "";
			for(int i = 0; i < DebugManager.instance.debugSkillId.Length; ++i)
			{
				tStr += DebugManager.instance.debugSkillId[i] + ",";
			}

			log ("Skill : "+tStr);

			log ("=============================================");
			log ("* PVP");
			log ("ID : "+DebugManager.instance.pvpDefaultHero);

			log ("Equip : "+DebugManager.instance.pvpEquipHead,DebugManager.instance.pvpEquipBody,DebugManager.instance.pvpEquipWeapon,DebugManager.instance.pvpEquipVehicle);


			tStr = "";
			for(int i = 0; i < DebugManager.instance.pvpDebugUnitId.Length; ++i)
			{
				tStr += DebugManager.instance.pvpDebugUnitId[i] + ",";
			}
			
			log ("Unit : "+tStr);
			
			tStr = "";
			for(int i = 0; i < DebugManager.instance.pvpDebugSkillId.Length; ++i)
			{
				tStr += DebugManager.instance.pvpDebugSkillId[i] + ",";
			}

			log ("Skill : "+tStr);

			log ("=============================================");

			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[DebugManager.instance.debugRoundId],GameType.Mode.Epic);

			nextGame(true);
		}
	}


	private int bTestMinPoint = -1;
	private int bTestMaxPoint = -1;
	private int bTestAvgPoint = 0;

	public void endSimulation()
	{
		if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.B_TEST)
		{
			int bTestKillCount = GameManager.me.mapManager.killedUnitCount;

			if(bTestKillCount > bTestMaxPoint) bTestMaxPoint = bTestKillCount;
			if(bTestKillCount < bTestMinPoint || bTestMinPoint < 0) bTestMinPoint = bTestKillCount;

			bTestAvgPoint += bTestKillCount;

			maxPoint = bTestMaxPoint;
			minPoint = bTestMinPoint;
			avgPoint = ((float)((float)bTestAvgPoint / (float)nowGameNum));
		}
//		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Challenge)
//		{
//			switch( GameManager.me.uiManager.uiPlay.challangeModeInfo.rank )
//			{
//			case 1: ++rank1; break;
//			case 2: ++rank2; break;
//			case 3: ++rank3; break;
//			}
//		}

		winPercent = ((float)(((float)win/(float)(win+lose))*1000.0f)*0.1f);
		if(nowGameNum >= gameCount)
		{
			nowSimulation = false;
			log ("=========================");
			log ("승: " + win + "   패배:"+lose + "     승률:"+ ((float)(((float)win/(float)(win+lose))*1000.0f)*0.1f));

			saveFileLog();

			if(nowBatchPlay)
			{
				nextBatch();
			}
			else
			{
				GameManager.me.returnToSelectScene();
			}
		}
		else
		{
			GameManager.me.player.isEnabled = false;
			GameManager.me.mapManager.isSetStage = false;		
			//GameManager.me.clearMemory();

			nextGame();
		}
	}

	private void nextGame(bool isFirst = false)
	{
		++nowGameNum;
		//yield return null;
#if UNITY_EDITOR
		//Log.clearFileLog();
		//if(needToSaveFileLog) Log.saveFileLog();
		//else Log.clearFileLog();
		Log.saveFileLog();
#endif
	

		//GameManager.me.startGame(0.01f);
		GameManager.me.startSimulator(isFirst);
	}


	int nowBatchIndex = 0;
	bool nowBatchPlay = false;
	public void batchStart ()
	{
		nowBatchPlay = true;
		preloadForBatchSimulation();
		nowBatchIndex = BatchStartNum;
		nextBatch(true);
	}

	void nextBatch(bool isFirst = false)
	{

		Resources.UnloadUnusedAssets();
		System.GC.Collect();

		nowBatchNum = nowBatchIndex;

		Debug.LogError("== next Batch : " + nowBatchIndex);
		if(isFirst == false)
		{

			if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.B_TEST)
			{
				// 데이터 저장.
				csvList[csvIndexDic[nowBatchIndex-1]][WIN] = nowGameNum + "";
				csvList[csvIndexDic[nowBatchIndex-1]][LOSE] = bTestMaxPoint + "";
				csvList[csvIndexDic[nowBatchIndex-1]][DRAW] = bTestMinPoint + "";
				csvList[csvIndexDic[nowBatchIndex-1]][WINPER] = ((float)((float)bTestAvgPoint / (float)nowGameNum)) + "";
			}
//			else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Challenge)
//			{
//				// 데이터 저장.
//				csvList[csvIndexDic[nowBatchIndex-1]][WIN] = rank1 + "";
//				csvList[csvIndexDic[nowBatchIndex-1]][LOSE] = rank2 + "";
//				csvList[csvIndexDic[nowBatchIndex-1]][DRAW] = rank3 + "";
//				csvList[csvIndexDic[nowBatchIndex-1]][WINPER] = "";
//			}
			else
			{
				// 데이터 저장.
				csvList[csvIndexDic[nowBatchIndex-1]][WIN] = win + "";
				csvList[csvIndexDic[nowBatchIndex-1]][LOSE] = lose + "";
				csvList[csvIndexDic[nowBatchIndex-1]][DRAW] = draw + "";
				csvList[csvIndexDic[nowBatchIndex-1]][WINPER] = winPercent + "";
			}

		}

		saveFile();

		if(csvIndexDic.ContainsKey(nowBatchIndex) && nowBatchIndex <= BatchEndNum)
		{
			float.TryParse(csvList[csvIndexDic[nowBatchIndex]][DRAW_TIME], out DebugManager.instance.pvpDrawTime);
			DebugManager.instance.pvpDrawAfterTimeOver = (DebugManager.instance.pvpDrawTime > 0.0f);
			int.TryParse(csvList[csvIndexDic[nowBatchIndex]][GAMENUM], out gameCount);

			DebugManager.instance.debugRoundId = csvList[csvIndexDic[nowBatchIndex]][ROUNDID];

			setBatchAsset();
			++nowBatchIndex;
			startSimulation();
		}
		else
		{
			++nowBatchIndex;
			endBatch();
		}
	}

	void endBatch()
	{
		if(nowBatchIndex >= BatchEndNum)
		{
			nowBatchPlay = false;
			//saveFile();
			GameManager.me.returnToSelectScene();
		}
		else
		{
			nextBatch();
		}
	}

	string[] units = new string[5];
	string[] skills = new string[3];
	void setBatchAsset()
	{
		Debug.LogError("== setBatchAsset!!! == ");

		string[] d = csvList[csvIndexDic[nowBatchIndex]];

		units[0] = d[U1];
		units[1] = d[U2];
		units[2] = d[U3];
		units[3] = d[U4];
		units[4] = d[U5];

		skills[0] = d[S1];
		skills[1] = d[S2];
		skills[2] = d[S3];

		int lv = -1;
		int.TryParse(d[LV],out lv);
		DebugManager.instance.setPlayerData(GameDataManager.instance.heroes[d[ID]],true,d[ID],d[HD],d[BD],d[WP],d[RD],units,skills,d[AI]);

		DebugManager.instance.defaultHero = d[ID];

		DebugManager.instance.equipHead = d[HD];
		DebugManager.instance.equipBody = d[BD];
		DebugManager.instance.equipWeapon = d[WP];
		DebugManager.instance.equipVehicle = d[RD];

		DebugManager.instance.debugUnitId = units;
		DebugManager.instance.debugSkillId = skills;
		DebugManager.instance.ai = d[AI];


		units[0] = d[PVP_U1];
		units[1] = d[PVP_U2];
		units[2] = d[PVP_U3];
		units[3] = d[PVP_U4];
		units[4] = d[PVP_U5];
		
		skills[0] = d[PVP_S1];
		skills[1] = d[PVP_S2];
		skills[2] = d[PVP_S3];
		
		lv = -1;
		int.TryParse(d[PVP_LV],out lv);
		DebugManager.instance.setPlayerData(DebugManager.instance.pvpPlayerData,false,d[PVP_ID],d[PVP_HD],d[PVP_BD],d[PVP_WP],d[PVP_RD],units,skills,d[PVP_AI]);


		DebugManager.instance.pvpDefaultHero = d[PVP_ID];

		DebugManager.instance.pvpEquipHead = d[PVP_HD];
		DebugManager.instance.pvpEquipBody = d[PVP_BD];
		DebugManager.instance.pvpEquipWeapon = d[PVP_WP];
		DebugManager.instance.pvpEquipVehicle = d[PVP_RD];
		
		DebugManager.instance.pvpDebugUnitId = units;
		DebugManager.instance.pvpDebugSkillId = skills;
		DebugManager.instance.pvpAi = d[PVP_AI];

		GameManager.me.changeMainPlayer(GameDataManager.instance.heroes[d[ID]], d[ID], GameDataManager.instance.heroes[d[ID]].partsVehicle.parts.resource.ToUpper());
	}


	void preloadForBatchSimulation()
	{
		UnitData ud;
		MonsterData md;

		// 아이디 불러오기.
		string unitId;
		for(int i = BatchStartNum; i <= BatchEndNum; ++i)
		{
			if(csvIndexDic.ContainsKey(i))
			{
				for(int j = U1; j <= U5; ++j)
				{
					unitId = csvList[csvIndexDic[i]][j];
//					Debug.Log("unitid : " + unitId);
					if(string.IsNullOrEmpty( unitId )) continue;
					ud = GameManager.info.unitData[unitId];
					md = GameManager.info.monsterData[ud.resource];
					GameDataManager.instance.addLoadModelData(md);
				}
				
				for(int j = PVP_U1; j <= PVP_U5; ++j)
				{
					unitId = csvList[csvIndexDic[i]][j];
//					Debug.Log("unitid : " + unitId);
					if(string.IsNullOrEmpty( unitId )) continue;
					ud = GameManager.info.unitData[unitId];
					md = GameManager.info.monsterData[ud.resource];
					GameDataManager.instance.addLoadModelData(md);
				}
			}
		}
	}




	public void restartGame()
	{
		GameManager.me.startQuickRestart();
	}















	private static StringBuilder fileLog = new StringBuilder();
	private static StringBuilder str = new StringBuilder();
	public static void log(params object[] data)
	{
		#if UNITY_EDITOR

		int len = str.Length;
		
		str.Remove(0,len);
		
		len = data.Length;		
		
		for(int i = 0; i < len; ++i)
		{
			str.Append(data[i].ToString() + (((i+1)<len)?",":""));
		}

		if(GameManager.me != null && GameManager.me.stageManager != null)
			fileLog.Append(str.ToString() + "\r\n");
		#endif
	}

	public static void saveFileLog()
	{
#if UNITY_EDITOR
		string path = Application.dataPath;
		path = path.Replace("Assets","battleLog/");
		if(Directory.Exists(path) == false) Directory.CreateDirectory(path);
		System.IO.File.WriteAllText(path + System.DateTime.Now.ToString("dd_hh_mm_ss") + ".txt",  fileLog.ToString());
		fileLog.Length = 0;
#endif
	}

}
