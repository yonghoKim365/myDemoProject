using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Achievement
{
    [Serializable]
    public class AchievementInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _Subtype;
        public byte Subtype
        {
            set { _Subtype = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Subtype); }
        }
        byte _Phase;
        public byte Phase
        {
            set { _Phase = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Phase); }
        }
        ushort _Cleartype;
        public ushort Cleartype
        {
            set { _Cleartype = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Cleartype); }
        }
        uint _Clearvalue;
        public uint Clearvalue
        {
            set { _Clearvalue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Clearvalue); }
        }
        uint _NextId;
        public uint NextId
        {
            set { _NextId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextId); }
        }
        uint _NameId;
        public uint NameId
        {
            set { _NameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NameId); }
        }
        uint _DescriptionId;
        public uint DescriptionId
        {
            set { _DescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_DescriptionId); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
        uint _Getpoint;
        public uint Getpoint
        {
            set { _Getpoint = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Getpoint); }
        }
    }

    [Serializable]
    public class AchievementCategoryInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        uint _Clearvalue;
        public uint Clearvalue
        {
            set { _Clearvalue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Clearvalue); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }

    }
    [Serializable]
    public class DailyInfo
    {
        public uint Id;
        uint _NameId;
        public uint NameId
        {
            set { _NameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NameId); }
        }
        uint _DescId;
        public uint DescId
        {
            set { _DescId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_DescId); }
        }
        ushort _Type;
        public ushort Type
        {
            set { _Type = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Type); }
        }
        byte _Sort;
        public byte Sort
        {
            set { _Sort = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Sort); }
        }
        byte _Phase;
        public byte Phase
        {
            set { _Phase = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Phase); }
        }
        uint _MaxCount;
        public uint MaxCount
        {
            set { _MaxCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MaxCount); }
        }
        uint _PointValue;
        public uint PointValue
        {
            set { _PointValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_PointValue); }
        }
        byte _LimitLv;
        public byte LimitLv
        {
            set { _LimitLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLv); }
        }
        uint _NextId;
        public uint NextId
        {
            set { _NextId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextId); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
    }

    [Serializable]
    public class DailyRewardInfo
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
        uint _VipRewardId;
        public uint VipRewardId
        {
            set { _VipRewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_VipRewardId); }
        }
        byte _LimitVip;
        public byte LimitVip
        {
            set { _LimitVip = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitVip); }
        }

    }


    public List<AchievementInfo> AchievementInfoList = new List<AchievementInfo>();
    public List<AchievementCategoryInfo> AchievementCategoryInfoList = new List<AchievementCategoryInfo>();
    public List<DailyInfo> DailyInfoList = new List<DailyInfo>();
    public List<DailyRewardInfo> DailyRewardInfoList = new List<DailyRewardInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Achievement_Achievement", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Achievement = new JSONObject(strSrc);

            for (int i = 0; i < Achievement.list.Count; i++)
            {
                AchievementInfo tmpInfo = new AchievementInfo();
                tmpInfo.Id = (uint)Achievement[i]["Id_ui"].n;
                tmpInfo.Type = (byte)Achievement[i]["Type_b"].n;
                tmpInfo.Subtype = (byte)Achievement[i]["Subtype_b"].n;
                tmpInfo.Phase = (byte)Achievement[i]["Phase_b"].n;
                tmpInfo.Cleartype = (ushort)Achievement[i]["Cleartype_us"].n;
                tmpInfo.Clearvalue = (uint)Achievement[i]["Clearvalue_ui"].n;
                tmpInfo.NextId = (uint)Achievement[i]["NextId_ui"].n;
                tmpInfo.NameId = (uint)Achievement[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Achievement[i]["DescriptionId_ui"].n;
                tmpInfo.RewardId = (uint)Achievement[i]["RewardId_ui"].n;
                tmpInfo.Getpoint = (uint)Achievement[i]["Getpoint_ui"].n;

                AchievementInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Achievement_AchievementCategory", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject AchievementCategory = new JSONObject(strSrc);

            for (int i = 0; i < AchievementCategory.list.Count; i++)
            {
                AchievementCategoryInfo tmpInfo = new AchievementCategoryInfo();
                tmpInfo.Id = (uint)AchievementCategory[i]["Id_ui"].n;
                tmpInfo.Type = (byte)AchievementCategory[i]["Type_b"].n;
                tmpInfo.Clearvalue = (uint)AchievementCategory[i]["Clearvalue_ui"].n;
                tmpInfo.RewardId = (uint)AchievementCategory[i]["RewardId_ui"].n;

                AchievementCategoryInfoList.Add(tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Achievement_Daily", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Daily = new JSONObject(strSrc);

            for (int i = 0; i < Daily.list.Count; i++)
            {
                DailyInfo tmpInfo = new DailyInfo();
                tmpInfo.Id = (uint)Daily[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)Daily[i]["NameId_ui"].n;
                tmpInfo.DescId = (uint)Daily[i]["DescId_ui"].n;
                tmpInfo.Type = (ushort)Daily[i]["Type_us"].n;
                tmpInfo.Sort = (byte)Daily[i]["Sort_b"].n;
                tmpInfo.Phase = (byte)Daily[i]["Phase_b"].n;
                tmpInfo.MaxCount = (uint)Daily[i]["MaxCount_ui"].n;
                tmpInfo.PointValue = (uint)Daily[i]["PointValue_ui"].n;
                tmpInfo.LimitLv = (byte)Daily[i]["LimitLv_b"].n;
                tmpInfo.NextId = (uint)Daily[i]["NextId_ui"].n;
                tmpInfo.RewardId = (uint)Daily[i]["RewardId_ui"].n;

                DailyInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Achievement_DailyReward", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject DailyReward = new JSONObject(strSrc);

            for (int i = 0; i < DailyReward.list.Count; i++)
            {
                DailyRewardInfo tmpInfo = new DailyRewardInfo();
                tmpInfo.Id = (uint)DailyReward[i]["Id_ui"].n;
                tmpInfo.RewardRank = (byte)DailyReward[i]["RewardRank_b"].n;
                tmpInfo.PointValue = (uint)DailyReward[i]["PointValue_ui"].n;
                tmpInfo.RewardId = (uint)DailyReward[i]["RewardId_ui"].n;
                tmpInfo.VipRewardId = (uint)DailyReward[i]["VipRewardId_ui"].n;
                tmpInfo.LimitVip = (byte)DailyReward[i]["LimitVip_b"].n;

                DailyRewardInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/AchievementInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, AchievementInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/AchievementCategoryInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, AchievementCategoryInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/DailyInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, DailyInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/DailyRewardInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, DailyRewardInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
        //        AchievementInfoList = _LowDataMgr.instance.DeserializeData<List<AchievementInfo>>("AchievementInfo");
        //        AchievementCategoryInfoList = _LowDataMgr.instance.DeserializeData<List<AchievementCategoryInfo>>("AchievementCategoryInfo");

        _LowDataMgr.instance.DeserializeData<List<AchievementInfo>>("AchievementInfo", (data) => { AchievementInfoList = data; });
        _LowDataMgr.instance.DeserializeData<List<AchievementCategoryInfo>>("AchievementCategoryInfo", (data) => { AchievementCategoryInfoList = data; });
        _LowDataMgr.instance.DeserializeData<List<DailyInfo>>("DailyInfo", (data) => { DailyInfoList = data; });
        _LowDataMgr.instance.DeserializeData<List<DailyRewardInfo>>("DailyRewardInfo", (data) => { DailyRewardInfoList = data; });


    }
}

