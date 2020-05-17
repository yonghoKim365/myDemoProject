using System;
using System.Collections.Generic;

sealed public class LoadingTipData
{
	public string id;

	public enum Type
	{
		All, Epic, Challenge, Championship, Friendly, Hell
	}

	public Type type = Type.All;

	public int[] actRange;


	public string text;


	public void setData(List<object> l, Dictionary<string, int> k)
	{
		int t = 0;

		Util.parseObject(l[k["BATTLE_TYPE"]], out t, 0);

		switch(t)
		{
		case 0: type = Type.All;
			break;
		case 1: type = Type.Epic;
			break;
		case 2: type = Type.Challenge;
			break;
		case 3: type = Type.Championship;
			break;
		case 4: type = Type.Friendly;
			break;
		case 5: type = Type.Hell;
			break;
		}

		actRange = Util.stringToIntArray((string)l[k["ACT"]],',');
		text = (l[k["TEXT"]]).ToString().Replace("\\n","\n");
	}


	public bool canUseThisTip()
	{
		switch(type)
		{
		case Type.All:
			break;
		case Type.Epic:
			if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Epic) return false;
			break;
//		case Type.Challenge:
//			if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Challenge) return false;
//			break;
		case Type.Championship:
			if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Championship) return false;
			break;
		case Type.Friendly:
			if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Friendly) return false;
			break;
		case Type.Hell:
			if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Hell) return false;
			break;
		}

		return (GameDataManager.instance.maxAct >= actRange[0] && GameDataManager.instance.maxAct <= actRange[1]);
	}


	static List<LoadingTipData> _list = new List<LoadingTipData>();
	public static string getTip()
	{
		string tip = "";
		_list.Clear();

		for(int i = GameManager.info.tipData.Count -1; i >= 0; --i)
		{
			if( GameManager.info.tipData[i].canUseThisTip() )
			{
				_list.Add(GameManager.info.tipData[i]);
			}
		}

		if(_list.Count > 0)
		{
			tip = _list[ UnityEngine.Random.Range(0,_list.Count) ].text;
		}

		_list.Clear();

		return tip;
	}
}
