using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Map
{
    [Serializable]
    public class MapDataInfo
    {
        public uint id;
        byte _type;
        public byte type
        {
            set { _type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_type); }
        }
        public string scene;
        public string maskfile;
        uint _regenposX;
        public uint regenposX
        {
            set { _regenposX = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_regenposX); }
        }
        uint _regenposY;
        public uint regenposY
        {
            set { _regenposY = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_regenposY); }
        }
        byte _AddColorR;
        public byte AddColorR
        {
            set { _AddColorR = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AddColorR); }
        }
        byte _AddColorG;
        public byte AddColorG
        {
            set { _AddColorG = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AddColorG); }
        }
        byte _AddColorB;
        public byte AddColorB
        {
            set { _AddColorB = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AddColorB); }
        }
        public float AddIntensity;
        public string Bgm;
        public float CharLightIntensity;
        public float ShadowStrength;
        byte _Bloom_Enable;
        public byte Bloom_Enable
        {
            set { _Bloom_Enable = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Bloom_Enable); }
        }

        public float Bloom_Threshhold;
        public float Bloom_Intensity;
        public float Bloom_BlurSize;
        public float Bloom_BlurIterations;
    }
    public Dictionary<uint, MapDataInfo> MapDataInfoDic = new Dictionary<uint, MapDataInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Map_MapData", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject MapData = new JSONObject(strSrc);

            for (int i = 0; i < MapData.list.Count; i++)
            {
                MapDataInfo tmpInfo = new MapDataInfo();
                tmpInfo.id = (uint)MapData[i]["id_ui"].n;
                tmpInfo.type = (byte)MapData[i]["type_b"].n;
                tmpInfo.scene = MapData[i]["scene_c"].str;
                tmpInfo.maskfile = MapData[i]["maskfile_c"].str;
                tmpInfo.regenposX = (uint)MapData[i]["regenposX_ui"].n;
                tmpInfo.regenposY = (uint)MapData[i]["regenposY_ui"].n;
                tmpInfo.AddColorR = (byte)MapData[i]["AddColorR_b"].n;
                tmpInfo.AddColorG = (byte)MapData[i]["AddColorG_b"].n;
                tmpInfo.AddColorB = (byte)MapData[i]["AddColorB_b"].n;
                tmpInfo.AddIntensity = (float)MapData[i]["AddIntensity_f"].n;
                tmpInfo.Bgm = MapData[i]["Bgm_c"].str;
                tmpInfo.CharLightIntensity = (float)MapData[i]["CharLightIntensity_f"].n;
                tmpInfo.ShadowStrength = (float)MapData[i]["ShadowStrength_f"].n;
                tmpInfo.Bloom_Enable = (byte)MapData[i]["Bloom_Enable_b"].n;

                tmpInfo.Bloom_Threshhold = (float)MapData[i]["Bloom_Threshhold_f"].n;
                tmpInfo.Bloom_Intensity = (float)MapData[i]["Bloom_Intensity_f"].n;
                tmpInfo.Bloom_BlurSize = (float)MapData[i]["Bloom_BlurSize_f"].n;
                tmpInfo.Bloom_BlurIterations = (float)MapData[i]["Bloom_BlurIterrations_f"].n;

                MapDataInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/MapDataInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, MapDataInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        MapDataInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, MapDataInfo>>("MapDataInfo");
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, MapDataInfo>>("MapDataInfo", (data) => { MapDataInfoDic = data; });
    }
}
