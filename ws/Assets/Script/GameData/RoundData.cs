using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class RoundData : BaseData
{
	public string id;
	public string description;

	public class TYPE
	{
		public const string EPIC = "EP";
		public const string CHAMPIONSHIP = "CP";
		public const string CHALLENGE = "CL";
		public const string FRIENDLY = "PVP";
		public const string HELL = "HELL";
	}

	public class MODE
	{
		public const string KILLEMALL = "KILLEMALL";
		public const string SURVIVAL = "SURVIVAL";
		public const string PROTECT = "PROTECT";
		public const string SNIPING = "SNIPING";
		public const string KILLCOUNT = "KILLCOUNT";
		public const string KILLCOUNT2 = "KILLCOUNT2";
		public const string ARRIVE = "ARRIVE";
		public const string DESTROY = "DESTROY";
		public const string GETITEM = "GETITEM";
		public const string PVP = "PVP";
		public const string C_RUN = "C_RUN";
		public const string C_SURVIVAL = "C_SURVIVAL";
		public const string C_HUNT = "C_HUNT";
		public const string DEMO = "DEMO";
		public const string B_TEST = "B_TEST";
		public const string HELL = "HELL";
	}

	public enum MODE_TYPE
	{
		KILLEMALL,
		SURVIVAL,
		PROTECT,
		SNIPING,
		KILLCOUNT,
		KILLCOUNT2,
		ARRIVE,
		DESTROY,
		GETITEM,
		PVP,
		C_RUN,
		C_SURVIVAL,
		C_HUNT,
		DEMO,
		B_TEST,
		HELL
	}

	
	public string mode;
	
	public string cutSceneId;
	public string mapBg;
	public int[] mapId;
	public Xint[] mapStartEndPosX;
	public Xint playerStartPosX = 0;
	public Xint settingTime = -1;
	public string settingAttr = "";
	public string settingAttr2 = null;

	public string[] units = null;

	//===========	
	
	public StageMonsterData[] heroMonsters = null;
	public StageMonsterData[] unitMonsters = null;
	public StageMonsterData[] protectObject = null;
	public StageMonsterData[] destroyObject = null;
	public StageMonsterData[] decoObject = null;
	public StageMonsterData[] blockObject = null;
	
	public StageMonsterData invisibleHeroMonster = null;

	public StageMonsterData challengeData = null;

	public StageMonsterData chaser;

	public string[] killMonsterIds = null;
	public Xint[] killMonsterNum = null;
	public Xint killMonsterCount;
	
	public GetItemData getItemData = null;
	
	public Xint targetPos = 0;
	
	public RoundData ()
	{
		
	}
	
	public Xint targetIndex = 0;
	public Xint targetHpPer = -1000;


	public string[] rewards;

	
	sealed public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		mode = (string)l[k["MODE"]];

		string[] tarr;

		settingTime = -1;

		string tt = l[k["TIME"]].ToString();
		if(string.IsNullOrEmpty(tt) == false && tt.Contains("/"))
		{
			string[] t2 = tt.Split('/');

			if(t2.Length > 0)
			{
				if(t2[0].Length > 0)
				{
					int st = 0;
					int.TryParse(t2[0], out st);
					settingTime = st;
				}
			}

			if(t2.Length == 2)
			{
				settingAttr2 = t2[1];
			}
		}
		else
		{
			Util.parseObject(l[k["TIME"]], out settingTime, true, -1);
		}

		settingAttr = (l[k["MODE_ATTR"]]).ToString();
		targetPos = 0;
		
		switch(mode)
		{

		case MODE.SURVIVAL:

			if(settingAttr2 != null)
			{
				challengeData = new StageMonsterData();
				challengeData.type = StageMonsterData.Type.UNIT;
				challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.
			}
			break;

		case MODE.PROTECT:
			tarr = settingAttr.Split('/');
			protectObject = new StageMonsterData[tarr.Length];
			for(int i = 0; i < tarr.Length; ++i)
			{
				protectObject[i] = new StageMonsterData();
				protectObject[i].type = StageMonsterData.Type.NPC;
				string[] tmp = tarr[i].Split(',');
				protectObject[i].id = tmp[0];
				Util.tryFloatParseToXfloat(tmp[1], out protectObject[i].posX, protectObject[i].posX);
				Util.tryFloatParseToXfloat(tmp[2], out protectObject[i].posY, protectObject[i].posY);
				Util.tryFloatParseToXfloat(tmp[3], out protectObject[i].posZ, protectObject[i].posZ);	

				if(tmp.Length >= 5) Util.tryFloatParseToXfloat(tmp[4], out protectObject[i].angle, protectObject[i].angle);	
				else protectObject[i].angle = -1000;

				if(tmp.Length == 6) protectObject[i].attr = tmp[5];
				else protectObject[i].attr = null;

			}


			if(settingAttr2 != null)
			{
				challengeData = new StageMonsterData();
				challengeData.type = StageMonsterData.Type.UNIT;
				challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.
			}

			break;
		case MODE.SNIPING:

			string str = (l[k["MODE_ATTR"]]).ToString();

			if(str.Contains(","))
			{
				int[] st1 = Util.stringToIntArray(str,',');
				targetIndex = st1[0];
				targetHpPer = st1[1];
			}
			else
			{
				Util.parseObject(l[k["MODE_ATTR"]], out targetIndex, true, 0);
				targetHpPer = -1000;
			}

			break;

		case MODE.KILLCOUNT:
			tarr = settingAttr.Split('/');
			killMonsterCount = tarr.Length;
			killMonsterIds = new string[killMonsterCount];
			killMonsterNum = new Xint[killMonsterCount];
		
			for(int i = 0; i < killMonsterCount; ++i)
			{
				string[] t = tarr[i].Split(',');
				killMonsterIds[i] = t[0];
				Util.tryIntParseToXInt (t[1],out killMonsterNum[i], killMonsterNum[i]);
			}
		
			if(killMonsterIds.Length == 1 && killMonsterIds[0].Equals("TOTAL"))
			{
				mode = MODE.KILLCOUNT2;
			}			
				
			break;

		case MODE.ARRIVE:
//* 목표지점 좌표, 호위NPC ID(옵션), 
//* 추격몬스터 ID(옵션), 이동속도(옵션)
//[목표지점/호위NPCID/추격몬스터ID,이동속도N]			
		
//* 목표지점 좌표, 호위NPC ID(옵션), 
//* 추격몬스터 ID(옵션), 이동속도(옵션)
//[목표지점/호위NPCID/추격몬스터ID,이동속도N]					
			tarr = settingAttr.Split('/');
		
			Util.tryIntParseToXInt (tarr[0], out targetPos, targetPos);
		
			if(tarr.Length >= 2)
			{
				if(string.IsNullOrEmpty(tarr[1]) == false)
				{
					protectObject = new StageMonsterData[1];
					protectObject[0] = new StageMonsterData();
					protectObject[0].type = StageMonsterData.Type.NPC;
					string[] tmp = tarr[1].Split(',');
					protectObject[0].id = tmp[0];
					
					if(tmp.Length >= 2)
					{
						Util.tryFloatParseToXfloat (tmp[1], out protectObject[0].posX, protectObject[0].posX);
						Util.tryFloatParseToXfloat (tmp[2], out protectObject[0].posY, protectObject[0].posY);	
						Util.tryFloatParseToXfloat (tmp[3], out protectObject[0].posZ, protectObject[0].posZ);				
					}

					if(tmp.Length >= 5) Util.tryFloatParseToXfloat (tmp[4], out protectObject[0].angle, protectObject[0].angle);				
					else protectObject[0].angle = -1000;

					if(tmp.Length == 6) Util.tryFloatParseToXfloat (tmp[5], out protectObject[0].checkLine, protectObject[0].checkLine);	
					else protectObject[0].checkLine = 300;
				}
				
				if(tarr.Length == 3)
				{
					string[] c = tarr[2].Split(',');
					chaser = new StageMonsterData();
					chaser.type = StageMonsterData.Type.NPC;
					chaser.id = c[0]; 
					Util.tryFloatParseToXfloat (c[1], out chaser.posX, chaser.posX);
					Util.tryFloatParseToXfloat (c[2], out chaser.posZ, chaser.posZ);
					Util.tryFloatParseToXfloat (c[3], out chaser.speed, chaser.speed);					
				}
			}			
		
			break;

		case MODE.DESTROY:
		
//* 목표 오브젝트 지정 (최소 1개 이상)
//* 추격몬스터 ID(옵션), 이동속도(옵션)
//[ID1,ID2,ID3/추격몬스터ID,이동속도N]			
//			id,px,py/id,px,py|npcid,px,py,speed
			tarr = settingAttr.Split('|');
			if(tarr.Length == 2)
			{
				string[] c = tarr[1].Split(',');
				chaser = new StageMonsterData();
				chaser.type = StageMonsterData.Type.NPC;
				chaser.id = c[0]; 
				Util.tryFloatParseToXfloat (c[1], out chaser.posX, chaser.posX);
				Util.tryFloatParseToXfloat (c[2], out chaser.posZ, chaser.posZ);
				Util.tryFloatParseToXfloat (c[3], out chaser.speed, chaser.speed);					
			}
			
			tarr = tarr[0].Split('/');
			int len = tarr.Length;

			destroyObject = new StageMonsterData[tarr.Length];
			for(int i = 0; i < len; ++i)
			{
				destroyObject[i] = new StageMonsterData();
				destroyObject[i].type = StageMonsterData.Type.NPC;
				string[] tmp = tarr[i].Split(',');
				destroyObject[i].id = tmp[0];
				Util.tryFloatParseToXfloat (tmp[1], out destroyObject[i].posX, destroyObject[i].posX);
				Util.tryFloatParseToXfloat (tmp[2], out destroyObject[i].posY, destroyObject[i].posY);
				Util.tryFloatParseToXfloat (tmp[3], out destroyObject[i].posZ, destroyObject[i].posZ);		

				if(tmp.Length == 5) Util.tryFloatParseToXfloat (tmp[4], out destroyObject[i].angle, destroyObject[i].angle);				
				else destroyObject[i].angle = -1000;
			}
		
			break;

		case MODE.GETITEM:
		
			getItemData = new GetItemData();
		
			tarr = settingAttr.Split('/');
			foreach(string ta in tarr)
			{
				string[] tl = ta.Split(',');
				getItemData.needCount[tl[0]] = 0;
				int needCount = 0;
				int.TryParse(tl[1], out needCount);
				getItemData.needCount[tl[0]] = needCount;
				getItemData.createChance[tl[0]] = new Dictionary<string, Xint>(StringComparer.Ordinal);
			
				int monLen = (tl.Length-2)/2;
				for(int i = 0; i < monLen; ++i)
				{
					string unitId = tl[2+(i*2)];
					getItemData.createChance[tl[0]][unitId] = 0;
					needCount = 0;
					int.TryParse( tl[2+(i*2)+1], out needCount);
					getItemData.createChance[tl[0]][unitId] = needCount;
				}
			}
		
			getItemData.itemIds = new string[getItemData.needCount.Keys.Count];
			getItemData.needCount.Keys.CopyTo(getItemData.itemIds,0);
			getItemData.itemCount = getItemData.itemIds.Length;


			if(settingAttr2 != null)
			{
				challengeData = new StageMonsterData();
				challengeData.type = StageMonsterData.Type.UNIT;
				challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.
			}

			//GameManager.me.uiManager.uiPlay.lbRoundInfo.text = getItemData.itemIds.ToString() + "획득";

			break;


		case MODE.C_RUN:
			tarr = settingAttr.Split(',');
			challengeData = new StageMonsterData();
			challengeData.type = StageMonsterData.Type.NPC;
			challengeData.id = tarr[0];
			units = ((string)l[k["MON_HERO_UNIT"]]).Split(',');
			break;

		case MODE.C_SURVIVAL:
			challengeData = new StageMonsterData();
			challengeData.type = StageMonsterData.Type.UNIT;
			challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.
			units = ((string)l[k["MON_HERO_UNIT"]]).Split(',');
			break;

		case MODE.C_HUNT:
			challengeData = new StageMonsterData();
			challengeData.type = StageMonsterData.Type.UNIT;
			challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.
			units = ((string)l[k["MON_HERO_UNIT"]]).Split(',');
			break;


		case MODE.B_TEST:
			challengeData = new StageMonsterData();
			challengeData.type = StageMonsterData.Type.UNIT;
			challengeData.id = "UN5"; // 어차피 화면에는 안보이게 할 거다. 더미나 마찬가지임.

			List<string> testUnits = new List<string>();

			string[] fuck = settingAttr.Split('/');
			for(int i = 0; i < fuck.Length; ++i)
			{
				string[] shit = fuck[i].Split(',');
				for(int j = 2; j < shit.Length; ++j)
				{
					if(testUnits.Contains(shit[j]) == false) testUnits.Add(shit[j]);
				}
			}

			units = testUnits.ToArray();
			break;
		}

		string tr = ((string)l[k["REWARD"]]).Trim();

		if(string.IsNullOrEmpty(tr)) rewards = null;
		else rewards = tr.Split('/');

		description = (string)l[k["DESCRIPTION"]];

		description = description.Replace("\\n","\n");

		cutSceneId = (string)l[k["SCENE_ID"]];
		mapBg = (string)l[k["MAP_BG"]];

		mapId = Util.stringToIntArray( l[k["MAP_ID"]].ToString(), '/');

		if(mapId == null) mapId = new int[1]{1};


		mapStartEndPosX = Util.stringToXIntArray((string)l[k["MAP_SIZE"]], ',');
		Util.parseObject( l[k["PLAYER_START_POINT"]] , out playerStartPosX, true,  0);
		parseMonsterData((string)l[k["MON_HERO"]],(string)l[k["DEFAULT_MON_UNIT"]],(string)l[k["MON_HERO_UNIT"]],(string)l[k["MON_HERO_SKILL"]],(string)l[k["MON_HERO_AI"]],(string)l[k["DECO"]],(string)l[k["OBJECT"]]);


//		Debug.LogError(id + " " + cutSceneId);

	}




	//parseMonsterData((string)l[k["MON_HERO"]],(string)l[k["DEFAULT_MON_UNIT"]],(string)l[k["MON_HERO_UNIT"]],(string)l[k["MON_HERO_SKILL"]],(string)l[k["MON_HERO_AI"]] );
	void parseMonsterData(string heroStr, string defaultMonUnit, string unitStr, string skillStr, string aiStr, string decoStr, string blockObjectStr)
	{
//* 해당 라운드에 등장하는 언데드히어로 지정 및 초기 위치 설정
//* (언데드히어로ID, 위치,보유SP%,보유MP%) 형식으로 지정 - HP는 무조건 100%, SP/MP는 초반 난이도 조절을 위해 필요
//* (언데드A/언데드B/… ) 형식으로 복수개 지정 가능
//예> 22,30,100,100/23,80,50,50	
		int i;
		
		string[] heroes;
		
		if(string.IsNullOrEmpty(heroStr))
		{
			heroes = new string[0];
		}else heroes = heroStr.Split('/');
		
		
		string[] heroUnit = null;
		
		if(unitStr.Trim().Length > 0)
		{	
			heroUnit = unitStr.Split('/');	
		}
		
		string[] heroSkill = skillStr.Split('/');
		string[] heroAi = aiStr.Split('/');
		
		heroMonsters = new StageMonsterData[heroes.Length];
		
		for(i = 0; i < heroes.Length; ++i)
		{
			heroMonsters[i] = new StageMonsterData();
			heroMonsters[i].type = StageMonsterData.Type.HERO;
			string[] tmp = heroes[i].Split(',');
			heroMonsters[i].id = tmp[0];
			Util.tryFloatParseToXfloat(tmp[1],out heroMonsters[i].posX, heroMonsters[i].posX);
			
			heroMonsters[i].hpPercent = 1.0f;
			
			Util.tryFloatParseToXfloat(tmp[2],out heroMonsters[i].spPercent, heroMonsters[i].spPercent);
			heroMonsters[i].spPercent *= 0.01f;
			
			Util.tryFloatParseToXfloat(tmp[3],out heroMonsters[i].mpPercent, heroMonsters[i].mpPercent);
			heroMonsters[i].mpPercent *= 0.01f;
			
			
			if(tmp.Length >= 5)
			{
				heroMonsters[i].attr = "H";
			}else heroMonsters[i].attr = null;
			
			
			if(heroUnit == null || i >= heroUnit.Length  || string.IsNullOrEmpty(heroUnit[i]))
			{
				heroMonsters[i].units = new string[0];
			}
			else
			{
				heroMonsters[i].units =  heroUnit[i].Split(',');//heroUnit[i].Split(',');	
			}
			
			if(heroSkill == null || i >= heroSkill.Length  || string.IsNullOrEmpty(heroSkill[i]))
			{
				heroMonsters[i].skills = new string[0];
			}
			else
			{
				heroMonsters[i].skills = heroSkill[i].Split(',') ;	
			}
			
			
			if(heroAi == null || i >= heroAi.Length  || string.IsNullOrEmpty(heroAi[i]))
			{
				heroMonsters[i].ai = new string[0];
			}
			else
			{
				heroMonsters[i].ai = heroAi[i].Split(',');	
			}
		}
		
		
		if(defaultMonUnit.Equals(String.Empty) == false)
		{
			string[] units = defaultMonUnit.Split('/');
			unitMonsters = new StageMonsterData[units.Length];
			
			for(i = 0; i < units.Length; ++i)
			{
				unitMonsters[i] = new StageMonsterData();
				unitMonsters[i].type = StageMonsterData.Type.UNIT;
				string[] tmp = units[i].Split(',');
				unitMonsters[i].id = tmp[0];
				Util.tryFloatParseToXfloat(tmp[1], out unitMonsters[i].posX, unitMonsters[i].posX);
				Util.tryFloatParseToXfloat(tmp[2], out unitMonsters[i].posZ, unitMonsters[i].posZ);
				
				unitMonsters[i].checkLine = -1000.0f;
				unitMonsters[i].attr = null;
				unitMonsters[i].angle = -1;				
				
				switch(tmp.Length)
				{
				case 4:
					Util.tryFloatParseToXfloat(tmp[3], out unitMonsters[i].checkLine, unitMonsters[i].checkLine);
					break;
				case 5:
					Util.tryFloatParseToXfloat(tmp[3], out unitMonsters[i].checkLine, unitMonsters[i].checkLine);
					Util.tryFloatParseToXfloat(tmp[4], out unitMonsters[i].angle, unitMonsters[i].angle);
					break;
				case 6:
					Util.tryFloatParseToXfloat(tmp[3], out unitMonsters[i].checkLine, unitMonsters[i].checkLine);
					Util.tryFloatParseToXfloat(tmp[4], out unitMonsters[i].angle, unitMonsters[i].angle);
					unitMonsters[i].attr = ((string)tmp[5]).ToUpper();
					break;
				}
			}		
		}
		
		
		if(decoStr.Equals(String.Empty) == false)
		{
			string[] decos = decoStr.Split('/');
			decoObject = new StageMonsterData[decos.Length];
			
			for(i = 0; i < decos.Length; ++i)
			{
				decoObject[i] = new StageMonsterData();
				decoObject[i].type = StageMonsterData.Type.NPC;
				string[] tmp = decos[i].Split(',');
				decoObject[i].id = tmp[0];
				Util.tryFloatParseToXfloat(tmp[1], out decoObject[i].posX, decoObject[i].posX);
				Util.tryFloatParseToXfloat(tmp[2], out decoObject[i].posY, decoObject[i].posY);	
				Util.tryFloatParseToXfloat(tmp[3], out decoObject[i].posZ, decoObject[i].posZ);	
				if(tmp.Length >= 5)
				{
					Util.tryFloatParseToXfloat(tmp[4], out decoObject[i].angle, decoObject[i].angle);
				}
				else decoObject[i].angle = -1000.0f;

				if(tmp.Length == 6) decoObject[i].attr = tmp[5];
				else decoObject[i].attr = null;
				
			}		
		}


		if(blockObjectStr.Equals(String.Empty) == false)
		{
//			사용 형식은 ‘NPCID1,X좌표,Z좌표,N/NPCID2,X좌표,Z좌표,N….’
//				<N>
//					- 0 : 몬스터 편 오브젝트 / 적은 오브젝트를 무시하고 통과
//					- 1 : 몬스터 편 오브젝트 / 적은 오브젝트를 지나치지 못함 
//					(이동 중 오브젝트 앞에서 멈추며, 오브젝트 파괴 시 이동)
//					- 2 : 유저 편 오브젝트 / 유저는 오브젝트를 무시하고 통과
//					- 3 : 유저 편 오브젝트 / 유저는 오브젝트를 지나치지 못함 
//					(이동 중 오브젝트 앞에서 멈추며, 오브젝트 파괴 시 이동)

			string[] blocks = blockObjectStr.Split('/');
			blockObject = new StageMonsterData[blocks.Length];
			
			for(i = 0; i < blockObject.Length; ++i)
			{
				blockObject[i] = new StageMonsterData();
				blockObject[i].type = StageMonsterData.Type.NPC;

				string[] tmp = blocks[i].Split(',');
				blockObject[i].id = tmp[0];
				Util.tryFloatParseToXfloat(tmp[1], out blockObject[i].posX, blockObject[i].posX);
				Util.tryFloatParseToXfloat(tmp[2], out blockObject[i].posY, blockObject[i].posY);	
				Util.tryFloatParseToXfloat(tmp[3], out blockObject[i].posZ, blockObject[i].posZ);	

				if(tmp.Length >= 5)
				{
					float t4 = 0;
					float.TryParse(tmp[4], out t4);
					blockObject[i].angle = t4;
				}
				else blockObject[i].angle = -1000.0f;

//				Debug.Log(id);
				blockObject[i].attr = tmp[5];
			}		
		}



	}
}




