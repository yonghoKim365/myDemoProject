using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Guild
{
    [Serializable]
    public class GuildInfo
    {
        public uint GuildLevel;
        uint _GuildNeedGold;
        public uint GuildNeedGold
        {
            set { _GuildNeedGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GuildNeedGold); }
        }
        uint _GuildTotalGold;
        public uint GuildTotalGold
        {
            set { _GuildTotalGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GuildTotalGold); }
        }
        ushort _GuildJoinLimit;
        public ushort GuildJoinLimit
        {
            set { _GuildJoinLimit = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_GuildJoinLimit); }
        }
        byte _GuildMaster;
        public byte GuildMaster
        {
            set { _GuildMaster = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildMaster); }
        }
        byte _GuildSubMaster;
        public byte GuildSubMaster
        {
            set { _GuildSubMaster = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildSubMaster); }
        }
        byte _GuildEliteMember;
        public byte GuildEliteMember
        {
            set { _GuildEliteMember = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildEliteMember); }
        }
        byte _AllQuestValue;
        public byte AllQuestValue
        {
            set { _AllQuestValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AllQuestValue); }
        }
        byte _OnlyQuestValue;
        public byte OnlyQuestValue
        {
            set { _OnlyQuestValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_OnlyQuestValue); }
        }
    }
    [Serializable]
    public class DonateInfo
    {
        public uint DonateIdx;
        byte _DonateType;
        public byte DonateType
        {
            set { _DonateType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DonateType); }
        }
        uint _DonateValue;
        public uint DonateValue
        {
            set { _DonateValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_DonateValue); }
        }
        uint _GuildGold;
        public uint GuildGold
        {
            set { _GuildGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GuildGold); }
        }
        uint _GetPoint;
        public uint GetPoint
        {
            set { _GetPoint = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GetPoint); }
        }
        byte _DailyDonate;
        public byte DailyDonate
        {
            set { _DailyDonate = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DailyDonate); }
        }
    }
    [Serializable]
    public class GuildPositionInfo
    {
        public uint PositionIdx;
        uint _name;
        public uint name
        {
            set { _name = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_name); }
        }
        byte _MasterEntrust;
        public byte MasterEntrust
        {
            set { _MasterEntrust = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MasterEntrust); }
        }
        byte _SubMasterEntrust;
        public byte SubMasterEntrust
        {
            set { _SubMasterEntrust = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_SubMasterEntrust); }
        }
        byte _NormalMember;
        public byte NormalMember
        {
            set { _NormalMember = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_NormalMember); }
        }
        byte _EliteMember;
        public byte EliteMember
        {
            set { _EliteMember = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_EliteMember); }
        }
        byte _MemberLeave;
        public byte MemberLeave
        {
            set { _MemberLeave = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MemberLeave); }
        }
        byte _GuildNameChange;
        public byte GuildNameChange
        {
            set { _GuildNameChange = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildNameChange); }
        }
        byte _GuildWriteChange;
        public byte GuildWriteChange
        {
            set { _GuildWriteChange = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildWriteChange); }
        }
        byte _Guildnotice;
        public byte Guildnotice
        {
            set { _Guildnotice = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Guildnotice); }
        }
        byte _UserAccept;
        public byte UserAccept
        {
            set { _UserAccept = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_UserAccept); }
        }
        byte _AcceptType;
        public byte AcceptType
        {
            set { _AcceptType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AcceptType); }
        }
        byte _GuildLevelUp;
        public byte GuildLevelUp
        {
            set { _GuildLevelUp = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildLevelUp); }
        }
        byte _GuildShopLevelUP;
        public byte GuildShopLevelUP
        {
            set { _GuildShopLevelUP = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildShopLevelUP); }
        }
        byte _GuildPrayLevelUP;
        public byte GuildPrayLevelUP
        {
            set { _GuildPrayLevelUP = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildPrayLevelUP); }
        }
        byte _GuildiconChange;
        public byte GuildiconChange
        {
            set { _GuildiconChange = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuildiconChange); }
        }
    }
    [Serializable]
    public class GuildShopLevelInfo
    {
        public uint GShopLevel;
        uint _ShopType;
        public uint ShopType
        {
            set { _ShopType = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShopType); }
        }
        uint _GPrayNeedGuildLv;
        public uint GPrayNeedGuildLv
        {
            set { _GPrayNeedGuildLv = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GPrayNeedGuildLv); }
        }
        uint _GShopLvNeedGold;
        public uint GShopLvNeedGold
        {
            set { _GShopLvNeedGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GShopLvNeedGold); }
        }
        uint _Gshop;
        public uint Gshop
        {
            set { _Gshop = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Gshop); }
        }

    }
    [Serializable]
    public class GuildprayLevelInfo
    {
        public uint GPrayLevel;
        uint _GPrayNeedGuildLv;
        public uint GPrayNeedGuildLv
        {
            set { _GPrayNeedGuildLv = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GPrayNeedGuildLv); }
        }
        uint _GPrayLvNeedGold;
        public uint GPrayLvNeedGold
        {
            set { _GPrayLvNeedGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GPrayLvNeedGold); }
        }
        ushort _GprayNormalcount;
        public ushort GprayNormalcount
        {
            set { _GprayNormalcount = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_GprayNormalcount); }
        }
        uint _GprayNormalcoin;
        public uint GprayNormalcoin
        {
            set { _GprayNormalcoin = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GprayNormalcoin); }
        }
        byte _GprayNormalValue;
        public byte GprayNormalValue
        {
            set { _GprayNormalValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GprayNormalValue); }
        }
        uint _GprayNormalitem;
        public uint GprayNormalitem
        {
            set { _GprayNormalitem = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GprayNormalitem); }
        }
        ushort _GprayHighcount;
        public ushort GprayHighcount
        {
            set { _GprayHighcount = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_GprayHighcount); }
        }
        uint _GprayHighNeedCash;
        public uint GprayHighNeedCash
        {
            set { _GprayHighNeedCash = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GprayHighNeedCash); }
        }
        uint _GprayHighcoin;
        public uint GprayHighcoin
        {
            set { _GprayHighcoin = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GprayHighcoin); }
        }
        byte _GprayHighValue;
        public byte GprayHighValue
        {
            set { _GprayHighValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GprayHighValue); }
        }
        uint _GprayHighitem;
        public uint GprayHighitem
        {
            set { _GprayHighitem = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GprayHighitem); }
        }
    }

    [Serializable]
    public class GuildQuestInfo
    {
        public uint QuestId;
        //public uint QuesrRate;
        uint _NeedLevel;
        public uint NeedLevel
        {
            set { _NeedLevel = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NeedLevel); }
        }
        public string QuestTitle;
        public string QuestExp;
        uint _Type;
        public uint Type
        {
            set { _Type = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Type); }
        }
        uint _QuestValue;
        public uint QuestValue
        {
            set { _QuestValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QuestValue); }
        }
        byte _ClearType;
        public byte ClearType
        {
            set { _ClearType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearType); }
        }
        byte _ResetType;
        public byte ResetType
        {
            set { _ResetType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ResetType); }
        }
        uint _ResetValue;
        public uint ResetValue
        {
            set { _ResetValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ResetValue); }
        }
        uint _Fixedreward;
        public uint Fixedreward
        {
            set { _Fixedreward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Fixedreward); }
        }
    }



    public Dictionary<uint, GuildInfo> GuildInfoDic = new Dictionary<uint, GuildInfo>();
    public Dictionary<uint, DonateInfo> DonateInfoDic = new Dictionary<uint, DonateInfo>();
    public Dictionary<uint, GuildPositionInfo> GuildPositionInfoDic = new Dictionary<uint, GuildPositionInfo>();
    public Dictionary<uint, GuildShopLevelInfo> GuildShopLevelInfoDic = new Dictionary<uint, GuildShopLevelInfo>();
    public Dictionary<uint, GuildprayLevelInfo> GuildprayLevelInfoDic = new Dictionary<uint, GuildprayLevelInfo>();
    public Dictionary<uint, GuildQuestInfo> GuildQuestInfoDic = new Dictionary<uint, GuildQuestInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Guild_Guild", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Guild = new JSONObject(strSrc);

            for (int i = 0; i < Guild.list.Count; i++)
            {
                GuildInfo tmpInfo = new GuildInfo();
                tmpInfo.GuildLevel = (uint)Guild[i]["GuildLevel_ui"].n;
                tmpInfo.GuildNeedGold = (uint)Guild[i]["GuildNeedGold_ui"].n;
                tmpInfo.GuildTotalGold = (uint)Guild[i]["GuildTotalGold_ui"].n;
                tmpInfo.GuildJoinLimit = (ushort)Guild[i]["GuildJoinLimit_us"].n;
                tmpInfo.GuildMaster = (byte)Guild[i]["GuildMaster_b"].n;
                tmpInfo.GuildSubMaster = (byte)Guild[i]["GuildSubMaster_b"].n;
                tmpInfo.GuildEliteMember = (byte)Guild[i]["GuildEliteMember_b"].n;
                tmpInfo.AllQuestValue = (byte)Guild[i]["AllQuestValue_b"].n;
                tmpInfo.OnlyQuestValue = (byte)Guild[i]["OnlyQuestValue_b"].n;

                GuildInfoDic.Add(tmpInfo.GuildLevel, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Guild_Donate", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Donate = new JSONObject(strSrc);

            for (int i = 0; i < Donate.list.Count; i++)
            {
                DonateInfo tmpInfo = new DonateInfo();
                tmpInfo.DonateIdx = (uint)Donate[i]["DonateIdx_ui"].n;
                tmpInfo.DonateType = (byte)Donate[i]["DonateType_b"].n;
                tmpInfo.DonateValue = (uint)Donate[i]["DonateValue_ui"].n;
                tmpInfo.GuildGold = (uint)Donate[i]["GuildGold_ui"].n;
                tmpInfo.GetPoint = (uint)Donate[i]["GetPoint_ui"].n;
                tmpInfo.DailyDonate = (byte)Donate[i]["DailyDonate_b"].n;

                DonateInfoDic.Add(tmpInfo.DonateIdx, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Guild_GuildPosition", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject GuildPosition = new JSONObject(strSrc);

            for (int i = 0; i < GuildPosition.list.Count; i++)
            {
                GuildPositionInfo tmpInfo = new GuildPositionInfo();
                tmpInfo.PositionIdx = (uint)GuildPosition[i]["PositionIdx_ui"].n;
                tmpInfo.name = (uint)GuildPosition[i]["name_ui"].n;
                tmpInfo.MasterEntrust = (byte)GuildPosition[i]["MasterEntrust_b"].n;
                tmpInfo.SubMasterEntrust = (byte)GuildPosition[i]["SubMasterEntrust_b"].n;
                tmpInfo.NormalMember = (byte)GuildPosition[i]["NormalMember_b"].n;
                tmpInfo.EliteMember = (byte)GuildPosition[i]["EliteMember_b"].n;
                tmpInfo.MemberLeave = (byte)GuildPosition[i]["MemberLeave_b"].n;
                tmpInfo.GuildNameChange = (byte)GuildPosition[i]["GuildNameChange_b"].n;
                tmpInfo.GuildWriteChange = (byte)GuildPosition[i]["GuildWriteChange_b"].n;
                tmpInfo.Guildnotice = (byte)GuildPosition[i]["Guildnotice_b"].n;
                tmpInfo.UserAccept = (byte)GuildPosition[i]["UserAccept_b"].n;
                tmpInfo.AcceptType = (byte)GuildPosition[i]["AcceptType_b"].n;
                tmpInfo.GuildLevelUp = (byte)GuildPosition[i]["GuildLevelUp_b"].n;
                tmpInfo.GuildShopLevelUP = (byte)GuildPosition[i]["GuildShopLevelUP_b"].n;
                tmpInfo.GuildPrayLevelUP = (byte)GuildPosition[i]["GuildPrayLevelUP_b"].n;
                tmpInfo.GuildiconChange = (byte)GuildPosition[i]["GuildiconChange_b"].n;

                GuildPositionInfoDic.Add(tmpInfo.PositionIdx, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Guild_GuildShopLevel", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject GuildShopLevel = new JSONObject(strSrc);

            for (int i = 0; i < GuildShopLevel.list.Count; i++)
            {
                GuildShopLevelInfo tmpInfo = new GuildShopLevelInfo();
                tmpInfo.GShopLevel = (uint)GuildShopLevel[i]["GShopLevel_ui"].n;
                tmpInfo.ShopType = (uint)GuildShopLevel[i]["ShopType_ui"].n;
                tmpInfo.GPrayNeedGuildLv = (uint)GuildShopLevel[i]["GPrayNeedGuildLv_ui"].n;
                tmpInfo.GShopLvNeedGold = (uint)GuildShopLevel[i]["GShopLvNeedGold_ui"].n;
                tmpInfo.Gshop = (uint)GuildShopLevel[i]["Gshop_ui"].n;

                GuildShopLevelInfoDic.Add(tmpInfo.GShopLevel, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Guild_GuildprayLevel", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject GuildprayLevel = new JSONObject(strSrc);

            for (int i = 0; i < GuildprayLevel.list.Count; i++)
            {
                GuildprayLevelInfo tmpInfo = new GuildprayLevelInfo();
                tmpInfo.GPrayLevel = (uint)GuildprayLevel[i]["GPrayLevel_ui"].n;
                tmpInfo.GPrayNeedGuildLv = (uint)GuildprayLevel[i]["GPrayNeedGuildLv_ui"].n;
                tmpInfo.GPrayLvNeedGold = (uint)GuildprayLevel[i]["GPrayLvNeedGold_ui"].n;
                tmpInfo.GprayNormalcount = (ushort)GuildprayLevel[i]["GprayNormalcount_us"].n;
                tmpInfo.GprayNormalcoin = (uint)GuildprayLevel[i]["GprayNormalcoin_ui"].n;
                tmpInfo.GprayNormalValue = (byte)GuildprayLevel[i]["GprayNormalValue_b"].n;
                tmpInfo.GprayNormalitem = (uint)GuildprayLevel[i]["GprayNormalitem_ui"].n;
                tmpInfo.GprayHighcount = (ushort)GuildprayLevel[i]["GprayHighcount_us"].n;
                tmpInfo.GprayHighNeedCash = (uint)GuildprayLevel[i]["GprayHighNeedCash_ui"].n;
                tmpInfo.GprayHighcoin = (uint)GuildprayLevel[i]["GprayHighcoin_ui"].n;
                tmpInfo.GprayHighValue = (byte)GuildprayLevel[i]["GprayHighValue_b"].n;
                tmpInfo.GprayHighitem = (uint)GuildprayLevel[i]["GprayHighitem_ui"].n;

                GuildprayLevelInfoDic.Add(tmpInfo.GPrayLevel, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Guild_GuildQuest", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject GuildQuest = new JSONObject(strSrc);

            for (int i = 0; i < GuildQuest.list.Count; i++)
            {
                GuildQuestInfo tmpInfo = new GuildQuestInfo();
                tmpInfo.QuestId = (uint)GuildQuest[i]["QuestId_ui"].n;
				//tmpInfo.QuesrRate = (uint)GuildQuest[i]["QuesrRate_ui"].n; // not use in client.
                tmpInfo.NeedLevel = (uint)GuildQuest[i]["NeedLevel_ui"].n;
                tmpInfo.QuestTitle = GuildQuest[i]["QuestTitle_c"].str;
                tmpInfo.QuestExp = GuildQuest[i]["QuestExp_c"].str;
                tmpInfo.Type = (uint)GuildQuest[i]["Type_ui"].n;
                tmpInfo.QuestValue = (uint)GuildQuest[i]["QuestValue_ui"].n;
                tmpInfo.ClearType = (byte)GuildQuest[i]["ClearType_b"].n;
                tmpInfo.ResetType = (byte)GuildQuest[i]["ResetType_b"].n;
                tmpInfo.ResetValue = (uint)GuildQuest[i]["ResetValue_ui"].n;
                tmpInfo.Fixedreward = (uint)GuildQuest[i]["Fixedreward_ui"].n;

                GuildQuestInfoDic.Add(tmpInfo.QuestId, tmpInfo);
            }
        }



    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GuildInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GuildInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/DonateInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, DonateInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GuildPositionInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GuildPositionInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GuildShopLevelInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GuildShopLevelInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GuildprayLevelInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GuildprayLevelInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/GuildQuestInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, GuildQuestInfoDic);
            fs.Close();
        }
    }

    public void DeserializeData()
    {
//        GuildInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildInfo>>("GuildInfo");
//        DonateInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, DonateInfo>>("DonateInfo");
//        GuildPositionInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildPositionInfo>>("GuildPositionInfo");
//        GuildShopLevelInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildShopLevelInfo>>("GuildShopLevelInfo");
//        GuildprayLevelInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildprayLevelInfo>>("GuildprayLevelInfo");
//        GuildQuestInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildQuestInfo>>("GuildQuestInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildInfo>>("GuildInfo", (data) => { GuildInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, DonateInfo>>("DonateInfo", (data) => { DonateInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildPositionInfo>>("GuildPositionInfo", (data) => { GuildPositionInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildShopLevelInfo>>("GuildShopLevelInfo", (data) => { GuildShopLevelInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildprayLevelInfo>>("GuildprayLevelInfo", (data) => { GuildprayLevelInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, GuildQuestInfo>>("GuildQuestInfo", (data) => { GuildQuestInfoDic = data; });


    }
}
