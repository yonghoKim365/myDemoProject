using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Title
{
	[Serializable]
	public class TitleInfo
	{
		public uint Id;
		byte _Type;
		public byte Type {
			set { _Type = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_Type); }
		}
		uint _TitleName;
		public uint TitleName {
			set { _TitleName = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_TitleName); }
		}
		uint _LinkAchievement;
		public uint LinkAchievement {
			set { _LinkAchievement = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_LinkAchievement); }
		}
	}
	public Dictionary<uint, TitleInfo>  TitleInfoDic = new Dictionary<uint, TitleInfo> ();

	public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/Title_Title", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject Title = new JSONObject(strSrc);

			for (int i = 0; i < Title.list.Count; i++)
			{
				TitleInfo tmpInfo = new TitleInfo();
				tmpInfo.Id = (uint)Title[i]["Id_ui"].n;
				tmpInfo.Type = (byte)Title[i]["Type_b"].n;
				tmpInfo.TitleName = (uint)Title[i]["TitleName_ui"].n;
				tmpInfo.LinkAchievement = (uint)Title[i]["LinkAchievement_ui"].n;

			TitleInfoDic.Add(tmpInfo.Id, tmpInfo);
			}
		}
	}

	public void SerializeData()
	{
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/TitleInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, TitleInfoDic);
			fs.Close();
		}

	}

	public void DeserializeData()
	{
//		TitleInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, TitleInfo> >("TitleInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, TitleInfo> >("TitleInfo", (data) => { TitleInfoDic = data; });



	}
}
