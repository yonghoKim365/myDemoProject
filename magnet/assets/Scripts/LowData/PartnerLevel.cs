using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PartnerLevel
{
	[Serializable]
	public class PartnerLevelInfo
	{
		public byte Level;
		uint _Exp;
		public uint Exp {
			set { _Exp = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_Exp); }
		}
		uint _Expoverlab;
		public uint Expoverlab {
			set { _Expoverlab = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_Expoverlab); }
		}
	}
	public Dictionary<byte, PartnerLevelInfo>  PartnerLevelInfoDic = new Dictionary<byte, PartnerLevelInfo> ();

	public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/PartnerLevel_PartnerLevel", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject PartnerLevel = new JSONObject(strSrc);

			for (int i = 0; i < PartnerLevel.list.Count; i++)
			{
				PartnerLevelInfo tmpInfo = new PartnerLevelInfo();
				tmpInfo.Level = (byte)PartnerLevel[i]["Level_b"].n;
				tmpInfo.Exp = (uint)PartnerLevel[i]["Exp_ui"].n;
				tmpInfo.Expoverlab = (uint)PartnerLevel[i]["Expoverlab_ui"].n;

			PartnerLevelInfoDic.Add(tmpInfo.Level, tmpInfo);
			}
		}
	}

	public void SerializeData()
	{
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/PartnerLevelInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, PartnerLevelInfoDic);
			fs.Close();
		}

	}

	public void DeserializeData()
	{
//		PartnerLevelInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<byte, PartnerLevelInfo> >("PartnerLevelInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<byte, PartnerLevelInfo> >("PartnerLevelInfo", (data) => { PartnerLevelInfoDic  = data; });


	}
}
