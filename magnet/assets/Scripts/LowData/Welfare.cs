using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Welfare
{
    [Serializable]
    public class WelfareInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _RewardRank;
        public byte RewardRank
        {
            set { _RewardRank = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RewardRank); }
        }
        uint _RewardCondition;
        public uint RewardCondition
        {
            set { _RewardCondition = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardCondition); }
        }
        uint _SubCondition;
        public uint SubCondition
        {
            set { _SubCondition = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SubCondition); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
    }

    [Serializable]
    public class DailyCheckInfo
    {
        public uint Id;
        ushort _Year;
        public ushort Year
        {
            set { _Year = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Year); }
        }
        byte _Month;
        public byte Month
        {
            set { _Month = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Month); }
        }
        byte _Day;
        public byte Day
        {
            set { _Day = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Day); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
    }

    [Serializable]
    public class EventCheckInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _AttendanceDay;
        public byte AttendanceDay
        {
            set { _AttendanceDay = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AttendanceDay); }
        }
        uint _RewardId;
        public uint RewardId
        {
            set { _RewardId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId); }
        }
    }

    [Serializable]
    public class RechargeBonusInfo
    {
        public uint Id;
        uint _PaymentValue;
        public uint PaymentValue
        {
            set { _PaymentValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_PaymentValue); }
        }
        byte _BonusValue;
        public byte BonusValue
        {
            set { _BonusValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BonusValue); }
        }
    }

    [Serializable]
    public class ServerEventInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _ServerType;
        public byte ServerType
        {
            set { _ServerType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ServerType); }
        }
        uint _EventStart;
        public uint EventStart
        {
            set { _EventStart = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_EventStart); }
        }
        uint _EventEnd;
        public uint EventEnd
        {
            set { _EventEnd = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_EventEnd); }
        }
    }





    public List<WelfareInfo> WelfareInfoList = new List<WelfareInfo>();
    public Dictionary<uint, DailyCheckInfo> DailyCheckInfoDic = new Dictionary<uint, DailyCheckInfo>();
    public List<EventCheckInfo> EventCheckInfoList = new List<EventCheckInfo>();
    public Dictionary<uint, RechargeBonusInfo> RechargeBonusInfoDic = new Dictionary<uint, RechargeBonusInfo>();
    public List<ServerEventInfo> ServerEventInfoList = new List<ServerEventInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Welfare_Welfare", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Welfare = new JSONObject(strSrc);

            for (int i = 0; i < Welfare.list.Count; i++)
            {
                WelfareInfo tmpInfo = new WelfareInfo();
                tmpInfo.Id = (uint)Welfare[i]["Id_ui"].n;
                tmpInfo.Type = (byte)Welfare[i]["Type_b"].n;
                tmpInfo.RewardRank = (byte)Welfare[i]["RewardRank_b"].n;
                tmpInfo.RewardCondition = (uint)Welfare[i]["RewardCondition_ui"].n;
                tmpInfo.SubCondition = (uint)Welfare[i]["SubCondition_ui"].n;
                tmpInfo.RewardId = (uint)Welfare[i]["RewardId_ui"].n;

                WelfareInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Welfare_DailyCheck", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject DailyCheck = new JSONObject(strSrc);

            for (int i = 0; i < DailyCheck.list.Count; i++)
            {
                DailyCheckInfo tmpInfo = new DailyCheckInfo();
                tmpInfo.Id = (uint)DailyCheck[i]["Id_ui"].n;
                tmpInfo.Year = (ushort)DailyCheck[i]["Year_us"].n;
                tmpInfo.Month = (byte)DailyCheck[i]["Month_b"].n;
                tmpInfo.Day = (byte)DailyCheck[i]["Day_b"].n;
                tmpInfo.RewardId = (uint)DailyCheck[i]["RewardId_ui"].n;

                DailyCheckInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Welfare_EventCheck", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject EventCheck = new JSONObject(strSrc);

            for (int i = 0; i < EventCheck.list.Count; i++)
            {
                EventCheckInfo tmpInfo = new EventCheckInfo();
                tmpInfo.Id = (uint)EventCheck[i]["Id_ui"].n;
                tmpInfo.Type = (byte)EventCheck[i]["Type_b"].n;
                tmpInfo.AttendanceDay = (byte)EventCheck[i]["AttendanceDay_b"].n;
                tmpInfo.RewardId = (uint)EventCheck[i]["RewardId_ui"].n;

                EventCheckInfoList.Add(tmpInfo);
            }
        }


        {
            TextAsset data = Resources.Load("TestJson/Welfare_RechargeBonus", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject RechargeBonus = new JSONObject(strSrc);

            for (int i = 0; i < RechargeBonus.list.Count; i++)
            {
                RechargeBonusInfo tmpInfo = new RechargeBonusInfo();
                tmpInfo.Id = (uint)RechargeBonus[i]["Id_ui"].n;
                tmpInfo.PaymentValue = (uint)RechargeBonus[i]["PaymentValue_ui"].n;
                tmpInfo.BonusValue = (byte)RechargeBonus[i]["BonusValue_b"].n;

                RechargeBonusInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Welfare_ServerEvent", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ServerEvent = new JSONObject(strSrc);

            for (int i = 0; i < ServerEvent.list.Count; i++)
            {
                ServerEventInfo tmpInfo = new ServerEventInfo();
                tmpInfo.Id = (uint)ServerEvent[i]["Id_ui"].n;
                tmpInfo.Type = (byte)ServerEvent[i]["Type_b"].n;
                tmpInfo.ServerType = (byte)ServerEvent[i]["ServerType_b"].n;
                tmpInfo.EventStart = (uint)ServerEvent[i]["EventStart_ui"].n;
                tmpInfo.EventEnd = (uint)ServerEvent[i]["EventEnd_ui"].n;

                ServerEventInfoList.Add(tmpInfo);
            }
        }



    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/WelfareInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, WelfareInfoList);
            fs.Close();
        }


        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/DailyCheckInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, DailyCheckInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EventCheckInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EventCheckInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/RechargeBonusInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, RechargeBonusInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ServerEventInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ServerEventInfoList);
            fs.Close();
        }


    }

    public void DeserializeData()
    {
//        WelfareInfoList = _LowDataMgr.instance.DeserializeData<List<WelfareInfo>>("WelfareInfo");
//        DailyCheckInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, DailyCheckInfo>>("DailyCheckInfo");
//        EventCheckInfoList = _LowDataMgr.instance.DeserializeData<List<EventCheckInfo>>("EventCheckInfo");
//        RechargeBonusInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, RechargeBonusInfo>>("RechargeBonusInfo");
//        ServerEventInfoList = _LowDataMgr.instance.DeserializeData<List<ServerEventInfo>>("ServerEventInfo");

		_LowDataMgr.instance.DeserializeData<List<WelfareInfo>>("WelfareInfo", (data) => { WelfareInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, DailyCheckInfo>>("DailyCheckInfo", (data) => { DailyCheckInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<EventCheckInfo>>("EventCheckInfo", (data) => { EventCheckInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, RechargeBonusInfo>>("RechargeBonusInfo", (data) => { RechargeBonusInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<ServerEventInfo>>("ServerEventInfo", (data) => { ServerEventInfoList = data; });


    }
}
