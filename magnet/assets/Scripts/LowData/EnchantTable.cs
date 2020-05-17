using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Enchant
{
    [Serializable]
    public class BreakInfo
    {
        public uint Id;
        uint _ItemIdx1;
        public uint ItemIdx1
        {
            set { _ItemIdx1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx1); }
        }
        ushort _ItemValue1;
        public ushort ItemValue1
        {
            set { _ItemValue1 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue1); }
        }
        uint _ItemIdx2;
        public uint ItemIdx2
        {
            set { _ItemIdx2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx2); }
        }
        ushort _ItemValue2;
        public ushort ItemValue2
        {
            set { _ItemValue2 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue2); }
        }
        uint _ItemIdx3;
        public uint ItemIdx3
        {
            set { _ItemIdx3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx3); }
        }
        ushort _ItemValue3;
        public ushort ItemValue3
        {
            set { _ItemValue3 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue3); }
        }
    }
    public Dictionary<uint, BreakInfo> BreakInfoDic = new Dictionary<uint, BreakInfo>();

    [Serializable]
    public class EnchantInfo
    {
        public uint Id;
        byte _enchantCount;
        public byte enchantCount
        {
            set { _enchantCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_enchantCount); }
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
        uint _CostGold;
        public uint CostGold
        {
            set { _CostGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CostGold); }
        }
    }
    public List<EnchantInfo> EnchantInfoList = new List<EnchantInfo>();

    [Serializable]
    public class EvolveInfo
    {
        public uint evolveId;
        byte _evolveCount;
        public byte evolveCount
        {
            set { _evolveCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_evolveCount); }
        }
        uint _ItemIdx1;
        public uint ItemIdx1
        {
            set { _ItemIdx1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx1); }
        }
        ushort _ItemValue1;
        public ushort ItemValue1
        {
            set { _ItemValue1 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue1); }
        }
        byte _ItemAdd1;
        public byte ItemAdd1
        {
            set { _ItemAdd1 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ItemAdd1); }
        }
        uint _ItemIdx2;
        public uint ItemIdx2
        {
            set { _ItemIdx2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx2); }
        }
        ushort _ItemValue2;
        public ushort ItemValue2
        {
            set { _ItemValue2 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue2); }
        }
        byte _ItemAdd2;
        public byte ItemAdd2
        {
            set { _ItemAdd2 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ItemAdd2); }
        }
        uint _ItemIdx3;
        public uint ItemIdx3
        {
            set { _ItemIdx3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemIdx3); }
        }
        ushort _ItemValue3;
        public ushort ItemValue3
        {
            set { _ItemValue3 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ItemValue3); }
        }
        byte _ItemAdd3;
        public byte ItemAdd3
        {
            set { _ItemAdd3 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ItemAdd3); }
        }
        uint _CostGold;
        public uint CostGold
        {
            set { _CostGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CostGold); }
        }
        uint _GoldAdd;
        public uint GoldAdd
        {
            set { _GoldAdd = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GoldAdd); }
        }
    }
    public List<EvolveInfo> EvolveInfoList = new List<EvolveInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Enchant_Break", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Break = new JSONObject(strSrc);

            for (int i = 0; i < Break.list.Count; i++)
            {
                BreakInfo tmpInfo = new BreakInfo();
                tmpInfo.Id = (uint)Break[i]["Id_ui"].n;
                tmpInfo.ItemIdx1 = (uint)Break[i]["ItemIdx1_ui"].n;
                tmpInfo.ItemValue1 = (ushort)Break[i]["ItemValue1_us"].n;
                tmpInfo.ItemIdx2 = (uint)Break[i]["ItemIdx2_ui"].n;
                tmpInfo.ItemValue2 = (ushort)Break[i]["ItemValue2_us"].n;
                tmpInfo.ItemIdx3 = (uint)Break[i]["ItemIdx3_ui"].n;
                tmpInfo.ItemValue3 = (ushort)Break[i]["ItemValue3_us"].n;

                BreakInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Enchant_Enchant", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Enchant = new JSONObject(strSrc);

            for (int i = 0; i < Enchant.list.Count; i++)
            {
                EnchantInfo tmpInfo = new EnchantInfo();
                tmpInfo.Id = (uint)Enchant[i]["Id_ui"].n;
                tmpInfo.enchantCount = (byte)Enchant[i]["enchantCount_b"].n;
                tmpInfo.ItemIdx1 = (uint)Enchant[i]["ItemIdx1_ui"].n;
                tmpInfo.ItemValue1 = (uint)Enchant[i]["ItemValue1_ui"].n;
                tmpInfo.ItemIdx2 = (uint)Enchant[i]["ItemIdx2_ui"].n;
                tmpInfo.ItemValue2 = (uint)Enchant[i]["ItemValue2_ui"].n;
                tmpInfo.ItemIdx3 = (uint)Enchant[i]["ItemIdx3_ui"].n;
                tmpInfo.ItemValue3 = (uint)Enchant[i]["ItemValue3_ui"].n;
                tmpInfo.ItemIdx4 = (uint)Enchant[i]["ItemIdx4_ui"].n;
                tmpInfo.ItemValue4 = (uint)Enchant[i]["ItemValue4_ui"].n;
                tmpInfo.ItemIdx5 = (uint)Enchant[i]["ItemIdx5_ui"].n;
                tmpInfo.ItemValue5 = (uint)Enchant[i]["ItemValue5_ui"].n;
                tmpInfo.CostGold = (uint)Enchant[i]["CostGold_ui"].n;

                EnchantInfoList.Add(tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Enchant_Evolve", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Evolve = new JSONObject(strSrc);

            for (int i = 0; i < Evolve.list.Count; i++)
            {
                EvolveInfo tmpInfo = new EvolveInfo();
                tmpInfo.evolveId = (uint)Evolve[i]["evolveId_ui"].n;
                tmpInfo.evolveCount = (byte)Evolve[i]["evolveCount_b"].n;
                tmpInfo.ItemIdx1 = (uint)Evolve[i]["ItemIdx1_ui"].n;
                tmpInfo.ItemValue1 = (ushort)Evolve[i]["ItemValue1_us"].n;
                tmpInfo.ItemAdd1 = (byte)Evolve[i]["ItemAdd1_b"].n;
                tmpInfo.ItemIdx2 = (uint)Evolve[i]["ItemIdx2_ui"].n;
                tmpInfo.ItemValue2 = (ushort)Evolve[i]["ItemValue2_us"].n;
                tmpInfo.ItemAdd2 = (byte)Evolve[i]["ItemAdd2_b"].n;
                tmpInfo.ItemIdx3 = (uint)Evolve[i]["ItemIdx3_ui"].n;
                tmpInfo.ItemValue3 = (ushort)Evolve[i]["ItemValue3_us"].n;
                tmpInfo.ItemAdd3 = (byte)Evolve[i]["ItemAdd3_b"].n;
                tmpInfo.CostGold = (uint)Evolve[i]["CostGold_ui"].n;
                tmpInfo.GoldAdd = (uint)Evolve[i]["GoldAdd_ui"].n;

                EvolveInfoList.Add(tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/BreakInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, BreakInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EnchantInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EnchantInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EvolveInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EvolveInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        BreakInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, BreakInfo>>("BreakInfo");
//        EnchantInfoList = _LowDataMgr.instance.DeserializeData<List<EnchantInfo>>("EnchantInfo");
//        EvolveInfoList = _LowDataMgr.instance.DeserializeData<List<EvolveInfo>>("EvolveInfo");


		_LowDataMgr.instance.DeserializeData<Dictionary<uint, BreakInfo>>("BreakInfo", (data) => { BreakInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<EnchantInfo>>("EnchantInfo", (data) => { EnchantInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<EvolveInfo>>("EvolveInfo", (data) => { EvolveInfoList = data; });


    }
}
