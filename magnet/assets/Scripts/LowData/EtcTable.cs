using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Etc
{
    [Serializable]
    public class EtcInfo
    {
        public uint Idx;
        public string Key;
        public string Value;
    }
    public Dictionary<uint, EtcInfo> EtcInfoDic = new Dictionary<uint, EtcInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Etc_Etc", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Etc = new JSONObject(strSrc);

            for (int i = 0; i < Etc.list.Count; i++)
            {
                EtcInfo tmpInfo = new EtcInfo();
                tmpInfo.Idx = (uint)Etc[i]["Idx_ui"].n;
                tmpInfo.Key = Etc[i]["Key_c"].str;
                tmpInfo.Value = Etc[i]["Value_c"].str;

                EtcInfoDic.Add(tmpInfo.Idx, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EtcInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EtcInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        EtcInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, EtcInfo>>("EtcInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, EtcInfo>>("EtcInfo", (data) => { EtcInfoDic = data; });



    }
}
