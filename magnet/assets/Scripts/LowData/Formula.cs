using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Formula
{
    [Serializable]
    public class battlepointInfo
    {
        public uint id;
        public string key;
        public float Value;
    }
    public Dictionary<uint, battlepointInfo> battlepointInfoDic = new Dictionary<uint, battlepointInfo>();

    [Serializable]
    public class formulaInfo
    {
        public uint id;
        public string key;
        public float Value1;
        public float Value2;
        public float Value3;
    }
    public Dictionary<uint, formulaInfo> formulaInfoDic = new Dictionary<uint, formulaInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Formula_battlepoint", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject battlepoint = new JSONObject(strSrc);

            for (int i = 0; i < battlepoint.list.Count; i++)
            {
                battlepointInfo tmpInfo = new battlepointInfo();
                tmpInfo.id = (uint)battlepoint[i]["id_ui"].n;
                tmpInfo.key = battlepoint[i]["key_c"].str;
                tmpInfo.Value = (float)battlepoint[i]["Value_f"].n;

                battlepointInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Formula_formula", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject formula = new JSONObject(strSrc);

            for (int i = 0; i < formula.list.Count; i++)
            {
                formulaInfo tmpInfo = new formulaInfo();
                tmpInfo.id = (uint)formula[i]["id_ui"].n;
                tmpInfo.key = formula[i]["key_c"].str;
                tmpInfo.Value1 = (float)formula[i]["Value1_f"].n;
                tmpInfo.Value2 = (float)formula[i]["Value2_f"].n;
                tmpInfo.Value3 = (float)formula[i]["Value3_f"].n;

                formulaInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/battlepointInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, battlepointInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/formulaInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, formulaInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        battlepointInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, battlepointInfo>>("battlepointInfo");
//        formulaInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, formulaInfo>>("formulaInfo");


		_LowDataMgr.instance.DeserializeData<Dictionary<uint, battlepointInfo>>("battlepointInfo", (data) => { battlepointInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, formulaInfo>>("formulaInfo", (data) => { formulaInfoDic = data; });


    }
}
