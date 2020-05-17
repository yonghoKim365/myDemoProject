using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NpcData
{
    [Serializable]
    public class NpcInfo
    {
        public uint Id;
        uint _DescriptionId;
        public uint DescriptionId
        {
            set { _DescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_DescriptionId); }
        }
        public string prefab;
        public string PortraitId;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
    }
    public Dictionary<uint, NpcInfo> NpcInfoDic = new Dictionary<uint, NpcInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/NpcData_Npc", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Npc = new JSONObject(strSrc);

            for (int i = 0; i < Npc.list.Count; i++)
            {
                NpcInfo tmpInfo = new NpcInfo();
                tmpInfo.Id = (uint)Npc[i]["Id_ui"].n;
                tmpInfo.DescriptionId = (uint)Npc[i]["DescriptionId_ui"].n;
                tmpInfo.prefab = Npc[i]["prefab_c"].str;
                tmpInfo.PortraitId = Npc[i]["PortraitId_c"].str;
                tmpInfo.Type = (byte)Npc[i]["Type_b"].n;

                NpcInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/NpcInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, NpcInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
        //NpcInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, NpcInfo>>("NpcInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, NpcInfo>>("NpcInfo", (data) => { NpcInfoDic = data; });

    }
}