using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Loading
{
    [Serializable]
    public class LoadingInfo
    {
        public uint Id;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _Panel;
        public byte Panel
        {
            set { _Panel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Panel); }
        }
        uint _Bgmplay;
        public uint Bgmplay
        {
            set { _Bgmplay = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Bgmplay); }
        }
        public string Loadimg;
        uint _Loadtext;
        public uint Loadtext
        {
            set { _Loadtext = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Loadtext); }
        }
    }
    public List<LoadingInfo> LoadingInfoList = new List<LoadingInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Loading_Loading", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Loading = new JSONObject(strSrc);

            for (int i = 0; i < Loading.list.Count; i++)
            {
                LoadingInfo tmpInfo = new LoadingInfo();
                tmpInfo.Id = (uint)Loading[i]["Id_ui"].n;
                tmpInfo.Type = (byte)Loading[i]["Type_b"].n;
                tmpInfo.Panel = (byte)Loading[i]["Panel_b"].n;
                tmpInfo.Bgmplay = (uint)Loading[i]["Bgmplay_ui"].n;
                tmpInfo.Loadimg = Loading[i]["Loadimg_c"].str;
                tmpInfo.Loadtext = (uint)Loading[i]["Loadtext_ui"].n;

                LoadingInfoList.Add(tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/LoadingInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, LoadingInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        LoadingInfoList = _LowDataMgr.instance.DeserializeData<List<LoadingInfo>>("LoadingInfo");
		_LowDataMgr.instance.DeserializeData<List<LoadingInfo>>("LoadingInfo", (data) => { LoadingInfoList = data; });

    }
}
