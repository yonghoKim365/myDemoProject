using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Item
{
    [Serializable]
    public class CostumeInfo
    {
        public uint Id;
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
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        public string Faceprefab;
        public string Bodyprefab;
        public string Weaprefab;
        public string LeftWeaDummy;
        public string RightWeaDummy;
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        uint _ShardIdx;
        public uint ShardIdx
        {
            set { _ShardIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShardIdx); }
        }
        uint _NeedShardValue;
        public uint NeedShardValue
        {
            set { _NeedShardValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NeedShardValue); }
        }
        byte _BasicGrade;
        public byte BasicGrade
        {
            set { _BasicGrade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BasicGrade); }
        }
        uint _BasicOptionIndex;
        public uint BasicOptionIndex
        {
            set { _BasicOptionIndex = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BasicOptionIndex); }
        }
        uint _evolveId;
        public uint evolveId
        {
            set { _evolveId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_evolveId); }
        }
    }

    [Serializable]
    public class EquipmentInfo
    {
        public uint Id;
        ushort _EquipSetId;
        public ushort EquipSetId
        {
            set { _EquipSetId = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_EquipSetId); }
        }
        ushort _SerialId;
        public ushort SerialId
        {
            set { _SerialId = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_SerialId); }
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
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        public string prefab;
        public string LeftWeaDummy;
        public string RightWeaDummy;
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        byte _UseParts;
        public byte UseParts
        {
            set { _UseParts = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_UseParts); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        byte _Grade;
        public byte Grade
        {
            set { _Grade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Grade); }
        }
        byte _SetType;
        public byte SetType
        {
            set { _SetType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_SetType); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        ushort _SellPrice;
        public ushort SellPrice
        {
            set { _SellPrice = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_SellPrice); }
        }
        uint _BasicOptionIndex;
        public uint BasicOptionIndex
        {
            set { _BasicOptionIndex = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BasicOptionIndex); }
        }
        uint _OptionIndex2;
        public uint OptionIndex2
        {
            set { _OptionIndex2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_OptionIndex2); }
        }
        uint _OptionIndex3;
        public uint OptionIndex3
        {
            set { _OptionIndex3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_OptionIndex3); }
        }
        uint _OptionIndex4;
        public uint OptionIndex4
        {
            set { _OptionIndex4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_OptionIndex4); }
        }
        uint _EnchantId;
        public uint EnchantId
        {
            set { _EnchantId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_EnchantId); }
        }
        uint _MaxEnchant;
        public uint MaxEnchant
        {
            set { _MaxEnchant = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MaxEnchant); }
        }
        uint _EvolveId;
        public uint EvolveId
        {
            set { _EvolveId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_EvolveId); }
        }
        uint _GradeUpLevel;
        public uint GradeUpLevel
        {
            set { _GradeUpLevel = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GradeUpLevel); }
        }
        uint _NextPartsId;
        public uint NextPartsId
        {
            set { _NextPartsId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextPartsId); }
        }
        uint _break;
        public uint Break {
			set { _break = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_break); }
		}
		uint _UseTime;
        public uint UseTime
        {
            set { _UseTime = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_UseTime); }
        }
    }

    [Serializable]
    public class EquipmentSetInfo
    {
        public uint Id;
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _Default;
        public byte Default
        {
            set { _Default = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Default); }
        }
        uint _SetName;
        public uint SetName
        {
            set { _SetName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SetName); }
        }
        uint _SetDescriptionId;
        public uint SetDescriptionId
        {
            set { _SetDescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SetDescriptionId); }
        }
        uint _ItemIdx1;
        public uint ItemIdx1
        {
            set { _ItemIdx1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx1); }
        }
        uint _ItemIdx2;
        public uint ItemIdx2
        {
            set { _ItemIdx2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx2); }
        }
        uint _ItemIdx3;
        public uint ItemIdx3
        {
            set { _ItemIdx3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx3); }
        }
        uint _ItemIdx4;
        public uint ItemIdx4
        {
            set { _ItemIdx4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx4); }
        }
        uint _ItemIdx5;
        public uint ItemIdx5
        {
            set { _ItemIdx5 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx5); }
        }
        uint _ItemIdx6;
        public uint ItemIdx6
        {
            set { _ItemIdx6 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx6); }
        }
    }

    [Serializable]
    public class fusionInfo
    {
        public uint Id;
        ushort _group;
        public ushort group
        {
            set { _group = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_group); }
        }
        byte _num;
        public byte num
        {
            set { _num = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_num); }
        }
        uint _ItemIdx1;
        public uint ItemIdx1
        {
            set { _ItemIdx1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx1); }
        }
        uint _ItemValue1;
        public uint ItemValue1
        {
            set { _ItemValue1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemValue1); }
        }
        uint _ItemIdx2;
        public uint ItemIdx2
        {
            set { _ItemIdx2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx2); }
        }
        uint _ItemValue2;
        public uint ItemValue2
        {
            set { _ItemValue2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemValue2); }
        }
        uint _ItemIdx3;
        public uint ItemIdx3
        {
            set { _ItemIdx3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx3); }
        }
        uint _ItemValue3;
        public uint ItemValue3
        {
            set { _ItemValue3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemValue3); }
        }
        uint _ItemIdx4;
        public uint ItemIdx4
        {
            set { _ItemIdx4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx4); }
        }
        uint _ItemValue4;
        public uint ItemValue4
        {
            set { _ItemValue4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemValue4); }
        }
        uint _ItemIdx5;
        public uint ItemIdx5
        {
            set { _ItemIdx5 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx5); }
        }
        uint _ItemValue5;
        public uint ItemValue5
        {
            set { _ItemValue5 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemValue5); }
        }
    }


    [Serializable]
    public class ItemInfo
    {
        public uint Id;
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
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        byte _Grade;
        public byte Grade
        {
            set { _Grade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Grade); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        uint _maxstack;
        public uint maxstack
        {
            set { _maxstack = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_maxstack); }
        }
        ushort _SellPrice;
        public ushort SellPrice
        {
            set { _SellPrice = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_SellPrice); }
        }
        ushort _OptionType;
        public ushort OptionType
        {
            set { _OptionType = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_OptionType); }
        }
        uint _value;
        public uint value
        {
            set { _value = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_value); }
        }
        uint _UseTime;
        public uint UseTime
        {
            set { _UseTime = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_UseTime); }
        }
    }

    [Serializable]
    public class ItemValueInfo
    {
        public uint Idx;
        ushort _OptionId;
        public ushort OptionId
        {
            set { _OptionId = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_OptionId); }
        }
        uint _BasicValue;
        public uint BasicValue
        {
            set { _BasicValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BasicValue); }
        }
        uint _MinValue;
        public uint MinValue
        {
            set { _MinValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MinValue); }
        }
        uint _MaxValue;
        public uint MaxValue
        {
            set { _MaxValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MaxValue); }
        }
    }

    [Serializable]
    public class ContentsListInfo
    {
        public uint Idx;
        uint _ItemIdx;
        public uint ItemIdx
        {
            set { _ItemIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx); }
        }
        byte _ContentsType;
        public byte ContentsType
        {
            set { _ContentsType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ContentsType); }
        }
        uint _ContentsIdx;
        public uint ContentsIdx
        {
            set { _ContentsIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ContentsIdx); }
        }
        byte _ContentsLinkType;
        public byte ContentsLinkType
        {
            set { _ContentsLinkType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ContentsLinkType); }
        }
        uint _ContentsName;
        public uint ContentsName
        {
            set { _ContentsName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ContentsName); }
        }
        public JsonCustomData ContentsParam;
        uint _ConditionName;
        public uint ConditionName
        {
            set { _ConditionName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ConditionName); }
        }
        public JsonCustomData ConditionParam;
    }

    [Serializable]
    public class CategoryListInfo
    {
        public uint Idx;
        uint _CategoryName;
        public uint CategoryName
        {
            set { _CategoryName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CategoryName); }
        }
        uint _CategoryDesc;
        public uint CategoryDesc
        {
            set { _CategoryDesc = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CategoryDesc); }
        }
        public JsonCustomData ItemList;
    }
    
    public Dictionary<uint, CostumeInfo> CostumeInfoDic = new Dictionary<uint, CostumeInfo>();
    public Dictionary<uint, EquipmentInfo> EquipmentInfoDic = new Dictionary<uint, EquipmentInfo>();
    public Dictionary<uint, EquipmentSetInfo> EquipmentSetInfoDic = new Dictionary<uint, EquipmentSetInfo>();
    public Dictionary<uint, fusionInfo> fusionInfoDic = new Dictionary<uint, fusionInfo>();
    public Dictionary<uint, ItemInfo> ItemInfoDic = new Dictionary<uint, ItemInfo>();
    public Dictionary<uint, ItemValueInfo> ItemValueInfoDic = new Dictionary<uint, ItemValueInfo>();
    public List<ContentsListInfo> ContentsListInfoList = new List<ContentsListInfo>();
    public List<CategoryListInfo> CategoryListInfoList = new List<CategoryListInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Item_Costume", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Costume = new JSONObject(strSrc);

            for (int i = 0; i < Costume.list.Count; i++)
            {
                CostumeInfo tmpInfo = new CostumeInfo();
                tmpInfo.Id = (uint)Costume[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)Costume[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Costume[i]["DescriptionId_ui"].n;
                tmpInfo.Class = (byte)Costume[i]["Class_b"].n;
                tmpInfo.Faceprefab = Costume[i]["Faceprefab_c"].str;
                tmpInfo.Bodyprefab = Costume[i]["Bodyprefab_c"].str;
                tmpInfo.Weaprefab = Costume[i]["Weaprefab_c"].str;
                tmpInfo.LeftWeaDummy = Costume[i]["LeftWeaDummy_c"].str;
                tmpInfo.RightWeaDummy = Costume[i]["RightWeaDummy_c"].str;
                tmpInfo.Icon = (uint)Costume[i]["Icon_ui"].n;
                tmpInfo.LimitLevel = (byte)Costume[i]["LimitLevel_b"].n;
                tmpInfo.ShardIdx = (uint)Costume[i]["ShardIdx_ui"].n;
                tmpInfo.NeedShardValue = (uint)Costume[i]["NeedShardValue_ui"].n;
                tmpInfo.BasicGrade = (byte)Costume[i]["BasicGrade_b"].n;
                tmpInfo.BasicOptionIndex = (uint)Costume[i]["BasicOptionIndex_ui"].n;
                tmpInfo.evolveId = (uint)Costume[i]["evolveId_ui"].n;

                CostumeInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_Equipment", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Equipment = new JSONObject(strSrc);

            for (int i = 0; i < Equipment.list.Count; i++)
            {
                EquipmentInfo tmpInfo = new EquipmentInfo();
                tmpInfo.Id = (uint)Equipment[i]["Id_ui"].n;
                tmpInfo.EquipSetId = (ushort)Equipment[i]["EquipSetId_us"].n;
                tmpInfo.SerialId = (ushort)Equipment[i]["SerialId_us"].n;
                tmpInfo.NameId = (uint)Equipment[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Equipment[i]["DescriptionId_ui"].n;
                tmpInfo.Type = (byte)Equipment[i]["Type_b"].n;
                tmpInfo.prefab = Equipment[i]["prefab_c"].str;
                tmpInfo.LeftWeaDummy = Equipment[i]["LeftWeaDummy_c"].str;
                tmpInfo.RightWeaDummy = Equipment[i]["RightWeaDummy_c"].str;
                tmpInfo.Class = (byte)Equipment[i]["Class_b"].n;
                tmpInfo.UseParts = (byte)Equipment[i]["UseParts_b"].n;
                tmpInfo.LimitLevel = (byte)Equipment[i]["LimitLevel_b"].n;
                tmpInfo.Grade = (byte)Equipment[i]["Grade_b"].n;
                tmpInfo.SetType = (byte)Equipment[i]["SetType_b"].n;
                tmpInfo.Icon = (uint)Equipment[i]["Icon_ui"].n;
                tmpInfo.SellPrice = (ushort)Equipment[i]["SellPrice_us"].n;
                tmpInfo.BasicOptionIndex = (uint)Equipment[i]["BasicOptionIndex_ui"].n;
                tmpInfo.OptionIndex2 = (uint)Equipment[i]["OptionIndex2_ui"].n;
                tmpInfo.OptionIndex3 = (uint)Equipment[i]["OptionIndex3_ui"].n;
                tmpInfo.OptionIndex4 = (uint)Equipment[i]["OptionIndex4_ui"].n;
                tmpInfo.EnchantId = (uint)Equipment[i]["EnchantId_ui"].n;
                tmpInfo.MaxEnchant = (uint)Equipment[i]["MaxEnchant_ui"].n;
                tmpInfo.EvolveId = (uint)Equipment[i]["EvolveId_ui"].n;
                tmpInfo.GradeUpLevel = (uint)Equipment[i]["GradeUpLevel_ui"].n;
                tmpInfo.NextPartsId = (uint)Equipment[i]["NextPartsId_ui"].n;
                tmpInfo.Break = (uint)Equipment[i]["break_ui"].n;
                tmpInfo.UseTime = (uint)Equipment[i]["UseTime_ui"].n;

                EquipmentInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_EquipmentSet", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject EquipmentSet = new JSONObject(strSrc);

            for (int i = 0; i < EquipmentSet.list.Count; i++)
            {
                EquipmentSetInfo tmpInfo = new EquipmentSetInfo();
                tmpInfo.Id = (uint)EquipmentSet[i]["Id_ui"].n;
                tmpInfo.Class = (byte)EquipmentSet[i]["Class_b"].n;
                tmpInfo.Type = (byte)EquipmentSet[i]["Type_b"].n;
                tmpInfo.Default = (byte)EquipmentSet[i]["Default_b"].n;
                tmpInfo.SetName = (uint)EquipmentSet[i]["SetName_ui"].n;
                tmpInfo.SetDescriptionId = (uint)EquipmentSet[i]["SetDescriptionId_ui"].n;
                tmpInfo.ItemIdx1 = (uint)EquipmentSet[i]["ItemIdx1_ui"].n;
                tmpInfo.ItemIdx2 = (uint)EquipmentSet[i]["ItemIdx2_ui"].n;
                tmpInfo.ItemIdx3 = (uint)EquipmentSet[i]["ItemIdx3_ui"].n;
                tmpInfo.ItemIdx4 = (uint)EquipmentSet[i]["ItemIdx4_ui"].n;
                tmpInfo.ItemIdx5 = (uint)EquipmentSet[i]["ItemIdx5_ui"].n;
                tmpInfo.ItemIdx6 = (uint)EquipmentSet[i]["ItemIdx6_ui"].n;

                EquipmentSetInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_fusion", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject fusion = new JSONObject(strSrc);

            for (int i = 0; i < fusion.list.Count; i++)
            {
                fusionInfo tmpInfo = new fusionInfo();
                tmpInfo.Id = (uint)fusion[i]["Id_ui"].n;
                tmpInfo.group = (ushort)fusion[i]["group_us"].n;
                tmpInfo.num = (byte)fusion[i]["num_b"].n;
                tmpInfo.ItemIdx1 = (uint)fusion[i]["ItemIdx1_ui"].n;
                tmpInfo.ItemValue1 = (uint)fusion[i]["ItemValue1_ui"].n;
                tmpInfo.ItemIdx2 = (uint)fusion[i]["ItemIdx2_ui"].n;
                tmpInfo.ItemValue2 = (uint)fusion[i]["ItemValue2_ui"].n;
                tmpInfo.ItemIdx3 = (uint)fusion[i]["ItemIdx3_ui"].n;
                tmpInfo.ItemValue3 = (uint)fusion[i]["ItemValue3_ui"].n;
                tmpInfo.ItemIdx4 = (uint)fusion[i]["ItemIdx4_ui"].n;
                tmpInfo.ItemValue4 = (uint)fusion[i]["ItemValue4_ui"].n;
                tmpInfo.ItemIdx5 = (uint)fusion[i]["ItemIdx5_ui"].n;
                tmpInfo.ItemValue5 = (uint)fusion[i]["ItemValue5_ui"].n;

                fusionInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }


        {
            TextAsset data = Resources.Load("TestJson/Item_Item", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Item = new JSONObject(strSrc);

            for (int i = 0; i < Item.list.Count; i++)
            {
                ItemInfo tmpInfo = new ItemInfo();
                tmpInfo.Id = (uint)Item[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)Item[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Item[i]["DescriptionId_ui"].n;
                tmpInfo.Class = (byte)Item[i]["Class_b"].n;
                tmpInfo.Type = (byte)Item[i]["Type_b"].n;
                tmpInfo.LimitLevel = (byte)Item[i]["LimitLevel_b"].n;
                tmpInfo.Grade = (byte)Item[i]["Grade_b"].n;
                tmpInfo.Icon = (uint)Item[i]["Icon_ui"].n;
                tmpInfo.maxstack = (uint)Item[i]["maxstack_ui"].n;
                tmpInfo.SellPrice = (ushort)Item[i]["SellPrice_us"].n;
                tmpInfo.OptionType = (ushort)Item[i]["OptionType_us"].n;
                tmpInfo.value = (uint)Item[i]["value_ui"].n;
                tmpInfo.UseTime = (uint)Item[i]["UseTime_ui"].n;

                ItemInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_ItemValue", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ItemValue = new JSONObject(strSrc);

            for (int i = 0; i < ItemValue.list.Count; i++)
            {
                ItemValueInfo tmpInfo = new ItemValueInfo();
                tmpInfo.Idx = (uint)ItemValue[i]["Idx_ui"].n;
                tmpInfo.OptionId = (ushort)ItemValue[i]["OptionId_us"].n;
                tmpInfo.BasicValue = (uint)ItemValue[i]["BasicValue_ui"].n;
                tmpInfo.MinValue = (uint)ItemValue[i]["MinValue_ui"].n;
                tmpInfo.MaxValue = (uint)ItemValue[i]["MaxValue_ui"].n;

                ItemValueInfoDic.Add(tmpInfo.Idx, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_ContentsList", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ContentsList = new JSONObject(strSrc);

            for (int i = 0; i < ContentsList.list.Count; i++)
            {
                ContentsListInfo tmpInfo = new ContentsListInfo();
                tmpInfo.Idx = (uint)ContentsList[i]["Idx_ui"].n;
                tmpInfo.ItemIdx = (uint)ContentsList[i]["ItemIdx_ui"].n;
                tmpInfo.ContentsType = (byte)ContentsList[i]["ContentsType_b"].n;
                tmpInfo.ContentsIdx = (uint)ContentsList[i]["ContentsIdx_ui"].n;
                tmpInfo.ContentsLinkType = (byte)ContentsList[i]["ContentsLinkType_b"].n;
                tmpInfo.ContentsName = (uint)ContentsList[i]["ContentsName_ui"].n;
                tmpInfo.ContentsParam = new JsonCustomData(ContentsList[i]["ContentsParam_j"].ToString());
                tmpInfo.ConditionName = (uint)ContentsList[i]["ConditionName_ui"].n;
                tmpInfo.ConditionParam = new JsonCustomData(ContentsList[i]["ConditionParam_j"].ToString());

                ContentsListInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Item_CategoryList", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject CategoryList = new JSONObject(strSrc);

            for (int i = 0; i < CategoryList.list.Count; i++)
            {
                CategoryListInfo tmpInfo = new CategoryListInfo();
                tmpInfo.Idx = (uint)CategoryList[i]["Idx_ui"].n;
                tmpInfo.CategoryName = (uint)CategoryList[i]["CategoryName_ui"].n;
                tmpInfo.CategoryDesc = (uint)CategoryList[i]["CategoryDesc_ui"].n;
                tmpInfo.ItemList = new JsonCustomData(CategoryList[i]["ItemList_j"].ToString());

                CategoryListInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/CostumeInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, CostumeInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EquipmentInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EquipmentInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EquipmentSetInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EquipmentSetInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/fusionInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, fusionInfoDic);
            fs.Close();
        }


        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ItemInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ItemInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ItemValueInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ItemValueInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ContentsListInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ContentsListInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/CategoryListInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, CategoryListInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        CostumeInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, CostumeInfo>>("CostumeInfo");
//        EquipmentInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, EquipmentInfo>>("EquipmentInfo");
//        EquipmentSetInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, EquipmentSetInfo>>("EquipmentSetInfo");
//        fusionInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, fusionInfo>>("fusionInfo");
//        ItemInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, ItemInfo>>("ItemInfo");
//        ItemValueInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, ItemValueInfo>>("ItemValueInfo");

        _LowDataMgr.instance.DeserializeData<Dictionary<uint, CostumeInfo>>("CostumeInfo", (data) =>
        {
            CostumeInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, EquipmentInfo>>("EquipmentInfo", (data) =>
        {
            EquipmentInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, EquipmentSetInfo>>("EquipmentSetInfo", (data) =>
        {
            EquipmentSetInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, fusionInfo>>("fusionInfo", (data) =>
        {
            fusionInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, ItemInfo>>("ItemInfo", (data) =>
        {
            ItemInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, ItemValueInfo>>("ItemValueInfo", (data) =>
        {
            ItemValueInfoDic = data;
        });
        _LowDataMgr.instance.DeserializeData<List<ContentsListInfo>>("ContentsListInfo", (data) =>
        {
            ContentsListInfoList = data;
        });
        _LowDataMgr.instance.DeserializeData<List<CategoryListInfo>>("CategoryListInfo", (data) =>
        {
            CategoryListInfoList = data;
        });

    }
}
