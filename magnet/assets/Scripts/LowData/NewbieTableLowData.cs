using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Newbie
{
    [Serializable]
    public class NewbieInfo
    {
        public uint Id;
        uint _defaultItemidx1;
        public uint defaultItemidx1
        {
            set { _defaultItemidx1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx1); }
        }
        uint _defaultItemidx2;
        public uint defaultItemidx2
        {
            set { _defaultItemidx2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx2); }
        }
        uint _defaultItemidx3;
        public uint defaultItemidx3
        {
            set { _defaultItemidx3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx3); }
        }
        uint _defaultItemidx4;
        public uint defaultItemidx4
        {
            set { _defaultItemidx4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx4); }
        }
        uint _defaultItemidx5;
        public uint defaultItemidx5
        {
            set { _defaultItemidx5 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx5); }
        }
        uint _defaultItemidx6;
        public uint defaultItemidx6
        {
            set { _defaultItemidx6 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_defaultItemidx6); }
        }
        uint _CostumIdx;
        public uint CostumIdx
        {
            set { _CostumIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CostumIdx); }
        }
        ushort _Gold;
        public ushort Gold
        {
            set { _Gold = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Gold); }
        }
        byte _Enemy;
        public byte Enemy
        {
            set { _Enemy = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Enemy); }
        }
        public JsonCustomData Item;
        uint _itemCount;
        public uint itemCount
        {
            set { _itemCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_itemCount); }
        }
    }
    public Dictionary<uint, NewbieInfo> NewbieInfoDic = new Dictionary<uint, NewbieInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Newbie_Newbie", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Newbie = new JSONObject(strSrc);

            for (int i = 0; i < Newbie.list.Count; i++)
            {
                NewbieInfo tmpInfo = new NewbieInfo();
                tmpInfo.Id = (uint)Newbie[i]["Id_ui"].n;
                tmpInfo.defaultItemidx1 = (uint)Newbie[i]["defaultItemidx1_ui"].n;
                tmpInfo.defaultItemidx2 = (uint)Newbie[i]["defaultItemidx2_ui"].n;
                tmpInfo.defaultItemidx3 = (uint)Newbie[i]["defaultItemidx3_ui"].n;
                tmpInfo.defaultItemidx4 = (uint)Newbie[i]["defaultItemidx4_ui"].n;
                tmpInfo.defaultItemidx5 = (uint)Newbie[i]["defaultItemidx5_ui"].n;
                tmpInfo.defaultItemidx6 = (uint)Newbie[i]["defaultItemidx6_ui"].n;
                tmpInfo.CostumIdx = (uint)Newbie[i]["CostumIdx_ui"].n;
                tmpInfo.Gold = (ushort)Newbie[i]["Gold_us"].n;
                tmpInfo.Enemy = (byte)Newbie[i]["Enemy_b"].n;
                tmpInfo.Item = new JsonCustomData(Newbie[i]["Item_j"].ToString());
                tmpInfo.itemCount = (uint)Newbie[i]["itemCount_ui"].n;

                NewbieInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/NewbieInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, NewbieInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        NewbieInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, NewbieInfo>>("NewbieInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, NewbieInfo>>("NewbieInfo", (data) => { NewbieInfoDic = data; });

    }
}
