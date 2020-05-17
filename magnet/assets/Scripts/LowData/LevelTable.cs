using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Level
{
    [Serializable]
    public class LevelInfo
    {
        public byte Level;
        uint _Exp;
        public uint Exp
        {
            set { _Exp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Exp); }
        }
        uint _Expoverlab;
        public uint Expoverlab
        {
            set { _Expoverlab = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Expoverlab); }
        }
        ushort _EnergyMax;
        public ushort EnergyMax
        {
            set { _EnergyMax = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_EnergyMax); }
        }
    }
    public Dictionary<byte, LevelInfo> LevelInfoDic = new Dictionary<byte, LevelInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Level_Level", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Level = new JSONObject(strSrc);

            for (int i = 0; i < Level.list.Count; i++)
            {
                LevelInfo tmpInfo = new LevelInfo();
                tmpInfo.Level = (byte)Level[i]["Level_b"].n;
                tmpInfo.Exp = (uint)Level[i]["Exp_ui"].n;
                tmpInfo.Expoverlab = (uint)Level[i]["Expoverlab_ui"].n;
                tmpInfo.EnergyMax = (ushort)Level[i]["EnergyMax_us"].n;

                LevelInfoDic.Add(tmpInfo.Level, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/LevelInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, LevelInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        LevelInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<byte, LevelInfo>>("LevelInfo");


		_LowDataMgr.instance.DeserializeData<Dictionary<byte, LevelInfo>>("LevelInfo", (data) => { LevelInfoDic = data; });


    }
}