using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System;

public class ReplayTestManager : MonoBehaviour 
{
	public int codePatchVer = 0;

	public TextAsset replayDecData;

	//public TextAsset[] replayPlayData;
	public string[] replayPlayData;

	string replayData;

	public string defenderId;
	public int roundNum = 0;

	void Awake()
	{
#if !UNITY_EDITOR
		replayDecData = null;
		replayData = null;
		replayPlayData = null;
#endif
	}

	public void init()
	{
		replayData = replayPlayData[roundNum].ToString();

		parseReplay();
	}


	public void parseReplay()
	{
		if(string.IsNullOrEmpty(replayData) || replayData.Length < 10 || ReplayManager.instance.convertServerReplayData(replayData) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "리플레이 데이터가 유효하지 않습니다.");
			return;
		}
		/*
		else if( VersionData.isCompatibilityVersion( GameManager.replayManager.replayGameVersion , false) == false )
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "호환되지 않는 리플레이 데이터입니다.\n(ver."+ GameManager.replayManager.replayGameVersion + ")");
			return;
		}
		*/
	}


	public void play()
	{
		if(codePatchVer > 0)
		{
			VersionData.codePatchVer = new int[]{codePatchVer};
		}

		Dictionary<string, object> dic = MiniJSON.Json.Deserialize(replayDecData.ToString()) as Dictionary<string, object>;

		Dictionary<string, object> dic2 = dic[defenderId+"_R"+roundNum] as Dictionary<string, object>;

		ReplayDecData p = JsonReader.Deserialize<ReplayDecData>(MiniJSON.Json.Serialize(dic2));

		GameManager.me.stageManager.setNowRound(GameManager.info.roundData["PVP"],GameType.Mode.Championship);
		
		ReplayManager.instance.replaySeed = Convert.ToInt32(p.aiId);

		GameDataManager.replayAttackerData = new GamePlayerData(p.hero.name);
		
		string[] u = new string[5];
		u[0] = p.selUnit.U1;
		u[1] = p.selUnit.U2;
		u[2] = p.selUnit.U3;
		u[3] = p.selUnit.U4;
		u[4] = p.selUnit.U5;
		
		string[] s = new string[3];
		s[0] = p.selSkill.S1;
		s[1] = p.selSkill.S2;
		s[2] = p.selSkill.S3;

		DebugManager.instance.setPlayerData(GameDataManager.replayAttackerData,true,p.hero.name,p.hero.selEqts.HD,p.hero.selEqts.BD,p.hero.selEqts.WP,p.hero.selEqts.RD,u,s,DebugManager.instance.pvpAi);
		
		string[] u2 = new string[5];
		u2[0] = p.eSelUnit.U1;
		u2[1] = p.eSelUnit.U2;
		u2[2] = p.eSelUnit.U3;
		u2[3] = p.eSelUnit.U4;
		u2[4] = p.eSelUnit.U5;
		
		string[] s2 = new string[3];
		s2[0] = p.eSelSkill.S1;
		s2[1] = p.eSelSkill.S2;
		s2[2] = p.eSelSkill.S3;
		
		
		DebugManager.instance.pvpPlayerData = new GamePlayerData(p.eHero.name);
		DebugManager.instance.setPlayerData(DebugManager.instance.pvpPlayerData,false,p.eHero.name, p.eHero.selEqts.HD,p.eHero.selEqts.BD,p.eHero.selEqts.WP,p.eHero.selEqts.RD,u2,s2,DebugManager.instance.pvpAi);
		
		GameManager.me.recordMode = GameManager.RecordMode.replay;
		GameManager.me.uiManager.showLoading();

		GameManager.me.startGame(0.5f);
	}
}


public class ReplayDecData
{
	public string aiId;

	public DecHeroData hero;
	public DecUnitData selUnit;
	public DecSkillData selSkill;


	public DecHeroData eHero;
	public DecUnitData eSelUnit;
	public DecSkillData eSelSkill;

}

public class DecHeroData
{
	public string name;
	public DecEquipData selEqts;
}

public class DecEquipData
{
	public string HD;
	public string BD;
	public string WP;
	public string RD;
}

public class DecUnitData
{
	public string U1;
	public string U2;
	public string U3;
	public string U4;
	public string U5;
}

public class DecSkillData
{
	public string S1;
	public string S2;
	public string S3;
}




