using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Shop
{
    [Serializable]
    public class ShopInfo
    {
        public uint LogIdx;
        byte _Openlevel;
        public byte Openlevel
        {
            set { _Openlevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Openlevel); }
        }
        ushort _Category;
        public ushort Category
        {
            set { _Category = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Category); }
        }
        ushort _Type;
        public ushort Type
        {
            set { _Type = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Type); }
        }
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        byte _ShopItemType;
        public byte ShopItemType
        {
            set { _ShopItemType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ShopItemType); }
        }
        uint _ShopItemList;
        public uint ShopItemList
        {
            set { _ShopItemList = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShopItemList); }
        }
        uint _ShopItemRate;
        public uint ShopItemRate
        {
            set { _ShopItemRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShopItemRate); }
        }
        byte _costType;
        public byte costType
        {
            set { _costType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_costType); }
        }
        uint _cost;
        public uint cost
        {
            set { _cost = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_cost); }
        }
        uint _ItemCount;
        public uint ItemCount
        {
            set { _ItemCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ItemCount); }
        }
        ulong _shopopentime;
        public ulong shopopentime
        {
            set { _shopopentime = EncryptHelper.SSecureULONG(value); }
            get { return EncryptHelper.GSecureULONG(_shopopentime); }
        }
        ulong _shopclosetime;
        public ulong shopclosetime
        {
            set { _shopclosetime = EncryptHelper.SSecureULONG(value); }
            get { return EncryptHelper.GSecureULONG(_shopclosetime); }
        }
    }

    public List<ShopInfo> ShopInfoList = new List<ShopInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Shop_Shop", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Shop = new JSONObject(strSrc);

            for (int i = 0; i < Shop.list.Count; i++)
            {
                ShopInfo tmpInfo = new ShopInfo();
                tmpInfo.LogIdx = (uint)Shop[i]["LogIdx_ui"].n;
                tmpInfo.Openlevel = (byte)Shop[i]["Openlevel_b"].n;
                tmpInfo.Category = (ushort)Shop[i]["Category_us"].n;
                tmpInfo.Type = (ushort)Shop[i]["Type_us"].n;
                tmpInfo.Class = (byte)Shop[i]["Class_b"].n;
                tmpInfo.ShopItemType = (byte)Shop[i]["ShopItemType_b"].n;
                tmpInfo.ShopItemList = (uint)Shop[i]["ShopItemList_ui"].n;
                tmpInfo.ShopItemRate = (uint)Shop[i]["ShopItemRate_ui"].n;
                tmpInfo.costType = (byte)Shop[i]["costType_b"].n;
                tmpInfo.cost = (uint)Shop[i]["cost_ui"].n;
                tmpInfo.ItemCount = (uint)Shop[i]["ItemCount_ui"].n;
                tmpInfo.shopopentime = (ulong)Shop[i]["shopopentime_ul"].n;
                tmpInfo.shopclosetime = (ulong)Shop[i]["shopclosetime_ul"].n;

                ShopInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ShopInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ShopInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        ShopInfoList = _LowDataMgr.instance.DeserializeData<List<ShopInfo>>("ShopInfo");

		_LowDataMgr.instance.DeserializeData<List<ShopInfo>>("ShopInfo", (data) => { ShopInfoList = data; });

    }
}