sealed public class StageMonsterData
{
	public enum Type{ HERO, UNIT , CHASER, NPC};
	
	public Type type;
	
	public string id;
	public Xfloat posX = 0;
	public Xfloat posY = 0;
	public Xfloat posZ = 0;
	public Xfloat speed = 0.0f;
	public Xfloat hpPercent = 1.0f;
	public Xfloat spPercent = 0.0f;
	public Xfloat mpPercent = 0.0f;
	
	public Xfloat checkLine = -1000.0f;
	public string attr = null;
	
	public Xfloat angle = 0.0f;
	
	public string[] units;
	public string[] skills;
	public string[] ai;
	
	public StageMonsterData()
	{
	}
}



public class GetItemData
{
	public Dictionary<string, Xint> needCount = new Dictionary<string, Xint>(StringComparer.Ordinal);
	public Dictionary<string, Dictionary<string, Xint>> createChance = new Dictionary<string, Dictionary<string, Xint>>(StringComparer.Ordinal);
	public string[] itemIds;
	public Xint itemCount = 0;
	int i;
	
	public void createItem(string unitId, IVector3 position)
	{
		for(i = 0; i < itemCount; ++i)
		{
			if(createChance[itemIds[i]].ContainsKey(unitId))
			{
				if(createChance[itemIds[i]][unitId] >  GameManager.inGameRandom.Range(0,100) )
				{
					position.x -= 100;

					if(position.x < StageManager.mapStartPosX + 100)
					{
						position.x = StageManager.mapStartPosX + 100;
					}

					GameManager.me.mapManager.addMapObjectToStage(itemIds[i],position).dropItemEffect.start(position);
				}
			}
		}
	}

	public bool isCreateItemMonster(string unitId)
	{
		for(i = 0; i < itemCount; ++i)
		{
			if(createChance[itemIds[i]].ContainsKey(unitId))
			{
				return true;
			}
		}

		return false;
	}
	
}




public struct ChallengeInfinityRun
{
	public int[] rankData;
	public float checkDistance;
	public float summonDelay;
	public string[] summonUnits;

}


public struct ChallengeInfinitySurvival
{
	public int[] rankData;
	public float hitRadius;
	public string[] summonUnits;

	public float[] constValue;
}


public struct ChallengeInfinityHunt
{
	public int[] rankData;
	public float[] checkLevelDistance;
	public float[] summonDelays;
	public string[] summonUnits;
}
