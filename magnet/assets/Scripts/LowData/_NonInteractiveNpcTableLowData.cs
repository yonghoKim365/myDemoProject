using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NonInteractiveNpcData
{
    [Serializable]
	public class NonInteractiveNpcDataInfo
    {
        public uint Id;
		public string prefab;
		public float  scale;
		public float  moveSpeed;
		public ushort firstPosMax;
		public ushort firstPosMin;
		public ushort moveRange;
        
    }
	public Dictionary<uint, NonInteractiveNpcDataInfo> NonInteractiveNpcInfoDic = new Dictionary<uint, NonInteractiveNpcDataInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/NpcData_NonNpc", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Npc = new JSONObject(strSrc);

            for (int i = 0; i < Npc.list.Count; i++)
            {
				NonInteractiveNpcDataInfo tmpInfo = new NonInteractiveNpcDataInfo();
                tmpInfo.Id = (uint)Npc[i]["ID_ui"].n;
				tmpInfo.prefab = Npc[i]["prefab_c"].str;
				tmpInfo.scale = (float)Npc[i]["scale_f"].n;
				tmpInfo.moveSpeed = (float)Npc[i]["movespeed_f"].n;

				tmpInfo.firstPosMax = (ushort)Npc[i]["firstposmin_us"].n;
				tmpInfo.firstPosMin = (ushort)Npc[i]["firstposmax_us"].n;
				tmpInfo.moveRange = (ushort)Npc[i]["moverange_us"].n;
                
				NonInteractiveNpcInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/NonInteractiveNpcInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, NonInteractiveNpcInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
		//NonInteractiveNpcInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, NonInteractiveNpcDataInfo>>("NonInteractiveNpcInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, NonInteractiveNpcDataInfo>>("NonInteractiveNpcInfo", (data) => { NonInteractiveNpcInfoDic = data; });

    }
}