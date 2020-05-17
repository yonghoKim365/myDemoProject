using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Price
{
    [Serializable]
    public class PriceInfo
    {
        public uint LogIdx;
        ushort _Type;
        public ushort Type
        {
            set { _Type = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Type); }
        }
        uint _ResetType;
        public uint ResetType
        {
            set { _ResetType = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ResetType); }
        }
        byte _ResetCount;
        public byte ResetCount
        {
            set { _ResetCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ResetCount); }
        }
        uint _ResetValue;
        public uint ResetValue
        {
            set { _ResetValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ResetValue); }
        }
    }

    public List<PriceInfo> PriceInfoList = new List<PriceInfo>();
    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Price_Price", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Price = new JSONObject(strSrc);

            for (int i = 0; i < Price.list.Count; i++)
            {
                PriceInfo tmpInfo = new PriceInfo();
                tmpInfo.LogIdx = (uint)Price[i]["LogIdx_ui"].n;
                tmpInfo.Type = (ushort)Price[i]["Type_us"].n;
                tmpInfo.ResetType = (uint)Price[i]["ResetType_ui"].n;
                tmpInfo.ResetCount = (byte)Price[i]["ResetCount_b"].n;
                tmpInfo.ResetValue = (uint)Price[i]["ResetValue_ui"].n;

                PriceInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PriceInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PriceInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        PriceInfoList = _LowDataMgr.instance.DeserializeData<List<PriceInfo>>("PriceInfo");

		_LowDataMgr.instance.DeserializeData<List<PriceInfo>>("PriceInfo", (data) => { PriceInfoList = data; });


    }
}
