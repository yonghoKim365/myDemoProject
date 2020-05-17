using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class GatchaReward
{
    [Serializable]
    public class GatchaInfo
    {
        public uint Idx;
        uint _GatchaId;
        public uint GatchaId
        {
            set { _GatchaId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GatchaId); }
        }
        uint _GroupIdx;
        public uint GroupIdx
        {
            set { _GroupIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GroupIdx); }
        }
    }

    [Serializable]
    public class RewardInfo
    {
        //public uint LogIdx;
        uint _GatchIdx;
        public uint GatchIdx
        {
            set { _GatchIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GatchIdx); }
        }
        byte _ClassType;
        public byte ClassType
        {
            set { _ClassType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClassType); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        uint _ItemIdx;
        public uint ItemIdx
        {
            set { _ItemIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx); }
        }
        public uint ItemIdxRate;
        uint _ShardMin;
        public uint ShardMin
        {
            set { _ShardMin = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShardMin); }
        }
        uint _ShardMax;
        public uint ShardMax
        {
            set { _ShardMax = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShardMax); }
        }
    }

    [Serializable]
    public class FixedRewardInfo
    {
        public uint ID;
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
        byte _ClassType;
        public byte ClassType
        {
            set { _ClassType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClassType); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        uint _ItemId;
        public uint ItemId
        {
            set { _ItemId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemId); }
        }
        uint _ItemCount;
        public uint ItemCount
        {
            set { _ItemCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemCount); }
        }
    }

    public List<GatchaInfo> GatchaInfoList = new List<GatchaInfo>();
    public List<RewardInfo> RewardInfoList = new List<RewardInfo>();
    public List<FixedRewardInfo> FixedRewardInfoList = new List<FixedRewardInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/GatchaReward_Gatcha", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Gatcha = new JSONObject(strSrc);

            for (int i = 0; i < Gatcha.list.Count; i++)
            {
                GatchaInfo tmpInfo = new GatchaInfo();
                tmpInfo.Idx = (uint)Gatcha[i]["Idx_ui"].n;
                tmpInfo.GatchaId = (uint)Gatcha[i]["GatchaId_ui"].n;
                tmpInfo.GroupIdx = (uint)Gatcha[i]["GroupIdx_ui"].n;

                GatchaInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/GatchaReward_Reward", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Reward = new JSONObject(strSrc);

            for (int i = 0; i < Reward.list.Count; i++)
            {
                RewardInfo tmpInfo = new RewardInfo();
                //tmpInfo.LogIdx = (uint)Reward[i]["LogIdx_ui"].n;
                tmpInfo.GatchIdx = (uint)Reward[i]["GatchIdx_ui"].n;
                tmpInfo.ClassType = (byte)Reward[i]["ClassType_b"].n;
                tmpInfo.Type = (byte)Reward[i]["Type_b"].n;
                tmpInfo.ItemIdx = (uint)Reward[i]["ItemIdx_ui"].n;
				tmpInfo.ItemIdxRate = (uint)Reward[i]["ItemIdxRate_ui"].n;
                tmpInfo.ShardMin = (uint)Reward[i]["ShardMin_ui"].n;
                tmpInfo.ShardMax = (uint)Reward[i]["ShardMax_ui"].n;

                RewardInfoList.Add(tmpInfo);
            }

        }
        {
            TextAsset data = Resources.Load("TestJson/GatchaReward_FixedReward", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject FixedReward = new JSONObject(strSrc);

            for (int i = 0; i < FixedReward.list.Count; i++)
            {
                FixedRewardInfo tmpInfo = new FixedRewardInfo();
                tmpInfo.ID = (uint)FixedReward[i]["ID_ui"].n;
                tmpInfo.RewardId = (uint)FixedReward[i]["RewardId_ui"].n;
                tmpInfo.ClassType = (byte)FixedReward[i]["ClassType_b"].n;
                tmpInfo.Type = (byte)FixedReward[i]["Type_b"].n;
                tmpInfo.ItemId = (uint)FixedReward[i]["ItemId_ui"].n;
                tmpInfo.ItemCount = (uint)FixedReward[i]["ItemCount_ui"].n;

                FixedRewardInfoList.Add(tmpInfo);
            }
        }


    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GatchaInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GatchaInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/RewardInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, RewardInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/FixedRewardInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, FixedRewardInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
		_LowDataMgr.instance.DeserializeData<List<GatchaInfo>>("GatchaInfo", (data) => { GatchaInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<FixedRewardInfo>>("FixedRewardInfo", (data) => { FixedRewardInfoList = data; });

		_LowDataMgr.instance.DeserializeData<List<RewardInfo>>("RewardInfo", (data) => { RewardInfoList = data; });
    }

	// use asset bundle
	public void DeserializeData2()
	{
		_LowDataMgr.instance.DeserializeData<List<RewardInfo>>("RewardInfo", (data) => { RewardInfoList = data; });
	}

	public void DeserializeDataAsync()
	{
		//_LowDataMgr.instance.DeserializeDataAsync<List<RewardInfo>>("RewardInfo", (data) => { RewardInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<RewardInfo>>("RewardInfo", (data) => { RewardInfoList = data; });
	}
}
