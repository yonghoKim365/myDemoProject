using System;
using System.Collections.Generic;
using UnityEngine;

public class HardcodingText
{
	static HardcodingText _instance = null;

	public Dictionary<string, TextData> data = new Dictionary<string, TextData>();

	// 클라이언트 데이터를 받기 전에 표시할 텍스트들은 여기에서 가져다 쓴다.
	// 이를테면 업데이트 문구같은 경우는 클라이언트 데이터 초기화를 하기 전에 뿌리기 때문에.
	// 텍스트 데이터에서 가져올 수가 없다.

	public static HardcodingText instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new HardcodingText();
				_instance.init();
			}

			return _instance;
		}
	}

	void init()
	{
		string source = (Resources.Load(ClientDataLoader.CLIENT_DATA_PATH + "data_hardcodingtext_client") as TextAsset).ToString();
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = ClientDataLoader.getKeyIndexDic((List<object>)jd[ClientDataLoader.NAME]);
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != ClientDataLoader.NAME)
			{
				List<object> list = kv.Value as List<object>;
				TextData td = new TextData();
				td.setData(list, k);
				data[(string)list[k["ID"]]] = td;
				list = null;
			}
		}

		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

	}


}

