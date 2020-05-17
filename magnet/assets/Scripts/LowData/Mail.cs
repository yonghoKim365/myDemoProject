using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Mail
{
	[Serializable]
	public class MailInfo
	{
		public ushort index;
		byte _type;
		public byte type {
			set { _type = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_type); }
		}
		public JsonCustomData itemType;
		public JsonCustomData itemLink;
		public JsonCustomData value;
		uint _acceptExpireTime;
		public uint acceptExpireTime {
			set { _acceptExpireTime = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_acceptExpireTime); }
		}
		public string NameId;
		public string DescriptionId;
	}
	public Dictionary<ushort, MailInfo>  MailInfoDic = new Dictionary<ushort, MailInfo> ();

	public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/Mail_Mail", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject Mail = new JSONObject(strSrc);

			for (int i = 0; i < Mail.list.Count; i++)
			{
				MailInfo tmpInfo = new MailInfo();
				tmpInfo.index = (ushort)Mail[i]["index_us"].n;
				tmpInfo.type = (byte)Mail[i]["type_b"].n;
				tmpInfo.itemType = new JsonCustomData(Mail[i]["itemType_j"].ToString());
				tmpInfo.itemLink = new JsonCustomData(Mail[i]["itemLink_j"].ToString());
				tmpInfo.value = new JsonCustomData(Mail[i]["value_j"].ToString());
				tmpInfo.acceptExpireTime = (uint)Mail[i]["acceptExpireTime_ui"].n;
				tmpInfo.NameId = Mail[i]["NameId_c"].str;
				tmpInfo.DescriptionId = Mail[i]["DescriptionId_c"].str;

			MailInfoDic.Add(tmpInfo.index, tmpInfo);
			}
		}
	}

	public void SerializeData()
	{
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/MailInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, MailInfoDic);
			fs.Close();
		}

	}

	public void DeserializeData()
	{
//		MailInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<ushort, MailInfo> >("MailInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<ushort, MailInfo> >("MailInfo", (data) => { MailInfoDic = data; });

	}
}
