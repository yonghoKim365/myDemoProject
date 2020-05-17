using System;
using System.Collections.Generic;

sealed public class TestModeData : BaseData
{

	public string id;
	public string hero;
	public int level;
	public string head;
	public string body;
	public string weapon;
	public string vehicle;
	public string u1;
	public string u2;
	public string u3;
	public string u4;
	public string u5;
	public string s1;
	public string s2;
	public string s3;

	public P_Sigong sigongData = null;

	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];

		hero = (string)l[k["HERO"]];

		Util.parseObject(l[k["LV"]], out level, true, 0);
		
		head = (string)l[k["HEAD"]];
		body = (string)l[k["BODY"]];
		weapon = (string)l[k["WEAPON"]];
		vehicle = (string)l[k["VEHICLE"]];

		u1 = (string)l[k["U1"]];
		u2 = (string)l[k["U2"]];
		u3 = (string)l[k["U3"]];
		u4 = (string)l[k["U4"]];
		u5 = (string)l[k["U5"]];
		s1 = (string)l[k["S1"]];
		s2 = (string)l[k["S2"]];
		s3 = (string)l[k["S3"]];
	}



	public static GamePlayerData getTestModePlayerData(string id, bool isPlayerSide, string ai, string handicapType = null)
	{
		TestModeData tmd = GameManager.info.testModeData[id];
		
		GamePlayerData testModeData = new GamePlayerData(tmd.hero);
		
		string[] u = new string[5];

		if(string.IsNullOrEmpty(handicapType) == false && ( handicapType == WSDefine.HANDICAP_TYPE_UNIT || handicapType == WSDefine.HANDICAP_TYPE_BOTH ))
		{
			u[0] = string.Empty;
			u[1] = string.Empty;
			u[2] = string.Empty;
			u[3] = string.Empty;
			u[4] = string.Empty;
		}
		else
		{
			u[0] = tmd.u1;
			u[1] = tmd.u2;
			u[2] = tmd.u3;
			u[3] = tmd.u4;
			u[4] = tmd.u5;
		}


		string[] s = new string[3];

		if(string.IsNullOrEmpty(handicapType) == false && ( handicapType == WSDefine.HANDICAP_TYPE_SKILL || handicapType == WSDefine.HANDICAP_TYPE_BOTH ))
		{
			s[0] = string.Empty;
			s[1] = string.Empty;
			s[2] = string.Empty;
		}
		else
		{
			s[0] = tmd.s1;
			s[1] = tmd.s2;
			s[2] = tmd.s3;
		}

		DebugManager.instance.setPlayerData(testModeData,isPlayerSide,tmd.hero,tmd.head,tmd.body,tmd.weapon,tmd.vehicle,u,s, ai);

		return testModeData;
	}

}