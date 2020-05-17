using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Icon
{
	[Serializable]
	public class IconInfo
	{
		public uint Id;
		public string Icon;
	}
	public Dictionary<uint, IconInfo>  IconInfoDic = new Dictionary<uint, IconInfo> ();

	public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/Icon_Icon", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject Icon = new JSONObject(strSrc);

			for (int i = 0; i < Icon.list.Count; i++)
			{
				IconInfo tmpInfo = new IconInfo();
				tmpInfo.Id = (uint)Icon[i]["Id_ui"].n;
				tmpInfo.Icon = Icon[i]["Icon_c"].str;

			IconInfoDic.Add(tmpInfo.Id, tmpInfo);
			}
		}
	}

	public void SerializeData()
	{
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/IconInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, IconInfoDic);
			fs.Close();
		}

	}

	public void DeserializeData()
	{
//		IconInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, IconInfo> >("IconInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, IconInfo> >("IconInfo", (data) => { IconInfoDic = data; });



	}
}
