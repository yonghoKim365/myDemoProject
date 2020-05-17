using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ActiveReward
{
    [Serializable]
    public class ActivePointInfo
    {
        public uint Id;
        uint _NameId;
        public uint NameId
        {
            set { _NameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NameId); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        uint _PointValue;
        public uint PointValue
        {
            set { _PointValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_PointValue); }
        }
        uint _MaxCount;
        public uint MaxCount
        {
            set { _MaxCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MaxCount); }
        }
    }
    [Serializable]
    public class ActiveRewardInfo
    {
        public uint Id;
        byte _RewardRank;
        public byte RewardRank
        {
            set { _RewardRank = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RewardRank); }
        }
        uint _PointValue;
        public uint PointValue
        {
            set { _PointValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_PointValue); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
    }
    public List<ActiveRewardInfo> ActiveRewardInfoList = new List<ActiveRewardInfo>();
    public Dictionary<uint, ActivePointInfo> ActivePointInfoDic = new Dictionary<uint, ActivePointInfo>();
    
    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/ActiveReward_ActivePoint", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ActivePoint = new JSONObject(strSrc);

            for (int i = 0; i < ActivePoint.list.Count; i++)
            {
                ActivePointInfo tmpInfo = new ActivePointInfo();
                tmpInfo.Id = (uint)ActivePoint[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)ActivePoint[i]["NameId_ui"].n;
                tmpInfo.Type = (byte)ActivePoint[i]["Type_b"].n;
                tmpInfo.PointValue = (uint)ActivePoint[i]["PointValue_ui"].n;
                tmpInfo.MaxCount = (uint)ActivePoint[i]["MaxCount_ui"].n;

                ActivePointInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/ActiveReward_ActiveReward", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ActiveReward = new JSONObject(strSrc);

            for (int i = 0; i < ActiveReward.list.Count; i++)
            {
                ActiveRewardInfo tmpInfo = new ActiveRewardInfo();
                tmpInfo.Id = (uint)ActiveReward[i]["Id_ui"].n;
                tmpInfo.RewardRank = (byte)ActiveReward[i]["RewardRank_b"].n;
                tmpInfo.PointValue = (uint)ActiveReward[i]["PointValue_ui"].n;
                tmpInfo.RewardId = (uint)ActiveReward[i]["RewardId_ui"].n;

                ActiveRewardInfoList.Add(tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ActivePointInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ActivePointInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ActiveRewardInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ActiveRewardInfoList);
            fs.Close();
        }
    }

    public void DeserializeData()
    {
        ActivePointInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, ActivePointInfo>>("ActivePointInfo");
        ActiveRewardInfoList = _LowDataMgr.instance.DeserializeData<List<ActiveRewardInfo>>("ActiveRewardInfo");
    }
}