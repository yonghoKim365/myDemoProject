using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class MissionTable
{
    [Serializable]
    public class MissionInfo
    {
        public uint MissionID;
        byte _MissionType;
        public byte MissionType
        {
            set { _MissionType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MissionType); }
        }
        byte _MissionSubType;
        public byte MissionSubType
        {
            set { _MissionSubType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MissionSubType); }
        }
        uint _MissionValue;
        public uint MissionValue
        {
            set { _MissionValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MissionValue); }
        }
        public string MissionString;
        uint _rewardGold;
        public uint rewardGold
        {
            set { _rewardGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardGold); }
        }
        uint _rewardExp;
        public uint rewardExp
        {
            set { _rewardExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardExp); }
        }
        uint _rewardCash;
        public uint rewardCash
        {
            set { _rewardCash = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardCash); }
        }
        uint _rewardEnergy;
        public uint rewardEnergy
        {
            set { _rewardEnergy = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardEnergy); }
        }
        byte _isDaily;
        public byte isDaily
        {
            set { _isDaily = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_isDaily); }
        }
        uint _NextMission;
        public uint NextMission
        {
            set { _NextMission = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextMission); }
        }
        uint _beforeMission;
        public uint beforeMission
        {
            set { _beforeMission = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_beforeMission); }
        }
    }
    public List<MissionInfo> MissionInfoList = new List<MissionInfo>();

    public void LoadLowData()
    {
        TextAsset data = Resources.Load("TestJson/Mission_Mission", typeof(TextAsset)) as TextAsset;
        StringReader sr = new StringReader(data.text);
        string strSrc = sr.ReadToEnd();
        JSONObject Mission = new JSONObject(strSrc);

        for (int i = 0; i < Mission.list.Count; i++)
        {
            MissionInfo tmpInfo = new MissionInfo();
            tmpInfo.MissionID = (uint)Mission[i]["MissionID_ui"].n;
            tmpInfo.MissionType = (byte)Mission[i]["MissionType_b"].n;
            tmpInfo.MissionSubType = (byte)Mission[i]["MissionSubType_b"].n;
            tmpInfo.MissionValue = (uint)Mission[i]["MissionValue_ui"].n;
            tmpInfo.MissionString = Mission[i]["MissionString_c"].str;
            tmpInfo.rewardGold = (uint)Mission[i]["rewardGold_ui"].n;
            tmpInfo.rewardExp = (uint)Mission[i]["rewardExp_ui"].n;
            tmpInfo.rewardCash = (uint)Mission[i]["rewardCash_ui"].n;
            tmpInfo.rewardEnergy = (uint)Mission[i]["rewardEnergy_ui"].n;
            tmpInfo.isDaily = (byte)Mission[i]["isDaily_b"].n;
            tmpInfo.NextMission = (uint)Mission[i]["NextMission_ui"].n;
            tmpInfo.beforeMission = (uint)Mission[i]["beforeMission_ui"].n;

            MissionInfoList.Add(tmpInfo);
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/MissionInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, MissionInfoList);
            fs.Close();
        }
    }

    public void DeserializeData()
    {
//        MissionInfoList = _LowDataMgr.instance.DeserializeData<List<MissionInfo>>("MissionInfo");

		_LowDataMgr.instance.DeserializeData<List<MissionInfo>>("MissionInfo", (data) => { MissionInfoList = data; });


    }
}
