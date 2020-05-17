using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Vip
{
	[Serializable]
	public class VipLevelInfo
	{
		public uint Id;
		byte _VipGrade;
		public byte VipGrade {
			set { _VipGrade = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_VipGrade); }
		}
		uint _needexp;
		public uint needexp {
			set { _needexp = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_needexp); }
		}
	}
    [Serializable]
    public class VipDataInfo
    {
        public uint Id;
        byte _VipGrade;
        public byte VipGrade
        {
            set { _VipGrade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_VipGrade); }
        }
        byte _type;
        public byte type
        {
            set { _type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_type); }
        }
        uint _Typevalue;
        public uint Typevalue
        {
            set { _Typevalue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Typevalue); }
        }
        byte _Shopvalue;
        public byte Shopvalue
        {
            set { _Shopvalue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Shopvalue); }
        }
    }
    [Serializable]
    public class VipStageInfo
    {
        public uint Id;
        byte _VipGrade;
        public byte VipGrade
        {
            set { _VipGrade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_VipGrade); }
        }
        byte _type;
        public byte type
        {
            set { _type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_type); }
        }
        public float addvalue;
    }




    public Dictionary<uint, VipLevelInfo>  VipLevelInfoDic = new Dictionary<uint, VipLevelInfo> ();
    public List<VipDataInfo> VipDataInfoList = new List<VipDataInfo>();
    public List<VipStageInfo> VipStageInfoList = new List<VipStageInfo>();

    public void LoadLowData()
	{
		{
			TextAsset data = Resources.Load("TestJson/Vip_VipLevel", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject VipLevel = new JSONObject(strSrc);

			for (int i = 0; i < VipLevel.list.Count; i++)
			{
				VipLevelInfo tmpInfo = new VipLevelInfo();
				tmpInfo.Id = (uint)VipLevel[i]["Id_ui"].n;
				tmpInfo.VipGrade = (byte)VipLevel[i]["VipGrade_b"].n;
				tmpInfo.needexp = (uint)VipLevel[i]["needexp_ui"].n;

			VipLevelInfoDic.Add(tmpInfo.Id, tmpInfo);
			}
		}
        {
            TextAsset data = Resources.Load("TestJson/VIP_VipData", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject VipData = new JSONObject(strSrc);

            for (int i = 0; i < VipData.list.Count; i++)
            {
                VipDataInfo tmpInfo = new VipDataInfo();
                tmpInfo.Id = (uint)VipData[i]["Id_ui"].n;
                tmpInfo.VipGrade = (byte)VipData[i]["VipGrade_b"].n;
                tmpInfo.type = (byte)VipData[i]["type_b"].n;
                tmpInfo.Typevalue = (uint)VipData[i]["Typevalue_ui"].n;
                tmpInfo.Shopvalue = (byte)VipData[i]["Shopvalue_b"].n;

                VipDataInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Vip_VipStage", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject VipStage = new JSONObject(strSrc);

            for (int i = 0; i < VipStage.list.Count; i++)
            {
                VipStageInfo tmpInfo = new VipStageInfo();
                tmpInfo.Id = (uint)VipStage[i]["Id_ui"].n;
                tmpInfo.VipGrade = (byte)VipStage[i]["VipGrade_b"].n;
                tmpInfo.type = (byte)VipStage[i]["type_b"].n;
                tmpInfo.addvalue = (float)VipStage[i]["addvalue_f"].n;

                VipStageInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            {
                FileStream fs = new FileStream("Assets/Resources/SerializeData/VipLevelInfo.txt", FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, VipLevelInfoDic);
                fs.Close();
            }
            {
                FileStream fs = new FileStream("Assets/Resources/SerializeData/VipDataInfo.txt", FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, VipDataInfoList);
                fs.Close();

            }
            {
                FileStream fs = new FileStream("Assets/Resources/SerializeData/VipStageInfo.txt", FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, VipStageInfoList);
                fs.Close();
            }

        }

    }



    public void DeserializeData()
	{
//		VipLevelInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, VipLevelInfo> >("VipLevelInfo");
//        VipDataInfoList = _LowDataMgr.instance.DeserializeData<List<VipDataInfo>>("VipDataInfo");
//        VipStageInfoList = _LowDataMgr.instance.DeserializeData<List<VipStageInfo>>("VipStageInfo");


		_LowDataMgr.instance.DeserializeData<Dictionary<uint, VipLevelInfo> >("VipLevelInfo", (data) => { VipLevelInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<VipDataInfo>>("VipDataInfo", (data) => { VipDataInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<VipStageInfo>>("VipStageInfo", (data) => { VipStageInfoList = data; });

    }
}
