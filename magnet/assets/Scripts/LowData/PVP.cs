using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PVP
{
	[Serializable]
	public class PVPAutoRewardInfo
	{
		public uint Id;
		uint _RankMin;
		public uint RankMin {
			set { _RankMin = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_RankMin); }
		}
		uint _RankMax;
		public uint RankMax {
			set { _RankMax = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_RankMax); }
		}
		uint _RewardValue;
		public uint RewardValue {
			set { _RewardValue = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_RewardValue); }
		}
	}
	public List<PVPAutoRewardInfo>  PVPAutoRewardInfoList = new List<PVPAutoRewardInfo> ();

	public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/PVP_PVPAutoReward", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject PVPAutoReward = new JSONObject(strSrc);

			for (int i = 0; i < PVPAutoReward.list.Count; i++)
			{
				PVPAutoRewardInfo tmpInfo = new PVPAutoRewardInfo();
				tmpInfo.Id = (uint)PVPAutoReward[i]["Id_ui"].n;
				tmpInfo.RankMin = (uint)PVPAutoReward[i]["RankMin_ui"].n;
				tmpInfo.RankMax = (uint)PVPAutoReward[i]["RankMax_ui"].n;
				tmpInfo.RewardValue = (uint)PVPAutoReward[i]["RewardValue_ui"].n;

			PVPAutoRewardInfoList.Add(tmpInfo);
			}
		}
	}

	public void SerializeData()
	{
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/PVPAutoRewardInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, PVPAutoRewardInfoList);
			fs.Close();
		}

	}

	public void DeserializeData()
	{
		//PVPAutoRewardInfoList = _LowDataMgr.instance.DeserializeData<List<PVPAutoRewardInfo>>("PVPAutoRewardInfo");

		_LowDataMgr.instance.DeserializeData<List<PVPAutoRewardInfo>>("PVPAutoRewardInfo", (data) => { 
			PVPAutoRewardInfoList = data; 
		});



	}
}
